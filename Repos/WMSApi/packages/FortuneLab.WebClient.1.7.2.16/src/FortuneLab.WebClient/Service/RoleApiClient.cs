using FortuneLab.Models;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using FortuneLab.WebClient.Models;
using FortuneLab.WebClient.Service.API;
using System;
using System.Collections.Generic;

namespace FortuneLab.WebClient.Service
{
    public class RoleApiClient
    {
        public static ApiResponse<Page<Role>> GetRoleList(ListQuery query)
        {
            var response = ApiClient.NExecute<Page<Role>>(ApiClientConst.BenzAPIURL, "role/role/getList", query);
            return response;
        }

        public static ApiResponse<Guid> CreateRoles(CoreQuery query, Role model)
        {
            var response = ApiClient.NExecute<Guid>(ApiClientConst.BenzAPIURL, "role/role/add", query, MethodType.Post, model);
            return response;
        }

        public static ApiResponse<Role> GetRoleById(LoginQuery query, Guid roleId)
        {
            query.ParmsObj = new { roleId };
            var response = ApiClient.NExecute<Role>(ApiClientConst.BenzAPIURL, "role/role/get", query);
            return response;
        }

        public static ApiResponse<Role> UpdateRole(CoreQuery query, Role model)
        {
            return ApiClient.NExecute<Role>(ApiClientConst.BenzAPIURL, "role/role/update", query, MethodType.Post, model);
        }

        public static ApiResponse<Role> DeleteRole(LoginQuery query, Guid roleId)
        {
            query.ParmsObj = new { roleId };
            return ApiClient.NExecute<Role>(ApiClientConst.BenzAPIURL, "role/role/delete", query, MethodType.Delete);
        }

        public static ApiResponse<Page<SystemFunction>> GetPermissionRecordList(ListQuery query)
        {
            var response = ApiClient.NExecute<Page<SystemFunction>>(ApiClientConst.BenzAPIURL, "role/permission/getList", query);
            return response;
        }

        public static ApiResponse<SystemFunction> UpdatePermissionRecord(LoginQuery query, SystemFunction model)
        {
            return ApiClient.NExecute<SystemFunction>(ApiClientConst.BenzAPIURL, "role/permission/updatePermissionRecords", query, MethodType.Post, model);
        }

        public static ApiResponse<SystemFunction> UpdateRolePermissions(LoginQuery query, List<SystemFunction> permissionRecords)
        {
            return ApiClient.NExecute<SystemFunction>(ApiClientConst.BenzAPIURL, "role/rolePermissions/update", query, MethodType.Post, permissionRecords);
        }
    }
}
