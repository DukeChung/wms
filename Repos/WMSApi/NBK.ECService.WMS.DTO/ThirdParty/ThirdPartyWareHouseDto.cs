using System;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyWareHouseDto
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contacts { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone { get; set; }  
        /// <summary>
        /// 外部ID
        /// </summary>
        public string OtherId { get; set; }
    }
}