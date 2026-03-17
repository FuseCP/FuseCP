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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace FuseCP.Server
{
	public static class Program
	{
		private static bool providerResolverConfigured;
		private static Dictionary<string, string> providerAssemblyMap;

		public static void Main(string[] args)
		{
			//if (!Debugger.IsAttached) Debugger.Launch();
			ConfigureProviderAssemblyResolver();
			PasswordValidator.Init();
			FuseCP.Web.Services.StartupCore.Init(args);
			FuseCP.Server.Utils.Log.LogLevel = FuseCP.Web.Services.Configuration.TraceLevel;
		}

		private static void ConfigureProviderAssemblyResolver()
		{
			if (providerResolverConfigured)
				return;

			providerResolverConfigured = true;
			providerAssemblyMap = BuildProviderAssemblyMap();

			AssemblyLoadContext.Default.Resolving += (context, assemblyName) =>
			{
				if (assemblyName == null || string.IsNullOrEmpty(assemblyName.Name))
					return null;

				if (providerAssemblyMap.TryGetValue(assemblyName.Name, out var assemblyPath) && File.Exists(assemblyPath))
					return context.LoadFromAssemblyPath(assemblyPath);

				return null;
			};
		}

		private static Dictionary<string, string> BuildProviderAssemblyMap()
		{
			var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (var providerRoot in GetProviderProbeRoots())
			{
				if (!Directory.Exists(providerRoot))
					continue;

				foreach (var file in Directory.EnumerateFiles(providerRoot, "*.dll", SearchOption.AllDirectories))
				{
					var name = Path.GetFileNameWithoutExtension(file);
					if (!map.ContainsKey(name))
						map[name] = file;
				}
			}

			return map;
		}

		private static IEnumerable<string> GetProviderProbeRoots()
		{
			var baseDir = AppContext.BaseDirectory;
			yield return Path.Combine(baseDir, "Providers");
			yield return Path.GetFullPath(Path.Combine(baseDir, "..", "bin", "Providers"));
		}
	}
}

#endif
