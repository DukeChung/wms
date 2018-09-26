using Abp.Timing;
using System;

namespace Abp.Domain.Entities.Auditing
{
    public class CreationSysIdEntity : SysIdEntity, ICreationAudited
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected CreationSysIdEntity()
        {
            CreationTime = Clock.Now;
        }

        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator of this entity.
        /// </summary>
        public virtual long? CreatorUserId { get; set; }
    }

    public class CreationSysIdEntity<TPrimaryKey> : SysIdEntity<TPrimaryKey>, ICreationAudited
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected CreationSysIdEntity()
        {
            CreationTime = Clock.Now;
        }

        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator of this entity.
        /// </summary>
        public virtual long? CreatorUserId { get; set; }
    }
}
