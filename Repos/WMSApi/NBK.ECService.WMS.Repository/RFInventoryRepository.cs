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
    public class RFInventoryRepository : CrudRepository, IRFInventoryRepository
    {
        /// <param name="dbContextProvider"></param>
        public RFInventoryRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// RF库存查询
        /// </summary>
        /// <param name="invSkuLocQuery"></param>
        /// <returns></returns>
        public List<RFInvSkuLocListDto> GetInvSkuLocList(RFInvSkuLocQuery invSkuLocQuery)
        {
            var query = from i in Context.invskulocs
                        join s in Context.skus on i.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId into t2
                        from p1 in t2.DefaultIfEmpty()
                        select new { s, i, p1 };

            query = query.Where(p => p.i.WareHouseSysId == invSkuLocQuery.WarehouseSysId);
            query = query.Where(p => p.i.Qty > 0);

            if (!invSkuLocQuery.SkuUPC.IsNull())
            {
                query = query.Where(p => p.s.UPC == invSkuLocQuery.SkuUPC);
            }
            if (!invSkuLocQuery.Loc.IsNull())
            {
                query = query.Where(p => p.i.Loc == invSkuLocQuery.Loc);
            }
            var list = query.Select(p => new RFInvSkuLocListDto
            {
                SkuName = p.s.SkuName,
                Loc = p.i.Loc,
                Qty = p.i.Qty,
                AvailableQty = p.i.Qty - p.i.AllocatedQty - p.i.PickedQty - p.i.FrozenQty,
                DisplayQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * p.i.Qty * 1.00m) / p.p1.FieldValue01.Value), 3) : p.i.Qty,
                DisplayAvailableQty = p.p1.InLabelUnit01.HasValue && p.p1.InLabelUnit01.Value == true
                            && p.p1.FieldValue01 > 0 && p.p1.FieldValue02 > 0
                            ? Math.Round(((p.p1.FieldValue02.Value * (p.i.Qty - p.i.AllocatedQty - p.i.PickedQty - p.i.FrozenQty) * 1.00m) / p.p1.FieldValue01.Value), 3) : (p.i.Qty - p.i.AllocatedQty - p.i.PickedQty - p.i.FrozenQty)
            }).OrderBy(p => p.Loc).ToList();

            return list;
        }
    }
}
