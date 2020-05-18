using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.ValueObjects
{
    public class StartSquare : Square
    {
        public StartSquare(string name) : base(name) { }
    }
}
