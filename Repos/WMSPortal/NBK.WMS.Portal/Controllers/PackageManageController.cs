using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class PackageManageController : BaseController
    {
        public PackageManageController()
        {

        }

        public ActionResult UOMMaintain()
        {
            return View();
        }

        public ActionResult GetUOMList(UOMQuery request)
        {
            var rsp = BaseDataApiClient.GetInstance().GetUOMList(request);
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

        public ActionResult GetAllUOM()
        {
            UOMQuery request = new UOMQuery() {
                iDisplayStart = 0,
                iDisplayLength = 99999
            };
            var rsp = BaseDataApiClient.GetInstance().GetUOMList(request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    success = true,
                    uomList = rsp.ResponseResult.TableResuls.aaData
                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult UOMEdit(Guid sysid)
        {
            ViewBag.UOMSysId = sysid;

            List<SelectListItem> uomList = new List<SelectListItem>();

            var rsp = BaseDataApiClient.GetInstance().SelectItemSysCode("UOM");
            if (rsp.Success && rsp.ResponseResult != null)
            {
                uomList.AddRange(rsp.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.UomList = uomList;

            return View();
        }

        public ActionResult GetUOMBySysId(Guid sysId)
        {
            var rsp = BaseDataApiClient.GetInstance().GetUOMBySysId(sysId);

            if (rsp.Success)
            {
                return Json(new { success = true, UOM = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateUOM(UOMDto uom)
        {
            var rsp = BaseDataApiClient.GetInstance().UpdateUOM(uom);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UOMAdd()
        {
            List<SelectListItem> uomList = new List<SelectListItem>();

            var rsp = BaseDataApiClient.GetInstance().SelectItemSysCode("UOM");
            if (rsp.Success && rsp.ResponseResult != null)
            {
                uomList.AddRange(rsp.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.UomList = uomList;

            return View();
        }

        public ActionResult AddUOM(UOMDto uom)
        {
            var rsp = BaseDataApiClient.GetInstance().AddUOM(uom);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteUOM(string sysIdList)
        {
            var rsp = BaseDataApiClient.GetInstance().DeleteUOM(sysIdList);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #region pack management

        public ActionResult PackMaintain()
        {
            return View();
        }

        public ActionResult GetPackList(PackQuery request)
        {
            var rsp = BaseDataApiClient.GetInstance().GetPackList(request);
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

        public ActionResult PackAdd()
        {
            return View(new PackDto());
        }

        public ActionResult PackEdit(Guid sysid)
        {
            ViewBag.PackSysId = sysid;

            return View();
        }

        public ActionResult GetPackBySysId(Guid sysId)
        {
            var rsp = BaseDataApiClient.GetInstance().GetPackBySysId(sysId);

            if (rsp.Success)
            {
                return Json(new { success = true, Pack = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdatePack(PackDto packDto)
        {
            var rsp = BaseDataApiClient.GetInstance().UpdatePack(packDto);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddPack(PackDto packDto)
        {
            var rsp = BaseDataApiClient.GetInstance().AddPack(packDto);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DeletePack(string sysIdList)
        {
            var rsp = BaseDataApiClient.GetInstance().DeletePack(sysIdList);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}