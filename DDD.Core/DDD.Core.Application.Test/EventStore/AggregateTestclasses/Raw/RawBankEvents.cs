using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Application.Test.Raw
{
    public class BankEvent
    {
        public long Id { get; set; }
        public long BankId { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
    }
}
