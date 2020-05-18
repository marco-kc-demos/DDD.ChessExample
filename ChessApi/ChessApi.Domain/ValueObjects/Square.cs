using DDD.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ChessApi.Domain.ValueObjects
{
    /// <summary>
    /// Represents a square on a chess board
    /// </summary>
    public class Square : ValueObject
    {
        /// <summary>
        /// name of the square on the chess board (e.g. "e2")
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// number of the rank (=row) on the chess board (i.e. 1-8)
        /// </summary>
        public int Rank { get; }
        /// <summary>
        /// letter of the file (=column) on the chess board (i.e. a-h)
        /// </summary>
        public char File { get; }

        public Square(string name)
        {
            IsValid(name);
            Name = name;
            File = name[0];
            Rank = name[1]-48;
        }

        public Square(char file, int rank)
        {
            string name = file.ToString() + rank.ToString();
            IsValid(name);
            Name = name;
            File = file;
            Rank = rank;
        }

        public static implicit operator Square(string name) => new Square(name);

        public override string ToString()
        {
            return Name;
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
        }


        private const string NAME_PATTERN = @"^[a-h][1-8]$";

        private void IsValid(string name)
        {
            if (!Regex.IsMatch(name, NAME_PATTERN))
            {
                throw new InvalidValueException($"'{name}' is not an existing square on a chess board.");
            }
        }
    }
}
