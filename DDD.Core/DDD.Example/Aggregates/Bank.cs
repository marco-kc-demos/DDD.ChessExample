using DDD.Core;
using DDD.Example.Commands;
using DDD.Example.DomainEvents;
using DDD.Example.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Example
{
    public class Bank : AggregateRoot<int>
    {

        public Bank(int id) : base(id)
        {
        }

        public Bank(int id, IEnumerable<DomainEvent> events) : base(id, events)
        {
        }

        public void OpenAccount(OpenAccount command)
        {
            AccountNumber number = new AccountNumber("NL12ABCD0001234567");
            AccountOpened accountOpened = new AccountOpened(number, command.Owner);
            RaiseEvent(accountOpened);
        }

        protected override void When(DomainEvent domainEvent)
        {
            Handle(domainEvent as dynamic);
        }

        public void Handle(AccountOpened accountOpened)
        {
        }
    }
}