using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core
{
    /// <summary>
    /// Represents a domain event (DDD) that should not be published publicly.
    /// </summary>
    public class InternalDomainEvent : DomainEvent
    {
    }
}
