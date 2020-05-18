using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Test
{
    internal class AccountOpened : DomainEvent
    {
        public long AccountNumber { get; set; }
        public string Owner { get; set; }

        public AccountOpened(long accountNumber, string owner)
        {
            AccountNumber = accountNumber;
            Owner = owner;
        }
    }
}
