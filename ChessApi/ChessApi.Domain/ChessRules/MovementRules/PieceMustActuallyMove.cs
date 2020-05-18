using ChessApi.Domain.Commands;
using ChessApi.Domain.ValueObjects;
using DDD.Core.BusinessRules;
using System.Collections.Generic;

namespace ChessApi.Domain.ChessRules
{
    public class PieceMustActuallyMove : BusinessRule
    {
        private readonly Move _move;

        public PieceMustActuallyMove(Move move)
        {
            _move = move;
        }

        public override IEnumerable<BusinessRuleViolation> CheckRule()
        {
            if (_move.StartSquare == _move.DestinationSquare)
            {
                yield return new BusinessRuleViolation("The start square cannot be the same as the destination square.");
            }
        }
    }
}