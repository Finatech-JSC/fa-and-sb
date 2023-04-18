using MicroBase.Entity;
using MicroBase.Entity.Accounts;
using MicroBase.Entity.Repositories;
using MicroBase.Share.DataAccess;

namespace MicroBase.Service.Accounts
{
    public interface IRolePermissionService : IGenericService<PrivilegesRoleGroupMap, Guid>
    {
    }

    public class RolePermissionService : GenericService<PrivilegesRoleGroupMap, Guid>, IRolePermissionService
    {
        public RolePermissionService(IRepository<PrivilegesRoleGroupMap, Guid> repository) : base(repository)
        {
        }

        protected override void ApplyDefaultSort(FindOptions<PrivilegesRoleGroupMap> findOptions)
        {
            findOptions.SortDescending(s => s.GroupId);
        }
    }
}