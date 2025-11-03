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
            CreateMap<TypeGameDto,TypeGame>()
                .ForMember(dest=> dest.Games, opt=>opt.Ignore())
                .ForMember(dest => dest.ChangedDate, opt => opt.Ignore())
                .ReverseMap()
                ;
            CreateMap<PlayerRatingDto, PlayerRating>()
                .ForMember(dest => dest.Game, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    // Ensure navigation properties are null to avoid EF Core tracking issues
                    dest.Player = null!;
                    dest.Game = null!;
                })
                .ReverseMap()
                ;

            CreateMap<PlayerDto, Player>()
                .ForMember(dest => dest.ChangedDate, opt=>opt.Ignore())
                .ForMember(dest=> dest.PlayerRatings, opt=> opt.Ignore())   
                //.ForMember(dest => dest.GameTeamOnePlayerOnes, opt => opt.Ignore())
                //.ForMember(dest => dest.GameTeamOnePlayerTwos, opt => opt.Ignore())
                //.ForMember(dest => dest.GameTeamTwoPlayerOnes, opt => opt.Ignore())
                //.ForMember(dest => dest.GameTeamTwoPlayerTwos, opt => opt.Ignore())
                .ReverseMap()
                ;

            CreateMap<GameDto, Game>()
                .ForMember(dest => dest.GamePrediction, opt => opt.Ignore())
                .ForMember(dest => dest.TeamOnePlayerOneId, opt => opt.MapFrom(src => src.TeamOnePlayerOne.PlayerId))
                .ForMember(dest => dest.TeamOnePlayerTwoId, opt => opt.MapFrom(src => src.TeamOnePlayerTwo.PlayerId))
                .ForMember(dest => dest.TeamTwoPlayerOneId, opt => opt.MapFrom(src => src.TeamTwoPlayerOne.PlayerId))
                .ForMember(dest => dest.TeamTwoPlayerTwoId, opt => opt.MapFrom(src => src.TeamTwoPlayerTwo.PlayerId))
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
                })
                .ReverseMap()
                ;
        }
    }
}
