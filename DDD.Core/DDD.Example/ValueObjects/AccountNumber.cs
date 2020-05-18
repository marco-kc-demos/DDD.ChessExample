using DDD.Core;
using System.Collections.Generic;

namespace DDD.Example.ValueObjects
{
    public class AccountNumber : ValueObject
    {
        public string IBANcode { get; }

        public AccountNumber(string ibancode)
        {
            IBANcode = ibancode;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return IBANcode;
        }
    }
}