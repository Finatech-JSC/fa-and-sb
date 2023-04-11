using MicroBase.Entity;
using MicroBase.Entity.Localtions;
using MicroBase.Entity.Repositories;
using MicroBase.RedisProvider;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Linqkit;
using MicroBase.Share.Models;
using MicroBase.Share.Models.CMS.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace MicroBase.Service.Localtions
{
    public interface IProvinceService : IGenericService<Province, Guid>
    {
        Task<TPaging<ProvinceResponse>> GetAvailableAsync(Expression<Func<Province, bool>> predicate,
            int pageIndex,
            int pageSize);

        Task<List<ProvinceResponse>> GetProvincesByCountryCodeAsync(string countryCode);

        Task<ProvinceResponse> GetProvinceByIdAsync(Guid provinceId);

        Task<IEnumerable<DistrictResponse>> GetDistrictsByProvinceIdAsync(Guid provinceId);

        Task<DistrictResponse> GetDistrictsByIdAsync(Guid provinceId);
    }

    public class ProvinceService : GenericService<Province, Guid>, IProvinceService
    {
        private readonly ILogger<ProvinceService> logger;
        private readonly MicroDbContext microDbContext;
        private readonly IRedisStogare redisStogare;
        private readonly IDistrictService districtService;

        public ProvinceService(IRepository<Province, Guid> repository,
            ILogger<ProvinceService> logger,
            MicroDbContext microDbContext,
            IRedisStogare redisStogare,
            IDistrictService districtService)
            : base(repository)
        {
            this.logger = logger;
            this.microDbContext = microDbContext;
            this.redisStogare = redisStogare;
            this.districtService = districtService;
        }

        protected override void ApplyDefaultSort(FindOptions<Province> findOptions)
        {
            findOptions.SortAscending(s => s.FullName);
        }

        public async Task<TPaging<ProvinceResponse>> GetAvailableAsync(Expression<Func<Province, bool>> predicate,
            int pageIndex,
            int pageSize)
        {
            try
            {
                predicate = predicate.And(s => s.IsDelete == false);

                var rows = await Repository.CountAsync(predicate);
                var records = await microDbContext.Set<Province>()
                    .Where(predicate)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(s => s.Order)
                    .ToListAsync();

                var response = records.Select(s => new ProvinceResponse
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    ShortName = s.ShortName,
                    CreatedDate = s.CreatedDate,
                    ModifiedDate = s.ModifiedDate,
                    CountryCode = s.CountryCode,
                    Enabled = s.Enabled
                });

                return new TPaging<ProvinceResponse>
                {
                    Source = response,
                    TotalRecords = rows
                };

            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return new TPaging<ProvinceResponse>
                {
                    Source = new List<ProvinceResponse>(),
                    TotalRecords = 0
                };
            }
        }

        public async Task<List<ProvinceResponse>> GetProvincesByCountryCodeAsync(string countryCode)
        {
            try
            {
                var provinces = new List<ProvinceResponse>();
                var res = await Repository.FindAsync(s => !s.IsDelete && s.Enabled && s.CountryCode == countryCode);
                if (res == null || !res.Any())
                {
                    return new List<ProvinceResponse>();
                }

                provinces = res.Select(s => new ProvinceResponse
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    ShortName = s.ShortName,
                    CreatedDate = s.CreatedDate,
                    ModifiedDate = s.ModifiedDate,
                    CountryCode = s.CountryCode,
                    Enabled = s.Enabled
                }).ToList();

                return provinces;
            }
            catch (Exception ex)
            {
                logger.LogError($"GetProvincesByCountryCodeAsync Exception: {ex}");
                throw;
            }
        }

        public async Task<ProvinceResponse> GetProvinceByIdAsync(Guid provinceId)
        {
            var entity = await GetByIdAsync(provinceId);
            if (entity == null)
            {
                return null;
            }

            return new ProvinceResponse
            {
                Id = entity.Id,
                FullName = entity.FullName,
                ShortName = entity.ShortName,
                CreatedDate = entity.CreatedDate,
                ModifiedDate = entity.ModifiedDate,
                Enabled = entity.Enabled
            };
        }

        public async Task<IEnumerable<DistrictResponse>> GetDistrictsByProvinceIdAsync(Guid provinceId)
        {
            var entities = await districtService.GetByProvinceIdAsync(provinceId);
            return entities;
        }

        public async Task<DistrictResponse> GetDistrictsByIdAsync(Guid districtId)
        {
            var entity = await districtService.GetDistrictByIdAsync(districtId);
            return entity;
        }
    }
}