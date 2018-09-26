using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBK.WMSLog.Portal.Services
{
    public class CheckStockApiClient
    {
        private static readonly CheckStockApiClient instance = new CheckStockApiClient();

        private CheckStockApiClient() { }

        public static CheckStockApiClient GetInstance() { return instance; }

        /// <summary>
        /// 查询有差异的库存(3张库存表差异)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<InvSkuLotLocLpnQty>> GetStockDataSet(CoreQuery query, int type)
        {
            query.ParmsObj = new { type };
            return ApiClient.Get<List<InvSkuLotLocLpnQty>>(PublicConst.WmsLogApiUrl, "/CheckStock/GetStockDataSet", query);
        }

        /// <summary>
        /// 3张库存表可用数量比较
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<InvSkuLotLocLpnQty>> GetStockAvailableQty(CoreQuery query, int type)
        {
            query.ParmsObj = new { type };
            return ApiClient.Get<List<InvSkuLotLocLpnQty>>(PublicConst.WmsLogApiUrl, "/CheckStock/GetStockAvailableQty", query);
        }

        /// <summary>
        /// 查询有差异的库存分配数量(3张库存表差异)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<InvSkuLotLocLpnQty>> GetStockAllocatedQty(CoreQuery query, int type)
        {
            query.ParmsObj = new { type };
            return ApiClient.Get<List<InvSkuLotLocLpnQty>>(PublicConst.WmsLogApiUrl, "/CheckStock/GetStockAllocatedQty", query);
        }

        /// <summary>
        /// 查询有差异的库存拣货数量(3张库存表差异)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<InvSkuLotLocLpnQty>> GetStockPickedQty(CoreQuery query, int type)
        {
            query.ParmsObj = new { type };
            return ApiClient.Get<List<InvSkuLotLocLpnQty>>(PublicConst.WmsLogApiUrl, "/CheckStock/GetStockPickedQty", query);
        }

        /// <summary>
        /// invLot和invLotLocLpn表商品相同批次数量差异
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<InvLotAndInvLotLocLpn>> GetStockSkuLotQty(CoreQuery query, int type)
        {
            query.ParmsObj = new { type };
            return ApiClient.Get<List<InvLotAndInvLotLocLpn>>(PublicConst.WmsLogApiUrl, "/CheckStock/GetStockSkuLotQty", query);
        }

        /// <summary>
        /// invSkuLoc和invLotLocLpn表商品相同货位数量差异
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<InvSkuLocAndInvLotLocLpn>> GetStockSkuLocQty(CoreQuery query, int type)
        {
            query.ParmsObj = new { type };
            return ApiClient.Get<List<InvSkuLocAndInvLotLocLpn>>(PublicConst.WmsLogApiUrl, "/CheckStock/GetStockSkuLocQty", query);
        }

        /// <summary>
        ///  查询入库，库存，出库的差异
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<DiffReceiptInvOut>> GetDiffReceiptInvOut(CoreQuery query, int type)
        {
            query.ParmsObj = new { type };
            return ApiClient.Get<List<DiffReceiptInvOut>>(PublicConst.WmsLogApiUrl, "/CheckStock/GetDiffReceiptInvOut", query);
        }

        /// <summary>
        /// 库存分配数量和拣货明细分配数量比较
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<InvAndPickDetailAllocatedQty>> GetDiffInvAndPickDetailAllocatedQty(CoreQuery query, int type)
        {
            query.ParmsObj = new { type };
            return ApiClient.Get<List<InvAndPickDetailAllocatedQty>>(PublicConst.WmsLogApiUrl, "/CheckStock/GetDiffInvAndPickDetailAllocatedQty", query);
        }
    }
}