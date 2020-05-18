using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minor.Miffy.InMemoryBus.Test
{
    [TestClass]
    public class InMemoryMessageReceiverTest
    {
        [TestMethod]
        public void InMemoryMessageReceiverHasQueueNameAndTopicFilters()
        {
            var queueName = "My.QueueName";
            var topicFilters = new List<string> { "My.*.Filter", "MVM.#" };
            var context = new InMemoryContext();

            var target = new InMemoryMessageReceiver(context, queueName, topicFilters);

            Assert.AreEqual("My.QueueName", target.QueueName);
            CollectionAssert.AreEquivalent(topicFilters, target.TopicFilters.ToList());
        }

        [TestMethod]
        public void StartReceivingMessagesDeclaresQueue()
        {
            var queueName = "My.QueueName";
            var topicFilters = new List<string> { "My.*.Filter", "MVM.#" };
            var context = new InMemoryContext();
            var target = new InMemoryMessageReceiver(context, queueName, topicFilters);

            target.StartReceivingMessages();

            var queue = context.Connection.GetNamedMessageQueue(queueName);
            Assert.AreEqual("My.QueueName", queue.QueueName);
            CollectionAssert.AreEquivalent(topicFilters, queue.TopicFilters.ToList());
        }

        [TestMethod]
        public void CannotCall_StartReceivingMessages_Twice()
        {
            var queueName = "My.QueueName";
            var topicFilters = new List<string> { "My.*.Filter", "MVM.#" };
            var context = new InMemoryContext();
            var target = new InMemoryMessageReceiver(context, queueName, topicFilters);
            target.StartReceivingMessages();

            Action act = () =>
            {
                target.StartReceivingMessages();
            };

            var ex = Assert.ThrowsException<BusException>(act);
            Assert.AreEqual("Cannot call 'StartReceivingMessages' multiple times.",
                ex.Message);
        }

        [TestMethod]
        public void StartHandlingMessagesRegistersCallback()
        {
            var queueName = "My.QueueName";
            var topicFilters = new List<string> { "My.*.Filter", "MVM.#" };
            var context = new InMemoryContext();
            var target = new InMemoryMessageReceiver(context, queueName, topicFilters);
            target.StartReceivingMessages();

            EventMessageReceivedCallback callback = m => { };
            target.StartHandlingMessages(callback);

            var queue = context.Connection.GetNamedMessageQueue(queueName);
            Assert.AreEqual(true, queue.HasConsumer);
        }

        [TestMethod]
        public async Task StartHandlingMessagesHandlesMessages()
        {
            var queueName = "My.QueueName";
            var topicFilters = new List<string> { "My.*.Filter", "MVM.#" };
            var context = new InMemoryContext();
            var target = new InMemoryMessageReceiver(context, queueName, topicFilters);
            target.StartReceivingMessages();

            EventMessage receivedMessage = null;
            target.StartHandlingMessages(m => { receivedMessage = m;  });

            var queue = context.Connection.GetNamedMessageQueue(queueName);
            await queue.PublishAsync(new EventMessage { Topic = "Some.Topic" });
            Assert.AreEqual("Some.Topic", receivedMessage.Topic);
        }

        [TestMethod]
        public void CannotCall_StartReceivingMessages_WithoutCallingStartReceivingMessagesFirst()
        {
            var queueName = "My.QueueName";
            var topicFilters = new List<string> { "My.*.Filter", "MVM.#" };
            var context = new InMemoryContext();
            var target = new InMemoryMessageReceiver(context, queueName, topicFilters);

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
            var queueName = "My.QueueName";
            var topicFilters = new List<string> { "My.*.Filter", "MVM.#" };
            var context = new InMemoryContext();
            var target = new InMemoryMessageReceiver(context, queueName, topicFilters);
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

        [TestMethod]
        public void DisposeUnregistersCallback()
        {
            var queueName = "My.QueueName";
            var topicFilters = new List<string> { "My.*.Filter", "MVM.#" };
            var context = new InMemoryContext();
            var target = new InMemoryMessageReceiver(context, queueName, topicFilters);
            target.StartReceivingMessages();
            EventMessageReceivedCallback callback = m => { };
            target.StartHandlingMessages(callback);

            // Check Arrange
            var queue = context.Connection.GetNamedMessageQueue(queueName);
            Assert.AreEqual(true, queue.HasConsumer);

            // Act
            target.Dispose();

            // Assert
            queue = context.Connection.GetNamedMessageQueue(queueName);
            Assert.AreEqual(false, queue.HasConsumer);
        }
        
        [TestMethod]
        public void CannotCall_StartReceivingMessages_AfterDispose()
        {
            var queueName = "My.QueueName";
            var topicFilters = new List<string> { "My.*.Filter", "MVM.#" };
            var context = new InMemoryContext();
            var target = new InMemoryMessageReceiver(context, queueName, topicFilters);
            target.StartReceivingMessages();
            target.Dispose();

            Action act = () =>
            {
                target.StartHandlingMessages(em => { });
            };

            var ex = Assert.ThrowsException<ObjectDisposedException>(act);
            Assert.AreEqual("Cannot access a disposed object.\r\nObject name: 'InMemoryMessageReceiver'.",
                ex.Message);
        }

        [TestMethod]
        public void CannotCall_StartHandlingMessages_AfterDispose()
        {
            var queueName = "My.QueueName";
            var topicFilters = new List<string> { "My.*.Filter", "MVM.#" };
            var context = new InMemoryContext();
            var target = new InMemoryMessageReceiver(context, queueName, topicFilters);
            target.Dispose();

            Action act = () =>
            {
                target.StartReceivingMessages();
            };

            var ex = Assert.ThrowsException<ObjectDisposedException>(act);
            Assert.AreEqual("Cannot access a disposed object.\r\nObject name: 'InMemoryMessageReceiver'.",
                ex.Message);
        }

    }
}
