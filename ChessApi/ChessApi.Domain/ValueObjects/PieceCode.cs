using DDD.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.ValueObjects
{
    public enum PieceCode
    {
        None,
        N,
        B,
        R,
        Q,
        K,
    }

    public static class PieceCodes
    {
        public static char? ToChar(this PieceCode pieceCode)
        {
            switch (pieceCode)
            {
                case PieceCode.None: return null;
                case PieceCode.N: return 'N'; 
                case PieceCode.B: return 'B';
                case PieceCode.R: return 'R';
                case PieceCode.Q: return 'Q';
                case PieceCode.K: return 'K';
                default: throw new InvalidOperationException("Unknown PieceCode");
            }
        }

        public static PieceCode FromChar(char? code)
        {
            if (code == null)
            {
                return PieceCode.None;
            }
            else if ("RNBQK".Contains(code.Value))
            {
                return Enum.Parse<PieceCode>(code.Value.ToString());
            }
            else
            {
                throw new InvalidValueException($"'{code}' is not a valid abbreviation for a chess piece.");
            }
        }

        public static string GetName(this PieceCode pieceCode)
        {
            switch (pieceCode)
            {
                case PieceCode.None: return "pawn";
                case PieceCode.N: return "Knight";
                case PieceCode.B: return "Bishop";
                case PieceCode.R: return "Rook";
                case PieceCode.Q: return "Queen";
                case PieceCode.K: return "King";
                default: throw new InvalidOperationException("Unknown PieceCode");
            }
        }
    }
}
