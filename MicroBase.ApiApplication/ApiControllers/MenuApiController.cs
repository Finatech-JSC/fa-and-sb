using MicroBase.BaseApi.Apis;
using MicroBase.Entity.Accounts;
using MicroBase.NoDependencyService;
using MicroBase.Service.Foundations;
using MicroBase.Share.Models.CMS.Accounts;
using MicroBase.Share.Models.CMS.Menus;
using Microsoft.AspNetCore.Mvc;

namespace MicroBase.ApiApplication.ApiControllers
{
    [ApiController]
    [Route("menu-api")]
    public class MenuApiController : CrudImExportBaseApiController<SystemMenu, Guid, SystemMenuModel, SystemMenuResponse, MenuExcelModel>
    {
        public MenuApiController(IHttpContextAccessor httpContextAccessor,
            ICrudAppService<SystemMenu, Guid, SystemMenuModel, SystemMenuResponse> crudAppService,
            IDataGridService dataGridService) : base(httpContextAccessor, crudAppService, dataGridService)
        {
        }
    }
}