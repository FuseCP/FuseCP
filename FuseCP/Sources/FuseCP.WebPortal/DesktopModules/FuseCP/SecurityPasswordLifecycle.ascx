<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecurityPasswordLifecycle.ascx.cs" Inherits="FuseCP.Portal.SecurityPasswordLifecycle" %>

<asp:Panel ID="pnlFixNow" runat="server" CssClass="card border-danger mb-3">
    <div class="card-header bg-danger text-white d-flex justify-content-between align-items-center">
        <h3 class="card-title m-0">Fix It Now</h3>
        <asp:Label ID="lblFixNowSeverity" runat="server" CssClass="badge bg-light text-dark" />
    </div>
    <div class="card-body">
        <asp:Label ID="lblFixNowSummary" runat="server" CssClass="d-block mb-3 fw-semibold" />
        <div class="table-responsive">
            <table class="table table-sm align-middle mb-0">
                <thead>
                    <tr>
                        <th style="width: 20%;">Area</th>
                        <th style="width: 55%;">Required Action</th>
                        <th style="width: 25%;">Go To</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><strong>Users</strong></td>
                        <td><asp:Label ID="lblFixUsersAction" runat="server" /></td>
                        <td><asp:HyperLink ID="lnkFixUsers" runat="server" CssClass="btn btn-sm btn-outline-danger" Text="Review Legacy Users" /></td>
                    </tr>
                    <tr>
                        <td><strong>Portal</strong></td>
                        <td><asp:Label ID="lblFixPortalAction" runat="server" /></td>
                        <td><asp:HyperLink ID="lnkFixPortal" runat="server" CssClass="btn btn-sm btn-outline-danger" Text="Open Portal Authentication" /></td>
                    </tr>
                    <tr>
                        <td><strong>Servers</strong></td>
                        <td><asp:Label ID="lblFixServersAction" runat="server" /></td>
                        <td><asp:HyperLink ID="lnkFixServers" runat="server" CssClass="btn btn-sm btn-outline-danger" Text="Open Server Status" /></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</asp:Panel>

<div class="card mb-3">
    <div class="card-header d-flex justify-content-between align-items-center">
        <h3 class="card-title m-0">Hardening Status</h3>
        <asp:Button ID="btnRefreshStatus" runat="server" CssClass="btn btn-primary" Text="Refresh Status" />
    </div>
    <div class="card-body">
        <p class="mb-3">
            This page shows what is already hardened, what is still running in legacy compatibility mode, and which migrations can happen automatically versus requiring an explicit reset.
        </p>

        <div class="row g-3 mb-3">
            <div class="col-lg-3 col-md-6">
                <div class="border rounded p-3 h-100">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <strong>Users</strong>
                        <asp:Label ID="lblUsersStatus" runat="server" CssClass="badge" />
                    </div>
                    <asp:Label ID="lblUsersSummary" runat="server" />
                </div>
            </div>
            <div class="col-lg-3 col-md-6">
                <div class="border rounded p-3 h-100">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <strong>Portal</strong>
                        <asp:Label ID="lblPortalStatus" runat="server" CssClass="badge" />
                    </div>
                    <asp:Label ID="lblPortalSummary" runat="server" />
                </div>
            </div>
            <div class="col-lg-3 col-md-6">
                <div class="border rounded p-3 h-100">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <strong>Enterprise API</strong>
                        <asp:Label ID="lblEnterpriseStatus" runat="server" CssClass="badge" />
                    </div>
                    <asp:Label ID="lblEnterpriseSummary" runat="server" />
                </div>
            </div>
            <div class="col-lg-3 col-md-6">
                <div class="border rounded p-3 h-100">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <strong>Servers</strong>
                        <asp:Label ID="lblServersStatus" runat="server" CssClass="badge" />
                    </div>
                    <asp:Label ID="lblServersSummary" runat="server" />
                </div>
            </div>
        </div>

        <div class="alert alert-info mb-0">
            <asp:Label ID="lblGeneratedUtc" runat="server" />
        </div>
    </div>
</div>

<div id="server-status" class="card mb-3">
    <div class="card-header">
        <h3 class="card-title m-0">Server Connection Status</h3>
    </div>
    <div class="card-body">
        <p class="mb-3">
            Use Fix on any row still allowing legacy fallback. If a server cannot be probed, use the Universal Installer emergency recovery on that host first, then return here and refresh.
        </p>
        <asp:GridView ID="gvServerStatus" runat="server" AutoGenerateColumns="False" CssSelectorClass="NormalGridView" Width="100%"
            RowStyle-CssClass="fcp-grid-row" AlternatingRowStyle-CssClass="fcp-grid-row-alt" EmptyDataText="No servers found.">
            <Columns>
                <asp:BoundField DataField="Status" HeaderText="Status" />
                <asp:BoundField DataField="ServerName" HeaderText="Server" />
                <asp:CheckBoxField DataField="PasswordIsSha256" HeaderText="SHA256" />
                <asp:CheckBoxField DataField="SupportsHmacAuthentication" HeaderText="HMAC" />
                <asp:CheckBoxField DataField="SupportsLegacyPasswordAuthentication" HeaderText="Legacy" />
                <asp:CheckBoxField DataField="ProbeSucceeded" HeaderText="Probe OK" />
                <asp:BoundField DataField="KeyId" HeaderText="Key Id" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnFixServer" runat="server" CssClass="btn btn-sm btn-outline-danger" Text="Fix"
                            CommandName="FixServer" CommandArgument='<%# Eval("ServerId") %>' CausesValidation="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<div id="legacy-users" class="card mb-3">
    <div class="card-header d-flex justify-content-between align-items-center">
        <h3 class="card-title m-0">Legacy User Inventory</h3>
        <div class="d-flex gap-2">
            <asp:Button ID="btnLoadLegacyUsers" runat="server" CssClass="btn btn-outline-primary" Text="Load Users" />
            <asp:Button ID="btnExportLegacyUsers" runat="server" CssClass="btn btn-outline-secondary" Text="Export CSV" />
        </div>
    </div>
    <div class="card-body">
        <p class="mb-3">
            This inventory shows accounts still using legacy or empty password storage so you can target resets, outreach, or forced sign-in migration.
        </p>
        <div class="row g-3 mb-3">
            <div class="col-md-6">
                <asp:Label ID="lblLegacyUserFilter" runat="server" AssociatedControlID="txtLegacyUserFilter" CssClass="form-label" Text="Username filter"></asp:Label>
                <asp:TextBox ID="txtLegacyUserFilter" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblLegacyUserMaxResults" runat="server" AssociatedControlID="txtLegacyUserMaxResults" CssClass="form-label" Text="Max results"></asp:Label>
                <asp:TextBox ID="txtLegacyUserMaxResults" runat="server" CssClass="form-control" Text="200" />
            </div>
            <div class="col-md-3 d-flex align-items-end">
                <asp:Label ID="lblLegacyInventorySummary" runat="server" CssClass="text-muted" />
            </div>
        </div>

        <asp:GridView ID="gvLegacyUsers" runat="server" AutoGenerateColumns="False" CssSelectorClass="NormalGridView" Width="100%"
            RowStyle-CssClass="fcp-grid-row" AlternatingRowStyle-CssClass="fcp-grid-row-alt" EmptyDataText="No legacy users matched the current filter.">
            <Columns>
                <asp:BoundField DataField="UserId" HeaderText="User Id" />
                <asp:BoundField DataField="Username" HeaderText="Username" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="PasswordStatus" HeaderText="Status" />
                <asp:CheckBoxField DataField="CanAutoHarden" HeaderText="Auto Fix" />
                <asp:CheckBoxField DataField="IsDemo" HeaderText="Demo" />
                <asp:BoundField DataField="RoleId" HeaderText="Role" />
                <asp:BoundField DataField="Changed" HeaderText="Changed" DataFormatString="{0:yyyy-MM-dd HH:mm}" HtmlEncode="false" />
            </Columns>
        </asp:GridView>
    </div>
</div>

<div id="user-convert" class="card card-body mt-3">
    <h4 class="mb-3">Convert Known User Passwords</h4>
    <p class="mb-3">
        Convert every legacy user record that already has a reusable SHA256 password proof stored in Enterprise. Remaining SHA1 or empty-password accounts still need a normal password reset or a successful sign-in migration.
    </p>
    <div class="d-flex flex-wrap gap-3 align-items-center">
        <asp:Button ID="btnAutoHardenEligibleUsers" runat="server" CssClass="btn btn-primary" Text="Convert Eligible Passwords Now" />
        <asp:Label ID="lblAutoHardenSummary" runat="server" CssClass="text-muted" />
    </div>
</div>
