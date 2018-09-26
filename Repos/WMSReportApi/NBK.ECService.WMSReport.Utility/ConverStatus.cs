using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility.Enum;

namespace NBK.ECService.WMSReport.Utility
{
    public static class ConverStatus
    {
        /// <summary>
        /// 入库状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string Receipt(int status)
        {
            var result = string.Empty;
            switch (status)
            {
                case (int)ReceiptStatus.Init:
                    result = PublicConst.ReceiptInit;
                    break;
                case (int)ReceiptStatus.New:
                    result = PublicConst.ReceiptNew;
                    break;
                case (int)ReceiptStatus.Print:
                    result = PublicConst.ReceiptPrint;
                    break;
                case (int)ReceiptStatus.Received:
                    result = PublicConst.ReceiptReceived;
                    break;
                case (int)ReceiptStatus.Receiving:
                    result = PublicConst.ReceiptReceiving;
                    break;
                case (int)ReceiptStatus.Cancel:
                    result = PublicConst.ReceiptCancel;
                    break;
            }
            return result;
        }

        public static string ReceiptDetail(int status)
        {
            var result = string.Empty;
            switch (status)
            {
                case (int)ReceiptDetailStatus.Init:
                    result = PublicConst.ReceiptDetailInit;
                    break;
                case (int)ReceiptDetailStatus.New:
                    result = PublicConst.ReceiptDetailNew;
                    break;
                case (int)ReceiptDetailStatus.Received:
                    result = PublicConst.ReceiptDetailReceived;
                    break;
                case (int)ReceiptDetailStatus.Receiving:
                    result = PublicConst.ReceiptDetailReceiving;
                    break;
                case (int)ReceiptDetailStatus.Cancel:
                    result = PublicConst.ReceiptDetailCancel;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 采购状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string Purchase(int status)
        {
            var result = string.Empty;
            switch (status)
            {
                case (int)PurchaseStatus.New:
                    result = PublicConst.PurchaseNew;
                    break;
                case (int)PurchaseStatus.InReceipt:
                    result = PublicConst.PurchaseInReceipt;
                    break;
                case (int)PurchaseStatus.PartReceipt:
                    result = PublicConst.PurchasePartReceipt;
                    break;
                case (int)PurchaseStatus.Finish:
                    result = PublicConst.PurchaseFinish;
                    break;
                case (int)PurchaseStatus.StopReceipt:
                    result = PublicConst.PurchaseStopReceipt;
                    break;
                case (int)PurchaseStatus.Void:
                    result = PublicConst.PurchaseVoid;
                    break;
                case (int)PurchaseStatus.Close:
                    result = PublicConst.PurchaseClose;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 拣货单状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string PickDetail(int status)
        {
            var result = string.Empty;
            switch (status)
            {
                case (int)PickDetailStatus.New:
                    result = PublicConst.PickDetailNew;
                    break;
                case (int)PickDetailStatus.Finish:
                    result = PublicConst.PickDetailFinish;
                    break;
                case (int)PickDetailStatus.Cancel:
                    result = PublicConst.PickDetailCancel;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 出库单状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetOutboundStatus(int status)
        {
            var result = string.Empty;
            switch (status)
            {
                case (int)OutboundStatus.New:
                    result = PublicConst.OutboundStatusNew;
                    break;
                case (int)OutboundStatus.PartAllocation:
                    result = PublicConst.OutboundStatusPartAllocation;
                    break;
                case (int)OutboundStatus.Allocation:
                    result = PublicConst.OutboundStatusAllocation;
                    break;
                case (int)OutboundStatus.PartPick:
                    result = PublicConst.OutboundStatusPartPick;
                    break;
                case (int)OutboundStatus.Picking:
                    result = PublicConst.OutboundStatusPicking;
                    break;
                case (int)OutboundStatus.Delivery:
                    result = PublicConst.OutboundStatusDelivery;
                    break;
                case (int)OutboundStatus.Cancel:
                    result = PublicConst.OutboundStatusCancel;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 出库单类型
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetOutboundType(int type)
        {
            var result = string.Empty;
            switch (type)
            {
                case (int)OutboundType.Normal:
                    result = PublicConst.OutboundTypeNormal;
                    break;
                case (int)OutboundType.B2C:
                    result = PublicConst.OutboundTypeB2C;
                    break;
                case (int)OutboundType.B2B:
                    result = PublicConst.OutboundTypeB2B;
                    break;
                case (int)OutboundType.Return:
                    result = PublicConst.OutboundTypeReturn;
                    break;
                case (int)OutboundType.FIFO:
                    result = PublicConst.OutboundTypeFIFO;
                    break;
                case (int)OutboundType.TransferInventory:
                    result = PublicConst.OutboundTypeTransferInventory;
                    break;
                case (int)OutboundType.Material:
                    result = PublicConst.OutboundTypeMaterial;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 损益状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetAdjustmentStatus(int status)
        {
            var result = string.Empty;
            switch (status)
            {
                case (int)AdjustmentStatus.New:
                    result = PublicConst.AdjustmentStatusNew;
                    break;
                case (int)AdjustmentStatus.Audit:
                    result = PublicConst.AdjustmentStatusAudit;
                    break;
                case (int)AdjustmentStatus.Void:
                    result = PublicConst.AdjustmentStatusVoid;
                    break;
            }

            return result;
        }

        public static string Vanning(int status)
        {
            var result = string.Empty;
            switch (status)
            {
                case (int)VanningStatus.New:
                    result = PublicConst.VanningStatusNew;
                    break;
                case (int)VanningStatus.Vanning:
                    result = PublicConst.VanningStatusVanning;
                    break;
                case (int)VanningStatus.Finish:
                    result = PublicConst.VanningStatusFinish;
                    break;
                case (int)VanningStatus.Cancel:
                    result = PublicConst.OutboundStatusCancel;
                    break;
            }

            return result;
        }



        /// <summary>
        /// 预包装单状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetPrePackStatus(int status)
        {
            var result = string.Empty;
            switch (status)
            {
                case (int)PrePackStatus.New:
                    result = PublicConst.PrePackNew;
                    break;
                case (int)PrePackStatus.Finish:
                    result = PublicConst.PrePackFinish;
                    break;
                case (int)PrePackStatus.Cancel:
                    result = PublicConst.PrePackCancel;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 预包装单状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetTransferInventoryStatus(int status)
        {
            var result = string.Empty;
            switch (status)
            {
                case (int)TransferInventoryStatus.New:
                    result = PublicConst.TransferInventoryNew;
                    break;
                case (int)TransferInventoryStatus.Delivery:
                    result = PublicConst.TransferInventoryDelivery;
                    break;
                case (int)TransferInventoryStatus.ReceiptFinish:
                    result = PublicConst.TransferInventoryReceiptFinish;
                    break;
            }
            return result;
        }
    }
}
