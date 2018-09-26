using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using System.Web.Http;
using NBK.ECService.WMS.DTO;
using System.Collections.Generic;
using NBK.ECService.WMS.Core.WebApi.Filters;

namespace NBK.ECService.WMS.ApiController.BaseDataControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Base")]
    [AccessLog]
    public class BaseController : AbpApiController
    {
        private IBaseAppService _baseAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseAppService"></param>
        public BaseController(IBaseAppService baseAppService)
        {
            this._baseAppService = baseAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void BaseApi() { }

        /// <summary>
        /// 生成单号  
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpPost, Route("GenNextNumber")]
        public List<string> GenNextNumber(string tableName, int count)
        {
            return _baseAppService.GenNextNumber(tableName, count);
        }

        /// <summary>
        /// 获取坐标
        /// </summary>
        /// <param name="city"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        [HttpGet, Route("GetCoordinate")]
        public CoordinateDto GetCoordinate(string city, string address)
        {
            return _baseAppService.GetCoordinate(city, address);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <returns></returns>
        public CommonResponse PushOutboundECC(string outboundOrder)
        {
            return _baseAppService.PushOutboundECC(outboundOrder);
        }

        /// <summary>
        /// 测试连接是否成功
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("TestConnection")]
        public bool TestConnection()
        {
            return true;
        }
    }
}
