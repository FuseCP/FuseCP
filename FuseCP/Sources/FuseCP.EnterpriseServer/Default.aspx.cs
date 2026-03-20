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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

// embedded resources
[assembly: WebResource("FuseCP.EnterpriseServer.Images.logo.png", "image/png")]


namespace FuseCP.EnterpriseServer
{
    public partial class DefaultPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // set url
            string url = Request.Url.ToString();
            litUrl.Text = url.Substring(0, url.LastIndexOf("/"));

            // set version
            object[] attrs = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
            if (attrs.Length > 0)
                litVersion.Text = ((AssemblyFileVersionAttribute)attrs[0]).Version;

			/* imgLogo.ImageUrl = Page.ClientScript.GetWebResourceUrl(
				typeof(DefaultPage), "FuseCP.EnterpriseServer.Images.logo.png"); */
        }
    }
}
