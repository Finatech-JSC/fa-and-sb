using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.SiteSettings
{
    public class SiteSettingLocalizationModel : BaseModel
    {
        [RoboText(Type = RoboTextType.RichText, LabelText = "Rich Text (html)", Name = "RichText", Cols = 12, Order = 1)]
        public string RichText { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Raw Text", Name = "RawText", Cols = 12, Order = 2)]
        public string RawText { get; set; }
    }
}