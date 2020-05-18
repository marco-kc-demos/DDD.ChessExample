using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Miffy;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Core.Application.Test.EventPublishing
{
    [TestClass]
    public class EventPublisherTest
    {
        public static Task EmptyTask;

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            EmptyTask = Task.Run(() => { });
        }

        [TestMethod]
        public async Task EventIsSendToMessageSender()
        {
            var senderMock = new Mock<IMessageSender>(MockBehavior.Strict);
            senderMock.Setup(r => r.SendMessageAsync(It.IsAny<EventMessage>()))
                      .Returns(EmptyTask);
            senderMock.Setup(r => r.Dispose());
            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Strict);
            contextMock.Setup(bc => bc.CreateMessageSender())
                       .Returns(senderMock.Object);
            var target = new EventPublisher<string>(contextMock.Object);

            var evt = new SomeEvent { SomeNumber = 5 };
            await target.PublishEventAsync(evt);

            senderMock.Verify(r => r.SendMessageAsync(It.Is<EventMessage>(em =>
                    em.Topic == "DDD.Core.Application.Test.SomeEvent" &&
                    em.EventType == "SomeEvent" &&
                    //Encoding.Unicode.GetString(em.Body) == "{\r\n  \"SomeNumber\": 5\r\n}"
                    Encoding.Unicode.GetString(em.Body).Contains("\"SomeNumber\"") &&
                    Encoding.Unicode.GetString(em.Body).Contains("5")
            )));
        }

        [TestMethod]
        public async Task InternalDomainEventIsNotSendToMessageSender()
        {
            var senderMock = new Mock<IMessageSender>(MockBehavior.Strict);
            senderMock.Setup(r => r.SendMessageAsync(It.IsAny<EventMessage>()))
                      .Returns(EmptyTask);
            senderMock.Setup(r => r.Dispose());
            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Strict);
            contextMock.Setup(bc => bc.CreateMessageSender())
                       .Returns(senderMock.Object);
            var target = new EventPublisher<string>(contextMock.Object);

            var evt = new SomeInternalEvent { SomeNumber = 42 };
            await target.PublishEventAsync(evt);

            senderMock.Verify(r => r.SendMessageAsync(It.IsAny<EventMessage>()), Times.Never);
        }

        [TestMethod]
        public async Task PublishEvents_CallsSendMesageMultipleTimes()
        {
            var senderMock = new Mock<IMessageSender>(MockBehavior.Strict);
            senderMock.Setup(r => r.SendMessageAsync(It.IsAny<EventMessage>()))
                      .Returns(EmptyTask);
            senderMock.Setup(r => r.Dispose());
            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Strict);
            contextMock.Setup(bc => bc.CreateMessageSender())
                       .Returns(senderMock.Object);
            var target = new EventPublisher<string>(contextMock.Object);

            var events = new List<DomainEvent>
            {
                new SomeEvent { SomeNumber = 1 },
                new SomeOtherEvent { SomeNumber = 2 },
                new SomeEvent { SomeNumber = 3 },
            };
            await target.PublishEventsAsync(events);

            senderMock.Verify(r => r.SendMessageAsync(It.IsAny<EventMessage>()), Times.Exactly(3));
        }

        [TestMethod]
        public async Task PublishEvents_CallsSendNoMesageForInternalDomainEvents()
        {
            var senderMock = new Mock<IMessageSender>(MockBehavior.Strict);
            senderMock.Setup(r => r.SendMessageAsync(It.IsAny<EventMessage>()))
                      .Returns(EmptyTask);
            senderMock.Setup(r => r.Dispose());
            var contextMock = new Mock<IBusContext<string>>(MockBehavior.Strict);
            contextMock.Setup(bc => bc.CreateMessageSender())
                       .Returns(senderMock.Object);
            var target = new EventPublisher<string>(contextMock.Object);

            var events = new List<DomainEvent>
            {
                new SomeInternalEvent { SomeNumber = 1 },
                new SomeOtherEvent { SomeNumber = 2 },
                new SomeEvent { SomeNumber = 3 },
            };
            await target.PublishEventsAsync(events);

            senderMock.Verify(r => r.SendMessageAsync(It.IsAny<EventMessage>()), Times.Exactly(2));

        }
    }
}
