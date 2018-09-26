using System;
using Abp.EntityFramework;
using System.Linq;
using Abp.EntityFramework.SimpleRepositories;
using FortuneLab.Models;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System.Collections.Generic;

namespace NBK.ECService.WMS.Repository
{
    public class SkuRepository : CrudRepository, ISkuRepository
    {
        /// <param name="dbContextProvider"></param>
        public SkuRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        /// <summary>
        /// 获取SKU列表
        /// </summary>
        /// <param name="skuQuery"></param>
        /// <returns></returns>
        public Pages<SkuListDto> GetSkuListByPaging(SkuQuery skuQuery)
        {
            var query = from s in Context.skus
                        join p in Context.packs on s.PackSysId equals p.SysId //into t1
                        join uom in Context.uoms on p.FieldUom01 equals uom.SysId
                        join l in Context.lottemplates on s.LotTemplateSysId equals l.SysId into t2
                        select new { s, uom.UOMCode };
            if (!skuQuery.SkuCodeSearch.IsNull())
            {
                var skucode = skuQuery.SkuCodeSearch.Trim();
                query = query.Where(p => p.s.SkuCode.Contains(skucode));
            }
            if (!skuQuery.SkuNameSearch.IsNull())
            {
                var skuName = skuQuery.SkuNameSearch.Trim();
                query = query.Where(p => p.s.SkuName.Contains(skuName));
            }
            if (!skuQuery.SkuDescrSearch.IsNull())
            {
                var descr = skuQuery.SkuDescrSearch.Trim();
                query = query.Where(p => p.s.SkuDescr.Contains(descr));
            }
            if (!skuQuery.UPC.IsNull())
            {
                var upc = skuQuery.UPC.Trim();
                query = query.Where(p => p.s.UPC.Equals(upc, StringComparison.OrdinalIgnoreCase));
            }
            if (skuQuery.IsActiveSearch.HasValue)
            {
                query = query.Where(p => p.s.IsActive == skuQuery.IsActiveSearch.Value);
            }
            if (!string.IsNullOrEmpty(skuQuery.UPCSearch))
            {
                var supc = skuQuery.UPCSearch.Trim();
                query = query.Where(p => p.s.UPC.Contains(supc));
            }
            if (!skuQuery.OtherIdSearch.IsNull())
            {
                var otherid = skuQuery.OtherIdSearch.Trim();
                query = query.Where(p => p.s.OtherId == otherid);
            }
            var skus = query.Select(p => new SkuListDto()
            {
                SysId = p.s.SysId,
                SkuCode = p.s.SkuCode,
                SkuName = p.s.SkuName,
                SkuDescr = p.s.SkuDescr,
                UPC = p.s.UPC,
                IsActive = p.s.IsActive,
                CreateDate = p.s.CreateDate,
                UOMCode = p.UOMCode,
                RecommendLoc = p.s.RecommendLoc,
                OtherId = p.s.OtherId
            }).Distinct();
            skuQuery.iTotalDisplayRecords = skus.Count();
            skus = skus.OrderByDescending(p => p.CreateDate).Skip(skuQuery.iDisplayStart).Take(skuQuery.iDisplayLength);
            return ConvertPages(skus, skuQuery);
        }

        public List<SkuWithPackDto> GetSkuPackListByUPC(string upc)
        {
            List<SkuWithPackDto> response = new List<SkuWithPackDto>();
            var query1 = from pack in Context.packs
                         join sku in Context.skus on pack.SysId equals sku.PackSysId
                         where pack.UPC02 == upc
                         select new SkuWithPackDto()
                         {
                             SysId = sku.SysId,
                             SkuCode = sku.SkuCode,
                             SkuName = sku.SkuName,
                             SkuDescr = sku.SkuDescr,
                             UPC = sku.UPC,
                             TypeDisplay = "包装",
                             PackQty = pack.FieldValue02.Value
                         };

            var query2 = from pack in Context.packs
                         join sku in Context.skus on pack.SysId equals sku.PackSysId
                         where pack.UPC03 == upc
                         select new SkuWithPackDto()
                         {
                             SysId = sku.SysId,
                             SkuCode = sku.SkuCode,
                             SkuName = sku.SkuName,
                             SkuDescr = sku.SkuDescr,
                             UPC = sku.UPC,
                             TypeDisplay = "包装",
                             PackQty = pack.FieldValue03.Value
                         };

            var query3 = from pack in Context.packs
                         join sku in Context.skus on pack.SysId equals sku.PackSysId
                         where pack.UPC04 == upc
                         select new SkuWithPackDto()
                         {
                             SysId = sku.SysId,
                             SkuCode = sku.SkuCode,
                             SkuName = sku.SkuName,
                             SkuDescr = sku.SkuDescr,
                             UPC = sku.UPC,
                             TypeDisplay = "包装",
                             PackQty = pack.FieldValue04.Value
                         };

            var query4 = from pack in Context.packs
                         join sku in Context.skus on pack.SysId equals sku.PackSysId
                         where pack.UPC05 == upc
                         select new SkuWithPackDto()
                         {
                             SysId = sku.SysId,
                             SkuCode = sku.SkuCode,
                             SkuName = sku.SkuName,
                             SkuDescr = sku.SkuDescr,
                             UPC = sku.UPC,
                             TypeDisplay = "包装",
                             PackQty = pack.FieldValue05.Value
                         };

            response.AddRange(query1.ToList());

            response.AddRange(query2.ToList());

            response.AddRange(query3.ToList());

            response.AddRange(query4.ToList());

            return response;
        }
    }
}