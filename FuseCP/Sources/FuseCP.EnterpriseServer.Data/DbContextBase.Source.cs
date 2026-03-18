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

// This file is auto generated, do not edit.
#if ScaffoldedDbContext
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FuseCP.EnterpriseServer.Data.Configuration;
using FuseCP.EnterpriseServer.Data.Entities.Sources;
#if NetCore
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
#endif
#if NetFX
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Spatial;
using System.Data.Entity.Validation;
#endif

namespace FuseCP.EnterpriseServer.Context
{

    using Version = FuseCP.EnterpriseServer.Data.Entities.Version;
    using GlobalDnsRecord = FuseCP.EnterpriseServer.Data.Entities.GlobalDnsRecord;
    using BackgroundTask = FuseCP.EnterpriseServer.Data.Entities.BackgroundTask;
    using BackgroundTaskParameter = FuseCP.EnterpriseServer.Data.Entities.BackgroundTaskParameter;

    public partial class DbContextBase : DbContext, Data.IGenericDbContext
    {

#if NetCore
        public DbContextBase(Data.DbContext context): this(new Data.DbOptions<DbContextBase>(context)) { }
        public DbContextBase(DbContextOptions<DbContextBase> options): base(options) {
			if (options is Data.DbOptions<DbContextBase> opts) {
                DbType = opts.DbType;
                InitSeedData = opts.InitSeedData;
            }
        }
#elif NetFX
        public DbContextBase(Data.DbContext context): base(context.DbConnection, true) { 
            DbType = context.DbType;
            InitSeedData = context.InitSeedData;
			Database.Log += WriteToLog;
        }
#endif

		private void WriteToLog(string msg) => Log?.Invoke(msg);
		public Action<string> Log { get; set; }
        public Data.DbType DbType { get; set; } = Data.DbType.Unknown;
        public bool InitSeedData { get; set; } = false;

        public virtual DbSet<AccessToken> AccessTokens { get; set; }

        public virtual DbSet<AdditionalGroup> AdditionalGroups { get; set; }

        public virtual DbSet<AuditLog> AuditLogs { get; set; }

        public virtual DbSet<AuditLogSource> AuditLogSources { get; set; }

        public virtual DbSet<AuditLogTask> AuditLogTasks { get; set; }

        public virtual DbSet<BackgroundTask> BackgroundTasks { get; set; }

        public virtual DbSet<BackgroundTaskLog> BackgroundTaskLogs { get; set; }

        public virtual DbSet<BackgroundTaskParameter> BackgroundTaskParameters { get; set; }

        public virtual DbSet<BackgroundTaskStack> BackgroundTaskStacks { get; set; }

        public virtual DbSet<BlackBerryUser> BlackBerryUsers { get; set; }

        public virtual DbSet<Cluster> Clusters { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }

        public virtual DbSet<Crmuser> Crmusers { get; set; }

        public virtual DbSet<DmzIpaddress> DmzIpaddresses { get; set; }

        public virtual DbSet<Domain> Domains { get; set; }

        public virtual DbSet<DomainDnsRecord> DomainDnsRecords { get; set; }

        public virtual DbSet<EnterpriseFolder> EnterpriseFolders { get; set; }

        public virtual DbSet<EnterpriseFoldersOwaPermission> EnterpriseFoldersOwaPermissions { get; set; }

        public virtual DbSet<ExchangeAccount> ExchangeAccounts { get; set; }

        public virtual DbSet<ExchangeAccountEmailAddress> ExchangeAccountEmailAddresses { get; set; }

        public virtual DbSet<ExchangeDeletedAccount> ExchangeDeletedAccounts { get; set; }

        public virtual DbSet<ExchangeDisclaimer> ExchangeDisclaimers { get; set; }

        public virtual DbSet<ExchangeMailboxPlan> ExchangeMailboxPlans { get; set; }

        public virtual DbSet<ExchangeMailboxPlanRetentionPolicyTag> ExchangeMailboxPlanRetentionPolicyTags { get; set; }

        public virtual DbSet<ExchangeOrganization> ExchangeOrganizations { get; set; }

        public virtual DbSet<ExchangeOrganizationDomain> ExchangeOrganizationDomains { get; set; }

        public virtual DbSet<ExchangeOrganizationSetting> ExchangeOrganizationSettings { get; set; }

        public virtual DbSet<ExchangeOrganizationSsFolder> ExchangeOrganizationSsFolders { get; set; }

        public virtual DbSet<ExchangeRetentionPolicyTag> ExchangeRetentionPolicyTags { get; set; }

        public virtual DbSet<GlobalDnsRecord> GlobalDnsRecords { get; set; }

        public virtual DbSet<HostingPlan> HostingPlans { get; set; }

        public virtual DbSet<HostingPlanQuota> HostingPlanQuotas { get; set; }

        public virtual DbSet<HostingPlanResource> HostingPlanResources { get; set; }

        public virtual DbSet<Ipaddress> Ipaddresses { get; set; }

        public virtual DbSet<LyncUser> LyncUsers { get; set; }

        public virtual DbSet<LyncUserPlan> LyncUserPlans { get; set; }

        public virtual DbSet<Ocsuser> Ocsusers { get; set; }

        public virtual DbSet<Package> Packages { get; set; }

        public virtual DbSet<PackageAddon> PackageAddons { get; set; }

        public virtual DbSet<PackageIpaddress> PackageIpaddresses { get; set; }

        public virtual DbSet<PackageQuota> PackageQuotas { get; set; }

        public virtual DbSet<PackageResource> PackageResources { get; set; }

        public virtual DbSet<PackageSetting> PackageSettings { get; set; }

        public virtual DbSet<PackageVlan> PackageVlans { get; set; }

        public virtual DbSet<PackagesBandwidth> PackagesBandwidths { get; set; }

        public virtual DbSet<PackagesDiskspace> PackagesDiskspaces { get; set; }

        public virtual DbSet<PackagesTreeCache> PackagesTreeCaches { get; set; }

        public virtual DbSet<PrivateIpaddress> PrivateIpaddresses { get; set; }

        public virtual DbSet<PrivateNetworkVlan> PrivateNetworkVlans { get; set; }

        public virtual DbSet<Provider> Providers { get; set; }

        public virtual DbSet<Quota> Quotas { get; set; }

        public virtual DbSet<Rdscertificate> Rdscertificates { get; set; }

        public virtual DbSet<Rdscollection> Rdscollections { get; set; }

        public virtual DbSet<RdscollectionSetting> RdscollectionSettings { get; set; }

        public virtual DbSet<RdscollectionUser> RdscollectionUsers { get; set; }

        public virtual DbSet<Rdsmessage> Rdsmessages { get; set; }

        public virtual DbSet<Rdsserver> Rdsservers { get; set; }

        public virtual DbSet<RdsserverSetting> RdsserverSettings { get; set; }

        public virtual DbSet<ResourceGroup> ResourceGroups { get; set; }

        public virtual DbSet<ResourceGroupDnsRecord> ResourceGroupDnsRecords { get; set; }

        public virtual DbSet<Schedule> Schedules { get; set; }

        public virtual DbSet<ScheduleParameter> ScheduleParameters { get; set; }

        public virtual DbSet<ScheduleTask> ScheduleTasks { get; set; }

        public virtual DbSet<ScheduleTaskParameter> ScheduleTaskParameters { get; set; }

        public virtual DbSet<ScheduleTaskViewConfiguration> ScheduleTaskViewConfigurations { get; set; }

        public virtual DbSet<Server> Servers { get; set; }

        public virtual DbSet<Service> Services { get; set; }

        public virtual DbSet<ServiceDefaultProperty> ServiceDefaultProperties { get; set; }

        public virtual DbSet<ServiceItem> ServiceItems { get; set; }

        public virtual DbSet<ServiceItemProperty> ServiceItemProperties { get; set; }

        public virtual DbSet<ServiceItemType> ServiceItemTypes { get; set; }

        public virtual DbSet<ServiceProperty> ServiceProperties { get; set; }

        public virtual DbSet<SfBuser> SfBusers { get; set; }

        public virtual DbSet<SfBuserPlan> SfBuserPlans { get; set; }

        public virtual DbSet<Sslcertificate> Sslcertificates { get; set; }

        public virtual DbSet<StorageSpace> StorageSpaces { get; set; }

        public virtual DbSet<StorageSpaceFolder> StorageSpaceFolders { get; set; }

        public virtual DbSet<StorageSpaceLevel> StorageSpaceLevels { get; set; }

        public virtual DbSet<StorageSpaceLevelResourceGroup> StorageSpaceLevelResourceGroups { get; set; }

        public virtual DbSet<SupportServiceLevel> SupportServiceLevels { get; set; }

        public virtual DbSet<SystemSetting> SystemSettings { get; set; }

        public virtual DbSet<TempId> TempIds { get; set; }

        public virtual DbSet<Theme> Themes { get; set; }

        public virtual DbSet<ThemeSetting> ThemeSettings { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserSetting> UserSettings { get; set; }

        public virtual DbSet<UsersDetailed> UsersDetaileds { get; set; }

        public virtual DbSet<Version> Versions { get; set; }

        public virtual DbSet<VirtualGroup> VirtualGroups { get; set; }

        public virtual DbSet<VirtualService> VirtualServices { get; set; }

        public virtual DbSet<WebDavAccessToken> WebDavAccessTokens { get; set; }

        public virtual DbSet<WebDavPortalUsersSetting> WebDavPortalUsersSettings { get; set; }


#if NetCore
        private void ApplyConfiguration<TEntity>(ModelBuilder model, Data.EntityTypeConfiguration<TEntity> configuration) where TEntity: class
#elif NetFX
        private void ApplyConfiguration<TEntity>(DbModelBuilder model, Data.EntityTypeConfiguration<TEntity> configuration) where TEntity: class
#else 
        private void ApplyConfiguration<TEntity>(DummyModel model, Data.EntityTypeConfiguration<TEntity> configuration) where TEntity: class
#endif
        {
            configuration.DbType = DbType;
            configuration.InitSeedData = InitSeedData;
#if NetFX
			configuration.Configure();
#endif
            model.ApplyConfiguration(configuration);
        }

#if NetCore
		static ConcurrentDictionary<string, ServerVersion> serverVersions = new ConcurrentDictionary<string, ServerVersion>();

   	    protected void UseSqlServer(DbContextOptionsBuilder builder, string connectionString) => builder.UseSqlServer(connectionString);
        protected void UseSqlite(DbContextOptionsBuilder builder, string connectionString) => builder.UseSqlite(connectionString);
        protected void UseMySqlOrMariaDb(DbContextOptionsBuilder builder, string connectionString)
        {
            ServerVersion serverVersion = serverVersions.GetOrAdd(connectionString, connectionString => ServerVersion.AutoDetect(connectionString));
            builder.UseMySql(connectionString, serverVersion);
        }
        protected void UsePostgreSql(DbContextOptionsBuilder builder, string connectionString) => builder.UseNpgsql(connectionString);

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!(builder.Options is Data.DbOptions<DbContextBase> options)) throw new NotSupportedException("This type of Options is not supported");
  
            InitSeedData = options.InitSeedData;

			switch (options.DbType)
			{
				case Data.DbType.SqlServer:
					UseSqlServer(builder, options.ConnectionString);
					break;
				case Data.DbType.Sqlite:
					UseSqlite(builder, options.ConnectionString);
					break;
				case Data.DbType.MySql:
				case Data.DbType.MariaDb:
					UseMySqlOrMariaDb(builder, options.ConnectionString);
					break;
				case Data.DbType.PostgreSql:
					UsePostgreSql(builder, options.ConnectionString);
					break;
				default: throw new NotSupportedException("This DB flavor is not supported");
			}
            
            builder.LogTo(WriteToLog);
        }
#endif

#if NetCore
        protected override void OnModelCreating(ModelBuilder model) {
#elif NetFX
        protected override void OnModelCreating(DbModelBuilder model) {
#else
        protected void OnModelCreating(DummyModel model) {
#endif
			base.OnModelCreating(model);

#if ScaffoldDbContextEntities
            ApplyConfiguration(model, new AccessTokenConfiguration());
            ApplyConfiguration(model, new AdditionalGroupConfiguration());
            ApplyConfiguration(model, new AuditLogConfiguration());
            ApplyConfiguration(model, new AuditLogSourceConfiguration());
            ApplyConfiguration(model, new AuditLogTaskConfiguration());
            ApplyConfiguration(model, new BackgroundTaskConfiguration());
            ApplyConfiguration(model, new BackgroundTaskLogConfiguration());
            ApplyConfiguration(model, new BackgroundTaskParameterConfiguration());
            ApplyConfiguration(model, new BackgroundTaskStackConfiguration());
            ApplyConfiguration(model, new BlackBerryUserConfiguration());
            ApplyConfiguration(model, new BruteForceLogConfiguration());
            ApplyConfiguration(model, new ClusterConfiguration());
            ApplyConfiguration(model, new CommentConfiguration());
            ApplyConfiguration(model, new CrmuserConfiguration());
            ApplyConfiguration(model, new DmzIpaddressConfiguration());
            ApplyConfiguration(model, new DomainConfiguration());
            ApplyConfiguration(model, new DomainDnsRecordConfiguration());
            ApplyConfiguration(model, new EnterpriseFolderConfiguration());
            ApplyConfiguration(model, new EnterpriseFoldersOwaPermissionConfiguration());
            ApplyConfiguration(model, new ExchangeAccountConfiguration());
            ApplyConfiguration(model, new ExchangeAccountEmailAddressConfiguration());
            ApplyConfiguration(model, new ExchangeDeletedAccountConfiguration());
            ApplyConfiguration(model, new ExchangeDisclaimerConfiguration());
            ApplyConfiguration(model, new ExchangeMailboxPlanConfiguration());
            ApplyConfiguration(model, new ExchangeMailboxPlanRetentionPolicyTagConfiguration());
            ApplyConfiguration(model, new ExchangeOrganizationConfiguration());
            ApplyConfiguration(model, new ExchangeOrganizationDomainConfiguration());
            ApplyConfiguration(model, new ExchangeOrganizationSettingConfiguration());
            ApplyConfiguration(model, new ExchangeOrganizationSsFolderConfiguration());
            ApplyConfiguration(model, new ExchangeRetentionPolicyTagConfiguration());
            ApplyConfiguration(model, new GlobalDnsRecordConfiguration());
            ApplyConfiguration(model, new HostingPlanConfiguration());
            ApplyConfiguration(model, new HostingPlanQuotaConfiguration());
            ApplyConfiguration(model, new HostingPlanResourceConfiguration());
            ApplyConfiguration(model, new IpaddressConfiguration());
            ApplyConfiguration(model, new IpSecurityPolicyConfiguration());
            ApplyConfiguration(model, new LyncUserConfiguration());
            ApplyConfiguration(model, new LyncUserPlanConfiguration());
            ApplyConfiguration(model, new OcsuserConfiguration());
            ApplyConfiguration(model, new PackageConfiguration());
            ApplyConfiguration(model, new PackageAddonConfiguration());
            ApplyConfiguration(model, new PackageIpaddressConfiguration());
            ApplyConfiguration(model, new PackageQuotaConfiguration());
            ApplyConfiguration(model, new PackageResourceConfiguration());
            ApplyConfiguration(model, new PackageSettingConfiguration());
            ApplyConfiguration(model, new PackageVlanConfiguration());
            ApplyConfiguration(model, new PackagesBandwidthConfiguration());
            ApplyConfiguration(model, new PackagesDiskspaceConfiguration());
            ApplyConfiguration(model, new PackagesTreeCacheConfiguration());
            ApplyConfiguration(model, new PrivateIpaddressConfiguration());
            ApplyConfiguration(model, new PrivateNetworkVlanConfiguration());
            ApplyConfiguration(model, new ProviderConfiguration());
            ApplyConfiguration(model, new QuotaConfiguration());
            ApplyConfiguration(model, new RdscertificateConfiguration());
            ApplyConfiguration(model, new RdscollectionConfiguration());
            ApplyConfiguration(model, new RdscollectionSettingConfiguration());
            ApplyConfiguration(model, new RdscollectionUserConfiguration());
            ApplyConfiguration(model, new RdsmessageConfiguration());
            ApplyConfiguration(model, new RdsserverConfiguration());
            ApplyConfiguration(model, new RdsserverSettingConfiguration());
            ApplyConfiguration(model, new ResourceGroupConfiguration());
            ApplyConfiguration(model, new ResourceGroupDnsRecordConfiguration());
            ApplyConfiguration(model, new ScheduleConfiguration());
            ApplyConfiguration(model, new ScheduleParameterConfiguration());
            ApplyConfiguration(model, new ScheduleTaskConfiguration());
            ApplyConfiguration(model, new ScheduleTaskParameterConfiguration());
            ApplyConfiguration(model, new ScheduleTaskViewConfigurationConfiguration());
            ApplyConfiguration(model, new ServerConfiguration());
            ApplyConfiguration(model, new ServiceConfiguration());
            ApplyConfiguration(model, new ServiceDefaultPropertyConfiguration());
            ApplyConfiguration(model, new ServiceItemConfiguration());
            ApplyConfiguration(model, new ServiceItemPropertyConfiguration());
            ApplyConfiguration(model, new ServiceItemTypeConfiguration());
            ApplyConfiguration(model, new ServicePropertyConfiguration());
            ApplyConfiguration(model, new SfBuserConfiguration());
            ApplyConfiguration(model, new SfBuserPlanConfiguration());
            ApplyConfiguration(model, new SslcertificateConfiguration());
            ApplyConfiguration(model, new StorageSpaceConfiguration());
            ApplyConfiguration(model, new StorageSpaceFolderConfiguration());
            ApplyConfiguration(model, new StorageSpaceLevelConfiguration());
            ApplyConfiguration(model, new StorageSpaceLevelResourceGroupConfiguration());
            ApplyConfiguration(model, new SupportServiceLevelConfiguration());
            ApplyConfiguration(model, new SystemSettingConfiguration());
            ApplyConfiguration(model, new TempIdConfiguration());
            ApplyConfiguration(model, new ThemeConfiguration());
            ApplyConfiguration(model, new ThemeSettingConfiguration());
            ApplyConfiguration(model, new UserConfiguration());
            ApplyConfiguration(model, new UserSettingConfiguration());
            ApplyConfiguration(model, new UsersDetailedConfiguration());
            ApplyConfiguration(model, new VersionConfiguration());
            ApplyConfiguration(model, new VirtualGroupConfiguration());
            ApplyConfiguration(model, new VirtualServiceConfiguration());
            ApplyConfiguration(model, new WebDavAccessTokenConfiguration());
            ApplyConfiguration(model, new WebDavPortalUsersSettingConfiguration());

#endif

            OnModelCreatingPartial(model);
        }
#if NetCore
		partial void OnModelCreatingPartial(ModelBuilder model);
#elif NetFX
        partial void OnModelCreatingPartial(DbModelBuilder model);
#else 
        partial void OnModelCreatingPartial(DummyModel model);
#endif

    }
}

namespace FuseCP.EnterpriseServer.Data {

    using Version = FuseCP.EnterpriseServer.Data.Entities.Version;
    using GlobalDnsRecord = FuseCP.EnterpriseServer.Data.Entities.GlobalDnsRecord;
    using BackgroundTask = FuseCP.EnterpriseServer.Data.Entities.BackgroundTask;
    using BackgroundTaskParameter = FuseCP.EnterpriseServer.Data.Entities.BackgroundTaskParameter;

#if ScaffoldDbContextEntities
    public partial class DbContext {
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

        DbSet<Cluster> clusters = null;
        public virtual DbSet<Cluster> Clusters => clusters ??= Set<Cluster>();

        DbSet<Comment> comments = null;
        public virtual DbSet<Comment> Comments => comments ??= Set<Comment>();

        DbSet<Crmuser> crmusers = null;
        public virtual DbSet<Crmuser> Crmusers => crmusers ??= Set<Crmuser>();

        DbSet<DmzIpaddress> dmzIpaddresses = null;
        public virtual DbSet<DmzIpaddress> DmzIpaddresses => dmzIpaddresses ??= Set<DmzIpaddress>();

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

        DbSet<Ipaddress> ipaddresses = null;
        public virtual DbSet<Ipaddress> Ipaddresses => ipaddresses ??= Set<Ipaddress>();

        DbSet<LyncUser> lyncUsers = null;
        public virtual DbSet<LyncUser> LyncUsers => lyncUsers ??= Set<LyncUser>();

        DbSet<LyncUserPlan> lyncUserPlans = null;
        public virtual DbSet<LyncUserPlan> LyncUserPlans => lyncUserPlans ??= Set<LyncUserPlan>();

        DbSet<Ocsuser> ocsusers = null;
        public virtual DbSet<Ocsuser> Ocsusers => ocsusers ??= Set<Ocsuser>();

        DbSet<Package> packages = null;
        public virtual DbSet<Package> Packages => packages ??= Set<Package>();

        DbSet<PackageAddon> packageAddons = null;
        public virtual DbSet<PackageAddon> PackageAddons => packageAddons ??= Set<PackageAddon>();

        DbSet<PackageIpaddress> packageIpaddresses = null;
        public virtual DbSet<PackageIpaddress> PackageIpaddresses => packageIpaddresses ??= Set<PackageIpaddress>();

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

        DbSet<PrivateIpaddress> privateIpaddresses = null;
        public virtual DbSet<PrivateIpaddress> PrivateIpaddresses => privateIpaddresses ??= Set<PrivateIpaddress>();

        DbSet<PrivateNetworkVlan> privateNetworkVlans = null;
        public virtual DbSet<PrivateNetworkVlan> PrivateNetworkVlans => privateNetworkVlans ??= Set<PrivateNetworkVlan>();

        DbSet<Provider> providers = null;
        public virtual DbSet<Provider> Providers => providers ??= Set<Provider>();

        DbSet<Quota> quotas = null;
        public virtual DbSet<Quota> Quotas => quotas ??= Set<Quota>();

        DbSet<Rdscertificate> rdscertificates = null;
        public virtual DbSet<Rdscertificate> Rdscertificates => rdscertificates ??= Set<Rdscertificate>();

        DbSet<Rdscollection> rdscollections = null;
        public virtual DbSet<Rdscollection> Rdscollections => rdscollections ??= Set<Rdscollection>();

        DbSet<RdscollectionSetting> rdscollectionSettings = null;
        public virtual DbSet<RdscollectionSetting> RdscollectionSettings => rdscollectionSettings ??= Set<RdscollectionSetting>();

        DbSet<RdscollectionUser> rdscollectionUsers = null;
        public virtual DbSet<RdscollectionUser> RdscollectionUsers => rdscollectionUsers ??= Set<RdscollectionUser>();

        DbSet<Rdsmessage> rdsmessages = null;
        public virtual DbSet<Rdsmessage> Rdsmessages => rdsmessages ??= Set<Rdsmessage>();

        DbSet<Rdsserver> rdsservers = null;
        public virtual DbSet<Rdsserver> Rdsservers => rdsservers ??= Set<Rdsserver>();

        DbSet<RdsserverSetting> rdsserverSettings = null;
        public virtual DbSet<RdsserverSetting> RdsserverSettings => rdsserverSettings ??= Set<RdsserverSetting>();

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

        DbSet<SfBuser> sfBusers = null;
        public virtual DbSet<SfBuser> SfBusers => sfBusers ??= Set<SfBuser>();

        DbSet<SfBuserPlan> sfBuserPlans = null;
        public virtual DbSet<SfBuserPlan> SfBuserPlans => sfBuserPlans ??= Set<SfBuserPlan>();

        DbSet<Sslcertificate> sslcertificates = null;
        public virtual DbSet<Sslcertificate> Sslcertificates => sslcertificates ??= Set<Sslcertificate>();

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

        DbSet<TempId> tempIds = null;
        public virtual DbSet<TempId> TempIds => tempIds ??= Set<TempId>();

        DbSet<Theme> themes = null;
        public virtual DbSet<Theme> Themes => themes ??= Set<Theme>();

        DbSet<ThemeSetting> themeSettings = null;
        public virtual DbSet<ThemeSetting> ThemeSettings => themeSettings ??= Set<ThemeSetting>();

        DbSet<User> users = null;
        public virtual DbSet<User> Users => users ??= Set<User>();

        DbSet<UserSetting> userSettings = null;
        public virtual DbSet<UserSetting> UserSettings => userSettings ??= Set<UserSetting>();

        DbSet<UsersDetailed> usersDetaileds = null;
        public virtual DbSet<UsersDetailed> UsersDetaileds => usersDetaileds ??= Set<UsersDetailed>();

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
#endif
