using Newtonsoft.Json;

namespace MicroBase.Share.Models.Accounts
{
    public class FacebookUserModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        public Picture Picture { get; set; }
    }

    public class Picture
    {
        public PictureData Data { get; set; }
    }

    public class PictureData
    {
        public string Url { get; set; }
    }
}