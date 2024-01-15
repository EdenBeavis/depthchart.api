using AutoMapper;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using depthchart.api.Models;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Features.DepthChart.RequestHandlers
{
    public class GetDepthChartRequest : IRequest<IEnumerable<DepthChartRowDto>>
    {
    }

    public class GetDepthChartRequestHandler : IRequestHandler<GetDepthChartRequest, IEnumerable<DepthChartRowDto>>
    {
        private readonly IMapper _mapper;
        private readonly DepthChartContext _dbContext;

        public GetDepthChartRequestHandler(IMapper mapper, DepthChartContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<DepthChartRowDto>> Handle(GetDepthChartRequest request, CancellationToken token)
        {
            var depthChartRow = await _dbContext.DepthPositions
                .GroupBy(dp => dp.PlayerPosition)
                .OrderBy(g => g.Key)
                .Select(grouping =>
                    new DepthChartRow
                    {
                        Position = grouping.Key,
                        Players = grouping.OrderBy(dp => dp.Depth).Select(dp => dp.Player)
                    }
                )
                .ToListAsync(token);

            return _mapper.Map<IEnumerable<DepthChartRowDto>>(depthChartRow);
        }
    }
}