using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility
{
    public class CommonBussinessMethod
    {
        /// <summary>
        /// 根据各种库存明细推算可用库存
        /// </summary>
        /// <param name="qty">财务库存</param>
        /// <param name="allocatedQty">分配库存</param>
        /// <param name="pickedQty">拣货库存</param>
        /// <param name="frozenQty">冻结库存</param>
        /// <returns></returns>
        public static int GetAvailableQty(int qty, int allocatedQty, int pickedQty, int frozenQty)
        {
            return (qty - allocatedQty - pickedQty - frozenQty);
        }

        /// <summary>
        /// 比较两个字符串的内容是否相等，其中 空字符串 和 null 视为相等
        /// </summary>
        /// <param name="compareA"></param>
        /// <param name="compareB"></param>
        /// <returns></returns>
        public static bool CompareStringDiffaultIgnoreNull(string compareA,string compareB)
        {
            if (string.IsNullOrEmpty(compareA) && string.IsNullOrEmpty(compareB))
            {
                return true;
            }
            else if (compareA != null && compareB != null)
            {
                return compareA.Equals(compareB, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
