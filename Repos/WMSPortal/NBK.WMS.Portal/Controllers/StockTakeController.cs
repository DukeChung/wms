using NBK.AuthServiceUtil;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.WMS.Portal.Models;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class StockTakeController : BaseController
    {
        [PermissionAuthorize("Inventory_StockTake")]
        public ActionResult StockTakeList()
        {
            InitializeViewBag(false);
            return View();
        }

        public ActionResult GetStockTakeList(StockTakeQuery stockTakeQuery)
        {
            ViewBag.CurrentUserName = CurrentUser.DisplayName;
            ViewBag.PrintName = PublicConst.PrintSettingZS;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            //stockTakeQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().GetStockTakeList(LoginCoreQuery, stockTakeQuery);
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

        public ActionResult AddStockTake()
        {
            InitializeViewBag();
            return View();
        }

        public ActionResult SaveAddStockTake(StockTakeDto stockTakeDto)
        {
            stockTakeDto.CreateBy = CurrentUser.UserId;
            stockTakeDto.CreateUserName = CurrentUser.DisplayName;
            stockTakeDto.UpdateBy = CurrentUser.UserId;
            stockTakeDto.UpdateUserName = CurrentUser.DisplayName;
            stockTakeDto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().AddStockTake(LoginCoreQuery, stockTakeDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StockTakeView(string sysId)
        {
            StockTakeViewDto model = null;
            var rsp = InventoryApiClient.GetInstance().GetStockTakeViewById(LoginCoreQuery, sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                model = rsp.ResponseResult;
            }
            List<ApplicationUser> userList = AuthorizeManager.GetAllSystemUser();
            List<SelectListItem> selectUserList = new List<SelectListItem>();
            userList.ForEach(p => {
                selectUserList.Add(new SelectListItem() { Text = p.DisplayName, Value = p.UserId.ToString() });
            });
            ViewBag.UserList = selectUserList;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            ViewBag.WarehouseName = CurrentUser.WarehouseName;
            return View(model);
        }

        public ActionResult GetStockTakeDiffList(StockTakeViewQuery stockTakeViewQuery)
        {
            var rsp = InventoryApiClient.GetInstance().GetStockTakeDiffList(LoginCoreQuery, stockTakeViewQuery);
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

        public ActionResult GetStockTakeDetailList(StockTakeViewQuery stockTakeViewQuery)
        {
            var rsp = InventoryApiClient.GetInstance().GetStockTakeDetailList(LoginCoreQuery, stockTakeViewQuery);
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

        public ActionResult ReplayStockTakeDetail(Guid stockTakeSysId, int replayBy, string replayUserName, Guid warehouseSysId)
        {
            ReplayStockTakeDto replayStockTakeDto = new ReplayStockTakeDto
            {
                StockTakeSysId = stockTakeSysId,
                ReplayBy = replayBy,
                ReplayUserName = replayUserName,
                UpdateBy = CurrentUser.UserId,
                UpdateUserName = CurrentUser.DisplayName,
                WarehouseSysId = warehouseSysId
            };
            var rsp = InventoryApiClient.GetInstance().ReplayStockTakeDetail(LoginCoreQuery, replayStockTakeDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 盘点完成
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ActionResult StockTakeComplete(string sysIds)
        {
            StockTakeCompleteDto stockTakeCompleteDto = new StockTakeCompleteDto
            {
                SysIds = sysIds.ToGuidList(),
                CurrentUserId = CurrentUser.UserId,
                CurrentDisplayName = CurrentUser.DisplayName,
                WarehouseSysId = CurrentUser.WarehouseSysId
            };
            var rsp = InventoryApiClient.GetInstance().StockTakeComplete(LoginCoreQuery, stockTakeCompleteDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteStockTake(string sysIds)
        {
            var rsp = InventoryApiClient.GetInstance().DeleteStockTake(LoginCoreQuery, sysIds.ToGuidList());
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StockTakeReport(string sysIds)
        {
            ViewBag.CurrentUserName = CurrentUser.DisplayName;
            ViewBag.PrintName = PublicConst.PrintSettingZS;
            ViewBag.SysIdStr = sysIds;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            return View();
        }

        public ActionResult GetStockTakeReport(StockTakeReportQuery stockTakeReportQuery)
        {
            stockTakeReportQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().GetStockTakeReport(LoginCoreQuery, stockTakeReportQuery);
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

        public ActionResult CreateAdjustment(Guid warehouseSysId, string detailSysIds)
        {
            var rsp1 = InventoryApiClient.GetInstance().GetAdjustmentDto(LoginCoreQuery, new CreateAdjustmentDto { WarehouseSysId = warehouseSysId, DetailSysIds = detailSysIds.ToGuidList() });
            if (rsp1.Success && rsp1.ResponseResult != null)
            {
                rsp1.ResponseResult.CreateBy = CurrentUser.UserId;
                rsp1.ResponseResult.CreateUserName = CurrentUser.DisplayName;
                rsp1.ResponseResult.UpdateBy = CurrentUser.UserId;
                rsp1.ResponseResult.UpdateUserName = CurrentUser.DisplayName;
                rsp1.ResponseResult.WarehouseSysId = CurrentUser.WarehouseSysId;
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

        public ActionResult GetSelectSkuClass(Guid? parentSysId)
        {
            var rsp = InventoryApiClient.GetInstance().SelectItemSkuClass(LoginCoreQuery, parentSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult NewStockTake()
        {
            InitializeViewBag();
            return View();
        }

        public ActionResult GetWaitingStockTakeSkuList(StockTakeSkuQuery stockTakeSkuQuery)
        {
            if (stockTakeSkuQuery.StockTakeType == (int)StockTakeType.Location && !stockTakeSkuQuery.ZoneSysId.HasValue && !stockTakeSkuQuery.LocSysId.HasValue)
            {
                return Json(new
                {
                    aaData = new List<StockTakeSkuListDto>(),
                    iTotalDisplayRecords = 0,
                    iTotalRecords = 0,
                    sEcho = 0
                }, JsonRequestBehavior.AllowGet);
            }
            if (stockTakeSkuQuery.StockTakeType == (int)StockTakeType.Touch && stockTakeSkuQuery.StartDate.HasValue && stockTakeSkuQuery.EndDate.HasValue)
            {
                stockTakeSkuQuery.EndDate = stockTakeSkuQuery.EndDate.Value.AddDays(1).AddMilliseconds(-1);
            }
            var rsp = InventoryApiClient.GetInstance().GetWaitingStockTakeSkuList(LoginCoreQuery, stockTakeSkuQuery);
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

        public ActionResult SaveNewStockTake(NewStockTakeDto newStockTakeDto)
        {
            if (newStockTakeDto.StockTakeType == (int)StockTakeType.Touch && newStockTakeDto.StartDate.HasValue && newStockTakeDto.EndDate.HasValue)
            {
                newStockTakeDto.EndDate = newStockTakeDto.EndDate.Value.AddDays(1).AddMilliseconds(-1);
            }
            var rsp = InventoryApiClient.GetInstance().NewStockTake(LoginCoreQuery, newStockTakeDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StockTakeStart(StockTakeStartDto stockTakeStartDto)
        {
            var rsp = InventoryApiClient.GetInstance().StockTakeStart(LoginCoreQuery, stockTakeStartDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSelectLocationList(string zoneSysId)
        {
            var rsp = BaseDataApiClient.GetInstance().SelectLocation(LoginCoreQuery, CurrentUser.WarehouseSysId, zoneSysId);
            if (rsp.Success)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult SaveAddStockTake(StockTakeDto stockTakeDto)
        //{
        //    stockTakeDto.CreateBy = CurrentUser.UserId;
        //    stockTakeDto.CreateUserName = CurrentUser.DisplayName;
        //    stockTakeDto.UpdateBy = CurrentUser.UserId;
        //    stockTakeDto.UpdateUserName = CurrentUser.DisplayName;
        //    stockTakeDto.WarehouseSysId = CurrentUser.WarehouseSysId;
        //    var rsp = InventoryApiClient.GetInstance().AddStockTake(LoginCoreQuery, stockTakeDto);
        //    if (rsp.Success)
        //    {
        //        return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //}

        private void InitializeViewBag()
        {
            List<SelectListItem> stockTakeTypeList = new List<SelectListItem>();
            stockTakeTypeList.Add(new SelectListItem { Text = StockTakeType.Location.ToDescription(), Value = ((int)StockTakeType.Location).ToString() });
            stockTakeTypeList.Add(new SelectListItem { Text = StockTakeType.Sku.ToDescription(), Value = ((int)StockTakeType.Sku).ToString() });
            //stockTakeTypeList.Add(new SelectListItem { Text = StockTakeType.Abc.ToDescription(), Value = ((int)StockTakeType.Abc).ToString() });
            //stockTakeTypeList.Add(new SelectListItem { Text = StockTakeType.Random.ToDescription(), Value = ((int)StockTakeType.Random).ToString() });
            ViewBag.StockTakeTypeList = stockTakeTypeList;

            List<SelectListItem> zoneList = new List<SelectListItem>();
            ZoneQuery zoneQuery = new ZoneQuery { iDisplayStart = 0, iDisplayLength = 500, mDataProp_1 = "ZoneCode", IsActiveSearch = true, WarehouseSysId = CurrentUser.WarehouseSysId };
            var rsp1 = BaseDataApiClient.GetInstance().GetZoneList(LoginCoreQuery, zoneQuery);
            if (rsp1.Success && rsp1.ResponseResult.TableResuls != null)
            {
                zoneList.AddRange(rsp1.ResponseResult.TableResuls.aaData.Select(p => new SelectListItem
                {
                    Text = p.ZoneCode,
                    Value = p.SysId.ToString()
                }));
            }
            ViewBag.ZoneList = zoneList;

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

            List<SelectListItem> skuClass1List = new List<SelectListItem>();
            var rsp3 = InventoryApiClient.GetInstance().SelectItemSkuClass(LoginCoreQuery, null);
            if (rsp3.Success && rsp3.ResponseResult != null)
            {
                skuClass1List.AddRange(rsp3.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.SkuClass1List = skuClass1List;

            List<ApplicationUser> userList = AuthorizeManager.GetAllSystemUser();
            List<SelectListItem> selectUserList = new List<SelectListItem>();
            userList.ForEach(p => {
                selectUserList.Add(new SelectListItem() { Text = p.DisplayName, Value = p.UserId.ToString() });
            });
            ViewBag.UserList = selectUserList;
        }

        private void InitializeViewBag(bool hasOptionLabel)
        {
            List<SelectListItem> statusList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                statusList.Add(new SelectListItem());
            }
            statusList.Add(new SelectListItem { Text = StockTakeStatus.New.ToDescription(), Value = ((int)StockTakeStatus.New).ToString() });
            statusList.Add(new SelectListItem { Text = StockTakeStatus.Started.ToDescription(), Value = ((int)StockTakeStatus.Started).ToString() });
            statusList.Add(new SelectListItem { Text = StockTakeStatus.StockTake.ToDescription(), Value = ((int)StockTakeStatus.StockTake).ToString() });
            statusList.Add(new SelectListItem { Text = StockTakeStatus.StockTakeFinished.ToDescription(), Value = ((int)StockTakeStatus.StockTakeFinished).ToString() });
            statusList.Add(new SelectListItem { Text = StockTakeStatus.Replay.ToDescription(), Value = ((int)StockTakeStatus.Replay).ToString() });
            statusList.Add(new SelectListItem { Text = StockTakeStatus.ReplayFinished.ToDescription(), Value = ((int)StockTakeStatus.ReplayFinished).ToString() });
            statusList.Add(new SelectListItem { Text = StockTakeStatus.Finished.ToDescription(), Value = ((int)StockTakeStatus.Finished).ToString() });
            ViewBag.StatusList = statusList;

            List<ApplicationUser> userList = AuthorizeManager.GetAllSystemUser();
            List<SelectListItem> selectUserList = new List<SelectListItem>();
            userList.ForEach(p => {
                selectUserList.Add(new SelectListItem() { Text = p.DisplayName, Value = p.UserId.ToString() });
            });
            ViewBag.UserList = selectUserList;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            ViewBag.WarehouseName = CurrentUser.WarehouseName;
        }
    }
}