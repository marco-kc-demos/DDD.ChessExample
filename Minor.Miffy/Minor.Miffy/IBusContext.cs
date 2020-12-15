using System;
using System.Collections.Generic;
using System.Text;

namespace Minor.Miffy
{
    public interface IBusContext : IDisposable
    {
        string ExchangeName { get; }

        IMessageSender CreateMessageSender();
        IMessageReceiver CreateMessageReceiver(string queueName, IEnumerable<string> topicFilters);
    }
}
