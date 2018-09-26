using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.ECService.Securities.Models
{
    public class SystemApplication : SysIdEntity<long>
    {
        public SystemApplication()
        {
            this.SysNO = 0;
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.ApplicationID = Guid.Empty;
            this.Status = ApplicationStatus.Enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        public int SysNO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Guid ApplicationID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationStatus Status { get; set; }
    }

    public enum ApplicationStatus
    {
        Disabled = 0,
        Enabled = 1
    }
}
