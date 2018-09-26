using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class ChannelInventoryGlobalQuery : BaseQuery
    {
        public Guid SearchWarehouseSysId { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// 商品UPC
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 商品外部ID
        /// </summary>
        public string OtherId { get; set; }
        /// <summary>
        /// 是否库存为零
        /// </summary>
        public bool? IsStoreZero { get; set; }
    }
}
