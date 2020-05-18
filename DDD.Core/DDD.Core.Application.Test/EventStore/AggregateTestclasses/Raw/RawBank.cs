using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DDD.Core.Application.Test.Raw
{
    public class Bank
    {
        public long Id { get; set; }
        [ConcurrencyCheck]
        public int Version { get; set; }
        public ICollection<BankEvent> Events { get; set; }
    }
}
