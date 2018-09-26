using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO.Chart;
using NBK.ECService.WMSReport.Model.Models;
using NBK.ECService.WMSReport.Repository.Interface;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using NBK.ECService.WMSReport.Utility.Redis;

namespace NBK.ECService.WMSReport.Application
{
    public class ChartAppService : ApplicationService, IChartAppService
    {
        private IChartRepository _crudRepository = null;

        public ChartAppService(IChartRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        public PurchaseAndOutboundChartDto GetPurchaseAndOutboundChart(Guid wareHouseSysId)
        {
            return _crudRepository.GetPurchaseAndOutboundChart(wareHouseSysId);
        }

        public List<AdventSkuChartDto> GetAdventSkuChartTop5(Guid wareHouseSysId)
        {
            _crudRepository.ChangeDB(wareHouseSysId);
            var sysCodes = _crudRepository.GetQuery<syscode>(x => x.SysCodeType == PublicConst.SysCodeTypeShelfLifeWarning, wareHouseSysId).FirstOrDefault();
            var detail = sysCodes.syscodedetails.FirstOrDefault();
            var day = int.Parse(detail.Code);
            var warningDate = DateTime.Now.AddDays(day);
            return _crudRepository.GetAdventSkuChartDto(warningDate, wareHouseSysId);
        }

        /// <summary>
        /// 近10日的收货发货数量
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public List<SkuReceiptOutboundReportDto> GetSkuReceiptOutboundChartAfter10(Guid wareHouseSysId)
        {
            var ts = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
            return RedisWMS.CacheResult(() =>
            {
                var list = new List<SkuReceiptOutboundReportDto>();
                int days = 10;
                DateTime begin = DateTime.Now.AddDays(-days).Date;
                DateTime end = DateTime.Now.Date;

                var receiptList = _crudRepository.GetSkuReceiptChart(begin, end, wareHouseSysId);
                var outboundList = _crudRepository.GetSkuOutboundChart(begin, end, wareHouseSysId);

                for (int i = days; i >= 1; i--)
                {
                    DateTime startDate = DateTime.Now.AddDays(-i).Date;
                    DateTime endDate = DateTime.Now.AddDays(-i + 1).Date;

                    list.Add(new SkuReceiptOutboundReportDto()
                    {
                        AfterDay = DateTime.Now.AddDays(-i).Date.ToShortDateString(),
                        ReceiptCount = receiptList.Where(p => p.ReceiptDate > startDate && p.ReceiptDate < endDate).Sum(p => p.ReceivedQty),
                        OutboundCount = outboundList.Where(p => p.OutboundDate > startDate && p.OutboundDate < endDate).Sum(p => p.OutboundQty)
                    });
                }
                return list;
            }, string.Format(RedisSourceKey.ReceiptOutboundChart, wareHouseSysId), ts);
        }

        /// <summary>
        /// 获取过去十天订单与退货数量
        /// </summary>
        /// <returns></returns>
        public List<OutboundAndReturnCharDto> GetOutboundAndReturnCharDataOfLastTenDays(Guid wareHouseSysId)
        {

            var ts = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
            return RedisWMS.CacheResult(() =>
            {
                return _crudRepository.GetOutboundAndReturnCharDataOfLastTenDays(wareHouseSysId);
            }, string.Format(RedisSourceKey.OutboundAndReturnCharData, wareHouseSysId), ts);

        }

        public List<PrePackBoardDto> GetPrePackBoardTop12(Guid wareHouseSysId)
        {
            var result = _crudRepository.GetPrePackBoardTop12(wareHouseSysId);
            return result;
        }

        /// <summary>
        /// 超过三天未收货的入库单
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public List<ExceedThreeDaysPurchase> GetExceedThreeDaysPurchase(Guid wareHouseSysId)
        {
            return _crudRepository.GetExceedThreeDaysPurchase(wareHouseSysId);
        }

        /// <summary>
        /// 清楚首页报表缓存
        /// </summary>
        public void CleanReceiptOutboundRedis(Guid wareHouseSysId)
        {
            RedisWMS.CleanRedis<string>(string.Format(RedisSourceKey.ReceiptOutboundChart, wareHouseSysId));
            RedisWMS.CleanRedis<string>(string.Format(RedisSourceKey.OutboundAndReturnCharData, wareHouseSysId));
        }

        /// <summary>
        /// 获取订单数量
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public OutboundTotalChartDto GetToDayOrderStatusTotal(Guid wareHouseSysId)
        {
            _crudRepository.ChangeDB(wareHouseSysId);
            var outboundChartData = new OutboundTotalChartDto();
            var strStartTime = DateTime.Now.ToString(PublicConst.StartDateFormat);
            var strEndTime = DateTime.Now.ToString(PublicConst.EndDateFormat);
            var startTime = Convert.ToDateTime(strStartTime);
            var endTime = Convert.ToDateTime(strEndTime);
            var po = _crudRepository.GetQuery<purchase>(x => x.CreateDate > startTime && x.CreateDate <= endTime && x.WarehouseSysId == wareHouseSysId).Count();
            var ot = _crudRepository.GetToDayOrderStatusTotal(wareHouseSysId, strStartTime, strEndTime);
            outboundChartData.PurchaseTotal = po;
            outboundChartData.OutboundTotal = ot.Count();
            outboundChartData.B2CMultiTotal = ot.Count(x => x.OutboundType == (int)OutboundType.B2C && x.OutboundSkuCount > 1);
            outboundChartData.B2CMultiNewTotal = ot.Count(x => x.Status == (int)OutboundStatus.New && x.OutboundSkuCount > 1 && x.OutboundType == (int)OutboundType.B2C);
            outboundChartData.B2CMultiPickTotal = ot.Count(x => x.Status == (int)OutboundStatus.Allocation && x.OutboundSkuCount > 1 && x.OutboundType == (int)OutboundType.B2C);
            outboundChartData.B2CMultiDeliverTotal = ot.Count(x => x.Status == (int)OutboundStatus.Picking && x.OutboundSkuCount > 1 && x.OutboundType == (int)OutboundType.B2C);
            outboundChartData.B2CMultiFinishTotal = ot.Count(x => x.Status == (int)OutboundStatus.Delivery && x.OutboundSkuCount > 1 && x.OutboundType == (int)OutboundType.B2C);

            outboundChartData.B2COnlyTotal = ot.Count(x => x.OutboundSkuCount == 1 && x.OutboundType == (int)OutboundType.B2C);
            outboundChartData.B2COnlyNewTotal = ot.Count(x => x.Status == (int)OutboundStatus.New && x.OutboundType == (int)OutboundType.B2C && x.OutboundSkuCount == 1);
            outboundChartData.B2COnlyPickTotal = ot.Count(x => x.Status == (int)OutboundStatus.Allocation && x.OutboundType == (int)OutboundType.B2C && x.OutboundSkuCount == 1);
            outboundChartData.B2COnlyDeliverTotal = ot.Count(x => x.Status == (int)OutboundStatus.Picking && x.OutboundType == (int)OutboundType.B2C && x.OutboundSkuCount == 1);
            outboundChartData.B2COnlyFinishTotal = ot.Count(x => x.Status == (int)OutboundStatus.Delivery && x.OutboundType == (int)OutboundType.B2C && x.OutboundSkuCount == 1);
            outboundChartData.B2BTotal = ot.Count(x => x.OutboundType == (int)OutboundType.B2B);
            outboundChartData.B2BNewTotal = ot.Count(x => x.Status == (int)OutboundStatus.New && x.OutboundType == (int)OutboundType.B2B);
            outboundChartData.B2BPickTotal = ot.Count(x => x.Status == (int)OutboundStatus.Allocation && x.OutboundType == (int)OutboundType.B2B);
            outboundChartData.B2BDeliverTotal = ot.Count(x => x.Status == (int)OutboundStatus.Picking && x.OutboundType == (int)OutboundType.B2B);
            outboundChartData.B2BFinishTotal = ot.Count(x => x.Status == (int)OutboundStatus.Delivery && x.OutboundType == (int)OutboundType.B2B);
            return outboundChartData;
        }
    }
}