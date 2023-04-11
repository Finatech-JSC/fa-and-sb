using MicroBase.Share.Models.CMS.RoboForm.UI;
using System;

namespace MicroBase.Share.Models.MobileApps
{
    public class MobileVersionModel
    {
        [RoboText(LabelText = "Android Version", Name = "AndroidVersion", MaxLength = 255, IsRequired = true, Cols = 12, Order = 1)]
        public string AndroidVersion { get; set; }

        [RoboText(LabelText = "IOS Version", Name = "IOSVersion", MaxLength = 255, IsRequired = true, Cols = 12, Order = 2)]
        public string IOSVersion { get; set; }

        [RoboText(Type = RoboTextType.MultiText, LabelText = "Description", Name = "Description", MaxLength = 255, IsRequired = true, Cols = 12, Order = 3)]
        public string Description { get; set; }

        [RoboCheckbox(LabelText = "Force Update", Name = "ForceUpdate", Cols = 12, Order = 4)]
        public bool ForceUpdate { get; set; }
    }

    public class ReviewMobileVersionModel
    {
        [RoboText(LabelText = "Android Version In-Review", Name = "AndroidVersionInReview", MaxLength = 255, Cols = 12, Order = 1)]
        public string AndroidVersionInrview { get; set; }

        [RoboText(LabelText = "IOS Version In-Review", Name = "IOSVersionInReview", MaxLength = 255, Cols = 12, Order = 2)]
        public string IOSVersionInrview { get; set; }
    }

    public class AppVersionModel : MobileVersionModel
    {
        [RoboText(IsHidden = true)]
        public Guid Id { get; set; }
    }

    public class AppVersionResponse
    {
        public bool NewVersion { get; set; }

        public bool ForceUpdate { get; set; }

        public string Description { get; set; }
    }
}