using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    public class PrePackController : BaseController
    {
        // GET: PrePack
        public ActionResult Index()
        {
            return View();
        }

        [SetUserInfo]
        public ActionResult GetPrePackByPage(PrePackQuery request)
        {
            if (!string.IsNullOrEmpty(request.PrePackOrder))
                request.PrePackOrder.Replace('，', ',');

            if (!string.IsNullOrEmpty(request.OutboundOrder))
                request.OutboundOrder.Replace('，', ',');

            if (request.CreateDateTo.HasValue)
                request.CreateDateTo = request.CreateDateTo.Value.AddDays(1);

            request.WarehouseSysId = CurrentUser.WarehouseSysId;

            var rsp = OrderManegerApiClient.GetInstance().GetPrePackByPage(request);
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

        public ActionResult AddPrePack()
        {
            return View();
        }

        public ActionResult GetPrePackSkuByPage(PrePackSkuQuery skuQuery)
        {
            var rsp = OrderManegerApiClient.GetInstance().GetPrePackSkuByPage(skuQuery);
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

        public ActionResult GetPrePackSku()
        {
            PrePackSkuDto prePackSkuDto = new PrePackSkuDto
            {
                StorageLoc = string.Empty,
                PrePackSkuListDto = new List<PrePackDetailDto>()
            };
            return Json(new { Success = true, ViewModel = prePackSkuDto }, JsonRequestBehavior.AllowGet);
        }

        [SetUserInfo]
        public ActionResult SavePrePackSku(PrePackSkuDto prePackSkuDto)
        {
            var rsp = OrderManegerApiClient.GetInstance().SavePrePackSku(prePackSkuDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdatePerPack(Guid sysId)
        {
            ViewBag.SysId = sysId;
            return View();
        }

        [SetUserInfo]
        public ActionResult UpdatePrePackSku(PrePackSkuDto prePackSkuDto)
        {
            var rsp = OrderManegerApiClient.GetInstance().UpdatePrePackSku(prePackSkuDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPrePackSkuBySysId(Guid sysId)
        {
            var rsp = OrderManegerApiClient.GetInstance().GetPrePackBySysId(sysId,CurrentUser.WarehouseSysId);
            PrePackSkuDto prePackSkuDto = new PrePackSkuDto();
            if (rsp.Success)
            {
                prePackSkuDto = rsp.ResponseResult;
            }
            else
            {
                prePackSkuDto.StorageLoc = string.Empty;
                prePackSkuDto.PrePackSkuListDto = new List<PrePackDetailDto>();
            }
            return Json(new { Success = true, ViewModel = prePackSkuDto }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletePerPack(string sysIdList)
        {
            var rsp = OrderManegerApiClient.GetInstance().DeletePerPack(sysIdList.ToGuidList(), CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new { success = true, message = "删除成功！" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "删除失败," + rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult PerPackView(Guid sysId)
        {
            ViewBag.PrintName = PublicConst.PrintSettingA4;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var response = OrderManegerApiClient.GetInstance().GetPrePackBySysId(sysId, CurrentUser.WarehouseSysId);
            var model = new PrePackSkuDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            };
            return View(model);
        }

        [SetUserInfo]

        public JsonResult ImportData(PrePackSkuDto prePackSkuDto)
        {
            var file = Request.Files["file"];
            if (file != null)
            {
                try
                {
                    prePackSkuDto.PrePackSkuListDto = new List<PrePackDetailDto>();
                    IWorkbook wk = new HSSFWorkbook(file.InputStream);
                    ISheet sheet = wk.GetSheetAt(0);                     // 获取导入模板的第一个sheet数据 
                    var rowOne = sheet.GetRow(0);
                    prePackSkuDto.ServiceStationName = rowOne.GetCell(1).ToString();
                    var rowTwo = sheet.GetRow(1);
                    prePackSkuDto.BatchNumber = rowTwo.GetCell(1).ToString();
                    for (int i = 3; i <= sheet.LastRowNum; i++)         //从第一行开始，第0行为title行
                    {
                        IRow row = sheet.GetRow(i);  //读取当前行数据
                        if (row != null)
                        {
                            if (row.GetCell(0) != null && row.GetCell(1) != null && row.GetCell(3) != null)
                            {
                                var prepackDetail = new PrePackDetailDto()
                                {
                                    OtherId = row.GetCell(0).ToString(),
                                    UPC = row.GetCell(1).ToString(),
                                    PreQty = Convert.ToInt32(row.GetCell(3).ToString())
                                };
                                prePackSkuDto.PrePackSkuListDto.Add(prepackDetail);
                            }
                        }
                    }
                    var rsp = OrderManegerApiClient.GetInstance().ImportPrePack(prePackSkuDto);
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
            return Json(new { Success = false, Message = "导入失败，导入数据不能为空" }, JsonRequestBehavior.AllowGet);
        }


        [SetUserInfo]
        public ActionResult IsStorageLoc(PrePackQuery query)
        {
            var rsp = OrderManegerApiClient.GetInstance().IsStorageLoc(query);
            if (rsp.Success)
            {
                return Json(new { success = rsp.ResponseResult, message = "该货位已经存在新建中" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
        }

        [SetUserInfo]
        public ActionResult CopyPrePack(PrePackCopy query)
        {
            var rsp = OrderManegerApiClient.GetInstance().CopyPrePack(query);
            if (rsp.Success)
            {
                return Json(new { success = rsp.ResponseResult, message = "复制成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}