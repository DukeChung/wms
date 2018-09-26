using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class InvSkuLotLocLpnQty
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
        /// invLot数量
        /// </summary>
        public decimal InvLotQty { get; set; }
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
