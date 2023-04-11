using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Models.Accounts
{
    public class ChangePassowrdModel : AccountTrackingModel
    {
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string Password { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [Compare("Password", ErrorMessage = CommonMessage.Account.RE_PASSWORD_INVALID)]
        public string RePassword { get; set; }
    }
}