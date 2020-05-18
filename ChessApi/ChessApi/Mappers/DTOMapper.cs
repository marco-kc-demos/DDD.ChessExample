using ChessApi.Domain.Commands;
using ChessApi.Domain.ValueObjects;
using System;
using System.Linq;

namespace ChessApi.AntiCorruption
{
    public class DTOMapper
    {

        public static MakeMove FromDTO(long id, DTO.MakeMove dtoCommand)
        {
            string notation = dtoCommand.Notation;

            PieceCode pieceCode;
            StartSquare start;
            DestinationSquare destination;

            if ("RNBQK".Contains(notation.First()))
            {
                Enum.TryParse(dtoCommand.Notation.Substring(0, 1), out pieceCode);
                start = new StartSquare(dtoCommand.Notation.Substring(1, 2));
                destination = new DestinationSquare(dtoCommand.Notation.Substring(4, 2));
            }
            else
            {
                pieceCode = PieceCode.None;
                start = new StartSquare(dtoCommand.Notation.Substring(0, 2));
                destination = new DestinationSquare(dtoCommand.Notation.Substring(3, 2));
            }

            var move = new Move(pieceCode, start, destination);
            var result = new MakeMove(id, move);
            return result;
        }
    }
}
