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
using System.Configuration;
using System.IO;

namespace FuseCP.AWStats.Viewer
{
	/// <summary>
	/// Summary description for ConfigFileAuthenticationProvider.
	/// </summary>
	public class ConfigFileAuthenticationProvider : AuthenticationProvider
	{
		public override AuthenticationResult AuthenticateUser(string domain, string username, string password)
		{
            string dataFolder = ConfigurationManager.AppSettings["AWStats.ConfigFileAuthenticationProvider.DataFolder"];
			if(dataFolder.StartsWith("~"))
				dataFolder = Path.Combine(AppContext.BaseDirectory, dataFolder.TrimStart('~', '/', '\\').Replace('/', Path.DirectorySeparatorChar));

            string awStatsScript = ConfigurationManager.AppSettings["AWStats.URL"];
			int idx = awStatsScript.LastIndexOf("/");
			awStatsScript = (idx == -1) ? awStatsScript : awStatsScript.Substring(idx + 1);

			string prefix = awStatsScript;
			int dotIdx = prefix.LastIndexOf(".");
			if(dotIdx != -1)
				prefix = prefix.Substring(0, dotIdx);

			string dataFile = Path.Combine(dataFolder, prefix + "." + domain + ".conf");
			if(!File.Exists(dataFile))
				return AuthenticationResult.DomainNotFound;

			string[] pairs = new string[0];

			// read file contents
			StreamReader reader = null;
			try
			{
				reader = new StreamReader(dataFile);
				string line;
				while((line = reader.ReadLine()) != null)
				{
					idx = line.IndexOf("=");
					if(idx == -1)
						continue;

					string key = line.Substring(0, idx).Trim();
					if(key.ToLower() == "siteusers")
					{
						pairs = line.Substring(idx + 1).Trim().Split(';');
						foreach(string pair in pairs)
						{
							string[] credentials = pair.Split('=');
							if(String.Compare(credentials[0], username, true) == 0)
							{
								// check password
								return (password == credentials[1]) ? AuthenticationResult.OK : AuthenticationResult.WrongPassword;
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Console.Error.WriteLine(ex.ToString());
			}
			finally
			{
				if(reader != null)
					reader.Close();
			}
			return AuthenticationResult.WrongUsername;
		}
	}
}
