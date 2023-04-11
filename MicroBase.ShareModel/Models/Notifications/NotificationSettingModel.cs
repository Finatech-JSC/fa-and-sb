using MicroBase.Share.Models.CMS.RoboForm.UI;
using Microsoft.AspNetCore.Http;
using System;

namespace MicroBase.Share.Models.Notifications
{
    public class NotificationSettingModel : BaseModel
    {
        #region On left side

        [RoboText(GroupPosition = RoboGroupPosition.Left, GroupCol = 9, LabelText = "Title", Name = "Title", MaxLength = 255, IsRequired = true, Cols = 12, Order = 1)]
        public string Title { get; set; }

        [RoboText(GroupPosition = RoboGroupPosition.Left, GroupCol = 9, Type = RoboTextType.MultiText, LabelText = "Sub Content", Name = "SubContent", MaxLength = 512, Cols = 12, Order = 2)]
        public string SubContent { get; set; }

        [RoboText(GroupPosition = RoboGroupPosition.Left, GroupCol = 9, Type = RoboTextType.RichText, LabelText = "Content", Name = "Content", Cols = 12, Order = 3)]
        public string Content { get; set; }

        #endregion

        #region On right side

        [RoboDropDown(GroupPosition = RoboGroupPosition.Right, GroupCol = 3, LabelText = "Redirect Type", FirstOptionLabel = "Choose Type", Name = "RedirectType", Order = 1)]
        public string RedirectType { get; set; }

        [RoboDropDown(GroupPosition = RoboGroupPosition.Right, GroupCol = 3, LabelText = "Redirect To", FirstOptionLabel = "Choose", Name = "RedirectTo", Order = 1)]
        public string RedirectTo { get; set; }

        [RoboText(GroupPosition = RoboGroupPosition.Right, GroupCol = 3, LabelText = "Link", Name = "Link", Cols = 12, Order = 2)]
        public string Link { get; set; }

        [RoboText(GroupPosition = RoboGroupPosition.Right, GroupCol = 3, LabelText = "Extra Params", Name = "ExtraParams", Cols = 12, Order = 2)]
        public string ExtraParams { get; set; }

        [RoboText(GroupPosition = RoboGroupPosition.Right, IsHidden = true)]
        public string Image { get; set; }

        [RoboFileUpload(GroupPosition = RoboGroupPosition.Right, GroupCol = 3, LabelText = "Thumbnail", IsRequired = true, IsShowPreview = true, Name = "FileImage", ThumbnailField = "Image", Order = 3)]
        public IFormFile FileImage { get; set; }

        [RoboText(GroupPosition = RoboGroupPosition.Right, GroupCol = 3, Type = RoboTextType.DateTimePicker, LabelText = "Start Date", Name = "StartDate", Order = 4)]
        public DateTime? StartDate { get; set; }

        [RoboDropDown(GroupPosition = RoboGroupPosition.Right, GroupCol = 3, LabelText = "Key", FirstOptionLabel = "Choose", Name = "Key", Order = 5)]
        public string Key { get; set; }

        [RoboText(GroupPosition = RoboGroupPosition.Right, GroupCol = 3,LabelText = "Repeat (Day)", Name = "RepeatDay", Order = 6)]
        public int RepeatDay { get; set; }

        [RoboCheckbox(GroupPosition = RoboGroupPosition.Right, GroupCol = 3, LabelText = "Push To Mail Box", Name = "PushToMailBox", Order = 7)]
        public bool PushToMailBox { get; set; }

        [RoboCheckbox(GroupPosition = RoboGroupPosition.Right, GroupCol = 3, LabelText = "Send All", Name = "SendToAll", Order = 8)]
        public bool SendToAll { get; set; }

        [RoboCheckbox(GroupPosition = RoboGroupPosition.Right, GroupCol = 3, LabelText = "Enabled", Name = "Enabled", Order = 9)]
        public bool Enabled { get; set; }

        #endregion
    }
}