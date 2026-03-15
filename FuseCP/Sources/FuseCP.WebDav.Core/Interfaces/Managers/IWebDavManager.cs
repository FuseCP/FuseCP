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

using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using FuseCP.WebDav.Core.Client;

namespace FuseCP.WebDav.Core.Interfaces.Managers
{
    using UploadedFile = IFormFile;

    public interface IWebDavManager
    {
        IEnumerable<IHierarchyItem> OpenFolder(string path);
        bool IsFile(string path);
        bool FileExist(string path);
        byte[] GetFileBytes(string path);
        void UploadFile(string path, UploadedFile file);
        void UploadFile(string path, byte[] bytes);
        void UploadFile(string path, Stream stream);
        IEnumerable<IHierarchyItem> SearchFiles(int itemId, string pathPart, string searchValue, string uesrPrincipalName, bool recursive);
        IResource GetResource(string path);
        string GetFileUrl(string path);
        void DeleteResource(string path, bool deleteNonEmptyFolder);
        void LockFile(string path);
        string GetFileFolderPath(string path);
    }
}
