using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DDD.Core.Application.Test
{
    internal class BankEventStoreContext : EventStoreContext<long, Bank>
    {
        public BankEventStoreContext(DbContextOptions options) 
            : base(idMustBeGeneratedHere: true, options)
        {
        }
    }
}
