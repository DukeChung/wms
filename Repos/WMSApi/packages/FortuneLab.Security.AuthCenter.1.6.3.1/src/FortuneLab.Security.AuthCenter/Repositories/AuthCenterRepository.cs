using Abp.Dependency;
using Castle.Core.Logging;
using FortuneLab.ECService.Securities.Entities;
using FortuneLab.Repositories.Dapper;
using System;
using System.Linq;

namespace FortuneLab.Security.AuthCenter.Repositories
{
    public class AuthCenterRepository : DapperRepositoryBase<AuthCenterDbConnProvider, Guid>
    {
        private ILogger _logger;
        public AuthCenterRepository()
            : base()
        {
            this._logger = IocManager.Instance.Resolve<ILogger>();
        }

        public SystemUser GetByLoginNameAndPWD(string loginName, string pwd)
        {
            string sql = "dbo.Get_SystemUser_By_LoginNameAndPWD";
            var parms = new Dapper.DynamicParameters();
            parms.Add("@LoginName", loginName);
            parms.Add("@Password", pwd);
            var results = ExecuteStoreProcedure<SystemUser>(sql, parms, null);
            return results.FirstOrDefault();
        }

        public SystemUser SearchByLoginName(string loginName)
        {
            string sql = "Search_SystemUser_By_LoginName";
            var parms = new Dapper.DynamicParameters();
            parms.Add("@LoginName", string.Format("%{0}%", loginName));
            var results = ExecuteStoreProcedure<SystemUser>(sql, parms, null);
            return results.FirstOrDefault();
        }

        public UserDevice GetUserDevice(string sessionKey)
        {
            return Get<UserDevice>("select t.* from UserDevice t where SessionKey = @sessionKey", new { sessionKey = sessionKey });
        }

        public UserDevice GetUserDevice(long userSysId, int deviceType)
        {
            return Get<UserDevice>("select t.* from UserDevice t where UserSysId = @userSysId and DeviceType = @deviceType", new { userSysId, deviceType });
        }

        public bool GetApiUserAuth(string token, string userID, string funAuthKey)
        {
            _logger.Info("GetApiUserAuth Logger");
            var getApiUserAuthCmd = @"DECLARE @IsAuthPassed BIT = 0
SELECT 
	TOP(1) @IsAuthPassed = 1
FROM 
	[dbo].[SystemFunction] F WITH(NOLOCK)
	INNER JOIN [dbo].[RoleFunctions] RF WITH(NOLOCK)
		ON RF.[FunctionSysNO] = F.[SysNO] 
		AND RF.[ApplicationID] = F.[ApplicationID]
	INNER JOIN [dbo].[SystemRole] R WITH(NOLOCK)
		ON R.[SysNO] = RF.[RoleSysNO]
		AND R.[ApplicationID] = RF.[ApplicationID]
	INNER JOIN [dbo].[UserRoles] UR WITH(NOLOCK)
		ON UR.[RoleSysNO] = R.[SysNO]
		AND UR.[ApplicationID] = R.[ApplicationID]
	INNER JOIN [dbo].[SystemUser] U WITH(NOLOCK)
		ON U.[LoginName] = UR.[LoginName]
	INNER JOIN [dbo].[SystemApplication] A WITH(NOLOCK)
		ON A.[ApplicationID] = UR.[ApplicationID]
WHERE 
	U.[LoginName] = @UserID
	AND A.[Name] = @Token
	AND F.[AuthKey] = @FunAuthKey

SELECT @IsAuthPassed AS IsAuthPassed";
            return Get<bool>(getApiUserAuthCmd, new { userID, token, funAuthKey });
        }
    }
}
