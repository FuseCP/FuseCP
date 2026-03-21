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
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Threading;
using System.Threading.Tasks;

using FuseCP.Providers.OS;

namespace FuseCP.Portal
{
	public partial class ServersEditServer : FuseCPModuleBase
	{
		private static readonly TimeSpan NonCriticalLoadTimeout = TimeSpan.FromSeconds(8);
		private const string DiagnosticsUnavailableText = "Server unavailable";

		int ServerId;
		Task<ServerInfo> serverInfo = null;
		async Task<ServerInfo> ServerInfo()
		{
			try
			{
				lock (this)
				{
					if (serverInfo == null)
					{
						serverInfo = ES.Services.Servers.GetServerByIdAsync(ServerId);
					}
				}
				return await serverInfo;
			}
			catch (Exception)
			{
				throw;

			}
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			Page.Load += PageLoadAsync;
		}
		protected async void PageLoadAsync(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				try
				{
					ServerId = PanelRequest.ServerId;
					await Task.WhenAll(BindTools(), BindServer());
					await Task.WhenAll(
						LoadNonCriticalAsync(BindServerMemory, SetMemoryDiagnosticsUnavailable),
						LoadNonCriticalAsync(BindServerVersion, () => SetDiagnosticsText(fcpVersion, DiagnosticsUnavailableText)),
						LoadNonCriticalAsync(BindServerFilepath, () => SetDiagnosticsText(fcpFilepath, DiagnosticsUnavailableText)));
				}
				catch (Exception ex)
				{
					ShowErrorMessage("SERVER_GET_SERVER", ex);
					return;
				}

				IPAddressesHeader.IsCollapsed = IsIpAddressesCollapsed;
			}
		}

		private async Task LoadNonCriticalAsync(Func<Task> action, Action onFailure)
		{
			try
			{
				await WithTimeoutAsync(action());
			}
			catch (Exception)
			{
				onFailure?.Invoke();
			}
		}

		private static async Task WithTimeoutAsync(Task task)
		{
			Task completedTask = await Task.WhenAny(task, Task.Delay(NonCriticalLoadTimeout));
			if (completedTask != task)
				throw new TimeoutException();

			await task;
		}

		private void SetMemoryDiagnosticsUnavailable()
		{
			SetDiagnosticsText(freeMemory, "N/A");
			SetDiagnosticsText(totalMemory, "N/A");
		}

		private static void SetDiagnosticsText(WebControl control, string value)
		{
			if (control is null)
				return;

			switch (control)
			{
				case Label label:
					label.Text = value;
					break;
			}
		}
		//protected void rbUsersCreationMode_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//    if (this.rbUsersCreationMode.SelectedValue == "1")
		//    {
		//        this.trAuthType.Visible = true;
		//    }

		//    else
		//    {
		//        this.trAuthType.Visible = false;
		//    }
		//}
		//protected void ddlAdAuthType_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//    if (this.ddlAdAuthType.SelectedValue == "Secure")
		//    {
		//        this.trAddDomain.Visible = true;
		//        this.trAdUserName.Visible = true;
		//        this.trAdPassword.Visible = true;
		//        this.trAdButton.Visible = true;
		//    }
		//    if (this.ddlAdAuthType.SelectedValue == "Delegation")
		//    {
		//        this.trAddDomain.Visible = true;
		//        this.trAdUserName.Visible = true;
		//        this.trAdPassword.Visible = true;
		//        this.trAdButton.Visible = true;
		//    }
		//    else
		//    {
		//        this.trAddDomain.Visible = false;
		//        this.trAdUserName.Visible = false;
		//        this.trAdPassword.Visible = false;
		//        this.trAdButton.Visible = false;
		//    }
		//}
		private async Task BindTools()
		{
			try
			{
				if (lnkTerminalSessions != null)
					lnkTerminalSessions.NavigateUrl = EditUrl("ServerID", ServerId.ToString(), "edit_termservices");

				if (lnkWindowsServices != null)
					lnkWindowsServices.NavigateUrl = EditUrl("ServerID", ServerId.ToString(), "edit_winservices");
				if (lnkUnixServices != null)
					lnkUnixServices.NavigateUrl = EditUrl("ServerID", ServerId.ToString(), "edit_winservices");
				if (lnkWindowsProcesses != null)
					lnkWindowsProcesses.NavigateUrl = EditUrl("ServerID", ServerId.ToString(), "edit_processes");
				if (lnkEventViewer != null)
					lnkEventViewer.NavigateUrl = EditUrl("ServerID", ServerId.ToString(), "edit_eventviewer");
				if (lnkServerReboot != null)
				{
					lnkServerReboot.NavigateUrl = EditUrl("ServerID", ServerId.ToString(), "edit_reboot");
					lnkServerReboot.Visible = true;
				}

				if (lnkBackup != null)
					lnkBackup.NavigateUrl = EditUrl("ServerID", ServerId.ToString(), "backup");
				if (lnkRestore != null)
					lnkRestore.NavigateUrl = EditUrl("ServerID", ServerId.ToString(), "restore");

				bool hasBackupPage = PortalUtils.PageExists("Backup");
				if (lnkBackup != null) lnkBackup.Visible = hasBackupPage;
				if (lnkRestore != null) lnkRestore.Visible = hasBackupPage;

				var serverInfo = await ServerInfo();
				bool isWindows = serverInfo.OSPlatform == OSPlatform.Windows;
				if (pnTerminalPanel != null) pnTerminalPanel.Visible = isWindows;
				if (pnWindowsServices != null) pnWindowsServices.Visible = isWindows;
				if (pnUnixServices != null) pnUnixServices.Visible = !isWindows;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task BindServer()
		{
			ServerInfo server = await ServerInfo();

			if (server == null)
				RedirectToBrowsePage();

			// header
			txtName.Text = PortalAntiXSS.DecodeOld(server.ServerName);
			txtComments.Text = PortalAntiXSS.DecodeOld(server.Comments);


			// connection
			txtUrl.Text = server.ServerUrl;

			// AD
			rbUsersCreationMode.SelectedIndex = server.ADEnabled ? 1 : 0;
			Utils.SelectListItem(ddlAdAuthType, server.ADAuthenticationType);
			txtDomainName.Text = server.ADRootDomain;
			txtAdUsername.Text = server.ADUsername;
			txtAdParentDomain.Text = server.ADParentDomain;
			txtAdParentDomainController.Text = server.ADParentDomainController;

			chkUseAdParentDomain.Checked = !string.IsNullOrEmpty(server.ADParentDomain);

			chkUseAdParentDomain_StateChanged(null, null);

			// Preview Domain
			txtPreviewDomain.Text = server.InstantDomainAlias;

			bool hasStoredCredential = !string.IsNullOrWhiteSpace(server.Password);
			string credentialMode = server.PasswordIsSHA256 ? "SHA-256" : "legacy SHA-1";
			lblServerCredentialState.Text = hasStoredCredential
				? $"Stored {credentialMode} credential present"
				: "No stored credential";
			lblServerCredentialState.CssClass = !hasStoredCredential
				? "badge bg-danger"
				: server.PasswordIsSHA256
					? "badge bg-success"
					: "badge bg-warning text-dark";
			lblServerCredentialGuidance.Text = hasStoredCredential
				? "Portal only edits the server URL. Direct GUI and web API password reset is disabled. Rotate the secret on the server host, then reconcile Enterprise with Recover-ServerCredential.ps1."
				: "Portal only edits the server URL. FuseCP does not currently have a stored server credential. Recover on the server host, then reconcile Enterprise with Recover-ServerCredential.ps1.";
		}

		private async Task BindServerVersion()
		{
			fcpVersion.Text = await ES.Services.Servers.GetServerVersionAsync(ServerId);
		}

		private async Task BindServerMemory()
		{
			try
			{
				//Memory memory = await ES.Services.Servers.GetMemoryAsync(ServerId);
                SystemMemoryInfo memory = null;
                // We need to get the ServiceInfo for VPS2012 servers, because only this way allows access to the Remote Hyper-V API.
                // Otherwise, it will return information about the local server.
                ServiceInfo ServiceInfo = (await ES.Services.Servers.GetServicesByServerIdGroupNameAsync(PanelRequest.ServerId, ResourceGroups.VPS2012)).FirstOrDefault();
                if (ServiceInfo != null)
                    memory = await ES.Services.VPS2012.GetSystemMemoryInfoAsync(ServiceInfo.ServiceId); //this is only immportant for Remote Hyper-V
                else
                    memory = await ES.Services.Servers.GetSystemMemoryInfoAsync(PanelRequest.ServerId);

				freeMemory.Text = (memory.FreePhysicalKB / 1024).ToString();
				totalMemory.Text = (memory.TotalVisibleSizeKB / 1024).ToString();
				ramGauge.Total = (int)memory.TotalVisibleSizeKB / 1024;
				ramGauge.Progress = (int)((memory.TotalVisibleSizeKB / 1024) - (memory.FreePhysicalKB / 1024));
			}
			catch (Exception)
			{
				freeMemory.Text = "N/A";
				totalMemory.Text = "N/A";
			}
		}

		private async Task BindServerFilepath()
		{
			fcpFilepath.Text = await ES.Services.Servers.GetServerFilePathAsync(ServerId);
		}

		private void UpdateServer()
		{
			if (!Page.IsValid)
				return;

			ServerInfo server = new ServerInfo();

			// header
			server.ServerId = PanelRequest.ServerId;
			server.ServerName = txtName.Text;
			server.Comments = txtComments.Text;

			// connection
			server.ServerUrl = txtUrl.Text;

			// AD
			server.ADEnabled = (rbUsersCreationMode.SelectedIndex == 1);
			server.ADAuthenticationType = ddlAdAuthType.SelectedValue;
			server.ADRootDomain = txtDomainName.Text;
			server.ADUsername = txtAdUsername.Text;
			server.ADParentDomain = txtAdParentDomain.Text;

			// Preview Domain
			server.InstantDomainAlias = txtPreviewDomain.Text;

			try
			{
				int result = ES.Services.Servers.UpdateServer(server);
				if (result < 0)
				{
					ShowResultMessage(result);
					return;
				}
			}
			catch (Exception ex)
			{
				ShowErrorMessage("SERVER_UPDATE_SERVER", ex);
				return;
			}

			// return to browse page
			RedirectToBrowsePage();
		}

		private void DeleteServer()
		{
			try
			{
				int result = ES.Services.Servers.DeleteServer(PanelRequest.ServerId);
				if (result < 0)
				{
					ShowResultMessage(result);
					return;
				}
			}
			catch (Exception ex)
			{
				ShowErrorMessage("SERVER_DELETE_SERVER", ex);
				return;
			}

			RedirectToBrowsePage();
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			DeleteServer();
		}

		protected void btnUpdate_Click(object sender, EventArgs e)
		{
			UpdateServer();
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			RedirectToBrowsePage();
		}

		protected void btnDiscoverServices_Click(object sender, EventArgs e)
		{
			try
			{
				int result = ES.Services.Servers.DiscoverAndAddServices(PanelRequest.ServerId);
				if (result < 0)
				{
					ShowResultMessage(result);
					return;
				} else
				{
					ServerServicesControl.BindServices();
				}
			}
			catch (Exception ex)
			{
				ShowErrorMessage("SERVER_DISCOVER_SERVICES", ex);
				return;
			}
		}

		protected void btnChangeADPassword_Click(object sender, EventArgs e)
		{
			Page.Validate("ADPassword");
			if (!Page.IsValid)
			{
				return;
			}

			try
			{
				int result = ES.Services.Servers.UpdateServerADPassword(
					 PanelRequest.ServerId, adPassword.Password);
				if (result < 0)
				{
					ShowResultMessage(result);
					return;
				}

				ShowSuccessMessage("SERVER_UPDATE_AD_PSW");
			}
			catch (Exception ex)
			{
				ShowErrorMessage("SERVER_UPDATE_AD_PSW", ex);
				return;
			}
		}

		protected bool IsIpAddressesCollapsed
		{
			get
			{
				return PanelRequest.GetBool("IpAddressesCollapsed", true);
			}
		}

		protected void chkUseAdParentDomain_StateChanged(object sender, EventArgs e)
		{
			//divParentDomain.Visible = chkUseAdParentDomain.Checked;
			//trParentDomainController.Visible = chkUseAdParentDomain.Checked;
			lblAdParentDomain.Visible = chkUseAdParentDomain.Checked;
			lblAdParentDomainController.Visible = chkUseAdParentDomain.Checked;
			txtAdParentDomain.Visible = chkUseAdParentDomain.Checked;
			txtAdParentDomainController.Visible = chkUseAdParentDomain.Checked;

			if (!chkUseAdParentDomain.Checked)
			{
				txtAdParentDomain.Text = null;
			}
		}
	}
}
