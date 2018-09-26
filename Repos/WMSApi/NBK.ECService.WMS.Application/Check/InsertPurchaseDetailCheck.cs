using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.ThirdParty;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.Application.Check
{
    public class InsertPurchaseDetailCheck : BaseCheck
    {
        private CommonResponse rsp = null;
        private ThirdPartyPurchaseDto purchaseDto = null;
        private List<sku> skuList = null;
        private List<skuclass> skuClassList = null;
        private List<pack> packList = null;
        private List<uom> uomList = null;

        public InsertPurchaseDetailCheck(CommonResponse rsp, ThirdPartyPurchaseDto purchaseDto, List<sku> skuList, List<skuclass> skuClassList, List<pack> packList, List<uom> uomList)
        {
            this.rsp = rsp;
            this.purchaseDto = purchaseDto;
            this.skuList = skuList;
            this.skuClassList = skuClassList;
            this.packList = packList;
            this.uomList = uomList;
        }

        public override CommonResponse Check()
        {
            //成品
            var product = false;
            //原材料
            var material = false;

            foreach (var purchaseDetailDto in purchaseDto.PurchaseDetailDtoList)
            {
                sku sku = skuList.FirstOrDefault(p => p.OtherId == purchaseDetailDto.OtherSkuId);
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

                #region 验证成品和原材料混合
                if(sku != null && sku.IsMaterial == true)
                {
                    material = true;
                    purchaseDto.Type = (int)PurchaseType.Material;
                }
                if(sku != null && sku.IsMaterial != true)
                {
                    product = true;
                }

                if (product == true && material == true)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "原材料和成品不能混合采购";
                    return rsp;
                }
                #endregion

                if (!InsertPurchaseDetailSingleCheck(rsp, sku, skuClass, pack, uom, purchaseDetailDto.OtherSkuId).IsSuccess)
                {
                    break;
                }
            }
            return rsp;
        }

        private CommonResponse InsertPurchaseDetailSingleCheck(CommonResponse rsp, sku sku, skuclass skuClass, pack pack, uom uom,string otherSkuId)
        {
            if (sku == null)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = "未找到对应的商品,Id"+ otherSkuId;
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
