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
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Management;
using System.Timers;
using System.Text;
#if NET8_0_OR_GREATER
using System.Runtime.Versioning;
#endif


namespace FuseCP.VmConfig
{
#if NET8_0_OR_GREATER
	[SupportedOSPlatform("windows")]
#endif
	public partial class VmConfigService : ServiceBase
	{
		internal const string RegistryInputKey = "SOFTWARE\\Microsoft\\Virtual Machine\\External";
		internal const string RegistryOutputKey = "SOFTWARE\\Microsoft\\Virtual Machine\\Guest";

		internal const string TaskPrefix = "FCP-";
		internal const string CurrentTaskName = "FCP-CurrentTask";

        internal const string RAM_SUMMARY_KEY = "VM-RAM-Summary";
        internal const string HDD_SUMMARY_KEY = "VM-HDD-Summary";

		private Dictionary<string, string> provisioningModules;
		private Timer idleTimer;
		private Timer pollTimer;
        private Timer summaryTimer;
		private bool rebootRequired = false;
		private System.Threading.Thread mainThread;
		
		public VmConfigService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			//start main procedure in separate thread
			mainThread = new System.Threading.Thread(new System.Threading.ThreadStart(Start));
			mainThread.Start();
		}

		private void Start()
		{
			//log
			ServiceLog.WriteApplicationStart();

            // delay (for sync with KVP exchange service)
            DelayOnStart();
			
            //init
			InitializeProvisioningModules();
			InitializeTimers();

            // start timer
            StartSummaryTimer();

			//run tasks
			ProcessTasks();
		}

		protected override void OnStop()
		{
			if (this.mainThread.IsAlive)
			{
#if NETFRAMEWORK
				this.mainThread.Abort();
#else
				this.mainThread.Interrupt();
#endif
			}
			this.mainThread.Join();
			ServiceLog.WriteApplicationStop();
		}

        private void DelayOnStart()
        {
            int startupDelay = 0;
            if (Int32.TryParse(ConfigurationManager.AppSettings["Service.StartupDelay"], out startupDelay)
                && startupDelay > 0)
            {
                ServiceLog.WriteStart("Delay on service start-up");
                System.Threading.Thread.Sleep(startupDelay);
                ServiceLog.WriteEnd("Delay on service start-up");
            }
        }

		private void DeleteOldResults()
		{
            // get the list of input tasks
            string[] strTasks = RegistryUtils.GetRegistryKeyValueNames(RegistryInputKey);
            List<string> tasks = new List<string>();
            foreach (string strTask in strTasks)
            {
                if (!string.IsNullOrEmpty(strTask) && strTask.StartsWith(TaskPrefix) && strTask != CurrentTaskName)
                {
                    //save only FuseCP tasks
                    tasks.Add(strTask);
                }
            }

            // get the list of task results
            int deletedResults = 0;
			string[] strResults = RegistryUtils.GetRegistryKeyValueNames(RegistryOutputKey);
			foreach (string strResult in strResults)
			{
				if (!string.IsNullOrEmpty(strResult) && strResult.StartsWith(TaskPrefix) && strResult != CurrentTaskName)
				{
					// check if task result exists in the input tasks
                    if (!tasks.Contains(strResult))
                    {
                        DeleteRegistryKeyValue(RegistryOutputKey, strResult);
                        ServiceLog.WriteInfo(string.Format("Deleted activity result: {0}", strResult));
                        deletedResults++;
                    }
				}
			}

            if(deletedResults > 0)
                ServiceLog.WriteEnd(string.Format("{0} result(s) deleted", deletedResults));
		}


		private void InitializeProvisioningModules()
		{
			ServiceLog.WriteStart("Loading provisioning modules...");
			provisioningModules = new Dictionary<string, string>();
			
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			ModuleSettingsSection section = config.Sections["moduleSettings"] as ModuleSettingsSection;
			if ( section != null)
			{
				foreach (string key in section.Modules.AllKeys)
				{
					provisioningModules.Add(key, section.Modules[key].Value);
				}
			}
			else
				ServiceLog.WriteError("Modules configuration section not found");
			ServiceLog.WriteEnd(string.Format("{0} module(s) loaded", provisioningModules.Count));
		}

		private void InitializeTimers()
		{
            // idle timer
			idleTimer = new Timer();
			idleTimer.AutoReset = false;
			double idleInterval;
			if (!Double.TryParse(ConfigurationManager.AppSettings["Service.ExitIdleInterval"], out idleInterval))
			{
				ServiceLog.WriteError("Invalid configuration parameter: Service.ExitIdleInterval");
				idleInterval = 600000;
			}
			idleTimer.Interval = idleInterval; 
			idleTimer.Enabled = false;
			idleTimer.Elapsed += new ElapsedEventHandler(OnIdleTimerElapsed);

            // poll timer
			pollTimer = new Timer();
			pollTimer.AutoReset = false;
			double pollInterval;
			if (!Double.TryParse(ConfigurationManager.AppSettings["Service.RegistryPollInterval"], out pollInterval))
			{
				ServiceLog.WriteError("Invalid configuration parameter: Service.RegistryPollInterval");
				pollInterval = 60000;
			}
			pollTimer.Interval = pollInterval;
			pollTimer.Enabled = false;
			pollTimer.Elapsed += new ElapsedEventHandler(OnPollTimerElapsed);

            // system symmary timer
            summaryTimer = new Timer();
            double summaryInterval;
            if (!Double.TryParse(ConfigurationManager.AppSettings["Service.SystemSummaryInterval"], out summaryInterval))
            {
                ServiceLog.WriteError("Invalid configuration parameter: Service.SystemSummaryInterval");
                summaryInterval = 15000;
            }
            summaryTimer.Interval = summaryInterval;
            summaryTimer.Enabled = false;
            summaryTimer.Elapsed += new ElapsedEventHandler(OnSummaryTimerElapsed);
		}

		private void OnIdleTimerElapsed(object sender, ElapsedEventArgs e)
		{
			base.Stop();
		}

		private void OnPollTimerElapsed(object sender, ElapsedEventArgs e)
		{
			ProcessTasks();
		}

        private void OnSummaryTimerElapsed(object sender, ElapsedEventArgs e)
        {
            GetSystemSummary();
        }

        private void GetSystemSummary()
        {
            const UInt64 Size1KB = 0x400;
            const UInt64 Size1GB = 0x40000000;

            // check free/total RAM
            WmiUtils wmi = new WmiUtils("root\\cimv2");
            ManagementObjectCollection objOses = wmi.ExecuteQuery("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject objOs in objOses)
            {
                UInt64 freeRam = Convert.ToUInt64(objOs["FreePhysicalMemory"]) / Size1KB;
                UInt64 totalRam = Convert.ToUInt64(objOs["TotalVisibleMemorySize"]) / Size1KB;

                // write to the registry
                RegistryUtils.SetRegistryKeyStringValue(RegistryOutputKey, RAM_SUMMARY_KEY,
                    String.Format("{0}:{1}", freeRam, totalRam));

                objOs.Dispose();
            }

            // check local HDD logical drives
            ManagementObjectCollection objDisks = wmi.ExecuteQuery("SELECT * FROM Win32_LogicalDisk WHERE DriveType = 3");
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (ManagementObject objDisk in objDisks)
            {
                if(!first)
                    sb.Append(";");

                sb.Append(objDisk["DeviceID"]);
                sb.Append(Convert.ToInt32(Convert.ToDouble(objDisk["FreeSpace"]) / (double)Size1GB)).Append(":");
                sb.Append(Convert.ToInt32(Convert.ToDouble(objDisk["Size"]) / (double)Size1GB));

                first = false;

                objDisk.Dispose();
            }

            // write HDD info
            RegistryUtils.SetRegistryKeyStringValue(RegistryOutputKey, HDD_SUMMARY_KEY, sb.ToString());

            // dispose resources
            objOses.Dispose();
            objDisks.Dispose();
        }

		private void ProcessTasks()
		{
            // delete old results
            DeleteOldResults();

			//process all tasks
			while (true)
			{
                //load completed tasks results
                string[] strResults = RegistryUtils.GetRegistryKeyValueNames(RegistryOutputKey);
                List<string> results = new List<string>();
                foreach (string strResult in strResults)
                {
                    if (!string.IsNullOrEmpty(strResult) && strResult.StartsWith(TaskPrefix) && strResult != CurrentTaskName)
                    {
						//save only FuseCP tasks
                        results.Add(strResult);
                    }
                }

                // sorted list of tasks - will be sorted by the TaskID (time in ticks)
                SortedList<long, string> tasks = new SortedList<long, string>();

				//load task definitions from input registry key 
				string[] strTasks = RegistryUtils.GetRegistryKeyValueNames(RegistryInputKey);
				foreach (string strTask in strTasks)
				{
                    if (results.Contains(strTask))
                        continue; // skip completed tasks

					if (!string.IsNullOrEmpty(strTask) && strTask.StartsWith(TaskPrefix))
					{
                        // extract Task ID parameter
                        int idx = strTask.LastIndexOf('-');
                        if(idx == -1)
                            continue;

                        string strTaskId = strTask.Substring(idx + 1);
                        long taskId = 0;
                        try
                        {
                            taskId = Int64.Parse(strTaskId);
                        }
                        catch
                        {
                            continue; // wrong task format
                        }

						//save only FuseCP tasks
                        if(!tasks.ContainsKey(taskId))
						    tasks.Add(taskId, strTask);
					}
				}
				if (tasks.Count == 0)
				{
					if (rebootRequired)
					{
						ServiceLog.WriteInfo("Reboot required");
						RebootSystem();
						return;
					}

					//start timers
					StartPollTimer(); //starts task processing after poll interval 
					StartIdleTimer(); //stops service if idle 
					//no tasks - exit! 
					return;
				}
				else
				{
					//stop idle timer as we need to process tasks
					StopIdleTimer();
				}

				ExecutionContext context = null;
				foreach (long tid in tasks.Keys)
				{
					//find first correct task 
					string taskDefinition = tasks[tid];
					string taskParameters = RegistryUtils.GetRegistryKeyStringValue(RegistryInputKey, taskDefinition);
					if (taskDefinition.LastIndexOf("-") == -1 || taskDefinition.LastIndexOf('-') == taskDefinition.Length - 1)
					{
						ServiceLog.WriteError(string.Format("Task was deleted from queue as its definition is invalid : {0}", taskDefinition));
						DeleteRegistryKeyValue(RegistryInputKey, taskDefinition);
						//go to next task
						continue;

					}
					string taskName = taskDefinition.Substring(0, taskDefinition.LastIndexOf("-")).Substring(TaskPrefix.Length);
					string taskId = taskDefinition.Substring(taskDefinition.LastIndexOf('-') + 1);

					if (!provisioningModules.ContainsKey(taskName))
					{
						ServiceLog.WriteError(string.Format("Task was deleted from queue as its definition was not found : {0}", taskName));
						DeleteRegistryKeyValue(RegistryInputKey, taskDefinition);
						//go to next task
						continue;
					}
					//prepare execution context for correct task
					context = new ExecutionContext();
					context.ActivityID = taskId;
					context.ActivityName = taskName;
					ParseParameters(context.Parameters, taskParameters);
					context.ActivityDefinition = taskDefinition;
					break; 
				}
				if (context != null)
				{
					string type = provisioningModules[context.ActivityName];
					ExecutionResult res = null;
					DateTime start = DateTime.Now;
					
					try
					{
						//load module and run task
						ServiceLog.WriteStart(string.Format("Starting '{0}' module...", context.ActivityName));
						context.Progress = 0;
						res = ModuleLoader.Run(type, ref context);
						context.Progress = 100;
						ServiceLog.WriteEnd(string.Format("'{0}' module finished.", context.ActivityName));
					}
					catch (Exception ex)
					{
						ServiceLog.WriteError("Unhandled exception:", ex);
						res = new ExecutionResult();
						res.ResultCode = -1;
						res.ErrorMessage = string.Format("Unhandled exception : {0}", ex);
					}
					DateTime end = DateTime.Now;
					SaveExecutionResult(context.ActivityDefinition, res, start, end);
					//DeleteRegistryKeyValue(RegistryInputKey, context.ActivityDefinition);
					
					if (res.RebootRequired)
						rebootRequired = true;
				}
			}
		}

		private void ParseParameters(Dictionary<string, string> dictionary, string parameters)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			if (string.IsNullOrEmpty(parameters))
				return;

			string[] pairs = parameters.Split('|');
			foreach (string pair in pairs)
			{
				string[] parts = pair.Split(new char[] { '=' }, 2);
				if (parts.Length != 2)
					continue;
				
				if (!dictionary.ContainsKey(parts[0]))
					dictionary.Add(parts[0], parts[1]);
			}
		}

		private void DeleteRegistryKeyValue(string key, string valueName)
		{
			try
			{
				RegistryUtils.DeleteRegistryKeyValue(key, valueName);
			}
			catch (Exception ex)
			{
				ServiceLog.WriteError("Registry error:", ex);
			}
		}

		private void SaveExecutionResult(string name, ExecutionResult res, DateTime started, DateTime ended)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("ResultCode={0}|", res.ResultCode);
			builder.AppendFormat("RebootRequired={0}|", res.RebootRequired);
			builder.AppendFormat("ErrorMessage={0}|", res.ErrorMessage);
			builder.AppendFormat("Value={0}|", res.Value);
			builder.AppendFormat("Started={0}|", started.ToString("yyyyMMddHHmmss"));
			builder.AppendFormat("Ended={0}", started.ToString("yyyyMMddHHmmss"));
			RegistryUtils.SetRegistryKeyStringValue(RegistryOutputKey, name, builder.ToString());
		}

		private void StopIdleTimer()
		{
			idleTimer.Stop();
		}

		private void StartIdleTimer()
		{
			if ( idleTimer.Interval > 1 )
				idleTimer.Start();
		}

		private void StartPollTimer()
		{
			if ( pollTimer.Interval > 1 )
				pollTimer.Start();
		}

        private void StartSummaryTimer()
        {
            if (summaryTimer.Interval > 1)
                summaryTimer.Start();
        }

		private void RebootSystem()
		{
			try
			{
				ServiceLog.WriteStart("RebootSystem");
				WmiUtils wmi = new WmiUtils("root\\cimv2");
				ManagementObjectCollection objOses = wmi.ExecuteQuery("SELECT * FROM Win32_OperatingSystem");
				foreach (ManagementObject objOs in objOses)
				{
					objOs.Scope.Options.EnablePrivileges = true;
					objOs.InvokeMethod("Reboot", null);
				}
				ServiceLog.WriteEnd("RebootSystem");
			}
			catch (Exception ex)
			{
				ServiceLog.WriteError("Reboot System error:", ex);
			}
		}
	}
}
