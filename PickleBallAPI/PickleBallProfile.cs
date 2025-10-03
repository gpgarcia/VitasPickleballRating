using AutoMapper;
using PickleBallAPI.Controllers;
using PickleBallAPI.Models;
using System.Collections.Generic;

namespace PickleBallAPI
{
    // This is the approach starting with version 5
    public class PickleBallProfile : Profile
    {
        public PickleBallProfile()
        {
            CreateMap<PlayerRating, PlayerRatingDto>().ReverseMap();
            CreateMap<TypeGame, TypeGameDto>().ReverseMap();
            CreateMap<Player, PlayerDto>().ReverseMap();
            //CreateMap<Game, GameDto>().ReverseMap();


            CreateMap<GameDto, Game>()
                .ForMember(dest => dest.TeamOnePlayerOneId, opt => opt.MapFrom(src => src.TeamOnePlayerOne.PlayerId))
                .ForMember(dest => dest.TeamOnePlayerTwoId, opt => opt.MapFrom(src => src.TeamOnePlayerTwo.PlayerId))
                .ForMember(dest => dest.TeamTwoPlayerOneId, opt => opt.MapFrom(src => src.TeamTwoPlayerOne.PlayerId))
                .ForMember(dest => dest.TeamTwoPlayerTwoId, opt => opt.MapFrom(src => src.TeamTwoPlayerTwo.PlayerId))
                .ForMember(dest => dest.TypeGameId, opt => opt.MapFrom(src => src.TypeGameId))
                .ForMember(dest => dest.PlayedDate, opt => opt.MapFrom(src => src.PlayedDate))
                .ForMember(dest => dest.TeamOneScore, opt => opt.MapFrom(src => src.TeamOneScore))
                .ForMember(dest => dest.TeamTwoScore, opt => opt.MapFrom(src => src.TeamTwoScore))
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId))
                .ForMember(dest => dest.GamePrediction, opt => opt.Ignore())
                .ForMember(dest => dest.PlayerRatings, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    // Ensure Player navigation properties are null to avoid EF Core tracking issues
                    dest.TeamOnePlayerOne = null!;
                    dest.TeamOnePlayerTwo = null!;
                    dest.TeamTwoPlayerOne = null!;
                    dest.TeamTwoPlayerTwo = null!;
                    dest.TypeGame = null!;
                    dest.GamePrediction = null!;
                    dest.PlayerRatings = null!;
                })
                .ReverseMap()
                ;

        }
    }
}
