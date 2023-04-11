using MicroBase.Entity;
using MicroBase.Entity.Accounts;
using MicroBase.Entity.Repositories;
using MicroBase.Share.DataAccess;

namespace MicroBase.Service.Accounts
{
    public interface IRolePermissionService : IGenericService<IdentityUserRoleGroupMap, Guid>
    {
    }

    public class RolePermissionService : GenericService<IdentityUserRoleGroupMap, Guid>, IRolePermissionService
    {
        public RolePermissionService(IRepository<IdentityUserRoleGroupMap, Guid> repository) : base(repository)
        {
        }

        protected override void ApplyDefaultSort(FindOptions<IdentityUserRoleGroupMap> findOptions)
        {
            findOptions.SortDescending(s => s.RoleGroupId);
        }
    }
}