﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFInvSkuLocQuery : BaseQuery
    {
        public string SkuUPC { get; set; }

        public string Loc { get; set; }
    }
}
