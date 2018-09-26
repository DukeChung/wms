using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty.OutboundReturn
{
    public class ECCReturnOrder
    {
        public int OriginalOutStockId { get; set; }

        public int WarhouseSysId { get; set; }

        public string RequestUser { get; set; }

        /// <summary>
        /// 固定传WMS
        /// </summary>
        public string SourcePlatform { get; set; }

        /// <summary>
        /// 0: 全部退货    1:部分退货
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public List<ECCReturnOrderDetail> ReturnDetailList { get; set; }
    }

    public class ECCReturnOrderDetail
    {
        public string ProductCode { get; set; }

        public int Qty { get; set; }

        public string ReturnReason { get; set; }
    }

    public enum ECCReturnOrderType
    {
        /// <summary>
        /// 全部退货
        /// </summary>
        [Description("全部退货")]
        All = 0,

        /// <summary>
        /// 部分退货
        /// </summary>
        [Description("部分退货")]
        Part = 1
    }
}
