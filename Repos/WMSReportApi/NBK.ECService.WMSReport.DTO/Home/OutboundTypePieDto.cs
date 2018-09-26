using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class ReturnOutboundTypePieDto
    {
        /// <summary>
        /// 出库总数量
        /// </summary>
        public long OutboundQty { get; set; }
        /// <summary>
        /// b2c出库
        /// </summary>
        public double B2COutboundPie { get; set; }
        /// <summary>
        /// b2b出库
        /// </summary>
        public double B2BOutboundPie { get; set; }
        /// <summary>
        /// 退货出库
        /// </summary>
        public double ReturnOutboundPie { get; set; }
        /// <summary>
        /// 移仓出库
        /// </summary>
        public double MoveOutboundPie { get; set; }
        /// <summary>
        /// 农资出库
        /// </summary>
        public double FertilizerOutboundPie { get; set; }
    }

    public class OutboundTypePieDto
    {
        public long Qty { get; set; }
        public int OutboundType { get; set; }

        public double Pie { get; set; }
    }
}
