using ChessApi.Application.Repositories;
using ChessApi.Domain.Aggregates;
using ChessApi.Domain.Commands;
using ChessApi.Domain.DomainEvents;
using ChessApi.Domain.IncomingDomainEvents;
using DDD.Core.Application;
using System.Threading.Tasks;

namespace ChessApi.Application.EventHandlers
{
    public class GameFormedEventHandler : IEventHandler<GameFormed>
    {
        private readonly IGameRepository _gameRepo;
        private readonly IEventPublisher _eventPublisher;

        public GameFormedEventHandler(IGameRepository gameRepo, IEventPublisher eventPublisher)
        {
            _gameRepo = gameRepo;
            _eventPublisher = eventPublisher;
        }

        public void HandleEvent(GameFormed domainEvent)
        {
            Game game = _gameRepo.FindAsync(domainEvent.GameId).Result;

            if (game == null)
            {
                game = new Game(domainEvent.GameId);

                var command = new StartGame(domainEvent.GameId);
                game.StartGame(command);

                _gameRepo.SaveAsync(game).Wait();

                _eventPublisher.PublishEventsAsync(game.Events).Wait();
            }
        }
    }
}