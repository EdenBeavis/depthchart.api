using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Features.DepthChart.RequestHandlers
{
    public class GetBackupsForDepthChartRequest(string playerPosition, int playerId) : IRequest<IEnumerable<Player>>
    {
        public string PlayerPosition { get; init; } = playerPosition;
        public int PlayerId { get; init; } = playerId;
    }

    public class GetBackupsForDepthChartRequestHandler : IRequestHandler<GetBackupsForDepthChartRequest, IEnumerable<Player>>
    {
        private readonly DepthChartContext _dbContext;

        public GetBackupsForDepthChartRequestHandler(DepthChartContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Player>> Handle(GetBackupsForDepthChartRequest request, CancellationToken token)
        {
            var depthPosition = await _dbContext.DepthPositions
                .Where(dp => dp.PlayerPosition.Equals(request.PlayerPosition) && dp.PlayerId.Equals(request.PlayerId))
                .FirstOrDefaultAsync(token);

            if (depthPosition is null) return Enumerable.Empty<Player>();

            var depthPositionBackups = await _dbContext.DepthPositions
                .Where(dp => dp.PlayerPosition.Equals(depthPosition.PlayerPosition) && dp.Depth > depthPosition.Depth)
                .OrderBy(dp => dp.Depth)
                .ToListAsync(token);

            return depthPositionBackups.Map(dp => dp.Player);
        }
    }
}