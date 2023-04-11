using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using static MicroBase.Share.Constants.Constants;

namespace MicroBase.Share.Extensions
{
    public static class ConstantsService
    {
        public static string GetEmailKeyLabel(string key)
        {
            if (!EmailTemplates.KeyMaps.ContainsKey(key))
            {
                return "N/A";
            }

            return EmailTemplates.KeyMaps[key];
        }

        public static IList<SelectListItem> GetEmailTemplateKey(string selectedVal)
        {
            var templates = EmailTemplates.KeyMaps.Select(s => new SelectListItem
            {
                Text = s.Value,
                Value = s.Key,
                Selected = selectedVal == s.Key
            }).ToList().OrderBy(s => s.Text);

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "Chọn mấu email"
                }
            };

            result.AddRange(templates);

            return result;
        }

        public static Dictionary<string, string> GetValueNameDict<TEnum>()
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum must be an Enumeration type");

            var res = Enum.GetValues(typeof(TEnum)).Cast<TEnum>()
                .ToDictionary(e => e.ToString(), e => ((int)(object)e).ToString());

            return res;
        }

        public static IList<SelectListItem> GetCultureCode(string selectedVal)
        {
            var templates = CultureCode.Maps.Select(s => new SelectListItem
            {
                Text = s.Value,
                Value = s.Key,
                Selected = selectedVal == s.Key
            }).ToList().OrderBy(s => s.Text);

            var result = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "Chọn ngôn ngữ"
                }
            };

            result.AddRange(templates);
            return result;
        }
    }
}