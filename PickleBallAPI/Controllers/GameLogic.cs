using Microsoft.AspNetCore.Components.Web;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PickleBallAPI.Controllers
{
    public class GameLogic(TimeProvider time, ILogger<GameLogic> logger )
    {
        public string ValidateGame(GameDto gameDto)
        {
            string msg = string.Empty;
            if (gameDto == null)
            {
                msg += "Game data is null.\n";
                return msg;
            }
            if (gameDto.TeamOnePlayerOne?.PlayerId == null )
            {
                msg += "Team One Player One can not be NULL\n";
            }
            if (gameDto.TeamTwoPlayerOne?.PlayerId == null)
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
            if (gameDto.TeamTwoPlayerOne.PlayerId == gameDto.TeamTwoPlayerTwo?.PlayerId)
            {
                msg += "Team Two cannot have the same player twice.\n";
            }
            if (gameDto.TeamOnePlayerOne.PlayerId == gameDto.TeamTwoPlayerOne.PlayerId)
            {
                msg += "Team 1 first Player can not be on both teams.\n";
            }
            if (gameDto.TeamOnePlayerOne.PlayerId == gameDto.TeamTwoPlayerTwo?.PlayerId)
            {
                msg += "Team 1 first Player can not be on both teams.\n";
            }
            if (gameDto.TeamOnePlayerTwo != null &&
                (gameDto.TeamOnePlayerTwo.PlayerId == gameDto.TeamTwoPlayerOne.PlayerId ||
                 gameDto.TeamOnePlayerTwo.PlayerId == gameDto.TeamTwoPlayerTwo?.PlayerId))
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
                if (gameDto.PlayedDate > time.GetLocalNow())
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

        public string ValidateGamePlayers(VprContext ctx, GameDto gameDto)
        {
            string msg = string.Empty;
            msg += ValidatePlayer(gameDto.TeamOnePlayerOne, isRequired: true);
            msg += ValidatePlayer(gameDto.TeamOnePlayerTwo, isRequired: false);
            msg += ValidatePlayer(gameDto.TeamTwoPlayerOne, isRequired: true);
            msg += ValidatePlayer(gameDto.TeamTwoPlayerTwo, isRequired: false);
            return msg;

            string ValidatePlayer(PlayerDto? player, bool isRequired)
            {
                if (player == null)
                {
                    return isRequired ? "Required player data is null.\n" : string.Empty;
                }
                if (player.PlayerId == 0)
                {
                    return "Player Id cannot be 0.\n";
                }
                if (!ctx.PlayerExists(player.PlayerId))
                {
                    return $"Player Id, {player.PlayerId}, does not exist.\n";
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// Calculates the prediction and gathers current player ratings for the supplied game.
        /// </summary>
        /// <param name="game">The game entity to calculate prediction for.</param>
        /// <param name="context">The database context for fetching player ratings.</param>
        /// <returns>
        /// A tuple containing:
        /// - <c>GameLogic.GameRatings ratings</c>: current player ratings used for prediction.
        /// - <c>GamePrediction gamePrediction</c>: the computed prediction for the game.
        /// </returns>
        public async Task<GamePrediction> CalculatePredictionAsync(VprContext context, Game game)
        {
            var playedAt = game.PlayedDate ?? time.GetLocalNow();
            var gameId = game.GameId;
            logger.LogTrace("Calculating Game Prediction  for game {gameId} played at {playedAt}.", gameId, playedAt);

            var ratings = await GetPlayerRatingsAsync(context, game, playedAt);

            var gamePrediction = GetGamePrediction(game, playedAt, ratings);
            return gamePrediction;
        }


        public async Task<GameRatings> GetPlayerRatingsAsync(VprContext ctx, Game game, DateTimeOffset playedAt)
        {
            var tmp = new GameRatings
            {
                T1P1 = (await GetRatingAsync(game.TeamOnePlayerOneId, playedAt)).Value,
                T1P2 = await GetRatingAsync(game.TeamOnePlayerTwoId, playedAt),
                T2P1 = (await GetRatingAsync(game.TeamTwoPlayerOneId, playedAt)).Value,
                T2P2 = await GetRatingAsync(game.TeamTwoPlayerTwoId, playedAt),
            };
            return tmp;
            // Helper function to fetch rating
            async Task<int?> GetRatingAsync(int? playerId, DateTimeOffset playedAt)
            {
                if (playerId != null)
                {
                    var PlayerRating = await ctx.GetLatestPlayerRatingBeforeDateAsync(playerId.Value, playedAt);
                    return PlayerRating?.Rating ?? EloCalculator.InitialRating;
                }
                return null;
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
        public (int team1Score, int team2Score) CalculateExpectedScore(decimal expectedOutcome)
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

        private static (int, int) AssignScore(bool team1Wins, int winScore, int lossScore)
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

        public IEnumerable<PlayerRating> CalculateNewPlayerRatings(Game game)
        {
            IEnumerable<PlayerRating> tmp;
            GamePrediction gamePrediction = game.Prediction
                ?? throw new ApplicationException("Game Prediction cannot be null");
            if ( game.TeamOnePlayerTwoId is not null && game.TeamTwoPlayerTwoId is not null)
            {
                tmp = GetNewPlayerRatingsDoubles(game);
            }
            else
            {
                tmp=  GetNewPlayerRatingsSingles(game);
            }
            return tmp;
        }
        public GamePrediction GetGamePrediction(Game game, DateTimeOffset playedAt, GameRatings ratings)
        {
            decimal expectedOutcome = EloCalculator.ExpectedTeamOutcome(ratings.T1P1, ratings.T1P2, ratings.T2P1, ratings.T2P2);
            (int t1Score, int t2Score) = CalculateExpectedScore(expectedOutcome);

            var gamePrediction = game.Prediction ?? new GamePrediction();
            bool needsUpdate = false;

            needsUpdate |= UpdatePropertyIfChanged(gamePrediction.GameId, game.GameId, v => gamePrediction.GameId = v);
            gamePrediction.Game = null!;
            needsUpdate |= UpdatePropertyIfChanged(gamePrediction.CreatedAt, playedAt, v => gamePrediction.CreatedAt = v);
            needsUpdate |= UpdatePropertyIfChanged(gamePrediction.T1p1rating, ratings.T1P1, v => gamePrediction.T1p1rating = v);
            if (ratings.T1P2.HasValue)
            {
                needsUpdate |= UpdatePropertyIfChanged(gamePrediction.T1p2rating, ratings.T1P2.Value, v => gamePrediction.T1p2rating = v);
            }
            needsUpdate |= UpdatePropertyIfChanged(gamePrediction.T2p1rating, ratings.T2P1, v => gamePrediction.T2p1rating = v);
            if (ratings.T2P2.HasValue)
            {
                needsUpdate |= UpdatePropertyIfChanged(gamePrediction.T2p2rating, ratings.T2P2.Value, v => gamePrediction.T2p2rating = v);
            }
            needsUpdate |= UpdatePropertyIfChanged(gamePrediction.T1predictedWinProb, expectedOutcome, v => gamePrediction.T1predictedWinProb = v);
            needsUpdate |= UpdatePropertyIfChanged(gamePrediction.ExpectT1score, t1Score, v => gamePrediction.ExpectT1score = v);
            needsUpdate |= UpdatePropertyIfChanged(gamePrediction.ExpectT2score, t2Score, v => gamePrediction.ExpectT2score = v);

            if (needsUpdate)
            {
                gamePrediction.ChangedTime = time.GetUtcNow().ToUnixTimeMilliseconds();
            }

            return gamePrediction;

            static bool UpdatePropertyIfChanged<T>(T currentValue, T newValue, Action<T> setter)
            {
                if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                {
                    return false;
                }

                setter(newValue);
                return true;
            }
        }

        public IEnumerable<PlayerRating> GetNewPlayerRatingsDoubles(Game game)
        {
            if (game.PlayedDate == null)
            {
                return [];
            }
            GamePrediction gamePrediction = game.Prediction
                ?? throw new ArgumentException("Game Prediction cannot be null");
            var changedToken = time.GetUtcNow().ToUnixTimeMilliseconds();
            var kFactor = EloCalculator.CalculateKFactor(game.Prediction.T1p1rating);
            double t1ActualOutcome = (double)game.TeamOneScore! / (double)(game.TeamTwoScore + game.TeamOneScore)!;
            (int newP1Rating, int newP2Rating) = EloCalculator.CalculateNewRatingDoubles(gamePrediction.T1p1rating, gamePrediction.T1p2rating, gamePrediction.T1predictedWinProb, (decimal)t1ActualOutcome, kFactor);
            (int newP3Rating, int newP4Rating) = EloCalculator.CalculateNewRatingDoubles(gamePrediction.T2p1rating, gamePrediction.T2p2rating, 1.0m - gamePrediction.T1predictedWinProb, (decimal)(1.0 - t1ActualOutcome), kFactor);
            List<PlayerRating> newRatings =
            [
                new()
                {
                    ChangedTime = changedToken,
                    PlayerId = game.TeamOnePlayerOneId,
                    Rating = newP1Rating,
                    RatingDate = (DateTimeOffset)game.PlayedDate,
                    GameId = game.GameId,
                },
                new()
                {
                    ChangedTime = changedToken,
                    PlayerId = game.TeamOnePlayerTwoId!.Value,
                    Rating = newP2Rating,
                    RatingDate = (DateTimeOffset)game.PlayedDate,
                    GameId = game.GameId,
                },
                new()
                {
                    ChangedTime = changedToken,
                    PlayerId = game.TeamTwoPlayerOneId,
                    Rating = newP3Rating,
                    RatingDate = (DateTimeOffset)game.PlayedDate,
                    GameId = game.GameId,
                },
                new()
                {
                    ChangedTime = changedToken,
                    PlayerId = game.TeamTwoPlayerTwoId!.Value,
                    Rating = newP4Rating,
                    RatingDate = (DateTimeOffset)game.PlayedDate,
                    GameId = game.GameId,
                },
            ];
            return newRatings;
        }
        public IEnumerable<PlayerRating> GetNewPlayerRatingsSingles(Game game)
        {
            if (game.PlayedDate == null)
            {
                return [];
            }
            GamePrediction gamePrediction = game.Prediction
                ?? throw new ArgumentException("Game Prediction cannot be null");
            var changedToken = time.GetUtcNow().ToUnixTimeMilliseconds();
            var kFactor = EloCalculator.CalculateKFactor(gamePrediction.T1p1rating);
            double t1ActualOutcome = (double)game.TeamOneScore! / (double)(game.TeamTwoScore + game.TeamOneScore)!;
            var newP1Rating = EloCalculator.CalculateNewRatingSingles(gamePrediction.T1p1rating, gamePrediction.T1predictedWinProb, (decimal)t1ActualOutcome, kFactor);
            var newP3Rating = EloCalculator.CalculateNewRatingSingles(gamePrediction.T2p1rating, 1.0m - gamePrediction.T1predictedWinProb, (decimal)(1.0 - t1ActualOutcome), kFactor);
            List<PlayerRating> newRatings =
            [
                new()
                {
                    ChangedTime = changedToken,
                    PlayerId = game.TeamOnePlayerOneId,
                    Rating = newP1Rating,
                    RatingDate = (DateTimeOffset)game.PlayedDate,
                    GameId = game.GameId,
                },
                new()
                {
                    ChangedTime = changedToken,
                    PlayerId = game.TeamTwoPlayerOneId,
                    Rating = newP3Rating,
                    RatingDate = (DateTimeOffset)game.PlayedDate,
                    GameId = game.GameId,
                },
            ];
            return newRatings;
        }


        public class GameRatings
        {
            public int T1P1 { get; set; }
            public int? T1P2 { get; set; }
            public int T2P1 { get; set; }
            public int? T2P2 { get; set; }
        }

    }


}
