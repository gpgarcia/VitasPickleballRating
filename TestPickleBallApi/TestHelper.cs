using PickleBallAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPickleBallApi
{
    public static class TestHelper
    {

        public static void SetupLookupData(VprContext seedContext)
        {

            List<TypeGame> typeGames =
            [
                new TypeGame{ TypeGameId = 1, Name="Recreational" },
                new TypeGame{ TypeGameId = 2, Name="Tournament" },
            ];
            seedContext.TypeGames.AddRange(typeGames);

            List<TypeFacility> typeFacilities =
            [
                new TypeFacility{ TypeFacilityId = 1, Name="Public" },
                new TypeFacility{ TypeFacilityId = 2, Name="Private" },
            ];
            seedContext.TypeFacilities.AddRange(typeFacilities);

            seedContext.Facilities.Add(new Facility
            {
                FacilityId = 1,
                Name = "Test Facility 1",
                TypeFacilityId = 1,
                NumberCourts = 4,
                AddressLine1 = "123 Main St",
                AddressLine2 = null,
                City = "Anytown",
                StateCode = "FL",
                PostalCode = "33130",
                ChangedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            });
        }

        public static void SetupPlayerData(VprContext seedContext)
        {
            List<Player> players =
            [
                new Player{ PlayerId = 1, FirstName = "Test", LastName = "Player1" },
                new Player{ PlayerId = 2, FirstName = "Test", LastName = "Player2" },
                new Player{ PlayerId = 3, FirstName = "Test", LastName = "Player3" },
                new Player{ PlayerId = 4, FirstName = "Test", LastName = "Player4" },
            ];
            seedContext.Players.AddRange(players);
        }

        public static void SetupGameData(VprContext seedContext)
        {
            List<Game> games =
            [
                new Game
                {
                    GameId = 1,
                    TeamOnePlayerOneId = 1,
                    TeamOnePlayerTwoId = 2,
                    TeamTwoPlayerOneId = 3,
                    TeamTwoPlayerTwoId = 4,
                    TypeGameId = 1,
                    TeamOnePlayerOne = null!,
                    TeamOnePlayerTwo = null!,
                    TeamTwoPlayerOne = null!,
                    TeamTwoPlayerTwo = null!,
                    TeamOneScore = 11,
                    TeamTwoScore = 8,
                    PlayedDate = DateTimeOffset.UtcNow.AddDays(-10)
                },
                new Game
                {
                    GameId = 2,
                    TeamOnePlayerOneId = 1,
                    TeamOnePlayerTwoId = 3,
                    TeamTwoPlayerOneId = 2,
                    TeamTwoPlayerTwoId = 4,
                    TypeGameId = 1,
                    TeamOnePlayerOne = null!,
                    TeamOnePlayerTwo = null!,
                    TeamTwoPlayerOne = null!,
                    TeamTwoPlayerTwo = null!,
                    TeamOneScore = 15,
                    TeamTwoScore = 13,
                    PlayedDate = DateTimeOffset.UtcNow.AddDays(-5)
                },
                new Game
                { 
                    GameId = 3,
                    TeamOnePlayerOneId=4,
                    TeamOnePlayerTwoId=2,
                    TeamTwoPlayerOneId=3,
                    TeamTwoPlayerTwoId=1,
                    TypeGameId=1,
                    TeamOnePlayerOne = null!,
                    TeamOnePlayerTwo = null!,
                    TeamTwoPlayerOne = null!,
                    TeamTwoPlayerTwo = null!,
                    TeamOneScore = 11,
                    TeamTwoScore = 1,
                    PlayedDate=DateTimeOffset.UtcNow.AddDays(-4)
                },

            ];
            seedContext.Games.AddRange(games);
        }
    }
}
