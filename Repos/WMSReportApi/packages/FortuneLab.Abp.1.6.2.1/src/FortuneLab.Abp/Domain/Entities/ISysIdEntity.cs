using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Domain.Entities
{
    public interface ISysIdEntity<TPrimaryKey>
    {
        // 摘要: 
        //     Unique identifier for this entity.
        TPrimaryKey SysId { get; set; }

        // 摘要: 
        //     Checks if this entity is transient (not persisted to database and it has
        //     not an Abp.Domain.Entities.ISysIdEntity<TPrimaryKey>.SysId).
        //
        // 返回结果: 
        //     True, if this entity is transient
        bool IsTransient();
    }

    public interface ISysIdEntity:ISysIdEntity<Guid>
    {

    }
}
