using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PickleBallAPI;
using PickleBallAPI.Controllers;
using PickleBallAPI.Models;

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
            _loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
            _connection = new SqliteConnection("DataSource=:memory:");
            _vprOpt = new DbContextOptionsBuilder<VprContext>()
                .UseSqlite(_connection)
                .Options;
            _connection.Open();
            using var ctx = new VprContext(_vprOpt);
            ctx.Database.EnsureCreated();
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<PickleBallProfile> (), _loggerFactory));

            using (var setupCtx = new VprContext(_vprOpt))
            {
                Player player =
                    new Player
                    {
                        PlayerId = 1,
                        FirstName = "Test",
                        LastName = "Player"
                    };
                setupCtx.Players.Add(player);

                var playerRating =
                    new PlayerRating
                    {
                        PlayerRatingId = 1,
                        PlayerId = 1,
                        Rating = 300,
                        RatingDate = DateTimeOffset.Now,
                        //GameId = 1
                    };
                setupCtx.PlayerRatings.Add(playerRating);
                setupCtx.SaveChanges();
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
            _connection?.Close();
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
        [TestMethod]
        [TestCategory("unit")]
        public void PostPlayerRatingTest()
        {
            // Arrange
            using var ctx = new VprContext(_vprOpt);

            PlayerRatingDto prDto = new PlayerRatingDto()
            {
                PlayerId = 1,
                Rating = 300,
                RatingDate = DateTimeOffset.Now,
                //GameId = 1
            };
            var target = new PlayerRatingsController(ctx, _mapper);
            // Act
            var actual = target.PostPlayerRating(prDto).Result;
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<ActionResult<PlayerRatingDto>>(actual);
            Assert.IsNotNull(actual.Result);
            Assert.IsNull(actual.Value);
            var a = ctx.PlayerRatings.FirstOrDefault(p => p.PlayerRatingId == 2);
            Assert.IsNotNull(a);
            Assert.AreEqual(prDto.Rating, a.Rating);
            Assert.AreEqual(prDto.RatingDate.Date, a.RatingDate.Date);

        }


    }
}
