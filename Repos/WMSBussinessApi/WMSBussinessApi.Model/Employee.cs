using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMSBussinessApi.Model
{
    public class Employee
    {
        public Guid SysId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePhone { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeIdCardNo { get; set; }
        public string SourceOrganizationId { get; set; }
        public int STATUS { get; set; }
        public string SourceID { get; set; }
        public string SourceNumber { get; set; }
        public int CreateUserID { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public int EditUserID { get; set; }
        public string EditUserName { get; set; }
        public DateTime EditDate { get; set; }
    }
}
