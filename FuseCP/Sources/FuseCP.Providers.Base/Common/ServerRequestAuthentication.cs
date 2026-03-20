// Copyright (C) 2026 FuseCP
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
using System.Security.Cryptography;
using System.Text;

namespace FuseCP.Providers
{
    public static class ServerRequestAuthentication
    {
        public const string VersionHeaderName = "X-FuseCP-Server-Auth-Version";
        public const string TimestampHeaderName = "X-FuseCP-Server-Auth-Timestamp";
        public const string NonceHeaderName = "X-FuseCP-Server-Auth-Nonce";
        public const string KeyIdHeaderName = "X-FuseCP-Server-Auth-KeyId";
        public const string ClusterIdHeaderName = "X-FuseCP-Server-Auth-ClusterId";
        public const string SignatureHeaderName = "X-FuseCP-Server-Auth-Signature";

        public const string CurrentVersion = "2";
        public const int DefaultAllowedClockSkewSeconds = 300;

        public static string CreateNonce()
        {
            byte[] bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return Convert.ToBase64String(bytes);
        }

        public static string BuildKeyId(string sharedSecret)
        {
            if (string.IsNullOrEmpty(sharedSecret))
                return "legacy-empty";

            return Cryptor.IsSHA256(sharedSecret) ? "legacy-sha256" : "legacy-sha1";
        }

        public static string BuildSigningString(string action, string resource, string timestamp, string nonce, string keyId, string clusterId)
        {
            return string.Join("\n", new[]
            {
                CurrentVersion,
                action ?? string.Empty,
                resource ?? string.Empty,
                timestamp ?? string.Empty,
                nonce ?? string.Empty,
                keyId ?? string.Empty,
                clusterId ?? string.Empty
            });
        }

        public static string ComputeSignature(string sharedSecret, string action, string resource, string timestamp, string nonce, string keyId, string clusterId)
        {
            if (string.IsNullOrEmpty(sharedSecret))
                return string.Empty;

            byte[] secretBytes = Encoding.UTF8.GetBytes(sharedSecret);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(BuildSigningString(action, resource, timestamp, nonce, keyId, clusterId));

            using (var hmac = new HMACSHA256(secretBytes))
            {
                return Convert.ToBase64String(hmac.ComputeHash(payloadBytes));
            }
        }

        public static bool FixedTimeEquals(string left, string right)
        {
            byte[] leftBytes = Encoding.UTF8.GetBytes(left ?? string.Empty);
            byte[] rightBytes = Encoding.UTF8.GetBytes(right ?? string.Empty);

            if (leftBytes.Length != rightBytes.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < leftBytes.Length; i++)
                diff |= leftBytes[i] ^ rightBytes[i];

            return diff == 0;
        }
    }

    public class ServerRequestAuthenticationData
    {
        public string Version { get; set; }
        public string Timestamp { get; set; }
        public string Nonce { get; set; }
        public string KeyId { get; set; }
        public string ClusterId { get; set; }
        public string Signature { get; set; }
        public string Action { get; set; }
        public string Resource { get; set; }
    }

    public class ServerAuthenticationInfo
    {
        public int Version { get; set; } = 2;
        public bool SupportsHmacAuthentication { get; set; }
        public bool SupportsLegacyPasswordAuthentication { get; set; }
        public bool PasswordIsSha256 { get; set; }
        public int AllowedClockSkewSeconds { get; set; } = ServerRequestAuthentication.DefaultAllowedClockSkewSeconds;
        public string KeyId { get; set; }
        public string ClusterId { get; set; }
    }
}