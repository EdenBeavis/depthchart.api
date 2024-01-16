using AutoMapper;
using depthchart.api.Infrastructure.Data.Entities;

namespace depthchart.api.Models.Mappings
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<PlayerDto, Player>();
            CreateMap<Player, PlayerDto>();
        }
    }
}