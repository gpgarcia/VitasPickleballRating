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
            var players = await _context
                .Players
                .ToListAsync()
                ;
            PlayerDto[] playerDtos = _mapper.Map<PlayerDto[]>(players);
            return playerDtos;

        }

        // GET: api/Players/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(int id)
        {
            var player = await _context
                .Players
                .FirstOrDefaultAsync(p=>p.PlayerId == id)
                ;

            if (player == null)
            {
                return NotFound();
            }
            var playerDto = _mapper.Map<PlayerDto>(player);
            return playerDto;
        }

        // GET: api/Players/4/PlayerRatings
        [HttpGet("{playerId}/PlayerRatings")]
        public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetPlayerRatings(int playerId)
        {
            var playerRatings = await _context
                .PlayerRatings
                .Where(p => p.PlayerId == playerId)
                .ToListAsync()
                ;

            if (playerRatings == null)
            {
                return NotFound();
            }
            var playerRatingDto = _mapper.Map<PlayerRatingDto[]>(playerRatings);
            return playerRatingDto;
        }

        // PUT: api/Players/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
                if (!PlayerExists(id))
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.PlayerId == id);
        }
    }
}
