using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IStockMovementAppService : IApplicationService
    {
        /// <summary>
        /// 获取库存移动SKU列表
        /// </summary>
        /// <param name="stockMovementSkuQuery"></param>
        /// <returns></returns>
        Pages<StockMovementSkuDto> GetStockMovementSkuList(StockMovementSkuQuery stockMovementSkuQuery);

        /// <summary>
        /// 获取库存移动信息
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="loc"></param>
        /// <param name="lot"></param>
        /// <returns></returns>
        StockMovementDto GetStockMovement(Guid skuSysId, string loc, string lot, Guid wareHouseSysId);

        /// <summary>
        /// 保存调整
        /// </summary>
        /// <param name="stockMovementDto"></param>
        void SaveStockMovement(StockMovementDto stockMovementDto);

        /// <summary>
        /// 获取移动单列表
        /// </summary>
        /// <param name="stockMovementQuery"></param>
        /// <returns></returns>
        Pages<StockMovementDto> GetStockMovementList(StockMovementQuery stockMovementQuery);

        /// <summary>
        /// 确认移动
        /// </summary>
        /// <param name="sysIds"></param>
        void ConfirmStockMovement(StockMovementOperationDto stockMovementOperationDto);

        /// <summary>
        /// 取消移动
        /// </summary>
        /// <param name="sysIds"></param>
        void CancelStockMovement(StockMovementOperationDto stockMovementOperationDto);

        /// <summary>
        /// 导入库位变更
        /// </summary>
        /// <param name="list"></param>
        void ImportStockMovementList(ImportStockMovement list);
    }
}
