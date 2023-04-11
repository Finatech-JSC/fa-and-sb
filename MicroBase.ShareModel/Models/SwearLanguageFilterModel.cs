using MicroBase.Share.Models.CMS.RoboForm.UI;
using System.Collections.Generic;

namespace MicroBase.Share.Models
{
    public class SwearLanguageFilterModel
    {
        [RoboText(Type = RoboTextType.MultiText, LabelText = "Language filter", Name = "BlackList", Height = 200, Cols = 12, Order = 1)]
        public string BlackList { get; set; }
    }

    public class SwearLanguageCacheModel
    {
        public IEnumerable<string> BlackList { get; set; }
    }
}