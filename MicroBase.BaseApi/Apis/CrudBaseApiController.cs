using MicroBase.Entity;
using MicroBase.Service.Foundations;
using MicroBase.Share.Constants;
using MicroBase.Share.Linqkit;
using MicroBase.Share.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroBase.BaseApi.Apis
{
    /// <summary>
    /// Base API Controller xử lý các nghiệp vụ về CRUD
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntityDto"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    [ApiController]
    public abstract class CrudBaseApiController<TEntity, TKey, TEntityDto, TResponse>
        : BaseTrackingCrudApiController<TEntity, TKey, TEntityDto>
        where TEntity : class, IBaseEntity<TKey>
        where TEntityDto : BaseModel
        where TResponse : class
    {
        private readonly ICrudAppService<TEntity, TKey, TEntityDto, TResponse> crudAppService;

        protected CrudBaseApiController(IHttpContextAccessor httpContextAccessor,
            ICrudAppService<TEntity, TKey, TEntityDto, TResponse> crudAppService) : base(httpContextAccessor)
        {
            this.crudAppService = crudAppService;
        }

        /// <summary>
        /// Lấy thông tin một bản ghi theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trả ra TResponse</returns>
        [HttpGet("find-by-id")]
        public virtual async Task<TResponse> GetById(TKey id)
        {
            try
            {
                var record = await crudAppService.GetRecordByIdAsync(id);
                return record;
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
        [HttpGet("find-by-ids")]
        public virtual async Task<IEnumerable<TResponse>> GetByIds([FromQuery] IReadOnlyList<TKey> ids)
        {
            try
            {
                var records = await crudAppService.GetManyRecordByIdsAsync(ids);
                return records;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách theo điều kiện được truyền từ API vào
        /// find?searchTerms[0].fieldName=Id&searchTerms[0].fieldValue=fdfds&searchTerms[0].condition=EQUAL&pageIndex=1&pageSize=30
        /// </summary>
        /// <param name="searchTerms"></param>
        /// <param name="fieldOrderBy"></param>
        /// <param name="orderDirection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("find")]
        public virtual async Task<TPaging<TResponse>> Find([FromQuery] List<SearchTermModel> searchTerms,
            string? fieldOrderBy,
            bool isDescending,
            int pageIndex = 1,
            int pageSize = 20)
        {
            try
            {
                var records = await crudAppService.FindAsync(searchTerms, fieldOrderBy, isDescending, pageIndex, pageSize);
                return records;
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
        [HttpPost("add-new-record")]
        public virtual async Task<BaseResponse<TKey>> AddNewRecord(TEntityDto entityDto)
        {
            try
            {
                // TODO: Validate dữ liệu đầu vào

                entityDto = SetCrudTrackingUser(entityDto, OperationActionConstants.CrudAction.CREATE);
                var res = await crudAppService.AddNewRecordAsync(entityDto);

                return res;
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
        [HttpPost("add-many-records")]
        public virtual async Task<BaseResponse<IEnumerable<TKey>>> AddManyRecords(IEnumerable<TEntityDto> entityDtos)
        {
            try
            {
                // TODO: Validate dữ liệu đầu vào

                for (int i = 0; i < entityDtos.Count(); i++)
                {
                    var item = entityDtos.ElementAt(i);
                    item = SetCrudTrackingUser(item, OperationActionConstants.CrudAction.CREATE);
                }

                var res = await crudAppService.AddManyRecordsAsync(entityDtos);
                return res;
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
        [HttpPut("update-record")]
        public virtual async Task<BaseResponse<TKey>> UpdateRecord(TEntityDto entityDto)
        {
            try
            {
                // TODO: Validate dữ liệu đầu vào

                if (!entityDto.Id.HasValue || entityDto.Id == Guid.Empty)
                {
                    new NullReferenceException(CommonMessage.UPDATE_RECORD_ID_REQUIRED);
                }

                entityDto = SetCrudTrackingUser(entityDto, OperationActionConstants.CrudAction.UPDATE);
                var res = await crudAppService.UpdateRecordAsync(entityDto);
                return res;
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
        [HttpPut("update-many-records")]
        public virtual async Task<BaseResponse<IEnumerable<TKey>>> UpdateManyRecords(IEnumerable<TEntityDto> entityDtos)
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

                for (int i = 0; i < entityDtos.Count(); i++)
                {
                    var item = entityDtos.ElementAt(i);
                    item = SetCrudTrackingUser(item, OperationActionConstants.CrudAction.UPDATE);
                }

                var res = await crudAppService.UpdateManyRecordsAsync(entityDtos);
                return res;
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
        [HttpDelete("delete")]
        public virtual async Task<bool> Delete(TKey id)
        {
            try
            {
                var res = await crudAppService.DeleteRecordAsync(id);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Xóa nhiều bản ghi theo danh sách Id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>Trả về bool</returns>
        [HttpDelete("delete-many")]
        public virtual async Task<bool> DeleteMany(IEnumerable<TKey> ids)
        {
            try
            {
                var res = await crudAppService.DeleteManyRecordAsync(ids);
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}