using System;

namespace DDD.Core.Application
{
    public interface IEventListener : IDisposable
    {
        void StartListening(string queueName);
    }
}