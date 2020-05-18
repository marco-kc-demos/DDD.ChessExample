using DDD.Example.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Example.DomainEvents
{
    public class BankCreated
    {
        public int BankId { get; }
        public BankCode BankCode { get; }

        public BankCreated(int bankId, BankCode bankCode)
        {
            BankId = bankId;
            BankCode = bankCode;
        }
    }
}
