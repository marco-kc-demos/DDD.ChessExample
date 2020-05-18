using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Minor.Miffy.InMemoryBus.Test
{
    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]
        public async Task MyTestMethod()
        {
            InMemoryContext context = new InMemoryContext();

            // create sender and receivers
            var sender = context.CreateMessageSender();

            var receiver1 = context.CreateMessageReceiver("Queue1", 
                                                          new List<string> { "My1.Filter", "MVM.#" });
            var receiver2 = context.CreateMessageReceiver("Queue2", 
                                                          new List<string> { "My2.Filter", "MVM.#" });
            var receiver3 = context.CreateMessageReceiver("Queue3", 
                                                          new List<string> { "My3.Filter", "MVM.#" });
            // for capturing the results:
            var reveivedMessages1 = new List<EventMessage>();
            var reveivedMessages2 = new List<EventMessage>();
            var reveivedMessages3 = new List<EventMessage>();

            // Act I
            receiver1.StartReceivingMessages();
            receiver1.StartHandlingMessages(em => { reveivedMessages1.Add(em); });
            receiver2.StartReceivingMessages();

            // Act II
            var message1 = new EventMessage { Topic = "MVM.SomeTopic1" };
            await sender.SendMessageAsync(message1);
            // will be reveived by receiver1 and receiver2

            // Act III
            receiver2.StartHandlingMessages(em => { reveivedMessages2.Add(em); });
            receiver3.StartReceivingMessages();
            receiver3.StartHandlingMessages(em => { reveivedMessages3.Add(em); });

            // Act IV
            var message2 = new EventMessage { Topic = "MVM.SomeTopic2" };
            await sender.SendMessageAsync(message2);
            // will be reveived by all three receivers

            // Act V
            var message3 = new EventMessage { Topic = "My2.Filter" };
            await sender.SendMessageAsync(message3);
            // Will only be reveived by the receiver2

            // Assert
            CollectionAssert.AreEqual(reveivedMessages1, new List<EventMessage> { message1, message2 });
            CollectionAssert.AreEqual(reveivedMessages2, new List<EventMessage> { message1, message2, message3 });
            CollectionAssert.AreEqual(reveivedMessages3, new List<EventMessage> { message2 });
        }
    }
}
