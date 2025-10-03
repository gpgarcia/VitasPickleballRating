using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PickleBallAPI.Models;

namespace PickleBallAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly VprContext _context;
        private readonly IMapper _mapper;

        public PlayersController(VprContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers()
        {
            var players = await _context.GetAllPlayersAsync();
            PlayerDto[] playerDtos = _mapper.Map<PlayerDto[]>(players);
            return Ok(playerDtos);

        }

        // GET: api/Players/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(int id)
        {
            var player = await _context.GetPlayerByIdAsync(id);

            if (player == null)
            {
                return NotFound();
            }
            var playerDto = _mapper.Map<PlayerDto>(player);
            return Ok(playerDto);
        }

        // GET: api/Players/4/PlayerRatings
        [HttpGet("{playerId}/PlayerRatings")]
        public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetPlayerRatings(int playerId)
        {
            if (!_context.PlayerExists(playerId))
            {
                return NotFound();
            }
            var playerRatings = await _context.GetPlayerRatingsAsync(playerId);
            var playerRatingDto = _mapper.Map<PlayerRatingDto[]>(playerRatings);
            return Ok(playerRatingDto);
        }

        // GET /api/Players/{playerId}/PlayerRatings/LatestBefore?date=YYYY-MM-DD
        [HttpGet("{playerId}/PlayerRatings/LatestBefore/{date}")]
        public async Task<IActionResult> GetLatestRatingBeforeDate(int playerId, DateTime date)
        {
            var rating = await _context.GetLatestPlayerRatingBeforeDateAsync(playerId, date);

            if (rating == null)
            {
                return NotFound();
            }
            return Ok(rating);
        }

        // PUT: api/Players/5
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayer(int id, PlayerDto playerDto)
        {
            if (id != playerDto.PlayerId)
            {
                return BadRequest();
            }
            var player = _mapper.Map<Player>(playerDto);
            _context.Entry(player).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PlayerExists(id))
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

        // POST: api/Players
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(PlayerDto playerDto)
        {
            var player = _mapper.Map<Player>(playerDto);
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayer", new { id = player.PlayerId }, playerDto);
        }

        //// DELETE: api/Players/5
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
