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
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FuseCP.Server.Utils;
using FuseCP.WebDavPortal.Models.Common;
using FuseCP.WebDavPortal.Models.Common.Enums;

namespace FuseCP.WebDavPortal.Controllers
{
    public class BaseController : Controller
    {
        public const string MessagesKey = "messagesKey";

        public void AddMessage(MessageType type, string value)
        {
            Log.WriteStart("AddMessage");

            var messages = TempData[MessagesKey] as List<Message>;

            if (messages == null)
            {
                messages = new List<Message>();
            }

            messages.Add(new Message
            {
                Type = type,
                Value = value
            });

            TempData[MessagesKey] = messages;

            Log.WriteEnd("AddMessage");
        }
    }
}
