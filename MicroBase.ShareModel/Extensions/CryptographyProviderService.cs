using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MicroBase.Share.Extensions
{
    public static partial class CryptographyProviderService
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static class Method
        {
            public static string Post = "POST";
            public static string Get = "GET";
        }

        public static long GetCurrentUnixTimestampSeconds()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Gets a timestamp in milliseconds.
        /// </summary>
        /// <returns>Timestamp in milliseconds.</returns>
        public static string GenerateTimeStamp(DateTime baseDateTime)
        {
            var dtOffset = new DateTimeOffset(baseDateTime);
            return dtOffset.ToUnixTimeMilliseconds().ToString();
        }

        public static string GetHmacInHex(string key, string data)
        {
            var hmacKey = Encoding.UTF8.GetBytes(key);

            using (var signatureStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var hex = new HMACSHA256(hmacKey).ComputeHash(signatureStream)
                    .Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat("{0:x2}", b), sb => sb.ToString());

                return hex;
            }
        }

        public static string GenerateSha256Base64(string text, string secretKey)
        {
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return Convert.ToBase64String(hashmessage);
            }
        }

        /// <summary>
        /// Return string after MD5 encrypt
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5Encrypt(this string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            var md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            var data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            foreach (byte t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}