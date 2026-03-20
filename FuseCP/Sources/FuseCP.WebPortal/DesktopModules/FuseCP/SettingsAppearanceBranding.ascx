<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SettingsAppearanceBranding.ascx.cs" Inherits="FuseCP.Portal.SettingsAppearanceBranding" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>

<fcp:CollapsiblePanel id="secAppearanceBranding" runat="server"
    TargetControlID="AppearanceBrandingPanel" Text="Appearance and Branding">
</fcp:CollapsiblePanel>
<asp:Panel ID="AppearanceBrandingPanel" runat="server" Height="0" style="overflow:hidden">
    <table class="SettingsPanel">
        <tr>
            <td class="SubHead">
                <asp:Label ID="lblLogoImage" runat="server" Text="Logo Image:"></asp:Label>
            </td>
            <td class="Normal">
                <asp:TextBox ID="txtLogoImageURL" runat="server" CssClass="form-control"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="SubHead">
                <asp:Label ID="lblDemoMessage" runat="server" Text="Demo Message:"></asp:Label>
            </td>
            <td class="Normal">
                <asp:TextBox ID="txtDemoMessage" runat="server" Rows="5" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Panel>
