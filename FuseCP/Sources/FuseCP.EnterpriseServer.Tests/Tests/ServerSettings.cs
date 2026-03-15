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

using System.Runtime.Serialization;
using FuseCP.EnterpriseServer;
using FuseCP.EnterpriseServer.Client;

namespace FuseCP.Tests
{
	[TestClass]
	public class TestServerSettings
	{
		static readonly object Lock = new object();
		const string EnterpriseServerUrlOverrideEnvVar = "FUSECP_TEST_ES_URL";
		public TestContext TestContext { get; set; }

		[TestMethod]
		public async Task TestESAccess()
		{
			try
			{
				var configuredUrl = Environment.GetEnvironmentVariable(EnterpriseServerUrlOverrideEnvVar);
				if (!string.IsNullOrWhiteSpace(configuredUrl))
				{
					TestContext.WriteLine($"Using EnterpriseServer URL override from {EnterpriseServerUrlOverrideEnvVar}: {configuredUrl}");
					await VerifyEnterpriseServerAccess(configuredUrl);
					return;
				}

				var frameworkCandidates = new[] { Framework.NetFramework, Framework.Core };
				Exception lastException = null;
				foreach (var framework in frameworkCandidates)
				{
					try
					{
						var url = Servers.Url(Component.EnterpriseServer, framework, Os.Windows, Web.Clients.Protocols.BasicHttps);
						TestContext.WriteLine($"Trying EnterpriseServer endpoint via {framework}: {url}");
						await VerifyEnterpriseServerAccess(url);
						return;
					}
					catch (Exception ex)
					{
						lastException = ex;
						TestContext.WriteLine($"EnterpriseServer check via {framework} failed: {ex.Message}");
					}
				}

				throw lastException ?? new InvalidOperationException("EnterpriseServer access checks failed with no exception details.");
			} catch (Exception ex)
			{
				TestContext.WriteLine($"Exception: {ex}");
				if (ex.InnerException != null) TestContext.WriteLine($"InnerException: {ex.InnerException}");
				throw;
			}
		}

		private static async Task VerifyEnterpriseServerAccess(string url)
		{
			var testClient = new esTest();
			testClient.Url = url;
			testClient.Protocol = Web.Clients.Protocols.BasicHttps;
			Assert.AreEqual("Hello", testClient.Echo("Hello"));
			Assert.AreEqual("Hello", await testClient.EchoAsync("Hello"));

			var esClient = new esSystem();
			esClient.Url = url;
			esClient.Protocol = Web.Clients.Protocols.BasicHttps;
			esClient.Credentials.UserName = "serveradmin";
			esClient.Credentials.Password = CryptoUtils.SHA256("123456");
			var settings = esClient.GetSystemSettings(SystemSettings.DEBUG_SETTINGS);
		}

		[TestMethod]
		public async Task TestESAssemblyAccess()
		{
			var testClient = new esTest();
			testClient.Url = EnterpriseServer.AssemblyUrl;
			Assert.AreEqual("Hello", testClient.Echo("Hello"));
			Assert.AreEqual("Hello", await testClient.EchoAsync("Hello"));

			var esClient = new esSystem();
			esClient.Url = EnterpriseServer.AssemblyUrl;
			esClient.Credentials.UserName = "serveradmin";
			esClient.Credentials.Password = CryptoUtils.SHA256("123456");
			var settings = esClient.GetSystemSettings(SystemSettings.DEBUG_SETTINGS);
		}
	}
}
