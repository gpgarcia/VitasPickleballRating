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
    public sealed class GamesControllerTests
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
        }

        [TestMethod]
        [TestCategory("integration")]
        public void CtorTest()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            // Act
            var target = new GamesController(ctx,_mapper);
            // Assert
            Assert.IsNotNull(target);
            ctx.Dispose();  //double dispose test; no exceptions!!
        }
        [TestMethod]
        [TestCategory("integration")]
        public void GetGameTest_ValueFound()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper);
            // Act
            var actual = target.GetGame(1).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<ActionResult<GameDto>>(actual);
            Assert.IsNotNull(actual.Value);
            Assert.AreEqual(1, actual.Value.GameId);
        }
        [TestMethod]
        [TestCategory("integration")]
        public void GetGameTest_ValueNotFount()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);
            var target = new GamesController(ctx, _mapper);
            // Act
            var actual = target.GetGame(-1).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<ActionResult<GameDto>>(actual);
            Assert.IsNotNull(actual.Result);
            Assert.IsNull(actual.Value);
        }
    }
}
