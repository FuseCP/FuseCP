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
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.Versioning;
using FuseCP.Providers.HostedSolution;

namespace FuseCP.Providers.Virtualization
{
    [SupportedOSPlatform("windows")]
    public class PowerShellManager : IDisposable
    {
        private readonly string _remoteComputerName;
        protected static InitialSessionState session = null;

        protected Runspace RunSpace { get; set; }

        public PowerShellManager(string remoteComputerName)
        {
            _remoteComputerName = remoteComputerName;
            OpenRunspace();
        }

        protected void OpenRunspace()
        {
            HostedSolutionLog.LogStart("OpenRunspace");

            if (session == null)
            {
                session = InitialSessionState.Create();
                session.ImportPSModule(new[] { "virtualmachinemanager" });
            }

            Runspace runSpace = RunspaceFactory.CreateRunspace(session);
            runSpace.Open();
            runSpace.SessionStateProxy.SetVariable("ConfirmPreference", "none");

            RunSpace = runSpace;
   
            HostedSolutionLog.LogEnd("OpenRunspace");
        }

        public void Dispose()
        {
            try
            {
                if (RunSpace != null && RunSpace.RunspaceStateInfo.State == RunspaceState.Opened)
                {
                    RunSpace.Close();
                    RunSpace = null;
                }
            }
            catch (Exception ex)
            {
                HostedSolutionLog.LogError("Runspace error", ex);
            }
        }

        public Collection<PSObject> Execute(Command cmd)
        {
            return Execute(cmd, true);
        }

        public Collection<PSObject> Execute(Command cmd, bool addComputerNameParameter)
        {
            return Execute(cmd, addComputerNameParameter, false);
        }

        public Collection<PSObject> Execute(Command cmd, bool addComputerNameParameter, bool withExceptions)
        {
            HostedSolutionLog.LogStart("Execute");

            List<object> errorList = new List<object>();

            HostedSolutionLog.DebugCommand(cmd);
            Collection<PSObject> results = null;

            // Add computerName parameter to command if it is remote server
            if (addComputerNameParameter)
            {
                if (!string.IsNullOrEmpty(_remoteComputerName))
                    cmd.Parameters.Add("ComputerName", _remoteComputerName);
            }

            // Create a pipeline
            Pipeline pipeLine = RunSpace.CreatePipeline();
            using (pipeLine)
            {
                // Add the command
                pipeLine.Commands.Add(cmd);
                // Execute the pipeline and save the objects returned.
                results = pipeLine.Invoke();

                // Only non-terminating errors are delivered here.
                // Terminating errors raise exceptions instead.
                // Log out any errors in the pipeline execution
                // NOTE: These errors are NOT thrown as exceptions! 
                // Be sure to check this to ensure that no errors 
                // happened while executing the command.
                if (pipeLine.Error != null && pipeLine.Error.Count > 0)
                {
                    foreach (object item in pipeLine.Error.ReadToEnd())
                    {
                        errorList.Add(item);
                        string errorMessage = string.Format("Invoke error: {0}", item);
                        HostedSolutionLog.LogWarning(errorMessage);
                    }
                }
            }
            pipeLine = null;

            if (withExceptions)
                ExceptionIfErrors(errorList);

            HostedSolutionLog.LogEnd("Execute");
            return results;
        }

        private static void ExceptionIfErrors(List<object> errors)
        {
            if (errors != null && errors.Count > 0)
                throw new Exception("Invoke error: " + string.Join("; ", errors.Select(e => e.ToString())));
        }
    }
}
