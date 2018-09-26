using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository
{
    public class AssemblyRepository : CrudRepository, IAssemblyRepository
    {
        public AssemblyRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider) : base(dbContextProvider) { }

        public Pages<AssemblyListDto> GetAssemblyList(AssemblyQuery assemblyQuery)
        {
            var query = from a in Context.assemblies
                        join s in Context.skus on a.SkuSysId equals s.SysId
                        select new { a, s };
            query = query.Where(p => p.a.WareHouseSysId == assemblyQuery.WarehouseSysId);
            if (!assemblyQuery.AssemblyOrderSearch.IsNull())
            {
                assemblyQuery.AssemblyOrderSearch = assemblyQuery.AssemblyOrderSearch.Trim();
                query = query.Where(p => p.a.AssemblyOrder == assemblyQuery.AssemblyOrderSearch);
            }
            if (!assemblyQuery.SkuNameSearch.IsNull())
            {
                assemblyQuery.SkuNameSearch = assemblyQuery.SkuNameSearch.Trim();
                query = query.Where(p => p.s.SkuName.Contains(assemblyQuery.SkuNameSearch));
            }
            if (!string.IsNullOrEmpty(assemblyQuery.Channel))
            {
                assemblyQuery.Channel = assemblyQuery.Channel.Trim();
                query = query.Where(p => p.a.Channel.Contains(assemblyQuery.Channel));
            }
            if (!assemblyQuery.SkuUPCSearch.IsNull())
            {
                assemblyQuery.SkuUPCSearch = assemblyQuery.SkuUPCSearch.Trim();
                query = query.Where(p => p.s.UPC == assemblyQuery.SkuUPCSearch);
            }
            if (assemblyQuery.StatusSearch.HasValue)
            {
                query = query.Where(p => p.a.Status == assemblyQuery.StatusSearch);
            }
            if (assemblyQuery.PlanProcessingDateSearch.HasValue)
            {
                DateTime fromDateTime = assemblyQuery.PlanProcessingDateSearch.Value.Date;
                DateTime toDateTime = assemblyQuery.PlanProcessingDateSearch.Value.Date.AddDays(1).AddMilliseconds(-1);
                query = query.Where(p => fromDateTime <= p.a.PlanProcessingDate && p.a.PlanProcessingDate <= toDateTime);
            }
            if (assemblyQuery.PlanCompletionDateSearch.HasValue)
            {
                DateTime fromDateTime = assemblyQuery.PlanCompletionDateSearch.Value.Date;
                DateTime toDateTime = assemblyQuery.PlanCompletionDateSearch.Value.Date.AddDays(1).AddMilliseconds(-1);
                query = query.Where(p => fromDateTime <= p.a.PlanCompletionDate && p.a.PlanCompletionDate <= toDateTime);
            }
            if (assemblyQuery.ActualProcessingDateSearch.HasValue)
            {
                DateTime fromDateTime = assemblyQuery.ActualProcessingDateSearch.Value.Date;
                DateTime toDateTime = assemblyQuery.ActualProcessingDateSearch.Value.Date.AddDays(1).AddMilliseconds(-1);
                query = query.Where(p => fromDateTime <= p.a.ActualProcessingDate && p.a.ActualProcessingDate <= toDateTime);
            }
            if (assemblyQuery.ActualCompletionDateSearch.HasValue)
            {
                DateTime fromDateTime = assemblyQuery.ActualCompletionDateSearch.Value.Date;
                DateTime toDateTime = assemblyQuery.ActualCompletionDateSearch.Value.Date.AddDays(1).AddMilliseconds(-1);
                query = query.Where(p => fromDateTime <= p.a.ActualCompletionDate && p.a.ActualCompletionDate <= toDateTime);
            }
            if (!assemblyQuery.ExternalOrderSearch.IsNull())
            {
                assemblyQuery.ExternalOrderSearch = assemblyQuery.ExternalOrderSearch.Trim();
                query = query.Where(p => p.a.ExternalOrder == assemblyQuery.ExternalOrderSearch);
            }
            var assemblies = query.Select(p => new AssemblyListDto
            {
                SysId = p.a.SysId,
                AssemblyOrder = p.a.AssemblyOrder,
                SkuSysId = p.s.SysId,
                SkuName = p.s.SkuName,
                Status = p.a.Status,
                PlanQty = p.a.PlanQty,
                ActualQty = p.a.ActualQty,
                PlanProcessingDate = p.a.PlanProcessingDate,
                PlanCompletionDate = p.a.PlanCompletionDate,
                ActualProcessingDate = p.a.ActualProcessingDate,
                ActualCompletionDate = p.a.ActualCompletionDate,
                CreateDate = p.a.CreateDate,
                ExternalOrder = p.a.ExternalOrder,
                Channel = p.a.Channel
            }).Distinct();
            assemblyQuery.iTotalDisplayRecords = assemblies.Count();
            assemblies = assemblies.OrderByDescending(p => p.CreateDate).Skip(assemblyQuery.iDisplayStart).Take(assemblyQuery.iDisplayLength);
            return ConvertPages(assemblies, assemblyQuery);
        }

        public AssemblyViewDto GetAssemblyViewDtoById(Guid sysId)
        {
            var query = from a in Context.assemblies
                        join s in Context.skus on a.SkuSysId equals s.SysId
                        join pack in Context.packs on s.PackSysId equals pack.SysId into t
                        from p in t.DefaultIfEmpty()
                        join w in Context.warehouses on a.WareHouseSysId equals w.SysId
                        where a.SysId == sysId
                        select new { a, s, p, w };
            AssemblyViewDto assemblyViewDto = query.Select(p => new AssemblyViewDto
            {
                SysId = p.a.SysId,
                AssemblyOrder = p.a.AssemblyOrder,
                SkuSysId = p.a.SkuSysId,
                SkuCode = p.s.SkuCode,
                SkuName = p.s.SkuName,
                SkuUPC = p.s.UPC,
                Status = p.a.Status,
                Remark = p.a.Remark,
                PlanProcessingDate = p.a.PlanProcessingDate,
                PlanCompletionDate = p.a.PlanCompletionDate,
                PlanQty = p.a.PlanQty,
                ActualQty = p.a.ActualQty,
                ActualProcessingDate = p.a.ActualProcessingDate,
                ActualCompletionDate = p.a.ActualCompletionDate,
                WareHouseSysId = p.a.WareHouseSysId,
                WareHouseName = p.w.Name,
                Packing = p.a.Packing,
                PackWeight = p.a.PackWeight,
                PackGrade = p.a.PackGrade,
                StorageConditions = p.a.StorageConditions,
                PackSpecification = p.a.PackSpecification,
                PackDescr = p.a.PackDescr,
                Source = p.a.Source,
                UPC02 = p.p.UPC02,
                UPC03 = p.p.UPC03,
                UPC04 = p.p.UPC04,
                UPC05 = p.p.UPC05,
                FieldValue02 = p.p.FieldValue02,
                FieldValue03 = p.p.FieldValue03,
                FieldValue04 = p.p.FieldValue04,
                FieldValue05 = p.p.FieldValue05,
                ShelvesQty = p.a.ShelvesQty,
                ShelvesStatus = p.a.ShelvesStatus
            }).FirstOrDefault();
            if (assemblyViewDto != null)
            {
                var detailQuery = from ad in Context.assemblydetails
                                  join s in Context.skus on ad.SkuSysId equals s.SysId

                                  join pack in Context.packs on s.PackSysId equals pack.SysId into t1
                                  from p in t1.DefaultIfEmpty()

                                  join uom in Context.uoms on p.FieldUom02 equals uom.SysId into t2
                                  from u in t2.DefaultIfEmpty()

                                      //join p in Context.packs on s.PackSysId equals p.SysId
                                  //join u in Context.uoms on p.FieldUom02 equals u.SysId
                                  where ad.AssemblySysId == assemblyViewDto.SysId
                                  select new { ad, s, p, uomCode = u.UOMCode };
                assemblyViewDto.AssemblyDetails = detailQuery.Select(p => new AssemblyDetailDto
                {
                    SysId = p.ad.SysId,
                    AssemblySysId = p.ad.AssemblySysId,
                    SkuSysId = p.ad.SkuSysId,
                    SkuCode = p.s.SkuCode,
                    SkuUPC = p.s.UPC,
                    SkuName = p.s.SkuName,
                    UOMCode = p.uomCode,
                    UnitQty = p.p.InLabelUnit01.HasValue && p.p.InLabelUnit01.Value == true
                                && p.p.FieldValue01 > 0 && p.p.FieldValue02 > 0
                                ? Math.Round(((p.p.FieldValue02.Value * p.ad.UnitQty * 1.00m) / p.p.FieldValue01.Value), 3) : p.ad.UnitQty, //p.ad.UnitQty,
                    Qty = p.ad.Qty,
                    LossQty = p.p.InLabelUnit01.HasValue && p.p.InLabelUnit01.Value == true
                                && p.p.FieldValue01 > 0 && p.p.FieldValue02 > 0
                                ? Math.Round(((p.p.FieldValue02.Value * p.ad.LossQty * 1.00m) / p.p.FieldValue01.Value), 3) : p.ad.LossQty,
                    //p.ad.LossQty,
                    Status = p.ad.Status,
                    Grade = p.ad.Grade,
                    UPC02 = p.p.UPC02,
                    UPC03 = p.p.UPC03,
                    UPC04 = p.p.UPC04,
                    UPC05 = p.p.UPC05,
                    FieldValue02 = p.p.FieldValue02,
                    FieldValue03 = p.p.FieldValue03,
                    FieldValue04 = p.p.FieldValue04,
                    FieldValue05 = p.p.FieldValue05
                }).Distinct().OrderBy(p => p.SkuUPC).ToList();
            }
            return assemblyViewDto;
        }

        public Pages<AssemblySkuDto> GetSkuListForAssembly(AssemblySkuQuery request)
        {
            var query = from sku in Context.skus
                        select new { sku };

            if (!string.IsNullOrEmpty(request.SkuName))
            {
                request.SkuName = request.SkuName.Trim();
                query = query.Where(p => p.sku.SkuName.Contains(request.SkuName));
            }

            if (!string.IsNullOrEmpty(request.UPC))
            {
                request.UPC = request.UPC.Trim();
                query = query.Where(p => p.sku.UPC == request.UPC );
            }

            var response = query.Select(p => new AssemblySkuDto()
            {
                SysId = p.sku.SysId,
                SkuSysId = p.sku.SysId,
                SkuName = p.sku.SkuName,
                UPC = p.sku.UPC
            });

            request.iTotalDisplayRecords = response.Count();
            response = response.OrderBy(p => p.SkuName).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(response, request);
        }

        public Pages<AssemblyWeightSkuDto> GetWeighSkuListForAssembly(AssemblyWeightSkuQuery request)
        {
            var query = from skuWeight in Context.assemblyskuweight
                        where skuWeight.AssemblySysId == request.AssemblySysId
                            && skuWeight.SkuSysId == request.SkuSysId
                            && skuWeight.WarehouseSysId == request.WarehouseSysId
                        select new { skuWeight };

            var response = query.Select(p => new AssemblyWeightSkuDto()
            {
                //Index = index + 1,
                Weight = p.skuWeight.Weight,
                CreateDate = p.skuWeight.CreateDate,
                CreateUserName = p.skuWeight.CreateUserName
            });

            request.iTotalDisplayRecords = response.Count();
            response = response.OrderByDescending(p => p.CreateDate).Skip(request.iDisplayStart).Take(request.iDisplayLength);
            return ConvertPages(response, request);
        }
    }
}
