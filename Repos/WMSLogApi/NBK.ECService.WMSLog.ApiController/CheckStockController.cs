using Abp.WebApi.Controllers;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMSLog.ApiController
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/CheckStock")]
    public class CheckStockController : AbpApiController
    {
        private ICheckStockAppService _checkStockAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkStockAppService"></param>
        public CheckStockController(ICheckStockAppService checkStockAppService)
        {
            _checkStockAppService = checkStockAppService;
        }


        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void CheckStockAPI() { }

        /// <summary>
        /// 查询有差异的库存(3张库存表差异)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStockDataSet")]
        public List<InvSkuLotLocLpnQty> GetStockDataSet(int type)
        {
            return _checkStockAppService.GetStockDataSet(type);
        }

        /// <summary>
        /// 3张库存表可用数量比较
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStockAvailableQty")]
        public List<InvSkuLotLocLpnQty> GetStockAvailableQty(int type)
        {
            return _checkStockAppService.GetStockAvailableQty(type);
        }

        /// <summary>
        /// /查询有差异的库存分配数量(3张库存表差异)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStockAllocatedQty")]
        public List<InvSkuLotLocLpnQty> GetStockAllocatedQty(int type)
        {
            return _checkStockAppService.GetStockAllocatedQty(type);
        }

        /// <summary>
        /// 查询有差异的库存拣货数量(3张库存表差异)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStockPickedQty")]
        public List<InvSkuLotLocLpnQty> GetStockPickedQty(int type)
        {
            return _checkStockAppService.GetStockPickedQty(type);
        }

        /// <summary>
        /// invLot和invLotLocLpn表商品相同批次数量差异
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStockSkuLotQty")]
        public List<InvLotAndInvLotLocLpn> GetStockSkuLotQty(int type)
        {
            return _checkStockAppService.GetStockSkuLotQty(type);
        }


        /// <summary>
        /// invSkuLoc和invLotLocLpn表商品相同货位数量差异
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStockSkuLocQty")]
        public List<InvSkuLocAndInvLotLocLpn> GetStockSkuLocQty(int type)
        {
            return _checkStockAppService.GetStockSkuLocQty(type);
        }

        /// <summary>
        /// 查询入库，库存，出库的差异
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("GetDiffReceiptInvOut")]
        public List<DiffReceiptInvOut> GetDiffReceiptInvOut(int type)
        {
            return _checkStockAppService.GetDiffReceiptInvOut(type);
        }

        /// <summary>
        /// 库存分配数量和拣货明细分配数量比较
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("GetDiffInvAndPickDetailAllocatedQty")]
        public List<InvAndPickDetailAllocatedQty> GetDiffInvAndPickDetailAllocatedQty(int type)
        {
            return _checkStockAppService.GetDiffInvAndPickDetailAllocatedQty(type);
        }
    }
}
