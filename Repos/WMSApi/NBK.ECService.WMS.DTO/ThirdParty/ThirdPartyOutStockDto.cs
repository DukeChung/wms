using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
  
    public class ThirdPartyOutStockDto
    {
        /// <summary>
        /// 出库订单主键
        /// </summary>
        public int OrderSysNo { get; set; }

        /// <summary>
        /// 出库人主键
        /// </summary>
        public long UserSysNo { get; set; }

        /// <summary>
        /// 出库状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 出库时间
        /// </summary>
        public string DateTime { get; set; }
    }
}
