using Microsoft.EntityFrameworkCore;
using PickleBallAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickleBallAPI.Controllers;

public static class GameQueries
{
    public static bool GameExists(this VprContext context, int id)
    {
        return context.Games.Any(e => e.GameId == id);
    }

    public static async Task<IEnumerable<Game>> GetAllGamesRawAsync(this VprContext context)
    {
        var tmp = await context
            .Games
            .ToListAsync()
            ;
        return tmp;
    }
    public static async Task<IEnumerable<Game>> GetAllGamesAsync(this VprContext context)
    {
        var tmp = await context
            .Games
            .Include(g => g.TypeGame)
            .Include(g => g.TeamOnePlayerOne)
            .Include(g => g.TeamOnePlayerTwo)
            .Include(g => g.TeamTwoPlayerOne)
            .Include(g => g.TeamTwoPlayerTwo)
            .Include(g => g.Facility)
            .ToListAsync()
            ;
        return tmp;
    }

    public static async Task<Game?> GetGameAsync(this VprContext context, int id)
    {
        var game = await context
            .Games
            .Include(g => g.TypeGame)
            .Include(g => g.TeamOnePlayerOne)
            .Include(g => g.TeamOnePlayerTwo)
            .Include(g => g.TeamTwoPlayerOne)
            .Include(g => g.TeamTwoPlayerTwo)
            .Include(g => g.GamePrediction)
            .Include(g => g.Facility)
            .Include(g => g.Facility!.TypeFacility)
            .FirstOrDefaultAsync(g => g.GameId == id)
            ;
        return game;
    }
}
