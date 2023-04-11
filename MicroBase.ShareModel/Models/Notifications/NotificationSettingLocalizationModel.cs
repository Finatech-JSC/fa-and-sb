using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.Notifications
{
    public class NotificationSettingLocalizationModel : BaseModel
    {
        [RoboText(LabelText = "Title", Name = "Title", MaxLength = 255, IsRequired = true, Cols = 12, Order = 1)]
        public string Title { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Sub-Content", Name = "SubContent", MaxLength = 512, Cols = 12, Order = 2)]
        public string SubContent { get; set; }

        [RoboText(Type = RoboTextType.RichText, LabelText = "Content", Name = "Content", Cols = 12, Order = 3)]
        public string Content { get; set; }

        [RoboText(LabelText = "Link", Name = "Link", MaxLength = 255, IsRequired = true, Cols = 12, Order = 1)]
        public string Link { get; set; }
    }
}