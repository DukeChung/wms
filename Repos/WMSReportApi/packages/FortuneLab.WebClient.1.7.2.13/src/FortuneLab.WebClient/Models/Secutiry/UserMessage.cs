using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebClient.Models.Secutiry
{
    /// <summary>
    /// 站内用户消息
    /// </summary>
    public class UserMessage
    {
        public Guid SysId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsReaded { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
