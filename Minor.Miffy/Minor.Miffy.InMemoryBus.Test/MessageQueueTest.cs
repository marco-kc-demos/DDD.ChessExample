using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minor.Miffy.InMemoryBus.Test
{
    [TestClass]
    public class MessageQueueTest
    {
        private string queueName = "My.Name";
        private List<string> topicFilters = new List<string>() { "My.#", "MVM.PolisService" };

        [TestMethod]
        public void MessageQueueHasNameAndTopicFilters()
        {
            var target = new MessageQueue(queueName, topicFilters);

            Assert.AreEqual("My.Name", target.QueueName);
            CollectionAssert.AreEquivalent(topicFilters, target.TopicFilters.ToList());
        }

        [TestMethod]
        public void NewMessageQueueHasNoMessagesAndNoConsumer()
        {
            var target = new MessageQueue(queueName, topicFilters);

            Assert.AreEqual(0, target.QueuedMessages.Count());
            Assert.AreEqual(false, target.HasConsumer);
        }

        [TestMethod]
        public async Task WithNoConsumer_PublishedMessageGoesIntoQueue()
        {
            var target = new MessageQueue(queueName, topicFilters);
            var message = new EventMessage
            {
                Topic = "MVM.SomeTopic",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "MVM.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':5}"),
            };

            await target.PublishAsync(message);

            Assert.AreEqual(1, target.QueuedMessages.Count());
            Assert.IsTrue(target.QueuedMessages.Any(m => m.Topic == "MVM.SomeTopic"));
        }


        [TestMethod]
        public async Task WithNoConsumer_PublishedMessageStayInSequence()
        {
            var target = new MessageQueue(queueName, topicFilters);
            var message1 = new EventMessage
            {
                Topic = "MVM.SomeTopic1",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "MVM.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':1}"),
            };
            var message2 = new EventMessage
            {
                Topic = "MVM.SomeTopic2",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "MVM.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':2}"),
            };

            await target.PublishAsync(message1);
            await target.PublishAsync(message2);

            Assert.AreEqual(2, target.QueuedMessages.Count());
            Assert.AreEqual("MVM.SomeTopic1", target.QueuedMessages.ElementAt(0).Topic);
            Assert.AreEqual("MVM.SomeTopic2", target.QueuedMessages.ElementAt(1).Topic);
        }

        [TestMethod]
        public void AfterSettingConsumer_MessageQueueHasConsumer()
        {
            var target = new MessageQueue(queueName, topicFilters);

            EventMessageReceivedCallback callback = m => { };
            target.SetConsumer(callback);

            Assert.AreEqual(true, target.HasConsumer);
        }

        [TestMethod]
        public async Task AfterSettingConsumer_PublishedMessageGoesToConsumer()
        {
            var target = new MessageQueue(queueName, topicFilters);
            var message = new EventMessage
            {
                Topic = "MVM.SomeTopic",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "MVM.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':5}"),
            };
            EventMessage receivedMessage = null;
            target.SetConsumer(m => { receivedMessage = m; });

            await target.PublishAsync(message);

            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual(message.Topic, receivedMessage.Topic);
            Assert.AreEqual(message.CorrelationId, receivedMessage.CorrelationId);
            Assert.AreEqual(message.Timestamp, receivedMessage.Timestamp);
            Assert.AreEqual(message.EventType, receivedMessage.EventType);
            Assert.AreEqual(message.Body, receivedMessage.Body);
        }

        [TestMethod]
        public async Task AfterSettingConsumer_PreviouslyPublishedMessagesWillBeConsumed()
        {
            var target = new MessageQueue(queueName, topicFilters);
            var message1 = new EventMessage
            {
                Topic = "MVM.SomeTopic1",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "MVM.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':1}"),
            };
            var message2 = new EventMessage
            {
                Topic = "MVM.SomeTopic2",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "MVM.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':2}"),
            };
            await target.PublishAsync(message1);
            await target.PublishAsync(message2);

            var receivedEventMessages = new List<EventMessage>();
            target.SetConsumer(m => { receivedEventMessages.Add(m); });

            Assert.AreEqual(0, target.QueuedMessages.Count());
            Assert.AreEqual(message1.Topic, receivedEventMessages[0].Topic);
            Assert.AreEqual(message2.Topic, receivedEventMessages[1].Topic);
        }

        [TestMethod]
        public async Task AfterConsumingPreviouslyPublisheMessagesNewMessageWillDirectlyBeConsumed()
        {
            var target = new MessageQueue(queueName, topicFilters);
            var message1 = new EventMessage
            {
                Topic = "MVM.SomeTopic1",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "MVM.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':1}"),
            };
            var message2 = new EventMessage
            {
                Topic = "MVM.SomeTopic2",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "MVM.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':2}"),
            };
            await target.PublishAsync(message1);
            await target.PublishAsync(message2);

            var receivedEventMessages = new List<EventMessage>();
            target.SetConsumer(m => { receivedEventMessages.Add(m); });

            // Act
            var message3 = new EventMessage
            {
                Topic = "MVM.SomeTopic3",
                CorrelationId = Guid.NewGuid(), Timestamp = DateTime.Now.Ticks, EventType = "MVM.SomethingHappened", Body = Encoding.Unicode.GetBytes("{'Number':2}"),
            };
           await target.PublishAsync(message3);

            Assert.AreEqual(0, target.QueuedMessages.Count());
            Assert.AreEqual(3, receivedEventMessages.Count());
            Assert.AreEqual(message1.Topic, receivedEventMessages[0].Topic);
            Assert.AreEqual(message2.Topic, receivedEventMessages[1].Topic);
            Assert.AreEqual(message3.Topic, receivedEventMessages[2].Topic);
        }

        [TestMethod]
        public void CannotSetConsumerTwice()
        {
            var target = new MessageQueue(queueName, topicFilters);
            target.SetConsumer(m => { });

            Action act = () =>
            {
                target.SetConsumer(m => { });
            };

            var ex = Assert.ThrowsException<InvalidOperationException>(act);
            Assert.AreEqual("Cannot subscribe a second listener to the same queue.", ex.Message);
        }

        [TestMethod]
        public void MessageQueueHAsNoConsumersAfterRemovingLastComsumer()
        {
            var target = new MessageQueue(queueName, topicFilters);

            EventMessageReceivedCallback callback = m => { };
            target.SetConsumer(callback);
            Assert.AreEqual(true, target.HasConsumer);

            target.RemoveConsumer(callback);

            Assert.AreEqual(false, target.HasConsumer);
        }

        [TestMethod]
        public async Task AfterRemovingLastComsumer_PublishedMessageDoNotGoToConsumer()
        {
            // Arrange
            var target = new MessageQueue(queueName, topicFilters);
            var message = new EventMessage { Topic = "MVM.SomeTopic" };
            EventMessage receivedMessage;
            EventMessageReceivedCallback callback = m => { receivedMessage = m; };
            target.SetConsumer(callback);

            // Check arrange
            receivedMessage = null;
            await target.PublishAsync(message);
            Assert.IsNotNull(receivedMessage);

            // Act
            target.RemoveConsumer(callback);

            // Assert
            receivedMessage = null; 
            await target.PublishAsync(message);
            Assert.IsNull(receivedMessage);

        }
    }
}
