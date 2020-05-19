using ChessApi.Domain.Entities;
using ChessApi.Domain.ValueObjects;
using DDD.Core.BusinessRules;
using System.Collections.Generic;

namespace ChessApi.Domain.ChessRules
{
    public class MoveIsValidForPiece : BusinessRule
    {
        private readonly Board board;
        private readonly Move move;

        public MoveIsValidForPiece(Board board, Move move)
        {
            this.board = board;
            this.move = move;
        }

        public override IEnumerable<BusinessRuleViolation> CheckRule()
        {
            if (board.IsEmptyAt(move.StartSquare))
            {
                yield return new BusinessRuleViolation($"There is no piece on {move.StartSquare}.");
            }
            else
            {
                Piece piece = board.GetPieceOn(move.StartSquare);

                if (!piece.IsValidMove(move))
                {
                    yield return new BusinessRuleViolation($"A {piece} cannot move from {move.StartSquare} to {move.DestinationSquare}.");
                }
                yield break;
            }
        }
    }
}