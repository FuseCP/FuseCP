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

namespace FuseCP.Server.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FuseCP.Providers;
using FuseCP.Server.Client;
using FuseCP.Web.Clients;
using FuseCP.Tests;
using FuseCP.Providers.OS;
using System.ServiceModel;

[TestClass]
public class Tests
{
	public const string DevServerPassword = FuseCP.Tests.Server.Password;
	public const Component Server = Component.Server;
	public TestContext? TestContext { get; set; }

	[TestMethod]
	[DataRow(Protocols.BasicHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Windows)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Ubuntu)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Ubuntu)]
	public void AutoDiscovery(Protocols protocol, Framework framework,
		Os os)
	{
		if (protocol == Protocols.WSHttp && OSInfo.IsCore) return;

		using (var client = new AutoDiscovery() { Url = Servers.Url(Server, framework, os, protocol) })
		{
			try
			{
				client.Protocol = protocol;
				var path = client.GetServerFilePath();
			}
			catch (FaultException fex)
			{
				TestContext.WriteLine($"Fault: {fex};{fex.InnerException}");
			}
			catch (Exception ex)
			{
				throw;
				Assert.Fail($"Exception {ex}");
			}
		}
	}

	[TestMethod]
	[DataRow(Protocols.BasicHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Windows)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Ubuntu)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Ubuntu)]
	public void Password(Protocols protocol, Framework framework,
	Os os)
	{
		if (protocol == Protocols.WSHttp && OSInfo.IsCore) return;

		using (var client = new Client.OperatingSystem() { Url = Servers.Url(Server, framework, os, protocol) })
		{
			try
			{
				client.SoapHeader = new ServiceProviderSettingsSoapHeader()
				{
					Settings = new string[] { "Provider:ProviderType=FuseCP.Providers.OS.Windows2022, FuseCP.Providers.OS.Windows2022", "Provider:ProviderName=Windows2022" }
				};
				client.Credentials.Password = DevServerPassword;
				client.Protocol = protocol;
				var res = client.DirectoryExists(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
				Assert.Fail();
			}
			catch (Exception ex)
			{

			}
		}
	}
	[TestMethod]
	public async Task TestAutoDicoverAsync()
	{
		using (var client = new AutoDiscovery() { Url = Servers.Url(Server, Framework.NetFramework, Os.Windows, Protocols.NetHttps) })
		{
			try
			{
				var path = await client.GetServerFilePathAsync();
			}
			catch (Exception ex)
			{
				throw;
				Assert.Fail();
			}
		}
	}

	[TestMethod]
	[DataRow(Protocols.BasicHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Windows)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Ubuntu)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Ubuntu)]
	public void Echo(Protocols protocol, Framework framework,
	Os os)
	{
		if (protocol == Protocols.WSHttp && OSInfo.IsCore) return;

		using (var client = new Test() { Url = Servers.Url(Server, framework, os, protocol) })
		{
			try
			{
				client.Protocol = protocol;
				var msg = client.Echo("Hello");
				Assert.AreEqual("Hello", msg);
			}
			catch (FaultException fex)
			{
				TestContext.WriteLine($"Fault: {fex};{fex.InnerException}");
			}
			catch (Exception ex)
			{
				throw;
				Assert.Fail($"Exception {ex}");
			}
		}
	}

	[TestMethod]
	[DataRow(Protocols.BasicHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Windows)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Ubuntu)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Ubuntu)]
	public void AnonymousWithSoapHeader(Protocols protocol, Framework framework,
	Os os)
	{
		if (protocol == Protocols.WSHttp && OSInfo.IsCore) return;

		using (var client = new Test() { Url = Servers.Url(Server, framework, os, protocol) })
		{
			try
			{
				client.Protocol = protocol;
				var h = new ServiceProviderSettingsSoapHeader();
				h.Settings = new string[] { "Hello from Settings!" };
				client.SoapHeader = h;
				var msg = client.EchoSettings();

				Assert.AreEqual("Hello from Settings!", msg);
			}
			catch (FaultException fex)
			{
				TestContext.WriteLine($"Fault: {fex};{fex.InnerException}");
				Assert.Fail($"Exception {fex}");
			}
			catch (Exception ex)
			{
				throw;
				Assert.Fail($"Exception {ex}");
			}
		}
	}

	// Do not test, since net.tcp does not work yet
	//[TestMethod]
	//[DataRow("tcp")]
	//[DataRow("tcp/ssl")]
	public void EchoNetTcp(string api)
	{
		/*using (var client = new Test() { Url = $"{Server.Url(Server, )}/{api}" })
		{
			try
			{
				var msg = client.Echo("Hello");
				Assert.AreEqual("Hello", msg);
			}
			catch (FaultException fex)
			{
				TestContext.WriteLine($"Fault: {fex};{fex.InnerException}");
			}
			catch (Exception ex)
			{
				throw;
				Assert.Fail("Exception", ex);
			}
		}*/
	}

	[TestMethod]
	[DataRow(Protocols.BasicHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Windows)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Ubuntu)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Ubuntu)]
	public void Authenticated(Protocols protocol, Framework framework,
		Os os)
	{
		if (protocol == Protocols.WSHttp && OSInfo.IsCore) return;

		using (var client = new TestWithAuthentication() { Url = Servers.Url(Server, framework, os, protocol) })
		{
			try
			{
				client.Credentials.Password = DevServerPassword;
				client.Protocol = protocol;

				// test echo method
				var res = client.Echo("Hello");
				Assert.AreEqual("Hello", res);

				// test method with soap header
				var header = new ServiceProviderSettingsSoapHeader()
				{
					Settings = new string[] { "Provider:ProviderType=FuseCP.Providers.OS.Windows2022, FuseCP.Providers.OS.Windows2022", "Provider:ProviderName=Windows2022" }
				};
				client.SoapHeader = header;
				var settings = client.EchoSettings();
				Assert.AreEqual(header.Settings[0], settings);

			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}

	[TestMethod]
	[DataRow(Protocols.BasicHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.WSHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.NetFramework, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Windows)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Windows)]
	[DataRow(Protocols.BasicHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.WSHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttps, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.BasicHttp, Framework.Core, Os.Ubuntu)]
	//[DataRow(Protocols.WSHttp, Framework.Core, Os.Ubuntu)]
	[DataRow(Protocols.NetHttp, Framework.Core, Os.Ubuntu)]
	public void WrongPassword(Protocols protocol, Framework framework,
		Os os)
	{
		if (protocol == Protocols.WSHttp && OSInfo.IsCore) return;

		using (var client = new TestWithAuthentication() { Url = Servers.Url(Server, framework, os, protocol) })
		{
			try
			{
				client.SoapHeader = new ServiceProviderSettingsSoapHeader()
				{
					Settings = new string[] { "Provider:ProviderType=FuseCP.Providers.OS.Windows2022, FuseCP.Providers.OS.Windows2022", "Provider:ProviderName=Windows2022" }
				};
				client.Credentials.Password = "1234";
				client.Protocol = protocol;

				// test echo method
				var res = client.Echo("Hello");
				Assert.AreEqual("Hello", res);

				// test method with soap header
				var settings = client.EchoSettings();
				Assert.AreEqual(((ServiceProviderSettingsSoapHeader)client.SoapHeader).Settings[0], settings);
				Assert.Fail("Authorized with wrong password");
			}
			catch (FaultException fex)
			{
				// Expected path: authenticated endpoint should reject wrong password with a SOAP fault.
				TestContext?.WriteLine($"Access denied as expected. Fault: {fex.Message}");
				return;
			}
			catch (ProtocolException pex)
			{
				Assert.Fail($"WrongPassword expected a SOAP auth fault but got protocol/content mismatch at '{client.Url}'. " +
					$"This usually indicates the service endpoint returned HTML (host/config issue) instead of SOAP. Details: {pex.Message}");
			}
			catch (CommunicationException cex)
			{
				Assert.Fail($"WrongPassword expected a SOAP auth fault but endpoint '{client.Url}' was unreachable. " +
					$"Ensure server test host is started and listening before tests run. Details: {cex.Message}");
			}
			catch (Exception ex)
			{
				Assert.Fail($"WrongPassword failed with unexpected exception at '{client.Url}': {ex}");
			}

			Assert.Fail($"WrongPassword expected an authentication failure but no fault was raised at '{client.Url}'.");
		}
	}
}
