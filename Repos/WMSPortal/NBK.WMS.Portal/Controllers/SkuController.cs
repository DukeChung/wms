using NBK.AuthServiceUtil;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class SkuController : BaseController
    {
        [PermissionAuthorize("BaseData_Sku")]
        public ActionResult SkuList()
        {
            ViewBag.PrintName = PublicConst.PrintSettingUPC;
            InitializeViewBag();
            return View();
        }

        public ActionResult GetSkuList(SkuQuery skuQuery)
        {
            var rsp = BaseDataApiClient.GetInstance().GetSkuList(LoginCoreQuery, skuQuery);
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

        [PermissionAuthorize("BaseData_Sku_Create")]
        public ActionResult AddSku()
        {
            InitializeViewBag(true);
            return View();
        }

        public ActionResult SaveAddSku(SkuDto skuDto)
        {
            skuDto.CreateBy = CurrentUser.UserId;
            skuDto.UpdateBy = CurrentUser.UserId;
            var rsp = BaseDataApiClient.GetInstance().AddSku(LoginCoreQuery, skuDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [PermissionAuthorize("BaseData_Sku_Update")]
        public ActionResult EditSku(string sysId)
        {
            InitializeViewBag(true);
            var rsp = BaseDataApiClient.GetInstance().GetSkuById(LoginCoreQuery, sysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return View(rsp.ResponseResult);
            }
            return View();
        }

        public ActionResult SaveEditSku(SkuDto skuDto)
        {
            skuDto.UpdateBy = CurrentUser.UserId;
            var rsp = BaseDataApiClient.GetInstance().EditSku(LoginCoreQuery, skuDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [PermissionAuthorize("BaseData_Sku_Delete")]
        public ActionResult DeleteSku(string sysIds)
        {
            var rsp = BaseDataApiClient.GetInstance().DeleteSku(LoginCoreQuery, sysIds.ToGuidList());
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        private void InitializeViewBag()
        {
            List<SelectListItem> isActiveList = new List<SelectListItem>();
            isActiveList.Add(new SelectListItem { Text = "请选择" });
            isActiveList.Add(new SelectListItem { Text = "是", Value = "true" });
            isActiveList.Add(new SelectListItem { Text = "否", Value = "false" });
            ViewBag.IsActiveList = isActiveList;
        }

        private void InitializeViewBag(bool hasOptionLabel)
        {
            List<SelectListItem> lotTemplateList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                lotTemplateList.Add(new SelectListItem());
            }
            var rsp1 = BaseDataApiClient.GetInstance().SelectItemLotTemplate();
            if (rsp1.Success && rsp1.ResponseResult != null)
            {
                lotTemplateList.AddRange(rsp1.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.LotTemplateList = lotTemplateList;

            List<SelectListItem> shelfLifeTypeList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                shelfLifeTypeList.Add(new SelectListItem());
            }
            var rsp2 = BaseDataApiClient.GetInstance().SelectItemSysCode("ShelfLifeType");
            if (rsp2.Success && rsp2.ResponseResult != null)
            {
                shelfLifeTypeList.AddRange(rsp2.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.ShelfLifeTypeList = shelfLifeTypeList;

            List<SelectListItem> packList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                packList.Add(new SelectListItem());
            }
            var rsp3 = BaseDataApiClient.GetInstance().SelectItemPack(null);
            if (rsp3.Success && rsp3.ResponseResult != null)
            {
                packList.AddRange(rsp3.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.PackList = packList;

            List<SelectListItem> skuClass1List = new List<SelectListItem>();
            var rsp4 = InventoryApiClient.GetInstance().SelectItemSkuClass(LoginCoreQuery, null);
            if (rsp4.Success && rsp4.ResponseResult != null)
            {
                skuClass1List.AddRange(rsp4.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.SkuClass1List = skuClass1List;

            List<SelectListItem> locationList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                locationList.Add(new SelectListItem());
            }
            var rsp5 = BaseDataApiClient.GetInstance().SelectLocation(LoginCoreQuery, CurrentUser.WarehouseSysId);
            if (rsp5.Success && rsp5.ResponseResult != null)
            {
                locationList.AddRange(rsp5.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.LocationList = locationList;
        }
    }
}