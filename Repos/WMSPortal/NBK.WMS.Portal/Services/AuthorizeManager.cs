using NBK.WMS.Portal.AuthServiceReference;
using NBK.WMS.Portal.Models;
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
        #region [容器管理]

        public readonly static AuthKey BaseData_Container_Create = new AuthKey("BaseData_Container_Create");

        public readonly static AuthKey BaseData_Container_Update = new AuthKey("BaseData_Container_Update");

        #endregion

        #region [商品管理]
        public readonly static AuthKey BaseData_Sku = new AuthKey("BaseData_Sku");

        public readonly static AuthKey BaseData_Sku_Create = new AuthKey("BaseData_Sku_Create");

        public readonly static AuthKey BaseData_Sku_Update = new AuthKey("BaseData_Sku_Update");

        public readonly static AuthKey BaseData_Sku_Delete = new AuthKey("BaseData_Sku_Delete");
        #endregion

        #region [收货单管理]
        public readonly static AuthKey Inbound_Receipt_View = new AuthKey("Inbound_Receipt_View");
        #endregion

        #region 损益
        public const string Inventory_Ajustment_Auditing = "Inventory_Ajustment_Auditing";
        #endregion 损益

        #region [盘点查询]
        public readonly static AuthKey Inventory_StockTake_View = new AuthKey("Inventory_StockTake_View");

        public readonly static AuthKey Inventory_StockTake_AddStockTake = new AuthKey("Inventory_StockTake_AddStockTake");

        public readonly static AuthKey Inventory_StockTake_Replay = new AuthKey("Inventory_StockTake_Replay");

        public readonly static AuthKey Inventory_StockTake_CreateAdj = new AuthKey("Inventory_StockTake_CreateAdj");
        #endregion

        #region 库存汇总报表导出
        public readonly static AuthKey Report_InvSkuReportExport = new AuthKey("Report_InvSkuReport_Export");

        /// <summary>
        /// 货位库存查询导出
        /// </summary>
        public readonly static AuthKey Report_InvLocBySkuReport = new AuthKey("Report_InvLocBySkuReport_Export");

        public readonly static AuthKey Report_ReceiptDetailReportExport = new AuthKey("Report_ReceiptDetailReportExport");

        public readonly static AuthKey Report_OutboundDetailReportExport = new AuthKey("Report_OutboundDetailReportExport");
        #endregion

        #region [报表]
        public readonly static AuthKey Report_InvTransBySkuReport_DocOrderView = new AuthKey("Report_InvTransBySkuReport_DocOrderView");

        public readonly static AuthKey Report_FinanceInvoicingReport_Export = new AuthKey("Report_FinanceInvoicingReport_Export");

        public readonly static AuthKey Report_InboundReport_Export = new AuthKey("Report_InboundReport_Export");

        public readonly static AuthKey Report_OutboundBoxReport_Export = new AuthKey("Report_OutboundBoxReport_Export");

        public readonly static AuthKey Report_OutboundPackReportExport = new AuthKey("Report_OutboundPackReportExport");

        public readonly static AuthKey Report_OutboundPackSkuReportExport = new AuthKey("Report_OutboundPackSkuReportExport");
        /// <summary>
        /// 商品包装查询报表——导出
        /// </summary>
        public readonly static AuthKey Report_SkuPackReport_Export = new AuthKey("Report_SkuPackReport_Export");

        /// <summary>
        /// B2C结算报表
        /// </summary>
        public readonly static AuthKey Report_BalanceReport_Export = new AuthKey("Report_BalanceReport_Export");

        /// <summary>
        /// 出库汇总报表
        /// </summary>
        public readonly static AuthKey Report_OutboundSummaryReport_Export = new AuthKey("Report_OutboundSummaryReport_Export");

        /// <summary>
        /// 冻结商品导出
        /// </summary>
        public readonly static AuthKey Report_FrozenSkuReportExport = new AuthKey("Report_FrozenSkuReportExport");

        #region [增值服务]
        public readonly static AuthKey VAS_Assembly_View = new AuthKey("VAS_Assembly_View");
        public readonly static AuthKey VAS_Assembly_Add = new AuthKey("VAS_Assembly_Add");
        public readonly static AuthKey QualityControl_View_Edit = new AuthKey("QualityControl_View_Edit");
        public readonly static AuthKey QualityControl_View_Finish = new AuthKey("QualityControl_View_Finish");
        public readonly static AuthKey QualityControl_View_CreateAdj = new AuthKey("QualityControl_View_CreateAdj");
        #endregion

        /// <summary>
        /// 渠道库存导出
        /// </summary>
        public readonly static AuthKey Report_ChannelInventoryReport_Export = new AuthKey("Report_ChannelInventoryReport_Export");

        #endregion

        #region 采购单
        public readonly static AuthKey Purchase_View_Obsolete = new AuthKey("Purchase_View_Obsolete");
        public readonly static AuthKey Purchase_View_Close = new AuthKey("Purchase_View_Close");
        public readonly static AuthKey Purchase_View_QC = new AuthKey("Purchase_View_QC");
        public readonly static AuthKey Purchase_View_AutoShelves = new AuthKey("Purchase_View_AutoShelves");
        public readonly static AuthKey Purchase_View_AppointBatchNumber = new AuthKey("Purchase_View_AppointBatchNumber");
        public readonly static AuthKey Purchase_View_BusinessType = new AuthKey("Purchase_View_BusinessType");
        public readonly static AuthKey Purchase_View_CancelReceipt = new AuthKey("Purchase_View_CancelReceipt");
        #endregion

        #region 装箱单
        /// <summary>
        /// 取消装箱
        /// </summary>
        public readonly static AuthKey Vanning_View_Cancel = new AuthKey("Vanning_View_Cancel");
        #endregion

        #region 出库单
        /// <summary>
        /// 退货入库
        /// </summary>
        public readonly static AuthKey Outbound_View_Return = new AuthKey("Outbound_View_Return");

        public readonly static AuthKey Outbound_View_Cancel = new AuthKey("Outbound_View_Cancel");
        public readonly static AuthKey Outbound_View_PartReturn = new AuthKey("Outbound_View_PartReturn");
        public readonly static AuthKey Outbound_View_CreatePrebulkPack = new AuthKey("Outbound_View_CreatePrebulkPack");
        public readonly static AuthKey Outbound_View_CreateTMSPackNumber = new AuthKey("Outbound_View_CreateTMSPackNumber");
        public readonly static AuthKey Outbound_View_Exception = new AuthKey("Outbound_View_Exception");
        public readonly static AuthKey Outbound_View_Obsolete = new AuthKey("Outbound_View_Obsolete");
        public readonly static AuthKey Outbound_View_AllocationShip = new AuthKey("Outbound_View_AllocationShip");
        public readonly static AuthKey Outbound_View_QuickDelivery = new AuthKey("Outbound_View_QuickDelivery");

        /// <summary>
        /// 绑定和解绑
        /// </summary>
        public readonly static AuthKey Outbound_View_BindPreack_UnBind = new AuthKey("Outbound_View_BindPreack_UnBind");

        public readonly static AuthKey Report_OutboundExceptionReport_Export = new AuthKey("Report_OutboundExceptionReport_Export");
        #endregion

        #region 入库明细报表
        /// <summary>
        /// 入库明细报表导出
        /// </summary>
        public readonly static AuthKey Report_PurchaseDetailReport_Export = new AuthKey("Report_PurchaseDetailReport_Export");
        #endregion

        #region 预包装单
        /// <summary>
        /// 新增
        /// </summary>
        public readonly static AuthKey OrderManger_PrePack_Add = new AuthKey("OrderManger_PrePack_Add");
        /// <summary>
        /// 编辑
        /// </summary>
        public readonly static AuthKey OrderManger_PrePack_Edit = new AuthKey("OrderManger_PrePack_Edit");
        /// <summary>
        /// 删除
        /// </summary>
        public readonly static AuthKey OrderManger_PrePack_Delete = new AuthKey("OrderManger_PrePack_Delete");
        /// <summary>
        /// 导入
        /// </summary>
        public readonly static AuthKey OrderManger_PrePack_Export = new AuthKey("OrderManger_PrePack_Export");
        /// <summary>
        /// 预包装管理_复制
        /// </summary>
        public readonly static AuthKey OrderManger_PrePack_Copy = new AuthKey("OrderManger_PrePack_Copy");

        public readonly static AuthKey OrderManagement_PrebulkPack_Edit = new AuthKey("OrderManger_PrebulkPack_Edit");

        public readonly static AuthKey OrderManagement_PrebulkPack_Delete = new AuthKey("OrderManger_PrebulkPack_Delete");

        /// <summary>
        /// 散货导入
        /// </summary>
        public readonly static AuthKey OrderManger_PrebulkPack_Export = new AuthKey("OrderManger_PrebulkPack_Export");
        #endregion

        #region 库存管理

        public readonly static AuthKey Inventory_Frozen_FrozenRequest = new AuthKey("Inventory_Frozen_FrozenRequest");

        #endregion

        #region[库位变更管理]
        /// <summary>
        /// 库位变更导入
        /// </summary>
        public readonly static AuthKey StockMovement_PrePack_Export = new AuthKey("StockMovement_PrePack_Export");
        #endregion

        #region 工单管理
        /// <summary>
        /// 编辑
        /// </summary>
        public readonly static AuthKey VAS_WorkManger_Edit = new AuthKey("VAS_WorkManger_Edit");
        /// <summary>
        /// 作废
        /// </summary>
        public readonly static AuthKey VAS_WorkManger_Cancel = new AuthKey("VAS_WorkManger_Cancel");
        #endregion

        #region 收发货明细
        public readonly static AuthKey Report_ReceivedAndSendSkuReportExport = new AuthKey("Report_ReceivedAndSendSkuReportExport");
        #endregion

        #region  出库处理时间统计表
        public readonly static AuthKey Report_OutboundHandleDateStatisticsReport_Export = new AuthKey("Report_OutboundHandleDateStatisticsReport_Export");
        #endregion

        #region 收货上架时间统计表
        public readonly static AuthKey Report_ReceiptAndDeliveryDateReport_Export = new AuthKey("Report_ReceiptAndDeliveryDateReport_Export");
        #endregion

        #region 交接管理
        public readonly static AuthKey OrderManger_OutboundTransferOrder_Print = new AuthKey("OrderManger_OutboundTransferOrder_Print");
        public readonly static AuthKey OrderManger_OutboundTransferOrder_Edit = new AuthKey("OrderManger_OutboundTransferOrder_Edit");
        #endregion

        #region 移仓单
        public readonly static AuthKey OrderManger_Transferinventory_Obsolete = new AuthKey("OrderManger_Transferinventory_Obsolete");
        #endregion

        #region 拣货单
        public readonly static AuthKey Outbound_PickDetail_PickingOperation = new AuthKey("Outbound_PickDetail_PickingOperation");
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