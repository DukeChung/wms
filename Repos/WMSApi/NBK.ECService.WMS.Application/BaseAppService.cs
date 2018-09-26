using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using NBK.ECService.WMS.DTO;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility.Redis;
using NBK.ECService.WMS.DTO.Extend;
using NBK.ECService.WMS.Model.Models;
using System.Threading;
using Abp.Domain.Uow;

namespace NBK.ECService.WMS.Application
{
    public class BaseAppService : WMSApplicationService, IBaseAppService
    {
        private static readonly object obj = new object();
        private ICrudRepository _crudRepository = null;
        private IWMSSqlRepository _wmsSqlRepository = null;

        public BaseAppService(ICrudRepository crudRepository, IWMSSqlRepository wmsSqlRepository)
        {
            this._crudRepository = crudRepository;
            this._wmsSqlRepository = wmsSqlRepository;
        }

        #region 单号生成
        /// <summary>
        /// 生成单号（提供接口服务的方法）
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<string> GenNextNumber(string tableName, int count)
        {
            lock (obj)
            {
                List<string> rslt = new List<string>();
                try
                {
                    var genOrderList = RedisWMS.GetRedisList<List<GenOrderReponseDto>>(RedisSourceKey.RedisGenOrderList);

                    //Redis中不存在单号存放时，第一次给Redis存放单号数据
                    if (genOrderList == null || genOrderList.Count == 0)
                    {
                        genOrderList = new List<GenOrderReponseDto>();

                        var orderList = _crudRepository.BatchGenNextNumber(tableName, PublicConst.SupOrderNumberCount);
                        genOrderList.Add(new GenOrderReponseDto()
                        {
                            TableName = tableName,
                            OrderNumberList = orderList
                        });
                    }

                    var orderRsp = genOrderList.Find(x => x.TableName == tableName);

                    //不存在某种单号时获取单号
                    if (orderRsp == null || orderRsp.OrderNumberList == null || orderRsp.OrderNumberList.Count == 0)
                    {
                        var orderList = _crudRepository.BatchGenNextNumber(tableName, PublicConst.SupOrderNumberCount);
                        genOrderList.Add(new GenOrderReponseDto()
                        {
                            TableName = tableName,
                            OrderNumberList = orderList
                        });
                        orderRsp = genOrderList.Find(x => x.TableName == tableName);
                    }

                    //少于某个数量时，自动补充单号给Redis
                    if (orderRsp.OrderNumberList.Count < (count + PublicConst.RemainOrderNumberCount))
                    {
                        var orderList = _crudRepository.BatchGenNextNumber(tableName, PublicConst.SupOrderNumberCount);
                        orderRsp.OrderNumberList.AddRange(orderList);
                    }

                    //从Redis的List中获取到需要的单号
                    for (var i = 0; i < count; i++)
                    {
                        rslt.Add(orderRsp.OrderNumberList[i]);
                    }

                    //移除已取到的单号
                    orderRsp.OrderNumberList.RemoveRange(0, count);

                    //重新存放到Redis
                    RedisWMS.SetRedis(genOrderList, RedisSourceKey.RedisGenOrderList);

                    if (rslt == null || rslt.Count == 0)
                    {
                        throw new Exception("获取单号失败");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return rslt;
            }
        }

        /// <summary>
        /// 获取单号
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<string> GetNumber(string tableName, int count)
        {
            try
            {
                var orderList = new List<string>();
                var alphaPrefix = GetAlphaPrefix(tableName);

                for (var i = 0; i < count; i++)
                {
                    var number = _wmsSqlRepository.AutoNextNumber();
                    var order = alphaPrefix + DateTime.Now.ToString("yyMMdd") + number;
                    orderList.Add(order);
                }

                //Random rd = new Random();
                //for(var i = 0; i < count; i++)
                //{
                //    DateTime dt = DateTime.Now;
                //    var order = alphaPrefix + "0" + dt.ToString("MMddHHmmss") + dt.Millisecond.ToString() + rd.Next(10, 99).ToString() + i.ToString();
                //    orderList.Add(order);
                //}

                return orderList;

                #region 屏蔽
                //var query = new CoreQuery();
                //query.ParmsObj = new { tableName, count };
                //var response = ApiClient.Post<List<string>>(PublicConst.WmsApiUrl, "/Base/GenNextNumber", query);
                //if (response != null && response.Success && response.ResponseResult != null)
                //{
                //    return response.ResponseResult;
                //}
                //else
                //{
                //    throw new Exception("单号生成失败");
                //}
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("生成单号出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取单号
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public string GetNumber(string tableName)
        {
            try
            {
                var alphaPrefix = GetAlphaPrefix(tableName);
                //Random rd = new Random();
                //DateTime dt = DateTime.Now;
                //var order = alphaPrefix + "0" + dt.ToString("MMddHHmmss") + dt.Millisecond.ToString() + rd.Next(1000,9999).ToString();
                var number = _wmsSqlRepository.AutoNextNumber();
                var order = alphaPrefix + DateTime.Now.ToString("yyMMdd") + number;
                return order;

                #region 屏蔽
                //var genOrderList = GetNumber(tableName, 1);
                //if(genOrderList != null && genOrderList.Count == 1)
                //{
                //    return genOrderList[0];
                //}
                //else
                //{
                //    throw new Exception("单号生成错误");
                //}
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("生成单号出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取单号前缀
        /// </summary>
        /// <returns></returns>
        private string GetAlphaPrefix(string tableName)
        {
            var alphaPrefix = string.Empty;

            switch (tableName)
            {
                case PublicConst.GenNextNumberPurchase:
                    alphaPrefix = PublicConst.AlphaPrefixPurchase;
                    break;
                case PublicConst.GenNextNumberReceipt:
                    alphaPrefix = PublicConst.AlphaPrefixReceipt;
                    break;
                case PublicConst.GenNextNumberReceiptSn:
                    alphaPrefix = PublicConst.AlphaPrefixReceiptSn;
                    break;
                case PublicConst.GenNextNumberPickDetail:
                    alphaPrefix = PublicConst.AlphaPrefixPickDetail;
                    break;
                case PublicConst.GenNextNumberLot:
                    var lotModel = _crudRepository.FirstOrDefault<nextnumbergen>(x => x.KeyName == PublicConst.GenNextNumberLot);
                    alphaPrefix = lotModel != null ? lotModel.AlphaPrefix : PublicConst.AlphaPrefixLot;
                    break;
                case PublicConst.GenNextNumberBatchLot:
                    var batchModel = _crudRepository.FirstOrDefault<nextnumbergen>(x => x.KeyName == PublicConst.GenNextNumberBatchLot);
                    alphaPrefix = batchModel != null ? batchModel.AlphaPrefix : PublicConst.AlphaPrefixBatchLot;
                    break;
                case PublicConst.GenNextNumberSku:
                    alphaPrefix = PublicConst.AlphaPrefixSku;
                    break;
                case PublicConst.GenNextNumberOutbound:
                    alphaPrefix = PublicConst.AlphaPrefixOutbound;
                    break;
                case PublicConst.GenNextNumberVanning:
                    alphaPrefix = PublicConst.AlphaPrefixVanning;
                    break;
                case PublicConst.GenNextNumberAdjustment:
                    alphaPrefix = PublicConst.AlphaPrefixAdjustment;
                    break;
                case PublicConst.GenNextNumberStockTake:
                    alphaPrefix = PublicConst.AlphaPrefixStockTake;
                    break;
                case PublicConst.GenNextNumberStockTransfer:
                    alphaPrefix = PublicConst.AlphaPrefixStockTransfer;
                    break;
                case PublicConst.GenNextNumberHandoverGroup:
                    alphaPrefix = PublicConst.AlphaPrefixHandoverGroup;
                    break;
                case PublicConst.GenNextNumberStockMovement:
                    alphaPrefix = PublicConst.AlphaPrefixStockMovement;
                    break;
                case PublicConst.GenNextNumberAssembly:
                    alphaPrefix = PublicConst.AlphaPrefixAssembly;
                    break;
                case PublicConst.GenNextNumberTransferInventory:
                    alphaPrefix = PublicConst.AlphaPrefixTransferInventory;
                    break;
                case PublicConst.GenNextNumberPrePack:
                    alphaPrefix = PublicConst.AlphaPrefixPrePack;
                    break;
                case PublicConst.GenNextNumberPreBulkPack:
                    alphaPrefix = PublicConst.AlphaPrefixPreBulkPack;
                    break;
                case PublicConst.GenNextNumberQC:
                    alphaPrefix = PublicConst.AlphaPrefixQC;
                    break;
                case PublicConst.GenNextNumberSkuBorrow:
                    alphaPrefix = PublicConst.AlphaPrefixSkuBorrow;
                    break;
                case PublicConst.GenNextNumberWork:
                    alphaPrefix = PublicConst.AlphaPrefixWorkManger;
                    break;
                case PublicConst.GenNextNumberPicking:
                    alphaPrefix = PublicConst.AlphaPrefixPicking;
                    break;
            }

            return alphaPrefix;
        }
        #endregion

        /// <summary>
        /// 获取经纬度
        /// </summary>
        /// <param name="city">城市名称 可空</param>
        /// <param name="address">详细地址 不可空</param>
        /// <returns></returns>
        public CoordinateDto GetCoordinate(string city, string address)
        {
            #region 获取经纬度
            string apiUrl = PublicConst.GeocoderURL;
            apiUrl += "?address=" + city + address + "&output=json" + "&ak=" + PublicConst.GeocoderAK;
            var result = HttpHelper.CreateGetHttpResponse(apiUrl);
            return result.JsonToDto<CoordinateDto>();
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <returns></returns>
        public CommonResponse PushOutboundECC(string outboundOrder)
        {
            return new CommonResponse();
        }
    }
}
