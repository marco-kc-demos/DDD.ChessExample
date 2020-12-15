using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Minor.Miffy.RabbitMQBus.Test
{
    [TestClass]
    public class RabbitMQMessageReceiverTest
    {
        private string _exchangeName;
        private string _queueName;
        private string[] _topicFilters;
        private string _topic;
        private byte[] _body;
        private EventMessage _eventMessage;

        private Mock<IBasicProperties> _propMock;
        private Mock<IBasicProperties> _propsFromChannelMock;
        private Mock<IModel> _channelMock;
        private Mock<IConnection> _connectionMock;
        private RabbitMQBusContext _busContext;
        private EventingBasicConsumer _consumer;

        [TestInitialize]
        public void TestInitialize()
        {
            _exchangeName = "My.Test.ExchangeName";
            _queueName = "My.Test.ListenQueue";
            _topicFilters = new string[] { "My.Test.Topic", "My.Test.OtherTopic" };

            _topic = "My.Test.Topic";
            _body = Encoding.Unicode.GetBytes("Hello, World");
            _eventMessage = new EventMessage
            {
                Topic = _topic,
                Body = _body,
            };

            _propMock = new Mock<IBasicProperties>(MockBehavior.Loose);
            _propsFromChannelMock = new Mock<IBasicProperties>();
            _channelMock = new Mock<IModel>();
            _channelMock.Setup(c => c.CreateBasicProperties()).Returns(_propsFromChannelMock.Object);
            _channelMock.Setup(c => c.BasicConsume(_queueName, true, It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), 
                                                   It.IsAny<IDictionary<string, object>>(), It.IsAny<IBasicConsumer>()))
                .Callback((string a, bool b, string c, bool d, bool e, IDictionary<string, object> args,
                           IBasicConsumer consumer) => _consumer = (EventingBasicConsumer)consumer);
            
            
            _connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            _connectionMock.Setup(conn => conn.CreateModel()).Returns(_channelMock.Object);

            _busContext = new RabbitMQBusContext(_connectionMock.Object, _exchangeName);
        }

        #region Test Constructor
        [TestMethod]
        public void MessageReceiverCreatesChannel()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);

            _connectionMock.Verify(conn => conn.CreateModel());
        }
        #endregion Test Constructor

        #region Test StartReceivingMessages
        [TestMethod]
        public void MessageReceiverDeclaresExchangeAndQueueAndBindings()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);

            target.StartReceivingMessages();

            _channelMock.Verify(c => c.ExchangeDeclare(_exchangeName, ExchangeType.Topic,
                                                     It.IsAny<bool>(), It.IsAny<bool>(), null));
            _channelMock.Verify(c => c.QueueDeclare(_queueName, true, false, false, null));
            _channelMock.Verify(c => c.QueueBind(_queueName, _exchangeName, "My.Test.Topic", null));
            _channelMock.Verify(c => c.QueueBind(_queueName, _exchangeName, "My.Test.OtherTopic", null));
        }

        [TestMethod]
        public void CannotCall_StartReceivingMessages_Twice()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);
            target.StartReceivingMessages();

            Action act = () =>
            {
                target.StartReceivingMessages();
            };

            var ex = Assert.ThrowsException<BusException>(act);
            Assert.AreEqual("Cannot call 'StartReceivingMessages' multiple times.",
                ex.Message);
        }
        #endregion Test StartReceivingMessages

        #region Test StartHandlingMessages(EventMessageReceivedCallback Callback)
        [TestMethod]
        public void StartHandlingMessages_CreatesConsumer()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);
            target.StartReceivingMessages();

            // Act
            var callback = new EventMessageReceivedCallback(em => { });
            target.StartHandlingMessages(callback);

            Assert.IsNotNull(_consumer);
        }

        [TestMethod]
        public void StartHandlingMessages_CallsCallbackOnReceivingMessages()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);
            target.StartReceivingMessages();
            bool hasBeenCalled = false;
            target.StartHandlingMessages(em => { hasBeenCalled = true; });

            // Act
            _consumer.HandleBasicDeliver("ctag", 5, false, _exchangeName, _topic, _propMock.Object, _body);

            Assert.AreEqual(true, hasBeenCalled);
        }

        [TestMethod]
        public void StartHandlingMessages_CallsCallbackWithCorrectEventMessage()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);
            target.StartReceivingMessages();
            EventMessage message = null;
            target.StartHandlingMessages(em => { message = em; });

            var guid = Guid.NewGuid();
            var timestamp = new AmqpTimestamp(3_141_592_653);
            _propMock.SetupProperty(p => p.CorrelationId, guid.ToString());
            _propMock.SetupProperty(p => p.Timestamp, timestamp);
            _propMock.SetupProperty(p => p.Type, "MyBank.AccountOpened");
            byte[] body = Encoding.Unicode.GetBytes("{AccountOpened data in Json}");
            var buffer = new ReadOnlyMemory<byte>(body);
            // Act
            _consumer.HandleBasicDeliver("ctag", 5, false, _exchangeName, _topic, _propMock.Object, buffer);

            // Assert
            Assert.AreEqual(_topic, message.Topic);
            Assert.AreEqual(guid, message.CorrelationId);
            Assert.AreEqual(3_141_592_653, message.Timestamp);
            Assert.AreEqual("MyBank.AccountOpened", message.EventType);
            CollectionAssert.AreEqual(body, message.Body);
        }


        [TestMethod]
        public void StartHandlingMessages_CanHandleCallbackWithEmptyValues()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);
            target.StartReceivingMessages();
            EventMessage message = null;
            target.StartHandlingMessages(em => { message = em; });

            // Act
            _consumer.HandleBasicDeliver("ctag", 5, false, _exchangeName, 
                                         routingKey: null, _propMock.Object, body: null);

            // Assert
            Assert.AreEqual(null, message.Topic);
            Assert.AreEqual(Guid.Empty, message.CorrelationId);
            Assert.AreEqual(0, message.Timestamp);
            Assert.AreEqual(null, message.EventType);
            Assert.IsTrue(message.Body.Length == 0);
        }


        [TestMethod]
        public void CannotCall_StartReceivingMessages_WithoutCallingStartReceivingMessagesFirst()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);

            Action act = () =>
            {
                target.StartHandlingMessages(em => { });
            };

            var ex = Assert.ThrowsException<BusException>(act);
            Assert.AreEqual("Before calling 'StartHandlingMessages', call 'StartReceivingMessages' first to declare queue and topics.",
                ex.Message);
        }

        [TestMethod]
        public void CannotCall_StartHandlingMessages_Twice()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);
            target.StartReceivingMessages();
            target.StartHandlingMessages(em => { });

            Action act = () =>
            {
                target.StartHandlingMessages(em => { });
            };

            var ex = Assert.ThrowsException<BusException>(act);
            Assert.AreEqual("Cannot call 'StartHandlingMessages' multiple times.",
                ex.Message);
        }
        #endregion Test StartHandlingMessages(EventMessageReceivedCallback Callback)

        #region Test Dispose
        [TestMethod]
        public void MessageReceiverDisposes()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);

            target.Dispose();

            _channelMock.Verify(c => c.Dispose());
        }

        [TestMethod]
        public void CannotCall_StartReceivingMessages_AfterDispose()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);
            target.StartReceivingMessages();
            target.Dispose();

            Action act = () =>
            {
                target.StartHandlingMessages(em => { });
            };

            var ex = Assert.ThrowsException<ObjectDisposedException>(act);
            Assert.AreEqual("Cannot access a disposed object.\r\nObject name: 'RabbitMQMessageReceiver'.",
                ex.Message);
        }


        [TestMethod]
        public void CannotCall_StartHandlingMessages_AfterDispose()
        {
            var target = new RabbitMQMessageReceiver(_busContext, _queueName, _topicFilters);
            target.Dispose();

            Action act = () =>
            {
                target.StartReceivingMessages();
            };

            var ex = Assert.ThrowsException<ObjectDisposedException>(act);
            Assert.AreEqual("Cannot access a disposed object.\r\nObject name: 'RabbitMQMessageReceiver'.",
                ex.Message);
        }
        #endregion Test Dispose
    }
}
