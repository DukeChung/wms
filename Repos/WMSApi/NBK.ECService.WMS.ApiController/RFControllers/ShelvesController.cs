using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.RFControllers
{
    /// <summary>
    /// RF上架
    /// </summary>
    [RoutePrefix("api/Shelves")]
    [AccessLog]
    public class ShelvesController : AbpApiController
    {
        IShelvesAppService _shelvesAppService;
        IReceiptAppService _receiptAppService;
        IAssemblyAppService _assemblyAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        public ShelvesController(IShelvesAppService shelvesAppService, IReceiptAppService receiptAppService, IAssemblyAppService assemblyAppService)
        {
            _shelvesAppService = shelvesAppService;
            _receiptAppService = receiptAppService;
            _assemblyAppService = assemblyAppService;
        }

        [HttpGet]
        public void ShelvesAPI()
        {
        }

        /// <summary>
        /// 查询待上架单据列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingShelvesList")]
        public Pages<RFWaitingShelvesListDto> GetWaitingShelvesList(RFShelvesQuery request)
        {
            return _shelvesAppService.GetWaitingShelvesList(request);
        }

        /// <summary>
        /// 查询某收货单待上架商品列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingShelvesSkuList")]
        public List<RFWaitingShelvesSkuListDto> GetWaitingShelvesSkuList(RFShelvesQuery request)
        {
            return _shelvesAppService.GetWaitingShelvesSkuList(request);
        }

        /// <summary>
        /// 获取收货明细推荐货位
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAdviceToLoc")]
        public string GetAdviceToLoc(RFShelvesQuery request)
        {
            return _shelvesAppService.GetAdviceToLoc(request);
        }

        /// <summary>
        /// 查询库存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInventoryList")]
        public List<RFInventoryListDto> GetInventoryList(RFInventoryQuery request)
        {
            return _shelvesAppService.GetInventoryList(request);
        }

        /// <summary>
        /// 扫描上架
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("ScanShelves")]
        public RFCommResult ScanShelves(ScanShelvesDto request)
        {
            var result = new RFCommResult();
            try
            {
                result = _shelvesAppService.ScanShelves(request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                result.Message = "操作失败!";
                throw new Exception(ex.Message);
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 扫描上架
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        [HttpPost, Route("AutoShelves")]
        public RFCommResult AutoShelves(ScanShelvesDto scanShelvesDto)
        {
            var result = new RFCommResult();
            try
            {
                result = _shelvesAppService.AutoShelves(scanShelvesDto);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                result.Message = "操作失败!";
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 检查商品是否存在于收货明细中
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckReceiptDetailSku")]
        public RFCommResult CheckReceiptDetailSku(ScanShelvesDto request)
        {
            var result = new RFCommResult();
            try
            {
                result = _shelvesAppService.CheckReceiptDetailSku(request);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 判断扫描的单号是否待上架
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckReceiptOrder")]
        public RFCommResult CheckReceiptOrder(ReceiptQuery request)
        {
            var result = new RFCommResult();
            request.WaitShelvesSearch = true;
            if (_receiptAppService.GetReceiptOrderByOrderId(request) != null)
            {
                result.IsSucess = true;
            }
            else
            {
                result.IsSucess = false;
                result.Message = "待上架单据中不存在此单据号";
            }
            return result;
        }

        /// <summary>
        /// 判断扫描的加工单是否待上架
        /// </summary>
        /// <param name="assemblyOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("CheckAssemblyOrderNotOnShelves")]
        public RFCommResult CheckAssemblyOrderNotOnShelves(string assemblyOrder, Guid warehouseSysId)
        {
            return _assemblyAppService.CheckAssemblyOrderNotOnShelves(assemblyOrder, warehouseSysId);
        }

        /// <summary>
        /// 获取待上架加工单列表
        /// </summary>
        /// <param name="assemblyShelvesQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAssemblyWaitingShelvesList")]
        public Pages<RFAssemblyWaitingShelvesListDto> GetAssemblyWaitingShelvesList(RFAssemblyShelvesQuery assemblyShelvesQuery)
        {
            return _shelvesAppService.GetAssemblyWaitingShelvesList(assemblyShelvesQuery);
        }

        /// <summary>
        /// 获取加工单待上架商品列表
        /// </summary>
        /// <param name="assemblyShelvesQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAssemblyWaitingShelvesSkuList")]
        public List<RFAssemblyWaitingShelvesSkuListDto> GetAssemblyWaitingShelvesSkuList(RFAssemblyShelvesQuery assemblyShelvesQuery)
        {
            return _shelvesAppService.GetAssemblyWaitingShelvesSkuList(assemblyShelvesQuery);
        }

        /// <summary>
        /// 加工单成品上架校验
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckAssemblyWaitShelvesSku")]
        public RFCommResult CheckAssemblyWaitShelvesSku(RFAssemblyScanShelvesDto scanShelvesDto)
        {
            return _shelvesAppService.CheckAssemblyWaitShelvesSku(scanShelvesDto);
        }

        /// <summary>
        /// 加工单成品扫描上架
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        [HttpPost, Route("AssemblyScanShelves")]
        public RFCommResult AssemblyScanShelves(RFAssemblyScanShelvesDto scanShelvesDto)
        {
            return _shelvesAppService.AssemblyScanShelves(scanShelvesDto);
        }

        /// <summary>
        /// 取消上架
        /// </summary>
        /// <param name="cancelShelvesDto"></param>
        /// <returns></returns>
        [HttpPost, Route("CancelShelves")]
        public CommonResponse CancelShelves(CancelShelvesDto cancelShelvesDto)
        {
            var rsp = new CommonResponse(false);
            try
            {
                rsp = _shelvesAppService.CancelShelves(cancelShelvesDto);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                rsp.Message = "操作失败!";
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                rsp.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return rsp;
        }
    }
}
