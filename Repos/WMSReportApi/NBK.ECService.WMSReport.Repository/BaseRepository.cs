using Abp.EntityFramework;
using NBK.ECService.WMSReport.Model;
using NBK.ECService.WMSReport.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Chart;
using NBK.ECService.WMSReport.Model.Models;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;

namespace NBK.ECService.WMSReport.Repository
{
    public class BaseRepository : CrudRepository, IBaseRepository
    {
        public BaseRepository(IDbContextProvider<NBK_WMS_ReportContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// 获取系统菜单按钮
        /// </summary>
        /// <returns></returns>
        public List<MenuDto> GetSystemMenuList()
        {
            var sql = new StringBuilder();

            sql.Append(@" SELECT SysId, MenuName, Action, Controller, ICons, ParentSysId, IsActive,
                          SortSequence, GroupMenuController, AuthKey FROM globalmenu g
                          WHERE g.IsActive=1;");

            return base.Context.Database.SqlQuery<MenuDto>(sql.ToString()).ToList();
        }
    }
}
