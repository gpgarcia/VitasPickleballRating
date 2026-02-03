using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using PickleBallAPI;
using PickleBallAPI.Controllers;
using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                builder.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.None);

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
                    T1predictedWinProb = 0.75m,
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
            Assert.AreEqual(0.75m, gamePredictionDto.T1predictedWinProb);
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
                TeamOnePlayerOne = new PlayerDto(1),
                TeamOnePlayerTwo = new PlayerDto(2),
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
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
        public void PostGameTest_InvalidPlayer()
        {
            _testLog.LogTrace("Starting PostGameTest_InvalidPlayer");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var newGameDto = new GameDto
            {
                GameId = 2,
                PlayedDate = DateTimeOffset.Now,
                TypeGameId = 1,
                TeamOnePlayerOne = new PlayerDto(-1),
                TeamOnePlayerTwo = new PlayerDto(2),
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
                TeamOneScore = 11,
                TeamTwoScore = 8
            };
            // Act
            var actual = target.PostGame(newGameDto).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<BadRequestObjectResult>(actual.Result);
            var result = actual.Result as BadRequestObjectResult;
            Assert.IsNotNull(result);   
            var valueStr = result.Value as string;
            Assert.IsNotNull(valueStr);
            Assert.Contains("does not exist", valueStr);
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
                TeamOnePlayerOne = new PlayerDto(1),
                TeamOnePlayerTwo = new PlayerDto(2),
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
                // without scores or date to simulate pre-game
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
                .Include(g => g.GamePrediction)
                .FirstOrDefault();
            Assert.IsNotNull(gameInDb);
            Assert.AreEqual(newGameDto.TeamOneScore, gameInDb.TeamOneScore);
            Assert.AreEqual(newGameDto.TeamTwoScore, gameInDb.TeamTwoScore);
            Assert.AreEqual(newGameDto.TeamOnePlayerOne.PlayerId, gameInDb.TeamOnePlayerOneId);
            Assert.AreEqual(newGameDto.TeamOnePlayerTwo.PlayerId, gameInDb.TeamOnePlayerTwoId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerOne.PlayerId, gameInDb.TeamTwoPlayerOneId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerTwo.PlayerId, gameInDb.TeamTwoPlayerTwoId);
            var predictionInDb = gameInDb.GamePrediction;
            Assert.IsNotNull(predictionInDb);
            Assert.AreEqual(200, predictionInDb.T1p1rating);
            Assert.AreEqual(600, predictionInDb.T1p2rating);
            Assert.AreEqual(300, predictionInDb.T2p1rating);
            Assert.AreEqual(500, predictionInDb.T2p2rating);    
            Assert.IsTrue(predictionInDb.CreatedAt.Date.Equals(DateTimeOffset.Now.Date));
            Assert.IsGreaterThan(0.0m, predictionInDb.T1predictedWinProb);
            Assert.IsLessThanOrEqualTo(1.0m, predictionInDb.T1predictedWinProb);


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
                Facility = new FacilityDto { FacilityId = 1, },
                TypeGameId = 1,
                TeamOnePlayerOne = new PlayerDto(1),
                TeamOnePlayerTwo = new PlayerDto(2),
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
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
            GameDto updatedNewGameDto = newGameDto with
            {
                PlayedDate = DateTimeOffset.Now.AddDays(-1),
                TeamOneScore = 11,
                TeamTwoScore = 9

            };
            _testLog.LogTrace("PutGameTest_ValidData Initialization Complete");
            _testLog.LogTrace("PutGameTest_ValidData end arrangement");

            // Act
            var actual = target.PutGame(4, updatedNewGameDto).Result;

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
            Assert.AreEqual(updatedNewGameDto.PlayedDate.Value.Date, gameInDb.PlayedDate.Value.Date);
            Assert.AreEqual(updatedNewGameDto.TeamOneScore, gameInDb.TeamOneScore);
            Assert.AreEqual(updatedNewGameDto.TeamTwoScore, gameInDb.TeamTwoScore);
            Assert.AreEqual(updatedNewGameDto.TeamOnePlayerOne.PlayerId, gameInDb.TeamOnePlayerOneId);
            Assert.AreEqual(updatedNewGameDto.TeamOnePlayerTwo.PlayerId, gameInDb.TeamOnePlayerTwoId);
            Assert.AreEqual(updatedNewGameDto.TeamTwoPlayerOne.PlayerId, gameInDb.TeamTwoPlayerOneId);
            Assert.AreEqual(updatedNewGameDto.TeamTwoPlayerTwo.PlayerId, gameInDb.TeamTwoPlayerTwoId);

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
            Assert.Contains("Id does not match", result.Value as string);
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
                TeamOnePlayerTwo = new PlayerDto(2),
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
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
            Assert.Contains("Player One can not be NULL", result.Value as string);
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
                Facility = new FacilityDto { FacilityId = 1,},
                TeamOnePlayerOne = new PlayerDto(-1),   //    <== invalid player
                TeamOnePlayerTwo = new PlayerDto(2),
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
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
            Assert.Contains("does not exist", result.Value as string);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void PutGameTest_DbUpdate()
        {
            _testLog.LogTrace("Starting PutGameTest_DbUpdate");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var newGameDto = new GameDto
            {
                GameId = 4,
                TypeGameId = 1,
                TeamOnePlayerOne = new PlayerDto(1),
                TeamOnePlayerTwo = new PlayerDto(2),
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
                // now update with scores and date after game played
                PlayedDate = DateTimeOffset.Now.AddDays(-1),
                TeamOneScore = 11,
                TeamTwoScore = 9
            };
            _testLog.LogTrace("PutGameTest_ValidData end arrangement");

            // Act
            var actual = target.PutGame(4, newGameDto).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NotFoundResult>(actual);

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
                TeamOnePlayerOne = new PlayerDto(1),
                TeamOnePlayerTwo = new PlayerDto(2),
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
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
            Assert.HasCount(4, playerRatingsInDb);
            for ( int i = 0; i < playerRatingsInDb.Count; i++)
            {
                Assert.AreEqual(gameInDb.PlayedDate.Value.Date, playerRatingsInDb[0].RatingDate.Date);
            }
        }
        [TestMethod]
        [TestCategory("unit")]
        public void PutGameUpdateTest_PredictTeam2Wins()
        {
            _testLog.LogTrace("Starting PutGameUpdateTest_PredictTeam2Wins");
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);
            var gamePlayedDate = DateTimeOffset.Now.AddDays(-1);
            var pr = new List<PlayerRating>
            {
                new() { PlayerId = 1, Rating = 257, RatingDate = gamePlayedDate.AddDays(-2), GameId = 2,  ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},
                new() { PlayerId = 2, Rating = 237, RatingDate = gamePlayedDate.AddDays(-3), GameId = 2,  ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},
                new() { PlayerId = 3, Rating = 263, RatingDate = gamePlayedDate.AddDays(-4), GameId = 2,  ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},
                new() { PlayerId = 4, Rating = 250, RatingDate = gamePlayedDate.AddDays(-5), GameId = 2,  ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},

            };
            var game2 = new Game
            {
                TypeGameId = 1,
                FacilityId = 1,
                TeamOnePlayerOneId = 1,
                TeamOnePlayerTwoId = 2,
                TeamTwoPlayerOneId = 3,
                TeamTwoPlayerTwoId = 4,
                ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()

            };
            var newGameDto = new GameDto
            {
                GameId = 3,
                TypeGameId = 1,
                Facility = new FacilityDto(FacilityId: 1 ),
                TeamOnePlayerOne = new PlayerDto(1),
                TeamOnePlayerTwo = new PlayerDto(2),
                TeamTwoPlayerOne = new PlayerDto(3),
                TeamTwoPlayerTwo = new PlayerDto(4),
                PlayedDate = gamePlayedDate,
                TeamOneScore = 11,
                TeamTwoScore = 5,

            };
            {
                using var setupCtx = new VprContext(_vprOpt);
                var setup = new GamesController(setupCtx, _mapper, log);
                // first create the game to be updated
                var game3 = _mapper.Map<Game>(newGameDto);
                setupCtx.Games.Add(game2);
                setupCtx.PlayerRatings.AddRange(pr);
                setupCtx.Games.Add(game3);
                setupCtx.SaveChanges();
            }
            _testLog.LogTrace("PutGameUpdateTest_PredictTeam2Wins Initialization Complete");

            // Act
            var actual = target.PutGameUpdate(3).Result;

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
            Assert.IsNotNull(gameInDb.GamePrediction);
            Assert.IsLessThan(0.5m, gameInDb.GamePrediction.T1predictedWinProb);
            Assert.AreEqual(11, gameInDb.GamePrediction.ExpectT2score);
            Assert.AreEqual(6, gameInDb.GamePrediction.ExpectT1score);
            var playerRatingsInDb = ctx.PlayerRatings
                .Where(pr => pr.GameId == newGameDto.GameId)
                .ToList();
            Assert.HasCount(4, playerRatingsInDb);
            for (int i = 0; i < playerRatingsInDb.Count; i++)
            {
                Assert.AreEqual(gameInDb.PlayedDate.Value.Date, playerRatingsInDb[1].RatingDate.Date);
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

        [TestMethod]
        [TestCategory("unit")]
        public void ExportRawGamesCsv_ReturnsCsvFile_WithSeededData()
        {
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper, log);

            // Act
            var actionResult = target.ExportRawGamesCsv().Result;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(FileContentResult));
            var fileResult = actionResult as FileContentResult;
            Assert.IsNotNull(fileResult);
            Assert.IsTrue(fileResult!.ContentType?.Contains("text/csv"), "Expected CSV content type.");
            Assert.IsNotNull(fileResult.FileContents);
            var content = Encoding.UTF8.GetString(fileResult.FileContents);
            // header should include the anonymous property names used when writing CSV
            Assert.Contains("GameId,FacilityId,PlayedDate,TypeGameId,TeamOnePlayerOneId,TeamOnePlayerTwoId,TeamOneScore,TeamTwoPlayerOneId,TeamTwoPlayerTwoId,TeamTwoScore,ChangedTime", content);
            // seeded game id expected in CSV body
            Assert.Contains("1,,2025-09-15T18:00:00.0000000-04:00,1,1,2,11,3,4,3,0", content);
            // verrify filename format
            Assert.IsFalse(string.IsNullOrWhiteSpace(fileResult.FileDownloadName));
            Assert.StartsWith("games_raw_", fileResult.FileDownloadName);
            var now = DateTime.UtcNow.ToString("yyyyMMdd");
            Assert.Contains(now, fileResult.FileDownloadName!);
            Assert.EndsWith(".csv", fileResult.FileDownloadName!);


        }

        [TestMethod]
        [TestCategory("unit")]
        public void ExportRawGamesCsv_ReturnsHeaderOnly_WhenNoGamesExist()
        {
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            using var ctx = new VprContext(_vprOpt);
            // Remove all data that would produce CSV rows
            ctx.PlayerRatings.RemoveRange(ctx.PlayerRatings);
            // Remove any GamePrediction rows (mapped entity but not a typed DbSet)
            var gpSet = ctx.Set<GamePrediction>();
            gpSet.RemoveRange(gpSet);
            ctx.Games.RemoveRange(ctx.Games);
            ctx.SaveChanges();

            var target = new GamesController(ctx, _mapper, log);

            // Act
            var actionResult = target.ExportRawGamesCsv().Result;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(FileContentResult));
            var fileResult = actionResult as FileContentResult;
            Assert.IsNotNull(fileResult);
            var content = Encoding.UTF8.GetString(fileResult!.FileContents);
            // Ensure CSV header exists and there are no non-header data rows
            char[] delimiters = [ '\r', '\n' ];
            var lines = content.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            // Expect exactly one non-empty line (the header) when there are no games
            Assert.HasCount(1, lines, "Expected only header line in CSV when database contains no games.");
            Assert.Contains("GameId", lines[0]);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void ExportRawGamesCsv_ContextThrowsException_Returns500()
        {
            // Arrange
            var log = _loggerFactory.CreateLogger<GamesController>();
            // Create a mock VprContext that throws when the Games property is accessed.
            var mockCtx = new Mock<VprContext>(_vprOpt) { CallBase = false };
            mockCtx.SetupGet(c => c.Games).Throws(new DbUpdateException("Concurrency exception"));

            var target = new GamesController(mockCtx.Object, _mapper, log);

            // Act
            var actionResult = Assert.ThrowsAsync<DbUpdateException>(()=>target.ExportRawGamesCsv()).Result;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(Exception));
            var excep = actionResult as Exception;
            Assert.IsNotNull(excep);
            //Assert.AreEqual(500, excep.StatusCode);
            Assert.Contains("Concurrency exception", excep.ToString()!);
        }






    }
}
