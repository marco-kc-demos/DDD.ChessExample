using ChessApi.Domain.ChessRules;
using ChessApi.Domain.Entities;
using ChessApi.Domain.ValueObjects;
using DDD.Core.BusinessRules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessApi.Domain.Test.ChessRules.MovementRules
{
    [TestClass]
    public class PieceMustActuallyMoveTest
    {
        [TestMethod]
        public void PieceMustAcuallyMove()
        {
            Piece rook = new Rook(Colour.Black);
            StartSquare d5 = new StartSquare("d5");
            DestinationSquare d8 = new DestinationSquare("d8");
            Move move = new Move(rook.Code, d5, d8);

            BusinessRule target = new PieceMustActuallyMove(move);

            IEnumerable<BusinessRuleViolation> violations = target.CheckRule();

            Assert.IsNotNull(violations);
            Assert.IsFalse(violations.Any());
        }

        [TestMethod]
        public void PieceMustAcuallyMove_ButDoesNot()
        {
            Piece rook = new Rook(Colour.Black);
            StartSquare d5 = new StartSquare("d5");
            DestinationSquare d5AsWell = new DestinationSquare("d5");
            Move move = new Move(rook.Code, d5, d5AsWell);

            BusinessRule target = new PieceMustActuallyMove(move);

            IEnumerable<BusinessRuleViolation> violations = target.CheckRule();

            Assert.AreEqual(1, violations.Count());
            Assert.IsTrue(violations.Any(v => v.ViolationMessage == 
                "The start square cannot be the same as the destination square."));
        }
    }
}
