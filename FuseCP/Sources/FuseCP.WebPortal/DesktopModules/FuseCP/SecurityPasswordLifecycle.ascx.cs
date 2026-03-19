// Copyright (C) 2026 FuseCP
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    public partial class SecurityPasswordLifecycle : FuseCPModuleBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnRefreshStatus.Click += btnRefreshStatus_Click;
            btnLoadLegacyUsers.Click += btnLoadLegacyUsers_Click;
            btnExportLegacyUsers.Click += btnExportLegacyUsers_Click;
            btnAutoHardenEligibleUsers.Click += btnAutoHardenEligibleUsers_Click;
            gvServerStatus.RowCommand += gvServerStatus_RowCommand;
            gvServerStatus.RowDataBound += gvServerStatus_RowDataBound;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeActionLinks();
                LoadStatus();
                LoadLegacyUsers();
            }
        }

        private void InitializeActionLinks()
        {
            lnkFixUsers.NavigateUrl = ResolveUrl("~/Default.aspx?pid=SecurityCredentialHardening#user-convert");
            lnkFixPortal.NavigateUrl = ResolveUrl("~/Default.aspx?pid=SecurityPortalAuthentication");
			lnkFixServers.NavigateUrl = ResolveUrl("~/Default.aspx?pid=SecurityCredentialHardening#server-status");
			lnkFixUsers.Text = "Use User Section";
			lnkFixServers.Text = "Use Server Section";
        }

        protected void btnRefreshStatus_Click(object sender, EventArgs e)
        {
            LoadStatus();
        }

        protected void btnAutoHardenEligibleUsers_Click(object sender, EventArgs e)
        {
            try
            {
                var result = ES.Services.Users.AutoHardenEligibleUserPasswords();
                ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
                lblAutoHardenSummary.Text = HttpUtility.HtmlEncode($"Converted {result?.ConvertedUserCount ?? 0} eligible user password record(s). Remaining legacy or empty records: {result?.RemainingLegacyUserCount ?? 0}.");
                LoadStatus();
                LoadLegacyUsers();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
            }
        }

        protected void btnLoadLegacyUsers_Click(object sender, EventArgs e)
        {
            LoadLegacyUsers();
        }

        protected void btnExportLegacyUsers_Click(object sender, EventArgs e)
        {
            try
            {
                IList<LegacyPasswordUserInfo> users = FetchLegacyUsers();
                ExportLegacyUsersCsv(users);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_LOAD", ex);
            }
        }

        private void LoadStatus()
        {
            try
            {
                var status = ES.Services.Users.GetPasswordHardeningStatus();
                if (status == null)
                {
                    ShowErrorMessage("SYSTEM_SETTINGS_LOAD");
                    return;
                }

                SetStatusLabel(lblUsersStatus,
                    status.LegacyUserCount == 0 && status.EmptyUserPasswordCount == 0,
                    status.HardenedUserCount > 0,
                    "Secure",
                    "Mixed",
                    "Legacy");
				lblUsersSummary.Text = HttpUtility.HtmlEncode($"{status.HardenedUserCount} hardened, {status.LegacyUserCount} legacy, {status.EmptyUserPasswordCount} empty of {status.TotalUserCount} users. {status.AutoHardenEligibleUserCount} can be converted immediately.");

                bool portalSecure = status.PortalPasswordResetEnabled && status.MfaDisplayNameConfigured;
                SetStatusLabel(lblPortalStatus,
                    portalSecure,
                    status.PortalPasswordResetEnabled || status.MfaDisplayNameConfigured,
                    "Secure",
                    "Mixed",
                    "Legacy");
                lblPortalSummary.Text = HttpUtility.HtmlEncode($"Reset {(status.PortalPasswordResetEnabled ? "enabled" : "disabled")}, MFA app {(status.MfaDisplayNameConfigured ? "configured" : "missing")}, peers {(status.CanPeerChangeMfa ? "may change MFA" : "cannot change MFA")}.");

                SetStatusLabel(lblEnterpriseStatus,
                    status.ApiBruteForceProtectionEnabled && status.UserLazyMigrationEnabled && status.ServerRequestAuthenticationEnabled,
                    status.ApiBruteForceProtectionEnabled || status.UserLazyMigrationEnabled,
                    "Secure",
                    "Mixed",
                    "Legacy");
                lblEnterpriseSummary.Text = HttpUtility.HtmlEncode($"Brute force: {status.BruteForceMaxFailedAttempts}/{status.BruteForceWindowMinutes}m, lockout {status.BruteForceLockoutMinutes}m, critical {status.BruteForceCriticalAttemptThreshold}. Lazy migration active.");

                bool serversSecure = status.TotalServerCount == 0 || (status.LegacyServerCount == 0 && status.ServerProbeFailureCount == 0);
                SetStatusLabel(lblServersStatus,
                    serversSecure,
                    status.HmacCapableServerCount > 0 || status.Sha256ServerCount > 0,
                    "Secure",
                    "Mixed",
                    "Legacy");
                lblServersSummary.Text = HttpUtility.HtmlEncode($"{status.HmacCapableServerCount} HMAC-ready, {status.Sha256ServerCount} SHA256, {status.LegacyServerCount} legacy-compatible, {status.ServerProbeFailureCount} probe failures across {status.TotalServerCount} servers.");

                lblGeneratedUtc.Text = HttpUtility.HtmlEncode($"Generated UTC: {status.GeneratedUtc:yyyy-MM-dd HH:mm:ss}. Bootstrap setup flag is currently {(status.SetupModeEnabled ? "enabled" : "disabled")} in Enterprise settings.");
                gvServerStatus.DataSource = status.Servers;
                gvServerStatus.DataBind();

                BindFixNowActions(status);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_LOAD", ex);
            }
        }

        private void BindFixNowActions(PasswordHardeningStatusInfo status)
        {
            int urgentIssues = 0;

            if (status.LegacyUserCount > 0 || status.EmptyUserPasswordCount > 0)
            {
                urgentIssues++;
				lblFixUsersAction.Text = HttpUtility.HtmlEncode($"{status.LegacyUserCount} legacy and {status.EmptyUserPasswordCount} empty-password users still need migration or reset. {status.AutoHardenEligibleUserCount} can be converted immediately here.");
                lnkFixUsers.Visible = true;
                lnkFixUsers.CssClass = "btn btn-sm btn-danger";
            }
            else
            {
                lblFixUsersAction.Text = "No immediate action. All user passwords are hardened.";
                lnkFixUsers.Visible = false;
            }

            if (!status.PortalPasswordResetEnabled || !status.MfaDisplayNameConfigured)
            {
                urgentIssues++;
                lblFixPortalAction.Text = HttpUtility.HtmlEncode($"Portal password reset is {(status.PortalPasswordResetEnabled ? "enabled" : "disabled")}; MFA app name is {(status.MfaDisplayNameConfigured ? "configured" : "missing")}.");
                lnkFixPortal.Visible = true;
                lnkFixPortal.CssClass = "btn btn-sm btn-danger";
            }
            else
            {
                lblFixPortalAction.Text = "No immediate action. Portal authentication settings look good.";
                lnkFixPortal.Visible = false;
            }

            if (status.LegacyServerCount > 0 || status.ServerProbeFailureCount > 0)
            {
                urgentIssues++;
				lblFixServersAction.Text = HttpUtility.HtmlEncode($"{status.LegacyServerCount} server(s) still expose legacy compatibility or fallback and {status.ServerProbeFailureCount} probe failure(s) need recovery validation.");
                lnkFixServers.Visible = true;
                lnkFixServers.CssClass = "btn btn-sm btn-danger";
            }
            else
            {
                lblFixServersAction.Text = "No immediate action. Server auth posture is clean.";
                lnkFixServers.Visible = false;
            }

            if (urgentIssues == 0)
            {
                pnlFixNow.CssClass = "card border-success mb-3";
                lblFixNowSeverity.Text = "No urgent blockers";
                lblFixNowSummary.Text = "Everything critical is already in a good state. Use the sections below for audits and targeted maintenance.";
            }
            else
            {
                pnlFixNow.CssClass = "card border-danger mb-3";
                lblFixNowSeverity.Text = urgentIssues == 1 ? "1 urgent blocker" : $"{urgentIssues} urgent blockers";
                lblFixNowSummary.Text = "Complete the actions below first, then refresh status to confirm hardening moved to green.";
            }
        }

        private void LoadLegacyUsers()
        {
            try
            {
                IList<LegacyPasswordUserInfo> users = FetchLegacyUsers();
                gvLegacyUsers.DataSource = users;
                gvLegacyUsers.DataBind();
				int autoHardenableUsers = users.Count(user => user.CanAutoHarden);
				lblLegacyInventorySummary.Text = HttpUtility.HtmlEncode($"{users.Count} matching legacy users loaded. {autoHardenableUsers} can be converted immediately.");
				lblAutoHardenSummary.Text = HttpUtility.HtmlEncode(autoHardenableUsers > 0
					? $"{autoHardenableUsers} loaded user record(s) are currently auto-fixable."
					: "No auto-fixable user records are in the current result set.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_LOAD", ex);
            }
        }

        private IList<LegacyPasswordUserInfo> FetchLegacyUsers()
        {
            string filter = string.IsNullOrWhiteSpace(txtLegacyUserFilter.Text) ? null : txtLegacyUserFilter.Text.Trim();
            int maxResults = ParseMaxResults();
            return ES.Services.Users.GetLegacyPasswordUsers(maxResults, filter) ?? Array.Empty<LegacyPasswordUserInfo>();
        }

        private int ParseMaxResults()
        {
            if (!int.TryParse(txtLegacyUserMaxResults.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int maxResults))
                return 200;

            if (maxResults <= 0)
                return 200;

            return Math.Min(maxResults, 2000);
        }

        private void ExportLegacyUsersCsv(IList<LegacyPasswordUserInfo> users)
        {
            var builder = new StringBuilder();
            builder.AppendLine("UserId,Username,Email,PasswordStatus,CanAutoHarden,IsDemo,RoleId,ChangedUtc");

            foreach (LegacyPasswordUserInfo user in users.OrderBy(u => u.Username))
            {
                builder.Append(Csv(user.UserId.ToString(CultureInfo.InvariantCulture))).Append(',')
                    .Append(Csv(user.Username)).Append(',')
                    .Append(Csv(user.Email)).Append(',')
                    .Append(Csv(user.PasswordStatus)).Append(',')
                    .Append(Csv(user.CanAutoHarden ? "true" : "false")).Append(',')
                    .Append(Csv(user.IsDemo ? "true" : "false")).Append(',')
                    .Append(Csv(user.RoleId.ToString(CultureInfo.InvariantCulture))).Append(',')
                    .Append(Csv(user.Changed.HasValue ? user.Changed.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) : string.Empty))
                    .AppendLine();
            }

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.ContentEncoding = Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment; filename=legacy-password-users.csv");
            Response.Write(builder.ToString());
            Response.End();
        }

        private static string Csv(string value)
        {
            string safeValue = value ?? string.Empty;
            if (safeValue.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0)
                return '"' + safeValue.Replace("\"", "\"\"") + '"';

            return safeValue;
        }

        protected void gvServerStatus_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "FixServer", StringComparison.Ordinal))
                return;

            if (!int.TryParse(Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out int serverId))
                return;

            try
            {
                var result = ES.Services.Servers.HardenServerAuthentication(serverId);
                if (result == null)
                {
                    ShowErrorMessage("SYSTEM_SETTINGS_SAVE", new InvalidOperationException("Server hardening did not return an updated authentication profile."));
                    return;
                }

                ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
                LoadStatus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
            }
        }

        protected void gvServerStatus_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var server = e.Row.DataItem as ServerPasswordHardeningStatusInfo;
            var button = e.Row.FindControl("btnFixServer") as LinkButton;
            if (server == null || button == null)
                return;

            bool canFixInPortal = server.ProbeSucceeded &&
                (server.SupportsLegacyPasswordAuthentication || !server.PasswordIsSha256 || !server.SupportsHmacAuthentication);

            button.Visible = canFixInPortal;
            if (canFixInPortal)
                button.OnClientClick = "return confirm('Rotate this server shared secret, switch it to SHA256, and disable legacy fallback?');";
        }

        private static void SetStatusLabel(System.Web.UI.WebControls.Label label, bool success, bool warning, string successText, string warningText, string dangerText)
        {
            if (success)
            {
                label.Text = successText;
                label.CssClass = "badge bg-success";
                return;
            }

            if (warning)
            {
                label.Text = warningText;
                label.CssClass = "badge bg-warning text-dark";
                return;
            }

            label.Text = dangerText;
            label.CssClass = "badge bg-danger";
        }
    }
}
