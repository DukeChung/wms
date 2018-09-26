using Abp.Dependency;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Abp.EntityFramework
{
    public abstract class FortuneLabDbContext : AbpDbContext
    {
        static ILogger _logger = LogManager.GetLogger("FortuneLab.SQL.FortuneLabDbContext");

        public FortuneLabDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            if (Database.Log == null)
            {
                Database.Log = logMessage => { _logger.Debug(logMessage); };
            }
        }

        static FortuneLabDbContext()
        {
            Database.SetInitializer<FortuneLabDbContext>(null);
        }

        protected override void ApplyAbpConcepts()
        {
            base.ApplyAbpConcepts();
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case System.Data.Entity.EntityState.Added:
                        //在添加的同时更新LastMondiferUserId,LastMondificationTime
                        SetModificationAuditProperties(entry);
                        break;
                }
            }
        }
    }
}
