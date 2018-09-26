using System.Collections.Generic;
using NBK.ECService.WMS.DTO;
using System;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IPickDetailRepository : ICrudRepository
    {
        Pages<PickDetailListDto> GetPickDetailListDtoByPageInfo(PickDetailQuery pickDetailQuery);

        List<PickDetailListDto> GetSummaryPickDetailListDto(List<Guid?> pickDetailSysIds);

        Pages<PickOutboundListDto> GetPickOutboundListDtoByPageInfo(PickDetailQuery pickDetailQuery);

        List<PickOutboundDetailListDto> GetPickOutboundDetailListDto(List<Guid?> outboundSysIds);

        List<PickDetailOperationDto> GetPickDetailOperationDto(string pickDetailOrder, Guid wareHouseSysId);

        List<PickingOperationDetail> GetPickingOperationDetails(PickingOperationQuery pickingOperationQuery);
    }
}