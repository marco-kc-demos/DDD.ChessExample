using ChessApi.Domain.Commands;
using ChessApi.Domain.ValueObjects;
using DDD.Core.BusinessRules;
using System.Collections.Generic;

namespace ChessApi.Domain.ChessRules
{
    public class PieceMustActuallyMove : BusinessRule
    {
        private readonly Move move;

        public PieceMustActuallyMove(Move move)
        {
            this.move = move;
        }

        public override IEnumerable<BusinessRuleViolation> CheckRule()
        {
            if (move.StartSquare == move.DestinationSquare)
            {
                yield return new BusinessRuleViolation("The start square cannot be the same as the destination square.");
            }
        }
    }
}