using System.Collections.Generic;

namespace MicroBase.Share.Models.DataGirds
{
    public class BaseFileGirdModel : BaseModel
    {
        public bool IsValidData  { get; set; }

        public List<string> ErrorMessages { get; set; }
    }

    public class ExcelFieldMapping
    {
        /// <summary>
        /// The name of a excel column want to read
        /// </summary>
        public string ExcelColumn { get; set; }

        /// <summary>
        /// The name of the entity object will be set to value by ExcelColumn
        /// </summary>
        public string EntityColumn { get; set; }
    }
}