using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Application.Test
{
    internal class SomeEvent : DomainEvent
    {
        public int SomeNumber { get; set; }
    }
}
