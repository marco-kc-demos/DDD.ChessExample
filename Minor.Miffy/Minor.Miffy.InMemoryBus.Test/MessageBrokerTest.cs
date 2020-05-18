using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minor.Miffy.InMemoryBus.Test
{
    [TestClass]
    public class MessageBrokerTest
    {
        private string queueName = "My.Name";
        private List<string> topicFilters = new List<string>() { "My.#", "MVM.PolisService" };
        private string queueName2 = "My2.Name";
        private List<string> topicFilters2 = new List<string>() { "My2.#", "MVM.CustomerRegistered" };

        [TestMethod]
        public void DeclareQueue_CreatesQueue()
        {
            var target = new MessageBroker();

            target.QueueDeclare(queueName, topicFilters);

            MessageQueue messageQueue = target.GetNamedMessageQueue(queueName);
            Assert.IsNotNull(messageQueue);
            Assert.AreEqual("My.Name", messageQueue.QueueName);
            CollectionAssert.AreEquivalent(topicFilters, messageQueue.TopicFilters.ToList());
        }

        [TestMethod]
        public void WhenNotDeclared_QueueIsNotThere()
        {
            var target = new MessageBroker();

            Action act = () =>
            {
                MessageQueue messageQueue = target.GetNamedMessageQueue(queueName);
            };

            var ex = Assert.ThrowsException<KeyNotFoundException>(act);
            Assert.AreEqual("The given key 'My.Name' was not present in the dictionary.", ex.Message);
        }

        [TestMethod]
        public void QueueCanBeDeclaredTwice()
        {
            var target = new MessageBroker();
            target.QueueDeclare(queueName, topicFilters);

            target.QueueDeclare(queueName, topicFilters);

            // OK
            Assert.AreEqual("My.Name", target.GetNamedMessageQueue(queueName).QueueName);
        }

        [TestMethod]
        public void QueueCanBeDeclaredTwiceWithSameTopicFilterInDifferentOrder()
        {
            var target = new MessageBroker();
            target.QueueDeclare(queueName, topicFilters);

            var reorderedTopicFilters = new List<string>() { "MVM.PolisService", "My.#" };
            target.QueueDeclare(queueName, reorderedTopicFilters);

            // OK
            var actualFilters = target.GetNamedMessageQueue(queueName).TopicFilters.ToList();
            CollectionAssert.AreEquivalent(topicFilters, actualFilters);
        }

        [TestMethod]
        public void WhenQueueIsRedeclared_TopicFiltersAreCombined()
        {
            var target = new MessageBroker();
            target.QueueDeclare(queueName, topicFilters);

            var newFilters = new List<string> { "Other.#", "MVM.OtherService" };
            Action act = () =>
            {
                target.QueueDeclare(queueName, newFilters);
            };

            var ex = Assert.ThrowsException<BusConfigurationException>(act);
            Assert.AreEqual("Cannot declare the same queue with a different set of topic filters", ex.Message);
        }

        [TestMethod]
        public void CanDeclareTwoQueuesWithTowDifferentNames()
        {
            var target = new MessageBroker();

            target.QueueDeclare(queueName, topicFilters);
            target.QueueDeclare(queueName2, topicFilters2);

            Assert.AreEqual("My.Name", target.GetNamedMessageQueue(queueName).QueueName);
            Assert.AreEqual("My2.Name", target.GetNamedMessageQueue(queueName2).QueueName);
        }

        [TestMethod]
        public void BasicConsumeSetsConsumer()
        {
            var target = new MessageBroker();
            target.QueueDeclare(queueName, topicFilters);

            EventMessageReceivedCallback callback = m => { };
            target.BasicComsume(queueName, callback);

            Assert.IsTrue(target.GetNamedMessageQueue(queueName).HasConsumer);
        }

        [TestMethod]
        public void WhenQueueNotDeclared_BasicConsumeThrowsBusConfigurationException()
        {
            var target = new MessageBroker();

            Action act = () =>
            {
                target.BasicComsume(queueName, m => { });
            };

            var ex = Assert.ThrowsException<BusConfigurationException>(act);
            Assert.AreEqual("Queue 'My.Name' has not been declared.", ex.Message);
        }

        [TestMethod]
        public async Task WhenAMessageIsPublished_ItIsLogged()
        {
            var target = new MessageBroker();
            var message = new EventMessage
            {
                Topic = "My.SomeTopic",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "My.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':5}"),
            };

            await target.BasicPublishAsync(message);

            Assert.AreEqual(1, target.LoggedMessages.Count());
            Assert.AreEqual("My.SomeTopic", target.LoggedMessages.First().Topic);
        }

        [TestMethod]
        public async Task AMessageIsForwardedToAQueueWithMatchingTopicFilter()
        {
            var target = new MessageBroker();
            target.QueueDeclare(queueName, topicFilters);
            EventMessage receivedMessage = null;
            target.BasicComsume(queueName, m => { receivedMessage = m; });

            var message = new EventMessage
            {
                Topic = "My.SomeTopic",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "My.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':5}"),
            };
            await target.BasicPublishAsync(message);

            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual("My.SomeTopic", receivedMessage.Topic);
        }
        
        [TestMethod]
        public async Task AMessageIsNOTForwardedToAQueueWithNoMatchingTopicFilter()
        {
            var target = new MessageBroker();
            target.QueueDeclare(queueName, new List<string>() { "NotMy.#", "MVM.PolisService" });
            EventMessage receivedMessage = null;
            target.BasicComsume(queueName, m => { receivedMessage = m; });

            var message = new EventMessage
            {
                Topic = "My.SomeTopic",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "My.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':5}"),
            };
            await target.BasicPublishAsync(message);

            Assert.IsNull(receivedMessage);
        }

        [TestMethod]
        public async Task AMessageIsForwardedToMoreQueuesWithMatchingTopicFilters()
        {
            var target = new MessageBroker();

            target.QueueDeclare("FirstQueue", new List<string>() { "My.*", "MVM.PolisService" });
            EventMessage receivedFromFirstQueue = null;
            target.BasicComsume("FirstQueue", m => { receivedFromFirstQueue = m; });

            target.QueueDeclare("SecondQueue", new List<string>() { "MVM.OtherService", "*.SomeTopic" });
            EventMessage receivedFromSecondQueue = null;
            target.BasicComsume("SecondQueue", m => { receivedFromSecondQueue = m; });

            var message = new EventMessage
            {
                Topic = "My.SomeTopic",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "My.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':5}"),
            };

            await target.BasicPublishAsync(message);

            Assert.IsNotNull(receivedFromFirstQueue);
            Assert.AreEqual("My.SomeTopic", receivedFromFirstQueue.Topic);
            Assert.IsNotNull(receivedFromSecondQueue);
            Assert.AreEqual("My.SomeTopic", receivedFromSecondQueue.Topic);
        }

        [TestMethod]
        public async Task AMessageIsForwardedOnlyOnceDespiteMoreMatchingTopicFilters()
        {
            var target = new MessageBroker();

            target.QueueDeclare("FirstQueue", new List<string>() { "My.*", "*.SomeTopic" });
            EventMessage receivedFromFirstQueue = null;
            int receiveCount = 0;
            target.BasicComsume("FirstQueue", m => { receivedFromFirstQueue = m; receiveCount++; });

            var message = new EventMessage
            {
                Topic = "My.SomeTopic",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "My.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':5}"),
            };
            await target.BasicPublishAsync(message);

            Assert.IsNotNull(receivedFromFirstQueue);
            Assert.AreEqual("My.SomeTopic", receivedFromFirstQueue.Topic);
            Assert.AreEqual(1, receiveCount);
        }
    }
}
