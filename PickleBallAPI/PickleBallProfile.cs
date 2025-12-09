using AutoMapper;
using PickleBallAPI.Controllers.DTO;
using PickleBallAPI.Models;

namespace PickleBallAPI
{
    // This is the approach starting with version 5
    public class PickleBallProfile : Profile
    {
        public PickleBallProfile()
        {
            CreateMap<TypeGameDto,TypeGame>()
                .ForMember(dest => dest.ChangedTime, opt => opt.Ignore())
                .ReverseMap()
                ;
            CreateMap<TypeFacilityDto, TypeFacility>()
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

            CreateMap<PlayerDto, Player>()
                .ForMember(dest => dest.ChangedTime, opt=>opt.Ignore())
                .ForMember(dest=> dest.PlayerRatings, opt=> opt.Ignore())
                .ReverseMap()
                ;
            CreateMap<GamePredictionDto, GamePrediction>()
                .ForMember(dest => dest.Game, opt => opt.Ignore())
                .ForMember(dest => dest.GameId, opt => opt.Ignore())
                .ReverseMap()
                ;
            CreateMap<FacilityDto, Facility>()
                .ForMember(dest => dest.ChangedTime, opt => opt.Ignore())
                .ForMember(dest => dest.TypeFacilityId, opt => opt.MapFrom(src => src.TypeFacility.TypeFacilityId))
                .ReverseMap()
                ;

            CreateMap<GameDto, Game>()
                .ForMember(dest => dest.GamePrediction, opt => opt.Ignore())
                .ForMember(dest => dest.ChangedTime, opt => opt.Ignore())
                .ForMember(dest => dest.FacilityId, opt=>opt.MapFrom(src => src.Facility!.FacilityId))
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
                })
                .ReverseMap()
                ;
        }
    }
}
