using DDD.Example.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Example.DomainEvents
{
    public class MoneTransfered
    {
        public AccountNumber FromAccountNumber { get;  }
        public AccountNumber ToAccountNumber { get; }
        public Money Money { get; }

        public MoneTransfered(AccountNumber fromAccountNumber, AccountNumber toAccountNumber, Money money)
        {
            FromAccountNumber = fromAccountNumber;
            ToAccountNumber = toAccountNumber;
            Money = money;
        }
    }
}
