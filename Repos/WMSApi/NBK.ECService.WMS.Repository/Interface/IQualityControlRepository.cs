using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IQualityControlRepository : ICrudRepository
    {
        /// <summary>
        /// 获取质检单列表
        /// </summary>
        /// <param name="qualityControlQuery"></param>
        /// <returns></returns>
        Pages<QualityControlListDto> GetQualityControlList(QualityControlQuery qualityControlQuery);

        /// <summary>
        /// 获取质检相关单据明细
        /// </summary>
        /// <param name="qcType"></param>
        /// <param name="docOrder"></param>
        /// <returns></returns>
        Pages<DocDetailDto> GetDocDetails(DocDetailQuery docDetailQuery);

        List<DocDetailDto> GetDocDetails(string docOrder, int qcType, List<Guid> skuSysIds);

        /// <summary>
        /// 获取质检明细
        /// </summary>
        /// <param name="qcSysId"></param>
        /// <returns></returns>
        Pages<QualityControlDetailDto> GetQCDetails(QCDetailQuery qcDetailQuery);
    }
}
