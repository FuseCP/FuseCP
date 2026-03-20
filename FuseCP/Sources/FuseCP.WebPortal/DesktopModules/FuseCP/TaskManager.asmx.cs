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
using System.Data;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    /// <summary>
    /// Summary description for PresentationServices
    /// </summary>
    [WebService(Namespace = "http://smbsaas/fusecp/webportal")]
    [WebServiceBinding]
	[System.Web.Script.Services.ScriptService]
    public class TaskManager : System.Web.Services.WebService
    {
        [WebMethod]
        public BackgroundTask GetTask(string taskId)
        {
            BackgroundTask task = ES.Services.Tasks.GetTask(taskId);
            return task;
        }

        [WebMethod]
        public BackgroundTask GetTaskWithLogRecords(string taskId, DateTime startLogTime)
        {
            return ES.Services.Tasks.GetTaskWithLogRecords(taskId, startLogTime);
        }

        [WebMethod]
        public int GetTasksNumber()
        {
            return ES.Services.Tasks.GetTasksNumber();
        }

        [WebMethod]
        public BackgroundTask[] GetUserTasks(int userId)
        {
            return ES.Services.Tasks.GetUserTasks(userId);
        }

        [WebMethod]
        public BackgroundTask[] GetCompletedTasks()
        {
            return ES.Services.Tasks.GetUserCompletedTasks(PanelSecurity.LoggedUserId);
        }

        [WebMethod]
        public void SetTaskNotifyOnComplete(string taskId)
        {
            ES.Services.Tasks.SetTaskNotifyOnComplete(taskId);
        }
    }
}
