using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface ISkuAppService : IApplicationService
    {
        /// <summary>
        /// 获取SKU列表
        /// </summary>
        /// <param name="skuQuery"></param>
        /// <returns></returns>
        Pages<SkuListDto> GetSkuList(SkuQuery skuQuery);

        /// <summary>
        /// 新增SKU
        /// </summary>
        /// <param name="skuDto"></param>
        /// <returns></returns>
        void AddSku(SkuDto skuDto);

        /// <summary>
        /// 根据Id获取SKU
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        SkuDto GetSkuById(Guid sysId);

        /// <summary>
        /// 编辑SKU
        /// </summary>
        /// <param name="skuDto"></param>
        void EditSku(SkuDto skuDto);

        /// <summary>
        /// 删除SKU
        /// </summary>
        /// <param name="sysIds"></param>
        void DeleteSku(List<Guid> sysIds);

        /// <summary>
        /// 根据商品条码获取商品数据
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        SkuDto GetSkuByUPC(string upc);

        /// <summary>
        /// 根据商品条码获取商品数据
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        SkuDto GetSkuByUPC(string upc, Guid warehouseSysId);

        List<SkuWithPackDto> GetSkuAndSkuPackListByUPC(DuplicateUPCChooseQuery request);
    }
}
