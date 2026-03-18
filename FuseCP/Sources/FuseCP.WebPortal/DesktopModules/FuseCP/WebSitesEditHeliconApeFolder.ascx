<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebSitesEditHeliconApeFolder.ascx.cs"
	Inherits="FuseCP.Portal.WebSitesEditHeliconApeFolder" %>
<%@ Register Src="UserControls/FileLookup.ascx" TagName="FileLookup" TagPrefix="uc1" %>
<%@ Register Src="UserControls/PopupHeader.ascx" TagName="PopupHeader" TagPrefix="fcp" %>
<link rel="stylesheet" href="/JavaScript/codemirror/codemirror.css" />
<script type="text/javascript" src="/JavaScript/jquery-1.4.4.min.js"></script>
<script type="text/javascript" src="/JavaScript/codemirror/codemirror.js"></script>
<script type="text/javascript" src="/JavaScript/codemirror/htaccess.js"></script>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/websites-helicon-ape-folder.js"></script>
<style type="text/css">
.CodeMirror {
    border: 1px solid #444;
    padding: 2px;
    font-family: Consolas, monospace;
    font-size: 14px;
}
</style>
<div class="card-body form-horizontal">
<table class="table table-borderless align-middle mb-0 w-100">
	<tr>
		<td class="SubHead text-nowrap" >
			<asp:Label ID="lblFolderName" runat="server" meta:resourcekey="lblFolderName"></asp:Label>
		</td>
		<td class="NormalBold text-nowrap" >
		    <asp:Label runat="server" ID="LabelWebSiteName"></asp:Label>

			<uc1:FileLookup id="folderPath" runat="server">
			</uc1:FileLookup>
			<asp:HiddenField ID="contentPath" runat="server" />
			<asp:HiddenField ID="DebuggerUrlField" runat="server" />
		</td>
        <td >
            <asp:LinkButton id="ButtonDebuggerStop" CssClass="btn btn-warning" runat="server" OnClick="DebugStopClick"> <i class="bi bi-stop-circle">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnApeDebuggerStopText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="ButtonDebuggerStart" CssClass="btn btn-success" runat="server" OnClick="DebugStartClick"> <i class="bi bi-play-circle">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnApeDebuggerStartText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="BUttonShowDebuggingPageLinkModal" CssClass="btn btn-success Hidden" runat="server"> <i class="bi bi-check-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnShowDebuggingPageLinkModalText"/> </asp:LinkButton>
        </td>
	</tr>
</table>

<asp:Panel runat="server" ID="DebuggerFramePanel" Visible="False">
    <iframe runat="server" ID="DebuggerFrame" height="400px"></iframe>
</asp:Panel>


<asp:TextBox ID="htaccessContent" runat="server" TextMode="MultiLine" CssClass="CodeEditor"></asp:TextBox>

</div>

<div class="card-footer text-end">
	<asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" > <i class="bi bi-arrow-clockwise">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnUpdate"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="BtnCancelClick"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click"> <i class="bi bi-floppy">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnSave"/> </asp:LinkButton>
</div>

<asp:Panel ID="DebuggingPageLinkPanel" runat="server" CssClass="PopupContainer" style="display:none" DefaultButton="btnCancelDebuggingPageLinkPanel">
    <div class="widget">
        <div class="widget-content Popup">
        <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom">
            <h3 class="m-0 fs-6 text-secondary"><i class="bi bi-list"></i> <fcp:PopupHeader runat="server" meta:resourcekey="lblDebuggingPageLink" Text="Debugging Page Link" /></h3>
            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="$find(this).get_element().closest('.PopupContainer').style.display='none'; return false;">
                <i class="bi bi-x-lg" aria-hidden="true"></i>
                <span class="ms-1">Close</span>
            </button>
        </div>
			<div class="FormRow">
				<asp:Label ID="LabelClickLink" runat="server" meta:resourcekey="lblCLickLink" Text="Click this link to open debugging page"></asp:Label>:
                <br/>
                <br/>
				<asp:HyperLink runat="server" ID="LinkDebuggingPage" Target="ape-debugging-page" CssClass="LinkDebuggingPage"></asp:HyperLink>
			</div>
			</div>
					<div class="popup-buttons text-end">
            <asp:LinkButton id="btnCancelDebuggingPageLinkPanel" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCloseText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="DebuggingPageLinkModal" runat="server"
    PopupControlID="DebuggingPageLinkPanel" TargetControlID="BUttonShowDebuggingPageLinkModal"
    BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelDebuggingPageLinkPanel" />


