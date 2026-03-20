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
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace FuseCP.WebDav.Core.Security.Cryptography
{
    public class CryptoUtils : ICryptography
    {
        private string EnterpriseServerRegistryPath = "SOFTWARE\\FuseCP\\EnterpriseServer";

        private string CryptoKey
        {
            get
            {
                string Key = ConfigurationManager.AppSettings["FuseCP.AltCryptoKey"];
                string value = string.Empty;

                if (!string.IsNullOrEmpty(Key))
                {
                    if (OperatingSystem.IsWindows())
                    {
                        RegistryKey root = Registry.LocalMachine;
                        RegistryKey rk = root.OpenSubKey(EnterpriseServerRegistryPath);
                        if (rk != null)
                        {
                            value = (string)rk.GetValue(Key, null);
                            rk.Close();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(value))
                    return value;
                else
                    return ConfigurationManager.AppSettings["FuseCP.CryptoKey"];

            }
        }

        private bool EncryptionEnabled
        {
            get
            {
                return (ConfigurationManager.AppSettings["FuseCP.EncryptionEnabled"] != null)
                    ? Boolean.Parse(ConfigurationManager.AppSettings["FuseCP.EncryptionEnabled"]) : true;
            }
        }

        public string Encrypt(string InputText)
        {
            string Password = CryptoKey;

            if (!EncryptionEnabled)
                return InputText;

            if (InputText == null)
                return InputText;

            // First we need to turn the input strings into a byte array.
            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(InputText);


            // We are using salt to make it harder to guess our key
            // using a dictionary attack.
            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());


            // The (Secret Key) will be generated from the specified 
            // password and salt.
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);


            // Create an encryptor from the existing SecretKey bytes.
            // We use 32 bytes for the secret key and 16 bytes for the IV
            // to preserve legacy payload compatibility.
            byte[] key = SecretKey.GetBytes(32);
            byte[] iv = SecretKey.GetBytes(16);
            byte[] CipherBytes;
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform Encryptor = aes.CreateEncryptor(key, iv))
                using (MemoryStream memoryStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write))
                {
                    // Start the encryption process.
                    cryptoStream.Write(PlainText, 0, PlainText.Length);

                    // Finish encrypting.
                    cryptoStream.FlushFinalBlock();

                    // Convert our encrypted data from a memoryStream into a byte array.
                    CipherBytes = memoryStream.ToArray();
                }
            }



            // Convert encrypted data into a base64-encoded string.
            // A common mistake would be to use an Encoding class for that. 
            // It does not work, because not all byte values can be
            // represented by characters. We are going to be using Base64 encoding
            // That is designed exactly for what we are trying to do. 
            string EncryptedData = Convert.ToBase64String(CipherBytes);



            // Return encrypted string.
            return EncryptedData;
        }

        public string Decrypt(string InputText)
        {
            try
            {
                if (!EncryptionEnabled)
                    return InputText;

                if (InputText == null || InputText == "")
                    return InputText;

                string Password = CryptoKey;

                byte[] EncryptedData = Convert.FromBase64String(InputText);
                byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());


                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
                byte[] key = SecretKey.GetBytes(32);
                byte[] iv = SecretKey.GetBytes(16);
                byte[] PlainText = new byte[EncryptedData.Length];
                int DecryptedCount;
                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform Decryptor = aes.CreateDecryptor(key, iv))
                    using (MemoryStream memoryStream = new MemoryStream(EncryptedData))
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read))
                    {
                        // Since at this point we don't know what the size of decrypted data
                        // will be, allocate the buffer long enough to hold EncryptedData;
                        // DecryptedData is never longer than EncryptedData.
                        // Start decrypting.
                        DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
                    }
                }

                // Convert decrypted data into a string. 
                string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);


                // Return decrypted string.   
                return DecryptedData;
            }
            catch
            {
                return "";
            }
        }

        private string SHA1(string plainText)
        {
            // Convert plain text into a byte array.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes;
            using (HashAlgorithm hash = System.Security.Cryptography.SHA1.Create())
            {
                hashBytes = hash.ComputeHash(plainTextBytes);
            }

            // Return the result.
            return Convert.ToBase64String(hashBytes);
        }
    }
}
