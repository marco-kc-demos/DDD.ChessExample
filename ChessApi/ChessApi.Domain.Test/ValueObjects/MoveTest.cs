using ChessApi.Domain.Entities;
using ChessApi.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.Test.Entities
{
    [TestClass]
    public class MoveTest
    {
        [TestMethod]
        public void AMoveHasAPieceStartAndDestination()
        {
            StartSquare e1 = new StartSquare("e1");
            DestinationSquare d2 = new DestinationSquare("d2");

            Move move = new Move(PieceCode.K, e1, d2);

            Assert.AreEqual(PieceCode.K, move.PieceCode);
            Assert.AreEqual(e1, move.StartSquare);
            Assert.AreEqual(d2, move.DestinationSquare);
        }

        [TestMethod]
        public void IsHorizontalMove()
        {
            StartSquare b2 = new StartSquare("b2");
            DestinationSquare d2 = new DestinationSquare("d2");
            Move move = new Move(PieceCode.R, b2, d2);

            bool result = move.IsHorizontalMove();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void IsNoHorizontalMove()
        {
            StartSquare b2 = new StartSquare("b2");
            DestinationSquare d3 = new DestinationSquare("d3");
            Move move = new Move(PieceCode.R, b2, d3);

            bool result = move.IsHorizontalMove();

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void IsVerticalMove()
        {
            StartSquare b2 = new StartSquare("b2");
            DestinationSquare b7 = new DestinationSquare("b7");
            Move move = new Move(PieceCode.R, b2, b7);

            bool result = move.IsVerticalMove();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void IsNOVerticalMove()
        {
            StartSquare b2 = new StartSquare("b2");
            DestinationSquare d3 = new DestinationSquare("d3");
            Move move = new Move(PieceCode.R, b2, d3);

            bool result = move.IsVerticalMove();

            Assert.AreEqual(false, result);
        }


        [TestMethod]
        public void IsOneStepInAnyDirection()
        {
            StartSquare b2 = new StartSquare("b2");
            
            Move movea3 = new Move(PieceCode.K, b2, new DestinationSquare("a3"));
            Move moveb3 = new Move(PieceCode.K, b2, new DestinationSquare("b3"));
            Move movec3 = new Move(PieceCode.K, b2, new DestinationSquare("c3"));
            Move movea2 = new Move(PieceCode.K, b2, new DestinationSquare("a2"));
            Move movec2 = new Move(PieceCode.K, b2, new DestinationSquare("c2"));
            Move movea1 = new Move(PieceCode.K, b2, new DestinationSquare("a1"));
            Move moveb1 = new Move(PieceCode.K, b2, new DestinationSquare("b1"));
            Move movec1 = new Move(PieceCode.K, b2, new DestinationSquare("c1"));

            Assert.AreEqual(true, movea3.IsOneStepInAnyDirection(), "a3 is one step");
            Assert.AreEqual(true, moveb3.IsOneStepInAnyDirection(), "b3 is one step");
            Assert.AreEqual(true, movec3.IsOneStepInAnyDirection(), "c3 is one step");
            Assert.AreEqual(true, movea2.IsOneStepInAnyDirection(), "a2 is one step");
            Assert.AreEqual(true, movec2.IsOneStepInAnyDirection(), "c2 is one step");
            Assert.AreEqual(true, movea1.IsOneStepInAnyDirection(), "a1 is one step");
            Assert.AreEqual(true, moveb1.IsOneStepInAnyDirection(), "b1 is one step");
            Assert.AreEqual(true, movec1.IsOneStepInAnyDirection(), "c1 is one step");
        }

        [TestMethod]
        public void IsNotOneStepInAnyDirection()
        {
            StartSquare b2 = new StartSquare("b2");

            Move movea5 = new Move(PieceCode.K, b2, new DestinationSquare("a5"));
            Move moveb4 = new Move(PieceCode.K, b2, new DestinationSquare("b4"));
            Move moved4 = new Move(PieceCode.K, b2, new DestinationSquare("d4"));
            Move movee2 = new Move(PieceCode.K, b2, new DestinationSquare("e2"));
            Move moveh8 = new Move(PieceCode.K, b2, new DestinationSquare("h8"));

            Assert.AreEqual(false, movea5.IsOneStepInAnyDirection(), "a5 is not one step");
            Assert.AreEqual(false, moveb4.IsOneStepInAnyDirection(), "b4 is not one step");
            Assert.AreEqual(false, moved4.IsOneStepInAnyDirection(), "d4 is not one step");
            Assert.AreEqual(false, movee2.IsOneStepInAnyDirection(), "e2 is not one step");
            Assert.AreEqual(false, moveh8.IsOneStepInAnyDirection(), "h8 is not one step");
        }


        [TestMethod]
        public void IsDiagonalMove()
        {
            StartSquare d5 = new StartSquare("d5");

            Move movea2 = new Move(PieceCode.B, d5, new DestinationSquare("a2"));
            Move movea8 = new Move(PieceCode.B, d5, new DestinationSquare("a8"));
            Move moveg8 = new Move(PieceCode.B, d5, new DestinationSquare("g8"));
            Move moveh1 = new Move(PieceCode.B, d5, new DestinationSquare("h1"));
            Move movef3 = new Move(PieceCode.B, d5, new DestinationSquare("f3"));
            Move movef7 = new Move(PieceCode.B, d5, new DestinationSquare("f7"));

            Assert.AreEqual(true, movea2.IsDiagonalMove(), "d5-a2 is a diagonal move");
            Assert.AreEqual(true, movea8.IsDiagonalMove(), "d5-a8 is a diagonal move");
            Assert.AreEqual(true, moveg8.IsDiagonalMove(), "d5-g8 is a diagonal move");
            Assert.AreEqual(true, moveh1.IsDiagonalMove(), "d5-h1 is a diagonal move");
            Assert.AreEqual(true, movef3.IsDiagonalMove(), "d5-f3 is a diagonal move");
            Assert.AreEqual(true, movef7.IsDiagonalMove(), "d5-f7 is a diagonal move");
        }

        [TestMethod]
        public void IsNoDiagonalMove()
        {
            StartSquare d5 = new StartSquare("d5");

            Move moved4 = new Move(PieceCode.B, d5, new DestinationSquare("d4"));
            Move moved6 = new Move(PieceCode.B, d5, new DestinationSquare("d6"));
            Move movea5 = new Move(PieceCode.B, d5, new DestinationSquare("a5"));
            Move moveh5 = new Move(PieceCode.B, d5, new DestinationSquare("h5"));
            Move movee7 = new Move(PieceCode.B, d5, new DestinationSquare("e7"));
            Move movef6 = new Move(PieceCode.B, d5, new DestinationSquare("f6"));

            Assert.AreEqual(false, moved4.IsDiagonalMove(), "d5-d4 is not a diagonal move");
            Assert.AreEqual(false, moved6.IsDiagonalMove(), "d5-d6 is not a diagonal move");
            Assert.AreEqual(false, movea5.IsDiagonalMove(), "d5-a5 is not a diagonal move");
            Assert.AreEqual(false, moveh5.IsDiagonalMove(), "d5-h5 is not a diagonal move");
            Assert.AreEqual(false, movee7.IsDiagonalMove(), "d5-e7 is not a diagonal move");
            Assert.AreEqual(false, movef6.IsDiagonalMove(), "d5-f6 is not a diagonal move");
        }

        [TestMethod]
        public void IsKnightMove()
        {
            StartSquare d4 = new StartSquare("d4");

            Move movee6 = new Move(PieceCode.N, d4, new DestinationSquare("e6"));
            Move movef5 = new Move(PieceCode.N, d4, new DestinationSquare("f5"));
            Move movef3 = new Move(PieceCode.N, d4, new DestinationSquare("f3"));
            Move movee2 = new Move(PieceCode.N, d4, new DestinationSquare("e2"));
            Move movec2 = new Move(PieceCode.N, d4, new DestinationSquare("c2"));
            Move moveb3 = new Move(PieceCode.N, d4, new DestinationSquare("b3"));
            Move moveb5 = new Move(PieceCode.N, d4, new DestinationSquare("b5"));
            Move movec6 = new Move(PieceCode.N, d4, new DestinationSquare("c6"));

            Assert.AreEqual(true, movee6.IsKnightMove(), "d4-e6 is a knight move");
            Assert.AreEqual(true, movef5.IsKnightMove(), "d4-f5 is a knight move");
            Assert.AreEqual(true, movef3.IsKnightMove(), "d4-f3 is a knight move");
            Assert.AreEqual(true, movee2.IsKnightMove(), "d4-e2 is a knight move");
            Assert.AreEqual(true, movec2.IsKnightMove(), "d4-c2 is a knight move");
            Assert.AreEqual(true, moveb3.IsKnightMove(), "d4-b3 is a knight move");
            Assert.AreEqual(true, moveb5.IsKnightMove(), "d4-b5 is a knight move");
            Assert.AreEqual(true, movec6.IsKnightMove(), "d4-c6 is a knight move");
        }

        [TestMethod]
        public void IsNoKnightMove()
        {
            StartSquare d4 = new StartSquare("d4");

            Move moved5 = new Move(PieceCode.N, d4, new DestinationSquare("d5"));
            Move moved1 = new Move(PieceCode.N, d4, new DestinationSquare("d1"));
            Move movef6 = new Move(PieceCode.N, d4, new DestinationSquare("f6"));
            Move movea1 = new Move(PieceCode.N, d4, new DestinationSquare("a1"));
            Move moveg4 = new Move(PieceCode.N, d4, new DestinationSquare("g4"));

            Assert.AreEqual(false, moved5.IsKnightMove(), "d4-d5 is not a knight move");
            Assert.AreEqual(false, moved1.IsKnightMove(), "d4-d1 is not a knight move");
            Assert.AreEqual(false, movef6.IsKnightMove(), "d4-f6 is not a knight move");
            Assert.AreEqual(false, movea1.IsKnightMove(), "d4-a1 is not a knight move");
            Assert.AreEqual(false, moveg4.IsKnightMove(), "d4-g4 is not a knight move");
        }

        [TestMethod]
        public void IsOneStepForwardForWhite()
        {
            Piece whitePawn = new Pawn(Colour.White);
            Move moved4d5 = new Move(PieceCode.None, new StartSquare("d4"), new DestinationSquare("d5"));
            Move movee2e3 = new Move(PieceCode.None, new StartSquare("e2"), new DestinationSquare("e3"));
            Move moveh7h8 = new Move(PieceCode.None, new StartSquare("h7"), new DestinationSquare("h8"));

            Assert.AreEqual(true, moved4d5.IsOneStepForward(whitePawn.Colour), "d4-d5 is one step forward");
            Assert.AreEqual(true, movee2e3.IsOneStepForward(whitePawn.Colour), "e2-e3 is one step forward");
            Assert.AreEqual(true, moveh7h8.IsOneStepForward(whitePawn.Colour), "h7-h8 is one step forward");
        }

        [TestMethod]
        public void IsOneStepForwardForBlack()
        {
            Piece blackPawn = new Pawn(Colour.Black);
            Move moved5d4 = new Move(PieceCode.None, new StartSquare("d5"), new DestinationSquare("d4"));
            Move movee7e6 = new Move(PieceCode.None, new StartSquare("e7"), new DestinationSquare("e6"));
            Move movea2a1 = new Move(PieceCode.None, new StartSquare("a2"), new DestinationSquare("a1"));

            Assert.AreEqual(true, moved5d4.IsOneStepForward(blackPawn.Colour), "d5-d4 is one step forward");
            Assert.AreEqual(true, movee7e6.IsOneStepForward(blackPawn.Colour), "e7-e6 is one step forward");
            Assert.AreEqual(true, movea2a1.IsOneStepForward(blackPawn.Colour), "a2-a1 is one step forward");
        }
    }
}
