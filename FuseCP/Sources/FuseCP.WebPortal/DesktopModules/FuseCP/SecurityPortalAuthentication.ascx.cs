using System;
using FCP = FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    public partial class SecurityPortalAuthentication : FuseCPModuleBase
    {
        private const int DefaultBruteForceMaxFailedAttempts = 5;
        private const int DefaultBruteForceWindowMinutes = 30;
        private const int DefaultBruteForceLockoutMinutes = 15;
        private const int DefaultBruteForceCriticalAttempts = 20;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSettings();
            }
        }

        private void LoadSettings()
        {
            try
            {
                FCP.SystemSettings authSettings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.AUTHENTICATION_SETTINGS);
                if (authSettings != null)
                {
                    txtMfaTokenAppDisplayName.Text = authSettings.GetValueOrDefault(FCP.SystemSettings.MFA_TOKEN_APP_DISPLAY_NAME, string.Empty);
                    chkCanPeerChangeMFa.Checked = authSettings.GetValueOrDefault(FCP.SystemSettings.MFA_CAN_PEER_CHANGE_MFA, true);
                    txtBruteForceMaxFailedAttempts.Text = authSettings.GetValueOrDefault(FCP.SystemSettings.AUTH_BRUTEFORCE_MAX_FAILED_ATTEMPTS, DefaultBruteForceMaxFailedAttempts).ToString();
                    txtBruteForceWindowMinutes.Text = authSettings.GetValueOrDefault(FCP.SystemSettings.AUTH_BRUTEFORCE_WINDOW_MINUTES, DefaultBruteForceWindowMinutes).ToString();
                    txtBruteForceLockoutMinutes.Text = authSettings.GetValueOrDefault(FCP.SystemSettings.AUTH_BRUTEFORCE_LOCKOUT_MINUTES, DefaultBruteForceLockoutMinutes).ToString();
                    txtBruteForceCriticalAttempts.Text = authSettings.GetValueOrDefault(FCP.SystemSettings.AUTH_BRUTEFORCE_CRITICAL_ATTEMPTS, DefaultBruteForceCriticalAttempts).ToString();
                }
                else
                {
                    txtBruteForceMaxFailedAttempts.Text = DefaultBruteForceMaxFailedAttempts.ToString();
                    txtBruteForceWindowMinutes.Text = DefaultBruteForceWindowMinutes.ToString();
                    txtBruteForceLockoutMinutes.Text = DefaultBruteForceLockoutMinutes.ToString();
                    txtBruteForceCriticalAttempts.Text = DefaultBruteForceCriticalAttempts.ToString();
                }

                FCP.SystemSettings portalAuthSettings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.WEBDAV_PORTAL_SETTINGS);
                if (portalAuthSettings != null)
                {
                    chkEnablePasswordReset.Checked = Utils.ParseBool(portalAuthSettings[FCP.SystemSettings.WEBDAV_PASSWORD_RESET_ENABLED_KEY], false);
                    txtPasswordResetLinkLifeSpan.Text = portalAuthSettings[FCP.SystemSettings.WEBDAV_PASSWORD_RESET_LINK_LIFE_SPAN];
                }

                BindFixNowStatus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_LOAD", ex);
            }
        }

        protected void btnSavePortalAuth_Click(object sender, EventArgs e)
        {
            try
            {
                FCP.SystemSettings settings = new FCP.SystemSettings();
                settings[FCP.SystemSettings.WEBDAV_PASSWORD_RESET_ENABLED_KEY] = chkEnablePasswordReset.Checked.ToString();
                settings[FCP.SystemSettings.WEBDAV_PASSWORD_RESET_LINK_LIFE_SPAN] = txtPasswordResetLinkLifeSpan.Text;

                int result = ES.Services.System.SetSystemSettings(FCP.SystemSettings.WEBDAV_PORTAL_SETTINGS, settings);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }

                ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
                BindFixNowStatus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
            }
        }

        protected void btnSaveMfa_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TryParsePositiveInt(txtBruteForceMaxFailedAttempts.Text, 1, 1000, out int maxAttempts) ||
                    !TryParsePositiveInt(txtBruteForceWindowMinutes.Text, 1, 1440, out int windowMinutes) ||
                    !TryParsePositiveInt(txtBruteForceLockoutMinutes.Text, 1, 10080, out int lockoutMinutes) ||
                    !TryParsePositiveInt(txtBruteForceCriticalAttempts.Text, 1, 5000, out int criticalAttempts))
                {
                    ShowErrorMessage("Please enter valid numeric brute force settings.");
                    return;
                }

                FCP.SystemSettings settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.AUTHENTICATION_SETTINGS) ?? new FCP.SystemSettings();
                settings[FCP.SystemSettings.MFA_TOKEN_APP_DISPLAY_NAME] = txtMfaTokenAppDisplayName.Text.Trim();
                settings[FCP.SystemSettings.MFA_CAN_PEER_CHANGE_MFA] = chkCanPeerChangeMFa.Checked ? "True" : "False";
                settings[FCP.SystemSettings.AUTH_BRUTEFORCE_MAX_FAILED_ATTEMPTS] = maxAttempts.ToString();
                settings[FCP.SystemSettings.AUTH_BRUTEFORCE_WINDOW_MINUTES] = windowMinutes.ToString();
                settings[FCP.SystemSettings.AUTH_BRUTEFORCE_LOCKOUT_MINUTES] = lockoutMinutes.ToString();
                settings[FCP.SystemSettings.AUTH_BRUTEFORCE_CRITICAL_ATTEMPTS] = criticalAttempts.ToString();

                int result = ES.Services.System.SetSystemSettings(FCP.SystemSettings.AUTHENTICATION_SETTINGS, settings);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }

                ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
                BindFixNowStatus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
            }
        }

        private void BindFixNowStatus()
        {
            int blockers = 0;

            bool passwordResetConfigured = chkEnablePasswordReset.Checked && !string.IsNullOrWhiteSpace(txtPasswordResetLinkLifeSpan.Text);
            SetStatusBadge(lblPasswordResetStatus, passwordResetConfigured,
                passwordResetConfigured ? "Configured" : (chkEnablePasswordReset.Checked ? "Lifespan Missing" : "Disabled"));
            if (!passwordResetConfigured) blockers++;

            bool mfaConfigured = !string.IsNullOrWhiteSpace(txtMfaTokenAppDisplayName.Text);
            SetStatusBadge(lblMfaStatus, mfaConfigured, mfaConfigured ? "Configured" : "Missing");
            if (!mfaConfigured) blockers++;

            bool bruteForceConfigured =
                TryParsePositiveInt(txtBruteForceMaxFailedAttempts.Text, 1, 1000, out _) &&
                TryParsePositiveInt(txtBruteForceWindowMinutes.Text, 1, 1440, out _) &&
                TryParsePositiveInt(txtBruteForceLockoutMinutes.Text, 1, 10080, out _) &&
                TryParsePositiveInt(txtBruteForceCriticalAttempts.Text, 1, 5000, out _);
            SetStatusBadge(lblBruteForceStatus, bruteForceConfigured, bruteForceConfigured ? "Configured" : "Invalid");
            if (!bruteForceConfigured) blockers++;

            if (blockers == 0)
            {
                pnlFixNow.CssClass = "card border-success mb-3";
                lblFixNowSeverity.Text = "No urgent blockers";
                lblFixNowSummary.Text = "Portal authentication controls are configured. Maintain values and re-check after policy changes.";
            }
            else
            {
                pnlFixNow.CssClass = "card border-danger mb-3";
                lblFixNowSeverity.Text = blockers == 1 ? "1 urgent blocker" : $"{blockers} urgent blockers";
                lblFixNowSummary.Text = "Address all red or missing items below, save each section, then confirm this panel turns green.";
            }
        }

        private static void SetStatusBadge(System.Web.UI.WebControls.Label label, bool success, string text)
        {
            label.Text = text;
            label.CssClass = success ? "badge bg-success" : "badge bg-danger";
        }

        private static bool TryParsePositiveInt(string value, int minValue, int maxValue, out int result)
        {
            result = 0;
            if (!int.TryParse(value, out int parsed))
            {
                return false;
            }

            if (parsed < minValue || parsed > maxValue)
            {
                return false;
            }

            result = parsed;
            return true;
        }
    }
}
