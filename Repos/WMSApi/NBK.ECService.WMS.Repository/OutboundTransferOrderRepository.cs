using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class OutboundTransferOrderRepository : CrudRepository, IOutboundTransferOrderRepository
    {
        public OutboundTransferOrderRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundTransferOrderDto> GetOutboundTransferOrderByPage(OutboundTransferOrderQuery request)
        {
            var query = from order in Context.outboundtransferorder
                        join p in Context.prebulkpack on order.PreBulkPackSysId equals p.SysId into t0
                        from p1 in t0.DefaultIfEmpty()
                        where order.WareHouseSysId == request.WarehouseSysId
                        select new { order, p1 };

            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.ServiceStationName))
                {
                    request.ServiceStationName = request.ServiceStationName.Trim();
                    query = query.Where(p => p.order.ServiceStationName.Contains(request.ServiceStationName));
                }
                if (!string.IsNullOrEmpty(request.OutboundOrder))
                {
                    request.OutboundOrder = request.OutboundOrder.Trim();
                    query = query.Where(p => p.order.OutboundOrder.Contains(request.OutboundOrder));
                }
                if (!string.IsNullOrEmpty(request.TransferOrderNumber))
                {
                    request.TransferOrderNumber = request.TransferOrderNumber.Trim();
                    query = query.Where(p => p.order.TransferOrder == request.TransferOrderNumber);
                }
                if (request.Status.HasValue)
                {
                    query = query.Where(p => p.order.Status == request.Status.Value);
                }
                if (request.TransferType.HasValue)
                {
                    query = query.Where(p => p.order.TransferType == request.TransferType.Value);
                }
            }

            var response = query.Select(p => new OutboundTransferOrderDto()
            {
                SysId = p.order.SysId,
                ServiceStationName = p.order.ServiceStationName,
                OutboundOrder = p.order.OutboundOrder,
                Status = p.order.Status,
                CreateDate = p.order.CreateDate,
                BoxNumber = p.order.BoxNumber,
                TransferOrder = p.order.TransferOrder,
                TransferType = p.order.TransferType
            }).Distinct();

            request.iTotalDisplayRecords = response.Count();
            response = response.OrderByDescending(p => p.CreateDate).ThenByDescending(x => x.BoxNumber).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(response, request);
        }

        /// <summary>
        /// 获取交接明细数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public OutboundTransferOrderDto GetDataBySysId(Guid sysId)
        {
            var query = from order in Context.outboundtransferorder
                        join p in Context.prebulkpack on order.PreBulkPackSysId equals p.SysId into t0
                        from p1 in t0.DefaultIfEmpty()
                        where order.SysId == sysId
                        select new OutboundTransferOrderDto()
                        {
                            SysId = order.SysId,
                            ServiceStationName = order.ServiceStationName,
                            BoxNumber = order.BoxNumber,
                            OutboundOrder = order.OutboundOrder,
                            CreateDate = order.CreateDate,
                            Status = order.Status,
                            TransferOrder = order.TransferOrder
                        };
            var torder = query.FirstOrDefault();
            if (torder != null)
            {
                var queryDetail =
                        from outDetail in Context.outboundtransferorderdetail
                        join sku in Context.skus on outDetail.SkuSysId equals sku.SysId
                        where outDetail.OutboundTransferOrderSysId == sysId

                        join p in Context.packs on sku.PackSysId equals p.SysId into t4
                        from p4 in t4.DefaultIfEmpty()

                        join u in Context.uoms on p4.FieldUom01 equals u.SysId into t2
                        from ut2 in t2.DefaultIfEmpty()
                        join u1 in Context.uoms on p4.FieldUom02 equals u1.SysId into t3
                        from ti3 in t3.DefaultIfEmpty()

                        select new OutboundTransferOrderDetailDto()
                        {
                            SysId = outDetail.SysId,
                            OutboundTransferOrderSysId = outDetail.OutboundTransferOrderSysId,
                            SkuSysId = outDetail.SkuSysId,
                            SkuCode = sku.SkuCode,
                            SkuName = sku.SkuName,
                            UPC = sku.UPC,
                            Qty = outDetail.Qty,
                            Loc = outDetail.Loc,
                            UomCode = p4.InLabelUnit01.HasValue && p4.InLabelUnit01.Value == true && p4.FieldValue01 > 0 && p4.FieldValue02 > 0 ? ti3.UOMCode : ut2.UOMCode,
                            PackCode = p4.PackCode
                        };

                torder.OutboundTransferOrderDetailDto = queryDetail.ToList();
            }
            return torder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="status"></param>
        public void UpdateOutboundTransferOrder(OutboundTransferOrderQueryDto dto)
        {
            var sql = new StringBuilder();

            //更新交接箱表
            sql.AppendFormat(@"UPDATE outboundtransferorder SET Status =@Status ,
                                UpdateBy=@UpdateBy,
                                UpdateDate=NOW(),
                                UpdateUserName=@UpdateUserName     
                        where OutboundSysId =@OutboundSysId and WareHouseSysId=@WareHouseSysId ;");


            var result = base.Context.Database.ExecuteSqlCommand(sql.ToString()
                 , new MySqlParameter("@Status", dto.Status)
                 , new MySqlParameter("@UpdateBy", dto.CurrentUserId)
                 , new MySqlParameter("@UpdateUserName", dto.CurrentDisplayName)
                 , new MySqlParameter("@OutboundSysId", dto.OutboundSysId)
                 , new MySqlParameter("@WareHouseSysId", dto.WarehouseSysId));

        }

        public void UpdateOutboundTransferOrderFinish(OutboundTransferOrderQueryDto dto)
        {
            var sql = new StringBuilder();

            //进行中改完成
            sql.Append($@"UPDATE outboundtransferorder SET Status = {(int)OutboundTransferOrderStatus.Finish},
                        UpdateBy=@UpdateBy ,
                        UpdateDate=NOW(),
                        UpdateUserName=@UpdateUserName
                        where OutboundSysId =@OutboundSysId and WareHouseSysId=@WareHouseSysId and Status = {(int)OutboundTransferOrderStatus.PrePack};");

            //新建改作废
            sql.Append($@"UPDATE outboundtransferorder SET Status = {(int)OutboundTransferOrderStatus.Cancel},
                        UpdateBy=@UpdateBy ,
                        UpdateDate=NOW(),
                        UpdateUserName=@UpdateUserName    
                        where OutboundSysId =@OutboundSysId  and WareHouseSysId=@WareHouseSysId and Status = {(int)OutboundTransferOrderStatus.New};");


            var result = base.Context.Database.ExecuteSqlCommand(sql.ToString()
                         , new MySqlParameter("@UpdateBy", dto.CurrentUserId)
                         , new MySqlParameter("@UpdateUserName", dto.CurrentDisplayName)
                         , new MySqlParameter("@OutboundSysId", dto.OutboundSysId)
                         , new MySqlParameter("@WareHouseSysId", dto.WarehouseSysId));

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="status"></param>
        public void DeleteOutboundTransferOrder(OutboundTransferOrderQueryDto dto)
        {
            var sql = new StringBuilder();

            //更新交接箱表
            sql.Append(@"UPDATE outboundtransferorder SET Status =@Status , TransferType = @TransferType,
                        PreBulkPackSysId=null,PreBulkPackOrder=null,
                        UpdateBy=@UpdateBy ,
                        UpdateDate=NOW(),
                        UpdateUserName=@UpdateUserName  
                        where OutboundSysId =@OutboundSysId  and WareHouseSysId=@WareHouseSysId ;");

            sql.AppendFormat(@"DELETE FROM outboundtransferorderdetail WHERE OutboundTransferOrderSysId IN
                            (SELECT SysId FROM outboundtransferorder o WHERE o.OutboundSysId =@OutboundSysId  and o.WareHouseSysId=@WareHouseSysId );");
            var result = base.Context.Database.ExecuteSqlCommand(sql.ToString()
                         , new MySqlParameter("@Status", dto.Status)
                         , new MySqlParameter("@TransferType", dto.TransferType)
                         , new MySqlParameter("@UpdateBy", dto.CurrentUserId)
                         , new MySqlParameter("@UpdateUserName", dto.CurrentDisplayName)
                         , new MySqlParameter("@OutboundSysId", dto.OutboundSysId)
                         , new MySqlParameter("@WareHouseSysId", dto.WarehouseSysId));
        }


        public List<OutboundTransferPrintDto> GetOutboundTransferBox(List<Guid> request)
        {
            var query = from ot in Context.outboundtransferorder
                        join o in Context.outbounds on ot.OutboundSysId equals o.SysId into t0
                        from p1 in t0.DefaultIfEmpty()
                        join t in Context.transferinventorys on p1.ExternOrderId equals t.TransferInventoryOrder into t1
                        from p2 in t1.DefaultIfEmpty()
                        select new OutboundTransferPrintDto()
                        {
                            SysId = ot.SysId,
                            ConsigneeArea = p1.ConsigneeArea,
                            ServiceStationName = ot.ServiceStationName,
                            BoxNumber = ot.BoxNumber,
                            ConsigneeTown = p1.ConsigneeTown,
                            TransferOrder = ot.TransferOrder,
                            OutboundChildType = p1.OutboundChildType,
                            OutboundType = p1.OutboundType,
                            ToWareHouseName = p2.ToWareHouseName
                        };
            if (request.Count > 0)
            {
                query = query.Where(x => request.Contains(x.SysId));
            }
            return query.ToList();
        }

        /// <summary>
        /// 根据出库单获取所有交接单
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public List<OutboundTransferPrintDto> GetOutboundTransferOrder(OutboundTransferOrderQuery dto)
        {
            var query = from ot in Context.outboundtransferorder
                        where ot.OutboundOrder == dto.OutboundOrder && ot.Status != (int)OutboundTransferOrderStatus.Cancel
                        && ot.WareHouseSysId == dto.WarehouseSysId && ot.TransferType != (int)OutboundTransferOrderType.Whole
                        select new OutboundTransferPrintDto()
                        {
                            SysId = ot.SysId,
                            TransferOrder = ot.TransferOrder
                        };

            return query.ToList();

        }
    }
}
