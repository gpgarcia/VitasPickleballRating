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
    public class PlayerRatingsController : ControllerBase
    {
        private readonly VprContext _context;
        private readonly IMapper _mapper;

        public PlayerRatingsController(VprContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/PlayerRatings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetPlayerRatings()
        {
            var ratings = await _context
                .PlayerRatings
                .ToListAsync()
                ;
            PlayerRatingDto[] ratingDtos = _mapper.Map<PlayerRatingDto[]>(ratings);
            return ratingDtos;
        }

        // GET: api/PlayerRatings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerRatingDto>> GetPlayerRating(int id)
        {
            var playerRating = await _context
                .PlayerRatings
                .Include(r => r.Player)
                .FirstOrDefaultAsync(r => r.PlayerRatingId == id)
                ;

            if (playerRating == null)
            {
                return NotFound();
            }
            var player = await _context.Players.FindAsync(playerRating.PlayerId);

            var ratingDto = _mapper.Map<PlayerRatingDto>(playerRating);
            return ratingDto;
        }

        // PUT: api/PlayerRatings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayerRating(int id, PlayerRatingDto playerRatingDto)
        {
            if (id != playerRatingDto.PlayerRatingId)
            {
                return BadRequest();
            }
            var playerRating = _mapper.Map<PlayerRating>(playerRatingDto);
            _context.Entry(playerRating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerRatingExists(id))
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

        // POST: api/PlayerRatings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlayerRatingDto>> PostPlayerRating(PlayerRatingDto playerRatingDto)
        {
            var playerRating = _mapper.Map<PlayerRating>(playerRatingDto);
            _context.PlayerRatings.Add(playerRating);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayerRating", new { id = playerRating.PlayerRatingId }, playerRating);
        }

        // DELETE: api/PlayerRatings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerRating(int id)
        {
            var playerRating = await _context.PlayerRatings.FindAsync(id);
            if (playerRating == null)
            {
                return NotFound();
            }

            _context.PlayerRatings.Remove(playerRating);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerRatingExists(int id)
        {
            return _context.PlayerRatings.Any(e => e.PlayerRatingId == id);
        }
    }
}
