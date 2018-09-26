using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.EntityFramework.SimpleRepositories
{
    /// <summary>
    /// EfSimpleRepository int主键基类
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public abstract class EfSimpleRepositoryBaseForIntKey<TDbContext> : EfSimpleRepositoryBase<TDbContext, int>
              where TDbContext : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public EfSimpleRepositoryBaseForIntKey(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }

    /// <summary>
    /// EfSimpleRepository long主键基类
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public abstract class EfSimpleRepositoryBaseForLongKey<TDbContext> : EfSimpleRepositoryBase<TDbContext, long>
              where TDbContext : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public EfSimpleRepositoryBaseForLongKey(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }

    /// <summary>
    /// EfSimpleRepository GUID主键基类
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public abstract class EfSimpleRepositoryBaseForGuidKey<TDbContext> : EfSimpleRepositoryBase<TDbContext, Guid>
              where TDbContext : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public EfSimpleRepositoryBaseForGuidKey(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        /// <summary>
        /// 添加一个实体， 如果Entity.SysId == Guid.Empty, 会默认给一个Guid
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override TEntity Insert<TEntity>(TEntity entity)
        {
            if (entity.SysId == Guid.Empty)
                entity.SysId = Guid.NewGuid();
            return base.Insert<TEntity>(entity);
        }

        /// <summary>
        /// 添加一个实体， 如果Entity.SysId == Guid.Empty, 会默认给一个Guid
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override Task<TEntity> InsertAsync<TEntity>(TEntity entity)
        {
            if (entity.SysId == Guid.Empty)
                entity.SysId = Guid.NewGuid();
            return base.InsertAsync<TEntity>(entity);
        }
    }
}
