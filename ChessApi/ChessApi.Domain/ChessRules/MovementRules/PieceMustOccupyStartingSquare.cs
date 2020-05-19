using ChessApi.Domain.Commands;
using ChessApi.Domain.Entities;
using ChessApi.Domain.ValueObjects;
using DDD.Core.BusinessRules;
using System.Collections.Generic;

namespace ChessApi.Domain.ChessRules
{
    public class PieceMustOccupyStartingSquare : BusinessRule
    {
        private readonly Board board;
        private readonly Move move;

        public PieceMustOccupyStartingSquare(Board board, Move move)
        {
            this.board = board;
            this.move = move;
        }

        public override IEnumerable<BusinessRuleViolation> CheckRule()
        {
            if (board.IsOccupiedAt(move.StartSquare) &&
                !board.HasThisPieceOn(move.StartSquare, move.PieceCode))
            {
                yield return new BusinessRuleViolation($"There is no {move.PieceCode.GetName()} on {move.StartSquare}.");
            }
        }
    }
}