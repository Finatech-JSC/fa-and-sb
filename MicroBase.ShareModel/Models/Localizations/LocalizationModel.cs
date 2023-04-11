using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.Localizations.Localizations
{
    public class LocalizationModel : BaseModel
    {
        [RoboText(IsHidden = true)]
        public string Prefix { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Key", Name = "Key", MaxLength = 255, IsRequired = true, Cols = 12, Order = 2)]
        public string Key { get; set; }

        [RoboDropDown(LabelText = "Culture Code", FirstOptionLabel = "Choose", Name = "CultureCode", IsRequired = true, Order = 3)]
        public string CultureCode { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Content", Name = "Content", MaxLength = 512, Cols = 12, Order = 4)]
        public string Content { get; set; }
    }
}