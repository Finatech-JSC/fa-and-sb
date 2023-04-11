using System;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public interface IBaseCmsIdentityUser
    {
        public Guid AccountId { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }
    }
}
