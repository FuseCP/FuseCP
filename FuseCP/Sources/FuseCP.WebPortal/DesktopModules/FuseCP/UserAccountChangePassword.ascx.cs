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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    public partial class UserAccountChangePassword : FuseCPModuleBase
    {
        const string changePasswordWarningKey = "LoggedUserEditDetails.ChangePasswordWarning";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUser();
            }
        }

        private void BindUser()
        {
            try
            {
                UserInfo user = UsersHelper.GetUser(PanelSecurity.SelectedUserId);
                if (user != null)
                {
                    // account info
                    lblUsername.Text = PortalAntiXSS.Encode(user.Username);

                    // password policy
                    userPassword.SetUserPolicy(PanelSecurity.SelectedUserId, UserSettings.FuseCP_POLICY, "PasswordPolicy");
                    userPassword.ValidationGroup = "NewPassword";
                }
                else
                {
                    // can't be found
                    RedirectBack();
                }

                if (PanelSecurity.LoggedUserId == PanelSecurity.SelectedUserId)
                {
                    trChangePasswordWarning.Visible = true;
                    string changePasswordWarningText = GetSharedLocalizedString(changePasswordWarningKey);
                    if (!String.IsNullOrEmpty(changePasswordWarningText))
                        lblChangePasswordWarning.Text = changePasswordWarningText;
                }

                if (PanelRequest.GetBool("onetimepassword"))
                {
                    ShowWarningMessage("USER_SHOULD_CHANGE_ONETIMEPASSWORD");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("USER_GET_USER", ex);
                return;
            }
        }

        protected void cmdChangePassword_Click(object sender, EventArgs e)
        {
            Page.Validate("NewPassword");
            if (!Page.IsValid)
                return;

            try
            {
                //int result = UsersHelper.ChangeUserPassword(PortalId, PanelRequest.UserID, userPassword.Password);
                int result = PortalUtils.ChangeUserPassword(PanelRequest.UserID, userPassword.Password);

                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }

                ShowSuccessMessage("USER_CHANGE_PASSWORD");
                if (PanelSecurity.SelectedUserId == PanelSecurity.LoggedUserId)
                {
                    const int redirectTimeout = Utils.CHANGE_PASSWORD_REDIRECT_TIMEOUT;
                    PasswordPanel.Visible = false;
                    string loginClientUrl = Page.ResolveClientUrl(PortalUtils.LoginRedirectUrl);
                    ShowSuccessMessage(Utils.ModuleName, "LOGGED_USER_CHANGE_PASSWORD", loginClientUrl, (redirectTimeout / 1000).ToString());
                    // Clear auth cookie explicitly using the same attributes as SetAuthTicket
                    // to avoid leaving a stale cookie when FormsAuthentication.SignOut() uses
                    // different cookie path/domain defaults.
                    HttpCookie expireCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
                    expireCookie.Domain = FormsAuthentication.CookieDomain;
                    expireCookie.Secure = FormsAuthentication.RequireSSL;
                    expireCookie.Path = FormsAuthentication.FormsCookiePath;
                    expireCookie.HttpOnly = true;
                    expireCookie.Expires = DateTime.Now.AddDays(-1);
                    expireCookie.Value = "";
                    Response.Cookies.Set(expireCookie);
                    FormsAuthentication.SignOut();
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "RedirectToLogin", String.Format("setTimeout(\"window.location='{0}'\",{1});", loginClientUrl, redirectTimeout), true); 
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("USER_CHANGE_PASSWORD", ex);
                return;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectBack();
        }

        private void RedirectBack()
        {
            Response.Redirect(NavigateURL(PortalUtils.USER_ID_PARAM, PanelSecurity.SelectedUserId.ToString()));
        }
    }
}
