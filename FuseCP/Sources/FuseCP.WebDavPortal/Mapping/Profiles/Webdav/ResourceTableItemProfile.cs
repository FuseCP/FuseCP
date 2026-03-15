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
using AutoMapper;
using FuseCP.WebDav.Core.Client;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Extensions;
using FuseCP.WebDavPortal.Constants;
using FuseCP.WebDavPortal.FileOperations;
using FuseCP.WebDavPortal.Models.FileSystem;

namespace FuseCP.WebDavPortal.Mapping.Profiles.Webdav
{
    public class ResourceTableItemProfile : Profile
    {
        public ResourceTableItemProfile()
        {
            var openerManager = new FileOpenerManager();

            CreateMap<WebDavResource, ResourceTableItemModel>()
                .ForMember(ti => ti.DisplayName, x => x.MapFrom(hi => hi.DisplayName.Trim('/')))
                .ForMember(ti => ti.Href, x => x.MapFrom(hi => hi.Href))
                .ForMember(ti => ti.Type, x => x.MapFrom(hi => hi.ItemType.GetDescription().ToLowerInvariant()))
                .ForMember(ti => ti.IconHref, x => x.MapFrom(hi => hi.ItemType == ItemType.Folder ? WebDavAppConfigManager.Instance.FileIcons.FolderPath.Trim('~') : WebDavAppConfigManager.Instance.FileIcons[Path.GetExtension(hi.DisplayName.Trim('/'))].Trim('~')))
                .ForMember(ti => ti.IsTargetBlank, x => x.MapFrom(hi => openerManager.GetIsTargetBlank(hi)))
                .ForMember(ti => ti.LastModified, x => x.MapFrom(hi => hi.LastModified))
                .ForMember(ti => ti.LastModifiedFormated, x => x.MapFrom(hi => hi.LastModified == DateTime.MinValue ? "--" : (new WebDavResource(null, hi)).LastModified.ToString(Formats.DateFormatWithTime)))

                .ForMember(ti => ti.Summary, x => x.MapFrom(hi => hi.Summary))
                .ForMember(ti => ti.IsRoot, x => x.MapFrom(hi => hi.IsRootItem))
                .ForMember(ti => ti.Size, x => x.MapFrom(hi => hi.ContentLength))
                .ForMember(ti => ti.Quota, x => x.MapFrom(hi => hi.AllocatedSpace))
                .ForMember(ti => ti.Url, x => x.Ignore())
                .ForMember(ti => ti.FolderUrlAbsoluteString, x => x.Ignore())
                .ForMember(ti => ti.FolderUrlLocalString, x => x.Ignore())
                .ForMember(ti => ti.FolderName, x => x.Ignore())
                .ForMember(ti => ti.IsFolder, x => x.MapFrom(hi => hi.ItemType == ItemType.Folder));
        }
    }
}
