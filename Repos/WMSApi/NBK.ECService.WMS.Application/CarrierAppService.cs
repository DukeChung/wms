using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using FortuneLab.WebApiClient;

namespace NBK.ECService.WMS.Application
{
    public class CarrierAppService : WMSApplicationService, ICarrierAppService
    {
        private ICrudRepository _crudRepository = null;

        public CarrierAppService(ICrudRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        /// <summary>
        /// 获取承运商列表
        /// </summary>
        /// <param name="carrierQuery"></param>
        /// <returns></returns>
        public Pages<CarrierDto> GetCarrierList(CarrierQuery carrierQuery)
        {
            var lambda = Wheres.Lambda<carrier>();
            if (carrierQuery != null)
            {
                if (!carrierQuery.CarrierNameSearch.IsNull())
                {
                    var name = carrierQuery.CarrierNameSearch.Trim();
                    lambda = lambda.And(p => p.CarrierName.Contains(name));
                }
                if (!carrierQuery.CarrierPhoneSearch.IsNull())
                {
                    lambda = lambda.And(p => p.CarrierPhone == carrierQuery.CarrierPhoneSearch.Trim());
                }
                if (!carrierQuery.CarrierContactsSearch.IsNull())
                {
                    var carrier = carrierQuery.CarrierContactsSearch.Trim();
                    lambda = lambda.And(p => p.CarrierContacts.Contains(carrier));
                }
                if (carrierQuery.IsActiveSearch.HasValue)
                {
                    lambda = lambda.And(p => p.IsActive == carrierQuery.IsActiveSearch.Value);
                }
            }
            return _crudRepository.GetQueryableByPage<carrier, CarrierDto>(carrierQuery, lambda);
        }

        /// <summary>
        /// 新增承运商
        /// </summary>
        /// <param name="carrierDto"></param>
        /// <returns></returns>
        public void AddCarrier(CarrierDto carrierDto)
        {
            carrierDto.SysId = Guid.NewGuid();
            carrierDto.CreateDate = DateTime.Now;
            carrierDto.UpdateDate = DateTime.Now;
            var carrier = carrierDto.JTransformTo<carrier>();
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateCarrier", method: MethodType.Post, postData: carrier);
                }).Start();
            }
            _crudRepository.InsertAndGetId(carrier);
            
        }

        /// <summary>
        /// 根据Id获取承运商
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public CarrierDto GetCarrierById(Guid sysId)
        {
            return _crudRepository.FirstOrDefault<carrier>(sysId).JTransformTo<CarrierDto>();
        }

        /// <summary>
        /// 编辑承运商
        /// </summary>
        /// <param name="carrierDto"></param>
        public void EditCarrier(CarrierDto carrierDto)
        {
            carrierDto.UpdateDate = DateTime.Now;
            var carrier = carrierDto.JTransformTo<carrier>();
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateCarrier", method: MethodType.Post, postData: carrier);
                }).Start();
            }
            
            _crudRepository.Update(carrier);
            
        }

        /// <summary>
        /// 删除承运商
        /// </summary>
        /// <param name="sysIds"></param>
        public void DeleteCarrier(List<Guid> sysIds)
        {
            if (PublicConst.SyncMultiWHSwitch)
            {
                new Task(() =>
                {
                    ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncDeleteCarrier", method: MethodType.Post, postData: sysIds);
                }).Start();
            }
            
            _crudRepository.Delete<carrier>(sysIds);
            
        }

        /// <summary>
        /// 获取可用承运商
        /// </summary>
        /// <returns></returns>
        public List<CarrierDto> GetExpressListByIsActive()
        {
            return _crudRepository.GetQuery<carrier>(x => x.IsActive == true).JTransformTo<CarrierDto>();
        }
    }
}
