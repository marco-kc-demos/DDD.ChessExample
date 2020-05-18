using ChessApi.Domain.Aggregates;
using DDD.Core.Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChessApi.Application.Repositories
{
    public class GameRepository : EventStoreRepository<long, Game>, IGameRepository
    {
        public GameRepository(DbContextOptions<GameEventStoreContext> options) : 
            base(options)
        {
        }

        protected override EventStoreContext<long, Game> CreateContext(DbContextOptions options)
        {
            return new GameEventStoreContext(options);
        }
    }
}
