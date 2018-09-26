using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using Newtonsoft.Json;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO.ThirdParty;

namespace NBK.ECService.WMS.Application
{
    public class StockMovementAppService : WMSApplicationService, IStockMovementAppService
    {
        private ICrudRepository _crudRepository = null;
        private IStockMovementRepository _stockMovementRepository = null;
        private IPackageAppService _packageAppService = null;
        private IWMSSqlRepository _wmsSqlRepository = null;
        private IBaseAppService _baseAppService = null;
        private IThirdPartyAppService _thirdPartyAppService = null;

        public StockMovementAppService(ICrudRepository crudRepository, IStockMovementRepository stockMovementRepository, IPackageAppService packageAppService, IWMSSqlRepository wmsSqlRepository, IBaseAppService baseAppService, IThirdPartyAppService thirdPartyAppService)
        {
            this._crudRepository = crudRepository;
            this._stockMovementRepository = stockMovementRepository;
            _packageAppService = packageAppService;
            _wmsSqlRepository = wmsSqlRepository;
            this._baseAppService = baseAppService;
            _thirdPartyAppService = thirdPartyAppService;
        }

        /// <summary>
        /// 获取库存移动SKU列表
        /// </summary>
        /// <param name="stockMovementSkuQuery"></param>
        /// <returns></returns>
        public Pages<StockMovementSkuDto> GetStockMovementSkuList(StockMovementSkuQuery stockMovementSkuQuery)
        {
            _crudRepository.ChangeDB(stockMovementSkuQuery.WarehouseSysId);
            var response = _stockMovementRepository.GetStockMovementSkuListByPaging(stockMovementSkuQuery); 

            return response;
        }

        /// <summary>
        /// 获取库存移动信息
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="loc"></param>
        /// <param name="lot"></param>
        /// <returns></returns>
        public StockMovementDto GetStockMovement(Guid skuSysId, string loc, string lot, Guid wareHouseSysId)
        {
            _crudRepository.ChangeDB(wareHouseSysId);
            var response = _stockMovementRepository.GetStockMovement(skuSysId, loc, lot, wareHouseSysId);

            return response;
        }

        /// <summary>
        /// 保存调整
        /// </summary>
        /// <param name="stockMovementDto"></param>
        public void SaveStockMovement(StockMovementDto stockMovementDto)
        {
            _crudRepository.ChangeDB(stockMovementDto.WareHouseSysId);
            stockMovementDto.SysId = Guid.NewGuid();
            stockMovementDto.StockMovementOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberStockMovement);
            stockMovementDto.CreateDate = DateTime.Now;
            stockMovementDto.UpdateDate = DateTime.Now;
            if (stockMovementDto.FromLoc == stockMovementDto.ToLoc)
            {
                throw new Exception("目标货位不能和来源货位相同");
            }
            var sameStockMovement = _crudRepository.GetQuery<stockmovement>(p => p.SkuSysId == stockMovementDto.SkuSysId
                && p.FromLoc == stockMovementDto.FromLoc
                && p.ToLoc == stockMovementDto.ToLoc
                && p.Lot == stockMovementDto.Lot
                && p.WareHouseSysId == stockMovementDto.WareHouseSysId
                && p.Status == (int)StockMovementStatus.New).FirstOrDefault();
            if (sameStockMovement != null)
            {
                throw new Exception("已存在相同商品、货位和批次的变更单，请勿重复创建");
            }
            var invLotLocLpn = _crudRepository.FirstOrDefault<invlotloclpn>(p => p.SkuSysId == stockMovementDto.SkuSysId && p.Loc == stockMovementDto.FromLoc && p.Lot == stockMovementDto.Lot && p.WareHouseSysId == stockMovementDto.WareHouseSysId);
            int availableQty = invLotLocLpn.Qty - invLotLocLpn.AllocatedQty - invLotLocLpn.PickedQty;
            if (Convert.ToDecimal(stockMovementDto.FromQty) > availableQty)
            {
                throw new Exception("移动数量不能大于可用数量");
            }
            _crudRepository.InsertAndGetId(stockMovementDto.JTransformTo<stockmovement>());
        }

        /// <summary>
        /// 获取移动单列表
        /// </summary>
        /// <param name="stockMovementQuery"></param>
        /// <returns></returns>
        public Pages<StockMovementDto> GetStockMovementList(StockMovementQuery stockMovementQuery)
        {
            _crudRepository.ChangeDB(stockMovementQuery.WarehouseSysId);
            var response = _stockMovementRepository.GetStockMovementList(stockMovementQuery); 
             
            return response;
        }

        /// <summary>
        /// 确认移动
        /// </summary>
        /// <param name="sysIds"></param>
        public void ConfirmStockMovement(StockMovementOperationDto stockMovementOperationDto)
        {
            _crudRepository.ChangeDB(stockMovementOperationDto.WarehouseSysId);
            var stockMovementList = _crudRepository.GetQuery<stockmovement>(p => stockMovementOperationDto.SysIds.Contains(p.SysId)).ToList();
            if (stockMovementList.Any(p => p.Status != (int)StockMovementStatus.New))
            {
                throw new Exception(string.Format("以下移动单必须为新建状态：\r\n{0}", string.Join("\r\n", stockMovementList.Where(p => p.Status != (int)StockMovementStatus.New).Select(p => p.StockMovementOrder))));
            }
            if (stockMovementList.GroupBy(p => new { p.FromLoc, p.Lot }).Any(p => p.Count() > 1))
            {
                throw new Exception("请勿同时提交相同批次和来源货位的变更单");
            }

            int userId = stockMovementOperationDto.CurrentUserId;
            string displayName = stockMovementOperationDto.CurrentDisplayName;
            List<SkuPackUomDto> skuPackUomList = _stockMovementRepository.GetSkuPackUomList(stockMovementList.Select(p => p.SkuSysId));

            var selectLocs = stockMovementList.Select(p => p.FromLoc).ToList();
            selectLocs.AddRange(stockMovementList.Select(p => p.ToLoc).ToList());
            var locations = _crudRepository.GetQuery<location>(p => selectLocs.Contains(p.Loc) && p.WarehouseSysId == stockMovementOperationDto.WarehouseSysId).ToList();
            var skuList = stockMovementList.Select(p => p.SkuSysId);

            var skuInfoList = _crudRepository.GetQuery<sku>(p => skuList.Contains(p.SysId)).ToList();

            var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuList.Contains(p.SkuSysId.Value)
                             && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == stockMovementOperationDto.WarehouseSysId);
            if (frozenSkuList.Count() > 0)
            {
                var skuSysId = frozenSkuList.First().SkuSysId;
                var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).First();
                throw new Exception($"商品{frozenSku.SkuName}已被冻结，不能变更操作!");
            }

            var locskuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == stockMovementOperationDto.WarehouseSysId).ToList();

            //后边调用ECC 冻结接口使用
            var eccwarehouse = _crudRepository.GetQuery<warehouse>(p => p.SysId == stockMovementOperationDto.WarehouseSysId).FirstOrDefault();
            var lots = stockMovementList.Select(p => p.Lot).ToList();
            var lotList = _crudRepository.GetQuery<invlot>(p => lots.Contains(p.Lot) && p.WareHouseSysId == stockMovementOperationDto.WarehouseSysId).ToList();

            List<LockOrderInput> ecclist = new List<LockOrderInput>();

            stockMovementList.ForEach(p =>
            {
                if (locskuList.Count > 0)
                {
                    var firstFrozenLocsku = locskuList.FirstOrDefault(q => q.SkuSysId == p.SkuSysId && (q.Loc == p.FromLoc || q.Loc == p.ToLoc));
                    if (firstFrozenLocsku != null)
                    {
                        var frozenSku = _crudRepository.GetQuery<sku>(q => q.SysId == firstFrozenLocsku.SkuSysId).FirstOrDefault();
                        throw new Exception($"商品'{frozenSku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，不能变更操作!");
                    }
                }

                StockMovementDto stockMovementDto = _stockMovementRepository.GetStockMovement(p.SkuSysId, p.FromLoc, p.Lot, p.WareHouseSysId);
                if (Convert.ToDecimal(p.FromQty) > stockMovementDto.Qty)
                {
                    throw new Exception(string.Format("移动单[{0}]可用数量不足。", p.StockMovementOrder));
                }
                p.Status = (int)StockMovementStatus.Confirm;
                _crudRepository.Update<stockmovement>(p.SysId, x =>
                {
                    x.Status = (int)StockMovementStatus.Confirm;
                    x.TS = Guid.NewGuid();
                    x.UpdateBy = userId;
                    x.UpdateDate = DateTime.Now;
                    x.UpdateUserName = displayName;
                });

                var deepStockMovement = p.DeepClone<stockmovement>();

                //gavin : 单位转换
                int transFromQty = 0;
                int transToQty = 0;
                pack transFromPack = new pack();
                pack transToPack = new pack();
                if (_packageAppService.GetSkuConversiontransQty(deepStockMovement.SkuSysId, decimal.Parse(deepStockMovement.FromQty), out transFromQty, ref transFromPack) == true
                    && _packageAppService.GetSkuConversiontransQty(deepStockMovement.SkuSysId, decimal.Parse(deepStockMovement.ToQty), out transToQty, ref transToPack) == true)
                {
                    //gavin: 单位转换更新库存后需要记录
                    unitconversiontran unitTran = new unitconversiontran()
                    {
                        WareHouseSysId = deepStockMovement.WareHouseSysId,
                        DocOrder = deepStockMovement.StockMovementOrder,
                        DocSysId = deepStockMovement.SysId,
                        DocDetailSysId = deepStockMovement.SysId,
                        SkuSysId = deepStockMovement.SkuSysId,
                        FromQty = decimal.Parse(deepStockMovement.FromQty),
                        ToQty = transFromQty,
                        Loc = deepStockMovement.FromLoc,
                        Lot = "",
                        Lpn = "",
                        Status = "Done",
                        PackSysId = transFromPack.SysId,
                        PackCode = transFromPack.PackCode,
                        FromUOMSysId = transFromPack.FieldUom02 ?? Guid.Empty,
                        ToUOMSysId = transFromPack.FieldUom01 ?? Guid.Empty,
                        CreateBy = stockMovementOperationDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = stockMovementOperationDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        TransType = InvTransType.Adjustment,
                        SourceTransType = InvSourceTransType.Movement
                    };
                    _crudRepository.Insert(unitTran);

                    deepStockMovement.FromQty = transFromQty.ToString();

                    unitconversiontran unitTranTo = new unitconversiontran()
                    {
                        WareHouseSysId = deepStockMovement.WareHouseSysId,
                        DocOrder = deepStockMovement.StockMovementOrder,
                        DocSysId = deepStockMovement.SysId,
                        DocDetailSysId = deepStockMovement.SysId,
                        SkuSysId = deepStockMovement.SkuSysId,
                        FromQty = decimal.Parse(deepStockMovement.ToQty),
                        ToQty = transToQty,
                        Loc = deepStockMovement.ToLoc,
                        Lot = "",
                        Lpn = "",
                        Status = "Done",
                        PackSysId = transToPack.SysId,
                        PackCode = transToPack.PackCode,
                        FromUOMSysId = transToPack.FieldUom02 ?? Guid.Empty,
                        ToUOMSysId = transToPack.FieldUom01 ?? Guid.Empty,
                        CreateBy = stockMovementOperationDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = stockMovementOperationDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        TransType = InvTransType.Adjustment,
                        SourceTransType = InvSourceTransType.Movement
                    };
                    _crudRepository.Insert(unitTranTo);

                    deepStockMovement.ToQty = transToQty.ToString();
                }

                bool isFromFrozen = false;
                bool isToFrozen = false;

                if (locations.FirstOrDefault(q => q.Loc == deepStockMovement.FromLoc && q.Status == (int)LocationStatus.Frozen) != null)
                {
                    isFromFrozen = true;
                }

                if (locations.FirstOrDefault(q => q.Loc == deepStockMovement.ToLoc && q.Status == (int)LocationStatus.Frozen) != null)
                {
                    isToFrozen = true;
                }

                //更新invskuloc invlotloclpn
                _wmsSqlRepository.UpdateInventoryStockMovement(deepStockMovement, userId, displayName, isFromFrozen, isToFrozen);

                #region 存在冻结库存变动，则需要通知ECC
                if (isFromFrozen == true)
                {
                    var eccsku = skuInfoList.FirstOrDefault(q => q.SysId == p.SkuSysId);
                    if (eccsku != null)
                    {
                        var tempLot = lotList.First(q => q.Lot.Equals(p.Lot, StringComparison.OrdinalIgnoreCase));
                        ecclist.Add(new LockOrderInput() {
                            FreezeType = (int)FreezeTypeForECC.Unfreeze, //解冻
                            ProductCode = int.Parse(eccsku.OtherId),
                            Quantity = int.Parse(deepStockMovement.FromQty),
                            CreateUserId = stockMovementOperationDto.CurrentUserId,
                            WarehouseId = int.Parse(eccwarehouse.OtherId),
                            CreateUserName = stockMovementOperationDto.CurrentDisplayName,
                            ChannelTypeText = tempLot.LotAttr01
                        });
                    }
                }

                if (isToFrozen == true)
                {
                    var eccsku = skuInfoList.FirstOrDefault(q => q.SysId == p.SkuSysId);
                    if (eccsku != null)
                    {
                        var tempLot = lotList.First(q => q.Lot.Equals(p.Lot, StringComparison.OrdinalIgnoreCase));
                        ecclist.Add(new LockOrderInput()
                        {
                            FreezeType = (int)FreezeTypeForECC.Freeze, //冻结
                            ProductCode = int.Parse(eccsku.OtherId),
                            Quantity = int.Parse(deepStockMovement.ToQty),
                            CreateUserId = stockMovementOperationDto.CurrentUserId,
                            WarehouseId = int.Parse(eccwarehouse.OtherId),
                            CreateUserName = stockMovementOperationDto.CurrentDisplayName,
                            ChannelTypeText = tempLot.LotAttr01
                        });
                    }
                }
                #endregion

                //写入invtrans
                SkuPackUomDto skuPackUom = skuPackUomList.FirstOrDefault(x => x.SkuSysId == p.SkuSysId);
                InsertInvTrans(deepStockMovement, skuPackUom, userId, displayName);
            });

            if (ecclist.Count > 0)
            {
                _thirdPartyAppService.PushLockOrderToECCSync(ecclist, stockMovementOperationDto.CurrentUserId, stockMovementOperationDto.CurrentDisplayName);
            }
        }

        //private void UpdateInvSkuLoc(stockmovement stockMovement, int currentUserId, string currentDisplayName)
        //{
        //    invskuloc fromInvSkuLoc = _crudRepository.GetAll<invskuloc>().FirstOrDefault(x => x.SkuSysId == stockMovement.SkuSysId && x.Loc == stockMovement.FromLoc);
        //    invskuloc toInvSkuLoc = _crudRepository.GetAll<invskuloc>().FirstOrDefault(x => x.SkuSysId == stockMovement.SkuSysId && x.Loc == stockMovement.ToLoc);

        //    var fromLocation = _crudRepository.GetQuery<location>(p => p.Loc == fromInvSkuLoc.Loc && p.WarehouseSysId == fromInvSkuLoc.WareHouseSysId).FirstOrDefault();
        //    var toLocation = _crudRepository.GetQuery<location>(p => p.Loc == stockMovement.ToLoc && p.WarehouseSysId == stockMovement.WareHouseSysId).FirstOrDefault();

        //    if (fromInvSkuLoc != null)
        //    {
        //        fromInvSkuLoc.Qty = fromInvSkuLoc.Qty - Convert.ToInt32(stockMovement.FromQty);
        //        if (fromLocation == null)
        //        {
        //            throw new Exception($"货位{fromLocation.Loc}已经不存在，请重新创建!");
        //        }
        //        if (fromLocation.Status == (int)LocationStatus.Frozen)
        //        {
        //            fromInvSkuLoc.FrozenQty = fromInvSkuLoc.Qty - Convert.ToInt32(stockMovement.FromQty);
        //        }
        //        fromInvSkuLoc.UpdateBy = currentUserId;
        //        fromInvSkuLoc.UpdateDate = DateTime.Now;
        //        fromInvSkuLoc.UpdateUserName = currentDisplayName;

        //        _crudRepository.Update(fromInvSkuLoc);
        //    }
        //    if (toInvSkuLoc != null)
        //    {
        //        toInvSkuLoc.Qty = toInvSkuLoc.Qty + Convert.ToInt32(stockMovement.ToQty);
        //        if (toLocation == null)
        //        {
        //            throw new Exception($"货位{toLocation.Loc}已经不存在，请重新创建!");
        //        }
        //        if (toLocation.Status == (int)LocationStatus.Frozen)
        //        {
        //            toInvSkuLoc.FrozenQty = toInvSkuLoc.FrozenQty + Convert.ToInt32(stockMovement.ToQty);
        //        }
        //        toInvSkuLoc.UpdateBy = currentUserId;
        //        toInvSkuLoc.UpdateDate = DateTime.Now;
        //        toInvSkuLoc.UpdateUserName = currentDisplayName;

        //        _crudRepository.Update(toInvSkuLoc);
        //    }
        //    else
        //    {
        //        toInvSkuLoc = new invskuloc
        //        {
        //            SysId = Guid.NewGuid(),
        //            WareHouseSysId = stockMovement.WareHouseSysId,
        //            SkuSysId = stockMovement.SkuSysId,
        //            Loc = stockMovement.ToLoc,
        //            Qty = Convert.ToInt32(stockMovement.ToQty),
        //            CreateBy = currentUserId,
        //            CreateDate = DateTime.Now,
        //            CreateUserName = currentDisplayName,
        //            UpdateBy = currentUserId,
        //            UpdateDate = DateTime.Now,
        //            UpdateUserName = currentDisplayName
        //        };
        //        if (toLocation.Status == (int)LocationStatus.Frozen)
        //        {
        //            toInvSkuLoc.FrozenQty = Convert.ToInt32(stockMovement.ToQty);
        //        }
        //        _crudRepository.Insert(toInvSkuLoc);
        //    }
        //}

        //private void UpdateInvLotLocLpn(stockmovement stockMovement, int currentUserId, string currentDisplayName)
        //{
        //    invlotloclpn fromInvLotLocLpn = _crudRepository.GetAll<invlotloclpn>().FirstOrDefault(x => x.SkuSysId == stockMovement.SkuSysId && x.Lot == stockMovement.Lot && x.Loc == stockMovement.FromLoc);
        //    invlotloclpn toInvLotLocLpn = _crudRepository.GetAll<invlotloclpn>().FirstOrDefault(x => x.SkuSysId == stockMovement.SkuSysId && x.Lot == stockMovement.Lot && x.Loc == stockMovement.ToLoc);

        //    var fromLocation = _crudRepository.GetQuery<location>(p => p.Loc == fromInvLotLocLpn.Loc && p.WarehouseSysId == fromInvLotLocLpn.WareHouseSysId).FirstOrDefault();
        //    var toLocation = _crudRepository.GetQuery<location>(p => p.Loc == stockMovement.ToLoc && p.WarehouseSysId == stockMovement.WareHouseSysId).FirstOrDefault();

        //    if (fromInvLotLocLpn != null)
        //    {
        //        fromInvLotLocLpn.Qty = fromInvLotLocLpn.Qty - Convert.ToInt32(stockMovement.FromQty);

        //        if (fromLocation == null)
        //        {
        //            throw new Exception($"货位{fromLocation.Loc}已经不存在，请重新创建!");
        //        }
        //        if (fromLocation.Status == (int)LocationStatus.Frozen)
        //        {
        //            fromInvLotLocLpn.FrozenQty = fromInvLotLocLpn.FrozenQty - Convert.ToInt32(stockMovement.FromQty);
        //        }
        //        fromInvLotLocLpn.UpdateBy = currentUserId;
        //        fromInvLotLocLpn.UpdateDate = DateTime.Now;
        //        fromInvLotLocLpn.UpdateUserName = currentDisplayName;

        //        _crudRepository.Update(fromInvLotLocLpn);
        //    }
        //    if (toInvLotLocLpn != null)
        //    {
        //        toInvLotLocLpn.Qty = toInvLotLocLpn.Qty + Convert.ToInt32(stockMovement.ToQty);

        //        if (toLocation == null)
        //        {
        //            throw new Exception($"货位{toLocation.Loc}已经不存在，请重新创建!");
        //        }
        //        if (toLocation.Status == (int)LocationStatus.Frozen)
        //        {
        //            toInvLotLocLpn.FrozenQty = toInvLotLocLpn.FrozenQty + Convert.ToInt32(stockMovement.ToQty);
        //        }

        //        toInvLotLocLpn.UpdateBy = currentUserId;
        //        toInvLotLocLpn.UpdateDate = DateTime.Now;
        //        toInvLotLocLpn.UpdateUserName = currentDisplayName;

        //        _crudRepository.Update(toInvLotLocLpn);
        //    }
        //    else
        //    {
        //        toInvLotLocLpn = new invlotloclpn
        //        {
        //            SysId = Guid.NewGuid(),
        //            WareHouseSysId = stockMovement.WareHouseSysId,
        //            SkuSysId = stockMovement.SkuSysId,
        //            Loc = stockMovement.ToLoc,
        //            Lot = stockMovement.Lot,
        //            Lpn = string.Empty,
        //            Qty = Convert.ToInt32(stockMovement.ToQty),
        //            Status = 1,
        //            CreateBy = currentUserId,
        //            CreateDate = DateTime.Now,
        //            CreateUserName = currentDisplayName,
        //            UpdateBy = currentUserId,
        //            UpdateDate = DateTime.Now,
        //            UpdateUserName = currentDisplayName
        //        };
        //        if (toLocation.Status == (int)LocationStatus.Frozen)
        //        {
        //            toInvLotLocLpn.FrozenQty = Convert.ToInt32(stockMovement.ToQty);
        //        }

        //        _crudRepository.Insert(toInvLotLocLpn);
        //    }
        //}

        private void InsertInvTrans(stockmovement stockMovement, SkuPackUomDto skuPackUom, int currentUserId, string currentDisplayName)
        {
            _crudRepository.ChangeDB(stockMovement.WareHouseSysId);
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
                CreateBy = currentUserId,
                CreateDate = DateTime.Now,
                CreateUserName = currentDisplayName,
                UpdateBy = currentUserId,
                UpdateDate = DateTime.Now,
                UpdateUserName = currentDisplayName
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
            _crudRepository.Insert(fromInvTran);
            _crudRepository.Insert(toInvTran);
        }

        /// <summary>
        /// 取消移动
        /// </summary>
        /// <param name="sysIds"></param>
        public void CancelStockMovement(StockMovementOperationDto stockMovementOperationDto)
        {
            _crudRepository.ChangeDB(stockMovementOperationDto.WarehouseSysId);
            var stockMovementList = _crudRepository.GetQuery<stockmovement>(p => stockMovementOperationDto.SysIds.Contains(p.SysId)).ToList();
            if (stockMovementList.Any(p => p.Status != (int)StockMovementStatus.New))
            {
                throw new Exception(string.Format("以下移动单必须为新建状态：\r\n{0}", string.Join("\r\n", stockMovementList.Where(p => p.Status != (int)StockMovementStatus.New).Select(p => p.StockMovementOrder))));
            }
            stockMovementList.ForEach(p =>
            {
                p.Status = (int)StockMovementStatus.Cancel;
                _crudRepository.Update<stockmovement>(p.SysId, x => x.Status = (int)StockMovementStatus.Cancel);
            });
        }

        /// <summary>
        /// 导入库位变更
        /// </summary>
        /// <param name=")"></param>
        public void ImportStockMovementList(ImportStockMovement dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            var skus = (from p in dto.StockMovementImportDto group p by p.OtherId into g select g.Key).ToList();
            var locs = (from p in dto.StockMovementImportDto group p by p.ToLoc into g select g.Key).ToList();
            var skuList = _crudRepository.GetQuery<sku>(x => skus.Contains(x.OtherId)).ToList();

            if (skuList == null)
            {
                throw new Exception("所导入的商品全部不存在");
            }
            var ids = skuList.Select(x => x.SysId).ToList();
            var packIds = skuList.Select(x => x.PackSysId).ToList();

            //获取所有商品批次库存
            var skuLotLpn = _crudRepository.GetAllList<invlotloclpn>(p => ids.Contains(p.SkuSysId) && p.WareHouseSysId == dto.WarehouseSysId).ToList();
            var locList = _crudRepository.GetAllList<location>(p => locs.Contains(p.Loc) && p.WarehouseSysId == dto.WarehouseSysId).ToList();

            //获取所有包装信息
            var packList = _crudRepository.GetAllList<pack>(x => packIds.Contains(x.SysId)).ToList();

            var insertList = new List<stockmovement>();
            foreach (var item in dto.StockMovementImportDto)
            {
                var detailSku = skuList.Find(x => x.OtherId == item.OtherId && x.UPC == item.UPC);
                if (detailSku == null)
                {
                    throw new Exception("商品外部Id:" + item.OtherId + "且UPC:" + item.UPC + "不存在");
                }

                var locInfo = skuLotLpn.Where(x => x.SkuSysId == detailSku.SysId && x.Loc == item.FromLoc).OrderByDescending(x => x.Qty).ToList();
                if (locInfo == null || locInfo.Count == 0)
                {
                    throw new Exception("商品外部Id:" + item.OtherId + "来源货位:" + item.FromLoc + "的商品货位批次不存在");
                }
                var locLpnQty = locInfo.Sum(x => x.Qty) - locInfo.Sum(x => x.AllocatedQty) - locInfo.Sum(x => x.PickedQty) - locInfo.Sum(x => x.FrozenQty);

                if (locLpnQty < item.ChangerQty)
                {
                    throw new Exception("商品外部Id:" + item.OtherId + "来源货位:" + item.FromLoc + "的商品货位数量不够");
                }
                var loc = locList.Find(x => x.Loc == item.ToLoc);
                if (loc == null)
                {
                    throw new Exception("商品外部Id:" + item.OtherId + " 目标库位为：" + item.ToLoc + "不存在");
                }

                var packInfo = packList.Find(x => x.SysId == detailSku.PackSysId);
                if (packInfo == null)
                {
                    throw new Exception("商品外部Id:" + item.OtherId + " 且UPC：" + item.UPC + "的包装信息不存在");
                }
                var fromQty = item.ChangerQty;
                //如果是原材料，将单位转到最小单位：g
                if (packInfo.InLabelUnit01.HasValue && packInfo.InLabelUnit01.Value == true)
                {
                    if (packInfo.FieldValue01 > 0 && packInfo.FieldValue02 > 0)
                    {
                        fromQty = ((packInfo.FieldValue01.Value * fromQty) / packInfo.FieldValue02.Value);
                    }
                }
                foreach (var locItem in locInfo)
                {
                    if (fromQty == 0)
                    {
                        break;
                    }
                    var itemQty = (locItem.Qty - locItem.AllocatedQty - locItem.PickedQty - locItem.FrozenQty);
                    var qty = 0m;
                    if (itemQty >= fromQty)
                    {
                        qty = fromQty;
                        fromQty = 0;
                    }
                    else
                    {
                        fromQty = fromQty - itemQty;
                        qty = itemQty;
                    }


                    //如果是原材料，将单位转到最小单位的：g 转到1Kg
                    if (packInfo.InLabelUnit01.HasValue && packInfo.InLabelUnit01.Value == true)
                    {
                        if (packInfo.FieldValue01 > 0 && packInfo.FieldValue02 > 0)
                        {
                            qty = Math.Round(((packInfo.FieldValue02.Value * qty * 1.00m) / packInfo.FieldValue01.Value), 3);
                        }
                    }

                    var isexist = insertList.Where(x => x.SkuSysId == detailSku.SysId && x.FromLoc == item.FromLoc && x.ToLoc == item.ToLoc && x.Lot == locItem.Lot).ToList();

                    if (isexist != null && isexist.Count > 0)
                    {
                        throw new Exception("外部Id:" + item.OtherId + ",来源货位：" + item.FromLoc + ",目标货位：" + item.ToLoc + "的商品存在重复，请检查");
                    }
                    var model = new stockmovement();
                    model.SysId = Guid.NewGuid();
                    //model.StockMovementOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberStockMovement);
                    model.CreateDate = model.UpdateDate = DateTime.Now;
                    model.CreateBy = model.UpdateBy = dto.CurrentUserId;
                    model.CreateUserName = model.UpdateUserName = dto.CurrentDisplayName;

                    model.SkuSysId = detailSku.SysId;
                    model.Lot = locItem.Lot;
                    model.FromLoc = item.FromLoc;
                    model.ToLoc = item.ToLoc;
                    model.FromQty = model.ToQty = qty.ToString();
                    model.Status = (int)StockMovementStatus.New;
                    model.WareHouseSysId = dto.WarehouseSysId;
                    insertList.Add(model);
                }
            }

            #region 生成单号重新赋值
            var orderList = _baseAppService.GetNumber(PublicConst.GenNextNumberStockMovement, insertList.Count);
            for (var i = 0; i < insertList.Count; i++)
            {
                insertList[i].StockMovementOrder = orderList[i];
            }
            #endregion

            _crudRepository.BatchInsert(insertList);

        }
    }
}
