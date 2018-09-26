using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class CharDataDto
    {
        public string[] labels { get; set; }

        public DataSet[] datasets { get; set; }
    }

    public class DataSet
    {
        public string label { get; set; }

        public string backgroundColor { get; set; }

        public string borderColor { get; set; }

        public string pointBackgroundColor { get; set; }

        public string pointBorderColor { get; set; }

        public int[] data { get; set; }
    }
}
