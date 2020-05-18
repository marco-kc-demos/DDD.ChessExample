using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minor.Miffy.RabbitMQBus.Test
{
    [TestClass]
    public class RabbitMQBusContextTest
    {
        [TestMethod]
        public void RabbitMQBusContextHasConnectionAndExchange()
        {
            Mock<IConnection> connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            string exchangeName = "My.Test.ExchangeName";

            var target = new RabbitMQBusContext(connectionMock.Object, exchangeName);

            Assert.AreEqual(connectionMock.Object, target.Connection);
            Assert.AreEqual("My.Test.ExchangeName", target.ExchangeName);
        }

        [TestMethod]
        public void CreateMessageSenderCreatesRabbitMQMessageSender()
        {
            Mock<IModel> channelMock = new Mock<IModel>();
            Mock<IConnection> connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            connectionMock.Setup(conn => conn.CreateModel()).Returns(channelMock.Object);

            var target = new RabbitMQBusContext(connectionMock.Object, "My.Test.ExchangeName");

            // Act
            var sender = target.CreateMessageSender();

            // Assert
            Assert.IsInstanceOfType(sender, typeof(RabbitMQMessageSender));
            connectionMock.Verify(conn => conn.CreateModel());
        }

        [TestMethod]
        public void CreateMessageReceiverCreatesRabbitMQMessageReceiver()
        {
            Mock<IModel> channelMock = new Mock<IModel>();
            Mock<IConnection> connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            connectionMock.Setup(conn => conn.CreateModel()).Returns(channelMock.Object);

            var target = new RabbitMQBusContext(connectionMock.Object, "My.Test.ExchangeName");

            // Act
            string queueName = "My.Test.ListenQueue";
            string[] topicFilters = new string[] { "My.Test.Topic", "My.Test.OtherTopic" };
            var receiver = target.CreateMessageReceiver(queueName, topicFilters);

            // Assert
            Assert.IsInstanceOfType(receiver, typeof(RabbitMQMessageReceiver));
            connectionMock.Verify(conn => conn.CreateModel());
            Assert.AreEqual("My.Test.ListenQueue", receiver.QueueName);
            CollectionAssert.AreEquivalent(topicFilters, receiver.TopicFilters.ToList());
        }

        [TestMethod]
        public void RabbitMQBusContextDisposesConnection()
        {
            Mock<IConnection> connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            connectionMock.Setup(conn => conn.Dispose());
            var target = new RabbitMQBusContext(connectionMock.Object, "My.Test.ExchangeName");

            target.Dispose();

            connectionMock.Verify(c => c.Dispose());
        }
    }
}
