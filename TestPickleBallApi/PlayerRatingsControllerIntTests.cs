using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PickleBallAPI;
using PickleBallAPI.Controllers;
using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace TestPickleBallApi
{
    [TestClass]
    public sealed class PlayerRatingsControllerIntTests
    {
        private DbContextOptions<VprContext> _vprOpt = null!;
        private IMapper _mapper = null!;
        private ILoggerFactory _loggerFactory = null!;

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
            _vprOpt = new DbContextOptionsBuilder<VprContext>()
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .UseSqlServer("Server=(localdb)\\ProjectModels;Database=vpr;Integrated Security=True;")
                .Options;
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<PickleBallProfile> (), _loggerFactory));

        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
            _loggerFactory?.Dispose();
        }

        [TestMethod]
        [TestCategory("integration")]
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
        }

        [TestMethod]
        [TestCategory("integration")]
        public void GetPlayerRatingTest_ValueFound()
        {
            //
            // NOTE: valid after update NOT initial
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new PlayerRatingsController(ctx, _mapper);
            var setup = new GamesController(ctx, _mapper, _loggerFactory.CreateLogger<GamesController>());
            var x = setup.PutGameUpdate().Result;   
            Assert.IsNotNull(x);
            var ncResult = x as NoContentResult;
            Assert.IsNotNull(ncResult);
            // Act
            var actual = target.GetPlayerRating(1).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<OkObjectResult>(actual.Result);
            var result = actual.Result as OkObjectResult;
            Assert.IsNotNull(result);
            var value = result.Value as PlayerRatingDto;
            Assert.IsNotNull(value);
            Assert.AreEqual(1, value.PlayerRatingId);
        }


        [TestMethod]
        [TestCategory("integration")]
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
            Assert.IsNull(actual.Value);
        }

    }
}
