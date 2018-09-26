using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility.Enum;

namespace NBK.ECService.WMSReport.Utility
{
    public static class ConvertType
    {
        /// <summary>
        /// 出库类型
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string Outbound(int status)
        {
            var result = String.Empty;
            switch (status)
            {
                case (int)OutboundType.Normal:
                    result = PublicConst.OutboundTypeNormal;
                    break;
                case (int)OutboundType.B2B:
                    result = PublicConst.OutboundTypeB2B;
                    break;
                case (int)OutboundType.B2C:
                    result = PublicConst.OutboundTypeB2C;
                    break;
                case (int)OutboundType.Return:
                    result = PublicConst.OutboundTypeReturn;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 出库类型
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string Purchase(int status)
        {
            var result = String.Empty;
            switch (status)
            {
                case (int)PurchaseType.Purchase:
                    result = PublicConst.PurchaseTypePurchase;
                    break;
                case (int)PurchaseType.Return:
                    result = PublicConst.PurchaseTypeReturn;
                    break;
                case (int)PurchaseType.FIFO:
                    result = PublicConst.PurchaseTypeFiFo;
                    break;
                case (int)PurchaseType.Material:
                    result = PublicConst.PurchaseTypeMaterial;
                    break;
                case (int)PurchaseType.TransferInventory:
                    result = PublicConst.PurchaseTypeTransferInventory;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 损益类型
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetAdjustmentType(int type)
        {
            var result = String.Empty;
            switch (type)
            {
                case (int)AdjustmentType.ProfiAndLoss:
                    result = PublicConst.AdjustmentTypeProfiAndLoss;
                    break;
            }

            return result;
        }

    }
}
