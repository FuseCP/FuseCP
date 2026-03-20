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

﻿using System;
using System.Collections.Generic;
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
    public class VirtualMachineSnapshotImage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "image/png";

            HttpRequest req = context.Request;

            int itemId = Utils.ParseInt(req["ItemID"]);
            string snapshotId = req["SnapshotID"];

            byte[] res = ES.Services.VPS.GetSnapshotThumbnail(itemId, snapshotId,
                FuseCP.Providers.Virtualization.ThumbnailSize.Medium160x120);
            if (res != null)
            {
                context.Response.OutputStream.Write(res, 0, res.Length);
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
