using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Models.Accounts
{
    public class LoginModel : BaseActivityTrackingModel
    {
        /// <summary>
        /// Username is EmailAddress or PhoneNumber
        /// </summary>
        [Required(ErrorMessage = CommonMessage.REQUIRED_USERNAME_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string UserName { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_PASSWORD_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string Password { get; set; }
    }
}