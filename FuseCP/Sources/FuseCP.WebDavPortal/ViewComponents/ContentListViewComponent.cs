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

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FuseCP.WebDav.Core;
using FuseCP.WebDav.Core.Client;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Entities.Account.Enums;
using FuseCP.WebDav.Core.Interfaces.Managers;
using FuseCP.WebDavPortal.Models;

namespace FuseCP.WebDavPortal.ViewComponents
{
    public class ContentListViewComponent : ViewComponent
    {
        private readonly IWebDavManager _webdavManager;

        public ContentListViewComponent(IWebDavManager webdavManager)
        {
            _webdavManager = webdavManager;
        }

        public IViewComponentResult Invoke(ModelForWebDav model)
        {
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString().ToLowerInvariant();
            bool isMobile = userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone");

            if (!isMobile && model.UserSettings.WebDavViewType == FolderViewTypes.Table)
                return View("/Views/FileSystem/_ShowContentTable.cshtml", model);

            var pathPart = model.UrlSuffix ?? string.Empty;
            IEnumerable<IHierarchyItem> children;

            if (string.IsNullOrEmpty(model.SearchValue))
            {
                children = _webdavManager.OpenFolder(pathPart);
            }
            else
            {
                children = _webdavManager.SearchFiles(
                    ScpContext.User.ItemId, pathPart, model.SearchValue, ScpContext.User.Login, true);
            }

            model.Items = children.Take(WebDavAppConfigManager.Instance.ElementsRendering.DefaultCount);
            return View("/Views/FileSystem/_ShowContentBigIcons.cshtml", model);
        }
    }
}
