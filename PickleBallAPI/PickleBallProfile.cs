using AutoMapper;
using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;
using System;

namespace PickleBallAPI
{
    // This is the approach starting with version 5
    public class PickleBallProfile : Profile
    {
        public PickleBallProfile()
        {
            CreateMap<TypeGame, TypeGameDto>()
                .ForCtorParam("TypeGameId", opt => opt.MapFrom(src => src.TypeGameId))
                .ForCtorParam("Name", opt => opt.MapFrom(src => src.Name))
                .ReverseMap()
                ;
            CreateMap<TypeFacility, TypeFacilityDto>()
                .ForCtorParam("TypeFacilityId", opt => opt.MapFrom(src => src.TypeFacilityId))
                .ForCtorParam("Name", opt => opt.MapFrom(src => src.Name))
                .ReverseMap()
                ;

            CreateMap<PlayerRatingDto, PlayerRating>()
                .ForMember(dest => dest.ChangedTime, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    // Ensure navigation properties are null to avoid EF Core tracking issues
                    dest.Player = null!;
                })
                .ReverseMap()
                ;

            CreateMap<Player, PlayerDto>()
                .ForCtorParam("PlayerId", opt => opt.MapFrom(src => src.PlayerId))
                .ForCtorParam("FirstName", opt => opt.MapFrom(src => src.FirstName))
                .ForCtorParam("NickName", opt => opt.MapFrom(src => src.NickName))
                .ForCtorParam("LastName", opt => opt.MapFrom(src => src.LastName))
                .ReverseMap()
                ;
            CreateMap<GamePredictionDto, GamePrediction>()
                .ForMember(dest => dest.Game, opt => opt.Ignore())
                .ForMember(dest => dest.GameId, opt => opt.Ignore())
                .ReverseMap()
                ;
            CreateMap<Facility, FacilityDto>()
                .ForCtorParam("FacilityId", opt => opt.MapFrom(src => src.FacilityId))
                .ForCtorParam("Name", opt => opt.MapFrom(src => src.Name))
                .ForCtorParam("AddressLine1", opt => opt.MapFrom(src => src.AddressLine1))
                .ForCtorParam("AddressLine2", opt => opt.MapFrom(src => src.AddressLine2))
                .ForCtorParam("City", opt => opt.MapFrom(src => src.City))
                .ForCtorParam("StateCode", opt => opt.MapFrom(src => src.StateCode))
                .ForCtorParam("PostalCode", opt => opt.MapFrom(src => src.PostalCode))
                .ForCtorParam("NumberCourts", opt => opt.MapFrom(src => src.NumberCourts))
                .ForCtorParam("TypeFacility", opt => opt.MapFrom(src => src.TypeFacility))
                .ForCtorParam("Notes", opt => opt.MapFrom(src => src.Notes))
                .ReverseMap()
                ;

            CreateMap< Game, GameDto>()
                .ForCtorParam("GameId", opt => opt.MapFrom(src => src.GameId))
                .ForCtorParam("PlayedDate", opt => opt.MapFrom(src => src.PlayedDate))
                .ForCtorParam("Facility", opt => opt.MapFrom(src => src.Facility))
                .ForCtorParam("TypeGameId", opt => opt.MapFrom(src => src.TypeGameId))
                .ForCtorParam("TeamOneScore", opt => opt.MapFrom(src => src.TeamOneScore))
                .ForCtorParam("TeamTwoScore", opt => opt.MapFrom(src => src.TeamTwoScore))
                .ForCtorParam("TeamOnePlayerOne", opt => opt.MapFrom(src => src.TeamOnePlayerOne))
                .ForCtorParam("TeamOnePlayerTwo", opt => opt.MapFrom(src => src.TeamOnePlayerTwo))
                .ForCtorParam("TeamTwoPlayerOne", opt => opt.MapFrom(src => src.TeamTwoPlayerOne))
                .ForCtorParam("TeamTwoPlayerTwo", opt => opt.MapFrom(src => src.TeamTwoPlayerTwo))
                .ForCtorParam("TypeGame", opt => opt.MapFrom(src => src.TypeGame))
                .ForCtorParam("GamePrediction", opt => opt.MapFrom(src => src.GamePrediction))
                // Ensure Player navigation properties are null to avoid EF Core tracking issues
                ;
            CreateMap<GameDto, Game>()
                 .ForMember(dest => dest.GamePrediction, opt => opt.Ignore())
                 .ForMember(dest => dest.ChangedTime, opt => opt.Ignore())
                 .ForMember(dest => dest.FacilityId, opt => opt.MapFrom(src => src.Facility!.FacilityId))
                 .ForMember(dest => dest.TeamOnePlayerOneId, opt => opt.MapFrom(src => src.TeamOnePlayerOne.PlayerId))
                 .ForMember(dest => dest.TeamOnePlayerTwoId, opt => opt.MapFrom(src => src.TeamOnePlayerTwo.PlayerId))
                 .ForMember(dest => dest.TeamTwoPlayerOneId, opt => opt.MapFrom(src => src.TeamTwoPlayerOne.PlayerId))
                 .ForMember(dest => dest.TeamTwoPlayerTwoId, opt => opt.MapFrom(src => src.TeamTwoPlayerTwo.PlayerId))
                 .AfterMap((src, dest) =>
                    {
                        // Ensure Player navigation properties are null to avoid EF Core tracking issues
                        dest.TeamOnePlayerOne = null!;
                        dest.TeamOnePlayerTwo = null!;
                        dest.TeamTwoPlayerOne = null!;
                        dest.TeamTwoPlayerTwo = null!;
                        dest.TypeGame = null!;
                        dest.GamePrediction = null!;
                        dest.Facility = null!;
                    });

            CreateMap<DateTimeOffset, long>()
                .ConvertUsing(src => src.ToUnixTimeMilliseconds());

            CreateMap<long, DateTimeOffset>()
                .ConvertUsing(src => DateTimeOffset.FromUnixTimeMilliseconds(src));

            CreateMap<Game, GameRawDto>()
                .ForCtorParam("GameId", opt => opt.MapFrom(src => src.GameId))
                .ForCtorParam("FacilityId", opt => opt.MapFrom(src => src.FacilityId))
                .ForCtorParam("PlayedDate", opt => opt.MapFrom(src => src.PlayedDate.Value.ToString("o")))
                .ForCtorParam("TypeGameId", opt => opt.MapFrom(src => src.TypeGameId))
                .ForCtorParam("TeamOnePlayerOneId", opt => opt.MapFrom(src => src.TeamOnePlayerOneId))
                .ForCtorParam("TeamOnePlayerTwoId", opt => opt.MapFrom(src => src.TeamOnePlayerTwoId))
                .ForCtorParam("TeamOneScore", opt => opt.MapFrom(src => src.TeamOneScore))
                .ForCtorParam("TeamTwoPlayerOneId", opt => opt.MapFrom(src => src.TeamTwoPlayerOneId))
                .ForCtorParam("TeamTwoPlayerTwoId", opt => opt.MapFrom(src => src.TeamTwoPlayerTwoId))
                .ForCtorParam("TeamTwoScore", opt => opt.MapFrom(src => src.TeamTwoScore))
                .ForCtorParam("ChangedTime", opt => opt.MapFrom(src => src.ChangedTime))
                .ReverseMap()
                ;


            CreateMap<Player, PlayerRawDto>()
                .ForCtorParam("PlayerId", opt => opt.MapFrom(src => src.PlayerId))
                .ForCtorParam("FirstName", opt => opt.MapFrom(src => src.FirstName))
                .ForCtorParam("NickName", opt => opt.MapFrom(src => src.NickName))
                .ForCtorParam("LastName", opt => opt.MapFrom(src => src.LastName))
                .ForCtorParam("ChangedTime", opt => opt.MapFrom(src => src.ChangedTime))
                .ReverseMap()
                ;

        }
    }
}
