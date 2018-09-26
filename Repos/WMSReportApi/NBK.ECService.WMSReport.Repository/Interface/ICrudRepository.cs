using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using NBK.ECService.WMSReport.DTO.Base;

namespace NBK.ECService.WMSReport.Repository.Interface
{
    public interface ICrudRepository : ISimpleRepository<Guid>
    {
        void BatchInsert<TEntity>(List<TEntity> list, Guid wareHouseSysId)
              where TEntity : class, ISysIdEntity;
        Pages<TDto> GetQueryableByPage<TEntity, TDto>(BaseQuery page,
            System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambda = null)
            where TEntity : class, ISysIdEntity where TDto : class;

        /// <summary>
        /// 根据lambda 获取相关数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetQuery<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambda, Guid? wareHouseSysId = null)
            where TEntity : class, ISysIdEntity;
        IQueryable<TEntity> GetAll<TEntity>(Guid wareHouseSysId) where TEntity : class, ISysIdEntity;

        void SaveChange();

        void ChangeDB(Guid wareHouseSysId);

        void ChangeLogDB();

        void ChangeGlobalDB();
    }
}