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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FuseCP.Providers.OS;

namespace FuseCP.UniversalInstaller;
 
public class Program
{
	static AssemblyLoader Loader = null;
	static Program()
	{
		ShowWaitCursor();

		Loader = AssemblyLoader.Init();
	}

	public static void ImportDlls()
	{
#if NETCOREAPP
		// Import Mono.Posix.NETStandard
		_ = Mono.Unix.Native.FilePermissions.ACCESSPERMS;
#endif
		// Import FuseCP.Providers.Base
		_ = typeof(FuseCP.Providers.Common.BoolResult);
	}

	const string CancelFileName = "WaitCursor.cancel";
	static string CancelFile => Path.Combine(Environment.CurrentDirectory, CancelFileName);

	public static CancellationTokenSource CancelWaitCursor = new CancellationTokenSource();
	public static void ShowWaitCursor()
	{
		try
		{
			Console.CursorVisible = false;
			Console.Clear();
		}
		catch
		{
			// No console available (e.g., running as GUI app), skip visual feedback
			return;
		}

		var write = (string txt) =>
		{
			if (CancelWaitCursor.Token.IsCancellationRequested || File.Exists(CancelFile)) throw new Exception();
			Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
			Console.Write(txt);
			Thread.Sleep(333);
		};

		try
		{
			write("|");
		}
		catch
		{
			CancelWaitCursor = new CancellationTokenSource();
			if (File.Exists(CancelFile)) File.Delete(CancelFile);
		}

        Task.Run(() =>
		{
			try
			{
				while (true)
				{
					write("/");
					write("-");
					write("\\");
					write("|");
				}
			}
			catch
			{
				CancelWaitCursor = new CancellationTokenSource();
				if (File.Exists(CancelFile)) File.Delete(CancelFile);
			}
		});
	}

	public static void EndWaitCursor()
	{
		File.WriteAllText(CancelFile, "");
		CancelWaitCursor.Cancel();
	}


	[STAThread]
	public static void Main(string[] args)
	{
		try
		{
			ImportDlls();
			if (TryRunEmergencyMode(args))
				return;
			StartMain(args);
		} catch { }
	}

	private static bool TryRunEmergencyMode(string[] args)
	{
		string emergencyMode = GetArgValue(args, "-emergency=");
		if (string.IsNullOrWhiteSpace(emergencyMode))
			return false;

		if (emergencyMode.Equals("help", StringComparison.OrdinalIgnoreCase))
		{
			PrintEmergencyHelp();
			return true;
		}

		if (!emergencyMode.Equals("recover-server-credential", StringComparison.OrdinalIgnoreCase))
		{
			Console.WriteLine($"Unsupported emergency mode: {emergencyMode}");
			PrintEmergencyHelp();
			return true;
		}

		int exitCode = RunRecoverServerCredentialEmergency(args);
		Environment.ExitCode = exitCode;
		return true;
	}

	private static int RunRecoverServerCredentialEmergency(string[] args)
	{
		string serverId = GetArgValue(args, "-serverId=");
		string serverName = GetArgValue(args, "-serverName=");

		if (string.IsNullOrWhiteSpace(serverId) == string.IsNullOrWhiteSpace(serverName))
		{
			Console.WriteLine("Provide exactly one of -serverId=<id> or -serverName=<name>.");
			return 2;
		}

		string recoverScriptPath = ResolveRecoverScriptPath();
		if (string.IsNullOrWhiteSpace(recoverScriptPath))
		{
			Console.WriteLine("Recover-ServerCredential.ps1 not found.");
			Console.WriteLine("Expected locations include:\n - <current>\\FuseCP\\Tools\\Recover-ServerCredential.ps1\n - <installer>\\Tools\\Recover-ServerCredential.ps1");
			return 2;
		}

		string configPath = GetArgValue(args, "-config=");
		string mode = GetArgValue(args, "-mode=");
		string password = GetArgValue(args, "-password=");
		bool dryRun = HasArg(args, "-dryRun") || HasArg(args, "-dry-run");

		var parts = new List<string>
		{
			"-NoProfile",
			"-ExecutionPolicy", "Bypass",
			"-File", QuoteArg(recoverScriptPath)
		};

		if (!string.IsNullOrWhiteSpace(configPath))
		{
			parts.Add("-ConfigPath");
			parts.Add(QuoteArg(configPath));
		}

		if (!string.IsNullOrWhiteSpace(serverId))
		{
			parts.Add("-ServerId");
			parts.Add(serverId);
		}
		else
		{
			parts.Add("-ServerName");
			parts.Add(QuoteArg(serverName));
		}

		if (!string.IsNullOrWhiteSpace(mode))
		{
			parts.Add("-Mode");
			parts.Add(mode);
		}

		if (!string.IsNullOrWhiteSpace(password))
		{
			parts.Add("-Password");
			parts.Add(QuoteArg(password));
		}

		if (dryRun)
			parts.Add("-DryRun");

		Console.WriteLine("Emergency: Recover Server Credential");
		Console.WriteLine($"Using script: {recoverScriptPath}");
		if (string.IsNullOrWhiteSpace(password))
			Console.WriteLine("You will be prompted securely for the credential by PowerShell.");
		else
			Console.WriteLine("Warning: password was passed via command argument. Prefer interactive prompt for better secret hygiene.");

		string shell = ResolvePowerShellHost();
		if (string.IsNullOrWhiteSpace(shell))
		{
			Console.WriteLine("PowerShell host not found. Install pwsh or powershell and retry.");
			return 2;
		}

		var startInfo = new ProcessStartInfo
		{
			FileName = shell,
			Arguments = string.Join(" ", parts),
			UseShellExecute = false,
			RedirectStandardOutput = false,
			RedirectStandardError = false,
			RedirectStandardInput = false
		};

		using var process = Process.Start(startInfo);
		if (process == null)
		{
			Console.WriteLine("Failed to start emergency recovery process.");
			return 1;
		}

		process.WaitForExit();
		return process.ExitCode;
	}

	private static string ResolveRecoverScriptPath()
	{
		var baseDir = AppContext.BaseDirectory;
		var cwd = Environment.CurrentDirectory;

		string[] candidates =
		{
			Path.GetFullPath(Path.Combine(cwd, "FuseCP", "Tools", "Recover-ServerCredential.ps1")),
			Path.GetFullPath(Path.Combine(cwd, "Tools", "Recover-ServerCredential.ps1")),
			Path.GetFullPath(Path.Combine(baseDir, "Tools", "Recover-ServerCredential.ps1")),
			Path.GetFullPath(Path.Combine(baseDir, "Recover-ServerCredential.ps1"))
		};

		return candidates.FirstOrDefault(File.Exists) ?? string.Empty;
	}

	private static string ResolvePowerShellHost()
	{
		if (TryFindExecutableOnPath("pwsh"))
			return "pwsh";

		if (TryFindExecutableOnPath("powershell"))
			return "powershell";

		return string.Empty;
	}

	private static bool TryFindExecutableOnPath(string executable)
	{
		var path = Environment.GetEnvironmentVariable("PATH");
		if (string.IsNullOrWhiteSpace(path))
			return false;

		var searchPaths = path.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
		foreach (var searchPath in searchPaths)
		{
			try
			{
				var fullPath = Path.Combine(searchPath.Trim(), executable + (OSInfo.IsWindows ? ".exe" : string.Empty));
				if (File.Exists(fullPath))
					return true;
			}
			catch
			{
			}
		}

		return false;
	}

	private static string GetArgValue(string[] args, string prefix)
	{
		return args.FirstOrDefault(arg => arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
			?.Substring(prefix.Length)
			?.Trim();
	}

	private static bool HasArg(string[] args, string value)
	{
		return args.Any(arg => arg.Equals(value, StringComparison.OrdinalIgnoreCase));
	}

	private static string QuoteArg(string value)
	{
		if (string.IsNullOrEmpty(value))
			return "\"\"";
		return "\"" + value.Replace("\"", "\\\"") + "\"";
	}

	private static void PrintEmergencyHelp()
	{
		Console.WriteLine("Emergency modes:");
		Console.WriteLine("  -emergency=recover-server-credential -serverId=<id> [-mode=keep|sha256|sha1] [-config=<path>] [-dryRun] [-password=<secret>]");
		Console.WriteLine("  -emergency=recover-server-credential -serverName=<name> [-mode=keep|sha256|sha1] [-config=<path>] [-dryRun] [-password=<secret>]");
		Console.WriteLine("  -emergency=help");
	}

	public static void StartMain(string[] args)
	{
		try
		{
			if (args.Any(arg => arg.Equals("-ui=winforms", StringComparison.OrdinalIgnoreCase))) UI.Current = UI.WinFormsUI;
			else if (args.Any(arg => arg.Equals("-ui=avalonia", StringComparison.OrdinalIgnoreCase))) UI.Current = UI.AvaloniaUI;
			else if (args.Any(arg => arg.Equals("-ui=console", StringComparison.OrdinalIgnoreCase))) UI.Current = UI.ConsoleUI;

			var unattendedInstallPackages = args.Select(arg => Regex.Match(arg, @"(?<=-unattended=).*$"))
				.Where(match => match.Success)
				.Select(match => match.Value)
				.FirstOrDefault();
			var unattendedAction = args.Select(arg => Regex.Match(arg, @"(?<=-action=).*$"))
				.Where(match => match.Success)
				.Select(match => match.Value)
				.FirstOrDefault()
				?? "Install";

			if (unattendedInstallPackages != null)
			{
				Installer.Current.Settings.Installer.UnattendedInstallPackages = unattendedInstallPackages;
				SetupActions action;
				if (!Enum.TryParse<SetupActions>(unattendedAction, out action)) action = SetupActions.Install;
				Installer.Current.Settings.Installer.Action = action;
			}

			Installer.Current.UI.Init();

			if (args.Any(arg => arg.Equals("-update", StringComparison.OrdinalIgnoreCase)))
			{
				Installer.Current.UI.DownloadInstallerUpdate();
				return;
			}

			Installer.Current.OnExit += Loader.Unload;

			EndWaitCursor();
			try
			{
				Console.Clear();
			}
			catch
			{
				// Running without an attached console (GUI context)
			}

			Installer.Current.UI.PrintInstallerVersion();
			Installer.Current.StartMain();
		}
		catch (Exception ex) {
			Console.WriteLine(ex.ToString());
		}
	}
}

