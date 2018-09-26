using FortuneLab.Models;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class RFReceiptAppService : WMSApplicationService, IRFReceiptAppService
    {
        private ICrudRepository _crudRepository = null;
        private IRFReceiptRepository _rfReceiptRepository = null;
        private IReceiptAppService _receiptAppService = null;

        public RFReceiptAppService(ICrudRepository crudRepository, IRFReceiptRepository rfReceiptRepository, IReceiptAppService receiptAppService)
        {
            this._crudRepository = crudRepository;
            this._rfReceiptRepository = rfReceiptRepository;
            this._receiptAppService = receiptAppService;
        }


        /// <summary>
        /// 待收货列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public Page<RFReceiptListDto> GetWaitingReceiptListByPaging(RFReceiptQuery receiptQuery)
        {
            _crudRepository.ChangeDB(receiptQuery.WarehouseSysId);
            return _rfReceiptRepository.GetWaitingReceiptListByPaging(receiptQuery);
        }

        /// <summary>
        /// 检查收货单是否能收货
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public RFCommResult CheckReceipt(RFReceiptQuery receiptQuery)
        {
            _crudRepository.ChangeDB(receiptQuery.WarehouseSysId);
            RFCommResult rsp = new RFCommResult();
            var receipt = _crudRepository.GetQuery<receipt>(p => p.ReceiptOrder == receiptQuery.ReceiptOrder && p.WarehouseSysId == receiptQuery.WarehouseSysId).FirstOrDefault();
            if (receipt == null)
            {
                rsp.Message = "收货单不存在";
                return rsp;
            }
            if (receipt.Status == (int)ReceiptStatus.Received || receipt.Status == (int)ReceiptStatus.Cancel)
            {
                rsp.Message = $"收货单状态为{((ReceiptStatus)receipt.Status).ToDescription()}，无法收货";
                return rsp;
            }
            var purchase = _crudRepository.GetQuery<purchase>(p => p.PurchaseOrder == receipt.ExternalOrder && p.WarehouseSysId == receiptQuery.WarehouseSysId).FirstOrDefault();
            if (purchase == null)
            {
                rsp.Message = "入库单不存在";
                return rsp;
            }
            if (purchase.Status == (int)PurchaseStatus.Finish || purchase.Status == (int)PurchaseStatus.Close || purchase.Status == (int)PurchaseStatus.StopReceipt || purchase.Status == (int)PurchaseStatus.Void)
            {
                rsp.Message = $"入库单状态为{((PurchaseStatus)purchase.Status).ToDescription()}，无法收货";
                return rsp;
            }
            var skuSysIds = _crudRepository.GetQuery<purchasedetail>(p => p.PurchaseSysId == purchase.SysId).Select(p => p.SkuSysId).ToList();
            var materialSku = _crudRepository.GetQuery<sku>(p => skuSysIds.Contains(p.SysId) && p.IsMaterial == true).FirstOrDefault();
            if (materialSku != null)
            {
                rsp.Message = $"暂不支持原材料商品收货";
                return rsp;
            }

            rsp.IsSucess = true;
            return rsp;
        }

        /// <summary>
        /// 获取入库单收货明细
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public List<RFReceiptOperationDetailListDto> GetReceiptOperationDetailList(RFReceiptQuery receiptQuery)
        {
            _crudRepository.ChangeDB(receiptQuery.WarehouseSysId);
            return _rfReceiptRepository.GetReceiptOperationDetailList(receiptQuery);
        }

        /// <summary>
        /// 收货完成
        /// </summary>
        /// <param name="receiptOperationDto"></param>
        /// <returns></returns>
        public RFCommResult SaveReceiptOperation(ReceiptOperationDto receiptOperationDto)
        {
            _crudRepository.ChangeDB(receiptOperationDto.WarehouseSysId);
            RFCommResult rsp = new RFCommResult() { IsSucess = true };
            try
            {
                var receipt = _crudRepository.GetQuery<receipt>(p => p.ReceiptOrder == receiptOperationDto.ReceiptOrder && p.WarehouseSysId == receiptOperationDto.WarehouseSysId).FirstOrDefault();
                if (receipt == null)
                {
                    throw new Exception("收货单不存在");
                }
                var purchase = _crudRepository.GetQuery<purchase>(p => p.PurchaseOrder == receipt.ExternalOrder && p.WarehouseSysId == receiptOperationDto.WarehouseSysId).FirstOrDefault();
                if (purchase == null)
                {
                    throw new Exception("入库单不存在");
                }
                receiptOperationDto.SysId = receipt.SysId;
                receiptOperationDto.PurchaseSysId = purchase.SysId;
                receiptOperationDto.PurchaseDetailViewDto = _rfReceiptRepository.GetPurchaseDetailViewDtoList(purchase.SysId);
                if (receiptOperationDto.LotTemplateValueDtos != null && receiptOperationDto.LotTemplateValueDtos.Count > 0)
                {
                    foreach (var item in receiptOperationDto.LotTemplateValueDtos)
                    {
                        item.LotValue04 = string.Empty;
                        item.LotValue05 = string.Empty;
                        item.LotValue06 = string.Empty;
                        item.LotValue07 = string.Empty;
                        item.ExternalLot = string.Empty;
                    }
                }
                _receiptAppService.SaveReceiptOperation(receiptOperationDto);
            }
            catch (Exception ex)
            {
                rsp.IsSucess = false;
                rsp.Message = ex.Message;
            }
            return rsp;
        }
    }
}
