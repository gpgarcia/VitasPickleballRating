using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using PickleBallAPI;
using PickleBallAPI.Controllers;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TestPickleBallApi
{
    [TestClass]
    public class PlayersControllerTests
    {
        private SqliteConnection _connection = null!;
        private DbContextOptions<VprContext> _options = null!;
        private IMapper _mapper = null!;
        private ILoggerFactory _loggerFactory = null!;

        [TestInitialize]
        public void Setup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<VprContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new VprContext(_options);
            context.Database.EnsureCreated();

            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<PickleBallProfile>(), _loggerFactory));

            using var seedContext = new VprContext(_options);
            var player = new Player { PlayerId = 1, FirstName = "Test", LastName = "Player" };
            seedContext.Players.Add(player);

            seedContext.PlayerRatings.AddRange(new List<PlayerRating>
            {
                new PlayerRating { PlayerRatingId = 1, PlayerId = 1, Rating = 400, RatingDate = new DateTime(2025, 01, 01) },
                new PlayerRating { PlayerRatingId = 2, PlayerId = 1, Rating = 500, RatingDate = new DateTime(2025, 06, 01) },
                new PlayerRating { PlayerRatingId = 3, PlayerId = 1, Rating = 600, RatingDate = new DateTime(2025, 09, 15) }
            });

            seedContext.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _connection.Close();
            _loggerFactory.Dispose();
        }

        [TestMethod]
        public async Task GetPlayers_ReturnsAllPlayers()
        {
            using var context = new VprContext(_options);
            var controller = new PlayersController(context, _mapper);

            var result = await controller.GetPlayers();

            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (result.Result as OkObjectResult)!;
            var players = okResult.Value as IEnumerable<PlayerDto>;
            Assert.AreEqual(1, players?.Count());
        }

        [TestMethod]
        public async Task GetPlayer_ReturnsPlayer_WhenExists()
        {
            using var context = new VprContext(_options);
            var controller = new PlayersController(context, _mapper);

            var result = await controller.GetPlayer(1);

            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType<OkObjectResult>(result.Result);
            var okResult = (result.Result as OkObjectResult)!;
            var playerDto = (okResult.Value as PlayerDto)!;
            Assert.AreEqual("Test", playerDto.FirstName);
        }

        [TestMethod]
        public async Task GetPlayer_ReturnsNotFound_WhenMissing()
        {
            using var context = new VprContext(_options);
            var controller = new PlayersController(context, _mapper);

            var result = await controller.GetPlayer(-1);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetPlayerRatings_ReturnsRatings()
        {
            using var context = new VprContext(_options);
            var controller = new PlayersController(context, _mapper);

            var result = await controller.GetPlayerRatings(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<OkObjectResult>(result.Result);
            var okResult = (result.Result as OkObjectResult)!;
            Assert.IsNotNull(okResult.Value);
            var ratings = okResult.Value as IEnumerable<PlayerRatingDto>;
            Assert.AreEqual(3, ratings?.Count());
        }

        [TestMethod]
        public async Task GetPlayerRatings_NotFound()
        {
            using var context = new VprContext(_options);
            var controller = new PlayersController(context, _mapper);

            var result = await controller.GetPlayerRatings(-1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<NotFoundResult>(result.Result);
        }

        [TestMethod]
        public async Task GetLatestRatingBeforeDate_ReturnsCorrectRating()
        {
            using var context = new VprContext(_options);
            var controller = new PlayersController(context, _mapper);

            var result = await controller.GetLatestRatingBeforeDate(1, new DateTime(2025, 09, 01)) as OkObjectResult;

            Assert.IsNotNull(result);
            var rating = result.Value as PlayerRating;
            Assert.AreEqual(500, rating?.Rating);
        }

        [TestMethod]
        public async Task GetLatestRatingBeforeDate_ReturnsNotFound_WhenNoMatch()
        {
            using var context = new VprContext(_options);
            var controller = new PlayersController(context, _mapper);

            var result = await controller.GetLatestRatingBeforeDate(1, new DateTime(2024, 01, 01));

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostPlayer_CreatesNewPlayer()
        {
            using var context = new VprContext(_options);
            var controller = new PlayersController(context, _mapper);

            var newPlayerDto = new PlayerDto { FirstName = "New", LastName = "Player" };
            var result = await controller.PostPlayer(newPlayerDto);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            Assert.AreEqual("New", newPlayerDto.FirstName);
        }

        [TestMethod]
        public async Task PutPlayer_UpdatesPlayer()
        {
            using var context = new VprContext(_options);
            var controller = new PlayersController(context, _mapper);

            var updatedDto = new PlayerDto { PlayerId = 1, FirstName = "Updated", LastName = "Player" };
            var result = await controller.PutPlayer(1, updatedDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task PutPlayer_ReturnsBadRequest_WhenIdMismatch()
        {
            using var context = new VprContext(_options);
            var controller = new PlayersController(context, _mapper);

            var updatedDto = new PlayerDto { PlayerId = 99, FirstName = "Updated", LastName = "Player" };
            var result = await controller.PutPlayer(1, updatedDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }
    }
}