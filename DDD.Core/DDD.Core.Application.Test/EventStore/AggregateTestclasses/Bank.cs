using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Application.Test
{
    internal class Bank : AggregateRoot<long>
    {
        public Bank(long id) : base(id)
        {
        }

        public Bank(long id, IEnumerable<DomainEvent> events) : base(id, events)
        {
        }

        public void OpenAccount(OpenAccount command)
        {
            AccountOpened accountOpened = new AccountOpened(1, command.Owner);
            RaiseEvent(accountOpened);
        }

        protected override void When(DomainEvent domainEvent)
        {
            Handle(domainEvent as dynamic);
        }

        public int HandleAccountOpenedCallCount = 0;
        public AccountOpened HandleAccountOpenedArgument = null;
        public bool HandleAccountIsReplaying = false;

        public void Handle(AccountOpened accountOpened)
        {
            HandleAccountOpenedCallCount++;
            HandleAccountOpenedArgument = accountOpened;
            HandleAccountIsReplaying = IsReplaying;
        }
    }
}
