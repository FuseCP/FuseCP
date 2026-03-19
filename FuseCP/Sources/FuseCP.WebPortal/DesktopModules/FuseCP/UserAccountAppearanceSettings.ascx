<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserAccountAppearanceSettings.ascx.cs" Inherits="FuseCP.Portal.UserAccountAppearanceSettings" %>
<div class="card-body form-horizontal">
    <ul class="LinksList mb-0">
        <li>
            <asp:HyperLink ID="lnkAppearanceBranding" runat="server"
                Text="Appearance and Branding" NavigateUrl='<%# GetSettingsLink("FuseCPPolicy", "SettingsAppearanceBranding") %>'></asp:HyperLink>
        </li>
    </ul>
</div>
<div class="card-footer d-flex justify-content-end align-items-center flex-wrap gap-2">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"><i class="bi bi-x-lg">&nbsp;</i>&nbsp;Cancel</asp:LinkButton>
</div>
