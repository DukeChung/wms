using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.VASControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/VAS/QualityControl")]
    [AccessLog]
    public class QualityControlController : AbpApiController
    {
        IQualityControlAppService _qualityControlAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualityControlAppService"></param>
        public QualityControlController(IQualityControlAppService qualityControlAppService)
        {
            this._qualityControlAppService = qualityControlAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void QualityControlAPI() { }

        /// <summary>
        /// 获取质检单列表
        /// </summary>
        /// <param name="qualityControlQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetQualityControlList")]
        public Pages<QualityControlListDto> GetQualityControlList(QualityControlQuery qualityControlQuery)
        {
            return _qualityControlAppService.GetQualityControlList(qualityControlQuery);
        }

        /// <summary>
        /// 删除质检单
        /// </summary>
        /// <param name="sysIds"></param>
        [HttpPost, Route("DeleteQualityControl")]
        public void DeleteQualityControl(List<Guid> sysIds, Guid warehouseSysId)
        {
            _qualityControlAppService.DeleteQualityControl(sysIds, warehouseSysId);
        }

        /// <summary>
        /// 获取质检详情
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetQualityControlViewDto")]
        public QualityControlDto GetQualityControlViewDto(Guid sysId, Guid warehouseSysId)
        {
            return _qualityControlAppService.GetQualityControlViewDto(sysId, warehouseSysId);
        }

        /// <summary>
        /// 获取质检相关单据明细
        /// </summary>
        /// <param name="docDetailQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetDocDetails")]
        public Pages<DocDetailDto> GetDocDetails(DocDetailQuery docDetailQuery)
        {
            return _qualityControlAppService.GetDocDetails(docDetailQuery);
        }

        /// <summary>
        /// 获取质检明细
        /// </summary>
        /// <param name="qcDetailQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetQCDetails")]
        public Pages<QualityControlDetailDto> GetQCDetails(QCDetailQuery qcDetailQuery)
        {
            return _qualityControlAppService.GetQCDetails(qcDetailQuery);
        }

        /// <summary>
        /// 保存质检单
        /// </summary>
        /// <param name="saveQualityControlDto"></param>
        [HttpPost, Route("SaveQualityControl")]
        public void SaveQualityControl(SaveQualityControlDto saveQualityControlDto)
        {
            _qualityControlAppService.SaveQualityControl(saveQualityControlDto);
        }

        /// <summary>
        /// 质检完成
        /// </summary>
        /// <param name="finishQualityControlDto"></param>
        [HttpPost, Route("FinishQualityControl")]
        public void FinishQualityControl(FinishQualityControlDto finishQualityControlDto)
        {
            _qualityControlAppService.FinishQualityControl(finishQualityControlDto);
        }

        /// <summary>
        /// 获取生成损益单数据
        /// </summary>
        /// <param name="createAdjustmentDto"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAdjustmentDto")]
        public AdjustmentDto GetAdjustmentDto(CreateAdjustmentDto createAdjustmentDto)
        {
            return _qualityControlAppService.GetAdjustmentDto(createAdjustmentDto);
        }
    }
}
