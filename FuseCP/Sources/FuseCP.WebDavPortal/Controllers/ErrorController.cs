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
using Microsoft.AspNetCore.Mvc;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDavPortal.Models;

namespace FuseCP.WebDavPortal.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(int statusCode, Exception exception, bool isAjaxRequet)
        {
            var model = new ErrorModel
            {
                HttpStatusCode = statusCode,
                Message = WebDavAppConfigManager.Instance.HttpErrors[statusCode],
                Exception = exception
            };
            
            Response.StatusCode = statusCode;

            if (!isAjaxRequet)
                return View(model);

            var errorObject = new { statusCode = model.HttpStatusCode, message = model.Message };
            return Json(errorObject);
        }
    }
}
