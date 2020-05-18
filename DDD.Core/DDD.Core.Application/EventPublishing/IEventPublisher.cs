using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDD.Core.Application
{
    public interface IEventPublisher
    {
        Task PublishEventAsync(DomainEvent domainEvent);
        Task PublishEventsAsync(IEnumerable<DomainEvent> domainEvents);
    }
}