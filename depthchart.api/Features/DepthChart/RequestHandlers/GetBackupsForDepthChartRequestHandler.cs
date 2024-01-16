using AutoMapper;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Models;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Features.DepthChart.RequestHandlers
{
    public class GetBackupsForDepthChartRequest(string playerPosition, int playerId) : IRequest<IEnumerable<PlayerDto>>
    {
        public string PlayerPosition { get; init; } = playerPosition;
        public int PlayerId { get; init; } = playerId;
    }

    public class GetBackupsForDepthChartRequestHandler : IRequestHandler<GetBackupsForDepthChartRequest, IEnumerable<PlayerDto>>
    {
        private readonly IMapper _mapper;
        private readonly DepthChartContext _dbContext;

        public GetBackupsForDepthChartRequestHandler(IMapper mapper, DepthChartContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PlayerDto>> Handle(GetBackupsForDepthChartRequest request, CancellationToken token)
        {
            var depthPosition = await _dbContext.DepthPositions
                .Where(dp => dp.PlayerPosition.Equals(request.PlayerPosition) && dp.PlayerId.Equals(request.PlayerId))
                .Include(dp => dp.Player)
                .FirstOrDefaultAsync(token);

            if (depthPosition is null) return Enumerable.Empty<PlayerDto>();

            var depthPositionBackups = await _dbContext.DepthPositions
                .Where(dp => dp.PlayerPosition.Equals(depthPosition.PlayerPosition) && dp.Depth > depthPosition.Depth)
                .OrderBy(dp => dp.Depth)
                .Include(dp => dp.Player)
                .ToListAsync(token);

            var players = depthPositionBackups.Map(dp => dp.Player);
            return _mapper.Map<IEnumerable<PlayerDto>>(players);
        }
    }
}