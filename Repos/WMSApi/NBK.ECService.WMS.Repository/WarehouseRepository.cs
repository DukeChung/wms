using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class WarehouseRepository : CrudRepository, IWarehouseRepository
    {
        public WarehouseRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public Pages<UserWarehouseDto> GetNoAssignedWarehouse(UserWarehouseQuery request)
        {
            var query = from warehouse in Context.warehouses
                        where !(from uwm in Context.userwarehousemappings
                                where uwm.UserId == request.UserId
                                select uwm.WarehouseSysId).Contains(warehouse.SysId )
                        
                        select new UserWarehouseDto()
                        {
                            WarehouseSysId = warehouse.SysId,
                            WarehouseName = warehouse.Name,
                            CreateDate=warehouse.CreateDate

                        };
            request.iTotalDisplayRecords = query.Count();
            query = query.OrderByDescending(p => p.CreateDate).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(query, request);
        }

        public Pages<UserWarehouseDto> GetAssignedWarehouse(UserWarehouseQuery request)
        {
            var query = from uwm in Context.userwarehousemappings
                        join warehouse in Context.warehouses on uwm.WarehouseSysId equals warehouse.SysId
                        where uwm.UserId == request.UserId

                        select new UserWarehouseDto()
                        {
                            WarehouseSysId = warehouse.SysId,
                            WarehouseName = warehouse.Name,
                            CreateDate = uwm.CreateDate
                        };
            request.iTotalDisplayRecords = query.Count();
            query = query.OrderByDescending(p => p.CreateDate).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(query.OrderByDescending(p => p.CreateDate), request);
        }

        /// <summary>
        /// 根据userId获取仓库信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<WareHouseDto> GetWareHouseByUserId(int userId)
        {
            var query = from wm in Context.userwarehousemappings
                        join w in Context.warehouses on wm.WarehouseSysId equals w.SysId
                        where wm.UserId == userId
                        select new WareHouseDto()
                        {
                            SysId = w.SysId,
                            Name = w.Name
                        };

            return query.ToList();
        }
    }
}
