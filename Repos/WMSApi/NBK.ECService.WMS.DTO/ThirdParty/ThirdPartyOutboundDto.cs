using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{

    public class ThirdPartyOutboundDto
    {
        /// <summary>
        /// 仓库代码, StockSysNo
        /// </summary>
        public string WareHouseSysId { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }

        /// <summary>
        /// 要求发货日期, DeliveryDate
        /// </summary>
        public DateTime? RequestedShipDate { get; set; }

        /// <summary>
        /// 实际发货日期
        /// </summary>
        public DateTime? ActualShipDate { get; set; }

        /// <summary>
        /// 到货日期
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// 审计时间, AuditingDate
        /// </summary>
        public DateTime? AuditingDate { get; set; }

        /// <summary>
        /// 审计人, AuditingBy
        /// </summary>
        public string AuditingBy { get; set; }

        /// <summary>
        /// 审计人姓名, AuditingName
        /// </summary>
        public string AuditingName { get; set; }

        /// <summary>
        /// 外部订单ID, ShoppingSysNo
        /// </summary>
        public string ExternOrderId { get; set; }

        /// <summary>
        /// 外部订单日期, OrderDate
        /// </summary>
        public DateTime? ExternOrderDate { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ConsigneeName { get; set; }

        /// <summary>
        /// 收货人地址
        /// </summary>
        public string ConsigneeAddress { get; set; }

        /// <summary>
        /// 收货人省份
        /// </summary>
        public string ConsigneeProvince { get; set; }

        /// <summary>
        /// 收货人城市
        /// </summary>
        public string ConsigneeCity { get; set; }

        /// <summary>
        /// 收货人区
        /// </summary>
        public string ConsigneeArea { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string ConsigneePhone { get; set; }

        /// <summary>
        /// 收货人手机
        /// </summary>
        public string ConsigneeCellPhone { get; set; }

        /// <summary>
        /// 收货人乡
        /// </summary>
        public string ConsigneeTown { get; set; }

        /// <summary>
        /// 收货人村
        /// </summary>
        public string ConsigneeVillage { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// 货到付款
        /// </summary>
        public int? CashOnDelivery { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        public string ShippingMethod { get; set; }

        /// <summary>
        /// 总订单数量
        /// </summary>
        public int? TotalQty { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        public int? InvoiceType { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal? Freight { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal? TotalPrice { get; set; }

        /// <summary>
        /// 订单折扣
        /// </summary>
        public decimal? DiscountPrice { get; set; }

        /// <summary>
        /// 平台订单号
        /// </summary>
        public string PlatformOrder { get; set; }
        /// <summary>
        /// 出库单类型
        /// 订单来源：
        /// app =》B2C = 20
        /// 农商=》B2B = 30
        /// 原材料  = 70
        /// </summary>
        public int OutboundType { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string OutboundChildType { get; set; }

        public string Source { get; set; }

        /// <summary>
        /// 服务站名称
        /// </summary>
        public string ServiceStationName { get; set; }

        /// <summary>
        /// 服务站编码
        /// </summary>
        public string ServiceStationCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNumber { get; set; }

        public string ExternPurchaseOrder { get; set; }

        /// <summary>
        /// 是否开票
        /// </summary>
        public bool HasInvoice { get; set; }
        /// <summary>
        /// 优惠券金额
        /// </summary>
        public decimal CouponAmount { get; set; }


        /// <summary>
        /// 订单明细
        /// </summary>
        public List<ThirdPartyOutboundDetailDto> OutboundDetailDtoList { get; set; }
    }
}
