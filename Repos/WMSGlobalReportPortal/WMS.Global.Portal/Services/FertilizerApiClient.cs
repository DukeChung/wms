using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS.Global.Portal.Services
{
    public class FertilizerApiClient
    {
        private static readonly FertilizerApiClient instance = new FertilizerApiClient();

        private FertilizerApiClient() { }

        public static FertilizerApiClient GetInstance()
        {
            return instance;
        }

        public ApiResponse<List<FertilizerRORadarGlobalDto>> GetFertilizerRORadarList(FertilizerRORadarGlobalQuery request)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Post<List<FertilizerRORadarGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetFertilizerRORadarList", query, request);
        }

        public ApiResponse<List<FertilizerInvRadarGlobalDto>> GetFertilizerInvRadarList(FertilizerInvRadarGlobalQuery request)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Post<List<FertilizerInvRadarGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetFertilizerInvRadarList", query, request);
        }

        public ApiResponse<List<FertilizerInvPieGlobalDto>> GetFertilizerInvPieList(FertilizerInvPieGlobalQuery request)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Post<List<FertilizerInvPieGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetFertilizerInvPieList", query, request);
        }
    }
}