CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `AdditionalGroups` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `UserID` int NOT NULL,
        `GroupName` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AdditionalGroup` PRIMARY KEY (`ID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `AuditLog` (
        `RecordID` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
        `UserID` int NULL,
        `Username` varchar(50) CHARACTER SET utf8mb4 NULL,
        `ItemID` int NULL,
        `SeverityID` int NOT NULL,
        `StartDate` datetime(6) NOT NULL,
        `FinishDate` datetime(6) NOT NULL,
        `SourceName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `TaskName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `ItemName` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ExecutionLog` TEXT CHARACTER SET utf8mb4 NULL,
        `PackageID` int NULL,
        CONSTRAINT `PK_Log` PRIMARY KEY (`RecordID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `AuditLogSources` (
        `SourceName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_AuditLogSources` PRIMARY KEY (`SourceName`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `AuditLogTasks` (
        `SourceName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `TaskName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `TaskDescription` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_LogActions` PRIMARY KEY (`SourceName`, `TaskName`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `BackgroundTasks` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `Guid` char(36) COLLATE ascii_general_ci NOT NULL,
        `TaskID` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ScheduleID` int NOT NULL,
        `PackageID` int NOT NULL,
        `UserID` int NOT NULL,
        `EffectiveUserID` int NOT NULL,
        `TaskName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ItemID` int NULL,
        `ItemName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `StartDate` datetime(6) NOT NULL,
        `FinishDate` datetime(6) NULL,
        `IndicatorCurrent` int NOT NULL,
        `IndicatorMaximum` int NOT NULL,
        `MaximumExecutionTime` int NOT NULL,
        `Source` longtext CHARACTER SET utf8mb4 NULL,
        `Severity` int NOT NULL,
        `Completed` tinyint(1) NULL,
        `NotifyOnComplete` tinyint(1) NULL,
        `Status` int NOT NULL,
        CONSTRAINT `PK_BackgroundTask` PRIMARY KEY (`ID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Clusters` (
        `ClusterID` int NOT NULL AUTO_INCREMENT,
        `ClusterName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Clusters` PRIMARY KEY (`ClusterID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeDeletedAccounts` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `AccountID` int NOT NULL,
        `OriginAT` int NOT NULL,
        `StoragePath` varchar(255) CHARACTER SET utf8mb4 NULL,
        `FolderName` varchar(128) CHARACTER SET utf8mb4 NULL,
        `FileName` varchar(128) CHARACTER SET utf8mb4 NULL,
        `ExpirationDate` datetime(6) NOT NULL,
        CONSTRAINT `PK_ExchangeDeletedAccount` PRIMARY KEY (`ID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeDisclaimers` (
        `ExchangeDisclaimerId` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `DisclaimerName` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        `DisclaimerText` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_ExchangeDisclaimers` PRIMARY KEY (`ExchangeDisclaimerId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeMailboxPlanRetentionPolicyTags` (
        `PlanTagID` int NOT NULL AUTO_INCREMENT,
        `TagID` int NOT NULL,
        `MailboxPlanId` int NOT NULL,
        CONSTRAINT `PK_ExchangeMailboxPlanRetentionPolicyTag` PRIMARY KEY (`PlanTagID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeRetentionPolicyTags` (
        `TagID` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `TagName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `TagType` int NOT NULL,
        `AgeLimitForRetention` int NOT NULL,
        `RetentionAction` int NOT NULL,
        CONSTRAINT `PK_ExchangeRetentionPolicyTag` PRIMARY KEY (`TagID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `OCSUsers` (
        `OCSUserID` int NOT NULL AUTO_INCREMENT,
        `AccountID` int NOT NULL,
        `InstanceID` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NOT NULL,
        CONSTRAINT `PK_OCSUsers` PRIMARY KEY (`OCSUserID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PackageSettings` (
        `PackageID` int NOT NULL,
        `SettingsName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyValue` TEXT CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_PackageSettings` PRIMARY KEY (`PackageID`, `SettingsName`, `PropertyName`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `RDSCertificates` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `ServiceId` int NOT NULL,
        `Content` TEXT CHARACTER SET utf8mb4 NOT NULL,
        `Hash` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `FileName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ValidFrom` datetime(6) NULL,
        `ExpiryDate` datetime(6) NULL,
        CONSTRAINT `PK_RDSCertificates` PRIMARY KEY (`ID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `RDSCollections` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Description` varchar(255) CHARACTER SET utf8mb4 NULL,
        `DisplayName` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_RdsCollection` PRIMARY KEY (`ID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `RDSServerSettings` (
        `RdsServerId` int NOT NULL,
        `SettingsName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyValue` TEXT CHARACTER SET utf8mb4 NULL,
        `ApplyUsers` tinyint(1) NOT NULL,
        `ApplyAdministrators` tinyint(1) NOT NULL,
        CONSTRAINT `PK_RDSServerSettings` PRIMARY KEY (`RdsServerId`, `SettingsName`, `PropertyName`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ResourceGroups` (
        `GroupID` int NOT NULL,
        `GroupName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `GroupOrder` int NOT NULL DEFAULT 1,
        `GroupController` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `ShowGroup` tinyint(1) NULL,
        CONSTRAINT `PK_ResourceGroups` PRIMARY KEY (`GroupID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ScheduleTasks` (
        `TaskID` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `TaskType` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
        `RoleID` int NOT NULL,
        CONSTRAINT `PK_ScheduleTasks` PRIMARY KEY (`TaskID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `SfBUserPlans` (
        `SfBUserPlanId` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `SfBUserPlanName` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        `SfBUserPlanType` int NULL,
        `IM` tinyint(1) NOT NULL,
        `Mobility` tinyint(1) NOT NULL,
        `MobilityEnableOutsideVoice` tinyint(1) NOT NULL,
        `Federation` tinyint(1) NOT NULL,
        `Conferencing` tinyint(1) NOT NULL,
        `EnterpriseVoice` tinyint(1) NOT NULL,
        `VoicePolicy` int NOT NULL,
        `IsDefault` tinyint(1) NOT NULL,
        `RemoteUserAccess` tinyint(1) NOT NULL DEFAULT FALSE,
        `PublicIMConnectivity` tinyint(1) NOT NULL DEFAULT FALSE,
        `AllowOrganizeMeetingsWithExternalAnonymous` tinyint(1) NOT NULL DEFAULT FALSE,
        `Telephony` int NULL,
        `ServerURI` varchar(300) CHARACTER SET utf8mb4 NULL,
        `ArchivePolicy` varchar(300) CHARACTER SET utf8mb4 NULL,
        `TelephonyDialPlanPolicy` varchar(300) CHARACTER SET utf8mb4 NULL,
        `TelephonyVoicePolicy` varchar(300) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_SfBUserPlans` PRIMARY KEY (`SfBUserPlanId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `SfBUsers` (
        `SfBUserID` int NOT NULL AUTO_INCREMENT,
        `AccountID` int NOT NULL,
        `SfBUserPlanID` int NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NOT NULL,
        `SipAddress` varchar(300) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_SfBUsers` PRIMARY KEY (`SfBUserID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `SSLCertificates` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `UserID` int NOT NULL,
        `SiteID` int NOT NULL,
        `FriendlyName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Hostname` varchar(255) CHARACTER SET utf8mb4 NULL,
        `DistinguishedName` varchar(500) CHARACTER SET utf8mb4 NULL,
        `CSR` TEXT CHARACTER SET utf8mb4 NULL,
        `CSRLength` int NULL,
        `Certificate` TEXT CHARACTER SET utf8mb4 NULL,
        `Hash` TEXT CHARACTER SET utf8mb4 NULL,
        `Installed` tinyint(1) NULL,
        `IsRenewal` tinyint(1) NULL,
        `ValidFrom` datetime(6) NULL,
        `ExpiryDate` datetime(6) NULL,
        `SerialNumber` varchar(250) CHARACTER SET utf8mb4 NULL,
        `Pfx` TEXT CHARACTER SET utf8mb4 NULL,
        `PreviousId` int NULL,
        CONSTRAINT `PK_SSLCertificates` PRIMARY KEY (`ID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `StorageSpaceLevels` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        `Description` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_StorageSpaceLevel` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `SupportServiceLevels` (
        `LevelID` int NOT NULL AUTO_INCREMENT,
        `LevelName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `LevelDescription` varchar(1000) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_SupportServiceLevel` PRIMARY KEY (`LevelID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `SystemSettings` (
        `SettingsName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyValue` TEXT CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_SystemSettings` PRIMARY KEY (`SettingsName`, `PropertyName`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `TempIds` (
        `Key` int NOT NULL AUTO_INCREMENT,
        `Created` datetime(6) NOT NULL,
        `Scope` char(36) COLLATE ascii_general_ci NOT NULL,
        `Level` int NOT NULL,
        `Id` int NOT NULL,
        `Date` datetime(6) NOT NULL,
        CONSTRAINT `PK_TempIds` PRIMARY KEY (`Key`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Themes` (
        `ThemeID` int NOT NULL AUTO_INCREMENT,
        `DisplayName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `LTRName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RTLName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Enabled` int NOT NULL,
        `DisplayOrder` int NOT NULL,
        CONSTRAINT `PK_Themes` PRIMARY KEY (`ThemeID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ThemeSettings` (
        `ThemeSettingID` int NOT NULL AUTO_INCREMENT,
        `ThemeID` int NOT NULL,
        `SettingsName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyValue` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_ThemeSettings` PRIMARY KEY (`ThemeSettingID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Users` (
        `UserID` int NOT NULL AUTO_INCREMENT,
        `OwnerID` int NULL,
        `RoleID` int NOT NULL,
        `StatusID` int NOT NULL,
        `IsDemo` tinyint(1) NOT NULL DEFAULT FALSE,
        `IsPeer` tinyint(1) NOT NULL DEFAULT FALSE,
        `Username` varchar(50) CHARACTER SET utf8mb4 NULL,
        `Password` varchar(200) CHARACTER SET utf8mb4 NULL,
        `FirstName` varchar(50) CHARACTER SET utf8mb4 NULL,
        `LastName` varchar(50) CHARACTER SET utf8mb4 NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Created` datetime(6) NULL,
        `Changed` datetime(6) NULL,
        `Comments` longtext CHARACTER SET utf8mb4 NULL,
        `SecondaryEmail` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Address` varchar(200) CHARACTER SET utf8mb4 NULL,
        `City` varchar(50) CHARACTER SET utf8mb4 NULL,
        `State` varchar(50) CHARACTER SET utf8mb4 NULL,
        `Country` varchar(50) CHARACTER SET utf8mb4 NULL,
        `Zip` varchar(20) CHARACTER SET utf8mb4 NULL,
        `PrimaryPhone` varchar(30) CHARACTER SET utf8mb4 NULL,
        `SecondaryPhone` varchar(30) CHARACTER SET utf8mb4 NULL,
        `Fax` varchar(30) CHARACTER SET utf8mb4 NULL,
        `InstantMessenger` varchar(100) CHARACTER SET utf8mb4 NULL,
        `HtmlMail` tinyint(1) NULL DEFAULT TRUE,
        `CompanyName` varchar(100) CHARACTER SET utf8mb4 NULL,
        `EcommerceEnabled` tinyint(1) NULL,
        `AdditionalParams` longtext CHARACTER SET utf8mb4 NULL,
        `LoginStatusId` int NULL,
        `FailedLogins` int NULL,
        `SubscriberNumber` varchar(32) CHARACTER SET utf8mb4 NULL,
        `OneTimePasswordState` int NULL,
        `MfaMode` int NOT NULL DEFAULT 0,
        `PinSecret` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Users` PRIMARY KEY (`UserID`),
        CONSTRAINT `FK_Users_Users` FOREIGN KEY (`OwnerID`) REFERENCES `Users` (`UserID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Versions` (
        `DatabaseVersion` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `BuildDate` datetime(6) NOT NULL,
        CONSTRAINT `PK_Versions` PRIMARY KEY (`DatabaseVersion`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `BackgroundTaskLogs` (
        `LogID` int NOT NULL AUTO_INCREMENT,
        `TaskID` int NOT NULL,
        `Date` datetime(6) NULL,
        `ExceptionStackTrace` TEXT CHARACTER SET utf8mb4 NULL,
        `InnerTaskStart` int NULL,
        `Severity` int NULL,
        `Text` TEXT CHARACTER SET utf8mb4 NULL,
        `TextIdent` int NULL,
        `XmlParameters` TEXT CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_BackgroundTaskLog` PRIMARY KEY (`LogID`),
        CONSTRAINT `FK_BackgroundTaskLog_Task` FOREIGN KEY (`TaskID`) REFERENCES `BackgroundTasks` (`ID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `BackgroundTaskParameters` (
        `ParameterID` int NOT NULL AUTO_INCREMENT,
        `TaskID` int NOT NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NULL,
        `SerializerValue` TEXT CHARACTER SET utf8mb4 NULL,
        `TypeName` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_BackgroundTaskParameter` PRIMARY KEY (`ParameterID`),
        CONSTRAINT `FK_BackgrounTaskParameter_Task` FOREIGN KEY (`TaskID`) REFERENCES `BackgroundTasks` (`ID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `BackgroundTaskStack` (
        `TaskStackID` int NOT NULL AUTO_INCREMENT,
        `TaskID` int NOT NULL,
        CONSTRAINT `PK_BackgroundTaskStack` PRIMARY KEY (`TaskStackID`),
        CONSTRAINT `FK_BackgroundTaskStack_Task` FOREIGN KEY (`TaskID`) REFERENCES `BackgroundTasks` (`ID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `RDSCollectionSettings` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `RDSCollectionId` int NOT NULL,
        `DisconnectedSessionLimitMin` int NULL,
        `ActiveSessionLimitMin` int NULL,
        `IdleSessionLimitMin` int NULL,
        `BrokenConnectionAction` varchar(20) CHARACTER SET utf8mb4 NULL,
        `AutomaticReconnectionEnabled` tinyint(1) NULL,
        `TemporaryFoldersDeletedOnExit` tinyint(1) NULL,
        `TemporaryFoldersPerSession` tinyint(1) NULL,
        `ClientDeviceRedirectionOptions` varchar(250) CHARACTER SET utf8mb4 NULL,
        `ClientPrinterRedirected` tinyint(1) NULL,
        `ClientPrinterAsDefault` tinyint(1) NULL,
        `RDEasyPrintDriverEnabled` tinyint(1) NULL,
        `MaxRedirectedMonitors` int NULL,
        `SecurityLayer` varchar(20) CHARACTER SET utf8mb4 NULL,
        `EncryptionLevel` varchar(20) CHARACTER SET utf8mb4 NULL,
        `AuthenticateUsingNLA` tinyint(1) NULL,
        CONSTRAINT `PK_RDSCollectionSettings` PRIMARY KEY (`ID`),
        CONSTRAINT `FK_RDSCollectionSettings_RDSCollections` FOREIGN KEY (`RDSCollectionId`) REFERENCES `RDSCollections` (`ID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `RDSMessages` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `RDSCollectionId` int NOT NULL,
        `MessageText` TEXT CHARACTER SET utf8mb4 NOT NULL,
        `UserName` char(250) CHARACTER SET utf8mb4 NOT NULL,
        `Date` datetime(6) NOT NULL,
        CONSTRAINT `PK_RDSMessages` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_RDSMessages_RDSCollections` FOREIGN KEY (`RDSCollectionId`) REFERENCES `RDSCollections` (`ID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `RDSServers` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NULL,
        `FqdName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Description` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RDSCollectionId` int NULL,
        `ConnectionEnabled` tinyint(1) NOT NULL DEFAULT TRUE,
        `Controller` int NULL,
        CONSTRAINT `PK_RdsServer` PRIMARY KEY (`ID`),
        CONSTRAINT `FK_RDSServers_RDSCollectionId` FOREIGN KEY (`RDSCollectionId`) REFERENCES `RDSCollections` (`ID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Providers` (
        `ProviderID` int NOT NULL,
        `GroupID` int NOT NULL,
        `ProviderName` varchar(100) CHARACTER SET utf8mb4 NULL,
        `DisplayName` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `ProviderType` varchar(400) CHARACTER SET utf8mb4 NULL,
        `EditorControl` varchar(100) CHARACTER SET utf8mb4 NULL,
        `DisableAutoDiscovery` tinyint(1) NULL,
        CONSTRAINT `PK_Provider` PRIMARY KEY (`ProviderID`),
        CONSTRAINT `FK_Providers_ResourceGroups` FOREIGN KEY (`GroupID`) REFERENCES `ResourceGroups` (`GroupID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ResourceGroupDnsRecords` (
        `RecordID` int NOT NULL AUTO_INCREMENT,
        `RecordOrder` int NOT NULL DEFAULT 1,
        `GroupID` int NOT NULL,
        `RecordType` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `RecordName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `RecordData` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `MXPriority` int NULL,
        CONSTRAINT `PK_ResourceGroupDnsRecords` PRIMARY KEY (`RecordID`),
        CONSTRAINT `FK_ResourceGroupDnsRecords_ResourceGroups` FOREIGN KEY (`GroupID`) REFERENCES `ResourceGroups` (`GroupID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Servers` (
        `ServerID` int NOT NULL AUTO_INCREMENT,
        `ServerName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `ServerUrl` varchar(255) CHARACTER SET utf8mb4 NULL DEFAULT '',
        `Password` varchar(100) CHARACTER SET utf8mb4 NULL,
        `Comments` TEXT CHARACTER SET utf8mb4 NULL,
        `VirtualServer` tinyint(1) NOT NULL DEFAULT FALSE,
        `InstantDomainAlias` varchar(200) CHARACTER SET utf8mb4 NULL,
        `PrimaryGroupID` int NULL,
        `ADRootDomain` varchar(200) CHARACTER SET utf8mb4 NULL,
        `ADUsername` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ADPassword` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ADAuthenticationType` varchar(50) CHARACTER SET utf8mb4 NULL,
        `ADEnabled` tinyint(1) NULL DEFAULT FALSE,
        `ADParentDomain` varchar(200) CHARACTER SET utf8mb4 NULL,
        `ADParentDomainController` varchar(200) CHARACTER SET utf8mb4 NULL,
        `OSPlatform` int NOT NULL DEFAULT 0,
        `IsCore` tinyint(1) NULL,
        `PasswordIsSHA256` tinyint(1) NOT NULL DEFAULT FALSE,
        CONSTRAINT `PK_Servers` PRIMARY KEY (`ServerID`),
        CONSTRAINT `FK_Servers_ResourceGroups` FOREIGN KEY (`PrimaryGroupID`) REFERENCES `ResourceGroups` (`GroupID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ServiceItemTypes` (
        `ItemTypeID` int NOT NULL,
        `GroupID` int NULL,
        `DisplayName` varchar(50) CHARACTER SET utf8mb4 NULL,
        `TypeName` varchar(200) CHARACTER SET utf8mb4 NULL,
        `TypeOrder` int NOT NULL DEFAULT 1,
        `CalculateDiskspace` tinyint(1) NULL,
        `CalculateBandwidth` tinyint(1) NULL,
        `Suspendable` tinyint(1) NULL,
        `Disposable` tinyint(1) NULL,
        `Searchable` tinyint(1) NULL,
        `Importable` tinyint(1) NOT NULL DEFAULT TRUE,
        `Backupable` tinyint(1) NOT NULL DEFAULT TRUE,
        CONSTRAINT `PK_ServiceItemTypes` PRIMARY KEY (`ItemTypeID`),
        CONSTRAINT `FK_ServiceItemTypes_ResourceGroups` FOREIGN KEY (`GroupID`) REFERENCES `ResourceGroups` (`GroupID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ScheduleTaskParameters` (
        `TaskID` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `ParameterID` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `DataTypeID` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `DefaultValue` longtext CHARACTER SET utf8mb4 NULL,
        `ParameterOrder` int NOT NULL DEFAULT 0,
        CONSTRAINT `PK_ScheduleTaskParameters` PRIMARY KEY (`TaskID`, `ParameterID`),
        CONSTRAINT `FK_ScheduleTaskParameters_ScheduleTasks` FOREIGN KEY (`TaskID`) REFERENCES `ScheduleTasks` (`TaskID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ScheduleTaskViewConfiguration` (
        `TaskID` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `ConfigurationID` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Environment` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_ScheduleTaskViewConfiguration` PRIMARY KEY (`ConfigurationID`, `TaskID`),
        CONSTRAINT `FK_ScheduleTaskViewConfiguration_ScheduleTaskViewConfiguration` FOREIGN KEY (`TaskID`) REFERENCES `ScheduleTasks` (`TaskID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `StorageSpaceLevelResourceGroups` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `LevelId` int NOT NULL,
        `GroupId` int NOT NULL,
        CONSTRAINT `PK_StorageSpaceLevelResourceGroup` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_StorageSpaceLevelResourceGroups_GroupId` FOREIGN KEY (`GroupId`) REFERENCES `ResourceGroups` (`GroupID`) ON DELETE CASCADE,
        CONSTRAINT `FK_StorageSpaceLevelResourceGroups_LevelId` FOREIGN KEY (`LevelId`) REFERENCES `StorageSpaceLevels` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Comments` (
        `CommentID` int NOT NULL AUTO_INCREMENT,
        `ItemTypeID` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `ItemID` int NOT NULL,
        `UserID` int NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `CommentText` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `SeverityID` int NULL,
        CONSTRAINT `PK_Comments` PRIMARY KEY (`CommentID`),
        CONSTRAINT `FK_Comments_Users` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `UserSettings` (
        `UserID` int NOT NULL,
        `SettingsName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyValue` TEXT CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_UserSettings` PRIMARY KEY (`UserID`, `SettingsName`, `PropertyName`),
        CONSTRAINT `FK_UserSettings_Users` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ServiceDefaultProperties` (
        `ProviderID` int NOT NULL,
        `PropertyName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyValue` varchar(1000) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_ServiceDefaultProperties` PRIMARY KEY (`ProviderID`, `PropertyName`),
        CONSTRAINT `FK_ServiceDefaultProperties_Providers` FOREIGN KEY (`ProviderID`) REFERENCES `Providers` (`ProviderID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `HostingPlans` (
        `PlanID` int NOT NULL AUTO_INCREMENT,
        `UserID` int NULL,
        `PackageID` int NULL,
        `ServerID` int NULL,
        `PlanName` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `PlanDescription` TEXT CHARACTER SET utf8mb4 NULL,
        `Available` tinyint(1) NOT NULL,
        `SetupPrice` decimal(65,30) NULL,
        `RecurringPrice` decimal(65,30) NULL,
        `RecurrenceUnit` int NULL,
        `RecurrenceLength` int NULL,
        `IsAddon` tinyint(1) NULL,
        CONSTRAINT `PK_HostingPlans` PRIMARY KEY (`PlanID`),
        CONSTRAINT `FK_HostingPlans_Servers` FOREIGN KEY (`ServerID`) REFERENCES `Servers` (`ServerID`),
        CONSTRAINT `FK_HostingPlans_Users` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `IPAddresses` (
        `AddressID` int NOT NULL AUTO_INCREMENT,
        `ExternalIP` varchar(24) CHARACTER SET utf8mb4 NOT NULL,
        `InternalIP` varchar(24) CHARACTER SET utf8mb4 NULL,
        `ServerID` int NULL,
        `Comments` TEXT CHARACTER SET utf8mb4 NULL,
        `SubnetMask` varchar(15) CHARACTER SET utf8mb4 NULL,
        `DefaultGateway` varchar(15) CHARACTER SET utf8mb4 NULL,
        `PoolID` int NULL,
        `VLAN` int NULL,
        CONSTRAINT `PK_IPAddresses` PRIMARY KEY (`AddressID`),
        CONSTRAINT `FK_IPAddresses_Servers` FOREIGN KEY (`ServerID`) REFERENCES `Servers` (`ServerID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PrivateNetworkVLANs` (
        `VlanID` int NOT NULL AUTO_INCREMENT,
        `Vlan` int NOT NULL,
        `ServerID` int NULL,
        `Comments` TEXT CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_PrivateNetworkVlan` PRIMARY KEY (`VlanID`),
        CONSTRAINT `FK_ServerID` FOREIGN KEY (`ServerID`) REFERENCES `Servers` (`ServerID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Services` (
        `ServiceID` int NOT NULL AUTO_INCREMENT,
        `ServerID` int NOT NULL,
        `ProviderID` int NOT NULL,
        `ServiceName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `Comments` TEXT CHARACTER SET utf8mb4 NULL,
        `ServiceQuotaValue` int NULL,
        `ClusterID` int NULL,
        CONSTRAINT `PK_Services` PRIMARY KEY (`ServiceID`),
        CONSTRAINT `FK_Services_Clusters` FOREIGN KEY (`ClusterID`) REFERENCES `Clusters` (`ClusterID`),
        CONSTRAINT `FK_Services_Providers` FOREIGN KEY (`ProviderID`) REFERENCES `Providers` (`ProviderID`),
        CONSTRAINT `FK_Services_Servers` FOREIGN KEY (`ServerID`) REFERENCES `Servers` (`ServerID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `VirtualGroups` (
        `VirtualGroupID` int NOT NULL AUTO_INCREMENT,
        `ServerID` int NOT NULL,
        `GroupID` int NOT NULL,
        `DistributionType` int NULL,
        `BindDistributionToPrimary` tinyint(1) NULL,
        CONSTRAINT `PK_VirtualGroups` PRIMARY KEY (`VirtualGroupID`),
        CONSTRAINT `FK_VirtualGroups_ResourceGroups` FOREIGN KEY (`GroupID`) REFERENCES `ResourceGroups` (`GroupID`),
        CONSTRAINT `FK_VirtualGroups_Servers` FOREIGN KEY (`ServerID`) REFERENCES `Servers` (`ServerID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Quotas` (
        `QuotaID` int NOT NULL,
        `GroupID` int NOT NULL,
        `QuotaOrder` double NOT NULL DEFAULT 1.0,
        `QuotaName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `QuotaDescription` varchar(200) CHARACTER SET utf8mb4 NULL,
        `QuotaTypeID` int NOT NULL DEFAULT 2,
        `ServiceQuota` tinyint(1) NULL DEFAULT FALSE,
        `ItemTypeID` int NULL,
        `HideQuota` tinyint(1) NULL,
        `PerOrganization` int NULL,
        CONSTRAINT `PK_Quotas` PRIMARY KEY (`QuotaID`),
        CONSTRAINT `FK_Quotas_ResourceGroups` FOREIGN KEY (`GroupID`) REFERENCES `ResourceGroups` (`GroupID`) ON DELETE CASCADE,
        CONSTRAINT `FK_Quotas_ServiceItemTypes` FOREIGN KEY (`ItemTypeID`) REFERENCES `ServiceItemTypes` (`ItemTypeID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `HostingPlanResources` (
        `PlanID` int NOT NULL,
        `GroupID` int NOT NULL,
        `CalculateDiskSpace` tinyint(1) NULL,
        `CalculateBandwidth` tinyint(1) NULL,
        CONSTRAINT `PK_HostingPlanResources` PRIMARY KEY (`PlanID`, `GroupID`),
        CONSTRAINT `FK_HostingPlanResources_HostingPlans` FOREIGN KEY (`PlanID`) REFERENCES `HostingPlans` (`PlanID`) ON DELETE CASCADE,
        CONSTRAINT `FK_HostingPlanResources_ResourceGroups` FOREIGN KEY (`GroupID`) REFERENCES `ResourceGroups` (`GroupID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Packages` (
        `PackageID` int NOT NULL AUTO_INCREMENT,
        `ParentPackageID` int NULL,
        `UserID` int NOT NULL,
        `PackageName` varchar(300) CHARACTER SET utf8mb4 NULL,
        `PackageComments` TEXT CHARACTER SET utf8mb4 NULL,
        `ServerID` int NULL,
        `StatusID` int NOT NULL,
        `PlanID` int NULL,
        `PurchaseDate` datetime(6) NULL,
        `OverrideQuotas` tinyint(1) NOT NULL DEFAULT FALSE,
        `BandwidthUpdated` datetime(6) NULL,
        `DefaultTopPackage` tinyint(1) NOT NULL DEFAULT FALSE,
        `StatusIDchangeDate` datetime(6) NOT NULL,
        CONSTRAINT `PK_Packages` PRIMARY KEY (`PackageID`),
        CONSTRAINT `FK_Packages_HostingPlans` FOREIGN KEY (`PlanID`) REFERENCES `HostingPlans` (`PlanID`) ON DELETE CASCADE,
        CONSTRAINT `FK_Packages_Packages` FOREIGN KEY (`ParentPackageID`) REFERENCES `Packages` (`PackageID`),
        CONSTRAINT `FK_Packages_Servers` FOREIGN KEY (`ServerID`) REFERENCES `Servers` (`ServerID`),
        CONSTRAINT `FK_Packages_Users` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ServiceProperties` (
        `ServiceID` int NOT NULL,
        `PropertyName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyValue` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_ServiceProperties` PRIMARY KEY (`ServiceID`, `PropertyName`),
        CONSTRAINT `FK_ServiceProperties_Services` FOREIGN KEY (`ServiceID`) REFERENCES `Services` (`ServiceID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `StorageSpaces` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        `ServiceId` int NOT NULL,
        `ServerId` int NOT NULL,
        `LevelId` int NOT NULL,
        `Path` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsShared` tinyint(1) NOT NULL,
        `UncPath` longtext CHARACTER SET utf8mb4 NULL,
        `FsrmQuotaType` int NOT NULL,
        `FsrmQuotaSizeBytes` bigint NOT NULL,
        `IsDisabled` tinyint(1) NOT NULL DEFAULT FALSE,
        CONSTRAINT `PK_StorageSpace` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_StorageSpaces_ServerId` FOREIGN KEY (`ServerId`) REFERENCES `Servers` (`ServerID`) ON DELETE CASCADE,
        CONSTRAINT `FK_StorageSpaces_ServiceId` FOREIGN KEY (`ServiceId`) REFERENCES `Services` (`ServiceID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `VirtualServices` (
        `VirtualServiceID` int NOT NULL AUTO_INCREMENT,
        `ServerID` int NOT NULL,
        `ServiceID` int NOT NULL,
        CONSTRAINT `PK_VirtualServices` PRIMARY KEY (`VirtualServiceID`),
        CONSTRAINT `FK_VirtualServices_Servers` FOREIGN KEY (`ServerID`) REFERENCES `Servers` (`ServerID`) ON DELETE CASCADE,
        CONSTRAINT `FK_VirtualServices_Services` FOREIGN KEY (`ServiceID`) REFERENCES `Services` (`ServiceID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `HostingPlanQuotas` (
        `PlanID` int NOT NULL,
        `QuotaID` int NOT NULL,
        `QuotaValue` int NOT NULL,
        CONSTRAINT `PK_HostingPlanQuota` PRIMARY KEY (`PlanID`, `QuotaID`),
        CONSTRAINT `FK_HostingPlanQuotas_HostingPlans` FOREIGN KEY (`PlanID`) REFERENCES `HostingPlans` (`PlanID`) ON DELETE CASCADE,
        CONSTRAINT `FK_HostingPlanQuotas_Quotas` FOREIGN KEY (`QuotaID`) REFERENCES `Quotas` (`QuotaID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `GlobalDnsRecords` (
        `RecordID` int NOT NULL AUTO_INCREMENT,
        `RecordType` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `RecordName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `RecordData` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
        `MXPriority` int NOT NULL,
        `ServiceID` int NULL,
        `ServerID` int NULL,
        `PackageID` int NULL,
        `IPAddressID` int NULL,
        `SrvPriority` int NULL,
        `SrvWeight` int NULL,
        `SrvPort` int NULL,
        CONSTRAINT `PK_GlobalDnsRecord` PRIMARY KEY (`RecordID`),
        CONSTRAINT `FK_GlobalDnsRecords_IPAddresses` FOREIGN KEY (`IPAddressID`) REFERENCES `IPAddresses` (`AddressID`),
        CONSTRAINT `FK_GlobalDnsRecords_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`) ON DELETE CASCADE,
        CONSTRAINT `FK_GlobalDnsRecords_Servers` FOREIGN KEY (`ServerID`) REFERENCES `Servers` (`ServerID`),
        CONSTRAINT `FK_GlobalDnsRecords_Services` FOREIGN KEY (`ServiceID`) REFERENCES `Services` (`ServiceID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PackageAddons` (
        `PackageAddonID` int NOT NULL AUTO_INCREMENT,
        `PackageID` int NULL,
        `PlanID` int NULL,
        `Quantity` int NULL,
        `PurchaseDate` datetime(6) NULL,
        `Comments` TEXT CHARACTER SET utf8mb4 NULL,
        `StatusID` int NULL,
        CONSTRAINT `PK_PackageAddons` PRIMARY KEY (`PackageAddonID`),
        CONSTRAINT `FK_PackageAddons_HostingPlans` FOREIGN KEY (`PlanID`) REFERENCES `HostingPlans` (`PlanID`),
        CONSTRAINT `FK_PackageAddons_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PackageQuotas` (
        `PackageID` int NOT NULL,
        `QuotaID` int NOT NULL,
        `QuotaValue` int NOT NULL,
        CONSTRAINT `PK_PackageQuotas` PRIMARY KEY (`PackageID`, `QuotaID`),
        CONSTRAINT `FK_PackageQuotas_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`),
        CONSTRAINT `FK_PackageQuotas_Quotas` FOREIGN KEY (`QuotaID`) REFERENCES `Quotas` (`QuotaID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PackageResources` (
        `PackageID` int NOT NULL,
        `GroupID` int NOT NULL,
        `CalculateDiskspace` tinyint(1) NOT NULL,
        `CalculateBandwidth` tinyint(1) NOT NULL,
        CONSTRAINT `PK_PackageResources` PRIMARY KEY (`PackageID`, `GroupID`),
        CONSTRAINT `FK_PackageResources_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`),
        CONSTRAINT `FK_PackageResources_ResourceGroups` FOREIGN KEY (`GroupID`) REFERENCES `ResourceGroups` (`GroupID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PackagesBandwidth` (
        `PackageID` int NOT NULL,
        `GroupID` int NOT NULL,
        `LogDate` datetime(6) NOT NULL,
        `BytesSent` bigint NOT NULL,
        `BytesReceived` bigint NOT NULL,
        CONSTRAINT `PK_PackagesBandwidth` PRIMARY KEY (`PackageID`, `GroupID`, `LogDate`),
        CONSTRAINT `FK_PackagesBandwidth_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`),
        CONSTRAINT `FK_PackagesBandwidth_ResourceGroups` FOREIGN KEY (`GroupID`) REFERENCES `ResourceGroups` (`GroupID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PackagesDiskspace` (
        `PackageID` int NOT NULL,
        `GroupID` int NOT NULL,
        `DiskSpace` bigint NOT NULL,
        CONSTRAINT `PK_PackagesDiskspace` PRIMARY KEY (`PackageID`, `GroupID`),
        CONSTRAINT `FK_PackagesDiskspace_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`),
        CONSTRAINT `FK_PackagesDiskspace_ResourceGroups` FOREIGN KEY (`GroupID`) REFERENCES `ResourceGroups` (`GroupID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PackageServices` (
        `PackageID` int NOT NULL,
        `ServiceID` int NOT NULL,
        CONSTRAINT `PK_PackageServices` PRIMARY KEY (`PackageID`, `ServiceID`),
        CONSTRAINT `FK_PackageServices_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`),
        CONSTRAINT `FK_PackageServices_Services` FOREIGN KEY (`ServiceID`) REFERENCES `Services` (`ServiceID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PackagesTreeCache` (
        `ParentPackageID` int NOT NULL,
        `PackageID` int NOT NULL,
        CONSTRAINT `PK_PackagesTreeCache` PRIMARY KEY (`ParentPackageID`, `PackageID`),
        CONSTRAINT `FK_PackagesTreeCache_Packages` FOREIGN KEY (`ParentPackageID`) REFERENCES `Packages` (`PackageID`),
        CONSTRAINT `FK_PackagesTreeCache_Packages1` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PackageVLANs` (
        `PackageVlanID` int NOT NULL AUTO_INCREMENT,
        `VlanID` int NOT NULL,
        `PackageID` int NOT NULL,
        `IsDmz` tinyint(1) NOT NULL DEFAULT FALSE,
        CONSTRAINT `PK_PackageVlan` PRIMARY KEY (`PackageVlanID`),
        CONSTRAINT `FK_PackageID` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`) ON DELETE CASCADE,
        CONSTRAINT `FK_VlanID` FOREIGN KEY (`VlanID`) REFERENCES `PrivateNetworkVLANs` (`VlanID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Schedule` (
        `ScheduleID` int NOT NULL AUTO_INCREMENT,
        `TaskID` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `PackageID` int NULL,
        `ScheduleName` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ScheduleTypeID` varchar(50) CHARACTER SET utf8mb4 NULL,
        `Interval` int NULL,
        `FromTime` datetime(6) NULL,
        `ToTime` datetime(6) NULL,
        `StartTime` datetime(6) NULL,
        `LastRun` datetime(6) NULL,
        `NextRun` datetime(6) NULL,
        `Enabled` tinyint(1) NOT NULL,
        `PriorityID` varchar(50) CHARACTER SET utf8mb4 NULL,
        `HistoriesNumber` int NULL,
        `MaxExecutionTime` int NULL,
        `WeekMonthDay` int NULL,
        CONSTRAINT `PK_Schedule` PRIMARY KEY (`ScheduleID`),
        CONSTRAINT `FK_Schedule_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`) ON DELETE CASCADE,
        CONSTRAINT `FK_Schedule_ScheduleTasks` FOREIGN KEY (`TaskID`) REFERENCES `ScheduleTasks` (`TaskID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ServiceItems` (
        `ItemID` int NOT NULL AUTO_INCREMENT,
        `PackageID` int NULL,
        `ItemTypeID` int NULL,
        `ServiceID` int NULL,
        `ItemName` varchar(500) CHARACTER SET utf8mb4 NULL,
        `CreatedDate` datetime(6) NULL,
        CONSTRAINT `PK_ServiceItems` PRIMARY KEY (`ItemID`),
        CONSTRAINT `FK_ServiceItems_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`),
        CONSTRAINT `FK_ServiceItems_ServiceItemTypes` FOREIGN KEY (`ItemTypeID`) REFERENCES `ServiceItemTypes` (`ItemTypeID`),
        CONSTRAINT `FK_ServiceItems_Services` FOREIGN KEY (`ServiceID`) REFERENCES `Services` (`ServiceID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `StorageSpaceFolders` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        `StorageSpaceId` int NOT NULL,
        `Path` longtext CHARACTER SET utf8mb4 NOT NULL,
        `UncPath` longtext CHARACTER SET utf8mb4 NULL,
        `IsShared` tinyint(1) NOT NULL,
        `FsrmQuotaType` int NOT NULL,
        `FsrmQuotaSizeBytes` bigint NOT NULL,
        CONSTRAINT `PK_StorageSpaceFolder` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_StorageSpaceFolders_StorageSpaceId` FOREIGN KEY (`StorageSpaceId`) REFERENCES `StorageSpaces` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ScheduleParameters` (
        `ScheduleID` int NOT NULL,
        `ParameterID` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `ParameterValue` varchar(1000) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_ScheduleParameters` PRIMARY KEY (`ScheduleID`, `ParameterID`),
        CONSTRAINT `FK_ScheduleParameters_Schedule` FOREIGN KEY (`ScheduleID`) REFERENCES `Schedule` (`ScheduleID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `DmzIPAddresses` (
        `DmzAddressID` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `IPAddress` varchar(15) CHARACTER SET utf8mb4 NOT NULL,
        `IsPrimary` tinyint(1) NOT NULL,
        CONSTRAINT `PK_DmzIPAddresses` PRIMARY KEY (`DmzAddressID`),
        CONSTRAINT `FK_DmzIPAddresses_ServiceItems` FOREIGN KEY (`ItemID`) REFERENCES `ServiceItems` (`ItemID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `Domains` (
        `DomainID` int NOT NULL AUTO_INCREMENT,
        `PackageID` int NOT NULL,
        `ZoneItemID` int NULL,
        `DomainName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `HostingAllowed` tinyint(1) NOT NULL DEFAULT FALSE,
        `WebSiteID` int NULL,
        `MailDomainID` int NULL,
        `IsSubDomain` tinyint(1) NOT NULL DEFAULT FALSE,
        `IsPreviewDomain` tinyint(1) NOT NULL DEFAULT FALSE,
        `IsDomainPointer` tinyint(1) NOT NULL,
        `DomainItemId` int NULL,
        `CreationDate` datetime(6) NULL,
        `ExpirationDate` datetime(6) NULL,
        `LastUpdateDate` datetime(6) NULL,
        `RegistrarName` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Domains` PRIMARY KEY (`DomainID`),
        CONSTRAINT `FK_Domains_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`) ON DELETE CASCADE,
        CONSTRAINT `FK_Domains_ServiceItems_MailDomain` FOREIGN KEY (`MailDomainID`) REFERENCES `ServiceItems` (`ItemID`),
        CONSTRAINT `FK_Domains_ServiceItems_WebSite` FOREIGN KEY (`WebSiteID`) REFERENCES `ServiceItems` (`ItemID`),
        CONSTRAINT `FK_Domains_ServiceItems_ZoneItem` FOREIGN KEY (`ZoneItemID`) REFERENCES `ServiceItems` (`ItemID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeOrganizationDomains` (
        `OrganizationDomainID` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `DomainID` int NULL,
        `IsHost` tinyint(1) NULL DEFAULT FALSE,
        `DomainTypeID` int NOT NULL DEFAULT 0,
        CONSTRAINT `PK_ExchangeOrganizationDomains` PRIMARY KEY (`OrganizationDomainID`),
        CONSTRAINT `FK_ExchangeOrganizationDomains_ServiceItems` FOREIGN KEY (`ItemID`) REFERENCES `ServiceItems` (`ItemID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeOrganizations` (
        `ItemID` int NOT NULL,
        `OrganizationID` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
        `ExchangeMailboxPlanID` int NULL,
        `LyncUserPlanID` int NULL,
        `SfBUserPlanID` int NULL,
        CONSTRAINT `PK_ExchangeOrganizations` PRIMARY KEY (`ItemID`),
        CONSTRAINT `FK_ExchangeOrganizations_ServiceItems` FOREIGN KEY (`ItemID`) REFERENCES `ServiceItems` (`ItemID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PackageIPAddresses` (
        `PackageAddressID` int NOT NULL AUTO_INCREMENT,
        `PackageID` int NOT NULL,
        `AddressID` int NOT NULL,
        `ItemID` int NULL,
        `IsPrimary` tinyint(1) NULL,
        `OrgID` int NULL,
        CONSTRAINT `PK_PackageIPAddresses` PRIMARY KEY (`PackageAddressID`),
        CONSTRAINT `FK_PackageIPAddresses_IPAddresses` FOREIGN KEY (`AddressID`) REFERENCES `IPAddresses` (`AddressID`),
        CONSTRAINT `FK_PackageIPAddresses_Packages` FOREIGN KEY (`PackageID`) REFERENCES `Packages` (`PackageID`) ON DELETE CASCADE,
        CONSTRAINT `FK_PackageIPAddresses_ServiceItems` FOREIGN KEY (`ItemID`) REFERENCES `ServiceItems` (`ItemID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `PrivateIPAddresses` (
        `PrivateAddressID` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `IPAddress` varchar(15) CHARACTER SET utf8mb4 NOT NULL,
        `IsPrimary` tinyint(1) NOT NULL,
        CONSTRAINT `PK_PrivateIPAddresses` PRIMARY KEY (`PrivateAddressID`),
        CONSTRAINT `FK_PrivateIPAddresses_ServiceItems` FOREIGN KEY (`ItemID`) REFERENCES `ServiceItems` (`ItemID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ServiceItemProperties` (
        `ItemID` int NOT NULL,
        `PropertyName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `PropertyValue` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_ServiceItemProperties` PRIMARY KEY (`ItemID`, `PropertyName`),
        CONSTRAINT `FK_ServiceItemProperties_ServiceItems` FOREIGN KEY (`ItemID`) REFERENCES `ServiceItems` (`ItemID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `EnterpriseFolders` (
        `EnterpriseFolderID` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `FolderName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `FolderQuota` int NOT NULL DEFAULT 0,
        `LocationDrive` varchar(255) CHARACTER SET utf8mb4 NULL,
        `HomeFolder` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Domain` varchar(255) CHARACTER SET utf8mb4 NULL,
        `StorageSpaceFolderId` int NULL,
        CONSTRAINT `PK_EnterpriseFolders` PRIMARY KEY (`EnterpriseFolderID`),
        CONSTRAINT `FK_EnterpriseFolders_StorageSpaceFolderId` FOREIGN KEY (`StorageSpaceFolderId`) REFERENCES `StorageSpaceFolders` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `DomainDnsRecords` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `DomainId` int NOT NULL,
        `RecordType` int NOT NULL,
        `DnsServer` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Value` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Date` datetime(6) NULL,
        CONSTRAINT `PK_DomainDnsRecord` PRIMARY KEY (`ID`),
        CONSTRAINT `FK_DomainDnsRecords_DomainId` FOREIGN KEY (`DomainId`) REFERENCES `Domains` (`DomainID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeMailboxPlans` (
        `MailboxPlanId` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `MailboxPlan` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        `MailboxPlanType` int NULL,
        `EnableActiveSync` tinyint(1) NOT NULL,
        `EnableIMAP` tinyint(1) NOT NULL,
        `EnableMAPI` tinyint(1) NOT NULL,
        `EnableOWA` tinyint(1) NOT NULL,
        `EnablePOP` tinyint(1) NOT NULL,
        `IsDefault` tinyint(1) NOT NULL,
        `IssueWarningPct` int NOT NULL,
        `KeepDeletedItemsDays` int NOT NULL,
        `MailboxSizeMB` int NOT NULL,
        `MaxReceiveMessageSizeKB` int NOT NULL,
        `MaxRecipients` int NOT NULL,
        `MaxSendMessageSizeKB` int NOT NULL,
        `ProhibitSendPct` int NOT NULL,
        `ProhibitSendReceivePct` int NOT NULL,
        `HideFromAddressBook` tinyint(1) NOT NULL,
        `AllowLitigationHold` tinyint(1) NULL,
        `RecoverableItemsWarningPct` int NULL,
        `RecoverableItemsSpace` int NULL,
        `LitigationHoldUrl` varchar(256) CHARACTER SET utf8mb4 NULL,
        `LitigationHoldMsg` varchar(512) CHARACTER SET utf8mb4 NULL,
        `Archiving` tinyint(1) NULL,
        `EnableArchiving` tinyint(1) NULL,
        `ArchiveSizeMB` int NULL,
        `ArchiveWarningPct` int NULL,
        `EnableAutoReply` tinyint(1) NULL,
        `IsForJournaling` tinyint(1) NULL,
        `EnableForceArchiveDeletion` tinyint(1) NULL,
        CONSTRAINT `PK_ExchangeMailboxPlans` PRIMARY KEY (`MailboxPlanId`),
        CONSTRAINT `FK_ExchangeMailboxPlans_ExchangeOrganizations` FOREIGN KEY (`ItemID`) REFERENCES `ExchangeOrganizations` (`ItemID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeOrganizationSettings` (
        `ItemId` int NOT NULL,
        `SettingsName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Xml` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_ExchangeOrganizationSettings` PRIMARY KEY (`ItemId`, `SettingsName`),
        CONSTRAINT `FK_ExchangeOrganizationSettings_ExchangeOrganizations_ItemId` FOREIGN KEY (`ItemId`) REFERENCES `ExchangeOrganizations` (`ItemID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeOrganizationSsFolders` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ItemId` int NOT NULL,
        `Type` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `StorageSpaceFolderId` int NOT NULL,
        CONSTRAINT `PK_ExchangeOrganizationSsFolder` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ExchangeOrganizationSsFolders_ItemId` FOREIGN KEY (`ItemId`) REFERENCES `ExchangeOrganizations` (`ItemID`) ON DELETE CASCADE,
        CONSTRAINT `FK_ExchangeOrganizationSsFolders_StorageSpaceFolderId` FOREIGN KEY (`StorageSpaceFolderId`) REFERENCES `StorageSpaceFolders` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `LyncUserPlans` (
        `LyncUserPlanId` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `LyncUserPlanName` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        `LyncUserPlanType` int NULL,
        `IM` tinyint(1) NOT NULL,
        `Mobility` tinyint(1) NOT NULL,
        `MobilityEnableOutsideVoice` tinyint(1) NOT NULL,
        `Federation` tinyint(1) NOT NULL,
        `Conferencing` tinyint(1) NOT NULL,
        `EnterpriseVoice` tinyint(1) NOT NULL,
        `VoicePolicy` int NOT NULL,
        `IsDefault` tinyint(1) NOT NULL,
        `RemoteUserAccess` tinyint(1) NOT NULL DEFAULT FALSE,
        `PublicIMConnectivity` tinyint(1) NOT NULL DEFAULT FALSE,
        `AllowOrganizeMeetingsWithExternalAnonymous` tinyint(1) NOT NULL DEFAULT FALSE,
        `Telephony` int NULL,
        `ServerURI` varchar(300) CHARACTER SET utf8mb4 NULL,
        `ArchivePolicy` varchar(300) CHARACTER SET utf8mb4 NULL,
        `TelephonyDialPlanPolicy` varchar(300) CHARACTER SET utf8mb4 NULL,
        `TelephonyVoicePolicy` varchar(300) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_LyncUserPlans` PRIMARY KEY (`LyncUserPlanId`),
        CONSTRAINT `FK_LyncUserPlans_ExchangeOrganizations` FOREIGN KEY (`ItemID`) REFERENCES `ExchangeOrganizations` (`ItemID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeAccounts` (
        `AccountID` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `AccountType` int NOT NULL,
        `AccountName` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        `DisplayName` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        `PrimaryEmailAddress` varchar(300) CHARACTER SET utf8mb4 NULL,
        `MailEnabledPublicFolder` tinyint(1) NULL,
        `MailboxManagerActions` varchar(200) CHARACTER SET utf8mb4 NULL,
        `SamAccountName` varchar(100) CHARACTER SET utf8mb4 NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `MailboxPlanId` int NULL,
        `SubscriberNumber` varchar(32) CHARACTER SET utf8mb4 NULL,
        `UserPrincipalName` varchar(300) CHARACTER SET utf8mb4 NULL,
        `ExchangeDisclaimerId` int NULL,
        `ArchivingMailboxPlanId` int NULL,
        `EnableArchiving` tinyint(1) NULL,
        `LevelID` int NULL,
        `IsVIP` tinyint(1) NOT NULL DEFAULT FALSE,
        CONSTRAINT `PK_ExchangeAccounts` PRIMARY KEY (`AccountID`),
        CONSTRAINT `FK_ExchangeAccounts_ExchangeMailboxPlans` FOREIGN KEY (`MailboxPlanId`) REFERENCES `ExchangeMailboxPlans` (`MailboxPlanId`),
        CONSTRAINT `FK_ExchangeAccounts_ServiceItems` FOREIGN KEY (`ItemID`) REFERENCES `ServiceItems` (`ItemID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `LyncUsers` (
        `LyncUserID` int NOT NULL AUTO_INCREMENT,
        `AccountID` int NOT NULL,
        `LyncUserPlanID` int NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NOT NULL,
        `SipAddress` varchar(300) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_LyncUsers` PRIMARY KEY (`LyncUserID`),
        CONSTRAINT `FK_LyncUsers_LyncUserPlans` FOREIGN KEY (`LyncUserPlanID`) REFERENCES `LyncUserPlans` (`LyncUserPlanId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `AccessTokens` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `AccessTokenGuid` char(36) COLLATE ascii_general_ci NOT NULL,
        `ExpirationDate` datetime(6) NOT NULL,
        `AccountID` int NOT NULL,
        `ItemId` int NOT NULL,
        `TokenType` int NOT NULL,
        `SmsResponse` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AccessToken` PRIMARY KEY (`ID`),
        CONSTRAINT `FK_AccessTokens_UserId` FOREIGN KEY (`AccountID`) REFERENCES `ExchangeAccounts` (`AccountID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `BlackBerryUsers` (
        `BlackBerryUserId` int NOT NULL AUTO_INCREMENT,
        `AccountId` int NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ModifiedDate` datetime(6) NOT NULL,
        CONSTRAINT `PK_BlackBerryUsers` PRIMARY KEY (`BlackBerryUserId`),
        CONSTRAINT `FK_BlackBerryUsers_ExchangeAccounts` FOREIGN KEY (`AccountId`) REFERENCES `ExchangeAccounts` (`AccountID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `CRMUsers` (
        `CRMUserID` int NOT NULL AUTO_INCREMENT,
        `AccountID` int NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ChangedDate` datetime(6) NOT NULL,
        `CRMUserGuid` char(36) COLLATE ascii_general_ci NULL,
        `BusinessUnitID` char(36) COLLATE ascii_general_ci NULL,
        `CALType` int NULL,
        CONSTRAINT `PK_CRMUsers` PRIMARY KEY (`CRMUserID`),
        CONSTRAINT `FK_CRMUsers_ExchangeAccounts` FOREIGN KEY (`AccountID`) REFERENCES `ExchangeAccounts` (`AccountID`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `EnterpriseFoldersOwaPermissions` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `ItemID` int NOT NULL,
        `FolderID` int NOT NULL,
        `AccountID` int NOT NULL,
        CONSTRAINT `PK_EnterpriseFoldersOwaPermission` PRIMARY KEY (`ID`),
        CONSTRAINT `FK_EnterpriseFoldersOwaPermissions_AccountId` FOREIGN KEY (`AccountID`) REFERENCES `ExchangeAccounts` (`AccountID`) ON DELETE CASCADE,
        CONSTRAINT `FK_EnterpriseFoldersOwaPermissions_FolderId` FOREIGN KEY (`FolderID`) REFERENCES `EnterpriseFolders` (`EnterpriseFolderID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `ExchangeAccountEmailAddresses` (
        `AddressID` int NOT NULL AUTO_INCREMENT,
        `AccountID` int NOT NULL,
        `EmailAddress` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_ExchangeAccountEmailAddresses` PRIMARY KEY (`AddressID`),
        CONSTRAINT `FK_ExchangeAccountEmailAddresses_ExchangeAccounts` FOREIGN KEY (`AccountID`) REFERENCES `ExchangeAccounts` (`AccountID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `RDSCollectionUsers` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `RDSCollectionId` int NOT NULL,
        `AccountID` int NOT NULL,
        CONSTRAINT `PK_RdsCollectionUser` PRIMARY KEY (`ID`),
        CONSTRAINT `FK_RDSCollectionUsers_RDSCollectionId` FOREIGN KEY (`RDSCollectionId`) REFERENCES `RDSCollections` (`ID`) ON DELETE CASCADE,
        CONSTRAINT `FK_RDSCollectionUsers_UserId` FOREIGN KEY (`AccountID`) REFERENCES `ExchangeAccounts` (`AccountID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `WebDavAccessTokens` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `FilePath` longtext CHARACTER SET utf8mb4 NOT NULL,
        `AuthData` longtext CHARACTER SET utf8mb4 NOT NULL,
        `AccessToken` char(36) COLLATE ascii_general_ci NOT NULL,
        `ExpirationDate` datetime(6) NOT NULL,
        `AccountID` int NOT NULL,
        `ItemId` int NOT NULL,
        CONSTRAINT `PK_WebDavAccessToken` PRIMARY KEY (`ID`),
        CONSTRAINT `FK_WebDavAccessTokens_UserId` FOREIGN KEY (`AccountID`) REFERENCES `ExchangeAccounts` (`AccountID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE TABLE `WebDavPortalUsersSettings` (
        `ID` int NOT NULL AUTO_INCREMENT,
        `AccountId` int NOT NULL,
        `Settings` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_WebDavPortalUsersSetting` PRIMARY KEY (`ID`),
        CONSTRAINT `FK_WebDavPortalUsersSettings_UserId` FOREIGN KEY (`AccountId`) REFERENCES `ExchangeAccounts` (`AccountID`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `AuditLogSources` (`SourceName`)
    VALUES ('APP_INSTALLER'),
    ('AUTO_DISCOVERY'),
    ('BACKUP'),
    ('DNS_ZONE'),
    ('DOMAIN'),
    ('ENTERPRISE_STORAGE'),
    ('EXCHANGE'),
    ('FILES'),
    ('FTP_ACCOUNT'),
    ('GLOBAL_DNS'),
    ('HOSTING_SPACE'),
    ('HOSTING_SPACE_WR'),
    ('IMPORT'),
    ('IP_ADDRESS'),
    ('MAIL_ACCOUNT'),
    ('MAIL_DOMAIN'),
    ('MAIL_FORWARDING'),
    ('MAIL_GROUP'),
    ('MAIL_LIST'),
    ('OCS'),
    ('ODBC_DSN'),
    ('ORGANIZATION'),
    ('REMOTE_DESKTOP_SERVICES'),
    ('SCHEDULER'),
    ('SERVER'),
    ('SHAREPOINT'),
    ('SPACE'),
    ('SQL_DATABASE'),
    ('SQL_USER'),
    ('STATS_SITE'),
    ('STORAGE_SPACES'),
    ('USER'),
    ('VIRTUAL_SERVER'),
    ('VLAN'),
    ('VPS'),
    ('VPS2012'),
    ('WAG_INSTALLER'),
    ('WEB_SITE');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `AuditLogTasks` (`SourceName`, `TaskName`, `TaskDescription`)
    VALUES ('APP_INSTALLER', 'INSTALL_APPLICATION', 'Install application'),
    ('AUTO_DISCOVERY', 'IS_INSTALLED', 'Is installed'),
    ('BACKUP', 'BACKUP', 'Backup'),
    ('BACKUP', 'RESTORE', 'Restore'),
    ('DNS_ZONE', 'ADD_RECORD', 'Add record'),
    ('DNS_ZONE', 'DELETE_RECORD', 'Delete record'),
    ('DNS_ZONE', 'UPDATE_RECORD', 'Update record'),
    ('DOMAIN', 'ADD', 'Add'),
    ('DOMAIN', 'DELETE', 'Delete'),
    ('DOMAIN', 'ENABLE_DNS', 'Enable DNS'),
    ('DOMAIN', 'UPDATE', 'Update'),
    ('ENTERPRISE_STORAGE', 'CREATE_FOLDER', 'Create folder'),
    ('ENTERPRISE_STORAGE', 'CREATE_MAPPED_DRIVE', 'Create mapped drive'),
    ('ENTERPRISE_STORAGE', 'DELETE_FOLDER', 'Delete folder'),
    ('ENTERPRISE_STORAGE', 'DELETE_MAPPED_DRIVE', 'Delete mapped drive'),
    ('ENTERPRISE_STORAGE', 'GET_ORG_STATS', 'Get organization statistics'),
    ('ENTERPRISE_STORAGE', 'SET_ENTERPRISE_FOLDER_GENERAL_SETTINGS', 'Set enterprise folder general settings'),
    ('EXCHANGE', 'ADD_DISTR_LIST_ADDRESS', 'Add distribution list e-mail address'),
    ('EXCHANGE', 'ADD_DOMAIN', 'Add organization domain'),
    ('EXCHANGE', 'ADD_EXCHANGE_EXCHANGEDISCLAIMER', 'Add Exchange disclaimer'),
    ('EXCHANGE', 'ADD_EXCHANGE_MAILBOXPLAN_RETENTIONPOLICY_ARCHIVING', 'Add Exchange archiving retention policy'),
    ('EXCHANGE', 'ADD_EXCHANGE_RETENTIONPOLICYTAG', 'Add Exchange retention policy tag'),
    ('EXCHANGE', 'ADD_MAILBOX_ADDRESS', 'Add mailbox e-mail address'),
    ('EXCHANGE', 'ADD_PUBLIC_FOLDER_ADDRESS', 'Add public folder e-mail address'),
    ('EXCHANGE', 'CALCULATE_DISKSPACE', 'Calculate organization disk space'),
    ('EXCHANGE', 'CREATE_CONTACT', 'Create contact'),
    ('EXCHANGE', 'CREATE_DISTR_LIST', 'Create distribution list'),
    ('EXCHANGE', 'CREATE_MAILBOX', 'Create mailbox'),
    ('EXCHANGE', 'CREATE_ORG', 'Create organization'),
    ('EXCHANGE', 'CREATE_PUBLIC_FOLDER', 'Create public folder'),
    ('EXCHANGE', 'DELETE_CONTACT', 'Delete contact'),
    ('EXCHANGE', 'DELETE_DISTR_LIST', 'Delete distribution list'),
    ('EXCHANGE', 'DELETE_DISTR_LIST_ADDRESSES', 'Delete distribution list e-mail addresses'),
    ('EXCHANGE', 'DELETE_DOMAIN', 'Delete organization domain'),
    ('EXCHANGE', 'DELETE_EXCHANGE_MAILBOXPLAN_RETENTIONPOLICY_ARCHIV', 'Delete Exchange archiving retention policy'),
    ('EXCHANGE', 'DELETE_EXCHANGE_RETENTIONPOLICYTAG', 'Delete Exchange retention policy tag'),
    ('EXCHANGE', 'DELETE_MAILBOX', 'Delete mailbox'),
    ('EXCHANGE', 'DELETE_MAILBOX_ADDRESSES', 'Delete mailbox e-mail addresses'),
    ('EXCHANGE', 'DELETE_ORG', 'Delete organization'),
    ('EXCHANGE', 'DELETE_PUBLIC_FOLDER', 'Delete public folder'),
    ('EXCHANGE', 'DELETE_PUBLIC_FOLDER_ADDRESSES', 'Delete public folder e-mail addresses'),
    ('EXCHANGE', 'DISABLE_MAIL_PUBLIC_FOLDER', 'Disable mail public folder');
    INSERT INTO `AuditLogTasks` (`SourceName`, `TaskName`, `TaskDescription`)
    VALUES ('EXCHANGE', 'DISABLE_MAILBOX', 'Disable Mailbox'),
    ('EXCHANGE', 'ENABLE_MAIL_PUBLIC_FOLDER', 'Enable mail public folder'),
    ('EXCHANGE', 'GET_ACTIVESYNC_POLICY', 'Get Activesync policy'),
    ('EXCHANGE', 'GET_CONTACT_GENERAL', 'Get contact general settings'),
    ('EXCHANGE', 'GET_CONTACT_MAILFLOW', 'Get contact mail flow settings'),
    ('EXCHANGE', 'GET_DISTR_LIST_ADDRESSES', 'Get distribution list e-mail addresses'),
    ('EXCHANGE', 'GET_DISTR_LIST_BYMEMBER', 'Get distributions list by member'),
    ('EXCHANGE', 'GET_DISTR_LIST_GENERAL', 'Get distribution list general settings'),
    ('EXCHANGE', 'GET_DISTR_LIST_MAILFLOW', 'Get distribution list mail flow settings'),
    ('EXCHANGE', 'GET_DISTRIBUTION_LIST_RESULT', 'Get distributions list result'),
    ('EXCHANGE', 'GET_EXCHANGE_ACCOUNTDISCLAIMERID', 'Get Exchange account disclaimer id'),
    ('EXCHANGE', 'GET_EXCHANGE_EXCHANGEDISCLAIMER', 'Get Exchange disclaimer'),
    ('EXCHANGE', 'GET_EXCHANGE_MAILBOXPLAN', 'Get Exchange Mailbox plan'),
    ('EXCHANGE', 'GET_EXCHANGE_MAILBOXPLANS', 'Get Exchange Mailbox plans'),
    ('EXCHANGE', 'GET_EXCHANGE_RETENTIONPOLICYTAG', 'Get Exchange retention policy tag'),
    ('EXCHANGE', 'GET_EXCHANGE_RETENTIONPOLICYTAGS', 'Get Exchange retention policy tags'),
    ('EXCHANGE', 'GET_FOLDERS_STATS', 'Get organization public folder statistics'),
    ('EXCHANGE', 'GET_MAILBOX_ADDRESSES', 'Get mailbox e-mail addresses'),
    ('EXCHANGE', 'GET_MAILBOX_ADVANCED', 'Get mailbox advanced settings'),
    ('EXCHANGE', 'GET_MAILBOX_AUTOREPLY', 'Get Mailbox autoreply'),
    ('EXCHANGE', 'GET_MAILBOX_GENERAL', 'Get mailbox general settings'),
    ('EXCHANGE', 'GET_MAILBOX_MAILFLOW', 'Get mailbox mail flow settings'),
    ('EXCHANGE', 'GET_MAILBOX_PERMISSIONS', 'Get Mailbox permissions'),
    ('EXCHANGE', 'GET_MAILBOX_STATS', 'Get Mailbox statistics'),
    ('EXCHANGE', 'GET_MAILBOXES_STATS', 'Get organization mailboxes statistics'),
    ('EXCHANGE', 'GET_MOBILE_DEVICES', 'Get mobile devices'),
    ('EXCHANGE', 'GET_ORG_LIMITS', 'Get organization storage limits'),
    ('EXCHANGE', 'GET_ORG_STATS', 'Get organization statistics'),
    ('EXCHANGE', 'GET_PICTURE', 'Get picture'),
    ('EXCHANGE', 'GET_PUBLIC_FOLDER_ADDRESSES', 'Get public folder e-mail addresses'),
    ('EXCHANGE', 'GET_PUBLIC_FOLDER_GENERAL', 'Get public folder general settings'),
    ('EXCHANGE', 'GET_PUBLIC_FOLDER_MAILFLOW', 'Get public folder mail flow settings'),
    ('EXCHANGE', 'GET_RESOURCE_MAILBOX', 'Get resource Mailbox settings'),
    ('EXCHANGE', 'SET_EXCHANGE_ACCOUNTDISCLAIMERID', 'Set exchange account disclaimer id'),
    ('EXCHANGE', 'SET_EXCHANGE_MAILBOXPLAN', 'Set exchange Mailbox plan'),
    ('EXCHANGE', 'SET_MAILBOXPLAN_RETENTIONPOLICY_ARCHIVING', 'Set Mailbox plan retention policy archiving'),
    ('EXCHANGE', 'SET_ORG_LIMITS', 'Update organization storage limits'),
    ('EXCHANGE', 'SET_PRIMARY_DISTR_LIST_ADDRESS', 'Set distribution list primary e-mail address'),
    ('EXCHANGE', 'SET_PRIMARY_MAILBOX_ADDRESS', 'Set mailbox primary e-mail address'),
    ('EXCHANGE', 'SET_PRIMARY_PUBLIC_FOLDER_ADDRESS', 'Set public folder primary e-mail address'),
    ('EXCHANGE', 'UPDATE_CONTACT_GENERAL', 'Update contact general settings'),
    ('EXCHANGE', 'UPDATE_CONTACT_MAILFLOW', 'Update contact mail flow settings');
    INSERT INTO `AuditLogTasks` (`SourceName`, `TaskName`, `TaskDescription`)
    VALUES ('EXCHANGE', 'UPDATE_DISTR_LIST_GENERAL', 'Update distribution list general settings'),
    ('EXCHANGE', 'UPDATE_DISTR_LIST_MAILFLOW', 'Update distribution list mail flow settings'),
    ('EXCHANGE', 'UPDATE_EXCHANGE_RETENTIONPOLICYTAG', 'Update Exchange retention policy tag'),
    ('EXCHANGE', 'UPDATE_MAILBOX_ADVANCED', 'Update mailbox advanced settings'),
    ('EXCHANGE', 'UPDATE_MAILBOX_AUTOREPLY', 'Update Mailbox autoreply'),
    ('EXCHANGE', 'UPDATE_MAILBOX_GENERAL', 'Update mailbox general settings'),
    ('EXCHANGE', 'UPDATE_MAILBOX_MAILFLOW', 'Update mailbox mail flow settings'),
    ('EXCHANGE', 'UPDATE_PUBLIC_FOLDER_GENERAL', 'Update public folder general settings'),
    ('EXCHANGE', 'UPDATE_PUBLIC_FOLDER_MAILFLOW', 'Update public folder mail flow settings'),
    ('EXCHANGE', 'UPDATE_RESOURCE_MAILBOX', 'Update resource Mailbox settings'),
    ('FILES', 'COPY_FILES', 'Copy files'),
    ('FILES', 'CREATE_ACCESS_DATABASE', 'Create MS Access database'),
    ('FILES', 'CREATE_FILE', 'Create file'),
    ('FILES', 'CREATE_FOLDER', 'Create folder'),
    ('FILES', 'DELETE_FILES', 'Delete files'),
    ('FILES', 'MOVE_FILES', 'Move files'),
    ('FILES', 'RENAME_FILE', 'Rename file'),
    ('FILES', 'SET_PERMISSIONS', NULL),
    ('FILES', 'UNZIP_FILES', 'Unzip files'),
    ('FILES', 'UPDATE_BINARY_CONTENT', 'Update file binary content'),
    ('FILES', 'ZIP_FILES', 'Zip files'),
    ('FTP_ACCOUNT', 'ADD', 'Add'),
    ('FTP_ACCOUNT', 'DELETE', 'Delete'),
    ('FTP_ACCOUNT', 'UPDATE', 'Update'),
    ('GLOBAL_DNS', 'ADD', 'Add'),
    ('GLOBAL_DNS', 'DELETE', 'Delete'),
    ('GLOBAL_DNS', 'UPDATE', 'Update'),
    ('HOSTING_SPACE', 'ADD', 'Add'),
    ('HOSTING_SPACE_WR', 'ADD', 'Add'),
    ('IMPORT', 'IMPORT', 'Import'),
    ('IP_ADDRESS', 'ADD', 'Add'),
    ('IP_ADDRESS', 'ADD_RANGE', 'Add range'),
    ('IP_ADDRESS', 'ALLOCATE_PACKAGE_IP', 'Allocate package IP addresses'),
    ('IP_ADDRESS', 'DEALLOCATE_PACKAGE_IP', 'Deallocate package IP addresses'),
    ('IP_ADDRESS', 'DELETE', 'Delete'),
    ('IP_ADDRESS', 'DELETE_RANGE', 'Delete IP Addresses'),
    ('IP_ADDRESS', 'UPDATE', 'Update'),
    ('IP_ADDRESS', 'UPDATE_RANGE', 'Update IP Addresses'),
    ('MAIL_ACCOUNT', 'ADD', 'Add'),
    ('MAIL_ACCOUNT', 'DELETE', 'Delete'),
    ('MAIL_ACCOUNT', 'UPDATE', 'Update'),
    ('MAIL_DOMAIN', 'ADD', 'Add');
    INSERT INTO `AuditLogTasks` (`SourceName`, `TaskName`, `TaskDescription`)
    VALUES ('MAIL_DOMAIN', 'ADD_POINTER', 'Add pointer'),
    ('MAIL_DOMAIN', 'DELETE', 'Delete'),
    ('MAIL_DOMAIN', 'DELETE_POINTER', 'Update pointer'),
    ('MAIL_DOMAIN', 'UPDATE', 'Update'),
    ('MAIL_FORWARDING', 'ADD', 'Add'),
    ('MAIL_FORWARDING', 'DELETE', 'Delete'),
    ('MAIL_FORWARDING', 'UPDATE', 'Update'),
    ('MAIL_GROUP', 'ADD', 'Add'),
    ('MAIL_GROUP', 'DELETE', 'Delete'),
    ('MAIL_GROUP', 'UPDATE', 'Update'),
    ('MAIL_LIST', 'ADD', 'Add'),
    ('MAIL_LIST', 'DELETE', 'Delete'),
    ('MAIL_LIST', 'UPDATE', 'Update'),
    ('OCS', 'CREATE_OCS_USER', 'Create OCS user'),
    ('OCS', 'GET_OCS_USERS', 'Get OCS users'),
    ('OCS', 'GET_OCS_USERS_COUNT', 'Get OCS users count'),
    ('ODBC_DSN', 'ADD', 'Add'),
    ('ODBC_DSN', 'DELETE', 'Delete'),
    ('ODBC_DSN', 'UPDATE', 'Update'),
    ('ORGANIZATION', 'CREATE_ORG', 'Create organization'),
    ('ORGANIZATION', 'CREATE_ORGANIZATION_ENTERPRISE_STORAGE', 'Create organization enterprise storage'),
    ('ORGANIZATION', 'CREATE_SECURITY_GROUP', 'Create security group'),
    ('ORGANIZATION', 'CREATE_USER', 'Create user'),
    ('ORGANIZATION', 'DELETE_ORG', 'Delete organization'),
    ('ORGANIZATION', 'DELETE_SECURITY_GROUP', 'Delete security group'),
    ('ORGANIZATION', 'GET_ORG_STATS', 'Get organization statistics'),
    ('ORGANIZATION', 'GET_SECURITY_GROUP_GENERAL', 'Get security group general settings'),
    ('ORGANIZATION', 'GET_SECURITY_GROUPS_BYMEMBER', 'Get security groups by member'),
    ('ORGANIZATION', 'GET_SUPPORT_SERVICE_LEVELS', 'Get support service levels'),
    ('ORGANIZATION', 'REMOVE_USER', 'Remove user'),
    ('ORGANIZATION', 'SEND_USER_PASSWORD_RESET_EMAIL_PINCODE', 'Send user password reset email pincode'),
    ('ORGANIZATION', 'SET_USER_PASSWORD', 'Set user password'),
    ('ORGANIZATION', 'SET_USER_USERPRINCIPALNAME', 'Set user principal name'),
    ('ORGANIZATION', 'UPDATE_PASSWORD_SETTINGS', 'Update password settings'),
    ('ORGANIZATION', 'UPDATE_SECURITY_GROUP_GENERAL', 'Update security group general settings'),
    ('ORGANIZATION', 'UPDATE_USER_GENERAL', 'Update user general settings'),
    ('REMOTE_DESKTOP_SERVICES', 'ADD_RDS_SERVER', 'Add RDS server'),
    ('REMOTE_DESKTOP_SERVICES', 'RESTART_RDS_SERVER', 'Restart RDS server'),
    ('REMOTE_DESKTOP_SERVICES', 'SET_RDS_SERVER_NEW_CONNECTIONS_ALLOWED', 'Set RDS new connection allowed'),
    ('SCHEDULER', 'RUN_SCHEDULE', NULL),
    ('SERVER', 'ADD', 'Add'),
    ('SERVER', 'ADD_SERVICE', 'Add service');
    INSERT INTO `AuditLogTasks` (`SourceName`, `TaskName`, `TaskDescription`)
    VALUES ('SERVER', 'CHANGE_WINDOWS_SERVICE_STATUS', 'Change Windows service status'),
    ('SERVER', 'CHECK_AVAILABILITY', 'Check availability'),
    ('SERVER', 'CLEAR_EVENT_LOG', 'Clear Windows event log'),
    ('SERVER', 'DELETE', 'Delete'),
    ('SERVER', 'DELETE_SERVICE', 'Delete service'),
    ('SERVER', 'REBOOT', 'Reboot'),
    ('SERVER', 'RESET_TERMINAL_SESSION', 'Reset terminal session'),
    ('SERVER', 'TERMINATE_SYSTEM_PROCESS', 'Terminate system process'),
    ('SERVER', 'UPDATE', 'Update'),
    ('SERVER', 'UPDATE_AD_PASSWORD', 'Update active directory password'),
    ('SERVER', 'UPDATE_PASSWORD', 'Update access password'),
    ('SERVER', 'UPDATE_SERVICE', 'Update service'),
    ('SHAREPOINT', 'ADD_GROUP', 'Add group'),
    ('SHAREPOINT', 'ADD_SITE', 'Add site'),
    ('SHAREPOINT', 'ADD_USER', 'Add user'),
    ('SHAREPOINT', 'BACKUP_SITE', 'Backup site'),
    ('SHAREPOINT', 'DELETE_GROUP', 'Delete group'),
    ('SHAREPOINT', 'DELETE_SITE', 'Delete site'),
    ('SHAREPOINT', 'DELETE_USER', 'Delete user'),
    ('SHAREPOINT', 'INSTALL_WEBPARTS', 'Install Web Parts package'),
    ('SHAREPOINT', 'RESTORE_SITE', 'Restore site'),
    ('SHAREPOINT', 'UNINSTALL_WEBPARTS', 'Uninstall Web Parts package'),
    ('SHAREPOINT', 'UPDATE_GROUP', 'Update group'),
    ('SHAREPOINT', 'UPDATE_USER', 'Update user'),
    ('SPACE', 'CALCULATE_DISKSPACE', 'Calculate disk space'),
    ('SPACE', 'CHANGE_ITEMS_STATUS', 'Change hosting items status'),
    ('SPACE', 'CHANGE_STATUS', 'Change hostng space status'),
    ('SPACE', 'DELETE', 'Delete hosting space'),
    ('SPACE', 'DELETE_ITEMS', 'Delete hosting items'),
    ('SQL_DATABASE', 'ADD', 'Add'),
    ('SQL_DATABASE', 'BACKUP', 'Backup'),
    ('SQL_DATABASE', 'DELETE', 'Delete'),
    ('SQL_DATABASE', 'RESTORE', 'Restore'),
    ('SQL_DATABASE', 'TRUNCATE', 'Truncate'),
    ('SQL_DATABASE', 'UPDATE', 'Update'),
    ('SQL_USER', 'ADD', 'Add'),
    ('SQL_USER', 'DELETE', 'Delete'),
    ('SQL_USER', 'UPDATE', 'Update'),
    ('STATS_SITE', 'ADD', 'Add statistics site'),
    ('STATS_SITE', 'DELETE', 'Delete statistics site'),
    ('STATS_SITE', 'UPDATE', 'Update statistics site'),
    ('STORAGE_SPACES', 'REMOVE_STORAGE_SPACE', 'Remove storage space');
    INSERT INTO `AuditLogTasks` (`SourceName`, `TaskName`, `TaskDescription`)
    VALUES ('STORAGE_SPACES', 'SAVE_STORAGE_SPACE', 'Save storage space'),
    ('STORAGE_SPACES', 'SAVE_STORAGE_SPACE_LEVEL', 'Save storage space level'),
    ('USER', 'ADD', 'Add'),
    ('USER', 'AUTHENTICATE', 'Authenticate'),
    ('USER', 'CHANGE_PASSWORD', 'Change password'),
    ('USER', 'CHANGE_PASSWORD_BY_USERNAME_PASSWORD', 'Change password by username/password'),
    ('USER', 'CHANGE_STATUS', 'Change status'),
    ('USER', 'DELETE', 'Delete'),
    ('USER', 'GET_BY_USERNAME_PASSWORD', 'Get by username/password'),
    ('USER', 'SEND_REMINDER', 'Send password reminder'),
    ('USER', 'UPDATE', 'Update'),
    ('USER', 'UPDATE_SETTINGS', 'Update settings'),
    ('VIRTUAL_SERVER', 'ADD_SERVICES', 'Add services'),
    ('VIRTUAL_SERVER', 'DELETE_SERVICES', 'Delete services'),
    ('VLAN', 'ADD', 'Add'),
    ('VLAN', 'ADD_RANGE', 'Add range'),
    ('VLAN', 'ALLOCATE_PACKAGE_VLAN', 'Allocate package VLAN'),
    ('VLAN', 'DEALLOCATE_PACKAGE_VLAN', 'Deallocate package VLAN'),
    ('VLAN', 'DELETE_RANGE', 'Delete range'),
    ('VLAN', 'UPDATE', 'Update'),
    ('VPS', 'ADD_EXTERNAL_IP', 'Add external IP'),
    ('VPS', 'ADD_PRIVATE_IP', 'Add private IP'),
    ('VPS', 'APPLY_SNAPSHOT', 'Apply VPS snapshot'),
    ('VPS', 'CANCEL_JOB', 'Cancel Job'),
    ('VPS', 'CHANGE_ADMIN_PASSWORD', 'Change administrator password'),
    ('VPS', 'CHANGE_STATE', 'Change VPS state'),
    ('VPS', 'CREATE', 'Create VPS'),
    ('VPS', 'DELETE', 'Delete VPS'),
    ('VPS', 'DELETE_EXTERNAL_IP', 'Delete external IP'),
    ('VPS', 'DELETE_PRIVATE_IP', 'Delete private IP'),
    ('VPS', 'DELETE_SNAPSHOT', 'Delete VPS snapshot'),
    ('VPS', 'DELETE_SNAPSHOT_SUBTREE', 'Delete VPS snapshot subtree'),
    ('VPS', 'EJECT_DVD_DISK', 'Eject DVD disk'),
    ('VPS', 'INSERT_DVD_DISK', 'Insert DVD disk'),
    ('VPS', 'REINSTALL', 'Re-install VPS'),
    ('VPS', 'RENAME_SNAPSHOT', 'Rename VPS snapshot'),
    ('VPS', 'SEND_SUMMARY_LETTER', 'Send VPS summary letter'),
    ('VPS', 'SET_PRIMARY_EXTERNAL_IP', 'Set primary external IP'),
    ('VPS', 'SET_PRIMARY_PRIVATE_IP', 'Set primary private IP'),
    ('VPS', 'TAKE_SNAPSHOT', 'Take VPS snapshot'),
    ('VPS', 'UPDATE_CONFIGURATION', 'Update VPS configuration'),
    ('VPS', 'UPDATE_HOSTNAME', 'Update host name');
    INSERT INTO `AuditLogTasks` (`SourceName`, `TaskName`, `TaskDescription`)
    VALUES ('VPS', 'UPDATE_IP', 'Update IP Address'),
    ('VPS', 'UPDATE_PERMISSIONS', 'Update VPS permissions'),
    ('VPS', 'UPDATE_VDC_PERMISSIONS', 'Update space permissions'),
    ('VPS2012', 'ADD_EXTERNAL_IP', 'Add external IP'),
    ('VPS2012', 'ADD_PRIVATE_IP', 'Add private IP'),
    ('VPS2012', 'APPLY_SNAPSHOT', 'Apply VM snapshot'),
    ('VPS2012', 'CHANGE_ADMIN_PASSWORD', 'Change administrator password'),
    ('VPS2012', 'CHANGE_STATE', 'Change VM state'),
    ('VPS2012', 'CREATE', 'Create VM'),
    ('VPS2012', 'DELETE', 'Delete VM'),
    ('VPS2012', 'DELETE_EXTERNAL_IP', 'Delete external IP'),
    ('VPS2012', 'DELETE_PRIVATE_IP', 'Delete private IP'),
    ('VPS2012', 'DELETE_SNAPSHOT', 'Delete VM snapshot'),
    ('VPS2012', 'DELETE_SNAPSHOT_SUBTREE', 'Delete VM snapshot subtree'),
    ('VPS2012', 'EJECT_DVD_DISK', 'Eject DVD disk'),
    ('VPS2012', 'INSERT_DVD_DISK', 'Insert DVD disk'),
    ('VPS2012', 'REINSTALL', 'Reinstall VM'),
    ('VPS2012', 'RENAME_SNAPSHOT', 'Rename VM snapshot'),
    ('VPS2012', 'SET_PRIMARY_EXTERNAL_IP', 'Set primary external IP'),
    ('VPS2012', 'SET_PRIMARY_PRIVATE_IP', 'Set primary private IP'),
    ('VPS2012', 'TAKE_SNAPSHOT', 'Take VM snapshot'),
    ('VPS2012', 'UPDATE_CONFIGURATION', 'Update VM configuration'),
    ('VPS2012', 'UPDATE_HOSTNAME', 'Update host name'),
    ('WAG_INSTALLER', 'GET_APP_PARAMS_TASK', 'Get application parameters'),
    ('WAG_INSTALLER', 'GET_GALLERY_APP_DETAILS_TASK', 'Get gallery application details'),
    ('WAG_INSTALLER', 'GET_GALLERY_APPS_TASK', 'Get gallery applications'),
    ('WAG_INSTALLER', 'GET_GALLERY_CATEGORIES_TASK', 'Get gallery categories'),
    ('WAG_INSTALLER', 'GET_SRV_GALLERY_APPS_TASK', 'Get server gallery applications'),
    ('WAG_INSTALLER', 'INSTALL_WEB_APP', 'Install Web application'),
    ('WEB_SITE', 'ADD', 'Add'),
    ('WEB_SITE', 'ADD_POINTER', 'Add domain pointer'),
    ('WEB_SITE', 'ADD_SSL_FOLDER', 'Add shared SSL folder'),
    ('WEB_SITE', 'ADD_VDIR', 'Add virtual directory'),
    ('WEB_SITE', 'CHANGE_FP_PASSWORD', 'Change FrontPage account password'),
    ('WEB_SITE', 'CHANGE_STATE', 'Change state'),
    ('WEB_SITE', 'DELETE', 'Delete'),
    ('WEB_SITE', 'DELETE_POINTER', 'Delete domain pointer'),
    ('WEB_SITE', 'DELETE_SECURED_FOLDER', 'Delete secured folder'),
    ('WEB_SITE', 'DELETE_SECURED_GROUP', 'Delete secured group'),
    ('WEB_SITE', 'DELETE_SECURED_USER', 'Delete secured user'),
    ('WEB_SITE', 'DELETE_SSL_FOLDER', 'Delete shared SSL folder'),
    ('WEB_SITE', 'DELETE_VDIR', 'Delete virtual directory');
    INSERT INTO `AuditLogTasks` (`SourceName`, `TaskName`, `TaskDescription`)
    VALUES ('WEB_SITE', 'GET_STATE', 'Get state'),
    ('WEB_SITE', 'INSTALL_FP', 'Install FrontPage Extensions'),
    ('WEB_SITE', 'INSTALL_SECURED_FOLDERS', 'Install secured folders'),
    ('WEB_SITE', 'UNINSTALL_FP', 'Uninstall FrontPage Extensions'),
    ('WEB_SITE', 'UNINSTALL_SECURED_FOLDERS', 'Uninstall secured folders'),
    ('WEB_SITE', 'UPDATE', 'Update'),
    ('WEB_SITE', 'UPDATE_SECURED_FOLDER', 'Add/update secured folder'),
    ('WEB_SITE', 'UPDATE_SECURED_GROUP', 'Add/update secured group'),
    ('WEB_SITE', 'UPDATE_SECURED_USER', 'Add/update secured user'),
    ('WEB_SITE', 'UPDATE_SSL_FOLDER', 'Update shared SSL folder'),
    ('WEB_SITE', 'UPDATE_VDIR', 'Update virtual directory');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ResourceGroups` (`GroupID`, `GroupController`, `GroupName`, `GroupOrder`, `ShowGroup`)
    VALUES (1, 'FuseCP.EnterpriseServer.OperatingSystemController', 'OS', 1, TRUE),
    (2, 'FuseCP.EnterpriseServer.WebServerController', 'Web', 2, TRUE),
    (3, 'FuseCP.EnterpriseServer.FtpServerController', 'FTP', 3, TRUE),
    (4, 'FuseCP.EnterpriseServer.MailServerController', 'Mail', 4, TRUE),
    (5, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2000', 7, TRUE),
    (6, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MySQL4', 11, TRUE),
    (7, 'FuseCP.EnterpriseServer.DnsServerController', 'DNS', 17, TRUE),
    (8, 'FuseCP.EnterpriseServer.StatisticsServerController', 'Statistics', 18, TRUE),
    (10, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2005', 8, TRUE),
    (11, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MySQL5', 12, TRUE),
    (12, NULL, 'Exchange', 5, TRUE),
    (13, NULL, 'Hosted Organizations', 6, TRUE),
    (20, 'FuseCP.EnterpriseServer.HostedSharePointServerController', 'Sharepoint Foundation Server', 14, TRUE),
    (22, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2008', 9, TRUE),
    (23, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2012', 10, TRUE),
    (30, NULL, 'VPS', 19, TRUE),
    (31, NULL, 'BlackBerry', 21, TRUE),
    (32, NULL, 'OCS', 22, TRUE),
    (33, NULL, 'VPS2012', 20, TRUE),
    (40, NULL, 'VPSForPC', 20, TRUE),
    (41, NULL, 'Lync', 24, TRUE),
    (44, 'FuseCP.EnterpriseServer.EnterpriseStorageController', 'EnterpriseStorage', 26, TRUE),
    (45, NULL, 'RDS', 27, TRUE),
    (46, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2014', 10, TRUE),
    (47, NULL, 'Service Levels', 2, TRUE),
    (49, 'FuseCP.EnterpriseServer.StorageSpacesController', 'StorageSpaceServices', 26, TRUE),
    (50, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MariaDB', 11, TRUE),
    (52, NULL, 'SfB', 26, TRUE),
    (61, NULL, 'MailFilters', 5, TRUE),
    (71, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2016', 10, TRUE),
    (72, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2017', 10, TRUE),
    (73, 'FuseCP.EnterpriseServer.HostedSharePointServerEntController', 'Sharepoint Enterprise Server', 15, TRUE),
    (74, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2019', 10, TRUE),
    (75, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2022', 10, TRUE),
    (76, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MsSQL2025', 10, TRUE),
    (90, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MySQL8', 12, TRUE),
    (91, 'FuseCP.EnterpriseServer.DatabaseServerController', 'MySQL9', 12, TRUE),
    (167, NULL, 'Proxmox', 20, TRUE);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ScheduleTasks` (`TaskID`, `RoleID`, `TaskType`)
    VALUES ('SCHEDULE_TASK_ACTIVATE_PAID_INVOICES', 0, 'FuseCP.Ecommerce.EnterpriseServer.ActivatePaidInvoicesTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_AUDIT_LOG_REPORT', 3, 'FuseCP.EnterpriseServer.AuditLogReportTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_BACKUP', 1, 'FuseCP.EnterpriseServer.BackupTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_BACKUP_DATABASE', 3, 'FuseCP.EnterpriseServer.BackupDatabaseTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_CALCULATE_EXCHANGE_DISKSPACE', 2, 'FuseCP.EnterpriseServer.CalculateExchangeDiskspaceTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH', 1, 'FuseCP.EnterpriseServer.CalculatePackagesBandwidthTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE', 1, 'FuseCP.EnterpriseServer.CalculatePackagesDiskspaceTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_CANCEL_OVERDUE_INVOICES', 0, 'FuseCP.Ecommerce.EnterpriseServer.CancelOverdueInvoicesTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_CHECK_WEBSITE', 3, 'FuseCP.EnterpriseServer.CheckWebSiteTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_CHECK_WEBSITES_SSL', 3, 'FuseCP.EnterpriseServer.CheckWebsitesSslTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_DELETE_EXCHANGE_ACCOUNTS', 3, 'FuseCP.EnterpriseServer.DeleteExchangeAccountsTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_DOMAIN_EXPIRATION', 3, 'FuseCP.EnterpriseServer.DomainExpirationTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_DOMAIN_LOOKUP', 1, 'FuseCP.EnterpriseServer.DomainLookupViewTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_FTP_FILES', 3, 'FuseCP.EnterpriseServer.FTPFilesTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_GENERATE_INVOICES', 0, 'FuseCP.Ecommerce.EnterpriseServer.GenerateInvoicesTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 2, 'FuseCP.EnterpriseServer.HostedSolutionReportTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 2, 'FuseCP.EnterpriseServer.NotifyOverusedDatabasesTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_RUN_PAYMENT_QUEUE', 0, 'FuseCP.Ecommerce.EnterpriseServer.RunPaymentQueueTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_RUN_SYSTEM_COMMAND', 1, 'FuseCP.EnterpriseServer.RunSystemCommandTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_SEND_MAIL', 3, 'FuseCP.EnterpriseServer.SendMailNotificationTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_SUSPEND_OVERDUE_INVOICES', 0, 'FuseCP.Ecommerce.EnterpriseServer.SuspendOverdueInvoicesTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_SUSPEND_PACKAGES', 2, 'FuseCP.EnterpriseServer.SuspendOverusedPackagesTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_USER_PASSWORD_EXPIRATION_NOTIFICATION', 1, 'FuseCP.EnterpriseServer.UserPasswordExpirationNotificationTask, FuseCP.EnterpriseServer.Code'),
    ('SCHEDULE_TASK_ZIP_FILES', 3, 'FuseCP.EnterpriseServer.ZipFilesTask, FuseCP.EnterpriseServer.Code');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `SystemSettings` (`PropertyName`, `SettingsName`, `PropertyValue`)
    VALUES ('AccessIps', 'AccessIpsSettings', ''),
    ('CanPeerChangeMfa', 'AuthenticationSettings', 'True'),
    ('MfaTokenAppDisplayName', 'AuthenticationSettings', 'FuseCP'),
    ('BackupsPath', 'BackupSettings', 'c:\\HostingBackups'),
    ('SmtpEnableSsl', 'SmtpSettings', 'False'),
    ('SmtpPort', 'SmtpSettings', '25'),
    ('SmtpServer', 'SmtpSettings', '127.0.0.1'),
    ('SmtpUsername', 'SmtpSettings', 'postmaster');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ThemeSettings` (`ThemeSettingID`, `PropertyName`, `PropertyValue`, `SettingsName`, `ThemeID`)
    VALUES (1, 'Light', 'light-theme', 'Style', 1),
    (2, 'Dark', 'dark-theme', 'Style', 1),
    (3, 'Semi Dark', 'semi-dark', 'Style', 1),
    (4, 'Minimal', 'minimal-theme', 'Style', 1),
    (5, '#0727d7', 'headercolor1', 'color-header', 1),
    (6, '#23282c', 'headercolor2', 'color-header', 1),
    (7, '#e10a1f', 'headercolor3', 'color-header', 1),
    (8, '#157d4c', 'headercolor4', 'color-header', 1),
    (9, '#673ab7', 'headercolor5', 'color-header', 1),
    (10, '#795548', 'headercolor6', 'color-header', 1),
    (11, '#d3094e', 'headercolor7', 'color-header', 1),
    (12, '#ff9800', 'headercolor8', 'color-header', 1),
    (13, '#6c85ec', 'sidebarcolor1', 'color-Sidebar', 1),
    (14, '#5b737f', 'sidebarcolor2', 'color-Sidebar', 1),
    (15, '#408851', 'sidebarcolor3', 'color-Sidebar', 1),
    (16, '#230924', 'sidebarcolor4', 'color-Sidebar', 1),
    (17, '#903a85', 'sidebarcolor5', 'color-Sidebar', 1),
    (18, '#a04846', 'sidebarcolor6', 'color-Sidebar', 1),
    (19, '#a65314', 'sidebarcolor7', 'color-Sidebar', 1),
    (20, '#1f0e3b', 'sidebarcolor8', 'color-Sidebar', 1);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `Themes` (`ThemeID`, `DisplayName`, `DisplayOrder`, `Enabled`, `LTRName`, `RTLName`)
    VALUES (1, 'FuseCP v1', 1, 1, 'Default', 'Default');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `Users` (`UserID`, `AdditionalParams`, `Address`, `Changed`, `City`, `Comments`, `CompanyName`, `Country`, `Created`, `EcommerceEnabled`, `Email`, `FailedLogins`, `Fax`, `FirstName`, `HtmlMail`, `InstantMessenger`, `LastName`, `LoginStatusId`, `OneTimePasswordState`, `OwnerID`, `Password`, `PinSecret`, `PrimaryPhone`, `RoleID`, `SecondaryEmail`, `SecondaryPhone`, `State`, `StatusID`, `SubscriberNumber`, `Username`, `Zip`)
    VALUES (1, NULL, '', TIMESTAMP '2010-07-16 10:53:02', '', '', NULL, '', TIMESTAMP '2010-07-16 12:53:02', TRUE, 'serveradmin@myhosting.com', NULL, '', 'Enterprise', TRUE, '', 'Administrator', NULL, NULL, NULL, '', NULL, '', 1, '', '', '', 1, NULL, 'serveradmin', '');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `Versions` (`DatabaseVersion`, `BuildDate`)
    VALUES ('1.0', TIMESTAMP '2010-04-10 00:00:00'),
    ('1.0.1.0', TIMESTAMP '2010-07-16 12:53:03'),
    ('1.0.2.0', TIMESTAMP '2010-09-03 00:00:00'),
    ('1.1.0.9', TIMESTAMP '2010-11-16 00:00:00'),
    ('1.1.2.13', TIMESTAMP '2011-04-15 00:00:00'),
    ('1.2.0.38', TIMESTAMP '2011-07-13 00:00:00'),
    ('1.2.1.6', TIMESTAMP '2012-03-29 00:00:00'),
    ('1.4.9', TIMESTAMP '2024-04-20 00:00:00'),
    ('1.5.1', TIMESTAMP '2024-12-17 00:00:00'),
    ('2.0.0', TIMESTAMP '2025-11-04 00:00:00');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `Packages` (`PackageID`, `BandwidthUpdated`, `PackageComments`, `PackageName`, `ParentPackageID`, `PlanID`, `PurchaseDate`, `ServerID`, `StatusID`, `StatusIDchangeDate`, `UserID`)
    VALUES (1, NULL, '', 'System', NULL, NULL, NULL, NULL, 1, TIMESTAMP '2024-10-12 19:29:19', 1);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `Providers` (`ProviderID`, `DisableAutoDiscovery`, `DisplayName`, `EditorControl`, `GroupID`, `ProviderName`, `ProviderType`)
    VALUES (1, NULL, 'Windows Server 2003', 'Windows2003', 1, 'Windows2003', 'FuseCP.Providers.OS.Windows2003, FuseCP.Providers.OS.Windows2003'),
    (2, NULL, 'Internet Information Services 6.0', 'IIS60', 2, 'IIS60', 'FuseCP.Providers.Web.IIs60, FuseCP.Providers.Web.IIs60'),
    (3, NULL, 'Microsoft FTP Server 6.0', 'MSFTP60', 3, 'MSFTP60', 'FuseCP.Providers.FTP.MsFTP, FuseCP.Providers.FTP.IIs60'),
    (4, NULL, 'MailEnable Server 1.x - 7.x', 'MailEnable', 4, 'MailEnable', 'FuseCP.Providers.Mail.MailEnable, FuseCP.Providers.Mail.MailEnable'),
    (5, NULL, 'Microsoft SQL Server 2000', 'MSSQL', 5, 'MSSQL', 'FuseCP.Providers.Database.MsSqlServer, FuseCP.Providers.Database.SqlServer'),
    (6, NULL, 'MySQL Server 4.x', 'MySQL', 6, 'MySQL', 'FuseCP.Providers.Database.MySqlServer, FuseCP.Providers.Database.MySQL'),
    (7, NULL, 'Microsoft DNS Server', 'MSDNS', 7, 'MSDNS', 'FuseCP.Providers.DNS.MsDNS, FuseCP.Providers.DNS.MsDNS'),
    (8, NULL, 'AWStats Statistics Service', 'AWStats', 8, 'AWStats', 'FuseCP.Providers.Statistics.AWStats, FuseCP.Providers.Statistics.AWStats'),
    (9, NULL, 'SimpleDNS Plus 4.x', 'SimpleDNS', 7, 'SimpleDNS', 'FuseCP.Providers.DNS.SimpleDNS, FuseCP.Providers.DNS.SimpleDNS'),
    (10, NULL, 'SmarterStats 3.x', 'SmarterStats', 8, 'SmarterStats', 'FuseCP.Providers.Statistics.SmarterStats, FuseCP.Providers.Statistics.SmarterStats'),
    (11, NULL, 'SmarterMail 2.x', 'SmarterMail', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail2, FuseCP.Providers.Mail.SmarterMail2'),
    (12, NULL, 'Gene6 FTP Server 3.x', 'Gene6FTP', 3, 'Gene6FTP', 'FuseCP.Providers.FTP.Gene6, FuseCP.Providers.FTP.Gene6'),
    (13, NULL, 'Merak Mail Server 8.0.3 - 9.2.x', 'Merak', 4, 'Merak', 'FuseCP.Providers.Mail.Merak, FuseCP.Providers.Mail.Merak'),
    (14, NULL, 'SmarterMail 3.x - 4.x', 'SmarterMail', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail3, FuseCP.Providers.Mail.SmarterMail3'),
    (16, NULL, 'Microsoft SQL Server 2005', 'MSSQL', 10, 'MSSQL', 'FuseCP.Providers.Database.MsSqlServer2005, FuseCP.Providers.Database.SqlServer'),
    (17, NULL, 'MySQL Server 5.0', 'MySQL', 11, 'MySQL', 'FuseCP.Providers.Database.MySqlServer50, FuseCP.Providers.Database.MySQL'),
    (18, NULL, 'MDaemon 9.x - 11.x', 'MDaemon', 4, 'MDaemon', 'FuseCP.Providers.Mail.MDaemon, FuseCP.Providers.Mail.MDaemon'),
    (19, TRUE, 'ArGoSoft Mail Server 1.x', 'ArgoMail', 4, 'ArgoMail', 'FuseCP.Providers.Mail.ArgoMail, FuseCP.Providers.Mail.ArgoMail'),
    (20, NULL, 'hMailServer 4.2', 'hMailServer', 4, 'hMailServer', 'FuseCP.Providers.Mail.hMailServer, FuseCP.Providers.Mail.hMailServer'),
    (21, NULL, 'Ability Mail Server 2.x', 'AbilityMailServer', 4, 'AbilityMailServer', 'FuseCP.Providers.Mail.AbilityMailServer, FuseCP.Providers.Mail.AbilityMailServer'),
    (22, NULL, 'hMailServer 4.3', 'hMailServer43', 4, 'hMailServer43', 'FuseCP.Providers.Mail.hMailServer43, FuseCP.Providers.Mail.hMailServer43'),
    (24, NULL, 'ISC BIND 8.x - 9.x', 'Bind', 7, 'Bind', 'FuseCP.Providers.DNS.IscBind, FuseCP.Providers.DNS.Bind'),
    (25, NULL, 'Serv-U FTP 6.x', 'ServU', 3, 'ServU', 'FuseCP.Providers.FTP.ServU, FuseCP.Providers.FTP.ServU'),
    (26, NULL, 'FileZilla FTP Server 0.9', 'FileZilla', 3, 'FileZilla', 'FuseCP.Providers.FTP.FileZilla, FuseCP.Providers.FTP.FileZilla'),
    (27, NULL, 'Hosted Microsoft Exchange Server 2007', 'Exchange', 12, 'Exchange2007', 'FuseCP.Providers.HostedSolution.Exchange2007, FuseCP.Providers.HostedSolution'),
    (28, NULL, 'SimpleDNS Plus 5.x', 'SimpleDNS', 7, 'SimpleDNS', 'FuseCP.Providers.DNS.SimpleDNS5, FuseCP.Providers.DNS.SimpleDNS50'),
    (29, NULL, 'SmarterMail 5.x', 'SmarterMail50', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail5, FuseCP.Providers.Mail.SmarterMail5'),
    (30, NULL, 'MySQL Server 5.1', 'MySQL', 11, 'MySQL', 'FuseCP.Providers.Database.MySqlServer51, FuseCP.Providers.Database.MySQL'),
    (31, NULL, 'SmarterStats 4.x', 'SmarterStats', 8, 'SmarterStats', 'FuseCP.Providers.Statistics.SmarterStats4, FuseCP.Providers.Statistics.SmarterStats'),
    (32, NULL, 'Hosted Microsoft Exchange Server 2010', 'Exchange', 12, 'Exchange2010', 'FuseCP.Providers.HostedSolution.Exchange2010, FuseCP.Providers.HostedSolution'),
    (56, TRUE, 'PowerDNS', 'PowerDNS', 7, 'PowerDNS', 'FuseCP.Providers.DNS.PowerDNS, FuseCP.Providers.DNS.PowerDNS'),
    (60, NULL, 'SmarterMail 6.x', 'SmarterMail60', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail6, FuseCP.Providers.Mail.SmarterMail6'),
    (61, NULL, 'Merak Mail Server 10.x', 'Merak', 4, 'Merak', 'FuseCP.Providers.Mail.Merak10, FuseCP.Providers.Mail.Merak10'),
    (62, NULL, 'SmarterStats 5.x +', 'SmarterStats', 8, 'SmarterStats', 'FuseCP.Providers.Statistics.SmarterStats5, FuseCP.Providers.Statistics.SmarterStats'),
    (63, NULL, 'hMailServer 5.x', 'hMailServer5', 4, 'hMailServer5', 'FuseCP.Providers.Mail.hMailServer5, FuseCP.Providers.Mail.hMailServer5'),
    (64, NULL, 'SmarterMail 7.x - 8.x', 'SmarterMail60', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail7, FuseCP.Providers.Mail.SmarterMail7'),
    (65, NULL, 'SmarterMail 9.x', 'SmarterMail60', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail9, FuseCP.Providers.Mail.SmarterMail9'),
    (66, NULL, 'SmarterMail 10.x +', 'SmarterMail100', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail10, FuseCP.Providers.Mail.SmarterMail10'),
    (67, NULL, 'SmarterMail 100.x +', 'SmarterMail100x', 4, 'SmarterMail', 'FuseCP.Providers.Mail.SmarterMail100, FuseCP.Providers.Mail.SmarterMail100'),
    (90, NULL, 'Hosted Microsoft Exchange Server 2010 SP2', 'Exchange', 12, 'Exchange2010SP2', 'FuseCP.Providers.HostedSolution.Exchange2010SP2, FuseCP.Providers.HostedSolution'),
    (91, NULL, 'Hosted Microsoft Exchange Server 2013', 'Exchange', 12, 'Exchange2013', 'FuseCP.Providers.HostedSolution.Exchange2013, FuseCP.Providers.HostedSolution.Exchange2013');
    INSERT INTO `Providers` (`ProviderID`, `DisableAutoDiscovery`, `DisplayName`, `EditorControl`, `GroupID`, `ProviderName`, `ProviderType`)
    VALUES (92, NULL, 'Hosted Microsoft Exchange Server 2016', 'Exchange', 12, 'Exchange2016', 'FuseCP.Providers.HostedSolution.Exchange2016, FuseCP.Providers.HostedSolution.Exchange2016'),
    (93, NULL, 'Hosted Microsoft Exchange Server 2019', 'Exchange', 12, 'Exchange2016', 'FuseCP.Providers.HostedSolution.Exchange2019, FuseCP.Providers.HostedSolution.Exchange2019'),
    (100, NULL, 'Windows Server 2008', 'Windows2008', 1, 'Windows2008', 'FuseCP.Providers.OS.Windows2008, FuseCP.Providers.OS.Windows2008'),
    (101, NULL, 'Internet Information Services 7.0', 'IIS70', 2, 'IIS70', 'FuseCP.Providers.Web.IIs70, FuseCP.Providers.Web.IIs70'),
    (102, NULL, 'Microsoft FTP Server 7.0', 'MSFTP70', 3, 'MSFTP70', 'FuseCP.Providers.FTP.MsFTP, FuseCP.Providers.FTP.IIs70'),
    (103, NULL, 'Hosted Organizations', 'Organizations', 13, 'Organizations', 'FuseCP.Providers.HostedSolution.OrganizationProvider, FuseCP.Providers.HostedSolution'),
    (104, NULL, 'Windows Server 2012', 'Windows2012', 1, 'Windows2012', 'FuseCP.Providers.OS.Windows2012, FuseCP.Providers.OS.Windows2012'),
    (105, NULL, 'Internet Information Services 8.0', 'IIS70', 2, 'IIS80', 'FuseCP.Providers.Web.IIs80, FuseCP.Providers.Web.IIs80'),
    (106, NULL, 'Microsoft FTP Server 8.0', 'MSFTP70', 3, 'MSFTP80', 'FuseCP.Providers.FTP.MsFTP80, FuseCP.Providers.FTP.IIs80'),
    (110, NULL, 'Cerberus FTP Server 6.x', 'CerberusFTP6', 3, 'CerberusFTP6', 'FuseCP.Providers.FTP.CerberusFTP6, FuseCP.Providers.FTP.CerberusFTP6'),
    (111, NULL, 'Windows Server 2016', 'Windows2008', 1, 'Windows2016', 'FuseCP.Providers.OS.Windows2016, FuseCP.Providers.OS.Windows2016'),
    (112, NULL, 'Internet Information Services 10.0', 'IIS70', 2, 'IIS100', 'FuseCP.Providers.Web.IIs100, FuseCP.Providers.Web.IIs100'),
    (113, NULL, 'Microsoft FTP Server 10.0', 'MSFTP70', 3, 'MSFTP100', 'FuseCP.Providers.FTP.MsFTP100, FuseCP.Providers.FTP.IIs100'),
    (160, NULL, 'IceWarp Mail Server', 'IceWarp', 4, 'IceWarp', 'FuseCP.Providers.Mail.IceWarp, FuseCP.Providers.Mail.IceWarp'),
    (200, NULL, 'Hosted Windows SharePoint Services 3.0', 'HostedSharePoint30', 20, 'HostedSharePoint30', 'FuseCP.Providers.HostedSolution.HostedSharePointServer, FuseCP.Providers.HostedSolution'),
    (202, NULL, 'Microsoft SQL Server 2008', 'MSSQL', 22, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2008, FuseCP.Providers.Database.SqlServer'),
    (203, TRUE, 'BlackBerry 4.1', 'BlackBerry', 31, 'BlackBerry 4.1', 'FuseCP.Providers.HostedSolution.BlackBerryProvider, FuseCP.Providers.HostedSolution'),
    (204, TRUE, 'BlackBerry 5.0', 'BlackBerry5', 31, 'BlackBerry 5.0', 'FuseCP.Providers.HostedSolution.BlackBerry5Provider, FuseCP.Providers.HostedSolution'),
    (205, TRUE, 'Office Communications Server 2007 R2', 'OCS', 32, 'OCS', 'FuseCP.Providers.HostedSolution.OCS2007R2, FuseCP.Providers.HostedSolution'),
    (206, TRUE, 'OCS Edge server', 'OCS_Edge', 32, 'OCSEdge', 'FuseCP.Providers.HostedSolution.OCSEdge2007R2, FuseCP.Providers.HostedSolution'),
    (208, NULL, 'Hosted SharePoint Foundation 2010', 'HostedSharePoint30', 20, 'HostedSharePoint2010', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2010, FuseCP.Providers.HostedSolution'),
    (209, NULL, 'Microsoft SQL Server 2012', 'MSSQL', 23, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2012, FuseCP.Providers.Database.SqlServer'),
    (250, NULL, 'Microsoft Lync Server 2010 Multitenant Hosting Pack', 'Lync', 41, 'Lync2010', 'FuseCP.Providers.HostedSolution.Lync2010, FuseCP.Providers.HostedSolution'),
    (300, TRUE, 'Microsoft Hyper-V', 'HyperV', 30, 'HyperV', 'FuseCP.Providers.Virtualization.HyperV, FuseCP.Providers.Virtualization.HyperV'),
    (301, NULL, 'MySQL Server 5.5', 'MySQL', 11, 'MySQL', 'FuseCP.Providers.Database.MySqlServer55, FuseCP.Providers.Database.MySQL'),
    (302, NULL, 'MySQL Server 5.6', 'MySQL', 11, 'MySQL', 'FuseCP.Providers.Database.MySqlServer56, FuseCP.Providers.Database.MySQL'),
    (303, NULL, 'MySQL Server 5.7', 'MySQL', 11, 'MySQL', 'FuseCP.Providers.Database.MySqlServer57, FuseCP.Providers.Database.MySQL'),
    (304, NULL, 'MySQL Server 8.0', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer80, FuseCP.Providers.Database.MySQL'),
    (305, NULL, 'MySQL Server 8.1', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer81, FuseCP.Providers.Database.MySQL'),
    (306, NULL, 'MySQL Server 8.2', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer82, FuseCP.Providers.Database.MySQL'),
    (307, NULL, 'MySQL Server 8.3', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer83, FuseCP.Providers.Database.MySQL'),
    (308, NULL, 'MySQL Server 8.4', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer84, FuseCP.Providers.Database.MySQL'),
    (320, NULL, 'MySQL Server 9.0', 'MySQL', 90, 'MySQL', 'FuseCP.Providers.Database.MySqlServer90, FuseCP.Providers.Database.MySQL'),
    (350, TRUE, 'Microsoft Hyper-V 2012 R2', 'HyperV2012R2', 33, 'HyperV2012R2', 'FuseCP.Providers.Virtualization.HyperV2012R2, FuseCP.Providers.Virtualization.HyperV2012R2'),
    (351, TRUE, 'Microsoft Hyper-V Virtual Machine Management', 'HyperVvmm', 33, 'HyperVvmm', 'FuseCP.Providers.Virtualization.HyperVvmm, FuseCP.Providers.Virtualization.HyperVvmm'),
    (352, TRUE, 'Microsoft Hyper-V 2016', 'HyperV2012R2', 33, 'HyperV2016', 'FuseCP.Providers.Virtualization.HyperV2016, FuseCP.Providers.Virtualization.HyperV2016'),
    (370, TRUE, 'Proxmox Virtualization (remote)', 'Proxmox', 167, 'Proxmox (remote)', 'FuseCP.Providers.Virtualization.Proxmoxvps, FuseCP.Providers.Virtualization.Proxmoxvps'),
    (371, FALSE, 'Proxmox Virtualization', 'Proxmox', 167, 'Proxmox', 'FuseCP.Providers.Virtualization.ProxmoxvpsLocal, FuseCP.Providers.Virtualization.Proxmoxvps'),
    (400, TRUE, 'Microsoft Hyper-V For Private Cloud', 'HyperVForPrivateCloud', 40, 'HyperVForPC', 'FuseCP.Providers.VirtualizationForPC.HyperVForPC, FuseCP.Providers.VirtualizationForPC.HyperVForPC'),
    (410, NULL, 'Microsoft DNS Server 2012+', 'MSDNS', 7, 'MSDNS.2012', 'FuseCP.Providers.DNS.MsDNS2012, FuseCP.Providers.DNS.MsDNS2012'),
    (500, NULL, 'Unix System', 'Unix', 1, 'UnixSystem', 'FuseCP.Providers.OS.Unix, FuseCP.Providers.OS.Unix');
    INSERT INTO `Providers` (`ProviderID`, `DisableAutoDiscovery`, `DisplayName`, `EditorControl`, `GroupID`, `ProviderName`, `ProviderType`)
    VALUES (600, NULL, 'Enterprise Storage Windows 2012', 'EnterpriseStorage', 44, 'EnterpriseStorage2012', 'FuseCP.Providers.EnterpriseStorage.Windows2012, FuseCP.Providers.EnterpriseStorage.Windows2012'),
    (700, NULL, 'Storage Spaces Windows 2012', 'StorageSpaceServices', 49, 'StorageSpace2012', 'FuseCP.Providers.StorageSpaces.Windows2012, FuseCP.Providers.StorageSpaces.Windows2012'),
    (1203, NULL, 'Microsoft SQL Server 2014', 'MSSQL', 46, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2014, FuseCP.Providers.Database.SqlServer'),
    (1301, NULL, 'Hosted SharePoint Foundation 2013', 'HostedSharePoint30', 20, 'HostedSharePoint2013', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2013, FuseCP.Providers.HostedSolution.SharePoint2013'),
    (1306, NULL, 'Hosted SharePoint Foundation 2016', 'HostedSharePoint30', 20, 'HostedSharePoint2016', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2016, FuseCP.Providers.HostedSolution.SharePoint2016'),
    (1401, NULL, 'Microsoft Lync Server 2013 Enterprise Edition', 'Lync', 41, 'Lync2013', 'FuseCP.Providers.HostedSolution.Lync2013, FuseCP.Providers.HostedSolution.Lync2013'),
    (1402, NULL, 'Microsoft Lync Server 2013 Multitenant Hosting Pack', 'Lync', 41, 'Lync2013HP', 'FuseCP.Providers.HostedSolution.Lync2013HP, FuseCP.Providers.HostedSolution.Lync2013HP'),
    (1403, NULL, 'Microsoft Skype for Business Server 2015', 'SfB', 52, 'SfB2015', 'FuseCP.Providers.HostedSolution.SfB2015, FuseCP.Providers.HostedSolution.SfB2015'),
    (1404, NULL, 'Microsoft Skype for Business Server 2019', 'SfB', 52, 'SfB2019', 'FuseCP.Providers.HostedSolution.SfB2019, FuseCP.Providers.HostedSolution.SfB2019'),
    (1501, TRUE, 'Remote Desktop Services Windows 2012', 'RDS', 45, 'RemoteDesktopServices2012', 'FuseCP.Providers.RemoteDesktopServices.Windows2012,FuseCP.Providers.RemoteDesktopServices.Windows2012'),
    (1502, TRUE, 'Remote Desktop Services Windows 2016', 'RDS', 45, 'RemoteDesktopServices2012', 'FuseCP.Providers.RemoteDesktopServices.Windows2016,FuseCP.Providers.RemoteDesktopServices.Windows2016'),
    (1503, TRUE, 'Remote Desktop Services Windows 2019', 'RDS', 45, 'RemoteDesktopServices2019', 'FuseCP.Providers.RemoteDesktopServices.Windows2019,FuseCP.Providers.RemoteDesktopServices.Windows2019'),
    (1504, TRUE, 'Remote Desktop Services Windows 2022', 'RDS', 45, 'RemoteDesktopServices2022', 'FuseCP.Providers.RemoteDesktopServices.Windows2022,FuseCP.Providers.RemoteDesktopServices.Windows2022'),
    (1505, TRUE, 'Remote Desktop Services Windows 2025', 'RDS', 45, 'RemoteDesktopServices2025', 'FuseCP.Providers.RemoteDesktopServices.Windows2025,FuseCP.Providers.RemoteDesktopServices.Windows2025'),
    (1550, NULL, 'MariaDB 10.1', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB101, FuseCP.Providers.Database.MariaDB'),
    (1552, NULL, 'Hosted SharePoint Enterprise 2013', 'HostedSharePoint30', 73, 'HostedSharePoint2013Ent', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2013Ent, FuseCP.Providers.HostedSolution.SharePoint2013Ent'),
    (1560, NULL, 'MariaDB 10.2', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB102, FuseCP.Providers.Database.MariaDB'),
    (1570, NULL, 'MariaDB 10.3', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB103, FuseCP.Providers.Database.MariaDB'),
    (1571, NULL, 'MariaDB 10.4', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB104, FuseCP.Providers.Database.MariaDB'),
    (1572, NULL, 'MariaDB 10.5', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB105, FuseCP.Providers.Database.MariaDB'),
    (1573, NULL, 'MariaDB 10.6', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB106, FuseCP.Providers.Database.MariaDB'),
    (1574, NULL, 'MariaDB 10.7', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB107, FuseCP.Providers.Database.MariaDB'),
    (1575, NULL, 'MariaDB 10.8', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB108, FuseCP.Providers.Database.MariaDB'),
    (1576, NULL, 'MariaDB 10.9', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB109, FuseCP.Providers.Database.MariaDB'),
    (1577, NULL, 'MariaDB 10.10', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB1010, FuseCP.Providers.Database.MariaDB'),
    (1578, NULL, 'MariaDB 10.11', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB1011, FuseCP.Providers.Database.MariaDB'),
    (1579, NULL, 'MariaDB 11.0', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB110, FuseCP.Providers.Database.MariaDB'),
    (1580, NULL, 'MariaDB 11.1', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB111, FuseCP.Providers.Database.MariaDB'),
    (1581, NULL, 'MariaDB 11.2', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB112, FuseCP.Providers.Database.MariaDB'),
    (1582, NULL, 'MariaDB 11.3', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB113, FuseCP.Providers.Database.MariaDB'),
    (1583, NULL, 'MariaDB 11.4', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB114, FuseCP.Providers.Database.MariaDB'),
    (1584, NULL, 'MariaDB 11.5', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB115, FuseCP.Providers.Database.MariaDB'),
    (1585, NULL, 'MariaDB 11.6', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB116, FuseCP.Providers.Database.MariaDB'),
    (1586, NULL, 'MariaDB 11.7', 'MariaDB', 50, 'MariaDB', 'FuseCP.Providers.Database.MariaDB117, FuseCP.Providers.Database.MariaDB'),
    (1601, TRUE, 'Mail Cleaner', 'MailCleaner', 61, 'MailCleaner', 'FuseCP.Providers.Filters.MailCleaner, FuseCP.Providers.Filters.MailCleaner'),
    (1602, TRUE, 'SpamExperts Mail Filter', 'SpamExperts', 61, 'SpamExperts', 'FuseCP.Providers.Filters.SpamExperts, FuseCP.Providers.Filters.SpamExperts'),
    (1701, NULL, 'Microsoft SQL Server 2016', 'MSSQL', 71, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2016, FuseCP.Providers.Database.SqlServer'),
    (1702, NULL, 'Hosted SharePoint Enterprise 2016', 'HostedSharePoint30', 73, 'HostedSharePoint2016Ent', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2016Ent, FuseCP.Providers.HostedSolution.SharePoint2016Ent');
    INSERT INTO `Providers` (`ProviderID`, `DisableAutoDiscovery`, `DisplayName`, `EditorControl`, `GroupID`, `ProviderName`, `ProviderType`)
    VALUES (1703, NULL, 'SimpleDNS Plus 6.x', 'SimpleDNS', 7, 'SimpleDNS', 'FuseCP.Providers.DNS.SimpleDNS6, FuseCP.Providers.DNS.SimpleDNS60'),
    (1704, NULL, 'Microsoft SQL Server 2017', 'MSSQL', 72, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2017, FuseCP.Providers.Database.SqlServer'),
    (1705, NULL, 'Microsoft SQL Server 2019', 'MSSQL', 74, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2019, FuseCP.Providers.Database.SqlServer'),
    (1706, NULL, 'Microsoft SQL Server 2022', 'MSSQL', 75, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2022, FuseCP.Providers.Database.SqlServer'),
    (1707, NULL, 'Microsoft SQL Server 2025', 'MSSQL', 76, 'MsSQL', 'FuseCP.Providers.Database.MsSqlServer2025, FuseCP.Providers.Database.SqlServer'),
    (1711, NULL, 'Hosted SharePoint 2019', 'HostedSharePoint30', 73, 'HostedSharePoint2019', 'FuseCP.Providers.HostedSolution.HostedSharePointServer2019, FuseCP.Providers.HostedSolution.SharePoint2019'),
    (1800, NULL, 'Windows Server 2019', 'Windows2012', 1, 'Windows2019', 'FuseCP.Providers.OS.Windows2019, FuseCP.Providers.OS.Windows2019'),
    (1801, TRUE, 'Microsoft Hyper-V 2019', 'HyperV2012R2', 33, 'HyperV2019', 'FuseCP.Providers.Virtualization.HyperV2019, FuseCP.Providers.Virtualization.HyperV2019'),
    (1802, NULL, 'Windows Server 2022', 'Windows2012', 1, 'Windows2022', 'FuseCP.Providers.OS.Windows2022, FuseCP.Providers.OS.Windows2022'),
    (1803, TRUE, 'Microsoft Hyper-V 2022', 'HyperV2012R2', 33, 'HyperV2022', 'FuseCP.Providers.Virtualization.HyperV2022, FuseCP.Providers.Virtualization.HyperV2022'),
    (1804, NULL, 'Windows Server 2025', 'Windows2012', 1, 'Windows2025', 'FuseCP.Providers.OS.Windows2025, FuseCP.Providers.OS.Windows2025'),
    (1805, TRUE, 'Microsoft Hyper-V 2025', 'HyperV2012R2', 33, 'HyperV2025', 'FuseCP.Providers.Virtualization.HyperV2025, FuseCP.Providers.Virtualization.HyperV2025'),
    (1901, NULL, 'SimpleDNS Plus 8.x', 'SimpleDNS', 7, 'SimpleDNS', 'FuseCP.Providers.DNS.SimpleDNS8, FuseCP.Providers.DNS.SimpleDNS80'),
    (1902, NULL, 'Microsoft DNS Server 2016', 'MSDNS', 7, 'MSDNS.2016', 'FuseCP.Providers.DNS.MsDNS2016, FuseCP.Providers.DNS.MsDNS2016'),
    (1903, NULL, 'SimpleDNS Plus 9.x', 'SimpleDNS', 7, 'SimpleDNS', 'FuseCP.Providers.DNS.SimpleDNS9, FuseCP.Providers.DNS.SimpleDNS90'),
    (1910, NULL, 'vsftpd FTP Server 3', 'vsftpd', 3, 'vsftpd', 'FuseCP.Providers.FTP.VsFtp3, FuseCP.Providers.FTP.VsFtp'),
    (1911, NULL, 'Apache Web Server 2.4', 'Apache', 2, 'Apache', 'FuseCP.Providers.Web.Apache24, FuseCP.Providers.Web.Apache');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `Quotas` (`QuotaID`, `GroupID`, `HideQuota`, `ItemTypeID`, `PerOrganization`, `QuotaDescription`, `QuotaName`, `QuotaOrder`, `QuotaTypeID`, `ServiceQuota`)
    VALUES (25, 2, NULL, NULL, NULL, 'ASP.NET 1.1', 'Web.AspNet11', 3.0, 1, FALSE),
    (26, 2, NULL, NULL, NULL, 'ASP.NET 2.0', 'Web.AspNet20', 4.0, 1, FALSE),
    (27, 2, NULL, NULL, NULL, 'ASP', 'Web.Asp', 2.0, 1, FALSE),
    (28, 2, NULL, NULL, NULL, 'PHP 4.x', 'Web.Php4', 5.0, 1, FALSE),
    (29, 2, NULL, NULL, NULL, 'PHP 5.x', 'Web.Php5', 6.0, 1, FALSE),
    (30, 2, NULL, NULL, NULL, 'Perl', 'Web.Perl', 7.0, 1, FALSE),
    (31, 2, NULL, NULL, NULL, 'Python', 'Web.Python', 8.0, 1, FALSE),
    (32, 2, NULL, NULL, NULL, 'Virtual Directories', 'Web.VirtualDirs', 9.0, 1, FALSE),
    (33, 2, NULL, NULL, NULL, 'FrontPage', 'Web.FrontPage', 10.0, 1, FALSE),
    (34, 2, NULL, NULL, NULL, 'Custom Security Settings', 'Web.Security', 11.0, 1, FALSE),
    (35, 2, NULL, NULL, NULL, 'Custom Default Documents', 'Web.DefaultDocs', 12.0, 1, FALSE),
    (36, 2, NULL, NULL, NULL, 'Dedicated Application Pools', 'Web.AppPools', 13.0, 1, FALSE),
    (37, 2, NULL, NULL, NULL, 'Custom Headers', 'Web.Headers', 14.0, 1, FALSE),
    (38, 2, NULL, NULL, NULL, 'Custom Errors', 'Web.Errors', 15.0, 1, FALSE),
    (39, 2, NULL, NULL, NULL, 'Custom MIME Types', 'Web.Mime', 16.0, 1, FALSE),
    (40, 4, NULL, NULL, NULL, 'Max Mailbox Size', 'Mail.MaxBoxSize', 2.0, 3, FALSE),
    (41, 5, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2000.MaxDatabaseSize', 3.0, 3, FALSE),
    (42, 5, NULL, NULL, NULL, 'Database Backups', 'MsSQL2000.Backup', 5.0, 1, FALSE),
    (43, 5, NULL, NULL, NULL, 'Database Restores', 'MsSQL2000.Restore', 6.0, 1, FALSE),
    (44, 5, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2000.Truncate', 7.0, 1, FALSE),
    (45, 6, NULL, NULL, NULL, 'Database Backups', 'MySQL4.Backup', 4.0, 1, FALSE),
    (48, 7, NULL, NULL, NULL, 'DNS Editor', 'DNS.Editor', 1.0, 1, FALSE),
    (49, 4, NULL, NULL, NULL, 'Max Group Recipients', 'Mail.MaxGroupMembers', 5.0, 3, FALSE),
    (50, 4, NULL, NULL, NULL, 'Max List Recipients', 'Mail.MaxListMembers', 7.0, 3, FALSE),
    (51, 1, NULL, NULL, NULL, 'Bandwidth, MB', 'OS.Bandwidth', 2.0, 2, FALSE),
    (52, 1, NULL, NULL, NULL, 'Disk space, MB', 'OS.Diskspace', 1.0, 2, FALSE),
    (53, 1, NULL, NULL, NULL, 'Domains', 'OS.Domains', 3.0, 2, FALSE),
    (54, 1, NULL, NULL, NULL, 'Sub-Domains', 'OS.SubDomains', 4.0, 2, FALSE),
    (55, 1, NULL, NULL, NULL, 'File Manager', 'OS.FileManager', 6.0, 1, FALSE),
    (57, 2, NULL, NULL, NULL, 'CGI-BIN Folder', 'Web.CgiBin', 8.0, 1, FALSE),
    (58, 2, NULL, NULL, NULL, 'Secured Folders', 'Web.SecuredFolders', 8.0, 1, FALSE),
    (60, 2, NULL, NULL, NULL, 'Web Sites Redirection', 'Web.Redirections', 8.0, 1, FALSE),
    (61, 2, NULL, NULL, NULL, 'Changing Sites Root Folders', 'Web.HomeFolders', 8.0, 1, FALSE),
    (64, 10, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2005.MaxDatabaseSize', 3.0, 3, FALSE),
    (65, 10, NULL, NULL, NULL, 'Database Backups', 'MsSQL2005.Backup', 5.0, 1, FALSE),
    (66, 10, NULL, NULL, NULL, 'Database Restores', 'MsSQL2005.Restore', 6.0, 1, FALSE),
    (67, 10, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2005.Truncate', 7.0, 1, FALSE),
    (70, 11, NULL, NULL, NULL, 'Database Backups', 'MySQL5.Backup', 4.0, 1, FALSE),
    (71, 1, NULL, NULL, NULL, 'Scheduled Tasks', 'OS.ScheduledTasks', 9.0, 2, FALSE),
    (72, 1, NULL, NULL, NULL, 'Interval Tasks Allowed', 'OS.ScheduledIntervalTasks', 10.0, 1, FALSE),
    (73, 1, NULL, NULL, NULL, 'Minimum Tasks Interval, minutes', 'OS.MinimumTaskInterval', 11.0, 3, FALSE),
    (74, 1, NULL, NULL, NULL, 'Applications Installer', 'OS.AppInstaller', 7.0, 1, FALSE);
    INSERT INTO `Quotas` (`QuotaID`, `GroupID`, `HideQuota`, `ItemTypeID`, `PerOrganization`, `QuotaDescription`, `QuotaName`, `QuotaOrder`, `QuotaTypeID`, `ServiceQuota`)
    VALUES (75, 1, NULL, NULL, NULL, 'Extra Application Packs', 'OS.ExtraApplications', 8.0, 1, FALSE),
    (77, 12, NULL, NULL, 1, 'Organization Disk Space, MB', 'Exchange2007.DiskSpace', 2.0, 2, FALSE),
    (78, 12, NULL, NULL, 1, 'Mailboxes per Organization', 'Exchange2007.Mailboxes', 3.0, 2, FALSE),
    (79, 12, NULL, NULL, 1, 'Contacts per Organization', 'Exchange2007.Contacts', 4.0, 3, FALSE),
    (80, 12, NULL, NULL, 1, 'Distribution Lists per Organization', 'Exchange2007.DistributionLists', 5.0, 3, FALSE),
    (81, 12, NULL, NULL, 1, 'Public Folders per Organization', 'Exchange2007.PublicFolders', 6.0, 3, FALSE),
    (83, 12, NULL, NULL, NULL, 'POP3 Access', 'Exchange2007.POP3Allowed', 9.0, 1, FALSE),
    (84, 12, NULL, NULL, NULL, 'IMAP Access', 'Exchange2007.IMAPAllowed', 11.0, 1, FALSE),
    (85, 12, NULL, NULL, NULL, 'OWA/HTTP Access', 'Exchange2007.OWAAllowed', 13.0, 1, FALSE),
    (86, 12, NULL, NULL, NULL, 'MAPI Access', 'Exchange2007.MAPIAllowed', 15.0, 1, FALSE),
    (87, 12, NULL, NULL, NULL, 'ActiveSync Access', 'Exchange2007.ActiveSyncAllowed', 17.0, 1, FALSE),
    (88, 12, NULL, NULL, NULL, 'Mail Enabled Public Folders Allowed', 'Exchange2007.MailEnabledPublicFolders', 8.0, 1, FALSE),
    (94, 2, NULL, NULL, NULL, 'ColdFusion', 'Web.ColdFusion', 17.0, 1, FALSE),
    (96, 2, NULL, NULL, NULL, 'ColdFusion Virtual Directories', 'Web.CFVirtualDirectories', 18.0, 1, FALSE),
    (97, 2, NULL, NULL, NULL, 'Remote web management allowed', 'Web.RemoteManagement', 20.0, 1, FALSE),
    (100, 2, NULL, NULL, NULL, 'Dedicated IP Addresses', 'Web.IPAddresses', 19.0, 2, TRUE),
    (102, 4, NULL, NULL, NULL, 'Disable Mailbox Size Edit', 'Mail.DisableSizeEdit', 8.0, 1, FALSE),
    (103, 6, NULL, NULL, NULL, 'Max Database Size', 'MySQL4.MaxDatabaseSize', 3.0, 3, FALSE),
    (104, 6, NULL, NULL, NULL, 'Database Restores', 'MySQL4.Restore', 5.0, 1, FALSE),
    (105, 6, NULL, NULL, NULL, 'Database Truncate', 'MySQL4.Truncate', 6.0, 1, FALSE),
    (106, 11, NULL, NULL, NULL, 'Max Database Size', 'MySQL5.MaxDatabaseSize', 3.0, 3, FALSE),
    (107, 11, NULL, NULL, NULL, 'Database Restores', 'MySQL5.Restore', 5.0, 1, FALSE),
    (108, 11, NULL, NULL, NULL, 'Database Truncate', 'MySQL5.Truncate', 6.0, 1, FALSE),
    (112, 90, NULL, NULL, NULL, 'Database Backups', 'MySQL8.Backup', 4.0, 1, FALSE),
    (113, 90, NULL, NULL, NULL, 'Max Database Size', 'MySQL8.MaxDatabaseSize', 3.0, 3, FALSE),
    (114, 90, NULL, NULL, NULL, 'Database Restores', 'MySQL8.Restore', 5.0, 1, FALSE),
    (115, 90, NULL, NULL, NULL, 'Database Truncate', 'MySQL8.Truncate', 6.0, 1, FALSE),
    (122, 91, NULL, NULL, NULL, 'Database Backups', 'MySQL9.Backup', 4.0, 1, FALSE),
    (123, 91, NULL, NULL, NULL, 'Max Database Size', 'MySQL9.MaxDatabaseSize', 3.0, 3, FALSE),
    (124, 91, NULL, NULL, NULL, 'Database Restores', 'MySQL9.Restore', 5.0, 1, FALSE),
    (125, 91, NULL, NULL, NULL, 'Database Truncate', 'MySQL9.Truncate', 6.0, 1, FALSE),
    (203, 10, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2005.MaxLogSize', 4.0, 3, FALSE),
    (204, 5, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2000.MaxLogSize', 4.0, 3, FALSE),
    (207, 13, NULL, NULL, 1, 'Domains per Organizations', 'HostedSolution.Domains', 3.0, 3, FALSE),
    (208, 20, NULL, NULL, NULL, 'Max site storage, MB', 'HostedSharePoint.MaxStorage', 2.0, 3, FALSE),
    (213, 22, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2008.MaxDatabaseSize', 3.0, 3, FALSE),
    (214, 22, NULL, NULL, NULL, 'Database Backups', 'MsSQL2008.Backup', 5.0, 1, FALSE),
    (215, 22, NULL, NULL, NULL, 'Database Restores', 'MsSQL2008.Restore', 6.0, 1, FALSE),
    (216, 22, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2008.Truncate', 7.0, 1, FALSE),
    (217, 22, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2008.MaxLogSize', 4.0, 3, FALSE);
    INSERT INTO `Quotas` (`QuotaID`, `GroupID`, `HideQuota`, `ItemTypeID`, `PerOrganization`, `QuotaDescription`, `QuotaName`, `QuotaOrder`, `QuotaTypeID`, `ServiceQuota`)
    VALUES (220, 1, TRUE, NULL, NULL, 'Domain Pointers', 'OS.DomainPointers', 5.0, 2, FALSE),
    (221, 23, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2012.MaxDatabaseSize', 3.0, 3, FALSE),
    (222, 23, NULL, NULL, NULL, 'Database Backups', 'MsSQL2012.Backup', 5.0, 1, FALSE),
    (223, 23, NULL, NULL, NULL, 'Database Restores', 'MsSQL2012.Restore', 6.0, 1, FALSE),
    (224, 23, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2012.Truncate', 7.0, 1, FALSE),
    (225, 23, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2012.MaxLogSize', 4.0, 3, FALSE),
    (230, 13, NULL, NULL, NULL, 'Allow to Change UserPrincipalName', 'HostedSolution.AllowChangeUPN', 4.0, 1, FALSE),
    (301, 30, NULL, NULL, NULL, 'Allow user to create VPS', 'VPS.ManagingAllowed', 2.0, 1, FALSE),
    (302, 30, NULL, NULL, NULL, 'Number of CPU cores', 'VPS.CpuNumber', 3.0, 2, FALSE),
    (303, 30, NULL, NULL, NULL, 'Boot from CD allowed', 'VPS.BootCdAllowed', 7.0, 1, FALSE),
    (304, 30, NULL, NULL, NULL, 'Boot from CD', 'VPS.BootCdEnabled', 8.0, 1, FALSE),
    (305, 30, NULL, NULL, NULL, 'RAM size, MB', 'VPS.Ram', 4.0, 2, FALSE),
    (306, 30, NULL, NULL, NULL, 'Hard Drive size, GB', 'VPS.Hdd', 5.0, 2, FALSE),
    (307, 30, NULL, NULL, NULL, 'DVD drive', 'VPS.DvdEnabled', 6.0, 1, FALSE),
    (308, 30, NULL, NULL, NULL, 'External Network', 'VPS.ExternalNetworkEnabled', 10.0, 1, FALSE),
    (309, 30, NULL, NULL, NULL, 'Number of External IP addresses', 'VPS.ExternalIPAddressesNumber', 11.0, 2, FALSE),
    (310, 30, NULL, NULL, NULL, 'Private Network', 'VPS.PrivateNetworkEnabled', 13.0, 1, FALSE),
    (311, 30, NULL, NULL, NULL, 'Number of Private IP addresses per VPS', 'VPS.PrivateIPAddressesNumber', 14.0, 3, FALSE),
    (312, 30, NULL, NULL, NULL, 'Number of Snaphots', 'VPS.SnapshotsNumber', 9.0, 3, FALSE),
    (313, 30, NULL, NULL, NULL, 'Allow user to Start, Turn off and Shutdown VPS', 'VPS.StartShutdownAllowed', 15.0, 1, FALSE),
    (314, 30, NULL, NULL, NULL, 'Allow user to Pause, Resume VPS', 'VPS.PauseResumeAllowed', 16.0, 1, FALSE),
    (315, 30, NULL, NULL, NULL, 'Allow user to Reboot VPS', 'VPS.RebootAllowed', 17.0, 1, FALSE),
    (316, 30, NULL, NULL, NULL, 'Allow user to Reset VPS', 'VPS.ResetAlowed', 18.0, 1, FALSE),
    (317, 30, NULL, NULL, NULL, 'Allow user to Re-install VPS', 'VPS.ReinstallAllowed', 19.0, 1, FALSE),
    (318, 30, NULL, NULL, NULL, 'Monthly bandwidth, GB', 'VPS.Bandwidth', 12.0, 2, FALSE),
    (319, 31, NULL, NULL, 1, NULL, 'BlackBerry.Users', 1.0, 2, FALSE),
    (320, 32, NULL, NULL, 1, NULL, 'OCS.Users', 1.0, 2, FALSE),
    (321, 32, NULL, NULL, NULL, NULL, 'OCS.Federation', 2.0, 1, FALSE),
    (322, 32, NULL, NULL, NULL, NULL, 'OCS.FederationByDefault', 3.0, 1, FALSE),
    (323, 32, NULL, NULL, NULL, NULL, 'OCS.PublicIMConnectivity', 4.0, 1, FALSE),
    (324, 32, NULL, NULL, NULL, NULL, 'OCS.PublicIMConnectivityByDefault', 5.0, 1, FALSE),
    (325, 32, NULL, NULL, NULL, NULL, 'OCS.ArchiveIMConversation', 6.0, 1, FALSE),
    (326, 32, NULL, NULL, NULL, NULL, 'OCS.ArchiveIMConvervationByDefault', 7.0, 1, FALSE),
    (327, 32, NULL, NULL, NULL, NULL, 'OCS.ArchiveFederatedIMConversation', 8.0, 1, FALSE),
    (328, 32, NULL, NULL, NULL, NULL, 'OCS.ArchiveFederatedIMConversationByDefault', 9.0, 1, FALSE),
    (329, 32, NULL, NULL, NULL, NULL, 'OCS.PresenceAllowed', 10.0, 1, FALSE),
    (330, 32, NULL, NULL, NULL, NULL, 'OCS.PresenceAllowedByDefault', 10.0, 1, FALSE),
    (331, 2, NULL, NULL, NULL, 'ASP.NET 4.0', 'Web.AspNet40', 4.0, 1, FALSE),
    (332, 2, NULL, NULL, NULL, 'SSL', 'Web.SSL', 21.0, 1, FALSE),
    (333, 2, NULL, NULL, NULL, 'Allow IP Address Mode Switch', 'Web.AllowIPAddressModeSwitch', 22.0, 1, FALSE),
    (334, 2, NULL, NULL, NULL, 'Enable Hostname Support', 'Web.EnableHostNameSupport', 23.0, 1, FALSE),
    (344, 2, NULL, NULL, NULL, 'htaccess', 'Web.Htaccess', 9.0, 1, FALSE);
    INSERT INTO `Quotas` (`QuotaID`, `GroupID`, `HideQuota`, `ItemTypeID`, `PerOrganization`, `QuotaDescription`, `QuotaName`, `QuotaOrder`, `QuotaTypeID`, `ServiceQuota`)
    VALUES (346, 40, NULL, NULL, NULL, 'Allow user to create VPS', 'VPSForPC.ManagingAllowed', 2.0, 1, FALSE),
    (347, 40, NULL, NULL, NULL, 'Number of CPU cores', 'VPSForPC.CpuNumber', 3.0, 2, FALSE),
    (348, 40, NULL, NULL, NULL, 'Boot from CD allowed', 'VPSForPC.BootCdAllowed', 7.0, 1, FALSE),
    (349, 40, NULL, NULL, NULL, 'Boot from CD', 'VPSForPC.BootCdEnabled', 7.0, 1, FALSE),
    (350, 40, NULL, NULL, NULL, 'RAM size, MB', 'VPSForPC.Ram', 4.0, 2, FALSE),
    (351, 40, NULL, NULL, NULL, 'Hard Drive size, GB', 'VPSForPC.Hdd', 5.0, 2, FALSE),
    (352, 40, NULL, NULL, NULL, 'DVD drive', 'VPSForPC.DvdEnabled', 6.0, 1, FALSE),
    (353, 40, NULL, NULL, NULL, 'External Network', 'VPSForPC.ExternalNetworkEnabled', 10.0, 1, FALSE),
    (354, 40, NULL, NULL, NULL, 'Number of External IP addresses', 'VPSForPC.ExternalIPAddressesNumber', 11.0, 2, FALSE),
    (355, 40, NULL, NULL, NULL, 'Private Network', 'VPSForPC.PrivateNetworkEnabled', 13.0, 1, FALSE),
    (356, 40, NULL, NULL, NULL, 'Number of Private IP addresses per VPS', 'VPSForPC.PrivateIPAddressesNumber', 14.0, 3, FALSE),
    (357, 40, NULL, NULL, NULL, 'Number of Snaphots', 'VPSForPC.SnapshotsNumber', 9.0, 3, FALSE),
    (358, 40, NULL, NULL, NULL, 'Allow user to Start, Turn off and Shutdown VPS', 'VPSForPC.StartShutdownAllowed', 15.0, 1, FALSE),
    (359, 40, NULL, NULL, NULL, 'Allow user to Pause, Resume VPS', 'VPSForPC.PauseResumeAllowed', 16.0, 1, FALSE),
    (360, 40, NULL, NULL, NULL, 'Allow user to Reboot VPS', 'VPSForPC.RebootAllowed', 17.0, 1, FALSE),
    (361, 40, NULL, NULL, NULL, 'Allow user to Reset VPS', 'VPSForPC.ResetAlowed', 18.0, 1, FALSE),
    (362, 40, NULL, NULL, NULL, 'Allow user to Re-install VPS', 'VPSForPC.ReinstallAllowed', 19.0, 1, FALSE),
    (363, 40, NULL, NULL, NULL, 'Monthly bandwidth, GB', 'VPSForPC.Bandwidth', 12.0, 2, FALSE),
    (364, 12, NULL, NULL, NULL, 'Keep Deleted Items (days)', 'Exchange2007.KeepDeletedItemsDays', 19.0, 3, FALSE),
    (365, 12, NULL, NULL, NULL, 'Maximum Recipients', 'Exchange2007.MaxRecipients', 20.0, 3, FALSE),
    (366, 12, NULL, NULL, NULL, 'Maximum Send Message Size (Kb)', 'Exchange2007.MaxSendMessageSizeKB', 21.0, 3, FALSE),
    (367, 12, NULL, NULL, NULL, 'Maximum Receive Message Size (Kb)', 'Exchange2007.MaxReceiveMessageSizeKB', 22.0, 3, FALSE),
    (368, 12, NULL, NULL, NULL, 'Is Consumer Organization', 'Exchange2007.IsConsumer', 1.0, 1, FALSE),
    (369, 12, NULL, NULL, NULL, 'Enable Plans Editing', 'Exchange2007.EnablePlansEditing', 23.0, 1, FALSE),
    (370, 41, NULL, NULL, 1, 'Users', 'Lync.Users', 1.0, 2, FALSE),
    (371, 41, NULL, NULL, NULL, 'Allow Federation', 'Lync.Federation', 2.0, 1, FALSE),
    (372, 41, NULL, NULL, NULL, 'Allow Conferencing', 'Lync.Conferencing', 3.0, 1, FALSE),
    (373, 41, NULL, NULL, NULL, 'Maximum Conference Particiapants', 'Lync.MaxParticipants', 4.0, 3, FALSE),
    (374, 41, NULL, NULL, NULL, 'Allow Video in Conference', 'Lync.AllowVideo', 5.0, 1, FALSE),
    (375, 41, NULL, NULL, NULL, 'Allow EnterpriseVoice', 'Lync.EnterpriseVoice', 6.0, 1, FALSE),
    (376, 41, NULL, NULL, NULL, 'Number of Enterprise Voice Users', 'Lync.EVUsers', 7.0, 2, FALSE),
    (377, 41, NULL, NULL, NULL, 'Allow National Calls', 'Lync.EVNational', 8.0, 1, FALSE),
    (378, 41, NULL, NULL, NULL, 'Allow Mobile Calls', 'Lync.EVMobile', 9.0, 1, FALSE),
    (379, 41, NULL, NULL, NULL, 'Allow International Calls', 'Lync.EVInternational', 10.0, 1, FALSE),
    (380, 41, NULL, NULL, NULL, 'Enable Plans Editing', 'Lync.EnablePlansEditing', 11.0, 1, FALSE),
    (381, 41, NULL, NULL, NULL, 'Phone Numbers', 'Lync.PhoneNumbers', 12.0, 2, FALSE),
    (400, 20, NULL, NULL, NULL, 'Use shared SSL Root', 'HostedSharePoint.UseSharedSSL', 3.0, 1, FALSE),
    (409, 1, NULL, NULL, NULL, 'Not allow Tenants to Delete Top Level Domains', 'OS.NotAllowTenantDeleteDomains', 13.0, 1, FALSE),
    (410, 1, NULL, NULL, NULL, 'Not allow Tenants to Create Top Level Domains', 'OS.NotAllowTenantCreateDomains', 12.0, 1, FALSE),
    (411, 2, NULL, NULL, NULL, 'Application Pools Restart', 'Web.AppPoolsRestart', 13.0, 1, FALSE),
    (420, 12, NULL, NULL, NULL, 'Allow Litigation Hold', 'Exchange2007.AllowLitigationHold', 24.0, 1, FALSE),
    (421, 12, NULL, NULL, 1, 'Recoverable Items Space', 'Exchange2007.RecoverableItemsSpace', 25.0, 2, FALSE);
    INSERT INTO `Quotas` (`QuotaID`, `GroupID`, `HideQuota`, `ItemTypeID`, `PerOrganization`, `QuotaDescription`, `QuotaName`, `QuotaOrder`, `QuotaTypeID`, `ServiceQuota`)
    VALUES (422, 12, NULL, NULL, NULL, 'Disclaimers Allowed', 'Exchange2007.DisclaimersAllowed', 26.0, 1, FALSE),
    (423, 13, NULL, NULL, 1, 'Security Groups', 'HostedSolution.SecurityGroups', 5.0, 2, FALSE),
    (424, 12, NULL, NULL, NULL, 'Allow Retention Policy', 'Exchange2013.AllowRetentionPolicy', 27.0, 1, FALSE),
    (425, 12, NULL, NULL, 1, 'Archiving storage, MB', 'Exchange2013.ArchivingStorage', 29.0, 2, FALSE),
    (426, 12, NULL, NULL, 1, 'Archiving Mailboxes per Organization', 'Exchange2013.ArchivingMailboxes', 28.0, 2, FALSE),
    (428, 12, NULL, NULL, 1, 'Resource Mailboxes per Organization', 'Exchange2013.ResourceMailboxes', 31.0, 2, FALSE),
    (429, 12, NULL, NULL, 1, 'Shared Mailboxes per Organization', 'Exchange2013.SharedMailboxes', 30.0, 2, FALSE),
    (430, 44, NULL, NULL, 1, 'Disk Storage Space (Mb)', 'EnterpriseStorage.DiskStorageSpace', 1.0, 2, FALSE),
    (431, 44, NULL, NULL, 1, 'Number of Root Folders', 'EnterpriseStorage.Folders', 1.0, 2, FALSE),
    (447, 61, NULL, NULL, NULL, 'Enable Spam Filter', 'Filters.Enable', 1.0, 1, FALSE),
    (448, 61, NULL, NULL, NULL, 'Enable Per-Mailbox Login', 'Filters.EnableEmailUsers', 2.0, 1, FALSE),
    (450, 45, NULL, NULL, 1, 'Remote Desktop Users', 'RDS.Users', 1.0, 2, FALSE),
    (451, 45, NULL, NULL, 1, 'Remote Desktop Servers', 'RDS.Servers', 2.0, 2, FALSE),
    (452, 45, NULL, NULL, NULL, 'Disable user from adding server', 'RDS.DisableUserAddServer', 3.0, 1, FALSE),
    (453, 45, NULL, NULL, NULL, 'Disable user from removing server', 'RDS.DisableUserDeleteServer', 3.0, 1, FALSE),
    (468, 45, NULL, NULL, NULL, 'Use Drive Maps', 'EnterpriseStorage.DriveMaps', 2.0, 1, FALSE),
    (472, 46, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2014.MaxDatabaseSize', 3.0, 3, FALSE),
    (473, 46, NULL, NULL, NULL, 'Database Backups', 'MsSQL2014.Backup', 5.0, 1, FALSE),
    (474, 46, NULL, NULL, NULL, 'Database Restores', 'MsSQL2014.Restore', 6.0, 1, FALSE),
    (475, 46, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2014.Truncate', 7.0, 1, FALSE),
    (476, 46, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2014.MaxLogSize', 4.0, 3, FALSE),
    (491, 45, NULL, NULL, 1, 'Remote Desktop Servers', 'RDS.Collections', 2.0, 2, FALSE),
    (495, 13, NULL, NULL, 1, 'Deleted Users', 'HostedSolution.DeletedUsers', 6.0, 2, FALSE),
    (496, 13, NULL, NULL, 1, 'Deleted Users Backup Storage Space, Mb', 'HostedSolution.DeletedUsersBackupStorageSpace', 6.0, 2, FALSE),
    (551, 73, NULL, NULL, NULL, 'Max site storage, MB', 'HostedSharePointEnterprise.MaxStorage', 2.0, 3, FALSE),
    (552, 73, NULL, NULL, NULL, 'Use shared SSL Root', 'HostedSharePointEnterprise.UseSharedSSL', 3.0, 1, FALSE),
    (554, 33, NULL, NULL, NULL, 'Allow user to create VPS', 'VPS2012.ManagingAllowed', 2.0, 1, FALSE),
    (555, 33, NULL, NULL, NULL, 'Number of CPU cores', 'VPS2012.CpuNumber', 3.0, 2, FALSE),
    (556, 33, NULL, NULL, NULL, 'Boot from CD allowed', 'VPS2012.BootCdAllowed', 7.0, 1, FALSE),
    (557, 33, NULL, NULL, NULL, 'Boot from CD', 'VPS2012.BootCdEnabled', 8.0, 1, FALSE),
    (558, 33, NULL, NULL, NULL, 'RAM size, MB', 'VPS2012.Ram', 4.0, 2, FALSE),
    (559, 33, NULL, NULL, NULL, 'Hard Drive size, GB', 'VPS2012.Hdd', 5.0, 2, FALSE),
    (560, 33, NULL, NULL, NULL, 'DVD drive', 'VPS2012.DvdEnabled', 6.0, 1, FALSE),
    (561, 33, NULL, NULL, NULL, 'External Network', 'VPS2012.ExternalNetworkEnabled', 10.0, 1, FALSE);
    INSERT INTO `Quotas` (`QuotaID`, `GroupID`, `HideQuota`, `ItemTypeID`, `PerOrganization`, `QuotaDescription`, `QuotaName`, `QuotaOrder`, `QuotaTypeID`, `ServiceQuota`)
    VALUES (562, 33, NULL, NULL, NULL, 'Number of External IP addresses', 'VPS2012.ExternalIPAddressesNumber', 11.0, 2, FALSE),
    (563, 33, NULL, NULL, NULL, 'Private Network', 'VPS2012.PrivateNetworkEnabled', 13.0, 1, FALSE),
    (564, 33, NULL, NULL, NULL, 'Number of Private IP addresses per VPS', 'VPS2012.PrivateIPAddressesNumber', 14.0, 3, FALSE),
    (565, 33, NULL, NULL, NULL, 'Number of Snaphots', 'VPS2012.SnapshotsNumber', 9.0, 3, FALSE),
    (566, 33, NULL, NULL, NULL, 'Allow user to Start, Turn off and Shutdown VPS', 'VPS2012.StartShutdownAllowed', 15.0, 1, FALSE),
    (567, 33, NULL, NULL, NULL, 'Allow user to Pause, Resume VPS', 'VPS2012.PauseResumeAllowed', 16.0, 1, FALSE),
    (568, 33, NULL, NULL, NULL, 'Allow user to Reboot VPS', 'VPS2012.RebootAllowed', 17.0, 1, FALSE),
    (569, 33, NULL, NULL, NULL, 'Allow user to Reset VPS', 'VPS2012.ResetAlowed', 18.0, 1, FALSE),
    (570, 33, NULL, NULL, NULL, 'Allow user to Re-install VPS', 'VPS2012.ReinstallAllowed', 19.0, 1, FALSE),
    (571, 33, NULL, NULL, NULL, 'Monthly bandwidth, GB', 'VPS2012.Bandwidth', 12.0, 2, FALSE),
    (572, 33, NULL, NULL, NULL, 'Allow user to Replication', 'VPS2012.ReplicationEnabled', 20.0, 1, FALSE),
    (575, 50, NULL, NULL, NULL, 'Max Database Size', 'MariaDB.MaxDatabaseSize', 3.0, 3, FALSE),
    (576, 50, NULL, NULL, NULL, 'Database Backups', 'MariaDB.Backup', 5.0, 1, FALSE),
    (577, 50, NULL, NULL, NULL, 'Database Restores', 'MariaDB.Restore', 6.0, 1, FALSE),
    (578, 50, NULL, NULL, NULL, 'Database Truncate', 'MariaDB.Truncate', 7.0, 1, FALSE),
    (579, 50, NULL, NULL, NULL, 'Max Log Size', 'MariaDB.MaxLogSize', 4.0, 3, FALSE),
    (581, 52, NULL, NULL, NULL, 'Phone Numbers', 'SfB.PhoneNumbers', 12.0, 2, FALSE),
    (582, 52, NULL, NULL, 1, 'Users', 'SfB.Users', 1.0, 2, FALSE),
    (583, 52, NULL, NULL, NULL, 'Allow Federation', 'SfB.Federation', 2.0, 1, FALSE),
    (584, 52, NULL, NULL, NULL, 'Allow Conferencing', 'SfB.Conferencing', 3.0, 1, FALSE),
    (585, 52, NULL, NULL, NULL, 'Maximum Conference Particiapants', 'SfB.MaxParticipants', 4.0, 3, FALSE),
    (586, 52, NULL, NULL, NULL, 'Allow Video in Conference', 'SfB.AllowVideo', 5.0, 1, FALSE),
    (587, 52, NULL, NULL, NULL, 'Allow EnterpriseVoice', 'SfB.EnterpriseVoice', 6.0, 1, FALSE),
    (588, 52, NULL, NULL, NULL, 'Number of Enterprise Voice Users', 'SfB.EVUsers', 7.0, 2, FALSE),
    (589, 52, NULL, NULL, NULL, 'Allow National Calls', 'SfB.EVNational', 8.0, 1, FALSE),
    (590, 52, NULL, NULL, NULL, 'Allow Mobile Calls', 'SfB.EVMobile', 9.0, 1, FALSE),
    (591, 52, NULL, NULL, NULL, 'Allow International Calls', 'SfB.EVInternational', 10.0, 1, FALSE),
    (592, 52, NULL, NULL, NULL, 'Enable Plans Editing', 'SfB.EnablePlansEditing', 11.0, 1, FALSE),
    (674, 167, NULL, NULL, NULL, 'Allow user to create VPS', 'PROXMOX.ManagingAllowed', 2.0, 1, FALSE),
    (675, 167, NULL, NULL, NULL, 'Number of CPU cores', 'PROXMOX.CpuNumber', 3.0, 3, FALSE),
    (676, 167, NULL, NULL, NULL, 'Boot from CD allowed', 'PROXMOX.BootCdAllowed', 7.0, 1, FALSE),
    (677, 167, NULL, NULL, NULL, 'Boot from CD', 'PROXMOX.BootCdEnabled', 8.0, 1, FALSE),
    (678, 167, NULL, NULL, NULL, 'RAM size, MB', 'PROXMOX.Ram', 4.0, 2, FALSE),
    (679, 167, NULL, NULL, NULL, 'Hard Drive size, GB', 'PROXMOX.Hdd', 5.0, 2, FALSE),
    (680, 167, NULL, NULL, NULL, 'DVD drive', 'PROXMOX.DvdEnabled', 6.0, 1, FALSE),
    (681, 167, NULL, NULL, NULL, 'External Network', 'PROXMOX.ExternalNetworkEnabled', 10.0, 1, FALSE),
    (682, 167, NULL, NULL, NULL, 'Number of External IP addresses', 'PROXMOX.ExternalIPAddressesNumber', 11.0, 2, FALSE),
    (683, 167, NULL, NULL, NULL, 'Private Network', 'PROXMOX.PrivateNetworkEnabled', 13.0, 1, FALSE),
    (684, 167, NULL, NULL, NULL, 'Number of Private IP addresses per VPS', 'PROXMOX.PrivateIPAddressesNumber', 14.0, 3, FALSE),
    (685, 167, NULL, NULL, NULL, 'Number of Snaphots', 'PROXMOX.SnapshotsNumber', 9.0, 3, FALSE),
    (686, 167, NULL, NULL, NULL, 'Allow user to Start, Turn off and Shutdown VPS', 'PROXMOX.StartShutdownAllowed', 15.0, 1, FALSE),
    (687, 167, NULL, NULL, NULL, 'Allow user to Pause, Resume VPS', 'PROXMOX.PauseResumeAllowed', 16.0, 1, FALSE);
    INSERT INTO `Quotas` (`QuotaID`, `GroupID`, `HideQuota`, `ItemTypeID`, `PerOrganization`, `QuotaDescription`, `QuotaName`, `QuotaOrder`, `QuotaTypeID`, `ServiceQuota`)
    VALUES (688, 167, NULL, NULL, NULL, 'Allow user to Reboot VPS', 'PROXMOX.RebootAllowed', 17.0, 1, FALSE),
    (689, 167, NULL, NULL, NULL, 'Allow user to Reset VPS', 'PROXMOX.ResetAlowed', 18.0, 1, FALSE),
    (690, 167, NULL, NULL, NULL, 'Allow user to Re-install VPS', 'PROXMOX.ReinstallAllowed', 19.0, 1, FALSE),
    (691, 167, NULL, NULL, NULL, 'Monthly bandwidth, GB', 'PROXMOX.Bandwidth', 12.0, 2, FALSE),
    (692, 167, NULL, NULL, NULL, 'Allow user to Replication', 'PROXMOX.ReplicationEnabled', 20.0, 1, FALSE),
    (703, 71, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2016.MaxDatabaseSize', 3.0, 3, FALSE),
    (704, 71, NULL, NULL, NULL, 'Database Backups', 'MsSQL2016.Backup', 5.0, 1, FALSE),
    (705, 71, NULL, NULL, NULL, 'Database Restores', 'MsSQL2016.Restore', 6.0, 1, FALSE),
    (706, 71, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2016.Truncate', 7.0, 1, FALSE),
    (707, 71, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2016.MaxLogSize', 4.0, 3, FALSE),
    (713, 72, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2017.MaxDatabaseSize', 3.0, 3, FALSE),
    (714, 72, NULL, NULL, NULL, 'Database Backups', 'MsSQL2017.Backup', 5.0, 1, FALSE),
    (715, 72, NULL, NULL, NULL, 'Database Restores', 'MsSQL2017.Restore', 6.0, 1, FALSE),
    (716, 72, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2017.Truncate', 7.0, 1, FALSE),
    (717, 72, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2017.MaxLogSize', 4.0, 3, FALSE),
    (723, 74, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2019.MaxDatabaseSize', 3.0, 3, FALSE),
    (724, 74, NULL, NULL, NULL, 'Database Backups', 'MsSQL2019.Backup', 5.0, 1, FALSE),
    (725, 74, NULL, NULL, NULL, 'Database Restores', 'MsSQL2019.Restore', 6.0, 1, FALSE),
    (726, 74, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2019.Truncate', 7.0, 1, FALSE),
    (727, 74, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2019.MaxLogSize', 4.0, 3, FALSE),
    (728, 33, NULL, NULL, NULL, 'Number of Private Network VLANs', 'VPS2012.PrivateVLANsNumber', 14.0, 2, FALSE),
    (729, 12, NULL, NULL, NULL, 'Automatic Replies via FuseCP Allowed', 'Exchange2013.AutoReply', 32.0, 1, FALSE),
    (730, 33, NULL, NULL, NULL, 'Additional Hard Drives per VPS', 'VPS2012.AdditionalVhdCount', 6.0, 3, FALSE),
    (731, 12, NULL, NULL, 1, 'Journaling Mailboxes per Organization', 'Exchange2013.JournalingMailboxes', 31.0, 2, FALSE),
    (734, 75, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2022.MaxDatabaseSize', 3.0, 3, FALSE),
    (735, 75, NULL, NULL, NULL, 'Database Backups', 'MsSQL2022.Backup', 5.0, 1, FALSE),
    (736, 75, NULL, NULL, NULL, 'Database Restores', 'MsSQL2022.Restore', 6.0, 1, FALSE),
    (737, 75, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2022.Truncate', 7.0, 1, FALSE),
    (738, 75, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2022.MaxLogSize', 4.0, 3, FALSE),
    (750, 33, NULL, NULL, NULL, 'DMZ Network', 'VPS2012.DMZNetworkEnabled', 22.0, 1, FALSE),
    (751, 33, NULL, NULL, NULL, 'Number of DMZ IP addresses per VPS', 'VPS2012.DMZIPAddressesNumber', 23.0, 3, FALSE),
    (752, 33, NULL, NULL, NULL, 'Number of DMZ Network VLANs', 'VPS2012.DMZVLANsNumber', 24.0, 2, FALSE),
    (753, 7, NULL, NULL, NULL, 'Allow editing TTL in DNS Editor', 'DNS.EditTTL', 2.0, 1, FALSE),
    (754, 4, TRUE, NULL, NULL, 'Allow changes to access controls', 'Mail.AllowAccessControls', 9.0, 1, FALSE),
    (762, 76, NULL, NULL, NULL, 'Max Database Size', 'MsSQL2025.MaxDatabaseSize', 3.0, 3, FALSE),
    (763, 76, NULL, NULL, NULL, 'Database Backups', 'MsSQL2025.Backup', 5.0, 1, FALSE),
    (764, 76, NULL, NULL, NULL, 'Database Restores', 'MsSQL2025.Restore', 6.0, 1, FALSE),
    (765, 76, NULL, NULL, NULL, 'Database Truncate', 'MsSQL2025.Truncate', 7.0, 1, FALSE),
    (766, 76, NULL, NULL, NULL, 'Max Log Size', 'MsSQL2025.MaxLogSize', 4.0, 3, FALSE),
    (771, 4, NULL, NULL, NULL, 'Mail Accounts per Domain', 'Mail.Accounts.per.Domain', 1.2, 2, TRUE);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ResourceGroupDnsRecords` (`RecordID`, `GroupID`, `MXPriority`, `RecordData`, `RecordName`, `RecordOrder`, `RecordType`)
    VALUES (1, 2, 0, '[IP]', '', 1, 'A'),
    (2, 2, 0, '[IP]', '*', 2, 'A'),
    (3, 2, 0, '[IP]', 'www', 3, 'A'),
    (4, 3, 0, '[IP]', 'ftp', 1, 'A'),
    (5, 4, 0, '[IP]', 'mail', 1, 'A'),
    (6, 4, 0, '[IP]', 'mail2', 2, 'A'),
    (7, 4, 10, 'mail.[DOMAIN_NAME]', '', 3, 'MX'),
    (9, 4, 21, 'mail2.[DOMAIN_NAME]', '', 4, 'MX'),
    (10, 5, 0, '[IP]', 'mssql', 1, 'A'),
    (11, 6, 0, '[IP]', 'mysql', 1, 'A'),
    (12, 8, 0, '[IP]', 'stats', 1, 'A'),
    (13, 4, 0, 'v=spf1 a mx -all', '', 5, 'TXT'),
    (14, 12, 0, '[IP]', 'smtp', 1, 'A'),
    (15, 12, 10, 'smtp.[DOMAIN_NAME]', '', 2, 'MX'),
    (16, 12, 0, '', 'autodiscover', 3, 'CNAME'),
    (17, 12, 0, '', 'owa', 4, 'CNAME');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ScheduleTaskParameters` (`ParameterID`, `TaskID`, `DataTypeID`, `DefaultValue`, `ParameterOrder`)
    VALUES ('AUDIT_LOG_DATE', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'List', 'today=Today;yesterday=Yesterday;schedule=Schedule', 5),
    ('AUDIT_LOG_SEVERITY', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'List', '-1=All;0=Information;1=Warning;2=Error', 2),
    ('AUDIT_LOG_SOURCE', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'List', '', 3),
    ('AUDIT_LOG_TASK', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'List', '', 4),
    ('MAIL_TO', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'String', NULL, 1),
    ('SHOW_EXECUTION_LOG', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', 'List', '0=No;1=Yes', 6),
    ('BACKUP_FILE_NAME', 'SCHEDULE_TASK_BACKUP', 'String', '', 1),
    ('DELETE_TEMP_BACKUP', 'SCHEDULE_TASK_BACKUP', 'Boolean', 'true', 1),
    ('STORE_PACKAGE_FOLDER', 'SCHEDULE_TASK_BACKUP', 'String', '\\', 1),
    ('STORE_PACKAGE_ID', 'SCHEDULE_TASK_BACKUP', 'String', '', 1),
    ('STORE_SERVER_FOLDER', 'SCHEDULE_TASK_BACKUP', 'String', '', 1),
    ('BACKUP_FOLDER', 'SCHEDULE_TASK_BACKUP_DATABASE', 'String', '\\backups', 3),
    ('BACKUP_NAME', 'SCHEDULE_TASK_BACKUP_DATABASE', 'String', 'database_backup.bak', 4),
    ('DATABASE_GROUP', 'SCHEDULE_TASK_BACKUP_DATABASE', 'List', 'MsSQL2014=SQL Server 2014;MsSQL2016=SQL Server 2016;MsSQL2017=SQL Server 2017;MsSQL2019=SQL Server 2019;MsSQL2022=SQL Server 2022;MySQL5=MySQL 5.0;MariaDB=MariaDB', 1),
    ('DATABASE_NAME', 'SCHEDULE_TASK_BACKUP_DATABASE', 'String', '', 2),
    ('ZIP_BACKUP', 'SCHEDULE_TASK_BACKUP_DATABASE', 'List', 'true=Yes;false=No', 5),
    ('MAIL_BODY', 'SCHEDULE_TASK_CHECK_WEBSITE', 'MultiString', '', 10),
    ('MAIL_FROM', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', 'admin@mysite.com', 7),
    ('MAIL_SUBJECT', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', 'Web Site is unavailable', 9),
    ('MAIL_TO', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', 'admin@mysite.com', 8),
    ('PASSWORD', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', NULL, 3),
    ('RESPONSE_CONTAIN', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', NULL, 5),
    ('RESPONSE_DOESNT_CONTAIN', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', NULL, 6),
    ('RESPONSE_STATUS', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', '500', 4),
    ('URL', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', 'http://', 1),
    ('USE_RESPONSE_CONTAIN', 'SCHEDULE_TASK_CHECK_WEBSITE', 'Boolean', 'false', 1),
    ('USE_RESPONSE_DOESNT_CONTAIN', 'SCHEDULE_TASK_CHECK_WEBSITE', 'Boolean', 'false', 1),
    ('USE_RESPONSE_STATUS', 'SCHEDULE_TASK_CHECK_WEBSITE', 'Boolean', 'false', 1),
    ('USERNAME', 'SCHEDULE_TASK_CHECK_WEBSITE', 'String', NULL, 2),
    ('BCC_MAIL', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'String', 'admin@mydomain.com', 3),
    ('ERROR_MAIL_BODY', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'MultiString', 'Hello, <br>we cannot verify the SSL certificate for the domain [domain]. <br><br>Error message: [error] <br><br>Please check if the website is available.', 11),
    ('ERROR_MAIL_SUBJECT', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'String', 'Certificate error or website is unavailable', 10),
    ('EXPIRATION_MAIL_BODY', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'MultiString', 'Hello, <br>Your certificate for the [domain] will expire in [expires_in_days] days (on [expires_on_date]).', 5),
    ('EXPIRATION_MAIL_SUBJECT', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'String', 'Website certificate expiration notice', 4),
    ('SEND_14_DAYS_BEFORE_EXPIRATION', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'true', 7),
    ('SEND_30_DAYS_BEFORE_EXPIRATION', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'true', 6),
    ('SEND_BCC', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'false', 2),
    ('SEND_MAIL_TO_CUSTOMER', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'true', 1),
    ('SEND_SSL_ERROR', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'false', 9),
    ('SEND_TODAY_EXPIRED', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', 'Boolean', 'true', 8),
    ('DAYS_BEFORE', 'SCHEDULE_TASK_DOMAIN_EXPIRATION', 'String', NULL, 1),
    ('ENABLE_NOTIFICATION', 'SCHEDULE_TASK_DOMAIN_EXPIRATION', 'Boolean', 'false', 3);
    INSERT INTO `ScheduleTaskParameters` (`ParameterID`, `TaskID`, `DataTypeID`, `DefaultValue`, `ParameterOrder`)
    VALUES ('INCLUDE_NONEXISTEN_DOMAINS', 'SCHEDULE_TASK_DOMAIN_EXPIRATION', 'Boolean', 'false', 4),
    ('MAIL_TO', 'SCHEDULE_TASK_DOMAIN_EXPIRATION', 'String', NULL, 2),
    ('DNS_SERVERS', 'SCHEDULE_TASK_DOMAIN_LOOKUP', 'String', NULL, 1),
    ('MAIL_TO', 'SCHEDULE_TASK_DOMAIN_LOOKUP', 'String', NULL, 2),
    ('PAUSE_BETWEEN_QUERIES', 'SCHEDULE_TASK_DOMAIN_LOOKUP', 'String', '100', 4),
    ('SERVER_NAME', 'SCHEDULE_TASK_DOMAIN_LOOKUP', 'String', '', 3),
    ('FILE_PATH', 'SCHEDULE_TASK_FTP_FILES', 'String', '\\', 1),
    ('FTP_FOLDER', 'SCHEDULE_TASK_FTP_FILES', 'String', NULL, 5),
    ('FTP_PASSWORD', 'SCHEDULE_TASK_FTP_FILES', 'String', NULL, 4),
    ('FTP_SERVER', 'SCHEDULE_TASK_FTP_FILES', 'String', 'ftp.myserver.com', 2),
    ('FTP_USERNAME', 'SCHEDULE_TASK_FTP_FILES', 'String', NULL, 3),
    ('EMAIL', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'String', NULL, 1),
    ('EXCHANGE_REPORT', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'Boolean', 'true', 2),
    ('LYNC_REPORT', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'Boolean', 'true', 4),
    ('ORGANIZATION_REPORT', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'Boolean', 'true', 7),
    ('SFB_REPORT', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'Boolean', 'true', 5),
    ('SHAREPOINT_REPORT', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', 'Boolean', 'true', 3),
    ('MARIADB_OVERUSED', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'Boolean', 'true', 1),
    ('MSSQL_OVERUSED', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'Boolean', 'true', 1),
    ('MYSQL_OVERUSED', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'Boolean', 'true', 1),
    ('OVERUSED_MAIL_BCC', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1),
    ('OVERUSED_MAIL_BODY', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1),
    ('OVERUSED_MAIL_FROM', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1),
    ('OVERUSED_MAIL_SUBJECT', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1),
    ('OVERUSED_USAGE_THRESHOLD', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '100', 1),
    ('SEND_OVERUSED_EMAIL', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'Boolean', 'true', 1),
    ('SEND_WARNING_EMAIL', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'Boolean', 'true', 1),
    ('WARNING_MAIL_BCC', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1),
    ('WARNING_MAIL_BODY', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1),
    ('WARNING_MAIL_FROM', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1),
    ('WARNING_MAIL_SUBJECT', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '', 1),
    ('WARNING_USAGE_THRESHOLD', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', 'String', '80', 1),
    ('EXECUTABLE_PARAMS', 'SCHEDULE_TASK_RUN_SYSTEM_COMMAND', 'String', '', 3),
    ('EXECUTABLE_PATH', 'SCHEDULE_TASK_RUN_SYSTEM_COMMAND', 'String', 'Executable.exe', 2),
    ('SERVER_NAME', 'SCHEDULE_TASK_RUN_SYSTEM_COMMAND', 'String', NULL, 1),
    ('MAIL_BODY', 'SCHEDULE_TASK_SEND_MAIL', 'MultiString', NULL, 4),
    ('MAIL_FROM', 'SCHEDULE_TASK_SEND_MAIL', 'String', NULL, 1),
    ('MAIL_SUBJECT', 'SCHEDULE_TASK_SEND_MAIL', 'String', NULL, 3),
    ('MAIL_TO', 'SCHEDULE_TASK_SEND_MAIL', 'String', NULL, 2),
    ('BANDWIDTH_OVERUSED', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'Boolean', 'true', 1),
    ('DISKSPACE_OVERUSED', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'Boolean', 'true', 1);
    INSERT INTO `ScheduleTaskParameters` (`ParameterID`, `TaskID`, `DataTypeID`, `DefaultValue`, `ParameterOrder`)
    VALUES ('SEND_SUSPENSION_EMAIL', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'Boolean', 'true', 1),
    ('SEND_WARNING_EMAIL', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'Boolean', 'true', 1),
    ('SUSPEND_OVERUSED', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'Boolean', 'true', 1),
    ('SUSPENSION_MAIL_BCC', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1),
    ('SUSPENSION_MAIL_BODY', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1),
    ('SUSPENSION_MAIL_FROM', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1),
    ('SUSPENSION_MAIL_SUBJECT', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1),
    ('SUSPENSION_USAGE_THRESHOLD', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', '100', 1),
    ('WARNING_MAIL_BCC', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1),
    ('WARNING_MAIL_BODY', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1),
    ('WARNING_MAIL_FROM', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1),
    ('WARNING_MAIL_SUBJECT', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', NULL, 1),
    ('WARNING_USAGE_THRESHOLD', 'SCHEDULE_TASK_SUSPEND_PACKAGES', 'String', '80', 1),
    ('DAYS_BEFORE_EXPIRATION', 'SCHEDULE_TASK_USER_PASSWORD_EXPIRATION_NOTIFICATION', 'String', NULL, 1),
    ('FOLDER', 'SCHEDULE_TASK_ZIP_FILES', 'String', NULL, 1),
    ('ZIP_FILE', 'SCHEDULE_TASK_ZIP_FILES', 'String', '\\archive.zip', 2);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ScheduleTaskViewConfiguration` (`ConfigurationID`, `TaskID`, `Description`, `Environment`)
    VALUES ('ASP_NET', 'SCHEDULE_TASK_ACTIVATE_PAID_INVOICES', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_AUDIT_LOG_REPORT', '~/DesktopModules/FuseCP/ScheduleTaskControls/AuditLogReportView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_BACKUP', '~/DesktopModules/FuseCP/ScheduleTaskControls/Backup.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_BACKUP_DATABASE', '~/DesktopModules/FuseCP/ScheduleTaskControls/BackupDatabase.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_CALCULATE_EXCHANGE_DISKSPACE', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_CANCEL_OVERDUE_INVOICES', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_CHECK_WEBSITE', '~/DesktopModules/FuseCP/ScheduleTaskControls/CheckWebsite.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_CHECK_WEBSITES_SSL', '~/DesktopModules/FuseCP/ScheduleTaskControls/CheckWebsitesSslView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_DOMAIN_EXPIRATION', '~/DesktopModules/FuseCP/ScheduleTaskControls/DomainExpirationView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_DOMAIN_LOOKUP', '~/DesktopModules/FuseCP/ScheduleTaskControls/DomainLookupView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_FTP_FILES', '~/DesktopModules/FuseCP/ScheduleTaskControls/SendFilesViaFtp.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_GENERATE_INVOICES', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_HOSTED_SOLUTION_REPORT', '~/DesktopModules/FuseCP/ScheduleTaskControls/HostedSolutionReport.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES', '~/DesktopModules/FuseCP/ScheduleTaskControls/NotifyOverusedDatabases.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_RUN_PAYMENT_QUEUE', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_RUN_SYSTEM_COMMAND', '~/DesktopModules/FuseCP/ScheduleTaskControls/ExecuteSystemCommand.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_SEND_MAIL', '~/DesktopModules/FuseCP/ScheduleTaskControls/SendEmailNotification.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_SUSPEND_OVERDUE_INVOICES', '~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_SUSPEND_PACKAGES', '~/DesktopModules/FuseCP/ScheduleTaskControls/SuspendOverusedSpaces.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_USER_PASSWORD_EXPIRATION_NOTIFICATION', '~/DesktopModules/FuseCP/ScheduleTaskControls/UserPasswordExpirationNotificationView.ascx', 'ASP.NET'),
    ('ASP_NET', 'SCHEDULE_TASK_ZIP_FILES', '~/DesktopModules/FuseCP/ScheduleTaskControls/ZipFiles.ascx', 'ASP.NET');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (2, TRUE, FALSE, TRUE, 'HomeFolder', TRUE, 1, FALSE, FALSE, 'FuseCP.Providers.OS.HomeFolder, FuseCP.Providers.Base', 15);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Importable`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (5, TRUE, FALSE, TRUE, 'MsSQL2000Database', TRUE, 5, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 9),
    (6, TRUE, FALSE, FALSE, 'MsSQL2000User', TRUE, 5, TRUE, TRUE, TRUE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 10),
    (7, TRUE, FALSE, TRUE, 'MySQL4Database', TRUE, 6, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 13),
    (8, TRUE, FALSE, FALSE, 'MySQL4User', TRUE, 6, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 14),
    (9, TRUE, TRUE, FALSE, 'FTPAccount', TRUE, 3, TRUE, TRUE, TRUE, 'FuseCP.Providers.FTP.FtpAccount, FuseCP.Providers.Base', 3),
    (10, TRUE, TRUE, TRUE, 'WebSite', TRUE, 2, TRUE, TRUE, TRUE, 'FuseCP.Providers.Web.WebSite, FuseCP.Providers.Base', 2),
    (11, TRUE, TRUE, FALSE, 'MailDomain', TRUE, 4, TRUE, TRUE, TRUE, 'FuseCP.Providers.Mail.MailDomain, FuseCP.Providers.Base', 8);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Importable`, `Searchable`, `Suspendable`, `TypeName`)
    VALUES (12, TRUE, FALSE, FALSE, 'DNSZone', TRUE, 7, TRUE, FALSE, TRUE, 'FuseCP.Providers.DNS.DnsZone, FuseCP.Providers.Base');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (13, FALSE, FALSE, 'Domain', FALSE, 1, TRUE, FALSE, 'FuseCP.Providers.OS.Domain, FuseCP.Providers.Base', 1);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Importable`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (14, TRUE, FALSE, FALSE, 'StatisticsSite', TRUE, 8, TRUE, TRUE, FALSE, 'FuseCP.Providers.Statistics.StatsSite, FuseCP.Providers.Base', 17);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (15, FALSE, TRUE, 'MailAccount', FALSE, 4, TRUE, FALSE, 'FuseCP.Providers.Mail.MailAccount, FuseCP.Providers.Base', 4),
    (16, FALSE, FALSE, 'MailAlias', FALSE, 4, TRUE, FALSE, 'FuseCP.Providers.Mail.MailAlias, FuseCP.Providers.Base', 5),
    (17, FALSE, FALSE, 'MailList', FALSE, 4, TRUE, FALSE, 'FuseCP.Providers.Mail.MailList, FuseCP.Providers.Base', 7),
    (18, FALSE, FALSE, 'MailGroup', FALSE, 4, TRUE, FALSE, 'FuseCP.Providers.Mail.MailGroup, FuseCP.Providers.Base', 6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Importable`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (20, TRUE, FALSE, FALSE, 'ODBCDSN', TRUE, 1, TRUE, TRUE, FALSE, 'FuseCP.Providers.OS.SystemDSN, FuseCP.Providers.Base', 22),
    (21, TRUE, FALSE, TRUE, 'MsSQL2005Database', TRUE, 10, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 11),
    (22, TRUE, FALSE, FALSE, 'MsSQL2005User', TRUE, 10, TRUE, TRUE, TRUE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 12),
    (23, TRUE, FALSE, TRUE, 'MySQL5Database', TRUE, 11, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 15),
    (24, TRUE, FALSE, FALSE, 'MySQL5User', TRUE, 11, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 16);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (25, TRUE, FALSE, FALSE, 'SharedSSLFolder', TRUE, 2, TRUE, FALSE, 'FuseCP.Providers.Web.SharedSSLFolder, FuseCP.Providers.Base', 21);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Searchable`, `Suspendable`, `TypeName`)
    VALUES (28, TRUE, FALSE, FALSE, 'SecondaryDNSZone', TRUE, 7, FALSE, TRUE, 'FuseCP.Providers.DNS.SecondaryDnsZone, FuseCP.Providers.Base');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (29, TRUE, FALSE, TRUE, 'Organization', TRUE, 13, TRUE, TRUE, 'FuseCP.Providers.HostedSolution.Organization, FuseCP.Providers.Base', 1),
    (30, TRUE, NULL, NULL, 'OrganizationDomain', NULL, 13, NULL, NULL, 'FuseCP.Providers.HostedSolution.OrganizationDomain, FuseCP.Providers.Base', 1);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Importable`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (31, TRUE, FALSE, TRUE, 'MsSQL2008Database', TRUE, 22, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1),
    (32, TRUE, FALSE, FALSE, 'MsSQL2008User', TRUE, 22, TRUE, TRUE, TRUE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (33, FALSE, FALSE, 'VirtualMachine', TRUE, 30, TRUE, TRUE, 'FuseCP.Providers.Virtualization.VirtualMachine, FuseCP.Providers.Base', 1),
    (34, FALSE, FALSE, 'VirtualSwitch', TRUE, 30, TRUE, TRUE, 'FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base', 2),
    (35, FALSE, FALSE, 'VMInfo', TRUE, 40, TRUE, TRUE, 'FuseCP.Providers.Virtualization.VMInfo, FuseCP.Providers.Base', 1),
    (36, FALSE, FALSE, 'VirtualSwitch', TRUE, 40, TRUE, TRUE, 'FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base', 2);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Importable`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (37, TRUE, FALSE, TRUE, 'MsSQL2012Database', TRUE, 23, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1),
    (38, TRUE, FALSE, FALSE, 'MsSQL2012User', TRUE, 23, TRUE, TRUE, TRUE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1),
    (39, TRUE, FALSE, TRUE, 'MsSQL2014Database', TRUE, 46, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1),
    (40, TRUE, FALSE, FALSE, 'MsSQL2014User', TRUE, 46, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (41, FALSE, FALSE, 'VirtualMachine', TRUE, 33, TRUE, TRUE, 'FuseCP.Providers.Virtualization.VirtualMachine, FuseCP.Providers.Base', 1),
    (42, FALSE, FALSE, 'VirtualSwitch', TRUE, 33, TRUE, TRUE, 'FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base', 2);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Importable`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (71, TRUE, FALSE, TRUE, 'MsSQL2016Database', TRUE, 71, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1),
    (72, TRUE, FALSE, FALSE, 'MsSQL2016User', TRUE, 71, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1),
    (73, TRUE, FALSE, TRUE, 'MsSQL2017Database', TRUE, 72, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1),
    (74, TRUE, FALSE, FALSE, 'MsSQL2017User', TRUE, 72, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1),
    (75, TRUE, FALSE, TRUE, 'MySQL8Database', TRUE, 90, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 18),
    (76, TRUE, FALSE, FALSE, 'MySQL8User', TRUE, 90, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 19),
    (77, TRUE, FALSE, TRUE, 'MsSQL2019Database', TRUE, 74, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1),
    (78, TRUE, FALSE, FALSE, 'MsSQL2019User', TRUE, 74, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1),
    (79, TRUE, FALSE, TRUE, 'MsSQL2022Database', TRUE, 75, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1),
    (80, TRUE, FALSE, FALSE, 'MsSQL2022User', TRUE, 75, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1),
    (90, TRUE, FALSE, TRUE, 'MySQL9Database', TRUE, 91, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 20),
    (91, TRUE, FALSE, FALSE, 'MySQL9User', TRUE, 91, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 21);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (143, FALSE, FALSE, 'VirtualMachine', TRUE, 167, TRUE, TRUE, 'FuseCP.Providers.Virtualization.VirtualMachine, FuseCP.Providers.Base', 1),
    (144, FALSE, FALSE, 'VirtualSwitch', TRUE, 167, TRUE, TRUE, 'FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base', 2);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceItemTypes` (`ItemTypeID`, `Backupable`, `CalculateBandwidth`, `CalculateDiskspace`, `DisplayName`, `Disposable`, `GroupID`, `Importable`, `Searchable`, `Suspendable`, `TypeName`, `TypeOrder`)
    VALUES (200, TRUE, FALSE, TRUE, 'SharePointFoundationSiteCollection', TRUE, 20, TRUE, TRUE, FALSE, 'FuseCP.Providers.SharePoint.SharePointSiteCollection, FuseCP.Providers.Base', 25),
    (202, TRUE, FALSE, TRUE, 'MariaDBDatabase', TRUE, 50, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1),
    (203, TRUE, FALSE, FALSE, 'MariaDBUser', TRUE, 50, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1),
    (204, TRUE, FALSE, TRUE, 'SharePointEnterpriseSiteCollection', TRUE, 73, TRUE, TRUE, FALSE, 'FuseCP.Providers.SharePoint.SharePointEnterpriseSiteCollection, FuseCP.Providers.Base', 100),
    (205, TRUE, FALSE, TRUE, 'MsSQL2025Database', TRUE, 76, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base', 1),
    (206, TRUE, FALSE, FALSE, 'MsSQL2025User', TRUE, 76, TRUE, TRUE, FALSE, 'FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base', 1);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `UserSettings` (`PropertyName`, `SettingsName`, `UserID`, `PropertyValue`)
    VALUES ('CC', 'AccountSummaryLetter', 1, 'support@HostingCompany.com'),
    ('EnableLetter', 'AccountSummaryLetter', 1, 'False'),
    ('From', 'AccountSummaryLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'AccountSummaryLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>Account Summary Information</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; }', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '', CHAR(13, 10), '<a name="top"></a>', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '	Hosting Account Information', CHAR(13, 10), '</div>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#Signup#">', CHAR(13, 10), '<p>', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'New user account has been created and below you can find its summary information.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<h1>Control Panel URL</h1>', CHAR(13, 10), '<table>', CHAR(13, 10), '    <thead>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <th>Control Panel URL</th>', CHAR(13, 10), '            <th>Username</th>', CHAR(13, 10), '            <th>Password</th>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '    </thead>', CHAR(13, 10), '    <tbody>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td><a href="http://panel.HostingCompany.com">http://panel.HostingCompany.com</a></td>', CHAR(13, 10), '            <td>#user.Username#</td>', CHAR(13, 10), '            <td>#user.Password#</td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '    </tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '<h1>Hosting Spaces</h1>', CHAR(13, 10), '<p>', CHAR(13, 10), '    The following hosting spaces have been created under your account:', CHAR(13, 10), '</p>', CHAR(13, 10), '<ad:foreach collection="#Spaces#" var="Space" index="i">', CHAR(13, 10), '<h2>#Space.PackageName#</h2>', CHAR(13, 10), '<table>', CHAR(13, 10), '	<tbody>', CHAR(13, 10), '		<tr>', CHAR(13, 10), '			<td class="Label">Hosting Plan:</td>', CHAR(13, 10), '			<td>', CHAR(13, 10), '				<ad:if test="#not(isnull(Plans[Space.PlanId]))#">#Plans[Space.PlanId].PlanName#<ad:else>System</ad:if>', CHAR(13, 10), '			</td>', CHAR(13, 10), '		</tr>', CHAR(13, 10), '		<ad:if test="#not(isnull(Plans[Space.PlanId]))#">', CHAR(13, 10), '		<tr>', CHAR(13, 10), '			<td class="Label">Purchase Date:</td>', CHAR(13, 10), '			<td>', CHAR(13, 10), '# Space.PurchaseDate#', CHAR(13, 10), '			</td>', CHAR(13, 10), '		</tr>', CHAR(13, 10), '		<tr>', CHAR(13, 10), '			<td class="Label">Disk Space, MB:</td>', CHAR(13, 10), '			<td><ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Diskspace" /></td>', CHAR(13, 10), '		</tr>', CHAR(13, 10), '		<tr>', CHAR(13, 10), '			<td class="Label">Bandwidth, MB/Month:</td>', CHAR(13, 10), '			<td><ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Bandwidth" /></td>', CHAR(13, 10), '		</tr>', CHAR(13, 10), '		<tr>', CHAR(13, 10), '			<td class="Label">Maximum Number of Domains:</td>', CHAR(13, 10), '			<td><ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Domains" /></td>', CHAR(13, 10), '		</tr>', CHAR(13, 10), '		<tr>', CHAR(13, 10), '			<td class="Label">Maximum Number of Sub-Domains:</td>', CHAR(13, 10), '			<td><ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.SubDomains" /></td>', CHAR(13, 10), '		</tr>', CHAR(13, 10), '		</ad:if>', CHAR(13, 10), '	</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</ad:foreach>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#Signup#">', CHAR(13, 10), '<p>', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Best regards,<br />', CHAR(13, 10), 'FuseCP.<br />', CHAR(13, 10), 'Web Site: <a href="https://fusecp.com">https://fusecp.com</a><br />', CHAR(13, 10), 'E-Mail: <a href="mailto:support@fusecp.com">support@fusecp.com</a>', CHAR(13, 10), '</p>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '<ad:template name="NumericQuota">', CHAR(13, 10), '	<ad:if test="#space.Quotas.ContainsKey(quota)#">', CHAR(13, 10), '		<ad:if test="#space.Quotas[quota].QuotaAllocatedValue isnot -1#">#space.Quotas[quota].QuotaAllocatedValue#<ad:else>Unlimited</ad:if>', CHAR(13, 10), '	<ad:else>', CHAR(13, 10), '		0', CHAR(13, 10), '	</ad:if>', CHAR(13, 10), '</ad:template>', CHAR(13, 10), '', CHAR(13, 10), '</div>', CHAR(13, 10), '</body>', CHAR(13, 10), '</html>')),
    ('Priority', 'AccountSummaryLetter', 1, 'Normal'),
    ('Subject', 'AccountSummaryLetter', 1, '<ad:if test="#Signup#">FuseCP  account has been created for<ad:else>FuseCP  account summary for</ad:if> #user.FirstName# #user.LastName#'),
    ('TextBody', 'AccountSummaryLetter', 1, CONCAT('=================================', CHAR(13, 10), '   Hosting Account Information', CHAR(13, 10), '=================================', CHAR(13, 10), '<ad:if test="#Signup#">Hello #user.FirstName#,', CHAR(13, 10), '', CHAR(13, 10), 'New user account has been created and below you can find its summary information.', CHAR(13, 10), '', CHAR(13, 10), 'Control Panel URL: https://panel.fusecp.com', CHAR(13, 10), 'Username: #user.Username#', CHAR(13, 10), 'Password: #user.Password#', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'Hosting Spaces', CHAR(13, 10), '==============', CHAR(13, 10), 'The following hosting spaces have been created under your account:', CHAR(13, 10), '', CHAR(13, 10), '<ad:foreach collection="#Spaces#" var="Space" index="i">', CHAR(13, 10), '=== #Space.PackageName# ===', CHAR(13, 10), 'Hosting Plan: <ad:if test="#not(isnull(Plans[Space.PlanId]))#">#Plans[Space.PlanId].PlanName#<ad:else>System</ad:if>', CHAR(13, 10), '<ad:if test="#not(isnull(Plans[Space.PlanId]))#">Purchase Date: #Space.PurchaseDate#', CHAR(13, 10), 'Disk Space, MB: <ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Diskspace" />', CHAR(13, 10), 'Bandwidth, MB/Month: <ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Bandwidth" />', CHAR(13, 10), 'Maximum Number of Domains: <ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.Domains" />', CHAR(13, 10), 'Maximum Number of Sub-Domains: <ad:NumericQuota space="#SpaceContexts[Space.PackageId]#" quota="OS.SubDomains" />', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '</ad:foreach>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#Signup#">If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '', CHAR(13, 10), 'Best regards,', CHAR(13, 10), 'FuseCP.', CHAR(13, 10), 'Web Site: https://fusecp.com">', CHAR(13, 10), 'E-Mail: support@fusecp.com', CHAR(13, 10), '</ad:if><ad:template name="NumericQuota"><ad:if test="#space.Quotas.ContainsKey(quota)#"><ad:if test="#space.Quotas[quota].QuotaAllocatedValue isnot -1#">#space.Quotas[quota].QuotaAllocatedValue#<ad:else>Unlimited</ad:if><ad:else>0</ad:if></ad:template>')),
    ('Transform', 'BandwidthXLST', 1, CONCAT('<?xml version="1.0" encoding="UTF-8"?>', CHAR(13, 10), '<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">', CHAR(13, 10), '<xsl:template match="/">', CHAR(13, 10), '  <html>', CHAR(13, 10), '  <body>', CHAR(13, 10), '  <img alt="Embedded Image" width="299" height="60" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASsAAAA8CAYAAAA+AJwjAAAACXBIWXMAAAsTAAALEwEAmpwYAAA4G2lUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4KPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS41LWMwMTQgNzkuMTUxNDgxLCAyMDEzLzAzLzEzLTEyOjA5OjE1ICAgICAgICAiPgogICA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIKICAgICAgICAgICAgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIgogICAgICAgICAgICB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIKICAgICAgICAgICAgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIj4KICAgICAgICAgPHhtcDpDcmVhdG9yVG9vbD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC94bXA6Q3JlYXRvclRvb2w+CiAgICAgICAgIDx4bXA6Q3JlYXRlRGF0ZT4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC94bXA6Q3JlYXRlRGF0ZT4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTYtMDMtMDFUMTQ6NTE6NTgrMDE6MDA8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOk1ldGFkYXRhRGF0ZT4yMDE2LTAzLTAxVDE0OjUxOjU4KzAxOjAwPC94bXA6TWV0YWRhdGFEYXRlPgogICAgICAgICA8ZGM6Zm9ybWF0PmltYWdlL3BuZzwvZGM6Zm9ybWF0PgogICAgICAgICA8cGhvdG9zaG9wOkNvbG9yTW9kZT4zPC9waG90b3Nob3A6Q29sb3JNb2RlPgogICAgICAgICA8eG1wTU06SW5zdGFuY2VJRD54bXAuaWlkOjZhNTdmMWYyLTgyZjYtMjk0MS1hYjFmLTNkOWQ0YjdmMTY2YjwveG1wTU06SW5zdGFuY2VJRD4KICAgICAgICAgPHhtcE1NOkRvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOkRvY3VtZW50SUQ+CiAgICAgICAgIDx4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD4KICAgICAgICAgPHhtcE1NOkhpc3Rvcnk+CiAgICAgICAgICAgIDxyZGY6U2VxPgogICAgICAgICAgICAgICA8cmRmOmxpIHJkZjpwYXJzZVR5cGU9IlJlc291cmNlIj4KICAgICAgICAgICAgICAgICAgPHN0RXZ0OmFjdGlvbj5jcmVhdGVkPC9zdEV2dDphY3Rpb24+CiAgICAgICAgICAgICAgICAgIDxzdEV2dDppbnN0YW5jZUlEPnhtcC5paWQ6NmE1N2YxZjItODJmNi0yOTQxLWFiMWYtM2Q5ZDRiN2YxNjZiPC9zdEV2dDppbnN0YW5jZUlEPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6d2hlbj4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC9zdEV2dDp3aGVuPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6c29mdHdhcmVBZ2VudD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC9zdEV2dDpzb2Z0d2FyZUFnZW50PgogICAgICAgICAgICAgICA8L3JkZjpsaT4KICAgICAgICAgICAgPC9yZGY6U2VxPgogICAgICAgICA8L3htcE1NOkhpc3Rvcnk+CiAgICAgICAgIDx0aWZmOk9yaWVudGF0aW9uPjE8L3RpZmY6T3JpZW50YXRpb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyMDAwMC8xMDAwMDwvdGlmZjpYUmVzb2x1dGlvbj4KICAgICAgICAgPHRpZmY6WVJlc29sdXRpb24+NzIwMDAwLzEwMDAwPC90aWZmOllSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpSZXNvbHV0aW9uVW5pdD4yPC90aWZmOlJlc29sdXRpb25Vbml0PgogICAgICAgICA8ZXhpZjpDb2xvclNwYWNlPjY1NTM1PC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWERpbWVuc2lvbj4yOTk8L2V4aWY6UGl4ZWxYRGltZW5zaW9uPgogICAgICAgICA8ZXhpZjpQaXhlbFlEaW1lbnNpb24+NjA8L2V4aWY6UGl4ZWxZRGltZW5zaW9uPgogICAgICA8L3JkZjpEZXNjcmlwdGlvbj4KICAgPC9yZGY6UkRGPgo8L3g6eG1wbWV0YT4KICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAg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" />', CHAR(13, 10), '  <h2>Bandwidth Report</h2>', CHAR(13, 10), '  <table border="1">', CHAR(13, 10), '    <tr bgcolor="#66ccff">', CHAR(13, 10), '		<th>PackageID</th>', CHAR(13, 10), '        <th>QuotaValue</th>', CHAR(13, 10), '        <th>Diskspace</th>', CHAR(13, 10), '        <th>UsagePercentage</th>', CHAR(13, 10), '        <th>PackageName</th>', CHAR(13, 10), '        <th>PackagesNumber</th>', CHAR(13, 10), '        <th>StatusID</th>', CHAR(13, 10), '        <th>UserID</th>', CHAR(13, 10), '      <th>Username</th>', CHAR(13, 10), '        <th>FirstName</th>', CHAR(13, 10), '        <th>LastName</th>', CHAR(13, 10), '        <th>FullName</th>', CHAR(13, 10), '        <th>RoleID</th>', CHAR(13, 10), '        <th>Email</th>', CHAR(13, 10), '        <th>UserComments</th> ', CHAR(13, 10), '    </tr>', CHAR(13, 10), '    <xsl:for-each select="//Table1">', CHAR(13, 10), '    <tr>', CHAR(13, 10), '	<td><xsl:value-of select="PackageID"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="QuotaValue"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="Diskspace"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="UsagePercentage"/>%</td>', CHAR(13, 10), '        <td><xsl:value-of select="PackageName"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="PackagesNumber"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="StatusID"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="UserID"/></td>', CHAR(13, 10), '      <td><xsl:value-of select="Username"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="FirstName"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="LastName"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="FullName"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="RoleID"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="Email"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="UserComments"/></td>', CHAR(13, 10), '    </tr>', CHAR(13, 10), '    </xsl:for-each>', CHAR(13, 10), '  </table>', CHAR(13, 10), '  </body>', CHAR(13, 10), '  </html>', CHAR(13, 10), '</xsl:template>', CHAR(13, 10), '</xsl:stylesheet>')),
    ('TransformContentType', 'BandwidthXLST', 1, 'test/html'),
    ('TransformSuffix', 'BandwidthXLST', 1, '.htm'),
    ('Transform', 'DiskspaceXLST', 1, CONCAT('<?xml version="1.0" encoding="UTF-8"?>', CHAR(13, 10), '<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">', CHAR(13, 10), '<xsl:template match="/">', CHAR(13, 10), '  <html>', CHAR(13, 10), '  <body>', CHAR(13, 10), '  <img alt="Embedded Image" width="299" height="60" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASsAAAA8CAYAAAA+AJwjAAAACXBIWXMAAAsTAAALEwEAmpwYAAA4G2lUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4KPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS41LWMwMTQgNzkuMTUxNDgxLCAyMDEzLzAzLzEzLTEyOjA5OjE1ICAgICAgICAiPgogICA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIKICAgICAgICAgICAgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIgogICAgICAgICAgICB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIKICAgICAgICAgICAgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIj4KICAgICAgICAgPHhtcDpDcmVhdG9yVG9vbD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC94bXA6Q3JlYXRvclRvb2w+CiAgICAgICAgIDx4bXA6Q3JlYXRlRGF0ZT4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC94bXA6Q3JlYXRlRGF0ZT4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTYtMDMtMDFUMTQ6NTE6NTgrMDE6MDA8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOk1ldGFkYXRhRGF0ZT4yMDE2LTAzLTAxVDE0OjUxOjU4KzAxOjAwPC94bXA6TWV0YWRhdGFEYXRlPgogICAgICAgICA8ZGM6Zm9ybWF0PmltYWdlL3BuZzwvZGM6Zm9ybWF0PgogICAgICAgICA8cGhvdG9zaG9wOkNvbG9yTW9kZT4zPC9waG90b3Nob3A6Q29sb3JNb2RlPgogICAgICAgICA8eG1wTU06SW5zdGFuY2VJRD54bXAuaWlkOjZhNTdmMWYyLTgyZjYtMjk0MS1hYjFmLTNkOWQ0YjdmMTY2YjwveG1wTU06SW5zdGFuY2VJRD4KICAgICAgICAgPHhtcE1NOkRvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOkRvY3VtZW50SUQ+CiAgICAgICAgIDx4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD4KICAgICAgICAgPHhtcE1NOkhpc3Rvcnk+CiAgICAgICAgICAgIDxyZGY6U2VxPgogICAgICAgICAgICAgICA8cmRmOmxpIHJkZjpwYXJzZVR5cGU9IlJlc291cmNlIj4KICAgICAgICAgICAgICAgICAgPHN0RXZ0OmFjdGlvbj5jcmVhdGVkPC9zdEV2dDphY3Rpb24+CiAgICAgICAgICAgICAgICAgIDxzdEV2dDppbnN0YW5jZUlEPnhtcC5paWQ6NmE1N2YxZjItODJmNi0yOTQxLWFiMWYtM2Q5ZDRiN2YxNjZiPC9zdEV2dDppbnN0YW5jZUlEPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6d2hlbj4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC9zdEV2dDp3aGVuPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6c29mdHdhcmVBZ2VudD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC9zdEV2dDpzb2Z0d2FyZUFnZW50PgogICAgICAgICAgICAgICA8L3JkZjpsaT4KICAgICAgICAgICAgPC9yZGY6U2VxPgogICAgICAgICA8L3htcE1NOkhpc3Rvcnk+CiAgICAgICAgIDx0aWZmOk9yaWVudGF0aW9uPjE8L3RpZmY6T3JpZW50YXRpb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyMDAwMC8xMDAwMDwvdGlmZjpYUmVzb2x1dGlvbj4KICAgICAgICAgPHRpZmY6WVJlc29sdXRpb24+NzIwMDAwLzEwMDAwPC90aWZmOllSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpSZXNvbHV0aW9uVW5pdD4yPC90aWZmOlJlc29sdXRpb25Vbml0PgogICAgICAgICA8ZXhpZjpDb2xvclNwYWNlPjY1NTM1PC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWERpbWVuc2lvbj4yOTk8L2V4aWY6UGl4ZWxYRGltZW5zaW9uPgogICAgICAgICA8ZXhpZjpQaXhlbFlEaW1lbnNpb24+NjA8L2V4aWY6UGl4ZWxZRGltZW5zaW9uPgogICAgICA8L3JkZjpEZXNjcmlwdGlvbj4KICAgPC9yZGY6UkRGPgo8L3g6eG1wbWV0YT4KICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAg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" />', CHAR(13, 10), '  <h2>DiskSpace Report</h2>', CHAR(13, 10), '  <table border="1">', CHAR(13, 10), '    <tr bgcolor="#66ccff">', CHAR(13, 10), '		<th>PackageID</th>', CHAR(13, 10), '        <th>QuotaValue</th>', CHAR(13, 10), '        <th>Bandwidth</th>', CHAR(13, 10), '        <th>UsagePercentage</th>', CHAR(13, 10), '        <th>PackageName</th>', CHAR(13, 10), '        <th>PackagesNumber</th>', CHAR(13, 10), '        <th>StatusID</th>', CHAR(13, 10), '        <th>UserID</th>', CHAR(13, 10), '      <th>Username</th>', CHAR(13, 10), '        <th>FirstName</th>', CHAR(13, 10), '        <th>LastName</th>', CHAR(13, 10), '        <th>FullName</th>', CHAR(13, 10), '        <th>RoleID</th>', CHAR(13, 10), '        <th>Email</th>', CHAR(13, 10), '    </tr>', CHAR(13, 10), '    <xsl:for-each select="//Table1">', CHAR(13, 10), '    <tr>', CHAR(13, 10), '	<td><xsl:value-of select="PackageID"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="QuotaValue"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="Bandwidth"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="UsagePercentage"/>%</td>', CHAR(13, 10), '        <td><xsl:value-of select="PackageName"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="PackagesNumber"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="StatusID"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="UserID"/></td>', CHAR(13, 10), '      <td><xsl:value-of select="Username"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="FirstName"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="LastName"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="FullName"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="RoleID"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="Email"/></td>', CHAR(13, 10), '        <td><xsl:value-of select="UserComments"/></td>', CHAR(13, 10), '    </tr>', CHAR(13, 10), '    </xsl:for-each>', CHAR(13, 10), '  </table>', CHAR(13, 10), '  </body>', CHAR(13, 10), '  </html>', CHAR(13, 10), '</xsl:template>', CHAR(13, 10), '</xsl:stylesheet>')),
    ('TransformContentType', 'DiskspaceXLST', 1, 'text/html'),
    ('TransformSuffix', 'DiskspaceXLST', 1, '.htm'),
    ('GridItems', 'DisplayPreferences', 1, '10'),
    ('CC', 'DomainExpirationLetter', 1, 'support@HostingCompany.com'),
    ('From', 'DomainExpirationLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'DomainExpirationLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>Domain Expiration Information</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '', CHAR(13, 10), '<a name="top"></a>', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '	Domain Expiration Information', CHAR(13, 10), '</div>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), '<p>', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</p>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Please, find below details of your domain expiration information.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<table>', CHAR(13, 10), '    <thead>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <th>Domain</th>', CHAR(13, 10), '			<th>Registrar</th>', CHAR(13, 10), '			<th>Customer</th>', CHAR(13, 10), '            <th>Expiration Date</th>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '    </thead>', CHAR(13, 10), '    <tbody>', CHAR(13, 10), '            <ad:foreach collection="#Domains#" var="Domain" index="i">', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td>#Domain.DomainName#</td>', CHAR(13, 10), '			<td>#iif(isnull(Domain.Registrar), "", Domain.Registrar)#</td>', CHAR(13, 10), '			<td>#Domain.Customer#</td>', CHAR(13, 10), '            <td>#iif(isnull(Domain.ExpirationDate), "", Domain.ExpirationDate)#</td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '    </ad:foreach>', CHAR(13, 10), '    </tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#IncludeNonExistenDomains#">', CHAR(13, 10), '	<p>', CHAR(13, 10), '	Please, find below details of your non-existen domains.', CHAR(13, 10), '	</p>', CHAR(13, 10), '', CHAR(13, 10), '	<table>', CHAR(13, 10), '		<thead>', CHAR(13, 10), '			<tr>', CHAR(13, 10), '				<th>Domain</th>', CHAR(13, 10), '				<th>Customer</th>', CHAR(13, 10), '			</tr>', CHAR(13, 10), '		</thead>', CHAR(13, 10), '		<tbody>', CHAR(13, 10), '				<ad:foreach collection="#NonExistenDomains#" var="Domain" index="i">', CHAR(13, 10), '			<tr>', CHAR(13, 10), '				<td>#Domain.DomainName#</td>', CHAR(13, 10), '				<td>#Domain.Customer#</td>', CHAR(13, 10), '			</tr>', CHAR(13, 10), '		</ad:foreach>', CHAR(13, 10), '		</tbody>', CHAR(13, 10), '	</table>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Best regards', CHAR(13, 10), '</p>')),
    ('Priority', 'DomainExpirationLetter', 1, 'Normal'),
    ('Subject', 'DomainExpirationLetter', 1, 'Domain expiration notification'),
    ('TextBody', 'DomainExpirationLetter', 1, CONCAT('=================================', CHAR(13, 10), '   Domain Expiration Information', CHAR(13, 10), '=================================', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'Please, find below details of your domain expiration information.', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '<ad:foreach collection="#Domains#" var="Domain" index="i">', CHAR(13, 10), '	Domain: #Domain.DomainName#', CHAR(13, 10), '	Registrar: #iif(isnull(Domain.Registrar), "", Domain.Registrar)#', CHAR(13, 10), '	Customer: #Domain.Customer#', CHAR(13, 10), '	Expiration Date: #iif(isnull(Domain.ExpirationDate), "", Domain.ExpirationDate)#', CHAR(13, 10), '', CHAR(13, 10), '</ad:foreach>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#IncludeNonExistenDomains#">', CHAR(13, 10), 'Please, find below details of your non-existen domains.', CHAR(13, 10), '', CHAR(13, 10), '<ad:foreach collection="#NonExistenDomains#" var="Domain" index="i">', CHAR(13, 10), '	Domain: #Domain.DomainName#', CHAR(13, 10), '	Customer: #Domain.Customer#', CHAR(13, 10), '', CHAR(13, 10), '</ad:foreach>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '', CHAR(13, 10), 'Best regards')),
    ('CC', 'DomainLookupLetter', 1, 'support@HostingCompany.com'),
    ('From', 'DomainLookupLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'DomainLookupLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>MX and NS Changes Information</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ', CHAR(13, 10), '		.Summary H3 { font-size: 1em; color: ##1F4978; } ', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '', CHAR(13, 10), '<a name="top"></a>', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '	MX and NS Changes Information', CHAR(13, 10), '</div>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), '<p>', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</p>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Please, find below details of MX and NS changes.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '    <ad:foreach collection="#Domains#" var="Domain" index="i">', CHAR(13, 10), '	<h2>#Domain.DomainName# - #DomainUsers[Domain.PackageId].FirstName# #DomainUsers[Domain.PackageId].LastName#</h2>', CHAR(13, 10), '	<h3>#iif(isnull(Domain.Registrar), "", Domain.Registrar)# #iif(isnull(Domain.ExpirationDate), "", Domain.ExpirationDate)#</h3>', CHAR(13, 10), '', CHAR(13, 10), '	<table>', CHAR(13, 10), '	    <thead>', CHAR(13, 10), '	        <tr>', CHAR(13, 10), '	            <th>DNS</th>', CHAR(13, 10), '				<th>Type</th>', CHAR(13, 10), '				<th>Status</th>', CHAR(13, 10), '	            <th>Old Value</th>', CHAR(13, 10), '                <th>New Value</th>', CHAR(13, 10), '	        </tr>', CHAR(13, 10), '	    </thead>', CHAR(13, 10), '	    <tbody>', CHAR(13, 10), '	        <ad:foreach collection="#Domain.DnsChanges#" var="DnsChange" index="j">', CHAR(13, 10), '	        <tr>', CHAR(13, 10), '	            <td>#DnsChange.DnsServer#</td>', CHAR(13, 10), '	            <td>#DnsChange.Type#</td>', CHAR(13, 10), '				<td>#DnsChange.Status#</td>', CHAR(13, 10), '                <td>#DnsChange.OldRecord.Value#</td>', CHAR(13, 10), '	            <td>#DnsChange.NewRecord.Value#</td>', CHAR(13, 10), '	        </tr>', CHAR(13, 10), '	    	</ad:foreach>', CHAR(13, 10), '	    </tbody>', CHAR(13, 10), '	</table>', CHAR(13, 10), '	', CHAR(13, 10), '    </ad:foreach>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Best regards', CHAR(13, 10), '</p>')),
    ('NoChangesHtmlBody', 'DomainLookupLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>MX and NS Changes Information</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '', CHAR(13, 10), '<a name="top"></a>', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '	MX and NS Changes Information', CHAR(13, 10), '</div>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), '<p>', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</p>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'No MX and NS changes have been found.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Best regards', CHAR(13, 10), '</p>')),
    ('NoChangesTextBody', 'DomainLookupLetter', 1, CONCAT('=================================', CHAR(13, 10), '   MX and NS Changes Information', CHAR(13, 10), '=================================', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'No MX and NS changes have been founded.', CHAR(13, 10), '', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '', CHAR(13, 10), 'Best regards', CHAR(13, 10), '')),
    ('Priority', 'DomainLookupLetter', 1, 'Normal'),
    ('Subject', 'DomainLookupLetter', 1, 'MX and NS changes notification'),
    ('TextBody', 'DomainLookupLetter', 1, CONCAT('=================================', CHAR(13, 10), '   MX and NS Changes Information', CHAR(13, 10), '=================================', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'Please, find below details of MX and NS changes.', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '<ad:foreach collection="#Domains#" var="Domain" index="i">', CHAR(13, 10), '', CHAR(13, 10), '# Domain.DomainName# - #DomainUsers[Domain.PackageId].FirstName# #DomainUsers[Domain.PackageId].LastName#', CHAR(13, 10), ' Registrar:      #iif(isnull(Domain.Registrar), "", Domain.Registrar)#', CHAR(13, 10), ' ExpirationDate: #iif(isnull(Domain.ExpirationDate), "", Domain.ExpirationDate)#', CHAR(13, 10), '', CHAR(13, 10), '        <ad:foreach collection="#Domain.DnsChanges#" var="DnsChange" index="j">', CHAR(13, 10), '            DNS:       #DnsChange.DnsServer#', CHAR(13, 10), '            Type:      #DnsChange.Type#', CHAR(13, 10), '	    Status:    #DnsChange.Status#', CHAR(13, 10), '            Old Value: #DnsChange.OldRecord.Value#', CHAR(13, 10), '            New Value: #DnsChange.NewRecord.Value#', CHAR(13, 10), '', CHAR(13, 10), '    	</ad:foreach>', CHAR(13, 10), '</ad:foreach>', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '', CHAR(13, 10), 'Best regards', CHAR(13, 10), '')),
    ('From', 'ExchangeMailboxSetupLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'ExchangeMailboxSetupLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>Account Summary Information</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '        body {font-family: ''Segoe UI Light'',''Open Sans'',Arial!important;color:black;}', CHAR(13, 10), '        p {color:black;}', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.SummaryHeader { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.5em; color: ##1F4978; border-bottom: dotted 3px ##efefef; font-weight:normal; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.2em; color: ##1F4978; } ', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; color:black;}', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '        .Label { color:##1F4978; }', CHAR(13, 10), '        .menu-bar a {padding: 15px 0;display: inline-block;}', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<table border="0" cellspacing="0" cellpadding="0" width="100%"><!-- was 800 -->', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="padding: 10px 20px 10px 20px; background-color: ##e1e1e1;">', CHAR(13, 10), '<table border="0" cellspacing="0" cellpadding="0" width="100%">', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="text-align: left; padding: 0px 0px 2px 0px;"><a href=""><img src="" border="0" alt="" /></a></td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '<table border="0" cellspacing="0" cellpadding="0" width="100%">', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="padding-bottom: 10px;">', CHAR(13, 10), '<table border="0" cellspacing="0" cellpadding="0" width="100%">', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="background-color: ##2e8bcc; padding: 3px;">', CHAR(13, 10), '<table class="menu-bar" border="0" cellspacing="0" cellpadding="0" width="100%">', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="text-align: center;" width="20%"><a style="color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;" href=""</a></td>', CHAR(13, 10), '<td style="text-align: center;" width="20%"><a style="color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;" href=""></a></td>', CHAR(13, 10), '<td style="text-align: center;" width="20%"><a style="color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;" href=""></a></td>', CHAR(13, 10), '<td style="text-align: center;" width="20%"><a style="color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;" href=""></a></td>', CHAR(13, 10), '<td style="text-align: center;" width="20%"><a style="color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;" href=""></a></td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '<table border="0" cellspacing="0" cellpadding="0" width="100%">', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="background-color: ##ffffff;">', CHAR(13, 10), '<table border="0" cellspacing="0" cellpadding="0" width="100%"><!-- was 759 -->', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="vertical-align: top; padding: 10px 10px 0px 10px;" width="100%">', CHAR(13, 10), '<table border="0" cellspacing="0" cellpadding="0" width="100%">', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="font-family: ''Segoe UI Light'',''Open Sans'',Arial; padding: 0px 10px 0px 0px;">', CHAR(13, 10), '<!-- Begin Content -->', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '    <ad:if test="#Email#">', CHAR(13, 10), '    <p>', CHAR(13, 10), '    Hello #Account.DisplayName#,', CHAR(13, 10), '    </p>', CHAR(13, 10), '    <p>', CHAR(13, 10), '    Thanks for choosing as your Exchange hosting provider.', CHAR(13, 10), '    </p>', CHAR(13, 10), '    </ad:if>', CHAR(13, 10), '    <ad:if test="#not(PMM)#">', CHAR(13, 10), '    <h1>User Accounts</h1>', CHAR(13, 10), '    <p>', CHAR(13, 10), '    The following user accounts have been created for you.', CHAR(13, 10), '    </p>', CHAR(13, 10), '    <table>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td class="Label">Username:</td>', CHAR(13, 10), '            <td>#Account.UserPrincipalName#</td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td class="Label">E-mail:</td>', CHAR(13, 10), '            <td>#Account.PrimaryEmailAddress#</td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '		<ad:if test="#PswResetUrl#">', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td class="Label">Password Reset Url:</td>', CHAR(13, 10), '            <td><a href="#PswResetUrl#" target="_blank">Click here</a></td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '		</ad:if>', CHAR(13, 10), '    </table>', CHAR(13, 10), '    </ad:if>', CHAR(13, 10), '    <h1>DNS</h1>', CHAR(13, 10), '    <p>', CHAR(13, 10), '    In order for us to accept mail for your domain, you will need to point your MX records to:', CHAR(13, 10), '    </p>', CHAR(13, 10), '    <table>', CHAR(13, 10), '        <ad:foreach collection="#SmtpServers#" var="SmtpServer" index="i">', CHAR(13, 10), '            <tr>', CHAR(13, 10), '                <td class="Label">#SmtpServer#</td>', CHAR(13, 10), '            </tr>', CHAR(13, 10), '        </ad:foreach>', CHAR(13, 10), '    </table>', CHAR(13, 10), '   <h1>', CHAR(13, 10), '    Webmail (OWA, Outlook Web Access)</h1>', CHAR(13, 10), '    <p>', CHAR(13, 10), '    <a href="" target="_blank"></a>', CHAR(13, 10), '    </p>', CHAR(13, 10), '    <h1>', CHAR(13, 10), '    Outlook (Windows Clients)</h1>', CHAR(13, 10), '    <p>', CHAR(13, 10), '    To configure MS Outlook to work with the servers, please reference:', CHAR(13, 10), '    </p>', CHAR(13, 10), '    <p>', CHAR(13, 10), '    <a href="" target="_blank"></a>', CHAR(13, 10), '    </p>', CHAR(13, 10), '    <p>', CHAR(13, 10), '    If you need to download and install the Outlook client:</p>', CHAR(13, 10), '        ', CHAR(13, 10), '        <table>', CHAR(13, 10), '            <tr><td colspan="2" class="Label"><font size="3">MS Outlook Client</font></td></tr>', CHAR(13, 10), '            <tr>', CHAR(13, 10), '                <td class="Label">', CHAR(13, 10), '                    Download URL:</td>', CHAR(13, 10), '                <td><a href=""></a></td>', CHAR(13, 10), '            </tr>', CHAR(13, 10), '<tr>', CHAR(13, 10), '                <td class="Label"></td>', CHAR(13, 10), '                <td><a href=""></a></td>', CHAR(13, 10), '            </tr>', CHAR(13, 10), '            <tr>', CHAR(13, 10), '                <td class="Label">', CHAR(13, 10), '                    KEY:</td>', CHAR(13, 10), '                <td></td>', CHAR(13, 10), '            </tr>', CHAR(13, 10), '        </table>', CHAR(13, 10), ' ', CHAR(13, 10), '       <h1>', CHAR(13, 10), '    ActiveSync, iPhone, iPad</h1>', CHAR(13, 10), '    <table>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td class="Label">Server:</td>', CHAR(13, 10), '            <td>#ActiveSyncServer#</td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td class="Label">Domain:</td>', CHAR(13, 10), '            <td>#SamDomain#</td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td class="Label">SSL:</td>', CHAR(13, 10), '            <td>must be checked</td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td class="Label">Your username:</td>', CHAR(13, 10), '            <td>#SamUsername#</td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '    </table>', CHAR(13, 10), ' ', CHAR(13, 10), '    <h1>Password Changes</h1>', CHAR(13, 10), '    <p>', CHAR(13, 10), '    Passwords can be changed at any time using Webmail or the <a href="" target="_blank">Control Panel</a>.</p>', CHAR(13, 10), '    <h1>Control Panel</h1>', CHAR(13, 10), '    <p>', CHAR(13, 10), '    If you need to change the details of your account, you can easily do this using <a href="" target="_blank">Control Panel</a>.</p>', CHAR(13, 10), '    <h1>Support</h1>', CHAR(13, 10), '    <p>', CHAR(13, 10), '    You have 2 options, email <a href="mailto:"></a> or use the web interface at <a href=""></a></p>', CHAR(13, 10), '    ', CHAR(13, 10), '</div>', CHAR(13, 10), '<!-- End Content -->', CHAR(13, 10), '<br></td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="background-color: ##ffffff; border-top: 1px solid ##999999;">', CHAR(13, 10), '<table border="0" cellspacing="0" cellpadding="0" width="100%">', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="vertical-align: top; padding: 0px 20px 15px 20px;">', CHAR(13, 10), '<table border="0" cellspacing="0" cellpadding="0" width="100%">', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="font-family: Arial, Helvetica, sans-serif; text-align: left; font-size: 9px; color: ##717073; padding: 20px 0px 0px 0px;">', CHAR(13, 10), '<table border="0" cellspacing="0" cellpadding="0" width="100%">', CHAR(13, 10), '<tbody>', CHAR(13, 10), '<tr>', CHAR(13, 10), '<td style="font-family: Arial, Helvetica, sans-serif; font-size: 9px; text-align: left; color: ##1666af; vertical-align: top;" width="33%"><a style="font-weight: bold; text-transform: uppercase; text-decoration: underline; color: ##1666af;" href=""></a><br />Learn more about the services can provide to improve your business.</td>', CHAR(13, 10), '<td style="font-family: Arial, Helvetica, sans-serif; font-size: 9px; text-align: left; color: ##1666af; padding: 0px 10px 0px 10px; vertical-align: top;" width="34%"><a style="font-weight: bold; text-transform: uppercase; text-decoration: underline; color: ##1666af;" href="">Privacy Policy</a><br /> follows strict guidelines in protecting your privacy. Learn about our <a style="font-weight: bold; text-decoration: underline; color: ##1666af;" href="">Privacy Policy</a>.</td>', CHAR(13, 10), '<td style="font-family: Arial, Helvetica, sans-serif; font-size: 9px; text-align: left; color: ##1666af; vertical-align: top;" width="33%"><a style="font-weight: bold; text-transform: uppercase; text-decoration: underline; color: ##1666af;" href="">Contact Us</a><br />Questions? For more information, <a style="font-weight: bold; text-decoration: underline; color: ##1666af;" href="">contact us</a>.</td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</td>', CHAR(13, 10), '</tr>', CHAR(13, 10), '</tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '</body>', CHAR(13, 10), '</html>')),
    ('Priority', 'ExchangeMailboxSetupLetter', 1, 'Normal'),
    ('Subject', 'ExchangeMailboxSetupLetter', 1, ' Hosted Exchange Mailbox Setup'),
    ('TextBody', 'ExchangeMailboxSetupLetter', 1, CONCAT('<ad:if test="#Email#">', CHAR(13, 10), 'Hello #Account.DisplayName#,', CHAR(13, 10), '', CHAR(13, 10), 'Thanks for choosing as your Exchange hosting provider.', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '<ad:if test="#not(PMM)#">', CHAR(13, 10), 'User Accounts', CHAR(13, 10), '', CHAR(13, 10), 'The following user accounts have been created for you.', CHAR(13, 10), '', CHAR(13, 10), 'Username: #Account.UserPrincipalName#', CHAR(13, 10), 'E-mail: #Account.PrimaryEmailAddress#', CHAR(13, 10), '<ad:if test="#PswResetUrl#">', CHAR(13, 10), 'Password Reset Url: #PswResetUrl#', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '=================================', CHAR(13, 10), 'DNS', CHAR(13, 10), '=================================', CHAR(13, 10), '', CHAR(13, 10), 'In order for us to accept mail for your domain, you will need to point your MX records to:', CHAR(13, 10), '', CHAR(13, 10), '<ad:foreach collection="#SmtpServers#" var="SmtpServer" index="i">#SmtpServer#</ad:foreach>', CHAR(13, 10), '', CHAR(13, 10), '=================================', CHAR(13, 10), 'Webmail (OWA, Outlook Web Access)', CHAR(13, 10), '=================================', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '=================================', CHAR(13, 10), 'Outlook (Windows Clients)', CHAR(13, 10), '=================================', CHAR(13, 10), '', CHAR(13, 10), 'To configure MS Outlook to work with servers, please reference:', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), 'If you need to download and install the MS Outlook client:', CHAR(13, 10), '', CHAR(13, 10), 'MS Outlook Download URL:', CHAR(13, 10), '', CHAR(13, 10), 'KEY: ', CHAR(13, 10), '', CHAR(13, 10), '=================================', CHAR(13, 10), 'ActiveSync, iPhone, iPad', CHAR(13, 10), '=================================', CHAR(13, 10), '', CHAR(13, 10), 'Server: #ActiveSyncServer#', CHAR(13, 10), 'Domain: #SamDomain#', CHAR(13, 10), 'SSL: must be checked', CHAR(13, 10), 'Your username: #SamUsername#', CHAR(13, 10), '', CHAR(13, 10), '=================================', CHAR(13, 10), 'Password Changes', CHAR(13, 10), '=================================', CHAR(13, 10), '', CHAR(13, 10), 'Passwords can be changed at any time using Webmail or the Control Panel', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '=================================', CHAR(13, 10), 'Control Panel', CHAR(13, 10), '=================================', CHAR(13, 10), '', CHAR(13, 10), 'If you need to change the details of your account, you can easily do this using the Control Panel ', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '=================================', CHAR(13, 10), 'Support', CHAR(13, 10), '=================================', CHAR(13, 10), '', CHAR(13, 10), 'You have 2 options, email or use the web interface at ')),
    ('MailboxPasswordPolicy', 'ExchangePolicy', 1, 'True;8;20;0;2;0;True'),
    ('UserNamePolicy', 'FtpPolicy', 1, 'True;-;1;20;;;'),
    ('UserPasswordPolicy', 'FtpPolicy', 1, 'True;5;20;0;1;0;True'),
    ('AccountNamePolicy', 'MailPolicy', 1, 'True;;1;50;;;'),
    ('AccountPasswordPolicy', 'MailPolicy', 1, 'True;5;20;0;1;0;False;;0;;;False;False;0;'),
    ('CatchAllName', 'MailPolicy', 1, 'mail'),
    ('DatabaseNamePolicy', 'MariaDBPolicy', 1, 'True;;1;40;;;'),
    ('UserNamePolicy', 'MariaDBPolicy', 1, 'True;;1;16;;;'),
    ('UserPasswordPolicy', 'MariaDBPolicy', 1, 'True;5;20;0;1;0;False;;0;;;False;False;0;');
    INSERT INTO `UserSettings` (`PropertyName`, `SettingsName`, `UserID`, `PropertyValue`)
    VALUES ('DatabaseNamePolicy', 'MsSqlPolicy', 1, 'True;-;1;120;;;'),
    ('UserNamePolicy', 'MsSqlPolicy', 1, 'True;-;1;120;;;'),
    ('UserPasswordPolicy', 'MsSqlPolicy', 1, 'True;5;20;0;1;0;True;;0;0;0;False;False;0;'),
    ('DatabaseNamePolicy', 'MySqlPolicy', 1, 'True;;1;40;;;'),
    ('UserNamePolicy', 'MySqlPolicy', 1, 'True;;1;16;;;'),
    ('UserPasswordPolicy', 'MySqlPolicy', 1, 'True;5;20;0;1;0;False;;0;0;0;False;False;0;'),
    ('From', 'OrganizationUserPasswordRequestLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'OrganizationUserPasswordRequestLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>Password request notification</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '<img src="#logoUrl#">', CHAR(13, 10), '</div>', CHAR(13, 10), '<h1>Password request notification</h1>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), '<p>', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</p>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Your account have been created. In order to create a password for your account, please follow next link:', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<a href="#passwordResetLink#" target="_blank">#passwordResetLink#</a>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Best regards', CHAR(13, 10), '</p>', CHAR(13, 10), '</div>', CHAR(13, 10), '</body>')),
    ('LogoUrl', 'OrganizationUserPasswordRequestLetter', 1, ''),
    ('Priority', 'OrganizationUserPasswordRequestLetter', 1, 'Normal'),
    ('SMSBody', 'OrganizationUserPasswordRequestLetter', 1, CONCAT('', CHAR(13, 10), 'User have been created. Password request url:', CHAR(13, 10), '# passwordResetLink#')),
    ('Subject', 'OrganizationUserPasswordRequestLetter', 1, 'Password request notification'),
    ('TextBody', 'OrganizationUserPasswordRequestLetter', 1, CONCAT('=========================================', CHAR(13, 10), '   Password request notification', CHAR(13, 10), '=========================================', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'Your account have been created. In order to create a password for your account, please follow next link:', CHAR(13, 10), '', CHAR(13, 10), '# passwordResetLink#', CHAR(13, 10), '', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '', CHAR(13, 10), 'Best regards')),
    ('DsnNamePolicy', 'OsPolicy', 1, 'True;-;2;40;;;'),
    ('CC', 'PackageSummaryLetter', 1, 'support@HostingCompany.com'),
    ('EnableLetter', 'PackageSummaryLetter', 1, 'True'),
    ('From', 'PackageSummaryLetter', 1, 'support@HostingCompany.com'),
    ('Priority', 'PackageSummaryLetter', 1, 'Normal'),
    ('Subject', 'PackageSummaryLetter', 1, '"#space.Package.PackageName#" <ad:if test="#Signup#">hosting space has been created for<ad:else>hosting space summary for</ad:if> #user.FirstName# #user.LastName#'),
    ('CC', 'PasswordReminderLetter', 1, ''),
    ('From', 'PasswordReminderLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'PasswordReminderLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>Account Summary Information</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; }', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '', CHAR(13, 10), '<a name="top"></a>', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '	Hosting Account Information', CHAR(13, 10), '</div>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Please, find below details of your control panel account. The one time password was generated for you. You should change the password after login. ', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<h1>Control Panel URL</h1>', CHAR(13, 10), '<table>', CHAR(13, 10), '    <thead>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <th>Control Panel URL</th>', CHAR(13, 10), '            <th>Username</th>', CHAR(13, 10), '            <th>One Time Password</th>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '    </thead>', CHAR(13, 10), '    <tbody>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td><a href="http://panel.HostingCompany.com">http://panel.HostingCompany.com</a></td>', CHAR(13, 10), '            <td>#user.Username#</td>', CHAR(13, 10), '            <td>#user.Password#</td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '    </tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Best regards,<br />', CHAR(13, 10), 'FuseCP.<br />', CHAR(13, 10), 'Web Site: <a href="https://fusecp.com">https://fusecp.com</a><br />', CHAR(13, 10), 'E-Mail: <a href="mailto:support@fusecp.com">support@fusecp.com</a>', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '</div>', CHAR(13, 10), '</body>', CHAR(13, 10), '</html>')),
    ('Priority', 'PasswordReminderLetter', 1, 'Normal'),
    ('Subject', 'PasswordReminderLetter', 1, 'Password reminder for #user.FirstName# #user.LastName#'),
    ('TextBody', 'PasswordReminderLetter', 1, CONCAT('=================================', CHAR(13, 10), '   Hosting Account Information', CHAR(13, 10), '=================================', CHAR(13, 10), '', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '', CHAR(13, 10), 'Please, find below details of your control panel account. The one time password was generated for you. You should change the password after login.', CHAR(13, 10), '', CHAR(13, 10), 'Control Panel URL: https://panel.fusecp.com', CHAR(13, 10), 'Username: #user.Username#', CHAR(13, 10), 'One Time Password: #user.Password#', CHAR(13, 10), '', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '', CHAR(13, 10), 'Best regards,', CHAR(13, 10), 'FuseCP.', CHAR(13, 10), 'Web Site: https://fusecp.com"', CHAR(13, 10), 'E-Mail: support@fusecp.com')),
    ('CC', 'RDSSetupLetter', 1, 'support@HostingCompany.com'),
    ('From', 'RDSSetupLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'RDSSetupLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>RDS Setup Information</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '', CHAR(13, 10), '<a name="top"></a>', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '	RDS Setup Information', CHAR(13, 10), '</div>', CHAR(13, 10), '</div>', CHAR(13, 10), '</body>')),
    ('Priority', 'RDSSetupLetter', 1, 'Normal'),
    ('Subject', 'RDSSetupLetter', 1, 'RDS setup'),
    ('TextBody', 'RDSSetupLetter', 1, CONCAT('=================================', CHAR(13, 10), '   RDS Setup Information', CHAR(13, 10), '=================================', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'Please, find below RDS setup instructions.', CHAR(13, 10), '', CHAR(13, 10), 'If you have any questions, feel free to contact our support department at any time.', CHAR(13, 10), '', CHAR(13, 10), 'Best regards')),
    ('GroupNamePolicy', 'SharePointPolicy', 1, 'True;-;1;20;;;'),
    ('UserNamePolicy', 'SharePointPolicy', 1, 'True;-;1;20;;;'),
    ('UserPasswordPolicy', 'SharePointPolicy', 1, 'True;5;20;0;1;0;True;;0;;;False;False;0;'),
    ('DemoMessage', 'FuseCPPolicy', 1, CONCAT('When user account is in demo mode the majority of operations are', CHAR(13, 10), 'disabled, especially those ones that modify or delete records.', CHAR(13, 10), 'You are welcome to ask your questions or place comments about', CHAR(13, 10), 'this demo on  <a href="http://forum.FuseCP.net"', CHAR(13, 10), 'target="_blank">FuseCP  Support Forum</a>')),
    ('ForbiddenIP', 'FuseCPPolicy', 1, ''),
    ('PasswordPolicy', 'FuseCPPolicy', 1, 'True;6;20;0;1;0;True;;0;;;False;False;0;'),
    ('From', 'UserPasswordExpirationLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'UserPasswordExpirationLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>Password expiration notification</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '<img src="#logoUrl#">', CHAR(13, 10), '</div>', CHAR(13, 10), '<h1>Password expiration notification</h1>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), '<p>', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</p>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Your password expiration date is #user.PasswordExpirationDateTime#. You can reset your own password by visiting the following page:', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<a href="#passwordResetLink#" target="_blank">#passwordResetLink#</a>', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Best regards', CHAR(13, 10), '</p>', CHAR(13, 10), '</div>', CHAR(13, 10), '</body>')),
    ('LogoUrl', 'UserPasswordExpirationLetter', 1, ''),
    ('Priority', 'UserPasswordExpirationLetter', 1, 'Normal'),
    ('Subject', 'UserPasswordExpirationLetter', 1, 'Password expiration notification');
    INSERT INTO `UserSettings` (`PropertyName`, `SettingsName`, `UserID`, `PropertyValue`)
    VALUES ('TextBody', 'UserPasswordExpirationLetter', 1, CONCAT('=========================================', CHAR(13, 10), '   Password expiration notification', CHAR(13, 10), '=========================================', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'Your password expiration date is #user.PasswordExpirationDateTime#. You can reset your own password by visiting the following page:', CHAR(13, 10), '', CHAR(13, 10), '# passwordResetLink#', CHAR(13, 10), '', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '', CHAR(13, 10), 'Best regards')),
    ('From', 'UserPasswordResetLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'UserPasswordResetLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>Password reset notification</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '<img src="#logoUrl#">', CHAR(13, 10), '</div>', CHAR(13, 10), '<h1>Password reset notification</h1>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), '<p>', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</p>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'We received a request to reset the password for your account. If you made this request, click the link below. If you did not make this request, you can ignore this email.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<a href="#passwordResetLink#" target="_blank">#passwordResetLink#</a>', CHAR(13, 10), '', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Best regards', CHAR(13, 10), '</p>', CHAR(13, 10), '</div>', CHAR(13, 10), '</body>')),
    ('LogoUrl', 'UserPasswordResetLetter', 1, ''),
    ('PasswordResetLinkSmsBody', 'UserPasswordResetLetter', 1, CONCAT('Password reset link:', CHAR(13, 10), '# passwordResetLink#', CHAR(13, 10), '')),
    ('Priority', 'UserPasswordResetLetter', 1, 'Normal'),
    ('Subject', 'UserPasswordResetLetter', 1, 'Password reset notification'),
    ('TextBody', 'UserPasswordResetLetter', 1, CONCAT('=========================================', CHAR(13, 10), '   Password reset notification', CHAR(13, 10), '=========================================', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'We received a request to reset the password for your account. If you made this request, click the link below. If you did not make this request, you can ignore this email.', CHAR(13, 10), '', CHAR(13, 10), '# passwordResetLink#', CHAR(13, 10), '', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '', CHAR(13, 10), 'Best regards')),
    ('From', 'UserPasswordResetPincodeLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'UserPasswordResetPincodeLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>Password reset notification</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; } ', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '<img src="#logoUrl#">', CHAR(13, 10), '</div>', CHAR(13, 10), '<h1>Password reset notification</h1>', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), '<p>', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</p>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'We received a request to reset the password for your account. Your password reset pincode:', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '# passwordResetPincode#', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Best regards', CHAR(13, 10), '</p>', CHAR(13, 10), '</div>', CHAR(13, 10), '</body>')),
    ('LogoUrl', 'UserPasswordResetPincodeLetter', 1, ''),
    ('PasswordResetPincodeSmsBody', 'UserPasswordResetPincodeLetter', 1, CONCAT('', CHAR(13, 10), 'Your password reset pincode:', CHAR(13, 10), '# passwordResetPincode#')),
    ('Priority', 'UserPasswordResetPincodeLetter', 1, 'Normal'),
    ('Subject', 'UserPasswordResetPincodeLetter', 1, 'Password reset notification'),
    ('TextBody', 'UserPasswordResetPincodeLetter', 1, CONCAT('=========================================', CHAR(13, 10), '   Password reset notification', CHAR(13, 10), '=========================================', CHAR(13, 10), '', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'We received a request to reset the password for your account. Your password reset pincode:', CHAR(13, 10), '', CHAR(13, 10), '# passwordResetPincode#', CHAR(13, 10), '', CHAR(13, 10), 'If you have any questions regarding your hosting account, feel free to contact our support department at any time.', CHAR(13, 10), '', CHAR(13, 10), 'Best regards')),
    ('CC', 'VerificationCodeLetter', 1, 'support@HostingCompany.com'),
    ('From', 'VerificationCodeLetter', 1, 'support@HostingCompany.com'),
    ('HtmlBody', 'VerificationCodeLetter', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>Verification code</title>', CHAR(13, 10), '    <style type="text/css">', CHAR(13, 10), '		.Summary { background-color: ##ffffff; padding: 5px; }', CHAR(13, 10), '		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }', CHAR(13, 10), '        .Summary A { color: ##0153A4; }', CHAR(13, 10), '        .Summary { font-family: Tahoma; font-size: 9pt; }', CHAR(13, 10), '        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }', CHAR(13, 10), '        .Summary H2 { font-size: 1.3em; color: ##1F4978; }', CHAR(13, 10), '        .Summary TABLE { border: solid 1px ##e5e5e5; }', CHAR(13, 10), '        .Summary TH,', CHAR(13, 10), '        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }', CHAR(13, 10), '        .Summary TD { padding: 8px; font-size: 9pt; }', CHAR(13, 10), '        .Summary UL LI { font-size: 1.1em; font-weight: bold; }', CHAR(13, 10), '        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }', CHAR(13, 10), '    </style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div class="Summary">', CHAR(13, 10), '', CHAR(13, 10), '<a name="top"></a>', CHAR(13, 10), '<div class="Header">', CHAR(13, 10), '	Verification code', CHAR(13, 10), '</div>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'to complete the sign in, enter the verification code on the device. ', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '<table>', CHAR(13, 10), '    <thead>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <th>Verification code</th>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '    </thead>', CHAR(13, 10), '    <tbody>', CHAR(13, 10), '        <tr>', CHAR(13, 10), '            <td>#verificationCode#</td>', CHAR(13, 10), '        </tr>', CHAR(13, 10), '    </tbody>', CHAR(13, 10), '</table>', CHAR(13, 10), '', CHAR(13, 10), '<p>', CHAR(13, 10), 'Best regards,<br />', CHAR(13, 10), '', CHAR(13, 10), '</p>', CHAR(13, 10), '', CHAR(13, 10), '</div>', CHAR(13, 10), '</body>', CHAR(13, 10), '</html>')),
    ('Priority', 'VerificationCodeLetter', 1, 'Normal'),
    ('Subject', 'VerificationCodeLetter', 1, 'Verification code'),
    ('TextBody', 'VerificationCodeLetter', 1, CONCAT('=================================', CHAR(13, 10), '   Verification code', CHAR(13, 10), '=================================', CHAR(13, 10), '<ad:if test="#user#">', CHAR(13, 10), 'Hello #user.FirstName#,', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '', CHAR(13, 10), 'to complete the sign in, enter the verification code on the device.', CHAR(13, 10), '', CHAR(13, 10), 'Verification code', CHAR(13, 10), '# verificationCode#', CHAR(13, 10), '', CHAR(13, 10), 'Best regards,', CHAR(13, 10), '')),
    ('AddParkingPage', 'WebPolicy', 1, 'True'),
    ('AddRandomDomainString', 'WebPolicy', 1, 'False'),
    ('AnonymousAccountPolicy', 'WebPolicy', 1, 'True;;5;20;;_web;'),
    ('AspInstalled', 'WebPolicy', 1, 'True'),
    ('AspNetInstalled', 'WebPolicy', 1, '2'),
    ('CgiBinInstalled', 'WebPolicy', 1, 'False'),
    ('DefaultDocuments', 'WebPolicy', 1, 'Default.htm,Default.asp,index.htm,Default.aspx'),
    ('EnableAnonymousAccess', 'WebPolicy', 1, 'True'),
    ('EnableBasicAuthentication', 'WebPolicy', 1, 'False'),
    ('EnableDedicatedPool', 'WebPolicy', 1, 'False'),
    ('EnableDirectoryBrowsing', 'WebPolicy', 1, 'False'),
    ('EnableParentPaths', 'WebPolicy', 1, 'False'),
    ('EnableParkingPageTokens', 'WebPolicy', 1, 'False'),
    ('EnableWindowsAuthentication', 'WebPolicy', 1, 'True'),
    ('EnableWritePermissions', 'WebPolicy', 1, 'False'),
    ('FrontPageAccountPolicy', 'WebPolicy', 1, 'True;;1;20;;;'),
    ('FrontPagePasswordPolicy', 'WebPolicy', 1, 'True;5;20;0;1;0;False;;0;0;0;False;False;0;'),
    ('ParkingPageContent', 'WebPolicy', 1, CONCAT('<html xmlns="http://www.w3.org/1999/xhtml">', CHAR(13, 10), '<head>', CHAR(13, 10), '    <title>The web site is under construction</title>', CHAR(13, 10), '<style type="text/css">', CHAR(13, 10), '	H1 { font-size: 16pt; margin-bottom: 4px; }', CHAR(13, 10), '	H2 { font-size: 14pt; margin-bottom: 4px; font-weight: normal; }', CHAR(13, 10), '</style>', CHAR(13, 10), '</head>', CHAR(13, 10), '<body>', CHAR(13, 10), '<div id="PageOutline">', CHAR(13, 10), '	<h1>This web site has just been created from <a href="https://www.FuseCP.com">FuseCP </a> and it is still under construction.</h1>', CHAR(13, 10), '	<h2>The web site is hosted by <a href="https://fusecp.com">FuseCP</a>.</h2>', CHAR(13, 10), '</div>', CHAR(13, 10), '</body>', CHAR(13, 10), '</html>')),
    ('ParkingPageName', 'WebPolicy', 1, 'default.aspx'),
    ('PerlInstalled', 'WebPolicy', 1, 'False'),
    ('PhpInstalled', 'WebPolicy', 1, '');
    INSERT INTO `UserSettings` (`PropertyName`, `SettingsName`, `UserID`, `PropertyValue`)
    VALUES ('PublishingProfile', 'WebPolicy', 1, CONCAT('<?xml version="1.0" encoding="utf-8"?>', CHAR(13, 10), '<publishData>', CHAR(13, 10), '<ad:if test="#WebSite.WebDeploySitePublishingEnabled#">', CHAR(13, 10), '	<publishProfile', CHAR(13, 10), '		profileName="#WebSite.Name# - Web Deploy"', CHAR(13, 10), '		publishMethod="MSDeploy"', CHAR(13, 10), '		publishUrl="#WebSite["WmSvcServiceUrl"]#:#WebSite["WmSvcServicePort"]#"', CHAR(13, 10), '		msdeploySite="#WebSite.Name#"', CHAR(13, 10), '		userName="#WebSite.WebDeployPublishingAccount#"', CHAR(13, 10), '		userPWD="#WebSite.WebDeployPublishingPassword#"', CHAR(13, 10), '		destinationAppUrl="http://#WebSite.Name#/"', CHAR(13, 10), '		<ad:if test="#Not(IsNull(MsSqlDatabase)) and Not(IsNull(MsSqlUser))#">SQLServerDBConnectionString="server=#MsSqlServerExternalAddress#;Initial Catalog=#MsSqlDatabase.Name#;uid=#MsSqlUser.Name#;pwd=#MsSqlUser.Password#"</ad:if>', CHAR(13, 10), '		<ad:if test="#Not(IsNull(MySqlDatabase)) and Not(IsNull(MySqlUser))#">mySQLDBConnectionString="server=#MySqlAddress#;database=#MySqlDatabase.Name#;uid=#MySqlUser.Name#;pwd=#MySqlUser.Password#"</ad:if>', CHAR(13, 10), '		<ad:if test="#Not(IsNull(MariaDBDatabase)) and Not(IsNull(MariaDBUser))#">MariaDBDBConnectionString="server=#MariaDBAddress#;database=#MariaDBDatabase.Name#;uid=#MariaDBUser.Name#;pwd=#MariaDBUser.Password#"</ad:if>', CHAR(13, 10), '		hostingProviderForumLink="https://fusecp.com/support"', CHAR(13, 10), '		controlPanelLink="https://panel.fusecp.com/"', CHAR(13, 10), '	/>', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '<ad:if test="#IsDefined("FtpAccount")#">', CHAR(13, 10), '	<publishProfile', CHAR(13, 10), '		profileName="#WebSite.Name# - FTP"', CHAR(13, 10), '		publishMethod="FTP"', CHAR(13, 10), '		publishUrl="ftp://#FtpServiceAddress#"', CHAR(13, 10), '		ftpPassiveMode="True"', CHAR(13, 10), '		userName="#FtpAccount.Name#"', CHAR(13, 10), '		userPWD="#FtpAccount.Password#"', CHAR(13, 10), '		destinationAppUrl="http://#WebSite.Name#/"', CHAR(13, 10), '		<ad:if test="#Not(IsNull(MsSqlDatabase)) and Not(IsNull(MsSqlUser))#">SQLServerDBConnectionString="server=#MsSqlServerExternalAddress#;Initial Catalog=#MsSqlDatabase.Name#;uid=#MsSqlUser.Name#;pwd=#MsSqlUser.Password#"</ad:if>', CHAR(13, 10), '		<ad:if test="#Not(IsNull(MySqlDatabase)) and Not(IsNull(MySqlUser))#">mySQLDBConnectionString="server=#MySqlAddress#;database=#MySqlDatabase.Name#;uid=#MySqlUser.Name#;pwd=#MySqlUser.Password#"</ad:if>', CHAR(13, 10), '		<ad:if test="#Not(IsNull(MariaDBDatabase)) and Not(IsNull(MariaDBUser))#">MariaDBDBConnectionString="server=#MariaDBAddress#;database=#MariaDBDatabase.Name#;uid=#MariaDBUser.Name#;pwd=#MariaDBUser.Password#"</ad:if>', CHAR(13, 10), '		hostingProviderForumLink="https://fusecp.com/support"', CHAR(13, 10), '		controlPanelLink="https://panel.fusecp.com/"', CHAR(13, 10), '    />', CHAR(13, 10), '</ad:if>', CHAR(13, 10), '</publishData>', CHAR(13, 10), '', CHAR(13, 10), '<!--', CHAR(13, 10), 'Control Panel:', CHAR(13, 10), 'Username: #User.Username#', CHAR(13, 10), 'Password: #User.Password#', CHAR(13, 10), '', CHAR(13, 10), 'Technical Contact:', CHAR(13, 10), 'support@fusecp.com', CHAR(13, 10), '-->')),
    ('PythonInstalled', 'WebPolicy', 1, 'False'),
    ('SecuredGroupNamePolicy', 'WebPolicy', 1, 'True;;1;20;;;'),
    ('SecuredUserNamePolicy', 'WebPolicy', 1, 'True;;1;20;;;'),
    ('SecuredUserPasswordPolicy', 'WebPolicy', 1, 'True;5;20;0;1;0;False;;0;0;0;False;False;0;'),
    ('VirtDirNamePolicy', 'WebPolicy', 1, 'True;-;3;50;;;'),
    ('WebDataFolder', 'WebPolicy', 1, '\\[DOMAIN_NAME]\\data'),
    ('WebLogsFolder', 'WebPolicy', 1, '\\[DOMAIN_NAME]\\logs'),
    ('WebRootFolder', 'WebPolicy', 1, '\\[DOMAIN_NAME]\\wwwroot');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `PackagesTreeCache` (`PackageID`, `ParentPackageID`)
    VALUES (1, 1);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `Quotas` (`QuotaID`, `GroupID`, `HideQuota`, `ItemTypeID`, `PerOrganization`, `QuotaDescription`, `QuotaName`, `QuotaOrder`, `QuotaTypeID`, `ServiceQuota`)
    VALUES (2, 6, NULL, 7, NULL, 'Databases', 'MySQL4.Databases', 1.0, 2, TRUE),
    (3, 5, NULL, 5, NULL, 'Databases', 'MsSQL2000.Databases', 1.0, 2, TRUE),
    (4, 3, NULL, 9, NULL, 'FTP Accounts', 'FTP.Accounts', 1.0, 2, TRUE),
    (12, 8, NULL, 14, NULL, 'Statistics Sites', 'Stats.Sites', 1.0, 2, TRUE),
    (13, 2, NULL, 10, NULL, 'Web Sites', 'Web.Sites', 1.0, 2, TRUE),
    (14, 4, NULL, 15, NULL, 'Mail Accounts', 'Mail.Accounts', 1.0, 2, TRUE),
    (15, 5, NULL, 6, NULL, 'Users', 'MsSQL2000.Users', 2.0, 2, FALSE),
    (18, 4, NULL, 16, NULL, 'Mail Forwardings', 'Mail.Forwardings', 3.0, 2, FALSE),
    (19, 6, NULL, 8, NULL, 'Users', 'MySQL4.Users', 2.0, 2, FALSE),
    (20, 4, NULL, 17, NULL, 'Mail Lists', 'Mail.Lists', 6.0, 2, FALSE),
    (24, 4, NULL, 18, NULL, 'Mail Groups', 'Mail.Groups', 4.0, 2, FALSE),
    (47, 1, NULL, 20, NULL, 'ODBC DSNs', 'OS.ODBC', 6.0, 2, FALSE),
    (59, 2, NULL, 25, NULL, 'Shared SSL Folders', 'Web.SharedSSL', 8.0, 2, FALSE),
    (62, 10, NULL, 21, NULL, 'Databases', 'MsSQL2005.Databases', 1.0, 2, FALSE),
    (63, 10, NULL, 22, NULL, 'Users', 'MsSQL2005.Users', 2.0, 2, FALSE),
    (68, 11, NULL, 23, NULL, 'Databases', 'MySQL5.Databases', 1.0, 2, FALSE),
    (69, 11, NULL, 24, NULL, 'Users', 'MySQL5.Users', 2.0, 2, FALSE),
    (110, 90, NULL, 75, NULL, 'Databases', 'MySQL8.Databases', 1.0, 2, FALSE),
    (111, 90, NULL, 76, NULL, 'Users', 'MySQL8.Users', 2.0, 2, FALSE),
    (120, 91, NULL, 75, NULL, 'Databases', 'MySQL9.Databases', 1.0, 2, FALSE),
    (121, 91, NULL, 76, NULL, 'Users', 'MySQL9.Users', 2.0, 2, FALSE),
    (200, 20, NULL, 200, 1, 'SharePoint Site Collections', 'HostedSharePoint.Sites', 1.0, 2, FALSE),
    (205, 13, NULL, 29, NULL, 'Organizations', 'HostedSolution.Organizations', 1.0, 2, FALSE),
    (206, 13, NULL, 30, 1, 'Users', 'HostedSolution.Users', 2.0, 2, FALSE),
    (211, 22, NULL, 31, NULL, 'Databases', 'MsSQL2008.Databases', 1.0, 2, FALSE),
    (212, 22, NULL, 32, NULL, 'Users', 'MsSQL2008.Users', 2.0, 2, FALSE),
    (218, 23, NULL, 37, NULL, 'Databases', 'MsSQL2012.Databases', 1.0, 2, FALSE),
    (219, 23, NULL, 38, NULL, 'Users', 'MsSQL2012.Users', 2.0, 2, FALSE),
    (300, 30, NULL, 33, NULL, 'Number of VPS', 'VPS.ServersNumber', 1.0, 2, FALSE),
    (345, 40, NULL, 35, NULL, 'Number of VPS', 'VPSForPC.ServersNumber', 1.0, 2, FALSE),
    (470, 46, NULL, 39, NULL, 'Databases', 'MsSQL2014.Databases', 1.0, 2, FALSE),
    (471, 46, NULL, 40, NULL, 'Users', 'MsSQL2014.Users', 2.0, 2, FALSE),
    (550, 73, NULL, 204, 1, 'SharePoint Site Collections', 'HostedSharePointEnterprise.Sites', 1.0, 2, FALSE),
    (553, 33, NULL, 41, NULL, 'Number of VPS', 'VPS2012.ServersNumber', 1.0, 2, FALSE),
    (573, 50, NULL, 202, NULL, 'Databases', 'MariaDB.Databases', 1.0, 2, FALSE),
    (574, 50, NULL, 203, NULL, 'Users', 'MariaDB.Users', 2.0, 2, FALSE),
    (673, 167, NULL, 41, NULL, 'Number of VPS', 'PROXMOX.ServersNumber', 1.0, 2, FALSE),
    (701, 71, NULL, 71, NULL, 'Databases', 'MsSQL2016.Databases', 1.0, 2, FALSE),
    (702, 71, NULL, 72, NULL, 'Users', 'MsSQL2016.Users', 2.0, 2, FALSE),
    (711, 72, NULL, 73, NULL, 'Databases', 'MsSQL2017.Databases', 1.0, 2, FALSE),
    (712, 72, NULL, 74, NULL, 'Users', 'MsSQL2017.Users', 2.0, 2, FALSE),
    (721, 74, NULL, 77, NULL, 'Databases', 'MsSQL2019.Databases', 1.0, 2, FALSE);
    INSERT INTO `Quotas` (`QuotaID`, `GroupID`, `HideQuota`, `ItemTypeID`, `PerOrganization`, `QuotaDescription`, `QuotaName`, `QuotaOrder`, `QuotaTypeID`, `ServiceQuota`)
    VALUES (722, 74, NULL, 78, NULL, 'Users', 'MsSQL2019.Users', 2.0, 2, FALSE),
    (732, 75, NULL, 79, NULL, 'Databases', 'MsSQL2022.Databases', 1.0, 2, FALSE),
    (733, 75, NULL, 80, NULL, 'Users', 'MsSQL2022.Users', 2.0, 2, FALSE),
    (760, 76, NULL, 79, NULL, 'Databases', 'MsSQL2025.Databases', 1.0, 2, FALSE),
    (761, 76, NULL, 80, NULL, 'Users', 'MsSQL2025.Users', 2.0, 2, FALSE),
    (770, 4, NULL, 11, NULL, 'Mail Domains', 'Mail.Domains', 1.1000000000000001, 2, TRUE);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `Schedule` (`ScheduleID`, `Enabled`, `FromTime`, `HistoriesNumber`, `Interval`, `LastRun`, `MaxExecutionTime`, `NextRun`, `PackageID`, `PriorityID`, `ScheduleName`, `ScheduleTypeID`, `StartTime`, `TaskID`, `ToTime`, `WeekMonthDay`)
    VALUES (1, TRUE, TIMESTAMP '2000-01-01 12:00:00', 7, 0, NULL, 3600, TIMESTAMP '2010-07-16 14:53:02', 1, 'Normal', 'Calculate Disk Space', 'Daily', TIMESTAMP '2000-01-01 12:30:00', 'SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE', TIMESTAMP '2000-01-01 12:00:00', 1),
    (2, TRUE, TIMESTAMP '2000-01-01 12:00:00', 7, 0, NULL, 3600, TIMESTAMP '2010-07-16 14:53:02', 1, 'Normal', 'Calculate Bandwidth', 'Daily', TIMESTAMP '2000-01-01 12:00:00', 'SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH', TIMESTAMP '2000-01-01 12:00:00', 1);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('UsersHome', 1, '%SYSTEMDRIVE%\\HostingSpaces'),
    ('AspNet11Path', 2, '%SYSTEMROOT%\\Microsoft.NET\\Framework\\v1.1.4322\\aspnet_isapi.dll'),
    ('AspNet11Pool', 2, 'ASP.NET V1.1'),
    ('AspNet20Path', 2, '%SYSTEMROOT%\\Microsoft.NET\\Framework\\v2.0.50727\\aspnet_isapi.dll'),
    ('AspNet20Pool', 2, 'ASP.NET V2.0'),
    ('AspNet40Path', 2, '%SYSTEMROOT%\\Microsoft.NET\\Framework\\v4.0.30319\\aspnet_isapi.dll'),
    ('AspNet40Pool', 2, 'ASP.NET V4.0'),
    ('AspPath', 2, '%SYSTEMROOT%\\System32\\InetSrv\\asp.dll'),
    ('CFFlashRemotingDirectory', 2, 'C:\\ColdFusion9\\runtime\\lib\\wsconfig\\1'),
    ('CFScriptsDirectory', 2, 'C:\\Inetpub\\wwwroot\\CFIDE'),
    ('ColdFusionPath', 2, 'C:\\ColdFusion9\\runtime\\lib\\wsconfig\\jrun_iis6.dll'),
    ('GalleryXmlFeedUrl', 2, ''),
    ('PerlPath', 2, '%SYSTEMDRIVE%\\Perl\\bin\\Perl.exe'),
    ('Php4Path', 2, '%PROGRAMFILES%\\PHP\\php.exe'),
    ('Php5Path', 2, '%PROGRAMFILES%\\PHP\\php-cgi.exe'),
    ('ProtectedAccessFile', 2, '.htaccess'),
    ('ProtectedFoldersFile', 2, '.htfolders'),
    ('ProtectedGroupsFile', 2, '.htgroup'),
    ('ProtectedUsersFile', 2, '.htpasswd'),
    ('PythonPath', 2, '%SYSTEMDRIVE%\\Python\\python.exe'),
    ('SecuredFoldersFilterPath', 2, '%SYSTEMROOT%\\System32\\InetSrv\\IISPasswordFilter.dll'),
    ('WebGroupName', 2, 'FCPWebUsers'),
    ('FtpGroupName', 3, 'FCPFtpUsers'),
    ('SiteId', 3, 'MSFTPSVC/1'),
    ('DatabaseLocation', 5, '%SYSTEMDRIVE%\\SQL2000Databases\\[USER_NAME]'),
    ('ExternalAddress', 5, '(local)'),
    ('InternalAddress', 5, '(local)'),
    ('SaLogin', 5, 'sa'),
    ('SaPassword', 5, ''),
    ('UseDefaultDatabaseLocation', 5, 'True'),
    ('UseTrustedConnection', 5, 'True'),
    ('ExternalAddress', 6, 'localhost'),
    ('InstallFolder', 6, '%PROGRAMFILES%\\MySQL\\MySQL Server 4.1'),
    ('InternalAddress', 6, 'localhost,3306'),
    ('RootLogin', 6, 'root'),
    ('RootPassword', 6, ''),
    ('ExpireLimit', 7, '1209600'),
    ('MinimumTTL', 7, '86400'),
    ('NameServers', 7, 'ns1.yourdomain.com;ns2.yourdomain.com'),
    ('RecordDefaultTTL', 7, '86400'),
    ('RecordMinimumTTL', 7, '3600'),
    ('RefreshInterval', 7, '3600');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('ResponsiblePerson', 7, 'hostmaster.[DOMAIN_NAME]'),
    ('RetryDelay', 7, '600'),
    ('AwStatsFolder', 8, '%SYSTEMDRIVE%\\AWStats\\wwwroot\\cgi-bin'),
    ('BatchFileName', 8, 'UpdateStats.bat'),
    ('BatchLineTemplate', 8, '%SYSTEMDRIVE%\\perl\\bin\\perl.exe awstats.pl config=[DOMAIN_NAME] -update'),
    ('ConfigFileName', 8, 'awstats.[DOMAIN_NAME].conf'),
    ('ConfigFileTemplate', 8, CONCAT('LogFormat = "%time2 %other %other %other %method %url %other %other %logname %host %other %ua %other %referer %other %code %other %other %bytesd %other %other"', CHAR(13, 10), 'LogSeparator = " "', CHAR(13, 10), 'DNSLookup = 2', CHAR(13, 10), 'DirCgi = "/cgi-bin"', CHAR(13, 10), 'DirIcons = "/icon"', CHAR(13, 10), 'AllowFullYearView=3', CHAR(13, 10), 'AllowToUpdateStatsFromBrowser = 0', CHAR(13, 10), 'UseFramesWhenCGI = 1', CHAR(13, 10), 'ShowFlagLinks = "en fr de it nl es"', CHAR(13, 10), 'LogFile = "[LOGS_FOLDER]\\ex%YY-3%MM-3%DD-3.log"', CHAR(13, 10), 'DirData = "%SYSTEMDRIVE%\\AWStats\\data"', CHAR(13, 10), 'SiteDomain = "[DOMAIN_NAME]"', CHAR(13, 10), 'HostAliases = [DOMAIN_ALIASES]')),
    ('StatisticsURL', 8, 'http://127.0.0.1/AWStats/cgi-bin/awstats.pl?config=[domain_name]'),
    ('AdminLogin', 9, 'Admin'),
    ('ExpireLimit', 9, '1209600'),
    ('MinimumTTL', 9, '86400'),
    ('NameServers', 9, 'ns1.yourdomain.com;ns2.yourdomain.com'),
    ('RecordDefaultTTL', 9, '86400'),
    ('RecordMinimumTTL', 9, '3600'),
    ('RefreshInterval', 9, '3600'),
    ('ResponsiblePerson', 9, 'hostmaster.[DOMAIN_NAME]'),
    ('RetryDelay', 9, '600'),
    ('SimpleDnsUrl', 9, 'http://127.0.0.1:8053'),
    ('LogDeleteDays', 10, '0'),
    ('LogFormat', 10, 'W3Cex'),
    ('LogWildcard', 10, '*.log'),
    ('Password', 10, ''),
    ('ServerID', 10, '1'),
    ('SmarterLogDeleteMonths', 10, '0'),
    ('SmarterLogsPath', 10, '%SYSTEMDRIVE%\\SmarterLogs'),
    ('SmarterUrl', 10, 'http://127.0.0.1:9999/services'),
    ('StatisticsURL', 10, 'http://127.0.0.1:9999/Login.aspx?txtSiteID=[site_id]&txtUser=[username]&txtPass=[password]&shortcutLink=autologin'),
    ('TimeZoneId', 10, '27'),
    ('Username', 10, 'Admin'),
    ('AdminPassword', 11, ''),
    ('AdminUsername', 11, 'admin'),
    ('defaultdomainhostname', 11, 'mail.[DOMAIN_NAME]'),
    ('DomainsPath', 11, '%SYSTEMDRIVE%\\SmarterMail'),
    ('ServerIPAddress', 11, '127.0.0.1;127.0.0.1'),
    ('ServiceUrl', 11, 'http://127.0.0.1:9998/services'),
    ('InstallFolder', 12, '%PROGRAMFILES%\\Gene6 FTP Server'),
    ('LogsFolder', 12, '%PROGRAMFILES%\\Gene6 FTP Server\\Log'),
    ('AdminPassword', 14, ''),
    ('AdminUsername', 14, 'admin'),
    ('defaultdomainhostname', 14, 'mail.[DOMAIN_NAME]'),
    ('DomainsPath', 14, '%SYSTEMDRIVE%\\SmarterMail'),
    ('ServerIPAddress', 14, '127.0.0.1;127.0.0.1');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('ServiceUrl', 14, 'http://127.0.0.1:9998/services'),
    ('BrowseMethod', 16, 'POST'),
    ('BrowseParameters', 16, CONCAT('ServerName=[SERVER]', CHAR(13, 10), 'Login=[USER]', CHAR(13, 10), 'Password=[PASSWORD]', CHAR(13, 10), 'Protocol=dbmssocn')),
    ('BrowseURL', 16, 'http://localhost/MLA/silentlogon.aspx'),
    ('DatabaseLocation', 16, '%SYSTEMDRIVE%\\SQL2005Databases\\[USER_NAME]'),
    ('ExternalAddress', 16, '(local)'),
    ('InternalAddress', 16, '(local)'),
    ('SaLogin', 16, 'sa'),
    ('SaPassword', 16, ''),
    ('UseDefaultDatabaseLocation', 16, 'True'),
    ('UseTrustedConnection', 16, 'True'),
    ('ExternalAddress', 17, 'localhost'),
    ('InstallFolder', 17, '%PROGRAMFILES%\\MySQL\\MySQL Server 5.0'),
    ('InternalAddress', 17, 'localhost,3306'),
    ('RootLogin', 17, 'root'),
    ('RootPassword', 17, ''),
    ('AdminPassword', 22, ''),
    ('AdminUsername', 22, 'Administrator'),
    ('DomainId', 25, '1'),
    ('KeepDeletedItemsDays', 27, '14'),
    ('KeepDeletedMailboxesDays', 27, '30'),
    ('MailboxDatabase', 27, 'Hosted Exchange Database'),
    ('RootOU', 27, 'FCP Hosting'),
    ('StorageGroup', 27, 'Hosted Exchange Storage Group'),
    ('TempDomain', 27, 'my-temp-domain.com'),
    ('AdminLogin', 28, 'Admin'),
    ('ExpireLimit', 28, '1209600'),
    ('MinimumTTL', 28, '86400'),
    ('NameServers', 28, 'ns1.yourdomain.com;ns2.yourdomain.com'),
    ('RecordDefaultTTL', 28, '86400'),
    ('RecordMinimumTTL', 28, '3600'),
    ('RefreshInterval', 28, '3600'),
    ('ResponsiblePerson', 28, 'hostmaster.[DOMAIN_NAME]'),
    ('RetryDelay', 28, '600'),
    ('SimpleDnsUrl', 28, 'http://127.0.0.1:8053'),
    ('AdminPassword', 29, ' '),
    ('AdminUsername', 29, 'admin'),
    ('defaultdomainhostname', 29, 'mail.[DOMAIN_NAME]'),
    ('DomainsPath', 29, '%SYSTEMDRIVE%\\SmarterMail'),
    ('ServerIPAddress', 29, '127.0.0.1;127.0.0.1'),
    ('ServiceUrl', 29, 'http://localhost:9998/services/'),
    ('ExternalAddress', 30, 'localhost');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('InstallFolder', 30, '%PROGRAMFILES%\\MySQL\\MySQL Server 5.1'),
    ('InternalAddress', 30, 'localhost,3306'),
    ('RootLogin', 30, 'root'),
    ('RootPassword', 30, ''),
    ('LogDeleteDays', 31, '0'),
    ('LogFormat', 31, 'W3Cex'),
    ('LogWildcard', 31, '*.log'),
    ('Password', 31, ''),
    ('ServerID', 31, '1'),
    ('SmarterLogDeleteMonths', 31, '0'),
    ('SmarterLogsPath', 31, '%SYSTEMDRIVE%\\SmarterLogs'),
    ('SmarterUrl', 31, 'http://127.0.0.1:9999/services'),
    ('StatisticsURL', 31, 'http://127.0.0.1:9999/Login.aspx?txtSiteID=[site_id]&txtUser=[username]&txtPass=[password]&shortcutLink=autologin'),
    ('TimeZoneId', 31, '27'),
    ('Username', 31, 'Admin'),
    ('KeepDeletedItemsDays', 32, '14'),
    ('KeepDeletedMailboxesDays', 32, '30'),
    ('MailboxDatabase', 32, 'Hosted Exchange Database'),
    ('RootOU', 32, 'FCP Hosting'),
    ('TempDomain', 32, 'my-temp-domain.com'),
    ('RecordDefaultTTL', 55, '86400'),
    ('RecordMinimumTTL', 55, '3600'),
    ('ExpireLimit', 56, '1209600'),
    ('MinimumTTL', 56, '86400'),
    ('NameServers', 56, 'ns1.yourdomain.com;ns2.yourdomain.com'),
    ('PDNSDbName', 56, 'pdnsdb'),
    ('PDNSDbPort', 56, '3306'),
    ('PDNSDbServer', 56, 'localhost'),
    ('PDNSDbUser', 56, 'root'),
    ('RecordDefaultTTL', 56, '86400'),
    ('RecordMinimumTTL', 56, '3600'),
    ('RefreshInterval', 56, '3600'),
    ('ResponsiblePerson', 56, 'hostmaster.[DOMAIN_NAME]'),
    ('RetryDelay', 56, '600'),
    ('AdminPassword', 60, ' '),
    ('AdminUsername', 60, 'admin'),
    ('defaultdomainhostname', 60, 'mail.[DOMAIN_NAME]'),
    ('DomainsPath', 60, '%SYSTEMDRIVE%\\SmarterMail'),
    ('ServerIPAddress', 60, '127.0.0.1;127.0.0.1'),
    ('ServiceUrl', 60, 'http://localhost:9998/services/'),
    ('LogDeleteDays', 62, '0'),
    ('LogFormat', 62, 'W3Cex');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('LogWildcard', 62, '*.log'),
    ('Password', 62, ''),
    ('ServerID', 62, '1'),
    ('SmarterLogDeleteMonths', 62, '0'),
    ('SmarterLogsPath', 62, '%SYSTEMDRIVE%\\SmarterLogs'),
    ('SmarterUrl', 62, 'http://127.0.0.1:9999/services'),
    ('StatisticsURL', 62, 'http://127.0.0.1:9999/Login.aspx?txtSiteID=[site_id]&txtUser=[username]&txtPass=[password]&shortcutLink=autologin'),
    ('TimeZoneId', 62, '27'),
    ('Username', 62, 'Admin'),
    ('AdminPassword', 63, ''),
    ('AdminUsername', 63, 'Administrator'),
    ('AdminPassword', 64, ''),
    ('AdminUsername', 64, 'admin'),
    ('defaultdomainhostname', 64, 'mail.[DOMAIN_NAME]'),
    ('DomainsPath', 64, '%SYSTEMDRIVE%\\SmarterMail'),
    ('ServerIPAddress', 64, '127.0.0.1;127.0.0.1'),
    ('ServiceUrl', 64, 'http://localhost:9998/services/'),
    ('AdminPassword', 65, ''),
    ('AdminUsername', 65, 'admin'),
    ('defaultdomainhostname', 65, 'mail.[DOMAIN_NAME]'),
    ('DomainsPath', 65, '%SYSTEMDRIVE%\\SmarterMail'),
    ('ServerIPAddress', 65, '127.0.0.1;127.0.0.1'),
    ('ServiceUrl', 65, 'http://localhost:9998/services/'),
    ('AdminPassword', 66, ''),
    ('AdminUsername', 66, 'admin'),
    ('defaultdomainhostname', 66, 'mail.[DOMAIN_NAME]'),
    ('DomainsPath', 66, '%SYSTEMDRIVE%\\SmarterMail'),
    ('ServerIPAddress', 66, '127.0.0.1;127.0.0.1'),
    ('ServiceUrl', 66, 'http://localhost:9998/services/'),
    ('AdminPassword', 67, ''),
    ('AdminUsername', 67, 'admin'),
    ('defaultdomainhostname', 67, 'mail.[DOMAIN_NAME]'),
    ('DomainsPath', 67, '%SYSTEMDRIVE%\\SmarterMail\\Domains'),
    ('ServerIPAddress', 67, '127.0.0.1;127.0.0.1'),
    ('ServiceUrl', 67, 'http://localhost:9998'),
    ('UsersHome', 100, '%SYSTEMDRIVE%\\HostingSpaces'),
    ('AspNet11Pool', 101, 'ASP.NET 1.1'),
    ('AspNet40Path', 101, '%WINDIR%\\Microsoft.NET\\Framework\\v4.0.30319\\aspnet_isapi.dll'),
    ('AspNet40x64Path', 101, '%WINDIR%\\Microsoft.NET\\Framework64\\v4.0.30319\\aspnet_isapi.dll'),
    ('AspNetBitnessMode', 101, '32'),
    ('CFFlashRemotingDirectory', 101, 'C:\\ColdFusion9\\runtime\\lib\\wsconfig\\1'),
    ('CFScriptsDirectory', 101, 'C:\\Inetpub\\wwwroot\\CFIDE');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('ClassicAspNet20Pool', 101, 'ASP.NET 2.0 (Classic)'),
    ('ClassicAspNet40Pool', 101, 'ASP.NET 4.0 (Classic)'),
    ('ColdFusionPath', 101, 'C:\\ColdFusion9\\runtime\\lib\\wsconfig\\jrun_iis6.dll'),
    ('GalleryXmlFeedUrl', 101, ''),
    ('IntegratedAspNet20Pool', 101, 'ASP.NET 2.0 (Integrated)'),
    ('IntegratedAspNet40Pool', 101, 'ASP.NET 4.0 (Integrated)'),
    ('PerlPath', 101, '%SYSTEMDRIVE%\\Perl\\bin\\PerlEx30.dll'),
    ('Php4Path', 101, '%PROGRAMFILES(x86)%\\PHP\\php.exe'),
    ('PhpMode', 101, 'FastCGI'),
    ('PhpPath', 101, '%PROGRAMFILES(x86)%\\PHP\\php-cgi.exe'),
    ('ProtectedGroupsFile', 101, '.htgroup'),
    ('ProtectedUsersFile', 101, '.htpasswd'),
    ('SecureFoldersModuleAssembly', 101, 'FuseCP.IIsModules.SecureFolders, FuseCP.IIsModules, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=37f9c58a0aa32ff0'),
    ('WebGroupName', 101, 'FCP_IUSRS'),
    ('WmSvc.CredentialsMode', 101, 'WINDOWS'),
    ('WmSvc.Port', 101, '8172'),
    ('FtpGroupName', 102, 'FCPFtpUsers'),
    ('SiteId', 102, 'Default FTP Site'),
    ('UsersHome', 104, '%SYSTEMDRIVE%\\HostingSpaces'),
    ('AspNet11Pool', 105, 'ASP.NET 1.1'),
    ('AspNet40Path', 105, '%WINDIR%\\Microsoft.NET\\Framework\\v4.0.30319\\aspnet_isapi.dll'),
    ('AspNet40x64Path', 105, '%WINDIR%\\Microsoft.NET\\Framework64\\v4.0.30319\\aspnet_isapi.dll'),
    ('AspNetBitnessMode', 105, '32'),
    ('CFFlashRemotingDirectory', 105, 'C:\\ColdFusion9\\runtime\\lib\\wsconfig\\1'),
    ('CFScriptsDirectory', 105, 'C:\\Inetpub\\wwwroot\\CFIDE'),
    ('ClassicAspNet20Pool', 105, 'ASP.NET 2.0 (Classic)'),
    ('ClassicAspNet40Pool', 105, 'ASP.NET 4.0 (Classic)'),
    ('ColdFusionPath', 105, 'C:\\ColdFusion9\\runtime\\lib\\wsconfig\\jrun_iis6.dll'),
    ('GalleryXmlFeedUrl', 105, ''),
    ('IntegratedAspNet20Pool', 105, 'ASP.NET 2.0 (Integrated)'),
    ('IntegratedAspNet40Pool', 105, 'ASP.NET 4.0 (Integrated)'),
    ('PerlPath', 105, '%SYSTEMDRIVE%\\Perl\\bin\\PerlEx30.dll'),
    ('Php4Path', 105, '%PROGRAMFILES(x86)%\\PHP\\php.exe'),
    ('PhpMode', 105, 'FastCGI'),
    ('PhpPath', 105, '%PROGRAMFILES(x86)%\\PHP\\php-cgi.exe'),
    ('ProtectedGroupsFile', 105, '.htgroup'),
    ('ProtectedUsersFile', 105, '.htpasswd'),
    ('SecureFoldersModuleAssembly', 105, 'FuseCP.IIsModules.SecureFolders, FuseCP.IIsModules, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=37f9c58a0aa32ff0'),
    ('sslusesni', 105, 'True'),
    ('WebGroupName', 105, 'FCP_IUSRS'),
    ('WmSvc.CredentialsMode', 105, 'WINDOWS'),
    ('WmSvc.Port', 105, '8172');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('FtpGroupName', 106, 'FCPFtpUsers'),
    ('SiteId', 106, 'Default FTP Site'),
    ('sslusesni', 106, 'False'),
    ('UsersHome', 111, '%SYSTEMDRIVE%\\HostingSpaces'),
    ('AspNet11Pool', 112, 'ASP.NET 1.1'),
    ('AspNet40Path', 112, '%WINDIR%\\Microsoft.NET\\Framework\\v4.0.30319\\aspnet_isapi.dll'),
    ('AspNet40x64Path', 112, '%WINDIR%\\Microsoft.NET\\Framework64\\v4.0.30319\\aspnet_isapi.dll'),
    ('AspNetBitnessMode', 112, '32'),
    ('CFFlashRemotingDirectory', 112, 'C:\\ColdFusion9\\runtime\\lib\\wsconfig\\1'),
    ('CFScriptsDirectory', 112, 'C:\\Inetpub\\wwwroot\\CFIDE'),
    ('ClassicAspNet20Pool', 112, 'ASP.NET 2.0 (Classic)'),
    ('ClassicAspNet40Pool', 112, 'ASP.NET 4.0 (Classic)'),
    ('ColdFusionPath', 112, 'C:\\ColdFusion9\\runtime\\lib\\wsconfig\\jrun_iis6.dll'),
    ('GalleryXmlFeedUrl', 112, ''),
    ('IntegratedAspNet20Pool', 112, 'ASP.NET 2.0 (Integrated)'),
    ('IntegratedAspNet40Pool', 112, 'ASP.NET 4.0 (Integrated)'),
    ('PerlPath', 112, '%SYSTEMDRIVE%\\Perl\\bin\\PerlEx30.dll'),
    ('Php4Path', 112, '%PROGRAMFILES(x86)%\\PHP\\php.exe'),
    ('PhpMode', 112, 'FastCGI'),
    ('PhpPath', 112, '%PROGRAMFILES(x86)%\\PHP\\php-cgi.exe'),
    ('ProtectedGroupsFile', 112, '.htgroup'),
    ('ProtectedUsersFile', 112, '.htpasswd'),
    ('SecureFoldersModuleAssembly', 112, 'FuseCP.IIsModules.SecureFolders, FuseCP.IIsModules, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=37f9c58a0aa32ff0'),
    ('sslusesni', 112, 'True'),
    ('WebGroupName', 112, 'FCP_IUSRS'),
    ('WmSvc.CredentialsMode', 112, 'WINDOWS'),
    ('WmSvc.Port', 112, '8172'),
    ('FtpGroupName', 113, 'FCPFtpUsers'),
    ('SiteId', 113, 'Default FTP Site'),
    ('sslusesni', 113, 'False'),
    ('RootWebApplicationIpAddress', 200, ''),
    ('UserName', 204, 'admin'),
    ('UtilityPath', 204, 'C:\\Program Files\\Research In Motion\\BlackBerry Enterprise Server Resource Kit\\BlackBerry Enterprise Server User Administration Tool'),
    ('CpuLimit', 300, '100'),
    ('CpuReserve', 300, '0'),
    ('CpuWeight', 300, '100'),
    ('DvdLibraryPath', 300, 'C:\\Hyper-V\\Library'),
    ('ExportedVpsPath', 300, 'C:\\Hyper-V\\Exported'),
    ('HostnamePattern', 300, 'vps[user_id].hosterdomain.com'),
    ('OsTemplatesPath', 300, 'C:\\Hyper-V\\Templates'),
    ('PrivateNetworkFormat', 300, '192.168.0.1/16'),
    ('RootFolder', 300, 'C:\\Hyper-V\\VirtualMachines\\[VPS_HOSTNAME]');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('StartAction', 300, 'start'),
    ('StartupDelay', 300, '0'),
    ('StopAction', 300, 'shutDown'),
    ('VirtualDiskType', 300, 'dynamic'),
    ('ExternalAddress', 301, 'localhost'),
    ('InstallFolder', 301, '%PROGRAMFILES%\\MySQL\\MySQL Server 5.5'),
    ('InternalAddress', 301, 'localhost,3306'),
    ('RootLogin', 301, 'root'),
    ('RootPassword', 301, ''),
    ('ExternalAddress', 304, 'localhost'),
    ('InstallFolder', 304, '%PROGRAMFILES%\\MySQL\\MySQL Server 8.0'),
    ('InternalAddress', 304, 'localhost,3306'),
    ('RootLogin', 304, 'root'),
    ('RootPassword', 304, ''),
    ('sslmode', 304, 'True'),
    ('ExternalAddress', 305, 'localhost'),
    ('InstallFolder', 305, '%PROGRAMFILES%\\MySQL\\MySQL Server 8.0'),
    ('InternalAddress', 305, 'localhost,3306'),
    ('RootLogin', 305, 'root'),
    ('RootPassword', 305, ''),
    ('sslmode', 305, 'True'),
    ('ExternalAddress', 306, 'localhost'),
    ('InstallFolder', 306, '%PROGRAMFILES%\\MySQL\\MySQL Server 8.0'),
    ('InternalAddress', 306, 'localhost,3306'),
    ('RootLogin', 306, 'root'),
    ('RootPassword', 306, ''),
    ('sslmode', 306, 'True'),
    ('ExternalAddress', 307, 'localhost'),
    ('InstallFolder', 307, '%PROGRAMFILES%\\MySQL\\MySQL Server 8.0'),
    ('InternalAddress', 307, 'localhost,3306'),
    ('RootLogin', 307, 'root'),
    ('RootPassword', 307, ''),
    ('sslmode', 307, 'True'),
    ('ExternalAddress', 308, 'localhost'),
    ('InstallFolder', 308, '%PROGRAMFILES%\\MySQL\\MySQL Server 8.0'),
    ('InternalAddress', 308, 'localhost,3306'),
    ('RootLogin', 308, 'root'),
    ('RootPassword', 308, ''),
    ('sslmode', 308, 'True'),
    ('ExternalAddress', 320, 'localhost'),
    ('InstallFolder', 320, '%PROGRAMFILES%\\MySQL\\MySQL Server 9.0'),
    ('InternalAddress', 320, 'localhost,3306');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('RootLogin', 320, 'root'),
    ('RootPassword', 320, ''),
    ('sslmode', 320, 'True'),
    ('admode', 410, 'False'),
    ('expirelimit', 410, '1209600'),
    ('minimumttl', 410, '86400'),
    ('nameservers', 410, 'ns1.yourdomain.com;ns2.yourdomain.com'),
    ('RecordDefaultTTL', 410, '86400'),
    ('RecordMinimumTTL', 410, '3600'),
    ('refreshinterval', 410, '3600'),
    ('responsibleperson', 410, 'hostmaster.[DOMAIN_NAME]'),
    ('retrydelay', 410, '600'),
    ('LogDir', 500, '/var/log'),
    ('UsersHome', 500, '/var/www/HostingSpaces'),
    ('ExternalAddress', 1550, 'localhost'),
    ('InstallFolder', 1550, '%PROGRAMFILES%\\MariaDB 10.1'),
    ('InternalAddress', 1550, 'localhost'),
    ('RootLogin', 1550, 'root'),
    ('RootPassword', 1550, ''),
    ('ExternalAddress', 1570, 'localhost'),
    ('InstallFolder', 1570, '%PROGRAMFILES%\\MariaDB 10.3'),
    ('InternalAddress', 1570, 'localhost'),
    ('RootLogin', 1570, 'root'),
    ('RootPassword', 1570, ''),
    ('ExternalAddress', 1571, 'localhost'),
    ('InstallFolder', 1571, '%PROGRAMFILES%\\MariaDB 10.4'),
    ('InternalAddress', 1571, 'localhost'),
    ('RootLogin', 1571, 'root'),
    ('RootPassword', 1571, ''),
    ('ExternalAddress', 1572, 'localhost'),
    ('InstallFolder', 1572, '%PROGRAMFILES%\\MariaDB 10.5'),
    ('InternalAddress', 1572, 'localhost'),
    ('RootLogin', 1572, 'root'),
    ('RootPassword', 1572, ''),
    ('ExternalAddress', 1573, 'localhost'),
    ('InstallFolder', 1573, '%PROGRAMFILES%\\MariaDB 10.6'),
    ('InternalAddress', 1573, 'localhost'),
    ('RootLogin', 1573, 'root'),
    ('RootPassword', 1573, ''),
    ('ExternalAddress', 1574, 'localhost'),
    ('InstallFolder', 1574, '%PROGRAMFILES%\\MariaDB 10.7'),
    ('InternalAddress', 1574, 'localhost');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('RootLogin', 1574, 'root'),
    ('RootPassword', 1574, ''),
    ('ExternalAddress', 1575, 'localhost'),
    ('InstallFolder', 1575, '%PROGRAMFILES%\\MariaDB 10.8'),
    ('InternalAddress', 1575, 'localhost'),
    ('RootLogin', 1575, 'root'),
    ('RootPassword', 1575, ''),
    ('ExternalAddress', 1576, 'localhost'),
    ('InstallFolder', 1576, '%PROGRAMFILES%\\MariaDB 10.9'),
    ('InternalAddress', 1576, 'localhost'),
    ('RootLogin', 1576, 'root'),
    ('RootPassword', 1576, ''),
    ('ExternalAddress', 1577, 'localhost'),
    ('InstallFolder', 1577, '%PROGRAMFILES%\\MariaDB 10.10'),
    ('InternalAddress', 1577, 'localhost'),
    ('RootLogin', 1577, 'root'),
    ('RootPassword', 1577, ''),
    ('ExternalAddress', 1578, 'localhost'),
    ('InstallFolder', 1578, '%PROGRAMFILES%\\MariaDB 10.11'),
    ('InternalAddress', 1578, 'localhost'),
    ('RootLogin', 1578, 'root'),
    ('RootPassword', 1578, ''),
    ('ExternalAddress', 1579, 'localhost'),
    ('InstallFolder', 1579, '%PROGRAMFILES%\\MariaDB 11.0'),
    ('InternalAddress', 1579, 'localhost'),
    ('RootLogin', 1579, 'root'),
    ('RootPassword', 1579, ''),
    ('ExternalAddress', 1580, 'localhost'),
    ('InstallFolder', 1580, '%PROGRAMFILES%\\MariaDB 11.1'),
    ('InternalAddress', 1580, 'localhost'),
    ('RootLogin', 1580, 'root'),
    ('RootPassword', 1580, ''),
    ('ExternalAddress', 1581, 'localhost'),
    ('InstallFolder', 1581, '%PROGRAMFILES%\\MariaDB 11.2'),
    ('InternalAddress', 1581, 'localhost'),
    ('RootLogin', 1581, 'root'),
    ('RootPassword', 1581, ''),
    ('ExternalAddress', 1582, 'localhost'),
    ('InstallFolder', 1582, '%PROGRAMFILES%\\MariaDB 11.3'),
    ('InternalAddress', 1582, 'localhost'),
    ('RootLogin', 1582, 'root'),
    ('RootPassword', 1582, '');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('ExternalAddress', 1583, 'localhost'),
    ('InstallFolder', 1583, '%PROGRAMFILES%\\MariaDB 11.4'),
    ('InternalAddress', 1583, 'localhost'),
    ('RootLogin', 1583, 'root'),
    ('RootPassword', 1583, ''),
    ('ExternalAddress', 1584, 'localhost'),
    ('InstallFolder', 1584, '%PROGRAMFILES%\\MariaDB 11.5'),
    ('InternalAddress', 1584, 'localhost'),
    ('RootLogin', 1584, 'root'),
    ('RootPassword', 1584, ''),
    ('ExternalAddress', 1585, 'localhost'),
    ('InstallFolder', 1585, '%PROGRAMFILES%\\MariaDB 11.6'),
    ('InternalAddress', 1585, 'localhost'),
    ('RootLogin', 1585, 'root'),
    ('RootPassword', 1585, ''),
    ('ExternalAddress', 1586, 'localhost'),
    ('InstallFolder', 1586, '%PROGRAMFILES%\\MariaDB 11.7'),
    ('InternalAddress', 1586, 'localhost'),
    ('RootLogin', 1586, 'root'),
    ('RootPassword', 1586, ''),
    ('RecordDefaultTTL', 1703, '86400'),
    ('RecordMinimumTTL', 1703, '3600'),
    ('UsersHome', 1800, '%SYSTEMDRIVE%\\HostingSpaces'),
    ('UsersHome', 1802, '%SYSTEMDRIVE%\\HostingSpaces'),
    ('UsersHome', 1804, '%SYSTEMDRIVE%\\HostingSpaces'),
    ('AdminLogin', 1901, 'Admin'),
    ('ExpireLimit', 1901, '1209600'),
    ('MinimumTTL', 1901, '86400'),
    ('NameServers', 1901, 'ns1.yourdomain.com;ns2.yourdomain.com'),
    ('RecordDefaultTTL', 1901, '86400'),
    ('RecordMinimumTTL', 1901, '3600'),
    ('RefreshInterval', 1901, '3600'),
    ('ResponsiblePerson', 1901, 'hostmaster.[DOMAIN_NAME]'),
    ('RetryDelay', 1901, '600'),
    ('SimpleDnsUrl', 1901, 'http://127.0.0.1:8053'),
    ('admode', 1902, 'False'),
    ('expirelimit', 1902, '1209600'),
    ('minimumttl', 1902, '86400'),
    ('nameservers', 1902, 'ns1.yourdomain.com;ns2.yourdomain.com'),
    ('RecordDefaultTTL', 1902, '86400'),
    ('RecordMinimumTTL', 1902, '3600'),
    ('refreshinterval', 1902, '3600');
    INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
    VALUES ('responsibleperson', 1902, 'hostmaster.[DOMAIN_NAME]'),
    ('retrydelay', 1902, '600'),
    ('AdminLogin', 1903, 'Admin'),
    ('ExpireLimit', 1903, '1209600'),
    ('NameServers', 1903, 'ns1.yourdomain.com;ns2.yourdomain.com'),
    ('RecordDefaultTTL', 1903, '86400'),
    ('RecordMinimumTTL', 1903, '3600'),
    ('RefreshInterval', 1903, '3600'),
    ('ResponsiblePerson', 1903, 'hostmaster.[DOMAIN_NAME]'),
    ('RetryDelay', 1903, '600'),
    ('SimpleDnsUrl', 1903, 'http://127.0.0.1:8053'),
    ('ConfigFile', 1910, '/etc/vsftpd.conf'),
    ('ConfigFile', 1911, '/etc/apache2/apache2.conf'),
    ('ConfigPath', 1911, '/etc/apache2');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `ScheduleParameters` (`ParameterID`, `ScheduleID`, `ParameterValue`)
    VALUES ('SUSPEND_OVERUSED', 1, 'false'),
    ('SUSPEND_OVERUSED', 2, 'false');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `AccessTokensIdx_AccountID` ON `AccessTokens` (`AccountID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `BackgroundTaskLogsIdx_TaskID` ON `BackgroundTaskLogs` (`TaskID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `BackgroundTaskParametersIdx_TaskID` ON `BackgroundTaskParameters` (`TaskID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `BackgroundTaskStackIdx_TaskID` ON `BackgroundTaskStack` (`TaskID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `BlackBerryUsersIdx_AccountId` ON `BlackBerryUsers` (`AccountId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `CommentsIdx_UserID` ON `Comments` (`UserID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `CRMUsersIdx_AccountID` ON `CRMUsers` (`AccountID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `DmzIPAddressesIdx_ItemID` ON `DmzIPAddresses` (`ItemID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `DomainDnsRecordsIdx_DomainId` ON `DomainDnsRecords` (`DomainId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `DomainsIdx_MailDomainID` ON `Domains` (`MailDomainID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `DomainsIdx_PackageID` ON `Domains` (`PackageID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `DomainsIdx_WebSiteID` ON `Domains` (`WebSiteID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `DomainsIdx_ZoneItemID` ON `Domains` (`ZoneItemID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `EnterpriseFoldersIdx_StorageSpaceFolderId` ON `EnterpriseFolders` (`StorageSpaceFolderId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `EnterpriseFoldersOwaPermissionsIdx_AccountID` ON `EnterpriseFoldersOwaPermissions` (`AccountID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `EnterpriseFoldersOwaPermissionsIdx_FolderID` ON `EnterpriseFoldersOwaPermissions` (`FolderID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ExchangeAccountEmailAddressesIdx_AccountID` ON `ExchangeAccountEmailAddresses` (`AccountID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE UNIQUE INDEX `IX_ExchangeAccountEmailAddresses_UniqueEmail` ON `ExchangeAccountEmailAddresses` (`EmailAddress`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ExchangeAccountsIdx_ItemID` ON `ExchangeAccounts` (`ItemID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ExchangeAccountsIdx_MailboxPlanId` ON `ExchangeAccounts` (`MailboxPlanId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE UNIQUE INDEX `IX_ExchangeAccounts_UniqueAccountName` ON `ExchangeAccounts` (`AccountName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ExchangeMailboxPlansIdx_ItemID` ON `ExchangeMailboxPlans` (`ItemID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE UNIQUE INDEX `IX_ExchangeMailboxPlans` ON `ExchangeMailboxPlans` (`MailboxPlanId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ExchangeOrganizationDomainsIdx_ItemID` ON `ExchangeOrganizationDomains` (`ItemID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE UNIQUE INDEX `IX_ExchangeOrganizationDomains_UniqueDomain` ON `ExchangeOrganizationDomains` (`DomainID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE UNIQUE INDEX `IX_ExchangeOrganizations_UniqueOrg` ON `ExchangeOrganizations` (`OrganizationID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ExchangeOrganizationSettingsIdx_ItemId` ON `ExchangeOrganizationSettings` (`ItemId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ExchangeOrganizationSsFoldersIdx_ItemId` ON `ExchangeOrganizationSsFolders` (`ItemId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ExchangeOrganizationSsFoldersIdx_StorageSpaceFolderId` ON `ExchangeOrganizationSsFolders` (`StorageSpaceFolderId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `GlobalDnsRecordsIdx_IPAddressID` ON `GlobalDnsRecords` (`IPAddressID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `GlobalDnsRecordsIdx_PackageID` ON `GlobalDnsRecords` (`PackageID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `GlobalDnsRecordsIdx_ServerID` ON `GlobalDnsRecords` (`ServerID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `GlobalDnsRecordsIdx_ServiceID` ON `GlobalDnsRecords` (`ServiceID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IX_HostingPlanQuotas_QuotaID` ON `HostingPlanQuotas` (`QuotaID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IX_HostingPlanResources_GroupID` ON `HostingPlanResources` (`GroupID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `HostingPlansIdx_PackageID` ON `HostingPlans` (`PackageID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `HostingPlansIdx_ServerID` ON `HostingPlans` (`ServerID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `HostingPlansIdx_UserID` ON `HostingPlans` (`UserID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IPAddressesIdx_ServerID` ON `IPAddresses` (`ServerID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE UNIQUE INDEX `IX_LyncUserPlans` ON `LyncUserPlans` (`LyncUserPlanId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `LyncUserPlansIdx_ItemID` ON `LyncUserPlans` (`ItemID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `LyncUsersIdx_LyncUserPlanID` ON `LyncUsers` (`LyncUserPlanID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageAddonsIdx_PackageID` ON `PackageAddons` (`PackageID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageAddonsIdx_PlanID` ON `PackageAddons` (`PlanID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageIPAddressesIdx_AddressID` ON `PackageIPAddresses` (`AddressID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageIPAddressesIdx_ItemID` ON `PackageIPAddresses` (`ItemID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageIPAddressesIdx_PackageID` ON `PackageIPAddresses` (`PackageID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IX_PackageQuotas_QuotaID` ON `PackageQuotas` (`QuotaID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IX_PackageResources_GroupID` ON `PackageResources` (`GroupID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageIndex_ParentPackageID` ON `Packages` (`ParentPackageID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageIndex_PlanID` ON `Packages` (`PlanID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageIndex_ServerID` ON `Packages` (`ServerID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageIndex_UserID` ON `Packages` (`UserID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IX_PackagesBandwidth_GroupID` ON `PackagesBandwidth` (`GroupID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IX_PackagesDiskspace_GroupID` ON `PackagesDiskspace` (`GroupID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IX_PackageServices_ServiceID` ON `PackageServices` (`ServiceID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IX_PackagesTreeCache_PackageID` ON `PackagesTreeCache` (`PackageID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageVLANsIdx_PackageID` ON `PackageVLANs` (`PackageID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PackageVLANsIdx_VlanID` ON `PackageVLANs` (`VlanID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PrivateIPAddressesIdx_ItemID` ON `PrivateIPAddresses` (`ItemID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `PrivateNetworkVLANsIdx_ServerID` ON `PrivateNetworkVLANs` (`ServerID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ProvidersIdx_GroupID` ON `Providers` (`GroupID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `QuotasIdx_GroupID` ON `Quotas` (`GroupID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `QuotasIdx_ItemTypeID` ON `Quotas` (`ItemTypeID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `RDSCollectionSettingsIdx_RDSCollectionId` ON `RDSCollectionSettings` (`RDSCollectionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `RDSCollectionUsersIdx_AccountID` ON `RDSCollectionUsers` (`AccountID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `RDSCollectionUsersIdx_RDSCollectionId` ON `RDSCollectionUsers` (`RDSCollectionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `RDSMessagesIdx_RDSCollectionId` ON `RDSMessages` (`RDSCollectionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `RDSServersIdx_RDSCollectionId` ON `RDSServers` (`RDSCollectionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ResourceGroupDnsRecordsIdx_GroupID` ON `ResourceGroupDnsRecords` (`GroupID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ScheduleIdx_PackageID` ON `Schedule` (`PackageID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ScheduleIdx_TaskID` ON `Schedule` (`TaskID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IX_ScheduleTaskViewConfiguration_TaskID` ON `ScheduleTaskViewConfiguration` (`TaskID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ServersIdx_PrimaryGroupID` ON `Servers` (`PrimaryGroupID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ServiceItemsIdx_ItemTypeID` ON `ServiceItems` (`ItemTypeID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ServiceItemsIdx_PackageID` ON `ServiceItems` (`PackageID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ServiceItemsIdx_ServiceID` ON `ServiceItems` (`ServiceID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ServiceItemTypesIdx_GroupID` ON `ServiceItemTypes` (`GroupID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ServicesIdx_ClusterID` ON `Services` (`ClusterID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ServicesIdx_ProviderID` ON `Services` (`ProviderID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ServicesIdx_ServerID` ON `Services` (`ServerID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `StorageSpaceFoldersIdx_StorageSpaceId` ON `StorageSpaceFolders` (`StorageSpaceId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `StorageSpaceLevelResourceGroupsIdx_GroupId` ON `StorageSpaceLevelResourceGroups` (`GroupId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `StorageSpaceLevelResourceGroupsIdx_LevelId` ON `StorageSpaceLevelResourceGroups` (`LevelId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `StorageSpacesIdx_ServerId` ON `StorageSpaces` (`ServerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `StorageSpacesIdx_ServiceId` ON `StorageSpaces` (`ServiceId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `IX_TempIds_Created_Scope_Level` ON `TempIds` (`Created`, `Scope`, `Level`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `ThemeSettingsIdx_ThemeID` ON `ThemeSettings` (`ThemeID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE UNIQUE INDEX `IX_Users_Username` ON `Users` (`Username`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `UsersIdx_OwnerID` ON `Users` (`OwnerID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `VirtualGroupsIdx_GroupID` ON `VirtualGroups` (`GroupID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `VirtualGroupsIdx_ServerID` ON `VirtualGroups` (`ServerID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `VirtualServicesIdx_ServerID` ON `VirtualServices` (`ServerID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `VirtualServicesIdx_ServiceID` ON `VirtualServices` (`ServiceID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `WebDavAccessTokensIdx_AccountID` ON `WebDavAccessTokens` (`AccountID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    CREATE INDEX `WebDavPortalUsersSettingsIdx_AccountId` ON `WebDavPortalUsersSettings` (`AccountId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251105040706_v2.0.0') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251105040706_v2.0.0', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251113211812_RemovedOldWindowsProviders') THEN

    DELETE FROM `Quotas`
    WHERE `QuotaID` = 770;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251113211812_RemovedOldWindowsProviders') THEN

    DELETE FROM `Quotas`
    WHERE `QuotaID` = 771;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251113211812_RemovedOldWindowsProviders') THEN

    DELETE FROM `ServiceDefaultProperties`
    WHERE `PropertyName` = 'UsersHome' AND `ProviderID` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251113211812_RemovedOldWindowsProviders') THEN

    DELETE FROM `ServiceDefaultProperties`
    WHERE `PropertyName` = 'UsersHome' AND `ProviderID` = 100;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251113211812_RemovedOldWindowsProviders') THEN

    DELETE FROM `ServiceDefaultProperties`
    WHERE `PropertyName` = 'UsersHome' AND `ProviderID` = 104;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251113211812_RemovedOldWindowsProviders') THEN

    DELETE FROM `Providers`
    WHERE `ProviderID` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251113211812_RemovedOldWindowsProviders') THEN

    DELETE FROM `Providers`
    WHERE `ProviderID` = 100;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251113211812_RemovedOldWindowsProviders') THEN

    DELETE FROM `Providers`
    WHERE `ProviderID` = 104;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251113211812_RemovedOldWindowsProviders') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251113211812_RemovedOldWindowsProviders', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260318133000_AddBruteForceProtection') THEN
    CREATE TABLE `BruteForceLogs` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `IpAddress` varchar(45) CHARACTER SET utf8mb4 NOT NULL,
        `Username` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Layer` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `AttemptTime` datetime(6) NOT NULL,
        `Succeeded` tinyint(1) NOT NULL,
        `UserAgent` varchar(500) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_BruteForceLogs` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260318133000_AddBruteForceProtection') THEN
    CREATE TABLE `IpSecurityPolicies` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `IpRange` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `IsWhitelist` tinyint(1) NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `ExpiresDate` datetime(6) NULL,
        `Reason` varchar(500) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `SeverityLevel` int NOT NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_IpSecurityPolicies` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260318133000_AddBruteForceProtection') THEN
    CREATE INDEX `IX_BruteForceLogs_IpAddress_Layer_AttemptTime` ON `BruteForceLogs` (`IpAddress`, `Layer`, `AttemptTime`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260318133000_AddBruteForceProtection') THEN
    CREATE INDEX `IX_IpSecurityPolicies_IpRange_IsActive` ON `IpSecurityPolicies` (`IpRange`, `IsActive`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260318133000_AddBruteForceProtection') THEN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260318133000_AddBruteForceProtection', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260320130001_RemovedLegacyStorefrontArtifacts') THEN
    SET FOREIGN_KEY_CHECKS = 0;

    DROP TABLE IF EXISTS `ecTopLevelDomainsCycles`;
    DROP TABLE IF EXISTS `ecTopLevelDomains`;
    DROP TABLE IF EXISTS `ecTaxations`;
    DROP TABLE IF EXISTS `ecSystemTriggers`;
    DROP TABLE IF EXISTS `ecSvcsUsageLog`;
    DROP TABLE IF EXISTS `ecSupportedPlugins`;
    DROP TABLE IF EXISTS `ecSupportedPluginLog`;
    DROP TABLE IF EXISTS `ecStoreSettings`;
    DROP TABLE IF EXISTS `ecStoreDefaultSettings`;
    DROP TABLE IF EXISTS `ecServiceHandlersResponses`;
    DROP TABLE IF EXISTS `ecService`;
    DROP TABLE IF EXISTS `ecProductTypeControls`;
    DROP TABLE IF EXISTS `ecProductType`;
    DROP TABLE IF EXISTS `ecProductsHighlights`;
    DROP TABLE IF EXISTS `ecProductCategories`;
    DROP TABLE IF EXISTS `ecProduct`;
    DROP TABLE IF EXISTS `ecPluginsProperties`;
    DROP TABLE IF EXISTS `ecPaymentProfiles`;
    DROP TABLE IF EXISTS `ecPaymentMethods`;
    DROP TABLE IF EXISTS `ecInvoiceItems`;
    DROP TABLE IF EXISTS `ecInvoice`;
    DROP TABLE IF EXISTS `ecHostingPlansBillingCycles`;
    DROP TABLE IF EXISTS `ecHostingPlans`;
    DROP TABLE IF EXISTS `ecHostingPackageSvcsCycles`;
    DROP TABLE IF EXISTS `ecHostingPackageSvcs`;
    DROP TABLE IF EXISTS `ecHostingAddonSvcsCycles`;
    DROP TABLE IF EXISTS `ecHostingAddonSvcs`;
    DROP TABLE IF EXISTS `ecHostingAddonsCycles`;
    DROP TABLE IF EXISTS `ecHostingAddons`;
    DROP TABLE IF EXISTS `ecDomainSvcsCycles`;
    DROP TABLE IF EXISTS `ecDomainSvcs`;
    DROP TABLE IF EXISTS `ecCustomersPayments`;
    DROP TABLE IF EXISTS `ecContracts`;
    DROP TABLE IF EXISTS `ecCategory`;
    DROP TABLE IF EXISTS `ecBillingCycles`;
    DROP TABLE IF EXISTS `ecAddonProducts`;

    SET FOREIGN_KEY_CHECKS = 1;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260320130001_RemovedLegacyStorefrontArtifacts') THEN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260320130001_RemovedLegacyStorefrontArtifacts', '9.0.9');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;
