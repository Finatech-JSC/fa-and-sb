using MicroBase.Share.DataAccess;
using System.Linq.Expressions;

namespace MicroBase.Service
{
    public interface IGenericService<TRecord, TKey>
    {
        #region Count

        long Count(Expression<Func<TRecord, bool>> predicate = null);

        Task<long> CountAsync(Expression<Func<TRecord, bool>> predicate = null);

        #endregion Count

        #region Delete

        Task DeleteAsync(TRecord record);

        void DeleteMany(IEnumerable<TRecord> records);

        Task DeleteManyAsync(IEnumerable<TRecord> records);

        void Delete(Expression<Func<TRecord, bool>> predicate);

        Task DeleteAsync(Expression<Func<TRecord, bool>> predicate);

        #endregion Delete

        #region SoftDelete

        Task<TRecord> SoftDeleteAsync(TKey id);

        void SoftDeleteMany(IEnumerable<TKey> ids);

        Task SoftDeleteManyAsync(IEnumerable<TKey> ids);

        void SoftDelete(Expression<Func<TRecord, bool>> predicate);

        Task SoftDeleteAsync(Expression<Func<TRecord, bool>> predicate);

        #endregion SoftDelete

        #region Find

        TRecord GetById(TKey id);

        Task<TRecord> GetByIdAsync(TKey id);

        TRecord GetRecord(Expression<Func<TRecord, bool>> predicate = null, params Expression<Func<TRecord, dynamic>>[] includePaths);

        Task<TRecord> GetRecordAsync(Expression<Func<TRecord, bool>> predicate = null, FindOptions<TRecord> findOptions = null);

        IEnumerable<TRecord> GetRecords(Expression<Func<TRecord, bool>> predicate = null);

        IEnumerable<TRecord> GetRecordsInclude(Expression<Func<TRecord, bool>> predicate = null, params Expression<Func<TRecord, dynamic>>[] includePaths);

        Task<IEnumerable<TRecord>> GetRecordsAsync(Expression<Func<TRecord, bool>> predicate = null);

        IEnumerable<TProjection> GetRecords<TProjection>(Expression<Func<TRecord, bool>> predicate, Expression<Func<TRecord, TProjection>> projection, FindOptions<TRecord> findOptions = null);

        Task<IEnumerable<TProjection>> GetRecordsAsync<TProjection>(Expression<Func<TRecord, bool>> predicate, Expression<Func<TRecord, TProjection>> projection, FindOptions<TRecord> findOptions = null);

        IEnumerable<TRecord> GetRecords(Expression<Func<TRecord, bool>> predicate, FindOptions<TRecord> findOptions);

        Task<IEnumerable<TRecord>> GetRecordsAsync(Expression<Func<TRecord, bool>> predicate, FindOptions<TRecord> findOptions);

        #endregion Find

        #region Insert

        void Insert(TRecord record);

        Task InsertAsync(TRecord record);

        void InsertMany(IEnumerable<TRecord> records);

        Task InsertManyAsync(IEnumerable<TRecord> records);

        #endregion Insert

        #region Update

        void Update(TRecord record);

        Task UpdateAsync(TRecord record);

        void UpdateMany(IEnumerable<TRecord> records);

        Task UpdateManyAsync(IEnumerable<TRecord> records);

        void UpdateMany(Expression<Func<TRecord, bool>> predicate, Expression<Func<TRecord, TRecord>> updateExpression);

        Task UpdateManyAsync(Expression<Func<TRecord, bool>> predicate, Expression<Func<TRecord, TRecord>> updateExpression);

        #endregion Update
    }
}