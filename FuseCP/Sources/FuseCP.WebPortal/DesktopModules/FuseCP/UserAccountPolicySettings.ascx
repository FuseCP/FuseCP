<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserAccountPolicySettings.ascx.cs" Inherits="FuseCP.Portal.UserAccountPolicySettings" %>
<div class="card-body form-horizontal">
    <ul class="LinksList mb-0">
        <li>
            <asp:HyperLink ID="lnkServiceLevels" runat="server" meta:resourcekey="lnkServiceLevels"
                    Text="Service Levels" NavigateUrl='<%# GetSettingsLink("ServiceLevels", "SettingsServiceLevels") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkWebPolicy" runat="server" meta:resourcekey="lnkWebPolicy"
                    Text="WEB Policy" NavigateUrl='<%# GetSettingsLink("WebPolicy", "SettingsWebPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkExchangeMailboxPlansPolicy" runat="server" meta:resourcekey="lnkExchangeMailboxPlansPolicy"
                    Text="Global Exchange Mailbox Plans" NavigateUrl='<%# GetSettingsLink("ExchangeMailboxPlansPolicy", "SettingsExchangeMailboxPlansPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkExchangeRetentionPolicy" runat="server" meta:resourcekey="lnkExchangeRetentionPolicy"
                    Text="Global Retention Policy" NavigateUrl='<%# GetSettingsLink("RetentionPolicy", "SettingsExchangeMailboxPlansPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkExchangeRetentionPolicyTag" runat="server" meta:resourcekey="lnkExchangeRetentionPolicyTag"
                    Text="Global Retention Policy Tag" NavigateUrl='<%# GetSettingsLink("RetentionPolicyTag", "SettingsExchangeRetentionPolicyTag") %>'></asp:HyperLink>
        </li>

        <li>
            <asp:HyperLink ID="lnkLyncUserPlansPolicy" runat="server" meta:resourcekey="lnkLyncUserPlansPolicy"
                    Text="Lync Userplan Policy" NavigateUrl='<%# GetSettingsLink("LyncUserPlansPolicy", "SettingsLyncUserPlansPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkSfBUserPlansPolicy" runat="server" meta:resourcekey="lnkSfBUserPlansPolicy"
                    Text="Skype for Business Userplan Policy" NavigateUrl='<%# GetSettingsLink("SfBUserPlansPolicy", "SettingsSfBUserPlansPolicy") %>'></asp:HyperLink>
        </li>

        <li>
            <asp:HyperLink ID="lnkFtpPolicy" runat="server" meta:resourcekey="lnkFtpPolicy"
                    Text="FTP Policy" NavigateUrl='<%# GetSettingsLink("FtpPolicy", "SettingsFtpPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkMailPolicy" runat="server" meta:resourcekey="lnkMailPolicy"
                    Text="MAIL Policy" NavigateUrl='<%# GetSettingsLink("MailPolicy", "SettingsMailPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkMsSqlPolicy" runat="server" meta:resourcekey="lnkMsSqlPolicy"
                    Text="MS SQL Policy" NavigateUrl='<%# GetSettingsLink("MsSqlPolicy", "SettingsMsSqlPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkMySqlPolicy" runat="server" meta:resourcekey="lnkMySqlPolicy"
                    Text="MySQL Policy" NavigateUrl='<%# GetSettingsLink("MySqlPolicy", "SettingsMySqlPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkMariaDBPolicy" runat="server" meta:resourcekey="lnkMariaDBPolicy"
                    Text="MariaDB Policy" NavigateUrl='<%# GetSettingsLink("MariaDBPolicy", "SettingsMariaDBPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkSharePointPolicy" runat="server" meta:resourcekey="lnkSharePointPolicy"
                    Text="SharePoint Policy" NavigateUrl='<%# GetSettingsLink("SharePointPolicy", "SettingsSharePointPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkOsPolicy" runat="server" meta:resourcekey="lnkOsPolicy"
                    Text="OperatingSystem Policy" NavigateUrl='<%# GetSettingsLink("OsPolicy", "SettingsOperatingSystemPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkExchangePolicy" runat="server" meta:resourcekey="lnkExchangePolicy"
                    Text="Exchange Server Policy" NavigateUrl='<%# GetSettingsLink("ExchangePolicy", "SettingsExchangePolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkVpsPolicy" runat="server" meta:resourcekey="lnkVpsPolicy"
                    Text="Virtual Private Servers Policy" NavigateUrl='<%# GetSettingsLink("VpsPolicy", "SettingsVpsPolicy") %>'></asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkRdsPolicy" runat="server" meta:resourcekey="lnkRdsPolicy"
                    Text="Remote Desktop Servers Policy" NavigateUrl='<%# GetSettingsLink("RdsPolicy", "SettingsRdsPolicy") %>'></asp:HyperLink>
        </li>
    </ul>
</div>
<div class="card-footer d-flex justify-content-end align-items-center flex-wrap gap-2">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"><i class="bi bi-x-lg">&nbsp;</i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnCancelText"/></asp:LinkButton>
</div>
