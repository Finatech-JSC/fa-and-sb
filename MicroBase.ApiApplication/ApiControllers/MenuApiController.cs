using MicroBase.BaseApi.Apis;
using MicroBase.Entity.Accounts;
using MicroBase.Service.Foundations;
using MicroBase.Share.Models.CMS.Accounts;
using MicroBase.Share.Models.CMS.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace MicroBase.ApiApplication.ApiControllers
{
    [ApiController]
    [Route("menu-api")]
    public class MenuApiController : CrudBaseApiController<SystemMenu, Guid, SystemMenuModel, SystemMenuResponse>
    {
        public MenuApiController(IHttpContextAccessor httpContextAccessor, ICrudAppService<SystemMenu, Guid, SystemMenuModel, SystemMenuResponse> crudAppService) 
            : base(httpContextAccessor, crudAppService)
        {
        }
    }
}