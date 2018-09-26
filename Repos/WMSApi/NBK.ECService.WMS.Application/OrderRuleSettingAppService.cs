using System;
using System.Collections.Generic;
using System.Linq;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ.OrderRule;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using Abp.Domain.Uow;

namespace NBK.ECService.WMS.Application
{
    public class OrderRuleSettingAppService : WMSApplicationService, IOrderRuleSettingAppService
    {
        private ICrudRepository _crudRepository = null;
        private IWMSSqlRepository _wmsSqlRepository = null;
        private IPickDetailAppService _pickDetailAppService = null;

        public OrderRuleSettingAppService(ICrudRepository crudRepository, IWMSSqlRepository wmsSqlRepository, IPickDetailAppService pickDetailAppService)
        {
            this._crudRepository = crudRepository;
            this._wmsSqlRepository = wmsSqlRepository;
            this._pickDetailAppService = pickDetailAppService;
        }

        /// <summary>
        /// 根据Id 获取预包装规则
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public PreOrderRuleDto GetPreOrderRuleByWarehouseSysId(Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var preOrderrule = _crudRepository.FirstOrDefault<preorderrule>(x => x.WarehouseSysId == warehouseSysId);
            if (preOrderrule == null)
            {
                return new PreOrderRuleDto();
            }
            else
            {
                return preOrderrule.TransformTo<PreOrderRuleDto>();
            }

        }

        /// <summary>
        /// 订单匹配规则
        /// </summary>
        /// <param name="mqOrderRuleDto"></param>
        /// <returns></returns>
        public CommonResponse OrderBindingByPreOrderRule(MQOrderRuleDto mqOrderRuleDto)
        {
            _crudRepository.ChangeDB(mqOrderRuleDto.WarehouseSysId);
            var commonResponse = new CommonResponse(false, "", mqOrderRuleDto.OrderNumber + "没有可以匹配的预包装订单");
            var preOrderrule = _crudRepository.FirstOrDefault<preorderrule>(x => x.WarehouseSysId == mqOrderRuleDto.WarehouseSysId);
            // 判断是否开启匹配规则
            if (preOrderrule != null && preOrderrule.Status.HasValue && preOrderrule.Status.Value)
            {
                var outbound = _crudRepository.Get<outbound>(mqOrderRuleDto.OrderSysId);

                if (preOrderrule.ServiceStation.HasValue && preOrderrule.ServiceStation.Value)
                {
                    var prePack = _crudRepository.GetQuery<prepack>(x => x.Status == (int)PrePackStatus.New && x.WareHouseSysId == mqOrderRuleDto.WarehouseSysId && x.OutboundSysId == null && x.ServiceStationName == outbound.ServiceStationName).OrderBy(x => x.CreateDate).FirstOrDefault();
                    if (prePack != null)
                    {
                        commonResponse = MatechingUpdate(prePack, outbound);
                    }
                }
                else
                {
                    // 匹配率
                    var matchingRate = preOrderrule.MatchingRate.Value;
                    var matchingMaxRate = preOrderrule.MatchingMaxRate.Value;

                    var outboundDetailList = _crudRepository.GetQuery<outbounddetail>(x => x.OutboundSysId == outbound.SysId).ToList();

                    var prePackList = _crudRepository.GetQuery<prepack>(x => x.Status == (int)PrePackStatus.New && x.WareHouseSysId == mqOrderRuleDto.WarehouseSysId && x.OutboundSysId == null).OrderBy(x => x.CreateDate).ToList();
                    foreach (var info in prePackList)
                    {
                        var matchingQtyRate = false;
                        var matchingSkuRate = false;
                        var prePackDetail = _crudRepository.GetQuery<prepackdetail>(x => x.PrePackSysId == info.SysId).ToList();
                        if (preOrderrule.MatchingQty != null && preOrderrule.MatchingQty.Value)
                        {
                            matchingQtyRate = MatchingOutboundBySkuQty(matchingRate, matchingMaxRate, prePackDetail, outboundDetailList);

                        }
                        if (preOrderrule.MatchingSku != null && preOrderrule.MatchingSku.Value)
                        {
                            matchingSkuRate = MatchingOutboundBySku(matchingRate, matchingMaxRate, prePackDetail, outboundDetailList);
                        }
                        #region 规则匹配成功结果更新
                        if (preOrderrule.MatchingQty != null && preOrderrule.MatchingQty.Value &&
                            preOrderrule.MatchingSku != null && preOrderrule.MatchingSku.Value)
                        {
                            if (matchingQtyRate && matchingSkuRate)
                            {
                                commonResponse = MatechingUpdate(info, outbound);
                                break;
                            }
                        }
                        else if (preOrderrule.MatchingQty != null && preOrderrule.MatchingQty.Value)
                        {
                            if (matchingQtyRate)
                            {
                                commonResponse = MatechingUpdate(info, outbound);
                                break;
                            }
                        }
                        else if (preOrderrule.MatchingSku != null && preOrderrule.MatchingSku.Value)
                        {
                            if (matchingSkuRate)
                            {
                                commonResponse = MatechingUpdate(info, outbound);
                                break;
                            }
                        }

                        #endregion

                    }
                }
            }
            return commonResponse;
        }

        /// <summary>
        /// 匹配成功后进行系统更新
        /// </summary>
        /// <param name="info"></param>
        /// <param name="outbound"></param>
        /// <returns></returns>

        private CommonResponse MatechingUpdate(prepack info, outbound outbound)
        {
            _crudRepository.ChangeDB(info.WareHouseSysId);
            info.OutboundOrder = outbound.OutboundOrder;
            info.OutboundSysId = outbound.SysId;
            info.UpdateBy = 999;
            info.UpdateDate = DateTime.Now;
            info.UpdateUserName = "WMS自动化处理中心";
            _crudRepository.Update(info);
            if (!string.IsNullOrEmpty(info.BatchNumber))
            {
                outbound.BatchNumber = info.BatchNumber;
                _crudRepository.Update(outbound);
            }

            //修改散货装箱出库单号
            _wmsSqlRepository.UpdatePreBulkPackOutboundByBind(outbound.SysId, outbound.OutboundOrder, info.SysId);

            return new CommonResponse(true);
        }

        /// <summary>
        /// 匹配出库单SkuQty
        /// </summary>
        /// <returns></returns>
        private bool MatchingOutboundBySkuQty(int matchingRate, int matchingMaxRate, List<prepackdetail> prePackDetailList, List<outbounddetail> outboundDetailList)
        {
            var preQtyRate = 0m;
            var outQtyRate = 0m;

            if (prePackDetailList.Any() && outboundDetailList.Any())
            {
                var prePackQty = Convert.ToDecimal(prePackDetailList.Sum(x => x.PreQty).Value);
                var outboundQty = Convert.ToDecimal(outboundDetailList.Sum(x => x.Qty).Value);
                preQtyRate = prePackQty / outboundQty * 100m;
                outQtyRate = outboundQty / prePackQty * 100m;
                if (preQtyRate > matchingMaxRate || preQtyRate < matchingRate)
                {
                    return false;
                }
                if (outQtyRate > matchingMaxRate || outQtyRate < matchingRate)
                {
                    return false;
                }
                var successCount = 0;
                #region 出库单匹配

                foreach (var info in outboundDetailList)
                {
                    var packDetail = prePackDetailList.FirstOrDefault(x => x.SkuSysId == info.SkuSysId);
                    if (packDetail != null)
                    {
                        var detailRate = Convert.ToDecimal(packDetail.PreQty.Value) / info.Qty * 100m;

                        if (detailRate >= matchingRate && detailRate < matchingMaxRate)
                        {
                            successCount++;
                        }
                    }
                }

                if (successCount > 0)
                {
                    outQtyRate = Convert.ToDecimal(successCount) / Convert.ToDecimal(outboundDetailList.Count()) * 100M;
                }
                #endregion


                if (outQtyRate >= matchingRate && outQtyRate < matchingMaxRate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 匹配出库单SkuQty
        /// </summary>
        /// <returns></returns>
        private bool MatchingOutboundBySku(int matchingRate, int matchingMaxRate, List<prepackdetail> prePackDetailList, List<outbounddetail> outboundDetailList)
        {
            var preRate = 0m;
            var outRate = 0m;

            if (prePackDetailList.Any() && outboundDetailList.Any())
            {
                var prePackDetail = Convert.ToDecimal(prePackDetailList.Count());
                var matchingSku = 0m;
                var outboundSku = Convert.ToDecimal(outboundDetailList.Count());

                preRate = prePackDetail / outboundSku * 100m;
                outRate = outboundSku / prePackDetail * 100m;
                if (preRate > matchingMaxRate || preRate < matchingRate)
                {
                    return false;
                }
                if (outRate > matchingMaxRate || outRate < matchingRate)
                {
                    return false;
                }

                preRate = 0;
                outRate = 0;
                foreach (var info in outboundDetailList)
                {
                    var detail = prePackDetailList.Where(x => x.SkuSysId == info.SkuSysId);
                    if (detail.Any())
                    {
                        matchingSku++;
                    }
                }

                outRate = matchingSku / outboundSku * 100m;
                preRate = matchingSku / prePackDetail * 100m;

                if ((preRate >= matchingRate && preRate < matchingMaxRate) &&
                      (outRate >= matchingRate && outRate < matchingMaxRate))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }


        /// <summary>
        /// 根据Id 获取预包装规则
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public void SavePreOrderRule(PreOrderRuleDto preOrderRuleDto)
        {
            _crudRepository.ChangeDB(preOrderRuleDto.WarehouseSysId);
            if (preOrderRuleDto.SysId.HasValue && preOrderRuleDto.SysId.Value != new Guid())
            {
                var preOrderrule = _crudRepository.Get<preorderrule>(preOrderRuleDto.SysId.Value);
                preOrderrule.Status = preOrderRuleDto.Status;
                preOrderrule.MatchingQty = preOrderRuleDto.MatchingQty;
                preOrderrule.MatchingRate = preOrderRuleDto.MatchingRate;
                preOrderrule.MatchingMaxRate = preOrderRuleDto.MatchingMaxRate;
                preOrderrule.MatchingSku = preOrderRuleDto.MatchingSku;
                preOrderrule.ExceedQty = preOrderRuleDto.ExceedQty;
                preOrderrule.DeliveryIntercept = preOrderRuleDto.DeliveryIntercept;
                preOrderrule.UpdateBy = preOrderRuleDto.CurrentUserId;
                preOrderrule.UpdateDate = DateTime.Now;
                preOrderrule.UpdateUserName = preOrderRuleDto.CurrentDisplayName;
                preOrderrule.ServiceStation = preOrderRuleDto.ServiceStation;
                _crudRepository.Update(preOrderrule);
            }
            else
            {
                var preOrderRule = new preorderrule();
                preOrderRule = preOrderRuleDto.TransformTo<preorderrule>();
                preOrderRule.SysId = Guid.NewGuid();
                preOrderRule.CreateBy = preOrderRuleDto.CurrentUserId;
                preOrderRule.CreateDate = DateTime.Now;
                preOrderRule.CreateUserName = preOrderRuleDto.CurrentDisplayName;
                preOrderRule.UpdateBy = preOrderRuleDto.CurrentUserId;
                preOrderRule.UpdateDate = DateTime.Now;
                preOrderRule.UpdateUserName = preOrderRuleDto.CurrentDisplayName;
                _crudRepository.Insert(preOrderRule);

            }
        }


        /// <summary>
        /// 获取出库单规则
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public OutboundRuleDto GetOutboundRuleByWarehouseSysId(Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var preOrderrule = _crudRepository.FirstOrDefault<outboundrule>(x => x.WarehouseSysId == warehouseSysId);
            if (preOrderrule == null)
            {
                return new OutboundRuleDto();
            }
            else
            {
                return preOrderrule.TransformTo<OutboundRuleDto>();
            }
        }

        /// <summary>
        /// 获取出库单规则
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public void SaveOutboundRule(OutboundRuleDto outboundRuleDto)
        {
            _crudRepository.ChangeDB(outboundRuleDto.WarehouseSysId);
            if (outboundRuleDto.SysId.HasValue && outboundRuleDto.SysId.Value != new Guid())
            {
                var outboundRule = _crudRepository.Get<outboundrule>(outboundRuleDto.SysId.Value);
                outboundRule.Status = outboundRuleDto.Status;
                outboundRule.MatchingLotAttr = outboundRuleDto.MatchingLotAttr;
                outboundRule.DeliverySortRules = outboundRuleDto.DeliverySortRules;
                outboundRule.DeliveryIsAsyn = outboundRuleDto.DeliveryIsAsyn;
                outboundRule.CreateOutboundIsAsyn = outboundRuleDto.CreateOutboundIsAsyn;
                outboundRule.IsPickingSkuLoc = outboundRuleDto.IsPickingSkuLoc;
                outboundRule.AutomaticAllocation = outboundRuleDto.AutomaticAllocation;
                outboundRule.UpdateBy = outboundRuleDto.CurrentUserId;
                outboundRule.UpdateDate = DateTime.Now;
                outboundRule.UpdateUserName = outboundRuleDto.CurrentDisplayName;
                _crudRepository.Update(outboundRule);
            }
            else
            {
                var outboundRule = new outboundrule();
                outboundRule = outboundRuleDto.TransformTo<outboundrule>();
                outboundRule.SysId = Guid.NewGuid();
                outboundRule.CreateBy = outboundRuleDto.CurrentUserId;
                outboundRule.CreateDate = DateTime.Now;
                outboundRule.CreateUserName = outboundRuleDto.CurrentDisplayName;
                outboundRule.UpdateBy = outboundRuleDto.CurrentUserId;
                outboundRule.UpdateDate = DateTime.Now;
                outboundRule.UpdateUserName = outboundRuleDto.CurrentDisplayName;
                _crudRepository.Insert(outboundRule);

            }
        }

        /// <summary>
        /// 出库单自动分配
        /// </summary>
        /// <param name="mqOrderRuleDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public CommonResponse OutboundOrderAutomaticAllocation(MQOrderRuleDto mqOrderRuleDto)
        {
            var cr = new CommonResponse();
            try
            {
                _crudRepository.ChangeDB(mqOrderRuleDto.WarehouseSysId);
                var warehouse = _crudRepository.FirstOrDefault<warehouse>(o => o.SysId == mqOrderRuleDto.WarehouseSysId);
                var outboundRule = _crudRepository.FirstOrDefault<outboundrule>(x => x.WarehouseSysId == mqOrderRuleDto.WarehouseSysId);
                if (outboundRule == null)
                {
                    cr.IsSuccess = false;
                    cr.ErrorMessage = warehouse.Name + "未设置出库单规则设置,分配异常请检查!";
                    return cr;
                }
                if (!outboundRule.Status.Value)
                {
                    cr.IsSuccess = true;
                    cr.ErrorMessage = warehouse.Name + "未开启出库规则.";
                    return cr;
                }
                if (outboundRule.AutomaticAllocation)
                {
                    var createPickDetailRuleDto = new CreatePickDetailRuleDto();
                    createPickDetailRuleDto.PickSysIds = mqOrderRuleDto.OrderSysId.ToString() + ",";
                    createPickDetailRuleDto.PickRule = "";
                    createPickDetailRuleDto.PickType = PublicConst.PickTypeByOrder;
                    createPickDetailRuleDto.WarehouseSysId = mqOrderRuleDto.WarehouseSysId;
                    createPickDetailRuleDto.CurrentUserId = mqOrderRuleDto.CurrentUserId;
                    createPickDetailRuleDto.CurrentDisplayName = mqOrderRuleDto.CurrentDisplayName;

                    _pickDetailAppService.GeneratePickDetailByPickRule(createPickDetailRuleDto);
                    cr.IsSuccess = true;
                    cr.ErrorMessage = mqOrderRuleDto.OrderNumber + "自动分配完成";
                    return cr;
                }
                else
                {
                    cr.IsSuccess = true;
                    cr.ErrorMessage = warehouse.Name + "未开启出库单规则设置,无法进行自动分配";
                    return cr;
                }
            }
            catch (Exception ex)
            {
                cr.IsSuccess = false;
                cr.ErrorMessage = ex.Message;
                return cr;
            }

        }


        public WorkRuleDto GetWorkRuleByWarehouseSysId(Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var preOrderrule = _crudRepository.FirstOrDefault<workrule>(x => x.WarehouseSysId == warehouseSysId);
            if (preOrderrule == null)
            {
                return new WorkRuleDto();
            }
            else
            {
                return preOrderrule.TransformTo<WorkRuleDto>();
            }
        }

        public void SaveWorkRule(WorkRuleDto workRuleDto)
        {
            _crudRepository.ChangeDB(workRuleDto.WarehouseSysId);
            if (workRuleDto.SysId.HasValue && workRuleDto.SysId.Value != new Guid())
            {
                var outboundRule = _crudRepository.Get<workrule>(workRuleDto.SysId.Value);
                outboundRule.Status = workRuleDto.Status;
                outboundRule.ReceiptWork = workRuleDto.ReceiptWork;
                outboundRule.PickWork = workRuleDto.PickWork;
                outboundRule.ShelvesWork = workRuleDto.ShelvesWork;
                outboundRule.UpdateBy = workRuleDto.CurrentUserId;
                outboundRule.UpdateDate = DateTime.Now;
                outboundRule.UpdateUserName = workRuleDto.CurrentDisplayName;
                _crudRepository.Update(outboundRule);
            }
            else
            {
                var outboundRule = new workrule();
                outboundRule = workRuleDto.TransformTo<workrule>();
                outboundRule.SysId = Guid.NewGuid();
                outboundRule.CreateBy = workRuleDto.CurrentUserId;
                outboundRule.CreateDate = DateTime.Now;
                outboundRule.CreateUserName = workRuleDto.CurrentDisplayName;
                outboundRule.UpdateBy = workRuleDto.CurrentUserId;
                outboundRule.UpdateDate = DateTime.Now;
                outboundRule.UpdateUserName = workRuleDto.CurrentDisplayName;
                _crudRepository.Insert(outboundRule);

            }
        }

        /// <summary>
        /// 加工规则
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public AssemblyRuleDto GetAssemblyRuleWarehouseSysId(Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var rule = _crudRepository.FirstOrDefault<assemblyrule>(x => x.WarehouseSysId == warehouseSysId);
            if (rule == null)
            {
                return new AssemblyRuleDto();
            }
            else
            {
                return rule.TransformTo<AssemblyRuleDto>();
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="dto"></param>
        public void SaveAssemblyRule(AssemblyRuleDto dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            if (dto.SysId.HasValue && dto.SysId.Value != new Guid())
            {
                var ruleModel = _crudRepository.Get<assemblyrule>(dto.SysId.Value);
                ruleModel.Status = dto.Status;
                ruleModel.MatchingLotAttr = dto.MatchingLotAttr;
                ruleModel.DeliverySortRules = dto.DeliverySortRules;
                ruleModel.MatchingSkuBorrowChannel = dto.MatchingSkuBorrowChannel;
                ruleModel.UpdateBy = dto.CurrentUserId;
                ruleModel.UpdateDate = DateTime.Now;
                ruleModel.UpdateUserName = dto.CurrentDisplayName;
                _crudRepository.Update(ruleModel);
            }
            else
            {
                var assemblyRule = new assemblyrule();
                assemblyRule = dto.TransformTo<assemblyrule>();
                assemblyRule.SysId = Guid.NewGuid();
                assemblyRule.CreateBy = dto.CurrentUserId;
                assemblyRule.CreateDate = DateTime.Now;
                assemblyRule.CreateUserName = dto.CurrentDisplayName;
                assemblyRule.UpdateBy = dto.CurrentUserId;
                assemblyRule.UpdateDate = DateTime.Now;
                assemblyRule.UpdateUserName = dto.CurrentDisplayName;
                _crudRepository.Insert(assemblyRule);
            }
        }
    }
}