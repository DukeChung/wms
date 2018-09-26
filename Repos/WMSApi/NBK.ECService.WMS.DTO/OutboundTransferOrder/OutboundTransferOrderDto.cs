using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundTransferOrderDto
    {  /// <summary>
       /// 主键
       /// </summary>
        public Guid SysId { get; set; }
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }
        public string TransferOrder { get; set; }

        public int BoxNumber { get; set; }
        public string ServiceStationName { get; set; }
        public string PreBulkPackOrder { get; set; }

        public DateTime? CreateDate { get; set; }
        public string CreateDateDisplay
        {
            get
            {
                if (CreateDate.HasValue)
                {
                    return CreateDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }
        public int Status { get; set; }

        public string StatusName
        {
            get
            {
                return ((OutboundTransferOrderStatus)Status).ToDescription();
            }
        }

        public int TransferType { get; set; }
        public string TransferTypeName
        {
            get
            {
                return ((OutboundTransferOrderType)TransferType).ToDescription();
            }
        }

        public List<OutboundTransferOrderDetailDto> OutboundTransferOrderDetailDto { get; set; } = new List<OutboundTransferOrderDetailDto>();
    }

    public class OutboundTransferOrderDetailDto
    {
        public Guid SysId { get; set; }
        public Guid OutboundTransferOrderSysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public int Qty { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public string UomCode { get; set; }

        public string PackCode { get; set; }

        /// <summary>
        /// 导入计划数量
        /// </summary>
        public int? PreQty { get; set; }
        /// <summary>
        /// 导入商品外部Id
        /// </summary>
        public string OtherId { get; set; }

        public string SkuDescr { get; set; }

    }
}
