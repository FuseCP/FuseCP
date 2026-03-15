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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using FuseCP.WebDavPortal.UI.Routes;

namespace FuseCP.WebDavPortal
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(IEndpointRouteBuilder endpoints)
        {
            #region Account

            endpoints.MapControllerRoute(
               name: AccountRouteNames.PhoneNumberIsAvailible,
               pattern: "account/profile/phone-number-availible",
               defaults: new { controller = "Account", action = "PhoneNumberIsAvailible" }
               );

            endpoints.MapControllerRoute(
                name: AccountRouteNames.UserProfile,
                pattern: "account/profile",
                defaults: new { controller = "Account", action = "UserProfile" }
                );

            endpoints.MapControllerRoute(
                name: AccountRouteNames.PasswordResetLogin,
                pattern: "account/password-reset/step-1",
                defaults: new { controller = "Account", action = "PasswordResetLogin" }
                );

            endpoints.MapControllerRoute(
                name: AccountRouteNames.PasswordResetPincodeSendOptions,
                pattern: "account/password-reset/step-2/{token}",
                defaults: new { controller = "Account", action = "PasswordResetPincodeSendOptions" }
                );

            endpoints.MapControllerRoute(
                name: AccountRouteNames.PasswordResetPincode,
                pattern: "account/password-reset/step-3/{token}",
                defaults: new { controller = "Account", action = "PasswordResetPincode" }
                );

            endpoints.MapControllerRoute(
                name: AccountRouteNames.PasswordResetFinalStep,
                pattern: "account/password-reset/step-final/{token}/{pincode?}",
                defaults: new { controller = "Account", action = "PasswordResetFinalStep" }
                );

            endpoints.MapControllerRoute(
                name: AccountRouteNames.PasswordResetSuccess,
                pattern: "account/password-reset/success",
                defaults: new { controller = "Account", action = "PasswordSuccessfullyChanged" }
                );

            endpoints.MapControllerRoute(
                name: AccountRouteNames.PasswordChange,
                pattern: "account/profile/password-change",
                defaults: new { controller = "Account", action = "PasswordChange" }
                );

            endpoints.MapControllerRoute(
                name: AccountRouteNames.Logout,
                pattern: "account/logout",
                defaults: new { controller = "Account", action = "Logout" }
                );

            endpoints.MapControllerRoute(
                name: AccountRouteNames.Login,
                pattern: "account/login",
                defaults: new { controller = "Account", action = "Login" }
                ); 

            #endregion

            #region Owa

            endpoints.MapControllerRoute(
                name: FileSystemRouteNames.ViewOfficeOnline,
                pattern: "office365/view/{org}/{*pathPart}",
                defaults:
                    new {controller = "FileSystem", action = "ViewOfficeDocument"}
                );

            endpoints.MapControllerRoute(
                name: FileSystemRouteNames.EditOfficeOnline,
                pattern: "office365/edit/{org}/{*pathPart}",
                defaults:
                    new {controller = "FileSystem", action = "EditOfficeDocument"}
                );

            #endregion

            #region Enterprise storage 

            endpoints.MapControllerRoute(
                name: FileSystemRouteNames.ItemExist,
                pattern: "storage/item-exist/{org}/{*pathPart}",
                defaults:
                    new { controller = "FileSystem", action = "ItemExist" }
                );

            endpoints.MapControllerRoute(
                name: FileSystemRouteNames.NewWebDavItem,
                pattern: "storage/new/{org}/{*pathPart}",
                defaults:
                    new { controller = "FileSystem", action = "NewWebDavItem" }
                );

            endpoints.MapControllerRoute(
                name: FileSystemRouteNames.NewFolder,
                pattern: "storage/new-folder/{org}/{*pathPart}",
                defaults: new { controller = "FileSystem", action = "NewFolder" }
                );

            endpoints.MapControllerRoute(
                    name: FileSystemRouteNames.SearchFiles,
                    pattern: "storage/search/{org}/{*pathPart}",
                    defaults: new { controller = "FileSystem", action = "SearchFiles" }
                    );

            endpoints.MapControllerRoute(
                    name: FileSystemRouteNames.SearchFilesContent,
                    pattern: "storage/ajax/search/{org}/{*pathPart}",
                    defaults: new { controller = "FileSystem", action = "SearchFilesContent" }
                    );

            endpoints.MapControllerRoute(
                    name: FileSystemRouteNames.ChangeWebDavViewType,
                    pattern: "storage/change-view-type/{viewType}",
                    defaults: new { controller = "FileSystem", action = "ChangeViewType" }
                    );

            endpoints.MapControllerRoute(
                    name: FileSystemRouteNames.DeleteFiles,
                    pattern: "storage/files-group-action/delete",
                    defaults: new { controller = "FileSystem", action = "DeleteFiles" }
                    );

            endpoints.MapControllerRoute(
                name: FileSystemRouteNames.UploadFile,
                pattern: "storage/upload-files/{org}/{*pathPart}",
                defaults: new { controller = "FileSystem", action = "UploadFiles" }
                );

            endpoints.MapControllerRoute(
                name: FileSystemRouteNames.DownloadFile,
                pattern: "storage/download-file/{org}/{*pathPart}",
                defaults: new { controller = "FileSystem", action = "DownloadFile" }
                );

            endpoints.MapControllerRoute(
                name: FileSystemRouteNames.ShowAdditionalContent,
                pattern: "storage/show-additional-content/{*path}",
                defaults: new { controller = "FileSystem", action = "ShowAdditionalContent" }
                );

            endpoints.MapControllerRoute(
                name: FileSystemRouteNames.ShowContentDetails,
                pattern: "storage/details/{org}/{*pathPart}",
                defaults: new { controller = "FileSystem", action = "GetContentDetails" }
                );

            endpoints.MapControllerRoute(
                name: FileSystemRouteNames.ShowContentPath,
                pattern: "{org}/{*pathPart}",
                defaults: new { controller = "FileSystem", action = "ShowContent" }
                ); 
            #endregion

            endpoints.MapControllerRoute(
                name: "Default",
                pattern: "{controller}/{action}",
                defaults: new { controller = "Account", action = "Login" }
            );
        }
    }
}
