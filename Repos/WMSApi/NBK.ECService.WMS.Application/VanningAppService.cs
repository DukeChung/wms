using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using System.Data.Entity.Infrastructure;
using NBK.ECService.WMS.DTO.ThirdParty.ZTO;
using NBK.ECService.WMS.DTO.MQ.Log;
using NBK.ECService.WMS.Utility.Enum.Log;
using NBK.ECService.WMS.Utility.RabbitMQ;
using Newtonsoft.Json;
using NBK.ECService.WMS.DTO.MQ;
using Abp.Domain.Uow;
using System.Transactions;

namespace NBK.ECService.WMS.Application
{
    public class VanningAppService : WMSApplicationService, IVanningAppService
    {
        private IVanningRepository _crudRepository = null;
        private IPickDetailRepository _pickDetailRepository = null;
        private IInventoryRepository _inventoryRepository = null;
        private IVanningRepository _vanningRepository = null;
        private IWMSSqlRepository _wmsSqlRepository = null;
        private IBaseAppService _baseAppService = null;
        private IZTOAppService _ztoAppService = null;

        public VanningAppService(IVanningRepository crudRepository, IPickDetailRepository pickDetailRepository, IInventoryRepository inventoryRepository, IVanningRepository vanningRepository, IWMSSqlRepository wmsSqlRepository, IBaseAppService baseAppService, IZTOAppService ztoAppService)
        {
            this._crudRepository = crudRepository;
            this._pickDetailRepository = pickDetailRepository;
            this._inventoryRepository = inventoryRepository;
            this._vanningRepository = vanningRepository;
            this._wmsSqlRepository = wmsSqlRepository;
            this._baseAppService = baseAppService;
            this._ztoAppService = ztoAppService;
        }

        /// <summary>
        /// 获取装箱操作相关数据
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public VanningOperationDto GetVanningOperationDtoByOrder(string orderNumber, Guid wareHouseSysId)
        {
            _crudRepository.ChangeDB(wareHouseSysId);

            //扫描完成后 判断出库单是否已经被B2C退货
            pickdetail pickDetail = null;
            outbound outbound = null;
            if (!string.IsNullOrEmpty(orderNumber))
            {
                pickDetail = _crudRepository.FirstOrDefault<pickdetail>(x => x.PickDetailOrder == orderNumber);
                if (pickDetail != null)
                {
                    outbound = _crudRepository.Get<outbound>((Guid)pickDetail.OutboundSysId);
                }
                else
                {
                    outbound = _crudRepository.FirstOrDefault<outbound>(x => x.OutboundOrder == orderNumber);
                }

                if (outbound != null)
                {
                    if ((outbound.IsReturn ?? 0) == (int)OutboundReturnStatus.B2CReturn)
                    {
                        throw new Exception("出库单" + outbound.OutboundOrder + ",已经被B2C退货，无法装箱!");
                    }
                }
            }

            var operationDto = new VanningOperationDto();

            orderNumber = orderNumber.Trim();
            operationDto.OutboundChildType = outbound != null ? outbound.OutboundChildType : null;
            operationDto.PickDetailOperationDto = _pickDetailRepository.GetPickDetailOperationDto(orderNumber, wareHouseSysId);
            if (!operationDto.PickDetailOperationDto.Any(x => x.Status != (int)PickDetailStatus.Cancel))
            {
                throw new Exception("未找到" + orderNumber + ",匹配的拣货记录!");
            }

            //var cancelPickDetailDto = operationDto.PickDetailOperationDto.Where(x => x.Status != (int)PickDetailStatus.Cancel).ToList();
            //if (!cancelPickDetailDto.Any())
            //{
            //    throw new Exception(orderNumber + ",已经取消拣货!");
            //}

            operationDto.PickDetailOperationDto = operationDto.PickDetailOperationDto.Where(x => x.Status == (int)PickDetailStatus.New).ToList();
            if (!operationDto.PickDetailOperationDto.Any())
            {
                throw new Exception(orderNumber + ",已经全部装箱!");
            }
            operationDto.TotalOrderCount = operationDto.PickDetailOperationDto.GroupBy(x => new { x.OutboundSysId }).Count();
            operationDto.TotalSkuCount = operationDto.PickDetailOperationDto.Sum(x => x.Qty);

            //转换成页面显示列表数据
            var pickDetailSumList =
                operationDto.PickDetailOperationDto.GroupBy(
                    x => new
                    {
                        x.OutboundSysId,
                        x.OutboundDetailSysId,
                        x.OutboundOrder,
                        x.SkuSysId,
                        x.SkuName,
                        x.UPC,
                        x.SkuDescr,
                        x.UPC01,
                        x.UPC02,
                        x.UPC03,
                        x.UPC04,
                        x.UPC05,
                        x.FieldValue01,
                        x.FieldValue02,
                        x.FieldValue03,
                        x.FieldValue04,
                        x.FieldValue05,
                    }).Select(
                        group => new PickDetailSumDto
                        {
                            OutboundSysId = group.Key.OutboundSysId,
                            OutboundDetailSysId = group.Key.OutboundDetailSysId,
                            OutboundOrder = group.Key.OutboundOrder,
                            SkuSysId = group.Key.SkuSysId,
                            SkuName = group.Key.SkuName,
                            UPC = group.Key.UPC,
                            SkuDescr = group.Key.SkuDescr,
                            Qty = group.Sum(x => x.Qty),
                            ScanQty = 0,
                            UnScanQty = group.Sum(x => x.Qty),
                            UPC01 = group.Key.UPC01,
                            UPC02 = group.Key.UPC02,
                            UPC03 = group.Key.UPC03,
                            UPC04 = group.Key.UPC04,
                            UPC05 = group.Key.UPC05,
                            FieldValue01 = group.Key.FieldValue01,
                            FieldValue02 = group.Key.FieldValue02,
                            FieldValue03 = group.Key.FieldValue03,
                            FieldValue04 = group.Key.FieldValue04,
                            FieldValue05 = group.Key.FieldValue05,
                        }).ToList();

            var vanningPickDetails = _crudRepository.GetVanningPickDetailByOrder(orderNumber, pickDetail);
            if (vanningPickDetails != null && vanningPickDetails.Count > 0)
            {
                operationDto.PickDetailSumDto = (from a in pickDetailSumList
                                                 join b in vanningPickDetails on a.SkuSysId equals b.SkuSysId into bb
                                                 from tempb in bb.DefaultIfEmpty()
                                                 select new PickDetailSumDto
                                                 {
                                                     OutboundSysId = a.OutboundSysId,
                                                     OutboundDetailSysId = a.OutboundDetailSysId,
                                                     OutboundOrder = a.OutboundOrder,
                                                     SkuSysId = a.SkuSysId,
                                                     SkuName = a.SkuName,
                                                     UPC = a.UPC,
                                                     SkuDescr = a.SkuDescr,
                                                     Qty = a.Qty - (tempb == null ? 0 : tempb.Qty),
                                                     ScanQty = a.ScanQty,
                                                     UnScanQty = a.Qty - (tempb == null ? 0 : tempb.Qty),
                                                     UPC01 = a.UPC01,
                                                     UPC02 = a.UPC02,
                                                     UPC03 = a.UPC03,
                                                     UPC04 = a.UPC04,
                                                     UPC05 = a.UPC05,
                                                     FieldValue01 = a.FieldValue01,
                                                     FieldValue02 = a.FieldValue02,
                                                     FieldValue03 = a.FieldValue03,
                                                     FieldValue04 = a.FieldValue04,
                                                     FieldValue05 = a.FieldValue05,
                                                 }).ToList();
            }
            else
            {
                operationDto.PickDetailSumDto = pickDetailSumList;
            }

            operationDto.VanningRecordDto = _crudRepository.GetVanningRecordByOrder(orderNumber, wareHouseSysId, pickDetail);
            return operationDto;
        }

        /// <summary>
        /// 获取装箱分页数据
        /// </summary>
        /// <param name="vanningQueryDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public Pages<VanningDto> GetVanningList(VanningQueryDto vanningQueryDto)
        {
            _crudRepository.ChangeDB(vanningQueryDto.WarehouseSysId);
            return _crudRepository.GetVanningList(vanningQueryDto);
        }

        /// <summary>
        /// 前台装箱-封箱业务数据提交
        /// </summary>
        /// <param name="vanningDetailOperationDto"></param>
        /// <param name="actionType">装箱类型 装箱完成 和 部分装箱 装箱完成触发扣减库存业务</param> 
        /// <returns></returns>
        public VanningDetailDto SaveVanningDetailOperationDto(List<VanningDetailOperationDto> vanningDetailOperationDto, string actionType, string currentUserName, int currentUserId, Guid wareHouseSysId)
        {
            _crudRepository.ChangeDB(wareHouseSysId);
            var vanningDetailDto = new VanningDetailDto();
            var updateInventoryDtos = new List<UpdateInventoryDto>();
            if (vanningDetailOperationDto.Any())
            {
                var outBoundSysIdList = vanningDetailOperationDto.GroupBy(x => x.OutboundSysId).Select(x => x.Key).ToList();
                var vList = _crudRepository.GetQuery<vanning>(x => outBoundSysIdList.Contains(x.OutboundSysId) && x.Status != (int)VanningStatus.Cancel).ToList();
                var vdList = new List<vanningdetail>();
                if (vList != null && vList.Count > 0)
                {
                    var list = vList.Select(p => p.SysId).ToList();
                    vdList = _crudRepository.GetQuery<vanningdetail>(x => list.Contains((Guid)x.VanningSysId) && x.Status != (int)VanningStatus.Cancel)
                               .OrderBy(x => x.ContainerNumber)
                               .ToList();
                }

                var pdList = _crudRepository.GetQuery<pickdetail>(x => outBoundSysIdList.Contains(x.OutboundSysId) && x.Status != (int)PickDetailStatus.Cancel).ToList();

                var outboundList = _crudRepository.GetAllList<outbound>(x => outBoundSysIdList.Contains(x.SysId)).ToList();

                foreach (var sysId in outBoundSysIdList)
                {
                    var totalQty = 0;
                    var vanning = new vanning();
                    var outBoundSysId = sysId;

                    //判断出库单B2C是否做过退货 
                    var outboundB2C = outboundList.Where(x => x.SysId == sysId.GetValueOrDefault()).FirstOrDefault();
                    if (outboundB2C != null)
                    {
                        if (outboundB2C.IsReturn == (int)OutboundReturnStatus.B2CReturn)
                        {
                            throw new Exception("出库单" + outboundB2C.OutboundOrder + ",已经被B2C退货，无法装箱!");
                        }
                    } 

                    #region  检查装箱主表记录是否存在
                    //如果单据不是首次装箱，那么装箱记录就已经存在 ，如果是首次装箱那么写入新的装箱信息 ，如果是最后一箱装箱记录 更新装箱主表的 状态  
                    //是否最后一箱根据actionType == PublicConst.VanningActionType 判断 
                    ///var vanningList = _crudRepository.GetQuery<vanning>(x => x.OutboundSysId == outBoundSysId && x.Status != (int)VanningStatus.Cancel);
                    var vanningList = vList.Where(x => x.OutboundSysId == outBoundSysId);
                    var vanningDetailList = new List<vanningdetail>();
                    if (vanningList.Any())
                    {
                        vanning = vanningList.FirstOrDefault();
                        //vanningDetailList =
                        //    _crudRepository.GetQuery<vanningdetail>(x => x.VanningSysId == vanning.SysId && x.Status != (int)VanningStatus.Cancel)
                        //        .OrderBy(x => x.ContainerNumber)
                        //        .ToList();

                        vanningDetailList = vdList.Where(x => x.VanningSysId == vanning.SysId).ToList();

                        if (actionType == PublicConst.VanningActionType)
                        {
                            vanning.Status = (int)VanningStatus.Finish;
                        }
                        else
                        {
                            vanning.Status = (int)VanningStatus.Vanning;
                        }
                        _crudRepository.Update(vanning);
                    }
                    else
                    {
                        vanning.SysId = Guid.NewGuid();
                        //vanning.VanningOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberVanning);
                        vanning.VanningOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberVanning);
                        vanning.OutboundSysId = sysId;
                        vanning.VanningType = (int)VanningType.Order;
                        /// 如果一个订单只装一箱那么 自动标记为装箱完成， 如果多箱状态是 装箱中
                        if (actionType == PublicConst.VanningActionType)
                        {
                            vanning.Status = (int)VanningStatus.Finish;
                        }
                        else
                        {
                            vanning.Status = (int)VanningStatus.Vanning;
                        }

                        vanning.Remark = "";
                        vanning.VanningDate = DateTime.Now;
                        vanning.CreateBy = currentUserId;
                        vanning.CreateUserName = currentUserName;
                        vanning.CreateDate = DateTime.Now;
                        vanning.UpdateBy = currentUserId;
                        vanning.UpdateDate = DateTime.Now;
                        vanning.UpdateUserName = currentUserName;
                        vanning.WarehouseSysId = wareHouseSysId;
                        _crudRepository.Insert(vanning);
                    }
                    #endregion

                    //记录返回结果 装箱单号
                    vanningDetailDto.ContainerNumber = vanning.VanningOrder;

                    //获取对应订单的拣货记录
                    ///var pickDetailList = _crudRepository.GetQuery<pickdetail>(x => x.OutboundSysId == sysId && x.Status != (int)PickDetailStatus.Cancel).ToList();
                    ///
                    var pickDetailList = pdList.Where(x => x.OutboundSysId == sysId).ToList();

                    //获取对应订单 所有装箱明细
                    var vanningDetailOperationByOutbound = vanningDetailOperationDto.Where(x => x.OutboundSysId == sysId);

                    #region  箱子明细
                    foreach (var info in vanningDetailOperationByOutbound)
                    {
                        var vanningdetail = new vanningdetail();
                        //判断箱号是否存在
                        var checkvanningDetail = vanningDetailList.Where(x => x.ContainerNumber == info.ContainerNumber);
                        if (checkvanningDetail.Any())
                        {
                            vanningdetail = checkvanningDetail.FirstOrDefault();
                            //箱号存在判断是否最后一箱 如果是 进行库存扣减
                            if (vanningdetail.Status == (int)VanningStatus.Vanning &&
                                actionType == PublicConst.VanningActionType)
                            {
                                var vanningPickDetail = _crudRepository.GetQuery<vanningpickdetail>(x => x.VanningDetailSysId == vanningdetail.SysId);

                                foreach (var pickDetailInfo in vanningPickDetail)
                                {
                                    updateInventoryDtos.Add(
                                        VanningConversion(pickDetailInfo, pickDetailList, vanning, vanningdetail,
                                            currentUserName, currentUserId));
                                }
                                vanningdetail.Status = (int)VanningStatus.Finish;
                                _crudRepository.Update(vanning);
                            }
                            continue;
                        }

                        vanningdetail = info.JTransformTo<vanningdetail>();
                        vanningdetail.VanningSysId = vanning.SysId;
                        vanningdetail.SysId = Guid.NewGuid();
                        if (actionType == PublicConst.VanningActionType)
                        {
                            vanningdetail.Status = (int)VanningStatus.Finish;
                        }
                        else
                        {
                            vanningdetail.Status = (int)VanningStatus.Vanning;
                        }

                        vanningdetail.CreateBy = currentUserId;
                        vanningdetail.CreateDate = DateTime.Now;
                        vanningdetail.CreateUserName = currentUserName;
                        vanningdetail.UpdateBy = currentUserId;
                        vanningdetail.UpdateDate = DateTime.Now;
                        vanningdetail.UpdateUserName = currentUserName;


                        //**中通接口给中通订单ID和大头笔字段赋值
                        var outboundZTO = outboundList.Where(x => x.SysId == vanning.OutboundSysId.Value).FirstOrDefault();
                        if (outboundZTO != null)
                        {
                            //创建订单接口
                            CreateZTOOrderRequest submitRequest = new CreateZTOOrderRequest()
                            {
                                id = Guid.NewGuid().ToString(),
                                typeid = "1",
                                sender = new sender()
                                {
                                    name = PublicConst.ZTOSenderName,
                                    phone = PublicConst.ZTOSenderPhone,
                                    city = PublicConst.ZTOSenderCity,
                                    address = PublicConst.ZTOSenderAddress
                                },
                                receiver = new receiver()
                                {
                                    name = outboundZTO.ConsigneeName,
                                    phone = outboundZTO.ConsigneePhone,
                                    city = outboundZTO.ConsigneeProvince + "," + outboundZTO.ConsigneeCity + "," + outboundZTO.ConsigneeArea,
                                    address = outboundZTO.ConsigneeAddress
                                },
                            };

                            InterfaceLogDto interfaceLogDto = new InterfaceLogDto(submitRequest)
                            {
                                interface_type = InterfaceType.Invoke.ToDescription(),
                                interface_name = PublicConst.ZTOOrderSubmitLog
                            };

                            try
                            {
                                //测试环境因为单号经常不够使用所以不再调用中通接口
                                if (PublicConst.ZTOUserName == "test")
                                {
                                    vanningdetail.CarrierNumber = "";
                                    vanningdetail.Marke = "测试专用大头笔";
                                    vanningDetailDto.CarrierNumber = "";
                                    vanningDetailDto.Marke = "测试专用大头笔";

                                    var carrier = _crudRepository.GetQuery<carrier>(x => x.CarrierName == "ZTO中通快递").FirstOrDefault();

                                    if (carrier == null)
                                    {
                                        throw new Exception("未找到 ZTO中通快递 对应的快递配置信息！请添加！");
                                    }

                                    vanningdetail.CarrierSysId = carrier.SysId;
                                    interfaceLogDto.doc_sysId = info.SysId;
                                    interfaceLogDto.doc_order = string.Empty;
                                    interfaceLogDto.response_json = string.Empty;
                                    interfaceLogDto.flag = true;
                                }
                                else
                                {

                                    dynamic submitResponse = _ztoAppService.OrderSubmit(submitRequest);
                                    if (submitResponse != null)
                                    {
                                        if (submitResponse.result == true)
                                        {
                                            vanningdetail.CarrierNumber = submitResponse.keys.mailno;
                                            vanningdetail.Marke = submitResponse.keys.mark;
                                            vanningDetailDto.CarrierNumber = submitResponse.keys.mailno;
                                            vanningDetailDto.Marke = submitResponse.keys.mark;

                                            var carrier = _crudRepository.GetQuery<carrier>(x => x.CarrierName == "ZTO中通快递").FirstOrDefault();

                                            if (carrier == null)
                                            {
                                                throw new Exception("未找到 ZTO中通快递 对应的快递配置信息！请添加！");
                                            }

                                            vanningdetail.CarrierSysId = carrier.SysId;
                                        }
                                        else
                                        {
                                            throw new Exception(submitResponse.remark);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("中通接口异常!");
                                    }
                                    interfaceLogDto.doc_sysId = info.SysId;
                                    interfaceLogDto.doc_order = string.Empty;
                                    interfaceLogDto.response_json = JsonConvert.SerializeObject(submitResponse);
                                    interfaceLogDto.flag = true;
                                }
                            }
                            catch (Exception e)
                            {
                                //记录接口异常日志
                                interfaceLogDto.response_json = JsonConvert.SerializeObject(e);
                                interfaceLogDto.flag = false;
                                throw new Exception("调用中通接口失败，失败原因：" + e.Message);
                            }
                            finally
                            {
                                //发送MQ
                                interfaceLogDto.end_time = DateTime.Now;
                                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InterfaceLog, interfaceLogDto);
                            }
                        }

                        _crudRepository.Insert(vanningdetail);
                        /// 组织返回结果 用于打印
                        vanningDetailDto.ContainerNumber += "-" + vanningdetail.ContainerNumber;
                        vanningDetailDto.SysId = vanningdetail.SysId;

                        if (info.VanningPickDetailDto == null)
                        {
                            vanningDetailDto = new VanningDetailDto();
                            throw new Exception("未找到需要装箱的记录");
                        }
                        if (info.VanningPickDetailDto.Any())
                        {
                            foreach (var item in info.VanningPickDetailDto)
                            {
                                var vanningpickdetail = item.JTransformTo<vanningpickdetail>();
                                vanningpickdetail.VanningDetailSysId = vanningdetail.SysId;
                                vanningpickdetail.SysId = Guid.NewGuid();
                                vanningpickdetail.CreateBy = currentUserId;
                                vanningpickdetail.CreateDate = DateTime.Now;
                                vanningpickdetail.CreateUserName = currentUserName;
                                vanningpickdetail.UpdateBy = currentUserId;
                                vanningpickdetail.UpdateDate = DateTime.Now;
                                vanningpickdetail.UpdateUserName = currentUserName;
                                totalQty = totalQty + vanningpickdetail.Qty.Value;
                                _crudRepository.Insert(vanningpickdetail);
                                if (actionType == PublicConst.VanningActionType)
                                {
                                    updateInventoryDtos.Add(
                                        VanningConversion(vanningpickdetail, pickDetailList, vanning, vanningdetail,
                                            currentUserName, currentUserId));
                                }
                            }

                            vanningDetailDto.VannginSkuCount = info.VanningPickDetailDto.GroupBy(x => x.SkuSysId).Count();
                        }
                    }
                    #endregion

                    #region 检查之前的装箱明细 
                    if (actionType == PublicConst.VanningActionType)
                    {
                        //var vanningdetailList =
                        //    _crudRepository.GetQuery<vanningdetail>(x => x.VanningSysId == vanning.SysId && x.Status != (int)VanningStatus.Cancel);

                        var vanningdetailList = vdList.Where(x => x.VanningSysId == vanning.SysId);
                        if (vanningdetailList.Any())
                        {
                            foreach (var info in vanningdetailList)
                            {
                                var vanningPickDetailList =
                                    vanningDetailOperationByOutbound.Where(
                                        x => x.ContainerNumber == info.ContainerNumber);
                                if (!vanningPickDetailList.Any())
                                {
                                    var vanningPickDetail =
                                        _crudRepository.GetQuery<vanningpickdetail>(
                                            x => x.VanningDetailSysId == info.SysId);

                                    foreach (var pickDetailInfo in vanningPickDetail)
                                    {
                                        updateInventoryDtos.Add(
                                            VanningConversion(pickDetailInfo, pickDetailList, vanning, info,
                                                currentUserName, currentUserId));

                                    }
                                    info.Status = (int)VanningStatus.Finish;
                                    _crudRepository.Update(info);
                                }
                            }
                        }
                    }
                    #endregion

                    //var outbound = _crudRepository.Get<outbound>(vanning.OutboundSysId.Value);
                    var outbound = outboundList.Where(x => x.SysId == vanning.OutboundSysId.Value).FirstOrDefault();

                    vanningDetailDto.ExternOrderId = outbound.ExternOrderId;
                    vanningDetailDto.ConsigneePhone = outbound.ConsigneePhone;
                    vanningDetailDto.OutboundType = outbound.OutboundType;

                    // 全部装箱后  触发
                    if (actionType == PublicConst.VanningActionType)
                    {
                        pickDetailList.ForEach(item =>
                        {
                            _crudRepository.Update(item);
                        });
                        outbound.Status = (int)OutboundStatus.Picking;
                        outbound.TotalAllocatedQty = outbound.TotalAllocatedQty - totalQty;
                        outbound.TotalPickedQty = outbound.TotalPickedQty + totalQty;
                        outbound.UpdateBy = 1;
                        outbound.UpdateDate = DateTime.Now;
                        _crudRepository.Update(outbound);

                        #region 组织推送拣货完成工单数据
                        if (outbound != null)
                        {
                            var mqWorkDto = new MQWorkDto()
                            {
                                WorkBusinessType = (int)WorkBusinessType.Update,
                                WorkType = (int)UserWorkType.Picking,
                                WarehouseSysId = wareHouseSysId,
                                CurrentUserId = currentUserId,
                                CurrentDisplayName = currentUserName,
                                CancelWorkDto = new CancelWorkDto()
                                {
                                    DocSysIds = new List<Guid>() { outbound.SysId },
                                    Status = (int)WorkStatus.Finish
                                }
                            };

                            var workProcessDto = new MQProcessDto<MQWorkDto>()
                            {
                                BussinessSysId = outbound.SysId,
                                BussinessOrderNumber = outbound.OutboundOrder,
                                Descr = "",
                                CurrentUserId = currentUserId,
                                CurrentDisplayName = currentUserName,
                                WarehouseSysId = wareHouseSysId,
                                BussinessDto = mqWorkDto
                            };
                            //推送工单数据
                            RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, workProcessDto);
                        }
                        #endregion
                    }
                }
                if (updateInventoryDtos.Any())
                {
                    _wmsSqlRepository.UpdateInventoryPickedByAllocatedQty(updateInventoryDtos);
                }

            }
            else
            {
                throw new Exception("装箱数据异常,请检查接口");
            }

            return vanningDetailDto;
        }

        /// <summary>
        /// 根据装箱记录进行库存统一扣减对象 组织
        /// </summary>
        /// <param name="vanningPickDetail"></param>
        /// <param name="pickDetailList"></param>
        /// <param name="vanning"></param>
        /// <param name="vanningDetail"></param>
        private UpdateInventoryDto VanningConversion(vanningpickdetail vanningPickDetail, List<pickdetail> pickDetailList, vanning vanning, vanningdetail vanningDetail, string currentUserName, int currentUserId)
        {

            var updateInventoryDto = new UpdateInventoryDto();

            #region invlotloclpn 
            var invList = _inventoryRepository.GetlotloclpnDto(vanningPickDetail.SkuSysId, vanningPickDetail.Lot, vanningPickDetail.Loc, vanningPickDetail.Lpn, vanning.WarehouseSysId);

            if (!invList.Any() || invList.Count > 1)
            {
                throw new Exception("库存出现异常");
            }
            var inv = invList.FirstOrDefault();
            var invlotloclpn = _inventoryRepository.Get<invlotloclpn>(inv.InvLotLocLpnSysId);
            var invLot = _inventoryRepository.Get<invlot>(inv.InvLotSysId);
            if (invlotloclpn.AllocatedQty == 0 || invlotloclpn.AllocatedQty < vanningPickDetail.Qty.Value)
            {
                throw new Exception("库存分配数量异常，操作失败");
            }

            updateInventoryDto.InvLotLocLpnSysId = inv.InvLotLocLpnSysId;
            updateInventoryDto.InvLotSysId = inv.InvLotSysId;
            updateInventoryDto.InvSkuLocSysId = inv.InvSkuLocSysId;
            updateInventoryDto.Qty = vanningPickDetail.Qty.Value;
            updateInventoryDto.CurrentDisplayName = currentUserName;
            updateInventoryDto.CurrentUserId = currentUserId;

            #endregion

            var pickDetail = pickDetailList.FirstOrDefault(x => x.SysId == vanningPickDetail.PickDetailSysId);
            pickDetail.PickDate = DateTime.Now;
            pickDetail.Status = (int)PickDetailStatus.Finish;
            pickDetail.UpdateBy = 1;
            pickDetail.UpdateDate = DateTime.Now;

            var pack = _crudRepository.Get<pack>(vanningPickDetail.PackSysId.Value);
            var sku = _crudRepository.Get<sku>(vanningPickDetail.SkuSysId);
            var uom = _crudRepository.Get<uom>(vanningPickDetail.UOMSysId.Value);
            #region InvTrans

            var invTrans = new invtran()
            {
                SysId = Guid.NewGuid(),
                WareHouseSysId = vanning.WarehouseSysId,
                DocOrder = vanning.VanningOrder, //装箱单
                DocSysId = vanningDetail.SysId,  // 箱子SysId
                DocDetailSysId = vanningPickDetail.SysId, //箱内拣货SysId,
                SkuSysId = vanningPickDetail.SkuSysId,
                SkuCode = sku.SkuCode,
                TransType = InvTransType.Outbound,
                SourceTransType = InvSourceTransType.Picking,
                Qty = vanningPickDetail.Qty.Value * -1,
                Loc = vanningPickDetail.Loc,
                Lot = vanningPickDetail.Lot,
                Lpn = vanningPickDetail.Lpn,
                ToLoc = vanningPickDetail.Loc,
                ToLot = vanningPickDetail.Lot,
                ToLpn = vanningPickDetail.Lpn,
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
                PackSysId = vanningPickDetail.PackSysId.Value,
                PackCode = pack != null ? pack.PackCode : "",
                UOMSysId = vanningPickDetail.UOMSysId.Value,
                UOMCode = uom != null ? uom.UOMCode : "",
                CreateBy = currentUserId,
                CreateDate = DateTime.Now,
                UpdateBy = currentUserId,
                UpdateDate = DateTime.Now,
                CreateUserName = currentUserName,
                UpdateUserName = currentUserName
            };
            _crudRepository.Insert(invTrans);

            #endregion

            return updateInventoryDto;
        }

        public Pages<HandoverGroupDto> GetHandoverGroupByPage(HandoverGroupQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            return _crudRepository.GetHandoverGroupByPage(request);
        }

        public HandoverGroupDto GetHandoverGroupByOrder(string handoverGroupOrder, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            HandoverGroupDto response = _crudRepository.GetHandoverGroupByOrder(handoverGroupOrder);

            response.HandoverGroupDetailList = _crudRepository.GetHandoverGroupDetailByOrder(handoverGroupOrder);

            return response;
        }

        /// <summary>
        /// 获取装箱单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public VanningViewDto GetVanningViewById(VanningViewQuery vanningViewQuery)
        {
            _crudRepository.ChangeDB(vanningViewQuery.WarehouseSysId);
            var vanning = _crudRepository.FirstOrDefault<vanning>(vanningViewQuery.VanningSysIdSearch);
            if (vanning != null)
            {
                outbound outbound = _crudRepository.GetAll<outbound>().FirstOrDefault(p => p.SysId == vanning.OutboundSysId);
                return new VanningViewDto
                {
                    VanningOrder = vanning.VanningOrder,
                    VanningType = vanning.VanningType,
                    Status = vanning.Status,
                    OutboundOrder = outbound != null ? outbound.OutboundOrder : string.Empty,
                    VanningDate = vanning.VanningDate,
                    VanningDetailViewDtoList = _vanningRepository.GetVanningDetailViewListByPaging(vanningViewQuery)
                };
            }
            return new VanningViewDto { VanningDetailViewDtoList = new Pages<VanningDetailViewDto> { TableResuls = new TableResults<VanningDetailViewDto> { aaData = new List<VanningDetailViewDto>() } } };
        }

        /// <summary>
        /// 根据装箱明细SysId获取装箱SysId
        /// </summary>
        /// <param name="vanningDetailSysId"></param>
        /// <returns></returns>
        public Guid? GetVanningSysIdByVanningDetailSysId(Guid vanningDetailSysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.Get<vanningdetail>(vanningDetailSysId).VanningSysId;
        }

        #region 取消装箱
        /// <summary>
        /// 取消装箱
        /// </summary>
        /// <param name="vanningCancelDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public CommonResponse CancelVanning(VanningCancelDto vanningCancelDto)
        {
            _crudRepository.ChangeDB(vanningCancelDto.WarehouseSysId);
            var rsp = new CommonResponse() { IsSuccess = false };
            var vanningOrder = string.Empty;
            
            try
            {
                var updateInvTransList = new List<invtran>();
                var updateInventoryDtos = new List<UpdateInventoryDto>();
                

                var vanning = _crudRepository.Get<vanning>(vanningCancelDto.VanningSysId);
                if (vanning == null)
                {
                    throw new Exception("装箱单不存在");
                }
                if (vanning.Status != (int)VanningStatus.Finish && vanning.Status != (int)VanningStatus.Vanning)
                {
                    throw new Exception("装箱单状态不等于装箱中或装箱完成，无法进行取消装箱");
                }

                var status = vanning.Status;
                vanningOrder = vanning.VanningOrder;
                vanning.Status = (int)VanningStatus.Cancel;
                vanning.UpdateBy = vanningCancelDto.CurrentUserId;
                vanning.UpdateDate = DateTime.Now;
                vanning.UpdateUserName = vanningCancelDto.CurrentDisplayName;

                if(status == (int)VanningStatus.Vanning)
                {
                    SaveCancelVanningSaveChange(vanning, vanningCancelDto);
                }
                else
                {
                    #region 装箱明细
                    var vanningDetails = _crudRepository.GetQuery<vanningdetail>(x => x.VanningSysId == vanning.SysId && x.Status != (int)VanningStatus.Cancel);

                    if (vanningDetails != null && vanningDetails.Count() > 0)
                    {
                        List<Guid> vanningDetailSysIds = vanningDetails.Select(p => p.SysId).ToList();

                        #region 装箱拣货明细
                        var vanningPickDetails = _crudRepository.GetQuery<vanningpickdetail>(x => vanningDetailSysIds.Contains((Guid)x.VanningDetailSysId));
                        if (vanningPickDetails != null && vanningPickDetails.Count() > 0)
                        {
                            var skusysid = vanningPickDetails.Select(x => x.SkuSysId).ToList();

                            var list = _inventoryRepository.GetlotloclpnDto(skusysid, vanning.WarehouseSysId);
                            foreach (var info in vanningPickDetails)
                            {
                                var inv = list.Where(x => x.SkuSysId == info.SkuSysId && x.Lot == info.Lot && x.Loc == info.Loc).FirstOrDefault();
                                updateInventoryDtos.Add(new UpdateInventoryDto()
                                {
                                    InvLotLocLpnSysId = inv.InvLotLocLpnSysId,
                                    InvLotSysId = inv.InvLotSysId,
                                    InvSkuLocSysId = inv.InvSkuLocSysId,
                                    Qty = (int)info.Qty,
                                    CurrentUserId = vanningCancelDto.CurrentUserId,
                                    CurrentDisplayName = vanningCancelDto.CurrentDisplayName,
                                });
                            }
                        }
                        else
                        {
                            throw new Exception("未找到装箱拣货明细");
                        }
                        #endregion

                        #region 交易
                        var invTransList = _crudRepository.GetQuery<invtran>(x => vanningDetailSysIds.Contains(x.DocSysId) && x.Status == InvTransStatus.Ok);

                        if (invTransList != null && invTransList.Count() > 0)
                        {
                            foreach (var invTrans in invTransList)
                            {
                                invTrans.Status = InvTransStatus.Cancel;
                                invTrans.UpdateBy = vanningCancelDto.CurrentUserId;
                                invTrans.UpdateDate = DateTime.Now;
                                invTrans.UpdateUserName = vanningCancelDto.CurrentDisplayName;
                                updateInvTransList.Add(invTrans);
                            }
                        }
                        else
                        {
                            throw new Exception("未找到交易记录");
                        }
                        #endregion
                    }
                    else
                    {
                        throw new Exception("未找到装箱明细");
                    }
                    #endregion

                    SaveCancelVanningByFinishSaveChange(vanning, updateInvTransList, updateInventoryDtos, vanningCancelDto);
                }

                #region 组织推送取消拣货工单数据
                if (vanning != null)
                {
                    var mqWorkDto = new MQWorkDto()
                    {
                        WorkBusinessType = (int)WorkBusinessType.Update,
                        WorkType = (int)UserWorkType.Picking,
                        WarehouseSysId = vanningCancelDto.WarehouseSysId,
                        CurrentUserId = vanningCancelDto.CurrentUserId,
                        CurrentDisplayName = vanningCancelDto.CurrentDisplayName,
                        CancelWorkDto = new CancelWorkDto()
                        {
                            DocSysIds = new List<Guid>() { (Guid)vanning.OutboundSysId },
                            Status = (int)WorkStatus.Cancel
                        }
                    };

                    var workProcessDto = new MQProcessDto<MQWorkDto>()
                    {
                        BussinessSysId = (Guid)vanning.SysId,
                        BussinessOrderNumber = vanning.VanningOrder,
                        Descr = "",
                        CurrentUserId = vanningCancelDto.CurrentUserId,
                        CurrentDisplayName = vanningCancelDto.CurrentDisplayName,
                        WarehouseSysId = vanningCancelDto.WarehouseSysId,
                        BussinessDto = mqWorkDto
                    };
                    //推送工单数据
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, workProcessDto);
                }
                #endregion

                rsp.IsSuccess = true;
                rsp.ErrorMessage = string.Format("取消装箱成功，单号：{0}", vanningOrder);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var message = string.Format("取消装箱失败，单号：{0}，失败原因：数据重复并发提交", vanningOrder);
                rsp.IsSuccess = false;
                rsp.ErrorMessage = message;
                throw new Exception(message);
            }
            catch (Exception ex)
            {
                var message = string.Format("取消装箱失败，失败原因：{0}", ex.Message);
                rsp.IsSuccess = false;
                rsp.ErrorMessage = message;
                throw new Exception(message);
            }
            return rsp;
        }

        /// <summary>
        /// 取消装箱方法执行（装箱完成）
        /// </summary>
        /// <param name="vanning"></param>
        /// <param name="invTransList"></param>
        /// <param name="updateInventoryDtos"></param>
        /// <param name="vanningCacelDto"></param>
        [UnitOfWork(isTransactional: false)]
        private void SaveCancelVanningByFinishSaveChange(vanning vanning, List<invtran> invTransList, List<UpdateInventoryDto> updateInventoryDtos, VanningCancelDto vanningCancelDto)
        {
            try
            {
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    //修改装箱单
                    _crudRepository.Update(vanning);
                    
                    //修改交易
                    if(invTransList != null && invTransList.Count > 0)
                    {
                        foreach(var info in invTransList)
                        {
                            _crudRepository.Update(info);
                        }
                    }

                    _crudRepository.CancelVanning(vanning, vanningCancelDto.CurrentUserId, vanningCancelDto.CurrentDisplayName);

                    _wmsSqlRepository.UpdateInventoryCancelVanning(updateInventoryDtos);

                    _crudRepository.SaveChange();
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 取消装箱方法执行（装箱中）
        /// </summary>
        /// <param name="vanning"></param>
        /// <param name="invTransList"></param>
        /// <param name="updateInventoryDtos"></param>
        /// <param name="vanningCacelDto"></param>
        [UnitOfWork(isTransactional: false)]
        private void SaveCancelVanningSaveChange(vanning vanning, VanningCancelDto vanningCancelDto)
        {
            try
            {
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    //修改装箱单
                    _crudRepository.Update(vanning);

                    //修改装箱明细
                    _crudRepository.CancelVanningDetail(vanning, vanningCancelDto.CurrentUserId, vanningCancelDto.CurrentDisplayName);

                    _crudRepository.SaveChange();
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion
    }
}