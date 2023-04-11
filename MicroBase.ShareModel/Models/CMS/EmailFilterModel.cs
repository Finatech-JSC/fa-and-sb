using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.CMS
{
    public class EmailFilterModel
    {
        [RoboText(Type = RoboTextType.MultiText, LabelText = "Black list", Name = "BlackList", Height = 200, Cols = 12, Order = 1)]
        public string BlackList { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "White list", Name = "WhiteList", Height = 200, Cols = 12, Order = 2)]
        public string WhiteList { get; set; }
    }
}