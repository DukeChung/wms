using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO.MQ.Log;
using Newtonsoft.Json;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.Utility.Enum.Log;
using NBK.ECService.WMS.Utility.RabbitMQ;
using NBK.ECService.WMS.Application.Check;
using System.Linq.Expressions;
using NBK.ECService.WMS.DTO.ThirdParty;

namespace NBK.ECService.WMS.Application
{
    public class AssemblyAppService : WMSApplicationService, IAssemblyAppService
    {
        private ICrudRepository _crudRepository = null;
        private IAssemblyRepository _assemblyRepository = null;
        private IWMSSqlRepository _wmsSqlRepository = null;
        private IPackageAppService _packageAppService = null;
        private IThirdPartyAppService _thirdPartyAppService = null;
        private IBaseAppService _baseAppService = null;

        public AssemblyAppService(ICrudRepository crudRepository, IAssemblyRepository assemblyRepository, IWMSSqlRepository wmsSqlRepository, IPackageAppService packageAppService, IThirdPartyAppService thirdPartyAppService, IBaseAppService baseAppService)
        {
            _crudRepository = crudRepository;
            _assemblyRepository = assemblyRepository;
            _wmsSqlRepository = wmsSqlRepository;
            _packageAppService = packageAppService;
            _thirdPartyAppService = thirdPartyAppService;
            this._baseAppService = baseAppService; 
        }

        public Pages<AssemblyListDto> GetAssemblyList(AssemblyQuery assemblyQuery)
        {
            _crudRepository.ChangeDB(assemblyQuery.WarehouseSysId);
            return _assemblyRepository.GetAssemblyList(assemblyQuery);
        }

        public AssemblyViewDto GetAssemblyViewDtoById(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var assemblyViewDto = _assemblyRepository.GetAssemblyViewDtoById(sysId);
            return assemblyViewDto;
        }

        /// <summary>
        /// 加工单领料
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="status"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentUserName"></param>
        /// <param name="warehouseSysId"></param>
        public void UpdateAssemblyStatus(Guid sysId, AssemblyStatus status, int currentUserId, string currentUserName, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var assembly = _crudRepository.Get<assembly>(sysId);
            if (assembly.Status != (int)AssemblyStatus.New)
            {
                throw new Exception(string.Format("加工单状态为{0},无法领料", ((AssemblyStatus)assembly.Status).ToDescription()));
            }

            #region 按照加工规则匹配数据
            var assemblyrule = _crudRepository.FirstOrDefault<assemblyrule>(x => x.WarehouseSysId == warehouseSysId);
            if (assemblyrule != null)
            {
                if (assemblyrule.Status)
                {
                    var assemblyDetail = _crudRepository.GetQuery<assemblydetail>(x => x.AssemblySysId == sysId).ToList();
                    foreach (var item in assemblyDetail)
                    {
                        var invlot = _crudRepository.GetQuery<invlot>(x => x.WareHouseSysId == warehouseSysId && x.SkuSysId == item.SkuSysId);
                        if (assemblyrule.MatchingLotAttr)
                        {
                            if (!string.IsNullOrEmpty(assembly.Channel))
                            {
                                invlot = invlot.Where(x => x.LotAttr01 == assembly.Channel);
                            }
                            else
                            {
                                invlot = invlot.Where(x => x.LotAttr01 == null || x.LotAttr01 == "");
                            }
                        }
                        if (invlot == null || invlot.Count() == 0)
                        {
                            throw new Exception("加工单中存在无库存记录的商品，无法领料");
                        }
                    }

                }
            }
            #endregion  

            assembly.Status = (int)status;
            assembly.ActualProcessingDate = DateTime.Now;
            assembly.UpdateBy = currentUserId;
            assembly.UpdateDate = DateTime.Now;
            assembly.UpdateUserName = currentUserName;
            _crudRepository.Update(assembly);

            #region 回调接口
            if (assembly.Source.Equals("ECC", StringComparison.OrdinalIgnoreCase))
            {
                var rsp = _thirdPartyAppService.WriteBackECCModifyAssemblyStatus(assembly, currentUserId, currentUserName);
                if (!rsp.IsSuccess)
                {
                    throw new Exception(rsp.ErrorMessage);
                }
            }

            #endregion
        }

        public void CancelAssemblyPicking(Guid sysId, int currentUserId, string currentUserName, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var businessLogDto = new BusinessLogDto();
            try
            {
                businessLogDto.access_log_sysId = AccessLogSysId.ToGuid();
                businessLogDto.request_json = JsonConvert.SerializeObject(new { AssemblySysId = sysId, CurrentUserId = currentUserId, CurrentUserName = currentUserName });
                businessLogDto.user_id = currentUserId.ToString();
                businessLogDto.user_name = currentUserName;
                businessLogDto.business_name = BusinessName.Assembly.ToDescription();
                businessLogDto.business_type = BusinessType.VAS.ToDescription();
                businessLogDto.business_operation = PublicConst.LogCancelReceive;
                businessLogDto.flag = true;
                businessLogDto.descr = "[old_json扣减库存, new_json 无记录]";

                var assembly = _crudRepository.Get<assembly>(sysId);
                if (assembly == null)
                {
                    throw new Exception("撤销领料失败,失败原因：未找到对应加工单");
                }
                if (assembly.Status != (int)AssemblyStatus.Assembling)
                {
                    throw new Exception(string.Format("加工单状态为{0},无法撤销领料", ((AssemblyStatus)assembly.Status).ToDescription()));
                }

                List<UpdateInventoryDto> updateInventoryDtos = new List<UpdateInventoryDto>();
                var pickDetails = _crudRepository.GetQuery<pickdetail>(x => x.OutboundSysId == assembly.SysId && x.WareHouseSysId == assembly.WareHouseSysId && x.Status != (int)PickDetailStatus.Cancel);
                if (pickDetails != null && pickDetails.Any())
                {
                    foreach (var item in pickDetails)
                    {
                        var pickDetail = _crudRepository.Get<pickdetail>(item.SysId);
                        if (pickDetail.Status != (int)PickDetailStatus.Finish)
                        {
                            throw new Exception(string.Format("拣货单状态为{0},无法撤销领料", ((PickDetailStatus)pickDetail.Status).ToDescription()));
                        }
                        //检查库存
                        invlot invLot = GetInvLot(pickDetail);
                        invskuloc invSkuLoc = GetInvSkuLoc(pickDetail);
                        invlotloclpn invLotLocLpn = GetInvLotLoLpn(pickDetail);
                        new InventoryCheck(invLot, invSkuLoc, invLotLocLpn, "撤销领料失败,失败原因：未找到对应分配库存").Execute();

                        updateInventoryDtos.Add(new UpdateInventoryDto()
                        {
                            InvLotLocLpnSysId = invLotLocLpn.SysId,
                            InvLotSysId = invLot.SysId,
                            InvSkuLocSysId = invSkuLoc.SysId,
                            Qty = (int)pickDetail.Qty,
                            CurrentUserId = currentUserId,
                            CurrentDisplayName = currentUserName,
                            WarehouseSysId = assembly.WareHouseSysId,
                        });
                        //更新加工单明细
                        var assemblyDetail = _crudRepository.Get<assemblydetail>(pickDetail.OutboundDetailSysId.ToGuid());
                        if (assemblyDetail != null)
                        {
                            assemblyDetail.PickedQty = 0;
                            assemblyDetail.Status = (int)AssemblyDetailStatus.New;
                            assemblyDetail.UpdateBy = currentUserId;
                            assemblyDetail.UpdateDate = DateTime.Now;
                            assemblyDetail.UpdateUserName = currentUserName;
                            _crudRepository.Update(assemblyDetail);
                        }
                        else
                        {
                            throw new Exception("撤销领料失败,失败原因：未找到对应加工单明细");
                        }
                        //更新拣货单
                        pickDetail.Status = (int)PickDetailStatus.Cancel;
                        pickDetail.UpdateBy = currentUserId;
                        pickDetail.UpdateDate = DateTime.Now;
                        pickDetail.UpdateUserName = currentUserName;
                        _crudRepository.Update(pickDetail);
                    }
                }
                //更新加工单
                assembly.Status = (int)AssemblyStatus.New;
                assembly.ActualProcessingDate = null;
                assembly.UpdateBy = currentUserId;
                assembly.UpdateDate = DateTime.Now;
                assembly.UpdateUserName = currentUserName;
                _crudRepository.Update(assembly);
                //更新拣货库存数量(撤销领料)
                if (updateInventoryDtos.Any())
                {
                    _wmsSqlRepository.UpdateInventoryCancelPickedQty(updateInventoryDtos);
                }
                businessLogDto.old_json = JsonConvert.SerializeObject(updateInventoryDtos);
            }
            catch (Exception ex)
            {
                businessLogDto.descr += ex.Message;
                businessLogDto.flag = false;
                throw ex;
            }
            finally
            {
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.BusinessLog, businessLogDto);
            }
        }

        /// <summary>
        /// 加工单完成
        /// </summary>
        /// <param name="assemblyFinishDto"></param>
        public void FinishAssemblyOrder(AssemblyFinishDto assemblyFinishDto)
        {
            _crudRepository.ChangeDB(assemblyFinishDto.WarehouseSysId);
            var businessLogDto = new BusinessLogDto();
            try
            {
                businessLogDto.access_log_sysId = AccessLogSysId.ToGuid();
                businessLogDto.request_json = JsonConvert.SerializeObject(assemblyFinishDto);
                businessLogDto.user_id = assemblyFinishDto.CurrentUserId.ToString();
                businessLogDto.user_name = assemblyFinishDto.CurrentDisplayName;
                businessLogDto.business_name = BusinessName.Assembly.ToDescription();
                businessLogDto.business_type = BusinessType.VAS.ToDescription();
                businessLogDto.business_operation = PublicConst.LogFinishAssembly;
                businessLogDto.flag = true;
                businessLogDto.descr = "[old_json扣减库存, new_json 无记录]";

                var assembly = _crudRepository.Get<assembly>(assemblyFinishDto.SysId);
                if (assembly == null)
                {
                    throw new Exception("完成加工单失败,失败原因：未找到对应加工单");
                }
                if (assembly.Status != (int)AssemblyStatus.Assembling)
                {
                    throw new Exception(string.Format("加工单状态为{0},无法完成加工单", ((AssemblyStatus)assembly.Status).ToDescription()));
                }
                var assemblyDetails = _crudRepository.GetQuery<assemblydetail>(p => p.AssemblySysId == assembly.SysId);
                if (assemblyDetails.Any(p => p.Status != (int)AssemblyDetailStatus.Picking))
                {
                    throw new Exception("完成加工单失败,失败原因：加工单明细未全部拣货完成");
                }
                var pickDetails = _crudRepository.GetQuery<pickdetail>(x => x.OutboundSysId == assembly.SysId && x.WareHouseSysId == assembly.WareHouseSysId && x.Status == (int)PickDetailStatus.Finish);
                if (pickDetails != null && pickDetails.Any())
                {
                    List<UpdateInventoryDto> updateInventoryDtos = new List<UpdateInventoryDto>();
                    foreach (var item in pickDetails)
                    {
                        var pickDetail = _crudRepository.Get<pickdetail>(item.SysId);
                        //检查库存
                        invlot invLot = GetInvLot(pickDetail, p => p.PickedQty >= item.Qty.Value);
                        invskuloc invSkuLoc = GetInvSkuLoc(pickDetail, p => p.PickedQty >= item.Qty.Value);
                        invlotloclpn invLotLocLpn = GetInvLotLoLpn(pickDetail, p => p.PickedQty >= item.Qty.Value);
                        new InventoryCheck(invLot, invSkuLoc, invLotLocLpn, "完成加工单失败,失败原因：未找到对应分配库存").Execute();

                        updateInventoryDtos.Add(new UpdateInventoryDto()
                        {
                            InvLotLocLpnSysId = invLotLocLpn.SysId,
                            InvLotSysId = invLot.SysId,
                            InvSkuLocSysId = invSkuLoc.SysId,
                            Qty = (int)pickDetail.Qty,
                            CurrentUserId = assemblyFinishDto.CurrentUserId,
                            CurrentDisplayName = assemblyFinishDto.CurrentDisplayName,
                            WarehouseSysId = assembly.WareHouseSysId,
                        });
                        //记录交易
                        RecordInvTrans(assemblyFinishDto, assembly, pickDetail, invLot);
                        //更新加工单明细
                        var assemblyDetail = _crudRepository.Get<assemblydetail>(pickDetail.OutboundDetailSysId.ToGuid());
                        if (assemblyDetail != null)
                        {
                            //LossQty单位转换
                            int lossQty = 0;
                            pack pack = new pack();
                            var assemblyFinishDetailDto = assemblyFinishDto.AssemblyDetails.FirstOrDefault(p => p.SkuSysId == assemblyDetail.SkuSysId);
                            if (_packageAppService.GetSkuConversiontransQty(assemblyDetail.SkuSysId, assemblyFinishDetailDto.LossQty, out lossQty, ref pack))
                            {
                                assemblyDetail.LossQty = lossQty;
                            }
                            else
                            {
                                assemblyDetail.LossQty = assemblyFinishDetailDto.LossQty;
                            }

                            assemblyDetail.PickedQty = 0;
                            assemblyDetail.Status = (int)AssemblyDetailStatus.Finished;
                            assemblyDetail.UpdateBy = assemblyFinishDto.CurrentUserId;
                            assemblyDetail.UpdateDate = DateTime.Now;
                            assemblyDetail.UpdateUserName = assemblyFinishDto.CurrentDisplayName;
                            _crudRepository.Update(assemblyDetail);
                        }
                        else
                        {
                            throw new Exception("完成加工单失败,失败原因：未找到对应加工单明细");
                        }
                    }

                    //更新加工单
                    assembly.Status = (int)AssemblyStatus.Finished;
                    assembly.ActualCompletionDate = DateTime.Now.Date;
                    assembly.ShelvesStatus = (int)ShelvesStatus.NotOnShelves;
                    assembly.ActualQty = assemblyFinishDto.ActualQty;
                    //设置成品sku批次
                    SetAssemblyLot(assembly);
                    assembly.UpdateBy = assemblyFinishDto.CurrentUserId;
                    assembly.UpdateDate = DateTime.Now;
                    assembly.UpdateUserName = assemblyFinishDto.CurrentDisplayName;
                    _crudRepository.Update(assembly);
                    //更新拣货库存，发生实际财务库存扣减
                    if (updateInventoryDtos.Any())
                    {
                        _wmsSqlRepository.UpdateInventoryQtyByPickedQty(updateInventoryDtos);
                    }
                    businessLogDto.old_json = JsonConvert.SerializeObject(updateInventoryDtos);
                }
                else
                {
                    throw new Exception("未找到拣货记录");
                }

                #region 回调接口
                var rsp = _thirdPartyAppService.WriteBackECCAssembly(assembly, assemblyFinishDto.CurrentUserId, assemblyFinishDto.CurrentDisplayName);
                if (!rsp.IsSuccess)
                {
                    throw new Exception(rsp.ErrorMessage);
                }
                #endregion
            }
            catch (Exception ex)
            {
                businessLogDto.descr += ex.Message;
                businessLogDto.flag = false;
                throw ex;
            }
            finally
            {
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.BusinessLog, businessLogDto);
            }
        }

        private void SetAssemblyLot(assembly assembly)
        {
            _crudRepository.ChangeDB(assembly.WareHouseSysId);


            assembly assemblyLot = null;
            if (!string.IsNullOrEmpty(assembly.Channel))
            {
                assemblyLot = _crudRepository.GetQuery<assembly>(p => p.SkuSysId == assembly.SkuSysId && p.WareHouseSysId == assembly.WareHouseSysId && p.ActualCompletionDate == assembly.ActualCompletionDate && p.Channel == assembly.Channel).FirstOrDefault();
            }
            else
            {
                assemblyLot = _crudRepository.GetQuery<assembly>(p => p.SkuSysId == assembly.SkuSysId && p.WareHouseSysId == assembly.WareHouseSysId && p.ActualCompletionDate == assembly.ActualCompletionDate && (p.Channel == "" || p.Channel == null)).FirstOrDefault();
            }
            if (assemblyLot != null)
            {
                assembly.Lot = assemblyLot.Lot;
            }
            else
            {
                //assembly.Lot = _crudRepository.GenNextNumber(PublicConst.GenNextNumberLot);
                assembly.Lot = _baseAppService.GetNumber(PublicConst.GenNextNumberLot);
            }
        }

        private void RecordInvTrans(AssemblyFinishDto assemblyFinishDto, assembly assembly, pickdetail pickDetail, invlot invLot)
        {
            _crudRepository.ChangeDB(assembly.WareHouseSysId);
            sku sku = _crudRepository.Get<sku>(pickDetail.SkuSysId);
            pack pack = _crudRepository.Get<pack>(sku.PackSysId);
            uom uom = _crudRepository.Get<uom>(pack.FieldUom01.GetValueOrDefault());
            invtran invtran = new invtran
            {
                SysId = Guid.NewGuid(),
                WareHouseSysId = assembly.WareHouseSysId,
                DocOrder = assembly.AssemblyOrder,
                DocSysId = assembly.SysId,
                DocDetailSysId = pickDetail.OutboundDetailSysId.ToGuid(),
                SkuSysId = pickDetail.SkuSysId,
                SkuCode = sku.SkuCode,
                TransType = InvTransType.Assembly,
                SourceTransType = InvSourceTransType.AssemblyPicking,
                Qty = -pickDetail.Qty.Value,
                Loc = pickDetail.Loc,
                Lot = pickDetail.Lot,
                Lpn = pickDetail.Lpn,
                ToLoc = pickDetail.Loc,
                ToLot = pickDetail.Lot,
                ToLpn = pickDetail.Lpn,
                Status = InvTransStatus.Ok,
                LotAttr01 = invLot.LotAttr01,
                LotAttr02 = invLot.LotAttr02,
                LotAttr03 = invLot.LotAttr03,
                LotAttr04 = invLot.LotAttr04,
                LotAttr05 = invLot.LotAttr05,
                LotAttr06 = invLot.LotAttr06,
                LotAttr07 = invLot.LotAttr07,
                LotAttr08 = invLot.LotAttr08,
                LotAttr09 = invLot.LotAttr09,
                ExternalLot = invLot.ExternalLot,
                ProduceDate = invLot.ProduceDate,
                ExpiryDate = invLot.ExpiryDate,
                ReceivedDate = invLot.ReceiptDate,
                PackSysId = sku.PackSysId,
                PackCode = pack != null ? pack.PackCode : string.Empty,
                UOMSysId = uom != null ? uom.SysId : Guid.Empty,
                UOMCode = uom != null ? uom.UOMCode : string.Empty,
                CreateBy = assemblyFinishDto.CurrentUserId,
                CreateDate = DateTime.Now,
                CreateUserName = assemblyFinishDto.CurrentDisplayName,
                UpdateBy = assemblyFinishDto.CurrentUserId,
                UpdateDate = DateTime.Now,
                UpdateUserName = assemblyFinishDto.CurrentDisplayName
            };
            _crudRepository.Insert(invtran);
        }

        private invlot GetInvLot(pickdetail pickDetail, Expression<Func<invlot, bool>> lambda = null)
        {
            _crudRepository.ChangeDB(pickDetail.WareHouseSysId);
            if (lambda == null) lambda = x => true;
            Expression<Func<invlot, bool>> whereLambda = x => x.SkuSysId == pickDetail.SkuSysId && x.Lot == pickDetail.Lot && x.WareHouseSysId == pickDetail.WareHouseSysId;
            return _crudRepository.GetQuery(whereLambda.And(lambda)).FirstOrDefault();
        }

        private invskuloc GetInvSkuLoc(pickdetail pickDetail, Expression<Func<invskuloc, bool>> lambda = null)
        {
            _crudRepository.ChangeDB(pickDetail.WareHouseSysId);
            if (lambda == null) lambda = x => true;
            Expression<Func<invskuloc, bool>> whereLambda = x => x.SkuSysId == pickDetail.SkuSysId && x.Loc == pickDetail.Loc && x.WareHouseSysId == pickDetail.WareHouseSysId;
            return _crudRepository.GetQuery(whereLambda.And(lambda)).FirstOrDefault();
        }

        private invlotloclpn GetInvLotLoLpn(pickdetail pickDetail, Expression<Func<invlotloclpn, bool>> lambda = null)
        {
            _crudRepository.ChangeDB(pickDetail.WareHouseSysId);
            if (lambda == null) lambda = x => true;
            Expression<Func<invlotloclpn, bool>> whereLambda = x => x.SkuSysId == pickDetail.SkuSysId && x.Lot == pickDetail.Lot && x.Loc == pickDetail.Loc && x.Lpn == pickDetail.Lpn && x.WareHouseSysId == pickDetail.WareHouseSysId;
            return _crudRepository.GetQuery(whereLambda.And(lambda)).FirstOrDefault();
        }

        /// <summary>
        /// 根据条件获取加工单
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public AssemblyViewDto GetAssemblyOrderByOrderId(AssemblyQuery assemblyQuery)
        {
            _crudRepository.ChangeDB(assemblyQuery.WarehouseSysId);
            var query =
                _assemblyRepository.GetQuery<assembly>(
                    x =>
                        x.AssemblyOrder == assemblyQuery.AssemblyOrderSearch &&
                        x.WareHouseSysId == assemblyQuery.WarehouseSysId);

            if (assemblyQuery.WaitPickSearch)
            {
                query =
                    query.Where(
                        x => x.Status == (int)AssemblyStatus.Assembling);
            }

            return query.FirstOrDefault().JTransformTo<AssemblyViewDto>();
        }

        public RFCommResult CheckAssemblyOrderNotOnShelves(string assemblyOrder, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            RFCommResult result = new RFCommResult { IsSucess = true };
            var assembly = _crudRepository.GetQuery<assembly>(p => p.AssemblyOrder == assemblyOrder
                && p.WareHouseSysId == warehouseSysId
                && p.Status == (int)AssemblyStatus.Finished
                && p.ShelvesStatus != (int)ShelvesStatus.Finish).FirstOrDefault();
            if (assembly == null)
            {
                result.IsSucess = false;
                result.Message = "待上架单据中不存在此单据号";
            }
            return result;
        }

        public Pages<AssemblySkuDto> GetSkuListForAssembly(AssemblySkuQuery query)
        {
            _assemblyRepository.ChangeDB(query.WarehouseSysId);

            return _assemblyRepository.GetSkuListForAssembly(query);
        }

        public void AddAssembly(AddAssemblyDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            assembly assembly = new assembly()
            {
                SysId = Guid.NewGuid(),
                AssemblyOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberAssembly),
                SkuSysId = request.SkuSysId,
                Status = (int)AssemblyStatus.New,
                PlanProcessingDate = request.PlanProcessingDate,
                PlanCompletionDate = request.PlanCompletionDate,
                Source = "WMS",
                Channel = "下行门店",
                ShelvesStatus = (int)ShelvesStatus.NotOnShelves,
                WareHouseSysId = request.WarehouseSysId,
                CreateBy = request.CurrentUserId,
                CreateDate = DateTime.Now,
                CreateUserName = request.CurrentDisplayName,
                UpdateBy = request.CurrentUserId,
                UpdateDate = DateTime.Now,
                UpdateUserName = request.CurrentDisplayName
            };

            var assemblydetailList = new List<assemblydetail>();
            request.AddAssemblyDetailList.ForEach(p =>
            {
                assemblydetailList.Add(new assemblydetail()
                {
                    SysId = Guid.NewGuid(),
                    AssemblySysId = assembly.SysId,
                    SkuSysId = p.SkuSysId,
                    UnitQty = p.UnitQty,
                    Qty = p.UnitQty,
                    Status = (int)AssemblyDetailStatus.New,
                    CreateBy = request.CurrentUserId,
                    CreateDate = DateTime.Now,
                    CreateUserName = request.CurrentDisplayName,
                    UpdateBy = request.CurrentUserId,
                    UpdateDate = DateTime.Now,
                    UpdateUserName = request.CurrentDisplayName
                });
            });

            _assemblyRepository.Insert(assembly);
            _assemblyRepository.BatchInsert(assemblydetailList);
        }

        public Pages<AssemblyWeightSkuDto> GetWeighSkuListForAssembly(AssemblyWeightSkuQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            return _assemblyRepository.GetWeighSkuListForAssembly(request);
        }

        public void SaveAssemblySkuWeight(AssemblyWeightSkuRequest request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);

            var list = _crudRepository.GetQuery<assemblyskuweight>(p => p.SkuSysId == request.SkuSysId && p.AssemblySysId == request.AssemblySysId && p.WarehouseSysId == request.WarehouseSysId);
            var unitQty = _crudRepository.GetQuery<assemblydetail>(p => p.SkuSysId == request.SkuSysId && p.AssemblySysId == request.AssemblySysId).First().UnitQty;
            if (list.Count() >= unitQty)
            {
                throw new Exception("称重次数超过需用数量，请检查!");
            }

            assemblyskuweight model = new assemblyskuweight()
            {
                SysId = Guid.NewGuid(),
                AssemblySysId = request.AssemblySysId,
                SkuSysId = request.SkuSysId,
                WarehouseSysId = request.WarehouseSysId,
                Weight = request.Weight,
                CreateBy = request.CurrentUserId,
                CreateDate = DateTime.Now,
                CreateUserName = request.CurrentDisplayName
            };

            _assemblyRepository.Insert(model);
        }
    }
}
