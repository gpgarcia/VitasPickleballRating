using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickleBallAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerRatingsController(VprContext context, IMapper mapper, TimeProvider time) : ControllerBase
    {
        // GET: api/PlayerRatings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetPlayerRatings()
        {
            var ratings = await context
                .PlayerRatings
                .ToListAsync()
                ;
            PlayerRatingDto[] ratingDtos = mapper.Map<PlayerRatingDto[]>(ratings);
            return Ok(ratingDtos);
        }

        // GET: api/PlayerRatings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerRatingDto>> GetPlayerRating(int id)
        {
            var playerRating = await context
                .PlayerRatings
                .Include(r => r.Player)
                .FirstOrDefaultAsync(r => r.PlayerRatingId == id)
                ;

            if (playerRating == null)
            {
                return NotFound();
            }
            var player = await context.Players.FindAsync(playerRating.PlayerId);

            var ratingDto = mapper.Map<PlayerRatingDto>(playerRating);
            return Ok(ratingDto);
        }

        // PUT: api/PlayerRatings/5
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayerRating(int id, PlayerRatingDto playerRatingDto)
        {
            if (id != playerRatingDto.PlayerRatingId)
            {
                return BadRequest("Id mismatch");
            }

            if (!PlayerRatingExists(id))
            {
                return NotFound($"No Player Rating with PlayerRatingId={id}");
            }

            var playerRating = mapper.Map<PlayerRating>(playerRatingDto);
            context.Entry(playerRating).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/PlayerRatings
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlayerRatingDto>> PostPlayerRating(PlayerRatingDto playerRatingDto)
        {
            var playerRating = mapper.Map<PlayerRating>(playerRatingDto);
            playerRating.ChangedTime = time.GetUtcNow().ToUnixTimeMilliseconds();
            context.PlayerRatings.Add(playerRating);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetPlayerRating", new { id = playerRating.PlayerRatingId }, playerRating);
        }

        // DELETE: api/PlayerRatings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerRating(int id)
        {
            var playerRating = await context.PlayerRatings.FindAsync(id);
            if (playerRating == null)
            {
                return NotFound();
            }

            context.PlayerRatings.Remove(playerRating);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerRatingExists(int id)
        {
            return context.PlayerRatings.Any(e => e.PlayerRatingId == id);
        }
    }
}
