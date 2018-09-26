using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintOutboundDto : BaseDto
    {
        public Guid SysId { get; set; }
        public string OutboundOrder { get; set; }

        public string ServiceStationName { get; set; }
        /// <summary>
        /// 指派人
        /// </summary>
        public string AppointUserNames { get; set; }
        public string ExternOrderId { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ActualShipDate { get; set; }

        public string ActualShipDateText
        {
            get
            {
                if (ActualShipDate != null)
                {
                    return Convert.ToDateTime(ActualShipDate).ToString("yyyy-MM-dd HH:mm");
                }
                return "";
            }
        }

        /// <summary>
        /// 收货联系人
        /// </summary>
        public string ConsigneeName { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string ConsigneePhone { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        public string ConsigneeAddress { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public string OperateDate { get; set; }

        /// <summary>
        /// Sku数量
        /// </summary>
        public int? SkuCount { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int? SkuQty { get; set; }

        public decimal DisplaySkuQty { get; set; }

        /// <summary>
        /// 出库单类型
        /// </summary>
        public int OutboundType { get; set; }

        public string OutboundChildType { get; set; }

        #region 移仓单
        /// <summary>
        /// 移出仓
        /// </summary>
        public string FromWareHouseName { get; set; }

        /// <summary>
        /// 移入仓
        /// </summary>
        public string ToWareHouseName { get; set; }

        /// <summary>
        /// 移仓单号
        /// </summary>
        public string TransferInventoryOrder { get; set; }
        #endregion

        public List<PrintOutboundDetailDto> PrintOutboundDetailDto { get; set; }
    }
}
