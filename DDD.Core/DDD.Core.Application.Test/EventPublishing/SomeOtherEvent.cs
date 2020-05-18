using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Application.Test
{
    internal class SomeOtherEvent : DomainEvent
    {
        public int SomeNumber { get; set; }
    }
}
