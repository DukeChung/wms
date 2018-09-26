using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using FortuneLab.WebApiClient;
using NBK.ECService.WMS.Utility.Enum;
using Newtonsoft.Json;

namespace NBK.ECService.WMS.Application
{
    public class SkuAppService : WMSApplicationService, ISkuAppService
    {
        private ISkuRepository _crudRepository = null;


        public SkuAppService(ISkuRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        /// <summary>
        /// 获取SKU列表
        /// </summary>
        /// <param name="skuQuery"></param>
        /// <returns></returns>
        public Pages<SkuListDto> GetSkuList(SkuQuery skuQuery)
        {
            return _crudRepository.GetSkuListByPaging(skuQuery);
        }

        /// <summary>
        /// 新增SKU
        /// </summary>
        /// <param name="skuDto"></param>
        /// <returns></returns>
        public void AddSku(SkuDto skuDto)
        {
            if (_crudRepository.GetQuery<sku>(p => p.SkuCode == skuDto.SkuCode).FirstOrDefault() != null)
            {
                throw new Exception("商品编号已存在");
            }
            skuDto.SysId = Guid.NewGuid();
            skuDto.CreateDate = DateTime.Now;
            skuDto.UpdateDate = DateTime.Now;
            
            var sku = skuDto.TransformTo<sku>();
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateSku", method: MethodType.Post, postData: sku);
                }).Start();
            }
            
            _crudRepository.Insert(sku);
            
        }

        /// <summary>
        /// 根据Id获取SKU
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public SkuDto GetSkuById(Guid sysId)
        {
            SkuDto skuDto = _crudRepository.Get<sku>(sysId).TransformTo<SkuDto>();
            skuclass skuClass = _crudRepository.Get<skuclass>(skuDto.SkuClassSysId);
            if (skuClass != null)
            {
                skuDto.SkuClassName = skuClass.SkuClassName;
            }
            return skuDto;
        }

        /// <summary>
        /// 编辑SKU
        /// </summary>
        /// <param name="skuDto"></param>
        public void EditSku(SkuDto skuDto)
        {
            if (_crudRepository.GetQuery<sku>(p => p.SysId != skuDto.SysId && p.SkuCode == skuDto.SkuCode).FirstOrDefault() != null)
            {
                throw new Exception("商品编号已存在");
            }
            skuDto.UpdateDate = DateTime.Now;
            var sku = skuDto.TransformTo<sku>();
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateSkuForWMS", method: MethodType.Post, postData: sku);
                }).Start();
            }
            
            _crudRepository.Update(sku);
         
        }

        /// <summary>
        /// 删除SKU
        /// </summary>
        /// <param name="sysIds"></param>
        public void DeleteSku(List<Guid> sysIds)
        {
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncDeleteSkuForWMS", method: MethodType.Post, postData: sysIds);
                }).Start();
            }

            _crudRepository.Delete<sku>(sysIds);
        }

        /// <summary>
        /// 根据商品条码获取商品数据
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        public SkuDto GetSkuByUPC(string upc)
        {
            var sku = _crudRepository.GetQuery<sku>(x => x.UPC == upc).FirstOrDefault();
            return sku.JTransformTo<SkuDto>();
        }

        /// <summary>
        /// 根据商品条码获取商品数据
        /// </summary>
        /// <param name="upc"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public SkuDto GetSkuByUPC(string upc, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var sku = _crudRepository.GetQuery<sku>(x => x.UPC == upc).FirstOrDefault();
            return sku.JTransformTo<SkuDto>();
        }

        public List<SkuWithPackDto> GetSkuAndSkuPackListByUPC(DuplicateUPCChooseQuery request)
        {
            var lambda = Wheres.Lambda<sku>();
            if (request != null)
            {
                if (!request.SkuName.IsNull())
                {
                    lambda = lambda.And(p => p.SkuName == request.SkuName);
                }
                if (!request.UPC.IsNull())
                {
                    lambda = lambda.And(p => p.UPC == request.UPC);
                }
                if (!request.SkuCode.IsNull())
                {
                    lambda = lambda.And(p => p.SkuCode == request.SkuCode);
                }
            }


            var skuList = _crudRepository.GetQuery<sku>(lambda).ToList();
            var response = skuList.JTransformTo<SkuWithPackDto>();
            response.ForEach(p =>
            {
                p.PackQty = 1;
                p.TypeDisplay = "商品";
            });

            if (!string.IsNullOrEmpty(request.UPC))
            {//存在upc再去查找包装
                var packSkuList = _crudRepository.GetSkuPackListByUPC(request.UPC);
                if (packSkuList != null && packSkuList.Count > 0)
                {
                    response.AddRange(packSkuList);
                }
            }

            if (request.ExcludeSkuSysId != null)
            {
                var query = from result in response
                            join skuSysId in request.ExcludeSkuSysId on result.SysId.ToString() equals skuSysId
                            select result;

                return query.Distinct().ToList();
            }

            return response;
        }
    }
}
