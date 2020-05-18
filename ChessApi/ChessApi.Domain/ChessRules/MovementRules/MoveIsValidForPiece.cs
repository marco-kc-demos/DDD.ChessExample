using ChessApi.Domain.Entities;
using ChessApi.Domain.ValueObjects;
using DDD.Core.BusinessRules;
using System.Collections.Generic;

namespace ChessApi.Domain.ChessRules
{
    public class MoveIsValidForPiece : BusinessRule
    {
        private readonly Board _board;
        private readonly Move _move;

        public MoveIsValidForPiece(Board board, Move move)
        {
            _board = board;
            _move = move;
        }

        public override IEnumerable<BusinessRuleViolation> CheckRule()
        {
            if (_board.IsEmpty(_move.StartSquare))
            {
                yield return new BusinessRuleViolation($"There is no piece on {_move.StartSquare}.");
            }
            else
            {
                Piece piece = _board.GetPieceOn(_move.StartSquare);

                if (!piece.IsValidMove(_move))
                {
                    yield return new BusinessRuleViolation($"A {piece} cannot move from {_move.StartSquare} to {_move.DestinationSquare}.");
                }
                yield break;
            }
        }
    }
}