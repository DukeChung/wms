using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.Application.Check
{
    public class VoidOutboundCheck : BaseCheck
    {
        private CommonResponse rsp = null;
        private outbound outbound = null;

        public VoidOutboundCheck(CommonResponse rsp, outbound outbound)
        {
            this.rsp = rsp;
            this.outbound = outbound;
        }

        public override CommonResponse Check()
        {
            if (outbound == null)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = "出库单不存在";
                return rsp;
            }
            if (outbound.Status != (int)OutboundStatus.New)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = string.Format("出库单已{0}，不能关闭", ((OutboundStatus)outbound.Status).ToDescription());
                return rsp;
            }
            return rsp;
        }
    }
}
