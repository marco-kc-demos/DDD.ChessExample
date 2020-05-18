using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Miffy;
using Minor.Miffy.InMemoryBus;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDD.Core.Application.Test.EventListening
{
    [TestClass]
    public class EventListenerTest
    {
        [TestMethod]
        public void StartListeningCreatesListenerWithCorrectQueueNameAndTopicFilters()
        {
            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Strict);
            contextMock.Setup(bc => bc.CreateMessageReceiver("My.Test.Queue", It.IsAny<IEnumerable<string>>()))
                       .Returns(new Mock<IMessageReceiver>().Object);

            var dispatchers = new Dictionary<string, IEventDispatcher>();
            dispatchers.Add("My.#", new Mock<IEventDispatcher>().Object);
            dispatchers.Add("Your.*", new Mock<IEventDispatcher>().Object);

            var target = new EventListener<string>(contextMock.Object, dispatchers);

            // Act
            target.StartListening("My.Test.Queue");

            // Assert
            contextMock.Verify(bc => bc.CreateMessageReceiver("My.Test.Queue", new List<string> { "My.#", "Your.*" }), Times.Once);
        }

        [TestMethod]
        public void StartListeningCallsStartReceivingMessagesAndStartHandlingMessages()
        {
            var receiverMock = new Mock<IMessageReceiver>(MockBehavior.Strict);
            receiverMock.Setup(r => r.StartReceivingMessages());
            receiverMock.Setup(r => r.StartHandlingMessages(It.IsAny<EventMessageReceivedCallback>()));

            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Strict);
            contextMock.Setup(bc => bc.CreateMessageReceiver("My.Test.Queue", It.IsAny<IEnumerable<string>>()))
                       .Returns(receiverMock.Object);

            var dispatchers = new Dictionary<string, IEventDispatcher>();

            var target = new EventListener<string>(contextMock.Object, dispatchers);

            // Act
            target.StartListening("My.Test.Queue");

            // Assert
            receiverMock.Verify(r => r.StartReceivingMessages());
            receiverMock.Verify(r => r.StartHandlingMessages(It.IsAny<EventMessageReceivedCallback>()), Times.Once);
        }

        [TestMethod]
        public void CannotStartListeningTwice()
        {
            var receiverMock = new Mock<IMessageReceiver>(MockBehavior.Loose);
            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Loose);
            contextMock.Setup(bc => bc.CreateMessageReceiver("My.Test.Queue", It.IsAny<IEnumerable<string>>()))
                       .Returns(receiverMock.Object);
            var dispatchers = new Dictionary<string, IEventDispatcher>();

            var target = new EventListener<string>(contextMock.Object, dispatchers);

            Action act = () =>
            {
                target.StartListening("My.Test.Queue");
                target.StartListening("My.Test.Queue");
            };

            // Assert
            var ex = Assert.ThrowsException<BusException>(act);
            Assert.AreEqual("An EventListener cannot Start Listening twice.", ex.Message);
        }

        [TestMethod]
        public void MessageGoesToDispatcher()
        {
            EventMessageReceivedCallback callback = null;
            var receiverMock = new Mock<IMessageReceiver>(MockBehavior.Strict);
            receiverMock.Setup(r => r.StartReceivingMessages());
            receiverMock.Setup(r => r.StartHandlingMessages(It.IsAny<EventMessageReceivedCallback>()))
                        .Callback<EventMessageReceivedCallback>(emrc => { callback = emrc; });

            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Strict);
            contextMock.Setup(bc => bc.CreateMessageReceiver("My.Test.Queue", It.IsAny<IEnumerable<string>>()))
                       .Returns(receiverMock.Object);

            var dispatcherMock = new Mock<IEventDispatcher>(MockBehavior.Strict);
            dispatcherMock.Setup(d => d.Dispatch(It.IsAny<EventMessage>()));
            var dispatchers = new Dictionary<string, IEventDispatcher>();
            dispatchers.Add("My.#", dispatcherMock.Object);

            var target = new EventListener<string>(contextMock.Object, dispatchers);
            target.StartListening("My.Test.Queue");

            // Act
            var message = new EventMessage { Topic = "My.Topic" };
            callback(message);

            // Assert
            dispatcherMock.Verify(d => d.Dispatch(message));
        }

        [TestMethod]
        public async Task MessageGoesToDispatcher2()
        {
            var dispatcherMock = new Mock<IEventDispatcher>(MockBehavior.Strict);
            dispatcherMock.Setup(d => d.Dispatch(It.IsAny<EventMessage>()));
            var dispatchers = new Dictionary<string, IEventDispatcher>();
            dispatchers.Add("My.#", dispatcherMock.Object);

            IBusContext<MessageBroker> context = new InMemoryContext();
            var target = new EventListener<MessageBroker>(context, dispatchers);
            target.StartListening("My.Test.Queue");

            // Act
            var message = new EventMessage { Topic = "My.Topic" };
            var sender = context.CreateMessageSender();
            await sender.SendMessageAsync(message);

            // Assert
            dispatcherMock.Verify(d => d.Dispatch(message));
        }

        [TestMethod]
        public async Task MessageGoesToAllMatchingDispatchers()
        {
            var myDispatcherMock = new Mock<IEventDispatcher>(MockBehavior.Strict);
            myDispatcherMock.Setup(d => d.Dispatch(It.IsAny<EventMessage>()));
            var topicDispatcherMock = new Mock<IEventDispatcher>(MockBehavior.Strict);
            topicDispatcherMock.Setup(d => d.Dispatch(It.IsAny<EventMessage>()));
            var dispatchers = new Dictionary<string, IEventDispatcher>();
            dispatchers.Add("My.#", myDispatcherMock.Object);
            dispatchers.Add("*.Topic", topicDispatcherMock.Object);

            IBusContext<MessageBroker> context = new InMemoryContext();
            var target = new EventListener<MessageBroker>(context, dispatchers);
            target.StartListening("My.Test.Queue");

            // Act
            var message = new EventMessage { Topic = "My.Topic" };
            var sender = context.CreateMessageSender();
            await sender.SendMessageAsync(message);

            // Assert
            myDispatcherMock.Verify(d => d.Dispatch(message));
            topicDispatcherMock.Verify(d => d.Dispatch(message));
        }

        [TestMethod]
        public async Task MessageDoesNotGoToNonMatchingDispatchers()
        {
            var dispatcherMock = new Mock<IEventDispatcher>(MockBehavior.Strict);
            dispatcherMock.Setup(d => d.Dispatch(It.IsAny<EventMessage>()));
            var dispatchers = new Dictionary<string, IEventDispatcher>();
            dispatchers.Add("My.#", dispatcherMock.Object);

            IBusContext<MessageBroker> context = new InMemoryContext();
            var target = new EventListener<MessageBroker>(context, dispatchers);
            target.StartListening("My.Test.Queue");

            // Act
            var message = new EventMessage { Topic = "NotMy.Topic" };
            var sender = context.CreateMessageSender();
            await sender.SendMessageAsync(message);

            // Assert
            dispatcherMock.Verify(d => d.Dispatch(message), Times.Never);
        }

        [TestMethod]
        public void EventListenerDisposesReceiver()
        {
            var receiverMock = new Mock<IMessageReceiver>(MockBehavior.Strict);
            receiverMock.Setup(r => r.StartReceivingMessages());
            receiverMock.Setup(r => r.StartHandlingMessages(It.IsAny<EventMessageReceivedCallback>()));
            receiverMock.Setup(r => r.Dispose());

            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Strict);
            contextMock.Setup(bc => bc.CreateMessageReceiver("My.Test.Queue", It.IsAny<IEnumerable<string>>()))
                       .Returns(receiverMock.Object);
            var dispatchers = new Dictionary<string, IEventDispatcher>();

            var target = new EventListener<string>(contextMock.Object, dispatchers);
            target.StartListening("My.Test.Queue");

            // Act
            target.Dispose();

            // Assert
            receiverMock.Verify(r => r.Dispose());
        }

        [TestMethod]
        public void EventListenerDisposesWithoudBeingStarted()
        {
            var receiverMock = new Mock<IMessageReceiver>(MockBehavior.Strict);
            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Strict);
            contextMock.Setup(bc => bc.CreateMessageReceiver("My.Test.Queue", It.IsAny<IEnumerable<string>>()))
                       .Returns(receiverMock.Object);
            var dispatchers = new Dictionary<string, IEventDispatcher>();

            var target = new EventListener<string>(contextMock.Object, dispatchers);

            // Act
            target.Dispose();

            // OK
        }

        [TestMethod]
        public void CannotStartListeningAfterBeingDisposed()
        {
            var receiverMock = new Mock<IMessageReceiver>(MockBehavior.Strict);
            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Strict);
            contextMock.Setup(bc => bc.CreateMessageReceiver("My.Test.Queue", It.IsAny<IEnumerable<string>>()))
                       .Returns(receiverMock.Object);
            var dispatchers = new Dictionary<string, IEventDispatcher>();

            var target = new EventListener<string>(contextMock.Object, dispatchers);

            // Act
            target.Dispose();

            Action act = () =>
            {
                target.StartListening("My.Test.Queue");
            };

            // Assert
            var ex = Assert.ThrowsException<ObjectDisposedException>(act);
            Assert.AreEqual("Cannot access a disposed object.\r\nObject name: 'EventListener'.", ex.Message);
        }


        [TestMethod]
        public async Task MessageWillNotBeReceivedAfterDispose()
        {
            var dispatcherMock = new Mock<IEventDispatcher>(MockBehavior.Strict);
            dispatcherMock.Setup(d => d.Dispatch(It.IsAny<EventMessage>()));
            var dispatchers = new Dictionary<string, IEventDispatcher>();
            dispatchers.Add("My.#", dispatcherMock.Object);

            IBusContext<MessageBroker> context = new InMemoryContext();
            var target = new EventListener<MessageBroker>(context, dispatchers);
            target.StartListening("My.Test.Queue");

            // Act
            target.Dispose();

            //Assert
            var message = new EventMessage { Topic = "My.Topic" };
            var sender = context.CreateMessageSender();
            await sender.SendMessageAsync(message);

            dispatcherMock.Verify(d => d.Dispatch(message), Times.Never);
        }


    }
}
