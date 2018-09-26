using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IPrePackCrudRepository:ICrudRepository
    {
        /// <summary>
        /// 分页获取与包装单
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        Pages<PrePackListDto> GetPrePackByPage(PrePackQuery prePackQuery);

        /// <summary>
        /// 获取预包装库存
        /// </summary>
        /// <param name="prePackSkuQuery"></param>
        /// <returns></returns>
        Pages<PrePackSkuListDto> GetPrePackSkuByPage(PrePackSkuQuery prePackSkuQuery);

        /// <summary>
        /// 获取预包装单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrePackSkuDto GetPrePackBySysId(Guid sysId);

        /// <summary>
        /// 获取预包装单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        List<PrePackDetailDto> GetPrePackDetailBySysId(Guid sysId,string batchNumber);

        /// <summary>
        /// 通过出库单获取预包装明细
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        List<PrePackDetailDto> GetPrePackDetailByOutboundSysId(Guid outboundSysId);

        /// <summary>
        /// 删除预包装
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        bool DeletePrePack(List<Guid> sysId);
    }
}
