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
using System.Text;

namespace FuseCP.EnterpriseServer
{
    public class BusinessErrorCodes
    {
        #region Servers
        public const int ERROR_ADD_SERVER_NOT_FOUND = -400;
        public const int ERROR_ADD_SERVER_BAD_REQUEST = -401;
        public const int ERROR_ADD_SERVER_INTERNAL_SERVER_ERROR = -403;
        public const int ERROR_ADD_SERVER_SERVICE_UNAVAILABLE = -404;
        public const int ERROR_ADD_SERVER_UNAUTHORIZED = -405;
        public const int ERROR_ADD_SERVER_WRONG_PASSWORD = -406;
        public const int ERROR_ADD_SERVER_APPLICATION_ERROR = -407;
        public const int ERROR_USER_SERVER_WRONG_AD_SETTINGS = -402;
        public const int ERROR_ADD_SERVER_URL_UNAVAILABLE = -409;
        #endregion

        #region Users
        public const int ERROR_USER_ALREADY_EXISTS = -100;

        public const int ERROR_USER_NOT_FOUND = -101;
        public const int ERROR_USER_HAS_USERS = -102;

        public const int ERROR_USER_ACCOUNT_PENDING = -103;
        public const int ERROR_USER_ACCOUNT_SUSPENDED = -104;
        public const int ERROR_USER_ACCOUNT_CANCELLED = -105;
        public const int ERROR_USER_ACCOUNT_DEMO = -106;
        public const int ERROR_USER_ACCOUNT_SHOULD_BE_ADMINISTRATOR = -107;
        public const int ERROR_USER_ACCOUNT_SHOULD_BE_RESELLER = -108;

        public const int ERROR_USER_WRONG_USERNAME = -109;
        public const int ERROR_USER_WRONG_PASSWORD = -110;
        public const int ERROR_INVALID_USER_NAME = -111;
        public const int ERROR_USER_ACCOUNT_NOT_ENOUGH_PERMISSIONS = -112;
        public const int ERROR_USER_ACCOUNT_ROLE_NOT_ALLOWED = -113;

        public const int ERROR_USER_ACCOUNT_DISABLED = -114;
        public const int ERROR_USER_ACCOUNT_LOCKEDOUT = -115;

        public const int ERROR_USER_EXPIRED_ONETIMEPASSWORD = -116;

        /// <summary>The originating IP address has been blocked by brute force protection.</summary>
        public const int ERROR_USER_IP_BLOCKED = -117;

        #endregion

        #region Packages
        public const int ERROR_PACKAGE_NOT_FOUND = -300;
        public const int ERROR_PACKAGE_HAS_PACKAGES = -301;

        public const int ERROR_PACKAGE_NEW = -302;
        public const int ERROR_PACKAGE_SUSPENDED = -303;
        public const int ERROR_PACKAGE_CANCELLED = -304;

        public const int ERROR_PACKAGE_QUOTA_EXCEED = -305;
        #endregion

        #region Settings
        public const int ERROR_SETTINGS_ACCOUNT_LETTER_EMPTY_BODY = -200;
        public const int ERROR_SETTINGS_PACKAGE_LETTER_EMPTY_BODY = -201;
        public const int ERROR_SETTINGS_PASSWORD_LETTER_EMPTY_BODY = -202;
        #endregion

        #region Domains
        public const int ERROR_DOMAIN_QUOTA_LIMIT = -500;
        public const int ERROR_DOMAINPOINTERS_QUOTA_LIMIT = -510;
        public const int ERROR_INSTANT_ALIAS_IS_NOT_CONFIGURED = -511;
        public const int ERROR_SUBDOMAIN_QUOTA_LIMIT = -508;
        public const int ERROR_RESTRICTED_DOMAIN = -501;
        public const int ERROR_DOMAIN_ALREADY_EXISTS = -502;
        public const int ERROR_DOMAIN_STARTS_WWW = -503;

        public const int ERROR_DOMAIN_PACKAGE_ITEM_NOT_FOUND = -504;
        public const int ERROR_DNS_PACKAGE_ITEM_NOT_FOUND = -505;
        public const int ERROR_DOMAIN_POINTS_TO_WEB_SITE = -506;
        public const int ERROR_DOMAIN_POINTS_TO_MAIL_DOMAIN = -507;
		public const int ERROR_DNS_ZONE_EXISTS = -509;
        #endregion

        #region Web Sites
        public const int ERROR_WEB_SITES_QUOTA_LIMIT = -600;
        public const int ERROR_WEB_SITE_ALREADY_EXISTS = -601;
        public const int ERROR_WEB_SITE_SERVICE_UNAVAILABLE = -602;
        public const int ERROR_FP_ACCOUNT_EXISTS = -603;
        public const int ERROR_FP_CHANGE_PASSWORD = -604;
        public const int ERROR_VDIR_ALREADY_EXISTS = -605;

        public const int ERROR_WEB_SITE_PACKAGE_ITEM_NOT_FOUND = -606;
        public const int ERROR_WEB_SITE_IP_ADDRESS_NOT_SPECIFIED = -607;
        public const int ERROR_WEB_SITE_SHARED_IP_ADDRESS_NOT_SPECIFIED = -608;
        public const int ERROR_WEB_SHARED_SSL_QUOTA_LIMIT = -609;
        public const int ERROR_GLOBALDNS_FOR_DEDICATEDIP = -610;
        public const int ERROR_PUBLICSHAREDIP_FOR_SHAREDIP = -611;
        #endregion

        #region Mail
        public const int ERROR_MAIL_DOMAIN_EXISTS = -700;
        public const int ERROR_MAIL_RESOURCE_UNAVAILABLE = -701;
        public const int ERROR_MAIL_ACCOUNTS_RESOURCE_QUOTA_LIMIT = -702;
        public const int ERROR_MAIL_ACCOUNTS_PACKAGE_ITEM_EXISTS = -703;
        public const int ERROR_MAIL_ACCOUNTS_SERVICE_ITEM_EXISTS = -704;
        
        public const int ERROR_MAIL_FORWARDINGS_RESOURCE_QUOTA_LIMIT = -705;
        public const int ERROR_MAIL_FORWARDINGS_PACKAGE_ITEM_EXISTS = -706;
        public const int ERROR_MAIL_FORWARDINGS_SERVICE_ITEM_EXISTS = -707;

        public const int ERROR_MAIL_GROUPS_RESOURCE_QUOTA_LIMIT = -708;
        public const int ERROR_MAIL_GROUPS_PACKAGE_ITEM_EXISTS = -709;
        public const int ERROR_MAIL_GROUPS_SERVICE_ITEM_EXISTS = -710;

        public const int ERROR_MAIL_LISTS_RESOURCE_QUOTA_LIMIT = -711;
        public const int ERROR_MAIL_LISTS_PACKAGE_ITEM_EXISTS = -712;
        public const int ERROR_MAIL_LISTS_SERVICE_ITEM_EXISTS = -713;

        public const int ERROR_MAIL_ACCOUNTS_PACKAGE_ITEM_NOT_FOUND = -714;
        public const int ERROR_MAIL_FORWARDINGS_PACKAGE_ITEM_NOT_FOUND = -715;
        public const int ERROR_MAIL_GROUPS_PACKAGE_ITEM_NOT_FOUND = -716;
        public const int ERROR_MAIL_LISTS_PACKAGE_ITEM_NOT_FOUND = -717;
        public const int ERROR_MAIL_DOMAIN_PACKAGE_ITEM_NOT_FOUND = -718;
        public const int ERROR_MAIL_DOMAIN_SERVICE_ITEM_NOT_FOUND = -719;
        public const int ERROR_MAIL_DOMAIN_IS_NOT_EMPTY = -720;

        public const int ERROR_MAIL_GROUPS_RECIPIENTS_LIMIT = -721;
        public const int ERROR_MAIL_LISTS_RECIPIENTS_LIMIT = -722;
        public const int ERROR_MAIL_LICENSE_DOMAIN_QUOTA = -723;
        public const int ERROR_MAIL_LICENSE_USERS_QUOTA = -724;

		public const int ERROR_MAIL_ACCOUNT_MAX_MAILBOX_SIZE_LIMIT = -725;
		public const int ERROR_MAIL_ACCOUNT_PASSWORD_NOT_COMPLEXITY = -726;
		#endregion

		#region FTP
		public const int ERROR_FTP_RESOURCE_UNAVAILABLE = -801;
        public const int ERROR_FTP_RESOURCE_QUOTA_LIMIT = -802;
        public const int ERROR_FTP_PACKAGE_ITEM_EXISTS = -803;
        public const int ERROR_FTP_SERVICE_ITEM_EXISTS = -804;
        public const int ERROR_FTP_PACKAGE_ITEM_NOT_FOUND = -805;
        public const int ERROR_FTP_USERNAME_LENGTH = -806;
        #endregion

        #region MS SQL
        public const int ERROR_MSSQL_RESOURCE_UNAVAILABLE = -901;
        public const int ERROR_MSSQL_DATABASES_RESOURCE_QUOTA_LIMIT = -902;
        public const int ERROR_MSSQL_DATABASES_PACKAGE_ITEM_EXISTS = -903;
        public const int ERROR_MSSQL_DATABASES_SERVICE_ITEM_EXISTS = -904;

        public const int ERROR_MSSQL_USERS_RESOURCE_QUOTA_LIMIT = -905;
        public const int ERROR_MSSQL_USERS_PACKAGE_ITEM_EXISTS = -906;
        public const int ERROR_MSSQL_USERS_SERVICE_ITEM_EXISTS = -907;

        public const int ERROR_MSSQL_DATABASES_PACKAGE_ITEM_NOT_FOUND = -908;
        public const int ERROR_MSSQL_USERS_PACKAGE_ITEM_NOT_FOUND = -909;
        #endregion

        #region MySQL
        public const int ERROR_MYSQL_RESOURCE_UNAVAILABLE = -1001;
        public const int ERROR_MYSQL_DATABASES_RESOURCE_QUOTA_LIMIT = -1002;
        public const int ERROR_MYSQL_DATABASES_PACKAGE_ITEM_EXISTS = -1003;
        public const int ERROR_MYSQL_DATABASES_SERVICE_ITEM_EXISTS = -1004;

        public const int ERROR_MYSQL_USERS_RESOURCE_QUOTA_LIMIT = -1005;
        public const int ERROR_MYSQL_USERS_PACKAGE_ITEM_EXISTS = -1006;
        public const int ERROR_MYSQL_USERS_SERVICE_ITEM_EXISTS = -1007;

        public const int ERROR_MYSQL_DATABASES_PACKAGE_ITEM_NOT_FOUND = -1008;
        public const int ERROR_MYSQL_USERS_PACKAGE_ITEM_NOT_FOUND = -1009;
        public const int ERROR_MYSQL_INVALID_USER_NAME = -1010;
        public const int ERROR_MYSQL_INVALID_DATABASE_NAME = -1011;
        #endregion

        #region MariaDB
        public const int ERROR_MARIADB_RESOURCE_UNAVAILABLE = -1001;
        public const int ERROR_MARIADB_DATABASES_RESOURCE_QUOTA_LIMIT = -1002;
        public const int ERROR_MARIADB_DATABASES_PACKAGE_ITEM_EXISTS = -1003;
        public const int ERROR_MARIADB_DATABASES_SERVICE_ITEM_EXISTS = -1004;

        public const int ERROR_MARIADB_USERS_RESOURCE_QUOTA_LIMIT = -1005;
        public const int ERROR_MARIADB_USERS_PACKAGE_ITEM_EXISTS = -1006;
        public const int ERROR_MARIADB_USERS_SERVICE_ITEM_EXISTS = -1007;

        public const int ERROR_MARIADB_DATABASES_PACKAGE_ITEM_NOT_FOUND = -1008;
        public const int ERROR_MARIADB_USERS_PACKAGE_ITEM_NOT_FOUND = -1009;
        public const int ERROR_MARIADB_INVALID_USER_NAME = -1010;
        public const int ERROR_MARIADB_INVALID_DATABASE_NAME = -1011;
        #endregion

        #region Statistics
        public const int ERROR_STATS_RESOURCE_UNAVAILABLE = -1301;
        public const int ERROR_STATS_RESOURCE_QUOTA_LIMIT = -1302;
        public const int ERROR_STATS_PACKAGE_ITEM_EXISTS = -1303;
        public const int ERROR_STATS_SERVICE_ITEM_EXISTS = -1304;

        public const int ERROR_STATS_PACKAGE_ITEM_NOT_FOUND = -1305;
        #endregion

        #region Account Creation Wizard
        public const int ERROR_ACCOUNT_WIZARD_USER_EXISTS = -1100;
        public const int ERROR_ACCOUNT_WIZARD_FTP_ACCOUNT_EXISTS = -1101;
        #endregion

        #region Web Apps Installer
        public const int ERROR_WEB_INSTALLER_WEBSITE_NOT_EXISTS = -1201;
        public const int ERROR_WEB_INSTALLER_CANT_CONNECT_DATABASE = -1202;

        public const int ERROR_WEB_INSTALLER_TARGET_WEBSITE_UNSUITABLE = -1203;
        public const int ERROR_WEB_INSTALLER_TARGET_DATABASE_UNSUITABLE = -1204;
        #endregion

        #region Hosting Plans
        public const int ERROR_HOSTING_PLAN_USED_IN_PACKAGE = -1501;
        public const int ERROR_HOSTING_ADDON_USED_IN_PACKAGE = -1502;
        #endregion

        #region IP Addresses
        public const int ERROR_IP_USED_IN_NAME_SERVER = -1601;
        public const int ERROR_IP_USED_BY_PACKAGE_ITEM = -1602;
        #endregion

        #region Servers
        public const int ERROR_SERVER_CONTAINS_SERVICES = -1701;
        public const int ERROR_SERVER_CONTAINS_PACKAGES = -1702;
        public const int ERROR_SERVER_USED_IN_HOSTING_PLANS = -1703;
        #endregion

        #region Services
        public const int ERROR_SERVICE_CONTAINS_SERVICE_ITEMS = -1801;
        public const int ERROR_SERVICE_USED_IN_VIRTUAL_SERVER = -1802;
        #endregion

        #region SMTP
        public const int SMTP_BAD_COMMAND_SEQUENCE = -1901;
        public const int SMTP_CANNOT_VERIFY_USER_WILL_ATTEMPT_DELIVERY = -1902;
        public const int SMTP_CLIENT_NOT_PERMITTED = -1903;
        public const int SMTP_COMMAND_NOT_IMPLEMENTED = -1904;
        public const int SMTP_COMMAND_PARAMETER_NOT_IMPLEMENTED = -1905;
        public const int SMTP_COMMAND_UNRECOGNIZED = -1906;
        public const int SMTP_EXCEEDED_STORAGE_ALLOCATION = -1907;
        public const int SMTP_GENERAL_FAILURE = -1908;
        public const int SMTP_INSUFFICIENT_STORAGE = -1909;
        public const int SMTP_LOCAL_ERROR_IN_PROCESSING = -1910;
        public const int SMTP_MAILBOX_BUSY = -1911;
        public const int SMTP_MAILBOX_NAME_NOTALLOWED = -1912;
        public const int SMTP_MAILBOX_UNAVAILABLE = -1913;
        public const int SMTP_MUST_ISSUE_START_TLS_FIRST = -1914;
        public const int SMTP_SERVICE_CLOSING_TRANSMISSION_CHANNEL = -1915;
        public const int SMTP_SERVICE_NOT_AVAILABLE = -1916;
        public const int SMTP_SYNTAX_ERROR = -1917;
        public const int SMTP_TRANSACTION_FAILED = -1918;
        public const int SMTP_USER_NOT_LOCAL_TRY_ALTERNATE_PATH = -1919;
        public const int SMTP_USER_NOT_LOCAL_WILL_FORWARD = -1920;
        public const int SMTP_UNKNOWN_ERROR = -1921;
        #endregion

        #region SharePoint
        public const int ERROR_SHAREPOINT_USERS_RESOURCE_QUOTA_LIMIT = -1402;
        public const int ERROR_SHAREPOINT_USERS_PACKAGE_ITEM_EXISTS = -1403;
        public const int ERROR_SHAREPOINT_USERS_SERVICE_ITEM_EXISTS = -1404;

        public const int ERROR_SHAREPOINT_GROUPS_RESOURCE_QUOTA_LIMIT = -1405;
        public const int ERROR_SHAREPOINT_GROUPS_PACKAGE_ITEM_EXISTS = -1406;
        public const int ERROR_SHAREPOINT_GROUPS_SERVICE_ITEM_EXISTS = -1407;

        public const int ERROR_SHAREPOINT_USERS_PACKAGE_ITEM_NOT_FOUND = -1408;
        public const int ERROR_SHAREPOINT_GROUPS_PACKAGE_ITEM_NOT_FOUND = -1409;
        public const int ERROR_SHAREPOINT_RESOURCE_QUOTA_LIMIT = -1410;

        public const int ERROR_SHAREPOINT_RESOURCE_UNAVAILABLE = -2001;
       
        public const int ERROR_SHAREPOINT_PACKAGE_ITEM_EXISTS = -2003;

        public const int ERROR_SHAREPOINT_PACKAGE_ITEM_NOT_FOUND = -2004;
        #endregion

        #region Operating System
        public const int ERROR_OS_RESOURCE_UNAVAILABLE = -2101;
        public const int ERROR_OS_DSN_RESOURCE_QUOTA_LIMIT = -2102;
        public const int ERROR_OS_DSN_PACKAGE_ITEM_EXISTS = -2103;
        public const int ERROR_OS_DSN_SERVICE_ITEM_EXISTS = -2104;
        public const int ERROR_OS_DSN_PACKAGE_ITEM_NOT_FOUND = -2105;
        #endregion

        #region Scheduled Tasks
        public const int ERROR_OS_SCHEDULED_TASK_QUOTA_LIMIT = -2200;
        #endregion

		#region Backup/Restore
		public const int ERROR_BACKUP_TEMP_FOLDER_UNAVAILABLE = -2400;
		public const int ERROR_BACKUP_DEST_FOLDER_UNAVAILABLE = -2401;
        public const int ERROR_BACKUP_SERVER_FOLDER_UNAVAILABLE = -2402;

		public const int ERROR_RESTORE_INVALID_BACKUP_SET = -2501;
		public const int ERROR_RESTORE_BACKUP_SOURCE_UNAVAILABLE = -2502;
		public const int ERROR_RESTORE_BACKUP_SOURCE_NOT_FOUND = -2503;
		#endregion

		#region Exchange Server
		
		public const int ERROR_EXCHANGE_EMAIL_EXISTS = -2602;						
		public const int ERROR_EXCHANGE_DELETE_SOME_PROBLEMS = -2606;
		public const int ERROR_EXCHANGE_MAILBOXES_QUOTA_LIMIT = -2607;
		public const int ERROR_EXCHANGE_CONTACTS_QUOTA_LIMIT = -2608;
		public const int ERROR_EXCHANGE_DLISTS_QUOTA_LIMIT = -2609;
		public const int ERROR_EXCHANGE_PFOLDERS_QUOTA_LIMIT = -2610;
		public const int ERROR_EXCHANGE_DOMAINS_QUOTA_LIMIT = -2611;
        public const int ERROR_EXCHANGE_STORAGE_QUOTAS_EXCEED_HOST_VALUES = -2612;
        public const int ERROR_EXCHANGE_INVALID_RECOVERABLEITEMS_QUOTA = -2613;
        public const int ERROR_EXCHANGE_JOURNALING_MAILBOXES_QUOTA_LIMIT = -2614;
        #endregion

        #region Organizations

        public const int ERROR_ORG_ID_EXISTS = -2701;
        public const int ERROR_ORG_EMAIL_EXISTS = -2702;        
        public const int ERROR_ORGS_RESOURCE_QUOTA_LIMIT = -2703;
        public const int ERROR_ORGANIZATION_TEMP_DOMAIN_IS_NOT_SPECIFIED = -2704;
        public const int ERROR_ORGANIZATION_DOMAIN_IS_IN_USE = -2705;
        public const int ERROR_ORGANIZATION_DELETE_SOME_PROBLEMS = -2706;
        public const int ERROR_USERS_RESOURCE_QUOTA_LIMIT = -2707;
        public const int CURRENT_USER_IS_CRM_USER = -2708;
        public const int CURRENT_USER_IS_OCS_USER = -2709;
        public const int CURRENT_USER_IS_LYNC_USER = -2710;
        public const int CURRENT_USER_IS_SFB_USER = -2710;
        public const int ERROR_DELETED_USERS_RESOURCE_QUOTA_LIMIT = -2711;

        #endregion

		#region Generic Error Codes

		public const int FAILED_EXECUTE_SERVICE_OPERATION = -999;
		
		#endregion

        #region File Error Codes

        public const int ERROR_FILE_GENERIC_LOGGED = -3333;
        public const int ERROR_FILE_COPY_TO_SELF = -3000;
        public const int ERROR_FILE_COPY_TO_OWN_SUBFOLDER = -3001;
        public const int ERROR_FILE_DEST_FOLDER_NONEXISTENT = -3002;
        public const int ERROR_FILE_CREATE_FILE_WITH_DIR_NAME = -3003;
        public const int ERROR_FILE_MOVE_PATH_ALREADY_EXISTS = -3004;
        #endregion

        #region Lync Server
        public const int ERROR_LYNC_DELETE_SOME_PROBLEMS = -2806;
        #endregion

        #region SfB Server
        public const int ERROR_SFB_DELETE_SOME_PROBLEMS = -2806;
        #endregion

        public static string ToText(int code)
        {
            switch (code)
            {
                // SecurityContext
                case BusinessErrorCodes.ERROR_USER_ACCOUNT_DEMO: return "ERROR_USER_ACCOUNT_DEMO";
                case BusinessErrorCodes.ERROR_USER_ACCOUNT_PENDING: return "ERROR_USER_ACCOUNT_PENDING";
                case BusinessErrorCodes.ERROR_USER_ACCOUNT_SUSPENDED: return "ERROR_USER_ACCOUNT_SUSPENDED";
                case BusinessErrorCodes.ERROR_USER_ACCOUNT_CANCELLED: return "ERROR_USER_ACCOUNT_CANCELLED";
                case BusinessErrorCodes.ERROR_USER_ACCOUNT_SHOULD_BE_ADMINISTRATOR: return "ERROR_USER_ACCOUNT_SHOULD_BE_ADMINISTRATOR";
                case BusinessErrorCodes.ERROR_USER_ACCOUNT_SHOULD_BE_RESELLER: return "ERROR_USER_ACCOUNT_SHOULD_BE_RESELLER";
                case BusinessErrorCodes.ERROR_PACKAGE_NOT_FOUND: return "ERROR_PACKAGE_NOT_FOUND";
                case BusinessErrorCodes.ERROR_PACKAGE_CANCELLED: return "ERROR_PACKAGE_CANCELLED";
                case BusinessErrorCodes.ERROR_PACKAGE_SUSPENDED: return "ERROR_PACKAGE_SUSPENDED";
                default: return "";
            }
        }
	}
}
