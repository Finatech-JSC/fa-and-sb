using MicroBase.Share.Models.CMS.RoboForm.UI;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json.Serialization;

namespace MicroBase.Share.Models.MobileApps
{
    public class MobileSlideModel
    {
        [RoboText(IsHidden = true)]
        public Guid Id { get; set; }

        [RoboText(LabelText = "Platform", Name = "Platform", MaxLength = 255, Cols = 12, Order = 1)]
        public string Platform { get; set; }

        [RoboText(LabelText = "Title", Name = "Title", MaxLength = 255, Cols = 12, Order = 2)]
        public string Title { get; set; }

        [RoboText(LabelText = "Description", Name = "Description", Type = RoboTextType.MultiText, MaxLength = 255, Cols = 12, Order = 3)]
        public string Description { get; set; }

        [RoboFileUpload(LabelText = "Thumbnail", Name = "Thumbnail", IsShowPreview = true, ThumbnailField = "ThumbnailUrl", IsRequired = true, Order = 4)]
        [JsonIgnore]
        public IFormFile Thumbnail { get; set; }

        [RoboText(IsHidden = true)]
        public string ThumbnailUrl { get; set; }

        [RoboText(LabelText = "Display Order", Name = "DisplayOrder", MaxLength = 255, IsRequired = true, Cols = 12, Order = 5)]
        public int DisplayOrder { get; set; }

        [RoboCheckbox(LabelText = "Enabled", Name = "Enabled", Order = 6)]
        public bool? Enabled { get; set; }
    }
}