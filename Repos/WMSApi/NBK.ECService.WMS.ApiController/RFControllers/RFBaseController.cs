using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Core.WebApi.Filters;

namespace NBK.ECService.WMS.ApiController.RFControllers
{
    /// <summary>
    /// RF基础资料
    /// </summary>
    [RoutePrefix("api/Base")]
    [AccessLog]
    public class RFBaseController : AbpApiController
    {
        ILocationAppService _locationAppService;
        ISkuAppService _skuAppService;
        IRFOutboundAppService _rfOutboundAppService;
        IRFBasicsAppService _rfBasicsAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        public RFBaseController(ILocationAppService locationAppService, ISkuAppService skuAppService, IRFOutboundAppService rfOutboundAppService, IRFBasicsAppService rfBasicsAppService)
        {
            _locationAppService = locationAppService;
            _skuAppService = skuAppService;
            _rfOutboundAppService = rfOutboundAppService;
            _rfBasicsAppService = rfBasicsAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void RFBaseAPI()
        {
        }

        /// <summary>
        /// 判断货位是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("LocIsExist")]
        public RFCommResult LocIsExist(LocationQuery request)
        {
            var result = new RFCommResult();
            if (_locationAppService.GetLocationByLoc(request.LocationSearch, request.WarehouseSysId) != null)
            {
                result.IsSucess = true;
            }
            else
            {
                result.IsSucess = false;
                result.Message = "库位不存在";
            }
            return result;
        }

        /// <summary>
        /// 判断商品是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckSkuIsExist")]
        public RFCommResult CheckSkuIsExist(SkuQuery request)
        {
            var result = new RFCommResult();
            if (_skuAppService.GetSkuByUPC(request.UPC, request.WarehouseSysId) != null)
            {
                result.IsSucess = true;
            }
            else
            {
                result.IsSucess = false;
                result.Message = "商品不存在";
            }
            return result;
        }

        /// <summary>
        /// 通过UPC获取SKU
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSkuByUPC")]
        public SkuDto GetSkuByUPC(string upc)
        {
            return _skuAppService.GetSkuByUPC(upc);
        }

        /// <summary>
        /// 根据UPC获取商品和包装
        /// </summary>
        /// <param name="upc"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSkuPackListByUPC")]
        public List<SkuPackDto> GetSkuPackListByUPC(string upc, Guid warehouseSysId)
        {
            return _rfOutboundAppService.GetSkuPackListByUPC(upc, warehouseSysId);
        }


        #region 包装管理
        /// <summary>
        /// 根据UPC获取商品
        /// </summary>
        /// <param name="upc"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSkuListByUPC")]
        public List<SkuPackDto> GetSkuListByUPC(string upc, Guid warehouseSysId)
        {
            return _rfBasicsAppService.GetSkuListByUPC(upc, warehouseSysId);
        }

        /// <summary>
        /// 根据商品SYSID获取商品包装
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPackBySkuSysId")]
        public RFPackDto GetPackBySkuSysId(Guid skuSysId, Guid warehouseSysId)
        {
            return _rfBasicsAppService.GetPackBySkuSysId(skuSysId, warehouseSysId);
        }


        /// <summary>
        /// 根据UPC获取包装信息
        /// </summary>
        /// <param name="upc"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPackListByUPC")]
        public List<RFPackDto> GetPackListByUPC(string upc, Guid warehouseSysId)
        {
            return _rfBasicsAppService.GetPackListByUPC(upc, warehouseSysId);
        }

        /// <summary>
        /// 更新商品外包装
        /// </summary>
        /// <param name="packDto"></param>
        [HttpPost, Route("UpdateSkuPack")]
        public RFCommResult UpdateSkuPack(RFSkuPackDto packDto)
        {
            return _rfBasicsAppService.UpdateSkuPack(packDto);
        }
        #endregion

    }
}
