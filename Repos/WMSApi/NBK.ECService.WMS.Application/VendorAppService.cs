using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;

namespace NBK.ECService.WMS.Application
{
    public class VendorAppService : WMSApplicationService, IVendorAppService
    {
        private ICrudRepository _crudRepository = null;
        public VendorAppService(ICrudRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        public List<SelectItem> GetSelectVendor()
        {
            return _crudRepository.GetAllList<vendor>().Select(p => new SelectItem { Text = p.VendorName, Value = p.SysId.ToString() }).ToList();
        }
    }
}
