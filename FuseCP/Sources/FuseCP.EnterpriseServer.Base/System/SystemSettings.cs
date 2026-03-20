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
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections;

namespace FuseCP.EnterpriseServer
{
	[DataContract]
	public class SystemSettings
	{

		public const string SMTP_SETTINGS = "SmtpSettings";
		public const string BACKUP_SETTINGS = "BackupSettings";
		public const string SETUP_SETTINGS = "SetupSettings";
        public const string FILEMANAGER_SETTINGS = "FileManagerSettings";
        public const string PACKAGE_DISPLAY_SETTINGS = "PackageDisplaySettings";
        public const string RDS_SETTINGS = "RdsSettings";
        public const string WEBDAV_PORTAL_SETTINGS = "WebdavPortalSettings";
        public const string TWILIO_SETTINGS = "TwilioSettings";
        public const string ACCESS_IP_SETTINGS = "AccessIpsSettings";
		public const string AUTHENTICATION_SETTINGS = "AuthenticationSettings";
		public const string DEBUG_SETTINGS = "DebugSettings";

		//Keys
		public const string TWILIO_ACTIVE_KEY = "TwilioActive";
        public const string TWILIO_ACCOUNTSID_KEY = "TwilioAccountSid";
        public const string TWILIO_AUTHTOKEN_KEY = "TwilioAuthToken";
        public const string TWILIO_PHONEFROM_KEY = "TwilioPhoneFrom";

        public const string WEBDAV_PASSWORD_RESET_ENABLED_KEY = "WebdavPswResetEnabled";
        public const string WEBDAV_PASSWORD_RESET_LINK_LIFE_SPAN = "WebdavPswdResetLinkLifeSpan";
        public const string WEBDAV_OWA_ENABLED_KEY = "WebdavOwaEnabled";
        public const string WEBDAV_OWA_URL = "WebdavOwaUrl";

		//Mfa token app display name
		public const string MFA_TOKEN_APP_DISPLAY_NAME = "MfaTokenAppDisplayName";
		public const string MFA_CAN_PEER_CHANGE_MFA = "CanPeerChangeMfa";
		public const string AUTH_BRUTEFORCE_MAX_FAILED_ATTEMPTS = "BruteForceMaxFailedAttempts";
		public const string AUTH_BRUTEFORCE_WINDOW_MINUTES = "BruteForceWindowMinutes";
		public const string AUTH_BRUTEFORCE_LOCKOUT_MINUTES = "BruteForceLockoutMinutes";
		public const string AUTH_BRUTEFORCE_CRITICAL_ATTEMPTS = "BruteForceCriticalAttemptThreshold";

		// Constant for IPAccess
		public const string ACCESS_IPs = "AccessIps";

        // Constants for Reporting Transforms
        public const string BANDWIDTH_TRANSFORM = "BandwidthXLST";
        public const string DISKSPACE_TRANSFORM = "DiskspaceXLST";

		// Always use EntityFramework
		public const string ALWAYS_USE_ENTITYFRAMEWORK = "AlwaysUseEntityFramework";

        public static readonly SystemSettings Empty = new SystemSettings { SettingsArray = new string[][] {} };

		NameValueCollection settingsHash = null;

		[DataMember]
		public string[][] SettingsArray;

		NameValueCollection Settings
		{
			get
			{
				if (settingsHash == null)
				{
					// create new dictionary
					settingsHash = new NameValueCollection();

					// fill dictionary
					if (SettingsArray != null)
					{
						foreach (string[] pair in SettingsArray)
							settingsHash.Add(pair[0], pair[1]);
					}
				}
				return settingsHash;
			}
		}


		public string this[string settingName]
		{
			get
			{
				return Settings[settingName];
			}
			set
			{
				// set setting
				Settings[settingName] = value;

				// rebuild array
				SettingsArray = new string[Settings.Count][];
				for (int i = 0; i < Settings.Count; i++)
				{
					SettingsArray[i] = new string[] { Settings.Keys[i], Settings[Settings.Keys[i]] };
				}
			}
		}

	    public bool Contains(string settingName)
	    {
	        return Settings.AllKeys.Any(x => x.ToLowerInvariant() == (settingName ?? string.Empty).ToLowerInvariant());
	    }

	    public T GetValueOrDefault<T>(string settingName, T defaultValue)
	    {
	        try
	        {
                return (T)Convert.ChangeType(Settings[settingName], typeof(T));
	        }
	        catch
	        {
	        }

	        return defaultValue;
	    }

	    public int GetInt(string settingName)
		{
			return Int32.Parse(Settings[settingName]);
		}
	}
}
