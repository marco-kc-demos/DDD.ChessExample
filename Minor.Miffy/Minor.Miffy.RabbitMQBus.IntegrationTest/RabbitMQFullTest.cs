using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Minor.Miffy.RabbitMQBus.IntegrationTest
{
    [TestClass]
    public class RabbitMQFullTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            try
            {
                var builder = new RabbitMQBusContextBuilder()
                    .WithExchange("MVM.Eventbus")
                    .WithAddress("localhost", 5672)
                    .WithCredentials("guest", "guest");

                using (IBusContext<IConnection> busContext = builder.CreateContext())
                {
                }
            }
            catch
            {
                Process.Start("docker", "run --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management");
                Thread.Sleep(10000);
            }
        }

        [TestMethod]
        public void SendAndReceiveTest()
        {
            var sentMessage = new EventMessage
            {
                CorrelationId = Guid.NewGuid(),
                EventType = "string",
                Timestamp = 100000,
                Topic = "MVM.Klantbeheer.KlantGeregistreerd",
                Body = Encoding.Unicode.GetBytes("Klant is geregistreerd"),
            };

            var receivedFlag = new AutoResetEvent(false);
            EventMessage receivedMessage = null;

            var builder = new RabbitMQBusContextBuilder()
                .WithExchange("MVM.Eventbus")
                .WithAddress("localhost", 5672)
                .WithCredentials("guest","guest");

            using (IBusContext<IConnection> busContext = builder.CreateContext())
            // Act - receiver
            using (IMessageReceiver receiver = busContext.CreateMessageReceiver(
                    queueName: "MVM.PolisService",
                    topicFilters: new string[] { "MVM.Klantbeheer.KlantGeregistreerd" }))
            {
                receiver.StartReceivingMessages();
                receiver.StartHandlingMessages((EventMessage message) =>
                {
                    receivedMessage = message;
                    receivedFlag.Set();
                });

                // Act - sender
                var sender = busContext.CreateMessageSender();
                sender.SendMessageAsync(sentMessage).Wait();

                // Assert
                bool messageHasBeenReveived = receivedFlag.WaitOne(2000);
                Assert.IsTrue(messageHasBeenReveived);
                Assert.AreEqual(sentMessage.CorrelationId, receivedMessage.CorrelationId);
                Assert.AreEqual(sentMessage.EventType, receivedMessage.EventType);
                Assert.AreEqual(sentMessage.Timestamp, receivedMessage.Timestamp);
                Assert.AreEqual(sentMessage.Topic, receivedMessage.Topic);
                CollectionAssert.AreEqual(sentMessage.Body, receivedMessage.Body);
            }
        }
    }
}
