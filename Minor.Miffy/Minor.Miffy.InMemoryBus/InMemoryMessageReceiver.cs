using System;
using System.Collections.Generic;

namespace Minor.Miffy.InMemoryBus
{
    public class InMemoryMessageReceiver : IMessageReceiver
    {
        private readonly MessageBroker _messageBroker;
        public string QueueName { get; }
        public IEnumerable<string> TopicFilters { get; }
        private bool _hasStartedReceivingMessages;
        private bool _hasStartHandlingMessages;
        private bool _isDisposed;
        private EventMessageReceivedCallback _callback;

        public InMemoryMessageReceiver(InMemoryContext inMemoryContext, string queueName, IEnumerable<string> topicExpressions)
        {
            _messageBroker = inMemoryContext.Connection;
            QueueName = queueName;
            TopicFilters = topicExpressions;

            _hasStartedReceivingMessages = false;
            _hasStartHandlingMessages = false;
            _isDisposed = false;
            _callback = null;
        }

        public void StartReceivingMessages()
        {
            CanStartReceivingMessages();
            _hasStartedReceivingMessages = true;

            _messageBroker.QueueDeclare(QueueName, TopicFilters);
        }

        public void StartHandlingMessages(EventMessageReceivedCallback callback)
        {
            CanStartHandlingMessages();
            _hasStartHandlingMessages = true;

            _callback = callback;
            _messageBroker.BasicComsume(QueueName, callback);
        }

        public void Dispose()
        {
            _isDisposed = true;
            _messageBroker.RemoveComsumer(QueueName, _callback);
        }

        private void CanStartReceivingMessages()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(InMemoryMessageReceiver));
            }
            else if (_hasStartedReceivingMessages)
            {
                throw new BusException("Cannot call 'StartReceivingMessages' multiple times.");
            }
        }

        private void CanStartHandlingMessages()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(InMemoryMessageReceiver));
            }
            else if (!_hasStartedReceivingMessages)
            {
                throw new BusException("Before calling 'StartHandlingMessages', call 'StartReceivingMessages' first to declare queue and topics.");
            }
            else if (_hasStartHandlingMessages)
            {
                throw new BusException("Cannot call 'StartHandlingMessages' multiple times.");
            }
        }

    }
}