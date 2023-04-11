namespace MicroBase.Share.Models.CMS.Permissions
{
    public class PermissionResponse : BaseModel
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string GroupName { get; set; }

        public string GroupCode { get; set; }

        public string HttpMethod { get; set; }

        public string Route { get; set; }

        public string Description { get; set; }
    }
}