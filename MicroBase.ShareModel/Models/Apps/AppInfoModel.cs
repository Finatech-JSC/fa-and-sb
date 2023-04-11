using MicroBase.Share.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MicroBase.Share.Models.Apps
{
    public class AppInfoModel : SubscribeModel
    {
        public Guid AccountId { get; set; }

        [Required]
        public string Platform { get; set; }

        [Required]
        public string Version { get; set; }

        private int intVersion;

        [JsonIgnore]
        public int IntVersion
        {
            get
            {
                intVersion = (int)Version.RemoveAllCharacter();
                return intVersion;
            }
            set
            {
                intVersion = value;
            }
        }

        [Required]
        public string DeviceId { get; set; }
    }
}