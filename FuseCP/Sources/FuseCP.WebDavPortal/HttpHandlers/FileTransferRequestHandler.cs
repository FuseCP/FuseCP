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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FuseCP.WebDavPortal.DependencyInjection;
using FuseCP.WebDavPortal.Models;

namespace FuseCP.WebDavPortal.HttpHandlers
{
    public class FileTransferRequestHandler
    {
        private readonly RequestDelegate _next;

        public FileTransferRequestHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestPath = context.Request.Path.Value;

            if (!string.IsNullOrWhiteSpace(requestPath))
            {
                var relativePath = requestPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var absolutePath = Path.Combine(AppContext.BaseDirectory, relativePath);

                if (File.Exists(absolutePath))
                {
                    await context.Response.SendFileAsync(absolutePath);
                    return;
                }
            }

            await _next(context);
        }
    }
}
