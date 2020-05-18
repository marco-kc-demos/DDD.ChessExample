using ChessApi.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.Test.ValueObjects
{
    [TestClass]
    public class PieceTest
    {
        [TestMethod]
        public void PieceHasColour()
        {
            Piece target = new Rook(Colour.White);

            Assert.AreEqual(Colour.White, target.Colour);
        }

        [TestMethod]
        public void PieceToString_WhiteRook()
        {
            Piece target = new Rook(Colour.White);

            Assert.AreEqual("rook", target.ToString());
        }


        [TestMethod]
        public void PieceToString_BlackKing()
        {
            Piece target = new King(Colour.Black);

            Assert.AreEqual("king", target.ToString());
        }
    }
}
