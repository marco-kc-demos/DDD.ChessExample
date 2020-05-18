using ChessApi.Domain.Aggregates;
using DDD.Core.Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Application
{
    public class GameEventStoreContext : EventStoreContext<long, Game>
    {
        public GameEventStoreContext(DbContextOptions options) 
            : base(idMustBeGeneratedHere: false, options)
        {
        }
    }
}
