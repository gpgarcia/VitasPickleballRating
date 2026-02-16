using AutoMapper;
using Castle.Components.DictionaryAdapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PickleBallAPI;
using PickleBallAPI.Controllers;
using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPickleBallApi;

[TestClass]
public class PlayersControllerTests
{
    private SqliteConnection _connection = null!;
    private DbContextOptions<VprContext> _options = null!;
    private IMapper _mapper = null!;
    private ILoggerFactory _loggerFactory = null!;
    private TimeProvider time = null!;

    [TestInitialize]
    public void Setup()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            // Set default minimum log level for all categories
            builder.SetMinimumLevel(LogLevel.Trace);
            // Set a specific log level for a category using a wildcard
            builder.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.Debug);

        });
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<VprContext>()
            .UseSqlite(_connection)
            .UseLoggerFactory(_loggerFactory)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        using var context = new VprContext(_options);
        context.Database.EnsureCreated();

        _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<PickleBallProfile>(), _loggerFactory));
        time = new FakeTimeProvider(DateTimeOffset.UtcNow);
        using var seedContext = new VprContext(_options);


        TestHelper.SetupLookupData(seedContext);
        TestHelper.SetupPlayerData(seedContext);
        TestHelper.SetupGameData(seedContext);

        List<PlayerRating> ratings =
        [
            new PlayerRating{ PlayerRatingId = 1, PlayerId = 1, GameId=1, Rating=400, RatingDate=DateTimeOffset.Parse("2025-01-01 18:11"), Player=null! },
            new PlayerRating{ PlayerRatingId = 2, PlayerId = 1, GameId=2, Rating=500, RatingDate=DateTimeOffset.Parse("2025-06-01 18:22"), Player=null! },
            new PlayerRating{ PlayerRatingId = 3, PlayerId = 1, GameId=3, Rating=600, RatingDate=DateTimeOffset.Parse("2025-09-15 18:33"), Player=null! },
        ];
        seedContext.PlayerRatings.AddRange(ratings);

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
        var controller = new PlayerController(context, _mapper, time, _loggerFactory.CreateLogger<PlayerController>());

        var result = await controller.GetPlayers();

        Assert.IsNotNull(result.Result);
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (result.Result as OkObjectResult)!;
        var players = okResult.Value as IEnumerable<PlayerDto>;
        Assert.IsNotNull(players);
        Assert.AreEqual(4, players.Count());
    }

    [TestMethod]
    public async Task GetPlayer_ReturnsPlayer_WhenExists()
    {
        using var context = new VprContext(_options);
        var controller = new PlayerController(context, _mapper, time, _loggerFactory.CreateLogger<PlayerController>());

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
        var controller = new PlayerController(context, _mapper, time, _loggerFactory.CreateLogger<PlayerController>());

        var result = await controller.GetPlayer(-1);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task GetPlayerRatings_ReturnsRatings()
    {
        using var context = new VprContext(_options);
        var controller = new PlayerController(context, _mapper, time, _loggerFactory.CreateLogger<PlayerController>());

        var result = await controller.GetPlayerRatings(1);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var okResult = (result.Result as OkObjectResult)!;
        Assert.IsNotNull(okResult.Value);
        var ratings = okResult.Value as IEnumerable<PlayerRatingDto>;
        Assert.IsNotNull(ratings);
        Assert.AreEqual(3, ratings.Count());
    }

    [TestMethod]
    public async Task GetPlayerRatings_NotFound()
    {
        using var context = new VprContext(_options);
        var controller = new PlayerController(context, _mapper, time, _loggerFactory.CreateLogger<PlayerController>());

        var result = await controller.GetPlayerRatings(-1);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    [TestMethod]
    public async Task GetLatestRatingBeforeDate_ReturnsCorrectRating()
    {
        using var context = new VprContext(_options);
        var controller = new PlayerController(context, _mapper, time, _loggerFactory.CreateLogger<PlayerController>());

        var result = await controller.GetLatestRatingBeforeDate(1, new DateTime(2025, 09, 01)) as OkObjectResult;

        Assert.IsNotNull(result);
        var rating = result.Value as PlayerRatingDto;
        Assert.IsNotNull(rating);
        Assert.AreEqual(500, rating.Rating);
    }

    [TestMethod]
    public async Task GetLatestRatingBeforeDate_ReturnsNotFound_WhenNoMatch()
    {
        using var context = new VprContext(_options);
        var controller = new PlayerController(context, _mapper, time, _loggerFactory.CreateLogger<PlayerController>());

        var result = await controller.GetLatestRatingBeforeDate(1, new DateTime(2024, 01, 01));

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task PostPlayer_CreatesNewPlayer()
    {
        using var context = new VprContext(_options);
        var controller = new PlayerController(context, _mapper, time, _loggerFactory.CreateLogger<PlayerController>());

        var newPlayerDto = new PlayerDto ( FirstName : "New", LastName : "Player" );
        var result = await controller.PostPlayer(newPlayerDto);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        Assert.AreEqual("New", newPlayerDto.FirstName);
    }

    [TestMethod]
    public async Task PutPlayer_UpdatesPlayer()
    {
        using var context = new VprContext(_options);
        var controller = new PlayerController(context, _mapper, time,_loggerFactory.CreateLogger<PlayerController>());

        var updatedDto = new PlayerDto { PlayerId = 1, FirstName = "Updated", LastName = "Player" };
        var result = await controller.PutPlayer(1, updatedDto);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public async Task PutPlayer_ReturnsBadRequest_WhenIdMismatch()
    {
        using var context = new VprContext(_options);
        var controller = new PlayerController(context, _mapper, time, _loggerFactory.CreateLogger<PlayerController>());

        var updatedDto = new PlayerDto { PlayerId = 99, FirstName = "Updated", LastName = "Player" };
        var result = await controller.PutPlayer(1, updatedDto);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
    }


    [TestMethod]
    [TestCategory("unit")]
    public void ExportRawPlayersCsv_ReturnsCsvFile_WithSeededData()
    {
        // Arrange
        var log = _loggerFactory.CreateLogger<PlayerController>();
        using var ctx = new VprContext(_options);
        var target = new PlayerController(ctx, _mapper, time, log);

        // Act
        var actionResult = target.ExportRawPlayerCsvAsync().Result;

        // Assert
        Assert.IsNotNull(actionResult);
        Assert.IsInstanceOfType(actionResult, typeof(FileContentResult));
        var fileResult = actionResult as FileContentResult;
        Assert.IsNotNull(fileResult);
        Assert.IsTrue(fileResult!.ContentType?.Contains("text/csv"), "Expected CSV content type.");
        Assert.IsNotNull(fileResult.FileContents);
        var content = Encoding.UTF8.GetString(fileResult.FileContents);
        // header should include the anonymous property names used when writing CSV
        Assert.Contains("﻿PlayerId,FirstName,NickName,LastName,ChangedTime", content);
        // seeded game id expected in CSV body
        Assert.Contains("1,Test,,Player1,0", content);
        // verrify filename format
        Assert.IsFalse(string.IsNullOrWhiteSpace(fileResult.FileDownloadName));
        Assert.StartsWith("player_raw_", fileResult.FileDownloadName);
        var now = DateTime.UtcNow.ToString("yyyyMMdd");
        Assert.Contains(now, fileResult.FileDownloadName!);
        Assert.EndsWith(".csv", fileResult.FileDownloadName!);


    }

    [TestMethod]
    [TestCategory("unit")]
    public void ExportRawPlayersCsv_ReturnsHeaderOnly_WhenNoPlayersExist()
    {
        // Arrange
        var log = _loggerFactory.CreateLogger<PlayerController>();
        using var ctx = new VprContext(_options);
        // Remove all data that would produce CSV rows
        ctx.PlayerRatings.RemoveRange(ctx.PlayerRatings);
        // Remove any GamePrediction rows (mapped entity but not a typed DbSet)
        var gpSet = ctx.Set<GamePrediction>();
        gpSet.RemoveRange(gpSet);
        ctx.Games.RemoveRange(ctx.Games);
        ctx.Players.RemoveRange(ctx.Players); // remove player
        ctx.SaveChanges();

        var target = new PlayerController(ctx, _mapper, time, log);

        // Act
        var actionResult = target.ExportRawPlayerCsvAsync().Result;

        // Assert
        Assert.IsNotNull(actionResult);
        Assert.IsInstanceOfType(actionResult, typeof(FileContentResult));
        var fileResult = actionResult as FileContentResult;
        Assert.IsNotNull(fileResult);
        var content = Encoding.UTF8.GetString(fileResult!.FileContents);
        // Ensure CSV header exists and there are no non-header data rows
        var lines = content.Split([ '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        // Expect exactly one non-empty line (the header) when there are no players
        Assert.HasCount(1, lines, "Expected only header line in CSV when database contains no players.");
        Assert.Contains("PlayerId", lines[0]);
    }

    [TestMethod]
    [TestCategory("unit")]
    public void ExportRawPlayersCsv_ContextThrowsException_Returns500()
    {
        // Arrange
        var log = _loggerFactory.CreateLogger<PlayerController>();
        // Create a mock VprContext that throws when the Games property is accessed.
        var mockCtx = new Mock<VprContext>(_options) { CallBase = false };
        mockCtx.SetupGet(c => c.Players).Throws(new DbUpdateException("Concurrency exception"));

        var target = new PlayerController(mockCtx.Object, _mapper, time, log);

        // Act
        var actionResult = Assert.ThrowsAsync<DbUpdateException>(() => target.ExportRawPlayerCsvAsync()).Result;

        // Assert
        Assert.IsNotNull(actionResult);
        Assert.IsInstanceOfType(actionResult, typeof(Exception));
        var excep = actionResult as Exception;
        Assert.IsNotNull(excep);
        //Assert.AreEqual(500, excep.StatusCode);
        Assert.Contains("Concurrency exception", excep.ToString()!);
    }

}