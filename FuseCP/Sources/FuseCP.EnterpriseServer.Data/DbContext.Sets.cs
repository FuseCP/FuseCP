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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuseCP.EnterpriseServer.Data.Entities;
#if NETFRAMEWORK
using System.Data.Entity;
#elif NETCOREAPP
using Microsoft.EntityFrameworkCore;
#endif

namespace FuseCP.EnterpriseServer.Data
{
	using Version = FuseCP.EnterpriseServer.Data.Entities.Version;
	using GlobalDnsRecord = FuseCP.EnterpriseServer.Data.Entities.GlobalDnsRecord;
	using BackgroundTask = FuseCP.EnterpriseServer.Data.Entities.BackgroundTask;
	using BackgroundTaskParameter = FuseCP.EnterpriseServer.Data.Entities.BackgroundTaskParameter;

	public partial class DbContext
	{
		DbSet<AccessToken> accessTokens = null;
		public virtual DbSet<AccessToken> AccessTokens => accessTokens ??= Set<AccessToken>();

		DbSet<AdditionalGroup> additionalGroups = null;
		public virtual DbSet<AdditionalGroup> AdditionalGroups => additionalGroups ??= Set<AdditionalGroup>();

		DbSet<AuditLog> auditLogs = null;
		public virtual DbSet<AuditLog> AuditLogs => auditLogs ??= Set<AuditLog>();

		DbSet<AuditLogSource> auditLogSources = null;
		public virtual DbSet<AuditLogSource> AuditLogSources => auditLogSources ??= Set<AuditLogSource>();

		DbSet<AuditLogTask> auditLogTasks = null;
		public virtual DbSet<AuditLogTask> AuditLogTasks => auditLogTasks ??= Set<AuditLogTask>();

		DbSet<BackgroundTask> backgroundTasks = null;
		public virtual DbSet<BackgroundTask> BackgroundTasks => backgroundTasks ??= Set<BackgroundTask>();

		DbSet<BackgroundTaskLog> backgroundTaskLogs = null;
		public virtual DbSet<BackgroundTaskLog> BackgroundTaskLogs => backgroundTaskLogs ??= Set<BackgroundTaskLog>();

		DbSet<BackgroundTaskParameter> backgroundTaskParameters = null;
		public virtual DbSet<BackgroundTaskParameter> BackgroundTaskParameters => backgroundTaskParameters ??= Set<BackgroundTaskParameter>();

		DbSet<BackgroundTaskStack> backgroundTaskStacks = null;
		public virtual DbSet<BackgroundTaskStack> BackgroundTaskStacks => backgroundTaskStacks ??= Set<BackgroundTaskStack>();

		DbSet<BlackBerryUser> blackBerryUsers = null;
		public virtual DbSet<BlackBerryUser> BlackBerryUsers => blackBerryUsers ??= Set<BlackBerryUser>();

		DbSet<BruteForceLog> bruteForceLogs = null;
		public virtual DbSet<BruteForceLog> BruteForceLogs => bruteForceLogs ??= Set<BruteForceLog>();

		DbSet<IpSecurityPolicy> ipSecurityPolicies = null;
		public virtual DbSet<IpSecurityPolicy> IpSecurityPolicies => ipSecurityPolicies ??= Set<IpSecurityPolicy>();

		DbSet<Cluster> clusters = null;
		public virtual DbSet<Cluster> Clusters => clusters ??= Set<Cluster>();

		DbSet<Comment> comments = null;
		public virtual DbSet<Comment> Comments => comments ??= Set<Comment>();

		DbSet<CrmUser> crmusers = null;
		public virtual DbSet<CrmUser> CrmUsers => crmusers ??= Set<CrmUser>();

		DbSet<DmzIpAddress> dmzIpAddresses = null;
		public virtual DbSet<DmzIpAddress> DmzIpAddresses => dmzIpAddresses ??= Set<DmzIpAddress>();

		DbSet<Domain> domains = null;
		public virtual DbSet<Domain> Domains => domains ??= Set<Domain>();

		DbSet<DomainDnsRecord> domainDnsRecords = null;
		public virtual DbSet<DomainDnsRecord> DomainDnsRecords => domainDnsRecords ??= Set<DomainDnsRecord>();

		DbSet<EnterpriseFolder> enterpriseFolders = null;
		public virtual DbSet<EnterpriseFolder> EnterpriseFolders => enterpriseFolders ??= Set<EnterpriseFolder>();

		DbSet<EnterpriseFoldersOwaPermission> enterpriseFoldersOwaPermissions = null;
		public virtual DbSet<EnterpriseFoldersOwaPermission> EnterpriseFoldersOwaPermissions => enterpriseFoldersOwaPermissions ??= Set<EnterpriseFoldersOwaPermission>();

		DbSet<ExchangeAccount> exchangeAccounts = null;
		public virtual DbSet<ExchangeAccount> ExchangeAccounts => exchangeAccounts ??= Set<ExchangeAccount>();

		DbSet<ExchangeAccountEmailAddress> exchangeAccountEmailAddresses = null;
		public virtual DbSet<ExchangeAccountEmailAddress> ExchangeAccountEmailAddresses => exchangeAccountEmailAddresses ??= Set<ExchangeAccountEmailAddress>();

		DbSet<ExchangeDeletedAccount> exchangeDeletedAccounts = null;
		public virtual DbSet<ExchangeDeletedAccount> ExchangeDeletedAccounts => exchangeDeletedAccounts ??= Set<ExchangeDeletedAccount>();

		DbSet<ExchangeDisclaimer> exchangeDisclaimers = null;
		public virtual DbSet<ExchangeDisclaimer> ExchangeDisclaimers => exchangeDisclaimers ??= Set<ExchangeDisclaimer>();

		DbSet<ExchangeMailboxPlan> exchangeMailboxPlans = null;
		public virtual DbSet<ExchangeMailboxPlan> ExchangeMailboxPlans => exchangeMailboxPlans ??= Set<ExchangeMailboxPlan>();

		DbSet<ExchangeMailboxPlanRetentionPolicyTag> exchangeMailboxPlanRetentionPolicyTags = null;
		public virtual DbSet<ExchangeMailboxPlanRetentionPolicyTag> ExchangeMailboxPlanRetentionPolicyTags => exchangeMailboxPlanRetentionPolicyTags ??= Set<ExchangeMailboxPlanRetentionPolicyTag>();

		DbSet<ExchangeOrganization> exchangeOrganizations = null;
		public virtual DbSet<ExchangeOrganization> ExchangeOrganizations => exchangeOrganizations ??= Set<ExchangeOrganization>();

		DbSet<ExchangeOrganizationDomain> exchangeOrganizationDomains = null;
		public virtual DbSet<ExchangeOrganizationDomain> ExchangeOrganizationDomains => exchangeOrganizationDomains ??= Set<ExchangeOrganizationDomain>();

		DbSet<ExchangeOrganizationSetting> exchangeOrganizationSettings = null;
		public virtual DbSet<ExchangeOrganizationSetting> ExchangeOrganizationSettings => exchangeOrganizationSettings ??= Set<ExchangeOrganizationSetting>();

		DbSet<ExchangeOrganizationSsFolder> exchangeOrganizationSsFolders = null;
		public virtual DbSet<ExchangeOrganizationSsFolder> ExchangeOrganizationSsFolders => exchangeOrganizationSsFolders ??= Set<ExchangeOrganizationSsFolder>();

		DbSet<ExchangeRetentionPolicyTag> exchangeRetentionPolicyTags = null;
		public virtual DbSet<ExchangeRetentionPolicyTag> ExchangeRetentionPolicyTags => exchangeRetentionPolicyTags ??= Set<ExchangeRetentionPolicyTag>();

		DbSet<GlobalDnsRecord> globalDnsRecords = null;
		public virtual DbSet<GlobalDnsRecord> GlobalDnsRecords => globalDnsRecords ??= Set<GlobalDnsRecord>();

		DbSet<HostingPlan> hostingPlans = null;
		public virtual DbSet<HostingPlan> HostingPlans => hostingPlans ??= Set<HostingPlan>();

		DbSet<HostingPlanQuota> hostingPlanQuotas = null;
		public virtual DbSet<HostingPlanQuota> HostingPlanQuotas => hostingPlanQuotas ??= Set<HostingPlanQuota>();

		DbSet<HostingPlanResource> hostingPlanResources = null;
		public virtual DbSet<HostingPlanResource> HostingPlanResources => hostingPlanResources ??= Set<HostingPlanResource>();

		DbSet<IpAddress> ipaddresses = null;
		public virtual DbSet<IpAddress> IpAddresses => ipaddresses ??= Set<IpAddress>();

		DbSet<LyncUser> lyncUsers = null;
		public virtual DbSet<LyncUser> LyncUsers => lyncUsers ??= Set<LyncUser>();

		DbSet<LyncUserPlan> lyncUserPlans = null;
		public virtual DbSet<LyncUserPlan> LyncUserPlans => lyncUserPlans ??= Set<LyncUserPlan>();

		DbSet<OcsUser> ocsusers = null;
		public virtual DbSet<OcsUser> OcsUsers => ocsusers ??= Set<OcsUser>();

		DbSet<Package> packages = null;
		public virtual DbSet<Package> Packages => packages ??= Set<Package>();

		DbSet<PackageService> packageServices = null;
		public virtual DbSet<PackageService> PackageServices => packageServices ??= Set<PackageService>();

		DbSet<PackageAddon> packageAddons = null;
		public virtual DbSet<PackageAddon> PackageAddons => packageAddons ??= Set<PackageAddon>();

		DbSet<PackageIpAddress> packageIpAddresses = null;
		public virtual DbSet<PackageIpAddress> PackageIpAddresses => packageIpAddresses ??= Set<PackageIpAddress>();

		DbSet<PackageQuota> packageQuotas = null;
		public virtual DbSet<PackageQuota> PackageQuotas => packageQuotas ??= Set<PackageQuota>();

		DbSet<PackageResource> packageResources = null;
		public virtual DbSet<PackageResource> PackageResources => packageResources ??= Set<PackageResource>();

		DbSet<PackageSetting> packageSettings = null;
		public virtual DbSet<PackageSetting> PackageSettings => packageSettings ??= Set<PackageSetting>();

		DbSet<PackageVlan> packageVlans = null;
		public virtual DbSet<PackageVlan> PackageVlans => packageVlans ??= Set<PackageVlan>();

		DbSet<PackagesBandwidth> packagesBandwidths = null;
		public virtual DbSet<PackagesBandwidth> PackagesBandwidths => packagesBandwidths ??= Set<PackagesBandwidth>();

		DbSet<PackagesDiskspace> packagesDiskspaces = null;
		public virtual DbSet<PackagesDiskspace> PackagesDiskspaces => packagesDiskspaces ??= Set<PackagesDiskspace>();

		DbSet<PackagesTreeCache> packagesTreeCaches = null;
		public virtual DbSet<PackagesTreeCache> PackagesTreeCaches => packagesTreeCaches ??= Set<PackagesTreeCache>();

		DbSet<PrivateIpAddress> privateIpAddresses = null;
		public virtual DbSet<PrivateIpAddress> PrivateIpAddresses => privateIpAddresses ??= Set<PrivateIpAddress>();

		DbSet<PrivateNetworkVlan> privateNetworkVlans = null;
		public virtual DbSet<PrivateNetworkVlan> PrivateNetworkVlans => privateNetworkVlans ??= Set<PrivateNetworkVlan>();

		DbSet<Provider> providers = null;
		public virtual DbSet<Provider> Providers => providers ??= Set<Provider>();

		DbSet<Quota> quotas = null;
		public virtual DbSet<Quota> Quotas => quotas ??= Set<Quota>();

		DbSet<RdsCertificate> rdsCertificates = null;
		public virtual DbSet<RdsCertificate> RdsCertificates => rdsCertificates ??= Set<RdsCertificate>();

		DbSet<RdsCollection> rdsCollections = null;
		public virtual DbSet<RdsCollection> RdsCollections => rdsCollections ??= Set<RdsCollection>();

		DbSet<RdsCollectionSetting> rdsCollectionSettings = null;
		public virtual DbSet<RdsCollectionSetting> RdsCollectionSettings => rdsCollectionSettings ??= Set<RdsCollectionSetting>();

		DbSet<RdsCollectionUser> rdsCollectionUsers = null;
		public virtual DbSet<RdsCollectionUser> RdsCollectionUsers => rdsCollectionUsers ??= Set<RdsCollectionUser>();

		DbSet<RdsMessage> rdsMessages = null;
		public virtual DbSet<RdsMessage> RdsMessages => rdsMessages ??= Set<RdsMessage>();

		DbSet<RdsServer> rdsServers = null;
		public virtual DbSet<RdsServer> RdsServers => rdsServers ??= Set<RdsServer>();

		DbSet<RdsServerSetting> rdsServerSettings = null;
		public virtual DbSet<RdsServerSetting> RdsServerSettings => rdsServerSettings ??= Set<RdsServerSetting>();

		DbSet<ResourceGroup> resourceGroups = null;
		public virtual DbSet<ResourceGroup> ResourceGroups => resourceGroups ??= Set<ResourceGroup>();

		DbSet<ResourceGroupDnsRecord> resourceGroupDnsRecords = null;
		public virtual DbSet<ResourceGroupDnsRecord> ResourceGroupDnsRecords => resourceGroupDnsRecords ??= Set<ResourceGroupDnsRecord>();

		DbSet<Schedule> schedules = null;
		public virtual DbSet<Schedule> Schedules => schedules ??= Set<Schedule>();

		DbSet<ScheduleParameter> scheduleParameters = null;
		public virtual DbSet<ScheduleParameter> ScheduleParameters => scheduleParameters ??= Set<ScheduleParameter>();

		DbSet<ScheduleTask> scheduleTasks = null;
		public virtual DbSet<ScheduleTask> ScheduleTasks => scheduleTasks ??= Set<ScheduleTask>();

		DbSet<ScheduleTaskParameter> scheduleTaskParameters = null;
		public virtual DbSet<ScheduleTaskParameter> ScheduleTaskParameters => scheduleTaskParameters ??= Set<ScheduleTaskParameter>();

		DbSet<ScheduleTaskViewConfiguration> scheduleTaskViewConfigurations = null;
		public virtual DbSet<ScheduleTaskViewConfiguration> ScheduleTaskViewConfigurations => scheduleTaskViewConfigurations ??= Set<ScheduleTaskViewConfiguration>();

		DbSet<Server> servers = null;
		public virtual DbSet<Server> Servers => servers ??= Set<Server>();

		DbSet<Service> services = null;
		public virtual DbSet<Service> Services => services ??= Set<Service>();

		DbSet<ServiceDefaultProperty> serviceDefaultProperties = null;
		public virtual DbSet<ServiceDefaultProperty> ServiceDefaultProperties => serviceDefaultProperties ??= Set<ServiceDefaultProperty>();

		DbSet<ServiceItem> serviceItems = null;
		public virtual DbSet<ServiceItem> ServiceItems => serviceItems ??= Set<ServiceItem>();

		DbSet<ServiceItemProperty> serviceItemProperties = null;
		public virtual DbSet<ServiceItemProperty> ServiceItemProperties => serviceItemProperties ??= Set<ServiceItemProperty>();

		DbSet<ServiceItemType> serviceItemTypes = null;
		public virtual DbSet<ServiceItemType> ServiceItemTypes => serviceItemTypes ??= Set<ServiceItemType>();

		DbSet<ServiceProperty> serviceProperties = null;
		public virtual DbSet<ServiceProperty> ServiceProperties => serviceProperties ??= Set<ServiceProperty>();

		DbSet<SfBUser> sfBUsers = null;
		public virtual DbSet<SfBUser> SfBUsers => sfBUsers ??= Set<SfBUser>();

		DbSet<SfBUserPlan> sfBUserPlans = null;
		public virtual DbSet<SfBUserPlan> SfBUserPlans => sfBUserPlans ??= Set<SfBUserPlan>();

		DbSet<SslCertificate> sslCertificates = null;
		public virtual DbSet<SslCertificate> SslCertificates => sslCertificates ??= Set<SslCertificate>();

		DbSet<StorageSpace> storageSpaces = null;
		public virtual DbSet<StorageSpace> StorageSpaces => storageSpaces ??= Set<StorageSpace>();

		DbSet<StorageSpaceFolder> storageSpaceFolders = null;
		public virtual DbSet<StorageSpaceFolder> StorageSpaceFolders => storageSpaceFolders ??= Set<StorageSpaceFolder>();

		DbSet<StorageSpaceLevel> storageSpaceLevels = null;
		public virtual DbSet<StorageSpaceLevel> StorageSpaceLevels => storageSpaceLevels ??= Set<StorageSpaceLevel>();

		DbSet<StorageSpaceLevelResourceGroup> storageSpaceLevelResourceGroups = null;
		public virtual DbSet<StorageSpaceLevelResourceGroup> StorageSpaceLevelResourceGroups => storageSpaceLevelResourceGroups ??= Set<StorageSpaceLevelResourceGroup>();

		DbSet<SupportServiceLevel> supportServiceLevels = null;
		public virtual DbSet<SupportServiceLevel> SupportServiceLevels => supportServiceLevels ??= Set<SupportServiceLevel>();

		DbSet<SystemSetting> systemSettings = null;
		public virtual DbSet<SystemSetting> SystemSettings => systemSettings ??= Set<SystemSetting>();

		DbSet<Theme> themes = null;
		public virtual DbSet<Theme> Themes => themes ??= Set<Theme>();

		DbSet<ThemeSetting> themeSettings = null;
		public virtual DbSet<ThemeSetting> ThemeSettings => themeSettings ??= Set<ThemeSetting>();

		DbSet<User> users = null;
		public virtual DbSet<User> Users => users ??= Set<User>();

		DbSet<UserSetting> userSettings = null;
		public virtual DbSet<UserSetting> UserSettings => userSettings ??= Set<UserSetting>();

		DbSet<UsersDetailed> usersDetailedView = null;
		public virtual DbSet<UsersDetailed> UsersDetailedView => usersDetailedView ??= Set<UsersDetailed>();

		DbSet<TempId> tempIds = null;
		public virtual DbSet<TempId> TempIds => tempIds ??= Set<TempId>();

		// UsersDetailed view implemented with Linq
		public virtual IQueryable<UsersDetailed> UsersDetailed
		{
			get {
				if (IsSqlServer) return UsersDetailedView;
				else
				{
					return Users
						.GroupJoin(Users, u => u.OwnerId, o => o.UserId, (u, o) => new
						{
							User = u,
							Owners = o
						})
						.SelectMany(u => u.Owners.DefaultIfEmpty(), (u, o) => new
						{
							u.User,
							Owner = o
						})
						.Select(g => new UsersDetailed()
						{
							UserId = g.User.UserId,
							RoleId = g.User.RoleId,
							StatusId = g.User.StatusId,
							LoginStatusId = g.User.LoginStatusId,
							SubscriberNumber = g.User.SubscriberNumber,
							FailedLogins = g.User.FailedLogins,
							OwnerId = g.Owner != null ? g.Owner.UserId : null,
							Created = g.User.Created,
							Changed = g.User.Changed,
							IsDemo = g.User.IsDemo,
							Comments = g.User.Comments,
							IsPeer = g.User.IsPeer,
							Username = g.User.Username,
							FirstName = g.User.FirstName,
							LastName = g.User.LastName,
							Email = g.User.Email,
							CompanyName = g.User.CompanyName,
							FullName = g.User.FirstName + " " + g.User.LastName,
							OwnerUsername = g.Owner != null ? g.Owner.Username : null,
							OwnerFirstName = g.Owner != null ? g.Owner.FirstName : null,
							OwnerLastName = g.Owner != null ? g.Owner.LastName : null,
							OwnerEmail = g.Owner != null ? g.Owner.Email : null,
							OwnerRoleId = g.Owner != null ? g.Owner.RoleId : null,
							OwnerFullName = g.Owner != null ? g.Owner.FirstName + " " + g.Owner.LastName : null,
							EcommerceEnabled = g.User.EcommerceEnabled,
							PackagesNumber = Packages.Count(p => p.UserId == g.User.UserId)
						});
				}
			}
		}

		DbSet<Version> versions = null;
		public virtual DbSet<Version> Versions => versions ??= Set<Version>();

		DbSet<VirtualGroup> virtualGroups = null;
		public virtual DbSet<VirtualGroup> VirtualGroups => virtualGroups ??= Set<VirtualGroup>();

		DbSet<VirtualService> virtualServices = null;
		public virtual DbSet<VirtualService> VirtualServices => virtualServices ??= Set<VirtualService>();

		DbSet<WebDavAccessToken> webDavAccessTokens = null;
		public virtual DbSet<WebDavAccessToken> WebDavAccessTokens => webDavAccessTokens ??= Set<WebDavAccessToken>();

		DbSet<WebDavPortalUsersSetting> webDavPortalUsersSettings = null;
		public virtual DbSet<WebDavPortalUsersSetting> WebDavPortalUsersSettings => webDavPortalUsersSettings ??= Set<WebDavPortalUsersSetting>();
	}
}
