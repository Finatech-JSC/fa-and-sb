using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MicroBase.Share.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Convert name to slug
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToSlugUrl(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            name = name.Replace("-", " ");
            var slug = Regex.Replace(name, "[^\\w\\s]", string.Empty).Replace(" ", "-").ToLower();
            string formD = slug.Normalize(NormalizationForm.FormD);
            slug = regex.Replace(formD, string.Empty).Replace("đ", "d");
            while (slug.IndexOf("--", StringComparison.Ordinal) > 0)
            {
                slug = slug.Replace("--", "-");
            }

            return slug.ToLower();
        }

        public static string RemoveUnicode(this string name)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            var slug = Regex.Replace(name, "[^\\w\\s]", string.Empty);
            string formD = slug.Normalize(NormalizationForm.FormD);
            slug = regex.Replace(formD, string.Empty).Replace("đ", "d");
            return slug;
        }

        /// <summary>
        /// Return content after substring by len
        /// </summary>
        /// <param name="content"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string ToSubContent(this string content, int len)
        {
            if (content != null)
            {
                var temContent = content;
                if (temContent.Length >= len)
                {
                    var subContent = temContent.Substring(0, len - 3);
                    var valuearray = subContent.Split(' ');
                    var result = string.Empty;
                    for (var i = 0; i < valuearray.Length - 1; i++)
                    {
                        result = result + " " + valuearray[i];
                    }
                    return result + "...";
                }
                return temContent;
            }
            return string.Empty;
        }

        /// <summary>
        /// Create level for a category
        /// </summary>
        /// <param name="order"></param>
        /// <param name="parentLevel"></param>
        /// <returns></returns>
        public static string MakeLevel(int order, string parentLevel)
        {
            if (parentLevel == null)
            {
                return string.Empty;
            }

            string level = order.ToString(CultureInfo.InvariantCulture);
            while (level.Length < 5)
            {
                level = "0" + level;
            }

            if (parentLevel.Length != 0)
            {
                level = parentLevel + level;
            }

            return level;
        }

        public static string LevelToString(string level, string replaceChar)
        {
            if (string.IsNullOrWhiteSpace(level) || level.Length == 5)
            {
                return string.Empty;
            }

            var str = string.Empty;
            foreach (var item in level)
            {
                str = str + replaceChar;
            }

            return str;
        }

        /// <summary>
        /// Return Vietnamese name day of week
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetDayOfWeek(this DateTime dateTime)
        {
            var dayOfWeek = dateTime.DayOfWeek;
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Thứ Hai";
                case DayOfWeek.Tuesday:
                    return "Thứ Ba";
                case DayOfWeek.Wednesday:
                    return "Thứ Tư";
                case DayOfWeek.Thursday:
                    return "Thứ Năm";
                case DayOfWeek.Friday:
                    return "Thứ Sáu";
                case DayOfWeek.Saturday:
                    return "Thứ Bảy";
                case DayOfWeek.Sunday:
                    return "Chủ Nhật";
                default:
                    return string.Empty;
            }
        }

        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static string FirstCharToLowwer(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.First().ToString().ToLower() + input.Substring(1);
        }

        public static string MaskEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return email;
            }

            var arr = email.Split('@');

            var len = 3;
            if (arr[0].Length < len)
            {
                len = arr[0].Length - 1;
            }

            if (arr[0].Length <= len)
            {
                arr[0] = arr[0] + "**";
            }
            var first = arr[0].Replace(arr[0].Substring(len, arr[0].Length - len), "***");

            var lasts = arr[1].Split('.');
            var last = arr[1].Replace(lasts[0], "***");

            return first + "@" + last;
        }

        public static string MaskPhone(this string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return phone;
            }

            var len = phone.Length - 2;
            var replate = "";

            var i = 0;
            while (i < len)
            {
                replate += "x";
                i++;
            }
            return replate + phone.Substring(len, 2);
        }

        /// <summary>
        /// Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>A decoded string</returns>
        public static string HtmlDecode(this string s)
        {
            return WebUtility.HtmlDecode(s);
        }

        /// <summary>
        /// Converts a string to an HTML-encoded string.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string HtmlEncode(this string s)
        {
            return WebUtility.HtmlEncode(s);
        }

        /// <summary>
        /// Converts a string to a url encode.
        /// </summary>
        public static string ToUrlEncode(this string s)
        {
            return HttpUtility.UrlEncode(s);
        }

        public static string RemoveBetween(this string str, char begin, char end)
        {
            var regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(str, string.Empty);
        }

        public static string JsEncode(this string value, bool appendQuotes = true)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            if (appendQuotes)
            {
                sb.Append("\"");
            }

            foreach (char c in value)
            {
                switch (c)
                {
                    case '\"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        var i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else { sb.Append(c); }
                        break;
                }
            }

            if (appendQuotes)
            {
                sb.Append("\"");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert date from format date
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">Default value = null</param>
        /// <param name="formatDate">Default format ""</param>
        /// <returns></returns>
        public static DateTime? ToDate(this string str, DateTime? defaultValue = null, string formatDate = "")
        {
            try
            {
                if (formatDate == "")
                {
                    return DateTime.Parse(str.ToString());
                }
                else
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime date = DateTime.UtcNow;
                    var kq = DateTime.TryParseExact(str.ToString(), formatDate, provider, System.Globalization.DateTimeStyles.None, out date);
                    if (kq)
                    {
                        return date;
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Return default value
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">Default value = 0</param>
        /// <returns></returns>
        public static object DefaultValue(this string str, object defaultValue = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }
            else
            {
                return str;
            }
        }

        public static object BoolDefaultValue(this bool? val, object defaultValue = null)
        {
            if (val.HasValue)
            {
                return val;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Return decimal value
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">Default value</param>
        /// <returns></returns>
        public static decimal? ToDecimal(this string str, decimal? defaultValue)
        {
            try
            {
                decimal returnValue = 0;
                var kq = decimal.TryParse(str, out returnValue);
                if (kq)
                {
                    return returnValue;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Return integer value
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">Default value = null</param>
        /// <returns></returns>
        public static int? ToInt(this string str, int? defaultValue = null)
        {
            try
            {
                int returnValue = 0;
                var kq = int.TryParse(str, out returnValue);
                if (kq)
                {
                    return returnValue;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Return double value
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">Default value = null</param>
        /// <returns></returns>
        public static double? ToDouble(this string str, double? defaultValue = null)
        {
            try
            {
                double returnValue = 0;
                var kq = double.TryParse(str, out returnValue);
                if (kq)
                {
                    return returnValue;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ToUpperString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            return str.ToUpper();
        }

        public static string FormatWithCommas(this decimal number)
        {
            return Convert.ToDecimal(number).ToString("#,##0");
        }

        public static int? RemoveAllCharacter(this string str, int? defaultValue = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    return 0;
                }

                int returnValue = 0;
                var temp = Regex.Replace(str, @"[^0-9]+", "");
                var result = int.TryParse(temp, out returnValue);
                if (result)
                {
                    return returnValue;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string ToSubstring(this string str, int start, int length)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            var strLength = str.Length;

            if (strLength <= start)
            {
                return str;
            }

            if (strLength - start < length)
            {
                return str.Substring(start, strLength);
            }

            return str.Substring(start, length);
        }

        public static Stream GenerateStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        public static string CustomTrim(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            return input.Trim();
        }
    }
}