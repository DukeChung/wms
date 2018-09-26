using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.InvLotLocLpn;
using NBK.ECService.WMS.DTO.ThirdParty;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class FrozenAppService : IFrozenAppService
    {
        private IFrozenRepository _crudRepository = null;
        private IInventoryRepository _inventoryRepository = null;
        private IWMSSqlRepository _wmsSqlRepository = null;
        private IThirdPartyAppService _thirdPartyAppService = null;

        public FrozenAppService(IFrozenRepository crudRepository, IInventoryRepository inventoryRepository, IWMSSqlRepository wmsSqlRepository, IThirdPartyAppService thirdPartyAppService)
        {
            this._crudRepository = crudRepository;
            _inventoryRepository = inventoryRepository;
            _wmsSqlRepository = wmsSqlRepository;
            _thirdPartyAppService = thirdPartyAppService;
        }

        public Pages<FrozenRequestSkuDto> GetFrozenRequestSkuByPage(FrozenRequestQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            return _crudRepository.GetFrozenRequestSkuByPage(request);
        }

        public Pages<FrozenRequestSkuDto> GetFrozenDetailByPage(FrozenRequestQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            return _crudRepository.GetFrozenDetailByPage(request);
        }

        /// <summary>
        /// 冻结请求
        /// </summary>
        /// <param name="request"></param>
        public void SaveFrozenRequest(FrozenRequestDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            if (request.Type == (int)FrozenType.Zone)
            {
                SaveFrozenRequestByZone(request, FrozenSource.FrozenBiz);
            }
            else if (request.Type == (int)FrozenType.Location)
            {
                SaveFrozenRequestByLocation(request, FrozenSource.FrozenBiz);
            }
            else if (request.Type == (int)FrozenType.Sku)
            {
                SaveFrozenRequestBySku(request, FrozenSource.FrozenBiz);
            }
            else if (request.Type == (int)FrozenType.LocSku)
            {
                SaveFrozenRequestByLocSku(request, FrozenSource.FrozenBiz);
            }
        }

        private void SaveFrozenRequestByZone(FrozenRequestDto request, FrozenSource frozenSource)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var existsStockFrozen = _crudRepository.GetQuery<stockfrozen>(p => p.ZoneSysId == request.ZoneSysId
                && p.WarehouseSysId == request.WarehouseSysId && p.Status == (int)FrozenStatus.Frozen).FirstOrDefault();
            if (existsStockFrozen != null)
            {
                if (string.IsNullOrEmpty(existsStockFrozen.Loc))
                {
                    throw new Exception("该储区已做冻结，请勿重复冻结!");
                }
                else
                {
                    throw new Exception($"该储区有货位({existsStockFrozen.Loc})已做冻结，请勿重复冻结!");
                }
            }

            var invList = _inventoryRepository.GetlotloclpnDtoByZone(request.ZoneSysId, request.WarehouseSysId);
            if (invList != null && invList.Count > 0)
            {
                var checkPickAllocatedInv = invList.FirstOrDefault(p => p.PickedQty > 0 || p.AllocatedQty > 0);
                if (checkPickAllocatedInv != null)
                {
                    throw new Exception("该储区下有库存正在拣货或装箱，无法冻结，请检查! ");
                }

                _wmsSqlRepository.UpdateInvForFrozenRequest(invList, request.CurrentUserId, request.CurrentDisplayName);
                if (frozenSource == FrozenSource.FrozenBiz)
                {
                    SendFrozenQtyToECC(invList, request);
                }
            }

            _wmsSqlRepository.BatchUpdateLocationStatusByZone(request.ZoneSysId, request.WarehouseSysId, LocationStatus.Frozen);

            stockfrozen stockfrozen = new stockfrozen();
            stockfrozen.ZoneSysId = request.ZoneSysId;
            stockfrozen.Type = request.Type;
            stockfrozen.Status = (int)FrozenStatus.Frozen;
            stockfrozen.WarehouseSysId = request.WarehouseSysId;
            stockfrozen.CreateBy = request.CurrentUserId;
            stockfrozen.CreateDate = DateTime.Now;
            stockfrozen.CreateUserName = request.CurrentDisplayName;
            stockfrozen.UpdateBy = request.CurrentUserId;
            stockfrozen.UpdateDate = DateTime.Now;
            stockfrozen.UpdateUserName = request.CurrentDisplayName;
            stockfrozen.Memo = request.Memo;
            stockfrozen.FrozenSource = (int)frozenSource;

            _crudRepository.Insert(stockfrozen);
        }

        private void SendFrozenQtyToECC(List<InvLotLocLpnDto> qtyList, FrozenRequestDto request)
        {
            if (qtyList != null && qtyList.Count > 0)
            {
                var warehouse = _crudRepository.GetQuery<warehouse>(p => p.SysId == request.WarehouseSysId).FirstOrDefault();

                List<LockOrderInput> ecclist = new List<LockOrderInput>();

                var skuSysIdList = qtyList.Select(p => p.SkuSysId).ToList();
                var skuList = _crudRepository.GetQuery<sku>(p => skuSysIdList.Contains(p.SysId)).ToList();
                qtyList.ForEach(p =>
                {
                    var sku = skuList.FirstOrDefault(q => q.SysId == p.SkuSysId);
                    if (sku != null && p.Qty > 0)
                    {
                        ecclist.Add(new LockOrderInput()
                        {
                            FreezeType = (int)FreezeTypeForECC.Freeze,
                            ProductCode = int.Parse(sku.OtherId),
                            Quantity = p.Qty,
                            CreateUserId = request.CurrentUserId,
                            WarehouseId = int.Parse(warehouse.OtherId),
                            CreateUserName = request.CurrentDisplayName,
                            ChannelTypeText = p.LotAttr01
                        });
                    }
                });

                _thirdPartyAppService.PushLockOrderToECCSync(ecclist, request.CurrentUserId, request.CurrentDisplayName);
            }
        }

        private void SendUnFrozenQtyToECC(List<InvLotLocLpnDto> qtyList, FrozenRequestDto request)
        {
            if (qtyList != null && qtyList.Count > 0)
            {
                var warehouse = _crudRepository.GetQuery<warehouse>(p => p.SysId == request.WarehouseSysId).FirstOrDefault();

                List<LockOrderInput> ecclist = new List<LockOrderInput>();

                var skuSysIdList = qtyList.Select(p => p.SkuSysId).ToList();
                var skuList = _crudRepository.GetQuery<sku>(p => skuSysIdList.Contains(p.SysId)).ToList();
                qtyList.ForEach(p =>
                {
                    var sku = skuList.FirstOrDefault(q => q.SysId == p.SkuSysId);
                    if (sku != null && p.FrozenQty > 0)
                    {
                        ecclist.Add(new LockOrderInput()
                        {
                            FreezeType = (int)FreezeTypeForECC.Unfreeze,
                            ProductCode = int.Parse(sku.OtherId),
                            Quantity = p.FrozenQty,
                            CreateUserId = request.CurrentUserId,
                            WarehouseId = int.Parse(warehouse.OtherId),
                            CreateUserName = request.CurrentDisplayName,
                            ChannelTypeText = p.LotAttr01
                        });
                    }
                });

                _thirdPartyAppService.PushLockOrderToECCSync(ecclist, request.CurrentUserId, request.CurrentDisplayName);
            }
        }

        private void SaveFrozenRequestByLocation(FrozenRequestDto request, FrozenSource frozenSource)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            if (string.IsNullOrEmpty(request.Loc))
            {
                throw new Exception("货位不能为空!");
            }

            var requestLocation = _crudRepository.GetQuery<location>(p => p.Loc == request.Loc && p.WarehouseSysId == request.WarehouseSysId).FirstOrDefault();
            if (requestLocation == null)
            {
                throw new Exception("货位不存在!");
            }
            if (requestLocation.ZoneSysId.HasValue)
                request.ZoneSysId = requestLocation.ZoneSysId.Value;

            var existsStockFrozen = _crudRepository.GetQuery<stockfrozen>(p => p.ZoneSysId == request.ZoneSysId && p.Loc == request.Loc
                && p.WarehouseSysId == request.WarehouseSysId && p.Type == (int)FrozenType.Location && p.Status == (int)FrozenStatus.Frozen).FirstOrDefault();
            if (existsStockFrozen != null)
            {
                throw new Exception($"货位'{request.Loc}'已做冻结，请勿重复冻结!");
            }

            var existsFrozenZone = _crudRepository.GetQuery<stockfrozen>(p => p.ZoneSysId == request.ZoneSysId && p.Loc == null 
                && p.WarehouseSysId == request.WarehouseSysId && p.Type == (int)FrozenType.Zone && p.Status == (int)FrozenStatus.Frozen).FirstOrDefault();
            if (existsFrozenZone != null)
            {
                throw new Exception($"冻结失败,货位'{request.Loc}'所在的储区已做冻结!");
            }

            var location = _crudRepository.GetQuery<location>(p => p.Loc == request.Loc && p.WarehouseSysId == request.WarehouseSysId && p.Status == (int)LocationStatus.Normal).FirstOrDefault();
            if (location == null)
            {
                throw new Exception($"货位'{request.Loc}'状态已发生变化，请检查! ");
            }

            var invList = _inventoryRepository.GetlotloclpnDtoByZoneLoc(request.ZoneSysId,request.Loc, request.WarehouseSysId);

            if (invList != null && invList.Count > 0)
            {
                var checkPickAllocatedInv = invList.FirstOrDefault(p => p.PickedQty > 0 || p.AllocatedQty > 0);
                if (checkPickAllocatedInv != null)
                {
                    throw new Exception($"货位'{request.Loc}'下有库存正在拣货或装箱，无法冻结，请检查! ");
                }

                _wmsSqlRepository.UpdateInvForFrozenRequest(invList, request.CurrentUserId, request.CurrentDisplayName);

                if (frozenSource == FrozenSource.FrozenBiz)
                    SendFrozenQtyToECC(invList, request);
            }

            
            location.Status = (int)LocationStatus.Frozen;
            location.TS = Guid.NewGuid();

            stockfrozen stockfrozen = new stockfrozen();
            stockfrozen.ZoneSysId = request.ZoneSysId;
            stockfrozen.Loc = request.Loc;
            stockfrozen.Type = request.Type;
            stockfrozen.Status = (int)FrozenStatus.Frozen;
            stockfrozen.WarehouseSysId = request.WarehouseSysId;
            stockfrozen.CreateBy = request.CurrentUserId;
            stockfrozen.CreateDate = DateTime.Now;
            stockfrozen.CreateUserName = request.CurrentDisplayName;
            stockfrozen.UpdateBy = request.CurrentUserId;
            stockfrozen.UpdateDate = DateTime.Now;
            stockfrozen.UpdateUserName = request.CurrentDisplayName;
            stockfrozen.Memo = request.Memo;
            stockfrozen.FrozenSource = (int)frozenSource;

            _crudRepository.Insert(stockfrozen);
            _crudRepository.Update(location);
        }

        private void SaveFrozenRequestBySku(FrozenRequestDto request, FrozenSource frozenSource)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            if (request.SkuList == null || request.SkuList.Count == 0)
            {
                throw new Exception("冻结商品信息缺失!");
            }

            request.SkuList = request.SkuList.Distinct().ToList();

            var filterList = _crudRepository.FilterUsedSkuList(request.SkuList, request.WarehouseSysId);
            if (filterList.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("以下商品在相关货位上已有库存分配，无法冻结!");
                filterList.ForEach(p => {
                    sb.Append($"{p.SkuName}__{p.Loc},");
                });
                throw new Exception(sb.ToString().Trim(','));
            }

            //商品冻结时， 获取需要更新的数据源，用于传给ECC做 占用
            var invList = _crudRepository.GetlotloclpnDtoByFrozenSku(request.SkuList, request.WarehouseSysId);

            //更新商品所有冻结库存
            _crudRepository.UpdateFrozenQtyBySku(request.SkuList, request.WarehouseSysId,request.CurrentUserId, request.CurrentDisplayName);

            if (frozenSource == FrozenSource.FrozenBiz)
                SendFrozenQtyToECC(invList, request);

            _crudRepository.BatchInsertStockFrozenBySku(request, frozenSource);
        }

        private void SaveFrozenRequestByLocSku(FrozenRequestDto request, FrozenSource frozenSource)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            if (request.LocSkuList == null || request.LocSkuList.Count == 0)
            {
                throw new Exception("冻结货位商品信息缺失!");
            }

            var skuLocList = _crudRepository.GetUsedLocSkuList(request.LocSkuList, request.WarehouseSysId);

            var filterList = skuLocList.Where(p => p.AllocatedQty > 0 || p.PickedQty > 0).Select(p => new { p.SkuName,p.Loc}).Distinct().ToList();
            if (filterList.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("以下商品在相关货位上已有库存分配，无法冻结!");
                filterList.ForEach(p => {
                    sb.Append($"{p.SkuName}__{p.Loc},");
                });
                throw new Exception(sb.ToString().Trim(','));
            }

            var skuList = skuLocList.Select(p => p.SkuSysId).Distinct();
            var locList = skuLocList.Select(p => p.Loc).Distinct();
            var zoneList = skuLocList.Select(p => p.ZoneSysId).Distinct();

            var frozenZoneList = _crudRepository.GetQuery<stockfrozen>(p => p.WarehouseSysId == request.WarehouseSysId
                && p.Type == (int)FrozenType.Zone && p.Status == (int)FrozenStatus.Frozen && zoneList.Contains(p.ZoneSysId));

            var frozenLocList = _crudRepository.GetQuery<stockfrozen>(p => p.WarehouseSysId == request.WarehouseSysId
                && p.Type == (int)FrozenType.Location && p.Status == (int)FrozenStatus.Frozen && locList.Contains(p.Loc));

            var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.WarehouseSysId == request.WarehouseSysId
                && p.Type == (int)FrozenType.Sku && p.Status == (int)FrozenStatus.Frozen && skuList.Contains(p.SkuSysId.Value));

            var frozenSkuLocList = _crudRepository.GetQuery<stockfrozen>(p => p.WarehouseSysId == request.WarehouseSysId
                && p.Type == (int)FrozenType.LocSku && p.Status == (int)FrozenStatus.Frozen);

            List<FrozenSkuDto> updateInventoryList = new List<FrozenSkuDto>();
            List<FrozenSkuDto> insertList = new List<FrozenSkuDto>();
            List<InvLotLocLpnDto> invList = new List<InvLotLocLpnDto>();

            foreach (var item in skuLocList)
            {
                if (frozenSkuLocList.Where(p => p.SkuSysId == item.SkuSysId && p.Loc == item.Loc).Count() > 0)
                {
                    continue;
                }

                //只要不存在重复的 库位商品级别的 冻结请求，就可以 insert stockfrozen
                if (!insertList.Exists(p => p.SkuSysId == item.SkuSysId && p.Loc == item.Loc))
                {
                    insertList.Add(item);
                }

                if (frozenLocList.Where(p => p.Loc == item.Loc).Count() > 0)
                {
                    continue;
                }

                if (frozenSkuList.Where(p => p.SkuSysId == item.SkuSysId).Count() > 0)
                {
                    continue;
                }

                if (frozenZoneList.Where(p => p.ZoneSysId == item.ZoneSysId).Count() > 0)
                {
                    continue;
                }

                //只有四种类型 都无冲突的 请求时，才能做库存更新
                updateInventoryList.Add(item);

                invList.Add(new InvLotLocLpnDto() {
                    SkuSysId = item.SkuSysId,
                    Qty = item.Qty,
                    LotAttr01 = item.LotAttr01
                });
            }

            //更新商品所有冻结库存
            _crudRepository.UpdateFrozenQtyByLocSku(updateInventoryList, request.WarehouseSysId, request.CurrentUserId, request.CurrentDisplayName);

            if (frozenSource == FrozenSource.FrozenBiz)
                SendFrozenQtyToECC(invList, request);

            //批量插入冻结请求
            _crudRepository.BatchInsertStockFrozenByLocSku(insertList, request.WarehouseSysId, request.CurrentUserId, request.CurrentDisplayName,request.Memo, frozenSource);
        }

        public Pages<FrozenListDto> GetFrozenRequestList(FrozenListQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            return _crudRepository.GetFrozenRequestList(request);
        }

        public void UnFrozenRequest(FrozenRequestDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var stockfrozen = _crudRepository.GetQuery<stockfrozen>(p => p.SysId == request.SysId).FirstOrDefault();
            if (stockfrozen == null)
            {
                throw new Exception("数据不存在，请检查!");
            }

            if (stockfrozen.Status != (int)FrozenStatus.Frozen)
            {
                throw new Exception("只有冻结状态的数据才能解冻，请检查!");
            }

            if (stockfrozen.FrozenSource == (int)FrozenSource.StockTake)
            {
                throw new Exception("盘点冻结不能手动解冻!");
            }

            if (stockfrozen.Type == (int)FrozenType.Zone)
            {
                UnFrozenRequestByZone(stockfrozen, request, FrozenSource.FrozenBiz);

                stockfrozen.Status = (int)FrozenStatus.UnFrozen;

                _crudRepository.Update(stockfrozen);
            }
            else if (stockfrozen.Type == (int)FrozenType.Location)
            {
                UnFrozenRequestByLocation(stockfrozen, request, FrozenSource.FrozenBiz);

                stockfrozen.Status = (int)FrozenStatus.UnFrozen;

                _crudRepository.Update(stockfrozen);
            }
            else if (stockfrozen.Type == (int)FrozenType.Sku)
            {
                request.SkuList = new List<Guid>();
                request.SkuList.Add(stockfrozen.SkuSysId.Value);
                UnFrozenRequestBySkuList(request, FrozenSource.FrozenBiz);
            }
            else if (stockfrozen.Type == (int)FrozenType.LocSku)
            {
                request.LocSkuList = new List<FrozenLocSkuDto>();
                request.LocSkuList.Add(new FrozenLocSkuDto() {
                    SkuSysId = stockfrozen.SkuSysId.Value,
                    Loc = stockfrozen.Loc
                });
                UnFrozenRequestByLocSkuList(request, FrozenSource.FrozenBiz);
            }
        }

        /// <summary>
        /// 根据储区解冻
        /// </summary>
        /// <param name="stockfrozen"></param>
        private void UnFrozenRequestByZone(stockfrozen stockfrozen, FrozenRequestDto request, FrozenSource frozenSource)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var invList = _inventoryRepository.GetlotloclpnDtoByZone(stockfrozen.ZoneSysId, stockfrozen.WarehouseSysId);
            if (invList != null && invList.Count > 0)
            {
                _crudRepository.UpdateInvForUnFrozenRequest(invList, request.CurrentUserId, request.CurrentDisplayName);

                if (frozenSource == FrozenSource.FrozenBiz)
                    SendUnFrozenQtyToECC(invList, request);
            }

            _wmsSqlRepository.BatchUpdateLocationStatusByZone(stockfrozen.ZoneSysId, stockfrozen.WarehouseSysId, LocationStatus.Normal);
        }

        /// <summary>
        /// 根据货位解冻
        /// </summary>
        /// <param name="stockfrozen"></param>
        private void UnFrozenRequestByLocation(stockfrozen stockfrozen, FrozenRequestDto request, FrozenSource frozenSource)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);

            var location = _crudRepository.GetQuery<location>(p => p.Loc == stockfrozen.Loc && p.WarehouseSysId == stockfrozen.WarehouseSysId && p.Status == (int)LocationStatus.Frozen).FirstOrDefault();
            if (location == null)
            {
                throw new Exception("该货位状态已发生变化，请检查! ");
            }

            var invList = _inventoryRepository.GetlotloclpnDtoByZoneLoc(stockfrozen.ZoneSysId, stockfrozen.Loc, stockfrozen.WarehouseSysId);
            if (invList != null && invList.Count > 0)
            {
                _crudRepository.UpdateInvForUnFrozenRequest(invList, request.CurrentUserId, request.CurrentDisplayName);

                if (frozenSource == FrozenSource.FrozenBiz)
                    SendUnFrozenQtyToECC(invList, request);
            }
            
            location.Status = (int)LocationStatus.Normal;
            location.TS = Guid.NewGuid();

            _crudRepository.Update(location);
        }

        /// <summary>
        /// 批量解冻商品
        /// </summary>
        /// <param name="stockfrozen"></param>
        /// <param name="request"></param>
        public void UnFrozenRequestBySkuList(FrozenRequestDto request, FrozenSource frozenSource)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            if (request.SkuList == null || request.SkuList.Count == 0)
            {
                throw new Exception("请求的商品列表信息不全! ");
            }

            //更新冻结记录表状态 stockfrozen
            _crudRepository.BatchUnFrozenSku(request.SkuList, request.WarehouseSysId);

            //更新相关冻结库存
            var invList = _crudRepository.GetBatchUpdateInventoryForUnFrozenSkuList(request.SkuList, request.WarehouseSysId, request.CurrentUserId, request.CurrentDisplayName);

            if (invList.Count > 0)
            {
                _crudRepository.UpdateInvForUnFrozenRequest(invList, request.CurrentUserId, request.CurrentDisplayName);

                if (frozenSource == FrozenSource.FrozenBiz)
                    SendUnFrozenQtyToECC(invList, request);
            }            
        }

        /// <summary>
        /// 批量解冻货位商品
        /// </summary>
        /// <param name="request"></param>
        public void UnFrozenRequestByLocSkuList(FrozenRequestDto request, FrozenSource frozenSource)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            if (request.LocSkuList == null || request.LocSkuList.Count == 0)
            {
                throw new Exception("请求的货位商品列表信息不全! ");
            }

            request.LocSkuList = request.LocSkuList.Distinct().ToList();

            var skuLocList = _crudRepository.GetUsedLocSkuList(request.LocSkuList, request.WarehouseSysId);

            var skuList = skuLocList.Select(p => p.SkuSysId).Distinct();
            var locList = skuLocList.Select(p => p.Loc).Distinct();
            var zoneList = skuLocList.Select(p => p.ZoneSysId).Distinct();

            var frozenZoneList = _crudRepository.GetQuery<stockfrozen>(p => p.WarehouseSysId == request.WarehouseSysId
                && p.Type == (int)FrozenType.Zone && p.Status == (int)FrozenStatus.Frozen && zoneList.Contains(p.ZoneSysId));

            var frozenLocList = _crudRepository.GetQuery<stockfrozen>(p => p.WarehouseSysId == request.WarehouseSysId
                && p.Type == (int)FrozenType.Location && p.Status == (int)FrozenStatus.Frozen && locList.Contains(p.Loc));

            var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.WarehouseSysId == request.WarehouseSysId
                && p.Type == (int)FrozenType.Sku && p.Status == (int)FrozenStatus.Frozen && skuList.Contains(p.SkuSysId.Value));

            var frozenSkuLocList = _crudRepository.GetQuery<stockfrozen>(p => p.WarehouseSysId == request.WarehouseSysId
                && p.Type == (int)FrozenType.LocSku && p.Status == (int)FrozenStatus.Frozen);

            List<FrozenSkuDto> updateInventoryList = new List<FrozenSkuDto>();
            List<FrozenSkuDto> updateList = new List<FrozenSkuDto>();
            List<InvLotLocLpnDto> invList = new List<InvLotLocLpnDto>();

            foreach (var item in skuLocList)
            {
                if (frozenSkuLocList.Where(p => p.SkuSysId == item.SkuSysId && p.Loc == item.Loc).Count() == 0)
                {//说明已经不存在 该货位商品 级别的冻结数据，可能已经被其他地方解冻，跳过处理
                    continue;
                }

                //只要不存在重复的 库位商品级别的 冻结请求，就 update stockfrozen 状态等
                if (!updateList.Exists(p => p.SkuSysId == item.SkuSysId && p.Loc == item.Loc))
                {
                    updateList.Add(item);
                }

                if (frozenLocList.Where(p => p.Loc == item.Loc).Count() > 0)
                {
                    continue;
                }

                if (frozenSkuList.Where(p => p.SkuSysId == item.SkuSysId).Count() > 0)
                {
                    continue;
                }

                if (frozenZoneList.Where(p => p.ZoneSysId == item.ZoneSysId).Count() > 0)
                {
                    continue;
                }

                //只有四种类型 都无冲突的 请求时，才能做库存更新
                updateInventoryList.Add(item);

                invList.Add(new InvLotLocLpnDto()
                {
                    SkuSysId = item.SkuSysId,
                    FrozenQty = item.Qty,
                    LotAttr01 = item.LotAttr01
                });
            }

            //更新商品所有冻结库存 （解冻）
            _crudRepository.UpdateUnFrozenQtyByLocSku(updateInventoryList, request.WarehouseSysId, request.CurrentUserId, request.CurrentDisplayName);

            if (frozenSource == FrozenSource.FrozenBiz)
                SendUnFrozenQtyToECC(invList, request);

            //批量更新冻结请求
            _crudRepository.BatchUpdateStockUnFrozenByLocSku(updateList, request.WarehouseSysId, request.CurrentUserId, request.CurrentDisplayName, request.Memo);
        }

        /// <summary>
        /// 其他系统使用冻结(盘点)
        /// </summary>
        /// <param name="request"></param>
        public void SaveFrozenRequestForOther(FrozenRequestDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            if (request.Type == (int)FrozenType.Zone)
            {
                SaveFrozenRequestByZone(request, FrozenSource.StockTake);
            }
            else if (request.Type == (int)FrozenType.Location)
            {
                SaveFrozenRequestByLocation(request, FrozenSource.StockTake);
            }
            else if (request.Type == (int)FrozenType.Sku)
            {
                SaveFrozenRequestBySku(request, FrozenSource.StockTake);
            }
            else if (request.Type == (int)FrozenType.LocSku)
            {
                SaveFrozenRequestByLocSku(request, FrozenSource.StockTake);
            }
        }

        /// <summary>
        /// 其他系统使用解冻(盘点)
        /// </summary>
        /// <param name="request"></param>
        public void UnFrozenRequestForOther(FrozenRequestDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            if (request.Type == (int)FrozenType.Sku)
            {
                UnFrozenRequestBySkuList(request, FrozenSource.StockTake);
            }
            else if (request.Type == (int)FrozenType.LocSku)
            {
                UnFrozenRequestByLocSkuList(request, FrozenSource.StockTake);
            }
            else if (request.Type == (int)FrozenType.Zone || request.Type == (int)FrozenType.Location)
            {
                stockfrozen stockfrozen = null;
                if (request.Type == (int)FrozenType.Zone)
                {
                    stockfrozen = _crudRepository.GetQuery<stockfrozen>(p => p.ZoneSysId == request.ZoneSysId && p.WarehouseSysId == request.WarehouseSysId
                        && p.Type == (int)FrozenType.Zone && p.Status == (int)FrozenStatus.Frozen).FirstOrDefault();

                    if (stockfrozen == null)
                    {
                        // 盘点操作，不需要抛异常，直接跳出即可
                        return;
                    }

                    UnFrozenRequestByZone(stockfrozen, request, FrozenSource.StockTake);

                    stockfrozen.Status = (int)FrozenStatus.UnFrozen;

                    _crudRepository.Update(stockfrozen);
                }
                else if (request.Type == (int)FrozenType.Location)
                {
                    stockfrozen = _crudRepository.GetQuery<stockfrozen>(p => p.Loc == request.Loc && p.WarehouseSysId == request.WarehouseSysId
                        && p.Type == (int)FrozenType.Location && p.Status == (int)FrozenStatus.Frozen).FirstOrDefault();

                    if (stockfrozen == null)
                    {
                        // 盘点操作，不需要抛异常，直接跳出即可
                        return;
                    }

                    UnFrozenRequestByLocation(stockfrozen, request, FrozenSource.StockTake);

                    stockfrozen.Status = (int)FrozenStatus.UnFrozen;

                    _crudRepository.Update(stockfrozen);
                }
            }
        }
    }
}
