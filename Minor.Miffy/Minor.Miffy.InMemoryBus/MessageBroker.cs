using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minor.Miffy.InMemoryBus
{
    public class MessageBroker
    {
        private readonly ConcurrentDictionary<string, MessageQueue> _queues;
        private readonly List<EventMessage> _loggedMessages;

        public IEnumerable<EventMessage> LoggedMessages => _loggedMessages;

        public MessageBroker()
        {
            _queues = new ConcurrentDictionary<string, MessageQueue>();
            _loggedMessages = new List<EventMessage>();
        }

        public MessageQueue GetNamedMessageQueue(string queueName)
        {
            return _queues[queueName];
        }

        public async Task BasicPublishAsync(EventMessage message)
        {
            _loggedMessages.Add(message);

            await from queue in _queues.Values
                  where queue.TopicFilters.ThatMatch(message.Topic).Any()
                  select queue.PublishAsync(message);
        }

        public void QueueDeclare(string queueName, IEnumerable<string> topicFilters)
        {
            MessageQueue queue = new MessageQueue(queueName, topicFilters);
            if (!_queues.TryAdd(queueName, queue))
            {
                if (!AreEquivalent(topicFilters, _queues[queueName].TopicFilters))
                {
                    throw new BusConfigurationException("Cannot declare the same queue with a different set of topic filters");
                }
            }
        }

        public void BasicComsume(string queueName, EventMessageReceivedCallback callback)
        {
            try
            {
                _queues[queueName].SetConsumer(callback);

            }
            catch (KeyNotFoundException)
            {
                throw new BusConfigurationException($"Queue '{queueName}' has not been declared.");
            }        
        }

        public void RemoveComsumer(string queueName, EventMessageReceivedCallback callback)
        {
            if (_queues.ContainsKey(queueName))
            {
                _queues[queueName].RemoveConsumer(callback);
            }
        }


        private static bool AreEquivalent<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            return !first.Except(second).Any() &&
                   !second.Except(first).Any();
        }
    }
}