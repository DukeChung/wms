using Abp.EntityFramework;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NBK.ECService.WMS.Repository
{
    public class RFBasicsRepository : CrudRepository, IRFBasicsRepository
    {
        public RFBasicsRepository(IDbContextProvider<NBK_WMSContext> dbContextProvider)
                : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 根据UPC获取商品和包装
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        public List<SkuPackDto> GetSkuListByUPC(string upc)
        {
            var query = from s in Context.skus
                        where s.UPC == upc
                        select new SkuPackDto
                        {
                            SkuSysId = s.SysId,
                            UPC = s.UPC,
                            SkuName = s.SkuName
                        };
            return query.ToList();
        }

        /// <summary>
        /// 根据skusysId获取包装信息
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <returns></returns>
        public RFPackDto GetPackBySkuSysId(Guid skuSysId)
        {
            var query = from p in Context.packs
                        join s in Context.skus on p.SysId equals s.PackSysId into t0
                        from p1 in t0.DefaultIfEmpty()
                        where p1.SysId == skuSysId
                        select new RFPackDto
                        {
                            SysId = p.SysId,
                            PackCode = p.PackCode,
                            FieldUom01 = p.FieldUom01,
                            FieldValue01 = p.FieldValue01,
                            InLabelUnit01 = p.InLabelUnit01,
                            FieldUom03 = p.FieldUom03,
                            FieldValue03 = p.FieldValue03,
                            UPC01 = p.UPC01,
                            UPC03 = p.UPC03
                        };

            return query.FirstOrDefault();
        }

        /// <summary>
        /// 根据UPC获取包装信息
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        public List<RFPackDto> GetPackListByUPC(string upc)
        {
            var query = from p in Context.packs
                        where p.UPC03 == upc
                        select new RFPackDto
                        {
                            SysId = p.SysId,
                            PackCode = p.PackCode,
                            FieldUom01 = p.FieldUom01,
                            FieldValue01 = p.FieldValue01,
                            InLabelUnit01 = p.InLabelUnit01,
                            FieldUom03 = p.FieldUom03,
                            FieldValue03 = p.FieldValue03,
                            UPC01 = p.UPC01,
                            UPC03 = p.UPC03
                        };

            return query.ToList();
        }

    }
}
