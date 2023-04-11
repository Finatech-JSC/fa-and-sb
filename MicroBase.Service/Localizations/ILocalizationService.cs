using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using MicroBase.Share.Models;
using MicroBase.Entity;
using MicroBase.Entity.Repositories;
using MicroBase.Share.DataAccess;
using MicroBase.Share;
using MicroBase.Share.Extensions;
using MicroBase.Share.Linqkit;
using MicroBase.Logging;
using MicroBase.Share.Models.Localizations.Localizations;
using MicroBase.Share.Constants;

namespace MicroBase.Service.Localizations
{
    public interface ILocalizationService : IGenericService<LocalizationKey, Guid>
    {
        Task BuildLocalizationKeyAsync();

        string GetLocalizationString(string localizationKey,
            List<string> msgParams = null,
            string prefix = null,
            string fallbackContent = null);

        string GetLocalizationString(string localizationKey,
            List<string> msgParams = null,
            string prefix = null,
            string fallbackContent = null,
            string cultureCode = null);

        Task<TPaging<LocalizationModel>> GetAvailableAsync(Expression<Func<LocalizationKey, bool>> predicate,
            int pageIndex,
            int pageSize);

        Task<BaseResponse<object>> InsertOrUpdateManyAsync(IEnumerable<LocalizationModel> localizations);
    }

    public class LocalizationService : GenericService<LocalizationKey, Guid>, ILocalizationService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IExceptionMonitorService exceptionMonitorService;
        private readonly MicroDbContext microDbContext;

        public LocalizationService(IRepository<LocalizationKey, Guid> repository,
            IHttpContextAccessor httpContextAccessor,
            IExceptionMonitorService exceptionMonitorService,
            MicroDbContext microDbContext) : base(repository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.exceptionMonitorService = exceptionMonitorService;
            this.microDbContext = microDbContext;
        }

        protected override void ApplyDefaultSort(FindOptions<LocalizationKey> findOptions)
        {
            findOptions.SortAscending(s => s.Key);
        }

        public async Task BuildLocalizationKeyAsync()
        {
            try
            {
                var records = await Repository.FindAsync(s => !s.IsDelete);
                foreach (var item in records)
                {
                    var key = $"{item.CultureCode}:{item.Prefix}:{item.Key}";
                    if (!StaticModel.LocalizationDics.ContainsKey(key))
                    {
                        StaticModel.LocalizationDics.Add(key, new LocalizationKeyModel
                        {
                            Key = item.Key,
                            Prefix = item.Prefix,
                            Content = item.Content,
                            CultureCode = item.CultureCode
                        });
                    }
                    else
                    {
                        var cache = StaticModel.LocalizationDics[key];
                        cache.Content = item.Content;
                    }
                }
            }
            catch (Exception ex)
            {
                await exceptionMonitorService.SendExceptionNotiAsync("BuildLocalizationKeyAsync", ex);
            }
        }

        public string GetLocalizationString(string localizationKey,
            List<string> msgParams = null,
            string prefix = null,
            string fallbackContent = null)
        {
            var cultureCode = httpContextAccessor.HttpContext.GetRequestCultureCode();
            if (string.IsNullOrWhiteSpace(cultureCode))
            {
                cultureCode = Constants.CultureCode.UnitedStates;
            }

            return GetLocalization(localizationKey, msgParams, prefix, fallbackContent, cultureCode);
        }

        public string GetLocalizationString(string localizationKey,
            List<string> msgParams = null,
            string prefix = null,
            string fallbackContent = null,
            string cultureCode = null)
        {
            if (string.IsNullOrWhiteSpace(cultureCode))
            {
                cultureCode = Constants.CultureCode.UnitedStates;
            }

            return GetLocalization(localizationKey, msgParams, prefix, fallbackContent, cultureCode);
        }

        private static string GetLocalization(string localizationKey, List<string> msgParams, string prefix, string fallbackContent, string cultureCode)
        {
            var key = $"{cultureCode}:{prefix}:{localizationKey}";
            var vnKey = $"{Constants.CultureCode.UnitedStates}:{prefix}:{localizationKey}";

            var localization = string.Empty;
            if (StaticModel.LocalizationDics.ContainsKey(key))
            {
                localization = StaticModel.LocalizationDics[key].Content;
            }
            else if (StaticModel.LocalizationDics.ContainsKey(vnKey))
            {
                localization = StaticModel.LocalizationDics[vnKey].Content;
            }

            if (string.IsNullOrWhiteSpace(localization))
            {
                var subDisc = StaticModel.LocalizationDics.Where(s => key.Contains(s.Key));
                if (!subDisc.Any())
                {
                    subDisc = StaticModel.LocalizationDics.Where(s => vnKey.Contains(s.Key));
                }

                if (subDisc.Any())
                {
                    var item = subDisc.FirstOrDefault();
                    if (item.Value != null && !string.IsNullOrWhiteSpace(item.Value.Key))
                    {
                        localization = localizationKey.Replace(item.Value.Key, item.Value.Content);
                    }
                }

                if (string.IsNullOrWhiteSpace(localization))
                {
                    if (!string.IsNullOrWhiteSpace(fallbackContent))
                    {
                        localization = fallbackContent;
                    }
                    else
                    {
                        localization = localizationKey;
                    }
                }
            }

            if (msgParams != null && msgParams.Any())
            {
                localization = string.Format(localization, msgParams.ToArray());
            }

            return localization;
        }

        public async Task<TPaging<LocalizationModel>> GetAvailableAsync(Expression<Func<LocalizationKey, bool>> predicate,
            int pageIndex,
            int pageSize)
        {
            predicate = predicate.And(s => !s.IsDelete);
            var records = await microDbContext.Set<LocalizationKey>()
                .Where(predicate)
                .OrderByDescending(s => s.Prefix)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = records.Select(s => new LocalizationModel
            {
                Id = s.Id,
                Prefix = s.Prefix,
                Key = s.Key,
                CultureCode = s.CultureCode,
                Content = s.Content
            });

            var rows = await Repository.CountAsync(predicate);
            return new TPaging<LocalizationModel>
            {
                Source = response,
                TotalRecords = rows
            };
        }

        public async Task<BaseResponse<object>> InsertOrUpdateManyAsync(IEnumerable<LocalizationModel> localizations)
        {
            var updateEntities = new List<LocalizationKey>();
            var addNewEntities = new List<LocalizationKey>();

            foreach (var item in localizations)
            {
                if (!string.IsNullOrWhiteSpace(item.Key) && !string.IsNullOrWhiteSpace(item.CultureCode))
                {
                    var local = await Repository.FindOneAsync(s => s.Key == item.Key && s.Prefix == item.Prefix && s.CultureCode == item.CultureCode);
                    if (local != null)
                    {
                        local.Content = item.Content;
                        local.ModifiedDate = DateTime.UtcNow;
                        local.CultureCode = item.CultureCode;
                        local.IsDelete = false;

                        updateEntities.Add(local);
                    }
                    else
                    {
                        addNewEntities.Add(new LocalizationKey
                        {
                            Id = Guid.NewGuid(),
                            Content = item.Content,
                            CultureCode = item.CultureCode,
                            Prefix = item.Prefix,
                            Key = item.Key,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }
            }

            if (updateEntities.Any())
            {
                await Repository.UpdateManyAsync(updateEntities);
            }

            if (addNewEntities.Any())
            {
                await Repository.InsertManyAsync(addNewEntities);
            }

            return new BaseResponse<object>
            {
                Success = true,
                Data = CommonMessage.UPDATE_SUCCESS
            };
        }
    }
}