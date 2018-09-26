using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using System;

namespace NBK.ECService.WMS.Application.Check
{
    public class InventoryCheck : BaseCheck
    {
        private invlot invLot = null;
        private invskuloc invSkuLoc = null;
        private invlotloclpn invLotLocLpn = null;
        private string message = string.Empty;

        public InventoryCheck(invlot invLot, invskuloc invSkuLoc, invlotloclpn invLotLocLpn, string message)
        {
            this.invLot = invLot;
            this.invSkuLoc = invSkuLoc;
            this.invLotLocLpn = invLotLocLpn;
            this.message = message;
        }

        public override CommonResponse Check()
        {
            if (invLot == null || invSkuLoc == null || invLotLocLpn == null)
            {
                throw new Exception(message);
            }
            return new CommonResponse { IsSuccess = true };
        }
    }
}
