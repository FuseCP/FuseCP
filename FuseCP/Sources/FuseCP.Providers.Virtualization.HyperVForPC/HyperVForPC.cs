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
using System.Text;
using FuseCP.Providers.Utils;
using System.Management;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FuseCP.Server.Utils;
using System.Linq;
using System.Management.Automation.Runspaces;

using Vds = Microsoft.Storage.Vds;
using System.Configuration;

using FuseCP.Providers.Virtualization;
using FuseCP.Providers.VirtualizationForPC.SVMMService;
using FuseCP.Providers.VirtualizationForPC.MonitoringWebService;

using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace FuseCP.Providers.VirtualizationForPC.MonitoringWebService
{
	public partial class PerformanceDataValue
	{
		public ExtensionDataObject ExtensionData { get; set; }
	}
}

namespace FuseCP.Providers.VirtualizationForPC
{
	public static class PowerShellExtensions
	{
		public const string VirtualMachineManagerSnapIn = "Microsoft.SystemCenter.VirtualMachineManager";
		public const string GetVMMServerByName = "Get-VMMServer -ComputerName \"{0}\" -TCPPort \"{1}\"";
		public const string GetVirtualMachineById = "$VirtualMachine = Get-VM -ID \"{0}\"";
		public const string NewPhysicalAddress = "$NicMacAddress = New-PhysicalAddress";
		public const string NewVirtualNetworkAdapterWithVLan = "New-VirtualNetworkAdapter -VirtualNetwork \"{0}\" -JobGroup {1} -PhysicalAddress $NicMacAddress -PhysicalAddressType \"Static\" -NetworkLocation \"\" -NetworkTag \"\" -VLanEnabled $true -VLanId {2} -MACAddressesSpoofingEnabled $false";
		public const string NewVirtualNetworkAdapterWithoutVLan = "New-VirtualNetworkAdapter -VirtualNetwork \"{0}\" -JobGroup {1} -PhysicalAddress $NicMacAddress -PhysicalAddressType \"Static\" -NetworkLocation \"\" -NetworkTag \"\" -MACAddressesSpoofingEnabled $false";

		/// <summary>
		/// Returns true if no warnings have been issued
		/// </summary>
		/// <param name="ps"></param>
		/// <returns></returns>
		public static void AddSystemCenterSnapIn(this PowerShell ps)
		{
			ps.AddScript("Add-PSSnapin -Name \"{0}\" -ErrorAction SilentlyContinue", VirtualMachineManagerSnapIn);
		}

		public static void AddConnectionScript(this PowerShell ps)
		{
			ps.AddScript(GetVMMServerByName, ConfigurationManager.AppSettings["SCVMMServerName"], ConfigurationManager.AppSettings["SCVMMServerPort"]);
		}

		public static void AddScript(this PowerShell ps, string script, params object[] args)
		{
			ps.AddScriptWithTrace(String.Format(script, args));
		}

		public static void AddScript(this PowerShell ps, string script, bool useLocalScope, params object[] args)
		{
			ps.AddScript(String.Format(script, args), useLocalScope);
		}

		public static PowerShell AddScriptWithTrace(this PowerShell ps, string script)
		{
			Log.WriteInfo(script);
			return ps.AddScript(script);
		}

		public static Collection<PSObject> NewVirtualNetworkAdapter(this PowerShell ps, string virtualNetwork, ushort? vlanId, Guid jobGroup)
		{
			//
			var results = default(Collection<PSObject>);
			//
			try
			{
				// Snap-in registration
				ps.AddSystemCenterSnapIn();
				//
				ps.AddConnectionScript();
				ps.AddScriptWithTrace(NewPhysicalAddress);
				// This is for private network
				if (vlanId.HasValue && vlanId.Value > ushort.MinValue)
				{
					ps.AddScript(NewVirtualNetworkAdapterWithVLan, virtualNetwork, jobGroup, vlanId);
				}
				else // This is for public network
				{
					ps.AddScript(NewVirtualNetworkAdapterWithoutVLan, virtualNetwork, jobGroup);
				}
				//
				results = ps.InvokeAndDumpResults();
				//
				return results;
			}
			catch (Exception e)
			{
				Log.WriteError(e);
				// Re-throw exception
				throw;
			}
		}

		public static Collection<PSObject> InvokeAndDumpResults(this PowerShell ps)
		{
			var callResult = default(Collection<PSObject>);
			// Create a pipeline
			callResult = ps.Invoke();
			//
			Log.WriteInfo("Entering into dump method");
			//
			try
			{
				foreach (var result in callResult)
				{
					try
					{
						var sb = new StringBuilder();
						//
						foreach (var item in result.Members)
						{
							sb.AppendFormat("{0} => {1} => {2}", item.Name, item.TypeNameOfValue, item.Value);
						}
						//
						Log.WriteInfo(sb.ToString());
					}
					catch
					{
						// Proceed silently...
					}
				}
			}
			catch
			{
				// Proceed silently...
			}
			Log.WriteInfo("Exiting from dump method");
			//
			return callResult;
		}
	}

	public class FCPVirtualMachineManagementServiceClient : VirtualMachineManagementServiceClient, IDisposable
	{
		private static WSHttpBinding CreateWsHttpBinding()
		{
			var binding = new WSHttpBinding(SecurityMode.Message);
			binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
			return binding;
		}

		private static EndpointAddress BuildEndpoint(string remoteAddress, string endpointConfigurationName)
		{
			if (!String.IsNullOrWhiteSpace(remoteAddress))
				return new EndpointAddress(remoteAddress);

			var defaultAddress = ConfigurationManager.AppSettings["SCVMMServer"];
			if (String.IsNullOrWhiteSpace(defaultAddress))
				defaultAddress = "http://localhost/SCVMMService/VirtualMachineManagementService.svc";

			return new EndpointAddress(defaultAddress);
		}

		private static Binding BuildBinding(string endpointConfigurationName)
		{
			return CreateWsHttpBinding();
		}

		public FCPVirtualMachineManagementServiceClient()
			: base(BuildBinding("WSHttpBinding_IVirtualMachineManagementService"), BuildEndpoint(null, "WSHttpBinding_IVirtualMachineManagementService"))
		{
		}

		public FCPVirtualMachineManagementServiceClient(string endpointConfigurationName) :
			base(BuildBinding(endpointConfigurationName), BuildEndpoint(null, endpointConfigurationName))
		{
		}

		public FCPVirtualMachineManagementServiceClient(string endpointConfigurationName, string remoteAddress) :
			base(BuildBinding(endpointConfigurationName), BuildEndpoint(remoteAddress, endpointConfigurationName))
		{
		}

		public FCPVirtualMachineManagementServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
			base(BuildBinding(endpointConfigurationName), remoteAddress)
		{
		}

		public FCPVirtualMachineManagementServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
			base(binding, remoteAddress)
		{
		}

		public void Dispose()
		{
			if ((this.State == CommunicationState.Opened || this.State == CommunicationState.Opening))
				this.Close();
		}

		public VirtualMachineInfo GetVirtualMachineByName(string virtualMachineName) => GetVirtualMachineByNameAsync(virtualMachineName).GetAwaiter().GetResult();
		public HostInfo GetHostById(Guid hostId) => GetHostByIdAsync(hostId).GetAwaiter().GetResult();
		public byte[] GetVirtualSystemThumbnailImage(int imgWidth, int imgHeight, string systemName, string serverName) => GetVirtualSystemThumbnailImageAsync(imgWidth, imgHeight, systemName, serverName).GetAwaiter().GetResult();
		public TemplateInfo GetTemplateById(Guid templateId) => GetTemplateByIdAsync(templateId).GetAwaiter().GetResult();
		public HostClusterInfo GetHostClusterByName(string clusterName) => GetHostClusterByNameAsync(clusterName).GetAwaiter().GetResult();
		public HostInfo GetHostByName(string hostName) => GetHostByNameAsync(hostName).GetAwaiter().GetResult();
		public HardwareProfileInfo[] GetHardwareProfles() => GetHardwareProflesAsync().GetAwaiter().GetResult();
		public void ShutdownVirtualMachine(Guid vmId) => ShutdownVirtualMachineAsync(vmId).GetAwaiter().GetResult();
		public LibraryServerInfo[] GetLibraryServers() => GetLibraryServersAsync().GetAwaiter().GetResult();
		public VMHostRatingInfo[] GetVMHostRatingsByCluster(Guid vmId, bool isMigration, string clusterName) => GetVMHostRatingsByClusterAsync(vmId, isMigration, clusterName).GetAwaiter().GetResult();
		public VirtualMachineInfo NewVirtualMachineFromVM(Guid vmId, string vmName, string description, string owner, LibraryServerInfo library, string sharePath, HardwareProfileInfo hwConfig, Guid? jobGroup) => NewVirtualMachineFromVMAsync(vmId, vmName, description, owner, library, sharePath, hwConfig, jobGroup).GetAwaiter().GetResult();
		public void MoveVirtualMachine(Guid vmId, Guid hostId, string path, bool startVM, bool useLan, bool runAsynchronously, Guid? jobGroup) => MoveVirtualMachineAsync(vmId, hostId, path, startVM, useLan, runAsynchronously, jobGroup).GetAwaiter().GetResult();
		public FuseCP.Providers.VirtualizationForPC.SVMMService.VirtualNetworkInfo[] GetVirtualNetworkByHost(HostInfo host) => GetVirtualNetworkByHostAsync(host).GetAwaiter().GetResult();
		public void StartVirtualMachine(Guid vmId) => StartVirtualMachineAsync(vmId).GetAwaiter().GetResult();
		public void ResumeVirtualMachine(Guid vmId) => ResumeVirtualMachineAsync(vmId).GetAwaiter().GetResult();
		public void PauseVirtualMachine(Guid vmId) => PauseVirtualMachineAsync(vmId).GetAwaiter().GetResult();
		public void StopVirtualMachine(Guid vmId) => StopVirtualMachineAsync(vmId).GetAwaiter().GetResult();
		public void DeleteVirtualMachine(Guid vmId) => DeleteVirtualMachineAsync(vmId).GetAwaiter().GetResult();
		public VirtualNetworkAdapterInfo[] GetVirtualNetworkAdaptersByVM(Guid vmId) => GetVirtualNetworkAdaptersByVMAsync(vmId).GetAwaiter().GetResult();
		public void RemoveVirtualNetworkAdapter(VirtualNetworkAdapterInfo adapter, bool runAsync, Guid? jobGroup) => RemoveVirtualNetworkAdapterAsync(adapter, runAsync, jobGroup).GetAwaiter().GetResult();
		public VMCheckpointInfo NewVirtualMachineCheckpoint(Guid vmId, string name, string description) => NewVirtualMachineCheckpointAsync(vmId, name, description).GetAwaiter().GetResult();
		public void RestoreVirtualMachineCheckpoint(string checkpointId) => RestoreVirtualMachineCheckpointAsync(checkpointId).GetAwaiter().GetResult();
		public void DeleteVirtualMachineCheckpoint(string checkpointId) => DeleteVirtualMachineCheckpointAsync(checkpointId).GetAwaiter().GetResult();
		public TemplateInfo[] GetTemplates() => GetTemplatesAsync().GetAwaiter().GetResult();
		public HostClusterInfo[] GetHostClusters() => GetHostClustersAsync().GetAwaiter().GetResult();
		public HostInfo[] GetHosts() => GetHostsAsync().GetAwaiter().GetResult();
	}

	public class FCPMonitoringServiceClient : MonitoringServiceClient, IDisposable
	{
		private static WSHttpBinding CreateWsHttpBinding()
		{
			var binding = new WSHttpBinding(SecurityMode.Message);
			binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
			return binding;
		}

		private static EndpointAddress BuildEndpoint(string remoteAddress, string endpointConfigurationName)
		{
			if (!String.IsNullOrWhiteSpace(remoteAddress))
				return new EndpointAddress(remoteAddress);

			var defaultAddress = ConfigurationManager.AppSettings["SCOMServer"];
			if (String.IsNullOrWhiteSpace(defaultAddress))
				defaultAddress = "http://localhost/MonitoringWebService/MonitoringService.svc";

			return new EndpointAddress(defaultAddress);
		}

		private static Binding BuildBinding(string endpointConfigurationName)
		{
			return CreateWsHttpBinding();
		}

		public FCPMonitoringServiceClient()
			: base(BuildBinding("WSHttpBinding_IMonitoringService"), BuildEndpoint(null, "WSHttpBinding_IMonitoringService"))
		{
		}

		public FCPMonitoringServiceClient(string endpointConfigurationName) :
			base(BuildBinding(endpointConfigurationName), BuildEndpoint(null, endpointConfigurationName))
		{
		}

		public FCPMonitoringServiceClient(string endpointConfigurationName, string remoteAddress) :
			base(BuildBinding(endpointConfigurationName), BuildEndpoint(remoteAddress, endpointConfigurationName))
		{
		}

		public FCPMonitoringServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
			base(BuildBinding(endpointConfigurationName), remoteAddress)
		{
		}

		public FCPMonitoringServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
			base(binding, remoteAddress)
		{
		}

		public void Dispose()
		{
			if ((this.State == CommunicationState.Opened || this.State == CommunicationState.Opening))
				this.Close();
		}

		public MonitoredObject GetMonitoredObjectByDisplayName(string serviceName, string displayName) => GetMonitoredObjectByDisplayNameAsync(serviceName, displayName).GetAwaiter().GetResult();
		public PerformanceData[] GetSingleVMHyperVCPUCounters(string serverName, string vmName) => GetSingleVMHyperVCPUCountersAsync(serverName, vmName).GetAwaiter().GetResult();
		public PerformanceData[] GetSingleVMHyperVVirtualNetwork(string serverName, string vmName) => GetSingleVMHyperVVirtualNetworkAsync(serverName, vmName).GetAwaiter().GetResult();
		public PerformanceData[] GetSingleVMHyperVGuestMemoryPagesAllocated(string serverName, string vmName) => GetSingleVMHyperVGuestMemoryPagesAllocatedAsync(serverName, vmName).GetAwaiter().GetResult();
		public FuseCP.Providers.VirtualizationForPC.MonitoringWebService.PerformanceDataValue[] GetMonitoringPerformanceValues(string serviceName, PerformanceData perfData, DateTime startRange, DateTime endRange) => GetMonitoringPerformanceValuesAsync(serviceName, perfData, startRange, endRange).GetAwaiter().GetResult();
	}

	public class HyperVForPC : HostingServiceProviderBase, IVirtualizationServerForPC
	{
		#region Constants
		private const string CONFIG_USE_DISKPART_TO_CLEAR_READONLY_FLAG = "FuseCP.HyperV.UseDiskPartClearReadOnlyFlag";
		private const string WMI_VIRTUALIZATION_NAMESPACE = @"root\virtualization";
		private const string WMI_CIMV2_NAMESPACE = @"root\cimv2";

		private const int SWITCH_PORTS_NUMBER = 1024;
		private const string LIBRARY_INDEX_FILE_NAME = "index.xml";
		private const string EXTERNAL_NETWORK_ADAPTER_NAME = "External Network Adapter";
		private const string PRIVATE_NETWORK_ADAPTER_NAME = "Private Network Adapter";
		private const string MANAGEMENT_NETWORK_ADAPTER_NAME = "Management Network Adapter";

		private const string KVP_RAM_SUMMARY_KEY = "VM-RAM-Summary";
		private const string KVP_HDD_SUMMARY_KEY = "VM-HDD-Summary";
		private const Int64 Size1G = 0x40000000;
		private const Int64 Size1M = 0x100000;

		private const int ByteToGbByte = 1073741824;

		#endregion

		private static Dictionary<string, HostInfo> HostinfoByVMName = new Dictionary<string, HostInfo>();

		#region Provider Settings
		/// <summary>
		/// Gets server name from the provider's settings.
		/// </summary>
		protected string ServerNameSettings
		{
			get { return ProviderSettings["ServerName"]; }
		}

		/// <summary>
		/// Gets an action that should take place automatically when starting a virtual machine.
		/// </summary>
		public int AutomaticStartActionSettings
		{
			get { return ProviderSettings.GetInt("StartAction"); }
		}

		protected string MonitoringServerNameSettings
		{
			get { return ProviderSettings["MonitoringServerName"]; }
		}


		/// <summary>
		/// Gets startup delay that should occur automatically when starting a virtual machine.
		/// </summary>
		public int AutomaticStartupDelaySettings
		{
			get { return ProviderSettings.GetInt("StartupDelay"); }
		}

		/// <summary>
		/// Gets an action that should take place automatically when stopping a virtual machine.
		/// </summary>
		public int AutomaticStopActionSettings
		{
			get { return ProviderSettings.GetInt("StopAction"); }
		}

		/// <summary>
		/// Gets a recorvery action that should take place when recovering a virtual machine (Restart only).
		/// </summary>
		public int AutomaticRecoveryActionSettings
		{
			get { return 1 /* Restart */; }
		}

		/// <summary>
		/// Gets CPU reserve setting.
		/// </summary>
		public int CpuReserveSettings
		{
			get { return ProviderSettings.GetInt("CpuReserve"); }
		}

		/// <summary>
		/// Gets CPU limit setting.
		/// </summary>
		public int CpuLimitSettings
		{
			get { return ProviderSettings.GetInt("CpuLimit"); }
		}

		/// <summary>
		/// Gets CPU weight setting.
		/// </summary>
		public int CpuWeightSettings
		{
			get { return ProviderSettings.GetInt("CpuWeight"); }
		}

		/// <summary>
		/// Gets server type (cluster only).
		/// </summary>
		public string ServerType
		{
			get { return ProviderSettings["ServerType"]; }
		}

		//Hyper-V Cloud
		/// <summary>
		/// Gets a DDCTK endpoint URL of System Center Virtual Machine Manager service to connect to.
		/// </summary>
		protected string SCVMMServer
		{
			get { return ProviderSettings[VMForPCSettingsName.SCVMMServer.ToString()]; }
		}

		/// <summary>
		/// Gets a principal name being used to connect to the System Center Virtual Manager DDCTK endpoint.
		/// </summary>
		protected string SCVMMPrincipalName
		{
			get { return ProviderSettings[VMForPCSettingsName.SCVMMPrincipalName.ToString()]; }
		}

		/// <summary>
		/// Gets a DDCTK endpoint URL of System Center Operations Manager service to connect to.
		/// </summary>
		protected string SCOMServer
		{
			get { return ProviderSettings[VMForPCSettingsName.SCOMServer.ToString()]; }
		}

		/// <summary>
		/// Gets a principal name being used to connect to the System Center Operations Manager DDCTK endpoint.
		/// </summary>
		protected string SCOMPrincipalName
		{
			get { return ProviderSettings[VMForPCSettingsName.SCOMPrincipalName.ToString()]; }
		}

		/// <summary>
		/// 
		/// </summary>
		protected string VPServer
		{
			get { return ProviderSettings[VMForPCSettingsName.VPServer.ToString()]; }
		}

		/// <summary>
		/// Gets a DDCTK endpoint URL of System Center Data Protection Manager service to connect to.
		/// </summary>
		protected string SCDPMServer
		{
			get { return ProviderSettings[VMForPCSettingsName.SCDPMServer.ToString()]; }
		}

		/// <summary>
		/// 
		/// </summary>
		protected string SCDPMEndPoint
		{
			get { return ProviderSettings[VMForPCSettingsName.SCDPMEndPoint.ToString()]; }
		}

		/// <summary>
		/// 
		/// </summary>
		protected string SCCMServer
		{
			get { return ProviderSettings[VMForPCSettingsName.SCCMServer.ToString()]; }
		}

		/// <summary>
		/// Gets a DDCTK endpoint URL of SCCM service to connect to.
		/// </summary>
		protected string SCCMEndPoint
		{
			get { return ProviderSettings[VMForPCSettingsName.SCCMEndPoint.ToString()]; }
		}

		/// <summary>
		/// Gets storage endpoint URL.
		/// </summary>
		protected string StorageEndPoint
		{
			get { return ProviderSettings[VMForPCSettingsName.StorageEndPoint.ToString()]; }
		}

		/// <summary>
		/// Gets an endpoint URL to core service.
		/// </summary>
		protected string CoreSvcEndpoint
		{
			get { return ProviderSettings["CoreSvcEndpoint"]; }
		}

		/// <summary>
		/// Gets a configured path to virtual machines library
		/// </summary>
		protected string LibraryPath
		{
			get { return ProviderSettings["LibraryPath"]; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Ctor.
		/// </summary>
		public HyperVForPC()
		{
		}
		#endregion

		#region Virtual Machines
		/// <summary>
		/// Gets a virtual machine information by its identificator.
		/// </summary>
		/// <param name="vmId">Virtual machine id, represented by Guid (for example "4664215D-D195-4E35-BB6F-BFC1F17666EB").</param>
		/// <returns>Virtual machine information, such as Name, HostName, Id, State, count of CPUs assigned, 
		/// CreationTime, ComputerName, Owner, Domain name (if assigned), CPU utilization, CPU performance utilization, RAM</returns>
		public VMInfo GetVirtualMachine(string vmId)
		{
			VMInfo vm = new VMInfo();
			//
			var vmInfo = default(VirtualMachineInfo);
			
			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{
				vmInfo = client.GetVirtualMachineByName(vmId);
				//
				client.Close();
			}

			if (vmInfo == null)
				throw new InvalidDataException(String.Format("GetVirtualMachineByName for VM {0} return NULL value.", vmId));

			vm.logMessage = String.Format("Current state VM {0} is {1}.", vmInfo.Name, vmInfo.Status);

			vm.Name = vmInfo.Name;
			vm.HostName = vmInfo.HostName;
			vm.VmGuid = vmInfo.Id;
			vm.State = (Virtualization.VMComputerSystemStateInfo)vmInfo.Status;
			vm.CPUCount = vmInfo.CPUCount;
			vm.CreatedDate = vmInfo.CreationTime;
			vm.ComputerName = vmInfo.ComputerName;
			vm.Owner = vmInfo.Owner;
			vm.JoinDomain = (vmInfo.VMHost == null ? string.Empty : vmInfo.VMHost.DomainName);
			vm.CPUUtilization = vmInfo.CPUUtilization;
			vm.PerfCPUUtilization = vmInfo.perfCPUUtilization;
			vm.ModifiedTime = "00:00:00";
			vm.Memory = vmInfo.Memory;
			vm.ProcessMemory = vmInfo.Memory;

			if ((vmInfo.VirtualHardDisks != null)
			 && (vmInfo.VirtualHardDisks.Length > 0))
			{
				vm.HddLogicalDisks = new LogicalDisk[vmInfo.VirtualHardDisks.Length];
				for (int i = 0; i < vmInfo.VirtualHardDisks.Length; i++)
				{
					vm.HddLogicalDisks[i] = new LogicalDisk();
					var d = vmInfo.VirtualHardDisks[i];
					if (d != null)
					{
						vm.HddLogicalDisks[i].Size = (int)(d.MaximumSize / ByteToGbByte);
						vm.HddLogicalDisks[i].FreeSpace = (int)(((long)d.MaximumSize - vmInfo.VirtualHardDisks[i].Size) / ByteToGbByte);
						vm.HddLogicalDisks[i].DriveLetter = d.Name;
					}
				}
			}

			vm.ProvisioningStatus = VirtualMachineProvisioningStatus.OK;

			return vm;
		}

		/// <summary>
		/// Gets a thumbnail image for the virtual machine (Wrapper).
		/// </summary>
		/// <param name="vmId">Virtual machine id, represented by Guid (for example "4664215D-D195-4E35-BB6F-BFC1F17666EB").</param>
		/// <param name="size">Size of the thumbnail being requested</param>
		/// <returns>Array of bytes representing the virtual machine thumbnail image requested.</returns>
		public byte[] GetVirtualMachineThumbnailImage(string vmId, ThumbnailSize size)
		{
			return GetTumbnailFromSummaryInformation(vmId, size);
		}

		/// <summary>
		/// Gets a thumbnail image for the virtual machine (Implementation).
		/// </summary>
		/// <param name="vmId">Virtual machine id, represented by Guid (for example "4664215D-D195-4E35-BB6F-BFC1F17666EB").</param>
		/// <param name="size">Size of the thumbnail being requested</param>
		/// <returns>Array of bytes representing the virtual machine thumbnail image requested.</returns>
		private byte[] GetTumbnailFromSummaryInformation(string vmName, ThumbnailSize size)
		{
			int width = 80;
			int height = 60;

			if (size == ThumbnailSize.Medium160x120)
			{
				width = 160;
				height = 120;
			}
			else if (size == ThumbnailSize.Large320x240)
			{
				width = 320;
				height = 240;
			}

			lock (HostinfoByVMName)
			{
				if (!HostinfoByVMName.ContainsKey(vmName))
				{
					using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
					{
						VirtualMachineInfo vminfo = client.GetVirtualMachineByName(vmName);
						if (vminfo != null)
						{
							HostInfo host = client.GetHostById(vminfo.HostId);

							HostinfoByVMName.Add(vmName, host);
						}
					}
				}
			}

			HostInfo hostInfo = null;

			HostinfoByVMName.TryGetValue(vmName, out hostInfo);

			byte[] imgData = null;

			if (hostInfo != null)
			{
				using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
				{
					try
					{
						imgData = client.GetVirtualSystemThumbnailImage(width, height, vmName, hostInfo.ComputerName);
					}
					catch (Exception ex)
					{
						imgData = null;
						//
						Log.WriteError(ex);
					}
				}
			}

			// Create new bitmap
			if (imgData == null)
			{
				using (Bitmap bmp = new Bitmap(width, height))
				{
					Graphics g = Graphics.FromImage(bmp);
					SolidBrush brush = new SolidBrush(Color.LightGray);
					g.FillRectangle(brush, 0, 0, width, height);

					using (MemoryStream stream = new MemoryStream())
					{
						bmp.Save(stream, ImageFormat.Png);
						imgData = stream.ToArray();
					}
				}
			}

			return imgData;
		}

		public VMInfo CreateVirtualMachine(VMInfo vm)
		{
			// Evaluate VM placement options configured
			try
			{
				//
				var jobGroup = new Guid(vm.CurrentTaskId);
				//
				var hostInfo = default(HostInfo);
				//
				#region Find out placement options, e.q. either deploy VM via cluster or directly on a host
				using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
				{
					TemplateInfo selTemplate = client.GetTemplateById(vm.TemplateId);
					//
					if (ServerType.Equals("cluster"))
					{
						HostClusterInfo selCluster = client.GetHostClusterByName(ServerNameSettings);

						if (selCluster.Nodes != null)
						{
							foreach (HostInfo curr in selCluster.Nodes)
							{
								if (curr.AvailableForPlacement)
								{
									hostInfo = curr;
									//
									break;
								}
							}
						}
					}

					if (hostInfo == null)
					{
						try
						{
                            hostInfo = client.GetHostByName(IsNullOrWhiteSpaceString(ServerNameSettings)
								? selTemplate.HostName : ServerNameSettings);
						}
						catch (Exception ex)
						{
							hostInfo = null;
							//
							Log.WriteError(ex);
						}
					}
				}
				#endregion
				//
				using (var ps = PowerShell.Create())
				{
					// Register snap-in
					ps.AddSystemCenterSnapIn();
					// Establish connection
					ps.AddConnectionScript();
					//
					ps.AddScriptWithTrace(new StringBuilder("$MyVM = @{")
						.Append("JobGroup = [System.Guid]::NewGuid();")
						.Append("HWProfileName = [System.String]::Concat(\"Profile\", [System.Guid]::NewGuid());")
						.AppendFormat("Name = \"{0}\";", vm.Name)
						.AppendFormat("TemplateId = \"{0}\";", vm.TemplateId)
						.AppendFormat("VMHostId = \"{0}\";", hostInfo.Id)
						.AppendFormat("CPUCount = {0};", vm.CPUCount)
						.AppendFormat("MemoryMB = {0};", vm.Memory)
						.AppendFormat("CPUMax = {0};", CpuLimitSettings)
						.AppendFormat("CPUReserve = {0};", CpuReserveSettings)
						.AppendFormat("RelativeWeight = {0};", CpuWeightSettings)
						.AppendFormat("NumLock = {0};", vm.NumLockEnabled ? "$true" : "$false")
						.AppendFormat("StartAction = {0};", AutomaticStartActionSettings)
						.AppendFormat("StopAction = {0};", AutomaticStopActionSettings)
						.AppendFormat("DelayStart = {0};", AutomaticStartupDelaySettings)
						.AppendFormat("HighlyAvailable = {0};", ServerType.Equals("cluster") ? "$true" : "$false")
						.Append("}")
						.ToString());
					//
					if (vm.ExternalNetworkEnabled)
					{
						//
						ps.AddScriptWithTrace(new StringBuilder("$MyVM.Nic1 = @{")
							.AppendFormat("VirtualNetwork = \"{0}\";", vm.ExternalVirtualNetwork)
							.Append("MacAddress = (New-PhysicalAddress -Commit);")
							.Append("}")
							.ToString());
						//
						ps.AddScriptWithTrace("New-VirtualNetworkAdapter -VirtualNetwork $MyVM.Nic1.VirtualNetwork -JobGroup $MyVM.JobGroup -PhysicalAddress $MyVM.Nic1.MacAddress -PhysicalAddressType Static -VLanEnabled $false");
					}
					//
					if (vm.PrivateNetworkEnabled)
					{
						//
						ps.AddScriptWithTrace(new StringBuilder("$MyVM.Nic2 = @{")
							.Append("MacAddress = (New-PhysicalAddress -Commit);")
							.AppendFormat("VirtualNetwork = \"{0}\";", vm.PrivateVirtualNetwork)
							.AppendFormat("VLanEnabled = {0};", (vm.PrivateVLanID > ushort.MinValue) ? "$true" : "$false")
							.AppendFormat("VLanId = {0};", vm.PrivateVLanID)
							.Append("}")
							.ToString());
						//
						ps.AddScriptWithTrace("New-VirtualNetworkAdapter -VirtualNetwork $MyVM.Nic2.VirtualNetwork -JobGroup $MyVM.JobGroup -PhysicalAddress $MyVM.Nic2.MacAddress -PhysicalAddressType Static -VLanEnabled $MyVM.Nic2.VLanEnabled -VLanId $MyVM.Nic2.VLanId");
					}
					//
					ps.AddScriptWithTrace("$Template = Get-Template | Where-Object {$_.ID -eq $MyVM.TemplateId}");
					// Retrieve operating system info chosen for the template
					ps.AddScriptWithTrace("$OperatingSystem = Get-OperatingSystem | Where-Object {$_.Name -eq $Template.OperatingSystem}");
					// Retrieve host info we chose for the VM placement
					ps.AddScriptWithTrace("$VMHost = Get-VMHost | Where-Object {$_.ID -eq $MyVM.VMHostId}");
					// Add SCSI Adapters to the hardware profile being created if any
					ps.AddScriptWithTrace("$Template.VirtualSCSIAdapters | ForEach-Object { New-VirtualSCSIAdapter -AdapterID $_.AdapterID  -Shared $_.Shared -JobGroup $MyVM.JobGroup }");
					// Create a hardware profile to be attached to the VM being provisoned
					ps.AddScriptWithTrace("$HardwareProfile = New-HardwareProfile -CPUType $Template.CPUType -Name $MyVM.HWProfileName -Description \"Profile used to create a VM/Template\" -HighlyAvailable $MyVM.HighlyAvailable -CPUCount $MyVM.CPUCount -MemoryMB $MyVM.MemoryMB -CPUMax $MyVM.CPUMax -CPUReserve $MyVM.CPUReserve -RelativeWeight $MyVM.RelativeWeight -NumLock $MyVM.NumLock -JobGroup $MyVM.JobGroup");
					// Create VM with the settings specified
					ps.AddScriptWithTrace("New-VM -Template $Template -Name $MyVM.Name -VMHost $VMHost -Path $VMHost.VMPaths[0] -JobGroup $MyVM.JobGroup -RunAsynchronously -HardwareProfile $HardwareProfile -ComputerName $MyVM.Name -AnswerFile $null -OperatingSystem $OperatingSystem -RunAsSystem -StartAction $MyVM.StartAction -DelayStart $MyVM.DelayStart -StopAction $MyVM.StopAction");
					// Remove the hardware profile we used to instantiate the VM
					ps.AddScriptWithTrace("Remove-HardwareProfile $HardwareProfile");
					//
					ps.InvokeAndDumpResults();
				}

				// Warning: 5 seconds thread sleep
				System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5).Milliseconds);
				//
				var vmWait = default(VirtualMachineInfo);
				//
				while (vmWait == null
					|| vmWait.Status == SVMMService.VMComputerSystemStateInfo.UnderCreation)
				{
					using (var client = GetVMMSClient())
					{
						vmWait = client.GetVirtualMachineByName(vm.Name);
					}
					// Warning: 3 seconds thread sleep
					System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30).Milliseconds);
				}
				// Expand virtual machine disk up to the size requested
				if (vmWait.Status != SVMMService.VMComputerSystemStateInfo.CreationFailed)
				{
					using (var ps = PowerShell.Create())
					{
						ps.AddSystemCenterSnapIn();
						//
						ps.AddConnectionScript();
						//
						ps.AddScript("$MyVM = Get-VM -ID \"{0}\"", vmWait.Id);
						//
						ps.AddScript("$MyVM.VirtualDiskDrives | Where-Object {($_.BusType -eq \"IDE\") -and ($_.Bus -eq 0)} | Expand-VirtualDiskDrive -Size " + vm.HddSize);
						//
						ps.AddScriptWithTrace("Start-VM -VM $MyVM -RunAsynchronously");
						//
						ps.InvokeAndDumpResults();
					}
				}
			}
			catch (Exception ex)
			{
				vm.ProvisioningStatus = VirtualMachineProvisioningStatus.Error;
				// TODO: Possibly we should avoid exposing such detailed exceptions to the end-user
				vm.exMessage = ex.Message;
				// Log the exception occured
				Log.WriteError(ex);
			}
			//
			return vm;
		}

		public VMInfo CreateVMFromVM(string sourceName, VMInfo vmTemplate, Guid taskGuid)
		{
			string paramCreate = String.Empty;
			//
			var steps = new StringBuilder();

			try
			{
				steps.AppendLine("Start Connect to ScVMM (new VirtualMachineManagementServiceClient)");
				//
				using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
				{
					steps.AppendLine("Connected to ScVMM");
					//
					#region Hardware profiles
					steps.AppendLine("Start select Hardware Profle (GetHardwareProfles())");
					HardwareProfileInfo[] hProfiles = client.GetHardwareProfles();

					if (hProfiles == null || hProfiles.Length == 0)
						throw new Exception("No hardware profile found can't continue.");
					steps.AppendLine("Hardware Profle selected");
					#endregion

					steps.AppendLine("Start Get template VM info (GetVirtualMachineByName())");
					//
					VirtualMachineInfo sourceVM = client.GetVirtualMachineByName(sourceName);
					//
					steps.AppendLine("Done Get template VM info");
					//
					if (sourceVM.Status == SVMMService.VMComputerSystemStateInfo.CreationFailed
						&& sourceVM.Status == SVMMService.VMComputerSystemStateInfo.CustomizationFailed
						&& sourceVM.Status == SVMMService.VMComputerSystemStateInfo.UpdateFailed
						&& sourceVM.Status == SVMMService.VMComputerSystemStateInfo.Deleting
						&& sourceVM.Status == SVMMService.VMComputerSystemStateInfo.TemplateCreationFailed
						&& sourceVM.Status == SVMMService.VMComputerSystemStateInfo.UnderCreation
						&& sourceVM.Status == SVMMService.VMComputerSystemStateInfo.UnderTemplateCreation
						&& sourceVM.Status == SVMMService.VMComputerSystemStateInfo.UnderUpdate)
					{
						throw new Exception(String.Format("Creation Failed. Template state = {0}.", sourceVM.Status));
					}

					if (sourceVM.Status != SVMMService.VMComputerSystemStateInfo.PowerOff
						&& sourceVM.Status != SVMMService.VMComputerSystemStateInfo.Stored
						&& sourceVM.Status != SVMMService.VMComputerSystemStateInfo.Saved)
					{
						//
						steps.AppendLine("Template VM Stopping (ShutdownVirtualMachine())");
						//
						client.ShutdownVirtualMachine(sourceVM.Id);

						while (sourceVM.Status != SVMMService.VMComputerSystemStateInfo.Stored
							&& sourceVM.Status != SVMMService.VMComputerSystemStateInfo.PowerOff
							&& sourceVM.Status != SVMMService.VMComputerSystemStateInfo.UpdateFailed)
						{
							System.Threading.Thread.Sleep(5000);
							sourceVM = client.GetVirtualMachineByName(sourceName);
						}

						if (sourceVM.Status == FuseCP.Providers.VirtualizationForPC.SVMMService.VMComputerSystemStateInfo.UpdateFailed)
						{
							throw new Exception(String.Format("Creation Failed. Template not stoped. Current state = {0}", sourceVM.Status));
						}
						//
						steps.AppendLine("Template VM Stoped");
					}

					#region Library
					steps.AppendLine("Start Select library (GetLibraryServers())");
					LibraryServerInfo[] arrLi = client.GetLibraryServers();
					if (arrLi.Length == 0)
						throw new InvalidOperationException("Get library servers returns empty list");

					LibraryServerInfo li = null;

					foreach (var cur in arrLi)
					{
						if (LibraryPath.ToLower().Contains(cur.ComputerName.ToLower()))
						{
							li = cur;
							break;
						}
					}

					if (li == null)
						throw new Exception(string.Format("Library server for share {0} not found.", LibraryPath));

					if (li.Status != ComputerStateInfo.Responding && li.Status != ComputerStateInfo.Pending && li.Status != ComputerStateInfo.Updating)
						throw new InvalidOperationException(string.Format("Library server {0} in invalid state {1}", li.ComputerName, li.Status));

					steps.AppendLine("Library selected");
					#endregion

					paramCreate = String.Format("Params: SourceVM ID: {0}\n New VM Name: {1}\n Owner Name : {2}\n Library : {3}\n Liblary Path:{4}\n Hardware profile:{5}\n"
						, sourceVM.Id, vmTemplate.Name, sourceVM.Owner, (li != null ? li.Name : "unknown"), LibraryPath, hProfiles[0].Name);

					steps.AppendLine("Start Create VM (NewVirtualMachineFromVM())");

					taskGuid = (((taskGuid == null) || (taskGuid == Guid.Empty)) ? Guid.NewGuid() : taskGuid);

					#region GetMovement params
					HostInfo hostInfo = null;
					if (ServerType.Equals("cluster"))
					{
						steps.AppendLine("Start Get Host by Rating (GetVMHostRatingsByCluster())");
						var ratings = client.GetVMHostRatingsByCluster(sourceVM.Id, true, ServerNameSettings).OrderByDescending(item => item.Rating).ToList();
						if (ratings.Count == 0)
							throw new InvalidOperationException("got empty ratings list");

						hostInfo = ratings.ToArray()[0].VMHost;
						steps.AppendLine("Done Get Host by Rating");
					}
					else
					{
						steps.AppendLine("Start Get Host (GetHostByName())");
						hostInfo = client.GetHostByName(ServerNameSettings);
						steps.AppendLine("Done Get Host");
					}
					#endregion
					//
					steps.AppendLine("start NewVirtualMachineFromVM");
					//
					VirtualMachineInfo newVM = client.NewVirtualMachineFromVM(sourceVM.Id
																			, vmTemplate.Name
																			, string.Format("Clone of {0}", sourceVM.Name)
																			, sourceVM.Owner
																			, li
																			, LibraryPath
																			, hProfiles[0]
																			, taskGuid);
					//
					steps.AppendLine("end NewVirtualMachineFromVM");
					//
					steps.AppendFormat("start MoveVirtualMachine {0} to {1} - {2}", newVM.Name, hostInfo.ComputerName, hostInfo.VMPaths[0]).AppendLine();
					//
					client.MoveVirtualMachine(newVM.Id, hostInfo.Id, hostInfo.VMPaths[0], false, true, false, taskGuid);
					//
					steps.AppendLine("end MoveVirtualMachine");

					vmTemplate.VmGuid = newVM.Id;
					vmTemplate.ComputerName = newVM.ComputerName;
					vmTemplate.State = (Virtualization.VMComputerSystemStateInfo)newVM.Status;
					vmTemplate.ProvisioningStatus = VirtualMachineProvisioningStatus.InProgress;
					//
					steps.AppendLine("VM created");
				}
			}
			catch (System.TimeoutException)
			{
				vmTemplate.ProvisioningStatus = VirtualMachineProvisioningStatus.InProgress;
			}
			catch (Exception ex)
			{
				vmTemplate.ProvisioningStatus = VirtualMachineProvisioningStatus.Error;
				// TO-DO: Possibly we should avoid exposing such detailed exceptions to the end-user
				vmTemplate.exMessage = ex.Message + "\n";
				//
				Log.WriteError(ex);
			}
			// 
			vmTemplate.logMessage = paramCreate + steps;

			//// Переносим виртуалку
			//Providers.Virtualization.VMComputerSystemStateInfo state = vmTemplate.State;

			//while (state == Providers.Virtualization.VMComputerSystemStateInfo.UnderCreation)
			//{
			//    System.Threading.Thread.Sleep(10000);
			//    VMInfo stateVmInfo = GetVirtualMachine(vmTemplate.Name);
			//    state = stateVmInfo.State;
			//}

			//if ((state == Providers.Virtualization.VMComputerSystemStateInfo.PowerOff)
			//    || (state == Providers.Virtualization.VMComputerSystemStateInfo.Stored)
			//    || (state == Providers.Virtualization.VMComputerSystemStateInfo.Saved)
			//    || String.IsNullOrEmpty(vmTemplate.exMessage))
			//{
			//    vmTemplate = MoveVM(vmTemplate);
			//}

			return vmTemplate;
		}

		public VMInfo MoveVM(VMInfo vmForMove)
		{
			var steps = new StringBuilder().AppendLine("MoveVM");
			//
			string paramsMove = String.Empty;
			try
			{
				steps.AppendLine("Start Connect to ScVNMM (new VirtualMachineManagementServiceClient)");
				using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
				{
					steps.AppendLine("Connected to ScVNMM");
					//
					steps.AppendLine("Start Get source VM info (GetVirtualMachineByName() )");
					//
					VirtualMachineInfo sourceVM = client.GetVirtualMachineByName(vmForMove.Name);
					//
					steps.AppendLine("Done Get source VM info");

					HostInfo hostInfo = null;

					if (ServerType.Equals("cluster"))
					{
						steps.AppendLine("Start Get Cluster (GetHostClusterByName())");
						//
						HostClusterInfo selCluster = client.GetHostClusterByName(ServerNameSettings);
						//
						steps.AppendLine("Done Get Cluster (GetHostClusterByName())");
						steps.AppendLine("Start Get Host by Rating (GetVMHostRatingsByCluster())");
						//
						VMHostRatingInfo overHost = client.GetVMHostRatingsByCluster(sourceVM.Id, true, ServerNameSettings)
							   .OrderByDescending(item => item.Rating).ToArray()[0];

						if (overHost != null)
						{
							hostInfo = overHost.VMHost;
						}
						//
						steps.AppendLine("Done Get Host by Rating");
					}
					else
					{
						steps.AppendLine("Start Get Host (GetHostByName())");
						hostInfo = client.GetHostByName(ServerNameSettings);
						steps.AppendLine("Done Get Host");
					}

					if (hostInfo == null)
					{
						throw new Exception("Host not found.");
					}

					paramsMove = String.Format("VM Id: {0}\n Host Id: {1}\n VM Path: {2}\n", sourceVM.Id, hostInfo.Id, hostInfo.VMPaths[0]);
					//
					steps.AppendLine("Start Move VM (MoveVirtualMachine)");
					//
					client.MoveVirtualMachine(sourceVM.Id, hostInfo.Id, hostInfo.VMPaths[0], false, true, true, null);
					//
					steps.AppendLine("Done Move VM (MoveVirtualMachine)");
				}
			}
			catch (Exception ex)
			{
				// TO-DO: Possibly we should avoid exposing such detailed exceptions to the end-user
				vmForMove.exMessage = vmForMove.exMessage + "\n MoveVM \n" + ex.Message;
				//
				Log.WriteError(ex);
			}

			vmForMove.logMessage = vmForMove.logMessage + steps + paramsMove;
			return vmForMove;
		}

		public void ConfigureCreatedVMNetworkAdapters(VMInfo vmInfo)
		{
			//
			var vm = default(VirtualMachineInfo);
			// Retrieve current VM details
			using (var client = GetVMMSClient())
			{
				vm = client.GetVirtualMachineByName(vmInfo.Name);
			}
			// Validate VM status
			if (vm.Status != SVMMService.VMComputerSystemStateInfo.PowerOff && vm.Status != SVMMService.VMComputerSystemStateInfo.Stored)
			{
				throw new ApplicationException("Virtual machine should has status PowerOff to configure network adapters");
			}
			// Remove exists Network adapters
			DeleteNetworkAdapters(vm.Id);
			// Find out if external nic should be added
			//if (vmInfo.ExternalNetworkEnabled)
			//{
			//    using (var ps = PowerShell.Create())
			//    {
			//        ps.NewVirtualNetworkAdapter(vm.Id, vmInfo.ExternalVirtualNetwork, null);
			//    }
			//}
			//// Find out if private nic should be added
			//if (vmInfo.PrivateNetworkEnabled)
			//{
			//    using (var ps = PowerShell.Create())
			//    {
			//        ps.NewVirtualNetworkAdapter(vm.Id, vmInfo.PrivateVirtualNetwork, vmInfo.PrivateVLanID);
			//    }
			//}
		}

		public Virtualization.VirtualNetworkInfo[] GetVirtualNetworkByHostName(string hostName)
		{
			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{
				HostInfo host = client.GetHostByName(hostName);
				return GetVirtualNetworkByHostInfo(host);
			}

		}

		public Virtualization.VirtualNetworkInfo[] GetVirtualNetworkByHostInfo(HostInfo hostInfo)
		{
			List<Virtualization.VirtualNetworkInfo> result = new List<Virtualization.VirtualNetworkInfo>();

			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{
				VirtualizationForPC.SVMMService.VirtualNetworkInfo[] networks = client.GetVirtualNetworkByHost(hostInfo);
				foreach (var item in networks)
				{
					result.Add(
						new Virtualization.VirtualNetworkInfo
						{
							BoundToVMHost = item.BoundToVMHost,
							DefaultGatewayAddress = item.DefaultGatewayAddress,
							Description = item.Description,
							DNSServers = item.DNSServers,
							EnablingIPAddress = item.EnablingIPAddress,
							HighlyAvailable = item.HighlyAvailable,
							HostBoundVlanId = item.HostBoundVlanId,
							Id = item.Id,
							Name = item.Name,
							NetworkAddress = item.NetworkAddress,
							NetworkMask = item.NetworkMask,
							Tag = item.Tag,
							VMHost = item.VMHost.ComputerName,
							VMHostId = item.VMHostId,
							WINServers = item.WINServers
						});
				}
			}

			return result.ToArray();

		}

		public JobResult ChangeVirtualMachineState(string vmId, VirtualMachineRequestedState newState)
		{
			// target computer
			JobResult ret = new JobResult();
			ret.Job = new ConcreteJob();
			ret.Job.Id = vmId;

			try
			{
				using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
				{

					VirtualMachineInfo vm = client.GetVirtualMachineByName(vmId);

					switch (newState)
					{
						case VirtualMachineRequestedState.Start:
							{
								client.StartVirtualMachine(vm.Id);
								ret.Job.TargetState = Virtualization.VMComputerSystemStateInfo.Running;
								break;
							}
						case VirtualMachineRequestedState.Resume:
							{
								client.ResumeVirtualMachine(vm.Id);
								ret.Job.TargetState = Virtualization.VMComputerSystemStateInfo.Running;
								break;
							}
						case VirtualMachineRequestedState.Pause:
							{
								client.PauseVirtualMachine(vm.Id);
								ret.Job.TargetState = Virtualization.VMComputerSystemStateInfo.Paused;
								break;
							}
						case VirtualMachineRequestedState.ShutDown:
							{
								client.ShutdownVirtualMachine(vm.Id);
								ret.Job.TargetState = Virtualization.VMComputerSystemStateInfo.Stored;
								break;
							}
						case VirtualMachineRequestedState.TurnOff:
							{
								client.StopVirtualMachine(vm.Id);
								ret.Job.TargetState = Virtualization.VMComputerSystemStateInfo.PowerOff;
								break;
							}
						default:
							{
								break;
							}
					}

					ret.Job.JobState = ConcreteJobState.Running;
					ret.Job.Caption = newState.ToString();
					ret.ReturnValue = ReturnCode.JobStarted;
				}
			}
			catch (Exception ex)
			{
				Log.WriteError("Could not change virtual machine state", ex);
				//
				ret.Job.JobState = ConcreteJobState.Exception;
				ret.Job.Caption = newState.ToString();
				ret.ReturnValue = ReturnCode.Failed;
			}

			return ret;
		}

		public ReturnCode ShutDownVirtualMachine(string vmId, bool force, string reason)
		{
			ReturnCode ret = ReturnCode.JobStarted;
			try
			{
				using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
				{

					VirtualMachineInfo vm = client.GetVirtualMachineByName(vmId);

					client.ShutdownVirtualMachine(vm.Id);

					ret = ReturnCode.OK;
				}
			}
			catch (Exception ex)
			{
				Log.WriteError("Could not shut down virtual machine", ex);
				//
				ret = ReturnCode.Failed;
			}

			return ret;
		}

		public JobResult DeleteVirtualMachine(string vmId)
		{
			// check state

			VMInfo vm = GetVirtualMachine(vmId);

			JobResult ret = new JobResult();
			ret.Job = new ConcreteJob();
			ret.Job.Id = vmId;
			ret.Job.JobState = ConcreteJobState.Completed;
			ret.ReturnValue = ReturnCode.OK;

			if (vm.VmGuid == Guid.Empty)
			{
				return ret;
			}

			// The virtual computer system must be in the powered off or saved state prior to calling this method.
			if (vm.State == FuseCP.Providers.Virtualization.VMComputerSystemStateInfo.Saved
				|| vm.State == FuseCP.Providers.Virtualization.VMComputerSystemStateInfo.PowerOff
				|| vm.State == FuseCP.Providers.Virtualization.VMComputerSystemStateInfo.CreationFailed
				|| vm.State == FuseCP.Providers.Virtualization.VMComputerSystemStateInfo.Stored
				|| vm.State == FuseCP.Providers.Virtualization.VMComputerSystemStateInfo.IncompleteVMConfig)
			{
				// delete network adapters and ports
				try
				{
					if (vm.State == FuseCP.Providers.Virtualization.VMComputerSystemStateInfo.PowerOff)
					{
						DeleteNetworkAdapters(vm.VmGuid);
					}

					using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
					{
						client.DeleteVirtualMachine(vm.VmGuid);

						ret.Job.Caption = "Delete VM Done";
						ret.Job.JobState = ConcreteJobState.Running;
						ret.Job.TargetState = Virtualization.VMComputerSystemStateInfo.Deleting;
						ret.ReturnValue = ReturnCode.JobStarted;
					}
				}
				catch (Exception ex)
				{
					ret.Job.Caption = ex.Message;
					ret.Job.Description = ex.StackTrace;
					ret.Job.JobState = ConcreteJobState.Exception;
					ret.ReturnValue = ReturnCode.Failed;
				}

				return ret;
			}
			else
			{
				throw new Exception("The virtual computer system must be in the powered off or saved state prior to calling Destroy method.");
			}
		}

		private void DeleteNetworkAdapters(Guid objVM)
		{
			var adapters = default(VirtualNetworkAdapterInfo[]);
			// Retrieve VM network adapters
			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{
				adapters = client.GetVirtualNetworkAdaptersByVM(objVM);
			}
			//
			if (adapters != null)
			{
				foreach (VirtualNetworkAdapterInfo item in adapters)
				{
					DeleteNetworkAdapter(item, false);
				}
			}
		}

		private void DeleteNetworkAdapter(VirtualNetworkAdapterInfo objVM, bool runAsunc)
		{
			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{
				client.RemoveVirtualNetworkAdapter(objVM, runAsunc, null);
			}
		}
		#endregion

		#region Snapshots
		public List<VirtualMachineSnapshot> GetVirtualMachineSnapshots(string vmId)
		{
			// get all VM setting objects

			List<VirtualMachineSnapshot> ret = new List<VirtualMachineSnapshot>();

			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{
				VMCheckpointInfo[] chkPtnList = client.GetVirtualMachineByName(vmId).VMCheckpoints;

				if (chkPtnList != null)
				{
					foreach (VMCheckpointInfo curr in chkPtnList)
					{
						ret.Add(new VirtualMachineSnapshot()
						{
							Created = curr.AddedTime
						,
							Id = curr.Id.ToString()
						,
							Name = curr.Name
						,
							CheckPointId = curr.CheckpointID
						,
							ParentId = curr.ParentCheckpointID
						});
					}
				}
			}

			return ret;
		}

		public JobResult CreateSnapshot(string vmId)
		{
			JobResult ret = new JobResult();

			try
			{
				using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
				{
					ret.Job = new ConcreteJob();
					ret.Job.Id = vmId;
					ret.Job.JobState = ConcreteJobState.Starting;
					ret.ReturnValue = ReturnCode.JobStarted;

					VirtualMachineInfo vm = client.GetVirtualMachineByName(vmId);

					client.NewVirtualMachineCheckpoint(vm.Id, String.Format("{0} - {1}", vm.Name, DateTime.Now), String.Empty);
				}
			}
			catch (TimeoutException)
			{
				ret.ReturnValue = ReturnCode.JobStarted;
			}
			catch (Exception ex)
			{
				ret.Job.ErrorDescription = ex.Message;
				ret.Job.JobState = ConcreteJobState.Exception;
				ret.ReturnValue = ReturnCode.Failed;
			}

			return ret;
		}

		public JobResult ApplySnapshot(string vmId, string snapshotId)
		{
			JobResult ret = new JobResult();
			bool error = false;
			try
			{
				using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
				{
					ret.Job = new ConcreteJob();
					ret.Job.Id = vmId;
					ret.Job.JobState = ConcreteJobState.Starting;
					ret.ReturnValue = ReturnCode.JobStarted;

					client.RestoreVirtualMachineCheckpoint(snapshotId);
				}
			}
			catch (TimeoutException)
			{
				error = true;
				ret.ReturnValue = ReturnCode.JobStarted;
			}
			catch (Exception ex)
			{
				error = true;
				ret.Job.ErrorDescription = ex.Message;
				ret.Job.JobState = ConcreteJobState.Exception;
				ret.ReturnValue = ReturnCode.Failed;
			}

			if (!error)
			{
				ret.ReturnValue = ReturnCode.OK;
			}

			return ret;
		}

		public JobResult DeleteSnapshot(string vmId, string snapshotId)
		{
			JobResult ret = new JobResult();

			try
			{
				using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
				{
					ret.Job = new ConcreteJob();
					ret.Job.Id = vmId;
					ret.Job.JobState = ConcreteJobState.Starting;
					ret.ReturnValue = ReturnCode.JobStarted;

					VirtualMachineInfo vm = client.GetVirtualMachineByName(vmId);

					client.DeleteVirtualMachineCheckpoint(snapshotId);
				}
			}
			catch (Exception ex)
			{
				ret.Job.ErrorDescription = ex.Message;
				ret.Job.JobState = ConcreteJobState.Exception;
				ret.ReturnValue = ReturnCode.Failed;
			}

			return ret;
		}

		public byte[] GetSnapshotThumbnailImage(string snapshotId, ThumbnailSize size)
		{
			//            ManagementBaseObject objSummary = GetSnapshotSummaryInformation(snapshotId, (SummaryInformationRequest)size);

			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{

				VirtualMachineInfo vminfo = client.GetVirtualMachineByName(snapshotId);
				HostInfo host = client.GetHostById(vminfo.HostId);

				return GetTumbnailFromSummaryInformation(vminfo.Name, size);
			}
		}
		#endregion

		#region Library
		public LibraryItem[] GetLibraryItems(string path)
		{
			return default(LibraryItem[]);
		}

		public LibraryItem[] GetOSLibraryItems()
		{
			List<LibraryItem> items = new List<LibraryItem>();

			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{
				TemplateInfo[] ti = client.GetTemplates();

				for (int i = 0; i < ti.Length; i++)
				{
					LibraryItem newItem = new LibraryItem();

					newItem.Description = ti[i].OperatingSystem.Name;
					newItem.Name = ti[i].Name;
					newItem.Path = ti[i].Id.ToString();
					newItem.ProcessVolume = ti[i].CPUCount;
					newItem.ProvisionAdministratorPassword = ti[i].AdminPasswordhasValue;
					newItem.ProvisionComputerName = true;
					newItem.ProvisionNetworkAdapters = (ti[i].VirtualNetworkAdapters.Length > 0);
					newItem.LegacyNetworkAdapter = (ti[i].NetworkUtilization > 0);

					items.Add(newItem);
				}
			}
			return items.ToArray();
		}

		public LibraryItem[] GetClusters()
		{
			List<LibraryItem> items = new List<LibraryItem>();

			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{

				if (client.State != CommunicationState.Opened)
				{
					client.Open();
				}

				HostClusterInfo[] ci = client.GetHostClusters();

				if (ci == null || ci.Length == 0)
				{
					throw new Exception("Clusters is not found.");
				}

				for (int i = 0; i < ci.Length; i++)
				{
					LibraryItem newItem = new LibraryItem();

					HostClusterInfo hostInfo = ci[i];

					newItem.Description = hostInfo.Description;
					newItem.Name = hostInfo.Name;
					newItem.Path = hostInfo.Id.ToString();

					//TODO нужно думать
					newItem.ProcessVolume = hostInfo.AvailableStorageNode.CoresPerCPU;
					newItem.ProvisionComputerName = true;

					// Get host's networks
					newItem.Networks = GetVirtualNetworkByHostInfo(hostInfo.AvailableStorageNode);

					items.Add(newItem);
				}

				client.Close();
			}

			return items.ToArray();
		}

		public LibraryItem[] GetHosts()
		{
			List<LibraryItem> items = new List<LibraryItem>();

			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{
				if (client.State != CommunicationState.Opened)
				{
					client.Open();
				}

				HostInfo[] ti = null;

				try
				{
					ti = client.GetHosts();
				}
				catch (Exception ex)
				{
					throw new Exception("GetHost Failed", ex);
				}

				if (ti == null || ti.Length == 0)
				{
					throw new Exception("Hosts is not found.");
				}

				for (int i = 0; i < ti.Length; i++)
				{
					LibraryItem newItem = new LibraryItem();

					HostInfo hostInfo = ti[i];

					newItem.Description = hostInfo.Description;
					newItem.Name = hostInfo.ComputerName;
					newItem.Path = hostInfo.Id.ToString();
					newItem.ProcessVolume = hostInfo.CoresPerCPU;
					newItem.ProvisionComputerName = true;

					// Get host's networks
					newItem.Networks = GetVirtualNetworkByHostInfo(hostInfo);

					items.Add(newItem);
				}
			}
			return items.ToArray();
		}
		#endregion

		#region Configuration
		public int GetProcessorCoresNumber(string templateId)
		{
			int ret = 0;

			using (FCPVirtualMachineManagementServiceClient client = GetVMMSClient())
			{
				TemplateInfo selTemplate = client.GetTemplateById(new Guid(templateId));
				ret = selTemplate.CPUMax;
			}
			return ret;
		}
		#endregion

		#region IHostingServiceProvier methods
		public override string[] Install()
		{
			List<string> messages = new List<string>();

			// TODO

			return messages.ToArray();
		}

		public override bool IsInstalled()
		{
			// check if Hyper-V role is installed and available for management
			//Wmi root = new Wmi(ServerNameSettings, "root");
			//ManagementObject objNamespace = root.GetWmiObject("__NAMESPACE", "name = 'virtualization'");
			//return (objNamespace != null);
			return true;
		}
		#endregion

		#region Hyper-V Cloud

		public bool CheckServerState(VMForPCSettingsName control, string connString, string connName)
		{
			bool ret = false;

			try
			{
				switch (control)
				{
					case VMForPCSettingsName.SCVMMServer:
						{
							if (!IsNullOrWhiteSpaceString(connString)
								&& !IsNullOrWhiteSpaceString(connName))
							{
								EndpointAddress endPointAddress = GetEndPointAddress(connString, connName);

								using (VirtualMachineManagementServiceClient check = new VirtualMachineManagementServiceClient(new WSHttpBinding(SecurityMode.Message)
								{
									Security = { Message = { ClientCredentialType = MessageCredentialType.Windows } }
								}, endPointAddress))
								{
									check.Open();
									ret = true;
									check.Close();
								}
							}
							break;
						}
					case VMForPCSettingsName.SCOMServer:
						{
							if (!IsNullOrWhiteSpaceString(connString)
								&& !IsNullOrWhiteSpaceString(connName))
							{
								EndpointAddress endPointAddress = GetEndPointAddress(connString, connName);

								using (MonitoringServiceClient checkMonitoring = new MonitoringServiceClient(new WSHttpBinding(SecurityMode.Message)
								{
									Security = { Message = { ClientCredentialType = MessageCredentialType.Windows } }
								}, endPointAddress))
								{
									checkMonitoring.Open();
									ret = true;
									checkMonitoring.Close();
								}
							}
							break;
						}
				}
			}
			catch (Exception ex)
			{
				//
				Log.WriteError("Could not check server state", ex);
				//
				ret = false;
				//
				throw;
			}
			return ret;
		}

		#endregion Hyper-V Cloud

		#region Monitoring
		/// <summary>
		/// Get device Events
		/// </summary>
		/// <param name="serviceName">serviceName</param>
		/// <param name="displayName">displayName</param>
		/// <returns></returns>
		public List<MonitoredObjectEvent> GetDeviceEvents(string serviceName, string displayName)
		{
			List<MonitoredObjectEvent> monitoredObjectEventCollection = new List<MonitoredObjectEvent>();
			using (FCPVirtualMachineManagementServiceClient context = GetVMMSClient())
			{
				VirtualMachineInfo vmi = context.GetVirtualMachineByName(displayName);
				if (vmi != null)
				{
					using (FCPMonitoringServiceClient client = GetMonitoringServiceClient())
					{
						MonitoredObject monitoringObject = client.GetMonitoredObjectByDisplayName(vmi.HostName, vmi.ComputerName);
						foreach (var item in monitoringObject.Events)
						{
							monitoredObjectEventCollection.Add(
								new MonitoredObjectEvent
								{
									Category = item.Category,
									Decription = item.Decription,
									EventData = item.EventData,
									Level = item.Level,
									Number = item.Number,
									TimeGenerated = item.TimeGenerated
								});
						}
					}
				}
			}

			return monitoredObjectEventCollection;
		}

		public List<MonitoredObjectAlert> GetMonitoringAlerts(string serviceName, string virtualMachineName)
		{
			List<MonitoredObjectAlert> result = new List<MonitoredObjectAlert>();
			using (FCPVirtualMachineManagementServiceClient context = GetVMMSClient())
			{
				VirtualMachineInfo vmi = context.GetVirtualMachineByName(virtualMachineName);
				if (vmi != null)
				{
					using (FCPMonitoringServiceClient client = GetMonitoringServiceClient())
					{
						MonitoredObject mo = client.GetMonitoredObjectByDisplayName(vmi.HostName, vmi.ComputerName);

						//                        Alert[] alerts = client.GetMonitoringAlertsByObjectDisplayName(serviceName, GetComputerNameByVMName(virtualMachineName));
						Alert[] alerts = mo.Alerts;
						foreach (var item in alerts)
						{
							result.Add(
								new MonitoredObjectAlert
								{
									Created = item.Created,
									Description = item.Description,
									Name = item.Name,
									ResolutionState = item.ResolutionState,
									Severity = item.Severity,
									Source = item.Source
								});
						}
					}
				}
			}
			return result;
		}

		public List<Virtualization.PerformanceDataValue> GetPerfomanceValue(string VmName, PerformanceType perf, DateTime startPeriod, DateTime endPeriod)
		{
			List<Virtualization.PerformanceDataValue> ret = new List<Virtualization.PerformanceDataValue>();

			/* This test code */
			//Random random = new Random((int)DateTime.Now.Ticks);

			//TimeSpan count = (endPeriod - startPeriod);

			//for (int pointIndex = 0; pointIndex < 20; pointIndex++)
			//{
			//    ret.Add(new Virtualization.PerformanceDataValue() { SampleValue = random.Next(1, 99), TimeSampled = DateTime.Now });
			//}

			//return ret;


			using (FCPMonitoringServiceClient client = GetMonitoringServiceClient())
			{
				client.Open();

				PerformanceData[] pdOneVM = null;

				switch (perf)
				{
					case PerformanceType.Processor:
						pdOneVM = client.GetSingleVMHyperVCPUCounters(MonitoringServerNameSettings, VmName);
						break;
					case PerformanceType.Network:
						pdOneVM = client.GetSingleVMHyperVVirtualNetwork(MonitoringServerNameSettings, VmName);
						break;
					case PerformanceType.Memory:
						pdOneVM = client.GetSingleVMHyperVGuestMemoryPagesAllocated(MonitoringServerNameSettings, VmName);
						break;
					//case PerformanceType.DiskIO:
					//    break;

				}

				if ((pdOneVM != null) && (pdOneVM.Length > 0))
				{
					FuseCP.Providers.VirtualizationForPC.MonitoringWebService.PerformanceDataValue[] retData =
						client.GetMonitoringPerformanceValues(MonitoringServerNameSettings, pdOneVM[0], startPeriod, endPeriod);

					int index = 1;

					if (retData.Length > 100)
					{
						index = (int)Math.Ceiling(((double)retData.Length) / 100);
					}

					for (int i = 0; i < retData.Length; i = i + index)
					{
						FuseCP.Providers.VirtualizationForPC.MonitoringWebService.PerformanceDataValue curr = retData[i];

						ret.Add(new Virtualization.PerformanceDataValue()
						{
							SampleValue = curr.SampleValue
							,
							TimeAdded = curr.TimeAdded
							,
							TimeSampled = curr.TimeSampled
							,
							ExtensionData = curr.ExtensionData
						});
					}
				}

				client.Close();
			}

			return ret;

			/* This test code */
			//Random random = new Random((int)DateTime.Now.Ticks);

			//TimeSpan count = (endPeriod - startPeriod);

			//for (int pointIndex = 0; pointIndex < 20; pointIndex++)
			//{
			//    ret.Add(new Virtualization.PerformanceDataValue() { SampleValue = random.Next(1, 99) });
			//}

			//return ret;
		}

		private static Dictionary<string, string> computerNameByVMName = new Dictionary<string, string>();

		private string GetComputerNameByVMName(string virtualMachineName)
		{
			string result;
			if (!computerNameByVMName.TryGetValue(virtualMachineName, out result))
			{
				using (FCPVirtualMachineManagementServiceClient context = GetVMMSClient())
				{
					VirtualMachineInfo vmInfo = context.GetVirtualMachineByName(virtualMachineName);
					computerNameByVMName[virtualMachineName] = result = (vmInfo != null) ? vmInfo.ComputerName : string.Empty;
				}
			}
			return result;
		}
		private EndpointAddress GetEndPointAddress(string connString, string connName)
		{
			bool UseSPN = true;

			if (!Boolean.TryParse(ConfigurationManager.AppSettings["UseSPN"], out UseSPN))
			{
				UseSPN = false;
			}

			EndpointAddress endPointAddress = null;

			if (UseSPN)
			{
				endPointAddress = new EndpointAddress(new Uri(connString)
								 , new SpnEndpointIdentity(connName));
			}
			else
			{
				endPointAddress = new EndpointAddress(new Uri(connString)
								 , new UpnEndpointIdentity(connName));

			}

			return endPointAddress;
		}

		#endregion

		#region Procxy

		public FCPVirtualMachineManagementServiceClient GetVMMSClient()
		{
			FCPVirtualMachineManagementServiceClient ret;

			if (!IsNullOrWhiteSpaceString(SCVMMServer)
				&& !IsNullOrWhiteSpaceString(SCVMMPrincipalName))
			{
				EndpointAddress endPointAddress = GetEndPointAddress(SCVMMServer, SCVMMPrincipalName);

				ret = new FCPVirtualMachineManagementServiceClient(new WSHttpBinding(SecurityMode.Message)
				{
					Security = { Message = { ClientCredentialType = MessageCredentialType.Windows } }
				}, endPointAddress);

				VersionInfo ver = new VersionInfo();
			}
			else
			{
				throw new Exception("SCVMMServer or SCVMMPrincipalName is empty");
			}

			return ret;
		}

		public FCPMonitoringServiceClient GetMonitoringServiceClient()
		{
			FCPMonitoringServiceClient ret;

			if (!IsNullOrWhiteSpaceString(SCOMServer)
				&& !IsNullOrWhiteSpaceString(SCOMPrincipalName))
			{
				EndpointAddress endPointAddress = GetEndPointAddress(SCOMServer, SCOMPrincipalName);

				ret = new FCPMonitoringServiceClient(new WSHttpBinding(SecurityMode.Message)
				{
					Security = { Message = { ClientCredentialType = MessageCredentialType.Windows } }
				}, endPointAddress);
			}
			else
			{
				throw new Exception("MonitoringServer or MonitoringPrincipalName is empty");
			}

			return ret;
		}

		#endregion Proxy

		public JobResult AddKVPItems(string vmId, KvpExchangeDataItem[] items)
		{
			throw new NotImplementedException();
		}

		public ChangeJobStateReturnCode ChangeJobState(string jobId, ConcreteJobRequestedState newState)
		{
			throw new NotImplementedException();
		}

		public JobResult ConvertVirtualHardDisk(string sourcePath, string destinationPath, VirtualHardDiskType diskType)
		{
			throw new NotImplementedException();
		}

		public VirtualSwitch CreateSwitch(string name)
		{
			throw new NotImplementedException();
		}

		public void DeleteRemoteFile(string path)
		{
			throw new NotImplementedException();
		}

		public JobResult DeleteSnapshotSubtree(string snapshotId)
		{
			throw new NotImplementedException();
		}

		public ReturnCode DeleteSwitch(string switchId)
		{
			throw new NotImplementedException();
		}

		public JobResult EjectDVD(string vmId)
		{
			throw new NotImplementedException();
		}

		public JobResult ExpandVirtualHardDisk(string vhdPath, ulong sizeGB)
		{
			throw new NotImplementedException();
		}

		public List<ConcreteJob> GetAllJobs()
		{
			throw new NotImplementedException();
		}

		public List<VirtualSwitch> GetExternalSwitches(string computerName)
		{
			throw new NotImplementedException();
		}

		public string GetInsertedDVD(string vmId)
		{
			throw new NotImplementedException();
		}

		public ConcreteJob GetJob(string jobId)
		{
			throw new NotImplementedException();
		}

		public List<KvpExchangeDataItem> GetKVPItems(string vmId)
		{
			throw new NotImplementedException();
		}

		public VirtualMachineSnapshot GetSnapshot(string snapshotId)
		{
			throw new NotImplementedException();
		}

		public List<KvpExchangeDataItem> GetStandardKVPItems(string vmId)
		{
			throw new NotImplementedException();
		}

		public List<VirtualSwitch> GetSwitches()
		{
			throw new NotImplementedException();
		}

		public Virtualization.VirtualHardDiskInfo GetVirtualHardDiskInfo(string vhdPath)
		{
			throw new NotImplementedException();
		}

		public VirtualMachine GetVirtualMachineEx(string vmId)
		{
			throw new NotImplementedException();
		}

		public List<ConcreteJob> GetVirtualMachineJobs(string vmId)
		{
			throw new NotImplementedException();
		}

		public List<VirtualMachine> GetVirtualMachines()
		{
			throw new NotImplementedException();
		}

		public JobResult InsertDVD(string vmId, string isoPath)
		{
			throw new NotImplementedException();
		}

		public JobResult ModifyKVPItems(string vmId, KvpExchangeDataItem[] items)
		{
			throw new NotImplementedException();
		}

		public MountedDiskInfo MountVirtualHardDisk(string vhdPath)
		{
			throw new NotImplementedException();
		}

		public string ReadRemoteFile(string path)
		{
			throw new NotImplementedException();
		}

		public JobResult RemoveKVPItems(string vmId, string[] itemNames)
		{
			throw new NotImplementedException();
		}

		public JobResult RenameSnapshot(string vmId, string snapshotId, string name)
		{
			throw new NotImplementedException();
		}

		public JobResult RenameVirtualMachine(string vmId, string name)
		{
			throw new NotImplementedException();
		}

		public bool SwitchExists(string switchId)
		{
			throw new NotImplementedException();
		}

		public ReturnCode UnmountVirtualHardDisk(string vhdPath)
		{
			throw new NotImplementedException();
		}

		public VMInfo UpdateVirtualMachine(VMInfo vm)
		{
			throw new NotImplementedException();
		}

		public void WriteRemoteFile(string path, string content)
		{
			throw new NotImplementedException();
		}

		public void ExpandDiskVolume(string diskAddress, string volumeName)
		{
			throw new NotImplementedException();
		}

        private bool IsNullOrWhiteSpaceString(string value)
        {
            return String.IsNullOrEmpty(value) || (value.Trim().Length == 0);
        }
	}
}
