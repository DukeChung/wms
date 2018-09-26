using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application
{
    public class CheckStockAppService : ICheckStockAppService
    {
        private ICrudRepository _crudRepository = null;
        private IStockRepository _stockRepository = null;
        public CheckStockAppService(ICrudRepository crudRepository, IStockRepository stockRepository)
        {
            this._crudRepository = crudRepository;
            this._stockRepository = stockRepository;
        }


        /// <summary>
        /// 查询有差异的库存(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public List<InvSkuLotLocLpnQty> GetStockDataSet(int type)
        {
            _crudRepository.ChangeDB(type);
            return _stockRepository.GetStockDataSet(type);
        }

        /// <summary>
        /// 3张库存表可用数量比较
        /// </summary>
        /// <returns></returns>
        public List<InvSkuLotLocLpnQty> GetStockAvailableQty(int type)
        {
            _crudRepository.ChangeDB(type);
            return _stockRepository.GetStockAvailableQty(type);
        }

        /// <summary>
        /// 查询有差异的库存分配数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public List<InvSkuLotLocLpnQty> GetStockAllocatedQty(int type)
        {
            _crudRepository.ChangeDB(type);
            return _stockRepository.GetStockAllocatedQty(type);
        }

        /// <summary>
        /// 查询有差异的库存拣货数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public List<InvSkuLotLocLpnQty> GetStockPickedQty(int type)
        {
            _crudRepository.ChangeDB(type);
            return _stockRepository.GetStockPickedQty(type);
        }

        /// <summary>
        ///  invLot和invLotLocLpn表商品相同批次数量差异
        /// </summary>
        /// <returns></returns>
        public List<InvLotAndInvLotLocLpn> GetStockSkuLotQty(int type)
        {
            _crudRepository.ChangeDB(type);
            return _stockRepository.GetStockSkuLotQty(type);
        }

        /// <summary>
        /// invSkuLoc和invLotLocLpn表商品相同货位数量差异
        /// </summary>
        /// <returns></returns>
        public List<InvSkuLocAndInvLotLocLpn> GetStockSkuLocQty(int type)
        {
            _crudRepository.ChangeDB(type);
            return _stockRepository.GetStockSkuLocQty(type);
        }

        /// <summary>
        ///  查询入库，库存，出库的差异
        /// </summary>
        /// <returns></returns>
        public List<DiffReceiptInvOut> GetDiffReceiptInvOut(int type)
        {
            _crudRepository.ChangeDB(type);
            return _stockRepository.GetDiffReceiptInvOut(type);
        }

        /// <summary>
        /// 库存分配数量和拣货明细分配数量比较
        /// </summary>
        /// <returns></returns>
        public List<InvAndPickDetailAllocatedQty> GetDiffInvAndPickDetailAllocatedQty(int type)
        {
            _crudRepository.ChangeDB(type);
            return _stockRepository.GetDiffInvAndPickDetailAllocatedQty(type);
        }

    }
}
