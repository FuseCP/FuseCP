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

namespace FuseCP.Portal
{
    public partial class SelectIPAddress : FuseCPControlBase
    {
        public bool AutoBindOnInitialLoad { get; set; } = true;

        private bool allowEmptySelection = true;
        public bool AllowEmptySelection
        {
            get { return allowEmptySelection; }
            set { allowEmptySelection = value; }
        }

        public string SelectValueText { get; set; }

        private bool useAddressValueAsKey = false;
        public bool UseAddressValueAsKey
        {
            get { return useAddressValueAsKey; }
            set { useAddressValueAsKey = value; }
        }

        private int addressId;
        public int AddressId
        {
            get { return Utils.ParseInt(ddlIPAddresses.SelectedValue, 0); }
            set
            {
                addressId = value;
                ListItem li = ddlIPAddresses.Items.FindByValue(addressId.ToString());
                if (li != null)
                {
                    // deselect previous item
                    ddlIPAddresses.SelectedIndex = -1;
                    li.Selected = true;
                }
            }
        }

        private string addressValue;
        public string AddressValue
        {
            get { return ddlIPAddresses.SelectedValue; }
            set
            {
                addressValue = value;
                ListItem li = ddlIPAddresses.Items.FindByValue(addressValue);
                if (li != null)
                {
                    // deselect previous item
                    ddlIPAddresses.SelectedIndex = -1;
                    li.Selected = true;
                }
            }
        }

        private string serverIdParam;
        public string ServerIdParam
        {
            get { return serverIdParam; }
            set { serverIdParam = value; }
        }

        private int serverId = -1;
        public int ServerId
        {
            get { return serverId; }
            set { serverId = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && AutoBindOnInitialLoad)
            {
                BindIPAddresses();
            }
        }

        public void EnsureBound()
        {
            if (ddlIPAddresses.Items.Count == 0)
            {
                BindIPAddresses();
            }
        }

        private void BindIPAddresses()
        {
            IPAddressInfo[] ips = null;

            if (serverIdParam != null || serverId != -1)
            {
                // get addresses by Server
                if (serverIdParam != null)
                    serverId = Utils.ParseInt(Request[serverIdParam], 0);

                ips = ES.Services.Servers.GetIPAddresses(IPAddressPool.General, serverId);
            }
            else
            {
                // get all IP addresses
                ips = ES.Services.Servers.GetIPAddresses(IPAddressPool.None, serverId);
            }

            // bind IP addresses
            ddlIPAddresses.Items.Clear();

            foreach (IPAddressInfo ip in ips)
            {
                string fullIP = ip.ExternalIP;
                if (ip.InternalIP != null &&
                    ip.InternalIP != "" &&
                    ip.InternalIP != ip.ExternalIP)
                    fullIP += " (" + ip.InternalIP + ")";

                string key = ip.AddressId.ToString();
                if (UseAddressValueAsKey)
                {
                    key = ip.ExternalIP + ";" + ip.InternalIP;
                }

                // add list item
                ddlIPAddresses.Items.Add(new ListItem(fullIP, key));
            }

            // add empty item if required
            if (AllowEmptySelection)
            {
                if (SelectValueText == null)
                    SelectValueText = GetLocalizedString("Text.SelectAddress");
                ddlIPAddresses.Items.Insert(0, new ListItem(SelectValueText, ""));
            }

            // select address by ID
            ListItem li = ddlIPAddresses.Items.FindByValue(addressId.ToString());
            if (li != null)
                li.Selected = true;

            // select address by Value
            li = ddlIPAddresses.Items.FindByValue(addressValue);
            if (li != null)
                li.Selected = true;
        }
    }
}
