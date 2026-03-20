<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SettingsFuseCPPolicy.ascx.cs" Inherits="FuseCP.Portal.SettingsFuseCPPolicy" %>
<%@ Register Src="UserControls/UsernamePolicyEditor.ascx" TagName="UsernamePolicyEditor" TagPrefix="uc2" %>
<%@ Register Src="UserControls/PasswordPolicyEditor.ascx" TagName="PasswordPolicyEditor" TagPrefix="uc1" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>

<fcp:CollapsiblePanel id="secPanelSettings" runat="server"
    TargetControlID="PanelSettingsPanel" meta:resourcekey="secPanelSettings" Text="FuseCP Settings">
</fcp:CollapsiblePanel>
<asp:Panel ID="PanelSettingsPanel" runat="server" Height="0" style="overflow:hidden">
    <table class="SettingsPanel">
        <tr>
            <td class="SubHead">
                <asp:Label ID="lblPassword" runat="server" meta:resourcekey="lblPassword" Text="Account Password:"></asp:Label>
            </td>
            <td class="Normal">
                <uc1:PasswordPolicyEditor id="passwordPolicy" runat="server">
                </uc1:PasswordPolicyEditor>
            </td>
        </tr>
    </table>
</asp:Panel>
