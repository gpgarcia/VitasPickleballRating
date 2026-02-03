using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PickleBallAPI.Controllers
{
    public static class GameLogic
    {
        public static string ValidateGame(GameDto gameDto)
        {
            string msg = string.Empty;
            if (gameDto == null)
            {
                msg += "Game data is null.\n";
                return msg;
            }
            if (gameDto.TeamOnePlayerOne == null)
            {
                msg += "Team One Player One can not be NULL\n";
            }
            if (gameDto.TeamTwoPlayerOne == null)
            {
                msg += "Team Two Player One can not be NULL\n";
            }
            if (gameDto.TeamOnePlayerOne == null || gameDto.TeamTwoPlayerOne == null)
            {
                return msg;
            }
            if (gameDto.TeamOnePlayerOne.PlayerId == gameDto.TeamOnePlayerTwo?.PlayerId)
            {
                msg += "Team One cannot have the same player twice.\n";
            }
            if (gameDto.TeamTwoPlayerOne.PlayerId == gameDto.TeamTwoPlayerTwo.PlayerId)
            {
                msg += "Team Two cannot have the same player twice.\n";
            }
            if (gameDto.TeamOnePlayerOne.PlayerId == gameDto.TeamTwoPlayerOne.PlayerId)
            {
                msg += "Team 1 first Player can not be on both teams.\n";
            }
            if ( gameDto.TeamOnePlayerOne.PlayerId == gameDto.TeamTwoPlayerTwo.PlayerId)
            {
                msg += "Team 1 first Player can not be on both teams.\n";
            }
            if (gameDto.TeamOnePlayerTwo != null &&
                (gameDto.TeamOnePlayerTwo.PlayerId == gameDto.TeamTwoPlayerOne.PlayerId ||
                 gameDto.TeamOnePlayerTwo.PlayerId == gameDto.TeamTwoPlayerTwo.PlayerId))
            {
                msg += "Player can not be on both teams.\n";
            }
            if (gameDto.TeamOneScore.HasValue && gameDto.TeamTwoScore.HasValue)
            {
                if (gameDto.TeamOneScore < 0 || gameDto.TeamTwoScore < 0)
                {
                    msg += "Scores can not be negative.\n";
                }
                if (gameDto.TeamOneScore == gameDto.TeamTwoScore)
                {
                    msg += "Scores can not be tied.\n";
                }
                if (gameDto.PlayedDate == null)
                {
                    msg += "If scores are provided, a played date must be provided.\n";
                }
            }
            if (gameDto.PlayedDate != null)
            {
                if (!gameDto.TeamOneScore.HasValue || !gameDto.TeamTwoScore.HasValue)
                {
                    msg += "If a played date is provided, both scores must be provided.\n";
                }
                if (gameDto.PlayedDate > DateTimeOffset.Now)
                {
                    msg += "Played date can not be in the future.\n";
                }
            }
            if (gameDto.PlayedDate == null)
            {
                if (gameDto.TeamOneScore.HasValue || gameDto.TeamTwoScore.HasValue)
                {
                    msg += "If date is not provide, then both score must be NULL\n";
                }
            }
            if (gameDto.TypeGameId == 0)
            {
                msg += "Valid TypeGameId must be present.\n";
            }

            return msg;
        }

        public static string ValidateGamePlayers(VprContext ctx, GameDto gameDto)
        {
            string msg = string.Empty;
            msg += ValidatePlayer(gameDto.TeamOnePlayerOne.PlayerId);
            msg += ValidatePlayer(gameDto.TeamOnePlayerTwo.PlayerId);
            msg += ValidatePlayer(gameDto.TeamTwoPlayerOne.PlayerId);
            msg += ValidatePlayer(gameDto.TeamTwoPlayerTwo.PlayerId);
            return msg;

            string ValidatePlayer(int? playerId)
            {
                string msg = string.Empty;
                if (playerId == null)
                {
                    msg = $"Player Id, canot be null.\n";
                }   
                else if (!ctx.PlayerExists(playerId.Value))
                {
                    msg = $"Player Id, {playerId}, does not exist.\n";
                }
                return msg;
            }
        }

        public static async Task<GameRatings> GetPlayerRatingsAsync(VprContext ctx, Game game, DateTimeOffset playedAt)
        {
            return new GameRatings
            {
                T1P1 = await GetRating(game.TeamOnePlayerOneId, playedAt),
                T1P2 = await GetRating(game.TeamOnePlayerTwoId, playedAt),
                T2P1 = await GetRating(game.TeamTwoPlayerOneId, playedAt),
                T2P2 = await GetRating(game.TeamTwoPlayerTwoId, playedAt),
            };
            // Helper function to fetch rating
            async Task<int> GetRating(int playerId, DateTimeOffset playedAt)
            {
                var PlayerRating = await ctx.GetLatestPlayerRatingBeforeDateAsync(playerId, playedAt);
                return PlayerRating?.Rating ?? EloCalculator.InitialRating;
            }
        }

        /// <summary>
        /// Given the expected outcome, calculate the winning and losing score. 
        /// Based on games to 11 win by 2 or first to 15
        /// </summary>
        /// <param name="expectedOutcome">Probability of victory</param>
        /// <returns>A game score tuple</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="expectedOutcome"/> is less than 0.0 or greater than 1.0.
        /// </exception>
        public static (int team1Score, int team2Score) CalculateExpectedScore(decimal expectedOutcome)
        {
            var team1Wins = true;
            if (expectedOutcome < 0.0m || expectedOutcome > 1.0m)
                throw new ArgumentOutOfRangeException(nameof(expectedOutcome), "Expected outcome must be between 0 and 1.");
            if (expectedOutcome < 0.5m)
            {
                // team one looses
                team1Wins = false;
                expectedOutcome = 1 - expectedOutcome;
            }
            int winScore = 15;
            int lossScore = 14;
            var winningScoreList = new List<int> { 11, 12, 13, 14, 15 };
            foreach (int w in winningScoreList)
            {
                winScore = w;
                var l = winScore * (1.0m / expectedOutcome - 1.0m);
                lossScore = (int)Math.Round(l);
                if (Math.Abs(l - lossScore) < 0.5m && winScore - lossScore >= 2)
                {
                    break;
                }
                if (w == 15 && lossScore > 14)
                {
                    winScore = 15;
                    lossScore = 14;
                }
            }
            int team1Score, team2Score;
            (team1Score, team2Score) = AssignScore(team1Wins, winScore, lossScore);
            return (team1Score, team2Score);
        }

        private static (int,int) AssignScore(bool team1Wins, int winScore, int lossScore)
        {
            int team1Score;
            int team2Score;

            if (team1Wins)
            {
                (team1Score, team2Score) = (winScore, lossScore);
            }
            else
            {
                (team1Score, team2Score) = (lossScore, winScore);
            }
            return (team1Score, team2Score);
        }

        public static IEnumerable<PlayerRating> CalculateNewPlayerRatings(Game game, GameRatings ratings)
        {
            GamePrediction gamePrediction = game.GamePrediction 
                ?? throw new ApplicationException("Game Prediction cannot be null");
            List<PlayerRating> newRatings = [];
            if (game.PlayedDate != null)
            {
                var PlayerRatings = GetNewPlayerRatings(game, gamePrediction, ratings);
                foreach (var pr in PlayerRatings)
                {
                    newRatings.Add(pr);
                }
            }
            return newRatings;
        }
        public static GamePrediction GetGamePrediction(int gameId, DateTimeOffset playedAt, GameRatings ratings)
        {
            decimal expectedOutcome = EloCalculator.ExpectedTeamOutcome2(ratings.T1P1, ratings.T1P2, ratings.T2P1, ratings.T2P2);
            (int t1Score, int t2Score) = CalculateExpectedScore(expectedOutcome);
            var gamePrediction = new GamePrediction
            {
                GameId = gameId,
                Game = null!,
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
                var changedToken = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                //var changedToken = DateTimeOffset.UtcNow;
                // Update player ratings based on game result
                var kFactor = EloCalculator.CalculateKFactor(ratings.T1P1);
                //if there is a date scores are not null
                double T1ActualOutcome = (double)game.TeamOneScore! / (double)(game.TeamTwoScore + game.TeamOneScore)!;
                (int newP1Rating, int newP2rating) = EloCalculator.CalculateNewRating(ratings.T1P1, ratings.T1P2, gamePrediction.T1predictedWinProb, (decimal)T1ActualOutcome, kFactor);
                (int newP3Rating, int newP4rating) = EloCalculator.CalculateNewRating(ratings.T2P1, ratings.T2P2, 1.0m - gamePrediction.T1predictedWinProb, (decimal)(1.0 - T1ActualOutcome), kFactor);
                tmp =
                [
                    new()
                    {
                        ChangedTime = changedToken,
                        PlayerId = game.TeamOnePlayerOneId,
                        Rating = newP1Rating,
                        RatingDate = (DateTimeOffset)game.PlayedDate!,
                        GameId = game.GameId,
                    },
                    new()
                    {
                        ChangedTime = changedToken,
                        PlayerId = game.TeamOnePlayerTwoId,
                        Rating = newP2rating,
                        RatingDate = (DateTimeOffset)game.PlayedDate!,
                        GameId = game.GameId,
                    },
                    new()
                    {
                        ChangedTime = changedToken,
                        PlayerId = game.TeamTwoPlayerOneId,
                        Rating = newP3Rating,
                        RatingDate = (DateTimeOffset)game.PlayedDate!,
                        GameId = game.GameId,
                    },
                    new()
                    {
                        ChangedTime = changedToken,
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
