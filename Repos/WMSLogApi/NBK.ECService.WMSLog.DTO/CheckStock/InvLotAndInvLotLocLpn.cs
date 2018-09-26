using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class InvLotAndInvLotLocLpn
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
        /// 批次
        /// </summary>
        public string Lot { get; set; }
        /// <summary>
        /// invskuLoc数量
        /// </summary>
        public decimal InvSkuLocQty { get; set; }
        /// <summary>
        /// invLotLocLpn数量
        /// </summary>
        public decimal InvLotLocLpnQty { get; set; }
    }
}
