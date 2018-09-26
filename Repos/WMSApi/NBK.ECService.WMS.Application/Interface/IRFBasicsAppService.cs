using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IRFBasicsAppService : IApplicationService
    {
        /// <summary>
        /// 根据商品UPC获取商品List
        /// </summary>
        /// <param name="upc"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        List<SkuPackDto> GetSkuListByUPC(string upc, Guid warehouseSysId);

        /// <summary>
        /// 根据商品SYSID获取商品包装
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        RFPackDto GetPackBySkuSysId(Guid skuSysId, Guid warehouseSysId);

        /// <summary>
        /// 根据UPC获取包装信息
        /// </summary>
        /// <param name="upc"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        List<RFPackDto> GetPackListByUPC(string upc, Guid warehouseSysId);

        /// <summary>
        /// 更新商品包装
        /// </summary>
        /// <param name="packDto"></param>
        /// <returns></returns>
        RFCommResult UpdateSkuPack(RFSkuPackDto packDto);
    }
}
