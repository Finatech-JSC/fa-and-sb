using MicroBase.Share.Models.CMS.RoboForm.UI;
using System.Text.Json.Serialization;

namespace MicroBase.Share.Models.Accounts
{
    public class GoogleAuthenticatorModel : BaseModel
    {
        [RoboText(LabelText = "Issuer", Name = "Issuer", MaxLength = 512, IsRequired = true, Cols = 12, Order = 1)]
        public string Issuer { get; set; }

        [RoboText(LabelText = "Account Title No Spaces", Name = "AccountTitleNoSpaces", MaxLength = 255, Cols = 12, Order = 2)]
        public string AccountTitleNoSpaces { get; set; }
    }

    public class AccountGoogleAuthenticatorModel
    {
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("accountTitleNoSpaces")]
        public string AccountTitleNoSpaces { get; set; }

        [JsonPropertyName("secretKey")]
        public string SecretKey { get; set; }
    }

    public class GoogleAuthenConfigResponse
    {
        public string Account { get; set; }

        public string ManualEntryKey { get; set; }

        public string QrCodeSetupImageUrl { get; set; }

        public string Key { get; set; }
    }
}