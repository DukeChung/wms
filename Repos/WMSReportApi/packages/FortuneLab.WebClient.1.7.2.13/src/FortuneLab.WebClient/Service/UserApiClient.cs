using FortuneLab.Models;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using FortuneLab.WebClient.Models;


namespace FortuneLab.WebClient.Service.API
{
    public class UserApiClient
    {
        public static ApiResponse<Page<User>> GetUserList(ListQuery query, string userTypeIds)
        {
            query.ParmsObj = new { userTypeIds };
            return ApiClient.NExecute<Page<User>>(ApiClientConst.BenzAPIURL, "users/user/getList", query);
        }

        public static ApiResponse<User> GetUser(CoreQuery query, long userId)
        {
            query.ParmsObj = new { userId };
            var response = ApiClient.NExecute<User>(ApiClientConst.BenzAPIURL, "users/user/get", query);
            return response;
        }
    }
}
