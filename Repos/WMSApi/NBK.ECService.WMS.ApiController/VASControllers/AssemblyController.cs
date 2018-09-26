using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.VASControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/VAS/Assembly")]
    [AccessLog]
    public class AssemblyController : AbpApiController
    {
        IAssemblyAppService _assemblyAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyAppService"></param>
        public AssemblyController(IAssemblyAppService assemblyAppService)
        {
            _assemblyAppService = assemblyAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void AssemblyAPI() { }

        /// <summary>
        /// 获取生产加工单列表
        /// </summary>
        /// <param name="assemblyQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAssemblyList")]
        public Pages<AssemblyListDto> GetAssemblyList(AssemblyQuery assemblyQuery)
        {
            return _assemblyAppService.GetAssemblyList(assemblyQuery);
        }

        /// <summary>
        /// 获取生产加工单详情
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetAssemblyViewDtoById")]
        public AssemblyViewDto GetAssemblyViewDtoById(Guid sysId, Guid warehouseSysId)
        {
            return _assemblyAppService.GetAssemblyViewDtoById(sysId, warehouseSysId);
        }

        /// <summary>
        /// 更新生产加工单状态
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="status"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentUserName"></param>
        [HttpGet, Route("UpdateAssemblyStatus")]
        public void UpdateAssemblyStatus(Guid sysId, AssemblyStatus status, int currentUserId, string currentUserName, Guid warehouseSysId)
        {
            _assemblyAppService.UpdateAssemblyStatus(sysId, status, currentUserId, currentUserName, warehouseSysId);
        }

        /// <summary>
        /// 撤销领料
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentUserName"></param>
        [HttpGet, Route("CancelAssemblyPicking")]
        public void CancelAssemblyPicking(Guid sysId, int currentUserId, string currentUserName, Guid warehouseSysId)
        {
            _assemblyAppService.CancelAssemblyPicking(sysId, currentUserId, currentUserName, warehouseSysId);
        }

        /// <summary>
        /// 完成加工单
        /// </summary>
        /// <param name="assemblyFinishDto"></param>
        [HttpPost, Route("FinishAssemblyOrder")]
        public void FinishAssemblyOrder(AssemblyFinishDto assemblyFinishDto)
        {
            _assemblyAppService.FinishAssemblyOrder(assemblyFinishDto);
        }

        /// <summary>
        /// 创建加工单商品选择列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSkuListForAssembly")]
        public Pages<AssemblySkuDto> GetSkuListForAssembly(AssemblySkuQuery query)
        {
            return _assemblyAppService.GetSkuListForAssembly(query);
        }

        /// <summary>
        /// WMS创建加工单
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("AddAssembly")]
        public void AddAssembly(AddAssemblyDto request)
        {
            _assemblyAppService.AddAssembly(request);
        }

        /// <summary>
        /// 查询加工源商品 已称重列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWeighSkuListForAssembly")]
        public Pages<AssemblyWeightSkuDto> GetWeighSkuListForAssembly(AssemblyWeightSkuQuery request)
        {
            return _assemblyAppService.GetWeighSkuListForAssembly(request);
        }

        /// <summary>
        /// 保存加工称重
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("SaveAssemblySkuWeight")]
        public void SaveAssemblySkuWeight(AssemblyWeightSkuRequest request)
        {
            _assemblyAppService.SaveAssemblySkuWeight(request);
        }
    }
}
