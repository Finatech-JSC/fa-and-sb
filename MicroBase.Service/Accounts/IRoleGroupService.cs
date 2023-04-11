using AutoMapper;
using MicroBase.Entity;
using MicroBase.Entity.Accounts;
using MicroBase.Entity.Repositories;
using MicroBase.Share.Constants;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Extensions;
using MicroBase.Share.Models;
using MicroBase.Share.Models.CMS.Permissions;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace MicroBase.Service.Accounts
{
    public interface IRoleGroupService : IGenericService<IdentityUserRoleGroup, Guid>
    {
        Task<TPaging<RoleGroupRoboModel>> GetAvailableAsync(int pageIndex, int pageSize);

        Task<IEnumerable<RoleGroupRoboModel>> GetRoleGroupsAsync();

        Task<IEnumerable<PermissionResponse>> GetPermissionByGroupIdAsync(Guid groupId);

        Task<BaseResponse<Guid>> AddOrUpdateRoleGroupAsync(Guid? groupId,
            RoleGroupRequest model,
            Guid? createById);

        Task<BaseResponse<Guid>> AddPermissionsToRoleGroupAsync(Guid groupId,
            Guid? createById,
            IEnumerable<Guid> permissionIds);
    }

    public class RoleGroupService : GenericService<IdentityUserRoleGroup, Guid>, IRoleGroupService
    {
        private readonly MicroDbContext microDbContext;
        private readonly IRepository<IdentityUserRoleGroupMap, Guid> rolePermissionRepo;
        private readonly IMapper mapper;

        public RoleGroupService(IRepository<IdentityUserRoleGroup, Guid> repository,
            MicroDbContext microDbContext,
            IRepository<IdentityUserRoleGroupMap, Guid> rolePermissionRepo,
            IMapper mapper)
            : base(repository)
        {
            this.microDbContext = microDbContext;
            this.rolePermissionRepo = rolePermissionRepo;
            this.mapper = mapper;
        }

        protected override void ApplyDefaultSort(FindOptions<IdentityUserRoleGroup> findOptions)
        {
            findOptions.SortDescending(s => s.Name);
        }

        public async Task<TPaging<RoleGroupRoboModel>> GetAvailableAsync(int pageIndex, int pageSize)
        {
            var source = await Repository.FindAsync(s => !s.IsDelete, findOptions: new FindOptions<IdentityUserRoleGroup>
            {
                Skip = (pageIndex - 1) * pageSize,
                Limit = pageSize
            }.SortAscending(s => s.Name));

            var rows = await Repository.CountAsync(s => !s.IsDelete);
            return new TPaging<RoleGroupRoboModel>
            {
                Source = source.Select(s => new RoleGroupRoboModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    AllowFullAccess = s.AllowFullAccess,
                    IsDefault = s.IsDefault,
                    Enabled = s.Enabled,
                    CreatedDate = s.CreatedDate.UtcToVietnamTime(),
                    ModifiedDate = s.ModifiedDate.UtcToVietnamTime()
                }),
                TotalRecords = rows
            };
        }

        public async Task<IEnumerable<PermissionResponse>> GetPermissionByGroupIdAsync(Guid groupId)
        {
            var roleGroup = await microDbContext.Set<IdentityUserRoleGroup>()
                .Include(s => s.IdentityUserRoleGroupMaps)
                .ThenInclude(s => s.IdentityUserRole)
                .Where(s => s.Id == groupId
                    && !s.IsDelete)
                .FirstOrDefaultAsync();

            var permissions = new List<PermissionResponse>();
            foreach (var item in roleGroup.IdentityUserRoleGroupMaps.Where(s => !s.IsDelete))
            {
                var p = item.IdentityUserRole;
                permissions.Add(new PermissionResponse
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    Route = p.Route,
                    GroupCode = p.GroupCode,
                    GroupName = p.GroupName,
                    HttpMethod = p.HttpMethod
                });
            }

            return permissions;
        }

        public async Task<BaseResponse<Guid>> AddOrUpdateRoleGroupAsync(Guid? groupId,
            RoleGroupRequest model,
            Guid? createById)
        {
            var entity = new IdentityUserRoleGroup();
            if (groupId.HasValue)
            {
                entity = await GetByIdAsync(groupId.Value);
                entity.Name = model.Name;
                entity.IsDefault = model.IsDefault;
                entity.ModifiedBy = createById;
                entity.ModifiedDate = DateTime.UtcNow;
                entity.AllowFullAccess = model.AllowFullAccess;
                entity.IsDelete = false;
                entity.Enabled = model.Enabled;

                await UpdateAsync(entity);
            }
            else
            {
                entity = new IdentityUserRoleGroup
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    IsDefault = model.IsDefault,
                    CreatedBy = createById,
                    CreatedDate = DateTime.UtcNow,
                    AllowFullAccess = model.AllowFullAccess,
                    IsDelete = false,
                    Enabled = model.Enabled
                };

                await InsertAsync(entity);
            }

            var permissionIds = new List<Guid>();
            foreach (var item in model.PermissionGroups)
            {
                permissionIds.AddRange(item.PermissionInfos.Where(s => s.Id != Guid.Empty).Select(s => s.Id));
            }

            await AddPermissionsToRoleGroupAsync(entity.Id, createById, permissionIds);
            return new BaseResponse<Guid>
            {
                Success = true,
                Message = CommonMessage.INSERT_SUCCESS,
                Data = entity.Id
            };
        }

        public async Task<BaseResponse<Guid>> AddPermissionsToRoleGroupAsync(Guid groupId,
            Guid? createById,
            IEnumerable<Guid> permissionIds)
        {
            var roleGroupPermissions = await rolePermissionRepo.FindAsync(s => s.RoleGroupId == groupId && !s.IsDelete);

            var newRolePermissionEntities = new List<IdentityUserRoleGroupMap>();
            var permissions = permissionIds.Where(s => !roleGroupPermissions.Select(p => p.RoleId).Contains(s));
            foreach (var id in permissions)
            {
                newRolePermissionEntities.Add(new IdentityUserRoleGroupMap
                {
                    RoleGroupId = groupId,
                    RoleId = id,
                    CreatedBy = createById,
                    CreatedDate = DateTime.UtcNow
                });
            }

            var deleteRolePermissionEntities = roleGroupPermissions.Where(s => !permissionIds.Contains(s.RoleId));
            foreach (var item in deleteRolePermissionEntities)
            {
                item.IsDelete = true;
            }

            using (var scope = new TransactionScope())
            {
                if (newRolePermissionEntities.Any())
                {
                    microDbContext.Set<IdentityUserRoleGroupMap>().AddRange(newRolePermissionEntities);
                }

                if (deleteRolePermissionEntities.Any())
                {
                    microDbContext.Set<IdentityUserRoleGroupMap>().UpdateRange(deleteRolePermissionEntities);
                }

                microDbContext.SaveChanges();
                scope.Complete();
            }

            return new BaseResponse<Guid>
            {
                Success = true,
                Message = CommonMessage.UPDATE_SUCCESS
            };
        }

        public async Task<IEnumerable<RoleGroupRoboModel>> GetRoleGroupsAsync()
        {
            var res = await Repository.FindAsync(s => !s.IsDelete && s.Enabled);
            return res.Select(s => mapper.Map<RoleGroupRoboModel>(s));
        }
    }
}