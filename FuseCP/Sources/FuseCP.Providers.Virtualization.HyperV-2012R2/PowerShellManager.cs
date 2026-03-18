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
using System.Text;
using FuseCP.Providers.HostedSolution;

namespace FuseCP.Providers.Virtualization
{
    [SupportedOSPlatform("windows")]
    public class PowerShellManager : IDisposable
    {
        private readonly string _remoteComputerName;
        protected static InitialSessionState session = null;
        object psLocker = new object();
        private bool isStatic = false;
        public bool IsStaticObj { get => isStatic; }
        public string RemoteComputerName { get => _remoteComputerName; }

        protected Runspace RunSpace { get; set; }

        //public PowerShellManager(string remoteComputerName)
        //{
        //    _remoteComputerName = remoteComputerName;
        //    OpenRunspace();
        //}
        ///<summary>
        ///Attention! If you want use PowerShell as JOBS (Async) you must create a static object!
        ///</summary>
        public PowerShellManager(string remoteComputerName, bool isStaticObj)
        {
            isStatic = isStaticObj;
            _remoteComputerName = remoteComputerName;
            OpenRunspace();
        }

        protected void OpenRunspace()
        {
            HostedSolutionLog.LogStart("OpenRunspace");

            if (session == null)
            {
                session = InitialSessionState.Create();
                session.ImportPSModule(new[] {"Hyper-V"});
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
                    RunSpace.Dispose();
                    RunSpace = null;
                }
            }
            catch (Exception ex)
            {
                HostedSolutionLog.LogError("Runspace error", ex);
            }
        }

        /// <summary>
        /// Executes the specified PowerShell command against the provided virtual machine by automatically
        /// adding the "-VM" parameter.
        /// </summary>
        /// <remarks>
        /// Not all PowerShell cmdlets support the "-VM" parameter; please consult the official documentation
        /// for each cmdlet to verify compatibility before using this method.
        /// </remarks>
        public Collection<PSObject> ExecuteOnVm(Command cmd, VirtualMachineData vmData, bool withExceptions = false)
        {
            if (vmData.RawObject == null)
            {
                if (withExceptions)
                {
                    throw new NullReferenceException("VM rawObject is null! You must use GetVirtualMachine/GetVmPSObject method before using this method!");
                }
                else
                {
                    return null;
                }
            }

            cmd.Parameters.Add("VM", vmData.RawObject);
            return Execute(cmd, false, withExceptions); //False, because all remote connection information is already contained in RawObject
        }

        public Collection<PSObject> Execute(Command cmd, bool addComputerNameParameter)
        {
            return Execute(cmd, addComputerNameParameter, false);
        }

        public Collection<PSObject> Execute(IEnumerable<Command> cmd, bool addComputerNameParameter)
        {
            return ExecuteInternal(cmd, addComputerNameParameter, false);
        }

        public Collection<PSObject> Execute(Command cmd, bool addComputerNameParameter, bool withExceptions)
        {
            if (isStatic)
                throw new Exception("Invoke error: You can't execute this method from a static object!");

            return ExecuteInternal(cmd, addComputerNameParameter, withExceptions);
        }

        public Collection<PSObject> Execute(IEnumerable<Command> cmd, bool addComputerNameParameter, bool withExceptions)
        {
            return ExecuteInternal(cmd, addComputerNameParameter, withExceptions);
        }

        ///<summary>
        ///IMPORTANT: It is not recommended to use this method if the task/job can be implemented via WMI, 
        ///as PowerShell does not guarantee that the session will remain open, which may result in the loss of the Job ID.
        ///Not all commands support native asynchronous execution.
        ///Please manually verify that your command includes the -AsJob argument!
        ///</summary>
        public Collection<PSObject> TryExecuteAsJob(Command cmd, bool addComputerNameParameter)
        {
            Collection<PSObject> results = null;
            try
            {
                cmd.Parameters.Add("asJob");
                results = ExecuteFromStaticObj(cmd, addComputerNameParameter, true);
            }
            catch(Exception ex)
            {
                //TODO: Add the packing of the command as "asJob"?
                //HostedSolutionLog.LogWarning("This command doesn't support native Async, try it in another way (asJobScript)");     
                HostedSolutionLog.LogError("TryExecuteAsJob", ex);
                throw;
            }
            return results;
        }
        ///<summary>
        ///Some commands can not be sent as Job (or wrap as asJob), this method is for them.
        ///For example for command Get-Job from Static object
        ///</summary>
        public Collection<PSObject> ExecuteFromStaticObj(Command cmd, bool addComputerNameParameter, bool withExceptions)
        {
            if (!isStatic)
                throw new Exception("Invoke error: You can't execute this method from a Non static object!");

            bool lockTaken = false;
            int timeoutMs = 1000 * 60 * 10; //We need to wait until another thread finishes working with Powershell (It's probably better to use lock() :))
            Collection<PSObject> results = null;
            try
            {
                System.Threading.Monitor.TryEnter(psLocker, timeoutMs, ref lockTaken);
                if (!lockTaken)   { HostedSolutionLog.LogWarning("ExecuteFromStaticObj too long"); }
                results = ExecuteInternal(cmd, addComputerNameParameter, withExceptions);
            }
            finally
            {
                if (lockTaken)
                    System.Threading.Monitor.Exit(psLocker);
            }
            return results;
        }

        private Collection<PSObject> ExecuteInternal(Command commands, bool addComputerNameParameter, bool withExceptions)
        {
            return ExecuteInternal(new List<Command> { commands }, addComputerNameParameter, withExceptions);
        }

        private Collection<PSObject> ExecuteInternal(IEnumerable<Command> commands, bool addComputerNameParameter, bool withExceptions)
        {
            HostedSolutionLog.LogStart("Execute");

            List<object> errorList = new List<object>();

            Collection<PSObject> results = null;

            // Create a pipeline
            Pipeline pipeLine = RunSpace.CreatePipeline();
            using (pipeLine)
            {
                var commandsList = commands.ToList();

                if (addComputerNameParameter && !string.IsNullOrWhiteSpace(_remoteComputerName)) {
                    commandsList[0].Parameters.Add("ComputerName", _remoteComputerName.Trim());
                }
                foreach (Command cmd in commandsList)
                {
                    HostedSolutionLog.DebugCommand(cmd);
                    // Add the command
                    pipeLine.Commands.Add(cmd);
                }
                
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

        #region Jobs commands
        ///<summary>
        ///You can call this method only from a static object, or you will get an exception!
        ///</summary>
        public void ClearOldJobs()
        {
            try
            {
                ExecuteFromStaticObj(new Command("Stop-Job -State Blocked", true), false, true); //first we have to stop blocked jobs
                //int hours = 3;
                Command cmd = new Command("Get-Job | " +
                    "Where-Object { (($_.State -NE 'Running') -AND ($_.State -NE 'Blocked')) -AND ($_.PSEndTime.AddHours(3) -lt (Get-Date)) } " +
                    "| Remove-Job", true);

                ExecuteFromStaticObj(cmd, false, true);
            }
            catch (Exception ex)
            {
                HostedSolutionLog.LogError("ClearOldJobs", ex);
            }
            
        }

        ///<summary>
        ///You can call this method only from a static object, or you will get an exception!
        ///</summary>
        public Collection<PSObject> GetJobResult(string jobId)
        {
            Command cmd = new Command("Receive-Job");
            if (string.IsNullOrEmpty(jobId))
                throw new NullReferenceException("jobId is null");
            cmd.Parameters.Add("InstanceId", jobId);
            cmd.Parameters.Add("Keep");

            return ExecuteFromStaticObj(cmd, false, true);
        }

        ///<summary>
        ///You can call this method only from a static object, or you will get an exception!
        ///</summary>
        public Collection<PSObject> GetJob(string jobId)
        {
            //ClearOldJobs();
            Command cmd = new Command("Get-Job");

            if (string.IsNullOrEmpty(jobId))
                throw new NullReferenceException("jobId is null");
            cmd.Parameters.Add("InstanceId", jobId);

            return ExecuteFromStaticObj(cmd, false, true); //only for Get-Job or different commands, that can't be packed as Job
        }

        ///<summary>
        ///You can call this method only from a static object, or you will get an exception!
        ///</summary>
        public Collection<PSObject> GetJobs()
        {
            //ClearOldJobs();
            Command cmd = new Command("Get-Job");
            return ExecuteFromStaticObj(cmd, false, true);
        }
        #endregion

        //TODO: Command wrapping as "asJob". Find the best solution, if possible.
        #region PackAsJobScript
        private string PackAsJob(string cmd)
        {
            return "Start-Job -ScriptBlock {" + cmd + "}";
        }
        #endregion

        #region ConvertAsScript
        ///<summary>
        ///IMPORTANT! Does not support all kinds of commands (basically which accept objects), before using check that it converts correctly.
        ///</summary>
        public string ConvertCommandsAsScript(Collection<Command> cmds)
        {
            StringBuilder sb = new StringBuilder();
            string formatString = " {0} |";
            foreach (Command cmd in cmds)
            {
                sb.AppendFormat(formatString, ConvertCommandAsScript(cmd));
            }
            sb.Length--; //delete the last symbol - "|"            
            return sb.ToString();
        }

        ///<summary>
        ///IMPORTANT! Does not support all kinds of commands (basically which accept objects), before using check that it converts correctly.
        ///</summary>
        public string ConvertCommandAsScript(Command cmd)
        {
            StringBuilder sb = new StringBuilder(cmd.CommandText);
            foreach (CommandParameter parameter in cmd.Parameters)
            {
                string strParameterValues = null;
                string formatString = " -{0} {1}";
                if (parameter.Value is string)
                    formatString = " -{0} '{1}'";
                else if (parameter.Value is bool)
                    formatString = " -{0} ${1}";
                else if (parameter.Value is string[])
                {
                    formatString = " -{0} @({1})";
                    strParameterValues = string.Format("'{0}'", string.Join("','", (string[])parameter.Value)); // 'elm1', 'elm2', 'elm3' and etc.
                }
                //TODO: else if () Is possible another array type?

                if (string.IsNullOrEmpty(strParameterValues))
                    sb.AppendFormat(formatString, parameter.Name, parameter.Value);
                else
                    sb.AppendFormat(formatString, parameter.Name, strParameterValues);
            }
            return sb.ToString();
        }
        #endregion
    }
}
