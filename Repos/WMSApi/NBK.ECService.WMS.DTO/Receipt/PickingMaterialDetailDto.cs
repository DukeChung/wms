﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PickingMaterialDetailDto
    {
        public Guid SkuSysId { get; set; }

        public int Qty { get; set; }

        public decimal InputQty { get; set; }
    }
}