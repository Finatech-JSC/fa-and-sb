using System;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Share.Models
{
    public class BaseApiModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class BasePagingModel
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}