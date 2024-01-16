using AutoMapper;
using depthchart.api.Infrastructure.Data.Entities;

namespace depthchart.api.Models.Mappings
{
    public class DepthChartRowProfile : Profile
    {
        public DepthChartRowProfile()
        {
            CreateMap<DepthChartRowDto, DepthChartRow>();
            CreateMap<DepthChartRow, DepthChartRowDto>();
        }
    }
}