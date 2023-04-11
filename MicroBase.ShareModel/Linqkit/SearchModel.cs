using MicroBase.Share.Constants;
using System.ComponentModel.DataAnnotations;

namespace MicroBase.Share.Linqkit
{
    public class SearchTermModel
    {
        [Required]
        public string FieldName { get; set; }
        
        [Required]
        public string FieldValue { get; set; }
        
        [Required]
        public string Condition { get; set; }
    }
}