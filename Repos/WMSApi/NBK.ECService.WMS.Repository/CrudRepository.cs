//  

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using Abp.EntityFramework;
using Abp.EntityFramework.SimpleRepositories;
using NBK.ECService.WMS.Model.Models;
using Abp.Domain.Entities;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility.Enum;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq.Expressions;
using Abp.WebApi.Controllers;
using Abp.Application.Services;
using NBK.ECService.WMS.Utility.Redis;
using System.Data.Entity.Validation;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO.ThirdParty;

namespace NBK.ECService.WMS.Repository
{
    // EfSimpleRepositoryBase<LandRoverDbContext, Guid>,
    public class CrudRepository : EfSimpleRepositoryBase<NBK_WMSContext, Guid>, ICrudRepository
    {
        private static readonly object obj = new object();

        /// <param name="dbContextProvider"></param>
        public CrudRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
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
                query.ParmsObj = new { WarehouseSysId = wareHouseSysId, IsWrite = true };
                var response = ApiClient.Post<string>(PublicConst.WmsApiUrl, "WareHouse/GetConnectionStringByWarehouseSysId", query);
                if (response.Success)
                {
                    connectionStr = response.ResponseResult;
                }
            }
            else
            {
                connectionStr = ware.ConnectionString;
            }
            if (string.IsNullOrEmpty(connectionStr))
            {
                throw new Exception($"根据WareHouseId : {wareHouseSysId.ToString()} 未找到仓库信息,请联系管理员!");
            }
            this.Context.ChangeDB(StringHelper.DecryptDES(connectionStr)); 
        }

        /// <summary>
        /// 批量写入数据库
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="list"></param>
        public void BatchInsert<TEntity>(List<TEntity> list)
            where TEntity : class, ISysIdEntity
        {
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

        #region 日志
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="obj">实体对象</param>
        public void SetOperationLog(OperationLogType type, object obj, string descr = "", string userName = "", int userId = 1)
        {
            var stackTrace = new StackTrace();

            var ctrlStackFrame = stackTrace.GetFrames().FirstOrDefault(
                p => p.GetMethod() != null
                && p.GetMethod().ReflectedType != null
                && p.GetMethod().ReflectedType.IsSubclassOf(typeof(AbpApiController)));

            var appStackFrame = stackTrace.GetFrames().FirstOrDefault(
                p => p.GetMethod() != null
                && p.GetMethod().ReflectedType != null
                && p.GetMethod().ReflectedType.IsSubclassOf(typeof(ApplicationService)));

            operationlog operationlog = new operationlog
            {
                SysId = Guid.NewGuid(),
                Type = (int)type,
                ApiController = ctrlStackFrame != null ? ctrlStackFrame.GetMethod().ReflectedType.Name : string.Empty,
                AppService = appStackFrame != null ? appStackFrame.GetMethod().Name : string.Empty,
                JsonValue = JsonConvert.SerializeObject(obj),
                Descr = descr,
                UserName = userName,
                CreateBy = userId,
                CreateDate = DateTime.Now
            };
            this.Insert(operationlog);
        }

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
        public void SetOperationLog(operationlog operationLog)
        {
            this.Insert(operationLog);
        }

        #endregion

        #region 单号生成
        public string GenNextNumber(string tableName)
        {
            lock (obj)
            {
                var iNextNumber = 0;
                var sRtnString = string.Empty;
                var serverDate = DateTime.Now;
                var dbList = this.Table<nextnumbergen>().Where(x => x.KeyName == tableName);
                if (!dbList.Any())
                {
                    throw new Exception("没有配置对应的单号生成规则");
                }

                var db = dbList.FirstOrDefault();

                if (db.IsDateBased == PublicConst.Yes && db.UpdateDate < serverDate)
                {
                    iNextNumber = db.StartNumber.Value;
                }
                else
                {
                    iNextNumber = db.NextNumber.Value + 1;
                }
                var number = genString(iNextNumber, tableName, db.AlphaPrefix, db.AlphaSuffix,
                     db.IsDateBased == PublicConst.Yes ? true : false,
                     db.LeadingZeros == PublicConst.Yes ? true : false,
                     serverDate, db.TotalLength.Value);
                db.NextNumber = iNextNumber;
                db.UpdateDate = serverDate;
                this.Context.SaveChanges();
                return number;

            }
        }

        private string genString(int iNumber, string sTableName, string sAlphaPrefix, string sAlphaSufFix,
            bool bDateBased, bool bLeadZero, DateTime sServerDate, int iTotalLenth)
        {
            var sTemp = string.Empty;
            var count = 0;
            sTemp = sAlphaPrefix;

            if (bDateBased)
            {
                sTemp = sTemp + sServerDate.ToString("yyMMdd");
            }
            if (bLeadZero)
            {
                sTemp = sTemp + iNumber.ToString().PadLeft(iTotalLenth, '0');
            }
            else
            {
                sTemp = sTemp + iNumber;
            }


            sTemp = sTemp + sAlphaSufFix;

            switch (sTableName)
            {
                case PublicConst.GenNextNumberPurchase:
                    count = this.Table<purchase>().Count(x => x.PurchaseOrder == sTemp);
                    break;
                case PublicConst.GenNextNumberReceipt:
                    count = this.Table<receipt>().Count(x => x.ReceiptOrder == sTemp);
                    break;
                case PublicConst.GenNextNumberReceiptSn:
                    count = this.Table<receiptsn>().Count(x => x.SN == sTemp);
                    break;
                case PublicConst.GenNextNumberLot:
                    count = this.Table<receiptdetail>().Count(x => x.ToLot == sTemp);
                    break;
                case PublicConst.GenNextNumberSku:
                    count = this.Table<sku>().Count(x => x.SkuCode == sTemp);
                    break;
                case PublicConst.GenNextNumberVanning:
                    count = this.Table<vanning>().Count(x => x.VanningOrder == sTemp);
                    break;
                case PublicConst.GenNextNumberHandoverGroup:
                    count = this.Table<vanningdetail>().Count(x => x.HandoverGroupOrder == sTemp);
                    break;
                case PublicConst.GenNextNumberAdjustment:
                    count = this.Table<adjustment>().Count(x => x.AdjustmentOrder == sTemp);
                    break;
                case PublicConst.GenNextNumberStockTransfer:
                    count = this.Table<stocktransfer>().Count(x => x.StockTransferOrder == sTemp);
                    break;
                case PublicConst.GenNextNumberStockMovement:
                    count = this.Table<stockmovement>().Count(x => x.StockMovementOrder == sTemp);
                    break;
                case PublicConst.GenNextNumberAssembly:
                    count = this.Table<assembly>().Count(x => x.AssemblyOrder == sTemp);
                    break;
                case PublicConst.GenNextNumberTransferInventory:
                    count = this.Table<transferinventory>().Count(x => x.TransferInventoryOrder == sTemp);
                    break;
            }

            //检查组合的字符串是否在表中唯一，如果不唯一，将iNextNumber+1重新组合，直至字符串唯一
            iNumber++;
            if (count != 0)
            {
                sTemp = genString(iNumber, sTableName, sAlphaPrefix, sAlphaSufFix, bDateBased, bLeadZero, sServerDate, iTotalLenth);

            }
            else
            {
                ////需要更新NEXT_NUMBER_GEN
                //sSQL = "UPDATE @_@NEXT_NUMBER_GEN SET NEXT_NUMBER=" + StrUtils.FormatSQLInt(iNumber) + ", UPDATED_DATE=GETDATE() WHERE KEY_NAME ="
                //        + StrUtils.FormatSQLStr(sTableName);
                //sSQL = sSQL.Replace("@_@", whLoginID);
                //dataBase.ExecuteNonQuery(tran, CommandType.Text, sSQL);
            }

            return sTemp;
        }

        #endregion

        #region 批量单号生成
        /// <summary>
        /// 批量生成单号
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<string> BatchGenNextNumber(string tableName, int num)
        {
            lock (obj)
            {
                var numberList = new List<string>();
                var iNextNumber = 0;
                var newNextNumber = 0;
                var sRtnString = string.Empty;
                var serverDate = DateTime.Now;
                var dbList = this.Table<nextnumbergen>().Where(x => x.KeyName == tableName);
                if (!dbList.Any())
                {
                    throw new Exception("没有配置对应的单号生成规则");
                }

                var db = dbList.FirstOrDefault();


                iNextNumber = db.NextNumber.Value + num;
                newNextNumber = db.NextNumber.Value;
                db.NextNumber = iNextNumber;
                db.UpdateDate = serverDate;
                this.Context.SaveChanges();

                for (var i = 0; i < num; i++)
                {
                    newNextNumber++;
                    var number = BatchGenString(newNextNumber, tableName, db.AlphaPrefix, db.AlphaSuffix,
                         db.IsDateBased == PublicConst.Yes ? true : false,
                         db.LeadingZeros == PublicConst.Yes ? true : false,
                         serverDate, db.TotalLength.Value);
                    numberList.Add(number);
                }

                return numberList;
            }
        }

        private string BatchGenString(int iNumber, string sTableName, string sAlphaPrefix, string sAlphaSufFix,
           bool bDateBased, bool bLeadZero, DateTime sServerDate, int iTotalLenth)
        {
            var sTemp = string.Empty;

            sTemp = sAlphaPrefix;

            if (bDateBased)
            {
                sTemp = sTemp + sServerDate.ToString("yyMMdd");
            }
            if (bLeadZero)
            {
                sTemp = sTemp + iNumber.ToString().PadLeft(iTotalLenth, '0');
            }
            else
            {
                sTemp = sTemp + iNumber;
            }


            sTemp = sTemp + sAlphaSufFix;

            return sTemp;
        }
        #endregion
    }
}