using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFInventoryListDto
    {
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        /// <summary>
        /// 盘点时间
        /// </summary>
        public DateTime? StockTakeTime { get; set; }

        public string StockTakeTimeText
        {
            get
            {
                if(StockTakeTime != null)
                {
                    return Convert.ToDateTime(StockTakeTime).ToString("yyyy-MM-dd HH:mm:ss");
                }
                return "";
            }
        }
    }
}
