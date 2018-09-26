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
    public class SkuBorrowController : BaseController
    {
        // GET: SkuBorrow
        public ActionResult SkuBorrowMaintain()
        {
            ViewBag.PrintName = PublicConst.PrintSettingA4; 
            ViewBag.UserName = CurrentUser.DisplayName;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            return View();
        } 

        public ActionResult GetSkuBorrowListByPage(SkuBorrowQuery request)
        { 
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = VASApiClient.GetInstance().GetSkuBorrowListByPage(request, this.LoginCoreQuery);
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

        public ActionResult AddSkuBorrow()
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

        public ActionResult AddSkuBorrowService(SkuBorrowDto dto)
        { 
            dto.CreateBy = CurrentUser.UserId;
            dto.CreateUserName = CurrentUser.DisplayName;
            dto.UpdateBy = CurrentUser.UserId;
            dto.UpdateUserName = CurrentUser.DisplayName;
            dto.WareHouseSysId = CurrentUser.WarehouseSysId;
            dto.SkuBorrowDetailList.ForEach(p =>
            {
                p.CreateBy = CurrentUser.UserId;
                p.CreateUserName = CurrentUser.DisplayName;
                p.UpdateBy = CurrentUser.UserId;
                p.UpdateUserName = CurrentUser.DisplayName;
            });
            var rsp = VASApiClient.GetInstance().AddSkuBorrow(dto, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            } 
        }


        public ActionResult DeleteSkuBorrowSkus(string sysIdList)
        {
            var rsp = VASApiClient.GetInstance().DeleteSkuBorrowSkus(sysIdList.ToGuidList(), this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult UpdateSkuBorrow(Guid sysId)
        {
            var response = VASApiClient.GetInstance().GetSkuBorrowBySysId(sysId, CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new SkuBorrowViewDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }
            ViewData["IsSaveDisplay"] = (model.Status == (int)SkuBorrowStatus.New);
            return View(model);
        }


        public ActionResult GetSkuList(SkuInvLotLocLpnQuery skuQuery)
        {
            var rsp = VASApiClient.GetInstance().GetSkuListByPage(skuQuery, LoginCoreQuery);
            
            if (rsp.Success && rsp.ResponseResult.TableResuls != null)
            {
                rsp.ResponseResult.TableResuls.aaData = rsp.ResponseResult.TableResuls.aaData.Where(o => o.DisplayQty > 0).ToList();
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

        public ActionResult SkuBorrowView(Guid sysId)
        {
            var response = VASApiClient.GetInstance().GetSkuBorrowBySysId(sysId,CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new SkuBorrowViewDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }
            ViewBag.PrintName = PublicConst.PrintSettingA4;
            ViewBag.BorrowOrder = model.SkuBorrowOrder;
            ViewBag.UserName = CurrentUser.DisplayName;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId; 
            ViewData["Status"] = model.Status;
            return View(model);
        }


        public ActionResult UpdateSkuBorrowService(SkuBorrowDto dto)
        { 
            dto.UpdateBy = CurrentUser.UserId;
            dto.UpdateUserName = CurrentUser.DisplayName;
            if (dto.SkuBorrowDetailList != null && dto.SkuBorrowDetailList.Count > 0)
            {
                dto.SkuBorrowDetailList.ForEach(p =>
                {
                    p.UpdateBy = CurrentUser.UserId;
                    p.UpdateUserName = CurrentUser.DisplayName;
                });
            }

            var rsp = VASApiClient.GetInstance().UpdateSkuBorrow(dto, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Audit(SkuBorrowDto dto)
        {
            dto.AuditingBy = CurrentUser.UserId; 
            dto.AuditingName = CurrentUser.DisplayName;
            dto.UpdateBy = CurrentUser.UserId;
            dto.UpdateDate = DateTime.Now;
            dto.UpdateUserName = CurrentUser.DisplayName;

            var rsp = VASApiClient.GetInstance().Audit(dto, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Void(SkuBorrowAuditDto dto)
        {
            dto.AuditingBy = CurrentUser.UserId;
            dto.AuditingName = CurrentUser.DisplayName;

            var rsp = VASApiClient.GetInstance().Void(dto, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
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
                FileUploader.UploadFile(FileUploader.SkuBorrow, file.InputStream, fileName, FtpFileType.Image);
                return Json(new { Success = true, Name = file.FileName, Url = fileName, Size = size, Suffix = Path.GetExtension(file.FileName).ToLower(), ShowUrl = FileUploader.httpAddress + "/" + FileUploader.SkuBorrow + "/" + fileName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = "上传文件不能为空" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}