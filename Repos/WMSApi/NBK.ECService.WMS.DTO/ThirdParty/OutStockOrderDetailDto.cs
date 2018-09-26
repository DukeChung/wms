using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
  
    public class OutStockOrderDetailDto
    {
        public OutStockOrderDetailDto()
        {
            OutStockOrderItemDetailList = new List<OutStockOrderItemDetailDto>();
        }
         
        /// <summary>
        /// 出库单子单来源类型
        /// </summary>
        public int SourceType { get; set; }
        
        /// <summary>
        /// 出库单子单来源编号
        /// </summary>
        public string SourceNumber { get; set; }
        public List<OutStockOrderItemDetailDto> OutStockOrderItemDetailList { get; set; }
    }
}