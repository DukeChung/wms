using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IPackageRepository : ICrudRepository
    {
        Pages<UOMDto> GetUOMList(UOMQuery query);

        /// <summary>
        /// 根据商品Id查询所有包装信息
        /// 获取商品单位转换后数量, e.g: 将1 公斤 转换为 1000 g 作为库存保存
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <returns></returns>
        List<SkuPackageConvertDto> GetSkuPackageList(List<Guid> skuSysId);
    }
}
