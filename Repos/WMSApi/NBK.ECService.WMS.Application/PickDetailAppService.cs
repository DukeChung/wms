using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO.MQ.Log;
using NBK.ECService.WMS.Utility.Enum.Log;
using NBK.ECService.WMS.Utility.RabbitMQ;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;
using System.Transactions;
using Abp.Domain.Uow;
using NBK.ECService.WMS.DTO.InvLotLocLpn;
using NBK.ECService.WMS.Utility.Redis;

namespace NBK.ECService.WMS.Application
{
    public class PickDetailAppService : WMSApplicationService, IPickDetailAppService
    {
        private IPickDetailRepository _crudRepository = null;
        private IInventoryRepository _inventoryRepository = null;
        private IWMSSqlRepository _WMSSqlRepository = null;
        private IPackageAppService _packageAppService = null;
        private IBaseAppService _baseAppService = null;
        private IOutboundTransferOrderRepository _outboundTransferOrderRepository = null;
        private IRedisAppService _redisAppService = null;
        private IPackageRepository _packageRepository = null;

        public PickDetailAppService(IPickDetailRepository crudRepository, IInventoryRepository inventoryRepository, IWMSSqlRepository wmsSqlRepository, IPackageAppService packageAppService, IBaseAppService baseAppService, IOutboundTransferOrderRepository outboundTransferOrdeRepository, IRedisAppService redisAppService, IPackageRepository packageRepository)
        {
            this._crudRepository = crudRepository;
            this._inventoryRepository = inventoryRepository;
            this._WMSSqlRepository = wmsSqlRepository;
            this._packageAppService = packageAppService;
            this._baseAppService = baseAppService;
            this._outboundTransferOrderRepository = outboundTransferOrdeRepository;
            this._redisAppService = redisAppService;
            this._packageRepository = packageRepository;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        public Pages<PickDetailListDto> GetPickDetailListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            _crudRepository.ChangeDB(pickDetailQuery.WarehouseSysId);
            var response = _crudRepository.GetPickDetailListDtoByPageInfo(pickDetailQuery);
            if (response != null && response.TableResuls != null && response.TableResuls.aaData.Count > 0)
            {
                List<Guid?> pickDetailSysIds = response.TableResuls.aaData.Select(p => p.SysId).ToList();
                var pickDetailList = _crudRepository.GetSummaryPickDetailListDto(pickDetailSysIds);
                foreach (var item in response.TableResuls.aaData)
                {
                    var detail = pickDetailList.Find(x => x.PickDetailOrder == item.PickDetailOrder);
                    item.OutboundCount = detail.OutboundCount;
                }
            }

            return response;
        }

        /// <summary>
        /// 获取待拣货出库数据
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        public Pages<PickOutboundListDto> GetPickOutboundListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            _crudRepository.ChangeDB(pickDetailQuery.WarehouseSysId);
            var response = _crudRepository.GetPickOutboundListDtoByPageInfo(pickDetailQuery);
            if (response != null && response.TableResuls != null && response.TableResuls.aaData.Count > 0)
            {
                List<Guid?> outboundSysIds = response.TableResuls.aaData.Select(p => p.SysId).ToList();
                var outboundDetailList = _crudRepository.GetPickOutboundDetailListDto(outboundSysIds);
                foreach (var item in response.TableResuls.aaData)
                {
                    var detail = outboundDetailList.Find(x => x.OutboundSysId == item.SysId);
                    item.SkuTypeQty = detail.SkuTypeQty;
                }
            }
            return response;
        }

        /// <summary>
        /// 根据拣货规则生成单
        /// </summary>
        /// <param name="createPickDetailRuleDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<string> GeneratePickDetailByPickRule(CreatePickDetailRuleDto createPickDetailRuleDto)
        {
            _crudRepository.ChangeDB(createPickDetailRuleDto.WarehouseSysId);
            var outboundSysIdList = createPickDetailRuleDto.OutboundSysIdList.Select(q => Guid.Parse(q)).ToList();
            if (!outboundSysIdList.Any())
            {
                throw new Exception("没有找到指定的拣货的出库单");
            }

            var pickDetailOrderList = new List<string>();

            #region 生成拣货单号
            var genOrderList = new List<string>();
            if (createPickDetailRuleDto.PickType == PublicConst.PickTypeByOrder)
            {
                genOrderList = _baseAppService.GetNumber(PublicConst.GenNextNumberPickDetail, outboundSysIdList.Count());
            }
            else
            {
                genOrderList.Add(_baseAppService.GetNumber(PublicConst.GenNextNumberPickDetail));
            }
            #endregion

            pickDetailOrderList.AddRange(genOrderList);
            var outboundRule = _crudRepository.GetQuery<outboundrule>(x => x.WarehouseSysId == createPickDetailRuleDto.WarehouseSysId).FirstOrDefault();
            List<outbounddetail> outboundDetailListAll = _crudRepository.GetQuery<outbounddetail>(p => outboundSysIdList.Contains(p.OutboundSysId.Value)).ToList();
            var outboundList = _crudRepository.GetQuery<outbound>(p => outboundSysIdList.Contains(p.SysId)).ToList();
            //添加退货入库校验，退货入库的单子不能捡货
            if (outboundList != null)
            {
                var isReturnList = outboundList.Select(o => o.IsReturn).ToList();
                if (isReturnList != null)
                {
                    if (isReturnList.Contains((int)OutboundReturnStatus.B2CReturn))
                    {
                        throw new Exception("单据已进行退货操作!");
                    }
                }
            }
            var skuList = outboundDetailListAll.Select(x => x.SkuSysId).ToList();

            outboundRule.Status = true;
            if (createPickDetailRuleDto.PickRule == PublicConst.PickRuleLO)
            {
                outboundRule.DeliverySortRules = (int)DeliverySortRules.AfterProduceFirstOutbound;
            }
            else if (createPickDetailRuleDto.PickRule == PublicConst.PickRuleFO)
            {
                outboundRule.DeliverySortRules = (int)DeliverySortRules.FirstProduceFirstOutbound;
            }
            else
            {
                outboundRule.DeliverySortRules = (int)DeliverySortRules.FirstReceiptFirstOutbound;
            }

            var invLotLocLpnList = _inventoryRepository.GetlotloclpnBySkuSysIdOrderByLotDetail(skuList, createPickDetailRuleDto.WarehouseSysId, new outbound() { SysId = Guid.Empty }, outboundRule);
            var locList = invLotLocLpnList.Select(x => x.Loc).ToList();
            var locationList = _crudRepository.GetQuery<location>(p => p.Loc != null && p.WarehouseSysId == createPickDetailRuleDto.WarehouseSysId && locList.Contains(p.Loc)).ToList();
            var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == createPickDetailRuleDto.WarehouseSysId).ToList();
            var frozenLocSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuList.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == createPickDetailRuleDto.WarehouseSysId).ToList();

            var skuInfoList = _crudRepository.GetQuery<sku>(p => skuList.Contains(p.SysId)).ToList();

            //定义生成工单
            var workDetailList = new List<WorkDetailDto>();
            var workRule = _crudRepository.GetQuery<workrule>(x => x.WarehouseSysId == createPickDetailRuleDto.WarehouseSysId).FirstOrDefault();
            try
            {
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption)
                    )
                {
                    outboundSysIdList.ForEach(item =>
                    {
                        createPickDetailRuleDto.PickDetailOrder = genOrderList[0];
                        if (createPickDetailRuleDto.PickType == PublicConst.PickTypeByOrder)
                        {
                            genOrderList.RemoveAt(0);
                        }
                        var outbound = outboundList.FirstOrDefault(x => x.SysId == item);
                        PickDetailAllocatedQty(outbound, createPickDetailRuleDto, locationList, outboundRule,
                            outboundDetailListAll, invLotLocLpnList, frozenSkuList, skuInfoList, frozenLocSkuList);

                        #region 组织工单数据
                        if (outbound != null)
                        {
                            var workDetail = new WorkDetailDto()
                            {
                                Status = (int)WorkStatus.Working,
                                WorkType = (int)UserWorkType.Picking,
                                Priority = 1,
                                StartTime = DateTime.Now,
                                EndTime = DateTime.Now,
                                WorkTime = DateTime.Now,
                                Source = "拣货",
                                DocSysId = outbound.SysId,
                                DocOrder = outbound.OutboundOrder,
                                WarehouseSysId = createPickDetailRuleDto.WarehouseSysId,
                                CurrentUserId = createPickDetailRuleDto.CurrentUserId,
                                CurrentDisplayName = createPickDetailRuleDto.CurrentDisplayName
                            };
                            workDetailList.Add(workDetail);
                        }
                        #endregion
                    });
                    scope.Complete();
                }

                #region 组织推送工单到MQ
                if (workRule != null && workRule.Status == true && workRule.PickWork == true)
                {
                    if (workDetailList != null && workDetailList.Count > 0)
                    {
                        var mqWorkDto = new MQWorkDto()
                        {
                            WorkBusinessType = (int)WorkBusinessType.Insert,
                            WorkType = (int)UserWorkType.Picking,
                            WorkDetailDtoList = workDetailList,
                            WarehouseSysId = createPickDetailRuleDto.WarehouseSysId,
                            CurrentUserId = createPickDetailRuleDto.CurrentUserId,
                            CurrentDisplayName = createPickDetailRuleDto.CurrentDisplayName
                        };

                        var processDto = new MQProcessDto<MQWorkDto>()
                        {
                            BussinessSysId = new Guid(),
                            BussinessOrderNumber = "",
                            Descr = "",
                            CurrentUserId = createPickDetailRuleDto.CurrentUserId,
                            CurrentDisplayName = createPickDetailRuleDto.CurrentDisplayName,
                            WarehouseSysId = createPickDetailRuleDto.WarehouseSysId,
                            BussinessDto = mqWorkDto
                        };
                        //推送工单数据
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, processDto);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("系统拣货出现异常：" + ex.Message, ex);
            }
            finally
            {
                //scope.
            }
            return pickDetailOrderList;
        }

        /// <summary>
        /// 取消拣货
        /// </summary>
        /// <param name="pickDetailOrderList"></param>
        /// <returns></returns>
        public string CancelPickDetail(CancelPickDetailDto cancelPickDetailDto)
        {
            var businessLogDto = new BusinessLogDto();
            try
            {
                _crudRepository.ChangeDB(cancelPickDetailDto.WarehouseSysId);
                businessLogDto.access_log_sysId = Guid.NewGuid();
                businessLogDto.request_json = JsonConvert.SerializeObject(cancelPickDetailDto);
                businessLogDto.user_id = cancelPickDetailDto.CurrentUserId.ToString();
                businessLogDto.user_name = cancelPickDetailDto.CurrentDisplayName;
                businessLogDto.business_name = BusinessName.CancelPick.ToDescription();
                businessLogDto.business_type = BusinessType.Outbound.ToDescription();
                businessLogDto.business_operation = PublicConst.LogCancelPickDetail;
                businessLogDto.flag = true;
                businessLogDto.descr = "[old_json扣减库存, new_json 无记录]";

                var pickDetailOrderList = cancelPickDetailDto.PickDetailOrderList.Distinct().ToList();

                var docSysId = new List<Guid>();

                if (pickDetailOrderList.Any())
                {
                    var updateInventoryDtos = new List<UpdateInventoryDto>();
                    foreach (var pickDetailOrder in pickDetailOrderList)
                    {
                        var pickDetailList = _crudRepository.GetQuery<pickdetail>(x => x.PickDetailOrder == pickDetailOrder && x.WareHouseSysId == cancelPickDetailDto.WarehouseSysId);
                        if (pickDetailList.Any())
                        {
                            foreach (var info in pickDetailList)
                            {
                                var pickDetail = _crudRepository.Get<pickdetail>(info.SysId);
                                if (pickDetail != null)
                                {
                                    if (pickDetail.Status != (int)PickDetailStatus.New)
                                    {
                                        throw new Exception("同批拣货单号: " + pickDetail.PickDetailOrder + " 存在状态不为新建，无法取消拣货");
                                    }

                                    #region InvLot
                                    var invLot = _crudRepository.GetQuery<invlot>(x => x.SkuSysId == pickDetail.SkuSysId && x.Lot == pickDetail.Lot && x.WareHouseSysId == pickDetail.WareHouseSysId).FirstOrDefault();
                                    if (invLot != null)
                                    {
                                        //var pInvLot = _crudRepository.Get<invlot>(invLot.SysId);
                                        //if (pInvLot != null)
                                        //{
                                        //    pInvLot.AllocatedQty -= (int)pickDetail.Qty;
                                        //    pInvLot.UpdateDate = DateTime.Now;
                                        //    if (pInvLot.AllocatedQty < 0)
                                        //    {
                                        //        throw new Exception("取消拣货失败,失败原因：分配数量出现异常");
                                        //    }
                                        //    _crudRepository.Update(pInvLot);
                                        //}
                                    }
                                    else
                                    {
                                        //_crudRepository.SetOperationLog(OperationLogType.Abnormal, null, "取消拣货失败，InvLot表，SkuSysId:" + pickDetail.SkuSysId + "Lot:" + pickDetail.Lot + "未找到对应库存");
                                        throw new Exception("取消拣货失败,失败原因：未找到对应分配库存");
                                    }
                                    #endregion

                                    #region InvSkuLoc
                                    var invSkuLoc = _crudRepository.GetQuery<invskuloc>(x => x.SkuSysId == pickDetail.SkuSysId && x.Loc == pickDetail.Loc && x.WareHouseSysId == pickDetail.WareHouseSysId).FirstOrDefault();
                                    if (invSkuLoc != null)
                                    {
                                        //var pInvSkuLoc = _crudRepository.Get<invskuloc>(invSkuLoc.SysId);
                                        //if (pInvSkuLoc != null)
                                        //{
                                        //    pInvSkuLoc.AllocatedQty -= (int)pickDetail.Qty;
                                        //    pInvSkuLoc.UpdateDate = DateTime.Now;
                                        //    if (pInvSkuLoc.AllocatedQty < 0)
                                        //    {
                                        //        throw new Exception("取消拣货失败,失败原因：分配数量出现异常");
                                        //    }
                                        //    _crudRepository.Update(pInvSkuLoc);
                                        //}
                                    }
                                    else
                                    {
                                        //_crudRepository.SetOperationLog(OperationLogType.Abnormal, null, "取消拣货失败，InvSkuLoc表，SkuSysId:" + pickDetail.SkuSysId + "Loc:" + pickDetail.Loc + "未找到对应库存");
                                        throw new Exception("取消拣货失败,失败原因：未找到对应分配库存");
                                    }
                                    #endregion

                                    #region InvLotLocLpn
                                    var invLotLocLpn = _crudRepository.GetQuery<invlotloclpn>(x => x.SkuSysId == pickDetail.SkuSysId && x.Lot == pickDetail.Lot && x.Loc == pickDetail.Loc && x.Lpn == pickDetail.Lpn && x.WareHouseSysId == pickDetail.WareHouseSysId).FirstOrDefault();
                                    if (invLotLocLpn != null)
                                    {
                                        //var pInvLotLocLpn = _crudRepository.Get<invlotloclpn>(invLotLocLpn.SysId);
                                        //if (pInvLotLocLpn != null)
                                        //{
                                        //    pInvLotLocLpn.AllocatedQty -= (int)pickDetail.Qty;
                                        //    pInvLotLocLpn.UpdateDate = DateTime.Now;
                                        //    if (pInvLotLocLpn.AllocatedQty < 0)
                                        //    {
                                        //        throw new Exception("取消拣货失败,失败原因：分配数量出现异常");
                                        //    }
                                        //    _crudRepository.Update(pInvLotLocLpn);
                                        //}
                                    }
                                    else
                                    {
                                        //_crudRepository.SetOperationLog(OperationLogType.Abnormal, null, "取消拣货失败，InvLotLonLpn表，SkuSysId:" + pickDetail.SkuSysId + "Lot:" + pickDetail.Lot + "Loc:" + pickDetail.Loc + "Lpn:" + pickDetail.Lpn + "未找到对应库存");
                                        throw new Exception("取消拣货失败,失败原因：未找到对应分配库存");
                                    }
                                    #endregion

                                    updateInventoryDtos.Add(new UpdateInventoryDto()
                                    {
                                        InvLotLocLpnSysId = invLotLocLpn.SysId,
                                        InvLotSysId = invLot.SysId,
                                        InvSkuLocSysId = invSkuLoc.SysId,
                                        Qty = (int)pickDetail.Qty,
                                        CurrentUserId = cancelPickDetailDto.CurrentUserId,
                                        CurrentDisplayName = cancelPickDetailDto.CurrentDisplayName,
                                        WarehouseSysId = cancelPickDetailDto.WarehouseSysId,
                                    });

                                    #region OutboundDetail
                                    var outboundDetail = _crudRepository.Get<outbounddetail>(pickDetail.OutboundDetailSysId.ToGuid());
                                    if (outboundDetail != null)
                                    {
                                        outboundDetail.AllocatedQty = 0;
                                        outboundDetail.Status = (int)OutboundDetailStatus.New;
                                        outboundDetail.UpdateDate = DateTime.Now;
                                        outboundDetail.UpdateBy = cancelPickDetailDto.CurrentUserId;
                                        outboundDetail.UpdateUserName = cancelPickDetailDto.CurrentDisplayName;
                                        _crudRepository.Update(outboundDetail);
                                    }
                                    else
                                    {
                                        //_crudRepository.SetOperationLog(OperationLogType.Abnormal, null, "取消拣货失败，OutboundDetail表，SysId:" + pickDetail.OutboundDetailSysId + "未找到对应出库单明细");
                                        throw new Exception("取消拣货失败,失败原因：未找到对应出库单明细");
                                    }
                                    #endregion

                                    #region Outbound
                                    var outbound = _crudRepository.Get<outbound>(pickDetail.OutboundSysId.ToGuid());
                                    if (outbound != null)
                                    {
                                        outbound.TotalAllocatedQty = 0;
                                        outbound.Status = (int)OutboundStatus.New;
                                        outbound.UpdateDate = DateTime.Now;
                                        outbound.UpdateBy = cancelPickDetailDto.CurrentUserId;
                                        outbound.UpdateUserName = cancelPickDetailDto.CurrentDisplayName;
                                        //非新建状态的出库单做过B2C退货入库的在取消捡货的时候修改出库单状态为关闭
                                        if (outbound.IsReturn == (int)OutboundReturnStatus.B2CReturn)
                                        {
                                            outbound.Status = (int)OutboundStatus.Close;
                                        }
                                        _crudRepository.Update(outbound);

                                        if (!docSysId.Contains(outbound.SysId))
                                        {
                                            docSysId.Add(outbound.SysId);
                                        }
                                    }
                                    else
                                    {
                                        //_crudRepository.SetOperationLog(OperationLogType.Abnormal, null, "取消拣货失败，Outbound表，SysId:" + pickDetail.OutboundSysId + "未找到对应出库单");
                                        throw new Exception("取消拣货失败,失败原因：未找到对应出库单");
                                    }
                                    #endregion

                                    #region PickDetail
                                    pickDetail.Status = (int)PickDetailStatus.Cancel;
                                    pickDetail.UpdateDate = DateTime.Now;
                                    pickDetail.UpdateBy = cancelPickDetailDto.CurrentUserId;
                                    pickDetail.UpdateUserName = cancelPickDetailDto.CurrentDisplayName;
                                    _crudRepository.Update(pickDetail);
                                    #endregion
                                }
                                else
                                {
                                    //_crudRepository.SetOperationLog(OperationLogType.Abnormal, null, "取消拣货失败，PickDetail表，SysId:" + info.SysId + "未找到对应拣货明细");
                                    throw new Exception("拣货单号：" + pickDetailOrder + "未找到拣货记录");
                                }

                            }
                        }
                        else
                        {
                            throw new Exception("拣货单号：" + pickDetailOrder + "未找到拣货记录");
                        }

                    }
                    //执行扣减库存方法(取消拣货)
                    _WMSSqlRepository.UpdateInventoryCancelAllocatedQty(updateInventoryDtos);
                    businessLogDto.old_json = JsonConvert.SerializeObject(updateInventoryDtos);
                }
                else
                {
                    throw new Exception("没有需要取消的拣货记录");
                }

                //还原拣货容器状态
                if (docSysId.Count > 0)
                {
                    foreach (var sysId in docSysId)
                    {
                        var outboundOrder = _crudRepository.GetQuery<outbound>(p => p.SysId == sysId).FirstOrDefault();
                        var containerSysIds = _crudRepository.GetQuery<prebulkpack>(p => p.OutboundSysId == sysId && p.Status == (int)PreBulkPackStatus.RFPicking).Select(p => p.SysId).ToList();
                        _WMSSqlRepository.ClearContainer(new ClearContainerDto
                        {
                            ContainerSysIds = containerSysIds,
                            WarehouseSysId = cancelPickDetailDto.WarehouseSysId,
                            CurrentUserId = cancelPickDetailDto.CurrentUserId,
                            CurrentDisplayName = cancelPickDetailDto.CurrentDisplayName
                        });
                        //清除复核缓存
                        _redisAppService.CleanReviewRecords(outboundOrder.OutboundOrder, cancelPickDetailDto.WarehouseSysId);
                        RedisWMS.CleanRedis<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewDiff, outboundOrder.OutboundOrder, cancelPickDetailDto.WarehouseSysId));
                        //清除拣货缓存
                        var key = string.Format(RedisSourceKey.RedisRFPicking, outboundOrder.OutboundOrder, cancelPickDetailDto.WarehouseSysId);
                        RedisWMS.CleanRedis<List<RFContainerPickingDetailListDto>>(key);

                        //取消拣货的容器------> 交接单修改为：新建，交接明细删除
                        _outboundTransferOrderRepository.DeleteOutboundTransferOrder(new OutboundTransferOrderQueryDto()
                        {
                            OutboundSysId = sysId,
                            WarehouseSysId = cancelPickDetailDto.WarehouseSysId,
                            Status = (int)OutboundTransferOrderStatus.New,
                            TransferType = (int)OutboundTransferOrderType.Unmark,
                            CurrentUserId = cancelPickDetailDto.CurrentUserId,
                            CurrentDisplayName = cancelPickDetailDto.CurrentDisplayName
                        });
                    }
                }

                #region 组织推送取消拣货工单数据
                if (docSysId != null && docSysId.Count > 0)
                {
                    var mqWorkDto = new MQWorkDto()
                    {
                        WorkBusinessType = (int)WorkBusinessType.Update,
                        WorkType = (int)UserWorkType.Picking,
                        WarehouseSysId = cancelPickDetailDto.WarehouseSysId,
                        CurrentUserId = cancelPickDetailDto.CurrentUserId,
                        CurrentDisplayName = cancelPickDetailDto.CurrentDisplayName,
                        CancelWorkDto = new CancelWorkDto()
                        {
                            DocSysIds = docSysId,
                            Status = (int)WorkStatus.Cancel
                        }
                    };

                    var workProcessDto = new MQProcessDto<MQWorkDto>()
                    {
                        BussinessSysId = new Guid(),
                        BussinessOrderNumber = "",
                        Descr = "",
                        CurrentUserId = cancelPickDetailDto.CurrentUserId,
                        CurrentDisplayName = cancelPickDetailDto.CurrentDisplayName,
                        WarehouseSysId = cancelPickDetailDto.WarehouseSysId,
                        BussinessDto = mqWorkDto
                    };
                    //推送工单数据
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, workProcessDto);
                }
                #endregion

            }
            catch (Exception ex)
            {
                businessLogDto.descr += ex.Message;
                businessLogDto.flag = false;
                throw new Exception(ex.Message);
            }
            finally
            {
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.BusinessLog, businessLogDto);
            }



            return string.Empty;
        }

        /// <summary>
        /// 取消拣货数量
        /// </summary>
        /// <param name="cancelPickQtyDto"></param>
        public void CancelPickQty(CancelPickQtyDto cancelPickQtyDto)
        {
            _crudRepository.ChangeDB(cancelPickQtyDto.WarehouseSysId);
            var pickDetail = _crudRepository.GetQuery<pickdetail>(p => p.SysId == cancelPickQtyDto.SysId).FirstOrDefault();
            if (pickDetail == null)
            {
                throw new Exception("拣货单不存在");
            }
            var outbound = _crudRepository.GetQuery<outbound>(p => p.SysId == pickDetail.OutboundSysId).FirstOrDefault();
            if (outbound != null && outbound.Status == (int)OutboundStatus.Delivery)
            {
                throw new Exception("出库单已发货，无法取消拣货数量");
            }
            if (pickDetail.Status != (int)PickDetailStatus.New)
            {
                throw new Exception("拣货单状态不为新建，无法取消拣货数量");
            }
            if (pickDetail.PickedQty < cancelPickQtyDto.Qty)
            {
                throw new Exception("取消拣货数量不能大于已拣货数量");
            }
            pickDetail.PickedQty -= cancelPickQtyDto.Qty;
            pickDetail.UpdateBy = cancelPickQtyDto.CurrentUserId;
            pickDetail.UpdateDate = DateTime.Now;
            pickDetail.UpdateUserName = cancelPickQtyDto.CurrentDisplayName;
            _crudRepository.Update(pickDetail);

            //取消容器拣货数量
            var container = _crudRepository.GetQuery<prebulkpack>(p => p.StorageCase.Equals(cancelPickQtyDto.StorageCase, StringComparison.OrdinalIgnoreCase) && p.WareHouseSysId == cancelPickQtyDto.WarehouseSysId).FirstOrDefault();
            if (container != null)
            {
                var containerDetail = _crudRepository.GetQuery<prebulkpackdetail>(p 
                    => p.PreBulkPackSysId == container.SysId 
                    && p.SkuSysId == pickDetail.SkuSysId 
                    && p.Loc.Equals(pickDetail.Loc, StringComparison.OrdinalIgnoreCase) 
                    && p.Lot == pickDetail.Lot).FirstOrDefault();
                if (containerDetail != null)
                {
                    if (containerDetail.Qty < cancelPickQtyDto.Qty)
                    {
                        throw new Exception("取消拣货数量不能大于容器中数量");
                    }
                    containerDetail.Qty -= cancelPickQtyDto.Qty;
                    containerDetail.UpdateBy = cancelPickQtyDto.CurrentUserId;
                    containerDetail.UpdateDate = DateTime.Now;
                    containerDetail.UpdateUserName = cancelPickQtyDto.CurrentDisplayName;
                    _crudRepository.Update(containerDetail);
                }
            }
        }

        /// <summary>
        /// 拣货
        /// </summary>
        /// <param name="pickingOperationDto"></param>
        public void SavePickingOperation(PickingOperationDto pickingOperationDto)
        {
            _crudRepository.ChangeDB(pickingOperationDto.WarehouseSysId);
            if (pickingOperationDto.PickingOperationDetails.Any(p => p.DisplayPickedQty > p.DisplayQty))
            {
                throw new Exception("拣货数量不能大于分配数量!");
            }
            var skuPackList = _packageRepository.GetSkuPackageList(pickingOperationDto.PickingOperationDetails.Select(p => p.SkuSysId).ToList());
            foreach (var item in pickingOperationDto.PickingOperationDetails)
            {
                var skuInfo = skuPackList.FirstOrDefault(p => p.SkuSysId == item.SkuSysId);
                if (skuInfo == null)
                {
                    throw new Exception("Id为：" + item.SkuSysId + "商品信息不存在");
                }
                skuInfo.Flag = (int)ReceiptConvert.ToMaterial;
                skuInfo.UnitQty = item.DisplayPickedQty;
                _packageAppService.GetSkuConversionQty(ref skuInfo);
                item.PickedQty = skuInfo.BaseQty;
            }
            _WMSSqlRepository.UpdatePickDetailPickedQty(pickingOperationDto);
        }

        /// <summary>
        /// 根据明细分配拣货数量
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="createPickDetailRuleDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        private bool PickDetailAllocatedQty(outbound outbound, CreatePickDetailRuleDto createPickDetailRuleDto, List<location> locationList,
            outboundrule outboundRule, List<outbounddetail> outboundDetailListAll, List<InvLotLocLpnDto> invLotLocLpnList, List<stockfrozen> frozenSkuList, List<sku> skuList, List<stockfrozen> frozenLocSkuList)
        {
            var businessLogDto = new BusinessLogDto();

            try
            {
                var msg = string.Empty;

                if (outbound.Status != (int)OutboundStatus.New)
                {
                    throw new Exception("订单" + outbound.OutboundOrder + "状态不等于新建无法进行库存分配");
                }

                businessLogDto.doc_sysId = outbound.SysId;
                businessLogDto.doc_order = outbound.OutboundOrder;
                businessLogDto.access_log_sysId = Guid.NewGuid();
                businessLogDto.request_json = JsonConvert.SerializeObject(createPickDetailRuleDto);
                businessLogDto.user_id = createPickDetailRuleDto.CurrentUserId.ToString();
                businessLogDto.user_name = createPickDetailRuleDto.CurrentDisplayName;
                businessLogDto.business_name = BusinessName.Pick.ToDescription();
                businessLogDto.business_type = BusinessType.Outbound.ToDescription();
                businessLogDto.business_operation = PublicConst.LogGeneratePickDetail;
                businessLogDto.flag = true;
                businessLogDto.descr = "[old_json扣减库存, new_json生成拣货单记录]";

                //总分配数量
                var totalAllocatedQty = 0;
                var outboundDetailList = outboundDetailListAll.Where(x => x.OutboundSysId == outbound.SysId).ToList();
                if (!outboundDetailList.Any())
                {
                    throw new Exception("没有找到指定的拣货的出库单");
                }

                var updateInventoryDtos = new List<UpdateInventoryDto>();
                var pickDetailList = new List<pickdetail>();
                var outbounddetailUpdate = new List<outbounddetail>();

                outboundDetailList.ForEach(item =>
                {
                    var invLotLocLpn = new List<InvLotLocLpnDto>();

                    if (outboundRule.Status.HasValue && outboundRule.Status.Value &&
                        outboundRule.MatchingLotAttr.HasValue && outboundRule.MatchingLotAttr.Value)
                    {


                        //注原方法
                        //invLotLocLpn = invLotLocLpnList.Where(x => x.SkuSysId == item.SkuSysId && x.LotAttr01 == outbound.Channel && x.LotAttr02 == outbound.BatchNumber).ToList();

                        invLotLocLpn = invLotLocLpnList.Where(x => x.SkuSysId == item.SkuSysId).ToList();

                        if (string.IsNullOrEmpty(outbound.Channel))
                        {
                            invLotLocLpn = invLotLocLpn.Where(x => x.LotAttr01 == "" || x.LotAttr01 == null).ToList();
                        }
                        else
                        {
                            invLotLocLpn = invLotLocLpn.Where(x => x.LotAttr01 == outbound.Channel).ToList();
                        }

                        if (string.IsNullOrEmpty(outbound.BatchNumber))
                        {
                            invLotLocLpn = invLotLocLpn.Where(x => x.LotAttr02 == "" || x.LotAttr02 == null).ToList();
                        }
                        else
                        {
                            invLotLocLpn = invLotLocLpn.Where(x => x.LotAttr02 == outbound.BatchNumber).ToList();
                        }
                    }
                    else
                    {
                        invLotLocLpn = invLotLocLpnList.Where(x => x.SkuSysId == item.SkuSysId && x.Qty > 0).ToList();
                    }

                    if (invLotLocLpn.Any())
                    {
                        //剩余数量
                        var residualQty = item.Qty.Value;
                        foreach (var info in invLotLocLpn)
                        {
                            //校验库存冻结
                            //商品级别
                            var frozenSku = frozenSkuList.FirstOrDefault(p => p.SkuSysId == item.SkuSysId);
                            if (frozenSku != null)
                            {
                                var sku = skuList.FirstOrDefault(x => x.SysId == item.SkuSysId);
                                throw new Exception($"商品({sku.SkuName})已经被冻结，不能拣货!");
                            }

                            //货位级别
                            var location = locationList.Where(p => p.Loc.ToUpper() == info.Loc.ToUpper() && p.WarehouseSysId == createPickDetailRuleDto.WarehouseSysId).FirstOrDefault();

                            if (location == null)
                            {
                                throw new Exception($"货位{info.Loc}已经不存在，请重新创建!");
                            }

                            if (location.Status == (int)LocationStatus.Frozen)
                            {
                                //冻结货位或者储区，不能分配
                                continue;
                            }

                            //货位商品级别
                            if (frozenLocSkuList.Count > 0)
                            {
                                var locskuFrozen = frozenLocSkuList.FirstOrDefault(p => p.SkuSysId == info.SkuSysId && p.Loc == info.Loc);//info

                                if (locskuFrozen != null)
                                {
                                    //货位商品冻结，不能分配
                                    continue;
                                }
                            }


                            //扣减数量
                            var deductionQty = 0;
                            //当前可用数量
                            var currentQty = CommonBussinessMethod.GetAvailableQty(info.Qty, info.AllocatedQty, info.PickedQty, info.FrozenQty);
                            if (currentQty == 0)
                            {
                                //如果可用数量 =0 那么匹配下一条库存信息
                                continue;
                            }
                            deductionQty = residualQty <= currentQty ? residualQty : currentQty;
                            //计算剩余数量
                            residualQty = residualQty - deductionQty;
                            totalAllocatedQty = totalAllocatedQty + deductionQty;
                            info.Qty = info.Qty - deductionQty;
                            updateInventoryDtos.Add(new UpdateInventoryDto()
                            {
                                InvLotLocLpnSysId = info.InvLotLocLpnSysId,
                                InvLotSysId = info.InvLotSysId,
                                InvSkuLocSysId = info.InvSkuLocSysId,
                                Qty = deductionQty,
                                CurrentUserId = createPickDetailRuleDto.CurrentUserId,
                                CurrentDisplayName = createPickDetailRuleDto.CurrentDisplayName,
                                WarehouseSysId = createPickDetailRuleDto.WarehouseSysId,
                            });

                            //拣货记录
                            pickDetailList.Add(
                                CreatePickDetail
                                    (
                                        item,
                                        createPickDetailRuleDto.PickDetailOrder,
                                        createPickDetailRuleDto.CurrentDisplayName,
                                        createPickDetailRuleDto.CurrentUserId,
                                        createPickDetailRuleDto.WarehouseSysId,
                                        PickDetailStatus.New,
                                        deductionQty,
                                        info.Loc,
                                        info.Lot,
                                        info.Lpn));
                            if (residualQty == 0)
                            {
                                break;
                            }
                        }

                        if (residualQty > 0)
                        {

                            throw new Exception("订单号" + outbound.OutboundOrder + "库存不足,无法进行分配！");
                        }

                        outbounddetailUpdate.Add(new outbounddetail()
                        {
                            SysId = item.SysId,
                            AllocatedQty = item.Qty,
                            Status = (int)OutboundDetailStatus.Allocation,
                            UpdateBy = createPickDetailRuleDto.CurrentUserId,
                            UpdateUserName = createPickDetailRuleDto.CurrentDisplayName

                        });
                    }
                    else
                    {

                        throw new Exception("订单号" + outbound.OutboundOrder + "库存不足,无法进行分配！");
                    }
                });
                outbound.Status = (int)OutboundStatus.Allocation;
                outbound.TotalAllocatedQty = totalAllocatedQty;
                outbound.UpdateBy = createPickDetailRuleDto.CurrentUserId;
                outbound.UpdateUserName = createPickDetailRuleDto.CurrentDisplayName;
                outbound.UpdateDate = DateTime.Now;
                outbound.TS = Guid.NewGuid();
                _WMSSqlRepository.Update(outbound);
                _WMSSqlRepository.UpdateInventoryAllocatedQty(updateInventoryDtos);
                _WMSSqlRepository.BatchUpdateOutboundDetail(outbounddetailUpdate);
                _WMSSqlRepository.BatchInsertPickDetail(pickDetailList);
                _WMSSqlRepository.SaveChange();
                businessLogDto.old_json = JsonConvert.SerializeObject(updateInventoryDtos);
                businessLogDto.new_json = JsonConvert.SerializeObject(pickDetailList);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                businessLogDto.descr += ex.Message;
                businessLogDto.flag = false;
                throw new Exception("操作失败");
            }
            catch (Exception ex)
            {
                businessLogDto.descr += ex.Message;
                businessLogDto.flag = false;
                throw new Exception(ex.Message);
            }
            finally
            {
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.BusinessLog, businessLogDto);
            }

            return true;
        }

        /// <summary>
        /// 创建拣货明细
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pickDetailOrder"></param>
        /// <param name="pickDetailStatus"></param>
        ///<param name="qty"></param>
        /// <param name="loc"></param>
        /// <param name="lot"></param>
        /// <param name="lpn"></param>
        private pickdetail CreatePickDetail(outbounddetail item, string pickDetailOrder, string currentDisplayName, int currentUserId, Guid wareHouseSysId, PickDetailStatus pickDetailStatus = PickDetailStatus.OutOfStock, int qty = 0, string loc = "", string lot = "", string lpn = "")
        {
            var pickDetail = new pickdetail();
            pickDetail.SysId = Guid.NewGuid();
            pickDetail.OutboundSysId = item.OutboundSysId;
            pickDetail.OutboundDetailSysId = item.SysId;
            pickDetail.SkuSysId = item.SkuSysId;
            pickDetail.PickDetailOrder = pickDetailOrder;
            pickDetail.PackSysId = item.PackSysId;
            pickDetail.UOMSysId = item.UOMSysId;
            pickDetail.Qty = qty;
            pickDetail.CreateBy = currentUserId;
            pickDetail.CreateUserName = currentDisplayName;
            pickDetail.CreateDate = DateTime.Now;
            pickDetail.UpdateUserName = currentDisplayName;
            pickDetail.UpdateBy = currentUserId;
            pickDetail.UpdateDate = DateTime.Now;
            pickDetail.Status = (int)pickDetailStatus;
            pickDetail.Loc = loc;
            pickDetail.Lot = lot;
            pickDetail.Lpn = lpn;
            pickDetail.WareHouseSysId = wareHouseSysId;
            // _crudRepository.Insert(pickDetail);
            return pickDetail;
        }

        /// <summary>
        /// 获取拣货单明细
        /// </summary>
        /// <param name="pickingOperationQuery"></param>
        /// <returns></returns>
        public List<PickingOperationDetail> GetPickingOperationDetails(PickingOperationQuery pickingOperationQuery)
        {
            _crudRepository.ChangeDB(pickingOperationQuery.WarehouseSysId);
            var pickDetails = _crudRepository.GetQuery<pickdetail>(p => p.PickDetailOrder.Equals(pickingOperationQuery.PickDetailOrder, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!pickDetails.Any())
            {
                throw new Exception("拣货单数据不存在");
            }
            if (pickDetails.Any(p => p.Status != (int)PickDetailStatus.New))
            {
                throw new Exception("同批拣货单必须全部为新建状态");
            }
            return _crudRepository.GetPickingOperationDetails(pickingOperationQuery);
        }
    }
}