using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.ECService.Securities.Entities
{
    public class SessionObject : IDto
    {
        public string SessionKey { get; set; }
        public SystemUser LogonUser { get; set; }
    }

    public class SystemUser : SystemUserBase
    {
        public int CompanySysNO { get; set; }

        public string CompanyName { get; set; }

        /// <summary>
        /// Only in TrustedLogin
        /// </summary>
        public string CompanyCode { get; set; }
    }

    public class SystemUserBase : SysIdEntity<int>, IFortuneLabUser<int>
    {
        public SystemUserBase()
        {
            this.SysNO = 0;
            this.LoginName = string.Empty;
            this.DisplayName = string.Empty;
            this.DepartmentName = string.Empty;
            this.PhoneNumber = string.Empty;
            this.Email = string.Empty;
            this.Password = string.Empty;
            this.Status = UserStatus.Enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        public int SysNO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserStatus Status { get; set; }

        public int UserId { get { return SysNO; } }
    }

    public enum UserStatus
    {
        Disabled = 0,
        Enabled = 1
    }
}
