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

using MySqlX.XDevAPI.Common;
using FuseCP.Providers.OS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if NETCOREAPP
using System.Runtime.Loader;
#endif

namespace FuseCP.UniversalInstaller;

#if NETCOREAPP
public class SetupAssemblyLoadContext : AssemblyLoadContext
{
	public object Loader { get; set; } = null;

	public SetupAssemblyLoadContext() : base("Setup Context", true) { }
	protected override Assembly Load(AssemblyName name)
	{
		if (name.Name == "netstandard" || name.Name == "System.Runtime" || name.Name == "System.Runtime.Loader" ||
			name.Name == "System.Collections") return null;

		lock (this)
		{
			if (Loader == null) Loader = AssemblyLoader.Init(Assemblies.Single());
		}
		
		var loadedAssembly = Assemblies.FirstOrDefault(a =>
		{
			var lname = a.GetName();
			return lname.Name == name.Name && lname.Version >= name.Version;
		});
		if (loadedAssembly != null)
			return loadedAssembly;

		var type = Loader?.GetType();
		var resolveAssembly = type?.GetMethod("ResolveAssembly", BindingFlags.Public | BindingFlags.Instance);
		return resolveAssembly?.Invoke(Loader, new object[] { this, name }) as Assembly;
		//return Loader?.ResolveAssembly(this, name);
	}
	protected override nint LoadUnmanagedDll(string unmanagedDllName)
	{
		var type = Loader?.GetType();
		var resolveAssembly = type?.GetMethod("ResolveNativeDll", BindingFlags.Public | BindingFlags.Instance);
		var result = resolveAssembly?.Invoke(Loader, new object[] { Assembly.GetExecutingAssembly(), unmanagedDllName });
		return (nint)(result ?? IntPtr.Zero);
		//return Loader?.ResolveNativeDll(Assembly.GetExecutingAssembly(), unmanagedDllName) ?? default(nint);
	}
}
#endif

// This class exists in FuseCP.UniversalInstaller, FuseCP.Setup and in
// FuseCP.UniversalInstaller.Runtime. It is responsible for loading the assemblies in the
// EmbeddedResources. The version in FuseCP.Setup is for NET Standard and relies on the
// version in FuseCP.UniversalInstaller.Runtime that is NetFX & NetCore specific.
public class AssemblyLoader
{
	public const string TmpFolder = "FuseCPInstallerAssemblyLoaderDlls";
	public const string NativeDllsFolder = "NativeDlls";
	public const string NativeDllsNetFXFolder = "NativeNetFXDlls";
	public const string NativeDllsNetCoreFolder = "NativeNetCoreDlls";
	public const string DllsFolder = "Dlls";
	public const string DllsNetFXFolder = "DllsNetFX";
	public const string DllsNetCoreFolder = "DllsNetCore";
	public const string DllsNetFXWinFolder = "DllsNetFXWin";
	public const string DllsNetCoreWinFolder = "DllsNetCoreWin";
	public const string DllsMonoUnixFolder = "DllsNetFXUnix";
	public const string DllsNetCoreUnixFolder = "DllsNetCoreUnix";
	public const string DllsUnixFolder = "DllsUnix";
	public const string DllsWinFolder = "DllsWin";
	public static AssemblyLoader Current { get; private set; } = null;
	public string AssembliesPath { get; set; } = null;

	public int NetVersion = 8;
	static string desktopRuntimePath = null;
	public string DesktopRuntimePath {
		get
		{
			if (desktopRuntimePath == null)
			{
				var dir = Directory.EnumerateDirectories(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
					"dotnet", "shared", "Microsoft.WindowsDesktop.App"))
					.Select(dir =>
					{
						Version version = default;
						Version.TryParse(Path.GetFileName(dir), out version);
						return new { Directory = dir, Version = version };
					})
					.Where(dir => dir.Version?.Major == NetVersion)
					.OrderByDescending(dir => dir.Version)
					.FirstOrDefault()
					?.Directory;
				desktopRuntimePath = dir;
			}
			return desktopRuntimePath;
		}
	}

	public object AssemblyLoadContext { get; set; } = null;
	public object LoaderRuntime { get; set; } = null;
	public bool IsDefault { get; set; } = false;
	public Assembly MainAssembly { get; set; }

	[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, uint dwFlags);
	public static bool IsCore => !(IsNetFX || IsNetNative);
	public static bool IsNetFX => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
	public static bool IsNetNative => RuntimeInformation.FrameworkDescription.StartsWith(".NET Native", StringComparison.OrdinalIgnoreCase);
	public static bool IsMono => Type.GetType("Mono.Runtime") != null;
	public static bool IsWindows => RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
	public static bool IsLinuxMusl
	{
		get
		{
			if (!IsLinux) return false;

			var info = new ProcessStartInfo("ldd");
			info.Arguments = "/bin/ls";
			info.RedirectStandardOutput = true;
			info.UseShellExecute = false;
			var p = Process.Start(info);
			return p.StandardOutput.ReadToEnd().Contains("musl");
		}
	}
	public static bool IsLinux => RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
	public static bool IsMac => RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);

	private static void Cleanup(string path)
	{
		// delete all old temp folders
		Task.Run(async () =>
		{
			await Task.Delay(TimeSpan.FromSeconds(5));
			var tmpFolder = Path.GetDirectoryName(path);
			var folders = Directory.EnumerateDirectories(tmpFolder)
				.Where(dir => dir != path);
			foreach (var folder in folders)
			{
				try
				{
					Directory.Delete(folder, true);
				}
				catch { }
			}
		});
	}

	public static AssemblyLoader Init(Assembly mainAssembly = null)
	{
		bool isSetup = mainAssembly != null;
		mainAssembly ??= Assembly.GetExecutingAssembly();

		var guid = Guid.NewGuid();
		var folder = guid.ToString("D");
		var path = Path.Combine(Path.GetTempPath(), TmpFolder, folder);
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		if (!isSetup) Cleanup(path);
		AppDomain.CurrentDomain.DomainUnload += (sender, args) =>
		{
			try
			{
				Directory.Delete(path, true);
			}
			catch { }
		};

		var loader = Current = new AssemblyLoader() { AssembliesPath = path, MainAssembly = mainAssembly };

#if Costura
		loader.SaveAssembliesToDisk();
#else
		if (isSetup) loader.SaveAssembliesToDisk();
#endif
		if (IsCore)
		{
			var ctxType = Type.GetType("System.Runtime.Loader.AssemblyLoadContext, System.Runtime.Loader");
			var getContext = ctxType?.GetMethod("GetLoadContext", BindingFlags.Public | BindingFlags.Static);
			var loadContext = getContext?.Invoke(null, new[] { mainAssembly });
			loader.AssemblyLoadContext = loadContext;

			var core = loader.ResolveAssembly(null, new AssemblyName("FuseCP.UniversalInstaller.Core"));
			var runtime = loader.ResolveAssembly(null, new AssemblyName($"FuseCP.UniversalInstaller.Runtime.{(IsCore ? "NetCore" : "NetFX")}"));

			if (runtime != null)
			{
				var loaderRuntimeType = runtime.GetType("FuseCP.UniversalInstaller.AssemblyLoader");
				if (loaderRuntimeType != null)
				{
					loader.LoaderRuntime = Activator.CreateInstance(loaderRuntimeType);
					var assCtx = loaderRuntimeType.GetMethod("InitAssemblyLoadContext");
					assCtx?.Invoke(loader.LoaderRuntime, new object[] { loader.AssembliesPath, loader.MainAssembly });
				}
			}
		} else
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => loader.ResolveAssembly(null, new AssemblyName(args.Name));
		}

		return loader;
	}

#if NETCOREAPP
	public void InitAssemblyLoadContext(string assembliesPath, Assembly mainAssembly)
	{
		Current = this;
		AssembliesPath = assembliesPath;
		MainAssembly = mainAssembly;
		var ctx = System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(mainAssembly);
		IsDefault = ctx == System.Runtime.Loader.AssemblyLoadContext.Default;
		if (IsDefault) {
			ctx.Resolving += (loadContext, name) => ResolveAssembly(loadContext, name);
			ctx.ResolvingUnmanagedDll += ResolveNativeDll;
		}
		else if (ctx is SetupAssemblyLoadContext setupCtx)
		{
			setupCtx.Loader = this;
			//setupCtx.ResolvingUnmanagedDll += ResolveNativeDll;
		}
		else if (ctx.GetType().Name == nameof(SetupAssemblyLoadContext))
		{
			var type = ctx.GetType();
			var loader = type.GetProperty("Loader", BindingFlags.Public | BindingFlags.Instance);
			loader.SetValue(ctx, this);
		}
		AssemblyLoadContext = ctx;
	}
	public string TryFile(string file)
	{
		var runtimeId = IsWindows ? "win-" :
			(IsMac ? "osx-" : IsLinux ? "linux-" : "");
		var arch = RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant();
		runtimeId += arch;
		file = Path.Combine(AssembliesPath, "runtimes", runtimeId, "native", file);
		if (File.Exists(file)) return file;
		file = Path.Combine(AssembliesPath, "runtimes", runtimeId, file);
		if (File.Exists(file)) return file;
		else return null;
	}
	public IntPtr ResolveNativeDll(Assembly assembly, string lib)
	{
		var dll = TryFile(lib) ??
			(IsWindows ? TryFile(lib + ".dll") : null) ??
			(IsLinux || IsMac ? (TryFile(lib + ".so") ?? TryFile($"lib{lib}.so")) : null) ??
			(IsMac ? TryFile(lib + ".dylib") ?? TryFile($"lib{lib}.dylib") : null);
		if (dll != null) return NativeLibrary.Load(dll);
		else return IntPtr.Zero;
	}
#endif

	public string ResolveAssemblyName(AssemblyName assemblyName, bool defaultContext)
	{
		var name = assemblyName.Name + ".dll";
		var culture = assemblyName.CultureInfo;
		string file;
		if (IsCore) {
			if (IsWindows)
			{
				file = Path.Combine(AssembliesPath, "runtimes", "win", "lib", "net10.0", name);
				if (File.Exists(file)) return file;
			}
			else if (IsMac || IsLinux)
			{
				if (IsMac)
				{
					file = Path.Combine(AssembliesPath, "runtimes", "osx", "lib", "net10.0", name);
					if (File.Exists(file)) return file;
				} else {
					file = Path.Combine(AssembliesPath, "runtimes", "linux", "lib", "net10.0", name);
					if (File.Exists(file)) return file;
				}
				file = Path.Combine(AssembliesPath, "runtimes", "unix", "lib", "net10.0", name);
				if (File.Exists(file)) return file;
			} else
			{
				file = Path.Combine(AssembliesPath, "runtimes", "unix", "lib", "net10.0", name);
				if (File.Exists(file)) return file;
			}
		}
		file = Path.Combine(AssembliesPath, name);
		if (File.Exists(file)) return file;
		if (culture != null && !culture.IsNeutralCulture)
		{
			file = Path.Combine(AssembliesPath, culture.TwoLetterISOLanguageName.ToLower(), name);
			if (File.Exists(file)) return file;
			file = Path.Combine(AssembliesPath, culture.Name.ToLower(), name);
			if (File.Exists(file)) return file;
		}

		if (IsCore && IsWindows && defaultContext && !string.IsNullOrWhiteSpace(DesktopRuntimePath))
		{
			file = Path.Combine(DesktopRuntimePath, name);
			if (File.Exists(file)) return file;
		}

		return null;
	}
	public Assembly ResolveAssembly(object loadContext, AssemblyName name)
	{
		Assembly assembly;
		if (loadContext == null) loadContext = AssemblyLoadContext;
#if NETCOREAPP
		var file = ResolveAssemblyName(name, loadContext == System.Runtime.Loader.AssemblyLoadContext.Default);
#else
		var file = ResolveAssemblyName(name, false);
#endif
		if (file != null)
		{
			if (loadContext == null) assembly = Assembly.LoadFrom(file);
			else
			{
#if NETCOREAPP
				var ctx = loadContext as AssemblyLoadContext;
				assembly = ctx.LoadFromAssemblyPath(file);
#elif NETSTANDARD
				var loadFromMethod = loadContext.GetType().GetMethod("LoadFromAssemblyPath");
				assembly = loadFromMethod.Invoke(AssemblyLoadContext, new[] { file }) as Assembly;
				
#else
				assembly = Assembly.LoadFrom(file);
#endif
			}
			//if (assembly != null) Console.WriteLine($"Loaded assembly {file}.");
			return assembly;
		}
		return null;
	}
	public void SaveToFile(Stream src, string filename)
	{
		var dir = Path.GetDirectoryName(filename);
		if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

		using (var file = new FileStream(filename, FileMode.Create, FileAccess.Write))
			src.CopyTo(file);
	}

	public HashSet<string> OmitAssembliesInNetCore = new()
	{
		"System.Memory.dll",
		"System.Threading.dll",
		"System.Threading.Tasks.dll",
		//"System.Diagnostics.DiagnosticSource.dll",
		"System.IO.FileSystem.AccessControl.dll",
		"System.Security.AccessControl.dll",
		"System.Security.Principal.Windows.dll",
		"System.Linq.dll",
		"Microsoft.Win32.Registry.dll"
	};
	public void SaveAssembliesToDisk()
	{
		var resourceNames = new HashSet<string>();
		var asm = MainAssembly;
		using (StreamReader reader = new StreamReader(asm.GetManifestResourceStream("costura.metadata")))
		{
			var line = reader.ReadLine();
			while (line != null)
			{
				var tokens = line.Split('|');
				var resname = tokens[0];
				resourceNames.Add(resname);

				// Special assemblies to omit in NET Core
				if (!IsCore || !OmitAssembliesInNetCore.Contains(tokens[3]))
				{
					var filename = tokens[3].Replace('/', Path.DirectorySeparatorChar);
					filename = Path.Combine(AssembliesPath, filename);
					using (var resStream = asm.GetManifestResourceStream(resname))
					{
						if (resname.EndsWith(".compressed"))
						{
							using (var src = new DeflateStream(resStream, CompressionMode.Decompress))
								SaveToFile(src, filename);
						}
						else SaveToFile(resStream, filename);
					}
				}
				line = reader.ReadLine();
			}
		}

		var arch = RuntimeInformation.ProcessArchitecture.ToString().ToLower();
		if (arch == "x64" && IsLinuxMusl) arch = "musl-x64";

		foreach (var resname in asm.GetManifestResourceNames())
		{
			if (resname.StartsWith(NativeDllsFolder + "/") ||
				IsNetFX && resname.StartsWith(NativeDllsNetFXFolder + "/") ||
				IsCore && resname.StartsWith(NativeDllsNetCoreFolder + "/"))
			{
				var slash = resname.IndexOf('/');
				var name = resname.Substring(slash + 1)
					.Replace('/', Path.DirectorySeparatorChar)
					.Replace('\\', Path.DirectorySeparatorChar);
				if ((name.StartsWith($"{arch}{Path.DirectorySeparatorChar}") ||
					name.StartsWith($"runtimes{Path.DirectorySeparatorChar}") ||
					!name.Contains(Path.DirectorySeparatorChar)) &&
					IsWindows && name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
					IsMac && name.EndsWith(".dylib", StringComparison.OrdinalIgnoreCase) ||
					IsLinux && name.EndsWith(".so", StringComparison.OrdinalIgnoreCase))
				{
					if (name.StartsWith($"{arch}{Path.DirectorySeparatorChar}"))
					{
						slash = name.IndexOf(Path.DirectorySeparatorChar);
						name = name.Substring(slash + 1);
					}
					var filename = Path.Combine(AssembliesPath, name);
					using (var src = asm.GetManifestResourceStream(resname))
						SaveToFile(src, filename);
					/*if (IsCore)
					{
						//NativeLibrary.Load(filename);
						var nativeLibType = Type.GetType("System.Runtime.InteropServices.NativeLinbrary, System.Runtime.InteropServices");
						var load = nativeLibType.GetMethod("Load", BindingFlags.Static | BindingFlags.Public);
						load.Invoke(null, new[] { filename });
					}*/
				}
			} else if (resname.StartsWith(DllsFolder + "/") ||
				IsNetFX && resname.StartsWith(DllsNetFXFolder + "/") ||
				IsCore && resname.StartsWith(DllsNetCoreFolder + "/") ||
				IsNetFX && IsWindows && resname.StartsWith(DllsNetFXWinFolder + "/") ||
				IsCore && IsWindows && resname.StartsWith(DllsNetCoreWinFolder + "/") ||
				IsMono && !IsWindows && resname.StartsWith(DllsMonoUnixFolder + "/") ||
				IsCore && !IsWindows && resname.StartsWith(DllsNetCoreUnixFolder + "/") ||
				IsWindows && resname.StartsWith(DllsWinFolder + "/") ||
				!IsWindows && resname.StartsWith(DllsUnixFolder + "/"))
			{
				var slash = resname.IndexOf('/');
				var name = resname.Substring(slash + 1)
					.Replace('/', Path.DirectorySeparatorChar)
					.Replace('\\', Path.DirectorySeparatorChar);
				var filename = Path.Combine(AssembliesPath, name);
				using (var src = asm.GetManifestResourceStream(resname))
					SaveToFile(src, filename);
			}
		}

		var runtimeId = IsWindows ? "win-" :
			(IsMac ? "osx-" : IsLinux ? "linux-" : "");
		if (IsLinux && IsLinuxMusl) runtimeId += "musl-";
		var arc = RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant();
		runtimeId += arc;

		if (IsNetFX) AddEnvironmentPaths(AssembliesPath, Path.Combine(AssembliesPath, arch));
		else if (IsCore)
		{
			var path = Path.Combine(AssembliesPath, "runtimes", runtimeId, "native");
			AddEnvironmentPaths(path);
			if (IsLinux && IsLinuxMusl)
			{
				path = Path.Combine(AssembliesPath, "runtimes", runtimeId.Replace("-musl-", "-"), "native");
				AddEnvironmentPaths(path);
			}
		}

		CopyRuntimeLibFiles(runtimeId);
	}

	private void CopyRuntimeLibFiles(string runtimeId)
	{
		if (runtimeId.Contains('-')) CopyRuntimeLibFiles(runtimeId.Substring(0, runtimeId.IndexOf('-')));

		var runtimeLibPath = Path.Combine(AssembliesPath, "runtimes", runtimeId, "lib", "netstandard2.0");
		if (Directory.Exists(runtimeLibPath))
		{
			foreach (var src in Directory.EnumerateFiles(runtimeLibPath))
			{
				File.Copy(src, Path.Combine(AssembliesPath, Path.GetFileName(src)));
			}
		}
		if (IsLinux && IsLinuxMusl && runtimeId.Contains("-musl-"))
		{
			runtimeId = runtimeId.Replace("-musl-", "-");
			runtimeLibPath = Path.Combine(AssembliesPath, "runtimes", runtimeId, "lib", "netstandard2.0");
			if (Directory.Exists(runtimeLibPath))
			{
				foreach (var src in Directory.EnumerateFiles(runtimeLibPath))
				{
					File.Copy(src, Path.Combine(AssembliesPath, Path.GetFileName(src)));
				}
			}
		}
	}

	private void AddEnvironmentPaths(params IEnumerable<string> newpaths)
	{
		var path = new[] { Environment.GetEnvironmentVariable("PATH") ?? string.Empty };

		var newpath = string.Join(Path.PathSeparator.ToString(), path.Concat(newpaths));

		Environment.SetEnvironmentVariable("PATH", newpath);
	}

	public void Unload()
	{
		if (AssemblyLoadContext != null)
		{
			if (!IsDefault)
			{
				try
				{
					var unload = AssemblyLoadContext.GetType().GetMethod("Unload");
					unload.Invoke(AssemblyLoadContext, new object[0]);
				}
				catch { }
			}
			try
			{
				Directory.Delete(AssembliesPath, true);
			}
			catch { }
		}
	}

	public void CosturaInit()
	{
#if Costura
		CosturaUtility.Initialize();
#endif
	}
}

[Serializable]
public class RemoteRunner : MarshalByRefObject
{
	public object RemoteRun(string fileName, string typeName, string methodName, object[] parameters)
	{
		AssemblyLoader.Init(this.GetType().Assembly);
		return RemoteRunOnLoadContext(fileName, typeName, methodName, parameters);
	}
	public object RemoteRunOnLoadContext(string fileName, string typeName, string methodName, object[] parameters)
	{
		return Installer.Current.LoadContext.RemoteRun(fileName, typeName, methodName, parameters);
	}
}
