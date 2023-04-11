using MicroBase.Share.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Share.Models.Accounts
{
    public class UpdateAccountProfileModel : AccountTrackingModel
    {
        public sbyte Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(255, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string Address { get; set; }

        public string CountryCode { get; set; }

        public Guid? ProvinceId { get; set; }

        public Guid? DistrictId { get; set; }

        [MaxLength(50, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string Fullname { get; set; }

        [MaxLength(20, ErrorMessage = CommonMessage.MAX_LENGTH)]
        public string PhoneNumber { get; set; }

        public IFormFile Avatar { get; set; }

        public string ReferralId { get; set; }
    }

    public class UpdateAccountBackground
    {
        public IFormFile BackgroundImage { get; set; }

        public string BackgroundUrl { get; set; }
    }
}