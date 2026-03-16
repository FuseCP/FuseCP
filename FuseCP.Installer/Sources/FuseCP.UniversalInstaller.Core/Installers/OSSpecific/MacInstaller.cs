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

using FuseCP.Providers.OS;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace FuseCP.UniversalInstaller;

public class MacInstaller : UnixInstaller
{
	public override string UnixAppRootPath => "/var/bin";
	public override void InstallNet10Runtime()
	{
		if (CheckNet10RuntimeInstalled()) return;

		string tmp = null;

		if (OSInfo.Architecture == Architecture.X64) tmp = DownloadFile("https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.0/dotnet-sdk-10.0.0-osx-x64.pkg");
		else if (OSInfo.Architecture == Architecture.Arm64) tmp = DownloadFile("https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.0/dotnet-sdk-10.0.0-osx-arm64.pkg");
		else throw new PlatformNotSupportedException("Only x64 and Arm64 architectures supported.");
	
		Info("Installing .NET 10 Runtime...");

		Shell.Exec($"installer -pkg \"{tmp}\" -target /");
		Shell.Exec("brew update");
		//Shell.Exec("brew install mono-libgdiplus");

		Net10RuntimeInstalled = true;

		InstallLog("Installed .NET 10 Runtime.");

		ResetHasDotnet();
	}

	public override void RemoveNet10AspRuntime()
	{
		//throw new NotImplementedException();
	}
	public override void RemoveNet10NetRuntime()
	{
		//throw new NotImplementedException();
	}


    public virtual void OpenAppFirewall(string app)
    {
        Shell.Exec($"sudo /usr/libexec/ApplicationFirewall/socketfilterfw --add {app}");
        Shell.Exec($"sudo /usr/libexec/ApplicationFirewall/socketfilterfw --unblockapp {app}");
    }
    public virtual void CloseAppFirewall(string app)
    {
        Shell.Exec($"sudo /usr/libexec/ApplicationFirewall/socketfilterfw --blockapp {app}");
        //Shell.Exec($"sudo /usr/libexec/ApplicationFirewall/socketfilterfw --remove {app}");
    }

    public override void OpenFirewall(int port) { }
    public override void RemoveFirewallRule(int port) { }
    public override void InstallAspNetCoreSharedServer()
    {
        base.InstallAspNetCoreSharedServer();

        OpenAppFirewall("/var/bin/AspNetCoreSharedServer");
    }

    public override void AddUnixGroup(string group)
    {
        Shell.Exec($"dscl . create /Groups/{group}");
        Shell.Exec($"dscl . create /Groups/{group} RealName \"{group}\"");
        Shell.Exec($"dscl . create /Groups/{group} Password \"*\"");
        // Get free PrimaryGroupID
        var output = Shell.Exec($"dscl . list /Groups PrimaryGroupID").Output().Result;
        var maxid = Regex.Matches(output, @"(?<=^\s*[^ \t]+\s+)[0-9]+", RegexOptions.Multiline)
            .OfType<Match>()
            .Select(m =>
            {
                if (int.TryParse(m.Value, out int v)) return v;
                return -1;
            })
            .Max();
        Shell.Exec($"dscl . create /Groups/{group} PrimaryGroupID {maxid + 100}");
    }
    public override void AddUnixUser(string user, string group, string password)
    {
        Shell.Exec($"sysadminctl -addUser {user} -fullName \"{user}\" -password \"{password}\"");
        var groups = group.Split(',');
        foreach (var g in groups)
        {
            Shell.Exec($"dscl . -append /Groups/{g.Trim()} GroupMembership {user}");
        }
    }
}
