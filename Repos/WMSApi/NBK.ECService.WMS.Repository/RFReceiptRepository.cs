using Abp.EntityFramework;
using FortuneLab.Models;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class RFReceiptRepository : CrudRepository, IRFReceiptRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public RFReceiptRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 待收货列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public Page<RFReceiptListDto> GetWaitingReceiptListByPaging(RFReceiptQuery receiptQuery)
        {
            var query = from r in Context.receipts
                        join p in Context.purchases on r.ExternalOrder equals p.PurchaseOrder
                        join pd in Context.purchasedetails on p.SysId equals pd.PurchaseSysId
                        join s in Context.skus on pd.SkuSysId equals s.SysId
                        where r.WarehouseSysId == receiptQuery.WarehouseSysId && s.IsMaterial == false
                        && (p.Status == (int)PurchaseStatus.New || p.Status == (int)PurchaseStatus.InReceipt || p.Status == (int)PurchaseStatus.PartReceipt)
                        && (r.Status != (int)ReceiptStatus.Received && r.Status != (int)ReceiptStatus.Cancel)
                        select new { r };
            var receipts = query.Select(p => new RFReceiptListDto()
            {
                SysId = p.r.SysId,
                ReceiptOrder = p.r.ReceiptOrder,
                Status = p.r.Status,
                CreateDate = p.r.CreateDate
            }).Distinct();
            receiptQuery.iTotalDisplayRecords = receipts.Count();
            receipts = receipts.OrderByDescending(p => p.CreateDate).Skip(receiptQuery.iDisplayStart).Take(receiptQuery.iDisplayLength);
            return ConvertPages(receipts, receiptQuery);
        }

        /// <summary>
        /// 获取入库单收货明细
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public List<RFReceiptOperationDetailListDto> GetReceiptOperationDetailList(RFReceiptQuery receiptQuery)
        {
            var query = from pd in Context.purchasedetails
                        join p in Context.purchases on pd.PurchaseSysId equals p.SysId
                        join r in Context.receipts on p.PurchaseOrder equals r.ExternalOrder
                        join s in Context.skus on pd.SkuSysId equals s.SysId
                        join pa in Context.packs on pd.PackSysId equals pa.SysId into t
                        from ti in t.DefaultIfEmpty()
                        where r.ReceiptOrder == receiptQuery.ReceiptOrder && r.WarehouseSysId == receiptQuery.WarehouseSysId
                        select new { pd, s, ti };
            var receiptOperationDetails = query.Select(p => new RFReceiptOperationDetailListDto()
            {
                UPC = p.s.UPC,
                SkuSysId = p.pd.SkuSysId,
                SkuName = p.s.SkuName,
                PurchaseQty = p.pd.Qty - p.pd.ReceivedQty - p.pd.RejectedQty,
                UPC01 = p.ti.UPC01,
                UPC02 = p.ti.UPC02,
                UPC03 = p.ti.UPC03,
                UPC04 = p.ti.UPC04,
                UPC05 = p.ti.UPC05,
                FieldValue01 = p.ti.FieldValue01,
                FieldValue02 = p.ti.FieldValue02,
                FieldValue03 = p.ti.FieldValue03,
                FieldValue04 = p.ti.FieldValue04,
                FieldValue05 = p.ti.FieldValue05
            }).OrderBy(p => p.UPC).ToList();
            return receiptOperationDetails;
        }

        /// <summary>
        /// 收货完成获取入库单明细
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <returns></returns>
        public List<PurchaseDetailViewDto> GetPurchaseDetailViewDtoList(Guid purchaseSysId)
        {
            var query = from pd in Context.purchasedetails
                        join s in Context.skus on pd.SkuSysId equals s.SysId
                        where pd.PurchaseSysId == purchaseSysId
                        select new { pd, s };
            var purchaseDetails = query.Select(p => new PurchaseDetailViewDto()
            {
                SysId = p.pd.SysId,
                SkuUPC = p.s.UPC,
                SkuSysId = p.pd.SkuSysId
            }).ToList();
            return purchaseDetails;
        }
    }
}
