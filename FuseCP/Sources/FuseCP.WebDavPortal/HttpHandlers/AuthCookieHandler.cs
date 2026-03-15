// Copyright (C) 2026 FuseCP
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
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Interfaces.Security;
using FuseCP.WebDav.Core.Security.Authentication.Principals;
using FuseCP.WebDav.Core.Security.Cryptography;

namespace FuseCP.WebDavPortal.HttpHandlers
{
    public class AuthCookieHandler
    {
        private const string AuthCookieName = "FuseCP.WebDav.Auth";
        private static readonly PathString OwaPrefix = new PathString("/owa");
        private readonly RequestDelegate _next;

        public AuthCookieHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthenticationService authenticationService, ICryptography cryptography)
        {
            if (!context.Request.Path.StartsWithSegments(OwaPrefix)
                && context.Request.Cookies.TryGetValue(AuthCookieName, out var cookieValue)
                && !string.IsNullOrWhiteSpace(cookieValue))
            {
                TryAuthenticateFromCookie(cookieValue, authenticationService, cryptography);

                if (!IsAjaxRequest(context.Request))
                {
                    SetAuthenticationExpirationTicket(context);
                }
            }

            await _next(context);
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers.TryGetValue("X-Requested-With", out var requestedWith)
                   && string.Equals(requestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        }

        private static void SetAuthenticationExpirationTicket(HttpContext context)
        {
            var expirationDateTimeInUtc = DateTime.UtcNow.AddMinutes(30).AddSeconds(1);
            var cookieValue = ((long)(expirationDateTimeInUtc - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();

            context.Response.Cookies.Append(WebDavAppConfigManager.Instance.AuthTimeoutCookieName, cookieValue, new CookieOptions
            {
                HttpOnly = false,
                IsEssential = true,
                Secure = context.Request.IsHttps,
                Path = "/"
            });
        }

        private static void TryAuthenticateFromCookie(string cookieValue, IAuthenticationService authenticationService, ICryptography cryptography)
        {
            try
            {
                var userData = cryptography.Decrypt(cookieValue);
                var principalSerialized = JsonSerializer.Deserialize<ScpPrincipal>(userData);

                if (!string.IsNullOrWhiteSpace(principalSerialized?.Login)
                    && !string.IsNullOrWhiteSpace(principalSerialized.EncryptedPassword))
                {
                    var password = cryptography.Decrypt(principalSerialized.EncryptedPassword);
                    authenticationService.LogIn(principalSerialized.Login, password);
                }
            }
            catch
            {
                // Ignore malformed/expired cookies and continue request pipeline.
            }
        }
    }
}
