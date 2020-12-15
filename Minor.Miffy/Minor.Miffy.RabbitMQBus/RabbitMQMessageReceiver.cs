using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minor.Miffy.RabbitMQBus
{
    public class RabbitMQMessageReceiver : IMessageReceiver
    {
        private readonly RabbitMQBusContext _buscontext;
        private IModel _channel;
        private bool _hasStartedReceivingMessages;
        private bool _hasStartHandlingMessages;
        private bool _isDisposed;

        public string QueueName { get; private set; }
        public IEnumerable<string> TopicFilters { get; private set; }

        public RabbitMQMessageReceiver(RabbitMQBusContext buscontext, string queueName, IEnumerable<string> topicFilters)
        {
            _buscontext = buscontext;
            QueueName = queueName;
            TopicFilters = topicFilters;

            _hasStartedReceivingMessages = false;
            _hasStartHandlingMessages = false;
            _isDisposed = false;

            _channel = _buscontext.Connection.CreateModel();
        }

        public void StartReceivingMessages()
        {
            CanStartReceivingMessages();
            _hasStartedReceivingMessages = true;

            _channel.ExchangeDeclare(_buscontext.ExchangeName, ExchangeType.Topic);
            _channel.QueueDeclare(QueueName,
                    durable: true, exclusive: false, autoDelete: false, arguments: null);

            foreach (var topicFilter in TopicFilters)
            {
                _channel.QueueBind(QueueName, _buscontext.ExchangeName, topicFilter);
            }
        }

        public void StartHandlingMessages(EventMessageReceivedCallback Callback)
        {
            CanStartHandlingMessages();
            _hasStartHandlingMessages = true;

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var eventMesage = ConvertToEventMessage(e);
                Callback.Invoke(eventMesage);

            };
            _channel.BasicConsume(QueueName, autoAck: true, consumer);
        }

        public void Dispose()
        {
            _isDisposed = true;
            _channel?.Dispose();
        }

        private EventMessage ConvertToEventMessage(BasicDeliverEventArgs e)
        {
            if (!Guid.TryParse(e.BasicProperties.CorrelationId, out Guid guid))
            {
                guid = Guid.Empty;
            }
            return new EventMessage
            {
                Topic = e.RoutingKey,
                CorrelationId = guid,
                Timestamp = e.BasicProperties.Timestamp.UnixTime,
                EventType = e.BasicProperties.Type,
                Body = e.Body.ToArray(),
            };
        }

        private void CanStartReceivingMessages()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQMessageReceiver));
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
                throw new ObjectDisposedException(nameof(RabbitMQMessageReceiver));
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
