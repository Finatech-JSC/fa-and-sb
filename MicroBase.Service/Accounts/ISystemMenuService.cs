using MicroBase.Entity;
using MicroBase.Entity.Accounts;
using MicroBase.Entity.Repositories;
using MicroBase.Service.Localizations;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Models.CMS.Accounts;
using MicroBase.Share.Models.CMS.Permissions;

namespace MicroBase.Service.Accounts
{
    public interface ISystemMenuService : IGenericService<SystemMenu, Guid>
    {
        Task<IEnumerable<SystemMenuResponse>> GetAvailableAsync();

        Task<IEnumerable<SystemMenuResponse>> GetMenusByAccountIdAsync(Guid accountId);
    }

    public class SystemMenuService : GenericService<SystemMenu, Guid>, ISystemMenuService
    {
        private readonly IPermissionManageService permissionManageService;

        public SystemMenuService(IRepository<SystemMenu, Guid> repository,
            IPermissionManageService permissionManageService) : base(repository)
        {
            this.permissionManageService = permissionManageService;
        }

        protected override void ApplyDefaultSort(FindOptions<SystemMenu> findOptions)
        {
            findOptions.SortDescending(s => s.Name);
        }


        public async Task<IEnumerable<SystemMenuResponse>> GetAvailableAsync()
        {
            var menus = await Repository.FindAsync(s => !s.IsDelete);
            var res = new List<SystemMenuResponse>();

            foreach (var item in menus)
            {
                res.Add(new SystemMenuResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    FontIcon = item.FontIcon,
                    Route = item.Route,
                    Target = item.Target,
                    ParentId = item.ParentId,
                    DisplayOrder = item.DisplayOrder,
                    Enabled = item.Enabled
                });
            }

            return res.OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.ParentId)
                .ThenBy(s => s.DisplayOrder);
        }

        public async Task<IEnumerable<SystemMenuResponse>> GetMenusByAccountIdAsync(Guid accountId)
        {
            var permissionGroups = await permissionManageService.GetPermissionsByAccountIdAsync(accountId);

            IEnumerable<SystemMenu> menus = new List<SystemMenu>();
            if (permissionGroups.Any(p => p.AllowFullAccess))
            {
                menus = await Repository.FindAsync(s => s.Enabled && !s.IsDelete);
            }
            else
            {
                var permissions = new List<PermissionInfoResponse>();
                foreach (var permission in permissionGroups)
                {
                    permissions.AddRange(permission.PermissionInfos);
                }

                var distinct = permissions.DistinctBy(s => s.GroupCode);
                menus = await Repository.FindAsync(s => (distinct.Select(p => p.GroupCode).Contains(s.Code) || string.IsNullOrWhiteSpace(s.Code))
                    && s.Enabled
                    && !s.IsDelete);
            }

            var menusRes = new List<SystemMenuResponse>();
            foreach (var item in menus.OrderBy(s => s.DisplayOrder))
            {
                if (item.ParentId.HasValue && item.ParentId.Value != Guid.Empty)
                {
                    continue;
                }

                menusRes.Add(new SystemMenuResponse
                {
                    Id = item.Id,
                    DisplayOrder = item.DisplayOrder,
                    Enabled = item.Enabled,
                    FontIcon = item.FontIcon,
                    ImageUrlIcon = item.ImageUrlIcon,
                    Code = item.Code,
                    Name = FileLocalizationService.GetLocalizationString(item.Name),
                    ParentId = item.ParentId,
                    Route = item.Route,
                    Target = item.Target,
                    CssClass = item.CssClass,
                    SubMenus = menus.Where(m => m.ParentId == item.Id)
                        .Select(m => new SubSystemMenuResponse
                        {
                            Id = m.Id,
                            DisplayOrder = m.DisplayOrder,
                            Enabled = m.Enabled,
                            FontIcon = m.FontIcon,
                            ImageUrlIcon = m.ImageUrlIcon,
                            Code = m.Code,
                            Name = FileLocalizationService.GetLocalizationString(m.Name),
                            ParentId = m.ParentId,
                            Route = m.Route,
                            Target = m.Target,
                            CssClass = m.CssClass
                        }).OrderBy(m => m.DisplayOrder)
                        .ToList()
                });
            }

            return menusRes.Where(s => !string.IsNullOrWhiteSpace(s.Code) || (string.IsNullOrWhiteSpace(s.Code) && s.SubMenus.Any()));
        }
    }
}