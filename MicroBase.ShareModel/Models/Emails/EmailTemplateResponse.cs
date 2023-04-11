namespace MicroBase.Share.Models.Emails
{
    public class EmailTemplateResponse : BaseModel
    {
        public string Key { get; set; }

        public string KeyLabel { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string CultureCode { get; set; }
    }
}
