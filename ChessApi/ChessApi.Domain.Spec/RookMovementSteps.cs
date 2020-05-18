using ChessApi.Domain.Aggregates;
using ChessApi.Domain.Entities;
using ChessApi.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TechTalk.SpecFlow;

namespace ChessApi.Domain.Spec
{
    [Binding]
    public class RookMovementSteps
    {
        private Board _board;
        private Piece _piece;
        private StartSquare _startSquare;

        [Given(@"an empty board")]
        public void GivenAnEmptyBoard()
        {
            _board = new Board(1);
        }
        
        [Given(@"a Rook that starts on '(.*)'")]
        public void GivenARookThatStartsOn(string startSquare)
        {
            _piece = new Rook(Colour.Black);
            _startSquare = new StartSquare(startSquare);
            _board.PutPieceOn(_startSquare, _piece);
        }

        [Given(@"a King that starts on '(.*)'")]
        public void GivenAKingThatStartsOn(string startSquare)
        {
            _piece = new King(Colour.Black);
            _startSquare = new StartSquare(startSquare);
            _board.PutPieceOn(_startSquare, _piece);
        }


        [When(@"It moves")]
        public void WhenItMoves()
        {
        }

        [Then(@"It can move to all '(.*)'s and cannot move to all '(.*)'s")]
        public void ThenItCanMoveToAllSAndNotToAllS(string go, string nogo, Table table)
        {
            int rank = 8;
            foreach(TableRow row in table.Rows)
            {
                foreach(var file in "abcdefgh")
                {
                    string expectedResult = row[file.ToString()];
                    DestinationSquare destination = new DestinationSquare(file, rank);
                    Move move = new Move(_piece.Code, _startSquare, destination);

                    if (expectedResult == go)
                    {
                        bool actualResult = _piece.IsValidMove(move);
                        Assert.AreEqual(true, actualResult, $"it should be allowd to go to {file}{rank}");
                    }
                    else if (expectedResult == nogo)
                    {
                        bool actualResult = _piece.IsValidMove(move);
                        Assert.AreEqual(false, actualResult, $"it should not be allowd to go to {file}{rank}");
                    }
                }
                rank--;
            }

        }

    }
}
