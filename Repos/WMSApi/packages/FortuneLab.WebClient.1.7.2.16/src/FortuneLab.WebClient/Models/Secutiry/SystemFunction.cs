using System;
using System.Collections.Generic;

namespace FortuneLab.WebClient.Models
{
    /// <summary>
    /// 权限定义
    /// </summary>
    public class SystemFunction
    {
        public int SysNo { get; set; }
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AuthKey { get; set; }
    }
}
