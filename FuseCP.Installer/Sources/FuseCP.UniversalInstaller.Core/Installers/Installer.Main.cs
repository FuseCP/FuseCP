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
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Security.Permissions;
using System.Security;
using System.Text;
using System.Threading;
using FuseCP.Providers.OS;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.IO;

namespace FuseCP.UniversalInstaller
{
	public partial class Installer
	{
		public virtual Mutex Mutex { get; private set; }
		public virtual bool IsNewInstance()
		{
			//check only one instance
			bool createdNew = true;
			Mutex = new Mutex(true, "FuseCP Installer", out createdNew);
			return createdNew;
		}
		public virtual void SaveMutex()
		{
			GC.KeepAlive(Mutex);
		}

		public virtual bool CheckSecurityNetFX()
		{
			try
			{
				PermissionSet set = new PermissionSet(PermissionState.Unrestricted);
				set.Demand();
				return true;
			}
			catch
			{
				return false;
			}
		}
		public virtual bool CheckSecurity()
		{
			return !OSInfo.IsNetFX || CheckSecurityNetFX();
		}
		public virtual void CheckWebServer()
		{
			if (!OSInfo.IsWindows) return;

			IOperatingSystem operatingSystem = null;
			try
			{
				operatingSystem = OSInfo.Current;
			}
			catch (Exception ex)
			{
				Log.WriteInfo("Unable to resolve Windows OS provider: {0}", ex.Message);
				return;
			}

			if (operatingSystem?.WebServer == null)
			{
				Log.WriteInfo("Skipping IIS detection because the OS provider did not expose a web server.");
				return;
			}

			if (!operatingSystem.WebServer.IsInstalled())
				Log.WriteError("IIS not found.");
			else
			{
				var version = operatingSystem.WebServer.Version;
				Log.WriteInfo("IIS {0} detected", version);
			}
		}

		public void LogOSVersion()
		{
			Log.WriteInfo($"{(OSInfo.WindowsVersion != WindowsVersion.NonWindows ? OSInfo.WindowsVersion : OSInfo.OSFlavor)} detected.");

		}
		public virtual void ShowLogFile() => UI.ShowLogFile();

		public virtual void SetAppDomainUnhandledException()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
			{
				Log.WriteError($"Unhandled Exception:", args.ExceptionObject as Exception);
				UI.Current.ShowError(args.ExceptionObject as Exception);
			};
		}

		public virtual void StartMain()
		{
			SetAppDomainUnhandledException();

			ImportLegacySettings();
			LoadSettings();
			UI.Current.CheckForInstallerUpdate(true);
			//
			//Utils.FixConfigurationSectionDefinition();

			//check security permissions && administrator permissions
			if (!CheckSecurity() || !IsRunningAsAdmin)
			{
				//ShowSecurityError();
				RestartAsAdmin();
				return;
			}

			//check for running instance
			if (!IsNewInstance())
			{
				UI.ShowRunningInstance();
				return;
			}

			Log.WriteApplicationStart();
			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

			//check OS version
			LogOSVersion();

			//check web server version
			CheckWebServer();

			/*if (!CheckCommandLineArgument("/nocheck"))
			{
				//Check for new versions
				if (CheckForUpdate(mainForm))
				{
					return;
				}
			} */

			/* if (CheckCommandLineArgument("/uselocalsetupdll"))
			{

			} */

			// Load setup parameters from an XML file
			//LoadSetupXmlFile();
			//start application

			UI.Init();
			UI.RunMainUI();

			//
			SaveMutex();
		}

	}
}
