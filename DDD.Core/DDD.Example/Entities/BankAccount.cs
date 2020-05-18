using DDD.Core;
using DDD.Example.ValueObjects;

namespace DDD.Example.Entities
{
    public class BankAccount : Entity<long>
    {
        public AccountNumber AccountNumber { get; set; }
        public string Owner { get; set; }
        public Money Balance { get; set; }

        public BankAccount(long id) : base(id)
        {
        }
    }
}
