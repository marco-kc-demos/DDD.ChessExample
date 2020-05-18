using ChessApi.Domain.ValueObjects;
using DDD.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.ValueObjects
{
    public class Move : ValueObject
    {
        public PieceCode PieceCode { get; }
        public StartSquare StartSquare { get; }
        public DestinationSquare DestinationSquare { get; }

        public Move(PieceCode pieceCode, StartSquare startSquare, DestinationSquare destinationSquare)
        {
            PieceCode = pieceCode;
            StartSquare = startSquare;
            DestinationSquare = destinationSquare;
        }

        public bool IsHorizontalMove()
        {
            return DestinationSquare.Rank == StartSquare.Rank;
        }

        public bool IsVerticalMove()
        {
            return DestinationSquare.File == StartSquare.File;
        }

        public bool IsOneStepInAnyDirection()
        {
            return  Math.Abs(DestinationSquare.Rank - StartSquare.Rank) <= 1 &&
                    Math.Abs(DestinationSquare.File - StartSquare.File) <= 1;
        }

        public bool IsDiagonalMove()
        {
            return
                Math.Abs(DestinationSquare.Rank - StartSquare.Rank) ==
                    Math.Abs(DestinationSquare.File - StartSquare.File);
        }

        public bool IsKnightMove()
        {
            return (Math.Abs(DestinationSquare.Rank - StartSquare.Rank) == 2 &&
                    Math.Abs(DestinationSquare.File - StartSquare.File) == 1)
                    ||
                   (Math.Abs(DestinationSquare.Rank - StartSquare.Rank) == 1 &&
                    Math.Abs(DestinationSquare.File - StartSquare.File) == 2);
        }

        public bool IsOneStepForward(Colour colour)
        {
            return colour switch
            {
                Colour.White => DestinationSquare.Rank > StartSquare.Rank &&
                                DestinationSquare.File == StartSquare.File,
                Colour.Black => DestinationSquare.Rank < StartSquare.Rank &&
                                DestinationSquare.File == StartSquare.File,
                _            => throw new InvalidOperationException($"Invalid colour encountered."),
            };
        }

        public bool IsFirstStepForPawn(Colour colour)
        {
            return colour switch
            {
                Colour.White => StartSquare.Rank == 2 && DestinationSquare.Rank == 4 && 
                                StartSquare.File == DestinationSquare.File,
                Colour.Black => StartSquare.Rank == 7 && DestinationSquare.Rank == 5 &&
                                StartSquare.File == DestinationSquare.File,
                _            => throw new InvalidOperationException($"Invalid colour encountered."),
            };
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return PieceCode;
            yield return StartSquare;
            yield return DestinationSquare;
        }
    }
}
