using ChessApi.Domain.ValueObjects;

namespace ChessApi.Domain.Commands
{
    public class MakeMove
    {
        public long GameId { get; }
        public Move Move { get; }

        public MakeMove(long gameId, Move move)
        {
            GameId = gameId;
            Move = move;
        }
    }
}
