using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.ThirdParty;
using NBK.ECService.WMS.DTO.ThirdParty.ZTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IZTOAppService: IApplicationService
    {
        dynamic OrderSubmit(CreateZTOOrderRequest request);

        GenerateZTOOrderMarkeResponse OrderMarke(GenerateZTOOrderMarkeRequest request);

        GenerateZTOOrderSubmitResponse OrderSubmit(GenerateZTOOrderSubmitRequest request);
    }
}
