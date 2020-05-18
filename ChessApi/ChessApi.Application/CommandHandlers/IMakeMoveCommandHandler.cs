using ChessApi.Domain.Commands;
using System.Threading.Tasks;

namespace ChessApi.Application.CommandHandlers
{
    public interface IMakeMoveCommandHandler
    {
        Task HandleCommandAsync(MakeMove command);
    }
}