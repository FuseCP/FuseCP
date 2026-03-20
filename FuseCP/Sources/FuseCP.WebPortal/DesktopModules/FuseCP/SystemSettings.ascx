<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SystemSettings.ascx.cs" Inherits="FuseCP.Portal.SystemSettings" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="UserControls/EditFeedsList.ascx" TagName="EditFeedsList" TagPrefix="uc1" %>
<div class="card-body fcp-system-settings">
    <div class="container">
        <div class="accordion" id="accordion">
            <div class="card">
                <div class="card-header card-header-link">
                    <span><i class="bi bi-envelope" aria-hidden="true">&nbsp;</i>&nbsp;</span>
                    <a data-bs-toggle="collapse" data-bs-parent="#accordion" href="#lclSmtpSettings" aria-expanded="false" class="collapsed">
                        <asp:Localize runat="server" meta:resourcekey="HeaderSmtpSettings" /><span class='bi bi-plus-lg float-end' aria-hidden='true'> </span>
                    </a>
                </div>
                <div id="lclSmtpSettings" class="accordion-collapse collapse" aria-expanded="false">
                    <div class="card-body">
                        <fieldset>
                            <div class="row">
                                <div class="col-sm-12">
                                    <div class="row mb-3">
                                        <label for="txtSmtpServer" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblSmtpServer" runat="server" meta:resourcekey="SettinglblSmtpServer" />
                                        </label>
                                        <div class="col-sm-6">
                                            <asp:TextBox runat="server" CssClass="form-control" meta:resourcekey="SettingPlcSmtpServer" ID="txtSmtpServer" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="row mb-3">
                                        <label for="txtSmtpPort" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblSmtpPort" runat="server" meta:resourcekey="SettinglblSmtpPort" />
                                        </label>
                                        <div class="col-sm-6">
                                            <asp:TextBox runat="server" CssClass="form-control" meta:resourcekey="SettingPlcSmtpPort" ID="txtSmtpPort" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="row mb-3">
                                        <label for="txtSmtpUser" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblSmtpUser" runat="server" meta:resourcekey="SettinglblSmtpUser" />
                                        </label>
                                        <div class="col-sm-6">
                                            <asp:TextBox runat="server" CssClass="form-control" meta:resourcekey="SettingPlcSmtpUser" ID="txtSmtpUser" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="row mb-3">
                                        <label for="txtSmtpPassword" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblSmtpUserPassword" runat="server" meta:resourcekey="SettinglblSmtpUserPassword" />
                                        </label>
                                        <div class="col-sm-6">
                                            <asp:TextBox runat="server" CssClass="form-control" ID="txtSmtpPassword" meta:resourcekey="SettingPlcSmtpUserPassword" TextMode="Password" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="row mb-3">
                                        <label for="chkEnableSsl" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblSmtpEnableSSL" runat="server" meta:resourcekey="SettinglblSmtpEnableSSL" />
                                        </label>
                                        <div class="col-sm-6">
                                            <asp:CheckBox ID="chkEnableSsl" runat="server" CssClass="fcp-check-inline" Text="Enable" meta:resourcekey="SettingchkSmtpEnableSSL" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="row mb-3">
                                        <label for="chkEnableLegacySSL" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblEnableLegacySSL" runat="server" meta:resourcekey="SettinglblSmtpEnableLegacySSL" Text="Enable Support for unsecure SSL Versions TLS1 and TLS1.1 (Not recommended):" />
                                        </label>
                                        <div class="col-sm-6">
                                            <asp:CheckBox ID="chkEnableLegacySSL" runat="server" CssClass="fcp-check-inline" Text="Enable" meta:resourcekey="SettingchkSmtpEnableLegacySSL"/>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="row mb-3">
                                    <p><asp:Localize ID="configuremailtemplates" runat="server" meta:resourcekey="Settingsconfiguremailtemplates" /><br />
                               <asp:HyperLink ID="MailTemplates" runat="server" NavigateUrl="/Default.aspx?mid=25&ctl=mail_templates&UserID=1" Text="Serveradmin - Home"></asp:HyperLink></p>
                            </div>
                        </div>
                            </div>
                        <hr />
                        <asp:LinkButton ID="StyleButton1" CssClass="btn btn-success w-100" runat="server" meta:resourcekey="SettingbtnSaveSettings" OnClick="btnSaveSMTP_Click" />
                    </div>
                </div>
            </div>
        </div>


        <div class="card">
            <div class="card-header card-header-link">
                <span><i class="bi bi-hdd-stack" aria-hidden="true">&nbsp;</i>&nbsp;&nbsp;</span>
                <a data-bs-toggle="collapse" data-bs-parent="#accordion" href="#lclBackupSettings" aria-expanded="false" class="collapsed">
                    <asp:Localize runat="server" meta:resourcekey="HeaderBackupSettings" /><span class='bi bi-plus-lg float-end' aria-hidden='true'> </span>
                </a>
            </div>
            <div id="lclBackupSettings" class="accordion-collapse collapse" aria-expanded="false">
                <div class="card-body">
                    <fieldset>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="row mb-3">
                                    <label for="txtBackupsPath" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblBackupFolderPath" runat="server" meta:resourcekey="SettinglblBackupFolderPath" />
                                    </label>
                                    <div class="col-sm-6">
                                        <asp:TextBox runat="server" ID="txtBackupsPath" meta:resourcekey="SettingPlcBackupFolderPath" CssClass="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <hr />
                    <asp:LinkButton ID="StyleButton2" CssClass="btn btn-success w-100" runat="server" meta:resourcekey="SettingbtnSaveSettings" OnClick="btnSaveBACKUP_Click" />
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header card-header-link">
                <span><i class="bi bi-file-text" aria-hidden="true">&nbsp;</i>&nbsp;&nbsp;</span>
                <a data-bs-toggle="collapse" data-bs-parent="#accordion" href="#PanelFileManagereSettings" aria-expanded="false" class="collapsed">
                    <asp:Localize runat="server" meta:resourcekey="HeaderFileManagerSettings" /><span class='bi bi-plus-lg float-end' aria-hidden='true'> </span>
                </a>
            </div>
            <div id="PanelFileManagereSettings" class="accordion-collapse collapse" aria-expanded="false">
                <div class="card-body">
                    <fieldset>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="row mb-3">
                                    <label for="txtFileManagerEditableExtensions" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblFileManagerEditableExtensions" runat="server" meta:resourcekey="SettinglblFileManagerEditableExtensions" />
                                    </label>
                                    <div class="col-sm-6">
                                        <asp:TextBox TextMode="MultiLine" Rows="10" runat="server" ID="txtFileManagerEditableExtensions" CssClass="form-control" />
                                        <asp:Literal ID="SettinglitFileManagerEditableExtensions" runat="server" meta:resourcekey="SettinglitFileManagerEditableExtensions" Text=" (One (1) extension per line)"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <hr />
                    <asp:LinkButton ID="StyleButton4" CssClass="btn btn-success w-100" runat="server" meta:resourcekey="SettingbtnSaveSettings" OnClick="btnSaveFILEMANAGER_Click" />
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header card-header-link">
                <span><i class="bi bi-server" aria-hidden="true">&nbsp;</i>&nbsp;&nbsp;</span>
                <a data-bs-toggle="collapse" data-bs-parent="#accordion" href="#RdsSettings" aria-expanded="false" class="collapsed">
                    <asp:Localize runat="server" meta:resourcekey="HeaderRdsSettings" /><span class='bi bi-plus-lg float-end' aria-hidden='true'> </span>
                </a>
            </div>
            <div id="RdsSettings" class="accordion-collapse collapse" aria-expanded="false">
                <div class="card-body">
                    <fieldset>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="row mb-3">
                                    <label for="ddlRdsController" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblRdsController" runat="server" meta:resourcekey="SettinglblRdsController" />
                                    </label>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="ddlRdsController" runat="server" CssClass="form-select" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <hr />
                    <asp:LinkButton ID="StyleButton5" CssClass="btn btn-success w-100" runat="server" meta:resourcekey="SettingbtnSaveSettings" OnClick="btnSaveRDS_Click" />
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header card-header-link">
                <span><i class="bi bi-stack-exchange" aria-hidden="true">&nbsp;</i>&nbsp;&nbsp;</span>
                <a data-bs-toggle="collapse" data-bs-parent="#accordion" href="#OwaSettings" aria-expanded="false" class="collapsed">
                    <asp:Localize runat="server" meta:resourcekey="HeaderOwaSettings" /><span class='bi bi-plus-lg float-end' aria-hidden='true'> </span>
                </a>
            </div>
            <div id="OwaSettings" class="accordion-collapse collapse" aria-expanded="false">
                <div class="card-body">
                    <fieldset>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="row mb-3">
                                    <label for="chkEnableOwa" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblEnableOwa" runat="server" meta:resourcekey="SettinglblEnableOwa" />
                                    </label>
                                    <div class="col-sm-6">
                                        <asp:CheckBox ID="chkEnableOwa" runat="server" CssClass="fcp-check-inline" Text="Yes" meta:resourcekey="SettingchkEnableOwa" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="row mb-3">
                                    <label for="txtOwaUrl" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblOwaUrl" runat="server" meta:resourcekey="SettinglblOwaUrl" />
                                    </label>
                                    <div class="col-sm-6">
                                        <asp:TextBox runat="server" ID="txtOwaUrl" CssClass="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <hr />
                    <asp:LinkButton ID="StyleButton6" CssClass="btn btn-success w-100" runat="server" meta:resourcekey="SettingbtnSaveSettings" OnClick="btnSaveOWA_Click" />
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header card-header-link">
                <span><i class="bi bi-cloud" aria-hidden="true">&nbsp;</i>&nbsp;</span>
                <a data-bs-toggle="collapse" data-bs-parent="#accordion" href="#collapse-764" aria-expanded="false" class="collapsed">
                    <asp:Localize runat="server" meta:resourcekey="HeaderCloudStorageSettings" /><span class='bi bi-plus-lg float-end' aria-hidden='true'> </span>
                </a>
            </div>
            <div id="collapse-764" class="accordion-collapse collapse" aria-expanded="false">
                <div class="card-body">
                    <div class="accordion" id="collapse-765">
                        <div class="card">
                            <div class="card-header card-header-link">
                                <span><i class="bi bi-chat-dots" aria-hidden="true">&nbsp;</i>&nbsp;</span>
                                <a data-bs-toggle="collapse" data-bs-parent="#collapse-765" href="#TwilioSettings" aria-expanded="false" class="collapsed">
                                    <asp:Localize runat="server" meta:resourcekey="HeaderTwilioSettings" /><span class='bi bi-plus-lg float-end' aria-hidden='true'> </span>
                                </a>
                            </div>
                            <div id="TwilioSettings" class="accordion-collapse collapse" aria-expanded="false">
                                <div class="card-body">
                                    <div class="alert alert-info">
                                        <p>Visit <a href="https://www.twilio.com">https://www.twilio.com</a> to get your Twilio Information.</p>
                                    </div>
                                    <fieldset>
                                        <div class="row">
                                            <div class="col-md-8">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="row mb-3">
                                                    <label for="txtAccountSid" class="col-sm-2 form-label">
                                                                <asp:Localize ID="SettinglblTwilioAccountSid" runat="server" meta:resourcekey="SettinglblTwilioAccountSid" />
                                                    </label>
                                                    <div class="col-sm-6">
                                                        <asp:TextBox runat="server" ID="txtAccountSid" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="row mb-3">
                                                    <label for="txtAuthToken" class="col-sm-2 form-label">
                                                                <asp:Localize ID="SettinglblTwilioAuthToken" runat="server" meta:resourcekey="SettinglblTwilioAuthToken" />
                                                    </label>
                                                    <div class="col-sm-6">
                                                        <asp:TextBox runat="server" ID="txtAuthToken" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="row mb-3">
                                                    <label for="SettingNoteTwilioAccount" class="col-sm-2 form-label">
                                                                <asp:Localize ID="SettinglblTwilioPhoneFrom" runat="server" meta:resourcekey="SettinglblTwilioPhoneFrom" />
                                                    </label>
                                                    <div class="col-sm-6">
                                                        <asp:TextBox runat="server" ID="txtPhoneFrom" meta:resourcekey="SettingPlcTwilioPhoneFrom" CssClass="form-control" />
                                                        <asp:Localize ID="SettingNoteTwilioAccount" runat="server" meta:resourcekey="SettingNoteTwilioAccount" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                                </div>
                                            <div class="col-md-4">
                                    <asp:LinkButton ID="btnTwilioDisable" CssClass="btn btn-danger" runat="server" meta:resourcekey="btnTwillioDisable" OnClick="btnDisableTWILIO_Click" />
                                            </div>
                                            </div>
                                    </fieldset>
                                    <hr />
                                    <asp:LinkButton ID="StyleButton7" CssClass="btn btn-success w-100" runat="server" meta:resourcekey="SettingbtnSaveSettings" OnClick="btnSaveTWILIO_Click" />
                                </div>
                            </div>
                        </div>
                        <div class="card">
                            <div class="card-header card-header-link">
                                <span><i class="bi bi-cloud" aria-hidden="true">&nbsp;</i>&nbsp;</span>
                                <a data-bs-toggle="collapse" data-bs-parent="#collapse-765" href="#WebdavPortalSettings" aria-expanded="false" class="collapsed">
                                    <asp:Localize runat="server" meta:resourcekey="HeaderCloudStorageSettings" /><span class='bi bi-plus-lg float-end' aria-hidden='true'> </span>
                                </a>
                            </div>
                            <div id="WebdavPortalSettings" class="accordion-collapse collapse" aria-expanded="false">
                                <div class="card-body">
                                    <fieldset>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="row mb-3">
                                                    <label for="txtWebdavPortalUrl" class="col-sm-2 form-label">
                                                                <asp:Localize ID="SettinglblWebdavPortalUrl" runat="server" meta:resourcekey="SettinglblWebdavPortalUrl" />
                                                    </label>
                                                    <div class="col-sm-6">
                                                        <asp:TextBox runat="server" ID="txtWebdavPortalUrl" meta:resourcekey="SettingPlcWebdavPortalUrl" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="row mb-3">
                                                    <label class="col-sm-2 form-label">Password Reset</label>
                                                    <div class="col-sm-6">
                                                        <div class="alert alert-info mb-0 py-2">
                                                            Password reset is always enabled and managed in
                                                            <a href="Default.aspx?pid=SecurityPortalAuthentication">Security Portal Authentication</a>.
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </fieldset>
                                    <hr />
                                    <asp:LinkButton ID="StyleButton8" CssClass="btn btn-success w-100" runat="server" meta:resourcekey="SettingbtnSaveSettings" OnClick="btnSaveCLOUD_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header card-header-link">
                <span><i class="bi bi-lock" aria-hidden="true">&nbsp;</i>&nbsp;&nbsp;</span>
                <a data-bs-toggle="collapse" data-bs-parent="#accordion" href="#AccessIPsSettings" aria-expanded="false" class="collapsed">
                    <asp:Localize ID="HeaderIpRestrictionSettings" runat="server" meta:resourcekey="HeaderIpRestrictionSettings" /><span class='bi bi-plus-lg float-end' aria-hidden='true'> </span>
                </a>
            </div>
            <div id="AccessIPsSettings" class="accordion-collapse collapse" aria-expanded="false">
                <div class="card-body">
                    <fieldset>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="row mb-3">
                                    <label for="txtIPAddress" class="col-sm-2 form-label">
                                                <asp:Localize ID="SettinglblIpAddressRestriction" runat="server" meta:resourcekey="SettinglblIpAddressRestriction" />
                                    </label>
                                    <div class="col-sm-6">
                                        <asp:TextBox runat="server" TextMode="MultiLine" Rows="10" ID="txtIPAddress" CssClass="form-control" />
                                        <div>
                                            <p class="text-info">
                                                Use this to Restrict administrator access from specific IP addresses, you can use single IP's or subnets (/26 /24 /22, etc..) Put one IP or Subnet per line and comma separate them.
                                                            <br />
                                                <strong>Examples:</strong><br />
                                                192.168.0.100,<br />
                                                10.0.1.0/24,<br />
                                                10.1.1.0/30<br />
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <hr />
                    <asp:LinkButton ID="StyleButton9" CssClass="btn btn-success w-100" runat="server" meta:resourcekey="SettingbtnSaveSettings" OnClick="btnSaveRESTRICT_Click" />
                </div>
            </div>
        </div>
        <asp:Panel id="DebugSettingsPanel" runat="server" CssClass="card">
            <div class="card-header card-header-link">
                <span><i class="bi bi-wrench" aria-hidden="true">&nbsp;</i>&nbsp;&nbsp;</span>
                <a data-bs-toggle="collapse" data-bs-parent="#accordion" href="#DebugSettings" aria-expanded="false" class="collapsed">
                    <asp:Localize ID="Localize1" runat="server" meta:resourcekey="HeaderDebugSettings" /><span class='bi bi-plus-lg float-end' aria-hidden='true'> </span>
                </a>
            </div>
            <div id="DebugSettings" class="accordion-collapse collapse" aria-expanded="false">
                <div class="card-body">
                    <fieldset>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="row mb-3">
                                    <label for="chkAlwaysUseEntityFramework" class="col-sm-2 form-label">
                                        <asp:Localize ID="SettingchkAlwaysUseEntityFramework" runat="server" meta:resourcekey="SettingchkAlwaysUseEntityFramework" />
                                    </label>
                                <div class="col-sm-6">
                                    <asp:CheckBox ID="chkAlwaysUseEntityFramework" runat="server" CssClass="fcp-check-inline" Text="Yes" meta:resourcekey="SettingchkAlwaysUseEntityFramework" />
                                </div>
                            </div>
                        </div>
                        </div>
                    </fieldset>
                    <hr />
                    <asp:LinkButton ID="btnDebugSettings" CssClass="btn btn-success w-100" runat="server" meta:resourcekey="SettingbtnSaveSettings" OnClick="btnDebugSettings_Click" />
                </div>
            </div>
        </asp:Panel>
    </div>
</div>


