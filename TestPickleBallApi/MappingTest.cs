

using AutoMapper;
using Microsoft.Extensions.Logging;
using PickleBallAPI;
using PickleBallAPI.Controllers;
using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;
using System;

namespace TestPickleBallApi;

[TestClass]

public class MappingTest
{
    private IMapper _mapper = null!;
    private ILoggerFactory _loggerFactory = null!;

    [TestInitialize]
    public void TestInit()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            // Set default minimum log level for all categories
            builder.SetMinimumLevel(LogLevel.Trace);
            // Set a specific log level for a category using a wildcard
            builder.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.Information);

        });

        _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<PickleBallProfile>(), _loggerFactory));

    }

    [TestMethod]
    public void ConfigurationIsValidTest()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [TestMethod]
    public void MapToPatientDtoTest()
    {
        var player = new PickleBallAPI.Models.Player
        {
            PlayerId = 1,
            FirstName = "Test",
            LastName = "Player",
        };
        var expected = new PlayerDto
        (
            PlayerId: 1,
            FirstName: "Test",
            LastName: "Player"
        );
        var actual = _mapper.Map<PlayerDto>(player);
        Assert.AreEqual(expected.PlayerId, actual.PlayerId);
        Assert.AreEqual(expected.FirstName, actual.FirstName);
        Assert.AreEqual(expected.LastName, actual.LastName);
    }

    [TestMethod]
    public void MapToPatientTest()
    {
        var expected = new Player
        {
            PlayerId = 1,
            FirstName = "Test",
            LastName = "Player",
        };
        var player = new PlayerDto
        (
            PlayerId: 1,
            FirstName: "Test",
            LastName: "Player"
        );
        var actual = _mapper.Map<Player>(player);
        Assert.AreEqual(expected.PlayerId, actual.PlayerId);
        Assert.AreEqual(expected.FirstName, actual.FirstName);
        Assert.AreEqual(expected.LastName, actual.LastName);
    }

    [TestMethod]
    public void MapToGameTest()
    {
        var expected = new Game
        {
            GameId = 1,
            FacilityId =2,
            TeamOnePlayerOneId = 1,
            TeamOnePlayerTwoId = 2,
            TeamTwoPlayerOneId = 3,
            TeamTwoPlayerTwoId = 4,
            TeamOneScore = 11,
            TeamTwoScore = 8,
            TypeGameId = 1,
            PlayedDate = new DateTimeOffset(2025, 01, 01, 13, 15, 30,TimeSpan.FromHours(-4.0)),
            
        };
        var gameDto = new GameDto
        {
            GameId = 1,
            Facility = new FacilityDto ( FacilityId : 2 ),
            TeamOnePlayerOne = new PlayerDto ( PlayerId : 1, FirstName : "Test1", LastName : "Player1" ),
            TeamOnePlayerTwo = new PlayerDto ( PlayerId : 2, FirstName : "Test2", LastName : "Player2" ),
            TeamTwoPlayerOne = new PlayerDto ( PlayerId : 3, FirstName : "Test3", LastName : "Player3" ),
            TeamTwoPlayerTwo = new PlayerDto ( PlayerId : 4, FirstName : "Test4", LastName : "Player4" ),
            TeamOneScore = 11,
            TeamTwoScore = 8,
            TypeGameId = 1,
            PlayedDate = new DateTimeOffset(2025, 01, 01, 13, 15, 30, TimeSpan.FromHours(-4.0)),
            TypeGame = new TypeGameDto ( TypeGameId: 1, Name: "Recreational" )
        };
        Game actual = _mapper.Map<Game>(gameDto);
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.GameId, actual.GameId);
        Assert.AreEqual(expected.TeamOneScore, actual.TeamOneScore);
        Assert.AreEqual(expected.TeamTwoScore, actual.TeamTwoScore);
        Assert.AreEqual(expected.TypeGameId, actual.TypeGameId);
        Assert.AreEqual(expected.TeamOnePlayerOneId, actual.TeamOnePlayerOneId);
        Assert.AreEqual(expected.TeamOnePlayerTwoId, actual.TeamOnePlayerTwoId);
        Assert.AreEqual(expected.TeamTwoPlayerOneId, actual.TeamTwoPlayerOneId);
        Assert.AreEqual(expected.TeamTwoPlayerTwoId, actual.TeamTwoPlayerTwoId);
    }
}

