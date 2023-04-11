using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Logging
{
    public class TelegramMessageModel
    {
        [Required]
        [JsonProperty("chat_id")]
        public string ChatId { get; set; }

        [Required]
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("disable_notification")]
        public bool DisableNotification { get; set; }
    }
}