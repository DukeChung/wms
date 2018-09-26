using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IOutboundRepository : ICrudRepository
    {
        Pages<OutboundListDto> GetOutboundByPage(OutboundQuery request);

        List<OutboundListDto> GetOutboundDetailBySummary(List<Guid> SysIds);

        OutboundViewDto GetOutboundBySysId(Guid outboundSysId);

        List<OutboundDetailViewDto> GetOutboundDetails(Guid outboundSysId);

        List<ScanDeliveryDto> GetDeliveryBoxByByOrderNumber(string type, string orderNumber, Guid wareHouseSysId);

        /// <summary>
        /// 检测快速发货
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        bool CheckDeliveryIntercept(Guid outboundSysId);

        List<PurchaseDetailReturnDto> GetPurchasedetailForOutboundReturn(Guid outboundSysId, Guid purchaseSysId, long userID, string userName);

        List<invlot> GetInvlotForOutboundCancel(Guid outboundSysId);

        List<invskuloc> GetInvskulocForOutboundCancel(Guid outboundSysId);

        List<invlotloclpn> GetInvlotloclpnForOutboundCancel(Guid outboundSysId);

        List<InsufficientStockSkuListDto> GetInsufficientStockSkuList(OutboundAllocationDeliveryDto dto);

        void BatchUpdateSNListForOutbound(List<string> SNList,Guid warehouseSysId, Guid outboundSysId, long userID, string userName);

        void CancelReceiptsnByOutbound(Guid outboundSysId, long userID, string userName);

        outbound GetOutboundInfoBySysId(Guid sysId);

        List<PartShipmentDetailDto> GetPartShipmentSkuList(OutboundAllocationDeliveryDto dto);

        void CancelOutboundReturnByPurchase(PurchaseForReturnDto request);
    }
}
