using System;
using Abp.EntityFramework;
using Abp.EntityFramework.SimpleRepositories;
using NBK.ECService.WMSLog.Model;
using NBK.ECService.WMSLog.Repository.Interface;
using System.Linq;
using Abp.Domain.Entities;
using NBK.ECService.WMSLog.DTO;

namespace NBK.ECService.WMSLog.Repository
{
    public class LogCrudRepository : EfSimpleRepositoryBase<NBK_WMS_LogContext, Guid>, ILogCrudRepository
    {
        public LogCrudRepository(IDbContextProvider<NBK_WMS_LogContext> dbContextProvider) : base(dbContextProvider) { }

        /// <summary>
        /// 根据lambda 获取相关数据
        /// </summary>
        /// <typeparam name="TEntity">数据库模型</typeparam>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetQuery<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambda)
            where TEntity : class, ISysIdEntity
        {
            //var redisDBSet = RedisWMS.GetRedisList<TEntity>();
            //if (redisDBSet == null)
            //{
            var dbSet = this.Table<TEntity>();
            IQueryable<TEntity> rlst = dbSet.Where(whereLambda).AsQueryable();
            return rlst;
            //  }
            // else
            // {
            // return redisDBSet.Where(whereLambda).AsQueryable();
            // } 
        }

        /// <summary>
        /// 传入入IQueryable 跟分页参数封装成Pages对象
        /// </summary>
        /// <typeparam name="TDto">DTO模型</typeparam>
        /// <param name="pagedquery"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public Pages<TDto> ConvertPages<TDto>(IQueryable<TDto> pagedquery, BaseQuery page)
        {
            var response = new Pages<TDto>();
            var list = pagedquery.ToList();
            response.TableResuls = new TableResults<TDto>()
            {
                aaData = list,
                iTotalDisplayRecords = page.iTotalDisplayRecords,
                iTotalRecords = list.Count(),
                sEcho = page.sEcho
            };
            return response;
        }
    }
}