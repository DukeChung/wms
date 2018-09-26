using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IPrePackAppService : IApplicationService
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
        /// 新增预包装明细
        /// </summary>
        /// <param name="prePackSkuDto"></param>
        bool SavePrePackSku(PrePackSkuDto prePackSkuDto);

        /// <summary>
        /// 获取预包装单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrePackSkuDto GetPrePackBySysId(Guid sysId,Guid warehouseSysId);

        /// <summary>
        /// 编辑预包装明细
        /// </summary>
        /// <param name="prePackSkuDto"></param>
        bool UpdatePrePackSku(PrePackSkuDto prePackSkuDto);

        /// <summary>
        /// 删除预包装
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        bool DeletePerPack(List<Guid> sysId, Guid warehouseSysId);

        /// <summary>
        /// 预包装导入
        /// </summary>
        /// <param name="prePackSkuDto"></param>
        /// <returns></returns>
        bool ImportPrePack(PrePackSkuDto prePackSkuDto);

        /// <summary>
        /// 判断预包装货位是否存在
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        bool IsStorageLoc(PrePackQuery query);

        /// <summary>
        /// 预包装复制
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        bool CopyPrePack(PrePackCopy query);
    }
}
