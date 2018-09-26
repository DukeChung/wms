using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.BaseDataControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Carrier")]
    [AccessLog]
    public class CarrierController : AbpApiController
    {
        private ICarrierAppService _carrierAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="carrierAppService"></param>
        public CarrierController(ICarrierAppService carrierAppService)
        {
            this._carrierAppService = carrierAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void CarrierApi() { }

        /// <summary>
        /// 获取承运商列表
        /// </summary>
        /// <param name="carrierQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetCarrierList")]
        public Pages<CarrierDto> GetCarrierList(CarrierQuery carrierQuery)
        {
            return _carrierAppService.GetCarrierList(carrierQuery);
        }

        /// <summary>
        /// 新增承运商
        /// </summary>
        /// <param name="carrierDto"></param>
        /// <returns></returns>
        [HttpPost, Route("AddCarrier")]
        public void AddCarrier(CarrierDto carrierDto)
        {
            _carrierAppService.AddCarrier(carrierDto);
        }

        /// <summary>
        /// 根据Id获取承运商
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetCarrierById")]
        public CarrierDto GetCarrierById(Guid sysId)
        {
            return _carrierAppService.GetCarrierById(sysId);
        }

        /// <summary>
        /// 编辑承运商
        /// </summary>
        /// <param name="carrierDto"></param>
        [HttpPost, Route("EditCarrier")]
        public void EditCarrier(CarrierDto carrierDto)
        {
            _carrierAppService.EditCarrier(carrierDto);
        }

        /// <summary>
        /// 删除承运商
        /// </summary>
        /// <param name="sysIds"></param>
        [HttpPost, Route("DeleteCarrier")]
        public void DeleteCarrier(List<Guid> sysIds)
        {
            _carrierAppService.DeleteCarrier(sysIds);
        }

        /// <summary>
        /// 获取可用承运商
        /// </summary>
        /// <param name=""></param>
        [HttpGet, Route("GetExpressListByIsActive")]
        public List<CarrierDto> GetExpressListByIsActive()
        {
           return  _carrierAppService.GetExpressListByIsActive();
        }
    }
}
