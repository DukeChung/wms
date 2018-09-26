using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.ThirdParty;
using NBK.ECService.WMS.DTO.ThirdParty.ZTO;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.VASControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/VAS/SkuBorrow")]
    [AccessLog]
    public class SkuBorrowController : AbpApiController
    {

        ISkuBorrowAppService _skuborrowAppService; 
        IZTOAppService _IZTOAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skuborrowAppService"></param>
        /// <param name="ztoTOAppService"></param>
        public SkuBorrowController(ISkuBorrowAppService skuborrowAppService, IZTOAppService ztoTOAppService)
        {
            _skuborrowAppService = skuborrowAppService;
            _IZTOAppService = ztoTOAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void SkuBorrowAPI() { }

        /// <summary>
        /// 分页查询商品外借
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSkuBorrowListByPage")]
        public Pages<SkuBorrowListDto> GetSkuBorrowListByPage(SkuBorrowQuery request)
        {
            return _skuborrowAppService.GetSkuBorrowListByPage(request);
        }

        /// <summary>
        /// 查询商品库存信息
        /// </summary>
        /// <param name="skuQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSkuInventoryList")]
        public Pages<SkuInvLotLocLpnDto> GetSkuInventoryList(SkuInvLotLocLpnQuery skuQuery)
        {
            return _skuborrowAppService.GetSkuInventoryList(skuQuery);
        }


        /// <summary>
        /// 创建商品外借
        /// </summary>
        /// <param name="skuborrow"></param>
        [HttpPost, Route("AddSkuBorrow")]
        public void AddSkuBorrow(SkuBorrowDto skuborrow)
        {
            _skuborrowAppService.AddSkuBorrow(skuborrow);
        }

        /// <summary>
        /// 查询商品外借
        /// </summary>
        /// <param name="SysId"></param>
        /// <param name="WareHouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSkuBorrowBySysId")]
        public SkuBorrowViewDto GetSkuBorrowBySysId(Guid SysId, Guid WareHouseSysId)
        {
            return _skuborrowAppService.GetSkuBorrowBySysId(SysId, WareHouseSysId);
        }

        /// <summary>
        /// 查询商品外借
        /// </summary>
        /// <param name="BorrowOrder"></param>
        /// <param name="WareHouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSkuBorrowByOrder")]
        public SkuBorrowViewDto GetSkuBorrowByOrder(string BorrowOrder, Guid WareHouseSysId)
        {
            return _skuborrowAppService.GetSkuBorrowByOrder(BorrowOrder, WareHouseSysId);
        }

        /// <summary>
        /// 编辑商品外借
        /// </summary>
        /// <param name="skuborrow"></param>
        [HttpPost, Route("UpdateSkuBorrow")]
        public void UpdateSkuBorrow(SkuBorrowDto skuborrow)
        {
            _skuborrowAppService.UpdateSkuBorrow(skuborrow);
        }

        /// <summary>
        /// 删除商品外界商品
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("DeleteSkuBorrowSkus")]
        public void DeleteSkuBorrowSkus(List<Guid> request)
        {
            _skuborrowAppService.DeleteSkuBorrowSkus(request);
        } 

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost, Route("Audit")]
        public void Audit(SkuBorrowDto dto)
        {
            try
            {

                _skuborrowAppService.Audit(dto);
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost, Route("Void")]
        public void Void(SkuBorrowAuditDto dto)
        {
            _skuborrowAppService.Void(dto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost,Route("OrderMarke")]
        public GenerateZTOOrderMarkeResponse OrderMarke(GenerateZTOOrderMarkeRequest request)
        {
            return _IZTOAppService.OrderMarke(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("OrderSubmit")]
        public GenerateZTOOrderSubmitResponse OrderSubmit(GenerateZTOOrderSubmitRequest request)
        {
            return _IZTOAppService.OrderSubmit(request);
        }

    }
}
