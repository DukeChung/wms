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
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;

namespace NBK.ECService.WMS.Application
{
    public class StockTakeAppService : WMSApplicationService, IStockTakeAppService
    {
        private ICrudRepository _crudRepository = null;
        private IStockTakeRepository _stockTakeRepository = null;
        private IBaseAppService _baseAppService = null;
        private IFrozenAppService _frozenAppService = null;
        private IWMSSqlRepository _wmsSqlRepository = null;

        public StockTakeAppService(ICrudRepository crudRepository, IStockTakeRepository stockTakeRepository, IBaseAppService baseAppService, IFrozenAppService frozenAppService, IWMSSqlRepository wmsSqlRepository)
        {
            this._crudRepository = crudRepository;
            this._stockTakeRepository = stockTakeRepository;
            this._baseAppService = baseAppService;
            this._frozenAppService = frozenAppService;
            this._wmsSqlRepository = wmsSqlRepository;
        }

        /// <summary>
        /// 获取盘点列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public Pages<StockTakeListDto> GetStockTakeList(StockTakeQuery stockTakeQuery)
        {
            _crudRepository.ChangeDB(stockTakeQuery.WarehouseSysId);
            return _stockTakeRepository.GetStockTakeListByPaging(stockTakeQuery);
        }

        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="stockTakeDto"></param>
        /// <returns></returns>
        public Guid AddStockTake(StockTakeDto stockTakeDto)
        {

            _crudRepository.ChangeDB(stockTakeDto.WarehouseSysId);
            stockTakeDto.SysId = Guid.NewGuid();
            //stockTakeDto.StockTakeOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberStockTake);
            stockTakeDto.StockTakeOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberStockTake);
            stockTakeDto.Status = (int)StockTakeStatus.New;
            stockTakeDto.CreateDate = DateTime.Now;
            stockTakeDto.UpdateDate = DateTime.Now;
            return _crudRepository.InsertAndGetId(stockTakeDto.JTransformTo<stocktake>());
        }

        /// <summary>
        /// 获取仓库下拉框
        /// </summary>
        /// <returns></returns>
        public List<SelectItem> GetSelectWarehouse()
        {
            return _crudRepository.GetAllList<warehouse>().Where(p => p.IsActive).Select(p => new SelectItem { Text = p.Name, Value = p.SysId.ToString() }).ToList();
        }

        /// <summary>
        /// 获取盘点单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public StockTakeViewDto GetStockTakeViewById(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var stockTake = _crudRepository.GetQuery<stocktake>(p => p.SysId == sysId).FirstOrDefault();
            var rsp = stockTake.JTransformTo<StockTakeViewDto>();
            if (rsp != null)
            {
                var warehouse = _crudRepository.GetQuery<warehouse>(p => p.SysId == rsp.WarehouseSysId).FirstOrDefault();
                if (warehouse != null)
                {
                    rsp.WarehouseName = warehouse.Name;
                }
            }
            return rsp;
        }

        /// <summary>
        /// 获取盘点单明细
        /// </summary>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        public Pages<StockTakeDetailViewDto> GetStockTakeDetailList(StockTakeViewQuery stockTakeViewQuery)
        {
            _crudRepository.ChangeDB(stockTakeViewQuery.WarehouseSysId);
            return _stockTakeRepository.GetStockTakeDetailListByPaging(stockTakeViewQuery);
        }

        /// <summary>
        /// 获取盘点单差异
        /// </summary>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        public Pages<StockTakeDetailViewDto> GetStockTakeDiffList(StockTakeViewQuery stockTakeViewQuery)
        {
            _crudRepository.ChangeDB(stockTakeViewQuery.WarehouseSysId);
            return _stockTakeRepository.GetStockTakeDiffListByPaging(stockTakeViewQuery); ;
        }

        /// <summary>
        /// 盘点单明细复盘
        /// </summary>
        /// <param name="replayStockTakeDto"></param>
        public void ReplayStockTakeDetail(ReplayStockTakeDto replayStockTakeDto)
        {
            _crudRepository.ChangeDB(replayStockTakeDto.WarehouseSysId);
            _crudRepository.Update<stocktake>(replayStockTakeDto.StockTakeSysId, p =>
            {
                p.Status = (int)StockTakeStatus.Replay;
                p.ReplayBy = replayStockTakeDto.ReplayBy;
                p.ReplayUserName = replayStockTakeDto.ReplayUserName;
                p.UpdateBy = replayStockTakeDto.UpdateBy;
                p.UpdateDate = DateTime.Now;
                p.UpdateUserName = replayStockTakeDto.UpdateUserName;
            });
            var diffDetails = _crudRepository.GetQuery<stocktakedetail>(p => p.StockTakeSysId == replayStockTakeDto.StockTakeSysId && p.StockTakeQty != p.Qty && p.Status == (int)StockTakeDetailStatus.StockTake);
            foreach (var diffDetail in diffDetails)
            {
                diffDetail.Status = (int)StockTakeDetailStatus.Replay;
                diffDetail.UpdateBy = replayStockTakeDto.UpdateBy;
                diffDetail.UpdateDate = DateTime.Now;
                diffDetail.UpdateUserName = replayStockTakeDto.UpdateUserName;
                _crudRepository.Update(diffDetail);
            }
        }

        /// <summary>
        /// 获取生成损益单数据
        /// </summary>
        /// <param name="createAdjustmentDto"></param>
        /// <returns></returns>
        public AdjustmentDto GetAdjustmentDto(CreateAdjustmentDto createAdjustmentDto)
        {
            _crudRepository.ChangeDB(createAdjustmentDto.WarehouseSysId);
            List<stocktakedetail> stockTakeDetails = _crudRepository.GetAllList<stocktakedetail>(p => createAdjustmentDto.DetailSysIds.Contains(p.SysId));
            List<Guid> skuSysIds = stockTakeDetails.Select(p => p.SkuSysId).ToList();
            List<sku> skuList = _crudRepository.GetAllList<sku>(p => skuSysIds.Contains(p.SysId));

            AdjustmentDto adjustmentDto = new AdjustmentDto { AdjustmentDetailList = new List<AdjustmentDetailDto>() };
            adjustmentDto.WarehouseSysId = createAdjustmentDto.WarehouseSysId;
            adjustmentDto.Type = (int)AdjustmentType.ProfiAndLoss;
            adjustmentDto.SourceType = PublicConst.AJSourceTypeStockTake;
            adjustmentDto.SourceOrder = string.Empty;
            if (stockTakeDetails != null && stockTakeDetails.Any())
            {
                stockTakeDetails.ForEach(p =>
                {
                    sku sku = skuList.FirstOrDefault(x => x.SysId == p.SkuSysId);
                    if (sku != null)
                    {
                        int qty = p.Status == (int)StockTakeDetailStatus.StockTake ? p.StockTakeQty - p.Qty : (p.Status == (int)StockTakeDetailStatus.Replay ? p.ReplayQty.GetValueOrDefault() - p.Qty : 0);
                        if (qty != 0)
                        {
                            adjustmentDto.AdjustmentDetailList.Add(new AdjustmentDetailDto
                            {
                                SkuSysId = sku.SysId,
                                SkuCode = sku.SkuCode,
                                SkuName = sku.SkuName,
                                SkuDescr = sku.SkuDescr,
                                Loc = p.Loc,
                                Lot = p.Lot,
                                Lpn = p.Lpn,
                                Qty = qty,
                            });
                        }
                    }
                });
            }
            return adjustmentDto;
        }

        /// <summary>
        /// 盘点完成
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public bool StockTakeComplete(StockTakeCompleteDto stockTake)
        {
            _crudRepository.ChangeDB(stockTake.WarehouseSysId);
            try
            {
                if (stockTake != null && stockTake.SysIds != null && stockTake.SysIds.Count > 0)
                {
                    foreach (var sysId in stockTake.SysIds)
                    {
                        var stocktake = _crudRepository.Get<stocktake>(sysId);
                        var stocktakeDetails = _crudRepository.GetQuery<stocktakedetail>(p => p.StockTakeSysId == sysId).ToList();

                        #region 解冻库存
                        List<FrozenRequestDto> requestList = new List<FrozenRequestDto>();
                        string frozeMemo = $"解冻库存，盘点单号[{stocktake.StockTakeOrder}]";
                        if (stocktake.StockTakeType == (int)StockTakeType.Location)
                        {
                            if (stocktake.ZoneSysId.HasValue)
                            {
                                requestList.Add(new FrozenRequestDto
                                {
                                    Type = (int)FrozenType.Zone,
                                    ZoneSysId = stocktake.ZoneSysId.Value,
                                    Memo = frozeMemo,
                                    WarehouseSysId = stockTake.WarehouseSysId,
                                    CurrentUserId = stockTake.CurrentUserId,
                                    CurrentDisplayName = stockTake.CurrentDisplayName
                                });
                            }
                            else
                            {
                                requestList.Add(new FrozenRequestDto
                                {
                                    Type = (int)FrozenType.Location,
                                    Loc = stocktakeDetails.FirstOrDefault().Loc,
                                    Memo = frozeMemo,
                                    WarehouseSysId = stockTake.WarehouseSysId,
                                    CurrentUserId = stockTake.CurrentUserId,
                                    CurrentDisplayName = stockTake.CurrentDisplayName
                                });
                            }
                        }
                        else if (stocktake.StockTakeType == (int)StockTakeType.Sku)
                        {
                            requestList.Add(new FrozenRequestDto
                            {
                                Type = (int)FrozenType.Sku,
                                SkuList = stocktakeDetails.Select(p => p.SkuSysId).ToList(),
                                Memo = frozeMemo,
                                WarehouseSysId = stockTake.WarehouseSysId,
                                CurrentUserId = stockTake.CurrentUserId,
                                CurrentDisplayName = stockTake.CurrentDisplayName
                            });
                        }
                        //动碰盘点，按照库位、商品解冻库存
                        else if (stocktake.StockTakeType == (int)StockTakeType.Touch)
                        {
                            requestList.Add(new FrozenRequestDto
                            {
                                Type = (int)FrozenType.LocSku,
                                LocSkuList = stocktakeDetails.Select(p => new FrozenLocSkuDto { Loc = p.Loc, SkuSysId = p.SkuSysId }).Distinct().ToList(),
                                Memo = frozeMemo,
                                WarehouseSysId = stockTake.WarehouseSysId,
                                CurrentUserId = stockTake.CurrentUserId,
                                CurrentDisplayName = stockTake.CurrentDisplayName
                            });

                            //var locs = stocktakeDetails.Select(p => p.Loc).Distinct();
                            //foreach (var loc in locs)
                            //{
                            //    requestList.Add(new FrozenRequestDto
                            //    {
                            //        Type = (int)FrozenType.Location,
                            //        Loc = loc,
                            //        Memo = frozeMemo,
                            //        WarehouseSysId = stockTake.WarehouseSysId,
                            //        CurrentUserId = stockTake.CurrentUserId,
                            //        CurrentDisplayName = stockTake.CurrentDisplayName
                            //    });
                            //}
                        }

                        foreach (var request in requestList)
                        {
                            _frozenAppService.UnFrozenRequestForOther(request);
                        }
                        #endregion

                        stocktake.EndTime = DateTime.Now;
                        stocktake.Status = (int)StockTakeStatus.Finished;
                        stocktake.UpdateBy = stockTake.CurrentUserId;
                        stocktake.UpdateDate = DateTime.Now;
                        stocktake.UpdateUserName = stockTake.CurrentDisplayName;
                        _crudRepository.Update(stocktake);

                        #region 不修改明细状态，只修改单头状态
                        //var stockTakeDetails = _crudRepository.GetQuery<stocktakedetail>(x => x.StockTakeSysId == sysId);
                        //if(stockTakeDetails != null && stockTakeDetails.Count() > 0)
                        //{
                        //    foreach(var detail in stockTakeDetails)
                        //    {
                        //        detail.Status = (int)StockTakeDetailStatus.Finish;
                        //        detail.UpdateBy = stockTake.CurrentUserId;
                        //        detail.UpdateDate = DateTime.Now;
                        //        detail.UpdateUserName = stockTake.CurrentDisplayName;
                        //        _crudRepository.Update(detail);
                        //    }
                        //}
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        public void DeleteStockTake(List<Guid> sysIds, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var stockTake = _crudRepository.GetQuery<stocktake>(p => sysIds.Contains(p.SysId) && p.Status != (int)StockTakeStatus.New).FirstOrDefault();
            if (stockTake != null)
            {
                throw new Exception("只能删除未开始状态的盘点单");
            }

            foreach (var sysId in sysIds)
            {
                _crudRepository.Delete<stocktake>(sysId);
                _crudRepository.Delete<stocktakedetail>(p => p.StockTakeSysId == sysId);
            }
        }

        public Pages<StockTakeReportListDto> GetStockTakeReport(StockTakeReportQuery stockTakeReportQuery)
        {
            _crudRepository.ChangeDB(stockTakeReportQuery.WarehouseSysId);
            return _stockTakeRepository.GetStockTakeReport(stockTakeReportQuery);
        }

        /// <summary>
        /// 获取商品分类下拉框
        /// </summary>
        /// <returns></returns>
        public List<SelectItem> GetSelectSkuClass(SelectSkuClassDto selectSkuClassDto)
        {
            var skuclassList = _crudRepository.GetQuery<skuclass>(p => p.ParentSysId == selectSkuClassDto.ParentSysId).ToList();
            return skuclassList.Select(p => new SelectItem { Text = p.SkuClassName, Value = p.SysId.ToString() }).ToList();
        }

        /// <summary>
        /// 获取待盘点商品信息
        /// </summary>
        /// <param name="stockTakeSkuQuery"></param>
        /// <returns></returns>
        public Pages<StockTakeSkuListDto> GetWaitingStockTakeSkuList(StockTakeSkuQuery stockTakeSkuQuery)
        {
            _crudRepository.ChangeDB(stockTakeSkuQuery.WarehouseSysId);
            if (stockTakeSkuQuery.StockTakeType == (int)StockTakeType.Location)
            {
                return _stockTakeRepository.GetWaitingStockTakeSkuByLocation(stockTakeSkuQuery);
            }
            else if (stockTakeSkuQuery.StockTakeType == (int)StockTakeType.Sku)
            {
                return _stockTakeRepository.GetWaitingStockTakeSkuBySkuInfo(stockTakeSkuQuery);
            }
            else if (stockTakeSkuQuery.StockTakeType == (int)StockTakeType.Touch)
            {
                return _stockTakeRepository.GetWaitingStockTakeSkuByInvTrans(stockTakeSkuQuery);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="stockTakeDto"></param>
        /// <returns></returns>
        public void NewStockTake(NewStockTakeDto newStockTakeDto)
        {
            _crudRepository.ChangeDB(newStockTakeDto.WarehouseSysId);
            stocktake stocktake = new stocktake();
            stocktake.SysId = Guid.NewGuid();
            stocktake.StockTakeOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberStockTake);
            stocktake.Status = (int)StockTakeStatus.New;
            stocktake.StockTakeType = newStockTakeDto.StockTakeType;
            stocktake.WarehouseSysId = newStockTakeDto.WarehouseSysId;
            stocktake.AssignBy = newStockTakeDto.AssignBy;
            stocktake.AssignUserName = newStockTakeDto.AssignUserName;
            stocktake.CreateBy = newStockTakeDto.CurrentUserId;
            stocktake.CreateDate = DateTime.Now;
            stocktake.CreateUserName = newStockTakeDto.CurrentDisplayName;
            stocktake.UpdateBy = newStockTakeDto.CurrentUserId;
            stocktake.UpdateDate = DateTime.Now;
            stocktake.UpdateUserName = newStockTakeDto.CurrentDisplayName;

            List<stocktakedetail> stockTakeDetails = new List<stocktakedetail>();
            List<StockTakeSkuListDto> details = new List<StockTakeSkuListDto>();
            if (newStockTakeDto.StockTakeType == (int)StockTakeType.Location)
            {
                if (newStockTakeDto.ZoneSysId.HasValue && !newStockTakeDto.LocSysId.HasValue)
                {
                    stocktake.ZoneSysId = newStockTakeDto.ZoneSysId.Value;
                }
                details = _stockTakeRepository.GeStockTakeDetailsByLocation(newStockTakeDto);
            }
            else if (newStockTakeDto.StockTakeType == (int)StockTakeType.Sku)
            {
                details = _stockTakeRepository.GetStockTakeDetailsBySkuInfo(newStockTakeDto);
            }
            else if (newStockTakeDto.StockTakeType == (int)StockTakeType.Touch)
            {
                details = _stockTakeRepository.GeStockTakeDetailsByInvTrans(newStockTakeDto);
            }
            foreach (var detail in details)
            {
                stockTakeDetails.Add(new stocktakedetail
                {
                    SkuSysId = detail.SysId,
                    SysId = Guid.NewGuid(),
                    StockTakeSysId = stocktake.SysId,
                    Loc = detail.Loc,
                    Status = (int)StockTakeDetailStatus.New,
                    CreateBy = newStockTakeDto.CurrentUserId,
                    CreateDate = DateTime.Now,
                    CreateUserName = newStockTakeDto.CurrentDisplayName,
                    UpdateBy = newStockTakeDto.CurrentUserId,
                    UpdateDate = DateTime.Now,
                    UpdateUserName = newStockTakeDto.CurrentDisplayName
                });
            }

            if (!stockTakeDetails.Any())
            {
                throw new Exception("盘点单不包含任何盘点明细，无法创建单据");
            }

            _crudRepository.Insert(stocktake);
            _crudRepository.BatchInsert(stockTakeDetails);
        }

        /// <summary>
        /// 开始盘点
        /// </summary>
        /// <param name="stockTakeStartDto"></param>
        public void StockTakeStart(StockTakeStartDto stockTakeStartDto)
        {
            _crudRepository.ChangeDB(stockTakeStartDto.WarehouseSysId);
            var stockTake = _crudRepository.GetQuery<stocktake>(p => p.SysId == stockTakeStartDto.SysId).FirstOrDefault();
            if (stockTake == null)
            {
                throw new Exception("盘点单不存在");
            }
            if (stockTake.Status != (int)StockTakeStatus.New)
            {
                throw new Exception("只能操作未开始状态的盘点单");
            }
            var stockTakeDetails = _crudRepository.GetQuery<stocktakedetail>(p => p.StockTakeSysId == stockTakeStartDto.SysId).ToList();
            if (!stockTakeDetails.Any())
            {
                throw new Exception("盘点单没有盘点明细");
            }

            #region 冻结库存
            List<FrozenRequestDto> requestList = new List<FrozenRequestDto>();
            string frozeMemo = $"冻结库存，盘点单号[{stockTake.StockTakeOrder}]";
            if (stockTake.StockTakeType == (int)StockTakeType.Location)
            {
                if (stockTake.ZoneSysId.HasValue)
                {
                    requestList.Add(new FrozenRequestDto
                    {
                        Type = (int)FrozenType.Zone,
                        ZoneSysId = stockTake.ZoneSysId.Value,
                        Memo = frozeMemo,
                        WarehouseSysId = stockTakeStartDto.WarehouseSysId,
                        CurrentUserId = stockTakeStartDto.CurrentUserId,
                        CurrentDisplayName = stockTakeStartDto.CurrentDisplayName
                    });
                }
                else
                {
                    requestList.Add(new FrozenRequestDto
                    {
                        Type = (int)FrozenType.Location,
                        Loc = stockTakeDetails.FirstOrDefault().Loc,
                        Memo = frozeMemo,
                        WarehouseSysId = stockTakeStartDto.WarehouseSysId,
                        CurrentUserId = stockTakeStartDto.CurrentUserId,
                        CurrentDisplayName = stockTakeStartDto.CurrentDisplayName
                    });
                }
            }
            else if (stockTake.StockTakeType == (int)StockTakeType.Sku)
            {
                requestList.Add(new FrozenRequestDto
                {
                    Type = (int)FrozenType.Sku,
                    SkuList = stockTakeDetails.Select(p => p.SkuSysId).ToList(),
                    Memo = frozeMemo,
                    WarehouseSysId = stockTakeStartDto.WarehouseSysId,
                    CurrentUserId = stockTakeStartDto.CurrentUserId,
                    CurrentDisplayName = stockTakeStartDto.CurrentDisplayName
                });
            }
            //动碰盘点，按照库位、商品冻结库存
            else if (stockTake.StockTakeType == (int)StockTakeType.Touch)
            {
                requestList.Add(new FrozenRequestDto
                {
                    Type = (int)FrozenType.LocSku,
                    LocSkuList = stockTakeDetails.Select(p => new FrozenLocSkuDto { Loc = p.Loc, SkuSysId = p.SkuSysId }).Distinct().ToList(),
                    Memo = frozeMemo,
                    WarehouseSysId = stockTakeStartDto.WarehouseSysId,
                    CurrentUserId = stockTakeStartDto.CurrentUserId,
                    CurrentDisplayName = stockTakeStartDto.CurrentDisplayName
                });

                //var locs = stockTakeDetails.Select(p => p.Loc).Distinct();
                //foreach (var loc in locs)
                //{
                //    requestList.Add(new FrozenRequestDto
                //    {
                //        Type = (int)FrozenType.Location,
                //        Loc = loc,
                //        Memo = frozeMemo,
                //        WarehouseSysId = stockTakeStartDto.WarehouseSysId,
                //        CurrentUserId = stockTakeStartDto.CurrentUserId,
                //        CurrentDisplayName = stockTakeStartDto.CurrentDisplayName
                //    });
                //}
            }

            foreach (var request in requestList)
            {
                _frozenAppService.SaveFrozenRequestForOther(request);
            }
            #endregion

            #region 写入盘点明细财务库存数量
            var invQtyList = _wmsSqlRepository.GetInvQtyBySkuAndLoc(stockTakeDetails, stockTakeStartDto.WarehouseSysId);
            foreach (var stockTakeDetail in stockTakeDetails)
            {
                var invQtyInfo = invQtyList.FirstOrDefault(p => p.SkuSysId == stockTakeDetail.SkuSysId && p.Loc == stockTakeDetail.Loc);
                if (invQtyInfo != null)
                {
                    stockTakeDetail.Qty = invQtyInfo.Qty;
                    _crudRepository.Update(stockTakeDetail);
                }
            }
            #endregion

            stockTake.Status = (int)StockTakeStatus.Started;
            stockTake.StartTime = DateTime.Now;
            stockTake.UpdateBy = stockTakeStartDto.CurrentUserId;
            stockTake.UpdateDate = DateTime.Now;
            stockTake.UpdateUserName = stockTakeStartDto.CurrentDisplayName;
            _crudRepository.Update(stockTake);
        }
    }
}
