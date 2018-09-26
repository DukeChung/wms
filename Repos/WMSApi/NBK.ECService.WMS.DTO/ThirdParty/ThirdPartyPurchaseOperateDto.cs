using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyPurchaseOperateDto : BaseDto
    {
        /// <summary>
        /// 外部单据号, SysNo
        /// </summary>
        public string ExternalOrder { get; set; }

        public string WareHouseId { get; set; }
    }
}
