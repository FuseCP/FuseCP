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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Threading.Tasks;
using FuseCP.EnterpriseServer;
using FCP = FuseCP.EnterpriseServer;



namespace FuseCP.Portal
{
	public partial class Login : FuseCPModuleBase
	{
		private const string ThemeModeCookieName = "UserThemeMode";
		private const string ThemeModeAuto = "auto";
		private const string ThemeStyleLight = "light-theme";
		private const string ThemeStyleDark = "dark-theme";

		string ipAddress;
		//private IMessageBoxControl messageBox;     --- compile warning - never used

		private bool IsLocalUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return false;
			}

			Uri absoluteUri;
			if (Uri.TryCreate(url, UriKind.Absolute, out absoluteUri))
			{
				return String.Equals(this.Request.Url.Host, absoluteUri.Host, StringComparison.OrdinalIgnoreCase);
			}
			else
			{
				bool isLocal = !url.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
					 && !url.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
					 && Uri.IsWellFormedUriString(url, UriKind.Relative);
				return isLocal;
			}
		}

		private string RedirectUrl
		{
			get
			{
				string redirectUrl = "";
				if (Request["returnurl"] != null)
				{
					// return to the url passed to signin
					// redirectUrl = HttpUtility.UrlDecode(Request["returnurl"]);
					redirectUrl = Request["ReturnUrl"];
					if (!IsLocalUrl(redirectUrl))
					{
						redirectUrl = PortalUtils.LoginRedirectUrl;
					}
				}
				else
				{
					redirectUrl = PortalUtils.LoginRedirectUrl;
				}
				return redirectUrl;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			ApplyInputMetadata();

			if (!IsPostBack)
			{
				Page.RegisterAsyncTask(new PageAsyncTask(() => Task.WhenAll(
					EnsureFCPAAsync())));

				BindControls();
			}

			// capture Enter key
			//DotNetNuke.UI.Utilities.ClientAPI.RegisterKeyCapture(this.Parent, btnLogin, 13);

			// get user IP
			if (Request.UserHostAddress != null)
				ipAddress = Request.UserHostAddress;

			// update password control
			txtPassword.Attributes["value"] = txtPassword.Text;

			// autologin
			string usr = Request["u"];
			if (String.IsNullOrEmpty(usr))
				usr = Request["user"];

			string psw = Request["p"];
			if (String.IsNullOrEmpty(psw))
				psw = Request["pwd"];
			if (String.IsNullOrEmpty(psw))
				psw = Request["password"];

			if (!String.IsNullOrEmpty(usr) && !String.IsNullOrEmpty(psw))
			{
				// perform login
				LoginUser(usr, psw, chkRemember.Checked, String.Empty, String.Empty);
			}
		}

		private void ApplyInputMetadata()
		{
			// Make login fields explicit for browser/extension autofill parsers.
			txtUsername.Attributes["autocomplete"] = "username";
			txtUsername.Attributes["aria-label"] = "Username";

			txtPassword.Attributes["autocomplete"] = "current-password";
			txtPassword.Attributes["aria-label"] = "Password";

			txtPin.Attributes["autocomplete"] = "one-time-code";
			txtPin.Attributes["aria-label"] = "One-time verification code";
		}

		private async Task EnsureFCPAAsync()
		{
			var enabledScpa = await ES.Services.Authentication.GetSystemSetupModeAsync().ConfigureAwait(false);
			//
			if (enabledScpa == false)
			{
				return;
			}
			//
			Response.Redirect(EditUrl("fcpa"), true);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
		}
		private void BindControls()
		{
			// load languages
			PortalUtils.LoadCultureDropDownList(ddlLanguage);
			BindThemeModes();

			string selectedThemeMode = ThemeModeAuto;
			HttpCookie modeCookie = Request.Cookies[ThemeModeCookieName];
			if (modeCookie != null)
			{
				string requestedMode = (modeCookie.Value ?? String.Empty).Trim().ToLowerInvariant();
				if (requestedMode == ThemeStyleDark || requestedMode == ThemeStyleLight)
				{
					selectedThemeMode = requestedMode;
				}
			}
			else
			{
				string currentThemeStyle = PortalUtils.CurrentThemeStyle;
				if (String.Equals(currentThemeStyle, ThemeStyleDark, StringComparison.OrdinalIgnoreCase))
				{
					selectedThemeMode = ThemeStyleDark;
				}
				else if (String.Equals(currentThemeStyle, ThemeStyleLight, StringComparison.OrdinalIgnoreCase))
				{
					selectedThemeMode = ThemeStyleLight;
				}
			}

			Utils.SelectListItem(ddlTheme, selectedThemeMode);

			// try to get the last login name from cookie
			HttpCookie cookie = Request.Cookies["FuseCPLogin"];
			if (cookie != null)
			{
				txtUsername.Text = cookie.Value;
				txtPassword.Focus();
			}
			else
			{
				// set focus on username field
				txtUsername.Focus();
			}

			if (PortalUtils.GetHideThemeAndLocale())
			{
				ddlLanguage.Visible = false;
				lblLanguage.Visible = false;
				ddlTheme.Visible = false;
				lblTheme.Visible = false;
			}
		}

		private void BindThemeModes()
		{
			ddlTheme.Items.Clear();
			ddlTheme.Items.Add(new ListItem("System", ThemeModeAuto));
			ddlTheme.Items.Add(new ListItem("Light", ThemeStyleLight));
			ddlTheme.Items.Add(new ListItem("Dark", ThemeStyleDark));
		}

		protected void cmdForgotPassword_Click(object sender, EventArgs e)
		{
			Response.Redirect(EditUrl("forgot_password"), true);
		}

		protected void btnLogin_Click(object sender, EventArgs e)
		{
			Page.Validate("LoginForm");

			// validate input
			if (!Page.IsValid)
				return;

			string username = txtUsername.Text.Trim();
			string password = txtPassword.Text;
			if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
			{
				ShowWarningMessage("WrongLogin");
				return;
			}

			// perform login
			LoginUser(username, password, chkRemember.Checked,
				 ddlLanguage.SelectedValue, PortalUtils.CurrentTheme);
		}

		protected void btnVerifyPin_Click(object sender, EventArgs e)
		{
			Page.Validate("PinForm");

			// validate input
			if (!Page.IsValid)
				return;

			if (String.IsNullOrWhiteSpace(txtPin.Text))
			{
				ShowErrorMessage("WrongPin");
				return;
			}

			var isValid = PortalUtils.ValidatePin(txtUsername.Text, txtPin.Text);

			if (!isValid)
			{
				ShowErrorMessage("WrongPin");
				return;
			}

			var encryptedTicket = tokenDiv.Attributes["value"];
			PortalUtils.SetTicketAndCompleteLogin(encryptedTicket, txtUsername.Text.Trim(), chkRemember.Checked, ddlLanguage.SelectedValue, PortalUtils.CurrentTheme);
			tokenDiv.Attributes["value"] = null;
			CompleteLogin(0);
		}


		private void LoginUser(string username, string password, bool rememberLogin,
			 string preferredLocale, string theme)
		{
			// status
			int loginStatus = PortalUtils.AuthenticateUser(username, password, ipAddress);

			if (loginStatus < 0)
			{
				ShowWarningMessage("WrongLogin");
				return;
			}

			string encryptedTicket = PortalUtils.CreateEncryptedAuthenticationTicket(username, password, ipAddress, rememberLogin, preferredLocale, theme);

			if (loginStatus == BusinessSuccessCodes.SUCCESS_USER_MFA_ACTIVE)
			{
				int mfaMode = PortalUtils.GetUserMfaMode(username, password, ipAddress);
				btnResendPin.Visible = mfaMode != 2;
				userPwdDiv.Visible = false;
				tokenDiv.Visible = true;
				tokenDiv.Attributes["value"] = encryptedTicket;
				txtPin.Focus();
				return;
			}

			PortalUtils.SetTicketAndCompleteLogin(encryptedTicket, txtUsername.Text.Trim(), chkRemember.Checked, ddlLanguage.SelectedValue, PortalUtils.CurrentTheme);
			CompleteLogin(loginStatus);
		}

		private void CompleteLogin(int loginStatus)
		{
			// Access IP Settings
			FCP.SystemSettings settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.ACCESS_IP_SETTINGS);
			String AccessIps = String.Empty;
			String[] arAccessIps = null;
			if (settings != null)
			{
				AccessIps = settings.GetValueOrDefault(FCP.SystemSettings.ACCESS_IPs, string.Empty);
				arAccessIps = AccessIps.Split(new[] { ',', ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			}

			if (!String.IsNullOrEmpty(AccessIps))
			{
				String RequestIP = Request.ServerVariables["REMOTE_ADDR"];
				// String l_stSubnet = Knom.Helpers.Net.SubnetMask.ReturnSubnetmask(AccessIps);
				Boolean l_Mach = false;

				try
				{
					foreach (String l_AccessIP in arAccessIps)
					{
						l_Mach = Knom.Helpers.Net.SubnetMask.IsInRange(RequestIP, l_AccessIP.Trim());
						if (l_Mach)
							break; // Once it passed then don't need to check for other access;
					}
				}
				catch (Exception)
				{ }
				if (!l_Mach)
				{
					PortalUtils.UserSignOutOnly();
					// messageBox.RenderMessage(MessageBoxType.Warning, "Unauthorized IP", "Unauthorized IP", null);
					ShowWarningMessage("IPAccessProhibited");
					return;
				}

			}

			if (loginStatus == BusinessSuccessCodes.SUCCESS_USER_ONETIMEPASSWORD)
			{
				// One time password should be changed after login
				Response.Redirect("Default.aspx?mid=1&ctl=change_onetimepassword&onetimepassword=true&UserID=" + PanelSecurity.LoggedUserId.ToString());
			}
			else
			{
				//Make Theme Cookies
				DataSet UserThemeSettingsData = ES.Services.Users.GetUserThemeSettings(PanelSecurity.LoggedUserId);
				if (UserThemeSettingsData.Tables.Count > 0)
				{
					foreach (DataRow row in UserThemeSettingsData.Tables[0].Rows)
					{
						string RowPropertyName = row.Field<String>("PropertyName");
						string RowPropertyValue = row.Field<String>("PropertyValue");

						if (RowPropertyName == "Style")
						{
							string UserThemeStyle = RowPropertyValue;

							HttpCookie UserThemeStyleCrumb = new HttpCookie("UserThemeStyle", UserThemeStyle);
							UserThemeStyleCrumb.Expires = DateTime.Now.AddMonths(2);
							HttpContext.Current.Response.Cookies.Add(UserThemeStyleCrumb);

						}

						if (RowPropertyName == "palette-Light")
						{
							HttpCookie paletteLightCookie = new HttpCookie("UserThemePaletteLight", RowPropertyValue ?? string.Empty);
							paletteLightCookie.Expires = DateTime.Now.AddMonths(2);
							HttpContext.Current.Response.Cookies.Add(paletteLightCookie);
						}

						if (RowPropertyName == "palette-Dark")
						{
							HttpCookie paletteDarkCookie = new HttpCookie("UserThemePaletteDark", RowPropertyValue ?? string.Empty);
							paletteDarkCookie.Expires = DateTime.Now.AddMonths(2);
							HttpContext.Current.Response.Cookies.Add(paletteDarkCookie);
						}

						if (RowPropertyName == "buttons-Light")
						{
							HttpCookie buttonsLightCookie = new HttpCookie("UserThemeButtonsLight", RowPropertyValue ?? string.Empty);
							buttonsLightCookie.Expires = DateTime.Now.AddMonths(2);
							HttpContext.Current.Response.Cookies.Add(buttonsLightCookie);
						}

						if (RowPropertyName == "buttons-Dark")
						{
							HttpCookie buttonsDarkCookie = new HttpCookie("UserThemeButtonsDark", RowPropertyValue ?? string.Empty);
							buttonsDarkCookie.Expires = DateTime.Now.AddMonths(2);
							HttpContext.Current.Response.Cookies.Add(buttonsDarkCookie);
						}
					}
				}

				// redirect by shortcut
				ShortcutRedirect();

				// standard redirect
				Response.Redirect(RedirectUrl, true);
			}
		}

		private void ShortcutRedirect()
		{
			if (PanelSecurity.EffectiveUser.Role == UserRole.Administrator)
				return; // not for administrators

			string shortcut = Request["shortcut"];
			if ("vps".Equals(shortcut, StringComparison.InvariantCultureIgnoreCase))
			{
				// load hosting spaces
				PackageInfo[] packages = ES.Services.Packages.GetMyPackages(PanelSecurity.EffectiveUserId);
				if (packages.Length == 0)
					return; // no spaces - exit

				// check if some package has VPS resource enabled
				foreach (PackageInfo package in packages)
				{
					int packageId = package.PackageId;
					PackageContext cntx = PackagesHelper.GetCachedPackageContext(packageId);
					if (cntx.Groups.ContainsKey(ResourceGroups.VPS))
					{
						// VPS resource found
						// check created VPS
						VirtualMachineMetaItemsPaged vms = ES.Services.VPS.GetVirtualMachines(packageId, "", "", "", 0, Int32.MaxValue, false);
						if (vms.Items.Length == 1)
						{
							// one VPS - redirect to its properties screen
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPS", "SpaceID", packageId.ToString(),
								 "ItemID=" + vms.Items[0].ItemID.ToString(), "ctl=vps_general", "moduleDefId=VPS"));
						}
						else
						{
							// several VPS - redirect to VPS list page
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPS", "SpaceID", packageId.ToString(),
								 "ctl=", "moduleDefId=VPS"));
						}
					}


					if (cntx.Groups.ContainsKey(ResourceGroups.VPS2012))
					{
						// VPS resource found
						// check created VPS
						VirtualMachineMetaItemsPaged vms = ES.Services.VPS2012.GetVirtualMachines(packageId, "", "", "", 0, Int32.MaxValue, false);
						if (vms.Items.Length == 1)
						{
							// one VPS - redirect to its properties screen
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPS2012", "SpaceID", packageId.ToString(),
								 "ItemID=" + vms.Items[0].ItemID.ToString(), "ctl=vps_general", "moduleDefId=VPS2012"));
						}
						else
						{
							// several VPS - redirect to VPS list page
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPS2012", "SpaceID", packageId.ToString(),
								 "ctl=", "moduleDefId=VPS2012"));
						}
					}


					if (cntx.Groups.ContainsKey(ResourceGroups.VPSForPC))
					{
						// VPS resource found
						// check created VPS
						VirtualMachineMetaItemsPaged vms = ES.Services.VPSPC.GetVirtualMachines(packageId, "", "", "", 0, Int32.MaxValue, false);
						if (vms.Items.Length == 1)
						{
							// one VPS - redirect to its properties screen
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPSForPC", "SpaceID", packageId.ToString(),
								 "ItemID=" + vms.Items[0].ItemID.ToString(), "ctl=vps_general", "moduleDefId=VPSForPC"));
						}
						else
						{
							// several VPS - redirect to VPS list page
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPSForPC", "SpaceID", packageId.ToString(),
								 "ctl=", "moduleDefId=VPSForPC"));
						}
					}

				}

				// no VPS resources found
				// redirect to space home
				if (packages.Length == 1)
					Response.Redirect(PortalUtils.GetSpaceHomePageUrl(packages[0].PackageId));
			}
		}

		private void SetCurrentLanguage()
		{
			PortalUtils.SetCurrentLanguage(ddlLanguage.SelectedValue);
		}

		private void SetCurrentTheme()
		{
			string selectedMode = (ddlTheme.SelectedValue ?? String.Empty).Trim().ToLowerInvariant();
			if (selectedMode == ThemeModeAuto)
			{
				HttpCookie resetModeCookie = new HttpCookie(ThemeModeCookieName, String.Empty);
				resetModeCookie.Expires = DateTime.Now.AddMonths(-1);
				HttpContext.Current.Response.Cookies.Add(resetModeCookie);
				return;
			}

			if (!String.Equals(selectedMode, ThemeStyleDark, StringComparison.OrdinalIgnoreCase))
			{
				selectedMode = ThemeStyleLight;
			}

			HttpCookie modeCookie = new HttpCookie(ThemeModeCookieName, selectedMode);
			modeCookie.Expires = DateTime.Now.AddMonths(2);
			HttpContext.Current.Response.Cookies.Add(modeCookie);
		}

		protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetCurrentLanguage();
			SetCurrentTheme();

			Response.Redirect(Request.Url.ToString());
		}

		protected void ddlTheme_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetCurrentTheme();
			Response.Redirect(Request.Url.ToString());
		}

		protected void btnResendPin_Click(object sender, EventArgs e)
		{
			int sendPinResult = PortalUtils.SendPin(txtUsername.Text.Trim());

			if (sendPinResult == 0)
				ShowSuccessMessage("PinSend");
			else
				ShowErrorMessage("PinSendError");
		}
	}
}
