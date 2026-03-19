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

using System.ServiceModel;
using System.Collections.Concurrent;
using System.Globalization;
using FuseCP.Web.Services;
using FuseCP.Providers;

namespace FuseCP.Server
{
	public class PasswordValidator
	{
		static readonly ConcurrentDictionary<string, DateTimeOffset> NonceCache = new ConcurrentDictionary<string, DateTimeOffset>(StringComparer.Ordinal);

		public static bool Validate(string password) =>
			Settings.AllowLegacyPasswordAuthentication && password == Settings.Password;

		public static bool ValidateRequestAuthentication(ServerRequestAuthenticationData requestAuthentication)
		{
			if (requestAuthentication == null)
				return false;

			if (!string.Equals(requestAuthentication.Version, ServerRequestAuthentication.CurrentVersion, StringComparison.Ordinal))
				return false;

			if (string.IsNullOrEmpty(requestAuthentication.Timestamp) ||
				string.IsNullOrEmpty(requestAuthentication.Nonce) ||
				string.IsNullOrEmpty(requestAuthentication.Signature))
			{
				return false;
			}

			if (!long.TryParse(requestAuthentication.Timestamp, NumberStyles.Integer, CultureInfo.InvariantCulture, out long unixTimestamp))
				return false;

			DateTimeOffset timestamp;
			try
			{
				timestamp = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
			}
			catch (ArgumentOutOfRangeException)
			{
				return false;
			}

			var now = DateTimeOffset.UtcNow;
			if ((now - timestamp).Duration() > TimeSpan.FromSeconds(ServerRequestAuthentication.DefaultAllowedClockSkewSeconds))
				return false;

			PruneExpiredNonces(now);
			if (!NonceCache.TryAdd(requestAuthentication.Nonce, now))
				return false;

			var expectedSignature = ServerRequestAuthentication.ComputeSignature(
				Settings.Password,
				requestAuthentication.Action,
				requestAuthentication.Resource,
				requestAuthentication.Timestamp,
				requestAuthentication.Nonce,
				requestAuthentication.KeyId,
				requestAuthentication.ClusterId);

			if (!ServerRequestAuthentication.FixedTimeEquals(expectedSignature, requestAuthentication.Signature))
			{
				NonceCache.TryRemove(requestAuthentication.Nonce, out _);
				return false;
			}

			return true;
		}

		static void PruneExpiredNonces(DateTimeOffset now)
		{
			var expiryThreshold = now - TimeSpan.FromSeconds(ServerRequestAuthentication.DefaultAllowedClockSkewSeconds);
			foreach (var nonce in NonceCache)
			{
				if (nonce.Value < expiryThreshold)
					NonceCache.TryRemove(nonce.Key, out _);
			}
		}

		public static void Init()
		{
			FuseCP.Web.Services.UserNamePasswordValidator.ValidateServer = Validate;
			FuseCP.Web.Services.UserNamePasswordValidator.ValidateServerRequestAuthentication = ValidateRequestAuthentication;
		}

	}
}
