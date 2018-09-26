using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;
using BachLib.Utility;
using WMSBussinessApi.Repository;
using WMSBussinessApi.Dto.BaseData;
using WMSBussinessApi.Utility;
using WMSBussinessApi.Utility.Redis;

namespace WMSBussinessApi.RepositoryImp
{
    public class BaseRep : IBaseRep
    {
        private string DBConnection { get; set; }

        protected IDbConnection Connection
        {
            get
            {
                if (string.IsNullOrEmpty(DBConnection))
                {
                    //Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(this.Configuration, "DefaultConnection");
                    //Config.AppSetting.Get<int>("Schubert:Data:ConnectionStrings:default");
                    DBConnection = Config.AppSetting.Get("Schubert:Data:ConnectionStrings:default");
                    //     return new MySqlConnection(Configuration);
                }
                return new MySqlConnection(DBConnection);
            }
        }

        public void SetConnection(string connectionString)
        {
            this.DBConnection = connectionString;
        }


        public List<WarehouseDto> GetAllWarehouseInfo()
        {
            var wareHouseList = RedisWMS.GetRedisList<List<WarehouseDto>>(RedisSourceKey.RedisWareHouseList);
            if (wareHouseList == null || wareHouseList.Count == 0)
            {
                string sql = @"
                    SELECT 
                      w.SysId,
                      w.Name,
                      w.ConnectionString,
                      w.ConnectionStringRead
                    FROM warehouse w;
                    ";
                var result = this.Connection.Query<WarehouseDto>(sql);

                return result.ToList();
            }

            return wareHouseList;
        }
    }
}
