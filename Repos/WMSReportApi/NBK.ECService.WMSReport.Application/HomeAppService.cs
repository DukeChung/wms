using Abp.Application.Services;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Chart;
using NBK.ECService.WMSReport.Model.Models;
using NBK.ECService.WMSReport.Repository.Interface;
using NBK.ECService.WMSReport.Utility.Enum;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using NBK.ECService.WMSReport.DTO.Base;
using System.Configuration;
using NBK.ECService.WMSReport.Utility.Redis;
using Abp.Domain.Uow;

namespace NBK.ECService.WMSReport.Application
{
    public class HomeAppService : ApplicationService, IHomeAppService
    {
        private IHomeRepository _crudRepository = null;
        private IGlobalRepository _globalRepository = null;
        private IBaseAppService _baseAppService = null;

        public HomeAppService(IHomeRepository crudRepository, IGlobalRepository globalRepository, IBaseAppService baseAppService)
        {
            this._crudRepository = crudRepository;
            this._globalRepository = globalRepository;
            this._baseAppService = baseAppService;
        }

        #region 报表首页相关数据查询

        /// <summary>
        /// 获取采购入库/B2B出库/B2C出库数据
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public SparkLineSummaryDto GetSparkLineSummaryDto(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new SparkLineSummaryDto();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetSparkLineSummaryDto(), RedisReportSourceKey.SparkLineSummaryDtoKey);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                     {
                         return _crudRepository.GetSparkLineSummaryDto();
                     }, RedisReportSourceKey.SparkLineSummaryDtoKey);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// 顶部入库单据类型占比
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public ReturnPurchaseTypePieDto GetPurchaseTypePieDto(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new ReturnPurchaseTypePieDto();
            try
            {
                if (flag)
                {
                    var list = _crudRepository.GetPurchaseTypePieDto();
                    if (list != null && list.Count > 0)
                    {
                        result.PurchaseQty = list.Sum(x => x.Qty);
                        foreach (var item in list)
                        {
                            switch (item.Type)
                            {
                                case (int)PurchaseType.Purchase:
                                    result.CGPurchasePie = Math.Floor((item.Qty / (double)result.PurchaseQty) * 100);
                                    break;
                                case (int)PurchaseType.Material:
                                    result.MaterialPurchasePie = Math.Floor((item.Qty / (double)result.PurchaseQty) * 100);
                                    break;
                                case (int)PurchaseType.Fertilizer:
                                    result.FertilizerPurchasePie = Math.Floor((item.Qty / (double)result.PurchaseQty) * 100);
                                    break;
                                case (int)PurchaseType.Return:
                                    result.MovePurchasePie = Math.Floor((item.Qty / (double)result.PurchaseQty) * 100);
                                    break;
                                    //case (int)PurchaseType.TransferInventory:
                                    //    result.ReturnPurchasePie =  (item.Qty / (double)result.PurchaseQty) * 100;
                                    //    break;
                            }

                        }
                        result.ReturnPurchasePie = 100 - (result.CGPurchasePie + result.MaterialPurchasePie + result.FertilizerPurchasePie + result.MovePurchasePie);
                    }
                    RedisWMS.SetRedis(result, RedisReportSourceKey.PurchaseTypePieDto);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        var model = new ReturnPurchaseTypePieDto();
                        var list = _crudRepository.GetPurchaseTypePieDto();
                        if (list != null && list.Count > 0)
                        {
                            model.PurchaseQty = list.Sum(x => x.Qty);
                            foreach (var item in list)
                            {
                                switch (item.Type)
                                {
                                    case (int)PurchaseType.Purchase:
                                        model.CGPurchasePie = Math.Floor((item.Qty / (double)model.PurchaseQty) * 100);
                                        break;
                                    case (int)PurchaseType.Material:
                                        model.MaterialPurchasePie = Math.Floor((item.Qty / (double)model.PurchaseQty) * 100);
                                        break;
                                    case (int)PurchaseType.Fertilizer:
                                        model.FertilizerPurchasePie = Math.Floor((item.Qty / (double)model.PurchaseQty) * 100);
                                        break;
                                    case (int)PurchaseType.Return:
                                        model.MovePurchasePie = Math.Floor((item.Qty / (double)model.PurchaseQty) * 100);
                                        break;
                                        //case (int)PurchaseType.TransferInventory:
                                        //    model.ReturnPurchasePie = (item.Qty / (double)model.PurchaseQty) * 100;
                                        //    break;
                                }

                            }

                            model.ReturnPurchasePie = 100 - (model.CGPurchasePie + model.MaterialPurchasePie + model.FertilizerPurchasePie + model.MovePurchasePie);
                        }
                        return model;
                    }, RedisReportSourceKey.PurchaseTypePieDto);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// 顶部出库单据类型占比
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public ReturnOutboundTypePieDto GetOutboundTypePieDto(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new ReturnOutboundTypePieDto();
            try
            {
                if (flag)
                {
                    var list = _crudRepository.GetOutboundTypePieDto();
                    if (list != null && list.Count > 0)
                    {
                        result.OutboundQty = list.Sum(x => x.Qty);
                        foreach (var item in list)
                        {
                            switch (item.OutboundType)
                            {
                                case (int)OutboundType.B2C:
                                    result.B2COutboundPie = Math.Floor((item.Qty / (double)result.OutboundQty) * 100);
                                    break;
                                case (int)OutboundType.B2B:
                                    result.B2BOutboundPie = Math.Floor((item.Qty / (double)result.OutboundQty) * 100);
                                    break;
                                case (int)OutboundType.Return:
                                    result.ReturnOutboundPie = Math.Floor((item.Qty / (double)result.OutboundQty) * 100);
                                    break;
                                case (int)OutboundType.TransferInventory:
                                    result.MoveOutboundPie = Math.Floor((item.Qty / (double)result.OutboundQty) * 100);
                                    break;
                                    //case (int)OutboundType.Fertilizer:
                                    //    result.FertilizerOutboundPie = (item.Qty / (double)result.OutboundQty) * 100;
                                    //    break;
                            }
                        }

                        result.FertilizerOutboundPie = 100 - (result.B2COutboundPie + result.B2BOutboundPie + result.ReturnOutboundPie + result.MoveOutboundPie);
                    }
                    RedisWMS.SetRedis(result, RedisReportSourceKey.OutboundTypePieDto);
                }

                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        var model = new ReturnOutboundTypePieDto();
                        var list = _crudRepository.GetOutboundTypePieDto();
                        if (list != null && list.Count > 0)
                        {
                            model.OutboundQty = list.Sum(x => x.Qty);
                            foreach (var item in list)
                            {
                                switch (item.OutboundType)
                                {
                                    case (int)OutboundType.B2C:
                                        model.B2COutboundPie = Math.Floor((item.Qty / (double)model.OutboundQty) * 100);
                                        break;
                                    case (int)OutboundType.B2B:
                                        model.B2BOutboundPie = Math.Floor((item.Qty / (double)model.OutboundQty) * 100);
                                        break;
                                    case (int)OutboundType.Return:
                                        model.ReturnOutboundPie = Math.Floor((item.Qty / (double)model.OutboundQty) * 100);
                                        break;
                                    case (int)OutboundType.TransferInventory:
                                        model.MoveOutboundPie = Math.Floor((item.Qty / (double)model.OutboundQty) * 100);
                                        break;
                                        //case (int)OutboundType.Fertilizer:
                                        //    model.FertilizerOutboundPie = (item.Qty / (double)model.OutboundQty) * 100;
                                        //    break;
                                }
                            }
                            result = model;
                            model.FertilizerOutboundPie = 100 - (model.B2COutboundPie + model.B2BOutboundPie + model.ReturnOutboundPie + model.MoveOutboundPie);
                        }
                        return model;
                    }, RedisReportSourceKey.OutboundTypePieDto);

                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// 全局一年之内收发存
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<StockInOutListDto> GetStockInOutData(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<StockInOutListDto>();
            try
            {
                if (flag)
                {
                    var startTime = DateTime.Now.AddYears(-1).AddMonths(1).ToString("yyyy-MM-01 00:00:00");
                    var endTime = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    var startQty = 0m;
                    var list = _crudRepository.GetStockInOutData(startTime, endTime, ref startQty);
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            item.InitialQty = startQty + (item.ReceiptQty + item.OutboundQty);
                            startQty = item.InitialQty;
                        }
                    }
                    RedisWMS.SetRedis(list, RedisReportSourceKey.StockInOutData);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        var startTime = DateTime.Now.AddYears(-1).AddMonths(1).ToString("yyyy-MM-01 00:00:00");
                        var endTime = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                        var startQty = 0m;
                        var list = _crudRepository.GetStockInOutData(startTime, endTime, ref startQty);
                        if (list != null && list.Count > 0)
                        {
                            foreach (var item in list)
                            {
                                item.InitialQty = startQty + (item.ReceiptQty + item.OutboundQty);
                                startQty = item.InitialQty;
                            }
                        }
                        return list;
                    }, RedisReportSourceKey.StockInOutData);

                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取所有仓库总收货分布
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<WareHouseQtyDto> GetWareHouseReceiptQtyList(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<WareHouseQtyDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetWareHouseReceiptQtyList(), RedisReportSourceKey.WareHouseReceiptQtyList);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _crudRepository.GetWareHouseReceiptQtyList();
                    }, RedisReportSourceKey.WareHouseReceiptQtyList);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取所有仓库总出库分布
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<WareHouseQtyDto> GetWareHouseOutboundQtyList(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<WareHouseQtyDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetWareHouseOutboundQtyList(), RedisReportSourceKey.WareHouseOutboundQtyList);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _crudRepository.GetWareHouseOutboundQtyList();
                    }, RedisReportSourceKey.WareHouseOutboundQtyList);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        //// <summary>
        /// 获取所有仓库总库存
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<WareHouseQtyDto> GetWareHouseQtyList(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<WareHouseQtyDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetWareHouseQtyList(), RedisReportSourceKey.WareHouseQtyList);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _crudRepository.GetWareHouseQtyList();
                    }, RedisReportSourceKey.WareHouseQtyList);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 仓库出库，库存库龄占比
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public StockAgeGroupDto GetStockAgeGroup(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new StockAgeGroupDto();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetStockAgeGroup(), RedisReportSourceKey.StockAgeGroup);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _crudRepository.GetStockAgeGroup();
                    }, RedisReportSourceKey.StockAgeGroup);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取畅销商品Top10
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<SkuSaleQtyDto> GetSkuSellingTop10(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<SkuSaleQtyDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetSkuSellingTop10(), RedisReportSourceKey.SkuSellingTop10);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _crudRepository.GetSkuSellingTop10();
                    }, RedisReportSourceKey.SkuSellingTop10);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取滞销商品Top10
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<SkuSaleQtyDto> GetSkuUnsalableTop10(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<SkuSaleQtyDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetSkuUnsalableTop10(), RedisReportSourceKey.SkuUnsalableTop10);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _crudRepository.GetSkuUnsalableTop10();
                    }, RedisReportSourceKey.SkuUnsalableTop10);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取服务站发货Top10
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<ServiceStationOutboundDto> GetServiceStationOutboundTopTen(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<ServiceStationOutboundDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetServiceStationOutboundTopTen(), RedisReportSourceKey.ServiceStationOutboundTopTen);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _crudRepository.GetServiceStationOutboundTopTen();
                    }, RedisReportSourceKey.ServiceStationOutboundTopTen);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取渠道库存
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<ChannelQtyDto> GetChannelPieData(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<ChannelQtyDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetChannelPieData(), RedisReportSourceKey.ChannelPieData);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _crudRepository.GetChannelPieData();
                    }, RedisReportSourceKey.ChannelPieData);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// /获取最新10个退货入库收货完成的单子
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<ReturnPurchaseDto> GetReturnPurchase(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<ReturnPurchaseDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetReturnPurchase(), RedisReportSourceKey.ReturnPurchase);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _crudRepository.GetReturnPurchase();
                    }, RedisReportSourceKey.ReturnPurchase);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 仓库日历订单统计
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public EventDataCountReportDto GetDailyEventDataCountInfo(bool flag, string startDate, string endDate)
        {
            _crudRepository.ChangeGlobalDB();
            var response = new EventDataCountReportDto();
            try
            {
                var list = _crudRepository.GetCalendarData(startDate, endDate);
                response = FormatData(list, startDate, endDate);
            }
            catch (Exception)
            {
            }
            return response;
        }

        private EventDataCountReportDto FormatData(CalendarOrdersOutInData list, string startDate, string endDate)
        {
            var result = new EventDataCountReportDto();
            var currentStartTime = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);
            int endDay = end.Day; // currentStartTime.AddMonths(1).AddDays(-1).Day;
            if (end.Month == DateTime.Now.Month)
            {
                endDay = DateTime.Now.Day;
            }
            int year, month, day;
            for (int i = 0; i < endDay; i++)
            {
                //循环遍历当前月的每一天，做数据信息统计
                var startTime = currentStartTime.AddDays(i);
                var endTime = startTime.AddDays(1);
                year = startTime.Year;
                month = startTime.Month;
                day = startTime.Day;

                #region 出库数据
                var dailyOutbound = list.OutboundList.Where(p => p.Date >= startTime && p.Date < endTime);
                result.OutboundList.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "出库",
                    Count = dailyOutbound.Count()
                });

                var newOutbound = list.OutboundList.Where(p => p.Date >= startTime && p.Date < endTime && p.Status == (int)OutboundStatus.New);
                result.OutboundList1.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "新建",
                    Count = newOutbound.Count()
                });

                var allocationOutbound = list.OutboundList.Where(p => p.Date >= startTime && p.Date < endTime && p.Status == (int)OutboundStatus.Allocation);
                result.OutboundList2.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "分配完成",
                    Count = allocationOutbound.Count()
                });

                var pickingOutbound = list.OutboundList.Where(p => p.Date >= startTime && p.Date < endTime && p.Status == (int)OutboundStatus.Picking);
                result.OutboundList3.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "拣货完成",
                    Count = pickingOutbound.Count()
                });

                var deliveryOutbound = list.OutboundList.Where(p => p.Date >= startTime && p.Date < endTime && p.Status == (int)OutboundStatus.Delivery);
                result.OutboundList4.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "已出库",
                    Count = deliveryOutbound.Count()
                });

                var cancelOutbound = list.OutboundList.Where(p => p.Date >= startTime && p.Date < endTime && (p.Status == (int)OutboundStatus.Cancel || p.Status == (int)OutboundStatus.Close));
                result.OutboundList5.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "关闭",
                    Count = cancelOutbound.Count()
                });
                #endregion

                #region 入库数据
                var dailyInbound = list.PurchaseList.Where(p => p.Date >= startTime && p.Date < endTime);
                result.InboundList.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "入库",
                    Count = dailyInbound.Count()
                });

                var newInbound = list.PurchaseList.Where(p => p.Date >= startTime && p.Date < endTime && p.Status == (int)PurchaseStatus.New);
                result.InboundList1.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "新建",
                    Count = newInbound.Count()
                });

                var inReceiptInbound = list.PurchaseList.Where(p => p.Date >= startTime && p.Date < endTime && p.Status == (int)PurchaseStatus.InReceipt);
                result.InboundList2.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "收货中",
                    Count = inReceiptInbound.Count()
                });

                var partReceiptInbound = list.PurchaseList.Where(p => p.Date >= startTime && p.Date < endTime && p.Status == (int)PurchaseStatus.PartReceipt);
                result.InboundList3.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "部分入库",
                    Count = partReceiptInbound.Count()
                });

                var finishInbound = list.PurchaseList.Where(p => p.Date >= startTime && p.Date < endTime && p.Status == (int)PurchaseStatus.Finish);
                result.InboundList4.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "已入库",
                    Count = finishInbound.Count()
                });

                var cancelInbound = list.PurchaseList.Where(p => p.Date >= startTime && p.Date < endTime && (p.Status == (int)PurchaseStatus.Close || p.Status == (int)PurchaseStatus.Void));
                result.InboundList5.Add(new DailyEventDataCountReportDto()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    BussinessName = "关闭",
                    Count = cancelInbound.Count()
                });

                #endregion
            }
            return result;

        }

        /// <summary>
        /// 获取仓库某一天出库入库数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<CalendarDataByDateDto> GetCalendarDataByDate(string date)
        {
            _crudRepository.ChangeGlobalDB();
            var response = new List<CalendarDataByDateDto>();
            try
            {
                var list = _crudRepository.GetCalendarDataByDate(date);
                response = FormartDateData(list);
            }
            catch (Exception)
            {

            }
            return response;
        }

        private List<CalendarDataByDateDto> FormartDateData(CalendarDataByDateOutboundOrPurchase dto)
        {
            var result = new List<CalendarDataByDateDto>();
            result = dto.OutboundList;
            foreach (var item in result)
            {
                var info = dto.PurchaseList.Where(x => x.Name == item.Name).First();
                if (info != null)
                {
                    item.PurchaseQty = info.PurchaseQty;
                    item.NewPurchaseQty = info.NewPurchaseQty;
                    item.InReceiptQty = info.InReceiptQty;
                    item.PartReceiptQty = info.PartReceiptQty;
                    item.FinishQty = info.FinishQty;
                    item.CancelPurchaseQty = info.CancelPurchaseQty;
                }
            }
            return result;
        }
        #endregion

        #region 定时批处理任务
        [UnitOfWork(isTransactional: false)]
        public int GetOutboundNoLngLatCount()
        {
            _crudRepository.ChangeGlobalDB();
            var result = 0;
            try
            {
                result = _crudRepository.GetOutboundNoLngLatCount();
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// 更新出库单经纬度
        /// </summary>
        /// <param name="pageCount">页面大小</param>
        /// <returns></returns>
        public bool UpdataOutboundLngLat(int pageCount)
        {
            _crudRepository.ChangeGlobalDB();
            var result = false;
            try
            {
                var list = _crudRepository.GetNeedToDoLngLatData(0, pageCount);
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var address = item.FullAddress;
                        var coordinate = _baseAppService.GetCoordinate("", address);
                        if (coordinate != null && coordinate.Status == 0 && coordinate.Result != null && coordinate.Result.location != null)
                        {
                            item.lat = coordinate.Result.location.lat;
                            item.lng = coordinate.Result.location.lng;
                        }
                    }
                    _crudRepository.UpdateOutboundLngLat(list);
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception("更新出库单经纬度报错" + ex.Message);
            }
            return result;
        }
        #endregion

        #region 地图相关业务
        /// <summary>
        /// 获取仓库发货覆盖
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<OutboundMapData> GetHistoryServiceStationData(int page)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<OutboundMapData>();
            try
            {
                result = _crudRepository.GetHistoryServiceStationData(page);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取仓库城市出库关系
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<WarehouseStationRelationDto> GetWarehouseStationRelation()
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<WarehouseStationRelationDto>();
            try
            {
                result = _crudRepository.GetWarehouseStationRelation();
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        #endregion

        #region 作业分布时间相关业务
        /// <summary>
        /// 获取仓库作业时间分布数据
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<WorkDistributionListData> GetWorkDistributionData(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<WorkDistributionListData>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(GetWorkDistributionData(), RedisReportSourceKey.WorkDistributionData);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return GetWorkDistributionData();
                    }, RedisReportSourceKey.WorkDistributionData);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 组织数据
        /// </summary>
        /// <returns></returns>
        private List<WorkDistributionListData> GetWorkDistributionData()
        {
            var result = new List<WorkDistributionListData>();
            var warehouseList = _globalRepository.GetAllWarehouse();
            var list = _crudRepository.GetWorkDistributionData();
            for (int i = 0; i < warehouseList.Count; i++)
            {
                var info = new WorkDistributionListData();
                info.SysId = warehouseList[i].SysId;
                info.Name = warehouseList[i].Name;
                var hoursData = list.Where(x => x.WarehouseSysId == warehouseList[i].SysId).ToList();
                if (hoursData != null && hoursData.Count > 0)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        var times = hoursData.Where(x => x.Hours == j).Sum(p => p.Times);
                        if (times > 0)
                        {
                            info.WorkDistributionDataDto.Add(new WorkDistributionDataDto()
                            {
                                Hours = j,
                                WarehouseSysId = warehouseList[i].SysId,
                                Times = times
                            });
                        }
                    }
                }
                result.Add(info);
            }
            return result;
        }

        /// <summary>
        /// 根据仓库获业务类型获取作业时间分布
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<WorkDistributionDataByWarehouseDto> GetWorkDistributionByWarehouse(bool flag, Guid sysId)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<WorkDistributionDataByWarehouseDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_crudRepository.GetWorkDistributionByWarehouse(sysId), string.Format(RedisReportSourceKey.WorkDistributionByWarehouse, sysId));
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _crudRepository.GetWorkDistributionByWarehouse(sysId);
                    }, string.Format(RedisReportSourceKey.WorkDistributionByWarehouse, sysId));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 仓库组作业类型占比数据
        /// </summary>
        /// <param name="flag">如果为True，更新缓存内容</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<WorkDistributionPieDto> GetWorkDistributionPieData(bool flag)
        {
            _crudRepository.ChangeGlobalDB();
            var result = new List<WorkDistributionPieDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(GetWorkDistributionPieData(), RedisReportSourceKey.WorkDistributionPie);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return GetWorkDistributionPieData();
                    }, RedisReportSourceKey.WorkDistributionPie);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 组织仓库组作业类型占比数据
        /// </summary>
        /// <returns></returns>
        private List<WorkDistributionPieDto> GetWorkDistributionPieData()
        {
            var result = new List<WorkDistributionPieDto>();
            var warehouseList = _globalRepository.GetAllWarehouse();
            var list = _crudRepository.GetWorkDistributionPieData();
            for (int i = 0; i < warehouseList.Count; i++)
            {
                var info = new WorkDistributionPieDto();
                info.SysId = warehouseList[i].SysId;
                info.Name = warehouseList[i].Name;
                var hoursData = list.Where(x => x.WarehouseSysId == warehouseList[i].SysId).ToList();
                if (hoursData != null && hoursData.Count > 0)
                {
                    for (int j = 0; j < hoursData.Count; j++)
                    {
                        hoursData[j].TypeName = info.Name + hoursData[j].TypeName;
                        info.WorkPieData.Add(hoursData[j]);
                    }
                }
                info.AllTims = info.WorkPieData.Sum(x => x.Times);
                result.Add(info);
            }
            return result;
        }
        #endregion
    }
}
