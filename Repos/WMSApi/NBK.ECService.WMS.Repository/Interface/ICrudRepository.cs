//  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using FortuneLab.Models;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface ICrudRepository: ISimpleRepository<Guid>
    {
        void BatchInsert<TEntity>(List<TEntity> list)
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
        IQueryable<TEntity> GetQuery<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambda)
            where TEntity : class, ISysIdEntity;

        /// <summary>
        /// 单号生成规则
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string GenNextNumber(string tableName);

        /// <summary>
        /// 批量生成单号
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<string> BatchGenNextNumber(string tableName, int num);

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="obj">实体对象</param>
        void SetOperationLog(OperationLogType type, object obj, string descr = "", string userName = "", int userId = 1);

        /// <summary>
        /// 写入日志
        /// Type 对应OperationLogType枚举
        /// ApiController 请求的 WeiApi
        /// AppService 请求的服务
        /// Descr 日志描述
        /// JsonValue 传入或返回的数据结果的Json
        /// UserName 日志写入用户的名称 
        /// CreateBy 日志写入的用户ID
        /// CreateDate 日志写入的时间 
        /// </summary>
        /// <param name="operationLog"></param>
        void SetOperationLog(operationlog operationLog);

        void SaveChange();

        void ChangeDB(Guid wareHouseSysId);
    }
}