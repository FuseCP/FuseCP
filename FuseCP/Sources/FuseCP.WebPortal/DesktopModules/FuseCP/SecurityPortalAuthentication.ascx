<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecurityPortalAuthentication.ascx.cs" Inherits="FuseCP.Portal.SecurityPortalAuthentication" %>

<asp:Panel ID="pnlFixNow" runat="server" CssClass="card border-danger mb-3">
    <div class="card-header bg-danger text-white d-flex justify-content-between align-items-center">
        <h3 class="card-title m-0">Fix It Now</h3>
        <asp:Label ID="lblFixNowSeverity" runat="server" CssClass="badge bg-light text-dark" />
    </div>
    <div class="card-body">
        <asp:Label ID="lblFixNowSummary" runat="server" CssClass="d-block mb-3 fw-semibold" />
        <div class="row g-3">
            <div class="col-md-6">
                <div class="border rounded p-3 h-100 d-flex justify-content-between align-items-center">
                    <div>
                        <strong>Password Reset</strong>
                        <div class="text-muted small">Ensure reset flow and lifespan are configured.</div>
                    </div>
                    <asp:Label ID="lblPasswordResetStatus" runat="server" CssClass="badge" />
                </div>
            </div>
            <div class="col-md-6">
                <div class="border rounded p-3 h-100 d-flex justify-content-between align-items-center">
                    <div>
                        <strong>MFA App Identity</strong>
                        <div class="text-muted small">Set authenticator app display name for users.</div>
                    </div>
                    <asp:Label ID="lblMfaStatus" runat="server" CssClass="badge" />
                </div>
            </div>
            <div class="col-md-6">
                <div class="border rounded p-3 h-100 d-flex justify-content-between align-items-center">
                    <div>
                        <strong>Brute Force Policy</strong>
                        <div class="text-muted small">Keep limits in safe, non-zero ranges.</div>
                    </div>
                    <asp:Label ID="lblBruteForceStatus" runat="server" CssClass="badge" />
                </div>
            </div>
        </div>
    </div>
</asp:Panel>

<div class="card mb-3">
    <div class="card-header">
        <h3 class="card-title m-0">Portal Authentication</h3>
    </div>
    <div class="card-body">
        <div class="row mb-3">
            <label for="chkEnablePasswordReset" class="col-sm-3 form-label">Enable password reset</label>
            <div class="col-sm-9">
                <asp:CheckBox ID="chkEnablePasswordReset" runat="server" CssClass="fcp-check-inline" Text="Enabled" />
            </div>
        </div>
        <div class="row mb-3">
            <label for="txtPasswordResetLinkLifeSpan" class="col-sm-3 form-label">Password reset link lifespan</label>
            <div class="col-sm-9">
                <asp:TextBox ID="txtPasswordResetLinkLifeSpan" runat="server" CssClass="form-control" />
            </div>
        </div>
        <asp:Button ID="btnSavePortalAuth" runat="server" CssClass="btn btn-success" Text="Save Portal Authentication" OnClick="btnSavePortalAuth_Click" />
    </div>
</div>

<div class="card">
    <div class="card-header">
        <h3 class="card-title m-0">MFA Settings</h3>
    </div>
    <div class="card-body">
        <div class="row mb-3">
            <label for="txtMfaTokenAppDisplayName" class="col-sm-3 form-label">Authenticator app display name</label>
            <div class="col-sm-9">
                <asp:TextBox ID="txtMfaTokenAppDisplayName" runat="server" CssClass="form-control" />
            </div>
        </div>
        <div class="row mb-3">
            <label for="chkCanPeerChangeMFa" class="col-sm-3 form-label">Allow peers to change MFA</label>
            <div class="col-sm-9">
                <asp:CheckBox ID="chkCanPeerChangeMFa" runat="server" CssClass="fcp-check-inline" Text="Allowed" />
            </div>
        </div>
        <div class="row mb-3">
            <label for="txtBruteForceMaxFailedAttempts" class="col-sm-3 form-label">Brute force max failed attempts</label>
            <div class="col-sm-9">
                <asp:TextBox ID="txtBruteForceMaxFailedAttempts" runat="server" CssClass="form-control" />
            </div>
        </div>
        <div class="row mb-3">
            <label for="txtBruteForceWindowMinutes" class="col-sm-3 form-label">Brute force window (minutes)</label>
            <div class="col-sm-9">
                <asp:TextBox ID="txtBruteForceWindowMinutes" runat="server" CssClass="form-control" />
            </div>
        </div>
        <div class="row mb-3">
            <label for="txtBruteForceLockoutMinutes" class="col-sm-3 form-label">Brute force lockout (minutes)</label>
            <div class="col-sm-9">
                <asp:TextBox ID="txtBruteForceLockoutMinutes" runat="server" CssClass="form-control" />
            </div>
        </div>
        <div class="row mb-3">
            <label for="txtBruteForceCriticalAttempts" class="col-sm-3 form-label">Brute force critical attempts</label>
            <div class="col-sm-9">
                <asp:TextBox ID="txtBruteForceCriticalAttempts" runat="server" CssClass="form-control" />
                <p class="text-muted mt-2 mb-0">Critical threshold creates a non-expiring block.</p>
            </div>
        </div>
        <asp:Button ID="btnSaveMfa" runat="server" CssClass="btn btn-success" Text="Save MFA Settings" OnClick="btnSaveMfa_Click" />
    </div>
</div>
