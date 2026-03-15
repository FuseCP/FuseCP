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
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Reflection;
using System.Net.Http;
using FuseCP.Providers.OS;

namespace FuseCP.Tests
{
    public class Kestrel: IDisposable
    {
		Process process = null;

		public (Scheme Protocol, string Url)[] Urls;
		public string HttpUrl => Urls.FirstOrDefault(u => u.Protocol == Scheme.Http).Url;
		public string HttpsUrl => Urls.FirstOrDefault(u => u.Protocol == Scheme.Https).Url;
		public string NetTcpUrl => Urls.FirstOrDefault(u => u.Protocol == Scheme.NetTcp).Url;
		Component component;
		WSLShell.WSLDistro WslDistro;
		static List<Kestrel> instances = new List<Kestrel>();
		public Kestrel(Component component, (Scheme, string)[] urls = null, WSLShell.WSLDistro wslDistro = null)
		{
			Urls = urls;
			this.component = component;
			this.WslDistro = wslDistro;
			instances.Add(this);
			var apppath = Paths.Path(component);
			var testdllpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var testprojpath = Path.GetFullPath(Path.Combine(testdllpath, "..", "..", ".."));
			var workingDir = Path.Combine(apppath, "bin_dotnet");
			var log = Path.GetFullPath(Path.Combine(Paths.Test, "TestResults", $"Kestrel.log"));
			if (!Directory.Exists(Path.GetDirectoryName(log))) Directory.CreateDirectory(Path.GetDirectoryName(log));
			var dll = Path.Combine(workingDir, $"{Paths.App}.{component}.dll");
			var pfx = Certificate.CertFilePath;

			Shell shell;
			if (wslDistro == null) shell = Shell.Standard.Clone;
			else
			{
				shell = new WSLShell(wslDistro);

				testdllpath = Paths.Wsl(testdllpath);
				testprojpath = Paths.Wsl(testprojpath);
				dll = Paths.Wsl(dll);
				pfx = Paths.Wsl(pfx);
			}
			var distro = (wslDistro?.ToString() ?? "Windows");

			var exe = wslDistro != null ? "/usr/lib/dotnet/dotnet" : shell.Find("dotnet");
			if (wslDistro != null)
			{
				var dotnetExists = shell.Exec($"test -x \"{exe}\" && echo ok").Output().Result.Trim() == "ok";
				if (!dotnetExists)
				{
					var discoveredExe = shell.Find("dotnet");
					if (!string.IsNullOrWhiteSpace(discoveredExe)) exe = discoveredExe;
				}
			}
			if (string.IsNullOrWhiteSpace(exe))
			{
				throw new FileNotFoundException(wslDistro != null
					? "Could not find dotnet in WSL. Expected /usr/lib/dotnet/dotnet or a PATH entry for dotnet."
					: "Could not find dotnet in PATH.");
			}
			shell.LogFile = log;
			shell.LogCommand += msg =>
			{
				if (Debugger.IsAttached) Debug.Write($"{distro}>{msg}");
				Console.Write($"{distro}>{msg}");
			};
			shell.Log += msg =>
				{
					if (Debugger.IsAttached) Debug.Write($"{distro}/Kestrel>{msg}");
					Console.Write($"{distro}/Kestrel>{msg}");
				};
			shell.LogError += msg =>
			{
				if (Debugger.IsAttached) Debug.Write($"{distro}/Kestrel>{msg}");
				var mainColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write($"{distro}/Kestrel>{msg}");
				Console.ForegroundColor = mainColor;
			};
			shell.Environment = new Dictionary<string, string>()
			{
				{ "ASPNETCORE_ENVIRONMENT", "Development" },
				//{ "ASPNETCORE_URLS", $"{HttpUrl};{HttpsUrl}" },
				//{ "ASPNETCORE_Kestrel__Certificates__Default__Path", pfx },
				//{ "ASPNETCORE_Kestrel__Certificates__Default__Password", Certificate.Password },
				//{ "ServerCertificate__File", pfx },
				//{ "ServerCertificate__Password", Certificate.Password },
			};
			shell.WorkingDirectory = workingDir;

			shell.Exec("dotnet dev-certs https");
			/*var cert = shell.ExecAsync("dotnet dev-certs https --trust");
			cert.Input.WriteLine("yes");
			cert.Wait();*/

			process = shell.ExecAsync($"\"{exe}\" \"{dll}\" --urls \"{HttpUrl};{HttpsUrl}\"").Process;

			if (WslDistro == null && process.HasExited) throw new Exception($"Kestrel exited with code {process.ExitCode}");

			// Wait for at least one HTTP endpoint to respond before continuing.
			const int maxRetries = 30;
			var isReady = false;
			for (var n = 0; n < maxRetries; n++)
			{
				if (process.HasExited)
				{
					throw new Exception($"Kestrel process terminated before readiness check completed (exit code {process.ExitCode}).");
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
				throw new TimeoutException($"Kestrel endpoint readiness timed out for component '{component}'. Probed URLs: {HttpUrl}, {HttpsUrl}");
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
			instances.Remove(this);

			if (process != null && !process.HasExited) process.Kill();
			process = null;

			if (WslDistro != null)
			{
				if (!instances.Any(k => k.WslDistro == WslDistro))
				{
					WSLShell.Default.Terminate(WslDistro);
				} 
				if (!instances.Any(k => k.WslDistro != null)) {
					WSLShell.Default.ShutdownAll();
				}
			}
        }
	}
}
