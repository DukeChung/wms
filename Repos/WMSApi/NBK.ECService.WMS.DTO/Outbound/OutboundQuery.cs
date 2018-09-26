using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundQuery : BaseQuery
    {
        /// <summary>
        /// 业务单据号
        /// </summary>
        public string ExternOrderId { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ConsigneeName { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string ConsigneePhone { get; set; }

        /// <summary>
        /// 收货人地址
        /// </summary>
        public string ConsigneeAddress { get; set; }

        /// <summary>
        /// 创建时间从
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }

        /// <summary>
        /// 创建时间到
        /// </summary>
        public DateTime? CreateDateTo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 审核时间从
        /// </summary>
        public DateTime? AuditingDateFrom { get; set; }

        /// <summary>
        /// 审核时间到
        /// </summary>
        public DateTime? AuditingDateTo { get; set; }

        /// <summary>
        /// 发货时间从
        /// </summary>
        public DateTime? ActualShipDateFrom { get; set; }

        /// <summary>
        /// 发货时间到
        /// </summary>
        public DateTime? ActualShipDateTo { get; set; }

        /// <summary>
        /// 出库单类型
        /// </summary>
        public int? OutboundType { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string CarrierNumber { get; set; }

        /// <summary>
        /// 待拣货出库单查询条件
        /// </summary>
        public bool WaitPickSearch { get; set; } = false;

        public string ToWareHouseSysId { get; set; }
        public string ServiceStationCode { get; set; }

        public string ServiceStationName { get; set; }

        public string Region { get; set; }

        public bool? IsMaterial { get; set; }
        public bool? IsReturn { get; set; }

        public string OutboundChildType { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        public string PurchaseOrder { get; set; }

        /// <summary>
        /// 平台订单号
        /// </summary>
        public string PlatformOrder { get; set; }
        /// <summary>
        /// TMS运单号
        /// </summary>
        public string TMSOrder { get; set; }

        /// <summary>
        /// 装车时间从
        /// </summary>
        public DateTime? DepartureDateFrom { get; set; }

        /// <summary>
        /// 装车时间到
        /// </summary>
        public DateTime? DepartureDateTo { get; set; }


        /// <summary>
        /// 是否为化肥出库单
        /// </summary>
        public bool IsFertilizer { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; } 

        /// <summary>
        /// 是否导出
        /// </summary>
        public bool IsExport { get; set; }
    }
}
