using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minor.Miffy.RabbitMQBus.Test
{
    [TestClass]
    public class RabbitMQBusContextBuilderTest
    {
        [TestMethod]
        public void RabbitMQBusContextBuilderHasDefaults()
        {
            var target = new RabbitMQBusContextBuilder();

            Assert.AreEqual("Miffy.DefaultEventBus", target.ExchangeName);
            Assert.AreEqual("localhost", target.HostName);
            Assert.AreEqual(5672, target.Port);
            Assert.AreEqual("guest", target.UserName);
            Assert.AreEqual("guest", target.Password);
        }

        [TestMethod]
        public void WithExchange_ChangesEchangeName()
        {
            var target = new RabbitMQBusContextBuilder();

            RabbitMQBusContextBuilder result = target.WithExchange("My.Test.Exchange");

            Assert.AreEqual("My.Test.Exchange", target.ExchangeName);
            Assert.AreEqual(target, result);
        }

        [TestMethod]
        public void WithAddress_ChangesHostNameAndPort()
        {
            var target = new RabbitMQBusContextBuilder();

            RabbitMQBusContextBuilder result = target.WithAddress("myHostName", 8128);

            Assert.AreEqual("myHostName", target.HostName);
            Assert.AreEqual(8128, target.Port);
            Assert.AreEqual(target, result);
        }

        [TestMethod]
        public void WithCredentials_ChangesUserNameAndPassword()
        {
            var target = new RabbitMQBusContextBuilder();

            RabbitMQBusContextBuilder result = target.WithCredentials("myUserName", "myPassword");

            Assert.AreEqual("myUserName", target.UserName);
            Assert.AreEqual("myPassword", target.Password);
            Assert.AreEqual(target, result);
        }

        [TestMethod]
        public void ReadFromEnvironmentVariables_ReadsExchangeName()
        {
            Environment.SetEnvironmentVariable("eventbus-exchangename", "My.Test.Exchange");
            var target = new RabbitMQBusContextBuilder()
                .WithExchange("This should be overwritten");
            Assert.AreEqual("This should be overwritten", target.ExchangeName);

            RabbitMQBusContextBuilder result = target.ReadFromEnvironmentVariables();

            Assert.AreEqual("My.Test.Exchange", target.ExchangeName);
            Assert.AreEqual("localhost", target.HostName);
            Assert.AreEqual(5672, target.Port);
            Assert.AreEqual("guest", target.UserName);
            Assert.AreEqual("guest", target.Password);
            Assert.AreEqual(target, result);

            Environment.SetEnvironmentVariable("eventbus-exchangename", null);
        }

        [TestMethod]
        public void ReadFromEnvironmentVariables_ReadsOtherVariables()
        {
            Environment.SetEnvironmentVariable("eventbus-hostname", "My.host");
            Environment.SetEnvironmentVariable("eventbus-port", "8128");
            Environment.SetEnvironmentVariable("eventbus-username", "My.username");
            Environment.SetEnvironmentVariable("eventbus-password", "My.password");

            var target = new RabbitMQBusContextBuilder();

            RabbitMQBusContextBuilder result = target.ReadFromEnvironmentVariables();

            Assert.AreEqual("Miffy.DefaultEventBus", target.ExchangeName);
            Assert.AreEqual("My.host", target.HostName);
            Assert.AreEqual(8128, target.Port);
            Assert.AreEqual("My.username", target.UserName);
            Assert.AreEqual("My.password", target.Password);
            Assert.AreEqual(target, result);

            Environment.SetEnvironmentVariable("eventbus-hostname", null);
            Environment.SetEnvironmentVariable("eventbus-port", null);
            Environment.SetEnvironmentVariable("eventbus-username", null);
            Environment.SetEnvironmentVariable("eventbus-password", null);
        }
    }
}
