// Copyright (C) 2026 FuseCP
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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using FuseCP.WebDavPortal.Models.Common.DataTable;

namespace FuseCP.WebDavPortal.ModelBinders.DataTables
{
    public class JqueryDataTableModelBinderProvider : IModelBinderProvider
    {
#nullable enable
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(JqueryDataTableRequest))
                return new JqueryDataTableModelBinder();

            return null;
        }
#nullable restore
    }
}
