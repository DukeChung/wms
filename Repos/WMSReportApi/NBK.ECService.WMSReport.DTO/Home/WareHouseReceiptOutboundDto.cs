using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class WareHouseReceiptOutboundDto
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        public List<ReceiptOutboundData> ReceiptOutboundData { get; set; }

        /// <summary>
        /// 图表展示数据集合
        /// </summary>
        public List<ReceiptOutboundList> ReceiptOutboundList { get; set; }

        /// <summary>
        /// 日期集合
        /// </summary>
        public string[] Dates { get; set; }

    }
    public class ReceiptOutboundList
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid WareHouseSysId { get; set; }
        /// <summary>
        /// 仓库Id
        /// </summary>
        public string WareHouseName { get; set; }

        public List<int> Qty { get; set; }

    }


    public class ReceiptOutboundData
    {
        /// <summary>
        /// 仓库Id
        /// </summary>
        public Guid WareHouseSysId { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        public string WareHouseName { get; set; }

        /// <summary>
        /// 入库数据
        /// </summary>
        public int? Qty { get; set; }

        /// <summary>
        /// 日期数据
        /// </summary>
        public DateTime? Date { get; set; }
        public string DateDisplay
        {
            get
            {
                { return Date.HasValue ? Date.Value.ToString(PublicConst.DateFormat) : string.Empty; }
            }
        }


    }
}
