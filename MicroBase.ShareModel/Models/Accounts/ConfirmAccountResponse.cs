using System;

namespace MicroBase.Share.Models.Accounts
{
    public class ConfirmAccountResponse
    {
        public Guid IdentityUserId { get; set; }

        public string DefaultLanguage { get; set; }

        public string ReferralId { get; set; }

        public Guid? ReferralAccountId { get; set; }

        public string ReferralAccountDefaultLanguage { get; set; }

        public Guid WalletId { get; set; }
    }
}