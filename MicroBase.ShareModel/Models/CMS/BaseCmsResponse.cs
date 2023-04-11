namespace MicroBase.Share.Models.CMS
{
    public class BaseCmsResponse<T> : BaseResponse<T>
    {
        public string RedirectUrl { get; set; }
    }
}