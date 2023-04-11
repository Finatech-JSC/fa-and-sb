using MicroBase.Entity;
using MicroBase.Entity.Repositories;
using MicroBase.Share.Extensions;
using MicroBase.Share.DataAccess;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MicroBase.Service
{
    public abstract class GenericService<TEntity, TKey> : IGenericService<TEntity, TKey> 
        where TEntity : class, IBaseEntity<TKey> 
    {
        protected readonly IRepository<TEntity, TKey> Repository;

        protected GenericService(IRepository<TEntity, TKey> repository)
        {
            Repository = repository;
        }

        #region Count

        public long Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            return Repository.Count(predicate);
        }

        public async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return await Repository.CountAsync(predicate);
        }

        #endregion Count

        #region Delete

        public virtual async Task DeleteAsync(TEntity record)
        {
            await Repository.DeleteAsync(record);
        }

        public virtual void DeleteMany(IEnumerable<TEntity> records)
        {
            Repository.DeleteMany(records);
        }

        public async Task DeleteManyAsync(IEnumerable<TEntity> records)
        {
            await Repository.DeleteManyAsync(records);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            Repository.DeleteMany(predicate);
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            await Repository.DeleteManyAsync(predicate);
        }

        #endregion Delete

        #region SoftDelete

        public virtual async Task<TEntity> SoftDeleteAsync(TKey id)
        {
            var record = Repository.GetById(id);
            if (record == null)
            {
                return null;
            }

            record.IsDelete = true;
            await Repository.UpdateAsync(record);

            return record;
        }

        public virtual void SoftDeleteMany(IEnumerable<TKey> ids)
        {
            var records = Repository.Find(s => ids.Contains(s.Id));
            if (!records.Any())
            {
                return;
            }

            for (int i = 0; i < records.Count(); i++)
            {
                records.ElementAt(i).IsDelete = true;
            }

            Repository.UpdateMany(records);
        }

        public async Task SoftDeleteManyAsync(IEnumerable<TKey> ids)
        {
            var records = await Repository.FindAsync(s => ids.Contains(s.Id));
            if (!records.Any())
            {
                return;
            }

            for (int i = 0; i < records.Count(); i++)
            {
                records.ElementAt(i).IsDelete = true;
            }

            await Repository.UpdateManyAsync(records);
        }

        public void SoftDelete(Expression<Func<TEntity, bool>> predicate)
        {
            var records = Repository.Find(predicate);
            if (!records.Any())
            {
                return;
            }

            for (int i = 0; i < records.Count(); i++)
            {
                records.ElementAt(i).IsDelete = true;
            }

            Repository.UpdateMany(records);
        }

        public async Task SoftDeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var records = await Repository.FindAsync(predicate);
            if (!records.Any())
            {
                return;
            }

            for (int i = 0; i < records.Count(); i++)
            {
                records.ElementAt(i).IsDelete = true;
            }

            await Repository.UpdateManyAsync(records);
        }

        #endregion SoftDelete

        #region Find

        public virtual TEntity GetById(TKey id)
        {
            return Repository.GetById(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await Repository.GetByIdAsync(id);
        }

        public TEntity GetRecord(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            return Repository.FindOne(predicate, null, includePaths);
        }

        public async Task<TEntity> GetRecordAsync(Expression<Func<TEntity, bool>> predicate = null, FindOptions<TEntity> findOptions = null)
        {
            return await Repository.FindOneAsync(predicate, findOptions);
        }

        public virtual IEnumerable<TEntity> GetRecords(Expression<Func<TEntity, bool>> predicate = null)
        {
            var findOptions = new FindOptions<TEntity>();
            ApplyDefaultSort(findOptions);

            return Repository.Find(predicate, findOptions);
        }

        public virtual IEnumerable<TEntity> GetRecordsInclude(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            var findOptions = new FindOptions<TEntity>();
            ApplyDefaultSort(findOptions);

            return Repository.Find(predicate, findOptions, includePaths);
        }

        public async Task<IEnumerable<TEntity>> GetRecordsAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            var findOptions = new FindOptions<TEntity>();
            ApplyDefaultSort(findOptions);

            return await Repository.FindAsync(predicate, findOptions);
        }

        public IEnumerable<TProjection> GetRecords<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> projection, FindOptions<TEntity> findOptions = null)
        {
            return Repository.Find(predicate, projection, findOptions);
        }

        public async Task<IEnumerable<TProjection>> GetRecordsAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> projection, FindOptions<TEntity> findOptions = null)
        {
            return await Repository.FindAsync(predicate, projection, findOptions);
        }

        public IEnumerable<TEntity> GetRecords(Expression<Func<TEntity, bool>> predicate, FindOptions<TEntity> findOptions)
        {
            return Repository.Find(predicate, findOptions);
        }

        public async Task<IEnumerable<TEntity>> GetRecordsAsync(Expression<Func<TEntity, bool>> predicate, FindOptions<TEntity> findOptions)
        {
            return await Repository.FindAsync(predicate, findOptions);
        }

        #endregion Find

        #region Insert

        public virtual void Insert(TEntity record)
        {
            record.CreatedDate = DateTime.UtcNow;

            Repository.Insert(record);
        }

        public virtual async Task InsertAsync(TEntity record)
        {
            try
            {
                record.CreatedDate = DateTime.UtcNow;

                await Repository.InsertAsync(record);
            }
            catch (Exception ex)
            {
                throw new Exception("Have exception when insert entity: " + record.GetType().Name + " with data: " + record.JsonSerialize(), ex);
            }
        }

        public virtual void InsertMany(IEnumerable<TEntity> records)
        {
            if (records == null)
            {
                return;
            }

            for (int i = 0; i < records.Count(); i++)
            {
                records.ElementAt(i).CreatedDate = DateTime.UtcNow;
            }

            Repository.InsertMany(records);
        }

        public virtual async Task InsertManyAsync(IEnumerable<TEntity> records)
        {
            if (records == null)
            {
                return;
            }

            for (int i = 0; i < records.Count(); i++)
            {
                records.ElementAt(i).CreatedDate = DateTime.UtcNow;
            }

            await Repository.InsertManyAsync(records);
        }

        #endregion Insert

        #region Update

        public virtual void Update(TEntity record)
        {
            record.ModifiedDate = DateTime.UtcNow;
            Repository.Update(record);
        }

        public virtual async Task UpdateAsync(TEntity record)
        {
            try
            {
                record.ModifiedDate = DateTime.UtcNow;
                await Repository.UpdateAsync(record);
            }
            catch (Exception ex)
            {
                throw new Exception("Have exception when update entity: " + record.GetType().Name + " with data: " + record.JsonSerialize(), ex);
            }
        }

        public virtual void UpdateMany(IEnumerable<TEntity> records)
        {
            for (int i = 0; i < records.Count(); i++)
            {
                records.ElementAt(i).ModifiedDate = DateTime.UtcNow;
            }

            Repository.UpdateMany(records);
        }

        public virtual async Task UpdateManyAsync(IEnumerable<TEntity> records)
        {
            for (int i = 0; i < records.Count(); i++)
            {
                records.ElementAt(i).ModifiedDate = DateTime.UtcNow;
            }

            await Repository.UpdateManyAsync(records);
        }

        public virtual void UpdateMany(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            Repository.UpdateMany(predicate, updateExpression);
        }

        public virtual async Task UpdateManyAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            await Repository.UpdateManyAsync(predicate, updateExpression);
        }

        #endregion Update

        #region Non-Public Methods

        protected abstract void ApplyDefaultSort(FindOptions<TEntity> findOptions);

        #endregion Non-Public Methods
    }
}