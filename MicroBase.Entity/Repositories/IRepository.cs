using MicroBase.Share.DataAccess;
using System.Linq.Expressions;

namespace MicroBase.Entity.Repositories
{
    public interface IRepository<TEntity, in TKey> where TEntity : class
    {
        #region Count

        int Count(Expression<Func<TEntity, bool>> predicate = null);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);

        long LongCount(Expression<Func<TEntity, bool>> predicate = null);

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null);

        #endregion Count

        #region Delete

        void Delete(TEntity entity);

        Task DeleteAsync(TEntity entity);

        void DeleteMany(Expression<Func<TEntity, bool>> filterExpression);

        void DeleteMany(IEnumerable<TEntity> items);

        Task DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression);

        Task DeleteManyAsync(IEnumerable<TEntity> items);

        #endregion Delete

        #region Find

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filterExpression, FindOptions<TEntity> findOptions = null, params Expression<Func<TEntity, dynamic>>[] includes);

        IEnumerable<TProjection> Find<TProjection>(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TProjection>> projection, FindOptions<TEntity> findOptions = null);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filterExpression, FindOptions<TEntity> findOptions = null);

        Task<IEnumerable<TProjection>> FindAsync<TProjection>(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TProjection>> projection, FindOptions<TEntity> findOptions = null);

        TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression, FindOptions<TEntity> findOptions = null, params Expression<Func<TEntity, dynamic>>[] includes);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression, FindOptions<TEntity> findOptions = null);

        Task<TProjection> FindOneAsync<TProjection>(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TProjection>> projection);

        TEntity GetById(TKey id);

        Task<TEntity> GetByIdAsync(TKey id);

        #endregion Fine

        #region Insert

        void Insert(TEntity entity);

        Task InsertAsync(TEntity entity);

        void InsertMany(IEnumerable<TEntity> items);

        Task InsertManyAsync(IEnumerable<TEntity> items);

        #endregion Insert

        #region Update

        void Update(TEntity entity);

        Task UpdateAsync(TEntity entity);

        void UpdateMany(IEnumerable<TEntity> items);

        void UpdateMany(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression);

        Task UpdateManyAsync(IEnumerable<TEntity> items);

        Task UpdateManyAsync(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression);

        #endregion Update
    }
}