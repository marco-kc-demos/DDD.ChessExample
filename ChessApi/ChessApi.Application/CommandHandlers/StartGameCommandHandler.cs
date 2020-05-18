using ChessApi.Application.Repositories;
using ChessApi.Domain.Aggregates;
using ChessApi.Domain.Commands;
using DDD.Core.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChessApi.Application.CommandHandlers
{
    public class StartGameCommandHandler : IStartGameCommandHandler
    {
        private readonly IGameRepository _gameRepo;
        private readonly IEventPublisher _eventPublisher;

        public StartGameCommandHandler(IGameRepository gameRepo, IEventPublisher eventPublisher)
        {
            _gameRepo = gameRepo;
            _eventPublisher = eventPublisher;
        }

        public async Task HandleCommandAsync(StartGame command)
        {
            // restore game
            Game game = await _gameRepo.FindAsync(command.GameId);

            if (game == null)   // if the game has not already been started
            {
                game = new Game(command.GameId);

                game.StartGame(command);

                await _gameRepo.SaveAsync(game);

                await _eventPublisher.PublishEventsAsync(game.Events);
            }
        }
    }
}
