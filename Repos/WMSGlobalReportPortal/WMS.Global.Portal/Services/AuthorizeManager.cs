using WMS.Global.Portal.AuthServiceReference;
using WMS.Global.Portal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;

namespace NBK.AuthServiceUtil
{
    /// <summary>
    /// 授权管理
    /// </summary>
    public static class AuthorizeManager
    {
        /// <summary>
        /// 用户登录验证
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="password">密码</param>
        /// <returns>AuthCenter用户信息</returns>
        public static SystemUser Login(string loginName, string password)
        {
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            SystemUser authSystemUser = authServiceSoapClient.LoginAuth(loginName, password);
            return authSystemUser;
        }

        /// <summary>
        /// 判断当前用户在当前应用程序中是否存在此权限Key
        /// </summary>
        /// <param name="authKey">权限Key</param>
        /// <returns></returns>
        public static bool HasFunction(AuthKey authKey, string loginUserName)
        {
            bool result = false;
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            result = authServiceSoapClient.CheckUserFunction(SystemDataConst.ApplicationID, authKey.Key, loginUserName);
            return result;
        }

        /// <summary>
        /// 判断当前用户在当前应用程序中是否存在此角色Key
        /// </summary>
        /// <param name="roleKey">角色Key</param>
        /// <returns></returns>
        public static bool HasRole(RoleKey roleKey, string loginUserName)
        {
            bool result = false;
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            var systemRoleList = authServiceSoapClient.GetUserFunction(SystemDataConst.ApplicationID, loginUserName).ToList<SystemRole>();
            if (systemRoleList != null && systemRoleList.Count > 0)
            {
                result = systemRoleList.Find(x => { return x.RoleName == roleKey.Key; }) != null;
            }
            return result;
        }

        /// <summary>
        /// 获取当权用户在当前应用程序中的所有权限Key
        /// </summary>
        /// <returns></returns>
        public static List<AuthKey> GetUserAuthKey(string loginUserName)
        {
            List<AuthKey> authKeyList = new List<AuthKey>();
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            var result = authServiceSoapClient.GetUserAuthKey(SystemDataConst.ApplicationID, loginUserName);
            foreach (var item in result)
            {
                authKeyList.Add(new AuthKey(item));
            }
            return authKeyList;
        }

        /// <summary>
        /// 获取当权用户在当前应用程序中的所有角色Key
        /// </summary>
        /// <returns></returns>
        public static List<RoleKey> GetUserRoleKey(string loginUserName)
        {
            List<RoleKey> roleKeyList = new List<RoleKey>();
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            var result = authServiceSoapClient.GetUserFunction(SystemDataConst.ApplicationID, loginUserName);
            foreach (var item in result)
            {
                roleKeyList.Add(new RoleKey(item.RoleName));
            }
            return roleKeyList;
        }

        public static List<ApplicationUser> GetAllSystemUser()
        {
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            var result = authServiceSoapClient.GetSystemUserByApplicationID(SystemDataConst.ApplicationID);
            List<ApplicationUser> list = new List<ApplicationUser>();
            foreach (var item in result)
            {
                list.Add(new ApplicationUser(item));
            }
            return list;
        }
    }

    /// <summary>
    /// 权限授权验证
    /// </summary>
    public class PermissionAuthorizeAttribute : ActionFilterAttribute
    {
        public List<string> RequiredPermissionNames { get; private set; }

        public PermissionAuthorizeAttribute(params string[] requiredPermissions)
        {
            this.RequiredPermissionNames = new List<string>();
            this.RequiredPermissionNames.AddRange(requiredPermissions);
            this.Order = 1;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.User.Identity.Name;

            if (!AuthorizeManager.HasFunction(new AuthKey(RequiredPermissionNames.FirstOrDefault()), user))
            {
                filterContext.Result = UnAuthorizedResult();
            }
        }

        public ActionResult UnAuthorizedResult()
        {
            return new RedirectResult("/Account/AccessDenied");
        }
    }

    public class AuthKey
    {
        public AuthKey(string key)
        {
            if (key == null || key.Trim().Length <= 0)
            {
                throw new Exception("The auth key cannot be null or empty!");
            }
            Key = key;
        }

        public string Key
        {
            get;
            private set;
        }
    }

    public class RoleKey
    {
        public RoleKey(string key)
        {
            if (key == null || key.Trim().Length <= 0)
            {
                throw new Exception("The auth key cannot be null or empty!");
            }
            Key = key;
        }

        public string Key
        {
            get;
            private set;
        }
    }

    public class AuthKeyConst
    {
        #region [全局汇总看板]
        /// <summary>
        /// 仓库业务分部
        /// </summary>
        public readonly static AuthKey Global_WarehouseBusinessBranch = new AuthKey("Global_WarehouseBusinessBranch");
        /// <summary>
        /// 农资汇总看板
        /// </summary>
        public readonly static AuthKey Global_FertilizerSummaryBoard = new AuthKey("Global_FertilizerSummaryBoard");
        /// <summary>
        /// 仓库汇总看板
        /// </summary>
        public readonly static AuthKey Global_WarehouseSummaryBoard = new AuthKey("Global_WarehouseSummaryBoard");

        #endregion

        #region [入库报表]
        /// <summary>
        /// 报表_入库_入库汇总报表
        /// </summary>
        public readonly static AuthKey Report_Storage_InboundReport = new AuthKey("Report_Storage_InboundReport");
        /// <summary>
        /// 报表_入库_入库明细查询
        /// </summary>
        public readonly static AuthKey Report_Storage_PurchaseDetailByReport = new AuthKey("Report_Storage_PurchaseDetailByReport");
        /// <summary>
        /// 报表_入库_收货明细查询
        /// </summary>
        public readonly static AuthKey Report_Storage_ReceiptDetailReport = new AuthKey("Report_Storage_ReceiptDetailReport");
        /// <summary>
        /// 报表_入库_收货上架时间统计表
        /// </summary>
        public readonly static AuthKey Report_Storage_ReceiptAndDeliveryDateReport = new AuthKey("Report_Storage_ReceiptAndDeliveryDateReport");

        /// <summary>
        /// 报表_入库_退货单查询
        /// </summary>
        public readonly static AuthKey Report_Storage_ReturnOrderReport = new AuthKey("Report_Storage_ReturnOrderReport");
        #endregion

        #region [出库报表]
        /// <summary>
        /// 报表_出库_出库复核工时报表
        /// </summary>
        public readonly static AuthKey Report_Outbound_OutboundTransferOrderReport = new AuthKey("Report_Outbound_OutboundTransferOrderReport");
        /// <summary>
        /// 报表_出库_出库捡货工时报表
        /// </summary>
        public readonly static AuthKey Report_Outbound_PickingTimeSpanReport = new AuthKey("Report_Outbound_PickingTimeSpanReport");
        /// <summary>
        /// 报表_出库_出库单商品汇总报表
        /// </summary>
        public readonly static AuthKey Report_Outbound_OutboundSkuReport = new AuthKey("Report_Outbound_OutboundSkuReport");
        /// <summary>
        /// 报表_出库_出库汇总报表
        /// </summary>
        public readonly static AuthKey Report_Outbound_OutboundSummaryReport = new AuthKey("Report_Outbound_OutboundSummaryReport");
        /// <summary>
        /// 报表_出库_出库箱数统计报表
        /// </summary>
        public readonly static AuthKey Report_Outbound_OutboundBoxReport = new AuthKey("Report_Outbound_OutboundBoxReport");
        /// <summary>
        /// 报表_出库_出库异常汇总报告
        /// </summary>
        public readonly static AuthKey Report_Outbound_OutboundExceptionReport = new AuthKey("Report_Outbound_OutboundExceptionReport");

        /// <summary>
        ///报表_出库_出库处理时间统计表
        /// </summary>
        public readonly static AuthKey Report_Outbound_OutboundHandleDateReport = new AuthKey("Report_Outbound_OutboundHandleDateReport");
        /// <summary>
        /// 报表_出库_出库明细查询
        /// </summary>
        public readonly static AuthKey Report_Outbound_OutboundDetailReport = new AuthKey("Report_Outbound_OutboundDetailReport");
        /// <summary>
        /// 报表_出库_整散箱商品明细报表
        /// </summary>
        public readonly static AuthKey Report_Outbound_OutboundPackReport = new AuthKey("Report_Outbound_OutboundPackReport");
        /// <summary>
        /// 报表_出库_整散箱装箱明细报表
        /// </summary>
        public readonly static AuthKey Report_Outbound_OutboundPackSkuReport = new AuthKey("Report_Outbound_OutboundPackSkuReport");
        /// <summary>
        /// 报表_出库_农资出库商品报表
        /// </summary>
        public readonly static AuthKey Report_Outbound_FertilizerOutboundSkuReport = new AuthKey("Report_Outbound_FertilizerOutboundSkuReport");
        #endregion

        #region [库存报表]
        /// <summary>
        /// 报表_库存_库存汇总报告
        /// </summary>
        public readonly static AuthKey Report_Stock_InvSkuReport = new AuthKey("Report_Stock_InvSkuReport");
        /// <summary>
        /// 报表_库存_临期批次库存查询
        /// </summary>
        public readonly static AuthKey Report_Stock_ExpiryInvLotBySkuReport = new AuthKey("Report_Stock_ExpiryInvLotBySkuReport");
        /// <summary>
        /// 报表_库存_货位批次库存查询
        /// </summary>
        public readonly static AuthKey Report_Stock_InvLotLocLpnBySkuReport = new AuthKey("Report_Stock_InvLotLocLpnBySkuReport");
        /// <summary>
        /// 报表_库存_批次库存查询
        /// </summary>
        public readonly static AuthKey Report_Stock_InvLotBySkuReport = new AuthKey("Report_Stock_InvLotBySkuReport");
        /// <summary>
        /// 报表_库存_货位库存查询
        /// </summary>
        public readonly static AuthKey Report_Stock_InvLocBySkuReport = new AuthKey("Report_Stock_InvLocBySkuReport");
        #endregion

        #region [库内作业]
        /// <summary>
        /// 报表_库内_收发货明细报表
        /// </summary>
        public readonly static AuthKey Report_InStock_ReceivedAndSendSkuReport = new AuthKey("Report_InStock_ReceivedAndSendSkuReport");
        /// <summary>
        /// 报表_库内_仓库进销存报表
        /// </summary>
        public readonly static AuthKey Report_InStock_FinanceInvoicingReport = new AuthKey("Report_InStock_FinanceInvoicingReport");
        /// <summary>
        /// 报表_库内_损益明细报表
        /// </summary>
        public readonly static AuthKey Report_InStock_AdjustmentDetailReport = new AuthKey("Report_InStock_AdjustmentDetailReport");
        /// <summary>
        /// 报表_库内_冻结商品明细报表
        /// </summary>
        public readonly static AuthKey Report_InStock_FrozenSkuReport = new AuthKey("Report_InStock_FrozenSkuReport");
        /// <summary>
        /// 报表_库内_SN管理报表
        /// </summary>
        public readonly static AuthKey Report_InStock_SNManageReport = new AuthKey("Report_InStock_SNManageReport");
        /// <summary>
        /// 报表_库内_B2C结算报表
        /// </summary>
        public readonly static AuthKey Report_InStock_BalanceReport = new AuthKey("Report_InStock_BalanceReport");
        /// <summary>
        /// 报表_库内_商品包装查询报表
        /// </summary>
        public readonly static AuthKey Report_InStock_SkuPackReport = new AuthKey("Report_InStock_SkuPackReport");
        /// <summary>
        /// 报表_库内_移仓单报表
        /// </summary>
        public readonly static AuthKey Report_InStock_TransferinventoryReport = new AuthKey("Report_InStock_TransferinventoryReport");

        /// <summary>
        /// 冻结商品导出
        /// </summary>
        public readonly static AuthKey Report_InStock_FrozenSkuReportExport = new AuthKey("Report_InStock_FrozenSkuReportExport");
        #endregion

        #region [报表导出按钮]
        /// <summary>
        /// 报表_B2C结算报表_导出
        /// </summary>
        public readonly static AuthKey Report_BalanceReport_Export = new AuthKey("Report_BalanceReport_Export");
        /// <summary>
        /// 报表_仓库进销存报表_导出
        /// </summary>
        public readonly static AuthKey Report_FinanceInvoicingReport_Export = new AuthKey("Report_FinanceInvoicingReport_Export");
        /// <summary>
        /// 报表_入库汇总报表_导出	
        /// </summary>
        public readonly static AuthKey Report_InboundReport_Export = new AuthKey("Report_InboundReport_Export");
        /// <summary>
        /// 报表_货位库存查询_导出
        /// </summary>
        public readonly static AuthKey Report_InvLocBySkuReport_Export = new AuthKey("Report_InvLocBySkuReport_Export");
        /// <summary>
        /// 报表_库存汇总报告_导出
        /// </summary>
        public readonly static AuthKey Report_InvSkuReport_Export = new AuthKey("Report_InvSkuReport_Export");
        /// <summary>
        /// 报表_出库箱数统计报表_导出
        /// </summary>
        public readonly static AuthKey Report_OutboundBoxReport_Export = new AuthKey("Report_OutboundBoxReport_Export");
        /// <summary>
        /// 报表_出库明细查询_导出
        /// </summary>
        public readonly static AuthKey Report_OutboundDetailReport_Export = new AuthKey("Report_OutboundDetailReport_Export");
        /// <summary>
        /// 报表_退货单_导出
        /// </summary>
        public readonly static AuthKey Report_ReturnOrderReport_Export = new AuthKey("Report_ReturnOrderReport_Export");
        /// <summary>
        /// 报表_出库异常汇总报告_导出
        /// </summary>
        public readonly static AuthKey Report_OutboundExceptionReport_Export = new AuthKey("Report_OutboundExceptionReport_Export");
        /// <summary>
        /// 报表_出库处理时间统计表_导出
        /// </summary>
        public readonly static AuthKey Report_OutboundHandleDateReport_Export = new AuthKey("Report_OutboundHandleDateReport_Export");
        /// <summary>
        /// 报表_整散箱商品明细报表_导出
        /// </summary>
        public readonly static AuthKey Report_OutboundPackReport_Export = new AuthKey("Report_OutboundPackReport_Export");
        /// <summary>
        /// 报表_整散箱装箱明细报表_导出
        /// </summary>
        public readonly static AuthKey Report_OutboundPackSkuReport_Export = new AuthKey("Report_OutboundPackSkuReport_Export");
        /// <summary>
        /// 报表_出库单商品汇总报表_导出
        /// </summary>
        public readonly static AuthKey Report_OutboundSkuReport_Export = new AuthKey("Report_OutboundSkuReport_Export");
        /// <summary>
        /// 报表_出库汇总报表_导出	
        /// </summary>
        public readonly static AuthKey Report_OutboundSummaryReport_Export = new AuthKey("Report_OutboundSummaryReport_Export");
        /// <summary>
        /// /报表_入库明细查询_导出
        /// </summary>
        public readonly static AuthKey Report_PurchaseDetailByReport_Export = new AuthKey("Report_PurchaseDetailByReport_Export");
        /// <summary>
        /// 报表_收货上架时间统计表_导出
        /// </summary>
        public readonly static AuthKey Report_ReceiptAndDeliveryDateReport_Export = new AuthKey("Report_ReceiptAndDeliveryDateReport_Export");
        /// <summary>
        /// 报表_收货明细查询_导出
        /// </summary>
        public readonly static AuthKey Report_ReceiptDetailReport_Export = new AuthKey("Report_ReceiptDetailReport_Export");
        /// <summary>
        /// 报表_收发货明细报表_导出
        /// </summary>
        public readonly static AuthKey Report_ReceivedAndSendSkuReport_Export = new AuthKey("Report_ReceivedAndSendSkuReport_Export");
        /// <summary>
        /// 报表_商品包装查询报表_导出
        /// </summary>
        public readonly static AuthKey Report_SkuPackReport_Export = new AuthKey("Report_SkuPackReport_Export");
        /// <summary>
        /// 报表_农资出库商品报表_导出
        /// </summary>
        public readonly static AuthKey Report_FertilizerOutboundSkuReport_Export = new AuthKey("Report_FertilizerOutboundSkuReport_Export");
        /// <summary>
        /// 报表_渠道库存报表_导出
        /// </summary>
        public readonly static AuthKey Report_ChannelInventoryReport_Export = new AuthKey("Report_ChannelInventoryReport_Export");
        #endregion
    }

    public static class RoleKeyConst
    {
        #region [PM]

        public readonly static RoleKey CMS_Item_PMLeader = new RoleKey("CMS_Item_PMLeader");

        #endregion
    }

    public class SystemDataConst
    {
        #region [应用程序ID]

        public readonly static string ApplicationID = ConfigurationManager.AppSettings["AuthCenterApplicationID"];

        #endregion

        public static string LoginUser = "LoginUser";

        public static string SessionLoginUserName = "LoginUserName";

        public static string SessionLoginDisplayUserName = "LoginDisplayUserName";

        public static string SessionLoginUserID = "LoginUserID";

        public static string SessionLoginWarehouseSysId = "SessionLoginWarehouseSysId";

        #region Demo数据设置

        //实际项目中请使用一下代码获取UserName 
        //public static string UserName = ApplicationUser

        //当前Demo 使用固定账号 Winnie 没有审核权限
        //public readonly static string UserName = "Winnie";

        //当前Demo 使用固定账号 Franky 有审核权限
        //public readonly static string UserName = "wms_admin";
        #endregion
    }

    #region 辅助工具类

    public static class UtilityTools
    {
        /// <summary>
        /// 实体类复制
        /// </summary>
        /// <param name="objold"></param>
        /// <param name="objnew"></param>
        public static void EntityCopy(object objold, object objnew)
        {
            Type myType = objold.GetType(),
                myType2 = objnew.GetType();
            PropertyInfo currobj = null;
            if (myType == myType2)
            {
                PropertyInfo[] myProperties = myType.GetProperties();
                for (int i = 0; i < myProperties.Length; i++)
                {
                    currobj = objold.GetType().GetProperties()[i];
                    currobj.SetValue(objnew, currobj.GetValue(objold, null), null);
                }
            }
        }

        /// <summary>
        /// 实体类克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RealObject"></param>
        /// <returns></returns>
        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }
    }

    #endregion
}