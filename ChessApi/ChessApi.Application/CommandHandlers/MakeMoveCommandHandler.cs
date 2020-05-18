using ChessApi.Application.Repositories;
using ChessApi.Domain.Aggregates;
using ChessApi.Domain.Commands;
using DDD.Core;
using DDD.Core.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChessApi.Application.CommandHandlers
{
    public class MakeMoveCommandHandler : IMakeMoveCommandHandler
    {
        private readonly IGameRepository _gameRepo;
        private readonly IEventPublisher _eventPublisher;

        public MakeMoveCommandHandler(IGameRepository gameRepo, IEventPublisher eventPublisher)
        {
            _gameRepo = gameRepo;
            _eventPublisher = eventPublisher;
        }

        public async Task HandleCommandAsync(MakeMove command)
        {
            // restore game
            Game game = await _gameRepo.FindAsync(command.GameId);
            if (game == null)
            {
                throw new GameNotFoundException($"A game with id {command.GameId} could not be found.");
            }

            // handle command
            game.MakeMove(command);

            // persist game
            await _gameRepo.SaveAsync(game);

            // publish events
            await _eventPublisher.PublishEventsAsync(game.Events);
        }
    }
}
