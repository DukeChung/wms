using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.SimpleRepositories;
using Abp.WebApi.Controllers;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Model;
using NBK.ECService.WMSReport.Repository.Interface;
using Newtonsoft.Json;
using Abp.Domain.Repositories;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSReport.Model.Models;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Redis;

namespace NBK.ECService.WMSReport.Repository
{
    public class CrudRepository : EfSimpleRepositoryBase<NBK_WMS_ReportContext, Guid>, ICrudRepository
    {
        private static readonly object obj = new object();

        /// <param name="dbContextProvider"></param>
        public CrudRepository(IDbContextProvider<NBK_WMS_ReportContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public void SaveChange()
        {
            this.Context.SaveChanges();
        }

        public void ChangeDB(Guid wareHouseSysId)
        {
            var wareHouseList = RedisWMS.GetRedisList<List<warehouse>>(RedisSourceKey.RedisWareHouseList);
            if (wareHouseList == null || !wareHouseList.Any())
            {
                var response = ApiClient.Post<string>(PublicConst.WmsApiUrl, "Redis/SynchroWarehouse", new CoreQuery());
                if (response.Success)
                {
                    wareHouseList = RedisWMS.GetRedisList<List<warehouse>>(RedisSourceKey.RedisWareHouseList);
                }
            }
            string connectionStr = string.Empty;
            var ware = wareHouseList.FirstOrDefault(x => x.SysId == wareHouseSysId);
            if (ware == null)
            {
                var query = new CoreQuery();
                query.ParmsObj = new { WarehouseSysId = wareHouseSysId, IsWrite = false };
                var response = ApiClient.Post<string>(PublicConst.WmsApiUrl, "WareHouse/GetConnectionStringByWarehouseSysId", query);
                if (response.Success)
                {
                    connectionStr = response.ResponseResult;
                }
            }
            else
            {
                connectionStr = ware.ConnectionStringRead;
            }
            if (string.IsNullOrEmpty(connectionStr))
            {
                throw new Exception($"根据WareHouseId : {wareHouseSysId.ToString()} 未找到仓库信息,请联系管理员!");
            }
            this.Context.ChangeDB(StringHelper.DecryptDES(connectionStr));
        }

        /// <summary>
        /// 修改日志报表查询连接字符串
        /// </summary>
        public void ChangeLogDB()
        {
            this.Context.ChangeDB(PublicConst.nbk_wms_LogContext);
        }

        /// <summary>
        /// 修改全局仓报表查询连接字符串
        /// </summary>
        public void ChangeGlobalDB()
        {
            this.Context.ChangeDB(PublicConst.nbk_wms_GlobalContext);
        }


        /// <summary>
        /// 批量写入数据库
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="list"></param>
        public void BatchInsert<TEntity>(List<TEntity> list, Guid wareHouseSysId)
            where TEntity : class, ISysIdEntity
        {
            ChangeDB(wareHouseSysId);
            foreach (var info in list)
            {
                this.Insert<TEntity>(info);
            }
        }

        /// <summary>
        /// 泛型分页查询 
        /// </summary>
        /// <typeparam name="TEntity">数据库实体</typeparam>
        /// <param name="page">分页参数</param>
        /// <param name="whereLambda">查询条件</param>
        /// <returns>返回业务实体分页数据</returns>
        private IQueryable<TEntity> GetQueryableByPage<TEntity>(BaseQuery page,
            System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambda = null)
            where TEntity : class, ISysIdEntity
        {
            var dbSet = this.Table<TEntity>();
            IQueryable<TEntity> rlst = null;
            rlst = dbSet.Where(whereLambda).AsQueryable();

            //排序字段如果为空，
            if (string.IsNullOrEmpty(page.mDataProp_1))
                page.mDataProp_1 = "SysId";

            rlst = rlst.OrderBy(page.mDataProp_1, true);
            page.iTotalDisplayRecords = rlst.Count();
            return rlst.Skip(page.iDisplayStart).Take(page.iDisplayLength);
        }

        /// <summary>
        ///  泛型分页查询
        /// </summary>
        /// <typeparam name="TEntity">数据库实体</typeparam>
        /// <typeparam name="TDto">业务实体</typeparam>
        /// <param name="page">分页参数</param>
        /// <param name="whereLambda">查询条件</param>
        /// <returns>返回业务实体分页数据</returns>
        public Pages<TDto> GetQueryableByPage<TEntity, TDto>(BaseQuery page,
            System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambda = null)
            where TEntity : class, ISysIdEntity where TDto : class
        {
            var pagedquery = GetQueryableByPage<TEntity>(page, whereLambda);
            return ConvertPages<TEntity, TDto>(pagedquery, page);
        }

        /// <summary>
        /// 根据lambda 获取相关数据
        /// </summary>
        /// <typeparam name="TEntity">数据库模型</typeparam>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetQuery<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambda, Guid? wareHouseSysId = null)
            where TEntity : class, ISysIdEntity
        {
            if (wareHouseSysId.HasValue)
            {
                ChangeDB(wareHouseSysId.Value);
            }
            var dbSet = this.Table<TEntity>();
            IQueryable<TEntity> rlst = dbSet.Where(whereLambda).AsQueryable();
            return rlst;

        }

        public IQueryable<TEntity> GetAll<TEntity>(Guid wareHouseSysId) where TEntity : class, ISysIdEntity
        {
            ChangeDB(wareHouseSysId);
            return this.Table<TEntity>();
        }

        /// <summary>
        /// 传入入IQueryable 跟分页参数封装成Pages对象
        /// </summary>
        /// <typeparam name="TEntity">数据库模型</typeparam>
        /// <typeparam name="TDto">DTO模型</typeparam>
        /// <param name="pagedquery"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public Pages<TDto> ConvertPages<TEntity, TDto>(IQueryable<TEntity> pagedquery, BaseQuery page)
        {
            var response = new Pages<TDto>();
            var list = pagedquery.ToList().TransformTo<TDto>();
            response.TableResuls = new TableResults<TDto>()
            {
                aaData = list,
                iTotalDisplayRecords = page.iTotalDisplayRecords,
                iTotalRecords = list.Count(),
                sEcho = page.sEcho
            };
            return response;
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