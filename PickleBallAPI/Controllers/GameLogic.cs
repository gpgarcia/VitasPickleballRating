using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickleBallAPI.Controllers
{
    public static class GameLogic
    {
        private const int MinimumRating = 200;

        public static string ValidateGame(GameDto gameDto)
        {
            if (gameDto == null)
            {
                return "Game data is null.\n";
            }
            if (gameDto.TeamOnePlayerOne == null)
            {
                return "Team One Player One can not be NULL";
            }
            if (gameDto.TeamTwoPlayerOne == null)
            {
                return "Team Two Player One can not be NULL";
            }
            if (gameDto.TeamOnePlayerOne.PlayerId == gameDto.TeamOnePlayerTwo.PlayerId)
            {
                return "Team One cannot have the same player twice.";
            }
            if (gameDto.TeamTwoPlayerOne.PlayerId == gameDto.TeamTwoPlayerTwo.PlayerId)
            {
                return "Team Two cannot have the same player twice.";
            }
            if (gameDto.TeamOnePlayerOne.PlayerId == gameDto.TeamTwoPlayerOne.PlayerId ||
                gameDto.TeamOnePlayerOne.PlayerId == gameDto.TeamTwoPlayerTwo.PlayerId)
            {
                return "Player can not be on both teams.";
            }
            if (gameDto.TeamOnePlayerTwo != null &&
                (gameDto.TeamOnePlayerTwo.PlayerId == gameDto.TeamTwoPlayerOne.PlayerId ||
                 gameDto.TeamOnePlayerTwo.PlayerId == gameDto.TeamTwoPlayerTwo.PlayerId))
            {
                return "Player can not be on both teams.";
            }
            if (gameDto.TeamOneScore.HasValue && gameDto.TeamTwoScore.HasValue)
            {
                if (gameDto.TeamOneScore < 0 || gameDto.TeamTwoScore < 0)
                {
                    return "Scores can not be negative.";
                }
                if (gameDto.TeamOneScore == gameDto.TeamTwoScore)
                {
                    return "Scores can not be tied.";
                }
                if (gameDto.PlayedDate == null)
                {
                    return "If scores are provided, a played date must be provided.";
                }
            }
            return string.Empty;
        }

        public static string ValidateGamePlayers(VprContext ctx, GameDto gameDto)
        {
            string msg = string.Empty;
            msg += ValidatePlayer(gameDto.TeamOnePlayerOne.PlayerId);
            msg += ValidatePlayer(gameDto.TeamOnePlayerTwo.PlayerId);
            msg += ValidatePlayer(gameDto.TeamTwoPlayerOne.PlayerId);
            msg += ValidatePlayer(gameDto.TeamTwoPlayerTwo.PlayerId);
            return msg;

            string ValidatePlayer(int playerId)
            {
                string msg = string.Empty;
                if (!ctx.PlayerExists(playerId))
                {
                    msg = $"Player Id, {playerId}, does not exist.\n";
                }
                return msg;
            }
        }

        public static async Task<GameRatings> GetPlayerRatings(VprContext ctx, Game game, DateTimeOffset playedAt)
        {
            return new GameRatings
            {
                T1P1 = await GetRating(game.TeamOnePlayerOneId, playedAt),
                T1P2 = await GetRating(game.TeamOnePlayerOneId, playedAt),
                T2P1 = await GetRating(game.TeamOnePlayerOneId, playedAt),
                T2P2 = await GetRating(game.TeamOnePlayerOneId, playedAt),
            };
            // Helper function to fetch rating
            async Task<int> GetRating(int playerId, DateTimeOffset playedAt)
            {
                var PlayerRating = await ctx.GetLatestPlayerRatingBeforeDateAsync(playerId, playedAt);
                return PlayerRating?.Rating ?? MinimumRating;
            }
        }

        /// <summary>
        /// Given the expected outcome, calculate the winning and losing score. 
        /// Based on games to 11 win by 2 or first to 15
        /// </summary>
        /// <param name="expectedOutcome"> Probability of victory</param>
        /// <returns> a game score tuple</returns>
        /// <exception cref="ArgumentOutOfRangeException"> when  0.0 <=expected outcome <= 1.0</exception>
        public static (int winScore, int lossScore) CalculateExpectedScore(double expectedOutcome)
        {
            if (expectedOutcome < 0.0 || expectedOutcome > 1.0)
                throw new ArgumentOutOfRangeException(nameof(expectedOutcome), "Expected outcome must be between 0 and 1.");
            int winScore;
            int lossScore;
            if (expectedOutcome >= 0.550)
            {
                winScore = (int)Math.Round(20 * expectedOutcome);
                lossScore = (int)Math.Round(20 * (1 - expectedOutcome));
            }
            else if (expectedOutcome >= 0.545)
            {
                winScore = 12;
                lossScore = 10;
            }
            else if (expectedOutcome >= 0.542)
            {
                winScore = 13;
                lossScore = 11;
            }
            else if (expectedOutcome >= 0.538)
            {
                winScore = 14;
                lossScore = 12;
            }
            else if (expectedOutcome >= 0.536)
            {
                winScore = 15;
                lossScore = 13;
            }
            else
            {
                winScore = 15;
                lossScore = 14;
            }

            return (winScore, lossScore);

        }

        public static IEnumerable<PlayerRating> CalculateNewPlayerRatings(Game game, GameLogic.GameRatings ratings, GamePrediction gamePrediction)
        {
            List<PlayerRating> newRatings = [];
            if (game.PlayedDate != null)
            {
                var PlayerRatings = GameLogic.GetNewPlayerRatings(game, gamePrediction, ratings);
                foreach (var pr in PlayerRatings)
                {
                    newRatings.Add(pr);
                }
            }
            return newRatings;
        }
        public static GamePrediction GetGamePrediction(int gameId, DateTimeOffset playedAt, GameRatings ratings)
        {
            double expectedOutcome = EloCalculator.ExpectedTeamOutcome(ratings.T1P1, ratings.T1P2, ratings.T2P1, ratings.T2P2);
            (int t1Score, int t2Score) = GameLogic.CalculateExpectedScore(expectedOutcome);
            var gamePrediction = new GamePrediction
            {
                GameId = gameId,
                CreatedAt = playedAt,
                T1p1rating = ratings.T1P1,
                T1p2rating = ratings.T1P2,
                T2p1rating = ratings.T2P1,
                T2p2rating = ratings.T2P2,
                T1predictedWinProb = expectedOutcome,
                ExpectT1score = t1Score,
                ExpectT2score = t2Score,
            };
            return gamePrediction;
        }

        public static IEnumerable<PlayerRating> GetNewPlayerRatings(Game game, GamePrediction gamePrediction, GameRatings ratings)
        {
            List<PlayerRating> tmp = [];
            if (game.PlayedDate != null)
            {
                // Update player ratings based on game result
                var kFactor = EloCalculator.CalculateKFactor(ratings.T1P1);
                //if there is a date scores are not null
                double T1ActualOutcome = (double)game.TeamOneScore! / (double)(game.TeamTwoScore + game.TeamOneScore)!;
                (int newP1Rating, int newP2rating) = EloCalculator.CalculateNewRating(ratings.T1P1, ratings.T1P2, gamePrediction.T1predictedWinProb, T1ActualOutcome, kFactor);
                (int newP3Rating, int newP4rating) = EloCalculator.CalculateNewRating(ratings.T2P1, ratings.T2P2, 1.0 - gamePrediction.T1predictedWinProb, 1.0 - T1ActualOutcome, kFactor);
                tmp =
                [
                    new()
                    {
                        PlayerId = game.TeamOnePlayerOneId,
                        Rating = newP1Rating,
                        RatingDate = (DateTimeOffset)game.PlayedDate!,
                        GameId = game.GameId,
                    },
                    new()
                    {
                        PlayerId = game.TeamOnePlayerTwoId,
                        Rating = newP2rating,
                        RatingDate = (DateTimeOffset)game.PlayedDate!,
                        GameId = game.GameId,
                    },
                    new()
                    {
                        PlayerId = game.TeamTwoPlayerOneId,
                        Rating = newP3Rating,
                        RatingDate = (DateTimeOffset)game.PlayedDate!,
                        GameId = game.GameId,
                    },
                    new()
                    {
                        PlayerId = game.TeamTwoPlayerTwoId,
                        Rating = newP4rating,
                        RatingDate = (DateTimeOffset)game.PlayedDate!,
                        GameId = game.GameId,
                    },
                ];
            }
            return tmp;
        }

        public class GameRatings
        {
            public int T1P1 { get; set; }
            public int T1P2 { get; set; }
            public int T2P1 { get; set; }
            public int T2P2 { get; set; }
        }

    }


}
