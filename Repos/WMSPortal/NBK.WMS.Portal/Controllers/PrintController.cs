using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Services;
using NBK.ECService.WMS.Utility;

namespace NBK.WMS.Portal.Controllers
{
    public class PrintController : BaseController
    {
        // GET: Print
        public ActionResult PrintPickDetailByOrder(string pickDetailOrder, string userName, Guid warehouseSysId)
        {
            ViewBag.PrintUserName = userName;
            var rsp = PrintApiClient.GetInstance().GetPrintPickDetailByOrderDto(pickDetailOrder, warehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                var d = new PrintPickDetailByOrderDto();
                d.PrintPickDetailDtos = new List<PrintPickDetailDto>();
                return View(d);
            }

        }

        public ActionResult PrintPickDetailByBatch(string pickDetailOrder, string userName, Guid warehouseSysId)
        {
            ViewBag.PrintUserName = userName;
            var rsp = PrintApiClient.GetInstance().GetPrintPickDetailByBatchDto(pickDetailOrder, warehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                var d = new PrintPickDetailByBatchDto();
                d.PrintPickDetailDtos = new List<PrintPickDetailDto>();
                return View(d);
            }
        }

        public ActionResult PrintRecommendPickDetail(string outboundSysId, string userName, Guid warehouseSysId)
        {
            ViewBag.PrintUserName = userName;
            var rsp = PrintApiClient.GetInstance().GetPrintRecommendPickDetail(outboundSysId, warehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                var d = new PrintPickDetailByOrderDto();
                d.PrintPickDetailDtos = new List<PrintPickDetailDto>();
                return View(d);
            }
        }

        public ActionResult PrintReceipt(string receiptOrder, Guid warehouseSysId)
        {
            var rsp = PrintApiClient.GetInstance().GetPrintReceiptDto(receiptOrder, warehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                var d = new PrintReceiptDto();
                d.PrintReceiptDetailList = new List<PrintReceiptDetailDto>();
                return View(d);
            }
        }

        public ActionResult PrintPurchaseView(string sysId, Guid warehouseSysId)
        {
            var rsp = PrintApiClient.GetInstance().GetPrintPurchaseViewDto(LoginCoreQuery, sysId, warehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                var model = new PrintPurchaseViewDto { PrintPurchaseDetailViewDtoList = new List<PrintPurchaseDetailViewDto>() };
                return View(model);
            }
        }

        //public ActionResult PrintVanningDetail(string sysId)
        //{
        //    var rsp = PrintApiClient.GetInstance().GetPrintVanningDetailDto(sysId,LoginCoreQuery);
        //    if (rsp.Success)
        //    {
        //        return View(rsp.ResponseResult);
        //    }
        //    else
        //    {
        //        var model = new PrintVanningDetailDto();
        //        model.PrintVanningDetailSkuDtoList = new List<PrintVanningDetailSkuDto>();
        //        return View(model);
        //    }
        //}

        /// <summary>
        /// 打印装箱单
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="printUserName"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public ActionResult PrintVanningPackingDetail(string sysId, string printUserName, string actionType, Guid warehouseSysId)
        {
            ViewData["PrintUserName"] = printUserName;
            ViewBag.ActionType = actionType;
            var rsp = PrintApiClient.GetInstance().GetPrintVanningDetailDto(sysId, actionType, LoginCoreQuery, warehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                var model = new PrintVanningDetailDto();
                model.PrintVanningDetailSkuDtoList = new List<PrintVanningDetailSkuDto>();
                return View(model);
            }
        }

        /// <summary>
        /// 打印装箱单(B2B)
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="printUserName"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public ActionResult PrintVanningPackingDetailToB(string sysId, string printUserName, string actionType, Guid warehouseSysId)
        {
            ViewData["PrintUserName"] = printUserName;
            var rsp = PrintApiClient.GetInstance().GetPrintVanningDetailDto(sysId, actionType, LoginCoreQuery, warehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                var model = new PrintVanningDetailDto();
                model.PrintVanningDetailSkuDtoList = new List<PrintVanningDetailSkuDto>();
                return View(model);
            }
        }

        /// <summary>
        /// 打印箱贴
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult PrintBoxLable(string sysId, string actionType, Guid warehouseSysId)
        {
            ViewBag.ActionType = actionType;
            var rsp = PrintApiClient.GetInstance().GetPrintVanningDetailStick(sysId, actionType, LoginCoreQuery, warehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                var model = rsp.ResponseResult;
                model.PrintMallName = PublicConst.PrintMallName;
                model.PrintMallHttpUrl = PublicConst.PrintMallHttpUrl;
                model.PrintMallPhone = PublicConst.PrintMallPhone;
                return View(model);
            }
            else
            {
                var model = new PrintVanningDetailStickDto();
                return View(model);
            }
        }


        public ActionResult PrintBoxLableZTO(string sysId, string actionType, Guid warehouseSysId)
        {
            ViewBag.ActionType = actionType;
            var rsp = PrintApiClient.GetInstance().GetPrintVanningDetailStick(sysId, actionType, LoginCoreQuery, warehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                var model = rsp.ResponseResult;
                model.PrintMallName = PublicConst.PrintMallName;
                model.PrintMallHttpUrl = PublicConst.PrintMallHttpUrl;
                model.PrintMallPhone = PublicConst.PrintMallPhone;
                return View(model);
            }
            else
            {
                var model = new PrintVanningDetailStickDto();
                return View(model);
            }
        }

        /// <summary>
        /// 打印箱贴ToB
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public ActionResult PrintBoxLableToB(string sysId, string actionType, Guid warehouseSysId)
        {
            ViewBag.ActionType = actionType;
            var rsp = PrintApiClient.GetInstance().GetPrintVanningDetailStickToB(sysId, actionType, LoginCoreQuery, warehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                var model = rsp.ResponseResult;
                model.PrintMallName = PublicConst.PrintMallName;
                model.PrintMallHttpUrl = PublicConst.PrintMallHttpUrl;
                model.PrintMallPhone = PublicConst.PrintMallPhone;

                var row = model.PrintVanningDetailStickSkuDto.Count();

                ViewBag.MarginTop = GetPrintMarginTop(row);

                return View(model);
            }
            else
            {
                var model = new PrintVanningDetailStickDto();
                return View(model);
            }
        }

        #region B2B箱贴计算
        /// <summary>
        /// B2B箱贴计算
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public int GetPrintMarginTop(int row)
        {
            var marginTop = 30;    //默认和上面间距30px
            var totalRow = 28 * row + 45;
            var firstPage = 310 + 270;
            var totalHeight = 100 + 310 + 270;
            var topHeight = 100 + 310;

            //大于第一页时
            if (totalRow > firstPage)
            {
                var page = 0;
                if ((totalRow - firstPage) % totalHeight == 0)
                {
                    page = Convert.ToInt32((totalRow - firstPage) / totalHeight);
                }
                else
                {
                    page = Convert.ToInt32((totalRow - firstPage) / totalHeight) + 1;
                }

                var remain = totalRow - firstPage - (totalHeight * (page - 1));
                if (remain > topHeight)
                {
                    marginTop += totalHeight - remain + topHeight;
                }
                else
                {
                    marginTop += topHeight - remain;
                }
                marginTop += 5 * page;
            }
            else
            {
                if (totalRow > 310)
                {
                    marginTop += (firstPage - totalRow) + topHeight;
                }
                else
                {
                    marginTop += 310 - totalRow;
                }
            }
            return marginTop;
        }
        #endregion

        public ActionResult PrintHandoverGroup(string handoverGroupOrder, Guid warehouseSysId)
        {
            var response = OutboundApiClient.GetInstance().GetHandoverGroupByOrder(handoverGroupOrder, warehouseSysId, this.LoginCoreQuery);
            var model = new HandoverGroupDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }

            return View(model);
        }

        #region 出库单

        /// <summary>
        /// 打印出库单
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="wareHouseSysId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ActionResult PrintOutbound(Guid outboundSysId, string userName, Guid warehouseSysId)
        {
            var response = PrintApiClient.GetInstance().GetPrintOutboundDto(outboundSysId, this.LoginCoreQuery, warehouseSysId);
            var model = new PrintOutboundDto();
            if (response.Success)
            {
                model = response.ResponseResult;
                model.OutboundOrder = response.ResponseResult.OutboundOrder;
                model.UserName = userName;
                model.OperateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }

            return View(model);
        }

        /// <summary>
        /// 获取打印预包装差异数据
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ActionResult PrintOutboundPrePackDiff(Guid outboundSysId, string userName, Guid warehouseSysId)
        {
            var response = PrintApiClient.GetInstance().GetPrintOutboundPrePackDiffDto(LoginCoreQuery, outboundSysId, warehouseSysId);
            var model = new PrintOutboundPrePackDiffDto();
            if (response.Success)
            {
                model = response.ResponseResult;
                model.OutboundOrder = response.ResponseResult.OutboundOrder;
                model.UserName = userName;
                model.OperateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
            return View(model);
        }

        public ActionResult PrintTMSPackNumberList(Guid outboundSysId, string userName, Guid warehouseSysId)
        {
            var response = PrintApiClient.GetInstance().GetPrintTMSPackNumberList(outboundSysId, this.LoginCoreQuery, warehouseSysId);
            var model = new List<PrintTMSPackNumberListDto>();
            if (response.Success)
            {
                model = response.ResponseResult;
            }
            return View(model);
        }

        #endregion

        /// <summary>
        /// 获取打印散货箱差异数据
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ActionResult PrintOutboundPreBulkPackDiff(Guid outboundSysId, string userName, Guid warehouseSysId)
        {
            var response = PrintApiClient.GetInstance().GetPrintOutboundPreBulkPackDiffDto(LoginCoreQuery, outboundSysId, warehouseSysId);
            var model = new PrintOutboundPrePackDiffDto();
            if (response.Success)
            {
                model = response.ResponseResult;
                model.OutboundOrder = response.ResponseResult.OutboundOrder;
                model.UserName = userName;
                model.OperateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
            return View(model);
        }

        public ActionResult PrintAssemblyRCMDPickDetail(string assemblySysId, string userName, Guid warehouseSysId)
        {
            ViewBag.PrintUserName = userName;
            var rsp = PrintApiClient.GetInstance().GetPrintAssemblyRCMDPickDetail(LoginCoreQuery, assemblySysId, warehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                var d = new PrintAssemblyRCMDPickDetailDto();
                d.PrintPickDetailDtos = new List<PrintAssemblyPickDetailDto>();
                return View(d);
            }
        }

        public ActionResult PrintAssembly(Guid assemblySysId, Guid warehouseSysId)
        {
            var rsp = VASApiClient.GetInstance().GetAssemblyViewDtoById(LoginCoreQuery, assemblySysId, warehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                var d = new AssemblyViewDto();
                d.AssemblyDetails = new List<AssemblyDetailDto>();
                return View(d);
            }
        }

        /// <summary>
        /// 打印预包装单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult PrintPrePack(Guid sysId, Guid warehouseSysId)
        {
            var response = OrderManegerApiClient.GetInstance().GetPrePackBySysId(sysId, warehouseSysId);
            var model = new PrePackSkuDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            };
            return View(model);

        }

        public ActionResult PrintPrebulkPackView(Guid sysId, Guid warehouseSysId)
        {
            var response = OrderManegerApiClient.GetInstance().GetPreBulkPackBySysId(sysId, warehouseSysId, this.LoginCoreQuery);
            var model = new PreBulkPackDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            };
            return View(model);
        }

        public ActionResult PrintReceiptAutoShelves(Guid sysId, Guid warehouseSysId)
        {
            var response = PrintApiClient.GetInstance().PrintReceiptAutoShelves(this.LoginCoreQuery, sysId, warehouseSysId);
            var model = new PrintAutoShelvesDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            };
            return View(model);
        }

        public ActionResult PrintStockTake(Guid sysId, Guid warehouseSysId)
        {
            var rsp = PrintApiClient.GetInstance().GetPrintStockTakeDto(LoginCoreQuery, sysId, warehouseSysId);
            var model = new PrintStockTakeDto();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                model = rsp.ResponseResult;
            }
            return View(model);
        }

        public ActionResult PrintStockTakeReport(string sysIds, Guid warehouseSysId)
        {
            var rsp = PrintApiClient.GetInstance().GetPrintStockTakeReportDto(LoginCoreQuery, sysIds.ToGuidList(), warehouseSysId);
            var model = new PrintStockTakeReportDto();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                model = rsp.ResponseResult;
            }
            return View(model);
        }

        public ActionResult PrintQualityControl(Guid sysId, Guid warehouseSysId)
        {
            var rsp = PrintApiClient.GetInstance().GetPrintQualityControlDto(LoginCoreQuery, sysId, warehouseSysId);
            var model = new PrintQualityControlDto();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                model = rsp.ResponseResult;
            }
            return View(model);
        }

        public ActionResult PrintPrebulkPackByBatch(string sysIds, Guid warehouseSysId)
        {
            var rsp = PrintApiClient.GetInstance().GetPrintPrebulkPackByBatchDto(LoginCoreQuery, sysIds.ToGuidList(), warehouseSysId);
            var model = new PrintPrebulkPackByBatchDto { PrintPreBulkPackDtoList = new List<PreBulkPackDto>() };
            if (rsp.Success && rsp.ResponseResult != null)
            {
                model = rsp.ResponseResult;
            }
            return View(model);
        }

        /// <summary>
        /// 打印领料分拣单
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <param name="pickingUserName"></param>
        /// <returns></returns>
        public ActionResult PrintPickingMaterial(Guid receiptSysId, string pickingUserName, string sysIds, Guid warehouseSysId)
        {
            var sysIdList = !string.IsNullOrEmpty(sysIds) ? sysIds.Split('|') : null;
            var request = new PrintPickingMaterialQuery() { ReceiptSysId = receiptSysId, PickingUserName = pickingUserName.Trim(), SysIds = sysIdList != null ? sysIdList.ToList() : null, WarehouseSysId = warehouseSysId };
            var rsp = PrintApiClient.GetInstance().GetPrintPickingMaterialDetailList(LoginCoreQuery, request);
            var model = new PrintPickingMaterialDto();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                model = rsp.ResponseResult;
            }
            return View(model);
        }

        /// <summary>
        /// 交接管理_交接明细打印
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public ActionResult PrintOutboundTransferOrder(Guid sysId, Guid warehouseSysId)
        {
            var response = OrderManegerApiClient.GetInstance().GetDataBySysId(sysId, warehouseSysId, this.LoginCoreQuery);
            var model = new OutboundTransferOrderDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            };
            return View(model);
        }

        /// <summary>
        /// 打印商品外借单
        /// </summary>
        /// <param name="skuBorrowOrder"></param>
        /// <param name="userName"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public ActionResult PrintSkuBorrowDetailByOrder(string skuBorrowOrder, string userName, Guid warehouseSysId)
        {
            ViewBag.PrintUserName = userName;
            var response = VASApiClient.GetInstance().GetSkuBorrowByOrder(skuBorrowOrder, warehouseSysId, this.LoginCoreQuery);
            var model = new SkuBorrowViewDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            };
            return View(model);
        }
    }
}