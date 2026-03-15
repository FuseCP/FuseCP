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

#if NETFRAMEWORK
using System;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.SessionState;
using AutoMapper;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Interfaces.Security;
using FuseCP.WebDav.Core.Security.Authentication.Principals;
using FuseCP.WebDav.Core.Security.Cryptography;
using FuseCP.WebDavPortal.App_Start;
using FuseCP.WebDavPortal.Controllers;
using FuseCP.WebDavPortal.CustomAttributes;
using FuseCP.WebDavPortal.DependencyInjection;
using FuseCP.WebDavPortal.HttpHandlers;
using FuseCP.WebDavPortal.Mapping;
using FuseCP.Server.Utils;

namespace FuseCP.WebDavPortal
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Log.WriteStart("Application_Start");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.MessageHandlers.Add(new AccessTokenHandler());

            DependencyResolver.SetResolver(new NinjectDependecyResolver());

            AutoMapperPortalConfiguration.Configure();
            
            Mapper.AssertConfigurationIsValid();

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
               typeof(PhoneNumberAttribute),
               typeof(RegularExpressionAttributeAdapter));

            Log.WriteEnd("Application_Start");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception lastError = Server.GetLastError();
            Server.ClearError();

            int statusCode;

            if (lastError.GetType() == typeof (HttpException))
                statusCode = ((HttpException) lastError).GetHttpCode();
            else
                statusCode = 500;

            var contextWrapper = new HttpContextWrapper(Context);

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "Index");
            routeData.Values.Add("statusCode", statusCode);
            routeData.Values.Add("exception", lastError);
            routeData.Values.Add("isAjaxRequet", contextWrapper.Request.IsAjaxRequest());

            IController controller = new ErrorController();
            var requestContext = new RequestContext(contextWrapper, routeData);
            controller.Execute(requestContext);
            Response.End();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var s = HttpContext.Current.Request;
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            Log.WriteStart("Application_PostAuthenticateRequest");

            if (!IsOwaRequest())
            {
                Log.WriteInfo("Try get HttpContext ...");
                var contextWrapper = new HttpContextWrapper(Context);

                Log.WriteInfo("Try get Auth-Cookie ...");
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

                Log.WriteInfo("Try get Auth-Servive ...");
                var authService = DependencyResolver.Current.GetService<IAuthenticationService>();

                Log.WriteInfo("Try get Crypto-Service ...");
                var cryptography = DependencyResolver.Current.GetService<ICryptography>();

                if (authCookie != null)
                {
                    Log.WriteInfo("Found Auth-Cookie!");
                    Log.WriteInfo("Try to Decrpyt ...");
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    Log.WriteInfo("Try to get UserData from Auth-Cookie");
                    var serializer = new JavaScriptSerializer();
                    var principalSerialized = serializer.Deserialize<ScpPrincipal>(authTicket.UserData);

                    Log.WriteInfo("Try to Login ...");
                    authService.LogIn(principalSerialized.Login, cryptography.Decrypt(principalSerialized.EncryptedPassword));

                    if (!contextWrapper.Request.IsAjaxRequest())
                    {
                        SetAuthenticationExpirationTicket();
                    }
                }
                else
                {
                    Log.WriteWarning("Auth-Cookie is null");
                }
            }
            else
            {
                Log.WriteInfo("Is OWA Request!");
            }

            Log.WriteEnd("Application_PostAuthenticateRequest");
        }



        private bool IsOwaRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/owa");
        }

        public static void SetAuthenticationExpirationTicket()
        {
            Log.WriteStart("SetAuthenticationExpirationTicket");

            var expirationDateTimeInUtc = DateTime.UtcNow.AddMinutes(FormsAuthentication.Timeout.TotalMinutes).AddSeconds(1);
            var authenticationExpirationTicketCookie = new HttpCookie(WebDavAppConfigManager.Instance.AuthTimeoutCookieName);
            
            authenticationExpirationTicketCookie.Value = expirationDateTimeInUtc.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString("F0");
            authenticationExpirationTicketCookie.HttpOnly = false; 
            authenticationExpirationTicketCookie.Secure = FormsAuthentication.RequireSSL;

            HttpContext.Current.Response.Cookies.Add(authenticationExpirationTicketCookie);

            Log.WriteEnd("SetAuthenticationExpirationTicket");
        }
    }
}
#endif
