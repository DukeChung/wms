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

namespace NBK.ECService.WMS.Application
{
    public class RFOutboundAppService : WMSApplicationService, IRFOutboundAppService
    {
        private ICrudRepository _crudRepository = null;
        private IRFOutboundRepository _rfOutboundRepository = null;
        private IWMSSqlRepository _WMSSqlRepository = null;
        private IPickDetailAppService _pickDetailAppService = null;
        private IRedisAppService _redisAppService = null;
        private IRFPickDetailRepository _rfPickDetailRepository = null;

        public RFOutboundAppService(ICrudRepository crudRepository, IRFOutboundRepository rfOutboundRepository, IWMSSqlRepository wmsSqlRepository, IPickDetailAppService pickDetailAppService, IRedisAppService redisAppService, IRFPickDetailRepository rfPickDetailRepository)
        {
            _crudRepository = crudRepository;
            _rfOutboundRepository = rfOutboundRepository;
            _WMSSqlRepository = wmsSqlRepository;
            _pickDetailAppService = pickDetailAppService;
            this._redisAppService = redisAppService;
            _rfPickDetailRepository = rfPickDetailRepository;
        }

        public RFWaitingReviewDto GetWaitingReviewList(RFOutboundQuery outboundQuery)
        {
            _rfOutboundRepository.ChangeDB(outboundQuery.WarehouseSysId);
            //RedisWMS.CleanRedis<List<RFOutboundDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, outboundQuery.OutboundOrder, outboundQuery.WarehouseSysId));
            return _rfOutboundRepository.GetWaitingReviewList(outboundQuery);
        }

        public RFCheckReviewResultDto CheckReviewDetailSku(RFCheckReviewDetailSkuDto checkReviewDetailSkuDto)
        {
            _rfOutboundRepository.ChangeDB(checkReviewDetailSkuDto.WarehouseSysId);
            RFCheckReviewResultDto rsp = new RFCheckReviewResultDto();
            var outboundDetails = _rfOutboundRepository.GetWaitingReviewList(new RFOutboundQuery { OutboundOrder = checkReviewDetailSkuDto.OutboundOrder, WarehouseSysId = checkReviewDetailSkuDto.WarehouseSysId }).WaitingReviewList;
            var scanningDetails = RedisWMS.GetRedisList<List<RFOutboundDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, checkReviewDetailSkuDto.OutboundOrder, checkReviewDetailSkuDto.WarehouseSysId)) ?? new List<RFOutboundDetailDto>();
            rsp.RFCommResult.IsSucess = false;
            if (checkReviewDetailSkuDto.SkuSysId.HasValue)
            {
                var sku = _crudRepository.GetQuery<sku>(p => p.SysId == checkReviewDetailSkuDto.SkuSysId.Value).FirstOrDefault();
                if (sku == null)
                {
                    rsp.RFCommResult.Message = "商品/散货箱不存在";
                }
                else
                {
                    var scanningDetail = scanningDetails.FirstOrDefault(p => p.SkuSysId == sku.SysId);
                    if (scanningDetail != null)
                    {
                        scanningDetail.DisplaySkuQty += checkReviewDetailSkuDto.Qty;
                    }
                    else
                    {
                        scanningDetail = new RFOutboundDetailDto { SkuSysId = sku.SysId, DisplaySkuQty = checkReviewDetailSkuDto.Qty };
                        scanningDetails.Add(scanningDetail);
                    }

                    var outboundDetail = outboundDetails.FirstOrDefault(p => p.SkuSysId == sku.SysId);
                    if (scanningDetail.DisplaySkuQty > outboundDetail.DisplaySkuQty)
                    {
                        rsp.RFCommResult.Message = string.Format("商品[{1}]复核数量超过出库单数量({0})，请重新复核！", outboundDetail.DisplaySkuQty, outboundDetail.UPC);
                    }
                    else
                    {
                        rsp.RFCommResult.IsSucess = true;
                        rsp.Skus.Add(new RFOutboundDetailDto
                        {
                            SkuSysId = sku.SysId,
                            DisplaySkuQty = scanningDetail.DisplaySkuQty
                        });
                    }
                }
            }
            else
            {
                var prebulkpack = _crudRepository.GetQuery<prebulkpack>(p => p.StorageCase == checkReviewDetailSkuDto.SkuUPC && p.WareHouseSysId == checkReviewDetailSkuDto.WarehouseSysId).FirstOrDefault();
                if (prebulkpack == null)
                {
                    rsp.RFCommResult.Message = "商品/散货箱不存在";
                }
                else
                {
                    if (checkReviewDetailSkuDto.Qty != 1)
                    {
                        rsp.RFCommResult.Message = "散件箱数量错误，请检查";
                        return rsp;
                    }
                    var prebulkpackdetails = _crudRepository.GetQuery<prebulkpackdetail>(p => p.PreBulkPackSysId == prebulkpack.SysId).ToList();
                    if (prebulkpackdetails.Count == 0)
                    {
                        rsp.RFCommResult.Message = "散货箱不包含任何商品";
                        return rsp;
                    }
                    else
                    {
                        rsp.RFCommResult.IsSucess = true;
                        foreach (var detail in prebulkpackdetails)
                        {
                            var scanningDetail = scanningDetails.FirstOrDefault(p => p.SkuSysId == detail.SkuSysId);
                            if (scanningDetail != null)
                            {
                                scanningDetail.DisplaySkuQty += (detail.Qty * checkReviewDetailSkuDto.Qty);
                            }
                            else
                            {
                                scanningDetail = new RFOutboundDetailDto { SkuSysId = detail.SkuSysId, DisplaySkuQty = detail.Qty * checkReviewDetailSkuDto.Qty };
                                scanningDetails.Add(scanningDetail);
                            }

                            var outboundDetail = outboundDetails.FirstOrDefault(p => p.SkuSysId == detail.SkuSysId);
                            if (outboundDetail == null)
                            {
                                var sku = _crudRepository.GetQuery<sku>(p => p.SysId == detail.SkuSysId).FirstOrDefault();
                                rsp.RFCommResult.IsSucess = false;
                                rsp.RFCommResult.Message = string.Format("出库单不包含该商品[{0}]，请重新复核！", sku.UPC);
                                return rsp;
                            }
                            if (scanningDetail.DisplaySkuQty > outboundDetail.DisplaySkuQty)
                            {
                                rsp.RFCommResult.IsSucess = false;
                                rsp.RFCommResult.Message = string.Format("商品[{1}]复核数量超过出库单数量({0})，请重新复核！", outboundDetail.DisplaySkuQty, outboundDetail.UPC);
                                return rsp;
                            }
                            else
                            {
                                rsp.Skus.Add(new RFOutboundDetailDto
                                {
                                    SkuSysId = detail.SkuSysId,
                                    DisplaySkuQty = scanningDetail.DisplaySkuQty
                                });
                            }
                        }
                    }
                }
            }
            if (rsp.RFCommResult.IsSucess)
            {
                RedisWMS.SetRedis(scanningDetails, string.Format(RedisSourceKey.RedisOutboundReviewScanning, checkReviewDetailSkuDto.OutboundOrder, checkReviewDetailSkuDto.WarehouseSysId));
            }
            return rsp;
        }

        public RFReviewResultDto GetReviewFinishResult(RFReviewFinishDto reviewFinishDto)
        {
            _rfOutboundRepository.ChangeDB(reviewFinishDto.WarehouseSysId);
            RFReviewResultDto rsp = new RFReviewResultDto();
            List<RFOutboundReviewInfo> reviewResult = new List<RFOutboundReviewInfo>();
            var outboundDetails = _rfOutboundRepository.GetWaitingReviewList(reviewFinishDto).WaitingReviewList;
            var scanningDetails = RedisWMS.GetRedisList<List<RFOutboundDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, reviewFinishDto.OutboundOrder, reviewFinishDto.WarehouseSysId)) ?? new List<RFOutboundDetailDto>();
            foreach (var outboundDetail in outboundDetails)
            {
                var scanningDetail = scanningDetails.FirstOrDefault(p => p.SkuSysId == outboundDetail.SkuSysId);
                if (scanningDetail == null)
                {
                    rsp.NoScanSkus.Add(GetOutboundReviewInfo(outboundDetail, scanningDetail));
                }
                else
                {
                    if (scanningDetail.DisplaySkuQty != outboundDetail.DisplaySkuQty)
                    {
                        rsp.QtyReducedSkus.Add(GetOutboundReviewInfo(outboundDetail, scanningDetail));
                    }
                    else
                    {
                        rsp.SameSkus.Add(GetOutboundReviewInfo(outboundDetail, scanningDetail));
                    }
                }
            }
            RedisWMS.CleanRedis<List<RFOutboundDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, reviewFinishDto.OutboundOrder, reviewFinishDto.WarehouseSysId));
            return rsp;
        }

        private RFOutboundReviewInfo GetOutboundReviewInfo(RFOutboundDetailDto outboundDetail, RFOutboundDetailDto scanningDetail)
        {
            return new RFOutboundReviewInfo
            {
                SkuUPC = outboundDetail.UPC,
                SkuName = outboundDetail.SkuName,
                OutboundQty = outboundDetail.SkuQty,
                DisplayQty = scanningDetail == null ? 0 : scanningDetail.DisplaySkuQty
            };
        }

        #region 整件预包装
        /// <summary>
        /// 获取预包装信息
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        public RFPrePackDto GetPrePackByQuery(RFPrePackQuery prePackQuery)
        {
            try
            {
                _crudRepository.ChangeDB(prePackQuery.WarehouseSysId);
                var prePack = _crudRepository.GetQuery<prepack>(x => x.StorageLoc == prePackQuery.StorageLoc && x.WareHouseSysId == prePackQuery.WarehouseSysId && x.Status == (int)PrePackStatus.New).FirstOrDefault();
                if (prePack != null)
                {
                    return prePack.JTransformTo<RFPrePackDto>();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 验证商品是否存在
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        public RFCommResult CheckPrePackDetailSku(RFPrePackQuery prePackQuery)
        {
            var result = new RFCommResult();
            try
            {
                _crudRepository.ChangeDB(prePackQuery.WarehouseSysId);
                prePackQuery.UPC = prePackQuery.UPC.Trim();
                var prePack = _crudRepository.GetQuery<prepack>(x => x.StorageLoc == prePackQuery.StorageLoc && x.WareHouseSysId == prePackQuery.WarehouseSysId).FirstOrDefault();
                if (prePack == null)
                {
                    throw new Exception("预包装单不存在");
                }

                sku sku = null;
                if (prePackQuery.SkuSysId.HasValue)
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.SysId == prePackQuery.SkuSysId.Value).FirstOrDefault();
                }
                else
                {
                    var skuList = _crudRepository.GetQuery<sku>(x => x.UPC == prePackQuery.UPC).ToList();

                    var query = from a in prePack.prepackdetails
                                join b in skuList on a.SkuSysId equals b.SysId
                                select b;

                    sku = query.FirstOrDefault();
                }

                if (sku == null)
                {
                    var preBulkPack = _crudRepository.GetQuery<prebulkpack>(x => x.StorageCase == prePackQuery.UPC && x.WareHouseSysId == prePackQuery.WarehouseSysId).FirstOrDefault();
                    if (preBulkPack == null)
                    {
                        throw new Exception("商品或箱号不存在");
                    }
                    if (preBulkPack.OutboundSysId != null && preBulkPack.OutboundOrder != null)
                    {
                        throw new Exception("该箱号已经绑定到出库单：" + preBulkPack.OutboundOrder + " 不能再进行绑定");
                    }
                }
                //else
                //{
                //    var prePackDetail = _crudRepository.GetQuery<prepackdetail>(x => x.PrePackSysId == prePack.SysId && x.SkuSysId == sku.SysId).FirstOrDefault();
                //    if (prePackDetail == null)
                //    {
                //        throw new Exception("预包装单内不存在扫描的商品");
                //    }
                //}

                result.IsSucess = true;
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 扫描预包装
        /// </summary>
        /// <param name="rfPrePackDto"></param>
        /// <returns></returns>
        public RFCommResult ScanPrePack(RFPrePackDto rfPrePackDto)
        {
            var result = new RFCommResult();
            try
            {
                _crudRepository.ChangeDB(rfPrePackDto.WarehouseSysId);
                var prePack = _crudRepository.GetQuery<prepack>(x => x.StorageLoc == rfPrePackDto.StorageLoc && x.WareHouseSysId == rfPrePackDto.WarehouseSysId && x.Status == (int)PrePackStatus.New).FirstOrDefault();
                if (prePack == null)
                {
                    throw new Exception("预包装单不存在");
                }

                var updatePrePackDetailList = new List<UpdatePrePackDetailDto>();

                sku sku = null;
                if (rfPrePackDto.SkuSysId.HasValue)
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.SysId == rfPrePackDto.SkuSysId.Value).FirstOrDefault();
                }
                else
                {
                    var skuList = _crudRepository.GetQuery<sku>(x => x.UPC == rfPrePackDto.UPC).ToList();

                    var query = from a in prePack.prepackdetails
                                join b in skuList on a.SkuSysId equals b.SysId
                                select b;

                    sku = query.FirstOrDefault();
                }

                if (sku == null)
                {
                    var preBulkPack = _crudRepository.GetQuery<prebulkpack>(x => x.StorageCase == rfPrePackDto.UPC && x.WareHouseSysId == rfPrePackDto.WarehouseSysId && x.Status != (int)PreBulkPackStatus.Finish && x.Status != (int)PreBulkPackStatus.Cancel).FirstOrDefault();
                    if (preBulkPack == null)
                    {
                        throw new Exception("商品或箱号不存在");
                    }
                    if (preBulkPack.OutboundSysId != null && preBulkPack.OutboundOrder != null)
                    {
                        throw new Exception("该箱号已经绑定到出库单：" + preBulkPack.OutboundOrder + " 不能再进行绑定");
                    }

                    var prePackDetails = _crudRepository.GetQuery<prepackdetail>(x => x.PrePackSysId == prePack.SysId).ToList();
                    if (prePackDetails == null || prePackDetails.Count == 0)
                    {
                        throw new Exception("预包装明细不存在");
                    }

                    var preBulkPackDetails = _crudRepository.GetQuery<prebulkpackdetail>(x => x.PreBulkPackSysId == preBulkPack.SysId && x.Qty != 0).ToList();
                    if (preBulkPackDetails != null && preBulkPackDetails.Count > 0)
                    {
                        foreach (var item in preBulkPackDetails)
                        {
                            var detail = prePackDetails.Find(x => x.SkuSysId == item.SkuSysId);
                            if (detail == null)
                            {
                                throw new Exception("预包装明细和箱明细商品不匹配");
                            }
                            if ((detail.PreQty ?? 0) - (detail.Qty ?? 0) < item.Qty)
                            {
                                throw new Exception("箱明细商品数量不能大于预包装明细商品数量");
                            }

                            var updatePrePackDetail = new UpdatePrePackDetailDto()
                            {
                                SysId = detail.SysId,
                                Qty = item.Qty,
                                CurrentUserId = rfPrePackDto.CurrentUserId,
                                CurrentDisplayName = rfPrePackDto.CurrentDisplayName
                            };
                            updatePrePackDetailList.Add(updatePrePackDetail);

                            var prepackrelation = new prepackrelation()
                            {
                                SysId = Guid.NewGuid(),
                                PrePackSysId = prePack.SysId,
                                PrePackDetailSysId = detail.SysId,
                                PreBulkPackSysId = preBulkPack.SysId,
                                PreBulkPackDetailSysId = item.SysId,
                                CreateBy = rfPrePackDto.CurrentUserId,
                                CreateUserName = rfPrePackDto.CurrentDisplayName,
                                CreateDate = DateTime.Now
                            };
                            _crudRepository.Insert(prepackrelation);
                        }
                    }
                    else
                    {
                        throw new Exception("箱明细不存在");
                    }

                    preBulkPack.Status = (int)PreBulkPackStatus.Finish;
                    preBulkPack.UpdateBy = rfPrePackDto.CurrentUserId;
                    preBulkPack.UpdateUserName = rfPrePackDto.CurrentDisplayName;
                    preBulkPack.UpdateDate = DateTime.Now;
                    preBulkPack.OutboundSysId = prePack.OutboundSysId;
                    preBulkPack.OutboundOrder = prePack.OutboundOrder;
                    _crudRepository.Update(preBulkPack);
                }
                else
                {
                    var prePackDetail = _crudRepository.GetQuery<prepackdetail>(x => x.PrePackSysId == prePack.SysId && x.SkuSysId == sku.SysId).FirstOrDefault();
                    if (prePackDetail == null)
                    {
                        throw new Exception("预包装单内不存在扫描的商品");
                    }
                    if ((rfPrePackDto.Qty + prePackDetail.Qty ?? 0) > (prePackDetail.PreQty ?? 0))
                    {
                        throw new Exception("扫描数量不能大于预包装单内数量");
                    }

                    var updatePrePackDetail = new UpdatePrePackDetailDto()
                    {
                        SysId = prePackDetail.SysId,
                        Qty = rfPrePackDto.Qty,
                        CurrentUserId = rfPrePackDto.CurrentUserId,
                        CurrentDisplayName = rfPrePackDto.CurrentDisplayName
                    };
                    updatePrePackDetailList.Add(updatePrePackDetail);
                }

                //执行更新预包装明细方法
                _WMSSqlRepository.UpdatePrePackDetail(updatePrePackDetailList);

                result.IsSucess = true;
                result.Message = "预包装成功";
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 查询预包装明细
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        public List<RFPrePackDetailList> GetPrePackDetailList(RFPrePackQuery prePackQuery)
        {
            _crudRepository.ChangeDB(prePackQuery.WarehouseSysId);
            return _rfOutboundRepository.GetPrePackDetailList(prePackQuery);
        }
        #endregion

        public RFPreBulkPackDto GetPreBulkPackDetailsByStorageCase(string storageCase, Guid warehouseSysId)
        {
            _rfOutboundRepository.ChangeDB(warehouseSysId);
            RFPreBulkPackDto preBulkPackDto = new RFPreBulkPackDto { RFCommResult = new RFCommResult { IsSucess = true } };
            var preBulkPack = _crudRepository.GetQuery<prebulkpack>(p => p.StorageCase == storageCase && p.WareHouseSysId == warehouseSysId).FirstOrDefault();
            if (preBulkPack == null)
            {
                preBulkPackDto.RFCommResult.IsSucess = false;
                preBulkPackDto.RFCommResult.Message = "未找到散货预包装单";
            }
            else if (preBulkPack.Status == (int)PreBulkPackStatus.Finish || preBulkPack.Status == (int)PreBulkPackStatus.Cancel)
            {
                preBulkPackDto.RFCommResult.IsSucess = false;
                preBulkPackDto.RFCommResult.Message = "散货预包装单已完成或作废";
            }
            else
            {
                preBulkPackDto.PreBulkPackDetails = _rfOutboundRepository.GetPreBulkPackDetailsByStorageCase(storageCase, warehouseSysId);
                preBulkPackDto.PreBulkPackNoScan = preBulkPackDto.PreBulkPackDetails.Where(p => p.Qty < p.PreQty).ToList();
            }
            return preBulkPackDto;
        }

        /// <summary>
        /// 根据容器获取商品list
        /// </summary>
        /// <param name="storageCase"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public List<RFPreBulkPackDetailDto> GetStorageCaseSkuList(string storageCase, Guid warehouseSysId)
        {
            _rfOutboundRepository.ChangeDB(warehouseSysId);
            var result = _rfOutboundRepository.GetPreBulkPackDetailsByStorageCase(storageCase, warehouseSysId);
            if (result != null)
            {
                result = result.GroupBy(x => new { x.SkuSysId, x.UPC, x.SkuName, x.UPC01, x.UPC02, x.UPC03, x.UPC04, x.UPC05, x.FieldValue01, x.FieldValue02, x.FieldValue03, x.FieldValue04, x.FieldValue05 }).Select(g => (new RFPreBulkPackDetailDto
                {
                    SkuSysId = g.Key.SkuSysId,
                    UPC = g.Key.UPC,
                    SkuName = g.Key.SkuName,
                    Qty = g.Sum(item => item.Qty),
                    UPC01 = g.Key.UPC01,
                    UPC02 = g.Key.UPC02,
                    UPC03 = g.Key.UPC03,
                    UPC04 = g.Key.UPC04,
                    UPC05 = g.Key.UPC05,
                    FieldValue01 = g.Key.FieldValue01,
                    FieldValue02 = g.Key.FieldValue02,
                    FieldValue03 = g.Key.FieldValue03,
                    FieldValue04 = g.Key.FieldValue04,
                    FieldValue05 = g.Key.FieldValue05

                })).ToList();
            }
            return result;
        }


        public RFCheckPreBulkPackDetailSkuDto CheckPreBulkPackDetailSku(string upc, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            RFCheckPreBulkPackDetailSkuDto rsp = new RFCheckPreBulkPackDetailSkuDto();
            rsp.Skus = _rfOutboundRepository.GetSkuByUPC(upc);
            return rsp;
        }

        /// <summary>
        /// 根据UPC获取商品和包装
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<SkuPackDto> GetSkuPackListByUPC(string upc, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var result = _rfOutboundRepository.GetSkuPackListByUPC(upc.Trim());
            if (result != null && result.Count > 0)
            {
                return result;
            }
            else
            {
                throw new Exception("商品信息不存在");
            }
        }

        public RFCommResult GeneratePreBulkPackDetail(RFPreBulkPackDetailDto preBulkPackDetailDto)
        {
            _rfOutboundRepository.ChangeDB(preBulkPackDetailDto.WarehouseSysId);
            RFCommResult rsp = new RFCommResult();
            sku sku = null;
            if (preBulkPackDetailDto.SkuSysId.HasValue)
            {
                sku = _crudRepository.GetQuery<sku>(p => p.SysId == preBulkPackDetailDto.SkuSysId.Value).FirstOrDefault();
            }
            else
            {
                sku = _crudRepository.GetQuery<sku>(p => p.UPC == preBulkPackDetailDto.UPC).FirstOrDefault(); ;
            }
            if (sku == null)
            {
                rsp.IsSucess = false;
                rsp.Message = "未找到SKU";
                return rsp;
            }
            var preBulkPack = _crudRepository.GetQuery<prebulkpack>(p => p.StorageCase == preBulkPackDetailDto.StorageCase && p.WareHouseSysId == preBulkPackDetailDto.WarehouseSysId).FirstOrDefault();
            if (preBulkPack == null)
            {
                rsp.IsSucess = false;
                rsp.Message = "未找到散货预包装单";
                return rsp;
            }
            if (preBulkPack.OutboundOrder != null && preBulkPack.OutboundSysId != null)
            {
                var outboundList = _crudRepository.GetAllList<outbounddetail>(x => x.OutboundSysId == preBulkPack.OutboundSysId).ToList();

                //根据商品获取出库单计划总数量
                var outQtySum = outboundList.Where(x => x.SkuSysId == sku.SysId).Sum(x => x.Qty);

                var detail = outboundList.Find(x => x.SkuSysId == sku.SysId);
                if (detail == null)
                {
                    rsp.IsSucess = false;
                    rsp.Message = "该商品不存在出库单: " + preBulkPack.OutboundOrder + " 明细中";
                    return rsp;
                }

                //获取与出库单关联的所有散货封箱单明细
                var preQty = _rfOutboundRepository.GetPreBulkOutboundQty((Guid)preBulkPack.OutboundSysId, sku.SysId);

                //已经预包装过的数量
                if (preQty + preBulkPackDetailDto.Qty > outQtySum)
                {
                    rsp.IsSucess = false;
                    rsp.Message = "扫描总数量不能超过出库单：" + preBulkPack.OutboundOrder + " 的出库数量";
                    return rsp;
                }
            }

            var pack = _crudRepository.GetQuery<pack>(p => p.SysId == sku.PackSysId).FirstOrDefault() ?? _crudRepository.GetQuery<pack>(p => p.PackCode == "缺省").FirstOrDefault();
            var uom = _crudRepository.GetQuery<uom>(p => p.SysId == pack.FieldUom01).FirstOrDefault() ?? _crudRepository.GetQuery<uom>(p => p.UOMCode == "缺省").FirstOrDefault();
            _rfOutboundRepository.InsertOrUpdatePreBulkPackDetail(preBulkPack, preBulkPackDetailDto, sku, uom);
            rsp.IsSucess = true;
            rsp.Message = string.Format("UPC: {0}, 数量: {1}, 箱号: {2}, 散货封箱成功!", sku.UPC, preBulkPackDetailDto.Qty, preBulkPackDetailDto.StorageCase);
            return rsp;
        }

        /// <summary>
        /// 查询出库明细
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public List<RFOutboundDetailDto> GetOutboundDetailList(string outboundOrder, Guid warehouseSysId)
        {
            _rfOutboundRepository.ChangeDB(warehouseSysId);
            return _rfOutboundRepository.GetOutboundDetailList(outboundOrder, warehouseSysId);
        }

        /// <summary>
        /// 根据容器号交接单号检查是否是同一出库单
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <param name="storageCase"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public RFCommResult CheckTransferOrderAndPreBulkPack(string transferOrder, string storageCase, Guid warehouseSysId)
        {
            _rfOutboundRepository.ChangeDB(warehouseSysId);
            RFCommResult rsp = new RFCommResult();

            var outTransfer = _crudRepository.GetQuery<outboundtransferorder>(x => x.TransferOrder == transferOrder && x.WareHouseSysId == warehouseSysId).FirstOrDefault();
            if (outTransfer == null)
            {
                rsp.IsSucess = false;
                rsp.Message = "根据交接单: " + transferOrder + "，未找到交接单";
                return rsp;
            }
            if (outTransfer.Status != (int)OutboundTransferOrderStatus.New && outTransfer.Status != (int)OutboundTransferOrderStatus.PrePack)
            {
                rsp.IsSucess = false;
                rsp.Message = "交接单: " + transferOrder + "状态不为新建或者进行中";
                return rsp;
            }

            var prebulkpack = _crudRepository.GetQuery<prebulkpack>(x => x.StorageCase == storageCase && x.WareHouseSysId == warehouseSysId).FirstOrDefault();
            if (prebulkpack == null)
            {
                rsp.IsSucess = false;
                rsp.Message = "根据箱号: " + storageCase + "，未找到对应容器";
                return rsp;
            }
            if (outTransfer.OutboundSysId != prebulkpack.OutboundSysId)
            {
                rsp.IsSucess = false;
                rsp.Message = "该交接单不是与容器对应出库单的交接单";
                return rsp;
            }
            rsp.IsSucess = true;
            rsp.Message = outTransfer.OutboundOrder;
            return rsp;
        }

        /// <summary>
        /// 整件复核检查交接单是否有效
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public RFCommResult CheckTransferOrder(string transferOrder, string outboundOrder, Guid warehouseSysId)
        {
            _rfOutboundRepository.ChangeDB(warehouseSysId);
            RFCommResult rsp = new RFCommResult { IsSucess = true };

            var outTransfer = _crudRepository.GetQuery<outboundtransferorder>(x => x.TransferOrder == transferOrder && x.WareHouseSysId == warehouseSysId).FirstOrDefault();
            if (outTransfer == null)
            {
                rsp.IsSucess = false;
                rsp.Message = "交接单不存在";
                return rsp;
            }
            if (outTransfer.Status != (int)OutboundTransferOrderStatus.New)
            {
                rsp.IsSucess = false;
                rsp.Message = "交接单状态必须为新建";
                return rsp;
            }
            if (outTransfer.OutboundOrder != outboundOrder)
            {
                rsp.IsSucess = false;
                rsp.Message = "交接单与出库单不匹配";
                return rsp;
            }
            return rsp;
        }

        /// <summary>
        /// 检查容器是否与出库单匹配
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="storageCase"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public RFCommResult CheckStorageCaseIsAvailable(string outboundOrder, string storageCase, Guid warehouseSysId)
        {
            _rfOutboundRepository.ChangeDB(warehouseSysId);
            RFCommResult rsp = new RFCommResult { IsSucess = true };

            var container = _crudRepository.GetQuery<prebulkpack>(p => p.StorageCase == storageCase && p.WareHouseSysId == warehouseSysId).FirstOrDefault();
            if (container == null)
            {
                rsp.IsSucess = false;
                rsp.Message = "未找到容器";
                return rsp;
            }

            if (container.OutboundOrder != outboundOrder)
            {
                rsp.IsSucess = false;
                rsp.Message = "容器与出库单不匹配";
                return rsp;
            }
            return rsp;
        }

        /// <summary>
        /// 插入交接单数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RFCommResult AddOutboundTransferOrder(RFOutboundTransferOrderQuery query)
        {
            _rfOutboundRepository.ChangeDB(query.WarehouseSysId);
            RFCommResult rsp = new RFCommResult();
            try
            {
                //获取交接单数据
                var outboundTransfer = _crudRepository.GetQuery<outboundtransferorder>(x => x.TransferOrder == query.TransferOrder && x.WareHouseSysId == query.WarehouseSysId).FirstOrDefault();
                if (outboundTransfer == null)
                {
                    throw new Exception("单号为：" + query.TransferOrder + "的交接箱不存在");
                }

                //获取容器数据
                var preBulkPack = _crudRepository.GetQuery<prebulkpack>(x => x.StorageCase == query.StorageCase && x.WareHouseSysId == query.WarehouseSysId && x.Status == (int)PreBulkPackStatus.RFPicking).FirstOrDefault();
                if (preBulkPack == null)
                {
                    throw new Exception("箱号为：" + query.StorageCase + "的容器不存在");
                }

                if (outboundTransfer.Status == (int)OutboundTransferOrderStatus.New)
                {
                    outboundTransfer.Status = (int)OutboundTransferOrderStatus.PrePack;
                    //outboundTransfer.PreBulkPackOrder = preBulkPack.PreBulkPackOrder;
                    //outboundTransfer.PreBulkPackSysId = preBulkPack.SysId;

                    //更新相关信息
                    outboundTransfer.UpdateBy = query.CurrentUserId;
                    outboundTransfer.UpdateUserName = query.CurrentDisplayName;
                    outboundTransfer.UpdateDate = DateTime.Now;

                    _crudRepository.Update<outboundtransferorder>(outboundTransfer);
                }

                if (preBulkPack.prebulkpackdetails == null)
                {
                    throw new Exception("箱号为：" + query.StorageCase + "的容器不存在任何商品");
                }
                var skuList = preBulkPack.prebulkpackdetails.Where(x => x.SkuSysId == query.SkuSysId).OrderByDescending(x => x.Qty).ToList();
                if (skuList == null || skuList.Count == 0)
                {
                    throw new Exception("UPC为：" + query.UPC + "的商品不存在容器中");
                }
                var hasQty = skuList.Sum(x => x.Qty);
                if (query.Qty > hasQty)
                {
                    throw new Exception("复核数量不能大于现有容器中数量");
                }
                var outboundTransferDetail = _crudRepository.GetAllList<outboundtransferorderdetail>(x => x.OutboundTransferOrderSysId == outboundTransfer.SysId && x.SkuSysId == query.SkuSysId).ToList();

                //与出库单比较商品是否存在出库单中
                var outbound = _crudRepository.GetAllList<outbounddetail>(x => x.OutboundSysId == outboundTransfer.OutboundSysId).ToList();
                if (outbound == null || outbound.Count == 0)
                {
                    throw new Exception("需要复核的出库单获取失败");
                }
                var outboundSkuList = outbound.Where(x => x.SkuSysId == query.SkuSysId).ToList();
                if (outboundSkuList == null || outboundSkuList.Count == 0)
                {
                    throw new Exception("UPC为：" + query.UPC + "的商品不存在出库单：" + outboundTransfer.OutboundOrder + "中");
                }

                if ((outboundTransferDetail.Sum(x => x.Qty) + query.Qty) > outboundSkuList.Sum(x => x.Qty))
                {
                    throw new Exception("复核数量不能超过出库单：" + outboundTransfer.OutboundOrder + "中的出库数量");
                }
                var redisQty = query.Qty;
                var model = new outboundtransferorderdetail();
                foreach (var item in skuList)
                {
                    if (query.Qty == 0)
                    {
                        break;
                    }
                    model.SysId = Guid.NewGuid();
                    model.OutboundTransferOrderSysId = outboundTransfer.SysId;
                    model.PackSysId = item.PackSysId;
                    model.UOMSysId = item.PackSysId;
                    model.SkuSysId = item.SkuSysId;
                    model.CreateBy = model.UpdateBy = query.CurrentUserId;
                    model.CreateUserName = model.UpdateUserName = query.CurrentDisplayName;
                    model.CreateDate = model.UpdateDate = DateTime.Now;

                    if (query.Qty >= item.Qty)
                    {
                        query.Qty = query.Qty - item.Qty;
                        model.Qty = item.Qty;

                        //复核数量大于容器的本条拣货数量删除容器中的记录
                        preBulkPack.prebulkpackdetails.Remove(item);
                        _crudRepository.Delete<prebulkpackdetail>(item);

                    }
                    else
                    {
                        model.Qty = (int)query.Qty;
                        query.Qty = 0;
                        item.Qty = item.Qty - (int)model.Qty;
                        preBulkPack.prebulkpackdetails.Remove(item);
                        preBulkPack.prebulkpackdetails.Add(item);
                        //复核数量小于容器的本条拣货数量更新容器中的记录
                        _crudRepository.Update<prebulkpackdetail>(item);
                    }

                    var info = outboundTransferDetail.Where(x => x.SkuSysId == model.SkuSysId).FirstOrDefault();
                    if (info == null)
                    {
                        _crudRepository.Insert<outboundtransferorderdetail>(model);
                    }
                    else
                    {
                        info.Qty = info.Qty + model.Qty;
                        _crudRepository.Update<outboundtransferorderdetail>(info);
                    }
                }

                //如果容器的明细数据已经清空，将容器置为空闲状态
                if (preBulkPack.prebulkpackdetails == null || preBulkPack.prebulkpackdetails.Count == 0)
                {
                    preBulkPack.OutboundOrder = null;
                    preBulkPack.OutboundSysId = null;
                    preBulkPack.Status = (int)PreBulkPackStatus.New;
                    preBulkPack.UpdateBy = query.CurrentUserId;
                    preBulkPack.UpdateDate = DateTime.Now;
                    preBulkPack.UpdateUserName = query.CurrentDisplayName;
                    _crudRepository.Update<prebulkpack>(preBulkPack);
                }


                #region 写入出库单复核数据
                var scanningDetails = RedisWMS.GetRedisList<List<RFOutboundDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, outboundTransfer.OutboundOrder, query.WarehouseSysId)) ?? new List<RFOutboundDetailDto>();

                var scanningDetail = scanningDetails.FirstOrDefault(p => p.SkuSysId == query.SkuSysId);
                if (scanningDetail != null)
                {
                    scanningDetail.DisplaySkuQty += redisQty;
                }
                else
                {
                    scanningDetail = new RFOutboundDetailDto { SkuSysId = query.SkuSysId, DisplaySkuQty = redisQty };
                    scanningDetails.Add(scanningDetail);
                }
                RedisWMS.SetRedis(scanningDetails, string.Format(RedisSourceKey.RedisOutboundReviewScanning, outboundTransfer.OutboundOrder, query.WarehouseSysId));

                #endregion 写入出库单复核数据

                rsp.IsSucess = true;
            }
            catch (Exception ex)
            {
                rsp.IsSucess = false;
                throw new Exception("复核失败：" + ex.Message);
            }
            return rsp;
        }


        /// <summary>
        /// 插入交接单数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RFCommResult SealedOutboundTransferBox(RFOutboundTransferOrderQuery query)
        {
            _rfOutboundRepository.ChangeDB(query.WarehouseSysId);
            RFCommResult rsp = new RFCommResult();
            try
            {
                var outboundtransfer = _crudRepository.GetQuery<outboundtransferorder>(x => x.TransferOrder == query.TransferOrder).FirstOrDefault();
                if (outboundtransfer == null)
                {
                    throw new Exception("获取交接箱失败");
                }
                if (outboundtransfer.Status != (int)OutboundTransferOrderStatus.PrePack)
                {
                    throw new Exception("状态不等于进行中的交接箱不能封箱");
                }
                var outboundTransferDetail = _crudRepository.GetAllList<outboundtransferorderdetail>(x => x.OutboundTransferOrderSysId == outboundtransfer.SysId).ToList();
                if (outboundTransferDetail == null || outboundTransferDetail.Count == 0)
                {
                    throw new Exception("不存在明细记录的交接箱不能封箱");
                }
                outboundtransfer.Status = (int)OutboundTransferOrderStatus.Finish;
                //复核人相关信息
                outboundtransfer.ReviewBy = query.CurrentUserId;
                outboundtransfer.ReviewDate = DateTime.Now;
                outboundtransfer.ReviewUserName = query.CurrentDisplayName;

                _crudRepository.Update<outboundtransferorder>(outboundtransfer);
                rsp.IsSucess = true;
                rsp.Message = "封箱完成";
            }
            catch (Exception ex)
            {
                rsp.IsSucess = false;
                throw new Exception("封箱失败：" + ex.Message);
            }
            return rsp;
        }

        /// <summary>
        /// 获取交接箱复核差异
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<RFOutboundTransferOrderReviewDiffDto> GetTransferOrderReviewDiffList(RFOutboundQuery query)
        {
            _rfOutboundRepository.ChangeDB(query.WarehouseSysId);
            List<RFOutboundTransferOrderReviewDiffDto> reviewResult = new List<RFOutboundTransferOrderReviewDiffDto>();

            //获取出库单明细数控
            var outboundDetails = _rfOutboundRepository.GetWaitingReviewList(query).WaitingReviewList;

            //获取缓存中的数据
            var scanningDetails = RedisWMS.GetRedisList<List<RFOutboundDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, query.OutboundOrder, query.WarehouseSysId)) ?? new List<RFOutboundDetailDto>();

            foreach (var outboundDetail in outboundDetails)
            {
                var scanningDetail = scanningDetails.FirstOrDefault(p => p.SkuSysId == outboundDetail.SkuSysId);
                if (scanningDetail == null)
                {
                    reviewResult.Add(new RFOutboundTransferOrderReviewDiffDto()
                    {
                        SkuSysId = outboundDetail.SkuSysId,
                        UPC = outboundDetail.UPC,
                        SkuName = outboundDetail.SkuName,
                        OutboundQty = outboundDetail.SkuQty,
                        ReviewQty = 0
                    });
                }
                else
                {
                    if (scanningDetail.DisplaySkuQty != outboundDetail.DisplaySkuQty)
                    {
                        reviewResult.Add(new RFOutboundTransferOrderReviewDiffDto()
                        {
                            SkuSysId = outboundDetail.SkuSysId,
                            UPC = outboundDetail.UPC,
                            SkuName = outboundDetail.SkuName,
                            OutboundQty = outboundDetail.SkuQty,
                            ReviewQty = scanningDetail.DisplaySkuQty
                        });
                    }
                }

            }
            return reviewResult;
        }

        /// <summary>
        /// 获取待复核的出库单
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        public Pages<RFOutboundReviewListDto> GetWaitingOutboundReviewListByPaging(RFOutboundQuery outboundQuery)
        {
            _crudRepository.ChangeDB(outboundQuery.WarehouseSysId);
            return _rfOutboundRepository.GetWaitingOutboundReviewListByPaging(outboundQuery);
        }

        /// <summary>
        /// 判断扫描的单号是否待复核
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        public RFCommResult CheckOutboundReviewOrder(RFOutboundQuery outboundQuery)
        {
            _crudRepository.ChangeDB(outboundQuery.WarehouseSysId);
            var rsp = new RFCommResult { IsSucess = true };
            var outbound = _crudRepository.GetQuery<outbound>(p => p.OutboundOrder == outboundQuery.OutboundOrder
                && p.WareHouseSysId == outboundQuery.WarehouseSysId
                && p.Status == (int)OutboundStatus.Allocation).FirstOrDefault();
            if (outbound == null)
            {
                var pickDetail = _crudRepository.GetQuery<pickdetail>(p => p.PickDetailOrder == outboundQuery.OutboundOrder
                    && p.WareHouseSysId == outboundQuery.WarehouseSysId
                    && p.Status == (int)PickDetailStatus.New
                    && p.Qty != 0).FirstOrDefault();
                if (pickDetail == null)
                {
                    rsp.IsSucess = false;
                    rsp.Message = "待复核单据中不存在此单据号";
                }
                else
                {
                    var pickDetailOutbound = _crudRepository.GetQuery<outbound>(p => p.SysId == pickDetail.OutboundSysId).FirstOrDefault();
                    rsp.Message = pickDetailOutbound == null ? string.Empty : pickDetailOutbound.OutboundOrder;
                }
            }
            else
            {
                rsp.Message = outbound.OutboundOrder;
            }
            return rsp;
        }

        /// <summary>
        /// 获取散货待复核明细
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        public List<RFOutboundReviewDetailDto> GetWaitingSingleReviewDetails(RFOutboundQuery outboundQuery)
        {
            _crudRepository.ChangeDB(outboundQuery.WarehouseSysId);
            var rsp = _rfOutboundRepository.GetWaitingSingleReviewDetails(outboundQuery);
            var transferOrders = _crudRepository.GetQuery<outboundtransferorder>(p => p.OutboundOrder == outboundQuery.OutboundOrder && p.WareHouseSysId == outboundQuery.WarehouseSysId).ToList();
            if (transferOrders != null && transferOrders.Any())
            {
                var transferOrderSysIds = transferOrders.Select(p => p.SysId).ToList();
                var transferOrderDetails = _crudRepository.GetQuery<outboundtransferorderdetail>(p => transferOrderSysIds.Contains(p.OutboundTransferOrderSysId)).ToList();
                //var storageCaseList = rsp.Select(p => p.StorageCase).ToList();
                //var containers = _crudRepository.GetQuery<prebulkpack>(p => storageCaseList.Contains(p.StorageCase) && p.WareHouseSysId == outboundQuery.WarehouseSysId).ToList();
                foreach (var item in rsp)
                {
                    //var details = transferOrderDetails.Where(p => p.SkuSysId == item.SkuSysId && p.OutboundTransferOrderSysId == containerOrder.SysId).ToList();
                    var details = transferOrderDetails.Where(p => p.SkuSysId == item.SkuSysId).ToList();
                    if (details != null && details.Any())
                    {
                        item.ReviewQty += details.Sum(p => p.Qty);
                    }
                    //var container = containers.FirstOrDefault(p => p.StorageCase == item.StorageCase);
                    //if (container != null)
                    //{
                    //    var containerOrders = transferOrders.Where(p => p.PreBulkPackSysId == container.SysId).ToList();
                    //    if (containerOrders.Any())
                    //    {
                    //        foreach (var containerOrder in containerOrders)
                    //        {
                    //            var details = transferOrderDetails.Where(p => p.SkuSysId == item.SkuSysId && p.OutboundTransferOrderSysId == containerOrder.SysId).ToList();
                    //            if (details != null && details.Any())
                    //            {
                    //                item.ReviewQty += details.Sum(p => p.Qty);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            return rsp;
        }

        /// <summary>
        /// 散件复核扫描
        /// </summary>
        /// <param name="scanningDto"></param>
        /// <returns></returns>
        public RFCommResult SingleReviewScanning(RFSingleReviewScanningDto scanningDto)
        {
            RFCommResult rsp = new RFCommResult { IsSucess = true };
            try
            {
                _rfOutboundRepository.ChangeDB(scanningDto.WarehouseSysId);

                var outboundTransfer = _crudRepository.GetQuery<outboundtransferorder>(x => x.TransferOrder == scanningDto.TransferOrder && x.WareHouseSysId == scanningDto.WarehouseSysId).FirstOrDefault();
                if (outboundTransfer == null)
                {
                    throw new Exception("交接箱不存在");
                }

                var allPreBulkPack = _crudRepository.GetQuery<prebulkpack>(x => x.OutboundOrder == scanningDto.OutboundOrder && x.WareHouseSysId == scanningDto.WarehouseSysId).ToList();
                if (!allPreBulkPack.Any())
                {
                    throw new Exception("容器不存在");
                }
                var currentPreBulkPack = allPreBulkPack.FirstOrDefault(p => p.StorageCase == scanningDto.StorageCase);
                if (currentPreBulkPack == null)
                {
                    throw new Exception("容器不存在");
                }

                if (currentPreBulkPack.prebulkpackdetails == null || !currentPreBulkPack.prebulkpackdetails.Any())
                {
                    throw new Exception("容器不存在任何商品");
                }

                var allContainerDetails = new List<prebulkpackdetail>();
                allPreBulkPack.ForEach(p => allContainerDetails.AddRange(p.prebulkpackdetails.Where(x => x.SkuSysId == scanningDto.SkuSysId)));
                var currentContainerDetails = currentPreBulkPack.prebulkpackdetails.Where(x => x.SkuSysId == scanningDto.SkuSysId).ToList();
                if (currentContainerDetails == null || currentContainerDetails.Count == 0)
                {
                    throw new Exception("容器中未找到该商品");
                }

                var outboundDetail = _crudRepository.GetQuery<outbounddetail>(p => p.OutboundSysId == outboundTransfer.OutboundSysId && p.SkuSysId == scanningDto.SkuSysId).FirstOrDefault();
                if (outboundDetail == null)
                {
                    throw new Exception("出库单中未找到该商品");
                }

                //商品装入多个交接箱的情况
                var transferOrderSysIds = _crudRepository.GetQuery<outboundtransferorder>(p => p.OutboundOrder == outboundTransfer.OutboundOrder).Select(p => p.SysId).ToList();
                var existsTransferOrderDetails = _crudRepository.GetQuery<outboundtransferorderdetail>(p => transferOrderSysIds.Contains(p.OutboundTransferOrderSysId) && p.SkuSysId == scanningDto.SkuSysId).ToList();
                int totalReviewQty = scanningDto.Qty;
                if (existsTransferOrderDetails != null && existsTransferOrderDetails.Any())
                {
                    totalReviewQty += existsTransferOrderDetails.Sum(p => p.Qty);
                }

                var totalQty = allContainerDetails.Sum(x => x.Qty);
                if (totalReviewQty > totalQty)
                {
                    throw new Exception("复核数量不能超过拣货数量");
                }

                if (totalReviewQty > outboundDetail.Qty)
                {
                    throw new Exception("复核数量不能超过出库数量");
                }

                //更新交接单状态和容器数据
                if (outboundTransfer.Status == (int)OutboundTransferOrderStatus.New)
                {
                    outboundTransfer.Status = (int)OutboundTransferOrderStatus.PrePack;
                    //outboundTransfer.PreBulkPackOrder = preBulkPack.PreBulkPackOrder;
                    //outboundTransfer.PreBulkPackSysId = preBulkPack.SysId;
                    outboundTransfer.TransferType = (int)OutboundTransferOrderType.Scattered;
                }
                //更新人
                outboundTransfer.UpdateBy = scanningDto.CurrentUserId;
                outboundTransfer.UpdateDate = DateTime.Now;
                outboundTransfer.UpdateUserName = scanningDto.CurrentDisplayName;

                //复核人
                outboundTransfer.ReviewBy = scanningDto.CurrentUserId;
                outboundTransfer.ReviewDate = DateTime.Now;
                outboundTransfer.ReviewUserName = scanningDto.CurrentDisplayName;
                _crudRepository.Update(outboundTransfer);


                var existsTransferOrderDetail = existsTransferOrderDetails.FirstOrDefault(p => p.OutboundTransferOrderSysId == outboundTransfer.SysId);
                if (existsTransferOrderDetail == null)
                {
                    outboundtransferorderdetail detail = new outboundtransferorderdetail
                    {
                        SysId = Guid.NewGuid(),
                        OutboundTransferOrderSysId = outboundTransfer.SysId,
                        SkuSysId = scanningDto.SkuSysId,
                        UOMSysId = outboundDetail.UOMSysId,
                        PackSysId = outboundDetail.PackSysId,
                        Qty = scanningDto.Qty,
                        CreateBy = scanningDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        CreateUserName = scanningDto.CurrentDisplayName,
                        UpdateBy = scanningDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        UpdateUserName = scanningDto.CurrentDisplayName
                    };
                    _crudRepository.Insert(detail);
                }
                else
                {
                    existsTransferOrderDetail.Qty += scanningDto.Qty;
                    existsTransferOrderDetail.UpdateBy = scanningDto.CurrentUserId;
                    existsTransferOrderDetail.UpdateDate = DateTime.Now;
                    existsTransferOrderDetail.UpdateUserName = scanningDto.CurrentDisplayName;
                    _crudRepository.Update(existsTransferOrderDetail);
                }
            }
            catch (Exception ex)
            {
                rsp.IsSucess = false;
                throw new Exception(ex.Message);
            }
            return rsp;
        }

        /// <summary>
        /// 散货复核完成
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RFCommResult SingleReviewFinish(RFSingleReviewFinishDto finishDto)
        {
            RFCommResult rsp = new RFCommResult { IsSucess = true };
            try
            {
                _rfOutboundRepository.ChangeDB(finishDto.WarehouseSysId);
                var transferOrders = _crudRepository.GetQuery<outboundtransferorder>(p => p.OutboundOrder == finishDto.OutboundOrder && p.WareHouseSysId == finishDto.WarehouseSysId).ToList();
                //foreach (var transferOrder in transferOrders)
                //{
                //    transferOrder.Status = (int)OutboundTransferOrderStatus.Finish;
                //    _crudRepository.Update(transferOrder);
                //}

                //记录整单差异
                var transferOrderSysIds = transferOrders.Select(p => p.SysId).ToList();
                var transferOrderDetails = _crudRepository.GetQuery<outboundtransferorderdetail>(p => transferOrderSysIds.Contains(p.OutboundTransferOrderSysId)).ToList();
                var pickDetails = _rfPickDetailRepository.GetContainerPickingDetailList(new RFPickQuery { OutboundOrder = finishDto.OutboundOrder, WarehouseSysId = finishDto.WarehouseSysId }).PickingDetails;
                var reviewedDetails = RedisWMS.GetRedisList<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewDiff, finishDto.OutboundOrder, finishDto.WarehouseSysId)) ?? new List<RFOutboundReviewDetailDto>();
                foreach (var transferOrderDetail in transferOrderDetails)
                {
                    var reviewedDetail = reviewedDetails.FirstOrDefault(p => p.SkuSysId == transferOrderDetail.SkuSysId);
                    if (reviewedDetail == null)
                    {
                        var pickDetail = pickDetails.Where(p => p.SkuSysId == transferOrderDetail.SkuSysId).ToList();
                        reviewedDetails.Add(new RFOutboundReviewDetailDto
                        {
                            SkuSysId = transferOrderDetail.SkuSysId,
                            SkuName = pickDetail.First().SkuName,
                            UPC = pickDetail.First().UPC,
                            OutboundQty = pickDetail.Sum(x => x.Qty),
                            PickQty = pickDetail.Sum(x => x.PickedQty),
                            ReviewQty = transferOrderDetail.Qty
                        });

                    }
                    else
                    {
                        reviewedDetail.ReviewQty += transferOrderDetail.Qty;
                    }
                }
                RedisWMS.SetRedis(reviewedDetails, string.Format(RedisSourceKey.RedisOutboundReviewDiff, finishDto.OutboundOrder, finishDto.WarehouseSysId));

                //还原拣货容器状态
                var containerSysIds = _crudRepository.GetQuery<prebulkpack>(p => p.OutboundOrder == finishDto.OutboundOrder && p.Status == (int)PreBulkPackStatus.RFPicking).Select(p => p.SysId).ToList();
                _WMSSqlRepository.ClearContainer(new ClearContainerDto
                {
                    ContainerSysIds = containerSysIds,
                    WarehouseSysId = finishDto.WarehouseSysId,
                    CurrentUserId = finishDto.CurrentUserId,
                    CurrentDisplayName = finishDto.CurrentDisplayName
                });
                //清除复核缓存
                //_redisAppService.CleanReviewRecords(finishDto.OutboundOrder, finishDto.WarehouseSysId);
            }
            catch (Exception ex)
            {
                rsp.IsSucess = false;
                throw new Exception(ex.Message);
            }
            return rsp;
        }

        /// <summary>
        /// 整件复核扫描
        /// </summary>
        /// <param name="scanningDto"></param>
        /// <returns></returns>
        public RFCommResult WholeReviewScanning(RFWholeReviewScanningDto scanningDto)
        {
            RFCommResult rsp = new RFCommResult { IsSucess = true };
            try
            {
                _crudRepository.ChangeDB(scanningDto.WarehouseSysId);

                var scanningDetails = RedisWMS.GetRedisList<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, scanningDto.OutboundOrder, scanningDto.WarehouseSysId)) ?? new List<RFOutboundReviewDetailDto>();
                var existsScanningDetail = scanningDetails.FirstOrDefault(p => p.SkuSysId == scanningDto.SkuSysId);

                var outbound = _crudRepository.GetQuery<outbound>(p => p.OutboundOrder == scanningDto.OutboundOrder && p.WareHouseSysId == scanningDto.WarehouseSysId).FirstOrDefault();
                if (outbound == null)
                {
                    throw new Exception("出库单不存在");
                }
                var transferOrder = _crudRepository.GetQuery<outboundtransferorder>(p => p.TransferOrder == scanningDto.TransferOrder && p.WareHouseSysId == scanningDto.WarehouseSysId).FirstOrDefault();
                if (transferOrder == null)
                {
                    throw new Exception("交接箱不存在");
                }
                var transferOrderDetailCount = _crudRepository.GetQuery<outboundtransferorderdetail>(p => p.OutboundTransferOrderSysId == transferOrder.SysId).Count();
                if (transferOrder.Status != (int)OutboundTransferOrderStatus.New || transferOrderDetailCount != 0)
                {
                    throw new Exception("交接箱已被使用");
                }
                var outboundDetail = _crudRepository.GetQuery<outbounddetail>(p => p.OutboundSysId == outbound.SysId && p.SkuSysId == scanningDto.SkuSysId).FirstOrDefault();
                if (outboundDetail == null)
                {
                    throw new Exception("出库单中未找到该商品");
                }
                var pickDetails = _crudRepository.GetQuery<pickdetail>(p => p.OutboundDetailSysId == outboundDetail.SysId).ToList();
                if (pickDetails == null || !pickDetails.Any())
                {
                    throw new Exception("拣货单中未找到该商品");
                }

                int totalReviewQty = scanningDto.Qty;
                if (existsScanningDetail != null)
                {
                    totalReviewQty += existsScanningDetail.ReviewQty;
                }

                var totalPickQty = pickDetails.Sum(p => p.PickedQty);
                if (totalReviewQty > totalPickQty)
                {
                    throw new Exception("复核数量不能超过拣货数量");
                }

                var singleReviewDetails = RedisWMS.GetRedisList<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewDiff, scanningDto.OutboundOrder, scanningDto.WarehouseSysId)) ?? new List<RFOutboundReviewDetailDto>();
                int singleReviewQty = 0;
                var singleReviewDetail = singleReviewDetails.FirstOrDefault(p => p.SkuSysId == scanningDto.SkuSysId);
                if (singleReviewDetail != null)
                {
                    singleReviewQty = singleReviewDetail.ReviewQty;
                }

                if (totalReviewQty > (totalPickQty - singleReviewQty))
                {
                    throw new Exception("复核数量不能超过整件数量");
                }

                //更新交接单主表状态和类型
                transferOrder.Status = (int)OutboundTransferOrderStatus.PrePack;
                transferOrder.TransferType = (int)OutboundTransferOrderType.Whole;
                //更新人
                transferOrder.UpdateBy = scanningDto.CurrentUserId;
                transferOrder.UpdateDate = DateTime.Now;
                transferOrder.UpdateUserName = scanningDto.CurrentDisplayName;

                //复核人
                transferOrder.ReviewBy = scanningDto.CurrentUserId;
                transferOrder.ReviewDate = DateTime.Now;
                transferOrder.ReviewUserName = scanningDto.CurrentDisplayName;

                _crudRepository.Update(transferOrder);

                //插入交接单子表数据
                outboundtransferorderdetail transferOrderDetail = new outboundtransferorderdetail
                {
                    SysId = Guid.NewGuid(),
                    OutboundTransferOrderSysId = transferOrder.SysId,
                    SkuSysId = scanningDto.SkuSysId,
                    UOMSysId = outboundDetail.UOMSysId,
                    PackSysId = outboundDetail.PackSysId,
                    Qty = scanningDto.Qty,
                    CreateBy = scanningDto.CurrentUserId,
                    CreateDate = DateTime.Now,
                    CreateUserName = scanningDto.CurrentDisplayName,
                    UpdateBy = scanningDto.CurrentUserId,
                    UpdateDate = DateTime.Now,
                    UpdateUserName = scanningDto.CurrentDisplayName
                };
                _crudRepository.Insert(transferOrderDetail);

                if (existsScanningDetail == null)
                {
                    var sku = _crudRepository.GetQuery<sku>(p => p.SysId == scanningDto.SkuSysId).FirstOrDefault();
                    RFOutboundReviewDetailDto detail = new RFOutboundReviewDetailDto
                    {
                        SkuSysId = scanningDto.SkuSysId,
                        SkuName = sku.SkuName,
                        UPC = sku.UPC,
                        OutboundQty = outboundDetail.Qty,
                        PickQty = totalPickQty,
                        ReviewQty = totalReviewQty
                    };
                    scanningDetails.Add(detail);
                }
                else
                {
                    existsScanningDetail.ReviewQty = totalReviewQty;
                }
                RedisWMS.SetRedis(scanningDetails, string.Format(RedisSourceKey.RedisOutboundReviewScanning, scanningDto.OutboundOrder, scanningDto.WarehouseSysId));
            }
            catch (Exception ex)
            {
                rsp.IsSucess = false;
                throw new Exception(ex.Message);
            }
            return rsp;
        }

        /// <summary>
        /// 获取整件待复核明细
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        public List<RFOutboundReviewDetailDto> GetWaitingWholeReviewDetails(RFOutboundQuery outboundQuery)
        {
            _crudRepository.ChangeDB(outboundQuery.WarehouseSysId);

            List<RFOutboundReviewDetailDto> rsp = new List<RFOutboundReviewDetailDto>();
            var outbound = _crudRepository.GetQuery<outbound>(p => p.OutboundOrder == outboundQuery.OutboundOrder && p.WareHouseSysId == outboundQuery.WarehouseSysId).FirstOrDefault();
            if (outbound != null)
            {
                var singleReviewDetails = RedisWMS.GetRedisList<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewDiff, outboundQuery.OutboundOrder, outboundQuery.WarehouseSysId)) ?? new List<RFOutboundReviewDetailDto>();
                var wholeReviewDetails = RedisWMS.GetRedisList<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, outboundQuery.OutboundOrder, outboundQuery.WarehouseSysId)) ?? new List<RFOutboundReviewDetailDto>();
                var outboundDetails = _crudRepository.GetQuery<outbounddetail>(p => p.OutboundSysId == outbound.SysId).ToList();
                var pickDetails = _crudRepository.GetQuery<pickdetail>(p => p.OutboundSysId == outbound.SysId && p.Status == (int)PickDetailStatus.New).ToList();
                var skuSysIds = outboundDetails.Select(p => p.SkuSysId).ToList();
                var skus = _crudRepository.GetQuery<sku>(p => skuSysIds.Contains(p.SysId)).ToList();
                foreach (var outboundDetail in outboundDetails)
                {
                    var sku = skus.FirstOrDefault(p => p.SysId == outboundDetail.SkuSysId);
                    var singleReviewDetail = singleReviewDetails.FirstOrDefault(p => p.SkuSysId == outboundDetail.SkuSysId);
                    var wholeReviewDetail = wholeReviewDetails.FirstOrDefault(p => p.SkuSysId == outboundDetail.SkuSysId);
                    var pickedQty = pickDetails.Where(p => p.SkuSysId == outboundDetail.SkuSysId).Sum(p => p.PickedQty);
                    RFOutboundReviewDetailDto detail = new RFOutboundReviewDetailDto();
                    detail.SkuSysId = outboundDetail.SkuSysId;
                    detail.SkuName = sku.SkuName;
                    detail.UPC = sku.UPC;
                    detail.OutboundQty = outboundDetail.Qty;
                    detail.WholeQty = singleReviewDetail != null ? pickedQty - singleReviewDetail.ReviewQty : pickedQty;
                    detail.ReviewQty = wholeReviewDetail != null ? wholeReviewDetail.ReviewQty : 0;
                    if (detail.WholeQty != 0)
                    {
                        rsp.Add(detail);
                    }
                }
            }
            return rsp;
        }

        /// <summary>
        /// 整件复核完成
        /// </summary>
        /// <param name="finishDto"></param>
        /// <returns></returns>
        public RFCommResult WholeReviewFinish(RFWholeReviewFinishDto finishDto)
        {
            _crudRepository.ChangeDB(finishDto.WarehouseSysId);

            //记录整单差异
            var scanningDetails = RedisWMS.GetRedisList<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, finishDto.OutboundOrder, finishDto.WarehouseSysId)) ?? new List<RFOutboundReviewDetailDto>();
            var pickDetails = _rfPickDetailRepository.GetContainerPickingDetailList(new RFPickQuery { OutboundOrder = finishDto.OutboundOrder, WarehouseSysId = finishDto.WarehouseSysId }).PickingDetails;
            var reviewedDetails = RedisWMS.GetRedisList<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewDiff, finishDto.OutboundOrder, finishDto.WarehouseSysId)) ?? new List<RFOutboundReviewDetailDto>();
            foreach (var scanningDetail in scanningDetails)
            {
                var reviewedDetail = reviewedDetails.FirstOrDefault(p => p.SkuSysId == scanningDetail.SkuSysId);
                if (reviewedDetail == null)
                {
                    var pickDetail = pickDetails.Where(p => p.SkuSysId == scanningDetail.SkuSysId).ToList();
                    reviewedDetails.Add(new RFOutboundReviewDetailDto
                    {
                        SkuSysId = scanningDetail.SkuSysId,
                        SkuName = pickDetail.First().SkuName,
                        UPC = pickDetail.First().UPC,
                        OutboundQty = pickDetail.Sum(x => x.Qty),
                        PickQty = pickDetail.Sum(x => x.PickedQty),
                        ReviewQty = scanningDetail.ReviewQty
                    });
                }
                else
                {
                    reviewedDetail.ReviewQty += scanningDetail.ReviewQty;
                }
            }
            RedisWMS.SetRedis(reviewedDetails, string.Format(RedisSourceKey.RedisOutboundReviewDiff, finishDto.OutboundOrder, finishDto.WarehouseSysId));

            RedisWMS.CleanRedis<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, finishDto.OutboundOrder, finishDto.WarehouseSysId));
            return new RFCommResult { IsSucess = true };
        }

        /// <summary>
        /// 整单差异
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        public RFOutboundReviewDiffDto GetOutboundReviewDiff(RFOutboundQuery outboundQuery)
        {
            RFOutboundReviewDiffDto rsp = new RFOutboundReviewDiffDto();
            var reviewedDetails = RedisWMS.GetRedisList<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewDiff, outboundQuery.OutboundOrder, outboundQuery.WarehouseSysId)) ?? new List<RFOutboundReviewDetailDto>();
            rsp.ReviewedList = reviewedDetails.Where(p => p.ReviewQty != 0).ToList();
            rsp.ToReviewList = reviewedDetails.Where(p => p.ReviewQty == 0).ToList();
            rsp.ReviewDiffList = reviewedDetails.Where(p => p.ReviewQty != 0 && p.PickQty != p.ReviewQty).ToList();
            //RedisWMS.CleanRedis<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewDiff, outboundQuery.OutboundOrder, outboundQuery.WarehouseSysId));
            return rsp;
        }
    }
}
