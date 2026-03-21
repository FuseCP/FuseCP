<%@ Control Language="C#" AutoEventWireup="true" Inherits="FuseCP.Portal.SpaceSettingsPreviewDomain" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>

<fcp:CollapsiblePanel id="secPreviewDomain" runat="server"
    TargetControlID="PreviewDomainPanel" meta:resourcekey="secPreviewDomain" Text="Preview Domain"/>
<asp:Panel ID="PreviewDomainPanel" runat="server" Height="0" style="overflow:hidden">
    <table>
        <tr>
            <td class="SubHead text-nowrap">
                <asp:Label ID="lblPreviewDomain" runat="server" meta:resourcekey="lblPreviewDomain" Text="Preview Domain:"></asp:Label>
            </td>
            <td class="NormalBold">
                <div class="d-flex flex-wrap gap-2 align-items-center">
                domain.com.&nbsp;<asp:TextBox ID="txtPreviewDomain" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>