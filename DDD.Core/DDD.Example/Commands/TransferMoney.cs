using DDD.Example.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Example.Commands
{
    public class TransferMoney
    {
        public AccountNumber FromAccountNumber { get; }
        public AccountNumber ToAccountNumber { get; }
        public Money Money { get; }

        public TransferMoney(AccountNumber fromAccountNumber, AccountNumber toAccountNumber, Money money)
        {
            FromAccountNumber = fromAccountNumber;
            ToAccountNumber = toAccountNumber;
            Money = money;
        }
    }
}
