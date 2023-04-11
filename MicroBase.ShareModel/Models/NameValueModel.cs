namespace MicroBase.Share.Models
{
    public class NameValueModel<ValueType>
    {
        public ValueType Value { get; set; }

        public string Name { get; set; }
    }
}