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

namespace FuseCP.Portal
{
    public partial class ServerServicesControl : FuseCPControlBase
    {
        private const string ServicesLoadedViewStateKey = "ServicesLoaded";

        DataSet dsServices = null;

        private bool ServicesLoaded
        {
            get { return (bool?)ViewState[ServicesLoadedViewStateKey] ?? false; }
            set { ViewState[ServicesLoadedViewStateKey] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ServicesLoaded = false;
                UpdateServicesVisibility();
            }
        }

        public void BindServices()
        {
            ServicesLoaded = true;
            dsServices = ES.Services.Servers.GetRawServicesByServerId(PanelRequest.ServerId);
            dlServiceGroups.DataSource = dsServices.Tables[0];
            dlServiceGroups.DataBind();
            UpdateServicesVisibility();
        }

        public DataView GetGroupServices(int groupId)
        {
            return new DataView(dsServices.Tables[1], "GroupID=" + groupId.ToString(), "", DataViewRowState.CurrentRows);
        }

        public string EditServiceUrl(string key, string keyVal, string ctrlKey)
        {
            return HostModule.EditUrl(key, keyVal, ctrlKey, "ServerID=" + PanelRequest.ServerId);
        }

        private void UpdateServicesVisibility()
        {
            pnlLoadServices.Visible = !ServicesLoaded;
            dlServiceGroups.Visible = ServicesLoaded;
        }

        protected void btnLoadServices_Click(object sender, EventArgs e)
        {
            BindServices();
        }

        private void linkAddService_Click(object sender, System.EventArgs e)
        {
            Response.Redirect(HostModule.EditUrl("ServerID", PanelRequest.ServerId.ToString(), "add_service"), true);
        }
    }
}
