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

using Microsoft.Win32;
using FuseCP.Providers.FTP.CerberusFTP6Proxy;
using FuseCP.Server.Utils;
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FuseCP.Providers.FTP
{
    public class CerberusFTP6 : HostingServiceProviderBase, IFtpServer
    {
        #region Properties

        protected string AdminUsername
        {
            get { return ProviderSettings["AdminUsername"]; }
        }

        protected string AdminPassword
        {
            get { return ProviderSettings["AdminPassword"]; }
        }

        protected string ServiceUrl
        {
            get { return ProviderSettings["ServiceUrl"]; }
        }
        #endregion
        CerberusFTPService client = new CerberusFTPService() { RequireMtom = false, EnableDecompression = true };

        #region Main Methods
        public override bool IsInstalled()
        {
            string productName = null;
            string productVersion = null;

            RegistryKey HKLM = Registry.LocalMachine;

            RegistryKey key = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            String[] names = null;

            if (key != null)
            {
                names = key.GetSubKeyNames();

                foreach (string s in names)
                {
                    RegistryKey subkey = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + s);
                    if (subkey != null)
                        if (!String.IsNullOrEmpty((string)subkey.GetValue("DisplayName")))
                        {
                            productName = (string)subkey.GetValue("DisplayName");
                        }
                    if (productName != null && productName.Equals("Cerberus FTP Server"))
                    {
                        if (subkey != null)
                            productVersion = (string)subkey.GetValue("DisplayVersion");
                        break;
                    }
                }

                if (!String.IsNullOrEmpty(productVersion))
                {
                    string[] split = productVersion.Split(new char[] { '.' });
                    return Int32.Parse(split[0]) > 6;
                }
            }

            key = HKLM.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

            if (key == null)
            {
                return false;
            }

            names = key.GetSubKeyNames();

            foreach (string s in names)
            {
                RegistryKey subkey = HKLM.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" + s);
                if (subkey != null)
                    if (!String.IsNullOrEmpty((string)subkey.GetValue("DisplayName")))
                    {
                        productName = (string)subkey.GetValue("DisplayName");
                    }
                if (productName != null)
                    if (productName.Equals("Cerberus FTP Server"))
                    {
                        if (subkey != null) productVersion = (string)subkey.GetValue("DisplayVersion");
                        break;
                    }
            }

            if (!String.IsNullOrEmpty(productVersion))
            {
                string[] split = productVersion.Split(new[] { '.' });
                return Int32.Parse(split[0]) > 6;
            }

            return false;

        }

        private Credentials GetCredentials(string Username, string Password)
        {
            var credentials = new Credentials();
            credentials.user = Username;
            credentials.password = Password;

            return credentials;
        }

        private Credentials GetCredentials()
        {
            return GetCredentials(AdminUsername, AdminPassword);
        }

        private static byte[] StrToByteArray(string str)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        private int[] getSHA1FromPassword(string strPassword)
        {
            byte[] data = StrToByteArray(strPassword);
            byte[] shaResult;

            using (var sha = SHA1.Create())
            {
                shaResult = sha.ComputeHash(data);
            }

            int[] returnResult = new int[shaResult.Length];
            for (var i = 0; i < shaResult.Length; i++)
            {
                returnResult[i] = shaResult[i];
            }

            return returnResult;
        }
        #endregion

        #region Cerberus Methods
        public String[] GetUserList()
        {
            client.Url = ServiceUrl;
            var response = client.GetUserList(new GetUserListRequest());

            if (response != null)
            {
                if (response.result)
                {
                    return response.UserList;
                }
            }

            return null;
        }

        public bool GetUserInformation(string username, ref User account, ref string message)
        {
            client.Url = ServiceUrl;
            bool success = false;

            GetUserInformationRequest request = new GetUserInformationRequest();
            request.credentials = GetCredentials();

            request.userName = username;

            GetUserInformationResponse response = client.GetUserInformation(request);

            if (response != null)
            {
                success = response.result;
                account = response.UserInformation;
                message = response.message;
            }

            return success;
        }

        public bool AddUser(string username, string password, bool anonymous, bool disabled, string site, string path, bool allowUpload, bool allowDownload, bool allowRename, bool allowDelete, bool allowDirectoryCreation, ref string message)
        {
            client.Url = ServiceUrl;
            bool success = false;

            AddUserRequest request = new AddUserRequest();
            request.credentials = GetCredentials();

            request.User = new User();
            request.User.name = username;
            request.User.password = new Password();
            request.User.password.lastChange = DateTime.Now;
            request.User.password.noExpire = true;
            request.User.password.type = PasswordType.plain;
            request.User.password.value = password;

            request.User.maxLoginsAllowed = new UserPropertyInt();
            request.User.maxLoginsAllowed.valueSpecified = true;
            request.User.maxLoginsAllowed.value = 10;
            request.User.isSimpleDirectoryMode = new UserPropertyBool();
            request.User.isSimpleDirectoryMode.valueSpecified = true;
            request.User.isSimpleDirectoryMode.value = true;
            request.User.isAnonymous = new UserPropertyBool();
            request.User.isAnonymous.valueSpecified = true;
            request.User.isAnonymous.value = false;
            request.User.isDisabled = new UserPropertyBool();
            request.User.isDisabled.valueSpecified = true;
            request.User.isDisabled.value = false;

            request.User.rootList = new VirtualDirectory[1];
            request.User.rootList[0] = new VirtualDirectory();

            request.User.rootList[0].name = site;
            request.User.rootList[0].path = path;

            request.User.rootList[0].permissions = new DirectoryPermissions();

            request.User.rootList[0].permissions.allowListDir = true;
            request.User.rootList[0].permissions.allowListFile = true;
            request.User.rootList[0].permissions.allowUpload = allowUpload;
            request.User.rootList[0].permissions.allowDownload = allowDownload;
            request.User.rootList[0].permissions.allowDisplayHidden = false;
            request.User.rootList[0].permissions.allowRename = allowRename;
            request.User.rootList[0].permissions.allowDelete = allowDelete;
            request.User.rootList[0].permissions.allowDirectoryCreation = allowDirectoryCreation;


            AddUserResponse response = client.AddUser(request);

            if (response != null)
            {
                success = response.result;
                message = response.message;
            }

            return success;
        }

        public bool DeleteUser(string username)
        {
            client.Url = ServiceUrl;
            DeleteUserRequest request = new DeleteUserRequest();
            request.credentials = GetCredentials();

            request.name = username;

            DeleteUserResponse response = client.DeleteUser(request);
            if (response != null)
            {
                return response.result;
            }

            return false;
        }
        #endregion

        #region Interface Methods
        public virtual void ChangeSiteState(string siteId, ServerState state)
        {
            //not implemented
        }

        public virtual ServerState GetSiteState(string siteId)
        {
            // not implemented
            return ServerState.Started;
        }

        public virtual bool SiteExists(string siteId)
        {
            // not implemented
            return false;
        }

        public virtual FtpSite[] GetSites()
        {
            // not implemented
            return null;
        }

        public virtual FtpSite GetSite(string siteId)
        {
            // not implemented
            return null;
        }

        public virtual string CreateSite(FtpSite site)
        {
            return site.Name;
        }

        public virtual void UpdateSite(FtpSite site)
        {
            // not implemented
        }

        public virtual void DeleteSite(string siteId)
        {
            // not implemented
        }

        public virtual bool AccountExists(string accountName)
        {
            string message = "";
            User userAccount = null;
            GetUserInformation(accountName, ref userAccount, ref message);

            if (userAccount == null)
                return false;
            return false;
        }

        public FtpAccount[] GetAccounts()
        {
            string message = "";
            User userAccount = null;
            var cerberusAccounts = GetUserList();
            var ftpAccounts = new FtpAccount[cerberusAccounts.Length];
            var i = 0;

            bool success;
            foreach (var cerberusAccount in cerberusAccounts)
            {
                success = GetUserInformation(cerberusAccount, ref userAccount, ref message);
                if (userAccount.rootList[0] != null)
                {
                    ftpAccounts[i].CanRead = userAccount.rootList[0].permissions.allowDownload;
                    ftpAccounts[i].CanWrite = userAccount.rootList[0].permissions.allowUpload;
                    ftpAccounts[i].Enabled = !userAccount.isDisabled.value;
                    ftpAccounts[i].Folder = userAccount.rootList[0].path;
                    ftpAccounts[i].Name = userAccount.name;
                }
            }

            return ftpAccounts;
        }

        public FtpAccount GetAccount(string accountName)
        {
            string message = "";
            User userAccount = null;
            GetUserInformation(accountName, ref userAccount, ref message);
            var ftpAccount = new FtpAccount();

            if (userAccount != null && userAccount.rootList != null && userAccount.rootList[0] != null)
            {
                ftpAccount.CanRead = userAccount.rootList[0].permissions.allowDownload;
                ftpAccount.CanWrite = userAccount.rootList[0].permissions.allowUpload;
                ftpAccount.Enabled = !userAccount.isDisabled.value;
                ftpAccount.Folder = userAccount.rootList[0].path;
                ftpAccount.Name = userAccount.name;
            }

            return ftpAccount;
        }

        public void CreateAccount(FtpAccount account)
        {
            string message = "";
            AddUser(account.Name, account.Password, false, !account.Enabled, account.Name, account.Folder, account.CanWrite, account.CanRead, account.CanWrite, account.CanWrite, account.CanWrite, ref message);

        }

        public void UpdateAccount(FtpAccount account)
        {
            string message = "";
            DeleteUser(account.Name);
            AddUser(account.Name, account.Password, false, !account.Enabled, account.Name, account.Folder, account.CanWrite, account.CanRead, account.CanWrite, account.CanWrite, account.CanWrite, ref message);
        }

        public void DeleteAccount(string accountName)
        {
            DeleteUser(accountName);
        }
        #endregion

        public override ServiceProviderItemBandwidth[] GetServiceItemsBandwidth(ServiceProviderItem[] items, DateTime since)
        {
            ServiceProviderItemBandwidth[] itemsBandwidth = new ServiceProviderItemBandwidth[items.Length];

            //%ProgramData%\Cerberus LLC\Cerberus FTP Server\log

            // update items with diskspace
            for (int i = 0; i < items.Length; i++)
            {
                ServiceProviderItem item = items[i];

                // create new bandwidth object
                itemsBandwidth[i] = new ServiceProviderItemBandwidth();
                itemsBandwidth[i].ItemId = item.Id;
                itemsBandwidth[i].Days = new DailyStatistics[0];

                if (item is FtpAccount)
                {
                    try
                    {
                        // get daily statistics
                        itemsBandwidth[i].Days = Calculate(item.Name, since.AddDays(-1));
                    }
                    catch (Exception ex)
                    {
                        Log.WriteError(ex);
                    }
                }
            }
            return itemsBandwidth;
        }

        DailyStatistics[] Calculate(string ftpUserName, DateTime fromDateTime)
        {
            string cerberusLogPath = String.Format(@"{0}\Cerberus LLC\Cerberus FTP Server\log", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            string[] filesList = Directory.GetFiles(cerberusLogPath, "server.*.log");

            ArrayList days = new ArrayList();
            DateTime dateParsed;

            for (int i = 0; i < filesList.Length; i++)
            {
                if (DateTime.TryParseExact(filesList[i].Substring(filesList[i].LastIndexOf(".") + 1), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out dateParsed))
                {
                    if (dateParsed >= fromDateTime)
                    {
                        using (StreamReader sr = new StreamReader(filesList[i]))
                        {
                            long bytesReceived = 0;
                            long bytesSent = 0;
                            string line = "";

                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.StartsWith("[") && line.Contains("B sent)") && ftpUserName == (line.Substring(line.IndexOf("[", 31) + 1, line.IndexOf("]", line.IndexOf("[", 31)) - (line.IndexOf("[", 31)) - 1)))
                                {
                                    bytesSent = bytesSent + long.Parse(line.Substring(line.IndexOf("' (") + 3, line.IndexOf(" B ", line.IndexOf("' (")) - (line.IndexOf("' (") + 3)));
                                }
                                if (line.StartsWith("[") && line.Contains("B received)") && ftpUserName == (line.Substring(line.IndexOf("[", 31) + 1, line.IndexOf("]", line.IndexOf("[", 31)) - (line.IndexOf("[", 31)) - 1)))
                                {
                                    bytesReceived = bytesReceived + long.Parse(line.Substring(line.IndexOf("' (") + 3, line.IndexOf(" B ", line.IndexOf("' (")) - (line.IndexOf("' (") + 3)));
                                }
                            }

                            DailyStatistics dailyStats = new DailyStatistics();
                            dailyStats.Year = dateParsed.Year;
                            dailyStats.Month = dateParsed.Month;
                            dailyStats.Day = dateParsed.Day;
                            dailyStats.BytesSent = bytesSent;
                            dailyStats.BytesReceived = bytesReceived;

                            days.Add(dailyStats);
                        }
                    }
                }
            }
            return (DailyStatistics[])days.ToArray(typeof(DailyStatistics));
        }

    }
}
