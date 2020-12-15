using System;
using System.Collections.Generic;
using System.Text;

namespace Minor.Miffy.InMemoryBus
{
    /// <summary>
    /// Each InMemoryContext object creates its own message broker system.
    /// </summary>
    public class InMemoryContext : IBusContext
    {
        public MessageBroker Connection { get; }
        public string ExchangeName { get; }

        public InMemoryContext()
        {
            Connection = new MessageBroker();
            ExchangeName = "In-memory message bus";
        }

        public IMessageSender CreateMessageSender()
        {
            return new InMemoryMessageSender(this);
        }

        public IMessageReceiver CreateMessageReceiver(string queueName, IEnumerable<string> topicExpressions)
        {
            return new InMemoryMessageReceiver(this, queueName, topicExpressions);
        }

        public void Dispose()
        {
        }
    }
}
