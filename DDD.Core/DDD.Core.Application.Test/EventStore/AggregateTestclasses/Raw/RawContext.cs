using DDD.Core.Application.EventStoreModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Application.Test.Raw
{
    public class RawContext : DbContext
    {
        public RawContext(DbContextOptions<RawContext> options) : base(options)
        {
        }

        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankEvent> BankEvents { get; set; }
    }


}
