/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Psang.Utils
{
    public static class EncryptionHelper
    {
        private static readonly string password =
            $"psang-R3tpA72bHC-{Environment.UserName + Environment.MachineName}";
        private static readonly byte[] salt = Encoding.UTF8.GetBytes("S0m3S@ltV@lu3");
        private static readonly int iterations = 10000;
        private static readonly int keysize = 32;

        public static string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = DeriveKey(password, salt, iterations, keysize);
                aes.GenerateIV();
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (
                        var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)
                    )
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            using (var aes = Aes.Create())
            {
                var iv = new byte[aes.BlockSize / 8];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;
                aes.Key = DeriveKey(password, salt, iterations, keysize);
                using (
                    var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length)
                )
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private static byte[] DeriveKey(string password, byte[] salt, int iterations, int keysize)
        {
            using (
                var rfc2898 = new Rfc2898DeriveBytes(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256
                )
            )
            {
                return rfc2898.GetBytes(keysize);
            }
        }
    }
}
