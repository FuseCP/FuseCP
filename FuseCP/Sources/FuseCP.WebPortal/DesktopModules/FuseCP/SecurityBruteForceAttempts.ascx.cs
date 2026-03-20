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
using System.Net;
using System.Web.UI.WebControls;

namespace FuseCP.Portal
{
    public partial class SecurityBruteForceAttempts : FuseCPModuleBase
    {
        private const int PageSize = 100;
        private const int ServiceBatchSize = 500;
        private const int MaxScannedRows = 5000;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCurrentPage(0);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            BindCurrentPage(0);
        }

        protected void btnPreviousPage_Click(object sender, EventArgs e)
        {
            var previousPage = Math.Max(0, GetCurrentPageIndex() - 1);
            BindCurrentPage(previousPage);
        }

        protected void btnNextPage_Click(object sender, EventArgs e)
        {
            var nextPage = GetCurrentPageIndex() + 1;
            BindCurrentPage(nextPage);
        }

        private bool GetFailedOnlyValue()
        {
            var postedValue = GetPostedResultFilterValue();
            var selectedValue = string.IsNullOrWhiteSpace(postedValue)
                ? ddlResultFilter.SelectedValue
                : postedValue;

            var failedOnly = !string.Equals(selectedValue, "all", StringComparison.OrdinalIgnoreCase);
            ddlResultFilter.SelectedValue = failedOnly ? "failed" : "all";
            return failedOnly;
        }

        private string GetPostedResultFilterValue()
        {
            return GetPostedValue(ddlResultFilter.ID, ddlResultFilter.UniqueID, ddlResultFilter.ClientID);
        }

        private string GetCurrentIpFilter()
        {
            var posted = GetPostedValue(txtIpFilter.ID, txtIpFilter.UniqueID, txtIpFilter.ClientID);
            var value = string.IsNullOrWhiteSpace(posted) ? txtIpFilter.Text : posted;
            var normalized = value?.Trim() ?? string.Empty;
            txtIpFilter.Text = normalized;
            return normalized;
        }

        private string GetCurrentLayerFilter()
        {
            var posted = GetPostedValue(ddlLayer.ID, ddlLayer.UniqueID, ddlLayer.ClientID);
            var value = string.IsNullOrWhiteSpace(posted) ? ddlLayer.SelectedValue : posted;
            if (ddlLayer.Items.FindByValue(value) != null)
                ddlLayer.SelectedValue = value;
            return ddlLayer.SelectedValue;
        }

        private string GetCurrentDateTimeValue(TextBox textBox)
        {
            var posted = GetPostedValue(textBox.ID, textBox.UniqueID, textBox.ClientID);
            var value = string.IsNullOrWhiteSpace(posted) ? textBox.Text : posted;
            textBox.Text = value?.Trim() ?? string.Empty;
            return textBox.Text;
        }

        private DateTime? BuildUtcDateTime(string dateTimeValue)
        {
            if (string.IsNullOrWhiteSpace(dateTimeValue))
                return null;

            if (!DateTime.TryParse(dateTimeValue, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var localDateTime))
            {
                return null;
            }

            return DateTime.SpecifyKind(localDateTime, DateTimeKind.Utc);
        }

        private void GetCurrentDateRange(out DateTime? fromUtc, out DateTime? toUtc)
        {
            var fromText = GetCurrentDateTimeValue(txtFromUtc);
            var toText = GetCurrentDateTimeValue(txtToUtc);

            fromUtc = BuildUtcDateTime(fromText);
            toUtc = BuildUtcDateTime(toText);

            if (fromUtc.HasValue && toUtc.HasValue && fromUtc.Value > toUtc.Value)
            {
                var swap = fromUtc;
                fromUtc = toUtc;
                toUtc = swap;
            }
        }

        private int GetCurrentPageIndex()
        {
            var raw = ViewState["BruteForcePageIndex"];
            if (raw is int pageIndex && pageIndex >= 0)
                return pageIndex;

            return 0;
        }

        private void BindCurrentPage(int pageIndex)
        {
            var normalizedPage = Math.Max(0, pageIndex);
            ViewState["BruteForcePageIndex"] = normalizedPage;

            GetCurrentDateRange(out var fromUtc, out var toUtc);

            BindAttempts(
                GetCurrentIpFilter(),
                GetCurrentLayerFilter(),
                GetFailedOnlyValue(),
                fromUtc,
                toUtc,
                normalizedPage);
        }

        private string GetPostedValue(string id, string uniqueId, string clientId)
        {
            var form = Request?.Form;
            if (form == null || form.Count == 0)
                return null;

            var exact = form[uniqueId] ?? form[clientId] ?? form[id];
            if (!string.IsNullOrWhiteSpace(exact))
                return exact;

            foreach (var key in form.AllKeys)
            {
                if (string.IsNullOrEmpty(key))
                    continue;

                if (key.Equals(id, StringComparison.OrdinalIgnoreCase) ||
                    key.EndsWith("$" + id, StringComparison.OrdinalIgnoreCase) ||
                    key.EndsWith(":" + id, StringComparison.OrdinalIgnoreCase))
                {
                    var value = form[key];
                    if (!string.IsNullOrWhiteSpace(value))
                        return value;
                }
            }

            return null;
        }

        private void BindAttempts(string ipFilter, string layerFilter, bool failedOnly,
            DateTime? fromUtc, DateTime? toUtc, int pageIndex)
        {
            try
            {
                var targetSkip = pageIndex * PageSize;
                var targetTake = PageSize + 1;
                var filteredAttempts = new List<FuseCP.EnterpriseServer.BruteForceAttemptInfo>(targetSkip + targetTake);

                for (var skip = 0; skip < MaxScannedRows && filteredAttempts.Count < targetSkip + targetTake; skip += ServiceBatchSize)
                {
                    var batch = ES.Services.Users.GetBruteForceAttempts(
                        ipFilter,
                        layerFilter,
                        failedOnly,
                        skip,
                        ServiceBatchSize);

                    if (batch == null || batch.Length == 0)
                        break;

                    var rangeFiltered = batch.Where(a =>
                        (!fromUtc.HasValue || a.AttemptTime >= fromUtc.Value) &&
                        (!toUtc.HasValue || a.AttemptTime <= toUtc.Value));

                    filteredAttempts.AddRange(rangeFiltered);

                    if (batch.Length < ServiceBatchSize)
                        break;
                }

                var pageRows = filteredAttempts.Skip(targetSkip).Take(PageSize).ToArray();
                var hasNextPage = filteredAttempts.Count > targetSkip + PageSize;

                if (pageRows.Length == 0 && pageIndex > 0)
                {
                    BindCurrentPage(pageIndex - 1);
                    return;
                }

                gvAttempts.DataSource = pageRows;
                gvAttempts.DataBind();

                btnPreviousPage.Enabled = pageIndex > 0;
                btnNextPage.Enabled = hasNextPage;
                lblPageInfo.Text = $"Page {pageIndex + 1} ({PageSize} rows per page)";
            }
            catch (Exception ex)
            {
                ProcessException(ex);
                DisableControls = true;
            }
        }

        protected void gvAttempts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "BlockIp", StringComparison.Ordinal) &&
                !string.Equals(e.CommandName, "WhitelistIp", StringComparison.Ordinal))
            {
                return;
            }

            var ipAddress = Convert.ToString(e.CommandArgument)?.Trim();
            if (string.IsNullOrWhiteSpace(ipAddress) || !IPAddress.TryParse(ipAddress, out _))
            {
                ShowErrorMessage("IP address is missing or invalid.");
                return;
            }

            try
            {
                var reason = string.Equals(e.CommandName, "BlockIp", StringComparison.Ordinal)
                    ? "Manual block from Brute Force Attempts"
                    : "Manual whitelist from Brute Force Attempts";

                if (string.Equals(e.CommandName, "BlockIp", StringComparison.Ordinal))
                    ES.Services.Users.BlockIpAddress(ipAddress, reason, 0);
                else
                    ES.Services.Users.WhitelistIpAddress(ipAddress, reason, 0);

                BindCurrentPage(GetCurrentPageIndex());
                ShowSuccessMessage("SYSTEM_SETTINGS_SAVE");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("SYSTEM_SETTINGS_SAVE", ex);
            }
        }
    }
}
