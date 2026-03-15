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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FuseCP.EnterpriseServer;
using FuseCP.Portal;
using System.Text;
using Microsoft.Reporting.NETCore;

namespace FuseCP.Portal
{
	public partial class OverusageReport : FuseCPModuleBase
	{
		const string ViewModeSummary = "summary";
		const string ViewModeDisk = "disk";
		const string ViewModeBandwidth = "bandwidth";

		const string OverusageReportName = "OverusageReport";
		const string DiskspaceDetailsReportName = "HostingSpaceDiskspaceOverusageDetails";
		const string BandwidthDetailsReportName = "HostingSpaceBandwidthOverusageDetails";

		const string QueryAction = "action";
		const string QueryFormat = "format";
		const string QueryView = "view";
		const string QuerySpace = "spaceId";
		const string QueryStart = "start";
		const string QueryEnd = "end";

		string CurrentView
		{
			get => ViewState[nameof(CurrentView)] as string ?? ViewModeSummary;
			set => ViewState[nameof(CurrentView)] = value;
		}

		int? CurrentSpaceId
		{
			get => ViewState[nameof(CurrentSpaceId)] as int?;
			set => ViewState[nameof(CurrentSpaceId)] = value;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (TryHandleExportRequest())
				{
					return;
				}

				if (!IsPostBack)
				{
					BindToolbar();
					CurrentView = ViewModeSummary;
					CurrentSpaceId = null;
					BindCurrentView();
					BindExportButtons();
				}
			}
			catch (Exception ex)
			{
				ShowWarningMessage(ex.Message);
			}
		}

		/// <summary>
		/// Initiates report refresh with changes parameters from toolbar.
		/// </summary>
		/// <param name="sender">Report refresh button</param>
		/// <param name="e">Corresponding event arguments</param>
		protected void OnRefreshButtonClick(object sender, EventArgs e)
		{
			DateTime startDate = startDateCalendar.SelectedDate;
			DateTime endDate = endDateCalendar.SelectedDate;
			if (startDate > endDate)
            {
                ShowWarningMessage("START_END_DATE_VALIDATION");
                return;
            }

			CurrentView = ViewModeSummary;
			CurrentSpaceId = null;
			BindCurrentView();
			BindExportButtons();
		}

		protected void OnBackToSummaryClick(object sender, EventArgs e)
		{
			CurrentView = ViewModeSummary;
			CurrentSpaceId = null;
			BindCurrentView();
			BindExportButtons();
		}

		protected void OnDiskDetailsRowCommand(object sender, GridViewCommandEventArgs e)
		{
			OpenDetailsFromCommand(e, ViewModeDisk);
		}

		protected void OnBandwidthDetailsRowCommand(object sender, GridViewCommandEventArgs e)
		{
			OpenDetailsFromCommand(e, ViewModeBandwidth);
		}

		void OpenDetailsFromCommand(GridViewCommandEventArgs e, string detailsView)
		{
			if (e.CommandName != "Details")
			{
				return;
			}

			if (int.TryParse(Convert.ToString(e.CommandArgument), out int hostingSpaceId))
			{
				CurrentView = detailsView;
				CurrentSpaceId = hostingSpaceId;
				BindCurrentView();
				BindExportButtons();
			}
		}

		protected void BindToolbar()
		{
			startDateCalendar.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
			endDateCalendar.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
		}

		void BindCurrentView()
		{
			if (CurrentView == ViewModeSummary)
			{
				BindSummaryView();
			}
			else if (CurrentView == ViewModeDisk)
			{
				BindDiskDetailsView();
			}
			else
			{
				BindBandwidthDetailsView();
			}
		}

		void BindSummaryView()
		{
			var summary = ES.Services.Packages
				.GetOverusageSummaryReport(
					PanelSecurity.SelectedUserId,
					PanelSecurity.PackageId,
					startDateCalendar.SelectedDate,
					endDateCalendar.SelectedDate);

			TranslateStatusField(summary.Tables["HostingSpace"]);
			gvDiskSummary.DataSource = BuildSummaryOverusageTable(summary.Tables["HostingSpace"], summary.Tables["DiskspaceOverusage"]);
			gvDiskSummary.DataBind();

			gvBandwidthSummary.DataSource = BuildSummaryOverusageTable(summary.Tables["HostingSpace"], summary.Tables["BandwidthOverusage"]);
			gvBandwidthSummary.DataBind();

			pnlSummary.Visible = true;
			pnlDetails.Visible = false;
		}

		void BindDiskDetailsView()
		{
			if (!CurrentSpaceId.HasValue)
			{
				CurrentView = ViewModeSummary;
				BindSummaryView();
				return;
			}

			var details = ES.Services.Packages.GetDiskspaceOverusageDetailsReport(PanelSecurity.SelectedUserId, CurrentSpaceId.Value);
			TranslateStatusField(details.Tables["HostingSpace"]);

			lblDetailsTitle.Text = GetLocalizedString("OverusageReport.DiskspaceLabel") ?? "Diskspace Overusage Report";
			gvDetails.DataSource = details.Tables["OverusageDetails"];
			gvDetails.DataBind();

			pnlSummary.Visible = false;
			pnlDetails.Visible = true;
		}

		void BindBandwidthDetailsView()
		{
			if (!CurrentSpaceId.HasValue)
		{
				CurrentView = ViewModeSummary;
				BindSummaryView();
				return;
		}

			var details = ES.Services.Packages.GetBandwidthOverusageDetailsReport(
				PanelSecurity.SelectedUserId,
				CurrentSpaceId.Value,
				startDateCalendar.SelectedDate,
				endDateCalendar.SelectedDate);
			TranslateStatusField(details.Tables["HostingSpace"]);

			lblDetailsTitle.Text = GetLocalizedString("OverusageReport.BandwidthLabel") ?? "Bandwidth Overusage Report";
			gvDetails.DataSource = details.Tables["OverusageDetails"];
			gvDetails.DataBind();

			pnlSummary.Visible = false;
			pnlDetails.Visible = true;
		}

		void BindExportButtons()
		{
			exportToExcel.NavigateUrl = BuildExportUrl("excel");
			exportToPdf.NavigateUrl = BuildExportUrl("pdf");
		}

		string BuildExportUrl(string format)
		{
			var sb = new StringBuilder();
			sb.Append(Request.Path);
			sb.Append("?");
			sb.Append(QueryAction);
			sb.Append("=export&");
			sb.Append(QueryFormat);
			sb.Append("=");
			sb.Append(format);
			sb.Append("&");
			sb.Append(QueryView);
			sb.Append("=");
			sb.Append(CurrentView);

			if (CurrentSpaceId.HasValue)
			{
				sb.Append("&");
				sb.Append(QuerySpace);
				sb.Append("=");
				sb.Append(CurrentSpaceId.Value);
			}

			sb.Append("&");
			sb.Append(QueryStart);
			sb.Append("=");
			sb.Append(startDateCalendar.SelectedDate.ToString("yyyy-MM-dd"));

			sb.Append("&");
			sb.Append(QueryEnd);
			sb.Append("=");
			sb.Append(endDateCalendar.SelectedDate.ToString("yyyy-MM-dd"));

			return sb.ToString();
		}

		bool TryHandleExportRequest()
		{
			if (!string.Equals(Request.QueryString[QueryAction], "export", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			if (!DateTime.TryParse(Request.QueryString[QueryStart], out DateTime startDate))
			{
				startDate = DateTime.Today.AddDays(-30);
			}
			if (!DateTime.TryParse(Request.QueryString[QueryEnd], out DateTime endDate))
			{
				endDate = DateTime.Today;
			}

			string view = Request.QueryString[QueryView] ?? ViewModeSummary;
			int.TryParse(Request.QueryString[QuerySpace], out int hostingSpaceId);

			string format = Request.QueryString[QueryFormat];
			string reportName = view == ViewModeDisk ? DiskspaceDetailsReportName :
				view == ViewModeBandwidth ? BandwidthDetailsReportName : OverusageReportName;

			var reportData = BuildReportData(reportName, hostingSpaceId, startDate, endDate);
			var rendered = RenderReport(reportName, reportData, startDate, endDate, format);

			Response.Clear();
			Response.ContentType = rendered.ContentType;
			Response.AddHeader("Content-Disposition", $"attachment; filename={rendered.FileName}");
			Response.BinaryWrite(rendered.Bytes);
			Response.End();
			return true;
		}

		protected string GetReportFullPath(string reportName)
		{
			return HttpContext.Current.Server.MapPath(
				String.Format("~/DesktopModules/FuseCP/Reports/{0}.rdlc", reportName)
				);
		}

		(DataSet ReportData, DataTable HostingSpace, DataTable Overusage, DataTable OverusageDetails) BuildReportData(string reportName, int hostingSpaceId, DateTime startDate, DateTime endDate)
		{
			if (reportName == OverusageReportName)
			{
				var report = ES.Services.Packages.GetOverusageSummaryReport(PanelSecurity.SelectedUserId, PanelSecurity.PackageId, startDate, endDate);
				TranslateStatusField(report.Tables["HostingSpace"]);
				return (report, report.Tables["HostingSpace"], null, report.Tables["OverusageDetails"]);
			}

			if (reportName == DiskspaceDetailsReportName)
			{
				var report = ES.Services.Packages.GetDiskspaceOverusageDetailsReport(PanelSecurity.SelectedUserId, hostingSpaceId);
				TranslateStatusField(report.Tables["HostingSpace"]);
				return (report, report.Tables["HostingSpace"], report.Tables["DiskspaceOverusage"], report.Tables["OverusageDetails"]);
			}

			var bwReport = ES.Services.Packages.GetBandwidthOverusageDetailsReport(PanelSecurity.SelectedUserId, hostingSpaceId, startDate, endDate);
			TranslateStatusField(bwReport.Tables["HostingSpace"]);
			return (bwReport, bwReport.Tables["HostingSpace"], bwReport.Tables["BandwidthOverusage"], bwReport.Tables["OverusageDetails"]);
		}

		(byte[] Bytes, string ContentType, string FileName) RenderReport(string reportName, (DataSet ReportData, DataTable HostingSpace, DataTable Overusage, DataTable OverusageDetails) reportData, DateTime startDate, DateTime endDate, string format)
		{
			var report = new LocalReport();
			using (var reportDefinition = System.IO.File.OpenRead(GetReportFullPath(reportName)))
			{
				report.LoadReportDefinition(reportDefinition);
			}

			report.DataSources.Add(new ReportDataSource("OverusageReport_HostingSpace", reportData.HostingSpace));
			if (reportData.ReportData.Tables.Contains("DiskspaceOverusage"))
			{
				report.DataSources.Add(new ReportDataSource("OverusageReport_DiskspaceOverusage", reportData.ReportData.Tables["DiskspaceOverusage"]));
			}
			if (reportData.ReportData.Tables.Contains("BandwidthOverusage"))
			{
				report.DataSources.Add(new ReportDataSource("OverusageReport_BandwidthOverusage", reportData.ReportData.Tables["BandwidthOverusage"]));
			}
			report.DataSources.Add(new ReportDataSource("OverusageReport_OverusageDetails", reportData.OverusageDetails));

			report.SetParameters(new[]
			{
				new ReportParameter("BandwidthStartDate", startDate.ToString()),
				new ReportParameter("BandwidthEndDate", endDate.ToString())
			});

			string renderFormat = string.Equals(format, "pdf", StringComparison.OrdinalIgnoreCase) ? "PDF" : "EXCELOPENXML";
			string mimeType;
			string encoding;
			string fileExtension;
			Warning[] warnings;
			string[] streams;
			var bytes = report.Render(renderFormat, null, out mimeType, out encoding, out fileExtension, out streams, out warnings);

			string fileName = $"{reportName}-{DateTime.UtcNow:yyyyMMddHHmmss}.{fileExtension}";
			return (bytes, mimeType, fileName);
		}

		DataTable BuildSummaryOverusageTable(DataTable hostingSpace, DataTable overusage)
		{
			var table = new DataTable();
			table.Columns.Add("HostingSpaceId", typeof(int));
			table.Columns.Add("HostingSpaceName", typeof(string));
			table.Columns.Add("UserName", typeof(string));
			table.Columns.Add("Status", typeof(string));
			table.Columns.Add("Allocated", typeof(decimal));
			table.Columns.Add("Used", typeof(decimal));
			table.Columns.Add("Overusage", typeof(decimal));

			foreach (DataRow row in overusage.Rows)
			{
				int hostingSpaceId = Convert.ToInt32(row["HostingSpaceId"]);
				var hostingRows = hostingSpace.Select($"HostingSpaceId={hostingSpaceId}");
				var hosting = hostingRows.Length > 0 ? hostingRows[0] : null;

				decimal allocated = Convert.ToDecimal(row["Allocated"]);
				decimal used = Convert.ToDecimal(row["Used"]);
				decimal over = used - allocated;

				table.Rows.Add(
					hostingSpaceId,
					hosting?["HostingSpaceName"]?.ToString() ?? string.Empty,
					hosting?["UserName"]?.ToString() ?? string.Empty,
					hosting?["Status"]?.ToString() ?? string.Empty,
					allocated,
					used,
					over);
			}

			return table;
		}

		protected void TranslateStatusField(DataTable dt)
		{
			if (dt == null || !dt.Columns.Contains("Status"))
			{
				return;
			}

			foreach (DataRow row in dt.Rows)
			{
				int statusId = 0;
				if (int.TryParse(row["Status"].ToString(), out statusId))
				{
					row["Status"] = PanelFormatter.GetPackageStatusName(statusId);
				}
			}
		}
	}
}
