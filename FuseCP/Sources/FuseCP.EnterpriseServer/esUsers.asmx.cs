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
using System.Linq;
using FuseCP.Web.Services;
using System.ComponentModel;
using FuseCP.EnterpriseServer.Security;

namespace FuseCP.EnterpriseServer
{
    /// <summary>
    /// Summary description for esApplicationsInstaller
    /// </summary>
    [WebService(Namespace = "http://smbsaas/fusecp/enterpriseserver")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [Policy("EnterpriseServerPolicy")]
    [ToolboxItem(false)]
    public class esUsers: WebService
    {
        private void EnsureServerAdminAccess()
        {
            if (SecurityContext.CheckAccount(DemandAccount.IsActive | DemandAccount.IsAdmin | DemandAccount.NotDemo) != 0)
                throw new Exception("This method could be called by serveradmin only.");
        }

        [WebMethod(Description = "Checks if the account with the specified username exists.")]
        public bool UserExists(string username)
        {
            return UserController.UserExists(username);
        }

        [WebMethod]
        public UserInfo GetUserById(int userId)
        {
            UserInfoInternal uinfo = UserController.GetUser(userId);
            return (uinfo != null) ? new UserInfo(uinfo) : null;
        }

        [WebMethod]
        public UserInfo GetUserByUsername(string username)
        {
            UserInfoInternal uinfo = UserController.GetUser(username);
            return (uinfo != null) ? new UserInfo(uinfo) : null;
        }

        [WebMethod]
        public List<UserInfo> GetUsers(int ownerId, bool recursive)
        {
            return UserController.GetUsers(ownerId, recursive);
        }

        [WebMethod]
        public void AddUserVLan(int userId, UserVlan vLan)
        {
            UserController.AddUserVLan(userId, vLan);
        }

        [WebMethod]
        public void DeleteUserVLan(int userId, ushort vLanId)
        {
            UserController.DeleteUserVLan(userId, vLanId);
        }

        [WebMethod]
        public DataSet GetRawUsers(int ownerId, bool recursive)
        {
            return UserController.GetRawUsers(ownerId, recursive);
        }

        [WebMethod]
        public DataSet GetUsersPaged(int userId, string filterColumn, string filterValue,
            int statusId, int roleId, string sortColumn, int startRow, int maximumRows)
        {
            return UserController.GetUsersPaged(userId, filterColumn, filterValue, statusId, roleId, sortColumn, startRow, maximumRows);
        }

        [WebMethod]
        public DataSet GetUsersPagedRecursive(int userId, string filterColumn, string filterValue,
            int statusId, int roleId, string sortColumn, int startRow, int maximumRows)
        {
            return UserController.GetUsersPagedRecursive(userId, filterColumn, filterValue, statusId, roleId, sortColumn, startRow, maximumRows);
        }

        [WebMethod]
        public DataSet GetUsersSummary(int userId)
        {
            return UserController.GetUsersSummary(userId);
        }

        [WebMethod]
        public DataSet GetUserDomainsPaged(int userId, string filterColumn, string filterValue,
            string sortColumn, int startRow, int maximumRows)
        {
            return UserController.GetUserDomainsPaged(userId, filterColumn, filterValue, sortColumn, startRow, maximumRows);
        }

        [WebMethod]
        public DataSet GetRawUserPeers(int userId)
        {
            return UserController.GetRawUserPeers(userId);
        }

        [WebMethod]
        public List<UserInfo> GetUserPeers(int userId)
        {
            return UserController.GetUserPeers(userId);
        }

        [WebMethod]
        public List<UserInfo> GetUserParents(int userId)
        {
            return UserController.GetUserParents(userId);
        }

        [WebMethod]
        public int AddUser(UserInfo user, bool sendLetter, string password, string[] notes)
        {
            return UserController.AddUser(user, sendLetter, password, notes);
        }

        [WebMethod]
        public int AddUserLiteral(
		    int ownerId,
		    int roleId,
		    int statusId,
		    bool isPeer,
		    bool isDemo,
		    string username,
		    string password,
		    string firstName,
		    string lastName,
		    string email,
		    string secondaryEmail,
		    string address,
		    string city,
		    string country,
		    string state,
		    string zip,
		    string primaryPhone,
		    string secondaryPhone,
		    string fax,
		    string instantMessenger,
            bool htmlMail,
		    string companyName,
		    bool ecommerceEnabled,
            bool sendLetter)
        {
            UserInfo user = new UserInfo();
            user.OwnerId = ownerId;
            user.RoleId = roleId;
            user.StatusId = statusId;
            user.IsPeer = isPeer;
            user.IsDemo = isDemo;
            user.Username = username;
//            user.Password = password;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.SecondaryEmail = secondaryEmail;
            user.Address = address;
            user.City = city;
            user.Country = country;
            user.State = state;
            user.Zip = zip;
            user.PrimaryPhone = primaryPhone;
            user.SecondaryPhone = secondaryPhone;
            user.Fax = fax;
            user.InstantMessenger = instantMessenger;
            user.HtmlMail = htmlMail;
            user.CompanyName = companyName;
            user.EcommerceEnabled = ecommerceEnabled;
            return UserController.AddUser(user, sendLetter, password);
        }

        [WebMethod]
        public int UpdateUserTask(string taskId, UserInfo user)
        {
            return UserController.UpdateUser(taskId, user);
        }

        [WebMethod]
        public int UpdateUserTaskAsynchronously(string taskId, UserInfo user)
        {
            return UserController.UpdateUserAsync(taskId, user);
        }

        [WebMethod]
        public int UpdateUser(UserInfo user)
        {
            return UserController.UpdateUser(user);
        }

        [WebMethod]
        public int UpdateUserLiteral(int userId,
            int roleId,
            int statusId,
            bool isPeer,
            bool isDemo,
            string firstName,
            string lastName,
            string email,
            string secondaryEmail,
            string address,
            string city,
            string country,
            string state,
            string zip,
            string primaryPhone,
            string secondaryPhone,
            string fax,
            string instantMessenger,
            bool htmlMail,
            string companyName,
            bool ecommerceEnabled)
        {
            UserInfo user = new UserInfo();
            user.UserId = userId;
            user.RoleId = roleId;
            user.StatusId = statusId;
            user.IsPeer = isPeer;
            user.IsDemo = isDemo;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.SecondaryEmail = secondaryEmail;
            user.Address = address;
            user.City = city;
            user.Country = country;
            user.State = state;
            user.Zip = zip;
            user.PrimaryPhone = primaryPhone;
            user.SecondaryPhone = secondaryPhone;
            user.Fax = fax;
            user.InstantMessenger = instantMessenger;
            user.HtmlMail = htmlMail;
            user.CompanyName = companyName;
            user.EcommerceEnabled = ecommerceEnabled;

            return UserController.UpdateUser(user);
        }

        [WebMethod]
        public int DeleteUser(int userId)
        {
            return UserController.DeleteUser(userId);
        }

        [WebMethod]
        public int ChangeUserPassword(int userId, string password)
        {
            int res = UserController.ChangeUserPassword(userId, password);
            return res;
        }

        [WebMethod]
        public bool UpdateUserMfa(string username, bool activate)
        {
            return UserController.UpdateUserMfaSecret(username, activate);
        }

        [WebMethod]
        public bool CanUserChangeMfa(int changeUserId)
        {
            return UserController.CanUserChangeMfa(changeUserId);
        }

        [WebMethod]
        public string[] GetUserMfaQrCodeData(string username)
        {
            return UserController.GetUserMfaQrCodeData(username);
        }

        [WebMethod]
        public bool ActivateUserMfaQrCode(string username, string pin)
        {
            return UserController.ActivateUserMfaQrCode(username, pin);
        }

        [WebMethod]
        public int ChangeUserStatus(int userId, UserStatus status)
        {
            return UserController.ChangeUserStatus(userId, status);
        }

        [WebMethod]
        public List<BruteForceAttemptInfo> GetBruteForceAttempts(string ipAddress, string layer,
            bool failedOnly, int skip, int take)
        {
            EnsureServerAdminAccess();

            var service = new BruteForceProtectionService();
            return service.GetAttempts(ipAddress, layer, failedOnly, skip, take)
                .Select(a => new BruteForceAttemptInfo
                {
                    Id = a.Id,
                    IpAddress = a.IpAddress,
                    Username = a.Username,
                    Layer = a.Layer,
                    AttemptTime = a.AttemptTime,
                    Succeeded = a.Succeeded,
                    UserAgent = a.UserAgent
                })
                .ToList();
        }

        [WebMethod]
        public List<IpSecurityPolicyInfo> GetIpSecurityPolicies(bool whitelistOnly, bool blacklistOnly, bool includeInactive)
        {
            EnsureServerAdminAccess();

            var service = new BruteForceProtectionService();
            return service.GetPolicies(whitelistOnly, blacklistOnly, includeInactive)
                .Select(p => new IpSecurityPolicyInfo
                {
                    Id = p.Id,
                    IpRange = p.IpRange,
                    IsWhitelist = p.IsWhitelist,
                    CreatedDate = p.CreatedDate,
                    ExpiresDate = p.ExpiresDate,
                    Reason = p.Reason,
                    IsActive = p.IsActive,
                    SeverityLevel = p.SeverityLevel,
                    CreatedBy = p.CreatedBy
                })
                .ToList();
        }

        [WebMethod]
        public void BlockIpAddress(string ipAddress, string reason, int durationMinutes)
        {
            EnsureServerAdminAccess();

            var service = new BruteForceProtectionService();
            DateTime? expiry = durationMinutes > 0 ? DateTime.UtcNow.AddMinutes(durationMinutes) : null;
            service.BlockIp(ipAddress, reason, SecurityContext.User?.Identity?.Name, expiry);
        }

        [WebMethod]
        public void UnblockIpAddress(string ipAddress)
        {
            EnsureServerAdminAccess();

            var service = new BruteForceProtectionService();
            service.UnblockIp(ipAddress);
        }

        [WebMethod]
        public void RemoveIpSecurityPolicy(int policyId)
        {
            EnsureServerAdminAccess();

            var service = new BruteForceProtectionService();
            service.RemovePolicy(policyId);
        }

        [WebMethod]
        public void SetIpSecurityPolicyState(int policyId, bool isActive)
        {
            EnsureServerAdminAccess();

            var service = new BruteForceProtectionService();
            service.SetPolicyState(policyId, isActive);
        }

        [WebMethod]
        public void WhitelistIpAddress(string ipRange, string reason, int durationMinutes)
        {
            EnsureServerAdminAccess();

            var service = new BruteForceProtectionService();
            DateTime? expiry = durationMinutes > 0 ? DateTime.UtcNow.AddMinutes(durationMinutes) : null;
            service.WhitelistIp(ipRange, reason, SecurityContext.User?.Identity?.Name, expiry);
        }

        [WebMethod]
        public PasswordHardeningStatusInfo GetPasswordHardeningStatus()
        {
            EnsureServerAdminAccess();

            var service = new PasswordHardeningStatusService();
            return service.GetStatus();
        }

        [WebMethod]
        public List<LegacyPasswordUserInfo> GetLegacyPasswordUsers(int maxResults, string usernameFilter)
        {
            EnsureServerAdminAccess();

            var service = new PasswordHardeningStatusService();
            return service.GetLegacyUsers(maxResults, usernameFilter);
        }

		[WebMethod]
		public AutoHardenEligibleUsersResultInfo AutoHardenEligibleUserPasswords()
		{
			EnsureServerAdminAccess();

			var service = new PasswordHardeningStatusService();
			return service.AutoHardenEligibleUsers();
		}

        #region User Settings
        [WebMethod]
        public UserSettings GetUserSettings(int userId, string settingsName)
        {
            return UserController.GetUserSettings(userId, settingsName);
        }

        [WebMethod]
        public int UpdateUserSettings(UserSettings settings)
        {
            return UserController.UpdateUserSettings(settings);
        }

        [WebMethod]
        public DataSet GetUserThemeSettings(int userId)
        {
            return UserController.GetUserThemeSettings(userId);
        }

        [WebMethod]
        public void UpdateUserThemeSetting(int userId, string PropertyName, string PropertyValue)
        {
            UserController.UpdateUserThemeSetting(userId, PropertyName, PropertyValue);
        }

        [WebMethod]
        public void DeleteUserThemeSetting(int userId, string PropertyName)
        {
            UserController.DeleteUserThemeSetting(userId, PropertyName);
        }

        #endregion
    }
}
