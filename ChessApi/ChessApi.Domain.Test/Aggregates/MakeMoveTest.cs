using ChessApi.Domain.Aggregates;
using ChessApi.Domain.Commands;
using ChessApi.Domain.DomainEvents;
using ChessApi.Domain.Entities;
using ChessApi.Domain.ValueObjects;
using DDD.Core;
using DDD.Core.BusinessRules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessApi.Domain.Test.Aggregates
{
    [TestClass]
    public class MakeMoveTest
    {
        [TestMethod]
        public void MakeMoveMovesPieceOnBoard()
        {
            // Given
            Piece rook = new Rook(Colour.White);
            StartSquare a1 = new StartSquare("a1");
            DestinationSquare a4 = new DestinationSquare("a4");

            Game target = new Game(1);
            target.Board.PutPieceOn(a1, rook);

            // When
            MakeMove command = new MakeMove(1, new Move(rook.Code, a1, a4));
            target.MakeMove(command);

            // Then
            Assert.IsTrue(target.Board.IsEmpty(a1), "rook should not be on a1");
            Assert.IsTrue(target.Board.IsOccupiedByPiece(a4, rook.Code), "rook should be on a4");
        }

        [TestMethod]
        public void MakeMoveRaisesEvent()
        {
            // Given
            Piece rook = new Rook(Colour.White);
            StartSquare a1 = new StartSquare("a1");
            DestinationSquare a4 = new DestinationSquare("a4");

            long gameId = 101;
            Game target = new Game(gameId);
            target.Board.PutPieceOn(a1, rook);

            // When
            MakeMove command = new MakeMove(gameId, new Move(rook.Code, a1, a4));
            target.MakeMove(command);

            // Then
            DomainEvent result = target.Events.Single();
            Assert.IsInstanceOfType(result, typeof(MoveMade));
            MoveMade movemade = (MoveMade)result;
            Assert.AreEqual(gameId, movemade.GameId);
            Assert.AreEqual('R', movemade.PieceCode);
            Assert.AreEqual("a1", movemade.StartSquare);
            Assert.AreEqual("a4", movemade.DestinationSquare);
        }

        [TestMethod]
        public void PieceMoustActuallyMove()
        {
            // Given
            Piece rook = new Rook(Colour.White);
            StartSquare a1 = new StartSquare("a1");
            DestinationSquare a1AsWell = new DestinationSquare("a1");

            long gameId = 101;
            Game target = new Game(gameId);
            target.Board.PutPieceOn(a1, rook);

            // When
            MakeMove command = new MakeMove(gameId, new Move(rook.Code, a1, a1AsWell));
            Action act = () =>
            {
                target.MakeMove(command);
            };

            // Then
            var ex = Assert.ThrowsException<BusinessRuleViolationException>(act);
            Assert.AreEqual("Rule Violations: 1 violations have been detected.", ex.Message);
            Assert.AreEqual(1, ex.Violations.Count());
            Assert.IsTrue(ex.Violations.Any(v => v.ViolationMessage== "The start square cannot be the same as the destination square."));
        }

    }
}
