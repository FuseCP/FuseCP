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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using FuseCP.WebDavPortal.Models.Common.DataTable;


namespace FuseCP.WebDavPortal.ModelBinders.DataTables
{
    public class JqueryDataTableModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string GetValue(string key)
            {
                return bindingContext.ValueProvider.GetValue(key).FirstValue;
            }

            int ParseInt(string key)
            {
                var value = GetValue(key);
                return int.TryParse(value, out var result) ? result : 0;
            }

            bool ParseBool(string key)
            {
                var value = GetValue(key);
                return bool.TryParse(value, out var result) && result;
            }

            // Retrieve request data
            int draw = ParseInt("draw");
            int start = ParseInt("start");
            int count = ParseInt("length");

            // Search
            var search = new JqueryDataTableSearch
            {
                Value = GetValue("search[value]"),
                IsRegex = ParseBool("search[regex]")
            };

            var orderIndex = 0;

            var orders = new List<JqueryDataTableOrder>();

            while (!string.IsNullOrEmpty(GetValue("order[" + orderIndex + "][column]")))
            {
                orders.Add(new JqueryDataTableOrder()
                {
                    Column = ParseInt("order[" + orderIndex + "][column]"),
                    Ascending = (GetValue("order[" + orderIndex + "][dir]") == "asc")
                });

                orderIndex++;
            }

            // Columns
            var columnsIndex = 0;
            var columns = new List<JqueryDataTableColumn>();

            while (!string.IsNullOrEmpty(GetValue("columns[" + columnsIndex + "][name]")))
            {
                columns.Add(new JqueryDataTableColumn
                {
                    Data = GetValue("columns[" + columnsIndex + "][data]"),
                    Name = GetValue("columns[" + columnsIndex + "][name]"),
                    Orderable = ParseBool("columns[" + columnsIndex + "][orderable]"),
                    Search = new JqueryDataTableSearch
                    {
                        Value = GetValue("columns[" + columnsIndex + "][search][value]"),
                        IsRegex = ParseBool("columns[" + columnsIndex + "][search][regex]")
                    }
                });

                columnsIndex++;
            }

            bindingContext.Result = ModelBindingResult.Success(new JqueryDataTableRequest
            {
                Draw = draw,
                Start = start,
                Count = count,
                Search = search,
                Orders = orders,
                Columns = columns
            });

            return Task.CompletedTask;
        }
         
    }
}
