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
    [RoutePrefix("api/Vendor")]
    [AccessLog]
    public class VendorController : AbpApiController
    {
        private IVendorAppService _vendorAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vendorAppService"></param>
        public VendorController(IVendorAppService vendorAppService)
        {
            this._vendorAppService = vendorAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void VendorApi() { }

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetSelectVendor")]
        public List<SelectItem> GetSelectVendor()
        {
            return _vendorAppService.GetSelectVendor();
        }
    }
}
