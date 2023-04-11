using MicroBase.Entity;
using MicroBase.Share.Linqkit;
using MicroBase.Share.Models;

namespace MicroBase.Service.Foundations
{
    public interface ICrudAppService<TEntity, TKey, TEntityDto, TResponse> : IGenericService<TEntity, TKey>
        where TEntity : class, IBaseEntity<TKey>
        where TEntityDto : BaseModel
        where TResponse : class
    {
        Task<TResponse> GetRecordByIdAsync(TKey id);

        Task<IEnumerable<TResponse>> GetManyRecordByIdsAsync(IReadOnlyList<TKey> ids);

        Task<TPaging<TResponse>> FindAsync(List<SearchTermModel> searchTerms, int pageIndex = 1, int pageSize = 20);

        Task<BaseResponse<TKey>> AddNewRecordAsync(TEntityDto entityDto);

        Task<BaseResponse<IEnumerable<TKey>>> AddManyRecordsAsync(IEnumerable<TEntityDto> entityDtos);

        Task<BaseResponse<TKey>> UpdateRecordAsync(TEntityDto entityDto);

        Task<BaseResponse<IEnumerable<TKey>>> UpdateManyRecordsAsync(IEnumerable<TEntityDto> entityDtos);

        Task<bool> DeleteRecordAsync(TKey id);

        Task<bool> DeleteManyRecordAsync(IEnumerable<TKey> ids);
    }
}