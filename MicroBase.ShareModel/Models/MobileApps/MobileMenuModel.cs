using MicroBase.Share.Models.CMS.RoboForm.UI;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace MicroBase.Share.Models.MobileApps
{
    public class MobileMenuModel : BaseMobilePlatfromModel
    {
        [RoboText(IsHidden = true)]
        public Guid Id { get; set; }

        [RoboFileUpload(LabelText = "Thumbnail", Name = "Thumbnail", IsShowPreview = true, ThumbnailField = "ThumbnailUrl", IsRequired = true, Order = 1)]
        [JsonIgnore]
        public IFormFile Thumbnail { get; set; }

        [RoboDropDown(LabelText = "Menu Type", FirstOptionLabel = "Choose Menu Type", Name = "MenuType", IsRequired = true, Cols = 6, Order = 2)]
        public string MenuType { get; set; }

        [RoboText(IsHidden = true)]
        public string ThumbnailUrl { get; set; }

        [RoboText(LabelText = "Name", Name = "Name", MaxLength = 255, Cols = 6, Order = 3)]
        public string Name { get; set; }

        [RoboDropDown(LabelText = "Action Type", FirstOptionLabel = "Choose Type", Name = "ActionType", IsRequired = true, Cols = 6, Order = 4)]
        public string ActionType { get; set; }

        [RoboDropDown(LabelText = "Action Screen", FirstOptionLabel = "Choose Screen", Name = "ActionToScreen", IsRequired = true, Cols = 6, Order = 5)]
        public string ActionToScreen { get; set; }

        [RoboText(LabelText = "Action Link", Name = "ActionToLink", MaxLength = 255, Cols = 6, Order = 6)]
        public string ActionToLink { get; set; }

        [RoboText(IsHidden = true)]
        public string ActionTo { get; set; }

        [RoboText(LabelText = "Tooltip", Name = "Tooltip", MaxLength = 255, Cols = 6, Order = 7)]
        public string Tooltip { get; set; }

        [RoboText(LabelText = "Tooltip Color", Name = "TooltipColor", MaxLength = 255, Cols = 6, Order = 8)]
        public string TooltipColor { get; set; }

        [RoboText(LabelText = "Display Order", Name = "DisplayOrder", MaxLength = 255, IsRequired = true, Cols = 6, Order = 9)]
        public int DisplayOrder { get; set; }

        [RoboDropDown(LabelText = "In-Reivew Mode", FirstOptionLabel = "Choose Mode", Name = "InReviewMode", IsBreakLine = true, Cols = 6, Order = 10)]
        public string InReviewMode { get; set; }

        [RoboCheckbox(LabelText = "Required Login", Name = "RequiredLogin", Cols = 6, Order = 11)]
        public bool RequiredLogin { get; set; }

        [RoboCheckbox(LabelText = "Allow IOS", Name = "IsAllowIos", Cols = 6, Order = 12)]
        public bool IsAllowIos { get; set; }

        [RoboCheckbox(LabelText = "Allow Android", Name = "IsAllowAndroid", Cols = 6, Order = 13)]
        public bool IsAllowAndroid { get; set; }

        [RoboCheckbox(LabelText = "Allow Web", Name = "IsAllowWeb", Cols = 6, Order = 14)]
        public bool IsAllowWeb { get; set; }

        [RoboCheckbox(LabelText = "Enabled", Name = "Enabled", Cols = 6, IsBreakLine = true, Order = 15)]
        public bool Enabled { get; set; }
    }
}