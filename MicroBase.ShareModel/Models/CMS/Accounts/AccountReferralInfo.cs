using System;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public class AccountReferralInfo
    {
        public Guid AccountId { get; set; }

        public string Email { get; set; }

        public string ReferralId { get; set; }

        public int NoOfReferee { get; set; }

        public DateTime DateTime { get; set; }
    }
}