using System;
using System.Threading.Tasks;

namespace Minor.Miffy.InMemoryBus
{
    public class InMemoryMessageSender : IMessageSender
    {
        private readonly MessageBroker _messageBroker;
        private bool _isDisposed;

        public InMemoryMessageSender(InMemoryContext inMemoryContext)
        {
            _messageBroker = inMemoryContext.Connection;
            _isDisposed = false;
        }

        public async Task SendMessageAsync(EventMessage message)
        {
            if (!_isDisposed)
            {
                await _messageBroker.BasicPublishAsync(message);
            }
            else
            {
                throw new ObjectDisposedException(nameof(InMemoryMessageSender));
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}