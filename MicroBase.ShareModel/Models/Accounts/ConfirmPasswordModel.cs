using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Models.Accounts
{
    public class ConfirmPasswordModel : AccountTrackingModel
    {
        /// <summary>
        /// Username is EmailAddress or PhoneNumber
        /// </summary>
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string UserName { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string Password { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [Compare("Password", ErrorMessage = CommonMessage.Account.RE_PASSWORD_INVALID)]
        public string RePassword { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string ValidateToken { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string Otp { get; set; }
    }
}