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
using System.IO;
using AutoMapper;
using FuseCP.Providers.HostedSolution;
using FuseCP.WebDav.Core.Client;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Extensions;
using FuseCP.WebDavPortal.Constants;
using FuseCP.WebDavPortal.FileOperations;
using FuseCP.WebDavPortal.Models.Account;
using FuseCP.WebDavPortal.Models.FileSystem;

namespace FuseCP.WebDavPortal.Mapping.Profiles.Account
{
    public class UserProfileProfile : Profile
    {
        public UserProfileProfile()
        {
            CreateMap<OrganizationUser, UserProfile>()
                .ForMember(ti => ti.PrimaryEmailAddress, x => x.MapFrom(hi => hi.PrimaryEmailAddress))
                .ForMember(ti => ti.DisplayName, x => x.MapFrom(hi => hi.DisplayName))
                .ForMember(ti => ti.DisplayName, x => x.MapFrom(hi => hi.DisplayName))
                .ForMember(ti => ti.AccountName, x => x.MapFrom(hi => hi.AccountName))
                .ForMember(ti => ti.FirstName, x => x.MapFrom(hi => hi.FirstName))
                .ForMember(ti => ti.Initials, x => x.MapFrom(hi => hi.Initials))
                .ForMember(ti => ti.LastName, x => x.MapFrom(hi => hi.LastName))
                .ForMember(ti => ti.JobTitle, x => x.MapFrom(hi => hi.JobTitle))
                .ForMember(ti => ti.Company, x => x.MapFrom(hi => hi.Company))
                .ForMember(ti => ti.Department, x => x.MapFrom(hi => hi.Department))
                .ForMember(ti => ti.Office, x => x.MapFrom(hi => hi.Office))
                .ForMember(ti => ti.BusinessPhone, x => x.MapFrom(hi => hi.BusinessPhone))
                .ForMember(ti => ti.Fax, x => x.MapFrom(hi => hi.Fax))
                .ForMember(ti => ti.HomePhone, x => x.MapFrom(hi => hi.HomePhone))
                .ForMember(ti => ti.MobilePhone, x => x.MapFrom(hi => hi.MobilePhone))
                .ForMember(ti => ti.Pager, x => x.MapFrom(hi => hi.Pager))
                .ForMember(ti => ti.WebPage, x => x.MapFrom(hi => hi.WebPage))
                .ForMember(ti => ti.Address, x => x.MapFrom(hi => hi.Address))
                .ForMember(ti => ti.City, x => x.MapFrom(hi => hi.City))
                .ForMember(ti => ti.State, x => x.MapFrom(hi => hi.State))
                .ForMember(ti => ti.Zip, x => x.MapFrom(hi => hi.Zip))
                .ForMember(ti => ti.Country, x => x.MapFrom(hi => hi.Country))
                .ForMember(ti => ti.Notes, x => x.MapFrom(hi => hi.Notes))
                .ForMember(ti => ti.PasswordExpirationDateTime, x => x.MapFrom(hi => hi.PasswordExpirationDateTime))
                .ForMember(ti => ti.ExternalEmail, x => x.MapFrom(hi => hi.ExternalEmail));
        }
    }
}

