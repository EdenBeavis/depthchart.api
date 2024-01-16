using AutoMapper;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using depthchart.api.Models;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Features.DepthChart.RequestHandlers
{
    public class RemovePlayerFromDepthChartRequest(string playerPosition, int playerId) : IRequest<IEnumerable<PlayerDto>>
    {
        public string PlayerPosition { get; init; } = playerPosition;
        public int PlayerId { get; init; } = playerId;
    }

    public class RemovePlayerFromDepthChartRequestHandler : IRequestHandler<RemovePlayerFromDepthChartRequest, IEnumerable<PlayerDto>>
    {
        private readonly IMapper _mapper;
        private readonly DepthChartContext _dbContext;
        private readonly ILogger<RemovePlayerFromDepthChartRequestHandler> _logger;

        public RemovePlayerFromDepthChartRequestHandler(IMapper mapper, DepthChartContext dbContext, ILogger<RemovePlayerFromDepthChartRequestHandler> logger)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<PlayerDto>> Handle(RemovePlayerFromDepthChartRequest request, CancellationToken token)
        {
            var depthPositionsToRemove = _dbContext.DepthPositions
                .Where(dp => dp.PlayerPosition.Equals(request.PlayerPosition) && dp.PlayerId.Equals(request.PlayerId));

            var playersTask = await RemoveDepthPostionsFromDatabase(depthPositionsToRemove, request, token);
            var players = await playersTask.Match(p => p, ex =>
            {
                _logger.LogError(ex, "Error trying to remove players from depth postions with playerId: {playerId}, player position: {playerPosition}.", request.PlayerId, request.PlayerPosition);
                return Enumerable.Empty<Player>();
            });

            return _mapper.Map<IEnumerable<PlayerDto>>(players);
        }

        private async Task<TryAsync<IEnumerable<Player>>> RemoveDepthPostionsFromDatabase(IQueryable<DepthPosition> depthPositionsToRemove, RemovePlayerFromDepthChartRequest request, CancellationToken token)
        {
            return async () => (await Perform(depthPositionsToRemove, request, token)).ToList();
        }

        private async Task<IEnumerable<Player>> Perform(IQueryable<DepthPosition> depthPositionsToRemove, RemovePlayerFromDepthChartRequest request, CancellationToken token)
        {
            var player = await depthPositionsToRemove.Select(dp => dp.Player).FirstOrDefaultAsync(token);
            _dbContext.DepthPositions.RemoveRange(depthPositionsToRemove);
            await _dbContext.SaveChangesAsync(token);

            return player is null ? Enumerable.Empty<Player>() : new List<Player> { player };
        }
    }
}