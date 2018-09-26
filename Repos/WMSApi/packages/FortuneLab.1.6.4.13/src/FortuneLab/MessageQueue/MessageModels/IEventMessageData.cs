using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.MessageQueue.MessageModels
{
    public interface IEventMessageData
    {
        Guid MessageId { get; }
        bool SupportRetry { get; set; }
    }
}
