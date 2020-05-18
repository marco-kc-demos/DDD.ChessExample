using ChessApi.Domain.Entities;
using ChessApi.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.Test.Entities
{
    [TestClass]
    public class BoardTest
    {
        [TestMethod]
        public void ANewBoardIsEmpty()
        {
            Board target = new Board(1);

            Assert.IsTrue(target.IsEmpty("a1"));
            Assert.IsTrue(target.IsEmpty("h8"));
            Assert.IsTrue(target.IsEmpty("c3"));
        }

        [TestMethod]
        public void CannotGetAPieceOnANewBoard()
        {
            Board target = new Board(1);

            Action act = () =>
            {
                target.GetPieceOn("a1");
            };

            var ex = Assert.ThrowsException<InvalidOperationException>(act);
            Assert.AreEqual("The square a1 does not contain a piece", ex.Message);
        }

        [TestMethod]
        public void QuareWithPiceIsNotEmpty()
        {
            Board board = new Board(1);
            Piece rook = new Rook(Colour.White);
            board.PutPieceOn("a1", rook);

            bool result = board.IsEmpty("a1");

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void CanGetAPieceWhenPlaced()
        {
            Board board = new Board(1);
            Piece rook = new Rook(Colour.White);
            board.PutPieceOn("a1", rook);

            Piece result = board.GetPieceOn("a1");

            Assert.AreEqual(rook, result);
        }

        [TestMethod]
        public void TwoBoardsAreNotTheSame()
        {
            Board board1 = new Board(1);
            Board board2 = new Board(2);
            Piece rook = new Rook(Colour.Black);
            board1.PutPieceOn("b2", rook);

            Action act = () =>
            {
                board2.GetPieceOn("b2");
            };

            var ex = Assert.ThrowsException<InvalidOperationException>(act);
            Assert.AreEqual("The square b2 does not contain a piece", ex.Message);
        }

        [TestMethod]
        public void TheSquareIsEmptyAfterPieceHasBeenRemoved()
        {
            Board board = new Board(1);
            Piece rook = new Rook(Colour.Black);
            board.PutPieceOn("h1", rook);

            board.RemovePieceFrom("h1");

            Assert.IsTrue(board.IsEmpty("h1"));
        }

        [TestMethod]
        public void ThePieceIsObtainedWhenPieceHasBeenRemoved()
        {
            Board board = new Board(1);
            Piece rook = new Rook(Colour.White);
            board.PutPieceOn("a8", rook);

            Piece result = board.RemovePieceFrom("a8");

            Assert.AreEqual(rook, result);
        }

        [TestMethod]
        public void WhenPiecePlacedThenSquareIsOccupied()
        {
            Board board = new Board(1);
            Piece rook = new Rook(Colour.Black);
            board.PutPieceOn("h8", rook);

            bool result = board.IsOccupied("h8");

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void WhenPieceNotPlacedThenSquareIsNotOccupied()
        {
            Board board = new Board(1);
            Piece rook = new Rook(Colour.Black);

            bool result = board.IsOccupied("h8");

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void WhenPiecePlacedThenSquareIsOccupiedByPiece()
        {
            Board board = new Board(1);
            Piece rook = new Rook(Colour.Black);
            board.PutPieceOn("h8", rook);

            bool result = board.IsOccupiedByPiece("h8", PieceCode.R);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void WhenPieceNotPlacedThenSquareIsNotOccupiedByPiece()
        {
            Board board = new Board(1);
            Piece rook = new Rook(Colour.Black);

            bool result = board.IsOccupiedByPiece("h8", PieceCode.R);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void WhenPieceNotPlacedOnThatSquareThenSquareIsNotOccupiedByPiece()
        {
            Board board = new Board(1);
            Piece blackRook = new Rook(Colour.Black);
            board.PutPieceOn("g8", blackRook);

            bool result = board.IsOccupiedByPiece("h8", PieceCode.R);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void MovePiece()
        {
            Board board = new Board(1);
            Piece blackRook = new Rook(Colour.Black);
            board.PutPieceOn("h8", blackRook);
            Move move = new Move(blackRook.Code, new StartSquare("h8"), new DestinationSquare("e8"));

            board.ExecuteMove(move);

            Assert.IsTrue(board.IsEmpty("h8"));
            Assert.IsTrue(board.IsOccupiedByPiece("e8", blackRook.Code));
        }
    }
}
