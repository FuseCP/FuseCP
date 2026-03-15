<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonitoringPage.aspx.cs"
	Inherits="FuseCP.Portal.VPSForPC.MonitoringPage" %>

<%@ Register Src="../UserControls/MessageBox.ascx" TagName="MessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox"
	TagPrefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title></title>
	<link href="/App_Themes/Default/Styles/jquery-ui-1.8.9.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<form id="form1" runat="server" style="height: 700px">
	<asp:ScriptManager ID="scriptManager" runat="server" EnablePartialRendering="true"
		EnableScriptGlobalization="true" EnableScriptLocalization="true">
	</asp:ScriptManager>
	<asp:Timer runat="server" Interval="10000" ID="operationTimer" OnTick="operationTimer_Tick" />
	<div id="testClass" style="height: 700px">
		<table  class="table table-sm">
			<tr>
				<td style="padding:3px">
					<asp:Label ID="lblStartPeriod" runat="server" AssociatedControlID="txtStartPeriod"
						meta:resourcekey="lblStartPeriod" Text="Start day" CssClass="MediumBold" />
				</td>
				<td style="padding:3px">
					<asp:TextBox ID="txtStartPeriod" runat="server" CssClass="form-control txtDateTimePeriod"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td style="padding:3px">
					<asp:Label ID="lblEndPeriod" runat="server" AssociatedControlID="txtEndPeriod" meta:resourcekey="lblEndPeriod"
						Text="End day" CssClass="MediumBold" />
				</td>
				<td style="padding:3px">
					<asp:TextBox ID="txtEndPeriod" runat="server" CssClass="form-control txtDateTimePeriod"></asp:TextBox>
				</td>
			</tr>
		</table>
		<asp:UpdatePanel runat="server" ID="UpdatePanelCharts" UpdateMode="Conditional">
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="operationTimer" EventName="Tick" />
			</Triggers>
			<ContentTemplate>
				<div id="monitoringWrapper">
					<asp:Literal ID="litCounterChart" runat="server" />
				</div>
			</ContentTemplate>
		</asp:UpdatePanel>
	</div>
	</form>
	<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/vps-monitoring.js"></script>
</body>
</html>
