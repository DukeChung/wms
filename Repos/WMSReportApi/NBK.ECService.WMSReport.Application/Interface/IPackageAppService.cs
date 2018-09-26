using System;
using Abp.Application.Services;
using NBK.ECService.WMSReport.Model.Models;

namespace NBK.ECService.WMSReport.Application.Interface
{
    public interface IPackageAppService : IApplicationService
    {
        bool GetSkuConversiontransQty(Guid skuSysId, int requestQty, out int responseQty, ref pack pack);

        bool GetSkuConversiontransQty(Guid skuSysId, decimal requestQty, out int responseQty, ref pack pack);

        bool GetSkuDeconversiontransQty(Guid skuSysId, int requestQty, out decimal responseQty);

        bool GetSkuDeconversiontransQty(Guid skuSysId, int requestQty, out decimal responseQty, ref uom uom);
    }
}