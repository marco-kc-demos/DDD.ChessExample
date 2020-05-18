using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Core.Application.Test
{
    internal class BankRepository : EventStoreRepository<long, Bank>
    {
        public BankRepository(DbContextOptions<BankEventStoreContext> options) 
            : base(options)
        {
        }

        protected override EventStoreContext<long, Bank> CreateContext(DbContextOptions options)
        {
            return new BankEventStoreContext(options);
        }
    }
}
