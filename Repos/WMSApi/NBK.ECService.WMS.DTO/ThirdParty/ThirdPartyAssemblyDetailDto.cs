using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyAssemblyDetailDto
    {
        public string OtherSkuId { get; set; }

        /// <summary>
        /// 数量(显示用)
        /// </summary>
        public decimal UnitQty { get; set; }

        /// <summary>
        /// 品级
        /// </summary>
        public string Grade { get; set; }
    }
}
