using MicroBase.Share.Models.CMS.RoboForm.UI;
using Newtonsoft.Json;
using System;

namespace MicroBase.Share.Models
{
    public abstract class BaseModel
    {
        [RoboText(IsHidden = true, Name = "Id")]
        public virtual Guid? Id { get; set; }

        [JsonIgnore]
        public virtual DateTime? CreatedDate { get; set; }
        
        [JsonIgnore]
        public virtual Guid? CreatedBy { get; set; }
        
        [JsonIgnore]
        public virtual DateTime? ModifiedDate { get; set; }
        
        [JsonIgnore]
        public virtual Guid? ModifiedBy { get; set; }
    }

    public abstract class BaseDto
    {
        public virtual Guid? Id { get; set; }
    }
}