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
    public class ComponentRepository : CrudRepository, IComponentRepository
    {
        /// <param name="dbContextProvider"></param>
        public ComponentRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        /// <summary>
        /// 获取组装件列表
        /// </summary>
        /// <param name="componentQuery"></param>
        /// <returns></returns>
        public Pages<ComponentListDto> GetComponentListByPaging(ComponentQuery componentQuery)
        {
            var query = from c in Context.components
                        join s in Context.skus on c.SkuSysId equals s.SysId
                        select new { c, s };
            if (!componentQuery.SkuCode.IsNull())
            {
                var code = componentQuery.SkuCode.Trim();
                query = query.Where(p => p.s.SkuCode.Contains(code));
            }

            if (!componentQuery.SkuName.IsNull())
            {
                var name = componentQuery.SkuName.Trim();
                query = query.Where(p => p.s.SkuName.Contains(name));
            }

            if (!componentQuery.UPC.IsNull())
            {
                var upc = componentQuery.UPC.Trim();
                query = query.Where(p => p.s.UPC == upc);
            }

            var components = query.Select(p => new ComponentListDto()
            {
                SysId = p.c.SysId,
                SkuSysId = p.c.SkuSysId,
                SkuCode = p.s.SkuCode,
                SkuName = p.s.SkuName,
                SkuDescr = p.s.SkuDescr,
                UPC = p.s.UPC,
                TimeConsuming = p.c.TimeConsuming,
                CreateDate = p.c.CreateDate
            }).Distinct();

            componentQuery.iTotalDisplayRecords = components.Count();
            components = components.OrderByDescending(p => p.CreateDate).Skip(componentQuery.iDisplayStart).Take(componentQuery.iDisplayLength);
            return ConvertPages(components, componentQuery);
        }

        /// <summary>
        /// 获取组装件
        /// </summary>
        /// <param name="componentQuery"></param>
        /// <returns></returns>
        public ComponentDto GetComponentById(ComponentQuery componentQuery)
        {
            var query = from c in Context.components
                        join s in Context.skus on c.SkuSysId equals s.SysId
                        where c.SysId == componentQuery.SysId
                        select new ComponentDto()
                        {
                            SysId = c.SysId,
                            SkuSysId = c.SkuSysId,
                            SkuCode = s.SkuCode,
                            SkuName = s.SkuName,
                            SkuDescr = s.SkuDescr,
                            UPC = s.UPC,
                            TimeConsuming = c.TimeConsuming
                        };

            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取组装件明细
        /// </summary>
        /// <param name="componentQuery"></param>
        /// <returns></returns>
        public List<ComponentDetailDto> GetComponentDetailList(ComponentQuery componentQuery)
        {
            var query = from cd in Context.componentdetails
                        join s in Context.skus on cd.SkuSysId equals s.SysId
                        join p in Context.packs on s.PackSysId equals p.SysId
                        join u in Context.uoms on p.FieldUom01 equals u.SysId
                        where cd.ComponentSysId == componentQuery.SysId
                        select new ComponentDetailDto()
                        {
                            SysId = cd.SysId,
                            SkuSysId = cd.SkuSysId,
                            SkuCode = s.SkuCode,
                            SkuName = s.SkuName,
                            SkuDescr = s.SkuDescr,
                            UPC = s.UPC,
                            Qty = cd.Qty,
                            LossQty = cd.LossQty,
                            IsMain = cd.IsMain,
                            UOMCode = u.UOMCode
                        };

            return query.ToList();
        }
    }
}
