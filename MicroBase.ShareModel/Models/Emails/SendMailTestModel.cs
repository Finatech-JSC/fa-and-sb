using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.Emails
{
    public class SendMailTestModel : BaseModel
    {
        [RoboDropDown(LabelText = "Template Key", FirstOptionLabel = "Choose Template", Name = "Key", Order = 1)]
        public string Key { get; set; }

        [RoboDropDown(IsHidden = true, LabelText = "Language", FirstOptionLabel = "Choose Language", Name = "CultureCode", Order = 2)]
        public string CultureCode { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Email address (Separate by ';')", Name = "Emails", IsRequired = true, Cols = 12, Order = 3)]
        public string Emails { get; set; }
    }
}