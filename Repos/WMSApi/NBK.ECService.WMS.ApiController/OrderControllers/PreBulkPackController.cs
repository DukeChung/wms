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
    [RoutePrefix("api/Order/PreBulkPack")]
    [AccessLog]
    public class PreBulkPackController : AbpApiController
    {
        private IPreBulkPackAppService _preBulkPackAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preBulkPackAppService"></param>
        public PreBulkPackController(IPreBulkPackAppService preBulkPackAppService)
        {
            _preBulkPackAppService = preBulkPackAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void PreBulkPackAPI() { }


        /// <summary>
        /// 增加散货预包装
        /// </summary>
        [HttpPost, Route("AddPreBulkPack")]
        public bool AddPreBulkPack(BatchPreBulkPackDto batchPreBulkPackDto)
        {
            return _preBulkPackAppService.AddPreBulkPack(batchPreBulkPackDto);
        }

        /// <summary>
        /// 分页查询 散货封装单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPreBulkPackByPage")]
        public Pages<PreBulkPackDto> GetPreBulkPackByPage(PreBulkPackQuery request)
        {
            return _preBulkPackAppService.GetPreBulkPackByPage(request);
        }

        /// <summary>
        /// 更新散货预包装
        /// </summary>
        [HttpPost, Route("UpdatePreBulkPack")]
        public void UpdatePreBulkPack(PreBulkPackDto request)
        {
            _preBulkPackAppService.UpdatePreBulkPack(request);
        }

        /// <summary>
        /// 获取封装单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPreBulkPackBySysId")]
        public PreBulkPackDto GetPreBulkPackBySysId(Guid sysId,Guid warehouseSysId)
        {
            return _preBulkPackAppService.GetPreBulkPackBySysId(sysId, warehouseSysId);
        }

        /// <summary>
        /// 删除封箱单商品
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("DeletePrebulkPackSkus")]
        public void DeletePrebulkPackSkus(List<Guid> request, Guid warehouseSysId)
        {
            _preBulkPackAppService.DeletePrebulkPackSkus(request, warehouseSysId);
        }

        /// <summary>
        /// 删除封箱单
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("DeletePrebulkPack")]
        public void DeletePrebulkPack(List<Guid> request, Guid warehouseSysId)
        {
            _preBulkPackAppService.DeletePrebulkPack(request, warehouseSysId);
        }


        /// <summary>
        /// 散货装导入
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost, Route("ImportPreBulkPack")]
        public void ImportPreBulkPack(PreBulkPackDto dto)
        {
            _preBulkPackAppService.ImportPreBulkPack(dto);
        }

        /// <summary>
        /// 根据出库单ID获取散货封箱单号
        /// </summary>
        /// <param name="sysId">出库单SYSID</param>
        [HttpGet, Route("GetPrebulkPackStorageCase")]
        public List<string> GetPrebulkPackStorageCase(Guid sysId,Guid warehouseSysId)
        {
            return _preBulkPackAppService.GetPrebulkPackStorageCase(sysId, warehouseSysId);
        }
    }
}
