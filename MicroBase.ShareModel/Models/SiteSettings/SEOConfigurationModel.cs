using MicroBase.Share.Models.CMS.RoboForm.UI;
using Microsoft.AspNetCore.Http;

namespace MicroBase.Share.Models.SiteSettings
{
    public class SEOConfigurationModel : BaseModel
    {
        [RoboText(IsHidden = true)]
        public string Favicon { get; set; }

        [RoboFileUpload(LabelText = "Favicon", Name = "FaviconFile", IsShowPreview = true, ThumbnailField = "Favicon", Order = 1)]
        public IFormFile LogoFile { get; set; }

        [RoboText(LabelText = "Title", Name = "Title", MaxLength = 255, IsRequired = true, Cols = 12, Order = 2)]
        public string Title { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Keyword", Name = "Keyword", MaxLength = 255, IsRequired = true, Cols = 12, Order = 3)]
        public string Keyword { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Description", Name = "Description", MaxLength = 255, IsRequired = true, Cols = 12, Order = 4)]
        public string Description { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Header meta data", Name = "HeaderMetadata", MaxLength = 5000, Cols = 12, Order = 5)]
        public string HeaderMetadata { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Header scripts", Name = "HeaderScripts", MaxLength = 5000, Cols = 12, Order = 6)]
        public string HeaderScripts { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Body script", Name = "BodyScript", MaxLength = 5000, Cols = 12, Order = 7)]
        public string BodyScript { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Footer script", Name = "FooterScript", MaxLength = 5000, Cols = 12, Order = 8)]
        public string FooterScript { get; set; }
    }
}