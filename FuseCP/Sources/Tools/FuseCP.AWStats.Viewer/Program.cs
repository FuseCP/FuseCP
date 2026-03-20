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
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FuseCP.AWStats.Viewer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/";
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.Redirect("/");
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", async context =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        string identity = context.User.Identity?.Name ?? string.Empty;
        string domain = identity.Split('=')[0];
        string requestConfig = context.Request.Query["config"].ToString();

        if (!string.Equals(requestConfig, domain, StringComparison.OrdinalIgnoreCase))
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            string rqDomain = context.Request.Query["domain"].ToString();
            string rqUsername = context.Request.Query["username"].ToString();
            string rqPassword = context.Request.Query["password"].ToString();

            if (!string.IsNullOrEmpty(rqDomain) && !string.IsNullOrEmpty(rqUsername) && !string.IsNullOrEmpty(rqPassword))
            {
                await TryLogin(context, rqDomain, rqUsername, rqPassword);
                return;
            }

            context.Response.Redirect(context.Request.Path);
            return;
        }

        string awStatsUrl = GetAWStatsUrl(context);
        string queryParams = context.Request.QueryString.Value ?? string.Empty;
        string page = await GetWebDocument(context, awStatsUrl + queryParams);
        string scriptName = GetAWStatsScript();
        string localPath = context.Request.Path.HasValue ? context.Request.Path.Value! : "/";

        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(page.Replace(scriptName, localPath));
        return;
    }

    string autoDomain = context.Request.Query["domain"].ToString();
    if (string.IsNullOrEmpty(autoDomain))
    {
        autoDomain = context.Request.Query["config"].ToString();
    }

    string autoUser = context.Request.Query["username"].ToString();
    string autoPass = context.Request.Query["password"].ToString();

    if (!string.IsNullOrEmpty(autoDomain) && !string.IsNullOrEmpty(autoUser) && !string.IsNullOrEmpty(autoPass))
    {
        await TryLogin(context, autoDomain, autoUser, autoPass);
        return;
    }

    await WriteLoginPage(context, autoDomain, autoUser, null);
});

app.MapPost("/login", async context =>
{
    var form = await context.Request.ReadFormAsync();
    string domain = form["domain"].ToString().Trim();
    string username = form["username"].ToString().Trim();
    string password = form["password"].ToString();

    if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        await WriteLoginPage(context, domain, username, "Domain, username and password are required.");
        return;
    }

    await TryLogin(context, domain, username, password);
});

app.Run();

static async Task TryLogin(HttpContext context, string domain, string username, string password)
{
    FuseCP.AWStats.Viewer.AuthenticationResult result = FuseCP.AWStats.Viewer.AuthenticationProvider.Instance.AuthenticateUser(domain, username, password);
    if (result == FuseCP.AWStats.Viewer.AuthenticationResult.OK)
    {
        var claims = new[] { new Claim(ClaimTypes.Name, domain + "=" + username) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        context.Response.Redirect("/?config=" + Uri.EscapeDataString(domain));
        return;
    }

    string message = ConfigurationManager.AppSettings["AWStats.Message.DomainNotFound"] ?? "Domain not found.";
    if (result == FuseCP.AWStats.Viewer.AuthenticationResult.WrongUsername)
        message = ConfigurationManager.AppSettings["AWStats.Message.WrongUsername"] ?? "Wrong username.";
    else if (result == FuseCP.AWStats.Viewer.AuthenticationResult.WrongPassword)
        message = ConfigurationManager.AppSettings["AWStats.Message.WrongPassword"] ?? "Wrong password.";

    await WriteLoginPage(context, domain, username, message);
}

static async Task<string> GetWebDocument(HttpContext context, string url)
{
    using var handler = new HttpClientHandler();
    string configuredUser = ConfigurationManager.AppSettings["AWStats.Username"] ?? string.Empty;
    if (!string.IsNullOrWhiteSpace(configuredUser))
    {
        string configuredPassword = ConfigurationManager.AppSettings["AWStats.Password"] ?? string.Empty;
        string configuredDomain = string.Empty;
        int sepIdx = configuredUser.IndexOf("\\", StringComparison.Ordinal);
        if (sepIdx != -1)
        {
            configuredDomain = configuredUser.Substring(0, sepIdx);
            configuredUser = configuredUser.Substring(sepIdx + 1);
        }

        handler.Credentials = new NetworkCredential(configuredUser, configuredPassword, configuredDomain);
    }
    else
    {
        handler.UseDefaultCredentials = true;
    }

    using var client = new HttpClient(handler);
    string lang = context.Request.Headers.AcceptLanguage.ToString();
    if (!string.IsNullOrEmpty(lang))
    {
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", lang);
    }

    return await client.GetStringAsync(url);
}

static string GetAWStatsUrl(HttpContext context)
{
    string awStatsUrl = ConfigurationManager.AppSettings["AWStats.URL"] ?? string.Empty;
    if (awStatsUrl.IndexOf(":", StringComparison.Ordinal) == -1)
    {
        string appUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}";
        awStatsUrl = appUrl + awStatsUrl;
    }

    return awStatsUrl;
}

static string GetAWStatsScript()
{
    string awStatsUrl = ConfigurationManager.AppSettings["AWStats.URL"] ?? string.Empty;
    int idx = awStatsUrl.LastIndexOf("/", StringComparison.Ordinal);
    return idx == -1 ? awStatsUrl : awStatsUrl.Substring(idx + 1);
}

static async Task WriteLoginPage(HttpContext context, string domain, string username, string message)
{
    context.Response.ContentType = "text/html; charset=utf-8";
    var sb = new StringBuilder();
    sb.Append("<!doctype html><html><head><meta charset=\"utf-8\"><title>Advanced Statistics</title>");
    sb.Append("<style>body{margin:0;font:13px verdana,arial,sans-serif}.aws_border{border:2px solid #CCCCDD;margin:100px auto 0;width:320px}.aws_title{font-weight:bold;background:#CCCCDD;text-align:center;padding:6px}.aws_data{padding:10px}.field{margin:8px 0}.field input{width:100%;padding:6px}.btn{padding:8px 10px;border:1px solid #ccd7e0;background:#f4f4f4}.msg{color:red;text-align:center;min-height:18px}</style></head><body>");
    sb.Append("<form method=\"post\" action=\"/login\"><div class=\"aws_border\"><div class=\"aws_title\">Login to Advanced Statistics</div><div class=\"aws_data\">");
    sb.Append("<div class=\"msg\">" + WebUtility.HtmlEncode(message ?? string.Empty) + "</div>");
    sb.Append("<div class=\"field\"><label>Domain</label><input name=\"domain\" value=\"").Append(WebUtility.HtmlEncode(domain ?? string.Empty)).Append("\"></div>");
    sb.Append("<div class=\"field\"><label>Username</label><input name=\"username\" value=\"").Append(WebUtility.HtmlEncode(username ?? string.Empty)).Append("\"></div>");
    sb.Append("<div class=\"field\"><label>Password</label><input type=\"password\" name=\"password\"></div>");
    sb.Append("<div style=\"text-align:center\"><button class=\"btn\" type=\"submit\">Display Statistics</button></div>");
    sb.Append("</div></div></form></body></html>");
    await context.Response.WriteAsync(sb.ToString());
}
