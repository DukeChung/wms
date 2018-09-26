using System.Web.Http;
using Abp.WebApi.Controllers;
using FortuneLab.ECService.Securities.Filters;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Core.WebApi.Filters;

namespace NBK.ECService.WMS.ApiController.BaseDataControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Sku")]
    [AccessLog]
    public class SkuController : AbpApiController
    {
        private ISkuAppService _skuAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skuAppService"></param>
        public SkuController(ISkuAppService skuAppService)
        {
            this._skuAppService = skuAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void SkuApi() { }

        /// <summary>
        /// 获取SKU列表
        /// </summary>
        /// <param name="skuQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSkuList")]
        public Pages<SkuListDto> GetSkuList(SkuQuery skuQuery)
        {
            return _skuAppService.GetSkuList(skuQuery);
        }

        /// <summary>
        /// 新增SKU
        /// </summary>
        /// <param name="skuDto"></param>
        /// <returns></returns>
        [HttpPost, Route("AddSku")]
        public void AddSku(SkuDto skuDto)
        {
            _skuAppService.AddSku(skuDto);
        }

        /// <summary>
        /// 根据Id获取SKU
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSkuById")]
        public SkuDto GetSkuById(Guid sysId)
        {
            return _skuAppService.GetSkuById(sysId);
        }

        /// <summary>
        /// 编辑SKU
        /// </summary>
        /// <param name="skuDto"></param>
        [HttpPost, Route("EditSku")]
        public void EditSku(SkuDto skuDto)
        {
            _skuAppService.EditSku(skuDto);
        }

        /// <summary>
        /// 删除SKU
        /// </summary>
        /// <param name="sysIds"></param>
        [HttpPost, Route("DeleteSku")]
        public void DeleteSku(List<Guid> sysIds)
        {
            _skuAppService.DeleteSku(sysIds);
        }

        /// <summary>
        /// 根据UPC 获取 商品UPC 已经 包装UPC对应的商品信息集合
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSkuAndSkuPackListByUPC")]
        public List<SkuWithPackDto> GetSkuAndSkuPackListByUPC(DuplicateUPCChooseQuery request)
        {
            return _skuAppService.GetSkuAndSkuPackListByUPC(request);
        }
    }
}