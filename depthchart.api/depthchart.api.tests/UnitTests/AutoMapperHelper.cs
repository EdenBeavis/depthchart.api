using AutoMapper;
using depthchart.api.Models.Mappings;

namespace depthchart.api.tests.UnitTests
{
    public static class AutoMapperHelper
    {
        public static IMapper CreateTestMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new PlayerProfile());
                cfg.AddProfile(new DepthPostionProfile());
                cfg.AddProfile(new DepthChartRowProfile());
            });

            IMapper mapper = new Mapper(configuration);

            return mapper;
        }
    }
}