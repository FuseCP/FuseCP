<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecurityBruteForceAttempts.ascx.cs" Inherits="FuseCP.Portal.SecurityBruteForceAttempts" %>

<div class="card">
    <div class="card-header">
        <h3 class="card-title m-0">Brute Force Attempts</h3>
    </div>
    <div class="card-body">
        <div class="row g-3 align-items-end mb-3">
            <div class="col-md-4">
                <asp:Label ID="lblIpFilter" runat="server" AssociatedControlID="txtIpFilter" CssClass="form-label" Text="IP Address"></asp:Label>
                <asp:TextBox ID="txtIpFilter" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblLayer" runat="server" AssociatedControlID="ddlLayer" CssClass="form-label" Text="Layer"></asp:Label>
                <asp:DropDownList ID="ddlLayer" runat="server" CssClass="form-select">
                    <asp:ListItem Text="All" Value=""></asp:ListItem>
                    <asp:ListItem Text="Portal" Value="Portal"></asp:ListItem>
                    <asp:ListItem Text="API" Value="API"></asp:ListItem>
                    <asp:ListItem Text="Server" Value="Server"></asp:ListItem>
                    <asp:ListItem Text="Module" Value="Module"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <div class="form-check">
                    <asp:CheckBox ID="chkFailedOnly" runat="server" CssClass="form-check-input" />
                    <asp:Label ID="lblFailedOnly" runat="server" AssociatedControlID="chkFailedOnly" CssClass="form-check-label" Text="Failed attempts only"></asp:Label>
                </div>
            </div>
            <div class="col-md-2 text-md-end">
                <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-primary w-100" Text="Refresh" OnClick="btnRefresh_Click" />
            </div>
        </div>

        <asp:GridView ID="gvAttempts" runat="server" AutoGenerateColumns="False" CssSelectorClass="NormalGridView" Width="100%"
            RowStyle-CssClass="fcp-grid-row" AlternatingRowStyle-CssClass="fcp-grid-row-alt" EmptyDataText="No attempts found.">
            <Columns>
                <asp:BoundField DataField="AttemptTime" HeaderText="Time (UTC)" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                <asp:BoundField DataField="IpAddress" HeaderText="IP Address" />
                <asp:BoundField DataField="Username" HeaderText="Username" />
                <asp:BoundField DataField="Layer" HeaderText="Layer" />
                <asp:CheckBoxField DataField="Succeeded" HeaderText="Succeeded" />
                <asp:BoundField DataField="UserAgent" HeaderText="User Agent" />
            </Columns>
        </asp:GridView>
    </div>
</div>
