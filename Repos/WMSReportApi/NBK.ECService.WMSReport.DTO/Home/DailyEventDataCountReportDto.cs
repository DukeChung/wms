using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{

    public class CalendarOrdersOutInData
    {
        public List<CalendarOrdersData> OutboundList { get; set; } = new List<CalendarOrdersData>();
        public List<CalendarOrdersData> PurchaseList { get; set; } = new List<CalendarOrdersData>();
    }

    /// <summary>
    /// 出库入库单数据
    /// </summary>
    public class CalendarOrdersData
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Date { get; set; }
    }


    public class DailyEventDataCountReportDto
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        public string BussinessName { get; set; } = "";

        public int Count { get; set; } = 0;

        public string BussinessNameDisplay
        {
            get
            {
                return $"{BussinessName}:{Count} 单";
            }
        }
    }

    /// <summary>
    /// 仓库日历订单
    /// </summary>
    public class EventDataCountReportDto
    {
        public List<DailyEventDataCountReportDto> OutboundList { get; set; } = new List<DailyEventDataCountReportDto>();
        public List<DailyEventDataCountReportDto> OutboundList1 { get; set; } = new List<DailyEventDataCountReportDto>();
        public List<DailyEventDataCountReportDto> OutboundList2 { get; set; } = new List<DailyEventDataCountReportDto>();
        public List<DailyEventDataCountReportDto> OutboundList3 { get; set; } = new List<DailyEventDataCountReportDto>();
        public List<DailyEventDataCountReportDto> OutboundList4 { get; set; } = new List<DailyEventDataCountReportDto>();
        public List<DailyEventDataCountReportDto> OutboundList5 { get; set; } = new List<DailyEventDataCountReportDto>();

        public List<DailyEventDataCountReportDto> InboundList { get; set; } = new List<DailyEventDataCountReportDto>();
        public List<DailyEventDataCountReportDto> InboundList1 { get; set; } = new List<DailyEventDataCountReportDto>();
        public List<DailyEventDataCountReportDto> InboundList2 { get; set; } = new List<DailyEventDataCountReportDto>();
        public List<DailyEventDataCountReportDto> InboundList3 { get; set; } = new List<DailyEventDataCountReportDto>();
        public List<DailyEventDataCountReportDto> InboundList4 { get; set; } = new List<DailyEventDataCountReportDto>();
        public List<DailyEventDataCountReportDto> InboundList5 { get; set; } = new List<DailyEventDataCountReportDto>();
    }


    public class CalendarDataByDateDto
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 出库数量
        /// </summary>
        public int OutboundQty { get; set; }
        public int NewOutboundQty { get; set; }
        public int AllocationQty { get; set; }
        public int PickingQty { get; set; }
        public int DeliveryQty { get; set; }
        public int CancelOutboundQty { get; set; }


        /// <summary>
        /// 入库数量
        /// </summary>
        public int PurchaseQty { get; set; }
        public int NewPurchaseQty { get; set; }
        public int InReceiptQty { get; set; }
        public int PartReceiptQty { get; set; }
        public int FinishQty { get; set; }
        public int CancelPurchaseQty { get; set; }
    }

    public class CalendarDataByDateOutboundOrPurchase
    {
        public List<CalendarDataByDateDto> OutboundList { get; set; } = new List<CalendarDataByDateDto>();
        public List<CalendarDataByDateDto> PurchaseList { get; set; } = new List<CalendarDataByDateDto>();
    }
}
