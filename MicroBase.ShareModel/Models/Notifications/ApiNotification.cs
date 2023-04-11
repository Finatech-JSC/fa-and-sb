using System;

namespace MicroBase.Share.Models.Notifications
{
    public class ApiNotification
    {
        public int TotalUnRead { get; set; }

        public TPaging<ApiNotificationModel> Notifications { get; set; }
    }

    public class ApiNotificationModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string SubContent { get; set; }

        public string BodyDescription { get; set; }

        public string ThumbnailImage { get; set; }

        public bool IsRead { get; set; }

        public string ActionType { get; set; }

        public string ActionTo { get; set; }

        public string ExtraParams { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool DefaultNoti { get; set; }
    }

    public class PopUpNotificationModel : ApiNotificationModel
    {
        public string Key { get; set; }

        public DateTime StartDate { get; set; }

        public string RedirectTo { get; set; }
    }
}