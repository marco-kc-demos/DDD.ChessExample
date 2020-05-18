using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DDD.Core.Test
{
    [TestClass]
    public class ValueObjectTest
    {
        [TestMethod]
        public void TestEquality()
        {
            Money m1 = new Money("EUR", 3.99M);
            Money m2 = new Money("EUR", 3.99M);

            Assert.IsTrue(m1.Equals(m2));
            Assert.IsTrue(m2.Equals(m1));
        }

        [TestMethod]
        public void TestInEquality()
        {
            Money m1 = new Money("EUR", 3.99M);
            Money m2 = new Money("USD", 3.99M);

            Assert.IsFalse(m1.Equals(m2));
            Assert.IsFalse(m2.Equals(m1));
        }

        [TestMethod]
        public void TestEqualityWithNullValues()
        {
            Money m1 = new Money(null, 3.99M);
            Money m2 = new Money(null, 3.99M);

            Assert.IsTrue(m1.Equals(m2));
            Assert.IsTrue(m2.Equals(m1));
        }

        [TestMethod]
        public void TestInEqualityWithNullValues()
        {
            Money m1 = new Money("EUR", 3.99M);
            Money m2 = new Money(null, 3.99M);

            Assert.IsFalse(m1.Equals(m2));
            Assert.IsFalse(m2.Equals(m1));
        }

    }

    internal class Money : ValueObject
    {
        public string Currency { get; }
        public decimal Amount { get; }

        public Money(string currency, decimal amount)
        {
            Currency = currency;
            Amount = amount;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Currency;
            yield return Amount;
        }
    }
}