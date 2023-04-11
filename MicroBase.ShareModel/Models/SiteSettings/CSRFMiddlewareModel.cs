using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.SiteSettings
{
    public class CSRFMiddlewareModel
    {
        [RoboText(Type = RoboTextType.TextBox, LabelText = "SecretKey", Name = "SecretKey", Cols = 12, Order = -1)]
        public string SecretKey { get; set; }

        [RoboText(Type = RoboTextType.TextBox, LabelText = "MinTime", Name = "MinTime", Cols = 12, Order = 0)]
        public int MinTime { get; set; }

        [RoboText(Type = RoboTextType.TextBox, LabelText = "MaxTime", Name = "MaxTime", Cols = 12, Order = 1)]
        public int MaxTime { get; set; }

        [RoboCheckbox(LabelText = "Kích hoạt", Name = "Enabled", Cols = 12, Order = 3)]
        public bool Enabled { get; set; }
    }
}
