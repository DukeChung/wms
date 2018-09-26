using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.UI;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Package;
using NBK.ECService.WMSReport.Model.Models;
using NBK.ECService.WMSReport.Repository.Interface;
using NBK.ECService.WMSReport.Utility;

namespace NBK.ECService.WMSReport.Application
{
    public class PackageAppService : ApplicationService, IPackageAppService
    {
        private IPackageRepository _crudRepository = null;
        public PackageAppService(IPackageRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }
 

        #region pack management

        /// <summary>
        /// 获取商品单位转换后数量, e.g: 将1 公斤 转换为 1000 g 作为库存保存
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="requestQty"></param>
        /// <returns></returns>
        public bool GetSkuConversiontransQty(Guid skuSysId, int requestQty, out int responseQty, ref pack pack)
        {
            bool result = false;
            responseQty = requestQty;

            var sku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
            if (sku != null)
            {
                pack = _crudRepository.Get<pack>(sku.PackSysId);

                if (pack.InLabelUnit01.HasValue && pack.InLabelUnit01.Value == true)
                {
                    if (pack.FieldValue01 > 0 && pack.FieldValue02 > 0)
                    {
                        responseQty = ((pack.FieldValue01.Value * requestQty) / pack.FieldValue02.Value);
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取商品单位转换后数量, e.g: 将1 公斤 转换为 1000 g 作为库存保存
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="requestQty"></param>
        /// <returns></returns>
        public bool GetSkuConversiontransQty(Guid skuSysId, decimal requestQty, out int responseQty, ref pack pack)
        {
            bool result = false;
            responseQty = 0;

            var sku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
            if (sku != null)
            {
                pack = _crudRepository.Get<pack>(sku.PackSysId);

                if ((pack.InLabelUnit01.HasValue && pack.InLabelUnit01.Value == true)
                    || (pack.OutLabelUnit01.HasValue && pack.OutLabelUnit01.Value == true))
                {
                    if (pack.FieldValue01 > 0 && pack.FieldValue02 > 0)
                    {
                        responseQty = Convert.ToInt32(((pack.FieldValue01.Value * requestQty) / pack.FieldValue02.Value));
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取商品单位 反向 转换后数量, e.g: 将1000 g 转换为 1 公斤 作为库存展示
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="requestQty"></param>
        /// <param name="responseQty"></param>
        /// <returns></returns>
        public bool GetSkuDeconversiontransQty(Guid skuSysId, int requestQty, out decimal responseQty)
        {
            bool result = false;
            responseQty = requestQty;

            var sku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
            if (sku != null)
            {
                var pack = _crudRepository.Get<pack>(sku.PackSysId);

                if (pack.InLabelUnit01.HasValue && pack.InLabelUnit01.Value == true)
                {
                    if (pack.FieldValue01 > 0 && pack.FieldValue02 > 0)
                    {
                        responseQty = Math.Round(((pack.FieldValue02.Value * requestQty * 1.00m) / pack.FieldValue01.Value), 3);
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取商品单位 反向 转换后数量, e.g: 将1000 g 转换为 1 公斤 作为库存展示
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="requestQty"></param>
        /// <param name="responseQty"></param>
        /// <returns></returns>
        public bool GetSkuDeconversiontransQty(Guid skuSysId, int requestQty, out decimal responseQty, ref uom uom)
        {
            bool result = false;
            responseQty = requestQty;

            var sku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
            if (sku != null)
            {
                var pack = _crudRepository.Get<pack>(sku.PackSysId);

                if (pack.InLabelUnit01.HasValue && pack.InLabelUnit01.Value == true)
                {
                    if (pack.FieldValue01 > 0 && pack.FieldValue02 > 0)
                    {
                        uom = _crudRepository.Get<uom>(pack.FieldUom02.GetValueOrDefault());
                        responseQty = Math.Round(((pack.FieldValue02.Value * requestQty * 1.00m) / pack.FieldValue01.Value), 3);
                        result = true;
                    }
                }

                if (result == false)
                {
                    uom = _crudRepository.Get<uom>(pack.FieldUom01.GetValueOrDefault());
                }
            }

            return result;
        }

        #endregion
    }
}