using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IRFBasicsRepository
    {
        /// <summary>
        /// 根据UPC获取商品
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        List<SkuPackDto> GetSkuListByUPC(string upc);

        /// <summary>
        /// 根据skusysId获取包装信息
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <returns></returns>
        RFPackDto GetPackBySkuSysId(Guid skuSysId);

        /// <summary>
        /// 根据UPC获取包装信息
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        List<RFPackDto> GetPackListByUPC(string upc);
    }
}
