using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.Commands
{
    public class StartGame
    {
        public long GameId { get; }

        public StartGame(long gameId)
        {
            GameId = gameId;
        }
    }
}
