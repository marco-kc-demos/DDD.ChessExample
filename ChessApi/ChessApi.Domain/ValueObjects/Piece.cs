using ChessApi.Domain.Entities;
using ChessApi.Domain.ValueObjects;
using DDD.Core;
using System;
using System.Collections.Generic;

namespace ChessApi.Domain.ValueObjects
{
    public abstract class Piece : ValueObject
    {
        public Colour Colour { get; }

        public Piece(Colour colour)
        {
            Colour = colour;
        }

        public abstract PieceCode Code { get; }

        public abstract bool IsValidMove(Move move);

        public override string ToString()
        {
            return this.GetType().Name.ToLower();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Colour;
        }
    }

    public class King : Piece
    {
        public King(Colour colour) : base(colour) { }

        public override PieceCode Code => PieceCode.K;

        public override bool IsValidMove(Move move)
        {
            return move.IsOneStepInAnyDirection();
        }
    }

    public class Queen : Piece
    {
        public Queen(Colour colour) : base(colour) { }

        public override PieceCode Code => PieceCode.Q;

        public override bool IsValidMove(Move move)
        {
            return move.IsHorizontalMove() ||
                     move.IsVerticalMove() ||
                     move.IsDiagonalMove();
        }
    }

    public class Rook : Piece
    {
        public Rook(Colour colour) : base(colour) { }

        public override PieceCode Code => PieceCode.R;

        public override bool IsValidMove(Move move)
        {
            return move.IsHorizontalMove() ||
                     move.IsVerticalMove();
        }
    }

    public class Bishop : Piece
    {
        public Bishop(Colour colour) : base(colour) { }

        public override PieceCode Code => PieceCode.B;

        public override bool IsValidMove(Move move)
        {
            return move.IsDiagonalMove();
        }
    }

    public class Knight : Piece
    {
        public Knight(Colour colour) : base(colour) { }

        public override PieceCode Code => PieceCode.N;

        public override bool IsValidMove(Move move)
        {
            return move.IsKnightMove();
        }
    }

    public class Pawn : Piece
    {
        public Pawn(Colour colour) : base(colour) { }

        public override PieceCode Code => PieceCode.None;

        public override bool IsValidMove(Move move)
        {
            return move.IsOneStepForward(Colour) ||
                   move.IsFirstStepForPawn(Colour);
        }
    }
}
