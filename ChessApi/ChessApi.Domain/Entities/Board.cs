using ChessApi.Domain.ValueObjects;
using DDD.Core;
using System;
using System.Collections.Generic;

namespace ChessApi.Domain.Entities
{
    public class Board : Entity<long>
    {
        private Piece?[,] _pieces = new Piece[8, 8];

        private static int fileIndex(Square square)
        {
            return square.File - 'a';
        }
        private static int rankIndex(Square square)
        {
            return square.Rank - 1;
        }

        public Board(long id) : base(id)
        {
        }


        public bool IsEmptyAt(Square square)
        {
            return _pieces[rankIndex(square), fileIndex(square)] == null;
        }


        public Piece GetPieceOn(Square square)
        {
            return _pieces[rankIndex(square), fileIndex(square)] ??
                   throw new InvalidOperationException($"The square {square} does not contain a piece");
        }

        public void PutPieceOn(Square square, Piece piece)
        {
            _pieces[rankIndex(square), fileIndex(square)] = piece;
        }

        public Piece RemovePieceFrom(Square square)
        {
            Piece piece = GetPieceOn(square);
            _pieces[rankIndex(square), fileIndex(square)] = null;
            return piece;
        }



        public bool IsOccupiedAt(Square square)
        {
            return !IsEmptyAt(square);
        }

        public bool HasThisPieceOn(Square square, PieceCode pieceCode)
        {
            return !IsEmptyAt(square) && GetPieceOn(square).Code == pieceCode;
        }


        public void ExecuteMove(Move move)
        {
            Piece piece = RemovePieceFrom(move.StartSquare);
            PutPieceOn(move.DestinationSquare, piece);
        }

        public void Setup()
        {
            PutPieceOn("a1", new Rook(Colour.White));
            PutPieceOn("b1", new Knight(Colour.White));
            PutPieceOn("c1", new Bishop(Colour.White));
            PutPieceOn("d1", new Queen(Colour.White));
            PutPieceOn("e1", new King(Colour.White));
            PutPieceOn("f1", new Bishop(Colour.White));
            PutPieceOn("g1", new Knight(Colour.White));
            PutPieceOn("h1", new Pawn(Colour.White));
            PutPieceOn("a2", new Pawn(Colour.White));
            PutPieceOn("b2", new Pawn(Colour.White));
            PutPieceOn("c2", new Pawn(Colour.White));
            PutPieceOn("d2", new Pawn(Colour.White));
            PutPieceOn("e2", new Pawn(Colour.White));
            PutPieceOn("f2", new Pawn(Colour.White));
            PutPieceOn("g2", new Pawn(Colour.White));
            PutPieceOn("h2", new Pawn(Colour.White));

            PutPieceOn("a7", new Pawn(Colour.Black));
            PutPieceOn("b7", new Pawn(Colour.Black));
            PutPieceOn("c7", new Pawn(Colour.Black));
            PutPieceOn("d7", new Pawn(Colour.Black));
            PutPieceOn("e7", new Pawn(Colour.Black));
            PutPieceOn("f7", new Pawn(Colour.Black));
            PutPieceOn("g7", new Pawn(Colour.Black));
            PutPieceOn("h7", new Pawn(Colour.Black));
            PutPieceOn("a8", new Rook(Colour.Black));
            PutPieceOn("b8", new Knight(Colour.Black));
            PutPieceOn("c8", new Bishop(Colour.Black));
            PutPieceOn("d8", new Queen(Colour.Black));
            PutPieceOn("e8", new King(Colour.Black));
            PutPieceOn("f8", new Bishop(Colour.Black));
            PutPieceOn("g8", new Knight(Colour.Black));
            PutPieceOn("h8", new Rook(Colour.Black));
        }
    }
}
