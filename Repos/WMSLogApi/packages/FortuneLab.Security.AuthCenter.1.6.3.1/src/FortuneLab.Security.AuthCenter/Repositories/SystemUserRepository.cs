using Abp.Application.Services.Dto;
using FortuneLab.ECService.Securities.Entities;
using FortuneLab.Repositories.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Security.AuthCenter.Repositories
{
    public class SystemUserRepository : DapperRepositoryBase<AuthCenterDbConnProvider, long>
    {
        const string selectSql = " t.SysNO as SysId,t.SysNo,t.LoginName,t.DisplayName,t.DepartmentName,t.PhoneNumber,t.Email,t.Password,t.Status,t.InUser,t.InDate,t.EditUser,t.EditDate ";
        public SystemUser GetById(long userId)
        {
            return Get<SystemUser>(string.Format(@"select {0} from SystemUser t where t.SysNO = @userId ", selectSql), new { userId });
        }

        public SystemUser GetUserByLoginId(string loginId)
        {
            return Get<SystemUser>(string.Format("select {0} from SystemUser t where LoginName = @loginName", selectSql), new { loginName = loginId });
        }

        /// <summary>
        /// 根据系统SessionKey获取对应的用户信息
        /// </summary>
        /// <param name="systemSessionKey"></param>
        /// <returns></returns>
        public SystemUser GetUserBySystemSessionKey(string systemSessionKey)
        {
            return Get<SystemUser>(@"select top (1) t.* from SystemUser t where exists (select null from [UserDevice] where DeviceType = 999 and SessionKey = @systemSessionKey and UserSysId = t.SysNo)",
                new { systemSessionKey });
        }
    }

    public class UserDeviceRepository : DapperRepositoryBase<AuthCenterDbConnProvider, Guid>
    {

        public void Insert(UserDevice userDevice)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(@"INSERT INTO [dbo].[UserDevice]
           ([SysId]
           ,[DeviceType]
           ,[SessionKey]
           ,[ActiveTime]
           ,[CreateTime]
           ,[ExpiredTime]
           ,[DeviceId]
           ,[ClientId]
           ,[UserAgent]
           ,[UserSysId])");
            sbSql.AppendFormat(" select @SysId,@DeviceType,@SessionKey,@ActiveTime,@CreateTime,@ExpiredTime,@DeviceId,@ClientId,@UserAgent,@UserSysId ");
            ExecuteNoQuery(sbSql.ToString(), new
            {
                userDevice.SysId,
                userDevice.DeviceType,
                userDevice.SessionKey,
                userDevice.ActiveTime,
                userDevice.CreateTime,
                userDevice.ExpiredTime,
                userDevice.DeviceId,
                userDevice.ClientId,
                userDevice.UserAgent,
                userDevice.UserSysId
            });
        }

        public void Update(UserDevice userDevice)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(@"update [dbo].[UserDevice]
           set [DeviceType]=@DeviceType
           ,[SessionKey]= @SessionKey
           ,[ActiveTime]= @ActiveTime
           ,[CreateTime]= @CreateTime
           ,[ExpiredTime]= @ExpiredTime
           ,[DeviceId]= @DeviceId
           ,[ClientId]= @ClientId
           ,[UserAgent]= @UserAgent
           ,[UserSysId]= @UserSysId where SysId = @SysId");
            ExecuteNoQuery(sbSql.ToString(), new
            {
                userDevice.SysId,
                userDevice.DeviceType,
                userDevice.SessionKey,
                userDevice.ActiveTime,
                userDevice.CreateTime,
                userDevice.ExpiredTime,
                userDevice.DeviceId,
                userDevice.ClientId,
                userDevice.UserAgent,
                userDevice.UserSysId
            });
        }
    }
}
