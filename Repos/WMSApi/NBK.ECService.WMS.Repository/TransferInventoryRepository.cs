using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class TransferInventoryRepository : CrudRepository, ITransferInventoryRepository
    {
        public TransferInventoryRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 分页获取移仓单号
        /// </summary>
        /// <param name="transferinventoryQuery"></param>
        /// <returns></returns>
        public Pages<TransferinventoryListDto> GetTransferinventoryByPage(TransferinventoryQuery transferinventoryQuery)
        {
            var query = from tf in Context.transferinventorys
                        select new { tf };

            if (!string.IsNullOrEmpty(transferinventoryQuery.TransferInventoryOrder))
            {
                transferinventoryQuery.TransferInventoryOrder = transferinventoryQuery.TransferInventoryOrder.Trim();
                query = query.Where(x => transferinventoryQuery.TransferInventoryOrder.Contains(x.tf.TransferInventoryOrder));
            }
            if (transferinventoryQuery.FromWareHouseSysId != new Guid())
            {
                query = query.Where(x => x.tf.FromWareHouseSysId == transferinventoryQuery.FromWareHouseSysId);
            }
            if (transferinventoryQuery.ToWareHouseSysId != new Guid())
            {
                query = query.Where(x => x.tf.ToWareHouseSysId == transferinventoryQuery.ToWareHouseSysId);
            }
            if (transferinventoryQuery.Status.HasValue)
            {
                query = query.Where(x => x.tf.Status == transferinventoryQuery.Status.Value);
            }
            if (transferinventoryQuery.TransferOutboundDateFrom.HasValue)
            {
                query = query.Where(x => x.tf.TransferOutboundDate.Value > transferinventoryQuery.TransferOutboundDateFrom.Value);
            }
            if (transferinventoryQuery.TransferOutboundDateTo.HasValue)
            {
                query = query.Where(x => x.tf.TransferOutboundDate.Value < transferinventoryQuery.TransferOutboundDateTo.Value);
            }

            if (transferinventoryQuery.TransferInboundDateFrom.HasValue)
            {
                query = query.Where(x => x.tf.TransferInboundDate.Value > transferinventoryQuery.TransferInboundDateFrom.Value);
            }
            if (transferinventoryQuery.TransferInboundDateTo.HasValue)
            {
                query = query.Where(x => x.tf.TransferInboundDate.Value < transferinventoryQuery.TransferInboundDateTo.Value);
            }
            if (!string.IsNullOrEmpty(transferinventoryQuery.TransferOutboundOrder))
            {
                transferinventoryQuery.TransferOutboundOrder = transferinventoryQuery.TransferOutboundOrder.Trim();
                query = query.Where(x => x.tf.TransferOutboundOrder == transferinventoryQuery.TransferOutboundOrder);
            }
            if (!string.IsNullOrEmpty(transferinventoryQuery.TransferPurchaseOrder))
            {
                transferinventoryQuery.TransferPurchaseOrder = transferinventoryQuery.TransferPurchaseOrder.Trim();
                query = query.Where(x => x.tf.TransferPurchaseOrder == transferinventoryQuery.TransferPurchaseOrder);
            }
            if (!string.IsNullOrEmpty(transferinventoryQuery.ExternOrderId))
            {
                transferinventoryQuery.ExternOrderId = transferinventoryQuery.ExternOrderId.Trim();
                query = query.Where(x => x.tf.ExternOrderId == transferinventoryQuery.ExternOrderId);
            }
            if (!string.IsNullOrEmpty(transferinventoryQuery.Channel))
            {
                transferinventoryQuery.Channel = transferinventoryQuery.Channel.Trim();
                query = query.Where(x => x.tf.Channel.Contains(transferinventoryQuery.Channel));
            }

            var transferinventoryList = query.Select(x => new TransferinventoryListDto()
            {
                SysId = x.tf.SysId,
                TransferInventoryOrder = x.tf.TransferInventoryOrder,
                Status = x.tf.Status,
                FromWareHouseName = x.tf.FromWareHouseName,
                ToWareHouseName = x.tf.ToWareHouseName,
                ExternOrderId = x.tf.ExternOrderId,
                TransferOutboundDate = x.tf.TransferOutboundDate,
                TransferInboundDate = x.tf.TransferInboundDate,
                TransferOutboundOrder = x.tf.TransferOutboundOrder,
                TransferPurchaseOrder = x.tf.TransferPurchaseOrder,
                Remark = x.tf.Remark,
                Channel = x.tf.Channel


            }).Distinct();
            transferinventoryQuery.iTotalDisplayRecords = transferinventoryList.Count();
            transferinventoryList = transferinventoryList.OrderByDescending(p => p.TransferInventoryOrder).Skip(transferinventoryQuery.iDisplayStart).Take(transferinventoryQuery.iDisplayLength);
            return ConvertPages<TransferinventoryListDto>(transferinventoryList, transferinventoryQuery);
        }

        public TransferInventoryViewDto GetTransferinventoryBySysId(Guid sysId)
        {
            var query = from tf in Context.transferinventorys
                        where tf.SysId == sysId
                        select new TransferInventoryViewDto()
                        {
                            SysId = tf.SysId,
                            TransferInventoryOrder = tf.TransferInventoryOrder,
                            FromWareHouseName = tf.FromWareHouseName,
                            ToWareHouseName = tf.ToWareHouseName,
                            TransferOutboundDate = tf.TransferOutboundDate,
                            TransferInboundDate = tf.TransferInboundDate,
                            Status = tf.Status,
                            ExternOrderId = tf.ExternOrderId,
                            Remark = tf.Remark,
                            Channel = tf.Channel
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 根据移仓单获取移仓单明细列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<TransferInventoryDetailDto> GetTransferInventoryDetail(Guid sysId)
        {
            var query = from tfDetail in Context.transferinventorydetails
                        join sku in Context.skus on tfDetail.SkuSysId equals sku.SysId into t0
                        from p0 in t0.DefaultIfEmpty()
                        join p in Context.packs on p0.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        join u in Context.uoms on p1.FieldUom01 equals u.SysId into t3
                        from u1 in t3.DefaultIfEmpty()
                        join u in Context.uoms on p1.FieldUom02 equals u.SysId into t4
                        from u2 in t4.DefaultIfEmpty()
                        where tfDetail.TransferInventorySysId == sysId
                        select new TransferInventoryDetailDto
                        {
                            SysId = tfDetail.SysId,
                            SkuSysId = p0.SysId,
                            SkuCode = p0.SkuCode,
                            SkuName = p0.SkuName,
                            SkuUPC = p0.UPC,
                            SkuDescr = p0.SkuDescr,
                            UomCode = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? u2.UOMCode : u1.UOMCode,
                            PackCode = p1.PackCode,
                            Status = tfDetail.Status,
                            Qty = tfDetail.Qty,
                            DisplayQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * (tfDetail.Qty.HasValue ? tfDetail.Qty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (tfDetail.Qty.HasValue ? tfDetail.Qty.Value : 0),
                            ShippedQty = tfDetail.ShippedQty,
                            DisplayShippedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * (tfDetail.ShippedQty.HasValue ? tfDetail.ShippedQty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (tfDetail.ShippedQty.HasValue ? tfDetail.ShippedQty.Value : 0),
                            ReceivedQty = tfDetail.ReceivedQty,
                            DisplayReceivedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * (tfDetail.ReceivedQty.HasValue ? tfDetail.ReceivedQty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (tfDetail.ReceivedQty.HasValue ? tfDetail.ReceivedQty.Value : 0),
                            RejectedQty = tfDetail.RejectedQty,
                            DisplayRejectedQty = p1.InLabelUnit01.HasValue && p1.InLabelUnit01.Value == true
                                    && p1.FieldValue01 > 0 && p1.FieldValue02 > 0
                                    ? Math.Round(((p1.FieldValue02.Value * (tfDetail.RejectedQty.HasValue ? tfDetail.RejectedQty.Value : 0) * 1.00m) / p1.FieldValue01.Value), 3) : (tfDetail.RejectedQty.HasValue ? tfDetail.RejectedQty.Value : 0),
                            Remark = tfDetail.Remark,
                            PackFactor = tfDetail.PackFactor
                        };
            var list = query.ToList();
            return list;
        }

        public void BatchInsertTransferinventoryReceiptExtend(List<MQTransferinventoryReceiptExtendDto> list)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            var i = 1;

            if (list != null && list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"INSERT INTO transferinventoryreceiptextend(SysId, SkuSysId, PurchaseSysId, WarehouseSysId, Qty,ReceivedQty, Lot, LotAttr01, LotAttr02, 
LotAttr03,LotAttr04 , LotAttr05, LotAttr06, LotAttr07, LotAttr08, LotAttr09, ReceivedDate, ProduceDate, ExpiryDate, ExternalLot) VALUES ");
 
                list.ForEach(p => {
                    sb.Append($@"('{Guid.NewGuid()}',@SkuSysId{i},@PurchaseSysId{i},@WarehouseSysId{i},@Qty{i},@ReceivedQty{i},@Lot{i},@LotAttr01{i},@LotAttr02{i},
                    @LotAttr03{i},@LotAttr04{i},@LotAttr05{i},@LotAttr06{i},@LotAttr07{i},@LotAttr08{i},@LotAttr09{i},@ReceivedDate{i},@ProduceDate{i},@ExpiryDate{i},@ExternalLot{i}),");

                    parameters.Add(new MySqlParameter($"@SkuSysId{i}", p.SkuSysId));
                    parameters.Add(new MySqlParameter($"@PurchaseSysId{i}", p.PurchaseSysId));
                    parameters.Add(new MySqlParameter($"@WarehouseSysId{i}", p.WarehouseSysId));
                    parameters.Add(new MySqlParameter($"@Qty{i}", p.Qty));
                    parameters.Add(new MySqlParameter($"@ReceivedQty{i}", p.ReceivedQty));
                    parameters.Add(new MySqlParameter($"@Lot{i}", p.Lot));
                    parameters.Add(new MySqlParameter($"@LotAttr01{i}", p.LotAttr01));
                    parameters.Add(new MySqlParameter($"@LotAttr02{i}", p.LotAttr02));
                    parameters.Add(new MySqlParameter($"@LotAttr03{i}", p.LotAttr03));
                    parameters.Add(new MySqlParameter($"@LotAttr04{i}", p.LotAttr04));
                    parameters.Add(new MySqlParameter($"@LotAttr05{i}", p.LotAttr05));
                    parameters.Add(new MySqlParameter($"@LotAttr06{i}", p.LotAttr06));
                    parameters.Add(new MySqlParameter($"@LotAttr07{i}", p.LotAttr07));
                    parameters.Add(new MySqlParameter($"@LotAttr08{i}", p.LotAttr08));
                    parameters.Add(new MySqlParameter($"@LotAttr09{i}", p.LotAttr09));
                    parameters.Add(new MySqlParameter($"@ReceivedDate{i}", p.ReceivedDate));
                    parameters.Add(new MySqlParameter($"@ProduceDate{i}", p.ProduceDate));
                    parameters.Add(new MySqlParameter($"@ExpiryDate{i}", p.ExpiryDate));
                    parameters.Add(new MySqlParameter($"@ExternalLot{i}", p.ExternalLot));
                    i++;
                });

                string sql = sb.ToString().Trim(',') + ";";

                base.Context.Database.ExecuteSqlCommand(sql, parameters.ToArray());
            }
        }
    }

}
