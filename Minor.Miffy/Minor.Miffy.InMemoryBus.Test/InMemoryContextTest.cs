using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Minor.Miffy.InMemoryBus.Test
{
    [TestClass]
    public class InMemoryContextTest
    {
        [TestMethod]
        public void InMemoryContextHasConnectionAndExchange()
        {
            var target = new InMemoryContext();

            Assert.IsInstanceOfType(target.Connection, typeof(MessageBroker));
            Assert.AreEqual("In-memory message bus", target.ExchangeName);
        }

        [TestMethod]
        public void CreateMessageSenderCreatesInMemoryMessageSender()
        {
            var target = new InMemoryContext();

            // Act
            var sender = target.CreateMessageSender();

            // Assert
            Assert.IsInstanceOfType(sender, typeof(InMemoryMessageSender));
        }

        [TestMethod]
        public void CreateMessageReceiverCreatesInMemoryMessageReceiver()
        {
            var target = new InMemoryContext();

            // Act
            string queueName = "My.Test.ListenQueue";
            string[] topicFilters = new string[] { "My.Test.Topic", "My.Test.OtherTopic" };
            var receiver = target.CreateMessageReceiver(queueName, topicFilters);

            // Assert
            Assert.IsInstanceOfType(receiver, typeof(InMemoryMessageReceiver));
            Assert.AreEqual("My.Test.ListenQueue", receiver.QueueName);
            CollectionAssert.AreEquivalent(topicFilters, receiver.TopicFilters.ToList());
        }

        [TestMethod]
        public void RabbitMQBusContextDisposesConnection()
        {
            var target = new InMemoryContext();

            target.Dispose();

            // Nothing happens
        }

    }
}
