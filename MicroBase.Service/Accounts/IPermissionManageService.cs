using MicroBase.Entity;
using MicroBase.Entity.Accounts;
using MicroBase.Entity.Repositories;
using MicroBase.Share.Constants;
using MicroBase.Share.Models;
using MicroBase.Share.Models.CMS.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Transactions;

namespace MicroBase.Service.Accounts
{
    public interface IPermissionManageService
    {
        void SyncPermissionsToDb(IEnumerable<AttrPermissionGroupModel> permissions);

        Task<BaseResponse<Guid>> GrantPermissionsToAccountAsync(Guid accountId,
            Guid? createdById,
            List<Guid> roleGroupIds,
            List<Guid> permissionIds);

        Task<IEnumerable<PermissionGroupResponse>> GetPermissionsByAccountIdAsync(Guid accountId,
            bool? fromDb = false);

        Task<BaseResponse<object>> AuthorizationAsync(Guid accountId, string actionCode);

        Task<IEnumerable<NameValueModel<Guid>>> GetAccountByRoleGroupCodeAsync(string roleGroupCode);
    }

    public class PermissionManageService : IPermissionManageService
    {
        private readonly MicroDbContext microDbContext;
        private readonly IRepository<IdentityUserACGroup, Guid> identityUserPermissionRepo;
        private readonly IConfiguration configuration;

        public PermissionManageService(MicroDbContext microDbContext,
            IRepository<IdentityUserACGroup, Guid> identityUserPermissionRepo,
            IConfiguration configuration)
        {
            this.microDbContext = microDbContext;
            this.identityUserPermissionRepo = identityUserPermissionRepo;
            this.configuration = configuration;
        }

        public void SyncPermissionsToDb(IEnumerable<AttrPermissionGroupModel> permissionsModel)
        {
            var permissionEntities = microDbContext.Set<IdentityUserRole>()
                .Where(s => true);

            var permissionCodes = new List<string>();
            foreach (var p in permissionsModel)
            {
                permissionCodes.AddRange(p.PermissionInfos.Select(s => s.Code));
            }

            // For new permissions
            var newPermissionEntities = new List<IdentityUserRole>();
            var newPermissions = permissionCodes.Where(s => !permissionEntities.Select(p => p.Code).Contains(s));
            foreach (var code in newPermissions)
            {
                foreach (var model in permissionsModel)
                {
                    var per = model.PermissionInfos.FirstOrDefault(s => s.Code == code);
                    if (per != null)
                    {
                        newPermissionEntities.Add(new IdentityUserRole
                        {
                            Name = per.Name,
                            Code = per.Code,
                            GroupName = model.Name,
                            GroupCode = model.Code,
                            HttpMethod = per.HttpMethod,
                            Route = per.Route,
                            BaseRoute = per.BaseRoute,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }
            }

            // For adjust permissions
            var updatePermissionEntities = permissionEntities.Where(s => permissionCodes.Contains(s.Code)).ToList();
            foreach (var item in updatePermissionEntities)
            {
                foreach (var p in permissionsModel)
                {
                    var per = p.PermissionInfos.FirstOrDefault(s => s.Code == item.Code);
                    if (per == null)
                    {
                        continue;
                    }

                    item.Name = per.Name;
                    item.Code = per.Code;
                    item.GroupName = p.Name;
                    item.GroupCode = p.Code;
                    item.HttpMethod = per.HttpMethod;
                    item.Route = per.Route;
                    item.BaseRoute = per.BaseRoute;
                    item.CreatedDate = DateTime.UtcNow;
                }
            }

            // For delete permissions
            var includeRoleGroups = configuration.GetSection("SuperAdminRoleGroup:IncludeRoleGroups").Get<List<string>>();
            var deletePermissionEntities = permissionEntities
                .Where(s => !permissionCodes.Contains(s.Code) && !includeRoleGroups.Contains(s.GroupCode));
            foreach (var item in deletePermissionEntities)
            {
                item.IsDelete = true;
            }

            updatePermissionEntities.AddRange(deletePermissionEntities);

            using (var scope = new TransactionScope())
            {
                if (newPermissionEntities.Any())
                {
                    microDbContext.Set<IdentityUserRole>().AddRange(newPermissionEntities);
                }

                if (updatePermissionEntities.Any())
                {
                    microDbContext.Set<IdentityUserRole>().UpdateRange(updatePermissionEntities);
                }

                microDbContext.SaveChanges();
                scope.Complete();
            }
        }

        public async Task<BaseResponse<Guid>> GrantPermissionsToAccountAsync(Guid accountId,
            Guid? createdById,
            List<Guid> roleGroupIds,
            List<Guid> permissionIds)
        {
            try
            {
                var newIdentityUserPermissionEntities = new List<IdentityUserACGroup>();
                var updateIdentityUserPermissionEntities = new List<IdentityUserACGroup>();

                var accountPermissionsEntites = await identityUserPermissionRepo.FindAsync(s => s.IdentityUserId == accountId
                    && !s.IsDelete);

                // Add new RoleGroups
                foreach (var groupId in roleGroupIds.Where(s => !accountPermissionsEntites.Select(p => p.RoleGroupId).Contains(s)))
                {
                    newIdentityUserPermissionEntities.Add(new IdentityUserACGroup
                    {
                        Id = Guid.NewGuid(),
                        IdentityUserId = accountId,
                        RoleGroupId = groupId,
                        RoleId = null,
                        CreatedBy = createdById,
                        CreatedDate = DateTime.UtcNow,
                        IsDelete = false
                    });
                }

                // Update RoleGroups
                updateIdentityUserPermissionEntities = accountPermissionsEntites
                    .Where(s => s.RoleGroupId.HasValue && !roleGroupIds.Contains(s.RoleGroupId.Value))
                    .ToList();

                foreach (var item in updateIdentityUserPermissionEntities)
                {
                    item.IsDelete = true;
                }

                if (permissionIds != null)
                {
                    // Add new Permissions
                    foreach (var permissionId in permissionIds.Where(s => !accountPermissionsEntites.Select(p => p.RoleId).Contains(s)))
                    {
                        newIdentityUserPermissionEntities.Add(new IdentityUserACGroup
                        {
                            Id = Guid.NewGuid(),
                            IdentityUserId = accountId,
                            RoleGroupId = null,
                            RoleId = permissionId,
                            CreatedBy = createdById,
                            CreatedDate = DateTime.UtcNow,
                            IsDelete = false
                        });
                    }
                }

                // Update Permissions
                updateIdentityUserPermissionEntities.AddRange(accountPermissionsEntites
                    .Where(s => s.RoleId.HasValue && !permissionIds.Contains(s.RoleId.Value))
                    .ToList());

                foreach (var item in updateIdentityUserPermissionEntities)
                {
                    item.IsDelete = true;
                }

                using (var scope = new TransactionScope())
                {
                    if (newIdentityUserPermissionEntities.Any())
                    {
                        microDbContext.Set<IdentityUserACGroup>().AddRange(newIdentityUserPermissionEntities);
                    }

                    if (updateIdentityUserPermissionEntities.Any())
                    {
                        microDbContext.Set<IdentityUserACGroup>().UpdateRange(updateIdentityUserPermissionEntities);
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PermissionGroupResponse>> GetPermissionsByAccountIdAsync(Guid accountId,
            bool? fromDb = false)
        {
            var query = await microDbContext.Set<IdentityUserACGroup>()
                .Include(s => s.IdentityUserRoleGroup)
                .ThenInclude(s => s.IdentityUserRoleGroupMaps)
                .ThenInclude(s => s.IdentityUserRole)
                .Where(s => !s.IsDelete
                    && !s.IdentityUserRoleGroup.IsDelete
                    && s.IdentityUserRoleGroup.Enabled
                    && s.IdentityUserId == accountId)
                .ToListAsync();

            var res = new List<PermissionGroupResponse>();
            foreach (var gr in query.GroupBy(s => s.RoleGroupId))
            {
                var group = query.FirstOrDefault(s => s.RoleGroupId == gr.Key);
                var permissionInfo = new PermissionGroupResponse
                {
                    Id = group.IdentityUserRoleGroup.Id,
                    Name = group.IdentityUserRoleGroup.Name,
                    AllowFullAccess = group.IdentityUserRoleGroup.AllowFullAccess,
                    PermissionInfos = new List<PermissionInfoResponse>()
                };

                foreach (var s in gr)
                {
                    if (s.IdentityUserRoleGroup != null)
                    {
                        foreach (var r in s.IdentityUserRoleGroup.IdentityUserRoleGroupMaps.Where(s => !s.IsDelete))
                        {
                            permissionInfo.PermissionInfos.Add(new PermissionInfoResponse
                            {
                                Id = r.RoleId,
                                Code = r.IdentityUserRole.Code,
                                GroupCode = r.IdentityUserRole.GroupCode,
                                HttpMethod = r.IdentityUserRole.HttpMethod,
                                Name = r.IdentityUserRole.Name,
                                Route = r.IdentityUserRole.Route
                            });
                        }
                    }
                    else
                    {
                        permissionInfo.PermissionInfos.Add(new PermissionInfoResponse
                        {
                            Id = s.RoleId.Value,
                            Code = s.IdentityUserRole?.Code,
                            GroupCode = s.IdentityUserRole?.GroupCode,
                            HttpMethod = s.IdentityUserRole?.HttpMethod,
                            Name = s.IdentityUserRole?.Name,
                            Route = s.IdentityUserRole?.Route
                        });
                    }
                }

                res.Add(permissionInfo);
            }

            return res;
        }

        public async Task<BaseResponse<object>> AuthorizationAsync(Guid accountId, string actionCode)
        {
            var roleGroups = await GetPermissionsByAccountIdAsync(accountId, false);
            if (roleGroups.Any(s => s.AllowFullAccess))
            {
                return new BaseResponse<object>
                {
                    Code = 200,
                    Success = true
                };
            }

            foreach (var item in roleGroups)
            {
                if (item.PermissionInfos.Any(s => s.Code == actionCode))
                {
                    return new BaseResponse<object>
                    {
                        Code = 200,
                        Success = true
                    };
                }
            }

            return new BaseResponse<object>
            {
                Code = 401,
                Success = false,
                Message = "Unauthorization"
            };
        }

        public async Task<IEnumerable<NameValueModel<Guid>>> GetAccountByRoleGroupCodeAsync(string roleGroupCode)
        {
            if (string.IsNullOrWhiteSpace(roleGroupCode))
            {
                return new List<NameValueModel<Guid>>();
            }

            var userRoles = await microDbContext.Set<IdentityUserRole>()
                .Where(s => !s.IsDelete && s.GroupCode == roleGroupCode.ToUpper())
                .ToListAsync();

            var query = await microDbContext.Set<IdentityUserACGroup>()
                .Include(s => s.IdentityUser)
                .Include(s => s.IdentityUserRoleGroup)
                .Where(s => s.IdentityUser.IsDelete == false
                    && s.IdentityUserRoleGroup.IsDelete == false
                    && s.IsDelete== false
                    && (userRoles.Select(r => r.Id).Contains(s.RoleId.Value) || s.IdentityUserRoleGroup.AllowFullAccess))
                .ToListAsync();

            return query.DistinctBy(s => s.IdentityUserId).Select(s => new NameValueModel<Guid>
            {
                Value = s.IdentityUserId,
                Name = s.IdentityUser.Email
            });
        }
    }
}