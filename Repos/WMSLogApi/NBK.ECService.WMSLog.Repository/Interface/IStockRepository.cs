using Abp.Domain.Repositories;
using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Repository.Interface
{
    public interface IStockRepository : ISimpleRepository<Guid>
    {
        /// <summary>
        /// 查询有差异的库存(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        List<InvSkuLotLocLpnQty> GetStockDataSet(int type);

        /// <summary>
        /// 3张库存表可用数量比较
        /// </summary>
        /// <returns></returns>
        List<InvSkuLotLocLpnQty> GetStockAvailableQty(int type);

        /// <summary>
        /// 查询有差异的库存分配数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        List<InvSkuLotLocLpnQty> GetStockAllocatedQty(int type);

        /// <summary>
        /// 查询有差异的库存拣货数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        List<InvSkuLotLocLpnQty> GetStockPickedQty(int type);

        /// <summary>
        ///  invLot和invLotLocLpn表商品相同批次数量差异
        /// </summary>
        /// <returns></returns>
        List<InvLotAndInvLotLocLpn> GetStockSkuLotQty(int type);

        /// <summary>
        /// invSkuLoc和invLotLocLpn表商品相同货位数量差异
        /// </summary>
        /// <returns></returns>
        List<InvSkuLocAndInvLotLocLpn> GetStockSkuLocQty(int type);

        /// <summary>
        ///  查询入库，库存，出库的差异
        /// </summary>
        /// <returns></returns>
        List<DiffReceiptInvOut> GetDiffReceiptInvOut(int type);

        /// <summary>
        /// 库存分配数量和拣货明细分配数量比较
        /// </summary>
        /// <returns></returns>
        List<InvAndPickDetailAllocatedQty> GetDiffInvAndPickDetailAllocatedQty(int type);
    }
}
