using Abp.WebApi.Controllers;
using FortuneLab.Models;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.RFControllers
{
    [RoutePrefix("api/RFReceipt")]
    [AccessLog]
    public class RFReceiptController : AbpApiController
    {
        IRFReceiptAppService _rfReceiptAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rfReceiptAppService"></param>
        public RFReceiptController(IRFReceiptAppService rfReceiptAppService)
        {
            _rfReceiptAppService = rfReceiptAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void RFReceiptAPI()
        {
        }

        /// <summary>
        /// 待收货列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingReceiptListByPaging")]
        public Page<RFReceiptListDto> GetWaitingReceiptListByPaging(RFReceiptQuery receiptQuery)
        {
            return _rfReceiptAppService.GetWaitingReceiptListByPaging(receiptQuery);
        }

        /// <summary>
        /// 检查收货单是否能收货
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckReceipt")]
        public RFCommResult CheckReceipt(RFReceiptQuery receiptQuery)
        {
            return _rfReceiptAppService.CheckReceipt(receiptQuery);
        }

        /// <summary>
        /// 获取入库单收货明细
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptOperationDetailList")]
        public List<RFReceiptOperationDetailListDto> GetReceiptOperationDetailList(RFReceiptQuery receiptQuery)
        {
            return _rfReceiptAppService.GetReceiptOperationDetailList(receiptQuery);
        }

        /// <summary>
        /// 收货完成
        /// </summary>
        /// <param name="receiptOperationDto"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveReceiptOperation")]
        public RFCommResult SaveReceiptOperation(ReceiptOperationDto receiptOperationDto)
        {
            return _rfReceiptAppService.SaveReceiptOperation(receiptOperationDto);
        }
    }
}
