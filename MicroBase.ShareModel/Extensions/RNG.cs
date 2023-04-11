using System;
using System.Security.Cryptography;
using System.Text;

namespace MicroBase.Share.Extensions
{
    public static partial class CryptographyProviderService
    {
        public static class RNG
        {
            public static string GenerateKey()
            {
                try
                {
                    var apiSecretKey = "";
                    using (var cryptoProvider = new RNGCryptoServiceProvider())
                    {
                        byte[] secretKeyByteArray = new byte[32]; //256 bit
                        cryptoProvider.GetBytes(secretKeyByteArray);
                        apiSecretKey = Convert.ToBase64String(secretKeyByteArray);
                    }
                    return apiSecretKey;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static string GenerateSignature(string timestamp, string method, string url, string body, string appSecret)
            {
                try
                {
                    string requestSignatureBase64String = "";

                    string signatureRawData = String.Format("{0}{1}{2}{3}", timestamp, method, url, string.IsNullOrWhiteSpace(body) ? "" : body);

                    var secretKeyByteArray = Convert.FromBase64String(appSecret);

                    byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);

                    using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
                    {
                        byte[] signatureBytes = hmac.ComputeHash(signature);
                        requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
                    }

                    return requestSignatureBase64String;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static string GenerateSignature(string content, string appSecret)
            {
                try
                {
                    string requestSignatureBase64String = "";
                    var secretKeyByteArray = Encoding.ASCII.GetBytes(appSecret);

                    byte[] signature = Encoding.UTF8.GetBytes(content);
                    using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
                    {
                        byte[] signatureBytes = hmac.ComputeHash(signature);
                        requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
                    }

                    return requestSignatureBase64String;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}