using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyStockTransferDto : BaseDto
    {
        /// <summary>
        /// 仓库Id(对应Warehouse表OtherId)
        /// </summary>
        public string WarehouseId { get; set; }

        public List<ThirdPartyStockTransferDetailDto> ThirdPartyStockTransferDetailDtoList { get; set; }
    }
}
