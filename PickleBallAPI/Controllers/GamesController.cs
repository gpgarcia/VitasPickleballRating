using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PickleBallAPI.Models;
using AutoMapper;

namespace PickleBallAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly VprContext _context;
        private readonly IMapper _mapper;

        public GamesController(VprContext context, IMapper mapper )
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames()
        {
            var games = await _context
                .Games
                .Include(g => g.TeamOne)
                .Include(g => g.TypeGame)
                .Include(g => g.TeamOne.PlayerOne)
                .Include(g => g.TeamOne.PlayerTwo)
                .Include(g => g.TeamTwo)
                .Include(g => g.TeamTwo.PlayerOne)
                .Include(g => g.TeamTwo.PlayerTwo)
                .ToListAsync();

            GameDto[] gameDtos = _mapper.Map<IEnumerable<Game>, GameDto[]>(games);
            return gameDtos;
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGame(int id)
        {
            var game = await _context
                .Games
                .Include(g => g.TeamOne)
                .Include(g => g.TypeGame)
                .Include(g => g.TeamOne.PlayerOne)
                .Include(g => g.TeamOne.PlayerTwo)
                .Include(g => g.TeamTwo)
                .Include(g => g.TeamTwo.PlayerOne)
                .Include(g => g.TeamTwo.PlayerTwo)
                .FirstOrDefaultAsync(g => g.GameId == id)
                ;
            if (game == null)
            {
                return NotFound();
            }

            return _mapper.Map<GameDto>(game);
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameDto gameDto)
        {
            if (id != gameDto.GameId)
            {
                return BadRequest();
            }
            var game = _mapper.Map<Game>(gameDto);

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
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

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameDto>> PostGame(GameDto gameDto)
        {
            var game = _mapper.Map<Game>(gameDto);
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.GameId }, gameDto);
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

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.GameId == id);
        }
    }
}
