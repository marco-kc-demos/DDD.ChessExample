using ChessApi.Domain.Commands;
using System.Threading.Tasks;

namespace ChessApi.Application.CommandHandlers
{
    public interface IStartGameCommandHandler
    {
        Task HandleCommandAsync(StartGame command);
    }
}