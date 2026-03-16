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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Web;
using System.Web.Script;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using System.Security.Principal;
using System.Web.UI;
using System.Net;
using System.Net.Http;
using System.Timers;
using System.Security.Cryptography;
using FuseCP.Portal;
using System.Threading.Tasks;


namespace FuseCP.WebPortal
{
	public class Global : System.Web.HttpApplication
	{
		const bool Debug = false;
		private int keepAliveMinutes = 10;
		private static string keepAliveUrl = "";
		private static System.Timers.Timer timer = null;

		protected void Application_PostAuthorizeRequest(Object sender, EventArgs e)
		{
			if (User.Identity.IsAuthenticated == true && Request.RawUrl.IndexOf("WebResource.axd") == -1)
			{
				FormsAuthenticationTicket authTicket = (FormsAuthenticationTicket)Context.Items[FormsAuthentication.FormsCookieName];

				string roleName = String.Empty;

				if (authTicket == null)
				{
					HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
					if (authCookie != null)
					{
						try
						{
							authTicket = FormsAuthentication.Decrypt(authCookie.Value);
							Context.Items[FormsAuthentication.FormsCookieName] = authTicket;

							int index = authTicket.UserData.IndexOf(Environment.NewLine);

							if (index > -1)
								roleName = authTicket.UserData.Substring(index + Environment.NewLine.Length);
						}
						catch (CryptographicException)
						{
							// Stale/tampered cookie (e.g. machine key rotation): clear and continue as anonymous.
							PortalUtils.InvalidateAuthCookieSafe();
							return;
						}
						catch (ArgumentException)
						{
							PortalUtils.InvalidateAuthCookieSafe();
							return;
						}
						catch
						{
							PortalUtils.InvalidateAuthCookieSafe();
							return;
						}
					}
				}

				string[] roles = null;

				switch (roleName)
				{
					case "Administrator":
						roles = new string[] { "Administrator", "PlatformHelpdesk", "PlatformCSR", "Reseller", "ResellerCSR", "ResellerHelpdesk", "User" };
						break;
					case "Reseller":
						roles = new string[] { "Reseller", "ResellerCSR", "ResellerHelpdesk", "User" };
						break;
					case "PlatformCSR":
						roles = new string[] { "PlatformCSR", "ResellerCSR", "ResellerHelpdesk", "User" };
						break;
					case "PlatformHelpdesk":
						roles = new string[] { "PlatformHelpdesk", "ResellerHelpdesk", "User" };
						break;
					case "ResellerCSR":
						roles = new string[] { "ResellerCSR", "User" };
						break;
					case "ResellerHelpdesk":
						roles = new string[] { "ResellerHelpdesk", "User" };
						break;
					default:
						roles = new string[] { "User" };
						break;
				}

				HttpContext.Current.User = new GenericPrincipal(HttpContext.Current.User.Identity, roles);
			}

		}


		protected void Application_OnStart(object sender, EventArgs e) => Application_Start(sender, e);


        Task TouchTask;
		protected void Application_Start(object sender, EventArgs e)
		{
			if (Debug && !Debugger.IsAttached) Debugger.Launch();

#if NETFRAMEWORK
			Web.Clients.CertificateValidator.Init();
#endif
			// start Enterprise Server
			string serverUrl = PortalConfiguration.SiteSettings["EnterpriseServer"];
			if (serverUrl.StartsWith("http://") || serverUrl.StartsWith("https://"))
			{
				var httpClientHandler = new HttpClientHandler() { AllowAutoRedirect = false };
				using (var client = new HttpClient(httpClientHandler))
					client.GetAsync(serverUrl);
			} else if (!serverUrl.StartsWith("assembly://"))
			{ // Start EnterpriseServer
				var esTestClient = new FuseCP.EnterpriseServer.Client.esTest();
				esTestClient.Url = serverUrl;
				TouchTask = esTestClient.TouchAsync();
			} else
			{
#if NETFRAMEWORK
				Web.Clients.AssemblyLoader.Init();
#endif
			}

			VncWebSocketHandler.Init();

			ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
				new ScriptResourceDefinition
				{
					Path = "~/JavaScript/jquery-2.1.0.min.js"
				}
			);
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			// ASP.NET Integration Mode workaround
			if (String.IsNullOrEmpty(keepAliveUrl))
			{
				// init keep-alive
				keepAliveUrl = HttpContext.Current.Request.Url.ToString();
				if (this.keepAliveMinutes > 0)
				{
					timer = new System.Timers.Timer(60000 * this.keepAliveMinutes);
					timer.Elapsed += new ElapsedEventHandler(KeepAlive);
					timer.AutoReset = true;
					timer.Start();
				}
			}
		}
		protected void Application_End(object sender, EventArgs e)
		{
			Web.Clients.ClientBase.DisposeAllSshTunnels();
			Web.Clients.AssemblyLoader.Dispose();
		}

		private void KeepAlive(Object sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				var httpClientHandler = new HttpClientHandler() { AllowAutoRedirect = false };
				using (var client = new HttpClient(httpClientHandler))
					client.GetAsync(keepAliveUrl);
			}
			catch { }
		}
	}
}
