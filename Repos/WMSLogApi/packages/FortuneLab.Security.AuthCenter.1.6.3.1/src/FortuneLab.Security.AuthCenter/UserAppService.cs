using Abp.Auditing;
using FortuneLab.ECService.Securities;
using FortuneLab.ECService.Securities.Entities;
using FortuneLab.ECService.Securities.Models;
using FortuneLab.Runtime;
using FortuneLab.Security.AuthCenter.Repositories;
using System;

namespace FortuneLab.Security.AuthCenter
{
    public class UserAppService : IUserAppService
    {
        private AuthCenterRepository _authCenterRepository;
        private SystemUserRepository _systemUserRepository;
        private UserDeviceRepository _userDeviceRepository;
        private SystemApplicationRepository _systemApplicationRepository;
        public UserAppService(AuthCenterRepository userRepository, SystemUserRepository systemUserRepository,
            UserDeviceRepository userDeviceRepository, SystemApplicationRepository systemApplicationRepository)
        {
            this._authCenterRepository = userRepository;
            this._systemUserRepository = systemUserRepository;
            this._userDeviceRepository = userDeviceRepository;
            this._systemApplicationRepository = systemApplicationRepository;
        }

        [DisableAuditing]
        public ApiToken GetToken(string token)
        {
            SystemApplication systemApplication = this._systemApplicationRepository.GetSystemApplication(token);

            return systemApplication == null ? null : new ApiToken() { Token = systemApplication.Name, ApplicationId = systemApplication.ApplicationID };
        }

        [DisableAuditing]
        public SystemUser Login(string loginId, string password)
        {
            return this._authCenterRepository.GetByLoginNameAndPWD(loginId, password);
        }

        [DisableAuditing]
        public SystemUser GetUser(int userId)
        {
            return _systemUserRepository.GetById(userId);
        }

        public UserDevice GetUserDevice(string sessionKey)
        {
            return this._authCenterRepository.GetUserDevice(sessionKey);
        }

        public SystemUser GetUserByLoginId(string loginId)
        {
            return this._systemUserRepository.GetUserByLoginId(loginId);
        }

        //public Page<SystemUser> GetSystemUserList(string keyword, int pageIndex, int pageSize)
        //{
        //    return this._systemUserRepository.GetSystemUserList(keyword, pageIndex, pageSize);
        //}

        public UserDevice GetUserDevice(int userSysId, int deviceType)
        {
            return _authCenterRepository.GetUserDevice(userSysId, deviceType);
        }

        public void AddUserDevice(UserDevice userDevice)
        {
            userDevice.SysId = Guid.NewGuid();
            _userDeviceRepository.Insert(userDevice);
        }

        public void UpdateUserDevice(UserDevice userDevice)
        {
            _userDeviceRepository.Update(userDevice);
        }

        public bool CheckUserAuthorization(string token, string userID, string funAuthKey)
        {
            return _authCenterRepository.GetApiUserAuth(token, userID, funAuthKey);
        }

        public SystemUser GetUserBySystemSession(string systemSessionKey)
        {
            return _systemUserRepository.GetUserBySystemSessionKey(systemSessionKey);
        }
    }
}
