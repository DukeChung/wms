using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IStockTransferAppService
    {
        Pages<StockTransferLotListDto> GetStockTransferLotByPage(StockTransferQuery request);

        StockTransferDto GetStockTransferBySysId(Guid sysid, Guid warehouseSysId);

        void CreateStockTransfer(StockTransferDto st);

        Pages<StockTransferDto> GetStockTransferOrderByPage(StockTransferQuery request);

        void StockTransferOperation(StockTransferDto request);

        void StockTransferCancel(StockTransferDto request);

        StockTransferDto GetStockTransferOrderBySysId(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 根据批次属性库存转移
        /// </summary>
        /// <param name="stockTransferDto"></param>
        /// <returns></returns>
        bool StockTransferByLotAttr(StockTransferDto stockTransferDto);
    }
}
