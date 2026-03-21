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

using FuseCP.Providers.DNS;
using FuseCP.EnterpriseServer;

namespace FuseCP.Portal.ExchangeServer
{
    public partial class ExchangeDomainRecords : FuseCPModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // save return URL
                if (Request.UrlReferrer != null)
                {
                    ViewState["ReturnUrl"] = Request.UrlReferrer.ToString();
                }

                ToggleRecordControls();

                // domain name
                DomainInfo domain = ES.Services.Servers.GetDomain(PanelRequest.DomainID);
                litDomainName.Text = domain.DomainName;
            }

            
            if (PanelSecurity.LoggedUser.Role == UserRole.User)
            {
                if (!PackagesHelper.CheckGroupQuotaEnabled(PanelSecurity.PackageId, ResourceGroups.Dns, Quotas.DNS_EDITOR))
                {
                    this.ExcludeDisableControls.Add(btnBack);
                    this.DisableControls = true;
                }
            }



        }

        public string GetRecordFullData(string recordType, string recordData, int mxPriority, int port)
        {
            switch (recordType)
            {
                case "MX":
                    return String.Format("[{0}], {1}", mxPriority, recordData);
                case "SRV":
                    return String.Format("[{0}], {1}", port, recordData);
                default:
                    return recordData;
            }
        }

        private void GetRecordsDetails(int recordIndex)
        {
            GridViewRow row = gvRecords.Rows[recordIndex];
            ViewState["SrvPort"] = ((Literal)row.Cells[0].FindControl("litSrvPort")).Text;
            ViewState["SrvWeight"] = ((Literal)row.Cells[0].FindControl("litSrvWeight")).Text;
            ViewState["SrvPriority"] = ((Literal)row.Cells[0].FindControl("litSrvPriority")).Text;
            ViewState["MxPriority"] = ((Literal)row.Cells[0].FindControl("litMxPriority")).Text;
            ViewState["RecordName"] = ((Literal)row.Cells[0].FindControl("litRecordName")).Text; ;
            ViewState["RecordType"] = (DnsRecordType)Enum.Parse(typeof(DnsRecordType), ((Literal)row.Cells[0].FindControl("litRecordType")).Text, true);
            ViewState["RecordData"] = ((Literal)row.Cells[0].FindControl("litRecordData")).Text;
        }

        private void BindDnsRecord(int recordIndex)
        {
            try
            {
                ViewState["NewRecord"] = false;
                GetRecordsDetails(recordIndex);

                ddlRecordType.SelectedValue = ViewState["RecordType"].ToString();
                litRecordType.Text = ViewState["RecordType"].ToString();
                txtRecordName.Text = ViewState["RecordName"].ToString();
                txtRecordData.Text = ViewState["RecordData"].ToString();
                txtMXPriority.Text = ViewState["MxPriority"].ToString();
                txtSRVPriority.Text = ViewState["SrvPriority"].ToString();
                txtSRVWeight.Text = ViewState["SrvWeight"].ToString();
                txtSRVPort.Text = ViewState["SrvPort"].ToString();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("GDNS_GET_RECORD", ex);
                return;
            }
        }

        protected void ddlRecordType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToggleRecordControls();
        }

        private void ToggleRecordControls()
        {
            rowMXPriority.Visible = false;
            rowSRVPriority.Visible = false;
            rowSRVWeight.Visible = false;
            rowSRVPort.Visible = false;
            lblRecordData.Text = "Record Data:";
            IPValidator.Enabled = false;

            switch (ddlRecordType.SelectedValue)
            {
                case "A":
                    lblRecordData.Text = "IP:";
                    IPValidator.Enabled = true;
                    break;
                case "AAAA":
                    lblRecordData.Text = "IP (v6):";
                    IPValidator.Enabled = true;
                    break;                    
                case "MX":
                    rowMXPriority.Visible = true;
                    break;
                case "SRV":
                    rowSRVPriority.Visible = true;
                    rowSRVWeight.Visible = true;
                    rowSRVPort.Visible = true;
                    lblRecordData.Text = "Host offering this service:";
                    break;
                default:
                    break;
            }
        }

		protected void Validate(object source, ServerValidateEventArgs args) {
			var ip = args.Value;
			System.Net.IPAddress ipaddr;
			args.IsValid = System.Net.IPAddress.TryParse(ip, out ipaddr) && (ip.Contains(":") || ip.Contains(".")) && 
                ((ddlRecordType.SelectedValue == "A" && ipaddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ||
                (ddlRecordType.SelectedValue == "AAAA" && ipaddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6));
		}
	
        private void SaveRecord()
        {
            if (!Page.IsValid)
                return;

            bool existingRecord = ViewState["ExistingRecord"] != null;

            if (!existingRecord)
            {
                // add record
                try
                {
                    int result = ES.Services.Servers.AddDnsZoneRecord(PanelRequest.DomainID,
                                                                      txtRecordName.Text.Trim(),
                                                                      (DnsRecordType)
                                                                      Enum.Parse(typeof(DnsRecordType),
                                                                                 ddlRecordType.SelectedValue, true),
                                                                      txtRecordData.Text.Trim(),
                                                                      Int32.Parse(txtMXPriority.Text.Trim()),
                                                                      Int32.Parse(txtSRVPriority.Text.Trim()),
                                                                      Int32.Parse(txtSRVWeight.Text.Trim()),
                                                                      Int32.Parse(txtSRVPort.Text.Trim()),
                                                                      0);

                    if (result < 0)
                    {
                        messageBox.ShowResultMessage(result);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    messageBox.ShowErrorMessage("GDNS_ADD_RECORD", ex);
                    return;
                }
            }
            else
            {
                // update record
                try
                {
                    int result = ES.Services.Servers.UpdateDnsZoneRecord(PanelRequest.DomainID,
                                                                         ViewState["RecordName"].ToString(),
                                                                         ViewState["RecordData"].ToString(),
                                                                         txtRecordName.Text.Trim(),
                                                                         (DnsRecordType)ViewState["RecordType"],
                                                                         txtRecordData.Text.Trim(),
                                                                         Int32.Parse(txtMXPriority.Text.Trim()),
                                                                         Int32.Parse(txtSRVPriority.Text.Trim()),
                                                                         Int32.Parse(txtSRVWeight.Text.Trim()),
                                                                         Int32.Parse(txtSRVPort.Text.Trim()),
                                                                         0);

                    if (result < 0)
                    {
                        messageBox.ShowResultMessage(result);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    messageBox.ShowErrorMessage("GDNS_UPDATE_RECORD", ex);
                    return;
                }
            }

            ResetPopup();

            // rebind and switch
            gvRecords.DataBind();
        }

        private void DeleteRecord(int recordIndex)
        {
            try
            {
                GetRecordsDetails(recordIndex);

                int result = ES.Services.Servers.DeleteDnsZoneRecord(PanelRequest.DomainID,
                    ViewState["RecordName"].ToString(),
                    (DnsRecordType)ViewState["RecordType"],
                    ViewState["RecordData"].ToString());

                if (result < 0)
                {
                    messageBox.ShowResultMessage(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorMessage("GDNS_DELETE_RECORD", ex);
                return;
            }

            // rebind grid
            gvRecords.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveRecord();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetPopup();
        }

        private void ResetPopup()
        {
            EditRecordModal.Hide();
            ViewState["ExistingRecord"] = null;

            // erase fields
            litRecordType.Visible = false;
            ddlRecordType.Visible = true;
            ddlRecordType.SelectedIndex = 0;
            txtRecordName.Text = "";
            txtRecordData.Text = "";
            txtMXPriority.Text = "1";
            txtSRVPriority.Text = "0";
            txtSRVWeight.Text = "0";
            txtSRVPort.Text = "0";
            ToggleRecordControls();
        }

        protected void gvRecords_RowEditing(object sender, GridViewEditEventArgs e)
        {
            ViewState["ExistingRecord"] = true;
            BindDnsRecord(e.NewEditIndex);
            EditRecordModal.Show();
            e.Cancel = true;
        }

        protected void gvRecords_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DeleteRecord(e.RowIndex);
            e.Cancel = true;
        }

        protected void odsDnsRecords_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                messageBox.ShowErrorMessage("GDNS_GET_RECORD", e.Exception);
                //this.DisableControls = true;
                e.ExceptionHandled = true;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (ViewState["ReturnUrl"] != null)
                Response.Redirect(ViewState["ReturnUrl"].ToString());
            else
                RedirectToBrowsePage();
        }
    }
}
