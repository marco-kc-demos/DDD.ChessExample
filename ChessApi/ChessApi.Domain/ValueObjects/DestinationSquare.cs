using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.ValueObjects
{
    public class DestinationSquare : Square
    {
        public DestinationSquare(string name) : base(name) { }

        public DestinationSquare(char file, int rank) : base(file, rank) { }
    }
}
