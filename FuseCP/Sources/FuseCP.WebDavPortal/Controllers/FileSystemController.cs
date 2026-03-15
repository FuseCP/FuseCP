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
using System.Net.Mime;
using System.Security.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using FuseCP.WebDav.Core;
using FuseCP.WebDav.Core.Client;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Entities.Account.Enums;
using FuseCP.WebDav.Core.Exceptions;
using FuseCP.WebDav.Core.Extensions;
using FuseCP.WebDav.Core.Interfaces.Managers;
using FuseCP.WebDav.Core.Interfaces.Managers.Users;
using FuseCP.WebDav.Core.Interfaces.Security;
using FuseCP.WebDav.Core.Security.Authorization.Enums;
using FuseCP.WebDav.Core.Security.Cryptography;
using FuseCP.WebDav.Core.Scp.Framework;
using FuseCP.WebDavPortal.CustomAttributes;
using FuseCP.WebDavPortal.Extensions;
using FuseCP.WebDavPortal.FileOperations;
using FuseCP.WebDavPortal.Helpers;
using FuseCP.WebDavPortal.Mapping;
using FuseCP.WebDavPortal.Models;
using System.Net;
using FuseCP.WebDavPortal.Models.Common;
using FuseCP.WebDavPortal.Models.Common.DataTable;
using FuseCP.WebDavPortal.Models.Common.Enums;
using FuseCP.WebDavPortal.Models.FileSystem;
using FuseCP.WebDavPortal.UI;
using FuseCP.WebDavPortal.UI.Routes;


namespace FuseCP.WebDavPortal.Controllers

{
    [LdapAuthorization]
    public class FileSystemController : BaseController
    {
        private readonly ICryptography _cryptography;
        private readonly IWebDavManager _webdavManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccessTokenManager _tokenManager;
        private readonly IWebDavAuthorizationService _webDavAuthorizationService;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly FileOpenerManager _openerManager;
        
        public FileSystemController(ICryptography cryptography, IWebDavManager webdavManager, IAuthenticationService authenticationService, IAccessTokenManager tokenManager, IWebDavAuthorizationService webDavAuthorizationService, FileOpenerManager openerManager, IUserSettingsManager userSettingsManager)
        {
            _cryptography = cryptography;
            _webdavManager = webdavManager;
            _authenticationService = authenticationService;
            _tokenManager = tokenManager;
            _webDavAuthorizationService = webDavAuthorizationService;
            _userSettingsManager = userSettingsManager;
                    
            _openerManager = new FileOpenerManager();
        }

        [HttpGet]
        public ActionResult ChangeViewType(FolderViewTypes viewType, string org, string pathPart = "")
        {
            _userSettingsManager.ChangeWebDavViewType(ScpContext.User.AccountId, viewType);

            return RedirectToRoute(FileSystemRouteNames.ShowContentPath, new  { org, pathPart });
        }

        public ActionResult ShowContent(string org, string pathPart = "", string searchValue = "")
        {
            if (org != ScpContext.User.OrganizationId)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }

            if (_webdavManager.IsFile(pathPart))
            {
                var resource = _webdavManager.GetResource(pathPart);

                var mimeType = _openerManager.GetMimeType(Path.GetExtension(pathPart));

                return new FileStreamResult(resource.GetReadStream(), mimeType);
            }

            try
            {
                var model = new ModelForWebDav
                {
                    UrlSuffix = pathPart, 
                    Permissions =_webDavAuthorizationService.GetPermissions(ScpContext.User, pathPart),
                    UserSettings = _userSettingsManager.GetUserSettings(ScpContext.User.AccountId),
                    SearchValue = searchValue
                };

                if (IsMobileDevice())
                {
                    model.UserSettings.WebDavViewType = FolderViewTypes.BigIcons;
                }

                return View(model);
            }
#pragma warning disable 0168
            catch (UnauthorizedException e)
#pragma warning restore 0168
            {
                return NotFound();
            }
        }

        public ActionResult ContentList(string org, ModelForWebDav model, string pathPart = "")
        {
            try
            {
                if (!IsMobileDevice() && model.UserSettings.WebDavViewType == FolderViewTypes.Table)
                {
                    return PartialView("_ShowContentTable", model);
                }

                IEnumerable<IHierarchyItem> children;

                if (string.IsNullOrEmpty(model.SearchValue))
                {
                    children = _webdavManager.OpenFolder(pathPart);
                }
                else
                {
                    children = _webdavManager.SearchFiles(ScpContext.User.ItemId, pathPart, model.SearchValue, ScpContext.User.Login, true);
                }

                model.Items = children.Take(WebDavAppConfigManager.Instance.ElementsRendering.DefaultCount);

                return PartialView("_ShowContentBigIcons", model);
            }
#pragma warning disable 0168
            catch (UnauthorizedException e)
#pragma warning disable 0168
            {
                return NotFound();
            }
        }

        [HttpGet]
        public ActionResult GetContentDetails(string org, string pathPart)
        {
            var dtRequest = BuildDataTableRequest();
            IEnumerable<WebDavResource> folderItems;

            if (string.IsNullOrEmpty(dtRequest.Search.Value) == false)
            {
                folderItems = _webdavManager.SearchFiles(ScpContext.User.ItemId, pathPart, dtRequest.Search.Value, ScpContext.User.Login, true).Cast<WebDavResource>();
            }
            else
            {
                folderItems = _webdavManager.OpenFolder(pathPart).Cast<WebDavResource>();
            }

            var tableItems = AutoMapperPortalConfiguration.Mapper.Map<IEnumerable<WebDavResource>, IEnumerable<ResourceTableItemModel>>(folderItems).ToList();

            FillContentModel(tableItems, org);

            var orders = dtRequest.Orders.ToList();
            orders.Insert(0, new JqueryDataTableOrder{Column = 3, Ascending = false});

            dtRequest.Orders = orders;

            var dataTableResponse = DataTableHelper.ProcessRequest(tableItems, dtRequest);

            return Json(dataTableResponse);
        }

        private JqueryDataTableRequest BuildDataTableRequest()
        {
            int ParseInt(string key, int defaultValue)
            {
                var value = GetRequestValue(key);
                return int.TryParse(value, out var parsed) ? parsed : defaultValue;
            }

            bool ParseBool(string key, bool defaultValue)
            {
                var value = GetRequestValue(key);
                return bool.TryParse(value, out var parsed) ? parsed : defaultValue;
            }

            var search = new JqueryDataTableSearch
            {
                Value = GetRequestValue("search[value]"),
                IsRegex = ParseBool("search[regex]", false)
            };

            var orders = new List<JqueryDataTableOrder>();
            for (var orderIndex = 0; !string.IsNullOrEmpty(GetRequestValue($"order[{orderIndex}][column]")); orderIndex++)
            {
                orders.Add(new JqueryDataTableOrder
                {
                    Column = ParseInt($"order[{orderIndex}][column]", 0),
                    Ascending = string.Equals(GetRequestValue($"order[{orderIndex}][dir]"), "asc", StringComparison.OrdinalIgnoreCase)
                });
            }

            var columns = new List<JqueryDataTableColumn>();
            for (var columnsIndex = 0; !string.IsNullOrEmpty(GetRequestValue($"columns[{columnsIndex}][name]")); columnsIndex++)
            {
                columns.Add(new JqueryDataTableColumn
                {
                    Data = GetRequestValue($"columns[{columnsIndex}][data]"),
                    Name = GetRequestValue($"columns[{columnsIndex}][name]"),
                    Orderable = ParseBool($"columns[{columnsIndex}][orderable]", true),
                    Search = new JqueryDataTableSearch
                    {
                        Value = GetRequestValue($"columns[{columnsIndex}][search][value]"),
                        IsRegex = ParseBool($"columns[{columnsIndex}][search][regex]", false)
                    }
                });
            }

            return new JqueryDataTableRequest
            {
                Draw = ParseInt("draw", 0),
                Start = ParseInt("start", 0),
                Count = ParseInt("length", 10),
                Search = search,
                Orders = orders,
                Columns = columns
            };
        }

        [HttpPost]
        public ActionResult ShowAdditionalContent(string path = "", int resourseRenderCount = 0)
        {
            path = path.Replace(ScpContext.User.OrganizationId, "").Trim('/');

            IEnumerable<IHierarchyItem> children = _webdavManager.OpenFolder(path);

            var result = children.Skip(resourseRenderCount).Take(WebDavAppConfigManager.Instance.ElementsRendering.AddElementsCount);

            return PartialView("_ResourseCollectionPartial", result);
        }

        public ActionResult SearchFiles(string org, string pathPart, string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return RedirectToRoute(FileSystemRouteNames.ShowContentPath);
            }

            var model = new ModelForWebDav
            {
                UrlSuffix = pathPart,
                Permissions = _webDavAuthorizationService.GetPermissions(ScpContext.User, pathPart),
                UserSettings = _userSettingsManager.GetUserSettings(ScpContext.User.AccountId),
                SearchValue = searchValue
            };

            return View("ShowContentSearchResultTable", model);
        }

        [HttpGet]
        public ActionResult DownloadFile(string org, string pathPart)
        {
            if (org != ScpContext.User.OrganizationId)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }

            string fileName = pathPart.Split('/').Last();

            if (_webdavManager.IsFile(pathPart) == false)
            {
                throw new Exception(Resources.UI.NotAFile);
            }

            var fileBytes = _webdavManager.GetFileBytes(pathPart);

            return File(fileBytes, MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet]
        public ActionResult UploadFiles(string org, string pathPart)
        {
            var model = new ModelForWebDav
            {
                UrlSuffix = pathPart
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("UploadFiles")]
        public ActionResult UploadFilePost(string org, string pathPart)
        {
            var uploadResults = new List<UploadFileResult>();

            foreach (var hpf in Request.Form.Files)
            {
                if (hpf == null || hpf.Length == 0)
                {
                    continue;
                }

                var destinationPath = string.Format("{0}/{1}", pathPart.TrimEnd('/'), Path.GetFileName(hpf.FileName));

                _webdavManager.UploadFile(destinationPath, hpf.OpenReadStream());

                uploadResults.Add(new UploadFileResult()
                {
                    name = hpf.FileName,
                    size = (int)hpf.Length,
                    type = hpf.ContentType
                });
            }

            var result = Json(new { files = uploadResults });

            //for IE8 which does not accept application/json
            var acceptHeader = Request.Headers["Accept"].ToString();
            if (!string.IsNullOrEmpty(acceptHeader) && !acceptHeader.Contains("application/json"))
            {
                result.ContentType = MediaTypeNames.Text.Plain;
            }

            return result;
        }

        [HttpPost]
        public JsonResult DeleteFiles(IEnumerable<string> filePathes = null, bool deleteNonEmptyFolder = false)
        {
            var model = new DeleteFilesModel();

            if (filePathes == null)
            {
                AddMessage(MessageType.Error, Resources.UI.NoFilesAreSelected);

                return Json(model);
            }

            foreach (var file in filePathes)
            {
                try
                {
                    _webdavManager.DeleteResource(Uri.UnescapeDataString(file), deleteNonEmptyFolder);

                    model.DeletedFiles.Add(file);
                }
                catch (WebDavException exception)
                {
                    if (exception.InnerException != null && !String.IsNullOrEmpty(exception.InnerException.Message))
                    {
                        model.AddMessage(MessageType.Error, exception.InnerException.Message);
                    }
                    else
                    {
                        model.AddMessage(MessageType.Error, exception.Message);
                    }
                }
            }

            if (model.DeletedFiles.Any())
            {
                model.AddMessage(MessageType.Success, string.Format(Resources.UI.ItemsWasRemovedFormat, model.DeletedFiles.Count));
            }

            return Json(model);
        }

        [HttpPost]
        public ActionResult NewFolder(string org, string pathPart)
        {
            string folderPath = pathPart + "/" + Request.Form["foldername"];
            var permissions = _webDavAuthorizationService.GetPermissions(ScpContext.User, pathPart);
            if (!permissions.HasFlag(WebDavPermissions.Write))
            {
                var model = new ErrorModel
                {
                    Message = "Permission denied"
                };
                return Json(model);
            }
            FCP.Services.EnterpriseStorage.CreateEnterpriseSubFolder(ScpContext.User.ItemId, folderPath);
            return new RedirectToRouteResult(FileSystemRouteNames.ShowContentPath, null);
        }

        public ActionResult NewWebDavItem(string org, string pathPart)
        {
            var permissions = _webDavAuthorizationService.GetPermissions(ScpContext.User, pathPart);

            var owaOpener = WebDavAppConfigManager.Instance.OfficeOnline.FirstOrDefault(x => x.Extension == Path.GetExtension(pathPart));

            if (permissions.HasFlag(WebDavPermissions.Write) == false || (owaOpener != null && permissions.HasFlag(WebDavPermissions.OwaEdit) == false))
            {
                return new RedirectToRouteResult(FileSystemRouteNames.ShowContentPath, null);
            }

            if (owaOpener != null)
            {
                return ShowOfficeDocument(org, pathPart, owaOpener.OwaNewFileView);
            }

            return new RedirectToRouteResult(FileSystemRouteNames.ShowContentPath, null);
        }

        [HttpPost]
        public JsonResult ItemExist(string org, string pathPart, string newItemName)
        {
            var exist = _webdavManager.FileExist(string.Format("{0}/{1}", pathPart.TrimEnd('/'), newItemName.Trim('/')));

            return Json(!exist);
        }

        #region Owa Actions

        public ActionResult ShowOfficeDocument(string org, string pathPart, string owaOpenerUri)
        {
            string fileUrl = WebDavAppConfigManager.Instance.WebdavRoot + org + "/" + pathPart.TrimStart('/');
            var accessToken = _tokenManager.CreateToken(ScpContext.User, pathPart);

            var urlPart = Url.RouteUrl(OwaRouteNames.CheckFileInfo, new { accessTokenId = accessToken.Id });
            var url = new Uri(new Uri(Request.GetDisplayUrl()), urlPart).ToString();

            string wopiSrc = Uri.UnescapeDataString(url);

            var uri = string.Format("{0}/{1}WOPISrc={2}&access_token={3}", WebDavAppConfigManager.Instance.OfficeOnline.Url, owaOpenerUri, Uri.EscapeDataString(wopiSrc), Uri.EscapeDataString(accessToken.AccessToken.ToString("N")));

            string fileName = fileUrl.Split('/').Last();
            string folder = pathPart.ReplaceLast(fileName, "").Trim('/');

            return View("ShowOfficeDocument", new OfficeOnlineModel(uri, fileName, folder));
        }

        public ActionResult ViewOfficeDocument(string org, string pathPart)
        {
            var owaOpener = WebDavAppConfigManager.Instance.OfficeOnline.Single(x => x.Extension == Path.GetExtension(pathPart));

            var owaOpenerUrl = IsMobileDevice() ? owaOpener.OwaMobileViev : owaOpener.OwaView;

            return ShowOfficeDocument(org, pathPart, owaOpenerUrl);
        }

        public ActionResult EditOfficeDocument(string org, string pathPart)
        {
            var permissions = _webDavAuthorizationService.GetPermissions(ScpContext.User, pathPart);

            if (permissions.HasFlag(WebDavPermissions.Write) == false || permissions.HasFlag(WebDavPermissions.OwaEdit) == false || IsMobileDevice())
            {
                return new RedirectToRouteResult(FileSystemRouteNames.ViewOfficeOnline, null);
            }

            var owaOpener = WebDavAppConfigManager.Instance.OfficeOnline.Single(x => x.Extension == Path.GetExtension(pathPart));

            return ShowOfficeDocument(org, pathPart, owaOpener.OwaEditor);
        }

        #endregion

        private void FillContentModel(IEnumerable<ResourceTableItemModel> items, string organizationId)
        {
            foreach (var item in items)
            {
                var opener = _openerManager[Path.GetExtension(item.DisplayName)];
                //var pathPart = item.Href.ToString().Replace("/" + ScpContext.User.OrganizationId, "").TrimStart('/');
                var pathPart = item.Href.ToStringPath().Replace("/" + ScpContext.User.OrganizationId, "").TrimStart('/');

                switch (opener)
                {
                    case FileOpenerType.OfficeOnline:
                    {
                        item.Url = string.Concat(Url.RouteUrl(FileSystemRouteNames.EditOfficeOnline, new {org = ScpContext.User.OrganizationId, pathPart = ""}), pathPart);
                        break;
                    }
                    default:
                    {
                        item.Url = item.Href.LocalPath;
                        break;
                    }
                }

                var folderPath = Uri.UnescapeDataString(_webdavManager.GetFileFolderPath(pathPart));

                var absoluteRouteUrl = Url.RouteUrl(FileSystemRouteNames.ShowContentPath, new { org = organizationId, pathPart = folderPath });
                item.FolderUrlAbsoluteString = Uri.UnescapeDataString(new Uri(new Uri(Request.GetDisplayUrl()), absoluteRouteUrl).ToString());
                item.FolderUrlLocalString = Url.RouteUrl(FileSystemRouteNames.ShowContentPath, new { org = organizationId, pathPart = folderPath });

                if (IsMobileDevice())
                {
                    item.IsTargetBlank = false;
                }
            }
        }

        private string GetRequestValue(string key)
        {
            if (Request.Query.TryGetValue(key, out var queryValue))
            {
                return queryValue.ToString();
            }

            if (Request.HasFormContentType && Request.Form.TryGetValue(key, out var formValue))
            {
                return formValue.ToString();
            }

            return string.Empty;
        }

        private bool IsMobileDevice()
        {
            var userAgent = Request.Headers.UserAgent.ToString().ToLowerInvariant();
            return userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone");
        }
    }
}
