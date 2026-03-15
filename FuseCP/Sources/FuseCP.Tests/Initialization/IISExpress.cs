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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using FuseCP.Providers.OS;
using System.Reflection;
using System.IO;

namespace FuseCP.Tests
{
    public class IISExpress: IDisposable
    {
		Process process = null;

		public (Scheme Protocol, string Url)[] Urls;
		public string HttpUrl => Urls.FirstOrDefault(u => u.Protocol == Scheme.Http).Url;
		public string HttpsUrl => Urls.FirstOrDefault(u => u.Protocol == Scheme.Https).Url;
		public string NetTcpUrl => Urls.FirstOrDefault(u => u.Protocol == Scheme.NetTcp).Url;
		public static IISExpress Current { get; private set; } = null;
		public int HttpPort => new Uri(HttpUrl).Port;
		public int HttpsPort => new Uri(HttpsUrl).Port;
		public int NetTcpPort => new Uri(NetTcpUrl).Port;
		public IISExpress(Component component, (Scheme, string)[] urls = null)
        {
			Urls = urls;
			var testprojpath = Paths.Test;
			var iisExprPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "IIS Express");
			var appcmd = Path.Combine(iisExprPath, "AppCmd.exe");
			var admincmd = Path.Combine(iisExprPath, "IisExpressAdminCmd.exe");
			var iisexpress = Path.Combine(iisExprPath, "iisexpress.exe");
			var serverPath = Paths.Path(component);
			var log = Path.GetFullPath(Path.Combine(Paths.Test, "TestResults", $"IISExpress.log"));
			if (!Directory.Exists(Path.GetDirectoryName(log))) Directory.CreateDirectory(Path.GetDirectoryName(log));

			// setup iis express
			var shell = Shell.Standard.Clone;
			shell.LogFile = log;
			shell.Log += msg =>
			{
				if (Debugger.IsAttached) Debug.Write($"IIS Express>{msg}");
				Console.Write($"IIS Express>{msg}");
			};
			shell.LogError += msg =>
			{
				if (Debugger.IsAttached) Debug.Write($"IIS Express>{msg}");
				var mainColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write($"IIS Express>{msg}");
				Console.ForegroundColor = mainColor;
			};
			shell.Exec($"\"{admincmd}\" setupSslUrl -url:{HttpsUrl} -UseSelfSigned");
			shell.Exec($"\"{appcmd}\" delete site {component}.Tests");
            shell.Exec($"\"{appcmd}\" add site /name:{component}.Tests /physicalPath:\"{serverPath}\" /bindings:http/*:{HttpPort}:localhost,https/*:{HttpsPort}:localhost");

			// start iis express
			shell.WorkingDirectory = serverPath;
			process = shell.ExecAsync($"\"{iisexpress}\" /site:{component}.Tests").Process;

			//if (process.HasExited) throw new Exception($"IIS Express exited with code {process.ExitCode}");

			// Wait for at least one HTTP endpoint to respond before continuing.
			const int maxRetries = 30;
			var isReady = false;
			for (var n = 0; n < maxRetries; n++)
			{
				if (process.HasExited)
				{
					throw new Exception($"IIS Express terminated before readiness check completed (exit code {process.ExitCode}).");
				}

				if (TryProbe(HttpUrl) || TryProbe(HttpsUrl))
				{
					isReady = true;
					break;
				}

				Thread.Sleep(2000);
			}

			if (!isReady)
			{
				throw new TimeoutException($"IIS Express endpoint readiness timed out for component '{component}'. Probed URLs: {HttpUrl}, {HttpsUrl}");
			}
        }

		static bool TryProbe(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return false;
			}

			try
			{
				var response = Servers.HttpClient.GetAsync(url).Result;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void Dispose()
		{
			if (process != null && !process.HasExited) process.Kill();
			process = null;
        }
	}
}
