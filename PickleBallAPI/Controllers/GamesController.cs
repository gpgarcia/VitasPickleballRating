using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickleBallAPI.Controllers
{
    /// <summary>
    /// Controller that exposes HTTP endpoints to manage games, predictions and player ratings.
    /// </summary>
    /// <remarks>
    /// Uses constructor-injected services:
    /// - <c>VprContext</c> for data access,
    /// - <c>IMapper</c> for DTO/domain mapping,
    /// - <c>ILogger{GamesController}</c> for structured logging.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController
    (
        VprContext context
        , IMapper mapper
        , TimeProvider time
        , GameLogic gameLogic
        , ILogger<GamesController> logger
    ) : ControllerBase
    {

        // GET: api/Games
        /// <summary>
        /// Retrieves a list of all available games.
        /// </summary>
        /// <returns>An <see cref="ActionResult{T}"/> containing an array of <see cref="GameDto"/> objects representing all
        /// games. Returns an empty array if no games are found.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameDto[]))]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames()
        {
            var games = await context.GetAllGamesAsync();
            GameDto[] gameDtos = mapper.Map<IEnumerable<Game>, GameDto[]>(games);
            return Ok(gameDtos);
        }

        // GET: api/Games/export/raw
        /// <summary>
        /// Exports raw game data as a CSV file and returns it as a downloadable attachment.
        /// </summary>
        /// <remarks>
        /// Produces a UTF-8 encoded CSV using CsvHelper. Each row represents a single <see cref="Game"/> and
        /// contains basic, raw fields: game id, facility id, played date (ISO 8601), game type id,
        /// player ids, team scores and changed time. Navigation properties are not included.
        /// The CSV is returned with content type 'text/csv; charset=utf-8' and a filename in the form
        /// 'games_raw_yyyyMMddHHmmss.csv'.
        /// </remarks>
        /// <returns>
        /// 200 OK with a CSV file attachment when successful.
        /// 500 Internal Server Error with a short diagnostic message when an error occurs.
        /// </returns>
        [HttpGet("export/raw")]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportRawGamesCsv()
        {
            logger.LogTrace("Received request to export raw games CSV.");
            var games = await context.GetAllGamesRawAsync();
            var records = mapper.Map<List<GameRawDto>>(games);
            var bytes = await GenerateCsvFileAsync(records);
            // filename of exported file with timestamp
            var fileName = $"games_raw_{time.GetUtcNow():yyyyMMddHHmmss}.csv";
            return File(bytes!, "text/csv; charset=utf-8", fileName);
        }

        private async Task<byte[]> GenerateCsvFileAsync(IEnumerable<GameRawDto> records)
        {
            await using var memoryStream = new MemoryStream();
            await using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
            using var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            csv.WriteRecords(records);
            await streamWriter.FlushAsync();
            var bytes = memoryStream.ToArray();
            if (bytes.Length == 0)
            {
                logger.LogWarning("Export produced an empty CSV.");
            }
            return bytes;
        }


        // GET: api/Games/5
        /// <summary>
        /// Retrieves a single game by identifier.
        /// </summary>
        /// <param name="id">The identifier of the game to fetch.</param>
        /// <returns>
        /// - 200 OK and a <see cref="GameDto"/> when the game exists.
        /// - 404 NotFound when no game with the provided id exists.
        /// </returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// <summary>
        /// Updates an existing game.
        /// </summary>
        /// <remarks>
        /// Validates the incoming <see cref="GameDto"/> and its players, maps the DTO to the
        /// domain <see cref="Game"/> model, recalculates the game's prediction and any affected
        /// player ratings, and persists the changes to the database.
        ///
        /// Behavior:
        /// - Returns 400 BadRequest when the route id does not match <c>gameDto.GameId</c>
        ///   or when validation of the game or its players fails.
        /// - Returns 404 NotFound when a concurrency conflict occurs and the game no longer exists.
        /// - Returns 204 NoContent on successful updat
        /// </remarks>
        /// <param name="id">The identifier of the game to update. Must match <c>gameDto.GameId</c>.</param>
        /// <param name="gameDto">The updated game data transfer object.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with one of:
        /// - <see cref="StatusCodes.Status204NoContent"/> when the update succeeds.
        /// - <see cref="StatusCodes.Status400BadRequest"/> when validation fails or ids mismatch.
        /// - <see cref="StatusCodes.Status404NotFound"/> when the game does not exist.
        /// </returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PutGame(int id, GameDto gameDto)
        {
            //TODO: implement optimistic concurrency
            logger.LogTrace("Received request to update a game.");

            if (id != gameDto.GameId)
            {
                return BadRequest("Id does not match game data");
            }

            var msg = gameLogic.ValidateGame(gameDto);
            if (msg == string.Empty) // only validate players if the game is valid
            {
                msg = gameLogic.ValidateGamePlayers(context, gameDto);
            }
            if (msg != string.Empty)
            {
                return BadRequest(msg);
            }

            // Load the existing game so we update the tracked entity instead of attaching a new one
            var existingGame = await context.GetGameAsync(id);
            if (existingGame == null)
            {
                return NotFound();
            }

            try
            {
                // Map scalar properties from DTO onto the tracked entity
                mapper.Map(gameDto, existingGame);

                // Let dependent data be recalculated for the tracked entity
                await UpdateGameDependencies(existingGame);

                await context.SaveChangesAsync();
                logger.LogTrace("All Saved.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!context.GameExists(id))
                {
                    return NotFound();
                }

                logger.LogWarning(ex, "Concurrency conflict when updating Game {GameId}.", id);
                return StatusCode(StatusCodes.Status409Conflict, "The game was modified by another process. Please refresh and try again.");
            }

            return NoContent();
        }


        // PUT: api/Games/5/update
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Recalculates prediction and ratings for a single existing game, based on its current persisted state.
        /// </summary>
        /// <param name="id">Identifier of the existing game to update.</param>
        /// <returns>
        /// - 204 NoContent on success.
        /// - 404 NotFound when the game does not exist or a concurrency conflict indicates deletion.
        /// - 409 Conflict when a concurrency conflict indicates the game was modified by another process during update.
        /// </returns>
        [HttpPut("{id}/update")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PutGameUpdate(int id)
        {
            logger.LogTrace("Received request to update a game base on existing data.");
            var tmp = await context.GetGameAsync(id);
            if (tmp == null)
            {
                return NotFound();
            }
            var  game = tmp;
            try
            {
                await UpdateGameDependencies(game);
                await context.SaveChangesAsync();
                logger.LogTrace("Saved single updated Game {game.GameId}", game.GameId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return Conflict("The game was modified by another process. Please refresh and try again.");
                }
            }
            return NoContent();
        }

        // PUT: api/Games/update
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Rebuilds all game predictions and player ratings for the entire dataset.
        /// </summary>
        /// <remarks>
        /// Steps performed:
        /// - Removes existing player ratings (via TRUNCATE for SQL Server or ExecuteDelete otherwise).
        /// - Clears stored game predictions for all games.
        /// - Iterates every game and recalculates its prediction and related player ratings.
        ///
        /// Use with caution: this operation affects all persisted rating data and can be long-running.
        /// </remarks>
        /// <returns>
        /// - 204 NoContent on success.
        /// - 404 NotFound if a concurrency conflict indicates a game was deleted during processing.
        /// - 409 Conflict if a concurrency conflict indicates a game was modified by another process during processing.
        /// </returns>
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PutGameUpdate()
        {
            logger.LogTrace("Remove old Player Ratings and Game Prediction");

            // Use bulk delete for efficiency
            if (context.Database.IsSqlServer())
            {
                await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [PlayerRating]");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM [GamePrediction]");
            }
            else
            {
                await context.PlayerRatings.ExecuteDeleteAsync();
                await context.GamePredictions.ExecuteDeleteAsync();
            }

            // Detach all tracked entities to prevent conflicts with bulk delete operations
            context.ChangeTracker.Clear();

            logger.LogTrace("Update all games based on existing data.");

            var gameIds = await context.Games.Select(g => g.GameId).ToListAsync();
            foreach (var id in gameIds)
            {
                var game = await context.GetGameAsync(id);
                if (game != null)
                {
                    await UpdateGameDependencies(game);
                }
            }

            try
            {
                await context.SaveChangesAsync();
                logger.LogTrace("Saved all updated games.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogWarning(ex, "A concurrency conflict occurred while updating games.");
                // Check if any of the games involved in the conflict have been deleted.
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Game game && !await context.GameExistsAsync(game.GameId))
                    {
                        return NotFound($"Game with ID {game.GameId} was deleted during the update process.");
                    }
                }
                return Conflict("The game was modified by another process. Please refresh and try again.");
            }

            return NoContent();
        }



        // POST: api/Games
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a new game.
        /// </summary>
        /// <remarks>
        /// Validates the provided <see cref="GameDto" /> and its players, maps it to the domain <see cref="Game"/>,
        /// computes the initial prediction and any player rating changes, persists the game and returns a 201 Created response.
        /// On success the Location header points to the newly created resource via the <c>GetGame</c> action.
        /// </remarks>
        /// <param name="gameDto">The game DTO to persist.</param>
        /// <returns>
        /// - 201 Created with the provided DTO when successful.
        /// - 400 BadRequest when validation fails.
        /// - 500 InternalServerError when an exception occurs while saving.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type= typeof(Exception))]
        public async Task<ActionResult<GameDto>> PostGame(GameDto gameDto)
        {
            logger.LogTrace("Received request to update a game.");
            var msg = gameLogic.ValidateGame(gameDto);
            try
            {
                if (msg == string.Empty)  // only validate players if the game is valid
                {
                    msg = gameLogic.ValidateGamePlayers(context, gameDto);
                }
                if (msg != string.Empty)
                {
                    return BadRequest(msg);
                }
                var game = mapper.Map<Game>(gameDto);
                context.Games.Add(game);
                await UpdateGameDependencies(game);
                await context.SaveChangesAsync();

                var result = CreatedAtAction("GetGame", new { id = game.GameId }, gameDto);
                context.Entry(game).State = EntityState.Detached;
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while saving the game. Rolling back transaction.");
                return StatusCode(500, "An error occurred while saving the game.");
            }

        }




        /// <summary>
        /// Recalculates and updates game-level dependencies:
        /// - computes and sets the game's <see cref="GamePrediction"/>,
        /// - when the game has a <c>PlayedDate</c>, calculates and persists new <c>PlayerRating</c> records.
        /// </summary>
        /// <param name="game">The game to update dependencies for. The entity may be tracked by the context.</param>
        /// <remarks>
        /// This method will add new <see cref="PlayerRating"/> entities when required and update the in-memory
        /// <see cref="Game"/> instance's <c>GamePrediction</c> property. Persisting to the database is the caller's responsibility.
        /// </remarks>
        private async Task UpdateGameDependencies(Game game)
        {
            //game is not null here
            GameLogic.GameRatings ratings;

            game.Prediction = await gameLogic.CalculatePredictionAsync(context, game);
            logger.LogTrace("Game Prediction calculated and updated.");
            if (game.PlayedDate != null)
            {
                //game played and has scores and prediction
                var newRatings = gameLogic.CalculateNewPlayerRatings(game);
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
                        pr.Rating = n.Rating;
                        pr.RatingDate = n.RatingDate;
                        pr.ChangedTime = n.ChangedTime;
                    }
                }
                logger.LogTrace("Player Ratings updated.");
            }
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
