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

using System.Linq;
using Microsoft.AspNetCore.Http;

namespace FuseCP.WebDavPortal.Configurations.ActionSelectors
{
    public static class OwaActionSelector
    {
        public static string ResolveOperation(IHeaderDictionary headers)
        {
            if (headers == null)
            {
                return string.Empty;
            }

            if (!headers.ContainsKey("X-WOPI-Override"))
            {
                return string.Empty;
            }

            return headers["X-WOPI-Override"].FirstOrDefault() ?? string.Empty;
        }
    }
}
