using AutoMapper;
using MicroBase.Entity;
using MicroBase.Entity.Accounts;
using MicroBase.Entity.Localtions;
using MicroBase.Entity.Notifications;
using MicroBase.Share.Models.CMS.Accounts;
using MicroBase.Share.Models.CMS.Locations;
using MicroBase.Share.Models.CMS.Menus;
using MicroBase.Share.Models.CMS.Permissions;
using MicroBase.Share.Models.Localizations.Localizations;
using MicroBase.Share.Models.Notifications;
using static MicroBase.Share.Constants.Constants;

namespace MicroBase.Service.AutoMap
{
    public class IdMappingProfile : Profile
    {
        public IdMappingProfile()
        {
            CreateMap<IdentityUser, IdentityUserResponse>();
            CreateMap<IdentityUserMetaData, IdentityUserMetaDataResponse>();
            CreateMap<IdentityUser, SystemAccountResponse>();
            CreateMap<IdentityUserRoleGroup, RoleGroupRoboModel>();

            CreateMap<IdentityUserCmsModel, IdentityUser>();
            CreateMap<IdentityUser, IdentityUserCmsModel>();

            CreateMap<IdentityUser, UpdateAccountModel>();
            CreateMap<UpdateAccountModel, IdentityUser>();
        }
    }

    public class BaseSolutionMappingProfile : Profile
    {
        public BaseSolutionMappingProfile()
        {
            CreateMap<LocalizationKey, LocalizationModel>();
            CreateMap<LocalizationModel, LocalizationKey>();

            CreateMap<NotificationSetting, NotificationSettingModel>();
            CreateMap<NotificationSettingModel, NotificationSetting>();

            CreateMap<SiteSettingModel, SiteSetting>();
            CreateMap<SiteSetting, SiteSettingModel>();

            CreateMap<ProvinceModel, Province>();
            CreateMap<Province, ProvinceModel>();
            
            CreateMap<DistrictModel, District>();
            CreateMap<District, DistrictModel>();
            
            CreateMap<SystemMenu, SystemMenuModel>();
            CreateMap<SystemMenuModel, SystemMenu>();
            CreateMap<SystemMenu, SystemMenuResponse>();
            CreateMap<SystemMenuResponse, SystemMenu>();
        }
    }
}