using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ChannelInventoryDto
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WareHouseName { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// 商品UPC
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 外部ID
        /// </summary>
        public string OtherId { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }
        /// <summary>
        /// 冻结数量
        /// </summary>
        public decimal FrozenQty { get; set; }
        /// <summary>
        /// 分配数量
        /// </summary>
        public decimal AllocatedQty { get; set; }
        /// <summary>
        /// 拣货数量
        /// </summary>
        public decimal PickedQty { get; set; }
    }
}
