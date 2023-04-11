using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Models.Accounts
{
    public class Web3RegisterModel
    {
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string WalletAddress { get; set; }
        
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string WalletProvider { get; set; }
        
        [Required(ErrorMessage = CommonMessage.REQUIRED_FIELD)]
        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string Network { get; set; }

        [MaxLength(50, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string FromReferralId { get; set; }
    }
}