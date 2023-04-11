using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace MicroBase.Share.Extensions
{
    public static partial class CryptographyProviderService
    {
        public static class RSA
        {
            static int dwKeySize = 2048;

            public static void GenerateKey(out string publicKey, out string privateKey)
            {
                try
                {
                    using (var csp = new RSACryptoServiceProvider(dwKeySize))
                    {
                        publicKey = ToXmlString(csp, false);
                        privateKey = ToXmlString(csp, true);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static string GenerateSignature(string body, string privateKey)
            {
                try
                {
                    byte[] hash;
                    var hashString = "";
                    using (var rsa = new RSACryptoServiceProvider(dwKeySize))
                    {
                        var parameters = ExportParameter(privateKey);
                        rsa.ImportParameters(parameters);

                        using (SHA256 sha256 = SHA256.Create())
                        {
                            hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(body));
                        }

                        RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(rsa);
                        RSAFormatter.SetHashAlgorithm("SHA256");

                        byte[] signedHash = RSAFormatter.CreateSignature(hash);
                        hashString = Convert.ToBase64String(signedHash);
                    }
                    return hashString;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            public static bool VerifySignature(string body, string signature, string pubKey)
            {
                try
                {
                    byte[] hash;
                    var signedHash = Convert.FromBase64String(signature);
                    var isVerify = false;
                    using (var rsa = new RSACryptoServiceProvider(dwKeySize))
                    {
                        var parameters = ExportParameter(pubKey);
                        rsa.ImportParameters(parameters);

                        using (SHA256 sha256 = SHA256.Create())
                        {
                            hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(body));
                        }

                        RSAPKCS1SignatureDeformatter RSADeFormatter = new RSAPKCS1SignatureDeformatter(rsa);

                        RSADeFormatter.SetHashAlgorithm("SHA256");
                        isVerify = RSADeFormatter.VerifySignature(hash, signedHash);
                    }
                    return isVerify;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            private static RSAParameters ExportParameter(string key)
            {
                try
                {
                    RSAParameters parameters = new RSAParameters();

                    XmlDocument xmlDoc = new XmlDocument();
                    var xmlString = Encoding.UTF8.GetString(Convert.FromBase64String(key));
                    xmlDoc.LoadXml(xmlString);

                    if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
                    {
                        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                        {
                            switch (node.Name)
                            {
                                case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
                                case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
                                case "P": parameters.P = Convert.FromBase64String(node.InnerText); break;
                                case "Q": parameters.Q = Convert.FromBase64String(node.InnerText); break;
                                case "DP": parameters.DP = Convert.FromBase64String(node.InnerText); break;
                                case "DQ": parameters.DQ = Convert.FromBase64String(node.InnerText); break;
                                case "InverseQ": parameters.InverseQ = Convert.FromBase64String(node.InnerText); break;
                                case "D": parameters.D = Convert.FromBase64String(node.InnerText); break;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid XML RSA key.");
                    }

                    return parameters;

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            private static string ToXmlString(RSACryptoServiceProvider rsa, bool includePrivateParameters = false)
            {
                string xmlString;
                RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

                if (includePrivateParameters)
                {
                    xmlString = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                        Convert.ToBase64String(parameters.Modulus),
                        Convert.ToBase64String(parameters.Exponent),
                        Convert.ToBase64String(parameters.P),
                        Convert.ToBase64String(parameters.Q),
                        Convert.ToBase64String(parameters.DP),
                        Convert.ToBase64String(parameters.DQ),
                        Convert.ToBase64String(parameters.InverseQ),
                        Convert.ToBase64String(parameters.D));
                }
                else
                {
                    xmlString = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                        Convert.ToBase64String(parameters.Modulus),
                        Convert.ToBase64String(parameters.Exponent));
                }

                byte[] xmlByte = Encoding.UTF8.GetBytes(xmlString);
                var key = Convert.ToBase64String(xmlByte);
                return key;
            }
        }
    }
}