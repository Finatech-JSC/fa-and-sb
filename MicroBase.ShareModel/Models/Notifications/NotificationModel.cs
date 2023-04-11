using System;
using System.Collections.Generic;

namespace MicroBase.Share.Models.Notifications
{
    public class NotificationModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string RedirectTo { get; set; }

        public string ExtraParams { get; set; }

        public string Image { get; set; }

        public string RedirectType { get; set; }

        public List<string> ConnectionIds { get; set; }
    }
}