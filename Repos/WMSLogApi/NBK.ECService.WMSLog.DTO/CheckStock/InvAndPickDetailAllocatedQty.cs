using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class InvAndPickDetailAllocatedQty
    {

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string SkuCode { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// 库存分配数量
        /// </summary>
        public decimal InventoryAllocation { get; set; }
        /// <summary>
        /// 拣货明细分配数量
        /// </summary>
        public decimal PickingDetails { get; set; }
    }
}
