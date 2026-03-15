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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using FuseCP.WebDav.Core;

namespace FuseCP.WebDavPortal.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UniqueAdPhoneNumberAttribute : RemoteAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valueString = value as string;

            if (!string.IsNullOrEmpty(valueString) && ScpContext.User != null)
            {
                var attributes =
                    validationContext.ObjectType.GetProperty(validationContext.MemberName)
                        .GetCustomAttributes(typeof(DisplayNameAttribute), true);

                string displayName = attributes != null && attributes.Any()
                    ? (attributes[0] as DisplayNameAttribute).DisplayName
                    : validationContext.DisplayName;


                var result = !ScpContext.Services.Organizations.CheckPhoneNumberIsInUse(ScpContext.User.ItemId, valueString, ScpContext.User.Login);

                return result ? ValidationResult.Success :
                       new ValidationResult(string.Format(Resources.Messages.AlreadyInUse, displayName));
            }

            return ValidationResult.Success;
        }

        public UniqueAdPhoneNumberAttribute(string routeName) : base(routeName) { }
        public UniqueAdPhoneNumberAttribute(string action, string controller) : base(action, controller) { }
        public UniqueAdPhoneNumberAttribute(string action, string controller, 
               string area) : base(action, controller, area) { }
    }
}
