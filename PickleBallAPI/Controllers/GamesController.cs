using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickleBallAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController(VprContext context, IMapper mapper, ILogger<GamesController> logger) : ControllerBase
    {
        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames()
        {
            var games = await context.GetAllGamesAsync();
            GameDto[] gameDtos = mapper.Map<IEnumerable<Game>, GameDto[]>(games);
            return Ok(gameDtos);
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGame(int id)
        {
            var game = await context.GetGameAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<GameDto>(game));
        }

        // PUT: api/Games/5
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameDto gameDto)
        {
            //TODO: implement optimistic concurrency
            logger.LogTrace("Received request to update a game.");
            Game game = null!;
            if (id != gameDto.GameId)
            {
                return BadRequest("Id does not match game data");
            }
            var msg = GameLogic.ValidateGame(gameDto);
            if (msg == string.Empty)  // only validate players if the game is valid
            {
                msg = GameLogic.ValidateGamePlayers(context, gameDto);
            }
            if (msg != string.Empty)
            {
                return BadRequest(msg);
            }
            game = mapper.Map<Game>(gameDto);


            try
            {
                context.Entry(game).State = EntityState.Modified;
                (GameLogic.GameRatings ratings, GamePrediction gamePrediction) = await CalculatePrediction(game);
                context.Entry(gamePrediction).State = EntityState.Modified;
                game.GamePrediction = gamePrediction;
                logger.LogTrace("Game Prediction calculated and added.");
                var newRatings = GameLogic.CalculateNewPlayerRatings(game, ratings, gamePrediction);
                context.PlayerRatings.AddRange(newRatings);
                logger.LogTrace("Player Ratings added.");

                await context.SaveChangesAsync();
                logger.LogTrace("All Saved.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        // PUT: api/Games/5/update
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}/update")]
        public async Task<IActionResult> PutGameUpdate(int id)
        {
            logger.LogTrace("Received request to update a game base on existing data.");
            Game game = null!;
            var tmp = await context.GetGameAsync(id);
            if (tmp == null)
            {
                return NotFound();
            }
            game = tmp;
            try
            {
                (GameLogic.GameRatings ratings, GamePrediction gamePrediction) = await CalculatePrediction(game);
                game.GamePrediction = gamePrediction;
                logger.LogTrace("Game Prediction calculated and added.");
                var newRatings = GameLogic.CalculateNewPlayerRatings(game, ratings, gamePrediction);
                foreach (var n in newRatings)
                {
                    var pr = context.PlayerRatings.FirstOrDefault(pr =>
                        pr.PlayerId == n.PlayerId &&
                        pr.GameId == n.GameId);
                    if(pr == null)
                    {
                        context.PlayerRatings.Add(n);
                    }
                    else
                    {
                        pr = n;
                    }
                }
                logger.LogTrace("Player Ratings updated.");
                await context.SaveChangesAsync();
                logger.LogTrace("All Saved.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // PUT: api/Games/update
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        public async Task<IActionResult> PutGameUpdate()
        {
            logger.LogTrace("update all games base on existing data.");
            Game game = null!;
            foreach (var id in context.Games.Select(g => g.GameId).ToList())
            {
                var tmp = await context.GetGameAsync(id);
                game = tmp!;
                try
                {
                    (GameLogic.GameRatings ratings, GamePrediction gamePrediction) = await CalculatePrediction(game);
                    game.GamePrediction = gamePrediction;
                    logger.LogTrace("Game Prediction calculated and updated.");
                    var newRatings = GameLogic.CalculateNewPlayerRatings(game, ratings, gamePrediction);
                    foreach (var n in newRatings)
                    {
                        var pr = context.PlayerRatings.FirstOrDefault(pr =>
                            pr.PlayerId == n.PlayerId &&
                            pr.GameId == n.GameId);
                        if (pr == null)
                        {
                            context.PlayerRatings.Add(n);
                        }
                        else
                        {
                            pr = n;
                        }
                    }
                    logger.LogTrace("Player Ratings updated.");
                    await context.SaveChangesAsync();
                    logger.LogTrace("Saved Updated Game {game.GameId}", game.GameId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!context.GameExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return NoContent();
        }


        // POST: api/Games
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameDto>> PostGame(GameDto gameDto)
        {
            logger.LogTrace("Received request to update a game.");
            var msg = GameLogic.ValidateGame(gameDto);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (msg == string.Empty)  // only validate players if the game is valid
                {
                    msg = GameLogic.ValidateGamePlayers(context, gameDto);
                }
                if (msg != string.Empty)
                {
                    return BadRequest(msg);
                }
                var game = mapper.Map<Game>(gameDto);
                context.Games.Add(game);
                logger.LogTrace("Game Saved.");

                (GameLogic.GameRatings ratings, GamePrediction gamePrediction) = await CalculatePrediction(game);
                game.GamePrediction = gamePrediction;
                logger.LogTrace("Game Prediction calculated and saved.");
                if ( game.PlayedDate != null)
                {
                    // Only calculate new ratings if the game has been played and scores exists)
                    var newRatings = GameLogic.CalculateNewPlayerRatings(game, ratings, gamePrediction);
                    context.PlayerRatings.AddRange(newRatings);
                }
                await context.SaveChangesAsync();
                logger.LogTrace("Player Ratings saved.");
                await transaction.CommitAsync();

                var result = CreatedAtAction("GetGame", new { id = game.GameId }, gameDto);
                context.Entry(game).State = EntityState.Detached;
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while saving the game. Rolling back transaction.");
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while saving the game.");
            }

        }



        private async Task<(GameLogic.GameRatings ratings, GamePrediction gamePrediction)> CalculatePrediction(Game game)
        {
            var playedAt = game.PlayedDate ?? DateTimeOffset.Now;
            var gameId = game.GameId;
            logger.LogTrace("Calculating Game Prediction  for game {gameId} played at {playedAt}.", gameId, playedAt);
            var ratings = await GameLogic.GetPlayerRatingsAsync(context, game, playedAt);
            var gamePrediction = GameLogic.GetGamePrediction(game.GameId, playedAt, ratings);
            return (ratings, gamePrediction);
        }

        //// DELETE: api/Games/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteGame(int id)
        //{
        //    var game = await _context.Games.FindAsync(id);
        //    if (game == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Games.Remove(game);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}


    }

}
