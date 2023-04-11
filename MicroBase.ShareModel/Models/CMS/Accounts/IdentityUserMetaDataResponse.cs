using Microsoft.AspNetCore.Http;
using System;

namespace MicroBase.Share.Models.CMS.Accounts
{
    public class IdentityUserMetaDataResponse : BaseModel
    {
        public Guid IdentityUserId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Address { get; set; }

        public Guid? DistrictId { get; set; }

        public Guid? ProvinceId { get; set; }

        public string Avatar { get; set; }

        public IFormFile AvatarFromForm { get; set; }

        public string Gender { get; set; }

        public string IdCardNumber { get; set; }

        public string HomeTownAddress { get; set; }

        public string HomeTownProvince { get; set; }

        public string HomeTownDistrict { get; set; }

        public string HomeTownWard { get; set; }

        public DateTime? IdCardIssueDate { get; set; }

        public string IdCardIssueLocation { get; set; }

        public DateTime? IdCardExpiredDate { get; set; }
    }
}
