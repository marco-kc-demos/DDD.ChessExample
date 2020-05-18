using ChessApi.Domain.ValueObjects;
using DDD.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.Test.ValueObjects
{
    [TestClass]
    public class SquareTest
    {
        [TestMethod]
        public void SquareNameHasRankAndFile()
        {
            Square target = new Square("e2");

            Assert.AreEqual("e2", target.Name);
            Assert.AreEqual('e', target.File);
            Assert.AreEqual(2, target.Rank);
        }
        [TestMethod]
        public void SquareRankAndFileHasName()
        {
            Square target = new Square('e',2);

            Assert.AreEqual("e2", target.Name);
            Assert.AreEqual('e', target.File);
            Assert.AreEqual(2, target.Rank);
        }

        [TestMethod]
        public void Square_a1_IsValid()
        {
            Square target = new Square("a1");

            Assert.AreEqual("a1", target.Name);
            Assert.AreEqual('a', target.File);
            Assert.AreEqual(1, target.Rank);
        }

        [TestMethod]
        public void Square_h8_IsValid()
        {
            Square target = new Square("h8");

            Assert.AreEqual("h8", target.Name);
            Assert.AreEqual('h', target.File);
            Assert.AreEqual(8, target.Rank);
        }

        [TestMethod]
        public void Square_a0_IsNotValid()
        {
            Action act = () =>
            {
                Square target = new Square("a0");
            };

            var ex = Assert.ThrowsException<InvalidValueException>(act);
            Assert.AreEqual("'a0' is not an existing square on a chess board.", ex.Message);
        }
        [TestMethod]
        public void Square_i2_IsNotValid()
        {
            Action act = () =>
            {
                Square target = new Square("i2");
            };

            var ex = Assert.ThrowsException<InvalidValueException>(act);
            Assert.AreEqual("'i2' is not an existing square on a chess board.", ex.Message);
        }

        [TestMethod]
        public void Square_ab2_IsNotValid()
        {
            Action act = () =>
            {
                Square target = new Square("ab2");
            };

            var ex = Assert.ThrowsException<InvalidValueException>(act);
            Assert.AreEqual("'ab2' is not an existing square on a chess board.", ex.Message);
        }

        [TestMethod]
        public void Square_a23_IsNotValid()
        {
            Action act = () =>
            {
                Square target = new Square("a23");
            };

            var ex = Assert.ThrowsException<InvalidValueException>(act);
            Assert.AreEqual("'a23' is not an existing square on a chess board.", ex.Message);
        }

        [TestMethod]
        public void CastOperator()
        {
            Square target = "e4";

            Assert.AreEqual("e4", target.Name);
        }
    }
}
