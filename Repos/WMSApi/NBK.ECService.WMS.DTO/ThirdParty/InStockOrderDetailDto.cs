using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
 
    public class InStockOrderDetailDto
    {
        public InStockOrderDetailDto()
        {
            InStockOrderItemDetailList = new List<InStockOrderItemDetailDto>();
        }

        /// <summary>
        /// 子单类型
        /// </summary>
        public int SourceType { get; set; }

        /// <summary>
        /// 子单来源单号ID
        /// </summary>
        public string SourceNumber { get; set; }


        /// <summary>
        /// 入库单子单对应的商品明细集合
        /// </summary>
        public List<InStockOrderItemDetailDto> InStockOrderItemDetailList { get; set; }
    }
}