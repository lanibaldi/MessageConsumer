using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;

namespace Plugins
{
    public interface IConsumingPlugin
    {
        void Consume(Message msg);
    }
}
