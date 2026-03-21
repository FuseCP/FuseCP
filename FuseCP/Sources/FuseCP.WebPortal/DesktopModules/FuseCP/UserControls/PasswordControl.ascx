<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PasswordControl.ascx.cs" Inherits="FuseCP.Portal.PasswordControl" %>
<script src="<%= GetRandomPasswordUrl() %>" language="javascript" type="text/javascript"></script>
<script src="/DesktopModules/FuseCP/Scripts/password-visibility.js" type="text/javascript"></script>

<div class="fcp-password-control fcp-form-sheet">
    <div class="row mb-3">
        <label for="<%= txtPassword.ClientID %>" class="col-sm-2 col-form-label fcp-form-label">
            <asp:Localize ID="locPassword" runat="server" meta:resourcekey="locPassword" Text="Password:" />
        </label>
        <div class="col-sm-10">
            <div class="input-group fcp-password-input">
                <!-- Used to stop browsers auto-completing the username box --><input style="display:none" type="text" name="fakeusernameremembered" />
                <!-- Used to stop browsers auto-completing the password box --><input style="display:none" type="password" name="fakepasswordremembered" />
                <span class="input-group-text"><i class="bi bi-lock fs-5" aria-hidden="true"></i></span>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="hideContentOnBlur form-control" type="password" TextMode="Password" MaxLength="50" meta:resourcekey="loctxtPassword" placeholder="Enter your password" autocomplete="new-password" spellcheck="false"></asp:TextBox>
            </div>
            <asp:RequiredFieldValidator ID="valRequirePassword" runat="server" meta:resourcekey="valRequirePassword" Text="" ErrorMessage="Enter password" ControlToValidate="txtPassword" SetFocusOnError="True" Display="Dynamic" CssClass="fcp-validation-message"></asp:RequiredFieldValidator>
        </div>
    </div>
    <div class="row mb-3">
        <label for="<%= txtConfirmPassword.ClientID %>" class="col-sm-2 col-form-label fcp-form-label">
            <asp:Localize ID="locConfirmPassword" runat="server" meta:resourcekey="locConfirmPassword" Text="Confirm Password:" />
        </label>
        <div class="col-sm-10">
            <div class="input-group fcp-password-input">
                <span class="input-group-text">
                    <i class="bi bi-lock fs-5" aria-hidden="true"></i>
                </span>
                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="hideContentOnBlur form-control" type="password" TextMode="Password" MaxLength="50" meta:resourcekey="loctxtConfirmPassword" placeholder="Confirm your password" autocomplete="new-password" spellcheck="false"></asp:TextBox>
            </div>
            <asp:RequiredFieldValidator ID="valRequireConfirmPassword" runat="server" meta:resourcekey="valRequireConfirmPassword" Text="" ErrorMessage="Confirm password" ControlToValidate="txtConfirmPassword" SetFocusOnError="True" Display="Dynamic" CssClass="fcp-validation-message"></asp:RequiredFieldValidator>
        </div>
    </div>
    <% if (ValidationEnabled)
        {%>
    <div class="row mb-3">
        <div class="col-sm-10 offset-sm-2">
            <div class="fcp-password-policy" id="password-hint-popup">
                <h3>Password requirements</h3>
                <ul>
                    <li><%= string.Format("Password should be at least {0} characters", MinimumLength) %></li>
                    <li><%= string.Format("Password should be maximum {0} characters", MaximumLength) %></li>

                    <% if (MinimumUppercase > 0)
                        {%>
                    <li><%= string.Format("Password should contain at least {0} UPPERCASE characters", MinimumUppercase) %></li>
                    <% }%>
                    <% if (MinimumNumbers > 0)
                        {%>
                    <li><%= string.Format("Password should contain at least {0} numbers", MinimumNumbers) %></li>
                    <% }%>
                    <% if (MinimumSymbols > 0)
                        {%>
                    <li><%= string.Format("Password should contain at least {0} non-alphanumeric symbols", MinimumSymbols) %></li>
                    <% }%>
                </ul>
            </div>
        </div>
    </div>
    <% }%>

    <div class="row mb-0">
        <div class="col-sm-10 offset-sm-2">
            <div class="fcp-password-tools d-flex align-items-center gap-2 flex-wrap">
                <ajaxToolkit:PasswordStrength ID="PS" runat="server" TargetControlID="txtPassword" DisplayPosition="RightSide" StrengthIndicatorType="Text"
                    PreferredPasswordLength="10" PrefixText="Strength:" TextCssClass="fcp-password-strength-text" MinimumNumericCharacters="1" MinimumSymbolCharacters="1"
                    RequiresUpperAndLowerCaseCharacters="true" TextStrengthDescriptions="Very Poor;Weak;Average;Strong;Excellent"
                    CalculationWeightings="50;15;15;20" />
                <asp:HyperLink ID="lnkGenerate" runat="server" NavigateUrl="#" meta:resourcekey="lnkGenerate" CssClass="btn btn-outline-primary btn-sm" Visible="true">
                    Generate Random Password
                </asp:HyperLink>
            </div>
            <asp:CompareValidator ID="valRequireEqualPassword" runat="server" ControlToCompare="txtPassword" Text="" ErrorMessage="Both passwords should be identical" Display="Dynamic" ControlToValidate="txtConfirmPassword" meta:resourcekey="valRequireEqualPassword" SetFocusOnError="True" CssClass="fcp-validation-message"></asp:CompareValidator>
            <asp:CustomValidator ID="valCorrectLength" runat="server" ControlToValidate="txtPassword" ErrorMessage="len" Display="Dynamic" Enabled="false" ClientValidationFunction="fcpValidatePasswordLength" OnServerValidate="valCorrectLength_ServerValidate" SetFocusOnError="True" CssClass="fcp-validation-message"></asp:CustomValidator>
            <asp:CustomValidator ID="valRequireNumbers" runat="server" ControlToValidate="txtPassword" ErrorMessage="num" Display="Dynamic" Enabled="false" ClientValidationFunction="fcpValidatePasswordNumbers" OnServerValidate="valRequireNumbers_ServerValidate" SetFocusOnError="True" CssClass="fcp-validation-message"></asp:CustomValidator>
            <asp:CustomValidator ID="valRequireUppercase" runat="server" ControlToValidate="txtPassword" ErrorMessage="upp" Display="Dynamic" Enabled="false" ClientValidationFunction="fcpValidatePasswordUppercase" OnServerValidate="valRequireUppercase_ServerValidate" SetFocusOnError="True" CssClass="fcp-validation-message"></asp:CustomValidator>
            <asp:CustomValidator ID="valRequireSymbols" runat="server" ControlToValidate="txtPassword" ErrorMessage="sym" Display="Dynamic" Enabled="false" ClientValidationFunction="fcpValidatePasswordSymbols" OnServerValidate="valRequireSymbols_ServerValidate" SetFocusOnError="True" CssClass="fcp-validation-message"></asp:CustomValidator>
        </div>
    </div>
</div>
