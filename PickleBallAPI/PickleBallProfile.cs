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
            CreateMap<Team, TeamDto>().ReverseMap();
            CreateMap<Game, GameDto>().ReverseMap();
        }
    }
}
