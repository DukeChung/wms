using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IRFInventoryRepository : ICrudRepository
    {
        /// <summary>
        /// RF库存查询
        /// </summary>
        /// <param name="invSkuLocQuery"></param>
        /// <returns></returns>
        List<RFInvSkuLocListDto> GetInvSkuLocList(RFInvSkuLocQuery invSkuLocQuery);
    }
}
