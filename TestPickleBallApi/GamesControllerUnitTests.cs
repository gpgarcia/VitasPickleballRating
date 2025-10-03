using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
                .UseSqlite(_connection)
                .Options;
            _connection.Open();
            using var ctx = new VprContext(_vprOpt);
            ctx.Database.EnsureCreated();
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<PickleBallProfile>(), _loggerFactory));

            using var setupCtx = new VprContext(_vprOpt);
            // Seed with a game and related data
            _testLog.LogTrace("Seeding test data Player...");
            var player1 = new Player
            {
                PlayerId = 1,
                FirstName = "Player",
                LastName = "One"
            };
            var player2 = new Player
            {
                PlayerId = 2,
                FirstName = "Player",
                LastName = "Two"
            };
            var player3 = new Player
            {
                PlayerId = 3,
                FirstName = "Player",
                LastName = "Three"
            };
            var player4 = new Player
            {
                PlayerId = 4,
                FirstName = "Player",
                LastName = "Four"
            };
            setupCtx.Players.AddRange(player1, player2, player3, player4);
            setupCtx.SaveChanges();

            _testLog.LogTrace("Seeding test data PlayerRating...");
            var p1r = new PlayerRating
            {
                PlayerRatingId = 1,
                PlayerId = 1,
                Rating = 200,
                RatingDate = new DateTimeOffset(2025, 09, 01, 18, 0, 0, TimeSpan.FromHours(-4.0)),

            };
            var p2r = new PlayerRating
            {
                PlayerRatingId = 2,
                PlayerId = 2,
                Rating = 600,
                RatingDate = new DateTimeOffset(2025, 09, 01, 18, 0, 0, TimeSpan.FromHours(-4.0)),

            };
            var p3r = new PlayerRating
            {
                PlayerRatingId = 3,
                PlayerId = 3,
                Rating = 300,
                RatingDate = new DateTimeOffset(2025, 09, 01, 18, 0, 0, TimeSpan.FromHours(-4.0)),
            };
            var p4r = new PlayerRating
            {
                PlayerRatingId = 4,
                PlayerId = 4,
                Rating = 500,
                RatingDate = new DateTimeOffset(2025, 09, 01, 18, 0, 0, TimeSpan.FromHours(-4.0)),
            };
            setupCtx.PlayerRatings.AddRange(p1r, p2r, p3r, p4r);
            setupCtx.SaveChanges();

            _testLog.LogTrace("Seeding test data TypeGame...");
            var typeGame1 = new TypeGame
            {
                TypeGameId = 1,
                GameType = "Recreational",

            };
            var typeGame2 = new TypeGame
            {
                TypeGameId = 2,
                GameType = "Tournament",

            };
            setupCtx.TypeGames.AddRange(typeGame1, typeGame2);
            setupCtx.SaveChanges();

            _testLog.LogTrace("Seeding test data Game...");

            var game = new Game
            {
                GameId = 1,
                PlayedDate = new DateTimeOffset(2025,9,15, 18,00,00, TimeSpan.FromHours(-4.0)),
                TeamOneScore = 11,
                TeamTwoScore = 3,
                TeamOnePlayerOneId = 1,
                TeamOnePlayerTwoId = 2,
                TeamTwoPlayerOneId = 3,
                TeamTwoPlayerTwoId = 4,
                TypeGameId = 1,

            };
            setupCtx.Games.Add(game);
            setupCtx.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
            _connection.Close();
            _loggerFactory.Dispose();
        }

        [TestMethod]
        [TestCategory("integration")]
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
            Assert.AreEqual(1, gameDto?.GameId);
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
            Assert.IsInstanceOfType<ActionResult<GameDto>>(actual);
            Assert.IsNotNull(actual.Result);
            Assert.IsNull(actual.Value);
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
            Assert.AreEqual(newGameDto.PlayedDate?.Date, gameInDb.PlayedDate?.Date);
            Assert.AreEqual(newGameDto.TeamOneScore, gameInDb.TeamOneScore);
            Assert.AreEqual(newGameDto.TeamTwoScore, gameInDb.TeamTwoScore);
            Assert.AreEqual(newGameDto.TeamOnePlayerOne.PlayerId, gameInDb.TeamOnePlayerOneId);
            Assert.AreEqual(newGameDto.TeamOnePlayerTwo.PlayerId, gameInDb.TeamOnePlayerTwoId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerOne.PlayerId, gameInDb.TeamTwoPlayerOneId);
            Assert.AreEqual(newGameDto.TeamTwoPlayerTwo.PlayerId, gameInDb.TeamTwoPlayerTwoId);

        }
    }
}
