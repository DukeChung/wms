using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class RFStockTakeAppService : WMSApplicationService, IRFStockTakeAppService
    {
        private ICrudRepository _crudRepository = null;
        private IRFStockTakeRepository _rFStockTakeRepository = null;
        private IPackageAppService _packageAppService = null;
        private IBaseAppService _baseAppService = null;

        public RFStockTakeAppService(ICrudRepository crudRepository, IRFStockTakeRepository rFStockTakeRepository, IPackageAppService packageAppService, IBaseAppService baseAppService)
        {
            this._crudRepository = crudRepository;
            _rFStockTakeRepository = rFStockTakeRepository;
            _packageAppService = packageAppService;
            this._baseAppService = baseAppService;
        }

        /// <summary>
        /// 查询未盘点库存
        /// </summary>
        /// <param name="inventoryQuery"></param>
        /// <returns></returns>
        public List<RFInventoryListDto> GetInventoryNoStockTakeList(RFInventoryQuery inventoryQuery)
        {
            return _rFStockTakeRepository.GetInventoryNoStockTakeList(inventoryQuery);
        }

        /// <summary>
        /// 初盘保存盘点单和盘点明细或修改盘点明细
        /// </summary>
        /// <param name="stockTakeFirst"></param>
        /// <returns></returns>
        public RFCommResult SaveStockTake(StockTakeFirstDto stockTakeFirst)
        {
            var result = new RFCommResult() { IsSucess = false };

            try
            {
                //var task = new Task(() =>
                //{
                    #region  盘点记录创建

                    var loc =
                        _crudRepository.GetQuery<location>(
                            x => x.Loc == stockTakeFirst.Loc && x.WarehouseSysId == stockTakeFirst.WarehouseSysId)
                            .FirstOrDefault();
                    if (loc == null)
                    {
                        throw new Exception("货位不存在");
                    }

                    var sku = _crudRepository.GetQuery<sku>(x => x.UPC == stockTakeFirst.UPC).FirstOrDefault();
                    if (sku == null)
                    {
                        throw new Exception("商品不存在");
                    }

                    var stockTake =
                        _crudRepository.GetQuery<stocktake>(
                            x =>
                                x.AssignBy == stockTakeFirst.UserId && x.WarehouseSysId == stockTakeFirst.WarehouseSysId)
                            .OrderByDescending(x => x.UpdateDate)
                            .FirstOrDefault();
                    if ((stockTake != null && stockTake.Status == (int) StockTakeStatus.StockTakeFinished) || stockTake == null)
                    {
                        var newStockTake = new stocktake()
                        {
                            SysId = Guid.NewGuid(),
                            //StockTakeOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberStockTake),
                            StockTakeOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberStockTake),
                            Status = (int) StockTakeStatus.StockTake,
                            StockTakeType = (int) StockTakeType.Random,
                            StartTime = DateTime.Now,
                            AssignBy = stockTakeFirst.UserId,
                            AssignUserName = stockTakeFirst.CurrentDisplayName,
                            WarehouseSysId = stockTakeFirst.WarehouseSysId,
                            ZoneSysId = null,
                            StartLoc = "",
                            EndLoc = "",
                            SkuClassSysId1 = null,
                            SkuClassSysId2 = null,
                            SkuClassSysId3 = null,
                            SkuClassSysId4 = null,
                            SkuClassSysId5 = null,
                            CreateBy = stockTakeFirst.UserId,
                            CreateDate = DateTime.Now,
                            CreateUserName = stockTakeFirst.CurrentDisplayName,
                            UpdateBy = stockTakeFirst.UserId,
                            UpdateDate = DateTime.Now,
                            UpdateUserName = stockTakeFirst.CurrentDisplayName
                        };
                        _crudRepository.Insert(newStockTake);

                        //查询库存
                        var invSkuLoc =
                            _crudRepository.GetQuery<invskuloc>(
                                x =>
                                    x.WareHouseSysId == stockTakeFirst.WarehouseSysId && x.Loc == stockTakeFirst.Loc &&
                                    x.SkuSysId == sku.SysId).FirstOrDefault();

                        var newStockTakeDetail = new stocktakedetail()
                        {
                            SysId = Guid.NewGuid(),
                            StockTakeSysId = newStockTake.SysId,
                            SkuSysId = sku.SysId,
                            Loc = stockTakeFirst.Loc,
                            Lot = "",
                            Lpn = "",
                            StockTakeTime = DateTime.Now,
                            Qty = invSkuLoc != null ? invSkuLoc.Qty : 0,
                            ReplayQty = 0,
                            StockTakeQty = stockTakeFirst.StockTakeQty,
                            Remark = "",
                            Status = (int) StockTakeDetailStatus.StockTake,
                            CreateBy = stockTakeFirst.UserId,
                            CreateDate = DateTime.Now,
                            CreateUserName = stockTakeFirst.CurrentDisplayName,
                            UpdateBy = stockTakeFirst.UserId,
                            UpdateDate = DateTime.Now,
                            UpdateUserName = stockTakeFirst.CurrentDisplayName
                        };
                        _crudRepository.Insert(newStockTakeDetail);

                    }
                    else
                    {
                        var stockTakeDetail =
                            _crudRepository.GetQuery<stocktakedetail>(
                                x =>
                                    x.StockTakeSysId == stockTake.SysId && x.SkuSysId == sku.SysId &&
                                    x.Loc == stockTakeFirst.Loc).FirstOrDefault();
                        if (stockTakeDetail != null)
                        {
                            if (stockTakeDetail.Status == (int) StockTakeDetailStatus.StockTake ||
                                stockTakeDetail.Status == (int) StockTakeDetailStatus.New)
                            {
                                stockTakeDetail.StockTakeQty = stockTakeFirst.StockTakeQty;
                                stockTakeDetail.UpdateBy = stockTakeFirst.UserId;
                                stockTakeDetail.UpdateDate = DateTime.Now;
                                stockTakeDetail.UpdateUserName = stockTakeFirst.CurrentDisplayName;
                                _crudRepository.Update(stockTakeDetail);
                            }
                            else
                            {
                                throw new Exception("初盘已结束");
                            }
                        }
                        else
                        {
                            //查询库存
                            var invSkuLoc =
                                _crudRepository.GetQuery<invskuloc>(
                                    x =>
                                        x.WareHouseSysId == stockTakeFirst.WarehouseSysId && x.Loc == stockTakeFirst.Loc &&
                                        x.SkuSysId == sku.SysId).FirstOrDefault();

                            var newStockTakeDetail = new stocktakedetail()
                            {
                                SysId = Guid.NewGuid(),
                                StockTakeSysId = stockTake.SysId,
                                SkuSysId = sku.SysId,
                                Loc = stockTakeFirst.Loc,
                                Lot = "",
                                Lpn = "",
                                StockTakeTime = DateTime.Now,
                                Qty = invSkuLoc != null ? invSkuLoc.Qty : 0,
                                ReplayQty = 0,
                                StockTakeQty = stockTakeFirst.StockTakeQty,
                                Remark = "",
                                Status = (int) StockTakeDetailStatus.StockTake,
                                CreateBy = stockTakeFirst.UserId,
                                CreateDate = DateTime.Now,
                                CreateUserName = stockTakeFirst.CurrentDisplayName,
                                UpdateBy = stockTakeFirst.UserId,
                                UpdateDate = DateTime.Now,
                                UpdateUserName = stockTakeFirst.CurrentDisplayName
                            };
                            _crudRepository.Insert(newStockTakeDetail);
                        }

                    }

                    #endregion
                //}
                //    );
                

                result.IsSucess = true;
                result.Message = "盘点成功";
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 初盘清单
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public RFStockTakeListingDto GetStockTakeFirstList(RFStockTakeQuery stockTakeQuery)
        {
            return _rFStockTakeRepository.GetStockTakeFirstList(stockTakeQuery);
        }

        /// <summary>
        /// 获取待初盘单据列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public Pages<RFStockTakeListDto> GetStockTakeFirstListByPaging(RFStockTakeQuery stockTakeQuery)
        {
            _crudRepository.ChangeDB(stockTakeQuery.WarehouseSysId);
            return _rFStockTakeRepository.GetStockTakeFirstListByPaging(stockTakeQuery);
        }

        public RFCheckStockTakeFirstDetailSkuDto CheckStockTakeFirstDetailSku(string upc)
        {
            RFCheckStockTakeFirstDetailSkuDto rsp = new RFCheckStockTakeFirstDetailSkuDto();
            rsp.Skus = _rFStockTakeRepository.GetSkuByUPC(upc);
            return rsp;
        }

        /// <summary>
        /// 获取初盘明细
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public List<StockTakeFirstListDto> GetStockTakeFirstDetailList(RFStockTakeQuery stockTakeQuery)
        {
            _crudRepository.ChangeDB(stockTakeQuery.WarehouseSysId);
            return _rFStockTakeRepository.GetStockTakeFirstDetailList(stockTakeQuery);
        }

        /// <summary>
        /// 初盘扫描
        /// </summary>
        /// <param name="stockTakeFirstDto"></param>
        /// <returns></returns>
        public RFCommResult StockTakeFirstScanning(StockTakeFirstDto stockTakeFirstDto)
        {
            _crudRepository.ChangeDB(stockTakeFirstDto.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                var stockTake = _crudRepository.GetQuery<stocktake>(p => p.StockTakeOrder == stockTakeFirstDto.StockTakeOrder
                && p.WarehouseSysId == stockTakeFirstDto.WarehouseSysId && p.AssignBy == stockTakeFirstDto.CurrentUserId
                && (p.Status == (int)StockTakeStatus.Started || p.Status == (int)StockTakeStatus.StockTake)).FirstOrDefault();
                if (stockTake == null)
                {
                    result.IsSucess = false;
                    result.Message = "未找到盘点单";
                    return result;
                }

                #region 验证盘点单是否有原材料和成品混合
                var sku = _crudRepository.Get<sku>(stockTakeFirstDto.SkuSysId);
                if(sku == null)
                {
                    result.IsSucess = false;
                    result.Message = "此商品不存在";
                    return result;
                }

                if (!_rFStockTakeRepository.GetStockTakeFirstMaterialProduct(stockTake.SysId, sku.IsMaterial))
                {
                    result.IsSucess = false;
                    result.Message = "原材料和普通商品不允许盘点到一个盘点单";
                    return result;
                }
                #endregion

                if (stockTake.Status == (int)StockTakeStatus.Started)
                {
                    stockTake.Status = (int)StockTakeStatus.StockTake;
                    stockTake.StartTime = DateTime.Now;
                    _crudRepository.Update(stockTake);
                }

                //此处增加单位转换数量，用于更新库存表
                int transQty = 0;
                pack transPack = new pack();
                if (_packageAppService.GetSkuConversiontransQty(stockTakeFirstDto.SkuSysId, stockTakeFirstDto.InputQty, out transQty, ref transPack) == true)
                {
                    //单位转换更新库存后需要记录
                    unitconversiontran unitTran = new unitconversiontran()
                    {
                        WareHouseSysId = stockTakeFirstDto.WarehouseSysId,
                        DocOrder = stockTake.StockTakeOrder,
                        DocSysId = stockTake.SysId,
                        DocDetailSysId = Guid.Empty,
                        SkuSysId = stockTakeFirstDto.SkuSysId,
                        FromQty = stockTakeFirstDto.InputQty,
                        ToQty = transQty,
                        Loc = "",
                        Lot = "",
                        Lpn = "",
                        Status = "Done",
                        PackSysId = transPack.SysId,
                        PackCode = transPack.PackCode,
                        FromUOMSysId = transPack.FieldUom02 ?? Guid.Empty,
                        ToUOMSysId = transPack.FieldUom01 ?? Guid.Empty,
                        CreateBy = stockTakeFirstDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = stockTakeFirstDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        TransType = InvTransType.StockTake,
                        SourceTransType = InvSourceTransType.ReplayStockTake
                    };
                    _crudRepository.Insert(unitTran);
                    stockTakeFirstDto.StockTakeQty = transQty;
                }
                else
                {
                    stockTakeFirstDto.StockTakeQty = Convert.ToInt32(stockTakeFirstDto.InputQty);
                }

                var stockTakeDetail = _crudRepository.GetQuery<stocktakedetail>(p => p.StockTakeSysId == stockTake.SysId && p.SkuSysId == stockTakeFirstDto.SkuSysId && p.Loc == stockTakeFirstDto.Loc).FirstOrDefault();
                if (stockTakeDetail != null)
                {
                    stockTakeDetail.Status = (int)StockTakeDetailStatus.StockTake;
                    stockTakeDetail.StockTakeQty += stockTakeFirstDto.StockTakeQty;
                    stockTakeDetail.UpdateBy = stockTakeFirstDto.CurrentUserId;
                    stockTakeDetail.UpdateDate = DateTime.Now;
                    stockTakeDetail.UpdateUserName = stockTakeFirstDto.CurrentDisplayName;
                    _crudRepository.Update(stockTakeDetail);
                }
                else
                {
                    throw new Exception("盘点明细不存在，请检查UPC或库位是否正确");
                }
                result.IsSucess = true;
                result.Message = string.Format("UPC：{0}，数量：{1}，盘点成功！", stockTakeFirstDto.UPC, stockTakeFirstDto.InputQty);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 复盘单据列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public Pages<RFStockTakeListDto> GetStockTakeSecondListByPage(RFStockTakeQuery stockTakeQuery)
        {
            _crudRepository.ChangeDB(stockTakeQuery.WarehouseSysId);
            return _rFStockTakeRepository.GetStockTakeSecondListByPage(stockTakeQuery);
        }

        /// <summary>
        /// 复盘清单
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public List<StockTakeSecondListDto> GetStockTakeSecondList(RFStockTakeQuery stockTakeQuery)
        {
            _crudRepository.ChangeDB(stockTakeQuery.WarehouseSysId);
            return _rFStockTakeRepository.GetStockTakeSecondList(stockTakeQuery);
        }

        /// <summary>
        /// 复盘
        /// </summary>
        /// <returns></returns>
        public RFCommResult StockTakeSecond(StockTakeSecondDto stockTakeSecond)
        {
            _crudRepository.ChangeDB(stockTakeSecond.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                var stockTake = _crudRepository.GetQuery<stocktake>(x => x.StockTakeOrder == stockTakeSecond.StockTakeOrder 
                && x.WarehouseSysId == stockTakeSecond.WarehouseSysId
                && x.ReplayBy == stockTakeSecond.CurrentUserId
                && x.Status == (int)StockTakeStatus.Replay).FirstOrDefault();
                if(stockTake == null)
                {
                    throw new Exception("待盘点单据中不存在此单据");
                }

                sku sku = null;

                if (stockTakeSecond.SkuSysId.HasValue)
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.SysId == stockTakeSecond.SkuSysId).FirstOrDefault();
                }
                else
                {
                    var skuList = _crudRepository.GetQuery<sku>(x => x.UPC == stockTakeSecond.UPC).ToList();

                    var query = from a in stockTake.stocktakedetails
                                join b in skuList on a.SkuSysId equals b.SysId
                                select b;

                    sku = query.FirstOrDefault();
                }

                if (sku == null)
                {
                    throw new Exception("商品不存在");
                }

                //此处增加单位转换数量，用于更新库存表
                int transQty = 0;
                pack transPack = new pack();
                if (_packageAppService.GetSkuConversiontransQty(sku.SysId, stockTakeSecond.InputQty, out transQty, ref transPack) == true)
                {
                    //单位转换更新库存后需要记录
                    unitconversiontran unitTran = new unitconversiontran()
                    {
                        WareHouseSysId = stockTakeSecond.WarehouseSysId,
                        DocOrder = stockTake.StockTakeOrder,
                        DocSysId = stockTake.SysId,
                        DocDetailSysId = Guid.Empty,
                        SkuSysId = sku.SysId,
                        FromQty = stockTakeSecond.InputQty,
                        ToQty = transQty,
                        Loc = "",
                        Lot = "",
                        Lpn = "",
                        Status = "Done",
                        PackSysId = transPack.SysId,
                        PackCode = transPack.PackCode,
                        FromUOMSysId = transPack.FieldUom02 ?? Guid.Empty,
                        ToUOMSysId = transPack.FieldUom01 ?? Guid.Empty,
                        CreateBy = stockTakeSecond.CurrentUserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = stockTakeSecond.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        TransType = InvTransType.StockTake,
                        SourceTransType = InvSourceTransType.ReplayStockTake
                    };
                    _crudRepository.Insert(unitTran);

                    stockTakeSecond.ReplayQty = transQty;
                }
                else
                {
                    stockTakeSecond.ReplayQty = Convert.ToInt32(stockTakeSecond.InputQty);
                }

                var stockTakeDetail = _crudRepository.GetQuery<stocktakedetail>(x => x.StockTakeSysId == stockTake.SysId && x.SkuSysId == sku.SysId && x.Loc == stockTakeSecond.Loc && x.Status == (int)StockTakeDetailStatus.Replay).FirstOrDefault();
                if(stockTakeDetail != null)
                {
                    stockTakeDetail.ReplayQty = stockTakeDetail.ReplayQty.GetValueOrDefault() + stockTakeSecond.ReplayQty;
                    stockTakeDetail.UpdateBy = stockTakeSecond.CurrentUserId;
                    stockTakeDetail.UpdateDate = DateTime.Now;
                    stockTakeDetail.UpdateUserName = stockTakeSecond.CurrentDisplayName;
                    _crudRepository.Update(stockTakeDetail);
                }
                else
                {
                    throw new Exception("盘点明细不存在，请检查UPC或库位是否正确");
                    //throw new Exception("未找到盘点信息");
                }

                result.IsSucess = true;
                result.Message = "复盘成功";
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 修改盘点单状态
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public RFCommResult UpdateStockTakeStatus(RFStockTakeQuery stockTakeQuery)
        {
            _crudRepository.ChangeDB(stockTakeQuery.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                var stockTake = _crudRepository.GetQuery<stocktake>(x => x.StockTakeOrder == stockTakeQuery.StockTakeOrder && x.WarehouseSysId == stockTakeQuery.WarehouseSysId).FirstOrDefault();
                if(stockTake == null)
                {
                    throw new Exception("盘点单不存在");
                }

                if (stockTakeQuery.Status == (int)StockTakeStatus.StockTakeFinished)
                {
                    var stockTakeDetails = _crudRepository.GetQuery<stocktakedetail>(p => p.StockTakeSysId == stockTake.SysId && p.Status == (int)StockTakeDetailStatus.New).ToList();
                    foreach (var detail in stockTakeDetails)
                    {
                        detail.StockTakeQty = 0;
                        detail.Status = (int)StockTakeDetailStatus.StockTake;
                        detail.UpdateBy = stockTakeQuery.CurrentUserId;
                        detail.UpdateDate = DateTime.Now;
                        detail.UpdateUserName = stockTakeQuery.CurrentDisplayName;
                        _crudRepository.Update(detail);
                    }
                }
                else if (stockTakeQuery.Status == (int)StockTakeStatus.ReplayFinished)
                {
                    var stockTakeDetails = _crudRepository.GetQuery<stocktakedetail>(p => p.StockTakeSysId == stockTake.SysId && p.Status == (int)StockTakeDetailStatus.Replay && !p.ReplayQty.HasValue).ToList();
                    foreach (var detail in stockTakeDetails)
                    {
                        detail.ReplayQty = 0;
                        detail.UpdateBy = stockTakeQuery.CurrentUserId;
                        detail.UpdateDate = DateTime.Now;
                        detail.UpdateUserName = stockTakeQuery.CurrentDisplayName;
                        _crudRepository.Update(detail);
                    }
                }

                //if(stockTakeQuery.Status == (int)StockTakeStatus.ReplayFinished)
                //{
                //    var stockTakeDetails = _crudRepository.GetQuery<stocktakedetail>(x => x.StockTakeSysId == stockTake.SysId && x.Status == (int)StockTakeDetailStatus.Replay && x.ReplayQty == null);
                //    if(stockTakeDetails != null && stockTakeDetails.Count() > 0)
                //    {
                //        throw new Exception("还有待复盘的商品，请检查");
                //    }
                //}

                stockTake.Status = stockTakeQuery.Status;
                stockTake.UpdateBy = stockTakeQuery.CurrentUserId;
                stockTake.UpdateDate = DateTime.Now;
                stockTake.UpdateUserName = stockTakeQuery.CurrentDisplayName;
                _crudRepository.Update(stockTake);

                result.IsSucess = true;
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 查询盘点单
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public RFStockTakeListDto GetStockTakeByOrder(RFStockTakeQuery stockTakeQuery)
        {
            _crudRepository.ChangeDB(stockTakeQuery.WarehouseSysId);
            var stockTake = _crudRepository.GetQuery<stocktake>(x => x.StockTakeOrder == stockTakeQuery.StockTakeOrder && x.WarehouseSysId == stockTakeQuery.WarehouseSysId && x.Status == stockTakeQuery.Status);
            if(stockTakeQuery.Status == (int)StockTakeStatus.New)
            {
                stockTake = stockTake.Where(x => x.AssignBy == stockTakeQuery.CurrentUserId);
            }

            if(stockTakeQuery.Status == (int)StockTakeStatus.Replay)
            {
                stockTake = stockTake.Where(x => x.ReplayBy == stockTakeQuery.CurrentUserId);
            }

            return stockTake.FirstOrDefault().JTransformTo<RFStockTakeListDto>();
        }
    }
}
