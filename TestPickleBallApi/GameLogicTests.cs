using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PickleBallAPI.Controllers;
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
                new () { PlayerId = 1, FirstName = "Player", LastName = "One" },
                new () { PlayerId = 2, FirstName = "Player", LastName = "Two" },
                new () { PlayerId = 3, FirstName = "Player", LastName = "Three" },
                new () { PlayerId = 4, FirstName = "Player", LastName = "Four" },
            };
            setupCtx.Players.AddRange(players);
            var typeGames = new List<TypeGame>
            {
                new () { TypeGameId = 1, GameType = "Recreational" },
                new () { TypeGameId = 2, GameType = "Tournament" },
            };
            setupCtx.TypeGames.AddRange(typeGames);
            var games = new List<Game>
            {
                new () { GameId = 1, PlayedDate = DateTimeOffset.Now.AddDays(-8), TypeGameId=1
                , TeamOnePlayerOneId = 1, TeamOnePlayerTwoId = 2, TeamTwoPlayerOneId = 3, TeamTwoPlayerTwoId = 4
                , TeamOnePlayerOne=null!, TeamOnePlayerTwo=null!, TeamTwoPlayerOne=null!, TeamTwoPlayerTwo=null!
                ,TeamOneScore = 11, TeamTwoScore = 8 
                },
                new () { GameId = 2, PlayedDate = DateTimeOffset.Now.AddDays(-6), TypeGameId=1
                , TeamOnePlayerOneId = 1, TeamOnePlayerTwoId = 3, TeamTwoPlayerOneId = 2, TeamTwoPlayerTwoId = 4
                , TeamOnePlayerOne=null!, TeamOnePlayerTwo=null!, TeamTwoPlayerOne=null!, TeamTwoPlayerTwo=null!
                , TeamOneScore = 9, TeamTwoScore = 11 
                }, 
                new () { GameId = 3, PlayedDate = DateTimeOffset.Now.AddDays(-4), TypeGameId=1
                , TeamOnePlayerOneId = 1, TeamOnePlayerTwoId = 4, TeamTwoPlayerOneId = 2, TeamTwoPlayerTwoId = 3
                , TeamOnePlayerOne=null!, TeamOnePlayerTwo=null!, TeamTwoPlayerOne=null!, TeamTwoPlayerTwo=null!
                , TeamOneScore = 11, TeamTwoScore = 5
                },
            };
            setupCtx.Games.AddRange(games);
            var playerRatings = new List<PlayerRating>
            {
                new () { PlayerId = 2, GameId =1, Rating = 300, RatingDate = DateTime.Now.AddDays(-8) },
                new () { PlayerId = 3, GameId =2, Rating = 400, RatingDate = DateTime.Now.AddDays(-6) },
                new () { PlayerId = 4, GameId =3, Rating = 500, RatingDate = DateTime.Now.AddDays(-4) },
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


        [DataTestMethod]
        [DataRow(0.75, 15, 5)]
        [DataRow(0.550, 11, 9)]
        [DataRow(0.545, 12, 10)]
        [DataRow(0.542, 13, 11)]
        [DataRow(0.538, 14, 12)]
        [DataRow(0.536, 15, 13)]
        [DataRow(0.500, 15, 14)]
        public void CalculateExpectedScoreTest_ValidInputs_ReturnsExpectedScores(double expectedOutcome, int expectedWin, int expectedLoss)
        {
            var (winScore, lossScore) = GameLogic.CalculateExpectedScore(expectedOutcome);

            Assert.AreEqual(expectedWin, winScore);
            Assert.AreEqual(expectedLoss, lossScore);
        }

        [DataTestMethod]
        [DataRow(-0.1)]
        [DataRow(1.1)]
        public void CalculateExpectedScoreTest_InvalidInputs_ThrowsArgumentOutOfRange(double expectedOutcome)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                GameLogic.CalculateExpectedScore(expectedOutcome));
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGamePlayersTest_ValidPlayers()
        {
            var game = new GameDto
            {
                TeamOnePlayerOne = new() { PlayerId = 1 },
                TeamOnePlayerTwo = new() { PlayerId = 2 },
                TeamTwoPlayerOne = new() { PlayerId = 3 },
                TeamTwoPlayerTwo = new() { PlayerId = 4 },
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
                TeamOnePlayerOne = new() { PlayerId = 1 },
                TeamOnePlayerTwo = new() { PlayerId = 2 },
                TeamTwoPlayerOne = new() { PlayerId = 3 },
                TeamTwoPlayerTwo = new() { PlayerId = 7 },  // <== OJO invalid player id
            };
            var actual = GameLogic.ValidateGamePlayers(_ctx, game);

            StringAssert.StartsWith(actual, "Player Id, 7");

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
            StringAssert.StartsWith(actual, "Game data is null");

        }
        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_NullPlayer1()
        {
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = null!,      // <== OJO null player id
                TeamOnePlayerTwo = new() { PlayerId = 2 },
                TeamTwoPlayerOne = new() { PlayerId = 3 },
                TeamTwoPlayerTwo = new() { PlayerId = 4 },  
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            StringAssert.StartsWith(actual, "Team One Player One can not be NULL");
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_NullPlayer3()
        {
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = new() { PlayerId = 1 },
                TeamOnePlayerTwo = new() { PlayerId = 2 },
                TeamTwoPlayerOne = null!,                      // <== OJO null player id
                TeamTwoPlayerTwo = new() { PlayerId = 4 },
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            StringAssert.StartsWith(actual, "Team Two Player One can not be NULL");
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_SamePlayerTwiceTeam1()
        {
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = new() { PlayerId = 1 },
                TeamOnePlayerTwo = new() { PlayerId = 1 },  // <== OJO duplicate player id
                TeamTwoPlayerOne = new() { PlayerId = 3 },
                TeamTwoPlayerTwo = new() { PlayerId = 4 },
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            StringAssert.StartsWith(actual, "Team One cannot have the same player twice.");
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_SamePlayerTwiceTeam2()
        {
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = new() { PlayerId = 1 },
                TeamOnePlayerTwo = new() { PlayerId = 2 }, 
                TeamTwoPlayerOne = new() { PlayerId = 4 }, // <== OJO duplicate player id
                TeamTwoPlayerTwo = new() { PlayerId = 4 },
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            StringAssert.StartsWith(actual, "Team Two cannot have the same player twice.");
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ValidateGameTest_HasTypeGame()
        {
            //arrange
            var game = new GameDto
            {
                TypeGameId = 0,// <== OJO invalid typeGameId
                TeamOnePlayerOne = new() { PlayerId = 1 },
                TeamOnePlayerTwo = new() { PlayerId = 2 },
                TeamTwoPlayerOne = new() { PlayerId = 3 }, 
                TeamTwoPlayerTwo = new() { PlayerId = 4 },
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            StringAssert.Contains(actual, "Valid TypeGameId");
        }



        [DataTestMethod]
        [DataRow(1, 2, 1, 4)]
        [DataRow(1, 2, 3, 1)]
        [DataRow(1, null, 1, null)]
        [DataRow(1, null, 3, null)]
        [DataRow(1, 2, 3, 2)]
        [DataRow(1, 2, 2, 4)]
        [TestCategory("unit")]
        public void ValidateGameTest_SamePlayerOnBothTeams(int p1, int p2, int p3, int p4)
        {
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = new() { PlayerId = p1 },
                TeamOnePlayerTwo = new() { PlayerId = p2 },
                TeamTwoPlayerOne = new() { PlayerId = p3 },
                TeamTwoPlayerTwo = new() { PlayerId = p4 },
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            StringAssert.StartsWith(actual, "Player can not be on both teams.");
        }

        [DataTestMethod]
        [DataRow(null, null, "")]
        [DataRow(11, null, "")]
        [DataRow(null, 8, "")]
        [DataRow(11, -1, "negative")]
        [DataRow(-1, 15, "negative")]
        [DataRow(11,11, "tied")]
        [TestCategory("unit")]
        public void ValidateGameTest_SamePlayerOnBothTeams(int s1, int s2, string expected)
        {
            //arrange
            var game = new GameDto
            {
                TeamOnePlayerOne = new() { PlayerId = 1 },
                TeamOnePlayerTwo = new() { PlayerId = 2 },
                TeamTwoPlayerOne = new() { PlayerId = 3 },
                TeamTwoPlayerTwo = new() { PlayerId = 4 },
                TeamOneScore = s1,
                TeamTwoScore = s2,  
            };

            //act
            var actual = GameLogic.ValidateGame(game);

            //assert
            StringAssert.Contains(actual, expected);
        }

        [DataTestMethod]
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
                TeamOnePlayerOne = new() { PlayerId = 1 },
                TeamOnePlayerTwo = new() { PlayerId = 2 },
                TeamTwoPlayerOne = new() { PlayerId = 3 },
                TeamTwoPlayerTwo = new() { PlayerId = 4 },
                TeamOneScore = s1,
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
                StringAssert.Contains(actual, expected);
            }
        }
    }


}
