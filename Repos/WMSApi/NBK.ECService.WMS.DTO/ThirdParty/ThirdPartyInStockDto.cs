using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
   
    public class ThirdPartyInStockDto
    {
        /// <summary>
        /// 采购单主键
        /// </summary>
        public int PoSysNo { get; set; }

        /// <summary>
        /// 入库人主键
        /// </summary>
        public long UserSysNo { get; set; }

        /// <summary>
        /// 入库状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public string DateTime { get; set; }

        /// <summary>
        /// 采购单明细
        /// </summary>
        public List<ThirdPartyInStockDetailDto> Items { get; set; }
    }

    public class ThirdPartyInStockB2CDto
    {
        /// <summary>
        /// 采购单主键
        /// </summary>
        public int PoSysNo { get; set; }

        /// <summary>
        /// 入库人主键
        /// </summary>
        public long UserSysNo { get; set; }

        /// <summary>
        /// 入库状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public string DateTime { get; set; }

        public new List<ThirdPartyInStockDetailB2CDto> Items { get; set; }
    }
}
