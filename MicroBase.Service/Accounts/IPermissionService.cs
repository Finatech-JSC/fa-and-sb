using MicroBase.Entity;
using MicroBase.Entity.Accounts;
using MicroBase.Entity.Repositories;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Models.CMS.Permissions;

namespace MicroBase.Service.Accounts
{
    public interface IPermissionService : IGenericService<IdentityUserRole, Guid>
    {
        Task<IEnumerable<PermissionGroupResponse>> GetSystemPermissionsAsync();
    }

    public class PermissionService : GenericService<IdentityUserRole, Guid>, IPermissionService
    {
        public PermissionService(IRepository<IdentityUserRole, Guid> repository) : base(repository)
        {
        }

        protected override void ApplyDefaultSort(FindOptions<IdentityUserRole> findOptions)
        {
            findOptions.SortDescending(s => s.Name);
        }


        public async Task<IEnumerable<PermissionGroupResponse>> GetSystemPermissionsAsync()
        {
            var permissions = await Repository.FindAsync(s => !s.IsDelete);
            var res = new List<PermissionGroupResponse>();

            foreach (var item in permissions.GroupBy(s => s.GroupCode))
            {
                var gr = permissions.FirstOrDefault(s => s.GroupCode == item.Key);
                res.Add(new PermissionGroupResponse
                {
                    Code = gr.GroupCode,
                    Name = gr.GroupName,
                    PermissionInfos = item.Select(s => new PermissionInfoResponse
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code = s.Code,
                        Route = s.Route,
                        HttpMethod = s.HttpMethod,
                        GroupCode = s.GroupCode
                    }).OrderBy(s => s.Name).ToList()
                });
            }

            return res;
        }
    }
}