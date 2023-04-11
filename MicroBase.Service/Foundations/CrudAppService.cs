using AutoMapper;
using MicroBase.Entity;
using MicroBase.Entity.Repositories;
using MicroBase.Share.Constants;
using MicroBase.Share.DataAccess;
using MicroBase.Share.Extensions;
using MicroBase.Share.Linqkit;
using MicroBase.Share.Models;

namespace MicroBase.Service.Foundations
{
    /// <summary>
    /// Base Service xử lý các nghiệp vụ về CRUD
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntityDto"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class CrudAppService<TEntity, TKey, TEntityDto, TResponse> : GenericService<TEntity, TKey>,
        ICrudAppService<TEntity, TKey, TEntityDto, TResponse>
        where TEntity : class, IBaseEntity<TKey>
        where TEntityDto : BaseModel
        where TResponse : class
    {
        private readonly IMapper mapper;

        public CrudAppService(IRepository<TEntity, TKey> repository,
            IMapper mapper) : base(repository)
        {
            this.mapper = mapper;
        }

        protected override void ApplyDefaultSort(FindOptions<TEntity> findOptions)
        {
            findOptions.SortDescending(s => s.CreatedDate);
        }

        /// <summary>
        /// Lấy thông tin một bản ghi theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trả ra TResponse</returns>
        public virtual async Task<TResponse> GetRecordByIdAsync(TKey id)
        {
            try
            {
                var record = await GetByIdAsync(id);
                var toTResponse = mapper.Map<TResponse>(record);

                return toTResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lấy thông tin các bản ghi theo danh sách Ids truyền vào
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>Trả ra danh sách TResponse</returns>
        public virtual async Task<IEnumerable<TResponse>> GetManyRecordByIdsAsync(IReadOnlyList<TKey> ids)
        {
            try
            {
                var records = await GetRecordsAsync(s => ids.Contains(s.Id) && s.IsDelete == false);
                var toTResponse = mapper.Map<IReadOnlyList<TResponse>>(records);

                return toTResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<TPaging<TResponse>> FindAsync(List<SearchTermModel> searchTerms,
            int pageIndex = 1,
            int pageSize = 20)
        {
            try
            {
                var predicate = PredicateBuilder.Create<TEntity>(s => s.IsDelete == false);
                predicate = PredicateBuilder.CombineFromDynamicTerm(predicate, searchTerms);

                var records = await GetRecordsAsync(predicate);
                var toTResponses = mapper.Map<List<TResponse>>(records);

                return new TPaging<TResponse>
                {
                    Pages = pageIndex,
                    TotalRecords = 100,
                    Source = toTResponses
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Thêm mới một bản ghi từ TEntityDto
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns>Trả ra Id của bản ghi đã được thêm mới</returns>
        public virtual async Task<BaseResponse<TKey>> AddNewRecordAsync(TEntityDto entityDto)
        {
            try
            {
                var entity = ToEntity(createdBy: entityDto.CreatedBy, modifiedBy: null, entityDto);
                if (entity == null || (Guid)(object)entity.Id == Guid.Empty)
                {
                    new NullReferenceException();
                }

                await InsertAsync(entity);
                return new BaseResponse<TKey>
                {
                    Success = true,
                    Code = (int)HttpConstants.ResponseCode.Success,
                    Message = CommonMessage.INSERT_SUCCESS,
                    Data = entity.Id
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Thêm mới danh sách bản ghi từ TEntityDtos
        /// </summary>
        /// <param name="IEnumerable<TEntityDto>"></param>
        /// <returns>Trả ra danh sách Id của các bản ghi đã được thêm mới</returns>
        public virtual async Task<BaseResponse<IEnumerable<TKey>>> AddManyRecordsAsync(IEnumerable<TEntityDto> entityDtos)
        {
            try
            {
                var enties = new List<TEntity>();
                foreach (var item in entityDtos)
                {
                    var entity = ToEntity(createdBy: item.CreatedBy, modifiedBy: null, item);
                    if (entity == null || (Guid)(object)entity.Id == Guid.Empty)
                    {
                        new NullReferenceException();
                    }

                    enties.Add(entity);
                }

                await InsertManyAsync(enties);
                return new BaseResponse<IEnumerable<TKey>>
                {
                    Success = true,
                    Code = (int)HttpConstants.ResponseCode.Success,
                    Message = CommonMessage.INSERT_SUCCESS,
                    Data = enties.Select(e => e.Id)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cập nhật thông tin bản ghi từ TEntityDto
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns>Trả ra Id của bản ghi đã được cập nhật</returns>
        public virtual async Task<BaseResponse<TKey>> UpdateRecordAsync(TEntityDto entityDto)
        {
            try
            {
                if (!entityDto.Id.HasValue || entityDto.Id == Guid.Empty)
                {
                    new NullReferenceException(CommonMessage.UPDATE_RECORD_ID_REQUIRED);
                }

                var entity = ToEntity(createdBy: null, modifiedBy: entityDto.ModifiedBy, entityDto);
                if (entity == null || (Guid)(object)entity.Id == Guid.Empty)
                {
                    new NullReferenceException();
                }

                await UpdateAsync(entity);
                return new BaseResponse<TKey>
                {
                    Success = true,
                    Code = (int)HttpConstants.ResponseCode.Success,
                    Message = CommonMessage.UPDATE_SUCCESS,
                    Data = entity.Id
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cập nhật danh sách bản ghi từ danh sách TEntityDto truyền vào
        /// </summary>
        /// <param name="entityDtos"></param>
        /// <returns>Trả ra danh sách Id của bản ghi đã được cập nhật</returns>
        public virtual async Task<BaseResponse<IEnumerable<TKey>>> UpdateManyRecordsAsync(IEnumerable<TEntityDto> entityDtos)
        {
            try
            {
                // TODO: Validate dữ liệu đầu vào

                foreach (var entityDto in entityDtos)
                {
                    if (!entityDto.Id.HasValue || entityDto.Id == Guid.Empty)
                    {
                        new NullReferenceException(CommonMessage.UPDATE_RECORD_ID_REQUIRED);
                    }
                }

                var entities = new List<TEntity>();
                foreach (var entityDto in entityDtos)
                {
                    var entity = ToEntity(createdBy: null, modifiedBy: entityDto.ModifiedBy, entityDto);
                    if (entity == null || (Guid)(object)entity.Id == Guid.Empty)
                    {
                        new NullReferenceException();
                    }
                }

                await UpdateManyAsync(entities);
                return new BaseResponse<IEnumerable<TKey>>
                {
                    Success = true,
                    Code = (int)HttpConstants.ResponseCode.Success,
                    Message = CommonMessage.UPDATE_SUCCESS,
                    Data = entities.Select(s => s.Id)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Xóa bản ghi theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trả về bool</returns>
        public virtual async Task<bool> DeleteRecordAsync(TKey id)
        {
            try
            {
                await SoftDeleteAsync(id);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Xóa bản ghi theo danh sách Ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>Trả về bool</returns>
        public virtual async Task<bool> DeleteManyRecordAsync(IEnumerable<TKey> ids)
        {
            try
            {
                await SoftDeleteManyAsync(ids);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private TEntity? ToEntity(Guid? createdBy, Guid? modifiedBy, TEntityDto entityDto)
        {
            var entity = mapper.Map<TEntity>(entityDto);
            if (entity == null)
            {
                return default(TEntity);
            }

            if (entity.Id == null || Guid.Parse(entity.Id.ToString()) == Guid.Empty)
            {
                entity.Id = (TKey)(object)Guid.NewGuid();
            }

            if (createdBy.HasValue)
            {
                entity.CreatedDate = DateTime.UtcNow.UtcToVietnamTime();
                entity.CreatedBy = createdBy;
            }
            else if (modifiedBy.HasValue)
            {
                entity.ModifiedDate = DateTime.UtcNow.UtcToVietnamTime();
                entity.ModifiedBy = createdBy;
            }

            return entity;
        }
    }
}