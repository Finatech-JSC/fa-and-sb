using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBase.Entity
{
    [Table("EmailTemplates")]
    public class EmailTemplate : BaseEntity<Guid>
    {
        [Required]
        public string Key { get; set; }

        [Required, MaxLength(512)]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [Required, MaxLength(10)]
        public string CultureCode { get; set; }
    }
}