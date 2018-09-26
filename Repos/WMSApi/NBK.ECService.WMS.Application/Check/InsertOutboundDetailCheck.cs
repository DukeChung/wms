using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.ThirdParty;
using NBK.ECService.WMS.Model.Models;

namespace NBK.ECService.WMS.Application.Check
{
    public class InsertOutboundDetailCheck : BaseCheck
    {
        private CommonResponse rsp = null;
        private List<ThirdPartyOutboundDetailDto> outboundDetailDtoList = null;
        private List<sku> skuList = null;
        private List<skuclass> skuClassList = null;
        private List<pack> packList = null;
        private List<uom> uomList = null;

        public InsertOutboundDetailCheck(CommonResponse rsp, List<ThirdPartyOutboundDetailDto> outboundDetailDtoList, List<sku> skuList, List<skuclass> skuClassList, List<pack> packList, List<uom> uomList)
        {
            this.rsp = rsp;
            this.outboundDetailDtoList = outboundDetailDtoList;
            this.skuList = skuList;
            this.skuClassList = skuClassList;
            this.packList = packList;
            this.uomList = uomList;
        }

        public override CommonResponse Check()
        {
            foreach (var outboundDetailDto in outboundDetailDtoList)
            {
                sku sku = skuList.FirstOrDefault(p => p.OtherId == outboundDetailDto.OtherSkuId);
                skuclass skuClass = null;
                pack pack = null;
                uom uom = null;
                if (sku != null)
                {
                    skuClass = skuClassList.FirstOrDefault(p => p.SysId == sku.SkuClassSysId);
                    pack = packList.FirstOrDefault(p => p.SysId == sku.PackSysId);
                    if (pack != null)
                    {
                        uom = uomList.FirstOrDefault(p => p.SysId == pack.FieldUom01);
                    }
                }
                if (!InsertOutboundDetailSingleCheck(rsp, sku, skuClass, pack, uom, outboundDetailDto.OtherSkuId).IsSuccess)
                {
                    break;
                }
            }
            return rsp;
        }

        private CommonResponse InsertOutboundDetailSingleCheck(CommonResponse rsp, sku sku, skuclass skuClass, pack pack, uom uom,string otherId)
        {
            if (sku == null)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = "未找到对应的商品,Id" + otherId;
                return rsp;
            }
            //if (skuClass == null)
            //{
            //    rsp.IsSuccess = false;
            //    rsp.ErrorMessage = "未找到对应的商品分类";
            //    return rsp;
            //}
            if (pack == null)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = "未找到对应的包装";
                return rsp;
            }
            if (uom == null)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = "未找到对应的单位";
                return rsp;
            }
            return rsp;
        }
    }
}
