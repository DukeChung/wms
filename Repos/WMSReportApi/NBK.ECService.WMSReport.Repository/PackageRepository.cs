using System;
using System.Linq;
using Abp.EntityFramework;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Package;
using NBK.ECService.WMSReport.Model;
using NBK.ECService.WMSReport.Repository.Interface;
using NBK.ECService.WMSReport.Utility;

namespace NBK.ECService.WMSReport.Repository
{
    public class PackageRepository : CrudRepository, IPackageRepository
    {

        public PackageRepository(IDbContextProvider<NBK_WMS_ReportContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public Pages<UOMDto> GetUOMList(UOMQuery uomQuery)
        {
            var query = from u in Context.uoms
                        join scd in Context.syscodedetails on u.UomType equals scd.Code
                        join s in Context.syscodes on scd.SysCodeSysId equals s.SysId
                        where s.SysCodeType.Equals("UOM", StringComparison.OrdinalIgnoreCase)
                        select new { u, UomTypeName = scd.Descr };
            if (!uomQuery.UOMCode.IsNull())
            {
                query = query.Where(p => p.u.UOMCode == uomQuery.UOMCode);
            }

            var uoms = query.Select(p => new UOMDto()
            {
                SysId = p.u.SysId,
                UOMCode = p.u.UOMCode,
                Descr = p.u.Descr,
                UomType = p.u.UomType,
                UomTypeName = p.UomTypeName
            }).Distinct().OrderByDescending(p => p.UOMCode);

            uomQuery.iTotalDisplayRecords = uoms.Count();
            return ConvertPages(uoms, uomQuery);
        }
    }
}