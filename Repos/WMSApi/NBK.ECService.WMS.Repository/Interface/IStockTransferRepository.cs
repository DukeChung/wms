using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IStockTransferRepository : ICrudRepository
    {
        Pages<StockTransferLotListDto> GetStockTransferLotByPage(StockTransferQuery request);

        StockTransferDto GetStockTransferBySysId(Guid sysid);

        Pages<StockTransferDto> GetStockTransferOrderByPage(StockTransferQuery request);

        StockTransferDto GetStockTransferOrderBySysId(Guid sysId);
    }
}
