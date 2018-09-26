using Abp.EntityFramework;
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
    public class PackageRepository : CrudRepository, IPackageRepository
    {
        public PackageRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
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
                var code = uomQuery.UOMCode.Trim();
                query = query.Where(p => p.u.UOMCode == code);
            }

            var uoms = query.Select(p => new UOMDto()
            {
                SysId = p.u.SysId,
                UOMCode = p.u.UOMCode,
                Descr = p.u.Descr,
                UomType = p.u.UomType,
                UomTypeName = p.UomTypeName
            }).Distinct();

            uomQuery.iTotalDisplayRecords = uoms.Count();
            uoms = uoms.OrderByDescending(p => p.UOMCode).Skip(uomQuery.iDisplayStart).Take(uomQuery.iDisplayLength);
            return ConvertPages(uoms, uomQuery);
        }

        /// <summary>
        /// 根据商品Id查询所有包装信息
        /// 获取商品单位转换后数量, e.g: 将1 公斤 转换为 1000 g 作为库存保存
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <returns></returns>
        public List<SkuPackageConvertDto> GetSkuPackageList(List<Guid> skuSysId)
        {
            var query = from sku in Context.skus
                        join p in Context.packs on sku.PackSysId equals p.SysId into t1
                        from pk in t1.DefaultIfEmpty()
                        select new SkuPackageConvertDto
                        {
                            SysId = pk.SysId,
                            PackCode = pk.PackCode,
                            InLabelUnit01 = pk.InLabelUnit01,
                            FieldValue01 = pk.FieldValue01,
                            FieldValue02 = pk.FieldValue02,
                            SkuSysId = sku.SysId
                        };
            if (skuSysId != null && skuSysId.Count > 0)
            {
                query = query.Where(x => skuSysId.Contains(x.SkuSysId));
            }
            return query.ToList();
        }
    }
}
