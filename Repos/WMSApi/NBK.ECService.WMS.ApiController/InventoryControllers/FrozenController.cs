using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.InventoryControllers
{
    [RoutePrefix("api/Inventory/Frozen")]
    [AccessLog]
    public class FrozenController : AbpApiController
    {
        private IFrozenAppService _frozenAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationAppService"></param>
        public FrozenController(IFrozenAppService frozenAppService)
        {
            this._frozenAppService = frozenAppService;
        }

        /// <summary>
        /// Location
        /// </summary>
        [HttpGet]
        public void FrozenApi() { }

        /// <summary>
        /// 查询冻结请求的商品展示列表数据源
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetFrozenRequestSkuByPage")]
        public Pages<FrozenRequestSkuDto> GetFrozenRequestSkuByPage(FrozenRequestQuery request)
        {
            return _frozenAppService.GetFrozenRequestSkuByPage(request);
        }

        /// <summary>
        /// 冻结库存操作
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("SaveFrozenRequest")]
        public void SaveFrozenRequest(FrozenRequestDto request)
        {
            try
            {
                _frozenAppService.SaveFrozenRequest(request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("冻结失败，失败原因：数据重复并发提交");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 冻结记录查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetFrozenRequestList")]
        public Pages<FrozenListDto> GetFrozenRequestList(FrozenListQuery request)
        {
            return _frozenAppService.GetFrozenRequestList(request);
        }

        /// <summary>
        /// 解冻
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("UnFrozenRequest")]
        public void UnFrozenRequest(FrozenRequestDto request)
        {
            try
            {
                _frozenAppService.UnFrozenRequest(request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("解冻失败，失败原因：数据重复并发提交");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查询冻结货位或者储区 商品明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetFrozenDetailByPage")]
        public Pages<FrozenRequestSkuDto> GetFrozenDetailByPage(FrozenRequestQuery request)
        {
            return _frozenAppService.GetFrozenDetailByPage(request);
        }
    }
}
