using System;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Minor.Miffy
{
    public class RabbitMQMessageSender : IMessageSender
    {
        private readonly IBusContext<IConnection> _buscontext;
        private readonly IModel _channel;

        public RabbitMQMessageSender(IBusContext<IConnection> buscontext)
        {
            _buscontext = buscontext;
            _channel = _buscontext.Connection.CreateModel();
            _channel.ExchangeDeclare(_buscontext.ExchangeName, ExchangeType.Topic);
        }

        public Task SendMessageAsync(EventMessage eventMessage)
        {
            return Task.Run(() =>
            {
                IBasicProperties props = _channel.CreateBasicProperties();
                props.CorrelationId = eventMessage.CorrelationId.ToString();
                props.Timestamp = new AmqpTimestamp(eventMessage.Timestamp);
                props.Type = eventMessage.EventType ?? "void";

                _channel.BasicPublish(
                    _buscontext.ExchangeName, 
                    eventMessage.Topic, 
                    props, 
                    new ReadOnlyMemory<byte>(eventMessage.Body));
            });
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}