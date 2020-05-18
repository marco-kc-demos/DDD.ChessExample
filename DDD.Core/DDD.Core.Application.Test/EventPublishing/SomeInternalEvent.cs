using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Application.Test
{
    internal class SomeInternalEvent : InternalDomainEvent
    {
        public int SomeNumber { get; set; }
    }
}
