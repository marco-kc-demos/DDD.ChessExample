using DDD.Core;
using DDD.Example.ValueObjects;

namespace DDD.Example.DomainEvents
{
    public class AccountOpened : DomainEvent
    {
        public AccountNumber AccountNumber { get; }
        public string Owner { get; set; }

        public AccountOpened(AccountNumber accountNumber, string owner)
        {
            AccountNumber = accountNumber;
            Owner = owner;
        }
    }
}
