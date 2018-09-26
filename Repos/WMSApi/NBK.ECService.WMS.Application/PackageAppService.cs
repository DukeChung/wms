using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Model.Models;
using Abp.UI;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.Utility.Redis;
using NBK.ECService.WMS.Utility.Enum;
using FortuneLab.WebApiClient;

namespace NBK.ECService.WMS.Application
{
    public class PackageAppService : WMSApplicationService, IPackageAppService
    {
        private IPackageRepository _crudRepository = null;
        public PackageAppService(IPackageRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        public Pages<UOMDto> GetUOMList(UOMQuery query)
        {
            return _crudRepository.GetUOMList(query);
        }

        public UOMDto GetUOMBySysId(Guid sysId)
        {
            var uom = _crudRepository.Get<uom>(sysId);
            return uom.TransformTo<UOMDto>();
        }

        public void UpdateUOM(UOMDto uomDto)
        {
            var uom = uomDto.TransformTo<uom>();
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateUOM", method: MethodType.Post, postData: uom);
                }).Start();
            }
            
            _crudRepository.Update(uom);
            
        }

        public void AddUOM(UOMDto uomDto)
        {
            if (_crudRepository.FirstOrDefault<uom>(p => p.UOMCode.Equals(uomDto.UOMCode, StringComparison.OrdinalIgnoreCase)) != null)
            {
                throw new UserFriendlyException($"已存在单位代码为'{uomDto.UOMCode}'的单位信息，请检查");
            }
            uomDto.SysId = Guid.NewGuid();

            var uom = uomDto.TransformTo<uom>();
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateUOM", method: MethodType.Post, postData: uom);
                }).Start();
            }
            
            _crudRepository.InsertAndGetId(uom);
            
        }

        public void DeleteUOM(string sysIdList)
        {
            var sysIds = sysIdList.ToGuidList();
            foreach (var sysId in sysIds)
            {
                if (_crudRepository.FirstOrDefault<pack>(p =>
                    (p.FieldUom01.HasValue && p.FieldUom01.Value == sysId) ||
                    (p.FieldUom02.HasValue && p.FieldUom02.Value == sysId) ||
                    (p.FieldUom03.HasValue && p.FieldUom03.Value == sysId)) != null)
                {
                    var uom = _crudRepository.Get<uom>(sysId);
                    throw new UserFriendlyException($"计量单位 '{uom.UOMCode}' 已被包装信息使用，不能删除，请检查");
                }
            }
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncDeleteUOM", method: MethodType.Post, postData: sysIds);
                }).Start();
            }
            
            _crudRepository.Delete<uom>(sysIds);
            
        }

        #region pack management

        public Pages<PackDto> GetPackList(PackQuery query)
        {
            #region 拼凑条件
            var lambda = Wheres.Lambda<pack>();
            if (query != null)
            {
                if (!query.PackCode.IsNull())
                {
                    var code = query.PackCode.Trim();
                    lambda = lambda.And(x => x.PackCode.Contains(code));
                }
            }

            #endregion
            return _crudRepository.GetQueryableByPage<pack, PackDto>(query, lambda);
        }

        public PackDto GetPackBySysId(Guid sysId)
        {
            var pack = _crudRepository.Get<pack>(sysId);
            return pack.TransformTo<PackDto>();
        }

        public void UpdatePack(PackDto packDto)
        {
            var pack = packDto.TransformTo<pack>();
            pack.PackCode = pack.PackCode.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            pack.UpdateDate = DateTime.Now;
            pack.UpdateUserName = packDto.CurrentDisplayName;
            pack.UpdateBy = packDto.CurrentUserId;
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdatePack", method: MethodType.Post, postData: pack);
                }).Start();
            }
            
            _crudRepository.Update(pack);
            
        }

        public void AddPack(PackDto packDto)
        {
            packDto.SysId = Guid.NewGuid();
            var pack = packDto.TransformTo<pack>();
            pack.PackCode = pack.PackCode.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            pack.CreateDate = DateTime.Now;
            pack.CreateUserName = packDto.CurrentDisplayName;
            pack.CreateBy = packDto.CurrentUserId;
            pack.UpdateDate = DateTime.Now;
            pack.UpdateUserName = packDto.CurrentDisplayName;
            pack.UpdateBy = packDto.CurrentUserId;
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreatePack", method: MethodType.Post, postData: pack);
                }).Start();
            }
            
            _crudRepository.InsertAndGetId(pack);
            
        }

        public void DeletePack(string sysIdList)
        {
            var sysIds = sysIdList.ToGuidList();
            foreach (var sysId in sysIds)
            {
                //后期根据业务扩展补充删除的验证条件

                if (_crudRepository.FirstOrDefault<sku>(p => p.PackSysId == sysId) != null)
                {
                    var pack = _crudRepository.Get<pack>(sysId);
                    throw new UserFriendlyException($"包装 '{pack.PackCode}' 已有商品使用，不能删除，请检查");
                }
            }
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncDeletePack", method: MethodType.Post, postData: sysIds);
                }).Start();
            }
            
            _crudRepository.Delete<pack>(sysIds);
            
        }

        public List<SelectItem> GetSelectPack(string packCode)
        {
            var packList = _crudRepository.GetAll<pack>();
            if (!string.IsNullOrEmpty(packCode))
            {
                packList = packList.Where(x => x.PackCode.Contains(packCode));
            }

            var list = new List<SelectItem>();
            packList.ToList().ForEach(info =>
            {
                var item = new SelectItem
                {
                    Text = info.PackCode,
                    Value = info.SysId.ToString()
                };
                list.Add(item);
            });
            return list;
        }

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

        /// <summary>
        /// 根据商品包赚信息获取单位转化后的数据
        /// </summary>
        /// <param name="request">需要转化的商品信息</param>
        /// <param name="response"></param>
        public void GetSkuConversionQty(ref SkuPackageConvertDto request)
        {
            if (request.Flag == (int)ReceiptConvert.ToMaterial)
            {   //成品转原材料
                if (request.InLabelUnit01.HasValue && request.InLabelUnit01.Value)
                {
                    if (request.FieldValue01 > 0 && request.FieldValue02 > 0)
                    {
                        request.BaseQty = Convert.ToInt32(((request.FieldValue01.Value * request.UnitQty) / request.FieldValue02.Value));

                        request.result = true;
                    }
                    else
                    {
                        request.result = false;
                        request.BaseQty = Convert.ToInt32(request.UnitQty);
                    }
                }
                else
                {
                    request.result = false;
                    request.BaseQty = Convert.ToInt32(request.UnitQty);
                }
            }
            else
            {
                if (request.InLabelUnit01.HasValue && request.InLabelUnit01.Value)
                {
                    if (request.FieldValue01 > 0 && request.FieldValue02 > 0)
                    {
                        request.UnitQty = ((request.FieldValue01.Value * request.BaseQty) / request.FieldValue02.Value);

                        request.result = true;
                    }
                    else
                    {
                        request.result = false;
                        request.UnitQty = request.BaseQty;
                    }
                }
                else
                {
                    request.result = false;
                    request.UnitQty = request.BaseQty;
                }
            }
        }
    }
}
