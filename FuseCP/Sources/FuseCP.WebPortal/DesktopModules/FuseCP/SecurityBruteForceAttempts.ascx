<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecurityBruteForceAttempts.ascx.cs" Inherits="FuseCP.Portal.SecurityBruteForceAttempts" %>

<div class="FormButtonsBar right fcp-home-create-toolbar">
    <div class="right">
        <a class="btn btn-primary" href="Default.aspx?pid=SecurityIpSecurityPolicies">
            <i class="bi bi-shield-lock" aria-hidden="true"></i>&nbsp;IP Security Policies
        </a>
    </div>
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
                <asp:Label ID="lblResultFilter" runat="server" AssociatedControlID="ddlResultFilter" CssClass="form-label" Text="Result"></asp:Label>
                <asp:DropDownList ID="ddlResultFilter" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Failed attempts only" Value="failed" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="All attempts" Value="all"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-2 text-md-end">
                <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-primary w-100" Text="Refresh" OnClick="btnRefresh_Click" CausesValidation="false" UseSubmitBehavior="false" />
            </div>
        </div>

    <div class="row g-3 align-items-end mb-3">
        <div class="col-md-3">
            <asp:Label ID="lblFromUtc" runat="server" AssociatedControlID="txtFromUtc" CssClass="form-label" Text="From (UTC)"></asp:Label>
            <asp:TextBox ID="txtFromUtc" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
        </div>
        <div class="col-md-3">
            <asp:Label ID="lblToUtc" runat="server" AssociatedControlID="txtToUtc" CssClass="form-label" Text="To (UTC)"></asp:Label>
            <asp:TextBox ID="txtToUtc" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
        </div>
        <div class="col-md-6 d-flex justify-content-md-end align-items-end gap-2">
            <asp:Button ID="btnPreviousPage" runat="server" CssClass="btn btn-outline-secondary" Text="Previous" OnClick="btnPreviousPage_Click" CausesValidation="false" UseSubmitBehavior="false" />
            <asp:Label ID="lblPageInfo" runat="server" CssClass="form-text mb-0" Text="Page 1"></asp:Label>
            <asp:Button ID="btnNextPage" runat="server" CssClass="btn btn-outline-secondary" Text="Next" OnClick="btnNextPage_Click" CausesValidation="false" UseSubmitBehavior="false" />
        </div>
    </div>

    <asp:GridView ID="gvAttempts" runat="server" AutoGenerateColumns="False" CssSelectorClass="NormalGridView" Width="100%" OnRowCommand="gvAttempts_RowCommand"
        RowStyle-CssClass="fcp-grid-row" AlternatingRowStyle-CssClass="fcp-grid-row-alt" EmptyDataText="No attempts found.">
        <Columns>
            <asp:BoundField DataField="AttemptTime" HeaderText="Time (UTC)" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
            <asp:BoundField DataField="IpAddress" HeaderText="IP Address" />
            <asp:BoundField DataField="Username" HeaderText="Username" />
            <asp:BoundField DataField="Layer" HeaderText="Layer" />
            <asp:TemplateField HeaderText="Result">
                <ItemTemplate>
                    <span class='<%# Convert.ToBoolean(Eval("Succeeded")) ? "badge text-bg-success" : "badge text-bg-danger" %>'>
                        <%# Convert.ToBoolean(Eval("Succeeded")) ? "Success" : "Failed" %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="IP Actions">
                <ItemTemplate>
                    <div class="d-flex gap-1">
                        <asp:LinkButton ID="btnBlockAttemptIp" runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Block this IP"
                            CommandName="BlockIp" CommandArgument='<%# Eval("IpAddress") %>' CausesValidation="false">
                            <i class="bi bi-shield-fill-x" aria-hidden="true"></i>
                            <span class="visually-hidden">Block IP</span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnWhitelistAttemptIp" runat="server" CssClass="btn btn-sm btn-outline-success" ToolTip="Whitelist this IP"
                            CommandName="WhitelistIp" CommandArgument='<%# Eval("IpAddress") %>' CausesValidation="false">
                            <i class="bi bi-shield-fill-check" aria-hidden="true"></i>
                            <span class="visually-hidden">Whitelist IP</span>
                        </asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
