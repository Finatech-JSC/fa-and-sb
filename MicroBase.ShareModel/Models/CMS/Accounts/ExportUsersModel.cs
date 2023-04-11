using System;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public class ExportUsersModel
    {
        public string Email { get; set; }

        [Display(Name = "Tên đầy đủ")]
        public string FullName { get; set; }

        [Display(Name = "Xác nhận Email")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Ngày xác nhận Email")]
        public DateTime? EmailConfirmDate { get; set; }

        public string PhoneNumber { get; set; }

        [Display(Name = "Đã xác nhận SĐT")]
        public bool? PhoneNumberConfirmed { get; set; }

        [Display(Name = "Ngày xác nhận SĐT")]
        public DateTime? PhoneConfirmDate { get; set; }

        [Display(Name = "Mã giới thiệu")]
        public string ReferralId { get; set; }

        [Display(Name = "Đăng nhập lần cuối")]
        public DateTime? LastLoginTime { get; set; }

        [Display(Name = "IP đăng nhập cuối")]
        public string LastLoginIpAddress { get; set; }

        [Display(Name = "Loại tài khoản")]
        public string AccountType { get; set; }

        [Display(Name = "Tỉnh")]
        public string Province { get; set; }

        [Display(Name = "Huyện")]
        public string District { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [Display(Name = "Email giới thiệu")]
        public string ReferralEmail { get; set; }
    }
}