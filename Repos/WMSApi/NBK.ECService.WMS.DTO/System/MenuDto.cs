using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.System
{
    public class MenuDto
    {
        public Guid SysId { get; set; }
        public string MenuName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string ICons { get; set; }
        public Guid? ParentSysId { get; set; }
        public bool IsActive { get; set; }

        public int SortSequence { get; set; }

        /// <summary>
        /// 用于将主菜单与子菜单分组
        /// </summary>
        public string GroupMenuController { get; set; }

        public string AuthKey { get; set; }
    }
}
