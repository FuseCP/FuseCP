<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserAccountChangePassword.ascx.cs"
    Inherits="FuseCP.Portal.UserAccountChangePassword" %>
<%@ Register Src="UserControls/PasswordControl.ascx" TagName="PasswordControl" TagPrefix="fcp" %>
<asp:Panel ID="PasswordPanel" runat="server" DefaultButton="cmdChangePassword">
    <div class="card-body form-horizontal fcp-form-sheet">
        <div class="row mb-4 align-items-center">
            <asp:Label ID="lblUsername2" runat="server" meta:resourcekey="lblUsername" Text="Username:" CssClass="col-sm-2 col-form-label fcp-form-label"></asp:Label>
            <div class="col-sm-10">
                <div class="fcp-readonly-field fcp-readonly-field-hero"><asp:Literal ID="lblUsername" runat="server"></asp:Literal></div>
            </div>
        </div>
        <fcp:PasswordControl ID="userPassword" runat="server" />
        <div class="row mb-3">
            <div class="col-sm-10 offset-sm-2">
                <asp:ValidationSummary ID="valSummaryNewPassword" runat="server" ValidationGroup="NewPassword" ShowSummary="true" ShowMessageBox="false" CssClass="text-danger" />
            </div>
        </div>
        <div id="trChangePasswordWarning" runat="server" visible="false" class="row mb-0">
            <div class="col-sm-10 offset-sm-2">
                <asp:Label ID="lblChangePasswordWarning" runat="server" CssClass="fcp-inline-warning">Warning: This will end the current session.</asp:Label>
            </div>
        </div>
    </div>
    <div class="card-footer text-end">
        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
        <asp:LinkButton id="cmdChangePassword" CssClass="btn btn-success" runat="server" OnClick="cmdChangePassword_Click" ValidationGroup="NewPassword"> <i class="bi bi-check-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="cmdChangePassword"/> </asp:LinkButton>
    </div>
</asp:Panel>
