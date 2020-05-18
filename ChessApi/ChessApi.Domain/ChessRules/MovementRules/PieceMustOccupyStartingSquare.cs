using ChessApi.Domain.Commands;
using ChessApi.Domain.Entities;
using ChessApi.Domain.ValueObjects;
using DDD.Core.BusinessRules;
using System.Collections.Generic;

namespace ChessApi.Domain.ChessRules
{
    public class PieceMustOccupyStartingSquare : BusinessRule
    {
        private readonly Board _board;
        private readonly Move _move;

        public PieceMustOccupyStartingSquare(Board board, Move move)
        {
            _board = board;
            _move = move;
        }

        public override IEnumerable<BusinessRuleViolation> CheckRule()
        {
            if (_board.IsOccupied(_move.StartSquare) &&
                !_board.IsOccupiedByPiece(_move.StartSquare, _move.PieceCode))
            {
                yield return new BusinessRuleViolation($"There is no {_move.PieceCode.GetName()} on {_move.StartSquare}.");
            }
        }
    }
}