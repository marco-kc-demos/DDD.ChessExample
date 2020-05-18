using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minor.Miffy.InMemoryBus
{
    public class MessageQueue
    {
        public string QueueName { get; }
        public IEnumerable<string> TopicFilters { get; }
        public IEnumerable<EventMessage> QueuedMessages => _messageQueue;
        public bool HasConsumer => _consumer != null;

        private Queue<EventMessage> _messageQueue;
        private EventMessageReceivedCallback? _consumer;

        public MessageQueue(string queueName, IEnumerable<string> topicFilters)
        {
            QueueName = queueName;
            TopicFilters = topicFilters;
            _messageQueue = new Queue<EventMessage>();
            _consumer = null;
        }

        public Task PublishAsync(EventMessage message)
        {
            return Task.Run(() =>
            {
                if (_consumer == null)
                {
                    lock (_messageQueue)
                    {
                        if (_consumer == null)
                        {
                            _messageQueue.Enqueue(message);
                            return;
                        }
                    }
                }

                _consumer.Invoke(message);
            });
        }

        public void SetConsumer(EventMessageReceivedCallback callback)
        {
            lock (_messageQueue)
            {
                if (_consumer == null)
                {
                    _consumer = callback;

                    while (_messageQueue.Any())
                    {
                        EventMessage message = _messageQueue.Dequeue();
                        _consumer.Invoke(message);
                    }
                    return;
                }
                else
                {
                    throw new InvalidOperationException("Cannot subscribe a second listener to the same queue.");
                }
            }
        }

        public void RemoveConsumer(EventMessageReceivedCallback callback)
        {
            lock (_messageQueue)
            {
                if (_consumer != null)
                {
                    _consumer = null;
                }
            }
        }
    }
}