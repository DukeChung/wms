using System;
using Common.Logging;
using Quartz;
using System.Net.Mail;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBK.WMSJob
{
    /// <summary>
    /// 更新报表缓存
    /// </summary>
    public class UpdateReportRedisJob : IJob
    {

        public UpdateReportRedisJob()
        {
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogManager.GetLogger(typeof(UpdateReportRedisJob)).InfoFormat("{0}/更新报表缓存开始", DateTime.Now);

                // WMSApiClient.GetInstance().GetSparkLineSummaryDto();(注：此方法现在弃用)
                WMSApiClient.GetInstance().GetPurchaseTypePieDto();
                WMSApiClient.GetInstance().GetOutboundTypePieDto();
                WMSApiClient.GetInstance().GetStockInOutData();


                WMSApiClient.GetInstance().GetWareHouseReceiptQtyList();
                WMSApiClient.GetInstance().GetWareHouseOutboundQtyList();
                WMSApiClient.GetInstance().GetWareHouseQtyList();


                WMSApiClient.GetInstance().GetStockAgeGroup();
                WMSApiClient.GetInstance().GetSkuSellingTop10();
                WMSApiClient.GetInstance().GetSkuUnsalableTop10();


                WMSApiClient.GetInstance().GetChannelPieData();
                WMSApiClient.GetInstance().GetServiceStationOutboundTop10();
                WMSApiClient.GetInstance().GetReturnPurchase();


                WMSApiClient.GetInstance().GetFertilizerRORadarList();
                WMSApiClient.GetInstance().GetFertilizerInvRadarList();
                WMSApiClient.GetInstance().GetFertilizerInvPieList();

                WMSApiClient.GetInstance().GetAccessBizList();
                WMSApiClient.GetInstance().GetWorkDistributionData();
                WMSApiClient.GetInstance().GetWorkDistributionPieData();
                WMSApiClient.GetInstance().GetWorkDistributionByWarehouse();

                LogManager.GetLogger(typeof(UpdateReportRedisJob)).InfoFormat("{0}/更新报表缓存结束", DateTime.Now);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(UpdateReportRedisJob)).InfoFormat("{0} 更新报表缓存报错：{1}", DateTime.Now, ex.Message);
            }
        }
    }
}
