using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IStockMovementRepository : ICrudRepository
    {
        /// <summary>
        /// 获取库存移动SKU列表
        /// </summary>
        /// <param name="stockMovementSkuQuery"></param>
        /// <returns></returns>
        Pages<StockMovementSkuDto> GetStockMovementSkuListByPaging(StockMovementSkuQuery stockMovementSkuQuery);

        /// <summary>
        /// 获取库存移动信息
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="loc"></param>
        /// <param name="lot"></param>
        /// <returns></returns>
        StockMovementDto GetStockMovement(Guid skuSysId, string loc, string lot, Guid wareHouseSysId);

        /// <summary>
        /// 获取移动单列表
        /// </summary>
        /// <param name="stockMovementQuery"></param>
        /// <returns></returns>
        Pages<StockMovementDto> GetStockMovementList(StockMovementQuery stockMovementQuery);

        /// <summary>
        /// 获取SkuPackUom
        /// </summary>
        /// <param name="skuSysIds"></param>
        /// <returns></returns>
        List<SkuPackUomDto> GetSkuPackUomList(IEnumerable<Guid> skuSysIds);
    }
}
