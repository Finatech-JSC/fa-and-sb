using MicroBase.Share.Models.CMS.RoboForm.UI;
using System;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public class LockAccountModel
    {
        [Required]
        public string Email { get; set; }

        public DateTime? LockedEndDate { get; set; }

        public string Description { get; set; }

        [Required]
        public bool IsSystemLock { get; set; }
    }

    public class LockAccountNftModel
    {
        public string Email { get; set; }

        public string NftName { get; set; }

        public DateTime? LockedEndDate { get; set; }

        public bool LockBuy { get; set; } = false;  

        public bool LockSell { get; set; } = false ;

        public bool LockBid { get; set; } = false ;

        public bool LockCreate { get; set; } = false;

        public string Description { get; set; }
    }

    public class LockAccountFromFileModel : BaseImportFromFileModel
    {
        [RoboText(LabelText = "Lý do khóa", Name = "Description", Order = 3)]
        public string Description { get; set; }

        [RoboDropDown(LabelText = "Chọn mẫu Email", FirstOptionLabel = "Chọn mẫu Email", Name = "EmailKeyCode", Order = 3)]
        public string EmailKeyCode { get; set; }
    }

    public class AccountLockImportFileModel
    {
        public int RowNumber { get; set; }

        public string Email { get; set; }

        public DateTime? LockTime { get; set; }

        public bool IsSystemLocked { get; set; }
    }

    public class AccountUnLockImportFileModel
    {
        public int RowNumber { get; set; }

        public string Email { get; set; }
    }

    public class Remove2FAModel
    {
        public string Email { get; set; }

        public bool Off2FAGoogle { get; set; }

        public bool Off2FAEmail { get; set; }
    }
}