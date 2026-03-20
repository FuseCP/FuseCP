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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using FuseCP.EnterpriseServer.Base.Common;
using FCP = FuseCP.EnterpriseServer;
using System.Text.RegularExpressions;

namespace FuseCP.Portal
{
    public partial class SystemSettings : FuseCPModuleBase
    {
        public const string SMTP_SERVER = "SmtpServer";
        public const string SMTP_PORT = "SmtpPort";
        public const string SMTP_USERNAME = "SmtpUsername";
        public const string SMTP_PASSWORD = "SmtpPassword";
        public const string SMTP_ENABLE_SSL = "SmtpEnableSsl";
        public const string SMTP_ENABLE_LEGACYSSL = "SmtpEnableLegacySSL";
        public const string BACKUPS_PATH = "BackupsPath";
        public const string FILE_MANAGER_EDITABLE_EXTENSIONS = "EditableExtensions";
        public const string RDS_MAIN_CONTROLLER = "RdsMainController";
        public const string WEBDAV_PORTAL_URL = "WebdavPortalUrl";
        public const string WEBDAV_PASSWORD_RESET_ENABLED = "WebdavPasswordResetEnabled";

        // new for XSLT reporting transforms 
        public const string BANDWIDTH_TRANSFORM = "BandwidthXLST";
        public const string DISKSPACE_TRANSFORM = "DiskspaceXLST";

        /*
        public const string FEED_ENABLE_MICROSOFT = "FeedEnableMicrosoft";
        public const string FEED_ENABLE_HELICON = "FeedEnableHelicon";
        */

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    LoadSettings();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("SYSTEM_SETTINGS_LOAD", ex);
                }

				bool showDebugSettings = string.Equals(
                    WebPortal.PortalConfiguration.SiteSettings["ShowDebugSettings"],
                    "true",
                    StringComparison.OrdinalIgnoreCase);
                DebugSettingsPanel.Visible = showDebugSettings;
			}
		}
        private void LoadSettings()
        {
            // SMTP
            FCP.SystemSettings settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.SMTP_SETTINGS);

            if (settings != null)
            {
                txtSmtpServer.Text = settings[SMTP_SERVER];
                txtSmtpPort.Text = settings[SMTP_PORT];
                txtSmtpUser.Text = settings[SMTP_USERNAME];
                txtSmtpPassword.Text = settings[SMTP_PASSWORD];
                chkEnableSsl.Checked = Utils.ParseBool(settings[SMTP_ENABLE_SSL], false);
                chkEnableLegacySSL.Checked = Utils.ParseBool(settings[SMTP_ENABLE_LEGACYSSL], false);
            }

            // BACKUP
            settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.BACKUP_SETTINGS);

            if (settings != null)
            {
                txtBackupsPath.Text = settings["BackupsPath"];
            }


            // FILE MANAGER
            settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.FILEMANAGER_SETTINGS);

            if (settings != null && !String.IsNullOrEmpty(settings[FILE_MANAGER_EDITABLE_EXTENSIONS]))
            {
                txtFileManagerEditableExtensions.Text = settings[FILE_MANAGER_EDITABLE_EXTENSIONS].Replace(",", System.Environment.NewLine);
            }
            else
            {
                // Original FuseCP Extensions
                txtFileManagerEditableExtensions.Text = FileManager.ALLOWED_EDIT_EXTENSIONS.Replace(",", System.Environment.NewLine);
            }

            // RDS
            var services = ES.Services.RDS.GetRdsServices();

            foreach (var service in services)
            {
                ddlRdsController.Items.Add(new ListItem(service.ServiceName, service.ServiceId.ToString()));
            }

            settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.RDS_SETTINGS);

            if (settings != null && !string.IsNullOrEmpty(settings[RDS_MAIN_CONTROLLER]))
            {
                ddlRdsController.SelectedValue = settings[RDS_MAIN_CONTROLLER];
            }
            else if (ddlRdsController.Items.Count > 0)
            {
                ddlRdsController.SelectedValue = ddlRdsController.Items[0].Value;
            }

            // Webdav portal
            settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.WEBDAV_PORTAL_SETTINGS);

            if (settings != null)
            {
                txtWebdavPortalUrl.Text = settings[WEBDAV_PORTAL_URL];

                chkEnableOwa.Checked = Utils.ParseBool(settings[FCP.SystemSettings.WEBDAV_OWA_ENABLED_KEY], false);
                txtOwaUrl.Text = settings[FCP.SystemSettings.WEBDAV_OWA_URL];
            }

            // Twilio portal
            settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.TWILIO_SETTINGS);

            if (settings != null)
            {
                txtAccountSid.Text = settings.GetValueOrDefault(FCP.SystemSettings.TWILIO_ACCOUNTSID_KEY, string.Empty);
                txtAuthToken.Text = settings.GetValueOrDefault(FCP.SystemSettings.TWILIO_AUTHTOKEN_KEY, string.Empty);
                txtPhoneFrom.Text = settings.GetValueOrDefault(FCP.SystemSettings.TWILIO_PHONEFROM_KEY, string.Empty);
            }

            // Access IP Settings
            settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.ACCESS_IP_SETTINGS);

            if (settings != null)
            {
                txtIPAddress.Text = settings.GetValueOrDefault(FCP.SystemSettings.ACCESS_IPs, string.Empty);
            }

            var isSqlServer = DbHelper.DbType == EnterpriseServer.Data.DbType.SqlServer;
            chkAlwaysUseEntityFramework.Enabled = isSqlServer;
            btnDebugSettings.Enabled = isSqlServer;
			if (!isSqlServer) chkAlwaysUseEntityFramework.Checked = true;
            else
            {
                // Debug settings
                /*settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.DEBUG_SETTINGS);

                if (settings != null)
                {
                    chkAlwaysUseEntityFramework.Checked = settings
                        .GetValueOrDefault(FCP.SystemSettings.ALWAYS_USE_ENTITYFRAMEWORK, false);
                }*/
                chkAlwaysUseEntityFramework.Checked = DbHelper.UseEntityFramework;
            }
		}
		private void SaveSMTP()
        {
            try
            {
                FCP.SystemSettings settings = new FCP.SystemSettings();

                // SMTP
                settings[SMTP_SERVER] = txtSmtpServer.Text.Trim();
                settings[SMTP_PORT] = txtSmtpPort.Text.Trim();
                settings[SMTP_USERNAME] = txtSmtpUser.Text.Trim();
                settings[SMTP_PASSWORD] = txtSmtpPassword.Text;
                settings[SMTP_ENABLE_SSL] = chkEnableSsl.Checked.ToString();
                settings[SMTP_ENABLE_LEGACYSSL] = chkEnableLegacySSL.Checked.ToString();

                // SMTP
                int result = ES.Services.System.SetSystemSettings(FCP.SystemSettings.SMTP_SETTINGS, settings);

                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
                return;
            }

            ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
        }
        private void SaveBACKUP()
        {
            try
            {
                FCP.SystemSettings settings = new FCP.SystemSettings();

                // BACKUP
                settings = new FCP.SystemSettings();
                settings[BACKUPS_PATH] = txtBackupsPath.Text.Trim();

                int result = ES.Services.System.SetSystemSettings(
                    FCP.SystemSettings.BACKUP_SETTINGS, settings);

                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
                return;
            }

            ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
        }
        private void SaveFILEMANAGER()
        {
            try
            {
                FCP.SystemSettings settings = new FCP.SystemSettings();
                // FILE MANAGER
                settings = new FCP.SystemSettings();
                settings[FILE_MANAGER_EDITABLE_EXTENSIONS] = Regex.Replace(txtFileManagerEditableExtensions.Text, @"[\r\n]+", ",");


                int result = ES.Services.System.SetSystemSettings(
                    FCP.SystemSettings.FILEMANAGER_SETTINGS, settings);

                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
                return;
            }

            ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
        }
        private void SaveRDS()
        {
            try
            {
                FCP.SystemSettings settings = new FCP.SystemSettings();
                // RDS Server
                settings = new FCP.SystemSettings();
                settings[RDS_MAIN_CONTROLLER] = ddlRdsController.SelectedValue;
                int result = ES.Services.System.SetSystemSettings(FCP.SystemSettings.RDS_SETTINGS, settings);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
                return;
            }

            ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
        }
        private void SaveOWA()
        {
            try
            {
                FCP.SystemSettings settings = new FCP.SystemSettings();
                // OWA Portal
                settings = new FCP.SystemSettings();

                settings[FCP.SystemSettings.WEBDAV_OWA_ENABLED_KEY] = chkEnableOwa.Checked.ToString();
                settings[FCP.SystemSettings.WEBDAV_OWA_URL] = txtOwaUrl.Text;

                int result = ES.Services.System.SetSystemSettings(FCP.SystemSettings.WEBDAV_PORTAL_SETTINGS, settings);

                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
                return;
            }

            ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
        }
        private void SaveCLOUD()
        {
            try
            {
                // Cloud Portal
                FCP.SystemSettings settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.WEBDAV_PORTAL_SETTINGS) ?? new FCP.SystemSettings();
                settings[WEBDAV_PORTAL_URL] = txtWebdavPortalUrl.Text;
                settings[FCP.SystemSettings.WEBDAV_PASSWORD_RESET_ENABLED_KEY] = bool.TrueString;

                int result = ES.Services.System.SetSystemSettings(FCP.SystemSettings.WEBDAV_PORTAL_SETTINGS, settings);

                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
                return;
            }

            ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
        }
        private void SaveTWILIO()
        {
            try
            {
                FCP.SystemSettings settings = new FCP.SystemSettings();

                // Twilio portal
                settings = new FCP.SystemSettings();
                settings[FCP.SystemSettings.TWILIO_ACCOUNTSID_KEY] = txtAccountSid.Text;
                settings[FCP.SystemSettings.TWILIO_AUTHTOKEN_KEY] = txtAuthToken.Text;
                settings[FCP.SystemSettings.TWILIO_PHONEFROM_KEY] = txtPhoneFrom.Text;
                int result = ES.Services.System.SetSystemSettings(FCP.SystemSettings.TWILIO_SETTINGS, settings);

                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
                return;
            }

            ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
        }
        private void DisableTWILIO()
        {
            txtAccountSid.Text = string.Empty;
            txtAuthToken.Text = string.Empty;
            txtPhoneFrom.Text = string.Empty;

            SaveTWILIO();
        }
        private void SaveRESTRICT()
        {
            try
            {
                FCP.SystemSettings settings = new FCP.SystemSettings();

                //AccessIPs
                settings = new FCP.SystemSettings();
                settings[FCP.SystemSettings.ACCESS_IPs] = txtIPAddress.Text;

                int result = ES.Services.System.SetSystemSettings(FCP.SystemSettings.ACCESS_IP_SETTINGS, settings);

                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
                return;
            }

            ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
        }

        private void SaveDebug()
        {
            var isSqlServer = ES.Services.System.GetDatabaseType() == EnterpriseServer.Data.DbType.SqlServer;
            if (isSqlServer)
            {
                try
                {
                    /*
                    FCP.SystemSettings settings = new FCP.SystemSettings();

                    // authentication settings
                    settings = new FCP.SystemSettings();
                    settings[FCP.SystemSettings.ALWAYS_USE_ENTITYFRAMEWORK] = chkAlwaysUseEntityFramework.Checked ? "True" : "False";

                    int result = ES.Services.System.SetSystemSettings(FCP.SystemSettings.DEBUG_SETTINGS, settings);
                    */
                    int result = DbHelper.SetUseEntityFramework(chkAlwaysUseEntityFramework.Checked);
					if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
                    return;
                }

                ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
            }
        }
		#region Button Calls
		protected void btnSaveSMTP_Click(object sender, EventArgs e)
        {
            SaveSMTP();
        }
        protected void btnSaveBACKUP_Click(object sender, EventArgs e)
        {
            SaveBACKUP();
        }
        protected void btnSaveFILEMANAGER_Click(object sender, EventArgs e)
        {
            SaveFILEMANAGER();
        }
        protected void btnSaveRDS_Click(object sender, EventArgs e)
        {
            SaveRDS();
        }
        protected void btnSaveOWA_Click(object sender, EventArgs e)
        {
            SaveOWA();
        }
        protected void btnSaveCLOUD_Click(object sender, EventArgs e)
        {
            SaveCLOUD();
        }
        protected void btnSaveTWILIO_Click(object sender, EventArgs e)
        {
            SaveTWILIO();
        }
        protected void btnDisableTWILIO_Click(object sender, EventArgs e)
        {
            DisableTWILIO();
        }
        protected void btnSaveRESTRICT_Click(object sender, EventArgs e)
        {
            SaveRESTRICT();
        }

		protected void btnDebugSettings_Click(object sender, EventArgs e)
		{
			SaveDebug();
		}
		#endregion
	}
}
