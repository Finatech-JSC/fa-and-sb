using MicroBase.Share.Models.CMS.RoboForm.UI;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace MicroBase.Share.Models.MobileApps
{
    public class AdsModel : BaseMobilePlatfromModel
    {
        [RoboText(IsHidden = true)]
        public Guid Id { get; set; }

        [RoboText(LabelText = "Name", Name = "Name", MaxLength = 255, Cols = 12, Order = 1)]
        public string Name { get; set; }

        [RoboText(LabelText = "Show In Screen", Name = "ShowInScreen", MaxLength = 255, Cols = 12, Order = 1)]
        public string ShowInScreen { get; set; }

        [RoboFileUpload(LabelText = "Thumbnail", Name = "Thumbnail", IsShowPreview = true, ThumbnailField = "ThumbnailUrl", IsRequired = true, Order = 2)]
        [JsonIgnore]
        public IFormFile Thumbnail { get; set; }

        [RoboText(IsHidden = true)]
        public string ThumbnailUrl { get; set; }

        [RoboDropDown(LabelText = "Action Type", FirstOptionLabel = "Choose", Name = "ActionType", IsRequired = true, Order = 3)]
        public string ActionType { get; set; }

        [RoboDropDown(LabelText = "Action To Screen", FirstOptionLabel = "Choose", Name = "ActionToScreen", IsRequired = true, Order = 4)]
        public string ActionToScreen { get; set; }

        [RoboText(LabelText = "Action To Link", Name = "ActionToLink", MaxLength = 255, Cols = 12, Order = 5)]
        public string ActionToLink { get; set; }

        [RoboText(IsHidden = true)]
        public string ActionTo { get; set; }

        [RoboText(LabelText = "Extra Params", Name = "ExtraParams", MaxLength = 255, Cols = 12, Order = 6)]
        public string ExtraParams { get; set; }

        [RoboDropDown(LabelText = "Culture Code", FirstOptionLabel = "Choose Culture Code", Name = "CultureCode", IsRequired = true, Order = 6)]
        public string CultureCode { get; set; }

        [RoboText(LabelText = "Display Order", Name = "DisplayOrder", MaxLength = 255, IsRequired = true, Cols = 12, Order = 7)]
        public int DisplayOrder { get; set; }

        [RoboCheckbox(LabelText = "Allow IOS", Name = "IsAllowIos", Order = 8)]
        public bool IsAllowIos { get; set; }

        [RoboCheckbox(LabelText = "Allow Android", Name = "IsAllowAndroid", Order = 9)]
        public bool IsAllowAndroid { get; set; }

        [RoboCheckbox(LabelText = "Allow Web", Name = "IsAllowWeb", Order = 10)]
        public bool IsAllowWeb { get; set; }

        [RoboText(Type = RoboTextType.DateTimePicker, LabelText = "Start Time", Name = "StartTime", Order = 11)]
        public DateTime? StartTime { get; set; }

        [RoboText(Type = RoboTextType.DateTimePicker, LabelText = "End Time", Name = "EndTime", Order = 12)]
        public DateTime? EndTime { get; set; }

        [RoboCheckbox(LabelText = "Enabled", Name = "Enabled", Order = 13)]
        public bool Enabled { get; set; }
    }

    public class MobileAdsBannerSlideResponse : BaseMobilePlatfromResponse
    {
        public string Name { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ActionType { get; set; }

        public string ActionTo { get; set; }

        public int DisplayOrder { get; set; }

        public string ExtraParams { get; set; }
    }
}