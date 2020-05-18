using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minor.Miffy.InMemoryBus.Test
{
    [TestClass]
    public class InMemoryMessageSenderTest
    {
        [TestMethod]
        public async Task SendMessageAsync_SendsMessage()
        {
            var context = new InMemoryContext();
            var sender = new InMemoryMessageSender(context);
            
            var message = new EventMessage { Topic = "SomeTopic" };
            await sender.SendMessageAsync(message);

            Assert.IsTrue(context.Connection.LoggedMessages.Any(m => m.Topic == "SomeTopic"));
        }

        [TestMethod]
        public async Task IfDisposedCannotSendMessagesAnymore()
        {
            var context = new InMemoryContext();
            var sender = new InMemoryMessageSender(context);
            sender.Dispose();

            var message = new EventMessage { Topic = "SomeTopic" };
            Func<Task> act = () =>
            {
                return sender.SendMessageAsync(message);
            };

            var ex = await Assert.ThrowsExceptionAsync<ObjectDisposedException>(act);
            Assert.AreEqual("Cannot access a disposed object.\r\nObject name: 'InMemoryMessageSender'.", ex.Message);
        }
    }
}
