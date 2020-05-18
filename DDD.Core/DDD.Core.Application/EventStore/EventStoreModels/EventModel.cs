
namespace DDD.Core.Application.EventStoreModels
{
    public class EventModel
    {
        public long Id { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
    }
}
