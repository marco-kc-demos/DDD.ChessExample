using DDD.Core;

namespace ChessApi.Domain.DomainEvents
{
    public class GameStarted : DomainEvent
    {
        public long GameId { get; }

        public GameStarted(long gameId)
        {
            GameId = gameId;
        }
    }
}
