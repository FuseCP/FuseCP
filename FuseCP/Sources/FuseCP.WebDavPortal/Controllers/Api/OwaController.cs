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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Cobalt;
using FuseCP.EnterpriseServer.Base.HostedSolution;
using FuseCP.WebDav.Core;
using FuseCP.WebDav.Core.Client;
using FuseCP.WebDav.Core.Entities.Owa;
using FuseCP.WebDav.Core.Interfaces.Managers;
using FuseCP.WebDav.Core.Interfaces.Owa;
using FuseCP.WebDav.Core.Interfaces.Security;
using FuseCP.WebDav.Core.Security.Cryptography;
using FuseCP.WebDav.Core.Scp.Framework;
using FuseCP.WebDavPortal.Configurations.ActionSelectors;
using FuseCP.WebDavPortal.Extensions;
using FuseCP.WebDavPortal.UI.Routes;
using FuseCP.WebDav.Core.Extensions;

namespace FuseCP.WebDavPortal.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("owa/wopi/files/{accessTokenId:int}")]
    public class OwaController : ControllerBase
    {
        private readonly IWopiServer _wopiServer;
        private readonly IWebDavManager _webDavManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccessTokenManager _tokenManager;
        private readonly ICryptography _cryptography;
        private readonly ICobaltManager _cobaltManager;

        public OwaController(IWopiServer wopiServer, IWebDavManager webDavManager, IAuthenticationService authenticationService, IAccessTokenManager tokenManager, ICryptography cryptography, ICobaltManager cobaltManager)
        {
            _wopiServer = wopiServer;
            _webDavManager = webDavManager;
            _authenticationService = authenticationService;
            _tokenManager = tokenManager;
            _cryptography = cryptography;
            _cobaltManager = cobaltManager;
        }

        [HttpGet]
        public CheckFileInfo CheckFileInfo(int accessTokenId)
        {
            var token = _tokenManager.GetToken(accessTokenId);

            var fileInfo = _wopiServer.GetCheckFileInfo(token);

            if (fileInfo.Size <= 1)
            {
                return fileInfo;
            }

            var urlPart = Url.RouteUrl(FileSystemRouteNames.ShowContentPath, new { org = ScpContext.User.OrganizationId, pathPart = token.FilePath });
            var url = new Uri(new Uri(Request.GetDisplayUrl()), urlPart).ToString();

            fileInfo.DownloadUrl = url;

            return fileInfo;
        }

        [HttpGet("contents")]
        public IActionResult GetFile(int accessTokenId)
        {
            var bytes = _wopiServer.GetFileBytes(accessTokenId);

            return File(bytes, "application/octet-stream");
        }

        [HttpPost]
        public IActionResult Post(int accessTokenId)
        {
            var operation = OwaActionSelector.ResolveOperation(Request.Headers);

            if (string.IsNullOrWhiteSpace(operation))
            {
                return Cobalt(accessTokenId);
            }

            switch (operation.ToUpperInvariant())
            {
                case "LOCK":
                    return Lock(accessTokenId);
                case "REFRESH_LOCK":
                    return RefreshLock(accessTokenId);
                case "UNLOCK":
                    return Unlock(accessTokenId);
                case "PUT":
                    return Put(accessTokenId);
                case "PUT_RELATIVE":
                    return PutRelative(accessTokenId);
                default:
                    return BadRequest();
            }
        }

        private IActionResult Cobalt(int accessTokenId)
        {
            var responseBatch = _cobaltManager.ProcessRequest(accessTokenId, HttpContext.Request.Body);

            var correlationId = Request.Headers["X-WOPI-CorrelationID"].FirstOrDefault() ?? string.Empty;

            Response.Headers["X-WOPI-CorellationID"] = correlationId;
            Response.Headers["request-id"] = correlationId;

            using (var copy = new MemoryStream())
            {
                responseBatch.CopyTo(copy);
                return File(copy.ToArray(), "application/octet-stream");
            }
        }

        private IActionResult Lock(int accessTokenId)
        {
           // var token = _tokenManager.GetToken(accessTokenId);

            //_webDavManager.LockFile(token.FilePath);

            return Ok();
        }

        private IActionResult RefreshLock(int accessTokenId)
        {
            return Ok();
        }

        private IActionResult Unlock(int accessTokenId)
        {
            return Ok();
        }

        private IActionResult Put(int accessTokenId)
        {
            var token = _tokenManager.GetToken(accessTokenId);

            var bytes = ReadRequestBytes();

            _webDavManager.UploadFile(token.FilePath, bytes);

            return Ok();
        }

        private IActionResult PutRelative(int accessTokenId)
        {
            var result = new PutRelativeFile();

            var token = _tokenManager.GetToken(accessTokenId);

            var newFilePath = string.Empty;

            var target = Request.Headers.ContainsKey("X-WOPI-RelativeTarget")
                ? Request.Headers["X-WOPI-RelativeTarget"].FirstOrDefault()
                : Request.Headers["X-WOPI-SuggestedTarget"].FirstOrDefault();

            bool overwrite = Request.Headers.ContainsKey("X-WOPI-RelativeTarget")
                && Convert.ToBoolean(Request.Headers["X-WOPI-OverwriteRelativeTarget"].FirstOrDefault());

            if (string.IsNullOrEmpty(target))
            {
                return BadRequest();
            }

            if (target.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Count() > 1)
            {
                var fileName = Path.GetFileName(token.FilePath);

                newFilePath = token.FilePath.ReplaceLast(fileName, target);
            }
            else
            {
                newFilePath = Path.ChangeExtension(token.FilePath, target);
            }

            if (overwrite == false && _webDavManager.FileExist(newFilePath))
            {
                return Conflict();
            }

            var bytes = ReadRequestBytes();

            _webDavManager.UploadFile(newFilePath, bytes);

            var newToken = _tokenManager.CreateToken(ScpContext.User,newFilePath);

            var readUrlPart = Url.RouteUrl(FileSystemRouteNames.ViewOfficeOnline, new { org = ScpContext.User.OrganizationId, pathPart = newFilePath});
            var writeUrlPart = Url.RouteUrl(FileSystemRouteNames.EditOfficeOnline, new { org = ScpContext.User.OrganizationId, pathPart = newFilePath });

            result.HostEditUrl = new Uri(new Uri(Request.GetDisplayUrl()), writeUrlPart).ToString();
            result.HostViewUrl = new Uri(new Uri(Request.GetDisplayUrl()), readUrlPart).ToString();
            result.Name = Path.GetFileName(newFilePath);
            result.Url = Url.GenerateWopiUrl(newToken, newFilePath);

            return Ok(result);
        }

        private byte[] ReadRequestBytes()
        {
            using (var memory = new MemoryStream())
            {
                Request.Body.CopyTo(memory);
                return memory.ToArray();
            }
        }
    }
}
