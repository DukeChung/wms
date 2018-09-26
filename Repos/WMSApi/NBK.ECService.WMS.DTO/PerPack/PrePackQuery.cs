using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrePackQuery : BaseQuery
    {
        public Guid SysId { get; set; }
        /// <summary>
        /// 出库订单号
        /// </summary>
        public string PrePackOrder { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }

        /// <summary>
        /// 托盘货位
        /// </summary>
        public string StorageLoc { get; set; }


        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 商品UPC
        /// </summary>
        public string SkuUPC { get; set; }

        public string BatchNumber { get; set; }

        public string ServiceStationName { get; set; }

        /// <summary>
        /// 创建时间从
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }
        /// <summary>
        /// 创建时间到
        /// </summary>
        public DateTime? CreateDateTo { get; set; }
    }

    public class PrePackCopy : BaseQuery
    {
        public Guid SysId { get; set; }
        public int CopyNumber { get; set; }
    }
}
