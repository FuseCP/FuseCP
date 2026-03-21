<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GlobalDnsRecordsControl.ascx.cs" Inherits="FuseCP.Portal.GlobalDnsRecordsControl" %>
<%@ Register Src="UserControls/SelectIPAddress.ascx" TagName="SelectIPAddress" TagPrefix="uc1" %>

<asp:Panel ID="pnlRecords" runat="server">
    <div class="text-end fcp-mb-10">
        <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" OnClick="btnAdd_Click" CausesValidation="False">
            <i class="bi bi-plus-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnAdd" />
        </asp:LinkButton>
        <br />
    </div>
</asp:Panel>
<asp:Panel ID="pnlLoadRecords" runat="server" CssClass="alert alert-secondary d-flex flex-wrap justify-content-between align-items-center gap-2">
    <asp:Label ID="lblLoadRecordsHint" runat="server" Text="Existing DNS records are loaded on demand to keep large server pages responsive."></asp:Label>
    <asp:LinkButton ID="btnLoadRecords" runat="server" CssClass="btn btn-outline-primary" OnClick="btnLoadRecords_Click" CausesValidation="False">
        <i class="bi bi-arrow-repeat">&nbsp;</i>&nbsp;Load Existing DNS Records
    </asp:LinkButton>
</asp:Panel>
<asp:GridView ID="gvRecords" runat="server" AutoGenerateColumns="False"
    DataKeyNames="RecordID" EmptyDataText="gvRecords" CssSelectorClass="NormalGridView"
    OnRowEditing="gvRecords_RowEditing" OnRowDeleting="gvRecords_RowDeleting">
    <Columns>
        <asp:TemplateField HeaderText="gvRecordsName" ItemStyle-CssClass="NormalBold" ItemStyle-Wrap="false">
            <ItemTemplate>
                <ItemStyle Width="25px" HorizontalAlign="Center" />
                <asp:ImageButton ID="cmdEdit" runat="server" SkinID="EditSmall" CommandName="edit" AlternateText="Edit record">
                </asp:ImageButton>
                <%# Eval("RecordName") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="RecordType" HeaderText="gvRecordsType" />
        <asp:BoundField DataField="FullRecordData" HeaderText="gvRecordsData" />
        <asp:TemplateField>
            <ItemStyle Width="65px" HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:LinkButton id="cmdDelete" CssClass="btn btn-danger" runat="server" CommandName="delete" OnClientClick="return confirm('Delete?');"> 
                    &nbsp;<i class="bi bi-trash"></i>&nbsp; 
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:Panel ID="pnlEdit" runat="server" DefaultButton="btnSave">
    <table class="table table-borderless align-middle mb-0">
        <tr>
            <td class="SubHead text-nowrap"><asp:Label ID="lblRecordType" runat="server" meta:resourcekey="lblRecordType" Text="Record Type:"></asp:Label></td>
            <td class="Normal">
                <asp:DropDownList ID="ddlRecordType" runat="server" SelectedValue='<%# Bind("RecordType") %>' CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlRecordType_SelectedIndexChanged">
                    <asp:ListItem>A</asp:ListItem>
                    <asp:ListItem>AAAA</asp:ListItem>
                    <asp:ListItem>MX</asp:ListItem>
                    <asp:ListItem>NS</asp:ListItem>
                    <asp:ListItem>TXT</asp:ListItem>
                    <asp:ListItem>CNAME</asp:ListItem>
                    <asp:ListItem>SRV</asp:ListItem>
                    <asp:ListItem>PTR</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="SubHead"><asp:Label ID="lblRecordName" runat="server" meta:resourcekey="lblRecordName" Text="Record Name:"></asp:Label></td>
            <td class="Normal">
                <asp:TextBox ID="txtRecordName" runat="server" Width="260px" CssClass="form-control"></asp:TextBox>
            </td>
        </tr>
        <tr id="rowData" runat="server">
            <td class="SubHead"><asp:Label ID="lblRecordData" runat="server" meta:resourcekey="lblRecordData" Text="Record Data:"></asp:Label></td>
            <td class="Normal text-nowrap">
                <div class="d-flex flex-wrap gap-2 align-items-center">
                <asp:TextBox ID="txtRecordData" runat="server" Width="260px" CssClass="form-control"></asp:TextBox> <uc1:SelectIPAddress ID="ipAddress" CssClass="form-control" runat="server" AutoBindOnInitialLoad="false" />
                    </div>
<!--
                <asp:RequiredFieldValidator ID="valRequireData" runat="server" ControlToValidate="txtRecordData"
                    ErrorMessage="*" ValidationGroup="DnsRecord" Display="Dynamic"></asp:RequiredFieldValidator>
-->
                <asp:CustomValidator ID="IPValidator" runat="server" ControlToValidate="txtRecordData" ValidationGroup="DnsRecord" Display="Dynamic" CssClass="NormalBold" 
                    OnServerValidate="Validate" Text="Please enter a valid IP" meta:resourcekey="IPValidator" ValidateEmptyText="True" />

            </td>
        </tr>
        <tr id="rowMXPriority" runat="server">
            <td class="SubHead"><asp:Label ID="lblMXPriority" runat="server" meta:resourcekey="lblMXPriority" Text="MX Priority:"></asp:Label></td>
            <td class="Normal">
                <asp:TextBox ID="txtMXPriority" runat="server"  CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valRequireMxPriority" runat="server" ControlToValidate="txtMXPriority"
                    ErrorMessage="*" ValidationGroup="DnsRecord" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="valRequireCorrectPriority" runat="server" ControlToValidate="txtMXPriority"
                    ErrorMessage="*" ValidationExpression="\d{1,3}" ValidationGroup="DnsRecord" />
            </td>
        </tr>

        <tr id="rowSRVPriority" runat="server">
            <td class="SubHead"><asp:Label ID="lblSRVPriority" runat="server" meta:resourcekey="lblSRVPriority" Text="SRV Priority:"></asp:Label></td>
            <td class="Normal">
                <asp:TextBox ID="txtSRVPriority" runat="server"  CssClass="form-control"></asp:TextBox>
            </td>
        </tr>

        <tr id="rowSRVWeight" runat="server">
            <td class="SubHead"><asp:Label ID="lblSRVWeight" runat="server" meta:resourcekey="lblSRVWeight" Text="Weight:"></asp:Label></td>
            <td class="Normal">
                <asp:TextBox ID="txtSRVWeight" runat="server"  CssClass="form-control"></asp:TextBox>
            </td>
        </tr>

        <tr id="rowSRVPort" runat="server">
            <td class="SubHead"><asp:Label ID="lblSRVPort" runat="server" meta:resourcekey="lblSRVPort" Text="Port Number:"></asp:Label></td>
            <td class="Normal">
                <asp:TextBox ID="txtSRVPort" runat="server"  CssClass="form-control"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td class="text-end" colspan="2">
                <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
                <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" ValidationGroup="DnsRecord"> <i class="bi bi-floppy">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnSave"/> </asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Panel>

