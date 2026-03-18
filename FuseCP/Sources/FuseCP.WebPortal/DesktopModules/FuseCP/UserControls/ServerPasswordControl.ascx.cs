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
using System.Configuration.Internal;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using FuseCP.EnterpriseServer;
using FuseCP.Providers.HostedSolution;

namespace FuseCP.Portal
{
    public partial class ServerPasswordControl : FuseCPControlBase
    {
        public const string EMPTY_PASSWORD = "$FuseCP!@";
        public const int MIN_PASSWORD_LENGTH = 1;

        public bool ValidationEnabled
        {
            get { return (ViewState["ValidationEnabled"] != null) ? (bool)ViewState["ValidationEnabled"] : true; }
            set { ViewState["ValidationEnabled"] = value; ToggleControls(); }
        }


        public string ValidationGroup
        {
            get
            {
                return valRequirePassword.ValidationGroup;
            }
            set
            {
                valRequirePassword.ValidationGroup = value;
            }
        }

        public bool EditMode
        {
            get { return (ViewState["EditMode"] != null) ? (bool)ViewState["EditMode"] : false; }
            set { ViewState["EditMode"] = value; ToggleControls(); }
        }

        public string Password
        {
            get
            {
                // Password textboxes can lose Text in some postback/lifecycle paths,
                // so fall back to the posted form value when needed.
                string value = txtPassword.Text;

                if (string.IsNullOrEmpty(value) && Page?.Request != null)
                {
                    value = Page.Request.Form[txtPassword.UniqueID]
                        ?? Page.Request.Form[txtPassword.ClientID];

                    // Last-resort: scan all form keys for any ending in "$txtPassword"
                    // (handles cases where the NamingContainer path differs at runtime)
                    if (string.IsNullOrEmpty(value))
                    {
                        string tail = "$" + txtPassword.ID;
                        foreach (string key in Page.Request.Form.AllKeys ?? [])
                        {
                            if (key != null && key.EndsWith(tail, StringComparison.OrdinalIgnoreCase))
                            {
                                string candidate = Page.Request.Form[key];
                                if (!string.IsNullOrEmpty(candidate))
                                {
                                    value = candidate;
                                    break;
                                }
                            }
                        }
                    }

                    value ??= string.Empty;
                }

                return (value == EMPTY_PASSWORD) ? "" : value;
            }
            set { txtPassword.Text = value; }
        }

        public bool CheckPasswordLength
        {
            get { return (ViewState["CheckPasswordLength"] != null) ? (bool)ViewState["CheckPasswordLength"] : true; }
            set { ViewState["CheckPasswordLength"] = value; ToggleControls(); }
        }

        public int MinimumLength
        {
            get { return (ViewState["MinimumLength"] != null) ? (int)ViewState["MinimumLength"] : 0; }
            set { ViewState["MinimumLength"] = value; }
        }

        public int MaximumLength
        {
            get { return (ViewState["MaximumLength"] != null) ? (int)ViewState["MaximumLength"] : 0; }
            set
            {
                {
                    txtPassword.MaxLength = value;
                    ViewState["MaximumLength"] = value;
                }
            }
        }

        private UserInfo PolicyUser
        {
            get { return (ViewState["PolicyUser"] != null) ? (UserInfo)ViewState["PolicyUser"] : null; }
            set { ViewState["PolicyUser"] = value; }
        }

        private string PolicyValue
        {
            get { return (ViewState["PolicyValue"] != null) ? (string)ViewState["PolicyValue"] : null; }
            set { ViewState["PolicyValue"] = value; }
        }

        public void SetPackagePolicy(int packageId, string settingsName, string key)
        {
            // load package
            PackageInfo package = PackagesHelper.GetCachedPackage(packageId);
            if (package != null)
            {
                // init by user
                SetUserPolicy(package.UserId, settingsName, key);
            }
        }

        public void SetUserPolicy(int userId, string settingsName, string key)
        {
            // load user profile
            UserInfo user = UsersHelper.GetCachedUser(userId);

            if (user != null)
            {
                PolicyUser = user;

                // load settings
                //UserSettings settings = UsersHelper.GetCachedUserSettings(userId, settingsName);
                //EP 2009/09/15: Removed caching for user policy as it was confusing users
                UserSettings settings = ES.Services.Users.GetUserSettings(userId, settingsName);

                if (settings != null)
                {
                    string policyValue = settings[key];
                    if (policyValue != null)
                        PolicyValue = policyValue;
                }
            }

            // toggle controls
            ToggleControls();
        }

        public void SetUserPolicy(bool enabled, int minLength, int maxLength, bool notEqualToUsername)
        {
            PolicyValue = string.Join(";", enabled, minLength, maxLength, notEqualToUsername);

            ToggleControls();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtPassword.Attributes["value"] = txtPassword.Text;
            ToggleControls();
        }

        private void ToggleControls()
        {

            // set empty password
            if (txtPassword.Text == "" && EditMode)
            {
                txtPassword.Attributes["value"] = EMPTY_PASSWORD;
            }

            // enable/disable require validators
            valRequirePassword.Enabled = ValidationEnabled;

            // require default length
            MinimumLength = Math.Max(MIN_PASSWORD_LENGTH, MinimumLength);

            // parse and enforce policy
            if (PolicyValue != null)
            {
                bool enabled = false;
                int minLength = 1;
                int maxLength = 50;
                bool notEqualToUsername = false;

                try
                {
                    // parse settings
                    string[] parts = PolicyValue.Split(';');
                    enabled = Utils.ParseBool(parts[0], false);
                    minLength = Math.Max(Utils.ParseInt(parts[1], 0), MinimumLength);
                    maxLength = Math.Max(Utils.ParseInt(parts[2], 0), MaximumLength);
                    notEqualToUsername = Utils.ParseBool(parts[6], false);
                }
                catch { /* skip */ }

                // apply policy
                if (enabled)
                {
                    // min length
                    if (minLength > 0)
                    {
                        MinimumLength = minLength;
                    }

                    // max length
                    if (maxLength > 0)
                    {
                        MaximumLength = maxLength;
                        txtPassword.MaxLength = maxLength;
                    }

                } // if(enabled)
            } // if (PolicyValue != null)

        }

        protected void valCorrectLength_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ((args.Value == EMPTY_PASSWORD) || (args.Value.Length >= MinimumLength));
        }

        private bool ValidatePattern(string regexp, string val, int minimumNumber)
        {
            return (Regex.Matches(val, regexp).Count >= minimumNumber);
        }
    }
}
