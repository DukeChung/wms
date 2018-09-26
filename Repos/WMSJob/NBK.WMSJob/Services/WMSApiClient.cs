
using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace NBK.WMSJob
{
    public class WMSApiClient
    {
        private static readonly WMSApiClient instance = new WMSApiClient();

        private WMSApiClient()
        {
        }

        public static WMSApiClient GetInstance()
        {
            return instance;
        }

        #region WMS业务Api测试连接
        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns></returns>
        public bool TestConnection()
        {
            try
            {
                string rsp = AppConst.HttpGet(AppConst.WmsApiUrl + "api/Base/TestConnection");
                if (rsp == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 更新报表缓存

        public bool GetSparkLineSummaryDto()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetSparkLineSummaryDto?flag=true");
            return true;
        }

        /// <summary>
        /// 入库类型环比
        /// </summary>
        /// <returns></returns>
        public bool GetPurchaseTypePieDto()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetPurchaseTypePieDto?flag=true");
            return true;
        }

        /// <summary>
        /// 出库类型环比
        /// </summary>
        /// <returns></returns>
        public bool GetOutboundTypePieDto()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetOutboundTypePieDto?flag=true");
            return true;
        }

        /// <summary>
        /// 全局收发存
        /// </summary>
        /// <returns></returns>
        public bool GetStockInOutData()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetStockInOutData?flag=true");
            return true;
        }

        /// <summary>
        /// 入库分布
        /// </summary>
        /// <returns></returns>
        public bool GetWareHouseReceiptQtyList()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetWareHouseReceiptQtyList?flag=true");
            return true;
        }

        /// <summary>
        /// 出库分布
        /// </summary>
        /// <returns></returns>
        public bool GetWareHouseOutboundQtyList()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetWareHouseOutboundQtyList?flag=true");
            return true;
        }

        /// <summary>
        /// 库存分布
        /// </summary>
        /// <returns></returns>
        public bool GetWareHouseQtyList()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetWareHouseQtyList?flag=true");
            return true;
        }

        /// <summary>
        /// 库龄分析
        /// </summary>
        /// <returns></returns>
        public bool GetStockAgeGroup()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetStockAgeGroup?flag=true");
            return true;
        }

        /// <summary>
        /// 畅销商品
        /// </summary>
        /// <returns></returns>
        public bool GetSkuSellingTop10()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetSkuSellingTop10?flag=true");
            return true;
        }

        /// <summary>
        /// 滞销商品
        /// </summary>
        /// <returns></returns>
        public bool GetSkuUnsalableTop10()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetSkuUnsalableTop10?flag=true");
            return true;
        }

        /// <summary>
        /// 渠道库存占比
        /// </summary>
        /// <returns></returns>
        public bool GetChannelPieData()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetChannelPieData?flag=true");
            return true;
        }

        /// <summary>
        /// 获取服务站发货Top10
        /// </summary>
        /// <returns></returns>
        public bool GetServiceStationOutboundTop10()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetServiceStationOutboundTopTen?flag=true");
            return true;
        }

        /// <summary>
        /// 退货入库Top10
        /// </summary>
        /// <returns></returns>
        public bool GetReturnPurchase()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetReturnPurchase?flag=true");
            return true;
        }

        public bool GetFertilizerRORadarList()
        {
            var requset = new
            {
                SkuSysIds = new List<string>()
                {
                    "03fabcb1-3f89-47e2-a9b8-1107ab323d1a",
                    "59707948-4dc0-4d9a-832d-5b4a48bd3155",
                    "68037927-d35d-40ad-a65a-0ca0af824578",
                    "74cd11ce-0893-49c5-b4dc-6e862eb1144c",
                    "e1f23697-edc7-46f4-a5cd-d1398291fffe",
                    "cb74960d-009e-4266-a9ad-350a52697b26",
                    "e7d5bbb4-7ddf-4be5-b708-608fc1a82dea",
                    "fb0c1f73-73ba-40af-b43b-838b6f00c17e"
                }
            };

            var str = JsonConvert.SerializeObject(requset);
            var rsp = AppConst.HttpPost(AppConst.WmsReportApiUrl + "api/Global/GetFertilizerRORadarList?flag=true", str);
            return true;
        }


        public bool GetFertilizerInvRadarList()
        {
            var requset = new
            {
                SkuSysIds = new List<string>()
                {
                    "03fabcb1-3f89-47e2-a9b8-1107ab323d1a",
                    "59707948-4dc0-4d9a-832d-5b4a48bd3155",
                    "68037927-d35d-40ad-a65a-0ca0af824578",
                    "74cd11ce-0893-49c5-b4dc-6e862eb1144c",
                    "e1f23697-edc7-46f4-a5cd-d1398291fffe",
                    "cb74960d-009e-4266-a9ad-350a52697b26",
                    "e7d5bbb4-7ddf-4be5-b708-608fc1a82dea",
                    "fb0c1f73-73ba-40af-b43b-838b6f00c17e"
                }
            };

            var str = JsonConvert.SerializeObject(requset);
            var rsp = AppConst.HttpPost(AppConst.WmsReportApiUrl + "api/Global/GetFertilizerInvRadarList?flag=true", str);
            return true;
        }


        public bool GetFertilizerInvPieList()
        {
            var requset = new List<string>(){
                    "03fabcb1-3f89-47e2-a9b8-1107ab323d1a",
                    "59707948-4dc0-4d9a-832d-5b4a48bd3155",
                    "68037927-d35d-40ad-a65a-0ca0af824578",
                    "74cd11ce-0893-49c5-b4dc-6e862eb1144c",
                    "e1f23697-edc7-46f4-a5cd-d1398291fffe",
                    "cb74960d-009e-4266-a9ad-350a52697b26",
                    "e7d5bbb4-7ddf-4be5-b708-608fc1a82dea",
                    "fb0c1f73-73ba-40af-b43b-838b6f00c17e"
            };
            foreach (var item in requset)
            {
                var sysId = new
                {
                    SkuSysId = item
                };
                var str = JsonConvert.SerializeObject(sysId);
                var rsp = AppConst.HttpPost(AppConst.WmsReportApiUrl + "api/Global/GetFertilizerInvPieList?flag=true", str);
            }
            return true;
        }

        /// <summary>
        /// 获取仓库作业时间分布数据
        /// </summary>
        /// <returns></returns>
        public bool GetAccessBizList()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Global/GetAccessBizList?flag=true");
            return true;
        }

        /// <summary>
        /// 获取仓库作业时间分布数据
        /// </summary>
        /// <returns></returns>
        public bool GetWorkDistributionData()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetWorkDistributionData?flag=true");
            return true;
        }

        /// <summary>
        /// 获取仓库作业时间分布数据
        /// </summary>
        /// <returns></returns>
        public bool GetWorkDistributionByWarehouse()
        {
            var requset = new List<string>(){
                "24e4a797-fa07-487e-9144-77259f8ebe0e",
                "37312551-950b-11e6-8ed7-005056bd5942",
                "4431c99a-0f79-11e7-bca5-246e965399cc",
                "48690250-19e6-40ac-a140-c7cef7293978",
                "68eaffea-fb55-4a45-a4e0-93f475a54935",
                "6a693610-002d-4b35-a6d7-452a05ff43ff",
                "6de8985e-ca5a-45bd-9880-0d7b3ce9027f",
                "9826150a-a746-4d87-8445-1a2d9669555d",
                "b15f7053-f243-4593-8a70-85aaafea4336",
                "b2137483-a583-4c94-8483-dbf44c1fdd2e",
                "b84465eb-2d97-4470-b31c-da7d256ffbdd",
                "c1ab67bf-9ada-47cc-811d-4db3e95dee8c",
                "cdd42312-4d97-46f8-a682-c9f6dd31405d",
                "d36541a4-f7f3-47da-b660-4f71612dde97"
            };
            foreach (var item in requset)
            {
                var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetWorkDistributionByWarehouse?flag=true&sysId=" + item);
            }
            return true;
        }


        /// <summary>
        /// 获取仓库作业时间分布数据
        /// </summary>
        /// <returns></returns>
        public bool GetWorkDistributionPieData()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetWorkDistributionPieData?flag=true");
            return true;
        }
        #endregion

        #region 更新出库单经纬度 

        public int GetOutboundNoLngLatCount()
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/GetOutboundNoLngLatCount");
            if (rsp != null)
            {
                return Convert.ToInt32(rsp);
            }
            return 0;
        }

        public bool UpdataOutboundLngLat(int pageCount)
        {
            var rsp = AppConst.HttpGet(AppConst.WmsReportApiUrl + "api/Home/UpdataOutboundLngLat?pageCount=" + pageCount);
            if (rsp == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
