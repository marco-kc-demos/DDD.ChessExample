using DDD.Core;
using System.Collections.Generic;

namespace DDD.Example.ValueObjects
{
    public class Money : ValueObject
    {
        public Currency Currency { get; }
        public decimal Amount { get; }

        public Money(Currency currency, decimal amount)
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