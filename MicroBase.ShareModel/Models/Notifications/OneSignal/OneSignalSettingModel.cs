using MicroBase.Share.Models.CMS.RoboForm.UI;

namespace MicroBase.Share.Models.Notifications.OneSignal
{
    public class OneSignalSettingModel
    {
        [RoboText(LabelText = "App Id", Name = "AppId", MaxLength = 255, IsRequired = true, Cols = 12, Order = 1)]
        public string AppId { get; set; }

        [RoboText(LabelText = "Api endpoint", Name = "ApiEndpoint", MaxLength = 255, IsRequired = true, Cols = 12, Order = 2)]
        public string ApiEndpoint { get; set; }

        [RoboText(LabelText = "Authentication", Name = "Authentication", MaxLength = 255, IsRequired = true, Cols = 12, Order = 3)]
        public string Authentication { get; set; }
    }
}