using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    public class PrebulkPackController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.PrintSettingCase = PublicConst.PrintSettingCase;
            ViewBag.PrintName = PublicConst.PrintSettingA4;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            return View();
        }

        public ActionResult GetPreBulkPackByPage(PreBulkPackQuery request)
        {

            var rsp = OrderManegerApiClient.GetInstance().GetPreBulkPackByPage(request, this.LoginCoreQuery);
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

        /// <summary>
        /// 生成散货预包装
        /// </summary>
        /// <param name="batchPreBulkPackDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult AddPreBulkPack(BatchPreBulkPackDto batchPreBulkPackDto)
        {
            var rsp = OrderManegerApiClient.GetInstance().AddPreBulkPack(batchPreBulkPackDto);
            if (rsp.Success && rsp.ResponseResult)
            {
                return Json(new { Success = true, Message = "生成成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrebulkPackView(Guid sysId)
        {
            ViewBag.PrintName = PublicConst.PrintSettingA4;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var response = OrderManegerApiClient.GetInstance().GetPreBulkPackBySysId(sysId,CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new PreBulkPackDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult UpdatePrebulkPack(Guid sysId)
        {
            var response = OrderManegerApiClient.GetInstance().GetPreBulkPackBySysId(sysId, CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new PreBulkPackDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdatePreBulkPack(PreBulkPackDto request)
        {
            var rsp = OrderManegerApiClient.GetInstance().UpdatePreBulkPack(request, this.LoginCoreQuery);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "更新成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeletePrebulkPackSkus(string sysIdList)
        {
            var rsp = OrderManegerApiClient.GetInstance().DeletePrebulkPackSkus(sysIdList.ToGuidList(), this.LoginCoreQuery, CurrentUser.WarehouseSysId);

            if (rsp.Success)
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeletePrebulkPack(string sysIdList)
        {
            var rsp = OrderManegerApiClient.GetInstance().DeletePrebulkPack(sysIdList.ToGuidList(), this.LoginCoreQuery, CurrentUser.WarehouseSysId);

            if (rsp.Success)
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetPrebulkPackStorageCase(Guid outboundSysId)
        {
            var rsp = OrderManegerApiClient.GetInstance().GetPrebulkPackStorageCase(outboundSysId, this.LoginCoreQuery,CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [SetUserInfo]
        public JsonResult ImportData(PreBulkPackDto dto)
        {
            var file = Request.Files["file"];
            var sysIds = Request.Form["sysIds"];
            if (file != null && !string.IsNullOrEmpty(sysIds))
            {
                try
                {
                    dto.ImportSysIds = sysIds;
                    IWorkbook wk = new HSSFWorkbook(file.InputStream);
                    ISheet sheet = wk.GetSheetAt(0);                     // 获取导入模板的第一个sheet数据 
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

                            var info = new PreBulkPackDetailDto()
                            {

                                OtherId = row.GetCell(0).ToString(),
                                UPC = row.GetCell(1).ToString(),
                                PreQty = Convert.ToInt32(row.GetCell(3).ToString())
                            };
                            dto.PreBulkPackDetailList.Add(info);
                        }
                    }
                    if (dto.PreBulkPackDetailList != null && dto.PreBulkPackDetailList.Count > 0)
                    {
                        var rsp = OrderManegerApiClient.GetInstance().ImportPreBulkPack(dto);
                        if (rsp.Success)
                        {
                            return Json(new { Success = true, Message = "数据导入成功。" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
                        }
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