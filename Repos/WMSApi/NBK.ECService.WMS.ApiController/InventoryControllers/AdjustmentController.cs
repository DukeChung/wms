using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.InventoryControllers
{
    /// <summary>
    /// 损益管理
    /// </summary>
    [RoutePrefix("api/Inventory/Adjustment")]
    [AccessLog]
    public class AdjustmentController : AbpApiController
    {
        IAdjustmentAppService _adjustmentAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        public AdjustmentController(IAdjustmentAppService adjustmentAppService)
        {
            _adjustmentAppService = adjustmentAppService;
        }
        [HttpGet]
        public void AdjustmentAPI()
        {
        }

        /// <summary>
        /// 分页查询损益单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAdjustmentListByPage")]
        public Pages<AdjustmentListDto> GetAdjustmentListByPage(AdjustmentQuery request)
        {
            return _adjustmentAppService.GetAdjustmentListByPage(request);
        }

        /// <summary>
        /// 查询损益明细
        /// </summary>
        /// <param name="sysid"></param>
        /// <returns></returns>
        [HttpGet, Route("GetAdjustmentBySysId")]
        public AdjustmentViewDto GetAdjustmentBySysId(Guid sysid,Guid warehouseSysId)
        {
            return _adjustmentAppService.GetAdjustmentBySysId(sysid, warehouseSysId);
        }

        /// <summary>
        /// 创建损益
        /// </summary>
        /// <param name="adjustment"></param>
        [HttpPost, Route("AddAdjustment")]
        public void AddAdjustment(AdjustmentDto adjustment)
        {
            _adjustmentAppService.AddAdjustment(adjustment);
        }

        /// <summary>
        /// 编辑损益
        /// </summary>
        /// <param name="adjustment"></param>
        [HttpPost, Route("UpdateAdjustment")]
        public void UpdateAdjustment(AdjustmentDto adjustment)
        {
            _adjustmentAppService.UpdateAdjustment(adjustment);
        }

        /// <summary>
        /// 删除损益单商品
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("DeleteAjustmentSkus")]
        public void DeleteAjustmentSkus(List<Guid> request, Guid warehouseSysId)
        {
            _adjustmentAppService.DeleteAjustmentSkus(request, warehouseSysId);
        }

        /// <summary>
        /// 查询商品库存信息
        /// </summary>
        /// <param name="skuQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSkuInventoryList")]
        public Pages<SkuInvLotLocLpnDto> GetSkuInventoryList(SkuInvLotLocLpnQuery skuQuery)
        {
            return _adjustmentAppService.GetSkuInventoryList(skuQuery);
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost, Route("Audit")]
        public void Audit(AdjustmentAuditDto dto)
        {
            try
            {
                _adjustmentAppService.Audit(dto);
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost, Route("Void")]
        public void Void(AdjustmentAuditDto dto)
        {
            _adjustmentAppService.Void(dto);
        }
    }
}
