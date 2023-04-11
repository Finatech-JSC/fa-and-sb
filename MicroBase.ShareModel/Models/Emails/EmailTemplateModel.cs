using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.Emails
{
    public class EmailTemplateModel : BaseModel
    {
        [RoboDropDown(IsHidden = true, LabelText = "Template Key", FirstOptionLabel = "Choose Template", Name = "Key", Cols = 3, Order = 1)]
        public string Key { get; set; }

        [RoboDropDown(IsHidden = true, LabelText = "Language", FirstOptionLabel = "Choose Language", Name = "CultureCode", Cols = 3, Order = 2)]
        public string CultureCode { get; set; }

        [RoboText(LabelText = "Subject", Name = "Subject", IsRequired = true, Cols = 12, Order = 3)]
        public string Subject { get; set; }

        [RoboText(Type = RoboTextType.RichText, LabelText = "Mail Content", Name = "Body", IsRequired = true, Cols = 12, Order = 4)]
        public string Body { get; set; }
    }
}