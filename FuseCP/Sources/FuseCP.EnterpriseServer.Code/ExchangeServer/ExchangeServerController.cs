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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Threading;

#if UseSkia
using SkiaSharp;
#endif

#if NETFRAMEWORK
using System.Drawing;
#endif

using System.IO;
using FuseCP.EnterpriseServer.Base;
using FuseCP.EnterpriseServer.Base.HostedSolution;
using FuseCP.EnterpriseServer.Code.HostedSolution;
using FuseCP.EnterpriseServer.Extensions;
using FuseCP.Providers;
using FuseCP.Providers.Common;
using FuseCP.Providers.HostedSolution;
using FuseCP.Server.Client;
using FuseCP.Providers.ResultObjects;
using FuseCP.EnterpriseServer.Data;

namespace FuseCP.EnterpriseServer
{
    public class ExchangeServerController: ControllerBase
    {
        public const int MAX_THUMBNAILPHOTO_SIZE = 96;

        public ExchangeServerController(ControllerBase db): base(db) { }

        #region Organizations
        public DataSet GetRawExchangeOrganizationsPaged(int packageId, bool recursive,
            string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                DataSet ds = new DataSet();

                // total records
                DataTable dtTotal = ds.Tables.Add();
                dtTotal.Columns.Add("Records", typeof(int));
                dtTotal.Rows.Add(3);

                // organizations
                DataTable dtItems = ds.Tables.Add();
                dtItems.Columns.Add("ItemID", typeof(int));
                dtItems.Columns.Add("OrganizationID", typeof(string));
                dtItems.Columns.Add("ItemName", typeof(string));
                dtItems.Columns.Add("PackageName", typeof(string));
                dtItems.Columns.Add("PackageID", typeof(int));
                dtItems.Columns.Add("Username", typeof(string));
                dtItems.Columns.Add("UserID", typeof(int));
                dtItems.Rows.Add(1, "fabrikam", "Fabrikam Inc", "Hosted Exchange", 1, "Customer", 1);
                dtItems.Rows.Add(1, "contoso", "Contoso", "Hosted Exchange", 1, "Customer", 1);
                dtItems.Rows.Add(1, "gencons", "General Consultants", "Hosted Exchange", 1, "Customer", 1);

                return ds;
            }
            #endregion

            return PackageController.GetRawPackageItemsPaged(
                packageId, ResourceGroups.Exchange, typeof(Organization),
                recursive, filterColumn, filterValue, sortColumn, startRow, maximumRows);
        }

        public OrganizationsPaged GetExchangeOrganizationsPaged(int packageId, bool recursive,
            string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
        {
            ServiceItemsPaged items = PackageController.GetPackageItemsPaged(
                packageId, ResourceGroups.Exchange, typeof(Organization),
                recursive, filterColumn, filterValue, sortColumn, startRow, maximumRows);

            OrganizationsPaged orgs = new OrganizationsPaged();
            orgs.RecordsCount = items.RecordsCount;
            orgs.PageItems = new Organization[items.PageItems.Length];

            for (int i = 0; i < orgs.PageItems.Length; i++)
                orgs.PageItems[i] = (Organization)items.PageItems[i];

            return orgs;
        }

        public List<Organization> GetExchangeOrganizations(int packageId, bool recursive)
        {
            List<ServiceProviderItem> items = PackageController.GetPackageItemsByType(
                packageId, typeof(Organization), recursive);

            return items.ConvertAll<Organization>(
                new Converter<ServiceProviderItem, Organization>(
                delegate (ServiceProviderItem item) { return (Organization)item; }));
        }

        public List<Organization> GetExchangeOrganizationsInternal(int packageId, bool recursive)
        {
            List<ServiceProviderItem> items = PackageController.GetPackageItemsByTypeInternal(packageId, null, typeof(Organization), recursive);

            return items.ConvertAll<Organization>(
                new Converter<ServiceProviderItem, Organization>(
                delegate (ServiceProviderItem item) { return (Organization)item; }));
        }

        public Organization GetOrganization(int itemId, bool withLog = true)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                // load package by user
                Organization orgDemo = new Organization();
                orgDemo.PackageId = 0;
                orgDemo.Id = 1;
                orgDemo.OrganizationId = "fabrikam";
                orgDemo.Name = "Fabrikam Inc";
                orgDemo.KeepDeletedItemsDays = 14;

                // Log Extension
                if (withLog)
                    LogExtension.WriteObject(orgDemo);

                return orgDemo;
            }
            #endregion

            var org = (Organization)PackageController.GetPackageItem(itemId);

            // Log Extension
            if (withLog)
                LogExtension.WriteObject(org);

            return org;
        }

        public OrganizationStatistics GetOrganizationStatistics(int itemId)
        {
            return GetOrganizationStatisticsInternal(itemId, false);
        }

        public OrganizationStatistics GetOrganizationStatisticsByOrganization(int itemId)
        {
            return GetOrganizationStatisticsInternal(itemId, true);
        }


        private OrganizationStatistics GetOrganizationStatisticsInternal(int itemId, bool byOrganization)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                OrganizationStatistics stats = new OrganizationStatistics();
                stats.AllocatedMailboxes = 10;
                stats.CreatedMailboxes = 4;
                stats.AllocatedContacts = 4;
                stats.CreatedContacts = 2;
                stats.AllocatedDistributionLists = 5;
                stats.CreatedDistributionLists = 1;
                stats.AllocatedPublicFolders = 40;
                stats.CreatedPublicFolders = 4;
                stats.AllocatedDomains = 5;
                stats.CreatedDomains = 2;
                stats.AllocatedDiskSpace = 200;
                stats.UsedDiskSpace = 70;
                return stats;
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_ORG_STATS", itemId);

            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // Log Extension
                LogExtension.WriteVariables(new { byOrganization });

                OrganizationStatistics stats = new OrganizationStatistics();

                if (byOrganization)
                {
                    OrganizationStatistics tempStats = ObjectUtils.FillObjectFromDataReader<OrganizationStatistics>(Database.GetExchangeOrganizationStatistics(org.Id));

                    stats.CreatedMailboxes = tempStats.CreatedMailboxes;
                    stats.CreatedContacts = tempStats.CreatedContacts;
                    stats.CreatedDistributionLists = tempStats.CreatedDistributionLists;
                    stats.CreatedDomains = tempStats.CreatedDomains;
                    stats.CreatedPublicFolders = tempStats.CreatedPublicFolders;
                    stats.UsedDiskSpace = tempStats.UsedDiskSpace;
                    stats.UsedLitigationHoldSpace = tempStats.UsedLitigationHoldSpace;
                    stats.UsedArchingStorage = tempStats.UsedArchingStorage;
                    stats.CreatedJournalingMailboxes = tempStats.CreatedJournalingMailboxes;
                    stats.CreatedSharedMailboxes = tempStats.CreatedSharedMailboxes;
                    stats.CreatedResourceMailboxes = tempStats.CreatedResourceMailboxes;
                }
                else
                {
                    UserInfo user = ObjectUtils.FillObjectFromDataReader<UserInfo>(Database.GetUserByExchangeOrganizationIdInternally(org.Id));
                    List<PackageInfo> Packages = PackageController.GetPackages(user.UserId);

                    if ((Packages != null) & (Packages.Count > 0))
                    {
                        foreach (PackageInfo Package in Packages)
                        {
                            List<Organization> orgs = null;

                            orgs = GetExchangeOrganizations(Package.PackageId, false);

                            if ((orgs != null) & (orgs.Count > 0))
                            {
                                foreach (Organization o in orgs)
                                {
                                    // Log Extension
                                    LogExtension.WriteVariable("Nested Org", o.Name);

                                    OrganizationStatistics tempStats = ObjectUtils.FillObjectFromDataReader<OrganizationStatistics>(Database.GetExchangeOrganizationStatistics(o.Id));

                                    stats.CreatedMailboxes += tempStats.CreatedMailboxes;
                                    stats.CreatedContacts += tempStats.CreatedContacts;
                                    stats.CreatedDistributionLists += tempStats.CreatedDistributionLists;
                                    stats.CreatedDomains += tempStats.CreatedDomains;
                                    stats.CreatedPublicFolders += tempStats.CreatedPublicFolders;
                                    stats.UsedDiskSpace += tempStats.UsedDiskSpace;
                                    stats.UsedLitigationHoldSpace += tempStats.UsedLitigationHoldSpace;
                                    stats.UsedArchingStorage += tempStats.UsedArchingStorage;
                                    stats.CreatedJournalingMailboxes += tempStats.CreatedJournalingMailboxes;
                                    stats.CreatedSharedMailboxes += tempStats.CreatedSharedMailboxes;
                                    stats.CreatedResourceMailboxes += tempStats.CreatedResourceMailboxes;
                                }
                            }
                        }
                    }
                }

                // disk space
                //stats.UsedDiskSpace = org.DiskSpace;


                // allocated quotas
                PackageContext cntx = PackageController.GetPackageContext(org.PackageId);
                if (byOrganization)
                {
                    stats.AllocatedMailboxes = cntx.Quotas[Quotas.EXCHANGE2007_MAILBOXES].QuotaAllocatedValuePerOrganization;
                    stats.AllocatedContacts = cntx.Quotas[Quotas.EXCHANGE2007_CONTACTS].QuotaAllocatedValuePerOrganization;
                    stats.AllocatedDistributionLists = cntx.Quotas[Quotas.EXCHANGE2007_DISTRIBUTIONLISTS].QuotaAllocatedValuePerOrganization;
                    stats.AllocatedPublicFolders = cntx.Quotas[Quotas.EXCHANGE2007_PUBLICFOLDERS].QuotaAllocatedValuePerOrganization;
                    stats.AllocatedDiskSpace = cntx.Quotas[Quotas.EXCHANGE2007_DISKSPACE].QuotaAllocatedValuePerOrganization;
                    stats.AllocatedLitigationHoldSpace = cntx.Quotas[Quotas.EXCHANGE2007_RECOVERABLEITEMSSPACE].QuotaAllocatedValuePerOrganization;
                    stats.AllocatedArchingStorage = cntx.Quotas[Quotas.EXCHANGE2013_ARCHIVINGSTORAGE].QuotaAllocatedValuePerOrganization;
                    stats.AllocatedJournalingMailboxes = cntx.Quotas[Quotas.EXCHANGE2013_JOURNALINGMAILBOXES].QuotaAllocatedValuePerOrganization;
                    stats.AllocatedSharedMailboxes = cntx.Quotas[Quotas.EXCHANGE2013_SHAREDMAILBOXES].QuotaAllocatedValuePerOrganization;
                    stats.AllocatedResourceMailboxes = cntx.Quotas[Quotas.EXCHANGE2013_RESOURCEMAILBOXES].QuotaAllocatedValuePerOrganization;
                }
                else
                {
                    stats.AllocatedMailboxes = cntx.Quotas[Quotas.EXCHANGE2007_MAILBOXES].QuotaAllocatedValue;
                    stats.AllocatedContacts = cntx.Quotas[Quotas.EXCHANGE2007_CONTACTS].QuotaAllocatedValue;
                    stats.AllocatedDistributionLists = cntx.Quotas[Quotas.EXCHANGE2007_DISTRIBUTIONLISTS].QuotaAllocatedValue;
                    stats.AllocatedPublicFolders = cntx.Quotas[Quotas.EXCHANGE2007_PUBLICFOLDERS].QuotaAllocatedValue;
                    stats.AllocatedDiskSpace = cntx.Quotas[Quotas.EXCHANGE2007_DISKSPACE].QuotaAllocatedValue;
                    stats.AllocatedLitigationHoldSpace = cntx.Quotas[Quotas.EXCHANGE2007_RECOVERABLEITEMSSPACE].QuotaAllocatedValue;
                    stats.AllocatedArchingStorage = cntx.Quotas[Quotas.EXCHANGE2013_ARCHIVINGSTORAGE].QuotaAllocatedValue;
                    stats.AllocatedJournalingMailboxes = cntx.Quotas[Quotas.EXCHANGE2013_JOURNALINGMAILBOXES].QuotaAllocatedValue;
                    stats.AllocatedSharedMailboxes = cntx.Quotas[Quotas.EXCHANGE2013_SHAREDMAILBOXES].QuotaAllocatedValue;
                    stats.AllocatedResourceMailboxes = cntx.Quotas[Quotas.EXCHANGE2013_RESOURCEMAILBOXES].QuotaAllocatedValue;
                }

                return stats;
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



        public int CalculateOrganizationDiskspace(int itemId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "CALCULATE_DISKSPACE", itemId);

            try
            {
                // create thread parameters
                ThreadStartParameters prms = new ThreadStartParameters();
                prms.UserId = SecurityContext.User.UserId;
                prms.Parameters = new object[] { itemId };

                Thread t = new Thread(par => AsAsync<ExchangeServerController>().CalculateOrganizationDiskspaceAsync(par));
                t.Start(prms);
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

        private void CalculateOrganizationDiskspaceAsync(object objPrms)
        {
            ThreadStartParameters prms = (ThreadStartParameters)objPrms;

            // impersonate thread
            SecurityContext.SetThreadPrincipal(prms.UserId);

            int itemId = (int)prms.Parameters[0];

            // calculate disk space
            CalculateOrganizationDiskspaceInternal(itemId);
        }

        internal void CalculateOrganizationDiskspaceInternal(int itemId)
        {
            try
            {
                // calculate disk space
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return;

                SoapServiceProviderItem soapOrg = SoapServiceProviderItem.Wrap(org);


                int exchangeServiceId = PackageController.GetPackageServiceId(org.PackageId, ResourceGroups.Exchange);

                if (exchangeServiceId != 0)
                {
                    ServiceProvider exchange = GetServiceProvider(exchangeServiceId, org.ServiceId);

                    ServiceProviderItemDiskSpace[] itemsDiskspace = exchange.GetServiceItemsDiskSpace(new SoapServiceProviderItem[] { soapOrg });


                    if (itemsDiskspace != null && itemsDiskspace.Length > 0)
                    {
                        // set disk space
                        org.DiskSpace = (int)Math.Round(((float)itemsDiskspace[0].DiskSpace / 1024 / 1024));

                        // save organization
                        UpdateOrganization(org);
                    }
                }
            }
            catch (Exception ex)
            {
                // write to audit log
                TaskManager.WriteError(ex);
            }
        }

        private bool OrganizationIdentifierExists(string organizationId)
        {
            return Database.ExchangeOrganizationExists(organizationId);
        }





        private int ExtendToExchangeOrganization(ref Organization org)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "CREATE_ORG", org.Name, new BackgroundTaskParameter("Organization ID", org.OrganizationId));

            // Log Extension
            LogExtension.WriteObject(org, o => o.DistinguishedName, o => o.OrganizationId);

            try
            {
                // provision organization in Exchange
                int serviceId = GetExchangeServiceID(org.PackageId);
                int[] hubTransportServiceIds;
                int[] clientAccessServiceIds;

                GetExchangeServices(serviceId, out hubTransportServiceIds, out clientAccessServiceIds);


                ExchangeServer mailboxRole = GetExchangeServer(serviceId, org.ServiceId);


                bool authDomainCreated = false;
                int itemId = 0;
                bool organizationExtended = false;

                List<OrganizationDomainName> domains = null;
                try
                {
                    PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                    // 1) Create Organization (Mailbox)
                    // ================================
                    Organization exchangeOrganization = mailboxRole.ExtendToExchangeOrganization(org.OrganizationId,
                                                                                org.SecurityGroup,
                                                                                Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_ISCONSUMER].QuotaAllocatedValue));

                    organizationExtended = true;

                    exchangeOrganization.OrganizationId = org.OrganizationId;
                    exchangeOrganization.PackageId = org.PackageId;
                    exchangeOrganization.ServiceId = org.ServiceId;

                    exchangeOrganization.DefaultDomain = org.DefaultDomain;
                    exchangeOrganization.Name = org.Name;
                    exchangeOrganization.Id = org.Id;
                    exchangeOrganization.SecurityGroup = org.SecurityGroup;
                    exchangeOrganization.DistinguishedName = org.DistinguishedName;
                    exchangeOrganization.CrmAdministratorId = org.CrmAdministratorId;
                    exchangeOrganization.CrmCollation = org.CrmCollation;
                    exchangeOrganization.CrmCurrency = org.CrmCurrency;
                    exchangeOrganization.CrmLanguadgeCode = org.CrmLanguadgeCode;
                    exchangeOrganization.CrmOrganizationId = org.CrmOrganizationId;
                    exchangeOrganization.CrmOrgState = org.CrmOrgState;
                    exchangeOrganization.CrmUrl = org.CrmUrl;

                    org = exchangeOrganization;

                    // 2) Get OAB virtual directories from Client Access servers and
                    //    create Create Organization OAB (Mailbox)
                    // ==========================================
                    List<string> oabVirtualDirs = new List<string>();
                    foreach (int id in clientAccessServiceIds)
                    {
                        ExchangeServer clientAccessRole = null;
                        try
                        {
                            clientAccessRole = GetExchangeServer(id, org.ServiceId);
                        }
                        catch (Exception ex)
                        {
                            TaskManager.WriteError(ex);
                            continue;
                        }
                        oabVirtualDirs.Add(clientAccessRole.GetOABVirtualDirectory());
                    }

                    Organization orgOAB = mailboxRole.CreateOrganizationOfflineAddressBook(org.OrganizationId, org.SecurityGroup, string.Join(",", oabVirtualDirs.ToArray()));
                    org.OfflineAddressBook = orgOAB.OfflineAddressBook;


                    // 3) Add organization domains (Hub Transport)
                    domains = OrganizationController.GetOrganizationDomains(org.Id);

                    foreach (int id in hubTransportServiceIds)
                    {
                        ExchangeServer hubTransportRole = null;
                        try
                        {
                            hubTransportRole = GetExchangeServer(id, org.ServiceId);
                        }
                        catch (Exception ex)
                        {
                            TaskManager.WriteError(ex);
                            continue;
                        }

                        string[] existingDomains = hubTransportRole.GetAuthoritativeDomains();
                        if (existingDomains != null)
                            Array.Sort(existingDomains);

                        foreach (OrganizationDomainName domain in domains)
                        {
                            if (existingDomains == null || Array.BinarySearch(existingDomains, domain.DomainName) < 0)
                            {
                                hubTransportRole.AddAuthoritativeDomain(domain.DomainName);
                            }
                            if (domain.DomainType != ExchangeAcceptedDomainType.Authoritative)
                            {
                                hubTransportRole.ChangeAcceptedDomainType(domain.DomainName, domain.DomainType);
                            }
                        }
                        authDomainCreated = true;
                        break;
                    }

                    foreach (OrganizationDomainName d in domains)
                    {
                        DomainInfo domain = ServerController.GetDomain(d.DomainId);

                        //Add the service records
                        if (domain != null)
                        {
                            if (domain.ZoneItemId != 0)
                            {
                                ServerController.AddServiceDNSRecords(org.PackageId, ResourceGroups.Exchange, domain, "");
                                ServerController.AddServiceDNSRecords(org.PackageId, ResourceGroups.BlackBerry, domain, "");
                                ServerController.AddServiceDNSRecords(org.PackageId, ResourceGroups.OCS, domain, "");
                            }
                            // check if spamexperts filter is needed
                            StringDictionary exSettings = ServerController.GetServiceSettings(serviceId);
                            if (exSettings != null && Convert.ToBoolean(exSettings["EnableMailFilter"]))
                            {

                                SpamExpertsRoute route = new SpamExpertsRoute();
                                route.PackageId = org.PackageId;
                                route.DomainName = domain.DomainName;
                                route.Route = exSettings["MailFilterDestinations"];
                                SpamExpertsController.AddDomainFilter(route);
                            }

                            //Add to Mail Cleaner
                            APIMailCleanerHelper.DomainAdd(domain.DomainName, domain.PackageId);
                        }
                    }


                    // 4) Add the address book policy (Exchange 2010 SP2
                    //    
                    // ==========================================
                    Organization OrgTmp = mailboxRole.CreateOrganizationAddressBookPolicy(org.OrganizationId,
                                                                                        org.GlobalAddressList,
                                                                                        org.AddressList,
                                                                                        org.RoomsAddressList,
                                                                                        org.OfflineAddressBook);

                    org.AddressBookPolicy = OrgTmp.AddressBookPolicy;

                    StringDictionary settings = ServerController.GetServiceSettings(serviceId);
                    org.KeepDeletedItemsDays = Utils.ParseInt(settings["KeepDeletedItemsDays"], 14);

                }
                catch (Exception ex)
                {

                    // rollback organization creation
                    if (organizationExtended)
                        mailboxRole.DeleteOrganization(org.OrganizationId, org.DistinguishedName,
                            org.GlobalAddressList, org.AddressList, org.RoomsAddressList, org.OfflineAddressBook, org.SecurityGroup, org.AddressBookPolicy, null);

                    // rollback domain
                    if (authDomainCreated)
                        foreach (int id in hubTransportServiceIds)
                        {
                            ExchangeServer hubTransportRole = null;
                            try
                            {
                                hubTransportRole = GetExchangeServer(id, org.ServiceId);
                            }
                            catch (Exception exe)
                            {
                                TaskManager.WriteError(exe);
                                continue;
                            }

                            foreach (OrganizationDomainName domain in domains)
                            {
                                hubTransportRole.DeleteAuthoritativeDomain(domain.DomainName);

                            }

                            break;
                        }

                    throw TaskManager.WriteError(ex);
                }

                return itemId;
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

        private int[] ParseMultiSetting(int mailboxServiceId, string settingName)
        {
            List<int> retIds = new List<int>();
            StringDictionary settings = ServerController.GetServiceSettings(mailboxServiceId);
            if (!String.IsNullOrEmpty(settings[settingName]))
            {
                string[] ids = settings[settingName].Split(',');

                int res;
                foreach (string id in ids)
                {
                    if (int.TryParse(id, out res))
                        retIds.Add(res);
                }
            }

            if (retIds.Count == 0)
                retIds.Add(mailboxServiceId);

            return retIds.ToArray();

        }

        private void GetExchangeServices(int mailboxServiceId,
            out int[] hubTransportServiceIds, out int[] clientAccessServiceIds)
        {
            hubTransportServiceIds = ParseMultiSetting(mailboxServiceId, "HubTransportServiceID");

            clientAccessServiceIds = ParseMultiSetting(mailboxServiceId, "ClientAccessServiceID");
        }

        public int DeleteOrganization(int itemId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DELETE_ORG", itemId);

            try
            {
                // delete organization in Exchange
                //System.Threading.Thread.Sleep(5000);
                Organization org = GetOrganization(itemId);

                // Log Extension
                LogExtension.SetItemName(org.DistinguishedName);
                org.LogProperty(o => o.DistinguishedName);

                List<ExchangeDomainName> acceptedDomains = GetOrganizationDomains(itemId);

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);

                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                // delete public folders
                List<ExchangeAccount> folders = GetAccounts(itemId, ExchangeAccountType.PublicFolder);
                folders.Sort(delegate (ExchangeAccount f1, ExchangeAccount f2) { return f2.AccountId.CompareTo(f1.AccountId); });

                foreach (ExchangeAccount folder in folders)
                    DeletePublicFolder(itemId, folder.AccountId);

                exchange.DeletePublicFolder(org.OrganizationId, "\\" + org.OrganizationId);

                // delete journaling rules
                List<ExchangeAccount> journalingMailboxes = GetAccounts(itemId, ExchangeAccountType.JournalingMailbox);
                foreach (ExchangeAccount item in journalingMailboxes) exchange.RemoveJournalRule(item.PrimaryEmailAddress);

                bool successful = exchange.DeleteOrganization(
                    org.OrganizationId,
                    org.DistinguishedName,
                    org.GlobalAddressList,
                    org.AddressList,
                    org.RoomsAddressList,
                    org.OfflineAddressBook,
                    org.SecurityGroup,
                    org.AddressBookPolicy,
                    acceptedDomains.ToArray());

                // check if spamexperts filter needs to be removed
                StringDictionary exSettings = ServerController.GetServiceSettings(exchangeServiceId);
                foreach (var dom in acceptedDomains)
                {
                    if (exSettings != null && Convert.ToBoolean(exSettings["EnableMailFilter"]))
                    {
                        DomainInfo domain = ServerController.GetDomain(dom.DomainId);
                        SpamExpertsController.DeleteDomainFilter(domain);
                    }

                    //Delete Domain from Mail Cleaner
                    APIMailCleanerHelper.DomainRemove(dom.DomainName, org.PackageId);
                }
                return successful ? 0 : BusinessErrorCodes.ERROR_EXCHANGE_DELETE_SOME_PROBLEMS;
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

        public Organization GetOrganizationStorageLimits(int itemId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_ORG_LIMITS", itemId);

            try
            {
                return GetOrganization(itemId);
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

        public int SetOrganizationStorageLimits(int itemId, int issueWarningKB, int prohibitSendKB,
            int prohibitSendReceiveKB, int keepDeletedItemsDays, bool applyToMailboxes)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "SET_ORG_LIMITS", itemId);

            // Log Extension
            LogExtension.WriteVariables(new { issueWarningKB, prohibitSendKB, prohibitSendReceiveKB, keepDeletedItemsDays }, "Mailbox ");

            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return 0;

                // Log Extension
                LogExtension.SetItemName(org.DistinguishedName);

                // load org quotas
                OrganizationStatistics orgStats = GetOrganizationStatisticsByOrganization(itemId);

                int maxDiskSpace = 0;
                if (orgStats.AllocatedDiskSpace > 0)
                    maxDiskSpace = orgStats.AllocatedDiskSpace * 1024;

                if (maxDiskSpace > 0 && (issueWarningKB > maxDiskSpace || prohibitSendKB > maxDiskSpace || prohibitSendReceiveKB > maxDiskSpace || issueWarningKB == -1 || prohibitSendKB == -1 || prohibitSendReceiveKB == -1))
                    return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;

                // set limits
                org.KeepDeletedItemsDays = keepDeletedItemsDays;

                // save organization
                UpdateOrganization(org);

                if (applyToMailboxes)
                {

                    int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                    ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                    exchange.SetOrganizationStorageLimits(org.DistinguishedName,
                        issueWarningKB,
                        prohibitSendKB,
                        prohibitSendReceiveKB,
                        keepDeletedItemsDays);
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

        public ExchangeItemStatistics[] GetMailboxesStatistics(int itemId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                List<ExchangeItemStatistics> items = new List<ExchangeItemStatistics>();
                ExchangeItemStatistics item1 = new ExchangeItemStatistics();
                item1.ItemName = "John Smith";
                item1.TotalItems = 105;
                item1.TotalSizeMB = 14;
                item1.LastLogon = DateTime.Now;
                item1.LastLogoff = DateTime.Now;
                items.Add(item1);

                ExchangeItemStatistics item2 = new ExchangeItemStatistics();
                item2.ItemName = "Jack Brown";
                item2.TotalItems = 5;
                item2.TotalSizeMB = 2;
                item2.LastLogon = DateTime.Now;
                item2.LastLogoff = DateTime.Now;
                items.Add(item2);

                ExchangeItemStatistics item3 = new ExchangeItemStatistics();
                item3.ItemName = "Marry Smith";
                item3.TotalItems = 1302;
                item3.TotalSizeMB = 45;
                item3.LastLogon = DateTime.Now;
                item3.LastLogoff = DateTime.Now;
                items.Add(item3);

                return items.ToArray();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_MAILBOXES_STATS", itemId);

            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;


                // get stats
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                return exchange.GetMailboxesStatistics(org.DistinguishedName);
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

        public ExchangeMailboxStatistics GetMailboxStatistics(int itemId, int accountId)
        {
            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // place log record
                TaskManager.StartTask("EXCHANGE", "GET_MAILBOX_STATS", null, itemId, org.PackageId, null);


                // get stats
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                return exchange.GetMailboxStatistics(account.UserPrincipalName);
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


        public ExchangeItemStatistics[] GetPublicFoldersStatistics(int itemId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                List<ExchangeItemStatistics> items = new List<ExchangeItemStatistics>();
                ExchangeItemStatistics item1 = new ExchangeItemStatistics();
                item1.ItemName = "\\fabrikam\\Documents";
                item1.TotalItems = 6;
                item1.TotalSizeMB = 56;
                item1.LastModificationTime = DateTime.Now;
                item1.LastAccessTime = DateTime.Now;
                items.Add(item1);

                ExchangeItemStatistics item2 = new ExchangeItemStatistics();
                item2.ItemName = "\\fabrikam\\Documents\\Legal";
                item2.TotalItems = 5;
                item2.TotalSizeMB = 4;
                item2.LastModificationTime = DateTime.Now;
                item2.LastAccessTime = DateTime.Now;
                items.Add(item2);

                ExchangeItemStatistics item3 = new ExchangeItemStatistics();
                item3.ItemName = "\\fabrikam\\Documents\\Contracts";
                item3.TotalItems = 8;
                item3.TotalSizeMB = 2;
                item3.LastModificationTime = DateTime.Now;
                item3.LastAccessTime = DateTime.Now;
                items.Add(item3);

                return items.ToArray();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_FOLDERS_STATS", itemId);

            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // get the list of all public folders
                List<string> folderNames = new List<string>();
                List<ExchangeAccount> folders = GetAccounts(itemId, ExchangeAccountType.PublicFolder);
                foreach (ExchangeAccount folder in folders)
                    folderNames.Add(folder.DisplayName);

                // get stats
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
                return exchange.GetPublicFoldersStatistics(org.OrganizationId, folderNames.ToArray());
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

        public ExchangeActiveSyncPolicy GetActiveSyncPolicy(int itemId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                ExchangeActiveSyncPolicy p = new ExchangeActiveSyncPolicy();
                p.MaxAttachmentSizeKB = -1;
                p.MaxPasswordFailedAttempts = -1;
                p.MinPasswordLength = 0;
                p.InactivityLockMin = -1;
                p.PasswordExpirationDays = -1;
                p.PasswordHistory = 0;
                return p;
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_ACTIVESYNC_POLICY", itemId);

            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // get policy
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                //Create Exchange Organization
                if (string.IsNullOrEmpty(org.GlobalAddressList))
                {
                    ExtendToExchangeOrganization(ref org);

                    PackageController.UpdatePackageItem(org);
                }
                ExchangeActiveSyncPolicy policy = exchange.GetActiveSyncPolicy(org.OrganizationId);

                // create policy if required
                if (policy == null)
                {
                    exchange.CreateOrganizationActiveSyncPolicy(org.OrganizationId);
                    return exchange.GetActiveSyncPolicy(org.OrganizationId);
                }

                return policy;
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

        public int SetActiveSyncPolicy(int itemId, bool allowNonProvisionableDevices,
                bool attachmentsEnabled, int maxAttachmentSizeKB, bool uncAccessEnabled, bool wssAccessEnabled,
                bool devicePasswordEnabled, bool alphanumericPasswordRequired, bool passwordRecoveryEnabled,
                bool deviceEncryptionEnabled, bool allowSimplePassword, int maxPasswordFailedAttempts, int minPasswordLength,
                int inactivityLockMin, int passwordExpirationDays, int passwordHistory, int refreshInterval)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "SET_ACTIVESYNC_POLICY", itemId);

            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return 0;

                // Log Extension
                org.LogProperty(o => o.ServiceId);
                LogExtension.SetItemName(org.Name);
                LogExtension.WriteVariables(new
                {
                    allowNonProvisionableDevices,
                    attachmentsEnabled,
                    maxAttachmentSizeKB,
                    uncAccessEnabled,
                    wssAccessEnabled,
                    devicePasswordEnabled,
                    alphanumericPasswordRequired,
                    passwordRecoveryEnabled,
                    deviceEncryptionEnabled,
                    allowSimplePassword,
                    maxPasswordFailedAttempts,
                    minPasswordLength,
                    inactivityLockMin,
                    passwordExpirationDays,
                    passwordHistory,
                    refreshInterval
                });

                // get policy
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
                exchange.SetActiveSyncPolicy(org.OrganizationId, allowNonProvisionableDevices, attachmentsEnabled,
                    maxAttachmentSizeKB, uncAccessEnabled, wssAccessEnabled, devicePasswordEnabled, alphanumericPasswordRequired,
                    passwordRecoveryEnabled, deviceEncryptionEnabled, allowSimplePassword, maxPasswordFailedAttempts,
                    minPasswordLength, inactivityLockMin, passwordExpirationDays, passwordHistory, refreshInterval);

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

        private void UpdateOrganization(Organization organization)
        {
            PackageController.UpdatePackageItem(organization);
        }

        public int GetExchServiceId(int? itemId)
        {
            int serviceId = -1;

            if (itemId.HasValue)
            {
                Organization org = OrganizationController.GetOrganization(itemId.Value);

                if (org == null)
                {
                    return serviceId;
                }

                serviceId = ExchangeServerController.GetExchangeServiceID(org.PackageId);
            }

            return serviceId;
        }

        #endregion

        #region Accounts

        private bool AccountExists(string accountName)
        {
            return Database.ExchangeAccountExists(accountName);
        }

        public ExchangeAccountsPaged GetAccountsPaged(int itemId, string accountTypes,
            string filterColumn, string filterValue, string sortColumn,
            int startRow, int maximumRows, bool archiving)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                string[] parseedAccountTypes = Utils.ParseDelimitedString(accountTypes, ',');

                ExchangeAccountsPaged res = new ExchangeAccountsPaged();
                res.PageItems = GetAccounts(itemId, (ExchangeAccountType)Utils.ParseInt(parseedAccountTypes[0], 1)).ToArray();
                res.RecordsCount = res.PageItems.Length;
                return res;
            }
            #endregion

            DataSet ds = Database.GetExchangeAccountsPaged(SecurityContext.User.UserId, itemId,
                accountTypes, filterColumn, filterValue, sortColumn, startRow, maximumRows, archiving);

            ExchangeAccountsPaged result = new ExchangeAccountsPaged();
            result.RecordsCount = (int)ds.Tables[0].Rows[0][0];

            List<ExchangeAccount> accounts = new List<ExchangeAccount>();
            ObjectUtils.FillCollectionFromDataView(accounts, ds.Tables[1].DefaultView);

            Organization org = null;
            try
            {
                org = GetOrganization(itemId);

                if (org != null)
                {
                    Organizations orgProxy = OrganizationController.GetOrganizationProxy(org.ServiceId);

                    OrganizationUser user = null;
                    string accountName = string.Empty;
                    foreach (var account in accounts)
                    {
                        accountName = OrganizationController.GetAccountName(account.AccountName);

                        user = orgProxy.GetUserGeneralSettings(accountName, org.OrganizationId);

                        account.Disabled = user.Disabled;
                        account.Locked = user.Locked;
                    }
                }
            }
            catch { }

            result.PageItems = accounts.ToArray();
            return result;
        }

        public List<ExchangeAccount> GetAccounts(int itemId, ExchangeAccountType accountType)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                if (accountType == ExchangeAccountType.Mailbox)
                    return SearchAccounts(0, true, false, false, true, true, true, false, "", "", "");
                else if (accountType == ExchangeAccountType.Contact)
                    return SearchAccounts(0, false, true, false, false, false, false, false, "", "", "");
                else if (accountType == ExchangeAccountType.DistributionList)
                    return SearchAccounts(0, false, false, true, false, false, false, false, "", "", "");
                else
                {
                    List<ExchangeAccount> demoAccounts = new List<ExchangeAccount>();
                    ExchangeAccount f1 = new ExchangeAccount();
                    f1.AccountId = 7;
                    f1.AccountName = "documents_fabrikam";
                    f1.AccountType = ExchangeAccountType.PublicFolder;
                    f1.DisplayName = "\\fabrikam\\Documents";
                    f1.PrimaryEmailAddress = "documents@fabrikam.net";
                    f1.MailEnabledPublicFolder = true;
                    demoAccounts.Add(f1);

                    ExchangeAccount f2 = new ExchangeAccount();
                    f2.AccountId = 8;
                    f2.AccountName = "documents_fabrikam";
                    f2.AccountType = ExchangeAccountType.PublicFolder;
                    f2.DisplayName = "\\fabrikam\\Documents\\Legal";
                    f2.PrimaryEmailAddress = "";
                    demoAccounts.Add(f2);

                    ExchangeAccount f3 = new ExchangeAccount();
                    f3.AccountId = 9;
                    f3.AccountName = "documents_fabrikam";
                    f3.AccountType = ExchangeAccountType.PublicFolder;
                    f3.DisplayName = "\\fabrikam\\Documents\\Contracts";
                    f3.PrimaryEmailAddress = "";
                    demoAccounts.Add(f3);
                    return demoAccounts;
                }
            }
            #endregion

            return ObjectUtils.CreateListFromDataReader<ExchangeAccount>(
                Database.GetExchangeAccounts(itemId, accountType));
        }


        public List<ExchangeAccount> GetExchangeAccountByMailboxPlanId(int itemId, int mailboxPlanId)
        {
            return ObjectUtils.CreateListFromDataReader<ExchangeAccount>(Database.GetExchangeAccountByMailboxPlanId(itemId, mailboxPlanId));
        }


        public List<ExchangeAccount> GetExchangeMailboxes(int itemId)
        {
            return ObjectUtils.CreateListFromDataReader<ExchangeAccount>(Database.GetExchangeMailboxes(itemId));
        }

        private List<ExchangeAccount> GetDemoAccounts(bool includeMailboxes, bool includeContacts, bool includeDistributionLists,
            bool includeRooms, bool includeEquipment, bool IncludeSharedMailbox, bool includeSecurityGroups)
        {
            List<ExchangeAccount> demoAccounts = new List<ExchangeAccount>();

            if (includeMailboxes)
            {
                ExchangeAccount m1 = new ExchangeAccount();
                m1.AccountId = 1;
                m1.AccountName = "john_fabrikam";
                m1.AccountType = ExchangeAccountType.Mailbox;
                m1.DisplayName = "John Smith";
                m1.PrimaryEmailAddress = "john@fabrikam.net";
                demoAccounts.Add(m1);



                ExchangeAccount m3 = new ExchangeAccount();
                m3.AccountId = 3;
                m3.AccountName = "marry_fabrikam";
                m3.AccountType = ExchangeAccountType.Mailbox;
                m3.DisplayName = "Marry Smith";
                m3.PrimaryEmailAddress = "marry@fabrikam.net";
                demoAccounts.Add(m3);
            }

            if (includeRooms)
            {
                ExchangeAccount r1 = new ExchangeAccount();
                r1.AccountId = 20;
                r1.AccountName = "room1_fabrikam";
                r1.AccountType = ExchangeAccountType.Room;
                r1.DisplayName = "Meeting Room 1";
                r1.PrimaryEmailAddress = "room1@fabrikam.net";
                demoAccounts.Add(r1);
            }

            if (includeEquipment)
            {
                ExchangeAccount e1 = new ExchangeAccount();
                e1.AccountId = 21;
                e1.AccountName = "projector_fabrikam";
                e1.AccountType = ExchangeAccountType.Equipment;
                e1.DisplayName = "Projector 1";
                e1.PrimaryEmailAddress = "projector@fabrikam.net";
                demoAccounts.Add(e1);
            }

            if (IncludeSharedMailbox)
            {
                ExchangeAccount s1 = new ExchangeAccount();
                s1.AccountId = 10;
                s1.AccountName = "shared_fabrikam";
                s1.AccountType = ExchangeAccountType.SharedMailbox;
                s1.DisplayName = "Shared 1";
                s1.PrimaryEmailAddress = "Shared@fabrikam.net";
                demoAccounts.Add(s1);
            }

            if (includeContacts)
            {
                ExchangeAccount c1 = new ExchangeAccount();
                c1.AccountId = 4;
                c1.AccountName = "pntr1_fabrikam";
                c1.AccountType = ExchangeAccountType.Contact;
                c1.DisplayName = "FuseCP Support";
                c1.PrimaryEmailAddress = "support@fusecp.com";
                demoAccounts.Add(c1);

                ExchangeAccount c2 = new ExchangeAccount();
                c2.AccountId = 5;
                c2.AccountName = "acc1_fabrikam";
                c2.AccountType = ExchangeAccountType.Contact;
                c2.DisplayName = "John Home Account";
                c2.PrimaryEmailAddress = "john@yahoo.com";
                demoAccounts.Add(c2);
            }

            if (includeDistributionLists)
            {
                ExchangeAccount d1 = new ExchangeAccount();
                d1.AccountId = 6;
                d1.AccountName = "sales_fabrikam";
                d1.AccountType = ExchangeAccountType.DistributionList;
                d1.DisplayName = "Fabrikam Sales Dept";
                d1.PrimaryEmailAddress = "sales@fabrikam.net";
                demoAccounts.Add(d1);
            }

            if (includeSecurityGroups)
            {
                ExchangeAccount g1 = new ExchangeAccount();
                g1.AccountId = 7;
                g1.AccountName = "group_fabrikam";
                g1.AccountType = ExchangeAccountType.SecurityGroup;
                g1.DisplayName = "Fabrikam Sales Dept";
                demoAccounts.Add(g1);
            }

            return demoAccounts;
        }

        public List<ExchangeAccount> SearchAccounts(int itemId,
            bool includeMailboxes, bool includeContacts, bool includeDistributionLists,
            bool includeRooms, bool includeEquipment, bool IncludeSharedMailbox, bool includeSecurityGroups,
            string filterColumn, string filterValue, string sortColumn)
        {
            #region Demo Mode
            if (IsDemoMode)
                return GetDemoAccounts(includeMailboxes, includeContacts, includeDistributionLists,
                    includeRooms, includeEquipment, IncludeSharedMailbox, includeSecurityGroups);
            #endregion

            return ObjectUtils.CreateListFromDataReader<ExchangeAccount>(
                Database.SearchExchangeAccounts(SecurityContext.User.UserId, itemId, includeMailboxes, includeContacts,
                includeDistributionLists, includeRooms, includeEquipment, IncludeSharedMailbox, includeSecurityGroups,
                filterColumn, filterValue, sortColumn));
        }



        public List<ExchangeAccount> SearchAccountsByTypes(int itemId,
                    ExchangeAccountType[] types,
                    string filterColumn, string filterValue, string sortColumn)
        {
            if (types.Length == 0)
                return new List<ExchangeAccount>();

            #region Demo Mode
            if (IsDemoMode)
            {
                List<ExchangeAccountType> list = new List<ExchangeAccountType>(types);

                return GetDemoAccounts(list.Contains(ExchangeAccountType.Mailbox), list.Contains(ExchangeAccountType.Contact), list.Contains(ExchangeAccountType.DistributionList),
                    list.Contains(ExchangeAccountType.Room), list.Contains(ExchangeAccountType.Equipment), list.Contains(ExchangeAccountType.SharedMailbox), list.Contains(ExchangeAccountType.SecurityGroup));
            }
            #endregion

            return ObjectUtils.CreateListFromDataReader<ExchangeAccount>(
                Database.SearchExchangeAccountsByTypes(SecurityContext.User.UserId, itemId,
                    types, filterColumn, filterValue, sortColumn));
        }


        public ExchangeAccount GetAccount(int itemId, int accountId, bool withLog = true)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                ExchangeAccount m1 = new ExchangeAccount();
                m1.AccountId = 1;
                m1.AccountName = "john_fabrikam";
                m1.AccountType = ExchangeAccountType.Mailbox;
                m1.DisplayName = "John Smith";
                m1.PrimaryEmailAddress = "john@fabrikam.net";

                // Log Extension
                if (withLog)
                    LogExtension.WriteObject(m1);

                return m1;
            }
            #endregion

            ExchangeAccount account = ObjectUtils.FillObjectFromDataReader<ExchangeAccount>(
                Database.GetExchangeAccount(itemId, accountId));

            if (account == null)
                return null;

            // Log Extension
            if (withLog)
                LogExtension.WriteObject(account);

            return account;
        }

        public ExchangeAccount GetAccountByAccountName(string userPrincipalName)
        {
            ExchangeAccount account = ObjectUtils.FillObjectFromDataReader<ExchangeAccount>(
                Database.GetExchangeAccountByAccountNameWithoutItemId(userPrincipalName));

            if (account == null)
                return null;

            return account;
        }

        public bool CheckAccountCredentials(int itemId, string email, string password)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "AUTHENTICATE", email, itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return false;

                // check credentials
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
                return exchange.CheckAccountCredentials(email, password);
            }
            catch (Exception ex)
            {
                TaskManager.WriteError(ex);
                return false;
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }

        public ExchangeAccount SearchAccount(ExchangeAccountType accountType, string primaryEmailAddress)
        {
            ExchangeAccount account = ObjectUtils.FillObjectFromDataReader<ExchangeAccount>(
                Database.SearchExchangeAccount(SecurityContext.User.UserId,
                accountType, primaryEmailAddress));

            if (account == null)
                return null;

            return account;
        }

        private int AddAccount(int itemId, ExchangeAccountType accountType,
            string accountName, string displayName, string primaryEmailAddress, bool mailEnabledPublicFolder,
            MailboxManagerActions mailboxManagerActions, string samAccountName, string accountPassword, int mailboxPlanId, string subscriberNumber)
        {
            return Database.AddExchangeAccount(itemId, accountType,
                accountName, displayName, primaryEmailAddress, mailEnabledPublicFolder,
                mailboxManagerActions.ToString(), samAccountName, mailboxPlanId, (string.IsNullOrEmpty(subscriberNumber) ? null : subscriberNumber.Trim()));
        }

        private void UpdateAccount(ExchangeAccount account)
        {
            Database.UpdateExchangeAccount(account.AccountId, account.AccountName, account.AccountType, account.DisplayName,
                account.PrimaryEmailAddress, account.MailEnabledPublicFolder,
                account.MailboxManagerActions.ToString(), account.SamAccountName, account.MailboxPlanId, account.ArchivingMailboxPlanId,
                (string.IsNullOrEmpty(account.SubscriberNumber) ? null : account.SubscriberNumber.Trim()),
                account.EnableArchiving);
        }

        private void DeleteAccount(int itemId, int accountId)
        {
            // try to get organization
            if (GetOrganization(itemId, false) == null)
                return;

            Database.DeleteExchangeAccount(itemId, accountId);
        }
        /*
                private string BuildAccountName(string orgId, string name)
                {
                    string accountName = name = name.Replace(" ", "");
                    int counter = 0;
                    bool bFound = false;

                    if (!AccountExists(accountName)) return accountName;

                    do
                    {
                        accountName = genSamLogin(name, counter.ToString("d5"));

                        if (!AccountExists(accountName)) bFound = true;

                        counter++;
                    }
                    while (!bFound);

                    return accountName;
                }

                private string genSamLogin(string login, string strCounter)
                {
                    int maxLogin = 20;
                    int fullLen = login.Length + strCounter.Length;
                    if (fullLen <= maxLogin)
                        return login + strCounter;
                    else
                    {
                        if (login.Length - (fullLen - maxLogin) > 0)
                            return login.Substring(0, login.Length - (fullLen - maxLogin)) + strCounter;
                        else return strCounter; // ????
                    }

                }
        */

        #endregion

        #region Account Email Addresses
        private bool EmailAddressExists(string emailAddress, bool checkContacts)
        {
            return Database.ExchangeAccountEmailAddressExists(emailAddress, checkContacts);
        }


        private ExchangeEmailAddress[] GetAccountEmailAddresses(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                List<ExchangeEmailAddress> demoEmails = new List<ExchangeEmailAddress>();
                ExchangeEmailAddress e1 = new ExchangeEmailAddress();
                e1.EmailAddress = "john@fabrikam.net";
                e1.IsPrimary = true;
                demoEmails.Add(e1);

                ExchangeEmailAddress e2 = new ExchangeEmailAddress();
                e2.EmailAddress = "john.smith@fabrikam.net";
                demoEmails.Add(e2);

                ExchangeEmailAddress e3 = new ExchangeEmailAddress();
                e3.EmailAddress = "john@fabrikam.hosted-exchange.com";
                demoEmails.Add(e3);
                return demoEmails.ToArray();
            }
            #endregion

            List<ExchangeEmailAddress> emails = ObjectUtils.CreateListFromDataReader<ExchangeEmailAddress>(
                Database.GetExchangeAccountEmailAddresses(accountId));

            // load account
            ExchangeAccount account = GetAccount(itemId, accountId, false);

            foreach (ExchangeEmailAddress email in emails)
            {
                if (String.Compare(account.PrimaryEmailAddress, email.EmailAddress, true) == 0)
                {
                    email.IsPrimary = true;
                }

                if (String.Compare(account.UserPrincipalName, email.EmailAddress, true) == 0)
                {
                    email.IsUserPrincipalName = true;
                }

            }

            return emails.ToArray();
        }

        private void AddAccountEmailAddress(int accountId, string emailAddress)
        {
            Database.AddExchangeAccountEmailAddress(accountId, emailAddress);
        }

        private void DeleteAccountEmailAddresses(int accountId, string[] emailAddresses)
        {
            foreach (string emailAddress in emailAddresses)
            {
                // Log Extension
                LogExtension.WriteVariable("E-mail Addresses deleted", emailAddress);

                Database.DeleteExchangeAccountEmailAddress(accountId, emailAddress);
            }
        }

        #endregion

        #region Domains
        public List<ExchangeDomainName> GetOrganizationDomains(int itemId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                List<ExchangeDomainName> demoDomains = new List<ExchangeDomainName>();
                ExchangeDomainName d1 = new ExchangeDomainName();
                d1.DomainId = 1;
                d1.DomainName = "fabrikam.hosted-exchange.com";
                d1.IsDefault = false;
                d1.IsHost = true;
                demoDomains.Add(d1);

                ExchangeDomainName d2 = new ExchangeDomainName();
                d2.DomainId = 2;
                d2.DomainName = "fabrikam.net";
                d2.IsDefault = true;
                d2.IsHost = false;
                demoDomains.Add(d2);

                return demoDomains;
            }
            #endregion

            // load organization
            Organization org = GetOrganization(itemId, false);
            if (org == null)
                return null;

            // load all domains
            List<ExchangeDomainName> domains = ObjectUtils.CreateListFromDataReader<ExchangeDomainName>(
                Database.GetExchangeOrganizationDomains(itemId));

            // set default domain
            foreach (ExchangeDomainName domain in domains)
            {
                if (String.Compare(domain.DomainName, org.DefaultDomain, true) == 0)
                {
                    domain.IsDefault = true;
                    break;
                }
            }

            return domains;
        }

        public int AddAuthoritativeDomain(int itemId, int domainId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "ADD_DOMAIN", itemId, new BackgroundTaskParameter("Domain ID", domainId));

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // load domain
                DomainInfo domain = ServerController.GetDomain(domainId);
                if (domain == null)
                    return -1;

                // Log Extension
                LogExtension.SetItemName(domain.DomainName);
                LogExtension.WriteObject(domain);

                // delete domain on Exchange
                int[] hubTransportServiceIds;
                int[] clientAccessServiceIds;
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                GetExchangeServices(exchangeServiceId, out hubTransportServiceIds, out clientAccessServiceIds);

                foreach (int id in hubTransportServiceIds)
                {
                    ExchangeServer hubTransportRole = null;
                    try
                    {
                        hubTransportRole = GetExchangeServer(id, org.ServiceId);
                    }
                    catch (Exception ex)
                    {
                        TaskManager.WriteError(ex);
                        continue;
                    }

                    string[] domains = hubTransportRole.GetAuthoritativeDomains();
                    if (domains != null)
                        Array.Sort(domains);

                    if (domains == null || Array.BinarySearch(domains, domain.DomainName) < 0)
                        hubTransportRole.AddAuthoritativeDomain(domain.DomainName);
                    break;
                }

                //Add the service records
                if (domain != null)
                {
                    if (domain.ZoneItemId != 0)
                    {
                        ServerController.AddServiceDNSRecords(org.PackageId, ResourceGroups.Exchange, domain, "");
                    }

                    // check if spamexperts filter is needed
                    StringDictionary exSettings = ServerController.GetServiceSettings(exchangeServiceId);
                    if (exSettings != null && Convert.ToBoolean(exSettings["EnableMailFilter"]))
                    {

                        SpamExpertsRoute route = new SpamExpertsRoute();
                        route.PackageId = org.PackageId;
                        route.DomainName = domain.DomainName;
                        route.Route = exSettings["MailFilterDestinations"];
                        SpamExpertsController.AddDomainFilter(route);
                    }

                    //Add to Mail Cleaner
                    APIMailCleanerHelper.DomainAdd(domain.DomainName, domain.PackageId);
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

        public int ChangeAcceptedDomainType(int itemId, int domainId, ExchangeAcceptedDomainType domainType)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record

            List<BackgroundTaskParameter> parameters = new List<BackgroundTaskParameter>();
            parameters.Add(new BackgroundTaskParameter("Domain ID", domainId));
            parameters.Add(new BackgroundTaskParameter("Domain Type", domainType.ToString()));

            TaskManager.StartTask("EXCHANGE", "CHANGE_DOMAIN_TYPE", itemId, parameters);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // load domain
                DomainInfo domain = ServerController.GetDomain(domainId);
                if (domain == null)
                    return -1;

                int[] hubTransportServiceIds;
                int[] clientAccessServiceIds;
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                GetExchangeServices(exchangeServiceId, out hubTransportServiceIds, out clientAccessServiceIds);

                foreach (int id in hubTransportServiceIds)
                {
                    ExchangeServer hubTransportRole = null;
                    try
                    {
                        hubTransportRole = GetExchangeServer(id, org.ServiceId);
                    }
                    catch (Exception ex)
                    {
                        TaskManager.WriteError(ex);
                        continue;
                    }

                    hubTransportRole.ChangeAcceptedDomainType(domain.DomainName, domainType);
                    break;

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

        public int DeleteAuthoritativeDomain(int itemId, int domainId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DELETE_DOMAIN", itemId, new BackgroundTaskParameter("Domain ID", domainId));

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // load domain
                DomainInfo domain = ServerController.GetDomain(domainId);
                if (domain == null)
                    return -1;

                // Log Extension
                LogExtension.SetItemName(domain.DomainName);
                LogExtension.WriteObject(domain);

                if (Database.CheckDomainUsedByHostedOrganization(domain.DomainName) == 1)
                {
                    return -1;
                }

                // delete domain on Exchange
                int[] hubTransportServiceIds;
                int[] clientAccessServiceIds;
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                GetExchangeServices(exchangeServiceId, out hubTransportServiceIds, out clientAccessServiceIds);

                foreach (int id in hubTransportServiceIds)
                {
                    ExchangeServer hubTransportRole = null;
                    try
                    {
                        hubTransportRole = GetExchangeServer(id, org.ServiceId);
                    }
                    catch (Exception ex)
                    {
                        TaskManager.WriteError(ex);
                        continue;
                    }

                    hubTransportRole.DeleteAuthoritativeDomain(domain.DomainName);
                    break;

                }

                //Delete the service records
                if (domain != null)
                {
                    if (domain.ZoneItemId != 0)
                    {
                        ServerController.RemoveServiceDNSRecords(org.PackageId, ResourceGroups.Exchange, domain, "", false);
                    }

                    // check if spamexperts filter needs to be removed
                    StringDictionary exSettings = ServerController.GetServiceSettings(exchangeServiceId);
                    if (exSettings != null && Convert.ToBoolean(exSettings["EnableMailFilter"]))
                    {
                        SpamExpertsController.DeleteDomainFilter(domain);
                    }

                    //Delete Domain from Mail Cleaner
                    APIMailCleanerHelper.DomainRemove(domain.DomainName, domain.PackageId);
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


        #endregion

        #region Mailboxes

        private void UpdateExchangeAccount(int accountId, string accountName, ExchangeAccountType accountType,
            string displayName, string primaryEmailAddress, bool mailEnabledPublicFolder,
            string mailboxManagerActions, string samAccountName, string accountPassword, int mailboxPlanId, int archivePlanId, string subscriberNumber,
            bool EnableArchiving)
        {
            Database.UpdateExchangeAccount(accountId,
                accountName,
                accountType,
                displayName,
                primaryEmailAddress,
                mailEnabledPublicFolder,
                mailboxManagerActions,
                samAccountName,
                mailboxPlanId, archivePlanId,
                (string.IsNullOrEmpty(subscriberNumber) ? null : subscriberNumber.Trim()), EnableArchiving);
        }

        public string CreateJournalRule(int itemId, string journalEmail, string scope, string recipientEmail, bool enabled)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return null;

            // place log record
            TaskManager.StartTask("EXCHANGE", "CREATE_JOURNAL_RULE", journalEmail, itemId);

            // Log Extension
            LogExtension.WriteVariables(new { journalEmail, scope, recipientEmail });

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null) return null;

                int exchangeServiceId = PackageController.GetPackageServiceId(org.PackageId, ResourceGroups.Exchange);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                return exchange.CreateJournalRule(journalEmail, scope, recipientEmail, enabled);
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

        public ExchangeJournalRule GetJournalRule(int itemId, string journalEmail)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return null;

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_JOURNAL_RULE", journalEmail, itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null) return null;

                int exchangeServiceId = PackageController.GetPackageServiceId(org.PackageId, ResourceGroups.Exchange);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                return exchange.GetJournalRule(journalEmail);
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

        public int SetJournalRule(int itemId, ExchangeJournalRule rule)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return -1;

            // place log record
            TaskManager.StartTask("EXCHANGE", "SET_JOURNAL_RULE", rule.JournalEmailAddress, itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null) return -1;

                int exchangeServiceId = PackageController.GetPackageServiceId(org.PackageId, ResourceGroups.Exchange);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.SetJournalRule(rule);
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

        public int CreateMailbox(int itemId, int accountId, ExchangeAccountType accountType, string accountName,
            string displayName, string name, string domain, string password, bool sendSetupInstructions, string setupInstructionMailAddress, int mailboxPlanId, int archivedPlanId, string subscriberNumber, bool EnableArchiving)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // check mailbox quota
            OrganizationStatistics orgStats = GetOrganizationStatisticsByOrganization(itemId);
            if (accountType == ExchangeAccountType.SharedMailbox)
            {
                if ((orgStats.AllocatedSharedMailboxes > -1) && (orgStats.CreatedSharedMailboxes >= orgStats.AllocatedSharedMailboxes))
                    return BusinessErrorCodes.ERROR_EXCHANGE_MAILBOXES_QUOTA_LIMIT;
            }
            else if ((accountType == ExchangeAccountType.Room) || (accountType == ExchangeAccountType.Equipment))
            {
                if ((orgStats.AllocatedResourceMailboxes > -1) && (orgStats.CreatedResourceMailboxes >= orgStats.AllocatedResourceMailboxes))
                    return BusinessErrorCodes.ERROR_EXCHANGE_MAILBOXES_QUOTA_LIMIT;
            }
            else if (accountType == ExchangeAccountType.JournalingMailbox)
            {
                if ((orgStats.AllocatedJournalingMailboxes > -1) && (orgStats.CreatedJournalingMailboxes >= orgStats.AllocatedJournalingMailboxes))
                    return BusinessErrorCodes.ERROR_EXCHANGE_JOURNALING_MAILBOXES_QUOTA_LIMIT;
            }
            else
            {
                if ((orgStats.AllocatedMailboxes > -1) && (orgStats.CreatedMailboxes >= orgStats.AllocatedMailboxes))
                    return BusinessErrorCodes.ERROR_EXCHANGE_MAILBOXES_QUOTA_LIMIT;
            }


            // place log record
            TaskManager.StartTask("EXCHANGE", "CREATE_MAILBOX", displayName, itemId);

            // Log Extension
            LogExtension.WriteVariables(new { accountName, displayName, accountType });

            bool userCreated = false;
            Organization org = null;
            try
            {
                accountName = accountName.Trim();
                displayName = displayName.Trim();
                name = name.Trim();
                domain = domain.Trim();


                // load organization
                org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // e-mail
                string email = name + "@" + domain;
                bool enabled = (accountType == ExchangeAccountType.Mailbox || accountType == ExchangeAccountType.JournalingMailbox);


                //  string accountName = string.Empty;
                //Create AD user if needed
                if (accountId == 0)
                {
                    accountId = OrganizationController.CreateUser(org.Id, displayName, name, domain, password, subscriberNumber, enabled, false, string.Empty, out accountName);
                    if (accountId > 0)
                        userCreated = true;
                }
                if (accountId < 0)
                    return accountId;

                // get mailbox settings
                Organizations orgProxy = OrganizationController.GetOrganizationProxy(org.ServiceId);
                OrganizationUser retUser = orgProxy.GetUserGeneralSettings(accountName, org.OrganizationId);


                int exchangeServiceId = PackageController.GetPackageServiceId(org.PackageId, ResourceGroups.Exchange);

                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);


                //Create Exchange Organization
                if (string.IsNullOrEmpty(org.GlobalAddressList))
                {
                    ExtendToExchangeOrganization(ref org);

                    PackageController.UpdatePackageItem(org);
                }

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                //verify if the mailbox fits in the storage quota
                int maxDiskSpace = -1;
                int quotaUsed = 0;
                if (orgStats.AllocatedDiskSpace > 0)
                {
                    maxDiskSpace = orgStats.AllocatedDiskSpace;
                    quotaUsed = orgStats.UsedDiskSpace;
                }

                ExchangeMailboxPlan plan = GetExchangeMailboxPlan(itemId, mailboxPlanId);
                if (maxDiskSpace != -1)
                {
                    if (plan.MailboxSizeMB == -1)
                        return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;

                    if ((quotaUsed + plan.MailboxSizeMB) > (maxDiskSpace))
                        return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;
                }

                if (plan.AllowLitigationHold)
                {
                    int maxRecoverableItemsSpace = -1;
                    int quotaRecoverableItemsUsed = 0;
                    if (orgStats.AllocatedLitigationHoldSpace > 0)
                    {
                        maxRecoverableItemsSpace = orgStats.AllocatedLitigationHoldSpace;
                        quotaRecoverableItemsUsed = orgStats.UsedLitigationHoldSpace;
                    }

                    if (maxRecoverableItemsSpace != -1)
                    {
                        if (plan.RecoverableItemsSpace == -1)
                            return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;

                        if (plan.RecoverableItemsSpace < 6144)
                            return BusinessErrorCodes.ERROR_EXCHANGE_INVALID_RECOVERABLEITEMS_QUOTA;

                        if ((quotaRecoverableItemsUsed + plan.RecoverableItemsSpace) > (maxRecoverableItemsSpace))
                            return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;
                    }
                }

                if (plan.EnableArchiving)
                {
                    int maxArchivingStorage = -1;
                    int quotaArchivingStorageUsed = 0;
                    if (orgStats.AllocatedArchingStorage > 0)
                    {
                        maxArchivingStorage = orgStats.AllocatedArchingStorage;
                        quotaArchivingStorageUsed = orgStats.UsedArchingStorage;
                    }

                    if (maxArchivingStorage != -1)
                    {
                        if (plan.ArchiveSizeMB == -1)
                            return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;

                        if ((quotaArchivingStorageUsed + plan.ArchiveSizeMB) > (maxArchivingStorage))
                            return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;
                    }
                }

                //GetServiceSettings
                StringDictionary primSettings = ServerController.GetServiceSettings(exchangeServiceId);
                PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                string samAccountName = exchange.CreateMailEnableUser(email, org.OrganizationId, org.DistinguishedName,
                                                org.SecurityGroup, org.DefaultDomain,
                                                accountType, primSettings["mailboxdatabase"],
                                                org.OfflineAddressBook,
                                                org.AddressBookPolicy,
                                                retUser.SamAccountName,
                                                plan.EnablePOP,
                                                plan.EnableIMAP,
                                                plan.EnableOWA,
                                                plan.EnableMAPI,
                                                plan.EnableActiveSync,
                                                plan.MailboxSizeMB != -1 ? (((long)plan.IssueWarningPct * (long)plan.MailboxSizeMB * 1024) / 100) : -1,
                                                plan.MailboxSizeMB != -1 ? (((long)plan.ProhibitSendPct * (long)plan.MailboxSizeMB * 1024) / 100) : -1,
                                                plan.MailboxSizeMB != -1 ? (((long)plan.ProhibitSendReceivePct * (long)plan.MailboxSizeMB * 1024) / 100) : -1,
                                                plan.KeepDeletedItemsDays,
                                                plan.MaxRecipients,
                                                plan.MaxSendMessageSizeKB,
                                                plan.MaxReceiveMessageSizeKB,
                                                plan.HideFromAddressBook,
                                                Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_ISCONSUMER].QuotaAllocatedValue),
                                                plan.AllowLitigationHold,
                                                plan.RecoverableItemsSpace != -1 ? (plan.RecoverableItemsSpace * 1024) : -1,
                                                plan.RecoverableItemsSpace != -1 ? (((long)plan.RecoverableItemsWarningPct * (long)plan.RecoverableItemsSpace * 1024) / 100) : -1);

                MailboxManagerActions pmmActions = MailboxManagerActions.GeneralSettings
                    | MailboxManagerActions.MailFlowSettings
                    | MailboxManagerActions.AdvancedSettings
                    | MailboxManagerActions.EmailAddresses;


                UpdateExchangeAccount(accountId, accountName, accountType, displayName, email, false, pmmActions.ToString(), samAccountName, password, mailboxPlanId, archivedPlanId, subscriberNumber, EnableArchiving);

                ResultObject resPolicy = new ResultObject() { IsSuccess = true };
                SetMailBoxRetentionPolicyAndArchiving(itemId, mailboxPlanId, archivedPlanId, accountName, exchange, org.OrganizationId, resPolicy, EnableArchiving);
                if (!resPolicy.IsSuccess)
                {
                    TaskManager.WriteError("Error SetMailBoxRetentionPolicy: " + string.Join(", ", resPolicy.ErrorCodes.ToArray()));
                }


                // send setup instructions
                if (sendSetupInstructions)
                {
                    try
                    {
                        // send setup instructions
                        int sendResult = SendMailboxSetupInstructions(itemId, accountId, true, setupInstructionMailAddress, null);
                        if (sendResult < 0)
                            TaskManager.WriteWarning("Setup instructions were not sent. Error code: " + sendResult);
                    }
                    catch (Exception ex)
                    {
                        TaskManager.WriteError(ex);
                    }
                }

                try
                {
                    // update OAB
                    // check if this is the first mailbox within the organization
                    if (GetAccounts(itemId, ExchangeAccountType.Mailbox).Count == 1)
                        exchange.UpdateOrganizationOfflineAddressBook(org.OfflineAddressBook);
                }
                catch (Exception ex)
                {
                    TaskManager.WriteError(ex);
                }

                // check if spamexperts filter per-mailbox is set
                if (primSettings != null && Convert.ToBoolean(primSettings["EnableMailFilter"]))
                {
                    SpamExpertsController.AddEmailFilter(org.PackageId, name, password, domain);
                }

                // Log Extension
                LogExtension.WriteVariables(new { email, samAccountName, accountId });

                return accountId;
            }
            catch (Exception ex)
            {
                //rollback AD user
                if (userCreated)
                {
                    try
                    {
                        OrganizationController.DeleteUser(org.Id, accountId);
                    }
                    catch (Exception rollbackException)
                    {
                        TaskManager.WriteError(rollbackException);
                    }
                }
                throw TaskManager.WriteError(ex);

            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }

        public int DisableMailbox(int itemId, int accountId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DISABLE_MAILBOX", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.UserPrincipalName);
                account.LogProperty(a => a.UserPrincipalName);

                if (BlackBerryController.CheckBlackBerryUserExists(accountId))
                {
                    BlackBerryController.DeleteBlackBerryUser(itemId, accountId);
                }

                // delete mailbox
                int serviceExchangeId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(serviceExchangeId, org.ServiceId);
                exchange.DisableMailbox(account.UserPrincipalName);

                if (account.AccountType == ExchangeAccountType.JournalingMailbox) exchange.RemoveJournalRule(account.PrimaryEmailAddress);

                account.AccountType = ExchangeAccountType.User;
                account.MailEnabledPublicFolder = false;
                UpdateAccount(account);
                Database.DeleteUserEmailAddresses(account.AccountId, account.PrimaryEmailAddress);

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

        public int ExportMailBox(int itemId, int accountId, string path)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);

            if (accountCheck < 0)
            {
                return accountCheck;
            }

            // place log record
            TaskManager.StartTask("EXCHANGE", "EXPORT_MAILBOX", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // export mailbox
                int serviceExchangeId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(serviceExchangeId, org.ServiceId);
                exchange.ExportMailBox(org.OrganizationId, account.UserPrincipalName, path);

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

        public int SetDeletedMailbox(int itemId, int accountId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "SET_DELETED_MAILBOX", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                if (BlackBerryController.CheckBlackBerryUserExists(accountId))
                {
                    BlackBerryController.DeleteBlackBerryUser(itemId, accountId);
                }

                // delete mailbox
                int serviceExchangeId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(serviceExchangeId, org.ServiceId);
                exchange.DisableMailbox(account.UserPrincipalName);

                if (account.AccountType == ExchangeAccountType.JournalingMailbox) exchange.RemoveJournalRule(account.PrimaryEmailAddress);

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

        public int DeleteMailbox(int itemId, int accountId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DELETE_MAILBOX", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.PrimaryEmailAddress);

                if (BlackBerryController.CheckBlackBerryUserExists(accountId))
                {
                    BlackBerryController.DeleteBlackBerryUser(itemId, accountId);
                }


                // delete mailbox
                int serviceExchangeId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(serviceExchangeId, org.ServiceId);

                if (account.AccountType == ExchangeAccountType.JournalingMailbox) exchange.RemoveJournalRule(account.PrimaryEmailAddress);

                exchange.DeleteMailbox(account.UserPrincipalName);



                // unregister account
                DeleteAccount(itemId, accountId);

                // check if spamexperts filter per-mailbox is set
                StringDictionary settings = ServerController.GetServiceSettings(serviceExchangeId);
                if (settings != null && Convert.ToBoolean(settings["EnableMailFilter"]))
                {
                    SpamExpertsController.DeleteEmailFilter(org.PackageId, account.UserPrincipalName);
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

        private ExchangeMailboxAutoReplySettings GetDemoMailboxAutoReplySettings()
        {
            ExchangeMailboxAutoReplySettings ar = new ExchangeMailboxAutoReplySettings();
            ar.AutoReplyState = OofState.Enabled;
            ar.ExternalAudience = ExternalAudience.All;
            ar.ExternalMessage = "<html><body>I am out of the office today.</body></html>";
            ar.InternalMessage = "<html><body>I'm out of the office today.</body></html>";
            return ar;
        }

        private ExchangeMailbox GetDemoMailboxSettings()
        {
            ExchangeMailbox mb = new ExchangeMailbox();
            mb.DisplayName = "John Smith";
            mb.Domain = "HSTDEXCH1";
            mb.AccountName = "john_fabrikam";
            mb.EnableForwarding = true;
            mb.SaveSentItems = 2;
            mb.EnableIMAP = true;
            mb.EnableMAPI = true;
            mb.EnablePOP = true;
            mb.FirstName = "John";
            mb.LastName = "Smith";
            mb.ForwardingAccount = GetAccounts(0, ExchangeAccountType.Mailbox)[1];
            mb.EnableForwarding = true;
            mb.IssueWarningKB = 150000;
            mb.KeepDeletedItemsDays = 14;
            mb.LastLogoff = DateTime.Now;
            mb.LastLogon = DateTime.Now;
            mb.ManagerAccount = GetAccounts(0, ExchangeAccountType.Mailbox)[1];
            mb.MaxReceiveMessageSizeKB = 20000;
            mb.MaxRecipients = 30;
            mb.MaxSendMessageSizeKB = 10000;
            mb.ProhibitSendKB = 160000;
            mb.ProhibitSendReceiveKB = 170000;
            mb.TotalItems = 5;
            mb.TotalSizeMB = 4;
            return mb;
        }

        private ExchangeResourceMailboxSettings GetDemoResourceMailboxSettings()
        {
            ExchangeResourceMailboxSettings mb = new ExchangeResourceMailboxSettings();
            mb.DisplayName = "Meeting Room";
            mb.AccountName = "room1";
            mb.AddAdditionalResponse = false;
            mb.AllBookInPolicy = true;
            mb.AllowRecurringMeetings = true;
            mb.AllRequestInPolicy = false;
            mb.AutomateProcessing = CalendarProcessingFlags.AutoAccept;
            mb.BookingWindowInDays = 180;
            mb.EnforceSchedulingHorizon = true;
            mb.MaximumDurationInMinutes = 1440;
            mb.ResourceCapacity = 20;
            mb.ScheduleOnlyDuringWorkHours = false;
            return mb;
        }

        public ExchangeMailboxAutoReplySettings GetMailboxAutoReplySettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoMailboxAutoReplySettings();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_MAILBOX_AUTOREPLY", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
                return exchange.GetMailboxAutoReplySettings(account.UserPrincipalName);
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

        public int SetMailboxAutoReplySettings(int itemId, int accountId, ExchangeMailboxAutoReplySettings settings)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_MAILBOX_AUTOREPLY", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.UserPrincipalName);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                var oldObj = exchange.GetMailboxAutoReplySettings(account.UserPrincipalName);

                exchange.SetMailboxAutoReplySettings(account.UserPrincipalName, settings);

                var newObj = exchange.GetMailboxAutoReplySettings(account.UserPrincipalName);

                // Log Extension
                LogExtension.LogPropertiesIfChanged(oldObj, newObj);

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

        public ExchangeMailbox GetMailboxGeneralSettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoMailboxSettings();
            }
            #endregion

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // place log record
                TaskManager.StartTask("EXCHANGE", "GET_MAILBOX_GENERAL", null, itemId, org.PackageId, null);

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
                return exchange.GetMailboxGeneralSettings(account.UserPrincipalName);
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

        public int SetMailboxGeneralSettings(int itemId, int accountId, bool hideAddressBook, bool disabled)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_MAILBOX_GENERAL", itemId);

            // Log Extension
            LogExtension.WriteVariables(new { hideAddressBook, disabled }, "Try ");

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.UserPrincipalName);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);


                PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                if (Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_ISCONSUMER].QuotaAllocatedValue))
                    hideAddressBook = true;

                // Log Extension
                LogExtension.WriteVariables(new { hideAddressBook });

                var oldObj = exchange.GetMailboxGeneralSettings(account.UserPrincipalName);

                exchange.SetMailboxGeneralSettings(
                    account.UserPrincipalName,
                    hideAddressBook,
                    disabled);

                var newObj = exchange.GetMailboxGeneralSettings(account.UserPrincipalName);

                // Log Extension
                LogExtension.LogPropertiesIfChanged(oldObj, newObj);

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

        public ExchangeResourceMailboxSettings GetResourceMailboxSettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoResourceMailboxSettings();
            }
            #endregion

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // place log record
                TaskManager.StartTask("EXCHANGE", "GET_RESOURCE_MAILBOX", null, itemId, org.PackageId, null);

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
                ExchangeResourceMailboxSettings resourceMailbox = exchange.GetResourceMailboxSettings(account.UserPrincipalName);
                resourceMailbox.DisplayName = account.DisplayName;
                return resourceMailbox;
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

        public int SetResourceMailboxSettings(int itemId, int accountId, ExchangeResourceMailboxSettings resourceSettings)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_RESOURCE_MAILBOX", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.UserPrincipalName);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);


                PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                var oldObj = exchange.GetResourceMailboxSettings(account.UserPrincipalName);

                exchange.SetResourceMailboxSettings(
                    account.UserPrincipalName,
                    resourceSettings);

                var newObj = exchange.GetResourceMailboxSettings(account.UserPrincipalName);

                // Log Extension
                LogExtension.LogPropertiesIfChanged(oldObj, newObj);

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

        public ExchangeEmailAddress[] GetMailboxEmailAddresses(int itemId, int accountId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_MAILBOX_ADDRESSES", itemId);

            try
            {
                return GetAccountEmailAddresses(itemId, accountId);
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

        public int AddMailboxEmailAddress(int itemId, int accountId, string emailAddress)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "ADD_MAILBOX_ADDRESS", itemId);

            // Log Extension
            LogExtension.WriteVariable("Email Address added", emailAddress);

            try
            {
                // check
                if (EmailAddressExists(emailAddress, false))
                    return BusinessErrorCodes.ERROR_EXCHANGE_EMAIL_EXISTS;

                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.PrimaryEmailAddress);
                account.LogProperty(a => a.UserPrincipalName);

                // add e-mail
                AddAccountEmailAddress(accountId, emailAddress);

                // update e-mail addresses
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.SetMailboxEmailAddresses(
                    account.UserPrincipalName,
                    GetAccountSimpleEmailAddresses(itemId, accountId));

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

        private OCSServer GetOCSProxy(int itemId)
        {
            Organization org = OrganizationController.GetOrganization(itemId);
            int serviceId = PackageController.GetPackageServiceId(org.PackageId, ResourceGroups.OCS);

            OCSServer ocs = new OCSServer();
            ServiceProviderProxy.Init(ocs, serviceId);


            return ocs;
        }

        public int SetMailboxPrimaryEmailAddress(int itemId, int accountId, string emailAddress)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "SET_PRIMARY_MAILBOX_ADDRESS", itemId);

            try
            {
                // get account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.UserPrincipalName);
                account.LogPropertyIfChanged(a => a.PrimaryEmailAddress, emailAddress);

                account.PrimaryEmailAddress = emailAddress;

                // update exchange
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.SetMailboxPrimaryEmailAddress(
                    account.UserPrincipalName,
                    emailAddress);

                if (Database.CheckOCSUserExists(account.AccountId))
                {
                    OCSServer ocs = GetOCSProxy(itemId);
                    string instanceId = Database.GetOCSUserInstanceID(account.AccountId);
                    ocs.SetUserPrimaryUri(instanceId, emailAddress);
                }

                if (Database.CheckLyncUserExists(account.AccountId))
                {
                    LyncController.SetLyncUserGeneralSettings(itemId, accountId, emailAddress, null);
                }

                if (Database.CheckSfBUserExists(account.AccountId))
                {
                    SfBController.SetSfBUserGeneralSettings(itemId, accountId, emailAddress, null);
                }

                // save account
                UpdateAccount(account);

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

        public int DeleteMailboxEmailAddresses(int itemId, int accountId, string[] emailAddresses)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DELETE_MAILBOX_ADDRESSES", itemId);

            try
            {
                // get account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // delete e-mail addresses
                List<string> toDelete = new List<string>();
                foreach (string emailAddress in emailAddresses)
                {
                    if ((String.Compare(account.PrimaryEmailAddress, emailAddress, true) != 0) &
                        (String.Compare(account.UserPrincipalName, emailAddress, true) != 0))
                        toDelete.Add(emailAddress);
                }

                // Log Extension
                LogExtension.SetItemName(account.UserPrincipalName);

                // delete from meta-base
                DeleteAccountEmailAddresses(accountId, toDelete.ToArray());

                // delete from Exchange
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // update e-mail addresses
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.SetMailboxEmailAddresses(
                    account.UserPrincipalName,
                    GetAccountSimpleEmailAddresses(itemId, accountId));

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

        public ExchangeMailbox GetMailboxMailFlowSettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoMailboxSettings();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_MAILBOX_MAILFLOW", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
                ExchangeMailbox mailbox = exchange.GetMailboxMailFlowSettings(account.UserPrincipalName);
                mailbox.DisplayName = account.DisplayName;
                return mailbox;
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

        public int SetMailboxMailFlowSettings(int itemId, int accountId,
            bool enableForwarding, int SaveSentItems, string forwardingAccountName, bool forwardToBoth,
            string[] sendOnBehalfAccounts, string[] acceptAccounts, string[] rejectAccounts,
            bool requireSenderAuthentication)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_MAILBOX_MAILFLOW", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.UserPrincipalName);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                var oldSettings = exchange.GetMailboxMailFlowSettings(account.UserPrincipalName);

                exchange.SetMailboxMailFlowSettings(account.UserPrincipalName,
                    enableForwarding,
                    SaveSentItems,
                    forwardingAccountName,
                    forwardToBoth,
                    sendOnBehalfAccounts,
                    acceptAccounts,
                    rejectAccounts,
                    requireSenderAuthentication);

                var newSettings = exchange.GetMailboxMailFlowSettings(account.UserPrincipalName);

                // Log Extension
                LogExtension.LogPropertiesIfChanged(oldSettings, newSettings);

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


        public ExchangeMailbox GetMailboxAdvancedSettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoMailboxSettings();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_MAILBOX_ADVANCED", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
                ExchangeMailbox mailbox = exchange.GetMailboxAdvancedSettings(account.UserPrincipalName);
                mailbox.DisplayName = account.DisplayName;
                return mailbox;
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

        public int SetMailboxManagerSettings(int itemId, int accountId, bool pmmAllowed, MailboxManagerActions action)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_MAILBOX_GENERAL", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                var newAction = action;

                // PMM settings
                if (pmmAllowed) newAction |= action;
                else newAction &= ~action;

                // Log Extension
                LogExtension.SetItemName(account.PrimaryEmailAddress);
                account.LogPropertyIfChanged(a => a.MailboxManagerActions, newAction);

                account.MailboxManagerActions = newAction;

                // update account
                UpdateAccount(account);

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

        public string GetMailboxSetupInstructions(int itemId, int accountId, bool pmm, bool emailMode, bool signup, string passwordResetUrl)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return string.Empty;
            }
            #endregion

            // load organization
            Organization org = GetOrganization(itemId);
            if (org == null)
                return null;

            // load user info
            UserInfo user = PackageController.GetPackageOwner(org.PackageId);

            // get letter settings
            UserSettings settings = UserController.GetUserSettings(user.UserId, UserSettings.EXCHANGE_MAILBOX_SETUP_LETTER);

            string settingName = user.HtmlMail ? "HtmlBody" : "TextBody";
            string body = settings[settingName];
            if (String.IsNullOrEmpty(body))
                return null;

            string result = "";
            try
            {
                result = EvaluateMailboxTemplate(itemId, accountId, pmm, false, false, body, passwordResetUrl);
            }
            catch (Exception ex)
            {
                TaskManager.WriteWarning("Cant EvaluateMailboxTemplate of GetMailboxSetupInstructions: " + ex.Message);
            }
            return user.HtmlMail ? result : result.Replace("\n", "<br/>");
        }

        private string EvaluateMailboxTemplate(int itemId, int accountId, bool pmm, bool emailMode, bool signup, string template, string passwordResetUrl)
        {
            Hashtable items = new Hashtable();

            // load organization
            Organization org = GetOrganization(itemId);
            if (org == null)
                return null;

            // add organization
            items["Organization"] = org;

            // load account
            ExchangeAccount account = GetAccount(itemId, accountId);
            if (account == null)
                return null;

            // add account
            items["Account"] = account;
            items["AccountDomain"] = account.PrimaryEmailAddress.Substring(account.PrimaryEmailAddress.IndexOf("@") + 1);
            items["DefaultDomain"] = org.DefaultDomain;

            Guid token;

            passwordResetUrl = OrganizationController.GenerateUserPasswordResetLink(account.ItemId, account.AccountId, out token, string.Empty, passwordResetUrl);

            if (!string.IsNullOrEmpty(passwordResetUrl))
            {
                items["PswResetUrl"] = passwordResetUrl;
            }


            if (!String.IsNullOrEmpty(account.SamAccountName))
            {
                int idx = account.SamAccountName.IndexOf("\\");
                items["SamDomain"] = account.SamAccountName.Substring(0, idx);
                items["SamUsername"] = account.SamAccountName.Substring(idx + 1);
            }

            // name servers
            PackageSettings packageSettings = PackageController.GetPackageSettings(org.PackageId, PackageSettings.NAME_SERVERS);
            string[] nameServers = new string[] { };
            if (!String.IsNullOrEmpty(packageSettings["NameServers"]))
                nameServers = packageSettings["NameServers"].Split(';');

            items["NameServers"] = nameServers;

            // service settings
            int exchangeServiceId = GetExchangeServiceID(org.PackageId);
            StringDictionary exchangeSettings = ServerController.GetServiceSettings(exchangeServiceId);
            if (exchangeSettings != null)
            {
                items["TempDomain"] = exchangeSettings["TempDomain"];
                items["AutodiscoverIP"] = exchangeSettings["AutodiscoverIP"];
                items["AutodiscoverDomain"] = exchangeSettings["AutodiscoverDomain"];
                items["OwaUrl"] = exchangeSettings["OwaUrl"];
                items["ActiveSyncServer"] = exchangeSettings["ActiveSyncServer"];
                items["SmtpServers"] = Utils.ParseDelimitedString(exchangeSettings["SmtpServers"], '\n');
            }

            items["Email"] = emailMode;
            items["Signup"] = signup;
            items["PMM"] = pmm;

            // evaluate template
            return PackageController.EvaluateTemplate(template, items);
        }

        public int SendMailboxSetupInstructions(int itemId, int accountId, bool signup, string to, string cc)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo);
            if (accountCheck < 0) return accountCheck;

            // load organization
            Organization org = GetOrganization(itemId);
            if (org == null)
                return -1;

            // load user info
            UserInfo user = PackageController.GetPackageOwner(org.PackageId);

            // get letter settings
            UserSettings settings = UserController.GetUserSettings(user.UserId, UserSettings.EXCHANGE_MAILBOX_SETUP_LETTER);

            string from = settings["From"];
            if (cc == null)
                cc = settings["CC"];
            string subject = settings["Subject"];
            string body = user.HtmlMail ? settings["HtmlBody"] : settings["TextBody"];
            bool isHtml = user.HtmlMail;

            MailPriority priority = MailPriority.Normal;
            if (!String.IsNullOrEmpty(settings["Priority"]))
                priority = (MailPriority)Enum.Parse(typeof(MailPriority), settings["Priority"], true);

            if (String.IsNullOrEmpty(body))
                return 0;// BusinessErrorCodes.ERROR_SETTINGS_ACCOUNT_LETTER_EMPTY_BODY;

            // load user info
            if (to == null)
                to = user.Email;

            subject = EvaluateMailboxTemplate(itemId, accountId, false, true, signup, subject, string.Empty);
            body = EvaluateMailboxTemplate(itemId, accountId, false, true, signup, body, string.Empty);

            // send message
            return MailHelper.SendMessage(from, to, cc, subject, body, priority, isHtml);
        }


        public ExchangeMailbox GetMailboxPermissions(int itemId, int accountId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_MAILBOX_PERMISSIONS", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
                ExchangeMailbox mailbox = exchange.GetMailboxPermissions(org.OrganizationId, account.UserPrincipalName);
                mailbox.DisplayName = account.DisplayName;
                return mailbox;
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

        public int SetMailboxPermissions(int itemId, int accountId, string[] sendAsaccounts, string[] fullAccessAcounts, string[] onBehalfOfAccounts, ExchangeAccount[] calendarAccounts, ExchangeAccount[] contactAccounts)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "SET_MAILBOX_PERMISSIONS", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.PrimaryEmailAddress);
                LogExtension.WriteVariables(new { sendAsaccounts, fullAccessAcounts, onBehalfOfAccounts });

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.SetMailboxPermissions(org.OrganizationId, account.UserPrincipalName, sendAsaccounts, fullAccessAcounts, onBehalfOfAccounts, calendarAccounts, contactAccounts);

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


        #region Mailbox plan
        public int SetExchangeMailboxPlan(int itemId, int accountId, int mailboxPlanId, int archivePlanId, bool EnableArchiving)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "SET_MAILBOXPLAN_RETENTIONPOLICY_ARCHIVING", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.UserPrincipalName);
                LogExtension.WriteVariable("RetentionPolicy/Archiving", archivePlanId.ToString());
                LogExtension.WriteVariables(new { EnableArchiving, mailboxPlanId });

                // load org quotas
                OrganizationStatistics orgStats = GetOrganizationStatisticsByOrganization(itemId);

                int maxDiskSpace = -1;
                int quotaUsed = 0;
                if (orgStats.AllocatedDiskSpace > 0)
                {
                    maxDiskSpace = orgStats.AllocatedDiskSpace;
                    quotaUsed = orgStats.UsedDiskSpace;
                }

                ExchangeMailboxPlan plan = GetExchangeMailboxPlan(itemId, mailboxPlanId);
                ExchangeAccount exchangeAccount = GetAccount(itemId, accountId);
                ExchangeMailboxPlan oldPlan = GetExchangeMailboxPlan(itemId, exchangeAccount.MailboxPlanId);

                // Log Extension
                LogExtension.WriteObject(plan);

                if (maxDiskSpace != -1)
                {
                    if (plan.MailboxSizeMB == -1)
                        return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;

                    var oldPlanValue = exchangeAccount.MailboxPlanId > 0 ? oldPlan.MailboxSizeMB : 0;

                    if (((quotaUsed - oldPlanValue) + plan.MailboxSizeMB) > maxDiskSpace)
                        return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;
                }

                if (plan.AllowLitigationHold)
                {
                    int maxRecoverableItemsSpace = -1;
                    int quotaRecoverableItemsUsed = 0;
                    if (orgStats.AllocatedLitigationHoldSpace > 0)
                    {
                        maxRecoverableItemsSpace = orgStats.AllocatedLitigationHoldSpace;
                        quotaRecoverableItemsUsed = orgStats.UsedLitigationHoldSpace;
                    }

                    if (maxRecoverableItemsSpace != -1)
                    {
                        if (plan.RecoverableItemsSpace == -1)
                            return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;

                        if (plan.RecoverableItemsSpace < 6144)
                            return BusinessErrorCodes.ERROR_EXCHANGE_INVALID_RECOVERABLEITEMS_QUOTA;

                        var oldPlanValue = exchangeAccount.MailboxPlanId > 0 && oldPlan.AllowLitigationHold ? oldPlan.RecoverableItemsSpace : 0;

                        if (((quotaRecoverableItemsUsed - oldPlanValue) + plan.RecoverableItemsSpace) > (maxRecoverableItemsSpace))
                            return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;
                    }
                }

                if (plan.EnableArchiving)
                {
                    int maxArchivingStorage = -1;
                    int quotaArchivingStorageUsed = 0;
                    if (orgStats.AllocatedArchingStorage > 0)
                    {
                        maxArchivingStorage = orgStats.AllocatedArchingStorage;
                        quotaArchivingStorageUsed = orgStats.UsedArchingStorage;
                    }

                    if (maxArchivingStorage != -1)
                    {
                        if (plan.ArchiveSizeMB == -1)
                            return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;

                        var oldPlanValue = exchangeAccount.MailboxPlanId > 0 && oldPlan.EnableArchiving ? oldPlan.ArchiveSizeMB : 0;

                        if (((quotaArchivingStorageUsed - oldPlanValue) + plan.ArchiveSizeMB) > maxArchivingStorage)
                            return BusinessErrorCodes.ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES;
                    }
                }

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                //TDMX
                exchange.SetMailboxAdvancedSettings(
                    org.OrganizationId,
                    account.UserPrincipalName,
                    plan.EnablePOP,
                    plan.EnableIMAP,
                    plan.EnableOWA,
                    plan.EnableMAPI,
                    plan.EnableActiveSync,
                    plan.MailboxSizeMB != -1 ? (((long)plan.IssueWarningPct * (long)plan.MailboxSizeMB * 1024) / 100) : -1,
                    plan.MailboxSizeMB != -1 ? (((long)plan.ProhibitSendPct * (long)plan.MailboxSizeMB * 1024) / 100) : -1,
                    plan.MailboxSizeMB != -1 ? (((long)plan.ProhibitSendReceivePct * (long)plan.MailboxSizeMB * 1024) / 100) : -1,
                    plan.KeepDeletedItemsDays,
                    plan.MaxRecipients,
                    plan.MaxSendMessageSizeKB,
                    plan.MaxReceiveMessageSizeKB,
                    plan.AllowLitigationHold,
                    plan.RecoverableItemsSpace != -1 ? (plan.RecoverableItemsSpace * 1024) : -1,
                    plan.RecoverableItemsSpace != -1 ? (((long)plan.RecoverableItemsWarningPct * (long)plan.RecoverableItemsSpace * 1024) / 100) : -1,
                    plan.LitigationHoldUrl,
                    plan.LitigationHoldMsg);

                ResultObject resPolicy = new ResultObject() { IsSuccess = true };
                SetMailBoxRetentionPolicyAndArchiving(itemId, mailboxPlanId, archivePlanId, account.UserPrincipalName, exchange, org.OrganizationId, resPolicy, EnableArchiving);
                if (!resPolicy.IsSuccess)
                {
                    TaskManager.WriteError("Error SetMailBoxRetentionPolicy: " + string.Join(", ", resPolicy.ErrorCodes.ToArray()));
                }

                Database.SetExchangeAccountMailboxPlan(accountId, mailboxPlanId, archivePlanId, EnableArchiving);

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

        public List<ExchangeMailboxPlan> GetExchangeMailboxPlans(int itemId, bool archiving)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_EXCHANGE_MAILBOXPLANS", itemId);

            try
            {
                List<ExchangeMailboxPlan> mailboxPlans = new List<ExchangeMailboxPlan>();
                int? defaultPlanId = null;

                UserInfo user = ObjectUtils.FillObjectFromDataReader<UserInfo>(Database.GetUserByExchangeOrganizationIdInternally(itemId));

                if (user.Role == UserRole.User)
                    GetExchangeMailboxPlansByUser(itemId, user, ref mailboxPlans, ref defaultPlanId, archiving);
                else
                    GetExchangeMailboxPlansByUser(0, user, ref mailboxPlans, ref defaultPlanId, archiving);

                if (defaultPlanId.HasValue)
                {
                    mailboxPlans.ForEach(p => p.IsDefault = (p.MailboxPlanId == defaultPlanId.Value));
                }

                return mailboxPlans;
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

        private void GetExchangeMailboxPlansByUser(int itemId, UserInfo user, ref List<ExchangeMailboxPlan> mailboxPlans, ref int? defaultPlanId, bool archiving)
        {
            if ((user != null))
            {
                List<Organization> orgs = null;

                if (user.UserId != 1)
                {
                    List<PackageInfo> Packages = PackageController.GetPackages(user.UserId);

                    if ((Packages != null) & (Packages.Count > 0))
                    {
                        orgs = GetExchangeOrganizationsInternal(Packages[0].PackageId, false);
                    }
                }
                else
                {
                    orgs = GetExchangeOrganizationsInternal(1, false);
                }

                int OrgId = -1;
                if (itemId > 0) OrgId = itemId;
                else if ((orgs != null) & (orgs.Count > 0)) OrgId = orgs[0].Id;


                if (OrgId != -1)
                {
                    List<ExchangeMailboxPlan> Plans = ObjectUtils.CreateListFromDataReader<ExchangeMailboxPlan>(Database.GetExchangeMailboxPlans(OrgId, archiving));

                    foreach (ExchangeMailboxPlan p in Plans)
                    {
                        mailboxPlans.Add(p);
                    }

                    // Set default plan
                    ExchangeOrganization exchangeOrg = ObjectUtils.FillObjectFromDataReader<ExchangeOrganization>(Database.GetExchangeOrganization(OrgId));

                    // If the default plan has not been set by the setting of higher priority 
                    if (!defaultPlanId.HasValue && exchangeOrg != null && exchangeOrg.ExchangeMailboxPlanID > 0)
                    {
                        defaultPlanId = exchangeOrg.ExchangeMailboxPlanID;
                    }
                }

                UserInfo owner = UserController.GetUserInternally(user.OwnerId);

                GetExchangeMailboxPlansByUser(0, owner, ref mailboxPlans, ref defaultPlanId, archiving);
            }
        }


        public ExchangeMailboxPlan GetExchangeMailboxPlan(int itemID, int mailboxPlanId)
        {

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_EXCHANGE_MAILBOXPLAN", mailboxPlanId);

            try
            {
                return ObjectUtils.FillObjectFromDataReader<ExchangeMailboxPlan>(
                    Database.GetExchangeMailboxPlan(mailboxPlanId));
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

        public int AddExchangeMailboxPlan(int itemID, ExchangeMailboxPlan mailboxPlan)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "ADD_EXCHANGE_MAILBOXPLAN_RETENTIONPOLICY_ARCHIVING", mailboxPlan.MailboxPlan, itemID);

            try
            {
                // Log Extension
                LogExtension.WriteObject(mailboxPlan);

                Organization org = GetOrganization(itemID);
                if (org == null)
                    return -1;

                // load package context
                PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                if (org.PackageId > 1)
                {
                    mailboxPlan.EnableActiveSync = mailboxPlan.EnableActiveSync & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_ACTIVESYNCALLOWED].QuotaAllocatedValue);
                    mailboxPlan.EnableIMAP = mailboxPlan.EnableIMAP & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_IMAPALLOWED].QuotaAllocatedValue);
                    mailboxPlan.EnableMAPI = mailboxPlan.EnableMAPI & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_MAPIALLOWED].QuotaAllocatedValue);
                    mailboxPlan.EnableOWA = mailboxPlan.EnableOWA & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_OWAALLOWED].QuotaAllocatedValue);
                    mailboxPlan.EnablePOP = mailboxPlan.EnablePOP & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_POP3ALLOWED].QuotaAllocatedValue);
                    mailboxPlan.EnableAutoReply = mailboxPlan.EnableAutoReply & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2013_AUTOREPLY].QuotaAllocatedValue);

                    if (cntx.Quotas[Quotas.EXCHANGE2007_KEEPDELETEDITEMSDAYS].QuotaAllocatedValue != -1)
                        if (mailboxPlan.KeepDeletedItemsDays > cntx.Quotas[Quotas.EXCHANGE2007_KEEPDELETEDITEMSDAYS].QuotaAllocatedValue)
                            mailboxPlan.KeepDeletedItemsDays = cntx.Quotas[Quotas.EXCHANGE2007_KEEPDELETEDITEMSDAYS].QuotaAllocatedValue;

                    if (mailboxPlan.Archiving)
                    {
                        if (cntx.Quotas[Quotas.EXCHANGE2013_ARCHIVINGSTORAGE].QuotaAllocatedValuePerOrganization != -1)
                            if (mailboxPlan.MailboxSizeMB > cntx.Quotas[Quotas.EXCHANGE2013_ARCHIVINGSTORAGE].QuotaAllocatedValuePerOrganization)
                                mailboxPlan.MailboxSizeMB = cntx.Quotas[Quotas.EXCHANGE2013_ARCHIVINGSTORAGE].QuotaAllocatedValuePerOrganization;
                    }
                    else
                    {
                        if (cntx.Quotas[Quotas.EXCHANGE2007_DISKSPACE].QuotaAllocatedValuePerOrganization != -1)
                            if (mailboxPlan.MailboxSizeMB > cntx.Quotas[Quotas.EXCHANGE2007_DISKSPACE].QuotaAllocatedValuePerOrganization)
                                mailboxPlan.MailboxSizeMB = cntx.Quotas[Quotas.EXCHANGE2007_DISKSPACE].QuotaAllocatedValuePerOrganization;
                    }

                    if (cntx.Quotas[Quotas.EXCHANGE2007_MAXRECEIVEMESSAGESIZEKB].QuotaAllocatedValue != -1)
                        if (mailboxPlan.MaxReceiveMessageSizeKB > cntx.Quotas[Quotas.EXCHANGE2007_MAXRECEIVEMESSAGESIZEKB].QuotaAllocatedValue)
                            mailboxPlan.MaxReceiveMessageSizeKB = cntx.Quotas[Quotas.EXCHANGE2007_MAXRECEIVEMESSAGESIZEKB].QuotaAllocatedValue;

                    if (cntx.Quotas[Quotas.EXCHANGE2007_MAXSENDMESSAGESIZEKB].QuotaAllocatedValue != -1)
                        if (mailboxPlan.MaxSendMessageSizeKB > cntx.Quotas[Quotas.EXCHANGE2007_MAXSENDMESSAGESIZEKB].QuotaAllocatedValue)
                            mailboxPlan.MaxSendMessageSizeKB = cntx.Quotas[Quotas.EXCHANGE2007_MAXSENDMESSAGESIZEKB].QuotaAllocatedValue;

                    if (cntx.Quotas[Quotas.EXCHANGE2007_MAXRECIPIENTS].QuotaAllocatedValue != -1)
                        if (mailboxPlan.MaxRecipients > cntx.Quotas[Quotas.EXCHANGE2007_MAXRECIPIENTS].QuotaAllocatedValue)
                            mailboxPlan.MaxRecipients = cntx.Quotas[Quotas.EXCHANGE2007_MAXRECIPIENTS].QuotaAllocatedValue;

                    if (Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_ISCONSUMER].QuotaAllocatedValue)) mailboxPlan.HideFromAddressBook = true;

                    mailboxPlan.AllowLitigationHold = mailboxPlan.AllowLitigationHold & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_ALLOWLITIGATIONHOLD].QuotaAllocatedValue);

                    if (cntx.Quotas[Quotas.EXCHANGE2007_RECOVERABLEITEMSSPACE].QuotaAllocatedValuePerOrganization != -1)
                        if (mailboxPlan.RecoverableItemsSpace > cntx.Quotas[Quotas.EXCHANGE2007_RECOVERABLEITEMSSPACE].QuotaAllocatedValuePerOrganization)
                            mailboxPlan.RecoverableItemsSpace = cntx.Quotas[Quotas.EXCHANGE2007_RECOVERABLEITEMSSPACE].QuotaAllocatedValuePerOrganization;
                }

                return Database.AddExchangeMailboxPlan(itemID, mailboxPlan.MailboxPlan, mailboxPlan.EnableActiveSync, mailboxPlan.EnableIMAP, mailboxPlan.EnableMAPI, mailboxPlan.EnableOWA, mailboxPlan.EnablePOP, mailboxPlan.EnableAutoReply,
                    mailboxPlan.IsDefault, mailboxPlan.IssueWarningPct, mailboxPlan.KeepDeletedItemsDays, mailboxPlan.MailboxSizeMB, mailboxPlan.MaxReceiveMessageSizeKB, mailboxPlan.MaxRecipients,
                    mailboxPlan.MaxSendMessageSizeKB, mailboxPlan.ProhibitSendPct, mailboxPlan.ProhibitSendReceivePct, mailboxPlan.HideFromAddressBook, mailboxPlan.MailboxPlanType,
                    mailboxPlan.AllowLitigationHold, mailboxPlan.RecoverableItemsSpace, mailboxPlan.RecoverableItemsWarningPct,
                    mailboxPlan.LitigationHoldUrl, mailboxPlan.LitigationHoldMsg, mailboxPlan.Archiving, mailboxPlan.EnableArchiving,
                    mailboxPlan.ArchiveSizeMB, mailboxPlan.ArchiveWarningPct, mailboxPlan.EnableForceArchiveDeletion, mailboxPlan.IsForJournaling);
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


        public int UpdateExchangeMailboxPlan(int itemID, ExchangeMailboxPlan mailboxPlan)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_EXCHANGE_MAILBOXPLAN", mailboxPlan.MailboxPlan, itemID);

            try
            {
                // Log Extension
                LogExtension.WriteObject(mailboxPlan);
                ExchangeMailboxPlan oldMailboxPlan = null;
                try
                {
                    oldMailboxPlan = ObjectUtils.Clone(mailboxPlan);
                }
                catch (Exception ex)
                {
                    TaskManager.Write("Can't clone mailbox: " + ex.Message);
                }

                Organization org = GetOrganization(itemID);
                if (org == null)
                    return -1;

                // load package context
                PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                if (org.PackageId > 1)
                {
                    mailboxPlan.EnableActiveSync = mailboxPlan.EnableActiveSync & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_ACTIVESYNCALLOWED].QuotaAllocatedValue);
                    mailboxPlan.EnableIMAP = mailboxPlan.EnableIMAP & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_IMAPALLOWED].QuotaAllocatedValue);
                    mailboxPlan.EnableMAPI = mailboxPlan.EnableMAPI & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_MAPIALLOWED].QuotaAllocatedValue);
                    mailboxPlan.EnableOWA = mailboxPlan.EnableOWA & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_OWAALLOWED].QuotaAllocatedValue);
                    mailboxPlan.EnablePOP = mailboxPlan.EnablePOP & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_POP3ALLOWED].QuotaAllocatedValue);
                    mailboxPlan.EnableAutoReply = mailboxPlan.EnableAutoReply & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2013_AUTOREPLY].QuotaAllocatedValue);

                    if (cntx.Quotas[Quotas.EXCHANGE2007_KEEPDELETEDITEMSDAYS].QuotaAllocatedValue != -1)
                        if (mailboxPlan.KeepDeletedItemsDays > cntx.Quotas[Quotas.EXCHANGE2007_KEEPDELETEDITEMSDAYS].QuotaAllocatedValue)
                            mailboxPlan.KeepDeletedItemsDays = cntx.Quotas[Quotas.EXCHANGE2007_KEEPDELETEDITEMSDAYS].QuotaAllocatedValue;

                    if (cntx.Quotas[Quotas.EXCHANGE2007_DISKSPACE].QuotaAllocatedValuePerOrganization != -1)
                        if (mailboxPlan.MailboxSizeMB > cntx.Quotas[Quotas.EXCHANGE2007_DISKSPACE].QuotaAllocatedValuePerOrganization)
                            mailboxPlan.MailboxSizeMB = cntx.Quotas[Quotas.EXCHANGE2007_DISKSPACE].QuotaAllocatedValuePerOrganization;

                    if (cntx.Quotas[Quotas.EXCHANGE2007_MAXRECEIVEMESSAGESIZEKB].QuotaAllocatedValue != -1)
                        if (mailboxPlan.MaxReceiveMessageSizeKB > cntx.Quotas[Quotas.EXCHANGE2007_MAXRECEIVEMESSAGESIZEKB].QuotaAllocatedValue)
                            mailboxPlan.MaxReceiveMessageSizeKB = cntx.Quotas[Quotas.EXCHANGE2007_MAXRECEIVEMESSAGESIZEKB].QuotaAllocatedValue;

                    if (cntx.Quotas[Quotas.EXCHANGE2007_MAXSENDMESSAGESIZEKB].QuotaAllocatedValue != -1)
                        if (mailboxPlan.MaxSendMessageSizeKB > cntx.Quotas[Quotas.EXCHANGE2007_MAXSENDMESSAGESIZEKB].QuotaAllocatedValue)
                            mailboxPlan.MaxSendMessageSizeKB = cntx.Quotas[Quotas.EXCHANGE2007_MAXSENDMESSAGESIZEKB].QuotaAllocatedValue;

                    if (cntx.Quotas[Quotas.EXCHANGE2007_MAXRECIPIENTS].QuotaAllocatedValue != -1)
                        if (mailboxPlan.MaxRecipients > cntx.Quotas[Quotas.EXCHANGE2007_MAXRECIPIENTS].QuotaAllocatedValue)
                            mailboxPlan.MaxRecipients = cntx.Quotas[Quotas.EXCHANGE2007_MAXRECIPIENTS].QuotaAllocatedValue;

                    if (Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_ISCONSUMER].QuotaAllocatedValue)) mailboxPlan.HideFromAddressBook = true;

                    mailboxPlan.AllowLitigationHold = mailboxPlan.AllowLitigationHold & Convert.ToBoolean(cntx.Quotas[Quotas.EXCHANGE2007_ALLOWLITIGATIONHOLD].QuotaAllocatedValue);

                    if (cntx.Quotas[Quotas.EXCHANGE2007_RECOVERABLEITEMSSPACE].QuotaAllocatedValuePerOrganization != -1)
                        if (mailboxPlan.RecoverableItemsSpace > cntx.Quotas[Quotas.EXCHANGE2007_RECOVERABLEITEMSSPACE].QuotaAllocatedValuePerOrganization)
                            mailboxPlan.RecoverableItemsSpace = cntx.Quotas[Quotas.EXCHANGE2007_RECOVERABLEITEMSSPACE].QuotaAllocatedValuePerOrganization;

                }

                Database.UpdateExchangeMailboxPlan(mailboxPlan.MailboxPlanId, mailboxPlan.MailboxPlan, mailboxPlan.EnableActiveSync, mailboxPlan.EnableIMAP, mailboxPlan.EnableMAPI, mailboxPlan.EnableOWA, mailboxPlan.EnablePOP, mailboxPlan.EnableAutoReply,
                    mailboxPlan.IsDefault, mailboxPlan.IssueWarningPct, mailboxPlan.KeepDeletedItemsDays, mailboxPlan.MailboxSizeMB, mailboxPlan.MaxReceiveMessageSizeKB, mailboxPlan.MaxRecipients,
                    mailboxPlan.MaxSendMessageSizeKB, mailboxPlan.ProhibitSendPct, mailboxPlan.ProhibitSendReceivePct, mailboxPlan.HideFromAddressBook, mailboxPlan.MailboxPlanType,
                    mailboxPlan.AllowLitigationHold, mailboxPlan.RecoverableItemsSpace, mailboxPlan.RecoverableItemsWarningPct,
                    mailboxPlan.LitigationHoldUrl, mailboxPlan.LitigationHoldMsg, mailboxPlan.Archiving, mailboxPlan.EnableArchiving,
                    mailboxPlan.ArchiveSizeMB, mailboxPlan.ArchiveWarningPct, mailboxPlan.EnableForceArchiveDeletion, mailboxPlan.IsForJournaling);

                // Log Extension
                LogExtension.LogPropertiesIfChanged(oldMailboxPlan, mailboxPlan);

            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }


            return 0;
        }



        public int DeleteExchangeMailboxPlan(int itemID, int mailboxPlanId)
        {
            TaskManager.StartTask("EXCHANGE", "DELETE_EXCHANGE_MAILBOXPLAN_RETENTIONPOLICY_ARCHIVING", itemID);

            try
            {
                Organization org = GetOrganization(itemID); // To write log
                if (org == null)
                    return -1;

                var mailboxPlan = GetExchangeMailboxPlan(itemID, mailboxPlanId);
                if (mailboxPlan != null)
                {
                    // Log Extension
                    LogExtension.SetItemName(mailboxPlan.MailboxPlan);
                    LogExtension.WriteObject(mailboxPlan);
                }

                Database.DeleteExchangeMailboxPlan(mailboxPlanId);

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

        public void SetOrganizationDefaultExchangeMailboxPlan(int itemId, int mailboxPlanId)
        {
            TaskManager.StartTask("EXCHANGE", "SET_EXCHANGE_MAILBOXPLAN", itemId);

            try
            {
                // Log Extension
                var org = GetOrganization(itemId);
                if (org != null) LogExtension.SetItemName(org.Name);
                ExchangeMailboxPlan plan = GetExchangeMailboxPlan(itemId, mailboxPlanId);
                LogExtension.WriteObject(plan);

                Database.SetOrganizationDefaultExchangeMailboxPlan(itemId, mailboxPlanId);
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

        #region Exchange Retention Policy Tags

        private void SetMailBoxRetentionPolicyAndArchiving(int itemId, int mailboxPlanId, int retentionPolicyId, string accountName, ExchangeServer exchange, string orgId, ResultObject result, bool EnableArchiving)
        {
            // Log Extension
            LogExtension.WriteVariables(new { retentionPolicyId });

            long archiveQuotaKB = 0;
            long archiveWarningQuotaKB = 0;
            string RetentionPolicy = "";

            ExchangeMailboxPlan mailboxPlan = GetExchangeMailboxPlan(itemId, mailboxPlanId);
            if (mailboxPlan != null)
            {
                archiveQuotaKB = mailboxPlan.ArchiveSizeMB != -1 ? ((long)mailboxPlan.ArchiveSizeMB * 1024) : -1;
                archiveWarningQuotaKB = mailboxPlan.ArchiveSizeMB != -1 ? (((long)mailboxPlan.ArchiveWarningPct * (long)mailboxPlan.ArchiveSizeMB * 1024) / 100) : -1;
            }


            if (retentionPolicyId > 0)
            {
                ExchangeMailboxPlan retentionPolicy = GetExchangeMailboxPlan(itemId, retentionPolicyId);
                if (retentionPolicy != null)
                {
                    UpdateExchangeRetentionPolicy(itemId, retentionPolicyId, result);
                    RetentionPolicy = retentionPolicy.FCPUniqueName;
                }

            }

            if (retentionPolicyId <= -1)
            {
                ExchangeMailboxPlan retentionPolicy = GetExchangeMailboxPlan(itemId, retentionPolicyId);
                if (retentionPolicy != null)
                {
                    RetentionPolicy = null;
                }

            }

            ResultObject res = exchange.SetMailBoxArchiving(orgId, accountName, EnableArchiving, archiveQuotaKB, archiveWarningQuotaKB, RetentionPolicy);
            if (res != null)
            {
                result.ErrorCodes.AddRange(res.ErrorCodes);
                result.IsSuccess = result.IsSuccess && res.IsSuccess;
            }
        }

        public List<ExchangeRetentionPolicyTag> GetExchangeRetentionPolicyTags(int itemId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_EXCHANGE_RETENTIONPOLICYTAGS", itemId);

            try
            {
                List<ExchangeRetentionPolicyTag> retentionPolicyTags = new List<ExchangeRetentionPolicyTag>();

                UserInfo user = ObjectUtils.FillObjectFromDataReader<UserInfo>(Database.GetUserByExchangeOrganizationIdInternally(itemId));

                if (user.Role == UserRole.User)
                    ExchangeServerController.GetExchangeRetentionPolicyTagsByUser(itemId, user, ref retentionPolicyTags);
                else
                    ExchangeServerController.GetExchangeRetentionPolicyTagsByUser(0, user, ref retentionPolicyTags);

                return retentionPolicyTags;
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

        private void GetExchangeRetentionPolicyTagsByUser(int itemId, UserInfo user, ref List<ExchangeRetentionPolicyTag> retentionPolicyTags)
        {
            if ((user != null))
            {
                List<Organization> orgs = null;

                if (user.UserId != 1)
                {
                    List<PackageInfo> Packages = PackageController.GetPackages(user.UserId);

                    if ((Packages != null) & (Packages.Count > 0))
                    {
                        orgs = GetExchangeOrganizationsInternal(Packages[0].PackageId, false);
                    }
                }
                else
                {
                    orgs = GetExchangeOrganizationsInternal(1, false);
                }

                int OrgId = -1;
                if (itemId > 0) OrgId = itemId;
                else if ((orgs != null) & (orgs.Count > 0)) OrgId = orgs[0].Id;


                if (OrgId != -1)
                {
                    List<ExchangeRetentionPolicyTag> RetentionPolicy = ObjectUtils.CreateListFromDataReader<ExchangeRetentionPolicyTag>(Database.GetExchangeRetentionPolicyTags(OrgId));

                    foreach (ExchangeRetentionPolicyTag p in RetentionPolicy)
                    {
                        retentionPolicyTags.Add(p);
                    }
                }

                UserInfo owner = UserController.GetUserInternally(user.OwnerId);

                GetExchangeRetentionPolicyTagsByUser(0, owner, ref retentionPolicyTags);
            }
        }

        public ExchangeRetentionPolicyTag GetExchangeRetentionPolicyTag(int itemID, int tagId)
        {

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_EXCHANGE_RETENTIONPOLICYTAG", tagId);

            try
            {
                return ObjectUtils.FillObjectFromDataReader<ExchangeRetentionPolicyTag>(
                    Database.GetExchangeRetentionPolicyTag(tagId));
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

        public IntResult AddExchangeRetentionPolicyTag(int itemID, ExchangeRetentionPolicyTag tag)
        {
            // place log record
            IntResult res = TaskManager.StartResultTask<IntResult>("EXCHANGE", "ADD_EXCHANGE_RETENTIONPOLICYTAG", tag.TagName, itemID);

            Organization org;
            try
            {
                org = GetOrganization(itemID);
                if (org == null)
                    throw new ApplicationException("Organization is null");
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ORGANIZATION_BY_ITEM_ID, ex);
                return res;
            }

            try
            {
                // Log Extension
                LogExtension.WriteObject(tag);

                // load package context
                PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                if (org.PackageId > 1)
                {
                    // quotas
                }

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);

                int tagId = Database.AddExchangeRetentionPolicyTag(itemID, tag.TagName, tag.TagType, tag.AgeLimitForRetention, tag.RetentionAction);
                tag.TagID = tagId;

                if (exchangeServiceId > 0)
                {
                    ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                    ResultObject resTag = exchange.SetRetentionPolicyTag(tag.FCPUniqueName, (ExchangeRetentionPolicyTagType)tag.TagType, tag.AgeLimitForRetention, (ExchangeRetentionPolicyTagAction)tag.RetentionAction);
                    res.ErrorCodes.AddRange(resTag.ErrorCodes);
                    res.IsSuccess = res.IsSuccess && resTag.IsSuccess;
                }

                if (res.IsSuccess)
                    res.Value = tagId;
                else
                    Database.DeleteExchangeRetentionPolicyTag(tagId);
            }
            catch (Exception ex)
            {
                TaskManager.WriteError(ex);
                TaskManager.CompleteResultTask(res);
                return res;
            }

            TaskManager.CompleteResultTask();
            return res;
        }

        public ResultObject UpdateExchangeRetentionPolicyTag(int itemID, ExchangeRetentionPolicyTag tag)
        {
            // place log record
            ResultObject res = TaskManager.StartResultTask<ResultObject>("EXCHANGE", "UPDATE_EXCHANGE_RETENTIONPOLICYTAG", itemID);

            Organization org;
            try
            {
                org = GetOrganization(itemID);
                if (org == null)
                    throw new ApplicationException("Organization is null");
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ORGANIZATION_BY_ITEM_ID, ex);
                return res;
            }

            try
            {
                // Log Extension
                LogExtension.WriteObject(tag);
                var oldObj = GetExchangeRetentionPolicyTag(itemID, tag.TagID);
                LogExtension.LogPropertiesIfChanged(oldObj, tag);
                LogExtension.SetItemName(tag.TagName);

                // load package context
                PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                if (org.PackageId > 1)
                {
                    // quotas
                }

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);

                if (exchangeServiceId > 0)
                {
                    ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                    ResultObject resTag = exchange.SetRetentionPolicyTag(tag.FCPUniqueName, (ExchangeRetentionPolicyTagType)tag.TagType, tag.AgeLimitForRetention, (ExchangeRetentionPolicyTagAction)tag.RetentionAction);
                    res.ErrorCodes.AddRange(resTag.ErrorCodes);
                    res.IsSuccess = res.IsSuccess && resTag.IsSuccess;
                }

                if (res.IsSuccess)
                    Database.UpdateExchangeRetentionPolicyTag(tag.TagID, tag.ItemID, tag.TagName, tag.TagType, tag.AgeLimitForRetention, tag.RetentionAction);
            }
            catch (Exception ex)
            {
                TaskManager.WriteError(ex);
                TaskManager.CompleteResultTask(res);
                return res;
            }

            TaskManager.CompleteResultTask();
            return res;
        }

        public ResultObject DeleteExchangeRetentionPolicyTag(int itemID, int tagId)
        {
            ResultObject res = TaskManager.StartResultTask<ResultObject>("EXCHANGE", "DELETE_EXCHANGE_RETENTIONPOLICYTAG", itemID);

            Organization org;
            try
            {
                org = GetOrganization(itemID);
                if (org == null)
                    throw new ApplicationException("Organization is null");
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ORGANIZATION_BY_ITEM_ID, ex);
                return res;
            }

            try
            {
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                if (exchangeServiceId > 0)
                {
                    // load package context
                    PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                    if (org.PackageId > 1)
                    {
                        // quotas
                    }

                    ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                    ExchangeRetentionPolicyTag tag = GetExchangeRetentionPolicyTag(itemID, tagId);
                    if (tag == null) throw new ApplicationException("Tag is null");

                    // Log Extension
                    LogExtension.SetItemName(tag.TagName);
                    LogExtension.WriteObject(tag);

                    ResultObject resTag = exchange.RemoveRetentionPolicyTag(tag.FCPUniqueName);
                    res.ErrorCodes.AddRange(resTag.ErrorCodes);
                    res.IsSuccess = res.IsSuccess && resTag.IsSuccess;

                }

                if (res.IsSuccess)
                    Database.DeleteExchangeRetentionPolicyTag(tagId);

            }
            catch (Exception ex)
            {
                TaskManager.WriteError(ex);
                TaskManager.CompleteResultTask(res);
                return res;
            }

            TaskManager.CompleteResultTask();
            return res;
        }


        public List<ExchangeMailboxPlanRetentionPolicyTag> GetExchangeMailboxPlanRetentionPolicyTags(int policyId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_EXCHANGE_RETENTIONPOLICYTAGS", policyId);

            try
            {
                List<ExchangeMailboxPlanRetentionPolicyTag> tags =
                    ObjectUtils.CreateListFromDataReader<ExchangeMailboxPlanRetentionPolicyTag>(Database.GetExchangeMailboxPlanRetentionPolicyTags(policyId));

                return tags;
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


        private void UpdateExchangeRetentionPolicy(int itemID, int policyId, ResultObject result)
        {
            Organization org = GetOrganization(itemID);
            if (org == null)
                return;

            int exchangeServiceId = GetExchangeServiceID(org.PackageId);

            if (exchangeServiceId > 0)
            {
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                ExchangeMailboxPlan policy = GetExchangeMailboxPlan(itemID, policyId);

                if (policy != null)
                {
                    List<ExchangeMailboxPlanRetentionPolicyTag> policytaglist = GetExchangeMailboxPlanRetentionPolicyTags(policyId);

                    List<string> tagLinks = new List<string>();

                    foreach (ExchangeMailboxPlanRetentionPolicyTag policytag in policytaglist)
                    {
                        ExchangeRetentionPolicyTag tag = GetExchangeRetentionPolicyTag(itemID, policytag.TagID);
                        tagLinks.Add(tag.FCPUniqueName);

                        // update PlanRetentionPolicyTags

                        ResultObject resItem = exchange.SetRetentionPolicyTag(tag.FCPUniqueName, (ExchangeRetentionPolicyTagType)tag.TagType, tag.AgeLimitForRetention, (ExchangeRetentionPolicyTagAction)tag.RetentionAction);
                        result.ErrorCodes.AddRange(resItem.ErrorCodes);
                        result.IsSuccess = result.IsSuccess && resItem.IsSuccess;
                    }

                    ResultObject res = exchange.SetRetentionPolicy(policy.FCPUniqueName, tagLinks.ToArray());
                    result.ErrorCodes.AddRange(res.ErrorCodes);
                    result.IsSuccess = result.IsSuccess && res.IsSuccess;
                }

            }
        }

        public IntResult AddExchangeMailboxPlanRetentionPolicyTag(int itemID, ExchangeMailboxPlanRetentionPolicyTag planTag)
        {
            // place log record
            IntResult res = TaskManager.StartResultTask<IntResult>("EXCHANGE", "ADD_EXCHANGE_RETENTIONPOLICYTAG", planTag.TagName, itemID);

            Organization org;
            try
            {
                org = GetOrganization(itemID);
                if (org == null)
                    throw new ApplicationException("Organization is null");
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ORGANIZATION_BY_ITEM_ID, ex);
                return res;
            }

            try
            {
                // Log Extension
                LogExtension.WriteObject(planTag);

                // load package context
                PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                if (org.PackageId > 1)
                {
                    // quotas
                }

                res.Value = Database.AddExchangeMailboxPlanRetentionPolicyTag(planTag.TagID, planTag.MailboxPlanId);

                UpdateExchangeRetentionPolicy(itemID, planTag.MailboxPlanId, res);

            }
            catch (Exception ex)
            {
                TaskManager.WriteError(ex);
                TaskManager.CompleteResultTask(res);
                return res;
            }

            TaskManager.CompleteResultTask();
            return res;

        }

        public ResultObject DeleteExchangeMailboxPlanRetentionPolicyTag(int itemID, int policyId, int planTagId)
        {
            ResultObject res = TaskManager.StartResultTask<ResultObject>("EXCHANGE", "DELETE_EXCHANGE_RETENTIONPOLICYTAG", itemID);

            Organization org;
            try
            {
                org = GetOrganization(itemID);
                if (org == null)
                    throw new ApplicationException("Organization is null");
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ORGANIZATION_BY_ITEM_ID, ex);
                return res;
            }

            try
            {
                // load package context
                PackageContext cntx = PackageController.GetPackageContext(org.PackageId);

                if (org.PackageId > 1)
                {
                    // quotas
                }

                Database.DeleteExchangeMailboxPlanRetentionPolicyTag(planTagId);

                UpdateExchangeRetentionPolicy(itemID, policyId, res);
            }
            catch (Exception ex)
            {
                TaskManager.WriteError(ex);
                TaskManager.CompleteResultTask(res);
                return res;
            }

            TaskManager.CompleteResultTask();
            return res;

        }

        #endregion

        #region Contacts
        public int CreateContact(int itemId, string displayName, string email)
        {
            //if (EmailAddressExists(email))
            //  return BusinessErrorCodes.ERROR_EXCHANGE_EMAIL_EXISTS;


            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // check mailbox quota
            OrganizationStatistics orgStats = GetOrganizationStatisticsByOrganization(itemId);
            if (orgStats.AllocatedContacts > -1
                && orgStats.CreatedContacts >= orgStats.AllocatedContacts)
                return BusinessErrorCodes.ERROR_EXCHANGE_CONTACTS_QUOTA_LIMIT;

            // place log record
            TaskManager.StartTask("EXCHANGE", "CREATE_CONTACT", displayName, itemId);

            try
            {
                displayName = displayName.Trim();
                email = email.Trim();

                // load organization
                Organization org = GetOrganization(itemId);

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                string name = email;
                int idx = email.IndexOf("@");
                if (idx > -1)
                    name = email.Substring(0, idx);

                string accountName = OrganizationController.BuildAccountNameEx(org, name);

                // add contact
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                //Create Exchange Organization
                if (string.IsNullOrEmpty(org.GlobalAddressList))
                {
                    ExtendToExchangeOrganization(ref org);

                    PackageController.UpdatePackageItem(org);
                }

                exchange.CreateContact(
                    org.OrganizationId,
                    org.DistinguishedName,
                    displayName,
                    accountName,
                    email, org.DefaultDomain);

                ExchangeContact contact = exchange.GetContactGeneralSettings(accountName);

                // Log Extension
                LogExtension.WriteObject(contact);

                // add meta-item
                int accountId = AddAccount(itemId, ExchangeAccountType.Contact, accountName,
                    displayName, email, false,
                    0, contact.SAMAccountName, null, 0, null);

                // Log Extension
                LogExtension.WriteVariables(new { accountId });

                return accountId;
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

        public int DeleteContact(int itemId, int accountId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DELETE_CONTACT", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.DisplayName);

                // delete contact
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.DeleteContact(account.AccountName);

                // remove meta-item
                DeleteAccount(itemId, accountId);

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

        private ExchangeContact GetDemoContactSettings()
        {
            ExchangeContact c = new ExchangeContact();
            c.DisplayName = "FuseCP Support";
            c.AccountName = "fcp_fabrikam";
            c.FirstName = "FuseCP";
            c.LastName = "Support";
            c.EmailAddress = "support@fusecp.com";
            c.AcceptAccounts = GetAccounts(0, ExchangeAccountType.Mailbox).ToArray();
            return c;
        }

        public ExchangeContact GetContactGeneralSettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoContactSettings();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_CONTACT_GENERAL", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                return exchange.GetContactGeneralSettings(account.AccountName);
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

        public int SetContactGeneralSettings(int itemId, int accountId, string displayName, string emailAddress,
            bool hideAddressBook, string firstName, string initials,
            string lastName, string address, string city, string state, string zip, string country,
            string jobTitle, string company, string department, string office, string managerAccountName,
            string businessPhone, string fax, string homePhone, string mobilePhone, string pager,
            string webPage, string notes, int useMapiRichTextFormat)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_CONTACT_GENERAL", displayName, itemId);

            try
            {
                displayName = displayName.Trim();
                emailAddress = emailAddress.Trim();
                firstName = firstName.Trim();
                lastName = lastName.Trim();

                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                var oldContact = exchange.GetContactGeneralSettings(account.AccountName);

                exchange.SetContactGeneralSettings(
                    account.AccountName,
                    displayName,
                    emailAddress,
                    hideAddressBook,
                    firstName,
                    initials,
                    lastName,
                    address,
                    city,
                    state,
                    zip,
                    country,
                    jobTitle,
                    company,
                    department,
                    office,
                    managerAccountName,
                    businessPhone,
                    fax,
                    homePhone,
                    mobilePhone,
                    pager,
                    webPage,
                    notes,
                    useMapiRichTextFormat, org.DefaultDomain);

                var newContact = exchange.GetContactGeneralSettings(account.AccountName);

                // Log Extension
                account.LogPropertyIfChanged(a => a.DisplayName, displayName);
                account.LogPropertyIfChanged(a => a.PrimaryEmailAddress, emailAddress);
                LogExtension.LogPropertiesIfChanged(oldContact, newContact);

                // update account
                account.DisplayName = displayName;
                account.PrimaryEmailAddress = emailAddress;
                UpdateAccount(account);

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

        public ExchangeContact GetContactMailFlowSettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoContactSettings();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_CONTACT_MAILFLOW", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                ExchangeContact contact = exchange.GetContactMailFlowSettings(account.AccountName);
                contact.DisplayName = account.DisplayName;
                return contact;
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

        public int SetContactMailFlowSettings(int itemId, int accountId,
            string[] acceptAccounts, string[] rejectAccounts, bool requireSenderAuthentication)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_CONTACT_MAILFLOW", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.DisplayName);
                LogExtension.WriteVariables(new { acceptAccounts, rejectAccounts, requireSenderAuthentication });

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                var oldObj = exchange.GetContactMailFlowSettings(account.AccountName);

                exchange.SetContactMailFlowSettings(account.AccountName,
                    acceptAccounts,
                    rejectAccounts,
                    requireSenderAuthentication);

                var newObj = exchange.GetContactMailFlowSettings(account.AccountName);

                // Log Extension
                LogExtension.LogPropertiesIfChanged(oldObj, newObj);

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

        #region Distribution Lists
        public int CreateDistributionList(int itemId, string displayName, string name, string domain, int managerId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // check mailbox quota
            OrganizationStatistics orgStats = GetOrganizationStatisticsByOrganization(itemId);
            if (orgStats.AllocatedDistributionLists > -1
                && orgStats.CreatedDistributionLists >= orgStats.AllocatedDistributionLists)
                return BusinessErrorCodes.ERROR_EXCHANGE_DLISTS_QUOTA_LIMIT;

            // place log record
            TaskManager.StartTask("EXCHANGE", "CREATE_DISTR_LIST", displayName, itemId);

            try
            {
                displayName = displayName.Trim();
                name = name.Trim();
                domain = domain.Trim();

                // e-mail
                string email = name + "@" + domain;

                // Log Extension
                LogExtension.WriteVariables(new { email });

                // check e-mail
                if (EmailAddressExists(email, true))
                    return BusinessErrorCodes.ERROR_EXCHANGE_EMAIL_EXISTS;

                // load organization
                Organization org = GetOrganization(itemId);

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                string accountName = OrganizationController.BuildAccountNameEx(org, name);

                // add account
                // add contact
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                //Create Exchange Organization
                if (string.IsNullOrEmpty(org.GlobalAddressList))
                {
                    ExtendToExchangeOrganization(ref org);

                    PackageController.UpdatePackageItem(org);
                }

                OrganizationUser manager = OrganizationController.GetAccount(itemId, managerId);

                List<string> addressLists = new List<string>();
                addressLists.Add(org.GlobalAddressList);
                addressLists.Add(org.AddressList);

                exchange.CreateDistributionList(
                    org.OrganizationId,
                    org.DistinguishedName,
                    displayName,
                    accountName,
                    name,
                    domain, manager.SamAccountName, addressLists.ToArray());

                ExchangeDistributionList dl = exchange.GetDistributionListGeneralSettings(accountName);

                // add meta-item
                int accountId = AddAccount(itemId, ExchangeAccountType.DistributionList, accountName,
                    displayName, email, false,
                    0, dl.SAMAccountName, null, 0, null);

                // register email address
                AddAccountEmailAddress(accountId, email);

                return accountId;
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

        public int DeleteDistributionList(int itemId, int accountId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DELETE_DISTR_LIST", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.PrimaryEmailAddress);

                // delete mailbox
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.DeleteDistributionList(account.AccountName);

                // unregister account
                DeleteAccount(itemId, accountId);

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

        private ExchangeDistributionList GetDemoDistributionListSettings()
        {
            ExchangeDistributionList c = new ExchangeDistributionList();
            c.DisplayName = "Fabrikam Sales";
            c.AccountName = "sales_fabrikam";
            c.ManagerAccount = GetAccounts(0, ExchangeAccountType.Mailbox)[0];
            c.MembersAccounts = GetAccounts(0, ExchangeAccountType.Mailbox).ToArray();
            c.AcceptAccounts = GetAccounts(0, ExchangeAccountType.Mailbox).ToArray();
            return c;
        }

        public ExchangeDistributionList GetDistributionListGeneralSettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoDistributionListSettings();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_DISTR_LIST_GENERAL", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                return exchange.GetDistributionListGeneralSettings(account.AccountName);
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

        public int SetDistributionListGeneralSettings(int itemId, int accountId, string displayName,
            bool hideAddressBook, string managerAccount, string[] memberAccounts,
            string notes)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_DISTR_LIST_GENERAL", itemId);

            try
            {
                displayName = displayName.Trim();

                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);
                LogExtension.WriteVariables(new { managerAccount, memberAccounts, notes });

                // Log Extension
                LogExtension.SetItemName(account.PrimaryEmailAddress);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                List<string> addressLists = new List<string>();
                addressLists.Add(org.GlobalAddressList);
                addressLists.Add(org.AddressList);

                var oldObj = exchange.GetDistributionListGeneralSettings(account.AccountName);

                exchange.SetDistributionListGeneralSettings(
                    account.AccountName,
                    displayName,
                    hideAddressBook,
                    managerAccount,
                    memberAccounts,
                    notes,
                    addressLists.ToArray());

                var newObj = exchange.GetDistributionListGeneralSettings(account.AccountName);

                // Log Extension
                account.LogPropertyIfChanged(a => a.DisplayName, displayName);
                LogExtension.LogPropertiesIfChanged(oldObj, newObj);

                // update account
                account.DisplayName = displayName;
                UpdateAccount(account);

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

        public ExchangeDistributionList GetDistributionListMailFlowSettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoDistributionListSettings();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_DISTR_LIST_MAILFLOW", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                ExchangeDistributionList list = exchange.GetDistributionListMailFlowSettings(account.AccountName);
                list.DisplayName = account.DisplayName;
                return list;
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

        public int SetDistributionListMailFlowSettings(int itemId, int accountId,
            string[] acceptAccounts, string[] rejectAccounts, bool requireSenderAuthentication)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_DISTR_LIST_MAILFLOW", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.PrimaryEmailAddress);
                LogExtension.WriteVariables(new { acceptAccounts, rejectAccounts, requireSenderAuthentication });

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                List<string> addressLists = new List<string>();
                addressLists.Add(org.GlobalAddressList);
                addressLists.Add(org.AddressList);

                var oldObj = exchange.GetDistributionListMailFlowSettings(account.AccountName);

                exchange.SetDistributionListMailFlowSettings(account.AccountName,
                    acceptAccounts,
                    rejectAccounts,
                    requireSenderAuthentication,
                    addressLists.ToArray());

                var newObj = exchange.GetDistributionListMailFlowSettings(account.AccountName);

                // Log Extension
                LogExtension.LogPropertiesIfChanged(oldObj, newObj);

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

        public ExchangeEmailAddress[] GetDistributionListEmailAddresses(int itemId, int accountId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_DISTR_LIST_ADDRESSES", itemId);

            try
            {
                return GetAccountEmailAddresses(itemId, accountId);
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

        public int AddDistributionListEmailAddress(int itemId, int accountId, string emailAddress)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "ADD_DISTR_LIST_ADDRESS", itemId);

            try
            {
                // check
                if (EmailAddressExists(emailAddress, true))
                    return BusinessErrorCodes.ERROR_EXCHANGE_EMAIL_EXISTS;

                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);


                // add e-mail
                AddAccountEmailAddress(accountId, emailAddress);

                // update e-mail addresses
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                List<string> addressLists = new List<string>();
                addressLists.Add(org.GlobalAddressList);
                addressLists.Add(org.AddressList);

                // Log Extension
                LogExtension.SetItemName(account.PrimaryEmailAddress);
                LogExtension.WriteVariable("Address Added", emailAddress);

                exchange.SetDistributionListEmailAddresses(
                    account.AccountName,
                    GetAccountSimpleEmailAddresses(itemId, accountId), addressLists.ToArray());

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

        public int SetDistributionListPrimaryEmailAddress(int itemId, int accountId, string emailAddress)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "SET_PRIMARY_DISTR_LIST_ADDRESS", itemId);

            try
            {
                // get account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.UserPrincipalName);
                account.LogPropertyIfChanged(a => a.PrimaryEmailAddress, emailAddress);

                account.PrimaryEmailAddress = emailAddress;

                // update exchange
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                List<string> addressLists = new List<string>();
                addressLists.Add(org.GlobalAddressList);
                addressLists.Add(org.AddressList);

                exchange.SetDistributionListPrimaryEmailAddress(
                    account.AccountName,
                    emailAddress,
                    addressLists.ToArray());

                // save account
                UpdateAccount(account);

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

        public int DeleteDistributionListEmailAddresses(int itemId, int accountId, string[] emailAddresses)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DELETE_DISTR_LIST_ADDRESSES", itemId);

            try
            {
                // get account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // delete e-mail addresses
                List<string> toDelete = new List<string>();
                foreach (string emailAddress in emailAddresses)
                {
                    if (String.Compare(account.PrimaryEmailAddress, emailAddress, true) != 0)
                        toDelete.Add(emailAddress);
                }
                var emailAddressesDeleted = toDelete.ToArray();

                // Log Extension
                LogExtension.SetItemName(account.PrimaryEmailAddress);
                LogExtension.WriteVariables(new { emailAddressesDeleted });

                // delete from meta-base
                DeleteAccountEmailAddresses(accountId, emailAddressesDeleted);

                // delete from Exchange
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // update e-mail addresses
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                List<string> addressLists = new List<string>();
                addressLists.Add(org.GlobalAddressList);
                addressLists.Add(org.AddressList);

                exchange.SetDistributionListEmailAddresses(
                    account.AccountName,
                    GetAccountSimpleEmailAddresses(itemId, accountId), addressLists.ToArray());

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


        public ResultObject SetDistributionListPermissions(int itemId, int accountId, string[] sendAsAccounts, string[] sendOnBehalfAccounts)
        {
            ResultObject res = TaskManager.StartResultTask<ResultObject>("EXCHANGE", "SET_DISTRIBUTION_LIST_PERMISSINS");
            Organization org;
            try
            {
                org = GetOrganization(itemId);
                if (org == null)
                    throw new ApplicationException("Organization is null");
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ORGANIZATION_BY_ITEM_ID, ex);
                return res;
            }

            ExchangeServer exchange;
            try
            {

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ORGANIZATION_PROXY, ex);
                return res;
            }

            ExchangeAccount account;

            try
            {
                account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.UserPrincipalName);
                LogExtension.WriteVariables(new { sendAsAccounts, sendOnBehalfAccounts });
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ACCOUNT, ex);
                return res;
            }

            try
            {
                List<string> addressLists = new List<string>();
                addressLists.Add(org.GlobalAddressList);
                addressLists.Add(org.AddressList);

                exchange.SetDistributionListPermissions(org.OrganizationId, account.AccountName, sendAsAccounts,
                                                        sendOnBehalfAccounts, addressLists.ToArray());
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_SET_DISTRIBUTION_LIST_PERMISSIONS, ex);
                return res;
            }

            TaskManager.CompleteTask();
            return res;
        }

        public ExchangeDistributionListResult GetDistributionListPermissions(int itemId, int accountId)
        {
            Organization org;
            ExchangeDistributionListResult res = TaskManager.StartResultTask<ExchangeDistributionListResult>("EXCHANGE", "GET_DISTRIBUTION_LIST_RESULT");

            try
            {
                org = GetOrganization(itemId);
                if (org == null)
                    throw new ApplicationException("Organization is null");
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ORGANIZATION_BY_ITEM_ID, ex);
                return res;
            }

            ExchangeServer exchange;
            try
            {
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ORGANIZATION_PROXY, ex);
                return res;
            }

            ExchangeAccount account;
            try
            {
                account = GetAccount(itemId, accountId);
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_ACCOUNT, ex);
                return res;
            }

            try
            {
                res.Value = exchange.GetDistributionListPermissions(org.OrganizationId, account.AccountName);
                res.Value.DisplayName = account.DisplayName;
            }
            catch (Exception ex)
            {
                TaskManager.CompleteResultTask(res, ErrorCodes.CANNOT_GET_DISTRIBUTION_LIST_PERMISSIONS, ex);
                return res;
            }

            TaskManager.CompleteTask();
            return res;
        }

        public ExchangeAccount[] GetDistributionListsByMember(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return null;
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_DISTR_LIST_BYMEMBER");
            TaskManager.ItemId = itemId;

            List<ExchangeAccount> ret = new List<ExchangeAccount>();

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                List<ExchangeAccount> DistributionLists = GetAccounts(itemId, ExchangeAccountType.DistributionList);
                foreach (ExchangeAccount DistributionAccount in DistributionLists)
                {
                    ExchangeDistributionList DistributionList = exchange.GetDistributionListGeneralSettings(DistributionAccount.AccountName);

                    foreach (ExchangeAccount member in DistributionList.MembersAccounts)
                    {
                        if (member.AccountName == account.AccountName)
                        {
                            ret.Add(DistributionAccount);
                            break;
                        }

                    }
                }

                return ret.ToArray();
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

        public int AddDistributionListMember(int itemId, string distributionListName, int memberId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return 0;
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "ADD_DISTR_LIST_MEMBER");
            TaskManager.ItemId = itemId;

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return 0;

                // load account
                ExchangeAccount memberAccount = GetAccount(itemId, memberId);

                // Log Extension
                LogExtension.SetItemName(memberAccount.PrimaryEmailAddress);
                LogExtension.WriteVariables(new { distributionListName });

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                ExchangeDistributionList distributionList = exchange.GetDistributionListGeneralSettings(distributionListName);

                List<string> members = new List<string>();
                foreach (ExchangeAccount member in distributionList.MembersAccounts)
                    members.Add(member.AccountName);
                members.Add(memberAccount.AccountName);

                List<string> addressLists = new List<string>();
                addressLists.Add(org.GlobalAddressList);
                addressLists.Add(org.AddressList);

                exchange.SetDistributionListGeneralSettings(distributionListName, distributionList.DisplayName, distributionList.HideFromAddressBook, distributionList.ManagerAccount.AccountName,
                    members.ToArray(),
                    distributionList.Notes, addressLists.ToArray());

            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }

            return 0;
        }

        public int DeleteDistributionListMember(int itemId, string distributionListName, int memberId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return 0;
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "DELETE_DISTR_LIST_MEMBER", distributionListName);
            TaskManager.ItemId = itemId;

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return 0;

                // load account
                ExchangeAccount memberAccount = GetAccount(itemId, memberId, false);

                // Log Extension
                memberAccount.LogProperty(a => a.AccountName, "Member Removed");

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                ExchangeDistributionList distributionList = exchange.GetDistributionListGeneralSettings(distributionListName);

                List<string> members = new List<string>();
                foreach (ExchangeAccount member in distributionList.MembersAccounts)
                    if (member.AccountName != memberAccount.AccountName) members.Add(member.AccountName);

                List<string> addressLists = new List<string>();
                addressLists.Add(org.GlobalAddressList);
                addressLists.Add(org.AddressList);

                exchange.SetDistributionListGeneralSettings(distributionListName, distributionList.DisplayName, distributionList.HideFromAddressBook, distributionList.ManagerAccount.AccountName,
                    members.ToArray(),
                    distributionList.Notes, addressLists.ToArray());

            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }

            return 0;
        }


        #endregion

        #region Public Folders
        public int CreatePublicFolder(int itemId, string parentFolder, string folderName,
            bool mailEnabled, string name, string domain)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // check mailbox quota
            OrganizationStatistics orgStats = GetOrganizationStatisticsByOrganization(itemId);
            if (orgStats.AllocatedPublicFolders > -1
                && orgStats.CreatedPublicFolders >= orgStats.AllocatedPublicFolders)
                return BusinessErrorCodes.ERROR_EXCHANGE_PFOLDERS_QUOTA_LIMIT;

            // place log record
            TaskManager.StartTask("EXCHANGE", "CREATE_PUBLIC_FOLDER", parentFolder + "\\" + folderName, itemId);

            try
            {
                // e-mail
                string email = "";
                if (mailEnabled && !String.IsNullOrEmpty(name))
                {
                    email = name + "@" + domain;

                    // check e-mail
                    if (EmailAddressExists(email, true))
                        return BusinessErrorCodes.ERROR_EXCHANGE_EMAIL_EXISTS;
                }

                // full folder name
                string normParent = parentFolder;
                if (!normParent.StartsWith("\\"))
                    normParent = "\\" + normParent;
                if (!normParent.EndsWith("\\"))
                    normParent = normParent + "\\";

                string folderPath = normParent + folderName;

                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                if (String.IsNullOrEmpty(name))
                    name = Utils.CleanIdentifier(folderName);

                string accountName = OrganizationController.BuildAccountNameEx(org, name);

                // add mailbox
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                //Create Exchange Organization
                if (string.IsNullOrEmpty(org.GlobalAddressList))
                {
                    ExtendToExchangeOrganization(ref org);

                    PackageController.UpdatePackageItem(org);
                }

                exchange.CreatePublicFolder(org.DistinguishedName,
                    org.OrganizationId,
                    org.SecurityGroup,
                    parentFolder,
                    folderName,
                    mailEnabled,
                    accountName,
                    name,
                    domain);


                ExchangePublicFolder folder = exchange.GetPublicFolderGeneralSettings(org.OrganizationId, parentFolder + "\\" + folderName);

                // add meta-item
                int accountId = AddAccount(itemId, ExchangeAccountType.PublicFolder, accountName,
                    folderPath, email, mailEnabled,
                    0, folder.NETBIOS + "\\" + accountName, null, 0, null);

                // register email address
                if (mailEnabled)
                    AddAccountEmailAddress(accountId, email);

                // Log Extension
                LogExtension.WriteVariables(new { accountId, accountName, folderName, folderPath, email });

                return accountId;
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

        public int DeletePublicFolders(int itemId, int[] accountIds)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            if (accountIds != null)
                foreach (int accountId in accountIds)
                {
                    int result = DeletePublicFolder(itemId, accountId);
                    if (result < 0)
                        return result;
                }
            return 0;
        }

        public int DeletePublicFolder(int itemId, int accountId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DELETE_PUBLIC_FOLDER", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.DisplayName);

                // delete folder
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.DeletePublicFolder(org.OrganizationId, account.DisplayName);

                // unregister account
                DeleteAccount(itemId, accountId);

                // delete all nested folder meta-items
                List<ExchangeAccount> folders = GetAccounts(itemId, ExchangeAccountType.PublicFolder);
                foreach (ExchangeAccount folder in folders)
                {
                    if (folder.DisplayName.ToLower().StartsWith(account.DisplayName.ToLower() + "\\"))
                        DeleteAccount(itemId, folder.AccountId);
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

        public int EnableMailPublicFolder(int itemId, int accountId,
            string name, string domain)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "ENABLE_MAIL_PUBLIC_FOLDER", name + "@" + domain, itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);
                if (account.MailEnabledPublicFolder)
                    return 0;

                // check email
                string email = name + "@" + domain;

                // check e-mail
                if (EmailAddressExists(email, true))
                    return BusinessErrorCodes.ERROR_EXCHANGE_EMAIL_EXISTS;

                string accountName = OrganizationController.BuildAccountNameEx(org, name);

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.EnableMailPublicFolder(
                    org.OrganizationId,
                    account.DisplayName,
                    account.AccountName,
                    name,
                    domain);

                // Log Extension
                account.LogPropertyIfChanged(a => a.MailEnabledPublicFolder, accountName);
                account.LogPropertyIfChanged(a => a.PrimaryEmailAddress, email, false);

                // update and save account
                account.AccountName = accountName;
                account.MailEnabledPublicFolder = true;
                account.PrimaryEmailAddress = email;
                UpdateAccount(account);

                // register e-mail
                AddAccountEmailAddress(accountId, email);

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

        public int DisableMailPublicFolder(int itemId, int accountId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DISABLE_MAIL_PUBLIC_FOLDER", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);
                if (!account.MailEnabledPublicFolder)
                    return 0;

                // Log Extension
                LogExtension.SetItemName(account.PrimaryEmailAddress);

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.DisableMailPublicFolder(org.OrganizationId, account.DisplayName);


                // update and save account
                account.MailEnabledPublicFolder = false;
                account.PrimaryEmailAddress = "";
                UpdateAccount(account);


                // delete all mail accounts
                List<string> addrs = new List<string>();
                ExchangeEmailAddress[] emails = GetAccountEmailAddresses(itemId, accountId);
                foreach (ExchangeEmailAddress email in emails)
                    addrs.Add(email.EmailAddress);
                var addressesDeleted = addrs.ToArray();

                // Log Extension
                LogExtension.WriteVariables(new { addressesDeleted });

                DeleteAccountEmailAddresses(accountId, addressesDeleted);

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

        private ExchangePublicFolder GetDemoPublicFolderSettings()
        {
            ExchangePublicFolder c = new ExchangePublicFolder();
            c.DisplayName = "\\fabrikam\\Documents";
            c.MailEnabled = true;
            c.Name = "Documents";
            c.Accounts = GetAccounts(0, ExchangeAccountType.Mailbox).ToArray();
            c.AcceptAccounts = GetAccounts(0, ExchangeAccountType.Mailbox).ToArray();
            return c;
        }

        public ExchangePublicFolder GetPublicFolderGeneralSettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoPublicFolderSettings();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_PUBLIC_FOLDER_GENERAL", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                ExchangePublicFolder folder = exchange.GetPublicFolderGeneralSettings(org.OrganizationId, account.DisplayName);
                folder.MailEnabled = account.MailEnabledPublicFolder;
                folder.DisplayName = account.DisplayName;
                return folder;
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

        public int SetPublicFolderGeneralSettings(int itemId, int accountId, string newName,
            bool hideAddressBook, ExchangeAccount[] accounts)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_PUBLIC_FOLDER_GENERAL", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                var oldObj = exchange.GetPublicFolderGeneralSettings(org.OrganizationId, account.DisplayName);

                exchange.SetPublicFolderGeneralSettings(
                    org.OrganizationId,
                    account.DisplayName,
                    newName,
                    hideAddressBook,
                    accounts
                    );


                // update folder name
                string origName = account.DisplayName;
                string newFullName = origName.Substring(0, origName.LastIndexOf("\\") + 1) + newName;

                TaskManager.Write("Getting new PF Settings");
                var newObj = exchange.GetPublicFolderGeneralSettings(org.OrganizationId, newFullName);

                // Log Extension
                LogExtension.LogPropertiesIfChanged(oldObj, newObj);
                LogExtension.SetItemName(account.DisplayName);

                if (String.Compare(origName, newFullName, true) != 0)
                {
                    // Log Extension
                    account.LogPropertyIfChanged(a => a.DisplayName, newFullName);
                    TaskManager.Write("Public Folders: ");

                    // rename original folder
                    account.DisplayName = newFullName;
                    UpdateAccount(account);

                    // rename nested folders
                    List<ExchangeAccount> folders = GetAccounts(itemId, ExchangeAccountType.PublicFolder);
                    foreach (ExchangeAccount folder in folders)
                    {
                        if (folder.DisplayName.ToLower().StartsWith(origName.ToLower() + "\\"))
                        {
                            var newFolderName = newFullName + folder.DisplayName.Substring(origName.Length);

                            // Log Extension
                            folder.LogPropertyIfChanged(f => f.DisplayName, newFolderName);

                            folder.DisplayName = newFolderName;
                            UpdateAccount(folder);
                        }
                    }
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

        public ExchangePublicFolder GetPublicFolderMailFlowSettings(int itemId, int accountId)
        {
            #region Demo Mode
            if (IsDemoMode)
            {
                return GetDemoPublicFolderSettings();
            }
            #endregion

            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_PUBLIC_FOLDER_MAILFLOW", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                ExchangePublicFolder folder = exchange.GetPublicFolderMailFlowSettings(org.OrganizationId, account.DisplayName);
                folder.DisplayName = account.DisplayName;
                return folder;
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

        public int SetPublicFolderMailFlowSettings(int itemId, int accountId,
            string[] acceptAccounts, string[] rejectAccounts, bool requireSenderAuthentication)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_PUBLIC_FOLDER_MAILFLOW", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.DisplayName);
                LogExtension.WriteVariables(new { acceptAccounts, rejectAccounts, requireSenderAuthentication });

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                var oldObj = exchange.GetPublicFolderMailFlowSettings(org.OrganizationId, account.DisplayName);

                exchange.SetPublicFolderMailFlowSettings(org.OrganizationId, account.DisplayName,
                    acceptAccounts,
                    rejectAccounts,
                    requireSenderAuthentication);

                var newObj = exchange.GetPublicFolderMailFlowSettings(org.OrganizationId, account.DisplayName);

                // Log Extension
                LogExtension.LogPropertiesIfChanged(oldObj, newObj);

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

        public ExchangeEmailAddress[] GetPublicFolderEmailAddresses(int itemId, int accountId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_PUBLIC_FOLDER_ADDRESSES", itemId);

            try
            {
                return GetAccountEmailAddresses(itemId, accountId);
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

        public int AddPublicFolderEmailAddress(int itemId, int accountId, string emailAddress)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "ADD_PUBLIC_FOLDER_ADDRESS", itemId);

            try
            {
                // check
                if (EmailAddressExists(emailAddress, true))
                    return BusinessErrorCodes.ERROR_EXCHANGE_EMAIL_EXISTS;

                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.DisplayName);
                LogExtension.WriteVariable("Email Address Added", emailAddress);

                // add e-mail
                AddAccountEmailAddress(accountId, emailAddress);

                // update e-mail addresses
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.SetPublicFolderEmailAddresses(
                    org.OrganizationId,
                    account.DisplayName,
                    GetAccountSimpleEmailAddresses(itemId, accountId));

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

        public int SetPublicFolderPrimaryEmailAddress(int itemId, int accountId, string emailAddress)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "SET_PRIMARY_PUBLIC_FOLDER_ADDRESS", itemId);

            try
            {
                // get account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // Log Extension
                LogExtension.SetItemName(account.DisplayName);
                account.LogPropertyIfChanged(a => a.PrimaryEmailAddress, emailAddress);

                account.PrimaryEmailAddress = emailAddress;

                // update exchange
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // check package
                int packageCheck = SecurityContext.CheckPackage(org.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0) return packageCheck;

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.SetPublicFolderPrimaryEmailAddress(
                    org.OrganizationId,
                    account.DisplayName,
                    emailAddress);

                // save account
                UpdateAccount(account);

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

        public int DeletePublicFolderEmailAddresses(int itemId, int accountId, string[] emailAddresses)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "DELETE_PUBLIC_FOLDER_ADDRESSES", itemId);

            try
            {
                // get account
                ExchangeAccount account = GetAccount(itemId, accountId);

                // delete e-mail addresses
                List<string> toDelete = new List<string>();
                foreach (string emailAddress in emailAddresses)
                {
                    if (String.Compare(account.PrimaryEmailAddress, emailAddress, true) != 0)
                        toDelete.Add(emailAddress);
                }
                var emailAddressesDeleted = toDelete.ToArray();

                // Log Extension
                LogExtension.SetItemName(account.DisplayName);
                LogExtension.WriteVariables(new { emailAddressesDeleted });

                // delete from meta-base
                DeleteAccountEmailAddresses(accountId, emailAddressesDeleted);

                // delete from Exchange
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                // update e-mail addresses
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.SetPublicFolderEmailAddresses(
                    org.OrganizationId,
                    account.DisplayName,
                    GetAccountSimpleEmailAddresses(itemId, accountId));

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

        public string CreateOrganizationRootPublicFolder(int itemId)
        {
            string res = null;

            // place log record
            TaskManager.StartTask("EXCHANGE", "CREATE_ORG_PUBLIC_FOLDER", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // Log Extension
                LogExtension.SetItemName(org.DistinguishedName);
                org.LogProperty(o => o.DistinguishedName);

                // get mailbox settings
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);

                if (exchangeServiceId <= 0)
                    return null;

                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                if (exchange == null)
                    return null;

                //Create Exchange Organization
                if (string.IsNullOrEmpty(org.GlobalAddressList))
                {
                    ExtendToExchangeOrganization(ref org);

                    PackageController.UpdatePackageItem(org);
                }

                res = exchange.CreateOrganizationRootPublicFolder(org.OrganizationId, org.DistinguishedName, org.SecurityGroup, org.DefaultDomain);
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }

            return res;
        }

        public string SetDefaultPublicFolderMailbox(int itemId)
        {
            string res = "";

            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);

                if (exchangeServiceId <= 0)
                    return null;

                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                if (exchange == null)
                    return null;

                //Create Exchange Organization
                if (string.IsNullOrEmpty(org.GlobalAddressList))
                {
                    ExtendToExchangeOrganization(ref org);

                    PackageController.UpdatePackageItem(org);
                }

                res += "OrgPublicFolderMailbox = " + exchange.CreateOrganizationRootPublicFolder(org.OrganizationId, org.DistinguishedName, org.SecurityGroup, org.DefaultDomain) + Environment.NewLine;

                List<ExchangeAccount> mailboxes = GetExchangeMailboxes(itemId);

                foreach (ExchangeAccount mailbox in mailboxes)
                {
                    string id = mailbox.PrimaryEmailAddress;
                    string[] defaultPublicFoldes = exchange.SetDefaultPublicFolderMailbox(id, org.OrganizationId, org.DistinguishedName);

                    if (defaultPublicFoldes.Length == 1)
                        res += id + " has a value \"" + defaultPublicFoldes[0] + "\"" + Environment.NewLine;

                    if (defaultPublicFoldes.Length == 2)
                        res += id + " changed \"" + defaultPublicFoldes[0] + "\" to \"" + defaultPublicFoldes[1] + "\"" + Environment.NewLine;

                }

            }
            catch (Exception ex)
            {
                res += " Error " + ex.ToString();
            }

            return res;

        }

        #endregion

        #region Private Helpers


        private string GetPrimaryDomainController(int organizationServiceId)
        {

            Organizations orgProxy = new Organizations();

            ServiceProviderProxy.Init(orgProxy, organizationServiceId);

            string[] organizationSettings = ((ServiceProviderSettingsSoapHeader)orgProxy.SoapHeader).Settings;



            string orgPrimaryDomainController = string.Empty;
            foreach (string str in organizationSettings)
            {
                string[] props = str.Split('=');
                if (props[0].ToLower() == "primarydomaincontroller")
                {
                    orgPrimaryDomainController = str;
                    break;
                }
            }

            return orgPrimaryDomainController;
        }

        private void ExtendExchangeSettings(List<string> exchangeSettings, string primaryDomainController)
        {
            bool isAdded = false;
            for (int i = 0; i < exchangeSettings.Count; i++)
            {
                string[] props = exchangeSettings[i].Split('=');
                if (props[0].ToLower() == "primarydomaincontroller")
                {
                    exchangeSettings[i] = primaryDomainController;
                    isAdded = true;
                    break;
                }
            }

            if (!isAdded)
            {
                exchangeSettings.Add(primaryDomainController);
            }
        }

        internal ServiceProvider GetServiceProvider(int exchangeServiceId, int organizationServiceId)
        {
            ServiceProvider ws = new ServiceProvider();

            ServiceProviderProxy.Init(ws, exchangeServiceId);

            string[] exchangeSettings = ((ServiceProviderSettingsSoapHeader)ws.SoapHeader).Settings;

            List<string> resSettings = new List<string>(exchangeSettings);

            string orgPrimaryDomainController = GetPrimaryDomainController(organizationServiceId);

            ExtendExchangeSettings(resSettings, orgPrimaryDomainController);
            ((ServiceProviderSettingsSoapHeader)ws.SoapHeader).Settings = resSettings.ToArray();
            return ws;
        }

        internal ExchangeServer GetExchangeServer(int exchangeServiceId, int organizationServiceId)
        {
            ExchangeServer ws = new ExchangeServer();

            ServiceProviderProxy.Init(ws, exchangeServiceId);

            string[] exchangeSettings = ((ServiceProviderSettingsSoapHeader)ws.SoapHeader).Settings;

            List<string> resSettings = new List<string>(exchangeSettings);

            string orgPrimaryDomainController = GetPrimaryDomainController(organizationServiceId);

            ExtendExchangeSettings(resSettings, orgPrimaryDomainController);
            ((ServiceProviderSettingsSoapHeader)ws.SoapHeader).Settings = resSettings.ToArray();
            return ws;
        }

        internal ServiceProvider GetExchangeServiceProvider(int exchangeServiceId, int organizationServiceId)
        {
            ServiceProvider ws = new ServiceProvider();

            ServiceProviderProxy.Init(ws, exchangeServiceId);

            string[] exchangeSettings = ((ServiceProviderSettingsSoapHeader)ws.SoapHeader).Settings;

            List<string> resSettings = new List<string>(exchangeSettings);

            string orgPrimaryDomainController = GetPrimaryDomainController(organizationServiceId);

            ExtendExchangeSettings(resSettings, orgPrimaryDomainController);
            ((ServiceProviderSettingsSoapHeader)ws.SoapHeader).Settings = resSettings.ToArray();
            return ws;
        }


        private int GetExchangeServiceID(int packageId)
        {
            return PackageController.GetPackageServiceId(packageId, ResourceGroups.Exchange);
        }

        private string[] GetAccountSimpleEmailAddresses(int itemId, int accountId)
        {
            ExchangeEmailAddress[] emails = GetAccountEmailAddresses(itemId, accountId);
            List<string> result = new List<string>();
            foreach (ExchangeEmailAddress email in emails)
            {
                string prefix = email.IsPrimary ? "SMTP:" : "smtp:";
                result.Add(prefix + email.EmailAddress);
            }

            return result.ToArray();
        }

        private bool QuotaEnabled(PackageContext cntx, string quotaName)
        {
            return cntx.Quotas.ContainsKey(quotaName) && !cntx.Quotas[quotaName].QuotaExhausted;
        }

        private bool IsDemoMode
        {
            get
            {
                return (SecurityContext.CheckAccount(DemandAccount.NotDemo) < 0);
            }
        }
        #endregion

        public ExchangeMobileDevice[] GetMobileDevices(int itemId, int accountId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_MOBILE_DEVICES", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                // load account
                ExchangeAccount account = GetAccount(itemId, accountId);

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                return exchange.GetMobileDevices(account.UserPrincipalName);
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

        public ExchangeMobileDevice GetMobileDevice(int itemId, string deviceId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "GET_MOBILE_DEVICE", itemId);

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                return exchange.GetMobileDevice(deviceId);
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

        public void WipeDataFromDevice(int itemId, string deviceId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "WIPE_DATA_FROM_DEVICE", deviceId, itemId);

            // Log Extension
            LogExtension.WriteVariables(new { deviceId });

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return;

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.WipeDataFromDevice(deviceId);
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

        public void CancelRemoteWipeRequest(int itemId, string deviceId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "CANCEL_REMOTE_WIPE_REQUEST", deviceId, itemId);

            // Log Extension
            LogExtension.WriteVariables(new { deviceId });

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return;

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.CancelRemoteWipeRequest(deviceId);
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

        public void RemoveDevice(int itemId, string deviceId)
        {
            // place log record
            TaskManager.StartTask("EXCHANGE", "REMOVE_DEVICE", deviceId, itemId);

            // Log Extension
            LogExtension.WriteVariables(new { deviceId });

            try
            {
                // load organization
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return;

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                exchange.RemoveDevice(deviceId);
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

        #region Disclaimers

        public int AddExchangeDisclaimer(int itemId, ExchangeDisclaimer disclaimer)
        {
            int res = -1;

            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "ADD_EXCHANGE_EXCHANGEDISCLAIMER", disclaimer.DisclaimerName, itemId);

            // Log Extension
            LogExtension.WriteObject(disclaimer);

            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null) return -1;

                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                res = Database.AddExchangeDisclaimer(itemId, disclaimer);
                disclaimer.ExchangeDisclaimerId = res;
                exchange.SetDisclaimer(disclaimer.FCPUniqueName, disclaimer.DisclaimerText);
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }

            return res;
        }

        public int UpdateExchangeDisclaimer(int itemId, ExchangeDisclaimer disclaimer)
        {
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "UPDATE_EXCHANGE_EXCHANGEDISCLAIMER", disclaimer.DisclaimerName, itemId);

            // Log Extension
            LogExtension.WriteObject(disclaimer);

            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null) return -1;
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                var oldObj = GetExchangeDisclaimer(itemId, disclaimer.ExchangeDisclaimerId);

                exchange.SetDisclaimer(disclaimer.FCPUniqueName, disclaimer.DisclaimerText);
                Database.UpdateExchangeDisclaimer(itemId, disclaimer);

                // Log Extension
                LogExtension.LogPropertiesIfChanged(oldObj, disclaimer);
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }

            return 0;
        }

        public int DeleteExchangeDisclaimer(int itemId, int exchangeDisclaimerId)
        {
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            TaskManager.StartTask("EXCHANGE", "DELETE_EXCHANGE_EXCHANGEDISCLAIMER", itemId);

            try
            {
                ExchangeDisclaimer disclaimer = null;
                if (exchangeDisclaimerId != -1) disclaimer = GetExchangeDisclaimer(itemId, exchangeDisclaimerId);

                // Log Extension
                if (disclaimer != null)
                {
                    LogExtension.SetItemName(disclaimer.DisclaimerName);
                    LogExtension.WriteObject(disclaimer);
                }

                Organization org = GetOrganization(itemId);
                if (org == null) return -1;
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                if (exchange.RemoveDisclaimer(disclaimer.FCPUniqueName) != -1)
                    Database.DeleteExchangeDisclaimer(exchangeDisclaimerId);
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }

            return 0;
        }

        public ExchangeDisclaimer GetExchangeDisclaimer(int itemId, int exchangeDisclaimerId)
        {
            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return null;

                TaskManager.StartTask("EXCHANGE", "GET_EXCHANGE_EXCHANGEDISCLAIMER", null, itemId, org.PackageId, null);

                return ObjectUtils.FillObjectFromDataReader<ExchangeDisclaimer>(
                    Database.GetExchangeDisclaimer(exchangeDisclaimerId));
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

        public List<ExchangeDisclaimer> GetExchangeDisclaimers(int itemId)
        {
            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return new List<ExchangeDisclaimer>();

                TaskManager.StartTask("EXCHANGE", "GET_EXCHANGE_EXCHANGEDISCLAIMER", null, itemId, org.PackageId, null);

                List<ExchangeDisclaimer> disclaimers = ObjectUtils.CreateListFromDataReader<ExchangeDisclaimer>(Database.GetExchangeDisclaimers(itemId));
                return disclaimers;
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

        public int SetExchangeAccountDisclaimerId(int itemId, int accountID, int newExchangeDisclaimerId)
        {
            int res = 0;

            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // place log record
            TaskManager.StartTask("EXCHANGE", "SET_EXCHANGE_ACCOUNTDISCLAIMERID", accountID);

            try
            {
                int oldExchangeDisclaimerId = GetExchangeAccountDisclaimerId(itemId, accountID);

                ExchangeDisclaimer newDisclaimer = null;
                ExchangeDisclaimer oldDisclaimer = null;

                if (newExchangeDisclaimerId != -1) newDisclaimer = GetExchangeDisclaimer(itemId, newExchangeDisclaimerId);
                if (oldExchangeDisclaimerId != -1) oldDisclaimer = GetExchangeDisclaimer(itemId, oldExchangeDisclaimerId);

                // load account
                ExchangeAccount account = GetAccount(itemId, accountID);

                Organization org = GetOrganization(itemId);
                if (org == null) return -1;
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                // remove old version
                string oldTransportRuleName = org.Name + "_" + account.PrimaryEmailAddress;
                exchange.RemoveDisclaimer(oldTransportRuleName);

                // update disclaimer
                if (newDisclaimer != null)
                    exchange.SetDisclaimer(newDisclaimer.FCPUniqueName, newDisclaimer.DisclaimerText);

                if (newExchangeDisclaimerId != oldExchangeDisclaimerId)
                {
                    if (oldDisclaimer != null)
                        exchange.RemoveDisclamerMember(oldDisclaimer.FCPUniqueName, account.PrimaryEmailAddress);

                    if (newDisclaimer != null)
                        res = exchange.AddDisclamerMember(newDisclaimer.FCPUniqueName, account.PrimaryEmailAddress);

                    if (res == 0)
                        Database.SetExchangeAccountDisclaimerId(accountID, newExchangeDisclaimerId);
                }

            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }

            return res;

        }

        public int GetExchangeAccountDisclaimerId(int itemId, int AccountID)
        {
            try
            {
                Organization org = GetOrganization(itemId);
                if (org == null)
                    return -1;

                TaskManager.StartTask("EXCHANGE", "GET_EXCHANGE_ACCOUNTDISCLAIMERID", null, itemId, org.PackageId, null);

                return Database.GetExchangeAccountDisclaimerId(AccountID);
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

        #region Pictures

#if !UseSkia || NETFRAMEWORK
        // proportional resizing
        private Bitmap ResizeBitmapGdi(Bitmap srcBitmap, Size newSize)
        {
            try
            {
                Bitmap destBitmap = new Bitmap(newSize.Width, newSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                Rectangle srcRect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
                Rectangle destRect = new Rectangle(0, 0, destBitmap.Width, destBitmap.Height);

                float kWidth = Convert.ToSingle(srcBitmap.Width) / Convert.ToSingle(destBitmap.Width);
                float kHeight = Convert.ToSingle(srcBitmap.Height) / Convert.ToSingle(destBitmap.Height);

                if (kHeight > kWidth)
                {
                    int l = (srcBitmap.Height - Convert.ToInt32(kWidth * destBitmap.Height));
                    srcRect.Y = l / 2;
                    srcRect.Height = srcBitmap.Height - l;
                }
                else if (kHeight < kWidth)
                {
                    int l = (srcBitmap.Width - Convert.ToInt32(kHeight * destBitmap.Width));
                    srcRect.X = l / 2;
                    srcRect.Width = srcBitmap.Width - l;
                }

                Graphics g = Graphics.FromImage(destBitmap);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.DrawImage(srcBitmap, destRect, srcRect, GraphicsUnit.Pixel);
                g.Dispose();

                return destBitmap;
            }
            catch
            {
                return null;
            }
        }
#endif
        public byte[] ResizeImageGdi(byte[] picture, int width, int height)
        {
            if (!Providers.OS.OSInfo.IsWindows) return null;

#if !UseSkia || NETFRAMEWORK
            var bitmap = new Bitmap(new MemoryStream(picture));

            MemoryStream pictureStream = new MemoryStream();

            if (bitmap.Width > width || bitmap.Height > height)
                bitmap = ResizeBitmapGdi(bitmap, new Size(width, height));

            bitmap.Save(pictureStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            return pictureStream.ToArray();
#else
            return null;
#endif
        }

        public byte[] ResizeImageSkia(byte[] data, int width, int height)
        {
#if UseSkia
            using (var bitmap = SKBitmap.Decode(data))
            {

                if (bitmap.Width > width || bitmap.Height > height)
                {
                    using (var scaledBitmap = bitmap.Resize(new SKImageInfo(width, height), new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)))
                    using (var scaledImage = SKImage.FromBitmap(scaledBitmap))
                    using (var encodedImage = scaledImage.Encode(SKEncodedImageFormat.Jpeg, 50))
                    {
                        var stream = new MemoryStream();
                        encodedImage.SaveTo(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        return stream.ToArray();
                    }
                }
                else return data;
            }
#else
            return null;
#endif
        }

        public byte[] ResizeImage(byte[] image, int width, int height)
        {
            if (image == null) return null;

            image = ResizeImageGdi(image, width, height);

            if (image != null) return image;

            Providers.OS.SkiaSharp.LoadNativeDlls();

            return ResizeImageSkia(image, width, height); 
        }
        public ResultObject SetPicture(int itemId, int accountID, byte[] picture)
        {
            ResultObject res = TaskManager.StartResultTask<ResultObject>("EXCHANGE", "SET_PICTURE", itemId);

            Organization org = GetOrganization(itemId);
            if (org == null)
                throw new ApplicationException("Organization is null");

            ExchangeAccount account = GetAccount(itemId, accountID);

            // Log Extension
            LogExtension.SetItemName(account.PrimaryEmailAddress);

            try
            {
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                if (exchangeServiceId > 0)
                {
                    ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);

                    if (picture != null)
                    {
                        picture = ResizeImage(picture, MAX_THUMBNAILPHOTO_SIZE, MAX_THUMBNAILPHOTO_SIZE);
                    }

					res = exchange.SetPicture(account.AccountName, picture);
                }
            }
            catch (Exception ex)
            {
                TaskManager.WriteError(ex);
                TaskManager.CompleteResultTask(res);
                return res;
            }

            TaskManager.CompleteResultTask();
            return res;

        }

        public BytesResult GetPicture(int itemId, int accountID)
        {
            BytesResult res = TaskManager.StartResultTask<BytesResult>("EXCHANGE", "GET_PICTURE", itemId);

            Organization org = GetOrganization(itemId);
            if (org == null)
                throw new ApplicationException("Organization is null");

            ExchangeAccount account = GetAccount(itemId, accountID);

            try
            {
                int exchangeServiceId = GetExchangeServiceID(org.PackageId);
                if (exchangeServiceId > 0)
                {
                    ExchangeServer exchange = GetExchangeServer(exchangeServiceId, org.ServiceId);
                    res = exchange.GetPicture(account.AccountName);
                }
            }
            catch (Exception ex)
            {
                TaskManager.WriteError(ex);
                TaskManager.CompleteResultTask(res);
                return res;
            }

            TaskManager.CompleteResultTask();
            return res;

        }

#endregion
    }
}
