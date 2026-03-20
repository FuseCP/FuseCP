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

﻿using System.Text;
using System.Web;
using System.Web.Services;
using FuseCP.Providers.ResultObjects;

namespace FuseCP.Portal.VPS
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding]
    public class VirtualMachineImage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "image/png";

            int itemId =  Utils.ParseInt(context.Request.QueryString["ItemID"]);
            byte[] buffer = ES.Services.VPS.GetVirtualMachineThumbnail(itemId,
                FuseCP.Providers.Virtualization.ThumbnailSize.Medium160x120);
            if (buffer != null)
            {
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);                
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
