using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minor.Miffy.RabbitMQBus
{
    public class RabbitMQBusContextBuilder
    {
        /// <summary>
        /// Default: "Miffy.DefaultEventBus"
        /// </summary>
        public string ExchangeName { get; private set; }
        /// <summary>
        /// Default HostName: "localhost"
        /// </summary>
        public string HostName { get; private set; }
        /// <summary>
        /// Default Port: 5672
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// Default UserName: "guest"
        /// </summary>
        public string UserName { get; private set; }
        /// <summary>
        /// Default Password: "guest"
        /// </summary>
        public string Password { get; private set; }

        public RabbitMQBusContextBuilder()
        {
            ExchangeName = "Miffy.DefaultEventBus";
            HostName = "localhost";
            Port = 5672;
            UserName = "guest";
            Password = "guest";
        }

        public RabbitMQBusContextBuilder WithExchange(string exchangeName)
        {
            ExchangeName = exchangeName;
            return this;    // for method chaining
        }

        public RabbitMQBusContextBuilder WithAddress(string hostName, int port)
        {
            HostName = hostName;
            Port = port;
            return this;    // for method chaining
        }

        public RabbitMQBusContextBuilder WithCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
            return this;    // for method chaining
        }

        public RabbitMQBusContextBuilder ReadFromEnvironmentVariables()
        {
            ExchangeName = Environment.GetEnvironmentVariable("eventbus-exchangename") ?? ExchangeName;
            HostName = Environment.GetEnvironmentVariable("eventbus-hostname") ?? HostName;
            if (int.TryParse(Environment.GetEnvironmentVariable("eventbus-port"), out int port))
            {
                Port = port;
            }
            UserName = Environment.GetEnvironmentVariable("eventbus-username") ?? UserName;
            Password = Environment.GetEnvironmentVariable("eventbus-password") ?? Password;
            return this;    // for method chaining
        }

        /// <summary>
        /// Creates a context with 
        ///  - an opened connection (based on HostName, Port, UserName and Password)
        ///  - a declared Topic-Exchange (based on ExchangeName)
        /// </summary>
        /// <returns></returns>
        public RabbitMQBusContext CreateContext()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = HostName,
                Port = Port,
                UserName = UserName,
                Password = Password,
            };
            IConnection connection = factory.CreateConnection();

            return new RabbitMQBusContext(connection, ExchangeName);
        }
    }

}
