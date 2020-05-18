using DDD.Example.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Example.Commands
{
    public class CreateBank
    {
        public BankCode BankCode { get; }

        public CreateBank(BankCode bankCode)
        {
            BankCode = bankCode;
        }
    }
}
