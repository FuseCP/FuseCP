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
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using FuseCP.EnterpriseServer.Security;

namespace FuseCP.EnterpriseServer
{
	public class UserCacheEntry
	{
		public DateTime LastAccess;
		public string Password;
		public UserInfo User;
	}

	public class UserCache : ConcurrentDictionary<string, UserCacheEntry>
	{
		const int MaxEntries = 20;

		public new bool TryGetValue(string key, out UserCacheEntry entry)
		{
			entry = null;
			if (base.TryGetValue(key, out entry))
			{
				entry.LastAccess = DateTime.Now;
				return true;
			}
			return false;
		}

		public void AddOrUpdate(string key, UserInfo user, string password)
		{
			var entry = new UserCacheEntry
			{
				LastAccess = DateTime.Now,
				User = user,
				Password = password
			};
			base.AddOrUpdate(key, entry, (k, v) => entry);
			if (Count > MaxEntries)
			{
				Task.Run(() =>
				{
					var oldEntries = Values
						.OrderByDescending(e => e.LastAccess)
						.Skip(MaxEntries);
					foreach (var oldEntry in oldEntries) base.TryRemove(oldEntry.User.Username, out _);
				});
			}
		}
	}

	public class UsernamePasswordValidator
	{
		static UserCache Users = new UserCache();
		static ConcurrentDictionary<string, Task> GetUserTasks = new ConcurrentDictionary<string, Task>();

		private static bool IsLoopbackHost(string hostAddress)
		{
			if (string.IsNullOrWhiteSpace(hostAddress))
				return false;

			if (!System.Net.IPAddress.TryParse(hostAddress, out var ipAddress))
				return false;

			return System.Net.IPAddress.IsLoopback(ipAddress);
		}

		public static bool Validate(string username, string password)
		{
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

			using (var controller = new Controllers())
			{
				UserCacheEntry cachedUser;
				var hostAddress = Web.Services.Server.UserHostAddress;
				var bruteForce = new BruteForceProtectionService(controller);
				var shouldApplyIpPolicy = !IsLoopbackHost(hostAddress);

				if (shouldApplyIpPolicy && !string.IsNullOrEmpty(hostAddress) && bruteForce.IsIpBlocked(hostAddress))
				{
					controller.AuditLog.AddAuditLogWarningRecord("User", "Login", username, new string[] { "IP blocked by brute force protection", "IP: " + hostAddress });
					return false;
				}

				if (Users.TryGetValue(username, out cachedUser) && password == cachedUser.Password)
				{
					GetUserTasks.GetOrAdd(username, (username) => Task.Run(async () =>
					{
						await Task.Delay(3000);

						UserInfo user = controller.UserController.GetUserByUsernamePassword(username, password, hostAddress, false);
						if (user == null) Users.TryRemove(username, out cachedUser);
						else Users.AddOrUpdate(username, user, password);
						Task task;
						GetUserTasks.TryRemove(username, out task);
					}));

					controller.SecurityContext.SetThreadPrincipal(cachedUser.User);
					if (shouldApplyIpPolicy)
						bruteForce.RecordAttempt(hostAddress, username, BruteForceProtectionService.Layers.Api, succeeded: true);
					return true;
				}

				UserInfo user = controller.UserController.GetUserByUsernamePassword(username, password, hostAddress, true);

				if (user == null)
				{
					Users.TryRemove(username, out cachedUser);
					if (shouldApplyIpPolicy)
						bruteForce.RecordAttempt(hostAddress, username, BruteForceProtectionService.Layers.Api, succeeded: false);
					
					controller.AuditLog.AddAuditLogWarningRecord("User", "Login", username, new string[] { "Invalid username or password" });

					return false;
				}

				Users.AddOrUpdate(username, user, password);
				if (shouldApplyIpPolicy)
					bruteForce.RecordAttempt(hostAddress, username, BruteForceProtectionService.Layers.Api, succeeded: true);
				controller.SecurityContext.SetThreadPrincipal(user);
			}
			return true;
		}

		public static void Init() { FuseCP.Web.Services.UserNamePasswordValidator.ValidateEnterpriseServer = Validate; }
	}
}
