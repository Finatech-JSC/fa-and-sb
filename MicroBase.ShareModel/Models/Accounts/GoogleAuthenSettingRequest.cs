using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Models.Accounts
{
    public class TwoFAModel
    {
        public IEnumerable<TwoFaRequestModel> TwoFaRequestModels { get; set; }
    }

    public class GoogleAuthenSettingRequest : TwoFAModel
    {
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [StringLength(50, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string GoogleSecretKey { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        public string VerifyCode { get; set; }
    }

    public class TwoFaGoogleModel : TwoFAModel
    {
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        public string VerifyCode { get; set; }
    }

    public class TwoFaEmailModel : TwoFAModel
    {
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        public string SessionId { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        public string Token { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        public string VerifyCode { get; set; }
    }

    public class RegisterTwoFaEmailModel
    {
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        public string Email { get; set; }
    }

    public class RegisterTwoFaEmailToCacheModel
    {
        public string Email { get; set; }

        public Guid AccountId { get; set; }
    }
}