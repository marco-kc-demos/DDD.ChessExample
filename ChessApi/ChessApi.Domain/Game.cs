using ChessApi.Domain.ChessRules;
using ChessApi.Domain.Commands;
using ChessApi.Domain.DomainEvents;
using ChessApi.Domain.Entities;
using ChessApi.Domain.IncomingDomainEvents;
using ChessApi.Domain.Mappers;
using ChessApi.Domain.ValueObjects;
using DDD.Core;
using DDD.Core.BusinessRules;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessApi.Domain.Aggregates
{
    public class Game : AggregateRoot<long>
    {
        public Board Board { get; private set; }

        public Game(long id) : base(id)
        {
            Board = new Board(id);
        }

        public Game(long id, IEnumerable<DomainEvent> events) : base(id, events)
        {
        }

        #region handle commands
        public void StartGame(StartGame command)
        {

            GameStarted e = new GameStarted(this.Id);
            RaiseEvent(e);
        }

        public void MakeMove(MakeMove command)
        {
            BusinessRule.ThrowIfNotSatisfied(
                new PieceMustActuallyMove(command.Move)
              & new PieceMustOccupyStartingSquare(Board, command.Move)
              & new MoveIsValidForPiece(Board, command.Move)
              // & new MovePathIsUnobstructed(Board, command.Move)
            );

            MoveMade e = command.Move.MapToMoveMade(this.Id);
            RaiseEvent(e);
        }
        #endregion handle commands


        #region execute events
        protected override void When(DomainEvent domainEvent)
        {
            Handle(domainEvent as dynamic);
        }
        
        public void Handle(GameStarted gameStarted)
        {
            Board = new Board(gameStarted.GameId);
            Board.Setup();
        }

        public void Handle(MoveMade moveMade)
        {
            Move move = moveMade.MapToMove();
            Board.ExecuteMove(move);
            //if (!IsReplaying)
            //{
            //    StuurRobotAan(move); // Zijeffect op de echte wereld
            //}
        }
        #endregion Execute Events
    }
}
