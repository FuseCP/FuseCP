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

#if !NETFRAMEWORK
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using FuseCP.Providers;
using FuseCP.Providers.OS;
using FuseCP.Web.Services;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace FuseCP.Web.Services
{
	public static class StartupCore
	{
		public const int KB = Configuration.KB;
		public const int MB = Configuration.MB;
		public const int MaxReceivedMessageSize = Configuration.MaxReceivedMessageSize;
		public const int MaxBufferSize = Configuration.MaxBufferSize;
		public const int MaxBytesPerRead = Configuration.MaxBytesPerRead;
		public const int MaxDepth = Configuration.MaxDepth;
		public const int MaxArrayLength = Configuration.MaxArrayLength;
		public const int MaxStringContentLength = Configuration.MaxStringContentLength;
		public const int MaxNameTableCharCount = Configuration.MaxNameTableCharCount;
		public const bool AllowInsecureHttp = Configuration.AllowInsecureHttp;

		public static bool IsIIS => OSInfo.IsWindows && NativeMethods.IsAspNetCoreModuleLoaded();

		public static void Log(string msg)
		{
			Console.WriteLine(msg);
			if (Debugger.IsAttached) Debugger.Log(1, "FuseCP", msg);
			//Trace.TraceInformation(msg);
		}
		public static void Error(string msg)
		{
			var col = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(msg);
			Console.ForegroundColor = col;
			if (Debugger.IsAttached) Debugger.Log(1, "FuseCP", msg);
			//Trace.TraceError(msg);
		}

		static int? HttpPort = null;
		static int? HttpsPort = null;
		static int? NetTcpPort = null;
		static string HttpHost = null;
		static string HttpsHost = null;
		static string NetTcpHost = null;
		static StoreLocation StoreLocation = StoreLocation.LocalMachine;
		static StoreName StoreName = StoreName.My;
		static X509FindType FindType = X509FindType.FindBySubjectName;
		static string CertificateName = null;
		static string CertificateFile = null;
		static string CertificatePassword = null;
		static string Password;
		static string KeyFile = null;
		static string AllowedHosts = "0.0.0.0";
		static bool IsLocalService = false;
		static TraceLevel TraceLevel = TraceLevel.Off;
		static X509Certificate2 Certificate = null;
		static string WebApplicationsPath = null;
		static int? ServerRequestTimeout = null;
		static string ConnectionString = null;
		static string AltConnectionString = null;
		static string CryptoKey = null;
		static string AltCryptoKey = null;
		static bool? EncryptionEnabled = null;
		static string ExposeWebServices = null;
		static ulong? HttpFile = null;
		static ulong? HttpsFile = null;
		static ulong? NetTcpFile = null;
		static TimeSpan IdleShutdownTime = default;

		static X509Certificate2 GetCertificate()
		{
			if (string.IsNullOrEmpty(CertificateFile) || string.IsNullOrEmpty(CertificatePassword))
			{
				if (!string.IsNullOrEmpty(CertificateName))
				{
					X509Store store = new X509Store(StoreName, StoreLocation);
					store.Open(OpenFlags.ReadOnly);
					Certificate = store.Certificates.Find(FindType, CertificateName, false).FirstOrDefault();
					if (Certificate != null) Log($"Use certificate {Certificate.GetNameInfo(X509NameType.SimpleName, false)} {Certificate.FriendlyName} found in {StoreName} at {StoreLocation}");
					else Error($"Certificate for {CertificateName} not found in {StoreName} at {StoreLocation}");
					return Certificate;
				}
			}
			else
			{
				var file = new FileInfo(CertificateFile).FullName;
				if (File.Exists(file))
				{
					if (!string.IsNullOrEmpty(KeyFile))
					{
						var keyFile = new FileInfo(KeyFile).FullName;
						if (File.Exists(keyFile)) CertificatePassword = File.ReadAllText(keyFile);
					}
					Certificate = X509CertificateLoader.LoadPkcs12FromFile(file, CertificatePassword, X509KeyStorageFlags.DefaultKeySet);
					if (Certificate != null) Log($"Use certificate {Certificate.SubjectName} from {file}.");
					else Error($"The certificate {file} was not found.");
					return Certificate;
				}
			}
			Error("No certificate specified.");
			return null;
		}

		public static void Init(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			Configuration.Read(builder.Configuration, args);

			HttpPort = Configuration.HttpPort;
			HttpHost = Configuration.HttpHost;
			HttpFile = Configuration.HttpFile;
			HttpsPort = Configuration.HttpsPort;
			HttpsHost = Configuration.HttpsHost;
			HttpsFile = Configuration.HttpsFile;
			NetTcpPort = Configuration.NetTcpPort;
			NetTcpHost = Configuration.NetTcpHost;
			NetTcpFile = Configuration.NetTcpFile;
			StoreLocation = Configuration.StoreLocation;
			StoreName = Configuration.StoreName;
			FindType = Configuration.FindType;
			CertificateName = Configuration.CertificateName;
			CertificateFile = Configuration.CertificateFile;
			CertificatePassword = Configuration.CertificatePassword;
			Password = Configuration.Password;
			AllowedHosts = Configuration.AllowedHosts;
			TraceLevel = Configuration.TraceLevel;
			KeyFile = Configuration.KeyFile;
			ExposeWebServices = Configuration.ExposeWebServices;
			WebApplicationsPath = Configuration.WebApplicationsPath;
			ServerRequestTimeout = Configuration.ServerRequestTimeout;
			ConnectionString = Configuration.ConnectionString;
			AltConnectionString = Configuration.AltConnectionString;
			CryptoKey = Configuration.CryptoKey;
			AltCryptoKey = Configuration.AltCryptoKey;
			EncryptionEnabled = Configuration.EncryptionEnabled;
			TraceLevel = Configuration.TraceLevel;
			IsLocalService = Configuration.IsLocalService;
			IdleShutdownTime = Configuration.IdleShutdownTime;

			if (TraceLevel != TraceLevel.Off)
			{
				var listener = OSInfo.Current.DefaultTraceListener;
				Trace.Listeners.Add(listener);
				Log($"Trace level set to {TraceLevel}");
			}

			Server.ConfigurationComplete?.Invoke();

			builder.Services.AddRazorPages();
			builder.Services.AddHttpContextAccessor();
			if (IdleShutdownTime != default && OSInfo.IsSystemd &&
				(HttpFile.HasValue || HttpsFile.HasValue || NetTcpFile.HasValue))
			{
				Console.WriteLine($"Idle shutdown time set to {IdleShutdownTime}");
				builder.Services.AddSingleton<ISystemdNotifier, SystemdNotifier>();
				builder.Services.AddHostedService<IdleShutdownService>();
			}

			if (OSInfo.IsSystemd)
			{
				builder.Host.UseSystemd();
			} else if (OSInfo.IsWindows)
			{
				builder.Host.UseWindowsService();
			}
			Server.ConfigureBuilder?.Invoke(builder);
			ConfigureServices(builder.Services);

			if (NetTcpPort.HasValue)
			{
				builder.WebHost.UseNetTcp(NetTcpPort.Value);
				Log($"Listen on net.tcp port {NetTcpPort.Value}");
			}

			if (IsIIS)
			{
				builder.Services.Configure<IISServerOptions>(options =>
				{
					options.AllowSynchronousIO = true;
					options.MaxRequestBodySize = MaxReceivedMessageSize;
				});
				builder.WebHost.UseIIS();
			}
			else
			{
				if (NetTcpPort.HasValue) builder.WebHost.UseNetTcp(NetTcpPort.Value);

				builder.WebHost.UseKestrel(options =>
				{
					options.AllowSynchronousIO = true;

					if (OSInfo.IsSystemd && !HttpFile.HasValue && !HttpsFile.HasValue && !NetTcpFile.HasValue)
					{
						options.UseSystemd();
					}

					if (HttpPort.HasValue && !HttpFile.HasValue) options.ListenAnyIP(HttpPort.Value, listenOptions =>
					{
						if (Debugger.IsAttached) listenOptions.UseConnectionLogging();
					});
					if (HttpsPort.HasValue && !HttpsFile.HasValue) options.ListenAnyIP(HttpsPort.Value, listenOptions =>
					{
						listenOptions.UseHttps(listenOptions =>
						{
							// if (Certificate == null) return;
							Certificate = GetCertificate();
							listenOptions.ServerCertificate = Certificate;

						});

						if (Debugger.IsAttached) listenOptions.UseConnectionLogging();
					});
					if (HttpFile.HasValue) options.ListenHandle(HttpFile.Value, listenOptions =>
					{
						if (Debugger.IsAttached) listenOptions.UseConnectionLogging();
					});
					if (HttpsFile.HasValue) options.ListenHandle(HttpsFile.Value, listenOptions =>
					{
						listenOptions.UseHttps(listenOptions =>
						{
							Certificate = GetCertificate();
							listenOptions.ServerCertificate = Certificate;

						});

						if (Debugger.IsAttached) listenOptions.UseConnectionLogging();
					});
				});
			}

			Server.ContentRoot = builder.Environment.ContentRootPath;
			Server.WebRoot = builder.Environment.WebRootPath;
			if (Server.ContentRoot.EndsWith($"{Path.DirectorySeparatorChar}bin_dotnet")) Server.ContentRoot = Path.GetDirectoryName(Server.ContentRoot);
			
			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseDefaultFiles()
				.UseStaticFiles()
				.UseRouting();

			app.MapRazorPages();

			ConfigureWCF(app);

			var tunnelHandler = new TunnelHandlerCore();
			tunnelHandler.Init(app);

			if (IdleShutdownTime != default && OSInfo.IsSystemd &&
				(HttpFile.HasValue || HttpsFile.HasValue || NetTcpFile.HasValue)) app.UseIdleTimeout(IdleShutdownTime);

			Server.ConfigureApp?.Invoke(app);

			app.Run();
		}

		public static void ConfigureServices(IServiceCollection services)
		{
			//Enable CoreWCF Services, with metadata (WSDL) support
			var a = Assembly.GetEntryAssembly();
			var srvcAssemblies = AppDomain.CurrentDomain.GetAssemblies()
				.Select(assembly => assembly.GetName().Name.Replace('.', ' '))
				.Where(name => name == "FuseCP Server" || name == "FuseCP EnterpriseServer");
			var title = $"{string.Join(" & ", srvcAssemblies.ToArray())} API";
            var hasServer = title.Contains("FuseCP Server");
            var hasEnterprise = title.Contains("FuseCP EnterpriseServer");

            var openServices = $"{(hasServer || hasEnterprise ? "but the " : "")}{(hasEnterprise ? $"esAuthentication{(hasServer ? ", " : " & ")}esTest{(hasServer ? ", " : " ")}" : "")}{(hasServer ? "AutoDiscovery & Test " : "")}";
            var clientAssembly = $"{(hasEnterprise ? "FuseCP.EnterpriseServer.Client " : "")}{(hasEnterprise && hasServer ? "& " : "")}{(hasServer ? "FuseCP.Server.Client " : "")}";

            var ver = a.GetCustomAttribute<AssemblyVersionAttribute>();
			services
				.AddServiceModelServices()
				.AddServiceModelMetadata()
				.AddServiceModelWebServices(o =>
				{
					o.Title = title;
					o.Version = ver?.Version ?? "1.0";
					o.Description = $"This is the REST API of FuseCP. Note that all {openServices}services use Basic Http Authentication. If you use .NET, you might want to access the API over WCF/SOAP, in this case refer to the {clientAssembly}assembly.";
					o.TermsOfService = new("http://fusecp.com/terms");
					o.ContactName = "Contact";
					o.ContactEmail = "support@fusecp.com";
					o.ContactUrl = new("http://fusecp.com/contact");
					o.ExternalDocumentUrl = new("http://fusecp.com/apidoc.pdf");
					o.ExternalDocumentDescription = "Documentation";
				});

            services.AddSingleton(new SwaggerOptions());
			//.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
		}

		public static IServiceBuilder AddServiceEndpoint(this IServiceBuilder builder, Type service, Type contract, Binding binding, Uri address)
		{
			var readerQuotas = new XmlDictionaryReaderQuotas
			{
				MaxBytesPerRead = MaxBytesPerRead,
				MaxDepth = MaxDepth,
				MaxArrayLength = MaxArrayLength,
				MaxStringContentLength = MaxStringContentLength,
				MaxNameTableCharCount = MaxNameTableCharCount
			};
			if (binding is BasicHttpBinding basicBinding)
			{
				basicBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
				basicBinding.MaxBufferSize = MaxBufferSize;
				basicBinding.ReaderQuotas = readerQuotas;
			} else if (binding is WSHttpBinding wsBinding)
			{
				wsBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
				wsBinding.ReaderQuotas = readerQuotas;
			} else if (binding is NetHttpBinding netBinding)
			{
				netBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
				netBinding.MaxBufferSize = MaxBufferSize;
				netBinding.ReaderQuotas = readerQuotas;
			} else if (binding is NetTcpBinding tcpBinding)
			{
				tcpBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
				tcpBinding.MaxBufferSize = MaxBufferSize;
				tcpBinding.ReaderQuotas = readerQuotas;
			}
			builder.AddServiceEndpoint(service, contract, binding, address, address, conf => conf.EndpointBehaviors.Add(new SoapHeaderMessageInspector()));
			return builder;
		}
		public static IServiceBuilder AddServiceWebEndpoint(this IServiceBuilder builder, Type service, Type contract, WebHttpBinding binding, Uri address)
		{
			var readerQuotas = new XmlDictionaryReaderQuotas
			{
				MaxBytesPerRead = MaxBytesPerRead,
				MaxDepth = MaxDepth,
				MaxArrayLength = MaxArrayLength,
				MaxStringContentLength = MaxStringContentLength,
				MaxNameTableCharCount = MaxNameTableCharCount
			};
			binding.MaxReceivedMessageSize = MaxReceivedMessageSize;
			binding.MaxBufferSize = MaxBufferSize;
			binding.ReaderQuotas = readerQuotas;
			builder.AddServiceWebEndpoint(service, contract, binding, address, address, behavior =>
			{
				behavior.AutomaticFormatSelectionEnabled = true;
				//behavior.DefaultBodyStyle = CoreWCF.Web.WebMessageBodyStyle.WrappedRequest;
				behavior.HelpEnabled = true;
			});
			return builder;
		}

		public static void ConfigureWCF(IApplicationBuilder app)
		{
			app.UseSwagger();
			app.UseSwaggerUI();

            app.UseServiceModel(builder =>
			{
				var webServices = ServiceTypes.Types;

				var isLocal = Configuration.IsLocalService;

				foreach (var ws in webServices)
				{
					var policy = ws.Contract.GetCustomAttributes(false).OfType<Services.PolicyAttribute>().FirstOrDefault();
					var isEncrypted = policy != null;
					var isAuthenticated = isEncrypted && policy.Policy != PolicyAttribute.Encrypted;

					BasicHttpBinding basicHttpBinding;
					WSHttpBinding wsHttpBinding;
					NetHttpBinding netHttpBinding;
					NetTcpBinding netTcpBinding = null, netTcpSslBinding = null;
					WebHttpBinding webHttpBinding;

					Uri basicUri, wsHttpUri, netHttpUri, netTcpUri = null, netTcpSslUri, webHttpUri, defaultUri;

					builder.AddService(ws.Service, options =>
						{
							//if (HttpPort.HasValue) options.BaseAddresses.Add(new Uri($"http://{HttpHost}:{HttpPort}/{ws.Service.Name}"));
							//if (HttpsPort.HasValue) options.BaseAddresses.Add(new Uri($"https://{HttpsHost}:{HttpsPort}/{ws.Service.Name}"));
							//if (NetTcpPort.HasValue) options.BaseAddresses.Add(new Uri($"tcp.net://{NetTcpHost}:{NetTcpPort}/{ws.Service.Name}"));
							options.DebugBehavior.IncludeExceptionDetailInFaults = true;
						});

					if (isEncrypted)
					{
						if (HttpPort.HasValue && (isLocal || AllowInsecureHttp))
						{
							basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
							basicUri = new Uri($"http://{HttpHost}:{HttpPort}/basic/{ws.Service.Name}");
							wsHttpBinding = new WSHttpBinding(CoreWCF.SecurityMode.None);
							wsHttpUri = new Uri($"http://{HttpHost}:{HttpPort}/ws/{ws.Service.Name}");
							netHttpBinding = new NetHttpBinding(BasicHttpSecurityMode.None);
							netHttpBinding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Never;
							netHttpUri = new Uri($"http://{HttpHost}:{HttpPort}/net/{ws.Service.Name}");
							webHttpBinding = new WebHttpBinding(isAuthenticated ? WebHttpSecurityMode.TransportCredentialOnly : WebHttpSecurityMode.None);
							webHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
							webHttpUri = new Uri($"http://{HttpHost}:{HttpPort}/api/{ws.Service.Name}");
							defaultUri = new Uri($"http://{HttpHost}:{HttpPort}/{ws.Service.Name}");
							builder.AddServiceEndpoint(ws.Service, ws.Contract, basicHttpBinding, defaultUri)
												.AddServiceEndpoint(ws.Service, ws.Contract, basicHttpBinding, basicUri)
												.AddServiceEndpoint(ws.Service, ws.Contract, wsHttpBinding, wsHttpUri)
												.AddServiceEndpoint(ws.Service, ws.Contract, netHttpBinding, netHttpUri)
												.AddServiceWebEndpoint(ws.Service, ws.Contract, webHttpBinding, webHttpUri);
						}
						if (HttpsPort.HasValue)
						{
							basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
							basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
							basicUri = new Uri($"https://{HttpsHost}:{HttpsPort}/basic/{ws.Service.Name}");
							wsHttpBinding = new WSHttpBinding(CoreWCF.SecurityMode.Transport);
							wsHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
							wsHttpUri = new Uri($"https://{HttpsHost}:{HttpsPort}/ws/{ws.Service.Name}");
							netHttpBinding = new NetHttpBinding(BasicHttpSecurityMode.Transport);
							netHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
							netHttpBinding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Never;
							netHttpUri = new Uri($"https://{HttpsHost}:{HttpsPort}/net/{ws.Service.Name}");
							webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.Transport);
							webHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
							webHttpUri = new Uri($"https://{HttpsHost}:{HttpsPort}/api/{ws.Service.Name}");
							defaultUri = new Uri($"https://{HttpsHost}:{HttpsPort}/{ws.Service.Name}");
							builder.AddServiceEndpoint(ws.Service, ws.Contract, basicHttpBinding, defaultUri)
												.AddServiceEndpoint(ws.Service, ws.Contract, basicHttpBinding, basicUri)
												.AddServiceEndpoint(ws.Service, ws.Contract, wsHttpBinding, wsHttpUri)
												.AddServiceEndpoint(ws.Service, ws.Contract, netHttpBinding, netHttpUri)
												.AddServiceWebEndpoint(ws.Service, ws.Contract, webHttpBinding, webHttpUri);
						}
						if (NetTcpPort.HasValue)
						{
							if (Certificate != null)
							{
								netTcpSslBinding = new NetTcpBinding(SecurityMode.Transport);
								netTcpSslBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
								netTcpSslBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
								netTcpSslUri = new Uri($"net.tcp://{NetTcpHost}:{NetTcpPort}/tcp/ssl/{ws.Service.Name}");
								builder.AddServiceEndpoint(ws.Service, ws.Contract, netTcpSslBinding, netTcpSslUri);
							}
							if (isLocal || AllowInsecureHttp)
							{
								netTcpBinding = new NetTcpBinding(SecurityMode.None);
								netTcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
								netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
								netTcpUri = new Uri($"net.tcp://{NetTcpHost}:{NetTcpPort}/tcp/{ws.Service.Name}");
								builder.AddServiceEndpoint(ws.Service, ws.Contract, netTcpBinding, netTcpUri);
							}
						}
					}
					else // isEncrypted = false
					{
						if (HttpPort.HasValue)
						{
							basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
							basicUri = new Uri($"http://{HttpHost}:{HttpPort}/basic/{ws.Service.Name}");
							wsHttpBinding = new WSHttpBinding(CoreWCF.SecurityMode.None);
							wsHttpUri = new Uri($"http://{HttpHost}:{HttpPort}/ws/{ws.Service.Name}");
							netHttpBinding = new NetHttpBinding(BasicHttpSecurityMode.None);
							netHttpBinding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Never;
							netHttpUri = new Uri($"http://{HttpHost}:{HttpPort}/net/{ws.Service.Name}");
							webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.None);
							webHttpUri = new Uri($"http://{HttpHost}:{HttpPort}/api/{ws.Service.Name}");
							defaultUri = new Uri($"http://{HttpHost}:{HttpPort}/{ws.Service.Name}");
							builder.AddServiceEndpoint(ws.Service, ws.Contract, basicHttpBinding, defaultUri)
											.AddServiceEndpoint(ws.Service, ws.Contract, basicHttpBinding, basicUri)
											.AddServiceEndpoint(ws.Service, ws.Contract, wsHttpBinding, wsHttpUri)
											.AddServiceEndpoint(ws.Service, ws.Contract, netHttpBinding, netHttpUri)
											.AddServiceWebEndpoint(ws.Service, ws.Contract, webHttpBinding, webHttpUri);
						}
						if (HttpsPort.HasValue)
						{
							basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
							basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
							basicUri = new Uri($"https://{HttpsHost}:{HttpsPort}/basic/{ws.Service.Name}");
							wsHttpBinding = new WSHttpBinding(CoreWCF.SecurityMode.Transport);
							wsHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
							wsHttpUri = new Uri($"https://{HttpsHost}:{HttpsPort}/ws/{ws.Service.Name}");
							netHttpBinding = new NetHttpBinding(BasicHttpSecurityMode.Transport);
							netHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
							netHttpBinding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Never;
							netHttpUri = new Uri($"https://{HttpsHost}:{HttpsPort}/net/{ws.Service.Name}");
							webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.Transport);
							webHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
							webHttpUri = new Uri($"https://{HttpsHost}:{HttpsPort}/api/{ws.Service.Name}");
							defaultUri = new Uri($"https://{HttpsHost}:{HttpsPort}/{ws.Service.Name}");
							builder.AddServiceEndpoint(ws.Service, ws.Contract, basicHttpBinding, defaultUri)
												.AddServiceEndpoint(ws.Service, ws.Contract, basicHttpBinding, basicUri)
												.AddServiceEndpoint(ws.Service, ws.Contract, wsHttpBinding, wsHttpUri)
												.AddServiceEndpoint(ws.Service, ws.Contract, netHttpBinding, netHttpUri)
												.AddServiceWebEndpoint(ws.Service, ws.Contract, webHttpBinding, webHttpUri);
						}
						if (NetTcpPort.HasValue)
						{
							if (Certificate != null)
							{
								netTcpSslBinding = new NetTcpBinding(SecurityMode.Transport);
								netTcpSslBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
								netTcpSslBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
								netTcpSslUri = new Uri($"net.tcp://{NetTcpHost}:{NetTcpPort}/tcp/ssl/{ws.Service.Name}");
								builder.AddServiceEndpoint(ws.Service, ws.Contract, netTcpSslBinding, netTcpSslUri);
							}
							netTcpBinding = new NetTcpBinding(SecurityMode.None);
							netTcpUri = new Uri($"net.tcp://{NetTcpHost}:{NetTcpPort}/tcp/{ws.Service.Name}");
							builder.AddServiceEndpoint(ws.Service, ws.Contract, netTcpBinding, netTcpUri);
						}
					}
				}

				// Configure WSDL to be available
				var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<ServiceMetadataBehavior>();
				serviceMetadataBehavior.HttpGetEnabled = HttpPort.HasValue;
				serviceMetadataBehavior.HttpsGetEnabled = HttpsPort.HasValue;

				builder.ConfigureAllServiceHostBase(host =>
				{
					if (Certificate != null) host.Credentials.ServiceCertificate.Certificate = Certificate;
					var behavior = host.Description.Behaviors.Find<ServiceDebugBehavior>();
					if (behavior != null) behavior.IncludeExceptionDetailInFaults = true;
					else
					{
						var debugBehavior = new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true };
						host.Description.Behaviors.Add(debugBehavior);
					}
					host.Authorization.ServiceAuthorizationManager = new RestAuthorizationManager();
					foreach (var endpoint in host.Description.Endpoints)
					{
						if (endpoint.Binding is WebHttpBinding)
						{
							endpoint.EndpointBehaviors.Add(new SoapHeaderMessageInspector());
						}
					}
				});
			});
		}
	}
}
#endif
