using AutoMapper;
using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using depthchart.api.Infrastructure.Enums;
using depthchart.api.Models;
using LanguageExt;
using MediatR;

namespace depthchart.api.Features.Players.RequestHandlers
{
    public class AddNewPlayerRequest(PlayerDto newPlayer) : IRequest<TryAsync<PlayerDto>>
    {
        public PlayerDto NewPlayer { get; init; } = newPlayer;
    }

    public class AddNewPlayerRequestHandler : IRequestHandler<AddNewPlayerRequest, TryAsync<PlayerDto>>
    {
        private readonly IMapper _mapper;
        private readonly DepthChartContext _dbContext;

        public AddNewPlayerRequestHandler(IMapper mapper, DepthChartContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<TryAsync<PlayerDto>> Handle(AddNewPlayerRequest request, CancellationToken token)
        {
            var newPlayer = _mapper.Map<Player>(request.NewPlayer);

            // Default until this woul be scalled out.
            newPlayer.Sport = SportType.NFL;

            return async () => await AddPlayerToDatabase(newPlayer, token);
        }

        private async Task<PlayerDto> AddPlayerToDatabase(Player player, CancellationToken token)
        {
            _dbContext.Players.Add(player);
            await _dbContext.SaveChangesAsync(token);

            return _mapper.Map<PlayerDto>(player);
        }
    }
}