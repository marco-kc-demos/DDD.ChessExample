using ChessApi.Domain.Aggregates;
using DDD.Core.Application;
using System.Threading.Tasks;

namespace ChessApi.Application.Repositories
{
    public interface IGameRepository : IEventStoreRepository<long, Game>
    {
    }
}