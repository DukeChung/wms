using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
  
    public class ThirdPartyInStockDetailDto
    {
        /// <summary>
        /// 产品主键
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 入库数量
        /// </summary>
        public int Quantity { get; set; }


        /// <summary>
        /// 拒收数量
        /// </summary>
        public int RejectedQty { get; set; }

        /// <summary>
        /// 拒收数量
        /// </summary>
        public string Remark { get; set; }


    }


    public class ThirdPartyInStockDetailB2CDto 
    {
        /// <summary>
        /// 产品主键
        /// </summary>
        public new string ProductSysNo { get; set; }

        /// <summary>
        /// 入库数量
        /// </summary>
        public int Quantity { get; set; }


        /// <summary>
        /// 拒收数量
        /// </summary>
        public int RejectedQty { get; set; }

        /// <summary>
        /// 拒收数量
        /// </summary>
        public string Remark { get; set; }
    }
}
