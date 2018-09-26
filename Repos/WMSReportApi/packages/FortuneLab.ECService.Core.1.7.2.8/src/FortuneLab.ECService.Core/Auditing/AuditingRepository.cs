using Abp.Dependency;
using FortuneLab.Repositories;
using FortuneLab.Repositories.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.ECService.Auditing
{
    public class LoggingDbConProvider : IDbConnProvider
    {
        public string NameOrConnectionString
        {
            get { return "Logging"; }
        }
    }

    public class AuditingRepository : DapperRepositoryBase<LoggingDbConProvider, Guid>, ITransientDependency
    {
        public async Task<int> InsertAsync(AuditLog auditLog)
        {
            //使用独立的事务提交审核日志
            using (var ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Suppress))
            {
                var sql = @"INSERT INTO [dbo].[AuditLogs] ([SysId] ,[UserId] ,[ServiceName] ,[MethodName] ,[Parameters] ,[ExecutionTime] ,[ExecutionDuration] ,[ClientIpAddress] ,[ClientName] ,[BrowserInfo] ,[Exception])
     VALUES (NEWID() ,@UserId ,@ServiceName ,@MethodName ,@Parameters ,@ExecutionTime ,@ExecutionDuration ,@ClientIpAddress ,@ClientName ,@BrowserInfo ,@Exception)";
                return await ExecuteNoQueryAsync(sql, new { auditLog.SysId, auditLog.UserId, auditLog.ServiceName, auditLog.MethodName, auditLog.Parameters, auditLog.ExecutionTime, auditLog.ExecutionDuration, auditLog.ClientIpAddress, auditLog.ClientName, auditLog.BrowserInfo, auditLog.Exception });
            }
        }
    }
}

/*
 CREATE TABLE [dbo].[AuditLogs](
	[SysId] [uniqueidentifier] NOT NULL,
	[UserId] [bigint] NULL,
	[ServiceName] [nvarchar](256) NULL,
	[MethodName] [nvarchar](256) NULL,
	[Parameters] [nvarchar](1024) NULL,
	[ExecutionTime] [datetime] NOT NULL,
	[ExecutionDuration] [int] NOT NULL,
	[ClientIpAddress] [nvarchar](64) NULL,
	[ClientName] [nvarchar](128) NULL,
	[BrowserInfo] [nvarchar](256) NULL,
	[Exception] [nvarchar](2000) NULL,
 CONSTRAINT [PK_dbo.AuditLogs] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
 */
