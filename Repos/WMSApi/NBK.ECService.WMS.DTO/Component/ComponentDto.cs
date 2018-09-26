﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ComponentDto : BaseDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string SkuDescr { get; set; }

        /// <summary>
        /// 加工耗时
        /// </summary>
        public decimal? TimeConsuming { get; set; }

        /// <summary>
        /// 组装件明细
        /// </summary>
        public List<ComponentDetailDto> ComponentDetailDtoList { get; set; }
    }
}
