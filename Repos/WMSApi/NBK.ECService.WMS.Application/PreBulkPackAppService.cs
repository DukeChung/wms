using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;

namespace NBK.ECService.WMS.Application
{
    public class PreBulkPackAppService : WMSApplicationService, IPreBulkPackAppService
    {
        private IPreBulkPackRepository _crudRepository = null;
        private IBaseAppService _baseAppService = null;
        private IThirdPartyAppService _thirdPartyAppService = null;

        public PreBulkPackAppService(IPreBulkPackRepository crudRepository, IBaseAppService baseAppService, IThirdPartyAppService thirdPartyAppService)
        {
            this._crudRepository = crudRepository;
            this._baseAppService = baseAppService;
            this._thirdPartyAppService = thirdPartyAppService;
        }

        /// <summary>
        /// 增加散货预包装
        /// </summary>
        /// <returns></returns>
        public bool AddPreBulkPack(BatchPreBulkPackDto batchPreBulkPackDto)
        {
            _crudRepository.ChangeDB(batchPreBulkPackDto.WarehouseSysId);
            var result = false;
            try
            {
                if (batchPreBulkPackDto != null && batchPreBulkPackDto.PreBulkPackDtos != null && batchPreBulkPackDto.PreBulkPackDtos.Count > 0)
                {
                    var preBulkPackOrders = _baseAppService.GetNumber(PublicConst.GenNextNumberPreBulkPack, batchPreBulkPackDto.PreBulkPackDtos.Count);

                    var storageList = batchPreBulkPackDto.PreBulkPackDtos.Select(x => x.StorageCase).ToList();

                    var preBulkPackList = _crudRepository.GetQuery<prebulkpack>(x => storageList.Contains(x.StorageCase) && x.WareHouseSysId == batchPreBulkPackDto.WarehouseSysId).ToList();

                    foreach (var item in batchPreBulkPackDto.PreBulkPackDtos)
                    {
                        var toPreBulkPack = preBulkPackList.Where(x => x.StorageCase == item.StorageCase).FirstOrDefault();
                        if (toPreBulkPack != null)
                        {
                            throw new Exception(string.Format("箱号,{0}已存在，请重新生成", item.StorageCase));
                        }

                        var preBulkPack = new prebulkpack()
                        {
                            SysId = Guid.NewGuid(),
                            WareHouseSysId = batchPreBulkPackDto.WarehouseSysId,
                            //PreBulkPackOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberPreBulkPack),
                            PreBulkPackOrder = preBulkPackOrders[0],
                            StorageCase = item.StorageCase,
                            Status = (int)PreBulkPackStatus.New,
                            CreateBy = batchPreBulkPackDto.CurrentUserId,
                            CreateUserName = batchPreBulkPackDto.CurrentDisplayName,
                            CreateDate = DateTime.Now,
                            UpdateBy = batchPreBulkPackDto.CurrentUserId,
                            UpdateUserName = batchPreBulkPackDto.CurrentDisplayName,
                            UpdateDate = DateTime.Now,
                            OutboundSysId = batchPreBulkPackDto.OutboundSysId,
                            OutboundOrder = batchPreBulkPackDto.OutboundOrder
                        };
                        preBulkPackOrders.RemoveAt(0);

                        _crudRepository.Insert(preBulkPack);
                    }

                    #region 如果出库单ID不为空，则将箱号推送给TMS
                    //TODO 创建散货箱时候不给TMS推送箱号
                    //if (batchPreBulkPackDto.OutboundSysId != null && batchPreBulkPackDto.OutboundSysId != new Guid())
                    //{
                    //    var outbound = _crudRepository.Get<outbound>((Guid)batchPreBulkPackDto.OutboundSysId);
                    //    if (outbound != null && outbound.OutboundType == (int)OutboundType.B2B)
                    //    {
                    //        var thirdPreBullPackDto = new ThirdPreBullPackDto()
                    //        {
                    //            OrderId = outbound.ExternOrderId,
                    //            OutboundSysId = batchPreBulkPackDto.OutboundSysId,
                    //            OutboundOrder = batchPreBulkPackDto.OutboundOrder,
                    //            StorageCases = storageList,
                    //            CreateDate = DateTime.Now,
                    //            CreateUserName = batchPreBulkPackDto.CurrentDisplayName,
                    //            CurrentUserId = batchPreBulkPackDto.CurrentUserId
                    //        };
                    //        _thirdPartyAppService.PreBullPackSendToTMS(thirdPreBullPackDto);
                    //    }
                    //}
                    #endregion
                }
                else
                {
                    throw new Exception("参数为空");
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public void UpdatePreBulkPack(PreBulkPackDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var prebulkpack = _crudRepository.GetQuery<prebulkpack>(p => p.SysId == request.SysId).FirstOrDefault();
            if (prebulkpack == null || prebulkpack.prebulkpackdetails == null)
            {
                throw new Exception("单据不完整，请检查！");
            }

            if (prebulkpack.Status != (int)PreBulkPackStatus.PrePack && prebulkpack.Status != (int)PreBulkPackStatus.RFPicking)
            {
                throw new Exception($"只有{PreBulkPackStatus.PrePack.ToDescription()}状态的单据可以更新！");
            }

            foreach (var detail in prebulkpack.prebulkpackdetails)
            {
                var requestDetail = request.PreBulkPackDetailList.FirstOrDefault(p => p.SysId == detail.SysId);
                if (requestDetail != null)
                {
                    //更新数量
                    detail.Qty = requestDetail.Qty;
                }
            }

            _crudRepository.Update(prebulkpack);
        }

        public Pages<PreBulkPackDto> GetPreBulkPackByPage(PreBulkPackQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var response = _crudRepository.GetPreBulkPackByPage(request);

            return response;
        }

        public PreBulkPackDto GetPreBulkPackBySysId(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var response = _crudRepository.GetPreBulkPackBySysId(sysId);

            ////gavin:单位反转
            //if (response != null)
            //{
            //    decimal transQty = 0m;
            //    if (_packageAppService.GetSkuDeconversiontransQty(response.FromSkuSysId, response.CurrentQty, out transQty) == true)
            //    {
            //        response.DisplayCurrentQty = transQty;
            //    }
            //    else
            //    {
            //        response.DisplayCurrentQty = response.CurrentQty;
            //    }
            //}

            return response;
        }

        public void DeletePrebulkPackSkus(List<Guid> request, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            _crudRepository.Delete<prebulkpackdetail>(request);
        }

        public void DeletePrebulkPack(List<Guid> request, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            foreach (var sysId in request)
            {
                var prebulkpack = _crudRepository.GetQuery<prebulkpack>(p => p.SysId == sysId).FirstOrDefault();

                if (prebulkpack.Status != (int)PreBulkPackStatus.New)
                {
                    throw new Exception($"只有{PreBulkPackStatus.New.ToDescription()}状态的单据可以删除，请检查！");
                }
            }
            _crudRepository.Delete<prebulkpack>(request);
        }

        /// <summary>
        /// 散货导入
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool ImportPreBulkPack(PreBulkPackDto dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            try
            {
                var list = StringHelper.ToGuidList(dto.ImportSysIds);
                var preBuilPackLsit = _crudRepository.GetQuery<prebulkpack>(x => list.Contains(x.SysId)).ToList();
                if (preBuilPackLsit == null)
                {
                    throw new Exception("所选择的散货单不存在");
                }
                if (dto.PreBulkPackDetailList != null && dto.PreBulkPackDetailList.Count > 0)
                {
                    var skus = (from p in dto.PreBulkPackDetailList group p by p.OtherId into g select g.Key).ToList();
                    var skuList = _crudRepository.GetQuery<sku>(x => skus.Contains(x.OtherId)).ToList();
                    var packList = _crudRepository.GetAllList<pack>().ToList();
                    var uomList = _crudRepository.GetAllList<uom>().ToList();

                    foreach (var item in preBuilPackLsit)
                    {
                        item.UpdateBy = dto.CurrentUserId;
                        item.UpdateDate = DateTime.Now;
                        item.UpdateUserName = dto.CurrentDisplayName;
                        //item.Status = (int)PreBulkPackStatus.PrePack;

                        foreach (var detail in dto.PreBulkPackDetailList)
                        {
                            var detailSku = skuList.Find(x => x.OtherId == detail.OtherId && x.UPC == detail.UPC);
                            if (detailSku == null)
                            {
                                throw new Exception("商品外部Id:" + detail.OtherId + "或UPC:" + detail.UPC + "不存在");
                            }

                            pack detailPack = null;
                            if (detailSku != null)
                            {
                                detailPack = packList.Find(x => x.SysId == detailSku.PackSysId);
                            }
                            if (detailPack == null)
                            {
                                throw new Exception("商品:" + detailSku.SkuName + ",包装代码不存在");
                            }

                            uom detailUom = null;
                            if (detailPack != null)
                            {
                                detailUom = uomList.Find(x => x.SysId == Guid.Parse(detailPack.FieldUom01.ToString()));
                                if (detailUom == null)
                                {
                                    throw new Exception("包装:" + detailPack.PackCode + ",单位不存在");
                                }
                            }

                            var isexist = item.prebulkpackdetails.Where(x => x.SkuSysId == detailSku.SysId).ToList();
                            if (isexist != null && isexist.Count > 0)
                            {
                                throw new Exception("外部Id:" + detail.OtherId + "或UPC:" + detail.UPC + "的商品存在重复，请检查");
                            }

                            var detailInfo = new prebulkpackdetail()
                            {
                                SysId = Guid.NewGuid(),
                                PreBulkPackSysId = item.SysId,
                                Loc = detail.Loc,
                                Lot = detail.Lot,
                                SkuSysId = detailSku.SysId,
                                UOMSysId = detailUom.SysId,
                                PackSysId = detailPack.SysId,
                                PreQty = detail.PreQty,
                                UpdateDate = DateTime.Now,
                                CreateDate = DateTime.Now,
                                CreateBy = dto.CurrentUserId,
                                UpdateBy = dto.CurrentUserId,
                                CreateUserName = dto.CurrentDisplayName,
                                UpdateUserName = dto.CurrentDisplayName
                            };
                            item.prebulkpackdetails.Add(detailInfo);
                        }
                        _crudRepository.Update(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("散货导入失败" + ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 根据出库单ID获取散货封箱单号
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public List<string> GetPrebulkPackStorageCase(Guid outboundSysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var result = new List<string>();
            try
            {
                var list = _crudRepository.GetAllList<prebulkpack>(x => x.OutboundSysId == outboundSysId).ToList();
                if (list != null && list.Count > 0)
                {
                    result = list.Select(x => x.StorageCase).ToList();
                }
                else
                {
                    throw new Exception("该出库单没有相对应的散货封箱单");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取箱号失败" + ex.Message);
            }
            return result;
        }
    }
}
