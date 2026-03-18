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

using FuseCP.EnterpriseServer.Extensions;
using FuseCP.Providers;
using FuseCP.Providers.OS;
using FuseCP.Providers.Common;
using FuseCP.Providers.DNS;
using FuseCP.Providers.DomainLookup;
using FuseCP.Providers.HostedSolution;
using FuseCP.Providers.ResultObjects;
using FuseCP.Providers.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Whois.NET;
using OS = FuseCP.Server.Client;
using FuseCP.Server.Client;
using FuseCP.EnterpriseServer.Data;

namespace FuseCP.EnterpriseServer
{
	/// <summary>
	/// Summary description for ServersController.
	/// </summary>
	public class ServerController: ControllerBase
	{
		private const string LOG_SOURCE_SERVERS = "SERVERS";

		private List<string> _createdDatePatterns = new List<string> { @"Creation Date:(.+)", // base
                                                                                @"created:(.+)",
																				@"Created On:(.+) UTC",
																				@"Created On:(.+)",
																				@"Domain Registration Date:(.+)",
																				@"Domain Create Date:(.+)",
																				@"Registered on:(.+)"};

		private List<string> _expiredDatePatterns = new List<string> {   @"Expiration Date:(.+) UTC", //base UTC
                                                                                @"Expiration Date:(.+)", // base
                                                                                @"Registry Expiry Date:(.+)", //.org
                                                                                @"paid-till:(.+)", //.ru
                                                                                @"Expires On:(.+)", //.name
                                                                                @"Domain Expiration Date:(.+)", //.us
                                                                                @"renewal date:(.+)", //.pl
                                                                                @"Expiry date:(.+)", //.uk
                                                                                @"anniversary:(.+)", //.fr
                                                                                @"expires:(.+)" //.fi 
                                                                              };

		private List<string> _registrarNamePatterns = new List<string>   {
																				@"Created by Registrar:(.+)",
																				@"Registrar:(.+)",
																				@"Registrant Name:(.+)"
																			};

		private List<string> _datePatterns = new List<string> {   @"ddd MMM dd HH:mm:ss G\MT yyyy",
																								 @"yyyymmdd"
																										};
		public ServerController(ControllerBase provider) : base(provider) { }

		#region Servers
		public List<ServerInfo> GetAllServers()
		{
			// fill collection
			var servers = ObjectUtils.CreateListFromDataSet<ServerInfo>(
				Database.GetAllServers(SecurityContext.User.UserId));
			
			foreach (var server in servers) DecryptServerUrl(server);

			return servers;
		}

		public DataSet GetRawAllServers()
		{
			return Database.GetAllServers(SecurityContext.User.UserId);
		}

		public List<ServerInfo> GetServers()
		{
			// create servers list
			List<ServerInfo> servers = new List<ServerInfo>();

			// fill collection
			ObjectUtils.FillCollectionFromDataSet<ServerInfo>(
				servers, Database.GetServers(SecurityContext.User.UserId));

			foreach (var server in servers) DecryptServerUrl(server);

			return servers;
		}

		public DataSet GetRawServers()
		{
			return Database.GetServers(SecurityContext.User.UserId);
		}

		internal ServerInfo GetServerByIdInternal(int serverId)
		{
			ServerInfo server = ObjectUtils.FillObjectFromDataReader<ServerInfo>(
				Database.GetServerInternal(serverId));

			if (server == null)
				return null;

			// decrypt passwords
			server.Password = CryptoUtils.Decrypt(server.Password);
			server.ADPassword = CryptoUtils.Decrypt(server.ADPassword);

			DecryptServerUrl(server);

			return server;
		}

		public void DecryptServerUrl(ServerInfo server)
		{
			if (server != null)
			{
				server.ServerUrl = CryptoUtils.DecryptServerUrl(server.ServerUrl);
			}
		}

		public void EncryptServerUrl(ServerInfo server)
		{
			server.ServerUrl = CryptoUtils.EncryptServerUrl(server.ServerUrl);
		}

		public ServerInfo GetServerShortDetails(int serverId)
		{
			var server = ObjectUtils.FillObjectFromDataReader<ServerInfo>(
				Database.GetServerShortDetails(serverId));
			DecryptServerUrl(server);
			return server;
		}

		public ServerInfo GetServerById(int serverId, bool forAutodiscover = false)
		{
			var server = ObjectUtils.FillObjectFromDataReader<ServerInfo>(
				Database.GetServer(SecurityContext.User.UserId, serverId, forAutodiscover));
			DecryptServerUrl(server);
			return server;
		}

		public ServerInfo GetServerByName(string serverName)
		{
			var server = ObjectUtils.FillObjectFromDataReader<ServerInfo>(
				Database.GetServerByName(SecurityContext.User.UserId, serverName));
			DecryptServerUrl(server);
			return server;
		}

		public int CheckServerAvailable(string serverUrl, string password)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive
				| DemandAccount.IsAdmin);
			if (accountCheck < 0) return accountCheck;

			TaskManager.StartTask("SERVER", "CHECK_AVAILABILITY", serverUrl);

			try
			{
				var test = new Server.Client.Test();
				test.Url = CryptoUtils.DecryptServerUrl(serverUrl);
				test.Touch();
				return 0;
			}
			catch (WebException ex)
			{
				HttpWebResponse response = (HttpWebResponse)ex.Response;
				if (response != null && response.StatusCode == HttpStatusCode.NotFound)
					return BusinessErrorCodes.ERROR_ADD_SERVER_NOT_FOUND;
				else if (response != null && response.StatusCode == HttpStatusCode.BadRequest)
					return BusinessErrorCodes.ERROR_ADD_SERVER_BAD_REQUEST;
				else if (response != null && response.StatusCode == HttpStatusCode.InternalServerError)
					return BusinessErrorCodes.ERROR_ADD_SERVER_INTERNAL_SERVER_ERROR;
				else if (response != null && response.StatusCode == HttpStatusCode.ServiceUnavailable)
					return BusinessErrorCodes.ERROR_ADD_SERVER_SERVICE_UNAVAILABLE;
				else if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
					return BusinessErrorCodes.ERROR_ADD_SERVER_UNAUTHORIZED;
				if (ex.Message.Contains("The remote name could not be resolved") || ex.Message.Contains("Unable to connect"))
				{
					TaskManager.WriteError("The remote server could not ne resolved");
					return BusinessErrorCodes.ERROR_ADD_SERVER_URL_UNAVAILABLE;
				}
				return BusinessErrorCodes.ERROR_ADD_SERVER_APPLICATION_ERROR;
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("The signature or decryption was invalid"))
				{
					TaskManager.WriteWarning("Wrong server access credentials");
					return BusinessErrorCodes.ERROR_ADD_SERVER_WRONG_PASSWORD;
				}
				else
				{
					TaskManager.WriteError("General Server Error");
					TaskManager.WriteError(ex);
					//return BusinessErrorCodes.ERROR_ADD_SERVER_APPLICATION_ERROR;

					throw;
				}
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public bool? GetServerPasswordIsSHA256(string serverUrl)
		{
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive
                 | DemandAccount.IsAdmin);
            if (accountCheck < 0) return null;

            TaskManager.StartTask("SERVER", "GET_SERVER_PLATFORM", serverUrl);

            try
            {
                var autoDiscovery = new AutoDiscovery();
                ServiceProviderProxy.ServerInit(autoDiscovery, serverUrl, "", false);
                return autoDiscovery.GetServerPasswordIsSHA256();
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    return null;
                else if (response != null && response.StatusCode == HttpStatusCode.BadRequest)
                    return null;
                else if (response != null && response.StatusCode == HttpStatusCode.InternalServerError)
                    return null;
                else if (response != null && response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    return null;
                else if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    return null;
                if (ex.Message.Contains("The remote name could not be resolved") || ex.Message.Contains("Unable to connect"))
                {
                    TaskManager.WriteError("The remote server could not be resolved");
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The signature or decryption was invalid"))
                {
                    TaskManager.WriteWarning("Wrong server access credentials");
                    return null;
                }
                else
                {
                    TaskManager.WriteError("General Server Error");
                    TaskManager.WriteError(ex);
                    return null;
                }
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }

        public void GetServerPlatform(string serverUrl, out OSPlatform platform, out bool? isCore)
		{
			platform = OSPlatform.Unknown;
			isCore = null;

			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive
				 | DemandAccount.IsAdmin);
			if (accountCheck < 0) return;

			TaskManager.StartTask("SERVER", "GET_SERVER_PLATFORM", serverUrl);

			try
			{
				var discovery = new Server.Client.AutoDiscovery();
                discovery.Url = serverUrl;
				var p = discovery.GetOSPlatform();
				platform = p.OSPlatform;
				isCore = p.IsCore;
			}
			catch (WebException ex)
			{
				HttpWebResponse response = (HttpWebResponse)ex.Response;
				if (response != null && response.StatusCode == HttpStatusCode.NotFound)
					return;
				else if (response != null && response.StatusCode == HttpStatusCode.BadRequest)
					return;
				else if (response != null && response.StatusCode == HttpStatusCode.InternalServerError)
					return;
				else if (response != null && response.StatusCode == HttpStatusCode.ServiceUnavailable)
					return;
				else if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
					return;
				if (ex.Message.Contains("The remote name could not be resolved") || ex.Message.Contains("Unable to connect"))
				{
					TaskManager.WriteError("The remote server could not be resolved");
					return;
				}
				return;
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("The signature or decryption was invalid"))
				{
					TaskManager.WriteWarning("Wrong server access credentials");
					return;
				}
				else
				{
					TaskManager.WriteError("General Server Error");
					TaskManager.WriteError(ex);
					return;
				}
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		private void DiscoverServices(ServerInfo server)
		{
			try
			{
				List<ProviderInfo> providers;
				try
				{
					providers = GetProviders();
				}
				catch (Exception ex)
				{
					TaskManager.WriteError(ex);
					throw new ApplicationException("Could not get providers list.");
				}

				var installedProviderIds = Database.GetProviderIdsByServerId(server.ServerId)
					.ToHashSet();
				var discovery = providers
					.Where(provider => !provider.DisableAutoDiscovery && !installedProviderIds.Contains(provider.ProviderId));

				foreach (var provider in discovery)
				{
					BoolResult isInstalled = IsInstalled(server, provider);
					if (isInstalled.IsSuccess)
					{
						if (isInstalled.Value)
						{
							try
							{
								ServiceInfo service = new ServiceInfo();
								service.ServerId = server.ServerId;
								service.ProviderId = provider.ProviderId;
								service.ServiceName = provider.DisplayName;
								using (var clone = AsAsync<ServerController>()) clone.AddService(service);
							}
							catch (Exception ex)
							{
								TaskManager.WriteError(ex);
								throw;
							}
						}
					}
					else
					{
						string errors = string.Join("\n", isInstalled.ErrorCodes.ToArray());
						string str =
							string.Format(
								"Could not check if specific software intalled for {0}. Following errors have been occured:\n{1}",
								provider.ProviderName, errors);

						TaskManager.WriteError(str);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Could not find services. General error has occurred.", ex);
			}
		}

		public int DiscoverAndAddServices(int serverId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive
				| DemandAccount.IsAdmin);
			if (accountCheck < 0) return accountCheck;

			var server = GetServerById(serverId, true);

			// init passwords
			if (server.Password == null)
				server.Password = "";
			if (server.ADPassword == null)
				server.ADPassword = "";

			TaskManager.StartTask("SERVER", "DISCOVER_AND_ADD_SERVICES", server.ServerName);

			try
			{
				DiscoverServices(server);
			}
			catch (Exception ex)
			{
				TaskManager.WriteError(ex);
			}
		
			TaskManager.ItemId = server.ServerId;

			TaskManager.CompleteTask();

			return server.ServerId;
		}

		public int AddServer(ServerInfo server, bool autoDiscovery)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive
				| DemandAccount.IsAdmin);
			if (accountCheck < 0) return accountCheck;

			// init passwords
			if (server.Password == null)
				server.Password = "";
			if (server.ADPassword == null)
				server.ADPassword = "";

			// check server availability
			if (!server.VirtualServer)
			{
				int availResult = CheckServerAvailable(server.ServerUrl, server.Password);
				if (availResult < 0)
					return availResult;

				OSPlatform osPlatform;
				bool? isCore;
				GetServerPlatform(server.ServerUrl, out osPlatform, out isCore);
				server.OSPlatform = osPlatform;
				server.IsCore = isCore;

				bool passwordIsSHA256 = GetServerPasswordIsSHA256(server.ServerUrl) ?? true;
				server.PasswordIsSHA256 = passwordIsSHA256;
			}

			TaskManager.StartTask("SERVER", "ADD", server.ServerName);

			var serverUrl = CryptoUtils.EncryptServerUrl(server.ServerUrl);

			int serverId = Database.AddServer(server.ServerName, serverUrl,
				CryptoUtils.EncryptServerPassword(server.Password), server.Comments, server.VirtualServer, server.InstantDomainAlias,
				server.PrimaryGroupId, server.ADEnabled, server.ADRootDomain, server.ADUsername, CryptoUtils.Encrypt(server.ADPassword),
				server.ADAuthenticationType, server.OSPlatform, server.IsCore, server.PasswordIsSHA256);

			if (autoDiscovery)
			{
				server.ServerId = serverId;
				try
				{
					DiscoverServices(server);
				}
				catch (Exception ex)
				{
					TaskManager.WriteError(ex);
				}
			}

			TaskManager.ItemId = serverId;

			TaskManager.CompleteTask();

			return serverId;
		}

		public int UpdateServer(ServerInfo server)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive
				| DemandAccount.IsAdmin);
			if (accountCheck < 0) return accountCheck;

			// get original server
			ServerInfo origServer = GetServerByIdInternal(server.ServerId);

			TaskManager.StartTask("SERVER", "UPDATE", origServer.ServerName, server.ServerId);

			// preserve passwords
			server.Password = origServer.Password;
			server.ADPassword = origServer.ADPassword;

			// check server availability
			if (!origServer.VirtualServer)
			{
				int availResult = CheckServerAvailable(server.ServerUrl, server.Password);
				if (availResult < 0)
					return availResult;

				OSPlatform osPlatform = OSPlatform.Unknown;
				bool? isCore = null;
				GetServerPlatform(server.ServerUrl, out osPlatform, out isCore);
				server.OSPlatform = osPlatform;
				server.IsCore = isCore;

				bool passwordIsSHA256 = GetServerPasswordIsSHA256(server.ServerUrl) ?? true;
				server.PasswordIsSHA256 = passwordIsSHA256;
			}

			var serverUrl = CryptoUtils.EncryptServerUrl(server.ServerUrl);

			Database.UpdateServer(server.ServerId, server.ServerName, serverUrl,
				CryptoUtils.EncryptServerPassword(server.Password), server.Comments, server.InstantDomainAlias,
				server.PrimaryGroupId, server.ADEnabled, server.ADRootDomain, server.ADUsername, CryptoUtils.Encrypt(server.ADPassword),
				server.ADAuthenticationType, server.ADParentDomain, server.ADParentDomainController,
				server.OSPlatform, server.IsCore, server.PasswordIsSHA256);

			TaskManager.CompleteTask();

			return 0;
		}

		public int UpdateServerConnectionPassword(int serverId, string password)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive
				| DemandAccount.IsAdmin);
			if (accountCheck < 0) return accountCheck;

			// get original server
			ServerInfo server = GetServerByIdInternal(serverId);

			TaskManager.StartTask("SERVER", "UPDATE_PASSWORD", server.ServerName, serverId);

			// set password
			server.Password = password;
			var passwordIsSHA256 = GetServerPasswordIsSHA256(server.ServerUrl);
			if (passwordIsSHA256 != null)
			{
				server.PasswordIsSHA256 = (bool)passwordIsSHA256;
			}

			var serverUrl = CryptoUtils.EncryptServerUrl(server.ServerUrl);

			// update server
			Database.UpdateServer(server.ServerId, server.ServerName, serverUrl,
				CryptoUtils.EncryptServerPassword(server.Password), server.Comments, server.InstantDomainAlias,
				server.PrimaryGroupId, server.ADEnabled, server.ADRootDomain, server.ADUsername, CryptoUtils.Encrypt(server.ADPassword),
				server.ADAuthenticationType, server.ADParentDomain, server.ADParentDomainController,
				server.OSPlatform, server.IsCore, server.PasswordIsSHA256);

			TaskManager.CompleteTask();

			return 0;
		}

		public int UpdateServerADPassword(int serverId, string adPassword)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive
				| DemandAccount.IsAdmin);
			if (accountCheck < 0) return accountCheck;

			// get original server
			ServerInfo server = GetServerByIdInternal(serverId);

			TaskManager.StartTask("SERVER", "UPDATE_AD_PASSWORD", server.ServerName, serverId);

			// set password
			server.ADPassword = adPassword;

			var serverUrl = CryptoUtils.EncryptServerUrl(server.ServerUrl);

			// update server
			Database.UpdateServer(server.ServerId, server.ServerName, serverUrl,
				CryptoUtils.EncryptServerPassword(server.Password), server.Comments, server.InstantDomainAlias,
				server.PrimaryGroupId, server.ADEnabled, server.ADRootDomain, server.ADUsername, CryptoUtils.Encrypt(server.ADPassword),
				server.ADAuthenticationType, server.ADParentDomain, server.ADParentDomainController,
				server.OSPlatform, server.IsCore, server.PasswordIsSHA256);

			TaskManager.CompleteTask();

			return 0;
		}

		public int DeleteServer(int serverId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive
				| DemandAccount.IsAdmin);
			if (accountCheck < 0) return accountCheck;

			// get original server
			ServerInfo server = GetServerByIdInternal(serverId);

			TaskManager.StartTask("SERVER", "DELETE", server.ServerName, serverId);

			try
			{
				int result = Database.DeleteServer(serverId);
				if (result == -1)
				{
					TaskManager.WriteError("Server contains services");
					return BusinessErrorCodes.ERROR_SERVER_CONTAINS_SERVICES;
				}
				else if (result == -2)
				{
					TaskManager.WriteError("Server contains spaces");
					return BusinessErrorCodes.ERROR_SERVER_CONTAINS_PACKAGES;
				}
				else if (result == -3)
				{
					TaskManager.WriteError("Server is used as a target in several hosting plans");
					return BusinessErrorCodes.ERROR_SERVER_USED_IN_HOSTING_PLANS;
				}

				return 0;
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public Dictionary<int, string> AutoUpdateServer(int serverId, int serviceId, byte[] zipFile, string zipFileName)
		{
			OS.OperatingSystem os = new OS.OperatingSystem();
			ServiceProviderProxy.Init(os, serviceId);
			Dictionary<int, string> res = new Dictionary<int, string>();
			string downloadPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"FuseCP", "Downloads");
			string unpackedZipDirectory = downloadPath + zipFileName.Replace(".zip", "");
			string ipAddress = os.Url.Split('/')[2].Split(':')[0];

			string targetPath = @"\\" + ipAddress + @"\" + ServerController.GetServerFilePath(serverId).Replace(":", "$").Replace(@"\\", @"\");

			if (Directory.Exists(targetPath))
			{
				if (!Directory.Exists(downloadPath))
				{
					Directory.CreateDirectory(downloadPath);
				}
				FileStream stream = File.Create(downloadPath + zipFileName);
				stream.Close();
				using (System.IO.MemoryStream zipFileContent = new System.IO.MemoryStream(zipFile))
				{
					int chunkSize = 4096;
					using (System.IO.BinaryReader reader = new System.IO.BinaryReader(zipFileContent))
					{
						byte[] chunk = new byte[chunkSize];
						int c;
						while ((c = reader.Read(chunk, 0, chunk.Length)) != 0)
						{
							FileStream s = new FileStream(downloadPath + zipFileName, FileMode.Append, FileAccess.Write);
							s.Write(chunk, 0, chunk.Length);
							s.Close();
						}
					}
				}

				FileUtils.UnzipFiles(downloadPath + zipFileName, unpackedZipDirectory);
				FileUtils.CopyDirectoryContentUNC(unpackedZipDirectory, targetPath);
				res.Add(0, "");
			}
			else
			{
				res.Add(serverId, "Directory " + targetPath + " not found");
			}
			return res;
		}

		#endregion

		#region Virtual Servers
		public DataSet GetVirtualServers()
		{
			return Database.GetVirtualServers(SecurityContext.User.UserId);
		}

		public DataSet GetAvailableVirtualServices(int serverId)
		{
			return Database.GetAvailableVirtualServices(SecurityContext.User.UserId, serverId);
		}

		public DataSet GetVirtualServices(int serverId, bool forAutodiscover)
		{
			return Database.GetVirtualServices(SecurityContext.User.UserId, serverId, forAutodiscover);
		}

		public int AddVirtualServices(int serverId, int[] ids)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			ServerInfo server = GetServerByIdInternal(serverId);

			TaskManager.StartTask("VIRTUAL_SERVER", "ADD_SERVICES", server.ServerName, serverId);

			// build XML
			string xml = BuildXmlFromArray(ids, "services", "service");

			// update server
			Database.AddVirtualServices(serverId, xml);

			TaskManager.CompleteTask();

			return 0;
		}

		public int DeleteVirtualServices(int serverId, int[] ids)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			ServerInfo server = GetServerByIdInternal(serverId);

			TaskManager.StartTask("VIRTUAL_SERVER", "DELETE_SERVICES", server.ServerName, serverId);

			// build XML
			string xml = BuildXmlFromArray(ids, "services", "service");

			// update server
			Database.DeleteVirtualServices(serverId, xml);

			TaskManager.CompleteTask();

			return 0;
		}

		public int UpdateVirtualGroups(int serverId, VirtualGroupInfo[] groups)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			/*
            XML Format:

            <groups>
	            <group id="16" distributionType="1" bindDistributionToPrimary="1"/>
            </groups>

            */

			// build XML
			XmlDocument doc = new XmlDocument();
			XmlElement nodeGroups = doc.CreateElement("groups");
			// groups
			if (groups != null)
			{
				foreach (VirtualGroupInfo group in groups)
				{
					XmlElement nodeGroup = doc.CreateElement("group");
					nodeGroups.AppendChild(nodeGroup);
					nodeGroup.SetAttribute("id", group.GroupId.ToString());
					nodeGroup.SetAttribute("distributionType", group.DistributionType.ToString());
					nodeGroup.SetAttribute("bindDistributionToPrimary", group.BindDistributionToPrimary ? "1" : "0");
				}
			}

			string xml = nodeGroups.OuterXml;

			// update server
			Database.UpdateVirtualGroups(serverId, xml);

			return 0;
		}

		private string BuildXmlFromArray(int[] ids, string rootName, string childName)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement nodeRoot = doc.CreateElement(rootName);
			foreach (int id in ids)
			{
				XmlElement nodeChild = doc.CreateElement(childName);
				nodeChild.SetAttribute("id", id.ToString());
				nodeRoot.AppendChild(nodeChild);
			}

			return nodeRoot.OuterXml;
		}
		#endregion

		#region Services
		public DataSet GetRawServicesByServerId(int serverId)
		{
			return Database.GetRawServicesByServerId(SecurityContext.User.UserId, serverId);
		}
		public String GetMailFilterURL(int PackageId, String ResorceGroupName)
		{
			return Database.GetMailFilterURL(SecurityContext.User.UserId, PackageId, ResorceGroupName);
		}
		public String GetMailFilterURLByHostingPlan(int PlanID, String ResorceGroupName)
		{
			return Database.GetMailFilterUrlByHostingPlan(SecurityContext.User.UserId, PlanID, ResorceGroupName);
		}
		public List<ServiceInfo> GetServicesByServerId(int serverId)
		{
			List<ServiceInfo> services = new List<ServiceInfo>();
			ObjectUtils.FillCollectionFromDataReader<ServiceInfo>(services,
				Database.GetServicesByServerId(SecurityContext.User.UserId, serverId));
			return services;
		}

		public List<ServiceInfo> GetServicesByServerIdGroupName(int serverId, string groupName)
		{
			List<ServiceInfo> services = new List<ServiceInfo>();
			ObjectUtils.FillCollectionFromDataReader<ServiceInfo>(services,
				Database.GetServicesByServerIdGroupName(SecurityContext.User.UserId,
				serverId, groupName));
			return services;
		}

		public DataSet GetRawServicesByGroupId(int groupId)
		{
			return Database.GetServicesByGroupId(SecurityContext.User.UserId, groupId);
		}

		public DataSet GetRawServicesByGroupName(string groupName, bool forAutodiscover)
		{
			return Database.GetServicesByGroupName(SecurityContext.User.UserId, groupName, forAutodiscover);
		}

		public List<ServiceInfo> GetServicesByGroupName(string groupName)
		{
			return ObjectUtils.CreateListFromDataSet<ServiceInfo>(
				Database.GetServicesByGroupName(SecurityContext.User.UserId, groupName, false));
		}

		public ServiceInfo GetServiceInfoAdmin(int serviceId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.IsAdmin
				| DemandAccount.IsActive);
			if (accountCheck < 0)
				return null;

			return ObjectUtils.FillObjectFromDataReader<ServiceInfo>(
				Database.GetService(SecurityContext.User.UserId, serviceId));
		}

		public ServiceInfo GetServiceInfo(int serviceId)
		{
			return ObjectUtils.FillObjectFromDataReader<ServiceInfo>(
				Database.GetService(SecurityContext.User.UserId, serviceId));
		}

		public int AddService(ServiceInfo service)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			TaskManager.StartTask("SERVER", "ADD_SERVICE", GetServerByIdInternal(service.ServerId).ServerName, service.ServerId);

			TaskManager.WriteParameter("Service name", service.ServiceName);
			TaskManager.WriteParameter("Provider", service.ProviderId);

			int serviceId = Database.AddService(service.ServerId, service.ProviderId, service.ServiceName,
				service.ServiceQuotaValue, service.ClusterId, service.Comments);

			// read service default settings
			try
			{
				// load original settings
				StringDictionary origSettings = GetServiceSettingsAdmin(serviceId);

				// load provider settings
				ServiceProvider svc = new ServiceProvider();
				ServiceProviderProxy.Init(svc, serviceId);

				SettingPair[] settings = svc.GetProviderDefaultSettings();

				if (settings != null && settings.Length > 0)
				{
					// merge settings
					foreach (SettingPair pair in settings)
						origSettings[pair.Name] = pair.Value;

					// update settings in the meta base
					string[] bareSettings = new string[origSettings.Count];
					int i = 0;
					foreach (string key in origSettings.Keys)
						bareSettings[i++] = key + "=" + origSettings[key];

					UpdateServiceSettings(serviceId, bareSettings);
				}
			}
			catch (Exception ex)
			{
				TaskManager.WriteError(ex, "Error reading default provider settings");
			}

			TaskManager.CompleteTask();

			return serviceId;
		}

		public int UpdateService(ServiceInfo service)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			// load original service
			ServiceInfo origService = GetServiceInfo(service.ServiceId);

			TaskManager.StartTask("SERVER", "UPDATE_SERVICE", GetServerByIdInternal(origService.ServerId).ServerName, origService.ServerId);

			if (!origService.ServiceName.Equals(service.ServiceName))
				TaskManager.WriteParameter("New service name", service.ServiceName);

			//TODO: Add the ability to transfer to another node (ServerID, update procedure) 
			if (service.ProviderId > 0) //if we have a value, then updateServiceFully
			{
				if (origService.ProviderId != service.ProviderId)
					TaskManager.WriteParameter("New Provider Id", service.ProviderId.ToString());
				Database.UpdateServiceFully(service.ServiceId, service.ProviderId, service.ServiceName,
				service.ServiceQuotaValue, service.ClusterId, service.Comments);
				TaskManager.Write("Updated Service Fully");
			}
			else
			{
				Database.UpdateService(service.ServiceId, service.ServiceName,
				service.ServiceQuotaValue, service.ClusterId, service.Comments);
				TaskManager.Write("Updated Service");
			}

			TaskManager.CompleteTask();

			return 0;
		}

		public int DeleteService(int serviceId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			ServiceInfo service = GetServiceInfoAdmin(serviceId);

			TaskManager.StartTask("SERVER", "DELETE_SERVICE", GetServerByIdInternal(service.ServerId).ServerName, service.ServerId);

			TaskManager.WriteParameter("Service name", service.ServiceName);

			try
			{
				int result = Database.DeleteService(serviceId);
				if (result == -1)
				{
					TaskManager.WriteError("Service contains service items");
					return BusinessErrorCodes.ERROR_SERVICE_CONTAINS_SERVICE_ITEMS;
				}
				else if (result == -2)
				{
					TaskManager.WriteError("Service is assigned to virtual server");
					return BusinessErrorCodes.ERROR_SERVICE_USED_IN_VIRTUAL_SERVER;
				}

				return 0;

			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public StringDictionary GetServiceSettings(int serviceId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.IsActive);
			if (accountCheck < 0)
				return null;

			bool isDemoAccount = (SecurityContext.CheckAccount(DemandAccount.NotDemo) < 0);

			return GetServiceSettings(serviceId, !isDemoAccount);
		}

		public StringDictionary GetServiceSettingsAdmin(int serviceId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.IsAdmin | DemandAccount.IsActive);
			if (accountCheck < 0)
				return null;

			bool isDemoAccount = (SecurityContext.CheckAccount(DemandAccount.NotDemo) < 0);

			return GetServiceSettings(serviceId, !isDemoAccount);
		}

		internal StringDictionary GetServiceSettings(int serviceId, bool decryptPassword)
		{
			// get service settings
			IDataReader reader = Database.GetServiceProperties(SecurityContext.User.UserId, serviceId);

			// create settings object
			StringDictionary settings = new StringDictionary();
			while (reader.Read())
			{
				string name = (string)reader["PropertyName"];
				string val = (string)reader["PropertyValue"];

				if (name.ToLower().IndexOf("password") != -1 && decryptPassword)
					val = CryptoUtils.Decrypt(val);

				settings.Add(name, val);
			}
			reader.Close();

			return settings;
		}

		public int UpdateServiceSettings(int serviceId, string[] settings)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			if (settings != null)
			{
				// build xml
				XmlDocument doc = new XmlDocument();
				XmlElement nodeProps = doc.CreateElement("properties");
				foreach (string setting in settings)
				{
					int idx = setting.IndexOf('=');
					string name = setting.Substring(0, idx);
					string val = setting.Substring(idx + 1);

					if (name.ToLower().IndexOf("password") != -1)
						val = CryptoUtils.Encrypt(val);

					XmlElement nodeProp = doc.CreateElement("property");
					nodeProp.SetAttribute("name", name);
					nodeProp.SetAttribute("value", val);
					nodeProps.AppendChild(nodeProp);
				}

				string xml = nodeProps.OuterXml;

				// update settings
				Database.UpdateServiceProperties(serviceId, xml);
			}

			return 0;
		}

		public string[] InstallService(int serviceId)
		{
			ServiceProvider prov = new ServiceProvider();
			ServiceProviderProxy.Init(prov, serviceId);
			return prov.Install();
		}

		public QuotaInfo GetProviderServiceQuota(int providerId)
		{
			return ObjectUtils.FillObjectFromDataReader<QuotaInfo>(
				Database.GetProviderServiceQuota(providerId));
		}

		public StringDictionary GetMailServiceSettingsByPackage(int packageID)
		{
			int serviceID = PackageController.GetPackageServiceId(packageID, ResourceGroups.Mail);

			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.IsActive);
			if (accountCheck < 0)
				return null;

			bool isDemoAccount = (SecurityContext.CheckAccount(DemandAccount.NotDemo) < 0);

			return GetServiceSettings(serviceID, !isDemoAccount);
		}
		#endregion

		#region Providers

		public List<ProviderInfo> GetInstalledProviders(int groupId)
		{
			List<ProviderInfo> provs = new List<ProviderInfo>();
			ObjectUtils.FillCollectionFromDataSet<ProviderInfo>(provs,
				Database.GetGroupProviders(groupId));
			return provs;
		}

		public List<ResourceGroupInfo> GetResourceGroups()
		{
			List<ResourceGroupInfo> groups = new List<ResourceGroupInfo>();
			ObjectUtils.FillCollectionFromDataSet<ResourceGroupInfo>(groups,
				Database.GetResourceGroups());
			return groups;
		}

		public ResourceGroupInfo GetResourceGroup(int groupId)
		{
			return ObjectUtils.FillObjectFromDataReader<ResourceGroupInfo>(
				Database.GetResourceGroup(groupId));
		}

		public ResourceGroupInfo GetResourceGroupByName(string name)
		{
			return ObjectUtils.FillObjectFromDataReader<ResourceGroupInfo>(
				Database.GetResourceGroupByName(name));
		}

		public ProviderInfo GetProvider(int providerId)
		{
			return ObjectUtils.FillObjectFromDataReader<ProviderInfo>(
				Database.GetProvider(providerId));
		}

		public List<ProviderInfo> GetProviders()
		{
			List<ProviderInfo> provs = new List<ProviderInfo>();
			ObjectUtils.FillCollectionFromDataSet<ProviderInfo>(
				provs, Database.GetProviders());
			return provs;
		}

		public List<ProviderInfo> GetProvidersByGroupID(int groupId)
		{
			List<ProviderInfo> provs = new List<ProviderInfo>();
			ObjectUtils.FillCollectionFromDataSet<ProviderInfo>(
				provs, Database.GetGroupProviders(groupId));
			return provs;
		}

		public String GetMailFilterUrl(int packageId, string groupName)
		{
			// load service
			String l_stURL = Database.GetMailFilterURL(SecurityContext.User.UserId, packageId, groupName);

			if (String.IsNullOrEmpty(l_stURL))
				return string.Empty;

			return l_stURL;
		}

		public String GetMailFilterUrlByHostingPlan(int PlanId, string groupName)
		{
			// load service
			String l_stURL = Database.GetMailFilterUrlByHostingPlan(SecurityContext.User.UserId, PlanId, groupName);

			if (String.IsNullOrEmpty(l_stURL))
				return string.Empty;

			return l_stURL;
		}
		public ProviderInfo GetPackageServiceProvider(int packageId, string groupName)
		{
			// load service
			int serviceId = PackageController.GetPackageServiceId(packageId, groupName);

			if (serviceId == 0)
				return null;

			ServiceInfo service = GetServiceInfo(serviceId);
			return GetProvider(service.ProviderId);
		}
		public BoolResult IsInstalled(int serverId, int providerId)
		{
			BoolResult res = TaskManager.StartResultTask<BoolResult>("AUTO_DISCOVERY", "IS_INSTALLED");

			try
			{
				ProviderInfo provider = GetProvider(providerId);
				if (provider == null)
				{
					TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_PROVIDER_INFO);
					return res;
				}

				AutoDiscovery ad = new AutoDiscovery();
				ServiceProviderProxy.ServerInit(ad, serverId);

				res = ad.IsInstalled(provider.ProviderType);
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_CHECK_IF_PROVIDER_SOFTWARE_INSTALLED, ex);

			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public BoolResult IsInstalled(int serverId, ProviderInfo provider)
		{
			BoolResult res = TaskManager.StartResultTask<BoolResult>("AUTO_DISCOVERY", "IS_INSTALLED");

			try
			{
				AutoDiscovery ad = new AutoDiscovery();
				ServiceProviderProxy.ServerInit(ad, serverId);

				res = ad.IsInstalled(provider.ProviderType);
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_CHECK_IF_PROVIDER_SOFTWARE_INSTALLED, ex);

			}

			TaskManager.CompleteResultTask();
			return res;

		}
		public BoolResult IsInstalled(ServerInfo server, ProviderInfo provider)
		{
			BoolResult res = TaskManager.StartResultTask<BoolResult>("AUTO_DISCOVERY", "IS_INSTALLED");

			try
			{
				AutoDiscovery ad = new AutoDiscovery();
				ServiceProviderProxy.ServerInit(ad, server.ServerUrl, server.Password, false);

				res = ad.IsInstalled(provider.ProviderType);
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_CHECK_IF_PROVIDER_SOFTWARE_INSTALLED, ex);

			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public string GetServerVersion(int serverId)
		{
			AutoDiscovery ad = new AutoDiscovery();
			ServiceProviderProxy.ServerInit(ad, serverId);

			return ad.GetServerVersion();
		}

		public string GetServerFilePath(int serverId)
		{
			AutoDiscovery ad = new AutoDiscovery();
			ServiceProviderProxy.ServerInit(ad, serverId);

			return ad.GetServerFilePath(); // ad.GetServer
		}

        public bool GetQuotaHidden(string quotaName, int groupID)
        {
            return Database.GetQuotaHidden(quotaName, groupID);
        }

        public int UpdateQuotaHidden(string quotaName, int groupID, bool hideQuota)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
                | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // update server
            return Database.UpdateQuotaHidden(quotaName, groupID, hideQuota);
        }

        #endregion

        #region Private / DMZ Network VLANs
        public VLANsPaged GetPrivateNetworkVLANsPaged(int serverId, string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
        {
            VLANsPaged result = new VLANsPaged();

			// get reader
			IDataReader reader = Database.GetPrivateNetworkVLANsPaged(SecurityContext.User.UserId, serverId, filterColumn, filterValue, sortColumn, startRow, maximumRows);

			// number of items = first data reader
			reader.Read();
			result.Count = (int)reader[0];

			// items = second data reader
			reader.NextResult();
			result.Items = ObjectUtils.CreateListFromDataReader<VLANInfo>(reader).ToArray();

			return result;
		}

		public IntResult AddPrivateNetworkVLAN(int serverId, int vlan, string comments)
		{
			IntResult res = new IntResult();

			#region Check account statuses
			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsAdmin | DemandAccount.IsActive))
				return res;
			#endregion

			// start task
			res = TaskManager.StartResultTask<IntResult>("VLAN", "ADD", vlan.ToString());

			TaskManager.WriteParameter("ServerID", serverId);

			try
			{
				res.Value = Database.AddPrivateNetworkVLAN(serverId, vlan, comments);

			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "VLAN_ADD_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public ResultObject DeletePrivateNetworkVLANs(int[] vlans)
		{
			ResultObject res = new ResultObject();

			#region Check account statuses
			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsAdmin | DemandAccount.IsActive))
				return res;
			#endregion

			// start task
			res = TaskManager.StartResultTask<ResultObject>("VLAN", "DELETE_RANGE");

			try
			{
				foreach (int vlanId in vlans)
				{
					ResultObject vlanRes = DeletePrivateNetworkVLAN(vlanId);
					if (!vlanRes.IsSuccess && vlanRes.ErrorCodes.Count > 0)
					{
						res.ErrorCodes.AddRange(vlanRes.ErrorCodes);
						res.IsSuccess = false;
					}
				}
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "VLAN_DELETE_RANGE_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		private ResultObject DeletePrivateNetworkVLAN(int vlanId)
		{
			ResultObject res = new ResultObject();

			// start task
			res = TaskManager.StartResultTask<ResultObject>("VLAN", "DELETE");

			try
			{
				int result = Database.DeletePrivateNetworkVLAN(vlanId);
				if (result == -2)
				{
					TaskManager.CompleteResultTask(res, "ERROR_VLAN_USED_BY_PACKAGE_ITEM");
					return res;
				}
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "VLAN_DELETE_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public ResultObject AddPrivateNetworkVLANsRange(int serverId, int startVLAN, int endVLAN, string comments)
		{
			ResultObject res = new ResultObject();

			#region Check account statuses
			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsAdmin | DemandAccount.IsActive))
				return res;
			#endregion

			// start task
			res = TaskManager.StartResultTask<ResultObject>("VLAN", "ADD_RANGE", startVLAN);

			TaskManager.WriteParameter("ServerID", serverId);
			TaskManager.WriteParameter("End VLAN", endVLAN);

			try
			{
				for (int i = startVLAN; i <= endVLAN; i++)
				{
					Database.AddPrivateNetworkVLAN(serverId, i, comments);
				}
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "VLAN_ADD_RANGE_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public VLANInfo GetPrivateNetworVLAN(int vlanId)
		{
			return ObjectUtils.FillObjectFromDataReader<VLANInfo>(
				 Database.GetPrivateNetworVLAN(vlanId));
		}

		public ResultObject UpdatePrivateNetworVLAN(int vlanId, int serverId, int vlan, string comments)
		{
			ResultObject res = new ResultObject();

			#region Check account statuses
			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsAdmin | DemandAccount.IsActive))
				return res;
			#endregion

			// start task
			res = TaskManager.StartResultTask<ResultObject>("VLAN", "UPDATE");

			try
			{
				Database.UpdatePrivateNetworVLAN(vlanId, serverId, vlan, comments);
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "VLAN_UPDATE_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public PackageVLANsPaged GetPackagePrivateNetworkVLANs(int packageId, string sortColumn, int startRow, int maximumRows)
		{
			PackageVLANsPaged result = new PackageVLANsPaged();

			// get reader
			IDataReader reader = Database.GetPackagePrivateNetworkVLANs(packageId, sortColumn, startRow, maximumRows);

			// number of items = first data reader
			reader.Read();
			result.Count = (int)reader[0];

			// items = second data reader
			reader.NextResult();
			result.Items = ObjectUtils.CreateListFromDataReader<PackageVLAN>(reader).ToArray();

			return result;
		}

		public PackageVLANsPaged GetPackageDmzNetworkVLANs(int packageId, string sortColumn, int startRow, int maximumRows)
		{
			PackageVLANsPaged result = new PackageVLANsPaged();

			// get reader
			IDataReader reader = Database.GetPackageDmzNetworkVLANs(packageId, sortColumn, startRow, maximumRows);

			// number of items = first data reader
			reader.Read();
			result.Count = (int)reader[0];

			// items = second data reader
			reader.NextResult();
			result.Items = ObjectUtils.CreateListFromDataReader<PackageVLAN>(reader).ToArray();

			return result;
		}

		public ResultObject DeallocatePackageVLANs(int packageId, int[] packageVlanId)
        {
            #region Check account and space statuses
            // create result object
            ResultObject res = new ResultObject();

			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsActive))
				return res;

			// check package
			if (!SecurityContext.CheckPackage(res, packageId, DemandPackage.IsActive))
				return res;
			#endregion

			res = TaskManager.StartResultTask<ResultObject>("VLAN", "DEALLOCATE_PACKAGE_VLAN", packageId);

			try
			{
				foreach (int id in packageVlanId)
				{
					Database.DeallocatePackageVLAN(id);
				}
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "DEALLOCATE_PACKAGE_VLAN_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public List<VLANInfo> GetUnallottedVLANs(int packageId, string groupName)
		{

			int serviceId = 0;
			bool servicebyid = int.TryParse(groupName, out serviceId);
			if (!servicebyid) // get service ID
				serviceId = PackageController.GetPackageServiceId(packageId, groupName);


			// get unallotted vlans
			return ObjectUtils.CreateListFromDataReader<VLANInfo>(
				 Database.GetUnallottedVLANs(packageId, serviceId));
		}

		public void AllocatePackageVLANs(int packageId, int[] vlanIds, bool isDmz)
		{
			if (vlanIds == null || vlanIds.Length == 0) return;
			// prepare XML document
			string xml = PrepareXML(vlanIds);

			// save to database
			Database.AllocatePackageVLANs(packageId, isDmz, xml);
		}

		public ResultObject AllocatePackageVLANs(int packageId, string groupName, bool allocateRandom, int vlansNumber, int[] vlanId, bool isDmz)
        {
            #region Check account and space statuses
            // create result object
            ResultObject res = new ResultObject();

			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsActive))
				return res;

			// check package
			if (!SecurityContext.CheckPackage(res, packageId, DemandPackage.IsActive))
				return res;
			#endregion

			// get total number of addresses requested
			if (!allocateRandom && vlanId != null)
				vlansNumber = vlanId.Length;

			if (vlansNumber <= 0)
			{
				res.IsSuccess = true;
				return res; // just exit
			}

			string quotaName;
			if (isDmz)
            {
				quotaName = Quotas.VPS2012_DMZ_VLANS_NUMBER;
			}
            else
            {
				quotaName = Quotas.VPS2012_PRIVATE_VLANS_NUMBER;
			}

			// get maximum server IPs
			List<VLANInfo> vlans = ServerController.GetUnallottedVLANs(packageId, groupName);
			int maxAvailableVLANs = vlans.Count;

			if (maxAvailableVLANs == 0)
			{
				res.ErrorCodes.Add("VLANS_POOL_IS_EMPTY");
				return res;
			}

			// get hosting plan VLAN limits
			PackageContext cntx = PackageController.GetPackageContext(packageId);
			int quotaAllocated = cntx.Quotas[quotaName].QuotaAllocatedValue;
			int quotaUsed = cntx.Quotas[quotaName].QuotaUsedValue;

			// check the maximum allowed number
			if (quotaAllocated != -1) // check only if not unlimited 
			{
				if (vlansNumber > (quotaAllocated - quotaUsed))
				{
					res.ErrorCodes.Add("VLANS_QUOTA_LIMIT_REACHED");
					return res;
				}
			}

			// check if requested more than available
			if (maxAvailableVLANs != -1 &&
				 (vlansNumber > maxAvailableVLANs))
				vlansNumber = maxAvailableVLANs;

			res = TaskManager.StartResultTask<ResultObject>("VLAN", "ALLOCATE_PACKAGE_VLAN", packageId);

			try
			{
				if (allocateRandom)
				{
					int[] ids = new int[vlansNumber];
					for (int i = 0; i < vlansNumber; i++)
						ids[i] = vlans[i].VlanId;

					vlanId = ids;
				}

				// prepare XML document
				string xml = PrepareXML(vlanId);

                // save to database
                try
                {
                    Database.AllocatePackageVLANs(packageId, isDmz, xml);
                }
                catch (Exception ex)
                {
                    TaskManager.CompleteResultTask(res, "VPS_CANNOT_ADD_VLANS_TO_DATABASE", ex);
                    return res;
                }
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, "VPS_ALLOCATE_PRIVATE_VLANS_GENERAL_ERROR", ex);
                return res;
            }

			TaskManager.CompleteResultTask();
			return res;
		}
		#endregion

		#region IP Addresses
		public List<IPAddressInfo> GetIPAddresses(IPAddressPool pool, int serverId)
		{
			return ObjectUtils.CreateListFromDataReader<IPAddressInfo>(
				Database.GetIPAddresses(SecurityContext.User.UserId, (int)pool, serverId));
		}

		public IPAddressesPaged GetIPAddressesPaged(IPAddressPool pool, int serverId,
			string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
		{
			IPAddressesPaged result = new IPAddressesPaged();

			// get reader
			IDataReader reader = Database.GetIPAddressesPaged(SecurityContext.User.UserId, (int)pool, serverId, filterColumn, filterValue, sortColumn, startRow, maximumRows);

			// number of items = first data reader
			reader.Read();
			result.Count = (int)reader[0];

			// items = second data reader
			reader.NextResult();
			result.Items = ObjectUtils.CreateListFromDataReader<IPAddressInfo>(reader).ToArray();

			return result;
		}

		public IPAddressInfo GetIPAddress(int addressId)
		{
			return ObjectUtils.FillObjectFromDataReader<IPAddressInfo>(
				Database.GetIPAddress(addressId));
		}

		public string GetExternalIPAddress(int addressId)
		{
			IPAddressInfo ip = GetIPAddress(addressId);
			return (ip != null ? ip.ExternalIP : null);
		}

		public IntResult AddIPAddress(IPAddressPool pool, int serverId,
			string externalIP, string internalIP, string subnetMask, string defaultGateway, string comments, int VLAN)
		{
			IntResult res = new IntResult();

			#region Check account statuses
			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsAdmin | DemandAccount.IsActive))
				return res;
			#endregion

			// start task
			res = TaskManager.StartResultTask<IntResult>("IP_ADDRESS", "ADD", externalIP);

			TaskManager.WriteParameter("IP Address", externalIP);
			TaskManager.WriteParameter("NAT Address", internalIP);

			try
			{
				res.Value = Database.AddIPAddress((int)pool, serverId, externalIP, internalIP,
											subnetMask, defaultGateway, comments, VLAN);

			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "IP_ADDRESS_ADD_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public ResultObject AddIPAddressesRange(IPAddressPool pool, int serverId,
			string externalIP, string endIP, string internalIP, string subnetMask, string defaultGateway, string comments, int VLAN)
		{
			const int MaxSubnet = 512; // TODO bigger max subnet?

			ResultObject res = new ResultObject();

			#region Check account statuses
			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsAdmin | DemandAccount.IsActive))
				return res;
			#endregion

			// start task
			res = TaskManager.StartResultTask<ResultObject>("IP_ADDRESS", "ADD_RANGE", externalIP);

			TaskManager.WriteParameter("IP Address", externalIP);
			TaskManager.WriteParameter("End IP Address", endIP);
			TaskManager.WriteParameter("NAT Address", internalIP);

			try
			{
				if (externalIP == endIP)
				{
					// add single IP and exit
					AddIPAddress(pool, serverId, externalIP, internalIP, subnetMask, defaultGateway, comments, VLAN);
					TaskManager.CompleteResultTask();
					return res;
				}

				if (pool == IPAddressPool.PhoneNumbers)
				{
					string phoneFormat = "D" + Math.Max(externalIP.Length, endIP.Length);

					UInt64 start = UInt64.Parse(externalIP);
					UInt64 end = UInt64.Parse(endIP);

					if (end < start) { UInt64 temp = start; start = end; end = temp; }

					const UInt64 maxPhones = 1000; // TODO max?

					end = Math.Min(end, start + maxPhones);

					for (UInt64 number = start; number <= end; number++)
						Database.AddIPAddress((int)pool, serverId, number.ToString(phoneFormat), "", subnetMask, defaultGateway, comments, VLAN);
				}

				else
				{
					var startExternalIP = IPAddress.Parse(externalIP);
					var startInternalIP = IPAddress.Parse(internalIP);
					var endExternalIP = IPAddress.Parse(endIP);

					// handle CIDR notation IP/Subnet addresses
					if (startExternalIP.IsSubnet && endExternalIP == null)
					{
						endExternalIP = startExternalIP.LastSubnetIP;
						startExternalIP = startExternalIP.FirstSubnetIP;
					}

					if (startExternalIP.V6 != startInternalIP.V6 && (startExternalIP.V6 != endExternalIP.V6 && endExternalIP != null)) throw new NotSupportedException("All IP addresses must be either V4 or V6.");

					int i = 0;
					long step = ((endExternalIP - startExternalIP) > 0) ? 1 : -1;

					while (true)
					{
						if (i > MaxSubnet)
							break;

						// add IP address
						Database.AddIPAddress((int)pool, serverId, startExternalIP.ToString(), startInternalIP.ToString(), subnetMask, defaultGateway, comments, VLAN);

						if (startExternalIP == endExternalIP)
							break;

						i++;

						startExternalIP += step;

						if (startInternalIP != 0)
							startInternalIP += step;
					}
				}
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "IP_ADDRESS_ADD_RANGE_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public ResultObject UpdateIPAddress(int addressId, IPAddressPool pool, int serverId,
			string externalIP, string internalIP, string subnetMask, string defaultGateway, string comments, int VLAN)
		{
			ResultObject res = new ResultObject();

			#region Check account statuses
			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsAdmin | DemandAccount.IsActive))
				return res;
			#endregion

			// start task
			res = TaskManager.StartResultTask<ResultObject>("IP_ADDRESS", "UPDATE");

			try
			{
				Database.UpdateIPAddress(addressId, (int)pool, serverId, externalIP, internalIP, subnetMask, defaultGateway, comments, VLAN);
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "IP_ADDRESS_UPDATE_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public ResultObject UpdateIPAddresses(int[] addresses, IPAddressPool pool, int serverId,
			string subnetMask, string defaultGateway, string comments, int VLAN)
		{
			ResultObject res = new ResultObject();

			#region Check account statuses
			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsAdmin | DemandAccount.IsActive))
				return res;
			#endregion

			// start task
			res = TaskManager.StartResultTask<ResultObject>("IP_ADDRESS", "UPDATE_RANGE");

			try
			{
				string xmlIds = PrepareXML(addresses);
				Database.UpdateIPAddresses(xmlIds, (int)pool, serverId, subnetMask, defaultGateway, comments, VLAN);
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "IP_ADDRESSES_UPDATE_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public ResultObject DeleteIPAddresses(int[] addresses)
		{
			ResultObject res = new ResultObject();

			#region Check account statuses
			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsAdmin | DemandAccount.IsActive))
				return res;
			#endregion

			// start task
			res = TaskManager.StartResultTask<ResultObject>("IP_ADDRESS", "DELETE_RANGE");

			try
			{
				foreach (int addressId in addresses)
				{
					ResultObject addrRes = DeleteIPAddress(addressId);
					if (!addrRes.IsSuccess && addrRes.ErrorCodes.Count > 0)
					{
						res.ErrorCodes.AddRange(addrRes.ErrorCodes);
						res.IsSuccess = false;
					}
				}
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "IP_ADDRESS_DELETE_RANGE_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public ResultObject DeleteIPAddress(int addressId)
		{
			ResultObject res = new ResultObject();

			#region Check account statuses
			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsAdmin | DemandAccount.IsActive))
				return res;
			#endregion

			// start task
			res = TaskManager.StartResultTask<ResultObject>("IP_ADDRESS", "DELETE");

			try
			{
				int result = Database.DeleteIPAddress(addressId);
				if (result == -1)
				{
					TaskManager.CompleteResultTask(res, "ERROR_IP_USED_IN_NAME_SERVER");
					return res;
				}
				else if (result == -2)
				{
					TaskManager.CompleteResultTask(res, "ERROR_IP_USED_BY_PACKAGE_ITEM");
					return res;
				}
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, "IP_ADDRESS_DELETE_ERROR", ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}
		#endregion

		#region Package IP Addresses
		public PackageIPAddressesPaged GetPackageIPAddresses(int packageId, int orgId, IPAddressPool pool,
			string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows, bool recursive)
		{
			PackageIPAddressesPaged result = new PackageIPAddressesPaged();

			// get reader
			IDataReader reader = Database.GetPackageIPAddresses(packageId, orgId, (int)pool, filterColumn, filterValue, sortColumn, startRow, maximumRows, recursive);

			// number of items = first data reader
			reader.Read();
			result.Count = (int)reader[0];

			// items = second data reader
			reader.NextResult();
			result.Items = ObjectUtils.CreateListFromDataReader<PackageIPAddress>(reader).ToArray();

			return result;
		}

		public int GetPackageIPAddressesCount(int packageId, int orgId, IPAddressPool pool)
		{
			return Database.GetPackageIPAddressesCount(packageId, orgId, (int)pool);
		}

		public List<IPAddressInfo> GetUnallottedIPAddresses(int packageId, string groupName, IPAddressPool pool)
		{

			int serviceId = 0;
			bool servicebyid = int.TryParse(groupName, out serviceId);
			if (!servicebyid) // get service ID
				serviceId = PackageController.GetPackageServiceId(packageId, groupName);


			// get unallotted addresses
			return ObjectUtils.CreateListFromDataReader<IPAddressInfo>(
				Database.GetUnallottedIPAddresses(packageId, serviceId, (int)pool));
		}

		public List<PackageIPAddress> GetPackageUnassignedIPAddresses(int packageId, int orgId, IPAddressPool pool)
		{
			return ObjectUtils.CreateListFromDataReader<PackageIPAddress>(
				Database.GetPackageUnassignedIPAddresses(SecurityContext.User.UserId, packageId, orgId, (int)pool));
		}

		public List<PackageIPAddress> GetPackageUnassignedIPAddresses(int packageId, IPAddressPool pool)
		{
			return GetPackageUnassignedIPAddresses(packageId, 0, pool);
		}

		public void AllocatePackageIPAddresses(int packageId, int[] addressId)
		{
			// prepare XML document
			string xml = PrepareXML(addressId);

			// save to database
			Database.AllocatePackageIPAddresses(packageId, 0, xml);
		}

		public ResultObject AllocatePackageIPAddresses(int packageId, int orgId, string groupName, IPAddressPool pool, bool allocateRandom, int addressesNumber, int[] addressId)
		{
			#region Check account and space statuses
			// create result object
			ResultObject res = new ResultObject();

			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsActive))
				return res;

			// check package
			if (!SecurityContext.CheckPackage(res, packageId, DemandPackage.IsActive))
				return res;
			#endregion

			// get total number of addresses requested
			if (!allocateRandom && addressId != null)
				addressesNumber = addressId.Length;

			if (addressesNumber <= 0)
			{
				res.IsSuccess = true;
				return res; // just exit
			}

			// check quotas
			string quotaName = GetIPAddressesQuotaByResourceGroup(groupName, pool);

			// get maximum server IPs
			List<IPAddressInfo> ips = ServerController.GetUnallottedIPAddresses(packageId, groupName, pool);
			int maxAvailableIPs = ips.Count;

			if (maxAvailableIPs == 0)
			{
				res.ErrorCodes.Add("IP_ADDRESSES_POOL_IS_EMPTY");
				return res;
			}

			// get hosting plan IP limits
			PackageContext cntx = PackageController.GetPackageContext(packageId);
			int quotaAllocated = cntx.Quotas[quotaName].QuotaAllocatedValue;
			int quotaUsed = cntx.Quotas[quotaName].QuotaUsedValue;

			if (pool == IPAddressPool.PhoneNumbers)
				quotaUsed = ServerController.GetPackageIPAddressesCount(packageId, orgId, pool);

			// check the maximum allowed number
			if (quotaAllocated != -1) // check only if not unlimited 
			{
				if (addressesNumber > (quotaAllocated - quotaUsed))
				{
					res.ErrorCodes.Add("IP_ADDRESSES_QUOTA_LIMIT_REACHED");
					return res;
				}
			}

			// check if requested more than available
			if (maxAvailableIPs != -1 &&
				(addressesNumber > maxAvailableIPs))
			{
				res.ErrorCodes.Add("IP_ADDRESSES_POOL_IS_NOT_ENOUGH_IPS");
				return res;
				//addressesNumber = maxAvailableIPs; //it is not good to ignore problem
			}


			res = TaskManager.StartResultTask<ResultObject>("IP_ADDRESS", "ALLOCATE_PACKAGE_IP", packageId);

			try
			{
				if (allocateRandom)
				{
					int[] ids = new int[addressesNumber];
					for (int i = 0; i < addressesNumber; i++)
						ids[i] = ips[i].AddressId;

					addressId = ids;
				}

				// prepare XML document
				string xml = PrepareXML(addressId);

				// save to database
				try
				{
					Database.AllocatePackageIPAddresses(packageId, orgId, xml);
				}
				catch (Exception ex)
				{
					TaskManager.CompleteResultTask(res, VirtualizationErrorCodes.CANNOT_ADD_IP_ADDRESSES_TO_DATABASE, ex);
					return res;
				}
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, VirtualizationErrorCodes.ALLOCATE_EXTERNAL_ADDRESSES_GENERAL_ERROR, ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		public ResultObject AllocateMaximumPackageIPAddresses(int packageId, string groupName, IPAddressPool pool)
		{
			// get maximum server IPs
			int maxAvailableIPs = GetUnallottedIPAddresses(packageId, groupName, pool).Count;

			// get quota name
			string quotaName = GetIPAddressesQuotaByResourceGroup(groupName, pool);

			// get hosting plan IPs
			int number = 0;

			PackageContext cntx = PackageController.GetPackageContext(packageId);
			if (cntx.Quotas.ContainsKey(quotaName))
			{
				number = cntx.Quotas[quotaName].QuotaAllocatedValue;
				if (number == -1)
				{
					// unlimited
					if (number > maxAvailableIPs) // requested more than available
						number = maxAvailableIPs; // assign max available server IPs
				}
				else
				{
					// quota
					number = number - cntx.Quotas[quotaName].QuotaUsedValue;
				}
			}

			// allocate
			return AllocatePackageIPAddresses(packageId, 0, groupName, pool,
				true, number, new int[0]);
		}

        public ResultObject AllocateMaximumPackageVLANs(int packageId, string groupName, bool isDmz)
        {
			// get maximum server VLANs
			int maxAvailableVLANs = GetUnallottedVLANs(packageId, groupName).Count;

			// get hosting plan VLANs
			int number = 0;

            string quotaName;
			if (isDmz)
            {
				quotaName = Quotas.VPS2012_DMZ_VLANS_NUMBER;
			}
            else
            {
				quotaName = Quotas.VPS2012_PRIVATE_VLANS_NUMBER;
			}

			PackageContext cntx = PackageController.GetPackageContext(packageId);
			if (cntx.Quotas.ContainsKey(quotaName))
            {
                if (cntx.Quotas[quotaName].QuotaAllocatedValue == -1)
                {
                    // unlimited
                    //number = maxAvailableVLANs; // assign max available server VLANs
                    if (maxAvailableVLANs > 0)
                    {
                        number = 1;//assign 1 VLAN or the entire free pool if unlimited. What is better???
                    }
                    else number = 0;
                }
                else
                {
                    // quota
                    number = cntx.Quotas[quotaName].QuotaAllocatedValue - cntx.Quotas[quotaName].QuotaUsedValue;
                }
            }

            // allocate
            return AllocatePackageVLANs(packageId, groupName, true, number, new int[0], isDmz);
        }

		public ResultObject DeallocatePackageIPAddresses(int packageId, int[] addressId)
		{
			#region Check account and space statuses
			// create result object
			ResultObject res = new ResultObject();

			// check account
			if (!SecurityContext.CheckAccount(res, DemandAccount.NotDemo | DemandAccount.IsActive))
				return res;

			// check package
			if (!SecurityContext.CheckPackage(res, packageId, DemandPackage.IsActive))
				return res;
			#endregion

			res = TaskManager.StartResultTask<ResultObject>("IP_ADDRESS", "DEALLOCATE_PACKAGE_IP", packageId);

			try
			{
				foreach (int id in addressId)
				{
					Database.DeallocatePackageIPAddress(id);
				}
			}
			catch (Exception ex)
			{
				TaskManager.CompleteResultTask(res, VirtualizationErrorCodes.CANNOT_DELLOCATE_EXTERNAL_ADDRESSES, ex);
				return res;
			}

			TaskManager.CompleteResultTask();
			return res;
		}

		#region Item IP Addresses
		public List<PackageIPAddress> GetItemIPAddresses(int itemId, IPAddressPool pool)
		{
			return ObjectUtils.CreateListFromDataReader<PackageIPAddress>(
				Database.GetItemIPAddresses(SecurityContext.User.UserId, itemId, (int)pool));
		}

		public PackageIPAddress GetPackageIPAddress(int packageAddressId)
		{
			return ObjectUtils.FillObjectFromDataReader<PackageIPAddress>(
				Database.GetPackageIPAddress(packageAddressId));
		}

		public int AddItemIPAddress(int itemId, int packageAddressId)
		{
			return Database.AddItemIPAddress(SecurityContext.User.UserId, itemId, packageAddressId);
		}

		public int SetItemPrimaryIPAddress(int itemId, int packageAddressId)
		{
			return Database.SetItemPrimaryIPAddress(SecurityContext.User.UserId, itemId, packageAddressId);
		}

		public int DeleteItemIPAddress(int itemId, int packageAddressId)
		{
			return Database.DeleteItemIPAddress(SecurityContext.User.UserId, itemId, packageAddressId);
		}

		public int DeleteItemIPAddresses(int itemId)
		{
			return Database.DeleteItemIPAddresses(SecurityContext.User.UserId, itemId);
		}

		#endregion

		private string PrepareXML(int[] items)
		{
			XmlDocument doc = new XmlDocument();
			XmlNode root = doc.CreateElement("items");
			foreach (int item in items)
			{
				XmlNode node = doc.CreateElement("item");
				XmlAttribute attribute = doc.CreateAttribute("id");
				attribute.Value = item.ToString();
				node.Attributes.Append(attribute);
				root.AppendChild(node);
			}
			doc.AppendChild(root);
			return doc.InnerXml;
		}

		private string GetIPAddressesQuotaByResourceGroup(string groupName, IPAddressPool pool)
		{
			if (pool == IPAddressPool.PhoneNumbers)
			{
				if (groupName == "SfB")
				{
					return Quotas.SFB_PHONE;
				}
				else
				{
					return Quotas.LYNC_PHONE;
				}
			}

			if (String.Compare(groupName, ResourceGroups.VPS, true) == 0)
			{
				return Quotas.VPS_EXTERNAL_IP_ADDRESSES_NUMBER;
			}
			else if (String.Compare(groupName, ResourceGroups.VPS2012, true) == 0)
			{
				return Quotas.VPS2012_EXTERNAL_IP_ADDRESSES_NUMBER;
			}
			else if (String.Compare(groupName, ResourceGroups.VPSForPC, true) == 0)
			{
				return Quotas.VPSForPC_EXTERNAL_IP_ADDRESSES_NUMBER;
			}
			else if (String.Compare(groupName, ResourceGroups.Proxmox, true) == 0)
			{
				return Quotas.PROXMOX_EXTERNAL_IP_ADDRESSES_NUMBER;
			}
			else
			{
				return Quotas.WEB_IP_ADDRESSES;
			}
		}
		#endregion

		#region Clusters
		public List<ClusterInfo> GetClusters()
		{
			List<ClusterInfo> list = new List<ClusterInfo>();
			ObjectUtils.FillCollectionFromDataReader<ClusterInfo>(list,
				Database.GetClusters(SecurityContext.User.UserId));
			return list;
		}

		public int AddCluster(ClusterInfo cluster)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			return Database.AddCluster(cluster.ClusterName);
		}

		public int DeleteCluster(int clusterId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			Database.DeleteCluster(clusterId);

			return 0;
		}
		#endregion

		#region Global DNS records
		public DataSet GetRawDnsRecordsByService(int serviceId)
		{
			return Database.GetDnsRecordsByService(SecurityContext.User.UserId, serviceId);
		}

		public DataSet GetRawDnsRecordsByServer(int serverId)
		{
			return Database.GetDnsRecordsByServer(SecurityContext.User.UserId, serverId);
		}

		public DataSet GetRawDnsRecordsByPackage(int packageId)
		{
			return Database.GetDnsRecordsByPackage(SecurityContext.User.UserId, packageId);
		}

		public DataSet GetRawDnsRecordsByGroup(int groupId)
		{
			return Database.GetDnsRecordsByGroup(groupId);
		}

		public DataSet GetRawDnsRecordsTotal(int packageId)
		{
			return Database.GetDnsRecordsTotal(SecurityContext.User.UserId, packageId);
		}

		public List<GlobalDnsRecord> GetDnsRecordsByService(int serviceId)
		{
			return ObjectUtils.CreateListFromDataSet<GlobalDnsRecord>(
				Database.GetDnsRecordsByService(SecurityContext.User.UserId, serviceId));
		}

		public List<GlobalDnsRecord> GetDnsRecordsByServer(int serverId)
		{
			return ObjectUtils.CreateListFromDataSet<GlobalDnsRecord>(
				Database.GetDnsRecordsByServer(SecurityContext.User.UserId, serverId));
		}

		public List<GlobalDnsRecord> GetDnsRecordsByPackage(int packageId)
		{
			return ObjectUtils.CreateListFromDataSet<GlobalDnsRecord>(
				Database.GetDnsRecordsByPackage(SecurityContext.User.UserId, packageId));
		}

		public List<GlobalDnsRecord> GetDnsRecordsByGroup(int groupId)
		{
			return ObjectUtils.CreateListFromDataSet<GlobalDnsRecord>(
				Database.GetDnsRecordsByGroup(groupId));
		}

		public List<GlobalDnsRecord> GetDnsRecordsTotal(int packageId)
		{
			return ObjectUtils.CreateListFromDataSet<GlobalDnsRecord>(
				GetRawDnsRecordsTotal(packageId));
		}

		public GlobalDnsRecord GetDnsRecord(int recordId)
		{
			return ObjectUtils.FillObjectFromDataReader<GlobalDnsRecord>(
				Database.GetDnsRecord(SecurityContext.User.UserId, recordId));
		}

		public int AddDnsRecord(GlobalDnsRecord record)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsReseller
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			TaskManager.StartTask("GLOBAL_DNS", "ADD", record.RecordName);
			TaskManager.WriteParameter("Type", record.RecordType);
			TaskManager.WriteParameter("Data", record.RecordData);

			Database.AddDnsRecord(SecurityContext.User.UserId, record.ServiceId, record.ServerId, record.PackageId,
				record.RecordType, record.RecordName, record.RecordData, record.MxPriority,
				record.SrvPriority, record.SrvWeight, record.SrvPort, record.IpAddressId);

			TaskManager.CompleteTask();

			return 0;
		}

		public int UpdateDnsRecord(GlobalDnsRecord record)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsReseller
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			TaskManager.StartTask("GLOBAL_DNS", "UPDATE", record.RecordName);
			TaskManager.WriteParameter("Type", record.RecordType);
			TaskManager.WriteParameter("Data", record.RecordData);

			Database.UpdateDnsRecord(SecurityContext.User.UserId, record.RecordId,
				record.RecordType, record.RecordName, record.RecordData, record.MxPriority,
				record.SrvPriority, record.SrvWeight, record.SrvPort, record.IpAddressId);

			TaskManager.CompleteTask();

			return 0;
		}

		public int DeleteDnsRecord(int recordId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsReseller
				| DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			GlobalDnsRecord record = GetDnsRecord(recordId);

			TaskManager.StartTask("GLOBAL_DNS", "DELETE", record.RecordName);
			TaskManager.WriteParameter("Type", record.RecordType);
			TaskManager.WriteParameter("Data", record.RecordData);

			Database.DeleteDnsRecord(SecurityContext.User.UserId, recordId);

			TaskManager.CompleteTask();

			return 0;
		}
		#endregion

		#region Domains

		public List<DnsRecordInfo> GetDomainDnsRecords(int domainId)
		{
			var result = new List<DnsRecordInfo>();

			var records = ObjectUtils.CreateListFromDataReader<DnsRecordInfo>(Database.GetDomainAllDnsRecords(domainId));

			var activeDomain = records.OrderByDescending(x => x.Date).FirstOrDefault();

			if (activeDomain != null)
			{
				records = records.Where(x => x.DnsServer == activeDomain.DnsServer).ToList();
			}

			result.AddRange(records);

			return result;
		}


		public int CheckDomain(string domainName)
		{
			int checkDomainResult = Database.CheckDomain(-10, domainName, false);

			if (checkDomainResult == -1)
				return BusinessErrorCodes.ERROR_DOMAIN_ALREADY_EXISTS;
			else if (checkDomainResult == -2)
				return BusinessErrorCodes.ERROR_RESTRICTED_DOMAIN;
			else
				return checkDomainResult;
		}

		public List<DomainInfo> GetDomains(int packageId, bool recursive)
		{
			return ObjectUtils.CreateListFromDataSet<DomainInfo>(
				Database.GetDomains(SecurityContext.User.UserId, packageId, recursive));
		}

		public List<DomainInfo> GetDomains(int packageId)
		{
			return ObjectUtils.CreateListFromDataSet<DomainInfo>(
				Database.GetDomains(SecurityContext.User.UserId, packageId, true));
		}


		public List<DomainInfo> GetDomainsByZoneId(int zoneId)
		{
			return ObjectUtils.CreateListFromDataSet<DomainInfo>(
				Database.GetDomainsByZoneId(SecurityContext.User.UserId, zoneId));
		}

		public List<DomainInfo> GetDomainsByDomainItemId(int zoneId)
		{
			return ObjectUtils.CreateListFromDataSet<DomainInfo>(
				Database.GetDomainsByDomainItemId(SecurityContext.User.UserId, zoneId));
		}


		public List<DomainInfo> GetMyDomains(int packageId)
		{
			return ObjectUtils.CreateListFromDataSet<DomainInfo>(
				Database.GetDomains(SecurityContext.User.UserId, packageId, false));
		}

		public List<DomainInfo> GetResellerDomains(int packageId)
		{
			return ObjectUtils.CreateListFromDataSet<DomainInfo>(
				Database.GetResellerDomains(SecurityContext.User.UserId, packageId));
		}

		public DataSet GetDomainsPaged(int packageId, int serverId, bool recursive, string filterColumn, string filterValue,
			string sortColumn, int startRow, int maximumRows)
		{
			DataSet ds = Database.GetDomainsPaged(SecurityContext.User.UserId,
				packageId, serverId, recursive, filterColumn, filterValue,
				sortColumn, startRow, maximumRows);

			return ds;
		}

		public DomainInfo GetDomain(int domainId, bool withLog = true)
		{
			// get domain by ID
			DomainInfo domain = GetDomainItem(domainId);

			//get default TTL
			if (domainId > 0)
			{
				StringDictionary settings = GetServiceSettings(domain.ZoneServiceID);
				domain.RecordDefaultTTL = Convert.ToInt32(settings["RecordDefaultTTL"]);
				if (domain.RecordDefaultTTL == 0) domain.RecordDefaultTTL = 86400;
				domain.RecordMinimumTTL = Convert.ToInt32(settings["RecordMinimumTTL"]);
				if (domain.RecordMinimumTTL == 0) domain.RecordMinimumTTL = 3600;
				domain.MinimumTTL = Convert.ToInt32(settings["MinimumTTL"]);
			}

			// return
			return GetDomain(domain, withLog);
		}

		public DomainInfo GetDomain(string domainName)
		{
			return ObjectUtils.FillObjectFromDataReader<DomainInfo>(
				Database.GetDomainByName(SecurityContext.User.UserId, domainName, false, false));
		}

		public DomainInfo GetDomain(string domainName, bool searchOnDomainPointer, bool isDomainPointer)
		{
			return GetDomainItem(domainName, searchOnDomainPointer, isDomainPointer);
		}


		private DomainInfo GetDomain(DomainInfo domain, bool withLog = true)
		{
			// check domain
			if (domain == null)
				return null;

			// get Preview Domain
			domain.PreviewDomainName = GetDomainAlias(domain.PackageId, domain.DomainName);
			DomainInfo previewDomain = GetDomainItem(domain.PreviewDomainName, true, false);
			if (previewDomain != null)
				domain.PreviewDomainId = previewDomain.DomainId;

			// Log Extension
			if (withLog)
				LogExtension.WriteObject(domain);

			return domain;
		}

		public DomainInfo GetDomainItem(int domainId)
		{
			return ObjectUtils.FillObjectFromDataReader<DomainInfo>(
				Database.GetDomain(SecurityContext.User.UserId, domainId));
		}

		public DomainInfo GetDomainItem(string domainName)
		{
			return GetDomainItem(domainName, false, false);
		}


		public DomainInfo GetDomainItem(string domainName, bool searchOnDomainPointer, bool isDomainPointer)
		{
			return ObjectUtils.FillObjectFromDataReader<DomainInfo>(
				Database.GetDomainByName(SecurityContext.User.UserId, domainName, searchOnDomainPointer, isDomainPointer));
		}

		public string GetDomainAlias(int packageId, string domainName)
		{
			// load package settings
			PackageSettings packageSettings = PackageController.GetPackageSettings(packageId,
				PackageSettings.INSTANT_ALIAS);

			string previewDomain = packageSettings["PreviewDomain"];

			// add Preview Domain
			if (!String.IsNullOrEmpty(previewDomain))
			{
				previewDomain = domainName + "." + previewDomain;
			}
			return previewDomain;
		}

		public int AddDomainWithProvisioning(int packageId, string domainName, DomainType domainType,
			bool createWebSite, int pointWebSiteId, int pointMailDomainId, bool createDnsZone, bool createPreviewDomain, bool allowSubDomains, string hostName)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			if (domainType == DomainType.Domain)
			{
				PackageContext cntx = PackageController.GetPackageContext(packageId);
				if (!cntx.Quotas[Quotas.OS_NOTALLOWTENANTCREATEDOMAINS].QuotaExhausted)
				{
					accountCheck = SecurityContext.CheckAccount(DemandAccount.IsAdmin);
					if (accountCheck < 0) return accountCheck;
				}
			}

			// check package
			int packageCheck = SecurityContext.CheckPackage(packageId, DemandPackage.IsActive);
			if (packageCheck < 0) return packageCheck;

			// set flags
			bool isSubDomain = (domainType == DomainType.SubDomain || domainType == DomainType.ProviderSubDomain);
			bool isDomainPointer = (domainType == DomainType.DomainPointer);

			// check services
			bool dnsEnabled = (PackageController.GetPackageServiceId(packageId, ResourceGroups.Dns) > 0);
			bool webEnabled = (PackageController.GetPackageServiceId(packageId, ResourceGroups.Web) > 0);
			bool mailEnabled = (PackageController.GetPackageServiceId(packageId, ResourceGroups.Mail) > 0);
			bool MailCleanerEnabled = (PackageController.GetPackageServiceId(packageId, ResourceGroups.Filters) > 0);

			// add main domain
			int domainId = AddDomainInternal(packageId, domainName, createDnsZone && dnsEnabled, isSubDomain, false, isDomainPointer, allowSubDomains);
			if (domainId < 0)
				return domainId;

			DomainInfo domain = ServerController.GetDomain(domainId);
			if (domain != null)
			{
				if (domain.ZoneItemId != 0)
				{
					AddAllServiceDNS(domain);
				}

				UpdateDomainWhoisData(domain);
			}

			// create web site if requested
			int webSiteId = 0;
			if (webEnabled && createWebSite)
			{
				webSiteId = WebServerController.AddWebSite(packageId, hostName, domainId, 0, createPreviewDomain, false);

				if (webSiteId < 0)
				{
					// return
					return webSiteId;
				}
			}

			// add web site pointer
			if (webEnabled && pointWebSiteId > 0)
			{
				WebServerController.AddWebSitePointer(pointWebSiteId, hostName, domainId, true, false, false);
			}

			// add mail domain pointer
			if (mailEnabled && pointMailDomainId > 0)
			{
				MailServerController.AddMailDomainPointer(pointMailDomainId, domainId);
			}

			// add Preview Domain
			createPreviewDomain &= (domainType != DomainType.DomainPointer);
			if (createPreviewDomain)
			{
				// check if Preview Domain is configured
				string domainAlias = GetDomainAlias(packageId, domainName);

				// add Preview Domain if required
				if (!String.IsNullOrEmpty(domainAlias))
				{
					// add alias
					CreateDomainPreviewDomain(hostName, domainId);
				}
			}

			return domainId;
		}

		public int AddDomain(DomainInfo domain)
		{
			return AddDomain(domain, false, false);
		}

		public int AddDomain(DomainInfo domain, bool createPreviewDomain, bool createZone)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			// check package
			int packageCheck = SecurityContext.CheckPackage(domain.PackageId, DemandPackage.IsActive);
			if (packageCheck < 0) return packageCheck;

			// add main domain
			int domainId = AddDomainInternal(domain.PackageId, domain.DomainName, createZone,
				domain.IsSubDomain, createPreviewDomain, domain.IsDomainPointer, false);

			/*
            if (domainId < 0)
                return domainId;

            // add Preview Domain if required
            string domainAlias = GetDomainAlias(domain.PackageId, domain.DomainName);
            if (createPreviewDomain && !String.IsNullOrEmpty(domainAlias))
            {
                AddDomainInternal(domain.PackageId, domainAlias, true, false, true, false, false);
            }
            */

			return domainId;
		}

		private int AddDomainInternal(int packageId, string domainName,
			bool createDnsZone, bool isSubDomain, bool isPreviewDomain, bool isDomainPointer, bool allowSubDomains)
		{
			// check quota
			if (!isPreviewDomain)
			{
				if (isSubDomain)
				{
					// sub-domain
					if (PackageController.GetPackageQuota(packageId, Quotas.OS_SUBDOMAINS).QuotaExhausted)
						return BusinessErrorCodes.ERROR_SUBDOMAIN_QUOTA_LIMIT;
				}
				else if (isDomainPointer)
				{
					// domain pointer
					//if (PackageController.GetPackageQuota(packageId, Quotas.OS_DOMAINPOINTERS).QuotaExhausted)
					//    return BusinessErrorCodes.ERROR_DOMAIN_QUOTA_LIMIT;
				}
				else
				{
					// top-level domain
					if (PackageController.GetPackageQuota(packageId, Quotas.OS_DOMAINS).QuotaExhausted)
						return BusinessErrorCodes.ERROR_DOMAIN_QUOTA_LIMIT;
				}
			}

			// check if the domain already exists
			int checkResult = Database.CheckDomain(packageId, domainName, isDomainPointer);

			if (checkResult < 0)
			{
				if (checkResult == -1)
					return BusinessErrorCodes.ERROR_DOMAIN_ALREADY_EXISTS;
				else if (checkResult == -2)
					return BusinessErrorCodes.ERROR_RESTRICTED_DOMAIN;
				else
					return checkResult;
			}

			//        if (domainName.ToLower().StartsWith("www."))
			//            return BusinessErrorCodes.ERROR_DOMAIN_STARTS_WWW;

			// place log record
			TaskManager.StartTask("DOMAIN", "ADD", domainName, 0, packageId, new BackgroundTaskParameter("CreateZone", createDnsZone));

			// Log Extension
			LogExtension.WriteVariables(new { domainName, createDnsZone, isSubDomain, isPreviewDomain, isDomainPointer, allowSubDomains });

			// create DNS zone
			int zoneItemId = 0;
			if (createDnsZone)
			{
				try
				{
					// add DNS zone
					int serviceId = PackageController.GetPackageServiceId(packageId, ResourceGroups.Dns);
					if (serviceId > 0)
					{
						zoneItemId = DnsServerController.AddZone(packageId, serviceId, domainName, true, isPreviewDomain);
					}

					if (zoneItemId < 0)
					{
						TaskManager.CompleteTask();
						return zoneItemId;
					}
				}
				catch (Exception ex)
				{
					throw TaskManager.WriteError(ex);
				}
			}

			int itemId = Database.AddDomain(SecurityContext.User.UserId,
				packageId, zoneItemId, domainName, allowSubDomains, 0, 0, isSubDomain, isPreviewDomain, isDomainPointer);

			TaskManager.ItemId = itemId;

			TaskManager.CompleteTask();

			return itemId;
		}

		public int AddDomainItem(DomainInfo domain)
		{
			return Database.AddDomain(SecurityContext.User.UserId,
				domain.PackageId, domain.ZoneItemId, domain.DomainName, domain.HostingAllowed,
				domain.WebSiteId, domain.MailDomainId, domain.IsSubDomain, domain.IsPreviewDomain, domain.IsDomainPointer);
		}

		public void AddServiceDNSRecords(int packageId, string groupName, DomainInfo domain, string serviceIP)
		{
			AddServiceDNSRecords(packageId, groupName, domain, serviceIP, false);
		}

		public void AddServiceDNSRecords(int packageId, string groupName, DomainInfo domain, string serviceIP, bool wildcardOnly)
		{
			int serviceId = PackageController.GetPackageServiceId(packageId, groupName);
			if (serviceId > 0)
			{
				List<DnsRecord> tmpZoneRecords = new List<DnsRecord>();
				List<GlobalDnsRecord> dnsRecords = ServerController.GetDnsRecordsByService(serviceId);

				if (wildcardOnly)
				{
					List<GlobalDnsRecord> temp = new List<GlobalDnsRecord>();
					foreach (GlobalDnsRecord d in dnsRecords)
					{
						if ((d.RecordName == "*") ||
							(d.RecordName == "@"))
							temp.Add(d);
					}

					dnsRecords = temp;
				}

				DnsZone zone = (DnsZone)PackageController.GetPackageItem(domain.ZoneItemId);
				tmpZoneRecords.AddRange(DnsServerController.BuildDnsResourceRecords(dnsRecords, "", domain.ZoneName, serviceIP));

				try
				{
					DNSServer dns = new DNSServer();
					ServiceProviderProxy.Init(dns, zone.ServiceId);

					DnsRecord[] domainRecords = dns.GetZoneRecords(domain.DomainName);

					List<DnsRecord> zoneRecords = new List<DnsRecord>();
					foreach (DnsRecord t in tmpZoneRecords)
					{
						if (!RecordDoesExist(t, domainRecords))
							zoneRecords.Add(t);
					}


					// add new resource records
					dns.AddZoneRecords(zone.Name, zoneRecords.ToArray());
				}
				catch (Exception ex1)
				{
					TaskManager.WriteError(ex1, "Error updating DNS records");
				}
			}
		}



		public void RemoveServiceDNSRecords(int packageId, string groupName, DomainInfo domain, string serviceIP, bool wildcardOnly)
		{
			int serviceId = PackageController.GetPackageServiceId(packageId, groupName);
			if (serviceId > 0)
			{
				List<DnsRecord> zoneRecords = new List<DnsRecord>();
				List<GlobalDnsRecord> dnsRecords = ServerController.GetDnsRecordsByService(serviceId);
				if (wildcardOnly)
				{
					List<GlobalDnsRecord> temp = new List<GlobalDnsRecord>();
					foreach (GlobalDnsRecord d in dnsRecords)
					{
						if ((d.RecordName == "*") ||
							(d.RecordName == "@"))
							temp.Add(d);
					}

					dnsRecords = temp;
				}

				DnsZone zone = (DnsZone)PackageController.GetPackageItem(domain.ZoneItemId);
				zoneRecords.AddRange(DnsServerController.BuildDnsResourceRecords(dnsRecords, "", domain.ZoneName, serviceIP));

				try
				{
					DNSServer dns = new DNSServer();
					ServiceProviderProxy.Init(dns, zone.ServiceId);

					// add new resource records
					dns.DeleteZoneRecords(zone.Name, zoneRecords.ToArray());
				}
				catch (Exception ex1)
				{
					TaskManager.WriteError(ex1, "Error updating DNS records");
				}
			}
		}


		private bool RecordDoesExist(DnsRecord record, DnsRecord[] domainRecords)
		{
			foreach (DnsRecord d in domainRecords)
			{
				if ((record.RecordName.ToLower() == d.RecordName.ToLower()) &
					(record.RecordType == d.RecordType))
				{
					return true;
				}
			}

			return false;
		}


		public int UpdateDomain(DomainInfo domain)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo);
			if (accountCheck < 0) return accountCheck;

			// place log record
			DomainInfo origDomain = GetDomain(domain.DomainId);
			TaskManager.StartTask("DOMAIN", "UPDATE", origDomain.DomainName, domain.DomainId);

			try
			{
				Database.UpdateDomain(SecurityContext.User.UserId,
					domain.DomainId, domain.ZoneItemId, domain.HostingAllowed, domain.WebSiteId,
					domain.MailDomainId, domain.DomainItemId);

				return 0;
			}
			catch (Exception ex)
			{
				throw TaskManager.WriteError(ex);
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public int DetachDomain(int domainId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin);
			if (accountCheck < 0) return accountCheck;

			// load domain
			DomainInfo domain = GetDomain(domainId);
			if (domain == null)
				return 0;

			// place log record
			TaskManager.StartTask("DOMAIN", "DETACH", domain.DomainName, domain.DomainId);

			try
			{
				// check if domain can be deleted
				if (domain.WebSiteId > 0)
				{
					TaskManager.WriteError("Domain points to the existing web site");
					return BusinessErrorCodes.ERROR_DOMAIN_POINTS_TO_WEB_SITE;
				}

				if (domain.MailDomainId > 0)
				{
					TaskManager.WriteError("Domain points to the existing mail domain");
					return BusinessErrorCodes.ERROR_DOMAIN_POINTS_TO_MAIL_DOMAIN;
				}

				if (Database.ExchangeOrganizationDomainExists(domain.DomainId))
				{
					TaskManager.WriteError("Domain points to the existing organization domain");
					return BusinessErrorCodes.ERROR_ORGANIZATION_DOMAIN_IS_IN_USE;
				}


				List<DomainInfo> domains = GetDomainsByDomainItemId(domain.DomainId);
				foreach (DomainInfo d in domains)
				{
					if (d.WebSiteId > 0)
					{
						TaskManager.WriteError("Domain points to the existing web site");
						return BusinessErrorCodes.ERROR_DOMAIN_POINTS_TO_WEB_SITE;
					}
				}

				// Find and delete all zone items for this domain
				var zoneItems = PackageController.GetPackageItemsByType(domain.PackageId, ResourceGroups.Dns, typeof(DnsZone));
				zoneItems.AddRange(PackageController.GetPackageItemsByType(domain.PackageId, ResourceGroups.Dns, typeof(SecondaryDnsZone)));

				foreach (var zoneItem in zoneItems.Where(z => z.Name == domain.ZoneName))
				{
					PackageController.DeletePackageItem(zoneItem.Id);
				}

				// delete domain
				Database.DeleteDomain(SecurityContext.User.UserId, domainId);

				return 0;
			}
			catch (Exception ex)
			{
				throw TaskManager.WriteError(ex);
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public int DeleteDomain(int domainId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo);
			if (accountCheck < 0) return accountCheck;

			// load domain
			DomainInfo domain = GetDomain(domainId);
			if (domain == null)
				return 0;

			if (!(domain.IsDomainPointer || domain.IsSubDomain || domain.IsPreviewDomain))
			{
				PackageContext cntx = PackageController.GetPackageContext(domain.PackageId);
				if (!cntx.Quotas[Quotas.OS_NOTALLOWTENANTDELETEDOMAINS].QuotaExhausted)
				{
					accountCheck = SecurityContext.CheckAccount(DemandAccount.IsAdmin);
					if (accountCheck < 0) return accountCheck;
				}
			}

			// place log record
			TaskManager.StartTask("DOMAIN", "DELETE", domain.DomainName, domain.DomainId);

			try
			{
				// check if domain can be deleted
				if (domain.WebSiteId > 0)
				{
					TaskManager.WriteError("Domain points to the existing web site");
					return BusinessErrorCodes.ERROR_DOMAIN_POINTS_TO_WEB_SITE;
				}

				if (domain.MailDomainId > 0)
				{
					TaskManager.WriteError("Domain points to the existing mail domain");
					return BusinessErrorCodes.ERROR_DOMAIN_POINTS_TO_MAIL_DOMAIN;
				}

				if (Database.ExchangeOrganizationDomainExists(domain.DomainId))
				{
					TaskManager.WriteError("Domain points to the existing organization domain");
					return BusinessErrorCodes.ERROR_ORGANIZATION_DOMAIN_IS_IN_USE;
				}


				if (!domain.IsDomainPointer)
				{
					List<DomainInfo> domains = GetDomainsByDomainItemId(domain.DomainId);
					foreach (DomainInfo d in domains)
					{
						if (d.WebSiteId > 0)
						{
							TaskManager.WriteError("Domain points to the existing web site");
							return BusinessErrorCodes.ERROR_DOMAIN_POINTS_TO_WEB_SITE;
						}
					}
				}


				// delete Preview Domain
				if (domain.PreviewDomainId > 0)
				{
					int res = DeleteDomainPreviewDomain(domainId);
					if (res < 0)
						return res;
				}

				// delete zone if required
				if (!domain.IsDomainPointer)
					DnsServerController.DeleteZone(domain.ZoneItemId);

				// delete domain
				Database.DeleteDomain(SecurityContext.User.UserId, domainId);

				return 0;
			}
			catch (Exception ex)
			{
				throw TaskManager.WriteError(ex);
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public int DisableDomainDns(int domainId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo);
			if (accountCheck < 0) return accountCheck;

			// load domain
			DomainInfo domain = GetDomain(domainId);

			// check if already disabled
			if (domain.ZoneItemId == 0)
				return 0;

			// place log record
			TaskManager.StartTask("DOMAIN", "DISABLE_DNS", domain.DomainName, domain.DomainId);

			try
			{
				// delete Preview Domain
				int aliasResult = DeleteDomainPreviewDomain(domainId);
				if (aliasResult < 0)
					return aliasResult;

				// delete zone if required
				if (domain.ZoneItemId > 0)
				{
					// delete zone
					DnsServerController.DeleteZone(domain.ZoneItemId);

					// update domain item
					domain.ZoneItemId = 0;
					UpdateDomain(domain);
				}

				return 0;
			}
			catch (Exception ex)
			{
				throw TaskManager.WriteError(ex);
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public int EnableDomainDns(int domainId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo);
			if (accountCheck < 0) return accountCheck;

			// load domain
			DomainInfo domain = GetDomain(domainId);

			// check if already enabled
			if (domain.ZoneItemId > 0)
				return 0;

			// place log record
			TaskManager.StartTask("DOMAIN", "ENABLE_DNS", domain.DomainName, domain.DomainId);

			try
			{
				// create DNS zone
				int serviceId = PackageController.GetPackageServiceId(domain.PackageId, ResourceGroups.Dns);
				if (serviceId > 0)
				{
					// add zone
					int zoneItemId = DnsServerController.AddZone(domain.PackageId, serviceId, domain.DomainName);

					// check results
					if (zoneItemId < 0)
					{
						TaskManager.CompleteTask();
						return zoneItemId;
					}

					// update domain
					domain.ZoneItemId = zoneItemId;
					UpdateDomain(domain);

					domain = GetDomain(domainId);

					AddAllServiceDNS(domain);
				}

				// add web site DNS records
				int res = AddWebSiteZoneRecords("", domainId);
				if (res < 0)
					return res;

				return 0;
			}
			catch (Exception ex)
			{
				throw TaskManager.WriteError(ex);
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		private void AddAllServiceDNS(DomainInfo domain)
		{
			PackageContext cntx = PackageController.GetPackageContext(domain.PackageId);
			if (cntx != null)
			{
				// fill dictionaries
				foreach (HostingPlanGroupInfo group in cntx.GroupsArray)
				{
					try
					{
						bool bFound = false;
						switch (group.GroupName)
						{
							case ResourceGroups.Dns:
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.Ftp, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MsSql2000, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MsSql2005, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MsSql2008, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MsSql2012, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MsSql2014, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MsSql2016, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MsSql2017, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MsSql2019, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MsSql2022, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MsSql2025, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MySql4, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MySql5, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MySql8, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MySql9, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.MariaDB, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.Statistics, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.VPS, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.VPS2012, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.VPSForPC, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.Dns, domain, "");
								break;
							case ResourceGroups.Os:
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.Os, domain, "");
								break;
							case ResourceGroups.RDS:
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.RDS, domain, "");
								break;
							case ResourceGroups.HostedOrganizations:
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.HostedOrganizations, domain, "");
								ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.HostedCRM, domain, "");
								break;
							case ResourceGroups.Mail:
								List<DomainInfo> myDomains = ServerController.GetMyDomains(domain.PackageId);
								foreach (DomainInfo mailDomain in myDomains)
								{
									if ((mailDomain.MailDomainId != 0) && (domain.DomainName.ToLower() == mailDomain.DomainName.ToLower()))
									{
										bFound = true;
										break;
									}
								}
								if (bFound) ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.Mail, domain, "");
								break;
							case ResourceGroups.Exchange:
								List<Organization> orgs = OrganizationController.GetOrganizations(domain.PackageId, false);
								foreach (Organization o in orgs)
								{
									List<OrganizationDomainName> names = OrganizationController.GetOrganizationDomains(o.Id);
									foreach (OrganizationDomainName name in names)
									{
										if (domain.DomainName.ToLower() == name.DomainName.ToLower())
										{
											bFound = true;
											break;
										}
									}
									if (bFound) break;
								}
								if (bFound)
								{
									ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.Exchange, domain, "");
									ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.BlackBerry, domain, "");
									ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.OCS, domain, "");
								}
								break;
							case ResourceGroups.Lync:
								List<Organization> orgsLync = OrganizationController.GetOrganizations(domain.PackageId, false);
								foreach (Organization o in orgsLync)
								{
									if ((o.DefaultDomain.ToLower() == domain.DomainName.ToLower()) &
										 (o.LyncTenantId != null))
									{
										bFound = true;
										break;
									}
								}
								if (bFound)
								{
									ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.Lync, domain, "");
								}
								break;
							case ResourceGroups.SfB:
								List<Organization> orgsSfB = OrganizationController.GetOrganizations(domain.PackageId, false);
								foreach (Organization o in orgsSfB)
								{
									if ((o.DefaultDomain.ToLower() == domain.DomainName.ToLower()) &
										 (o.SfBTenantId != null))
									{
										bFound = true;
										break;
									}
								}
								if (bFound)
								{
									ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.SfB, domain, "");
								}
								break;
							case ResourceGroups.Web:
								List<WebSite> sites = WebServerController.GetWebSites(domain.PackageId, false);
								foreach (WebSite w in sites)
								{
									if ((w.SiteId.ToLower().Replace("." + domain.DomainName.ToLower(), "").IndexOf('.') == -1) ||
										 (w.SiteId.ToLower() == domain.DomainName.ToLower()))
									{
										WebServerController.AddWebSitePointer(w.Id,
																							 (w.SiteId.ToLower() == domain.DomainName.ToLower()) ? "" : w.SiteId.ToLower().Replace("." + domain.DomainName.ToLower(), ""),
																							 domain.DomainId, false, true, true);
									}

									List<DomainInfo> pointers = WebServerController.GetWebSitePointers(w.Id);
									foreach (DomainInfo pointer in pointers)
									{
										if ((pointer.DomainName.ToLower().Replace("." + domain.DomainName.ToLower(), "").IndexOf('.') == -1) ||
											 (pointer.DomainName.ToLower() == domain.DomainName.ToLower()))
										{
											WebServerController.AddWebSitePointer(w.Id,
												(pointer.DomainName.ToLower() == domain.DomainName.ToLower()) ? "" : pointer.DomainName.ToLower().Replace("." + domain.DomainName.ToLower(), ""),
												domain.DomainId, false, true, true);
										}
									}
								}

								if (sites.Count == 1)
								{
									// load site item
									IPAddressInfo ip = ServerController.GetIPAddress(sites[0].SiteIPAddressId);

									string serviceIp = (ip != null) ? ip.ExternalIP : null;

									if (string.IsNullOrEmpty(serviceIp))
									{
										StringDictionary settings = ServerController.GetServiceSettings(sites[0].ServiceId);
										if (settings["PublicSharedIP"] != null)
											serviceIp = settings["PublicSharedIP"].ToString();
									}

									ServerController.AddServiceDNSRecords(domain.PackageId, ResourceGroups.Web, domain, serviceIp, true);
								}

								break;
						}
					}
					catch (Exception ex)
					{
						TaskManager.WriteError(ex);
					}
				}
			}
		}

		private int AddWebSiteZoneRecords(string hostName, int domainId)
		{
			// load domain
			DomainInfo domain = GetDomainItem(domainId);
			if (domain == null)
				return 0;

			int res = 0;
			if (domain.WebSiteId > 0)
				res = WebServerController.AddWebSitePointer(domain.WebSiteId, hostName, domainId, false);

			return res;
		}

		public int CreateDomainPreviewDomain(string hostName, int domainId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo);
			if (accountCheck < 0) return accountCheck;

			// load domain
			DomainInfo domain = GetDomain(domainId);

			if (String.IsNullOrEmpty(domain.PreviewDomainName))
				return BusinessErrorCodes.ERROR_INSTANT_ALIAS_IS_NOT_CONFIGURED;

			// place log record
			TaskManager.StartTask("DOMAIN", "CREATE_INSTANT_ALIAS", domain.DomainName, domain.DomainId);

			try
			{
				// check if it already exists
				DomainInfo previewDomain = GetDomainItem(domain.PreviewDomainName);
				int previewDomainId = 0;
				if (previewDomain == null)
				{
					// create Preview Domain
					previewDomainId = AddDomainInternal(domain.PackageId, domain.PreviewDomainName,
						true, false, true, false, false);
					if (previewDomainId < 0)
						return previewDomainId;

					// load Preview Domain again
					previewDomain = GetDomainItem(previewDomainId);
				}

				string parentZone = domain.ZoneName;
				if (string.IsNullOrEmpty(parentZone))
				{
					DomainInfo parentDomain = GetDomain(domain.DomainId);
					parentZone = parentDomain.DomainName;
				}

				if (previewDomainId > 0)
				{
					EnableDomainDns(previewDomainId);
				}

				if (domain.WebSiteId > 0)
				{
					WebServerController.AddWebSitePointer(domain.WebSiteId,
															((domain.DomainName.Replace("." + parentZone, "") == parentZone) |
															(domain.DomainName == parentZone))
															? "" : domain.DomainName.Replace("." + parentZone, ""),
															previewDomain.DomainId);
				}


				// add web site pointer if required
				List<DomainInfo> domains = GetDomainsByDomainItemId(domain.DomainId);
				foreach (DomainInfo d in domains)
				{

					if (d.WebSiteId > 0)
					{
						WebServerController.AddWebSitePointer(d.WebSiteId,
																((d.DomainName.Replace("." + parentZone, "") == parentZone) |
																(d.DomainName == parentZone))
																? "" : d.DomainName.Replace("." + parentZone, ""),
																previewDomain.DomainId);
					}
				}

				// add mail domain pointer
				if (domain.MailDomainId > 0 && previewDomain.MailDomainId == 0)
				{
					int mailRes = MailServerController.AddMailDomainPointer(domain.MailDomainId, previewDomainId);
					if (mailRes < 0)
						return mailRes;
				}

				return 0;
			}
			catch (Exception ex)
			{
				throw TaskManager.WriteError(ex);
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public int DeleteDomainPreviewDomain(int domainId)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo);
			if (accountCheck < 0) return accountCheck;

			// load domain
			DomainInfo domain = GetDomain(domainId);
			if (domain == null)
				return 0;

			// place log record
			TaskManager.StartTask("DOMAIN", "DELETE_INSTANT_ALIAS", domain.DomainName, domain.DomainId);

			try
			{
				// load Preview Domain domain
				DomainInfo previewDomain = GetDomainItem(domain.PreviewDomainName, true, false);
				if (previewDomain == null)
					return 0;

				// remove from web site pointers
				if (previewDomain.WebSiteId > 0)
				{
					int webRes = WebServerController.DeleteWebSitePointer(previewDomain.WebSiteId, previewDomain.DomainId);
					if (webRes < 0)
						return webRes;
				}

				List<DomainInfo> domains = GetDomainsByDomainItemId(previewDomain.DomainId);

				foreach (DomainInfo d in domains)
				{
					if (d.WebSiteId > 0)
					{
						WebServerController.DeleteWebSitePointer(d.WebSiteId, d.DomainId);
					}
				}

				// remove from mail domain pointers
				if (previewDomain.MailDomainId > 0)
				{
					int mailRes = MailServerController.DeleteMailDomainPointer(previewDomain.MailDomainId, previewDomain.DomainId);
					if (mailRes < 0)
						return mailRes;
				}

				// delete Preview Domain
				int res = DeleteDomain(previewDomain.DomainId);
				if (res < 0)
					return res;

				return 0;
			}
			catch (Exception ex)
			{
				throw TaskManager.WriteError(ex);
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public DomainInfo UpdateDomainWhoisData(DomainInfo domain)
		{
			try
			{
				var idn = new IdnMapping();
				var whoisResult = WhoisClient.Query(idn.GetAscii(domain.DomainName).ToLowerInvariant(), new WhoisQueryOptions());

				string creationDateString = ParseWhoisDomainInfo(whoisResult.Raw, _createdDatePatterns);
				string expirationDateString = ParseWhoisDomainInfo(whoisResult.Raw, _expiredDatePatterns);

				domain.CreationDate = ParseDate(creationDateString);
				domain.ExpirationDate = ParseDate(expirationDateString);
				domain.RegistrarName = ParseWhoisDomainInfo(whoisResult.Raw, _registrarNamePatterns);
				domain.LastUpdateDate = DateTime.Now;

				Database.UpdateWhoisDomainInfo(domain.DomainId, domain.CreationDate, domain.ExpirationDate, DateTime.Now, domain.RegistrarName);
			}
			catch (Exception)
			{
				//wrong domain 
			}

			return domain;
		}

		public DomainInfo UpdateDomainWhoisData(DomainInfo domain, DateTime? creationDate, DateTime? expirationDate, string registrarName)
		{
			Database.UpdateWhoisDomainInfo(domain.DomainId, creationDate, expirationDate, DateTime.Now, registrarName);

			domain.CreationDate = creationDate;
			domain.ExpirationDate = expirationDate;
			domain.RegistrarName = registrarName;
			domain.LastUpdateDate = DateTime.Now;

			return domain;
		}

		private string ParseWhoisDomainInfo(string raw, IEnumerable<string> patterns)
		{
			foreach (var createdRegex in patterns)
			{
				var regex = new Regex(createdRegex, RegexOptions.IgnoreCase);

				foreach (Match match in regex.Matches(raw))
				{
					if (match.Success && match.Groups.Count == 2)
					{
						return match.Groups[1].ToString().Trim();
					}
				}
			}

			return null;
		}

		private DateTime? ParseDate(string dateString)
		{
			if (string.IsNullOrEmpty(dateString))
			{
				return null;
			}

			var result = DateTime.MinValue;

			foreach (var datePattern in _datePatterns)
			{
				if (DateTime.TryParseExact(dateString, datePattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
				{
					return result;
				}
			}

			return DateTime.Parse(dateString);
		}

		#endregion

		#region DNS Zones
		public DnsRecord[] GetDnsZoneRecords(int domainId)
		{
			// load domain info
			DomainInfo domain = GetDomain(domainId);

			// get DNS zone
			DnsZone zoneItem = (DnsZone)PackageController.GetPackageItem(domain.ZoneItemId);

			if (zoneItem != null)
			{
				// fill records array
				DNSServer dns = new DNSServer();
				ServiceProviderProxy.Init(dns, zoneItem.ServiceId);

				return dns.GetZoneRecords(zoneItem.Name);
			}

			return new DnsRecord[] { };
		}

		public DataSet GetRawDnsZoneRecords(int domainId)
		{
			DataSet ds = new DataSet();
			DataTable dt = ds.Tables.Add();

			// add columns
			dt.Columns.Add("RecordType", typeof(string));
			dt.Columns.Add("RecordName", typeof(string));
			dt.Columns.Add("RecordTTL", typeof(int));
			dt.Columns.Add("RecordData", typeof(string));
			dt.Columns.Add("MxPriority", typeof(int));
			dt.Columns.Add("SrvPriority", typeof(int));
			dt.Columns.Add("SrvWeight", typeof(int));
			dt.Columns.Add("SrvPort", typeof(int));

			// add rows
			DnsRecord[] records = GetDnsZoneRecords(domainId);
			foreach (DnsRecord record in records)
			{
				dt.Rows.Add(record.RecordType, record.RecordName, record.RecordTTL, record.RecordData, record.MxPriority, record.SrvPriority, record.SrvWeight, record.SrvPort);
			}

			return ds;
		}

		public DnsRecord GetDnsZoneRecord(int domainId, string recordName, DnsRecordType recordType,
			string recordData)
		{
			// get all zone records
			DnsRecord[] records = GetDnsZoneRecords(domainId);
			foreach (DnsRecord record in records)
			{
				if (String.Compare(recordName, record.RecordName, true) == 0
					&& String.Compare(recordData, record.RecordData, true) == 0
					&& recordType == record.RecordType)
					return record;
			}
			return null;
		}

		public int AddDnsZoneRecord(int domainId, string recordName, DnsRecordType recordType,
					string recordData, int mxPriority, int srvPriority, int srvWeight, int srvPort, int recordTTL)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			// load domain info
			DomainInfo domain = GetDomain(domainId);

			// check package
			int packageCheck = SecurityContext.CheckPackage(domain.PackageId, DemandPackage.IsActive);
			if (packageCheck < 0) return packageCheck;

			// get DNS service
			DnsZone zoneItem = (DnsZone)PackageController.GetPackageItem(domain.ZoneItemId);

			if (zoneItem == null)
				return 0;

			// cover the setting not being set for the provider
			if (domain.RecordDefaultTTL == 0) domain.RecordDefaultTTL = 86400;
			if (domain.RecordMinimumTTL == 0) domain.RecordMinimumTTL = 3600;
			// Check Quota for allowing editing TTL
			int EditTTL = 0;
			PackageContext cntx = PackageController.GetPackageContext(domain.PackageId);
			if (cntx != null && cntx.Quotas.ContainsKey("DNS.EditTTL"))
			{
				EditTTL = cntx.Quotas["DNS.EditTTL"].QuotaAllocatedValue;
				if (EditTTL == -1)
					EditTTL = 0;
			}
			if (recordType == DnsRecordType.SOA)
			{
				recordTTL = domain.MinimumTTL;
			}
			else if (EditTTL == 1 && recordType != DnsRecordType.SOA)
			{
				// Make sure quota meets minimum
				if (recordTTL == 0)
				{
					TaskManager.WriteWarning("Record TTL set to 0. Setting to RecordDefaultTTL {0}", domain.RecordDefaultTTL.ToString());
					recordTTL = domain.RecordDefaultTTL;
				}
				else if (domain.RecordMinimumTTL > recordTTL)
				{
					TaskManager.WriteWarning("Tried to set TTL to {0} which is below MinimumTTL. Setting to DefaultTTL {1}", recordTTL.ToString(), domain.RecordDefaultTTL.ToString());
					recordTTL = domain.RecordDefaultTTL;
				}
			}
			else
			{
				recordTTL = domain.RecordDefaultTTL;
			}

			// place log record
			TaskManager.StartTask("DNS_ZONE", "ADD_RECORD", domain.DomainName, domain.ZoneItemId);

			try
			{

				// check if record already exists
				if (GetDnsZoneRecord(domainId, recordName, recordType, recordData) != null)
					return 0;

				DNSServer dns = new DNSServer();
				ServiceProviderProxy.Init(dns, zoneItem.ServiceId);

				DnsRecord record = new DnsRecord();
				record.RecordType = recordType;
				record.RecordName = recordName;
				record.RecordData = recordData;
				record.MxPriority = mxPriority;
				record.SrvPriority = srvPriority;
				record.SrvWeight = srvWeight;
				record.SrvPort = srvPort;
				record.RecordTTL = recordTTL;
				dns.AddZoneRecord(zoneItem.Name, record);

				return 0;
			}
			catch (Exception ex)
			{
				throw TaskManager.WriteError(ex);
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public int UpdateDnsZoneRecord(int domainId,
			string originalRecordName, string originalRecordData,
			string recordName, DnsRecordType recordType, string recordData, int mxPriority, int srvPriority, int srvWeight, int srvPortNumber, int recordTTL)
		{
			// place log record
			DomainInfo domain = GetDomain(domainId);
			TaskManager.StartTask("DNS_ZONE", "UPDATE_RECORD", domain.DomainName, domain.ZoneItemId);

			try
			{

				// delete existing record
				DeleteDnsZoneRecord(domainId, originalRecordName, recordType, originalRecordData);

				// add new record
				AddDnsZoneRecord(domainId, recordName, recordType, recordData, mxPriority, srvPriority, srvWeight, srvPortNumber, recordTTL);

				return 0;
			}
			catch (Exception ex)
			{
				throw TaskManager.WriteError(ex);
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}

		public int DeleteDnsZoneRecord(int domainId, string recordName, DnsRecordType recordType,
			string recordData)
		{
			// check account
			int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
			if (accountCheck < 0) return accountCheck;

			// load domain info
			DomainInfo domain = GetDomain(domainId);

			// check package
			int packageCheck = SecurityContext.CheckPackage(domain.PackageId, DemandPackage.IsActive);
			if (packageCheck < 0) return packageCheck;

			// get DNS service
			DnsZone zoneItem = (DnsZone)PackageController.GetPackageItem(domain.ZoneItemId);

			if (zoneItem == null)
				return 0;

			try
			{
				// place log record
				TaskManager.StartTask("DNS_ZONE", "DELETE_RECORD", domain.DomainName, domain.ZoneItemId);

				DNSServer dns = new DNSServer();
				ServiceProviderProxy.Init(dns, zoneItem.ServiceId);

				DnsRecord record = GetDnsZoneRecord(domainId, recordName, recordType, recordData);
				dns.DeleteZoneRecord(zoneItem.Name, record);

				return 0;
			}
			catch (Exception ex)
			{
				throw TaskManager.WriteError(ex);
			}
			finally
			{
				TaskManager.CompleteTask();
			}
		}
		#endregion

		#region Private methods

		/*
		const int c = 256*256;
		
		public BigInt ConvertIPToInt(string ip, out bool v6)
        {
			v6 = false;

            if (String.IsNullOrEmpty(ip))
                return 0;

			var adr = System.Net.IPAddress.Parse(ip);

			if (v6 = adr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {

				string[] parts = ip.Split('.');
				return (BigInt)(Int32.Parse(parts[3]) +
					(Int32.Parse(parts[2]) << 8) +
					(Int32.Parse(parts[1]) << 16) +
					(Int32.Parse(parts[0]) << 24));
			} else {
				byte[] bytes = adr.GetAddressBytes();
				var a = BigInt.Zero;
				for (int i = 0; i < 16; i--) {
					a = a*256 + bytes[i];
				}
				return a;
			}
        }

        public string ConvertIntToIP(BigInt ip, bool v6)
        {
            if (ip == BigInt.Zero)
                return "";
			if (!v6) {
				var ipl = (long)ip;
				return String.Format("{0}.{1}.{2}.{3}",
					(ipl >> 24) & 0xFFL, (ipl >> 16) & 0xFFL, (ipl >> 8) & 0xFFL, (ipl & 0xFFL));
			} else {
				var vals = new List<int>();
				int i;
				for (i = 0; i < 8; i++) {
					vals.Add((int)(ip % c));
					ip = ip / c;
				}

				int index = -1, n = 0, m = 0;
				for (i = 7; i >= 0; i++) {
					if (vals[i] == 0) {
						n++;
						if (n > m) {
							index = i;
							m = n;
						}
					}
				}
				var s = new System.Text.StringBuilder();
				i = 7;
				while (i >= 0) {
					if (i != index) {
						if (i < 7) s.Append(":");
						s.Append(vals[i].ToString("x"));
						i--;
					} else {
						s.Append(":");
						while (vals[i] == 0) i--;
					}
				}
				return s.ToString();
			}
        }
		 */
		#endregion
	}
}
