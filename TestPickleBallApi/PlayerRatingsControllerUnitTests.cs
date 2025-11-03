using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PickleBallAPI;
using PickleBallAPI.Controllers;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestPickleBallApi
{
    [TestClass]
    public sealed class PlayerRatingsControllerUnitTests
    {
        private DbContextOptions<VprContext> _vprOpt = null!;
        private IMapper _mapper = null!;
        private ILoggerFactory _loggerFactory = null!;
        SqliteConnection _connection = null!;

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
                builder.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.Debug);

            });

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

            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<PickleBallProfile> (), _loggerFactory));

            using var setupCtx = new VprContext(_vprOpt);
            TestHelper.SetupLookupData(setupCtx);
            TestHelper.SetupPlayerData(setupCtx);
            TestHelper.SetupGameData(setupCtx);


            var playerRating = new PlayerRating
            {
                PlayerRatingId = 1,
                PlayerId = 1,
                Rating = 300,
                RatingDate = DateTimeOffset.Now,
                GameId = 1
            };
            setupCtx.PlayerRatings.Add(playerRating);
            setupCtx.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
            _connection?.Close();
            _loggerFactory?.Dispose();
        }

        [TestMethod]
        [TestCategory("unit")]
        public void CtorTest()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            // Act
            var target = new PlayerRatingsController(ctx,_mapper);
            // Assert
            Assert.IsNotNull(target);
            ctx.Dispose();  //double dispose test; no exceptions!!
        }

        [TestMethod]
        [TestCategory("unit")]
        public void GetPlayerRatingsTest_ReturnAllValues()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new PlayerRatingsController(ctx, _mapper);
            // Act
            var actual = target.GetPlayerRatings().Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<OkObjectResult>(actual.Result);
            var result = actual.Result as OkObjectResult;
            Assert.IsNotNull(result);
            var playerDto = (result.Value as PlayerDto)!;
            var list = result.Value as IEnumerable<PlayerRatingDto>;
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.First().PlayerRatingId);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void GetPlayerRatingTest_ValueFound()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new PlayerRatingsController(ctx, _mapper);
            // Act
            var actual = target.GetPlayerRating(1).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<OkObjectResult> (actual.Result);
            var result = actual.Result as OkObjectResult;
            var playerDto = (result?.Value as PlayerRatingDto)!;
            Assert.AreEqual(1, playerDto.PlayerRatingId);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void GetPlayerRatingTest_ValueNotFount()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new PlayerRatingsController(ctx, _mapper);
            // Act
            var actual = target.GetPlayerRating(-1).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NotFoundResult>(actual.Result);
            var result = actual.Result as NotFoundResult;
            Assert.IsNotNull(result);
            result.StatusCode.Equals(404);  
            Assert.IsNull(actual.Value);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void PostPlayerRatingTest_ValidData()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);

            PlayerRatingDto prDto = new()
            {
                PlayerId = 1,
                Rating = 300,
                RatingDate = DateTimeOffset.Now,
                GameId = 2
            };
            var target = new PlayerRatingsController(ctx, _mapper);
            // Act
            var actual = target.PostPlayerRating(prDto).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<CreatedAtActionResult>(actual.Result);
            var result = actual.Result as CreatedAtActionResult;
            Assert.IsNotNull(result?.Value);
            var rating = result?.Value as PlayerRating;
            Assert.IsNotNull(rating);
            Assert.AreEqual(prDto.PlayerId, rating.PlayerId);
            Assert.AreEqual(prDto.Rating, rating.Rating);
            Assert.AreEqual(prDto.RatingDate, rating.RatingDate);

            var a = ctx.PlayerRatings.FirstOrDefault(p => p.PlayerRatingId == 2);
            Assert.IsNotNull(a);
            Assert.AreEqual(prDto.Rating, a.Rating);
            Assert.AreEqual(prDto.RatingDate.Date, a.RatingDate.Date);

        }

        [TestMethod]
        [TestCategory("unit")]
        public void DeletePlayerRatingTest_IdFound()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new PlayerRatingsController(ctx, _mapper);
            // Act
            var actual = target.DeletePlayerRating(1).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NoContentResult>(actual);

        }
        [TestMethod]
        [TestCategory("unit")]
        public void DeletePlayerRatingTest_IdNotFound()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new PlayerRatingsController(ctx, _mapper);
            // Act
            var actual = target.DeletePlayerRating(-11).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NotFoundResult>(actual);

        }

        [TestMethod]
        [TestCategory("unit")]
        public void PutPlayerRatingTest_IdMatch()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new PlayerRatingsController(ctx, _mapper);
            PlayerRatingDto prDto = new()
            {
                PlayerRatingId = 1,
                PlayerId = 1,
                GameId = 1,
                Rating = 400,
                RatingDate = DateTimeOffset.Now,
            };
            // Act
            var actual = target.PutPlayerRating(1, prDto).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NoContentResult>(actual);
            var a = ctx.PlayerRatings.FirstOrDefault(p => p.PlayerRatingId == 1);
            Assert.IsNotNull(a);
            Assert.AreEqual(prDto.Rating, a.Rating);
            Assert.AreEqual(prDto.RatingDate.Date, a.RatingDate.Date);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void PutPlayerRatingTest_IdMismatchMatch()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new PlayerRatingsController(ctx, _mapper);
            PlayerRatingDto prDto = new()
            {
                PlayerRatingId = 1,
            };
            // Act
            var actual = target.PutPlayerRating(7, prDto).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<BadRequestObjectResult>(actual);
            var result = actual as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Id mismatch", result.Value);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void PutPlayerRatingTest_IdNotFound()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new PlayerRatingsController(ctx, _mapper);
            PlayerRatingDto prDto = new()
            {
                PlayerRatingId = -1,
            };
            // Act
            var actual = target.PutPlayerRating(-1, prDto).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<NotFoundObjectResult>(actual);
            var result = actual as NotFoundObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("No Player Rating with PlayerRatingId=-1", result.Value);
        }
    }
}
