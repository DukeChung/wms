using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Services;
using NBK.AuthServiceUtil;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class QualityControlController : BaseController
    {
        [PermissionAuthorize("VAS_QualityControl")]
        public ActionResult QualityControlList()
        {
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem { Text = QCStatus.New.ToDescription(), Value = ((int)QCStatus.New).ToString() });
            statusList.Add(new SelectListItem { Text = QCStatus.Finish.ToDescription(), Value = ((int)QCStatus.Finish).ToString() });
            ViewBag.StatusList = statusList;

            var qcTypeList = new List<SelectListItem>();
            qcTypeList.Add(new SelectListItem { Text = QCType.PurchaseQC.ToDescription(), Value = ((int)QCType.PurchaseQC).ToString() });
            ViewBag.QCTypeList = qcTypeList;

            return View();
        }

        public ActionResult GetQualityControlList(QualityControlQuery qualityControlQuery)
        {
            var rsp = VASApiClient.GetInstance().GetQualityControlList(LoginCoreQuery, qualityControlQuery);
            if (rsp.Success && rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult DeleteQualityControl(string sysIds)
        {
            var rsp = VASApiClient.GetInstance().DeleteQualityControl(LoginCoreQuery, sysIds.ToGuidList(),CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QualityControlView(Guid sysId)
        {
            ViewBag.PrintName = PublicConst.PrintSettingZS;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            QualityControlDto model = new QualityControlDto();
            var rsp = VASApiClient.GetInstance().GetQualityControlViewDto(LoginCoreQuery, sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                model = rsp.ResponseResult;
            }
            return View(rsp.ResponseResult);
        }

        public ActionResult GetDocDetails(DocDetailQuery docDetailQuery)
        {
            var rsp = VASApiClient.GetInstance().GetDocDetails(LoginCoreQuery, docDetailQuery);
            if (rsp.Success && rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetQCDetails(QCDetailQuery qcDetailQuery)
        {
            var rsp = VASApiClient.GetInstance().GetQCDetails(LoginCoreQuery, qcDetailQuery);
            if (rsp.Success && rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult SaveQualityControl(SaveQualityControlDto saveQualityControlDto)
        {
            var rsp = VASApiClient.GetInstance().SaveQualityControl(LoginCoreQuery, saveQualityControlDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FinishQualityControl(FinishQualityControlDto finishQualityControlDto)
        {
            var rsp = VASApiClient.GetInstance().FinishQualityControl(LoginCoreQuery, finishQualityControlDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateAdjustment(string qcOrder, Guid warehouseSysId, string detailSysIds)
        {
            var rsp1 = VASApiClient.GetInstance().GetAdjustmentDto(LoginCoreQuery, new CreateAdjustmentDto
            {
                SourceOrder = qcOrder,
                WarehouseSysId = warehouseSysId,
                DetailSysIds = detailSysIds.ToGuidList()
            });
            if (rsp1.Success && rsp1.ResponseResult != null)
            {
                rsp1.ResponseResult.CreateBy = CurrentUser.UserId;
                rsp1.ResponseResult.CreateUserName = CurrentUser.DisplayName;
                rsp1.ResponseResult.UpdateBy = CurrentUser.UserId;
                rsp1.ResponseResult.UpdateUserName = CurrentUser.DisplayName;
                rsp1.ResponseResult.AdjustmentDetailList.ForEach(p =>
                {
                    p.CreateBy = CurrentUser.UserId;
                    p.CreateUserName = CurrentUser.DisplayName;
                    p.UpdateBy = CurrentUser.UserId;
                    p.UpdateUserName = CurrentUser.DisplayName;
                });

                var rsp2 = InventoryApiClient.GetInstance().AddAdjustment(rsp1.ResponseResult, LoginCoreQuery);
                if (rsp2.Success)
                {
                    return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, Message = rsp2.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = false, Message = rsp1.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}