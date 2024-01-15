using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Features.DepthChart.RequestHandlers
{
    public class RemovePlayerFromDepthChartRequest(string playerPosition, int playerId) : IRequest<IEnumerable<Player>>
    {
        public string PlayerPosition { get; init; } = playerPosition;
        public int PlayerId { get; init; } = playerId;
    }

    public class RemovePlayerFromDepthChartRequestHandler : IRequestHandler<RemovePlayerFromDepthChartRequest, IEnumerable<Player>>
    {
        private readonly DepthChartContext _dbContext;
        private readonly ILogger<RemovePlayerFromDepthChartRequestHandler> _logger;

        public RemovePlayerFromDepthChartRequestHandler(DepthChartContext dbContext, ILogger<RemovePlayerFromDepthChartRequestHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Player>> Handle(RemovePlayerFromDepthChartRequest request, CancellationToken token)
        {
            var depthsPositionsToRemove = await _dbContext.DepthPositions
                .Where(dp => dp.PlayerPosition.Equals(request.PlayerPosition) && dp.PlayerId.Equals(request.PlayerId))
                .ToListAsync(token);

            _dbContext.DepthPositions.RemoveRange(depthsPositionsToRemove);
            return await RemoveDepthPostionsFromDatabase(depthsPositionsToRemove, request, token);
        }

        private async Task<IEnumerable<Player>> RemoveDepthPostionsFromDatabase(IEnumerable<DepthPosition> depthPositionsToRemove, RemovePlayerFromDepthChartRequest request, CancellationToken token)
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to remove players from depth postions with playerId: {playerId}, player position: {playerPosition}.", request.PlayerId, request.PlayerPosition);
                return Enumerable.Empty<Player>();
            }

            var player = depthPositionsToRemove.Select(dp => dp.Player).FirstOrDefault();
            return player is null ? Enumerable.Empty<Player>() : new List<Player> { player };
        }
    }
}