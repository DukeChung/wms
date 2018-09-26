using System;
using System.Collections.Generic;
using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Core.WebApi.Filters;
using System.Data.Entity.Infrastructure;

namespace NBK.ECService.WMS.ApiController.OutboundControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Outbound/PickDetail")]
    [AccessLog]
    public class PickDetailController : AbpApiController
    {
        private IPickDetailAppService _pickDetailAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        /// <param name="pickDetailAppService"></param>
        public PickDetailController(IPickDetailAppService pickDetailAppService)
        {
            _pickDetailAppService = pickDetailAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void PickDetailAPI()
        {
        }

        /// <summary>
        /// 获取分页信息
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPickDetailListDto")]
        public Pages<PickDetailListDto> GetPickDetailListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            return _pickDetailAppService.GetPickDetailListDtoByPageInfo(pickDetailQuery);
        }

        /// <summary>
        /// 获取分页信息
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPickOutboundListDto")]
        public Pages<PickOutboundListDto> GetPickOutboundListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            return _pickDetailAppService.GetPickOutboundListDtoByPageInfo(pickDetailQuery);
        }

        /// <summary>
        /// 根据拣货规则生成拣货单
        /// </summary>
        /// <param name="createPickDetailRuleDto"></param>
        /// <returns></returns>
        [HttpPost, Route("GeneratePickDetailByPickRule")]
        public List<string> GeneratePickDetailByPickRule(CreatePickDetailRuleDto createPickDetailRuleDto)
        {
            try
            {
                return _pickDetailAppService.GeneratePickDetailByPickRule(createPickDetailRuleDto);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("操作失败");
            }
        }

        /// <summary>
        /// 取消拣货
        /// </summary>
        /// <param name="cancelPickDetailDto"></param>
        /// <returns></returns>
        [HttpPost, Route("CancelPickDetail")]
        public string CancelPickDetail(CancelPickDetailDto cancelPickDetailDto)
        {
            return _pickDetailAppService.CancelPickDetail(cancelPickDetailDto);
        }

        /// <summary>
        /// 取消拣货数量
        /// </summary>
        /// <param name="cancelPickQtyDto"></param>
        [HttpPost, Route("CancelPickQty")]
        public void CancelPickQty(CancelPickQtyDto cancelPickQtyDto)
        {
            _pickDetailAppService.CancelPickQty(cancelPickQtyDto);
        }

        /// <summary>
        /// 拣货
        /// </summary>
        /// <param name="pickingOperationDto"></param>
        [HttpPost, Route("SavePickingOperation")]
        public void SavePickingOperation(PickingOperationDto pickingOperationDto)
        {
            _pickDetailAppService.SavePickingOperation(pickingOperationDto);
        }

        /// <summary>
        /// 获取拣货单明细
        /// </summary>
        /// <param name="pickingOperationQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPickingOperationDetails")]
        public List<PickingOperationDetail> GetPickingOperationDetails(PickingOperationQuery pickingOperationQuery)
        {
            return _pickDetailAppService.GetPickingOperationDetails(pickingOperationQuery);
        }
    }
}