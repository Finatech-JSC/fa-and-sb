using System;
using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Models.Accounts
{
    public class RegisterModel
    {
        /// <summary>
        /// Username is EmailAddress or PhoneNumber
        /// </summary>
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string FullName { get; set; }

        /// <summary>
        /// Username is EmailAddress or PhoneNumber
        /// </summary>
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        [EmailAddress(ErrorMessage = CommonMessage.EMAIL_ADDRESS_INVALID)]
        public string UserName { get; set; }

        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string Password { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [Compare("Password", ErrorMessage = CommonMessage.Account.RE_PASSWORD_INVALID)]
        public string RePassword { get; set; }

        [MaxLength(50, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string FromReferralId { get; set; }
    }

    public class AccountRegisterResponse
    {
        public Guid AccountId { get; set; }

        public OtpResponse OtpResponse { get; set; }
    }

    public class AccountNotificationSettingModel
    {
        [MaxLength(50)]
        public string Language { get; set; }

        public bool AllowAppNotification { get; set; }

        public bool AllowEmailNotification { get; set; }
    }
}