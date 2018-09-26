using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class WorkDistributionDto
    {
        /// <summary>
        /// 收获交易量
        /// </summary>
        public List<WorkDistributionListDto> ReceiptQty { get; set; }
        /// <summary>
        /// 上架
        /// </summary>
        public List<WorkDistributionListDto> ShelveQty { get; set; }
        /// <summary>
        /// 拣货
        /// </summary>
        public List<WorkDistributionListDto> PickingQty { get; set; }
        /// <summary>
        /// 发货
        /// </summary>
        public List<WorkDistributionListDto> ShipmentQty { get; set; }
    }

    public class WorkDistributionListDto
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 小时
        /// </summary>
        public string Hours { get; set; }
    }
}
