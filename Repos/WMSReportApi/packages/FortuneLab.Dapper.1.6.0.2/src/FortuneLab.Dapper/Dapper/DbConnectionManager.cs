using Abp.Dependency;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FortuneLab.Repositories.Dapper
{
    public class DbConnectionManager
    {
        private ILogger _logger = LogManager.GetLogger("FortuneLab.DbConnectionManager");
        public static DbConnectionManager Instance = new DbConnectionManager();
        DateTime lastGCTime = DateTime.Now;
        LocalDataStoreSlot dbConnSlot = System.Threading.Thread.AllocateNamedDataSlot("DapperDbConn");

        static DbConnectionManager()
        {

        }

        private DbConnectionManager()
        {

        }

        public void SafeCloseConnection(IDbConnection conn)
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
                var dbConnSession = Thread.GetData(dbConnSlot) as DbConnSession;
                if (dbConnSession != null)
                {
                    dbConnSession.Watch.Stop();
                    _logger.Trace("Release DbConn {0}, Total Used: {1}ms", dbConnSession.Id, dbConnSession.Watch.ElapsedMilliseconds);
                }

                if ((DateTime.Now - lastGCTime).TotalMinutes > 1)
                {
                    lastGCTime = DateTime.Now;
                    System.GC.Collect();
                }
            }
        }

        public void SafeOpenConnection(IDbConnection conn)
        {
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
        }

        public IDbConnection GetConnection(string nameOrConnectionString)
        {
            var dbConnSession = new DbConnSession();
            Thread.SetData(dbConnSlot, dbConnSession);
            _logger.Trace("Start DbConn {0}", dbConnSession.Id);
            return new SqlConnection(GetDbConnectionString(nameOrConnectionString));
        }

        private static readonly ConcurrentDictionary<string, string> ConnCache = new ConcurrentDictionary<string, string>();
        private System.Diagnostics.Stopwatch _stopWatcher;

        private string GetDbConnectionString(string connName)
        {
            string connString;
            if (ConnCache.TryGetValue(connName, out connString))
                return connString;

            return ConnCache.TryAdd(connName, ConfigurationManager.ConnectionStrings[connName].ConnectionString) 
                ? ConnCache[connName] 
                : ConfigurationManager.ConnectionStrings[connName].ConnectionString;
        }

        public class DbConnSession
        {
            public DbConnSession()
            {
                Id = Guid.NewGuid();
                Watch = Stopwatch.StartNew();
            }
            public Guid Id { get; set; }
            public Stopwatch Watch { get; set; }
        }
    }
}
