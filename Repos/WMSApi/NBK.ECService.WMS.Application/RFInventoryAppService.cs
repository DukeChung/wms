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
using NBK.ECService.WMS.Utility.Enum;
using Newtonsoft.Json;
using Abp.Domain.Uow;
using System.Transactions;
using NBK.ECService.WMS.DTO.ThirdParty;

namespace NBK.ECService.WMS.Application
{
    public class RFInventoryAppService : WMSApplicationService, IRFInventoryAppService
    {
        private IRFInventoryRepository _rfInventoryRepository = null;
        private IWMSSqlRepository _wmsSqlRepository = null;
        private IBaseAppService _baseAppService = null;
        private IStockMovementRepository _stockMovementRepository = null;
        private IStockMovementAppService _stockMovementAppService = null;
        private IThirdPartyAppService _thirdPartyAppService = null;

        public RFInventoryAppService(IRFInventoryRepository rfInventoryRepository, IWMSSqlRepository wmsSqlRepository, IBaseAppService baseAppService, IStockMovementRepository stockMovementRepository, IStockMovementAppService stockMovementAppService, IThirdPartyAppService thirdPartyAppService)
        {
            _rfInventoryRepository = rfInventoryRepository;
            _wmsSqlRepository = wmsSqlRepository;
            this._baseAppService = baseAppService;
            this._stockMovementRepository = stockMovementRepository;
            this._stockMovementAppService = stockMovementAppService;
            _thirdPartyAppService = thirdPartyAppService;
        }

        /// <summary>
        /// RF库存查询
        /// </summary>
        /// <param name="invSkuLocQuery"></param>
        /// <returns></returns>
        public List<RFInvSkuLocListDto> GetInvSkuLocList(RFInvSkuLocQuery invSkuLocQuery)
        {
            _rfInventoryRepository.ChangeDB(invSkuLocQuery.WarehouseSysId);
            return _rfInventoryRepository.GetInvSkuLocList(invSkuLocQuery);
        }

        #region RF库位变更
        /// <summary>
        /// RF库位变更
        /// </summary>
        /// <param name="rfStockMovementDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public RFCommResult StockMovement(RFStockMovementDto rfStockMovementDto)
        {
            _rfInventoryRepository.ChangeDB(rfStockMovementDto.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                #region 验证货位
                rfStockMovementDto.FromLoc = rfStockMovementDto.FromLoc.Trim();
                rfStockMovementDto.ToLoc = rfStockMovementDto.ToLoc.Trim();

                if (rfStockMovementDto.FromLoc == rfStockMovementDto.ToLoc)
                {
                    throw new Exception("目标货位不能和来源货位相同");
                }

                var locs = new List<string>() { rfStockMovementDto.FromLoc, rfStockMovementDto.ToLoc };
                var locationList = _rfInventoryRepository.GetQuery<location>(x => locs.Contains(x.Loc) && x.WarehouseSysId == rfStockMovementDto.WarehouseSysId).ToList();
                if (locationList == null)
                {
                    throw new Exception("来源货位和目标货位不存在");
                }

                var fromLoc = locationList.FirstOrDefault(x => x.Loc == rfStockMovementDto.FromLoc);
                if (fromLoc == null)
                {
                    throw new Exception("来源货位不存在");
                }

                var toLoc = locationList.FirstOrDefault(x => x.Loc == rfStockMovementDto.ToLoc);
                if (fromLoc == null)
                {
                    throw new Exception("目标货位不存在");
                }
                #endregion

                #region 商品 包装
                var sku = _rfInventoryRepository.Get<sku>(rfStockMovementDto.SkuSysId);
                if(sku == null)
                {
                    throw new Exception("商品不存在");
                }

                var packInfo = _rfInventoryRepository.Get<pack>(sku.PackSysId);
                #endregion

                #region 验证商品是否被冻结
                var frozenSkuList = _rfInventoryRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && p.SkuSysId == sku.SysId
                 && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == rfStockMovementDto.WarehouseSysId);
                if (frozenSkuList != null && frozenSkuList.Count() > 0)
                {
                    throw new Exception($"商品{sku.SkuName}已被冻结，不能变更操作!");
                }
                #endregion

                #region 验证商品货位是否被冻结
                var fromLocSkuList = _rfInventoryRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && p.SkuSysId == sku.SysId && p.Loc == rfStockMovementDto.FromLoc
                 && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == rfStockMovementDto.WarehouseSysId);
                if (fromLocSkuList != null && fromLocSkuList.Count() > 0)
                {
                    throw new Exception($"商品'{sku.SkuName}'在来源货位'{rfStockMovementDto.FromLoc}'已被冻结，不能变更操作!");
                }

                var toLocSkuList = _rfInventoryRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && p.SkuSysId == sku.SysId && p.Loc == rfStockMovementDto.ToLoc
                 && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == rfStockMovementDto.WarehouseSysId);
                if (toLocSkuList != null && toLocSkuList.Count() > 0)
                {
                    throw new Exception($"商品'{sku.SkuName}'在目标货位'{rfStockMovementDto.ToLoc}'已被冻结，不能变更操作!");
                }
                #endregion

                #region  单位转换为最小单位
                var fromQty = rfStockMovementDto.InputQty;
                if (packInfo.InLabelUnit01.HasValue && packInfo.InLabelUnit01.Value == true)
                {
                    if (packInfo.FieldValue01 > 0 && packInfo.FieldValue02 > 0)
                    {
                        fromQty = ((packInfo.FieldValue01.Value * fromQty) / packInfo.FieldValue02.Value);
                    }
                }
                #endregion

                var residualQty = (int)fromQty;
                if (residualQty <= 0)
                {
                    throw new Exception("数量必须大于0");
                }

                //库位变更数据
                var stockMovementList = new List<stockmovement>();
                var updateInvStockMovementList = new List<stockmovement>();

                var invLotLocLpnList = _rfInventoryRepository.GetQuery<invlotloclpn>(x => x.WareHouseSysId == rfStockMovementDto.WarehouseSysId && x.SkuSysId == sku.SysId && x.Loc == fromLoc.Loc && x.Qty > 0).ToList();
                if (invLotLocLpnList != null && invLotLocLpnList.Count > 0)
                {
                    #region 验证批次库存是否存在
                    if (!string.IsNullOrEmpty(rfStockMovementDto.FromLot))
                    {
                        invLotLocLpnList = invLotLocLpnList.Where(p => p.Lot.Equals(rfStockMovementDto.FromLot, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (!invLotLocLpnList.Any())
                        {
                            throw new Exception("来源批次没有库存");
                        }
                    }
                    #endregion

                    foreach (var info in invLotLocLpnList)
                    {
                        #region  计算库存数量
                        //当前可用数量
                        var currentQty = CommonBussinessMethod.GetAvailableQty(info.Qty, info.AllocatedQty, info.PickedQty, 0);
                        if (currentQty == 0)
                        {
                            continue;
                        }
                        //扣减数量
                        var deductionQty = residualQty <= currentQty ? residualQty : currentQty;
                        //计算剩余数量
                        residualQty = residualQty - deductionQty;
                        #endregion

                        #region 组织库位变更数据
                        decimal qty = deductionQty;
                        if (packInfo.InLabelUnit01.HasValue && packInfo.InLabelUnit01.Value == true)
                        {
                            if (packInfo.FieldValue01 > 0 && packInfo.FieldValue02 > 0)
                            {
                                qty = Math.Round(((packInfo.FieldValue02.Value * deductionQty * 1.00m) / packInfo.FieldValue01.Value), 3);
                            }
                        }

                        var sm = new stockmovement()
                        {
                            SysId = Guid.NewGuid(),
                            StockMovementOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberStockMovement),
                            WareHouseSysId = info.WareHouseSysId,
                            Status = (int)StockMovementStatus.Confirm,
                            SkuSysId = info.SkuSysId,
                            Lot = info.Lot,
                            Lpn = info.Lpn,
                            FromLoc = fromLoc.Loc,
                            ToLoc = toLoc.Loc,
                            FromQty = qty.ToString(),
                            ToQty = qty.ToString(),
                            CreateBy = rfStockMovementDto.CurrentUserId,
                            CreateUserName = rfStockMovementDto.CurrentDisplayName,
                            CreateDate = DateTime.Now,
                            UpdateBy = rfStockMovementDto.CurrentUserId,
                            UpdateUserName = rfStockMovementDto.CurrentDisplayName,
                            UpdateDate = DateTime.Now
                        };

                        var updateSm = sm.JTransformTo<stockmovement>();
                        updateSm.FromQty = deductionQty.ToString();
                        updateSm.ToQty = deductionQty.ToString();

                        stockMovementList.Add(sm);
                        updateInvStockMovementList.Add(updateSm);
                        #endregion

                        if (residualQty == 0)
                        {
                            break;
                        }
                    }

                    if (residualQty > 0)
                    {
                        throw new Exception("库存不足,无法进行变更！");
                    }
                }
                else
                {
                    throw new Exception("来源货位没有库存");
                }

                SkuPackUomDto skuPackUom = _stockMovementRepository.GetSkuPackUomList(stockMovementList.Select(p => p.SkuSysId)).FirstOrDefault();
                bool isFromFrozen = false;
                bool isToFrozen = false;

                if (fromLoc.Status == (int)LocationStatus.Frozen)
                {
                    isFromFrozen = true;
                }

                if (toLoc.Status == (int)LocationStatus.Frozen)
                {
                    isToFrozen = true;
                }

                //后边调用ECC 冻结接口使用
                var eccwarehouse = _stockMovementRepository.GetQuery<warehouse>(p => p.SysId == rfStockMovementDto.WarehouseSysId).FirstOrDefault();
                var lots = stockMovementList.Select(p => p.Lot).ToList();
                var lotList = _stockMovementRepository.GetQuery<invlot>(p => lots.Contains(p.Lot) && p.WareHouseSysId == rfStockMovementDto.WarehouseSysId).ToList();
                List<LockOrderInput> ecclist = new List<LockOrderInput>();
                #region 存在冻结库存变动，则需要通知ECC
                if (isFromFrozen == true)
                {
                    updateInvStockMovementList.ForEach(p => {
                        var tempLot = lotList.First(q => q.Lot.Equals(p.Lot, StringComparison.OrdinalIgnoreCase));
                        ecclist.Add(new LockOrderInput()
                        {
                            FreezeType = (int)FreezeTypeForECC.Unfreeze, //解冻
                            ProductCode = int.Parse(sku.OtherId),
                            Quantity = int.Parse(p.FromQty),
                            CreateUserId = rfStockMovementDto.CurrentUserId,
                            WarehouseId = int.Parse(eccwarehouse.OtherId),
                            CreateUserName = rfStockMovementDto.CurrentDisplayName,
                            ChannelTypeText = tempLot.LotAttr01
                        });
                    });
                }

                if (isToFrozen == true)
                {
                    updateInvStockMovementList.ForEach(p => {
                        var tempLot = lotList.First(q => q.Lot.Equals(p.Lot, StringComparison.OrdinalIgnoreCase));
                        ecclist.Add(new LockOrderInput()
                        {
                            FreezeType = (int)FreezeTypeForECC.Freeze, //冻结
                            ProductCode = int.Parse(sku.OtherId),
                            Quantity = int.Parse(p.ToQty),
                            CreateUserId = rfStockMovementDto.CurrentUserId,
                            WarehouseId = int.Parse(eccwarehouse.OtherId),
                            CreateUserName = rfStockMovementDto.CurrentDisplayName,
                            ChannelTypeText = tempLot.LotAttr01
                        });
                    });
                }

                StockMovementSaveChange(updateInvStockMovementList, stockMovementList, rfStockMovementDto, skuPackUom, isFromFrozen, isToFrozen, ecclist);
                
                #endregion

                result.IsSucess = true;
                result.Message = "库位变更成功!";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        [UnitOfWork(isTransactional: false)]
        private void StockMovementSaveChange(List<stockmovement> updateInvStockMovementList, List<stockmovement> stockMovementList, RFStockMovementDto rfStockMovementDto, SkuPackUomDto skuPackUom, bool isFromFrozen, bool isToFrozen, List<LockOrderInput> ecclist)
        {
            try
            {
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    //插入库位变更数据
                    _rfInventoryRepository.BatchInsert(stockMovementList);
                    foreach (var info in updateInvStockMovementList)
                    {
                        //更新invskuloc invlotloclpn
                        _wmsSqlRepository.UpdateInventoryStockMovement(info, rfStockMovementDto.CurrentUserId, rfStockMovementDto.CurrentDisplayName, isFromFrozen, isToFrozen);
                        //增加交易
                        InsertInvTrans(info, skuPackUom);
                    }
                    _rfInventoryRepository.SaveChange();

                    if (ecclist.Count > 0)
                    {
                        _thirdPartyAppService.PushLockOrderToECCSync(ecclist, rfStockMovementDto.CurrentUserId, rfStockMovementDto.CurrentDisplayName);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 增加库位变更交易
        /// </summary>
        /// <param name="stockMovement"></param>
        /// <param name="skuPackUom"></param>
        private void InsertInvTrans(stockmovement stockMovement, SkuPackUomDto skuPackUom)
        {
            invtran fromInvTran = new invtran
            {
                WareHouseSysId = stockMovement.WareHouseSysId,
                DocOrder = stockMovement.StockMovementOrder,
                DocSysId = stockMovement.SysId,
                DocDetailSysId = Guid.Empty,
                SkuSysId = stockMovement.SkuSysId,
                SkuCode = skuPackUom.SkuCode,
                TransType = InvTransType.Adjustment,
                SourceTransType = InvSourceTransType.Movement,
                Lot = stockMovement.Lot,
                Lpn = stockMovement.Lpn,
                ToLot = stockMovement.Lot,
                ToLpn = stockMovement.Lpn,
                Status = InvTransStatus.Ok,
                PackSysId = skuPackUom.PackSysId ?? Guid.Empty,
                PackCode = skuPackUom.PackCode,
                UOMSysId = skuPackUom.UOMSysId ?? Guid.Empty,
                UOMCode = skuPackUom.UOMCode,
                CreateBy = stockMovement.CreateBy,
                CreateDate = DateTime.Now,
                CreateUserName = stockMovement.CreateUserName,
                UpdateBy = stockMovement.UpdateBy,
                UpdateDate = DateTime.Now,
                UpdateUserName = stockMovement.UpdateUserName
            };
            invtran toInvTran = JsonConvert.DeserializeObject<invtran>(JsonConvert.SerializeObject(fromInvTran));
            fromInvTran.SysId = Guid.NewGuid();
            fromInvTran.Qty = -(int)Convert.ToDecimal(stockMovement.ToQty);
            fromInvTran.Loc = stockMovement.FromLoc;
            fromInvTran.ToLoc = stockMovement.FromLoc;
            toInvTran.SysId = Guid.NewGuid();
            toInvTran.Qty = (int)Convert.ToDecimal(stockMovement.ToQty);
            toInvTran.Loc = stockMovement.ToLoc;
            toInvTran.ToLoc = stockMovement.ToLoc;
            _rfInventoryRepository.Insert(fromInvTran);
            _rfInventoryRepository.Insert(toInvTran);
        }
        #endregion
    }
}
