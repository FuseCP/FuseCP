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
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net.Http;
using System.Net.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using FuseCP.Providers.OS;
using FuseCP.Web.Clients;
using FuseCP.Web.Services;

namespace FuseCP.Tests;

public enum Component
{
	Server = 0,
	EnterpriseServer = 1,
	Portal = 2,
	WebDavPortal = 3,
	Installer = 4,
}

public enum Framework
{
	NetFramework = 0,
	Core = 1,
}

public enum Scheme
{
	Http = 0,
	Https = 1,
	NetTcp = 2,
	Assembly = 3,
}
public enum Os
{
	Windows = 0,
	WslDefault = 1,
	Ubuntu = 2,
	Fedora = 3,
	AlmaLinux = 4,
	Alpine = 5,
}
public class Servers
{
	public const int StartPort = 9000;
	public const int IISExpressSslStartPort = 44300;
	static List<int> occupiedPorts = null;
	static HttpClientHandler handler = new HttpClientHandler
	{
		ServerCertificateCustomValidationCallback =
			(HttpRequestMessage message, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslErrors) => true
	};
	public static HttpClient HttpClient = new HttpClient(handler);

	public static List<int> OccupiedPortsAboveStartPort
	{
		get
		{
			if (occupiedPorts == null)
			{
				occupiedPorts = new List<int>();
				
				IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
				IPEndPoint[] tcpEndPoints = ipProperties.GetActiveTcpListeners();
				occupiedPorts.AddRange(tcpEndPoints
					.Select(p => p.Port)
					.Distinct()
					.Where(p => p >= StartPort));
				occupiedPorts.Sort();
			}
			return occupiedPorts;
		}
	}

	public static void Init(Component server)
	{
#if NETFRAMEWORK
		AssemblyLoader.Init($@"..\{Paths.App}.EnterpriseServer\bin;" +
				$@"..\{Paths.App}.EnterpriseServer\bin\Code;" +
				$@"..\{Paths.App}.EnterpriseServer\bin\netstandard;" +
				$@"..\{Paths.App}.Server\bin;" +
				$@"..\{Paths.App}.Server\bin\netstandard;" +
				$@"..\{Paths.App}.Server\bin\Lazy;", "none", false);
#else
		Configuration.ProbingPaths = $@"..\..\..\..\{Paths.App}.EnterpriseServer\bin_dotnet;" +
			$@"..\..\..\..\{Paths.App}.EnterpriseServer\bin\netstandard" +
			$@"..\..\..\..\{Paths.App}.Server\bin_dotnet;" +
			$@"..\..\..\..\{Paths.App}.Server\bin\netstandard;";
		FuseCP.Web.Services.AssemblyLoaderNetCore.Init();
#endif
		// TODO rest of Server path

		try
			{
				var eserver = Assembly.Load("FuseCP.EnterpriseServer");
				if (eserver != null)
				{
					// init password validator
					var validatorType = eserver.GetType("FuseCP.EnterpriseServer.UsernamePasswordValidator");
					var init = validatorType.GetMethod("Init", BindingFlags.Public | BindingFlags.Static);
					init.Invoke(null, new object[0]);
				}
			}
			catch (Exception) { }

		try
		{
			var aserver = Assembly.Load("FuseCP.Server");
			if (aserver != null)
			{
				var validatorType = aserver.GetType("FuseCP.Server.PasswordValidator");
				var init = validatorType.GetMethod("Init", BindingFlags.Public | BindingFlags.Static);
				init.Invoke(null, new object[0]);
			}
		}
		catch (Exception) { }
	}
	public static WSLShell.WSLDistro WslDistro(Os os)
	{
		var shell = WSLShell.Default.Clone as WSLShell;
		shell.LogFile = System.IO.Path.Combine(Paths.Test, "TestResults", $"WSL.log");
		var installed = shell.InstalledDistros
			.OrderByDescending(d => d.Distro);
		var distro = os switch
		{
			Os.Ubuntu => installed
				.FirstOrDefault(d => d.Distro == WSLShell.Distro.Ubuntu ||
					d.Distro == WSLShell.Distro.Ubuntu22 ||
					d.Distro == WSLShell.Distro.Ubuntu22 ||
					d.Distro == WSLShell.Distro.Ubuntu24 ||
					d.OtherDistroName?.Contains("Ubuntu") == true),
			Os.Fedora => installed
				.FirstOrDefault(d => d.Distro == WSLShell.Distro.FedoraRemix ||
					d.OtherDistroName?.Contains("Fedora") == true),
			Os.Alpine => installed
				.FirstOrDefault(d => d.Distro == WSLShell.Distro.Alpine ||
				d.OtherDistroName?.Contains("Alpine") == true),
			Os.AlmaLinux => installed
				.FirstOrDefault(d => d.Distro == WSLShell.Distro.AlmaLinux ||
				d.OtherDistroName?.Contains("Alma") == true),
			_ => throw new ArgumentOutOfRangeException(nameof(os), os, null)
		};
		if (distro == null)
		{
			throw new NotSupportedException($"WSL {distro} is not installed.");
			/*distro = os switch
			{
				Os.Ubuntu => WSLShell.Distro.Ubuntu24,
				Os.Fedora => WSLShell.Distro.FedoraRemix,
				Os.Alpine => WSLShell.Distro.Alpine,
				Os.AlmaLinux => WSLShell.Distro.AlmaLinux
			};
			shell.SetDefaultVersion(2);
			shell.Install(distro);
			Thread.Sleep(20000);*/
		}

		return distro;
	}
	
	static Dictionary<string, IDisposable> Processes = new Dictionary<string, IDisposable>();
	public static void Start((Component Component, Framework Framework, Os Os, Scheme Protocol) type)
	{
		if (type.Protocol == Scheme.Assembly) return;

		var types = new (Component Component, Framework Framework, Os Os, Scheme Protocol)[] {
			(type.Component, type.Framework, type.Os, Scheme.Http),
			(type.Component, type.Framework, type.Os, Scheme.Https),
			(type.Component, type.Framework, type.Os, Scheme.NetTcp),
		};
		var urls = types.Select(t =>
			{
				(Scheme Protocol, string Url) srv = (t.Protocol, GetUrl(t));
				return srv;
			})
			.ToArray();

		if (urls.All(u => Processes.ContainsKey(u.Url))) return;

		IDisposable server = type.Os == Os.Windows ?
		type.Framework switch
		{
			Framework.NetFramework => type.Component == Component.Server
				? new Kestrel(type.Component, urls)
				: new IISExpress(type.Component, urls),
			Framework.Core => new Kestrel(type.Component, urls),
			_ => null
		} :
		type.Framework switch
		{
			Framework.Core => new Kestrel(type.Component, urls, WslDistro(type.Os)),
			_ => null
		};
		if (server == null) return;

		foreach (var u in urls)
		{
			if (Processes.ContainsKey(u.Url))
			{
				Processes[u.Url].Dispose();
				Processes.Remove(u.Url);
			}
			Processes.Add(u.Url, server);
		}
	}

	public static void StopAll()
	{
		foreach (var p in Processes.Values.Distinct()) p.Dispose();
		Processes.Clear();
	}

	public static string Url((Component, Framework Framework, Os Os, Scheme Protocol) type)
	{
		Start(type);
		return GetUrl(type);
	}
	public static string Url(Component component, (Framework Framework, Os Os, Scheme Protocol) type)
		=> Url((component, type.Framework, type.Os, type.Protocol));
	public static string Url(Component component, Framework framework, Os os, Scheme protocol)
		=> Url((component, framework, os, protocol));
	public static bool IsHttp(Web.Clients.Protocols protocol) => protocol <= Web.Clients.Protocols.WSHttp;
	public static bool IsHttps(Web.Clients.Protocols protocol) => protocol >= Web.Clients.Protocols.BasicHttps && protocol <= Web.Clients.Protocols.WSHttps;
	public static string Url(Component component, Framework framework, Os os, Web.Clients.Protocols protocol)
	{
		Scheme scheme;
		if (IsHttp(protocol)) scheme = Scheme.Http;
		else if (IsHttps(protocol)) scheme = Scheme.Https;
		else if (protocol == Web.Clients.Protocols.NetTcp || protocol == Web.Clients.Protocols.NetTcpSsl) scheme = Scheme.NetTcp;
		else throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null);
	
		return Url((component, framework, os, scheme));
	}


	private static string GetUrl((Component Component, Framework Framework, Os Os, Scheme Protocol) type)
	{
		if (type.Protocol == Scheme.Assembly)
		{
			return type.Component switch
			{
				Component.Server => "assembly://FuseCP.Server",
				Component.EnterpriseServer => "assembly://FuseCP.EnterpriseServer",
				_ => throw new ArgumentOutOfRangeException(nameof(type.Component), type.Component, null)
			};
		}
		else
		{
			const int ComponentN = 5;
			const int FrameworkN = 2;
			const int OsN = 5;

			int index, port;
			if (type.Framework == Framework.NetFramework && type.Os != Os.Windows) throw new NotSupportedException(".NET Framework only supported on Windows.");

			if (type.Framework == Framework.NetFramework && type.Os == Os.Windows && type.Protocol == Scheme.Https)
			{
				// IIS Express only supports SSL for ports between 44300 and 44399
				index = (int)type.Component + ComponentN;
				port = IISExpressSslStartPort + index + OccupiedPortsAboveStartPort.Count(p => IISExpressSslStartPort <= p && p <= index);
			}
			else
			{
				index = (int)type.Component + ComponentN * (int)(type.Framework + FrameworkN * (int)(type.Os + OsN * (int)type.Protocol));
				port = StartPort + index + OccupiedPortsAboveStartPort.Count(port => port <= index);
			}
			
			var scheme = type.Protocol switch
			{
				Scheme.Http => "http",
				Scheme.Https => "https",
				Scheme.NetTcp => "net.tcp",
				_ => throw new ArgumentOutOfRangeException(nameof(type.Protocol), type.Protocol, null)
			};
			if (type.Os == Os.Windows) return $"{scheme}://localhost:{port}";
			else
			{
				/*var wsl = new WSLShell(WslDistro(type.Os));
				var ip = wsl.Exec("hostname -I").Output().Result.Trim();
				return $"{scheme}://{ip}:{port}";*/
				return $"{scheme}://localhost:{port}";
			}
		}
	}
}
