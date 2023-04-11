using System;
using System.ComponentModel.DataAnnotations;
using MicroBase.Share.Constants;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public class IdentityUserResponse : BaseModel
    {
        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int AccountType { get; set; }

        public string FullName { get; set; }

        public string ReferralId { get; set; }

        public string FromReferralId { get; set; }

        public string ReferralFromEmail { get; set; }

        public virtual string LastLoginIpAddress { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public int Via { get; set; }

        public string ViaInString { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool IsSystemLock { get; set; }

        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string Password { get; set; }

        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public virtual FromReferralUser FromReferralUser { get; set; }

        public IdentityUserMetaDataResponse IdentityUserMetaData { get; set; }
    }
}