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
using FuseCP.WebDav.Core.Config.Entities;
using FuseCP.WebDavPortal.WebConfigSections;

namespace FuseCP.WebDav.Core.Config
{
    public class WebDavAppConfigManager : IWebDavAppConfig
    {
        private static WebDavAppConfigManager _instance;
        private readonly WebDavExplorerConfigurationSettingsSection _configSection;

        private WebDavAppConfigManager()
        {
            _configSection = WebDavConfigSectionResolver.GetRequiredSection();
            FuseCPConstantUserParameters = new FuseCPConstantUserParameters();
            ElementsRendering = new ElementsRendering();
            SessionKeys = new SessionKeysCollection();
            FileIcons = new FileIconsDictionary();
            HttpErrors = new HttpErrorsCollection();
            OfficeOnline = new OfficeOnlineCollection();
            OwaSupportedBrowsers = new OwaSupportedBrowsersCollection();
            FilesToIgnore = new FilesToIgnoreCollection();
            FileOpener = new OpenerCollection();
            TwilioParameters = new TwilioParameters();
        }

        public static WebDavAppConfigManager Instance
        {
            get { return _instance ?? (_instance = new WebDavAppConfigManager()); }
        }

        public string UserDomain
        {
            get { return _configSection.UserDomain.Value; }
        }

        public string WebdavRoot
        {
            get { return _configSection.WebdavRoot.Value; }
        }

        public string ApplicationName
        {
            get { return _configSection.ApplicationName.Value; }
        }

        public string AuthTimeoutCookieName
        {
            get { return _configSection.AuthTimeoutCookieName.Value; }
        }

        public string EnterpriseServerUrl
        {
            get { return _configSection.EnterpriseServerUrl.Value; }
        }

        public ElementsRendering ElementsRendering { get; private set; }
        public FuseCPConstantUserParameters FuseCPConstantUserParameters { get; private set; }
        public TwilioParameters TwilioParameters { get; private set; }
        public SessionKeysCollection SessionKeys { get; private set; }
        public FileIconsDictionary FileIcons { get; private set; }
        public HttpErrorsCollection HttpErrors { get; private set; }
        public OfficeOnlineCollection OfficeOnline { get; private set; }
        public OwaSupportedBrowsersCollection OwaSupportedBrowsers { get; private set; }
        public FilesToIgnoreCollection FilesToIgnore { get; private set; }
        public OpenerCollection FileOpener { get; private set; }
    }
}
