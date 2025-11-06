using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PickleBallAPI;
using PickleBallAPI.Controllers;
using PickleBallAPI.Models;
using System;
using System.Linq;

namespace TestPickleBallApi
{
    [TestClass]
    public sealed class GamesControllerUnitTests
    {
        private DbContextOptions<VprContext> _vprOpt = null!;
        private IMapper _mapper = null!;
        private ILoggerFactory _loggerFactory = null!;
        private SqliteConnection _connection = null!;
        private ILogger<GamesControllerUnitTests> _testLog = null!;

        [TestInitialize]
        public void TestInit()
        {
            // This method is called before each test method.
            _loggerFactory = LoggerFactory.Create(builder => 
            { 
                builder.AddConsole();
                // Set default minimum log level for all categories
                builder.SetMinimumLevel(LogLevel.Trace);
                // Set a specific log level for a category using a wildcard
                builder.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.Information);

            });
            _testLog = _loggerFactory.CreateLogger<GamesControllerUnitTests>();

            _connection = new SqliteConnection("DataSource=:memory:");
            _vprOpt = new DbContextOptionsBuilder<VprContext>()
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .UseSqlite(_connection)
                .Options;
            _connection.Open();
            using var ctx = new VprContext(_vprOpt);
            ctx.Database.EnsureCreated();

            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<PickleBallProfile>(), _loggerFactory));

            using var setupCtx = new VprContext(_vprOpt);
            // Seed with a game and related data
            _testLog.LogTrace("Seeding test data TypeGame...");
            TestHelper.SetupLookupData(setupCtx);

            _testLog.LogTrace("Seeding test data Player...");
            TestHelper.SetupPlayerData(setupCtx);

            _testLog.LogTrace("Seeding test data Game...");
            var game = new Game
            {
                GameId = 1,
                PlayedDate = new DateTimeOffset(2025,9,15, 18,00,00, TimeSpan.FromHours(-4.0)),
                TypeGameId = 1,
                TeamOnePlayerOneId = 1,
                TeamOnePlayerTwoId = 2,
                TeamTwoPlayerOneId = 3,
                TeamTwoPlayerTwoId = 4,
                TeamOneScore = 11,
                TeamTwoScore = 3,
                GamePrediction = new GamePrediction
                {
                    GameId = 1,
                    Game=null!,
                    T1p1rating = 200,
                    T1p2rating = 600,
                    T2p1rating = 300,
                    T2p2rating = 500,
                    T1predictedWinProb = 0.75,
                    ExpectT1score = 11,
                    ExpectT2score = 9,
                    CreatedAt = new DateTimeOffset(2025, 9, 15, 18, 00, 00, TimeSpan.FromHours(-4.0)),
                },
            };
            setupCtx.Games.Add(game);
            setupCtx.SaveChanges();

            _testLog.LogTrace("Seeding test data PlayerRating...");
            var p1r = new PlayerRating
            {
                PlayerRatingId = 1,
                PlayerId = 1,
                GameId = 1,
                Rating = 200,
                RatingDate = new DateTimeOffset(2025, 09, 01, 18, 0, 0, TimeSpan.FromHours(-4.0)),

            };
            var p2r = new PlayerRating
            {
                PlayerRatingId = 2,
                PlayerId = 2,
                GameId = 1, 
                Rating = 600,
                RatingDate = new DateTimeOffset(2025, 09, 01, 18, 0, 0, TimeSpan.FromHours(-4.0)),

            };
            var p3r = new PlayerRating
            {
                PlayerRatingId = 3,
                PlayerId = 3,
                GameId = 1,
                Rating = 300,
                RatingDate = new DateTimeOffset(2025, 09, 01, 18, 0, 0, TimeSpan.FromHours(-4.0)),
            };
            var p4r = new PlayerRating
            {
                PlayerRatingId = 4,
                PlayerId = 4,
                GameId = 1,
                Rating = 500,
                RatingDate = new DateTimeOffset(2025, 09, 01, 18, 0, 0, TimeSpan.FromHours(-4.0)),
            };
            setupCtx.PlayerRatings.AddRange(p1r, p2r, p3r, p4r);
            setupCtx.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
            _connection.Dispose();
            _loggerFactory.Dispose();
        }

        [TestMethod]
        [TestCategory("unit")]
        public void CtorTest()
        {
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);

            // Act
            var target = new GamesController(ctx,_mapper,log);
            // Assert
            Assert.IsNotNull(target);
            ctx.Dispose();  //double dispose test; no exceptions!!
        }

        [TestMethod]
        [TestCategory("unit")]
        public void GetGamesTest_ValueFound()
        {
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            // Act
            var actual = target.GetGames().Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<OkObjectResult>(actual.Result);
            var result = actual.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<GameDto[]>(result.Value);
            var gameDto = result.Value as GameDto[];
            Assert.IsNotNull(gameDto);
            Assert.AreEqual(1, gameDto.First().GameId);
        }


        [TestMethod]
        [TestCategory("unit")]
        public void GetGameTest_ValueFound()
        {
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            // Act
            var actual = target.GetGame(1).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<OkObjectResult>(actual.Result);
            var result = actual.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<GameDto>(result.Value);
            var gameDto = result.Value as GameDto;
            Assert.IsNotNull(gameDto);
            Assert.AreEqual(1, gameDto.GameId);
            var gamePredictionDto = gameDto.GamePrediction;
            Assert.IsNotNull(gamePredictionDto);
            Assert.AreEqual(200, gamePredictionDto.T1p1rating);
            Assert.AreEqual(600, gamePredictionDto.T1p2rating);
            Assert.AreEqual(300, gamePredictionDto.T2p1rating); 
            Assert.AreEqual(500, gamePredictionDto.T2p2rating); 
            Assert.AreEqual(0.75, gamePredictionDto.T1predictedWinProb);
            Assert.AreEqual(11, gamePredictionDto.ExpectT1score);
            Assert.AreEqual(9, gamePredictionDto.ExpectT2score);    

        }

        [TestMethod]
        [TestCategory("unit")]
        public void GetGameTest_ValueNotFount()
        {
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            // Act
            var actual = target.GetGame(-1).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NotFoundResult>(actual.Result);
            var result = actual.Result as NotFoundResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);

        }

        [TestMethod]
        [TestCategory("unit")]
        public void PostGameTest_ValidData()
        {
            _testLog.LogTrace("Starting PostGameTest_ValidData");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var newGameDto = new GameDto
            {
                GameId = 2,
                PlayedDate = DateTimeOffset.Now,
                TypeGameId = 1,
                TeamOnePlayerOne = new PlayerDto { PlayerId = 1, },
                TeamOnePlayerTwo = new PlayerDto { PlayerId = 2, },
                TeamTwoPlayerOne = new PlayerDto { PlayerId = 3, },
                TeamTwoPlayerTwo = new PlayerDto { PlayerId = 4, },
                TeamOneScore = 11,
                TeamTwoScore = 8,
            };
            // Act
            var actual = target.PostGame(newGameDto).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<CreatedAtActionResult>(actual.Result);
            var result = actual.Result as CreatedAtActionResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<GameDto>(result.Value);
            var createdGameDto = result.Value as GameDto;
            Assert.IsNotNull(createdGameDto);
            Assert.AreEqual(newGameDto.PlayedDate, createdGameDto.PlayedDate);
            Assert.AreEqual(newGameDto.TeamOneScore, createdGameDto.TeamOneScore);
            Assert.AreEqual(newGameDto.TeamTwoScore, createdGameDto.TeamTwoScore);
            Assert.AreEqual(newGameDto.TeamOnePlayerOne.PlayerId, createdGameDto.TeamOnePlayerOne.PlayerId);
            Assert.AreEqual(newGameDto.TeamOnePlayerTwo.PlayerId, createdGameDto.TeamOnePlayerTwo.PlayerId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerOne.PlayerId, createdGameDto.TeamTwoPlayerOne.PlayerId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerTwo.PlayerId, createdGameDto.TeamTwoPlayerTwo.PlayerId);
            
            var gameInDb = ctx.Games
                .Where(g => g.GameId == createdGameDto.GameId)
                .Include(g => g.TeamOnePlayerOne)
                .Include(g => g.TeamOnePlayerTwo)
                .Include(g => g.TeamTwoPlayerOne)
                .Include(g => g.TeamTwoPlayerTwo)
                .FirstOrDefault();
            Assert.IsNotNull(gameInDb);
            Assert.IsNotNull(gameInDb.PlayedDate);
            Assert.AreEqual(newGameDto.PlayedDate.Value.Date, gameInDb.PlayedDate.Value.Date);
            Assert.AreEqual(newGameDto.TeamOneScore, gameInDb.TeamOneScore);
            Assert.AreEqual(newGameDto.TeamTwoScore, gameInDb.TeamTwoScore);
            Assert.AreEqual(newGameDto.TeamOnePlayerOne.PlayerId, gameInDb.TeamOnePlayerOneId);
            Assert.AreEqual(newGameDto.TeamOnePlayerTwo.PlayerId, gameInDb.TeamOnePlayerTwoId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerOne.PlayerId, gameInDb.TeamTwoPlayerOneId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerTwo.PlayerId, gameInDb.TeamTwoPlayerTwoId);

        }

        [TestMethod]
        [TestCategory("unit")]
        public void PostGameTest_PreGame()
        {
            _testLog.LogTrace("Starting PostGameTest_PreGame");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var newGameDto = new GameDto
            {
                GameId = 2,
                TypeGameId = 1,
                TeamOnePlayerOne = new PlayerDto { PlayerId = 1, },
                TeamOnePlayerTwo = new PlayerDto { PlayerId = 2, },
                TeamTwoPlayerOne = new PlayerDto { PlayerId = 3, },
                TeamTwoPlayerTwo = new PlayerDto { PlayerId = 4, },
            };
            // Act
            var actual = target.PostGame(newGameDto).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<CreatedAtActionResult>(actual.Result);
            var result = actual.Result as CreatedAtActionResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<GameDto>(result.Value);
            var createdGameDto = result.Value as GameDto;
            Assert.IsNotNull(createdGameDto);
            Assert.AreEqual(newGameDto.TeamOneScore, createdGameDto.TeamOneScore);
            Assert.AreEqual(newGameDto.TeamTwoScore, createdGameDto.TeamTwoScore);
            Assert.AreEqual(newGameDto.TeamOnePlayerOne.PlayerId, createdGameDto.TeamOnePlayerOne.PlayerId);
            Assert.AreEqual(newGameDto.TeamOnePlayerTwo.PlayerId, createdGameDto.TeamOnePlayerTwo.PlayerId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerOne.PlayerId, createdGameDto.TeamTwoPlayerOne.PlayerId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerTwo.PlayerId, createdGameDto.TeamTwoPlayerTwo.PlayerId);

            var gameInDb = ctx.Games
                .Where(g => g.GameId == createdGameDto.GameId)
                .Include(g => g.TeamOnePlayerOne)
                .Include(g => g.TeamOnePlayerTwo)
                .Include(g => g.TeamTwoPlayerOne)
                .Include(g => g.TeamTwoPlayerTwo)
                .FirstOrDefault();
            Assert.IsNotNull(gameInDb);
            Assert.AreEqual(newGameDto.TeamOneScore, gameInDb.TeamOneScore);
            Assert.AreEqual(newGameDto.TeamTwoScore, gameInDb.TeamTwoScore);
            Assert.AreEqual(newGameDto.TeamOnePlayerOne.PlayerId, gameInDb.TeamOnePlayerOneId);
            Assert.AreEqual(newGameDto.TeamOnePlayerTwo.PlayerId, gameInDb.TeamOnePlayerTwoId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerOne.PlayerId, gameInDb.TeamTwoPlayerOneId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerTwo.PlayerId, gameInDb.TeamTwoPlayerTwoId);

        }



        [TestMethod]
        [TestCategory("unit")]
        public void PutGameTest_ValidData()
        {
            _testLog.LogTrace("Starting PutGameTest_ValidData");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var newGameDto = new GameDto
            {
                GameId = 4,
                TypeGameId = 1,
                TeamOnePlayerOne = new PlayerDto { PlayerId = 1, },
                TeamOnePlayerTwo = new PlayerDto { PlayerId = 2, },
                TeamTwoPlayerOne = new PlayerDto { PlayerId = 3, },
                TeamTwoPlayerTwo = new PlayerDto { PlayerId = 4, },
                // without scores or date to simulate pre-game update
            };
            {
                using var setupCtx = new VprContext(_vprOpt);
                var setup = new GamesController(setupCtx, _mapper, log);
                // first create the game to be updated
                var postResult = target.PostGame(newGameDto).Result;
                // confirm created
                Assert.IsNotNull(postResult);
                Assert.IsInstanceOfType<CreatedAtActionResult>(postResult.Result);
                var postCreatedResult = postResult.Result as CreatedAtActionResult;
                Assert.IsNotNull(postCreatedResult);
                Assert.AreEqual(201, postCreatedResult.StatusCode);
            }

            // now update with scores and date after game played
            newGameDto.PlayedDate = DateTimeOffset.Now.AddDays(-1);
            newGameDto.TeamOneScore = 11;
            newGameDto.TeamTwoScore = 9;
            _testLog.LogTrace("PutGameTest_ValidData Initialization Complete");

            // Act
            var actual = target.PutGame(4, newGameDto).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NoContentResult>(actual);

            var gameInDb = ctx.Games
                .Where(g => g.GameId == newGameDto.GameId)
                .Include(g => g.TeamOnePlayerOne)
                .Include(g => g.TeamOnePlayerTwo)
                .Include(g => g.TeamTwoPlayerOne)
                .Include(g => g.TeamTwoPlayerTwo)
                .Include(g=> g.GamePrediction)
                .FirstOrDefault();
            Assert.IsNotNull(gameInDb);
            Assert.IsNotNull(gameInDb.PlayedDate);
            Assert.AreEqual(newGameDto.PlayedDate.Value.Date, gameInDb.PlayedDate.Value.Date);
            Assert.AreEqual(newGameDto.TeamOneScore, gameInDb.TeamOneScore);
            Assert.AreEqual(newGameDto.TeamTwoScore, gameInDb.TeamTwoScore);
            Assert.AreEqual(newGameDto.TeamOnePlayerOne.PlayerId, gameInDb.TeamOnePlayerOneId);
            Assert.AreEqual(newGameDto.TeamOnePlayerTwo.PlayerId, gameInDb.TeamOnePlayerTwoId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerOne.PlayerId, gameInDb.TeamTwoPlayerOneId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerTwo.PlayerId, gameInDb.TeamTwoPlayerTwoId);

        }

        [TestMethod]
        [TestCategory("unit")]
        public void PutGameTest_InvalidGameId()
        {
            _testLog.LogTrace("Starting PutGameTest_InvalidGameId");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var newGameDto = new GameDto
            {
                GameId = 1,
            };

            // Act
            var actual = target.PutGame(3, newGameDto).Result; // id does not match gameDto.GameId

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<BadRequestObjectResult>(actual);
            var result = actual as BadRequestObjectResult;
            Assert.IsNotNull(result);
            StringAssert.Contains(result.Value as string, "Id does not match");
        }

        [TestMethod]
        [TestCategory("unit")]
        public void PutGameTest_InvalidGame()
        {
            _testLog.LogTrace("Starting PutGameTest_InvalidGame");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var newGameDto = new GameDto
            {
                GameId = 1,
                PlayedDate = DateTimeOffset.Now,
                TypeGameId = 1,
                TeamOnePlayerOne =null!,
                TeamOnePlayerTwo = new PlayerDto { PlayerId = 2, },
                TeamTwoPlayerOne = new PlayerDto { PlayerId = 3, },
                TeamTwoPlayerTwo = new PlayerDto { PlayerId = 4, },
                TeamOneScore = 11,
                TeamTwoScore = 0,
            };

            // Act
            var actual = target.PutGame(1, newGameDto).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<BadRequestObjectResult>(actual);
            var result = actual as BadRequestObjectResult;
            Assert.IsNotNull(result);
            StringAssert.Contains(result.Value as string, "Player One can not be NULL");
        }

        [TestMethod]
        [TestCategory("unit")]
        public void PutGameTest_InvalidPlayer()
        {
            _testLog.LogTrace("Starting PutGameTest_InvalidPlayer");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var newGameDto = new GameDto
            {
                GameId = 1,
                PlayedDate = DateTimeOffset.Now,
                TypeGameId = 1,
                TeamOnePlayerOne = new PlayerDto { PlayerId = -1, },  //    <== invalid player
                TeamOnePlayerTwo = new PlayerDto { PlayerId = 2, },
                TeamTwoPlayerOne = new PlayerDto { PlayerId = 3, },
                TeamTwoPlayerTwo = new PlayerDto { PlayerId = 4, },
                TeamOneScore = 11,
                TeamTwoScore = 0,
            };

            // Act
            var actual = target.PutGame(1, newGameDto).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<BadRequestObjectResult>(actual);
            var result = actual as BadRequestObjectResult;
            Assert.IsNotNull(result);
            StringAssert.Contains(result.Value as string, "does not exist");
        }

        [TestMethod]
        [TestCategory("unit")]
        public void PutGameUpdateTest_ValidData()
        {
            _testLog.LogTrace("Starting PutGameUpdateTest_ValidData");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var gamePlayedDate = DateTimeOffset.Now.AddDays(-1);
            var newGameDto = new GameDto
            {
                GameId = 2,
                TypeGameId = 1,
                TeamOnePlayerOne = new PlayerDto { PlayerId = 1, },
                TeamOnePlayerTwo = new PlayerDto { PlayerId = 2, },
                TeamTwoPlayerOne = new PlayerDto { PlayerId = 3, },
                TeamTwoPlayerTwo = new PlayerDto { PlayerId = 4, },
                PlayedDate = gamePlayedDate,
                TeamOneScore = 11,
                TeamTwoScore = 9,
            };
            {
                using var setupCtx = new VprContext(_vprOpt);
                var setup = new GamesController(setupCtx, _mapper, log);
                // first create the game to be updated
                var game = _mapper.Map<Game>(newGameDto);
                setupCtx.Games.Add(game);
                setupCtx.SaveChanges();
            }
            _testLog.LogTrace("PutGameTest_ValidData Initialization Complete");

            // Act
            var actual = target.PutGameUpdate(2).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NoContentResult>(actual);

            var gameInDb = ctx.Games
                .Where(g => g.GameId == newGameDto.GameId)
                .Include(g => g.TeamOnePlayerOne)
                .Include(g => g.TeamOnePlayerTwo)
                .Include(g => g.TeamTwoPlayerOne)
                .Include(g => g.TeamTwoPlayerTwo)
                .Include(g => g.GamePrediction)
                .FirstOrDefault();
            Assert.IsNotNull(gameInDb);
            Assert.IsNotNull(gameInDb.PlayedDate);
            Assert.AreEqual(newGameDto.PlayedDate.Value.Date, gameInDb.PlayedDate.Value.Date);
            Assert.AreEqual(newGameDto.TeamOneScore, gameInDb.TeamOneScore);
            Assert.AreEqual(newGameDto.TeamTwoScore, gameInDb.TeamTwoScore);
            Assert.AreEqual(newGameDto.TeamOnePlayerOne.PlayerId, gameInDb.TeamOnePlayerOneId);
            Assert.AreEqual(newGameDto.TeamOnePlayerTwo.PlayerId, gameInDb.TeamOnePlayerTwoId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerOne.PlayerId, gameInDb.TeamTwoPlayerOneId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerTwo.PlayerId, gameInDb.TeamTwoPlayerTwoId);
            var playerRatingsInDb = ctx.PlayerRatings
                .Where(pr => pr.GameId == newGameDto.GameId)
                .ToList();
            Assert.AreEqual(4, playerRatingsInDb.Count);
            for ( int i = 0; i < playerRatingsInDb.Count; i++)
            {
                Assert.AreEqual(gameInDb.PlayedDate.Value.Date, playerRatingsInDb[0].RatingDate.Date);
            }

        }

        [TestMethod]
        [TestCategory("unit")]
        public void PutGameUpdateTest_GameNotFound()
        {
            _testLog.LogTrace("Starting PutGameUpdateTest_ValidData");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var gamePlayedDate = DateTimeOffset.Now.AddDays(-1);
            // game not present
            _testLog.LogTrace("PutGameUpdateTest_ValidData Initialization Complete");

            // Act
            var actual = target.PutGameUpdate(2).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NotFoundResult>(actual);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void PutGameUpdateTest_All()
        {
            _testLog.LogTrace("Starting PutGameUpdateTest_All");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var gamePlayedDate = DateTimeOffset.Now.AddDays(-1);
            _testLog.LogTrace("PutGameUpdateTest_All Initialization Complete");

            // Act
            var actual = target.PutGameUpdate().Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NoContentResult>(actual);
        }
    }
}
