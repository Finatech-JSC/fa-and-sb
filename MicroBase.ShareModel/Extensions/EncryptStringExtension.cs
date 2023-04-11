using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroBase.Share.Extensions
{
    public static class EncryptStringExtension
    {
        public static string Encrypt(this string input)
        {
            var random = new Random();
            var nonce = random.Next(2, input.Length);
            var subLen = 0;
            if (input.Length % nonce == 0)
            {
                subLen = input.Length / nonce;
            }
            else
            {
                subLen = input.Length / nonce + 1;
            }

            var arrs = new List<string>();
            var i = 0;
            while (i < subLen)
            {
                var temp = input.Substring(0, nonce);
                var nextTemp = string.Empty;
                foreach (var item in temp)
                {
                    var step = random.Next(1, 100);
                    var c = item >> step;
                    var t = ByteToBitsString((byte)c);

                    var y = item << step;
                    var k = ByteToBitsString((byte)y);
                }

                arrs.Add(temp);

                input = input.Substring(nonce);
                i++;
            }

            return string.Empty;
        }

        private static string ByteToBitsString(byte byteIn)
        {
            string[] bits = new string[8];
            bits[0] = Convert.ToString((byteIn / 128) % 2);
            bits[1] = Convert.ToString((byteIn / 64) % 2);
            bits[2] = Convert.ToString((byteIn / 32) % 2);
            bits[3] = Convert.ToString((byteIn / 16) % 2);
            bits[4] = Convert.ToString((byteIn / 8) % 2);
            bits[5] = Convert.ToString((byteIn / 4) % 2);
            bits[6] = Convert.ToString((byteIn / 2) % 2);
            bits[7] = Convert.ToString((byteIn / 1) % 2);

            return string.Join("", bits);
        }
    }
}
