using MicroBase.Share.Extensions;
using System;
namespace MicroBase.Share.Models.Accounts
{
    public class InvitedUserModel
    {
        public int Index { get; set; }

        private string email;

        public string Email
        {
            get { return email; }
            set { email = StringExtension.MaskEmail(value); }
        }

        public DateTime CreatedDate { get; set; }

        public string FullName { get; set; }
    }
}