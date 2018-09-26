using System;
using System.Collections.Generic;
using Abp.Domain.Repositories;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IPurchaseRepository: ICrudRepository
    {
        Pages<PurchaseListDto> GetPurchaseDtoListByPageInfo(PurchaseQuery purchaseQuery);

        PurchaseViewDto GetPurchaseViewDtoBySysId(Guid sysId);

        List<PurchaseDetailViewDto> GetPurchaseDetailViewBySysId(Guid sysId);

        List<PurchaseDetailSkuDto> GetPurchaseDetailSkuByUpcIsNull(Guid purchaseSysId);

        List<PurchaseDetailSkuDto> GetPurchaseDetailSku(Guid purchaseSysId);

        List<ReceiptdetailAutoShelvesDto> GetReceiptdetailForAutoShelves(Guid outboundSysId, Guid receiptSysId, int userId, string userName);

        List<ReceiptdetailAutoShelvesDto> GetReceiptdetailForAutoShelves(Guid outboundSysId, Guid receiptSysId, int userId, string userName,List<Guid> skuSysIds);

        List<invlot> GetInvlotForAutoShelves(Guid outboundSysId);

        List<invskuloc> GetInvskulocForAutoShelves(Guid outboundSysId);

        List<invlotloclpn> GetInvlotloclpnForAutoShelves(Guid outboundSysId);

        List<PurchaseAutoShelvesSkuInfo> GetSkuForAutoShelves(Guid outboundSysId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        void UpdatePurchaseBatchNumberBySysId(List<Guid> sysId, string batchNumber);

        /// <summary>
        /// 修改业务类型（指定上下行）
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="batchNumber"></param>
        /// <returns></returns>
        bool UpdatePurchaseBusinessTypeBySysId(List<Guid> sysId, string businessType);
    }
}