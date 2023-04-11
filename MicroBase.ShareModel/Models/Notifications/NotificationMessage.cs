using System;

namespace MicroBase.Share.Models.Notifications
{
    public class NotificationMessage
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string RedirectTo { get; set; }

        public string RedirectType { get; set; }

        public string Link { get; set; }

        public string ExtraParams { get; set; }

        public string Image { get; set; }

        public bool PushToMailBox { get; set; }
    }
}