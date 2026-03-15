<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OverusageReport.ascx.cs" Inherits="FuseCP.Portal.OverusageReport" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<%@ Register TagPrefix="fcp" TagName="CalendarControl" Src="UserControls/CalendarControl.ascx" %>

<!-- Our Toolbar -->
<div class="card-body form-horizontal">

	<!-- Bandwidth search criteria -->
	<fcp:CollapsiblePanel 
		ID="bandwidthCollapsiblePanel" runat="server"
		TargetControlID="bandwidthSearchCriteria"
		Text="Bandwidth Search Criteria" resourceKey="bandwidthCollapsiblePanel"
		IsCollapsed="true"
		>
	</fcp:CollapsiblePanel>
	<asp:Panel ID="bandwidthSearchCriteria" runat="server" Height="0" style="overflow:hidden">
		<div class="fcp-ms-5pt">
			<p>
				<asp:Label
					ID="bandwidthCaption" runat="server"
					Text="Choose either a time frame or a month to see the bandwidth overusage for." meta:resourcekey="bandwidthCaption"
					/>
			</p>
			<table class="table table-borderless align-middle mb-0 w-100">
				<tr>
					<td class="Normal align-top">
						<table>
							<tr>
								<td class="Normal">
									<asp:Label ID="startDateLabel" runat="server" Text="Start date:" meta:resourcekey="startDateLabel" />
								</td>
								<td class="Normal">
									<fcp:CalendarControl ID="startDateCalendar" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="Normal">
									<asp:Label ID="endDateLabel" runat="server" Text="End date:" meta:resourcekey="endDateLabel" />
								</td>
								<td class="Normal">
									<fcp:CalendarControl ID="endDateCalendar" runat="server" />
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
	</asp:Panel>
	
	
	<!-- Export functionality -->
	<fcp:CollapsiblePanel
		ID="exportCollapsiblePanel" runat="server"
		TargetControlID="exportPanel"
		Text="Export" resourceKey="exportCollapsiblePanel"
		IsCollapsed="true"
		>
	</fcp:CollapsiblePanel>
	<asp:Panel ID="exportPanel" runat="server" Height="0" style="overflow:hidden">
		<div class="fcp-ms-5pt">
			<asp:HyperLink 
				ID="exportToExcel" runat="server"
				Text="Export to Microsoft Excel" meta:resourcekey="exportToExcel"
				NavigateUrl="#" Target="_blank"
				SkinID="CommandButton"
			/>
			&nbsp;
			<asp:HyperLink 
				ID="exportToPdf" runat="server"
				Text="Export to Acrobat Reader (PDF)" meta:resourcekey="exportToPdf"
				NavigateUrl="#" Target="_blank"
				SkinID="CommandButton"
			/>
		</div>
	</asp:Panel>
</div>
<div class="FormButtonsBar">
	<asp:LinkButton id="refreshButton" CssClass="btn btn-success" runat="server" OnClick="OnRefreshButtonClick"> <i class="bi bi-arrow-clockwise">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="refreshButtonText"/> </asp:LinkButton>
</div>

<asp:Panel ID="pnlSummary" runat="server" CssClass="mt-3">
	<h4><asp:Localize runat="server" meta:resourcekey="OverusageReport.DiskspaceLabel" Text="Diskspace Overusage Report" /></h4>
	<asp:GridView ID="gvDiskSummary" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="false" OnRowCommand="OnDiskDetailsRowCommand">
		<Columns>
			<asp:BoundField DataField="HostingSpaceName" HeaderText="Hosting Space" />
			<asp:BoundField DataField="UserName" HeaderText="User" />
			<asp:BoundField DataField="Status" HeaderText="Status" />
			<asp:BoundField DataField="Allocated" HeaderText="Allocated" />
			<asp:BoundField DataField="Used" HeaderText="Used" />
			<asp:BoundField DataField="Overusage" HeaderText="Overusage" />
			<asp:TemplateField HeaderText="Details">
				<ItemTemplate>
					<asp:LinkButton ID="lnkDiskDetails" runat="server" Text="Details" CommandName="Details" CommandArgument='<%# Eval("HostingSpaceId") %>' />
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>

	<h4 class="mt-3"><asp:Localize runat="server" meta:resourcekey="OverusageReport.BandwidthLabel" Text="Bandwidth Overusage Report" /></h4>
	<asp:GridView ID="gvBandwidthSummary" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="false" OnRowCommand="OnBandwidthDetailsRowCommand">
		<Columns>
			<asp:BoundField DataField="HostingSpaceName" HeaderText="Hosting Space" />
			<asp:BoundField DataField="UserName" HeaderText="User" />
			<asp:BoundField DataField="Status" HeaderText="Status" />
			<asp:BoundField DataField="Allocated" HeaderText="Allocated" />
			<asp:BoundField DataField="Used" HeaderText="Used" />
			<asp:BoundField DataField="Overusage" HeaderText="Overusage" />
			<asp:TemplateField HeaderText="Details">
				<ItemTemplate>
					<asp:LinkButton ID="lnkBandwidthDetails" runat="server" Text="Details" CommandName="Details" CommandArgument='<%# Eval("HostingSpaceId") %>' />
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>
</asp:Panel>

<asp:Panel ID="pnlDetails" runat="server" Visible="false" CssClass="mt-3">
	<div class="d-flex justify-content-between align-items-center">
		<h4><asp:Label ID="lblDetailsTitle" runat="server" Text="Overusage Details" /></h4>
		<asp:LinkButton ID="btnBackToSummary" runat="server" CssClass="btn btn-outline-secondary" OnClick="OnBackToSummaryClick">Back to summary</asp:LinkButton>
	</div>
	<asp:GridView ID="gvDetails" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="true" />
</asp:Panel>
