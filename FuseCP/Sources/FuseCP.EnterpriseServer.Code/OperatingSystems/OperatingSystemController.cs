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
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using FuseCP.Server;
using OS = FuseCP.Server.Client;
using FuseCP.Server.Client;
using FuseCP.Providers;
using FuseCP.Providers.OS;
//using OS = FuseCP.Providers.OS;
using System.Collections;
using FuseCP.Providers.DomainLookup;
using FuseCP.Providers.DNS;
using System.Linq;
using FuseCP.Providers.Common;


namespace FuseCP.EnterpriseServer
{
    public class OperatingSystemController : ControllerBase, IImportController, IBackupController
    {
        private const int FILE_BUFFER_LENGTH = 5000000; // ~5MB
        public OperatingSystemController(ControllerBase provider) : base(provider) { }

        private OS.OperatingSystem GetOS(int serviceId)
        {
            OS.OperatingSystem os = new OS.OperatingSystem();
            ServiceProviderProxy.Init(os, serviceId);
            return os;
        }

        #region ODBC DSNs
        public DataSet GetRawOdbcSourcesPaged(int packageId,
            string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
        {
            return PackageController.GetRawPackageItemsPaged(packageId, ResourceGroups.Os, typeof(SystemDSN),
                true, filterColumn, filterValue, sortColumn, startRow, maximumRows);
        }

        public List<SystemDSN> GetOdbcSources(int packageId, bool recursive)
        {
            List<ServiceProviderItem> items = PackageController.GetPackageItemsByType(
                packageId, ResourceGroups.Os, typeof(SystemDSN), recursive);

            return items.ConvertAll<SystemDSN>(
                new Converter<ServiceProviderItem, SystemDSN>(ConvertItemToSystemDSN));
        }

        private SystemDSN ConvertItemToSystemDSN(ServiceProviderItem item)
        {
            return (SystemDSN)item;
        }

        public string[] GetInstalledOdbcDrivers(int packageId)
        {
            // load service item
            int serviceId = PackageController.GetPackageServiceId(packageId, ResourceGroups.Os);
            OS.OperatingSystem os = GetOS(serviceId);
            return os.GetInstalledOdbcDrivers();
        }

        public SystemDSN GetOdbcSource(int itemId)
        {
            // load meta item
            SystemDSN item = (SystemDSN)PackageController.GetPackageItem(itemId);

            // load service item
            OS.OperatingSystem os = GetOS(item.ServiceId);
            SystemDSN dsn = os.GetDSN(item.Name);

            // add common properties
            dsn.Id = item.Id;
            dsn.PackageId = item.PackageId;
            dsn.ServiceId = item.ServiceId;

            if(dsn.Driver == "MsAccess" || dsn.Driver == "MsAccess2010" || dsn.Driver == "Excel" || dsn.Driver == "Excel2010" || dsn.Driver == "Text")
                dsn.DatabaseName = FilesController.GetVirtualPackagePath(item.PackageId, dsn.DatabaseName);

            return dsn;
        }

        public int AddOdbcSource(SystemDSN item)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // check package
            int packageCheck = SecurityContext.CheckPackage(item.PackageId, DemandPackage.IsActive);
            if (packageCheck < 0) return packageCheck;

            // check quota
            QuotaValueInfo quota = PackageController.GetPackageQuota(item.PackageId, Quotas.OS_ODBC);
            if (quota.QuotaExhausted)
                return BusinessErrorCodes.ERROR_OS_DSN_RESOURCE_QUOTA_LIMIT;

            // check if mail resource is available
            int serviceId = PackageController.GetPackageServiceId(item.PackageId, ResourceGroups.Os);
            if (serviceId == 0)
                return BusinessErrorCodes.ERROR_OS_RESOURCE_UNAVAILABLE;

            // check package items
            if (PackageController.GetPackageItemByName(item.PackageId, item.Name, typeof(SystemDSN)) != null)
                return BusinessErrorCodes.ERROR_OS_DSN_PACKAGE_ITEM_EXISTS;

            // place log record
            TaskManager.StartTask("ODBC_DSN", "ADD", item.Name);

            try
            {
                // check service items
                OS.OperatingSystem os = GetOS(serviceId);
                if (os.GetDSN(item.Name) != null)
                    return BusinessErrorCodes.ERROR_OS_DSN_SERVICE_ITEM_EXISTS;

                string[] dbNameParts = item.DatabaseName.Split('|');
                string groupName = null;
                if (dbNameParts.Length > 1)
                {
                    item.DatabaseName = dbNameParts[0];
                    groupName = dbNameParts[1];
                }

                // get database server address
                item.DatabaseServer = GetDatabaseServerName(groupName, item.PackageId);

                if (item.Driver == "MsAccess" || item.Driver == "MsAccess2010" || item.Driver == "Excel" || item.Driver == "Excel2010" || item.Driver == "Text")
                    item.DatabaseName = FilesController.GetFullPackagePath(item.PackageId, item.DatabaseName);

                // add service item
                os.CreateDSN(item);

                // save item
                item.DatabasePassword = CryptoUtils.Encrypt(item.DatabasePassword);
                item.ServiceId = serviceId;
                int itemId = PackageController.AddPackageItem(item);

                TaskManager.ItemId = itemId;

                return itemId;
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }

        public int UpdateOdbcSource(SystemDSN item)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // load original meta item
            SystemDSN origItem = (SystemDSN)PackageController.GetPackageItem(item.Id);
            if (origItem == null)
                return BusinessErrorCodes.ERROR_OS_DSN_PACKAGE_ITEM_NOT_FOUND;

            // check package
            int packageCheck = SecurityContext.CheckPackage(origItem.PackageId, DemandPackage.IsActive);
            if (packageCheck < 0) return packageCheck;

            // place log record
            TaskManager.StartTask("ODBC_DSN", "UPDATE", origItem.Name, item.Id);

            try
            {
                // get service
                OS.OperatingSystem os = GetOS(origItem.ServiceId);

                // password
                item.Driver = origItem.Driver;
                item.Name = origItem.Name;

                if (item.DatabasePassword == "")
                    item.DatabasePassword = CryptoUtils.Decrypt(origItem.DatabasePassword);

                string[] dbNameParts = item.DatabaseName.Split('|');
                string groupName = null;
                if (dbNameParts.Length > 1)
                {
                    item.DatabaseName = dbNameParts[0];
                    groupName = dbNameParts[1];
                }

                // get database server address
                item.DatabaseServer = GetDatabaseServerName(groupName, item.PackageId);

                if (item.Driver == "MsAccess" || item.Driver == "MsAccess2010" || item.Driver == "Excel" || item.Driver == "Excel2010" || item.Driver == "Text")
                    item.DatabaseName = FilesController.GetFullPackagePath(origItem.PackageId, item.DatabaseName);

                // update service item
                os.UpdateDSN(item);

                // update meta item
                if (item.DatabasePassword != "")
                {
                    item.DatabasePassword = CryptoUtils.Encrypt(item.DatabasePassword);
                    PackageController.UpdatePackageItem(item);
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }

        private string GetDatabaseServerName(string groupName, int packageId)
        {
            int sqlServiceId = PackageController.GetPackageServiceId(packageId, groupName);
            if (sqlServiceId > 0)
            {
                StringDictionary sqlSettings = ServerController.GetServiceSettings(sqlServiceId);
                return sqlSettings["ExternalAddress"];
            }
            return "";
        }

        public int DeleteOdbcSource(int itemId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo);
            if (accountCheck < 0) return accountCheck;

            // load original meta item
            SystemDSN origItem = (SystemDSN)PackageController.GetPackageItem(itemId);
            if (origItem == null)
                return BusinessErrorCodes.ERROR_OS_DSN_PACKAGE_ITEM_NOT_FOUND;

            // place log record
            TaskManager.StartTask("ODBC_DSN", "DELETE", origItem.Name, itemId);

            try
            {
                // get service
                OS.OperatingSystem os = GetOS(origItem.ServiceId);

                // delete service item
                os.DeleteDSN(origItem.Name);

                // delete meta item
                PackageController.DeletePackageItem(origItem.Id);

                return 0;
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }
        #endregion

        private Server.Client.OperatingSystem GetServerService(int serverId)
        {
            Server.Client.OperatingSystem server = new Server.Client.OperatingSystem();
            // Look up the OS service registered on this server so ProviderType is included
            // in the SOAP header. Without it the FuseCP.Server falls back to OSInfo.Current
            // which requires the provider assembly to already be in the AppDomain (fragile
            // under .NET Core assembly loading rules).
            var osServices = ServerController.GetServicesByServerIdGroupName(serverId, ResourceGroups.Os);
            if (osServices != null && osServices.Count > 0)
                ServiceProviderProxy.Init(server, osServices[0].ServiceId);
            else
                ServiceProviderProxy.ServerInit(server, serverId);
            return server;
        }

        #region Terminal Services Sessions
        public TerminalSession[] GetTerminalServicesSessions(int serverId)
	    {
            return GetServerService(serverId).GetTerminalServicesSessions();
	    }

        public int CloseTerminalServicesSession(int serverId, int sessionId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
                | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // load server info
            ServerInfo server = ServerController.GetServerById(serverId, false);

            // place log record
            TaskManager.StartTask("SERVER", "RESET_TERMINAL_SESSION", sessionId, serverId);

            try
            {
                GetServerService(serverId).CloseTerminalServicesSession(sessionId);
                return 0;
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }
        #endregion

        #region OS Processes
        public OSProcess[] GetOSProcesses(int serverId)
        {
            return GetServerService(serverId).GetOSProcesses();
        }

        public int TerminateOSProcess(int serverId, int pid)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
                | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // load server info
            ServerInfo server = ServerController.GetServerById(serverId, false);

            // place log record
            TaskManager.StartTask("SERVER", "TERMINATE_SYSTEM_PROCESS", pid, serverId);

            try
            {
                GetServerService(serverId).TerminateOSProcess(pid);
                return 0;
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }
        #endregion

        #region Windows Services
        public OSService[] GetOSServices(int serverId)
        {
            return GetServerService(serverId).GetOSServices();
        }

        public int ChangeOSServiceStatus(int serverId, string id, OSServiceStatus status)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
                | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // load server info
            ServerInfo server = ServerController.GetServerById(serverId, false);

            // place log record
            TaskManager.StartTask("SERVER", "CHANGE_WINDOWS_SERVICE_STATUS", id, serverId);
            TaskManager.WriteParameter("New Status", status);

            try
            {
                GetServerService(serverId).ChangeOSServiceStatus(id, status);
                return 0;
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }

        // Check If FSRM Role services were installed
        public bool CheckFileServicesInstallation(int serviceId)
        {
            OS.OperatingSystem os = GetOS(serviceId);
            return os.CheckFileServicesInstallation();

        }

        #endregion

        #region Web Platform Installer

        public bool CheckLoadUserProfile(int serverId)
        {
            int serviceId = getWebServiceId(serverId);
            if (serviceId != -1)
            {
                return WebServerController.GetWebServer(serviceId).CheckLoadUserProfile();
            }

            return false;
        }

        private int getWebServiceId(int serverId)
        {
            DataSet dsServices = ServerController.GetRawServicesByServerId(serverId);

            int webGroup = -1;
            
            if (dsServices.Tables.Count < 1) return -1;

            foreach (DataRow r in dsServices.Tables[0].Rows)
            {
                if (r["GroupName"].ToString() == "Web")
                {
                    webGroup = (int)r["GroupID"];
                    break;
                }
            }

            if (webGroup == -1) return -1;

            foreach (DataRow r in dsServices.Tables[1].Rows)
            {
                if ((int)r["GroupID"] == webGroup)
                {
                    return (int)r["ServiceID"];
                }
            }

            return -1;
        }



        public void EnableLoadUserProfile(int serverId)
        {
            int serviceId = getWebServiceId(serverId);
            if (serviceId != -1)
            {
                WebServerController.GetWebServer(serviceId).EnableLoadUserProfile();
            }
        }
        #endregion

        #region Event Viewer
        public string[] GetLogNames(int serverId)
        {
            return GetServerService(serverId).GetLogNames();
        }

        public SystemLogEntry[] GetLogEntries(int serverId, string logName)
        {
            return GetServerService(serverId).GetLogEntries(logName);
        }

        public SystemLogEntriesPaged GetLogEntriesPaged(int serverId, string logName, int startRow, int maximumRows)
        {
            return GetServerService(serverId).GetLogEntriesPaged(logName, startRow, maximumRows);
        }

        public int ClearLog(int serverId, string logName)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
                | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            TaskManager.StartTask("SERVER", "CLEAR_EVENT_LOG", logName, serverId);

            try
            {
                GetServerService(serverId).ClearLog(logName);
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }

            return 0;
        }
        #endregion

        #region Server Reboot
        public int RebootSystem(int serverId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsAdmin
                | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // load server info
            ServerInfo server = ServerController.GetServerById(serverId, false);

            // place log record
            TaskManager.StartTask("SERVER", "REBOOT", serverId);

            try
            {
                GetServerService(serverId).RebootSystem();
                return 0;
            }
            catch (Exception ex)
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }
        #endregion

        #region Server informations
        public SystemResourceUsageInfo GetSystemResourceUsageInfoPackageId(int packageId)
        {
            PackageInfo package = PackageController.GetPackage(packageId);
            if (package == null)
            {
                throw new Exception("PACKAGE_NOT_FOUND");
            }
            return GetSystemResourceUsageInfo(package.ServerId);
        }

        public SystemResourceUsageInfo GetSystemResourceUsageInfo(int serverId)
        {
            return GetServerService(serverId).GetSystemResourceUsageInfo();
        }

        public SystemMemoryInfo GetSystemMemoryInfoPackageId(int packageId)
        {
            PackageInfo package = PackageController.GetPackage(packageId);
            if (package == null)
            {
                throw new Exception("PACKAGE_NOT_FOUND");
            }
            return GetSystemMemoryInfoInternal(package.ServerId);
        }
        public SystemMemoryInfo GetSystemMemoryInfo(int serverId)
        {
            return GetSystemMemoryInfoInternal(serverId);
        }
        private SystemMemoryInfo GetSystemMemoryInfoInternal(int serverId)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) 
                throw new Exception("The account dont have permission");

            return GetServerService(serverId).GetSystemMemoryInfo();
        }

        #endregion

        #region IImportController Members

        public List<string> GetImportableItems(int packageId, int itemTypeId, Type itemType,
			ResourceGroupInfo group)
        {
            List<string> items = new List<string>();

            // get service id
            int serviceId = PackageController.GetPackageServiceId(packageId, group.GroupName);
            if (serviceId == 0)
                return items;

            OS.OperatingSystem os = GetOS(serviceId);
            if (itemType == typeof(SystemDSN))
                items.AddRange(os.GetDSNNames());

            return items;
        }

        public void ImportItem(int packageId, int itemTypeId, Type itemType,
			ResourceGroupInfo group, string itemName)
        {
            // get service id
            int serviceId = PackageController.GetPackageServiceId(packageId, group.GroupName);
            if (serviceId == 0)
                return;

            if (itemType == typeof(SystemDSN))
            {
                // save DSN info
                SystemDSN dsn = new SystemDSN();
                dsn.Name = itemName;
                dsn.ServiceId = serviceId;
                dsn.PackageId = packageId;
                PackageController.AddPackageItem(dsn);
            }
        }

        #endregion

        #region IBackupController Members

        public int BackupItem(string tempFolder, System.Xml.XmlWriter writer, ServiceProviderItem item, ResourceGroupInfo group)
        {
            if (item is HomeFolder)
            {
                // backup home folder files
                string backupName = String.Format("SpaceFiles_{0}_{1}.zip",
                    item.Id, DateTime.Now.Ticks);
                /*
                // get the list of remote files
                List<SystemFile> files = FilesController.GetFiles(item.PackageId, "\\", true);

                string[] zipFiles = new string[files.Count];
                for(int i = 0; i < zipFiles.Length; i++)
                    zipFiles[i] = files[i].Name;
                                    List<String> list_zipfiles = new List<String>();
                foreach (SystemFile file_i in files)
                {
                    // Ignore fcpak and old wspak Backup Files in Backup Folder
                    if (!(file_i.Name.EndsWith(".fcpak")) && !(file_i.Name.EndsWith(".wspak"))) {
                        list_zipfiles.Add(file_i.Name);
                    }
                }
                string[] zipFiles = list_zipfiles.ToArray();
                */


                // zip remote files
                //FilesController.ZipFiles(item.PackageId, zipFiles, backupName);
                CreateBackupZip(item.PackageId, backupName);

                // download zipped file
                string localBackupPath = Path.Combine(tempFolder, backupName);

                byte[] buffer = null;
                FileStream stream = new FileStream(localBackupPath, FileMode.Create, FileAccess.Write);

                int offset = 0;
                long length = 0;
                do
                {
                    // read remote content
                    buffer = FilesController.GetFileBinaryChunk(item.PackageId, backupName, offset, FILE_BUFFER_LENGTH);

                    // write remote content
                    stream.Write(buffer, 0, buffer.Length);

                    length += buffer.Length;
                    offset += FILE_BUFFER_LENGTH;
                }
                while (buffer.Length == FILE_BUFFER_LENGTH);
                stream.Close();

				// delete zipped file
				if (FilesController.FileExists(item.PackageId, backupName))
					FilesController.DeleteFiles(item.PackageId, new string[] { backupName });

                // add file pointer
                BackupController.WriteFileElement(writer, "SpaceFiles", backupName, length);

                // store meta item
                XmlSerializer serializer = new XmlSerializer(typeof(HomeFolder));
                serializer.Serialize(writer, item);
            }
            else if (item is SystemDSN)
            {
                // backup ODBC DSN
                OS.OperatingSystem os = GetOS(item.ServiceId);

                // read DSN info
                SystemDSN itemDsn = item as SystemDSN;
                SystemDSN dsn = os.GetDSN(item.Name);
                dsn.DatabasePassword = itemDsn.DatabasePassword;

                XmlSerializer serializer = new XmlSerializer(typeof(SystemDSN));
                serializer.Serialize(writer, dsn);
            }

            return 0;
        }

        public int CreateBackupZip(int packageId, string archivePath)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // check package
            int packageCheck = SecurityContext.CheckPackage(packageId, DemandPackage.IsActive);
            if (packageCheck < 0) return packageCheck;

            // place log record
            TaskManager.StartTask("FILES", "ZIP_FILES", archivePath, packageId);


            try
            {

                OS.OperatingSystem os = FilesController.GetOS(packageId);
                string zipFilePath = FilesController.GetFullPackagePath(packageId, archivePath);
                string rootFolder = FilesController.GetFullPackagePath(packageId, "");
                os.CreateBackupZip(zipFilePath, rootFolder);

                return 0;
            }
            catch (Exception ex)
            {
                //Log and return a generic error rather than throwing an exception
                TaskManager.WriteError(ex);
                return BusinessErrorCodes.ERROR_FILE_GENERIC_LOGGED;
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }


        public int RestoreItem(string tempFolder, System.Xml.XmlNode itemNode, int itemId, Type itemType, string itemName, int packageId, int serviceId, ResourceGroupInfo group)
        {
            if (itemType == typeof(HomeFolder))
            {
                OS.OperatingSystem os = GetOS(serviceId);
                
                // extract meta item
                XmlSerializer serializer = new XmlSerializer(typeof(HomeFolder));
                HomeFolder homeFolder = (HomeFolder)serializer.Deserialize(
                    new XmlNodeReader(itemNode.SelectSingleNode("HomeFolder")));

                // create home folder if required
                if (!os.DirectoryExists(homeFolder.Name))
                {
                    os.CreatePackageFolder(homeFolder.Name);
                }

                // copy database backup to remote server
                XmlNode fileNode = itemNode.SelectSingleNode("File[@name='SpaceFiles']");
                string backupFileName = fileNode.Attributes["path"].Value;
                long backupFileLength = Int64.Parse(fileNode.Attributes["size"].Value);
                string localBackupFilePath = Path.Combine(tempFolder, backupFileName);

                if (new FileInfo(localBackupFilePath).Length != backupFileLength)
                    return -3;

                FileStream stream = new FileStream(localBackupFilePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[FILE_BUFFER_LENGTH];

                int readBytes = 0;
                long length = 0;
                string remoteBackupPath = Path.Combine(homeFolder.Name, backupFileName);
                do
                {
                    // read package file
                    readBytes = stream.Read(buffer, 0, FILE_BUFFER_LENGTH);
                    length += readBytes;

                    if (readBytes < FILE_BUFFER_LENGTH)
                        // resize buffer
                        Array.Resize<byte>(ref buffer, readBytes);

                    // write remote backup file
                    os.AppendFileBinaryContent(remoteBackupPath, buffer);
                }
                while (readBytes == FILE_BUFFER_LENGTH);
                stream.Close();

                // unzip files
                os.UnzipFiles(remoteBackupPath, homeFolder.Name);

				// delete archive
				if (os.FileExists(remoteBackupPath))
					os.DeleteFile(remoteBackupPath);

                // add meta-item if required
                if (PackageController.GetPackageItemByName(packageId, itemName, typeof(HomeFolder)) == null)
                {
                    homeFolder.PackageId = packageId;
                    homeFolder.ServiceId = serviceId;
                    PackageController.AddPackageItem(homeFolder);
                }
            }
            else if (itemType == typeof(SystemDSN))
            {
                OS.OperatingSystem os = GetOS(serviceId);

                // extract meta item
                XmlSerializer serializer = new XmlSerializer(typeof(SystemDSN));
                SystemDSN dsn = (SystemDSN)serializer.Deserialize(
                    new XmlNodeReader(itemNode.SelectSingleNode("SystemDSN")));

                // create DSN if required
                if (os.GetDSN(itemName) == null)
                {
                    dsn.DatabasePassword = CryptoUtils.Decrypt(dsn.DatabasePassword);
                    os.CreateDSN(dsn);

                    // restore password
                    dsn.DatabasePassword = CryptoUtils.Encrypt(dsn.DatabasePassword);
                }

                // add meta-item if required
                if (PackageController.GetPackageItemByName(packageId, itemName, typeof(SystemDSN)) == null)
                {
                    dsn.PackageId = packageId;
                    dsn.ServiceId = serviceId;
                    PackageController.AddPackageItem(dsn);
                }
            }

            return 0;
        }

      
       
        #endregion

    }
}
