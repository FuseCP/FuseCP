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

using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization.Json;
using System.Text;
using Cobalt;
using FuseCP.EnterpriseServer.Base.HostedSolution;
using FuseCP.WebDav.Core.Client;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Entities.Owa;
using FuseCP.WebDav.Core.Interfaces.Managers;
using FuseCP.WebDav.Core.Interfaces.Owa;
using FuseCP.WebDav.Core.Interfaces.Security;
using FuseCP.WebDav.Core.Security.Authentication.Principals;
using FuseCP.WebDav.Core.Security.Authorization.Enums;

namespace FuseCP.WebDav.Core.Owa
{
    public class WopiServer : IWopiServer
    {
        private readonly IWebDavManager _webDavManager;
        private readonly IAccessTokenManager _tokenManager;
        private readonly IWebDavAuthorizationService _webDavAuthorizationService;
        private readonly IWopiFileManager _fileManager;


        public WopiServer(IWebDavManager webDavManager, IAccessTokenManager tokenManager, IWebDavAuthorizationService webDavAuthorizationService, IWopiFileManager fileManager)
        {
            _webDavManager = webDavManager;
            _tokenManager = tokenManager;
            _webDavAuthorizationService = webDavAuthorizationService;
            _fileManager = fileManager;
        }

        public CheckFileInfo GetCheckFileInfo(WebDavAccessToken token)
        {
            var resource = _webDavManager.GetResource(token.FilePath);

            var permissions = _webDavAuthorizationService.GetPermissions(ScpContext.User, token.FilePath);

            var readOnly = permissions.HasFlag(WebDavPermissions.Write) == false || permissions.HasFlag(WebDavPermissions.OwaEdit) == false;

            var cFileInfo = new CheckFileInfo
            {
                BaseFileName = resource == null ? token.FilePath.Split('/').Last() : resource.DisplayName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(),
                OwnerId = ScpContext.User.Login,
                Size = resource == null ? 0 : resource.ContentLength,
                Version = DateTime.Now.ToString("s"),
                SupportsCoauth = true,
                SupportsCobalt = true,
                SupportsFolders = true,
                SupportsLocks = true,
                SupportsScenarioLinks = false,
                SupportsSecureStore = false,
                SupportsUpdate = true,
                UserCanWrite = !readOnly,
                ReadOnly = readOnly,
                RestrictedWebViewOnly = false,
                CloseButtonClosesWindow = true
            };

            if (resource != null)
            {
                cFileInfo.ClientUrl = _webDavManager.GetFileUrl(token.FilePath);
            }

            return cFileInfo;
        }

        public byte[] GetFileBytes(int accessTokenId)
        {
            var token = _tokenManager.GetToken(accessTokenId);

            if (_webDavManager.FileExist(token.FilePath))
            {
                return _webDavManager.GetFileBytes(token.FilePath);
            }

            var cobaltFile = _fileManager.Get(token.FilePath) ?? _fileManager.Create(accessTokenId);

            var stream = new MemoryStream();

            new GenericFda(cobaltFile.CobaltEndpoint, null).GetContentStream().CopyTo(stream);

            return stream.ToArray();
        }
    }
}
