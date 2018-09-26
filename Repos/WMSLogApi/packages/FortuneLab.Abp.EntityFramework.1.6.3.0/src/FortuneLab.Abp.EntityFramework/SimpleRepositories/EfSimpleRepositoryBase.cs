using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.Runtime.Validation;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Abp.EntityFramework.SimpleRepositories
{
    public class EfSimpleRepositoryBase<TDbContext, TPrimaryKey> : AbpSimpleRepositoryBase<TPrimaryKey>
                where TDbContext : DbContext
    {
        /// <summary>
        /// Gets EF DbContext object.
        /// </summary>
        protected virtual TDbContext Context { get { return _dbContextProvider.DbContext; } }

        public override string ConnectionString
        {
            get { return Connection.ConnectionString; }
        }

        public override IDbConnection Connection
        {
            get { return Context.Database.Connection; }
        }

        /// <summary>
        /// Gets DbSet for given entity.
        /// </summary>
        protected virtual DbSet<TEntity> Table<TEntity>()
            where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            return Context.Set<TEntity>();
        }

        private readonly IDbContextProvider<TDbContext> _dbContextProvider;
        protected readonly IAbpSession _session;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public EfSimpleRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
            _session = NullAbpSession.Instance;
            //_session = IocManager.Instance.IocContainer.Resolve<IAbpSession>();
        }

        public override IQueryable<TEntity> GetAll<TEntity>()
        {
            return Table<TEntity>();
        }

        public override async Task<List<TEntity>> GetAllListAsync<TEntity>()
        {
            return await GetAll<TEntity>().ToListAsync();
        }

        public override async Task<List<TEntity>> GetAllListAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll<TEntity>().Where(predicate).ToListAsync();
        }

        public override async Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll<TEntity>().SingleAsync(predicate);
        }

        public override async Task<TEntity> FirstOrDefaultAsync<TEntity>(TPrimaryKey id)
        {
            return await GetAll<TEntity>().FirstOrDefaultAsync(CreateEqualityExpressionForId<TEntity>(id));
        }

        public override async Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public override TEntity Insert<TEntity>(TEntity entity)
        {
            return Table<TEntity>().Add(entity);
        }

        public override Task<TEntity> InsertAsync<TEntity>(TEntity entity)
        {
            return Task.FromResult(Table<TEntity>().Add(entity));
        }

        public override TPrimaryKey InsertAndGetId<TEntity>(TEntity entity)
        {
            entity = Insert(entity);

            if (entity.IsTransient())
            {
                Context.SaveChanges();
            }

            return entity.SysId;
        }

        public override async Task<TPrimaryKey> InsertAndGetIdAsync<TEntity>(TEntity entity)
        {
            entity = await InsertAsync(entity);

            if (entity.IsTransient())
            {
                await Context.SaveChangesAsync();
            }

            return entity.SysId;
        }

        public override TPrimaryKey InsertOrUpdateAndGetId<TEntity>(TEntity entity)
        {
            entity = InsertOrUpdate(entity);

            if (entity.IsTransient())
            {
                Context.SaveChanges();
            }

            return entity.SysId;
        }

        public override async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync<TEntity>(TEntity entity)
        {
            entity = await InsertOrUpdateAsync(entity);

            if (entity.IsTransient())
            {
                await Context.SaveChangesAsync();
            }

            return entity.SysId;
        }

        public override TEntity Update<TEntity>(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public override Task<TEntity> UpdateAsync<TEntity>(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        //LYM 增加批量更新(用到EntityFramework.Extended中的扩展方法)
        /// <summary>
        /// 通过条件批量更新
        /// 特别注意：此方法会直接生成带where条件的update SQL语句，一次性更新多条记录。不会产生AbpDbContext.cs中EntityUpdatedEvent的事件！
        /// </summary>
        /// <param name="predicate">更新符合此条件的记录</param>
        /// <param name="updateExpression">要执行的更新表达式</param>
        /// <returns>被更新的记录数</returns>
        public override int BatchUpdate<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            return GetAll<TEntity>().Where(predicate).Update(updateExpression);
        }

        /// <summary>
        /// 异步执行：通过条件批量更新
        /// 特别注意：此方法会直接生成带where条件的update SQL语句，一次性更新多条记录。不会产生AbpDbContext.cs中EntityUpdatedEvent的事件！
        /// </summary>
        /// <param name="predicate">更新符合此条件的记录</param>
        /// <param name="updateExpression">要执行的更新表达式</param>
        /// <returns>被更新的记录数</returns>
        public override async Task<int> BatchUpdateAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            return await GetAll<TEntity>().Where(predicate).UpdateAsync<TEntity>(updateExpression);
        }

        public override void Delete<TEntity>(TEntity entity)
        {
            AttachIfNot(entity);

            if (entity is ISoftDelete)
            {
                (entity as ISoftDelete).IsDeleted = true;
            }
            else
            {
                Table<TEntity>().Remove(entity);
            }
        }

        public override void Delete<TEntity>(TPrimaryKey id)
        {
            var entity = Table<TEntity>().Local.FirstOrDefault(ent => EqualityComparer<TPrimaryKey>.Default.Equals(ent.SysId, id));
            if (entity == null)
            {
                entity = FirstOrDefault<TEntity>(id);
                if (entity == null)
                {
                    return;
                }
            }

            Delete(entity);
        }

        /// <summary>
        /// 批量删除符合指定条件表达式的记录，此方法使用直接生成SQL的方式，将不能触发EntityDeletedEvent事件！
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        public override void BatchDelete<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            if (typeof(TEntity).GetInterface("ISoftDelete") != null)  //实现了ISoftDelete接口，为软删除
            {
                BatchUpdate<TEntity>(predicate, CreateSoftDeleteExpression<TEntity>());
            }
            else
            {
                //LYM 使用EntityFramework.Extensions的批量删除
                GetAll<TEntity>().Where(predicate).Delete();
            }
        }

        /// <summary>
        /// 异步执行：批量删除符合指定条件表达式的记录，此方法使用直接生成SQL的方式，将不能触发EntityDeletedEvent事件！
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        public override async Task BatchDeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            if (typeof(TEntity).GetInterface("ISoftDelete") != null)  //实现了ISoftDelete接口，为软删除
            {
                await BatchUpdateAsync(predicate, CreateSoftDeleteExpression<TEntity>());
            }
            else
            {
                await GetAll<TEntity>().Where(predicate).DeleteAsync();
            }
        }

        /// <summary>
        /// 批量删除指定Id列表中的记录，此方法使用直接生成SQL的方式，将不能触发EntityDeletedEvent事件！
        /// </summary>
        /// <param name="idList">Id列表</param>
        public override void BatchDelete<TEntity>(IEnumerable<TPrimaryKey> idList)
        {
            if (!idList.Any()) return;
            BatchDelete<TEntity>(entity => idList.Contains(entity.SysId));
        }

        /// <summary>
        /// 异步执行：批量删除指定Id列表中的记录，此方法使用直接生成SQL的方式，将不能触发EntityDeletedEvent事件！
        /// </summary>
        /// <param name="idList">Id列表</param>
        public override async Task BatchDeleteAsync<TEntity>(IEnumerable<TPrimaryKey> idList)
        {
            if (!idList.Any()) return;
            await BatchDeleteAsync<TEntity>(entity => idList.Contains(entity.SysId));
        }

        /// <summary>
        /// 按idList的顺序批量更新显示顺序(DisplayOrder)的值
        /// </summary>
        /// <param name="idList"></param>
        public override void BatchUpdateDisplayOrder<TEntity>(IEnumerable<TPrimaryKey> idList)
        {
            var list = GetAllList<TEntity>(m => idList.Contains(m.SysId));
            var index = 1;
            foreach (TPrimaryKey id in idList)
            {
                var entity = list.First(m => EqualityComparer<TPrimaryKey>.Default.Equals(m.SysId, id));
                ((IDisplayOrderable)entity).DisplayOrder = index++;
            }
        }

        /// <summary>
        /// 按idList的顺序批量更新显示顺序(DisplayOrder)的值
        /// </summary>
        /// <param name="idList"></param>
        public override async Task BatchUpdateDisplayOrderAsync<TEntity>(IEnumerable<TPrimaryKey> idList)
        {
            var list = await GetAllListAsync<TEntity>(m => idList.Contains(m.SysId));
            var index = 1;
            foreach (TPrimaryKey id in idList)
            {
                var entity = list.First(m => EqualityComparer<TPrimaryKey>.Default.Equals(m.SysId, id));
                ((IDisplayOrderable)entity).DisplayOrder = index++;
            }
        }


        public override async Task<int> CountAsync<TEntity>()
        {
            return await GetAll<TEntity>().CountAsync();
        }

        public override async Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll<TEntity>().Where(predicate).CountAsync();
        }

        public override async Task<long> LongCountAsync<TEntity>()
        {
            return await GetAll<TEntity>().LongCountAsync();
        }

        public override async Task<long> LongCountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll<TEntity>().Where(predicate).LongCountAsync();
        }

        protected virtual void AttachIfNot<TEntity>(TEntity entity) where TEntity : class, ISysIdEntity<TPrimaryKey>
        {
            if (!Table<TEntity>().Local.Contains(entity))
            {
                Table<TEntity>().Attach(entity);
            }
        }

        //LYM 2015-1-18 生成SoftDelete的表达式
        private Expression<Func<TEntity, TEntity>> CreateSoftDeleteExpression<TEntity>()
        {
            //生成表达式： entity => new TEntity{IsDeleted = true}
            NewExpression expression = Expression.New(typeof(TEntity));
            MemberInfo memberIsDeleted = typeof(TEntity).GetMember("IsDeleted")[0];
            MemberInfo memberDeletionTime = typeof(TEntity).GetMember("DeletionTime")[0];
            MemberInfo memberDeleterUserId = typeof(TEntity).GetMember("DeleterUserId")[0];
            MemberBinding binding = Expression.Bind(memberIsDeleted, Expression.Constant(true));
            MemberBinding binding2 = Expression.Bind(memberDeletionTime, Expression.Constant(DateTime.Now));
            MemberBinding binding3 = Expression.Bind(memberDeleterUserId, Expression.Constant(_session.UserId));
            MemberInitExpression lambdaBody = Expression.MemberInit(expression, binding, binding2, binding3);
            ParameterExpression param = Expression.Parameter(typeof(TEntity), "entity");
            return Expression.Lambda<Func<TEntity, TEntity>>(lambdaBody, param);
        }
    }
}
