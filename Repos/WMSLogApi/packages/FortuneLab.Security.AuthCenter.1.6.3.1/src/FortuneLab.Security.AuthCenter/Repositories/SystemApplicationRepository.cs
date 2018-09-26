using FortuneLab.ECService.Securities.Models;
using FortuneLab.Repositories.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Security.AuthCenter.Repositories
{
    public class SystemApplicationRepository : DapperRepositoryBase<AuthCenterDbConnProvider, long>
    {
        const string SelectSql = "  t.SysNO as SysId, t.SysNO, t.Name, t.Description, t.ApplicationID, t.Status, t.InUser, t.InDate, t.EditUser, t.EditDate ";

        public SystemApplication GetSystemApplication(string token)
        {
            return Get<SystemApplication>(string.Format("select {0} from SystemApplication t where Name = @name", SelectSql), new { name = token });
        }
    }
}
