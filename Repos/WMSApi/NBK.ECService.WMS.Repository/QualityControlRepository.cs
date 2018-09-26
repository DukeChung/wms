using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class QualityControlRepository : CrudRepository, IQualityControlRepository
    {
        /// <param name="dbContextProvider"></param>
        public QualityControlRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        /// <summary>
        /// 获取质检单列表
        /// </summary>
        /// <param name="qualityControlQuery"></param>
        /// <returns></returns>
        public Pages<QualityControlListDto> GetQualityControlList(QualityControlQuery qualityControlQuery)
        {
            var query = from q in Context.qualitycontrol
                        where q.WareHouseSysId == qualityControlQuery.WarehouseSysId
                        select new { q };
            if (!qualityControlQuery.QCOrderSearch.IsNull())
            {
                qualityControlQuery.QCOrderSearch = qualityControlQuery.QCOrderSearch.Trim();
                query = query.Where(p => p.q.QCOrder == qualityControlQuery.QCOrderSearch);
            }
            if (qualityControlQuery.StatusSearch.HasValue)
            {
                query = query.Where(p => p.q.Status == qualityControlQuery.StatusSearch.Value);
            }
            if (qualityControlQuery.QCTypeSearch.HasValue)
            {
                query = query.Where(p => p.q.QCType == qualityControlQuery.QCTypeSearch.Value);
            }
            if (!qualityControlQuery.DocOrderSearch.IsNull())
            {
                qualityControlQuery.DocOrderSearch = qualityControlQuery.DocOrderSearch.Trim();
                query = query.Where(p => p.q.DocOrder == qualityControlQuery.DocOrderSearch);
            }
            if (!qualityControlQuery.ExternOrderIdSearch.IsNull())
            {
                qualityControlQuery.ExternOrderIdSearch = qualityControlQuery.ExternOrderIdSearch.Trim();
                query = query.Where(p => p.q.ExternOrderId == qualityControlQuery.ExternOrderIdSearch);
            }
            var qcList = query.Select(p => new QualityControlListDto
            {
                SysId = p.q.SysId,
                Status = p.q.Status,
                QCOrder = p.q.QCOrder,
                CreateDate = p.q.CreateDate,
                QCType = p.q.QCType,
                ExternOrderId = p.q.ExternOrderId,
                DocOrder = p.q.DocOrder,
                QCUserName = p.q.QCUserName,
                QCDate = p.q.QCDate
            }).Distinct();
            qualityControlQuery.iTotalDisplayRecords = qcList.Count();
            qcList = qcList.OrderByDescending(p => p.CreateDate).Skip(qualityControlQuery.iDisplayStart).Take(qualityControlQuery.iDisplayLength);
            return ConvertPages(qcList, qualityControlQuery);
        }

        public Pages<DocDetailDto> GetDocDetails(DocDetailQuery docDetailQuery)
        {
            if (docDetailQuery.QCType == (int)QCType.PurchaseQC)
            {
                var query = from pd in Context.purchasedetails
                            join p in Context.purchases on pd.PurchaseSysId equals p.SysId
                            join s in Context.skus on pd.SkuSysId equals s.SysId
                            join pa in Context.packs on s.PackSysId equals pa.SysId into t1
                            from ti1 in t1.DefaultIfEmpty()
                            join u1 in Context.uoms on ti1.FieldUom01 equals u1.SysId into t2
                            from ti2 in t2.DefaultIfEmpty()
                            join u2 in Context.uoms on ti1.FieldUom02 equals u2.SysId into t3
                            from ti3 in t3.DefaultIfEmpty()
                            where p.PurchaseOrder == docDetailQuery.DocOrder
                            select new { pd, s, ti1, UOMSysId1 = ti2.SysId, UOMCode1 = ti2.UOMCode, UOMSysId2 = ti3.SysId, UOMCode2 = ti3.UOMCode };
                var docDetails = query.Select(p => new DocDetailDto
                {
                    SysId = p.pd.SysId,
                    SkuSysId = p.s.SysId,
                    SkuName = p.s.SkuName,
                    UPC = p.s.UPC,
                    UOMSysId = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                                && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0 ? p.UOMSysId2 : p.UOMSysId1,
                    UOMCode = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                                && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0 ? p.UOMCode2 : p.UOMCode1,
                    PackSysId = p.ti1.SysId,
                    Qty = p.pd.ReceivedQty,
                    DisplayQty = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                                    && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0
                                    ? Math.Round(((p.ti1.FieldValue02.Value * p.pd.ReceivedQty * 1.00m) / p.ti1.FieldValue01.Value), 3)
                                    : p.pd.ReceivedQty
                }).Distinct();
                docDetailQuery.iTotalDisplayRecords = docDetails.Count();
                docDetails = docDetails.OrderByDescending(p => p.UPC).Skip(docDetailQuery.iDisplayStart).Take(docDetailQuery.iDisplayLength);
                return ConvertPages(docDetails, docDetailQuery);
            }
            else
            {
                return null;
            }
        }

        public List<DocDetailDto> GetDocDetails(string docOrder, int qcType, List<Guid> skuSysIds)
        {
            if (qcType == (int)QCType.PurchaseQC)
            {
                var query = from pd in Context.purchasedetails
                            join p in Context.purchases on pd.PurchaseSysId equals p.SysId
                            join s in Context.skus on pd.SkuSysId equals s.SysId
                            join pa in Context.packs on s.PackSysId equals pa.SysId into t1
                            from ti1 in t1.DefaultIfEmpty()
                            where p.PurchaseOrder == docOrder && skuSysIds.Contains(pd.SkuSysId)
                            select new { pd, s, ti1 };
                return query.Select(p => new DocDetailDto
                {
                    SysId = p.pd.SysId,
                    SkuSysId = p.s.SysId,
                    SkuName = p.s.SkuName,
                    UPC = p.s.UPC,
                    PackSysId = p.ti1.SysId,
                    Qty = p.pd.ReceivedQty,
                    DisplayQty = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                                    && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0
                                    ? Math.Round(((p.ti1.FieldValue02.Value * p.pd.ReceivedQty * 1.00m) / p.ti1.FieldValue01.Value), 3)
                                    : p.pd.ReceivedQty
                }).Distinct().ToList();
            }
            else
            {
                return null;
            }
        }

        public Pages<QualityControlDetailDto> GetQCDetails(QCDetailQuery qcDetailQuery)
        {
            var query = from qd in Context.qualitycontroldetail
                        join s in Context.skus on qd.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t1
                        from ti1 in t1.DefaultIfEmpty()
                        join u1 in Context.uoms on ti1.FieldUom01 equals u1.SysId into t2
                        from ti2 in t2.DefaultIfEmpty()
                        join u2 in Context.uoms on ti1.FieldUom02 equals u2.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()
                        where qd.QualityControlSysId == qcDetailQuery.QCSysId
                        select new { qd, s, ti1, UOMSysId1 = ti2.SysId, UOMCode1 = ti2.UOMCode, UOMSysId2 = ti3.SysId, UOMCode2 = ti3.UOMCode };
            var qcDetails = query.Select(p => new QualityControlDetailDto
            {
                SysId = p.qd.SysId,
                QualityControlSysId = p.qd.QualityControlSysId,
                SkuSysId = p.s.SysId,
                SkuName = p.s.SkuName,
                UPC = p.s.UPC,
                UOMSysId = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                                && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0 ? p.UOMSysId2 : p.UOMSysId1,
                UOMCode = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                                && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0 ? p.UOMCode2 : p.UOMCode1,
                PackSysId = p.ti1.SysId,
                Qty = p.qd.Qty,
                DisplayQty = p.ti1.InLabelUnit01.HasValue && p.ti1.InLabelUnit01.Value == true
                                && p.ti1.FieldValue01 > 0 && p.ti1.FieldValue02 > 0
                                ? Math.Round(((p.ti1.FieldValue02.Value * (p.qd.Qty.HasValue ? p.qd.Qty.Value : 0) * 1.00m) / p.ti1.FieldValue01.Value), 3)
                                : (p.qd.Qty.HasValue ? p.qd.Qty.Value : 0),
                Descr = p.qd.Descr ?? string.Empty,
                UpdateDate = p.qd.UpdateDate
            }).Distinct();
            qcDetailQuery.iTotalDisplayRecords = qcDetails.Count();
            qcDetails = qcDetails.OrderByDescending(p => p.UPC);
            return ConvertPages(qcDetails, qcDetailQuery);
        }
    }
}
