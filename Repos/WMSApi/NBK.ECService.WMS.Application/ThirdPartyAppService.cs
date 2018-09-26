using System;
using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.ThirdParty;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Model.Models;
using System.Linq;
using System.Diagnostics;
using NBK.ECService.WMS.Utility.Enum;
using Newtonsoft.Json;
using NBK.ECService.WMS.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.Utility.RabbitMQ;
using NBK.ECService.WMS.DTO.MQ.Log;
using NBK.ECService.WMS.Utility.Enum.Log;
using NBK.ECService.WMS.Application.Check;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using System.Data.Entity.Infrastructure;
using NBK.ECService.WMS.DTO.MQ;
using NBK.ECService.WMS.DTO.MQ.OrderRule;
using NBK.ECService.WMS.DTO.ThirdParty.OutboundReturn;
using System.Net;
using NBK.ECService.WMS.Utility.Redis;
using Abp.Domain.Uow;
using System.Transactions;

namespace NBK.ECService.WMS.Application
{
    public class ThirdPartyAppService : WMSApplicationService, IThirdPartyAppService
    {
        private IWMSSqlRepository _crudRepository = null;
        private IPackageAppService _packageAppService = null;
        private IBaseAppService _baseAppService = null;
        private IStockTransferAppService _stockTransferAppService = null;
        private IPreBulkPackRepository _preBulkPackRepository;
        private IWareHouseAppService _wareHouseAppService = null;
        private IPickDetailAppService _pickDetailAppService = null;
        private IOutboundTransferOrderRepository _outboundTransferOrderRepository = null;
        private IWMSSqlRepository _wmsSqlRepository = null;
        private IRedisAppService _redisAppService = null;
        private IOutboundRepository _outboundRepository = null;

        public ThirdPartyAppService(IWMSSqlRepository crudRepository, IPackageAppService packageAppService, IBaseAppService baseAppService, IStockTransferAppService stockTransferAppService, IPreBulkPackRepository preBulkPackRepository, IWareHouseAppService wareHouseAppService, IPickDetailAppService pickDetailAppService, IOutboundTransferOrderRepository outboundTransferOrderRepository, IWMSSqlRepository wMSSqlRepository, IRedisAppService redisAppService, IOutboundRepository outboundRepository)
        {
            this._crudRepository = crudRepository;
            this._packageAppService = packageAppService;
            this._baseAppService = baseAppService;
            this._stockTransferAppService = stockTransferAppService;
            this._preBulkPackRepository = preBulkPackRepository;
            this._wareHouseAppService = wareHouseAppService;
            this._pickDetailAppService = pickDetailAppService;
            this._outboundTransferOrderRepository = outboundTransferOrderRepository;
            this._wmsSqlRepository = wMSSqlRepository;
            this._redisAppService = redisAppService;
            this._outboundRepository = outboundRepository;
        }

        #region 商城写数据接口
        /// <summary>
        /// 插入/更新SKU
        /// </summary>
        /// <param name="skuDto"></param>
        /// <returns></returns>
        public CommonResponse InsertOrUpdateSku(ThirdPartySkuDto skuDto)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(skuDto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.InsertOrUpdateSku
            };
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                #region 数据转换及校验
                pack pack = null;
                uom uom = null;
                //uom uom = _crudRepository.GetQuery<uom>(p => p.Descr == skuDto.UOMName).FirstOrDefault();
                //if (uom != null)
                //{
                //    pack = _crudRepository.GetQuery<pack>(p => (p.FieldUom01 == uom.SysId) || (p.FieldUom02 == uom.SysId) || (p.FieldUom03 == uom.SysId)).FirstOrDefault();
                //}
                //else
                //{
                if (skuDto.IsMaterial == true)
                {
                    uom = _crudRepository.GetQuery<uom>(p => p.UOMCode == "g").FirstOrDefault();
                    pack = _crudRepository.GetQuery<pack>(p => p.FieldUom01 == uom.SysId).FirstOrDefault();
                }
                else
                {
                    uom = _crudRepository.GetQuery<uom>(p => p.UOMCode == "缺省").FirstOrDefault();
                    pack = _crudRepository.GetQuery<pack>(p => p.PackCode == "缺省").FirstOrDefault();
                }


                // }
                skuclass skuClass = _crudRepository.GetQuery<skuclass>(p => p.OtherId == skuDto.SkuClassSysId).FirstOrDefault();
                #endregion

                sku editSku = _crudRepository.GetQuery<sku>(p => p.OtherId == skuDto.OtherId).FirstOrDefault();
                Guid? sysId = null;
                string skuCode = null;
                if (new InsertOrUpdateSkuCheck(rsp, skuDto.UPC, pack, skuClass).Execute().IsSuccess)
                {
                    decimal? cube = (skuDto.Length.HasValue && skuDto.Width.HasValue && skuDto.Height.HasValue)
                        ? (skuDto.Length.Value * skuDto.Width.Value * skuDto.Height.Value)
                        : new decimal?();
                    if (editSku == null && skuDto.InsertFlag == true)
                    {
                        sysId = Guid.NewGuid();
                        //skuCode = _crudRepository.GenNextNumber(PublicConst.GenNextNumberSku);
                        skuCode = _baseAppService.GetNumber(PublicConst.GenNextNumberSku);
                        lottemplate lottemplate = _crudRepository.GetQuery<lottemplate>(p => p.LotCode == "缺省").FirstOrDefault();
                        sku newSku = new sku();
                        newSku.SysId = sysId.GetValueOrDefault();
                        newSku.SkuCode = skuCode;
                        newSku.SkuName = skuDto.SkuName;
                        newSku.SkuClassSysId = skuClass.SysId;
                        newSku.SkuDescr = skuDto.SkuDescr;
                        newSku.ShelfLifeOnReceiving = 0;
                        newSku.ShelfLife = 0;
                        newSku.PackSysId = pack.SysId;
                        newSku.DaysToExpire = skuDto.DaysToExpire ?? 0;
                        newSku.LotTemplateSysId = lottemplate != null ? lottemplate.SysId : Guid.Empty;
                        newSku.Length = skuDto.Length ?? decimal.Zero;
                        newSku.Width = skuDto.Width ?? decimal.Zero;
                        newSku.Height = skuDto.Height ?? decimal.Zero;
                        newSku.Cube = cube ?? decimal.Zero;
                        newSku.NetWeight = skuDto.NetWeight != null ? skuDto.NetWeight / 1000 : 0;
                        newSku.GrossWeight = skuDto.GrossWeight != null ? skuDto.GrossWeight / 1000 : 0;
                        newSku.SalePrice = skuDto.SalePrice;
                        newSku.IsInvoices = skuDto.IsInvoices;
                        newSku.IsRefunds = skuDto.IsRefunds;
                        newSku.IsMaterial = skuDto.IsMaterial;
                        newSku.Image = skuDto.Image;
                        newSku.OtherId = skuDto.OtherId;
                        newSku.UPC = skuDto.UPC;
                        newSku.IsActive = true;
                        newSku.CreateBy = 99999;
                        newSku.CreateDate = DateTime.Now;
                        newSku.UpdateBy = 99999;
                        newSku.UpdateDate = DateTime.Now;
                        newSku.SpecialTypes = skuDto.HasSN ? (int)SkuSpecialTypes.RedCard : (int)SkuSpecialTypes.Normal;
                        if (PublicConst.SyncMultiWHSwitch)
                        {
                            new Task(() =>
                            {
                                ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateSku", method: MethodType.Post, postData: newSku);
                            }).Start();
                        }

                        _crudRepository.Insert(newSku);

                    }
                    else
                    {
                        if (editSku == null)
                        {
                            throw new Exception("更新失败，商品不存在");
                        }

                        editSku.SkuName = skuDto.SkuName;
                        editSku.SkuClassSysId = skuClass.SysId;
                        editSku.SkuDescr = skuDto.SkuDescr;
                        //editSku.PackSysId = pack.SysId;
                        editSku.DaysToExpire = skuDto.DaysToExpire ?? 0;
                        editSku.Length = skuDto.Length ?? decimal.Zero;
                        editSku.Width = skuDto.Width ?? decimal.Zero;
                        editSku.Height = skuDto.Height ?? decimal.Zero;
                        editSku.Cube = cube ?? decimal.Zero;
                        editSku.NetWeight = skuDto.NetWeight != null ? skuDto.NetWeight / 1000 : 0;
                        editSku.GrossWeight = skuDto.GrossWeight != null ? skuDto.GrossWeight / 1000 : 0;
                        editSku.SalePrice = skuDto.SalePrice;
                        editSku.IsInvoices = skuDto.IsInvoices;
                        editSku.IsRefunds = skuDto.IsRefunds;
                        editSku.IsMaterial = skuDto.IsMaterial;
                        editSku.Image = skuDto.Image;
                        editSku.UPC = skuDto.UPC;
                        editSku.UpdateBy = 99999;
                        editSku.UpdateDate = DateTime.Now;
                        editSku.SpecialTypes = skuDto.HasSN ? (int)SkuSpecialTypes.RedCard : (int)SkuSpecialTypes.Normal;
                        if (PublicConst.SyncMultiWHSwitch)
                        {
                            new Task(() =>
                            {
                                ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateSku", method: MethodType.Post, postData: editSku);
                            }).Start();
                        }

                        _crudRepository.Update(editSku);

                    }
                }
                //记录接口日志
                interfaceLogDto.doc_sysId = rsp.IsSuccess ? (editSku == null ? sysId : editSku.SysId) : new Guid?();
                interfaceLogDto.doc_order = rsp.IsSuccess ? (editSku == null ? skuCode : editSku.SkuCode) : null;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw ex;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }


        /// <summary>
        /// 同步包装和单位
        /// </summary>
        /// <param name="skuPack"></param>
        public CommonResponse InsertOrUpdateSkuPack(ThirdPartySkuPackDto skuPack)
        {
            CommonResponse rsp = new CommonResponse { IsSuccess = true };
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(skuPack)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.InsertOrUpdatePackSku
            };
            try
            {
                var sku = _crudRepository.FirstOrDefault<sku>(x => x.OtherId == skuPack.SkuOtherId);
                if (sku == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "商品" + skuPack.SkuOtherId + "不存在";
                    throw new Exception(rsp.ErrorMessage);
                }
                if (sku.IsMaterial.HasValue && sku.IsMaterial.Value)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "商品" + skuPack.SkuOtherId + "是原材料,无法指定商品包装系数！";
                    throw new Exception(rsp.ErrorMessage);
                }
                var pack = _crudRepository.Get<pack>(sku.PackSysId);
                var uom = new uom();
                #region  新增
                if (skuPack.Action == PublicConst.ThirdPartyActionInsert)
                {
                    #region 创建基础包装
                    if (string.IsNullOrEmpty(pack.Source))
                    {
                        uom = _crudRepository.GetQuery<uom>(p => p.UOMCode == "缺省").FirstOrDefault();
                        pack = new pack();
                        pack.SysId = Guid.NewGuid();
                        pack.PackCode = sku.SkuName.TrimEnd() + "包装";
                        pack.Descr = sku.SkuName.TrimEnd() + "包装";
                        pack.SysId = Guid.NewGuid();
                        pack.FieldUom01 = uom.SysId;
                        pack.FieldValue01 = 1;
                        pack.UPC01 = sku.UPC;
                        pack.Source = PublicConst.ThirdPartySourceERP;
                        pack.CreateDate = DateTime.Now;
                        pack.CreateUserName = "ECC";
                        pack.CreateBy = 999;
                        pack.UpdateDate = DateTime.Now;
                        pack.UpdateUserName = "ECC";
                        pack.UpdateBy = 999;
                        if (PublicConst.SyncMultiWHSwitch)
                        {
                            new Task(() =>
                            {
                                ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreatePack", method: MethodType.Post, postData: pack);
                            }).Start();
                        }

                        _crudRepository.Insert(pack);

                        sku.PackSysId = pack.SysId;
                        if (PublicConst.SyncMultiWHSwitch)
                        {
                            new Task(() =>
                            {
                                ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateSkuForPack", method: MethodType.Post, postData: sku);
                            }).Start();
                        }

                        _crudRepository.Update(sku);


                        _crudRepository.SaveChange();
                    }
                    #endregion
                    uom = GetUomByUomCode(skuPack.UOMName);
                    var isAdd = false;
                    if (!pack.FieldValue02.HasValue)
                    {
                        pack.FieldUom02 = uom.SysId;
                        pack.UPC02 = skuPack.UPC;
                        pack.FieldValue02 = skuPack.PackQty;
                        if (skuPack.CoefficientID.HasValue)
                        {
                            pack.CoefficientId02 = skuPack.CoefficientID.Value;
                        }
                        isAdd = true;
                    }

                    if (!isAdd && !pack.FieldValue03.HasValue)
                    {
                        pack.FieldUom03 = uom.SysId;
                        pack.UPC03 = skuPack.UPC;
                        pack.FieldValue03 = skuPack.PackQty;
                        if (skuPack.CoefficientID.HasValue)
                        {
                            pack.CoefficientId03 = skuPack.CoefficientID.Value;
                        }

                        isAdd = true;
                    }

                    if (!isAdd && !pack.FieldValue04.HasValue)
                    {
                        pack.FieldUom04 = uom.SysId;
                        pack.UPC04 = skuPack.UPC;
                        pack.FieldValue04 = skuPack.PackQty;
                        if (skuPack.CoefficientID.HasValue)
                        {
                            pack.CoefficientId04 = skuPack.CoefficientID.Value;
                        }
                        isAdd = true;
                    }

                    if (!isAdd && !pack.FieldValue05.HasValue)
                    {
                        pack.FieldUom05 = uom.SysId;
                        pack.UPC05 = skuPack.UPC;
                        pack.FieldValue05 = skuPack.PackQty;
                        if (skuPack.CoefficientID.HasValue)
                        {
                            pack.CoefficientId05 = skuPack.CoefficientID.Value;
                        }
                        isAdd = true;
                    }
                    if (isAdd)
                    {
                        pack.UpdateDate = DateTime.Now;
                        pack.UpdateUserName = "ECC";
                        pack.UpdateBy = 999;

                        if (PublicConst.SyncMultiWHSwitch)
                        {
                            new Task(() =>
                            {
                                ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdatePack", method: MethodType.Post, postData: pack);
                            }).Start();
                        }

                        _crudRepository.Update(pack);

                        _crudRepository.SaveChange();
                    }
                    else
                    {
                        rsp.IsSuccess = false;
                        rsp.ErrorMessage = "商品" + sku.SkuName + "包装系数,已经超过最大数量4个,写入新的包装系数失败！";
                        throw new Exception(rsp.ErrorMessage);
                    }

                }
                #endregion
                #region 删除 
                if (skuPack.Action == PublicConst.ThirdPartyActionDelete)
                {

                    var isDelete = false;
                    if (pack.CoefficientId02 == skuPack.CoefficientID)
                    {
                        pack.FieldUom02 = null;
                        pack.UPC02 = null;
                        pack.FieldValue02 = null;
                        pack.CoefficientId02 = null;
                        isDelete = true;
                    }

                    if (!isDelete && pack.CoefficientId03 == skuPack.CoefficientID)
                    {
                        pack.FieldUom03 = null;
                        pack.UPC03 = null;
                        pack.FieldValue03 = null;
                        pack.CoefficientId03 = null;
                        isDelete = true;
                    }

                    if (!isDelete && pack.CoefficientId04 == skuPack.CoefficientID)
                    {
                        pack.FieldUom04 = null;
                        pack.UPC04 = null;
                        pack.FieldValue04 = null;
                        pack.CoefficientId04 = null;
                        isDelete = true;
                    }

                    if (!isDelete && pack.CoefficientId05 == skuPack.CoefficientID)
                    {
                        pack.FieldUom05 = null;
                        pack.UPC05 = null;
                        pack.FieldValue05 = null;
                        pack.CoefficientId05 = null;
                        isDelete = true;
                    }
                    if (isDelete)
                    {
                        pack.UpdateDate = DateTime.Now;
                        pack.UpdateUserName = "ECC";
                        pack.UpdateBy = 999;

                        if (PublicConst.SyncMultiWHSwitch)
                        {
                            new Task(() =>
                            {
                                ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdatePack", method: MethodType.Post, postData: pack);
                            }).Start();
                        }

                        _crudRepository.Update(pack);

                        _crudRepository.SaveChange();
                    }
                    else
                    {
                        rsp.IsSuccess = true;
                        rsp.ErrorMessage = "";

                    }
                }
                #endregion
                #region  更新
                if (skuPack.Action == PublicConst.ThirdPartyActionUpdate)
                {
                    var isUpdate = false;
                    uom = GetUomByUomCode(skuPack.UOMName);

                    if (pack.CoefficientId02 == skuPack.CoefficientID)
                    {
                        pack.FieldUom02 = uom.SysId;
                        pack.UPC02 = skuPack.UPC;
                        pack.FieldValue02 = skuPack.PackQty;
                        if (skuPack.CoefficientID.HasValue)
                        {
                            pack.CoefficientId02 = skuPack.CoefficientID.Value;
                        }
                        isUpdate = true;
                    }

                    if (pack.CoefficientId03 == skuPack.CoefficientID)
                    {
                        pack.FieldUom03 = uom.SysId;
                        pack.UPC03 = skuPack.UPC;
                        pack.FieldValue03 = skuPack.PackQty;
                        if (skuPack.CoefficientID.HasValue)
                        {
                            pack.CoefficientId03 = skuPack.CoefficientID.Value;
                        }
                        isUpdate = true;
                    }

                    if (pack.CoefficientId04 == skuPack.CoefficientID)
                    {
                        pack.FieldUom04 = uom.SysId;
                        pack.UPC04 = skuPack.UPC;
                        pack.FieldValue04 = skuPack.PackQty;
                        if (skuPack.CoefficientID.HasValue)
                        {
                            pack.CoefficientId04 = skuPack.CoefficientID.Value;
                        }
                        isUpdate = true;
                    }


                    if (pack.CoefficientId05 == skuPack.CoefficientID)
                    {
                        pack.FieldUom05 = uom.SysId;
                        pack.UPC05 = skuPack.UPC;
                        pack.FieldValue05 = skuPack.PackQty;
                        if (skuPack.CoefficientID.HasValue)
                        {
                            pack.CoefficientId05 = skuPack.CoefficientID.Value;
                        }
                        isUpdate = true;
                    }
                    if (isUpdate)
                    {
                        pack.UpdateDate = DateTime.Now;
                        pack.UpdateUserName = "ECC";
                        pack.UpdateBy = 999;

                        if (PublicConst.SyncMultiWHSwitch)
                        {
                            new Task(() =>
                            {
                                ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdatePack", method: MethodType.Post, postData: pack);
                            }).Start();
                        }

                        _crudRepository.Update(pack);

                        _crudRepository.SaveChange();
                    }
                    else
                    {
                        rsp.IsSuccess = true;
                    }
                }
                #endregion
                interfaceLogDto.doc_sysId = rsp.IsSuccess ? pack.SysId : new Guid?();
                interfaceLogDto.doc_order = rsp.IsSuccess ? pack.PackCode : null;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw ex;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }
        #endregion

        private uom GetUomByUomCode(string uomCode)
        {
            var uom = _crudRepository.FirstOrDefault<uom>(p => p.UOMCode == uomCode);
            if (uom == null)
            {
                uom = new uom();
                uom.SysId = Guid.NewGuid();
                uom.Descr = uomCode;
                uom.UOMCode = uomCode;
                uom.UomType = "Package";
                _crudRepository.Insert(uom);

                if (PublicConst.SyncMultiWHSwitch)
                {
                    new Task(() =>
                    {
                        ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateUOM", method: MethodType.Post, postData: uom);
                    }).Start();
                }

                _crudRepository.SaveChange();
            }
            return uom;
        }

        /// <summary>
        /// 插入/更新SKU分类
        /// </summary>
        /// <param name="skuClassDto"></param>
        /// <returns></returns>
        public CommonResponse InsertOrUpdateSkuClass(ThirdPartySkuClassDto skuClassDto)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(skuClassDto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.InsertOrUpdateSkuClass
            };
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                var editSkuClass = _crudRepository.GetQuery<skuclass>(p => p.OtherId == skuClassDto.OtherId).FirstOrDefault();
                Guid? parentSysId = null;
                if (!string.IsNullOrEmpty(skuClassDto.Source))
                {
                    var parentSkuClass = _crudRepository.GetQuery<skuclass>(p => p.OtherId == skuClassDto.Source).FirstOrDefault();
                    if (parentSkuClass != null)
                    {
                        parentSysId = parentSkuClass.SysId;
                    }
                }
                Guid? sysId = null;
                if (editSkuClass == null)
                {
                    sysId = Guid.NewGuid();
                    skuclass newSkuClass = new skuclass();
                    newSkuClass.SysId = sysId.Value;
                    newSkuClass.SkuClassName = skuClassDto.SkuClassName;
                    newSkuClass.IsActive = true;
                    newSkuClass.ParentSysId = parentSysId;
                    newSkuClass.CreateBy = 99999;
                    newSkuClass.CreateDate = DateTime.Now;
                    newSkuClass.UpdateBy = 99999;
                    newSkuClass.UpdateDate = DateTime.Now;
                    newSkuClass.OtherId = skuClassDto.OtherId;
                    newSkuClass.Source = skuClassDto.Source;
                    if (PublicConst.SyncMultiWHSwitch)
                    {
                        new Task(() =>
                        {
                            ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateSkuclass", method: MethodType.Post, postData: newSkuClass);
                        }).Start();
                    }

                    _crudRepository.Insert(newSkuClass);

                }
                else
                {
                    editSkuClass.SkuClassName = skuClassDto.SkuClassName;
                    editSkuClass.ParentSysId = parentSysId;
                    editSkuClass.UpdateBy = 99999;
                    editSkuClass.UpdateDate = DateTime.Now;
                    editSkuClass.OtherId = skuClassDto.OtherId;
                    editSkuClass.Source = skuClassDto.Source;
                    if (PublicConst.SyncMultiWHSwitch)
                    {
                        new Task(() =>
                        {
                            ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateSkuclass", method: MethodType.Post, postData: editSkuClass);
                        }).Start();
                    }

                    _crudRepository.Update(editSkuClass);

                }
                //记录接口日志
                interfaceLogDto.doc_sysId = rsp.IsSuccess ? (editSkuClass == null ? sysId : editSkuClass.SysId) : new Guid?();
                interfaceLogDto.doc_order = rsp.IsSuccess ? skuClassDto.SkuClassName : null;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw ex;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }

        /// <summary>
        /// 插入采购单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        public CommonResponse InsertPurchase(ThirdPartyPurchaseDto purchaseDto)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(purchaseDto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.InsertPurchase
            };
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                warehouse warehouse = RedisGetWareHouseByOtherId(purchaseDto.WarehouseSysId);
                if (warehouse == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "仓库不存在,Id" + purchaseDto.WarehouseSysId;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }
                _crudRepository.ChangeDB(warehouse.SysId);

                var purchaseList = _crudRepository.GetQuery<purchase>(p => p.ExternalOrder == purchaseDto.ExternalOrder);
                if (purchaseList.Any())
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "入库单号" + purchaseDto.ExternalOrder + "重复无法保存";
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }


                var purchaseSysId = Guid.NewGuid();
                string purchaseOrder = null;
                if (purchaseDto.PurchaseDetailDtoList != null && purchaseDto.PurchaseDetailDtoList.Any())
                {
                    List<string> otherSkuIdList = purchaseDto.PurchaseDetailDtoList.Select(p => p.OtherSkuId).ToList();
                    List<sku> skuList = _crudRepository.GetAllList<sku>(p => otherSkuIdList.Contains(p.OtherId));
                    List<Guid> skuClassSysIdList = skuList.Select(p => p.SkuClassSysId).ToList();
                    List<Guid> packSysIdList = skuList.Select(p => p.PackSysId).ToList();
                    List<skuclass> skuClassList = _crudRepository.GetAllList<skuclass>(p => skuClassSysIdList.Contains(p.SysId));
                    List<pack> packList = _crudRepository.GetAllList<pack>(p => packSysIdList.Contains(p.SysId));
                    List<Guid?> fieldUom01List = packList.Select(p => p.FieldUom01).ToList();
                    List<uom> uomList = _crudRepository.GetAllList<uom>(p => fieldUom01List.Contains(p.SysId));
                    if (new InsertPurchaseDetailCheck(rsp, purchaseDto, skuList, skuClassList, packList, uomList).Execute().IsSuccess)
                    {
                        #region 采购单明细
                        List<purchasedetail> purchaseDetails = new List<purchasedetail>();
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
                            purchasedetail purchaseDetail = new purchasedetail
                            {
                                SysId = Guid.NewGuid(),
                                PurchaseSysId = purchaseSysId,
                                SkuSysId = sku.SysId,
                                SkuClassSysId = skuClass == null ? new Guid?() : skuClass.SysId,
                                UOMSysId = uom.SysId,
                                UomCode = uom.UOMCode,
                                PackSysId = pack.SysId,
                                PackCode = pack.PackCode,
                                Qty = purchaseDetailDto.Qty,
                                GiftQty = purchaseDetailDto.GiftQty,
                                ReceivedQty = purchaseDetailDto.ReceivedQty,
                                RejectedQty = purchaseDetailDto.RejectedQty,
                                PurchasePrice = purchaseDetailDto.PurchasePrice,
                                Remark = purchaseDetailDto.Remark,
                                OtherSkuId = purchaseDetailDto.OtherSkuId,
                                PackFactor = purchaseDetailDto.PackFactor,
                                UpdateBy = 99999,
                                UpdateDate = DateTime.Now,
                            };
                            purchaseDetails.Add(purchaseDetail);
                        }
                        #endregion

                        #region 采购单
                        purchase purchase = null;

                        if (new InsertPurchaseCheck(rsp, warehouse).Execute().IsSuccess)

                        {
                            Guid vendorSysId;
                            vendor vendor =
                                _crudRepository.GetQuery<vendor>(p => p.OtherVendorId == purchaseDto.VendorSysId)
                                    .FirstOrDefault();
                            if (vendor == null)
                            {
                                vendorSysId = Guid.NewGuid();
                                _crudRepository.Insert(new vendor
                                {
                                    SysId = vendorSysId,
                                    VendorName = purchaseDto.VendorName,
                                    VendorPhone = purchaseDto.VendorPhone,
                                    OtherVendorId = purchaseDto.VendorSysId,
                                    VendorContacts = purchaseDto.VendorContacts
                                });
                            }
                            else
                            {
                                vendorSysId = vendor.SysId;
                            }

                            //如果接口中的单号字段不为空就用接口中的，否则自己生成
                            if (!string.IsNullOrEmpty(purchaseDto.PurchaseOrder))
                            {
                                purchaseOrder = purchaseDto.PurchaseOrder;
                            }
                            else
                            {
                                //purchaseOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberPurchase);
                                purchaseOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberPurchase);
                            }

                            //获取之前业务的单号
                            Guid? outboundSysId = null;
                            string outboundOrder = string.Empty;
                            if (!string.IsNullOrEmpty(purchaseDto.ExternOutboundOrder))
                            {
                                var outbound = _crudRepository.GetQuery<outbound>(x => x.ExternOrderId == purchaseDto.ExternOutboundOrder).FirstOrDefault();
                                if (outbound != null)
                                {
                                    outboundSysId = outbound.SysId;
                                    outboundOrder = outbound.OutboundOrder;
                                }
                            }

                            purchase = new purchase
                            {
                                SysId = purchaseSysId,
                                PurchaseOrder = purchaseOrder,
                                DeliveryDate = purchaseDto.DeliveryDate,
                                ExternalOrder = purchaseDto.ExternalOrder,
                                VendorSysId = vendorSysId,
                                Descr = purchaseDto.Descr,
                                PurchaseDate = purchaseDto.PurchaseDate,
                                AuditingDate = purchaseDto.AuditingDate,
                                AuditingBy = purchaseDto.AuditingBy,
                                AuditingName = purchaseDto.AuditingName,
                                Status = (int)PurchaseStatus.New,
                                Type = purchaseDto.Type,
                                Source = purchaseDto.Source,
                                WarehouseSysId = warehouse.SysId,
                                CreateBy = 99999,
                                CreateDate = DateTime.Now,
                                UpdateBy = 99999,
                                UpdateDate = DateTime.Now,
                                Channel = purchaseDto.Channel,
                                BatchNumber = purchaseDto.BatchNumber,
                                OutboundSysId = outboundSysId,
                                OutboundOrder = outboundOrder
                            };
                            _crudRepository.Insert(purchase);
                            _crudRepository.BatchInsert(purchaseDetails);
                        }
                        else
                        {
                            interfaceLogDto.doc_sysId = rsp.IsSuccess ? purchaseSysId : new Guid?();
                            interfaceLogDto.doc_order = rsp.IsSuccess ? purchaseOrder : null;
                            interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                            interfaceLogDto.flag = rsp.IsSuccess;
                            return rsp;
                        }
                        #endregion

                        //发送邮件通知
                        Guid sysCodeSysId = _crudRepository.FirstOrDefault<syscode>(p => p.SysCodeType == PublicConst.SysCodeTypeReceiptOutboundMail).SysId;
                        string mailTo = _crudRepository.FirstOrDefault<syscodedetail>(p => p.SysCodeSysId == sysCodeSysId && p.Code == "PurchaseMail").Descr;
                        EmailHelper.SendMailAsync(PublicConst.NewPurchaseSubject, string.Format(PublicConst.NewPurchaseMailBody, purchase.PurchaseOrder, purchase.DeliveryDate), mailTo);
                    }
                    else
                    {
                        interfaceLogDto.doc_sysId = rsp.IsSuccess ? purchaseSysId : new Guid?();
                        interfaceLogDto.doc_order = rsp.IsSuccess ? purchaseOrder : null;
                        interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                        interfaceLogDto.flag = rsp.IsSuccess;
                        return rsp;
                    }
                }

                //记录接口日志
                interfaceLogDto.doc_sysId = rsp.IsSuccess ? purchaseSysId : new Guid?();
                interfaceLogDto.doc_order = rsp.IsSuccess ? purchaseOrder : null;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw ex;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }

        /// <summary>
        /// 插入订单
        /// </summary>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public CommonResponse InsertOutbound(ThirdPartyOutboundDto outboundDto)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(outboundDto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.InsertOutbound
            };
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                warehouse warehouse = RedisGetWareHouseByOtherId(outboundDto.WareHouseSysId);
                if (warehouse == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "仓库不存在,Id" + outboundDto.WareHouseSysId;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }
                _crudRepository.ChangeDB(warehouse.SysId);

                var purchaseList = _crudRepository.GetQuery<outbound>(p => p.ExternOrderId == outboundDto.ExternOrderId);
                if (purchaseList.Any())
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "出库单" + outboundDto.ExternOrderId + "重复无法保存";
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }

                var outboundRule = _crudRepository.GetQuery<outboundrule>(x => x.WarehouseSysId == warehouse.SysId).FirstOrDefault();

                var outboundSysId = Guid.NewGuid();
                string outboundOrder = null;
                //如果接口中的单号字段不为空就用接口中的，否则自己生成
                if (!string.IsNullOrEmpty(outboundDto.OutboundOrder))
                {
                    outboundOrder = outboundDto.OutboundOrder;
                }
                else
                {
                    //outboundOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberOutbound);
                    outboundOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberOutbound);
                }

                if (outboundRule != null)
                {
                    //异步创建出库单
                    if (outboundRule.CreateOutboundIsAsyn == true)
                    {
                        MQProcessDto<ThirdPartyOutboundDto> bussinessProcessLogDto = new MQProcessDto<ThirdPartyOutboundDto>()
                        {
                            BussinessSysId = outboundSysId,
                            BussinessOrderNumber = outboundOrder,
                            //BussinessType = "InsertOutbound",
                            //BussinessTypeName = "快速出库",
                            Descr = "异步创建出库单",
                            CurrentUserId = 99999,
                            CurrentDisplayName = "ECC",
                            WarehouseSysId = warehouse.SysId,
                            BussinessDto = outboundDto
                        };
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InsertOutbound, bussinessProcessLogDto);
                        rsp.IsAsyn = true;
                        rsp.Message = "创建出库单命令已提交，请稍后等待返回处理消息";
                    }

                    //同步创建出库单
                    else
                    {
                        #region 同步创建出库单
                        if (outboundDto.OutboundDetailDtoList != null && outboundDto.OutboundDetailDtoList.Any())
                        {
                            List<string> otherSkuIdList = outboundDto.OutboundDetailDtoList.Select(p => p.OtherSkuId).ToList();
                            List<sku> skuList = _crudRepository.GetQuery<sku>(p => otherSkuIdList.Contains(p.OtherId)).ToList();
                            List<Guid> skuClassSysIdList = skuList.Select(p => p.SkuClassSysId).ToList();
                            List<Guid> packSysIdList = skuList.Select(p => p.PackSysId).ToList();
                            List<skuclass> skuClassList = _crudRepository.GetAllList<skuclass>(p => skuClassSysIdList.Contains(p.SysId));
                            List<pack> packList = _crudRepository.GetQuery<pack>(p => packSysIdList.Contains(p.SysId)).ToList();
                            List<Guid?> fieldUom01List = packList.Select(p => p.FieldUom01).ToList();
                            List<uom> uomList = _crudRepository.GetQuery<uom>(p => fieldUom01List.Contains(p.SysId)).ToList();
                            if (new InsertOutboundDetailCheck(rsp, outboundDto.OutboundDetailDtoList, skuList, skuClassList, packList, uomList).Execute().IsSuccess)
                            {
                                #region 出库单明细
                                List<outbounddetail> outboundDetails = new List<outbounddetail>();

                                #region 组织B2C赠品数据
                                if (outboundDto.OutboundType == (int)OutboundType.B2C)
                                {
                                    var skuOtherIds = outboundDto.OutboundDetailDtoList.GroupBy(p => new { p.OtherSkuId }).Select(p => new ThirdPartyOutboundDetailDto()
                                    {
                                        OtherSkuId = p.Key.OtherSkuId
                                    }).ToList();

                                    var newOutboundDetailList = new List<ThirdPartyOutboundDetailDto>();
                                    foreach (var otherId in skuOtherIds)
                                    {
                                        var newlits = outboundDto.OutboundDetailDtoList.Where(x => x.OtherSkuId == otherId.OtherSkuId).ToList();
                                        var detailDto = new ThirdPartyOutboundDetailDto();
                                        if (newlits.Count() > 1)
                                        {
                                            detailDto.OtherSkuId = otherId.OtherSkuId;
                                            detailDto.PackFactor = newlits.Where(x => x.IsGift == false).First().PackFactor;
                                            detailDto.Price = newlits.Where(x => x.IsGift == false).First().Price;
                                            detailDto.IsGift = false;
                                            detailDto.Qty = newlits.Sum(x => x.Qty);

                                            var giftQty = newlits.Where(x => x.IsGift == true).Sum(x => x.Qty);
                                            detailDto.GiftQty = giftQty.HasValue ? giftQty.Value : 0;
                                        }
                                        else
                                        {
                                            detailDto = newlits[0];
                                            if (detailDto.IsGift)
                                            {
                                                detailDto.GiftQty = detailDto.Qty.Value;
                                            }
                                        }
                                        newOutboundDetailList.Add(detailDto);
                                    }

                                    outboundDto.OutboundDetailDtoList = newOutboundDetailList;
                                }
                                #endregion

                                foreach (var outboundDetailDto in outboundDto.OutboundDetailDtoList)
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

                                    outbounddetail outboundDetail = new outbounddetail
                                    {
                                        SysId = Guid.NewGuid(),
                                        OutboundSysId = outboundSysId,
                                        SkuSysId = sku.SysId,
                                        UOMSysId = uom.SysId,
                                        PackSysId = pack.SysId,
                                        Qty = outboundDetailDto.Qty,
                                        Price = outboundDetailDto.Price,
                                        Status = (int)OutboundDetailStatus.New,
                                        CreateBy = 99999,
                                        CreateDate = DateTime.Now,
                                        UpdateBy = 99999,
                                        UpdateDate = DateTime.Now,
                                        PackFactor = outboundDetailDto.PackFactor,
                                        //是否赠品
                                        IsGift = outboundDetailDto.IsGift,
                                        GiftQty = outboundDetailDto.GiftQty
                                    };
                                    outboundDetails.Add(outboundDetail);
                                }
                                #endregion

                                #region 出库单
                                outbound outbound = null;

                                if (new InsertOutboundCheck(rsp, warehouse).Execute().IsSuccess)
                                {
                                    //获取之前业务的单号
                                    Guid? receiptSysId = null;
                                    string purchaseOrder = string.Empty;
                                    if (!string.IsNullOrEmpty(outboundDto.ExternPurchaseOrder))
                                    {
                                        var purchase = _crudRepository.GetQuery<purchase>(x => x.ExternalOrder == outboundDto.ExternPurchaseOrder).FirstOrDefault();
                                        if (purchase != null)
                                        {
                                            receiptSysId = purchase.SysId;
                                            purchaseOrder = purchase.PurchaseOrder;
                                        }
                                    }

                                    outbound = new outbound
                                    {
                                        SysId = outboundSysId,
                                        OutboundOrder = outboundOrder,
                                        WareHouseSysId = warehouse.SysId,
                                        RequestedShipDate = outboundDto.RequestedShipDate,
                                        ActualShipDate = outboundDto.ActualShipDate,
                                        DeliveryDate = outboundDto.DeliveryDate,
                                        OutboundType = outboundDto.OutboundType,
                                        OutboundChildType = outboundDto.OutboundChildType,
                                        Status = (int)OutboundStatus.New,
                                        AuditingDate = outboundDto.AuditingDate,
                                        AuditingBy = outboundDto.AuditingBy,
                                        AuditingName = outboundDto.AuditingName,
                                        OutboundDate = outboundDto.ExternOrderDate,
                                        ExternOrderDate = outboundDto.ExternOrderDate,
                                        ExternOrderId = outboundDto.ExternOrderId,
                                        ConsigneeName = outboundDto.ConsigneeName,
                                        ConsigneeAddress = (outboundDto.OutboundType == (int)OutboundType.B2C && !string.IsNullOrEmpty(outboundDto.ConsigneeAddress)) ? outboundDto.ConsigneeAddress.Replace(outboundDto.ConsigneeProvince + outboundDto.ConsigneeCity + outboundDto.ConsigneeArea, "") : outboundDto.ConsigneeAddress,
                                        ConsigneeProvince = outboundDto.ConsigneeProvince,
                                        ConsigneeCity = (outboundDto.ConsigneeCity == "县" || outboundDto.ConsigneeCity == "市辖区") ? outboundDto.ConsigneeProvince : outboundDto.ConsigneeCity,
                                        ConsigneeArea = outboundDto.ConsigneeArea,
                                        ConsigneePhone =
                                            string.IsNullOrEmpty(outboundDto.ConsigneeCellPhone)
                                                ? outboundDto.ConsigneePhone
                                                : outboundDto.ConsigneeCellPhone,
                                        ConsigneeTown = outboundDto.ConsigneeTown,
                                        ConsigneeVillage = outboundDto.ConsigneeVillage,
                                        PostalCode = outboundDto.PostalCode,
                                        CashOnDelivery = outboundDto.CashOnDelivery,
                                        ShippingMethod = outboundDto.ShippingMethod,
                                        TotalQty = outboundDto.TotalQty,
                                        InvoiceType = outboundDto.InvoiceType,
                                        Freight = outboundDto.Freight,
                                        Source = outboundDto.Source,
                                        ServiceStationName = outboundDto.ServiceStationName,
                                        Remark = outboundDto.Remark,
                                        TotalPrice = outboundDto.TotalPrice,
                                        CreateBy = 99999,
                                        CreateDate = DateTime.Now,
                                        UpdateBy = 99999,
                                        UpdateDate = DateTime.Now,
                                        Channel = outboundDto.Channel,
                                        BatchNumber = outboundDto.BatchNumber,
                                        ReceiptSysId = receiptSysId,
                                        PurchaseOrder = purchaseOrder,
                                        //新增平台订单号和订单折扣
                                        PlatformOrder = outboundDto.PlatformOrder,
                                        DiscountPrice = outboundDto.DiscountPrice,
                                        //新增服务站编码,是否开票，优惠券价格
                                        ServiceStationCode = outboundDto.ServiceStationCode,
                                        IsInvoice = outboundDto.HasInvoice,
                                        CouponPrice = outboundDto.CouponAmount
                                    };

                                    #region 经纬度更新
                                    //var city = outboundDto.ConsigneeProvince + outboundDto.ConsigneeCity;
                                    //var address = outboundDto.ConsigneeAddress;
                                    //var coordinate = _baseAppService.GetCoordinate(city, address);
                                    //if (coordinate != null && coordinate.Status == 0 && coordinate.Result != null && coordinate.Result.location != null)
                                    //{
                                    //    outbound.Lat = coordinate.Result.location.lat;
                                    //    outbound.Lng = coordinate.Result.location.lng;
                                    //}
                                    #endregion

                                    TransactionOptions transactionOption = new TransactionOptions();
                                    transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                                    using (
                                        TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                                            transactionOption))
                                    {
                                        _crudRepository.Insert(outbound);
                                        _crudRepository.SaveChange();
                                        _crudRepository.ThirdPartyInsertOutboundDetail(outboundDetails);

                                        scope.Complete();
                                    }

                                    #region 推送匹配预包装MQ
                                    if (outbound.OutboundType != (int)OutboundType.Normal
                                        && outbound.OutboundType != (int)OutboundType.B2C
                                        && outbound.OutboundType != (int)OutboundType.Return)
                                    {
                                        MQOrderRuleDto mqOrderRuleDto = new MQOrderRuleDto() { OrderSysId = outbound.SysId, OrderNumber = outbound.OutboundOrder, WarehouseSysId = outbound.WareHouseSysId };
                                        MQProcessDto<MQOrderRuleDto> mqDto = new MQProcessDto<MQOrderRuleDto>()
                                        {
                                            BussinessDto = mqOrderRuleDto,
                                            BussinessSysId = outbound.SysId,
                                            BussinessOrderNumber = outbound.OutboundOrder,
                                            CurrentUserId = 99999,
                                            CurrentDisplayName = "ECC",
                                            WarehouseSysId = outbound.WareHouseSysId
                                        };
                                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InsertOutbound_Prepack, mqDto);
                                    }

                                    #endregion
                                }
                                else
                                {
                                    interfaceLogDto.doc_sysId = rsp.IsSuccess ? outboundSysId : new Guid?();
                                    interfaceLogDto.doc_order = rsp.IsSuccess ? outboundOrder : null;
                                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                                    interfaceLogDto.flag = rsp.IsSuccess;
                                    return rsp;
                                }
                                #endregion

                                //发送邮件通知
                                Guid sysCodeSysId = _crudRepository.FirstOrDefault<syscode>(p => p.SysCodeType == PublicConst.SysCodeTypeReceiptOutboundMail).SysId;
                                string mailTo = _crudRepository.FirstOrDefault<syscodedetail>(p => p.SysCodeSysId == sysCodeSysId && p.Code == "OutboundMail").Descr;
                                EmailHelper.SendMailAsync(PublicConst.NewOutboundSubject, string.Format(PublicConst.NewOutboundMailBody, outbound.OutboundOrder, outbound.OutboundDate), mailTo);


                                MQProcessDto<MQOrderRuleDto> bussinessProcessLogDto = new MQProcessDto<MQOrderRuleDto>()
                                {
                                    BussinessSysId = outboundSysId,
                                    BussinessOrderNumber = outboundOrder,
                                    Descr = "出库单自动分配",
                                    CurrentUserId = 99999,
                                    CurrentDisplayName = "WMSSystem",
                                    WarehouseSysId = warehouse.SysId,
                                    BussinessDto = new MQOrderRuleDto()
                                    {
                                        OrderSysId = outboundSysId,
                                        OrderNumber = outboundOrder,
                                        CurrentUserId = 99999,
                                        CurrentDisplayName = "WMSSystem",
                                        WarehouseSysId = warehouse.SysId
                                    }
                                };
                                RabbitWMS.SetRabbitMQAsync(RabbitMQType.Outbound_AutoAllocation, bussinessProcessLogDto);
                            }
                            else
                            {
                                interfaceLogDto.doc_sysId = rsp.IsSuccess ? outboundSysId : new Guid?();
                                interfaceLogDto.doc_order = rsp.IsSuccess ? outboundOrder : null;
                                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                                interfaceLogDto.flag = rsp.IsSuccess;
                                return rsp;
                            }
                        }
                        #endregion


                    }
                }
                else
                {
                    //throw new Exception("出库设置规则不存在，请检查");
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "出库设置规则不存在，请检查";
                    return rsp;
                }



                //记录接口日志
                interfaceLogDto.doc_sysId = rsp.IsSuccess ? outboundSysId : new Guid?();
                interfaceLogDto.doc_order = rsp.IsSuccess ? outboundOrder : null;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw ex;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }

        /// <summary>
        /// 插入仓库
        /// </summary>
        /// <returns></returns>
        public CommonResponse InsertWareHouse(ThirdPartyWareHouseDto thirdPartyWareHouseDto)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(thirdPartyWareHouseDto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.InsertOrUpdateWareHouse
            };
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                var editWareHouse = _crudRepository.GetQuery<warehouse>(p => p.OtherId == thirdPartyWareHouseDto.OtherId).FirstOrDefault();
                Guid? sysId = null;
                if (editWareHouse == null)
                {
                    sysId = Guid.NewGuid();
                    warehouse warehouse = new warehouse();
                    warehouse.SysId = sysId.Value;
                    warehouse.Name = thirdPartyWareHouseDto.Name;
                    warehouse.Address = thirdPartyWareHouseDto.Address;
                    warehouse.Contacts = thirdPartyWareHouseDto.Contacts;
                    warehouse.Telephone = thirdPartyWareHouseDto.Telephone;
                    warehouse.IsActive = true;
                    warehouse.CreateBy = 99999;
                    warehouse.CreateDate = DateTime.Now;
                    warehouse.UpdateBy = 99999;
                    warehouse.UpdateDate = DateTime.Now;
                    warehouse.OtherId = thirdPartyWareHouseDto.OtherId;
                    warehouse.URL = "";
                    if (PublicConst.SyncMultiWHSwitch)
                    {
                        new Task(() =>
                        {
                            ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateWarehouse", method: MethodType.Post, postData: warehouse);
                        }).Start();
                    }

                    _crudRepository.Insert(warehouse);

                    editWareHouse = warehouse;
                }
                else
                {
                    editWareHouse.Name = thirdPartyWareHouseDto.Name;
                    editWareHouse.Address = thirdPartyWareHouseDto.Address;
                    editWareHouse.Contacts = thirdPartyWareHouseDto.Contacts;
                    editWareHouse.Telephone = thirdPartyWareHouseDto.Telephone;
                    editWareHouse.UpdateBy = 99999;
                    editWareHouse.UpdateDate = DateTime.Now;
                    if (PublicConst.SyncMultiWHSwitch)
                    {
                        new Task(() =>
                        {
                            ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateWarehouse", method: MethodType.Post, postData: editWareHouse);
                        }).Start();
                    }

                    _crudRepository.Update(editWareHouse);

                }
                //记录接口日志
                interfaceLogDto.doc_sysId = rsp.IsSuccess ? editWareHouse.SysId : new Guid?();
                interfaceLogDto.doc_order = rsp.IsSuccess ? editWareHouse.Name : null;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw ex;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }



        #region 写入商城数据接口
        /// <summary>
        /// 商城入库接口
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public CommonResponse InsertInStock(int currentUserId, string currentDisplayName, Guid sysId, List<PurchaseDetailDto> purchaseDetailDtoList = null)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(new { SysId = sysId, PurchaseDetailDtoList = purchaseDetailDtoList }, currentUserId.ToString(), currentDisplayName)
            {
                interface_type = InterfaceType.Invoke.ToDescription(),
                interface_name = PublicConst.WriteERPPOStatus
            };
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                purchase purchase = _crudRepository.Get<purchase>(sysId);
                if (purchaseDetailDtoList == null)
                {
                    purchaseDetailDtoList = _crudRepository.GetQuery<purchasedetail>(p => p.PurchaseSysId == sysId).JTransformTo<PurchaseDetailDto>();
                }

                if (purchase != null)
                {
                    if (purchase.Source == PublicConst.ThirdPartySourceERP)
                    {
                        rsp = WriteBackReceiptERP(currentUserId, currentDisplayName, purchase, purchaseDetailDtoList);
                    }
                    else if (purchase.Source == PublicConst.ThirdPartySourceGZNB)
                    {
                        rsp = WriteBackReceiptGZNB(currentUserId, currentDisplayName, purchase, purchaseDetailDtoList);
                    }
                    else
                    {
                        rsp = WriteBackReceiptB2C(currentUserId, currentDisplayName, purchase, purchaseDetailDtoList);
                    }
                }
                else
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "采购单不存在";
                    //记录接口日志
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = false;
                    //发送MQ
                    interfaceLogDto.end_time = DateTime.Now;
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                }
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                throw ex;
            }
        }

        /// <summary>
        /// 入库回写农商后台接口
        /// </summary>
        /// <param name="purchase"></param>
        /// <param name="purchaseDetailDtoList"></param>
        private CommonResponse WriteBackReceiptB2C(int currentUserId, string currentDisplayName, purchase purchase, List<PurchaseDetailDto> purchaseDetailDtoList)
        {
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                ThirdPartyInStockB2CDto inStockDto = new ThirdPartyInStockB2CDto { Items = new List<ThirdPartyInStockDetailB2CDto>() };
                inStockDto.PoSysNo = Convert.ToInt32(purchase.ExternalOrder);
                inStockDto.UserSysNo = 1;
                if (purchase.Status == (int)PurchaseStatus.PartReceipt)
                {
                    inStockDto.Status = 5;
                }
                else if (purchase.Status == (int)PurchaseStatus.Finish)
                {
                    inStockDto.Status = 2;
                }
                inStockDto.DateTime = purchase.LastReceiptDate.HasValue ? purchase.LastReceiptDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty;
                if (purchaseDetailDtoList.Any())
                {
                    purchaseDetailDtoList.ForEach(p =>
                    {

                        inStockDto.Items.Add(new ThirdPartyInStockDetailB2CDto
                        {
                            ProductSysNo = p.OtherSkuId,
                            Quantity = p.ReceivedQty.Value, // 内存数据（item）- 未提交的数量（p） = 本次接受数量
                            RejectedQty = p.RejectedQty.Value,
                            Remark = p.Remark,
                        });
                    });
                }

                new Task(() =>
                {
                    var response = ApiClient.NExecute<ThirdPartyResponse>(PublicConst.B2CBaseUrl, "ERP/Po/InStock", method: MethodType.Post, postData: inStockDto, useEndpointPreffix: false);
                    if (response.Success && response.ResponseResult != null)
                    {
                        rsp.IsSuccess = response.ResponseResult.IsSuccess;
                    }
                    else
                    {
                        rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                    }
                    //记录接口日志
                    InterfaceLogDto interfaceLogDto = new InterfaceLogDto(inStockDto, currentUserId.ToString(), currentDisplayName)
                    {
                        doc_sysId = purchase.SysId,
                        doc_order = purchase.PurchaseOrder,
                        interface_type = InterfaceType.Invoke.ToDescription(),
                        interface_name = PublicConst.WriteGZNBPOStatus,
                        response_json = JsonConvert.SerializeObject(response),
                        flag = rsp.IsSuccess,
                        descr = "B2CBaseUrl|ERP/Po/InStock|" + "1"
                    };
                    //发送MQ
                    interfaceLogDto.end_time = DateTime.Now;
                    RabbitWMS.SetRabbitMQSync(RabbitMQType.InterfaceLog, interfaceLogDto);
                }).Start();
                return rsp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 入库回写农商后台接口
        /// </summary>
        /// <param name="purchase"></param>
        /// <param name="purchaseDetailDtoList"></param>
        private CommonResponse WriteBackReceiptGZNB(int currentUserId, string currentDisplayName, purchase purchase, List<PurchaseDetailDto> purchaseDetailDtoList)
        {
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                ThirdPartyInStockDto inStockDto = new ThirdPartyInStockDto { Items = new List<ThirdPartyInStockDetailDto>() };
                inStockDto.PoSysNo = Convert.ToInt32(purchase.ExternalOrder);
                inStockDto.UserSysNo = 1;
                if (purchase.Status == (int)PurchaseStatus.PartReceipt)
                {
                    inStockDto.Status = 5;
                }
                else if (purchase.Status == (int)PurchaseStatus.Finish)
                {
                    inStockDto.Status = 2;
                }
                inStockDto.DateTime = purchase.LastReceiptDate.HasValue ? purchase.LastReceiptDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty;
                if (purchaseDetailDtoList.Any())
                {
                    purchaseDetailDtoList.ForEach(p =>
                    {

                        inStockDto.Items.Add(new ThirdPartyInStockDetailDto
                        {
                            ProductSysNo = int.Parse(p.OtherSkuId),
                            Quantity = p.ReceivedQty.Value, // 内存数据（item）- 未提交的数量（p） = 本次接受数量
                            RejectedQty = p.RejectedQty.Value,
                            Remark = p.Remark,
                        });
                    });
                }

                new Task(() =>
                {
                    var response = ApiClient.NExecute<ThirdPartyResponse>(PublicConst.GZNBBaseUrl, "ErpApi/IntoStock", method: MethodType.Post, postData: inStockDto, useEndpointPreffix: false);
                    if (response.Success && response.ResponseResult != null)
                    {
                        rsp.IsSuccess = response.ResponseResult.IsSuccess;
                    }
                    else
                    {
                        rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                    }
                    //记录接口日志
                    InterfaceLogDto interfaceLogDto = new InterfaceLogDto(inStockDto, currentUserId.ToString(), currentDisplayName)
                    {
                        doc_sysId = purchase.SysId,
                        doc_order = purchase.PurchaseOrder,
                        interface_type = InterfaceType.Invoke.ToDescription(),
                        interface_name = PublicConst.WriteGZNBPOStatus,
                        response_json = JsonConvert.SerializeObject(response),
                        flag = rsp.IsSuccess,
                        descr = "GZNBBaseUrl|ErpApi/IntoStock|" + "1"
                    };
                    //发送MQ
                    interfaceLogDto.end_time = DateTime.Now;
                    RabbitWMS.SetRabbitMQSync(RabbitMQType.InterfaceLog, interfaceLogDto);
                }).Start();
                return rsp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 入库回写农商后台接口
        /// </summary>
        /// <param name="purchase"></param>
        /// <param name="purchaseDetailDtoList"></param>
        private CommonResponse WriteBackReceiptERP(int currentUserId, string currentDisplayName, purchase purchase, List<PurchaseDetailDto> purchaseDetailDtoList)
        {
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                var inStockOrderDetailDto = new InStockOrderDetailDto();
                inStockOrderDetailDto.SourceNumber = "";
                inStockOrderDetailDto.SourceType = 0;
                inStockOrderDetailDto.InStockOrderItemDetailList = new List<InStockOrderItemDetailDto>();

                if (purchaseDetailDtoList.Any())
                {
                    purchaseDetailDtoList.ForEach(p =>
                    {

                        inStockOrderDetailDto.InStockOrderItemDetailList.Add(new InStockOrderItemDetailDto
                        {
                            ProductCode = Convert.ToInt32(p.OtherSkuId),
                            Quantity = p.ReceivedQty.Value, // 内存数据（item）- 未提交的数量（p） = 本次接受数量
                            InStockQuantity = p.ReceivedQty.Value,
                            RejectedQuantity = p.RejectedQty.Value,
                            InStockGiftQuantity = p.GiftQty.Value,
                            RejectedGiftQuantity = p.RejectedGiftQty.Value,
                            DamagedQuantity = p.DamagedQuantity
                        });
                    });
                }

                InStockDto inStockDto = new InStockDto() { InStockOrderDetailList = new List<InStockOrderDetailDto>() };
                inStockDto.InStockOrderDetailList.Add(inStockOrderDetailDto);
                inStockDto.InStockOrderID = Convert.ToInt32(purchase.ExternalOrder);
                inStockDto.EditUserID = currentUserId;
                inStockDto.EditUserName = currentDisplayName;
                inStockDto.EditDate = DateTime.Now;

                if (purchase.Status == (int)PurchaseStatus.PartReceipt)
                {
                    inStockDto.Status = 2;
                }
                else if (purchase.Status == (int)PurchaseStatus.Finish)
                {
                    inStockDto.Status = 3;
                }

                if (PublicConst.IsAsyncECCBussinessByMQ)
                {
                    InStockOrderBussinessDto mqDto = new InStockOrderBussinessDto();
                    mqDto.InStockDto = inStockDto;

                    MQProcessDto<InStockOrderBussinessDto> bussinessProcessLogDto = new MQProcessDto<InStockOrderBussinessDto>()
                    {
                        BussinessSysId = purchase.SysId,
                        BussinessOrderNumber = purchase.PurchaseOrder,
                        //BussinessType = "InsertOutbound",
                        //BussinessTypeName = "快速出库",
                        Descr = "收货回写ECC",
                        CurrentUserId = currentUserId,
                        CurrentDisplayName = currentDisplayName,
                        WarehouseSysId = purchase.WarehouseSysId.Value,
                        BussinessDto = mqDto
                    };

                    //MQ 回写 ECC
                    RabbitWMS.SetRabbitMQSync(RabbitMQType.ReceiptERP_InStockOrder, bussinessProcessLogDto);
                }
                else
                {
                    // 防止 MQ 异常的备用 异步调用方案，一般情况下不会开启
                    new Task(() =>
                    {
                        var response = ApiClient.Post<ThirdPartyResponse>(PublicConst.ERPBaseUrl, "OMS/InStockOrder/InStock", new CoreQuery(), inStockDto);
                        if (response.Success && response.ResponseResult != null)
                        {
                            rsp.IsSuccess = response.ResponseResult.IsSuccess;
                        }
                        else
                        {
                            rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                        }
                        //记录接口日志
                        InterfaceLogDto interfaceLogDto = new InterfaceLogDto(inStockDto, currentUserId.ToString(), currentDisplayName)
                        {
                            doc_sysId = purchase.SysId,
                            doc_order = purchase.PurchaseOrder,
                            interface_type = InterfaceType.Invoke.ToDescription(),
                            interface_name = PublicConst.WriteOMSPOStatus,
                            response_json = JsonConvert.SerializeObject(response),
                            flag = rsp.IsSuccess,
                            descr = "ERPBaseUrl|OMS/InStockOrder/InStock|" + "WMSAPI"
                        };
                        //发送MQ
                        interfaceLogDto.end_time = DateTime.Now;
                        RabbitWMS.SetRabbitMQSync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }).Start();
                }

                return rsp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 商城出库接口
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public CommonResponse InsertOutStock(Guid sysId, string currentUserName, int currentUserId)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(sysId, currentUserId.ToString(), currentUserName)
            {
                interface_type = InterfaceType.Invoke.ToDescription(),
                interface_name = PublicConst.WriteERPSOStatus
            };
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                outbound outbound = _outboundRepository.GetOutboundInfoBySysId(sysId);
                if (outbound != null)
                {
                    if (outbound.Source == PublicConst.ThirdPartySourceERP)
                    {
                        rsp = WriteBackOutStockERP(outbound, currentUserName, currentUserId);
                    }
                    else
                    {
                        rsp = WriteBackOutStockGZNB(outbound, currentUserName, currentUserId);
                    }
                }
                else
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "出库单不存在";
                    //记录接口日志
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = false;
                    //发送MQ
                    interfaceLogDto.end_time = DateTime.Now;
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                }
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                throw ex;
            }
        }

        private CommonResponse WriteBackOutStockGZNB(outbound outbound, string currentUserName, int currentUserId)
        {
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                ThirdPartyOutStockDto outStockDto = new ThirdPartyOutStockDto();
                outStockDto.OrderSysNo = Convert.ToInt32(outbound.ExternOrderId);
                outStockDto.UserSysNo = 1;
                if (outbound.Status.GetValueOrDefault() == (int)OutboundStatus.Delivery)
                {
                    outStockDto.Status = 4;
                }
                //实际发货时间
                outStockDto.DateTime = outbound.ActualShipDate.HasValue ? outbound.ActualShipDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty;

                new Task(() =>
                {
                    var response = ApiClient.NExecute<ThirdPartyResponse>(PublicConst.GZNBBaseUrl, "ErpApi/Delivery", method: MethodType.Post, postData: outStockDto, useEndpointPreffix: false);
                    if (response.Success && response.ResponseResult != null)
                    {
                        rsp.IsSuccess = true;
                    }
                    else
                    {
                        rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                    }

                    //记录接口日志
                    InterfaceLogDto interfaceLogDto = new InterfaceLogDto(outStockDto, currentUserId.ToString(), currentUserName)
                    {
                        doc_sysId = outbound.SysId,
                        doc_order = outbound.OutboundOrder,
                        interface_type = InterfaceType.Invoke.ToDescription(),
                        interface_name = PublicConst.WriteGZNBSOStatus,
                        response_json = JsonConvert.SerializeObject(response),
                        flag = rsp.IsSuccess,
                        descr = "GZNBBaseUrl|ErpApi/Delivery|" + "1"
                    };
                    //发送MQ
                    interfaceLogDto.end_time = DateTime.Now;
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                }).Start();
                return rsp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private CommonResponse WriteBackOutStockERP(outbound outbound, string currentUserName, int currentUserId)
        {
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                OutStockDto outStockDto = new OutStockDto();
                outStockDto.OutStockOrderID = Convert.ToInt32(outbound.ExternOrderId);
                outStockDto.EditUserID = currentUserId;
                outStockDto.ExpressName = ""; //暂无
                outStockDto.ExpressCode = "";//暂无
                outStockDto.ExpressNumber = "";//暂无
                outStockDto.DeliveryUserName = currentUserName;
                //实际发货时间
                outStockDto.DeliveryDate = outbound.ActualShipDate.HasValue ? outbound.ActualShipDate.Value : DateTime.Now;

                //获取物流信息
                List<vanning> vanningList = _crudRepository.GetAllList<vanning>(o => o.OutboundSysId == outbound.SysId);
                if (vanningList != null && vanningList.Count > 0)
                {
                    List<Guid> vanningSysIds = vanningList.Select(o => o.SysId).ToList();
                    List<vanningdetail> vanningdetailList = _crudRepository.GetAllList<vanningdetail>(o => vanningSysIds.Contains(o.VanningSysId.HasValue ? (Guid)o.VanningSysId : Guid.Empty));
                    if (vanningdetailList != null && vanningdetailList.Count > 0)
                    {
                        outStockDto.ExpressName = "ZTO中通快递";
                        var carrier = _crudRepository.FirstOrDefault<carrier>(x => x.CarrierName == "ZTO中通快递");
                        if (carrier != null)
                        {
                            outStockDto.ExpressCode = carrier.OtherCarrierId;
                        }
                        List<string> carrierNumberList = vanningdetailList.Where(o => !string.IsNullOrEmpty(o.CarrierNumber)).Select(o => o.CarrierNumber).ToList();
                        if (carrierNumberList != null && carrierNumberList.Count > 0)
                        {
                            outStockDto.ExpressNumber = string.Join(",", carrierNumberList.ToArray());
                        }
                    }
                }

                var outStockOrderItemDetailList = new List<OutStockOrderItemDetailDto>();
                var detail = outbound.outbounddetails.ToList();
                var skuIdList = detail.Select(x => x.SkuSysId);
                var skuList = _crudRepository.GetQuery<sku>(x => skuIdList.Contains(x.SysId)).ToList();
                if (detail.Any())
                {
                    detail.ForEach(item =>
                    {
                        var sku = skuList.FirstOrDefault(x => x.SysId == item.SkuSysId);
                        var outStockOrderItemDetailDto = new OutStockOrderItemDetailDto();
                        outStockOrderItemDetailDto.ProductCode = int.Parse(sku.OtherId);
                        outStockOrderItemDetailDto.Quantity = item.ShippedQty;
                        outStockOrderItemDetailList.Add(outStockOrderItemDetailDto);
                    });
                }
                outStockDto.OutStockOrderDetailList = new List<OutStockOrderDetailDto>();
                outStockDto.OutStockOrderDetailList.Add(new OutStockOrderDetailDto()
                {
                    //  1： b2c销售单 2 ：B2B服务站铺货
                    SourceType = outbound.OutboundType == (int)OutboundType.B2C ? 1 : 2,
                    OutStockOrderItemDetailList = outStockOrderItemDetailList
                });


                if (PublicConst.IsAsyncECCBussinessByMQ)
                {
                    //MQ 回写 ECC
                    OutStockBussinessDto mqDto = new OutStockBussinessDto();
                    mqDto.OutStockDto = outStockDto;

                    MQProcessDto<OutStockBussinessDto> bussinessProcessLogDto = new MQProcessDto<OutStockBussinessDto>()
                    {
                        BussinessSysId = outbound.SysId,
                        BussinessOrderNumber = outbound.OutboundOrder,
                        //BussinessType = "InsertOutbound",
                        //BussinessTypeName = "快速出库",
                        Descr = "发货回写ECC",
                        CurrentUserId = currentUserId,
                        CurrentDisplayName = currentUserName,
                        WarehouseSysId = outbound.WareHouseSysId,
                        BussinessDto = mqDto
                    };

                    RabbitWMS.SetRabbitMQSync(RabbitMQType.OutStockERP_OutStock, bussinessProcessLogDto);
                }
                else
                {
                    // 防止 MQ 异常的备用 异步调用方案，一般情况下不会开启
                    new Task(() =>
                    {
                        var response = ApiClient.Post<ThirdPartyResponse>(PublicConst.ERPBaseUrl, "OMS/OutStockOrder/OutStock", new CoreQuery(), outStockDto);
                        if (response.Success && response.ResponseResult != null)
                        {
                            rsp.IsSuccess = response.ResponseResult.IsSuccess;
                        }
                        else
                        {
                            rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                        }

                        //记录接口日志
                        InterfaceLogDto interfaceLogDto = new InterfaceLogDto(outStockDto, currentUserId.ToString(), currentUserName)
                        {
                            doc_sysId = outbound.SysId,
                            doc_order = outbound.OutboundOrder,
                            interface_type = InterfaceType.Invoke.ToDescription(),
                            interface_name = PublicConst.WriteOMSSOStatus,
                            response_json = JsonConvert.SerializeObject(response),
                            flag = rsp.IsSuccess,
                            descr = "ERPBaseUrl|OMS/OutStockOrder/OutStock|" + "WMSAPI"
                        };
                        //发送MQ
                        interfaceLogDto.end_time = DateTime.Now;
                        RabbitWMS.SetRabbitMQSync(RabbitMQType.InterfaceLog, interfaceLogDto);
                        //_crudRepository.SetOperationLog(OperationLogType.Write, interfaceLogDto, "出库单" + outbound.OutboundOrder);
                    }).Start();
                }

                return rsp;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region 出库单
        /// <summary>
        /// 关闭出库单
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentUserName"></param>
        /// <returns></returns>
        public CommonResponse VoidOutbound(string orderCode, string wareHouseId, int currentUserId, string currentUserName)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(new { OrderCode = orderCode, wareHouseId = wareHouseId, currentUserId = currentUserId, currentUserName = currentUserName })
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.VoidOutbound
            };
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                if (string.IsNullOrEmpty(wareHouseId))
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "缺少关键参数 仓库ID wareHouseId,关闭失败";
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = false;
                    return rsp;
                }
                warehouse warehouse = RedisGetWareHouseByOtherId(wareHouseId);
                if (warehouse == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "仓库不存在,Id" + wareHouseId;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }
                _crudRepository.ChangeDB(warehouse.SysId);

                outbound outbound = _crudRepository.FirstOrDefault<outbound>(p => p.ExternOrderId == orderCode);
                if (new VoidOutboundCheck(rsp, outbound).Execute().IsSuccess)
                {
                    outbound.Status = (int)OutboundStatus.Close;
                    outbound.UpdateBy = currentUserId;
                    outbound.UpdateUserName = currentUserName;
                    outbound.UpdateDate = DateTime.Now;
                    _crudRepository.Update(outbound);

                    #region 如果B2B作废，通知TMS
                    if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.Fertilizer)
                    {   //5作废
                        var tmsDto = new ThirdPartyUpdateOutboundTypeDto()
                        {
                            OutboundSysId = outbound.SysId,
                            OutboundOrder = outbound.OutboundOrder,
                            OrderId = outbound.ExternOrderId,
                            Status = (int)TMSStatus.Close,
                            UpdateDate = DateTime.Now,
                            EditUserName = currentUserName,
                            UserId = currentUserId
                        };
                        UpdateOutboundTypeToTMS(tmsDto);
                    }
                    #endregion

                    #region 关闭出库单，将出库单绑定的散货封箱作废，更新出库单绑定的交接单到作废
                    _preBulkPackRepository.UpdatePreBulkPackStatus(outbound.SysId, currentUserId, currentUserName, (int)PreBulkPackStatus.Cancel);

                    //////还原拣货容器状态
                    ////var containerSysIds = _crudRepository.GetQuery<prebulkpack>(p => p.OutboundSysId == outbound.SysId && p.Status == (int)PreBulkPackStatus.RFPicking).Select(p => p.SysId).ToList();
                    ////_wmsSqlRepository.ClearContainer(new ClearContainerDto
                    ////{
                    ////    ContainerSysIds = containerSysIds,
                    ////    WarehouseSysId = warehouse.SysId,
                    ////    CurrentUserId = currentUserId,
                    ////    CurrentDisplayName = currentUserName
                    ////});
                    //////清除复核缓存
                    ////_redisAppService.CleanReviewRecords(outbound.OutboundOrder, warehouse.SysId);

                    //更新交接单到-->作废
                    _outboundTransferOrderRepository.UpdateOutboundTransferOrder(new OutboundTransferOrderQueryDto()
                    {
                        Status = (int)OutboundTransferOrderStatus.Cancel,
                        WarehouseSysId = warehouse.SysId,
                        OutboundSysId = outbound.SysId,
                        CurrentUserId = currentUserId,
                        CurrentDisplayName = currentUserName
                    });
                    #endregion

                }
                //记录接口日志
                interfaceLogDto.doc_sysId = outbound == null ? new Guid?() : outbound.SysId;
                interfaceLogDto.doc_order = outbound == null ? null : outbound.OutboundOrder;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;

                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw ex;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }

        #endregion

        #region 采购单
        /// <summary>
        /// 关闭采购单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        public CommonResponse ClosePurchase(ThirdPartyPurchaseOperateDto purchaseDto)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(purchaseDto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.ClosePurchase
            };
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };
                if (string.IsNullOrEmpty(purchaseDto.WareHouseId))
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "缺少关键参数 仓库ID wareHouseId,关闭失败";
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = false;
                    return rsp;
                }
                warehouse warehouse = RedisGetWareHouseByOtherId(purchaseDto.WareHouseId);
                if (warehouse == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "仓库不存在,Id" + purchaseDto.WareHouseId;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }
                _crudRepository.ChangeDB(warehouse.SysId);

                var purchase = _crudRepository.FirstOrDefault<purchase>(p => p.ExternalOrder == purchaseDto.ExternalOrder);

                if (purchase == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "采购单不存在";
                    return rsp;
                }

                if (purchase.Status == (int)PurchaseStatus.Finish)
                {
                    throw new Exception("已入库的采购单不能关闭");
                }
                if (purchase.Status == (int)PurchaseStatus.Void)
                {
                    throw new Exception("已作废的采购单不能关闭");
                }
                purchase.Status = (int)PurchaseStatus.Close;
                purchase.UpdateBy = purchaseDto.CurrentUserId;
                purchase.UpdateDate = DateTime.Now;
                purchase.UpdateUserName = purchaseDto.CurrentDisplayName;
                _crudRepository.Update(purchase);

                var receiptList = _crudRepository.GetQuery<receipt>(x => x.ExternalOrder == purchase.PurchaseOrder);
                if (receiptList != null && receiptList.Count() > 0)
                {
                    foreach (var item in receiptList)
                    {
                        var receipt = _crudRepository.Get<receipt>(item.SysId);
                        receipt.TS = Guid.NewGuid();
                        _crudRepository.Update(receipt);
                    }
                }

                if (purchase.FromWareHouseSysId.HasValue && purchase.OutboundSysId.HasValue)
                {
                    PurchaseForReturnDto purchaseReturn = purchase.JTransformTo<PurchaseForReturnDto>();
                    purchaseReturn.UpdateBy = purchaseDto.CurrentUserId;
                    purchaseReturn.UpdateUserName = purchaseDto.CurrentDisplayName;
                    var response = ApiClient.Post(PublicConst.WmsApiUrl, "/Outbound/CancelOutboundReturnByPurchase", new CoreQuery(), purchaseReturn);

                    if (!response.Success)
                    {
                        throw new Exception($"作废入库单失败(CancelOutboundReturnByPurchase):{response.ApiMessage.ErrorMessage}");
                    }
                }

                //记录接口日志
                interfaceLogDto.doc_sysId = purchase == null ? new Guid?() : purchase.SysId;
                interfaceLogDto.doc_order = purchase == null ? null : purchase.PurchaseOrder;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;

                return rsp;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("操作失败");
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw ex;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }
        #endregion

        #region 组装单
        /// <summary>
        /// 组装单
        /// </summary>
        /// <param name="assemblyDto"></param>
        /// <returns></returns>
        public CommonResponse InsertAssembly(ThirdPartyAssemblyDto assemblyDto)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(assemblyDto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.InsertAssembly
            };
            var assemblySysId = Guid.NewGuid();
            string assemblyOrder = null;
            CommonResponse rsp = new CommonResponse { IsSuccess = true };
            warehouse warehouse = RedisGetWareHouseByOtherId(assemblyDto.WarehouseId);
            if (warehouse == null)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = "仓库不存在,Id" + assemblyDto.WarehouseId;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                return rsp;
            }
            _crudRepository.ChangeDB(warehouse.SysId);

            try
            {
                if (assemblyDto != null)
                {


                    var sku = _crudRepository.GetQuery<sku>(x => x.OtherId == assemblyDto.OtherSkuId).FirstOrDefault();
                    if (sku == null)
                    {
                        rsp.IsSuccess = false;
                        rsp.ErrorMessage = "未找到对应的商品,Id" + assemblyDto.OtherSkuId;
                        return rsp;
                    }

                    if (assemblyDto.ThirdPartyAssemblyDetailDtoList != null && assemblyDto.ThirdPartyAssemblyDetailDtoList.Count > 0)
                    {
                        #region 组装单
                        var assembly = new assembly()
                        {
                            SysId = Guid.NewGuid(),
                            //AssemblyOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberAssembly),
                            AssemblyOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberAssembly),
                            ExternalOrder = assemblyDto.ExternalOrder,
                            SkuSysId = sku.SysId,
                            Status = (int)AssemblyStatus.New,
                            Remark = assemblyDto.Remark,
                            PlanProcessingDate = assemblyDto.PlanProcessingDate,
                            PlanCompletionDate = assemblyDto.PlanCompletionDate,
                            PlanQty = assemblyDto.PlanQty,
                            Packing = assemblyDto.Packing,
                            PackWeight = assemblyDto.PackWeight,
                            PackGrade = assemblyDto.PackGrade,
                            StorageConditions = assemblyDto.StorageConditions,
                            PackSpecification = assemblyDto.PackSpecification,
                            PackDescr = assemblyDto.PackDescr,
                            ActualQty = 0,
                            ShelvesQty = 0,
                            ShelvesStatus = (int)ShelvesStatus.NotOnShelves,
                            Source = "ECC",
                            WareHouseSysId = warehouse.SysId,
                            CreateBy = assemblyDto.CurrentUserId,
                            CreateDate = DateTime.Now,
                            CreateUserName = assemblyDto.CurrentDisplayName,
                            UpdateBy = assemblyDto.CurrentUserId,
                            UpdateDate = DateTime.Now,
                            UpdateUserName = assemblyDto.CurrentDisplayName,
                            Channel = assemblyDto.Channel,
                            BatchNumber = assemblyDto.BatchNumber
                        };
                        assemblySysId = assembly.SysId;
                        assemblyOrder = assembly.AssemblyOrder;
                        #endregion

                        #region 组装件
                        component component = null;
                        var oldComponent = _crudRepository.GetQuery<component>(x => x.SkuSysId == sku.SysId).FirstOrDefault();
                        if (oldComponent == null)
                        {
                            component = new component()
                            {
                                SysId = Guid.NewGuid(),
                                SkuSysId = sku.SysId,
                                TimeConsuming = 0,
                                Status = 0,
                                CreateBy = assemblyDto.CurrentUserId,
                                CreateDate = DateTime.Now,
                                CreateUserName = assemblyDto.CurrentDisplayName,
                                UpdateBy = assemblyDto.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                UpdateUserName = assemblyDto.CurrentDisplayName
                            };
                        }
                        #endregion

                        List<string> otherSkuIdList = assemblyDto.ThirdPartyAssemblyDetailDtoList.Select(p => p.OtherSkuId).ToList();
                        List<sku> skuList = _crudRepository.GetAllList<sku>(p => otherSkuIdList.Contains(p.OtherId));
                        List<Guid> packSysIdList = skuList.Select(p => p.PackSysId).ToList();
                        List<pack> packList = _crudRepository.GetAllList<pack>(p => packSysIdList.Contains(p.SysId));

                        #region 组装单明细
                        var assemblydetailList = new List<assemblydetail>();
                        var componentdetailList = new List<componentdetail>();
                        //var unitconversiontranList = new List<unitconversiontran>();
                        foreach (var detail in assemblyDto.ThirdPartyAssemblyDetailDtoList)
                        {
                            var detailSku = skuList.FirstOrDefault(x => x.OtherId == detail.OtherSkuId);
                            if (detailSku == null)
                            {
                                rsp.IsSuccess = false;
                                rsp.ErrorMessage = "未找到对应的商品,Id" + detail.OtherSkuId;
                                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                                interfaceLogDto.flag = rsp.IsSuccess;
                                return rsp;
                            }

                            var detailPack = packList.FirstOrDefault(x => x.SysId == detailSku.PackSysId);
                            if (detailSku == null)
                            {
                                rsp.IsSuccess = false;
                                rsp.ErrorMessage = "未找到对应的包装,商品Id" + detail.OtherSkuId;
                                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                                interfaceLogDto.flag = rsp.IsSuccess;
                                return rsp;
                            }

                            var assemblyDetail = new assemblydetail()
                            {
                                SysId = Guid.NewGuid(),
                                AssemblySysId = assembly.SysId,
                                SkuSysId = detailSku.SysId,
                                UnitQty = detail.UnitQty,
                                Qty = (int)detail.UnitQty,
                                LossQty = 0,
                                Grade = detail.Grade,
                                AllocatedQty = 0,
                                PickedQty = 0,
                                Status = (int)AssemblyDetailStatus.New,
                                CreateBy = assemblyDto.CurrentUserId,
                                CreateDate = DateTime.Now,
                                CreateUserName = assemblyDto.CurrentDisplayName,
                                UpdateBy = assemblyDto.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                UpdateUserName = assemblyDto.CurrentDisplayName
                            };
                            assemblydetailList.Add(assemblyDetail);

                            #region 组装件明细
                            var componentDetail = new componentdetail()
                            {
                                SysId = Guid.NewGuid(),
                                ComponentSysId = oldComponent != null ? oldComponent.SysId : component.SysId,
                                SkuSysId = detailSku.SysId,
                                Qty = Convert.ToInt32(assemblyDetail.Qty / assembly.PlanQty),
                                LossQty = 0,
                                IsMain = false,
                                Status = 0,
                                CreateBy = assemblyDto.CurrentUserId,
                                CreateDate = DateTime.Now,
                                CreateUserName = assemblyDto.CurrentDisplayName,
                                UpdateBy = assemblyDto.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                UpdateUserName = assemblyDto.CurrentDisplayName
                            };
                            componentdetailList.Add(componentDetail);
                            #endregion

                            #region 单位转换
                            //var transQty = 0;
                            //var transPack = new pack();
                            //if (_packageAppService.GetSkuConversiontransQty(detailSku.SysId, Convert.ToInt32(detail.UnitQty), out transQty, ref transPack) == true)
                            //{
                            //    //gavin: 单位转换更新库存后需要记录
                            //    var unitTran = new unitconversiontran()
                            //    {
                            //        WareHouseSysId = assembly.WareHouseSysId,
                            //        DocOrder = assembly.AssemblyOrder,
                            //        DocSysId = assembly.SysId,
                            //        DocDetailSysId = assemblyDetail.SysId,
                            //        SkuSysId = assemblyDetail.SkuSysId,
                            //        FromQty = detail.UnitQty,
                            //        ToQty = transQty,
                            //        Loc = "",
                            //        Lot = "",
                            //        Lpn = "",
                            //        Status = "Done",
                            //        PackSysId = transPack.SysId,
                            //        PackCode = transPack.PackCode,
                            //        FromUOMSysId = transPack.FieldUom02 ?? Guid.Empty,
                            //        ToUOMSysId = transPack.FieldUom01 ?? Guid.Empty,
                            //        CreateBy = assemblyDto.CurrentUserId,
                            //        CreateDate = DateTime.Now,
                            //        UpdateBy = assemblyDto.CurrentUserId,
                            //        UpdateDate = DateTime.Now
                            //    };
                            //    unitconversiontranList.Add(unitTran);
                            //}

                            ////转换后的数量
                            //assemblyDetail.Qty = transQty;
                            #endregion

                        }
                        #endregion

                        _crudRepository.Insert(assembly);
                        _crudRepository.BatchInsert(assemblydetailList);
                        if (oldComponent != null)
                        {
                            _crudRepository.Delete<componentdetail>(x => x.ComponentSysId == oldComponent.SysId);
                        }
                        else
                        {
                            _crudRepository.Insert(component);
                        }
                        _crudRepository.BatchInsert(componentdetailList);

                        //_crudRepository.BatchInsert(unitconversiontranList);
                    }
                    else
                    {
                        rsp.IsSuccess = false;
                        rsp.ErrorMessage = "组装单明细为空";
                        interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                        interfaceLogDto.flag = rsp.IsSuccess;
                        return rsp;
                    }
                }
                else
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "组装单为空";
                }

                //记录接口日志
                interfaceLogDto.doc_sysId = rsp.IsSuccess ? assemblySysId : new Guid?();
                interfaceLogDto.doc_order = rsp.IsSuccess ? assemblyOrder : null;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                return rsp;
            }
            catch (Exception ex)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
                throw ex;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }

        /// <summary>
        /// 回写ECC加工单
        /// </summary>
        /// <param name="assemblyWriteBack"></param>
        /// <returns></returns>
        public CommonResponse WriteBackECCAssembly(assembly assembly, int currentUserId, string currentDisplayName)
        {
            CommonResponse rsp = new CommonResponse { IsSuccess = true };
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto()
            {
                interface_type = InterfaceType.Invoke.ToDescription(),
                interface_name = PublicConst.WriteBackeECCAssembly,
                user_id = currentUserId.ToString(),
                user_name = currentDisplayName
            };
            try
            {
                var assemblyDetailsList = _crudRepository.GetQuery<assemblydetail>(x => x.AssemblySysId == assembly.SysId);
                List<Guid> skuSysIdList = assemblyDetailsList.Select(p => p.SkuSysId).ToList();
                skuSysIdList.Add(assembly.SkuSysId);
                List<sku> skuList = _crudRepository.GetAllList<sku>(p => skuSysIdList.Contains(p.SysId));

                if (assembly.Source.Equals("ECC", StringComparison.OrdinalIgnoreCase))
                {
                    #region 组织加工单明细
                    var thirdPartyAssemblyDetailWriteBackDtoList = new List<ThirdPartyAssemblyDetailWriteBackDto>();

                    foreach (var item in assemblyDetailsList)
                    {
                        var sku = skuList.FirstOrDefault(x => x.SysId == item.SkuSysId);
                        if (sku == null)
                        {
                            throw new Exception("商品不存在");
                        }

                        thirdPartyAssemblyDetailWriteBackDtoList.Add(new ThirdPartyAssemblyDetailWriteBackDto()
                        {
                            ProductCode = int.Parse(sku.OtherId),
                            LossQuantity = (int)item.LossQty
                        });
                    }
                    #endregion

                    var assemblyWriteBack = new ThirdPartyAssemblyWriteBackDto()
                    {
                        RmpOrderId = int.Parse(assembly.ExternalOrder),
                        ActualToProcessTime = (DateTime)assembly.ActualProcessingDate,
                        ActualToCompletedTime = (DateTime)assembly.ActualCompletionDate,
                        ActualToCompletedQuantity = assembly.ActualQty,
                        CurrentUserId = currentUserId,
                        CurrentUserName = currentDisplayName,
                        RawMaterialsDetail = thirdPartyAssemblyDetailWriteBackDtoList
                    };
                    interfaceLogDto.request_json = JsonConvert.SerializeObject(assemblyWriteBack);

                    new Task(() =>
                    {
                        var response = ApiClient.Post<ThirdPartyResponse>(PublicConst.ERPBaseUrl, "RMPOrder/ModifyActualProcessInfo", new CoreQuery(), assemblyWriteBack);
                        if (response.Success && response.ResponseResult != null)
                        {
                            rsp.IsSuccess = response.ResponseResult.IsSuccess;
                            rsp.ErrorMessage = response.ResponseResult.ErrorMessage;
                        }
                        else
                        {
                            rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                        }

                        //记录接口日志
                        interfaceLogDto.doc_sysId = assembly.SysId;
                        interfaceLogDto.doc_order = assembly.AssemblyOrder;
                        interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                        interfaceLogDto.flag = rsp.IsSuccess;
                        interfaceLogDto.descr = "ERPBaseUrl|RMPOrder/ModifyActualProcessInfo|" + "1";
                        //发送MQ
                        interfaceLogDto.end_time = DateTime.Now;
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }).Start();
                }
                else if (assembly.Source.Equals("WMS", StringComparison.OrdinalIgnoreCase))
                {
                    var sourcesku = skuList.FirstOrDefault(x => x.SysId == assembly.SkuSysId);
                    var warehouse = _crudRepository.GetQuery<warehouse>(x => x.SysId == assembly.WareHouseSysId).First();
                    var assemblyWriteBack = new ThirdPartyWMSAssemblyWriteBackDto()
                    {
                        WarehouseID = warehouse.OtherId,
                        CreateDate = assembly.CreateDate,
                        CreateUserName = assembly.CreateUserName,
                        SourceNumber = assembly.AssemblyOrder,
                        ProductCode = sourcesku.OtherId,
                        IncreaseQty = assembly.ActualQty
                    };
                    assemblyWriteBack.MachiningSingleDetailList = new List<ThirdPartyWMSAssemblyDetailWriteBackDto>();

                    foreach (var item in assemblyDetailsList)
                    {
                        var sku = skuList.FirstOrDefault(x => x.SysId == item.SkuSysId);
                        if (sku == null)
                        {
                            throw new Exception("商品不存在");
                        }

                        assemblyWriteBack.MachiningSingleDetailList.Add(new ThirdPartyWMSAssemblyDetailWriteBackDto()
                        {
                            ProductCode = sku.OtherId,
                            DeductionQty = (int)item.Qty
                        });
                    }

                    interfaceLogDto.request_json = JsonConvert.SerializeObject(assemblyWriteBack);

                    new Task(() =>
                    {
                        var response = ApiClient.Post<ThirdPartyResponse>(PublicConst.ERPBaseUrl, "Inventory/MachiningSingle/InMachiningSingle", new CoreQuery(), assemblyWriteBack);
                        if (response.Success && response.ResponseResult != null)
                        {
                            rsp.IsSuccess = response.ResponseResult.IsSuccess;
                            rsp.ErrorMessage = response.ResponseResult.ErrorMessage;
                        }
                        else
                        {
                            rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                        }

                        //记录接口日志
                        interfaceLogDto.doc_sysId = assembly.SysId;
                        interfaceLogDto.doc_order = assembly.AssemblyOrder;
                        interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                        interfaceLogDto.flag = rsp.IsSuccess;
                        interfaceLogDto.descr = "ERPBaseUrl|Inventory/MachiningSingle/InMachiningSingle|" + "1";
                        //发送MQ
                        interfaceLogDto.end_time = DateTime.Now;
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }).Start();
                }

                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                throw ex;
            }
        }

        /// <summary>
        /// 回写ECC加工单领料
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentDisplayName"></param>
        /// <returns></returns>
        public CommonResponse WriteBackECCModifyAssemblyStatus(assembly assembly, int currentUserId, string currentDisplayName)
        {
            CommonResponse rsp = new CommonResponse { IsSuccess = true };

            InterfaceLogDto interfaceLogDto = new InterfaceLogDto()
            {
                interface_type = InterfaceType.Invoke.ToDescription(),
                interface_name = PublicConst.WriteBackeECCAssemblyStatus,
                user_id = currentUserId.ToString(),
                user_name = currentDisplayName
            };
            try
            {
                if (assembly.Source.Equals("ECC", StringComparison.OrdinalIgnoreCase))
                {
                    ThirdPartyAssemblyModifyStatusDto assemblyWriteBack = new ThirdPartyAssemblyModifyStatusDto()
                    {
                        RmpOrderId = int.Parse(assembly.ExternalOrder),
                        CurrentUserId = currentUserId,
                        CurrentUserName = currentDisplayName
                    };
                    interfaceLogDto.request_json = JsonConvert.SerializeObject(assemblyWriteBack);

                    new Task(() =>
                    {
                        var response = ApiClient.Post<ThirdPartyResponse>(PublicConst.ERPBaseUrl, "RMPOrder/StartProcess", new CoreQuery(), assemblyWriteBack);
                        if (response.Success && response.ResponseResult != null)
                        {
                            rsp.IsSuccess = response.ResponseResult.IsSuccess;
                            rsp.ErrorMessage = response.ResponseResult.ErrorMessage;
                        }
                        else
                        {
                            rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                        }

                        //记录接口日志
                        interfaceLogDto.doc_sysId = assembly.SysId;
                        interfaceLogDto.doc_order = assembly.AssemblyOrder;
                        interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                        interfaceLogDto.flag = rsp.IsSuccess;
                        interfaceLogDto.descr = "ERPBaseUrl|RMPOrder/StartProcess|" + "1";

                        //发送MQ
                        interfaceLogDto.end_time = DateTime.Now;
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    }).Start();
                }

                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                throw ex;
            }
        }
        #endregion

        #region 移仓单

        public CommonResponse CreateTransferInventory(ThirdPartTransferInventoryDto request)
        {
            CommonResponse rsp = new CommonResponse { IsSuccess = true };

            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(new { OrderCode = request.ExternOrderId })
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.CreateTransInventory,
                request_json = JsonConvert.SerializeObject(request),
                start_time = DateTime.Now
            };
            try
            {
                if (request.ThirdPartTransferInventoryDetailList == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "移仓商品信息不完整，请检查";
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }

                Guid tiSysId = Guid.NewGuid();

                warehouse warehouse = RedisGetWareHouseByOtherId(request.FromWarehouseId);
                if (warehouse == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "仓库不存在,Id" + request.FromWarehouseId;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }
                _crudRepository.ChangeDB(warehouse.SysId);

                var fromWarehouse = _crudRepository.GetQuery<warehouse>(p => p.OtherId == request.FromWarehouseId).FirstOrDefault();
                if (fromWarehouse == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "仓库不存在,Id" + request.FromWarehouseId;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }

                var toWarehouse = _crudRepository.GetQuery<warehouse>(p => p.OtherId == request.ToWarehouseId).FirstOrDefault();
                if (toWarehouse == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "仓库不存在,Id" + request.ToWarehouseId;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }

                transferinventory ti = new transferinventory();
                //ti.TransferInventoryOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberTransferInventory);
                ti.TransferInventoryOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberTransferInventory);
                ti.SysId = tiSysId;
                ti.Status = (int)TransferInventoryStatus.New;
                ti.FromWareHouseName = fromWarehouse.Name;
                ti.FromWareHouseSysId = fromWarehouse.SysId;
                ti.ToWareHouseSysId = toWarehouse.SysId;
                ti.ToWareHouseName = toWarehouse.Name;
                ti.ExternOrderDate = request.ExternOrderDate;
                ti.ExternOrderId = request.ExternOrderId;
                ti.Remark = request.Remark;
                ti.AuditingBy = request.AuditingBy;
                ti.AuditingName = request.AuditingName;
                ti.AuditingDate = request.AuditingDate;
                ti.ShippingMethod = request.ShippingMethod;
                ti.Freight = request.Freight;
                ti.CreateBy = 99999;
                ti.CreateUserName = "ECC";
                ti.CreateDate = DateTime.Now;
                //移仓出库增加渠道
                ti.Channel = string.IsNullOrEmpty(request.Channel) ? "" : request.Channel;

                List<string> otherSkuIdList = request.ThirdPartTransferInventoryDetailList.Select(p => p.OtherId).ToList();
                List<sku> skuList = _crudRepository.GetAllList<sku>(p => otherSkuIdList.Contains(p.OtherId));
                List<Guid> packSysIdList = skuList.Select(p => p.PackSysId).ToList();
                List<pack> packList = _crudRepository.GetAllList<pack>(p => packSysIdList.Contains(p.SysId));
                List<Guid?> fieldUom01List = packList.Select(p => p.FieldUom01).ToList();
                List<uom> uomList = _crudRepository.GetAllList<uom>(p => fieldUom01List.Contains(p.SysId));

                ti.transferinventorydetails = new List<transferinventorydetail>();
                request.ThirdPartTransferInventoryDetailList.ForEach(p =>
                {

                    sku sku = skuList.FirstOrDefault(q => q.OtherId == p.OtherId);

                    if (sku == null)
                    {
                        rsp.IsSuccess = false;
                        rsp.ErrorMessage = "商品不存在";
                        throw new Exception("商品不存在");
                    }

                    var pack = packList.FirstOrDefault(q => q.SysId == sku.PackSysId);
                    var uom = uomList.FirstOrDefault(q => q.SysId == pack.FieldUom01);

                    ti.transferinventorydetails.Add(new transferinventorydetail()
                    {
                        TransferInventorySysId = tiSysId,
                        SkuSysId = sku.SysId,
                        Qty = p.Qty,
                        UOMSysId = uom.SysId,
                        PackSysId = sku.PackSysId,
                        Remark = p.Remark,
                        Status = (int)TransferInventoryDetailStatus.New,
                        ReceivedQty = 0,
                        RejectedQty = 0,
                        ShippedQty = 0,
                        PackFactor = p.PackFactor
                    });
                });

                _crudRepository.Insert(ti);

                //记录接口日志
                interfaceLogDto.doc_sysId = tiSysId;
                interfaceLogDto.doc_order = ti.TransferInventoryOrder;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                interfaceLogDto.end_time = DateTime.Now;

                AddOutboundByTransferInventory(ti);

                //推送移仓出库MQ
                //RabbitWMS.SetRabbitMQAsync(RabbitMQType.TransferInventoryOutbound, mqDto);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
                throw new Exception("操作失败");
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
                throw new Exception(ex.Message);
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
            return rsp;
        }

        #region 移仓单创建出库单
        /// <summary>
        /// 移仓单创建出库单
        /// </summary>
        /// <param name="transferInventoryDto"></param>
        /// <returns></returns>
        public CommonResponse AddOutboundByTransferInventory(transferinventory transferInventory)
        {
            var response = new CommonResponse() { IsSuccess = false };
            try
            {
                //if (transferInventory != null)
                //{
                //var transferInventory = _crudRepository.GetQuery<transferinventory>(x => x.TransferInventoryOrder == transferInventoryDto.TransferInventoryOrder).FirstOrDefault();
                if (transferInventory != null)
                {
                    var warehouse = _crudRepository.Get<warehouse>(transferInventory.ToWareHouseSysId);
                    if (warehouse == null)
                    {
                        throw new Exception("仓库不存在");
                    }

                    //var transferInventoryDetails = _crudRepository.GetQuery<transferinventorydetail>(x => x.TransferInventorySysId == transferInventory.SysId).ToList();
                    var transferInventoryDetails = transferInventory.transferinventorydetails;

                    var outbound = new outbound()
                    {
                        SysId = Guid.NewGuid(),
                        WareHouseSysId = transferInventory.FromWareHouseSysId,
                        //OutboundOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberOutbound),
                        OutboundOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberOutbound),
                        OutboundType = (int)OutboundType.TransferInventory,
                        Status = (int)OutboundStatus.New,
                        OutboundDate = DateTime.Now,
                        Remark = transferInventory.Remark,
                        ConsigneeName = warehouse.Contacts,
                        ConsigneeAddress = warehouse.Address,
                        ConsigneePhone = warehouse.Telephone,
                        TotalShippedQty = 0,
                        TotalAllocatedQty = 0,
                        TotalPickedQty = 0,
                        TotalQty = transferInventoryDetails.Sum(x => x.Qty),
                        TotalPrice = 0,
                        ExternOrderDate = transferInventory.ExternOrderDate,
                        ExternOrderId = transferInventory.TransferInventoryOrder,
                        ShippingMethod = transferInventory.ShippingMethod,
                        Freight = transferInventory.Freight,
                        Source = PublicConst.ThirdPartySourceERP,
                        AuditingBy = transferInventory.AuditingBy,
                        AuditingDate = transferInventory.AuditingDate,
                        AuditingName = transferInventory.AuditingName,
                        CreateBy = transferInventory.CreateBy,
                        CreateDate = DateTime.Now,
                        CreateUserName = transferInventory.CreateUserName,
                        UpdateBy = transferInventory.UpdateBy,
                        UpdateDate = DateTime.Now,
                        UpdateUserName = transferInventory.UpdateUserName,
                        //移仓出库增加渠道
                        Channel = transferInventory.Channel
                    };
                    _crudRepository.Insert(outbound);

                    transferInventory.TransferOutboundSysId = outbound.SysId;
                    transferInventory.TransferOutboundOrder = outbound.OutboundOrder;
                    transferInventory.UpdateBy = transferInventory.UpdateBy;
                    transferInventory.UpdateDate = DateTime.Now;
                    transferInventory.UpdateUserName = transferInventory.UpdateUserName;
                    //_crudRepository.Update(transferInventory);

                    foreach (var detail in transferInventoryDetails)
                    {
                        var outboundDetail = new outbounddetail()
                        {
                            SysId = Guid.NewGuid(),
                            OutboundSysId = outbound.SysId,
                            SkuSysId = detail.SkuSysId,
                            Status = (int)OutboundDetailStatus.New,
                            UOMSysId = detail.UOMSysId,
                            PackSysId = detail.PackSysId,
                            Loc = detail.Loc,
                            Lot = detail.Lot,
                            Lpn = detail.Lpn,
                            LotAttr01 = detail.LotAttr01,
                            LotAttr02 = detail.LotAttr02,
                            LotAttr03 = detail.LotAttr03,
                            LotAttr04 = detail.LotAttr04,
                            LotAttr05 = detail.LotAttr05,
                            LotAttr06 = detail.LotAttr06,
                            LotAttr07 = detail.LotAttr07,
                            LotAttr08 = detail.LotAttr08,
                            LotAttr09 = detail.LotAttr09,
                            ExternalLot = detail.ExternalLot,
                            ProduceDate = detail.ProduceDate,
                            ExpiryDate = detail.ExpiryDate,
                            Qty = detail.Qty,
                            ShippedQty = 0,
                            AllocatedQty = 0,
                            PickedQty = 0,
                            Price = 0,
                            PackFactor = detail.PackFactor,
                            CreateBy = detail.CreateBy,
                            CreateDate = DateTime.Now,
                            CreateUserName = detail.CreateUserName,
                            UpdateBy = detail.UpdateBy,
                            UpdateDate = DateTime.Now,
                            UpdateUserName = detail.UpdateUserName
                        };
                        _crudRepository.Insert(outboundDetail);
                    }
                }
                else
                {
                    throw new Exception("移仓单不存在");
                }
                //}
                //else
                //{
                //    throw new Exception("输入参数为空,调用失败");
                //}
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                throw new Exception(ex.Message);
            }
            return response;
        }
        #endregion

        /// <summary>
        /// 移仓单出库完成调用ECC接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CommonResponse WriteBackTransferInventoryByOutbound(transferinventory transferInv)
        {
            CommonResponse rsp = new CommonResponse { IsSuccess = true };

            InterfaceLogDto interfaceLogDto = new InterfaceLogDto()
            {
                interface_type = InterfaceType.Invoke.ToDescription(),
                interface_name = PublicConst.WriteBackTransInventoryOutbound,
                user_id = transferInv.UpdateBy.ToString(),
                user_name = transferInv.UpdateUserName.ToString()
            };

            try
            {
                //调用ECC接口出库回写
                var request = new ThirdPartyTransferInventoryOutBackDto()
                {
                    ShiftOrderID = int.Parse(transferInv.ExternOrderId),
                    UpdateUserID = transferInv.UpdateBy,
                    UpdateUserName = transferInv.UpdateUserName,
                    UpdateDate = DateTime.Now,
                    Remark = transferInv.Remark
                };

                interfaceLogDto.request_json = JsonConvert.SerializeObject(request);

                new Task(() =>
                {
                    var response = ApiClient.Post<ThirdPartyResponse>(PublicConst.ERPBaseUrl, "Inventory/ShiftOrder/ShiftOrderStorageOut", new CoreQuery(), request);
                    if (response.Success && response.ResponseResult != null)
                    {
                        rsp.IsSuccess = response.ResponseResult.IsSuccess;
                        rsp.ErrorMessage = response.ResponseResult.ErrorMessage;
                    }
                    else
                    {
                        rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                    }

                    //记录接口日志
                    interfaceLogDto.doc_sysId = transferInv.SysId;
                    interfaceLogDto.doc_order = transferInv.TransferInventoryOrder;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    interfaceLogDto.descr = "ERPBaseUrl|Inventory/ShiftOrder/ShiftOrderStorageOut|" + "1";

                    //发送MQ
                    interfaceLogDto.end_time = DateTime.Now;
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                }).Start();

                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                throw ex;
            }
        }

        /// <summary>
        /// 移仓单入库调用ECC接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CommonResponse WriteBackTransferInventoryByInbound(transferinventory transferInv, List<PurchaseDetailViewDto> PurchaseDetailViewDto)
        {
            CommonResponse rsp = new CommonResponse { IsSuccess = true };

            InterfaceLogDto interfaceLogDto = new InterfaceLogDto()
            {
                interface_type = InterfaceType.Invoke.ToDescription(),
                interface_name = PublicConst.WriteBackTransInventoryInbound,
                user_id = transferInv.UpdateBy.ToString(),
                user_name = transferInv.UpdateUserName.ToString()
            };
            try
            {
                var shiftOrderStorageInDetailList = new List<ThirdPartyTransferInventoryInboundDetailBackDto>();

                if (PurchaseDetailViewDto.Any())
                {
                    List<Guid?> skuSysIds = PurchaseDetailViewDto.Select(p => p.SkuSysId).ToList();
                    List<sku> skuList = _crudRepository.GetAllList<sku>(p => skuSysIds.Contains(p.SysId));
                    foreach (var info in PurchaseDetailViewDto)
                    {
                        var sku = skuList.FirstOrDefault(p => p.SysId == info.SkuSysId);
                        var otherId = int.Parse(sku.OtherId);
                        shiftOrderStorageInDetailList.Add(new ThirdPartyTransferInventoryInboundDetailBackDto()
                        {
                            ProductNo = otherId,
                            Counts = (int)info.ReceivedQty,
                            RejectCounts = (int)info.RejectedQty
                        });
                    }
                }

                //调用ECC接口出库回写
                var request = new ThirdPartyTransferInventoryInboundBackDto()
                {
                    ShiftOrderID = int.Parse(transferInv.ExternOrderId),
                    UpdateUserID = transferInv.UpdateBy,
                    UpdateUserName = transferInv.UpdateUserName,
                    UpdateDate = DateTime.Now,
                    Remark = transferInv.Remark,
                    ShiftOrderStorageInDetailList = shiftOrderStorageInDetailList
                };

                interfaceLogDto.request_json = JsonConvert.SerializeObject(request);

                new Task(() =>
                {
                    var response = ApiClient.Post<ThirdPartyResponse>(PublicConst.ERPBaseUrl, "Inventory/ShiftOrder/ShiftOrderStorageIn", new CoreQuery(), request);
                    if (response.Success && response.ResponseResult != null)
                    {
                        rsp.IsSuccess = response.ResponseResult.IsSuccess;
                        rsp.ErrorMessage = response.ResponseResult.ErrorMessage;
                    }
                    else
                    {
                        rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                    }

                    //记录接口日志
                    interfaceLogDto.doc_sysId = transferInv.SysId;
                    interfaceLogDto.doc_order = transferInv.TransferInventoryOrder;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    interfaceLogDto.descr = "ERPBaseUrl|Inventory/ShiftOrder/ShiftOrderStorageIn|" + "1";

                    //发送MQ
                    interfaceLogDto.end_time = DateTime.Now;
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                }).Start();

                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                throw ex;
            }
        }

        #endregion

        #region 损益单
        /// <summary>
        /// ECC增加损益单
        /// </summary>
        /// <param name="adjust"></param>
        /// <returns></returns>
        public CommonResponse InsertAdjustment(adjustment adjust)
        {
            var rsp = new CommonResponse();

            InterfaceLogDto interfaceLogDto = new InterfaceLogDto()
            {
                interface_type = InterfaceType.Invoke.ToDescription(),
                interface_name = PublicConst.InsertAdjustment,
                user_id = adjust.AuditingBy.ToString(),
                user_name = adjust.AuditingName.ToString()
            };
            try
            {
                if (adjust != null && adjust.adjustmentdetails != null && adjust.adjustmentdetails.Any())
                {
                    #region 组织损益数据
                    var warehouse = _crudRepository.Get<warehouse>(adjust.WareHouseSysId);
                    if (warehouse == null)
                    {
                        throw new Exception("仓库信息不存在");
                    }

                    var adjustmentDto = new ThirdPartAdjustmentDto()
                    {
                        CreateDate = adjust.CreateDate,
                        CreateUserName = adjust.CreateUserName,
                        AdjustmentReason = "",
                        SourceNumber = adjust.AdjustmentOrder,
                        WarehouseID = int.Parse(warehouse.OtherId)

                    };
                    var sourceNumber = string.Empty;
                    var info = _crudRepository.GetQuery<qualitycontrol>(x => x.QCOrder == adjust.SourceOrder).FirstOrDefault();
                    if (info != null)
                    {
                        sourceNumber = info.ExternOrderId;
                    }

                    adjustmentDto.AdjustmentDetailList = new List<ThirdPartAdjustmentDetailDto>();
                    List<Guid?> skuSysIds = adjust.adjustmentdetails.Select(p => p.SkuSysId).ToList();
                    List<sku> skuList = _crudRepository.GetAllList<sku>(p => skuSysIds.Contains(p.SysId));

                    //根据批次获取渠道
                    List<string> lotList = adjust.adjustmentdetails.Select(p => p.Lot).ToList();
                    List<invlot> invlotList = _crudRepository.GetAllList<invlot>(p => lotList.Contains(p.Lot));

                    //获取损益明细中的所有图片
                    List<Guid> adjustmentdetailsGuid = adjust.adjustmentdetails.Select(p => p.SysId).ToList();
                    List<picture> picList = _crudRepository.GetAllList<picture>(p => p.TableKey == PublicConst.FileAdjustmentDetail && adjustmentdetailsGuid.Contains((Guid)p.TableSysId)).ToList();

                    foreach (var detail in adjust.adjustmentdetails)
                    {
                        var sku = skuList.FirstOrDefault(p => p.SysId == detail.SkuSysId);
                        var lot = invlotList.FirstOrDefault(p => p.Lot == detail.Lot);
                        var lotAttr01 = string.Empty;
                        if (lot != null)
                        { lotAttr01 = lot.LotAttr01; }

                        var adjustmentDetailDto = new ThirdPartAdjustmentDetailDto()
                        {
                            Qty = detail.Qty,
                            ProductCode = int.Parse(sku.OtherId),
                            Type = int.Parse(detail.AdjustlevelCode),
                            Channel = lotAttr01,            //渠道名称
                            SourceOrderId = sourceNumber,     //来源单号
                            AdjustOrderProductPicture = new List<AdjustOrderProductPicture>()//损益图片list
                        };

                        //组织损益明细图片
                        var childList = picList.Where(x => x.TableSysId == detail.SysId).ToList();
                        if (childList != null)
                        {
                            foreach (var item in childList)
                            {
                                adjustmentDetailDto.AdjustOrderProductPicture.Add(new AdjustOrderProductPicture
                                {
                                    PictureName = item.Name,
                                    PictureUrl = PublicConst.Adjustment + "/" + item.Url
                                });
                            }
                        }
                        adjustmentDto.AdjustmentDetailList.Add(adjustmentDetailDto);
                    }
                    #endregion

                    interfaceLogDto.request_json = JsonConvert.SerializeObject(adjustmentDto);

                    //new Task(() =>
                    //{
                    var response = ApiClient.Post<ThirdPartyResponse>(PublicConst.ERPBaseUrl, "Inventory/AdjustOrder/AddAdjustOrder", new CoreQuery(), adjustmentDto);
                    if (response.Success && response.ResponseResult != null)
                    {
                        rsp.IsSuccess = response.ResponseResult.IsSuccess;
                        rsp.ErrorMessage = response.ResponseResult.ErrorMessage;
                        if (!response.ResponseResult.IsSuccess)
                        {
                            throw new Exception("ECC接口返回：" + rsp.ErrorMessage);
                        }
                    }
                    else
                    {
                        rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                        throw new Exception("ECC接口返回：" + rsp.ErrorMessage);
                    }

                    //记录接口日志
                    interfaceLogDto.doc_sysId = adjust.SysId;
                    interfaceLogDto.doc_order = adjust.AdjustmentOrder;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    interfaceLogDto.descr = "ERPBaseUrl|Inventory/AdjustOrder/AddAdjustOrder|" + "1";

                    //发送MQ
                    interfaceLogDto.end_time = DateTime.Now;
                    RabbitWMS.SetRabbitMQSync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    //}).Start();
                }
                else
                {
                    throw new Exception("损益数据为空");
                }
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                throw ex;
            }
        }
        #endregion

        #region 查询地区信息
        /// <summary>
        /// 通过名称查询地区列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetRegionListByName(string name)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(string.Empty, "请输入");
            ThirdPartyRegionListQuery request = new ThirdPartyRegionListQuery { Name = name, iDisplayLength = 10 };
            var rsp = ApiClient.Post<List<ThirdPartyRegionListDto>>(PublicConst.ERPBaseUrl, "BaseData/Region/GetRegionListByQueryCondition", new CoreQuery(), request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                foreach (var item in rsp.ResponseResult)
                {
                    dic.Add(item.SysId.ToString(), item.Name);
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取详细地址
        /// </summary>
        /// <param name="regionSysId"></param>
        /// <returns></returns>
        public string GetRegionIntactBySysId(Guid regionSysId)
        {
            string regionIntact = string.Empty;
            CoreQuery query = new CoreQuery { ParmsObj = new { regionSysId } };
            var rsp = ApiClient.Get<ThirdPartyRegionIntactDto>(PublicConst.ERPBaseUrl, "BaseData/Region/GetRegionIntactByRegionSysId", query);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                List<string> regionIntactList = new List<string>();
                if (!rsp.ResponseResult.ProvinceName.IsNull())
                {
                    regionIntactList.Add(rsp.ResponseResult.ProvinceName);
                }
                if (!rsp.ResponseResult.CityName.IsNull())
                {
                    regionIntactList.Add(rsp.ResponseResult.CityName);
                }
                if (!rsp.ResponseResult.DistrictName.IsNull())
                {
                    regionIntactList.Add(rsp.ResponseResult.DistrictName);
                }
                if (!rsp.ResponseResult.TownName.IsNull())
                {
                    regionIntactList.Add(rsp.ResponseResult.TownName);
                }
                if (!rsp.ResponseResult.VillageName.IsNull())
                {
                    regionIntactList.Add(rsp.ResponseResult.VillageName);
                }
                regionIntact = string.Join(",", regionIntactList);
            }
            return regionIntact;
        }
        #endregion

        #region 库存转移
        /// <summary>
        /// 库存转移
        /// </summary>
        /// <param name="stockTransferDto"></param>
        /// <returns></returns>
        public CommonResponse InsertStockTransfer(ThirdPartyStockTransferDto stockTransferDto)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(stockTransferDto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.InsertStockTransfer
            };

            var stockTransferSysId = Guid.NewGuid();
            string stockTransferOrder = null;
            CommonResponse rsp = new CommonResponse { IsSuccess = true };
            try
            {
                if (string.IsNullOrEmpty(stockTransferDto.WarehouseId))
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "缺少关键参数 仓库ID WarehouseId,关闭失败";
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = false;
                    return rsp;
                }
                var warehouse = RedisGetWareHouseByOtherId(stockTransferDto.WarehouseId);
                if (warehouse == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "仓库不存在,Id" + stockTransferDto.WarehouseId;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = false;
                    return rsp;
                }
                _crudRepository.ChangeDB(warehouse.SysId);


                if (stockTransferDto != null && stockTransferDto.ThirdPartyStockTransferDetailDtoList != null && stockTransferDto.ThirdPartyStockTransferDetailDtoList.Count > 0)
                {
                    //warehouse warehouse = _crudRepository.GetQuery<warehouse>(p => p.OtherId == stockTransferDto.WarehouseId).FirstOrDefault();
                    //if (warehouse == null)
                    //{
                    //    rsp.IsSuccess = false;
                    //    rsp.ErrorMessage = "仓库不存在,Id" + stockTransferDto.WarehouseId;
                    //    return rsp;
                    //}

                    List<string> otherSkuIdList = stockTransferDto.ThirdPartyStockTransferDetailDtoList.Select(p => p.OtherSkuId).ToList();
                    List<sku> skuList = _crudRepository.GetQuery<sku>(p => otherSkuIdList.Contains(p.OtherId)).ToList();

                    var stockTransferDtoList = new List<StockTransferDto>();
                    foreach (var detail in stockTransferDto.ThirdPartyStockTransferDetailDtoList)
                    {
                        var sku = skuList.FirstOrDefault(x => x.OtherId == detail.OtherSkuId);
                        if (sku == null)
                        {
                            throw new Exception("未找到对应的商品,Id" + detail.OtherSkuId);
                        }

                        #region 组织转移数据
                        var stfDto = new StockTransferDto()
                        {
                            FromSkuSysId = sku.SysId,
                            ToSkuSysId = sku.SysId,
                            SkuCode = sku.SkuCode,
                            FromQty = detail.Qty,
                            ToQty = detail.Qty,
                            FromLotAttr01 = detail.FromChannel ?? string.Empty,
                            FromLotAttr02 = detail.FromBatchNumber ?? string.Empty,
                            ToLotAttr01 = detail.ToChannel,
                            ToLotAttr02 = detail.ToBatchNumber,
                            WarehouseSysId = warehouse.SysId,
                            CurrentUserId = stockTransferDto.CurrentUserId,
                            CurrentDisplayName = stockTransferDto.CurrentDisplayName
                        };
                        #endregion

                        _stockTransferAppService.StockTransferByLotAttr(stfDto);
                    }
                }
                else
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "库存转移数据为空";
                }

                //记录接口日志
                interfaceLogDto.doc_sysId = rsp.IsSuccess ? stockTransferSysId : new Guid?();
                interfaceLogDto.doc_order = rsp.IsSuccess ? stockTransferOrder : null;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;

                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
                throw new Exception(ex.Message);
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }
        #endregion

        #region 渠道库存初始化

        public List<ThirdPartyStockTransferDto> GetInitChannelInventoryData(InitInventoryFromChannelRequest request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);

            List<ThirdPartyStockTransferDto> transResponse = new List<ThirdPartyStockTransferDto>();


            var selectWarehouseIdList = request.InitList.Select(p => p.WarehouseId).Distinct().ToList();
            var selectSkuIdList = request.InitList.Select(p => p.SkuOtherID).Distinct().ToList();

            var warehouseList = _crudRepository.GetQuery<warehouse>(p => selectWarehouseIdList.Contains(p.OtherId)).ToList();
            var skuList = _crudRepository.GetQuery<sku>(p => selectSkuIdList.Contains(p.OtherId)).ToList();



            //            transResponse.WarehouseId = warehouseList.First().OtherId;

            // 完善请求数据源
            var initList = (from a in request.InitList
                            join b in warehouseList on a.WarehouseId equals b.OtherId
                            join s in skuList on a.SkuOtherID equals s.OtherId
                            select new InitInventoryFromChannelDto()
                            {
                                WarehouseId = a.WarehouseId,
                                SkuOtherID = a.SkuOtherID,
                                Channel = a.Channel,
                                Qty = a.Qty,
                                WarehouseSysId = b.SysId,
                                SkuSysId = s.SysId
                            }).ToList();

            if (initList == null || initList.Count == 0)
            {
                return transResponse;
            }

            var allLot = _crudRepository.GetAll<invlot>().ToList();

            // 统计当前WMS系统 渠道级别的 商品 库存
            var lotList = allLot.GroupBy(p => new { p.SkuSysId, p.LotAttr01, p.WareHouseSysId }).Select(p => new InitInventoryFromChannelDto()
            {
                SkuSysId = p.Key.SkuSysId,
                Channel = p.Key.LotAttr01,
                WarehouseSysId = p.Key.WareHouseSysId,
                Qty = p.Sum(q => q.Qty)
            });


            foreach (var item in initList)
            {
                var skuAllChannel = initList.Where(p => p.SkuSysId == item.SkuSysId && p.WarehouseSysId == item.WarehouseSysId).ToList();
                var wmsSkuAllChannel = lotList.Where(p => p.SkuSysId == item.SkuSysId && p.WarehouseSysId == item.WarehouseSysId).ToList();

                var wmsChannel = lotList.FirstOrDefault(p => p.SkuSysId == item.SkuSysId && p.WarehouseSysId == item.WarehouseSysId
                                    && p.Channel == item.Channel);

                int wmsChannelQty;

                if (wmsChannel == null)
                {
                    // wms 系统不存在该商品的渠道库存数据,需要从其他渠道来填充
                    wmsChannelQty = 0;
                }
                else
                {
                    // wms 系统存在该商品的渠道库存数据
                    wmsChannelQty = wmsChannel.Qty;
                }

                if (item.Qty == wmsChannelQty)
                {
                    //wms 渠道库存与 ecc 渠道库存一致，那么无需调整库存
                }
                else
                {
                    if (item.Qty > wmsChannelQty)
                    {
                        // ecc 渠道 大于 wms 渠道 库存，那么就需要将 wms 库存多的渠道 转移给 当前渠道
                        var diffQty = item.Qty - wmsChannelQty;

                        //取出该商品其他渠道中 wms 渠道库存 多余 ecc渠道库存的商品，用于转移填充
                        var compareQuery = (from wms in wmsSkuAllChannel
                                            join tempecc in skuAllChannel on new { wms.SkuSysId, wms.WarehouseSysId, wms.Channel }
                                                equals new { tempecc.SkuSysId, tempecc.WarehouseSysId, tempecc.Channel } into t0
                                            from ecc in t0.DefaultIfEmpty()
                                            where wms.Channel != item.Channel
                                                 && (ecc == null || ecc.Qty < wms.Qty)
                                            select new { ecc, wms }).ToList();

                        var compareDto = compareQuery.FirstOrDefault(p => (p.wms.Qty - (p.ecc == null ? 0 : p.ecc.Qty)) >= diffQty);
                        if (compareDto != null)
                        {
                            // 存在 wms 其他渠道库存可以足够直接一次性 转移给 当前渠道的
                            var wmsChannelLot = lotList.First(p => p.SkuSysId == compareDto.wms.SkuSysId && p.WarehouseSysId == compareDto.wms.WarehouseSysId
                                            && p.Channel == compareDto.wms.Channel);
                            wmsChannelLot.Qty = wmsChannelLot.Qty - diffQty;

                            transResponse.Add(new ThirdPartyStockTransferDto()
                            {
                                WarehouseId = item.WarehouseId,
                                ThirdPartyStockTransferDetailDtoList = new List<ThirdPartyStockTransferDetailDto>() { new ThirdPartyStockTransferDetailDto()
                                {
                                    OtherSkuId = item.SkuOtherID,
                                    Qty = diffQty,
                                    FromChannel = wmsChannelLot.Channel,
                                    ToChannel = item.Channel
                                }},
                                WarehouseSysId = item.WarehouseSysId,
                                CurrentUserId = request.CurrentUserId,
                                CurrentDisplayName = request.CurrentDisplayName
                            });
                            //transResponse.Add( new ThirdPartyStockTransferDetailDtoList.Add(new ThirdPartyStockTransferDetailDto()
                            //{
                            //    OtherSkuId = item.SkuOtherID,
                            //    Qty = diffQty,
                            //    FromChannel = wmsChannelLot.Channel,
                            //    ToChannel = item.Channel
                            //});
                        }
                        else
                        {
                            int tempCompareQty = 0;

                            // wms 其他渠道库存没有足够直接一次性 转移给 当前渠道的，需要组织好几个其他渠道一块转
                            foreach (var compareItem in compareQuery)
                            {
                                var eccQty = compareItem.ecc == null ? 0 : compareItem.ecc.Qty;
                                var wmsChannelLot = lotList.First(p => p.SkuSysId == compareItem.wms.SkuSysId && p.WarehouseSysId == compareItem.wms.WarehouseSysId
                                            && p.Channel == compareItem.wms.Channel);

                                if ((compareItem.wms.Qty - eccQty) <= (diffQty - tempCompareQty))
                                {
                                    if ((compareItem.wms.Qty - eccQty) > 0)
                                    {
                                        wmsChannelLot.Qty = wmsChannelLot.Qty - (compareItem.wms.Qty - eccQty);

                                        transResponse.Add(new ThirdPartyStockTransferDto()
                                        {
                                            WarehouseId = item.WarehouseId,
                                            ThirdPartyStockTransferDetailDtoList = new List<ThirdPartyStockTransferDetailDto>() { new ThirdPartyStockTransferDetailDto()
                                        {
                                            OtherSkuId = item.SkuOtherID,
                                            Qty = (compareItem.wms.Qty - eccQty),
                                            FromChannel = wmsChannelLot.Channel,
                                            ToChannel = item.Channel
                                        }},
                                            WarehouseSysId = item.WarehouseSysId,
                                            CurrentUserId = request.CurrentUserId,
                                            CurrentDisplayName = request.CurrentDisplayName
                                        });

                                        tempCompareQty = tempCompareQty + (compareItem.wms.Qty - eccQty);
                                    }
                                }
                                else if ((compareItem.wms.Qty - eccQty) > (diffQty - tempCompareQty))
                                {
                                    if ((diffQty - tempCompareQty) > 0)
                                    {
                                        wmsChannelLot.Qty = wmsChannelLot.Qty - (diffQty - tempCompareQty);

                                        transResponse.Add(new ThirdPartyStockTransferDto()
                                        {
                                            WarehouseId = item.WarehouseId,
                                            ThirdPartyStockTransferDetailDtoList = new List<ThirdPartyStockTransferDetailDto>() { new ThirdPartyStockTransferDetailDto()
                                        {
                                            OtherSkuId = item.SkuOtherID,
                                            Qty = (diffQty - tempCompareQty),
                                            FromChannel = wmsChannelLot.Channel,
                                            ToChannel = item.Channel
                                        }},
                                            WarehouseSysId = item.WarehouseSysId,
                                            CurrentUserId = request.CurrentUserId,
                                            CurrentDisplayName = request.CurrentDisplayName
                                        });

                                        tempCompareQty = tempCompareQty + (diffQty - tempCompareQty);
                                    }

                                }

                                if (tempCompareQty == diffQty)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else  // item.Qty < wmsChannel.Qty
                    {
                        // ecc 渠道 小于 wms 渠道 库存，暂时不做处理，等待 wms 渠道库存小于通渠道ECC的时候， 由那里的逻辑来扣减多余的wms渠道库存
                    }

                }

            }

            return transResponse;
        }

        #endregion

        #region 退货入库相关

        /// <summary>
        /// 退货入库 通知ecc 创建入库单，并返回入库单号
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="currentDisplayName"></param>
        /// <param name="ExternOrderId"></param>
        /// <param name="purchase"></param>
        /// <returns></returns>
        public CreatePurchaseOrderNumberResponse CreatePurchaseOrderNumber(ECCReturnOrder trdAPIRequest, int currentUserId, string currentDisplayName, Guid purchaseSysId)
        {
            try
            {
                CreatePurchaseOrderNumberResponse rsp = new CreatePurchaseOrderNumberResponse() { IsSuccess = true };

                //ECCReturnOrder trdAPIRequest = new ECCReturnOrder()
                //{
                //    OriginalOutStockId = int.Parse(ExternOrderId),
                //    WarhouseSysId = 0, //若为0默认为出库仓收货
                //    RequestUser = currentDisplayName
                //};

                var response = ApiClient.Post<CreatePurchaseOrderNumberResponse>(PublicConst.ERPBaseUrl, "Refund/WMSRefund/InsertInStockOrder", new CoreQuery(), trdAPIRequest);

                if (response.Success)
                {
                    if (response.ResponseResult != null && response.ResponseResult.ResultData != null)
                    {
                        rsp.ResultData = response.ResponseResult.ResultData;
                        rsp.IsSuccess = response.ResponseResult.IsSuccess;
                        rsp.ErrorMessage = response.ResponseResult.ErrorMessage;
                    }
                    else
                    {
                        rsp.IsSuccess = false;
                    }
                }
                else
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = response.ApiMessage.ErrorMessage;
                }

                //记录接口日志
                InterfaceLogDto interfaceLogDto = new InterfaceLogDto(trdAPIRequest, currentUserId.ToString(), currentDisplayName)
                {
                    doc_sysId = purchaseSysId,
                    doc_order = rsp.IsSuccess ? response.ResponseResult.ResultData.PurchaseOrder : "ECC单号生成失败",
                    interface_type = InterfaceType.Invoke.ToDescription(),
                    interface_name = PublicConst.CreatePurchaseOrderNumber,
                    response_json = JsonConvert.SerializeObject(response),
                    flag = rsp.IsSuccess,
                    descr = "ERPBaseUrl|Refund/WMSRefund/InsertInStockOrder|1"
                };
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);

                return rsp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region 质检

        /// <summary>
        /// 质检完成通知ECC
        /// </summary>
        /// <param name="finishQualityControlDto"></param>
        /// <param name="qualityControl"></param>
        /// <returns></returns>
        public CommonResponse FinishQualityControl(ThirdPartyFinishQualityControlDto finishQualityControlDto, qualitycontrol qualityControl)
        {
            //记录接口日志
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(finishQualityControlDto, finishQualityControlDto.CurrentUserId.ToString(), finishQualityControlDto.CurrentDisplayName)
            {
                doc_sysId = qualityControl.SysId,
                doc_order = qualityControl.QCOrder,
                interface_type = InterfaceType.Invoke.ToDescription(),
                interface_name = PublicConst.LogFinishQC,
                //response_json = JsonConvert.SerializeObject(rsp),
                //flag = rsp.ResponseResult.IsSuccess,
                descr = "ERPBaseUrl|Refund/WMSRefund/QCInStock|1"
            };
            ApiResponse<CommonResponse> rsp = new ApiResponse<CommonResponse> { ResponseResult = new CommonResponse { IsSuccess = false } };
            try
            {
                rsp = ApiClient.Post<CommonResponse>(PublicConst.ERPBaseUrl, "Refund/WMSRefund/QCInStock", new CoreQuery(), finishQualityControlDto);

                if (rsp != null && rsp.ResponseResult != null)
                {
                    return rsp.ResponseResult;
                }
                else
                {
                    return new CommonResponse(isSuccess: false, errorMessage: "调用ECC接口失败");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.ResponseResult.IsSuccess;
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }

        #endregion

        #region 调用TMS：向TMS推送箱号
        /// <summary>
        /// 向TMS推送箱号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public CommonResponse PreBullPackSendToTMS(ThirdPreBullPackDto dto)
        {
            try
            {
                var result = new CommonResponse();
                new Task(() =>
                {
                    var rsp = ApiClient.Post<TMSCommonResponse>(PublicConst.TMSBaseUrl, "/order/b2b/bullpack?System=WMS", new CoreQuery(), dto);
                    //记录接口日志
                    InterfaceLogDto interfaceLogDto = new InterfaceLogDto(dto, dto.CurrentUserId.ToString(), dto.CreateUserName)
                    {
                        doc_sysId = dto.OutboundSysId,
                        doc_order = dto.OutboundOrder,
                        interface_type = InterfaceType.Invoke.ToDescription(),
                        interface_name = PublicConst.TMSStorageCase,
                        response_json = JsonConvert.SerializeObject(rsp),
                        start_time = DateTime.Now,
                        descr = "TMSBaseUrl|/order/b2b/bullpack?System=WMS|1"
                    };
                    if (rsp.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        interfaceLogDto.flag = true;
                    }
                    else
                    {
                        interfaceLogDto.flag = false;
                    }
                    //发送MQ
                    interfaceLogDto.response_json = rsp.Content;
                    interfaceLogDto.end_time = DateTime.Now;
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    if (interfaceLogDto.flag)
                    {
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = "调用TMS接口失败";
                    }
                }).Start();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region TMS调用：装车顺序
        /// <summary>
        /// TMS调用：装车顺序
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CommonResponse InsertLoadingSequence(ThirdPartyLoadingSequenceDto request)
        {
            CommonResponse rsp = new CommonResponse { IsSuccess = true };
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto()
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.LoadingSequence,
                request_json = JsonConvert.SerializeObject(request),
                start_time = DateTime.Now,
                user_id = request.UserId.ToString(),
                user_name = request.UserName
            };
            try
            {
                if (request == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "请求数据不能为空";
                    return rsp;
                }
                if (string.IsNullOrEmpty(request.TMSOrder))
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "请求参数TMS运单号不能为空";
                    return rsp;
                }
                interfaceLogDto.doc_order = request.TMSOrder;
                if (request.LoadingSequenceOutboundList == null || request.LoadingSequenceOutboundList.Count <= 0)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "请求参数出库单列表不能为空";
                    return rsp;
                }

                var list = request.LoadingSequenceOutboundList.GroupBy(x => x.OtherId).Select(x => new { OtherId = x.Key }).ToList();

                foreach (var item in list)
                {
                    request.OtherId = item.OtherId;
                    var response = ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/ThirdParty/InsertTMSOrder", new CoreQuery(), request);
                    if (response == null || response.ResponseResult == null || !response.ResponseResult.IsSuccess)
                    {
                        throw new Exception("装车顺序更新失败。");
                    }
                }

            }
            catch (DbUpdateConcurrencyException ex)
            {
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
                throw new Exception("操作失败");
            }
            catch (Exception ex)
            {
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
                throw new Exception(ex.Message);
            }
            finally
            {
                if (rsp.IsSuccess)
                {
                    interfaceLogDto.flag = true;
                }
                else
                {
                    interfaceLogDto.flag = false;
                }
                interfaceLogDto.end_time = DateTime.Now;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
            return rsp;
        }


        public CommonResponse InsertTMSOrder(ThirdPartyLoadingSequenceDto request)
        {

            CommonResponse rsp = new CommonResponse { IsSuccess = true };
            warehouse warehouse = RedisGetWareHouseByOtherId(request.OtherId);
            if (warehouse == null)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = "仓库不存在,Id" + request.OtherId;
                return rsp;
            }
            _crudRepository.ChangeDB(warehouse.SysId);

            try
            {
                var externOrderIdList = request.LoadingSequenceOutboundList.Where(x => x.OtherId == request.OtherId).ToList();
                foreach (var item in externOrderIdList)
                {
                    if (item.Type == "1")
                    {
                        if (item.OrderType == (int)TMSOrderType.TransferOrder)
                        {
                            var tmodel = _crudRepository.GetAllList<transferinventory>(x => x.ExternOrderId == item.ExternOrderId).FirstOrDefault();
                            if (tmodel == null)
                            {
                                throw new Exception("根据请求单号：" + item.ExternOrderId + " 未找到移仓单信息");
                            }
                            var outModel = _crudRepository.GetQuery<outbound>(x => x.SysId == tmodel.TransferOutboundSysId).FirstOrDefault();
                            if (outModel == null)
                            {
                                throw new Exception("根据移仓单号：" + tmodel.TransferInventoryOrder + " 未找到出库单信息信息");
                            }
                            outModel.UpdateBy = request.UserId;
                            outModel.UpdateUserName = request.UserName;
                            outModel.UpdateDate = DateTime.Now;
                            outModel.TMSOrder = request.TMSOrder;
                            outModel.DepartureDate = request.DepartureDate;
                            outModel.SortNumber = item.SortNumber;
                            _crudRepository.Update<outbound>(outModel);
                        }
                        else
                        {
                            var model = _crudRepository.GetQuery<outbound>(x => x.ExternOrderId == item.ExternOrderId).FirstOrDefault();
                            if (model == null)
                            {
                                throw new Exception("根据请求单号：" + item.ExternOrderId + " 未找到出库单信息");
                            }
                            model.UpdateBy = request.UserId;
                            model.UpdateUserName = request.UserName;
                            model.UpdateDate = DateTime.Now;
                            model.TMSOrder = request.TMSOrder;
                            model.DepartureDate = request.DepartureDate;
                            model.SortNumber = item.SortNumber;
                            _crudRepository.Update<outbound>(model);
                        }

                    }
                }
                //var externOrderIdList = request.LoadingSequenceOutboundList.Where(x => x.OtherId == request.OtherId).Select(x => x.ExternOrderId).ToList();
                //var outboundList = _crudRepository.GetAllList<outbound>(x => externOrderIdList.Contains(x.ExternOrderId));
                ////if (outboundList == null || outboundList.Count <= 0)
                //{
                //    throw new Exception("根据请求数据未找到出库单信息");
                //}
                //foreach (var item in outboundList)
                //{
                //    LoadingSequenceOutboundList req = request.LoadingSequenceOutboundList.Where(x => x.ExternOrderId == item.ExternOrderId).FirstOrDefault();
                //    if (req.Type == "1")
                //    {
                //        if (req.OrderType == (int)TMSOrderType.B2BOrder)
                //        {
                //            item.UpdateBy = request.UserId;
                //            item.UpdateUserName = request.UserName;
                //            item.UpdateDate = DateTime.Now;
                //            item.TMSOrder = request.TMSOrder;
                //            item.DepartureDate = request.DepartureDate;
                //            item.SortNumber = req.SortNumber;
                //            _crudRepository.Update<outbound>(item);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
                throw new Exception(ex.Message);
            }

            return rsp;
        }

        #endregion

        #region 调用TMS：出库单状态变更通知TMS
        /// <summary>
        /// 调用TMS：出库单状态变更通知TMS
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public CommonResponse UpdateOutboundTypeToTMS(ThirdPartyUpdateOutboundTypeDto dto)
        {
            try
            {
                var result = new CommonResponse();
                new Task(() =>
                {
                    var rsp = ApiClient.Post<TMSCommonResponse>(PublicConst.TMSBaseUrl, "/order/UpdateOrderStatus?System=WMS", new CoreQuery(), dto);
                    //记录接口日志
                    InterfaceLogDto interfaceLogDto = new InterfaceLogDto(dto, dto.UserId.ToString(), dto.EditUserName)
                    {
                        doc_sysId = dto.OutboundSysId,
                        doc_order = dto.OutboundOrder,
                        interface_type = InterfaceType.Invoke.ToDescription(),
                        interface_name = PublicConst.TMSUpdateStatus,
                        response_json = JsonConvert.SerializeObject(rsp),
                        start_time = DateTime.Now,
                        descr = "TMSBaseUrl|/order/UpdateOrderStatus?System=WMS|1"
                    };
                    if (rsp.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        interfaceLogDto.flag = true;
                    }
                    else
                    {
                        interfaceLogDto.flag = false;
                    }
                    //发送MQ
                    interfaceLogDto.response_json = rsp.Content;
                    interfaceLogDto.end_time = DateTime.Now;
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                    if (interfaceLogDto.flag)
                    {
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = "调用TMS接口失败";
                    }
                }).Start();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region 调用TMS：给TMS推送出库单总箱数
        /// <summary>
        /// 给TMS推送出库单总箱数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public CommonResponse PushBoxCount(ThirdPartyPushBoxCountDto dto)
        {
            var result = new CommonResponse();
            try
            {
                var rsp = ApiClient.Post<TMSCommonResponse>(PublicConst.TMSBaseUrl, "/order/b2b/bullpack/count?System=WMS", new CoreQuery(), dto);
                //记录接口日志
                InterfaceLogDto interfaceLogDto = new InterfaceLogDto(dto, dto.CurrentUserId.ToString(), dto.EditUserName)
                {
                    doc_sysId = dto.OutboundSysId,
                    doc_order = dto.OutboundOrder,
                    interface_type = InterfaceType.Invoke.ToDescription(),
                    interface_name = PublicConst.PushBoxCount,
                    response_json = JsonConvert.SerializeObject(rsp),
                    start_time = DateTime.Now,
                    descr = "TMSBaseUrl|/order/b2b/bullpack/count?System=WMS|1"
                };
                if (rsp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result.IsSuccess = true;
                    interfaceLogDto.flag = true;
                }
                else
                {
                    interfaceLogDto.flag = false;
                    result.IsSuccess = false;
                    result.ErrorMessage = rsp.Content;
                }
                //发送MQ
                interfaceLogDto.response_json = rsp.Content;
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "{'code':null,'message':'TMS接口调用失败'}";
            }
            return result;
        }
        #endregion

        #region ECC调用插入商品外借信息 & 商品外借推送ECC 

        public CommonResponse InsertSkuBorrow(ThirdPartySkuBorrowAddDto dto)
        {
            CommonResponse rsp = new CommonResponse { IsSuccess = true };

            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(dto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.InsertSkuBorrow
            };

            try
            {
                warehouse warehouse = RedisGetWareHouseByOtherId(dto.WareHouseId.ToString());
                _crudRepository.ChangeDB(warehouse.SysId);
                if (dto != null)
                {
                    SkuBorrowDto skuBorrowDto = new SkuBorrowDto();
                    skuBorrowDto.SkuBorrowDetailList = new List<SkuBorrowDetailDto>();
                    skuBorrowDto.WareHouseSysId = warehouse.SysId;
                    skuBorrowDto.Status = (int)SkuBorrowStatus.New;
                    //skuBorrowDto.BorrowStartTime = dto.BorrowStartTime;
                    //skuBorrowDto.BorrowEndTime = dto.BorrowEndTime;
                    skuBorrowDto.Remark = dto.Remark;
                    skuBorrowDto.BorrowName = dto.BorrowName;
                    skuBorrowDto.LendingDepartment = dto.LendingDepartment;
                    skuBorrowDto.OtherId = dto.LendOrderId;
                    skuBorrowDto.Channel = dto.Channel;
                    skuBorrowDto.CreateBy = dto.CreateBy;
                    skuBorrowDto.CreateDate = DateTime.Now;
                    skuBorrowDto.CreateUserName = dto.CreateUserName;
                    skuBorrowDto.UpdateBy = dto.CreateBy;
                    skuBorrowDto.UpdateDate = DateTime.Now;
                    skuBorrowDto.UpdateUserName = dto.CreateUserName;
                    if (dto.SkuBorrowDetailList != null && dto.SkuBorrowDetailList.Count > 0)
                    {
                        foreach (ThirdPartySkuBorrowAddDetailDto detail in dto.SkuBorrowDetailList)
                        {
                            SkuBorrowDetailDto detailDto = new SkuBorrowDetailDto();
                            sku sku = _crudRepository.FirstOrDefault<sku>(s => s.OtherId == detail.SkuId);
                            if (sku == null)
                            {
                                rsp.IsSuccess = false;
                                rsp.ErrorMessage = "商品ID: " + detail.SkuId + " 没有找到对应的商品";

                                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                                interfaceLogDto.flag = rsp.IsSuccess;

                                return rsp;
                            }


                            detailDto.SkuSysId = sku.SysId;
                            detailDto.SkuCode = sku.SkuCode;
                            detailDto.SkuName = sku.SkuName;
                            detailDto.SkuDescr = sku.SkuDescr;
                            //detailDto.Loc = invlotloclpn.Loc;
                            //detailDto.Lot = invlotloclpn.Lot;
                            //detailDto.Lpn = invlotloclpn.Lpn;
                            detailDto.Qty = detail.Qty;
                            detailDto.BorrowStartTime = detail.BorrowStartTime;
                            detailDto.BorrowEndTime = detail.BorrowEndTime;
                            detailDto.Status = (int)SkuBorrowStatus.New;
                            detailDto.CreateBy = 9999;
                            detailDto.CreateDate = DateTime.Now;
                            detailDto.CreateUserName = "ECC";
                            detailDto.UpdateBy = 9999;
                            detailDto.UpdateDate = DateTime.Now;
                            detailDto.UpdateUserName = "ECC";
                            detailDto.Remark = detail.Remark;

                            skuBorrowDto.SkuBorrowDetailList.Add(detailDto);


                            ////取出所有货位信息按照数量倒序排列
                            //List<invlotloclpn> invlotloclpnList = _crudRepository.GetAllList<invlotloclpn>(i => i.WareHouseSysId == skuBorrowDto.WareHouseSysId && i.SkuSysId == sku.SysId).OrderByDescending(i => i.Qty).ToList();
                            //if (invlotloclpnList == null || invlotloclpnList.Count <= 0)
                            //{
                            //    rsp.IsSuccess = false;
                            //    rsp.ErrorMessage = "商品: " + sku.SkuName + " 没有找到对应的库位";
                            //    return rsp;
                            //}
                            ////总数量
                            //int totalQty = detail.Qty;

                            //foreach (var invlotloclpn in invlotloclpnList)
                            //{

                            //    var currentQty = CommonBussinessMethod.GetAvailableQty(invlotloclpn.Qty, invlotloclpn.AllocatedQty, invlotloclpn.PickedQty, invlotloclpn.FrozenQty); 

                            //    detailDto.SkuSysId = sku.SysId;
                            //    detailDto.SkuCode = sku.SkuCode;
                            //    detailDto.SkuName = sku.SkuName;
                            //    detailDto.SkuDescr = sku.SkuDescr;
                            //    detailDto.Loc = invlotloclpn.Loc;
                            //    detailDto.Lot = invlotloclpn.Lot;
                            //    detailDto.Lpn = invlotloclpn.Lpn;
                            //    detailDto.Qty = detail.Qty;
                            //    detailDto.BorrowStartTime = detail.BorrowStartTime;
                            //    detailDto.BorrowEndTime = detail.BorrowEndTime;
                            //    detailDto.Status = (int)SkuBorrowStatus.New;
                            //    detailDto.CreateBy = 9999;
                            //    detailDto.CreateDate = DateTime.Now;
                            //    detailDto.CreateUserName = "ECC";
                            //    detailDto.UpdateBy = 9999;
                            //    detailDto.UpdateDate = DateTime.Now;
                            //    detailDto.UpdateUserName = "ECC";
                            //    detailDto.Remark = detail.Remark;

                            //    skuBorrowDto.SkuBorrowDetailList.Add(detailDto);

                            //    if (currentQty < totalQty)
                            //    {
                            //        totalQty = totalQty - currentQty;
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        break;
                            //    } 
                            //}                             
                        }
                    }


                    skuBorrowDto.SysId = Guid.NewGuid();
                    skuBorrowDto.Status = (int)SkuBorrowStatus.New;

                    skuBorrowDto.CreateDate = DateTime.Now;
                    skuBorrowDto.UpdateDate = DateTime.Now;

                    skuBorrowDto.SkuBorrowDetailList.ForEach(p =>
                    {
                        ////原材料单位转换
                        //int transQty = 0;
                        //pack transPack = new pack();
                        //if (_packageAppService.GetSkuConversiontransQty(p.SkuSysId, p.DisplayQty, out transQty, ref transPack) == false)
                        //{
                        //    transQty = Convert.ToInt32(p.DisplayQty);
                        //}
                        //p.Qty = transQty; 
                        p.SysId = Guid.NewGuid();
                        p.SkuBorrowSysId = skuBorrowDto.SysId.Value;
                        p.CreateDate = DateTime.Now;
                        p.UpdateDate = DateTime.Now;
                    });

                    skuBorrowDto.BorrowOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberSkuBorrow);
                    var skuborrow = skuBorrowDto.JTransformTo<skuborrow>();
                    skuborrow.skuborrowdetails = skuBorrowDto.SkuBorrowDetailList.JTransformTo<skuborrowdetail>();
                    _crudRepository.Insert(skuborrow);
                }
                //记录接口日志
                //interfaceLogDto.doc_sysId = rsp.IsSuccess ? purchaseSysId : new Guid?();
                //interfaceLogDto.doc_order = rsp.IsSuccess ? purchaseOrder : null;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                return rsp;
            }
            catch (Exception ex)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                return rsp;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }

        public CommonResponse PushLendInfoToECC(ThirdPartySkuBorrowLendDto dto)
        {
            try
            {
                var result = new CommonResponse();
                var rsp = ApiClient.Post<ThirdPartyECCLendResponse>(PublicConst.ERPBaseUrl, "Inventory/LendOrder/WmsOutStock", new CoreQuery(), dto);

                if (rsp.Success && rsp.ResponseResult != null)
                {
                    result.IsSuccess = rsp.ResponseResult.Succeeded;
                    result.ErrorMessage = rsp.ResponseResult.Message;
                }
                else
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = rsp.ApiMessage.ErrorMessage;
                }

                //记录接口日志
                InterfaceLogDto interfaceLogDto = new InterfaceLogDto(dto)
                {
                    interface_type = InterfaceType.Invoke.ToDescription(),
                    interface_name = PublicConst.ECCSkuBorrow,
                    response_json = JsonConvert.SerializeObject(rsp),
                    descr = "ERPBaseUrl|Inventory/LendOrder/WmsOutStock"
                };
                //发送MQ
                interfaceLogDto.response_json = rsp.Content;
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public CommonResponse PushReturnInfoToECC(ThirdPartySkuBorrowReturnDto dto)
        {
            try
            {
                var result = new CommonResponse();
                var rsp = ApiClient.Post<ThirdPartyECCLendResponse>(PublicConst.ERPBaseUrl, "Inventory/LendOrder/WmsInStock", new CoreQuery(), dto);

                if (rsp.Success && rsp.ResponseResult != null)
                {
                    result.IsSuccess = rsp.ResponseResult.Succeeded;
                    result.ErrorMessage = rsp.ResponseResult.Message;
                }
                else
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = rsp.ApiMessage.ErrorMessage;
                }

                //记录接口日志
                InterfaceLogDto interfaceLogDto = new InterfaceLogDto(dto)
                {
                    interface_type = InterfaceType.Invoke.ToDescription(),
                    interface_name = PublicConst.ECCSkuBorrowReturn,
                    response_json = JsonConvert.SerializeObject(rsp),
                    descr = "ERPBaseUrl|Inventory/LendOrder/WmsInStock"
                };
                //发送MQ
                interfaceLogDto.response_json = rsp.Content;
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        /// <summary>
        /// 根据Redis 获取仓库信息（若Redis丢失则通过API从DB取得）
        /// </summary>
        /// <returns></returns>
        public warehouse RedisGetWareHouseByOtherId(string otherId)
        {
            var wareHouseList = RedisWMS.GetRedisList<List<warehouse>>(RedisSourceKey.RedisWareHouseList);
            if (wareHouseList == null || !wareHouseList.Any())
            {
                var query = new CoreQuery();
                query.ParmsObj = new { OtherId = otherId };
                var response = ApiClient.Post<WareHouseDto>(PublicConst.WmsApiUrl, "WareHouse/GetWarehouseByOtherId", query);
                if (response.Success)
                {
                    if (response.ResponseResult != null)
                    {
                        warehouse ware = new warehouse { SysId = response.ResponseResult.SysId, Name = response.ResponseResult.Name, OtherId = response.ResponseResult.OtherId };
                        wareHouseList = new List<warehouse>() { ware };
                        return ware;
                    }
                }
            }
            if (wareHouseList == null || !wareHouseList.Any())
            {
                throw new Exception("无法获取仓库信息！");
            }
            return wareHouseList.FirstOrDefault(x => x.OtherId == otherId);
        }

        #region  库存冻结，解冻 通知ECC

        /// <summary>
        /// 同步 冻结 调用ECC接口
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        public void PushLockOrderToECCSync(List<LockOrderInput> request, int userId, string userName)
        {
            if (request != null && request.Count > 0)
            {
                request = request.Where(p => p.Quantity > 0).ToList();
                if (request.Count == 0)
                    return;

                request = request.GroupBy(p => new { p.FreezeType, p.ProductCode, p.WarehouseId, p.CreateUserId, p.CreateUserName, p.ChannelTypeText })
                        .Select(x => new LockOrderInput()
                        {
                            FreezeType = x.Key.FreezeType,
                            ProductCode = x.Key.ProductCode,
                            Quantity = x.Sum(y => y.Quantity),
                            CreateUserId = x.Key.CreateUserId,
                            WarehouseId = x.Key.WarehouseId,
                            CreateUserName = x.Key.CreateUserName,
                            ChannelTypeText = x.Key.ChannelTypeText
                        }).ToList();
            }

            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(request, userId.ToString(), userName)
            {
                interface_type = InterfaceType.Invoke.ToDescription(),
                interface_name = PublicConst.ECCLockOrder,
                flag = true,
                descr = "ERPBaseUrl|Inventory/FreezenRecord/FreezeOrFreedInventory"
            };
            try
            {
                var result = new CommonResponse();
                var rsp = ApiClient.Post<LockOrderResponse>(PublicConst.ERPBaseUrl, "Inventory/FreezenRecord/FreezeOrFreedInventory", new CoreQuery(), request);

                //记录接口日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);

                if (rsp.Success)
                {
                    if (rsp.ResponseResult != null)
                    {
                        result.IsSuccess = rsp.ResponseResult.Succeeded;
                        result.ErrorMessage = rsp.ResponseResult.Message;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = "ECC接口无返回值";
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = rsp.ApiMessage.ErrorMessage;
                }

                if (!result.IsSuccess)
                {
                    interfaceLogDto.flag = false;
                    throw new Exception(result.ErrorMessage);
                }

            }
            catch (Exception ex)
            {
                interfaceLogDto.flag = false;
                interfaceLogDto.response_json = ex.Message;
                throw new Exception($"ECC接口返回:{ex.Message}");
            }
            finally
            {
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQSync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }
        #endregion

        #region B2C发起退货申请
        public CommonResponse InsertReturnPurchase(ThirdPartyReturnPurchaseDto purchaseDto)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(purchaseDto)
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.InsertReturnPurchase
            };
            try
            {
                CommonResponse rsp = new CommonResponse { IsSuccess = true };

                warehouse toWarehouse = RedisGetWareHouseByOtherId(purchaseDto.ToWarehouseId);
                if (toWarehouse == null)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "仓库不存在,Id" + purchaseDto.ToWarehouseId;
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }
                _crudRepository.ChangeDB(toWarehouse.SysId);

                var purchaseList = _crudRepository.GetQuery<purchase>(p => p.ExternalOrder == purchaseDto.ExternalOrder);
                if (purchaseList.Any())
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "入库单号" + purchaseDto.ExternalOrder + "重复无法保存";
                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                    interfaceLogDto.flag = rsp.IsSuccess;
                    return rsp;
                }


                var purchaseSysId = Guid.NewGuid();
                string purchaseOrder = null;
                if (purchaseDto.ReturnPurchaseDetailDtoList != null && purchaseDto.ReturnPurchaseDetailDtoList.Any())
                {
                    List<string> otherSkuIdList = purchaseDto.ReturnPurchaseDetailDtoList.Select(p => p.OtherSkuId).ToList();
                    List<sku> skuList = _crudRepository.GetAllList<sku>(p => otherSkuIdList.Contains(p.OtherId));
                    List<Guid> skuClassSysIdList = skuList.Select(p => p.SkuClassSysId).ToList();
                    List<Guid> packSysIdList = skuList.Select(p => p.PackSysId).ToList();
                    List<skuclass> skuClassList = _crudRepository.GetAllList<skuclass>(p => skuClassSysIdList.Contains(p.SysId));
                    List<pack> packList = _crudRepository.GetAllList<pack>(p => packSysIdList.Contains(p.SysId));
                    List<Guid?> fieldUom01List = packList.Select(p => p.FieldUom01).ToList();
                    List<uom> uomList = _crudRepository.GetAllList<uom>(p => fieldUom01List.Contains(p.SysId));

                    if (new InsertReturnPurchaseDetailCheck(rsp, purchaseDto, skuList, skuClassList, packList, uomList).Execute().IsSuccess)
                    {
                        #region 采购单明细  & 判断并更新原始出库明细数据
                        //var outbound = _crudRepository.GetQuery<outbound>(x => x.ExternOrderId == purchaseDto.ExternOutboundOrder).FirstOrDefault();
                        List<purchasedetail> purchaseDetails = new List<purchasedetail>();
                        foreach (var purchaseDetailDto in purchaseDto.ReturnPurchaseDetailDtoList)
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
                            purchasedetail purchaseDetail = new purchasedetail
                            {
                                SysId = Guid.NewGuid(),
                                PurchaseSysId = purchaseSysId,
                                SkuSysId = sku.SysId,
                                SkuClassSysId = skuClass == null ? new Guid?() : skuClass.SysId,
                                UOMSysId = uom.SysId,
                                UomCode = uom.UOMCode,
                                PackSysId = pack.SysId,
                                PackCode = pack.PackCode,
                                Qty = purchaseDetailDto.Qty,
                                GiftQty = purchaseDetailDto.GiftQty,
                                ReceivedQty = 0,
                                RejectedQty = 0,
                                PurchasePrice = purchaseDetailDto.PurchasePrice,
                                Remark = purchaseDetailDto.Remark,
                                OtherSkuId = purchaseDetailDto.OtherSkuId,
                                PackFactor = purchaseDetailDto.PackFactor,
                                UpdateBy = 99999,
                                UpdateDate = DateTime.Now
                            };
                            purchaseDetails.Add(purchaseDetail);
                            //if (outbound != null)
                            //{
                            //    var outbounddetail = _crudRepository.GetQuery<outbounddetail>(o => o.OutboundSysId == outbound.SysId && o.SkuSysId == sku.SysId).FirstOrDefault();
                            //    if (outbounddetail.ReturnQty + purchaseDetail.Qty > outbounddetail.ShippedQty)
                            //    {
                            //        //throw new Exception("商品: " + sku.SkuName + " 可退货数量不足"); 
                            //        rsp.IsSuccess = false;
                            //        rsp.ErrorMessage = "商品: " + sku.SkuName + " 可退货数量不足";
                            //        interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                            //        interfaceLogDto.flag = rsp.IsSuccess;
                            //        return rsp;
                            //    }
                            //    outbounddetail.ReturnQty += purchaseDetail.Qty;
                            //    _crudRepository.Update(outbounddetail);
                            //}
                        }
                        #endregion

                        #region 退货入库单
                        purchase purchase = null;
                        purchaseextend purchaseextend = null;

                        if (new InsertPurchaseCheck(rsp, toWarehouse).Execute().IsSuccess)

                        {
                            Guid vendorSysId;
                            vendor vendor =
                                _crudRepository.GetQuery<vendor>(p => p.VendorName == "WMS退货专用虚拟供应商")
                                    .FirstOrDefault();
                            if (vendor == null)
                            {
                                vendorSysId = Guid.NewGuid();
                                _crudRepository.Insert(new vendor
                                {
                                    SysId = vendorSysId,
                                    VendorName = "WMS退货专用虚拟供应商"
                                });
                            }
                            else
                            {
                                vendorSysId = vendor.SysId;
                            }

                            //如果接口中的单号字段不为空就用接口中的，否则自己生成
                            if (!string.IsNullOrEmpty(purchaseDto.PurchaseOrder))
                            {
                                purchaseOrder = purchaseDto.PurchaseOrder;
                            }
                            else
                            {
                                //purchaseOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberPurchase);
                                purchaseOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberPurchase);
                            }

                            //处理之前的出库单数据逻辑
                            Guid? outboundSysId = null;
                            string outboundOrder = string.Empty;
                            Guid? fromWarehouseSysId = null;
                            // outbound outboundUpdate = null;
                            if (!string.IsNullOrEmpty(purchaseDto.ExternOutboundOrder) && !string.IsNullOrEmpty(purchaseDto.WarehouseSysId))
                            {
                                warehouse warehouse = RedisGetWareHouseByOtherId(purchaseDto.WarehouseSysId);
                                if (warehouse == null)
                                {
                                    rsp.IsSuccess = false;
                                    rsp.ErrorMessage = "仓库不存在,Id" + purchaseDto.WarehouseSysId;
                                    interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                                    interfaceLogDto.flag = rsp.IsSuccess;
                                    return rsp;
                                }
                                fromWarehouseSysId = warehouse.SysId;
                                OutboundReturnDto outboundRequest = new OutboundReturnDto();
                                outboundRequest.ExternOrderId = purchaseDto.ExternOutboundOrder;
                                outboundRequest.WarehouseSysId = warehouse.SysId;
                                outboundRequest.CurrentUserId = 99999;
                                outboundRequest.CurrentDisplayName = purchaseDto.AuditingName;
                                outboundRequest.OutboundReturnDetailDtoList = purchaseDto.ReturnPurchaseDetailDtoList;
                                //异步更新出库单 相关类型 和 退货数量
                                var outboundResponse = ApiClient.Post<OutboundReturnDto>(PublicConst.WmsApiUrl, "/Outbound/AddOutboundReturnQtyByPurchase", new CoreQuery(), outboundRequest);

                                if (!outboundResponse.Success)
                                {
                                    throw new Exception(outboundResponse.ApiMessage.ErrorMessage);
                                }

                                if (outboundResponse.ResponseResult.OutboundSysId.HasValue)
                                {
                                    outboundSysId = outboundResponse.ResponseResult.OutboundSysId.Value;
                                    outboundOrder = outboundResponse.ResponseResult.OutboundOrder;
                                    //outboundUpdate = outbound;
                                }
                            }

                            purchase = new purchase
                            {
                                SysId = purchaseSysId,
                                PurchaseOrder = purchaseOrder,
                                DeliveryDate = purchaseDto.DeliveryDate,
                                ExternalOrder = purchaseDto.ExternalOrder,
                                VendorSysId = vendorSysId,
                                Descr = purchaseDto.Descr,
                                PurchaseDate = purchaseDto.PurchaseDate,
                                AuditingDate = purchaseDto.AuditingDate,
                                AuditingBy = purchaseDto.AuditingBy,
                                AuditingName = purchaseDto.AuditingName,
                                Status = (int)PurchaseStatus.New,
                                Type = purchaseDto.Type,
                                Source = purchaseDto.Source,
                                WarehouseSysId = toWarehouse.SysId,
                                FromWareHouseSysId = fromWarehouseSysId,
                                CreateBy = 99999,
                                CreateDate = DateTime.Now,
                                UpdateBy = 99999,
                                UpdateDate = DateTime.Now,
                                Channel = purchaseDto.Channel,
                                OutboundSysId = outboundSysId,
                                OutboundOrder = outboundOrder
                            };

                            //退货入库单扩展
                            purchaseextend = new purchaseextend
                            {
                                SysId = Guid.NewGuid(),
                                PurchaseSysId = purchaseSysId,
                                PlatformOrderId = purchaseDto.PlatformOrderId,
                                CustomerName = purchaseDto.CustomerName,
                                ReturnContact = purchaseDto.ReturnContact,
                                ShippingAddress = purchaseDto.ShippingAddress,
                                ExpressCompany = purchaseDto.ExpressCompany,
                                ExpressNumber = purchaseDto.ExpressNumber,
                                ReturnTime = purchaseDto.ReturnTime,
                                ReturnReason = purchaseDto.ReturnReason,
                                ServiceStationCode = purchaseDto.ServiceStationCode,
                                ServiceStationName = purchaseDto.ServiceStationName,
                                CreateBy = 99999,
                                CreateDate = DateTime.Now,
                                UpdateBy = 99999,
                                UpdateDate = DateTime.Now,
                                CreateUserName = "ECC",
                                UpdateUserName = "ECC"
                            };
                            _crudRepository.Insert(purchase);
                            _crudRepository.Insert(purchaseextend);
                            _crudRepository.BatchInsert(purchaseDetails);
                            //修改对应的出库单状态
                            //if (outboundUpdate != null)
                            //{
                            //    outboundUpdate.IsReturn = (int)OutboundReturnStatus.B2CReturn;
                            //    _crudRepository.Update(outboundUpdate);
                            //}
                        }
                        else
                        {
                            interfaceLogDto.doc_sysId = rsp.IsSuccess ? purchaseSysId : new Guid?();
                            interfaceLogDto.doc_order = rsp.IsSuccess ? purchaseOrder : null;
                            interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                            interfaceLogDto.flag = rsp.IsSuccess;
                            return rsp;
                        }
                        #endregion

                        //发送邮件通知
                        Guid sysCodeSysId = _crudRepository.FirstOrDefault<syscode>(p => p.SysCodeType == PublicConst.SysCodeTypeReceiptOutboundMail).SysId;
                        string mailTo = _crudRepository.FirstOrDefault<syscodedetail>(p => p.SysCodeSysId == sysCodeSysId && p.Code == "PurchaseMail").Descr;
                        EmailHelper.SendMailAsync(PublicConst.NewPurchaseSubject, string.Format(PublicConst.NewPurchaseMailBody, purchase.PurchaseOrder, purchase.DeliveryDate), mailTo);
                    }
                    else
                    {
                        interfaceLogDto.doc_sysId = rsp.IsSuccess ? purchaseSysId : new Guid?();
                        interfaceLogDto.doc_order = rsp.IsSuccess ? purchaseOrder : null;
                        interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                        interfaceLogDto.flag = rsp.IsSuccess;
                        return rsp;
                    }
                }

                //记录接口日志
                interfaceLogDto.doc_sysId = rsp.IsSuccess ? purchaseSysId : new Guid?();
                interfaceLogDto.doc_order = rsp.IsSuccess ? purchaseOrder : null;
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                interfaceLogDto.flag = rsp.IsSuccess;
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw ex;
            }
            finally
            {
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }

        public CommonResponse UpdateOutboundByReturnPurchase(ThirdPartyUpdateOutboundDto thirdPartyUpdateOutboundDto)
        {
            InterfaceLogDto interfaceLogDto = new InterfaceLogDto()
            {
                interface_type = InterfaceType.Invoked.ToDescription(),
                interface_name = PublicConst.UpdateOutboundByReturnPurchase
            };
            CommonResponse rsp = new CommonResponse { IsSuccess = true };
            Guid outboundSysId = new Guid();
            string outboundOrder = string.Empty;
            try
            {

                var warehouse = _wareHouseAppService.GetWarehouseByOtherId(thirdPartyUpdateOutboundDto.WarehouseSysId);
                if (warehouse != null)
                {
                    _crudRepository.ChangeDB(warehouse.SysId);
                    outbound outbound = _crudRepository.FirstOrDefault<outbound>(o => o.PlatformOrder == thirdPartyUpdateOutboundDto.PlatformOrder);
                    if (outbound != null)
                    {
                        if (outbound.IsReturn == (int)OutboundReturnStatus.B2CReturn)
                        {
                            rsp.IsSuccess = false;
                            rsp.ErrorMessage = "出库单已经做过退货操作!";
                        }
                        else
                        {
                            //新建状态出库单更新状态为关闭
                            if (outbound.Status == (int)OutboundStatus.New)
                            {
                                outbound.Status = (int)OutboundStatus.Close;
                            }
                            outbound.IsReturn = (int)OutboundReturnStatus.B2CReturn;
                            outboundSysId = outbound.SysId;
                            outboundOrder = outbound.OutboundOrder;
                            _crudRepository.Update(outbound);
                        }
                    }
                    else
                    {
                        rsp.IsSuccess = false;
                        rsp.ErrorMessage = "无法找到对应的出库单!";
                    }
                }
                else
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "无法找到对应的仓库!";
                }
                interfaceLogDto.response_json = JsonConvert.SerializeObject(rsp);
                return rsp;
            }
            catch (Exception ex)
            {
                //记录接口异常日志
                interfaceLogDto.response_json = JsonConvert.SerializeObject(ex);
                interfaceLogDto.flag = false;
                throw ex;
            }
            finally
            {
                //记录接口日志
                interfaceLogDto.doc_sysId = outboundSysId;
                interfaceLogDto.doc_order = outboundOrder;
                interfaceLogDto.flag = rsp.IsSuccess;
                //发送MQ
                interfaceLogDto.end_time = DateTime.Now;
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
            }
        }

        #endregion

    }
}