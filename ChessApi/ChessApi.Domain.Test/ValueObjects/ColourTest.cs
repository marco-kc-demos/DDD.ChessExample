using ChessApi.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.Test.ValueObjects
{
    [TestClass]
    public class ColourTest
    {
        [TestMethod]
        public void WhiteIsWhite()
        {
            Colour white = Colour.White;
            Colour white2 = Colour.White;

            Assert.IsTrue(white == white2);
            Assert.IsFalse(white != white2);
            Assert.AreEqual(white, white2);
        }

        [TestMethod]
        public void BlackIsNotWhite()
        {
            Colour white = Colour.White;
            Colour black = Colour.Black;

            Assert.IsTrue(white != black);
            Assert.IsFalse(white == black);
            Assert.AreNotEqual(white, black);
        }
    }
}
