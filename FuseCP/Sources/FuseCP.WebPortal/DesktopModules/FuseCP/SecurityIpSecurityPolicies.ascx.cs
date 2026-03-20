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
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using FCP = FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    public partial class SecurityIpSecurityPolicies : FuseCPModuleBase
    {
        private HashSet<string> portalAdminAllowlist;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            gvPolicies.RowDataBound += gvPolicies_RowDataBound;
            gvPolicies.RowCommand += gvPolicies_RowCommand;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // WebForms CheckBox renders a wrapper span; set Bootstrap class on the actual input.
            chkServerAdminAccess.InputAttributes["class"] = "form-check-input";

            if (!IsPostBack)
            {
                BindPolicies();
            }
        }

        protected void btnBlock_Click(object sender, EventArgs e)
        {
            ExecutePolicyAction(() =>
            {
                string ipRange = GetSubmittedIpRange();
                ES.Services.Users.BlockIpAddress(
                    ipRange,
                    txtReason.Text?.Trim(),
                    ParseDurationMinutes());
                SetPortalAdminAccess(ipRange, false);
            });
        }

        protected void btnWhitelist_Click(object sender, EventArgs e)
        {
            ExecutePolicyAction(() =>
            {
                string ipRange = GetSubmittedIpRange();
                ES.Services.Users.WhitelistIpAddress(
                    ipRange,
                    txtReason.Text?.Trim(),
                    ParseDurationMinutes());
                SetPortalAdminAccess(ipRange, chkServerAdminAccess.Checked);
            });
        }

        protected void btnRefreshPolicies_Click(object sender, EventArgs e)
        {
            BindPolicies();
        }

        protected void chkPortalAdminAccess_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = sender as CheckBox;
                if (checkBox == null)
                    return;

                GridViewRow row = checkBox.NamingContainer as GridViewRow;
                if (row == null)
                    return;

                FCP.IpSecurityPolicyInfo policy = row.DataItem as FCP.IpSecurityPolicyInfo;
                if (policy == null)
                {
                    // On postback DataItem is not available; resolve from row index and current datasource.
                    policy = ResolvePolicyFromRow(row);
                }

                if (policy == null || !policy.IsWhitelist)
                    return;

                string ipRange = policy.IpRange?.Trim();
                if (string.IsNullOrWhiteSpace(ipRange))
                    return;

                SetPortalAdminAccess(ipRange, checkBox.Checked);
                BindPolicies();
                ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
            }
        }

        protected void gvPolicies_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int policyId))
                return;

            try
            {
                if (string.Equals(e.CommandName, "RemovePolicy", StringComparison.Ordinal))
                {
                    ES.Services.Users.RemoveIpSecurityPolicy(policyId);
                }
                else if (string.Equals(e.CommandName, "TogglePolicyState", StringComparison.Ordinal))
                {
                    FCP.IpSecurityPolicyInfo policy = ResolvePolicyById(policyId);
                    if (policy == null)
                        return;

                    bool nextState = !policy.IsActive;
                    ES.Services.Users.SetIpSecurityPolicyState(policyId, nextState);

                    if (!nextState && policy.IsWhitelist)
                        SetPortalAdminAccess(policy.IpRange?.Trim(), false);
                }
                else
                {
                    return;
                }

                NormalizePortalAllowlistAgainstPolicies();
                BindPolicies();
                ShowSuccessMessage("POLICY_UPDATED");
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        private void ExecutePolicyAction(Action action)
        {
            try
            {
                action();
                BindPolicies();
                ShowSuccessMessage("POLICY_UPDATED");
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        private int ParseDurationMinutes()
        {
            if (!int.TryParse(txtDurationMinutes.Text, out var minutes) || minutes < 0)
                return 0;
            return minutes;
        }

        private string GetSubmittedIpRange()
        {
            return txtIpRange.Text?.Trim();
        }

        private void gvPolicies_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            if (!(e.Row.DataItem is FCP.IpSecurityPolicyInfo policy))
                return;

            portalAdminAllowlist ??= GetPortalAdminAllowlist();

            if (e.Row.FindControl("lblPolicyType") is Label typeLabel)
            {
                bool isWhitelist = policy.IsWhitelist;
                typeLabel.Text = isWhitelist ? LocalizeOrDefault("PolicyType.Whitelist", "Whitelist") : LocalizeOrDefault("PolicyType.Block", "Block");
                typeLabel.CssClass = isWhitelist ? "badge bg-success" : "badge bg-danger";
            }

            if (e.Row.FindControl("lblPolicyStatus") is Label statusLabel)
            {
                statusLabel.Text = policy.IsActive
                    ? LocalizeOrDefault("PolicyStatus.Active", "Enabled")
                    : LocalizeOrDefault("PolicyStatus.Disabled", "Disabled");
                statusLabel.CssClass = policy.IsActive ? "badge bg-primary" : "badge bg-secondary";
            }

            if (e.Row.FindControl("btnTogglePolicyState") is LinkButton toggleButton)
            {
                string toggleToolTip = policy.IsActive
                    ? LocalizeOrDefault("PolicyState.Disable.ToolTip", "Disable this rule")
                    : LocalizeOrDefault("PolicyState.Enable.ToolTip", "Enable this rule");

                toggleButton.ToolTip = toggleToolTip;
                toggleButton.CssClass = policy.IsActive
                    ? "btn btn-sm btn-link p-0 text-decoration-none"
                    : "btn btn-sm btn-link p-0 text-decoration-none text-secondary";

                if (e.Row.FindControl("iconPolicyState") is HtmlGenericControl toggleIcon)
                    toggleIcon.Attributes["class"] = policy.IsActive ? "bi bi-eye-fill" : "bi bi-eye-slash-fill";
            }

            if (e.Row.FindControl("chkPortalAdminAccess") is CheckBox adminCheck)
            {
                bool isWhitelist = policy.IsWhitelist;
                adminCheck.Visible = isWhitelist;
                adminCheck.Enabled = isWhitelist && policy.IsActive;
                adminCheck.Checked = isWhitelist && policy.IsActive && portalAdminAllowlist.Contains(policy.IpRange ?? string.Empty);
                adminCheck.ToolTip = policy.IsActive
                    ? LocalizeOrDefault("ServerAdminAccess.Enabled.ToolTip", "Allow this whitelisted IP or subnet to access serveradmin-only portal pages.")
                    : LocalizeOrDefault("ServerAdminAccess.Disabled.ToolTip", "Enable this whitelist rule before granting serveradmin page access.");
            }

            if (e.Row.FindControl("btnRemovePolicy") is LinkButton removeButton)
            {
                removeButton.Text = LocalizeOrDefault("Action.Remove", "Remove");
                removeButton.ToolTip = policy.IsWhitelist
                    ? LocalizeOrDefault("Action.RemoveWhitelist.ToolTip", "Remove this whitelist rule")
                    : LocalizeOrDefault("Action.RemoveBlock.ToolTip", "Remove this block rule");
                removeButton.OnClientClick = policy.IsWhitelist
                    ? $"return confirm('{JavaScriptStringEncode(LocalizeOrDefault("Action.RemoveWhitelist.Confirm", "Remove this whitelist rule?"))}');"
                    : $"return confirm('{JavaScriptStringEncode(LocalizeOrDefault("Action.RemoveBlock.Confirm", "Remove this block rule?"))}');";
            }

            if (!policy.IsActive)
                e.Row.CssClass = string.Concat(e.Row.CssClass, " text-muted");
        }

        private static List<string> ParseIpEntries(string raw)
        {
            return (raw ?? string.Empty)
                .Split(new[] { ',', ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(entry => entry.Trim())
                .Where(entry => !string.IsNullOrWhiteSpace(entry))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private int SavePortalAccessIpSettings(string value)
        {
            FCP.SystemSettings settings = new FCP.SystemSettings();
            settings[FCP.SystemSettings.ACCESS_IPs] = value ?? string.Empty;
            return ES.Services.System.SetSystemSettings(FCP.SystemSettings.ACCESS_IP_SETTINGS, settings);
        }

        private void SetPortalAdminAccess(string ipRange, bool enabled)
        {
            if (string.IsNullOrWhiteSpace(ipRange))
                return;

            portalAdminAllowlist = GetPortalAdminAllowlist();
            if (enabled)
                portalAdminAllowlist.Add(ipRange);
            else
                portalAdminAllowlist.RemoveWhere(entry => string.Equals(entry, ipRange, StringComparison.OrdinalIgnoreCase));

            SavePortalAccessIpSettings(string.Join(",", portalAdminAllowlist.OrderBy(entry => entry, StringComparer.OrdinalIgnoreCase)));
        }

        private HashSet<string> GetPortalAdminAllowlist()
        {
            FCP.SystemSettings settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.ACCESS_IP_SETTINGS);
            string accessIps = settings?.GetValueOrDefault(FCP.SystemSettings.ACCESS_IPs, string.Empty) ?? string.Empty;
            return new HashSet<string>(ParseIpEntries(accessIps), StringComparer.OrdinalIgnoreCase);
        }

        private void NormalizePortalAllowlistAgainstPolicies()
        {
            HashSet<string> allowedEntries = GetPortalAdminAllowlist();
            if (allowedEntries.Count == 0)
                return;

            HashSet<string> activeWhitelistRanges = new HashSet<string>(
                (ES.Services.Users.GetIpSecurityPolicies(true, false, false) ?? Array.Empty<FCP.IpSecurityPolicyInfo>())
                    .Where(policy => policy.IsActive)
                    .Select(policy => (policy.IpRange ?? string.Empty).Trim())
                    .Where(value => !string.IsNullOrWhiteSpace(value)),
                StringComparer.OrdinalIgnoreCase);

            if (allowedEntries.RemoveWhere(entry => !activeWhitelistRanges.Contains(entry)) > 0)
            {
                SavePortalAccessIpSettings(string.Join(",", allowedEntries.OrderBy(entry => entry, StringComparer.OrdinalIgnoreCase)));
            }
        }

        private void BindPolicies()
        {
            try
            {
                NormalizePortalAllowlistAgainstPolicies();
                portalAdminAllowlist = GetPortalAdminAllowlist();
                var policies = ES.Services.Users.GetIpSecurityPolicies(false, false, true);
                gvPolicies.DataSource = policies;
                gvPolicies.DataBind();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
                DisableControls = true;
            }
        }

        private FCP.IpSecurityPolicyInfo ResolvePolicyFromRow(GridViewRow row)
        {
            if (row == null)
                return null;

            var policies = ES.Services.Users.GetIpSecurityPolicies(false, false, true) ?? Array.Empty<FCP.IpSecurityPolicyInfo>();
            if (row.RowIndex < 0 || row.RowIndex >= policies.Length)
                return null;

            return policies[row.RowIndex];
        }

        private FCP.IpSecurityPolicyInfo ResolvePolicyById(int policyId)
        {
            return (ES.Services.Users.GetIpSecurityPolicies(false, false, true) ?? Array.Empty<FCP.IpSecurityPolicyInfo>())
                .FirstOrDefault(policy => policy.Id == policyId);
        }

        private string LocalizeOrDefault(string key, string fallback)
        {
            return GetLocalizedString(key) ?? fallback;
        }

        private static string JavaScriptStringEncode(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", string.Empty)
                .Replace("\n", "\\n");
        }
    }
}
