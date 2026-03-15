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

using Ninject;
using FuseCP.WebDav.Core.Interfaces.Managers;
using FuseCP.WebDav.Core.Interfaces.Managers.Users;
using FuseCP.WebDav.Core.Interfaces.Owa;
using FuseCP.WebDav.Core.Interfaces.Security;
using FuseCP.WebDav.Core.Interfaces.Services;
using FuseCP.WebDav.Core.Interfaces.Storages;
using FuseCP.WebDav.Core.Managers;
using FuseCP.WebDav.Core.Managers.Users;
using FuseCP.WebDav.Core.Owa;
using FuseCP.WebDav.Core.Security.Authentication;
using FuseCP.WebDav.Core.Security.Authorization;
using FuseCP.WebDav.Core.Security.Cryptography;
using FuseCP.WebDav.Core.Services;
using FuseCP.WebDav.Core.Storages;
#if NETFRAMEWORK
using FuseCP.WebDavPortal.DependencyInjection.Providers;
#endif

namespace FuseCP.WebDavPortal.DependencyInjection
{
    public class PortalDependencies
    {
        public static void Configure(IKernel kernel)
        {
#if NETFRAMEWORK
            kernel.Bind<HttpSessionState>().ToProvider<HttpSessionStateProvider>();
#endif
            kernel.Bind<ICryptography>().To<CryptoUtils>();
            kernel.Bind<IAuthenticationService>().To<FormsAuthenticationService>();
            kernel.Bind<IWebDavManager>().To<WebDavManager>();
            kernel.Bind<IAccessTokenManager>().To<AccessTokenManager>();
            kernel.Bind<IWopiServer>().To<WopiServer>();
            kernel.Bind<IWopiFileManager>().To<CobaltSessionManager>();
            kernel.Bind<IWebDavAuthorizationService>().To<WebDavAuthorizationService>();
            kernel.Bind<ICobaltManager>().To<CobaltManager>();
            kernel.Bind<ITtlStorage>().To<CacheTtlStorage>();
            kernel.Bind<IUserSettingsManager>().To<UserSettingsManager>();
            kernel.Bind<ISmsDistributionService>().To<TwillioSmsDistributionService>();
            kernel.Bind<ISmsAuthenticationService>().To<SmsAuthenticationService>();
        }
    }
}
