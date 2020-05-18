namespace DDD.Core.Application
{
    public interface IEventListenerBuilder
    {
        void AddEventHandler<TEvent, THandler>(string topicFilter) 
            where THandler : IEventHandler<TEvent>;
    }
}