using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    /// <summary>
    /// ERP
    /// </summary>

    public class InStockDto
    {
        public InStockDto()
        {
            InStockOrderDetailList = new List<InStockOrderDetailDto>();
        }

        /// <summary>
        /// 入库单业务编号
        /// </summary>
        public int? InStockOrderID { get; set; }

        /// <summary>
        /// 入库单状态 2 部分入库 3全部入库
        /// </summary>
        public int? Status { get; set; }  
        
        /// <summary>
        /// 入库单编辑人ID
        /// </summary>
        public int EditUserID { get; set; }

        /// <summary>
        /// 入库单编辑人名称
        /// </summary>
        public string EditUserName { get; set; }
        /// <summary>
        /// 入库单编辑日期
        /// </summary>
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 入库单子单集合主信息
        /// </summary>
        public List<InStockOrderDetailDto> InStockOrderDetailList { get; set; }
    }
     
}