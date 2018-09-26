using Abp.EntityFramework;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.Repository
{
    public class LocationRepository : CrudRepository, ILocationRepository
    {
        /// <param name="dbContextProvider"></param>
        public LocationRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        /// <summary>
        /// 获取库位列表
        /// </summary>
        /// <param name="locationQuery"></param>
        /// <returns></returns>
        public Pages<LocationListDto> GetLocationListByPaging(LocationQuery locationQuery)
        {
            var query = from l in Context.locations
                        join scd in
                              (from sc in Context.syscodes
                              join scd in Context.syscodedetails on sc.SysId equals scd.SysCodeSysId
                            where sc.SysCodeType == PublicConst.SysCodeLocUsage
                            select new { scd.Code, scd.Descr }) on l.LocUsage equals scd.Code
                        select new { l, scd };
            query = query.Where(p => p.l.WarehouseSysId == locationQuery.WarehouseSysId);
            if (!locationQuery.LocationSearch.IsNull())
            {
                var seacrh = locationQuery.LocationSearch.Trim();
                query = query.Where(p => p.l.Loc.Contains(seacrh));
            }
            if (locationQuery.ZoneSysIdSearch.HasValue)
            {
                query = query.Where(p => p.l.ZoneSysId == locationQuery.ZoneSysIdSearch.Value);
            }
            if (locationQuery.IsActiveSearch.HasValue)
            {
                query = query.Where(p => p.l.IsActive == locationQuery.IsActiveSearch.Value);
            }
            var locations = query.Select(p => new LocationListDto
            {
                SysId = p.l.SysId,
                Loc = p.l.Loc,
                LocUsage = p.l.LocUsage,
                LocUsageText = p.scd.Descr,
                ZoneSysId = p.l.ZoneSysId,
                LogicalLoc = p.l.LogicalLoc,
                IsActive = p.l.IsActive,
                CreateDate = p.l.CreateDate,
                ZoneCode = p.l.zone.ZoneCode,
                Status = p.l.Status
            }).Distinct();
            locationQuery.iTotalDisplayRecords = locations.Count();
            locations = locations.OrderBy(p => p.Loc).Skip(locationQuery.iDisplayStart).Take(locationQuery.iDisplayLength);
            return ConvertPages(locations, locationQuery);
        }
    }
}
