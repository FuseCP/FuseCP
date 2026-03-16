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
using System.Xml;
using System.Security.Principal;
using System.Resources;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.Hosting;
using System.Net;
using MimeKit;
using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using System.Security.Cryptography;

using FuseCP.EnterpriseServer;
using FuseCP.EnterpriseServer.Client;
using FuseCP.WebPortal;
using System.Collections;
using System.Security.Authentication;

namespace FuseCP.Portal;


public class PortalUtils
{
	public const string SharedResourcesFile = "SharedResources.ascx.resx";
	public const string CONFIG_FOLDER = "~/App_Data/";
	public const string SUPPORTED_LOCALES_FILE = "SupportedLocales.config";
	public const string EXCHANGE_SERVER_HIERARCHY_FILE = "ESModule_ControlsHierarchy.config";
	public const string USER_ID_PARAM = "UserID";
	public const string SPACE_ID_PARAM = "SpaceID";
	public const string SEARCH_QUERY_PARAM = "Query";


	public static string CultureCookieName
	{
		get { return PortalConfiguration.SiteSettings["CultureCookieName"]; }
	}

	public static string ThemeCookieName
	{
		get { return PortalConfiguration.SiteSettings["ThemeCookieName"]; }
	}

	public static System.Globalization.CultureInfo CurrentCulture
	{
		get { return GetCurrentCulture(); }
	}

	public static System.Globalization.CultureInfo CurrentUICulture
	{
		get { return GetCurrentCulture(); }
	}

	public static string CurrentTheme
	{
		get { return GetCurrentTheme(); }
	}

	public static string CurrentThemeStyle
	{
		get { return GetCurrentThemeStyle(); }
	}

	internal static string GetCurrentTheme()
	{
		string theme = (string)HttpContext.Current.Items[ThemeCookieName];

		if (theme == null)
		{
			HttpCookie cookie = HttpContext.Current.Request.Cookies[ThemeCookieName];
			if (cookie != null)
			{
				theme = cookie.Value;

				if (!String.IsNullOrEmpty(theme))
				{
					HttpContext.Current.Items[ThemeCookieName] = theme;
					return theme;
				}
			}
		}
		return theme;
	}

	public static void SetCurrentTheme(string theme)
	{
		// theme
		if (!String.IsNullOrEmpty(theme))
		{

			HttpCookie cookieTheme = new HttpCookie(ThemeCookieName, theme);
			cookieTheme.Expires = DateTime.Now.AddMonths(2);
			HttpContext.Current.Response.Cookies.Add(cookieTheme);
		}
	}

	internal static string GetCurrentThemeStyle()
	{
		string themeStyle = (string)HttpContext.Current.Items["UserThemeStyle"];

		if (themeStyle == null)
		{
			HttpCookie cookie = HttpContext.Current.Request.Cookies["UserThemeStyle"];
			if (cookie != null)
			{
				themeStyle = cookie.Value;

				if (!String.IsNullOrEmpty(themeStyle))
				{
					HttpContext.Current.Items["UserThemeStyle"] = themeStyle;
					return themeStyle;
				}
			}
		}
		return themeStyle;
	}

	internal static System.Globalization.CultureInfo GetCurrentCulture()
	{
		System.Globalization.CultureInfo ci = (System.Globalization.CultureInfo)
				HttpContext.Current.Items[CultureCookieName];

		if (ci == null)
		{
			HttpCookie localeCrumb = HttpContext.Current.Request.Cookies[CultureCookieName];
			if (localeCrumb != null)
			{
				ci = System.Globalization.CultureInfo.CreateSpecificCulture(localeCrumb.Value);

				if (ci != null)
				{
					HttpContext.Current.Items[CultureCookieName] = ci;
					return ci;
				}
			}
		}
		else
			return ci;

		return System.Threading.Thread.CurrentThread.CurrentCulture;
	}

	public static bool ParseBool(string val, bool defaultValue)
	{
		bool result = defaultValue;
		// Perf: allow only non-empty strings to go through
		if (!String.IsNullOrEmpty(val))
		{
			try
			{
				result = Boolean.Parse(val);
			}
			catch
			{
				/* do nothing */
			}
		}

		return result;
	}

	public static string AdminEmail
	{
		get { return PortalConfiguration.SiteSettings["AdminEmail"]; }
	}

	public static string FromEmail
	{
		get { return PortalConfiguration.SiteSettings["FromEmail"]; }
	}

	public static void SendMail(string from, string to, string bcc, string subject, string body, bool isBodyHtml = false)
	{
		using (var client = new SmtpClient())
		{
			// set SMTP client settings
			string smtpServer = PortalConfiguration.SiteSettings["SmtpHost"];
			int smtpPort = Int32.Parse(PortalConfiguration.SiteSettings["SmtpPort"]);
			string smtpUsername = PortalConfiguration.SiteSettings["SmtpUsername"];
			string smtpPassword = PortalConfiguration.SiteSettings["SmtpPassword"];
			bool enableSsl = ParseBool(PortalConfiguration.SiteSettings["SmtpEnableSsl"], false);
			bool enableLegacySSL = ParseBool(PortalConfiguration.SiteSettings["SmtpEnableLegacySSL"], false);

			// Determine SecureSocketOptions based on EnableSsl and Port
			// Common logic: Port 465 usually implies SslOnConnect, others often use StartTls. Adjust as needed.
			SecureSocketOptions secureSocketOptions = SecureSocketOptions.Auto;
			if (enableSsl)
			{
				secureSocketOptions = smtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
			}

			if (enableLegacySSL)
			{
				client.SslProtocols = SslProtocols.None;
			}
			else
			{
				client.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
			}

			client.Connect(smtpServer, smtpPort, secureSocketOptions);

			if (String.IsNullOrEmpty(smtpUsername))
			{
				client.Authenticate(smtpUsername, smtpPassword);
			}

			// create message
			var message = new MimeMessage();
			message.From.Add(MailboxAddress.Parse(from));
			message.To.Add(MailboxAddress.Parse(to));
			if (!String.IsNullOrEmpty(bcc))
				message.Bcc.Add(MailboxAddress.Parse(bcc));
			message.Subject = subject;
			message.Priority = MessagePriority.Urgent;

			// Create body
			var bodyBuilder = new BodyBuilder();
			if (isBodyHtml) bodyBuilder.HtmlBody = body;
			else bodyBuilder.TextBody = body;
			message.Body = bodyBuilder.ToMessageBody();

			// send message
			try
			{
				client.Send(message);
			}
			finally
			{
				client.Disconnect(true);
			}
		}
	}

	public static void UserSignOut()
	{
		FormsAuthentication.SignOut();

		if (HttpContext.Current.Session != null)
		{
			HttpContext.Current.Session.Clear();
			HttpContext.Current.Session.Abandon();
		}

		// Clear authentication cookie 
		HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
		rFormsCookie.Expires = DateTime.Now.AddYears(-1);
		HttpContext.Current.Response.Cookies.Add(rFormsCookie);

		// Clear session cookie  
		HttpCookie rSessionCookie = new HttpCookie("ASP.NET_SessionId", "");
		rSessionCookie.Expires = DateTime.Now.AddYears(-1);
		HttpContext.Current.Response.Cookies.Add(rSessionCookie);

		HttpContext.Current.Response.Redirect(LoginRedirectUrl);
	}
	public static void UserSignOutOnly()
	{
		FormsAuthentication.SignOut();

		if (HttpContext.Current.Session != null)
		{
			HttpContext.Current.Session.Clear();
			HttpContext.Current.Session.Abandon();
		}

		// Clear authentication cookie 
		HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
		rFormsCookie.Expires = DateTime.Now.AddYears(-1);
		HttpContext.Current.Response.Cookies.Add(rFormsCookie);

		// Clear session cookie  
		HttpCookie rSessionCookie = new HttpCookie("ASP.NET_SessionId", "");
		rSessionCookie.Expires = DateTime.Now.AddYears(-1);
		HttpContext.Current.Response.Cookies.Add(rSessionCookie);
	}

	public static void InvalidateAuthCookieSafe()
	{
		try
		{
			FormsAuthentication.SignOut();

			HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
			rFormsCookie.Expires = DateTime.Now.AddYears(-1);
			HttpContext.Current.Response.Cookies.Add(rFormsCookie);
		}
		catch
		{
			// Never let auth-cookie cleanup fail the request.
		}
	}

	public static MenuItem GetSpaceMenuItem(string menuItemKey)
	{
		MenuItem item = new MenuItem();
		item.Value = menuItemKey;

		menuItemKey = String.Concat("Space", menuItemKey);

		PortalPage page = PortalConfiguration.Site.Pages[menuItemKey];

		if (page != null)
			item.NavigateUrl = DefaultPage.GetPageUrl(menuItemKey);

		return item;
	}

	public static FormsAuthenticationTicket AuthTicket
	{
		get
		{
			FormsAuthenticationTicket authTicket = (FormsAuthenticationTicket)HttpContext.Current.Items[FormsAuthentication.FormsCookieName];

			if (authTicket == null)
			{
				// original code
				HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
				// workaround for cases when AuthTicket is required before round-trip
				if (authCookie == null || String.IsNullOrEmpty(authCookie.Value))
					authCookie = HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName];
				//
				if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
				{
					try
					{
						authTicket = FormsAuthentication.Decrypt(authCookie.Value);
						HttpContext.Current.Items[FormsAuthentication.FormsCookieName] = authTicket;
					}
					catch (CryptographicException)
					{
						// Stale/tampered cookie (e.g. machine key rotation): clear and continue as anonymous.
						UserSignOutOnly();
					}
					catch (ArgumentException)
					{
						// Malformed cookie payload should not break the request pipeline.
						InvalidateAuthCookieSafe();
					}
					catch
					{
						InvalidateAuthCookieSafe();
					}
				}
			}

			return authTicket;
		}
	}

	private static void SetAuthTicket(FormsAuthenticationTicket ticket, bool persistent)
	{
		// issue authentication cookie
		HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
		authCookie.Domain = FormsAuthentication.CookieDomain;
		authCookie.Secure = FormsAuthentication.RequireSSL;
		authCookie.Path = FormsAuthentication.FormsCookiePath;
		authCookie.Value = FormsAuthentication.Encrypt(ticket);
		authCookie.HttpOnly = true;

		if (persistent)
			authCookie.Expires = ticket.Expiration;

		HttpContext.Current.Response.Cookies.Set(authCookie);
	}

	public static string ApplicationPath
	{
		get
		{
			if (HttpContext.Current.Request.ApplicationPath == "/")
				return "";
			else
				return HttpContext.Current.Request.ApplicationPath;
		}
	}

	public static string GetSharedLocalizedString(string moduleName, string resourceKey)
	{
		string className = SharedResourcesFile.Replace(".resx", "");

		if (!String.IsNullOrEmpty(moduleName))
			className = String.Concat(moduleName, "_", className);

		return (string)HttpContext.GetGlobalResourceObject(className, resourceKey);
	}

	public static string GetLocalizedString(string virtualPath, string resourceKey)
	{
		return (string)HttpContext.GetLocalResourceObject(virtualPath, resourceKey);
	}

	public static string GetCurrentPageId()
	{
		return HttpContext.Current.Request["pid"];
	}

	public static bool PageExists(string pageId)
	{
		return PortalConfiguration.Site.Pages[pageId] != null;
	}

	public static string GetLocalizedPageName(string pageId)
	{
		return DefaultPage.GetLocalizedPageName(pageId);
	}

	public static string SHA1(string plainText)
	{
		// Convert plain text into a byte array.
		byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

		HashAlgorithm hash = System.Security.Cryptography.SHA1.Create();

		// Compute hash value of our plain text with appended salt.
		byte[] hashBytes = hash.ComputeHash(plainTextBytes);

		// Return the result.
		return Convert.ToBase64String(hashBytes);
	}

	public static bool ValidatePin(string username, string pin)
	{
		esAuthentication authService = new esAuthentication();
		ConfigureEnterpriseServerProxy(authService, false);
		return authService.ValidatePin(username, pin);
	}
	public static string SHA256(string plainText) => Providers.Cryptor.SHA256(plainText);

	public static int SendPin(string username)
	{
		esAuthentication authService = new esAuthentication();
		ConfigureEnterpriseServerProxy(authService, false);
		return authService.SendPin(username);
	}


	public static bool UpdateUserMfa(string username, bool activate)
	{
		esUsers usersService = new esUsers();
		ConfigureEnterpriseServerProxy(usersService, true);
		try
		{
			return usersService.UpdateUserMfa(username, activate);
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static string[] GetQrCodeData(string username)
	{
		esUsers usersService = new esUsers();
		ConfigureEnterpriseServerProxy(usersService, true);
		return usersService.GetUserMfaQrCodeData(username);
	}

	public static bool ActivateQrCode(string username, string pin)
	{
		esUsers usersService = new esUsers();
		ConfigureEnterpriseServerProxy(usersService, true);
		return usersService.ActivateUserMfaQrCode(username, pin);
	}

	public static string CreateEncryptedAuthenticationTicket(string username, string password, string ipAddress, bool rememberLogin, string preferredLocale, string theme)
	{
		esAuthentication authService = new esAuthentication();
		ConfigureEnterpriseServerProxy(authService, false);

		UserInfo user = authService.GetUserByUsernamePassword(username, SHA256(password), ipAddress);
		FormsAuthenticationTicket ticket = CreateAuthTicket(user.Username, password, user.Role, rememberLogin);
		return FormsAuthentication.Encrypt(ticket);
	}

	public static int GetUserMfaMode(string username, string password, string ipAddress)
	{
		esAuthentication authService = new esAuthentication();
		ConfigureEnterpriseServerProxy(authService, false);
		UserInfo user = authService.GetUserByUsernamePassword(username, SHA256(password), ipAddress);

		return user.MfaMode;
	}

	public static void SetTicketAndCompleteLogin(string encryptedTicket, string username, bool rememberLogin, string preferredLocale, string theme)
	{
		var ticket = FormsAuthentication.Decrypt(encryptedTicket);
		SetAuthTicket(ticket, rememberLogin);
		CompleteUserLogin(username, rememberLogin, preferredLocale, theme);
	}

	public static int AuthenticateUser(string username, string password, string ipAddress)
	{
		esAuthentication authService = new esAuthentication();
		ConfigureEnterpriseServerProxy(authService, false);

		string passwordSH = SHA256(password);

		try
		{
			int authResult = authService.AuthenticateUser(username, passwordSH, ipAddress);

			if (authResult < 0)
			{
				return authResult;
			}

			UserInfo user = authService.GetUserByUsernamePassword(username, passwordSH, ipAddress);
			if (user == null)
			{
				return authResult;
			}

			if (!IsRoleAllowedToLogin(user.Role))
			{
				return BusinessErrorCodes.ERROR_USER_ACCOUNT_ROLE_NOT_ALLOWED;
			}

			if (authResult == BusinessSuccessCodes.SUCCESS_USER_MFA_ACTIVE)
			{
				return BusinessSuccessCodes.SUCCESS_USER_MFA_ACTIVE;
			}

			return authResult;

		}
		catch (Exception)
		{
			throw;
		}
	}

	private static bool IsRoleAllowedToLogin(UserRole role)
	{

		string tmp = GetExcludedRolesToLogin();

		if (tmp == null) tmp = string.Empty;

		string roleKey = ((UserRole)role).ToString();

		return !tmp.Contains(roleKey);
	}


	public static string GetExcludedRolesToLogin()
	{
		return PortalConfiguration.SiteSettings["ExcludedRolesToLogin"];
	}



	public static bool GetHideThemeAndLocale()
	{
		bool bResult = false;

		try
		{
			bResult = Convert.ToBoolean(PortalConfiguration.SiteSettings["HideThemeAndLocale"]);
		}
		catch (Exception)
		{

		}

		return bResult;
	}

	public static bool GetHideDemoCheckbox()
	{
		bool bResult = false;

		try
		{
			bResult = Convert.ToBoolean(PortalConfiguration.SiteSettings["HideDemoCheckbox"]);
		}
		catch (Exception)
		{

		}

		return bResult;
	}


	private static int GetAuthenticationFormsTimeout()
	{
		//default
		int retValue = 30;
		try
		{
			AuthenticationSection authenticationSection = WebConfigurationManager.GetSection("system.web/authentication") as AuthenticationSection;
			if (authenticationSection != null)
			{
				FormsAuthenticationConfiguration fac = authenticationSection.Forms;
				retValue = (int)Math.Truncate(fac.Timeout.TotalMinutes);
			}

		}
		catch
		{
			return retValue;
		}

		return retValue;
	}

	private static FormsAuthenticationTicket CreateAuthTicket(string username, string password,
		 UserRole role, bool persistent)
	{
		return new FormsAuthenticationTicket(
			 1,
			 username,
			 DateTime.Now,
			 persistent ? DateTime.Now.AddMonths(1) : DateTime.Now.AddMinutes(GetAuthenticationFormsTimeout()),
			 persistent,
			 String.Concat(password, Environment.NewLine, Enum.GetName(typeof(UserRole), role))
		);
	}

	public static int ChangeUserPassword(int userId, string newPassword)
	{
		// load user account
		esUsers usersService = new esUsers();
		ConfigureEnterpriseServerProxy(usersService, true);

		try
		{
			UserInfo user = usersService.GetUserById(userId);

			// Resolve currently authenticated user by ticket name to avoid stale/mismatched panel user ids.
			if (AuthTicket != null && !String.IsNullOrEmpty(AuthTicket.Name))
			{
				UserInfo authUser = usersService.GetUserByUsername(AuthTicket.Name);
				if (authUser != null && authUser.UserId > 0 && authUser.UserId != userId)
				{
					userId = authUser.UserId;
					user = authUser;
				}
			}

			// change FuseCP account password
			int result = usersService.ChangeUserPassword(userId, newPassword);
			if (result < 0)
				return result;

			// Verify the new password can be used by the authentication service before reporting success.
			esAuthentication authService = new esAuthentication();
			ConfigureEnterpriseServerProxy(authService, false);
			string ipAddress = HttpContext.Current?.Request?.UserHostAddress;
			string passwordHash = SHA256(newPassword);
			UserInfo verifiedUser = authService.GetUserByUsernamePassword(user.Username, passwordHash, ipAddress);
			if (verifiedUser == null)
				return BusinessErrorCodes.ERROR_USER_WRONG_PASSWORD;

			// change auth cookie
			if (String.Compare(user.Username, AuthTicket.Name, true) == 0)
			{
				FormsAuthenticationTicket ticket = CreateAuthTicket(user.Username, newPassword, user.Role, AuthTicket.IsPersistent);
				SetAuthTicket(ticket, AuthTicket.IsPersistent);
			}
			return result;
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static int UpdateUserAccount(UserInfo user)
	{
		return UpdateUserAccount(null, user);
	}

	public static int UpdateUserAccount(string taskId, UserInfo user)
	{
		esUsers usersService = new esUsers();
		ConfigureEnterpriseServerProxy(usersService, true);

		try
		{
			// update user in FuseCP
			return usersService.UpdateUserTask(taskId, user);
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static int AddUserAccount(List<string> log, UserInfo user, bool sendLetter, string password)
	{
		esUsers usersService = new esUsers();
		ConfigureEnterpriseServerProxy(usersService, true);

		try
		{
			// add user to FuseCP server
			// TODO correct value for prameter notes (instead of null).
			return usersService.AddUser(user, sendLetter, password, null);
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static int DeleteUserAccount(int userId)
	{
		esUsers usersService = new esUsers();
		ConfigureEnterpriseServerProxy(usersService, true);

		try
		{
			// add user to FuseCP server
			return usersService.DeleteUser(userId);
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static int ChangeUserStatus(int userId, UserStatus status)
	{
		esUsers usersService = new esUsers();
		ConfigureEnterpriseServerProxy(usersService, true);

		try
		{
			// add user to FuseCP server
			return usersService.ChangeUserStatus(userId, status);
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static UserInfo GetCurrentUser()
	{
		UserInfo user = null;

		if (AuthTicket != null)
		{
			esUsers usersService = new esUsers();
			ConfigureEnterpriseServerProxy(usersService);

			user = usersService.GetUserByUsername(AuthTicket.Name);
		}

		return user;
	}

	private static void CompleteUserLogin(string username, bool rememberLogin,
		 string preferredLocale, string theme)
	{
		// store last successful username in the cookie
		HttpCookie cookie = new HttpCookie("FuseCPLogin", username);
		cookie.Expires = DateTime.Now.AddDays(7);
		cookie.Secure = FormsAuthentication.RequireSSL;
		cookie.HttpOnly = true;
		HttpContext.Current.Response.Cookies.Add(cookie);

		// set language
		SetCurrentLanguage(preferredLocale);

		// set theme
		SetCurrentTheme(theme);

		//// remember me
		//if (rememberLogin)
		//    HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddMonths(1);
	}

	public static void SetCurrentLanguage(string preferredLocale)
	{
		if (!String.IsNullOrEmpty(preferredLocale))
		{
			HttpCookie localeCrumb = new HttpCookie(CultureCookieName, preferredLocale);
			localeCrumb.Expires = DateTime.Now.AddMonths(2);
			HttpContext.Current.Response.Cookies.Add(localeCrumb);
		}
	}

	public static void ConfigureEnterpriseServerProxy(Web.Clients.ClientBase proxy)
	{
		ConfigureEnterpriseServerProxy(proxy, true);
	}

	public static void ConfigureEnterpriseServerProxy(Web.Clients.ClientBase proxy, bool applyPolicy, FormsAuthenticationTicket authTicket = null)
	{
		// load ES properties
		string serverUrl = PortalConfiguration.SiteSettings["EnterpriseServer"];

		EnterpriseServerProxyConfigurator cnfg = new EnterpriseServerProxyConfigurator();
		cnfg.EnterpriseServerUrl = serverUrl;

		// create assertion
		if (applyPolicy)
		{
			if (authTicket == null) authTicket = AuthTicket;
			if (authTicket != null)
			{
				cnfg.Username = authTicket.Name;
				cnfg.Password = authTicket.UserData.Substring(0, authTicket.UserData.IndexOf(Environment.NewLine));
			}
		}

		cnfg.Configure(proxy);
	}

	public static XmlNode GetModuleContentNode(WebPortalControlBase module)
	{
		XmlNodeList nodes = module.Module.SelectNodes("Content");
		return nodes.Count > 0 ? nodes[0] : null;
	}

	public static XmlNodeList GetModuleMenuItems(WebPortalControlBase module)
	{
		return module.Module.SelectNodes("MenuItem");
	}

	public static string FormatIconImageUrl(string url)
	{
		return url;
	}

	public static string FormatIconLinkUrl(object url)
	{
		return DefaultPage.GetPageUrl(url.ToString());
	}

	public static string GetThemedImage(string imageUrl)
	{
		Page page = HttpContext.Current.Handler as Page;
		if (page != null) return page.ResolveUrl("~/App_Themes/" + page.Theme + "/Images/" + imageUrl);
		else return VirtualPathUtility.ToAbsolute("~/App_Themes/Default/Images/" + imageUrl);
	}

	public static string GetThemedIcon(string iconUrl)
	{
		Page page = (Page)HttpContext.Current.Handler;
		return page.ResolveUrl("~/App_Themes/" + page.Theme + "/" + iconUrl);
	}

	public static void LoadStatesDropDownList(DropDownList list, string countryCode)
	{
		string xmlFilePath = HostingEnvironment.MapPath(CONFIG_FOLDER + "CountryStates.config");
		list.Items.Clear();
		if (File.Exists(xmlFilePath))
		{
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(xmlFilePath);

				List<ListItem> items = new List<ListItem>();

				XmlNodeList xmlNodes = xmlDoc.SelectNodes("//State[@countryCode=" + ToXPathLiteral(countryCode) + "]");
				foreach (XmlElement xmlNode in xmlNodes)
				{
					string nodeName = xmlNode.GetAttribute("name");
					string nodeKey = xmlNode.GetAttribute("key");

					items.Add(new ListItem(nodeName, nodeKey));
				}

				list.Items.AddRange(items.ToArray());
			}
			catch
			{
			}
		}
	}

	public static void LoadCountriesDropDownList(DropDownList list, string countryToSelect)
	{
		string countriesPath = HostingEnvironment.MapPath(CONFIG_FOLDER + "Countries.config");

		if (File.Exists(countriesPath))
		{
			try
			{
				XmlDocument xmlCountriesDoc = new XmlDocument();
				xmlCountriesDoc.Load(countriesPath);

				List<ListItem> items = new List<ListItem>();

				XmlNodeList xmlCountries = xmlCountriesDoc.SelectNodes("//Country");
				foreach (XmlElement xmlCountry in xmlCountries)
				{
					string countryName = xmlCountry.GetAttribute("name");
					string countryKey = xmlCountry.GetAttribute("key");

					if (String.Compare(countryKey, countryToSelect) == 0)
					{
						ListItem li = new ListItem(countryName, countryKey);
						li.Selected = true;
						items.Add(li);
					}
					else
						items.Add(new ListItem(countryName, countryKey));
				}

				list.Items.AddRange(items.ToArray());
			}
			catch
			{
			}
		}
	}


	public static void LoadCultureDropDownList(DropDownList list)
	{
		string localesPath = HostingEnvironment.MapPath(CONFIG_FOLDER + "SupportedLocales.config");

		if (File.Exists(localesPath))
		{

			string localeToSelect = CurrentCulture.Name;

			try
			{
				XmlDocument xmlLocalesDoc = new XmlDocument();
				xmlLocalesDoc.Load(localesPath);

				XmlNodeList xmlLocales = xmlLocalesDoc.SelectNodes("//Locale");
				for (int i = 0; i < xmlLocales.Count; i++)
				{
					XmlElement xmlLocale = (XmlElement)xmlLocales[i];

					string localeName = xmlLocale.GetAttribute("name");
					string localeKey = xmlLocale.GetAttribute("key");

					list.Items.Add(new ListItem(localeName, localeKey));
				}


				HttpCookie localeCrumb = HttpContext.Current?.Request.Cookies[CultureCookieName];
				if (localeCrumb != null)
				{
					ListItem item = list.Items.FindByValue(localeToSelect);
					if (item != null)
						item.Selected = true;
				}
				else
				{
					if (list.Items.Count > 0 && list.Items[0] != null)
					{
						SetCurrentLanguage(list.Items[0].Value);
						HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.ToString());
					}
				}
			}
#if NETCOREAPP
			catch (ResponseEndException)
			{
				throw;
			}
#endif
			catch
			{
			}
		}
	}

	#region Navigation Routines
	public static string LoginRedirectUrl
	{
		get { return DefaultPage.GetPageUrl(PortalConfiguration.SiteSettings["DefaultPage"]); }
	}

	public static string GetUserHomePageUrl(int userId)
	{
		string userHomePageId = PortalConfiguration.SiteSettings["UserHomePage"];
		return userId > 0 ? NavigatePageURL(userHomePageId, USER_ID_PARAM, userId.ToString())
			 : NavigatePageURL(userHomePageId);
	}

	public static string GetUserCustomersPageId()
	{
		return PortalConfiguration.SiteSettings["UserCustomersPage"];
	}

	public static string GetUsersSearchPageId()
	{
		return PortalConfiguration.SiteSettings["UsersSearchPage"];
	}

	public static string GetSpacesSearchPageId()
	{
		return PortalConfiguration.SiteSettings["SpacesSearchPage"];
	}

	//TODO START
	public static string GetObjectSearchPageId()
	{
		return "SearchObject";
	}
	//TODO END

	public static string GetNestedSpacesPageId()
	{
		return PortalConfiguration.SiteSettings["NestedSpacesPage"];
	}

	public static string GetSpaceHomePageUrl(int spaceId)
	{
		string spaceHomePageId = PortalConfiguration.SiteSettings["SpaceHomePage"];
		return spaceId > -1 ? NavigatePageURL(spaceHomePageId, SPACE_ID_PARAM, spaceId.ToString())
			 : NavigatePageURL(spaceHomePageId);
	}

	public static string GetLoggedUserAccountPageUrl()
	{
		return NavigatePageURL(PortalConfiguration.SiteSettings["LoggedUserAccountPage"]);
	}

	public static string NavigateURL()
	{
		return NavigateURL(null, null);
	}

	public static string NavigatePageURL(string pageId)
	{
		return NavigatePageURL(pageId, null, null, new string[] { });
	}

	public static string NavigateURL(string keyName, string keyValue, params string[] additionalParams)
	{
		return NavigatePageURL(HttpContext.Current.Request[DefaultPage.PAGE_ID_PARAM], keyName, keyValue, additionalParams);
	}

	public static string NavigatePageURL(string pageId, string keyName, string keyValue, params string[] additionalParams)
	{
		string navigateUrl = DefaultPage.DEFAULT_PAGE;

		List<string> urlBuilder = new List<string>();

		// add page id parameter
		if (!String.IsNullOrEmpty(pageId))
			urlBuilder.Add(String.Concat(DefaultPage.PAGE_ID_PARAM, "=", pageId));

		// add specified key
		if (!String.IsNullOrEmpty(keyName) && !String.IsNullOrEmpty(keyValue))
			urlBuilder.Add(String.Concat(keyName, "=", keyValue));

		// load additional params
		if (additionalParams != null)
		{
			string controlId = null;
			string moduleDefinitionId = null;
			//
			foreach (string paramStr in additionalParams)
			{
				if (paramStr.StartsWith("ctl=", StringComparison.InvariantCultureIgnoreCase))
				{
					// ensure page exists and avoid unnecessary exceptions throw
					if (PortalConfiguration.Site.Pages.ContainsKey(pageId))
					{
						string[] pair = paramStr.Split('=');
						controlId = pair[1];
					}

				}
				else if (paramStr.StartsWith("moduleDefId=", StringComparison.InvariantCultureIgnoreCase))
				{
					// ensure page exists and avoid unnecessary exceptions throw
					if (PortalConfiguration.Site.Pages.ContainsKey(pageId))
					{
						string[] pair = paramStr.Split('=');
						moduleDefinitionId = pair[1];
					}
					continue;
				}
				urlBuilder.Add(paramStr);
			}
			if (!String.IsNullOrEmpty(moduleDefinitionId) && !String.IsNullOrEmpty(controlId))
			{
				// 1. Read module controls first information first
				foreach (ModuleDefinition md in PortalConfiguration.ModuleDefinitions.Values)
				{
					if (String.Equals(md.Id, moduleDefinitionId, StringComparison.InvariantCultureIgnoreCase))
					{
						// 2. Lookup for module control
						foreach (ModuleControl mc in md.Controls.Values)
						{
							// 3. Compare against ctl parameter value
							if (mc.Key.Equals(controlId, StringComparison.InvariantCultureIgnoreCase))
							{
								// 4. Lookup for module id
								foreach (int pmKey in PortalConfiguration.Site.Modules.Keys)
								{
									PageModule pm = PortalConfiguration.Site.Modules[pmKey];
									if (String.Equals(pm.ModuleDefinitionID, md.Id,
										 StringComparison.InvariantCultureIgnoreCase))
									{
										// 5. Append module id parameter
										urlBuilder.Add("mid=" + pmKey);
										goto End;
									}
								}
							}
						}
					}
				}
			}
		}

	End:
		if (urlBuilder.Count > 0)
			navigateUrl += String.Concat("?", String.Join("&", urlBuilder.ToArray()));

		return navigateUrl;
	}

	public static string EditUrl(string keyName, string keyValue, string controlKey, params string[] additionalParams)
	{
		List<string> url = new List<string>();

		string pageId = HttpContext.Current.Request[DefaultPage.PAGE_ID_PARAM];

		if (!String.IsNullOrEmpty(pageId))
			url.Add(String.Concat(DefaultPage.PAGE_ID_PARAM, "=", pageId));

		if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["mid"]))
			url.Add(String.Concat(DefaultPage.MODULE_ID_PARAM, "=", HttpContext.Current.Request.QueryString["mid"]));

		url.Add(String.Concat(DefaultPage.CONTROL_ID_PARAM, "=", controlKey));

		if (!String.IsNullOrEmpty(keyName) && !String.IsNullOrEmpty(keyValue))
		{
			url.Add(String.Concat(keyName, "=", keyValue));
		}

		if (additionalParams != null)
		{
			foreach (string additionalParam in additionalParams)
			{
				if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["mid"]))
				{
					if (!additionalParam.Contains("mid="))
					{
						url.Add(additionalParam);
					}
				}
				else
					url.Add(additionalParam);
			}
		}

		return "~/Default.aspx?" + String.Join("&", url.ToArray());
	}
	#endregion

	public static string GetGeneralESControlKey(string controlKey)
	{
		string generalControlKey = string.Empty;

		string appData = HostingEnvironment.MapPath(CONFIG_FOLDER);
		string xmlFilePath = Path.Combine(appData, EXCHANGE_SERVER_HIERARCHY_FILE);
		if (File.Exists(xmlFilePath))
		{
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(xmlFilePath);

				XmlElement xmlNode = (XmlElement)xmlDoc.SelectSingleNode("/Controls/Control[@key=" + ToXPathLiteral(controlKey) + "]");

				if (xmlNode.HasAttribute("general_key"))
				{
					generalControlKey = xmlNode.GetAttribute("general_key");
				}
				else generalControlKey = xmlNode.GetAttribute("key");
			}
			catch
			{
			}
		}
		return generalControlKey;
	}

	private static string ToXPathLiteral(string value)
	{
		if (value == null)
			return "''";

		if (!value.Contains("'"))
			return "'" + value + "'";

		if (!value.Contains("\""))
			return "\"" + value + "\"";

		string[] parts = value.Split('\'');
		StringBuilder builder = new StringBuilder("concat(");
		for (int i = 0; i < parts.Length; i++)
		{
			if (i > 0)
				builder.Append(", \"'\", ");

			builder.Append("'");
			builder.Append(parts[i]);
			builder.Append("'");
		}
		builder.Append(")");

		return builder.ToString();
	}
}


/*
 // 
 */
