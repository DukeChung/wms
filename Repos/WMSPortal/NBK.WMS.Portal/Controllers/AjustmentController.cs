using NBK.AuthServiceUtil;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class AjustmentController : BaseController
    {
        public ActionResult AjustmentMaintain()
        {
            return View();
        }

        public ActionResult GetAdjustmentListByPage(AdjustmentQuery request)
        {
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().GetAdjustmentListByPage(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
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

        public ActionResult AjustmentView(Guid sysId)
        {
            var response = InventoryApiClient.GetInstance().GetAdjustmentBySysId(sysId, CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new AdjustmentViewDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }
            ViewData["IsAuditDisplay"] = this.CheckHasAuthFun(AuthKeyConst.Inventory_Ajustment_Auditing)
                && (model.Status == (int)AdjustmentStatus.New);
            return View(model);
        }

        public ActionResult AddAjustment()
        {
            List<SelectListItem> warehouseList = new List<SelectListItem>();
            var rsp2 = InventoryApiClient.GetInstance().SelectItemWarehouse(LoginCoreQuery);
            if (rsp2.Success && rsp2.ResponseResult != null)
            {
                warehouseList.AddRange(rsp2.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.WarehouseList = warehouseList;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;

            return View();
        }

        public ActionResult GetAdjustmentLevelSysCode()
        {
            List<SelectListItem> adjustmentLevelList = new List<SelectListItem>();

            var rsp = BaseDataApiClient.GetInstance().SelectItemSysCode("AdjustmentLevel");
            if (rsp.Success && rsp.ResponseResult != null)
            {
                adjustmentLevelList.AddRange(rsp.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }

            return Json(new { adjustmentLevelList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddAjustmentService(AdjustmentDto dto)
        {
            dto.Type = (int)AdjustmentType.ProfiAndLoss;
            dto.CreateBy = CurrentUser.UserId;
            dto.CreateUserName = CurrentUser.DisplayName;
            dto.UpdateBy = CurrentUser.UserId;
            dto.UpdateUserName = CurrentUser.DisplayName;
            dto.AdjustmentDetailList.ForEach(p =>
            {
                p.CreateBy = CurrentUser.UserId;
                p.CreateUserName = CurrentUser.DisplayName;
                p.UpdateBy = CurrentUser.UserId;
                p.UpdateUserName = CurrentUser.DisplayName;
            });
            var rsp = InventoryApiClient.GetInstance().AddAdjustment(dto, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSkuList(SkuInvLotLocLpnQuery skuQuery)
        {
            var rsp = InventoryApiClient.GetInstance().GetAdjustmentListByPage(skuQuery, LoginCoreQuery);
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

        public ActionResult UpdateAjustment(Guid sysId)
        {
            var response = InventoryApiClient.GetInstance().GetAdjustmentBySysId(sysId, CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new AdjustmentViewDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }
            ViewData["IsSaveDisplay"] = (model.Status == (int)AdjustmentStatus.New);
            return View(model);
        }

        public JsonResult UploadImage()
        {
            var file = Request.Files["file"];
            if (file != null)
            {
                var size = file.ContentLength;
                var type = Path.GetExtension(file.FileName);
                if (type != ".jpeg" && type != ".gif" && type != ".jpg" && type != ".png" && type != ".svg")
                {
                    return Json(new { Success = false, Message = "只能上传图片类型文件" }, JsonRequestBehavior.AllowGet);
                }
                var fileName = Guid.NewGuid() + type.ToLower();
                FileUploader.UploadFile(FileUploader.Adjustment, file.InputStream, fileName, FtpFileType.Image);
                return Json(new { Success = true, Name = file.FileName, Url = fileName, Size = size, Suffix = Path.GetExtension(file.FileName).ToLower(), ShowUrl = FileUploader.httpAddress + "/" + FileUploader.Adjustment + "/" + fileName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = "上传文件不能为空" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateAjustmentService(AdjustmentDto dto)
        {
            dto.Type = (int)AdjustmentType.ProfiAndLoss;
            dto.UpdateBy = CurrentUser.UserId;
            dto.UpdateUserName = CurrentUser.DisplayName;
            dto.AdjustmentDetailList.ForEach(p =>
            {
                p.UpdateBy = CurrentUser.UserId;
                p.UpdateUserName = CurrentUser.DisplayName;
            });

            var rsp = InventoryApiClient.GetInstance().UpdateAdjustment(dto, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteAjustmentSkus(string sysIdList)
        {
            var rsp = InventoryApiClient.GetInstance().DeleteAjustmentSkus(sysIdList.ToGuidList(),CurrentUser.WarehouseSysId, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Audit(AdjustmentAuditDto dto)
        {
            dto.AuditingBy = CurrentUser.UserId;
            dto.AuditingName = CurrentUser.DisplayName;
            dto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().Audit(dto, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Void(AdjustmentAuditDto dto)
        {
            dto.AuditingBy = CurrentUser.UserId;
            dto.AuditingName = CurrentUser.DisplayName;

            var rsp = InventoryApiClient.GetInstance().Void(dto, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet); 
            }
        }
    }
}