using System;
using System.Collections.Generic;

namespace MicroBase.Share.Models.Notifications
{
    public class AccountNotificationModel
    {
        public Guid IdentityUserId { get; set; }

        public string EventKey { get; set; }

        public List<string> ConnectionIds { get; set; }
    }

    public class SendEmailModel
    {
        public string TemplateKey { get; set; }

        public Dictionary<string, string> ReplaceValues { get; set; }
    }

    public class NotificationWithDataModel
    {
        public string TemplateKey { get; set; }

        public Dictionary<string, string> ReplaceValuesTitle { get; set; }

        public Dictionary<string, string> ReplaceValuesDescription { get; set; }

        public Dictionary<string, string> ReplaceValuesContent { get; set; }

        public string ExtraParams { get; set; }
    }
}