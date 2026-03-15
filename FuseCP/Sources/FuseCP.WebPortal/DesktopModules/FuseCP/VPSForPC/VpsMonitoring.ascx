<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsMonitoring.ascx.cs" Inherits="FuseCP.Portal.VPSForPC.VpsMonitoring" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
<asp:Timer runat="server" Interval="180000" ID="operationTimer" />

<link type="text/css" href="/App_Themes/Default/Styles/jquery-ui-1.8.9.css" rel="stylesheet" />	
<link type="text/css" href="/App_Themes/Default/Styles/jquery.window.css" rel="stylesheet" />	

<script src="JavaScript/jquery.window.js" type="text/javascript"></script>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/vps-monitoring.js"></script>
        <asp:HiddenField ID="hItemId" runat="server"  />
	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="Monitoring48" runat="server" />
                    <fcp:FormTitle ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Monitoring" />
			    </div>
			    <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="" />
                 <div class="card tab-content">
                <div class="card-body form-horizontal">
                    <fcp:ServerTabs id="tabs" runat="server" SelectedTab="vps_monitoring" />	
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />

                    <h1>Monitoring</h1>
               <div id="testClass">
                    <div id="monitoringWrapper">
				        <asp:UpdatePanel runat="server" ID="UpdatePanelCharts" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="operationTimer" EventName="Tick" />
                            </Triggers>
                            <ContentTemplate>                
                            <asp:Literal ID="litProcessorChart" runat="server" />
                            <div>
                                <asp:LinkButton id="btnShowProcessorAsPanel" CssClass="btn btn-success" runat="server" OnClientClick="return FuseCPVpsMonitoring.showAsPanel('Processor','<%=hItemId.ClientID%>');"> <i class="bi bi-check-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnShowProcessorAsPanelText"/> </asp:LinkButton>
			                </div>
                            <asp:Literal ID="litNetworkChart" runat="server" />
                            <div>
                                <asp:LinkButton id="btnShowNetworkAsPanel" CssClass="btn btn-success" runat="server" OnClientClick="return FuseCPVpsMonitoring.showAsPanel('Network','<%=hItemId.ClientID%>');"> <i class="bi bi-check-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnShowNetworkAsPanelText"/> </asp:LinkButton>
			                </div>
                            <asp:Literal ID="litMemoryChart" runat="server" />
                            <div>
                                <asp:LinkButton id="btnShowMemoryAsPanel" CssClass="btn btn-success" runat="server" OnClientClick="return FuseCPVpsMonitoring.showAsPanel('Memory','<%=hItemId.ClientID%>');"> <i class="bi bi-check-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnShowMemoryAsPanelText"/> </asp:LinkButton>
			                </div>

                        </ContentTemplate>
                     </asp:UpdatePanel>
                            </div>
                       </div>
	                   <div class="Right">
		                <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
	                </div>
                    </div>
            </div>
                    </div>
            </div>
