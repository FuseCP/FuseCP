<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnableAsyncTasksSupport.ascx.cs" Inherits="FuseCP.Portal.EnableAsyncTasksSupport" %>
<%@ Import Namespace="FuseCP.Portal" %>

<asp:HiddenField ID="taskID" runat="server" />
<input type="hidden" id="taskConfig" data-complete-message="<%= GetLocalizedString("Text.CompleteMessage") %>" />
<script src="/DesktopModules/FuseCP/Scripts/async-tasks-config.js" type="text/javascript"></script>
<script src="<%= GetAjaxUtilsUrl() %>" type="text/javascript"></script>

<asp:Panel id="pnlModal" runat="server" CssClass="PopupContainer" style="display:none">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-cogs"></i> <asp:Label ID="lblTitle" runat="server" meta:resourcekey="lblTitle" Text="Running"></asp:Label></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
            <div class="ProgressPanelArea">
				<div class="MediumBold text-nowrap fcp-pr-45">
					<img id="imgAjaxIndicator" src='<%= PortalUtils.GetThemedImage("indicator_medium.gif") %>' class="align-middle" />&nbsp;
					<span id="objProgressDialogTitle"></span>
				</div>
			</div>
			<div id="ProgressPanelArea" class="ProgressPanelArea">
				<fieldset>
					<table class="table table-borderless align-middle mb-0 w-100">
						<tr>
							<td class="SubHead" colspan="2">
								<span id="objProgressDialogStep">Step</span>
							</td>
						</tr>
						<tr>
							<td colspan="2">
								<div class="ProgressBarContainer">
									<div id="objProgressDialogProgressBar" class="ProgressBarIndicator" ></div>
								</div>
							</td>
						</tr>
						<tr>
							<td class="SubHead text-nowrap">
								<asp:Label ID="lblStarted" runat="server" meta:resourcekey="lblStarted" Text="Started"></asp:Label>
							</td>
							<td class="Normal">
								<span id="progressStartTime"></span>
							</td>
						</tr>
						<tr>
							<td class="SubHead">
								<asp:Label ID="lblDuration" runat="server" meta:resourcekey="lblDuration" Text="Duration"></asp:Label>   
							</td>
							<td class="Normal">
								<span id="progressDuration"></span>
							</td>
						</tr>
					</table>
				</fieldset>
			</div>
            <div id="PopupFormFooter" class="FormFooter text-center" >
			    <div id="objProgressDialogCommandButtons">
				    <asp:LinkButton id="btnCancelProgressDialog" Text="  Close  " CssClass="btn btn-warning" meta:resourcekey="btnCancelProgressDialog" runat="server" />
			    </div>
			    <div id="objProgressDialogCloseButton" style="display: none">
				    <asp:LinkButton id="btnCloseProgressDialog" Text="  OK  " CssClass="btn btn-success" meta:resourcekey="btnCloseProgressDialog" runat="server" />
			    </div>
		    </div>
        </div>
    </div>
</asp:Panel>
<ajaxToolkit:ModalPopupExtender ID="ModalPopupProperties" runat="server"
	BehaviorID="ModalPopupProperties"
	TargetControlID="btnShowProgressDialog" PopupControlID="pnlModal"
    BackgroundCssClass="modalBackground" DropShadow="false"
    OkControlID="btnCloseProgressDialog" OnCancelScript="OnCancelProgressDialog()"
    CancelControlID="btnCancelProgressDialog" />
<asp:LinkButton id="btnShowProgressDialog" runat="server" Text="Progress" style="display:none" />

