using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Miffy.RabbitMQBus;
using Moq;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Minor.Miffy.RabbitMQBus.Test
{
    [TestClass]
    public class RabbitMQMessageSenderTest
    {
        private string _exchangeName;
        private string _topic;
        private byte[] _body;
        private EventMessage _eventMessage;

        private Mock<IBasicProperties> _propsMock;
        private Mock<IModel> _channelMock;
        private Mock<IConnection> _connectionMock;
        private RabbitMQBusContext _busContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _exchangeName = "My.Test.ExchangeName";
            _topic = "My.Test.Topic";
            _body = Encoding.Unicode.GetBytes("Hello, World");
            _eventMessage = new EventMessage
            {
                Topic = _topic,
                Body = _body,
            };

            _propsMock = new Mock<IBasicProperties>();
            _channelMock = new Mock<IModel>();
            _channelMock.Setup(c => c.CreateBasicProperties()).Returns(_propsMock.Object);
            _connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            _connectionMock.Setup(conn => conn.CreateModel()).Returns(_channelMock.Object);

            _busContext = new RabbitMQBusContext(_connectionMock.Object, _exchangeName);
        }

        [TestMethod]
        public async Task EventMessageSenderDeclaresExchange()
        {
            var target = new RabbitMQMessageSender(_busContext);

            await target.SendMessageAsync(_eventMessage);

            _channelMock.Verify(c => c.ExchangeDeclare(_exchangeName, ExchangeType.Topic,
                                                     It.IsAny<bool>(), It.IsAny<bool>(), null));
        }

        [TestMethod]
        public void EventMessageSenderDisposes()
        {
            var target = new RabbitMQMessageSender(_busContext);

            target.Dispose();

            _channelMock.Verify(c => c.Dispose());
        }

        [TestMethod]
        public async Task EventMessageSenderSendsCorrectMessageToCorrectExchangeWithCorrectTopic()
        {
            var target = new RabbitMQMessageSender(_busContext);

            await target.SendMessageAsync(_eventMessage);

            _channelMock.Verify(c => c.BasicPublish(_exchangeName, _topic, false, _propsMock.Object, _body));
        }

        [TestMethod]
        public async Task EventMessageSenderSendsCorrectHeaderInfo()
        {
            _propsMock.SetupProperty(p => p.CorrelationId);
            _propsMock.SetupProperty(p => p.Timestamp);
            _propsMock.SetupProperty(p => p.Type);

            Guid guid = Guid.NewGuid();
            long timestamp = DateTime.Now.Ticks;
            _eventMessage.CorrelationId = guid;
            _eventMessage.Timestamp = timestamp;
            _eventMessage.EventType = "String";

            var target = new RabbitMQMessageSender(_busContext);

            await target.SendMessageAsync(_eventMessage);

            Assert.AreEqual(guid.ToString(), _propsMock.Object.CorrelationId);
            Assert.AreEqual(new AmqpTimestamp(timestamp), _propsMock.Object.Timestamp);
            Assert.AreEqual("String", _propsMock.Object.Type);
        }
    }
}
