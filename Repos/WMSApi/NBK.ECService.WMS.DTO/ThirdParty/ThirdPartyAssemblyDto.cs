using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyAssemblyDto : BaseDto
    {
        /// <summary>
        /// 外部单号
        /// </summary>
        public string ExternalOrder { get; set; }

        public string OtherSkuId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 计划加工日期
        /// </summary>
        public DateTime? PlanProcessingDate { get; set; }

        /// <summary>
        /// 计划完工日期
        /// </summary>
        public DateTime? PlanCompletionDate { get; set; }

        /// <summary>
        /// 计划数量
        /// </summary>
        public int PlanQty { get; set; }

        /// <summary>
        /// 包装方式
        /// </summary>
        public string Packing { get; set; }

        /// <summary>
        /// 包装重量
        /// </summary>
        public string PackWeight { get; set; }

        /// <summary>
        /// 包装等级
        /// </summary>
        public string PackGrade { get; set; }

        /// <summary>
        /// 存储条件
        /// </summary>
        public string StorageConditions { get; set; }

        /// <summary>
        /// 包装规格
        /// </summary>
        public string PackSpecification { get; set; }

        /// <summary>
        /// 包装方式描述
        /// </summary>
        public string PackDescr { get; set; }

        /// <summary>
        /// 仓库Id(对应Warehouse表OtherId)
        /// </summary>
        public string WarehouseId { get; set; }


        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNumber { get; set; }

        public List<ThirdPartyAssemblyDetailDto> ThirdPartyAssemblyDetailDtoList { get; set; }
    }
}
