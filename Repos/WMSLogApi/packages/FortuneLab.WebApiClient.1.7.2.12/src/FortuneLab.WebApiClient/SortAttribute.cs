﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebApiClient
{
    public class SortAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
