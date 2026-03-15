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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using FuseCP.Providers.HostedSolution;
using FuseCP.WebDav.Core;

namespace FuseCP.WebDavPortal.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class OrganizationPasswordPolicyAttribute : ValidationAttribute
    {
        public int ItemId { get; private set; }

        public OrganizationPasswordPolicyAttribute()
        {
            if (ScpContext.User != null)
            {
                ItemId = ScpContext.User.ItemId;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var resultMessages = new List<string>();

                var settings = ScpContext.Services.Organizations.GetOrganizationPasswordSettings(ItemId);

                if (settings != null)
                {
                    var valueString = value.ToString();

                    if (valueString.Length < settings.MinimumLength)
                    {
                        resultMessages.Add(string.Format(Resources.Messages.PasswordMinLengthFormat,
                            settings.MinimumLength));
                    }

                    if (valueString.Length > settings.MaximumLength)
                    {
                        resultMessages.Add(string.Format(Resources.Messages.PasswordMaxLengthFormat,
                            settings.MaximumLength));
                    }

                    if (settings.PasswordComplexityEnabled)
                    {
                        var numbersCount = valueString.Count(Char.IsDigit);
                        var upperLetterCount = valueString.Count(Char.IsUpper);
                        var symbolsCount = Regex.Matches(valueString, @"[~!@#$%^&*_\-+'\|\\(){}\[\]:;\""'<>,.?/]").Count;

                        if (upperLetterCount < settings.UppercaseLettersCount)
                        {
                            resultMessages.Add(string.Format(Resources.Messages.PasswordUppercaseCountFormat,
                                settings.UppercaseLettersCount));
                        }

                        if (numbersCount < settings.NumbersCount)
                        {
                            resultMessages.Add(string.Format(Resources.Messages.PasswordNumbersCountFormat,
                                settings.NumbersCount));
                        }

                        if (symbolsCount < settings.SymbolsCount)
                        {
                            resultMessages.Add(string.Format(Resources.Messages.PasswordSymbolsCountFormat,
                                settings.SymbolsCount));
                        }
                    }

                }

                return resultMessages.Any()?  new ValidationResult(string.Join("<br>", resultMessages)) : ValidationResult.Success;
            }

            return ValidationResult.Success;
        }
    }
}
