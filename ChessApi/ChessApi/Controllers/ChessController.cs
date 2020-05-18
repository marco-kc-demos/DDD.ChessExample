using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessApi.AntiCorruption;
using ChessApi.Application;
using ChessApi.Application.CommandHandlers;
using ChessApi.Domain.Aggregates;
using ChessApi.Domain.Commands;
using DDD.Core.BusinessRules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChessApi.Controllers
{
    [ApiController]
    public class ChessController : ControllerBase
    {
        private readonly IStartGameCommandHandler _startGameCommandHandler;
        private readonly IMakeMoveCommandHandler _makeMoveCommandHandler;

        public ChessController(IStartGameCommandHandler startGameCommandHandler, 
                               IMakeMoveCommandHandler makeMoveCommandHandler)
        {
            _startGameCommandHandler = startGameCommandHandler;
            _makeMoveCommandHandler = makeMoveCommandHandler;
        }

        [HttpGet("Game/{id}/StartGame")]
        public async Task<IActionResult> StartGame(long id)
        {
            try
            {
                await _startGameCommandHandler.HandleCommandAsync(new StartGame(id));
                return Ok();
            }
            catch (BusinessRuleViolationException brve)
            {
                return StatusCode(StatusCodes.Status409Conflict, brve.Violations);
            }
            //catch
            //{
            //    return StatusCode(501);
            //}
        }

        [HttpPost("Game/{id}/MakeMove")]
        public async Task<IActionResult> MakeMove(long id, [FromBody] DTO.MakeMove dtoCommand)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest("not a valid move.");
            //}

            try
            {
                MakeMove command = DTOMapper.FromDTO(id, dtoCommand);
                await _makeMoveCommandHandler.HandleCommandAsync(command);
                return Ok();
            }
            catch (GameNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BusinessRuleViolationException brve)
            {
                return StatusCode(StatusCodes.Status409Conflict, brve.Violations);
            }
            //catch 
            //{
            //    return StatusCode(501);
            //}
        }
    }
}