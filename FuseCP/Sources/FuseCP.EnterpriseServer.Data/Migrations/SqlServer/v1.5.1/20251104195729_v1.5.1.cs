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
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class v151 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdditionalGroups",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalGroup", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    RecordID = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ItemID = table.Column<int>(type: "int", nullable: true),
                    SeverityID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    FinishDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    SourceName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TaskName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExecutionLog = table.Column<string>(type: "ntext", nullable: true),
                    PackageID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.RecordID);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogSources",
                columns: table => new
                {
                    SourceName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogSources", x => x.SourceName);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogTasks",
                columns: table => new
                {
                    SourceName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    TaskName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogActions", x => new { x.SourceName, x.TaskName });
                });

            migrationBuilder.CreateTable(
                name: "BackgroundTasks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ScheduleID = table.Column<int>(type: "int", nullable: false),
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    EffectiveUserID = table.Column<int>(type: "int", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ItemID = table.Column<int>(type: "int", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    FinishDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IndicatorCurrent = table.Column<int>(type: "int", nullable: false),
                    IndicatorMaximum = table.Column<int>(type: "int", nullable: false),
                    MaximumExecutionTime = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: true),
                    NotifyOnComplete = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundTask", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Clusters",
                columns: table => new
                {
                    ClusterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClusterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clusters", x => x.ClusterID);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeDeletedAccounts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    OriginAT = table.Column<int>(type: "int", nullable: false),
                    StoragePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FolderName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeDeletedAccount", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeDisclaimers",
                columns: table => new
                {
                    ExchangeDisclaimerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    DisclaimerName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DisclaimerText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeDisclaimers", x => x.ExchangeDisclaimerId);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeMailboxPlanRetentionPolicyTags",
                columns: table => new
                {
                    PlanTagID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagID = table.Column<int>(type: "int", nullable: false),
                    MailboxPlanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeMailboxPlanRetentionPolicyTag", x => x.PlanTagID);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRetentionPolicyTags",
                columns: table => new
                {
                    TagID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TagType = table.Column<int>(type: "int", nullable: false),
                    AgeLimitForRetention = table.Column<int>(type: "int", nullable: false),
                    RetentionAction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRetentionPolicyTag", x => x.TagID);
                });

            migrationBuilder.CreateTable(
                name: "OCSUsers",
                columns: table => new
                {
                    OCSUserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    InstanceID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OCSUsers", x => x.OCSUserID);
                });

            migrationBuilder.CreateTable(
                name: "PackageSettings",
                columns: table => new
                {
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    SettingsName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyValue = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageSettings", x => new { x.PackageID, x.SettingsName, x.PropertyName });
                });

            migrationBuilder.CreateTable(
                name: "RDSCertificates",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RDSCertificates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RDSCollections",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RdsCollection", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RDSServerSettings",
                columns: table => new
                {
                    RdsServerId = table.Column<int>(type: "int", nullable: false),
                    SettingsName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyValue = table.Column<string>(type: "ntext", nullable: true),
                    ApplyUsers = table.Column<bool>(type: "bit", nullable: false),
                    ApplyAdministrators = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RDSServerSettings", x => new { x.RdsServerId, x.SettingsName, x.PropertyName });
                });

            migrationBuilder.CreateTable(
                name: "ResourceGroups",
                columns: table => new
                {
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GroupOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    GroupController = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ShowGroup = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceGroups", x => x.GroupID);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTasks",
                columns: table => new
                {
                    TaskID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TaskType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTasks", x => x.TaskID);
                });

            migrationBuilder.CreateTable(
                name: "SfBUserPlans",
                columns: table => new
                {
                    SfBUserPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    SfBUserPlanName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SfBUserPlanType = table.Column<int>(type: "int", nullable: true),
                    IM = table.Column<bool>(type: "bit", nullable: false),
                    Mobility = table.Column<bool>(type: "bit", nullable: false),
                    MobilityEnableOutsideVoice = table.Column<bool>(type: "bit", nullable: false),
                    Federation = table.Column<bool>(type: "bit", nullable: false),
                    Conferencing = table.Column<bool>(type: "bit", nullable: false),
                    EnterpriseVoice = table.Column<bool>(type: "bit", nullable: false),
                    VoicePolicy = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    RemoteUserAccess = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PublicIMConnectivity = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AllowOrganizeMeetingsWithExternalAnonymous = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Telephony = table.Column<int>(type: "int", nullable: true),
                    ServerURI = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ArchivePolicy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TelephonyDialPlanPolicy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TelephonyVoicePolicy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfBUserPlans", x => x.SfBUserPlanId);
                });

            migrationBuilder.CreateTable(
                name: "SfBUsers",
                columns: table => new
                {
                    SfBUserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    SfBUserPlanID = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    SipAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfBUsers", x => x.SfBUserID);
                });

            migrationBuilder.CreateTable(
                name: "SSLCertificates",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    FriendlyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Hostname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DistinguishedName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CSR = table.Column<string>(type: "ntext", nullable: true),
                    CSRLength = table.Column<int>(type: "int", nullable: true),
                    Certificate = table.Column<string>(type: "ntext", nullable: true),
                    Hash = table.Column<string>(type: "ntext", nullable: true),
                    Installed = table.Column<bool>(type: "bit", nullable: true),
                    IsRenewal = table.Column<bool>(type: "bit", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Pfx = table.Column<string>(type: "ntext", nullable: true),
                    PreviousId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSLCertificates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StorageSpaceLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageSpaceLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportServiceLevels",
                columns: table => new
                {
                    LevelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LevelDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportServiceLevel", x => x.LevelID);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    SettingsName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyValue = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => new { x.SettingsName, x.PropertyName });
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    ThemeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LTRName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RTLName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Enabled = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.ThemeID);
                });

            migrationBuilder.CreateTable(
                name: "ThemeSettings",
                columns: table => new
                {
                    ThemeSettingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThemeID = table.Column<int>(type: "int", nullable: false),
                    SettingsName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PropertyValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThemeSettings", x => x.ThemeSettingID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerID = table.Column<int>(type: "int", nullable: true),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    StatusID = table.Column<int>(type: "int", nullable: false),
                    IsDemo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsPeer = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: true),
                    Changed = table.Column<DateTime>(type: "datetime", nullable: true),
                    Comments = table.Column<string>(type: "ntext", nullable: true),
                    SecondaryEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Zip = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    PrimaryPhone = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    Fax = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    InstantMessenger = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    HtmlMail = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EcommerceEnabled = table.Column<bool>(type: "bit", nullable: true),
                    AdditionalParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginStatusId = table.Column<int>(type: "int", nullable: true),
                    FailedLogins = table.Column<int>(type: "int", nullable: true),
                    SubscriberNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    OneTimePasswordState = table.Column<int>(type: "int", nullable: true),
                    MfaMode = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PinSecret = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Users_Users",
                        column: x => x.OwnerID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Versions",
                columns: table => new
                {
                    DatabaseVersion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BuildDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versions", x => x.DatabaseVersion);
                });

            migrationBuilder.CreateTable(
                name: "BackgroundTaskLogs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    ExceptionStackTrace = table.Column<string>(type: "ntext", nullable: true),
                    InnerTaskStart = table.Column<int>(type: "int", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "ntext", nullable: true),
                    TextIdent = table.Column<int>(type: "int", nullable: true),
                    XmlParameters = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundTaskLog", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_BackgroundTaskLog_Task",
                        column: x => x.TaskID,
                        principalTable: "BackgroundTasks",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "BackgroundTaskParameters",
                columns: table => new
                {
                    ParameterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SerializerValue = table.Column<string>(type: "ntext", nullable: true),
                    TypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundTaskParameter", x => x.ParameterID);
                    table.ForeignKey(
                        name: "FK_BackgrounTaskParameter_Task",
                        column: x => x.TaskID,
                        principalTable: "BackgroundTasks",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "BackgroundTaskStack",
                columns: table => new
                {
                    TaskStackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundTaskStack", x => x.TaskStackID);
                    table.ForeignKey(
                        name: "FK_BackgroundTaskStack_Task",
                        column: x => x.TaskID,
                        principalTable: "BackgroundTasks",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "RDSCollectionSettings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RDSCollectionId = table.Column<int>(type: "int", nullable: false),
                    DisconnectedSessionLimitMin = table.Column<int>(type: "int", nullable: true),
                    ActiveSessionLimitMin = table.Column<int>(type: "int", nullable: true),
                    IdleSessionLimitMin = table.Column<int>(type: "int", nullable: true),
                    BrokenConnectionAction = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AutomaticReconnectionEnabled = table.Column<bool>(type: "bit", nullable: true),
                    TemporaryFoldersDeletedOnExit = table.Column<bool>(type: "bit", nullable: true),
                    TemporaryFoldersPerSession = table.Column<bool>(type: "bit", nullable: true),
                    ClientDeviceRedirectionOptions = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ClientPrinterRedirected = table.Column<bool>(type: "bit", nullable: true),
                    ClientPrinterAsDefault = table.Column<bool>(type: "bit", nullable: true),
                    RDEasyPrintDriverEnabled = table.Column<bool>(type: "bit", nullable: true),
                    MaxRedirectedMonitors = table.Column<int>(type: "int", nullable: true),
                    SecurityLayer = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EncryptionLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AuthenticateUsingNLA = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RDSCollectionSettings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RDSCollectionSettings_RDSCollections",
                        column: x => x.RDSCollectionId,
                        principalTable: "RDSCollections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RDSMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RDSCollectionId = table.Column<int>(type: "int", nullable: false),
                    MessageText = table.Column<string>(type: "ntext", nullable: false),
                    UserName = table.Column<string>(type: "nchar(250)", fixedLength: true, maxLength: 250, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RDSMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RDSMessages_RDSCollections",
                        column: x => x.RDSCollectionId,
                        principalTable: "RDSCollections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RDSServers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FqdName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RDSCollectionId = table.Column<int>(type: "int", nullable: true),
                    ConnectionEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Controller = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RdsServer", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RDSServers_RDSCollectionId",
                        column: x => x.RDSCollectionId,
                        principalTable: "RDSCollections",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProviderType = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    EditorControl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisableAutoDiscovery = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provider", x => x.ProviderID);
                    table.ForeignKey(
                        name: "FK_Providers_ResourceGroups",
                        column: x => x.GroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID");
                });

            migrationBuilder.CreateTable(
                name: "ResourceGroupDnsRecords",
                columns: table => new
                {
                    RecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    RecordType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    RecordName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecordData = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MXPriority = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceGroupDnsRecords", x => x.RecordID);
                    table.ForeignKey(
                        name: "FK_ResourceGroupDnsRecords_ResourceGroups",
                        column: x => x.GroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    ServerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServerUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: ""),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Comments = table.Column<string>(type: "ntext", nullable: true),
                    VirtualServer = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    InstantDomainAlias = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PrimaryGroupID = table.Column<int>(type: "int", nullable: true),
                    ADRootDomain = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ADUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ADPassword = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ADAuthenticationType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ADEnabled = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ADParentDomain = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ADParentDomainController = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.ServerID);
                    table.ForeignKey(
                        name: "FK_Servers_ResourceGroups",
                        column: x => x.PrimaryGroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID");
                });

            migrationBuilder.CreateTable(
                name: "ServiceItemTypes",
                columns: table => new
                {
                    ItemTypeID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TypeOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CalculateDiskspace = table.Column<bool>(type: "bit", nullable: true),
                    CalculateBandwidth = table.Column<bool>(type: "bit", nullable: true),
                    Suspendable = table.Column<bool>(type: "bit", nullable: true),
                    Disposable = table.Column<bool>(type: "bit", nullable: true),
                    Searchable = table.Column<bool>(type: "bit", nullable: true),
                    Importable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Backupable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceItemTypes", x => x.ItemTypeID);
                    table.ForeignKey(
                        name: "FK_ServiceItemTypes_ResourceGroups",
                        column: x => x.GroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID");
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTaskParameters",
                columns: table => new
                {
                    TaskID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParameterID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataTypeID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParameterOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTaskParameters", x => new { x.TaskID, x.ParameterID });
                    table.ForeignKey(
                        name: "FK_ScheduleTaskParameters_ScheduleTasks",
                        column: x => x.TaskID,
                        principalTable: "ScheduleTasks",
                        principalColumn: "TaskID");
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTaskViewConfiguration",
                columns: table => new
                {
                    TaskID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfigurationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Environment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTaskViewConfiguration", x => new { x.ConfigurationID, x.TaskID });
                    table.ForeignKey(
                        name: "FK_ScheduleTaskViewConfiguration_ScheduleTaskViewConfiguration",
                        column: x => x.TaskID,
                        principalTable: "ScheduleTasks",
                        principalColumn: "TaskID");
                });

            migrationBuilder.CreateTable(
                name: "StorageSpaceLevelResourceGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageSpaceLevelResourceGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageSpaceLevelResourceGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorageSpaceLevelResourceGroups_LevelId",
                        column: x => x.LevelId,
                        principalTable: "StorageSpaceLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemTypeID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CommentText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SeverityID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_Comments_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    SettingsName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyValue = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => new { x.UserID, x.SettingsName, x.PropertyName });
                    table.ForeignKey(
                        name: "FK_UserSettings_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDefaultProperties",
                columns: table => new
                {
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDefaultProperties", x => new { x.ProviderID, x.PropertyName });
                    table.ForeignKey(
                        name: "FK_ServiceDefaultProperties_Providers",
                        column: x => x.ProviderID,
                        principalTable: "Providers",
                        principalColumn: "ProviderID");
                });

            migrationBuilder.CreateTable(
                name: "HostingPlans",
                columns: table => new
                {
                    PlanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    PackageID = table.Column<int>(type: "int", nullable: true),
                    ServerID = table.Column<int>(type: "int", nullable: true),
                    PlanName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PlanDescription = table.Column<string>(type: "ntext", nullable: true),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    SetupPrice = table.Column<decimal>(type: "money", nullable: true),
                    RecurringPrice = table.Column<decimal>(type: "money", nullable: true),
                    RecurrenceUnit = table.Column<int>(type: "int", nullable: true),
                    RecurrenceLength = table.Column<int>(type: "int", nullable: true),
                    IsAddon = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingPlans", x => x.PlanID);
                    table.ForeignKey(
                        name: "FK_HostingPlans_Servers",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ServerID");
                    table.ForeignKey(
                        name: "FK_HostingPlans_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "IPAddresses",
                columns: table => new
                {
                    AddressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalIP = table.Column<string>(type: "varchar(24)", unicode: false, maxLength: 24, nullable: false),
                    InternalIP = table.Column<string>(type: "varchar(24)", unicode: false, maxLength: 24, nullable: true),
                    ServerID = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "ntext", nullable: true),
                    SubnetMask = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    DefaultGateway = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    PoolID = table.Column<int>(type: "int", nullable: true),
                    VLAN = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPAddresses", x => x.AddressID);
                    table.ForeignKey(
                        name: "FK_IPAddresses_Servers",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivateNetworkVLANs",
                columns: table => new
                {
                    VlanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vlan = table.Column<int>(type: "int", nullable: false),
                    ServerID = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateNetworkVlan", x => x.VlanID);
                    table.ForeignKey(
                        name: "FK_ServerID",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ServiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerID = table.Column<int>(type: "int", nullable: false),
                    ProviderID = table.Column<int>(type: "int", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Comments = table.Column<string>(type: "ntext", nullable: true),
                    ServiceQuotaValue = table.Column<int>(type: "int", nullable: true),
                    ClusterID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ServiceID);
                    table.ForeignKey(
                        name: "FK_Services_Clusters",
                        column: x => x.ClusterID,
                        principalTable: "Clusters",
                        principalColumn: "ClusterID");
                    table.ForeignKey(
                        name: "FK_Services_Providers",
                        column: x => x.ProviderID,
                        principalTable: "Providers",
                        principalColumn: "ProviderID");
                    table.ForeignKey(
                        name: "FK_Services_Servers",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ServerID");
                });

            migrationBuilder.CreateTable(
                name: "VirtualGroups",
                columns: table => new
                {
                    VirtualGroupID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    DistributionType = table.Column<int>(type: "int", nullable: true),
                    BindDistributionToPrimary = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualGroups", x => x.VirtualGroupID);
                    table.ForeignKey(
                        name: "FK_VirtualGroups_ResourceGroups",
                        column: x => x.GroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID");
                    table.ForeignKey(
                        name: "FK_VirtualGroups_Servers",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quotas",
                columns: table => new
                {
                    QuotaID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    QuotaOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    QuotaName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuotaDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QuotaTypeID = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    ServiceQuota = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ItemTypeID = table.Column<int>(type: "int", nullable: true),
                    HideQuota = table.Column<bool>(type: "bit", nullable: true),
                    PerOrganization = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotas", x => x.QuotaID);
                    table.ForeignKey(
                        name: "FK_Quotas_ResourceGroups",
                        column: x => x.GroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quotas_ServiceItemTypes",
                        column: x => x.ItemTypeID,
                        principalTable: "ServiceItemTypes",
                        principalColumn: "ItemTypeID");
                });

            migrationBuilder.CreateTable(
                name: "HostingPlanResources",
                columns: table => new
                {
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    CalculateDiskSpace = table.Column<bool>(type: "bit", nullable: true),
                    CalculateBandwidth = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingPlanResources", x => new { x.PlanID, x.GroupID });
                    table.ForeignKey(
                        name: "FK_HostingPlanResources_HostingPlans",
                        column: x => x.PlanID,
                        principalTable: "HostingPlans",
                        principalColumn: "PlanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HostingPlanResources_ResourceGroups",
                        column: x => x.GroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID");
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    PackageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentPackageID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    PackageName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PackageComments = table.Column<string>(type: "ntext", nullable: true),
                    ServerID = table.Column<int>(type: "int", nullable: true),
                    StatusID = table.Column<int>(type: "int", nullable: false),
                    PlanID = table.Column<int>(type: "int", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    OverrideQuotas = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BandwidthUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DefaultTopPackage = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    StatusIDchangeDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.PackageID);
                    table.ForeignKey(
                        name: "FK_Packages_HostingPlans",
                        column: x => x.PlanID,
                        principalTable: "HostingPlans",
                        principalColumn: "PlanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Packages_Packages",
                        column: x => x.ParentPackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID");
                    table.ForeignKey(
                        name: "FK_Packages_Servers",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ServerID");
                    table.ForeignKey(
                        name: "FK_Packages_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ServiceProperties",
                columns: table => new
                {
                    ServiceID = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProperties", x => new { x.ServiceID, x.PropertyName });
                    table.ForeignKey(
                        name: "FK_ServiceProperties_Services",
                        column: x => x.ServiceID,
                        principalTable: "Services",
                        principalColumn: "ServiceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StorageSpaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    Path = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    IsShared = table.Column<bool>(type: "bit", nullable: false),
                    UncPath = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    FsrmQuotaType = table.Column<int>(type: "int", nullable: false),
                    FsrmQuotaSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageSpace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageSpaces_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorageSpaces_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VirtualServices",
                columns: table => new
                {
                    VirtualServiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerID = table.Column<int>(type: "int", nullable: false),
                    ServiceID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualServices", x => x.VirtualServiceID);
                    table.ForeignKey(
                        name: "FK_VirtualServices_Servers",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VirtualServices_Services",
                        column: x => x.ServiceID,
                        principalTable: "Services",
                        principalColumn: "ServiceID");
                });

            migrationBuilder.CreateTable(
                name: "HostingPlanQuotas",
                columns: table => new
                {
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    QuotaID = table.Column<int>(type: "int", nullable: false),
                    QuotaValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingPlanQuota", x => new { x.PlanID, x.QuotaID });
                    table.ForeignKey(
                        name: "FK_HostingPlanQuotas_HostingPlans",
                        column: x => x.PlanID,
                        principalTable: "HostingPlans",
                        principalColumn: "PlanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HostingPlanQuotas_Quotas",
                        column: x => x.QuotaID,
                        principalTable: "Quotas",
                        principalColumn: "QuotaID");
                });

            migrationBuilder.CreateTable(
                name: "GlobalDnsRecords",
                columns: table => new
                {
                    RecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordType = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    RecordName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecordData = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MXPriority = table.Column<int>(type: "int", nullable: false),
                    ServiceID = table.Column<int>(type: "int", nullable: true),
                    ServerID = table.Column<int>(type: "int", nullable: true),
                    PackageID = table.Column<int>(type: "int", nullable: true),
                    IPAddressID = table.Column<int>(type: "int", nullable: true),
                    SrvPriority = table.Column<int>(type: "int", nullable: true),
                    SrvWeight = table.Column<int>(type: "int", nullable: true),
                    SrvPort = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalDnsRecord", x => x.RecordID);
                    table.ForeignKey(
                        name: "FK_GlobalDnsRecords_IPAddresses",
                        column: x => x.IPAddressID,
                        principalTable: "IPAddresses",
                        principalColumn: "AddressID");
                    table.ForeignKey(
                        name: "FK_GlobalDnsRecords_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GlobalDnsRecords_Servers",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ServerID");
                    table.ForeignKey(
                        name: "FK_GlobalDnsRecords_Services",
                        column: x => x.ServiceID,
                        principalTable: "Services",
                        principalColumn: "ServiceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageAddons",
                columns: table => new
                {
                    PackageAddonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageID = table.Column<int>(type: "int", nullable: true),
                    PlanID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Comments = table.Column<string>(type: "ntext", nullable: true),
                    StatusID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageAddons", x => x.PackageAddonID);
                    table.ForeignKey(
                        name: "FK_PackageAddons_HostingPlans",
                        column: x => x.PlanID,
                        principalTable: "HostingPlans",
                        principalColumn: "PlanID");
                    table.ForeignKey(
                        name: "FK_PackageAddons_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageQuotas",
                columns: table => new
                {
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    QuotaID = table.Column<int>(type: "int", nullable: false),
                    QuotaValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageQuotas", x => new { x.PackageID, x.QuotaID });
                    table.ForeignKey(
                        name: "FK_PackageQuotas_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID");
                    table.ForeignKey(
                        name: "FK_PackageQuotas_Quotas",
                        column: x => x.QuotaID,
                        principalTable: "Quotas",
                        principalColumn: "QuotaID");
                });

            migrationBuilder.CreateTable(
                name: "PackageResources",
                columns: table => new
                {
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    CalculateDiskspace = table.Column<bool>(type: "bit", nullable: false),
                    CalculateBandwidth = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageResources", x => new { x.PackageID, x.GroupID });
                    table.ForeignKey(
                        name: "FK_PackageResources_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID");
                    table.ForeignKey(
                        name: "FK_PackageResources_ResourceGroups",
                        column: x => x.GroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID");
                });

            migrationBuilder.CreateTable(
                name: "PackagesBandwidth",
                columns: table => new
                {
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    BytesSent = table.Column<long>(type: "bigint", nullable: false),
                    BytesReceived = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackagesBandwidth", x => new { x.PackageID, x.GroupID, x.LogDate });
                    table.ForeignKey(
                        name: "FK_PackagesBandwidth_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID");
                    table.ForeignKey(
                        name: "FK_PackagesBandwidth_ResourceGroups",
                        column: x => x.GroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID");
                });

            migrationBuilder.CreateTable(
                name: "PackagesDiskspace",
                columns: table => new
                {
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    DiskSpace = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackagesDiskspace", x => new { x.PackageID, x.GroupID });
                    table.ForeignKey(
                        name: "FK_PackagesDiskspace_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID");
                    table.ForeignKey(
                        name: "FK_PackagesDiskspace_ResourceGroups",
                        column: x => x.GroupID,
                        principalTable: "ResourceGroups",
                        principalColumn: "GroupID");
                });

            migrationBuilder.CreateTable(
                name: "PackageServices",
                columns: table => new
                {
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    ServiceID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageServices", x => new { x.PackageID, x.ServiceID });
                    table.ForeignKey(
                        name: "FK_PackageServices_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID");
                    table.ForeignKey(
                        name: "FK_PackageServices_Services",
                        column: x => x.ServiceID,
                        principalTable: "Services",
                        principalColumn: "ServiceID");
                });

            migrationBuilder.CreateTable(
                name: "PackagesTreeCache",
                columns: table => new
                {
                    ParentPackageID = table.Column<int>(type: "int", nullable: false),
                    PackageID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackagesTreeCache", x => new { x.ParentPackageID, x.PackageID });
                    table.ForeignKey(
                        name: "FK_PackagesTreeCache_Packages",
                        column: x => x.ParentPackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID");
                    table.ForeignKey(
                        name: "FK_PackagesTreeCache_Packages1",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID");
                });

            migrationBuilder.CreateTable(
                name: "PackageVLANs",
                columns: table => new
                {
                    PackageVlanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VlanID = table.Column<int>(type: "int", nullable: false),
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    IsDmz = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageVlan", x => x.PackageVlanID);
                    table.ForeignKey(
                        name: "FK_PackageID",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VlanID",
                        column: x => x.VlanID,
                        principalTable: "PrivateNetworkVLANs",
                        principalColumn: "VlanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    ScheduleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PackageID = table.Column<int>(type: "int", nullable: true),
                    ScheduleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScheduleTypeID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Interval = table.Column<int>(type: "int", nullable: true),
                    FromTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ToTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastRun = table.Column<DateTime>(type: "datetime", nullable: true),
                    NextRun = table.Column<DateTime>(type: "datetime", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    PriorityID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HistoriesNumber = table.Column<int>(type: "int", nullable: true),
                    MaxExecutionTime = table.Column<int>(type: "int", nullable: true),
                    WeekMonthDay = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.ScheduleID);
                    table.ForeignKey(
                        name: "FK_Schedule_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schedule_ScheduleTasks",
                        column: x => x.TaskID,
                        principalTable: "ScheduleTasks",
                        principalColumn: "TaskID");
                });

            migrationBuilder.CreateTable(
                name: "ServiceItems",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageID = table.Column<int>(type: "int", nullable: true),
                    ItemTypeID = table.Column<int>(type: "int", nullable: true),
                    ServiceID = table.Column<int>(type: "int", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceItems", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK_ServiceItems_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID");
                    table.ForeignKey(
                        name: "FK_ServiceItems_ServiceItemTypes",
                        column: x => x.ItemTypeID,
                        principalTable: "ServiceItemTypes",
                        principalColumn: "ItemTypeID");
                    table.ForeignKey(
                        name: "FK_ServiceItems_Services",
                        column: x => x.ServiceID,
                        principalTable: "Services",
                        principalColumn: "ServiceID");
                });

            migrationBuilder.CreateTable(
                name: "StorageSpaceFolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    StorageSpaceId = table.Column<int>(type: "int", nullable: false),
                    Path = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    UncPath = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    IsShared = table.Column<bool>(type: "bit", nullable: false),
                    FsrmQuotaType = table.Column<int>(type: "int", nullable: false),
                    FsrmQuotaSizeBytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageSpaceFolder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageSpaceFolders_StorageSpaceId",
                        column: x => x.StorageSpaceId,
                        principalTable: "StorageSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleParameters",
                columns: table => new
                {
                    ScheduleID = table.Column<int>(type: "int", nullable: false),
                    ParameterID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParameterValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleParameters", x => new { x.ScheduleID, x.ParameterID });
                    table.ForeignKey(
                        name: "FK_ScheduleParameters_Schedule",
                        column: x => x.ScheduleID,
                        principalTable: "Schedule",
                        principalColumn: "ScheduleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DmzIPAddresses",
                columns: table => new
                {
                    DmzAddressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    IPAddress = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DmzIPAddresses", x => x.DmzAddressID);
                    table.ForeignKey(
                        name: "FK_DmzIPAddresses_ServiceItems",
                        column: x => x.ItemID,
                        principalTable: "ServiceItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    DomainID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    ZoneItemID = table.Column<int>(type: "int", nullable: true),
                    DomainName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HostingAllowed = table.Column<bool>(type: "bit", nullable: false),
                    WebSiteID = table.Column<int>(type: "int", nullable: true),
                    MailDomainID = table.Column<int>(type: "int", nullable: true),
                    IsSubDomain = table.Column<bool>(type: "bit", nullable: false),
                    IsPreviewDomain = table.Column<bool>(type: "bit", nullable: false),
                    IsDomainPointer = table.Column<bool>(type: "bit", nullable: false),
                    DomainItemId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RegistrarName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.DomainID);
                    table.ForeignKey(
                        name: "FK_Domains_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Domains_ServiceItems_MailDomain",
                        column: x => x.MailDomainID,
                        principalTable: "ServiceItems",
                        principalColumn: "ItemID");
                    table.ForeignKey(
                        name: "FK_Domains_ServiceItems_WebSite",
                        column: x => x.WebSiteID,
                        principalTable: "ServiceItems",
                        principalColumn: "ItemID");
                    table.ForeignKey(
                        name: "FK_Domains_ServiceItems_ZoneItem",
                        column: x => x.ZoneItemID,
                        principalTable: "ServiceItems",
                        principalColumn: "ItemID");
                });

            migrationBuilder.CreateTable(
                name: "ExchangeOrganizationDomains",
                columns: table => new
                {
                    OrganizationDomainID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    DomainID = table.Column<int>(type: "int", nullable: true),
                    IsHost = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DomainTypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeOrganizationDomains", x => x.OrganizationDomainID);
                    table.ForeignKey(
                        name: "FK_ExchangeOrganizationDomains_ServiceItems",
                        column: x => x.ItemID,
                        principalTable: "ServiceItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeOrganizations",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    OrganizationID = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExchangeMailboxPlanID = table.Column<int>(type: "int", nullable: true),
                    LyncUserPlanID = table.Column<int>(type: "int", nullable: true),
                    SfBUserPlanID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeOrganizations", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK_ExchangeOrganizations_ServiceItems",
                        column: x => x.ItemID,
                        principalTable: "ServiceItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageIPAddresses",
                columns: table => new
                {
                    PackageAddressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    AddressID = table.Column<int>(type: "int", nullable: false),
                    ItemID = table.Column<int>(type: "int", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: true),
                    OrgID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageIPAddresses", x => x.PackageAddressID);
                    table.ForeignKey(
                        name: "FK_PackageIPAddresses_IPAddresses",
                        column: x => x.AddressID,
                        principalTable: "IPAddresses",
                        principalColumn: "AddressID");
                    table.ForeignKey(
                        name: "FK_PackageIPAddresses_Packages",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageIPAddresses_ServiceItems",
                        column: x => x.ItemID,
                        principalTable: "ServiceItems",
                        principalColumn: "ItemID");
                });

            migrationBuilder.CreateTable(
                name: "PrivateIPAddresses",
                columns: table => new
                {
                    PrivateAddressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    IPAddress = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateIPAddresses", x => x.PrivateAddressID);
                    table.ForeignKey(
                        name: "FK_PrivateIPAddresses_ServiceItems",
                        column: x => x.ItemID,
                        principalTable: "ServiceItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceItemProperties",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceItemProperties", x => new { x.ItemID, x.PropertyName });
                    table.ForeignKey(
                        name: "FK_ServiceItemProperties_ServiceItems",
                        column: x => x.ItemID,
                        principalTable: "ServiceItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnterpriseFolders",
                columns: table => new
                {
                    EnterpriseFolderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    FolderName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FolderQuota = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LocationDrive = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HomeFolder = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StorageSpaceFolderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnterpriseFolders", x => x.EnterpriseFolderID);
                    table.ForeignKey(
                        name: "FK_EnterpriseFolders_StorageSpaceFolderId",
                        column: x => x.StorageSpaceFolderId,
                        principalTable: "StorageSpaceFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DomainDnsRecords",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DomainId = table.Column<int>(type: "int", nullable: false),
                    RecordType = table.Column<int>(type: "int", nullable: false),
                    DnsServer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainDnsRecord", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DomainDnsRecords_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "DomainID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeMailboxPlans",
                columns: table => new
                {
                    MailboxPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    MailboxPlan = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MailboxPlanType = table.Column<int>(type: "int", nullable: true),
                    EnableActiveSync = table.Column<bool>(type: "bit", nullable: false),
                    EnableIMAP = table.Column<bool>(type: "bit", nullable: false),
                    EnableMAPI = table.Column<bool>(type: "bit", nullable: false),
                    EnableOWA = table.Column<bool>(type: "bit", nullable: false),
                    EnablePOP = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IssueWarningPct = table.Column<int>(type: "int", nullable: false),
                    KeepDeletedItemsDays = table.Column<int>(type: "int", nullable: false),
                    MailboxSizeMB = table.Column<int>(type: "int", nullable: false),
                    MaxReceiveMessageSizeKB = table.Column<int>(type: "int", nullable: false),
                    MaxRecipients = table.Column<int>(type: "int", nullable: false),
                    MaxSendMessageSizeKB = table.Column<int>(type: "int", nullable: false),
                    ProhibitSendPct = table.Column<int>(type: "int", nullable: false),
                    ProhibitSendReceivePct = table.Column<int>(type: "int", nullable: false),
                    HideFromAddressBook = table.Column<bool>(type: "bit", nullable: false),
                    AllowLitigationHold = table.Column<bool>(type: "bit", nullable: true),
                    RecoverableItemsWarningPct = table.Column<int>(type: "int", nullable: true),
                    RecoverableItemsSpace = table.Column<int>(type: "int", nullable: true),
                    LitigationHoldUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LitigationHoldMsg = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Archiving = table.Column<bool>(type: "bit", nullable: true),
                    EnableArchiving = table.Column<bool>(type: "bit", nullable: true),
                    ArchiveSizeMB = table.Column<int>(type: "int", nullable: true),
                    ArchiveWarningPct = table.Column<int>(type: "int", nullable: true),
                    EnableAutoReply = table.Column<bool>(type: "bit", nullable: true),
                    IsForJournaling = table.Column<bool>(type: "bit", nullable: true),
                    EnableForceArchiveDeletion = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeMailboxPlans", x => x.MailboxPlanId);
                    table.ForeignKey(
                        name: "FK_ExchangeMailboxPlans_ExchangeOrganizations",
                        column: x => x.ItemID,
                        principalTable: "ExchangeOrganizations",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeOrganizationSettings",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SettingsName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Xml = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeOrganizationSettings", x => new { x.ItemId, x.SettingsName });
                    table.ForeignKey(
                        name: "FK_ExchangeOrganizationSettings_ExchangeOrganizations_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ExchangeOrganizations",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeOrganizationSsFolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    StorageSpaceFolderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeOrganizationSsFolder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeOrganizationSsFolders_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ExchangeOrganizations",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExchangeOrganizationSsFolders_StorageSpaceFolderId",
                        column: x => x.StorageSpaceFolderId,
                        principalTable: "StorageSpaceFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LyncUserPlans",
                columns: table => new
                {
                    LyncUserPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    LyncUserPlanName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    LyncUserPlanType = table.Column<int>(type: "int", nullable: true),
                    IM = table.Column<bool>(type: "bit", nullable: false),
                    Mobility = table.Column<bool>(type: "bit", nullable: false),
                    MobilityEnableOutsideVoice = table.Column<bool>(type: "bit", nullable: false),
                    Federation = table.Column<bool>(type: "bit", nullable: false),
                    Conferencing = table.Column<bool>(type: "bit", nullable: false),
                    EnterpriseVoice = table.Column<bool>(type: "bit", nullable: false),
                    VoicePolicy = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    RemoteUserAccess = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PublicIMConnectivity = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AllowOrganizeMeetingsWithExternalAnonymous = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Telephony = table.Column<int>(type: "int", nullable: true),
                    ServerURI = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ArchivePolicy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TelephonyDialPlanPolicy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TelephonyVoicePolicy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LyncUserPlans", x => x.LyncUserPlanId);
                    table.ForeignKey(
                        name: "FK_LyncUserPlans_ExchangeOrganizations",
                        column: x => x.ItemID,
                        principalTable: "ExchangeOrganizations",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeAccounts",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PrimaryEmailAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MailEnabledPublicFolder = table.Column<bool>(type: "bit", nullable: true),
                    MailboxManagerActions = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    SamAccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    MailboxPlanId = table.Column<int>(type: "int", nullable: true),
                    SubscriberNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    UserPrincipalName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ExchangeDisclaimerId = table.Column<int>(type: "int", nullable: true),
                    ArchivingMailboxPlanId = table.Column<int>(type: "int", nullable: true),
                    EnableArchiving = table.Column<bool>(type: "bit", nullable: true),
                    LevelID = table.Column<int>(type: "int", nullable: true),
                    IsVIP = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeAccounts", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK_ExchangeAccounts_ExchangeMailboxPlans",
                        column: x => x.MailboxPlanId,
                        principalTable: "ExchangeMailboxPlans",
                        principalColumn: "MailboxPlanId");
                    table.ForeignKey(
                        name: "FK_ExchangeAccounts_ServiceItems",
                        column: x => x.ItemID,
                        principalTable: "ServiceItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LyncUsers",
                columns: table => new
                {
                    LyncUserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    LyncUserPlanID = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    SipAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LyncUsers", x => x.LyncUserID);
                    table.ForeignKey(
                        name: "FK_LyncUsers_LyncUserPlans",
                        column: x => x.LyncUserPlanID,
                        principalTable: "LyncUserPlans",
                        principalColumn: "LyncUserPlanId");
                });

            migrationBuilder.CreateTable(
                name: "AccessTokens",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessTokenGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    TokenType = table.Column<int>(type: "int", nullable: false),
                    SmsResponse = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessToken", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccessTokens_UserId",
                        column: x => x.AccountID,
                        principalTable: "ExchangeAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlackBerryUsers",
                columns: table => new
                {
                    BlackBerryUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackBerryUsers", x => x.BlackBerryUserId);
                    table.ForeignKey(
                        name: "FK_BlackBerryUsers_ExchangeAccounts",
                        column: x => x.AccountId,
                        principalTable: "ExchangeAccounts",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "CRMUsers",
                columns: table => new
                {
                    CRMUserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ChangedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CRMUserGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BusinessUnitID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CALType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMUsers", x => x.CRMUserID);
                    table.ForeignKey(
                        name: "FK_CRMUsers_ExchangeAccounts",
                        column: x => x.AccountID,
                        principalTable: "ExchangeAccounts",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "EnterpriseFoldersOwaPermissions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    FolderID = table.Column<int>(type: "int", nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnterpriseFoldersOwaPermission", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EnterpriseFoldersOwaPermissions_AccountId",
                        column: x => x.AccountID,
                        principalTable: "ExchangeAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnterpriseFoldersOwaPermissions_FolderId",
                        column: x => x.FolderID,
                        principalTable: "EnterpriseFolders",
                        principalColumn: "EnterpriseFolderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeAccountEmailAddresses",
                columns: table => new
                {
                    AddressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeAccountEmailAddresses", x => x.AddressID);
                    table.ForeignKey(
                        name: "FK_ExchangeAccountEmailAddresses_ExchangeAccounts",
                        column: x => x.AccountID,
                        principalTable: "ExchangeAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RDSCollectionUsers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RDSCollectionId = table.Column<int>(type: "int", nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RdsCollectionUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RDSCollectionUsers_RDSCollectionId",
                        column: x => x.RDSCollectionId,
                        principalTable: "RDSCollections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RDSCollectionUsers_UserId",
                        column: x => x.AccountID,
                        principalTable: "ExchangeAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebDavAccessTokens",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebDavAccessToken", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WebDavAccessTokens_UserId",
                        column: x => x.AccountID,
                        principalTable: "ExchangeAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebDavPortalUsersSettings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Settings = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebDavPortalUsersSetting", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WebDavPortalUsersSettings_UserId",
                        column: x => x.AccountId,
                        principalTable: "ExchangeAccounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AuditLogSources",
                column: "SourceName",
                values: new object[]
                {
                    "APP_INSTALLER",
                    "AUTO_DISCOVERY",
                    "BACKUP",
                    "DNS_ZONE",
                    "DOMAIN",
                    "ENTERPRISE_STORAGE",
                    "EXCHANGE",
                    "FILES",
                    "FTP_ACCOUNT",
                    "GLOBAL_DNS",
                    "HOSTING_SPACE",
                    "HOSTING_SPACE_WR",
                    "IMPORT",
                    "IP_ADDRESS",
                    "MAIL_ACCOUNT",
                    "MAIL_DOMAIN",
                    "MAIL_FORWARDING",
                    "MAIL_GROUP",
                    "MAIL_LIST",
                    "OCS",
                    "ODBC_DSN",
                    "ORGANIZATION",
                    "REMOTE_DESKTOP_SERVICES",
                    "SCHEDULER",
                    "SERVER",
                    "SHAREPOINT",
                    "SPACE",
                    "SQL_DATABASE",
                    "SQL_USER",
                    "STATS_SITE",
                    "STORAGE_SPACES",
                    "USER",
                    "VIRTUAL_SERVER",
                    "VLAN",
                    "VPS",
                    "VPS2012",
                    "WAG_INSTALLER",
                    "WEB_SITE"
                });

            migrationBuilder.InsertData(
                table: "AuditLogTasks",
                columns: new[] { "SourceName", "TaskName", "TaskDescription" },
                values: new object[,]
                {
                    { "APP_INSTALLER", "INSTALL_APPLICATION", "Install application" },
                    { "AUTO_DISCOVERY", "IS_INSTALLED", "Is installed" },
                    { "BACKUP", "BACKUP", "Backup" },
                    { "BACKUP", "RESTORE", "Restore" },
                    { "DNS_ZONE", "ADD_RECORD", "Add record" },
                    { "DNS_ZONE", "DELETE_RECORD", "Delete record" },
                    { "DNS_ZONE", "UPDATE_RECORD", "Update record" },
                    { "DOMAIN", "ADD", "Add" },
                    { "DOMAIN", "DELETE", "Delete" },
                    { "DOMAIN", "ENABLE_DNS", "Enable DNS" },
                    { "DOMAIN", "UPDATE", "Update" },
                    { "ENTERPRISE_STORAGE", "CREATE_FOLDER", "Create folder" },
                    { "ENTERPRISE_STORAGE", "CREATE_MAPPED_DRIVE", "Create mapped drive" },
                    { "ENTERPRISE_STORAGE", "DELETE_FOLDER", "Delete folder" },
                    { "ENTERPRISE_STORAGE", "DELETE_MAPPED_DRIVE", "Delete mapped drive" },
                    { "ENTERPRISE_STORAGE", "GET_ORG_STATS", "Get organization statistics" },
                    { "ENTERPRISE_STORAGE", "SET_ENTERPRISE_FOLDER_GENERAL_SETTINGS", "Set enterprise folder general settings" },
                    { "EXCHANGE", "ADD_DISTR_LIST_ADDRESS", "Add distribution list e-mail address" },
                    { "EXCHANGE", "ADD_DOMAIN", "Add organization domain" },
                    { "EXCHANGE", "ADD_EXCHANGE_EXCHANGEDISCLAIMER", "Add Exchange disclaimer" },
                    { "EXCHANGE", "ADD_EXCHANGE_MAILBOXPLAN_RETENTIONPOLICY_ARCHIVING", "Add Exchange archiving retention policy" },
                    { "EXCHANGE", "ADD_EXCHANGE_RETENTIONPOLICYTAG", "Add Exchange retention policy tag" },
                    { "EXCHANGE", "ADD_MAILBOX_ADDRESS", "Add mailbox e-mail address" },
                    { "EXCHANGE", "ADD_PUBLIC_FOLDER_ADDRESS", "Add public folder e-mail address" },
                    { "EXCHANGE", "CALCULATE_DISKSPACE", "Calculate organization disk space" },
                    { "EXCHANGE", "CREATE_CONTACT", "Create contact" },
                    { "EXCHANGE", "CREATE_DISTR_LIST", "Create distribution list" },
                    { "EXCHANGE", "CREATE_MAILBOX", "Create mailbox" },
                    { "EXCHANGE", "CREATE_ORG", "Create organization" },
                    { "EXCHANGE", "CREATE_PUBLIC_FOLDER", "Create public folder" },
                    { "EXCHANGE", "DELETE_CONTACT", "Delete contact" },
                    { "EXCHANGE", "DELETE_DISTR_LIST", "Delete distribution list" },
                    { "EXCHANGE", "DELETE_DISTR_LIST_ADDRESSES", "Delete distribution list e-mail addresses" },
                    { "EXCHANGE", "DELETE_DOMAIN", "Delete organization domain" },
                    { "EXCHANGE", "DELETE_EXCHANGE_MAILBOXPLAN_RETENTIONPOLICY_ARCHIV", "Delete Exchange archiving retention policy" },
                    { "EXCHANGE", "DELETE_EXCHANGE_RETENTIONPOLICYTAG", "Delete Exchange retention policy tag" },
                    { "EXCHANGE", "DELETE_MAILBOX", "Delete mailbox" },
                    { "EXCHANGE", "DELETE_MAILBOX_ADDRESSES", "Delete mailbox e-mail addresses" },
                    { "EXCHANGE", "DELETE_ORG", "Delete organization" },
                    { "EXCHANGE", "DELETE_PUBLIC_FOLDER", "Delete public folder" },
                    { "EXCHANGE", "DELETE_PUBLIC_FOLDER_ADDRESSES", "Delete public folder e-mail addresses" },
                    { "EXCHANGE", "DISABLE_MAIL_PUBLIC_FOLDER", "Disable mail public folder" },
                    { "EXCHANGE", "DISABLE_MAILBOX", "Disable Mailbox" },
                    { "EXCHANGE", "ENABLE_MAIL_PUBLIC_FOLDER", "Enable mail public folder" },
                    { "EXCHANGE", "GET_ACTIVESYNC_POLICY", "Get Activesync policy" },
                    { "EXCHANGE", "GET_CONTACT_GENERAL", "Get contact general settings" },
                    { "EXCHANGE", "GET_CONTACT_MAILFLOW", "Get contact mail flow settings" },
                    { "EXCHANGE", "GET_DISTR_LIST_ADDRESSES", "Get distribution list e-mail addresses" },
                    { "EXCHANGE", "GET_DISTR_LIST_BYMEMBER", "Get distributions list by member" },
                    { "EXCHANGE", "GET_DISTR_LIST_GENERAL", "Get distribution list general settings" },
                    { "EXCHANGE", "GET_DISTR_LIST_MAILFLOW", "Get distribution list mail flow settings" },
                    { "EXCHANGE", "GET_DISTRIBUTION_LIST_RESULT", "Get distributions list result" },
                    { "EXCHANGE", "GET_EXCHANGE_ACCOUNTDISCLAIMERID", "Get Exchange account disclaimer id" },
                    { "EXCHANGE", "GET_EXCHANGE_EXCHANGEDISCLAIMER", "Get Exchange disclaimer" },
                    { "EXCHANGE", "GET_EXCHANGE_MAILBOXPLAN", "Get Exchange Mailbox plan" },
                    { "EXCHANGE", "GET_EXCHANGE_MAILBOXPLANS", "Get Exchange Mailbox plans" },
                    { "EXCHANGE", "GET_EXCHANGE_RETENTIONPOLICYTAG", "Get Exchange retention policy tag" },
                    { "EXCHANGE", "GET_EXCHANGE_RETENTIONPOLICYTAGS", "Get Exchange retention policy tags" },
                    { "EXCHANGE", "GET_FOLDERS_STATS", "Get organization public folder statistics" },
                    { "EXCHANGE", "GET_MAILBOX_ADDRESSES", "Get mailbox e-mail addresses" },
                    { "EXCHANGE", "GET_MAILBOX_ADVANCED", "Get mailbox advanced settings" },
                    { "EXCHANGE", "GET_MAILBOX_AUTOREPLY", "Get Mailbox autoreply" },
                    { "EXCHANGE", "GET_MAILBOX_GENERAL", "Get mailbox general settings" },
                    { "EXCHANGE", "GET_MAILBOX_MAILFLOW", "Get mailbox mail flow settings" },
                    { "EXCHANGE", "GET_MAILBOX_PERMISSIONS", "Get Mailbox permissions" },
                    { "EXCHANGE", "GET_MAILBOX_STATS", "Get Mailbox statistics" },
                    { "EXCHANGE", "GET_MAILBOXES_STATS", "Get organization mailboxes statistics" },
                    { "EXCHANGE", "GET_MOBILE_DEVICES", "Get mobile devices" },
                    { "EXCHANGE", "GET_ORG_LIMITS", "Get organization storage limits" },
                    { "EXCHANGE", "GET_ORG_STATS", "Get organization statistics" },
                    { "EXCHANGE", "GET_PICTURE", "Get picture" },
                    { "EXCHANGE", "GET_PUBLIC_FOLDER_ADDRESSES", "Get public folder e-mail addresses" },
                    { "EXCHANGE", "GET_PUBLIC_FOLDER_GENERAL", "Get public folder general settings" },
                    { "EXCHANGE", "GET_PUBLIC_FOLDER_MAILFLOW", "Get public folder mail flow settings" },
                    { "EXCHANGE", "GET_RESOURCE_MAILBOX", "Get resource Mailbox settings" },
                    { "EXCHANGE", "SET_EXCHANGE_ACCOUNTDISCLAIMERID", "Set exchange account disclaimer id" },
                    { "EXCHANGE", "SET_EXCHANGE_MAILBOXPLAN", "Set exchange Mailbox plan" },
                    { "EXCHANGE", "SET_MAILBOXPLAN_RETENTIONPOLICY_ARCHIVING", "Set Mailbox plan retention policy archiving" },
                    { "EXCHANGE", "SET_ORG_LIMITS", "Update organization storage limits" },
                    { "EXCHANGE", "SET_PRIMARY_DISTR_LIST_ADDRESS", "Set distribution list primary e-mail address" },
                    { "EXCHANGE", "SET_PRIMARY_MAILBOX_ADDRESS", "Set mailbox primary e-mail address" },
                    { "EXCHANGE", "SET_PRIMARY_PUBLIC_FOLDER_ADDRESS", "Set public folder primary e-mail address" },
                    { "EXCHANGE", "UPDATE_CONTACT_GENERAL", "Update contact general settings" },
                    { "EXCHANGE", "UPDATE_CONTACT_MAILFLOW", "Update contact mail flow settings" },
                    { "EXCHANGE", "UPDATE_DISTR_LIST_GENERAL", "Update distribution list general settings" },
                    { "EXCHANGE", "UPDATE_DISTR_LIST_MAILFLOW", "Update distribution list mail flow settings" },
                    { "EXCHANGE", "UPDATE_EXCHANGE_RETENTIONPOLICYTAG", "Update Exchange retention policy tag" },
                    { "EXCHANGE", "UPDATE_MAILBOX_ADVANCED", "Update mailbox advanced settings" },
                    { "EXCHANGE", "UPDATE_MAILBOX_AUTOREPLY", "Update Mailbox autoreply" },
                    { "EXCHANGE", "UPDATE_MAILBOX_GENERAL", "Update mailbox general settings" },
                    { "EXCHANGE", "UPDATE_MAILBOX_MAILFLOW", "Update mailbox mail flow settings" },
                    { "EXCHANGE", "UPDATE_PUBLIC_FOLDER_GENERAL", "Update public folder general settings" },
                    { "EXCHANGE", "UPDATE_PUBLIC_FOLDER_MAILFLOW", "Update public folder mail flow settings" },
                    { "EXCHANGE", "UPDATE_RESOURCE_MAILBOX", "Update resource Mailbox settings" },
                    { "FILES", "COPY_FILES", "Copy files" },
                    { "FILES", "CREATE_ACCESS_DATABASE", "Create MS Access database" },
                    { "FILES", "CREATE_FILE", "Create file" },
                    { "FILES", "CREATE_FOLDER", "Create folder" },
                    { "FILES", "DELETE_FILES", "Delete files" },
                    { "FILES", "MOVE_FILES", "Move files" },
                    { "FILES", "RENAME_FILE", "Rename file" },
                    { "FILES", "SET_PERMISSIONS", null },
                    { "FILES", "UNZIP_FILES", "Unzip files" },
                    { "FILES", "UPDATE_BINARY_CONTENT", "Update file binary content" },
                    { "FILES", "ZIP_FILES", "Zip files" },
                    { "FTP_ACCOUNT", "ADD", "Add" },
                    { "FTP_ACCOUNT", "DELETE", "Delete" },
                    { "FTP_ACCOUNT", "UPDATE", "Update" },
                    { "GLOBAL_DNS", "ADD", "Add" },
                    { "GLOBAL_DNS", "DELETE", "Delete" },
                    { "GLOBAL_DNS", "UPDATE", "Update" },
                    { "HOSTING_SPACE", "ADD", "Add" },
                    { "HOSTING_SPACE_WR", "ADD", "Add" },
                    { "IMPORT", "IMPORT", "Import" },
                    { "IP_ADDRESS", "ADD", "Add" },
                    { "IP_ADDRESS", "ADD_RANGE", "Add range" },
                    { "IP_ADDRESS", "ALLOCATE_PACKAGE_IP", "Allocate package IP addresses" },
                    { "IP_ADDRESS", "DEALLOCATE_PACKAGE_IP", "Deallocate package IP addresses" },
                    { "IP_ADDRESS", "DELETE", "Delete" },
                    { "IP_ADDRESS", "DELETE_RANGE", "Delete IP Addresses" },
                    { "IP_ADDRESS", "UPDATE", "Update" },
                    { "IP_ADDRESS", "UPDATE_RANGE", "Update IP Addresses" },
                    { "MAIL_ACCOUNT", "ADD", "Add" },
                    { "MAIL_ACCOUNT", "DELETE", "Delete" },
                    { "MAIL_ACCOUNT", "UPDATE", "Update" },
                    { "MAIL_DOMAIN", "ADD", "Add" },
                    { "MAIL_DOMAIN", "ADD_POINTER", "Add pointer" },
                    { "MAIL_DOMAIN", "DELETE", "Delete" },
                    { "MAIL_DOMAIN", "DELETE_POINTER", "Update pointer" },
                    { "MAIL_DOMAIN", "UPDATE", "Update" },
                    { "MAIL_FORWARDING", "ADD", "Add" },
                    { "MAIL_FORWARDING", "DELETE", "Delete" },
                    { "MAIL_FORWARDING", "UPDATE", "Update" },
                    { "MAIL_GROUP", "ADD", "Add" },
                    { "MAIL_GROUP", "DELETE", "Delete" },
                    { "MAIL_GROUP", "UPDATE", "Update" },
                    { "MAIL_LIST", "ADD", "Add" },
                    { "MAIL_LIST", "DELETE", "Delete" },
                    { "MAIL_LIST", "UPDATE", "Update" },
                    { "OCS", "CREATE_OCS_USER", "Create OCS user" },
                    { "OCS", "GET_OCS_USERS", "Get OCS users" },
                    { "OCS", "GET_OCS_USERS_COUNT", "Get OCS users count" },
                    { "ODBC_DSN", "ADD", "Add" },
                    { "ODBC_DSN", "DELETE", "Delete" },
                    { "ODBC_DSN", "UPDATE", "Update" },
                    { "ORGANIZATION", "CREATE_ORG", "Create organization" },
                    { "ORGANIZATION", "CREATE_ORGANIZATION_ENTERPRISE_STORAGE", "Create organization enterprise storage" },
                    { "ORGANIZATION", "CREATE_SECURITY_GROUP", "Create security group" },
                    { "ORGANIZATION", "CREATE_USER", "Create user" },
                    { "ORGANIZATION", "DELETE_ORG", "Delete organization" },
                    { "ORGANIZATION", "DELETE_SECURITY_GROUP", "Delete security group" },
                    { "ORGANIZATION", "GET_ORG_STATS", "Get organization statistics" },
                    { "ORGANIZATION", "GET_SECURITY_GROUP_GENERAL", "Get security group general settings" },
                    { "ORGANIZATION", "GET_SECURITY_GROUPS_BYMEMBER", "Get security groups by member" },
                    { "ORGANIZATION", "GET_SUPPORT_SERVICE_LEVELS", "Get support service levels" },
                    { "ORGANIZATION", "REMOVE_USER", "Remove user" },
                    { "ORGANIZATION", "SEND_USER_PASSWORD_RESET_EMAIL_PINCODE", "Send user password reset email pincode" },
                    { "ORGANIZATION", "SET_USER_PASSWORD", "Set user password" },
                    { "ORGANIZATION", "SET_USER_USERPRINCIPALNAME", "Set user principal name" },
                    { "ORGANIZATION", "UPDATE_PASSWORD_SETTINGS", "Update password settings" },
                    { "ORGANIZATION", "UPDATE_SECURITY_GROUP_GENERAL", "Update security group general settings" },
                    { "ORGANIZATION", "UPDATE_USER_GENERAL", "Update user general settings" },
                    { "REMOTE_DESKTOP_SERVICES", "ADD_RDS_SERVER", "Add RDS server" },
                    { "REMOTE_DESKTOP_SERVICES", "RESTART_RDS_SERVER", "Restart RDS server" },
                    { "REMOTE_DESKTOP_SERVICES", "SET_RDS_SERVER_NEW_CONNECTIONS_ALLOWED", "Set RDS new connection allowed" },
                    { "SCHEDULER", "RUN_SCHEDULE", null },
                    { "SERVER", "ADD", "Add" },
                    { "SERVER", "ADD_SERVICE", "Add service" },
                    { "SERVER", "CHANGE_WINDOWS_SERVICE_STATUS", "Change Windows service status" },
                    { "SERVER", "CHECK_AVAILABILITY", "Check availability" },
                    { "SERVER", "CLEAR_EVENT_LOG", "Clear Windows event log" },
                    { "SERVER", "DELETE", "Delete" },
                    { "SERVER", "DELETE_SERVICE", "Delete service" },
                    { "SERVER", "REBOOT", "Reboot" },
                    { "SERVER", "RESET_TERMINAL_SESSION", "Reset terminal session" },
                    { "SERVER", "TERMINATE_SYSTEM_PROCESS", "Terminate system process" },
                    { "SERVER", "UPDATE", "Update" },
                    { "SERVER", "UPDATE_AD_PASSWORD", "Update active directory password" },
                    { "SERVER", "UPDATE_PASSWORD", "Update access password" },
                    { "SERVER", "UPDATE_SERVICE", "Update service" },
                    { "SHAREPOINT", "ADD_GROUP", "Add group" },
                    { "SHAREPOINT", "ADD_SITE", "Add site" },
                    { "SHAREPOINT", "ADD_USER", "Add user" },
                    { "SHAREPOINT", "BACKUP_SITE", "Backup site" },
                    { "SHAREPOINT", "DELETE_GROUP", "Delete group" },
                    { "SHAREPOINT", "DELETE_SITE", "Delete site" },
                    { "SHAREPOINT", "DELETE_USER", "Delete user" },
                    { "SHAREPOINT", "INSTALL_WEBPARTS", "Install Web Parts package" },
                    { "SHAREPOINT", "RESTORE_SITE", "Restore site" },
                    { "SHAREPOINT", "UNINSTALL_WEBPARTS", "Uninstall Web Parts package" },
                    { "SHAREPOINT", "UPDATE_GROUP", "Update group" },
                    { "SHAREPOINT", "UPDATE_USER", "Update user" },
                    { "SPACE", "CALCULATE_DISKSPACE", "Calculate disk space" },
                    { "SPACE", "CHANGE_ITEMS_STATUS", "Change hosting items status" },
                    { "SPACE", "CHANGE_STATUS", "Change hostng space status" },
                    { "SPACE", "DELETE", "Delete hosting space" },
                    { "SPACE", "DELETE_ITEMS", "Delete hosting items" },
                    { "SQL_DATABASE", "ADD", "Add" },
                    { "SQL_DATABASE", "BACKUP", "Backup" },
                    { "SQL_DATABASE", "DELETE", "Delete" },
                    { "SQL_DATABASE", "RESTORE", "Restore" },
                    { "SQL_DATABASE", "TRUNCATE", "Truncate" },
                    { "SQL_DATABASE", "UPDATE", "Update" },
                    { "SQL_USER", "ADD", "Add" },
                    { "SQL_USER", "DELETE", "Delete" },
                    { "SQL_USER", "UPDATE", "Update" },
                    { "STATS_SITE", "ADD", "Add statistics site" },
                    { "STATS_SITE", "DELETE", "Delete statistics site" },
                    { "STATS_SITE", "UPDATE", "Update statistics site" },
                    { "STORAGE_SPACES", "REMOVE_STORAGE_SPACE", "Remove storage space" },
                    { "STORAGE_SPACES", "SAVE_STORAGE_SPACE", "Save storage space" },
                    { "STORAGE_SPACES", "SAVE_STORAGE_SPACE_LEVEL", "Save storage space level" },
                    { "USER", "ADD", "Add" },
                    { "USER", "AUTHENTICATE", "Authenticate" },
                    { "USER", "CHANGE_PASSWORD", "Change password" },
                    { "USER", "CHANGE_PASSWORD_BY_USERNAME_PASSWORD", "Change password by username/password" },
                    { "USER", "CHANGE_STATUS", "Change status" },
                    { "USER", "DELETE", "Delete" },
                    { "USER", "GET_BY_USERNAME_PASSWORD", "Get by username/password" },
                    { "USER", "SEND_REMINDER", "Send password reminder" },
                    { "USER", "UPDATE", "Update" },
                    { "USER", "UPDATE_SETTINGS", "Update settings" },
                    { "VIRTUAL_SERVER", "ADD_SERVICES", "Add services" },
                    { "VIRTUAL_SERVER", "DELETE_SERVICES", "Delete services" },
                    { "VLAN", "ADD", "Add" },
                    { "VLAN", "ADD_RANGE", "Add range" },
                    { "VLAN", "ALLOCATE_PACKAGE_VLAN", "Allocate package VLAN" },
                    { "VLAN", "DEALLOCATE_PACKAGE_VLAN", "Deallocate package VLAN" },
                    { "VLAN", "DELETE_RANGE", "Delete range" },
                    { "VLAN", "UPDATE", "Update" },
                    { "VPS", "ADD_EXTERNAL_IP", "Add external IP" },
                    { "VPS", "ADD_PRIVATE_IP", "Add private IP" },
                    { "VPS", "APPLY_SNAPSHOT", "Apply VPS snapshot" },
                    { "VPS", "CANCEL_JOB", "Cancel Job" },
                    { "VPS", "CHANGE_ADMIN_PASSWORD", "Change administrator password" },
                    { "VPS", "CHANGE_STATE", "Change VPS state" },
                    { "VPS", "CREATE", "Create VPS" },
                    { "VPS", "DELETE", "Delete VPS" },
                    { "VPS", "DELETE_EXTERNAL_IP", "Delete external IP" },
                    { "VPS", "DELETE_PRIVATE_IP", "Delete private IP" },
                    { "VPS", "DELETE_SNAPSHOT", "Delete VPS snapshot" },
                    { "VPS", "DELETE_SNAPSHOT_SUBTREE", "Delete VPS snapshot subtree" },
                    { "VPS", "EJECT_DVD_DISK", "Eject DVD disk" },
                    { "VPS", "INSERT_DVD_DISK", "Insert DVD disk" },
                    { "VPS", "REINSTALL", "Re-install VPS" },
                    { "VPS", "RENAME_SNAPSHOT", "Rename VPS snapshot" },
                    { "VPS", "SEND_SUMMARY_LETTER", "Send VPS summary letter" },
                    { "VPS", "SET_PRIMARY_EXTERNAL_IP", "Set primary external IP" },
                    { "VPS", "SET_PRIMARY_PRIVATE_IP", "Set primary private IP" },
                    { "VPS", "TAKE_SNAPSHOT", "Take VPS snapshot" },
                    { "VPS", "UPDATE_CONFIGURATION", "Update VPS configuration" },
                    { "VPS", "UPDATE_HOSTNAME", "Update host name" },
                    { "VPS", "UPDATE_IP", "Update IP Address" },
                    { "VPS", "UPDATE_PERMISSIONS", "Update VPS permissions" },
                    { "VPS", "UPDATE_VDC_PERMISSIONS", "Update space permissions" },
                    { "VPS2012", "ADD_EXTERNAL_IP", "Add external IP" },
                    { "VPS2012", "ADD_PRIVATE_IP", "Add private IP" },
                    { "VPS2012", "APPLY_SNAPSHOT", "Apply VM snapshot" },
                    { "VPS2012", "CHANGE_ADMIN_PASSWORD", "Change administrator password" },
                    { "VPS2012", "CHANGE_STATE", "Change VM state" },
                    { "VPS2012", "CREATE", "Create VM" },
                    { "VPS2012", "DELETE", "Delete VM" },
                    { "VPS2012", "DELETE_EXTERNAL_IP", "Delete external IP" },
                    { "VPS2012", "DELETE_PRIVATE_IP", "Delete private IP" },
                    { "VPS2012", "DELETE_SNAPSHOT", "Delete VM snapshot" },
                    { "VPS2012", "DELETE_SNAPSHOT_SUBTREE", "Delete VM snapshot subtree" },
                    { "VPS2012", "EJECT_DVD_DISK", "Eject DVD disk" },
                    { "VPS2012", "INSERT_DVD_DISK", "Insert DVD disk" },
                    { "VPS2012", "REINSTALL", "Reinstall VM" },
                    { "VPS2012", "RENAME_SNAPSHOT", "Rename VM snapshot" },
                    { "VPS2012", "SET_PRIMARY_EXTERNAL_IP", "Set primary external IP" },
                    { "VPS2012", "SET_PRIMARY_PRIVATE_IP", "Set primary private IP" },
                    { "VPS2012", "TAKE_SNAPSHOT", "Take VM snapshot" },
                    { "VPS2012", "UPDATE_CONFIGURATION", "Update VM configuration" },
                    { "VPS2012", "UPDATE_HOSTNAME", "Update host name" },
                    { "WAG_INSTALLER", "GET_APP_PARAMS_TASK", "Get application parameters" },
                    { "WAG_INSTALLER", "GET_GALLERY_APP_DETAILS_TASK", "Get gallery application details" },
                    { "WAG_INSTALLER", "GET_GALLERY_APPS_TASK", "Get gallery applications" },
                    { "WAG_INSTALLER", "GET_GALLERY_CATEGORIES_TASK", "Get gallery categories" },
                    { "WAG_INSTALLER", "GET_SRV_GALLERY_APPS_TASK", "Get server gallery applications" },
                    { "WAG_INSTALLER", "INSTALL_WEB_APP", "Install Web application" },
                    { "WEB_SITE", "ADD", "Add" },
                    { "WEB_SITE", "ADD_POINTER", "Add domain pointer" },
                    { "WEB_SITE", "ADD_SSL_FOLDER", "Add shared SSL folder" },
                    { "WEB_SITE", "ADD_VDIR", "Add virtual directory" },
                    { "WEB_SITE", "CHANGE_FP_PASSWORD", "Change FrontPage account password" },
                    { "WEB_SITE", "CHANGE_STATE", "Change state" },
                    { "WEB_SITE", "DELETE", "Delete" },
                    { "WEB_SITE", "DELETE_POINTER", "Delete domain pointer" },
                    { "WEB_SITE", "DELETE_SECURED_FOLDER", "Delete secured folder" },
                    { "WEB_SITE", "DELETE_SECURED_GROUP", "Delete secured group" },
                    { "WEB_SITE", "DELETE_SECURED_USER", "Delete secured user" },
                    { "WEB_SITE", "DELETE_SSL_FOLDER", "Delete shared SSL folder" },
                    { "WEB_SITE", "DELETE_VDIR", "Delete virtual directory" },
                    { "WEB_SITE", "GET_STATE", "Get state" },
                    { "WEB_SITE", "INSTALL_FP", "Install FrontPage Extensions" },
                    { "WEB_SITE", "INSTALL_SECURED_FOLDERS", "Install secured folders" },
                    { "WEB_SITE", "UNINSTALL_FP", "Uninstall FrontPage Extensions" },
                    { "WEB_SITE", "UNINSTALL_SECURED_FOLDERS", "Uninstall secured folders" },
                    { "WEB_SITE", "UPDATE", "Update" },
                    { "WEB_SITE", "UPDATE_SECURED_FOLDER", "Add/update secured folder" },
                    { "WEB_SITE", "UPDATE_SECURED_GROUP", "Add/update secured group" },
                    { "WEB_SITE", "UPDATE_SECURED_USER", "Add/update secured user" },
                    { "WEB_SITE", "UPDATE_SSL_FOLDER", "Update shared SSL folder" },
                    { "WEB_SITE", "UPDATE_VDIR", "Update virtual directory" }
                });

            migrationBuilder.InsertData(
                table: "ResourceGroups",
                columns: new[] { "GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup" },
                values: new object[,]
                {
                    { 1, "FuseCP.EnterpriseServer.OperatingSystemController", "OS", 1, true },
                    { 2, "FuseCP.EnterpriseServer.WebServerController", "Web", 2, true },
                    { 3, "FuseCP.EnterpriseServer.FtpServerController", "FTP", 3, true },
                    { 4, "FuseCP.EnterpriseServer.MailServerController", "Mail", 4, true },
                    { 5, "FuseCP.EnterpriseServer.DatabaseServerController", "MsSQL2000", 7, true },
                    { 6, "FuseCP.EnterpriseServer.DatabaseServerController", "MySQL4", 11, true },
                    { 7, "FuseCP.EnterpriseServer.DnsServerController", "DNS", 17, true },
                    { 8, "FuseCP.EnterpriseServer.StatisticsServerController", "Statistics", 18, true },
                    { 10, "FuseCP.EnterpriseServer.DatabaseServerController", "MsSQL2005", 8, true },
                    { 11, "FuseCP.EnterpriseServer.DatabaseServerController", "MySQL5", 12, true },
                    { 12, null, "Exchange", 5, true },
                    { 13, null, "Hosted Organizations", 6, true },
                    { 20, "FuseCP.EnterpriseServer.HostedSharePointServerController", "Sharepoint Foundation Server", 14, true },
                    { 22, "FuseCP.EnterpriseServer.DatabaseServerController", "MsSQL2008", 9, true },
                    { 23, "FuseCP.EnterpriseServer.DatabaseServerController", "MsSQL2012", 10, true },
                    { 30, null, "VPS", 19, true },
                    { 31, null, "BlackBerry", 21, true },
                    { 32, null, "OCS", 22, true },
                    { 33, null, "VPS2012", 20, true },
                    { 40, null, "VPSForPC", 20, true },
                    { 41, null, "Lync", 24, true },
                    { 42, "FuseCP.EnterpriseServer.HeliconZooController", "HeliconZoo", 2, true },
                    { 44, "FuseCP.EnterpriseServer.EnterpriseStorageController", "EnterpriseStorage", 26, true },
                    { 45, null, "RDS", 27, true },
                    { 46, "FuseCP.EnterpriseServer.DatabaseServerController", "MsSQL2014", 10, true },
                    { 47, null, "Service Levels", 2, true },
                    { 49, "FuseCP.EnterpriseServer.StorageSpacesController", "StorageSpaceServices", 26, true },
                    { 50, "FuseCP.EnterpriseServer.DatabaseServerController", "MariaDB", 11, true },
                    { 52, null, "SfB", 26, true },
                    { 61, null, "MailFilters", 5, true },
                    { 71, "FuseCP.EnterpriseServer.DatabaseServerController", "MsSQL2016", 10, true },
                    { 72, "FuseCP.EnterpriseServer.DatabaseServerController", "MsSQL2017", 10, true },
                    { 73, "FuseCP.EnterpriseServer.HostedSharePointServerEntController", "Sharepoint Enterprise Server", 15, true },
                    { 74, "FuseCP.EnterpriseServer.DatabaseServerController", "MsSQL2019", 10, true },
                    { 75, "FuseCP.EnterpriseServer.DatabaseServerController", "MsSQL2022", 10, true },
                    { 90, "FuseCP.EnterpriseServer.DatabaseServerController", "MySQL8", 12, true },
                    { 167, null, "Proxmox", 20, true }
                });

            migrationBuilder.InsertData(
                table: "ScheduleTasks",
                columns: new[] { "TaskID", "RoleID", "TaskType" },
                values: new object[,]
                {
                    { "SCHEDULE_TASK_ACTIVATE_PAID_INVOICES", 0, "FuseCP.Ecommerce.EnterpriseServer.ActivatePaidInvoicesTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_AUDIT_LOG_REPORT", 3, "FuseCP.EnterpriseServer.AuditLogReportTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_BACKUP", 1, "FuseCP.EnterpriseServer.BackupTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_BACKUP_DATABASE", 3, "FuseCP.EnterpriseServer.BackupDatabaseTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_CALCULATE_EXCHANGE_DISKSPACE", 2, "FuseCP.EnterpriseServer.CalculateExchangeDiskspaceTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH", 1, "FuseCP.EnterpriseServer.CalculatePackagesBandwidthTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE", 1, "FuseCP.EnterpriseServer.CalculatePackagesDiskspaceTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_CANCEL_OVERDUE_INVOICES", 0, "FuseCP.Ecommerce.EnterpriseServer.CancelOverdueInvoicesTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_CHECK_WEBSITE", 3, "FuseCP.EnterpriseServer.CheckWebSiteTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_DELETE_EXCHANGE_ACCOUNTS", 3, "FuseCP.EnterpriseServer.DeleteExchangeAccountsTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_DOMAIN_EXPIRATION", 3, "FuseCP.EnterpriseServer.DomainExpirationTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_DOMAIN_LOOKUP", 1, "FuseCP.EnterpriseServer.DomainLookupViewTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_FTP_FILES", 3, "FuseCP.EnterpriseServer.FTPFilesTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_GENERATE_INVOICES", 0, "FuseCP.Ecommerce.EnterpriseServer.GenerateInvoicesTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_HOSTED_SOLUTION_REPORT", 2, "FuseCP.EnterpriseServer.HostedSolutionReportTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", 2, "FuseCP.EnterpriseServer.NotifyOverusedDatabasesTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_RUN_PAYMENT_QUEUE", 0, "FuseCP.Ecommerce.EnterpriseServer.RunPaymentQueueTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_RUN_SYSTEM_COMMAND", 1, "FuseCP.EnterpriseServer.RunSystemCommandTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_SEND_MAIL", 3, "FuseCP.EnterpriseServer.SendMailNotificationTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_SUSPEND_OVERDUE_INVOICES", 0, "FuseCP.Ecommerce.EnterpriseServer.SuspendOverdueInvoicesTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_SUSPEND_PACKAGES", 2, "FuseCP.EnterpriseServer.SuspendOverusedPackagesTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_USER_PASSWORD_EXPIRATION_NOTIFICATION", 1, "FuseCP.EnterpriseServer.UserPasswordExpirationNotificationTask, FuseCP.EnterpriseServer.Code" },
                    { "SCHEDULE_TASK_ZIP_FILES", 3, "FuseCP.EnterpriseServer.ZipFilesTask, FuseCP.EnterpriseServer.Code" }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "PropertyName", "SettingsName", "PropertyValue" },
                values: new object[,]
                {
                    { "AccessIps", "AccessIpsSettings", "" },
                    { "CanPeerChangeMfa", "AuthenticationSettings", "True" },
                    { "MfaTokenAppDisplayName", "AuthenticationSettings", "FuseCP" },
                    { "BackupsPath", "BackupSettings", "c:\\HostingBackups" },
                    { "SmtpEnableSsl", "SmtpSettings", "False" },
                    { "SmtpPort", "SmtpSettings", "25" },
                    { "SmtpServer", "SmtpSettings", "127.0.0.1" },
                    { "SmtpUsername", "SmtpSettings", "postmaster" }
                });

            migrationBuilder.InsertData(
                table: "ThemeSettings",
                columns: new[] { "ThemeSettingID", "PropertyName", "PropertyValue", "SettingsName", "ThemeID" },
                values: new object[,]
                {
                    { 1, "Light", "light-theme", "Style", 1 },
                    { 2, "Dark", "dark-theme", "Style", 1 },
                    { 3, "Semi Dark", "semi-dark", "Style", 1 },
                    { 4, "Minimal", "minimal-theme", "Style", 1 },
                    { 5, "#0727d7", "headercolor1", "color-header", 1 },
                    { 6, "#23282c", "headercolor2", "color-header", 1 },
                    { 7, "#e10a1f", "headercolor3", "color-header", 1 },
                    { 8, "#157d4c", "headercolor4", "color-header", 1 },
                    { 9, "#673ab7", "headercolor5", "color-header", 1 },
                    { 10, "#795548", "headercolor6", "color-header", 1 },
                    { 11, "#d3094e", "headercolor7", "color-header", 1 },
                    { 12, "#ff9800", "headercolor8", "color-header", 1 },
                    { 13, "#6c85ec", "sidebarcolor1", "color-Sidebar", 1 },
                    { 14, "#5b737f", "sidebarcolor2", "color-Sidebar", 1 },
                    { 15, "#408851", "sidebarcolor3", "color-Sidebar", 1 },
                    { 16, "#230924", "sidebarcolor4", "color-Sidebar", 1 },
                    { 17, "#903a85", "sidebarcolor5", "color-Sidebar", 1 },
                    { 18, "#a04846", "sidebarcolor6", "color-Sidebar", 1 },
                    { 19, "#a65314", "sidebarcolor7", "color-Sidebar", 1 },
                    { 20, "#1f0e3b", "sidebarcolor8", "color-Sidebar", 1 }
                });

            migrationBuilder.InsertData(
                table: "Themes",
                columns: new[] { "ThemeID", "DisplayName", "DisplayOrder", "Enabled", "LTRName", "RTLName" },
                values: new object[] { 1, "FuseCP v1", 1, 1, "Default", "Default" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "AdditionalParams", "Address", "Changed", "City", "Comments", "CompanyName", "Country", "Created", "EcommerceEnabled", "Email", "FailedLogins", "Fax", "FirstName", "HtmlMail", "InstantMessenger", "LastName", "LoginStatusId", "OneTimePasswordState", "OwnerID", "Password", "PinSecret", "PrimaryPhone", "RoleID", "SecondaryEmail", "SecondaryPhone", "State", "StatusID", "SubscriberNumber", "Username", "Zip" },
                values: new object[] { 1, null, "", new DateTime(2010, 7, 16, 10, 53, 2, 453, DateTimeKind.Utc), "", "", null, "", new DateTime(2010, 7, 16, 12, 53, 2, 453, DateTimeKind.Utc), true, "serveradmin@myhosting.com", null, "", "Enterprise", true, "", "Administrator", null, null, null, "", null, "", 1, "", "", "", 1, null, "serveradmin", "" });

            migrationBuilder.InsertData(
                table: "Versions",
                columns: new[] { "DatabaseVersion", "BuildDate" },
                values: new object[,]
                {
                    { "1.0", new DateTime(2010, 4, 9, 22, 0, 0, 0, DateTimeKind.Utc) },
                    { "1.0.1.0", new DateTime(2010, 7, 16, 10, 53, 3, 563, DateTimeKind.Utc) },
                    { "1.0.2.0", new DateTime(2010, 9, 2, 22, 0, 0, 0, DateTimeKind.Utc) },
                    { "1.1.0.9", new DateTime(2010, 11, 15, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { "1.1.2.13", new DateTime(2011, 4, 14, 22, 0, 0, 0, DateTimeKind.Utc) },
                    { "1.2.0.38", new DateTime(2011, 7, 12, 22, 0, 0, 0, DateTimeKind.Utc) },
                    { "1.2.1.6", new DateTime(2012, 3, 28, 22, 0, 0, 0, DateTimeKind.Utc) },
                    { "1.5.1", new DateTime(2024, 12, 16, 23, 0, 0, 0, DateTimeKind.Utc) },
                    { "2.0.0.228", new DateTime(2012, 12, 6, 23, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Packages",
                columns: new[] { "PackageID", "BandwidthUpdated", "PackageComments", "PackageName", "ParentPackageID", "PlanID", "PurchaseDate", "ServerID", "StatusID", "StatusIDchangeDate", "UserID" },
                values: new object[] { 1, null, "", "System", null, null, null, null, 1, new DateTime(2024, 12, 17, 12, 54, 59, 933, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 1, null, "Windows Server 2003", "Windows2003", 1, "Windows2003", "FuseCP.Providers.OS.Windows2003, FuseCP.Providers.OS.Windows2003" },
                    { 2, null, "Internet Information Services 6.0", "IIS60", 2, "IIS60", "FuseCP.Providers.Web.IIs60, FuseCP.Providers.Web.IIs60" },
                    { 3, null, "Microsoft FTP Server 6.0", "MSFTP60", 3, "MSFTP60", "FuseCP.Providers.FTP.MsFTP, FuseCP.Providers.FTP.IIs60" },
                    { 4, null, "MailEnable Server 1.x - 7.x", "MailEnable", 4, "MailEnable", "FuseCP.Providers.Mail.MailEnable, FuseCP.Providers.Mail.MailEnable" },
                    { 5, null, "Microsoft SQL Server 2000", "MSSQL", 5, "MSSQL", "FuseCP.Providers.Database.MsSqlServer, FuseCP.Providers.Database.SqlServer" },
                    { 6, null, "MySQL Server 4.x", "MySQL", 6, "MySQL", "FuseCP.Providers.Database.MySqlServer, FuseCP.Providers.Database.MySQL" },
                    { 7, null, "Microsoft DNS Server", "MSDNS", 7, "MSDNS", "FuseCP.Providers.DNS.MsDNS, FuseCP.Providers.DNS.MsDNS" },
                    { 8, null, "AWStats Statistics Service", "AWStats", 8, "AWStats", "FuseCP.Providers.Statistics.AWStats, FuseCP.Providers.Statistics.AWStats" },
                    { 9, null, "SimpleDNS Plus 4.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS, FuseCP.Providers.DNS.SimpleDNS" },
                    { 10, null, "SmarterStats 3.x", "SmarterStats", 8, "SmarterStats", "FuseCP.Providers.Statistics.SmarterStats, FuseCP.Providers.Statistics.SmarterStats" },
                    { 11, null, "SmarterMail 2.x", "SmarterMail", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail2, FuseCP.Providers.Mail.SmarterMail2" },
                    { 12, null, "Gene6 FTP Server 3.x", "Gene6FTP", 3, "Gene6FTP", "FuseCP.Providers.FTP.Gene6, FuseCP.Providers.FTP.Gene6" },
                    { 13, null, "Merak Mail Server 8.0.3 - 9.2.x", "Merak", 4, "Merak", "FuseCP.Providers.Mail.Merak, FuseCP.Providers.Mail.Merak" },
                    { 14, null, "SmarterMail 3.x - 4.x", "SmarterMail", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail3, FuseCP.Providers.Mail.SmarterMail3" },
                    { 16, null, "Microsoft SQL Server 2005", "MSSQL", 10, "MSSQL", "FuseCP.Providers.Database.MsSqlServer2005, FuseCP.Providers.Database.SqlServer" },
                    { 17, null, "MySQL Server 5.0", "MySQL", 11, "MySQL", "FuseCP.Providers.Database.MySqlServer50, FuseCP.Providers.Database.MySQL" },
                    { 18, null, "MDaemon 9.x - 11.x", "MDaemon", 4, "MDaemon", "FuseCP.Providers.Mail.MDaemon, FuseCP.Providers.Mail.MDaemon" },
                    { 19, true, "ArGoSoft Mail Server 1.x", "ArgoMail", 4, "ArgoMail", "FuseCP.Providers.Mail.ArgoMail, FuseCP.Providers.Mail.ArgoMail" },
                    { 20, null, "hMailServer 4.2", "hMailServer", 4, "hMailServer", "FuseCP.Providers.Mail.hMailServer, FuseCP.Providers.Mail.hMailServer" },
                    { 21, null, "Ability Mail Server 2.x", "AbilityMailServer", 4, "AbilityMailServer", "FuseCP.Providers.Mail.AbilityMailServer, FuseCP.Providers.Mail.AbilityMailServer" },
                    { 22, null, "hMailServer 4.3", "hMailServer43", 4, "hMailServer43", "FuseCP.Providers.Mail.hMailServer43, FuseCP.Providers.Mail.hMailServer43" },
                    { 24, null, "ISC BIND 8.x - 9.x", "Bind", 7, "Bind", "FuseCP.Providers.DNS.IscBind, FuseCP.Providers.DNS.Bind" },
                    { 25, null, "Serv-U FTP 6.x", "ServU", 3, "ServU", "FuseCP.Providers.FTP.ServU, FuseCP.Providers.FTP.ServU" },
                    { 26, null, "FileZilla FTP Server 0.9", "FileZilla", 3, "FileZilla", "FuseCP.Providers.FTP.FileZilla, FuseCP.Providers.FTP.FileZilla" },
                    { 27, null, "Hosted Microsoft Exchange Server 2007", "Exchange", 12, "Exchange2007", "FuseCP.Providers.HostedSolution.Exchange2007, FuseCP.Providers.HostedSolution" },
                    { 28, null, "SimpleDNS Plus 5.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS5, FuseCP.Providers.DNS.SimpleDNS50" },
                    { 29, null, "SmarterMail 5.x", "SmarterMail50", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail5, FuseCP.Providers.Mail.SmarterMail5" },
                    { 30, null, "MySQL Server 5.1", "MySQL", 11, "MySQL", "FuseCP.Providers.Database.MySqlServer51, FuseCP.Providers.Database.MySQL" },
                    { 31, null, "SmarterStats 4.x", "SmarterStats", 8, "SmarterStats", "FuseCP.Providers.Statistics.SmarterStats4, FuseCP.Providers.Statistics.SmarterStats" },
                    { 32, null, "Hosted Microsoft Exchange Server 2010", "Exchange", 12, "Exchange2010", "FuseCP.Providers.HostedSolution.Exchange2010, FuseCP.Providers.HostedSolution" },
                    { 56, true, "PowerDNS", "PowerDNS", 7, "PowerDNS", "FuseCP.Providers.DNS.PowerDNS, FuseCP.Providers.DNS.PowerDNS" },
                    { 60, null, "SmarterMail 6.x", "SmarterMail60", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail6, FuseCP.Providers.Mail.SmarterMail6" },
                    { 61, null, "Merak Mail Server 10.x", "Merak", 4, "Merak", "FuseCP.Providers.Mail.Merak10, FuseCP.Providers.Mail.Merak10" },
                    { 62, null, "SmarterStats 5.x +", "SmarterStats", 8, "SmarterStats", "FuseCP.Providers.Statistics.SmarterStats5, FuseCP.Providers.Statistics.SmarterStats" },
                    { 63, null, "hMailServer 5.x", "hMailServer5", 4, "hMailServer5", "FuseCP.Providers.Mail.hMailServer5, FuseCP.Providers.Mail.hMailServer5" },
                    { 64, null, "SmarterMail 7.x - 8.x", "SmarterMail60", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail7, FuseCP.Providers.Mail.SmarterMail7" },
                    { 65, null, "SmarterMail 9.x", "SmarterMail60", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail9, FuseCP.Providers.Mail.SmarterMail9" },
                    { 66, null, "SmarterMail 10.x +", "SmarterMail100", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail10, FuseCP.Providers.Mail.SmarterMail10" },
                    { 67, null, "SmarterMail 100.x +", "SmarterMail100x", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail100, FuseCP.Providers.Mail.SmarterMail100" },
                    { 90, null, "Hosted Microsoft Exchange Server 2010 SP2", "Exchange", 12, "Exchange2010SP2", "FuseCP.Providers.HostedSolution.Exchange2010SP2, FuseCP.Providers.HostedSolution" },
                    { 91, true, "Hosted Microsoft Exchange Server 2013", "Exchange", 12, "Exchange2013", "FuseCP.Providers.HostedSolution.Exchange2013, FuseCP.Providers.HostedSolution.Exchange2013" },
                    { 92, null, "Hosted Microsoft Exchange Server 2016", "Exchange", 12, "Exchange2016", "FuseCP.Providers.HostedSolution.Exchange2016, FuseCP.Providers.HostedSolution.Exchange2016" },
                    { 93, null, "Hosted Microsoft Exchange Server 2019", "Exchange", 12, "Exchange2016", "FuseCP.Providers.HostedSolution.Exchange2019, FuseCP.Providers.HostedSolution.Exchange2019" },
                    { 100, null, "Windows Server 2008", "Windows2008", 1, "Windows2008", "FuseCP.Providers.OS.Windows2008, FuseCP.Providers.OS.Windows2008" },
                    { 101, null, "Internet Information Services 7.0", "IIS70", 2, "IIS70", "FuseCP.Providers.Web.IIs70, FuseCP.Providers.Web.IIs70" },
                    { 102, null, "Microsoft FTP Server 7.0", "MSFTP70", 3, "MSFTP70", "FuseCP.Providers.FTP.MsFTP, FuseCP.Providers.FTP.IIs70" },
                    { 103, null, "Hosted Organizations", "Organizations", 13, "Organizations", "FuseCP.Providers.HostedSolution.OrganizationProvider, FuseCP.Providers.HostedSolution" },
                    { 104, null, "Windows Server 2012", "Windows2012", 1, "Windows2012", "FuseCP.Providers.OS.Windows2012, FuseCP.Providers.OS.Windows2012" },
                    { 105, null, "Internet Information Services 8.0", "IIS70", 2, "IIS80", "FuseCP.Providers.Web.IIs80, FuseCP.Providers.Web.IIs80" },
                    { 106, null, "Microsoft FTP Server 8.0", "MSFTP70", 3, "MSFTP80", "FuseCP.Providers.FTP.MsFTP80, FuseCP.Providers.FTP.IIs80" },
                    { 110, null, "Cerberus FTP Server 6.x", "CerberusFTP6", 3, "CerberusFTP6", "FuseCP.Providers.FTP.CerberusFTP6, FuseCP.Providers.FTP.CerberusFTP6" },
                    { 111, null, "Windows Server 2016", "Windows2008", 1, "Windows2016", "FuseCP.Providers.OS.Windows2016, FuseCP.Providers.OS.Windows2016" },
                    { 112, null, "Internet Information Services 10.0", "IIS70", 2, "IIS100", "FuseCP.Providers.Web.IIs100, FuseCP.Providers.Web.IIs100" },
                    { 113, null, "Microsoft FTP Server 10.0", "MSFTP70", 3, "MSFTP100", "FuseCP.Providers.FTP.MsFTP100, FuseCP.Providers.FTP.IIs100" },
                    { 135, true, "Web Application Engines", "HeliconZoo", 42, "HeliconZoo", "FuseCP.Providers.Web.HeliconZoo.HeliconZoo, FuseCP.Providers.Web.HeliconZoo" },
                    { 200, null, "Hosted Windows SharePoint Services 3.0", "HostedSharePoint30", 20, "HostedSharePoint30", "FuseCP.Providers.HostedSolution.HostedSharePointServer, FuseCP.Providers.HostedSolution" },
                    { 202, null, "Microsoft SQL Server 2008", "MSSQL", 22, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2008, FuseCP.Providers.Database.SqlServer" },
                    { 203, true, "BlackBerry 4.1", "BlackBerry", 31, "BlackBerry 4.1", "FuseCP.Providers.HostedSolution.BlackBerryProvider, FuseCP.Providers.HostedSolution" },
                    { 204, true, "BlackBerry 5.0", "BlackBerry5", 31, "BlackBerry 5.0", "FuseCP.Providers.HostedSolution.BlackBerry5Provider, FuseCP.Providers.HostedSolution" },
                    { 205, true, "Office Communications Server 2007 R2", "OCS", 32, "OCS", "FuseCP.Providers.HostedSolution.OCS2007R2, FuseCP.Providers.HostedSolution" },
                    { 206, true, "OCS Edge server", "OCS_Edge", 32, "OCSEdge", "FuseCP.Providers.HostedSolution.OCSEdge2007R2, FuseCP.Providers.HostedSolution" },
                    { 208, null, "Hosted SharePoint Foundation 2010", "HostedSharePoint30", 20, "HostedSharePoint2010", "FuseCP.Providers.HostedSolution.HostedSharePointServer2010, FuseCP.Providers.HostedSolution" },
                    { 250, null, "Microsoft Lync Server 2010 Multitenant Hosting Pack", "Lync", 41, "Lync2010", "FuseCP.Providers.HostedSolution.Lync2010, FuseCP.Providers.HostedSolution" },
                    { 300, true, "Microsoft Hyper-V", "HyperV", 30, "HyperV", "FuseCP.Providers.Virtualization.HyperV, FuseCP.Providers.Virtualization.HyperV" },
                    { 301, null, "MySQL Server 5.5", "MySQL", 11, "MySQL", "FuseCP.Providers.Database.MySqlServer55, FuseCP.Providers.Database.MySQL" },
                    { 302, null, "MySQL Server 5.6", "MySQL", 11, "MySQL", "FuseCP.Providers.Database.MySqlServer56, FuseCP.Providers.Database.MySQL" },
                    { 303, null, "MySQL Server 5.7", "MySQL", 11, "MySQL", "FuseCP.Providers.Database.MySqlServer57, FuseCP.Providers.Database.MySQL" },
                    { 304, null, "MySQL Server 8.0", "MySQL", 90, "MySQL", "FuseCP.Providers.Database.MySqlServer80, FuseCP.Providers.Database.MySQL" },
                    { 351, true, "Microsoft Hyper-V Virtual Machine Management", "HyperVvmm", 33, "HyperVvmm", "FuseCP.Providers.Virtualization.HyperVvmm, FuseCP.Providers.Virtualization.HyperVvmm" },
                    { 400, true, "Microsoft Hyper-V For Private Cloud", "HyperVForPrivateCloud", 40, "HyperVForPC", "FuseCP.Providers.VirtualizationForPC.HyperVForPC, FuseCP.Providers.VirtualizationForPC.HyperVForPC" },
                    { 410, null, "Microsoft DNS Server 2012+", "MSDNS", 7, "MSDNS.2012", "FuseCP.Providers.DNS.MsDNS2012, FuseCP.Providers.DNS.MsDNS2012" },
                    { 600, true, "Enterprise Storage Windows 2012", "EnterpriseStorage", 44, "EnterpriseStorage2012", "FuseCP.Providers.EnterpriseStorage.Windows2012, FuseCP.Providers.EnterpriseStorage.Windows2012" },
                    { 700, true, "Storage Spaces Windows 2012", "StorageSpaceServices", 49, "StorageSpace2012", "FuseCP.Providers.StorageSpaces.Windows2012, FuseCP.Providers.StorageSpaces.Windows2012" },
                    { 1203, null, "Microsoft SQL Server 2014", "MSSQL", 46, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2014, FuseCP.Providers.Database.SqlServer" },
                    { 1301, null, "Hosted SharePoint Foundation 2013", "HostedSharePoint30", 20, "HostedSharePoint2013", "FuseCP.Providers.HostedSolution.HostedSharePointServer2013, FuseCP.Providers.HostedSolution.SharePoint2013" },
                    { 1306, null, "Hosted SharePoint Foundation 2016", "HostedSharePoint30", 20, "HostedSharePoint2016", "FuseCP.Providers.HostedSolution.HostedSharePointServer2016, FuseCP.Providers.HostedSolution.SharePoint2016" },
                    { 1401, null, "Microsoft Lync Server 2013 Enterprise Edition", "Lync", 41, "Lync2013", "FuseCP.Providers.HostedSolution.Lync2013, FuseCP.Providers.HostedSolution.Lync2013" },
                    { 1402, null, "Microsoft Lync Server 2013 Multitenant Hosting Pack", "Lync", 41, "Lync2013HP", "FuseCP.Providers.HostedSolution.Lync2013HP, FuseCP.Providers.HostedSolution.Lync2013HP" },
                    { 1403, null, "Microsoft Skype for Business Server 2015", "SfB", 52, "SfB2015", "FuseCP.Providers.HostedSolution.SfB2015, FuseCP.Providers.HostedSolution.SfB2015" },
                    { 1404, null, "Microsoft Skype for Business Server 2019", "SfB", 52, "SfB2019", "FuseCP.Providers.HostedSolution.SfB2019, FuseCP.Providers.HostedSolution.SfB2019" },
                    { 1501, true, "Remote Desktop Services Windows 2012", "RDS", 45, "RemoteDesktopServices2012", "FuseCP.Providers.RemoteDesktopServices.Windows2012,FuseCP.Providers.RemoteDesktopServices.Windows2012" },
                    { 1502, true, "Remote Desktop Services Windows 2016", "RDS", 45, "RemoteDesktopServices2012", "FuseCP.Providers.RemoteDesktopServices.Windows2016,FuseCP.Providers.RemoteDesktopServices.Windows2016" },
                    { 1503, true, "Remote Desktop Services Windows 2019", "RDS", 45, "RemoteDesktopServices2019", "FuseCP.Providers.RemoteDesktopServices.Windows2019,FuseCP.Providers.RemoteDesktopServices.Windows2019" },
                    { 1504, true, "Remote Desktop Services Windows 2022", "RDS", 45, "RemoteDesktopServices2022", "FuseCP.Providers.RemoteDesktopServices.Windows2019,FuseCP.Providers.RemoteDesktopServices.Windows2019" },
                    { 1505, true, "Remote Desktop Services Windows 2025", "RDS", 45, "RemoteDesktopServices2025", "FuseCP.Providers.RemoteDesktopServices.Windows2025,FuseCP.Providers.RemoteDesktopServices.Windows2019" },
                    { 1550, null, "MariaDB 10.1", "MariaDB", 50, "MariaDB", "FuseCP.Providers.Database.MariaDB101, FuseCP.Providers.Database.MariaDB" },
                    { 1552, null, "Hosted SharePoint Enterprise 2013", "HostedSharePoint30", 73, "HostedSharePoint2013Ent", "FuseCP.Providers.HostedSolution.HostedSharePointServer2013Ent, FuseCP.Providers.HostedSolution.SharePoint2013Ent" },
                    { 1560, null, "MariaDB 10.2", "MariaDB", 50, "MariaDB", "FuseCP.Providers.Database.MariaDB102, FuseCP.Providers.Database.MariaDB" },
                    { 1570, true, "MariaDB 10.3", "MariaDB", 50, "MariaDB", "FuseCP.Providers.Database.MariaDB103, FuseCP.Providers.Database.MariaDB" },
                    { 1571, true, "MariaDB 10.4", "MariaDB", 50, "MariaDB", "FuseCP.Providers.Database.MariaDB104, FuseCP.Providers.Database.MariaDB" },
                    { 1572, true, "MariaDB 10.5", "MariaDB", 50, "MariaDB", "FuseCP.Providers.Database.MariaDB105, FuseCP.Providers.Database.MariaDB" },
                    { 1601, true, "Mail Cleaner", "MailCleaner", 61, "MailCleaner", "FuseCP.Providers.Filters.MailCleaner, FuseCP.Providers.Filters.MailCleaner" },
                    { 1602, true, "SpamExperts Mail Filter", "SpamExperts", 61, "SpamExperts", "FuseCP.Providers.Filters.SpamExperts, FuseCP.Providers.Filters.SpamExperts" },
                    { 1701, null, "Microsoft SQL Server 2016", "MSSQL", 71, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2016, FuseCP.Providers.Database.SqlServer" },
                    { 1702, null, "Hosted SharePoint Enterprise 2016", "HostedSharePoint30", 73, "HostedSharePoint2016Ent", "FuseCP.Providers.HostedSolution.HostedSharePointServer2016Ent, FuseCP.Providers.HostedSolution.SharePoint2016Ent" },
                    { 1703, null, "SimpleDNS Plus 6.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS6, FuseCP.Providers.DNS.SimpleDNS60" },
                    { 1704, true, "Microsoft SQL Server 2017", "MSSQL", 72, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2017, FuseCP.Providers.Database.SqlServer" },
                    { 1705, true, "Microsoft SQL Server 2019", "MSSQL", 74, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2019, FuseCP.Providers.Database.SqlServer" },
                    { 1706, null, "Microsoft SQL Server 2022", "MSSQL", 75, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2022, FuseCP.Providers.Database.SqlServer" },
                    { 1711, null, "Hosted SharePoint 2019", "HostedSharePoint30", 73, "HostedSharePoint2019", "FuseCP.Providers.HostedSolution.HostedSharePointServer2019, FuseCP.Providers.HostedSolution.SharePoint2019" },
                    { 1800, null, "Windows Server 2019", "Windows2012", 1, "Windows2019", "FuseCP.Providers.OS.Windows2019, FuseCP.Providers.OS.Windows2019" },
                    { 1801, true, "Microsoft Hyper-V 2019", "HyperV2012R2", 33, "HyperV2019", "FuseCP.Providers.Virtualization.HyperV2019, FuseCP.Providers.Virtualization.HyperV2019" },
                    { 1802, null, "Windows Server 2022", "Windows2012", 1, "Windows2022", "FuseCP.Providers.OS.Windows2022, FuseCP.Providers.OS.Windows2022" },
                    { 1803, true, "Microsoft Hyper-V 2022", "HyperV2012R2", 33, "HyperV2022", "FuseCP.Providers.Virtualization.HyperV2022, FuseCP.Providers.Virtualization.HyperV2022" },
                    { 1804, true, "Windows Server 2025", "Windows2012", 1, "Windows2025", "FuseCP.Providers.OS.Windows2025, FuseCP.Providers.OS.Windows2025" },
                    { 1805, true, "Microsoft Hyper-V 2025", "HyperV2012R2", 33, "HyperV2025", "FuseCP.Providers.Virtualization.HyperV2025, FuseCP.Providers.Virtualization.HyperV2025" },
                    { 1901, null, "SimpleDNS Plus 8.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS8, FuseCP.Providers.DNS.SimpleDNS80" },
                    { 1902, null, "Microsoft DNS Server 2016", "MSDNS", 7, "MSDNS.2016", "FuseCP.Providers.DNS.MsDNS2016, FuseCP.Providers.DNS.MsDNS2016" },
                    { 1903, null, "SimpleDNS Plus 9.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS9, FuseCP.Providers.DNS.SimpleDNS90" }
                });

            migrationBuilder.InsertData(
                table: "Quotas",
                columns: new[] { "QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota" },
                values: new object[,]
                {
                    { 25, 2, null, null, null, "ASP.NET 1.1", "Web.AspNet11", 3, 1, false },
                    { 26, 2, null, null, null, "ASP.NET 2.0", "Web.AspNet20", 4, 1, false },
                    { 27, 2, null, null, null, "ASP", "Web.Asp", 2, 1, false },
                    { 28, 2, null, null, null, "PHP 4.x", "Web.Php4", 5, 1, false },
                    { 29, 2, null, null, null, "PHP 5.x", "Web.Php5", 6, 1, false },
                    { 30, 2, null, null, null, "Perl", "Web.Perl", 7, 1, false },
                    { 31, 2, null, null, null, "Python", "Web.Python", 8, 1, false },
                    { 32, 2, null, null, null, "Virtual Directories", "Web.VirtualDirs", 9, 1, false },
                    { 33, 2, null, null, null, "FrontPage", "Web.FrontPage", 10, 1, false },
                    { 34, 2, null, null, null, "Custom Security Settings", "Web.Security", 11, 1, false },
                    { 35, 2, null, null, null, "Custom Default Documents", "Web.DefaultDocs", 12, 1, false },
                    { 36, 2, null, null, null, "Dedicated Application Pools", "Web.AppPools", 13, 1, false },
                    { 37, 2, null, null, null, "Custom Headers", "Web.Headers", 14, 1, false },
                    { 38, 2, null, null, null, "Custom Errors", "Web.Errors", 15, 1, false },
                    { 39, 2, null, null, null, "Custom MIME Types", "Web.Mime", 16, 1, false },
                    { 40, 4, null, null, null, "Max Mailbox Size", "Mail.MaxBoxSize", 2, 3, false },
                    { 41, 5, null, null, null, "Max Database Size", "MsSQL2000.MaxDatabaseSize", 3, 3, false },
                    { 42, 5, null, null, null, "Database Backups", "MsSQL2000.Backup", 5, 1, false },
                    { 43, 5, null, null, null, "Database Restores", "MsSQL2000.Restore", 6, 1, false },
                    { 44, 5, null, null, null, "Database Truncate", "MsSQL2000.Truncate", 7, 1, false },
                    { 45, 6, null, null, null, "Database Backups", "MySQL4.Backup", 4, 1, false },
                    { 48, 7, null, null, null, "DNS Editor", "DNS.Editor", 1, 1, false },
                    { 49, 4, null, null, null, "Max Group Recipients", "Mail.MaxGroupMembers", 5, 3, false },
                    { 50, 4, null, null, null, "Max List Recipients", "Mail.MaxListMembers", 7, 3, false },
                    { 51, 1, null, null, null, "Bandwidth, MB", "OS.Bandwidth", 2, 2, false },
                    { 52, 1, null, null, null, "Disk space, MB", "OS.Diskspace", 1, 2, false },
                    { 53, 1, null, null, null, "Domains", "OS.Domains", 3, 2, false },
                    { 54, 1, null, null, null, "Sub-Domains", "OS.SubDomains", 4, 2, false },
                    { 55, 1, null, null, null, "File Manager", "OS.FileManager", 6, 1, false },
                    { 57, 2, null, null, null, "CGI-BIN Folder", "Web.CgiBin", 8, 1, false },
                    { 58, 2, null, null, null, "Secured Folders", "Web.SecuredFolders", 8, 1, false },
                    { 60, 2, null, null, null, "Web Sites Redirection", "Web.Redirections", 8, 1, false },
                    { 61, 2, null, null, null, "Changing Sites Root Folders", "Web.HomeFolders", 8, 1, false },
                    { 64, 10, null, null, null, "Max Database Size", "MsSQL2005.MaxDatabaseSize", 3, 3, false },
                    { 65, 10, null, null, null, "Database Backups", "MsSQL2005.Backup", 5, 1, false },
                    { 66, 10, null, null, null, "Database Restores", "MsSQL2005.Restore", 6, 1, false },
                    { 67, 10, null, null, null, "Database Truncate", "MsSQL2005.Truncate", 7, 1, false },
                    { 70, 11, null, null, null, "Database Backups", "MySQL5.Backup", 4, 1, false },
                    { 71, 1, null, null, null, "Scheduled Tasks", "OS.ScheduledTasks", 9, 2, false },
                    { 72, 1, null, null, null, "Interval Tasks Allowed", "OS.ScheduledIntervalTasks", 10, 1, false },
                    { 73, 1, null, null, null, "Minimum Tasks Interval, minutes", "OS.MinimumTaskInterval", 11, 3, false },
                    { 74, 1, null, null, null, "Applications Installer", "OS.AppInstaller", 7, 1, false },
                    { 75, 1, null, null, null, "Extra Application Packs", "OS.ExtraApplications", 8, 1, false },
                    { 77, 12, null, null, 1, "Organization Disk Space, MB", "Exchange2007.DiskSpace", 2, 2, false },
                    { 78, 12, null, null, 1, "Mailboxes per Organization", "Exchange2007.Mailboxes", 3, 2, false },
                    { 79, 12, null, null, 1, "Contacts per Organization", "Exchange2007.Contacts", 4, 3, false },
                    { 80, 12, null, null, 1, "Distribution Lists per Organization", "Exchange2007.DistributionLists", 5, 3, false },
                    { 81, 12, null, null, 1, "Public Folders per Organization", "Exchange2007.PublicFolders", 6, 3, false },
                    { 83, 12, null, null, null, "POP3 Access", "Exchange2007.POP3Allowed", 9, 1, false },
                    { 84, 12, null, null, null, "IMAP Access", "Exchange2007.IMAPAllowed", 11, 1, false },
                    { 85, 12, null, null, null, "OWA/HTTP Access", "Exchange2007.OWAAllowed", 13, 1, false },
                    { 86, 12, null, null, null, "MAPI Access", "Exchange2007.MAPIAllowed", 15, 1, false },
                    { 87, 12, null, null, null, "ActiveSync Access", "Exchange2007.ActiveSyncAllowed", 17, 1, false },
                    { 88, 12, null, null, null, "Mail Enabled Public Folders Allowed", "Exchange2007.MailEnabledPublicFolders", 8, 1, false },
                    { 94, 2, null, null, null, "ColdFusion", "Web.ColdFusion", 17, 1, false },
                    { 95, 2, null, null, null, "Web Application Gallery", "Web.WebAppGallery", 1, 1, false },
                    { 96, 2, null, null, null, "ColdFusion Virtual Directories", "Web.CFVirtualDirectories", 18, 1, false },
                    { 97, 2, null, null, null, "Remote web management allowed", "Web.RemoteManagement", 20, 1, false },
                    { 100, 2, null, null, null, "Dedicated IP Addresses", "Web.IPAddresses", 19, 2, true },
                    { 102, 4, null, null, null, "Disable Mailbox Size Edit", "Mail.DisableSizeEdit", 8, 1, false },
                    { 103, 6, null, null, null, "Max Database Size", "MySQL4.MaxDatabaseSize", 3, 3, false },
                    { 104, 6, null, null, null, "Database Restores", "MySQL4.Restore", 5, 1, false },
                    { 105, 6, null, null, null, "Database Truncate", "MySQL4.Truncate", 6, 1, false },
                    { 106, 11, null, null, null, "Max Database Size", "MySQL5.MaxDatabaseSize", 3, 3, false },
                    { 107, 11, null, null, null, "Database Restores", "MySQL5.Restore", 5, 1, false },
                    { 108, 11, null, null, null, "Database Truncate", "MySQL5.Truncate", 6, 1, false },
                    { 112, 90, null, null, null, "Database Backups", "MySQL8.Backup", 4, 1, false },
                    { 113, 90, null, null, null, "Max Database Size", "MySQL8.MaxDatabaseSize", 3, 3, false },
                    { 114, 90, null, null, null, "Database Restores", "MySQL8.Restore", 5, 1, false },
                    { 115, 90, null, null, null, "Database Truncate", "MySQL8.Truncate", 6, 1, false },
                    { 203, 10, null, null, null, "Max Log Size", "MsSQL2005.MaxLogSize", 4, 3, false },
                    { 204, 5, null, null, null, "Max Log Size", "MsSQL2000.MaxLogSize", 4, 3, false },
                    { 207, 13, null, null, 1, "Domains per Organizations", "HostedSolution.Domains", 3, 3, false },
                    { 208, 20, null, null, null, "Max site storage, MB", "HostedSharePoint.MaxStorage", 2, 3, false },
                    { 213, 22, null, null, null, "Max Database Size", "MsSQL2008.MaxDatabaseSize", 3, 3, false },
                    { 214, 22, null, null, null, "Database Backups", "MsSQL2008.Backup", 5, 1, false },
                    { 215, 22, null, null, null, "Database Restores", "MsSQL2008.Restore", 6, 1, false },
                    { 216, 22, null, null, null, "Database Truncate", "MsSQL2008.Truncate", 7, 1, false },
                    { 217, 22, null, null, null, "Max Log Size", "MsSQL2008.MaxLogSize", 4, 3, false },
                    { 220, 1, true, null, null, "Domain Pointers", "OS.DomainPointers", 5, 2, false },
                    { 221, 23, null, null, null, "Max Database Size", "MsSQL2012.MaxDatabaseSize", 3, 3, false },
                    { 222, 23, null, null, null, "Database Backups", "MsSQL2012.Backup", 5, 1, false },
                    { 223, 23, null, null, null, "Database Restores", "MsSQL2012.Restore", 6, 1, false },
                    { 224, 23, null, null, null, "Database Truncate", "MsSQL2012.Truncate", 7, 1, false },
                    { 225, 23, null, null, null, "Max Log Size", "MsSQL2012.MaxLogSize", 4, 3, false },
                    { 230, 13, null, null, null, "Allow to Change UserPrincipalName", "HostedSolution.AllowChangeUPN", 4, 1, false },
                    { 301, 30, null, null, null, "Allow user to create VPS", "VPS.ManagingAllowed", 2, 1, false },
                    { 302, 30, null, null, null, "Number of CPU cores", "VPS.CpuNumber", 3, 2, false },
                    { 303, 30, null, null, null, "Boot from CD allowed", "VPS.BootCdAllowed", 7, 1, false },
                    { 304, 30, null, null, null, "Boot from CD", "VPS.BootCdEnabled", 8, 1, false },
                    { 305, 30, null, null, null, "RAM size, MB", "VPS.Ram", 4, 2, false },
                    { 306, 30, null, null, null, "Hard Drive size, GB", "VPS.Hdd", 5, 2, false },
                    { 307, 30, null, null, null, "DVD drive", "VPS.DvdEnabled", 6, 1, false },
                    { 308, 30, null, null, null, "External Network", "VPS.ExternalNetworkEnabled", 10, 1, false },
                    { 309, 30, null, null, null, "Number of External IP addresses", "VPS.ExternalIPAddressesNumber", 11, 2, false },
                    { 310, 30, null, null, null, "Private Network", "VPS.PrivateNetworkEnabled", 13, 1, false },
                    { 311, 30, null, null, null, "Number of Private IP addresses per VPS", "VPS.PrivateIPAddressesNumber", 14, 3, false },
                    { 312, 30, null, null, null, "Number of Snaphots", "VPS.SnapshotsNumber", 9, 3, false },
                    { 313, 30, null, null, null, "Allow user to Start, Turn off and Shutdown VPS", "VPS.StartShutdownAllowed", 15, 1, false },
                    { 314, 30, null, null, null, "Allow user to Pause, Resume VPS", "VPS.PauseResumeAllowed", 16, 1, false },
                    { 315, 30, null, null, null, "Allow user to Reboot VPS", "VPS.RebootAllowed", 17, 1, false },
                    { 316, 30, null, null, null, "Allow user to Reset VPS", "VPS.ResetAlowed", 18, 1, false },
                    { 317, 30, null, null, null, "Allow user to Re-install VPS", "VPS.ReinstallAllowed", 19, 1, false },
                    { 318, 30, null, null, null, "Monthly bandwidth, GB", "VPS.Bandwidth", 12, 2, false },
                    { 319, 31, null, null, 1, null, "BlackBerry.Users", 1, 2, false },
                    { 320, 32, null, null, 1, null, "OCS.Users", 1, 2, false },
                    { 321, 32, null, null, null, null, "OCS.Federation", 2, 1, false },
                    { 322, 32, null, null, null, null, "OCS.FederationByDefault", 3, 1, false },
                    { 323, 32, null, null, null, null, "OCS.PublicIMConnectivity", 4, 1, false },
                    { 324, 32, null, null, null, null, "OCS.PublicIMConnectivityByDefault", 5, 1, false },
                    { 325, 32, null, null, null, null, "OCS.ArchiveIMConversation", 6, 1, false },
                    { 326, 32, null, null, null, null, "OCS.ArchiveIMConvervationByDefault", 7, 1, false },
                    { 327, 32, null, null, null, null, "OCS.ArchiveFederatedIMConversation", 8, 1, false },
                    { 328, 32, null, null, null, null, "OCS.ArchiveFederatedIMConversationByDefault", 9, 1, false },
                    { 329, 32, null, null, null, null, "OCS.PresenceAllowed", 10, 1, false },
                    { 330, 32, null, null, null, null, "OCS.PresenceAllowedByDefault", 10, 1, false },
                    { 331, 2, null, null, null, "ASP.NET 4.0", "Web.AspNet40", 4, 1, false },
                    { 332, 2, null, null, null, "SSL", "Web.SSL", 21, 1, false },
                    { 333, 2, null, null, null, "Allow IP Address Mode Switch", "Web.AllowIPAddressModeSwitch", 22, 1, false },
                    { 334, 2, null, null, null, "Enable Hostname Support", "Web.EnableHostNameSupport", 23, 1, false },
                    { 344, 2, null, null, null, "htaccess", "Web.Htaccess", 9, 1, false },
                    { 346, 40, null, null, null, "Allow user to create VPS", "VPSForPC.ManagingAllowed", 2, 1, false },
                    { 347, 40, null, null, null, "Number of CPU cores", "VPSForPC.CpuNumber", 3, 2, false },
                    { 348, 40, null, null, null, "Boot from CD allowed", "VPSForPC.BootCdAllowed", 7, 1, false },
                    { 349, 40, null, null, null, "Boot from CD", "VPSForPC.BootCdEnabled", 7, 1, false },
                    { 350, 40, null, null, null, "RAM size, MB", "VPSForPC.Ram", 4, 2, false },
                    { 351, 40, null, null, null, "Hard Drive size, GB", "VPSForPC.Hdd", 5, 2, false },
                    { 352, 40, null, null, null, "DVD drive", "VPSForPC.DvdEnabled", 6, 1, false },
                    { 353, 40, null, null, null, "External Network", "VPSForPC.ExternalNetworkEnabled", 10, 1, false },
                    { 354, 40, null, null, null, "Number of External IP addresses", "VPSForPC.ExternalIPAddressesNumber", 11, 2, false },
                    { 355, 40, null, null, null, "Private Network", "VPSForPC.PrivateNetworkEnabled", 13, 1, false },
                    { 356, 40, null, null, null, "Number of Private IP addresses per VPS", "VPSForPC.PrivateIPAddressesNumber", 14, 3, false },
                    { 357, 40, null, null, null, "Number of Snaphots", "VPSForPC.SnapshotsNumber", 9, 3, false },
                    { 358, 40, null, null, null, "Allow user to Start, Turn off and Shutdown VPS", "VPSForPC.StartShutdownAllowed", 15, 1, false },
                    { 359, 40, null, null, null, "Allow user to Pause, Resume VPS", "VPSForPC.PauseResumeAllowed", 16, 1, false },
                    { 360, 40, null, null, null, "Allow user to Reboot VPS", "VPSForPC.RebootAllowed", 17, 1, false },
                    { 361, 40, null, null, null, "Allow user to Reset VPS", "VPSForPC.ResetAlowed", 18, 1, false },
                    { 362, 40, null, null, null, "Allow user to Re-install VPS", "VPSForPC.ReinstallAllowed", 19, 1, false },
                    { 363, 40, null, null, null, "Monthly bandwidth, GB", "VPSForPC.Bandwidth", 12, 2, false },
                    { 364, 12, null, null, null, "Keep Deleted Items (days)", "Exchange2007.KeepDeletedItemsDays", 19, 3, false },
                    { 365, 12, null, null, null, "Maximum Recipients", "Exchange2007.MaxRecipients", 20, 3, false },
                    { 366, 12, null, null, null, "Maximum Send Message Size (Kb)", "Exchange2007.MaxSendMessageSizeKB", 21, 3, false },
                    { 367, 12, null, null, null, "Maximum Receive Message Size (Kb)", "Exchange2007.MaxReceiveMessageSizeKB", 22, 3, false },
                    { 368, 12, null, null, null, "Is Consumer Organization", "Exchange2007.IsConsumer", 1, 1, false },
                    { 369, 12, null, null, null, "Enable Plans Editing", "Exchange2007.EnablePlansEditing", 23, 1, false },
                    { 370, 41, null, null, 1, "Users", "Lync.Users", 1, 2, false },
                    { 371, 41, null, null, null, "Allow Federation", "Lync.Federation", 2, 1, false },
                    { 372, 41, null, null, null, "Allow Conferencing", "Lync.Conferencing", 3, 1, false },
                    { 373, 41, null, null, null, "Maximum Conference Particiapants", "Lync.MaxParticipants", 4, 3, false },
                    { 374, 41, null, null, null, "Allow Video in Conference", "Lync.AllowVideo", 5, 1, false },
                    { 375, 41, null, null, null, "Allow EnterpriseVoice", "Lync.EnterpriseVoice", 6, 1, false },
                    { 376, 41, null, null, null, "Number of Enterprise Voice Users", "Lync.EVUsers", 7, 2, false },
                    { 377, 41, null, null, null, "Allow National Calls", "Lync.EVNational", 8, 1, false },
                    { 378, 41, null, null, null, "Allow Mobile Calls", "Lync.EVMobile", 9, 1, false },
                    { 379, 41, null, null, null, "Allow International Calls", "Lync.EVInternational", 10, 1, false },
                    { 380, 41, null, null, null, "Enable Plans Editing", "Lync.EnablePlansEditing", 11, 1, false },
                    { 400, 20, null, null, null, "Use shared SSL Root", "HostedSharePoint.UseSharedSSL", 3, 1, false },
                    { 409, 1, null, null, null, "Not allow Tenants to Delete Top Level Domains", "OS.NotAllowTenantDeleteDomains", 13, 1, false },
                    { 410, 1, null, null, null, "Not allow Tenants to Create Top Level Domains", "OS.NotAllowTenantCreateDomains", 12, 1, false },
                    { 411, 2, null, null, null, "Application Pools Restart", "Web.AppPoolsRestart", 13, 1, false },
                    { 420, 12, null, null, null, "Allow Litigation Hold", "Exchange2007.AllowLitigationHold", 24, 1, false },
                    { 421, 12, null, null, 1, "Recoverable Items Space", "Exchange2007.RecoverableItemsSpace", 25, 2, false },
                    { 422, 12, null, null, null, "Disclaimers Allowed", "Exchange2007.DisclaimersAllowed", 26, 1, false },
                    { 423, 13, null, null, 1, "Security Groups", "HostedSolution.SecurityGroups", 5, 2, false },
                    { 424, 12, null, null, null, "Allow Retention Policy", "Exchange2013.AllowRetentionPolicy", 27, 1, false },
                    { 425, 12, null, null, 1, "Archiving storage, MB", "Exchange2013.ArchivingStorage", 29, 2, false },
                    { 426, 12, null, null, 1, "Archiving Mailboxes per Organization", "Exchange2013.ArchivingMailboxes", 28, 2, false },
                    { 428, 12, null, null, 1, "Resource Mailboxes per Organization", "Exchange2013.ResourceMailboxes", 31, 2, false },
                    { 429, 12, null, null, 1, "Shared Mailboxes per Organization", "Exchange2013.SharedMailboxes", 30, 2, false },
                    { 430, 44, null, null, 1, "Disk Storage Space (Mb)", "EnterpriseStorage.DiskStorageSpace", 1, 2, false },
                    { 431, 44, null, null, 1, "Number of Root Folders", "EnterpriseStorage.Folders", 1, 2, false },
                    { 447, 61, null, null, null, "Enable Spam Filter", "Filters.Enable", 1, 1, false },
                    { 448, 61, null, null, null, "Enable Per-Mailbox Login", "Filters.EnableEmailUsers", 2, 1, false },
                    { 450, 45, null, null, 1, "Remote Desktop Users", "RDS.Users", 1, 2, false },
                    { 451, 45, null, null, 1, "Remote Desktop Servers", "RDS.Servers", 2, 2, false },
                    { 452, 45, null, null, null, "Disable user from adding server", "RDS.DisableUserAddServer", 3, 1, false },
                    { 453, 45, null, null, null, "Disable user from removing server", "RDS.DisableUserDeleteServer", 3, 1, false },
                    { 468, 45, null, null, null, "Use Drive Maps", "EnterpriseStorage.DriveMaps", 2, 1, false },
                    { 472, 46, null, null, null, "Max Database Size", "MsSQL2014.MaxDatabaseSize", 3, 3, false },
                    { 473, 46, null, null, null, "Database Backups", "MsSQL2014.Backup", 5, 1, false },
                    { 474, 46, null, null, null, "Database Restores", "MsSQL2014.Restore", 6, 1, false },
                    { 475, 46, null, null, null, "Database Truncate", "MsSQL2014.Truncate", 7, 1, false },
                    { 476, 46, null, null, null, "Max Log Size", "MsSQL2014.MaxLogSize", 4, 3, false },
                    { 491, 45, null, null, 1, "Remote Desktop Servers", "RDS.Collections", 2, 2, false },
                    { 495, 13, null, null, 1, "Deleted Users", "HostedSolution.DeletedUsers", 6, 2, false },
                    { 496, 13, null, null, 1, "Deleted Users Backup Storage Space, Mb", "HostedSolution.DeletedUsersBackupStorageSpace", 6, 2, false },
                    { 551, 73, null, null, null, "Max site storage, MB", "HostedSharePointEnterprise.MaxStorage", 2, 3, false },
                    { 552, 73, null, null, null, "Use shared SSL Root", "HostedSharePointEnterprise.UseSharedSSL", 3, 1, false },
                    { 554, 33, null, null, null, "Allow user to create VPS", "VPS2012.ManagingAllowed", 2, 1, false },
                    { 555, 33, null, null, null, "Number of CPU cores", "VPS2012.CpuNumber", 3, 2, false },
                    { 556, 33, null, null, null, "Boot from CD allowed", "VPS2012.BootCdAllowed", 7, 1, false },
                    { 557, 33, null, null, null, "Boot from CD", "VPS2012.BootCdEnabled", 8, 1, false },
                    { 558, 33, null, null, null, "RAM size, MB", "VPS2012.Ram", 4, 2, false },
                    { 559, 33, null, null, null, "Hard Drive size, GB", "VPS2012.Hdd", 5, 2, false },
                    { 560, 33, null, null, null, "DVD drive", "VPS2012.DvdEnabled", 6, 1, false },
                    { 561, 33, null, null, null, "External Network", "VPS2012.ExternalNetworkEnabled", 10, 1, false },
                    { 562, 33, null, null, null, "Number of External IP addresses", "VPS2012.ExternalIPAddressesNumber", 11, 2, false },
                    { 563, 33, null, null, null, "Private Network", "VPS2012.PrivateNetworkEnabled", 13, 1, false },
                    { 564, 33, null, null, null, "Number of Private IP addresses per VPS", "VPS2012.PrivateIPAddressesNumber", 14, 3, false },
                    { 565, 33, null, null, null, "Number of Snaphots", "VPS2012.SnapshotsNumber", 9, 3, false },
                    { 566, 33, null, null, null, "Allow user to Start, Turn off and Shutdown VPS", "VPS2012.StartShutdownAllowed", 15, 1, false },
                    { 567, 33, null, null, null, "Allow user to Pause, Resume VPS", "VPS2012.PauseResumeAllowed", 16, 1, false },
                    { 568, 33, null, null, null, "Allow user to Reboot VPS", "VPS2012.RebootAllowed", 17, 1, false },
                    { 569, 33, null, null, null, "Allow user to Reset VPS", "VPS2012.ResetAlowed", 18, 1, false },
                    { 570, 33, null, null, null, "Allow user to Re-install VPS", "VPS2012.ReinstallAllowed", 19, 1, false },
                    { 571, 33, null, null, null, "Monthly bandwidth, GB", "VPS2012.Bandwidth", 12, 2, false },
                    { 572, 33, null, null, null, "Allow user to Replication", "VPS2012.ReplicationEnabled", 20, 1, false },
                    { 575, 50, null, null, null, "Max Database Size", "MariaDB.MaxDatabaseSize", 3, 3, false },
                    { 576, 50, null, null, null, "Database Backups", "MariaDB.Backup", 5, 1, false },
                    { 577, 50, null, null, null, "Database Restores", "MariaDB.Restore", 6, 1, false },
                    { 578, 50, null, null, null, "Database Truncate", "MariaDB.Truncate", 7, 1, false },
                    { 579, 50, null, null, null, "Max Log Size", "MariaDB.MaxLogSize", 4, 3, false },
                    { 581, 52, null, null, null, "Phone Numbers", "SfB.PhoneNumbers", 12, 2, false },
                    { 582, 52, null, null, 1, "Users", "SfB.Users", 1, 2, false },
                    { 583, 52, null, null, null, "Allow Federation", "SfB.Federation", 2, 1, false },
                    { 584, 52, null, null, null, "Allow Conferencing", "SfB.Conferencing", 3, 1, false },
                    { 585, 52, null, null, null, "Maximum Conference Particiapants", "SfB.MaxParticipants", 4, 3, false },
                    { 586, 52, null, null, null, "Allow Video in Conference", "SfB.AllowVideo", 5, 1, false },
                    { 587, 52, null, null, null, "Allow EnterpriseVoice", "SfB.EnterpriseVoice", 6, 1, false },
                    { 588, 52, null, null, null, "Number of Enterprise Voice Users", "SfB.EVUsers", 7, 2, false },
                    { 589, 52, null, null, null, "Allow National Calls", "SfB.EVNational", 8, 1, false },
                    { 590, 52, null, null, null, "Allow Mobile Calls", "SfB.EVMobile", 9, 1, false },
                    { 591, 52, null, null, null, "Allow International Calls", "SfB.EVInternational", 10, 1, false },
                    { 592, 52, null, null, null, "Enable Plans Editing", "SfB.EnablePlansEditing", 11, 1, false },
                    { 674, 167, null, null, null, "Allow user to create VPS", "PROXMOX.ManagingAllowed", 2, 1, false },
                    { 675, 167, null, null, null, "Number of CPU cores", "PROXMOX.CpuNumber", 3, 3, false },
                    { 676, 167, null, null, null, "Boot from CD allowed", "PROXMOX.BootCdAllowed", 7, 1, false },
                    { 677, 167, null, null, null, "Boot from CD", "PROXMOX.BootCdEnabled", 8, 1, false },
                    { 678, 167, null, null, null, "RAM size, MB", "PROXMOX.Ram", 4, 2, false },
                    { 679, 167, null, null, null, "Hard Drive size, GB", "PROXMOX.Hdd", 5, 2, false },
                    { 680, 167, null, null, null, "DVD drive", "PROXMOX.DvdEnabled", 6, 1, false },
                    { 681, 167, null, null, null, "External Network", "PROXMOX.ExternalNetworkEnabled", 10, 1, false },
                    { 682, 167, null, null, null, "Number of External IP addresses", "PROXMOX.ExternalIPAddressesNumber", 11, 2, false },
                    { 683, 167, null, null, null, "Private Network", "PROXMOX.PrivateNetworkEnabled", 13, 1, false },
                    { 684, 167, null, null, null, "Number of Private IP addresses per VPS", "PROXMOX.PrivateIPAddressesNumber", 14, 3, false },
                    { 685, 167, null, null, null, "Number of Snaphots", "PROXMOX.SnapshotsNumber", 9, 3, false },
                    { 686, 167, null, null, null, "Allow user to Start, Turn off and Shutdown VPS", "PROXMOX.StartShutdownAllowed", 15, 1, false },
                    { 687, 167, null, null, null, "Allow user to Pause, Resume VPS", "PROXMOX.PauseResumeAllowed", 16, 1, false },
                    { 688, 167, null, null, null, "Allow user to Reboot VPS", "PROXMOX.RebootAllowed", 17, 1, false },
                    { 689, 167, null, null, null, "Allow user to Reset VPS", "PROXMOX.ResetAlowed", 18, 1, false },
                    { 690, 167, null, null, null, "Allow user to Re-install VPS", "PROXMOX.ReinstallAllowed", 19, 1, false },
                    { 691, 167, null, null, null, "Monthly bandwidth, GB", "PROXMOX.Bandwidth", 12, 2, false },
                    { 692, 167, null, null, null, "Allow user to Replication", "PROXMOX.ReplicationEnabled", 20, 1, false },
                    { 703, 71, null, null, null, "Max Database Size", "MsSQL2016.MaxDatabaseSize", 3, 3, false },
                    { 704, 71, null, null, null, "Database Backups", "MsSQL2016.Backup", 5, 1, false },
                    { 705, 71, null, null, null, "Database Restores", "MsSQL2016.Restore", 6, 1, false },
                    { 706, 71, null, null, null, "Database Truncate", "MsSQL2016.Truncate", 7, 1, false },
                    { 707, 71, null, null, null, "Max Log Size", "MsSQL2016.MaxLogSize", 4, 3, false },
                    { 713, 72, null, null, null, "Max Database Size", "MsSQL2017.MaxDatabaseSize", 3, 3, false },
                    { 714, 72, null, null, null, "Database Backups", "MsSQL2017.Backup", 5, 1, false },
                    { 715, 72, null, null, null, "Database Restores", "MsSQL2017.Restore", 6, 1, false },
                    { 716, 72, null, null, null, "Database Truncate", "MsSQL2017.Truncate", 7, 1, false },
                    { 717, 72, null, null, null, "Max Log Size", "MsSQL2017.MaxLogSize", 4, 3, false },
                    { 723, 74, null, null, null, "Max Database Size", "MsSQL2019.MaxDatabaseSize", 3, 3, false },
                    { 724, 74, null, null, null, "Database Backups", "MsSQL2019.Backup", 5, 1, false },
                    { 725, 74, null, null, null, "Database Restores", "MsSQL2019.Restore", 6, 1, false },
                    { 726, 74, null, null, null, "Database Truncate", "MsSQL2019.Truncate", 7, 1, false },
                    { 727, 74, null, null, null, "Max Log Size", "MsSQL2019.MaxLogSize", 4, 3, false },
                    { 728, 33, null, null, null, "Number of Private Network VLANs", "VPS2012.PrivateVLANsNumber", 14, 2, false },
                    { 729, 12, null, null, null, "Automatic Replies via FuseCP Allowed", "Exchange2013.AutoReply", 32, 1, false },
                    { 730, 33, null, null, null, "Additional Hard Drives per VPS", "VPS2012.AdditionalVhdCount", 6, 3, false },
                    { 731, 12, null, null, 1, "Journaling Mailboxes per Organization", "Exchange2013.JournalingMailboxes", 31, 2, false },
                    { 734, 75, null, null, null, "Max Database Size", "MsSQL2022.MaxDatabaseSize", 3, 3, false },
                    { 735, 75, null, null, null, "Database Backups", "MsSQL2022.Backup", 5, 1, false },
                    { 736, 75, null, null, null, "Database Restores", "MsSQL2022.Restore", 6, 1, false },
                    { 737, 75, null, null, null, "Database Truncate", "MsSQL2022.Truncate", 7, 1, false },
                    { 738, 75, null, null, null, "Max Log Size", "MsSQL2022.MaxLogSize", 4, 3, false },
                    { 750, 33, null, null, null, "DMZ Network", "VPS2012.DMZNetworkEnabled", 22, 1, false },
                    { 751, 33, null, null, null, "Number of DMZ IP addresses per VPS", "VPS2012.DMZIPAddressesNumber", 23, 3, false },
                    { 752, 33, null, null, null, "Number of DMZ Network VLANs", "VPS2012.DMZVLANsNumber", 24, 2, false },
                    { 753, 7, null, null, null, "Allow editing TTL in DNS Editor", "DNS.EditTTL", 2, 1, false }
                });

            migrationBuilder.InsertData(
                table: "ResourceGroupDnsRecords",
                columns: new[] { "RecordID", "GroupID", "MXPriority", "RecordData", "RecordName", "RecordOrder", "RecordType" },
                values: new object[,]
                {
                    { 1, 2, 0, "[IP]", "", 1, "A" },
                    { 2, 2, 0, "[IP]", "*", 2, "A" },
                    { 3, 2, 0, "[IP]", "www", 3, "A" },
                    { 4, 3, 0, "[IP]", "ftp", 1, "A" },
                    { 5, 4, 0, "[IP]", "mail", 1, "A" },
                    { 6, 4, 0, "[IP]", "mail2", 2, "A" },
                    { 7, 4, 10, "mail.[DOMAIN_NAME]", "", 3, "MX" },
                    { 9, 4, 21, "mail2.[DOMAIN_NAME]", "", 4, "MX" },
                    { 10, 5, 0, "[IP]", "mssql", 1, "A" },
                    { 11, 6, 0, "[IP]", "mysql", 1, "A" },
                    { 12, 8, 0, "[IP]", "stats", 1, "A" },
                    { 13, 4, 0, "v=spf1 a mx -all", "", 5, "TXT" },
                    { 14, 12, 0, "[IP]", "smtp", 1, "A" },
                    { 15, 12, 10, "smtp.[DOMAIN_NAME]", "", 2, "MX" },
                    { 16, 12, 0, "", "autodiscover", 3, "CNAME" },
                    { 17, 12, 0, "", "owa", 4, "CNAME" }
                });

            migrationBuilder.InsertData(
                table: "ScheduleTaskParameters",
                columns: new[] { "ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder" },
                values: new object[,]
                {
                    { "AUDIT_LOG_DATE", "SCHEDULE_TASK_AUDIT_LOG_REPORT", "List", "today=Today;yesterday=Yesterday;schedule=Schedule", 5 },
                    { "AUDIT_LOG_SEVERITY", "SCHEDULE_TASK_AUDIT_LOG_REPORT", "List", "-1=All;0=Information;1=Warning;2=Error", 2 },
                    { "AUDIT_LOG_SOURCE", "SCHEDULE_TASK_AUDIT_LOG_REPORT", "List", "", 3 },
                    { "AUDIT_LOG_TASK", "SCHEDULE_TASK_AUDIT_LOG_REPORT", "List", "", 4 },
                    { "MAIL_TO", "SCHEDULE_TASK_AUDIT_LOG_REPORT", "String", null, 1 },
                    { "SHOW_EXECUTION_LOG", "SCHEDULE_TASK_AUDIT_LOG_REPORT", "List", "0=No;1=Yes", 6 },
                    { "BACKUP_FILE_NAME", "SCHEDULE_TASK_BACKUP", "String", "", 1 },
                    { "DELETE_TEMP_BACKUP", "SCHEDULE_TASK_BACKUP", "Boolean", "true", 1 },
                    { "STORE_PACKAGE_FOLDER", "SCHEDULE_TASK_BACKUP", "String", "\\", 1 },
                    { "STORE_PACKAGE_ID", "SCHEDULE_TASK_BACKUP", "String", "", 1 },
                    { "STORE_SERVER_FOLDER", "SCHEDULE_TASK_BACKUP", "String", "", 1 },
                    { "BACKUP_FOLDER", "SCHEDULE_TASK_BACKUP_DATABASE", "String", "\\backups", 3 },
                    { "BACKUP_NAME", "SCHEDULE_TASK_BACKUP_DATABASE", "String", "database_backup.bak", 4 },
                    { "DATABASE_GROUP", "SCHEDULE_TASK_BACKUP_DATABASE", "List", "MsSQL2014=SQL Server 2014;MsSQL2016=SQL Server 2016;MsSQL2017=SQL Server 2017;MsSQL2019=SQL Server 2019;MsSQL2022=SQL Server 2022;MySQL5=MySQL 5.0;MariaDB=MariaDB", 1 },
                    { "DATABASE_NAME", "SCHEDULE_TASK_BACKUP_DATABASE", "String", "", 2 },
                    { "ZIP_BACKUP", "SCHEDULE_TASK_BACKUP_DATABASE", "List", "true=Yes;false=No", 5 },
                    { "MAIL_BODY", "SCHEDULE_TASK_CHECK_WEBSITE", "MultiString", "", 10 },
                    { "MAIL_FROM", "SCHEDULE_TASK_CHECK_WEBSITE", "String", "admin@mysite.com", 7 },
                    { "MAIL_SUBJECT", "SCHEDULE_TASK_CHECK_WEBSITE", "String", "Web Site is unavailable", 9 },
                    { "MAIL_TO", "SCHEDULE_TASK_CHECK_WEBSITE", "String", "admin@mysite.com", 8 },
                    { "PASSWORD", "SCHEDULE_TASK_CHECK_WEBSITE", "String", null, 3 },
                    { "RESPONSE_CONTAIN", "SCHEDULE_TASK_CHECK_WEBSITE", "String", null, 5 },
                    { "RESPONSE_DOESNT_CONTAIN", "SCHEDULE_TASK_CHECK_WEBSITE", "String", null, 6 },
                    { "RESPONSE_STATUS", "SCHEDULE_TASK_CHECK_WEBSITE", "String", "500", 4 },
                    { "URL", "SCHEDULE_TASK_CHECK_WEBSITE", "String", "http://", 1 },
                    { "USE_RESPONSE_CONTAIN", "SCHEDULE_TASK_CHECK_WEBSITE", "Boolean", "false", 1 },
                    { "USE_RESPONSE_DOESNT_CONTAIN", "SCHEDULE_TASK_CHECK_WEBSITE", "Boolean", "false", 1 },
                    { "USE_RESPONSE_STATUS", "SCHEDULE_TASK_CHECK_WEBSITE", "Boolean", "false", 1 },
                    { "USERNAME", "SCHEDULE_TASK_CHECK_WEBSITE", "String", null, 2 },
                    { "DAYS_BEFORE", "SCHEDULE_TASK_DOMAIN_EXPIRATION", "String", null, 1 },
                    { "ENABLE_NOTIFICATION", "SCHEDULE_TASK_DOMAIN_EXPIRATION", "Boolean", "false", 3 },
                    { "INCLUDE_NONEXISTEN_DOMAINS", "SCHEDULE_TASK_DOMAIN_EXPIRATION", "Boolean", "false", 4 },
                    { "MAIL_TO", "SCHEDULE_TASK_DOMAIN_EXPIRATION", "String", null, 2 },
                    { "DNS_SERVERS", "SCHEDULE_TASK_DOMAIN_LOOKUP", "String", null, 1 },
                    { "MAIL_TO", "SCHEDULE_TASK_DOMAIN_LOOKUP", "String", null, 2 },
                    { "PAUSE_BETWEEN_QUERIES", "SCHEDULE_TASK_DOMAIN_LOOKUP", "String", "100", 4 },
                    { "SERVER_NAME", "SCHEDULE_TASK_DOMAIN_LOOKUP", "String", "", 3 },
                    { "FILE_PATH", "SCHEDULE_TASK_FTP_FILES", "String", "\\", 1 },
                    { "FTP_FOLDER", "SCHEDULE_TASK_FTP_FILES", "String", null, 5 },
                    { "FTP_PASSWORD", "SCHEDULE_TASK_FTP_FILES", "String", null, 4 },
                    { "FTP_SERVER", "SCHEDULE_TASK_FTP_FILES", "String", "ftp.myserver.com", 2 },
                    { "FTP_USERNAME", "SCHEDULE_TASK_FTP_FILES", "String", null, 3 },
                    { "EMAIL", "SCHEDULE_TASK_HOSTED_SOLUTION_REPORT", "String", null, 1 },
                    { "EXCHANGE_REPORT", "SCHEDULE_TASK_HOSTED_SOLUTION_REPORT", "Boolean", "true", 2 },
                    { "LYNC_REPORT", "SCHEDULE_TASK_HOSTED_SOLUTION_REPORT", "Boolean", "true", 4 },
                    { "ORGANIZATION_REPORT", "SCHEDULE_TASK_HOSTED_SOLUTION_REPORT", "Boolean", "true", 7 },
                    { "SFB_REPORT", "SCHEDULE_TASK_HOSTED_SOLUTION_REPORT", "Boolean", "true", 5 },
                    { "SHAREPOINT_REPORT", "SCHEDULE_TASK_HOSTED_SOLUTION_REPORT", "Boolean", "true", 3 },
                    { "MARIADB_OVERUSED", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "Boolean", "true", 1 },
                    { "MSSQL_OVERUSED", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "Boolean", "true", 1 },
                    { "MYSQL_OVERUSED", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "Boolean", "true", 1 },
                    { "OVERUSED_MAIL_BCC", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "String", "", 1 },
                    { "OVERUSED_MAIL_BODY", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "String", "", 1 },
                    { "OVERUSED_MAIL_FROM", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "String", "", 1 },
                    { "OVERUSED_MAIL_SUBJECT", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "String", "", 1 },
                    { "OVERUSED_USAGE_THRESHOLD", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "String", "100", 1 },
                    { "SEND_OVERUSED_EMAIL", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "Boolean", "true", 1 },
                    { "SEND_WARNING_EMAIL", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "Boolean", "true", 1 },
                    { "WARNING_MAIL_BCC", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "String", "", 1 },
                    { "WARNING_MAIL_BODY", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "String", "", 1 },
                    { "WARNING_MAIL_FROM", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "String", "", 1 },
                    { "WARNING_MAIL_SUBJECT", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "String", "", 1 },
                    { "WARNING_USAGE_THRESHOLD", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "String", "80", 1 },
                    { "EXECUTABLE_PARAMS", "SCHEDULE_TASK_RUN_SYSTEM_COMMAND", "String", "", 3 },
                    { "EXECUTABLE_PATH", "SCHEDULE_TASK_RUN_SYSTEM_COMMAND", "String", "Executable.exe", 2 },
                    { "SERVER_NAME", "SCHEDULE_TASK_RUN_SYSTEM_COMMAND", "String", null, 1 },
                    { "MAIL_BODY", "SCHEDULE_TASK_SEND_MAIL", "MultiString", null, 4 },
                    { "MAIL_FROM", "SCHEDULE_TASK_SEND_MAIL", "String", null, 1 },
                    { "MAIL_SUBJECT", "SCHEDULE_TASK_SEND_MAIL", "String", null, 3 },
                    { "MAIL_TO", "SCHEDULE_TASK_SEND_MAIL", "String", null, 2 },
                    { "BANDWIDTH_OVERUSED", "SCHEDULE_TASK_SUSPEND_PACKAGES", "Boolean", "true", 1 },
                    { "DISKSPACE_OVERUSED", "SCHEDULE_TASK_SUSPEND_PACKAGES", "Boolean", "true", 1 },
                    { "SEND_SUSPENSION_EMAIL", "SCHEDULE_TASK_SUSPEND_PACKAGES", "Boolean", "true", 1 },
                    { "SEND_WARNING_EMAIL", "SCHEDULE_TASK_SUSPEND_PACKAGES", "Boolean", "true", 1 },
                    { "SUSPEND_OVERUSED", "SCHEDULE_TASK_SUSPEND_PACKAGES", "Boolean", "true", 1 },
                    { "SUSPENSION_MAIL_BCC", "SCHEDULE_TASK_SUSPEND_PACKAGES", "String", null, 1 },
                    { "SUSPENSION_MAIL_BODY", "SCHEDULE_TASK_SUSPEND_PACKAGES", "String", null, 1 },
                    { "SUSPENSION_MAIL_FROM", "SCHEDULE_TASK_SUSPEND_PACKAGES", "String", null, 1 },
                    { "SUSPENSION_MAIL_SUBJECT", "SCHEDULE_TASK_SUSPEND_PACKAGES", "String", null, 1 },
                    { "SUSPENSION_USAGE_THRESHOLD", "SCHEDULE_TASK_SUSPEND_PACKAGES", "String", "100", 1 },
                    { "WARNING_MAIL_BCC", "SCHEDULE_TASK_SUSPEND_PACKAGES", "String", null, 1 },
                    { "WARNING_MAIL_BODY", "SCHEDULE_TASK_SUSPEND_PACKAGES", "String", null, 1 },
                    { "WARNING_MAIL_FROM", "SCHEDULE_TASK_SUSPEND_PACKAGES", "String", null, 1 },
                    { "WARNING_MAIL_SUBJECT", "SCHEDULE_TASK_SUSPEND_PACKAGES", "String", null, 1 },
                    { "WARNING_USAGE_THRESHOLD", "SCHEDULE_TASK_SUSPEND_PACKAGES", "String", "80", 1 },
                    { "DAYS_BEFORE_EXPIRATION", "SCHEDULE_TASK_USER_PASSWORD_EXPIRATION_NOTIFICATION", "String", null, 1 },
                    { "FOLDER", "SCHEDULE_TASK_ZIP_FILES", "String", null, 1 },
                    { "ZIP_FILE", "SCHEDULE_TASK_ZIP_FILES", "String", "\\archive.zip", 2 }
                });

            migrationBuilder.InsertData(
                table: "ScheduleTaskViewConfiguration",
                columns: new[] { "ConfigurationID", "TaskID", "Description", "Environment" },
                values: new object[,]
                {
                    { "ASP_NET", "SCHEDULE_TASK_ACTIVATE_PAID_INVOICES", "~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_AUDIT_LOG_REPORT", "~/DesktopModules/FuseCP/ScheduleTaskControls/AuditLogReportView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_BACKUP", "~/DesktopModules/FuseCP/ScheduleTaskControls/Backup.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_BACKUP_DATABASE", "~/DesktopModules/FuseCP/ScheduleTaskControls/BackupDatabase.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_CALCULATE_EXCHANGE_DISKSPACE", "~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH", "~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE", "~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_CANCEL_OVERDUE_INVOICES", "~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_CHECK_WEBSITE", "~/DesktopModules/FuseCP/ScheduleTaskControls/CheckWebsite.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_DOMAIN_EXPIRATION", "~/DesktopModules/FuseCP/ScheduleTaskControls/DomainExpirationView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_DOMAIN_LOOKUP", "~/DesktopModules/FuseCP/ScheduleTaskControls/DomainLookupView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_FTP_FILES", "~/DesktopModules/FuseCP/ScheduleTaskControls/SendFilesViaFtp.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_GENERATE_INVOICES", "~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_HOSTED_SOLUTION_REPORT", "~/DesktopModules/FuseCP/ScheduleTaskControls/HostedSolutionReport.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_NOTIFY_OVERUSED_DATABASES", "~/DesktopModules/FuseCP/ScheduleTaskControls/NotifyOverusedDatabases.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_RUN_PAYMENT_QUEUE", "~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_RUN_SYSTEM_COMMAND", "~/DesktopModules/FuseCP/ScheduleTaskControls/ExecuteSystemCommand.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_SEND_MAIL", "~/DesktopModules/FuseCP/ScheduleTaskControls/SendEmailNotification.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_SUSPEND_OVERDUE_INVOICES", "~/DesktopModules/FuseCP/ScheduleTaskControls/EmptyView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_SUSPEND_PACKAGES", "~/DesktopModules/FuseCP/ScheduleTaskControls/SuspendOverusedSpaces.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_USER_PASSWORD_EXPIRATION_NOTIFICATION", "~/DesktopModules/FuseCP/ScheduleTaskControls/UserPasswordExpirationNotificationView.ascx", "ASP.NET" },
                    { "ASP_NET", "SCHEDULE_TASK_ZIP_FILES", "~/DesktopModules/FuseCP/ScheduleTaskControls/ZipFiles.ascx", "ASP.NET" }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[] { 2, true, false, true, "HomeFolder", true, 1, false, false, "FuseCP.Providers.OS.HomeFolder, FuseCP.Providers.Base", 15 });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 5, true, false, true, "MsSQL2000Database", true, 5, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 9 },
                    { 6, true, false, false, "MsSQL2000User", true, 5, true, true, true, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 10 },
                    { 7, true, false, true, "MySQL4Database", true, 6, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 13 },
                    { 8, true, false, false, "MySQL4User", true, 6, true, true, false, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 14 },
                    { 9, true, true, false, "FTPAccount", true, 3, true, true, true, "FuseCP.Providers.FTP.FtpAccount, FuseCP.Providers.Base", 3 },
                    { 10, true, true, true, "WebSite", true, 2, true, true, true, "FuseCP.Providers.Web.WebSite, FuseCP.Providers.Base", 2 },
                    { 11, true, true, false, "MailDomain", true, 4, true, true, true, "FuseCP.Providers.Mail.MailDomain, FuseCP.Providers.Base", 8 }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName" },
                values: new object[] { 12, true, false, false, "DNSZone", true, 7, true, false, true, "FuseCP.Providers.DNS.DnsZone, FuseCP.Providers.Base" });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[] { 13, false, false, "Domain", false, 1, true, false, "FuseCP.Providers.OS.Domain, FuseCP.Providers.Base", 1 });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[] { 14, true, false, false, "StatisticsSite", true, 8, true, true, false, "FuseCP.Providers.Statistics.StatsSite, FuseCP.Providers.Base", 17 });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 15, false, true, "MailAccount", false, 4, true, false, "FuseCP.Providers.Mail.MailAccount, FuseCP.Providers.Base", 4 },
                    { 16, false, false, "MailAlias", false, 4, true, false, "FuseCP.Providers.Mail.MailAlias, FuseCP.Providers.Base", 5 },
                    { 17, false, false, "MailList", false, 4, true, false, "FuseCP.Providers.Mail.MailList, FuseCP.Providers.Base", 7 },
                    { 18, false, false, "MailGroup", false, 4, true, false, "FuseCP.Providers.Mail.MailGroup, FuseCP.Providers.Base", 6 }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 20, true, false, false, "ODBCDSN", true, 1, true, true, false, "FuseCP.Providers.OS.SystemDSN, FuseCP.Providers.Base", 22 },
                    { 21, true, false, true, "MsSQL2005Database", true, 10, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 11 },
                    { 22, true, false, false, "MsSQL2005User", true, 10, true, true, true, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 12 },
                    { 23, true, false, true, "MySQL5Database", true, 11, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 15 },
                    { 24, true, false, false, "MySQL5User", true, 11, true, true, false, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 16 }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[] { 25, true, false, false, "SharedSSLFolder", true, 2, true, false, "FuseCP.Providers.Web.SharedSSLFolder, FuseCP.Providers.Base", 21 });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName" },
                values: new object[] { 28, true, false, false, "SecondaryDNSZone", true, 7, false, true, "FuseCP.Providers.DNS.SecondaryDnsZone, FuseCP.Providers.Base" });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 29, true, false, true, "Organization", true, 13, true, true, "FuseCP.Providers.HostedSolution.Organization, FuseCP.Providers.Base", 1 },
                    { 30, true, null, null, "OrganizationDomain", null, 13, null, null, "FuseCP.Providers.HostedSolution.OrganizationDomain, FuseCP.Providers.Base", 1 }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 31, true, false, true, "MsSQL2008Database", true, 22, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 1 },
                    { 32, true, false, false, "MsSQL2008User", true, 22, true, true, true, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 1 }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 33, false, false, "VirtualMachine", true, 30, true, true, "FuseCP.Providers.Virtualization.VirtualMachine, FuseCP.Providers.Base", 1 },
                    { 34, false, false, "VirtualSwitch", true, 30, true, true, "FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base", 2 },
                    { 35, false, false, "VMInfo", true, 40, true, true, "FuseCP.Providers.Virtualization.VMInfo, FuseCP.Providers.Base", 1 },
                    { 36, false, false, "VirtualSwitch", true, 40, true, true, "FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base", 2 }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 37, true, false, true, "MsSQL2012Database", true, 23, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 1 },
                    { 38, true, false, false, "MsSQL2012User", true, 23, true, true, true, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 1 },
                    { 39, true, false, true, "MsSQL2014Database", true, 46, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 1 },
                    { 40, true, false, false, "MsSQL2014User", true, 46, true, true, false, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 1 }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 41, false, false, "VirtualMachine", true, 33, true, true, "FuseCP.Providers.Virtualization.VirtualMachine, FuseCP.Providers.Base", 1 },
                    { 42, false, false, "VirtualSwitch", true, 33, true, true, "FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base", 2 }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 71, true, false, true, "MsSQL2016Database", true, 71, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 1 },
                    { 72, true, false, false, "MsSQL2016User", true, 71, true, true, false, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 1 },
                    { 73, true, false, true, "MsSQL2017Database", true, 72, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 1 },
                    { 74, true, false, false, "MsSQL2017User", true, 72, true, true, false, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 1 },
                    { 75, true, false, true, "MySQL8Database", true, 90, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 18 },
                    { 76, true, false, false, "MySQL8User", true, 90, true, true, false, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 19 },
                    { 77, true, false, true, "MsSQL2019Database", true, 74, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 1 },
                    { 78, true, false, false, "MsSQL2019User", true, 74, true, true, false, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 1 },
                    { 79, true, false, true, "MsSQL2022Database", true, 75, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 1 },
                    { 80, true, false, false, "MsSQL2022User", true, 75, true, true, false, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 1 }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 143, false, false, "VirtualMachine", true, 167, true, true, "FuseCP.Providers.Virtualization.VirtualMachine, FuseCP.Providers.Base", 1 },
                    { 144, false, false, "VirtualSwitch", true, 167, true, true, "FuseCP.Providers.Virtualization.VirtualSwitch, FuseCP.Providers.Base", 2 }
                });

            migrationBuilder.InsertData(
                table: "ServiceItemTypes",
                columns: new[] { "ItemTypeID", "Backupable", "CalculateBandwidth", "CalculateDiskspace", "DisplayName", "Disposable", "GroupID", "Importable", "Searchable", "Suspendable", "TypeName", "TypeOrder" },
                values: new object[,]
                {
                    { 200, true, false, true, "SharePointFoundationSiteCollection", true, 20, true, true, false, "FuseCP.Providers.SharePoint.SharePointSiteCollection, FuseCP.Providers.Base", 25 },
                    { 202, true, false, true, "MariaDBDatabase", true, 50, true, true, false, "FuseCP.Providers.Database.SqlDatabase, FuseCP.Providers.Base", 1 },
                    { 203, true, false, false, "MariaDBUser", true, 50, true, true, false, "FuseCP.Providers.Database.SqlUser, FuseCP.Providers.Base", 1 },
                    { 204, true, false, true, "SharePointEnterpriseSiteCollection", true, 73, true, true, false, "FuseCP.Providers.SharePoint.SharePointEnterpriseSiteCollection, FuseCP.Providers.Base", 100 }
                });

            migrationBuilder.InsertData(
                table: "UserSettings",
                columns: new[] { "PropertyName", "SettingsName", "UserID", "PropertyValue" },
                values: new object[,]
                {
                    { "CC", "AccountSummaryLetter", 1, "support@HostingCompany.com" },
                    { "EnableLetter", "AccountSummaryLetter", 1, "False" },
                    { "From", "AccountSummaryLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "AccountSummaryLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>Account Summary Information</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; }\r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n\r\n<a name=\"top\"></a>\r\n<div class=\"Header\">\r\n	Hosting Account Information\r\n</div>\r\n\r\n<ad:if test=\"#Signup#\">\r\n<p>\r\nHello #user.FirstName#,\r\n</p>\r\n\r\n<p>\r\nNew user account has been created and below you can find its summary information.\r\n</p>\r\n\r\n<h1>Control Panel URL</h1>\r\n<table>\r\n    <thead>\r\n        <tr>\r\n            <th>Control Panel URL</th>\r\n            <th>Username</th>\r\n            <th>Password</th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n        <tr>\r\n            <td><a href=\"http://panel.HostingCompany.com\">http://panel.HostingCompany.com</a></td>\r\n            <td>#user.Username#</td>\r\n            <td>#user.Password#</td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n</ad:if>\r\n\r\n<h1>Hosting Spaces</h1>\r\n<p>\r\n    The following hosting spaces have been created under your account:\r\n</p>\r\n<ad:foreach collection=\"#Spaces#\" var=\"Space\" index=\"i\">\r\n<h2>#Space.PackageName#</h2>\r\n<table>\r\n	<tbody>\r\n		<tr>\r\n			<td class=\"Label\">Hosting Plan:</td>\r\n			<td>\r\n				<ad:if test=\"#not(isnull(Plans[Space.PlanId]))#\">#Plans[Space.PlanId].PlanName#<ad:else>System</ad:if>\r\n			</td>\r\n		</tr>\r\n		<ad:if test=\"#not(isnull(Plans[Space.PlanId]))#\">\r\n		<tr>\r\n			<td class=\"Label\">Purchase Date:</td>\r\n			<td>\r\n				#Space.PurchaseDate#\r\n			</td>\r\n		</tr>\r\n		<tr>\r\n			<td class=\"Label\">Disk Space, MB:</td>\r\n			<td><ad:NumericQuota space=\"#SpaceContexts[Space.PackageId]#\" quota=\"OS.Diskspace\" /></td>\r\n		</tr>\r\n		<tr>\r\n			<td class=\"Label\">Bandwidth, MB/Month:</td>\r\n			<td><ad:NumericQuota space=\"#SpaceContexts[Space.PackageId]#\" quota=\"OS.Bandwidth\" /></td>\r\n		</tr>\r\n		<tr>\r\n			<td class=\"Label\">Maximum Number of Domains:</td>\r\n			<td><ad:NumericQuota space=\"#SpaceContexts[Space.PackageId]#\" quota=\"OS.Domains\" /></td>\r\n		</tr>\r\n		<tr>\r\n			<td class=\"Label\">Maximum Number of Sub-Domains:</td>\r\n			<td><ad:NumericQuota space=\"#SpaceContexts[Space.PackageId]#\" quota=\"OS.SubDomains\" /></td>\r\n		</tr>\r\n		</ad:if>\r\n	</tbody>\r\n</table>\r\n</ad:foreach>\r\n\r\n<ad:if test=\"#Signup#\">\r\n<p>\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n</p>\r\n\r\n<p>\r\nBest regards,<br />\r\nFuseCP.<br />\r\nWeb Site: <a href=\"https://fusecp.com\">https://fusecp.com</a><br />\r\nE-Mail: <a href=\"mailto:support@fusecp.com\">support@fusecp.com</a>\r\n</p>\r\n</ad:if>\r\n\r\n<ad:template name=\"NumericQuota\">\r\n	<ad:if test=\"#space.Quotas.ContainsKey(quota)#\">\r\n		<ad:if test=\"#space.Quotas[quota].QuotaAllocatedValue isnot -1#\">#space.Quotas[quota].QuotaAllocatedValue#<ad:else>Unlimited</ad:if>\r\n	<ad:else>\r\n		0\r\n	</ad:if>\r\n</ad:template>\r\n\r\n</div>\r\n</body>\r\n</html>" },
                    { "Priority", "AccountSummaryLetter", 1, "Normal" },
                    { "Subject", "AccountSummaryLetter", 1, "<ad:if test=\"#Signup#\">FuseCP  account has been created for<ad:else>FuseCP  account summary for</ad:if> #user.FirstName# #user.LastName#" },
                    { "TextBody", "AccountSummaryLetter", 1, "=================================\r\n   Hosting Account Information\r\n=================================\r\n<ad:if test=\"#Signup#\">Hello #user.FirstName#,\r\n\r\nNew user account has been created and below you can find its summary information.\r\n\r\nControl Panel URL: https://panel.fusecp.com\r\nUsername: #user.Username#\r\nPassword: #user.Password#\r\n</ad:if>\r\n\r\nHosting Spaces\r\n==============\r\nThe following hosting spaces have been created under your account:\r\n\r\n<ad:foreach collection=\"#Spaces#\" var=\"Space\" index=\"i\">\r\n=== #Space.PackageName# ===\r\nHosting Plan: <ad:if test=\"#not(isnull(Plans[Space.PlanId]))#\">#Plans[Space.PlanId].PlanName#<ad:else>System</ad:if>\r\n<ad:if test=\"#not(isnull(Plans[Space.PlanId]))#\">Purchase Date: #Space.PurchaseDate#\r\nDisk Space, MB: <ad:NumericQuota space=\"#SpaceContexts[Space.PackageId]#\" quota=\"OS.Diskspace\" />\r\nBandwidth, MB/Month: <ad:NumericQuota space=\"#SpaceContexts[Space.PackageId]#\" quota=\"OS.Bandwidth\" />\r\nMaximum Number of Domains: <ad:NumericQuota space=\"#SpaceContexts[Space.PackageId]#\" quota=\"OS.Domains\" />\r\nMaximum Number of Sub-Domains: <ad:NumericQuota space=\"#SpaceContexts[Space.PackageId]#\" quota=\"OS.SubDomains\" />\r\n</ad:if>\r\n</ad:foreach>\r\n\r\n<ad:if test=\"#Signup#\">If you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n\r\nBest regards,\r\nFuseCP.\r\nWeb Site: https://fusecp.com\">\r\nE-Mail: support@fusecp.com\r\n</ad:if><ad:template name=\"NumericQuota\"><ad:if test=\"#space.Quotas.ContainsKey(quota)#\"><ad:if test=\"#space.Quotas[quota].QuotaAllocatedValue isnot -1#\">#space.Quotas[quota].QuotaAllocatedValue#<ad:else>Unlimited</ad:if><ad:else>0</ad:if></ad:template>" },
                    { "Transform", "BandwidthXLST", 1, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">\r\n<xsl:template match=\"/\">\r\n  <html>\r\n  <body>\r\n  <img alt=\"Embedded Image\" width=\"299\" height=\"60\" src=\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASsAAAA8CAYAAAA+AJwjAAAACXBIWXMAAAsTAAALEwEAmpwYAAA4G2lUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4KPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS41LWMwMTQgNzkuMTUxNDgxLCAyMDEzLzAzLzEzLTEyOjA5OjE1ICAgICAgICAiPgogICA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIKICAgICAgICAgICAgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIgogICAgICAgICAgICB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIKICAgICAgICAgICAgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIj4KICAgICAgICAgPHhtcDpDcmVhdG9yVG9vbD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC94bXA6Q3JlYXRvclRvb2w+CiAgICAgICAgIDx4bXA6Q3JlYXRlRGF0ZT4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC94bXA6Q3JlYXRlRGF0ZT4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTYtMDMtMDFUMTQ6NTE6NTgrMDE6MDA8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOk1ldGFkYXRhRGF0ZT4yMDE2LTAzLTAxVDE0OjUxOjU4KzAxOjAwPC94bXA6TWV0YWRhdGFEYXRlPgogICAgICAgICA8ZGM6Zm9ybWF0PmltYWdlL3BuZzwvZGM6Zm9ybWF0PgogICAgICAgICA8cGhvdG9zaG9wOkNvbG9yTW9kZT4zPC9waG90b3Nob3A6Q29sb3JNb2RlPgogICAgICAgICA8eG1wTU06SW5zdGFuY2VJRD54bXAuaWlkOjZhNTdmMWYyLTgyZjYtMjk0MS1hYjFmLTNkOWQ0YjdmMTY2YjwveG1wTU06SW5zdGFuY2VJRD4KICAgICAgICAgPHhtcE1NOkRvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOkRvY3VtZW50SUQ+CiAgICAgICAgIDx4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD4KICAgICAgICAgPHhtcE1NOkhpc3Rvcnk+CiAgICAgICAgICAgIDxyZGY6U2VxPgogICAgICAgICAgICAgICA8cmRmOmxpIHJkZjpwYXJzZVR5cGU9IlJlc291cmNlIj4KICAgICAgICAgICAgICAgICAgPHN0RXZ0OmFjdGlvbj5jcmVhdGVkPC9zdEV2dDphY3Rpb24+CiAgICAgICAgICAgICAgICAgIDxzdEV2dDppbnN0YW5jZUlEPnhtcC5paWQ6NmE1N2YxZjItODJmNi0yOTQxLWFiMWYtM2Q5ZDRiN2YxNjZiPC9zdEV2dDppbnN0YW5jZUlEPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6d2hlbj4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC9zdEV2dDp3aGVuPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6c29mdHdhcmVBZ2VudD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC9zdEV2dDpzb2Z0d2FyZUFnZW50PgogICAgICAgICAgICAgICA8L3JkZjpsaT4KICAgICAgICAgICAgPC9yZGY6U2VxPgogICAgICAgICA8L3htcE1NOkhpc3Rvcnk+CiAgICAgICAgIDx0aWZmOk9yaWVudGF0aW9uPjE8L3RpZmY6T3JpZW50YXRpb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyMDAwMC8xMDAwMDwvdGlmZjpYUmVzb2x1dGlvbj4KICAgICAgICAgPHRpZmY6WVJlc29sdXRpb24+NzIwMDAwLzEwMDAwPC90aWZmOllSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpSZXNvbHV0aW9uVW5pdD4yPC90aWZmOlJlc29sdXRpb25Vbml0PgogICAgICAgICA8ZXhpZjpDb2xvclNwYWNlPjY1NTM1PC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWERpbWVuc2lvbj4yOTk8L2V4aWY6UGl4ZWxYRGltZW5zaW9uPgogICAgICAgICA8ZXhpZjpQaXhlbFlEaW1lbnNpb24+NjA8L2V4aWY6UGl4ZWxZRGltZW5zaW9uPgogICAgICA8L3JkZjpEZXNjcmlwdGlvbj4KICAgPC9yZGY6UkRGPgo8L3g6eG1wbWV0YT4KICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAg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\" />\r\n  <h2>Bandwidth Report</h2>\r\n  <table border=\"1\">\r\n    <tr bgcolor=\"#66ccff\">\r\n		<th>PackageID</th>\r\n        <th>QuotaValue</th>\r\n        <th>Diskspace</th>\r\n        <th>UsagePercentage</th>\r\n        <th>PackageName</th>\r\n        <th>PackagesNumber</th>\r\n        <th>StatusID</th>\r\n        <th>UserID</th>\r\n      <th>Username</th>\r\n        <th>FirstName</th>\r\n        <th>LastName</th>\r\n        <th>FullName</th>\r\n        <th>RoleID</th>\r\n        <th>Email</th>\r\n        <th>UserComments</th> \r\n    </tr>\r\n    <xsl:for-each select=\"//Table1\">\r\n    <tr>\r\n	<td><xsl:value-of select=\"PackageID\"/></td>\r\n        <td><xsl:value-of select=\"QuotaValue\"/></td>\r\n        <td><xsl:value-of select=\"Diskspace\"/></td>\r\n        <td><xsl:value-of select=\"UsagePercentage\"/>%</td>\r\n        <td><xsl:value-of select=\"PackageName\"/></td>\r\n        <td><xsl:value-of select=\"PackagesNumber\"/></td>\r\n        <td><xsl:value-of select=\"StatusID\"/></td>\r\n        <td><xsl:value-of select=\"UserID\"/></td>\r\n      <td><xsl:value-of select=\"Username\"/></td>\r\n        <td><xsl:value-of select=\"FirstName\"/></td>\r\n        <td><xsl:value-of select=\"LastName\"/></td>\r\n        <td><xsl:value-of select=\"FullName\"/></td>\r\n        <td><xsl:value-of select=\"RoleID\"/></td>\r\n        <td><xsl:value-of select=\"Email\"/></td>\r\n        <td><xsl:value-of select=\"UserComments\"/></td>\r\n    </tr>\r\n    </xsl:for-each>\r\n  </table>\r\n  </body>\r\n  </html>\r\n</xsl:template>\r\n</xsl:stylesheet>" },
                    { "TransformContentType", "BandwidthXLST", 1, "test/html" },
                    { "TransformSuffix", "BandwidthXLST", 1, ".htm" },
                    { "Transform", "DiskspaceXLST", 1, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">\r\n<xsl:template match=\"/\">\r\n  <html>\r\n  <body>\r\n  <img alt=\"Embedded Image\" width=\"299\" height=\"60\" src=\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASsAAAA8CAYAAAA+AJwjAAAACXBIWXMAAAsTAAALEwEAmpwYAAA4G2lUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4KPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS41LWMwMTQgNzkuMTUxNDgxLCAyMDEzLzAzLzEzLTEyOjA5OjE1ICAgICAgICAiPgogICA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIKICAgICAgICAgICAgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIgogICAgICAgICAgICB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIKICAgICAgICAgICAgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIj4KICAgICAgICAgPHhtcDpDcmVhdG9yVG9vbD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC94bXA6Q3JlYXRvclRvb2w+CiAgICAgICAgIDx4bXA6Q3JlYXRlRGF0ZT4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC94bXA6Q3JlYXRlRGF0ZT4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTYtMDMtMDFUMTQ6NTE6NTgrMDE6MDA8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOk1ldGFkYXRhRGF0ZT4yMDE2LTAzLTAxVDE0OjUxOjU4KzAxOjAwPC94bXA6TWV0YWRhdGFEYXRlPgogICAgICAgICA8ZGM6Zm9ybWF0PmltYWdlL3BuZzwvZGM6Zm9ybWF0PgogICAgICAgICA8cGhvdG9zaG9wOkNvbG9yTW9kZT4zPC9waG90b3Nob3A6Q29sb3JNb2RlPgogICAgICAgICA8eG1wTU06SW5zdGFuY2VJRD54bXAuaWlkOjZhNTdmMWYyLTgyZjYtMjk0MS1hYjFmLTNkOWQ0YjdmMTY2YjwveG1wTU06SW5zdGFuY2VJRD4KICAgICAgICAgPHhtcE1NOkRvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOkRvY3VtZW50SUQ+CiAgICAgICAgIDx4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ+eG1wLmRpZDo2YTU3ZjFmMi04MmY2LTI5NDEtYWIxZi0zZDlkNGI3ZjE2NmI8L3htcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD4KICAgICAgICAgPHhtcE1NOkhpc3Rvcnk+CiAgICAgICAgICAgIDxyZGY6U2VxPgogICAgICAgICAgICAgICA8cmRmOmxpIHJkZjpwYXJzZVR5cGU9IlJlc291cmNlIj4KICAgICAgICAgICAgICAgICAgPHN0RXZ0OmFjdGlvbj5jcmVhdGVkPC9zdEV2dDphY3Rpb24+CiAgICAgICAgICAgICAgICAgIDxzdEV2dDppbnN0YW5jZUlEPnhtcC5paWQ6NmE1N2YxZjItODJmNi0yOTQxLWFiMWYtM2Q5ZDRiN2YxNjZiPC9zdEV2dDppbnN0YW5jZUlEPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6d2hlbj4yMDE2LTAzLTAxVDE0OjUwOjQzKzAxOjAwPC9zdEV2dDp3aGVuPgogICAgICAgICAgICAgICAgICA8c3RFdnQ6c29mdHdhcmVBZ2VudD5BZG9iZSBQaG90b3Nob3AgQ0MgKFdpbmRvd3MpPC9zdEV2dDpzb2Z0d2FyZUFnZW50PgogICAgICAgICAgICAgICA8L3JkZjpsaT4KICAgICAgICAgICAgPC9yZGY6U2VxPgogICAgICAgICA8L3htcE1NOkhpc3Rvcnk+CiAgICAgICAgIDx0aWZmOk9yaWVudGF0aW9uPjE8L3RpZmY6T3JpZW50YXRpb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyMDAwMC8xMDAwMDwvdGlmZjpYUmVzb2x1dGlvbj4KICAgICAgICAgPHRpZmY6WVJlc29sdXRpb24+NzIwMDAwLzEwMDAwPC90aWZmOllSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpSZXNvbHV0aW9uVW5pdD4yPC90aWZmOlJlc29sdXRpb25Vbml0PgogICAgICAgICA8ZXhpZjpDb2xvclNwYWNlPjY1NTM1PC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWERpbWVuc2lvbj4yOTk8L2V4aWY6UGl4ZWxYRGltZW5zaW9uPgogICAgICAgICA8ZXhpZjpQaXhlbFlEaW1lbnNpb24+NjA8L2V4aWY6UGl4ZWxZRGltZW5zaW9uPgogICAgICA8L3JkZjpEZXNjcmlwdGlvbj4KICAgPC9yZGY6UkRGPgo8L3g6eG1wbWV0YT4KICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAKPD94cGFja2V0IGVuZD0idyI/Pq+oh5kAAAAgY0hSTQAAeiUAAICDAAD5/wAAgOkAAHUwAADqYAAAOpgAABdvkl/FRgAALU1JREFUeNrsnXmcnFWV97/neZ6q6jXd2XcI+76DoqACKgq4gPsAggvIoDjovOq4oDMqCu7buOAuKg4iyCCCG4uyCI4CYQlJCIGErJ1O70tVPc+95/3j3uqurtTWSTpErZPP80lVddXz3PV3zzn3d86Vh+YtoSABIP7/QGTsNf5z/OfF74VxUbYVG4TM6e1i9dw9Oeb7D2A0gO5eCIPir4VAB3AAsAQ4ApgDzAfaAQP0ARuAx/21GlgH5KglxoKWKV0UQv8gbNoMYehqYP1lDFjrXieJu0eSQBxD03Quu+cqPv7IDaxpnVXz8WkgLG1PEaZK1NdVAev/N4Ap1wa+D7cYg0XH+5kdL1+iyoJUilYRDLuXKLA+jjElYxggVmVuFNEZhiQV2myyMqu/i2+e+CbeeeHXYLDLjbcoKuoEgVCmvuKJnwvqx3U2B81N0NrsxvtUi9XyQAHjz1c//2B8zgHRszxmWoAjgVOBFwInAqk6f7sK+A1wC3Av0E9DGtKQf1h5NsHqROBtwBs9aE1W9gUuAf4VuBn4NvA7r0Q0pCEN+QeTYOofIagEpJM4UBEQmQ9cAdwBvHU7gaoUcM8Efg18Gdij0a0NaUhDsyqVVmAezr8027tIImBPoElFrBXpmDbUG62fPueKKI73ygfpryM8f0pQ0WlazwfeA9zV6N6GNOSfG6yagROA4/z/ewILLEz3JlgIiKhigpDFXWsYaGm76rK3Xf4SNXyFwcGFBFOq0B0N/C9wDnBro4sb0pB/PrBKAa9T1XOAk9RpVaWO/bH7mSBk4ZZ1jASpK8993w9vufWs11zNmuGFxPmJuyBTI9OBa4CzG4DVkIb8Y0i9Ks4rcDtvP7VwhoVW622+4mt8q1wJk5iujtnXveW937nh1jNe823WDC0hl4VUalfVrRP4qTcLG9KQhvyDa1bNwGeA86zjQW0j5SgT2SjNgeufXPG+8z/xlV+d9cZP0TVyIHEOUmF5vtOEG+oAIvcDDwNP+U/TwN6oHggcj0jbJDSsn4CeDKyhmNukCkHAFJukDWlIQ3YBWO2j8B0LJxd/WI42JkUkxCQImdW3Jf/AXod87OYTX30GvfFLGRp2oBBXZRX0IfwAkRsQWQ5sLcHCEJiO6gFY+1KUC4CFNWso7AX6IRJzKarjBNIggP5R6Ol3ZulOIv81pCEN2bVg9RxVvVodo7ymJlUQE4TMHOwhH4a/uOiSq3qfWnDg29iypfDXauX4I4F8hDD8CyJxpdsD3e7SezH2Oqz9KI6nVQ2sAN6O1Ru9KTuuWcUx5PIQBQ2wasjUiiqB2kY77EywEjhU4XvlgKoWWIlamnLD9qcnvO6Gx/Y49HwdGJhLHEP10JJrCIN/h2jzZLoeeAz0AhLzFPABqvvfIuDVWHsHkEPE0fj7B+szTXeuhL48nd5M7QQSoAmY4etRb4HEK7s9QN4D+hZ/5dkNCLJ1VqSD8d1kW1K/EBjymnZBZuPCsLJMjJYpfH8A6K3VaPWUWyrXQXx/tQPlFtiUL0ePqBJHKUbSLZXGWlg0LjqANmBmUf/NwG1oTaY/A//8QV/dLX6xz/nxVq2Tip+vO2kYpHFRJsMT1IiJRputVsdSsJqr8CUcYJXtqDIlj4EhK5JvHx1JNnfM/d53XvaOocSYkxnqr7Xa/JIouhQJurerCUSGgI+TJAL8R41vvwTHCVszBlbW7kqfVRPwXOA04CBVnQ/M8oOiwE/b3t2HvL9HgoufXAU8ATwI/L5kou+6lVCETUnMwjAiI1ININ4FXOgHsymZcM3Abf47BfmQ16g3lgz6woS/AfgYVVT0LmPKxgUCpETYmrj53FG53K3AfwJn4OJWS4EsA/xaRT4wZ7CbWw48kQ+/4WPQt7GwGx4Ac3FulqOAuRid48dDh/+bLbqXbCdIFBat9YzH1v4Z+C3QRxjA6CgEAplMIT7vdcB7/T2SnQRWTX5R3eDrXhp23OPBdC1wJy7ud7AiWCl8SN2kHnuCVl53RoDlwM+shH8SbO8+fZtzZ/zH9WsfPfRFV7H+yQWg1Zr4UZAPYG03xtTSviqMOgPWZrH2Sqw9FOGMaj44rC4G1mCsC+DcdUA1A7hC4UIDUxGtmil6PRM4rOj9Qzhm/4/rUCimRLPKqhJVB6sDcQHslaS7BAiWAAv8VU6WVypLBGwyCXl1yFZRLRGh2xhsENAeBOXKnvHl3rvKbVYH1shApk0fmb8/2XDMN9oJ+k7i+K24sLGpXiTxmuuhuDjcS4F7gI/7xQziZNw94riThz+LCvlW4C/Af+NC6BKAqEjdfami7y50qhT/vy1iLQM+C1wjQmyCkD16NrFq+jy6Oue2MTj4Imf+VQSDmICPI6wiZyCbhc5Ot+LUE/kdhG6k9Y+CWhD6UK7A6otBmipq/8ICVJ2fSmSXmH8C7QrfS+DMZ8MrJi5Q/LtAi8A3y03ggKlB0IKq020NFshUXhyGatymv6TItVb7gUp20ajqmLpST9l7rMUCLUFQungn3gytAtQyNHeoj3v3PIwP/svl0Lse0tEMrLkaa87g2XWTnoBwDcjrCcI7SYybS05GvcWUepbKNhM4DeE04JOI/BdgI+sKOE2Qj+CUwQmdUka7ulHgvQJPmyBkWn6YhV1reHrmQl777qtZvvDgU9j09DyCsNpCfguB3Oye5tFwNOuAasH8ykueAH2j0NfjUrYYO/4M5W8Yez2OuV7pBrM9GBZ8DZuoJ8VMGbDsyA0SxKMYCQirOE4VLk7gTH12HfiRwOXAbQorS5t0VHVKnVsB0GsNsyprz1Ibc3cYtMmr0med+RfVecsQ6LcWAzRX1w63/a1NZGNrJ78/8AQYHQAJMlj7FRJ7xs6p1Q6rvbMQvolyAlZ7HIo/24UqWZaEjwKPY+zPIq89vdjCi4rbT4sgqmia/UKQN4tfUdpzwzw+Z29uO+hkvvT8N7Fy0eGwcfUhhFGHA5KyYgmC69EgS1J05/wgkclz8oO/JVKLlmm09uF+rnvZW8GmoGsdpNPFf84C12NMNbDqw5hWovBHwGKvIa4Alno/z6N1NWJumD8uOoYXb3qM2dkBRsJ0pQkyLxA5dzfZZ5yh8Bbgw6VAstUmqAqpKR6no9bSNMkJXwF36uEHbuPtHrSWRCefNioAhqwbk5lJlL85n+Vviw/hitddBlvWggSnkui57EZ4ABxIkpxJYr9PKirNM7e7ANZl5JM/ROr68awCJJkKo0PhfnFO0Cw4msI+PZu5/pBT+PD7roEnV8CGJyGKFpJUXafXEOp9YwnAxpahiCTTzpVfuYhpSb5sOaYDx6y8jw/+yxeheRrkhiauBMpjWLMRZX6FZz8FHEliXuHfH1H0t6e9/f5b4Eaq7byM9HLzIa/ikK4VfHjp9fS3zao0yA+J4LDdiBRxlFcWTLFtHLJrkuP1W4sJAppEdtQCqpWpo7XU/h9SS6K63WlGAmDYm5CpOrWPJAiIkjxsXgdJHpCTnJ90N6PJqBxDGHzf+YB3ywxLB6McGVllD3FOtwlaVEkG0GGE9wn0CaASsGCwm0fm7sGvjz0LnlwO2SHnS7I6o2pnKA+QJKvGHGKFJ0ybx7m//296O2fRp1pWs3o6jHjN7deS9A5w2blfAo0gN1J8oz5EViJlwSqPtQlB8G9liycswYHxhbj8WN/zoFXeZ5YfYr/+9TSbfFlT0AP8vnFdg0XB5VDsBYakvjCogoXeAXTWqb7P8/6ArkIZR9SQqBJO8XJfuPugNaTCaLu0K3GXAt8BHmGi472AKSng/ybYwMCoWvKqpAPZoTqMqiWQsC64SYDEWkhiiJNmJFiCbAcYTMqFIEULeN2/O9y7RnrYbUWOjBQ9BEdZmDCoSgbTNSh3F77Tkh/m4VlL+PfXfoJlS46G7jUQRsVmfjVZgYhOaMsow1tv/QLvuf0qels7SYLyWn5kElbPWcSZj/4G87OI/3z9p12JTVwoeS+wscLEXY3qxVj7+jpa5hXAS4GfAJ8E1mxr0/TyzSNex4KRHg7sW89gqqnc2N6/roEmcrW4Z/UDo0H93gz1cZovFWs/gOPH1JpvqfGOEkbVYph6sCogiQEGjZm0dlXgRnm6wfW4q87fqQPkHfTHFMowai2pOqAg8b4ugmbAdpLk5tbdzFYB/oRINyLGA0muzPwqLFrTgA5UFdV24EBEFtX5vCavrfZMEkQHUUaRSWdvSYBOlDQidY52OSBSOF5Lal6CycZP2rG/hEmWZTMXs2yPY2DjKqdpmHhc564+MfMTTTcLzTM445HfkU01EQcpqg3j0FrWdy7gVY/8FlHl8pf/P/JhCtSA28FYWeGn04E31wGmBckAbwc5Fpcn6+6Jetowf9vreax96DqO27yCnlQzwcRyZ3DkxVpyWwgXCWSlaFJPzqTnPuNyil1S4+tRAdWdlmAwqmN59UtkD6+JzfL/p/wgU8b5YeIXiE2Mc5wMjitT+HxtaXnzqqS3w3fliGRaP5L7toxVsW5mlPbPQcAiHKdJfNnn+PoO+LILjqu03Ps0R3ybCSBadUYqBoEgA2SnkcvNdxtPNUv+CEHwPpClBAz5xX2kjr51K6a1GZS5oG9DeTeOkFlN0mzPzp/wQUTuRrV5kr+MvTVwJKrvQMsT0EvaZI9IXYdVhAdFHxBk+QSzUAKa4xwM9ri+LKYbqCY10DiY8P1MK5fedDkzR/sYzrRQz3orqvS2dnLi6r+QJ3SDIT9cKPCyCrWZu50L6hGIXANcjMtG6gsRQu8zfO6485ib7eOA3nUMpzLl5lcteSJvNbu9fgz1GlIo3GNcmuiA8ozqdopIl4Uv2Yno/ULgtcAxis7ymlqzX3UL27vKOHenIAUmecEhM+g/G1anlf7Om9bLC5rOqFpS2+G78mX9T+B0xtnQxcqb4PLyf8ahj6JM8FUdBbxT0aP9YtJW5AMrrVuuaMHuBTYL/FLh83m1uVDEVg0/A6xaJclDkk+TJE1IzSExCPwXmfTvJqnsJhQoIIEMgWwln/8PjHkOyAvqcA/MYjxxQH06fTr9F4Lg4R2gAN2OSR7C6PW4SI5q0hxZdEYNM3mZogMyQUNVbLkCioAxXTW4UhO1DYk4fP1jTMsO0dPcSb3DNwlCrIT8+KfvZOWsvfnkKz4CSQ6S5C6S5G+IHLPzzGVZDPLdIn+WH40xT87ej82ZaRxkE4xmKrlqqsnJBj1e0fu2t8sTlFDllgg5qWieFLl5EL+R0gd0C5Dz2oZ34eyj8FGrnOEH7WRBpBS8Zha93l8cS/s9wA+BTyrkjI6j33b4vp7jr0rSTZGqVMQbvMwq7wLm1Vm/TIlTf6HAkeo2Tf7NKj3VOtj4g5KIE4gTS5wkNYnIyuOEwS/J5twJOM1Nk7S1AxjJQT4HiEHpqq7/jYG8TopLoYDJttHc7AhPOrmfjw0wq/eSJH/GRXZUExNZrT4u1anCE8hvRgRrrSNzZnOlKLea8Yyh5eQwb5K52K0kYTjdgglCJuvFCNWwV8869u9aTWgsl7/8QyTWbMCYj6J8B5E5Xj0uuDtM0QIdTdLimofIlxDZBPx1DJyHt/ChEy/m27//FHv1byQbpYtV3aE67nuAwLXi0jAP+t9t8Gp/xq+Yz+CIeoUwhRDY7M2sCEglqhsT9G+UUOOs/0HGE3SdRqXE4+bUYkV/lqDHTaG7KsJlyPiQIDME3qtoLr/9u2K1TKLBUpcz8JkE+152PJV3ALxBHAt9pLoZaEnUQj4P+VhJYq1ClC4sjKsIUoq1447yyRKYVSExDuysfhXltirrTxoX2vK4c6Xo5NaOOIa2NgdYVicHVqOjYK2i+nQ9v4gmnuJV9mGbyj5JLeTjsTO9Jqh2biJVCoU4GLVHoHonCAR5muJcsb9nujfZch5cYj9hR/zruFBQRRhOtyBRhtOX/wHUcuVpl5GNMrcyMvBC4vxJSLCfdxw+g4slK8Rk7Ys7YefISZiI+yLyaUReOWYiqNLbMh2xFqzFjC8wZhJq9R5MJLOWKATE477lMXNryE/KwJ84N2SdWWR9nz2D45DdZ9DHh60ZjERokgmhI6LKfxiXonpXSAB6vsKfE9UfB+riB7cDsrSev6s3OQPkFQLvZuee5nSqHwO20qJnFdSq06ySJCQxYQ0tR4HNY6Ev1rqQsukd3gCfREuN+yH/5K/a3zf+eZPdiOjth6Fhigd/7ZEg0NLsDyiVdD0/iSwalKrZJU3SWcaPRT6IIMp4x/aEEj6K5eEqYDUD9GWo3ulWgJinpi3k2M3LiNSQSDjD+xVcwj2XU31Pdf6CJ3B8qM1e+1gJ9FgJBntbOnnF8t8TBxE/P/481ki4Ot8+dzWj/WCS8h3guF4HA+9EeTWqi+roqJeCvhfkyrHBE+dZNW0+84e2ABY73h5PbudECMtoJuVMr1l13CsRuFnhi3m1dw3bhIigABL7inA6u1ZagAv87qeaKeYcCYSKvkG3NVV3hmSqbgiId5c4M3AOSTKrhmaVTFAOVKGn1wHIrJnufTUXi6o/PDV05M4gcA4AraOVrD/Et16wKt6JGxn15ZqMGSkwPApRFJIO96ynK6M6siosUQcc+cIHeYmYMdrHnl0rWdM8fXwncEwvsF9G9YWIVCLvnY2x12D1EeI+rnjexSTGcvbq2+hLtz1pJHjSP3Nf5zPj+QqnAC8u2ug0XnN4FLhHkT90tc1++uTV946c/eBNfPM5b+Te/U/hgc5FMG0uDG8Fm5Rr0GUol2DtVah+HtVT6+isd5HYW7D6sLtdlg8e/y7+8L/vojnJkQvTBdBfZpUu3A7TsyURcKb3G50XIDdZlNjtaJ2CyoI679MD9Hue05CfWIWsCGnvgppH7Z0ngL0E9ldYMZVRSL4P9gHq9V/mvE8vV6TNF9L3NAOBjqf1qSlGwdgxsGolNk11+I+2rcWWHmfWLZwHUboyYIUhDI24hJLGeNOsDj1UPP0nMUV7xXW1bi+5PBNM1slr2q8mNs+tZwGPVFlfrjZFn+ynbsdkjIPRl27m+PVLufie7/HB138buldBbtDtkLl6/BZrbgZ5QxWz5/2g52EUhnv53KHnEcQxb157O0kQ0ZXuyAu6zIPVtaiGCudalyrkOK99HOyvNwBbRfWWOIi+tWrW4nvPevx3XHL/tVx54vmsWHAYf150FLTOhMEtlXrvEVRPw9jvA+fXaLhFKOeAPop1kdRic9w253BO3vAAGFMgta7AkUs/xLMvHcC31GVhWOsmkx4LmqnDP/RTgV+KyJO+8Xr94iVeU2q3bvv6uep2TY+scc92328rprrSis4RZFEdX70duCaApYj0+3oPFgF+B5C2qgco/AvwppoqrSpGLeTHHOzU9FlVAoaePlK5hHQqRbk4UwUknSLu6ibe2FUailZdrIXWFmhrqd9nJQJJ/CKgnWC7bPkm0FNJzAV+PNSS/kjRZVrdKfAcHBelp4CngSq9mTZacgO8+s9Xcf+sfdm01/Oh7xnIDTvQslyGSY4E2b9C+78Zq/eRJN8gicEGfOaQ8xiJWlgw0sXrNtxLLAHDUTNd6WlEmhjgR6jeYF2uon8VaCsiws/E8ajOjWzy3dEo86UVMxc8/va//g8t2R/xuRe8nXUz9uTugz1Zf7Cr3ApigYuIk2nAWVVXGOH1WP06qmudVhtxxTFv5VXP3MNo1Iz1/Zeofk0dJeCE3QCw5qvqhcBHHU2opmmUFfiEeBpApUHkfYEAS9UFS1+t1Q/qaKI+DtoOAhWgzBSZGH5TBgp+E7qsGLkamiXACoVfWbfjeEktm86gBbCS7QIrVUhHpPKGBcuW0TQ4ignDMlaVEFjL1plt9MzppEbI27Zeh2zOmY6pSaX4/nKltCx11WtSZH59KlJ4sNyjLGO51VvUHcH18Pg2kzIcNrNP10pOXf1XfrPHETy0YSk37Hk8A4uOgp6nIY6fwJjzUa4F2aPCKPk8IoME4Y+dM3GUrx18Pox2syk9g6FMB0f0Luc1G++nPwzZmu5gIGwaFLXvV/iTwjcEFpVkhhBRe6EoJwdq/l9vc8dNW5qFd9/zPfJByPe2rmbN9D2469g3OdNwsNuBq4iz8VVyaPw2jD0A5OBqpgzoiS7NBqAJKWv5wiFv5PzHbiCRgLQITVG0Ma/6JmPtx63q6d5Uol5le3JGT13yXCBE1TCRYlBOVonwtUikEnG0RKcXgCcT9P3G6m+pzKhv2oWm8SJUqyHEgKJfRCQXBcHYeQI16qhW7eWJcpZUOQfAWMUaH26TmNCZWTUntpkwoVMR0XCWeWu6SSVKrqO1rPKjIgSq2HRq+8AjFLdhtvvKisioPuad1QuK1cuS1K+XKNyicJ/x/BxVQz5qYumMFg7qfoKXrV3Kfvs8yLpZ+3LVYWdipy+B7qfuI47fgOoPgIPKzKdmkG/jeI0/xCqMbAIJ+cpB50OQYV7vcp5omc/Glnm8Zt3tnLB1JStbZzEaRr8S1S1WuVZgDx2br2N12Be4DrgoUP3hpvbZBGp5z90/ZFPrNA7bsor75x3M3w49A3rWuNUv78N2jO3D2stQvb4qI99yMsK1boBZFMtQEjOYxCQEpIKAWJVEdV0o8vaUyHOt6tF+06DdoNOAWZ6Ok4hz2C7wjttR6mfBTwasFvp7bqJMdoISeUKVESPO5msKIxzBRMs+eVSN4zapbrTKGuCQ3WCQ17KH+gUeyqtiVEkFbpExZSZ8IMKITRw3xDJqHFVkYWUz0GKsdTvmcbyVOBlCgrYanThe3iggGskyb3MfKWOJ09U3EwUwgThflZnw8Uy/QCQVfJohIt0YM+zoxLtRWoixOS0PRQrrVPV3wFt0W+dkQaYLfAF4ucBgcZhkZA1D6VYezkzjuese4PQn/8zcnqfoap/HN444B9Kd9zOw6dUol6F6XhlHXBNwFcqewJdB+gNriEa3EFjDpsx0vrbvORCmWdE0h70Xbuata25men6Qvqj5PtC3qAs4nlYmrVQauAq3e3irlYCnO+eQMjHvuO8aXtHSzmcF7jz4FFi/jjEintPPfkli7sHRGyrZGftgbSeqW5GQllw/lzx0DUPp1rHMmIM+PW4kQhIE96vq/UVmSgDMSQcSWDCBc1TPAtICWXVgtciDViEZ2gIm7g5ahU7UHo3wsjpGWrpI46mliK9FPbkRJVZLZypDWsIxwPLgxEASE6uOBxvXjqbZVVlLayXq0wCJUYjVkrOWaakUTUGILUmTNBDnyXnntvdDV62DEYtRK+QTJZ90ESdbaoBVhOPzQSikRmLmbh0ilVjiVEhYIzGlCYRpI3nyqYhcJkKMReEIgc+pW6BMeRuQAOUTwC9cEr7dS6US1Ts0Ch+JVDVRuMHnOioy9Lax/5+PyzT5DsqQ8iJr6G+aRm9zwAvW/oXIxCzofYZ1bfP51pFvfoKRgQsweh1qLhR4lW47gf4Ll8/6CuBXQKIS0GxyRLHLCPrItL15ZOYRrGydz+ce+QbNJsdokL7DKh9zQLdtyf2u4pdwjuWNokoSRDzROZd9+zZz6LI/cef8k2BwyFNHbTEMXI/q86lMHt0X1XmgWwu7v93N02ky+YJiPT8U6QASC+Ssxe/PFLhieYGNeYV04AwMq/qM+lXcr+5/qXPXa7q1ernCO+sAiXq9BbZ01m/MjjjukBQ/X0gFIUVZ0GQSIXzP+lxI0FSx37grN4qxus26mgrCidXW6rwtYxVjrZJYgsQGUWykDjNwXiYxNMWGKDGkjCWJgjqidMCKkIoNc3qH6WlvIpsKCa0epPCSOjrjcOAXuxVQCYglZwL5lGL6C9SFu1T1ToWTKjvaFeAcx9zl/bjE89uqymrpbXLnoT5/w4MID4JN+NZRF8SMDt1MfuRuQY4OrX2lwLEKx447euU4kJ8g+hDCDSh3A/c7mzygLRlB4mEeb9+HDxx6MR99/AdMS4bJE37XomegvLQc0AocoOh7repHKMTNqeXp9g5OWv9/LL//x/xhz1MI4/7SHy4XW5n0502AxcBjVlyO6IL/TJ1v6ipvCtkKyrVR+Fps7FdHkwRFaQ4jAhH68jnaUmkyQVA+tKnYNDEJidXelIQ3KnouLgK/mt9Z67QdW4ufk00ShpPY7UYVzWQBWqIUmSAqwFUo8HdzeqxFpVDHxCQMxTFGFSmpY1NoaQ5Txb6lfFWwUr/25QwSm6A5MYGtBVbC4enEzGjLxj1WIJlkMjwTBgRWmTGYJZeOyEXhbBOQiNYkxM7z43y3OS9MlH4C/n00jG7DKJFxtlOfwqcoA1aUOPvVaT+HAz8CvoU7NaP89klTB5E1vOyZezm2exn3zDyUHxx6Xp9m+24XuCtSOlH2R+3egWVvXFD1vghHgOyr6FsE1qg7nfkeYKmKrG+Lh3R52x4MBilajMEEMqzwac8lKssdU3iHOhrB2HZ5NkixZKibJYPrwEY0bXsI62ZVtlKZ4R54Ew0jkEoMRi1WrYu2d2BWi/B2mLGWBAsKI+qOLjOqDCcxo/VMNp8UjoA26k8vDj6vVTXntCAYtcRqsdZ6wvG2j8hZQ6KWACGUYGEozNf6yjDVUmu2pwOYY1S7RtWOgVS5BHuxKtbELt5JwrZAKiZ5HPNZWbUQG5piI81xIrb2buABolxpguBSRUe3C3wDIbCQSswMRc8SrSuLc8RkDcCp08KWC9xgkV/kMuGDhdWieGL/QZWvAJeWpzJM+HQxLj3u2QK/VvgDjlnex/ixUh1AKglCq8jmBUOb15/d5zJuXH3wOcT5kTgw8ZZAzRbgHnGDNwPajNIOmhLnK5utjoT4DG6bXBVoNVlEbbGasNS6NC4nVWjEDtzJPSvGtUAlG0IiacjnSW8LVj2489bm1tq9MaKkY4N6R611nw/U0TEnK5waIH9D6FdQfO4lVa3rHCRxSkGrUX2N1M5pFRb5vIZrfPdoxZ5krL3T1lDFCr4rl5xRTzbK9Cr3jZl4CMRUSi1AnmHgDKP20YIGW18d7clW5bjqmpWiqiJqNTRmuClv+o3o4jqs+gvURXH8Bpd6O/Jza32FbjBM9G9aG7BvYPXMprw5us52embS8CPchrKW7Y8OKNRjk59n62woKxMJnhLYikwc/qVHcX1W0aOAF9ZR6gDYG+Xd6vxYee/IHvW7THMLq6egK4ZSLZ8Mg/Qvz151M4mE/GavlxObPNkgg7isMsb7wkao45y7XJh2avZ4BoheHLHvpEqgr/BCq3pVsdM1DwTGEsaWTLKNBjwNqk66In8BpBMXuBqoxUes1hOLtg/wS3Ha4//5+qeoz04rBGfPRjnRYPerQ7MaLQKpWoHW8wSuQuSDonqHuubSIp9bYRxEOJZOM+jZRu3FNe6b9YNzilUqQYTuRDUnlUNjUri4wTXiOGIjTPQrFtcxEOcDPcuqfkzRqPrzXTjZ/N4tpG1+QFQ3R2IOrdNbcyxu1zgpMnBM5R8okdGwqP8j0MnYkD2ieIO4LhMNE8lnbBjcuRP8k7boqqi0RclEf8gGhQ+gXCfo4jpCcQqfZzyitxd/WFSDo0T5ZiJhb09Tx52vXHsH71hxAx8++lLuWvQiWkY2MZntUhVh8fAmQpNgVIqDvZf6YkqFZBJHej9M/0Q/m9OKUnFpamKdpyK1qANhAaxSxpJYS+AOFxim/sNFW4Dj/TXVshHo8q29SpmYYLqM7I/Iz0AfwOWmGvUrYSERXxMwX9E2hUMt7FdH9H1WfJD3VPtzBfrV9Xc1XtdC4McIS72FMODrWPBJNfs6NgN7JXBMPRkGAmtILKiJSCUjgyjrAtXJ1DmgvvClHXNiA4qsHecraT0/I50jn2uK4lqUip0l5VaG+60LJfgfb+7VhVjluqBET5kLvBPkThUhG0YcMLCau8zzyIbNtNrRugBaVBlp6uTSB77AnOxWhqMJIQKbFM1SmT80U50P6eFijShIDJlsTLqEFKciC2xQ1bdiCif9WAkIkwTr/RSK9AOP4VIk707yGJAXEVCWKzpYwyEPSgZ4nr92hv9ik0jwV2fxTvkoX+2vOTXngnIM1eIIJ1nURCGdDLPXwBNsTc3JB9i/iNYM5dq14ur0jI1kmQ0CQmNrEmOLwbRlMMdoW5pcU4rATm1fBhUqcK8qr7Kq91hnd49fqKc/ll4TdsLUus0QSq59FJoV2Nw0nTev+hUXPv4jbNjEaNhUVz6rOExxcPcjpEyOXBBiscXlMBbiMZ3Sm4hFl6hqW6EuBiFtoT0/RDpvycTJhCs05iVRYsIocdvIpVcqNmsDY58OjKVwlcSB/p6dc/z2zpRbnE2hKHofldNAT2kZVG2yi85S7MadWDS1070cWAVpZue7eOWGaxkNW8Blmn2G3U9uDYyuiPJmMkDlKh8IzcN50tkYDWTXg5V7pD6k8EoLVxvImyIHiS25tGBQu2u9Ue636gLOrbXF10xr7f7WWsQkrG2ZxTmrbuWCZT8iiZrIBZmqgBWoJdu2gLc88XMWDq1lWCIStWOXURtYa1OF5+m2ZfVZp9z7tMnyZNM8lrUcSJMZHTsdzF/7RIk9NRUbKl1RYldGxm4MjFK4SuROxvhfu4V8A7cZQpHP6pNsz0GvO6bpfL26El5LSa/LB1Is3wbu28n1ML4tu6tMZasCA6kORsJ2QmvXAB9kN6IHIKxH/BiVSeuQWnDLtHjAslMIWLUccL0o56vqm63qbcVaVgkIoV7Hsdj3WOxDFpt27yeARbuF2WOeNGtY0zqTNz71Gy5Y/mPyqeaKJ9sAZFMtPGft7bTFwwyHrQToeN5e95U5xSagqo6bZa7cgVV3CnWiMCM/zMrmfbiz8yQ68v2I1eLrg2J0jhioeFkeCmM7kMobUnkHYCXdbICPA1/dDYblT/xEKZWbcBskG3dBGVYAF5XRLmrtYHaUrKW1DjcojeLfgMuksTMB62PAR6gestQWS4rZuS6O7b2bbNgMcA1wNu4osWdbbgdew7acyWbqO0AiU+xHbhnOk8klUwZY9WZO/DkuNvDVqno28GIt3V1xntrfAb8A3l5BU+4AFk+Y0GpY3zyds1bfSqLCDw853wUWl1NHW+dw+oNfZkn/Bja2TifY9juHleYaG88RZgHiQGSDFaEz6WNDpoO/TjuW+bmNGJngmjoVeEsNF1oM+sex6SMQiGVa0s9oOGH8DgGX+oFxmneiHzTljlMnA7gUzD/H5T+vpEFdjTu95R04Ht28KdCm/tdrOMvL/D2c5DhNbce4XgmcCbzXT9D9trMua3F8vU/7vqwaPhNLmvnZbp7T+yfumfES9hxdhZXwWtzO75uAFwCH4pz8u4L1v8E/+zYcV3JgO5SYiUbY2BwTmodyLjSjKUVgdq4CKdd3zt4GVtRCQStSR/wZ9zuptuFY58co+jxgCS4/1WyQV6pyM8JjuHxFvkYTQOXbqlw0scaCoLTHQ9wy93jiMDVmOxfHKCZhmkO2Psb87FbibY9sbxPhRlFerCX3dnaBArJCRY5sSYazubCZby9+F081LWFOvqsYrPbH8Vv2qtF293hQGyl0VIByRP/9vHDLb6lC/luCc/LP8Ndi3A6l4ngy7ZPwc4mf6H24PNqFMxjWA6twu2DLmdx5cEf7chzkJ9BCxpMvBl57LU181OfNoSb/+Ub/zDW4Xb91OL5QJTkOl4poqMREKgT2PgncUfT5ybhc/r1lJlnon1VNizoQl4X2cN8fc3y5W/3rwr7YFsbpNKtwIVurGd+gWQC8mJLklEXa3wrg7kgTskErt805naUdz6HFDJdqgXu7+cMMXGzoXoynsV6AixqZjO8zxOXiKvCyuvzrbl+n5ZScq1AiBzN+IEc5ukTKP+MmHF1pvOIOMBhty5DPRJP2gdlQSPzcsSLkMiGjqcjN3u0Aq6LvaiswQx3PZzZwh6KdKPfgsh6Uk6cVTlX0iUKvJtY9QVDassMVl5cA6IsiSDcTbtsGpwC3VtJY1Pm8bggxb8oFmfgHiy7imaY9mZ70YscXkhMYD5GpJW8Hvj9B1ZIMGR3lklWXY7cryRoZP+HNJMAqwNEJ8lO0Erf5gVlIs9ru27i4B0Y90BQ0nkF2u3DYqm1YWCwyvr6FNXLIt2t+R9pXEWbmh1naeQg/2PNSZuS7J/PztDfLJpP9KfBadO5ZaVCjJOmQoY6mSe8QVgOrHU2gP+yvZ8Dtvila64SlJcBZqvrZwgeJFvYToS/TUkdjmLE85+Lo222CjB3mWI5GIUDaZm8bjtrinyy8gE2ZhXQmfQWgynjz4NI6TaA7gJ9u+wxLazK4I+35rA2wKjJU4z0loPX3JlpUp2Gm4Ah1QYkDGAnbCHTSptEOAeWzBf9TsTMY7cTyFXp+s1c1t8kQWqSVXYTbvn7Ujq09MqnRVRyLq3Cmoq+a+KwSvdXmzU1zX3vXyraDGQqn0WYGWy1BQY2/EDiC+uLVutj1O2gNacg/veyUyHgXM+XoA6pqVXWZqqX0GjMzYW+FzyjM2Qm2wgtU+awqwUQ+2Dj7yqJE1vyiOz27c3Nm/qczNvtdS3Az7qy+L3s/Tb2BtV8t8Z+MI78m9KZmceOCc8nYbGN0NaQhu5tmpWNG3Nj7Pymc55nPJd8ce306jvfzTmoHm1aSF6nq9xWtGP1uEKbHw/pwx5E/39i0+Nz2ZOCCHQDpq4Arq7WEkZDBaBryd+OyaUhD/gk0K5cszpI1CUbtGLtd4deq+rSy7T8mvOK1uC31Ayf56DTwOpyDe+/K0CE0WYMJghsf7jg2P5jqPCOl8fbW+X9webxMLf9EqKYxshrSkN0JrBTGclWXhNX0KnxRKRvyMjF8R/U0XBjC+3A7iNVOImnBHXjwWRy5ripQqcCcONv/VOt+X1o27fA3tiRDC2XyIR7WA+rFlBxL3pCGNOTvwAwUgdhYskniTz7ZBgS+j+MhvbaO2+0NfA53tNHdwJ9xAbcDHizacA7w43GBwZ21EUbojIfYkGl//52zTt0rUnNWpPFks1l0+XJ9vjFUGtKQv1OwKjYFK0gCvAdHMDuoztvt6a9zPEgNebNrGnU6wBXBiDA9zjIctn/9R3te+NdVrYffNDu3sTXA1gtWimP4fgoX39eQhjTk79kMrEPWAW/F5QbanrIVkt/VDVQKdMZDzDfJL/4644SvLWs/6oud8dZFIUk9QKW4kKGzgZc3gKohDfkH0qzqkPv95P8pVM9ZvSPimFqWlMkzkur4441zjvvdvTNedN3M3JbD0jaLrYx3hfCDPwP3eo1qqDE0GtKQfz6wAsdLeg2OqnDUVD0kY/OkrfnBDbNP+8Btc886bUZu8zOtycBtiBQnMhRvWvYyHrO2ml2QZrchDWnI7g9W4AJLz8Sl1XjHFNy/N0A/CXxjS3pOLq9cNy3u/ZkJouTv5wi7hjSkIZVkV5/tthaXnP+1wJ920j0HcDuPL1d3mGkuY3OkNMlaCZJGFzekIQ3NanslD9zgweoFwBuB09k2YVotWYpLV3szLmfTaKM7G9KQBlhNhXQDv8Ttvu2HO3nmRBzNYT7juZEKNtxmnG/pIVzysKX+HnGjGxvSkH98+f8DACeR8Z8W+T8oAAAAAElFTkSuQmCC\" />\r\n  <h2>DiskSpace Report</h2>\r\n  <table border=\"1\">\r\n    <tr bgcolor=\"#66ccff\">\r\n		<th>PackageID</th>\r\n        <th>QuotaValue</th>\r\n        <th>Bandwidth</th>\r\n        <th>UsagePercentage</th>\r\n        <th>PackageName</th>\r\n        <th>PackagesNumber</th>\r\n        <th>StatusID</th>\r\n        <th>UserID</th>\r\n      <th>Username</th>\r\n        <th>FirstName</th>\r\n        <th>LastName</th>\r\n        <th>FullName</th>\r\n        <th>RoleID</th>\r\n        <th>Email</th>\r\n    </tr>\r\n    <xsl:for-each select=\"//Table1\">\r\n    <tr>\r\n	<td><xsl:value-of select=\"PackageID\"/></td>\r\n        <td><xsl:value-of select=\"QuotaValue\"/></td>\r\n        <td><xsl:value-of select=\"Bandwidth\"/></td>\r\n        <td><xsl:value-of select=\"UsagePercentage\"/>%</td>\r\n        <td><xsl:value-of select=\"PackageName\"/></td>\r\n        <td><xsl:value-of select=\"PackagesNumber\"/></td>\r\n        <td><xsl:value-of select=\"StatusID\"/></td>\r\n        <td><xsl:value-of select=\"UserID\"/></td>\r\n      <td><xsl:value-of select=\"Username\"/></td>\r\n        <td><xsl:value-of select=\"FirstName\"/></td>\r\n        <td><xsl:value-of select=\"LastName\"/></td>\r\n        <td><xsl:value-of select=\"FullName\"/></td>\r\n        <td><xsl:value-of select=\"RoleID\"/></td>\r\n        <td><xsl:value-of select=\"Email\"/></td>\r\n        <td><xsl:value-of select=\"UserComments\"/></td>\r\n    </tr>\r\n    </xsl:for-each>\r\n  </table>\r\n  </body>\r\n  </html>\r\n</xsl:template>\r\n</xsl:stylesheet>" },
                    { "TransformContentType", "DiskspaceXLST", 1, "text/html" },
                    { "TransformSuffix", "DiskspaceXLST", 1, ".htm" },
                    { "GridItems", "DisplayPreferences", 1, "10" },
                    { "CC", "DomainExpirationLetter", 1, "support@HostingCompany.com" },
                    { "From", "DomainExpirationLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "DomainExpirationLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>Domain Expiration Information</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; } \r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n\r\n<a name=\"top\"></a>\r\n<div class=\"Header\">\r\n	Domain Expiration Information\r\n</div>\r\n\r\n<ad:if test=\"#user#\">\r\n<p>\r\nHello #user.FirstName#,\r\n</p>\r\n</ad:if>\r\n\r\n<p>\r\nPlease, find below details of your domain expiration information.\r\n</p>\r\n\r\n<table>\r\n    <thead>\r\n        <tr>\r\n            <th>Domain</th>\r\n			<th>Registrar</th>\r\n			<th>Customer</th>\r\n            <th>Expiration Date</th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n            <ad:foreach collection=\"#Domains#\" var=\"Domain\" index=\"i\">\r\n        <tr>\r\n            <td>#Domain.DomainName#</td>\r\n			<td>#iif(isnull(Domain.Registrar), \"\", Domain.Registrar)#</td>\r\n			<td>#Domain.Customer#</td>\r\n            <td>#iif(isnull(Domain.ExpirationDate), \"\", Domain.ExpirationDate)#</td>\r\n        </tr>\r\n    </ad:foreach>\r\n    </tbody>\r\n</table>\r\n\r\n<ad:if test=\"#IncludeNonExistenDomains#\">\r\n	<p>\r\n	Please, find below details of your non-existen domains.\r\n	</p>\r\n\r\n	<table>\r\n		<thead>\r\n			<tr>\r\n				<th>Domain</th>\r\n				<th>Customer</th>\r\n			</tr>\r\n		</thead>\r\n		<tbody>\r\n				<ad:foreach collection=\"#NonExistenDomains#\" var=\"Domain\" index=\"i\">\r\n			<tr>\r\n				<td>#Domain.DomainName#</td>\r\n				<td>#Domain.Customer#</td>\r\n			</tr>\r\n		</ad:foreach>\r\n		</tbody>\r\n	</table>\r\n</ad:if>\r\n\r\n\r\n<p>\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n</p>\r\n\r\n<p>\r\nBest regards\r\n</p>" },
                    { "Priority", "DomainExpirationLetter", 1, "Normal" },
                    { "Subject", "DomainExpirationLetter", 1, "Domain expiration notification" },
                    { "TextBody", "DomainExpirationLetter", 1, "=================================\r\n   Domain Expiration Information\r\n=================================\r\n<ad:if test=\"#user#\">\r\nHello #user.FirstName#,\r\n</ad:if>\r\n\r\nPlease, find below details of your domain expiration information.\r\n\r\n\r\n<ad:foreach collection=\"#Domains#\" var=\"Domain\" index=\"i\">\r\n	Domain: #Domain.DomainName#\r\n	Registrar: #iif(isnull(Domain.Registrar), \"\", Domain.Registrar)#\r\n	Customer: #Domain.Customer#\r\n	Expiration Date: #iif(isnull(Domain.ExpirationDate), \"\", Domain.ExpirationDate)#\r\n\r\n</ad:foreach>\r\n\r\n<ad:if test=\"#IncludeNonExistenDomains#\">\r\nPlease, find below details of your non-existen domains.\r\n\r\n<ad:foreach collection=\"#NonExistenDomains#\" var=\"Domain\" index=\"i\">\r\n	Domain: #Domain.DomainName#\r\n	Customer: #Domain.Customer#\r\n\r\n</ad:foreach>\r\n</ad:if>\r\n\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n\r\nBest regards" },
                    { "CC", "DomainLookupLetter", 1, "support@HostingCompany.com" },
                    { "From", "DomainLookupLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "DomainLookupLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>MX and NS Changes Information</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; } \r\n		.Summary H3 { font-size: 1em; color: ##1F4978; } \r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n\r\n<a name=\"top\"></a>\r\n<div class=\"Header\">\r\n	MX and NS Changes Information\r\n</div>\r\n\r\n<ad:if test=\"#user#\">\r\n<p>\r\nHello #user.FirstName#,\r\n</p>\r\n</ad:if>\r\n\r\n<p>\r\nPlease, find below details of MX and NS changes.\r\n</p>\r\n\r\n    <ad:foreach collection=\"#Domains#\" var=\"Domain\" index=\"i\">\r\n	<h2>#Domain.DomainName# - #DomainUsers[Domain.PackageId].FirstName# #DomainUsers[Domain.PackageId].LastName#</h2>\r\n	<h3>#iif(isnull(Domain.Registrar), \"\", Domain.Registrar)# #iif(isnull(Domain.ExpirationDate), \"\", Domain.ExpirationDate)#</h3>\r\n\r\n	<table>\r\n	    <thead>\r\n	        <tr>\r\n	            <th>DNS</th>\r\n				<th>Type</th>\r\n				<th>Status</th>\r\n	            <th>Old Value</th>\r\n                <th>New Value</th>\r\n	        </tr>\r\n	    </thead>\r\n	    <tbody>\r\n	        <ad:foreach collection=\"#Domain.DnsChanges#\" var=\"DnsChange\" index=\"j\">\r\n	        <tr>\r\n	            <td>#DnsChange.DnsServer#</td>\r\n	            <td>#DnsChange.Type#</td>\r\n				<td>#DnsChange.Status#</td>\r\n                <td>#DnsChange.OldRecord.Value#</td>\r\n	            <td>#DnsChange.NewRecord.Value#</td>\r\n	        </tr>\r\n	    	</ad:foreach>\r\n	    </tbody>\r\n	</table>\r\n	\r\n    </ad:foreach>\r\n\r\n<p>\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n</p>\r\n\r\n<p>\r\nBest regards\r\n</p>" },
                    { "NoChangesHtmlBody", "DomainLookupLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>MX and NS Changes Information</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; } \r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n\r\n<a name=\"top\"></a>\r\n<div class=\"Header\">\r\n	MX and NS Changes Information\r\n</div>\r\n\r\n<ad:if test=\"#user#\">\r\n<p>\r\nHello #user.FirstName#,\r\n</p>\r\n</ad:if>\r\n\r\n<p>\r\nNo MX and NS changes have been found.\r\n</p>\r\n\r\n<p>\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n</p>\r\n\r\n<p>\r\nBest regards\r\n</p>" },
                    { "NoChangesTextBody", "DomainLookupLetter", 1, "=================================\r\n   MX and NS Changes Information\r\n=================================\r\n<ad:if test=\"#user#\">\r\nHello #user.FirstName#,\r\n</ad:if>\r\n\r\nNo MX and NS changes have been founded.\r\n\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n\r\nBest regards\r\n" },
                    { "Priority", "DomainLookupLetter", 1, "Normal" },
                    { "Subject", "DomainLookupLetter", 1, "MX and NS changes notification" },
                    { "TextBody", "DomainLookupLetter", 1, "=================================\r\n   MX and NS Changes Information\r\n=================================\r\n<ad:if test=\"#user#\">\r\nHello #user.FirstName#,\r\n</ad:if>\r\n\r\nPlease, find below details of MX and NS changes.\r\n\r\n\r\n<ad:foreach collection=\"#Domains#\" var=\"Domain\" index=\"i\">\r\n\r\n #Domain.DomainName# - #DomainUsers[Domain.PackageId].FirstName# #DomainUsers[Domain.PackageId].LastName#\r\n Registrar:      #iif(isnull(Domain.Registrar), \"\", Domain.Registrar)#\r\n ExpirationDate: #iif(isnull(Domain.ExpirationDate), \"\", Domain.ExpirationDate)#\r\n\r\n        <ad:foreach collection=\"#Domain.DnsChanges#\" var=\"DnsChange\" index=\"j\">\r\n            DNS:       #DnsChange.DnsServer#\r\n            Type:      #DnsChange.Type#\r\n	    Status:    #DnsChange.Status#\r\n            Old Value: #DnsChange.OldRecord.Value#\r\n            New Value: #DnsChange.NewRecord.Value#\r\n\r\n    	</ad:foreach>\r\n</ad:foreach>\r\n\r\n\r\n\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n\r\nBest regards\r\n" },
                    { "From", "ExchangeMailboxSetupLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "ExchangeMailboxSetupLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>Account Summary Information</title>\r\n    <style type=\"text/css\">\r\n        body {font-family: 'Segoe UI Light','Open Sans',Arial!important;color:black;}\r\n        p {color:black;}\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.SummaryHeader { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.5em; color: ##1F4978; border-bottom: dotted 3px ##efefef; font-weight:normal; }\r\n        .Summary H2 { font-size: 1.2em; color: ##1F4978; } \r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; color:black;}\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n        .Label { color:##1F4978; }\r\n        .menu-bar a {padding: 15px 0;display: inline-block;}\r\n    </style>\r\n</head>\r\n<body>\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\"><!-- was 800 -->\r\n<tbody>\r\n<tr>\r\n<td style=\"padding: 10px 20px 10px 20px; background-color: ##e1e1e1;\">\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">\r\n<tbody>\r\n<tr>\r\n<td style=\"text-align: left; padding: 0px 0px 2px 0px;\"><a href=\"\"><img src=\"\" border=\"0\" alt=\"\" /></a></td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">\r\n<tbody>\r\n<tr>\r\n<td style=\"padding-bottom: 10px;\">\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">\r\n<tbody>\r\n<tr>\r\n<td style=\"background-color: ##2e8bcc; padding: 3px;\">\r\n<table class=\"menu-bar\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">\r\n<tbody>\r\n<tr>\r\n<td style=\"text-align: center;\" width=\"20%\"><a style=\"color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;\" href=\"\"</a></td>\r\n<td style=\"text-align: center;\" width=\"20%\"><a style=\"color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;\" href=\"\"></a></td>\r\n<td style=\"text-align: center;\" width=\"20%\"><a style=\"color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;\" href=\"\"></a></td>\r\n<td style=\"text-align: center;\" width=\"20%\"><a style=\"color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;\" href=\"\"></a></td>\r\n<td style=\"text-align: center;\" width=\"20%\"><a style=\"color: ##ffffff; text-transform: uppercase; font-size: 9px; font-weight: bold; font-family: Arial, Helvetica, sans-serif; text-decoration: none;\" href=\"\"></a></td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">\r\n<tbody>\r\n<tr>\r\n<td style=\"background-color: ##ffffff;\">\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\"><!-- was 759 -->\r\n<tbody>\r\n<tr>\r\n<td style=\"vertical-align: top; padding: 10px 10px 0px 10px;\" width=\"100%\">\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">\r\n<tbody>\r\n<tr>\r\n<td style=\"font-family: 'Segoe UI Light','Open Sans',Arial; padding: 0px 10px 0px 0px;\">\r\n<!-- Begin Content -->\r\n<div class=\"Summary\">\r\n    <ad:if test=\"#Email#\">\r\n    <p>\r\n    Hello #Account.DisplayName#,\r\n    </p>\r\n    <p>\r\n    Thanks for choosing as your Exchange hosting provider.\r\n    </p>\r\n    </ad:if>\r\n    <ad:if test=\"#not(PMM)#\">\r\n    <h1>User Accounts</h1>\r\n    <p>\r\n    The following user accounts have been created for you.\r\n    </p>\r\n    <table>\r\n        <tr>\r\n            <td class=\"Label\">Username:</td>\r\n            <td>#Account.UserPrincipalName#</td>\r\n        </tr>\r\n        <tr>\r\n            <td class=\"Label\">E-mail:</td>\r\n            <td>#Account.PrimaryEmailAddress#</td>\r\n        </tr>\r\n		<ad:if test=\"#PswResetUrl#\">\r\n        <tr>\r\n            <td class=\"Label\">Password Reset Url:</td>\r\n            <td><a href=\"#PswResetUrl#\" target=\"_blank\">Click here</a></td>\r\n        </tr>\r\n		</ad:if>\r\n    </table>\r\n    </ad:if>\r\n    <h1>DNS</h1>\r\n    <p>\r\n    In order for us to accept mail for your domain, you will need to point your MX records to:\r\n    </p>\r\n    <table>\r\n        <ad:foreach collection=\"#SmtpServers#\" var=\"SmtpServer\" index=\"i\">\r\n            <tr>\r\n                <td class=\"Label\">#SmtpServer#</td>\r\n            </tr>\r\n        </ad:foreach>\r\n    </table>\r\n   <h1>\r\n    Webmail (OWA, Outlook Web Access)</h1>\r\n    <p>\r\n    <a href=\"\" target=\"_blank\"></a>\r\n    </p>\r\n    <h1>\r\n    Outlook (Windows Clients)</h1>\r\n    <p>\r\n    To configure MS Outlook to work with the servers, please reference:\r\n    </p>\r\n    <p>\r\n    <a href=\"\" target=\"_blank\"></a>\r\n    </p>\r\n    <p>\r\n    If you need to download and install the Outlook client:</p>\r\n        \r\n        <table>\r\n            <tr><td colspan=\"2\" class=\"Label\"><font size=\"3\">MS Outlook Client</font></td></tr>\r\n            <tr>\r\n                <td class=\"Label\">\r\n                    Download URL:</td>\r\n                <td><a href=\"\"></a></td>\r\n            </tr>\r\n<tr>\r\n                <td class=\"Label\"></td>\r\n                <td><a href=\"\"></a></td>\r\n            </tr>\r\n            <tr>\r\n                <td class=\"Label\">\r\n                    KEY:</td>\r\n                <td></td>\r\n            </tr>\r\n        </table>\r\n \r\n       <h1>\r\n    ActiveSync, iPhone, iPad</h1>\r\n    <table>\r\n        <tr>\r\n            <td class=\"Label\">Server:</td>\r\n            <td>#ActiveSyncServer#</td>\r\n        </tr>\r\n        <tr>\r\n            <td class=\"Label\">Domain:</td>\r\n            <td>#SamDomain#</td>\r\n        </tr>\r\n        <tr>\r\n            <td class=\"Label\">SSL:</td>\r\n            <td>must be checked</td>\r\n        </tr>\r\n        <tr>\r\n            <td class=\"Label\">Your username:</td>\r\n            <td>#SamUsername#</td>\r\n        </tr>\r\n    </table>\r\n \r\n    <h1>Password Changes</h1>\r\n    <p>\r\n    Passwords can be changed at any time using Webmail or the <a href=\"\" target=\"_blank\">Control Panel</a>.</p>\r\n    <h1>Control Panel</h1>\r\n    <p>\r\n    If you need to change the details of your account, you can easily do this using <a href=\"\" target=\"_blank\">Control Panel</a>.</p>\r\n    <h1>Support</h1>\r\n    <p>\r\n    You have 2 options, email <a href=\"mailto:\"></a> or use the web interface at <a href=\"\"></a></p>\r\n    \r\n</div>\r\n<!-- End Content -->\r\n<br></td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n<tr>\r\n<td style=\"background-color: ##ffffff; border-top: 1px solid ##999999;\">\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">\r\n<tbody>\r\n<tr>\r\n<td style=\"vertical-align: top; padding: 0px 20px 15px 20px;\">\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">\r\n<tbody>\r\n<tr>\r\n<td style=\"font-family: Arial, Helvetica, sans-serif; text-align: left; font-size: 9px; color: ##717073; padding: 20px 0px 0px 0px;\">\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">\r\n<tbody>\r\n<tr>\r\n<td style=\"font-family: Arial, Helvetica, sans-serif; font-size: 9px; text-align: left; color: ##1666af; vertical-align: top;\" width=\"33%\"><a style=\"font-weight: bold; text-transform: uppercase; text-decoration: underline; color: ##1666af;\" href=\"\"></a><br />Learn more about the services can provide to improve your business.</td>\r\n<td style=\"font-family: Arial, Helvetica, sans-serif; font-size: 9px; text-align: left; color: ##1666af; padding: 0px 10px 0px 10px; vertical-align: top;\" width=\"34%\"><a style=\"font-weight: bold; text-transform: uppercase; text-decoration: underline; color: ##1666af;\" href=\"\">Privacy Policy</a><br /> follows strict guidelines in protecting your privacy. Learn about our <a style=\"font-weight: bold; text-decoration: underline; color: ##1666af;\" href=\"\">Privacy Policy</a>.</td>\r\n<td style=\"font-family: Arial, Helvetica, sans-serif; font-size: 9px; text-align: left; color: ##1666af; vertical-align: top;\" width=\"33%\"><a style=\"font-weight: bold; text-transform: uppercase; text-decoration: underline; color: ##1666af;\" href=\"\">Contact Us</a><br />Questions? For more information, <a style=\"font-weight: bold; text-decoration: underline; color: ##1666af;\" href=\"\">contact us</a>.</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</body>\r\n</html>" },
                    { "Priority", "ExchangeMailboxSetupLetter", 1, "Normal" },
                    { "Subject", "ExchangeMailboxSetupLetter", 1, " Hosted Exchange Mailbox Setup" },
                    { "TextBody", "ExchangeMailboxSetupLetter", 1, "<ad:if test=\"#Email#\">\r\nHello #Account.DisplayName#,\r\n\r\nThanks for choosing as your Exchange hosting provider.\r\n</ad:if>\r\n<ad:if test=\"#not(PMM)#\">\r\nUser Accounts\r\n\r\nThe following user accounts have been created for you.\r\n\r\nUsername: #Account.UserPrincipalName#\r\nE-mail: #Account.PrimaryEmailAddress#\r\n<ad:if test=\"#PswResetUrl#\">\r\nPassword Reset Url: #PswResetUrl#\r\n</ad:if>\r\n</ad:if>\r\n\r\n=================================\r\nDNS\r\n=================================\r\n\r\nIn order for us to accept mail for your domain, you will need to point your MX records to:\r\n\r\n<ad:foreach collection=\"#SmtpServers#\" var=\"SmtpServer\" index=\"i\">#SmtpServer#</ad:foreach>\r\n\r\n=================================\r\nWebmail (OWA, Outlook Web Access)\r\n=================================\r\n\r\n\r\n\r\n=================================\r\nOutlook (Windows Clients)\r\n=================================\r\n\r\nTo configure MS Outlook to work with servers, please reference:\r\n\r\n\r\n\r\nIf you need to download and install the MS Outlook client:\r\n\r\nMS Outlook Download URL:\r\n\r\nKEY: \r\n\r\n=================================\r\nActiveSync, iPhone, iPad\r\n=================================\r\n\r\nServer: #ActiveSyncServer#\r\nDomain: #SamDomain#\r\nSSL: must be checked\r\nYour username: #SamUsername#\r\n\r\n=================================\r\nPassword Changes\r\n=================================\r\n\r\nPasswords can be changed at any time using Webmail or the Control Panel\r\n\r\n\r\n=================================\r\nControl Panel\r\n=================================\r\n\r\nIf you need to change the details of your account, you can easily do this using the Control Panel \r\n\r\n\r\n=================================\r\nSupport\r\n=================================\r\n\r\nYou have 2 options, email or use the web interface at " },
                    { "MailboxPasswordPolicy", "ExchangePolicy", 1, "True;8;20;0;2;0;True" },
                    { "UserNamePolicy", "FtpPolicy", 1, "True;-;1;20;;;" },
                    { "UserPasswordPolicy", "FtpPolicy", 1, "True;5;20;0;1;0;True" },
                    { "AccountNamePolicy", "MailPolicy", 1, "True;;1;50;;;" },
                    { "AccountPasswordPolicy", "MailPolicy", 1, "True;5;20;0;1;0;False;;0;;;False;False;0;" },
                    { "CatchAllName", "MailPolicy", 1, "mail" },
                    { "DatabaseNamePolicy", "MariaDBPolicy", 1, "True;;1;40;;;" },
                    { "UserNamePolicy", "MariaDBPolicy", 1, "True;;1;16;;;" },
                    { "UserPasswordPolicy", "MariaDBPolicy", 1, "True;5;20;0;1;0;False;;0;;;False;False;0;" },
                    { "DatabaseNamePolicy", "MsSqlPolicy", 1, "True;-;1;120;;;" },
                    { "UserNamePolicy", "MsSqlPolicy", 1, "True;-;1;120;;;" },
                    { "UserPasswordPolicy", "MsSqlPolicy", 1, "True;5;20;0;1;0;True;;0;0;0;False;False;0;" },
                    { "DatabaseNamePolicy", "MySqlPolicy", 1, "True;;1;40;;;" },
                    { "UserNamePolicy", "MySqlPolicy", 1, "True;;1;16;;;" },
                    { "UserPasswordPolicy", "MySqlPolicy", 1, "True;5;20;0;1;0;False;;0;0;0;False;False;0;" },
                    { "From", "OrganizationUserPasswordRequestLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "OrganizationUserPasswordRequestLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>Password request notification</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; } \r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n<div class=\"Header\">\r\n<img src=\"#logoUrl#\">\r\n</div>\r\n<h1>Password request notification</h1>\r\n\r\n<ad:if test=\"#user#\">\r\n<p>\r\nHello #user.FirstName#,\r\n</p>\r\n</ad:if>\r\n\r\n<p>\r\nYour account have been created. In order to create a password for your account, please follow next link:\r\n</p>\r\n\r\n<a href=\"#passwordResetLink#\" target=\"_blank\">#passwordResetLink#</a>\r\n\r\n<p>\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n</p>\r\n\r\n<p>\r\nBest regards\r\n</p>\r\n</div>\r\n</body>" },
                    { "LogoUrl", "OrganizationUserPasswordRequestLetter", 1, "" },
                    { "Priority", "OrganizationUserPasswordRequestLetter", 1, "Normal" },
                    { "SMSBody", "OrganizationUserPasswordRequestLetter", 1, "\r\nUser have been created. Password request url:\r\n#passwordResetLink#" },
                    { "Subject", "OrganizationUserPasswordRequestLetter", 1, "Password request notification" },
                    { "TextBody", "OrganizationUserPasswordRequestLetter", 1, "=========================================\r\n   Password request notification\r\n=========================================\r\n\r\n<ad:if test=\"#user#\">\r\nHello #user.FirstName#,\r\n</ad:if>\r\n\r\nYour account have been created. In order to create a password for your account, please follow next link:\r\n\r\n#passwordResetLink#\r\n\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n\r\nBest regards" },
                    { "DsnNamePolicy", "OsPolicy", 1, "True;-;2;40;;;" },
                    { "CC", "PackageSummaryLetter", 1, "support@HostingCompany.com" },
                    { "EnableLetter", "PackageSummaryLetter", 1, "True" },
                    { "From", "PackageSummaryLetter", 1, "support@HostingCompany.com" },
                    { "Priority", "PackageSummaryLetter", 1, "Normal" },
                    { "Subject", "PackageSummaryLetter", 1, "\"#space.Package.PackageName#\" <ad:if test=\"#Signup#\">hosting space has been created for<ad:else>hosting space summary for</ad:if> #user.FirstName# #user.LastName#" },
                    { "CC", "PasswordReminderLetter", 1, "" },
                    { "From", "PasswordReminderLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "PasswordReminderLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>Account Summary Information</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; }\r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n\r\n<a name=\"top\"></a>\r\n<div class=\"Header\">\r\n	Hosting Account Information\r\n</div>\r\n\r\n<p>\r\nHello #user.FirstName#,\r\n</p>\r\n\r\n<p>\r\nPlease, find below details of your control panel account. The one time password was generated for you. You should change the password after login. \r\n</p>\r\n\r\n<h1>Control Panel URL</h1>\r\n<table>\r\n    <thead>\r\n        <tr>\r\n            <th>Control Panel URL</th>\r\n            <th>Username</th>\r\n            <th>One Time Password</th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n        <tr>\r\n            <td><a href=\"http://panel.HostingCompany.com\">http://panel.HostingCompany.com</a></td>\r\n            <td>#user.Username#</td>\r\n            <td>#user.Password#</td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n\r\n<p>\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n</p>\r\n\r\n<p>\r\nBest regards,<br />\r\nFuseCP.<br />\r\nWeb Site: <a href=\"https://fusecp.com\">https://fusecp.com</a><br />\r\nE-Mail: <a href=\"mailto:support@fusecp.com\">support@fusecp.com</a>\r\n</p>\r\n\r\n</div>\r\n</body>\r\n</html>" },
                    { "Priority", "PasswordReminderLetter", 1, "Normal" },
                    { "Subject", "PasswordReminderLetter", 1, "Password reminder for #user.FirstName# #user.LastName#" },
                    { "TextBody", "PasswordReminderLetter", 1, "=================================\r\n   Hosting Account Information\r\n=================================\r\n\r\nHello #user.FirstName#,\r\n\r\nPlease, find below details of your control panel account. The one time password was generated for you. You should change the password after login.\r\n\r\nControl Panel URL: https://panel.fusecp.com\r\nUsername: #user.Username#\r\nOne Time Password: #user.Password#\r\n\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n\r\nBest regards,\r\nFuseCP.\r\nWeb Site: https://fusecp.com\"\r\nE-Mail: support@fusecp.com" },
                    { "CC", "RDSSetupLetter", 1, "support@HostingCompany.com" },
                    { "From", "RDSSetupLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "RDSSetupLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>RDS Setup Information</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; } \r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n\r\n<a name=\"top\"></a>\r\n<div class=\"Header\">\r\n	RDS Setup Information\r\n</div>\r\n</div>\r\n</body>" },
                    { "Priority", "RDSSetupLetter", 1, "Normal" },
                    { "Subject", "RDSSetupLetter", 1, "RDS setup" },
                    { "TextBody", "RDSSetupLetter", 1, "=================================\r\n   RDS Setup Information\r\n=================================\r\n<ad:if test=\"#user#\">\r\nHello #user.FirstName#,\r\n</ad:if>\r\n\r\nPlease, find below RDS setup instructions.\r\n\r\nIf you have any questions, feel free to contact our support department at any time.\r\n\r\nBest regards" },
                    { "GroupNamePolicy", "SharePointPolicy", 1, "True;-;1;20;;;" },
                    { "UserNamePolicy", "SharePointPolicy", 1, "True;-;1;20;;;" },
                    { "UserPasswordPolicy", "SharePointPolicy", 1, "True;5;20;0;1;0;True;;0;;;False;False;0;" },
                    { "DemoMessage", "FuseCPPolicy", 1, "When user account is in demo mode the majority of operations are\r\ndisabled, especially those ones that modify or delete records.\r\nYou are welcome to ask your questions or place comments about\r\nthis demo on  <a href=\"http://forum.FuseCP.net\"\r\ntarget=\"_blank\">FuseCP  Support Forum</a>" },
                    { "ForbiddenIP", "FuseCPPolicy", 1, "" },
                    { "PasswordPolicy", "FuseCPPolicy", 1, "True;6;20;0;1;0;True;;0;;;False;False;0;" },
                    { "From", "UserPasswordExpirationLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "UserPasswordExpirationLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>Password expiration notification</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; } \r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n<div class=\"Header\">\r\n<img src=\"#logoUrl#\">\r\n</div>\r\n<h1>Password expiration notification</h1>\r\n\r\n<ad:if test=\"#user#\">\r\n<p>\r\nHello #user.FirstName#,\r\n</p>\r\n</ad:if>\r\n\r\n<p>\r\nYour password expiration date is #user.PasswordExpirationDateTime#. You can reset your own password by visiting the following page:\r\n</p>\r\n\r\n<a href=\"#passwordResetLink#\" target=\"_blank\">#passwordResetLink#</a>\r\n\r\n\r\n<p>\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n</p>\r\n\r\n<p>\r\nBest regards\r\n</p>\r\n</div>\r\n</body>" },
                    { "LogoUrl", "UserPasswordExpirationLetter", 1, "" },
                    { "Priority", "UserPasswordExpirationLetter", 1, "Normal" },
                    { "Subject", "UserPasswordExpirationLetter", 1, "Password expiration notification" },
                    { "TextBody", "UserPasswordExpirationLetter", 1, "=========================================\r\n   Password expiration notification\r\n=========================================\r\n\r\n<ad:if test=\"#user#\">\r\nHello #user.FirstName#,\r\n</ad:if>\r\n\r\nYour password expiration date is #user.PasswordExpirationDateTime#. You can reset your own password by visiting the following page:\r\n\r\n#passwordResetLink#\r\n\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n\r\nBest regards" },
                    { "From", "UserPasswordResetLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "UserPasswordResetLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>Password reset notification</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; } \r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n<div class=\"Header\">\r\n<img src=\"#logoUrl#\">\r\n</div>\r\n<h1>Password reset notification</h1>\r\n\r\n<ad:if test=\"#user#\">\r\n<p>\r\nHello #user.FirstName#,\r\n</p>\r\n</ad:if>\r\n\r\n<p>\r\nWe received a request to reset the password for your account. If you made this request, click the link below. If you did not make this request, you can ignore this email.\r\n</p>\r\n\r\n<a href=\"#passwordResetLink#\" target=\"_blank\">#passwordResetLink#</a>\r\n\r\n\r\n<p>\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n</p>\r\n\r\n<p>\r\nBest regards\r\n</p>\r\n</div>\r\n</body>" },
                    { "LogoUrl", "UserPasswordResetLetter", 1, "" },
                    { "PasswordResetLinkSmsBody", "UserPasswordResetLetter", 1, "Password reset link:\r\n#passwordResetLink#\r\n" },
                    { "Priority", "UserPasswordResetLetter", 1, "Normal" },
                    { "Subject", "UserPasswordResetLetter", 1, "Password reset notification" },
                    { "TextBody", "UserPasswordResetLetter", 1, "=========================================\r\n   Password reset notification\r\n=========================================\r\n\r\n<ad:if test=\"#user#\">\r\nHello #user.FirstName#,\r\n</ad:if>\r\n\r\nWe received a request to reset the password for your account. If you made this request, click the link below. If you did not make this request, you can ignore this email.\r\n\r\n#passwordResetLink#\r\n\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n\r\nBest regards" },
                    { "From", "UserPasswordResetPincodeLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "UserPasswordResetPincodeLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>Password reset notification</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; } \r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n<div class=\"Header\">\r\n<img src=\"#logoUrl#\">\r\n</div>\r\n<h1>Password reset notification</h1>\r\n\r\n<ad:if test=\"#user#\">\r\n<p>\r\nHello #user.FirstName#,\r\n</p>\r\n</ad:if>\r\n\r\n<p>\r\nWe received a request to reset the password for your account. Your password reset pincode:\r\n</p>\r\n\r\n#passwordResetPincode#\r\n\r\n<p>\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n</p>\r\n\r\n<p>\r\nBest regards\r\n</p>\r\n</div>\r\n</body>" },
                    { "LogoUrl", "UserPasswordResetPincodeLetter", 1, "" },
                    { "PasswordResetPincodeSmsBody", "UserPasswordResetPincodeLetter", 1, "\r\nYour password reset pincode:\r\n#passwordResetPincode#" },
                    { "Priority", "UserPasswordResetPincodeLetter", 1, "Normal" },
                    { "Subject", "UserPasswordResetPincodeLetter", 1, "Password reset notification" },
                    { "TextBody", "UserPasswordResetPincodeLetter", 1, "=========================================\r\n   Password reset notification\r\n=========================================\r\n\r\n<ad:if test=\"#user#\">\r\nHello #user.FirstName#,\r\n</ad:if>\r\n\r\nWe received a request to reset the password for your account. Your password reset pincode:\r\n\r\n#passwordResetPincode#\r\n\r\nIf you have any questions regarding your hosting account, feel free to contact our support department at any time.\r\n\r\nBest regards" },
                    { "CC", "VerificationCodeLetter", 1, "support@HostingCompany.com" },
                    { "From", "VerificationCodeLetter", 1, "support@HostingCompany.com" },
                    { "HtmlBody", "VerificationCodeLetter", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>Verification code</title>\r\n    <style type=\"text/css\">\r\n		.Summary { background-color: ##ffffff; padding: 5px; }\r\n		.Summary .Header { padding: 10px 0px 10px 10px; font-size: 16pt; background-color: ##E5F2FF; color: ##1F4978; border-bottom: solid 2px ##86B9F7; }\r\n        .Summary A { color: ##0153A4; }\r\n        .Summary { font-family: Tahoma; font-size: 9pt; }\r\n        .Summary H1 { font-size: 1.7em; color: ##1F4978; border-bottom: dotted 3px ##efefef; }\r\n        .Summary H2 { font-size: 1.3em; color: ##1F4978; }\r\n        .Summary TABLE { border: solid 1px ##e5e5e5; }\r\n        .Summary TH,\r\n        .Summary TD.Label { padding: 5px; font-size: 8pt; font-weight: bold; background-color: ##f5f5f5; }\r\n        .Summary TD { padding: 8px; font-size: 9pt; }\r\n        .Summary UL LI { font-size: 1.1em; font-weight: bold; }\r\n        .Summary UL UL LI { font-size: 0.9em; font-weight: normal; }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"Summary\">\r\n\r\n<a name=\"top\"></a>\r\n<div class=\"Header\">\r\n	Verification code\r\n</div>\r\n\r\n<p>\r\nHello #user.FirstName#,\r\n</p>\r\n\r\n<p>\r\nto complete the sign in, enter the verification code on the device. \r\n</p>\r\n\r\n<table>\r\n    <thead>\r\n        <tr>\r\n            <th>Verification code</th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n        <tr>\r\n            <td>#verificationCode#</td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n<p>\r\nBest regards,<br />\r\n\r\n</p>\r\n\r\n</div>\r\n</body>\r\n</html>" },
                    { "Priority", "VerificationCodeLetter", 1, "Normal" },
                    { "Subject", "VerificationCodeLetter", 1, "Verification code" },
                    { "TextBody", "VerificationCodeLetter", 1, "=================================\r\n   Verification code\r\n=================================\r\n<ad:if test=\"#user#\">\r\nHello #user.FirstName#,\r\n</ad:if>\r\n\r\nto complete the sign in, enter the verification code on the device.\r\n\r\nVerification code\r\n#verificationCode#\r\n\r\nBest regards,\r\n" },
                    { "AddParkingPage", "WebPolicy", 1, "True" },
                    { "AddRandomDomainString", "WebPolicy", 1, "False" },
                    { "AnonymousAccountPolicy", "WebPolicy", 1, "True;;5;20;;_web;" },
                    { "AspInstalled", "WebPolicy", 1, "True" },
                    { "AspNetInstalled", "WebPolicy", 1, "2" },
                    { "CgiBinInstalled", "WebPolicy", 1, "False" },
                    { "DefaultDocuments", "WebPolicy", 1, "Default.htm,Default.asp,index.htm,Default.aspx" },
                    { "EnableAnonymousAccess", "WebPolicy", 1, "True" },
                    { "EnableBasicAuthentication", "WebPolicy", 1, "False" },
                    { "EnableDedicatedPool", "WebPolicy", 1, "False" },
                    { "EnableDirectoryBrowsing", "WebPolicy", 1, "False" },
                    { "EnableParentPaths", "WebPolicy", 1, "False" },
                    { "EnableParkingPageTokens", "WebPolicy", 1, "False" },
                    { "EnableWindowsAuthentication", "WebPolicy", 1, "True" },
                    { "EnableWritePermissions", "WebPolicy", 1, "False" },
                    { "FrontPageAccountPolicy", "WebPolicy", 1, "True;;1;20;;;" },
                    { "FrontPagePasswordPolicy", "WebPolicy", 1, "True;5;20;0;1;0;False;;0;0;0;False;False;0;" },
                    { "ParkingPageContent", "WebPolicy", 1, "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>The web site is under construction</title>\r\n<style type=\"text/css\">\r\n	H1 { font-size: 16pt; margin-bottom: 4px; }\r\n	H2 { font-size: 14pt; margin-bottom: 4px; font-weight: normal; }\r\n</style>\r\n</head>\r\n<body>\r\n<div id=\"PageOutline\">\r\n	<h1>This web site has just been created from <a href=\"https://www.FuseCP.com\">FuseCP </a> and it is still under construction.</h1>\r\n	<h2>The web site is hosted by <a href=\"https://fusecp.com\">FuseCP</a>.</h2>\r\n</div>\r\n</body>\r\n</html>" },
                    { "ParkingPageName", "WebPolicy", 1, "default.aspx" },
                    { "PerlInstalled", "WebPolicy", 1, "False" },
                    { "PhpInstalled", "WebPolicy", 1, "" },
                    { "PublishingProfile", "WebPolicy", 1, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<publishData>\r\n<ad:if test=\"#WebSite.WebDeploySitePublishingEnabled#\">\r\n	<publishProfile\r\n		profileName=\"#WebSite.Name# - Web Deploy\"\r\n		publishMethod=\"MSDeploy\"\r\n		publishUrl=\"#WebSite[\"WmSvcServiceUrl\"]#:#WebSite[\"WmSvcServicePort\"]#\"\r\n		msdeploySite=\"#WebSite.Name#\"\r\n		userName=\"#WebSite.WebDeployPublishingAccount#\"\r\n		userPWD=\"#WebSite.WebDeployPublishingPassword#\"\r\n		destinationAppUrl=\"http://#WebSite.Name#/\"\r\n		<ad:if test=\"#Not(IsNull(MsSqlDatabase)) and Not(IsNull(MsSqlUser))#\">SQLServerDBConnectionString=\"server=#MsSqlServerExternalAddress#;database=#MsSqlDatabase.Name#;uid=#MsSqlUser.Name#;pwd=#MsSqlUser.Password#\"</ad:if>\r\n		<ad:if test=\"#Not(IsNull(MySqlDatabase)) and Not(IsNull(MySqlUser))#\">mySQLDBConnectionString=\"server=#MySqlAddress#;database=#MySqlDatabase.Name#;uid=#MySqlUser.Name#;pwd=#MySqlUser.Password#\"</ad:if>\r\n		<ad:if test=\"#Not(IsNull(MariaDBDatabase)) and Not(IsNull(MariaDBUser))#\">MariaDBDBConnectionString=\"server=#MariaDBAddress#;database=#MariaDBDatabase.Name#;uid=#MariaDBUser.Name#;pwd=#MariaDBUser.Password#\"</ad:if>\r\n		hostingProviderForumLink=\"https://fusecp.com/support\"\r\n		controlPanelLink=\"https://panel.fusecp.com/\"\r\n	/>\r\n</ad:if>\r\n<ad:if test=\"#IsDefined(\"FtpAccount\")#\">\r\n	<publishProfile\r\n		profileName=\"#WebSite.Name# - FTP\"\r\n		publishMethod=\"FTP\"\r\n		publishUrl=\"ftp://#FtpServiceAddress#\"\r\n		ftpPassiveMode=\"True\"\r\n		userName=\"#FtpAccount.Name#\"\r\n		userPWD=\"#FtpAccount.Password#\"\r\n		destinationAppUrl=\"http://#WebSite.Name#/\"\r\n		<ad:if test=\"#Not(IsNull(MsSqlDatabase)) and Not(IsNull(MsSqlUser))#\">SQLServerDBConnectionString=\"server=#MsSqlServerExternalAddress#;database=#MsSqlDatabase.Name#;uid=#MsSqlUser.Name#;pwd=#MsSqlUser.Password#\"</ad:if>\r\n		<ad:if test=\"#Not(IsNull(MySqlDatabase)) and Not(IsNull(MySqlUser))#\">mySQLDBConnectionString=\"server=#MySqlAddress#;database=#MySqlDatabase.Name#;uid=#MySqlUser.Name#;pwd=#MySqlUser.Password#\"</ad:if>\r\n		<ad:if test=\"#Not(IsNull(MariaDBDatabase)) and Not(IsNull(MariaDBUser))#\">MariaDBDBConnectionString=\"server=#MariaDBAddress#;database=#MariaDBDatabase.Name#;uid=#MariaDBUser.Name#;pwd=#MariaDBUser.Password#\"</ad:if>\r\n		hostingProviderForumLink=\"https://fusecp.com/support\"\r\n		controlPanelLink=\"https://panel.fusecp.com/\"\r\n    />\r\n</ad:if>\r\n</publishData>\r\n\r\n<!--\r\nControl Panel:\r\nUsername: #User.Username#\r\nPassword: #User.Password#\r\n\r\nTechnical Contact:\r\nsupport@fusecp.com\r\n-->" },
                    { "PythonInstalled", "WebPolicy", 1, "False" },
                    { "SecuredGroupNamePolicy", "WebPolicy", 1, "True;;1;20;;;" },
                    { "SecuredUserNamePolicy", "WebPolicy", 1, "True;;1;20;;;" },
                    { "SecuredUserPasswordPolicy", "WebPolicy", 1, "True;5;20;0;1;0;False;;0;0;0;False;False;0;" },
                    { "VirtDirNamePolicy", "WebPolicy", 1, "True;-;3;50;;;" },
                    { "WebDataFolder", "WebPolicy", 1, "\\[DOMAIN_NAME]\\data" },
                    { "WebLogsFolder", "WebPolicy", 1, "\\[DOMAIN_NAME]\\logs" },
                    { "WebRootFolder", "WebPolicy", 1, "\\[DOMAIN_NAME]\\wwwroot" }
                });

            migrationBuilder.InsertData(
                table: "PackagesTreeCache",
                columns: new[] { "PackageID", "ParentPackageID" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Quotas",
                columns: new[] { "QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota" },
                values: new object[,]
                {
                    { 2, 6, null, 7, null, "Databases", "MySQL4.Databases", 1, 2, true },
                    { 3, 5, null, 5, null, "Databases", "MsSQL2000.Databases", 1, 2, true },
                    { 4, 3, null, 9, null, "FTP Accounts", "FTP.Accounts", 1, 2, true },
                    { 12, 8, null, 14, null, "Statistics Sites", "Stats.Sites", 1, 2, true },
                    { 13, 2, null, 10, null, "Web Sites", "Web.Sites", 1, 2, true },
                    { 14, 4, null, 15, null, "Mail Accounts", "Mail.Accounts", 1, 2, true },
                    { 15, 5, null, 6, null, "Users", "MsSQL2000.Users", 2, 2, false },
                    { 18, 4, null, 16, null, "Mail Forwardings", "Mail.Forwardings", 3, 2, false },
                    { 19, 6, null, 8, null, "Users", "MySQL4.Users", 2, 2, false },
                    { 20, 4, null, 17, null, "Mail Lists", "Mail.Lists", 6, 2, false },
                    { 24, 4, null, 18, null, "Mail Groups", "Mail.Groups", 4, 2, false },
                    { 47, 1, null, 20, null, "ODBC DSNs", "OS.ODBC", 6, 2, false },
                    { 59, 2, null, 25, null, "Shared SSL Folders", "Web.SharedSSL", 8, 2, false },
                    { 62, 10, null, 21, null, "Databases", "MsSQL2005.Databases", 1, 2, false },
                    { 63, 10, null, 22, null, "Users", "MsSQL2005.Users", 2, 2, false },
                    { 68, 11, null, 23, null, "Databases", "MySQL5.Databases", 1, 2, false },
                    { 69, 11, null, 24, null, "Users", "MySQL5.Users", 2, 2, false },
                    { 110, 90, null, 75, null, "Databases", "MySQL8.Databases", 1, 2, false },
                    { 111, 90, null, 76, null, "Users", "MySQL8.Users", 2, 2, false },
                    { 200, 20, null, 200, 1, "SharePoint Site Collections", "HostedSharePoint.Sites", 1, 2, false },
                    { 205, 13, null, 29, null, "Organizations", "HostedSolution.Organizations", 1, 2, false },
                    { 206, 13, null, 30, 1, "Users", "HostedSolution.Users", 2, 2, false },
                    { 211, 22, null, 31, null, "Databases", "MsSQL2008.Databases", 1, 2, false },
                    { 212, 22, null, 32, null, "Users", "MsSQL2008.Users", 2, 2, false },
                    { 218, 23, null, 37, null, "Databases", "MsSQL2012.Databases", 1, 2, false },
                    { 219, 23, null, 38, null, "Users", "MsSQL2012.Users", 2, 2, false },
                    { 300, 30, null, 33, null, "Number of VPS", "VPS.ServersNumber", 1, 2, false },
                    { 345, 40, null, 35, null, "Number of VPS", "VPSForPC.ServersNumber", 1, 2, false },
                    { 470, 46, null, 39, null, "Databases", "MsSQL2014.Databases", 1, 2, false },
                    { 471, 46, null, 40, null, "Users", "MsSQL2014.Users", 2, 2, false },
                    { 550, 73, null, 204, 1, "SharePoint Site Collections", "HostedSharePointEnterprise.Sites", 1, 2, false },
                    { 553, 33, null, 41, null, "Number of VPS", "VPS2012.ServersNumber", 1, 2, false },
                    { 573, 50, null, 202, null, "Databases", "MariaDB.Databases", 1, 2, false },
                    { 574, 50, null, 203, null, "Users", "MariaDB.Users", 2, 2, false },
                    { 673, 167, null, 41, null, "Number of VPS", "PROXMOX.ServersNumber", 1, 2, false },
                    { 701, 71, null, 39, null, "Databases", "MsSQL2016.Databases", 1, 2, false },
                    { 702, 71, null, 40, null, "Users", "MsSQL2016.Users", 2, 2, false },
                    { 711, 72, null, 73, null, "Databases", "MsSQL2017.Databases", 1, 2, false },
                    { 712, 72, null, 74, null, "Users", "MsSQL2017.Users", 2, 2, false },
                    { 721, 74, null, 77, null, "Databases", "MsSQL2019.Databases", 1, 2, false },
                    { 722, 74, null, 78, null, "Users", "MsSQL2019.Users", 2, 2, false },
                    { 732, 75, null, 79, null, "Databases", "MsSQL2022.Databases", 1, 2, false },
                    { 733, 75, null, 80, null, "Users", "MsSQL2022.Users", 2, 2, false }
                });

            migrationBuilder.InsertData(
                table: "Schedule",
                columns: new[] { "ScheduleID", "Enabled", "FromTime", "HistoriesNumber", "Interval", "LastRun", "MaxExecutionTime", "NextRun", "PackageID", "PriorityID", "ScheduleName", "ScheduleTypeID", "StartTime", "TaskID", "ToTime", "WeekMonthDay" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2000, 1, 1, 11, 0, 0, 0, DateTimeKind.Utc), 7, 0, null, 3600, new DateTime(2010, 7, 16, 12, 53, 2, 470, DateTimeKind.Utc), 1, "Normal", "Calculate Disk Space", "Daily", new DateTime(2000, 1, 1, 11, 30, 0, 0, DateTimeKind.Utc), "SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE", new DateTime(2000, 1, 1, 11, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, true, new DateTime(2000, 1, 1, 11, 0, 0, 0, DateTimeKind.Utc), 7, 0, null, 3600, new DateTime(2010, 7, 16, 12, 53, 2, 477, DateTimeKind.Utc), 1, "Normal", "Calculate Bandwidth", "Daily", new DateTime(2000, 1, 1, 11, 0, 0, 0, DateTimeKind.Utc), "SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH", new DateTime(2000, 1, 1, 11, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                table: "ServiceDefaultProperties",
                columns: new[] { "PropertyName", "ProviderID", "PropertyValue" },
                values: new object[,]
                {
                    { "UsersHome", 1, "%SYSTEMDRIVE%\\HostingSpaces" },
                    { "AspNet11Path", 2, "%SYSTEMROOT%\\Microsoft.NET\\Framework\\v1.1.4322\\aspnet_isapi.dll" },
                    { "AspNet11Pool", 2, "ASP.NET V1.1" },
                    { "AspNet20Path", 2, "%SYSTEMROOT%\\Microsoft.NET\\Framework\\v2.0.50727\\aspnet_isapi.dll" },
                    { "AspNet20Pool", 2, "ASP.NET V2.0" },
                    { "AspNet40Path", 2, "%SYSTEMROOT%\\Microsoft.NET\\Framework\\v4.0.30319\\aspnet_isapi.dll" },
                    { "AspNet40Pool", 2, "ASP.NET V4.0" },
                    { "AspPath", 2, "%SYSTEMROOT%\\System32\\InetSrv\\asp.dll" },
                    { "CFFlashRemotingDirectory", 2, "C:\\ColdFusion9\\runtime\\lib\\wsconfig\\1" },
                    { "CFScriptsDirectory", 2, "C:\\Inetpub\\wwwroot\\CFIDE" },
                    { "ColdFusionPath", 2, "C:\\ColdFusion9\\runtime\\lib\\wsconfig\\jrun_iis6.dll" },
                    { "GalleryXmlFeedUrl", 2, "" },
                    { "PerlPath", 2, "%SYSTEMDRIVE%\\Perl\\bin\\Perl.exe" },
                    { "Php4Path", 2, "%PROGRAMFILES%\\PHP\\php.exe" },
                    { "Php5Path", 2, "%PROGRAMFILES%\\PHP\\php-cgi.exe" },
                    { "ProtectedAccessFile", 2, ".htaccess" },
                    { "ProtectedFoldersFile", 2, ".htfolders" },
                    { "ProtectedGroupsFile", 2, ".htgroup" },
                    { "ProtectedUsersFile", 2, ".htpasswd" },
                    { "PythonPath", 2, "%SYSTEMDRIVE%\\Python\\python.exe" },
                    { "SecuredFoldersFilterPath", 2, "%SYSTEMROOT%\\System32\\InetSrv\\IISPasswordFilter.dll" },
                    { "WebGroupName", 2, "FCPWebUsers" },
                    { "FtpGroupName", 3, "FCPFtpUsers" },
                    { "SiteId", 3, "MSFTPSVC/1" },
                    { "DatabaseLocation", 5, "%SYSTEMDRIVE%\\SQL2000Databases\\[USER_NAME]" },
                    { "ExternalAddress", 5, "(local)" },
                    { "InternalAddress", 5, "(local)" },
                    { "SaLogin", 5, "sa" },
                    { "SaPassword", 5, "" },
                    { "UseDefaultDatabaseLocation", 5, "True" },
                    { "UseTrustedConnection", 5, "True" },
                    { "ExternalAddress", 6, "localhost" },
                    { "InstallFolder", 6, "%PROGRAMFILES%\\MySQL\\MySQL Server 4.1" },
                    { "InternalAddress", 6, "localhost,3306" },
                    { "RootLogin", 6, "root" },
                    { "RootPassword", 6, "" },
                    { "ExpireLimit", 7, "1209600" },
                    { "MinimumTTL", 7, "86400" },
                    { "NameServers", 7, "ns1.yourdomain.com;ns2.yourdomain.com" },
                    { "RecordDefaultTTL", 7, "86400" },
                    { "RecordMinimumTTL", 7, "3600" },
                    { "RefreshInterval", 7, "3600" },
                    { "ResponsiblePerson", 7, "hostmaster.[DOMAIN_NAME]" },
                    { "RetryDelay", 7, "600" },
                    { "AwStatsFolder", 8, "%SYSTEMDRIVE%\\AWStats\\wwwroot\\cgi-bin" },
                    { "BatchFileName", 8, "UpdateStats.bat" },
                    { "BatchLineTemplate", 8, "%SYSTEMDRIVE%\\perl\\bin\\perl.exe awstats.pl config=[DOMAIN_NAME] -update" },
                    { "ConfigFileName", 8, "awstats.[DOMAIN_NAME].conf" },
                    { "ConfigFileTemplate", 8, "LogFormat = \"%time2 %other %other %other %method %url %other %other %logname %host %other %ua %other %referer %other %code %other %other %bytesd %other %other\"\r\nLogSeparator = \" \"\r\nDNSLookup = 2\r\nDirCgi = \"/cgi-bin\"\r\nDirIcons = \"/icon\"\r\nAllowFullYearView=3\r\nAllowToUpdateStatsFromBrowser = 0\r\nUseFramesWhenCGI = 1\r\nShowFlagLinks = \"en fr de it nl es\"\r\nLogFile = \"[LOGS_FOLDER]\\ex%YY-3%MM-3%DD-3.log\"\r\nDirData = \"%SYSTEMDRIVE%\\AWStats\\data\"\r\nSiteDomain = \"[DOMAIN_NAME]\"\r\nHostAliases = [DOMAIN_ALIASES]" },
                    { "StatisticsURL", 8, "http://127.0.0.1/AWStats/cgi-bin/awstats.pl?config=[domain_name]" },
                    { "AdminLogin", 9, "Admin" },
                    { "ExpireLimit", 9, "1209600" },
                    { "MinimumTTL", 9, "86400" },
                    { "NameServers", 9, "ns1.yourdomain.com;ns2.yourdomain.com" },
                    { "RecordDefaultTTL", 9, "86400" },
                    { "RecordMinimumTTL", 9, "3600" },
                    { "RefreshInterval", 9, "3600" },
                    { "ResponsiblePerson", 9, "hostmaster.[DOMAIN_NAME]" },
                    { "RetryDelay", 9, "600" },
                    { "SimpleDnsUrl", 9, "http://127.0.0.1:8053" },
                    { "LogDeleteDays", 10, "0" },
                    { "LogFormat", 10, "W3Cex" },
                    { "LogWildcard", 10, "*.log" },
                    { "Password", 10, "" },
                    { "ServerID", 10, "1" },
                    { "SmarterLogDeleteMonths", 10, "0" },
                    { "SmarterLogsPath", 10, "%SYSTEMDRIVE%\\SmarterLogs" },
                    { "SmarterUrl", 10, "http://127.0.0.1:9999/services" },
                    { "StatisticsURL", 10, "http://127.0.0.1:9999/Login.aspx?txtSiteID=[site_id]&txtUser=[username]&txtPass=[password]&shortcutLink=autologin" },
                    { "TimeZoneId", 10, "27" },
                    { "Username", 10, "Admin" },
                    { "AdminPassword", 11, "" },
                    { "AdminUsername", 11, "admin" },
                    { "defaultdomainhostname", 11, "mail.[DOMAIN_NAME]" },
                    { "DomainsPath", 11, "%SYSTEMDRIVE%\\SmarterMail" },
                    { "ServerIPAddress", 11, "127.0.0.1;127.0.0.1" },
                    { "ServiceUrl", 11, "http://127.0.0.1:9998/services" },
                    { "InstallFolder", 12, "%PROGRAMFILES%\\Gene6 FTP Server" },
                    { "LogsFolder", 12, "%PROGRAMFILES%\\Gene6 FTP Server\\Log" },
                    { "AdminPassword", 14, "" },
                    { "AdminUsername", 14, "admin" },
                    { "defaultdomainhostname", 14, "mail.[DOMAIN_NAME]" },
                    { "DomainsPath", 14, "%SYSTEMDRIVE%\\SmarterMail" },
                    { "ServerIPAddress", 14, "127.0.0.1;127.0.0.1" },
                    { "ServiceUrl", 14, "http://127.0.0.1:9998/services" },
                    { "BrowseMethod", 16, "POST" },
                    { "BrowseParameters", 16, "ServerName=[SERVER]\r\nLogin=[USER]\r\nPassword=[PASSWORD]\r\nProtocol=dbmssocn" },
                    { "BrowseURL", 16, "http://localhost/MLA/silentlogon.aspx" },
                    { "DatabaseLocation", 16, "%SYSTEMDRIVE%\\SQL2005Databases\\[USER_NAME]" },
                    { "ExternalAddress", 16, "(local)" },
                    { "InternalAddress", 16, "(local)" },
                    { "SaLogin", 16, "sa" },
                    { "SaPassword", 16, "" },
                    { "UseDefaultDatabaseLocation", 16, "True" },
                    { "UseTrustedConnection", 16, "True" },
                    { "ExternalAddress", 17, "localhost" },
                    { "InstallFolder", 17, "%PROGRAMFILES%\\MySQL\\MySQL Server 5.0" },
                    { "InternalAddress", 17, "localhost,3306" },
                    { "RootLogin", 17, "root" },
                    { "RootPassword", 17, "" },
                    { "AdminPassword", 22, "" },
                    { "AdminUsername", 22, "Administrator" },
                    { "DomainId", 25, "1" },
                    { "KeepDeletedItemsDays", 27, "14" },
                    { "KeepDeletedMailboxesDays", 27, "30" },
                    { "MailboxDatabase", 27, "Hosted Exchange Database" },
                    { "RootOU", 27, "FCP Hosting" },
                    { "StorageGroup", 27, "Hosted Exchange Storage Group" },
                    { "TempDomain", 27, "my-temp-domain.com" },
                    { "AdminPassword", 29, " " },
                    { "AdminUsername", 29, "admin" },
                    { "defaultdomainhostname", 29, "mail.[DOMAIN_NAME]" },
                    { "DomainsPath", 29, "%SYSTEMDRIVE%\\SmarterMail" },
                    { "ServerIPAddress", 29, "127.0.0.1;127.0.0.1" },
                    { "ServiceUrl", 29, "http://localhost:9998/services/" },
                    { "ExternalAddress", 30, "localhost" },
                    { "InstallFolder", 30, "%PROGRAMFILES%\\MySQL\\MySQL Server 5.1" },
                    { "InternalAddress", 30, "localhost,3306" },
                    { "RootLogin", 30, "root" },
                    { "RootPassword", 30, "" },
                    { "LogDeleteDays", 31, "0" },
                    { "LogFormat", 31, "W3Cex" },
                    { "LogWildcard", 31, "*.log" },
                    { "Password", 31, "" },
                    { "ServerID", 31, "1" },
                    { "SmarterLogDeleteMonths", 31, "0" },
                    { "SmarterLogsPath", 31, "%SYSTEMDRIVE%\\SmarterLogs" },
                    { "SmarterUrl", 31, "http://127.0.0.1:9999/services" },
                    { "StatisticsURL", 31, "http://127.0.0.1:9999/Login.aspx?txtSiteID=[site_id]&txtUser=[username]&txtPass=[password]&shortcutLink=autologin" },
                    { "TimeZoneId", 31, "27" },
                    { "Username", 31, "Admin" },
                    { "KeepDeletedItemsDays", 32, "14" },
                    { "KeepDeletedMailboxesDays", 32, "30" },
                    { "MailboxDatabase", 32, "Hosted Exchange Database" },
                    { "RootOU", 32, "FCP Hosting" },
                    { "TempDomain", 32, "my-temp-domain.com" },
                    { "ExpireLimit", 56, "1209600" },
                    { "MinimumTTL", 56, "86400" },
                    { "NameServers", 56, "ns1.yourdomain.com;ns2.yourdomain.com" },
                    { "PDNSDbName", 56, "pdnsdb" },
                    { "PDNSDbPort", 56, "3306" },
                    { "PDNSDbServer", 56, "localhost" },
                    { "PDNSDbUser", 56, "root" },
                    { "RecordDefaultTTL", 56, "86400" },
                    { "RecordMinimumTTL", 56, "3600" },
                    { "RefreshInterval", 56, "3600" },
                    { "ResponsiblePerson", 56, "hostmaster.[DOMAIN_NAME]" },
                    { "RetryDelay", 56, "600" },
                    { "AdminPassword", 60, " " },
                    { "AdminUsername", 60, "admin" },
                    { "defaultdomainhostname", 60, "mail.[DOMAIN_NAME]" },
                    { "DomainsPath", 60, "%SYSTEMDRIVE%\\SmarterMail" },
                    { "ServerIPAddress", 60, "127.0.0.1;127.0.0.1" },
                    { "ServiceUrl", 60, "http://localhost:9998/services/" },
                    { "LogDeleteDays", 62, "0" },
                    { "LogFormat", 62, "W3Cex" },
                    { "LogWildcard", 62, "*.log" },
                    { "Password", 62, "" },
                    { "ServerID", 62, "1" },
                    { "SmarterLogDeleteMonths", 62, "0" },
                    { "SmarterLogsPath", 62, "%SYSTEMDRIVE%\\SmarterLogs" },
                    { "SmarterUrl", 62, "http://127.0.0.1:9999/services" },
                    { "StatisticsURL", 62, "http://127.0.0.1:9999/Login.aspx?txtSiteID=[site_id]&txtUser=[username]&txtPass=[password]&shortcutLink=autologin" },
                    { "TimeZoneId", 62, "27" },
                    { "Username", 62, "Admin" },
                    { "AdminPassword", 63, "" },
                    { "AdminUsername", 63, "Administrator" },
                    { "AdminPassword", 64, "" },
                    { "AdminUsername", 64, "admin" },
                    { "defaultdomainhostname", 64, "mail.[DOMAIN_NAME]" },
                    { "DomainsPath", 64, "%SYSTEMDRIVE%\\SmarterMail" },
                    { "ServerIPAddress", 64, "127.0.0.1;127.0.0.1" },
                    { "ServiceUrl", 64, "http://localhost:9998/services/" },
                    { "AdminPassword", 65, "" },
                    { "AdminUsername", 65, "admin" },
                    { "defaultdomainhostname", 65, "mail.[DOMAIN_NAME]" },
                    { "DomainsPath", 65, "%SYSTEMDRIVE%\\SmarterMail" },
                    { "ServerIPAddress", 65, "127.0.0.1;127.0.0.1" },
                    { "ServiceUrl", 65, "http://localhost:9998/services/" },
                    { "AdminPassword", 66, "" },
                    { "AdminUsername", 66, "admin" },
                    { "defaultdomainhostname", 66, "mail.[DOMAIN_NAME]" },
                    { "DomainsPath", 66, "%SYSTEMDRIVE%\\SmarterMail" },
                    { "ServerIPAddress", 66, "127.0.0.1;127.0.0.1" },
                    { "ServiceUrl", 66, "http://localhost:9998/services/" },
                    { "AdminPassword", 67, "" },
                    { "AdminUsername", 67, "admin" },
                    { "defaultdomainhostname", 67, "mail.[DOMAIN_NAME]" },
                    { "DomainsPath", 67, "%SYSTEMDRIVE%\\SmarterMail\\Domains" },
                    { "ServerIPAddress", 67, "127.0.0.1;127.0.0.1" },
                    { "ServiceUrl", 67, "http://localhost:9998" },
                    { "UsersHome", 100, "%SYSTEMDRIVE%\\HostingSpaces" },
                    { "AspNet11Pool", 101, "ASP.NET 1.1" },
                    { "AspNet40Path", 101, "%WINDIR%\\Microsoft.NET\\Framework\\v4.0.30319\\aspnet_isapi.dll" },
                    { "AspNet40x64Path", 101, "%WINDIR%\\Microsoft.NET\\Framework64\\v4.0.30319\\aspnet_isapi.dll" },
                    { "AspNetBitnessMode", 101, "32" },
                    { "CFFlashRemotingDirectory", 101, "C:\\ColdFusion9\\runtime\\lib\\wsconfig\\1" },
                    { "CFScriptsDirectory", 101, "C:\\Inetpub\\wwwroot\\CFIDE" },
                    { "ClassicAspNet20Pool", 101, "ASP.NET 2.0 (Classic)" },
                    { "ClassicAspNet40Pool", 101, "ASP.NET 4.0 (Classic)" },
                    { "ColdFusionPath", 101, "C:\\ColdFusion9\\runtime\\lib\\wsconfig\\jrun_iis6.dll" },
                    { "GalleryXmlFeedUrl", 101, "" },
                    { "IntegratedAspNet20Pool", 101, "ASP.NET 2.0 (Integrated)" },
                    { "IntegratedAspNet40Pool", 101, "ASP.NET 4.0 (Integrated)" },
                    { "PerlPath", 101, "%SYSTEMDRIVE%\\Perl\\bin\\PerlEx30.dll" },
                    { "Php4Path", 101, "%PROGRAMFILES(x86)%\\PHP\\php.exe" },
                    { "PhpMode", 101, "FastCGI" },
                    { "PhpPath", 101, "%PROGRAMFILES(x86)%\\PHP\\php-cgi.exe" },
                    { "ProtectedGroupsFile", 101, ".htgroup" },
                    { "ProtectedUsersFile", 101, ".htpasswd" },
                    { "SecureFoldersModuleAssembly", 101, "FuseCP.IIsModules.SecureFolders, FuseCP.IIsModules, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=37f9c58a0aa32ff0" },
                    { "WebGroupName", 101, "FCP_IUSRS" },
                    { "WmSvc.CredentialsMode", 101, "WINDOWS" },
                    { "WmSvc.Port", 101, "8172" },
                    { "FtpGroupName", 102, "FCPFtpUsers" },
                    { "SiteId", 102, "Default FTP Site" },
                    { "UsersHome", 104, "%SYSTEMDRIVE%\\HostingSpaces" },
                    { "AspNet11Pool", 105, "ASP.NET 1.1" },
                    { "AspNet40Path", 105, "%WINDIR%\\Microsoft.NET\\Framework\\v4.0.30319\\aspnet_isapi.dll" },
                    { "AspNet40x64Path", 105, "%WINDIR%\\Microsoft.NET\\Framework64\\v4.0.30319\\aspnet_isapi.dll" },
                    { "AspNetBitnessMode", 105, "32" },
                    { "CFFlashRemotingDirectory", 105, "C:\\ColdFusion9\\runtime\\lib\\wsconfig\\1" },
                    { "CFScriptsDirectory", 105, "C:\\Inetpub\\wwwroot\\CFIDE" },
                    { "ClassicAspNet20Pool", 105, "ASP.NET 2.0 (Classic)" },
                    { "ClassicAspNet40Pool", 105, "ASP.NET 4.0 (Classic)" },
                    { "ColdFusionPath", 105, "C:\\ColdFusion9\\runtime\\lib\\wsconfig\\jrun_iis6.dll" },
                    { "GalleryXmlFeedUrl", 105, "" },
                    { "IntegratedAspNet20Pool", 105, "ASP.NET 2.0 (Integrated)" },
                    { "IntegratedAspNet40Pool", 105, "ASP.NET 4.0 (Integrated)" },
                    { "PerlPath", 105, "%SYSTEMDRIVE%\\Perl\\bin\\PerlEx30.dll" },
                    { "Php4Path", 105, "%PROGRAMFILES(x86)%\\PHP\\php.exe" },
                    { "PhpMode", 105, "FastCGI" },
                    { "PhpPath", 105, "%PROGRAMFILES(x86)%\\PHP\\php-cgi.exe" },
                    { "ProtectedGroupsFile", 105, ".htgroup" },
                    { "ProtectedUsersFile", 105, ".htpasswd" },
                    { "SecureFoldersModuleAssembly", 105, "FuseCP.IIsModules.SecureFolders, FuseCP.IIsModules, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=37f9c58a0aa32ff0" },
                    { "sslusesni", 105, "True" },
                    { "WebGroupName", 105, "FCP_IUSRS" },
                    { "WmSvc.CredentialsMode", 105, "WINDOWS" },
                    { "WmSvc.Port", 105, "8172" },
                    { "FtpGroupName", 106, "FCPFtpUsers" },
                    { "SiteId", 106, "Default FTP Site" },
                    { "sslusesni", 106, "False" },
                    { "UsersHome", 111, "%SYSTEMDRIVE%\\HostingSpaces" },
                    { "AspNet11Pool", 112, "ASP.NET 1.1" },
                    { "AspNet40Path", 112, "%WINDIR%\\Microsoft.NET\\Framework\\v4.0.30319\\aspnet_isapi.dll" },
                    { "AspNet40x64Path", 112, "%WINDIR%\\Microsoft.NET\\Framework64\\v4.0.30319\\aspnet_isapi.dll" },
                    { "AspNetBitnessMode", 112, "32" },
                    { "CFFlashRemotingDirectory", 112, "C:\\ColdFusion9\\runtime\\lib\\wsconfig\\1" },
                    { "CFScriptsDirectory", 112, "C:\\Inetpub\\wwwroot\\CFIDE" },
                    { "ClassicAspNet20Pool", 112, "ASP.NET 2.0 (Classic)" },
                    { "ClassicAspNet40Pool", 112, "ASP.NET 4.0 (Classic)" },
                    { "ColdFusionPath", 112, "C:\\ColdFusion9\\runtime\\lib\\wsconfig\\jrun_iis6.dll" },
                    { "GalleryXmlFeedUrl", 112, "" },
                    { "IntegratedAspNet20Pool", 112, "ASP.NET 2.0 (Integrated)" },
                    { "IntegratedAspNet40Pool", 112, "ASP.NET 4.0 (Integrated)" },
                    { "PerlPath", 112, "%SYSTEMDRIVE%\\Perl\\bin\\PerlEx30.dll" },
                    { "Php4Path", 112, "%PROGRAMFILES(x86)%\\PHP\\php.exe" },
                    { "PhpMode", 112, "FastCGI" },
                    { "PhpPath", 112, "%PROGRAMFILES(x86)%\\PHP\\php-cgi.exe" },
                    { "ProtectedGroupsFile", 112, ".htgroup" },
                    { "ProtectedUsersFile", 112, ".htpasswd" },
                    { "SecureFoldersModuleAssembly", 112, "FuseCP.IIsModules.SecureFolders, FuseCP.IIsModules, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=37f9c58a0aa32ff0" },
                    { "sslusesni", 112, "True" },
                    { "WebGroupName", 112, "FCP_IUSRS" },
                    { "WmSvc.CredentialsMode", 112, "WINDOWS" },
                    { "WmSvc.Port", 112, "8172" },
                    { "FtpGroupName", 113, "FCPFtpUsers" },
                    { "SiteId", 113, "Default FTP Site" },
                    { "sslusesni", 113, "False" },
                    { "RootWebApplicationIpAddress", 200, "" },
                    { "UserName", 204, "admin" },
                    { "UtilityPath", 204, "C:\\Program Files\\Research In Motion\\BlackBerry Enterprise Server Resource Kit\\BlackBerry Enterprise Server User Administration Tool" },
                    { "CpuLimit", 300, "100" },
                    { "CpuReserve", 300, "0" },
                    { "CpuWeight", 300, "100" },
                    { "DvdLibraryPath", 300, "C:\\Hyper-V\\Library" },
                    { "ExportedVpsPath", 300, "C:\\Hyper-V\\Exported" },
                    { "HostnamePattern", 300, "vps[user_id].hosterdomain.com" },
                    { "OsTemplatesPath", 300, "C:\\Hyper-V\\Templates" },
                    { "PrivateNetworkFormat", 300, "192.168.0.1/16" },
                    { "RootFolder", 300, "C:\\Hyper-V\\VirtualMachines\\[VPS_HOSTNAME]" },
                    { "StartAction", 300, "start" },
                    { "StartupDelay", 300, "0" },
                    { "StopAction", 300, "shutDown" },
                    { "VirtualDiskType", 300, "dynamic" },
                    { "ExternalAddress", 301, "localhost" },
                    { "InstallFolder", 301, "%PROGRAMFILES%\\MySQL\\MySQL Server 5.5" },
                    { "InternalAddress", 301, "localhost,3306" },
                    { "RootLogin", 301, "root" },
                    { "RootPassword", 301, "" },
                    { "ExternalAddress", 304, "localhost" },
                    { "InstallFolder", 304, "%PROGRAMFILES%\\MySQL\\MySQL Server 8.0" },
                    { "InternalAddress", 304, "localhost,3306" },
                    { "RootLogin", 304, "root" },
                    { "RootPassword", 304, "" },
                    { "sslmode", 304, "True" },
                    { "admode", 410, "False" },
                    { "expirelimit", 410, "1209600" },
                    { "minimumttl", 410, "86400" },
                    { "nameservers", 410, "ns1.yourdomain.com;ns2.yourdomain.com" },
                    { "RecordDefaultTTL", 410, "86400" },
                    { "RecordMinimumTTL", 410, "3600" },
                    { "refreshinterval", 410, "3600" },
                    { "responsibleperson", 410, "hostmaster.[DOMAIN_NAME]" },
                    { "retrydelay", 410, "600" },
                    { "ExternalAddress", 1550, "localhost" },
                    { "InstallFolder", 1550, "%PROGRAMFILES%\\MariaDB 10.1" },
                    { "InternalAddress", 1550, "localhost" },
                    { "RootLogin", 1550, "root" },
                    { "RootPassword", 1550, "" },
                    { "ExternalAddress", 1570, "localhost" },
                    { "InstallFolder", 1570, "%PROGRAMFILES%\\MariaDB 10.3" },
                    { "InternalAddress", 1570, "localhost" },
                    { "RootLogin", 1570, "root" },
                    { "RootPassword", 1570, "" },
                    { "ExternalAddress", 1571, "localhost" },
                    { "InstallFolder", 1571, "%PROGRAMFILES%\\MariaDB 10.4" },
                    { "InternalAddress", 1571, "localhost" },
                    { "RootLogin", 1571, "root" },
                    { "RootPassword", 1571, "" },
                    { "ExternalAddress", 1572, "localhost" },
                    { "InstallFolder", 1572, "%PROGRAMFILES%\\MariaDB 10.5" },
                    { "InternalAddress", 1572, "localhost" },
                    { "RootLogin", 1572, "root" },
                    { "RootPassword", 1572, "" },
                    { "RecordDefaultTTL", 1703, "86400" },
                    { "RecordMinimumTTL", 1703, "3600" },
                    { "UsersHome", 1800, "%SYSTEMDRIVE%\\HostingSpaces" },
                    { "UsersHome", 1802, "%SYSTEMDRIVE%\\HostingSpaces" },
                    { "UsersHome", 1804, "%SYSTEMDRIVE%\\HostingSpaces" },
                    { "AdminLogin", 1901, "Admin" },
                    { "ExpireLimit", 1901, "1209600" },
                    { "MinimumTTL", 1901, "86400" },
                    { "NameServers", 1901, "ns1.yourdomain.com;ns2.yourdomain.com" },
                    { "RecordDefaultTTL", 1901, "86400" },
                    { "RecordMinimumTTL", 1901, "3600" },
                    { "RefreshInterval", 1901, "3600" },
                    { "ResponsiblePerson", 1901, "hostmaster.[DOMAIN_NAME]" },
                    { "RetryDelay", 1901, "600" },
                    { "SimpleDnsUrl", 1901, "http://127.0.0.1:8053" },
                    { "admode", 1902, "False" },
                    { "expirelimit", 1902, "1209600" },
                    { "minimumttl", 1902, "86400" },
                    { "nameservers", 1902, "ns1.yourdomain.com;ns2.yourdomain.com" },
                    { "RecordDefaultTTL", 1902, "86400" },
                    { "RecordMinimumTTL", 1902, "3600" },
                    { "refreshinterval", 1902, "3600" },
                    { "responsibleperson", 1902, "hostmaster.[DOMAIN_NAME]" },
                    { "retrydelay", 1902, "600" },
                    { "AdminLogin", 1903, "Admin" },
                    { "ExpireLimit", 1903, "1209600" },
                    { "NameServers", 1903, "ns1.yourdomain.com;ns2.yourdomain.com" },
                    { "RecordDefaultTTL", 1903, "86400" },
                    { "RecordMinimumTTL", 1903, "3600" },
                    { "RefreshInterval", 1903, "3600" },
                    { "ResponsiblePerson", 1903, "hostmaster.[DOMAIN_NAME]" },
                    { "RetryDelay", 1903, "600" },
                    { "SimpleDnsUrl", 1903, "http://127.0.0.1:8053" }
                });

            migrationBuilder.InsertData(
                table: "ScheduleParameters",
                columns: new[] { "ParameterID", "ScheduleID", "ParameterValue" },
                values: new object[,]
                {
                    { "SUSPEND_OVERUSED", 1, "false" },
                    { "SUSPEND_OVERUSED", 2, "false" }
                });

            migrationBuilder.CreateIndex(
                name: "AccessTokensIdx_AccountID",
                table: "AccessTokens",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "BackgroundTaskLogsIdx_TaskID",
                table: "BackgroundTaskLogs",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "BackgroundTaskParametersIdx_TaskID",
                table: "BackgroundTaskParameters",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "BackgroundTaskStackIdx_TaskID",
                table: "BackgroundTaskStack",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "BlackBerryUsersIdx_AccountId",
                table: "BlackBerryUsers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "CommentsIdx_UserID",
                table: "Comments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "CRMUsersIdx_AccountID",
                table: "CRMUsers",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "DmzIPAddressesIdx_ItemID",
                table: "DmzIPAddresses",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "DomainDnsRecordsIdx_DomainId",
                table: "DomainDnsRecords",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "DomainsIdx_MailDomainID",
                table: "Domains",
                column: "MailDomainID");

            migrationBuilder.CreateIndex(
                name: "DomainsIdx_PackageID",
                table: "Domains",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "DomainsIdx_WebSiteID",
                table: "Domains",
                column: "WebSiteID");

            migrationBuilder.CreateIndex(
                name: "DomainsIdx_ZoneItemID",
                table: "Domains",
                column: "ZoneItemID");

            migrationBuilder.CreateIndex(
                name: "EnterpriseFoldersIdx_StorageSpaceFolderId",
                table: "EnterpriseFolders",
                column: "StorageSpaceFolderId");

            migrationBuilder.CreateIndex(
                name: "EnterpriseFoldersOwaPermissionsIdx_AccountID",
                table: "EnterpriseFoldersOwaPermissions",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "EnterpriseFoldersOwaPermissionsIdx_FolderID",
                table: "EnterpriseFoldersOwaPermissions",
                column: "FolderID");

            migrationBuilder.CreateIndex(
                name: "ExchangeAccountEmailAddressesIdx_AccountID",
                table: "ExchangeAccountEmailAddresses",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeAccountEmailAddresses_UniqueEmail",
                table: "ExchangeAccountEmailAddresses",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ExchangeAccountsIdx_ItemID",
                table: "ExchangeAccounts",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "ExchangeAccountsIdx_MailboxPlanId",
                table: "ExchangeAccounts",
                column: "MailboxPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeAccounts_UniqueAccountName",
                table: "ExchangeAccounts",
                column: "AccountName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ExchangeMailboxPlansIdx_ItemID",
                table: "ExchangeMailboxPlans",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeMailboxPlans",
                table: "ExchangeMailboxPlans",
                column: "MailboxPlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ExchangeOrganizationDomainsIdx_ItemID",
                table: "ExchangeOrganizationDomains",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeOrganizationDomains_UniqueDomain",
                table: "ExchangeOrganizationDomains",
                column: "DomainID",
                unique: true,
                filter: "[DomainID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeOrganizations_UniqueOrg",
                table: "ExchangeOrganizations",
                column: "OrganizationID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ExchangeOrganizationSettingsIdx_ItemId",
                table: "ExchangeOrganizationSettings",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "ExchangeOrganizationSsFoldersIdx_ItemId",
                table: "ExchangeOrganizationSsFolders",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "ExchangeOrganizationSsFoldersIdx_StorageSpaceFolderId",
                table: "ExchangeOrganizationSsFolders",
                column: "StorageSpaceFolderId");

            migrationBuilder.CreateIndex(
                name: "GlobalDnsRecordsIdx_IPAddressID",
                table: "GlobalDnsRecords",
                column: "IPAddressID");

            migrationBuilder.CreateIndex(
                name: "GlobalDnsRecordsIdx_PackageID",
                table: "GlobalDnsRecords",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "GlobalDnsRecordsIdx_ServerID",
                table: "GlobalDnsRecords",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "GlobalDnsRecordsIdx_ServiceID",
                table: "GlobalDnsRecords",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_HostingPlanQuotas_QuotaID",
                table: "HostingPlanQuotas",
                column: "QuotaID");

            migrationBuilder.CreateIndex(
                name: "IX_HostingPlanResources_GroupID",
                table: "HostingPlanResources",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "HostingPlansIdx_PackageID",
                table: "HostingPlans",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "HostingPlansIdx_ServerID",
                table: "HostingPlans",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "HostingPlansIdx_UserID",
                table: "HostingPlans",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IPAddressesIdx_ServerID",
                table: "IPAddresses",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_LyncUserPlans",
                table: "LyncUserPlans",
                column: "LyncUserPlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "LyncUserPlansIdx_ItemID",
                table: "LyncUserPlans",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "LyncUsersIdx_LyncUserPlanID",
                table: "LyncUsers",
                column: "LyncUserPlanID");

            migrationBuilder.CreateIndex(
                name: "PackageAddonsIdx_PackageID",
                table: "PackageAddons",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "PackageAddonsIdx_PlanID",
                table: "PackageAddons",
                column: "PlanID");

            migrationBuilder.CreateIndex(
                name: "PackageIPAddressesIdx_AddressID",
                table: "PackageIPAddresses",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "PackageIPAddressesIdx_ItemID",
                table: "PackageIPAddresses",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "PackageIPAddressesIdx_PackageID",
                table: "PackageIPAddresses",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "IX_PackageQuotas_QuotaID",
                table: "PackageQuotas",
                column: "QuotaID");

            migrationBuilder.CreateIndex(
                name: "IX_PackageResources_GroupID",
                table: "PackageResources",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "PackageIndex_ParentPackageID",
                table: "Packages",
                column: "ParentPackageID");

            migrationBuilder.CreateIndex(
                name: "PackageIndex_PlanID",
                table: "Packages",
                column: "PlanID");

            migrationBuilder.CreateIndex(
                name: "PackageIndex_ServerID",
                table: "Packages",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "PackageIndex_UserID",
                table: "Packages",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_PackagesBandwidth_GroupID",
                table: "PackagesBandwidth",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_PackagesDiskspace_GroupID",
                table: "PackagesDiskspace",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_PackageServices_ServiceID",
                table: "PackageServices",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_PackagesTreeCache_PackageID",
                table: "PackagesTreeCache",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "PackageVLANsIdx_PackageID",
                table: "PackageVLANs",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "PackageVLANsIdx_VlanID",
                table: "PackageVLANs",
                column: "VlanID");

            migrationBuilder.CreateIndex(
                name: "PrivateIPAddressesIdx_ItemID",
                table: "PrivateIPAddresses",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "PrivateNetworkVLANsIdx_ServerID",
                table: "PrivateNetworkVLANs",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "ProvidersIdx_GroupID",
                table: "Providers",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "QuotasIdx_GroupID",
                table: "Quotas",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "QuotasIdx_ItemTypeID",
                table: "Quotas",
                column: "ItemTypeID");

            migrationBuilder.CreateIndex(
                name: "RDSCollectionSettingsIdx_RDSCollectionId",
                table: "RDSCollectionSettings",
                column: "RDSCollectionId");

            migrationBuilder.CreateIndex(
                name: "RDSCollectionUsersIdx_AccountID",
                table: "RDSCollectionUsers",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "RDSCollectionUsersIdx_RDSCollectionId",
                table: "RDSCollectionUsers",
                column: "RDSCollectionId");

            migrationBuilder.CreateIndex(
                name: "RDSMessagesIdx_RDSCollectionId",
                table: "RDSMessages",
                column: "RDSCollectionId");

            migrationBuilder.CreateIndex(
                name: "RDSServersIdx_RDSCollectionId",
                table: "RDSServers",
                column: "RDSCollectionId");

            migrationBuilder.CreateIndex(
                name: "ResourceGroupDnsRecordsIdx_GroupID",
                table: "ResourceGroupDnsRecords",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "ScheduleIdx_PackageID",
                table: "Schedule",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "ScheduleIdx_TaskID",
                table: "Schedule",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTaskViewConfiguration_TaskID",
                table: "ScheduleTaskViewConfiguration",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "ServersIdx_PrimaryGroupID",
                table: "Servers",
                column: "PrimaryGroupID");

            migrationBuilder.CreateIndex(
                name: "ServiceItemsIdx_ItemTypeID",
                table: "ServiceItems",
                column: "ItemTypeID");

            migrationBuilder.CreateIndex(
                name: "ServiceItemsIdx_PackageID",
                table: "ServiceItems",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "ServiceItemsIdx_ServiceID",
                table: "ServiceItems",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "ServiceItemTypesIdx_GroupID",
                table: "ServiceItemTypes",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "ServicesIdx_ClusterID",
                table: "Services",
                column: "ClusterID");

            migrationBuilder.CreateIndex(
                name: "ServicesIdx_ProviderID",
                table: "Services",
                column: "ProviderID");

            migrationBuilder.CreateIndex(
                name: "ServicesIdx_ServerID",
                table: "Services",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "StorageSpaceFoldersIdx_StorageSpaceId",
                table: "StorageSpaceFolders",
                column: "StorageSpaceId");

            migrationBuilder.CreateIndex(
                name: "StorageSpaceLevelResourceGroupsIdx_GroupId",
                table: "StorageSpaceLevelResourceGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "StorageSpaceLevelResourceGroupsIdx_LevelId",
                table: "StorageSpaceLevelResourceGroups",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "StorageSpacesIdx_ServerId",
                table: "StorageSpaces",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "StorageSpacesIdx_ServiceId",
                table: "StorageSpaces",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "ThemeSettingsIdx_ThemeID",
                table: "ThemeSettings",
                column: "ThemeID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UsersIdx_OwnerID",
                table: "Users",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "VirtualGroupsIdx_GroupID",
                table: "VirtualGroups",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "VirtualGroupsIdx_ServerID",
                table: "VirtualGroups",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "VirtualServicesIdx_ServerID",
                table: "VirtualServices",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "VirtualServicesIdx_ServiceID",
                table: "VirtualServices",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "WebDavAccessTokensIdx_AccountID",
                table: "WebDavAccessTokens",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "WebDavPortalUsersSettingsIdx_AccountId",
                table: "WebDavPortalUsersSettings",
                column: "AccountId");

            if (migrationBuilder.IsSqlServer())
            {
                migrationBuilder.SqlScript(@"!\.SqlServer\.v1\.5\.1\.ViewsAndTriggers\.sql$");
                migrationBuilder.SqlScript(@"!\.SqlServer\.v1\.5\.1\.StoredProcedures\.sql$");
            }
        }

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsSqlServer())
            {
                migrationBuilder.SqlScript(@"!\.SqlServer\.v1\.5\.1\.DropStoredProcedures\.sql$");
                migrationBuilder.SqlScript(@"!\.SqlServer\.v1\.5\.1\.DropViewsAndTriggers\.sql$");
            }

			migrationBuilder.DropTable(
                name: "AccessTokens");

            migrationBuilder.DropTable(
                name: "AdditionalGroups");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "AuditLogSources");

            migrationBuilder.DropTable(
                name: "AuditLogTasks");

            migrationBuilder.DropTable(
                name: "BackgroundTaskLogs");

            migrationBuilder.DropTable(
                name: "BackgroundTaskParameters");

            migrationBuilder.DropTable(
                name: "BackgroundTaskStack");

            migrationBuilder.DropTable(
                name: "BlackBerryUsers");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CRMUsers");

            migrationBuilder.DropTable(
                name: "DmzIPAddresses");

            migrationBuilder.DropTable(
                name: "DomainDnsRecords");

            migrationBuilder.DropTable(
                name: "EnterpriseFoldersOwaPermissions");

            migrationBuilder.DropTable(
                name: "ExchangeAccountEmailAddresses");

            migrationBuilder.DropTable(
                name: "ExchangeDeletedAccounts");

            migrationBuilder.DropTable(
                name: "ExchangeDisclaimers");

            migrationBuilder.DropTable(
                name: "ExchangeMailboxPlanRetentionPolicyTags");

            migrationBuilder.DropTable(
                name: "ExchangeOrganizationDomains");

            migrationBuilder.DropTable(
                name: "ExchangeOrganizationSettings");

            migrationBuilder.DropTable(
                name: "ExchangeOrganizationSsFolders");

            migrationBuilder.DropTable(
                name: "ExchangeRetentionPolicyTags");

            migrationBuilder.DropTable(
                name: "GlobalDnsRecords");

            migrationBuilder.DropTable(
                name: "HostingPlanQuotas");

            migrationBuilder.DropTable(
                name: "HostingPlanResources");

            migrationBuilder.DropTable(
                name: "LyncUsers");

            migrationBuilder.DropTable(
                name: "OCSUsers");

            migrationBuilder.DropTable(
                name: "PackageAddons");

            migrationBuilder.DropTable(
                name: "PackageIPAddresses");

            migrationBuilder.DropTable(
                name: "PackageQuotas");

            migrationBuilder.DropTable(
                name: "PackageResources");

            migrationBuilder.DropTable(
                name: "PackagesBandwidth");

            migrationBuilder.DropTable(
                name: "PackagesDiskspace");

            migrationBuilder.DropTable(
                name: "PackageServices");

            migrationBuilder.DropTable(
                name: "PackageSettings");

            migrationBuilder.DropTable(
                name: "PackagesTreeCache");

            migrationBuilder.DropTable(
                name: "PackageVLANs");

            migrationBuilder.DropTable(
                name: "PrivateIPAddresses");

            migrationBuilder.DropTable(
                name: "RDSCertificates");

            migrationBuilder.DropTable(
                name: "RDSCollectionSettings");

            migrationBuilder.DropTable(
                name: "RDSCollectionUsers");

            migrationBuilder.DropTable(
                name: "RDSMessages");

            migrationBuilder.DropTable(
                name: "RDSServers");

            migrationBuilder.DropTable(
                name: "RDSServerSettings");

            migrationBuilder.DropTable(
                name: "ResourceGroupDnsRecords");

            migrationBuilder.DropTable(
                name: "ScheduleParameters");

            migrationBuilder.DropTable(
                name: "ScheduleTaskParameters");

            migrationBuilder.DropTable(
                name: "ScheduleTaskViewConfiguration");

            migrationBuilder.DropTable(
                name: "ServiceDefaultProperties");

            migrationBuilder.DropTable(
                name: "ServiceItemProperties");

            migrationBuilder.DropTable(
                name: "ServiceProperties");

            migrationBuilder.DropTable(
                name: "SfBUserPlans");

            migrationBuilder.DropTable(
                name: "SfBUsers");

            migrationBuilder.DropTable(
                name: "SSLCertificates");

            migrationBuilder.DropTable(
                name: "StorageSpaceLevelResourceGroups");

            migrationBuilder.DropTable(
                name: "SupportServiceLevels");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "Themes");

            migrationBuilder.DropTable(
                name: "ThemeSettings");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "VirtualGroups");

            migrationBuilder.DropTable(
                name: "VirtualServices");

            migrationBuilder.DropTable(
                name: "WebDavAccessTokens");

            migrationBuilder.DropTable(
                name: "WebDavPortalUsersSettings");

            migrationBuilder.DropTable(
                name: "BackgroundTasks");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "EnterpriseFolders");

            migrationBuilder.DropTable(
                name: "LyncUserPlans");

            migrationBuilder.DropTable(
                name: "IPAddresses");

            migrationBuilder.DropTable(
                name: "Quotas");

            migrationBuilder.DropTable(
                name: "PrivateNetworkVLANs");

            migrationBuilder.DropTable(
                name: "RDSCollections");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "StorageSpaceLevels");

            migrationBuilder.DropTable(
                name: "ExchangeAccounts");

            migrationBuilder.DropTable(
                name: "StorageSpaceFolders");

            migrationBuilder.DropTable(
                name: "ScheduleTasks");

            migrationBuilder.DropTable(
                name: "ExchangeMailboxPlans");

            migrationBuilder.DropTable(
                name: "StorageSpaces");

            migrationBuilder.DropTable(
                name: "ExchangeOrganizations");

            migrationBuilder.DropTable(
                name: "ServiceItems");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "ServiceItemTypes");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "HostingPlans");

            migrationBuilder.DropTable(
                name: "Clusters");

            migrationBuilder.DropTable(
                name: "Providers");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ResourceGroups");
		}
	}
}



