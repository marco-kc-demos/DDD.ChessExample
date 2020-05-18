using ChessApi.Domain.ValueObjects;
using DDD.Core;

namespace ChessApi.Domain.DomainEvents
{
    public class MoveMade : DomainEvent
    {
        public long GameId { get; set; }
        public char? PieceCode { get; set; }
        public string StartSquare { get; set; }
        public string DestinationSquare { get; set; }
    }
}
