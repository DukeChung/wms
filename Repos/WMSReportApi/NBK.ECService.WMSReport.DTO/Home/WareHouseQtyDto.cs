using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    /// <summary>
    /// 仓库收货，出库，库存分布
    /// </summary>
    public class WareHouseQtyDto
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WareHouseName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public long Qty { get; set; }

        /// <summary>
        /// 显示数量
        /// </summary>
        public long DisplayQty { get { return Math.Abs(Qty); } }
    }
}
