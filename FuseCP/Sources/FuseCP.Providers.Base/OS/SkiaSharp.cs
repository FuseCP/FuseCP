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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace FuseCP.Providers.OS
{
    public class SkiaSharp
    {
        static string GetAssemblyDirectory(Assembly assembly)
        {
            string codeBase = null;
            try
            {
                codeBase = assembly?.GetType().GetProperty("CodeBase")?.GetValue(assembly) as string;
            }
            catch
            {
            }

            if (!string.IsNullOrWhiteSpace(codeBase) && Uri.TryCreate(codeBase, UriKind.Absolute, out var codeBaseUri) && codeBaseUri.IsFile)
            {
                return Path.GetDirectoryName(codeBaseUri.LocalPath);
            }

            if (!string.IsNullOrWhiteSpace(assembly?.Location))
            {
                return Path.GetDirectoryName(assembly.Location);
            }

            return AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public bool IsLinuxMusl
        {
            get
            {
                if (!OSInfo.IsLinux) return false;
                return OS.Shell.Default.Exec("ldd /bin/ls").OutputAndError().Result.Contains("musl");
            }
        }

        static readonly SkiaSharp Current = new SkiaSharp(); 

        static Dictionary<string, IntPtr> loadedNativeDlls = new Dictionary<string, IntPtr>();
        public IntPtr SkiaDllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName.Contains("SkiaSharp"))
            {
                lock (this)
                {
                    IntPtr dll;
                    if (loadedNativeDlls.TryGetValue(libraryName, out dll)) return dll;

                    var runtimeInformation = typeof(RuntimeInformation);
                    var runtimeIdentifier = (string)runtimeInformation.GetProperty("RuntimeIdentifier")?.GetValue(null);
                    if (runtimeIdentifier == "linux-x64" && IsLinuxMusl) runtimeIdentifier = "linux-musl-x64";
                    runtimeIdentifier = runtimeIdentifier.Replace("linux-", "");
                    var currentDllPath = GetAssemblyDirectory(Assembly.Load("SkiaSharp"));
                    string libraryFileName = libraryName;
                    if (!libraryFileName.EndsWith(".so")) libraryFileName += ".so";
                    if (!libraryFileName.StartsWith("lib")) libraryFileName = "lib" + libraryFileName;
                    var nativeDllPath = Path.Combine(currentDllPath, runtimeIdentifier, libraryFileName);

                    if (File.Exists(nativeDllPath))
                    {
                        // call NativeLibrary.Load via reflection, because it's not available in NET Standard
                        var nativeLibrary = Type.GetType("System.Runtime.InteropServices.NativeLibrary, System.Runtime.InteropServices");
                        var load = nativeLibrary.GetMethod("Load", new Type[] { typeof(string), typeof(Assembly), typeof(DllImportSearchPath?) });
                        dll = (IntPtr)load?.Invoke(null, new object[] { nativeDllPath, assembly, searchPath });
                        loadedNativeDlls.Add(libraryName, dll);

                        Console.WriteLine($"Loaded native library: {nativeDllPath}");

                        return dll;
                    }
                }
            }

            // Otherwise, fallback to default import resolver.
            return IntPtr.Zero;
        }

        static bool nativeSkiaDllLoaded = false;
        public static void LoadNativeDlls()
        {
            if (nativeSkiaDllLoaded) return;
            nativeSkiaDllLoaded = true;

            if (OSInfo.IsLinux || OSInfo.IsMac)
            {
                // call NativeLibrary.SetDllImportResolver via reflection, becuase it's not available in NET Standard
                var nativeLibrary = Type.GetType("System.Runtime.InteropServices.NativeLibrary, System.Runtime.InteropServices");
                var dllImportResolver = Type.GetType("System.Runtime.InteropServices.DllImportResolver, System.Runtime.InteropServices");

                Assembly skiaSharp = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "SkiaSharp");
                if (skiaSharp == null)
                {
                    skiaSharp = Assembly.Load("SkiaSharp");
                }
                var setDllImportResolver = nativeLibrary.GetMethod("SetDllImportResolver", new Type[] { typeof(Assembly), dllImportResolver });
                //var importResolverMethod = this.GetType().GetMethod(nameof(SkiaDllImportResolver));

                var skiaDllImportResolver = Delegate.CreateDelegate(dllImportResolver, Current, nameof(SkiaDllImportResolver));
                setDllImportResolver?.Invoke(null, new object[] { skiaSharp, skiaDllImportResolver });

                Console.WriteLine("Added SkiaSharp DllImportResolver");
            }
        }
    }
}
