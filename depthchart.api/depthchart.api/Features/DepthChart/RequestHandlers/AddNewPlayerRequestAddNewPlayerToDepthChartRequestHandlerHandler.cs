using Bolt.Common.Extensions;
using depthchart.api.Infrastructure;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Features.DepthChart.RequestHandlers
{
    public class AddNewPlayerToDepthChartRequest(string playerPosition, int playerId, int? positionDepth) : IRequest<Either<string, DepthPosition>>
    {
        public string PlayerPosition { get; init; } = playerPosition;
        public int PlayerId { get; init; } = playerId;
        public int? Depth { get; init; } = positionDepth;
    }

    public class AddNewPlayerToDepthChartRequestHandler : IRequestHandler<AddNewPlayerToDepthChartRequest, Either<string, DepthPosition>>
    {
        private readonly DepthChartContext _dbContext;
        private readonly ILogger<AddNewPlayerToDepthChartRequestHandler> _logger;

        public AddNewPlayerToDepthChartRequestHandler(DepthChartContext dbContext, ILogger<AddNewPlayerToDepthChartRequestHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Either<string, DepthPosition>> Handle(AddNewPlayerToDepthChartRequest request, CancellationToken token)
        {
            var player = await _dbContext.Players.FirstOrDefaultAsync(p => p.Id == request.PlayerId, token);

            if (player is null) return "Player does not exist.";

            var depthPositions = await DepthPositionsWithSamePlayerPosition(request, token);
            var maxDepth = depthPositions.Max(dp => dp.Depth) + Constants.AddtionalPositionDepth;
            var depth = GetDepth(request.Depth, maxDepth);

            var newDepthPosition = CreateAndAddDepthPosition(request.PlayerPosition, player, depth);

            if (!depthPositions.IsEmpty() && !newDepthPosition.Depth.Equals(maxDepth))
                UpdateOtherDepthPositions(depthPositions, newDepthPosition.Depth);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to add player and update other depth positions with player position: {playerPosition}, playerId: {playerId}, depth: {depth}.",
                    request.PlayerPosition, request.PlayerId, request.Depth);
                return "Unable to add player to depth chart";
            }

            return newDepthPosition;
        }

        private async Task<IEnumerable<DepthPosition>> DepthPositionsWithSamePlayerPosition(AddNewPlayerToDepthChartRequest request, CancellationToken token)
        {
            var depthPositionsQuery = _dbContext.DepthPositions.Where(dp => dp.PlayerPosition.Equals(request.PlayerPosition));

            if (request.Depth.HasValue)
                depthPositionsQuery = depthPositionsQuery.Where(dp => dp.Depth >= request.Depth.Value);

            return await depthPositionsQuery
                .OrderBy(dp => dp.Depth)
                .ToListAsync(token);
        }

        private int GetDepth(int? positionDepth, int maxDepth) =>
            positionDepth.HasValue && positionDepth.Value >= Constants.MinimumPositionDepth
                ? positionDepth.Value
                : maxDepth;

        private DepthPosition CreateAndAddDepthPosition(string playerPosition, Player player, int depth)
        {
            var newDepthPositon = new DepthPosition(playerPosition, player, depth);
            _dbContext.DepthPositions.Add(newDepthPositon);

            return newDepthPositon;
        }

        private void UpdateOtherDepthPositions(IEnumerable<DepthPosition> depthPositions, int currentDepth)
        {
            foreach (var depthPositon in depthPositions)
            {
                if (depthPositon.Depth.Equals(currentDepth))
                {
                    depthPositon.Depth++;
                    currentDepth++;
                }
                else
                    // There is a gap in the depth positions;
                    break;
            }

            _dbContext.DepthPositions.UpdateRange(depthPositions);
        }
    }
}