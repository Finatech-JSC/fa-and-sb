using System;

namespace MicroBase.Share.Models.Notifications
{
    public class NotificationSettingResponse : NotificationSettingModel
    {
        public bool IsSend { get; set; }

        public DateTime? LatestSend { get; set; }
    }
}