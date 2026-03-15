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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using FuseCP.WebDavPortal.UI.Routes;

namespace FuseCP.WebDavPortal.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(IEndpointRouteBuilder endpoints)
        {
            #region Owa

            endpoints.MapControllerRoute(
                name: OwaRouteNames.GetFile,
                pattern: "owa/wopi/files/{accessTokenId}/contents",
                defaults: new {controller = "Owa", action = "GetFile"});

            endpoints.MapControllerRoute(
                name: OwaRouteNames.CheckFileInfo,
                pattern: "owa/wopi/files/{accessTokenId}",
                defaults: new {controller = "Owa", action = "CheckFileInfo"});

            #endregion

            endpoints.MapControllerRoute("API Default", "api/{controller}/{id?}");
        }
    }
}
