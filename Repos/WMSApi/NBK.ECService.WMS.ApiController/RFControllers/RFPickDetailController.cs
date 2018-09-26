using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Core.WebApi.Filters;

namespace NBK.ECService.WMS.ApiController.RFControllers
{
    /// <summary>
    /// RF拣货
    /// </summary>
    [RoutePrefix("api/PickDetail")]
    [AccessLog]
    public class RFPickDetailController : AbpApiController
    {
        IRFPickDetailAppService _rfPickDetailAppService;
        IOutboundAppService _outboundAppService;
        IAssemblyAppService _assemblyAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        public RFPickDetailController(IRFPickDetailAppService rfPickDetailAppService, IOutboundAppService outboundAppService, IAssemblyAppService assemblyAppService)
        {
            _rfPickDetailAppService = rfPickDetailAppService;
            _outboundAppService = outboundAppService;
            _assemblyAppService = assemblyAppService;
        }

        [HttpGet]
        public void RFPickDetailAPI()
        {
        }

        /// <summary>
        /// 获取待拣货的出库单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingPickOutboundList")]
        public Pages<RFWaitingPickListDto> GetWaitingPickOutboundList(RFPickQuery request)
        {
            return _rfPickDetailAppService.GetWaitingPickOutboundList(request);
        }

        /// <summary>
        /// 获取待容器拣货的出库单
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingContainerPickingListByPaging")]
        public Pages<RFContainerPickingListDto> GetWaitingContainerPickingListByPaging(RFPickQuery pickQuery)
        {
            return _rfPickDetailAppService.GetWaitingContainerPickingListByPaging(pickQuery);
        }

        /// <summary>
        /// 获取某个出库单的待拣货商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingPickSkuList")]
        public List<RFWaitingPickSkuListDto> GetWaitingPickSkuList(RFPickQuery request)
        {
            return _rfPickDetailAppService.GetWaitingPickSkuList(request);
        }

        /// <summary>
        /// 获取出库单容器拣货明细
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetContainerPickingDetailList")]
        public RFContainerPickingDto GetContainerPickingDetailList(RFPickQuery pickQuery)
        {
            return _rfPickDetailAppService.GetContainerPickingDetailList(pickQuery);
        }

        /// <summary>
        /// 检查商品是否存在于出库单明细中
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("CheckOutboundDetailSku")]
        public RFCommResult CheckOutboundDetailSku(RFPickDetailDto request)
        {
            var result = new RFCommResult();
            try
            {
                result = _rfPickDetailAppService.CheckOutboundDetailSku(request);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 扫描拣货
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("ScanPickDetail")]
        public RFCommResult ScanPickDetail(RFPickDetailDto request)
        {
            var result = new RFCommResult();
            try
            {
                result = _rfPickDetailAppService.ScanPickDetail(request);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 容器拣货扫描
        /// </summary>
        /// <param name="pickingDetailDto"></param>
        /// <returns></returns>
        [HttpPost, Route("GenerateContainerPickingDetail")]
        public RFCommResult GenerateContainerPickingDetail(RFGenerateContainerPickingDetailDto pickingDetailDto)
        {
            var result = new RFCommResult();
            try
            {
                result = _rfPickDetailAppService.GenerateContainerPickingDetail(pickingDetailDto);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// RF拣货记录缓存
        /// </summary>
        /// <param name="setRedisDto"></param>
        /// <returns></returns>
        [HttpPost, Route("RFSetPickingRedis")]
        public RFCommResult RFSetPickingRedis(RFPickFinishDto setRedisDto)
        {
            return _rfPickDetailAppService.RFSetPickingRedis(setRedisDto);
        }

        /// <summary>
        /// RF拣货完成
        /// </summary>
        /// <param name="pickFinishDto"></param>
        /// <returns></returns>
        [HttpPost, Route("RFPickFinish")]
        public RFCommResult RFPickFinish(RFPickFinishDto pickFinishDto)
        {
            var result = new RFCommResult();
            try
            {
                result = _rfPickDetailAppService.RFPickFinish(pickFinishDto);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 拣货完成结果
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPickResult")]
        public RFPickResultDto GetPickResult(RFPickQuery pickQuery)
        {
            return _rfPickDetailAppService.GetPickResult(pickQuery);
        }

        /// <summary>
        /// 判断扫描的单号是否待拣货
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckOutboundOrder")]
        public RFCommResult CheckOutboundOrder(OutboundQuery request)
        {
            var result = new RFCommResult();
            request.WaitPickSearch = true;
            if (_outboundAppService.GetOutboundOrderByOrderId(request) != null)
            {
                result.IsSucess = true;
            }
            else
            {
                result.IsSucess = false;
                result.Message = "待拣货单据中不存在此单据号";
            }
            return result;
        }

        /// <summary>
        /// 判断扫描的单号是否待容器拣货
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckContainerPickingOutboundOrder")]
        public RFCommResult CheckContainerPickingOutboundOrder(RFOutboundQuery request)
        {
            return _rfPickDetailAppService.CheckContainerPickingOutboundOrder(request);
        }


        /// <summary>
        /// 判断容器是否可用
        /// </summary>
        /// <param name="storageCase"></param>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("CheckContainerIsAvailable")]
        public RFCommResult CheckContainerIsAvailable(string storageCase, string outboundOrder, Guid warehouseSysId)
        {
            return _rfPickDetailAppService.CheckContainerIsAvailable(storageCase, outboundOrder, warehouseSysId);
        }

        #region 加工单拣货
        /// <summary>
        /// 获取待拣货的加工单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingAssemblyList")]
        public Pages<RFWaitingAssemblyPickListDto> GetWaitingAssemblyList(RFAssemblyPickQuery request)
        {
            return _rfPickDetailAppService.GetWaitingAssemblyList(request);
        }

        /// <summary>
        /// 判断扫描的单号是否待拣货
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckAssemblyOrder")]
        public RFCommResult CheckAssemblyOrder(AssemblyQuery request)
        {
            var result = new RFCommResult();
            request.WaitPickSearch = true;
            if (_assemblyAppService.GetAssemblyOrderByOrderId(request) != null)
            {
                result.IsSucess = true;
            }
            else
            {
                result.IsSucess = false;
                result.Message = "待拣货单据中不存在此单据号";
            }
            return result;
        }

        /// <summary>
        /// 获取加工单中的待拣货商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingAssemblyPickSkuList")]
        public List<RFWaitingAssemblyPickSkuListDto> GetWaitingAssemblyPickSkuList(RFAssemblyPickQuery request)
        {
            return _rfPickDetailAppService.GetWaitingAssemblyPickSkuList(request);
        }

        /// <summary>
        /// 检查商品是否存在于加工单明细中
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("CheckAssemblyDetailSku")]
        public RFCommResult CheckAssemblyDetailSku(RFAssemblyPickDetailDto request)
        {
            var result = new RFCommResult();
            try
            {
                result = _rfPickDetailAppService.CheckAssemblyDetailSku(request);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 加工单扫描拣货
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("AssemblyScanPickDetail")]
        public RFCommResult AssemblyScanPickDetail(RFAssemblyPickDetailDto request)
        {
            var result = new RFCommResult();
            try
            {
                result = _rfPickDetailAppService.AssemblyScanPickDetail(request);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }
        #endregion
    }
}
