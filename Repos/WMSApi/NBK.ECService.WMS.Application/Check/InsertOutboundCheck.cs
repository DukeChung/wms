using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;

namespace NBK.ECService.WMS.Application.Check
{
    public class InsertOutboundCheck : BaseCheck
    {
        private CommonResponse rsp = null;
        private warehouse warehouse = null;

        public InsertOutboundCheck(CommonResponse rsp, warehouse warehouse)
        {
            this.rsp = rsp;
            this.warehouse = warehouse;
        }

        public override CommonResponse Check()
        {
            if (warehouse == null)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = "未找到对应的仓库";
                return rsp;
            }
            return rsp;
        }
    }
}
