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

namespace FuseCP.EnterpriseServer.Security
{
    /// <summary>
    /// Provides salted password-proof hashing for Enterprise user authentication.
    /// The stored format is: PBKDF2-SHA256$iterations$saltBase64$hashBase64
    /// where the input proof is the SHA256 client password token (or normalized from raw input).
    /// </summary>
    public static class PasswordHardeningService
    {
        private const string Prefix = "PBKDF2-SHA256";
        private const int DefaultIterations = 120000;
        private const int SaltSizeBytes = 16;
        private const int HashSizeBytes = 32;

        public static bool IsHardenedHash(string storedPassword)
        {
            return !string.IsNullOrEmpty(storedPassword)
                && storedPassword.StartsWith(Prefix + "$", StringComparison.Ordinal);
        }

        public static string NormalizeClientPasswordProof(string providedPassword)
        {
            if (string.IsNullOrEmpty(providedPassword))
                return string.Empty;

            // Portal/enterprise auth calls typically pass SHA256:<base64> already.
            if (providedPassword.StartsWith("SHA256:", StringComparison.Ordinal))
                return providedPassword;

            // Fallback for legacy/plain callers.
            return CryptoUtils.SHA256(providedPassword);
        }

        public static string HashClientPasswordProof(string providedPassword)
        {
            string proof = NormalizeClientPasswordProof(providedPassword);
            byte[] proofBytes = Encoding.UTF8.GetBytes(proof);

            byte[] salt = new byte[SaltSizeBytes];
            RandomNumberGenerator.Fill(salt);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                proofBytes,
                salt,
                DefaultIterations,
                HashAlgorithmName.SHA256,
                HashSizeBytes);

            return string.Concat(
                Prefix,
                "$",
                DefaultIterations.ToString(),
                "$",
                Convert.ToBase64String(salt),
                "$",
                Convert.ToBase64String(hash));
        }

        public static bool Verify(string storedPassword, string providedPassword)
        {
            if (!IsHardenedHash(storedPassword))
                return false;

            string[] parts = storedPassword.Split('$');
            if (parts.Length != 4)
                return false;

            if (!int.TryParse(parts[1], out int iterations) || iterations <= 0)
                return false;

            byte[] salt;
            byte[] expectedHash;

            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expectedHash = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            string proof = NormalizeClientPasswordProof(providedPassword);
            byte[] proofBytes = Encoding.UTF8.GetBytes(proof);
            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                proofBytes,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
