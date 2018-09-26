using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class OutStockDto
    {
        public OutStockDto()
        {
            OutStockOrderDetailList = new List<OutStockOrderDetailDto>();
        } 

        /// <summary>
        /// 出库单编号
        /// </summary>
        public int? OutStockOrderID { get; set; } 
  
        /// <summary>
        /// 快递公司名称
        /// </summary>
        public string ExpressName { get; set; }

        /// <summary>
        /// 快递公司编码
        /// </summary>
        public string ExpressCode { get; set; } 

        /// <summary>
        /// 快递单号
        /// </summary>
        public string ExpressNumber { get; set; }

        /// <summary>
        /// 发货人名称
        /// </summary>
        public string DeliveryUserName { get; set; }

        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime? DeliveryDate { get; set; } 
         

        /// <summary>
        /// 出库单编辑人ID
        /// </summary>
        public int EditUserID { get; set; }

        /// <summary>
        /// 出库单编辑人名称
        /// </summary>
        public string EditUserName { get; set; }

        /// <summary>
        /// 入库单编辑日期
        /// </summary>
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 出库单子单集合
        /// </summary>
        public List<OutStockOrderDetailDto> OutStockOrderDetailList { get; set; }
    }
}