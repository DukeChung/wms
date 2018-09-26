using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrePackBoardDto
    {
        public Guid SysId { get; set; }
        public Guid? OutboundSysId { get; set; }

        /// <summary>
        /// 预包装单号
        /// </summary>
        public string PrePackOrder { get; set; }
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }
        /// <summary>
        /// 托盘货位
        /// </summary>
        public string StorageLoc { get; set; }
        /// <summary>
        /// 预包装数量
        /// </summary>
        public int PreQty { get; set; }

        /// <summary>
        /// 实际包装数量
        /// </summary>
        public int Qty { get; set; }
        public string Progress
        {
            get
            {
                if (PreQty != 0&& Qty!=0)
                {
                    if (PreQty > Qty)
                    {
                        return ((Qty / (decimal)PreQty) * 100).ToString("F2")+ "%";
                    }
                    else
                    {
                        return "100%";
                    }

                }
                return "0.00%";
            }
        }

        public int Type { get; set; }
    }


    public class OutboundSkuList {
        public Guid? OutboundSysId { get; set; }

        public string OutboundOrder { get; set; }
        public Guid? SkuSysId { get; set; }
        public int? Qty { get; set; }
    }

    public class PrePackOutboundSkuList
    {
        public Guid? OutboundSysId { get; set; }

        public string OutboundOrder { get; set; }
        public Guid? SkuSysId { get; set; }
        public int? Qty { get; set; }
    }
}
