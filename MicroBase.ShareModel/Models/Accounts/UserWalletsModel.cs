using System;
namespace MicroBase.Share.Models.Accounts
{
    public class UserWalletsModel
    {
        public Guid WalletId { get; set; }

        public string Email { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Currency { get; set; }

        public decimal Available { get; set; }

        public decimal Pending { get; set; }

        public decimal AppAmount { get; set; }

        public decimal BlockchainAmount { get; set; }
    }
}