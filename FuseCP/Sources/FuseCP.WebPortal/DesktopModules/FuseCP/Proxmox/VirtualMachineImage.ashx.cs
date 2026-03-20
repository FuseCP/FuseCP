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

using System.Text;
using System.Web;
using System.Web.Services;
using FuseCP.Providers.ResultObjects;

namespace FuseCP.Portal.Proxmox
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
			int itemId = Utils.ParseInt(context.Request.QueryString["ItemID"]);
			var image = ES.Services.Proxmox.GetVirtualMachineThumbnail(itemId,
				 FuseCP.Providers.Virtualization.ThumbnailSize.Medium160x120);
			
			context.Response.Clear();
			context.Response.ContentType = image?.MimeType ?? "image/png";

			if (image != null)
			{
				context.Response.OutputStream.Write(image.RawData, 0, image.RawData.Length);
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
