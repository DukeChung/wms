using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using MySql.Data.MySqlClient;
using System.Text;

namespace NBK.ECService.WMS.Repository
{
    public class RFPickDetailRepository : CrudRepository, IRFPickDetailRepository
    {
        /// <param name="dbContextProvider"></param>
        public RFPickDetailRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 获取待拣货的出库单
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        public Pages<RFWaitingPickListDto> GetWaitingPickOutboundList(RFPickQuery pickQuery)
        {
            var query = from o in Context.outbounds
                        join od in Context.outbounddetails on o.SysId equals od.OutboundSysId
                        where (od.Status == (int)OutboundDetailStatus.New || od.Status == (int)OutboundDetailStatus.PartAllocation)
                        && o.WareHouseSysId == pickQuery.WarehouseSysId
                        group new { o, od } by new { o.SysId, o.OutboundOrder, od.SkuSysId } into g
                        select new
                        {
                            SysId = g.Key.SysId,
                            OutboundOrder = g.Key.OutboundOrder,
                            SkuSysId = g.Key.SkuSysId,
                            Qty = g.Sum(x => x.od.Qty)
                        };

            var waitingPickOutboundList = (from q in query
                                           group q by new { q.SysId, q.OutboundOrder } into g
                                           select new RFWaitingPickListDto
                                           {
                                               SysId = g.Key.SysId,
                                               OutboundOrder = g.Key.OutboundOrder,
                                               SkuCount = g.Count(),
                                               SkuQty = g.Sum(x => x.Qty)
                                           });

            pickQuery.iTotalDisplayRecords = waitingPickOutboundList.Count();
            waitingPickOutboundList = waitingPickOutboundList.OrderByDescending(p => p.OutboundOrder).Skip(pickQuery.iDisplayStart).Take(pickQuery.iDisplayLength);
            return ConvertPages(waitingPickOutboundList, pickQuery);
        }

        /// <summary>
        /// 获取待容器拣货的出库单
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        public Pages<RFContainerPickingListDto> GetWaitingContainerPickingListByPaging(RFPickQuery pickQuery)
        {
            var query = from o in Context.outbounds
                        join pd in Context.pickdetails on o.SysId equals pd.OutboundSysId
                        where o.Status == (int)OutboundStatus.Allocation && pd.Status == (int)PickDetailStatus.New && o.WareHouseSysId == pickQuery.WarehouseSysId && pd.Qty != pd.PickedQty
                        select new RFContainerPickingListDto
                        {
                            SysId = o.SysId,
                            OutboundOrder = o.OutboundOrder,
                            TotalQty = o.TotalQty.Value,
                            AuditingDate = o.AuditingDate
                        };
            var waitingContainerPickingList = query.Distinct();
            pickQuery.iTotalDisplayRecords = waitingContainerPickingList.Count();
            waitingContainerPickingList = waitingContainerPickingList.OrderByDescending(p => p.AuditingDate).Skip(pickQuery.iDisplayStart).Take(pickQuery.iDisplayLength);
            var rsp = ConvertPages(waitingContainerPickingList, pickQuery);
            if (rsp != null && rsp.TableResuls != null && rsp.TableResuls.aaData.Count > 0)
            {
                List<Guid> outboundSysIds = rsp.TableResuls.aaData.Select(p => p.SysId).ToList();
                var outboundDetailList = GetPickOutboundDetailListDto(outboundSysIds, pickQuery.WarehouseSysId);
                foreach (var item in rsp.TableResuls.aaData)
                {
                    var detail = outboundDetailList.Find(x => x.OutboundSysId == item.SysId);
                    item.SkuQty = detail.SkuTypeQty.GetValueOrDefault();
                }
            }
            return rsp;
        }

        private List<PickOutboundDetailListDto> GetPickOutboundDetailListDto(List<Guid> outboundSysIds, Guid wareHouseSysId)
        {

            var query = from obd in Context.outbounddetails
                        where outboundSysIds.Contains(obd.OutboundSysId.Value)
                        group obd by new { obd.OutboundSysId }
                into g
                        select new PickOutboundDetailListDto()
                        {
                            OutboundSysId = g.Key.OutboundSysId,
                            SkuTypeQty = g.Count()
                        };

            return query.ToList();
        }

        /// <summary>
        /// 获取某个出库单的待拣货商品
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        public List<RFWaitingPickSkuListDto> GetWaitingPickSkuList(RFPickQuery pickQuery)
        {
            var query = from od in Context.outbounddetails
                        join s in Context.skus on od.SkuSysId equals s.SysId
                        join o in Context.outbounds on od.OutboundSysId equals o.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId
                        where o.OutboundOrder == pickQuery.OutboundOrder && o.WareHouseSysId == pickQuery.WarehouseSysId
                        group new { od, s, p } by new { od, s, p } into g
                        select new RFWaitingPickSkuListDto
                        {
                            OutboundSysId = g.Key.od.OutboundSysId,
                            SkuSysId = g.Key.od.SkuSysId,
                            SkuCode = g.Key.s.SkuCode,
                            SkuName = g.Key.s.SkuName,
                            UPC = g.Key.s.UPC,
                            Qty = g.Sum(x => x.od.Qty),
                            UPC01 = g.Key.p.UPC01,
                            UPC02 = g.Key.p.UPC02,
                            UPC03 = g.Key.p.UPC03,
                            UPC04 = g.Key.p.UPC04,
                            UPC05 = g.Key.p.UPC05,
                            FieldValue01 = g.Key.p.FieldValue01,
                            FieldValue02 = g.Key.p.FieldValue02,
                            FieldValue03 = g.Key.p.FieldValue03,
                            FieldValue04 = g.Key.p.FieldValue04,
                            FieldValue05 = g.Key.p.FieldValue05,
                            WaitPickQty = g.Sum(x => x.od.Qty - (x.od.AllocatedQty == null ? 0 : x.od.AllocatedQty))
                        };

            return query.ToList();
        }

        /// <summary>
        /// 获取出库单容器拣货明细
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        public RFContainerPickingDto GetContainerPickingDetailList(RFPickQuery pickQuery)
        {
            RFContainerPickingDto rsp = new RFContainerPickingDto();
            var outboundSysId = GetQuery<outbound>(p => p.OutboundOrder == pickQuery.OutboundOrder && p.WareHouseSysId == pickQuery.WarehouseSysId).FirstOrDefault().SysId;

            var query = from pd in Context.pickdetails
                        join s in Context.skus on pd.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId
                        where pd.OutboundSysId == outboundSysId && pd.Status == (int)PickDetailStatus.New && pd.WareHouseSysId == pickQuery.WarehouseSysId
                        group new { pd } by new { pd.SysId, pd.SkuSysId, s.SkuName, s.SkuDescr, s.UPC, pd.Loc, pd.Lot, pd.PickDate, p.UPC01, p.UPC02, p.UPC03, p.UPC04, p.UPC05, p.FieldValue01, p.FieldValue02, p.FieldValue03, p.FieldValue04, p.FieldValue05 } into g
                        select new RFContainerPickingDetailListDto
                        {
                            SysId = g.Key.SysId,
                            OutboundSysId = outboundSysId,
                            SkuSysId = g.Key.SkuSysId,
                            SkuName = g.Key.SkuName,
                            SkuDescr = g.Key.SkuDescr,
                            UPC = g.Key.UPC,
                            Loc = g.Key.Loc,
                            Lot = g.Key.Lot,
                            Qty = g.Sum(p => p.pd.Qty),
                            PickedQty = g.Sum(p => p.pd.PickedQty),
                            CurrentPickedQty = 0,
                            PickDate = g.Key.PickDate,
                            UPC01 = g.Key.UPC01,
                            UPC02 = g.Key.UPC02,
                            UPC03 = g.Key.UPC03,
                            UPC04 = g.Key.UPC04,
                            UPC05 = g.Key.UPC05,
                            FieldValue01 = g.Key.FieldValue01,
                            FieldValue02 = g.Key.FieldValue02,
                            FieldValue03 = g.Key.FieldValue03,
                            FieldValue04 = g.Key.FieldValue04,
                            FieldValue05 = g.Key.FieldValue05
                        };
            rsp.PickingDetails = query.OrderByDescending(p => p.PickDate).ThenBy(p => p.Loc).ToList();
            rsp.GroupedPickingDetails = rsp.PickingDetails.GroupBy(p => p.SkuSysId).Select(p => new RFContainerPickingDetailListDto
            {
                SkuSysId = p.Key,
                UPC = p.FirstOrDefault().UPC,
                SkuName = p.FirstOrDefault().SkuName,
                UPC01 = p.FirstOrDefault().UPC01,
                UPC02 = p.FirstOrDefault().UPC02,
                UPC03 = p.FirstOrDefault().UPC03,
                UPC04 = p.FirstOrDefault().UPC04,
                UPC05 = p.FirstOrDefault().UPC05,
                FieldValue01 = p.FirstOrDefault().FieldValue01,
                FieldValue02 = p.FirstOrDefault().FieldValue02,
                FieldValue03 = p.FirstOrDefault().FieldValue03,
                FieldValue04 = p.FirstOrDefault().FieldValue04,
                FieldValue05 = p.FirstOrDefault().FieldValue05
            }).ToList();
            return rsp;
        }

        #region 加工单拣货
        /// <summary>
        /// 获取待拣货的加工单
        /// </summary>
        /// <param name="assemblyPickQuery"></param>
        /// <returns></returns>
        public Pages<RFWaitingAssemblyPickListDto> GetWaitingAssemblyList(RFAssemblyPickQuery assemblyPickQuery)
        {
            var query = from a in Context.assemblies
                        join ad in Context.assemblydetails on a.SysId equals ad.AssemblySysId
                        join s in Context.skus on ad.SkuSysId equals s.SysId into t1
                        from s1 in t1.DefaultIfEmpty()
                        join p in Context.packs on s1.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        where a.Status == (int)AssemblyStatus.Assembling && (ad.Status == (int)AssemblyDetailStatus.New || ad.Status == (int)AssemblyDetailStatus.PartPicking)
                        && a.WareHouseSysId == assemblyPickQuery.WarehouseSysId
                        group new { a, ad } by new { a.SysId, a.AssemblyOrder, ad.SkuSysId, ad.Qty, p1 } into g
                        select new
                        {
                            SysId = g.Key.SysId,
                            AssemblyOrder = g.Key.AssemblyOrder,
                            SkuSysId = g.Key.SkuSysId,
                            Qty = g.Key.Qty,
                            DisplaySkuQty = g.Key.p1.InLabelUnit01.HasValue && g.Key.p1.InLabelUnit01.Value == true
                            && g.Key.p1.FieldValue01 > 0 && g.Key.p1.FieldValue02 > 0
                            ? Math.Round(((g.Key.p1.FieldValue02.Value * g.Key.Qty * 1.00m) / g.Key.p1.FieldValue01.Value), 3) : g.Key.Qty
                        };

            var waitingPickList = (from q in query
                                   group q by new { q.SysId, q.AssemblyOrder } into g
                                   select new RFWaitingAssemblyPickListDto
                                   {
                                       SysId = g.Key.SysId,
                                       AssemblyOrder = g.Key.AssemblyOrder,
                                       SkuCount = g.Count(),
                                       SkuQty = g.Sum(x => x.Qty),
                                       DisplaySkuQty = g.Sum(x => x.DisplaySkuQty)
                                   });

            assemblyPickQuery.iTotalDisplayRecords = waitingPickList.Count();
            waitingPickList = waitingPickList.OrderByDescending(p => p.AssemblyOrder).Skip(assemblyPickQuery.iDisplayStart).Take(assemblyPickQuery.iDisplayLength);
            return ConvertPages(waitingPickList, assemblyPickQuery);
        }

        /// <summary>
        /// 获取加工单中的待拣货商品
        /// </summary>
        /// <param name="assemblyPickQuery"></param>
        /// <returns></returns>
        public List<RFWaitingAssemblyPickSkuListDto> GetWaitingAssemblyPickSkuList(RFAssemblyPickQuery assemblyPickQuery)
        {
            var query = from ad in Context.assemblydetails
                        join s in Context.skus on ad.SkuSysId equals s.SysId
                        join a in Context.assemblies on ad.AssemblySysId equals a.SysId
                        where a.AssemblyOrder == assemblyPickQuery.AssemblyOrder && a.WareHouseSysId == assemblyPickQuery.WarehouseSysId
                        group ad by new { ad.AssemblySysId, ad.SkuSysId, s.SkuCode, s.SkuName, s.UPC } into g
                        select new RFWaitingAssemblyPickSkuListDto
                        {
                            AssemblySysId = g.Key.AssemblySysId,
                            SkuSysId = g.Key.SkuSysId,
                            SkuCode = g.Key.SkuCode,
                            SkuName = g.Key.SkuName,
                            UPC = g.Key.UPC,
                            Qty = g.Sum(x => x.Qty),
                            WaitPickQty = g.Sum(x => x.Qty - x.PickedQty)
                        };

            var list = query.ToList();

            var displayQuery = from a in list
                               join s in Context.skus on a.SkuSysId equals s.SysId
                               join p in Context.packs on s.PackSysId equals p.SysId into t
                               from p1 in t.DefaultIfEmpty()
                               select new RFWaitingAssemblyPickSkuListDto
                               {
                                   AssemblySysId = a.AssemblySysId,
                                   SkuSysId = a.SkuSysId,
                                   SkuCode = a.SkuCode,
                                   SkuName = a.SkuName,
                                   UPC = a.UPC,
                                   Qty = a.Qty,
                                   DisplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                            && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                            ? Math.Round(((p1.FieldValue02.Value * a.Qty * 1.00m) / p1.FieldValue01.Value), 3) : a.Qty,
                                   WaitPickQty = a.WaitPickQty,
                                   DisplayWaitPickQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                            && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                            ? Math.Round(((p1.FieldValue02.Value * a.WaitPickQty * 1.00m) / p1.FieldValue01.Value), 3) : a.WaitPickQty,
                                   UPC01 = p1.UPC01,
                                   UPC02 = p1.UPC02,
                                   UPC03 = p1.UPC03,
                                   UPC04 = p1.UPC04,
                                   UPC05 = p1.UPC05,
                                   FieldValue01 = p1.FieldValue01,
                                   FieldValue02 = p1.FieldValue02,
                                   FieldValue03 = p1.FieldValue03,
                                   FieldValue04 = p1.FieldValue04,
                                   FieldValue05 = p1.FieldValue05
                               };
            return displayQuery.ToList();
        }
        #endregion

        /// <summary>
        /// 加工拣货时根据规则获取库存记录
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="skuSysId"></param>
        /// <param name="wareHouseSysId"></param>
        /// <param name="channel"></param>
        /// <param name="assembRule"></param>
        /// <returns></returns>
        public IQueryable<FRInvLotLocLpnListDto> GetInvlotloclpnList(string loc, Guid skuSysId, Guid wareHouseSysId, string channel, assemblyrule assembRule)
        {
            var query = from iil in Context.invlotloclpns
                        join inv in Context.invlots on new { iil.Lot, iil.WareHouseSysId, iil.SkuSysId } equals new { inv.Lot, inv.WareHouseSysId, inv.SkuSysId }
                        where iil.Loc == loc && iil.WareHouseSysId == wareHouseSysId && iil.SkuSysId == skuSysId
                        select new
                        {
                            iil,
                            inv.ProduceDate,
                            inv.ReceiptDate,
                            inv.LotAttr01
                        };

            #region  增加加工拣货时匹配加工规则
            if (assembRule != null)
            {
                if (assembRule.Status)
                {
                    if (assembRule.MatchingLotAttr)
                    {
                        if (!string.IsNullOrEmpty(channel))
                        {
                            query = query.Where(x => x.LotAttr01 == channel);
                        }
                        else
                        {
                            query = query.Where(x => x.LotAttr01 == null || x.LotAttr01 == "");
                        }
                    }
                    //先生产先分配
                    if (assembRule.DeliverySortRules == (int)DeliveryAssemblyRule.FirstProduceFirstAssembly)
                    {
                        query = query.OrderBy(x => x.ProduceDate);
                    }
                    //后生产先分配
                    if (assembRule.DeliverySortRules == (int)DeliveryAssemblyRule.AfterProduceFirstAssembly)
                    {
                        query = query.OrderByDescending(x => x.ProduceDate);
                    }
                    //先入库先分配
                    if (assembRule.DeliverySortRules == (int)DeliveryAssemblyRule.FirstReceiptFirstAssembly)
                    {
                        query = query.OrderBy(x => x.ReceiptDate);
                    }

                }
            }
            #endregion

            return query.Select(p => new FRInvLotLocLpnListDto
            {
                SysId = p.iil.SysId,
                WareHouseSysId = p.iil.WareHouseSysId,
                SkuSysId = p.iil.SkuSysId,
                Loc = p.iil.Loc,
                Lot = p.iil.Lot,
                Lpn = p.iil.Lpn,
                Qty = p.iil.Qty,
                AllocatedQty = p.iil.AllocatedQty,
                PickedQty = p.iil.PickedQty,
                Status = p.iil.Status,
                FrozenQty = p.iil.FrozenQty
            });
        }
    }
}
