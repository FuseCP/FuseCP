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
    public partial class PasswordControl : FuseCPControlBase
    {
        public const string EMPTY_PASSWORD = "";
        public const int MIN_PASSWORD_LENGTH = 1;

        public bool ValidationEnabled
        {
            get { return (ViewState["ValidationEnabled"] != null) ? (bool)ViewState["ValidationEnabled"] : true; }
            set { ViewState["ValidationEnabled"] = value; ToggleControls(); }
        }

        public bool AllowGeneratePassword
        {
            get { return lnkGenerate.Visible; }
            set { lnkGenerate.Visible = value;  }
        }

        public string GetRandomPasswordUrl()
        {
            return Page.ClientScript.GetWebResourceUrl(
                typeof(PasswordControl), "FuseCP.Portal.Scripts.RandomPassword.js");
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
                valRequireConfirmPassword.ValidationGroup = value;
                valRequireEqualPassword.ValidationGroup = value;
                valCorrectLength.ValidationGroup = value;
                valRequireNumbers.ValidationGroup = value;
                valRequireUppercase.ValidationGroup = value;
                valRequireSymbols.ValidationGroup = value;
            }
        }

        public bool EditMode
        {
            get { return (ViewState["EditMode"] != null) ? (bool)ViewState["EditMode"] : false; }
            set { ViewState["EditMode"] = value; ToggleControls(); }
        }

        public string Password
        {
            get { return (txtPassword.Text == EMPTY_PASSWORD) ? "" : NormalizePassword(txtPassword.Text); }
            set { txtPassword.Text = value; txtConfirmPassword.Text = value; }
        }

        public bool ValidateInput()
        {
            ToggleControls();

            valRequirePassword.Validate();
            valRequireConfirmPassword.Validate();
            valRequireEqualPassword.Validate();

            if (valCorrectLength.Enabled)
                valCorrectLength.Validate();

            if (valRequireNumbers.Enabled)
                valRequireNumbers.Validate();

            if (valRequireUppercase.Enabled)
                valRequireUppercase.Validate();

            if (valRequireSymbols.Enabled)
                valRequireSymbols.Validate();

            return valRequirePassword.IsValid
                && valRequireConfirmPassword.IsValid
                && valRequireEqualPassword.IsValid
                && (!valCorrectLength.Enabled || valCorrectLength.IsValid)
                && (!valRequireNumbers.Enabled || valRequireNumbers.IsValid)
                && (!valRequireUppercase.Enabled || valRequireUppercase.IsValid)
                && (!valRequireSymbols.Enabled || valRequireSymbols.IsValid);
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
            get { return (ViewState["MaximumLength"] != null) ? (int)ViewState["MaximumLength"] : 50; }
            set
            {
                {
                    txtPassword.MaxLength = value;
                    txtConfirmPassword.MaxLength = value;
                    ViewState["MaximumLength"] = value;
                }
            }
        }

        public int MinimumNumbers
        {
            get { return (ViewState["MinimumNumbers"] != null) ? (int)ViewState["MinimumNumbers"] : 0; }
            set { ViewState["MinimumNumbers"] = value; }
        }

        public int MinimumUppercase
        {
            get { return (ViewState["MinimumUppercase"] != null) ? (int)ViewState["MinimumUppercase"] : 0; }
            set { ViewState["MinimumUppercase"] = value; }
        }

        public int MinimumSymbols
        {
            get { return (ViewState["MinimumSymbols"] != null) ? (int)ViewState["MinimumSymbols"] : 0; }
            set { ViewState["MinimumSymbols"] = value; }
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

        public void SetUserPolicy(OrganizationPasswordSettings settings)
        {
            int minimumUppercase;
            int minimumNumbers;
            int minimumSymbols;


            if (settings.PasswordComplexityEnabled)
            {
                minimumUppercase = settings.UppercaseLettersCount;
                minimumNumbers = settings.NumbersCount;
                minimumSymbols = settings.SymbolsCount;
            }
            else
            {
                minimumUppercase = 0;
                minimumNumbers = 0;
                minimumSymbols = 0;
            }

            PolicyValue = string.Join(";", true, settings.MinimumLength, settings.MaximumLength, minimumUppercase, minimumNumbers, minimumSymbols, true);

            ToggleControls();
        }

        public void SetUserPolicy(bool enabled, int minLength, int maxLength, int minimumUppercase, int minimumNumbers, int minimumSymbols, bool notEqualToUsername)
        {
            PolicyValue = string.Join(";", enabled, minLength, maxLength, minimumUppercase, minimumNumbers, minimumSymbols, notEqualToUsername);

            ToggleControls();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtPassword.Attributes["autocomplete"] = "new-password";
            txtConfirmPassword.Attributes["autocomplete"] = "new-password";
            txtPassword.Attributes["spellcheck"] = "false";
            txtConfirmPassword.Attributes["spellcheck"] = "false";

            txtPassword.Attributes["value"] = txtPassword.Text;
            txtConfirmPassword.Attributes["value"] = txtConfirmPassword.Text;
            
            ToggleControls();
        }

        
        protected override void OnPreRender(EventArgs e)
        {
            RenderValidationJavaScript();
        }

        private void RenderValidationJavaScript()
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("fcpValidationFunctions"))
            {
              Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "fcpValidationFunctions", @"
                
                function fcpValidatePasswordNumbers(source, args)
                {
                    if(args.Value == source.getAttribute('dpsw')) return true;
                    args.IsValid = fcpValidatePattern(/(\d)/g, args.Value,
                        parseInt(source.getAttribute('minimumNumber')));
                }

                function fcpValidatePasswordUppercase(source, args)
                {
                    if(args.Value == source.getAttribute('dpsw')) return true;
                    args.IsValid = fcpValidatePattern(/([A-Z])/g, args.Value,
                        parseInt(source.getAttribute('minimumNumber')));
                }

                function fcpValidatePasswordSymbols(source, args)
                {
                    if(args.Value == source.getAttribute('dpsw')) return true;
                    args.IsValid = fcpValidatePattern(/([\W_])/g, args.Value,
                        parseInt(source.getAttribute('minimumNumber')));
                }

                function fcpValidatePasswordLength(source, args)
                {
                    if(args.Value == source.getAttribute('dpsw')) return true;
                    args.IsValid = (args.Value.length >= parseInt(source.getAttribute('minimumLength')));
                }
                
                function fcpValidatePattern(re, val, minMatches)
                {
                    var matches = val.match(re);
                    return ((matches != null) && matches.length >= minMatches);
                }
                ", true);


            }
        }

        private void ToggleControls()
        {
            valRequirePassword.Text = String.Empty;
            valRequireConfirmPassword.Text = String.Empty;
            valRequireEqualPassword.Text = String.Empty;
            valCorrectLength.Attributes["dpsw"] = EMPTY_PASSWORD;
            valRequireNumbers.Attributes["dpsw"] = EMPTY_PASSWORD;
            valRequireUppercase.Attributes["dpsw"] = EMPTY_PASSWORD;
            valRequireSymbols.Attributes["dpsw"] = EMPTY_PASSWORD;

            // set empty password
            if (txtPassword.Text == "" && EditMode)
            {
                txtPassword.Attributes["value"] = EMPTY_PASSWORD;
                txtConfirmPassword.Attributes["value"] = EMPTY_PASSWORD;
            }

            // enable/disable require validators
            valRequirePassword.Enabled = ValidationEnabled;
            valRequireConfirmPassword.Enabled = ValidationEnabled;

            // require default length
            MinimumLength = Math.Max(MIN_PASSWORD_LENGTH, MinimumLength);

            // default max length when policy does not provide one
            if (MaximumLength < MIN_PASSWORD_LENGTH)
            {
                MaximumLength = Math.Max(50, txtPassword.MaxLength);
                txtPassword.MaxLength = MaximumLength;
                txtConfirmPassword.MaxLength = MaximumLength;
            }

            // parse and enforce policy
            if (PolicyValue != null)
            {
                bool enabled = false;
                int minLength = -1;
                int maxLength = -1;
                bool notEqualToUsername = false;

                try
                {
                    // parse settings
                    string[] parts = PolicyValue.Split(';');
                    enabled = (parts.Length > 0) && Utils.ParseBool(parts[0], false);
                    minLength = (parts.Length > 1) ? Math.Max(Utils.ParseInt(parts[1], 0), MinimumLength) : MinimumLength;
                    maxLength = (parts.Length > 2) ? Math.Max(Utils.ParseInt(parts[2], 0), MaximumLength) : MaximumLength;
                    MinimumUppercase = (parts.Length > 3) ? Math.Max(Utils.ParseInt(parts[3], 0), MinimumUppercase) : MinimumUppercase;
                    MinimumNumbers = (parts.Length > 4) ? Math.Max(Utils.ParseInt(parts[4], 0), MinimumNumbers) : MinimumNumbers;
                    MinimumSymbols = (parts.Length > 5) ? Math.Max(Utils.ParseInt(parts[5], 0), MinimumSymbols) : MinimumSymbols;
                    notEqualToUsername = (parts.Length > 6) && Utils.ParseBool(parts[6], false);
                }
                catch { /* skip */ }

                // apply policy
                if (enabled)
                {
                    // min length
                    if (minLength > 0)
                    {
                        MinimumLength = minLength;
                        valCorrectLength.Enabled = true;
                        valCorrectLength.Attributes["minimumLength"] = MinimumLength.ToString();
                        valCorrectLength.ErrorMessage = NormalizeValidationMessage(String.Format(GetLocalizedString("CorrectLength.Text"), MinimumLength));
                    }

                    // max length
                    if (maxLength > 0)
                    {
                        MaximumLength = maxLength;
                        txtPassword.MaxLength = maxLength;
                        txtConfirmPassword.MaxLength = maxLength;
                    }

                    // numbers
                    if (MinimumNumbers > 0)
                    {
                        valRequireNumbers.Enabled = true;
                        valRequireNumbers.Attributes["minimumNumber"] = MinimumNumbers.ToString();
                        valRequireNumbers.ErrorMessage = NormalizeValidationMessage(String.Format(
                            GetLocalizedString("RequireNumbers.Text"), MinimumNumbers));
                    }

                    // UPPERCASE
                    if (MinimumUppercase > 0)
                    {
                        valRequireUppercase.Enabled = true;
                        valRequireUppercase.Attributes["minimumNumber"] = MinimumUppercase.ToString();
                        valRequireUppercase.ErrorMessage = NormalizeValidationMessage(String.Format(
                            GetLocalizedString("RequireUppercase.Text"), MinimumUppercase));
                    }

                    // symbols
                    if (MinimumSymbols > 0)
                    {
                        valRequireSymbols.Enabled = true;
                        valRequireSymbols.Attributes["minimumNumber"] = MinimumSymbols.ToString();
                        valRequireSymbols.ErrorMessage = NormalizeValidationMessage(String.Format(
                            GetLocalizedString("RequireSymbols.Text"), MinimumSymbols));
                    }

                } // if(enabled)
            } // if (PolicyValue != null)

                        // Use recommended defaults when policy values are not available.
                        int generatorMaxLength = MaximumLength;
                        int generatorMinUpper = MinimumUppercase;
                        int generatorMinNumbers = MinimumNumbers;
                        int generatorMinSymbols = MinimumSymbols;

                        if (generatorMaxLength < MIN_PASSWORD_LENGTH)
                        {
                                generatorMaxLength = 14;
                                if (generatorMinUpper < 1) generatorMinUpper = 2;
                                if (generatorMinNumbers < 1) generatorMinNumbers = 2;
                                if (generatorMinSymbols < 1) generatorMinSymbols = 2;
                        }

                        int generatorRequiredChars = generatorMinUpper + generatorMinNumbers + generatorMinSymbols;
                        if (generatorRequiredChars >= generatorMaxLength)
                                generatorMaxLength = generatorRequiredChars + 4;

                        // set min password generator
                        lnkGenerate.NavigateUrl = String.Format("javascript:GeneratePassword({0}, {1}, {2}, {3}, '{4}', '{5}');",
                            generatorMaxLength, generatorMinUpper, generatorMinNumbers, generatorMinSymbols, txtPassword.ClientID, txtConfirmPassword.ClientID);
            
        }

        protected void valCorrectLength_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ((args.Value == EMPTY_PASSWORD) || (args.Value.Length >= MinimumLength));
        }

        protected void valRequireNumbers_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ((args.Value == EMPTY_PASSWORD) || ValidatePattern("(\\d)", args.Value, MinimumNumbers));
        }

        protected void valRequireUppercase_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ((args.Value == EMPTY_PASSWORD) || ValidatePattern("([A-Z])", args.Value, MinimumUppercase));
        }

        protected void valRequireSymbols_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ((args.Value == EMPTY_PASSWORD) || ValidatePattern("([\\W_])", args.Value, MinimumSymbols));
        }

        private bool ValidatePattern(string regexp, string val, int minimumNumber)
        {
            return (Regex.Matches(val, regexp).Count >= minimumNumber);
        }

        private static string NormalizePassword(string value)
        {
            return (value ?? String.Empty).Replace("\r", String.Empty).Replace("\n", String.Empty);
        }

        private static string NormalizeValidationMessage(string value)
        {
            return Regex.Replace(value ?? String.Empty, "<br\\s*/?>", String.Empty, RegexOptions.IgnoreCase).Trim();
        }
    }
}
