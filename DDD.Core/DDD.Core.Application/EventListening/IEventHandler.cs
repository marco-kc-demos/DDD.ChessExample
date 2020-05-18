namespace DDD.Core.Application
{
    public interface IEventHandler<TEvent>
    {
        void HandleEvent(TEvent domainEvent);
    }
}