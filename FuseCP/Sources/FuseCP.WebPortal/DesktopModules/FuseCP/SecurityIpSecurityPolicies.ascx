<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecurityIpSecurityPolicies.ascx.cs" Inherits="FuseCP.Portal.SecurityIpSecurityPolicies" %>

<div class="card-body border-bottom">
    <h5 class="mb-3">IP Policy Actions</h5>
        <div class="row g-3">
            <div class="col-xl-3 col-lg-4">
                <asp:Label ID="lblIpRange" runat="server" AssociatedControlID="txtIpRange" CssClass="form-label" Text="IP or CIDR"></asp:Label>
                <asp:TextBox ID="txtIpRange" runat="server" CssClass="form-control" />
            </div>
            <div class="col-xl-4 col-lg-4">
                <asp:Label ID="lblReason" runat="server" AssociatedControlID="txtReason" CssClass="form-label" Text="Reason"></asp:Label>
                <asp:TextBox ID="txtReason" runat="server" CssClass="form-control" />
            </div>
            <div class="col-xl-2 col-lg-2 col-md-4">
                <asp:Label ID="lblDuration" runat="server" AssociatedControlID="txtDurationMinutes" CssClass="form-label" Text="Duration (minutes)"></asp:Label>
                <asp:TextBox ID="txtDurationMinutes" runat="server" CssClass="form-control" Text="0" />
            </div>
            <div class="col-xl-3 col-lg-2 col-md-8">
                <div class="d-flex flex-wrap flex-xl-nowrap align-items-center gap-2 pt-lg-4 h-100">
                    <div class="form-check mb-0 flex-shrink-0">
                        <asp:CheckBox ID="chkServerAdminAccess" runat="server" meta:resourcekey="chkServerAdminAccess" />
                        <asp:Label ID="lblServerAdminAccess" runat="server" AssociatedControlID="chkServerAdminAccess" CssClass="form-check-label text-nowrap" meta:resourcekey="lblServerAdminAccess" Text="Server Admin"></asp:Label>
                    </div>
                    <asp:LinkButton ID="btnBlock" runat="server" CssClass="btn btn-danger flex-shrink-0" ToolTip="Block this IP or subnet" OnClick="btnBlock_Click" meta:resourcekey="btnBlock">
                        <i class="bi bi-shield-fill-x" aria-hidden="true"></i>
                        <span class="visually-hidden"><asp:Localize ID="locBlockButtonText" runat="server" meta:resourcekey="locBlockButtonText" Text="Block" /></span>
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnWhitelist" runat="server" CssClass="btn btn-success flex-shrink-0" ToolTip="Whitelist this IP or subnet" OnClick="btnWhitelist_Click" meta:resourcekey="btnWhitelist">
                        <i class="bi bi-shield-fill-check" aria-hidden="true"></i>
                        <span class="visually-hidden"><asp:Localize ID="locWhitelistButtonText" runat="server" meta:resourcekey="locWhitelistButtonText" Text="Whitelist" /></span>
                    </asp:LinkButton>
                </div>
            </div>
        </div>

        <p class="text-muted mt-3 mb-0"><asp:Localize ID="litActionHelp" runat="server" meta:resourcekey="litActionHelp" Text="Enable Server Admin before whitelisting if this IP should also reach serveradmin-only portal pages. You can still change or remove it from the list below." /></p>
</div>

<div class="card-body">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h5 class="m-0"><asp:Localize ID="litPoliciesTitle" runat="server" meta:resourcekey="litPoliciesTitle" Text="IP Security Policies" /></h5>
        <asp:Button ID="btnRefreshPolicies" runat="server" CssClass="btn btn-primary" Text="Refresh" OnClick="btnRefreshPolicies_Click" />
    </div>
        <asp:GridView ID="gvPolicies" runat="server" AutoGenerateColumns="False" CssSelectorClass="NormalGridView" Width="100%"
            RowStyle-CssClass="fcp-grid-row" AlternatingRowStyle-CssClass="fcp-grid-row-alt" EmptyDataText="No policies.">
            <Columns>
                <asp:BoundField DataField="CreatedDate" HeaderText="Created (UTC)" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                <asp:BoundField DataField="IpRange" HeaderText="IP/CIDR" />
                <asp:TemplateField HeaderText="Type">
                    <ItemTemplate>
                        <asp:Label ID="lblPolicyType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Rule">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnTogglePolicyState" runat="server" CssClass="btn btn-sm btn-link p-0 text-decoration-none"
                            CommandName="TogglePolicyState" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">
                            <i id="iconPolicyState" runat="server" class="bi bi-eye-fill" aria-hidden="true"></i>
                            <span class="visually-hidden"><asp:Localize ID="locTogglePolicyState" runat="server" meta:resourcekey="locTogglePolicyState" Text="Toggle rule state" /></span>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Server Admin">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkPortalAdminAccess" runat="server" AutoPostBack="true" OnCheckedChanged="chkPortalAdminAccess_CheckedChanged" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblPolicyStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SeverityLevel" HeaderText="Severity" />
                <asp:BoundField DataField="ExpiresDate" HeaderText="Expires (UTC)" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                <asp:BoundField DataField="CreatedBy" HeaderText="Created By" />
                <asp:BoundField DataField="Reason" HeaderText="Reason" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnRemovePolicy" runat="server" CssClass="btn btn-sm btn-outline-secondary"
                            Text="Remove" CommandName="RemovePolicy" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
</div>
