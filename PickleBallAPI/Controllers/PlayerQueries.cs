using Microsoft.EntityFrameworkCore;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickleBallAPI.Controllers
{
    public static class PlayerQueries
    {
        public static async Task<Player[]> GetAllPlayersAsync(this VprContext context)
        {
            var players = await context
                .Players
                .ToArrayAsync()
                ;
            return players;
        }

        public static async Task<Player[]> GetAllPlayersRawAsync(this VprContext context)
        {
            var players = await context
                .Players
                .ToArrayAsync()
                ;
            return players;
        }


        public static async Task<Player?> GetPlayerByIdAsync(this VprContext context, int playerId)
        {
            var player = await context
                .Players
                .Include(p=>p.PlayerRatings)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId)
                ;
            return player;
        }

        public static async Task<PlayerRating[]> GetPlayerRatingsAsync(this VprContext context, int playerId)
        {
            var playerRatings = await context
                .PlayerRatings
                .Where(pr => pr.PlayerId == playerId)
                .ToListAsync()
                ;
            // Deal with date handling issues
            playerRatings.Sort(new DateOnlyComparer());
            return playerRatings.ToArray();
        }

        class DateOnlyComparer : IComparer<PlayerRating>
        {
            public int Compare(PlayerRating? x, PlayerRating? y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return x.RatingDate.CompareTo(y.RatingDate);
            }
        }

        public static async Task<PlayerRating?> GetLatestPlayerRatingBeforeDateAsync(this VprContext context, int playerId, DateTimeOffset date)
        {
            var playerRatings = await context.PlayerRatings
                .Where(r => r.PlayerId == playerId)
                .Include("Player")
                .ToListAsync()
                ;
            // deal with date handling limitations
            var rating = playerRatings
                .OrderByDescending(r => r.RatingDate)
                .FirstOrDefault(r => r.RatingDate < date)
                ;
            return rating;
        }

        public static bool PlayerExists(this VprContext context, int id)
        {
            return context.Players.Any(e => e.PlayerId == id);
        }

    }
}
