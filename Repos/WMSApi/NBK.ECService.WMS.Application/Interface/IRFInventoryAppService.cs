using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IRFInventoryAppService : IApplicationService
    {
        /// <summary>
        /// RF库存查询
        /// </summary>
        /// <param name="invSkuLocQuery"></param>
        /// <returns></returns>
        List<RFInvSkuLocListDto> GetInvSkuLocList(RFInvSkuLocQuery invSkuLocQuery);

        #region RF库位变更
        /// <summary>
        /// RF库位变更
        /// </summary>
        /// <param name="rfStockMovementDto"></param>
        /// <returns></returns>
        RFCommResult StockMovement(RFStockMovementDto rfStockMovementDto);
        #endregion
    }
}
