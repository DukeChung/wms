using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class InventoryController : BaseController
    {
        public ActionResult InitInventory()
        {
            return View();
        }

        public ActionResult ImportInitInventoryData()
        {
            var file = Request.Files["file"];
            if (file != null)
            {
                List<InitInventoryFromChannelDto> excelList = new List<InitInventoryFromChannelDto>();
                try
                {
                    IWorkbook wk = new HSSFWorkbook(file.InputStream);
                    ISheet sheet = wk.GetSheetAt(0);                     // 获取导入模板的第一个sheet数据 
                    var rowOne = sheet.GetRow(0);

                    for (int i = 1; i <= sheet.LastRowNum; i++)         //从第一行开始，第0行为title行
                    {
                        IRow row = sheet.GetRow(i);  //读取当前行数据
                        if (row != null)
                        {
                            // WarehouseId  SkuOtherID	Channel Qty
                            if (row.GetCell(0) != null && row.GetCell(1) != null && row.GetCell(3) != null)
                            {
                                excelList.Add(new InitInventoryFromChannelDto()
                                {
                                    WarehouseId = row.GetCell(0).ToString(),
                                    SkuOtherID = row.GetCell(1).ToString(),
                                    Channel = row.GetCell(2) == null ? string.Empty : row.GetCell(2).ToString(),
                                    Qty = int.Parse(row.GetCell(3).ToString())
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
                
                // TODO: 此处可能要分批执行
                InitInventoryFromChannelRequest request = new InitInventoryFromChannelRequest() {
                    WarehouseSysId = CurrentUser.WarehouseSysId,
                    InitList = excelList
                };

                var response = InventoryApiClient.GetInstance().GetInitChannelInventoryData(request,this.LoginCoreQuery);
                if (response.Success && response.ResponseResult != null)
                {
                    var thirdPartyStockTransferList = response.ResponseResult;

                    foreach (var thirdPartyStockTransfer in thirdPartyStockTransferList)
                    {
                        thirdPartyStockTransfer.CurrentDisplayName = CurrentUser.DisplayName;
                        thirdPartyStockTransfer.CurrentUserId = CurrentUser.UserId;
                        thirdPartyStockTransfer.WarehouseSysId = CurrentUser.WarehouseSysId;
                        var thirdPartyStockTransferResponse = InventoryApiClient.GetInstance().InsertStockTransfer(thirdPartyStockTransfer, this.LoginCoreQuery);
                    }
                }
                else
                {
                    return Json(new { Success = false, Message = response.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}