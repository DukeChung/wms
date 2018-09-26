using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;

namespace NBK.ECService.WMS.Application.Check
{
    public class InsertOrUpdateSkuCheck : BaseCheck
    {
        private CommonResponse rsp = null;
        private string UPC = null;
        private pack pack = null;
        private skuclass skuClass = null;

        public InsertOrUpdateSkuCheck(CommonResponse rsp, string UPC, pack pack, skuclass skuClass)
        {
            this.rsp = rsp;
            this.UPC = UPC;
            this.pack = pack;
            this.skuClass = skuClass;
        }

        public override CommonResponse Check()
        {
            if (pack == null)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = "未找到对应的包装";
                return rsp;
            }
            if (skuClass == null)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = "未找到对应的商品分类";
                return rsp;
            }
            return rsp;
        }
    }
}
