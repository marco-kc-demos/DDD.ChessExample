using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Miffy;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Application.Test
{
    [TestClass]
    public class EventDispatcherTest
    {
        [TestMethod]
        public void DispatcherInjectsAndCallsHandler()
        {
            var services = new ServiceCollection();
            var injectable = new SomethingToInject();
            services.AddSingleton<ISomethingToInject>(injectable);

            var target = new EventDispatcher<MyEvent, MyHandler>(services.BuildServiceProvider());

            var message = new EventMessage
            {
                Body = Encoding.Unicode.GetBytes("{\"SomeNumber\":5}"),
            };
            target.Dispatch(message);

            Assert.IsNotNull(injectable.EventData);
            Assert.AreEqual(5, injectable.EventData.SomeNumber);
        }

        [TestMethod]
        public void WhenThereInNothingToInject()
        {
            var services = new ServiceCollection();
            var target = new EventDispatcher<MyEvent, MyHandler>(services.BuildServiceProvider());

            var message = new EventMessage
            {
                Body = Encoding.Unicode.GetBytes("{\"SomeNumber\":5}"),
            };

            Action act = () =>
            {
                target.Dispatch(message);
            };

            var ex = Assert.ThrowsException<InvalidOperationException>(act);
            Assert.AreEqual("Unable to resolve service for type 'DDD.Core.Application.Test.ISomethingToInject' " +
                            "while attempting to activate 'DDD.Core.Application.Test.MyHandler'.", ex.Message);
        }
    }

    internal class MyHandler : IEventHandler<MyEvent>
    {
        private readonly ISomethingToInject _injectable;

        public MyHandler(ISomethingToInject injectable)
        {
            _injectable = injectable;
        }

        public void HandleEvent(MyEvent domainEvent)
        {
            _injectable.EventData = domainEvent;
        }
    }

    internal interface ISomethingToInject
    {
        public MyEvent EventData { get; set; }
    }
    internal class SomethingToInject : ISomethingToInject
    {
        public MyEvent EventData { get; set; }
    }

    internal class MyEvent : DomainEvent
    {
        public int SomeNumber { get; set; }
    }
}
