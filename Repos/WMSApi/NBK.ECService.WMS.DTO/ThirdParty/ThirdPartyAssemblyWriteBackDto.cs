using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyAssemblyWriteBackDto
    {
        /// <summary>
        /// 获取或设置生产加工单的单号，业务 id。
        /// </summary>
        public int RmpOrderId { get; set; }

        /// <summary>
        /// 获取或设置实际加工时间。
        /// </summary>
        public DateTime ActualToProcessTime { get; set; }

        /// <summary>
        ///获取或设置实际加工数量。
        /// </summary>
        public int ActualToCompletedQuantity { get; set; }

        /// <summary>
        /// 获取或设置当前用户唯一标识符 id。业务主键。
        /// </summary>
        public int CurrentUserId { get; set; }

        /// <summary>
        /// 获取或设置当前用户名称。
        /// </summary>
        public string CurrentUserName { get; set; }

        /// <summary>
        ///获取或设置实际完工时间。
        /// </summary>
        public DateTime ActualToCompletedTime { get; set; }

        /// <summary>
        /// 获取或设置生产加工单原材料列表。
        /// </summary>
        public List<ThirdPartyAssemblyDetailWriteBackDto> RawMaterialsDetail { get; set; }
    }

    public class ThirdPartyAssemblyDetailWriteBackDto
    {
        /// <summary>
        /// 获取或设置原材料商品编号。
        /// </summary>
        public int ProductCode { get; set; }

        /// <summary>
        /// 获取或设置生产加工过程中损耗的质量，单位：克。
        /// </summary>
        public int LossQuantity { get; set; }
    }

    public class ThirdPartyWMSAssemblyWriteBackDto
    {
        public string WarehouseID { get; set; }

        public string Channel { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUserName { get; set; }

        public string SourceNumber { get; set; }

        public string ProductCode { get; set; }

        public int IncreaseQty { get; set; }

        public List<ThirdPartyWMSAssemblyDetailWriteBackDto> MachiningSingleDetailList { get; set; }
    }

    public class ThirdPartyWMSAssemblyDetailWriteBackDto
    {
        public string ProductCode { get; set; }

        public int DeductionQty { get; set; }
    }
}
