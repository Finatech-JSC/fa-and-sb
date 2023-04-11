using System;

namespace MicroBase.Share.Extensions
{
    public static class NumberExtension
    {
        /// <summary>
        /// return default value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object DefaultValue(this int? value, object defaultValue = null)
        {
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return value.Value;
            }
        }

        public static string NumberFormat(this decimal value)
        {
            var balance = value % 1;
            if (balance > 0)
            {
                var priceStr = balance.ToString("#.########");
                return (value - balance).ToString("n0") + priceStr;
            }

            return value.ToString("n0");
        }

        public static string NumberFormat(this int value)
        {
            return value.ToString("n0");
        }

        public static decimal Trunc(this decimal value, int places)
        {
            var f = (decimal)Math.Pow(10, places);
            return Math.Truncate(value * f) / f;
        }

        public static double Trunc(this double value, int places)
        {
            var f = Math.Pow(10, places);
            return Math.Truncate(value * f) / f;
        }

        public static long GetLongRandomNumber(long min, long max)
        {
            var rand = new Random();
            long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((Int32)min, (Int32)max);

            return result;
        }
    }
}