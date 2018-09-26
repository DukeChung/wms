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
    /// 包装管理
    /// </summary>
    [RoutePrefix("api/BaseData/Package")]
    [AccessLog]
    public class PackageController : AbpApiController
    {
        IPackageAppService _packageAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        public PackageController(IPackageAppService packageAppService)
        {
            _packageAppService = packageAppService;
        }

        [HttpGet]
        public void PackageAPI() { }

        [HttpPost, Route("GetUOMList")]
        public Pages<UOMDto> GetUOMList(UOMQuery query)
        {
            return _packageAppService.GetUOMList(query);
        }

        [HttpGet, Route("GetUOMBySysId")]
        public UOMDto GetUOMBySysId(Guid sysId)
        {
            return _packageAppService.GetUOMBySysId(sysId);
        }

        [HttpPost, Route("UpdateUOM")]
        public void UpdateUOM(UOMDto uomDto)
        {
             _packageAppService.UpdateUOM(uomDto);
        }


        [HttpPost, Route("AddUOM")]
        public void AddUOM(UOMDto uom)
        {
            _packageAppService.AddUOM(uom);
        }

        [HttpGet, Route("DeleteUOM")]
        public void DeleteUOM(string sysIdList)
        {
            _packageAppService.DeleteUOM(sysIdList);
        }

        #region pack management

        [HttpPost, Route("GetPackList")]
        public Pages<PackDto> GetPackList(PackQuery query)
        {
            return _packageAppService.GetPackList(query);
        }

        [HttpGet, Route("GetPackBySysId")]
        public PackDto GetPackBySysId(Guid sysId)
        {
            return _packageAppService.GetPackBySysId(sysId);
        }

        [HttpPost, Route("UpdatePack")]
        public void UpdatePack(PackDto packDto)
        {
             _packageAppService.UpdatePack(packDto);
        }

        [HttpPost, Route("AddPack")]
        public void AddPack(PackDto packDto)
        {
            _packageAppService.AddPack(packDto);
        }

        [HttpGet, Route("DeletePack")]
        public void DeletePack(string sysIdList)
        {
            _packageAppService.DeletePack(sysIdList);
        }

        /// <summary>
        /// 获取下拉 批次模板
        /// </summary>
        /// <param name="lotCode"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSelectPack")]
        public List<SelectItem> GetSelectPack(string packCode = null)
        {
            return _packageAppService.GetSelectPack(packCode);
        }
        #endregion
    }
}
