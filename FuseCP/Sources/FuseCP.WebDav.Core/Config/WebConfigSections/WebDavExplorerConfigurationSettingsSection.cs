// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Configuration;
using FuseCP.WebDav.Core.Config.WebConfigSections;

namespace FuseCP.WebDavPortal.WebConfigSections
{
    public class WebDavExplorerConfigurationSettingsSection : ConfigurationSection
    {
        private const string UserDomainKey = "userDomain";
        private const string WebdavRootKey = "webdavRoot";
        private const string AuthTimeoutCookieNameKey = "authTimeoutCookieName";
        private const string AppName = "applicationName";
        private const string EnterpriseServerUrlNameKey = "enterpriseServer";
        private const string FuseCPConstantUserKey = "FuseCPConstantUser";
        private const string ElementsRenderingKey = "elementsRendering";
        private const string Rfc2898CryptographyKey = "rfc2898Cryptography";
        private const string ConnectionStringsKey = "appConnectionStrings";
        private const string SessionKeysKey = "sessionKeys";
        private const string FileIconsKey = "fileIcons";
        private const string OwaSupportedBrowsersKey = "owaSupportedBrowsers";
        private const string OfficeOnlineKey = "officeOnline";
        private const string FilesToIgnoreKey = "filesToIgnore";
        private const string TypeOpenerKey = "typeOpener";
        private const string TwilioKey = "twilio";

        public const string SectionName = "webDavExplorerConfigurationSettings";

        [ConfigurationProperty(AuthTimeoutCookieNameKey, IsRequired = true)]
        public AuthTimeoutCookieNameElement AuthTimeoutCookieName
        {
            get { return (AuthTimeoutCookieNameElement)this[AuthTimeoutCookieNameKey]; }
            set { this[AuthTimeoutCookieNameKey] = value; }
        }

        [ConfigurationProperty(EnterpriseServerUrlNameKey, IsRequired = true)]
        public EnterpriseServerElement EnterpriseServerUrl
        {
            get { return (EnterpriseServerElement)this[EnterpriseServerUrlNameKey]; }
            set { this[EnterpriseServerUrlNameKey] = value; }
        }

        [ConfigurationProperty(WebdavRootKey, IsRequired = true)]
        public WebdavRootElement WebdavRoot
        {
            get { return (WebdavRootElement)this[WebdavRootKey]; }
            set { this[WebdavRootKey] = value; }
        }

        [ConfigurationProperty(UserDomainKey, IsRequired = true)]
        public UserDomainElement UserDomain
        {
            get { return (UserDomainElement) this[UserDomainKey]; }
            set { this[UserDomainKey] = value; }
        }

        [ConfigurationProperty(AppName, IsRequired = true)]
        public ApplicationNameElement ApplicationName
        {
            get { return (ApplicationNameElement)this[AppName]; }
            set { this[AppName] = value; }
        }

        [ConfigurationProperty(FuseCPConstantUserKey, IsRequired = true)]
        public FuseCPConstantUserElement FuseCPConstantUser
        {
            get { return (FuseCPConstantUserElement)this[FuseCPConstantUserKey]; }
            set { this[FuseCPConstantUserKey] = value; }
        }

        [ConfigurationProperty(TwilioKey, IsRequired = false)]
        public TwilioElement Twilio
        {
            get { return (TwilioElement)this[TwilioKey]; }
            set { this[TwilioKey] = value; }
        }

        [ConfigurationProperty(ElementsRenderingKey, IsRequired = true)]
        public ElementsRenderingElement ElementsRendering
        {
            get { return (ElementsRenderingElement)this[ElementsRenderingKey]; }
            set { this[ElementsRenderingKey] = value; }
        }

        [ConfigurationProperty(SessionKeysKey, IsDefaultCollection = false)]
        public SessionKeysElementCollection SessionKeys
        {
            get { return (SessionKeysElementCollection) this[SessionKeysKey]; }
            set { this[SessionKeysKey] = value; }
        }

        [ConfigurationProperty(FileIconsKey, IsDefaultCollection = false)]
        public FileIconsElementCollection FileIcons
        {
            get { return (FileIconsElementCollection) this[FileIconsKey]; }
            set { this[FileIconsKey] = value; }
        }

        [ConfigurationProperty(OwaSupportedBrowsersKey, IsDefaultCollection = false)]
        public OwaSupportedBrowsersElementCollection OwaSupportedBrowsers
        {
            get { return (OwaSupportedBrowsersElementCollection)this[OwaSupportedBrowsersKey]; }
            set { this[OwaSupportedBrowsersKey] = value; }
        }

        [ConfigurationProperty(OfficeOnlineKey, IsDefaultCollection = false)]
        public OfficeOnlineElementCollection OfficeOnline
        {
            get { return (OfficeOnlineElementCollection)this[OfficeOnlineKey]; }
            set { this[OfficeOnlineKey] = value; }
        }

        [ConfigurationProperty(TypeOpenerKey, IsDefaultCollection = false)]
        public OpenerElementCollection TypeOpener
        {
            get { return (OpenerElementCollection)this[TypeOpenerKey]; }
            set { this[TypeOpenerKey] = value; }
        }

        [ConfigurationProperty(FilesToIgnoreKey, IsDefaultCollection = false)]
        public FilesToIgnoreElementCollection FilesToIgnore
        {
            get { return (FilesToIgnoreElementCollection)this[FilesToIgnoreKey]; }
            set { this[FilesToIgnoreKey] = value; }
        }
    }
}
