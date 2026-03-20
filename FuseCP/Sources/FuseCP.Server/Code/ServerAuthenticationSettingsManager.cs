// Copyright (C) 2026 FuseCP
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
using System.IO;
using System.Linq;
using FuseCP.Providers;
using FuseCP.Server.Code;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FuseCP.Server
{
	public class ServerAuthenticationSettingsManager
	{
		private const string HardenedConfigFileName = "appsettings.hardened.json";

		public ServerAuthenticationInfo HardenServerAuthentication(string sharedSecret)
		{
			if (string.IsNullOrWhiteSpace(sharedSecret))
				throw new ArgumentException("Shared secret is required.", nameof(sharedSecret));

			string hashedSharedSecret = Cryptor.SHA256(sharedSecret);
			PersistSettings(hashedSharedSecret, allowLegacyPasswordAuthentication: false);
			Settings.ApplyAuthenticationSettings(hashedSharedSecret, allowLegacyPasswordAuthentication: false);

			return AutoDiscoveryHelper.GetServerAuthenticationInfo();
		}

		private static void PersistSettings(string hashedSharedSecret, bool allowLegacyPasswordAuthentication)
		{
			// Write only the two hardened values to a dedicated overlay file that:
			//   - is registered in Startup.Core with optional:true, reloadOnChange:true
			//   - is NOT overwritten by builds or deployments
			//   - requires write access only on this one file (not appsettings.json / bin_dotnet)
			string targetPath = ResolveHardenedConfigPath();

			var overlay = new JObject(
				new JProperty("Server", new JObject(
					new JProperty("Password", hashedSharedSecret),
					new JProperty("AllowLegacyPasswordAuthentication", allowLegacyPasswordAuthentication)
				))
			);

			try
			{
				string directoryPath = Path.GetDirectoryName(targetPath);
				if (!string.IsNullOrWhiteSpace(directoryPath))
					Directory.CreateDirectory(directoryPath);

				File.WriteAllText(targetPath, overlay.ToString(Formatting.Indented));
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new InvalidOperationException(
					$"Unable to write hardened authentication settings to '{targetPath}'. " +
					$"Ensure the IIS application pool identity has Modify permission on that file.", ex);
			}
			catch (IOException ex)
			{
				throw new InvalidOperationException(
					$"Unable to write hardened authentication settings to '{targetPath}'.", ex);
			}
		}

		private static string ResolveHardenedConfigPath()
		{
			// Prefer the server content root (parent of bin_dotnet), fall back to the running directory.
			string[] candidateDirs = new[]
			{
				AutoDiscoveryHelper.GetServerFilePath(),
				Directory.GetCurrentDirectory(),
				AppContext.BaseDirectory
			}
			.Where(d => !string.IsNullOrWhiteSpace(d))
			.Select(Path.GetFullPath)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

			// Pick the first directory that already has the file, or the first candidate if none do.
			string dir = candidateDirs.FirstOrDefault(d =>
				File.Exists(Path.Join(d, HardenedConfigFileName)))
				?? candidateDirs[0];

			return Path.Join(dir, HardenedConfigFileName);
		}
	}
}