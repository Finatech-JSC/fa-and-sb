using Newtonsoft.Json;
using System.Collections.Generic;

namespace MicroBase.Share.Models.Notifications
{
    public class NotificationRequest
    {
        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("contents")]
        public Dictionary<string, string> Contents { get; set; }

        [JsonProperty("included_segments")]
        public List<string> IncludedSegments { get; set; }

        [JsonProperty("data")]
        public NotificationRequestData Data { get; set; }

        [JsonProperty("big_picture")]
        public string BigPicture { get; set; }

        [JsonProperty("ios_attachments")]
        public NotificationRequestIosAttachments IosAttachments { get; set; }

        [JsonProperty("headings")]
        public Dictionary<string, string> Headings { get; set; }

        [JsonProperty("include_player_ids")]
        public List<string> IncludePlayerIds { get; set; }
    }

    public class NotificationRequestData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("actionTo")]
        public string ActionTo { get; set; }

        [JsonProperty("actionType")]
        public string ActionType { get; set; }

        [JsonProperty("params")]
        public string Params { get; set; }
    }

    public class NotificationRequestIosAttachments
    {
        [JsonProperty("mediaId")]
        public string MediaId { get; set; }
    }
}