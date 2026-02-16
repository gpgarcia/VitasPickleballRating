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
using System.Text;
using System.Threading.Tasks;

namespace PickleBallAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController(VprContext context, IMapper mapper, TimeProvider time, ILogger<PlayerController> logger) : ControllerBase
    {

        // GET: api/Player
        /// <summary>
        /// Retrieves all players.
        /// </summary>
        /// <remarks>
        /// Returns an array of <see cref="PlayerDto"/> representing all players in the system.
        /// </remarks>
        /// <returns>200 OK with an array of <see cref="PlayerDto"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlayerDto[]))]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers()
        {
            var players = await context.GetAllPlayersAsync();
            PlayerDto[] playerDtos = mapper.Map<PlayerDto[]>(players);
            return Ok(playerDtos);

        }

        // GET: api/Player/5
        /// <summary>
        /// Retrieves a single player by id.
        /// </summary>
        /// <param name="id">Player ide.</param>
        /// <returns>
        /// 200 OK with a <see cref="PlayerDto"/> when the player exists.
        /// 404 NotFound when no player with the provided id exists.
        /// </returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlayerDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlayerDto>> GetPlayer(int id)
        {
            var player = await context.GetPlayerByIdAsync(id);

            if (player == null)
            {
                return NotFound();
            }
            var playerDto = mapper.Map<PlayerDto>(player);
            return Ok(playerDto);
        }
        // GET: api/Games/export/raw
        /// <summary>
        /// Exports raw player data as a CSV file and returns it as a downloadable attachment.
        /// </summary>
        /// <remarks>
        /// Produces a UTF-8 encoded CSV using CsvHelper. Each row represents a single <see cref="Player"/> and
        /// contains basic, raw fields: player id and name. Navigation properties are not included.
        /// The CSV is returned with content type 'text/csv; charset=utf-8' and a filename in the form
        /// 'players_raw_yyyyMMddHHmmss.csv'.
        /// </remarks>
        /// <returns>
        /// 200 OK with a CSV file attachment when successful.
        /// 500 Internal Server Error with a short diagnostic message when an error occurs.
        /// </returns>
        [HttpGet("export/raw")]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportRawPlayerCsvAsync()
        {
            logger.LogTrace("Received request to export raw players CSV.");
            var games = await context.GetAllPlayersRawAsync();
            var records = mapper.Map<List<PlayerRawDto>>(games);
            var bytes = await GenerateCsvFileAsync(records);
            // filename of exported file with timestamp
            var fileName = $"player_raw_{time.GetLocalNow():yyyyMMddHHmmss}.csv";
            return File(bytes!, "text/csv; charset=utf-8", fileName);
        }

        private async Task<byte[]> GenerateCsvFileAsync(IEnumerable<PlayerRawDto> records)
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


        // GET: api/Player/4/PlayerRatings
        /// <summary>
        /// Retrieves ratings for the specified player.
        /// </summary>
        /// <param name="playerId">Player id whose ratings will be returned.</param>
        /// <returns>
        /// 200 OK with an array of <see cref="PlayerRatingDto"/> when the player exists.
        /// 404 NotFound when the player does not exist.
        /// </returns>
        [HttpGet("{playerId}/PlayerRatings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlayerRatingDto[]))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetPlayerRatings(int playerId)
        {
            if (!context.PlayerExists(playerId))
            {
                return NotFound();
            }
            var playerRatings = await context.GetPlayerRatingsAsync(playerId);
            var playerRatingDto = mapper.Map<PlayerRatingDto[]>(playerRatings);
            return Ok(playerRatingDto);
        }

        // GET /api/Player/{playerId}/PlayerRatings/LatestBefore?date=YYYY-MM-DD
        [HttpGet("{playerId}/PlayerRatings/LatestBefore/{date}")]
        public async Task<IActionResult> GetLatestRatingBeforeDate(int playerId, DateTime date)
        {
            var rating = await context
                .GetLatestPlayerRatingBeforeDateAsync(playerId, date)
                ;

            if (rating == null)
            {
                return NotFound();
            }
            var ratingDto = mapper.Map<PlayerRatingDto>(rating);
            return Ok(ratingDto);
        }

        // PUT: api/Player/5
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates an existing player.
        /// </summary>
        /// <param name="id">Player Id of player to update. Must match <c>playerDto.PlayerId</c>.</param>
        /// <param name="playerDto">Player data to update.</param>
        /// <returns>
        /// 204 NoContent when the update succeeds.
        /// 400 BadRequest when the route id does not match the DTO id.
        /// 404 NotFound when the player does not exist.
        /// </returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutPlayer(int id, PlayerDto playerDto)
        {
            if (id != playerDto.PlayerId)
            {
                return BadRequest();
            }
            var player = mapper.Map<Player>(playerDto);
            context.Entry(player).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.PlayerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return Conflict("The player was modified by another process. Please refresh and try again.");
                }
            }

            return NoContent();
        }

        // POST: api/Player
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="playerDto">Player data to create.</param>
        /// <returns>
        /// 201 Created with the created <see cref="PlayerDto"/>.
        /// 400 BadRequest when the supplied data is invalid.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PlayerDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(PlayerDto playerDto)
        {
            var player = mapper.Map<Player>(playerDto);
            player.ChangedTime= time.GetUtcNow().ToUnixTimeMilliseconds();
            context.Players.Add(player);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetPlayer", new { id = player.PlayerId }, playerDto);
        }

        //// DELETE: api/Player/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletePlayer(int id)
        //{
        //    var player = await _context.Players.FindAsync(id);
        //    if (player == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Players.Remove(player);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

    }
}
