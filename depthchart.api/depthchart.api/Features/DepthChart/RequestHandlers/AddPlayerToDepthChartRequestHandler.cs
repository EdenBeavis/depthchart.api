using AutoMapper;
using Bolt.Common.Extensions;
using depthchart.api.Infrastructure;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using depthchart.api.Models;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Features.DepthChart.RequestHandlers
{
    public class AddPlayerToDepthChartRequest(string playerPosition, int playerId, int? positionDepth) : IRequest<TryAsync<DepthPositionDto>>
    {
        public string PlayerPosition { get; init; } = playerPosition;
        public int PlayerId { get; init; } = playerId;
        public int? Depth { get; init; } = positionDepth;
    }

    public class AddPlayerToDepthChartRequestHandler : IRequestHandler<AddPlayerToDepthChartRequest, TryAsync<DepthPositionDto>>
    {
        private readonly IMapper _mapper;
        private readonly DepthChartContext _dbContext;

        public AddPlayerToDepthChartRequestHandler(IMapper mapper, DepthChartContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<TryAsync<DepthPositionDto>> Handle(AddPlayerToDepthChartRequest request, CancellationToken token)
        {
            var player = await _dbContext.Players.FirstOrDefaultAsync(p => p.Id == request.PlayerId, token);

            return async () => await AddPlayerToDepthChart(request, player!, token);
        }

        private async Task<DepthPositionDto> AddPlayerToDepthChart(AddPlayerToDepthChartRequest request, Player player, CancellationToken token)
        {
            if (player is null) throw new ArgumentException(Constants.ErrorMessages.PlayerDoesNotExist);

            var depthPositions = await DepthPositionsWithSamePlayerPosition(request, token);
            var maxDepth = depthPositions.HasItem() ? depthPositions.Max(dp => dp.Depth) + Constants.AddtionalPositionDepth : Constants.MinimumPositionDepth;
            var depth = GetDepth(request.Depth, maxDepth);

            var newDepthPosition = CreateAndAddDepthPosition(request.PlayerPosition, player, depth);

            if (!depthPositions.IsEmpty() && !newDepthPosition.Depth.Equals(maxDepth))
                UpdateOtherDepthPositions(depthPositions, newDepthPosition.Depth);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<DepthPositionDto>(newDepthPosition);
        }

        private async Task<IEnumerable<DepthPosition>> DepthPositionsWithSamePlayerPosition(AddPlayerToDepthChartRequest request, CancellationToken token)
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