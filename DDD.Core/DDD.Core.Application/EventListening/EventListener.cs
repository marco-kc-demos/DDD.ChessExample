using Microsoft.Extensions.DependencyInjection;
using Minor.Miffy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DDD.Core.Application
{
    public class EventListener<TConnection> : IEventListener
    {
        private readonly IBusContext<TConnection> _busContext;
        private Dictionary<string, IEventDispatcher> _dispatchers;
        private IMessageReceiver _receiver;
        private bool _hasStarted;
        private bool _isDisposed;

        public EventListener(IBusContext<TConnection> busContext, 
               Dictionary<string, IEventDispatcher> dispatchers)
        {
            _busContext = busContext;
            _dispatchers = dispatchers;
            _hasStarted = false;
            _isDisposed = false;
        }

        public void StartListening(string queueName)
        {
            CanBeStartedCheck();

            IEnumerable<string> topicFilters = _dispatchers.Keys;
            _receiver = _busContext.CreateMessageReceiver(queueName, topicFilters);
            _receiver.StartReceivingMessages();
            _receiver.StartHandlingMessages(EventReveived);
        }

        private void EventReveived(EventMessage message)
        {
            foreach (var topicFilter in _dispatchers.Keys)
            {
                if (TopicFilterMatcher.IsMatch(topicFilter, message.Topic))
                {
                    _dispatchers[topicFilter].Dispatch(message);
                }
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            _receiver?.Dispose();
        }


        private void CanBeStartedCheck()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(EventListener<TConnection>));
            }
            else if (_hasStarted)
            {
                throw new BusException("An EventListener cannot Start Listening twice.");
            }
            else
            {
                _hasStarted = true;
            }
        }
    }
}
