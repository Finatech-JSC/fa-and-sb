using MicroBase.Entity;
using MicroBase.Entity.Localtions;
using MicroBase.Entity.Repositories;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Linqkit;
using MicroBase.Share.Models;
using MicroBase.Share.Models.CMS.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace MicroBase.Service.Localtions
{
    public interface IDistrictService : IGenericService<District, Guid>
    {
        Task<TPaging<DistrictResponse>> GetAvailableAsync(Expression<Func<District, bool>> predicate,
            int pageIndex,
            int pageSize);

        Task<IEnumerable<DistrictResponse>> GetByProvinceIdAsync(Guid provinceId);

        Task<DistrictResponse> GetDistrictByIdAsync(Guid districtId);
    }

    public class DistrictService : GenericService<District, Guid>, IDistrictService
    {
        private readonly ILogger<DistrictService> logger;
        private readonly MicroDbContext microDbContext;

        public DistrictService(IRepository<District, Guid> repository,
            ILogger<DistrictService> logger,
            MicroDbContext microDbContext)
            : base(repository)
        {
            this.logger = logger;
            this.microDbContext = microDbContext;
        }

        protected override void ApplyDefaultSort(FindOptions<District> findOptions)
        {
            findOptions.SortAscending(s => s.FullName);
        }

        public async Task<TPaging<DistrictResponse>> GetAvailableAsync(Expression<Func<District, bool>> predicate,
            int pageIndex,
            int pageSize)
        {
            try
            {
                predicate = predicate.And(s => s.IsDelete == false);

                var rows = await Repository.CountAsync(predicate);
                var records = await microDbContext.Set<District>()
                    .Include(s => s.Province)
                    .Where(predicate)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(s => s.FullName)
                    .ToListAsync();

                var response = records.Select(s => new DistrictResponse
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    ShortName = s.ShortName,
                    CreatedDate = s.CreatedDate,
                    ModifiedDate = s.ModifiedDate,
                    Enabled = s.Enabled,
                    Province = new NameValueModel<Guid>
                    {
                        Name = s.Province.FullName,
                        Value = s.Province.Id
                    },
                    ProvinceId = s.ProvinceId
                });

                return new TPaging<DistrictResponse>
                {
                    Source = response,
                    TotalRecords = rows
                };

            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return new TPaging<DistrictResponse>
                {
                    Source = new List<DistrictResponse>(),
                    TotalRecords = 0
                };
            }
        }

        public async Task<IEnumerable<DistrictResponse>> GetByProvinceIdAsync(Guid provinceId)
        {
            try
            {
                var records = await microDbContext.Set<District>()
                    .Include(s => s.Province)
                    .Where(s => s.ProvinceId == provinceId)
                    .OrderBy(s => s.FullName)
                    .ToListAsync();

                return records.Select(s => new DistrictResponse
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    ShortName = s.ShortName,
                    CreatedDate = s.CreatedDate,
                    ModifiedDate = s.ModifiedDate,
                    Enabled = s.Enabled,
                    Province = new NameValueModel<Guid>
                    {
                        Name = s.Province.FullName,
                        Value = s.Province.Id
                    }
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DistrictResponse> GetDistrictByIdAsync(Guid districtId)
        {
            try
            {
                var entity = await microDbContext.Set<District>()
                    .Include(s => s.Province)
                    .FirstOrDefaultAsync(s => s.Id == districtId);

                return new DistrictResponse
                {
                    Id = entity.Id,
                    FullName = entity.FullName,
                    ShortName = entity.ShortName,
                    CreatedDate = entity.CreatedDate,
                    ModifiedDate = entity.ModifiedDate,
                    Enabled = entity.Enabled,
                    Province = new NameValueModel<Guid>
                    {
                        Name = entity.Province.FullName,
                        Value = entity.Province.Id
                    },
                    ProvinceId = entity.ProvinceId
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}