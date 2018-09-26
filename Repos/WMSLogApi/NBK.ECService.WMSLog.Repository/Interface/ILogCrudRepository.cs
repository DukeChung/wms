using System;
using Abp.Domain.Repositories;
using Abp.Domain.Entities;
using System.Linq;
using NBK.ECService.WMSLog.DTO;

namespace NBK.ECService.WMSLog.Repository.Interface
{
    public interface ILogCrudRepository: ISimpleRepository<Guid>
    {
        /// <summary>
        /// 根据lambda 获取相关数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetQuery<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambda)
            where TEntity : class, ISysIdEntity;

        /// <summary>
        /// 传入IQueryable 跟分页参数封装成Pages对象
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="pagedquery"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        Pages<TDto> ConvertPages<TDto>(IQueryable<TDto> pagedquery, BaseQuery page);
    }
}