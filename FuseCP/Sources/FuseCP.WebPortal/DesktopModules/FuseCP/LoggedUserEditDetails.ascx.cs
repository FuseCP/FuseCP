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
using FuseCP.EnterpriseServer;
using System.Web.Security;
using System.Data;
using System.Web;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using FuseCP.WebPortal;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FuseCP.Portal
{
    public partial class LoggedUserEditDetails : FuseCPModuleBase
    {
        const int redirectTimeout = Utils.CHANGE_PASSWORD_REDIRECT_TIMEOUT;
        const string changePasswordWarningKey = "LoggedUserEditDetails.ChangePasswordWarning";
        private const string ThemePaletteLightCookie = "UserThemePaletteLight";
        private const string ThemePaletteDarkCookie = "UserThemePaletteDark";
        private const string ThemeButtonsLightCookie = "UserThemeButtonsLight";
        private const string ThemeButtonsDarkCookie = "UserThemeButtonsDark";
        private const string ThemeModeCookie = "UserThemeMode";
        private const string ThemeStyleLight = "light-theme";
        private const string ThemeStyleDark = "dark-theme";

        private static readonly Regex PaletteHexColorRegex = new Regex("^#(?:[0-9A-Fa-f]{6})$", RegexOptions.Compiled);
        private static readonly Regex CssLengthRegex = new Regex("^(?:0|(?:\\d+(?:\\.\\d+)?)(?:px|rem|em|%))$", RegexOptions.Compiled);

        private static readonly PaletteColorSetting[] LightPaletteDefaults = new[]
        {
            new PaletteColorSetting("--fcp-bg", "Page Background", "#F4F6F9"),
            new PaletteColorSetting("--fcp-topbar-bg", "Top Bar Background", "#FFFFFF"),
            new PaletteColorSetting("--fcp-topbar-bdr", "Top Bar Border", "#E8EAF0"),
            new PaletteColorSetting("--fcp-sidebar-bg", "Sidebar Background", "#2D3E50"),
            new PaletteColorSetting("--fcp-surface", "Surface", "#FFFFFF"),
            new PaletteColorSetting("--fcp-surface-alt", "Surface Alt", "#F5F7FA"),
            new PaletteColorSetting("--fcp-surface-h", "Surface Hover", "#EDF3FB"),
            new PaletteColorSetting("--fcp-border", "Border", "#E0E0E0"),
            new PaletteColorSetting("--fcp-input-bdr", "Input Border", "#CCCCCC"),
            new PaletteColorSetting("--fcp-text", "Text", "#333333"),
            new PaletteColorSetting("--fcp-text-hi", "Text High", "#1A1A1A"),
            new PaletteColorSetting("--fcp-accent", "Accent", "#0A8FD8")
        };

        private static readonly PaletteColorSetting[] DarkPaletteDefaults = new[]
        {
            new PaletteColorSetting("--fcp-dark-bg", "Page Background", "#1A1A2E"),
            new PaletteColorSetting("--fcp-dark-topbar-bg", "Top Bar Background", "#141414"),
            new PaletteColorSetting("--fcp-dark-topbar-bdr", "Top Bar Border", "#2A2A2A"),
            new PaletteColorSetting("--fcp-dark-sidebar-bg", "Sidebar Background", "#1D1F22"),
            new PaletteColorSetting("--fcp-dark-surface", "Surface", "#202225"),
            new PaletteColorSetting("--fcp-dark-surface-alt", "Surface Alt", "#292C31"),
            new PaletteColorSetting("--fcp-dark-surface-h", "Surface Hover", "#31353B"),
            new PaletteColorSetting("--fcp-dark-border", "Border", "#3A3E45"),
            new PaletteColorSetting("--fcp-dark-input-bdr", "Input Border", "#4A4F57"),
            new PaletteColorSetting("--fcp-dark-text", "Text", "#D0D5DC"),
            new PaletteColorSetting("--fcp-dark-text-hi", "Text High", "#F3F5F7"),
            new PaletteColorSetting("--fcp-dark-accent", "Accent", "#0A8FD8")
        };

        private static readonly ButtonStyleSetting[] LightButtonDefaults = new[]
        {
            new ButtonStyleSetting("--fcp-btn-primary-text", "Primary Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-btn-primary-bg", "Primary Background", "#0A8FD8", true),
            new ButtonStyleSetting("--fcp-btn-primary-bdr", "Primary Border", "#0A8FD8", true),
            new ButtonStyleSetting("--fcp-btn-primary-h-text", "Primary Hover Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-btn-primary-h-bg", "Primary Hover Background", "#0877B5", true),
            new ButtonStyleSetting("--fcp-btn-primary-h-bdr", "Primary Hover Border", "#0877B5", true),

            new ButtonStyleSetting("--fcp-btn-success-text", "Success Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-btn-success-bg", "Success Background", "#28A745", true),
            new ButtonStyleSetting("--fcp-btn-success-bdr", "Success Border", "#28A745", true),
            new ButtonStyleSetting("--fcp-btn-success-h-text", "Success Hover Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-btn-success-h-bg", "Success Hover Background", "#218838", true),
            new ButtonStyleSetting("--fcp-btn-success-h-bdr", "Success Hover Border", "#218838", true),

            new ButtonStyleSetting("--fcp-btn-danger-text", "Danger Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-btn-danger-bg", "Danger Background", "#DC3545", true),
            new ButtonStyleSetting("--fcp-btn-danger-bdr", "Danger Border", "#DC3545", true),
            new ButtonStyleSetting("--fcp-btn-danger-h-text", "Danger Hover Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-btn-danger-h-bg", "Danger Hover Background", "#C82333", true),
            new ButtonStyleSetting("--fcp-btn-danger-h-bdr", "Danger Hover Border", "#C82333", true),

            new ButtonStyleSetting("--fcp-btn-warning-text", "Warning Text", "#1A1A1A", true),
            new ButtonStyleSetting("--fcp-btn-warning-bg", "Warning Background", "#FFC107", true),
            new ButtonStyleSetting("--fcp-btn-warning-bdr", "Warning Border", "#FFC107", true),
            new ButtonStyleSetting("--fcp-btn-warning-h-text", "Warning Hover Text", "#1A1A1A", true),
            new ButtonStyleSetting("--fcp-btn-warning-h-bg", "Warning Hover Background", "#E0A800", true),
            new ButtonStyleSetting("--fcp-btn-warning-h-bdr", "Warning Hover Border", "#E0A800", true),

            new ButtonStyleSetting("--fcp-btn-info-text", "Info Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-btn-info-bg", "Info Background", "#17A2B8", true),
            new ButtonStyleSetting("--fcp-btn-info-bdr", "Info Border", "#17A2B8", true),
            new ButtonStyleSetting("--fcp-btn-info-h-text", "Info Hover Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-btn-info-h-bg", "Info Hover Background", "#138496", true),
            new ButtonStyleSetting("--fcp-btn-info-h-bdr", "Info Hover Border", "#138496", true),

            new ButtonStyleSetting("--fcp-btn-radius", "Button Radius", "6px", false)
        };

        private static readonly ButtonStyleSetting[] DarkButtonDefaults = new[]
        {
            new ButtonStyleSetting("--fcp-dark-btn-primary-text", "Primary Text", "#F3F5F7", true),
            new ButtonStyleSetting("--fcp-dark-btn-primary-bg", "Primary Background", "#0A8FD8", true),
            new ButtonStyleSetting("--fcp-dark-btn-primary-bdr", "Primary Border", "#0A8FD8", true),
            new ButtonStyleSetting("--fcp-dark-btn-primary-h-text", "Primary Hover Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-dark-btn-primary-h-bg", "Primary Hover Background", "#0877B5", true),
            new ButtonStyleSetting("--fcp-dark-btn-primary-h-bdr", "Primary Hover Border", "#0877B5", true),

            new ButtonStyleSetting("--fcp-dark-btn-success-text", "Success Text", "#F3F5F7", true),
            new ButtonStyleSetting("--fcp-dark-btn-success-bg", "Success Background", "#2D9B53", true),
            new ButtonStyleSetting("--fcp-dark-btn-success-bdr", "Success Border", "#2D9B53", true),
            new ButtonStyleSetting("--fcp-dark-btn-success-h-text", "Success Hover Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-dark-btn-success-h-bg", "Success Hover Background", "#278846", true),
            new ButtonStyleSetting("--fcp-dark-btn-success-h-bdr", "Success Hover Border", "#278846", true),

            new ButtonStyleSetting("--fcp-dark-btn-danger-text", "Danger Text", "#F3F5F7", true),
            new ButtonStyleSetting("--fcp-dark-btn-danger-bg", "Danger Background", "#C34755", true),
            new ButtonStyleSetting("--fcp-dark-btn-danger-bdr", "Danger Border", "#C34755", true),
            new ButtonStyleSetting("--fcp-dark-btn-danger-h-text", "Danger Hover Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-dark-btn-danger-h-bg", "Danger Hover Background", "#B33E4A", true),
            new ButtonStyleSetting("--fcp-dark-btn-danger-h-bdr", "Danger Hover Border", "#B33E4A", true),

            new ButtonStyleSetting("--fcp-dark-btn-warning-text", "Warning Text", "#1A1A1A", true),
            new ButtonStyleSetting("--fcp-dark-btn-warning-bg", "Warning Background", "#D9A406", true),
            new ButtonStyleSetting("--fcp-dark-btn-warning-bdr", "Warning Border", "#D9A406", true),
            new ButtonStyleSetting("--fcp-dark-btn-warning-h-text", "Warning Hover Text", "#121212", true),
            new ButtonStyleSetting("--fcp-dark-btn-warning-h-bg", "Warning Hover Background", "#C59305", true),
            new ButtonStyleSetting("--fcp-dark-btn-warning-h-bdr", "Warning Hover Border", "#C59305", true),

            new ButtonStyleSetting("--fcp-dark-btn-info-text", "Info Text", "#F3F5F7", true),
            new ButtonStyleSetting("--fcp-dark-btn-info-bg", "Info Background", "#248D9D", true),
            new ButtonStyleSetting("--fcp-dark-btn-info-bdr", "Info Border", "#248D9D", true),
            new ButtonStyleSetting("--fcp-dark-btn-info-h-text", "Info Hover Text", "#FFFFFF", true),
            new ButtonStyleSetting("--fcp-dark-btn-info-h-bg", "Info Hover Background", "#1F7E8C", true),
            new ButtonStyleSetting("--fcp-dark-btn-info-h-bdr", "Info Hover Border", "#1F7E8C", true),

            new ButtonStyleSetting("--fcp-dark-btn-radius", "Button Radius", "6px", false)
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // bind form
                BindLanguages();
                BindThemes();
                BindUser();
                string changePasswordWarningText = GetSharedLocalizedString(changePasswordWarningKey);
                if (!String.IsNullOrEmpty(changePasswordWarningText)) 
                    lblChangePasswordWarning.Text = changePasswordWarningText;
            }
        }

        private void BindLanguages()
        {
			PortalUtils.LoadCultureDropDownList(ddlLanguage);
        }

        private void BindThemes()
        {
            //PortalUtils.LoadThemesDropDownList(ddlTheme);
            DataSet ThemeData = ES.Services.System.GetThemes();
            ddlTheme.DataSource = ThemeData;
            ddlTheme.DataBind();

            if (ddlTheme.Items.Count > 0)
            {
                Utils.SelectListItem(ddlTheme, PortalUtils.CurrentTheme);
                if (ddlTheme.SelectedIndex < 0)
                {
                    ddlTheme.SelectedIndex = 0;
                }
            }

            BindThemeSettings(ThemeData);
        }

        private void BindThemeSettings(DataSet ThemeData)
        {
            int themeId;
            if (!TryGetSelectedThemeId(ThemeData, out themeId))
            {
                ddlThemeStyle.Items.Clear();
                return;
            }

            DataSet ThemeStyleData = ES.Services.System.GetThemeSetting(themeId, "Style");
            if (ThemeStyleData != null)
            {
                ddlThemeStyle.DataSource = ThemeStyleData;
                ddlThemeStyle.DataBind();
            }

        }

        private void BindUser()
        {
            UserInfo user = ES.Services.Users.GetUserById(PanelSecurity.LoggedUserId);

            if (user != null)
            {
                userPassword.SetUserPolicy(user.UserId, UserSettings.FuseCP_POLICY, "PasswordPolicy");

                // account info
                txtFirstName.Text = PortalAntiXSS.DecodeOld(user.FirstName);
                txtLastName.Text = PortalAntiXSS.DecodeOld(user.LastName);
                txtEmail.Text = user.Email;
                txtSecondaryEmail.Text = user.SecondaryEmail;
                lblUsername.Text = PortalAntiXSS.Encode(user.Username);
                ddlMailFormat.SelectedIndex = user.HtmlMail ? 1 : 0;
                cbxMfaEnabled.Checked = user.MfaMode > 0 ? true : false;
                cbxMfaEnabled.Enabled = ES.Services.Users.CanUserChangeMfa(PanelSecurity.LoggedUserId);
                btnGetQRCodeData.Visible = cbxMfaEnabled.Checked;
                lblMfaEnabled.Visible = cbxMfaEnabled.Checked;

                // contact info
                contact.CompanyName = user.CompanyName;
                contact.Address = user.Address;
                contact.City = user.City;
                contact.Country = user.Country;
                contact.State = user.State;
                contact.Zip = user.Zip;
                contact.PrimaryPhone = user.PrimaryPhone;
                contact.SecondaryPhone = user.SecondaryPhone;
                contact.Fax = user.Fax;
                contact.MessengerId = user.InstantMessenger;

                // bind language
                /*DotNetNuke.Entities.Users.UserInfo dnnUser =
                    DnnUsers.GetUserByName(PortalSettings.PortalId, user.Username, false);

                if (dnnUser != null)
                    Utils.SelectListItem(ddlLanguage, dnnUser.Profile.PreferredLocale);*/

                // bind items per page
                
                txtItemsPerPage.Text = UsersHelper.GetDisplayItemsPerPage().ToString();

                string UserThemeStyle = "";
                DataSet UserThemeSettingsData = ES.Services.Users.GetUserThemeSettings(PanelSecurity.LoggedUserId);
                if (UserThemeSettingsData != null && UserThemeSettingsData.Tables.Count > 0)
                {
                    foreach (DataRow row in UserThemeSettingsData.Tables[0].Rows)
                    {
                        string RowPropertyName = row.Field<String>("PropertyName");
                        string RowPropertyValue = row.Field<String>("PropertyValue");

                        if (RowPropertyName == "Style")
                        {
                            Utils.SelectListItem(ddlThemeStyle, RowPropertyValue);
                            UserThemeStyle = RowPropertyValue;

                        }

                    }
                }

                ApplySelectedThemeMode();

                BindPaletteEditors(UserThemeSettingsData);
                BindButtonStyleEditors(UserThemeSettingsData);

                //TODO: Dynamically load the Theme Settings

            }
        }

        private bool TryGetSelectedThemeId(DataSet themeData, out int themeId)
        {
            themeId = 0;

            if (themeData == null || themeData.Tables.Count == 0 || themeData.Tables[0] == null || themeData.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            int selectedIndex = 0;
            if (ddlTheme != null && ddlTheme.SelectedIndex >= 0 && ddlTheme.SelectedIndex < themeData.Tables[0].Rows.Count)
            {
                selectedIndex = ddlTheme.SelectedIndex;
            }

            object themeIdValue = themeData.Tables[0].Rows[selectedIndex]["ThemeID"];
            if (themeIdValue == null)
            {
                return false;
            }

            return int.TryParse(themeIdValue.ToString(), out themeId);
        }

        private void SaveUser(bool switchUser)
        {
            // get owner
            UserInfo user = ES.Services.Users.GetUserById(PanelSecurity.LoggedUserId);

            if (Page.IsValid)
            {
                // gather data from form
                // account info
                user.FirstName = NormalizeUserText(txtFirstName.Text);
                user.LastName = NormalizeUserText(txtLastName.Text);
                user.Email = NormalizeEmail(txtEmail.Text);
                user.SecondaryEmail = NormalizeEmail(txtSecondaryEmail.Text);
                user.HtmlMail = ddlMailFormat.SelectedIndex == 1;

                // contact info
				user.CompanyName = contact.CompanyName;
                user.Address = contact.Address;
                user.City = contact.City;
                user.Country = contact.Country;
                user.State = contact.State;
                user.Zip = contact.Zip;
                user.PrimaryPhone = contact.PrimaryPhone;
                user.SecondaryPhone = contact.SecondaryPhone;
                user.Fax = contact.Fax;
                user.InstantMessenger = contact.MessengerId;

                // update existing user
                try
                {
                    //int result = UsersHelper.UpdateUser(PortalId, user);
					int result = PortalUtils.UpdateUserAccount(user);

                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }

					// set language
					PortalUtils.SetCurrentLanguage(ddlLanguage.SelectedValue);

                    // set items per page
                    UsersHelper.SetDisplayItemsPerPage(Utils.ParseInt(txtItemsPerPage.Text.Trim(), 10));

                    if (ddlLanguage.SelectedValue != PortalUtils.CurrentUICulture.ToString())
                    {
                        SetCurrentLanguage();
                    }

                    if (ddlTheme.SelectedValue != PortalUtils.CurrentTheme)
                    {
                        SetCurrentTheme();
                    }

                    if (!string.IsNullOrEmpty(ddlThemeStyle.SelectedValue))
                    {
                        if (ddlThemeStyle.SelectedValue != PortalUtils.CurrentThemeStyle)
                        {
                            RemoveThemeOptions();
                        }
                        
                        HttpCookie UserThemeStyleCrum = new HttpCookie("UserThemeStyle", ddlThemeStyle.SelectedValue);
                        UserThemeStyleCrum.Expires = DateTime.Now.AddMonths(2);
                        HttpContext.Current.Response.Cookies.Add(UserThemeStyleCrum);

                        SyncThemeModeCookie(ddlThemeStyle.SelectedValue);

                        ES.Services.Users.UpdateUserThemeSetting(PanelSecurity.LoggedUserId, "Style", ddlThemeStyle.SelectedValue);

                    }

                    SavePaletteSetting("palette-Light", ThemePaletteLightCookie, ReadPaletteValues(rptPaletteMatrix, "txtPaletteLightHex"));
                    SavePaletteSetting("palette-Dark", ThemePaletteDarkCookie, ReadPaletteValues(rptPaletteMatrix, "txtPaletteDarkHex"));
                    SaveButtonSetting("buttons-Light", ThemeButtonsLightCookie, ReadButtonValues(rptButtonMatrix, "txtButtonLightValue", LightButtonDefaults));
                    SaveButtonSetting("buttons-Dark", ThemeButtonsDarkCookie, ReadButtonValues(rptButtonMatrix, "txtButtonDarkValue", DarkButtonDefaults));

                }
                catch (Exception ex)
                {
                    ShowErrorMessage("USER_UPDATE_USER", ex);
                    return;
                }

                // show message
                ShowSuccessMessage("USER_UPDATE_USER");
            }
        }

        private void ChangeUserPassword()
        {
            Page.Validate("NewPassword");
            if (!Page.IsValid)
                return;

            try
            {
                //int result = UsersHelper.ChangeUserPassword(PortalId, PanelRequest.UserID, userPassword.Password);
                int result = PortalUtils.ChangeUserPassword(PanelSecurity.LoggedUserId, userPassword.Password);

                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
                pnlEdit.Visible = false;
                string loginClientUrl = Page.ResolveClientUrl(PortalUtils.LoginRedirectUrl);
                ShowSuccessMessage(Utils.ModuleName, "LOGGED_USER_CHANGE_PASSWORD", loginClientUrl, (redirectTimeout/1000).ToString());
                // Clear auth cookie explicitly using the same attributes as SetAuthTicket
                // to avoid leaving a stale cookie when FormsAuthentication.SignOut() uses
                // different cookie path/domain defaults.
                HttpCookie expireCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
                expireCookie.Domain = FormsAuthentication.CookieDomain;
                expireCookie.Secure = FormsAuthentication.RequireSSL;
                expireCookie.Path = FormsAuthentication.FormsCookiePath;
                expireCookie.HttpOnly = true;
                expireCookie.Expires = DateTime.Now.AddDays(-1);
                expireCookie.Value = "";
                Response.Cookies.Set(expireCookie);
                FormsAuthentication.SignOut();
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "RedirectToLogin", String.Format("setTimeout(\"window.location='{0}'\",{1});", loginClientUrl, redirectTimeout), true); 
            }
            catch (Exception ex)
            {
                ShowErrorMessage("USER_CHANGE_PASSWORD", ex);
                return;
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            SaveUser(false);
        }

        protected void cmdChangePassword_Click(object sender, EventArgs e)
        {
            // change password
            ChangeUserPassword();
        }

        protected void cbxMfaEnabled_CheckedChanged(object sender, EventArgs e)
        {
            UserInfo user = ES.Services.Users.GetUserById(PanelSecurity.LoggedUserId);
            var result = PortalUtils.UpdateUserMfa(user.Username, cbxMfaEnabled.Checked);
            qrData.Visible = false;
            btnGetQRCodeData.Visible = result;
            lblMfaEnabled.Visible = result;
        }

        private static string NormalizeUserText(string value)
        {
            return PortalAntiXSS.Encode((value ?? String.Empty).Trim());
        }

        private static string NormalizeEmail(string value)
        {
            return (value ?? String.Empty).Trim();
        }

        private void SetCurrentLanguage()
        {
            PortalUtils.SetCurrentLanguage(ddlLanguage.SelectedValue);
        }

        private void SetCurrentTheme()
        {
            string selectedTheme = ddlTheme.SelectedValue;

            HttpCookie userRtlCookie = HttpContext.Current.Response.Cookies["UserRTL"];
            if (userRtlCookie != null && userRtlCookie.Value == "1")
            {
                DataSet themeData = ES.Services.Authentication.GetLoginThemes();
                selectedTheme = themeData.Tables[0].Rows[ddlTheme.SelectedIndex]["RTLName"].ToString();
            }

            RemoveThemeOptions();

            PortalUtils.SetCurrentTheme(selectedTheme);

        }

        protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCurrentLanguage();

            HttpCookie userThemeCookie = HttpContext.Current.Response.Cookies["UserTheme"];
            if (userThemeCookie != null && !string.IsNullOrEmpty(userThemeCookie.Value))
            {
                SetCurrentTheme();
            }

            Response.Redirect(Request.Url.ToString());
        }

        protected void ddlTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCurrentTheme();
            Response.Redirect(Request.Url.ToString());

        }

        private void BindPaletteEditors(DataSet userThemeSettingsData)
        {
            string lightCsv = GetThemeSettingValue(userThemeSettingsData, "palette-Light");
            string darkCsv = GetThemeSettingValue(userThemeSettingsData, "palette-Dark");

            rptPaletteMatrix.DataSource = BuildPaletteMatrixRows(lightCsv, darkCsv);
            rptPaletteMatrix.DataBind();
        }

        private void BindButtonStyleEditors(DataSet userThemeSettingsData)
        {
            string lightCsv = GetThemeSettingValue(userThemeSettingsData, "buttons-Light");
            string darkCsv = GetThemeSettingValue(userThemeSettingsData, "buttons-Dark");

            rptButtonMatrix.DataSource = BuildButtonMatrixRows(lightCsv, darkCsv);
            rptButtonMatrix.DataBind();
        }

        private static string GetThemeSettingValue(DataSet settingsData, string key)
        {
            if (settingsData == null || settingsData.Tables.Count == 0)
                return String.Empty;

            foreach (DataRow row in settingsData.Tables[0].Rows)
            {
                if (String.Equals(row.Field<string>("PropertyName"), key, StringComparison.OrdinalIgnoreCase))
                {
                    return row.Field<string>("PropertyValue") ?? String.Empty;
                }
            }

            return String.Empty;
        }

        private IList<PaletteMatrixSetting> BuildPaletteMatrixRows(string lightCsv, string darkCsv)
        {
            var rows = new List<PaletteMatrixSetting>(LightPaletteDefaults.Length);
            string[] lightValues = ParsePaletteCsv(lightCsv, LightPaletteDefaults.Length);
            string[] darkValues = ParsePaletteCsv(darkCsv, DarkPaletteDefaults.Length);

            for (int i = 0; i < LightPaletteDefaults.Length; i++)
            {
                string localizedDisplayName = GetPaletteDisplayName(LightPaletteDefaults[i].CssVariable, LightPaletteDefaults[i].DisplayName);
                rows.Add(new PaletteMatrixSetting(
                    localizedDisplayName,
                    lightValues != null ? lightValues[i] : LightPaletteDefaults[i].ColorValue,
                    darkValues != null ? darkValues[i] : DarkPaletteDefaults[i].ColorValue));
            }

            return rows;
        }

        private string GetPaletteDisplayName(string cssVariable, string fallback)
        {
            string key;

            switch (cssVariable)
            {
                case "--fcp-bg":
                case "--fcp-dark-bg":
                    key = "Palette.PageBackground";
                    break;
                case "--fcp-topbar-bg":
                case "--fcp-dark-topbar-bg":
                    key = "Palette.TopBarBackground";
                    break;
                case "--fcp-topbar-bdr":
                case "--fcp-dark-topbar-bdr":
                    key = "Palette.TopBarBorder";
                    break;
                case "--fcp-sidebar-bg":
                case "--fcp-dark-sidebar-bg":
                    key = "Palette.SidebarBackground";
                    break;
                case "--fcp-surface":
                case "--fcp-dark-surface":
                    key = "Palette.Surface";
                    break;
                case "--fcp-surface-alt":
                case "--fcp-dark-surface-alt":
                    key = "Palette.SurfaceAlt";
                    break;
                case "--fcp-surface-h":
                case "--fcp-dark-surface-h":
                    key = "Palette.SurfaceHover";
                    break;
                case "--fcp-border":
                case "--fcp-dark-border":
                    key = "Palette.Border";
                    break;
                case "--fcp-input-bdr":
                case "--fcp-dark-input-bdr":
                    key = "Palette.InputBorder";
                    break;
                case "--fcp-text":
                case "--fcp-dark-text":
                    key = "Palette.Text";
                    break;
                case "--fcp-text-hi":
                case "--fcp-dark-text-hi":
                    key = "Palette.TextHigh";
                    break;
                case "--fcp-accent":
                case "--fcp-dark-accent":
                    key = "Palette.Accent";
                    break;
                default:
                    return fallback;
            }

            string localized = GetLocalizedString(key);
            return String.IsNullOrWhiteSpace(localized) ? fallback : localized;
        }

        private static string[] ParsePaletteCsv(string csv, int expectedLength)
        {
            if (String.IsNullOrWhiteSpace(csv))
                return null;

            string[] values = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != expectedLength)
                return null;

            for (int i = 0; i < values.Length; i++)
            {
                string normalized = NormalizeHex(values[i]);
                if (!PaletteHexColorRegex.IsMatch(normalized))
                    return null;

                values[i] = normalized;
            }

            return values;
        }

        private static string NormalizeHex(string value)
        {
            return (value ?? String.Empty).Trim().ToUpperInvariant();
        }

        private static string[] ReadPaletteValues(Repeater repeater, string textBoxControlId)
        {
            var values = new List<string>();

            foreach (RepeaterItem item in repeater.Items)
            {
            var txt = item.FindControl(textBoxControlId) as TextBox;
                string normalized = NormalizeHex(txt != null ? txt.Text : String.Empty);

                if (!PaletteHexColorRegex.IsMatch(normalized))
                {
                    throw new InvalidOperationException("Invalid palette color value. Use #RRGGBB format.");
                }

                values.Add(normalized);
            }

            return values.ToArray();
        }

        private void SavePaletteSetting(string propertyName, string cookieName, string[] values)
        {
            string csv = String.Join(",", values);
            ES.Services.Users.UpdateUserThemeSetting(PanelSecurity.LoggedUserId, propertyName, csv);

            HttpCookie cookie = new HttpCookie(cookieName, csv);
            cookie.Expires = DateTime.Now.AddMonths(2);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private IList<ButtonMatrixSetting> BuildButtonMatrixRows(string lightCsv, string darkCsv)
        {
            var rows = new List<ButtonMatrixSetting>(LightButtonDefaults.Length);
            string[] lightValues = ParseButtonCsv(lightCsv, LightButtonDefaults);
            string[] darkValues = ParseButtonCsv(darkCsv, DarkButtonDefaults);

            for (int i = 0; i < LightButtonDefaults.Length; i++)
            {
                rows.Add(new ButtonMatrixSetting(
                    LightButtonDefaults[i].DisplayName,
                    lightValues != null ? lightValues[i] : LightButtonDefaults[i].DefaultValue,
                    darkValues != null ? darkValues[i] : DarkButtonDefaults[i].DefaultValue,
                    LightButtonDefaults[i].IsColorValue));
            }

            return rows;
        }

        private static string[] ParseButtonCsv(string csv, ButtonStyleSetting[] defaults)
        {
            if (String.IsNullOrWhiteSpace(csv))
                return null;

            string[] values = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != defaults.Length)
                return null;

            for (int i = 0; i < values.Length; i++)
            {
                string normalized = NormalizeButtonValue(values[i], defaults[i].IsColorValue);
                if (normalized == null)
                    return null;

                values[i] = normalized;
            }

            return values;
        }

        private static string[] ReadButtonValues(Repeater repeater, string textBoxControlId, ButtonStyleSetting[] defaults)
        {
            var values = new List<string>(defaults.Length);
            int index = 0;

            foreach (RepeaterItem item in repeater.Items)
            {
                if (index >= defaults.Length)
                    break;

                var txt = item.FindControl(textBoxControlId) as TextBox;
                string normalized = NormalizeButtonValue(txt != null ? txt.Text : String.Empty, defaults[index].IsColorValue);

                if (normalized == null)
                {
                    throw new InvalidOperationException(defaults[index].IsColorValue
                        ? "Invalid button color value. Use #RRGGBB format."
                        : "Invalid button radius value. Use 0, px, rem, em, or %." );
                }

                values.Add(normalized);
                index++;
            }

            if (values.Count != defaults.Length)
            {
                throw new InvalidOperationException("Button style settings are incomplete.");
            }

            return values.ToArray();
        }

        private static string NormalizeButtonValue(string value, bool isColor)
        {
            string normalized = (value ?? String.Empty).Trim();
            if (isColor)
            {
                normalized = normalized.ToUpperInvariant();
                return PaletteHexColorRegex.IsMatch(normalized) ? normalized : null;
            }

            return CssLengthRegex.IsMatch(normalized) ? normalized.ToLowerInvariant() : null;
        }

        private void SaveButtonSetting(string propertyName, string cookieName, string[] values)
        {
            string csv = String.Join(",", values);
            ES.Services.Users.UpdateUserThemeSetting(PanelSecurity.LoggedUserId, propertyName, csv);

            HttpCookie cookie = new HttpCookie(cookieName, csv);
            cookie.Expires = DateTime.Now.AddMonths(2);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        protected void cmdResetDisplay_Click(object sender, EventArgs e)
        {
            RemoveThemeOptions();
            Response.Redirect(Request.Url.ToString());
        }

        private void ApplySelectedThemeMode()
        {
            HttpCookie modeCookie = Request.Cookies[ThemeModeCookie];
            if (modeCookie == null)
                return;

            string modeValue = (modeCookie.Value ?? String.Empty).Trim();
            if (!String.Equals(modeValue, ThemeStyleLight, StringComparison.OrdinalIgnoreCase)
                && !String.Equals(modeValue, ThemeStyleDark, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Utils.SelectListItem(ddlThemeStyle, modeValue);
        }

        private void SyncThemeModeCookie(string selectedThemeStyle)
        {
            string normalizedStyle = (selectedThemeStyle ?? String.Empty).Trim();

            if (String.Equals(normalizedStyle, ThemeStyleLight, StringComparison.OrdinalIgnoreCase)
                || String.Equals(normalizedStyle, ThemeStyleDark, StringComparison.OrdinalIgnoreCase))
            {
                HttpCookie modeCookie = new HttpCookie(ThemeModeCookie, normalizedStyle.ToLowerInvariant());
                modeCookie.Expires = DateTime.Now.AddMonths(2);
                HttpContext.Current.Response.Cookies.Add(modeCookie);
                return;
            }

            HttpCookie expiredModeCookie = new HttpCookie(ThemeModeCookie, String.Empty);
            expiredModeCookie.Expires = DateTime.Now.AddMonths(-1);
            HttpContext.Current.Response.Cookies.Add(expiredModeCookie);
        }

        protected void RemoveThemeOptions()
        {
            ES.Services.Users.DeleteUserThemeSetting(PanelSecurity.LoggedUserId, "Style"); 
            ES.Services.Users.DeleteUserThemeSetting(PanelSecurity.LoggedUserId, "color-Header");
            ES.Services.Users.DeleteUserThemeSetting(PanelSecurity.LoggedUserId, "color-Sidebar");
            ES.Services.Users.DeleteUserThemeSetting(PanelSecurity.LoggedUserId, "colorHeader");
            ES.Services.Users.DeleteUserThemeSetting(PanelSecurity.LoggedUserId, "colorSidebar");
            ES.Services.Users.DeleteUserThemeSetting(PanelSecurity.LoggedUserId, "palette-Light");
            ES.Services.Users.DeleteUserThemeSetting(PanelSecurity.LoggedUserId, "palette-Dark");
            ES.Services.Users.DeleteUserThemeSetting(PanelSecurity.LoggedUserId, "buttons-Light");
            ES.Services.Users.DeleteUserThemeSetting(PanelSecurity.LoggedUserId, "buttons-Dark");

            HttpCookie UserThemeStyleCrum = new HttpCookie("UserThemeStyle", "");
            UserThemeStyleCrum.Expires = DateTime.Now.AddMonths(-1);
            HttpContext.Current.Response.Cookies.Add(UserThemeStyleCrum);

            HttpCookie userThemeModeCrumb = new HttpCookie(ThemeModeCookie, "");
            userThemeModeCrumb.Expires = DateTime.Now.AddMonths(-1);
            HttpContext.Current.Response.Cookies.Add(userThemeModeCrumb);

            HttpCookie legacyHeaderColorCookie = new HttpCookie("UserThemecolorHeader", "");
            legacyHeaderColorCookie.Expires = DateTime.Now.AddMonths(-1);
            HttpContext.Current.Response.Cookies.Add(legacyHeaderColorCookie);

            HttpCookie legacySidebarColorCookie = new HttpCookie("UserThemecolorSidebar", "");
            legacySidebarColorCookie.Expires = DateTime.Now.AddMonths(-1);
            HttpContext.Current.Response.Cookies.Add(legacySidebarColorCookie);

            HttpCookie userThemePaletteLightCrumb = new HttpCookie("UserThemePaletteLight", "");
            userThemePaletteLightCrumb.Expires = DateTime.Now.AddMonths(-1);
            HttpContext.Current.Response.Cookies.Add(userThemePaletteLightCrumb);

            HttpCookie userThemePaletteDarkCrumb = new HttpCookie("UserThemePaletteDark", "");
            userThemePaletteDarkCrumb.Expires = DateTime.Now.AddMonths(-1);
            HttpContext.Current.Response.Cookies.Add(userThemePaletteDarkCrumb);

            HttpCookie userThemeButtonsLightCrumb = new HttpCookie("UserThemeButtonsLight", "");
            userThemeButtonsLightCrumb.Expires = DateTime.Now.AddMonths(-1);
            HttpContext.Current.Response.Cookies.Add(userThemeButtonsLightCrumb);

            HttpCookie userThemeButtonsDarkCrumb = new HttpCookie("UserThemeButtonsDark", "");
            userThemeButtonsDarkCrumb.Expires = DateTime.Now.AddMonths(-1);
            HttpContext.Current.Response.Cookies.Add(userThemeButtonsDarkCrumb);
        }

        private sealed class PaletteColorSetting
        {
            public PaletteColorSetting(string cssVariable, string displayName, string colorValue)
            {
                CssVariable = cssVariable;
                DisplayName = displayName;
                ColorValue = colorValue;
            }

            public string CssVariable { get; private set; }
            public string DisplayName { get; private set; }
            public string ColorValue { get; private set; }
        }

        private sealed class ButtonStyleSetting
        {
            public ButtonStyleSetting(string cssVariable, string displayName, string defaultValue, bool isColorValue)
            {
                CssVariable = cssVariable;
                DisplayName = displayName;
                DefaultValue = defaultValue;
                IsColorValue = isColorValue;
            }

            public string CssVariable { get; private set; }
            public string DisplayName { get; private set; }
            public string DefaultValue { get; private set; }
            public bool IsColorValue { get; private set; }
        }

        private sealed class PaletteMatrixSetting
        {
            public PaletteMatrixSetting(string displayName, string lightColorValue, string darkColorValue)
            {
                DisplayName = displayName;
                LightColorValue = lightColorValue;
                DarkColorValue = darkColorValue;
            }

            public string DisplayName { get; private set; }
            public string LightColorValue { get; private set; }
            public string DarkColorValue { get; private set; }
        }

        private sealed class ButtonMatrixSetting
        {
            public ButtonMatrixSetting(string displayName, string lightValue, string darkValue, bool isColorValue)
            {
                DisplayName = displayName;
                LightValue = lightValue;
                DarkValue = darkValue;
                IsColorValue = isColorValue;
            }

            public string DisplayName { get; private set; }
            public string LightValue { get; private set; }
            public string DarkValue { get; private set; }
            public bool IsColorValue { get; private set; }
        }

        protected void btnGetQRCodeData_Click(object sender, EventArgs e)
        {
            UserInfo user = ES.Services.Users.GetUserById(PanelSecurity.LoggedUserId);
            var qrCodeData = PortalUtils.GetQrCodeData(user.Username);
            if (cbxMfaEnabled.Checked && qrCodeData.Length == 2)
            {
                qrData.Visible = true;
                lblManualAuth.Text = qrCodeData[0];
                imgQrCode.ImageUrl = qrCodeData[1];
            }
        }

        protected void btnActivateQRCode_Click(object sender, EventArgs e)
        {
            UserInfo user = ES.Services.Users.GetUserById(PanelSecurity.LoggedUserId);
            var success = PortalUtils.ActivateQrCode(user.Username, txtQrCodeActivationPin.Text.Trim());
            if (!success)
            {
                ShowErrorMessage("QRCodeActivation");
                return;
            }

            qrData.Visible = false;
            ShowSuccessMessage("QRCodeActivation");
        }
    }
}
