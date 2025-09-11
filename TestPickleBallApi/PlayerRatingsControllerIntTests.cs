using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PickleBallAPI;
using PickleBallAPI.Controllers;
using PickleBallAPI.Models;

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
            _loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
            _vprOpt = new DbContextOptionsBuilder<VprContext>()
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
        [TestCategory("integration")]
        public void GetPlayerRatingTest_ValueFound()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new PlayerRatingsController(ctx, _mapper);
            // Act
            var actual = target.GetPlayerRating(1).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<ActionResult<PlayerRatingDto>>(actual);
            Assert.IsNotNull(actual.Value);
            Assert.AreEqual(1, actual.Value.PlayerRatingId);
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
            Assert.IsInstanceOfType<ActionResult<PlayerRatingDto>>(actual);
            Assert.IsNotNull(actual.Result);
            Assert.IsNull(actual.Value);
        }

    }
}
