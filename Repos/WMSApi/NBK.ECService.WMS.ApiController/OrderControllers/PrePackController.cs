using Abp.WebApi.Controllers;
using FortuneLab.WebApiClient;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.OrderControllers
{
    /// <summary>
    /// 订单管理_移仓单管理
    /// </summary>
    [RoutePrefix("api/Order/PrePack")]
    [AccessLog]
    public class PrePackController : AbpApiController
    {

        private IPrePackAppService _prePackAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prePackAppService"></param>
        public PrePackController(IPrePackAppService prePackAppService)
        {
            _prePackAppService = prePackAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void PrePackAPI() { }


        /// <summary>
        /// 分页获取预包装单数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPrePackByPage")]
        public Pages<PrePackListDto> GetPrePackByPage(PrePackQuery request)
        {
            return _prePackAppService.GetPrePackByPage(request);
        }

        /// <summary>
        /// 分页获取预包装库存数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPrePackSkuByPage")]
        public Pages<PrePackSkuListDto> GetPrePackSkuByPage(PrePackSkuQuery request)
        {
            return _prePackAppService.GetPrePackSkuByPage(request);
        }

        /// <summary>
        /// 生成预包装单
        /// </summary>
        [HttpPost, Route("SavePrePackSku")]

        public void SavePrePackSku(PrePackSkuDto prePackSkuDto)
        {
            _prePackAppService.SavePrePackSku(prePackSkuDto);
        }

        /// <summary>
        /// 获取预包装单明细
        /// </summary>
        [HttpPost, Route("GetPrePackBySysId")]

        public PrePackSkuDto GetPrePackBySysId(Guid sysId, Guid warehouseSysId)
        {
            return _prePackAppService.GetPrePackBySysId(sysId, warehouseSysId);
        }


        /// <summary>
        /// 编辑预包装单和明细
        /// </summary>
        [HttpPost, Route("UpdatePrePackSku")]

        public void UpdatePrePackSku(PrePackSkuDto prePackSkuDto)
        {
            _prePackAppService.UpdatePrePackSku(prePackSkuDto);
        }

        /// <summary>
        /// 删除预包装
        /// </summary>
        [HttpPost, Route("DeletePerPack")]

        public void DeletePerPack(List<Guid> sysId, Guid warehouseSysId)
        {
            _prePackAppService.DeletePerPack(sysId, warehouseSysId);
        }

        /// <summary>
        /// 预包装导入
        /// </summary>
        /// <param name="prePackSkuDto"></param>
        [HttpPost, Route("ImportPrePack")]
        public void ImportPrePack(PrePackSkuDto prePackSkuDto)
        {
            _prePackAppService.ImportPrePack(prePackSkuDto);
        }

        /// <summary>
        /// 判断预包装货位是否存在
        /// </summary>
        /// <param name="query"></param>
        [HttpPost, Route("IsStorageLoc")]
        public bool IsStorageLoc(PrePackQuery query)
        {
            return _prePackAppService.IsStorageLoc(query);
        }
        /// <summary>
        /// 预报装复制
        /// </summary>
        /// <param name="query"></param>
        [HttpPost, Route("CopyPrePack")]
        public bool CopyPrePack(PrePackCopy query)
        {
            return _prePackAppService.CopyPrePack(query);
        }

    }
}
