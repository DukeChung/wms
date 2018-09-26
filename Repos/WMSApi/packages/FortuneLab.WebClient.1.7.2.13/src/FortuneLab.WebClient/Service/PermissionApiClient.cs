using FortuneLab.Models;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using FortuneLab.WebClient.Models;
using FortuneLab.WebClient.Service.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebClient.Service
{
    public class PermissionApiClient
    {
        public static ApiResponse<List<string>> GetSystemFunctionAuthKeies(SessionQuery query, bool isFilterByApplication = true)
        {
            query.ParmsObj = new { isFilterByApplication };
            return ApiClient.NExecute<List<string>>(ApiClientConst.BenzAPIURL, "permissions/systemFunction/currentUser/getAuthKeyList", query, MethodType.Get);
        }

        public static ApiResponse<List<string>> GetSystemRoleNames(SessionQuery query, bool isFilterByApplication = true)
        {
            query.ParmsObj = new { isFilterByApplication };
            return ApiClient.NExecute<List<string>>(ApiClientConst.BenzAPIURL, "permissions/systemRole/currentUser/getRoleNameList", query, MethodType.Get);
        }
    }
}
