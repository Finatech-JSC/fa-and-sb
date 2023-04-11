namespace MicroBase.Share.Models
{
    public class SiteSettingResponse : BaseModel
    {
        public string GroupKey { get; set; }

        public string Key { get; set; }

        public string StringValue { get; set; }

        public bool? BoolValue { get; set; }

        public decimal? NumberValue { get; set; }

        public int? Order { get; set; }

        public string Description { get; set; }

        public string ModelFields { get; set; }

        public bool ModelFieldIsArray { get; set; }

        public bool IsSecret { get; set; }
    }
}