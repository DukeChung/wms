using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Domain.Entities
{
    /// <summary>
    /// A shortcut of <see cref="Entity{TPrimaryKey}"/> for most used primary key type (<see cref="Guid"/>).
    /// </summary>
    [Serializable]
    public abstract class SysIdEntity : SysIdEntity<Guid>, ISysIdEntity
    {
        public SysIdEntity()
        {
            SysId = Guid.NewGuid();
        }
    }
}
