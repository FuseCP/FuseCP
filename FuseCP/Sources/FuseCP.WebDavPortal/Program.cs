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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
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
using FuseCP.WebDav.Core.Scp.Framework;
using FuseCP.WebDav.Core.Services;
using FuseCP.WebDav.Core.Storages;
using FuseCP.WebDavPortal;
using FuseCP.WebDavPortal.App_Start;
using FuseCP.WebDavPortal.HttpHandlers;
using FuseCP.WebDavPortal.Mapping;
using FuseCP.WebDavPortal.ModelBinders.DataTables;

var builder = WebApplication.CreateBuilder(args);

// --- Domain services (replaces Ninject bindings from PortalDependencies) ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<ICryptography, CryptoUtils>();
builder.Services.AddScoped<IAuthenticationService, FormsAuthenticationService>();
builder.Services.AddScoped<IWebDavManager, WebDavManager>();
builder.Services.AddScoped<IAccessTokenManager, AccessTokenManager>();
builder.Services.AddScoped<IWopiServer, WopiServer>();
builder.Services.AddScoped<IWopiFileManager, CobaltSessionManager>();
builder.Services.AddScoped<IWebDavAuthorizationService, WebDavAuthorizationService>();
builder.Services.AddScoped<ICobaltManager, CobaltManager>();
builder.Services.AddSingleton<ITtlStorage, CacheTtlStorage>();
builder.Services.AddScoped<IUserSettingsManager, UserSettingsManager>();
builder.Services.AddScoped<ISmsDistributionService, TwillioSmsDistributionService>();
builder.Services.AddScoped<ISmsAuthenticationService, SmsAuthenticationService>();

// --- MVC + filters + model binders ---
builder.Services.AddControllersWithViews(options =>
{
    FilterConfig.RegisterGlobalFilters(options);
    options.ModelBinderProviders.Insert(0, new JqueryDataTableModelBinderProvider());
}).AddNewtonsoftJson();

// --- Razor views ---
builder.Services.AddRazorPages();

// --- AutoMapper ---
AutoMapperPortalConfiguration.Configure();

// ============================================================
var app = builder.Build();
// ============================================================

FCP.ServiceProvider = app.Services;

app.UseExceptionHandler("/Error/Index");

var contentRoot = app.Environment.ContentRootPath;

var rootAssetExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    ".png",
    ".ico",
    ".json",
    ".svg",
    ".xml"
};

app.Use(async (context, next) =>
{
    var requestPath = context.Request.Path.Value ?? string.Empty;
    if (requestPath.Length > 1
        && requestPath.IndexOf('/', 1) < 0)
    {
        var fileName = requestPath.TrimStart('/');
        var extension = Path.GetExtension(fileName);
        var absolutePath = Path.Combine(contentRoot, fileName);

        if (rootAssetExtensions.Contains(extension) && File.Exists(absolutePath))
        {
            await context.Response.SendFileAsync(absolutePath);
            return;
        }
    }

    await next();
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(contentRoot, "Content")),
    RequestPath = "/Content"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(contentRoot, "Scripts")),
    RequestPath = "/Scripts"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(contentRoot, "fonts")),
    RequestPath = "/fonts"
});

app.UseRouting();
app.UseSession();

// --- Custom middleware (replaces DelegatingHandler / IHttpHandler) ---
app.UseMiddleware<AccessTokenHandler>();
app.UseMiddleware<AuthCookieHandler>();
app.UseMiddleware<FileTransferRequestHandler>();

app.UseAuthentication();
app.UseAuthorization();

// Register routes directly on app (implements IEndpointRouteBuilder)
RouteConfig.RegisterRoutes(app);
WebApiConfig.Register(app);

app.Run();
