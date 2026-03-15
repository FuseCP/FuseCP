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
using System.Configuration;
using System.IO;
using FuseCP.WebDavPortal.WebConfigSections;

namespace FuseCP.WebDav.Core.Config
{
    internal static class WebDavConfigSectionResolver
    {
        public static WebDavExplorerConfigurationSettingsSection GetRequiredSection()
        {
            var section = ConfigurationManager.GetSection(WebDavExplorerConfigurationSettingsSection.SectionName)
                as WebDavExplorerConfigurationSettingsSection;

            if (section != null)
            {
                return section;
            }

            var configPath = FindWebConfigPath();
            if (string.IsNullOrEmpty(configPath))
            {
                throw new ConfigurationErrorsException(
                    "Unable to locate Web.config for webDavExplorerConfigurationSettings.");
            }

            var fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configPath
            };

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            section = configuration.GetSection(WebDavExplorerConfigurationSettingsSection.SectionName)
                as WebDavExplorerConfigurationSettingsSection;

            if (section == null)
            {
                throw new ConfigurationErrorsException(
                    $"Configuration section '{WebDavExplorerConfigurationSettingsSection.SectionName}' was not found in '{configPath}'.");
            }

            return section;
        }

        private static string FindWebConfigPath()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);

            while (current != null)
            {
                var candidate = Path.Combine(current.FullName, "Web.config");
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                current = current.Parent;
            }

            return null;
        }
    }
}
