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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using Cobalt;
using FuseCP.WebDav.Core.Client;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Interfaces.Managers;
using FuseCP.WebDav.Core.Interfaces.Owa;
using FuseCP.WebDav.Core.Interfaces.Storages;

namespace FuseCP.WebDav.Core.Owa
{
    public class CobaltSessionManager : IWopiFileManager
    {
        private readonly IWebDavManager _webDavManager;
        private readonly IAccessTokenManager _tokenManager;
        private readonly ITtlStorage _storage;

        public CobaltSessionManager(IWebDavManager webDavManager, IAccessTokenManager tokenManager, ITtlStorage storage)
        {
            _webDavManager = webDavManager;

            _tokenManager = tokenManager;
            _storage = storage;
        }

        public CobaltFile Create(int accessTokenId)
        {
            var disposal = new DisposalEscrow(accessTokenId.ToString(CultureInfo.InvariantCulture));

            var content = new CobaltFilePartitionConfig
            {
                IsNewFile = true,
                HostBlobStore = new TemporaryHostBlobStore(new TemporaryHostBlobStore.Config(), disposal, accessTokenId + @".Content"),
                cellSchemaIsGenericFda = true,
                CellStorageConfig = new CellStorageConfig(),
                Schema = CobaltFilePartition.Schema.ShreddedCobalt,
                PartitionId = FilePartitionId.Content
            };

            var coauth = new CobaltFilePartitionConfig
            {
                IsNewFile = true,
                HostBlobStore = new TemporaryHostBlobStore(new TemporaryHostBlobStore.Config(), disposal, accessTokenId + @".CoauthMetadata"),
                cellSchemaIsGenericFda = false,
                CellStorageConfig = new CellStorageConfig(),
                Schema = CobaltFilePartition.Schema.ShreddedCobalt,
                PartitionId = FilePartitionId.CoauthMetadata
            };

            var wacupdate = new CobaltFilePartitionConfig
            {
                IsNewFile = true,
                HostBlobStore = new TemporaryHostBlobStore(new TemporaryHostBlobStore.Config(), disposal, accessTokenId + @".WordWacUpdate"),
                cellSchemaIsGenericFda = false,
                CellStorageConfig = new CellStorageConfig(),
                Schema = CobaltFilePartition.Schema.ShreddedCobalt,
                PartitionId = FilePartitionId.WordWacUpdate
            };

            var partitionConfs = new Dictionary<FilePartitionId, CobaltFilePartitionConfig>
            {
                {FilePartitionId.Content, content},
                {FilePartitionId.WordWacUpdate, wacupdate},
                {FilePartitionId.CoauthMetadata, coauth}
            };

            var cobaltFile = new CobaltFile(disposal, partitionConfs, new CobaltHostLockingStore(), null);

            var token = _tokenManager.GetToken(accessTokenId);

            Atom atom;

            if (_webDavManager.FileExist(token.FilePath))
            {
                var fileBytes = _webDavManager.GetFileBytes(token.FilePath);

                atom = new AtomFromByteArray(fileBytes);
            }
            else
            {
                var filePath = ResolveContentRootPath(WebDavAppConfigManager.Instance.OfficeOnline.NewFilePath + Path.GetExtension(token.FilePath));

                atom = new AtomFromByteArray(File.ReadAllBytes(filePath));
            }

            Cobalt.Metrics o1;
            cobaltFile.GetCobaltFilePartition(FilePartitionId.Content).SetStream(RootId.Default.Value, atom, out o1);
            cobaltFile.GetCobaltFilePartition(FilePartitionId.Content).GetStream(RootId.Default.Value).Flush();

            Add(token.FilePath, cobaltFile);

            return cobaltFile;
        }

        public CobaltFile Get(string filePath)
        {
            return _storage.Get<CobaltFile>(GetSessionKey(filePath)); 
        }

        public bool Add(string filePath, CobaltFile file)
        {
            return _storage.Add(GetSessionKey(filePath), file);
        }

        public bool Delete(string filePath)
        {
            return _storage.Delete(GetSessionKey(filePath));
        }

        private string GetSessionKey(string filePath)
        {
            return string.Format("{0}/{1}", ScpContext.User.AccountId, filePath);
        }

        private static string ResolveContentRootPath(string relativePath)
        {
            var normalizedPath = relativePath
                .Replace('/', Path.DirectorySeparatorChar)
                .TrimStart('~', '/', '\\');

            return Path.Combine(AppContext.BaseDirectory, normalizedPath);
        }
    }
}
