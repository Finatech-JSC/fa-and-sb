using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MicroBase.Share.Models.Accounts
{
    public class AccountProfileModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string AccountType { get; set; }

        public string Via { get; set; }

        public DateTime? LatestLoginTime { get; set; }

        public string LastLoginIpAddress { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Fullname { get; set; }

        public string Avatar { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsDefaultPassword { get; set; }

        public sbyte Gender { get; set; }

        public bool IsConfirmed { get; set; }

        public IEnumerable<TwoFaResponse> TwoFactorServices { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime? EmailConfirmDate { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public DateTime? PhoneConfirmDate { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public bool? LockoutEnabled { get; set; }

        public string LockedDescription { get; set; }

        public bool? IsSystemLocked { get; set; }

        public bool IsFirstTimeLogin { get; set; }

        [JsonIgnore]
        public IEnumerable<TwoFaSetting> TwoFASettings { get; set; }
    }
}