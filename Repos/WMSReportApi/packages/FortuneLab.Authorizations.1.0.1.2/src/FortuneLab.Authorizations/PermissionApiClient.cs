using System.Collections.Generic;
using System.Configuration;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;

namespace FortuneLab.Authorizations
{
    public static class PermissionApiClient
    {
        public static ApiResponse<List<string>> GetSystemFunctionAuthKeies(long userId, bool isFilterByApplication = false)
        {
            var query = new CoreQuery() { ParmsObj = new { userId, isFilterByApplication } };
            return ApiClient.NExecute<List<string>>(ApiClientConst.BenzAPIURL, "permissions/systemFunction/authKeyList", query, MethodType.Get);
        }

        public static ApiResponse<List<string>> GetSystemRoleNames(long userId, bool isFilterByApplication = false)
        {
            var query = new CoreQuery() { ParmsObj = new { userId, isFilterByApplication } };
            return ApiClient.NExecute<List<string>>(ApiClientConst.BenzAPIURL, "permissions/systemRole/roleNameList", query, MethodType.Get);
        }
    }

    internal static class ApiClientConst
    {
        private const string FxApiUrlConfigName = "BenzAPIURL";
        public static readonly string BenzAPIURL = ConfigurationManager.AppSettings[FxApiUrlConfigName];
    }
}
