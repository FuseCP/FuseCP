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
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
#if NETCOREAPP
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
#endif

using FuseCP.Providers.OS;

namespace FuseCP.Web.Services;

public class Configuration
{
	public const int KB = 1024;
	public const int MB = 1024 * 1024;
	public const int MaxReceivedMessageSize = 32 * MB;
	public const int MaxBufferSize = 32 * MB;
	public const int MaxBytesPerRead = 4 * KB;
	public const int MaxDepth = 1024;
	public const int MaxArrayLength = 1 * MB;
	public const int MaxStringContentLength = 16 * MB;
	public const int MaxNameTableCharCount = 16 * MB;

	public const bool AllowInsecureHttp = PolicyAttribute.AllowInsecureHttp;

	public static int? HttpPort = null;
	public static int? HttpsPort = null;
	public static ulong? HttpFile = null;
	public static ulong? HttpsFile = null;
	public static int? NetTcpPort = null;
	public static ulong? NetTcpFile = null;
	public static string HttpHost = null;
	public static string HttpsHost = null;
	public static string NetTcpHost = null;
	public static StoreLocation StoreLocation = StoreLocation.LocalMachine;
	public static StoreName StoreName = StoreName.My;
	public static X509FindType FindType = X509FindType.FindBySubjectName;
	public static string CertificateName = null;
	public static string CertificateFile = null;
	public static string CertificatePassword = null;
	public static string Password;
	public static bool AllowLegacyPasswordAuthentication = true;
	public static string KeyFile = null;
	public static string ProbingPaths = "";
	public static string AllowedHosts = "0.0.0.0";
	public static bool IsLocalService = false;
	public static TraceLevel TraceLevel = TraceLevel.Off;
	public static X509Certificate2 Certificate = null;
	public static string WebApplicationsPath = null;
	public static int? ServerRequestTimeout = null;
	public static string ConnectionString = null;
	public static string AltConnectionString = null;
	public static string CryptoKey = null;
	public static string AltCryptoKey = null;
	public static bool? EncryptionEnabled = null;
	public static string ExposeWebServices = null;
	public static bool IsPortal = false;
	public static TimeSpan IdleShutdownTime = default;
	public static void Log(string msg)
	{
		Console.WriteLine(msg);
		if (Debugger.IsAttached) Debugger.Log(1, "FuseCP", msg);
		//Trace.TraceInformation(msg);
	}

#if NETCOREAPP
	public static void Read(IConfiguration configuration, string[] args)
	{
		ProbingPaths = configuration["probingPaths"];
		AssemblyLoaderNetCore.Init();
		string urls = null;
		var urlsParPos = Array.IndexOf(args, "--urls");
		if (urlsParPos >= 0 && urlsParPos < args.Length - 1) urls = args[urlsParPos + 1];
		urls = urls ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ??
			Environment.GetEnvironmentVariable("DOTNET_URLS") ??
			configuration["applicationUrls"];
		if (urls != null)
		{
			Console.WriteLine($"Listening on URLs: {urls}");
			foreach (var url in urls.Split(';'))
			{
				var uri = new Uri(url);
				if (uri.Scheme == "http")
				{
					ulong file = 0;
					if (ulong.TryParse(uri.UserInfo, out file))
					{
						HttpFile = file;
					}
					HttpPort = uri.Port;
					HttpHost = uri.Host;
				}
				else if (uri.Scheme == "https")
				{
					ulong file = 0;
					if (ulong.TryParse(uri.UserInfo, out file))
					{
						HttpsFile = file;
					}
					HttpsPort = uri.Port;
					HttpsHost = uri.Host;
				}
				else if (uri.Scheme == "net.tcp")
				{
					ulong file = 0;
					if (ulong.TryParse(uri.UserInfo, out file))
					{
						NetTcpFile = file;
					}
					NetTcpPort = uri.Port;
					NetTcpHost = uri.Host;
				}
			}
		}
		StoreLocation = configuration.GetValue<StoreLocation?>("ServerCertificate:StoreLocation") ?? StoreLocation.LocalMachine;
		StoreName = configuration.GetValue<StoreName?>("ServerCertificate:StoreName") ?? StoreName.My;
		FindType = configuration.GetValue<X509FindType?>("ServerCertificate:FindType") ?? X509FindType.FindBySubjectName;
		CertificateName = configuration.GetValue<string>("ServerCertificate:Name") ?? null;
		CertificateFile = configuration.GetValue<string>("ServerCertificate:File");
		CertificatePassword = configuration.GetValue<string>("ServerCertificate:Password");
		Password = configuration.GetValue<string>("Server:Password") ?? String.Empty;
		AllowLegacyPasswordAuthentication = configuration.GetValue<bool?>("Server:AllowLegacyPasswordAuthentication") ?? true;
		AllowedHosts = configuration.GetValue<string>("AllowedHosts") ?? "*";
		TraceLevel = configuration.GetValue<TraceLevel?>("TraceLevel") ?? TraceLevel.Off;
		KeyFile = configuration.GetValue<string>("ServerCertificate:KeyFile");
		ExposeWebServices = configuration.GetValue<string>("exposeWebServices") ?? "";
		WebApplicationsPath = configuration.GetValue<string>("EnterpriseServer:WebApplicationPath");
		ServerRequestTimeout = configuration.GetValue<int?>("EnterpriseServer:ServerRequestTimeout") ?? -1;
		ConnectionString = configuration.GetValue<string>("EnterpriseServer:ConnectionString");
		AltConnectionString = configuration.GetValue<string>("EnterpriseServer:AltConnectionString");
		CryptoKey = configuration.GetValue<string>("EnterpriseServer:CryptoKey");
		AltCryptoKey = configuration.GetValue<string>("EnterpriseServer:AltCryptoKey");
		EncryptionEnabled = configuration.GetValue<bool?>("EnterpriseServer:EncryptionEnabled");
		IsLocalService = AllowedHosts.Split(';')
			.All(host => host != "*" && DnsService.IsHostLAN(host)); // local network ip
		IdleShutdownTime = configuration.GetValue<TimeSpan?>("IdleShutdownTime") ?? default;
	}
#endif
}
