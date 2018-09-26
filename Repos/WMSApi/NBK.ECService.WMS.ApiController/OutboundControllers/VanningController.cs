using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Core.WebApi.Filters;

namespace NBK.ECService.WMS.ApiController.OutboundControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Outbound/Vanning")]
    [AccessLog]
    public class VanningController : AbpApiController
    {
        private IVanningAppService _vanningAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        /// <param name="vanningAppService"></param>
        public VanningController(IVanningAppService vanningAppService)
        {
            _vanningAppService = vanningAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void VanningAPI()
        {
        }

        /// <summary>
        /// 内裤扫描仪
        /// </summary>
        /// <param name="vanningQueryDto"></param>
        /// <returns></returns>
        [HttpPost, Route("GetVanningList")]
        public Pages<VanningDto> GetVanningList(VanningQueryDto vanningQueryDto)
        {
            return _vanningAppService.GetVanningList(vanningQueryDto);
        }


        /// <summary>
        /// 获取装箱操作相关数据
        /// </summary>
        /// <param name="orderNumber">拣货单或者出库单</param>
        /// <returns></returns>
        [HttpPost, Route("GetVanningOperationDtoByOrder")]
        public VanningOperationDto GetVanningOperationDtoByOrder(string orderNumber, Guid wareHouseSysId)
        {
            return _vanningAppService.GetVanningOperationDtoByOrder(orderNumber, wareHouseSysId);
        }

        /// <summary>
        /// 装箱-封箱 提交数据
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("SaveVanningDetailOperationDto")]
        public VanningDetailDto SaveVanningDetailOperationDto(VanningOperationDto vanningOperationDto)
        {
            var vanningDetailOperationDto = vanningOperationDto.VanningDetailOperationDto;
            //try
            //{
              return  _vanningAppService.SaveVanningDetailOperationDto(vanningDetailOperationDto, vanningOperationDto.ActionType, vanningOperationDto.CurrentUserName, vanningOperationDto.CurrentUserId, vanningOperationDto.WarehouseSysId);
            //}
            //catch (DbEntityValidationException dbEx)
            //{
            //    throw new Exception(dbEx);
            //}
        
        }

        /// <summary>
        /// 交接单分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetHandoverGroupByPage")]
        public Pages<HandoverGroupDto> GetHandoverGroupByPage(HandoverGroupQuery request)
        {
            return _vanningAppService.GetHandoverGroupByPage(request);
        }

        /// <summary>
        /// 根据交接单号查询交接单
        /// </summary>
        /// <param name="HandoverGroupOrder"></param>
        /// <returns></returns>
        [HttpGet, Route("GetHandoverGroupByOrder")]
        public HandoverGroupDto GetHandoverGroupByOrder(string HandoverGroupOrder, Guid wareHouseSysId)
        {
            return _vanningAppService.GetHandoverGroupByOrder(HandoverGroupOrder, wareHouseSysId);
        }


        /// <summary>
        /// 获取装箱单明细
        /// </summary>
        /// <param name="vanningViewQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetVanningViewById")]
        public VanningViewDto GetVanningViewById(VanningViewQuery vanningViewQuery)
        {
            return _vanningAppService.GetVanningViewById(vanningViewQuery);
        }

        /// <summary>
        /// 根据装箱明细SysId获取装箱SysId
        /// </summary>
        /// <param name="vanningDetailSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetVanningSysIdByVanningDetailSysId")]
        public Guid? GetVanningSysIdByVanningDetailSysId(Guid vanningDetailSysId, Guid wareHouseSysId)
        {
            return _vanningAppService.GetVanningSysIdByVanningDetailSysId(vanningDetailSysId, wareHouseSysId);
        }

        /// <summary>
        /// 取消装箱
        /// </summary>
        /// <param name="vanningCancelDto"></param>
        /// <returns></returns>
        [HttpPost, Route("CancelVanning")]
        public CommonResponse CancelVanning(VanningCancelDto vanningCancelDto)
        {
            return _vanningAppService.CancelVanning(vanningCancelDto);
        }
    }
}