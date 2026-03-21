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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using FuseCP.EnterpriseServer;
using FuseCP.EnterpriseServer.Base.HostedSolution;

namespace FuseCP.Portal
{
    public partial class SpaceQuotas : FuseCPModuleBase
    {
		//
		private PackageContext cntx;
		
		// Brief quotas mapping hash
		// In case if you add new brief quota: just update this collection with quota & table row control ids
		// The code will do the rest...
		private readonly Dictionary<string, string> briefQuotaHash = new Dictionary<string, string>
		{
			// QUOTA CONTROL ID <=> TABLE ROW CONTROL ID
			{ "quotaDiskspace", "pnlDiskspace" },
            { "quotaBandwidth", "pnlBandwidth" },
			{ "quotaDomains", "pnlDomains" },
			{ "quotaSubDomains", "pnlSubDomains" },
			//{ "quotaDomainPointers", "pnlDomainPointers" },
            { "quotaOrganizations", "pnlOrganizations" },
            { "quotaUserAccounts", "pnlUserAccounts" },
            { "quotaDeletedUsers", "pnlDeletedUsers" },
			{ "quotaMailAccounts", "pnlMailAccounts" },
            { "quotaExchangeAccounts", "pnlExchangeAccounts" },
            { "quotaOCSUsers", "pnlOCSUsers" },
            { "quotaLyncUsers", "pnlLyncUsers" },
            { "quotaLyncPhone", "pnlLyncPhone" },
            { "quotaSfBUsers", "pnlSfBUsers" },
            { "quotaSfBPhone", "pnlSfBPhone" },
            { "quotaBlackBerryUsers", "pnlBlackBerryUsers" },
            { "quotaSharepointSites", "pnlSharepointSites" },            
			{ "quotaWebSites", "pnlWebSites" },
			{ "quotaNumberOfVm", "pnlHyperVForPC" },
            { "quotaFtpAccounts", "pnlFtpAccounts" },
            { "quotaExchangeStorage", "pnlExchangeStorage" },
            { "quotaNumberOfFolders", "pnlFolders" },
            { "quotaEnterpriseStorage", "pnlEnterpriseStorage" },
            { "quotaEnterpriseSharepointSites", "pnlEnterpriseSharepointSites"},
            { "quotaRdsCollections", "pnlRdsCollections"},
            { "quotaRdsServers", "pnlRdsServers"},
            { "quotaRdsUsers", "pnlRdsUsers"},
            { "quotavps2012servers", "pnlVPS2012Servers" },
            { "quotavps2012cpuquota", "pnlVPS2012CpuQuota" },
            { "quotavps2012ramquota", "pnlVPS2012RamQuota" },
            { "quotavps2012hddquota", "pnlVPS2012HddQuota" },
            { "quotamssql2008databases", "pnlMsSQL2008Databases" },
            { "quotamssql2012databases", "pnlMsSQL2012Databases" },
            { "quotamssql2014databases", "pnlMsSQL2014Databases" },
            { "quotamssql2016databases", "pnlMsSQL2016Databases" },
            { "quotamssql2017databases", "pnlMsSQL2017Databases" },
            { "quotamssql2019databases", "pnlMsSQL2019Databases" },
            { "quotamssql2022databases", "pnlMsSQL2022Databases" },
            { "quotamssql2025databases", "pnlMsSQL2025Databases" },
            { "quotamysql5databases", "pnlMySQL5Databases" },
            { "quotamysql8databases", "pnlMySQL8Databases" },
            { "quotamysql9databases", "pnlMySQL9Databases" },
            { "quotamariadbdatabases", "pnlMariaDBDatabases" }
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            // bind quotas
            BindQuotas();

            UserInfo user = UsersHelper.GetUser(PanelSecurity.EffectiveUserId);

            if (user != null)
            {
                PackageContext cntx = PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId);
                if ((user.Role == UserRole.User) & (Utils.CheckQouta(Quotas.EXCHANGE2007_ISCONSUMER, cntx)))
                {
                    btnViewQuotas.Visible = lnkViewDiskspaceDetails.Visible = false;
                }

            }

        }

        private void BindQuotas()
        {
            // load package context
            cntx = PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId);
            var package = ES.Services.Packages.GetPackage(PanelSecurity.PackageId);
            if (cntx == null || package == null)
            {
                // Invalid or inaccessible package context should not crash page rendering.
                lnkViewBandwidthDetails.Visible = false;
                lnkViewDiskspaceDetails.Visible = false;
                return;
            }

            int packageId = package.PackageId;
            lnkViewBandwidthDetails.NavigateUrl = GetNavigateBandwidthDetails(packageId);
			lnkViewDiskspaceDetails.NavigateUrl = GetNavigateDiskspaceDetails(packageId);
        }

        protected string GetNavigateBandwidthDetails(int packageId)
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            return
                PortalUtils.NavigatePageURL("BandwidthReport", PortalUtils.SPACE_ID_PARAM, packageId.ToString(),
                                            "StartDate=" + startDate.Ticks.ToString(),
                                           "EndDate=" + endDate.Ticks.ToString(), "ctl=edit", "moduleDefId=BandwidthReport");
            
        }

		protected string GetNavigateDiskspaceDetails(int packageId)
		{
			return PortalUtils.NavigatePageURL("DiskspaceReport", PortalUtils.SPACE_ID_PARAM, packageId.ToString(),
				"ctl=edit", "moduleDefId=DiskspaceReport");

		}

        protected void btnViewQuotas_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl(PortalUtils.SPACE_ID_PARAM, PanelSecurity.PackageId.ToString(),
                "view_quotas"));
        }

		protected override void OnPreRender(EventArgs e)
		{
            if (cntx == null || cntx.QuotasArray == null)
            {
                base.OnPreRender(e);
                return;
            }

            //
            AddServiceLevelsQuotas();
			//
			SetVisibilityStatus4BriefQuotasBlock();
			//
			base.OnPreRender(e);
		}

		private void SetVisibilityStatus4BriefQuotasBlock()
		{
			foreach (KeyValuePair<string, string> kvp in briefQuotaHash)
			{
				// Lookup for quota control...
				Quota quotaCtl = FindControl(kvp.Key) as Quota;
				Control containerCtl = FindControl(kvp.Value);
				
				// Skip processing if quota or its container ctrl not found
				if (quotaCtl == null || containerCtl == null)
					continue;
				
				// Find out a quota value info within the package context
				QuotaValueInfo qvi = Array.Find<QuotaValueInfo>(
						cntx.QuotasArray, x => x.QuotaName == quotaCtl.QuotaName);

                // Skip processing if quota not defined in the package context
                if (qvi == null)
                {
                    containerCtl.Visible = false;
                    continue;
                }

				// Show or hide corresponding quotas' containers
				switch (qvi.QuotaTypeId)
				{
					case QuotaInfo.BooleanQuota:
						// 1: Quota is enabled;
						// 0: Quota is disabled;
						containerCtl.Visible = (qvi.QuotaAllocatedValue > 0);
						break;
					case QuotaInfo.NumericQuota:
					case QuotaInfo.MaximumValueQuota:
						// -1: Quota is unlimited
						//  0: Quota is disabled
						// xx: Quota is enabled
						containerCtl.Visible = (qvi.QuotaAllocatedValue != 0);
						break;
				}
			}
		}

        private void AddServiceLevelsQuotas()
        {
            if (cntx == null || cntx.QuotasArray == null)
            {
                return;
            }

            var orgs = ES.Services.Organizations.GetOrganizations(PanelSecurity.PackageId, true);
            OrganizationStatistics stats = null;
            if (orgs != null && orgs.FirstOrDefault() != null)
                stats = ES.Services.Organizations.GetOrganizationStatistics(orgs.First().Id);

            foreach (var quota in Array.FindAll<QuotaValueInfo>(
                cntx.QuotasArray, x => x.QuotaName.Contains(Quotas.SERVICE_LEVELS)))
            {
                HtmlTableRow tr = new HtmlTableRow();
                tr.ID = "pnl_" + quota.QuotaName.Replace(Quotas.SERVICE_LEVELS, "").Replace(" ", string.Empty).Trim();
                HtmlTableCell col1 = new HtmlTableCell();
                col1.Attributes["class"] = "SubHead";
                col1.Attributes["nowrap"] = "nowrap";
                Label lbl = new Label();
                lbl.ID = "lbl_" + quota.QuotaName.Replace(Quotas.SERVICE_LEVELS, "").Replace(" ", string.Empty).Trim();
                lbl.Text = quota.QuotaDescription + ":";

                col1.Controls.Add(lbl);

                HtmlTableCell col2 = new HtmlTableCell();
                col2.Attributes["class"] = "Normal";
                Quota quotaControl = (Quota) LoadControl("UserControls/Quota.ascx");
                quotaControl.ID = "quota_" +
                                  quota.QuotaName.Replace(Quotas.SERVICE_LEVELS, "").Replace(" ", string.Empty).Trim();
                quotaControl.QuotaName = quota.QuotaName;
                quotaControl.DisplayGauge = true;

                col2.Controls.Add(quotaControl);

                tr.Controls.Add(col1);
                tr.Controls.Add(col2);
                tblQuotas.Controls.Add(tr);

                if (stats != null)
                {
                    var serviceLevel = stats.ServiceLevels.FirstOrDefault(q => q.QuotaName == quota.QuotaName);
                    if (serviceLevel != null)
                    {
                        quotaControl.QuotaAllocatedValue = serviceLevel.QuotaAllocatedValue;
                    }
                }
            }
        }
    }
}
