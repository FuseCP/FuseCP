CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "AdditionalGroups" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_AdditionalGroup" PRIMARY KEY AUTOINCREMENT,
    "UserID" INTEGER NOT NULL,
    "GroupName" TEXT COLLATE NOCASE NULL
);

CREATE TABLE "AuditLog" (
    "RecordID" TEXT COLLATE NOCASE NOT NULL CONSTRAINT "PK_Log" PRIMARY KEY,
    "UserID" INTEGER NULL,
    "Username" TEXT NULL,
    "ItemID" INTEGER NULL,
    "SeverityID" INTEGER NOT NULL,
    "StartDate" TEXT NOT NULL,
    "FinishDate" TEXT NOT NULL,
    "SourceName" TEXT NOT NULL,
    "TaskName" TEXT NOT NULL,
    "ItemName" TEXT NULL,
    "ExecutionLog" TEXT NULL,
    "PackageID" INTEGER NULL
);

CREATE TABLE "AuditLogSources" (
    "SourceName" TEXT NOT NULL CONSTRAINT "PK_AuditLogSources" PRIMARY KEY
);

CREATE TABLE "AuditLogTasks" (
    "SourceName" TEXT NOT NULL,
    "TaskName" TEXT NOT NULL,
    "TaskDescription" TEXT NULL,
    CONSTRAINT "PK_LogActions" PRIMARY KEY ("SourceName", "TaskName")
);

CREATE TABLE "BackgroundTasks" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_BackgroundTask" PRIMARY KEY AUTOINCREMENT,
    "Guid" TEXT NOT NULL,
    "TaskID" TEXT NULL,
    "ScheduleID" INTEGER NOT NULL,
    "PackageID" INTEGER NOT NULL,
    "UserID" INTEGER NOT NULL,
    "EffectiveUserID" INTEGER NOT NULL,
    "TaskName" TEXT NULL,
    "ItemID" INTEGER NULL,
    "ItemName" TEXT NULL,
    "StartDate" TEXT NOT NULL,
    "FinishDate" TEXT NULL,
    "IndicatorCurrent" INTEGER NOT NULL,
    "IndicatorMaximum" INTEGER NOT NULL,
    "MaximumExecutionTime" INTEGER NOT NULL,
    "Source" TEXT NULL,
    "Severity" INTEGER NOT NULL,
    "Completed" INTEGER NULL,
    "NotifyOnComplete" INTEGER NULL,
    "Status" INTEGER NOT NULL
);

CREATE TABLE "Clusters" (
    "ClusterID" INTEGER NOT NULL CONSTRAINT "PK_Clusters" PRIMARY KEY AUTOINCREMENT,
    "ClusterName" TEXT NOT NULL
);

CREATE TABLE "ExchangeDeletedAccounts" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_ExchangeDeletedAccount" PRIMARY KEY AUTOINCREMENT,
    "AccountID" INTEGER NOT NULL,
    "OriginAT" INTEGER NOT NULL,
    "StoragePath" TEXT NULL,
    "FolderName" TEXT NULL,
    "FileName" TEXT NULL,
    "ExpirationDate" TEXT NOT NULL
);

CREATE TABLE "ExchangeDisclaimers" (
    "ExchangeDisclaimerId" INTEGER NOT NULL CONSTRAINT "PK_ExchangeDisclaimers" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "DisclaimerName" TEXT COLLATE NOCASE NOT NULL,
    "DisclaimerText" TEXT NULL
);

CREATE TABLE "ExchangeMailboxPlanRetentionPolicyTags" (
    "PlanTagID" INTEGER NOT NULL CONSTRAINT "PK_ExchangeMailboxPlanRetentionPolicyTag" PRIMARY KEY AUTOINCREMENT,
    "TagID" INTEGER NOT NULL,
    "MailboxPlanId" INTEGER NOT NULL
);

CREATE TABLE "ExchangeRetentionPolicyTags" (
    "TagID" INTEGER NOT NULL CONSTRAINT "PK_ExchangeRetentionPolicyTag" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "TagName" TEXT COLLATE NOCASE NULL,
    "TagType" INTEGER NOT NULL,
    "AgeLimitForRetention" INTEGER NOT NULL,
    "RetentionAction" INTEGER NOT NULL
);

CREATE TABLE "OCSUsers" (
    "OCSUserID" INTEGER NOT NULL CONSTRAINT "PK_OCSUsers" PRIMARY KEY AUTOINCREMENT,
    "AccountID" INTEGER NOT NULL,
    "InstanceID" TEXT COLLATE NOCASE NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "ModifiedDate" TEXT NOT NULL
);

CREATE TABLE "PackageSettings" (
    "PackageID" INTEGER NOT NULL,
    "SettingsName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyValue" TEXT NULL,
    CONSTRAINT "PK_PackageSettings" PRIMARY KEY ("PackageID", "SettingsName", "PropertyName")
);

CREATE TABLE "RDSCertificates" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_RDSCertificates" PRIMARY KEY AUTOINCREMENT,
    "ServiceId" INTEGER NOT NULL,
    "Content" TEXT NOT NULL,
    "Hash" TEXT NOT NULL,
    "FileName" TEXT NOT NULL,
    "ValidFrom" TEXT NULL,
    "ExpiryDate" TEXT NULL
);

CREATE TABLE "RDSCollections" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_RdsCollection" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "Name" TEXT COLLATE NOCASE NULL,
    "Description" TEXT NULL,
    "DisplayName" TEXT NULL
);

CREATE TABLE "RDSServerSettings" (
    "RdsServerId" INTEGER NOT NULL,
    "SettingsName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyValue" TEXT NULL,
    "ApplyUsers" INTEGER NOT NULL,
    "ApplyAdministrators" INTEGER NOT NULL,
    CONSTRAINT "PK_RDSServerSettings" PRIMARY KEY ("RdsServerId", "SettingsName", "PropertyName")
);

CREATE TABLE "ResourceGroups" (
    "GroupID" INTEGER NOT NULL CONSTRAINT "PK_ResourceGroups" PRIMARY KEY,
    "GroupName" TEXT COLLATE NOCASE NOT NULL,
    "GroupOrder" INTEGER NOT NULL DEFAULT 1,
    "GroupController" TEXT NULL,
    "ShowGroup" INTEGER NULL
);

CREATE TABLE "ScheduleTasks" (
    "TaskID" TEXT NOT NULL CONSTRAINT "PK_ScheduleTasks" PRIMARY KEY,
    "TaskType" TEXT NOT NULL,
    "RoleID" INTEGER NOT NULL
);

CREATE TABLE "SfBUserPlans" (
    "SfBUserPlanId" INTEGER NOT NULL CONSTRAINT "PK_SfBUserPlans" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "SfBUserPlanName" TEXT NOT NULL,
    "SfBUserPlanType" INTEGER NULL,
    "IM" INTEGER NOT NULL,
    "Mobility" INTEGER NOT NULL,
    "MobilityEnableOutsideVoice" INTEGER NOT NULL,
    "Federation" INTEGER NOT NULL,
    "Conferencing" INTEGER NOT NULL,
    "EnterpriseVoice" INTEGER NOT NULL,
    "VoicePolicy" TEXT COLLATE NOCASE NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "RemoteUserAccess" INTEGER NOT NULL DEFAULT 0,
    "PublicIMConnectivity" INTEGER NOT NULL DEFAULT 0,
    "AllowOrganizeMeetingsWithExternalAnonymous" INTEGER NOT NULL DEFAULT 0,
    "Telephony" INTEGER NULL,
    "ServerURI" TEXT NULL,
    "ArchivePolicy" TEXT COLLATE NOCASE NULL,
    "TelephonyDialPlanPolicy" TEXT COLLATE NOCASE NULL,
    "TelephonyVoicePolicy" TEXT COLLATE NOCASE NULL
);

CREATE TABLE "SfBUsers" (
    "SfBUserID" INTEGER NOT NULL CONSTRAINT "PK_SfBUsers" PRIMARY KEY AUTOINCREMENT,
    "AccountID" INTEGER NOT NULL,
    "SfBUserPlanID" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "ModifiedDate" TEXT NOT NULL,
    "SipAddress" TEXT NULL
);

CREATE TABLE "SSLCertificates" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_SSLCertificates" PRIMARY KEY AUTOINCREMENT,
    "UserID" INTEGER NOT NULL,
    "SiteID" INTEGER NOT NULL,
    "FriendlyName" TEXT NULL,
    "Hostname" TEXT NULL,
    "DistinguishedName" TEXT NULL,
    "CSR" TEXT NULL,
    "CSRLength" INTEGER NULL,
    "Certificate" TEXT NULL,
    "Hash" TEXT NULL,
    "Installed" INTEGER NULL,
    "IsRenewal" INTEGER NULL,
    "ValidFrom" TEXT NULL,
    "ExpiryDate" TEXT NULL,
    "SerialNumber" TEXT NULL,
    "Pfx" TEXT NULL,
    "PreviousId" INTEGER NULL
);

CREATE TABLE "StorageSpaceLevels" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_StorageSpaceLevel" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL
);

CREATE TABLE "SupportServiceLevels" (
    "LevelID" INTEGER NOT NULL CONSTRAINT "PK_SupportServiceLevel" PRIMARY KEY AUTOINCREMENT,
    "LevelName" TEXT NOT NULL,
    "LevelDescription" TEXT NULL
);

CREATE TABLE "SystemSettings" (
    "SettingsName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyValue" TEXT NULL,
    CONSTRAINT "PK_SystemSettings" PRIMARY KEY ("SettingsName", "PropertyName")
);

CREATE TABLE "TempIds" (
    "Key" INTEGER NOT NULL CONSTRAINT "PK_TempIds" PRIMARY KEY AUTOINCREMENT,
    "Created" TEXT NOT NULL,
    "Scope" TEXT NOT NULL,
    "Level" INTEGER NOT NULL,
    "Id" INTEGER NOT NULL,
    "Date" TEXT NOT NULL
);

CREATE TABLE "Themes" (
    "ThemeID" INTEGER NOT NULL CONSTRAINT "PK_Themes" PRIMARY KEY AUTOINCREMENT,
    "DisplayName" TEXT NULL,
    "LTRName" TEXT NULL,
    "RTLName" TEXT NULL,
    "Enabled" INTEGER NOT NULL,
    "DisplayOrder" INTEGER NOT NULL
);

CREATE TABLE "ThemeSettings" (
    "ThemeSettingID" INTEGER NOT NULL CONSTRAINT "PK_ThemeSettings" PRIMARY KEY AUTOINCREMENT,
    "ThemeID" INTEGER NOT NULL,
    "SettingsName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyValue" TEXT NOT NULL
);

CREATE TABLE "Users" (
    "UserID" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "OwnerID" INTEGER NULL,
    "RoleID" INTEGER NOT NULL,
    "StatusID" INTEGER NOT NULL,
    "IsDemo" INTEGER NOT NULL DEFAULT 0,
    "IsPeer" INTEGER NOT NULL DEFAULT 0,
    "Username" TEXT NULL,
    "Password" TEXT NULL,
    "FirstName" TEXT NULL,
    "LastName" TEXT NULL,
    "Email" TEXT NULL,
    "Created" TEXT NULL,
    "Changed" TEXT NULL,
    "Comments" TEXT NULL,
    "SecondaryEmail" TEXT NULL,
    "Address" TEXT NULL,
    "City" TEXT NULL,
    "State" TEXT NULL,
    "Country" TEXT NULL,
    "Zip" TEXT NULL,
    "PrimaryPhone" TEXT NULL,
    "SecondaryPhone" TEXT NULL,
    "Fax" TEXT NULL,
    "InstantMessenger" TEXT NULL,
    "HtmlMail" INTEGER NULL DEFAULT 1,
    "CompanyName" TEXT NULL,
    "EcommerceEnabled" INTEGER NULL,
    "AdditionalParams" TEXT NULL,
    "LoginStatusId" INTEGER NULL,
    "FailedLogins" INTEGER NULL,
    "SubscriberNumber" TEXT NULL,
    "OneTimePasswordState" INTEGER NULL,
    "MfaMode" INTEGER NOT NULL DEFAULT 0,
    "PinSecret" TEXT NULL,
    CONSTRAINT "FK_Users_Users" FOREIGN KEY ("OwnerID") REFERENCES "Users" ("UserID")
);

CREATE TABLE "Versions" (
    "DatabaseVersion" TEXT NOT NULL CONSTRAINT "PK_Versions" PRIMARY KEY,
    "BuildDate" TEXT NOT NULL
);

CREATE TABLE "BackgroundTaskLogs" (
    "LogID" INTEGER NOT NULL CONSTRAINT "PK_BackgroundTaskLog" PRIMARY KEY AUTOINCREMENT,
    "TaskID" INTEGER NOT NULL,
    "Date" TEXT NULL,
    "ExceptionStackTrace" TEXT NULL,
    "InnerTaskStart" INTEGER NULL,
    "Severity" INTEGER NULL,
    "Text" TEXT NULL,
    "TextIdent" INTEGER NULL,
    "XmlParameters" TEXT NULL,
    CONSTRAINT "FK_BackgroundTaskLog_Task" FOREIGN KEY ("TaskID") REFERENCES "BackgroundTasks" ("ID")
);

CREATE TABLE "BackgroundTaskParameters" (
    "ParameterID" INTEGER NOT NULL CONSTRAINT "PK_BackgroundTaskParameter" PRIMARY KEY AUTOINCREMENT,
    "TaskID" INTEGER NOT NULL,
    "Name" TEXT COLLATE NOCASE NULL,
    "SerializerValue" TEXT NULL,
    "TypeName" TEXT NULL,
    CONSTRAINT "FK_BackgrounTaskParameter_Task" FOREIGN KEY ("TaskID") REFERENCES "BackgroundTasks" ("ID")
);

CREATE TABLE "BackgroundTaskStack" (
    "TaskStackID" INTEGER NOT NULL CONSTRAINT "PK_BackgroundTaskStack" PRIMARY KEY AUTOINCREMENT,
    "TaskID" INTEGER NOT NULL,
    CONSTRAINT "FK_BackgroundTaskStack_Task" FOREIGN KEY ("TaskID") REFERENCES "BackgroundTasks" ("ID")
);

CREATE TABLE "RDSCollectionSettings" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_RDSCollectionSettings" PRIMARY KEY AUTOINCREMENT,
    "RDSCollectionId" INTEGER NOT NULL,
    "DisconnectedSessionLimitMin" INTEGER NULL,
    "ActiveSessionLimitMin" INTEGER NULL,
    "IdleSessionLimitMin" INTEGER NULL,
    "BrokenConnectionAction" TEXT NULL,
    "AutomaticReconnectionEnabled" INTEGER NULL,
    "TemporaryFoldersDeletedOnExit" INTEGER NULL,
    "TemporaryFoldersPerSession" INTEGER NULL,
    "ClientDeviceRedirectionOptions" TEXT NULL,
    "ClientPrinterRedirected" INTEGER NULL,
    "ClientPrinterAsDefault" INTEGER NULL,
    "RDEasyPrintDriverEnabled" INTEGER NULL,
    "MaxRedirectedMonitors" INTEGER NULL,
    "SecurityLayer" TEXT NULL,
    "EncryptionLevel" TEXT NULL,
    "AuthenticateUsingNLA" INTEGER NULL,
    CONSTRAINT "FK_RDSCollectionSettings_RDSCollections" FOREIGN KEY ("RDSCollectionId") REFERENCES "RDSCollections" ("ID") ON DELETE CASCADE
);

CREATE TABLE "RDSMessages" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RDSMessages" PRIMARY KEY AUTOINCREMENT,
    "RDSCollectionId" INTEGER NOT NULL,
    "MessageText" TEXT NOT NULL,
    "UserName" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    CONSTRAINT "FK_RDSMessages_RDSCollections" FOREIGN KEY ("RDSCollectionId") REFERENCES "RDSCollections" ("ID") ON DELETE CASCADE
);

CREATE TABLE "RDSServers" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_RdsServer" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NULL,
    "Name" TEXT NULL,
    "FqdName" TEXT NULL,
    "Description" TEXT NULL,
    "RDSCollectionId" INTEGER NULL,
    "ConnectionEnabled" INTEGER NOT NULL DEFAULT 1,
    "Controller" INTEGER NULL,
    CONSTRAINT "FK_RDSServers_RDSCollectionId" FOREIGN KEY ("RDSCollectionId") REFERENCES "RDSCollections" ("ID")
);

CREATE TABLE "Providers" (
    "ProviderID" INTEGER NOT NULL CONSTRAINT "PK_Provider" PRIMARY KEY,
    "GroupID" INTEGER NOT NULL,
    "ProviderName" TEXT NULL,
    "DisplayName" TEXT NOT NULL,
    "ProviderType" TEXT NULL,
    "EditorControl" TEXT NULL,
    "DisableAutoDiscovery" INTEGER NULL,
    CONSTRAINT "FK_Providers_ResourceGroups" FOREIGN KEY ("GroupID") REFERENCES "ResourceGroups" ("GroupID")
);

CREATE TABLE "ResourceGroupDnsRecords" (
    "RecordID" INTEGER NOT NULL CONSTRAINT "PK_ResourceGroupDnsRecords" PRIMARY KEY AUTOINCREMENT,
    "RecordOrder" INTEGER NOT NULL DEFAULT 1,
    "GroupID" INTEGER NOT NULL,
    "RecordType" TEXT COLLATE NOCASE NOT NULL,
    "RecordName" TEXT NOT NULL,
    "RecordData" TEXT NOT NULL,
    "MXPriority" INTEGER NULL,
    CONSTRAINT "FK_ResourceGroupDnsRecords_ResourceGroups" FOREIGN KEY ("GroupID") REFERENCES "ResourceGroups" ("GroupID") ON DELETE CASCADE
);

CREATE TABLE "Servers" (
    "ServerID" INTEGER NOT NULL CONSTRAINT "PK_Servers" PRIMARY KEY AUTOINCREMENT,
    "ServerName" TEXT NOT NULL,
    "ServerUrl" TEXT NULL DEFAULT '',
    "Password" TEXT NULL,
    "Comments" TEXT NULL,
    "VirtualServer" INTEGER NOT NULL DEFAULT 0,
    "InstantDomainAlias" TEXT NULL,
    "PrimaryGroupID" INTEGER NULL,
    "ADRootDomain" TEXT NULL,
    "ADUsername" TEXT NULL,
    "ADPassword" TEXT NULL,
    "ADAuthenticationType" TEXT COLLATE NOCASE NULL,
    "ADEnabled" INTEGER NULL DEFAULT 0,
    "ADParentDomain" TEXT NULL,
    "ADParentDomainController" TEXT NULL,
    "OSPlatform" INTEGER NOT NULL DEFAULT 0,
    "IsCore" INTEGER NULL,
    "PasswordIsSHA256" INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT "FK_Servers_ResourceGroups" FOREIGN KEY ("PrimaryGroupID") REFERENCES "ResourceGroups" ("GroupID")
);

CREATE TABLE "ServiceItemTypes" (
    "ItemTypeID" INTEGER NOT NULL CONSTRAINT "PK_ServiceItemTypes" PRIMARY KEY,
    "GroupID" INTEGER NULL,
    "DisplayName" TEXT NULL,
    "TypeName" TEXT NULL,
    "TypeOrder" INTEGER NOT NULL DEFAULT 1,
    "CalculateDiskspace" INTEGER NULL,
    "CalculateBandwidth" INTEGER NULL,
    "Suspendable" INTEGER NULL,
    "Disposable" INTEGER NULL,
    "Searchable" INTEGER NULL,
    "Importable" INTEGER NOT NULL DEFAULT 1,
    "Backupable" INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT "FK_ServiceItemTypes_ResourceGroups" FOREIGN KEY ("GroupID") REFERENCES "ResourceGroups" ("GroupID")
);

CREATE TABLE "ScheduleTaskParameters" (
    "TaskID" TEXT NOT NULL,
    "ParameterID" TEXT COLLATE NOCASE NOT NULL,
    "DataTypeID" TEXT NOT NULL,
    "DefaultValue" TEXT NULL,
    "ParameterOrder" INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT "PK_ScheduleTaskParameters" PRIMARY KEY ("TaskID", "ParameterID"),
    CONSTRAINT "FK_ScheduleTaskParameters_ScheduleTasks" FOREIGN KEY ("TaskID") REFERENCES "ScheduleTasks" ("TaskID")
);

CREATE TABLE "ScheduleTaskViewConfiguration" (
    "TaskID" TEXT NOT NULL,
    "ConfigurationID" TEXT COLLATE NOCASE NOT NULL,
    "Environment" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    CONSTRAINT "PK_ScheduleTaskViewConfiguration" PRIMARY KEY ("ConfigurationID", "TaskID"),
    CONSTRAINT "FK_ScheduleTaskViewConfiguration_ScheduleTaskViewConfiguration" FOREIGN KEY ("TaskID") REFERENCES "ScheduleTasks" ("TaskID")
);

CREATE TABLE "StorageSpaceLevelResourceGroups" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_StorageSpaceLevelResourceGroup" PRIMARY KEY AUTOINCREMENT,
    "LevelId" INTEGER NOT NULL,
    "GroupId" INTEGER NOT NULL,
    CONSTRAINT "FK_StorageSpaceLevelResourceGroups_GroupId" FOREIGN KEY ("GroupId") REFERENCES "ResourceGroups" ("GroupID") ON DELETE CASCADE,
    CONSTRAINT "FK_StorageSpaceLevelResourceGroups_LevelId" FOREIGN KEY ("LevelId") REFERENCES "StorageSpaceLevels" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Comments" (
    "CommentID" INTEGER NOT NULL CONSTRAINT "PK_Comments" PRIMARY KEY AUTOINCREMENT,
    "ItemTypeID" TEXT COLLATE NOCASE NOT NULL,
    "ItemID" INTEGER NOT NULL,
    "UserID" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "CommentText" TEXT NULL,
    "SeverityID" INTEGER NULL,
    CONSTRAINT "FK_Comments_Users" FOREIGN KEY ("UserID") REFERENCES "Users" ("UserID") ON DELETE CASCADE
);

CREATE TABLE "UserSettings" (
    "UserID" INTEGER NOT NULL,
    "SettingsName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyValue" TEXT NULL,
    CONSTRAINT "PK_UserSettings" PRIMARY KEY ("UserID", "SettingsName", "PropertyName"),
    CONSTRAINT "FK_UserSettings_Users" FOREIGN KEY ("UserID") REFERENCES "Users" ("UserID") ON DELETE CASCADE
);

CREATE TABLE "ServiceDefaultProperties" (
    "ProviderID" INTEGER NOT NULL,
    "PropertyName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyValue" TEXT NULL,
    CONSTRAINT "PK_ServiceDefaultProperties" PRIMARY KEY ("ProviderID", "PropertyName"),
    CONSTRAINT "FK_ServiceDefaultProperties_Providers" FOREIGN KEY ("ProviderID") REFERENCES "Providers" ("ProviderID")
);

CREATE TABLE "HostingPlans" (
    "PlanID" INTEGER NOT NULL CONSTRAINT "PK_HostingPlans" PRIMARY KEY AUTOINCREMENT,
    "UserID" INTEGER NULL,
    "PackageID" INTEGER NULL,
    "ServerID" INTEGER NULL,
    "PlanName" TEXT COLLATE NOCASE NOT NULL,
    "PlanDescription" TEXT NULL,
    "Available" INTEGER NOT NULL,
    "SetupPrice" TEXT NULL,
    "RecurringPrice" TEXT NULL,
    "RecurrenceUnit" INTEGER NULL,
    "RecurrenceLength" INTEGER NULL,
    "IsAddon" INTEGER NULL,
    CONSTRAINT "FK_HostingPlans_Servers" FOREIGN KEY ("ServerID") REFERENCES "Servers" ("ServerID"),
    CONSTRAINT "FK_HostingPlans_Users" FOREIGN KEY ("UserID") REFERENCES "Users" ("UserID")
);

CREATE TABLE "IPAddresses" (
    "AddressID" INTEGER NOT NULL CONSTRAINT "PK_IPAddresses" PRIMARY KEY AUTOINCREMENT,
    "ExternalIP" TEXT COLLATE NOCASE NOT NULL,
    "InternalIP" TEXT COLLATE NOCASE NULL,
    "ServerID" INTEGER NULL,
    "Comments" TEXT NULL,
    "SubnetMask" TEXT COLLATE NOCASE NULL,
    "DefaultGateway" TEXT COLLATE NOCASE NULL,
    "PoolID" INTEGER NULL,
    "VLAN" INTEGER NULL,
    CONSTRAINT "FK_IPAddresses_Servers" FOREIGN KEY ("ServerID") REFERENCES "Servers" ("ServerID") ON DELETE CASCADE
);

CREATE TABLE "PrivateNetworkVLANs" (
    "VlanID" INTEGER NOT NULL CONSTRAINT "PK_PrivateNetworkVlan" PRIMARY KEY AUTOINCREMENT,
    "Vlan" INTEGER NOT NULL,
    "ServerID" INTEGER NULL,
    "Comments" TEXT NULL,
    CONSTRAINT "FK_ServerID" FOREIGN KEY ("ServerID") REFERENCES "Servers" ("ServerID") ON DELETE CASCADE
);

CREATE TABLE "Services" (
    "ServiceID" INTEGER NOT NULL CONSTRAINT "PK_Services" PRIMARY KEY AUTOINCREMENT,
    "ServerID" INTEGER NOT NULL,
    "ProviderID" INTEGER NOT NULL,
    "ServiceName" TEXT NOT NULL,
    "Comments" TEXT NULL,
    "ServiceQuotaValue" INTEGER NULL,
    "ClusterID" INTEGER NULL,
    CONSTRAINT "FK_Services_Clusters" FOREIGN KEY ("ClusterID") REFERENCES "Clusters" ("ClusterID"),
    CONSTRAINT "FK_Services_Providers" FOREIGN KEY ("ProviderID") REFERENCES "Providers" ("ProviderID"),
    CONSTRAINT "FK_Services_Servers" FOREIGN KEY ("ServerID") REFERENCES "Servers" ("ServerID")
);

CREATE TABLE "VirtualGroups" (
    "VirtualGroupID" INTEGER NOT NULL CONSTRAINT "PK_VirtualGroups" PRIMARY KEY AUTOINCREMENT,
    "ServerID" INTEGER NOT NULL,
    "GroupID" INTEGER NOT NULL,
    "DistributionType" INTEGER NULL,
    "BindDistributionToPrimary" INTEGER NULL,
    CONSTRAINT "FK_VirtualGroups_ResourceGroups" FOREIGN KEY ("GroupID") REFERENCES "ResourceGroups" ("GroupID"),
    CONSTRAINT "FK_VirtualGroups_Servers" FOREIGN KEY ("ServerID") REFERENCES "Servers" ("ServerID") ON DELETE CASCADE
);

CREATE TABLE "Quotas" (
    "QuotaID" INTEGER NOT NULL CONSTRAINT "PK_Quotas" PRIMARY KEY,
    "GroupID" INTEGER NOT NULL,
    "QuotaOrder" REAL NOT NULL DEFAULT 1.0,
    "QuotaName" TEXT COLLATE NOCASE NOT NULL,
    "QuotaDescription" TEXT NULL,
    "QuotaTypeID" INTEGER NOT NULL DEFAULT 2,
    "ServiceQuota" INTEGER NULL DEFAULT 0,
    "ItemTypeID" INTEGER NULL,
    "HideQuota" INTEGER NULL,
    "PerOrganization" INTEGER NULL,
    CONSTRAINT "FK_Quotas_ResourceGroups" FOREIGN KEY ("GroupID") REFERENCES "ResourceGroups" ("GroupID") ON DELETE CASCADE,
    CONSTRAINT "FK_Quotas_ServiceItemTypes" FOREIGN KEY ("ItemTypeID") REFERENCES "ServiceItemTypes" ("ItemTypeID")
);

CREATE TABLE "HostingPlanResources" (
    "PlanID" INTEGER NOT NULL,
    "GroupID" INTEGER NOT NULL,
    "CalculateDiskSpace" INTEGER NULL,
    "CalculateBandwidth" INTEGER NULL,
    CONSTRAINT "PK_HostingPlanResources" PRIMARY KEY ("PlanID", "GroupID"),
    CONSTRAINT "FK_HostingPlanResources_HostingPlans" FOREIGN KEY ("PlanID") REFERENCES "HostingPlans" ("PlanID") ON DELETE CASCADE,
    CONSTRAINT "FK_HostingPlanResources_ResourceGroups" FOREIGN KEY ("GroupID") REFERENCES "ResourceGroups" ("GroupID")
);

CREATE TABLE "Packages" (
    "PackageID" INTEGER NOT NULL CONSTRAINT "PK_Packages" PRIMARY KEY AUTOINCREMENT,
    "ParentPackageID" INTEGER NULL,
    "UserID" INTEGER NOT NULL,
    "PackageName" TEXT NULL,
    "PackageComments" TEXT NULL,
    "ServerID" INTEGER NULL,
    "StatusID" INTEGER NOT NULL,
    "PlanID" INTEGER NULL,
    "PurchaseDate" TEXT NULL,
    "OverrideQuotas" INTEGER NOT NULL DEFAULT 0,
    "BandwidthUpdated" TEXT NULL,
    "DefaultTopPackage" INTEGER NOT NULL DEFAULT 0,
    "StatusIDchangeDate" TEXT NOT NULL,
    CONSTRAINT "FK_Packages_HostingPlans" FOREIGN KEY ("PlanID") REFERENCES "HostingPlans" ("PlanID") ON DELETE CASCADE,
    CONSTRAINT "FK_Packages_Packages" FOREIGN KEY ("ParentPackageID") REFERENCES "Packages" ("PackageID"),
    CONSTRAINT "FK_Packages_Servers" FOREIGN KEY ("ServerID") REFERENCES "Servers" ("ServerID"),
    CONSTRAINT "FK_Packages_Users" FOREIGN KEY ("UserID") REFERENCES "Users" ("UserID")
);

CREATE TABLE "ServiceProperties" (
    "ServiceID" INTEGER NOT NULL,
    "PropertyName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyValue" TEXT COLLATE NOCASE NULL,
    CONSTRAINT "PK_ServiceProperties" PRIMARY KEY ("ServiceID", "PropertyName"),
    CONSTRAINT "FK_ServiceProperties_Services" FOREIGN KEY ("ServiceID") REFERENCES "Services" ("ServiceID") ON DELETE CASCADE
);

CREATE TABLE "StorageSpaces" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_StorageSpace" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "ServiceId" INTEGER NOT NULL,
    "ServerId" INTEGER NOT NULL,
    "LevelId" INTEGER NOT NULL,
    "Path" TEXT NOT NULL,
    "IsShared" INTEGER NOT NULL,
    "UncPath" TEXT NULL,
    "FsrmQuotaType" INTEGER NOT NULL,
    "FsrmQuotaSizeBytes" INTEGER NOT NULL,
    "IsDisabled" INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT "FK_StorageSpaces_ServerId" FOREIGN KEY ("ServerId") REFERENCES "Servers" ("ServerID") ON DELETE CASCADE,
    CONSTRAINT "FK_StorageSpaces_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("ServiceID") ON DELETE CASCADE
);

CREATE TABLE "VirtualServices" (
    "VirtualServiceID" INTEGER NOT NULL CONSTRAINT "PK_VirtualServices" PRIMARY KEY AUTOINCREMENT,
    "ServerID" INTEGER NOT NULL,
    "ServiceID" INTEGER NOT NULL,
    CONSTRAINT "FK_VirtualServices_Servers" FOREIGN KEY ("ServerID") REFERENCES "Servers" ("ServerID") ON DELETE CASCADE,
    CONSTRAINT "FK_VirtualServices_Services" FOREIGN KEY ("ServiceID") REFERENCES "Services" ("ServiceID")
);

CREATE TABLE "HostingPlanQuotas" (
    "PlanID" INTEGER NOT NULL,
    "QuotaID" INTEGER NOT NULL,
    "QuotaValue" INTEGER NOT NULL,
    CONSTRAINT "PK_HostingPlanQuota" PRIMARY KEY ("PlanID", "QuotaID"),
    CONSTRAINT "FK_HostingPlanQuotas_HostingPlans" FOREIGN KEY ("PlanID") REFERENCES "HostingPlans" ("PlanID") ON DELETE CASCADE,
    CONSTRAINT "FK_HostingPlanQuotas_Quotas" FOREIGN KEY ("QuotaID") REFERENCES "Quotas" ("QuotaID")
);

CREATE TABLE "GlobalDnsRecords" (
    "RecordID" INTEGER NOT NULL CONSTRAINT "PK_GlobalDnsRecord" PRIMARY KEY AUTOINCREMENT,
    "RecordType" TEXT COLLATE NOCASE NOT NULL,
    "RecordName" TEXT NOT NULL,
    "RecordData" TEXT NOT NULL,
    "MXPriority" INTEGER NOT NULL,
    "ServiceID" INTEGER NULL,
    "ServerID" INTEGER NULL,
    "PackageID" INTEGER NULL,
    "IPAddressID" INTEGER NULL,
    "SrvPriority" INTEGER NULL,
    "SrvWeight" INTEGER NULL,
    "SrvPort" INTEGER NULL,
    CONSTRAINT "FK_GlobalDnsRecords_IPAddresses" FOREIGN KEY ("IPAddressID") REFERENCES "IPAddresses" ("AddressID"),
    CONSTRAINT "FK_GlobalDnsRecords_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID") ON DELETE CASCADE,
    CONSTRAINT "FK_GlobalDnsRecords_Servers" FOREIGN KEY ("ServerID") REFERENCES "Servers" ("ServerID"),
    CONSTRAINT "FK_GlobalDnsRecords_Services" FOREIGN KEY ("ServiceID") REFERENCES "Services" ("ServiceID") ON DELETE CASCADE
);

CREATE TABLE "PackageAddons" (
    "PackageAddonID" INTEGER NOT NULL CONSTRAINT "PK_PackageAddons" PRIMARY KEY AUTOINCREMENT,
    "PackageID" INTEGER NULL,
    "PlanID" INTEGER NULL,
    "Quantity" INTEGER NULL,
    "PurchaseDate" TEXT NULL,
    "Comments" TEXT NULL,
    "StatusID" INTEGER NULL,
    CONSTRAINT "FK_PackageAddons_HostingPlans" FOREIGN KEY ("PlanID") REFERENCES "HostingPlans" ("PlanID"),
    CONSTRAINT "FK_PackageAddons_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID") ON DELETE CASCADE
);

CREATE TABLE "PackageQuotas" (
    "PackageID" INTEGER NOT NULL,
    "QuotaID" INTEGER NOT NULL,
    "QuotaValue" INTEGER NOT NULL,
    CONSTRAINT "PK_PackageQuotas" PRIMARY KEY ("PackageID", "QuotaID"),
    CONSTRAINT "FK_PackageQuotas_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID"),
    CONSTRAINT "FK_PackageQuotas_Quotas" FOREIGN KEY ("QuotaID") REFERENCES "Quotas" ("QuotaID")
);

CREATE TABLE "PackageResources" (
    "PackageID" INTEGER NOT NULL,
    "GroupID" INTEGER NOT NULL,
    "CalculateDiskspace" INTEGER NOT NULL,
    "CalculateBandwidth" INTEGER NOT NULL,
    CONSTRAINT "PK_PackageResources" PRIMARY KEY ("PackageID", "GroupID"),
    CONSTRAINT "FK_PackageResources_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID"),
    CONSTRAINT "FK_PackageResources_ResourceGroups" FOREIGN KEY ("GroupID") REFERENCES "ResourceGroups" ("GroupID")
);

CREATE TABLE "PackagesBandwidth" (
    "PackageID" INTEGER NOT NULL,
    "GroupID" INTEGER NOT NULL,
    "LogDate" TEXT NOT NULL,
    "BytesSent" INTEGER NOT NULL,
    "BytesReceived" INTEGER NOT NULL,
    CONSTRAINT "PK_PackagesBandwidth" PRIMARY KEY ("PackageID", "GroupID", "LogDate"),
    CONSTRAINT "FK_PackagesBandwidth_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID"),
    CONSTRAINT "FK_PackagesBandwidth_ResourceGroups" FOREIGN KEY ("GroupID") REFERENCES "ResourceGroups" ("GroupID")
);

CREATE TABLE "PackagesDiskspace" (
    "PackageID" INTEGER NOT NULL,
    "GroupID" INTEGER NOT NULL,
    "DiskSpace" INTEGER NOT NULL,
    CONSTRAINT "PK_PackagesDiskspace" PRIMARY KEY ("PackageID", "GroupID"),
    CONSTRAINT "FK_PackagesDiskspace_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID"),
    CONSTRAINT "FK_PackagesDiskspace_ResourceGroups" FOREIGN KEY ("GroupID") REFERENCES "ResourceGroups" ("GroupID")
);

CREATE TABLE "PackageServices" (
    "PackageID" INTEGER NOT NULL,
    "ServiceID" INTEGER NOT NULL,
    CONSTRAINT "PK_PackageServices" PRIMARY KEY ("PackageID", "ServiceID"),
    CONSTRAINT "FK_PackageServices_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID"),
    CONSTRAINT "FK_PackageServices_Services" FOREIGN KEY ("ServiceID") REFERENCES "Services" ("ServiceID")
);

CREATE TABLE "PackagesTreeCache" (
    "ParentPackageID" INTEGER NOT NULL,
    "PackageID" INTEGER NOT NULL,
    CONSTRAINT "PK_PackagesTreeCache" PRIMARY KEY ("ParentPackageID", "PackageID"),
    CONSTRAINT "FK_PackagesTreeCache_Packages" FOREIGN KEY ("ParentPackageID") REFERENCES "Packages" ("PackageID"),
    CONSTRAINT "FK_PackagesTreeCache_Packages1" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID")
);

CREATE TABLE "PackageVLANs" (
    "PackageVlanID" INTEGER NOT NULL CONSTRAINT "PK_PackageVlan" PRIMARY KEY AUTOINCREMENT,
    "VlanID" INTEGER NOT NULL,
    "PackageID" INTEGER NOT NULL,
    "IsDmz" INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT "FK_PackageID" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID") ON DELETE CASCADE,
    CONSTRAINT "FK_VlanID" FOREIGN KEY ("VlanID") REFERENCES "PrivateNetworkVLANs" ("VlanID") ON DELETE CASCADE
);

CREATE TABLE "Schedule" (
    "ScheduleID" INTEGER NOT NULL CONSTRAINT "PK_Schedule" PRIMARY KEY AUTOINCREMENT,
    "TaskID" TEXT NOT NULL,
    "PackageID" INTEGER NULL,
    "ScheduleName" TEXT NULL,
    "ScheduleTypeID" TEXT COLLATE NOCASE NULL,
    "Interval" INTEGER NULL,
    "FromTime" TEXT NULL,
    "ToTime" TEXT NULL,
    "StartTime" TEXT NULL,
    "LastRun" TEXT NULL,
    "NextRun" TEXT NULL,
    "Enabled" INTEGER NOT NULL,
    "PriorityID" TEXT NULL,
    "HistoriesNumber" INTEGER NULL,
    "MaxExecutionTime" INTEGER NULL,
    "WeekMonthDay" INTEGER NULL,
    CONSTRAINT "FK_Schedule_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID") ON DELETE CASCADE,
    CONSTRAINT "FK_Schedule_ScheduleTasks" FOREIGN KEY ("TaskID") REFERENCES "ScheduleTasks" ("TaskID")
);

CREATE TABLE "ServiceItems" (
    "ItemID" INTEGER NOT NULL CONSTRAINT "PK_ServiceItems" PRIMARY KEY AUTOINCREMENT,
    "PackageID" INTEGER NULL,
    "ItemTypeID" INTEGER NULL,
    "ServiceID" INTEGER NULL,
    "ItemName" TEXT NULL,
    "CreatedDate" TEXT NULL,
    CONSTRAINT "FK_ServiceItems_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID"),
    CONSTRAINT "FK_ServiceItems_ServiceItemTypes" FOREIGN KEY ("ItemTypeID") REFERENCES "ServiceItemTypes" ("ItemTypeID"),
    CONSTRAINT "FK_ServiceItems_Services" FOREIGN KEY ("ServiceID") REFERENCES "Services" ("ServiceID")
);

CREATE TABLE "StorageSpaceFolders" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_StorageSpaceFolder" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "StorageSpaceId" INTEGER NOT NULL,
    "Path" TEXT NOT NULL,
    "UncPath" TEXT NULL,
    "IsShared" INTEGER NOT NULL,
    "FsrmQuotaType" INTEGER NOT NULL,
    "FsrmQuotaSizeBytes" INTEGER NOT NULL,
    CONSTRAINT "FK_StorageSpaceFolders_StorageSpaceId" FOREIGN KEY ("StorageSpaceId") REFERENCES "StorageSpaces" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ScheduleParameters" (
    "ScheduleID" INTEGER NOT NULL,
    "ParameterID" TEXT COLLATE NOCASE NOT NULL,
    "ParameterValue" TEXT NULL,
    CONSTRAINT "PK_ScheduleParameters" PRIMARY KEY ("ScheduleID", "ParameterID"),
    CONSTRAINT "FK_ScheduleParameters_Schedule" FOREIGN KEY ("ScheduleID") REFERENCES "Schedule" ("ScheduleID") ON DELETE CASCADE
);

CREATE TABLE "DmzIPAddresses" (
    "DmzAddressID" INTEGER NOT NULL CONSTRAINT "PK_DmzIPAddresses" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "IPAddress" TEXT COLLATE NOCASE NOT NULL,
    "IsPrimary" INTEGER NOT NULL,
    CONSTRAINT "FK_DmzIPAddresses_ServiceItems" FOREIGN KEY ("ItemID") REFERENCES "ServiceItems" ("ItemID") ON DELETE CASCADE
);

CREATE TABLE "Domains" (
    "DomainID" INTEGER NOT NULL CONSTRAINT "PK_Domains" PRIMARY KEY AUTOINCREMENT,
    "PackageID" INTEGER NOT NULL,
    "ZoneItemID" INTEGER NULL,
    "DomainName" TEXT COLLATE NOCASE NOT NULL,
    "HostingAllowed" INTEGER NOT NULL DEFAULT 0,
    "WebSiteID" INTEGER NULL,
    "MailDomainID" INTEGER NULL,
    "IsSubDomain" INTEGER NOT NULL DEFAULT 0,
    "IsPreviewDomain" INTEGER NOT NULL DEFAULT 0,
    "IsDomainPointer" INTEGER NOT NULL,
    "DomainItemId" INTEGER NULL,
    "CreationDate" TEXT NULL,
    "ExpirationDate" TEXT NULL,
    "LastUpdateDate" TEXT NULL,
    "RegistrarName" TEXT NULL,
    CONSTRAINT "FK_Domains_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID") ON DELETE CASCADE,
    CONSTRAINT "FK_Domains_ServiceItems_MailDomain" FOREIGN KEY ("MailDomainID") REFERENCES "ServiceItems" ("ItemID"),
    CONSTRAINT "FK_Domains_ServiceItems_WebSite" FOREIGN KEY ("WebSiteID") REFERENCES "ServiceItems" ("ItemID"),
    CONSTRAINT "FK_Domains_ServiceItems_ZoneItem" FOREIGN KEY ("ZoneItemID") REFERENCES "ServiceItems" ("ItemID")
);

CREATE TABLE "ExchangeOrganizationDomains" (
    "OrganizationDomainID" INTEGER NOT NULL CONSTRAINT "PK_ExchangeOrganizationDomains" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "DomainID" INTEGER NULL,
    "IsHost" INTEGER NULL DEFAULT 0,
    "DomainTypeID" INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT "FK_ExchangeOrganizationDomains_ServiceItems" FOREIGN KEY ("ItemID") REFERENCES "ServiceItems" ("ItemID") ON DELETE CASCADE
);

CREATE TABLE "ExchangeOrganizations" (
    "ItemID" INTEGER NOT NULL CONSTRAINT "PK_ExchangeOrganizations" PRIMARY KEY,
    "OrganizationID" TEXT COLLATE NOCASE NOT NULL,
    "ExchangeMailboxPlanID" INTEGER NULL,
    "LyncUserPlanID" INTEGER NULL,
    "SfBUserPlanID" INTEGER NULL,
    CONSTRAINT "FK_ExchangeOrganizations_ServiceItems" FOREIGN KEY ("ItemID") REFERENCES "ServiceItems" ("ItemID") ON DELETE CASCADE
);

CREATE TABLE "PackageIPAddresses" (
    "PackageAddressID" INTEGER NOT NULL CONSTRAINT "PK_PackageIPAddresses" PRIMARY KEY AUTOINCREMENT,
    "PackageID" INTEGER NOT NULL,
    "AddressID" INTEGER NOT NULL,
    "ItemID" INTEGER NULL,
    "IsPrimary" INTEGER NULL,
    "OrgID" INTEGER NULL,
    CONSTRAINT "FK_PackageIPAddresses_IPAddresses" FOREIGN KEY ("AddressID") REFERENCES "IPAddresses" ("AddressID"),
    CONSTRAINT "FK_PackageIPAddresses_Packages" FOREIGN KEY ("PackageID") REFERENCES "Packages" ("PackageID") ON DELETE CASCADE,
    CONSTRAINT "FK_PackageIPAddresses_ServiceItems" FOREIGN KEY ("ItemID") REFERENCES "ServiceItems" ("ItemID")
);

CREATE TABLE "PrivateIPAddresses" (
    "PrivateAddressID" INTEGER NOT NULL CONSTRAINT "PK_PrivateIPAddresses" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "IPAddress" TEXT COLLATE NOCASE NOT NULL,
    "IsPrimary" INTEGER NOT NULL,
    CONSTRAINT "FK_PrivateIPAddresses_ServiceItems" FOREIGN KEY ("ItemID") REFERENCES "ServiceItems" ("ItemID") ON DELETE CASCADE
);

CREATE TABLE "ServiceItemProperties" (
    "ItemID" INTEGER NOT NULL,
    "PropertyName" TEXT COLLATE NOCASE NOT NULL,
    "PropertyValue" TEXT NULL,
    CONSTRAINT "PK_ServiceItemProperties" PRIMARY KEY ("ItemID", "PropertyName"),
    CONSTRAINT "FK_ServiceItemProperties_ServiceItems" FOREIGN KEY ("ItemID") REFERENCES "ServiceItems" ("ItemID") ON DELETE CASCADE
);

CREATE TABLE "EnterpriseFolders" (
    "EnterpriseFolderID" INTEGER NOT NULL CONSTRAINT "PK_EnterpriseFolders" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "FolderName" TEXT NOT NULL,
    "FolderQuota" INTEGER NOT NULL DEFAULT 0,
    "LocationDrive" TEXT NULL,
    "HomeFolder" TEXT NULL,
    "Domain" TEXT COLLATE NOCASE NULL,
    "StorageSpaceFolderId" INTEGER NULL,
    CONSTRAINT "FK_EnterpriseFolders_StorageSpaceFolderId" FOREIGN KEY ("StorageSpaceFolderId") REFERENCES "StorageSpaceFolders" ("Id") ON DELETE CASCADE
);

CREATE TABLE "DomainDnsRecords" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_DomainDnsRecord" PRIMARY KEY AUTOINCREMENT,
    "DomainId" INTEGER NOT NULL,
    "RecordType" INTEGER NOT NULL,
    "DnsServer" TEXT NULL,
    "Value" TEXT NULL,
    "Date" TEXT NULL,
    CONSTRAINT "FK_DomainDnsRecords_DomainId" FOREIGN KEY ("DomainId") REFERENCES "Domains" ("DomainID") ON DELETE CASCADE
);

CREATE TABLE "ExchangeMailboxPlans" (
    "MailboxPlanId" INTEGER NOT NULL CONSTRAINT "PK_ExchangeMailboxPlans" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "MailboxPlan" TEXT COLLATE NOCASE NOT NULL,
    "MailboxPlanType" INTEGER NULL,
    "EnableActiveSync" INTEGER NOT NULL,
    "EnableIMAP" INTEGER NOT NULL,
    "EnableMAPI" INTEGER NOT NULL,
    "EnableOWA" INTEGER NOT NULL,
    "EnablePOP" INTEGER NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "IssueWarningPct" INTEGER NOT NULL,
    "KeepDeletedItemsDays" INTEGER NOT NULL,
    "MailboxSizeMB" INTEGER NOT NULL,
    "MaxReceiveMessageSizeKB" INTEGER NOT NULL,
    "MaxRecipients" INTEGER NOT NULL,
    "MaxSendMessageSizeKB" INTEGER NOT NULL,
    "ProhibitSendPct" INTEGER NOT NULL,
    "ProhibitSendReceivePct" INTEGER NOT NULL,
    "HideFromAddressBook" INTEGER NOT NULL,
    "AllowLitigationHold" INTEGER NULL,
    "RecoverableItemsWarningPct" INTEGER NULL,
    "RecoverableItemsSpace" INTEGER NULL,
    "LitigationHoldUrl" TEXT NULL,
    "LitigationHoldMsg" TEXT NULL,
    "Archiving" INTEGER NULL,
    "EnableArchiving" INTEGER NULL,
    "ArchiveSizeMB" INTEGER NULL,
    "ArchiveWarningPct" INTEGER NULL,
    "EnableAutoReply" INTEGER NULL,
    "IsForJournaling" INTEGER NULL,
    "EnableForceArchiveDeletion" INTEGER NULL,
    CONSTRAINT "FK_ExchangeMailboxPlans_ExchangeOrganizations" FOREIGN KEY ("ItemID") REFERENCES "ExchangeOrganizations" ("ItemID") ON DELETE CASCADE
);

CREATE TABLE "ExchangeOrganizationSettings" (
    "ItemId" INTEGER NOT NULL,
    "SettingsName" TEXT COLLATE NOCASE NOT NULL,
    "Xml" TEXT NOT NULL,
    CONSTRAINT "PK_ExchangeOrganizationSettings" PRIMARY KEY ("ItemId", "SettingsName"),
    CONSTRAINT "FK_ExchangeOrganizationSettings_ExchangeOrganizations_ItemId" FOREIGN KEY ("ItemId") REFERENCES "ExchangeOrganizations" ("ItemID") ON DELETE CASCADE
);

CREATE TABLE "ExchangeOrganizationSsFolders" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ExchangeOrganizationSsFolder" PRIMARY KEY AUTOINCREMENT,
    "ItemId" INTEGER NOT NULL,
    "Type" TEXT COLLATE NOCASE NOT NULL,
    "StorageSpaceFolderId" INTEGER NOT NULL,
    CONSTRAINT "FK_ExchangeOrganizationSsFolders_ItemId" FOREIGN KEY ("ItemId") REFERENCES "ExchangeOrganizations" ("ItemID") ON DELETE CASCADE,
    CONSTRAINT "FK_ExchangeOrganizationSsFolders_StorageSpaceFolderId" FOREIGN KEY ("StorageSpaceFolderId") REFERENCES "StorageSpaceFolders" ("Id") ON DELETE CASCADE
);

CREATE TABLE "LyncUserPlans" (
    "LyncUserPlanId" INTEGER NOT NULL CONSTRAINT "PK_LyncUserPlans" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "LyncUserPlanName" TEXT COLLATE NOCASE NOT NULL,
    "LyncUserPlanType" INTEGER NULL,
    "IM" INTEGER NOT NULL,
    "Mobility" INTEGER NOT NULL,
    "MobilityEnableOutsideVoice" INTEGER NOT NULL,
    "Federation" INTEGER NOT NULL,
    "Conferencing" INTEGER NOT NULL,
    "EnterpriseVoice" INTEGER NOT NULL,
    "VoicePolicy" TEXT COLLATE NOCASE NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "RemoteUserAccess" INTEGER NOT NULL DEFAULT 0,
    "PublicIMConnectivity" INTEGER NOT NULL DEFAULT 0,
    "AllowOrganizeMeetingsWithExternalAnonymous" INTEGER NOT NULL DEFAULT 0,
    "Telephony" INTEGER NULL,
    "ServerURI" TEXT NULL,
    "ArchivePolicy" TEXT COLLATE NOCASE NULL,
    "TelephonyDialPlanPolicy" TEXT COLLATE NOCASE NULL,
    "TelephonyVoicePolicy" TEXT COLLATE NOCASE NULL,
    CONSTRAINT "FK_LyncUserPlans_ExchangeOrganizations" FOREIGN KEY ("ItemID") REFERENCES "ExchangeOrganizations" ("ItemID") ON DELETE CASCADE
);

CREATE TABLE "ExchangeAccounts" (
    "AccountID" INTEGER NOT NULL CONSTRAINT "PK_ExchangeAccounts" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "AccountType" INTEGER NOT NULL,
    "AccountName" TEXT NOT NULL,
    "DisplayName" TEXT NOT NULL,
    "PrimaryEmailAddress" TEXT NULL,
    "MailEnabledPublicFolder" INTEGER NULL,
    "MailboxManagerActions" TEXT NULL,
    "SamAccountName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "MailboxPlanId" INTEGER NULL,
    "SubscriberNumber" TEXT NULL,
    "UserPrincipalName" TEXT NULL,
    "ExchangeDisclaimerId" INTEGER NULL,
    "ArchivingMailboxPlanId" INTEGER NULL,
    "EnableArchiving" INTEGER NULL,
    "LevelID" INTEGER NULL,
    "IsVIP" INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT "FK_ExchangeAccounts_ExchangeMailboxPlans" FOREIGN KEY ("MailboxPlanId") REFERENCES "ExchangeMailboxPlans" ("MailboxPlanId"),
    CONSTRAINT "FK_ExchangeAccounts_ServiceItems" FOREIGN KEY ("ItemID") REFERENCES "ServiceItems" ("ItemID") ON DELETE CASCADE
);

CREATE TABLE "LyncUsers" (
    "LyncUserID" INTEGER NOT NULL CONSTRAINT "PK_LyncUsers" PRIMARY KEY AUTOINCREMENT,
    "AccountID" INTEGER NOT NULL,
    "LyncUserPlanID" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "ModifiedDate" TEXT NOT NULL,
    "SipAddress" TEXT COLLATE NOCASE NULL,
    CONSTRAINT "FK_LyncUsers_LyncUserPlans" FOREIGN KEY ("LyncUserPlanID") REFERENCES "LyncUserPlans" ("LyncUserPlanId")
);

CREATE TABLE "AccessTokens" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_AccessToken" PRIMARY KEY AUTOINCREMENT,
    "AccessTokenGuid" TEXT NOT NULL,
    "ExpirationDate" TEXT NOT NULL,
    "AccountID" INTEGER NOT NULL,
    "ItemId" INTEGER NOT NULL,
    "TokenType" INTEGER NOT NULL,
    "SmsResponse" TEXT NULL,
    CONSTRAINT "FK_AccessTokens_UserId" FOREIGN KEY ("AccountID") REFERENCES "ExchangeAccounts" ("AccountID") ON DELETE CASCADE
);

CREATE TABLE "BlackBerryUsers" (
    "BlackBerryUserId" INTEGER NOT NULL CONSTRAINT "PK_BlackBerryUsers" PRIMARY KEY AUTOINCREMENT,
    "AccountId" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "ModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_BlackBerryUsers_ExchangeAccounts" FOREIGN KEY ("AccountId") REFERENCES "ExchangeAccounts" ("AccountID")
);

CREATE TABLE "CRMUsers" (
    "CRMUserID" INTEGER NOT NULL CONSTRAINT "PK_CRMUsers" PRIMARY KEY AUTOINCREMENT,
    "AccountID" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "ChangedDate" TEXT NOT NULL,
    "CRMUserGuid" TEXT NULL,
    "BusinessUnitID" TEXT NULL,
    "CALType" INTEGER NULL,
    CONSTRAINT "FK_CRMUsers_ExchangeAccounts" FOREIGN KEY ("AccountID") REFERENCES "ExchangeAccounts" ("AccountID")
);

CREATE TABLE "EnterpriseFoldersOwaPermissions" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_EnterpriseFoldersOwaPermission" PRIMARY KEY AUTOINCREMENT,
    "ItemID" INTEGER NOT NULL,
    "FolderID" INTEGER NOT NULL,
    "AccountID" INTEGER NOT NULL,
    CONSTRAINT "FK_EnterpriseFoldersOwaPermissions_AccountId" FOREIGN KEY ("AccountID") REFERENCES "ExchangeAccounts" ("AccountID") ON DELETE CASCADE,
    CONSTRAINT "FK_EnterpriseFoldersOwaPermissions_FolderId" FOREIGN KEY ("FolderID") REFERENCES "EnterpriseFolders" ("EnterpriseFolderID") ON DELETE CASCADE
);

CREATE TABLE "ExchangeAccountEmailAddresses" (
    "AddressID" INTEGER NOT NULL CONSTRAINT "PK_ExchangeAccountEmailAddresses" PRIMARY KEY AUTOINCREMENT,
    "AccountID" INTEGER NOT NULL,
    "EmailAddress" TEXT NOT NULL,
    CONSTRAINT "FK_ExchangeAccountEmailAddresses_ExchangeAccounts" FOREIGN KEY ("AccountID") REFERENCES "ExchangeAccounts" ("AccountID") ON DELETE CASCADE
);

CREATE TABLE "RDSCollectionUsers" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_RdsCollectionUser" PRIMARY KEY AUTOINCREMENT,
    "RDSCollectionId" INTEGER NOT NULL,
    "AccountID" INTEGER NOT NULL,
    CONSTRAINT "FK_RDSCollectionUsers_RDSCollectionId" FOREIGN KEY ("RDSCollectionId") REFERENCES "RDSCollections" ("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_RDSCollectionUsers_UserId" FOREIGN KEY ("AccountID") REFERENCES "ExchangeAccounts" ("AccountID") ON DELETE CASCADE
);

CREATE TABLE "WebDavAccessTokens" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_WebDavAccessToken" PRIMARY KEY AUTOINCREMENT,
    "FilePath" TEXT NOT NULL,
    "AuthData" TEXT NOT NULL,
    "AccessToken" TEXT NOT NULL,
    "ExpirationDate" TEXT NOT NULL,
    "AccountID" INTEGER NOT NULL,
    "ItemId" INTEGER NOT NULL,
    CONSTRAINT "FK_WebDavAccessTokens_UserId" FOREIGN KEY ("AccountID") REFERENCES "ExchangeAccounts" ("AccountID") ON DELETE CASCADE
);

CREATE TABLE "WebDavPortalUsersSettings" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_WebDavPortalUsersSetting" PRIMARY KEY AUTOINCREMENT,
    "AccountId" INTEGER NOT NULL,
    "Settings" TEXT NULL,
    CONSTRAINT "FK_WebDavPortalUsersSettings_UserId" FOREIGN KEY ("AccountId") REFERENCES "ExchangeAccounts" ("AccountID") ON DELETE CASCADE
);

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('APP_INSTALLER');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('AUTO_DISCOVERY');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('BACKUP');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('DNS_ZONE');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('DOMAIN');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('ENTERPRISE_STORAGE');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('EXCHANGE');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('FILES');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('FTP_ACCOUNT');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('GLOBAL_DNS');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('HOSTING_SPACE');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('HOSTING_SPACE_WR');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('IMPORT');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('IP_ADDRESS');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('MAIL_ACCOUNT');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('MAIL_DOMAIN');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('MAIL_FORWARDING');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('MAIL_GROUP');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('MAIL_LIST');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('OCS');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('ODBC_DSN');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('ORGANIZATION');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('REMOTE_DESKTOP_SERVICES');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('SCHEDULER');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('SERVER');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('SHAREPOINT');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('SPACE');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('SQL_DATABASE');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('SQL_USER');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('STATS_SITE');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('STORAGE_SPACES');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('USER');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('VIRTUAL_SERVER');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('VLAN');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('VPS');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('VPS2012');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('WAG_INSTALLER');
SELECT changes();

INSERT INTO "AuditLogSources" ("SourceName")
VALUES ('WEB_SITE');
SELECT changes();


INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('APP_INSTALLER', 'INSTALL_APPLICATION', 'Install application');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('AUTO_DISCOVERY', 'IS_INSTALLED', 'Is installed');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('BACKUP', 'BACKUP', 'Backup');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('BACKUP', 'RESTORE', 'Restore');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('DNS_ZONE', 'ADD_RECORD', 'Add record');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('DNS_ZONE', 'DELETE_RECORD', 'Delete record');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('DNS_ZONE', 'UPDATE_RECORD', 'Update record');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('DOMAIN', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('DOMAIN', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('DOMAIN', 'ENABLE_DNS', 'Enable DNS');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('DOMAIN', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ENTERPRISE_STORAGE', 'CREATE_FOLDER', 'Create folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ENTERPRISE_STORAGE', 'CREATE_MAPPED_DRIVE', 'Create mapped drive');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ENTERPRISE_STORAGE', 'DELETE_FOLDER', 'Delete folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ENTERPRISE_STORAGE', 'DELETE_MAPPED_DRIVE', 'Delete mapped drive');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ENTERPRISE_STORAGE', 'GET_ORG_STATS', 'Get organization statistics');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ENTERPRISE_STORAGE', 'SET_ENTERPRISE_FOLDER_GENERAL_SETTINGS', 'Set enterprise folder general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'ADD_DISTR_LIST_ADDRESS', 'Add distribution list e-mail address');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'ADD_DOMAIN', 'Add organization domain');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'ADD_EXCHANGE_EXCHANGEDISCLAIMER', 'Add Exchange disclaimer');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'ADD_EXCHANGE_MAILBOXPLAN_RETENTIONPOLICY_ARCHIVING', 'Add Exchange archiving retention policy');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'ADD_EXCHANGE_RETENTIONPOLICYTAG', 'Add Exchange retention policy tag');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'ADD_MAILBOX_ADDRESS', 'Add mailbox e-mail address');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'ADD_PUBLIC_FOLDER_ADDRESS', 'Add public folder e-mail address');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'CALCULATE_DISKSPACE', 'Calculate organization disk space');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'CREATE_CONTACT', 'Create contact');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'CREATE_DISTR_LIST', 'Create distribution list');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'CREATE_MAILBOX', 'Create mailbox');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'CREATE_ORG', 'Create organization');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'CREATE_PUBLIC_FOLDER', 'Create public folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_CONTACT', 'Delete contact');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_DISTR_LIST', 'Delete distribution list');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_DISTR_LIST_ADDRESSES', 'Delete distribution list e-mail addresses');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_DOMAIN', 'Delete organization domain');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_EXCHANGE_MAILBOXPLAN_RETENTIONPOLICY_ARCHIV', 'Delete Exchange archiving retention policy');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_EXCHANGE_RETENTIONPOLICYTAG', 'Delete Exchange retention policy tag');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_MAILBOX', 'Delete mailbox');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_MAILBOX_ADDRESSES', 'Delete mailbox e-mail addresses');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_ORG', 'Delete organization');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_PUBLIC_FOLDER', 'Delete public folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DELETE_PUBLIC_FOLDER_ADDRESSES', 'Delete public folder e-mail addresses');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DISABLE_MAIL_PUBLIC_FOLDER', 'Disable mail public folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'DISABLE_MAILBOX', 'Disable Mailbox');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'ENABLE_MAIL_PUBLIC_FOLDER', 'Enable mail public folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_ACTIVESYNC_POLICY', 'Get Activesync policy');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_CONTACT_GENERAL', 'Get contact general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_CONTACT_MAILFLOW', 'Get contact mail flow settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_DISTR_LIST_ADDRESSES', 'Get distribution list e-mail addresses');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_DISTR_LIST_BYMEMBER', 'Get distributions list by member');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_DISTR_LIST_GENERAL', 'Get distribution list general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_DISTR_LIST_MAILFLOW', 'Get distribution list mail flow settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_DISTRIBUTION_LIST_RESULT', 'Get distributions list result');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_EXCHANGE_ACCOUNTDISCLAIMERID', 'Get Exchange account disclaimer id');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_EXCHANGE_EXCHANGEDISCLAIMER', 'Get Exchange disclaimer');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_EXCHANGE_MAILBOXPLAN', 'Get Exchange Mailbox plan');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_EXCHANGE_MAILBOXPLANS', 'Get Exchange Mailbox plans');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_EXCHANGE_RETENTIONPOLICYTAG', 'Get Exchange retention policy tag');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_EXCHANGE_RETENTIONPOLICYTAGS', 'Get Exchange retention policy tags');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_FOLDERS_STATS', 'Get organization public folder statistics');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_MAILBOX_ADDRESSES', 'Get mailbox e-mail addresses');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_MAILBOX_ADVANCED', 'Get mailbox advanced settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_MAILBOX_AUTOREPLY', 'Get Mailbox autoreply');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_MAILBOX_GENERAL', 'Get mailbox general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_MAILBOX_MAILFLOW', 'Get mailbox mail flow settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_MAILBOX_PERMISSIONS', 'Get Mailbox permissions');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_MAILBOX_STATS', 'Get Mailbox statistics');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_MAILBOXES_STATS', 'Get organization mailboxes statistics');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_MOBILE_DEVICES', 'Get mobile devices');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_ORG_LIMITS', 'Get organization storage limits');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_ORG_STATS', 'Get organization statistics');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_PICTURE', 'Get picture');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_PUBLIC_FOLDER_ADDRESSES', 'Get public folder e-mail addresses');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_PUBLIC_FOLDER_GENERAL', 'Get public folder general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_PUBLIC_FOLDER_MAILFLOW', 'Get public folder mail flow settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'GET_RESOURCE_MAILBOX', 'Get resource Mailbox settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'SET_EXCHANGE_ACCOUNTDISCLAIMERID', 'Set exchange account disclaimer id');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'SET_EXCHANGE_MAILBOXPLAN', 'Set exchange Mailbox plan');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'SET_MAILBOXPLAN_RETENTIONPOLICY_ARCHIVING', 'Set Mailbox plan retention policy archiving');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'SET_ORG_LIMITS', 'Update organization storage limits');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'SET_PRIMARY_DISTR_LIST_ADDRESS', 'Set distribution list primary e-mail address');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'SET_PRIMARY_MAILBOX_ADDRESS', 'Set mailbox primary e-mail address');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'SET_PRIMARY_PUBLIC_FOLDER_ADDRESS', 'Set public folder primary e-mail address');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_CONTACT_GENERAL', 'Update contact general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_CONTACT_MAILFLOW', 'Update contact mail flow settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_DISTR_LIST_GENERAL', 'Update distribution list general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_DISTR_LIST_MAILFLOW', 'Update distribution list mail flow settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_EXCHANGE_RETENTIONPOLICYTAG', 'Update Exchange retention policy tag');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_MAILBOX_ADVANCED', 'Update mailbox advanced settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_MAILBOX_AUTOREPLY', 'Update Mailbox autoreply');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_MAILBOX_GENERAL', 'Update mailbox general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_MAILBOX_MAILFLOW', 'Update mailbox mail flow settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_PUBLIC_FOLDER_GENERAL', 'Update public folder general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_PUBLIC_FOLDER_MAILFLOW', 'Update public folder mail flow settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('EXCHANGE', 'UPDATE_RESOURCE_MAILBOX', 'Update resource Mailbox settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'COPY_FILES', 'Copy files');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'CREATE_ACCESS_DATABASE', 'Create MS Access database');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'CREATE_FILE', 'Create file');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'CREATE_FOLDER', 'Create folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'DELETE_FILES', 'Delete files');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'MOVE_FILES', 'Move files');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'RENAME_FILE', 'Rename file');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'SET_PERMISSIONS', NULL);
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'UNZIP_FILES', 'Unzip files');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'UPDATE_BINARY_CONTENT', 'Update file binary content');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FILES', 'ZIP_FILES', 'Zip files');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FTP_ACCOUNT', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FTP_ACCOUNT', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('FTP_ACCOUNT', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('GLOBAL_DNS', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('GLOBAL_DNS', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('GLOBAL_DNS', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('HOSTING_SPACE', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('HOSTING_SPACE_WR', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('IMPORT', 'IMPORT', 'Import');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('IP_ADDRESS', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('IP_ADDRESS', 'ADD_RANGE', 'Add range');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('IP_ADDRESS', 'ALLOCATE_PACKAGE_IP', 'Allocate package IP addresses');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('IP_ADDRESS', 'DEALLOCATE_PACKAGE_IP', 'Deallocate package IP addresses');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('IP_ADDRESS', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('IP_ADDRESS', 'DELETE_RANGE', 'Delete IP Addresses');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('IP_ADDRESS', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('IP_ADDRESS', 'UPDATE_RANGE', 'Update IP Addresses');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_ACCOUNT', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_ACCOUNT', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_ACCOUNT', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_DOMAIN', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_DOMAIN', 'ADD_POINTER', 'Add pointer');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_DOMAIN', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_DOMAIN', 'DELETE_POINTER', 'Update pointer');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_DOMAIN', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_FORWARDING', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_FORWARDING', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_FORWARDING', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_GROUP', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_GROUP', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_GROUP', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_LIST', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_LIST', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('MAIL_LIST', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('OCS', 'CREATE_OCS_USER', 'Create OCS user');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('OCS', 'GET_OCS_USERS', 'Get OCS users');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('OCS', 'GET_OCS_USERS_COUNT', 'Get OCS users count');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ODBC_DSN', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ODBC_DSN', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ODBC_DSN', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'CREATE_ORG', 'Create organization');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'CREATE_ORGANIZATION_ENTERPRISE_STORAGE', 'Create organization enterprise storage');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'CREATE_SECURITY_GROUP', 'Create security group');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'CREATE_USER', 'Create user');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'DELETE_ORG', 'Delete organization');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'DELETE_SECURITY_GROUP', 'Delete security group');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'GET_ORG_STATS', 'Get organization statistics');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'GET_SECURITY_GROUP_GENERAL', 'Get security group general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'GET_SECURITY_GROUPS_BYMEMBER', 'Get security groups by member');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'GET_SUPPORT_SERVICE_LEVELS', 'Get support service levels');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'REMOVE_USER', 'Remove user');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'SEND_USER_PASSWORD_RESET_EMAIL_PINCODE', 'Send user password reset email pincode');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'SET_USER_PASSWORD', 'Set user password');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'SET_USER_USERPRINCIPALNAME', 'Set user principal name');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'UPDATE_PASSWORD_SETTINGS', 'Update password settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'UPDATE_SECURITY_GROUP_GENERAL', 'Update security group general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('ORGANIZATION', 'UPDATE_USER_GENERAL', 'Update user general settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('REMOTE_DESKTOP_SERVICES', 'ADD_RDS_SERVER', 'Add RDS server');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('REMOTE_DESKTOP_SERVICES', 'RESTART_RDS_SERVER', 'Restart RDS server');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('REMOTE_DESKTOP_SERVICES', 'SET_RDS_SERVER_NEW_CONNECTIONS_ALLOWED', 'Set RDS new connection allowed');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SCHEDULER', 'RUN_SCHEDULE', NULL);
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'ADD_SERVICE', 'Add service');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'CHANGE_WINDOWS_SERVICE_STATUS', 'Change Windows service status');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'CHECK_AVAILABILITY', 'Check availability');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'CLEAR_EVENT_LOG', 'Clear Windows event log');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'DELETE_SERVICE', 'Delete service');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'REBOOT', 'Reboot');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'RESET_TERMINAL_SESSION', 'Reset terminal session');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'TERMINATE_SYSTEM_PROCESS', 'Terminate system process');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'UPDATE_AD_PASSWORD', 'Update active directory password');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'UPDATE_PASSWORD', 'Update access password');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SERVER', 'UPDATE_SERVICE', 'Update service');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'ADD_GROUP', 'Add group');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'ADD_SITE', 'Add site');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'ADD_USER', 'Add user');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'BACKUP_SITE', 'Backup site');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'DELETE_GROUP', 'Delete group');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'DELETE_SITE', 'Delete site');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'DELETE_USER', 'Delete user');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'INSTALL_WEBPARTS', 'Install Web Parts package');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'RESTORE_SITE', 'Restore site');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'UNINSTALL_WEBPARTS', 'Uninstall Web Parts package');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'UPDATE_GROUP', 'Update group');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SHAREPOINT', 'UPDATE_USER', 'Update user');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SPACE', 'CALCULATE_DISKSPACE', 'Calculate disk space');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SPACE', 'CHANGE_ITEMS_STATUS', 'Change hosting items status');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SPACE', 'CHANGE_STATUS', 'Change hostng space status');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SPACE', 'DELETE', 'Delete hosting space');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SPACE', 'DELETE_ITEMS', 'Delete hosting items');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SQL_DATABASE', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SQL_DATABASE', 'BACKUP', 'Backup');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SQL_DATABASE', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SQL_DATABASE', 'RESTORE', 'Restore');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SQL_DATABASE', 'TRUNCATE', 'Truncate');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SQL_DATABASE', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SQL_USER', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SQL_USER', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('SQL_USER', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('STATS_SITE', 'ADD', 'Add statistics site');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('STATS_SITE', 'DELETE', 'Delete statistics site');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('STATS_SITE', 'UPDATE', 'Update statistics site');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('STORAGE_SPACES', 'REMOVE_STORAGE_SPACE', 'Remove storage space');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('STORAGE_SPACES', 'SAVE_STORAGE_SPACE', 'Save storage space');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('STORAGE_SPACES', 'SAVE_STORAGE_SPACE_LEVEL', 'Save storage space level');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('USER', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('USER', 'AUTHENTICATE', 'Authenticate');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('USER', 'CHANGE_PASSWORD', 'Change password');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('USER', 'CHANGE_PASSWORD_BY_USERNAME_PASSWORD', 'Change password by username/password');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('USER', 'CHANGE_STATUS', 'Change status');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('USER', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('USER', 'GET_BY_USERNAME_PASSWORD', 'Get by username/password');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('USER', 'SEND_REMINDER', 'Send password reminder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('USER', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('USER', 'UPDATE_SETTINGS', 'Update settings');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VIRTUAL_SERVER', 'ADD_SERVICES', 'Add services');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VIRTUAL_SERVER', 'DELETE_SERVICES', 'Delete services');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VLAN', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VLAN', 'ADD_RANGE', 'Add range');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VLAN', 'ALLOCATE_PACKAGE_VLAN', 'Allocate package VLAN');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VLAN', 'DEALLOCATE_PACKAGE_VLAN', 'Deallocate package VLAN');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VLAN', 'DELETE_RANGE', 'Delete range');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VLAN', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'ADD_EXTERNAL_IP', 'Add external IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'ADD_PRIVATE_IP', 'Add private IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'APPLY_SNAPSHOT', 'Apply VPS snapshot');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'CANCEL_JOB', 'Cancel Job');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'CHANGE_ADMIN_PASSWORD', 'Change administrator password');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'CHANGE_STATE', 'Change VPS state');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'CREATE', 'Create VPS');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'DELETE', 'Delete VPS');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'DELETE_EXTERNAL_IP', 'Delete external IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'DELETE_PRIVATE_IP', 'Delete private IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'DELETE_SNAPSHOT', 'Delete VPS snapshot');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'DELETE_SNAPSHOT_SUBTREE', 'Delete VPS snapshot subtree');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'EJECT_DVD_DISK', 'Eject DVD disk');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'INSERT_DVD_DISK', 'Insert DVD disk');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'REINSTALL', 'Re-install VPS');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'RENAME_SNAPSHOT', 'Rename VPS snapshot');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'SEND_SUMMARY_LETTER', 'Send VPS summary letter');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'SET_PRIMARY_EXTERNAL_IP', 'Set primary external IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'SET_PRIMARY_PRIVATE_IP', 'Set primary private IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'TAKE_SNAPSHOT', 'Take VPS snapshot');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'UPDATE_CONFIGURATION', 'Update VPS configuration');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'UPDATE_HOSTNAME', 'Update host name');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'UPDATE_IP', 'Update IP Address');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'UPDATE_PERMISSIONS', 'Update VPS permissions');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS', 'UPDATE_VDC_PERMISSIONS', 'Update space permissions');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'ADD_EXTERNAL_IP', 'Add external IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'ADD_PRIVATE_IP', 'Add private IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'APPLY_SNAPSHOT', 'Apply VM snapshot');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'CHANGE_ADMIN_PASSWORD', 'Change administrator password');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'CHANGE_STATE', 'Change VM state');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'CREATE', 'Create VM');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'DELETE', 'Delete VM');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'DELETE_EXTERNAL_IP', 'Delete external IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'DELETE_PRIVATE_IP', 'Delete private IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'DELETE_SNAPSHOT', 'Delete VM snapshot');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'DELETE_SNAPSHOT_SUBTREE', 'Delete VM snapshot subtree');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'EJECT_DVD_DISK', 'Eject DVD disk');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'INSERT_DVD_DISK', 'Insert DVD disk');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'REINSTALL', 'Reinstall VM');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'RENAME_SNAPSHOT', 'Rename VM snapshot');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'SET_PRIMARY_EXTERNAL_IP', 'Set primary external IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'SET_PRIMARY_PRIVATE_IP', 'Set primary private IP');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'TAKE_SNAPSHOT', 'Take VM snapshot');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'UPDATE_CONFIGURATION', 'Update VM configuration');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('VPS2012', 'UPDATE_HOSTNAME', 'Update host name');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WAG_INSTALLER', 'GET_APP_PARAMS_TASK', 'Get application parameters');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WAG_INSTALLER', 'GET_GALLERY_APP_DETAILS_TASK', 'Get gallery application details');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WAG_INSTALLER', 'GET_GALLERY_APPS_TASK', 'Get gallery applications');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WAG_INSTALLER', 'GET_GALLERY_CATEGORIES_TASK', 'Get gallery categories');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WAG_INSTALLER', 'GET_SRV_GALLERY_APPS_TASK', 'Get server gallery applications');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WAG_INSTALLER', 'INSTALL_WEB_APP', 'Install Web application');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'ADD', 'Add');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'ADD_POINTER', 'Add domain pointer');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'ADD_SSL_FOLDER', 'Add shared SSL folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'ADD_VDIR', 'Add virtual directory');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'CHANGE_FP_PASSWORD', 'Change FrontPage account password');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'CHANGE_STATE', 'Change state');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'DELETE', 'Delete');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'DELETE_POINTER', 'Delete domain pointer');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'DELETE_SECURED_FOLDER', 'Delete secured folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'DELETE_SECURED_GROUP', 'Delete secured group');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'DELETE_SECURED_USER', 'Delete secured user');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'DELETE_SSL_FOLDER', 'Delete shared SSL folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'DELETE_VDIR', 'Delete virtual directory');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'GET_STATE', 'Get state');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'INSTALL_FP', 'Install FrontPage Extensions');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'INSTALL_SECURED_FOLDERS', 'Install secured folders');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'UNINSTALL_FP', 'Uninstall FrontPage Extensions');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'UNINSTALL_SECURED_FOLDERS', 'Uninstall secured folders');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'UPDATE', 'Update');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'UPDATE_SECURED_FOLDER', 'Add/update secured folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'UPDATE_SECURED_GROUP', 'Add/update secured group');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'UPDATE_SECURED_USER', 'Add/update secured user');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'UPDATE_SSL_FOLDER', 'Update shared SSL folder');
SELECT changes();

INSERT INTO "AuditLogTasks" ("SourceName", "TaskName", "TaskDescription")
VALUES ('WEB_SITE', 'UPDATE_VDIR', 'Update virtual directory');
SELECT changes();


INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (1, 'FuseCP.EnterpriseServer.OperatingSystemController', 'OS', 1, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (2, 'FuseCP.EnterpriseServer.WebServerController', 'Web', 2, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (3, 'FuseCP.EnterpriseServer.FtpServerController', 'FTP', 3, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (4, 'FuseCP.EnterpriseServer.MailServerController', 'Mail', 4, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (5, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2000', 7, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (6, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MySQL4', 11, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (7, 'FuseCP.EnterpriseServer.DnsServerController', 'DNS', 17, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (8, 'FuseCP.EnterpriseServer.StatisticsServerController', 'Statistics', 18, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (10, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2005', 8, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (11, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MySQL5', 12, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (12, NULL, 'Exchange', 5, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (13, NULL, 'Hosted Organizations', 6, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (20, 'FuseCP.EnterpriseServer.HostedSharePointServerController', 'Sharepoint Foundation Server', 14, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (22, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2008', 9, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (23, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2012', 10, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (30, NULL, 'VPS', 19, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (31, NULL, 'BlackBerry', 21, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (32, NULL, 'OCS', 22, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (33, NULL, 'VPS2012', 20, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (40, NULL, 'VPSForPC', 20, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (41, NULL, 'Lync', 24, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (44, 'FuseCP.EnterpriseServer.EnterpriseStorageController', 'EnterpriseStorage', 26, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (45, NULL, 'RDS', 27, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (46, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2014', 10, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (47, NULL, 'Service Levels', 2, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (49, 'FuseCP.EnterpriseServer.StorageSpacesController', 'StorageSpaceServices', 26, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (50, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MariaDB', 11, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (52, NULL, 'SfB', 26, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (61, NULL, 'MailFilters', 5, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (71, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2016', 10, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (72, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2017', 10, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (73, 'FuseCP.EnterpriseServer.HostedSharePointServerEntController', 'Sharepoint Enterprise Server', 15, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (74, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2019', 10, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (75, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2022', 10, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (76, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2025', 10, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (90, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MySQL8', 12, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (91, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MySQL9', 12, 1);
SELECT changes();

INSERT INTO "ResourceGroups" ("GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup")
VALUES (167, NULL, 'Proxmox', 20, 1);
SELECT changes();


INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_ACTIVATE_PAID_INVOICES', 0, 'FuseCP.Ecommerce.EnterpriseServer.ActivatePaidInvoicesTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_AUDIT_LOG_REPORT', 3, 'FuseCP.EnterpriseServer.AuditLogReportTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_BACKUP', 1, 'FuseCP.EnterpriseServer.BackupTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_BACKUP_DATABASE', 3, 'FuseCP.EnterpriseServer.BackupDatabaseTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_CALCULATE_EXCHANGE_DISKSPACE', 2, 'FuseCP.EnterpriseServer.CalculateExchangeDiskspaceTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH', 1, 'FuseCP.EnterpriseServer.CalculatePackagesBandwidthTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE', 1, 'FuseCP.EnterpriseServer.CalculatePackagesDiskspaceTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_CANCEL_OVERDUE_INVOICES', 0, 'FuseCP.Ecommerce.EnterpriseServer.CancelOverdueInvoicesTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_CHECK_WEBSITE', 3, 'FuseCP.EnterpriseServer.CheckWebSiteTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_CHECK_WEBSITES_SSL', 3, 'FuseCP.EnterpriseServer.CheckWebsitesSslTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_DELETE_EXCHANGE_ACCOUNTS', 3, 'FuseCP.EnterpriseServer.DeleteExchangeAccountsTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_DOMAIN_EXPIRATION', 3, 'FuseCP.EnterpriseServer.DomainExpirationTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_DOMAIN_LOOKUP', 1, 'FuseCP.EnterpriseServer.DomainLookupViewTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_FTP_FILES', 3, 'FuseCP.EnterpriseServer.FTPFilesTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_GENERATE_INVOICES', 0, 'FuseCP.Ecommerce.EnterpriseServer.GenerateInvoicesTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 2, 'FuseCP.EnterpriseServer.HostedSolutionReportTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 2, 'FuseCP.EnterpriseServer.NotifyOverusedDatabasesTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_RUN_PAYMENT_QUEUE', 0, 'FuseCP.Ecommerce.EnterpriseServer.RunPaymentQueueTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_RUN_SYSTEM_COMMAND', 1, 'FuseCP.EnterpriseServer.RunSystemCommandTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_SEND_MAIL', 3, 'FuseCP.EnterpriseServer.SendMailNotificationTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_SUSPEND_OVERDUE_INVOICES', 0, 'FuseCP.Ecommerce.EnterpriseServer.SuspendOverdueInvoicesTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_SUSPEND_PACKAGES', 2, 'FuseCP.EnterpriseServer.SuspendOverusedPackagesTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_USER_PASSWORD_EXPIRATION_NOTIFICATION', 1, 'FuseCP.EnterpriseServer.UserPasswordExpirationNotificationTask, FuseCP.EnterpriseServer.Code');
SELECT changes();

INSERT INTO "ScheduleTasks" ("TaskID", "RoleID", "TaskType")
VALUES ('SCHEDULE_TASK_ZIP_FILES', 3, 'FuseCP.EnterpriseServer.ZipFilesTask, FuseCP.EnterpriseServer.Code');
SELECT changes();


INSERT INTO "SystemSettings" ("PropertyName", "SettingsName", "PropertyValue")
VALUES ('AccessIps', 'AccessIpsSettings', '');
SELECT changes();

INSERT INTO "SystemSettings" ("PropertyName", "SettingsName", "PropertyValue")
VALUES ('CanPeerChangeMfa', 'AuthenticationSettings', 'True');
SELECT changes();

INSERT INTO "SystemSettings" ("PropertyName", "SettingsName", "PropertyValue")
VALUES ('MfaTokenAppDisplayName', 'AuthenticationSettings', 'FuseCP');
SELECT changes();

INSERT INTO "SystemSettings" ("PropertyName", "SettingsName", "PropertyValue")
VALUES ('BackupsPath', 'BackupSettings', 'c:\HostingBackups');
SELECT changes();

INSERT INTO "SystemSettings" ("PropertyName", "SettingsName", "PropertyValue")
VALUES ('SmtpEnableSsl', 'SmtpSettings', 'False');
SELECT changes();

INSERT INTO "SystemSettings" ("PropertyName", "SettingsName", "PropertyValue")
VALUES ('SmtpPort', 'SmtpSettings', '25');
SELECT changes();

INSERT INTO "SystemSettings" ("PropertyName", "SettingsName", "PropertyValue")
VALUES ('SmtpServer', 'SmtpSettings', '127.0.0.1');
SELECT changes();

INSERT INTO "SystemSettings" ("PropertyName", "SettingsName", "PropertyValue")
VALUES ('SmtpUsername', 'SmtpSettings', 'postmaster');
SELECT changes();


INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (1, 'Light', 'light-theme', 'Style', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (2, 'Dark', 'dark-theme', 'Style', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (3, 'Semi Dark', 'semi-dark', 'Style', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (4, 'Minimal', 'minimal-theme', 'Style', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (5, '#0727d7', 'headercolor1', 'color-header', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (6, '#23282c', 'headercolor2', 'color-header', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (7, '#e10a1f', 'headercolor3', 'color-header', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (8, '#157d4c', 'headercolor4', 'color-header', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (9, '#673ab7', 'headercolor5', 'color-header', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (10, '#795548', 'headercolor6', 'color-header', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (11, '#d3094e', 'headercolor7', 'color-header', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (12, '#ff9800', 'headercolor8', 'color-header', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (13, '#6c85ec', 'sidebarcolor1', 'color-Sidebar', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (14, '#5b737f', 'sidebarcolor2', 'color-Sidebar', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (15, '#408851', 'sidebarcolor3', 'color-Sidebar', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (16, '#230924', 'sidebarcolor4', 'color-Sidebar', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (17, '#903a85', 'sidebarcolor5', 'color-Sidebar', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (18, '#a04846', 'sidebarcolor6', 'color-Sidebar', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (19, '#a65314', 'sidebarcolor7', 'color-Sidebar', 1);
SELECT changes();

INSERT INTO "ThemeSettings" ("ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID")
VALUES (20, '#1f0e3b', 'sidebarcolor8', 'color-Sidebar', 1);
SELECT changes();


INSERT INTO "Themes" ("ThemeID", "DisplayName", "DisplayOrder", "Enabled", "LTRName", "RTLName")
VALUES (1, 'FuseCP v1', 1, 1, 'Default', 'Default');
SELECT changes();


INSERT INTO "Users" ("UserID", "AdditionalParams", "Address", "Changed", "City", "Comments", "CompanyName", "Country", "Created", "EcommerceEnabled", "Email", "FailedLogins", "Fax", "FirstName", "HtmlMail", "InstantMessenger", "LastName", "LoginStatusId", "OneTimePasswordState", "OwnerID", "Password", "PinSecret", "PrimaryPhone", "RoleID", "SecondaryEmail", "SecondaryPhone", "State", "StatusID", "SubscriberNumber", "Username", "Zip")
VALUES (1, NULL, '', '2010-07-16 10:53:02.453', '', '', NULL, '', '2010-07-16 12:53:02.453', 1, 'serveradmin@myhosting.com', NULL, '', 'Enterprise', 1, '', 'Administrator', NULL, NULL, NULL, '', NULL, '', 1, '', '', '', 1, NULL, 'serveradmin', '');
SELECT changes();


INSERT INTO "Versions" ("DatabaseVersion", "BuildDate")
VALUES ('1.0', '2010-04-10 00:00:00');
SELECT changes();

INSERT INTO "Versions" ("DatabaseVersion", "BuildDate")
VALUES ('1.0.1.0', '2010-07-16 12:53:03.563');
SELECT changes();

INSERT INTO "Versions" ("DatabaseVersion", "BuildDate")
VALUES ('1.0.2.0', '2010-09-03 00:00:00');
SELECT changes();

INSERT INTO "Versions" ("DatabaseVersion", "BuildDate")
VALUES ('1.1.0.9', '2010-11-16 00:00:00');
SELECT changes();

INSERT INTO "Versions" ("DatabaseVersion", "BuildDate")
VALUES ('1.1.2.13', '2011-04-15 00:00:00');
SELECT changes();

INSERT INTO "Versions" ("DatabaseVersion", "BuildDate")
VALUES ('1.2.0.38', '2011-07-13 00:00:00');
SELECT changes();

INSERT INTO "Versions" ("DatabaseVersion", "BuildDate")
VALUES ('1.2.1.6', '2012-03-29 00:00:00');
SELECT changes();

INSERT INTO "Versions" ("DatabaseVersion", "BuildDate")
VALUES ('1.4.9', '2024-04-20 00:00:00');
SELECT changes();

INSERT INTO "Versions" ("DatabaseVersion", "BuildDate")
VALUES ('1.5.1', '2024-12-17 00:00:00');
SELECT changes();

INSERT INTO "Versions" ("DatabaseVersion", "BuildDate")
VALUES ('2.0.0', '2025-11-04 00:00:00');
SELECT changes();


INSERT INTO "Packages" ("PackageID", "BandwidthUpdated", "PackageComments", "PackageName", "ParentPackageID", "PlanID", "PurchaseDate", "ServerID", "StatusID", "StatusIDchangeDate", "UserID")
VALUES (1, NULL, '', 'System', NULL, NULL, NULL, NULL, 1, '2024-10-12 19:29:19.927', 1);
SELECT changes();


INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1, NULL, 'Windows Server 2003', 'Windows2003', 1, 'Windows2003', 'FuseCP.Providers.OS.Windows2003, FuseCP.Providers.OS.Windows2003');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (2, NULL, 'Internet Information Services 6.0', 'IIS60', 2, 'IIS60', 'FuseCP.Providers.Web.IIs60, FuseCP.Providers.Web.IIs60');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (3, NULL, 'Microsoft FTP Server 6.0', 'MSFTP60', 3, 'MSFTP60', 'FuseCP.Providers.FTP.MsFTP, FuseCP.Providers.FTP.IIs60');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (4, NULL, 'MailEnable Server 1.x - 7.x', 'MailEnable', 4, 'MailEnable', 'FuseCP.Providers.Mail.MailEnable, FuseCP.Providers.Mail.MailEnable');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (5, NULL, 'Microsoft SQL Server 2000', 'MSSQL', 5, 'MSSQL', 'FuseCP.Providers.Database.MsSqlServer, FuseCP.Providers.Database.SqlServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (6, NULL, 'MySQL Server 4.x', 'MySQL', 6, 'MySQL', 'FuseCP.Providers.Database.MySqlServer, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (7, NULL, 'Microsoft DNS Server', 'MSDNS', 7, 'MSDNS', 'FuseCP.Providers.DNS.MsDNS, FuseCP.Providers.DNS.MsDNS');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (8, NULL, 'AWStats Statistics Service', 'AWStats', 8, 'AWStats', 'FuseCP.Providers.Statistics.AWStats, FuseCP.Providers.Statistics.AWStats');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (9, NULL, 'SimpleDNS Plus 4.x', 'SimpleDNS', 7, 'SimpleDNS', 'FuseCP.Providers.DNS.SimpleDNS, FuseCP.Providers.DNS.SimpleDNS');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (10, NULL, 'SmarterStats 3.x', 'SmarterStats', 8, 'SmarterStats', 'FuseCP.Providers.Statistics.SmarterStats, FuseCP.Providers.Statistics.SmarterStats');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (11, NULL, 'SmarterMail 2.x', 'SmarterMail', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail2, FuseCP.Providers.Mail.SmarterMail2');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (12, NULL, 'Gene6 FTP Server 3.x', 'Gene6FTP', 3, 'Gene6FTP', 'FuseCP.Providers.FTP.Gene6, FuseCP.Providers.FTP.Gene6');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (13, NULL, 'Merak Mail Server 8.0.3 - 9.2.x', 'Merak', 4, 'Merak', 'FuseCP.Providers.Mail.Merak, FuseCP.Providers.Mail.Merak');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (14, NULL, 'SmarterMail 3.x - 4.x', 'SmarterMail', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail3, FuseCP.Providers.Mail.SmarterMail3');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (16, NULL, 'Microsoft SQL Server 2005', 'MSSQL', 10, 'MSSQL', 'FuseCP.Providers.Database.MsSqlServer2005, FuseCP.Providers.Database.SqlServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (17, NULL, 'MySQL Server 5.0', 'MySQL', 11, 'MySQL', 'FuseCP.Providers.Database.MySqlServer50, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (18, NULL, 'MDaemon 9.x - 11.x', 'MDaemon', 4, 'MDaemon', 'FuseCP.Providers.Mail.MDaemon, FuseCP.Providers.Mail.MDaemon');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (19, 1, 'ArGoSoft Mail Server 1.x', 'ArgoMail', 4, 'ArgoMail', 'FuseCP.Providers.Mail.ArgoMail, FuseCP.Providers.Mail.ArgoMail');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (20, NULL, 'hMailServer 4.2', 'hMailServer', 4, 'hMailServer', 'FuseCP.Providers.Mail.hMailServer, FuseCP.Providers.Mail.hMailServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (21, NULL, 'Ability Mail Server 2.x', 'AbilityMailServer', 4, 'AbilityMailServer', 'FuseCP.Providers.Mail.AbilityMailServer, FuseCP.Providers.Mail.AbilityMailServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (22, NULL, 'hMailServer 4.3', 'hMailServer43', 4, 'hMailServer43', 'FuseCP.Providers.Mail.hMailServer43, FuseCP.Providers.Mail.hMailServer43');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (24, NULL, 'ISC BIND 8.x - 9.x', 'Bind', 7, 'Bind', 'FuseCP.Providers.DNS.IscBind, FuseCP.Providers.DNS.Bind');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (25, NULL, 'Serv-U FTP 6.x', 'ServU', 3, 'ServU', 'FuseCP.Providers.FTP.ServU, FuseCP.Providers.FTP.ServU');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (26, NULL, 'FileZilla FTP Server 0.9', 'FileZilla', 3, 'FileZilla', 'FuseCP.Providers.FTP.FileZilla, FuseCP.Providers.FTP.FileZilla');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (27, NULL, 'Hosted Microsoft Exchange Server 2007', 'Exchange', 12, 'Exchange2007', 'FuseCP.Providers.HostedSolution.Exchange2007, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (28, NULL, 'SimpleDNS Plus 5.x', 'SimpleDNS', 7, 'SimpleDNS', 'FuseCP.Providers.DNS.SimpleDNS5, FuseCP.Providers.DNS.SimpleDNS50');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (29, NULL, 'SmarterMail 5.x', 'SmarterMail50', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail5, FuseCP.Providers.Mail.SmarterMail5');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (30, NULL, 'MySQL Server 5.1', 'MySQL', 11, 'MySQL', 'FuseCP.Providers.Database.MySqlServer51, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (31, NULL, 'SmarterStats 4.x', 'SmarterStats', 8, 'SmarterStats', 'FuseCP.Providers.Statistics.SmarterStats4, FuseCP.Providers.Statistics.SmarterStats');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (32, NULL, 'Hosted Microsoft Exchange Server 2010', 'Exchange', 12, 'Exchange2010', 'FuseCP.Providers.HostedSolution.Exchange2010, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (56, 1, 'PowerDNS', 'PowerDNS', 7, 'PowerDNS', 'FuseCP.Providers.DNS.PowerDNS, FuseCP.Providers.DNS.PowerDNS');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (60, NULL, 'SmarterMail 6.x', 'SmarterMail60', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail6, FuseCP.Providers.Mail.SmarterMail6');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (61, NULL, 'Merak Mail Server 10.x', 'Merak', 4, 'Merak', 'FuseCP.Providers.Mail.Merak10, FuseCP.Providers.Mail.Merak10');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (62, NULL, 'SmarterStats 5.x +', 'SmarterStats', 8, 'SmarterStats', 'FuseCP.Providers.Statistics.SmarterStats5, FuseCP.Providers.Statistics.SmarterStats');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (63, NULL, 'hMailServer 5.x', 'hMailServer5', 4, 'hMailServer5', 'FuseCP.Providers.Mail.hMailServer5, FuseCP.Providers.Mail.hMailServer5');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (64, NULL, 'SmarterMail 7.x - 8.x', 'SmarterMail60', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail7, FuseCP.Providers.Mail.SmarterMail7');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (65, NULL, 'SmarterMail 9.x', 'SmarterMail60', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail9, FuseCP.Providers.Mail.SmarterMail9');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (66, NULL, 'SmarterMail 10.x +', 'SmarterMail100', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail10, FuseCP.Providers.Mail.SmarterMail10');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (67, NULL, 'SmarterMail 100.x +', 'SmarterMail100x', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail100, FuseCP.Providers.Mail.SmarterMail100');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (90, NULL, 'Hosted Microsoft Exchange Server 2010 SP2', 'Exchange', 12, 'Exchange2010SP2', 'FuseCP.Providers.HostedSolution.Exchange2010SP2, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (91, NULL, 'Hosted Microsoft Exchange Server 2013', 'Exchange', 12, 'Exchange2013', 'FuseCP.Providers.HostedSolution.Exchange2013, FuseCP.Providers.HostedSolution.Exchange2013');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (92, NULL, 'Hosted Microsoft Exchange Server 2016', 'Exchange', 12, 'Exchange2016', 'FuseCP.Providers.HostedSolution.Exchange2016, FuseCP.Providers.HostedSolution.Exchange2016');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (93, NULL, 'Hosted Microsoft Exchange Server 2019', 'Exchange', 12, 'Exchange2016', 'FuseCP.Providers.HostedSolution.Exchange2019, FuseCP.Providers.HostedSolution.Exchange2019');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (100, NULL, 'Windows Server 2008', 'Windows2008', 1, 'Windows2008', 'FuseCP.Providers.OS.Windows2008, FuseCP.Providers.OS.Windows2008');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (101, NULL, 'Internet Information Services 7.0', 'IIS70', 2, 'IIS70', 'FuseCP.Providers.Web.IIs70, FuseCP.Providers.Web.IIs70');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (102, NULL, 'Microsoft FTP Server 7.0', 'MSFTP70', 3, 'MSFTP70', 'FuseCP.Providers.FTP.MsFTP, FuseCP.Providers.FTP.IIs70');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (103, NULL, 'Hosted Organizations', 'Organizations', 13, 'Organizations', 'FuseCP.Providers.HostedSolution.OrganizationProvider, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (104, NULL, 'Windows Server 2012', 'Windows2012', 1, 'Windows2012', 'FuseCP.Providers.OS.Windows2012, FuseCP.Providers.OS.Windows2012');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (105, NULL, 'Internet Information Services 8.0', 'IIS70', 2, 'IIS80', 'FuseCP.Providers.Web.IIs80, FuseCP.Providers.Web.IIs80');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (106, NULL, 'Microsoft FTP Server 8.0', 'MSFTP70', 3, 'MSFTP80', 'FuseCP.Providers.FTP.MsFTP80, FuseCP.Providers.FTP.IIs80');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (110, NULL, 'Cerberus FTP Server 6.x', 'CerberusFTP6', 3, 'CerberusFTP6', 'FuseCP.Providers.FTP.CerberusFTP6, FuseCP.Providers.FTP.CerberusFTP6');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (111, NULL, 'Windows Server 2016', 'Windows2008', 1, 'Windows2016', 'FuseCP.Providers.OS.Windows2016, FuseCP.Providers.OS.Windows2016');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (112, NULL, 'Internet Information Services 10.0', 'IIS70', 2, 'IIS100', 'FuseCP.Providers.Web.IIs100, FuseCP.Providers.Web.IIs100');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (113, NULL, 'Microsoft FTP Server 10.0', 'MSFTP70', 3, 'MSFTP100', 'FuseCP.Providers.FTP.MsFTP100, FuseCP.Providers.FTP.IIs100');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (160, NULL, 'IceWarp Mail Server', 'IceWarp', 4, 'IceWarp', 'FuseCP.Providers.Mail.IceWarp, FuseCP.Providers.Mail.IceWarp');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (200, NULL, 'Hosted Windows SharePoint Services 3.0', 'HostedSharePoint30', 20, 'HostedSharePoint30', 'FuseCP.Providers.HostedSolution.HostedSharePointServer, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (202, NULL, 'Microsoft SQL Server 2008', 'MSSQL', 22, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2008, FuseCP.Providers.Database.SqlServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (203, 1, 'BlackBerry 4.1', 'BlackBerry', 31, 'BlackBerry 4.1', 'FuseCP.Providers.HostedSolution.BlackBerryProvider, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (204, 1, 'BlackBerry 5.0', 'BlackBerry5', 31, 'BlackBerry 5.0', 'FuseCP.Providers.HostedSolution.BlackBerry5Provider, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (205, 1, 'Office Communications Server 2007 R2', 'OCS', 32, 'OCS', 'FuseCP.Providers.HostedSolution.OCS2007R2, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (206, 1, 'OCS Edge server', 'OCS_Edge', 32, 'OCSEdge', 'FuseCP.Providers.HostedSolution.OCSEdge2007R2, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (208, NULL, 'Hosted SharePoint Foundation 2010', 'HostedSharePoint30', 20, 'HostedSharePoint2010', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2010, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (250, NULL, 'Microsoft Lync Server 2010 Multitenant Hosting Pack', 'Lync', 41, 'Lync2010', 'FuseCP.Providers.HostedSolution.Lync2010, FuseCP.Providers.HostedSolution');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (300, 1, 'Microsoft Hyper-V', 'HyperV', 30, 'HyperV', 'FuseCP.Providers.Virtualization.HyperV, FuseCP.Providers.Virtualization.HyperV');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (301, NULL, 'MySQL Server 5.5', 'MySQL', 11, 'MySQL', 'FuseCP.Providers.Database.MySqlServer55, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (302, NULL, 'MySQL Server 5.6', 'MySQL', 11, 'MySQL', 'FuseCP.Providers.Database.MySqlServer56, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (303, NULL, 'MySQL Server 5.7', 'MySQL', 11, 'MySQL', 'FuseCP.Providers.Database.MySqlServer57, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (304, NULL, 'MySQL Server 8.0', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer80, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (305, NULL, 'MySQL Server 8.1', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer81, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (306, NULL, 'MySQL Server 8.2', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer82, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (307, NULL, 'MySQL Server 8.3', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer83, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (308, NULL, 'MySQL Server 8.4', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer84, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (320, NULL, 'MySQL Server 9.0', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer90, FuseCP.Providers.Database.MySQL');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (350, 1, 'Microsoft Hyper-V 2012 R2', 'HyperV2012R2', 33, 'HyperV2012R2', 'FuseCP.Providers.Virtualization.HyperV2012R2, FuseCP.Providers.Virtualization.HyperV2012R2');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (351, 1, 'Microsoft Hyper-V Virtual Machine Management', 'HyperVvmm', 33, 'HyperVvmm', 'FuseCP.Providers.Virtualization.HyperVvmm, FuseCP.Providers.Virtualization.HyperVvmm');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (352, 1, 'Microsoft Hyper-V 2016', 'HyperV2012R2', 33, 'HyperV2016', 'FuseCP.Providers.Virtualization.HyperV2016, FuseCP.Providers.Virtualization.HyperV2016');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (370, 1, 'Proxmox Virtualization (remote)', 'Proxmox', 167, 'Proxmox (remote)', 'FuseCP.Providers.Virtualization.Proxmoxvps, FuseCP.Providers.Virtualization.Proxmoxvps');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (371, 0, 'Proxmox Virtualization', 'Proxmox', 167, 'Proxmox', 'FuseCP.Providers.Virtualization.ProxmoxvpsLocal, FuseCP.Providers.Virtualization.Proxmoxvps');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (400, 1, 'Microsoft Hyper-V For Private Cloud', 'HyperVForPrivateCloud', 40, 'HyperVForPC', 'FuseCP.Providers.VirtualizationForPC.HyperVForPC, FuseCP.Providers.VirtualizationForPC.HyperVForPC');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (410, NULL, 'Microsoft DNS Server 2012+', 'MSDNS', 7, 'MSDNS.2012', 'FuseCP.Providers.DNS.MsDNS2012, FuseCP.Providers.DNS.MsDNS2012');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (500, NULL, 'Unix System', 'Unix', 1, 'UnixSystem', 'FuseCP.Providers.OS.Unix, FuseCP.Providers.OS.Unix');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (600, NULL, 'Enterprise Storage Windows 2012', 'EnterpriseStorage', 44, 'EnterpriseStorage2012', 'FuseCP.Providers.EnterpriseStorage.Windows2012, FuseCP.Providers.EnterpriseStorage.Windows2012');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (700, NULL, 'Storage Spaces Windows 2012', 'StorageSpaceServices', 49, 'StorageSpace2012', 'FuseCP.Providers.StorageSpaces.Windows2012, FuseCP.Providers.StorageSpaces.Windows2012');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1203, NULL, 'Microsoft SQL Server 2014', 'MSSQL', 46, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2014, FuseCP.Providers.Database.SqlServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1301, NULL, 'Hosted SharePoint Foundation 2013', 'HostedSharePoint30', 20, 'HostedSharePoint2013', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2013, FuseCP.Providers.HostedSolution.SharePoint2013');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1306, NULL, 'Hosted SharePoint Foundation 2016', 'HostedSharePoint30', 20, 'HostedSharePoint2016', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2016, FuseCP.Providers.HostedSolution.SharePoint2016');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1401, NULL, 'Microsoft Lync Server 2013 Enterprise Edition', 'Lync', 41, 'Lync2013', 'FuseCP.Providers.HostedSolution.Lync2013, FuseCP.Providers.HostedSolution.Lync2013');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1402, NULL, 'Microsoft Lync Server 2013 Multitenant Hosting Pack', 'Lync', 41, 'Lync2013HP', 'FuseCP.Providers.HostedSolution.Lync2013HP, FuseCP.Providers.HostedSolution.Lync2013HP');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1403, NULL, 'Microsoft Skype for Business Server 2015', 'SfB', 52, 'SfB2015', 'FuseCP.Providers.HostedSolution.SfB2015, FuseCP.Providers.HostedSolution.SfB2015');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1404, NULL, 'Microsoft Skype for Business Server 2019', 'SfB', 52, 'SfB2019', 'FuseCP.Providers.HostedSolution.SfB2019, FuseCP.Providers.HostedSolution.SfB2019');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1501, 1, 'Remote Desktop Services Windows 2012', 'RDS', 45, 'RemoteDesktopServices2012', 'FuseCP.Providers.RemoteDesktopServices.Windows2012,FuseCP.Providers.RemoteDesktopServices.Windows2012');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1502, 1, 'Remote Desktop Services Windows 2016', 'RDS', 45, 'RemoteDesktopServices2012', 'FuseCP.Providers.RemoteDesktopServices.Windows2016,FuseCP.Providers.RemoteDesktopServices.Windows2016');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1503, 1, 'Remote Desktop Services Windows 2019', 'RDS', 45, 'RemoteDesktopServices2019', 'FuseCP.Providers.RemoteDesktopServices.Windows2019,FuseCP.Providers.RemoteDesktopServices.Windows2019');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1504, 1, 'Remote Desktop Services Windows 2022', 'RDS', 45, 'RemoteDesktopServices2022', 'FuseCP.Providers.RemoteDesktopServices.Windows2022,FuseCP.Providers.RemoteDesktopServices.Windows2022');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1505, 1, 'Remote Desktop Services Windows 2025', 'RDS', 45, 'RemoteDesktopServices2025', 'FuseCP.Providers.RemoteDesktopServices.Windows2025,FuseCP.Providers.RemoteDesktopServices.Windows2025');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1550, NULL, 'MariaDB 10.1', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB101, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1552, NULL, 'Hosted SharePoint Enterprise 2013', 'HostedSharePoint30', 73, 'HostedSharePoint2013Ent', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2013Ent, FuseCP.Providers.HostedSolution.SharePoint2013Ent');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1560, NULL, 'MariaDB 10.2', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB102, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1570, NULL, 'MariaDB 10.3', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB103, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1571, NULL, 'MariaDB 10.4', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB104, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1572, NULL, 'MariaDB 10.5', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB105, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1573, NULL, 'MariaDB 10.6', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB106, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1574, NULL, 'MariaDB 10.7', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB107, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1575, NULL, 'MariaDB 10.8', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB108, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1576, NULL, 'MariaDB 10.9', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB109, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1577, NULL, 'MariaDB 10.10', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB1010, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1578, NULL, 'MariaDB 10.11', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB1011, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1579, NULL, 'MariaDB 11.0', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB110, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1580, NULL, 'MariaDB 11.1', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB111, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1581, NULL, 'MariaDB 11.2', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB112, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1582, NULL, 'MariaDB 11.3', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB113, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1583, NULL, 'MariaDB 11.4', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB114, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1584, NULL, 'MariaDB 11.5', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB115, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1585, NULL, 'MariaDB 11.6', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB116, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1586, NULL, 'MariaDB 11.7', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB117, FuseCP.Providers.Database.MariaDB');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1601, 1, 'Mail Cleaner', 'MailCleaner', 61, 'MailCleaner', 'FuseCP.Providers.Filters.MailCleaner, FuseCP.Providers.Filters.MailCleaner');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1602, 1, 'SpamExperts Mail Filter', 'SpamExperts', 61, 'SpamExperts', 'FuseCP.Providers.Filters.SpamExperts, FuseCP.Providers.Filters.SpamExperts');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1701, NULL, 'Microsoft SQL Server 2016', 'MSSQL', 71, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2016, FuseCP.Providers.Database.SqlServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1702, NULL, 'Hosted SharePoint Enterprise 2016', 'HostedSharePoint30', 73, 'HostedSharePoint2016Ent', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2016Ent, FuseCP.Providers.HostedSolution.SharePoint2016Ent');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1703, NULL, 'SimpleDNS Plus 6.x', 'SimpleDNS', 7, 'SimpleDNS', 'FuseCP.Providers.DNS.SimpleDNS6, FuseCP.Providers.DNS.SimpleDNS60');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1704, NULL, 'Microsoft SQL Server 2017', 'MSSQL', 72, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2017, FuseCP.Providers.Database.SqlServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1705, NULL, 'Microsoft SQL Server 2019', 'MSSQL', 74, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2019, FuseCP.Providers.Database.SqlServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1706, NULL, 'Microsoft SQL Server 2022', 'MSSQL', 75, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2022, FuseCP.Providers.Database.SqlServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1707, NULL, 'Microsoft SQL Server 2025', 'MSSQL', 76, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2025, FuseCP.Providers.Database.SqlServer');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1711, NULL, 'Hosted SharePoint 2019', 'HostedSharePoint30', 73, 'HostedSharePoint2019', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2019, FuseCP.Providers.HostedSolution.SharePoint2019');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1800, NULL, 'Windows Server 2019', 'Windows2012', 1, 'Windows2019', 'FuseCP.Providers.OS.Windows2019, FuseCP.Providers.OS.Windows2019');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1801, 1, 'Microsoft Hyper-V 2019', 'HyperV2012R2', 33, 'HyperV2019', 'FuseCP.Providers.Virtualization.HyperV2019, FuseCP.Providers.Virtualization.HyperV2019');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1802, NULL, 'Windows Server 2022', 'Windows2012', 1, 'Windows2022', 'FuseCP.Providers.OS.Windows2022, FuseCP.Providers.OS.Windows2022');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1803, 1, 'Microsoft Hyper-V 2022', 'HyperV2012R2', 33, 'HyperV2022', 'FuseCP.Providers.Virtualization.HyperV2022, FuseCP.Providers.Virtualization.HyperV2022');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1804, NULL, 'Windows Server 2025', 'Windows2012', 1, 'Windows2025', 'FuseCP.Providers.OS.Windows2025, FuseCP.Providers.OS.Windows2025');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1805, 1, 'Microsoft Hyper-V 2025', 'HyperV2012R2', 33, 'HyperV2025', 'FuseCP.Providers.Virtualization.HyperV2025, FuseCP.Providers.Virtualization.HyperV2025');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1901, NULL, 'SimpleDNS Plus 8.x', 'SimpleDNS', 7, 'SimpleDNS', 'FuseCP.Providers.DNS.SimpleDNS8, FuseCP.Providers.DNS.SimpleDNS80');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1902, NULL, 'Microsoft DNS Server 2016', 'MSDNS', 7, 'MSDNS.2016', 'FuseCP.Providers.DNS.MsDNS2016, FuseCP.Providers.DNS.MsDNS2016');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1903, NULL, 'SimpleDNS Plus 9.x', 'SimpleDNS', 7, 'SimpleDNS', 'FuseCP.Providers.DNS.SimpleDNS9, FuseCP.Providers.DNS.SimpleDNS90');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1910, NULL, 'vsftpd FTP Server 3', 'vsftpd', 3, 'vsftpd', 'FuseCP.Providers.FTP.VsFtp3, FuseCP.Providers.FTP.VsFtp');
SELECT changes();

INSERT INTO "Providers" ("ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType")
VALUES (1911, NULL, 'Apache Web Server 2.4', 'Apache', 2, 'Apache', 'FuseCP.Providers.Web.Apache24, FuseCP.Providers.Web.Apache');
SELECT changes();


INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (25, 2, NULL, NULL, NULL, 'ASP.NET 1.1', 'Web.AspNet11', 3.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (26, 2, NULL, NULL, NULL, 'ASP.NET 2.0', 'Web.AspNet20', 4.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (27, 2, NULL, NULL, NULL, 'ASP', 'Web.Asp', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (28, 2, NULL, NULL, NULL, 'PHP 4.x', 'Web.Php4', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (29, 2, NULL, NULL, NULL, 'PHP 5.x', 'Web.Php5', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (30, 2, NULL, NULL, NULL, 'Perl', 'Web.Perl', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (31, 2, NULL, NULL, NULL, 'Python', 'Web.Python', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (32, 2, NULL, NULL, NULL, 'Virtual Directories', 'Web.VirtualDirs', 9.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (33, 2, NULL, NULL, NULL, 'FrontPage', 'Web.FrontPage', 10.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (34, 2, NULL, NULL, NULL, 'Custom Security Settings', 'Web.Security', 11.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (35, 2, NULL, NULL, NULL, 'Custom Default Documents', 'Web.DefaultDocs', 12.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (36, 2, NULL, NULL, NULL, 'Dedicated Application Pools', 'Web.AppPools', 13.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (37, 2, NULL, NULL, NULL, 'Custom Headers', 'Web.Headers', 14.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (38, 2, NULL, NULL, NULL, 'Custom Errors', 'Web.Errors', 15.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (39, 2, NULL, NULL, NULL, 'Custom MIME Types', 'Web.Mime', 16.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (40, 4, NULL, NULL, NULL, 'Max Mailbox Size', 'Mail.MaxBoxSize', 2.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (41, 5, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2000.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (42, 5, NULL, NULL, NULL, 'Database Backups', 'MsSQL2000.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (43, 5, NULL, NULL, NULL, 'Database Restores', 'MsSQL2000.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (44, 5, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2000.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (45, 6, NULL, NULL, NULL, 'Database Backups', 'MySQL4.Backup', 4.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (48, 7, NULL, NULL, NULL, 'DNS Editor', 'DNS.Editor', 1.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (49, 4, NULL, NULL, NULL, 'Max Group Recipients', 'Mail.MaxGroupMembers', 5.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (50, 4, NULL, NULL, NULL, 'Max List Recipients', 'Mail.MaxListMembers', 7.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (51, 1, NULL, NULL, NULL, 'Bandwidth, MB', 'OS.Bandwidth', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (52, 1, NULL, NULL, NULL, 'Disk space, MB', 'OS.Diskspace', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (53, 1, NULL, NULL, NULL, 'Domains', 'OS.Domains', 3.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (54, 1, NULL, NULL, NULL, 'Sub-Domains', 'OS.SubDomains', 4.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (55, 1, NULL, NULL, NULL, 'File Manager', 'OS.FileManager', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (57, 2, NULL, NULL, NULL, 'CGI-BIN Folder', 'Web.CgiBin', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (58, 2, NULL, NULL, NULL, 'Secured Folders', 'Web.SecuredFolders', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (60, 2, NULL, NULL, NULL, 'Web Sites Redirection', 'Web.Redirections', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (61, 2, NULL, NULL, NULL, 'Changing Sites Root Folders', 'Web.HomeFolders', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (64, 10, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2005.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (65, 10, NULL, NULL, NULL, 'Database Backups', 'MsSQL2005.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (66, 10, NULL, NULL, NULL, 'Database Restores', 'MsSQL2005.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (67, 10, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2005.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (70, 11, NULL, NULL, NULL, 'Database Backups', 'MySQL5.Backup', 4.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (71, 1, NULL, NULL, NULL, 'Scheduled Tasks', 'OS.ScheduledTasks', 9.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (72, 1, NULL, NULL, NULL, 'Interval Tasks Allowed', 'OS.ScheduledIntervalTasks', 10.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (73, 1, NULL, NULL, NULL, 'Minimum Tasks Interval, minutes', 'OS.MinimumTaskInterval', 11.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (74, 1, NULL, NULL, NULL, 'Applications Installer', 'OS.AppInstaller', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (75, 1, NULL, NULL, NULL, 'Extra Application Packs', 'OS.ExtraApplications', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (77, 12, NULL, NULL, 1, 'Organization Disk Space, MB', 'Exchange2007.DiskSpace', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (78, 12, NULL, NULL, 1, 'Mailboxes per Organization', 'Exchange2007.Mailboxes', 3.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (79, 12, NULL, NULL, 1, 'Contacts per Organization', 'Exchange2007.Contacts', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (80, 12, NULL, NULL, 1, 'Distribution Lists per Organization', 'Exchange2007.DistributionLists', 5.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (81, 12, NULL, NULL, 1, 'Public Folders per Organization', 'Exchange2007.PublicFolders', 6.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (83, 12, NULL, NULL, NULL, 'POP3 Access', 'Exchange2007.POP3Allowed', 9.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (84, 12, NULL, NULL, NULL, 'IMAP Access', 'Exchange2007.IMAPAllowed', 11.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (85, 12, NULL, NULL, NULL, 'OWA/HTTP Access', 'Exchange2007.OWAAllowed', 13.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (86, 12, NULL, NULL, NULL, 'MAPI Access', 'Exchange2007.MAPIAllowed', 15.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (87, 12, NULL, NULL, NULL, 'ActiveSync Access', 'Exchange2007.ActiveSyncAllowed', 17.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (88, 12, NULL, NULL, NULL, 'Mail Enabled Public Folders Allowed', 'Exchange2007.MailEnabledPublicFolders', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (94, 2, NULL, NULL, NULL, 'ColdFusion', 'Web.ColdFusion', 17.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (96, 2, NULL, NULL, NULL, 'ColdFusion Virtual Directories', 'Web.CFVirtualDirectories', 18.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (97, 2, NULL, NULL, NULL, 'Remote web management allowed', 'Web.RemoteManagement', 20.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (100, 2, NULL, NULL, NULL, 'Dedicated IP Addresses', 'Web.IPAddresses', 19.0, 2, 1);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (102, 4, NULL, NULL, NULL, 'Disable Mailbox Size Edit', 'Mail.DisableSizeEdit', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (103, 6, NULL, NULL, NULL, 'Max Database Size', 'MySQL4.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (104, 6, NULL, NULL, NULL, 'Database Restores', 'MySQL4.Restore', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (105, 6, NULL, NULL, NULL, 'Database Truncate', 'MySQL4.Truncate', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (106, 11, NULL, NULL, NULL, 'Max Database Size', 'MySQL5.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (107, 11, NULL, NULL, NULL, 'Database Restores', 'MySQL5.Restore', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (108, 11, NULL, NULL, NULL, 'Database Truncate', 'MySQL5.Truncate', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (112, 90, NULL, NULL, NULL, 'Database Backups', 'MySQL8.Backup', 4.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (113, 90, NULL, NULL, NULL, 'Max Database Size', 'MySQL8.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (114, 90, NULL, NULL, NULL, 'Database Restores', 'MySQL8.Restore', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (115, 90, NULL, NULL, NULL, 'Database Truncate', 'MySQL8.Truncate', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (122, 91, NULL, NULL, NULL, 'Database Backups', 'MySQL9.Backup', 4.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (123, 91, NULL, NULL, NULL, 'Max Database Size', 'MySQL9.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (124, 91, NULL, NULL, NULL, 'Database Restores', 'MySQL9.Restore', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (125, 91, NULL, NULL, NULL, 'Database Truncate', 'MySQL9.Truncate', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (203, 10, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2005.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (204, 5, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2000.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (207, 13, NULL, NULL, 1, 'Domains per Organizations', 'HostedSolution.Domains', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (208, 20, NULL, NULL, NULL, 'Max site storage, MB', 'HostedSharePoint.MaxStorage', 2.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (213, 22, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2008.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (214, 22, NULL, NULL, NULL, 'Database Backups', 'MsSQL2008.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (215, 22, NULL, NULL, NULL, 'Database Restores', 'MsSQL2008.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (216, 22, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2008.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (217, 22, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2008.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (220, 1, 1, NULL, NULL, 'Domain Pointers', 'OS.DomainPointers', 5.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (221, 23, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2012.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (222, 23, NULL, NULL, NULL, 'Database Backups', 'MsSQL2012.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (223, 23, NULL, NULL, NULL, 'Database Restores', 'MsSQL2012.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (224, 23, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2012.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (225, 23, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2012.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (230, 13, NULL, NULL, NULL, 'Allow to Change UserPrincipalName', 'HostedSolution.AllowChangeUPN', 4.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (301, 30, NULL, NULL, NULL, 'Allow user to create VPS', 'VPS.ManagingAllowed', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (302, 30, NULL, NULL, NULL, 'Number of CPU cores', 'VPS.CpuNumber', 3.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (303, 30, NULL, NULL, NULL, 'Boot from CD allowed', 'VPS.BootCdAllowed', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (304, 30, NULL, NULL, NULL, 'Boot from CD', 'VPS.BootCdEnabled', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (305, 30, NULL, NULL, NULL, 'RAM size, MB', 'VPS.Ram', 4.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (306, 30, NULL, NULL, NULL, 'Hard Drive size, GB', 'VPS.Hdd', 5.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (307, 30, NULL, NULL, NULL, 'DVD drive', 'VPS.DvdEnabled', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (308, 30, NULL, NULL, NULL, 'External Network', 'VPS.ExternalNetworkEnabled', 10.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (309, 30, NULL, NULL, NULL, 'Number of External IP addresses', 'VPS.ExternalIPAddressesNumber', 11.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (310, 30, NULL, NULL, NULL, 'Private Network', 'VPS.PrivateNetworkEnabled', 13.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (311, 30, NULL, NULL, NULL, 'Number of Private IP addresses per VPS', 'VPS.PrivateIPAddressesNumber', 14.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (312, 30, NULL, NULL, NULL, 'Number of Snaphots', 'VPS.SnapshotsNumber', 9.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (313, 30, NULL, NULL, NULL, 'Allow user to Start, Turn off and Shutdown VPS', 'VPS.StartShutdownAllowed', 15.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (314, 30, NULL, NULL, NULL, 'Allow user to Pause, Resume VPS', 'VPS.PauseResumeAllowed', 16.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (315, 30, NULL, NULL, NULL, 'Allow user to Reboot VPS', 'VPS.RebootAllowed', 17.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (316, 30, NULL, NULL, NULL, 'Allow user to Reset VPS', 'VPS.ResetAlowed', 18.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (317, 30, NULL, NULL, NULL, 'Allow user to Re-install VPS', 'VPS.ReinstallAllowed', 19.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (318, 30, NULL, NULL, NULL, 'Monthly bandwidth, GB', 'VPS.Bandwidth', 12.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (319, 31, NULL, NULL, 1, NULL, 'BlackBerry.Users', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (320, 32, NULL, NULL, 1, NULL, 'OCS.Users', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (321, 32, NULL, NULL, NULL, NULL, 'OCS.Federation', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (322, 32, NULL, NULL, NULL, NULL, 'OCS.FederationByDefault', 3.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (323, 32, NULL, NULL, NULL, NULL, 'OCS.PublicIMConnectivity', 4.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (324, 32, NULL, NULL, NULL, NULL, 'OCS.PublicIMConnectivityByDefault', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (325, 32, NULL, NULL, NULL, NULL, 'OCS.ArchiveIMConversation', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (326, 32, NULL, NULL, NULL, NULL, 'OCS.ArchiveIMConvervationByDefault', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (327, 32, NULL, NULL, NULL, NULL, 'OCS.ArchiveFederatedIMConversation', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (328, 32, NULL, NULL, NULL, NULL, 'OCS.ArchiveFederatedIMConversationByDefault', 9.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (329, 32, NULL, NULL, NULL, NULL, 'OCS.PresenceAllowed', 10.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (330, 32, NULL, NULL, NULL, NULL, 'OCS.PresenceAllowedByDefault', 10.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (331, 2, NULL, NULL, NULL, 'ASP.NET 4.0', 'Web.AspNet40', 4.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (332, 2, NULL, NULL, NULL, 'SSL', 'Web.SSL', 21.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (333, 2, NULL, NULL, NULL, 'Allow IP Address Mode Switch', 'Web.AllowIPAddressModeSwitch', 22.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (334, 2, NULL, NULL, NULL, 'Enable Hostname Support', 'Web.EnableHostNameSupport', 23.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (344, 2, NULL, NULL, NULL, 'htaccess', 'Web.Htaccess', 9.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (346, 40, NULL, NULL, NULL, 'Allow user to create VPS', 'VPSForPC.ManagingAllowed', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (347, 40, NULL, NULL, NULL, 'Number of CPU cores', 'VPSForPC.CpuNumber', 3.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (348, 40, NULL, NULL, NULL, 'Boot from CD allowed', 'VPSForPC.BootCdAllowed', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (349, 40, NULL, NULL, NULL, 'Boot from CD', 'VPSForPC.BootCdEnabled', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (350, 40, NULL, NULL, NULL, 'RAM size, MB', 'VPSForPC.Ram', 4.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (351, 40, NULL, NULL, NULL, 'Hard Drive size, GB', 'VPSForPC.Hdd', 5.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (352, 40, NULL, NULL, NULL, 'DVD drive', 'VPSForPC.DvdEnabled', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (353, 40, NULL, NULL, NULL, 'External Network', 'VPSForPC.ExternalNetworkEnabled', 10.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (354, 40, NULL, NULL, NULL, 'Number of External IP addresses', 'VPSForPC.ExternalIPAddressesNumber', 11.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (355, 40, NULL, NULL, NULL, 'Private Network', 'VPSForPC.PrivateNetworkEnabled', 13.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (356, 40, NULL, NULL, NULL, 'Number of Private IP addresses per VPS', 'VPSForPC.PrivateIPAddressesNumber', 14.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (357, 40, NULL, NULL, NULL, 'Number of Snaphots', 'VPSForPC.SnapshotsNumber', 9.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (358, 40, NULL, NULL, NULL, 'Allow user to Start, Turn off and Shutdown VPS', 'VPSForPC.StartShutdownAllowed', 15.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (359, 40, NULL, NULL, NULL, 'Allow user to Pause, Resume VPS', 'VPSForPC.PauseResumeAllowed', 16.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (360, 40, NULL, NULL, NULL, 'Allow user to Reboot VPS', 'VPSForPC.RebootAllowed', 17.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (361, 40, NULL, NULL, NULL, 'Allow user to Reset VPS', 'VPSForPC.ResetAlowed', 18.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (362, 40, NULL, NULL, NULL, 'Allow user to Re-install VPS', 'VPSForPC.ReinstallAllowed', 19.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (363, 40, NULL, NULL, NULL, 'Monthly bandwidth, GB', 'VPSForPC.Bandwidth', 12.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (364, 12, NULL, NULL, NULL, 'Keep Deleted Items (days)', 'Exchange2007.KeepDeletedItemsDays', 19.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (365, 12, NULL, NULL, NULL, 'Maximum Recipients', 'Exchange2007.MaxRecipients', 20.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (366, 12, NULL, NULL, NULL, 'Maximum Send Message Size (Kb)', 'Exchange2007.MaxSendMessageSizeKB', 21.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (367, 12, NULL, NULL, NULL, 'Maximum Receive Message Size (Kb)', 'Exchange2007.MaxReceiveMessageSizeKB', 22.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (368, 12, NULL, NULL, NULL, 'Is Consumer Organization', 'Exchange2007.IsConsumer', 1.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (369, 12, NULL, NULL, NULL, 'Enable Plans Editing', 'Exchange2007.EnablePlansEditing', 23.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (370, 41, NULL, NULL, 1, 'Users', 'Lync.Users', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (371, 41, NULL, NULL, NULL, 'Allow Federation', 'Lync.Federation', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (372, 41, NULL, NULL, NULL, 'Allow Conferencing', 'Lync.Conferencing', 3.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (373, 41, NULL, NULL, NULL, 'Maximum Conference Particiapants', 'Lync.MaxParticipants', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (374, 41, NULL, NULL, NULL, 'Allow Video in Conference', 'Lync.AllowVideo', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (375, 41, NULL, NULL, NULL, 'Allow EnterpriseVoice', 'Lync.EnterpriseVoice', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (376, 41, NULL, NULL, NULL, 'Number of Enterprise Voice Users', 'Lync.EVUsers', 7.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (377, 41, NULL, NULL, NULL, 'Allow National Calls', 'Lync.EVNational', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (378, 41, NULL, NULL, NULL, 'Allow Mobile Calls', 'Lync.EVMobile', 9.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (379, 41, NULL, NULL, NULL, 'Allow International Calls', 'Lync.EVInternational', 10.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (380, 41, NULL, NULL, NULL, 'Enable Plans Editing', 'Lync.EnablePlansEditing', 11.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (381, 41, NULL, NULL, NULL, 'Phone Numbers', 'Lync.PhoneNumbers', 12.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (400, 20, NULL, NULL, NULL, 'Use shared SSL Root', 'HostedSharePoint.UseSharedSSL', 3.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (409, 1, NULL, NULL, NULL, 'Not allow Tenants to Delete Top Level Domains', 'OS.NotAllowTenantDeleteDomains', 13.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (410, 1, NULL, NULL, NULL, 'Not allow Tenants to Create Top Level Domains', 'OS.NotAllowTenantCreateDomains', 12.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (411, 2, NULL, NULL, NULL, 'Application Pools Restart', 'Web.AppPoolsRestart', 13.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (420, 12, NULL, NULL, NULL, 'Allow Litigation Hold', 'Exchange2007.AllowLitigationHold', 24.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (421, 12, NULL, NULL, 1, 'Recoverable Items Space', 'Exchange2007.RecoverableItemsSpace', 25.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (422, 12, NULL, NULL, NULL, 'Disclaimers Allowed', 'Exchange2007.DisclaimersAllowed', 26.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (423, 13, NULL, NULL, 1, 'Security Groups', 'HostedSolution.SecurityGroups', 5.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (424, 12, NULL, NULL, NULL, 'Allow Retention Policy', 'Exchange2013.AllowRetentionPolicy', 27.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (425, 12, NULL, NULL, 1, 'Archiving storage, MB', 'Exchange2013.ArchivingStorage', 29.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (426, 12, NULL, NULL, 1, 'Archiving Mailboxes per Organization', 'Exchange2013.ArchivingMailboxes', 28.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (428, 12, NULL, NULL, 1, 'Resource Mailboxes per Organization', 'Exchange2013.ResourceMailboxes', 31.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (429, 12, NULL, NULL, 1, 'Shared Mailboxes per Organization', 'Exchange2013.SharedMailboxes', 30.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (430, 44, NULL, NULL, 1, 'Disk Storage Space (Mb)', 'EnterpriseStorage.DiskStorageSpace', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (431, 44, NULL, NULL, 1, 'Number of Root Folders', 'EnterpriseStorage.Folders', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (447, 61, NULL, NULL, NULL, 'Enable Spam Filter', 'Filters.Enable', 1.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (448, 61, NULL, NULL, NULL, 'Enable Per-Mailbox Login', 'Filters.EnableEmailUsers', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (450, 45, NULL, NULL, 1, 'Remote Desktop Users', 'RDS.Users', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (451, 45, NULL, NULL, 1, 'Remote Desktop Servers', 'RDS.Servers', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (452, 45, NULL, NULL, NULL, 'Disable user from adding server', 'RDS.DisableUserAddServer', 3.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (453, 45, NULL, NULL, NULL, 'Disable user from removing server', 'RDS.DisableUserDeleteServer', 3.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (468, 45, NULL, NULL, NULL, 'Use Drive Maps', 'EnterpriseStorage.DriveMaps', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (472, 46, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2014.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (473, 46, NULL, NULL, NULL, 'Database Backups', 'MsSQL2014.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (474, 46, NULL, NULL, NULL, 'Database Restores', 'MsSQL2014.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (475, 46, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2014.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (476, 46, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2014.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (491, 45, NULL, NULL, 1, 'Remote Desktop Servers', 'RDS.Collections', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (495, 13, NULL, NULL, 1, 'Deleted Users', 'HostedSolution.DeletedUsers', 6.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (496, 13, NULL, NULL, 1, 'Deleted Users Backup Storage Space, Mb', 'HostedSolution.DeletedUsersBackupStorageSpace', 6.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (551, 73, NULL, NULL, NULL, 'Max site storage, MB', 'HostedSharePointEnterprise.MaxStorage', 2.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (552, 73, NULL, NULL, NULL, 'Use shared SSL Root', 'HostedSharePointEnterprise.UseSharedSSL', 3.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (554, 33, NULL, NULL, NULL, 'Allow user to create VPS', 'VPS2012.ManagingAllowed', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (555, 33, NULL, NULL, NULL, 'Number of CPU cores', 'VPS2012.CpuNumber', 3.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (556, 33, NULL, NULL, NULL, 'Boot from CD allowed', 'VPS2012.BootCdAllowed', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (557, 33, NULL, NULL, NULL, 'Boot from CD', 'VPS2012.BootCdEnabled', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (558, 33, NULL, NULL, NULL, 'RAM size, MB', 'VPS2012.Ram', 4.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (559, 33, NULL, NULL, NULL, 'Hard Drive size, GB', 'VPS2012.Hdd', 5.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (560, 33, NULL, NULL, NULL, 'DVD drive', 'VPS2012.DvdEnabled', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (561, 33, NULL, NULL, NULL, 'External Network', 'VPS2012.ExternalNetworkEnabled', 10.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (562, 33, NULL, NULL, NULL, 'Number of External IP addresses', 'VPS2012.ExternalIPAddressesNumber', 11.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (563, 33, NULL, NULL, NULL, 'Private Network', 'VPS2012.PrivateNetworkEnabled', 13.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (564, 33, NULL, NULL, NULL, 'Number of Private IP addresses per VPS', 'VPS2012.PrivateIPAddressesNumber', 14.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (565, 33, NULL, NULL, NULL, 'Number of Snaphots', 'VPS2012.SnapshotsNumber', 9.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (566, 33, NULL, NULL, NULL, 'Allow user to Start, Turn off and Shutdown VPS', 'VPS2012.StartShutdownAllowed', 15.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (567, 33, NULL, NULL, NULL, 'Allow user to Pause, Resume VPS', 'VPS2012.PauseResumeAllowed', 16.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (568, 33, NULL, NULL, NULL, 'Allow user to Reboot VPS', 'VPS2012.RebootAllowed', 17.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (569, 33, NULL, NULL, NULL, 'Allow user to Reset VPS', 'VPS2012.ResetAlowed', 18.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (570, 33, NULL, NULL, NULL, 'Allow user to Re-install VPS', 'VPS2012.ReinstallAllowed', 19.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (571, 33, NULL, NULL, NULL, 'Monthly bandwidth, GB', 'VPS2012.Bandwidth', 12.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (572, 33, NULL, NULL, NULL, 'Allow user to Replication', 'VPS2012.ReplicationEnabled', 20.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (575, 50, NULL, NULL, NULL, 'Max Database Size', 'MariaDB.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (576, 50, NULL, NULL, NULL, 'Database Backups', 'MariaDB.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (577, 50, NULL, NULL, NULL, 'Database Restores', 'MariaDB.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (578, 50, NULL, NULL, NULL, 'Database Truncate', 'MariaDB.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (579, 50, NULL, NULL, NULL, 'Max Log Size', 'MariaDB.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (581, 52, NULL, NULL, NULL, 'Phone Numbers', 'SfB.PhoneNumbers', 12.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (582, 52, NULL, NULL, 1, 'Users', 'SfB.Users', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (583, 52, NULL, NULL, NULL, 'Allow Federation', 'SfB.Federation', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (584, 52, NULL, NULL, NULL, 'Allow Conferencing', 'SfB.Conferencing', 3.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (585, 52, NULL, NULL, NULL, 'Maximum Conference Particiapants', 'SfB.MaxParticipants', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (586, 52, NULL, NULL, NULL, 'Allow Video in Conference', 'SfB.AllowVideo', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (587, 52, NULL, NULL, NULL, 'Allow EnterpriseVoice', 'SfB.EnterpriseVoice', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (588, 52, NULL, NULL, NULL, 'Number of Enterprise Voice Users', 'SfB.EVUsers', 7.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (589, 52, NULL, NULL, NULL, 'Allow National Calls', 'SfB.EVNational', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (590, 52, NULL, NULL, NULL, 'Allow Mobile Calls', 'SfB.EVMobile', 9.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (591, 52, NULL, NULL, NULL, 'Allow International Calls', 'SfB.EVInternational', 10.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (592, 52, NULL, NULL, NULL, 'Enable Plans Editing', 'SfB.EnablePlansEditing', 11.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (674, 167, NULL, NULL, NULL, 'Allow user to create VPS', 'PROXMOX.ManagingAllowed', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (675, 167, NULL, NULL, NULL, 'Number of CPU cores', 'PROXMOX.CpuNumber', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (676, 167, NULL, NULL, NULL, 'Boot from CD allowed', 'PROXMOX.BootCdAllowed', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (677, 167, NULL, NULL, NULL, 'Boot from CD', 'PROXMOX.BootCdEnabled', 8.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (678, 167, NULL, NULL, NULL, 'RAM size, MB', 'PROXMOX.Ram', 4.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (679, 167, NULL, NULL, NULL, 'Hard Drive size, GB', 'PROXMOX.Hdd', 5.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (680, 167, NULL, NULL, NULL, 'DVD drive', 'PROXMOX.DvdEnabled', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (681, 167, NULL, NULL, NULL, 'External Network', 'PROXMOX.ExternalNetworkEnabled', 10.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (682, 167, NULL, NULL, NULL, 'Number of External IP addresses', 'PROXMOX.ExternalIPAddressesNumber', 11.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (683, 167, NULL, NULL, NULL, 'Private Network', 'PROXMOX.PrivateNetworkEnabled', 13.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (684, 167, NULL, NULL, NULL, 'Number of Private IP addresses per VPS', 'PROXMOX.PrivateIPAddressesNumber', 14.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (685, 167, NULL, NULL, NULL, 'Number of Snaphots', 'PROXMOX.SnapshotsNumber', 9.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (686, 167, NULL, NULL, NULL, 'Allow user to Start, Turn off and Shutdown VPS', 'PROXMOX.StartShutdownAllowed', 15.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (687, 167, NULL, NULL, NULL, 'Allow user to Pause, Resume VPS', 'PROXMOX.PauseResumeAllowed', 16.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (688, 167, NULL, NULL, NULL, 'Allow user to Reboot VPS', 'PROXMOX.RebootAllowed', 17.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (689, 167, NULL, NULL, NULL, 'Allow user to Reset VPS', 'PROXMOX.ResetAlowed', 18.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (690, 167, NULL, NULL, NULL, 'Allow user to Re-install VPS', 'PROXMOX.ReinstallAllowed', 19.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (691, 167, NULL, NULL, NULL, 'Monthly bandwidth, GB', 'PROXMOX.Bandwidth', 12.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (692, 167, NULL, NULL, NULL, 'Allow user to Replication', 'PROXMOX.ReplicationEnabled', 20.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (703, 71, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2016.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (704, 71, NULL, NULL, NULL, 'Database Backups', 'MsSQL2016.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (705, 71, NULL, NULL, NULL, 'Database Restores', 'MsSQL2016.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (706, 71, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2016.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (707, 71, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2016.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (713, 72, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2017.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (714, 72, NULL, NULL, NULL, 'Database Backups', 'MsSQL2017.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (715, 72, NULL, NULL, NULL, 'Database Restores', 'MsSQL2017.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (716, 72, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2017.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (717, 72, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2017.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (723, 74, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2019.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (724, 74, NULL, NULL, NULL, 'Database Backups', 'MsSQL2019.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (725, 74, NULL, NULL, NULL, 'Database Restores', 'MsSQL2019.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (726, 74, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2019.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (727, 74, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2019.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (728, 33, NULL, NULL, NULL, 'Number of Private Network VLANs', 'VPS2012.PrivateVLANsNumber', 14.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (729, 12, NULL, NULL, NULL, 'Automatic Replies via FuseCP Allowed', 'Exchange2013.AutoReply', 32.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (730, 33, NULL, NULL, NULL, 'Additional Hard Drives per VPS', 'VPS2012.AdditionalVhdCount', 6.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (731, 12, NULL, NULL, 1, 'Journaling Mailboxes per Organization', 'Exchange2013.JournalingMailboxes', 31.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (734, 75, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2022.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (735, 75, NULL, NULL, NULL, 'Database Backups', 'MsSQL2022.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (736, 75, NULL, NULL, NULL, 'Database Restores', 'MsSQL2022.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (737, 75, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2022.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (738, 75, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2022.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (750, 33, NULL, NULL, NULL, 'DMZ Network', 'VPS2012.DMZNetworkEnabled', 22.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (751, 33, NULL, NULL, NULL, 'Number of DMZ IP addresses per VPS', 'VPS2012.DMZIPAddressesNumber', 23.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (752, 33, NULL, NULL, NULL, 'Number of DMZ Network VLANs', 'VPS2012.DMZVLANsNumber', 24.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (753, 7, NULL, NULL, NULL, 'Allow editing TTL in DNS Editor', 'DNS.EditTTL', 2.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (754, 4, 1, NULL, NULL, 'Allow changes to access controls', 'Mail.AllowAccessControls', 9.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (762, 76, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2025.MaxDatabaseSize', 3.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (763, 76, NULL, NULL, NULL, 'Database Backups', 'MsSQL2025.Backup', 5.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (764, 76, NULL, NULL, NULL, 'Database Restores', 'MsSQL2025.Restore', 6.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (765, 76, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2025.Truncate', 7.0, 1, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (766, 76, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2025.MaxLogSize', 4.0, 3, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (771, 4, NULL, NULL, NULL, 'Mail Accounts per Domain', 'Mail.Accounts.per.Domain', 1.2, 2, 1);
SELECT changes();


INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (1, 2, 0, '[IP]', '', 1, 'A');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (2, 2, 0, '[IP]', '*', 2, 'A');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (3, 2, 0, '[IP]', 'www', 3, 'A');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (4, 3, 0, '[IP]', 'ftp', 1, 'A');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (5, 4, 0, '[IP]', 'mail', 1, 'A');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (6, 4, 0, '[IP]', 'mail2', 2, 'A');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (7, 4, 10, 'mail.[DOMAIN_NAME]', '', 3, 'MX');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (9, 4, 21, 'mail2.[DOMAIN_NAME]', '', 4, 'MX');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (10, 5, 0, '[IP]', 'mssql', 1, 'A');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (11, 6, 0, '[IP]', 'mysql', 1, 'A');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (12, 8, 0, '[IP]', 'stats', 1, 'A');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (13, 4, 0, 'v=spf1 a mx -all', '', 5, 'TXT');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (14, 12, 0, '[IP]', 'smtp', 1, 'A');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (15, 12, 10, 'smtp.[DOMAIN_NAME]', '', 2, 'MX');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (16, 12, 0, '', 'autodiscover', 3, 'CNAME');
SELECT changes();

INSERT INTO "ResourceGroupDnsRecords" ("RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType")
VALUES (17, 12, 0, '', 'owa', 4, 'CNAME');
SELECT changes();


INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('AUDIT_LOG_DATE', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'List', 'today=Today;yesterday=Yesterday;schedule=Schedule', 5);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('AUDIT_LOG_SEVERITY', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'List', '-1=All;0=Information;1=Warning;2=Error', 2);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('AUDIT_LOG_SOURCE', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'List', '', 3);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('AUDIT_LOG_TASK', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'List', '', 4);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_TO', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SHOW_EXECUTION_LOG', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'List', '0=No;1=Yes', 6);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('BACKUP_FILE_NAME', 'SCHEDULE_TASK_BACKUP', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('DELETE_TEMP_BACKUP', 'SCHEDULE_TASK_BACKUP', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('STORE_PACKAGE_FOLDER', 'SCHEDULE_TASK_BACKUP', 'String', '\', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('STORE_PACKAGE_ID', 'SCHEDULE_TASK_BACKUP', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('STORE_SERVER_FOLDER', 'SCHEDULE_TASK_BACKUP', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('BACKUP_FOLDER', 'SCHEDULE_TASK_BACKUP_DATABASE', 'String', '\backups', 3);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('BACKUP_NAME', 'SCHEDULE_TASK_BACKUP_DATABASE', 'String', 'database_backup.bak', 4);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('DATABASE_GROUP', 'SCHEDULE_TASK_BACKUP_DATABASE', 'List', 'MsSQL2014=SQL Server 2014;MsSQL2016=SQL Server 2016;MsSQL2017=SQL Server 2017;MsSQL2019=SQL Server 2019;MsSQL2022=SQL Server 2022;MySQL5=MySQL 5.0;MariaDB=MariaDB', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('DATABASE_NAME', 'SCHEDULE_TASK_BACKUP_DATABASE', 'String', '', 2);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('ZIP_BACKUP', 'SCHEDULE_TASK_BACKUP_DATABASE', 'List', 'true=Yes;false=No', 5);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_BODY', 'SCHEDULE_TASK_CHECK_WEBSITE', 'MultiString', '', 10);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_FROM', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', 'admin@mysite.com', 7);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_SUBJECT', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', 'Web Site is unavailable', 9);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_TO', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', 'admin@mysite.com', 8);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('PASSWORD', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', NULL, 3);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('RESPONSE_CONTAIN', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', NULL, 5);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('RESPONSE_DOESNT_CONTAIN', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', NULL, 6);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('RESPONSE_STATUS', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', '500', 4);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('URL', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', 'http://', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('USE_RESPONSE_CONTAIN', 'SCHEDULE_TASK_CHECK_WEBSITE', 'Boolean', 'false', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('USE_RESPONSE_DOESNT_CONTAIN', 'SCHEDULE_TASK_CHECK_WEBSITE', 'Boolean', 'false', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('USE_RESPONSE_STATUS', 'SCHEDULE_TASK_CHECK_WEBSITE', 'Boolean', 'false', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('USERNAME', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', NULL, 2);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('BCC_MAIL', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'String', 'admin@mydomain.com', 3);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('ERROR_MAIL_BODY', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'MultiString', 'Hello, <br>we cannot verify the SSL certificate for the domain [domain]. <br><br>Error message: [error] <br><br>Please check if the website is available.', 11);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('ERROR_MAIL_SUBJECT', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'String', 'Certificate error or website is unavailable', 10);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('EXPIRATION_MAIL_BODY', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'MultiString', 'Hello, <br>Your certificate for the [domain] will expire in [expires_in_days] days (on [expires_on_date]).', 5);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('EXPIRATION_MAIL_SUBJECT', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'String', 'Website certificate expiration notice', 4);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SEND_14_DAYS_BEFORE_EXPIRATION', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'true', 7);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SEND_30_DAYS_BEFORE_EXPIRATION', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'true', 6);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SEND_BCC', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'false', 2);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SEND_MAIL_TO_CUSTOMER', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SEND_SSL_ERROR', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'false', 9);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SEND_TODAY_EXPIRED', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'true', 8);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('DAYS_BEFORE', 'SCHEDULE_TASK_DOMAIN_EXPIRATION', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('ENABLE_NOTIFICATION', 'SCHEDULE_TASK_DOMAIN_EXPIRATION', 'Boolean', 'false', 3);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('INCLUDE_NONEXISTEN_DOMAINS', 'SCHEDULE_TASK_DOMAIN_EXPIRATION', 'Boolean', 'false', 4);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_TO', 'SCHEDULE_TASK_DOMAIN_EXPIRATION', 'String', NULL, 2);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('DNS_SERVERS', 'SCHEDULE_TASK_DOMAIN_LOOKUP', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_TO', 'SCHEDULE_TASK_DOMAIN_LOOKUP', 'String', NULL, 2);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('PAUSE_BETWEEN_QUERIES', 'SCHEDULE_TASK_DOMAIN_LOOKUP', 'String', '100', 4);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SERVER_NAME', 'SCHEDULE_TASK_DOMAIN_LOOKUP', 'String', '', 3);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('FILE_PATH', 'SCHEDULE_TASK_FTP_FILES', 'String', '\', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('FTP_FOLDER', 'SCHEDULE_TASK_FTP_FILES', 'String', NULL, 5);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('FTP_PASSWORD', 'SCHEDULE_TASK_FTP_FILES', 'String', NULL, 4);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('FTP_SERVER', 'SCHEDULE_TASK_FTP_FILES', 'String', 'ftp.myserver.com', 2);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('FTP_USERNAME', 'SCHEDULE_TASK_FTP_FILES', 'String', NULL, 3);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('EMAIL', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('EXCHANGE_REPORT', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'Boolean', 'true', 2);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('LYNC_REPORT', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'Boolean', 'true', 4);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('ORGANIZATION_REPORT', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'Boolean', 'true', 7);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SFB_REPORT', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'Boolean', 'true', 5);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SHAREPOINT_REPORT', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'Boolean', 'true', 3);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MARIADB_OVERUSED', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MSSQL_OVERUSED', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MYSQL_OVERUSED', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('OVERUSED_MAIL_BCC', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('OVERUSED_MAIL_BODY', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('OVERUSED_MAIL_FROM', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('OVERUSED_MAIL_SUBJECT', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('OVERUSED_USAGE_THRESHOLD', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '100', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SEND_OVERUSED_EMAIL', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SEND_WARNING_EMAIL', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('WARNING_MAIL_BCC', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('WARNING_MAIL_BODY', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('WARNING_MAIL_FROM', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('WARNING_MAIL_SUBJECT', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('WARNING_USAGE_THRESHOLD', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '80', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('EXECUTABLE_PARAMS', 'SCHEDULE_TASK_RUN_SYSTEM_COMMAND', 'String', '', 3);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('EXECUTABLE_PATH', 'SCHEDULE_TASK_RUN_SYSTEM_COMMAND', 'String', 'Executable.exe', 2);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SERVER_NAME', 'SCHEDULE_TASK_RUN_SYSTEM_COMMAND', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_BODY', 'SCHEDULE_TASK_SEND_MAIL', 'MultiString', NULL, 4);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_FROM', 'SCHEDULE_TASK_SEND_MAIL', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_SUBJECT', 'SCHEDULE_TASK_SEND_MAIL', 'String', NULL, 3);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('MAIL_TO', 'SCHEDULE_TASK_SEND_MAIL', 'String', NULL, 2);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('BANDWIDTH_OVERUSED', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('DISKSPACE_OVERUSED', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SEND_SUSPENSION_EMAIL', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SEND_WARNING_EMAIL', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SUSPEND_OVERUSED', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'Boolean', 'true', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SUSPENSION_MAIL_BCC', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SUSPENSION_MAIL_BODY', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SUSPENSION_MAIL_FROM', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SUSPENSION_MAIL_SUBJECT', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('SUSPENSION_USAGE_THRESHOLD', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', '100', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('WARNING_MAIL_BCC', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('WARNING_MAIL_BODY', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('WARNING_MAIL_FROM', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('WARNING_MAIL_SUBJECT', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('WARNING_USAGE_THRESHOLD', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', '80', 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('DAYS_BEFORE_EXPIRATION', 'SCHEDULE_TASK_USER_PASSWORD_EXPIRATION_NOTIFICATION', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('FOLDER', 'SCHEDULE_TASK_ZIP_FILES', 'String', NULL, 1);
SELECT changes();

INSERT INTO "ScheduleTaskParameters" ("ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder")
VALUES ('ZIP_FILE', 'SCHEDULE_TASK_ZIP_FILES', 'String', '\archive.zip', 2);
SELECT changes();


INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_ACTIVATE_PAID_INVOICES', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', '~/DesktopModules/FuseCP/ScheduleTaskControls/AuditLogReportView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_BACKUP', '~/DesktopModules/FuseCP/ScheduleTaskControls/Backup.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_BACKUP_DATABASE', '~/DesktopModules/FuseCP/ScheduleTaskControls/BackupDatabase.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_CALCULATE_EXCHANGE_DISKSPACE', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_CANCEL_OVERDUE_INVOICES', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_CHECK_WEBSITE', '~/DesktopModules/FuseCP/ScheduleTaskControls/CheckWebsite.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', '~/DesktopModules/FuseCP/ScheduleTaskControls/CheckWebsitesSslView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_DOMAIN_EXPIRATION', '~/DesktopModules/FuseCP/ScheduleTaskControls/DomainExpirationView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_DOMAIN_LOOKUP', '~/DesktopModules/FuseCP/ScheduleTaskControls/DomainLookupView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_FTP_FILES', '~/DesktopModules/FuseCP/ScheduleTaskControls/SendFilesViaFtp.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_GENERATE_INVOICES', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', '~/DesktopModules/FuseCP/ScheduleTaskControls/HostedSolutionReport.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', '~/DesktopModules/FuseCP/ScheduleTaskControls/NotifyOverusedDatabases.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_RUN_PAYMENT_QUEUE', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_RUN_SYSTEM_COMMAND', '~/DesktopModules/FuseCP/ScheduleTaskControls/ExecuteSystemCommand.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_SEND_MAIL', '~/DesktopModules/FuseCP/ScheduleTaskControls/SendEmailNotification.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_SUSPEND_OVERDUE_INVOICES', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_SUSPEND_PACKAGES', '~/DesktopModules/FuseCP/ScheduleTaskControls/SuspendOverusedSpaces.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_USER_PASSWORD_EXPIRATION_NOTIFICATION', '~/DesktopModules/FuseCP/ScheduleTaskControls/UserPasswordExpirationNotificationView.ascx', 'ASP.NET');
SELECT changes();

INSERT INTO "ScheduleTaskViewConfiguration" ("ConfigurationID", "TaskID", "Description", "Environment")
VALUES ('ASP_NET', 'SCHEDULE_TASK_ZIP_FILES', '~/DesktopModules/FuseCP/ScheduleTaskControls/ZipFiles.ascx', 'ASP.NET');
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (2, 1, 0, 1, 'HomeFolder', 1, 1, 0, 0, 'FuseCP.Providers.OS.HomeFolder, FuseCP.Providers.Base', 15);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (5, 1, 0, 1, 'MsSQL2000Database', 1, 5, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 9);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (6, 1, 0, 0, 'MsSQL2000User', 1, 5, 1, 1, 1, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 10);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (7, 1, 0, 1, 'MySQL4Database', 1, 6, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 13);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (8, 1, 0, 0, 'MySQL4User', 1, 6, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 14);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (9, 1, 1, 0, 'FTPAccount', 1, 3, 1, 1, 1, 'FuseCP.Providers.FTP.FtpAccount, FuseCP.Providers.Base', 3);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (10, 1, 1, 1, 'WebSite', 1, 2, 1, 1, 1, 'FuseCP.Providers.Web.WebSite, FuseCP.Providers.Base', 2);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (11, 1, 1, 0, 'MailDomain', 1, 4, 1, 1, 1, 'FuseCP.Providers.Mail.MailDomain, FuseCP.Providers.Base', 8);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName")
VALUES (12, 1, 0, 0, 'DNSZone', 1, 7, 1, 0, 1, 'FuseCP.Providers.DNS.DnsZone, FuseCP.Providers.Base');
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (13, 0, 0, 'Domain', 0, 1, 1, 0, 'FuseCP.Providers.OS.Domain, FuseCP.Providers.Base', 1);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (14, 1, 0, 0, 'StatisticsSite', 1, 8, 1, 1, 0, 'FuseCP.Providers.Statistics.StatsSite, FuseCP.Providers.Base', 17);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (15, 0, 1, 'MailAccount', 0, 4, 1, 0, 'FuseCP.Providers.Mail.MailAccount, FuseCP.Providers.Base', 4);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (16, 0, 0, 'MailAlias', 0, 4, 1, 0, 'FuseCP.Providers.Mail.MailAlias, FuseCP.Providers.Base', 5);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (17, 0, 0, 'MailList', 0, 4, 1, 0, 'FuseCP.Providers.Mail.MailList, FuseCP.Providers.Base', 7);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (18, 0, 0, 'MailGroup', 0, 4, 1, 0, 'FuseCP.Providers.Mail.MailGroup, FuseCP.Providers.Base', 6);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (20, 1, 0, 0, 'ODBCDSN', 1, 1, 1, 1, 0, 'FuseCP.Providers.OS.SystemDSN, FuseCP.Providers.Base', 22);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (21, 1, 0, 1, 'MsSQL2005Database', 1, 10, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 11);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (22, 1, 0, 0, 'MsSQL2005User', 1, 10, 1, 1, 1, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 12);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (23, 1, 0, 1, 'MySQL5Database', 1, 11, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 15);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (24, 1, 0, 0, 'MySQL5User', 1, 11, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 16);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (25, 1, 0, 0, 'SharedSSLFolder', 1, 2, 1, 0, 'FuseCP.Providers.Web.SharedSSLFolder, FuseCP.Providers.Base', 21);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName")
VALUES (28, 1, 0, 0, 'SecondaryDNSZone', 1, 7, 0, 1, 'FuseCP.Providers.DNS.SecondaryDnsZone, FuseCP.Providers.Base');
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (29, 1, 0, 1, 'Organization', 1, 13, 1, 1, 'FuseCP.Providers.HostedSolution.Organization, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (30, 1, NULL, NULL, 'OrganizationDomain', NULL, 13, NULL, NULL, 'FuseCP.Providers.HostedSolution.OrganizationDomain, FuseCP.Providers.Base', 1);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (31, 1, 0, 1, 'MsSQL2008Database', 1, 22, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (32, 1, 0, 0, 'MsSQL2008User', 1, 22, 1, 1, 1, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (33, 0, 0, 'VirtualMachine', 1, 30, 1, 1, 'FuseCP.Providers.Virtualization.VirtualMachine, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (34, 0, 0, 'VirtualSwitch', 1, 30, 1, 1, 'FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base', 2);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (35, 0, 0, 'VMInfo', 1, 40, 1, 1, 'FuseCP.Providers.Virtualization.VMInfo, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (36, 0, 0, 'VirtualSwitch', 1, 40, 1, 1, 'FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base', 2);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (37, 1, 0, 1, 'MsSQL2012Database', 1, 23, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (38, 1, 0, 0, 'MsSQL2012User', 1, 23, 1, 1, 1, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (39, 1, 0, 1, 'MsSQL2014Database', 1, 46, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (40, 1, 0, 0, 'MsSQL2014User', 1, 46, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (41, 0, 0, 'VirtualMachine', 1, 33, 1, 1, 'FuseCP.Providers.Virtualization.VirtualMachine, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (42, 0, 0, 'VirtualSwitch', 1, 33, 1, 1, 'FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base', 2);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (71, 1, 0, 1, 'MsSQL2016Database', 1, 71, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (72, 1, 0, 0, 'MsSQL2016User', 1, 71, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (73, 1, 0, 1, 'MsSQL2017Database', 1, 72, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (74, 1, 0, 0, 'MsSQL2017User', 1, 72, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (75, 1, 0, 1, 'MySQL8Database', 1, 90, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 18);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (76, 1, 0, 0, 'MySQL8User', 1, 90, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 19);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (77, 1, 0, 1, 'MsSQL2019Database', 1, 74, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (78, 1, 0, 0, 'MsSQL2019User', 1, 74, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (79, 1, 0, 1, 'MsSQL2022Database', 1, 75, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (80, 1, 0, 0, 'MsSQL2022User', 1, 75, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (90, 1, 0, 1, 'MySQL9Database', 1, 91, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 20);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (91, 1, 0, 0, 'MySQL9User', 1, 91, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 21);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (143, 0, 0, 'VirtualMachine', 1, 167, 1, 1, 'FuseCP.Providers.Virtualization.VirtualMachine, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (144, 0, 0, 'VirtualSwitch', 1, 167, 1, 1, 'FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base', 2);
SELECT changes();


INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (200, 1, 0, 1, 'SharePointFoundationSiteCollection', 1, 20, 1, 1, 0, 'FuseCP.Providers.SharePoint.SharePointSiteCollection, FuseCP.Providers.Base', 25);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (202, 1, 0, 1, 'MariaDBDatabase', 1, 50, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (203, 1, 0, 0, 'MariaDBUser', 1, 50, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (204, 1, 0, 1, 'SharePointEnterpriseSiteCollection', 1, 73, 1, 1, 0, 'FuseCP.Providers.SharePoint.SharePointEnterpriseSiteCollection, FuseCP.Providers.Base', 100);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (205, 1, 0, 1, 'MsSQL2025Database', 1, 76, 1, 1, 0, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1);
SELECT changes();

INSERT INTO "ServiceItemTypes" ("ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder")
VALUES (206, 1, 0, 0, 'MsSQL2025User', 1, 76, 1, 1, 0, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);
SELECT changes();


INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('CC', 'AccountSummaryLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('EnableLetter', 'AccountSummaryLetter', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'AccountSummaryLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'AccountSummaryLetter', 1, (((((((('<html xmlns="http://www.w3.org/1999/xhtml">' || CHAR(13)) || (CHAR(10) || ('<head>' || CHAR(13)))) || ((CHAR(10) || '    <title>Account Summary Information</title>') || (CHAR(13) || (CHAR(10) || '    <style type="text/css">')))) || (((CHAR(13) || CHAR(10)) || ('		.Summary { background-color: ##ffffff; padding: 5px; }' || (CHAR(13) || CHAR(10)))) || (('		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }' || (CHAR(13) || CHAR(10))) || ('        .Summary A { color: ##0153A4; }' || (CHAR(13) || CHAR(10)))))) || (((('        .Summary { font-family: Tahoma; font-size: 9pt; }' || CHAR(13)) || (CHAR(10) || ('        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }' || CHAR(13)))) || ((CHAR(10) || ('        .Summary H2 { font-size: 1.3em; color: ##1F4978; }' || CHAR(13))) || (CHAR(10) || ('        .Summary TABLE { border: solid 1px ##e5e5e5; }' || CHAR(13))))) || (((CHAR(10) || '        .Summary TH,') || (CHAR(13) || (CHAR(10) || '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }'))) || ((CHAR(13) || (CHAR(10) || '        .Summary TD { padding: 8px; font-size: 9pt; }')) || (CHAR(13) || (CHAR(10) || '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }')))))) || (((((CHAR(13) || CHAR(10)) || ('        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }' || (CHAR(13) || CHAR(10)))) || (('    </style>' || (CHAR(13) || CHAR(10))) || ('</head>' || (CHAR(13) || CHAR(10))))) || ((('<body>' || CHAR(13)) || (CHAR(10) || ('<div class="Summary">' || CHAR(13)))) || ((CHAR(10) || (CHAR(13) || CHAR(10))) || ('<a name="top"></a>' || (CHAR(13) || CHAR(10)))))) || (((('<div class="Header">' || CHAR(13)) || (CHAR(10) || ('	Hosting Account Information' || CHAR(13)))) || ((CHAR(10) || ('</div>' || CHAR(13))) || (CHAR(10) || (CHAR(13) || CHAR(10))))) || ((('<ad:if test="#Signup#">' || CHAR(13)) || (CHAR(10) || ('<p>' || CHAR(13)))) || ((CHAR(10) || ('Hello #user.FirstName#,' || CHAR(13))) || (CHAR(10) || ('</p>' || CHAR(13)))))))) || ((((((CHAR(10) || CHAR(13)) || (CHAR(10) || ('<p>' || CHAR(13)))) || ((CHAR(10) || 'New user account has been created and below you can find its summary information.') || (CHAR(13) || (CHAR(10) || '</p>')))) || (((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || '<h1>Control Panel URL</h1>'))) || ((CHAR(13) || (CHAR(10) || '<table>')) || (CHAR(13) || (CHAR(10) || '    <thead>'))))) || ((((CHAR(13) || CHAR(10)) || ('        <tr>' || (CHAR(13) || CHAR(10)))) || (('            <th>Control Panel URL</th>' || (CHAR(13) || CHAR(10))) || ('            <th>Username</th>' || (CHAR(13) || CHAR(10))))) || ((('            <th>Password</th>' || CHAR(13)) || (CHAR(10) || ('        </tr>' || CHAR(13)))) || ((CHAR(10) || ('    </thead>' || CHAR(13))) || (CHAR(10) || ('    <tbody>' || CHAR(13))))))) || (((((CHAR(10) || '        <tr>') || (CHAR(13) || (CHAR(10) || '            <td><a href="http://panel.HostingCompany.com">http://panel.HostingCompany.com</a></td>'))) || ((CHAR(13) || (CHAR(10) || '            <td>#user.Username#</td>')) || (CHAR(13) || (CHAR(10) || '            <td>#user.Password#</td>')))) || (((CHAR(13) || CHAR(10)) || ('        </tr>' || (CHAR(13) || CHAR(10)))) || (('    </tbody>' || (CHAR(13) || CHAR(10))) || ('</table>' || (CHAR(13) || CHAR(10)))))) || (((('</ad:if>' || CHAR(13)) || (CHAR(10) || (CHAR(13) || CHAR(10)))) || (('<h1>Hosting Spaces</h1>' || (CHAR(13) || CHAR(10))) || ('<p>' || (CHAR(13) || CHAR(10))))) || ((('    The following hosting spaces have been created under your account:' || CHAR(13)) || (CHAR(10) || ('</p>' || CHAR(13)))) || ((CHAR(10) || ('<ad:foreach collection="#Spaces#" var="Space" index="i">' || CHAR(13))) || (CHAR(10) || ('<h2>#Space.PackageName#</h2>' || CHAR(13))))))))) || (((((((CHAR(10) || '<table>') || (CHAR(13) || (CHAR(10) || '	<tbody>'))) || ((CHAR(13) || CHAR(10)) || ('		<tr>' || (CHAR(13) || CHAR(10))))) || ((('			<td class="Label">Hosting Plan:</td>' || CHAR(13)) || (CHAR(10) || ('			<td>' || CHAR(13)))) || ((CHAR(10) || ('				<ad:if test="#not(isnull(Plans[Space.PlanId]))#">#Plans[Space.PlanId].PlanName#<ad:else>System</ad:if>' || CHAR(13))) || (CHAR(10) || ('			</td>' || CHAR(13)))))) || ((((CHAR(10) || '		</tr>') || (CHAR(13) || (CHAR(10) || '		<ad:if test="#not(isnull(Plans[Space.PlanId]))#">'))) || ((CHAR(13) || (CHAR(10) || '		<tr>')) || (CHAR(13) || (CHAR(10) || '			<td class="Label">Purchase Date:</td>')))) || (((CHAR(13) || CHAR(10)) || ('			<td>' || (CHAR(13) || CHAR(10)))) || (('# Space.PurchaseDate#' || (CHAR(13) || CHAR(10))) || ('			</td>' || (CHAR(13) || CHAR(10))))))) || ((((('		</tr>' || CHAR(13)) || (CHAR(10) || ('		<tr>' || CHAR(13)))) || ((CHAR(10) || ('			<td class="Label">Disk Space, MB:</td>' || CHAR(13))) || (CHAR(10) || ('			<td><ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Diskspace" /></td>' || CHAR(13))))) || (((CHAR(10) || '		</tr>') || (CHAR(13) || (CHAR(10) || '		<tr>'))) || ((CHAR(13) || (CHAR(10) || '			<td class="Label">Bandwidth, MB/Month:</td>')) || (CHAR(13) || (CHAR(10) || '			<td><ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Bandwidth" /></td>'))))) || ((((CHAR(13) || CHAR(10)) || ('		</tr>' || (CHAR(13) || CHAR(10)))) || (('		<tr>' || (CHAR(13) || CHAR(10))) || ('			<td class="Label">Maximum Number of Domains:</td>' || (CHAR(13) || CHAR(10))))) || ((('			<td><ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Domains" /></td>' || CHAR(13)) || (CHAR(10) || ('		</tr>' || CHAR(13)))) || ((CHAR(10) || ('		<tr>' || CHAR(13))) || (CHAR(10) || ('			<td class="Label">Maximum Number of Sub-Domains:</td>' || CHAR(13)))))))) || ((((((CHAR(10) || '			<td><ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.SubDomains" /></td>') || (CHAR(13) || (CHAR(10) || '		</tr>'))) || ((CHAR(13) || (CHAR(10) || '		</ad:if>')) || (CHAR(13) || (CHAR(10) || '	</tbody>')))) || (((CHAR(13) || CHAR(10)) || ('</table>' || (CHAR(13) || CHAR(10)))) || (('</ad:foreach>' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || '<ad:if test="#Signup#">'))))) || ((((CHAR(13) || CHAR(10)) || ('<p>' || (CHAR(13) || CHAR(10)))) || (('If you have any questions regarding your hosting account, feel free to contact our support department at any time.' || (CHAR(13) || CHAR(10))) || ('</p>' || (CHAR(13) || CHAR(10))))) || (((CHAR(13) || CHAR(10)) || ('<p>' || (CHAR(13) || CHAR(10)))) || (('Best regards,<br />' || (CHAR(13) || CHAR(10))) || ('FuseCP.<br />' || (CHAR(13) || CHAR(10))))))) || ((((('Web Site: <a href="https://fusecp.com">https://fusecp.com</a><br />' || CHAR(13)) || (CHAR(10) || ('E-Mail: <a href="mailto:support@fusecp.com">support@fusecp.com</a>' || CHAR(13)))) || ((CHAR(10) || ('</p>' || CHAR(13))) || (CHAR(10) || ('</ad:if>' || CHAR(13))))) || (((CHAR(10) || CHAR(13)) || (CHAR(10) || ('<ad:template name="NumericQuota">' || CHAR(13)))) || ((CHAR(10) || ('	<ad:if test="#space.Quotas.ContainsKey(quota)#">' || CHAR(13))) || (CHAR(10) || ('		<ad:if test="#space.Quotas[quota].QuotaAllocatedValue isnot -1#">#space.Quotas[quota].QuotaAllocatedValue#<ad:else>Unlimited</ad:if>' || CHAR(13)))))) || ((((CHAR(10) || '	<ad:else>') || (CHAR(13) || (CHAR(10) || '		0'))) || ((CHAR(13) || (CHAR(10) || '	</ad:if>')) || (CHAR(13) || (CHAR(10) || '</ad:template>')))) || (((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || '</div>'))) || ((CHAR(13) || (CHAR(10) || '</body>')) || (CHAR(13) || (CHAR(10) || '</html>'))))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'AccountSummaryLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'AccountSummaryLetter', 1, '<ad:if test="#Signup#">FuseCP  account has been created for<ad:else>FuseCP  account summary for</ad:if> #user.FirstName# #user.LastName#');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'AccountSummaryLetter', 1, (((((('=================================' || CHAR(13)) || (CHAR(10) || ('   Hosting Account Information' || CHAR(13)))) || ((CHAR(10) || ('=================================' || CHAR(13))) || (CHAR(10) || ('<ad:if test="#Signup#">Hello #user.FirstName#,' || CHAR(13))))) || (((CHAR(10) || (CHAR(13) || CHAR(10))) || ('New user account has been created and below you can find its summary information.' || (CHAR(13) || CHAR(10)))) || ((CHAR(13) || (CHAR(10) || 'Control Panel URL: https://panel.fusecp.com')) || (CHAR(13) || (CHAR(10) || 'Username: #user.Username#'))))) || ((((CHAR(13) || (CHAR(10) || 'Password: #user.Password#')) || (CHAR(13) || (CHAR(10) || '</ad:if>'))) || ((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || ('Hosting Spaces' || CHAR(13))))) || (((CHAR(10) || ('==============' || CHAR(13))) || (CHAR(10) || ('The following hosting spaces have been created under your account:' || CHAR(13)))) || ((CHAR(10) || (CHAR(13) || CHAR(10))) || ('<ad:foreach collection="#Spaces#" var="Space" index="i">' || (CHAR(13) || CHAR(10))))))) || ((((('=== #Space.PackageName# ===' || CHAR(13)) || (CHAR(10) || ('Hosting Plan: <ad:if test="#not(isnull(Plans[Space.PlanId]))#">#Plans[Space.PlanId].PlanName#<ad:else>System</ad:if>' || CHAR(13)))) || ((CHAR(10) || ('<ad:if test="#not(isnull(Plans[Space.PlanId]))#">Purchase Date: #Space.PurchaseDate#' || CHAR(13))) || (CHAR(10) || ('Disk Space, MB: <ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Diskspace" />' || CHAR(13))))) || (((CHAR(10) || ('Bandwidth, MB/Month: <ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Bandwidth" />' || CHAR(13))) || (CHAR(10) || ('Maximum Number of Domains: <ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Domains" />' || CHAR(13)))) || ((CHAR(10) || ('Maximum Number of Sub-Domains: <ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.SubDomains" />' || CHAR(13))) || (CHAR(10) || ('</ad:if>' || CHAR(13)))))) || ((((CHAR(10) || ('</ad:foreach>' || CHAR(13))) || (CHAR(10) || (CHAR(13) || CHAR(10)))) || (('<ad:if test="#Signup#">If you have any questions regarding your hosting account, feel free to contact our support department at any time.' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || 'Best regards,')))) || (((CHAR(13) || (CHAR(10) || 'FuseCP.')) || (CHAR(13) || (CHAR(10) || 'Web Site: https://fusecp.com">'))) || ((CHAR(13) || (CHAR(10) || 'E-Mail: support@fusecp.com')) || (CHAR(13) || (CHAR(10) || '</ad:if><ad:template name="NumericQuota"><ad:if test="#space.Quotas.ContainsKey(quota)#"><ad:if test="#space.Quotas[quota].QuotaAllocatedValue isnot -1#">#space.Quotas[quota].QuotaAllocatedValue#<ad:else>Unlimited</ad:if><ad:else>0</ad:if></ad:template>'))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Transform', 'BandwidthXLST', 1, ((((((('<?xml version="1.0" encoding="UTF-8"?>' || CHAR(13)) || (CHAR(10) || '<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">')) || ((CHAR(13) || CHAR(10)) || ('<xsl:template match="/">' || (CHAR(13) || CHAR(10))))) || ((('  <html>' || CHAR(13)) || (CHAR(10) || '  <body>')) || ((CHAR(13) || CHAR(10)) || ('  <img alt="Embedded Image" width="299" height="60" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASsAAAA8CAYAAAA+AJwjAAAACXBIWXMAAAsTAAALEwEAmpwYAAA4G2lUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4KPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS41LWMwMTQgNzkuMTUxNDgxLCAyMDEzLzAzLzEzLTEyOjA5OjE1ICAgICAgICAiPgogICA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIKICAgICAgICAgICAgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIgogICAgICAgICAgICB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIKICAgICAgICAgICAgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIj4KICAgICAgICAgPHhtcDpDcmVhdG9yVG9vbD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC94bXA6Q3JlYXRvclRvb2w+CiAgICAgICAgIDx4bXA6Q3JlYXRlRGF0ZT4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC94bXA6Q3JlYXRlRGF0ZT4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTYtMDMtMDFUMTQ6NTE6NTgrMDE6MDA8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOk1ldGFkYXRhRGF0ZT4yMDE2LTAzLTAxVDE0OjUxOjU4KzAxOjAwPC94bXA6TWV0YWRhdGFEYXRlPgogICAgICAgICA8ZGM6Zm9ybWF0PmltYWdlL3BuZzwvZGM6Zm9ybWF0PgogICAgICAgICA8cGhvdG9zaG9wOkNvbG9yTW9kZT4zPC9waG90b3Nob3A6Q29sb3JNb2RlPgogICAgICAgICA8eG1wTU06SW5zdGFuY2VJRD54bXAuaWlkOjZhNTdmMWYyLTgyZjYtMjk0MS1hYjFmLTNkOWQ0YjdmMTY2YjwveG1wTU06SW5zdGFuY2VJRD4KICAgICAgICAgPHhtcE1NOkRvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOkRvY3VtZW50SUQ+CiAgICAgICAgIDx4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD4KICAgICAgICAgPHhtcE1NOkhpc3Rvcnk+CiAgICAgICAgICAgIDxyZGY6U2VxPgogICAgICAgICAgICAgICA8cmRmOmxpIHJkZjpwYXJzZVR5cGU9IlJlc291cmNlIj4KICAgICAgICAgICAgICAgICAgPHN0RXZ0OmFjdGlvbj5jcmVhdGVkPC9zdEV2dDphY3Rpb24+CiAgICAgICAgICAgICAgICAgIDxzdEV2dDppbnN0YW5jZUlEPnhtcC5paWQ6NmE1N2YxZjItODJmNi0yOTQxLWFiMWYtM2Q5ZDRiN2YxNjZiPC9zdEV2dDppbnN0YW5jZUlEPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6d2hlbj4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC9zdEV2dDp3aGVuPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6c29mdHdhcmVBZ2VudD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC9zdEV2dDpzb2Z0d2FyZUFnZW50PgogICAgICAgICAgICAgICA8L3JkZjpsaT4KICAgICAgICAgICAgPC9yZGY6U2VxPgogICAgICAgICA8L3htcE1NOkhpc3Rvcnk+CiAgICAgICAgIDx0aWZmOk9yaWVudGF0aW9uPjE8L3RpZmY6T3JpZW50YXRpb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyMDAwMC8xMDAwMDwvdGlmZjpYUmVzb2x1dGlvbj4KICAgICAgICAgPHRpZmY6WVJlc29sdXRpb24+NzIwMDAwLzEwMDAwPC90aWZmOllSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpSZXNvbHV0aW9uVW5pdD4yPC90aWZmOlJlc29sdXRpb25Vbml0PgogICAgICAgICA8ZXhpZjpDb2xvclNwYWNlPjY1NTM1PC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWERpbWVuc2lvbj4yOTk8L2V4aWY6UGl4ZWxYRGltZW5zaW9uPgogICAgICAgICA8ZXhpZjpQaXhlbFlEaW1lbnNpb24+NjA8L2V4aWY6UGl4ZWxZRGltZW5zaW9uPgogICAgICA8L3JkZjpEZXNjcmlwdGlvbj4KICAgPC9yZGY6UkRGPgo8L3g6eG1wbWV0YT4KICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAg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" />' || (CHAR(13) || CHAR(10)))))) || (((('  <h2>Bandwidth Report</h2>' || CHAR(13)) || (CHAR(10) || '  <table border="1">')) || ((CHAR(13) || CHAR(10)) || ('    <tr bgcolor="#66ccff">' || (CHAR(13) || CHAR(10))))) || ((('		<th>PackageID</th>' || CHAR(13)) || (CHAR(10) || '        <th>QuotaValue</th>')) || ((CHAR(13) || CHAR(10)) || ('        <th>Diskspace</th>' || (CHAR(13) || CHAR(10))))))) || ((((('        <th>UsagePercentage</th>' || CHAR(13)) || (CHAR(10) || '        <th>PackageName</th>')) || ((CHAR(13) || CHAR(10)) || ('        <th>PackagesNumber</th>' || (CHAR(13) || CHAR(10))))) || ((('        <th>StatusID</th>' || CHAR(13)) || (CHAR(10) || '        <th>UserID</th>')) || ((CHAR(13) || CHAR(10)) || ('      <th>Username</th>' || (CHAR(13) || CHAR(10)))))) || (((('        <th>FirstName</th>' || CHAR(13)) || (CHAR(10) || '        <th>LastName</th>')) || ((CHAR(13) || CHAR(10)) || ('        <th>FullName</th>' || (CHAR(13) || CHAR(10))))) || ((('        <th>RoleID</th>' || CHAR(13)) || (CHAR(10) || '        <th>Email</th>')) || ((CHAR(13) || CHAR(10)) || ('        <th>UserComments</th> ' || (CHAR(13) || CHAR(10)))))))) || (((((('    </tr>' || CHAR(13)) || (CHAR(10) || '    <xsl:for-each select="//Table1">')) || ((CHAR(13) || CHAR(10)) || ('    <tr>' || (CHAR(13) || CHAR(10))))) || ((('	<td><xsl:value-of select="PackageID"/></td>' || CHAR(13)) || (CHAR(10) || '        <td><xsl:value-of select="QuotaValue"/></td>')) || ((CHAR(13) || CHAR(10)) || ('        <td><xsl:value-of select="Diskspace"/></td>' || (CHAR(13) || CHAR(10)))))) || (((('        <td><xsl:value-of select="UsagePercentage"/>%</td>' || CHAR(13)) || (CHAR(10) || '        <td><xsl:value-of select="PackageName"/></td>')) || ((CHAR(13) || CHAR(10)) || ('        <td><xsl:value-of select="PackagesNumber"/></td>' || (CHAR(13) || CHAR(10))))) || ((('        <td><xsl:value-of select="StatusID"/></td>' || CHAR(13)) || (CHAR(10) || '        <td><xsl:value-of select="UserID"/></td>')) || ((CHAR(13) || CHAR(10)) || ('      <td><xsl:value-of select="Username"/></td>' || (CHAR(13) || CHAR(10))))))) || ((((('        <td><xsl:value-of select="FirstName"/></td>' || CHAR(13)) || (CHAR(10) || '        <td><xsl:value-of select="LastName"/></td>')) || ((CHAR(13) || CHAR(10)) || ('        <td><xsl:value-of select="FullName"/></td>' || (CHAR(13) || CHAR(10))))) || ((('        <td><xsl:value-of select="RoleID"/></td>' || CHAR(13)) || (CHAR(10) || '        <td><xsl:value-of select="Email"/></td>')) || ((CHAR(13) || CHAR(10)) || ('        <td><xsl:value-of select="UserComments"/></td>' || (CHAR(13) || CHAR(10)))))) || (((('    </tr>' || CHAR(13)) || (CHAR(10) || '    </xsl:for-each>')) || ((CHAR(13) || CHAR(10)) || ('  </table>' || (CHAR(13) || CHAR(10))))) || ((('  </body>' || CHAR(13)) || (CHAR(10) || ('  </html>' || CHAR(13)))) || ((CHAR(10) || '</xsl:template>') || (CHAR(13) || (CHAR(10) || '</xsl:stylesheet>')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TransformContentType', 'BandwidthXLST', 1, 'test/html');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TransformSuffix', 'BandwidthXLST', 1, '.htm');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Transform', 'DiskspaceXLST', 1, ((((((('<?xml version="1.0" encoding="UTF-8"?>' || CHAR(13)) || (CHAR(10) || '<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">')) || ((CHAR(13) || CHAR(10)) || ('<xsl:template match="/">' || CHAR(13)))) || (((CHAR(10) || '  <html>') || (CHAR(13) || CHAR(10))) || (('  <body>' || CHAR(13)) || (CHAR(10) || ('  <img alt="Embedded Image" width="299" height="60" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASsAAAA8CAYAAAA+AJwjAAAACXBIWXMAAAsTAAALEwEAmpwYAAA4G2lUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4KPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS41LWMwMTQgNzkuMTUxNDgxLCAyMDEzLzAzLzEzLTEyOjA5OjE1ICAgICAgICAiPgogICA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIKICAgICAgICAgICAgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIgogICAgICAgICAgICB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIKICAgICAgICAgICAgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIj4KICAgICAgICAgPHhtcDpDcmVhdG9yVG9vbD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC94bXA6Q3JlYXRvclRvb2w+CiAgICAgICAgIDx4bXA6Q3JlYXRlRGF0ZT4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC94bXA6Q3JlYXRlRGF0ZT4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTYtMDMtMDFUMTQ6NTE6NTgrMDE6MDA8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOk1ldGFkYXRhRGF0ZT4yMDE2LTAzLTAxVDE0OjUxOjU4KzAxOjAwPC94bXA6TWV0YWRhdGFEYXRlPgogICAgICAgICA8ZGM6Zm9ybWF0PmltYWdlL3BuZzwvZGM6Zm9ybWF0PgogICAgICAgICA8cGhvdG9zaG9wOkNvbG9yTW9kZT4zPC9waG90b3Nob3A6Q29sb3JNb2RlPgogICAgICAgICA8eG1wTU06SW5zdGFuY2VJRD54bXAuaWlkOjZhNTdmMWYyLTgyZjYtMjk0MS1hYjFmLTNkOWQ0YjdmMTY2YjwveG1wTU06SW5zdGFuY2VJRD4KICAgICAgICAgPHhtcE1NOkRvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOkRvY3VtZW50SUQ+CiAgICAgICAgIDx4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD4KICAgICAgICAgPHhtcE1NOkhpc3Rvcnk+CiAgICAgICAgICAgIDxyZGY6U2VxPgogICAgICAgICAgICAgICA8cmRmOmxpIHJkZjpwYXJzZVR5cGU9IlJlc291cmNlIj4KICAgICAgICAgICAgICAgICAgPHN0RXZ0OmFjdGlvbj5jcmVhdGVkPC9zdEV2dDphY3Rpb24+CiAgICAgICAgICAgICAgICAgIDxzdEV2dDppbnN0YW5jZUlEPnhtcC5paWQ6NmE1N2YxZjItODJmNi0yOTQxLWFiMWYtM2Q5ZDRiN2YxNjZiPC9zdEV2dDppbnN0YW5jZUlEPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6d2hlbj4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC9zdEV2dDp3aGVuPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6c29mdHdhcmVBZ2VudD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC9zdEV2dDpzb2Z0d2FyZUFnZW50PgogICAgICAgICAgICAgICA8L3JkZjpsaT4KICAgICAgICAgICAgPC9yZGY6U2VxPgogICAgICAgICA8L3htcE1NOkhpc3Rvcnk+CiAgICAgICAgIDx0aWZmOk9yaWVudGF0aW9uPjE8L3RpZmY6T3JpZW50YXRpb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyMDAwMC8xMDAwMDwvdGlmZjpYUmVzb2x1dGlvbj4KICAgICAgICAgPHRpZmY6WVJlc29sdXRpb24+NzIwMDAwLzEwMDAwPC90aWZmOllSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpSZXNvbHV0aW9uVW5pdD4yPC90aWZmOlJlc29sdXRpb25Vbml0PgogICAgICAgICA8ZXhpZjpDb2xvclNwYWNlPjY1NTM1PC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWERpbWVuc2lvbj4yOTk8L2V4aWY6UGl4ZWxYRGltZW5zaW9uPgogICAgICAgICA8ZXhpZjpQaXhlbFlEaW1lbnNpb24+NjA8L2V4aWY6UGl4ZWxZRGltZW5zaW9uPgogICAgICA8L3JkZjpEZXNjcmlwdGlvbj4KICAgPC9yZGY6UkRGPgo8L3g6eG1wbWV0YT4KICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAg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" />' || CHAR(13)))))) || ((((CHAR(10) || '  <h2>DiskSpace Report</h2>') || (CHAR(13) || CHAR(10))) || (('  <table border="1">' || CHAR(13)) || (CHAR(10) || ('    <tr bgcolor="#66ccff">' || CHAR(13))))) || (((CHAR(10) || '		<th>PackageID</th>') || (CHAR(13) || CHAR(10))) || (('        <th>QuotaValue</th>' || CHAR(13)) || (CHAR(10) || ('        <th>Bandwidth</th>' || CHAR(13))))))) || (((((CHAR(10) || '        <th>UsagePercentage</th>') || (CHAR(13) || CHAR(10))) || (('        <th>PackageName</th>' || CHAR(13)) || (CHAR(10) || ('        <th>PackagesNumber</th>' || CHAR(13))))) || (((CHAR(10) || '        <th>StatusID</th>') || (CHAR(13) || CHAR(10))) || (('        <th>UserID</th>' || CHAR(13)) || (CHAR(10) || ('      <th>Username</th>' || CHAR(13)))))) || ((((CHAR(10) || '        <th>FirstName</th>') || (CHAR(13) || CHAR(10))) || (('        <th>LastName</th>' || CHAR(13)) || (CHAR(10) || ('        <th>FullName</th>' || CHAR(13))))) || (((CHAR(10) || '        <th>RoleID</th>') || (CHAR(13) || CHAR(10))) || (('        <th>Email</th>' || CHAR(13)) || (CHAR(10) || ('    </tr>' || CHAR(13)))))))) || ((((((CHAR(10) || '    <xsl:for-each select="//Table1">') || (CHAR(13) || CHAR(10))) || (('    <tr>' || CHAR(13)) || (CHAR(10) || '	<td><xsl:value-of select="PackageID"/></td>'))) || (((CHAR(13) || CHAR(10)) || ('        <td><xsl:value-of select="QuotaValue"/></td>' || CHAR(13))) || ((CHAR(10) || '        <td><xsl:value-of select="Bandwidth"/></td>') || (CHAR(13) || (CHAR(10) || '        <td><xsl:value-of select="UsagePercentage"/>%</td>'))))) || ((((CHAR(13) || CHAR(10)) || ('        <td><xsl:value-of select="PackageName"/></td>' || CHAR(13))) || ((CHAR(10) || '        <td><xsl:value-of select="PackagesNumber"/></td>') || (CHAR(13) || (CHAR(10) || '        <td><xsl:value-of select="StatusID"/></td>')))) || (((CHAR(13) || CHAR(10)) || ('        <td><xsl:value-of select="UserID"/></td>' || CHAR(13))) || ((CHAR(10) || '      <td><xsl:value-of select="Username"/></td>') || (CHAR(13) || (CHAR(10) || '        <td><xsl:value-of select="FirstName"/></td>')))))) || (((((CHAR(13) || CHAR(10)) || ('        <td><xsl:value-of select="LastName"/></td>' || CHAR(13))) || ((CHAR(10) || '        <td><xsl:value-of select="FullName"/></td>') || (CHAR(13) || (CHAR(10) || '        <td><xsl:value-of select="RoleID"/></td>')))) || (((CHAR(13) || CHAR(10)) || ('        <td><xsl:value-of select="Email"/></td>' || CHAR(13))) || ((CHAR(10) || '        <td><xsl:value-of select="UserComments"/></td>') || (CHAR(13) || (CHAR(10) || '    </tr>'))))) || ((((CHAR(13) || CHAR(10)) || ('    </xsl:for-each>' || CHAR(13))) || ((CHAR(10) || '  </table>') || (CHAR(13) || (CHAR(10) || '  </body>')))) || (((CHAR(13) || CHAR(10)) || ('  </html>' || CHAR(13))) || ((CHAR(10) || '</xsl:template>') || (CHAR(13) || (CHAR(10) || '</xsl:stylesheet>')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TransformContentType', 'DiskspaceXLST', 1, 'text/html');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TransformSuffix', 'DiskspaceXLST', 1, '.htm');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('GridItems', 'DisplayPreferences', 1, '10');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('CC', 'DomainExpirationLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'DomainExpirationLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'DomainExpirationLetter', 1, ((((((('<html xmlns="http://www.w3.org/1999/xhtml">' || (CHAR(13) || CHAR(10))) || (('<head>' || CHAR(13)) || (CHAR(10) || '    <title>Domain Expiration Information</title>'))) || (((CHAR(13) || CHAR(10)) || ('    <style type="text/css">' || CHAR(13))) || ((CHAR(10) || '		.Summary { background-color: ##ffffff; padding: 5px; }') || (CHAR(13) || CHAR(10))))) || (((('		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }' || CHAR(13)) || (CHAR(10) || '        .Summary A { color: ##0153A4; }')) || ((CHAR(13) || CHAR(10)) || ('        .Summary { font-family: Tahoma; font-size: 9pt; }' || CHAR(13)))) || (((CHAR(10) || '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }') || (CHAR(13) || CHAR(10))) || (('        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ' || CHAR(13)) || (CHAR(10) || '        .Summary TABLE { border: solid 1px ##e5e5e5; }'))))) || (((((CHAR(13) || CHAR(10)) || ('        .Summary TH,' || CHAR(13))) || ((CHAR(10) || '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }') || (CHAR(13) || CHAR(10)))) || ((('        .Summary TD { padding: 8px; font-size: 9pt; }' || CHAR(13)) || (CHAR(10) || '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }')) || ((CHAR(13) || CHAR(10)) || ('        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }' || CHAR(13))))) || ((((CHAR(10) || '    </style>') || (CHAR(13) || CHAR(10))) || (('</head>' || CHAR(13)) || (CHAR(10) || '<body>'))) || (((CHAR(13) || CHAR(10)) || ('<div class="Summary">' || CHAR(13))) || ((CHAR(10) || CHAR(13)) || (CHAR(10) || '<a name="top"></a>')))))) || (((((CHAR(13) || (CHAR(10) || '<div class="Header">')) || ((CHAR(13) || CHAR(10)) || ('	Domain Expiration Information' || CHAR(13)))) || (((CHAR(10) || '</div>') || (CHAR(13) || CHAR(10))) || ((CHAR(13) || CHAR(10)) || ('<ad:if test="#user#">' || CHAR(13))))) || ((((CHAR(10) || '<p>') || (CHAR(13) || CHAR(10))) || (('Hello #user.FirstName#,' || CHAR(13)) || (CHAR(10) || '</p>'))) || (((CHAR(13) || CHAR(10)) || ('</ad:if>' || CHAR(13))) || ((CHAR(10) || CHAR(13)) || (CHAR(10) || '<p>'))))) || (((((CHAR(13) || CHAR(10)) || ('Please, find below details of your domain expiration information.' || CHAR(13))) || ((CHAR(10) || '</p>') || (CHAR(13) || CHAR(10)))) || (((CHAR(13) || CHAR(10)) || ('<table>' || CHAR(13))) || ((CHAR(10) || '    <thead>') || (CHAR(13) || CHAR(10))))) || (((('        <tr>' || CHAR(13)) || (CHAR(10) || '            <th>Domain</th>')) || ((CHAR(13) || CHAR(10)) || ('			<th>Registrar</th>' || CHAR(13)))) || (((CHAR(10) || '			<th>Customer</th>') || (CHAR(13) || CHAR(10))) || (('            <th>Expiration Date</th>' || CHAR(13)) || (CHAR(10) || '        </tr>'))))))) || ((((((CHAR(13) || (CHAR(10) || '    </thead>')) || ((CHAR(13) || CHAR(10)) || ('    <tbody>' || CHAR(13)))) || (((CHAR(10) || '            <ad:foreach collection="#Domains#" var="Domain" index="i">') || (CHAR(13) || CHAR(10))) || (('        <tr>' || CHAR(13)) || (CHAR(10) || '            <td>#Domain.DomainName#</td>')))) || ((((CHAR(13) || CHAR(10)) || ('			<td>#iif(isnull(Domain.Registrar), "", Domain.Registrar)#</td>' || CHAR(13))) || ((CHAR(10) || '			<td>#Domain.Customer#</td>') || (CHAR(13) || CHAR(10)))) || ((('            <td>#iif(isnull(Domain.ExpirationDate), "", Domain.ExpirationDate)#</td>' || CHAR(13)) || (CHAR(10) || '        </tr>')) || ((CHAR(13) || CHAR(10)) || ('    </ad:foreach>' || CHAR(13)))))) || (((((CHAR(10) || '    </tbody>') || (CHAR(13) || CHAR(10))) || (('</table>' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || '<ad:if test="#IncludeNonExistenDomains#">') || (CHAR(13) || CHAR(10))) || (('	<p>' || CHAR(13)) || (CHAR(10) || '	Please, find below details of your non-existen domains.')))) || ((((CHAR(13) || CHAR(10)) || ('	</p>' || CHAR(13))) || ((CHAR(10) || CHAR(13)) || (CHAR(10) || '	<table>'))) || (((CHAR(13) || CHAR(10)) || ('		<thead>' || CHAR(13))) || ((CHAR(10) || '			<tr>') || (CHAR(13) || CHAR(10))))))) || (((((('				<th>Domain</th>' || CHAR(13)) || (CHAR(10) || '				<th>Customer</th>')) || ((CHAR(13) || CHAR(10)) || ('			</tr>' || CHAR(13)))) || (((CHAR(10) || '		</thead>') || (CHAR(13) || CHAR(10))) || (('		<tbody>' || CHAR(13)) || (CHAR(10) || '				<ad:foreach collection="#NonExistenDomains#" var="Domain" index="i">')))) || ((((CHAR(13) || CHAR(10)) || ('			<tr>' || CHAR(13))) || ((CHAR(10) || '				<td>#Domain.DomainName#</td>') || (CHAR(13) || CHAR(10)))) || ((('				<td>#Domain.Customer#</td>' || CHAR(13)) || (CHAR(10) || '			</tr>')) || ((CHAR(13) || CHAR(10)) || ('		</ad:foreach>' || CHAR(13)))))) || (((((CHAR(10) || '		</tbody>') || (CHAR(13) || CHAR(10))) || (('	</table>' || CHAR(13)) || (CHAR(10) || '</ad:if>'))) || (((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))) || ((CHAR(13) || CHAR(10)) || ('<p>' || CHAR(13))))) || ((((CHAR(10) || 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.') || (CHAR(13) || CHAR(10))) || (('</p>' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || '<p>') || (CHAR(13) || CHAR(10))) || (('Best regards' || CHAR(13)) || (CHAR(10) || '</p>')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'DomainExpirationLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'DomainExpirationLetter', 1, 'Domain expiration notification');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'DomainExpirationLetter', 1, (((((('=================================' || CHAR(13)) || (CHAR(10) || ('   Domain Expiration Information' || CHAR(13)))) || ((CHAR(10) || '=================================') || (CHAR(13) || (CHAR(10) || '<ad:if test="#user#">')))) || (((CHAR(13) || CHAR(10)) || ('Hello #user.FirstName#,' || (CHAR(13) || CHAR(10)))) || (('</ad:if>' || CHAR(13)) || (CHAR(10) || (CHAR(13) || CHAR(10)))))) || (((('Please, find below details of your domain expiration information.' || CHAR(13)) || (CHAR(10) || (CHAR(13) || CHAR(10)))) || ((CHAR(13) || CHAR(10)) || ('<ad:foreach collection="#Domains#" var="Domain" index="i">' || (CHAR(13) || CHAR(10))))) || ((('	Domain: #Domain.DomainName#' || CHAR(13)) || (CHAR(10) || ('	Registrar: #iif(isnull(Domain.Registrar), "", Domain.Registrar)#' || CHAR(13)))) || ((CHAR(10) || ('	Customer: #Domain.Customer#' || CHAR(13))) || (CHAR(10) || ('	Expiration Date: #iif(isnull(Domain.ExpirationDate), "", Domain.ExpirationDate)#' || CHAR(13))))))) || (((((CHAR(10) || CHAR(13)) || (CHAR(10) || ('</ad:foreach>' || CHAR(13)))) || ((CHAR(10) || CHAR(13)) || (CHAR(10) || ('<ad:if test="#IncludeNonExistenDomains#">' || CHAR(13))))) || (((CHAR(10) || 'Please, find below details of your non-existen domains.') || (CHAR(13) || (CHAR(10) || CHAR(13)))) || ((CHAR(10) || '<ad:foreach collection="#NonExistenDomains#" var="Domain" index="i">') || (CHAR(13) || (CHAR(10) || '	Domain: #Domain.DomainName#'))))) || ((((CHAR(13) || CHAR(10)) || ('	Customer: #Domain.Customer#' || (CHAR(13) || CHAR(10)))) || ((CHAR(13) || CHAR(10)) || ('</ad:foreach>' || (CHAR(13) || CHAR(10))))) || ((('</ad:if>' || CHAR(13)) || (CHAR(10) || (CHAR(13) || CHAR(10)))) || (('If you have any questions regarding your hosting account, feel free to contact our support department at any time.' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || 'Best regards'))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('CC', 'DomainLookupLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'DomainLookupLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'DomainLookupLetter', 1, ((((((('<html xmlns="http://www.w3.org/1999/xhtml">' || (CHAR(13) || CHAR(10))) || ('<head>' || (CHAR(13) || CHAR(10)))) || (('    <title>MX and NS Changes Information</title>' || (CHAR(13) || CHAR(10))) || (('    <style type="text/css">' || CHAR(13)) || (CHAR(10) || '		.Summary { background-color: ##ffffff; padding: 5px; }')))) || (((CHAR(13) || (CHAR(10) || '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }')) || (CHAR(13) || (CHAR(10) || '        .Summary A { color: ##0153A4; }'))) || ((CHAR(13) || (CHAR(10) || '        .Summary { font-family: Tahoma; font-size: 9pt; }')) || ((CHAR(13) || CHAR(10)) || ('        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }' || CHAR(13)))))) || ((((CHAR(10) || ('        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ' || CHAR(13))) || (CHAR(10) || ('		.Summary H3 { font-size: 1em; color: ##1F4978; } ' || CHAR(13)))) || ((CHAR(10) || ('        .Summary TABLE { border: solid 1px ##e5e5e5; }' || CHAR(13))) || ((CHAR(10) || '        .Summary TH,') || (CHAR(13) || CHAR(10))))) || ((('        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }' || (CHAR(13) || CHAR(10))) || ('        .Summary TD { padding: 8px; font-size: 9pt; }' || (CHAR(13) || CHAR(10)))) || (('        .Summary UL LI { font-size: 1.1em; font-weight: bold; }' || (CHAR(13) || CHAR(10))) || (('        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }' || CHAR(13)) || (CHAR(10) || '    </style>')))))) || (((((CHAR(13) || (CHAR(10) || '</head>')) || (CHAR(13) || (CHAR(10) || '<body>'))) || ((CHAR(13) || (CHAR(10) || '<div class="Summary">')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))))) || ((('<a name="top"></a>' || (CHAR(13) || CHAR(10))) || ('<div class="Header">' || (CHAR(13) || CHAR(10)))) || (('	MX and NS Changes Information' || (CHAR(13) || CHAR(10))) || (('</div>' || CHAR(13)) || (CHAR(10) || CHAR(13)))))) || ((((CHAR(10) || ('<ad:if test="#user#">' || CHAR(13))) || (CHAR(10) || ('<p>' || CHAR(13)))) || ((CHAR(10) || ('Hello #user.FirstName#,' || CHAR(13))) || ((CHAR(10) || '</p>') || (CHAR(13) || CHAR(10))))) || ((('</ad:if>' || (CHAR(13) || CHAR(10))) || ((CHAR(13) || CHAR(10)) || ('<p>' || CHAR(13)))) || ((CHAR(10) || ('Please, find below details of MX and NS changes.' || CHAR(13))) || ((CHAR(10) || '</p>') || (CHAR(13) || CHAR(10)))))))) || ((((((CHAR(13) || (CHAR(10) || '    <ad:foreach collection="#Domains#" var="Domain" index="i">')) || (CHAR(13) || (CHAR(10) || '	<h2>#Domain.DomainName# - #DomainUsers[Domain.PackageId].FirstName# #DomainUsers[Domain.PackageId].LastName#</h2>'))) || ((CHAR(13) || (CHAR(10) || '	<h3>#iif(isnull(Domain.Registrar), "", Domain.Registrar)# #iif(isnull(Domain.ExpirationDate), "", Domain.ExpirationDate)#</h3>')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))))) || ((('	<table>' || (CHAR(13) || CHAR(10))) || ('	    <thead>' || (CHAR(13) || CHAR(10)))) || (('	        <tr>' || (CHAR(13) || CHAR(10))) || (('	            <th>DNS</th>' || CHAR(13)) || (CHAR(10) || '				<th>Type</th>'))))) || ((((CHAR(13) || (CHAR(10) || '				<th>Status</th>')) || (CHAR(13) || (CHAR(10) || '	            <th>Old Value</th>'))) || ((CHAR(13) || (CHAR(10) || '                <th>New Value</th>')) || ((CHAR(13) || CHAR(10)) || ('	        </tr>' || CHAR(13))))) || (((CHAR(10) || ('	    </thead>' || CHAR(13))) || (CHAR(10) || ('	    <tbody>' || CHAR(13)))) || ((CHAR(10) || ('	        <ad:foreach collection="#Domain.DnsChanges#" var="DnsChange" index="j">' || CHAR(13))) || ((CHAR(10) || '	        <tr>') || (CHAR(13) || CHAR(10))))))) || ((((('	            <td>#DnsChange.DnsServer#</td>' || (CHAR(13) || CHAR(10))) || ('	            <td>#DnsChange.Type#</td>' || (CHAR(13) || CHAR(10)))) || (('				<td>#DnsChange.Status#</td>' || (CHAR(13) || CHAR(10))) || (('                <td>#DnsChange.OldRecord.Value#</td>' || CHAR(13)) || (CHAR(10) || '	            <td>#DnsChange.NewRecord.Value#</td>')))) || (((CHAR(13) || (CHAR(10) || '	        </tr>')) || (CHAR(13) || (CHAR(10) || '	    	</ad:foreach>'))) || ((CHAR(13) || (CHAR(10) || '	    </tbody>')) || ((CHAR(13) || CHAR(10)) || ('	</table>' || CHAR(13)))))) || ((((CHAR(10) || ('	' || CHAR(13))) || (CHAR(10) || ('    </ad:foreach>' || CHAR(13)))) || ((CHAR(10) || (CHAR(13) || CHAR(10))) || (('<p>' || CHAR(13)) || (CHAR(10) || 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.')))) || (((CHAR(13) || (CHAR(10) || '</p>')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10)))) || (('<p>' || (CHAR(13) || CHAR(10))) || (('Best regards' || CHAR(13)) || (CHAR(10) || '</p>')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('NoChangesHtmlBody', 'DomainLookupLetter', 1, (((((('<html xmlns="http://www.w3.org/1999/xhtml">' || (CHAR(13) || CHAR(10))) || (('<head>' || CHAR(13)) || (CHAR(10) || '    <title>MX and NS Changes Information</title>'))) || (((CHAR(13) || CHAR(10)) || ('    <style type="text/css">' || CHAR(13))) || ((CHAR(10) || '		.Summary { background-color: ##ffffff; padding: 5px; }') || (CHAR(13) || CHAR(10))))) || ((('		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }' || (CHAR(13) || CHAR(10))) || (('        .Summary A { color: ##0153A4; }' || CHAR(13)) || (CHAR(10) || '        .Summary { font-family: Tahoma; font-size: 9pt; }'))) || (((CHAR(13) || CHAR(10)) || ('        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }' || CHAR(13))) || ((CHAR(10) || '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ') || (CHAR(13) || CHAR(10)))))) || (((('        .Summary TABLE { border: solid 1px ##e5e5e5; }' || (CHAR(13) || CHAR(10))) || (('        .Summary TH,' || CHAR(13)) || (CHAR(10) || '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }'))) || (((CHAR(13) || CHAR(10)) || ('        .Summary TD { padding: 8px; font-size: 9pt; }' || CHAR(13))) || ((CHAR(10) || '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }') || (CHAR(13) || CHAR(10))))) || (((('        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }' || CHAR(13)) || (CHAR(10) || '    </style>')) || ((CHAR(13) || CHAR(10)) || ('</head>' || CHAR(13)))) || (((CHAR(10) || '<body>') || (CHAR(13) || CHAR(10))) || (('<div class="Summary">' || CHAR(13)) || (CHAR(10) || CHAR(13))))))) || (((((CHAR(10) || ('<a name="top"></a>' || CHAR(13))) || ((CHAR(10) || '<div class="Header">') || (CHAR(13) || CHAR(10)))) || ((('	MX and NS Changes Information' || CHAR(13)) || (CHAR(10) || '</div>')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))))) || ((('<ad:if test="#user#">' || (CHAR(13) || CHAR(10))) || (('<p>' || CHAR(13)) || (CHAR(10) || 'Hello #user.FirstName#,'))) || (((CHAR(13) || CHAR(10)) || ('</p>' || CHAR(13))) || ((CHAR(10) || '</ad:if>') || (CHAR(13) || CHAR(10)))))) || ((((CHAR(13) || (CHAR(10) || '<p>')) || ((CHAR(13) || CHAR(10)) || ('No MX and NS changes have been found.' || CHAR(13)))) || (((CHAR(10) || '</p>') || (CHAR(13) || CHAR(10))) || ((CHAR(13) || CHAR(10)) || ('<p>' || CHAR(13))))) || ((((CHAR(10) || 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.') || (CHAR(13) || CHAR(10))) || (('</p>' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || '<p>') || (CHAR(13) || CHAR(10))) || (('Best regards' || CHAR(13)) || (CHAR(10) || '</p>'))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('NoChangesTextBody', 'DomainLookupLetter', 1, ((((('=================================' || CHAR(13)) || (CHAR(10) || '   MX and NS Changes Information')) || ((CHAR(13) || CHAR(10)) || ('=================================' || CHAR(13)))) || (((CHAR(10) || '<ad:if test="#user#">') || (CHAR(13) || CHAR(10))) || (('Hello #user.FirstName#,' || CHAR(13)) || (CHAR(10) || '</ad:if>')))) || ((((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))) || (('No MX and NS changes have been founded.' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.') || (CHAR(13) || CHAR(10))) || ((CHAR(13) || CHAR(10)) || ('Best regards' || (CHAR(13) || CHAR(10))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'DomainLookupLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'DomainLookupLetter', 1, 'MX and NS changes notification');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'DomainLookupLetter', 1, (((((('=================================' || CHAR(13)) || (CHAR(10) || ('   MX and NS Changes Information' || CHAR(13)))) || ((CHAR(10) || '=================================') || (CHAR(13) || (CHAR(10) || '<ad:if test="#user#">')))) || (((CHAR(13) || CHAR(10)) || ('Hello #user.FirstName#,' || (CHAR(13) || CHAR(10)))) || (('</ad:if>' || CHAR(13)) || (CHAR(10) || (CHAR(13) || CHAR(10)))))) || (((('Please, find below details of MX and NS changes.' || CHAR(13)) || (CHAR(10) || (CHAR(13) || CHAR(10)))) || ((CHAR(13) || CHAR(10)) || ('<ad:foreach collection="#Domains#" var="Domain" index="i">' || (CHAR(13) || CHAR(10))))) || (((CHAR(13) || CHAR(10)) || ('# Domain.DomainName# - #DomainUsers[Domain.PackageId].FirstName# #DomainUsers[Domain.PackageId].LastName#' || (CHAR(13) || CHAR(10)))) || ((' Registrar:      #iif(isnull(Domain.Registrar), "", Domain.Registrar)#' || (CHAR(13) || CHAR(10))) || (' ExpirationDate: #iif(isnull(Domain.ExpirationDate), "", Domain.ExpirationDate)#' || (CHAR(13) || CHAR(10))))))) || (((((CHAR(13) || CHAR(10)) || ('        <ad:foreach collection="#Domain.DnsChanges#" var="DnsChange" index="j">' || (CHAR(13) || CHAR(10)))) || (('            DNS:       #DnsChange.DnsServer#' || CHAR(13)) || (CHAR(10) || ('            Type:      #DnsChange.Type#' || CHAR(13))))) || (((CHAR(10) || '	    Status:    #DnsChange.Status#') || (CHAR(13) || (CHAR(10) || '            Old Value: #DnsChange.OldRecord.Value#'))) || ((CHAR(13) || (CHAR(10) || '            New Value: #DnsChange.NewRecord.Value#')) || (CHAR(13) || (CHAR(10) || CHAR(13)))))) || ((((CHAR(10) || '    	</ad:foreach>') || (CHAR(13) || (CHAR(10) || '</ad:foreach>'))) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || CHAR(13))))) || (((CHAR(10) || CHAR(13)) || (CHAR(10) || ('If you have any questions regarding your hosting account, feel free to contact our support department at any time.' || CHAR(13)))) || ((CHAR(10) || (CHAR(13) || CHAR(10))) || ('Best regards' || (CHAR(13) || CHAR(10)))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'ExchangeMailboxSetupLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'ExchangeMailboxSetupLetter', 1, ((((((((('<html xmlns="http://www.w3.org/1999/xhtml">' || CHAR(13)) || (CHAR(10) || ('<head>' || CHAR(13)))) || ((CHAR(10) || '    <title>Account Summary Information</title>') || (CHAR(13) || (CHAR(10) || '    <style type="text/css">')))) || (((CHAR(13) || CHAR(10)) || ('        body {font-family: ''Segoe UI Light'',''Open Sans'',Arial!important;color:black;}' || (CHAR(13) || CHAR(10)))) || (('        p {color:black;}' || (CHAR(13) || CHAR(10))) || ('		.Summary { background-color: ##ffffff; padding: 5px; }' || (CHAR(13) || CHAR(10)))))) || (((('		.SummaryHeader { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }' || CHAR(13)) || (CHAR(10) || ('        .Summary A { color: ##0153A4; }' || CHAR(13)))) || ((CHAR(10) || ('        .Summary { font-family: Tahoma; font-size: 9pt; }' || CHAR(13))) || (CHAR(10) || ('        .Summary H1 { font-size: 1.5em; color: ##1F4978; border-bottom: dotted 3px ##efefef; font-weight:normal; }' || CHAR(13))))) || (((CHAR(10) || '        .Summary H2 { font-size: 1.2em; color: ##1F4978; } ') || (CHAR(13) || (CHAR(10) || '        .Summary TABLE { border: solid 1px ##e5e5e5; }'))) || ((CHAR(13) || (CHAR(10) || '        .Summary TH,')) || (CHAR(13) || (CHAR(10) || '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }')))))) || (((((CHAR(13) || CHAR(10)) || ('        .Summary TD { padding: 8px; font-size: 9pt; color:black;}' || (CHAR(13) || CHAR(10)))) || (('        .Summary UL LI { font-size: 1.1em; font-weight: bold; }' || CHAR(13)) || (CHAR(10) || ('        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }' || CHAR(13))))) || (((CHAR(10) || '        .Label { color:##1F4978; }') || (CHAR(13) || (CHAR(10) || '        .menu-bar a {padding: 15px 0;display: inline-block;}'))) || ((CHAR(13) || (CHAR(10) || '    </style>')) || (CHAR(13) || (CHAR(10) || '</head>'))))) || ((((CHAR(13) || CHAR(10)) || ('<body>' || (CHAR(13) || CHAR(10)))) || (('<table border="0" cellspacing="0" cellpadding="0" width="100%"><!-- was 800 -->' || (CHAR(13) || CHAR(10))) || ('<tbody>' || (CHAR(13) || CHAR(10))))) || ((('<tr>' || CHAR(13)) || (CHAR(10) || ('<td style="padding: 10px 20px 10px 20px; background-color: ##e1e1e1;">' || CHAR(13)))) || ((CHAR(10) || ('<table border="0" cellspacing="0" cellpadding="0" width="100%">' || CHAR(13))) || (CHAR(10) || ('<tbody>' || CHAR(13)))))))) || ((((((CHAR(10) || '<tr>') || (CHAR(13) || (CHAR(10) || '<td style="text-align: left; padding: 0px 0px 2px 0px;"><a href=""><img src="" border="0" alt="" /></a></td>'))) || ((CHAR(13) || CHAR(10)) || ('</tr>' || (CHAR(13) || CHAR(10))))) || ((('</tbody>' || CHAR(13)) || (CHAR(10) || ('</table>' || CHAR(13)))) || ((CHAR(10) || ('<table border="0" cellspacing="0" cellpadding="0" width="100%">' || CHAR(13))) || (CHAR(10) || ('<tbody>' || CHAR(13)))))) || ((((CHAR(10) || '<tr>') || (CHAR(13) || (CHAR(10) || '<td style="padding-bottom: 10px;">'))) || ((CHAR(13) || (CHAR(10) || '<table border="0" cellspacing="0" cellpadding="0" width="100%">')) || (CHAR(13) || (CHAR(10) || '<tbody>')))) || (((CHAR(13) || CHAR(10)) || ('<tr>' || (CHAR(13) || CHAR(10)))) || (('<td style="background-color: ##2e8bcc; padding: 3px;">' || (CHAR(13) || CHAR(10))) || ('<table class="menu-bar" border="0" cellspacing="0" cellpadding="0" width="100%">' || (CHAR(13) || CHAR(10))))))) || ((((('<tbody>' || CHAR(13)) || (CHAR(10) || ('<tr>' || CHAR(13)))) || ((CHAR(10) || '<td style="text-align: center;" width="20%"><a style="color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;" href=""</a></td>') || (CHAR(13) || (CHAR(10) || '<td style="text-align: center;" width="20%"><a style="color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;" href=""></a></td>')))) || (((CHAR(13) || CHAR(10)) || ('<td style="text-align: center;" width="20%"><a style="color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;" href=""></a></td>' || (CHAR(13) || CHAR(10)))) || (('<td style="text-align: center;" width="20%"><a style="color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;" href=""></a></td>' || (CHAR(13) || CHAR(10))) || ('<td style="text-align: center;" width="20%"><a style="color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;" href=""></a></td>' || (CHAR(13) || CHAR(10)))))) || (((('</tr>' || CHAR(13)) || (CHAR(10) || ('</tbody>' || CHAR(13)))) || ((CHAR(10) || ('</table>' || CHAR(13))) || (CHAR(10) || ('</td>' || CHAR(13))))) || (((CHAR(10) || '</tr>') || (CHAR(13) || (CHAR(10) || '</tbody>'))) || ((CHAR(13) || (CHAR(10) || '</table>')) || (CHAR(13) || (CHAR(10) || '</td>')))))))) || (((((((CHAR(13) || CHAR(10)) || ('</tr>' || (CHAR(13) || CHAR(10)))) || (('</tbody>' || CHAR(13)) || (CHAR(10) || ('</table>' || CHAR(13))))) || (((CHAR(10) || '<table border="0" cellspacing="0" cellpadding="0" width="100%">') || (CHAR(13) || (CHAR(10) || '<tbody>'))) || ((CHAR(13) || (CHAR(10) || '<tr>')) || (CHAR(13) || (CHAR(10) || '<td style="background-color: ##ffffff;">'))))) || ((((CHAR(13) || CHAR(10)) || ('<table border="0" cellspacing="0" cellpadding="0" width="100%"><!-- was 759 -->' || (CHAR(13) || CHAR(10)))) || (('<tbody>' || (CHAR(13) || CHAR(10))) || ('<tr>' || (CHAR(13) || CHAR(10))))) || ((('<td style="vertical-align: top; padding: 10px 10px 0px 10px;" width="100%">' || CHAR(13)) || (CHAR(10) || ('<table border="0" cellspacing="0" cellpadding="0" width="100%">' || CHAR(13)))) || ((CHAR(10) || ('<tbody>' || CHAR(13))) || (CHAR(10) || ('<tr>' || CHAR(13))))))) || (((((CHAR(10) || '<td style="font-family: ''Segoe UI Light'',''Open Sans'',Arial; padding: 0px 10px 0px 0px;">') || (CHAR(13) || (CHAR(10) || '<!-- Begin Content -->'))) || ((CHAR(13) || CHAR(10)) || ('<div class="Summary">' || (CHAR(13) || CHAR(10))))) || ((('    <ad:if test="#Email#">' || CHAR(13)) || (CHAR(10) || ('    <p>' || CHAR(13)))) || ((CHAR(10) || ('    Hello #Account.DisplayName#,' || CHAR(13))) || (CHAR(10) || ('    </p>' || CHAR(13)))))) || ((((CHAR(10) || '    <p>') || (CHAR(13) || (CHAR(10) || '    Thanks for choosing as your Exchange hosting provider.'))) || ((CHAR(13) || (CHAR(10) || '    </p>')) || (CHAR(13) || (CHAR(10) || '    </ad:if>')))) || (((CHAR(13) || CHAR(10)) || ('    <ad:if test="#not(PMM)#">' || (CHAR(13) || CHAR(10)))) || (('    <h1>User Accounts</h1>' || (CHAR(13) || CHAR(10))) || ('    <p>' || (CHAR(13) || CHAR(10)))))))) || (((((('    The following user accounts have been created for you.' || CHAR(13)) || (CHAR(10) || ('    </p>' || CHAR(13)))) || ((CHAR(10) || '    <table>') || (CHAR(13) || (CHAR(10) || '        <tr>')))) || (((CHAR(13) || CHAR(10)) || ('            <td class="Label">Username:</td>' || (CHAR(13) || CHAR(10)))) || (('            <td>#Account.UserPrincipalName#</td>' || (CHAR(13) || CHAR(10))) || ('        </tr>' || (CHAR(13) || CHAR(10)))))) || (((('        <tr>' || CHAR(13)) || (CHAR(10) || ('            <td class="Label">E-mail:</td>' || CHAR(13)))) || ((CHAR(10) || ('            <td>#Account.PrimaryEmailAddress#</td>' || CHAR(13))) || (CHAR(10) || ('        </tr>' || CHAR(13))))) || (((CHAR(10) || '		<ad:if test="#PswResetUrl#">') || (CHAR(13) || (CHAR(10) || '        <tr>'))) || ((CHAR(13) || (CHAR(10) || '            <td class="Label">Password Reset Url:</td>')) || (CHAR(13) || (CHAR(10) || '            <td><a href="#PswResetUrl#" target="_blank">Click here</a></td>')))))) || (((((CHAR(13) || CHAR(10)) || ('        </tr>' || (CHAR(13) || CHAR(10)))) || (('		</ad:if>' || CHAR(13)) || (CHAR(10) || ('    </table>' || CHAR(13))))) || (((CHAR(10) || '    </ad:if>') || (CHAR(13) || (CHAR(10) || '    <h1>DNS</h1>'))) || ((CHAR(13) || (CHAR(10) || '    <p>')) || (CHAR(13) || (CHAR(10) || '    In order for us to accept mail for your domain, you will need to point your MX records to:'))))) || ((((CHAR(13) || CHAR(10)) || ('    </p>' || (CHAR(13) || CHAR(10)))) || (('    <table>' || (CHAR(13) || CHAR(10))) || ('        <ad:foreach collection="#SmtpServers#" var="SmtpServer" index="i">' || (CHAR(13) || CHAR(10))))) || ((('            <tr>' || CHAR(13)) || (CHAR(10) || ('                <td class="Label">#SmtpServer#</td>' || CHAR(13)))) || ((CHAR(10) || ('            </tr>' || CHAR(13))) || (CHAR(10) || ('        </ad:foreach>' || CHAR(13)))))))))) || ((((((((CHAR(10) || '    </table>') || (CHAR(13) || (CHAR(10) || '   <h1>'))) || ((CHAR(13) || CHAR(10)) || ('    Webmail (OWA, Outlook Web Access)</h1>' || (CHAR(13) || CHAR(10))))) || ((('    <p>' || CHAR(13)) || (CHAR(10) || ('    <a href="" target="_blank"></a>' || CHAR(13)))) || ((CHAR(10) || ('    </p>' || CHAR(13))) || (CHAR(10) || ('    <h1>' || CHAR(13)))))) || ((((CHAR(10) || '    Outlook (Windows Clients)</h1>') || (CHAR(13) || (CHAR(10) || '    <p>'))) || ((CHAR(13) || (CHAR(10) || '    To configure MS Outlook to work with the servers, please reference:')) || (CHAR(13) || (CHAR(10) || '    </p>')))) || (((CHAR(13) || CHAR(10)) || ('    <p>' || (CHAR(13) || CHAR(10)))) || (('    <a href="" target="_blank"></a>' || (CHAR(13) || CHAR(10))) || ('    </p>' || (CHAR(13) || CHAR(10))))))) || ((((('    <p>' || CHAR(13)) || (CHAR(10) || ('    If you need to download and install the Outlook client:</p>' || CHAR(13)))) || ((CHAR(10) || '        ') || (CHAR(13) || (CHAR(10) || '        <table>')))) || (((CHAR(13) || CHAR(10)) || ('            <tr><td colspan="2" class="Label"><font size="3">MS Outlook Client</font></td></tr>' || (CHAR(13) || CHAR(10)))) || (('            <tr>' || (CHAR(13) || CHAR(10))) || ('                <td class="Label">' || (CHAR(13) || CHAR(10)))))) || (((('                    Download URL:</td>' || CHAR(13)) || (CHAR(10) || ('                <td><a href=""></a></td>' || CHAR(13)))) || ((CHAR(10) || ('            </tr>' || CHAR(13))) || (CHAR(10) || ('<tr>' || CHAR(13))))) || (((CHAR(10) || '                <td class="Label"></td>') || (CHAR(13) || (CHAR(10) || '                <td><a href=""></a></td>'))) || ((CHAR(13) || (CHAR(10) || '            </tr>')) || (CHAR(13) || (CHAR(10) || '            <tr>'))))))) || ((((((CHAR(13) || CHAR(10)) || ('                <td class="Label">' || (CHAR(13) || CHAR(10)))) || (('                    KEY:</td>' || CHAR(13)) || (CHAR(10) || ('                <td></td>' || CHAR(13))))) || (((CHAR(10) || '            </tr>') || (CHAR(13) || (CHAR(10) || '        </table>'))) || ((CHAR(13) || (CHAR(10) || ' ')) || (CHAR(13) || (CHAR(10) || '       <h1>'))))) || ((((CHAR(13) || CHAR(10)) || ('    ActiveSync, iPhone, iPad</h1>' || (CHAR(13) || CHAR(10)))) || (('    <table>' || (CHAR(13) || CHAR(10))) || ('        <tr>' || (CHAR(13) || CHAR(10))))) || ((('            <td class="Label">Server:</td>' || CHAR(13)) || (CHAR(10) || ('            <td>#ActiveSyncServer#</td>' || CHAR(13)))) || ((CHAR(10) || ('        </tr>' || CHAR(13))) || (CHAR(10) || ('        <tr>' || CHAR(13))))))) || (((((CHAR(10) || '            <td class="Label">Domain:</td>') || (CHAR(13) || (CHAR(10) || '            <td>#SamDomain#</td>'))) || ((CHAR(13) || CHAR(10)) || ('        </tr>' || (CHAR(13) || CHAR(10))))) || ((('        <tr>' || CHAR(13)) || (CHAR(10) || ('            <td class="Label">SSL:</td>' || CHAR(13)))) || ((CHAR(10) || ('            <td>must be checked</td>' || CHAR(13))) || (CHAR(10) || ('        </tr>' || CHAR(13)))))) || ((((CHAR(10) || '        <tr>') || (CHAR(13) || (CHAR(10) || '            <td class="Label">Your username:</td>'))) || ((CHAR(13) || (CHAR(10) || '            <td>#SamUsername#</td>')) || (CHAR(13) || (CHAR(10) || '        </tr>')))) || (((CHAR(13) || CHAR(10)) || ('    </table>' || (CHAR(13) || CHAR(10)))) || ((' ' || (CHAR(13) || CHAR(10))) || ('    <h1>Password Changes</h1>' || (CHAR(13) || CHAR(10))))))))) || ((((((('    <p>' || CHAR(13)) || (CHAR(10) || ('    Passwords can be changed at any time using Webmail or the <a href="" target="_blank">Control Panel</a>.</p>' || CHAR(13)))) || ((CHAR(10) || '    <h1>Control Panel</h1>') || (CHAR(13) || (CHAR(10) || '    <p>')))) || (((CHAR(13) || CHAR(10)) || ('    If you need to change the details of your account, you can easily do this using <a href="" target="_blank">Control Panel</a>.</p>' || (CHAR(13) || CHAR(10)))) || (('    <h1>Support</h1>' || (CHAR(13) || CHAR(10))) || ('    <p>' || (CHAR(13) || CHAR(10)))))) || (((('    You have 2 options, email <a href="mailto:"></a> or use the web interface at <a href=""></a></p>' || CHAR(13)) || (CHAR(10) || ('    ' || CHAR(13)))) || ((CHAR(10) || ('</div>' || CHAR(13))) || (CHAR(10) || ('<!-- End Content -->' || CHAR(13))))) || (((CHAR(10) || '<br></td>') || (CHAR(13) || (CHAR(10) || '</tr>'))) || ((CHAR(13) || (CHAR(10) || '</tbody>')) || (CHAR(13) || (CHAR(10) || '</table>')))))) || (((((CHAR(13) || CHAR(10)) || ('</td>' || (CHAR(13) || CHAR(10)))) || (('</tr>' || CHAR(13)) || (CHAR(10) || ('</tbody>' || CHAR(13))))) || (((CHAR(10) || '</table>') || (CHAR(13) || (CHAR(10) || '</td>'))) || ((CHAR(13) || (CHAR(10) || '</tr>')) || (CHAR(13) || (CHAR(10) || '<tr>'))))) || ((((CHAR(13) || CHAR(10)) || ('<td style="background-color: ##ffffff; border-top: 1px solid ##999999;">' || (CHAR(13) || CHAR(10)))) || (('<table border="0" cellspacing="0" cellpadding="0" width="100%">' || (CHAR(13) || CHAR(10))) || ('<tbody>' || (CHAR(13) || CHAR(10))))) || ((('<tr>' || CHAR(13)) || (CHAR(10) || ('<td style="vertical-align: top; padding: 0px 20px 15px 20px;">' || CHAR(13)))) || ((CHAR(10) || ('<table border="0" cellspacing="0" cellpadding="0" width="100%">' || CHAR(13))) || (CHAR(10) || ('<tbody>' || CHAR(13)))))))) || ((((((CHAR(10) || '<tr>') || (CHAR(13) || (CHAR(10) || '<td style="font-family: Arial, Helvetica, sans-serif; text-align: left; font-size: 9px; color: ##717073; padding: 20px 0px 0px 0px;">'))) || ((CHAR(13) || CHAR(10)) || ('<table border="0" cellspacing="0" cellpadding="0" width="100%">' || (CHAR(13) || CHAR(10))))) || ((('<tbody>' || CHAR(13)) || (CHAR(10) || ('<tr>' || CHAR(13)))) || ((CHAR(10) || ('<td style="font-family: Arial, Helvetica, sans-serif; font-size: 9px; text-align: left; color: ##1666af; vertical-align: top;" width="33%"><a style="font-weight: bold; text-transform: uppercase; text-decoration: underline; color: ##1666af;" href=""></a><br />Learn more about the services can provide to improve your business.</td>' || CHAR(13))) || (CHAR(10) || ('<td style="font-family: Arial, Helvetica, sans-serif; font-size: 9px; text-align: left; color: ##1666af; padding: 0px 10px 0px 10px; vertical-align: top;" width="34%"><a style="font-weight: bold; text-transform: uppercase; text-decoration: underline; color: ##1666af;" href="">Privacy Policy</a><br /> follows strict guidelines in protecting your privacy. Learn about our <a style="font-weight: bold; text-decoration: underline; color: ##1666af;" href="">Privacy Policy</a>.</td>' || CHAR(13)))))) || ((((CHAR(10) || '<td style="font-family: Arial, Helvetica, sans-serif; font-size: 9px; text-align: left; color: ##1666af; vertical-align: top;" width="33%"><a style="font-weight: bold; text-transform: uppercase; text-decoration: underline; color: ##1666af;" href="">Contact Us</a><br />Questions? For more information, <a style="font-weight: bold; text-decoration: underline; color: ##1666af;" href="">contact us</a>.</td>') || (CHAR(13) || (CHAR(10) || '</tr>'))) || ((CHAR(13) || (CHAR(10) || '</tbody>')) || (CHAR(13) || (CHAR(10) || '</table>')))) || (((CHAR(13) || CHAR(10)) || ('</td>' || (CHAR(13) || CHAR(10)))) || (('</tr>' || (CHAR(13) || CHAR(10))) || ('</tbody>' || (CHAR(13) || CHAR(10))))))) || ((((('</table>' || CHAR(13)) || (CHAR(10) || ('</td>' || CHAR(13)))) || ((CHAR(10) || '</tr>') || (CHAR(13) || (CHAR(10) || '</tbody>')))) || (((CHAR(13) || CHAR(10)) || ('</table>' || (CHAR(13) || CHAR(10)))) || (('</td>' || (CHAR(13) || CHAR(10))) || ('</tr>' || (CHAR(13) || CHAR(10)))))) || (((('</tbody>' || CHAR(13)) || (CHAR(10) || ('</table>' || CHAR(13)))) || ((CHAR(10) || ('</td>' || CHAR(13))) || (CHAR(10) || ('</tr>' || CHAR(13))))) || (((CHAR(10) || '</tbody>') || (CHAR(13) || (CHAR(10) || '</table>'))) || ((CHAR(13) || (CHAR(10) || '</body>')) || (CHAR(13) || (CHAR(10) || '</html>')))))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'ExchangeMailboxSetupLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'ExchangeMailboxSetupLetter', 1, ' Hosted Exchange Mailbox Setup');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'ExchangeMailboxSetupLetter', 1, ((((((('<ad:if test="#Email#">' || CHAR(13)) || (CHAR(10) || ('Hello #Account.DisplayName#,' || CHAR(13)))) || ((CHAR(10) || (CHAR(13) || CHAR(10))) || ('Thanks for choosing as your Exchange hosting provider.' || (CHAR(13) || CHAR(10))))) || ((('</ad:if>' || (CHAR(13) || CHAR(10))) || ('<ad:if test="#not(PMM)#">' || (CHAR(13) || CHAR(10)))) || (('User Accounts' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || 'The following user accounts have been created for you.'))))) || ((((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || ('Username: #Account.UserPrincipalName#' || CHAR(13)))) || ((CHAR(10) || ('E-mail: #Account.PrimaryEmailAddress#' || CHAR(13))) || (CHAR(10) || ('<ad:if test="#PswResetUrl#">' || CHAR(13))))) || (((CHAR(10) || ('Password Reset Url: #PswResetUrl#' || CHAR(13))) || (CHAR(10) || ('</ad:if>' || CHAR(13)))) || ((CHAR(10) || ('</ad:if>' || CHAR(13))) || (CHAR(10) || (CHAR(13) || CHAR(10))))))) || ((((('=================================' || (CHAR(13) || CHAR(10))) || ('DNS' || (CHAR(13) || CHAR(10)))) || (('=================================' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || 'In order for us to accept mail for your domain, you will need to point your MX records to:')))) || (((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || ('<ad:foreach collection="#SmtpServers#" var="SmtpServer" index="i">#SmtpServer#</ad:foreach>' || CHAR(13)))) || ((CHAR(10) || (CHAR(13) || CHAR(10))) || ('=================================' || (CHAR(13) || CHAR(10)))))) || (((('Webmail (OWA, Outlook Web Access)' || (CHAR(13) || CHAR(10))) || ('=================================' || (CHAR(13) || CHAR(10)))) || ((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || (CHAR(13) || CHAR(10))))) || ((('=================================' || (CHAR(13) || CHAR(10))) || ('Outlook (Windows Clients)' || (CHAR(13) || CHAR(10)))) || (('=================================' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || 'To configure MS Outlook to work with servers, please reference:'))))))) || ((((((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || (CHAR(13) || CHAR(10)))) || ((CHAR(13) || (CHAR(10) || 'If you need to download and install the MS Outlook client:')) || (CHAR(13) || (CHAR(10) || CHAR(13))))) || (((CHAR(10) || ('MS Outlook Download URL:' || CHAR(13))) || (CHAR(10) || (CHAR(13) || CHAR(10)))) || (('KEY: ' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || '================================='))))) || ((((CHAR(13) || (CHAR(10) || 'ActiveSync, iPhone, iPad')) || (CHAR(13) || (CHAR(10) || '================================='))) || ((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || ('Server: #ActiveSyncServer#' || CHAR(13))))) || (((CHAR(10) || ('Domain: #SamDomain#' || CHAR(13))) || (CHAR(10) || ('SSL: must be checked' || CHAR(13)))) || ((CHAR(10) || ('Your username: #SamUsername#' || CHAR(13))) || (CHAR(10) || (CHAR(13) || CHAR(10))))))) || ((((('=================================' || (CHAR(13) || CHAR(10))) || ('Password Changes' || (CHAR(13) || CHAR(10)))) || (('=================================' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || 'Passwords can be changed at any time using Webmail or the Control Panel')))) || (((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || (CHAR(13) || CHAR(10)))) || (('=================================' || (CHAR(13) || CHAR(10))) || ('Control Panel' || (CHAR(13) || CHAR(10)))))) || (((('=================================' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || 'If you need to change the details of your account, you can easily do this using the Control Panel '))) || ((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || (CHAR(13) || CHAR(10))))) || ((('=================================' || (CHAR(13) || CHAR(10))) || ('Support' || (CHAR(13) || CHAR(10)))) || (('=================================' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || 'You have 2 options, email or use the web interface at ')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('MailboxPasswordPolicy', 'ExchangePolicy', 1, 'True;8;20;0;2;0;True');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('UserNamePolicy', 'FtpPolicy', 1, 'True;-;1;20;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('UserPasswordPolicy', 'FtpPolicy', 1, 'True;5;20;0;1;0;True');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('AccountNamePolicy', 'MailPolicy', 1, 'True;;1;50;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('AccountPasswordPolicy', 'MailPolicy', 1, 'True;5;20;0;1;0;False;;0;;;False;False;0;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('CatchAllName', 'MailPolicy', 1, 'mail');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('DatabaseNamePolicy', 'MariaDBPolicy', 1, 'True;;1;40;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('UserNamePolicy', 'MariaDBPolicy', 1, 'True;;1;16;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('UserPasswordPolicy', 'MariaDBPolicy', 1, 'True;5;20;0;1;0;False;;0;;;False;False;0;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('DatabaseNamePolicy', 'MsSqlPolicy', 1, 'True;-;1;120;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('UserNamePolicy', 'MsSqlPolicy', 1, 'True;-;1;120;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('UserPasswordPolicy', 'MsSqlPolicy', 1, 'True;5;20;0;1;0;True;;0;0;0;False;False;0;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('DatabaseNamePolicy', 'MySqlPolicy', 1, 'True;;1;40;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('UserNamePolicy', 'MySqlPolicy', 1, 'True;;1;16;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('UserPasswordPolicy', 'MySqlPolicy', 1, 'True;5;20;0;1;0;False;;0;0;0;False;False;0;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'OrganizationUserPasswordRequestLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'OrganizationUserPasswordRequestLetter', 1, ((((((('<html xmlns="http://www.w3.org/1999/xhtml">' || CHAR(13)) || (CHAR(10) || '<head>')) || ((CHAR(13) || CHAR(10)) || ('    <title>Password request notification</title>' || CHAR(13)))) || (((CHAR(10) || '    <style type="text/css">') || (CHAR(13) || CHAR(10))) || (('		.Summary { background-color: ##ffffff; padding: 5px; }' || CHAR(13)) || (CHAR(10) || '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }')))) || ((((CHAR(13) || CHAR(10)) || ('        .Summary A { color: ##0153A4; }' || CHAR(13))) || ((CHAR(10) || '        .Summary { font-family: Tahoma; font-size: 9pt; }') || (CHAR(13) || CHAR(10)))) || ((('        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }' || CHAR(13)) || (CHAR(10) || '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ')) || ((CHAR(13) || CHAR(10)) || ('        .Summary TABLE { border: solid 1px ##e5e5e5; }' || CHAR(13)))))) || (((((CHAR(10) || '        .Summary TH,') || (CHAR(13) || CHAR(10))) || (('        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }' || CHAR(13)) || (CHAR(10) || '        .Summary TD { padding: 8px; font-size: 9pt; }'))) || (((CHAR(13) || CHAR(10)) || ('        .Summary UL LI { font-size: 1.1em; font-weight: bold; }' || CHAR(13))) || ((CHAR(10) || '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }') || (CHAR(13) || CHAR(10))))) || (((('    </style>' || CHAR(13)) || (CHAR(10) || '</head>')) || ((CHAR(13) || CHAR(10)) || ('<body>' || CHAR(13)))) || (((CHAR(10) || '<div class="Summary">') || (CHAR(13) || CHAR(10))) || (('<div class="Header">' || CHAR(13)) || (CHAR(10) || ('<img src="#logoUrl#">' || CHAR(13)))))))) || ((((((CHAR(10) || '</div>') || (CHAR(13) || CHAR(10))) || (('<h1>Password request notification</h1>' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || '<ad:if test="#user#">') || (CHAR(13) || CHAR(10))) || (('<p>' || CHAR(13)) || (CHAR(10) || 'Hello #user.FirstName#,')))) || ((((CHAR(13) || CHAR(10)) || ('</p>' || CHAR(13))) || ((CHAR(10) || '</ad:if>') || (CHAR(13) || CHAR(10)))) || (((CHAR(13) || CHAR(10)) || ('<p>' || CHAR(13))) || ((CHAR(10) || 'Your account have been created. In order to create a password for your account, please follow next link:') || (CHAR(13) || (CHAR(10) || '</p>')))))) || (((((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))) || (('<a href="#passwordResetLink#" target="_blank">#passwordResetLink#</a>' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || '<p>') || (CHAR(13) || CHAR(10))) || (('If you have any questions regarding your hosting account, feel free to contact our support department at any time.' || CHAR(13)) || (CHAR(10) || '</p>')))) || ((((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))) || (('<p>' || CHAR(13)) || (CHAR(10) || 'Best regards'))) || (((CHAR(13) || CHAR(10)) || ('</p>' || CHAR(13))) || ((CHAR(10) || '</div>') || (CHAR(13) || (CHAR(10) || '</body>')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('LogoUrl', 'OrganizationUserPasswordRequestLetter', 1, '');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'OrganizationUserPasswordRequestLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('SMSBody', 'OrganizationUserPasswordRequestLetter', 1, ((CHAR(13) || (CHAR(10) || 'User have been created. Password request url:')) || (CHAR(13) || (CHAR(10) || '# passwordResetLink#'))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'OrganizationUserPasswordRequestLetter', 1, 'Password request notification');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'OrganizationUserPasswordRequestLetter', 1, ((((('=========================================' || CHAR(13)) || (CHAR(10) || '   Password request notification')) || ((CHAR(13) || CHAR(10)) || ('=========================================' || (CHAR(13) || CHAR(10))))) || (((CHAR(13) || CHAR(10)) || ('<ad:if test="#user#">' || (CHAR(13) || CHAR(10)))) || (('Hello #user.FirstName#,' || CHAR(13)) || (CHAR(10) || ('</ad:if>' || CHAR(13)))))) || ((((CHAR(10) || CHAR(13)) || (CHAR(10) || 'Your account have been created. In order to create a password for your account, please follow next link:')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || '# passwordResetLink#')))) || (((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.'))) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || 'Best regards')))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('DsnNamePolicy', 'OsPolicy', 1, 'True;-;2;40;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('CC', 'PackageSummaryLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('EnableLetter', 'PackageSummaryLetter', 1, 'True');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'PackageSummaryLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'PackageSummaryLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'PackageSummaryLetter', 1, '"#space.Package.PackageName#" <ad:if test="#Signup#">hosting space has been created for<ad:else>hosting space summary for</ad:if> #user.FirstName# #user.LastName#');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('CC', 'PasswordReminderLetter', 1, '');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'PasswordReminderLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'PasswordReminderLetter', 1, ((((((('<html xmlns="http://www.w3.org/1999/xhtml">' || CHAR(13)) || (CHAR(10) || ('<head>' || CHAR(13)))) || ((CHAR(10) || ('    <title>Account Summary Information</title>' || CHAR(13))) || (CHAR(10) || ('    <style type="text/css">' || CHAR(13))))) || (((CHAR(10) || ('		.Summary { background-color: ##ffffff; padding: 5px; }' || CHAR(13))) || (CHAR(10) || ('		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }' || CHAR(13)))) || ((CHAR(10) || ('        .Summary A { color: ##0153A4; }' || CHAR(13))) || (CHAR(10) || ('        .Summary { font-family: Tahoma; font-size: 9pt; }' || CHAR(13)))))) || ((((CHAR(10) || ('        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }' || CHAR(13))) || (CHAR(10) || ('        .Summary H2 { font-size: 1.3em; color: ##1F4978; }' || CHAR(13)))) || ((CHAR(10) || ('        .Summary TABLE { border: solid 1px ##e5e5e5; }' || CHAR(13))) || (CHAR(10) || ('        .Summary TH,' || CHAR(13))))) || (((CHAR(10) || ('        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }' || CHAR(13))) || (CHAR(10) || ('        .Summary TD { padding: 8px; font-size: 9pt; }' || CHAR(13)))) || ((CHAR(10) || ('        .Summary UL LI { font-size: 1.1em; font-weight: bold; }' || CHAR(13))) || (CHAR(10) || ('        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }' || CHAR(13))))))) || (((((CHAR(10) || ('    </style>' || CHAR(13))) || (CHAR(10) || ('</head>' || CHAR(13)))) || ((CHAR(10) || ('<body>' || CHAR(13))) || (CHAR(10) || ('<div class="Summary">' || CHAR(13))))) || (((CHAR(10) || (CHAR(13) || CHAR(10))) || ('<a name="top"></a>' || (CHAR(13) || CHAR(10)))) || (('<div class="Header">' || (CHAR(13) || CHAR(10))) || ('	Hosting Account Information' || (CHAR(13) || CHAR(10)))))) || (((('</div>' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || '<p>'))) || ((CHAR(13) || (CHAR(10) || 'Hello #user.FirstName#,')) || (CHAR(13) || (CHAR(10) || '</p>')))) || (((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || ('<p>' || CHAR(13)))) || ((CHAR(10) || ('Please, find below details of your control panel account. The one time password was generated for you. You should change the password after login. ' || CHAR(13))) || (CHAR(10) || ('</p>' || CHAR(13)))))))) || ((((((CHAR(10) || (CHAR(13) || CHAR(10))) || ('<h1>Control Panel URL</h1>' || (CHAR(13) || CHAR(10)))) || (('<table>' || (CHAR(13) || CHAR(10))) || ('    <thead>' || (CHAR(13) || CHAR(10))))) || ((('        <tr>' || (CHAR(13) || CHAR(10))) || ('            <th>Control Panel URL</th>' || (CHAR(13) || CHAR(10)))) || (('            <th>Username</th>' || (CHAR(13) || CHAR(10))) || ('            <th>One Time Password</th>' || (CHAR(13) || CHAR(10)))))) || (((('        </tr>' || (CHAR(13) || CHAR(10))) || ('    </thead>' || (CHAR(13) || CHAR(10)))) || (('    <tbody>' || (CHAR(13) || CHAR(10))) || ('        <tr>' || (CHAR(13) || CHAR(10))))) || ((('            <td><a href="http://panel.HostingCompany.com">http://panel.HostingCompany.com</a></td>' || (CHAR(13) || CHAR(10))) || ('            <td>#user.Username#</td>' || (CHAR(13) || CHAR(10)))) || (('            <td>#user.Password#</td>' || (CHAR(13) || CHAR(10))) || ('        </tr>' || (CHAR(13) || CHAR(10))))))) || ((((('    </tbody>' || (CHAR(13) || CHAR(10))) || ('</table>' || (CHAR(13) || CHAR(10)))) || ((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || ('<p>' || CHAR(13))))) || (((CHAR(10) || ('If you have any questions regarding your hosting account, feel free to contact our support department at any time.' || CHAR(13))) || (CHAR(10) || ('</p>' || CHAR(13)))) || ((CHAR(10) || (CHAR(13) || CHAR(10))) || ('<p>' || (CHAR(13) || CHAR(10)))))) || (((('Best regards,<br />' || (CHAR(13) || CHAR(10))) || ('FuseCP.<br />' || (CHAR(13) || CHAR(10)))) || (('Web Site: <a href="https://fusecp.com">https://fusecp.com</a><br />' || (CHAR(13) || CHAR(10))) || ('E-Mail: <a href="mailto:support@fusecp.com">support@fusecp.com</a>' || (CHAR(13) || CHAR(10))))) || ((('</p>' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || '</div>'))) || ((CHAR(13) || (CHAR(10) || '</body>')) || (CHAR(13) || (CHAR(10) || '</html>')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'PasswordReminderLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'PasswordReminderLetter', 1, 'Password reminder for #user.FirstName# #user.LastName#');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'PasswordReminderLetter', 1, ((((('=================================' || CHAR(13)) || (CHAR(10) || ('   Hosting Account Information' || CHAR(13)))) || ((CHAR(10) || ('=================================' || CHAR(13))) || (CHAR(10) || (CHAR(13) || CHAR(10))))) || ((('Hello #user.FirstName#,' || (CHAR(13) || CHAR(10))) || (CHAR(13) || (CHAR(10) || 'Please, find below details of your control panel account. The one time password was generated for you. You should change the password after login.'))) || ((CHAR(13) || (CHAR(10) || CHAR(13))) || (CHAR(10) || ('Control Panel URL: https://panel.fusecp.com' || CHAR(13)))))) || ((((CHAR(10) || ('Username: #user.Username#' || CHAR(13))) || (CHAR(10) || ('One Time Password: #user.Password#' || CHAR(13)))) || ((CHAR(10) || (CHAR(13) || CHAR(10))) || ('If you have any questions regarding your hosting account, feel free to contact our support department at any time.' || (CHAR(13) || CHAR(10))))) || (((CHAR(13) || (CHAR(10) || 'Best regards,')) || (CHAR(13) || (CHAR(10) || 'FuseCP.'))) || ((CHAR(13) || (CHAR(10) || 'Web Site: https://fusecp.com"')) || (CHAR(13) || (CHAR(10) || 'E-Mail: support@fusecp.com')))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('CC', 'RDSSetupLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'RDSSetupLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'RDSSetupLetter', 1, (((((('<html xmlns="http://www.w3.org/1999/xhtml">' || CHAR(13)) || (CHAR(10) || '<head>')) || ((CHAR(13) || CHAR(10)) || ('    <title>RDS Setup Information</title>' || (CHAR(13) || CHAR(10))))) || ((('    <style type="text/css">' || CHAR(13)) || (CHAR(10) || ('		.Summary { background-color: ##ffffff; padding: 5px; }' || CHAR(13)))) || ((CHAR(10) || '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }') || (CHAR(13) || (CHAR(10) || '        .Summary A { color: ##0153A4; }'))))) || ((((CHAR(13) || CHAR(10)) || ('        .Summary { font-family: Tahoma; font-size: 9pt; }' || (CHAR(13) || CHAR(10)))) || (('        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }' || CHAR(13)) || (CHAR(10) || ('        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ' || CHAR(13))))) || (((CHAR(10) || '        .Summary TABLE { border: solid 1px ##e5e5e5; }') || (CHAR(13) || (CHAR(10) || '        .Summary TH,'))) || ((CHAR(13) || CHAR(10)) || ('        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }' || (CHAR(13) || CHAR(10))))))) || ((((('        .Summary TD { padding: 8px; font-size: 9pt; }' || CHAR(13)) || (CHAR(10) || '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }')) || ((CHAR(13) || CHAR(10)) || ('        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }' || (CHAR(13) || CHAR(10))))) || ((('    </style>' || CHAR(13)) || (CHAR(10) || ('</head>' || CHAR(13)))) || ((CHAR(10) || '<body>') || (CHAR(13) || (CHAR(10) || '<div class="Summary">'))))) || ((((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || '<a name="top"></a>'))) || ((CHAR(13) || CHAR(10)) || ('<div class="Header">' || (CHAR(13) || CHAR(10))))) || ((('	RDS Setup Information' || CHAR(13)) || (CHAR(10) || ('</div>' || CHAR(13)))) || ((CHAR(10) || '</div>') || (CHAR(13) || (CHAR(10) || '</body>'))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'RDSSetupLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'RDSSetupLetter', 1, 'RDS setup');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'RDSSetupLetter', 1, (((('=================================' || (CHAR(13) || CHAR(10))) || (('   RDS Setup Information' || CHAR(13)) || (CHAR(10) || '================================='))) || (((CHAR(13) || CHAR(10)) || ('<ad:if test="#user#">' || CHAR(13))) || ((CHAR(10) || 'Hello #user.FirstName#,') || (CHAR(13) || CHAR(10))))) || (((('</ad:if>' || CHAR(13)) || (CHAR(10) || CHAR(13))) || ((CHAR(10) || 'Please, find below RDS setup instructions.') || (CHAR(13) || CHAR(10)))) || (((CHAR(13) || CHAR(10)) || ('If you have any questions, feel free to contact our support department at any time.' || CHAR(13))) || ((CHAR(10) || CHAR(13)) || (CHAR(10) || 'Best regards'))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('GroupNamePolicy', 'SharePointPolicy', 1, 'True;-;1;20;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('UserNamePolicy', 'SharePointPolicy', 1, 'True;-;1;20;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('UserPasswordPolicy', 'SharePointPolicy', 1, 'True;5;20;0;1;0;True;;0;;;False;False;0;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('DemoMessage', 'FuseCPPolicy', 1, ((('When user account is in demo mode the majority of operations are' || (CHAR(13) || CHAR(10))) || ('disabled, especially those ones that modify or delete records.' || (CHAR(13) || CHAR(10)))) || (('You are welcome to ask your questions or place comments about' || (CHAR(13) || CHAR(10))) || (('this demo on  <a href="http://forum.FuseCP.net"' || CHAR(13)) || (CHAR(10) || 'target="_blank">FuseCP  Support Forum</a>')))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('ForbiddenIP', 'FuseCPPolicy', 1, '');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('PasswordPolicy', 'FuseCPPolicy', 1, 'True;6;20;0;1;0;True;;0;;;False;False;0;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'UserPasswordExpirationLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'UserPasswordExpirationLetter', 1, ((((((('<html xmlns="http://www.w3.org/1999/xhtml">' || CHAR(13)) || (CHAR(10) || '<head>')) || ((CHAR(13) || CHAR(10)) || ('    <title>Password expiration notification</title>' || CHAR(13)))) || (((CHAR(10) || '    <style type="text/css">') || (CHAR(13) || CHAR(10))) || (('		.Summary { background-color: ##ffffff; padding: 5px; }' || CHAR(13)) || (CHAR(10) || '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }')))) || ((((CHAR(13) || CHAR(10)) || ('        .Summary A { color: ##0153A4; }' || CHAR(13))) || ((CHAR(10) || '        .Summary { font-family: Tahoma; font-size: 9pt; }') || (CHAR(13) || CHAR(10)))) || ((('        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }' || CHAR(13)) || (CHAR(10) || '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ')) || ((CHAR(13) || CHAR(10)) || ('        .Summary TABLE { border: solid 1px ##e5e5e5; }' || (CHAR(13) || CHAR(10))))))) || ((((('        .Summary TH,' || CHAR(13)) || (CHAR(10) || '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }')) || ((CHAR(13) || CHAR(10)) || ('        .Summary TD { padding: 8px; font-size: 9pt; }' || CHAR(13)))) || (((CHAR(10) || '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }') || (CHAR(13) || CHAR(10))) || (('        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }' || CHAR(13)) || (CHAR(10) || '    </style>')))) || ((((CHAR(13) || CHAR(10)) || ('</head>' || CHAR(13))) || ((CHAR(10) || '<body>') || (CHAR(13) || CHAR(10)))) || ((('<div class="Summary">' || CHAR(13)) || (CHAR(10) || '<div class="Header">')) || ((CHAR(13) || CHAR(10)) || ('<img src="#logoUrl#">' || (CHAR(13) || CHAR(10)))))))) || (((((('</div>' || CHAR(13)) || (CHAR(10) || '<h1>Password expiration notification</h1>')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10)))) || ((('<ad:if test="#user#">' || CHAR(13)) || (CHAR(10) || '<p>')) || ((CHAR(13) || CHAR(10)) || ('Hello #user.FirstName#,' || CHAR(13))))) || ((((CHAR(10) || '</p>') || (CHAR(13) || CHAR(10))) || (('</ad:if>' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || '<p>') || (CHAR(13) || CHAR(10))) || (('Your password expiration date is #user.PasswordExpirationDateTime#. You can reset your own password by visiting the following page:' || CHAR(13)) || (CHAR(10) || ('</p>' || CHAR(13))))))) || (((((CHAR(10) || CHAR(13)) || (CHAR(10) || '<a href="#passwordResetLink#" target="_blank">#passwordResetLink#</a>')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10)))) || (((CHAR(13) || CHAR(10)) || ('<p>' || CHAR(13))) || ((CHAR(10) || 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.') || (CHAR(13) || (CHAR(10) || '</p>'))))) || ((((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))) || (('<p>' || CHAR(13)) || (CHAR(10) || 'Best regards'))) || (((CHAR(13) || CHAR(10)) || ('</p>' || CHAR(13))) || ((CHAR(10) || '</div>') || (CHAR(13) || (CHAR(10) || '</body>')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('LogoUrl', 'UserPasswordExpirationLetter', 1, '');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'UserPasswordExpirationLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'UserPasswordExpirationLetter', 1, 'Password expiration notification');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'UserPasswordExpirationLetter', 1, ((((('=========================================' || CHAR(13)) || (CHAR(10) || '   Password expiration notification')) || ((CHAR(13) || CHAR(10)) || ('=========================================' || (CHAR(13) || CHAR(10))))) || (((CHAR(13) || CHAR(10)) || ('<ad:if test="#user#">' || (CHAR(13) || CHAR(10)))) || (('Hello #user.FirstName#,' || CHAR(13)) || (CHAR(10) || ('</ad:if>' || CHAR(13)))))) || ((((CHAR(10) || CHAR(13)) || (CHAR(10) || 'Your password expiration date is #user.PasswordExpirationDateTime#. You can reset your own password by visiting the following page:')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || '# passwordResetLink#')))) || (((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.'))) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || 'Best regards')))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'UserPasswordResetLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'UserPasswordResetLetter', 1, ((((((('<html xmlns="http://www.w3.org/1999/xhtml">' || CHAR(13)) || (CHAR(10) || '<head>')) || ((CHAR(13) || CHAR(10)) || ('    <title>Password reset notification</title>' || CHAR(13)))) || (((CHAR(10) || '    <style type="text/css">') || (CHAR(13) || CHAR(10))) || (('		.Summary { background-color: ##ffffff; padding: 5px; }' || CHAR(13)) || (CHAR(10) || '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }')))) || ((((CHAR(13) || CHAR(10)) || ('        .Summary A { color: ##0153A4; }' || CHAR(13))) || ((CHAR(10) || '        .Summary { font-family: Tahoma; font-size: 9pt; }') || (CHAR(13) || CHAR(10)))) || ((('        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }' || CHAR(13)) || (CHAR(10) || '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ')) || ((CHAR(13) || CHAR(10)) || ('        .Summary TABLE { border: solid 1px ##e5e5e5; }' || (CHAR(13) || CHAR(10))))))) || ((((('        .Summary TH,' || CHAR(13)) || (CHAR(10) || '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }')) || ((CHAR(13) || CHAR(10)) || ('        .Summary TD { padding: 8px; font-size: 9pt; }' || CHAR(13)))) || (((CHAR(10) || '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }') || (CHAR(13) || CHAR(10))) || (('        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }' || CHAR(13)) || (CHAR(10) || '    </style>')))) || ((((CHAR(13) || CHAR(10)) || ('</head>' || CHAR(13))) || ((CHAR(10) || '<body>') || (CHAR(13) || CHAR(10)))) || ((('<div class="Summary">' || CHAR(13)) || (CHAR(10) || '<div class="Header">')) || ((CHAR(13) || CHAR(10)) || ('<img src="#logoUrl#">' || (CHAR(13) || CHAR(10)))))))) || (((((('</div>' || CHAR(13)) || (CHAR(10) || '<h1>Password reset notification</h1>')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10)))) || ((('<ad:if test="#user#">' || CHAR(13)) || (CHAR(10) || '<p>')) || ((CHAR(13) || CHAR(10)) || ('Hello #user.FirstName#,' || CHAR(13))))) || ((((CHAR(10) || '</p>') || (CHAR(13) || CHAR(10))) || (('</ad:if>' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || '<p>') || (CHAR(13) || CHAR(10))) || (('We received a request to reset the password for your account. If you made this request, click the link below. If you did not make this request, you can ignore this email.' || CHAR(13)) || (CHAR(10) || ('</p>' || CHAR(13))))))) || (((((CHAR(10) || CHAR(13)) || (CHAR(10) || '<a href="#passwordResetLink#" target="_blank">#passwordResetLink#</a>')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10)))) || (((CHAR(13) || CHAR(10)) || ('<p>' || CHAR(13))) || ((CHAR(10) || 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.') || (CHAR(13) || (CHAR(10) || '</p>'))))) || ((((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))) || (('<p>' || CHAR(13)) || (CHAR(10) || 'Best regards'))) || (((CHAR(13) || CHAR(10)) || ('</p>' || CHAR(13))) || ((CHAR(10) || '</div>') || (CHAR(13) || (CHAR(10) || '</body>')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('LogoUrl', 'UserPasswordResetLetter', 1, '');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('PasswordResetLinkSmsBody', 'UserPasswordResetLetter', 1, (('Password reset link:' || (CHAR(13) || CHAR(10))) || ('# passwordResetLink#' || (CHAR(13) || CHAR(10)))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'UserPasswordResetLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'UserPasswordResetLetter', 1, 'Password reset notification');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'UserPasswordResetLetter', 1, ((((('=========================================' || CHAR(13)) || (CHAR(10) || '   Password reset notification')) || ((CHAR(13) || CHAR(10)) || ('=========================================' || (CHAR(13) || CHAR(10))))) || (((CHAR(13) || CHAR(10)) || ('<ad:if test="#user#">' || (CHAR(13) || CHAR(10)))) || (('Hello #user.FirstName#,' || CHAR(13)) || (CHAR(10) || ('</ad:if>' || CHAR(13)))))) || ((((CHAR(10) || CHAR(13)) || (CHAR(10) || 'We received a request to reset the password for your account. If you made this request, click the link below. If you did not make this request, you can ignore this email.')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || '# passwordResetLink#')))) || (((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.'))) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || 'Best regards')))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'UserPasswordResetPincodeLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'UserPasswordResetPincodeLetter', 1, ((((((('<html xmlns="http://www.w3.org/1999/xhtml">' || CHAR(13)) || (CHAR(10) || '<head>')) || ((CHAR(13) || CHAR(10)) || ('    <title>Password reset notification</title>' || CHAR(13)))) || (((CHAR(10) || '    <style type="text/css">') || (CHAR(13) || CHAR(10))) || (('		.Summary { background-color: ##ffffff; padding: 5px; }' || CHAR(13)) || (CHAR(10) || '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }')))) || ((((CHAR(13) || CHAR(10)) || ('        .Summary A { color: ##0153A4; }' || CHAR(13))) || ((CHAR(10) || '        .Summary { font-family: Tahoma; font-size: 9pt; }') || (CHAR(13) || CHAR(10)))) || ((('        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }' || CHAR(13)) || (CHAR(10) || '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ')) || ((CHAR(13) || CHAR(10)) || ('        .Summary TABLE { border: solid 1px ##e5e5e5; }' || CHAR(13)))))) || (((((CHAR(10) || '        .Summary TH,') || (CHAR(13) || CHAR(10))) || (('        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }' || CHAR(13)) || (CHAR(10) || '        .Summary TD { padding: 8px; font-size: 9pt; }'))) || (((CHAR(13) || CHAR(10)) || ('        .Summary UL LI { font-size: 1.1em; font-weight: bold; }' || CHAR(13))) || ((CHAR(10) || '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }') || (CHAR(13) || CHAR(10))))) || (((('    </style>' || CHAR(13)) || (CHAR(10) || '</head>')) || ((CHAR(13) || CHAR(10)) || ('<body>' || CHAR(13)))) || (((CHAR(10) || '<div class="Summary">') || (CHAR(13) || CHAR(10))) || (('<div class="Header">' || CHAR(13)) || (CHAR(10) || ('<img src="#logoUrl#">' || CHAR(13)))))))) || ((((((CHAR(10) || '</div>') || (CHAR(13) || CHAR(10))) || (('<h1>Password reset notification</h1>' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || '<ad:if test="#user#">') || (CHAR(13) || CHAR(10))) || (('<p>' || CHAR(13)) || (CHAR(10) || 'Hello #user.FirstName#,')))) || ((((CHAR(13) || CHAR(10)) || ('</p>' || CHAR(13))) || ((CHAR(10) || '</ad:if>') || (CHAR(13) || CHAR(10)))) || (((CHAR(13) || CHAR(10)) || ('<p>' || CHAR(13))) || ((CHAR(10) || 'We received a request to reset the password for your account. Your password reset pincode:') || (CHAR(13) || (CHAR(10) || '</p>')))))) || (((((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))) || (('# passwordResetPincode#' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || '<p>') || (CHAR(13) || CHAR(10))) || (('If you have any questions regarding your hosting account, feel free to contact our support department at any time.' || CHAR(13)) || (CHAR(10) || '</p>')))) || ((((CHAR(13) || CHAR(10)) || (CHAR(13) || CHAR(10))) || (('<p>' || CHAR(13)) || (CHAR(10) || 'Best regards'))) || (((CHAR(13) || CHAR(10)) || ('</p>' || CHAR(13))) || ((CHAR(10) || '</div>') || (CHAR(13) || (CHAR(10) || '</body>')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('LogoUrl', 'UserPasswordResetPincodeLetter', 1, '');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('PasswordResetPincodeSmsBody', 'UserPasswordResetPincodeLetter', 1, ((CHAR(13) || (CHAR(10) || 'Your password reset pincode:')) || (CHAR(13) || (CHAR(10) || '# passwordResetPincode#'))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'UserPasswordResetPincodeLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'UserPasswordResetPincodeLetter', 1, 'Password reset notification');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'UserPasswordResetPincodeLetter', 1, ((((('=========================================' || CHAR(13)) || (CHAR(10) || '   Password reset notification')) || ((CHAR(13) || CHAR(10)) || ('=========================================' || (CHAR(13) || CHAR(10))))) || (((CHAR(13) || CHAR(10)) || ('<ad:if test="#user#">' || (CHAR(13) || CHAR(10)))) || (('Hello #user.FirstName#,' || CHAR(13)) || (CHAR(10) || ('</ad:if>' || CHAR(13)))))) || ((((CHAR(10) || CHAR(13)) || (CHAR(10) || 'We received a request to reset the password for your account. Your password reset pincode:')) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || '# passwordResetPincode#')))) || (((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.'))) || ((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || 'Best regards')))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('CC', 'VerificationCodeLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('From', 'VerificationCodeLetter', 1, 'support@HostingCompany.com');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('HtmlBody', 'VerificationCodeLetter', 1, ((((((('<html xmlns="http://www.w3.org/1999/xhtml">' || CHAR(13)) || (CHAR(10) || '<head>')) || ((CHAR(13) || CHAR(10)) || ('    <title>Verification code</title>' || (CHAR(13) || CHAR(10))))) || ((('    <style type="text/css">' || CHAR(13)) || (CHAR(10) || ('		.Summary { background-color: ##ffffff; padding: 5px; }' || CHAR(13)))) || ((CHAR(10) || '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }') || (CHAR(13) || (CHAR(10) || '        .Summary A { color: ##0153A4; }'))))) || ((((CHAR(13) || CHAR(10)) || ('        .Summary { font-family: Tahoma; font-size: 9pt; }' || (CHAR(13) || CHAR(10)))) || (('        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }' || CHAR(13)) || (CHAR(10) || ('        .Summary H2 { font-size: 1.3em; color: ##1F4978; }' || CHAR(13))))) || (((CHAR(10) || '        .Summary TABLE { border: solid 1px ##e5e5e5; }') || (CHAR(13) || (CHAR(10) || '        .Summary TH,'))) || ((CHAR(13) || CHAR(10)) || ('        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }' || (CHAR(13) || CHAR(10))))))) || ((((('        .Summary TD { padding: 8px; font-size: 9pt; }' || CHAR(13)) || (CHAR(10) || '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }')) || ((CHAR(13) || CHAR(10)) || ('        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }' || (CHAR(13) || CHAR(10))))) || ((('    </style>' || CHAR(13)) || (CHAR(10) || ('</head>' || CHAR(13)))) || ((CHAR(10) || '<body>') || (CHAR(13) || (CHAR(10) || '<div class="Summary">'))))) || ((((CHAR(13) || CHAR(10)) || (CHAR(13) || (CHAR(10) || '<a name="top"></a>'))) || ((CHAR(13) || CHAR(10)) || ('<div class="Header">' || (CHAR(13) || CHAR(10))))) || ((('	Verification code' || CHAR(13)) || (CHAR(10) || ('</div>' || CHAR(13)))) || ((CHAR(10) || CHAR(13)) || (CHAR(10) || ('<p>' || CHAR(13)))))))) || ((((((CHAR(10) || 'Hello #user.FirstName#,') || (CHAR(13) || CHAR(10))) || (('</p>' || CHAR(13)) || (CHAR(10) || (CHAR(13) || CHAR(10))))) || ((('<p>' || CHAR(13)) || (CHAR(10) || ('to complete the sign in, enter the verification code on the device. ' || CHAR(13)))) || ((CHAR(10) || '</p>') || (CHAR(13) || (CHAR(10) || CHAR(13)))))) || ((((CHAR(10) || '<table>') || (CHAR(13) || (CHAR(10) || '    <thead>'))) || ((CHAR(13) || CHAR(10)) || ('        <tr>' || (CHAR(13) || CHAR(10))))) || ((('            <th>Verification code</th>' || CHAR(13)) || (CHAR(10) || ('        </tr>' || CHAR(13)))) || ((CHAR(10) || '    </thead>') || (CHAR(13) || (CHAR(10) || '    <tbody>')))))) || (((((CHAR(13) || CHAR(10)) || ('        <tr>' || CHAR(13))) || ((CHAR(10) || '            <td>#verificationCode#</td>') || (CHAR(13) || (CHAR(10) || '        </tr>')))) || (((CHAR(13) || CHAR(10)) || ('    </tbody>' || (CHAR(13) || CHAR(10)))) || (('</table>' || CHAR(13)) || (CHAR(10) || (CHAR(13) || CHAR(10)))))) || (((('<p>' || CHAR(13)) || (CHAR(10) || ('Best regards,<br />' || CHAR(13)))) || ((CHAR(10) || CHAR(13)) || (CHAR(10) || ('</p>' || CHAR(13))))) || (((CHAR(10) || CHAR(13)) || (CHAR(10) || ('</div>' || CHAR(13)))) || ((CHAR(10) || '</body>') || (CHAR(13) || (CHAR(10) || '</html>')))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Priority', 'VerificationCodeLetter', 1, 'Normal');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('Subject', 'VerificationCodeLetter', 1, 'Verification code');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('TextBody', 'VerificationCodeLetter', 1, ((((('=================================' || CHAR(13)) || (CHAR(10) || '   Verification code')) || ((CHAR(13) || CHAR(10)) || ('=================================' || (CHAR(13) || CHAR(10))))) || ((('<ad:if test="#user#">' || CHAR(13)) || (CHAR(10) || 'Hello #user.FirstName#,')) || ((CHAR(13) || CHAR(10)) || ('</ad:if>' || (CHAR(13) || CHAR(10)))))) || ((((CHAR(13) || CHAR(10)) || ('to complete the sign in, enter the verification code on the device.' || CHAR(13))) || ((CHAR(10) || CHAR(13)) || (CHAR(10) || ('Verification code' || CHAR(13))))) || (((CHAR(10) || '# verificationCode#') || (CHAR(13) || CHAR(10))) || ((CHAR(13) || CHAR(10)) || ('Best regards,' || (CHAR(13) || CHAR(10))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('AddParkingPage', 'WebPolicy', 1, 'True');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('AddRandomDomainString', 'WebPolicy', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('AnonymousAccountPolicy', 'WebPolicy', 1, 'True;;5;20;;_web;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('AspInstalled', 'WebPolicy', 1, 'True');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('AspNetInstalled', 'WebPolicy', 1, '2');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('CgiBinInstalled', 'WebPolicy', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('DefaultDocuments', 'WebPolicy', 1, 'Default.htm,Default.asp,index.htm,Default.aspx');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('EnableAnonymousAccess', 'WebPolicy', 1, 'True');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('EnableBasicAuthentication', 'WebPolicy', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('EnableDedicatedPool', 'WebPolicy', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('EnableDirectoryBrowsing', 'WebPolicy', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('EnableParentPaths', 'WebPolicy', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('EnableParkingPageTokens', 'WebPolicy', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('EnableWindowsAuthentication', 'WebPolicy', 1, 'True');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('EnableWritePermissions', 'WebPolicy', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('FrontPageAccountPolicy', 'WebPolicy', 1, 'True;;1;20;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('FrontPagePasswordPolicy', 'WebPolicy', 1, 'True;5;20;0;1;0;False;;0;0;0;False;False;0;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('ParkingPageContent', 'WebPolicy', 1, ((((('<html xmlns="http://www.w3.org/1999/xhtml">' || CHAR(13)) || (CHAR(10) || ('<head>' || CHAR(13)))) || ((CHAR(10) || '    <title>The web site is under construction</title>') || (CHAR(13) || (CHAR(10) || '<style type="text/css">')))) || (((CHAR(13) || CHAR(10)) || ('	H1 { font-size: 16pt; margin-bottom: 4px; }' || (CHAR(13) || CHAR(10)))) || (('	H2 { font-size: 14pt; margin-bottom: 4px; font-weight: normal; }' || (CHAR(13) || CHAR(10))) || ('</style>' || (CHAR(13) || CHAR(10)))))) || (((('</head>' || CHAR(13)) || (CHAR(10) || ('<body>' || CHAR(13)))) || ((CHAR(10) || ('<div id="PageOutline">' || CHAR(13))) || (CHAR(10) || ('	<h1>This web site has just been created from <a href="https://www.FuseCP.com">FuseCP </a> and it is still under construction.</h1>' || CHAR(13))))) || (((CHAR(10) || '	<h2>The web site is hosted by <a href="https://fusecp.com">FuseCP</a>.</h2>') || (CHAR(13) || (CHAR(10) || '</div>'))) || ((CHAR(13) || (CHAR(10) || '</body>')) || (CHAR(13) || (CHAR(10) || '</html>')))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('ParkingPageName', 'WebPolicy', 1, 'default.aspx');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('PerlInstalled', 'WebPolicy', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('PhpInstalled', 'WebPolicy', 1, '');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('PublishingProfile', 'WebPolicy', 1, ((((((('<?xml version="1.0" encoding="utf-8"?>' || CHAR(13)) || (CHAR(10) || '<publishData>')) || ((CHAR(13) || CHAR(10)) || ('<ad:if test="#WebSite.WebDeploySitePublishingEnabled#">' || CHAR(13)))) || (((CHAR(10) || '	<publishProfile') || (CHAR(13) || CHAR(10))) || (('		profileName="#WebSite.Name# - Web Deploy"' || CHAR(13)) || (CHAR(10) || '		publishMethod="MSDeploy"')))) || ((((CHAR(13) || CHAR(10)) || ('		publishUrl="#WebSite["WmSvcServiceUrl"]#:#WebSite["WmSvcServicePort"]#"' || CHAR(13))) || ((CHAR(10) || '		msdeploySite="#WebSite.Name#"') || (CHAR(13) || CHAR(10)))) || ((('		userName="#WebSite.WebDeployPublishingAccount#"' || CHAR(13)) || (CHAR(10) || '		userPWD="#WebSite.WebDeployPublishingPassword#"')) || ((CHAR(13) || CHAR(10)) || ('		destinationAppUrl="http://#WebSite.Name#/"' || CHAR(13)))))) || (((((CHAR(10) || '		<ad:if test="#Not(IsNull(MsSqlDatabase)) and Not(IsNull(MsSqlUser))#">SQLServerDBConnectionString="server=#MsSqlServerExternalAddress#;Initial Catalog=#MsSqlDatabase.Name#;uid=#MsSqlUser.Name#;pwd=#MsSqlUser.Password#"</ad:if>') || (CHAR(13) || CHAR(10))) || (('		<ad:if test="#Not(IsNull(MySqlDatabase)) and Not(IsNull(MySqlUser))#">mySQLDBConnectionString="server=#MySqlAddress#;database=#MySqlDatabase.Name#;uid=#MySqlUser.Name#;pwd=#MySqlUser.Password#"</ad:if>' || CHAR(13)) || (CHAR(10) || '		<ad:if test="#Not(IsNull(MariaDBDatabase)) and Not(IsNull(MariaDBUser))#">MariaDBDBConnectionString="server=#MariaDBAddress#;database=#MariaDBDatabase.Name#;uid=#MariaDBUser.Name#;pwd=#MariaDBUser.Password#"</ad:if>'))) || (((CHAR(13) || CHAR(10)) || ('		hostingProviderForumLink="https://fusecp.com/support"' || CHAR(13))) || ((CHAR(10) || '		controlPanelLink="https://panel.fusecp.com/"') || (CHAR(13) || CHAR(10))))) || (((('	/>' || CHAR(13)) || (CHAR(10) || '</ad:if>')) || ((CHAR(13) || CHAR(10)) || ('<ad:if test="#IsDefined("FtpAccount")#">' || CHAR(13)))) || (((CHAR(10) || '	<publishProfile') || (CHAR(13) || CHAR(10))) || (('		profileName="#WebSite.Name# - FTP"' || CHAR(13)) || (CHAR(10) || '		publishMethod="FTP"')))))) || ((((((CHAR(13) || CHAR(10)) || ('		publishUrl="ftp://#FtpServiceAddress#"' || CHAR(13))) || ((CHAR(10) || '		ftpPassiveMode="True"') || (CHAR(13) || CHAR(10)))) || ((('		userName="#FtpAccount.Name#"' || CHAR(13)) || (CHAR(10) || '		userPWD="#FtpAccount.Password#"')) || ((CHAR(13) || CHAR(10)) || ('		destinationAppUrl="http://#WebSite.Name#/"' || CHAR(13))))) || ((((CHAR(10) || '		<ad:if test="#Not(IsNull(MsSqlDatabase)) and Not(IsNull(MsSqlUser))#">SQLServerDBConnectionString="server=#MsSqlServerExternalAddress#;Initial Catalog=#MsSqlDatabase.Name#;uid=#MsSqlUser.Name#;pwd=#MsSqlUser.Password#"</ad:if>') || (CHAR(13) || CHAR(10))) || (('		<ad:if test="#Not(IsNull(MySqlDatabase)) and Not(IsNull(MySqlUser))#">mySQLDBConnectionString="server=#MySqlAddress#;database=#MySqlDatabase.Name#;uid=#MySqlUser.Name#;pwd=#MySqlUser.Password#"</ad:if>' || CHAR(13)) || (CHAR(10) || '		<ad:if test="#Not(IsNull(MariaDBDatabase)) and Not(IsNull(MariaDBUser))#">MariaDBDBConnectionString="server=#MariaDBAddress#;database=#MariaDBDatabase.Name#;uid=#MariaDBUser.Name#;pwd=#MariaDBUser.Password#"</ad:if>'))) || (((CHAR(13) || CHAR(10)) || ('		hostingProviderForumLink="https://fusecp.com/support"' || CHAR(13))) || ((CHAR(10) || '		controlPanelLink="https://panel.fusecp.com/"') || (CHAR(13) || CHAR(10)))))) || ((((('    />' || CHAR(13)) || (CHAR(10) || '</ad:if>')) || ((CHAR(13) || CHAR(10)) || ('</publishData>' || CHAR(13)))) || (((CHAR(10) || CHAR(13)) || (CHAR(10) || '<!--')) || ((CHAR(13) || CHAR(10)) || ('Control Panel:' || CHAR(13))))) || ((((CHAR(10) || 'Username: #User.Username#') || (CHAR(13) || CHAR(10))) || (('Password: #User.Password#' || CHAR(13)) || (CHAR(10) || CHAR(13)))) || (((CHAR(10) || 'Technical Contact:') || (CHAR(13) || CHAR(10))) || (('support@fusecp.com' || CHAR(13)) || (CHAR(10) || '-->'))))))));
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('PythonInstalled', 'WebPolicy', 1, 'False');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('SecuredGroupNamePolicy', 'WebPolicy', 1, 'True;;1;20;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('SecuredUserNamePolicy', 'WebPolicy', 1, 'True;;1;20;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('SecuredUserPasswordPolicy', 'WebPolicy', 1, 'True;5;20;0;1;0;False;;0;0;0;False;False;0;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('VirtDirNamePolicy', 'WebPolicy', 1, 'True;-;3;50;;;');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('WebDataFolder', 'WebPolicy', 1, '\[DOMAIN_NAME]\data');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('WebLogsFolder', 'WebPolicy', 1, '\[DOMAIN_NAME]\logs');
SELECT changes();

INSERT INTO "UserSettings" ("PropertyName", "SettingsName", "UserID", "PropertyValue")
VALUES ('WebRootFolder', 'WebPolicy', 1, '\[DOMAIN_NAME]\wwwroot');
SELECT changes();


INSERT INTO "PackagesTreeCache" ("PackageID", "ParentPackageID")
VALUES (1, 1);
SELECT changes();


INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (2, 6, NULL, 7, NULL, 'Databases', 'MySQL4.Databases', 1.0, 2, 1);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (3, 5, NULL, 5, NULL, 'Databases', 'MsSQL2000.Databases', 1.0, 2, 1);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (4, 3, NULL, 9, NULL, 'FTP Accounts', 'FTP.Accounts', 1.0, 2, 1);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (12, 8, NULL, 14, NULL, 'Statistics Sites', 'Stats.Sites', 1.0, 2, 1);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (13, 2, NULL, 10, NULL, 'Web Sites', 'Web.Sites', 1.0, 2, 1);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (14, 4, NULL, 15, NULL, 'Mail Accounts', 'Mail.Accounts', 1.0, 2, 1);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (15, 5, NULL, 6, NULL, 'Users', 'MsSQL2000.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (18, 4, NULL, 16, NULL, 'Mail Forwardings', 'Mail.Forwardings', 3.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (19, 6, NULL, 8, NULL, 'Users', 'MySQL4.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (20, 4, NULL, 17, NULL, 'Mail Lists', 'Mail.Lists', 6.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (24, 4, NULL, 18, NULL, 'Mail Groups', 'Mail.Groups', 4.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (47, 1, NULL, 20, NULL, 'ODBC DSNs', 'OS.ODBC', 6.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (59, 2, NULL, 25, NULL, 'Shared SSL Folders', 'Web.SharedSSL', 8.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (62, 10, NULL, 21, NULL, 'Databases', 'MsSQL2005.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (63, 10, NULL, 22, NULL, 'Users', 'MsSQL2005.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (68, 11, NULL, 23, NULL, 'Databases', 'MySQL5.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (69, 11, NULL, 24, NULL, 'Users', 'MySQL5.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (110, 90, NULL, 75, NULL, 'Databases', 'MySQL8.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (111, 90, NULL, 76, NULL, 'Users', 'MySQL8.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (120, 91, NULL, 75, NULL, 'Databases', 'MySQL9.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (121, 91, NULL, 76, NULL, 'Users', 'MySQL9.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (200, 20, NULL, 200, 1, 'SharePoint Site Collections', 'HostedSharePoint.Sites', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (205, 13, NULL, 29, NULL, 'Organizations', 'HostedSolution.Organizations', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (206, 13, NULL, 30, 1, 'Users', 'HostedSolution.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (211, 22, NULL, 31, NULL, 'Databases', 'MsSQL2008.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (212, 22, NULL, 32, NULL, 'Users', 'MsSQL2008.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (218, 23, NULL, 37, NULL, 'Databases', 'MsSQL2012.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (219, 23, NULL, 38, NULL, 'Users', 'MsSQL2012.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (300, 30, NULL, 33, NULL, 'Number of VPS', 'VPS.ServersNumber', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (345, 40, NULL, 35, NULL, 'Number of VPS', 'VPSForPC.ServersNumber', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (470, 46, NULL, 39, NULL, 'Databases', 'MsSQL2014.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (471, 46, NULL, 40, NULL, 'Users', 'MsSQL2014.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (550, 73, NULL, 204, 1, 'SharePoint Site Collections', 'HostedSharePointEnterprise.Sites', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (553, 33, NULL, 41, NULL, 'Number of VPS', 'VPS2012.ServersNumber', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (573, 50, NULL, 202, NULL, 'Databases', 'MariaDB.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (574, 50, NULL, 203, NULL, 'Users', 'MariaDB.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (673, 167, NULL, 41, NULL, 'Number of VPS', 'PROXMOX.ServersNumber', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (701, 71, NULL, 71, NULL, 'Databases', 'MsSQL2016.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (702, 71, NULL, 72, NULL, 'Users', 'MsSQL2016.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (711, 72, NULL, 73, NULL, 'Databases', 'MsSQL2017.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (712, 72, NULL, 74, NULL, 'Users', 'MsSQL2017.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (721, 74, NULL, 77, NULL, 'Databases', 'MsSQL2019.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (722, 74, NULL, 78, NULL, 'Users', 'MsSQL2019.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (732, 75, NULL, 79, NULL, 'Databases', 'MsSQL2022.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (733, 75, NULL, 80, NULL, 'Users', 'MsSQL2022.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (760, 76, NULL, 79, NULL, 'Databases', 'MsSQL2025.Databases', 1.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (761, 76, NULL, 80, NULL, 'Users', 'MsSQL2025.Users', 2.0, 2, 0);
SELECT changes();

INSERT INTO "Quotas" ("QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota")
VALUES (770, 4, NULL, 11, NULL, 'Mail Domains', 'Mail.Domains', 1.1000000000000001, 2, 1);
SELECT changes();


INSERT INTO "Schedule" ("ScheduleID", "Enabled", "FromTime", "HistoriesNumber", "Interval", "LastRun", "MaxExecutionTime", "NextRun", "PackageID", "PriorityID", "ScheduleName", "ScheduleTypeID", "StartTime", "TaskID", "ToTime", "WeekMonthDay")
VALUES (1, 1, '2000-01-01 12:00:00', 7, 0, NULL, 3600, '2010-07-16 14:53:02.47', 1, 'Normal', 'Calculate Disk Space', 'Daily', '2000-01-01 12:30:00', 'SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE', '2000-01-01 12:00:00', 1);
SELECT changes();

INSERT INTO "Schedule" ("ScheduleID", "Enabled", "FromTime", "HistoriesNumber", "Interval", "LastRun", "MaxExecutionTime", "NextRun", "PackageID", "PriorityID", "ScheduleName", "ScheduleTypeID", "StartTime", "TaskID", "ToTime", "WeekMonthDay")
VALUES (2, 1, '2000-01-01 12:00:00', 7, 0, NULL, 3600, '2010-07-16 14:53:02.477', 1, 'Normal', 'Calculate Bandwidth', 'Daily', '2000-01-01 12:00:00', 'SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH', '2000-01-01 12:00:00', 1);
SELECT changes();


INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UsersHome', 1, '%SYSTEMDRIVE%\HostingSpaces');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet11Path', 2, '%SYSTEMROOT%\Microsoft.NET\Framework\v1.1.4322\aspnet_isapi.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet11Pool', 2, 'ASP.NET V1.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet20Path', 2, '%SYSTEMROOT%\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet20Pool', 2, 'ASP.NET V2.0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet40Path', 2, '%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\aspnet_isapi.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet40Pool', 2, 'ASP.NET V4.0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspPath', 2, '%SYSTEMROOT%\System32\InetSrv\asp.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CFFlashRemotingDirectory', 2, 'C:\ColdFusion9\runtime\lib\wsconfig\1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CFScriptsDirectory', 2, 'C:\Inetpub\wwwroot\CFIDE');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ColdFusionPath', 2, 'C:\ColdFusion9\runtime\lib\wsconfig\jrun_iis6.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('GalleryXmlFeedUrl', 2, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PerlPath', 2, '%SYSTEMDRIVE%\Perl\bin\Perl.exe');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Php4Path', 2, '%PROGRAMFILES%\PHP\php.exe');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Php5Path', 2, '%PROGRAMFILES%\PHP\php-cgi.exe');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ProtectedAccessFile', 2, '.htaccess');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ProtectedFoldersFile', 2, '.htfolders');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ProtectedGroupsFile', 2, '.htgroup');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ProtectedUsersFile', 2, '.htpasswd');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PythonPath', 2, '%SYSTEMDRIVE%\Python\python.exe');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SecuredFoldersFilterPath', 2, '%SYSTEMROOT%\System32\InetSrv\IISPasswordFilter.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('WebGroupName', 2, 'FCPWebUsers');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('FtpGroupName', 3, 'FCPFtpUsers');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SiteId', 3, 'MSFTPSVC/1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DatabaseLocation', 5, '%SYSTEMDRIVE%\SQL2000Databases\[USER_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 5, '(local)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 5, '(local)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SaLogin', 5, 'sa');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SaPassword', 5, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UseDefaultDatabaseLocation', 5, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UseTrustedConnection', 5, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 6, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 6, '%PROGRAMFILES%\MySQL\MySQL Server 4.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 6, 'localhost,3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 6, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 6, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExpireLimit', 7, '1209600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('MinimumTTL', 7, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('NameServers', 7, 'ns1.yourdomain.com;ns2.yourdomain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordDefaultTTL', 7, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordMinimumTTL', 7, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RefreshInterval', 7, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ResponsiblePerson', 7, 'hostmaster.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RetryDelay', 7, '600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AwStatsFolder', 8, '%SYSTEMDRIVE%\AWStats\wwwroot\cgi-bin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('BatchFileName', 8, 'UpdateStats.bat');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('BatchLineTemplate', 8, '%SYSTEMDRIVE%\perl\bin\perl.exe awstats.pl config=[DOMAIN_NAME] -update');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ConfigFileName', 8, 'awstats.[DOMAIN_NAME].conf');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ConfigFileTemplate', 8, ((((('LogFormat = "%time2 %other %other %other %method %url %other %other %logname %host %other %ua %other %referer %other %code %other %other %bytesd %other %other"' || CHAR(13)) || (CHAR(10) || 'LogSeparator = " "')) || ((CHAR(13) || CHAR(10)) || ('DNSLookup = 2' || (CHAR(13) || CHAR(10))))) || ((('DirCgi = "/cgi-bin"' || CHAR(13)) || (CHAR(10) || 'DirIcons = "/icon"')) || ((CHAR(13) || CHAR(10)) || ('AllowFullYearView=3' || (CHAR(13) || CHAR(10)))))) || (((('AllowToUpdateStatsFromBrowser = 0' || CHAR(13)) || (CHAR(10) || 'UseFramesWhenCGI = 1')) || ((CHAR(13) || CHAR(10)) || ('ShowFlagLinks = "en fr de it nl es"' || (CHAR(13) || CHAR(10))))) || ((('LogFile = "[LOGS_FOLDER]\ex%YY-3%MM-3%DD-3.log"' || CHAR(13)) || (CHAR(10) || ('DirData = "%SYSTEMDRIVE%\AWStats\data"' || CHAR(13)))) || ((CHAR(10) || 'SiteDomain = "[DOMAIN_NAME]"') || (CHAR(13) || (CHAR(10) || 'HostAliases = [DOMAIN_ALIASES]')))))));
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('StatisticsURL', 8, 'http://127.0.0.1/AWStats/cgi-bin/awstats.pl?config=[domain_name]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminLogin', 9, 'Admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExpireLimit', 9, '1209600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('MinimumTTL', 9, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('NameServers', 9, 'ns1.yourdomain.com;ns2.yourdomain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordDefaultTTL', 9, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordMinimumTTL', 9, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RefreshInterval', 9, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ResponsiblePerson', 9, 'hostmaster.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RetryDelay', 9, '600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SimpleDnsUrl', 9, 'http://127.0.0.1:8053');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogDeleteDays', 10, '0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogFormat', 10, 'W3Cex');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogWildcard', 10, '*.log');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Password', 10, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerID', 10, '1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SmarterLogDeleteMonths', 10, '0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SmarterLogsPath', 10, '%SYSTEMDRIVE%\SmarterLogs');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SmarterUrl', 10, 'http://127.0.0.1:9999/services');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('StatisticsURL', 10, 'http://127.0.0.1:9999/Login.aspx?txtSiteID=[site_id]&txtUser=[username]&txtPass=[password]&shortcutLink=autologin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('TimeZoneId', 10, '27');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Username', 10, 'Admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminPassword', 11, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminUsername', 11, 'admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('defaultdomainhostname', 11, 'mail.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DomainsPath', 11, '%SYSTEMDRIVE%\SmarterMail');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerIPAddress', 11, '127.0.0.1;127.0.0.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServiceUrl', 11, 'http://127.0.0.1:9998/services');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 12, '%PROGRAMFILES%\Gene6 FTP Server');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogsFolder', 12, '%PROGRAMFILES%\Gene6 FTP Server\Log');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminPassword', 14, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminUsername', 14, 'admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('defaultdomainhostname', 14, 'mail.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DomainsPath', 14, '%SYSTEMDRIVE%\SmarterMail');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerIPAddress', 14, '127.0.0.1;127.0.0.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServiceUrl', 14, 'http://127.0.0.1:9998/services');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('BrowseMethod', 16, 'POST');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('BrowseParameters', 16, ((('ServerName=[SERVER]' || CHAR(13)) || (CHAR(10) || ('Login=[USER]' || CHAR(13)))) || ((CHAR(10) || 'Password=[PASSWORD]') || (CHAR(13) || (CHAR(10) || 'Protocol=dbmssocn')))));
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('BrowseURL', 16, 'http://localhost/MLA/silentlogon.aspx');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DatabaseLocation', 16, '%SYSTEMDRIVE%\SQL2005Databases\[USER_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 16, '(local)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 16, '(local)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SaLogin', 16, 'sa');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SaPassword', 16, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UseDefaultDatabaseLocation', 16, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UseTrustedConnection', 16, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 17, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 17, '%PROGRAMFILES%\MySQL\MySQL Server 5.0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 17, 'localhost,3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 17, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 17, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminPassword', 22, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminUsername', 22, 'Administrator');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DomainId', 25, '1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('KeepDeletedItemsDays', 27, '14');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('KeepDeletedMailboxesDays', 27, '30');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('MailboxDatabase', 27, 'Hosted Exchange Database');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootOU', 27, 'FCP Hosting');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('StorageGroup', 27, 'Hosted Exchange Storage Group');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('TempDomain', 27, 'my-temp-domain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminLogin', 28, 'Admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExpireLimit', 28, '1209600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('MinimumTTL', 28, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('NameServers', 28, 'ns1.yourdomain.com;ns2.yourdomain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordDefaultTTL', 28, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordMinimumTTL', 28, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RefreshInterval', 28, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ResponsiblePerson', 28, 'hostmaster.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RetryDelay', 28, '600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SimpleDnsUrl', 28, 'http://127.0.0.1:8053');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminPassword', 29, ' ');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminUsername', 29, 'admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('defaultdomainhostname', 29, 'mail.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DomainsPath', 29, '%SYSTEMDRIVE%\SmarterMail');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerIPAddress', 29, '127.0.0.1;127.0.0.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServiceUrl', 29, 'http://localhost:9998/services/');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 30, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 30, '%PROGRAMFILES%\MySQL\MySQL Server 5.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 30, 'localhost,3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 30, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 30, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogDeleteDays', 31, '0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogFormat', 31, 'W3Cex');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogWildcard', 31, '*.log');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Password', 31, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerID', 31, '1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SmarterLogDeleteMonths', 31, '0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SmarterLogsPath', 31, '%SYSTEMDRIVE%\SmarterLogs');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SmarterUrl', 31, 'http://127.0.0.1:9999/services');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('StatisticsURL', 31, 'http://127.0.0.1:9999/Login.aspx?txtSiteID=[site_id]&txtUser=[username]&txtPass=[password]&shortcutLink=autologin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('TimeZoneId', 31, '27');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Username', 31, 'Admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('KeepDeletedItemsDays', 32, '14');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('KeepDeletedMailboxesDays', 32, '30');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('MailboxDatabase', 32, 'Hosted Exchange Database');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootOU', 32, 'FCP Hosting');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('TempDomain', 32, 'my-temp-domain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExpireLimit', 56, '1209600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('MinimumTTL', 56, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('NameServers', 56, 'ns1.yourdomain.com;ns2.yourdomain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PDNSDbName', 56, 'pdnsdb');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PDNSDbPort', 56, '3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PDNSDbServer', 56, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PDNSDbUser', 56, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordDefaultTTL', 56, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordMinimumTTL', 56, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RefreshInterval', 56, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ResponsiblePerson', 56, 'hostmaster.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RetryDelay', 56, '600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminPassword', 60, ' ');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminUsername', 60, 'admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('defaultdomainhostname', 60, 'mail.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DomainsPath', 60, '%SYSTEMDRIVE%\SmarterMail');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerIPAddress', 60, '127.0.0.1;127.0.0.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServiceUrl', 60, 'http://localhost:9998/services/');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogDeleteDays', 62, '0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogFormat', 62, 'W3Cex');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogWildcard', 62, '*.log');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Password', 62, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerID', 62, '1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SmarterLogDeleteMonths', 62, '0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SmarterLogsPath', 62, '%SYSTEMDRIVE%\SmarterLogs');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SmarterUrl', 62, 'http://127.0.0.1:9999/services');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('StatisticsURL', 62, 'http://127.0.0.1:9999/Login.aspx?txtSiteID=[site_id]&txtUser=[username]&txtPass=[password]&shortcutLink=autologin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('TimeZoneId', 62, '27');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Username', 62, 'Admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminPassword', 63, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminUsername', 63, 'Administrator');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminPassword', 64, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminUsername', 64, 'admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('defaultdomainhostname', 64, 'mail.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DomainsPath', 64, '%SYSTEMDRIVE%\SmarterMail');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerIPAddress', 64, '127.0.0.1;127.0.0.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServiceUrl', 64, 'http://localhost:9998/services/');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminPassword', 65, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminUsername', 65, 'admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('defaultdomainhostname', 65, 'mail.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DomainsPath', 65, '%SYSTEMDRIVE%\SmarterMail');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerIPAddress', 65, '127.0.0.1;127.0.0.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServiceUrl', 65, 'http://localhost:9998/services/');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminPassword', 66, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminUsername', 66, 'admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('defaultdomainhostname', 66, 'mail.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DomainsPath', 66, '%SYSTEMDRIVE%\SmarterMail');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerIPAddress', 66, '127.0.0.1;127.0.0.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServiceUrl', 66, 'http://localhost:9998/services/');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminPassword', 67, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminUsername', 67, 'admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('defaultdomainhostname', 67, 'mail.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DomainsPath', 67, '%SYSTEMDRIVE%\SmarterMail\Domains');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServerIPAddress', 67, '127.0.0.1;127.0.0.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ServiceUrl', 67, 'http://localhost:9998');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UsersHome', 100, '%SYSTEMDRIVE%\HostingSpaces');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet11Pool', 101, 'ASP.NET 1.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet40Path', 101, '%WINDIR%\Microsoft.NET\Framework\v4.0.30319\aspnet_isapi.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet40x64Path', 101, '%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\aspnet_isapi.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNetBitnessMode', 101, '32');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CFFlashRemotingDirectory', 101, 'C:\ColdFusion9\runtime\lib\wsconfig\1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CFScriptsDirectory', 101, 'C:\Inetpub\wwwroot\CFIDE');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ClassicAspNet20Pool', 101, 'ASP.NET 2.0 (Classic)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ClassicAspNet40Pool', 101, 'ASP.NET 4.0 (Classic)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ColdFusionPath', 101, 'C:\ColdFusion9\runtime\lib\wsconfig\jrun_iis6.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('GalleryXmlFeedUrl', 101, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('IntegratedAspNet20Pool', 101, 'ASP.NET 2.0 (Integrated)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('IntegratedAspNet40Pool', 101, 'ASP.NET 4.0 (Integrated)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PerlPath', 101, '%SYSTEMDRIVE%\Perl\bin\PerlEx30.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Php4Path', 101, '%PROGRAMFILES(x86)%\PHP\php.exe');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PhpMode', 101, 'FastCGI');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PhpPath', 101, '%PROGRAMFILES(x86)%\PHP\php-cgi.exe');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ProtectedGroupsFile', 101, '.htgroup');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ProtectedUsersFile', 101, '.htpasswd');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SecureFoldersModuleAssembly', 101, 'FuseCP.IIsModules.SecureFolders, FuseCP.IIsModules, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=37f9c58a0aa32ff0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('WebGroupName', 101, 'FCP_IUSRS');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('WmSvc.CredentialsMode', 101, 'WINDOWS');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('WmSvc.Port', 101, '8172');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('FtpGroupName', 102, 'FCPFtpUsers');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SiteId', 102, 'Default FTP Site');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UsersHome', 104, '%SYSTEMDRIVE%\HostingSpaces');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet11Pool', 105, 'ASP.NET 1.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet40Path', 105, '%WINDIR%\Microsoft.NET\Framework\v4.0.30319\aspnet_isapi.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet40x64Path', 105, '%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\aspnet_isapi.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNetBitnessMode', 105, '32');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CFFlashRemotingDirectory', 105, 'C:\ColdFusion9\runtime\lib\wsconfig\1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CFScriptsDirectory', 105, 'C:\Inetpub\wwwroot\CFIDE');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ClassicAspNet20Pool', 105, 'ASP.NET 2.0 (Classic)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ClassicAspNet40Pool', 105, 'ASP.NET 4.0 (Classic)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ColdFusionPath', 105, 'C:\ColdFusion9\runtime\lib\wsconfig\jrun_iis6.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('GalleryXmlFeedUrl', 105, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('IntegratedAspNet20Pool', 105, 'ASP.NET 2.0 (Integrated)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('IntegratedAspNet40Pool', 105, 'ASP.NET 4.0 (Integrated)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PerlPath', 105, '%SYSTEMDRIVE%\Perl\bin\PerlEx30.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Php4Path', 105, '%PROGRAMFILES(x86)%\PHP\php.exe');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PhpMode', 105, 'FastCGI');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PhpPath', 105, '%PROGRAMFILES(x86)%\PHP\php-cgi.exe');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ProtectedGroupsFile', 105, '.htgroup');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ProtectedUsersFile', 105, '.htpasswd');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SecureFoldersModuleAssembly', 105, 'FuseCP.IIsModules.SecureFolders, FuseCP.IIsModules, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=37f9c58a0aa32ff0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('sslusesni', 105, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('WebGroupName', 105, 'FCP_IUSRS');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('WmSvc.CredentialsMode', 105, 'WINDOWS');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('WmSvc.Port', 105, '8172');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('FtpGroupName', 106, 'FCPFtpUsers');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SiteId', 106, 'Default FTP Site');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('sslusesni', 106, 'False');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UsersHome', 111, '%SYSTEMDRIVE%\HostingSpaces');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet11Pool', 112, 'ASP.NET 1.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet40Path', 112, '%WINDIR%\Microsoft.NET\Framework\v4.0.30319\aspnet_isapi.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNet40x64Path', 112, '%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\aspnet_isapi.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AspNetBitnessMode', 112, '32');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CFFlashRemotingDirectory', 112, 'C:\ColdFusion9\runtime\lib\wsconfig\1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CFScriptsDirectory', 112, 'C:\Inetpub\wwwroot\CFIDE');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ClassicAspNet20Pool', 112, 'ASP.NET 2.0 (Classic)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ClassicAspNet40Pool', 112, 'ASP.NET 4.0 (Classic)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ColdFusionPath', 112, 'C:\ColdFusion9\runtime\lib\wsconfig\jrun_iis6.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('GalleryXmlFeedUrl', 112, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('IntegratedAspNet20Pool', 112, 'ASP.NET 2.0 (Integrated)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('IntegratedAspNet40Pool', 112, 'ASP.NET 4.0 (Integrated)');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PerlPath', 112, '%SYSTEMDRIVE%\Perl\bin\PerlEx30.dll');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('Php4Path', 112, '%PROGRAMFILES(x86)%\PHP\php.exe');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PhpMode', 112, 'FastCGI');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PhpPath', 112, '%PROGRAMFILES(x86)%\PHP\php-cgi.exe');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ProtectedGroupsFile', 112, '.htgroup');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ProtectedUsersFile', 112, '.htpasswd');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SecureFoldersModuleAssembly', 112, 'FuseCP.IIsModules.SecureFolders, FuseCP.IIsModules, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=37f9c58a0aa32ff0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('sslusesni', 112, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('WebGroupName', 112, 'FCP_IUSRS');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('WmSvc.CredentialsMode', 112, 'WINDOWS');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('WmSvc.Port', 112, '8172');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('FtpGroupName', 113, 'FCPFtpUsers');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SiteId', 113, 'Default FTP Site');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('sslusesni', 113, 'False');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootWebApplicationIpAddress', 200, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UserName', 204, 'admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UtilityPath', 204, 'C:\Program Files\Research In Motion\BlackBerry Enterprise Server Resource Kit\BlackBerry Enterprise Server User Administration Tool');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CpuLimit', 300, '100');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CpuReserve', 300, '0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('CpuWeight', 300, '100');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('DvdLibraryPath', 300, 'C:\Hyper-V\Library');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExportedVpsPath', 300, 'C:\Hyper-V\Exported');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('HostnamePattern', 300, 'vps[user_id].hosterdomain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('OsTemplatesPath', 300, 'C:\Hyper-V\Templates');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('PrivateNetworkFormat', 300, '192.168.0.1/16');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootFolder', 300, 'C:\Hyper-V\VirtualMachines\[VPS_HOSTNAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('StartAction', 300, 'start');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('StartupDelay', 300, '0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('StopAction', 300, 'shutDown');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('VirtualDiskType', 300, 'dynamic');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 301, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 301, '%PROGRAMFILES%\MySQL\MySQL Server 5.5');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 301, 'localhost,3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 301, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 301, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 304, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 304, '%PROGRAMFILES%\MySQL\MySQL Server 8.0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 304, 'localhost,3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 304, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 304, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('sslmode', 304, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 305, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 305, '%PROGRAMFILES%\MySQL\MySQL Server 8.0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 305, 'localhost,3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 305, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 305, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('sslmode', 305, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 306, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 306, '%PROGRAMFILES%\MySQL\MySQL Server 8.0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 306, 'localhost,3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 306, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 306, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('sslmode', 306, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 307, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 307, '%PROGRAMFILES%\MySQL\MySQL Server 8.0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 307, 'localhost,3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 307, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 307, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('sslmode', 307, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 308, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 308, '%PROGRAMFILES%\MySQL\MySQL Server 8.0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 308, 'localhost,3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 308, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 308, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('sslmode', 308, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 320, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 320, '%PROGRAMFILES%\MySQL\MySQL Server 9.0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 320, 'localhost,3306');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 320, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 320, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('sslmode', 320, 'True');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('admode', 410, 'False');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('expirelimit', 410, '1209600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('minimumttl', 410, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('nameservers', 410, 'ns1.yourdomain.com;ns2.yourdomain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordDefaultTTL', 410, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordMinimumTTL', 410, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('refreshinterval', 410, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('responsibleperson', 410, 'hostmaster.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('retrydelay', 410, '600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('LogDir', 500, '/var/log');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UsersHome', 500, '/var/www/HostingSpaces');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1550, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1550, '%PROGRAMFILES%\MariaDB 10.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1550, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1550, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1550, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1570, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1570, '%PROGRAMFILES%\MariaDB 10.3');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1570, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1570, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1570, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1571, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1571, '%PROGRAMFILES%\MariaDB 10.4');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1571, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1571, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1571, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1572, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1572, '%PROGRAMFILES%\MariaDB 10.5');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1572, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1572, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1572, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1573, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1573, '%PROGRAMFILES%\MariaDB 10.6');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1573, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1573, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1573, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1574, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1574, '%PROGRAMFILES%\MariaDB 10.7');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1574, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1574, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1574, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1575, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1575, '%PROGRAMFILES%\MariaDB 10.8');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1575, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1575, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1575, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1576, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1576, '%PROGRAMFILES%\MariaDB 10.9');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1576, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1576, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1576, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1577, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1577, '%PROGRAMFILES%\MariaDB 10.10');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1577, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1577, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1577, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1578, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1578, '%PROGRAMFILES%\MariaDB 10.11');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1578, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1578, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1578, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1579, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1579, '%PROGRAMFILES%\MariaDB 11.0');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1579, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1579, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1579, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1580, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1580, '%PROGRAMFILES%\MariaDB 11.1');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1580, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1580, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1580, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1581, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1581, '%PROGRAMFILES%\MariaDB 11.2');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1581, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1581, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1581, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1582, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1582, '%PROGRAMFILES%\MariaDB 11.3');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1582, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1582, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1582, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1583, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1583, '%PROGRAMFILES%\MariaDB 11.4');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1583, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1583, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1583, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1584, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1584, '%PROGRAMFILES%\MariaDB 11.5');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1584, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1584, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1584, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1585, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1585, '%PROGRAMFILES%\MariaDB 11.6');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1585, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1585, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1585, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExternalAddress', 1586, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InstallFolder', 1586, '%PROGRAMFILES%\MariaDB 11.7');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('InternalAddress', 1586, 'localhost');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootLogin', 1586, 'root');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RootPassword', 1586, '');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordDefaultTTL', 1703, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordMinimumTTL', 1703, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UsersHome', 1800, '%SYSTEMDRIVE%\HostingSpaces');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UsersHome', 1802, '%SYSTEMDRIVE%\HostingSpaces');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('UsersHome', 1804, '%SYSTEMDRIVE%\HostingSpaces');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminLogin', 1901, 'Admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExpireLimit', 1901, '1209600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('MinimumTTL', 1901, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('NameServers', 1901, 'ns1.yourdomain.com;ns2.yourdomain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordDefaultTTL', 1901, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordMinimumTTL', 1901, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RefreshInterval', 1901, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ResponsiblePerson', 1901, 'hostmaster.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RetryDelay', 1901, '600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SimpleDnsUrl', 1901, 'http://127.0.0.1:8053');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('admode', 1902, 'False');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('expirelimit', 1902, '1209600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('minimumttl', 1902, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('nameservers', 1902, 'ns1.yourdomain.com;ns2.yourdomain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordDefaultTTL', 1902, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordMinimumTTL', 1902, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('refreshinterval', 1902, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('responsibleperson', 1902, 'hostmaster.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('retrydelay', 1902, '600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('AdminLogin', 1903, 'Admin');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ExpireLimit', 1903, '1209600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('NameServers', 1903, 'ns1.yourdomain.com;ns2.yourdomain.com');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordDefaultTTL', 1903, '86400');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RecordMinimumTTL', 1903, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RefreshInterval', 1903, '3600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ResponsiblePerson', 1903, 'hostmaster.[DOMAIN_NAME]');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('RetryDelay', 1903, '600');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('SimpleDnsUrl', 1903, 'http://127.0.0.1:8053');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ConfigFile', 1910, '/etc/vsftpd.conf');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ConfigFile', 1911, '/etc/apache2/apache2.conf');
SELECT changes();

INSERT INTO "ServiceDefaultProperties" ("PropertyName", "ProviderID", "PropertyValue")
VALUES ('ConfigPath', 1911, '/etc/apache2');
SELECT changes();


INSERT INTO "ScheduleParameters" ("ParameterID", "ScheduleID", "ParameterValue")
VALUES ('SUSPEND_OVERUSED', 1, 'false');
SELECT changes();

INSERT INTO "ScheduleParameters" ("ParameterID", "ScheduleID", "ParameterValue")
VALUES ('SUSPEND_OVERUSED', 2, 'false');
SELECT changes();


CREATE INDEX "AccessTokensIdx_AccountID" ON "AccessTokens" ("AccountID");

CREATE INDEX "BackgroundTaskLogsIdx_TaskID" ON "BackgroundTaskLogs" ("TaskID");

CREATE INDEX "BackgroundTaskParametersIdx_TaskID" ON "BackgroundTaskParameters" ("TaskID");

CREATE INDEX "BackgroundTaskStackIdx_TaskID" ON "BackgroundTaskStack" ("TaskID");

CREATE INDEX "BlackBerryUsersIdx_AccountId" ON "BlackBerryUsers" ("AccountId");

CREATE INDEX "CommentsIdx_UserID" ON "Comments" ("UserID");

CREATE INDEX "CRMUsersIdx_AccountID" ON "CRMUsers" ("AccountID");

CREATE INDEX "DmzIPAddressesIdx_ItemID" ON "DmzIPAddresses" ("ItemID");

CREATE INDEX "DomainDnsRecordsIdx_DomainId" ON "DomainDnsRecords" ("DomainId");

CREATE INDEX "DomainsIdx_MailDomainID" ON "Domains" ("MailDomainID");

CREATE INDEX "DomainsIdx_PackageID" ON "Domains" ("PackageID");

CREATE INDEX "DomainsIdx_WebSiteID" ON "Domains" ("WebSiteID");

CREATE INDEX "DomainsIdx_ZoneItemID" ON "Domains" ("ZoneItemID");

CREATE INDEX "EnterpriseFoldersIdx_StorageSpaceFolderId" ON "EnterpriseFolders" ("StorageSpaceFolderId");

CREATE INDEX "EnterpriseFoldersOwaPermissionsIdx_AccountID" ON "EnterpriseFoldersOwaPermissions" ("AccountID");

CREATE INDEX "EnterpriseFoldersOwaPermissionsIdx_FolderID" ON "EnterpriseFoldersOwaPermissions" ("FolderID");

CREATE INDEX "ExchangeAccountEmailAddressesIdx_AccountID" ON "ExchangeAccountEmailAddresses" ("AccountID");

CREATE UNIQUE INDEX "IX_ExchangeAccountEmailAddresses_UniqueEmail" ON "ExchangeAccountEmailAddresses" ("EmailAddress");

CREATE INDEX "ExchangeAccountsIdx_ItemID" ON "ExchangeAccounts" ("ItemID");

CREATE INDEX "ExchangeAccountsIdx_MailboxPlanId" ON "ExchangeAccounts" ("MailboxPlanId");

CREATE UNIQUE INDEX "IX_ExchangeAccounts_UniqueAccountName" ON "ExchangeAccounts" ("AccountName");

CREATE INDEX "ExchangeMailboxPlansIdx_ItemID" ON "ExchangeMailboxPlans" ("ItemID");

CREATE UNIQUE INDEX "IX_ExchangeMailboxPlans" ON "ExchangeMailboxPlans" ("MailboxPlanId");

CREATE INDEX "ExchangeOrganizationDomainsIdx_ItemID" ON "ExchangeOrganizationDomains" ("ItemID");

CREATE UNIQUE INDEX "IX_ExchangeOrganizationDomains_UniqueDomain" ON "ExchangeOrganizationDomains" ("DomainID");

CREATE UNIQUE INDEX "IX_ExchangeOrganizations_UniqueOrg" ON "ExchangeOrganizations" ("OrganizationID");

CREATE INDEX "ExchangeOrganizationSettingsIdx_ItemId" ON "ExchangeOrganizationSettings" ("ItemId");

CREATE INDEX "ExchangeOrganizationSsFoldersIdx_ItemId" ON "ExchangeOrganizationSsFolders" ("ItemId");

CREATE INDEX "ExchangeOrganizationSsFoldersIdx_StorageSpaceFolderId" ON "ExchangeOrganizationSsFolders" ("StorageSpaceFolderId");

CREATE INDEX "GlobalDnsRecordsIdx_IPAddressID" ON "GlobalDnsRecords" ("IPAddressID");

CREATE INDEX "GlobalDnsRecordsIdx_PackageID" ON "GlobalDnsRecords" ("PackageID");

CREATE INDEX "GlobalDnsRecordsIdx_ServerID" ON "GlobalDnsRecords" ("ServerID");

CREATE INDEX "GlobalDnsRecordsIdx_ServiceID" ON "GlobalDnsRecords" ("ServiceID");

CREATE INDEX "IX_HostingPlanQuotas_QuotaID" ON "HostingPlanQuotas" ("QuotaID");

CREATE INDEX "IX_HostingPlanResources_GroupID" ON "HostingPlanResources" ("GroupID");

CREATE INDEX "HostingPlansIdx_PackageID" ON "HostingPlans" ("PackageID");

CREATE INDEX "HostingPlansIdx_ServerID" ON "HostingPlans" ("ServerID");

CREATE INDEX "HostingPlansIdx_UserID" ON "HostingPlans" ("UserID");

CREATE INDEX "IPAddressesIdx_ServerID" ON "IPAddresses" ("ServerID");

CREATE UNIQUE INDEX "IX_LyncUserPlans" ON "LyncUserPlans" ("LyncUserPlanId");

CREATE INDEX "LyncUserPlansIdx_ItemID" ON "LyncUserPlans" ("ItemID");

CREATE INDEX "LyncUsersIdx_LyncUserPlanID" ON "LyncUsers" ("LyncUserPlanID");

CREATE INDEX "PackageAddonsIdx_PackageID" ON "PackageAddons" ("PackageID");

CREATE INDEX "PackageAddonsIdx_PlanID" ON "PackageAddons" ("PlanID");

CREATE INDEX "PackageIPAddressesIdx_AddressID" ON "PackageIPAddresses" ("AddressID");

CREATE INDEX "PackageIPAddressesIdx_ItemID" ON "PackageIPAddresses" ("ItemID");

CREATE INDEX "PackageIPAddressesIdx_PackageID" ON "PackageIPAddresses" ("PackageID");

CREATE INDEX "IX_PackageQuotas_QuotaID" ON "PackageQuotas" ("QuotaID");

CREATE INDEX "IX_PackageResources_GroupID" ON "PackageResources" ("GroupID");

CREATE INDEX "PackageIndex_ParentPackageID" ON "Packages" ("ParentPackageID");

CREATE INDEX "PackageIndex_PlanID" ON "Packages" ("PlanID");

CREATE INDEX "PackageIndex_ServerID" ON "Packages" ("ServerID");

CREATE INDEX "PackageIndex_UserID" ON "Packages" ("UserID");

CREATE INDEX "IX_PackagesBandwidth_GroupID" ON "PackagesBandwidth" ("GroupID");

CREATE INDEX "IX_PackagesDiskspace_GroupID" ON "PackagesDiskspace" ("GroupID");

CREATE INDEX "IX_PackageServices_ServiceID" ON "PackageServices" ("ServiceID");

CREATE INDEX "IX_PackagesTreeCache_PackageID" ON "PackagesTreeCache" ("PackageID");

CREATE INDEX "PackageVLANsIdx_PackageID" ON "PackageVLANs" ("PackageID");

CREATE INDEX "PackageVLANsIdx_VlanID" ON "PackageVLANs" ("VlanID");

CREATE INDEX "PrivateIPAddressesIdx_ItemID" ON "PrivateIPAddresses" ("ItemID");

CREATE INDEX "PrivateNetworkVLANsIdx_ServerID" ON "PrivateNetworkVLANs" ("ServerID");

CREATE INDEX "ProvidersIdx_GroupID" ON "Providers" ("GroupID");

CREATE INDEX "QuotasIdx_GroupID" ON "Quotas" ("GroupID");

CREATE INDEX "QuotasIdx_ItemTypeID" ON "Quotas" ("ItemTypeID");

CREATE INDEX "RDSCollectionSettingsIdx_RDSCollectionId" ON "RDSCollectionSettings" ("RDSCollectionId");

CREATE INDEX "RDSCollectionUsersIdx_AccountID" ON "RDSCollectionUsers" ("AccountID");

CREATE INDEX "RDSCollectionUsersIdx_RDSCollectionId" ON "RDSCollectionUsers" ("RDSCollectionId");

CREATE INDEX "RDSMessagesIdx_RDSCollectionId" ON "RDSMessages" ("RDSCollectionId");

CREATE INDEX "RDSServersIdx_RDSCollectionId" ON "RDSServers" ("RDSCollectionId");

CREATE INDEX "ResourceGroupDnsRecordsIdx_GroupID" ON "ResourceGroupDnsRecords" ("GroupID");

CREATE INDEX "ScheduleIdx_PackageID" ON "Schedule" ("PackageID");

CREATE INDEX "ScheduleIdx_TaskID" ON "Schedule" ("TaskID");

CREATE INDEX "IX_ScheduleTaskViewConfiguration_TaskID" ON "ScheduleTaskViewConfiguration" ("TaskID");

CREATE INDEX "ServersIdx_PrimaryGroupID" ON "Servers" ("PrimaryGroupID");

CREATE INDEX "ServiceItemsIdx_ItemTypeID" ON "ServiceItems" ("ItemTypeID");

CREATE INDEX "ServiceItemsIdx_PackageID" ON "ServiceItems" ("PackageID");

CREATE INDEX "ServiceItemsIdx_ServiceID" ON "ServiceItems" ("ServiceID");

CREATE INDEX "ServiceItemTypesIdx_GroupID" ON "ServiceItemTypes" ("GroupID");

CREATE INDEX "ServicesIdx_ClusterID" ON "Services" ("ClusterID");

CREATE INDEX "ServicesIdx_ProviderID" ON "Services" ("ProviderID");

CREATE INDEX "ServicesIdx_ServerID" ON "Services" ("ServerID");

CREATE INDEX "StorageSpaceFoldersIdx_StorageSpaceId" ON "StorageSpaceFolders" ("StorageSpaceId");

CREATE INDEX "StorageSpaceLevelResourceGroupsIdx_GroupId" ON "StorageSpaceLevelResourceGroups" ("GroupId");

CREATE INDEX "StorageSpaceLevelResourceGroupsIdx_LevelId" ON "StorageSpaceLevelResourceGroups" ("LevelId");

CREATE INDEX "StorageSpacesIdx_ServerId" ON "StorageSpaces" ("ServerId");

CREATE INDEX "StorageSpacesIdx_ServiceId" ON "StorageSpaces" ("ServiceId");

CREATE INDEX "IX_TempIds_Created_Scope_Level" ON "TempIds" ("Created", "Scope", "Level");

CREATE INDEX "ThemeSettingsIdx_ThemeID" ON "ThemeSettings" ("ThemeID");

CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" ("Username");

CREATE INDEX "UsersIdx_OwnerID" ON "Users" ("OwnerID");

CREATE INDEX "VirtualGroupsIdx_GroupID" ON "VirtualGroups" ("GroupID");

CREATE INDEX "VirtualGroupsIdx_ServerID" ON "VirtualGroups" ("ServerID");

CREATE INDEX "VirtualServicesIdx_ServerID" ON "VirtualServices" ("ServerID");

CREATE INDEX "VirtualServicesIdx_ServiceID" ON "VirtualServices" ("ServiceID");

CREATE INDEX "WebDavAccessTokensIdx_AccountID" ON "WebDavAccessTokens" ("AccountID");

CREATE INDEX "WebDavPortalUsersSettingsIdx_AccountId" ON "WebDavPortalUsersSettings" ("AccountId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251105040648_v2.0.0', '9.0.9');

DELETE FROM "Quotas"
WHERE "QuotaID" = 770;
SELECT changes();


DELETE FROM "Quotas"
WHERE "QuotaID" = 771;
SELECT changes();


DELETE FROM "ServiceDefaultProperties"
WHERE "PropertyName" = 'UsersHome' AND "ProviderID" = 1;
SELECT changes();


DELETE FROM "ServiceDefaultProperties"
WHERE "PropertyName" = 'UsersHome' AND "ProviderID" = 100;
SELECT changes();


DELETE FROM "ServiceDefaultProperties"
WHERE "PropertyName" = 'UsersHome' AND "ProviderID" = 104;
SELECT changes();


DELETE FROM "Providers"
WHERE "ProviderID" = 1;
SELECT changes();


DELETE FROM "Providers"
WHERE "ProviderID" = 100;
SELECT changes();


DELETE FROM "Providers"
WHERE "ProviderID" = 104;
SELECT changes();


INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251113211755_RemovedOldWindowsProviders', '9.0.9');

CREATE TABLE "BruteForceLogs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_BruteForceLogs" PRIMARY KEY AUTOINCREMENT,
    "IpAddress" TEXT NOT NULL,
    "Username" TEXT NULL,
    "Layer" TEXT NOT NULL,
    "AttemptTime" TEXT NOT NULL,
    "Succeeded" INTEGER NOT NULL,
    "UserAgent" TEXT NULL
);

CREATE TABLE "IpSecurityPolicies" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_IpSecurityPolicies" PRIMARY KEY AUTOINCREMENT,
    "IpRange" TEXT NOT NULL,
    "IsWhitelist" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "ExpiresDate" TEXT NULL,
    "Reason" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "SeverityLevel" INTEGER NOT NULL,
    "CreatedBy" TEXT NULL
);

CREATE INDEX "IX_BruteForceLogs_IpAddress_Layer_AttemptTime" ON "BruteForceLogs" ("IpAddress", "Layer", "AttemptTime");

CREATE INDEX "IX_IpSecurityPolicies_IpRange_IsActive" ON "IpSecurityPolicies" ("IpRange", "IsActive");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260318133000_AddBruteForceProtection', '9.0.9');

COMMIT;

