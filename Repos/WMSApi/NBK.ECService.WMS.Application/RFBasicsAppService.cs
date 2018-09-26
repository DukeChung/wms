using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Redis;
using NBK.ECService.WMS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using Abp.Domain.Uow;
using FortuneLab.WebApiClient;

namespace NBK.ECService.WMS.Application
{
    public class RFBasicsAppService : WMSApplicationService, IRFBasicsAppService
    {
        private ICrudRepository _crudRepository = null;
        private IRFBasicsRepository _rfBasicsRepository = null;

        public RFBasicsAppService(ICrudRepository crudRepository, IRFBasicsRepository rfBasicsRepository)
        {
            this._crudRepository = crudRepository;
            this._rfBasicsRepository = rfBasicsRepository;
        }



        /// <summary>
        /// 根据UPC获取商品
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<SkuPackDto> GetSkuListByUPC(string upc, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var result = _rfBasicsRepository.GetSkuListByUPC(upc.Trim());
            if (result != null && result.Count > 0)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据商品SYSID获取商品包装
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public RFPackDto GetPackBySkuSysId(Guid skuSysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var result = _rfBasicsRepository.GetPackBySkuSysId(skuSysId);
            if (result != null && result.PackCode != "缺省")
            {
                return result;
            }
            else
            {
                return new RFPackDto();
            }
        }


        /// <summary>
        /// 根据UPC获取商品
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<RFPackDto> GetPackListByUPC(string upc, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var result = _rfBasicsRepository.GetPackListByUPC(upc.Trim());
            if (result != null && result.Count > 0)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// /更新商品外包装
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public RFCommResult UpdateSkuPack(RFSkuPackDto dto)
        {
            var result = new RFCommResult() { IsSucess = true };
            _crudRepository.ChangeDB(dto.WareHouseSysId);
            try
            {
                var sku = _crudRepository.FirstOrDefault<sku>(x => x.SysId == dto.SkuSysId);
                if (sku == null)
                {
                    throw new Exception("UPC为：" + dto.UPC01 + " 的商品不存在");
                }
                if (dto.PackSysId == null || dto.PackSysId == new Guid())
                {
                    //新增包装信息
                    var packInfo = new pack() { SysId = Guid.NewGuid() };
                    sku.PackSysId = packInfo.SysId;
                    packInfo.FieldValue01 = dto.FieldValue01;
                    packInfo.FieldUom01 = dto.FieldUom01;
                    packInfo.UPC01 = sku.UPC;
                    packInfo.PackCode = dto.PackCode;
                    packInfo.Descr = dto.PackCode;
                    packInfo.FieldValue03 = dto.FieldValue03;
                    packInfo.FieldUom03 = dto.FieldUom03;
                    packInfo.UPC03 = dto.UPC03;
                    packInfo.CreateDate = DateTime.Now;
                    packInfo.CreateUserName = "RF";
                    packInfo.UpdateDate = DateTime.Now;
                    packInfo.UpdateUserName = "RF";
                    //新增包装
                    new Task(() =>
                    {
                        ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreatePack", method: MethodType.Post, postData: packInfo);
                    }).Start();

                    //更新商品
                    new Task(() =>
                    {
                        ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateSkuForWMS", method: MethodType.Post, postData: sku);
                    }).Start();

                }
                else
                {


                    var pack = _crudRepository.FirstOrDefault<pack>(x => x.SysId == (Guid)dto.PackSysId);
                    if (pack != null)
                    {   //更新包装信息
                        sku.PackSysId = pack.SysId;
                        pack.FieldValue01 = dto.FieldValue01;
                        pack.FieldUom01 = dto.FieldUom01;
                        pack.UPC01 = sku.UPC;
                        pack.PackCode = dto.PackCode;
                        pack.Descr = dto.PackCode;
                        pack.FieldValue03 = dto.FieldValue03;
                        pack.FieldUom03 = dto.FieldUom03;
                        pack.UPC03 = dto.UPC03;
                        pack.UpdateDate = DateTime.Now;
                        pack.UpdateUserName = "RF";
                        //更新包装
                        new Task(() =>
                        {
                            ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdatePack", method: MethodType.Post, postData: pack);
                        }).Start();

                    }
                    else
                    {    //新增包装信息
                        pack = new pack() { SysId = Guid.NewGuid() };
                        sku.PackSysId = pack.SysId;
                        pack.FieldValue01 = dto.FieldValue01;
                        pack.FieldUom01 = dto.FieldUom01;
                        pack.UPC01 = sku.UPC;
                        pack.PackCode = dto.PackCode;
                        pack.Descr = dto.PackCode;
                        pack.FieldValue03 = dto.FieldValue03;
                        pack.FieldUom03 = dto.FieldUom03;
                        pack.UPC03 = dto.UPC03;
                        pack.CreateDate = DateTime.Now;
                        pack.CreateUserName = "RF";
                        pack.UpdateDate = DateTime.Now;
                        pack.UpdateUserName = "RF";
                        //新增包装
                        new Task(() =>
                           {
                               ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreatePack", method: MethodType.Post, postData: pack);
                           }).Start();

                        //更新商品
                        new Task(() =>
                        {
                            ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateSkuForWMS", method: MethodType.Post, postData: sku);
                        }).Start();

                    }
                }


            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
