using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;
using static MicroBase.Share.Constants.Constants;

namespace MicroBase.Share.Models.Accounts
{
    public class TwoFaRequestModel
    {
        public string ValidateToken { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string Otp { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string TwoFaService { get; set; }
    }

    public class TwoFaResponse
    {
        public string EmailRegistration { get; set; }

        public string TwoFaServiceCode { get; set; }

        public string TwoFaServiceName { get; set; }
    }

    public class TwoFaSetting
    {
        public string TwoFaServiceCode { get; set; }

        public string Setting { get; set; }
    }

    public class RegisterTwoFaEmailResponseModel : OtpResponse
    {
        public string SessionId { get; set; }

        public string EmailRegistration { get; set; }
    }

    public class OtpResponse
    {
        public OtpType OtpType { get; set; }

        public string Token { get; set; }

        public int SendTo { get; set; }
    }
}