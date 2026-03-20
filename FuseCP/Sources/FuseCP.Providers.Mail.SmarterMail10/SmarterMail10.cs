// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Services.Protocols;
using System.Net;
using FuseCP.Mail.SM10;
using FuseCP.Providers;
using FuseCP.Providers.Common;
using FuseCP.Providers.Mail;
using FuseCP.Server.Utils;
using Microsoft.Win32;
using FileUtils = FuseCP.Providers.Utils.FileUtils;
using FuseCP.Providers.Mail.SM10.Extensions;
 
namespace FuseCP.Providers.Mail
{
	class SmarterMail10 : HostingServiceProviderBase, IMailServer
	{
		static string[] smListSettings = new string[] {
            "description",
			"disabled",
			"moderator",
			"password",
			"requirepassword",
			"whocanpost",
			"prependsubject",
			"maxmessagesize",
			"maxrecipients",
			"replytolist",
			"subject",
            "listtoaddress",
            "listfromaddress",
            "listreplytoaddress",
            "digestmode",
            "sendsubscribe",
            "sendunsubscribe",
            "allowunsubscribe",
            "disablelistcommand",
            "disablesubscribecommand"
		};

		#region Public Properties

		protected string AdminUsername
		{
			get { return ProviderSettings["AdminUsername"]; }
		}

		protected string AdminPassword
		{
			get { return ProviderSettings["AdminPassword"]; }
		}

		protected bool ImportDomainAdmin
		{
			get
			{
				bool res;
				bool.TryParse(ProviderSettings[Constants.ImportDomainAdmin], out res);
				return res;
			}
		}

		protected bool InheritDomainDefaultLimits
		{
			get
			{
				bool res;
				bool.TryParse(ProviderSettings[Constants.InheritDomainDefaultLimits], out res);
				return res;
			}
		}

		protected bool EnableDomainAdministrators
		{
			get
			{
				bool res;
				bool.TryParse(ProviderSettings[Constants.EnableDomainAdministrators], out res);
				return res;
			}
		}

		protected string DomainsPath
		{
			get { return FileUtils.EvaluateSystemVariables(ProviderSettings["DomainsPath"]); }
		}

		protected string ServerIP
		{
			get
			{
				string val = ProviderSettings["ServerIPAddress"];
				if (String.IsNullOrEmpty(val))
					return "127.0.0.1";

				string ip = val.Trim();
				if (ip.IndexOf(";") > -1)
				{
					string[] ips = ip.Split(';');
					ip = String.IsNullOrEmpty(ips[1]) ? ips[0] : ips[1]; // get internal IP part
				}
				return ip;
			}
		}

		protected string ServiceUrl
		{
			get { return ProviderSettings["ServiceUrl"]; }
		}
		#endregion

		#region Constants
		public const string SYSTEM_DOMAIN_ADMIN = "system.domain.admin";
		public const string SYSTEM_CATCH_ALL = "system.catch.all";
		#endregion

		#region IHostingServiceProvier methods

		public override void ChangeServiceItemsState(ServiceProviderItem[] items, bool enabled)
		{
			foreach (ServiceProviderItem item in items)
			{
				if (item is MailDomain)
				{
					try
					{
						// enable/disable mail domain
						if (DomainExists(item.Name))
						{
							MailDomain mailDomain = GetDomain(item.Name);
							mailDomain.Enabled = enabled;
							UpdateDomain(mailDomain);
						}
					}
					catch (Exception ex)
					{
						Log.WriteError(String.Format("Error switching '{0}' SmarterMail domain", item.Name), ex);
					}
				}
			}
		}

		public override void DeleteServiceItems(ServiceProviderItem[] items)
		{
			foreach (ServiceProviderItem item in items)
			{
				if (item is MailDomain)
				{
					try
					{
						// delete mail domain
						DeleteDomain(item.Name);
					}
					catch (Exception ex)
					{
						Log.WriteError(String.Format("Error deleting '{0}' SmarterMail domain", item.Name), ex);
					}
				}
			}
		}

		public override ServiceProviderItemDiskSpace[] GetServiceItemsDiskSpace(ServiceProviderItem[] items)
		{
			List<ServiceProviderItemDiskSpace> itemsDiskspace = new List<ServiceProviderItemDiskSpace>();

			// update items with diskspace
			foreach (ServiceProviderItem item in items)
			{
				if (item is MailAccount)
				{
					try
					{
                        svcUserAdmin users = new svcUserAdmin();
                        PrepareProxy(users);

                        StatInfoResult userStats = users.GetUserStats(AdminUsername, AdminPassword, item.Name, DateTime.Now, DateTime.Now);
                        if (!userStats.Result)
                        {
                            throw new Exception(userStats.Message);
                        }

                        Log.WriteStart(String.Format("Calculating mail account '{0}' size", item.Name));
                        // calculate disk space
                        ServiceProviderItemDiskSpace diskspace = new ServiceProviderItemDiskSpace();
                        diskspace.ItemId = item.Id;
                        //diskspace.DiskSpace = 0;
                        diskspace.DiskSpace = userStats.BytesSize;
                        itemsDiskspace.Add(diskspace);
                        Log.WriteEnd(String.Format("Calculating mail account '{0}' size", item.Name));
					}
					catch (Exception ex)
					{
						Log.WriteError(ex);
					}
				}
			}
			return itemsDiskspace.ToArray();
		}

		public override ServiceProviderItemBandwidth[] GetServiceItemsBandwidth(ServiceProviderItem[] items, DateTime since)
		{
			ServiceProviderItemBandwidth[] itemsBandwidth = new ServiceProviderItemBandwidth[items.Length];

			// update items with diskspace
			for (int i = 0; i < items.Length; i++)
			{
				ServiceProviderItem item = items[i];

				// create new bandwidth object
				itemsBandwidth[i] = new ServiceProviderItemBandwidth();
				itemsBandwidth[i].ItemId = item.Id;
				itemsBandwidth[i].Days = new DailyStatistics[0];

				if (item is MailDomain)
				{
					try
					{
						// get daily statistics
						itemsBandwidth[i].Days = GetDailyStatistics(since, item.Name);
					}
					catch (Exception ex)
					{
						Log.WriteError(ex);
						System.Diagnostics.Debug.WriteLine(ex);
					}
				}
			}

			return itemsBandwidth;
		}

		public DailyStatistics[] GetDailyStatistics(DateTime since, string maildomainName)
		{

			ArrayList days = new ArrayList();
			// read statistics
			DateTime now = DateTime.Now;
			DateTime date = since;

			try
			{
				while (date < now)
				{
					svcDomainAdmin domains = new svcDomainAdmin();
					PrepareProxy(domains);
					StatInfoResult result =
						domains.GetDomainStatistics(AdminUsername, AdminPassword, maildomainName, date, date);

					if (!result.Result)
						throw new Exception(result.Message);

					if (result.BytesReceived != 0 | result.BytesSent != 0)
					{
						DailyStatistics dailyStats = new DailyStatistics();
						dailyStats.Year = date.Year;
						dailyStats.Month = date.Month;
						dailyStats.Day = date.Day;
						dailyStats.BytesSent = result.BytesSent;
						dailyStats.BytesReceived = result.BytesReceived;
						days.Add(dailyStats);
					}

					// advance day
					date = date.AddDays(1);
				}
			}
			catch (Exception ex)
			{
				Log.WriteError("Could not get SmarterMail domain statistics", ex);
			}
			return (DailyStatistics[])days.ToArray(typeof(DailyStatistics));
		}

		#endregion

		#region Domains

		/// <summary>
		/// Checks whether the specified domain exists
		/// </summary>
		/// <param name="domainName">Domain name</param>
		/// <returns>true if the specified domain exists, otherwise false</returns>
		public bool DomainExists(string domainName)
		{
			try
			{
				svcDomainAdmin domains = new svcDomainAdmin();
				PrepareProxy(domains);

				DomainSettingsResult result = domains.GetDomainSettings(AdminUsername, AdminPassword, domainName);
				return result.Result;
			}
			catch (Exception ex)
			{
				throw new Exception("Could not check whether mail domain exists", ex);
			}
		}

		public string[] GetDomains()
		{
			try
			{
				svcDomainAdmin domains = new svcDomainAdmin();
				PrepareProxy(domains);

				DomainListResult result = domains.GetAllDomains(AdminUsername, AdminPassword);
				if (!result.Result)
					throw new Exception(result.Message);

				return result.DomainNames;
			}
			catch (Exception ex)
			{
				throw new Exception("Could not get the list of mail domains", ex);
			}
		}

		public MailDomain GetDomain(string domainName)
		{
			try
			{
				svcDomainAdmin domains = new svcDomainAdmin();
				PrepareProxy(domains);

				DomainSettingsResult result = domains.GetDomainSettings(AdminUsername, AdminPassword, domainName);
				if (!result.Result)
					throw new Exception(result.Message);

				// fill domain properties
				MailDomain domain = new MailDomain();
				domain.Name = domainName;
				domain.Path = result.Path;
				domain.ServerIP = result.ServerIP;
				domain.ImapPort = result.ImapPort;
				domain.SmtpPort = result.SmtpPort;
				domain.PopPort = result.PopPort;
				domain.MaxAliases = result.MaxAliases;
				domain.MaxDomainAliases = result.MaxDomainAliases;
				domain.MaxLists = result.MaxLists;
				domain.MaxDomainSizeInMB = result.MaxDomainSizeInMB;
				domain.MaxDomainUsers = result.MaxDomainUsers;
				domain.MaxMailboxSizeInMB = result.MaxMailboxSizeInMB;
				domain.MaxMessageSize = result.MaxMessageSize;
				domain.MaxRecipients = result.MaxRecipients;
				domain.RequireSmtpAuthentication = result.RequireSmtpAuthentication;
				domain.ListCommandAddress = result.ListCommandAddress;
				domain.ShowContentFilteringMenu = result.ShowContentFilteringMenu;
				domain.ShowDomainAliasMenu = result.ShowDomainAliasMenu;
				domain.ShowListMenu = result.ShowListMenu;
				domain.ShowSpamMenu = result.ShowSpamMenu;
				// get additional domain settings
				string[] requestedSettings = new string[]
                {
                    "catchall",
					"enablepopretrieval",
					"enablecatchalls",
                    "isenabled",
                    "ldapport",
                    "altsmtpport",
                    "sharedcalendar",
                    "sharedcontact",
                    "sharedfolder",
                    "sharednotes",
                    "sharedtasks",
                    "sharedgal",
                    "bypassforwardblacklist",
					"showdomainreports",
					"spamresponderoption",
					"spamforwardoption",
					"maxmessagesperhour",
					"maxmessagesperhourenabled",
					"maxsmtpoutbandwidthperhour",
					"maxsmtpoutbandwidthperhourenabled",
					"maxpopretrievalaccounts",
					"maxbouncesreceivedperhour",
					"maxbouncesreceivedperhourenabled",
                    "enableimapretrieval",
                    "enablemailsigning",
                    "enableemailreports",
                    "syncml"
				};

				SettingsRequestResult addResult = domains.GetRequestedDomainSettings(AdminUsername, AdminPassword, domainName, requestedSettings);
				if (!addResult.Result)
					throw new Exception(addResult.Message);

				FillMailDomainFields(domain, addResult);


				// get catch-all address
				if (!String.IsNullOrEmpty(domain.CatchAllAccount))
				{
					// get catch-all group
					string groupName = SYSTEM_CATCH_ALL + "@" + domain.Name;
					if (GroupExists(groupName))
					{
						// get the first member of this group
						MailGroup group = GetGroup(groupName);

						if (group.Members.Length > 0)
						{
							domain.CatchAllAccount = GetAccountName(group.Members[0]);
						}
					}
				}


				svcServerAdmin serverAdmin = new svcServerAdmin();
				PrepareProxy(serverAdmin);

				EditionResult licenseType = serverAdmin.GetEdition(AdminUsername, AdminPassword);

				if (licenseType.Edition == "PRO")
				{
					domain[MailDomain.SMARTERMAIL_LICENSE_TYPE] = "PRO";
				}
				if (licenseType.Edition == "ENT")
				{
					domain[MailDomain.SMARTERMAIL_LICENSE_TYPE] = "ENT";
				}
				if (licenseType.Edition == "FREE")
				{
					domain[MailDomain.SMARTERMAIL_LICENSE_TYPE] = "FREE";
				}


				return domain;
			}
			catch (Exception ex)
			{
				throw new Exception("Could not get mail domain", ex);
			}
		}

		public void CreateDomain(MailDomain domain)
		{
			try
			{

				svcDomainAdmin domains = new svcDomainAdmin();
				PrepareProxy(domains);

				DomainSettingsResult defaultDomainSettings = domains.GetDomainDefaults(AdminUsername, AdminPassword);

				SettingsRequestResult defaultRequestedSettings =
					domains.GetRequestedDomainDefaults(AdminUsername, AdminPassword, new string[]
                                                                                         {
                                                                                             "defaultaltsmtpport",
                                                                                             "defaultaltsmtpportenabled",
                                                                                             "defaultautoresponderrestriction",
                                                                                             "defaultbypassgreylisting",
                                                                                             "defaultenablecatchalls",
                                                                                             "defaultenabledomainkeys",
                                                                                             "defaultenableemailreports",
                                                                                             "defaultenablepopretrieval",
                                                                                             "defaultmaxmessagesperhour",
                                                                                             "defaultmaxmessagesperhourenabled",
                                                                                             "defaultmaxsmtpoutbandwidthperhour",
                                                                                             "defaultmaxsmtpoutbandwidthperhourenabled",
                                                                                             "defaultmaxbouncesreceivedperhour",
                                                                                             "defaultmaxbouncesreceivedperhourenabled",
                                                                                             "defaultmaxpopretrievalaccounts",
                                                                                             "defaultsharedcalendar",
                                                                                             "defaultsharedcontact",
                                                                                             "defaultsharedfolder",
                                                                                             "defaultsharedgal",
                                                                                             "defaultsharednotes",
                                                                                             "defaultsharedtasks",
                                                                                             "defaultshowcalendar",
                                                                                             "defaultshowcontacts",
                                                                                             "defaultshowcontentfilteringmenu",
                                                                                             "defaultshowdomainaliasmenu",
                                                                                             "defaultshowdomainreports",
                                                                                             "defaultshowlistmenu",
                                                                                             "defaultshownotes",
                                                                                             "defaultshowspammenu",
                                                                                             "defaultshowtasks",
                                                                                             "defaultshowuserreports",
                                                                                             "defaultskin",
                                                                                             "defaultspamresponderoption",
                                                                                             "defaultspamforwardoption"
                                                                                         });

				string[] requestedDomainDefaults = defaultRequestedSettings.settingValues;

				//domain Path is taken from FuseCP Service settings

				GenericResult result = null;

				if (!InheritDomainDefaultLimits)
				{
					result = domains.AddDomain(AdminUsername,
											   AdminPassword,
											   domain.Name,
											   Path.Combine(DomainsPath, domain.Name),
											   SYSTEM_DOMAIN_ADMIN, // admin username
											   Guid.NewGuid().ToString("P"), // admin password
											   "Domain", // admin first name
											   "Administrator", // admin last name
											   ServerIP,
											   defaultDomainSettings.ImapPort,
											   defaultDomainSettings.PopPort,
											   defaultDomainSettings.SmtpPort,
											   domain.MaxAliases,
											   domain.MaxDomainSizeInMB,
											   domain.MaxDomainUsers,
											   domain.MaxMailboxSizeInMB,
											   domain.MaxMessageSize,
											   domain.MaxRecipients,
											   domain.MaxDomainAliases,
											   domain.MaxLists,
											   defaultDomainSettings.ShowDomainAliasMenu,
						// ShowDomainAliasMenu
											   defaultDomainSettings.ShowContentFilteringMenu,
						// ShowContentFilteringMenu
											   defaultDomainSettings.ShowSpamMenu, // ShowSpamMenu
											   defaultDomainSettings.ShowStatsMenu, // ShowStatsMenu
											   defaultDomainSettings.RequireSmtpAuthentication,
											   defaultDomainSettings.ShowListMenu, // ShowListMenu
											   defaultDomainSettings.ListCommandAddress);
				}
				else
				{
					result = domains.AddDomain(AdminUsername, AdminPassword,
											   domain.Name,
											   Path.Combine(DomainsPath, domain.Name),
											   SYSTEM_DOMAIN_ADMIN, // admin username
											   Guid.NewGuid().ToString("P"), // admin password
											   "Domain", // admin first name
											   "Administrator", // admin last name
											   ServerIP,
											   defaultDomainSettings.ImapPort,
											   defaultDomainSettings.PopPort,
											   defaultDomainSettings.SmtpPort,
											   defaultDomainSettings.MaxAliases,
											   defaultDomainSettings.MaxDomainSizeInMB,
											   defaultDomainSettings.MaxDomainUsers,
											   defaultDomainSettings.MaxMailboxSizeInMB,
											   defaultDomainSettings.MaxMessageSize,
											   defaultDomainSettings.MaxRecipients,
											   defaultDomainSettings.MaxDomainAliases,
											   defaultDomainSettings.MaxLists,
											   defaultDomainSettings.ShowDomainAliasMenu, // ShowDomainAliasMenu
											   defaultDomainSettings.ShowContentFilteringMenu, // ShowContentFilteringMenu
											   defaultDomainSettings.ShowSpamMenu, // ShowSpamMenu
											   defaultDomainSettings.ShowStatsMenu, // ShowStatsMenu
											   defaultDomainSettings.RequireSmtpAuthentication,
											   defaultDomainSettings.ShowListMenu, // ShowListMenu
											   defaultDomainSettings.ListCommandAddress);
				}
				if (!result.Result)
					throw new Exception(result.Message);


				// update additional settings
				result = domains.SetRequestedDomainSettings(AdminUsername, AdminPassword, domain.Name,
															SetMailDomainDefaultSettings(requestedDomainDefaults));

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				if (DomainExists(domain.Name))
				{
					DeleteDomain(domain.Name);
				}
				Log.WriteError(ex);
				throw new Exception("Could not create mail domain", ex);
			}
		}

		public void UpdateDomain(MailDomain domain)
		{
			try
			{
				// load original domain
				MailDomain origDomain = GetDomain(domain.Name);

				svcDomainAdmin domains = new svcDomainAdmin();
				PrepareProxy(domains);

				GenericResult result = domains.UpdateDomain(AdminUsername, AdminPassword,
					domain.Name,
					origDomain.ServerIP,
					domain.ImapPort,
					domain.PopPort,
					domain.SmtpPort,
					domain.MaxAliases,
					domain.MaxDomainSizeInMB,
					domain.MaxDomainUsers,
					domain.MaxMailboxSizeInMB,
					domain.MaxMessageSize,
					domain.MaxRecipients,
					domain.MaxDomainAliases,
					domain.MaxLists,
					domain.ShowDomainAliasMenu, // ShowDomainAliasMenu
					domain.ShowContentFilteringMenu, // ShowContentFilteringMenu
					domain.ShowSpamMenu, // ShowSpamMenu
					domain.ShowsStatsMenu, // this parameter is no longer used in SM5
					origDomain.RequireSmtpAuthentication,
					domain.ShowListMenu, // Showlistmenu
					origDomain.ListCommandAddress);

				if (!result.Result)
					throw new Exception(result.Message);

				// update catch-all group
				UpdateDomainCatchAllGroup(domain.Name, domain.CatchAllAccount);


				// update additional settings
				result = domains.SetRequestedDomainSettings(AdminUsername, AdminPassword, domain.Name,
					new string[] {
                        "isenabled=" + domain.Enabled,
                        "catchall=" + (!String.IsNullOrEmpty(domain.CatchAllAccount) ? SYSTEM_CATCH_ALL : ""),
                        "altsmtpport=" + domain.SmtpPortAlt,
                        "ldapport=" + domain.LdapPort,
                        "sharedcalendar=" + domain.SharedCalendars,
                        "sharedcontact=" + domain.SharedContacts,
                        "sharedfolder=" + domain.SharedFolders,
                        "sharednotes=" + domain.SharedNotes,
                        "sharedtasks=" + domain.SharedTasks,
                        "sharedgal=" + domain.IsGlobalAddressList,
                        "enablecatchalls=" + domain[MailDomain.SMARTERMAIL5_CATCHALLS_ENABLED],
                        "bypassforwardblacklist=" + domain.BypassForwardBlackList,
						"showdomainreports=" + domain[MailDomain.SMARTERMAIL5_SHOW_DOMAIN_REPORTS],
						"maxmessagesperhour=" + domain[MailDomain.SMARTERMAIL5_MESSAGES_PER_HOUR],
					    "maxmessagesperhourenabled=" + domain[MailDomain.SMARTERMAIL5_MESSAGES_PER_HOUR_ENABLED],
					    "maxsmtpoutbandwidthperhour=" + domain[MailDomain.SMARTERMAIL5_BANDWIDTH_PER_HOUR],
					    "maxsmtpoutbandwidthperhourenabled=" + domain[MailDomain.SMARTERMAIL5_BANDWIDTH_PER_HOUR_ENABLED],
                        "enablepopretrieval=" + domain[MailDomain.SMARTERMAIL5_POP_RETREIVAL_ENABLED],
						"maxpopretrievalaccounts=" + domain[MailDomain.SMARTERMAIL5_POP_RETREIVAL_ACCOUNTS],
						"maxbouncesreceivedperhour=" + domain[MailDomain.SMARTERMAIL5_BOUNCES_PER_HOUR],
						"maxbouncesreceivedperhourenabled=" + domain[MailDomain.SMARTERMAIL5_BOUNCES_PER_HOUR_ENABLED],
                        "enableimapretrieval=" + domain[MailDomain.SMARTERMAIL6_IMAP_RETREIVAL_ENABLED],
                        "enablemailsigning=" + domain[MailDomain.SMARTERMAIL6_MAIL_SIGNING_ENABLED],
                        "enableemailreports=" + domain[MailDomain.SMARTERMAIL6_EMAIL_REPORTS_ENABLED],
                        "syncml=" + domain[MailDomain.SMARTERMAIL6_SYNCML_ENABLED]
				});

				/*
                                          
				string[] requestedSettings = new string[]
					{
						"maxmessagesperhour",
						"maxmessagesperhourenabled",
						"maxsmtpoutbandwidthperhour",
						"maxsmtpoutbandwidthperhourenabled"
					};

				SettingsRequestResult addResult =
					domains.GetRequestedDomainSettings(AdminUsername, AdminPassword, domain.Name, requestedSettings);
				*/

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not update mail domain", ex);
			}
		}

		public void DeleteDomain(string domainName)
		{
			try
			{
				svcDomainAdmin domains = new svcDomainAdmin();
				PrepareProxy(domains);

				GenericResult result = domains.DeleteDomain(AdminUsername, AdminPassword,
					domainName,
					true // delete files
					);

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not delete mail domain", ex);
			}
		}

		public bool DomainAliasExists(string domainName, string aliasName)
		{
			try
			{
				string[] aliases = GetDomainAliases(domainName);
				foreach (string alias in aliases)
				{
					if (String.Compare(alias, aliasName, true) == 0)
						return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				throw new Exception("Could not check whether mail domain alias exists", ex);
			}
		}

		public string[] GetDomainAliases(string domainName)
		{
			try
			{
				svcDomainAliasAdmin aliases = new svcDomainAliasAdmin();
				PrepareProxy(aliases);

				DomainAliasInfoListResult result = aliases.GetAliases(AdminUsername, AdminPassword, domainName);

				if (!result.Result)
					throw new Exception(result.Message);

				return result.DomainAliasNames;
			}
			catch (Exception ex)
			{
				throw new Exception("Could not get the list of mail domain aliases", ex);
			}
		}

		public void AddDomainAlias(string domainName, string aliasName)
		{
			try
			{
				svcDomainAliasAdmin aliases = new svcDomainAliasAdmin();
				PrepareProxy(aliases);

				GenericResult result = aliases.AddDomainAliasWithoutMxCheck(AdminUsername, AdminPassword,
					domainName, aliasName);

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not add mail domain alias", ex);
			}
		}

		public void DeleteDomainAlias(string domainName, string aliasName)
		{
			try
			{
				svcDomainAliasAdmin aliases = new svcDomainAliasAdmin();
				PrepareProxy(aliases);

				GenericResult result = aliases.DeleteDomainAlias(AdminUsername, AdminPassword,
					domainName, aliasName);

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not delete mail domain alias", ex);
			}
		}

		private static string[] SetMailDomainDefaultSettings(string[] defaultSettings)
		{
			List<string> settings = new List<string>();

			foreach (string pair in defaultSettings)
			{
				string[] parts = pair.Split('=');
				switch (parts[0])
				{
					case "defaultaltsmtpport":
						settings.Add("altsmtpport=" + parts[1]);
						break;
					case "defaultaltsmtpportenabled":
						settings.Add("altsmtpportenabled=" + parts[1]);
						break;
					case "defaultbypassgreylisting":
						settings.Add("bypassgreylisting=" + parts[1]);
						break;
					case "defaultenablecatchalls":
						if (String.Equals(parts[1], "Enabled"))
							settings.Add("enablecatchalls=True");
						if (String.Equals(parts[1], "Disabled"))
							settings.Add("enablecatchalls=False");
						break;
					case "defaultenabledomainkeys":
						settings.Add("enabledomainkeys=" + parts[1]);
						break;
					case "defaultenableemailreports":
						settings.Add("enableemailreports=" + parts[1]);
						break;
					case "defaultenablepopretrieval":
						settings.Add("enablepopretrieval=" + parts[1]);
						break;
					case "defaultautoresponderrestriction":
						settings.Add("autoresponderrestriction=" + parts[1]);
						break;
					case "defaultmaxmessagesperhour":
						settings.Add("maxmessagesperhour=" + parts[1]);
						break;
					case "defaultmaxmessagesperhourenabled":
						settings.Add("maxmessagesperhourenabled=" + parts[1]);
						break;
					case "defaultmaxsmtpoutbandwidthperhour":
						settings.Add("maxsmtpoutbandwidthperhour=" + parts[1]);
						break;
					case "defaultmaxsmtpoutbandwidthperhourenabled":
						settings.Add("maxsmtpoutbandwidthperhourenabled=" + parts[1]);
						break;
					case "defaultmaxbouncesreceivedperhour":
						settings.Add("maxbouncesreceivedperhour=" + parts[1]);
						break;
					case "defaultmaxbouncesreceivedperhourenabled":
						settings.Add("maxbouncesreceivedperhourenabled=" + parts[1]);
						break;
					case "defaultsharedcalendar":
						settings.Add("sharedcalendar=" + parts[1]);
						break;
					case "defaultsharedcontact":
						settings.Add("sharedcontact=" + parts[1]);
						break;
					case "defaultsharedfolder":
						settings.Add("sharedfolder=" + parts[1]);
						break;
					case "defaultsharedgal":
						settings.Add("sharedgal=" + parts[1]);
						break;
					case "defaultsharednotes":
						settings.Add("sharednotes=" + parts[1]);
						break;
					case "defaultsharedtasks":
						settings.Add("sharedtasks=" + parts[1]);
						break;

					case "defaultshowcalendar":
						settings.Add("showcalendar=" + parts[1]);
						break;
					case "defaultshowcontacts":
						settings.Add("showcontacts=" + parts[1]);
						break;
					case "defaultshowcontentfilteringmenu":
						settings.Add("showcontentfilteringmenu=" + parts[1]);
						break;
					case "defaultshowdomainaliasmenu":
						settings.Add("showdomainaliasmenu=" + parts[1]);
						break;
					case "defaultshowdomainreports":
						settings.Add("showdomainreports=" + parts[1]);
						break;
					case "defaultshowlistmenu":
						settings.Add("showlistmenu=" + parts[1]);
						break;
					case "defaultshownotes":
						settings.Add("shownotes=" + parts[1]);
						break;
					case "defaultshowtasks":
						settings.Add("showtasks=" + parts[1]);
						break;
					case "defaultshowuserreports":
						settings.Add("showuserreports=" + parts[1]);
						break;
					case "defaultshowspammenu":
						settings.Add("showspammenu=" + parts[1]);
						break;
					case "defaultspamresponderoption":
						settings.Add("spamresponderoption=" + parts[1]);
						break;
					case "defaultspamforwardoption":
						settings.Add("spamforwardoption=" + parts[1]);
						break;

					//  "defaultskin"
				}
			}
			return settings.ToArray();
		}

		private static void FillMailDomainFields(MailDomain domain, SettingsRequestResult addResult)
		{
			foreach (string pair in addResult.settingValues)
			{
				string[] parts = pair.Split('=');
				switch (parts[0])
				{
					case "catchall":
						domain.CatchAllAccount = parts[1];
						break;
					case "enablecatchalls":
						if (String.Equals(parts[1], "Enabled"))
							domain[MailDomain.SMARTERMAIL5_CATCHALLS_ENABLED] = "True";
						if (String.Equals(parts[1], "Disabled"))
							domain[MailDomain.SMARTERMAIL5_CATCHALLS_ENABLED] = "False";
						break;
					case "enablepopretrieval":
						domain[MailDomain.SMARTERMAIL5_POP_RETREIVAL_ENABLED] = parts[1];
						break;
					case "isenabled":
						domain.Enabled = Boolean.Parse(parts[1]);
						break;
					case "ldapport":
						domain.LdapPort = int.Parse(parts[1]);
						break;
					case "altsmtpport":
						domain.SmtpPortAlt = int.Parse(parts[1]);
						break;
					case "sharedcalendar":
						domain.SharedCalendars = Boolean.Parse(parts[1]);
						break;
					case "sharedcontact":
						domain.SharedContacts = Boolean.Parse(parts[1]);
						break;
					case "sharedfolder":
						domain.SharedFolders = Boolean.Parse(parts[1]);
						break;
					case "sharednotes":
						domain.SharedNotes = Boolean.Parse(parts[1]);
						break;
					case "sharedtasks":
						domain.SharedTasks = Boolean.Parse(parts[1]);
						break;
					case "sharedgal":
						domain.IsGlobalAddressList = Boolean.Parse(parts[1]);
						break;
					case "bypassforwardblacklist":
						domain.BypassForwardBlackList = Boolean.Parse(parts[1]);
						break;
					case "showdomainreports":
						domain[MailDomain.SMARTERMAIL5_SHOW_DOMAIN_REPORTS] = parts[1];
						break;
					case "maxmessagesperhour":
						domain[MailDomain.SMARTERMAIL5_MESSAGES_PER_HOUR] = parts[1];
						break;
					case "maxmessagesperhourenabled":
						domain[MailDomain.SMARTERMAIL5_MESSAGES_PER_HOUR_ENABLED] = parts[1];
						break;
					case "maxsmtpoutbandwidthperhour":
						domain[MailDomain.SMARTERMAIL5_BANDWIDTH_PER_HOUR] = parts[1];
						break;
					case "maxsmtpoutbandwidthperhourenabled":
						domain[MailDomain.SMARTERMAIL5_BANDWIDTH_PER_HOUR_ENABLED] = parts[1];
						break;
					case "maxpopretrievalaccounts":
						domain[MailDomain.SMARTERMAIL5_POP_RETREIVAL_ACCOUNTS] = parts[1];
						break;
					case "maxbouncesreceivedperhour":
						domain[MailDomain.SMARTERMAIL5_BOUNCES_PER_HOUR] = parts[1];
						break;
					case "maxbouncesreceivedperhourenabled":
						domain[MailDomain.SMARTERMAIL5_BOUNCES_PER_HOUR_ENABLED] = parts[1];
						break;
					case "enableimapretrieval":
						domain[MailDomain.SMARTERMAIL6_IMAP_RETREIVAL_ENABLED] = parts[1];
						break;
					case "enablemailsigning":
						domain[MailDomain.SMARTERMAIL6_MAIL_SIGNING_ENABLED] = parts[1];
						break;
					case "enableemailreports":
						domain[MailDomain.SMARTERMAIL6_EMAIL_REPORTS_ENABLED] = parts[1];
						break;
					case "syncml":
						domain[MailDomain.SMARTERMAIL6_SYNCML_ENABLED] = parts[1];
						break;
				}
			}
		}


		private void UpdateDomainCatchAllGroup(string domainName, string mailboxName)
		{
			// check if system catch all group exists
			string groupName = SYSTEM_CATCH_ALL + "@" + domainName;
			if (GroupExists(groupName))
			{
				// delete group
				DeleteGroup(groupName);
			}

			if (!String.IsNullOrEmpty(mailboxName))
			{
				// create catch-all group
				MailGroup group = new MailGroup();
				group.Name = groupName;
				group.Enabled = true;
				group.Members = new string[] { mailboxName + "@" + domainName };

				// create
				CreateGroup(group);
			}
		}


		#endregion

		#region Mail Accounts

		public bool AccountExists(string mailboxName)
		{
			try
			{
				svcUserAdmin users = new svcUserAdmin();
				PrepareProxy(users);

				UserInfoResult result = users.GetUser(AdminUsername, AdminPassword, mailboxName);

				return result.Result;
			}
			catch (Exception ex)
			{
				throw new Exception("Could not check whether mailbox exists", ex);
			}
		}

		public MailAccount[] GetAccounts(string domainName)
		{
			try
			{
				svcUserAdmin users = new svcUserAdmin();
				PrepareProxy(users);

				UserInfoListResult result = users.GetUsers(AdminUsername, AdminPassword, domainName);

				if (!result.Result)
					throw new Exception(result.Message);

				List<MailAccount> accounts = new List<MailAccount>();


				foreach (UserInfo user in result.Users)
				{
					if (user.IsDomainAdmin && !ImportDomainAdmin)
						continue;

					MailAccount account = new MailAccount();
					account.Name = user.UserName;
					account.Password = user.Password;
					accounts.Add(account);
				}
				return accounts.ToArray();
			}
			catch (Exception ex)
			{
				throw new Exception("Could not get the list of domain mailboxes", ex);
			}
		}

		public MailAccount GetAccount(string mailboxName)
		{
			try
			{
				svcUserAdmin users = new svcUserAdmin();
				PrepareProxy(users);

				UserInfoResult result = users.GetUser(AdminUsername, AdminPassword, mailboxName);

				if (!result.Result)
					throw new Exception(result.Message);

				MailAccount mailbox = new MailAccount();
				mailbox.Name = result.UserInfo.UserName;
				mailbox.Password = result.UserInfo.Password;
				mailbox.FirstName = result.UserInfo.FirstName;
				mailbox.LastName = result.UserInfo.LastName;
				mailbox.IsDomainAdmin = result.UserInfo.IsDomainAdmin;
				mailbox.IsDomainAdminEnabled = EnableDomainAdministrators;

				// get additional settings
				string[] requestedSettings = new string[]
                {
                    "isenabled",
                    "maxsize",
                    "lockpassword",
                    "replytoaddress",
                    "signature",
					"passwordlocked"
                };

				SettingsRequestResult addResult = users.GetRequestedUserSettings(AdminUsername, AdminPassword,
					mailboxName, requestedSettings);

				if (!addResult.Result)
					throw new Exception(addResult.Message);

				foreach (string pair in addResult.settingValues)
				{
					string[] parts = pair.Split('=');
					if (parts[0] == "isenabled") mailbox.Enabled = Boolean.Parse(parts[1]);
					else if (parts[0] == "maxsize") mailbox.MaxMailboxSize = Int32.Parse(parts[1]);
					else if (parts[0] == "passwordlocked") mailbox.PasswordLocked = Boolean.Parse(parts[1]);
					else if (parts[0] == "replytoaddress") mailbox.ReplyTo = parts[1];
					else if (parts[0] == "signature") mailbox.Signature = parts[1];
				}

				// get forwardings info
				UserForwardingInfoResult forwResult = users.GetUserForwardingInfo(AdminUsername, AdminPassword, mailboxName);

				if (!forwResult.Result)
					throw new Exception(forwResult.Message);

				string[] forwAddresses = forwResult.ForwardingAddress.Split(';', ',');
				List<string> listForAddresses = new List<string>();
				foreach (string forwAddress in forwAddresses)
				{
					if (!String.IsNullOrEmpty(forwAddress.Trim()))
						listForAddresses.Add(forwAddress.Trim());
				}

				mailbox.ForwardingAddresses = listForAddresses.ToArray();
				mailbox.DeleteOnForward = forwResult.DeleteOnForward;

				// get autoresponder info
				UserAutoResponseResult respResult = users.GetUserAutoResponseInfo(AdminUsername, AdminPassword, mailboxName);

				if (!respResult.Result)
					throw new Exception(respResult.Message);

				mailbox.ResponderEnabled = respResult.Enabled;
				mailbox.ResponderSubject = respResult.Subject;
				mailbox.ResponderMessage = respResult.Body;

				return mailbox;
			}
			catch (Exception ex)
			{
				throw new Exception("Could not get mailbox", ex);
			}
		}

		public void CreateAccount(MailAccount mailbox)
		{
			try
			{
				svcUserAdmin users = new svcUserAdmin();
				PrepareProxy(users);

				GenericResult result = users.AddUser(AdminUsername, AdminPassword,
					mailbox.Name,
					mailbox.Password,
					GetDomainName(mailbox.Name),
					mailbox.FirstName,
					mailbox.LastName,
					mailbox.IsDomainAdmin // domain admin is false
					);

                if (!result.Result)
                {
                    if (result.ResultCode == -21)
                        throw new Exception("Password doesn't meet complexity", new Exception(result.Message));

                    throw new Exception(result.Message);
                }

				// set forwarding settings
				result = users.UpdateUserForwardingInfo(AdminUsername, AdminPassword,
					mailbox.Name, mailbox.DeleteOnForward,
					(mailbox.ForwardingAddresses != null ? String.Join(", ", mailbox.ForwardingAddresses) : ""));

				if (!result.Result)
					throw new Exception(result.Message);

                // set forwarding settings - Not tracking errors as this only applies to some versions due to API change (The above wont error)
                result = users.UpdateUserForwardingInfo2(AdminUsername, AdminPassword,
                    mailbox.Name, mailbox.DeleteOnForward,
                    (mailbox.ForwardingAddresses != null ? String.Join(", ", mailbox.ForwardingAddresses) : ""));

                // set additional settings
                result = users.SetRequestedUserSettings(AdminUsername, AdminPassword, mailbox.Name, mailbox.PrepareSetRequestedUserSettingsWebMethodParams());

				if (!result.Result)
					throw new Exception(result.Message);

				// set autoresponder settings
				result = users.UpdateUserAutoResponseInfo(AdminUsername, AdminPassword,
					mailbox.Name,
					mailbox.ResponderEnabled,
					(mailbox.ResponderSubject != null ? mailbox.ResponderSubject : ""),
					(mailbox.ResponderMessage != null ? mailbox.ResponderMessage : ""));

				if (!result.Result)
					throw new Exception(result.Message);

			}
			catch (Exception ex)
			{
				if (AccountExists(mailbox.Name))
				{
					DeleteAccount(mailbox.Name);
				}
				Log.WriteError(ex);
				throw new Exception("Could not create mailbox", ex);
			}
		}

		public void UpdateAccount(MailAccount mailbox)
		{

			try
			{
				//get original account
				MailAccount account = GetAccount(mailbox.Name);

				svcUserAdmin users = new svcUserAdmin();
				PrepareProxy(users);

				string strPassword = mailbox.Password;

				//Don't change password. Get it from mail server.
				if (!mailbox.ChangePassword)
				{
					strPassword = account.Password;
				}

				GenericResult result = users.UpdateUser(
					AdminUsername, AdminPassword, mailbox.Name, strPassword, mailbox.FirstName, mailbox.LastName, mailbox.IsDomainAdmin);

			    if (!result.Result)
			    {
			        if (result.ResultCode == -21)
                        throw new Exception("Password doesn't meet complexity", new Exception(result.Message));
			        
                    throw new Exception(result.Message);
			    }

			    // set forwarding settings
				result = users.UpdateUserForwardingInfo(AdminUsername, AdminPassword,
					mailbox.Name, mailbox.DeleteOnForward,
					(mailbox.ForwardingAddresses != null ? String.Join(", ", mailbox.ForwardingAddresses) : ""));

				if (!result.Result)
					throw new Exception(result.Message);

                // set forwarding settings - Not tracking errors as this only applies to some versions due to API change (The above wont error)
                result = users.UpdateUserForwardingInfo2(AdminUsername, AdminPassword,
                    mailbox.Name, mailbox.DeleteOnForward,
                    (mailbox.ForwardingAddresses != null ? String.Join(", ", mailbox.ForwardingAddresses) : ""));

                // Set additional settings
                result = users.SetRequestedUserSettings(AdminUsername, AdminPassword, mailbox.Name, mailbox.PrepareSetRequestedUserSettingsWebMethodParams());

				if (!result.Result)
					throw new Exception(result.Message);

				// set autoresponder settings
				result = users.UpdateUserAutoResponseInfo(AdminUsername, AdminPassword,
					mailbox.Name,
					mailbox.ResponderEnabled,
					(mailbox.ResponderSubject != null ? mailbox.ResponderSubject : ""),
					(mailbox.ResponderMessage != null ? mailbox.ResponderMessage : ""));

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not update mailbox", ex);
			}

		}

		public void DeleteAccount(string mailboxName)
		{
			try
			{
				svcUserAdmin users = new svcUserAdmin();
				PrepareProxy(users);

				GenericResult result = users.DeleteUser(AdminUsername, AdminPassword,
					mailboxName, GetDomainName(mailboxName));

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not delete mailbox", ex);
			}
		}

		#endregion

		#region Mail Aliases

		public bool MailAliasExists(string mailAliasName)
		{
			try
			{
				svcAliasAdmin aliases = new svcAliasAdmin();
				PrepareProxy(aliases);

				AliasInfoResult result = aliases.GetAlias(AdminUsername, AdminPassword, GetDomainName(mailAliasName), mailAliasName);

				if ((!result.Result || (result.AliasInfo.Name.Equals("Empty")) && (result.AliasInfo.Addresses.Length == 0)))
					return false;

				return true;
			}
			catch (Exception ex)
			{
				throw new Exception("Could not check whether mail alias exists", ex);
			}
		}

		public MailAlias[] GetMailAliases(string domainName)
		{
			try
			{

				svcAliasAdmin aliases = new svcAliasAdmin();
				PrepareProxy(aliases);

				AliasInfoListResult result = aliases.GetAliases(AdminUsername, AdminPassword, domainName);

				if (!result.Result)
					throw new Exception(result.Message);

				List<MailAlias> aliasesList = new List<MailAlias>();


				foreach (AliasInfo alias in result.AliasInfos)
				{
					if (alias.Addresses.Length == 1)
					{
						MailAlias mailAlias = new MailAlias();
						mailAlias.Name = alias.Name + "@" + domainName;
						mailAlias.ForwardTo = alias.Addresses[0];
						aliasesList.Add(mailAlias);
					}
				}
				return aliasesList.ToArray();
			}
			catch (Exception ex)
			{
				throw new Exception("Could not get the list of mail aliases", ex);
			}


		}

		public MailAlias GetMailAlias(string mailAliasName)
		{
			svcAliasAdmin aliases = new svcAliasAdmin();
			PrepareProxy(aliases);


			MailAlias alias = new MailAlias();
			MailAlias newAlias = new MailAlias();

			//convert old alliases created as mailboxes
			if (!MailAliasExists(mailAliasName))
			{
				MailAccount account = GetAccount(mailAliasName);
				newAlias.Name = account.Name;
				if ((account.ForwardingAddresses != null) && (account.ForwardingAddresses.Length > 0))
				{
					newAlias.ForwardTo = account.ForwardingAddresses[0];
				}
				else
				{
					newAlias.ForwardTo = string.Empty;
				}
				DeleteAccount(mailAliasName);
				CreateMailAlias(newAlias);
				return newAlias;
			}

			AliasInfoResult result = aliases.GetAlias(AdminUsername, AdminPassword, GetDomainName(mailAliasName), mailAliasName);
			alias.Name = result.AliasInfo.Name;
			if ((result.AliasInfo.Addresses != null) && (result.AliasInfo.Addresses.Length > 0))
			{
				alias.ForwardTo = result.AliasInfo.Addresses[0];
			}
			else
			{
				alias.ForwardTo = "empty@email.com";
			}
			return alias;
		}

		public void CreateMailAlias(MailAlias mailAlias)
		{
			try
			{
				svcAliasAdmin aliases = new svcAliasAdmin();
				PrepareProxy(aliases);

				GenericResult result = aliases.AddAlias(AdminUsername, AdminPassword,
														GetDomainName(mailAlias.Name), mailAlias.Name,
														new string[] { mailAlias.ForwardTo });


				if (!result.Result)
					throw new Exception(result.Message);
			}

			catch (Exception ex)
			{
				if (MailAliasExists(mailAlias.Name))
				{
					DeleteMailAlias(mailAlias.Name);
				}
				Log.WriteError(ex);
				throw new Exception("Could not create mail alias", ex);

			}

		}

		public void UpdateMailAlias(MailAlias mailAlias)
		{
			try
			{
				svcAliasAdmin aliases = new svcAliasAdmin();
				PrepareProxy(aliases);


				GenericResult result = aliases.UpdateAlias(AdminUsername, AdminPassword, GetDomainName(mailAlias.Name),
															 mailAlias.Name,
															 new string[] { mailAlias.ForwardTo });

				if (!result.Result)
					throw new Exception(result.Message);

			}
			catch (Exception ex)
			{
				throw new Exception("Could not update mailAlias", ex);
			}

		}

		public void DeleteMailAlias(string mailAliasName)
		{
			try
			{
				svcAliasAdmin aliases = new svcAliasAdmin();
				PrepareProxy(aliases);

				GenericResult result = aliases.DeleteAlias(AdminUsername, AdminPassword, GetDomainName(mailAliasName),
					mailAliasName);

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not delete mailAlias", ex);
			}
		}
		#endregion

		#region Groups

		public bool GroupExists(string groupName)
		{
			try
			{
				svcAliasAdmin svcGroups = new svcAliasAdmin();
				PrepareProxy(svcGroups);

				AliasInfoResult result = svcGroups.GetAlias(AdminUsername, AdminPassword,
					GetDomainName(groupName), groupName);

				return (result.Result
					&& result.AliasInfo.Name != "Empty");
			}
			catch (Exception ex)
			{
				throw new Exception("Could not check whether mail domain group exists", ex);
			}
		}

		public MailGroup[] GetGroups(string domainName)
		{
			try
			{
				svcAliasAdmin svcGroups = new svcAliasAdmin();
				PrepareProxy(svcGroups);

				AliasInfoListResult result = svcGroups.GetAliases(AdminUsername, AdminPassword, domainName);

				if (!result.Result)
					throw new Exception(result.Message);


				List<MailGroup> groups = new List<MailGroup>();

				foreach (AliasInfo alias in result.AliasInfos)
				{
					//group - alias with more than one forwarding address
					if (alias.Addresses.Length > 1)
					{
						MailGroup mailGroup = new MailGroup();
						mailGroup.Name = alias.Name + "@" + domainName;
						mailGroup.Members = alias.Addresses;
						groups.Add(mailGroup);
					}
				}

				return groups.ToArray();
			}
			catch (Exception ex)
			{
				throw new Exception("Could not get the list of mail domain groups", ex);
			}
		}

		public MailGroup GetGroup(string groupName)
		{
			try
			{
				svcAliasAdmin svcGroups = new svcAliasAdmin();
				PrepareProxy(svcGroups);

				AliasInfoResult result = svcGroups.GetAlias(AdminUsername, AdminPassword,
					GetDomainName(groupName), groupName);

				if (!result.Result)
					throw new Exception(result.Message);

				MailGroup group = new MailGroup();
				group.Name = groupName;
				group.Members = result.AliasInfo.Addresses;
				group.Enabled = true; // by default
				return group;
			}
			catch (Exception ex)
			{
				throw new Exception("Could not get mail domain group", ex);
			}
		}

		public void CreateGroup(MailGroup group)
		{
			try
			{
				svcAliasAdmin svcGroups = new svcAliasAdmin();
				PrepareProxy(svcGroups);

				GenericResult result = svcGroups.AddAlias(AdminUsername, AdminPassword,
					GetDomainName(group.Name), group.Name, group.Members);

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not create mail domain group", ex);
			}
		}

		public void UpdateGroup(MailGroup group)
		{
			try
			{
				svcAliasAdmin svcGroups = new svcAliasAdmin();
				PrepareProxy(svcGroups);

				GenericResult result = svcGroups.UpdateAlias(AdminUsername, AdminPassword,
					GetDomainName(group.Name), group.Name, group.Members);

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not update mail domain group", ex);
			}
		}

		public void DeleteGroup(string groupName)
		{
			try
			{
				svcAliasAdmin svcGroups = new svcAliasAdmin();
				PrepareProxy(svcGroups);

				GenericResult result = svcGroups.DeleteAlias(AdminUsername, AdminPassword,
					GetDomainName(groupName), groupName);

				if (!result.Result)
					throw new Exception(result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not delete mail domain group", ex);
			}
		}

		#endregion

		#region Lists

		public bool ListExists(string listName)
		{
			bool exists = false;

			try
			{
				string domain = GetDomainName(listName);
				string account = GetAccountName(listName);

				svcMailListAdmin lists = new svcMailListAdmin();
				PrepareProxy(lists);

				MailingListResult result = lists.GetMailingListsByDomain(AdminUsername, AdminPassword, domain);

				if (result.Result)
				{
					foreach (string member in result.listNames)
					{
						if (string.Compare(member, listName, true) == 0)
						{
							exists = true;
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't obtain mail list.", ex);
			}

			return exists;
		}

		public MailList[] GetLists(string domainName)
		{
			try
			{
				svcMailListAdmin svcLists = new svcMailListAdmin();
				PrepareProxy(svcLists);

				MailingListResult mResult = svcLists.GetMailingListsByDomain(
					AdminUsername,
					AdminPassword,
					domainName
				);

				if (!mResult.Result)
					throw new Exception(mResult.Message);

				List<MailList> mailLists = new List<MailList>();
				foreach (string listName in mResult.listNames)
				{
					SettingsRequestResult sResult = svcLists.GetRequestedListSettings(
						AdminUsername,
						AdminPassword,
						domainName,
						listName,
						smListSettings
					);

					if (!sResult.Result)
						throw new Exception(sResult.Message);

					SubscriberListResult rResult = svcLists.GetSubscriberList(
						AdminUsername,
						AdminPassword,
						domainName,
						listName
					);

					if (!rResult.Result)
						throw new Exception(rResult.Message);

					MailList list = new MailList();
					list.Name = string.Concat(listName, "@", domainName);
					SetMailListSettings(list, sResult.settingValues);
					SetMailListMembers(list, rResult.Subscribers);
					mailLists.Add(list);
				}

				return mailLists.ToArray();
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't obtain domain mail lists.", ex);
			}
		}

		public MailList GetList(string listName)
		{
			try
			{
				string domain = GetDomainName(listName);
				string account = GetAccountName(listName);

				svcMailListAdmin svcLists = new svcMailListAdmin();
				PrepareProxy(svcLists);

				SettingsRequestResult sResult = svcLists.GetRequestedListSettings(
					AdminUsername,
					AdminPassword,
					domain,
					account,
					smListSettings
				);

				if (!sResult.Result)
					throw new Exception(sResult.Message);

				SubscriberListResult mResult = svcLists.GetSubscriberList(
					AdminUsername,
					AdminPassword,
					domain,
					account
				);

				if (!mResult.Result)
					throw new Exception(mResult.Message);

				MailList list = new MailList();
				list.Name = listName;

				SetMailListSettings(list, sResult.settingValues);
				SetMailListMembers(list, mResult.Subscribers);

				return list;
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't obtain mail list.", ex);
			}
		}

		protected void SetMailListMembers(MailList list, string[] subscribers)
		{
			List<string> members = new List<string>();

			foreach (string subscriber in subscribers)
				members.Add(subscriber);

			list.Members = members.ToArray();
		}

		private void SetMailListSettings(MailList list, string[] smSettings)
		{
			foreach (string setting in smSettings)
			{
				string[] bunch = setting.Split(new char[] { '=' });

				switch (bunch[0])
				{
					case "description":
						list.Description = bunch[1];
						break;
					case "disabled":
						list.Enabled = !Convert.ToBoolean(bunch[1]);
						break;
					case "moderator":
						list.ModeratorAddress = bunch[1];
						list.Moderated = !string.IsNullOrEmpty(bunch[1]);
						break;
					case "password":
						list.Password = bunch[1];
						break;
					case "requirepassword":
						list.RequirePassword = Convert.ToBoolean(bunch[1]);
						break;
					case "whocanpost":
						if (string.Compare(bunch[1], "anyone", true) == 0)
							list.PostingMode = PostingMode.AnyoneCanPost;
						else if (string.Compare(bunch[1], "moderator", true) == 0)
							list.PostingMode = PostingMode.ModeratorCanPost;
						else
							list.PostingMode = PostingMode.MembersCanPost;
						break;
					case "prependsubject":
						list.EnableSubjectPrefix = Convert.ToBoolean(bunch[1]);
						break;
					case "maxmessagesize":
						list.MaxMessageSize = Convert.ToInt32(bunch[1]);
						break;
					case "maxrecipients":
						list.MaxRecipientsPerMessage = Convert.ToInt32(bunch[1]);
						break;
					case "replytolist":
						list.ReplyToMode = string.Compare(bunch[1], "true", true) == 0 ? ReplyTo.RepliesToList : ReplyTo.RepliesToSender;
						break;
					case "subject":
						list.SubjectPrefix = bunch[1];
						break;
					case "listtoaddress":
						if (string.Compare(bunch[1], "DEFAULT", true) == 0)
							list.ListToAddress = "DEFAULT";
						else if (string.Compare(bunch[1], "LISTADDRESS", true) == 0)
							list.ListToAddress = "LISTADDRESS";
						else if (string.Compare(bunch[1], "SUBSCRIBERADDRESS", true) == 0)
							list.ListToAddress = "SUBSCRIBERADDRESS";
						else
							list.ListToAddress = bunch[1];
						break;
					case "listfromaddress":
						if (string.Compare(bunch[1], "LISTADDRESS", true) == 0)
							list.ListFromAddress = "LISTADDRESS";
						else list.ListFromAddress = string.Compare(bunch[1], "POSTERADDRESS", true) == 0 ? "POSTERADDRESS" : bunch[1];
						break;
					case "listreplytoaddress":
						if (string.Compare(bunch[1], "LISTADDRESS", true) == 0)
							list.ListReplyToAddress = "LISTADDRESS";
						else list.ListReplyToAddress = string.Compare(bunch[1], "POSTERADDRESS", true) == 0 ? "POSTERADDRESS" : bunch[1];
						break;
					case "digestmode":
						list.DigestMode = Convert.ToBoolean(bunch[1]);
						break;
					case "sendsubscribe":
						list.SendSubscribe = Convert.ToBoolean(bunch[1]);
						break;
					case "sendunsubscribe":
						list.SendUnsubscribe = Convert.ToBoolean(bunch[1]);
						break;
					case "allowunsubscribe":
						list.AllowUnsubscribe = Convert.ToBoolean(bunch[1]);
						break;
					case "disablelistcommand":
						list.DisableListcommand = Convert.ToBoolean(bunch[1]);
						break;
					case "disablesubscribecommand":
						list.DisableSubscribecommand = Convert.ToBoolean(bunch[1]);
						break;

				}
			}
		}

		public void CreateList(MailList list)
		{
			try
			{
				string domain = GetDomainName(list.Name);
				string account = GetAccountName(list.Name);

				svcMailListAdmin lists = new svcMailListAdmin();
				PrepareProxy(lists);

				GenericResult result = lists.AddList(AdminUsername, AdminPassword,
					domain,
					account,
					list.ModeratorAddress,
					list.Description
				);

				if (!result.Result)
					throw new Exception(result.Message);

				List<string> settings = new List<string>();
				settings.Add(string.Concat("description=", list.Description));
				settings.Add(string.Concat("disabled=", !list.Enabled));
				settings.Add(string.Concat("moderator=", list.ModeratorAddress));
				settings.Add(string.Concat("password=", list.Password));
				settings.Add(string.Concat("requirepassword=", list.RequirePassword));

				switch (list.PostingMode)
				{
					case PostingMode.AnyoneCanPost:
						settings.Add("whocanpost=anyone");
						break;
					case PostingMode.MembersCanPost:
						settings.Add("whocanpost=subscribersonly");
						break;
					case PostingMode.ModeratorCanPost:
						settings.Add("whocanpost=moderator");
						break;
				}

				settings.Add(string.Concat("prependsubject=", list.EnableSubjectPrefix));
				settings.Add(string.Concat("maxmessagesize=", list.MaxMessageSize));
				settings.Add(string.Concat("maxrecipients=", list.MaxRecipientsPerMessage));
				settings.Add(string.Concat("subject=", list.SubjectPrefix));

				switch (list.ReplyToMode)
				{
					case ReplyTo.RepliesToList:
						settings.Add("replytolist=true");
						break;
				}

				settings.Add(string.Concat("listtoaddress=", list.ListToAddress));
				settings.Add(string.Concat("listfromaddress=", list.ListFromAddress));
				settings.Add(string.Concat("listreplytoaddress=", list.ListReplyToAddress));
				settings.Add(string.Concat("digestmode=", list.DigestMode));
				settings.Add(string.Concat("sendsubscribe=", list.SendSubscribe));
				settings.Add(string.Concat("sendunsubscribe=", list.SendUnsubscribe));
				settings.Add(string.Concat("allowunsubscribe=", list.AllowUnsubscribe));
				settings.Add(string.Concat("disablelistcommand=", list.DisableListcommand));
				settings.Add(string.Concat("disablesubscribecommand=", list.DisableSubscribecommand));

				result = lists.SetRequestedListSettings(AdminUsername, AdminPassword,
					domain,
					list.Name,
					settings.ToArray()
				);

				if (!result.Result)
					throw new Exception(result.Message);

				if (list.Members.Length > 0)
				{
					result = lists.SetSubscriberList(AdminUsername, AdminPassword,
						domain,
						list.Name,
						list.Members
					);

					if (!result.Result)
						throw new Exception(result.Message);
				}
			}
			catch (Exception ex)
			{
				if (ListExists(list.Name))
				{
					DeleteList(list.Name);
				}
				Log.WriteError(ex);
				throw new Exception("Couldn't create mail list.", ex);
			}
		}

		public void UpdateList(MailList list)
		{
			try
			{
				string domain = GetDomainName(list.Name);
				string account = GetAccountName(list.Name);

				svcMailListAdmin lists = new svcMailListAdmin();
				PrepareProxy(lists);

				List<string> settings = new List<string>();
				settings.Add(string.Concat("description=", list.Description));
				settings.Add(string.Concat("disabled=", !list.Enabled));
				settings.Add(string.Concat("moderator=", list.ModeratorAddress));
				settings.Add(string.Concat("password=", list.Password));
				settings.Add(string.Concat("requirepassword=", list.RequirePassword));

				switch (list.PostingMode)
				{
					case PostingMode.AnyoneCanPost:
						settings.Add("whocanpost=anyone");
						break;
					case PostingMode.MembersCanPost:
						settings.Add("whocanpost=subscribersonly");
						break;
					case PostingMode.ModeratorCanPost:
						settings.Add("whocanpost=moderatoronly");
						break;
				}

				settings.Add(string.Concat("prependsubject=", list.EnableSubjectPrefix));
				settings.Add(string.Concat("maxmessagesize=", list.MaxMessageSize));
				settings.Add(string.Concat("maxrecipients=", list.MaxRecipientsPerMessage));
				settings.Add(string.Concat("subject=", list.SubjectPrefix));

				switch (list.ReplyToMode)
				{
					case ReplyTo.RepliesToList:
						settings.Add("replytolist=true");
						break;
					case ReplyTo.RepliesToSender:
						settings.Add("replytolist=false");
						break;
				}

				settings.Add(string.Concat("listtoaddress=", list.ListToAddress));
				settings.Add(string.Concat("listfromaddress=", list.ListFromAddress));
				settings.Add(string.Concat("listreplytoaddress=", list.ListReplyToAddress));
				settings.Add(string.Concat("digestmode=", list.DigestMode));
				settings.Add(string.Concat("sendsubscribe=", list.SendSubscribe));
				settings.Add(string.Concat("sendunsubscribe=", list.SendUnsubscribe));
				settings.Add(string.Concat("allowunsubscribe=", list.AllowUnsubscribe));
				settings.Add(string.Concat("disablelistcommand=", list.DisableListcommand));
				settings.Add(string.Concat("disablesubscribecommand=", list.DisableSubscribecommand));

				Log.WriteWarning(string.Join(" , ", settings.ToArray()));

				GenericResult result = lists.SetRequestedListSettings(AdminUsername, AdminPassword,
																	  domain,
																	  list.Name,
																	  settings.ToArray()
					);

				if (!result.Result)
					throw new Exception(result.Message);

				SubscriberListResult subsribersResult = lists.GetSubscriberList(AdminUsername, AdminPassword, domain, account);

				foreach (string member in subsribersResult.Subscribers)
				{
					GenericResult memberResult = lists.RemoveSubscriber(AdminUsername, AdminPassword, domain, account, member);
					if (!memberResult.Result)
					{
						throw new Exception(memberResult.Message);
					}
				}

				if (list.Members.Length > 0)
				{
					result = lists.SetSubscriberList(AdminUsername, AdminPassword,
													 domain,
													 list.Name,
													 list.Members
						);

					if (!result.Result)
						throw new Exception(result.Message);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't update mail list.", ex);
			}
		}

		/// <summary>
		/// Deletes specified mail list.
		/// </summary>
		/// <param name="listName">Mail list name.</param>
		public void DeleteList(string listName)
		{
			try
			{
				svcMailListAdmin svcLists = new svcMailListAdmin();
				PrepareProxy(svcLists);

				string account = GetAccountName(listName);
				string domain = GetDomainName(listName);

				GenericResult Result = svcLists.DeleteList(
					AdminUsername,
					AdminPassword,
					domain,
					listName
				);

				if (!Result.Result)
					throw new Exception(Result.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't delete a mail list.", ex);
			}
		}

		#endregion

		private void PrepareProxy(SoapHttpClientProtocol proxy)
        {
            string smarterUrl = ServiceUrl;

            if (String.IsNullOrEmpty(smarterUrl))
                smarterUrl = "http://localhost:9998/services/";

            int idx = proxy.Url.LastIndexOf("/");

            // strip the last slash if any
            if (smarterUrl[smarterUrl.Length - 1] == '/')
                smarterUrl = smarterUrl.Substring(0, smarterUrl.Length - 1);
			proxy.Url = smarterUrl + proxy.Url.Substring(idx);
        }

        protected string GetDomainName(string email)
        {
            return email.Substring(email.IndexOf('@') + 1);
        }

        protected string GetAccountName(string email)
        {
            return email.Substring(0, email.IndexOf('@'));
        }

        public override bool IsInstalled()
        {
            string productName = null;
            string productVersion = null;

            RegistryKey HKLM = Registry.LocalMachine;

            RegistryKey key = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            String[] names = null;

            if (key != null)
            {
                names = key.GetSubKeyNames();

                foreach (string s in names)
                {
                    RegistryKey subkey = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + s);
                    if (subkey != null)
                        if (!String.IsNullOrEmpty((string)subkey.GetValue("DisplayName")))
                        {
                            productName = (string)subkey.GetValue("DisplayName");
                        }
                    if (productName != null && productName.Contains("SmarterMail"))
                    {
                        if (subkey != null)
                            productVersion = (string)subkey.GetValue("DisplayVersion");
                        break;
                    }
                }

				if (!String.IsNullOrEmpty(productVersion))
				{
					int version = 0;
					string[] split = productVersion.Split(new[] { '.' });

					if (int.TryParse(split[0], out version))
					{
						if (version >= 10) return true;
					}
					else
						return split[0].Equals("10");
				}
			}

			//checking x64 platform
            key = HKLM.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

            if (key == null)
            {
                return false;
            }

            names = key.GetSubKeyNames();

            foreach (string s in names)
            {
                RegistryKey subkey = HKLM.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" + s);
                if (subkey != null)
                    if (!String.IsNullOrEmpty((string)subkey.GetValue("DisplayName")))
                    {
                        productName = (string)subkey.GetValue("DisplayName");
                    }
                if (productName != null)
                    if (productName.Contains("SmarterMail"))
                    {
                        if (subkey != null) productVersion = (string)subkey.GetValue("DisplayVersion");
                        break;
                    }
            }

            if (!String.IsNullOrEmpty(productVersion))
            {
				int version = 0;
                string[] split = productVersion.Split(new[] { '.' });

				if (int.TryParse(split[0], out version))
				{
					if (version >= 10) return true;
				}
				else
					return split[0].Equals("10");
            }

            return false;
        }
    }	
}
