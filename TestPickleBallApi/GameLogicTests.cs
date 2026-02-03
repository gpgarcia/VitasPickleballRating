using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PickleBallAPI.Controllers;
using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;

namespace TestPickleBallApi
{
    [TestClass]
    public class GameLogicTests
    {
        SqliteConnection _connection = null!;
        private VprContext _ctx = null!;
        private ILoggerFactory _loggerFactory = null!;
        [TestInitialize]
        public void Setup()
        {
            // This method is called before each test method.
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                // Set default minimum log level for all categories
                builder.SetMinimumLevel(LogLevel.Trace);
                // Set a specific log level for a category using a wildcard
                builder.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.Warning);

            });
            _connection = new SqliteConnection("DataSource=:memory:");
            var vprOpt = new DbContextOptionsBuilder<VprContext>()
                .UseLoggerFactory(_loggerFactory)
                .UseSqlite(_connection)
                .Options;
            _connection.Open();
            _ctx = new VprContext(vprOpt);
            _ctx.Database.EnsureCreated();


            // Seed ctx with test data
            using var setupCtx = new VprContext(vprOpt);
            var players = new List<Player>
            {
                new () { PlayerId = 1, FirstName = "Player", LastName = "One", ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},
                new () { PlayerId = 2, FirstName = "Player", LastName = "Two", ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                new () { PlayerId = 3, FirstName = "Player", LastName = "Three", ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                new () { PlayerId = 4, FirstName = "Player", LastName = "Four", ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
            };
            setupCtx.Players.AddRange(players);
            var typeGames = new List<TypeGame>
            {
                new () { TypeGameId = 1, Name = "Recreational" },
                new () { TypeGameId = 2, Name = "Tournament" },
            };
            setupCtx.TypeGames.AddRange(typeGames);
            var games = new List<Game>
            {
                new () { GameId = 1, PlayedDate = DateTimeOffset.Now.AddDays(-8), TypeGameId=1
                , TeamOnePlayerOneId = 1, TeamOnePlayerTwoId = 2, TeamTwoPlayerOneId = 3, TeamTwoPlayerTwoId = 4
                , TeamOnePlayerOne=null!, TeamOnePlayerTwo=null!, TeamTwoPlayerOne=null!, TeamTwoPlayerTwo=null!
                ,TeamOneScore = 11, TeamTwoScore = 8
                , ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                },
                new () { GameId = 2, PlayedDate = DateTimeOffset.Now.AddDays(-6), TypeGameId=1
                , TeamOnePlayerOneId = 1, TeamOnePlayerTwoId = 3, TeamTwoPlayerOneId = 2, TeamTwoPlayerTwoId = 4
                , TeamOnePlayerOne=null!, TeamOnePlayerTwo=null!, TeamTwoPlayerOne=null!, TeamTwoPlayerTwo=null!
                , TeamOneScore = 9, TeamTwoScore = 11
                , ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                }, 
                new () { GameId = 3, PlayedDate = DateTimeOffset.Now.AddDays(-4), TypeGameId=1
                , TeamOnePlayerOneId = 1, TeamOnePlayerTwoId = 4, TeamTwoPlayerOneId = 2, TeamTwoPlayerTwoId = 3
                , TeamOnePlayerOne=null!, TeamOnePlayerTwo=null!, TeamTwoPlayerOne=null!, TeamTwoPlayerTwo=null!
                , TeamOneScore = 11, TeamTwoScore = 5
                , ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                },
            };
            setupCtx.Games.AddRange(games);
            var playerRatings = new List<PlayerRating>
            {
                new () { PlayerId = 2, GameId =1, Rating = 300, RatingDate = DateTimeOffset.Now.AddDays(-8), ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                new () { PlayerId = 3, GameId =2, Rating = 400, RatingDate = DateTimeOffset.Now.AddDays(-6), ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                new () { PlayerId = 4, GameId =3, Rating = 500, RatingDate = DateTimeOffset.Now.AddDays(-4), ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
            };
            setupCtx.PlayerRatings.AddRange(playerRatings);
            setupCtx.SaveChanges();

        }

        [TestCleanup]
        public void Cleanup()
        {
            // This method is called after each test method.
            _ctx.Dispose();
            _connection.Close();
        }


        [TestMethod]
        [DataRow(0.7563, 11, 4)]
        [DataRow(0.550, 11, 9)]
        [DataRow(0.545, 11, 9)]
        [DataRow(0.538, 11, 9)]
        [DataRow(0.532, 13, 11)]
        [DataRow(0.531, 13, 11)]
        [DataRow(0.500, 15, 14)]
        public void CalculateExpectedScoreTest_ValidInputs_ReturnsExpectedScores(double expectedOutcome, int expectedWin, int expectedLoss)
        {
            var (winScore, lossScore) = GameLogic.CalculateExpectedScore((decimal)expectedOutcome);

            Assert.AreEqual(expectedWin, winScore);
            Assert.AreEqual(expectedLoss, lossScore);
        }

        [TestMethod]
        [DataRow(-0.1)]
        [DataRow(1.1)]
        public void CalculateExpectedScoreTest_InvalidInputs_ThrowsArgumentOutOfRange(double expectedOutcome)
        {
            Assert.ThrowsExactly <ArgumentOutOfRangeException>(() =>
                GameLogic.CalculateExpectedScore((decimal)expectedOutcome));
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGamePlayersTest_ValidPlayers()
        {
            var game = new GameDto
            {
                TeamOnePlayerOne = new PlayerDto(PlayerId:1),
                TeamOnePlayerTwo = new PlayerDto(PlayerId:2),
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
            };
            var actual = GameLogic.ValidateGamePlayers(_ctx, game);

            Assert.AreEqual(string.Empty, actual);

        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGamePlayersTest_InValidPlayers()
        {
            var game = new GameDto
            {
                TeamOnePlayerOne = new PlayerDto(PlayerId: 1),
                TeamOnePlayerTwo = new PlayerDto(PlayerId: 2),
                TeamTwoPlayerOne = new PlayerDto(PlayerId: 3),
                TeamTwoPlayerTwo = new PlayerDto(PlayerId: 7),// <== OJO invalid player id

            };
            var actual = GameLogic.ValidateGamePlayers(_ctx, game);

            Assert.StartsWith("Player Id, 7", actual);

        }

        [TestMethod]
        public void GetPlayerRatingsTest_ValidOneInitial()
        {
            var playedAt = DateTimeOffset.Now;
            Game game = new()
            {
                TeamOnePlayerOneId = 1,
                TeamOnePlayerTwoId = 2,
                TeamTwoPlayerOneId = 3,
                TeamTwoPlayerTwoId = 4,
            };

            //act
            var actual = GameLogic.GetPlayerRatingsAsync(_ctx, game, playedAt).Result;

            //assert    
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<GameLogic.GameRatings>(actual);
            Assert.AreEqual(250, actual.T1P1); // new player default rating
            Assert.AreEqual(300, actual.T1P2); // existing player
            Assert.AreEqual(400, actual.T2P1); // existing player
            Assert.AreEqual(500, actual.T2P2); // existing player
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_NullGame()
        {
            //arrange
            GameDto game = null!;

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            Assert.StartsWith("Game data is null", actual);

        }
        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_NullPlayer1()
        {
            //arrange
            var game = new GameDto
            (
                TeamOnePlayerOne: null!,      // <== OJO null player id
                TeamOnePlayerTwo: new PlayerDto(PlayerId: 2),
                TeamTwoPlayerOne: new PlayerDto(3),
                TeamTwoPlayerTwo: new PlayerDto(4)
            );

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            Assert.StartsWith("Team One Player One can not be NULL", actual);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_NullPlayer3()
        {
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = new PlayerDto(PlayerId: 1),
                TeamOnePlayerTwo = new PlayerDto(PlayerId: 2),
                TeamTwoPlayerOne = null!,                      // <== OJO null player id
                TeamTwoPlayerTwo = new PlayerDto(PlayerId: 4),
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            Assert.StartsWith("Team Two Player One can not be NULL", actual);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_SamePlayerTwiceTeam1()
        {
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = new PlayerDto(PlayerId: 1),
                TeamOnePlayerTwo = new(1),  // <== OJO duplicate player id
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            Assert.StartsWith("Team One cannot have the same player twice.", actual);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_SamePlayerTwiceTeam2()
        {
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = new PlayerDto(PlayerId: 1),
                TeamOnePlayerTwo = new PlayerDto(PlayerId: 2), 
                TeamTwoPlayerOne = new PlayerDto(PlayerId: 4),  // <== OJO duplicate player id
                TeamTwoPlayerTwo = new PlayerDto(4),           
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            Assert.StartsWith("Team Two cannot have the same player twice.", actual );
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_HasTypeGame()
        {
            //arrange
            var game = new GameDto
            {
                TypeGameId = 0,                     // <== OJO invalid typeGameId
                TeamOnePlayerOne = new PlayerDto(PlayerId: 1),
                TeamOnePlayerTwo = new PlayerDto(PlayerId: 2),
                TeamTwoPlayerOne = new PlayerDto(PlayerId: 3),
                TeamTwoPlayerTwo = new PlayerDto(PlayerId: 4),
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            Assert.Contains("Valid TypeGameId", actual);
        }



        [TestMethod]
        [DataRow(1, null, null, 2, "Player can not be on both teams")]
        [DataRow(1, null, 3, null, "Player can not be on both teams")]
        [DataRow(1, 2, 3, 2, "Player can not be on both teams")]
        [DataRow(1, 2, 2, 4, "Player can not be on both teams")]
        [DataRow(1, 2, 3, 1, "Team 1 first Player can not be on both teams")]
        [DataRow(1, 2, 1, 4, "Team 1 first Player can not be on both teams")]
        [DataRow(1, 1, 2, 4, "Team One cannot have the same player")]
        [DataRow(1, 2, 3, 3, "Team Two cannot have the same player")]

        [TestCategory("unit")]
        public void ValidateGameTest_SamePlayer(int p1, int p2, int p3, int p4, string expected)
        {
            //arrange
            var game = new GameDto
            {
                TypeGameId = 1,
                TeamOnePlayerOne = new PlayerDto(PlayerId: p1),
                TeamOnePlayerTwo = new PlayerDto(PlayerId: p2),
                TeamTwoPlayerOne = new PlayerDto(PlayerId: p3),
                TeamTwoPlayerTwo = new PlayerDto(PlayerId: p4),
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            Assert.StartsWith(expected, actual);
        }

        [TestMethod]
        [DataRow(null, null, "")]
        [DataRow(11, null, "")]
        [DataRow(null, 8, "")]
        [DataRow(11, -1, "negative")]
        [DataRow(-1, 15, "negative")]
        [DataRow(11,11, "tied")]
        [TestCategory("unit")]
        public void ValidateGameTest_VariousScores(int s1, int s2, string expected)
        {
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = new PlayerDto(PlayerId: 1),
                TeamOnePlayerTwo = new PlayerDto(PlayerId: 2),
                TeamTwoPlayerOne = new PlayerDto(PlayerId: 3),
                TeamTwoPlayerTwo = new PlayerDto(PlayerId: 4),
                TeamOneScore = s1,
                TeamTwoScore = s2,  
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            Assert.Contains(expected, actual);
        }

        [TestMethod]
        [DataRow(null, null, null, "")]
        [DataRow(  11, null, null, "both score must be NULL")]
        [DataRow(null,    8, null, "both score must be NULL")]
        [DataRow(11,      8, null, "date must be provided")]
        [DataRow(null, null, 0L, "both scores must be provided")]
        [DataRow(11,   null, 0L, "both scores must be provided")]
        [DataRow(null,    8, 638943944899710263, "both scores must be provided")]
        [DataRow(11,      8, 638943944899710263, "")]
        [DataRow(11, 8, 648943954899710263, "the future")]
        [TestCategory("unit")]
        public void ValidateGameTest_ScoreDate(int? s1, int? s2, long? ticks, string expected)
        {
            //638943954899710263
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = new PlayerDto(PlayerId: 1),
                TeamOnePlayerTwo = new PlayerDto(PlayerId: 2),
                TeamOneScore = s1,
                TeamTwoPlayerOne = new PlayerDto(PlayerId: 3),
                TeamTwoPlayerTwo = new PlayerDto(PlayerId: 4),
                TeamTwoScore = s2,
                PlayedDate = ticks.HasValue ? new DateTimeOffset(ticks.Value, TimeSpan.Zero) : null,
                TypeGameId = 1,
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            if (string.IsNullOrEmpty(expected))
            {
                Assert.AreEqual(string.Empty, actual);
            }
            else
            {
                Assert.Contains(expected, actual);
            }
        }
    }


}
