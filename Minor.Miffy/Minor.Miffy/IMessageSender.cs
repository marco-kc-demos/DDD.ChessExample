using System;
using System.Threading.Tasks;

namespace Minor.Miffy
{
    public interface IMessageSender : IDisposable
    {
        Task SendMessageAsync(EventMessage message);
    }
}
