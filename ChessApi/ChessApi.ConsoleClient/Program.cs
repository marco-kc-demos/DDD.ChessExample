using ChessApi.Domain.IncomingDomainEvents;
using DDD.Core.Application;
using Minor.Miffy;
using Minor.Miffy.RabbitMQBus;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChessApi.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var busContext = new RabbitMQBusContextBuilder()
                .WithExchange("Chess.Exchange")
                .CreateContext();

            var publisher = new EventPublisher<IConnection>(busContext);

            var topicfilters = new List<string> { "#" };
            using var receiver = busContext.CreateMessageReceiver("ChessApi.ConsoleClient", topicfilters);
            receiver.StartReceivingMessages();
            receiver.StartHandlingMessages(MessageReceived);

            Console.WriteLine($"Listening to all messages on exchange '{busContext.ExchangeName}'...");

            while (true)
            {
                Console.WriteLine("Send a GameFormed-event by entering the GameId of a new game.");
                Console.Write("GameId:");
                if (int.TryParse(Console.ReadLine(), out int gameId))
                {
                    var gameFormed = new GameFormed { GameId = gameId };
                    await publisher.PublishEventAsync(gameFormed);
                }
            }
        }

        public static void MessageReceived(EventMessage message)
        {
            Console.WriteLine($"Topic: {message.Topic}");
            Console.WriteLine($"Body: {Encoding.Unicode.GetString(message.Body)}");
            Console.WriteLine();
        }
    }
}
