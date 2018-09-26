using System;

namespace Abp.Domain.Entities.Auditing
{
    /// <summary>
    /// A shortcut of <see cref="AuditedSysIdEntity{TPrimaryKey}"/> for most used primary key type (<see cref="Guid"/>).
    /// </summary>
    [Serializable]
    public abstract class AuditedSysIdEntity : CreationSysIdEntity, IAudited
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected AuditedSysIdEntity()
        {
            CreationTime = DateTime.Now;
        }

        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public virtual long? LastModifierUserId { get; set; }
    }

    [Serializable]
    public abstract class AuditedSysIdEntity<TPrimaryKey> : CreationSysIdEntity<TPrimaryKey>, IAudited, IModificationAudited
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected AuditedSysIdEntity()
        {
            CreationTime = DateTime.Now;
        }

        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public virtual long? LastModifierUserId { get; set; }
    }
}
