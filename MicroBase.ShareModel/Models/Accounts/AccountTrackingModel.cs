using System;
using System.Text.Json.Serialization;

namespace MicroBase.Share.Models.Accounts
{
    public class BaseActivityTrackingModel
    {
        [JsonIgnore]
        public string IpAddress { get; set; }

        [JsonIgnore]
        public string Location { get; set; }

        [JsonIgnore]
        public string UserAgent { get; set; }

        [JsonIgnore]
        public string Via { get; set; }

        [JsonIgnore]
        public string Action { get; set; }

        [JsonIgnore]
        public string Description { get; set; }
    }

    public class AccountTrackingModel : BaseActivityTrackingModel
    {
        public string UserName { get; set; }

        public Guid? IdentityUserId { get; set; }
    }
}