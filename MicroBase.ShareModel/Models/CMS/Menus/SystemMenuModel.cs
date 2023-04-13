using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Share.Models.CMS.Menus
{
    public class SystemMenuModel : BaseModel
    {
        public Guid? ParentId { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string FontIcon { get; set; }

        public IFormFile IconFile { get; set; }

        [Required, MaxLength(255)]
        public string Route { get; set; }

        [MaxLength(255)]
        public string Target { get; set; }

        [Required]
        public int DisplayOrder { get; set; }
    }
}