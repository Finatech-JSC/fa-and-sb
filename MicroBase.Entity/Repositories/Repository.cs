using MicroBase.Share.Extensions;
using MicroBase.Share.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using Z.EntityFramework.Plus;

namespace MicroBase.Entity.Repositories
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class 
    {
        private readonly MicroDbContext microDbContext;

        public Repository(MicroDbContext microDbContext)
        {
            this.microDbContext = microDbContext;
        }

        private MicroDbContext OpenDbContext()
        {
            return microDbContext;
        }

        public int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            var dbSet = microDbContext.Set<TEntity>();
            return predicate == null ? dbSet.Count() : dbSet.Count(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            var dbSet = microDbContext.Set<TEntity>();
            return predicate == null ? await dbSet.CountAsync() : await dbSet.CountAsync(predicate);
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate = null)
        {
            var dbSet = microDbContext.Set<TEntity>();
            return predicate != null ? dbSet.LongCount(predicate) : dbSet.LongCount();
        }

        public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            var dbSet = microDbContext.Set<TEntity>();
            return predicate != null ? await dbSet.LongCountAsync(predicate) : await dbSet.LongCountAsync();
        }

        public void Delete(TEntity entity)
        {
            var dbSet = microDbContext.Set<TEntity>();
            dbSet.Remove(entity);
            microDbContext.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            var dbSet = microDbContext.Set<TEntity>();
            dbSet.Remove(entity);
            await microDbContext.SaveChangesAsync();
        }

        public void DeleteMany(Expression<Func<TEntity, bool>> filterExpression)
        {
            var dbSet = microDbContext.Set<TEntity>();
            dbSet.Where(filterExpression).Delete();
        }

        public void DeleteMany(IEnumerable<TEntity> items)
        {
            var dbSet = microDbContext.Set<TEntity>();
            dbSet.RemoveRange(items);
            microDbContext.SaveChanges();
        }

        public async Task DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            var dbSet = microDbContext.Set<TEntity>();
            await dbSet.Where(filterExpression).DeleteAsync();
        }

        public async Task DeleteManyAsync(IEnumerable<TEntity> items)
        {
            var dbSet = microDbContext.Set<TEntity>();
            dbSet.RemoveRange(items);
            await microDbContext.SaveChangesAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filterExpression,
            FindOptions<TEntity> findOptions = null,
            params Expression<Func<TEntity, dynamic>>[] includes)
        {
            IQueryable<TEntity> queryable = microDbContext.Set<TEntity>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    queryable = queryable.Include(include);
                }
            }

            var skip = 0;
            var limit = 0;

            if (findOptions != null)
            {
                if (findOptions.Sorts != null)
                {
                    bool isFirst = true;
                    foreach (var sortDirection in findOptions.Sorts)
                    {
                        queryable = SetOrderBy(queryable, sortDirection, isFirst);
                        isFirst = false;
                    }
                }

                if (findOptions.Skip.HasValue && findOptions.Skip.Value > 0)
                {
                    skip = findOptions.Skip.Value;
                }

                if (findOptions.Limit.HasValue && findOptions.Limit.Value > 0)
                {
                    limit = findOptions.Limit.Value;
                }
            }

            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }

            if (skip > 0)
            {
                queryable = queryable.Skip(skip);
            }

            if (limit > 0)
            {
                queryable = queryable.Take(limit);
            }

            return queryable.ToImmutableArray();
        }

        public IEnumerable<TProjection> Find<TProjection>(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TProjection>> projection, FindOptions<TEntity> findOptions = null)
        {
            if (projection == null)
            {
                throw new ArgumentNullException("projection");
            }

            IQueryable<TEntity> queryable = microDbContext.Set<TEntity>();
            var skip = 0;
            var limit = 0;

            if (findOptions != null)
            {
                if (!findOptions.Sorts.IsNullOrEmpty())
                {
                    bool isFirst = true;
                    foreach (var sortDirection in findOptions.Sorts)
                    {
                        queryable = SetOrderBy(queryable, sortDirection, isFirst);
                        isFirst = false;
                    }
                }

                if (findOptions.Skip.HasValue && findOptions.Skip.Value > 0)
                {
                    skip = findOptions.Skip.Value;
                }

                if (findOptions.Limit.HasValue && findOptions.Limit.Value > 0)
                {
                    limit = findOptions.Limit.Value;
                }
            }

            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }

            if (skip > 0)
            {
                queryable = queryable.Skip(skip);
            }

            if (limit > 0)
            {
                queryable = queryable.Take(limit);
            }

            return queryable.Select(projection).ToImmutableArray();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filterExpression, FindOptions<TEntity> findOptions = null)
        {
            IQueryable<TEntity> queryable = microDbContext.Set<TEntity>();
            var skip = 0;
            var limit = 0;

            if (findOptions != null)
            {
                if (!findOptions.Sorts.IsNullOrEmpty())
                {
                    bool isFirst = true;
                    foreach (var sortDirection in findOptions.Sorts)
                    {
                        queryable = SetOrderBy(queryable, sortDirection, isFirst);
                        isFirst = false;
                    }
                }

                if (findOptions.Skip.HasValue && findOptions.Skip.Value > 0)
                {
                    skip = findOptions.Skip.Value;
                }

                if (findOptions.Limit.HasValue && findOptions.Limit.Value > 0)
                {
                    limit = findOptions.Limit.Value;
                }
            }

            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }

            if (skip > 0)
            {
                queryable = queryable.Skip(skip);
            }

            if (limit > 0)
            {
                queryable = queryable.Take(limit);
            }

            return await queryable.ToListAsync();
        }

        public async Task<IEnumerable<TProjection>> FindAsync<TProjection>(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TProjection>> projection, FindOptions<TEntity> findOptions = null)
        {
            if (projection == null)
            {
                throw new ArgumentNullException("projection");
            }

            IQueryable<TEntity> queryable = microDbContext.Set<TEntity>();
            var skip = 0;
            var limit = 0;

            if (findOptions != null)
            {
                if (!findOptions.Sorts.IsNullOrEmpty())
                {
                    bool isFirst = true;
                    foreach (var sortDirection in findOptions.Sorts)
                    {
                        queryable = SetOrderBy(queryable, sortDirection, isFirst);
                        isFirst = false;
                    }
                }

                if (findOptions.Skip.HasValue && findOptions.Skip.Value > 0)
                {
                    skip = findOptions.Skip.Value;
                }

                if (findOptions.Limit.HasValue && findOptions.Limit.Value > 0)
                {
                    limit = findOptions.Limit.Value;
                }
            }

            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }

            if (skip > 0)
            {
                queryable = queryable.Skip(skip);
            }

            if (limit > 0)
            {
                queryable = queryable.Take(limit);
            }

            return await queryable.Select(projection).ToArrayAsync();
        }

        public TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression, FindOptions<TEntity> findOptions = null, params Expression<Func<TEntity, dynamic>>[] includes)
        {
            IQueryable<TEntity> queryable = microDbContext.Set<TEntity>();

            if (!includes.IsNullOrEmpty())
            {
                foreach (var include in includes)
                {
                    queryable = queryable.Include(include);
                }
            }

            if (findOptions != null)
            {
                if (!findOptions.Sorts.IsNullOrEmpty())
                {
                    bool isFirst = true;
                    foreach (var sortDirection in findOptions.Sorts)
                    {
                        queryable = SetOrderBy(queryable, sortDirection, isFirst);
                        isFirst = false;
                    }
                }
            }

            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }

            return queryable.FirstOrDefault();
        }

        public async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression, FindOptions<TEntity> findOptions = null)
        {
            IQueryable<TEntity> queryable = microDbContext.Set<TEntity>();

            if (findOptions != null)
            {
                if (!findOptions.Sorts.IsNullOrEmpty())
                {
                    bool isFirst = true;
                    foreach (var sortDirection in findOptions.Sorts)
                    {
                        queryable = SetOrderBy(queryable, sortDirection, isFirst);
                        isFirst = false;
                    }
                }
            }

            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }

            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<TProjection> FindOneAsync<TProjection>(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TProjection>> projection)
        {
            if (projection == null)
            {
                throw new ArgumentNullException("projection");
            }

            if (filterExpression == null)
            {
                return await microDbContext.Set<TEntity>().Select(projection).FirstOrDefaultAsync();
            }

            return await microDbContext.Set<TEntity>().Where(filterExpression).Select(projection).FirstOrDefaultAsync();
        }

        public TEntity GetById(TKey id)
        {
            return microDbContext.Set<TEntity>().Find(id);
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await microDbContext.Set<TEntity>().FindAsync(id);
        }

        #region Insert

        public void Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            microDbContext.Set<TEntity>().Add(entity);
            microDbContext.SaveChanges();
        }

        public async Task InsertAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            microDbContext.Set<TEntity>().Add(entity);
            await microDbContext.SaveChangesAsync();
        }

        public void InsertMany(IEnumerable<TEntity> items)
        {
            if (items == null)
            {
                return;
            }

            microDbContext.Set<TEntity>().AddRange(items);
            microDbContext.SaveChanges();
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> items)
        {
            if (items == null)
            {
                return;
            }

            microDbContext.Set<TEntity>().AddRange(items);
            await microDbContext.SaveChangesAsync();
        }

        #endregion Insert

        #region Update

        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            // Set the entity's state to modified
            microDbContext.Entry(entity).State = EntityState.Modified;
            microDbContext.SaveChanges();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            // Set the entity's state to modified
            microDbContext.Entry(entity).State = EntityState.Modified;
            await microDbContext.SaveChangesAsync();
        }

        public void UpdateMany(IEnumerable<TEntity> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                microDbContext.Entry(item).State = EntityState.Modified;
            }
            microDbContext.SaveChanges();
        }

        public void UpdateMany(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            var memberInitExpression = updateExpression.Body as MemberInitExpression;
            if (memberInitExpression == null)
            {
                throw new ArgumentException("The update expression must be of type MemberInitExpression.", "updateExpression");
            }

            var records = microDbContext.Set<TEntity>().Where(filterExpression);
            bool needUpdate = false;

            foreach (var record in records)
            {
                foreach (MemberBinding binding in memberInitExpression.Bindings)
                {
                    var memberAssignment = binding as MemberAssignment;

                    if (memberAssignment == null)
                    {
                        throw new ArgumentException("The update expression MemberBinding must only by type MemberAssignment.", "updateExpression");
                    }

                    Expression memberExpression = memberAssignment.Expression;

                    object value;
                    if (memberExpression.NodeType == ExpressionType.Constant)
                    {
                        var constantExpression = memberExpression as ConstantExpression;
                        if (constantExpression == null)
                        {
                            throw new ArgumentException("The MemberAssignment expression is not a ConstantExpression.", "updateExpression");
                        }

                        value = constantExpression.Value;
                    }
                    else
                    {
                        LambdaExpression lambda = Expression.Lambda(memberExpression, null);
                        value = lambda.Compile().DynamicInvoke();
                    }

                    ((PropertyInfo)memberAssignment.Member).SetValue(record, value);
                    needUpdate = true;
                }
            }

            if (needUpdate)
            {
                microDbContext.SaveChanges();
            }
        }

        public async Task UpdateManyAsync(IEnumerable<TEntity> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                microDbContext.Entry(item).State = EntityState.Modified;
            }
            await microDbContext.SaveChangesAsync();
        }

        public async Task UpdateManyAsync(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            var memberInitExpression = updateExpression.Body as MemberInitExpression;

            if (memberInitExpression == null)
            {
                throw new ArgumentException("The update expression must be of type MemberInitExpression.", "updateExpression");
            }

            var records = microDbContext.Set<TEntity>().Where(filterExpression);
            var needUpdate = false;

            foreach (var record in records)
            {
                foreach (MemberBinding binding in memberInitExpression.Bindings)
                {
                    var memberAssignment = binding as MemberAssignment;
                    if (memberAssignment == null)
                    {
                        throw new ArgumentException("The update expression MemberBinding must only by type MemberAssignment.", "updateExpression");
                    }

                    Expression memberExpression = memberAssignment.Expression;

                    object value;
                    if (memberExpression.NodeType == ExpressionType.Constant)
                    {
                        var constantExpression = memberExpression as ConstantExpression;
                        if (constantExpression == null)
                        {
                            throw new ArgumentException("The MemberAssignment expression is not a ConstantExpression.", "updateExpression");
                        }
                        value = constantExpression.Value;
                    }
                    else
                    {
                        LambdaExpression lambda = Expression.Lambda(memberExpression, null);
                        value = lambda.Compile().DynamicInvoke();
                    }

                    ((PropertyInfo)memberAssignment.Member).SetValue(record, value);
                    needUpdate = true;
                }
            }

            if (needUpdate)
            {
                await microDbContext.SaveChangesAsync();
            }
        }

        #endregion Update

        #region Non-Public Methods

        private static Expression<Func<TSource, TSourceKey>> GetExpression<TSource, TSourceKey>(LambdaExpression lambdaExpression)
        {
            return (Expression<Func<TSource, TSourceKey>>)lambdaExpression;
        }

        /// <summary>
        /// Orders a queryable according to specified SortDirection
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The queryable</param>
        /// <param name="sortDirection">The sort direction</param>
        /// <param name="isFirst">true to use OrderBy(), false to use ThenBy()</param>
        /// <returns></returns>
        private static IOrderedQueryable<TSource> SetOrderBy<TSource>(
            IQueryable<TSource> source, KeyValuePair<LambdaExpression, SortDirection> sortDirection, bool isFirst)
        {
            Type type = sortDirection.Key.Body.Type;
            bool isNullable = type.IsNullable();

            if (isNullable)
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (isFirst)
            {
                if (sortDirection.Value == SortDirection.Ascending)
                {
                    #region OrderBy

                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.String: return source.OrderBy(GetExpression<TSource, string>(sortDirection.Key));
                        case TypeCode.Boolean:
                            {
                                return isNullable
                                    ? source.OrderBy(GetExpression<TSource, bool?>(sortDirection.Key))
                                    : source.OrderBy(GetExpression<TSource, bool>(sortDirection.Key));
                            }
                        case TypeCode.Int16:
                            {
                                return isNullable
                                    ? source.OrderBy(GetExpression<TSource, short?>(sortDirection.Key))
                                    : source.OrderBy(GetExpression<TSource, short>(sortDirection.Key));
                            }
                        case TypeCode.Int32:
                            {
                                return isNullable
                                    ? source.OrderBy(GetExpression<TSource, int?>(sortDirection.Key))
                                    : source.OrderBy(GetExpression<TSource, int>(sortDirection.Key));
                            }
                        case TypeCode.Int64:
                            {
                                return isNullable
                                    ? source.OrderBy(GetExpression<TSource, long?>(sortDirection.Key))
                                    : source.OrderBy(GetExpression<TSource, long>(sortDirection.Key));
                            }
                        case TypeCode.Single:
                            {
                                return isNullable
                                    ? source.OrderBy(GetExpression<TSource, float?>(sortDirection.Key))
                                    : source.OrderBy(GetExpression<TSource, float>(sortDirection.Key));
                            }
                        case TypeCode.Byte:
                            {
                                return isNullable
                                    ? source.OrderBy(GetExpression<TSource, byte?>(sortDirection.Key))
                                    : source.OrderBy(GetExpression<TSource, byte>(sortDirection.Key));
                            }
                        case TypeCode.Decimal:
                            {
                                return isNullable
                                    ? source.OrderBy(GetExpression<TSource, decimal?>(sortDirection.Key))
                                    : source.OrderBy(GetExpression<TSource, decimal>(sortDirection.Key));
                            }
                        case TypeCode.DateTime:
                            {
                                return isNullable
                                    ? source.OrderBy(GetExpression<TSource, DateTime?>(sortDirection.Key))
                                    : source.OrderBy(GetExpression<TSource, DateTime>(sortDirection.Key));
                            }
                        default:
                            if (type == typeof(Guid))
                            {
                                return isNullable
                                    ? source.OrderBy(GetExpression<TSource, Guid?>(sortDirection.Key))
                                    : source.OrderBy(GetExpression<TSource, Guid>(sortDirection.Key));
                            }
                            throw new ArgumentOutOfRangeException();
                    }

                    #endregion OrderBy
                }
                else
                {
                    #region OrderByDescending

                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.String: return source.OrderByDescending(GetExpression<TSource, string>(sortDirection.Key));
                        case TypeCode.Boolean:
                            {
                                return isNullable
                                    ? source.OrderByDescending(GetExpression<TSource, bool?>(sortDirection.Key))
                                    : source.OrderByDescending(GetExpression<TSource, bool>(sortDirection.Key));
                            }
                        case TypeCode.Int16:
                            {
                                return isNullable
                                    ? source.OrderByDescending(GetExpression<TSource, short?>(sortDirection.Key))
                                    : source.OrderByDescending(GetExpression<TSource, short>(sortDirection.Key));
                            }
                        case TypeCode.Int32:
                            {
                                return isNullable
                                    ? source.OrderByDescending(GetExpression<TSource, int?>(sortDirection.Key))
                                    : source.OrderByDescending(GetExpression<TSource, int>(sortDirection.Key));
                            }
                        case TypeCode.Int64:
                            {
                                return isNullable
                                    ? source.OrderByDescending(GetExpression<TSource, long?>(sortDirection.Key))
                                    : source.OrderByDescending(GetExpression<TSource, long>(sortDirection.Key));
                            }
                        case TypeCode.Single:
                            {
                                return isNullable
                                    ? source.OrderByDescending(GetExpression<TSource, float?>(sortDirection.Key))
                                    : source.OrderByDescending(GetExpression<TSource, float>(sortDirection.Key));
                            }
                        case TypeCode.Byte:
                            {
                                return isNullable
                                    ? source.OrderByDescending(GetExpression<TSource, byte?>(sortDirection.Key))
                                    : source.OrderByDescending(GetExpression<TSource, byte>(sortDirection.Key));
                            }
                        case TypeCode.Decimal:
                            {
                                return isNullable
                                    ? source.OrderByDescending(GetExpression<TSource, decimal?>(sortDirection.Key))
                                    : source.OrderByDescending(GetExpression<TSource, decimal>(sortDirection.Key));
                            }
                        case TypeCode.DateTime:
                            {
                                return isNullable
                                    ? source.OrderByDescending(GetExpression<TSource, DateTime?>(sortDirection.Key))
                                    : source.OrderByDescending(GetExpression<TSource, DateTime>(sortDirection.Key));
                            }

                        default:
                            if (type == typeof(Guid))
                            {
                                return isNullable
                                    ? source.OrderByDescending(GetExpression<TSource, Guid?>(sortDirection.Key))
                                    : source.OrderByDescending(GetExpression<TSource, Guid>(sortDirection.Key));
                            }
                            throw new ArgumentOutOfRangeException();
                    }

                    #endregion OrderByDescending
                }
            }
            else
            {
                var orderedQueryable = source as IOrderedQueryable<TSource>;

                if (sortDirection.Value == SortDirection.Ascending)
                {
                    #region ThenBy

                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.String: return orderedQueryable.ThenBy(GetExpression<TSource, string>(sortDirection.Key));
                        case TypeCode.Boolean:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenBy(GetExpression<TSource, bool?>(sortDirection.Key))
                                    : orderedQueryable.ThenBy(GetExpression<TSource, bool>(sortDirection.Key));
                            }
                        case TypeCode.Int16:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenBy(GetExpression<TSource, short?>(sortDirection.Key))
                                    : orderedQueryable.ThenBy(GetExpression<TSource, short>(sortDirection.Key));
                            }
                        case TypeCode.Int32:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenBy(GetExpression<TSource, int?>(sortDirection.Key))
                                    : orderedQueryable.ThenBy(GetExpression<TSource, int>(sortDirection.Key));
                            }
                        case TypeCode.Int64:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenBy(GetExpression<TSource, long?>(sortDirection.Key))
                                    : orderedQueryable.ThenBy(GetExpression<TSource, long>(sortDirection.Key));
                            }
                        case TypeCode.Single:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenBy(GetExpression<TSource, float?>(sortDirection.Key))
                                    : orderedQueryable.ThenBy(GetExpression<TSource, float>(sortDirection.Key));
                            }
                        case TypeCode.DateTime:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenBy(GetExpression<TSource, DateTime?>(sortDirection.Key))
                                    : orderedQueryable.ThenBy(GetExpression<TSource, DateTime>(sortDirection.Key));
                            }
                        case TypeCode.Byte:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenBy(GetExpression<TSource, byte?>(sortDirection.Key))
                                    : orderedQueryable.ThenBy(GetExpression<TSource, byte>(sortDirection.Key));
                            }
                        case TypeCode.Decimal:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenBy(GetExpression<TSource, decimal?>(sortDirection.Key))
                                    : orderedQueryable.ThenBy(GetExpression<TSource, decimal>(sortDirection.Key));
                            }

                        default:
                            if (type == typeof(Guid))
                            {
                                return isNullable
                                    ? orderedQueryable.ThenBy(GetExpression<TSource, Guid?>(sortDirection.Key))
                                    : orderedQueryable.ThenBy(GetExpression<TSource, Guid>(sortDirection.Key));
                            }
                            throw new ArgumentOutOfRangeException();
                    }

                    #endregion ThenBy
                }
                else
                {
                    #region ThenByDescending

                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.String: return orderedQueryable.ThenByDescending(GetExpression<TSource, string>(sortDirection.Key));
                        case TypeCode.Boolean:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenByDescending(GetExpression<TSource, bool?>(sortDirection.Key))
                                    : orderedQueryable.ThenByDescending(GetExpression<TSource, bool>(sortDirection.Key));
                            }
                        case TypeCode.Int16:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenByDescending(GetExpression<TSource, short?>(sortDirection.Key))
                                    : orderedQueryable.ThenByDescending(GetExpression<TSource, short>(sortDirection.Key));
                            }
                        case TypeCode.Int32:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenByDescending(GetExpression<TSource, int?>(sortDirection.Key))
                                    : orderedQueryable.ThenByDescending(GetExpression<TSource, int>(sortDirection.Key));
                            }
                        case TypeCode.Int64:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenByDescending(GetExpression<TSource, long?>(sortDirection.Key))
                                    : orderedQueryable.ThenByDescending(GetExpression<TSource, long>(sortDirection.Key));
                            }
                        case TypeCode.Single:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenByDescending(GetExpression<TSource, float?>(sortDirection.Key))
                                    : orderedQueryable.ThenByDescending(GetExpression<TSource, float>(sortDirection.Key));
                            }
                        case TypeCode.DateTime:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenByDescending(GetExpression<TSource, DateTime?>(sortDirection.Key))
                                    : orderedQueryable.ThenByDescending(GetExpression<TSource, DateTime>(sortDirection.Key));
                            }
                        case TypeCode.Byte:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenByDescending(GetExpression<TSource, byte?>(sortDirection.Key))
                                    : orderedQueryable.ThenByDescending(GetExpression<TSource, byte>(sortDirection.Key));
                            }
                        case TypeCode.Decimal:
                            {
                                return isNullable
                                    ? orderedQueryable.ThenByDescending(GetExpression<TSource, decimal?>(sortDirection.Key))
                                    : orderedQueryable.ThenByDescending(GetExpression<TSource, decimal>(sortDirection.Key));
                            }

                        default:
                            if (type == typeof(Guid))
                            {
                                return isNullable
                                    ? orderedQueryable.ThenByDescending(GetExpression<TSource, Guid?>(sortDirection.Key))
                                    : orderedQueryable.ThenByDescending(GetExpression<TSource, Guid>(sortDirection.Key));
                            }
                            throw new ArgumentOutOfRangeException();
                    }

                    #endregion ThenByDescending
                }
            }
        }

        #endregion Non-Public Methods
    }
}