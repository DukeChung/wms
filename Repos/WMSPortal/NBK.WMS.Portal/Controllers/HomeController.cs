using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NBK.AuthServiceUtil;
using NBK.WMS.Portal.Models;
using NBK.WMS.Portal.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index(string id)
        {
            if (CurrentUser != null && CurrentUser.WareHouseList != null && CurrentUser.WareHouseList.Count > 0 && id != null)
            {
                var wareHouseSysId = Guid.Parse(id);
                var warehouseDto = CurrentUser.WareHouseList.Find(x => x.SysId == wareHouseSysId);
                if (warehouseDto != null)
                {
                    ApplicationUser loginUser = CurrentUser;
                    loginUser.WarehouseSysId = warehouseDto.SysId;
                    loginUser.WarehouseName = warehouseDto.Name;

                    var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
                    if (claimsIdentity != null)
                    {
                        claimsIdentity.RemoveClaim(claimsIdentity.Claims.FirstOrDefault(x => x.Type == SystemDataConst.LoginUser));
                        claimsIdentity.AddClaim(new Claim(SystemDataConst.LoginUser, JsonConvert.SerializeObject(loginUser)));
                        claimsIdentity.RemoveClaim(claimsIdentity.Claims.FirstOrDefault(x => x.Type == SystemDataConst.SessionLoginWarehouseSysId));
                        claimsIdentity.AddClaim(new Claim(SystemDataConst.SessionLoginWarehouseSysId, loginUser.WarehouseSysId.ToString()));
                        HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = true }, claimsIdentity);
                    }
                }
                else
                {
                    Response.Redirect("/Account/Login");
                }
            }

            return View();
        }

        public ActionResult QuickSearch(string searchNumber)
        {
            if (string.IsNullOrEmpty(searchNumber))
            {
                return null;
            }

            //收货单
            if (searchNumber.StartsWith("GR", StringComparison.OrdinalIgnoreCase)
                || (searchNumber.StartsWith("PO", StringComparison.OrdinalIgnoreCase) && searchNumber.Contains("-")))
            {
                var rsp = InboundApiClient.GetInstance().GetReceiptList(LoginCoreQuery, new ECService.WMS.DTO.ReceiptQuery()
                {
                    ReceiptOrderSearch = searchNumber,
                    WarehouseSysId = CurrentUser.WarehouseSysId,
                    iDisplayStart = 0,
                    iDisplayLength = 5
                });
                if (rsp.ResponseResult.TableResuls != null && rsp.ResponseResult.TableResuls.aaData.Count > 0)
                {
                    return RedirectToRoute(new { controller = "Receipt", action = "ReceiptView", sysId = rsp.ResponseResult.TableResuls.aaData[0].SysId });
                }
            }

            //采购单
            if (searchNumber.StartsWith("PO", StringComparison.OrdinalIgnoreCase))
            {
                var rsp = InboundApiClient.GetInstance().GetPurchaseDtoList(new ECService.WMS.DTO.PurchaseQuery()
                {
                    PurchaseOrderSearch = searchNumber,
                    WarehouseSysId = CurrentUser.WarehouseSysId,
                    iDisplayStart = 0,
                    iDisplayLength = 5
                });
                if (rsp.ResponseResult.TableResuls != null && rsp.ResponseResult.TableResuls.aaData.Count > 0)
                {
                    return RedirectToRoute(new { controller = "Purchase", action = "PurchaseView", sysId = rsp.ResponseResult.TableResuls.aaData[0].SysId });
                }
            }

            //出库单
            if (searchNumber.StartsWith("OB", StringComparison.OrdinalIgnoreCase))
            {
                var rsp = OutboundApiClient.GetInstance().GetOutboundByPage(new ECService.WMS.DTO.OutboundQuery()
                {
                    OutboundOrder = searchNumber,
                    WarehouseSysId = CurrentUser.WarehouseSysId,
                    iDisplayStart = 0,
                    iDisplayLength = 5
                });

                if (rsp.ResponseResult.TableResuls != null && rsp.ResponseResult.TableResuls.aaData.Count > 0)
                {
                    return RedirectToRoute(new { controller = "Outbound", action = "OutboundView", sysId = rsp.ResponseResult.TableResuls.aaData[0].SysId });
                }
            }

            //装箱单
            if (searchNumber.StartsWith("PN", StringComparison.OrdinalIgnoreCase))
            {
                var rsp = OutboundApiClient.GetInstance().GetVanningList(new ECService.WMS.DTO.VanningQueryDto()
                {
                    VanningOrderSearch = searchNumber,
                    WarehouseSysId = CurrentUser.WarehouseSysId,
                    iDisplayStart = 0,
                    iDisplayLength = 5
                });

                if (rsp.ResponseResult.TableResuls != null && rsp.ResponseResult.TableResuls.aaData.Count > 0)
                {
                    return RedirectToRoute(new { controller = "Vanning", action = "VanningView", sysId = rsp.ResponseResult.TableResuls.aaData[0].SysId });
                }
            }

            //交接单
            if (searchNumber.StartsWith("PS", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToRoute(new { controller = "Vanning", action = "HandoverGroupDetail", handoverGroupOrder = searchNumber });
            }

            //损益单
            if (searchNumber.StartsWith("PL", StringComparison.OrdinalIgnoreCase))
            {
                var rsp = InventoryApiClient.GetInstance().GetAdjustmentListByPage(new ECService.WMS.DTO.AdjustmentQuery() {
                    AdjustmentOrder = searchNumber,
                    WarehouseSysId = CurrentUser.WarehouseSysId,
                    iDisplayStart = 0,
                    iDisplayLength = 5
                }, this.LoginCoreQuery);

                if (rsp.ResponseResult.TableResuls != null && rsp.ResponseResult.TableResuls.aaData.Count > 0)
                {
                    return RedirectToRoute(new { controller = "Ajustment", action = "AjustmentView", sysId = rsp.ResponseResult.TableResuls.aaData[0].SysId });
                }
            }

            //生产加工单
            if (searchNumber.StartsWith("BZ", StringComparison.OrdinalIgnoreCase))
            {
                var rsp = VASApiClient.GetInstance().GetAssemblyList(LoginCoreQuery, new ECService.WMS.DTO.AssemblyQuery() {
                    AssemblyOrderSearch = searchNumber,
                    WarehouseSysId = CurrentUser.WarehouseSysId,
                    iDisplayStart = 0,
                    iDisplayLength = 5
                });

                if (rsp.ResponseResult.TableResuls != null && rsp.ResponseResult.TableResuls.aaData.Count > 0)
                {
                    return RedirectToRoute(new { controller = "Assembly", action = "AssemblyView", sysId = rsp.ResponseResult.TableResuls.aaData[0].SysId });
                }
            }

            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }

        public ActionResult GetSystemMessageByUser(int userID)
        {


            return Json(new
            {
               
            }, JsonRequestBehavior.AllowGet);
        }
    }
}