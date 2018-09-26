using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class MonitorReportDto
    {
        public List<PurchaseMonitorDto> purchaseMonitorDtoList { get; set; }

        public List<OutboundMonitorDto> outboundMonitorDtoList { get; set; }

        public List<WorkMonitorDto> workMonitorDtoList { get; set; }
    }

    /// <summary>
    /// 入库监控
    /// </summary>
    public class PurchaseMonitorDto
    {
        /// <summary>
        /// 入库单号
        /// </summary>
        public string PurchaseOrder { get; set; }

        /// <summary>
        /// 入库单状态
        /// </summary>
        public int PurchaseStatus { get; set; }

        /// <summary>
        /// 入库单状态名称
        /// </summary>
        public string PurchaseStatusName
        {
            get
            {
                return ((PurchaseStatus)PurchaseStatus).ToDescription();
            }
        }

        /// <summary>
        /// 入库单创建时间
        /// </summary>
        public DateTime PurchaseCreateTime { get; set; }

        /// <summary>
        /// 入库单操作人
        /// </summary>
        public string PurchaseEditUserName { get; set; }

        /// <summary>
        /// 入库单创建信息显示
        /// </summary>
        public string PurchaseCreateDisplay
        {
            get
            {
                return PurchaseCreateTime.ToString(PublicConst.DateTimeFormat) + (string.IsNullOrEmpty(PurchaseEditUserName) ? string.Empty : "[" + PurchaseEditUserName + "]");
            }
        }

        /// <summary>
        /// 收货单号
        /// </summary>
        public string ReceiptOrder { get; set; }

        /// <summary>
        /// 收货单状态
        /// </summary>
        public int? ReceiptStatus { get; set; }

        /// <summary>
        /// 收货单状态名称
        /// </summary>
        public string ReceiptStatusName
        {
            get
            {
                if (ReceiptStatus != null)
                {
                    return ((ReceiptStatus)ReceiptStatus).ToDescription();
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        /// <summary>
        /// 收货单创建时间
        /// </summary>
        public DateTime? ReceiptCreateTime { get; set; }

        /// <summary>
        /// 收货单操作人
        /// </summary>
        public string ReceiptEditUserName { get; set; }

        /// <summary>
        /// 收货单创建信息显示
        /// </summary>
        public string ReceiptCreateDisplay
        {
            get
            {
                if (ReceiptCreateTime == null)
                {
                    return string.Empty;
                }
                else
                {
                    return Convert.ToDateTime(ReceiptCreateTime).ToString(PublicConst.DateTimeFormat) + (string.IsNullOrEmpty(ReceiptEditUserName) ? string.Empty : "[" + ReceiptEditUserName + "]");
                }
            }
        }

        /// <summary>
        /// 入库单编辑时间
        /// </summary>
        public DateTime? PurchaseEditTime { get; set; }


        /// <summary>
        /// 停留时间
        /// </summary>
        public string StayTime
        {
            get
            { 
                TimeSpan dtSpan = DateTime.Now - Convert.ToDateTime(PurchaseCreateTime);
                return dtSpan.Days + "天" + dtSpan.Hours + "小时" + dtSpan.Minutes + "分"; 
            }
        }

        /// <summary>
        /// 是否为新数据
        /// </summary>
        public int IsNew { get; set; }


    }

    /// <summary>
    /// 出库监控
    /// </summary>
    public class OutboundMonitorDto
    {
        /// <summary>
        /// 出库单号 
        /// </summary>
        public string OutboundOrder { get; set; }

        /// <summary>
        /// 出库单状态
        /// </summary>
        public int OutboundStatus { get; set; }

        /// <summary>
        /// 出库单状态名称
        /// </summary>
        public string OutboundStatusName
        {
            get
            {
                return ((OutboundStatus)OutboundStatus).ToDescription();
            }
        }

        /// <summary>
        /// 出库单创建时间
        /// </summary>
        public DateTime OutboundCreateTime { get; set; }

        /// <summary>
        /// 出库单操作人
        /// </summary>
        public string OutboundEditUserName { get; set; }

        /// <summary>
        /// 入库单创建信息显示
        /// </summary>
        public string OutboundCreateDisplay
        {
            get
            {
                return OutboundCreateTime.ToString(PublicConst.DateTimeFormat) + (string.IsNullOrEmpty(OutboundEditUserName) ? string.Empty : "[" + OutboundEditUserName + "]");
            }
        }

        /// <summary>
        /// 出货单编辑时间
        /// </summary>
        public DateTime? OutboundEditTime { get; set; }

        /// <summary>
        /// 停留时间
        /// </summary>
        public string StayTime
        {
            get
            {
                TimeSpan dtSpan = DateTime.Now - Convert.ToDateTime(OutboundCreateTime);
                return dtSpan.Days + "天" + dtSpan.Hours + "小时" + dtSpan.Minutes + "分";
            }
        }

        /// <summary>
        /// 是否为新数据
        /// </summary>
        public int IsNew { get; set; }
    }

    /// <summary>
    /// 工单监控
    /// </summary>
    public class WorkMonitorDto
    {
        /// <summary>
        /// 工单单号 
        /// </summary>
        public string WorkOrder { get; set; }

        /// <summary>
        /// 工单状态
        /// </summary>
        public int WorkStatus { get; set; }

        /// <summary>
        /// 工单状态名称
        /// </summary>
        public string WorkStatusName
        {
            get
            {
                return ((WorkStatus)WorkStatus).ToDescription();
            }
        }

        /// <summary>
        /// 指派人
        /// </summary>
        public string AppointUserName { get; set; }

        /// <summary>
        /// 来源单据号
        /// </summary>
        public string DocOrder { get; set; }

        /// <summary>
        /// 工单单创建时间
        /// </summary>
        public DateTime WorkCreateTime { get; set; }

        /// <summary>
        /// 工单单操作人
        /// </summary>
        public string WorkEditUserName { get; set; }

        /// <summary>
        /// 工单创建信息显示
        /// </summary>
        public string WorkCreateDisplay
        {
            get
            {
                return WorkCreateTime.ToString(PublicConst.DateTimeFormat) + (string.IsNullOrEmpty(WorkEditUserName) ? string.Empty : "[" + WorkEditUserName + "]");
            }
        }

        /// <summary>
        /// 工单编辑时间
        /// </summary>
        public DateTime? WorkEditTime { get; set; }

        /// <summary>
        /// 停留时间
        /// </summary>
        public string StayTime
        {
            get
            {
                TimeSpan dtSpan = DateTime.Now - Convert.ToDateTime(WorkCreateTime);
                return dtSpan.Days + "天" + dtSpan.Hours + "小时" + dtSpan.Minutes + "分";
            }
        }

        /// <summary>
        /// 是否为新数据
        /// </summary>
        public int IsNew { get; set; }
    }

    /// <summary>
    /// 退货入库单监控
    /// </summary>
    public class PurchaseReturnMonitorDto
    {
        /// <summary>
        /// 退货入库单单号 
        /// </summary>
        public string PurchaseOrder { get; set; }

        /// <summary>
        /// 退货入库单状态
        /// </summary>
        public int PurchaseStatus { get; set; }

        /// <summary>
        /// 退货入库单状态名称
        /// </summary>
        public string PurchaseStatusName
        {
            get
            {
                return ((PurchaseStatus)PurchaseStatus).ToDescription();
            }
        } 

        /// <summary>
        /// 退货入库单创建时间
        /// </summary>
        public DateTime PurchaseCreateTime { get; set; }

        /// <summary>
        /// 退货入库单操作人
        /// </summary>
        public string PurchaseEditUserName { get; set; }

        /// <summary>
        /// 退货入库单创建信息显示
        /// </summary>
        public string PurchasekCreateDisplay
        {
            get
            {
                return PurchaseCreateTime.ToString(PublicConst.DateTimeFormat) + (string.IsNullOrEmpty(PurchaseEditUserName) ? string.Empty : "[" + PurchaseEditUserName + "]");
            }
        } 

        /// <summary>
        /// 停留时间
        /// </summary>
        public string StayTime
        {
            get
            {
                TimeSpan dtSpan = DateTime.Now - Convert.ToDateTime(PurchaseCreateTime);
                return dtSpan.Days + "天" + dtSpan.Hours + "小时" + dtSpan.Minutes + "分";
            }
        }

        /// <summary>
        /// 是否为新数据
        /// </summary>
        public int IsNew { get; set; }
    }

    /// <summary>
    /// 库存转移单监控
    /// </summary>
    public class StockTransferMonitorDto
    {
        /// <summary>
        /// 库存转移单单号 
        /// </summary>
        public string StockTransferOrder { get; set; }

        /// <summary>
        /// 库存转移单状态
        /// </summary>
        public int StockTransferStatus { get; set; }

        /// <summary>
        /// 库存转移单状态名称
        /// </summary>
        public string StockTransferStatusName
        {
            get
            {
                return ((StockTransferStatus)StockTransferStatus).ToDescription();
            }
        }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// UPC
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal DisplayCurrentQty { get; set; }

        /// <summary>
        /// 转移数量
        /// </summary>
        public decimal DisplayToQty { get; set; }

        /// <summary>
        /// 来源货位
        /// </summary>
        public string FromLoc { get; set; }

        /// <summary>
        /// 目标货位
        /// </summary>
        public string ToLoc { get; set; }

        /// <summary>
        /// 来源批次
        /// </summary>
        public string FromLot { get; set; }

        /// <summary>
        /// 目标批次
        /// </summary>
        public string ToLot { get; set; }

        /// <summary>
        /// 库存转移单创建时间
        /// </summary>
        public DateTime TransferCreateTime { get; set; }

        /// <summary>
        /// 库存转移单操作人
        /// </summary>
        public string TransferEditUserName { get; set; }

        /// <summary>
        /// 库存转移单创建信息显示
        /// </summary>
        public string TransferCreateDisplay
        {
            get
            {
                return TransferCreateTime.ToString(PublicConst.DateTimeFormat) + (string.IsNullOrEmpty(TransferEditUserName) ? string.Empty : "[" + TransferEditUserName + "]");
            }
        }

        /// <summary>
        /// 停留时间
        /// </summary>
        public string StayTime
        {
            get
            {
                TimeSpan dtSpan = DateTime.Now - Convert.ToDateTime(TransferCreateTime);
                return dtSpan.Days + "天" + dtSpan.Hours + "小时" + dtSpan.Minutes + "分";
            }
        }

        /// <summary>
        /// 是否为新数据
        /// </summary>
        public int IsNew { get; set; }
    }

}
