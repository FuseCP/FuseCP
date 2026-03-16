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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using FuseCP.EnterpriseServer;

namespace FuseCP.Portal.SkinControls
{
    public partial class Logo : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindLogo();
        }

        private void BindLogo()
        {
            imgLogo.ImageUrl = PortalUtils.GetThemedImage("logo.png");

            // load settings
            if (Page.User.Identity.IsAuthenticated)
            {
                try
                {
                    UserSettings settings = UsersHelper.GetCachedUserSettings(PanelSecurity.EffectiveUserId,
                                                                              UserSettings.FuseCP_POLICY);
                    if (settings != null)
                    {
                        string logoImageURL = settings["LogoImageURL"];
                        if (!String.IsNullOrEmpty(logoImageURL))
                            imgLogo.ImageUrl = logoImageURL;
                    }
                }
                catch
                {
                    // Auth cookie may be stale after a password change or session expiry;
                    // fall back to the default logo rather than crashing the page.
                }
            }
        }
    }
}
