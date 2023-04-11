using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Models.Accounts
{
    public class SocialLoginModel : AccountTrackingModel
    {
        /// <summary>
        /// Open Id: Googole, Facebook, Apple
        /// </summary>
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string ProviderName { get; set; }

        /// <summary>
        /// JWT login token response from option Id
        /// </summary>
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        public string Token { get; set; }

        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(50, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string ExternalId { get; set; }

        [EmailAddress(ErrorMessage = CommonMessage.EMAIL_ADDRESS_INVALID)]
        public string Email { get; set; }

        public string FullName { get; set; }

        public string Avatar { get; set; }
    }
}