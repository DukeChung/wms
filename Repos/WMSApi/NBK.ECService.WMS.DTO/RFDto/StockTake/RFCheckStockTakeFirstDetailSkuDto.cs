using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFCheckStockTakeFirstDetailSkuDto
    {
        public RFCheckStockTakeFirstDetailSkuDto()
        {
            Skus = new List<RFStockTakeFirstDetailDto>();
        }

        public RFCommResult RFCommResult
        {
            get
            {
                if (Skus != null && Skus.Count > 0)
                {
                    return new RFCommResult { IsSucess = true };
                }
                else
                {
                    return new RFCommResult { IsSucess = false, Message = "商品不存在" };
                }
            }
        }

        public List<RFStockTakeFirstDetailDto> Skus { get; set; }
    }
}
