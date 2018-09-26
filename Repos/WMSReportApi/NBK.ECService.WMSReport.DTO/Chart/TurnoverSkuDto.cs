using System;

namespace NBK.ECService.WMSReport.DTO.Chart
{
    public class TurnoverSkuDto
    {
        public string SkuName { get; set; }
        public string  UPC { get; set; }
        public decimal InQty1 { get; set; }
        public decimal OutQty1 { get; set; }
        public decimal InQty2 { get; set; }
        public decimal OutQty2 { get; set; }
        public decimal InQty3 { get; set; }
        public decimal OutQty3 { get; set; }
        public decimal InQty4 { get; set; }
        public decimal OutQty4 { get; set; }
        public decimal InQty5 { get; set; }
        public decimal OutQty5 { get; set; }
        public decimal InQty6 { get; set; }
        public decimal OutQty6 { get; set; } 
        public decimal AverageQty { get; set; }
        public decimal InvQty { get; set; }
        public int CountQty { get; set; }

        public string Prompt
        {
            get
            {
                var result = string.Empty;
                if (InvQty > AverageQty * 3)
                {
                    result = "<span style='color:red' >库存充足</span>";
                }
                else if (InvQty > AverageQty*2)
                {
                    result = "<span style='color:red' >安全库存";
                }
                else if (InvQty > AverageQty)
                {
                    result = "<span style='color:red'>库存紧张";
                }
                else if (InvQty == AverageQty)
                {
                    result = "<span style='color:red'>需要补货";
                }
                else if (InvQty < AverageQty)
                {
                    result = "<span style='color:red'>缺货";
                }
                return result;
            }
        }
    }
}