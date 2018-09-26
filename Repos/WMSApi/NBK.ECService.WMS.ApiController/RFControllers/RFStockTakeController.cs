using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.ApiController.RFControllers
{
    /// <summary>
    /// RF盘点
    /// </summary>
    [RoutePrefix("api/RFStockTake")]
    [AccessLog]
    public class RFStockTakeController : AbpApiController
    {
        IRFStockTakeAppService _rFStockTakeAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        public RFStockTakeController(IRFStockTakeAppService rFStockTakeAppService)
        {
            _rFStockTakeAppService = rFStockTakeAppService;
        }

        [HttpGet]
        public void RFStockTakeAPI()
        {
        }

        /// <summary>
        /// 查询未盘点库存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInventoryNoStockTakeList")]
        public List<RFInventoryListDto> GetInventoryNoStockTakeList(RFInventoryQuery request)
        {
            return _rFStockTakeAppService.GetInventoryNoStockTakeList(request);
        }

        /// <summary>
        /// 初盘保存盘点单和盘点明细或修改盘点明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveStockTake")]
        public RFCommResult SaveStockTake(StockTakeFirstDto request)
        {
            var result = new RFCommResult();
            try
            {
                result = _rFStockTakeAppService.SaveStockTake(request);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 初盘清单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTakeFirstList")]
        public RFStockTakeListingDto GetStockTakeFirstList(RFStockTakeQuery request)
        {
            return _rFStockTakeAppService.GetStockTakeFirstList(request);
        }

        /// <summary>
        /// 获取待初盘单据列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTakeFirstListByPaging")]
        public Pages<RFStockTakeListDto> GetStockTakeFirstListByPaging(RFStockTakeQuery stockTakeQuery)
        {
            return _rFStockTakeAppService.GetStockTakeFirstListByPaging(stockTakeQuery);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        [HttpGet, Route("CheckStockTakeFirstDetailSku")]
        public RFCheckStockTakeFirstDetailSkuDto CheckStockTakeFirstDetailSku(string upc)
        {
            return _rFStockTakeAppService.CheckStockTakeFirstDetailSku(upc);
        }

        /// <summary>
        /// 获取初盘明细
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTakeFirstDetailList")]
        public List<StockTakeFirstListDto> GetStockTakeFirstDetailList(RFStockTakeQuery stockTakeQuery)
        {
            return _rFStockTakeAppService.GetStockTakeFirstDetailList(stockTakeQuery);
        }

        /// <summary>
        /// 初盘扫描
        /// </summary>
        /// <param name="stockTakeFirstDto"></param>
        /// <returns></returns>
        [HttpPost, Route("StockTakeFirstScanning")]
        public RFCommResult StockTakeFirstScanning(StockTakeFirstDto stockTakeFirstDto)
        {
            return _rFStockTakeAppService.StockTakeFirstScanning(stockTakeFirstDto);
        }

        /// <summary>
        /// 复盘单据列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTakeSecondListByPage")]
        public Pages<RFStockTakeListDto> GetStockTakeSecondListByPage(RFStockTakeQuery request)
        {
            return _rFStockTakeAppService.GetStockTakeSecondListByPage(request);
        }

        /// <summary>
        /// 复盘清单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTakeSecondList")]
        public List<StockTakeSecondListDto> GetStockTakeSecondList(RFStockTakeQuery request)
        {
            return _rFStockTakeAppService.GetStockTakeSecondList(request);
        }

        /// <summary>
        /// 复盘
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("StockTakeSecond")]
        public RFCommResult StockTakeSecond(StockTakeSecondDto request)
        {
            var result = new RFCommResult();
            try
            {
                result = _rFStockTakeAppService.StockTakeSecond(request);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 复盘修改盘点单状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateSecondStockTakeStatus")]
        public RFCommResult UpdateSecondStockTakeStatus(RFStockTakeQuery request)
        {
            var result = new RFCommResult();
            try
            {
                request.Status = (int)StockTakeStatus.ReplayFinished;
                result = _rFStockTakeAppService.UpdateStockTakeStatus(request);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 修改盘点单状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateFirstStockTakeStatus")]
        public RFCommResult UpdateFirstStockTakeStatus(RFStockTakeQuery request)
        {
            var result = new RFCommResult();
            try
            {
                request.Status = (int)StockTakeStatus.StockTakeFinished;
                result = _rFStockTakeAppService.UpdateStockTakeStatus(request);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 检查复盘单据是否存在
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("CheckSecondStockTake")]
        public RFCommResult CheckSecondStockTake(RFStockTakeQuery request)
        {
            var result = new RFCommResult();
            request.Status = (int)StockTakeStatus.Replay;
            if (_rFStockTakeAppService.GetStockTakeByOrder(request) != null)
            {
                result.IsSucess = true;
            }
            else
            {
                result.IsSucess = false;
                result.Message = "待复盘单据中不存在此单据";
            }
            return result;
        }

        /// <summary>
        /// 检查初盘单据是否存在
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("CheckFirstStockTake")]
        public RFCommResult CheckFirstStockTake(RFStockTakeQuery request)
        {
            var result = new RFCommResult();
            request.Status = (int)StockTakeStatus.Started;
            if (_rFStockTakeAppService.GetStockTakeByOrder(request) != null)
            {
                result.IsSucess = true;
            }
            else
            {
                result.IsSucess = false;
                result.Message = "待初盘单据中不存在此单据";
            }
            return result;
        }
    }
}
