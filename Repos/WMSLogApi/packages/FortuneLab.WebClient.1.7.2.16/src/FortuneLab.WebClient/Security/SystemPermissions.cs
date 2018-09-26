using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebClient.Security
{
    public static class SystemPermissions
    {
        public const string VipApplyAudit = "VipApplyAudit";
        public const string LoanQuotaApplyAudit = "LoanCreditAudit";
        public const string UserAttachmentAudit = "UserAttachmentAudit";

        public const string LoanRecordFirstAudit = "LoanRecordFirstAudit";
        public const string LoanRecordSecondAudit = "LoanRecordSecondAudit";
        public const string LoanEarlyLendingAudit = "LoanEarlyLendingAudit";

        public const string AdminUserManager = "AdminUserManager";
        public const string RoleManager = "RoleManager";
        public const string RolePermissionManager = "RolePermissionManager";
        public const string LoanTypeManager = "LoanTypeManager";
        public const string SysLogManager = "SysLogManager";

        public const string UserWithdrawAudit = "UserWithdrawAudit";
    }
}
