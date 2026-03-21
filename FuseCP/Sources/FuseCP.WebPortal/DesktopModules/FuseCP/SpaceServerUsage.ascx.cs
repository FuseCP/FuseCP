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

using FuseCP.EnterpriseServer;
using FuseCP.Providers.Common;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Web.UI;

namespace FuseCP.Portal
{
    public partial class SpaceServerUsage : FuseCPModuleBase
    {
        private static readonly TimeSpan NonCriticalLoadTimeout = TimeSpan.FromSeconds(10);

        protected void Page_Init(object sender, EventArgs e)
        {
            Page.Load += PageLoadAsync;
        }

        protected async void PageLoadAsync(object sender, EventArgs e)
        {
            this.ContainerControl.Visible = (PanelSecurity.SelectedUser.Role != UserRole.User);
            gaugeUsage.Visible = false;

            if (!IsPostBack)
            {
                FillNA();
                //Timer1.Enabled = true;  //Enable timer to post-load Resource Usage (to prevent slow page loading)
                if (PanelSecurity.PackageId != 1) // PackageId 1 is the serveradmin package
                    await BindSpaceServerUsage();
            }
        }

        /*protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (PanelSecurity.PackageId != 1) // PackageId 1 is the serveradmin package
                BindSpaceServerUsage();

            Timer1.Enabled = false; //disable timer, after getting usage information
        }*/

        private async Task<Providers.OS.SystemResourceUsageInfo> GetSystemResourceUsage()
        {
            PackageInfo packageInfo = PackagesHelper.GetCachedPackage(PanelSecurity.PackageId);
            // TODO: We need to find a way to detect whether other services have a Remote Computer setting.
            // As of 2025, this setting exists only for Hyper-V (VPS2012).
            // In other cases, we assume it's not Hyper-V and they don't have Remote Computer settings.
            ServiceInfo serviceInfo = (await ES.Services.Servers.GetServicesByServerIdGroupNameAsync(packageInfo.ServerId, ResourceGroups.VPS2012)).FirstOrDefault();
            if (serviceInfo != null)
                return await ES.Services.VPS2012.GetSystemResourceUsageInfoAsync(serviceInfo.ServiceId);

            return await ES.Services.Servers.GetSystemResourceUsageInfoAsync(packageInfo.ServerId);
        }

        private async Task BindSpaceServerUsage()
        {
            try
            {
                Providers.OS.SystemResourceUsageInfo resourceUsage = await WithTimeoutAsync(GetSystemResourceUsage());
                int cpuUsage = 0;
                if (resourceUsage.LogicalProcessorUsagePercent != -1)
                {
                    cpuUsage = resourceUsage.LogicalProcessorUsagePercent; //this is more accurate if installed Hyper-V
                    locUsageCpu.Text = "VPS " + locUsageCpu.Text; //GetLocalizedString("locUsageCpu.Text");
                } 
                else
                    cpuUsage = resourceUsage.ProcessorTimeUsagePercent; //this is for everything else

                usageCpu.Text = cpuUsage.ToString();
                cpuGauge.Progress = cpuUsage;
                totalCpu.Text = cpuGauge.Total.ToString();

                freeMemory.Text = (resourceUsage.SystemMemoryInfo.FreePhysicalKB / 1024).ToString();
                totalMemory.Text = (resourceUsage.SystemMemoryInfo.TotalVisibleSizeKB / 1024).ToString();
                ramGauge.Total = (int)resourceUsage.SystemMemoryInfo.TotalVisibleSizeKB / 1024;
                ramGauge.Progress = (int)((resourceUsage.SystemMemoryInfo.TotalVisibleSizeKB / 1024) - (resourceUsage.SystemMemoryInfo.FreePhysicalKB / 1024));
            }
            catch
            {
                FillNA();
            }
            finally
            {
                gaugeUsage.Visible = true;
            }
        }

        private static async Task<T> WithTimeoutAsync<T>(Task<T> task)
        {
            Task completedTask = await Task.WhenAny(task, Task.Delay(NonCriticalLoadTimeout));
            if (completedTask != task)
                throw new TimeoutException();

            return await task;
        }

        private void FillNA()
        {
            usageCpu.Text = totalCpu.Text =
                    freeMemory.Text = totalMemory.Text = "N/A";
        }
    }
}
