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
using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using FuseCP.WebDav.Core;
using FuseCP.WebDav.Core.Client;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDavPortal.Extensions;
using FuseCP.WebDavPortal.UI.Routes;

namespace FuseCP.WebDavPortal.FileOperations
{
    public class FileOpenerManager
    {
        private readonly IDictionary<string, FileOpenerType> _operationTypes = new Dictionary<string, FileOpenerType>();

        private readonly Lazy<IDictionary<string, FileOpenerType>> _officeOperationTypes = new Lazy<IDictionary<string, FileOpenerType>>(
            () =>
            {
                if (WebDavAppConfigManager.Instance.OfficeOnline.IsEnabled)
                {
                    return 
                        WebDavAppConfigManager.Instance.OfficeOnline.ToDictionary(x => x.Extension,
                            y => FileOpenerType.OfficeOnline);
                }

                return new Dictionary<string, FileOpenerType>();
            });

        public FileOpenerManager()
        {
            _operationTypes.AddRange(
                    WebDavAppConfigManager.Instance.FileOpener.ToDictionary(x => x.Extension,
                        y => FileOpenerType.Open));
        }

        public string GetUrl(IHierarchyItem item, IUrlHelper urlHelper)
        {
            var opener = this[Path.GetExtension(item.DisplayName)];
            string href = "/";

            switch (opener)
            {
                case FileOpenerType.OfficeOnline:
                {
                    var pathPart = item.Href.AbsolutePath.Replace("/" + ScpContext.User.OrganizationId, "").TrimStart('/');
                    href = string.Concat(urlHelper.RouteUrl(FileSystemRouteNames.EditOfficeOnline, new { org = ScpContext.User.OrganizationId, pathPart = "" }), pathPart);
                    break;
                }
                default:
                {
                    href = item.Href.LocalPath;
                    break;
                }
            }

            return href;
        }

        public bool GetIsTargetBlank(IHierarchyItem item)
        {
            var opener = this[Path.GetExtension(item.DisplayName)];
            var result = false;

            switch (opener)
            {
                case FileOpenerType.OfficeOnline:
                {
                    result = true;
                    break;
                }
                    case FileOpenerType.Open:
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public string GetMimeType(string extension)
        {
            var opener = WebDavAppConfigManager.Instance.FileOpener.FirstOrDefault(x => x.Extension.ToLowerInvariant() == extension.ToLowerInvariant());

            if (opener == null)
            {
                return MediaTypeNames.Application.Octet;
            }

            return opener.MimeType;
        }

        public FileOpenerType this[string fileExtension]
        {
            get
            {
                FileOpenerType result;
                if (_officeOperationTypes.Value.TryGetValue(fileExtension, out result) && CheckBrowserSupport())
                {
                    return result;
                }

                if (_operationTypes.TryGetValue(fileExtension, out result))
                {
                    return result;
                }

                return FileOpenerType.Download;
            }
        }

        private bool CheckBrowserSupport()
        {
            // Browser capability checks are moved to the web host layer during ASP.NET Core migration.
            return true;
        }
    }
}
