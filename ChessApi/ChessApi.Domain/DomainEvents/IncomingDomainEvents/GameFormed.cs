using DDD.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApi.Domain.IncomingDomainEvents
{
    public class GameFormed : DomainEvent
    {
        public long GameId { get; set; }
    }
}
