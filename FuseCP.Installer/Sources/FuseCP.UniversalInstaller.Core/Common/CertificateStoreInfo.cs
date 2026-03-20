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
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using FuseCP.Providers.OS;
using UnixFileMode = FuseCP.Providers.OS.UnixFileMode;

namespace FuseCP.UniversalInstaller.Core
{
	public class CertificateStoreInfo
	{
		public class StoreId
		{
			public StoreName Name;
			public StoreLocation Location;
		}

		public static IEnumerable<StoreId> GetStores()
		{
			var names = new StoreName[] { StoreName.My, StoreName.Root, StoreName.TrustedPeople };
			var locations = new StoreLocation[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine };

			foreach (var name in names)
			{
				foreach (var loc in locations)
				{
					StoreId storeId = null;
					try
					{
						var store = new X509Store(name, loc);
						store.Open(OpenFlags.ReadOnly);
						storeId = new StoreId() { Name = name, Location = loc };
					}
					catch { }

					if (storeId != null) yield return storeId;
				}
			}
		}

		public static bool ExistsDirect(StoreLocation location, StoreName name, X509FindType findType, object findValue)
		{
			try
			{
				var store = new X509Store(name, location);
				store.Open(OpenFlags.ReadOnly);
				return store.Certificates.Find(findType, findValue, true)
					.OfType<X509Certificate2>()
					.Any();
			}
			catch
			{
				return false;
			}
		}

		static string hostDllCore = null;
		public static string GetHostCore(Assembly assembly = null)
		{
			if (hostDllCore == null)
			{
				assembly = assembly ?? Assembly.GetExecutingAssembly();
				var fileName = assembly.Location;
				OSInfo.Unix.GrantUnixPermissions(fileName, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
					 UnixFileMode.GroupRead | UnixFileMode.GroupExecute | UnixFileMode.OtherRead | UnixFileMode.OtherExecute);

				var runtimeconfig = Path.Combine(Path.GetDirectoryName(fileName),
					$"{Path.GetFileNameWithoutExtension(fileName)}.runtimeconfig.json");
				File.WriteAllText(runtimeconfig,
	@"{
	""runtimeOptions"": {
		""tfm"": ""net10.0"",
		""framework"": {
			""name"": ""Microsoft.NETCore.App"",
			""version"": ""10.0.0""
		}
	}
}", Encoding.UTF8);
				hostDllCore = fileName;
			}
			return hostDllCore;
		}

		static string hostDllFX = null;
		public static string GetHostFX(Assembly assembly = null)
		{
			if (hostDllFX == null)
			{
				const string DllNameFX = "FuseCP.UniversalInstaller.CertificateStoreInfo.NetFX.exe";
				assembly = assembly ?? Assembly.GetExecutingAssembly();
				var resources = assembly.GetManifestResourceNames();
				var res = resources
					.FirstOrDefault(name => name.EndsWith(DllNameFX));
				var fileName = $"{Path.GetTempFileName()}.{DllNameFX}";
				using (var stream = assembly.GetManifestResourceStream(res))
				using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					stream.CopyTo(file);
				}
				hostDllFX = fileName;
			}
			return hostDllFX;
		}

		public static void GetStoreNames(out string[] names, out string[] locations)
		{
			if (!OSInfo.IsWindows && OSInfo.IsMono)
			{
				if (Shell.Default.Find("dotnet") != null)
				{
					var dll = GetHostCore();
					var shell = Shell.Default.Exec($"dotnet \"{dll}\"");
					var text = shell.Output().Result;
					var exitCode = shell.ExitCode().Result;
					var namesList = new List<string>();
					var locList = new List<string>();
					if (exitCode == 0)
					{
						var output = new StringReader(text);
						var line = output.ReadLine();
						while (line != null)
						{
							namesList.Add(line);
							line = output.ReadLine();
							if (line != null)
							{
								locList.Add(line);
								line = output.ReadLine();
							}
						}
						names = namesList.Distinct().ToArray();
						locations = locList.Distinct().ToArray();
						return;
					}
				}
				names = new string[3] { nameof(StoreName.My), nameof(StoreName.Root), nameof(StoreName.TrustedPeople) };
				locations = new string[2] { nameof(StoreLocation.CurrentUser), nameof(StoreLocation.LocalMachine) };
			}
			else
			{
				var stores = GetStores();
				names = stores.Select(s => s.Name.ToString()).Distinct().ToArray();
				locations = stores.Select(s => s.Location.ToString()).Distinct().ToArray();
			}
		}
		public static bool Exists(StoreLocation location, StoreName name, X509FindType findType, object findValue)
		{
			if (!OSInfo.IsWindows && OSInfo.IsMono)
			{
				if (Shell.Default.Find("dotnet") == null) return true;

				var dll = GetHostCore();
				var code = Shell.Default.Exec($"dotnet \"{dll}\" {name} {location} {findType} {findValue}").ExitCode().Result;
				return code == 0;
			}
			else return ExistsDirect(location, name, findType, findValue);
		}
	}
}
