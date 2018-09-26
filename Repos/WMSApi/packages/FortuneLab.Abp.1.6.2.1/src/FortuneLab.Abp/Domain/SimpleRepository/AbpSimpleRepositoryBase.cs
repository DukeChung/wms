using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Abp.Domain.Repositories
{
    public abstract class AbpSimpleRepositoryBase<TPrimaryKey> : ISimpleRepository<TPrimaryKey>
    {
        public abstract string ConnectionString { get; }
        public abstract IDbConnection Connection { get; }
        public abstract IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, ISysIdEntity<TPrimaryKey>;

        public virtual List<TEntity> GetAllList<TEntity>()
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return GetAll<TEntity>().ToList();
        }

        public virtual Task<List<TEntity>> GetAllListAsync<TEntity>()
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(GetAllList<TEntity>());
        }

        public virtual List<TEntity> GetAllList<TEntity>(Expression<Func<TEntity, bool>> predicate)
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return GetAll<TEntity>().Where(predicate).ToList();
        }

        public virtual Task<List<TEntity>> GetAllListAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
               where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(GetAllList(predicate));
        }

        public virtual T Query<TEntity, T>(Func<IQueryable<TEntity>, T> queryMethod)
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return queryMethod(GetAll<TEntity>());
        }

        public virtual TEntity Get<TEntity>(TPrimaryKey sysId)
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            var entity = FirstOrDefault<TEntity>(sysId);
            //if (entity == null)
            //{
            //    throw new AbpException("指定的SysId不存在。 type: " + typeof(TEntity).FullName + ", SysId: " + sysId);
            //}

            return entity;
        }

        public virtual async Task<TEntity> GetAsync<TEntity>(TPrimaryKey sysId)
               where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            var entity = await FirstOrDefaultAsync<TEntity>(sysId);
            //if (entity == null)
            //{
            //    throw new AbpException("指定的SysId不存在。 type: " + typeof(TEntity).FullName + ", SysId: " + sysId);
            //}

            return entity;
        }

        public virtual TEntity Single<TEntity>(Expression<Func<TEntity, bool>> predicate)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return GetAll<TEntity>().Single(predicate);
        }

        public virtual Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
                   where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(Single(predicate));
        }

        public virtual TEntity FirstOrDefault<TEntity>(TPrimaryKey id)
                   where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return GetAll<TEntity>().FirstOrDefault(CreateEqualityExpressionForId<TEntity>(id));
        }

        public virtual Task<TEntity> FirstOrDefaultAsync<TEntity>(TPrimaryKey id)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(FirstOrDefault<TEntity>(id));
        }

        public virtual TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return GetAll<TEntity>().FirstOrDefault(predicate);
        }

        public virtual Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(FirstOrDefault(predicate));
        }

        public virtual TEntity Load<TEntity>(TPrimaryKey id)
            where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Get<TEntity>(id);
        }

        public abstract TEntity Insert<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        public virtual Task<TEntity> InsertAsync<TEntity>(TEntity entity)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(Insert(entity));
        }

        public virtual TPrimaryKey InsertAndGetId<TEntity>(TEntity entity)
            where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Insert(entity).SysId;
        }

        public virtual Task<TPrimaryKey> InsertAndGetIdAsync<TEntity>(TEntity entity)
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(InsertAndGetId(entity));
        }

        public virtual TEntity InsertOrUpdate<TEntity>(TEntity entity)
               where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return EqualityComparer<TPrimaryKey>.Default.Equals(entity.SysId, default(TPrimaryKey))
                ? Insert(entity)
                : Update(entity);
        }

        public virtual async Task<TEntity> InsertOrUpdateAsync<TEntity>(TEntity entity)
               where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return EqualityComparer<TPrimaryKey>.Default.Equals(entity.SysId, default(TPrimaryKey))
                ? await InsertAsync(entity)
                : await UpdateAsync(entity);
        }

        public virtual TPrimaryKey InsertOrUpdateAndGetId<TEntity>(TEntity entity)
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return InsertOrUpdate(entity).SysId;
        }

        public virtual Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync<TEntity>(TEntity entity)
                    where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(InsertOrUpdateAndGetId(entity));
        }

        public abstract TEntity Update<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        public virtual Task<TEntity> UpdateAsync<TEntity>(TEntity entity)
                   where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(Update(entity));
        }

        public virtual TEntity Update<TEntity>(TPrimaryKey id, Action<TEntity> updateAction)
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            var entity = Get<TEntity>(id);
            updateAction(entity);
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync<TEntity>(TPrimaryKey id, Func<TEntity, Task> updateAction)
               where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            var entity = await GetAsync<TEntity>(id);
            await updateAction(entity);
            return entity;
        }

        public abstract void Delete<TEntity>(TEntity entity)
            where TEntity : class, ISysIdEntity<TPrimaryKey>;

        public virtual async Task DeleteAsync<TEntity>(TEntity entity)
               where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            Delete(entity);
        }

        public abstract void Delete<TEntity>(TPrimaryKey id) where TEntity : class, ISysIdEntity<TPrimaryKey>;

        public virtual async Task DeleteAsync<TEntity>(TPrimaryKey id)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            Delete<TEntity>(id);
        }

        /// <summary>
        /// 循环单条删除符合指定条件表达式的记录
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        public virtual void Delete<TEntity>(Expression<Func<TEntity, bool>> predicate)
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            foreach (var entity in GetAll<TEntity>().Where(predicate).ToList())
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// 异步执行：循环单条删除符合指定条件表达式的记录
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        public virtual async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            Delete(predicate);
        }

        public virtual int Count<TEntity>()
            where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return GetAll<TEntity>().Count();
        }

        public virtual Task<int> CountAsync<TEntity>()
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(Count<TEntity>());
        }

        public virtual int Count<TEntity>(Expression<Func<TEntity, bool>> predicate)
               where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return GetAll<TEntity>().Where(predicate).Count();
        }

        public virtual Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
                 where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(Count<TEntity>(predicate));
        }

        public virtual long LongCount<TEntity>()
                   where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return GetAll<TEntity>().LongCount<TEntity>();
        }

        public virtual Task<long> LongCountAsync<TEntity>()
                   where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(LongCount<TEntity>());
        }

        public virtual long LongCount<TEntity>(Expression<Func<TEntity, bool>> predicate)
              where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return GetAll<TEntity>().Where(predicate).LongCount();
        }

        public virtual Task<long> LongCountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Task.FromResult(LongCount<TEntity>(predicate));
        }

        //LYM 增加批量更新(用到EntityFramework.Extended中的扩展方法)
        /// <summary>
        /// 通过条件批量更新
        /// 特别注意：此方法会直接生成带where条件的update SQL语句，一次性更新多条记录。不会产生AbpDbContext.cs中EntityUpdatedEvent的事件！
        /// </summary>
        /// <param name="predicate">更新符合此条件的记录</param>
        /// <param name="updateExpression">要执行的更新表达式</param>
        /// <returns>被更新的记录数</returns>
        public virtual int BatchUpdate<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
                   where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return 0;
        }

        /// <summary>
        /// 异步执行：通过条件批量更新
        /// 特别注意：此方法会直接生成带where条件的update SQL语句，一次性更新多条记录。不会产生AbpDbContext.cs中EntityUpdatedEvent的事件！
        /// </summary>
        /// <param name="predicate">更新符合此条件的记录</param>
        /// <param name="updateExpression">要执行的更新表达式</param>
        /// <returns>被更新的记录数</returns>
        public virtual async Task<int> BatchUpdateAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return 0;
        }

        /// <summary>
        /// 循环单条删除指定Id列表中的记录
        /// </summary>
        /// <param name="idList">Id列表</param>
        public virtual void Delete<TEntity>(IEnumerable<TPrimaryKey> idList)
                 where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            Delete<TEntity>(entity => idList.Contains(entity.SysId));
        }

        /// <summary>
        /// 异步执行：循环单条删除指定Id列表中的记录
        /// </summary>
        /// <param name="idList">Id列表</param>
        public virtual async Task DeleteAsync<TEntity>(IEnumerable<TPrimaryKey> idList)
                   where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            await DeleteAsync<TEntity>(entity => idList.Contains(entity.SysId));
        }

        /// <summary>
        /// 批量删除符合指定条件表达式的记录
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        public virtual void BatchDelete<TEntity>(Expression<Func<TEntity, bool>> predicate)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            Delete<TEntity>(predicate);
        }

        /// <summary>
        /// 异步执行：批量删除符合指定条件表达式的记录
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        public virtual async Task BatchDeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            await DeleteAsync<TEntity>(predicate);
        }

        /// <summary>
        /// 批量删除指定Id列表中的记录
        /// </summary>
        /// <param name="idList">Id列表</param>
        public virtual void BatchDelete<TEntity>(IEnumerable<TPrimaryKey> idList)
                  where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            Delete<TEntity>(idList);
        }

        /// <summary>
        /// 异步执行：批量删除指定Id列表中的记录
        /// </summary>
        /// <param name="idList">Id列表</param>
        public virtual async Task BatchDeleteAsync<TEntity>(IEnumerable<TPrimaryKey> idList)
                   where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            await DeleteAsync<TEntity>(idList);
        }

        public virtual void BatchUpdateDisplayOrder<TEntity>(IEnumerable<TPrimaryKey> idList)
                   where TEntity : class, ISysIdEntity<TPrimaryKey>
        {

        }

        public virtual async Task BatchUpdateDisplayOrderAsync<TEntity>(IEnumerable<TPrimaryKey> idList)
                   where TEntity : class, ISysIdEntity<TPrimaryKey>
        {

        }

        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId<TEntity>(TPrimaryKey sysId)
                 where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "SysId"),
                Expression.Constant(sysId, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}
