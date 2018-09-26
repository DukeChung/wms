using System;
using System.Linq;
using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility.Enum;
using Abp.Domain.Uow;

namespace NBK.ECService.WMS.Application
{
    public class PrintAppService : WMSApplicationService, IPrintAppService
    {
        private IPrintRepository _crudRepository = null;
        private IPackageAppService _packageAppService = null;
        private IOutboundAppService _outboundAppService = null;

        public PrintAppService(IPrintRepository crudRepository, IPackageAppService packageAppService, IOutboundAppService outboundAppService)
        {
            this._crudRepository = crudRepository;
            _packageAppService = packageAppService;
            _outboundAppService = outboundAppService;
        }

        /// <summary>
        /// 按订单打印拣货单
        /// </summary>
        /// <param name="pickDetailOrder"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintPickDetailByOrderDto GetPrintPickDetailByOrderDto(string pickDetailOrder, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var pickdetailList = _crudRepository.GetQuery<pickdetail>(x => x.PickDetailOrder == pickDetailOrder).ToList();
            if (pickdetailList.Any())
            {
                var pickDetail = pickdetailList.FirstOrDefault();
                var printPickDetailByOrderDto = _crudRepository.Get<outbound>(pickDetail.OutboundSysId.Value).JTransformTo<PrintPickDetailByOrderDto>();
                printPickDetailByOrderDto.PickDetailOrder = pickDetailOrder;
                printPickDetailByOrderDto.PrintPickDetailDtos = _crudRepository.GetPrintPickDetailByOrderDto(pickDetailOrder);
                return printPickDetailByOrderDto;
            }
            else
            {
                throw new Exception("未找到需要打印的出库单");
            }


        }

        /// <summary>
        /// 批量打印拣货单
        /// </summary>
        /// <param name="pickDetailOrder"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintPickDetailByBatchDto GetPrintPickDetailByBatchDto(string pickDetailOrder, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var pickdetailList = _crudRepository.GetPrintPickDetailByBatchDtoList(pickDetailOrder);
            if (pickdetailList.Any())
            {
                var printPickDetailByBatchDto = new PrintPickDetailByBatchDto();
                printPickDetailByBatchDto.PickDetailOrder = pickDetailOrder;
                printPickDetailByBatchDto.OrderCount = pickdetailList.GroupBy(x => x.OutboundSysId).Count();
                printPickDetailByBatchDto.SkuCount = pickdetailList.GroupBy(x => x.SkuSysId).Count();
                printPickDetailByBatchDto.SkuQty = pickdetailList.Sum(x => x.Qty.Value);

                var query = pickdetailList.GroupBy(x => new { x.SkuSysId, x.UPC, x.SkuName, x.SkuDescr, x.Loc, x.UomName, x.PackFactor, x.Lot, x.Channel, x.OutboundChildType })
                    .Select(x =>
                    new PrintPickDetailDto()
                    {
                        SkuSysId = x.Key.SkuSysId,
                        UPC = x.Key.UPC,
                        SkuName = x.Key.SkuName,
                        SkuDescr = x.Key.SkuDescr,
                        Loc = x.Key.Loc,
                        Lot = x.Key.Lot,
                        UomName = x.Key.UomName,
                        Qty = x.Sum(y => y.Qty),
                        PackFactor = x.Key.PackFactor,
                        Channel = x.Key.Channel,
                        OutboundChildType = x.Key.OutboundChildType
                    });

                printPickDetailByBatchDto.PrintPickDetailDtos = query.ToList();
                return printPickDetailByBatchDto;
            }
            else
            {
                throw new Exception("未找到需要打印拣货单");
            }


        }

        [UnitOfWork(isTransactional: false)]
        public PrintPickDetailByOrderDto GetPrintRecommendPickDetail(Guid outboundSysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            PrintPickDetailByOrderDto response = new PrintPickDetailByOrderDto() { PrintPickDetailDtos = new List<PrintPickDetailDto>() };

            var outbound = _crudRepository.GetQuery<outbound>(p => p.SysId == outboundSysId).FirstOrDefault();
            var outboundDetails = _crudRepository.GetQuery<outbounddetail>(p => p.OutboundSysId == outboundSysId);

            foreach (var item in outboundDetails)
            {
                var recommendPickDetailList = _crudRepository.GetPrintRecommendPickDetail(item.SysId, outbound.WareHouseSysId);
                response.PrintPickDetailDtos.AddRange(recommendPickDetailList);
            }

            response.OutboundOrder = outbound.OutboundOrder;
            //response.SkuCount = response.PrintPickDetailDtos.GroupBy(x => x.SkuSysId).Count();
            //response.SkuQty = response.PrintPickDetailDtos.Sum(x => x.Qty.Value);

            return response;
        }

        /// <summary>
        /// 打印收货单
        /// </summary>
        /// <param name="receiptOrder"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintReceiptDto GetPrintReceiptDto(string receiptOrder, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var receiptDto = _crudRepository.GetPrintReceiptDto(receiptOrder);
            if (receiptDto != null)
            {
                var receiptDetailList = _crudRepository.GetQuery<receiptdetail>(x => x.ReceiptSysId == receiptDto.SysId).ToList();
                if (receiptDetailList.Any())
                {
                    receiptDto.PrintReceiptDetailList = _crudRepository.GetPrintReceiptDetailDtoByPurchaseDetail(receiptDto.ReceiptOrder);
                }
                else
                {
                    receiptDto.PrintReceiptDetailList = _crudRepository.GetPrintReceiptDetailDto(receiptDto.ReceiptOrder);
                }

            }
            return receiptDto;
        }

        /// <summary>
        /// 打印采购单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintPurchaseViewDto GetPrintPurchaseViewDto(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var purchaseViewDto = _crudRepository.GetPrintPurchaseViewDtoBySysId(sysId);
            if (purchaseViewDto != null)
            {
                purchaseViewDto.PrintPurchaseDetailViewDtoList = _crudRepository.GetPrintPurchaseDetailViewBySysId(sysId);

            }
            return purchaseViewDto;
        }

        [UnitOfWork(isTransactional: false)]
        public PrintVanningDetailDto GetPrintVanningDetailDto(Guid vanningDetailSysId, string actionType, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var vanningDetailDto = _crudRepository.GetPrintVanningDetailDto(vanningDetailSysId);
            if (vanningDetailDto != null)
            {
                ///vanningDetailDto.PrintVanningDetailSkuDtoList = _crudRepository.GetPrintVanningDetailSkuDtoList(vanningDetailSysId);

                var list = _crudRepository.GetPrintVanningDetailSkuDtoListNew(vanningDetailDto.OutboundSysId);
                vanningDetailDto.PrintVanningDetailSkuDtoList = new List<PrintVanningDetailSkuDto>();

                var thisList = list.Where(x => x.SysId == vanningDetailSysId).ToList();

                var newList = thisList.GroupBy(x => new { x.OtherId }).Select(x => new PrintVanningDetailSkuDto() { OtherId = x.Key.OtherId }).ToList();
                foreach (var info in newList)
                {
                    var item = list.Where(x => x.SysId == vanningDetailSysId && x.OtherId == info.OtherId).First();
                    item.Qty = list.Where(x => x.SysId == vanningDetailSysId && x.OtherId == info.OtherId).Sum(x => x.Qty);

                    if (item.IsGift || item.GiftQty != 0)
                    {
                        var boxQty = list.Where(x => int.Parse(x.ContainerNumber) < int.Parse(item.ContainerNumber) && x.OtherId == item.OtherId).ToList().Sum(x => x.Qty);
                        if ((item.GiftQty - boxQty) >= item.Qty)
                        {
                            item.TotalPrice = 0;
                            item.Price = 0;
                            item.IsGift = true;
                        }
                        else
                        {
                            if ((item.GiftQty - boxQty) > 0)
                            {
                                if (item.Qty == (item.GiftQty - boxQty))
                                {
                                    var newItem = new PrintVanningDetailSkuDto()
                                    {
                                        OtherId = item.OtherId,
                                        UPC = item.UPC,
                                        SkuName = item.SkuName,
                                        SkuDescr = item.SkuDescr,
                                        UOMCode = item.UOMCode,
                                        Qty = item.GiftQty - boxQty,
                                        Price = 0,
                                        TotalPrice = 0,
                                        IsGift = true
                                    };
                                    vanningDetailDto.PrintVanningDetailSkuDtoList.Add(newItem);

                                    item.Qty = item.Qty - (item.GiftQty - boxQty);
                                    item.TotalPrice = item.Price * item.Qty;
                                }
                                else if (item.Qty > (item.GiftQty - boxQty))
                                {
                                    var newItem = new PrintVanningDetailSkuDto()
                                    {
                                        OtherId = item.OtherId,
                                        UPC = item.UPC,
                                        SkuName = item.SkuName,
                                        SkuDescr = item.SkuDescr,
                                        UOMCode = item.UOMCode,
                                        Qty = item.GiftQty - boxQty,
                                        Price = 0,
                                        TotalPrice = 0,
                                        IsGift = true
                                    };
                                    vanningDetailDto.PrintVanningDetailSkuDtoList.Add(newItem);

                                    item.Qty = item.Qty - newItem.Qty;
                                    item.TotalPrice = item.Price * item.Qty;
                                }
                                else
                                {
                                    item.IsGift = true;
                                    item.Price = 0;
                                    item.TotalPrice = 0;
                                }
                            }
                            else
                            {
                                item.TotalPrice = item.Price * item.Qty;
                            }
                        }
                    }
                    else
                    {
                        item.TotalPrice = item.Qty * item.Price;
                    }
                    vanningDetailDto.PrintVanningDetailSkuDtoList.Add(item);
                }

                #region 最后一箱
                if (actionType == "Finish")
                {
                    var vanningDetailList = _crudRepository.GetPrintVanningSkuList(vanningDetailDto.OutboundSysId);
                    if (vanningDetailList != null && vanningDetailList.Count > 0)
                    {
                        foreach (var item in vanningDetailList)
                        {
                            vanningDetailDto.TotalOrderQty += item.Qty;
                            vanningDetailDto.TotalPrice += item.TotalPrice;
                        }

                        vanningDetailDto.TotalOrderPrice = vanningDetailDto.TotalPrice + vanningDetailDto.Freight + vanningDetailDto.DiscountPrice + vanningDetailDto.CouponPrice;
                    }
                }
                #endregion
            }

            return vanningDetailDto;
        }

        /// <summary>
        /// 获取箱贴数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintVanningDetailStickDto GetPrintVanningDetailStick(Guid sysId, string actionType, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var printVanningDetail = _crudRepository.GetPrintVanningDetailStick(sysId);
            if (printVanningDetail != null)
            {
                var number = _crudRepository.GetQuery<vanningdetail>(x => x.VanningSysId == printVanningDetail.VanningSysId && x.Status != (int)VanningStatus.Cancel).Count();
                if (actionType == PublicConst.VanningActionType)
                {
                    printVanningDetail.ParcelNumber = number + "/" + number;
                }
                else
                {
                    printVanningDetail.ParcelNumber = printVanningDetail.ContainerNumber;
                }
            }
            return printVanningDetail;
        }

        /// <summary>
        /// 获取箱贴数据ToB
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintVanningDetailStickDto GetPrintVanningDetailStickToB(Guid sysId, string actionType, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var printVanningDetail = _crudRepository.GetPrintVanningDetailStickToB(sysId);
            if (printVanningDetail != null)
            {
                var number = _crudRepository.GetQuery<vanningdetail>(x => x.VanningSysId == printVanningDetail.VanningSysId && x.Status != (int)VanningStatus.Cancel).Count();
                if (actionType == PublicConst.VanningActionType)
                {
                    printVanningDetail.ParcelNumber = number + "/" + number;
                }
                else
                {
                    printVanningDetail.ParcelNumber = printVanningDetail.ContainerNumber;
                }
            }
            return printVanningDetail;
        }

        /// <summary>
        /// 获取打印出库单数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintOutboundDto GetPrintOutboundDto(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var response = _crudRepository.GetPrintOutboundDto(sysId);
            //gavin: 原材料单位反转
            if (response.PrintOutboundDetailDto != null && response.PrintOutboundDetailDto.Count > 0)
            {
                response.DisplaySkuQty = response.PrintOutboundDetailDto.Sum(p => p.DisplayShippedQty.Value);
            }


            return response;
        }

        /// <summary>
        /// 获取打印预包装差异数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintOutboundPrePackDiffDto GetPrintOutboundPrePackDiffDto(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var response = _crudRepository.GetPrintOutboundPrePackDiffDto(sysId);
            if (response != null)
            {
                var diffDto = _outboundAppService.GetOutboundPrePackDiff(response.SysId, warehouseSysId);
                response.StorageLoc = diffDto.StorageLoc;
                response.PrintOutboundDetailPrePackDiffDto = diffDto.DetailDiffList;
            }
            return response;
        }


        /// <summary>
        /// 获取打印出库单对应的交接单列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<PrintTMSPackNumberListDto> GetPrintTMSPackNumberList(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.GetPrintTMSPackNumberList(sysId);
        }



        /// <summary>
        /// 获取打印散货箱差异数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintOutboundPrePackDiffDto GetPrintOutboundPreBulkPackDiffDto(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var response = _crudRepository.GetPrintOutboundPrePackDiffDto(sysId);
            if (response != null)
            {
                var diffDto = _outboundAppService.GetOutboundPreBulkPackDiff(response.SysId);
                response.PrintOutboundDetailPrePackDiffDto = diffDto.DetailDiffList;
            }
            return response;
        }

        /// <summary>
        /// 生产加工单推荐拣货货位
        /// </summary>
        /// <param name="assemblySysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintAssemblyRCMDPickDetailDto GetPrintAssemblyRCMDPickDetail(Guid assemblySysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            PrintAssemblyRCMDPickDetailDto rsp = new PrintAssemblyRCMDPickDetailDto() { PrintPickDetailDtos = new List<PrintAssemblyPickDetailDto>() };

            var assembly = _crudRepository.GetQuery<assembly>(p => p.SysId == assemblySysId).FirstOrDefault();
            var assemblyDetails = _crudRepository.GetQuery<assemblydetail>(p => p.AssemblySysId == assemblySysId);

            //增加加工领料规则
            var assemblyRule = _crudRepository.GetQuery<assemblyrule>(x => x.WarehouseSysId == warehouseSysId).FirstOrDefault
                ();
            foreach (var item in assemblyDetails)
            {
                var recommendPickDetailList = _crudRepository.GetPrintAssemblyRCMDPickDetail(item.SysId, assembly.WareHouseSysId, assemblyRule, assembly.Channel);
                rsp.PrintPickDetailDtos.AddRange(recommendPickDetailList);
            }
            rsp.SysId = assembly.SysId;
            rsp.AssemblyOrder = assembly.AssemblyOrder;
            rsp.Channel = assembly.Channel;
            return rsp;
        }


        /// <summary>
        /// 打印自动上架单
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintAutoShelvesDto GetPrintReceiptAutoShelves(Guid receiptSysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var printAutoShelvesDto = new PrintAutoShelvesDto();
            var receipt = _crudRepository.Get<receipt>(receiptSysId);
            printAutoShelvesDto = receipt.JTransformTo<PrintAutoShelvesDto>();
            printAutoShelvesDto.PrintReceiptDetailDtos = _crudRepository.GetPrintReceiptAutoShelvesDetail(receiptSysId);
            return printAutoShelvesDto;
        }

        /// <summary>
        /// 打印盘点单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintStockTakeDto GetPrintStockTakeDto(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            PrintStockTakeDto rsp = new PrintStockTakeDto();
            var stockTake = _crudRepository.GetQuery<stocktake>(p => p.SysId == sysId).FirstOrDefault();
            if (stockTake != null)
            {
                rsp.SysId = stockTake.SysId;
                rsp.AssignUserName = stockTake.AssignUserName;
                rsp.CreateDate = stockTake.CreateDate;
                rsp.PrintStockTakeDetails = _crudRepository.GetPrintStockTakeDetails(sysId);
            }
            else
            {
                throw new Exception("盘点单不存在");
            }
            return rsp;
        }

        /// <summary>
        /// 打印盘点单汇总报告
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintStockTakeReportDto GetPrintStockTakeReportDto(List<Guid> sysIds, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            PrintStockTakeReportDto rsp = new PrintStockTakeReportDto();
            rsp.PrintStockTakeReportDetails = _crudRepository.GetPrintStockTakeReportDetails(sysIds);
            return rsp;
        }

        /// <summary>
        /// 打印质检单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintQualityControlDto GetPrintQualityControlDto(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            PrintQualityControlDto rsp = new PrintQualityControlDto();
            var qualityControl = _crudRepository.GetQuery<qualitycontrol>(p => p.SysId == sysId).FirstOrDefault();
            if (qualityControl != null)
            {
                rsp.QCOrder = qualityControl.QCOrder;
                rsp.Status = qualityControl.Status;
                rsp.QCType = qualityControl.QCType;
                rsp.DocOrder = qualityControl.DocOrder;
                rsp.QCUserName = qualityControl.QCUserName;
                rsp.QCDate = qualityControl.QCDate;
                rsp.Descr = qualityControl.Descr;
                rsp.PrintQualityControlDetails = _crudRepository.GetPrintQualityControlDetails(sysId);
            }
            else
            {
                throw new Exception("质检单不存在");
            }
            return rsp;
        }

        /// <summary>
        /// 批量打印散货封箱单
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public PrintPrebulkPackByBatchDto GetPrintPrebulkPackByBatchDto(List<Guid> sysIds, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.GetPrintPrebulkPackByBatchDto(sysIds);
        }

        /// <summary>
        /// 获取打印领料数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <summary>
        [UnitOfWork(isTransactional: false)]
        public PrintPickingMaterialDto GetPrintPickingMaterialDetailList(PrintPickingMaterialQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var picking = new PrintPickingMaterialDto();
            try
            {
                var detailList = _crudRepository.GetPrintPickingMaterialDetailList(request);
                if (detailList != null && detailList.Count > 0)
                {
                    picking = detailList.GroupBy(x => new { x.PickingDate, x.ReceiptSysId, x.ReceiptOrder, x.PickingUserName })
                        .Select(x => new PrintPickingMaterialDto
                        {
                            ReceiptSysId = x.Key.ReceiptSysId,
                            ReceiptOrder = x.Key.ReceiptOrder,
                            PickingDate = x.Key.PickingDate,
                            PickingUserName = x.Key.PickingUserName
                        }).FirstOrDefault();
                    picking.PrintPickingMaterialDetailDtoList = _crudRepository.GetPrintPickingMaterialDetailList(request);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return picking;
        }
    }
}