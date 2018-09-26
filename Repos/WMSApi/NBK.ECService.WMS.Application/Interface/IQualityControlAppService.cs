using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IQualityControlAppService : IApplicationService
    {
        /// <summary>
        /// 获取质检单列表
        /// </summary>
        /// <param name="qualityControlQuery"></param>
        /// <returns></returns>
        Pages<QualityControlListDto> GetQualityControlList(QualityControlQuery qualityControlQuery);

        /// <summary>
        /// 删除质检单
        /// </summary>
        /// <param name="sysIds"></param>
        void DeleteQualityControl(List<Guid> sysIds, Guid warehouseSysId);

        /// <summary>
        /// 获取质检详情
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        QualityControlDto GetQualityControlViewDto(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 获取质检相关单据明细
        /// </summary>
        /// <param name="qcType"></param>
        /// <param name="docOrder"></param>
        /// <returns></returns>
        Pages<DocDetailDto> GetDocDetails(DocDetailQuery docDetailQuery);

        /// <summary>
        /// 获取质检明细
        /// </summary>
        /// <param name="qcSysId"></param>
        /// <returns></returns>
        Pages<QualityControlDetailDto> GetQCDetails(QCDetailQuery qcDetailQuery);

        /// <summary>
        /// 保存质检单
        /// </summary>
        /// <param name="saveQualityControlDto"></param>
        void SaveQualityControl(SaveQualityControlDto saveQualityControlDto);

        /// <summary>
        /// 质检完成
        /// </summary>
        /// <param name="finishQualityControlDto"></param>
        void FinishQualityControl(FinishQualityControlDto finishQualityControlDto);

        /// <summary>
        /// 获取生成损益单数据
        /// </summary>
        /// <param name="createAdjustmentDto"></param>
        /// <returns></returns>
        AdjustmentDto GetAdjustmentDto(CreateAdjustmentDto createAdjustmentDto);
    }
}
