using MicroBase.Entity;
using MicroBase.Share.Constants;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroBase.BaseApi.Apis
{
    public abstract class BaseTrackingCrudApiController<TEntity, TKey, TEntityDto> : ControllerBase
        where TEntity : IBaseEntity<TKey>
        where TEntityDto : BaseModel
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public BaseTrackingCrudApiController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected TEntityDto SetCrudTrackingUser(TEntityDto entityDto, OperationActionConstants.CrudAction crudAction)
        {
            var accountId = httpContextAccessor.HttpContext.User.GetAccountId();

            if (crudAction == OperationActionConstants.CrudAction.CREATE)
            {
                entityDto.CreatedBy = accountId;
                entityDto.CreatedDate = DateTime.UtcNow.UtcToVietnamTime();
            }
            else
            {
                entityDto.ModifiedBy = accountId;
                entityDto.ModifiedDate = DateTime.UtcNow.UtcToVietnamTime();
            }

            return entityDto;
        }
    }
}