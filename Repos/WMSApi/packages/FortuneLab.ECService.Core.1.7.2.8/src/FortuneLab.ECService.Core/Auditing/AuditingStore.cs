using Abp.Auditing;
using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.ECService.Auditing
{
    /// <summary>
    /// Implements <see cref="IAuditingStore"/> to save auditing informations to database.
    /// </summary>
    //public class AuditingStore : IAuditingStore, ITransientDependency
    //{
    //    private readonly AuditingRepository _auditLogRepository;

    //    /// <summary>
    //    /// Creates  a new <see cref="AuditingStore"/>.
    //    /// </summary>
    //    public AuditingStore(AuditingRepository auditLogRepository)
    //    {
    //        _auditLogRepository = auditLogRepository;
    //    }

    //    public Task SaveAsync(AuditInfo auditInfo)
    //    {
    //        return Task.FromResult(_auditLogRepository.InsertAsync(AuditLog.CreateFromAuditInfo(auditInfo)));
    //    }
    //}
}
