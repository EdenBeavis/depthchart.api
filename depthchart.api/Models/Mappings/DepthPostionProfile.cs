using AutoMapper;
using depthchart.api.Infrastructure.Data.Entities;

namespace depthchart.api.Models.Mappings
{
    public class DepthPostionProfile : Profile
    {
        public DepthPostionProfile()
        {
            CreateMap<DepthPositionDto, DepthPosition>();
            CreateMap<DepthPosition, DepthPositionDto>();
        }
    }
}