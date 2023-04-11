using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Models.Accounts
{
    public class ResetPasswordModel : AccountTrackingModel
    {
        /// <summary>
        /// Username is EmailAddress or PhoneNumber
        /// </summary>
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string UserName { get; set; }
    }
}