using NBK.AuthServiceUtil;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility.Enum;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.Utility;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class StockMovementController : BaseController
    {
        [PermissionAuthorize("Inventory_StockMovement")]
        public ActionResult StockMovement()
        {
            ///InitializeViewBag();
            return View();
        }

        public ActionResult GetStockMovementSkuList(StockMovementSkuQuery stockMovementSkuQuery)
        {
            stockMovementSkuQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().GetStockMovementSkuList(LoginCoreQuery, stockMovementSkuQuery);
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

        public ActionResult AddMovement(string skuSysId, string loc, string lot)
        {
            ViewBag.SkuSysId = skuSysId;
            ViewBag.Loc = loc;
            ViewBag.Lot = lot;
            InitializeViewBag();
            return View();
        }

        public ActionResult GetStockMovement(string skuSysId, string loc, string lot)
        {
            var rsp = InventoryApiClient.GetInstance().GetStockMovement(LoginCoreQuery, skuSysId, loc, lot, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new { Success = true, ViewModel = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveStockMovement(StockMovementDto stockMovementDto)
        {
            stockMovementDto.ToQty = stockMovementDto.FromQty;
            stockMovementDto.CreateBy = CurrentUser.UserId;
            stockMovementDto.CreateUserName = CurrentUser.DisplayName;
            stockMovementDto.UpdateBy = CurrentUser.UserId;
            stockMovementDto.UpdateUserName = CurrentUser.DisplayName;
            stockMovementDto.WareHouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().SaveStockMovement(LoginCoreQuery, stockMovementDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StockMovementList()
        {
            List<SelectListItem> statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem { Text = StockMovementStatus.New.ToDescription(), Value = ((int)StockMovementStatus.New).ToString() });
            statusList.Add(new SelectListItem { Text = StockMovementStatus.Confirm.ToDescription(), Value = ((int)StockMovementStatus.Confirm).ToString() });
            statusList.Add(new SelectListItem { Text = StockMovementStatus.Cancel.ToDescription(), Value = ((int)StockMovementStatus.Cancel).ToString() });
            ViewBag.StatusList = statusList;
            InitializeViewBag();
            return View();
        }

        public ActionResult GetStockMovementList(StockMovementQuery stockMovementQuery)
        {
            stockMovementQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().GetStockMovementList(LoginCoreQuery, stockMovementQuery);
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

        public ActionResult ConfirmStockMovement(string sysIds)
        {
            StockMovementOperationDto stockMovementOperationDto = new StockMovementOperationDto
            {
                SysIds = sysIds.ToGuidList(),
                CurrentUserId = CurrentUser.UserId,
                CurrentDisplayName = CurrentUser.DisplayName,
                WarehouseSysId = CurrentUser.WarehouseSysId
            };
            var rsp = InventoryApiClient.GetInstance().ConfirmStockMovement(LoginCoreQuery, stockMovementOperationDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelStockMovement(string sysIds)
        {
            StockMovementOperationDto stockMovementOperationDto = new StockMovementOperationDto
            {
                SysIds = sysIds.ToGuidList(),
                CurrentUserId = CurrentUser.UserId,
                CurrentDisplayName = CurrentUser.DisplayName,
                WarehouseSysId = CurrentUser.WarehouseSysId
            };
            var rsp = InventoryApiClient.GetInstance().CancelStockMovement(LoginCoreQuery, stockMovementOperationDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public void InitializeViewBag()
        {
            List<SelectListItem> locationList = new List<SelectListItem>();
            locationList.Add(new SelectListItem());
            var rsp = BaseDataApiClient.GetInstance().SelectLocation(LoginCoreQuery, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                locationList.AddRange(rsp.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.LocationList = locationList;
        }


        /// <summary>
        /// 库位变更导入数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public JsonResult ImportData()
        {
            var file = Request.Files["file"];
            if (file != null)
            {
                try
                {
                    var list = new ImportStockMovement()
                    {
                        StockMovementImportDto = new List<StockMovementImportDto>()
                    };
                    IWorkbook wk = new HSSFWorkbook(file.InputStream);
                    ISheet sheet = wk.GetSheetAt(0);
                    for (int i = 1; i <= sheet.LastRowNum; i++)         //从第一行开始，第0行为title行
                    {
                        IRow row = sheet.GetRow(i);  //读取当前行数据
                        if (row != null)
                        {
                            if (row.GetCell(0) == null || row.GetCell(0).ToString() == "")
                            {
                                return Json(new { Success = false, Message = "第 " + (i + 1) + " 行第 1 列不能为空" }, JsonRequestBehavior.AllowGet);
                            }
                            if (row.GetCell(1) == null || row.GetCell(1).ToString() == "")
                            {
                                return Json(new { Success = false, Message = "第 " + (i + 1) + " 行第 2 列不能为空" }, JsonRequestBehavior.AllowGet);
                            }
                            if (row.GetCell(3) == null || row.GetCell(3).ToString() == "")
                            {
                                return Json(new { Success = false, Message = "第 " + (i + 1) + " 行第 4 列不能为空" }, JsonRequestBehavior.AllowGet);
                            }
                            if (row.GetCell(3).ToString().Length > 10)
                            {
                                return Json(new { Success = false, Message = "第 " + (i + 1) + " 行第 4 列数据超出长度" }, JsonRequestBehavior.AllowGet);
                            }

                            if (row.GetCell(4) == null || row.GetCell(4).ToString() == "")
                            {
                                return Json(new { Success = false, Message = "第 " + (i + 1) + " 行第 5 列不能为空" }, JsonRequestBehavior.AllowGet);
                            }
                            if (row.GetCell(5) == null || row.GetCell(5).ToString() == "")
                            {
                                return Json(new { Success = false, Message = "第 " + (i + 1) + " 行第 6列不能为空" }, JsonRequestBehavior.AllowGet);
                            }

                            if (row.GetCell(5).ToString().Trim() == row.GetCell(4).ToString().Trim())
                            {
                                return Json(new { Success = false, Message = "第 " + (i + 1) + " 行 第5列和第6列不能相同" }, JsonRequestBehavior.AllowGet);
                            }

                            var qty = Convert.ToDecimal(row.GetCell(3).ToString());
                            if (qty <= 0)
                            {
                                return Json(new { Success = false, Message = "第 " + (i + 1) + " 行第 4 列不能小于等于0" }, JsonRequestBehavior.AllowGet);
                            }
                            var info = new StockMovementImportDto()
                            {

                                OtherId = row.GetCell(0).ToString(),
                                UPC = row.GetCell(1).ToString(),
                                SkuName = row.GetCell(2) != null ? row.GetCell(2).ToString() : "",
                                ChangerQty = qty,
                                FromLoc = row.GetCell(4).ToString(),
                                ToLoc = row.GetCell(5).ToString(),
                            };
                            list.StockMovementImportDto.Add(info);
                        }
                    }
                    if (list.StockMovementImportDto.Count > 50)
                    {
                        return Json(new { Success = false, Message = "一次导入不能超过50条记录" }, JsonRequestBehavior.AllowGet);
                    }
                    list.CurrentUserId = CurrentUser.UserId;
                    list.CurrentDisplayName = CurrentUser.DisplayName;
                    list.WarehouseSysId = CurrentUser.WarehouseSysId;
                    var rsp = InventoryApiClient.GetInstance().ImportStockMovementList(LoginCoreQuery, list);
                    if (rsp.Success)
                    {
                        return Json(new { Success = true, Message = "数据导入成功。" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return Json(new { Success = false, Message = "导入数据或选择单号不能为空" }, JsonRequestBehavior.AllowGet);
        }

    }
}