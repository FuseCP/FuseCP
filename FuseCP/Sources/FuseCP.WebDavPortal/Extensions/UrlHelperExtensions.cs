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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using FuseCP.EnterpriseServer.Base.HostedSolution;
using FuseCP.WebDav.Core;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDavPortal.UI.Routes;

namespace FuseCP.WebDavPortal.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string GenerateWopiUrl(this IUrlHelper urlHelper, WebDavAccessToken token, string path)
        {
            var urlPart = urlHelper.RouteUrl(OwaRouteNames.CheckFileInfo, new { accessTokenId = token.Id });

            return GenerateWopiUrl(urlHelper, token, urlPart, path);
        }

        private static string GenerateWopiUrl(IUrlHelper urlHelper, WebDavAccessToken token, string urlPart, string path)
        {
            var requestUrl = urlHelper.ActionContext.HttpContext.Request.GetEncodedUrl();
            var url = new Uri(new Uri(requestUrl), urlPart).ToString();

            string wopiSrc = Uri.UnescapeDataString(url);

            return string.Format("{0}&access_token={1}", wopiSrc, token.AccessToken.ToString("N"));
        }
    }
}
