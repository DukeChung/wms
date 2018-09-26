using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFStockTakeQuery : BaseQuery
    {
        /// <summary>
        /// 盘点单号
        /// </summary>
        public string StockTakeOrder { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
    }
}
