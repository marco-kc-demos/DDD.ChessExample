using ChessApi.Domain.DomainEvents;
using ChessApi.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.Mappers
{
    public static class EventMapper
    {
        public static MoveMade MapToMoveMade(this Move move, long gameId)
        {
            return new MoveMade
            {
                GameId = gameId,
                PieceCode = move.PieceCode.ToChar(),
                StartSquare = move.StartSquare.Name,
                DestinationSquare = move.DestinationSquare.Name,
            };
        }

        public static Move MapToMove(this MoveMade moveMade)
        {
            return new Move
            (
                PieceCodes.FromChar(moveMade.PieceCode),
                new StartSquare(moveMade.StartSquare),
                new DestinationSquare(moveMade.DestinationSquare)
            );
        }
    }
}
