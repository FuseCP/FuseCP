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

using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using FuseCP.WebDav.Core;
using FuseCP.WebDavPortal.DependencyInjection;
using FuseCP.WebDavPortal.Models;

namespace FuseCP.WebDavPortal.Constraints
{
    public class OrganizationRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (ScpContext.User == null)
            {
                return false;
            }

            object value;
            if (!values.TryGetValue(routeKey, out value))
                return false;

            var str = value as string;
            if (str == null)
                return false;

            return ScpContext.User.OrganizationId == str;
        }
    }
}
