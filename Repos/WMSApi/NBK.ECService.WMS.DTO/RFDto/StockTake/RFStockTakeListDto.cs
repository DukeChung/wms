using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class RFStockTakeListDto : BaseDto
    {
        /// <summary>
        /// 盘点Id
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 盘点单号
        /// </summary>
        public string StockTakeOrder { get; set; }

        /// <summary>
        /// Sku数
        /// </summary>
        public int SkuCount { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        public string StatusName
        {
            get { return ((StockTakeStatus)Status).ToDescription(); }
        }
    }
}
